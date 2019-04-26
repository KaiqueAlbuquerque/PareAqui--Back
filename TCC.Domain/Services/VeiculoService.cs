using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class VeiculoService : ServiceBase<Veiculo>, IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;

        public VeiculoService(IVeiculoRepository veiculoRepository)
            : base(veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        public void DisableAllVehicle(int idUsuario)
        {
            _veiculoRepository.DisableAllVehicle(idUsuario);
        }

        public void DisableVehicle(int idVeiculo)
        {
            _veiculoRepository.DisableVehicle(idVeiculo);
        }

        public IQueryable<Veiculo> GetByIdUsuario(int idUsuario)
        {
            return _veiculoRepository.GetByIdUsuario(idUsuario);
        }

        public int GetQtdVehicleActive()
        {
            return _veiculoRepository.GetQtdVehicleActive();
        }

        public int GetQtdVehicleActivePerMonth(int mes, int ano)
        {
            return _veiculoRepository.GetQtdVehicleActivePerMonth(mes, ano);
        }
    }
}
