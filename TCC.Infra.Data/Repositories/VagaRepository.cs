using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class VagaRepository : RepositoryBase<Vaga>, IVagaRepository
    {
        public void DisableVacancy(int idVaga)
        {
            var vaga = Db.Vagas.Where(v => v.VagaId == idVaga).FirstOrDefault();
            vaga.Ativo = false;
            vaga.Desativa = true;
            Db.SaveChanges();
        }

        public void DisableAllVacancy(int idGaragem)
        {
            var vagas = Db.Vagas.Where(v => v.GaragemId == idGaragem).ToList();
            foreach (Vaga v in vagas)
            {
                v.Ativo = false;
                v.Desativa = true;
            }
            Db.SaveChanges();
        }

        public IQueryable<Vaga> GetAllByIdGarageAndIdUser(int idGaragem, int idUsuario)
        {
            IQueryable<Vaga> vagas = Db.Vagas.Where(v => v.GaragemId == idGaragem && v.UsuarioId == idUsuario && v.Garagem.Ativo != false && v.Desativa != true);
            return vagas;
        }

        public IQueryable<Vaga> GetByIdGarage(int idGaragem)
        {
            IQueryable<Vaga> vagas = Db.Vagas.Where(v => v.GaragemId == idGaragem && v.Ativo == true);
            return vagas;
        }

        public IQueryable<Vaga> GetByIdGarageAndIdUser(int idGaragem, int idUsuario)
        {
            IQueryable<Vaga> vagas = Db.Vagas.Where(v => v.GaragemId == idGaragem && v.Ativo == true && v.UsuarioId == idUsuario);
            return vagas;
        }

        public IQueryable<Vaga> GetVacanciesForApproval()
        {
            IQueryable<Vaga> vagas = Db.Vagas.Where(v => v.Aceita == null).OrderBy(v => v.VagaId);
            return vagas;
        }

        public int GetQtdVacanciesActive()
        {
            return Db.Vagas.Where(v => v.Ativo == true).Count();
        }

        public int GetQtdVacanciesActivePerMonth(int mes, int ano)
        {
            return Db.Vagas.Where(v => v.DataCadastro.Month == mes && v.DataCadastro.Year == ano && v.Aceita == true).Count();
        }
    }
}
