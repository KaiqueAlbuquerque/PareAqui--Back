using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface ILocacaoService : IServiceBase<Locacao>
    {
        IQueryable<Locacao> GetByIdVaga(int idVaga);

        IQueryable<Locacao> GetByIdVeiculo(int idVeiculo);

        int GetQtdLocationActive();

        int GetQtdLocationActivePerMonth(int mes, int ano);
    }
}
