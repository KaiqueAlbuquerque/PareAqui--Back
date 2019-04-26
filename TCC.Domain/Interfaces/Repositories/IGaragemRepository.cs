using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IGaragemRepository : IRepositoryBase<Garagem>
    {
        bool CheckIfGarageAlreadyExists(string cep, int numeroRua);

        List<Garagem> GetByIdUser(int idUsuario);

        int IdGarageIfAlreadyExists(string cep, int numeroRua);

        void DisableGarage(int idGaragem);

        void DisableAllGarage(int idUsuario);

        List<Garagem> GetByLatLong(double latitude, double longitude, double? distancia);
    }
}
