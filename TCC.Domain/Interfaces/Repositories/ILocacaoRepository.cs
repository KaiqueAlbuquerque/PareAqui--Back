using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface ILocacaoRepository : IRepositoryBase<Locacao>
    {
        IQueryable<Locacao> GetByIdVaga(int idVaga);

        IQueryable<Locacao> GetByIdVeiculo(int idVeiculo);

        int GetQtdLocationActive();

        int GetQtdLocationActivePerMonth(int mes, int ano);
    }
}
