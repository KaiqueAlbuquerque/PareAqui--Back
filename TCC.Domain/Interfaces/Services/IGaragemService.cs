using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IGaragemService : IServiceBase<Garagem>
    {
        List<Garagem> GetByLatLong(double latitude, double longitude, double? distancia);

        List<Garagem> GetByIdUser(int idUsuario);

        bool CheckIfGarageAlreadyExists(string cep, int numeroRua);

        int IdGarageIfAlreadyExists(string cep, int numeroRua);

        void DisableGarage(int idGaragem);

        void DisableAllGarage(int idUsuario);
    }
}
