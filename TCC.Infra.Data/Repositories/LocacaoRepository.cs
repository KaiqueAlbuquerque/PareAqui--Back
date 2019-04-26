using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class LocacaoRepository : RepositoryBase<Locacao>, ILocacaoRepository
    {
        public IQueryable<Locacao> GetByIdVaga(int idVaga)
        {
            IQueryable<Locacao> locacoes = from l in Db.Locacoes where l.VagaId == idVaga select l;
            return locacoes;
        }

        public IQueryable<Locacao> GetByIdVeiculo(int idVeiculo)
        {

            IQueryable<Locacao> locacoes = from l in Db.Locacoes where l.VeiculoId == idVeiculo select l;
            return locacoes;
        }

        public int GetQtdLocationActive()
        {
            return Db.Locacoes.Where(l => l.Ativo == true).Count();
        }

        public int GetQtdLocationActivePerMonth(int mes, int ano)
        {
            return Db.Locacoes.Where(l => l.DiaHoraInicio.Month == mes && l.DiaHoraInicio.Year == ano && l.Aprovada == true && l.Cancelada == false).Count();
        }
    }
}
