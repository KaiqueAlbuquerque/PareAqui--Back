using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class VeiculoRepository : RepositoryBase<Veiculo>, IVeiculoRepository
    {
        public void DisableVehicle(int idVeiculo)
        {
            var veiculo = Db.Veiculos.Where(v => v.VeiculoId == idVeiculo).FirstOrDefault();
            veiculo.Ativo = false;
            Db.SaveChanges();
        }

        public IQueryable<Veiculo> GetByIdUsuario(int idUsuario)
        {
            IQueryable<Veiculo> veiculos = Db.Veiculos.Where(v => v.UsuarioId == idUsuario && v.Ativo == true);
            return veiculos;
        }

        public void DisableAllVehicle(int idUsuario)
        {
            var veiculos = Db.Veiculos.Where(v => v.UsuarioId == idUsuario).ToList();
            foreach (Veiculo v in veiculos)
            {
                v.Ativo = false;
            }
            Db.SaveChanges();
        }

        public int GetQtdVehicleActive()
        {
            return Db.Veiculos.Where(v => v.Ativo == true).Count();
        }

        public int GetQtdVehicleActivePerMonth(int mes, int ano)
        {
            return Db.Veiculos.Where(v => v.DataCadastro.Month == mes && v.DataCadastro.Year == ano).Count();
        }
    }
}
