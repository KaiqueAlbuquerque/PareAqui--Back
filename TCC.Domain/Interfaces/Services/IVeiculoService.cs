using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IVeiculoService : IServiceBase<Veiculo>
    {
        IQueryable<Veiculo> GetByIdUsuario(int idUsuario);

        void DisableVehicle(int idVeiculo);

        void DisableAllVehicle(int idUsuario);

        int GetQtdVehicleActive();

        int GetQtdVehicleActivePerMonth(int mes, int ano);
    }
}
