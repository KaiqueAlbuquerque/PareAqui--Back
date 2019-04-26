using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class EventoRepository : RepositoryBase<Evento>, IEventoRepository
    {
        public IQueryable<Evento> CheckIfEventAlreadyExists(string cep, int numero, DateTime dataInicio)
        {
            var evento = Db.Eventos.AsNoTracking().Where(e => e.Cep == cep && e.Numero == numero && e.Ativo == true && e.DataHoraInicio.Day == dataInicio.Day && e.DataHoraInicio.Month == dataInicio.Month && e.DataHoraInicio.Year == dataInicio.Year);
            return evento;
        }

        public bool DisableEvent(int idEvento)
        {
            var evento = Db.Eventos.AsNoTracking().Where(e => e.EventoId == idEvento).FirstOrDefault();
            evento.Ativo = false;
            return true;
        }

        public List<Evento> GetByLatLong(double latitude, double longitude, double? distancia)
        {
            List<Evento> eventos = new List<Evento>();

            using (SqlConnection conn = new SqlConnection())
            {
                //Ubik
                //conn.ConnectionString = "Data Source=localhost;Initial Catalog=PareAqui;Integrated Security=SSPI;MultipleActiveResultSets=True";

                //Local Kaique
                //conn.ConnectionString = "Data Source=.\\SQLEXPRESS01; Initial Catalog = PareAqui; Integrated Security = SSPI; MultipleActiveResultSets = True";

                //Amazon
                conn.ConnectionString = "Data Source=pareaquidb.cb6prssuzkam.sa-east-1.rds.amazonaws.com; Initial Catalog = PareAqui; Integrated Security = False;User Id=admin;Password=tccpareaqui; MultipleActiveResultSets = True";

                conn.Open();

                SqlCommand command = new SqlCommand(
                                                    "SELECT EventoId, " +
                                                    "( 6371 * acos( cos( radians(@latitude) ) " +
                                                    "* cos( radians( Latitude ) ) " +
                                                    "* cos( radians( Longitude ) - radians(@longitude) ) + sin( radians(@latitude) ) " +
                                                    "* sin(radians(Latitude)) ) ) AS distance " +
                                                    "FROM Evento " +
                                                    "Where 6371 * acos( cos( radians(@latitude) ) " +
                                                    "* cos( radians( Latitude ) ) " +
                                                    "* cos( radians( Longitude ) - radians(@longitude) ) + sin( radians(@latitude) ) " +
                                                    "* sin(radians(Latitude)) ) < @distancia " +
                                                    "ORDER BY 6371 * acos( cos( radians(@latitude) ) " +
                                                    "* cos( radians( Latitude ) ) " +
                                                    "* cos( radians( Longitude ) - radians(@longitude) ) + sin( radians(@latitude) ) " +
                                                    "* sin(radians(Latitude)) )", conn
                                                    );

                command.Parameters.Add(new SqlParameter("latitude", latitude));
                command.Parameters.Add(new SqlParameter("longitude", longitude));
                command.Parameters.Add(new SqlParameter("distancia", distancia == null ? 1 : distancia));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ev = GetById(int.Parse(reader[0].ToString()));
                        eventos.Add(ev);
                    }
                }

                conn.Close();

                return eventos;
            }
        }

        public IQueryable<Evento> GetEventsForApproval()
        {
            IQueryable<Evento> eventos = Db.Eventos.Where(e => e.Aprovado == null).OrderBy(e => e.Aprovado);
            return eventos;
        }
    }
}
