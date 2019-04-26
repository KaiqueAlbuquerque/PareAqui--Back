using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IVeiculoRepository : IRepositoryBase<Veiculo>
    {
        IQueryable<Veiculo> GetByIdUsuario(int idUsuario);

        void DisableVehicle(int idVeiculo);

        void DisableAllVehicle(int idUsuario);

        int GetQtdVehicleActive();

        int GetQtdVehicleActivePerMonth(int mes, int ano);
    }
}
