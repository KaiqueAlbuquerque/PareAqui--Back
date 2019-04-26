using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class GaragemRepository : RepositoryBase<Garagem>, IGaragemRepository
    {
        public bool CheckIfGarageAlreadyExists(string cep, int numeroRua)
        {
            var garagem = Db.Garagens.AsNoTracking().Where(g => g.Cep == cep && g.NumeroRua == numeroRua).FirstOrDefault();
            if(garagem != null)
            {
                return true;
            }
            
            return false;
        }

        public void DisableGarage(int idGaragem)
        {
            var garagem = Db.Garagens.Where(v => v.GaragemId == idGaragem).FirstOrDefault();
            garagem.Ativo = false;
            Db.SaveChanges();
        }

        public void DisableAllGarage(int idUsuario)
        {
            var idsGaragens = Db.Vagas.Where(v => v.UsuarioId == idUsuario && v.Garagem.Condominio == false).Select(v => v.GaragemId).Distinct();
                        
            foreach(int ids in idsGaragens)
            {
                var garagem = Db.Garagens.Where(g => g.GaragemId == ids).FirstOrDefault();
                garagem.Ativo = false;
            }
            Db.SaveChanges();
        }

        public List<Garagem> GetByIdUser(int idUsuario)
        {
            List<Garagem> garagens = new List<Garagem>();
            var idsGaragens = Db.Vagas.Where(v => v.UsuarioId == idUsuario && v.Aceita != false).Select(v => v.GaragemId).Distinct();

            foreach (int ids in idsGaragens)
            {
                IQueryable<Garagem> gar = from g in Db.Garagens where g.GaragemId == ids select g;

                foreach(var garagem in gar)
                {
                    garagens.Add(garagem);
                }
            }
            return garagens;
        }

        public int IdGarageIfAlreadyExists(string cep, int numeroRua)
        {
            var garagem = Db.Garagens.AsNoTracking().Where(g => g.Cep == cep && g.NumeroRua == numeroRua).FirstOrDefault();
            return garagem.GaragemId;
        }

        public List<Garagem> GetByLatLong(double latitude, double longitude, double? distancia)
        {
            List<Garagem> garagens = new List<Garagem>();

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
                                                    "SELECT GaragemId, " +
                                                    "( 6371 * acos( cos( radians(@latitude) ) " +
                                                    "* cos( radians( Latitude ) ) " +
                                                    "* cos( radians( Longitude ) - radians(@longitude) ) + sin( radians(@latitude) ) " +
                                                    "* sin(radians(Latitude)) ) ) AS distance " +
                                                    "FROM Garagem " +
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
                        var gar = GetById(int.Parse(reader[0].ToString()));
                        garagens.Add(gar);
                    }
                }
                
                conn.Close();

                return garagens;
            }
        }
    }
}
