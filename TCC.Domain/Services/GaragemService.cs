using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class GaragemService : ServiceBase<Garagem>, IGaragemService
    {
        private readonly IGaragemRepository _garagemRepository;

        public GaragemService(IGaragemRepository garagemRepository)
            : base (garagemRepository)
        {
            _garagemRepository = garagemRepository;
        }

        public bool CheckIfGarageAlreadyExists(string cep, int numeroRua)
        {
            return _garagemRepository.CheckIfGarageAlreadyExists(cep, numeroRua);
        }

        public void DisableAllGarage(int idUsuario)
        {
            _garagemRepository.DisableAllGarage(idUsuario);
        }

        public void DisableGarage(int idGaragem)
        {
            _garagemRepository.DisableGarage(idGaragem);
        }

        public List<Garagem> GetByIdUser(int idUsuario)
        {
            return _garagemRepository.GetByIdUser(idUsuario);
        }

        public int IdGarageIfAlreadyExists(string cep, int numeroRua)
        {
            return _garagemRepository.IdGarageIfAlreadyExists(cep, numeroRua);
        }

        public List<Garagem> GetByLatLong(double latitude, double longitude, double? distancia)
        {
            return _garagemRepository.GetByLatLong(latitude, longitude, distancia);
        }
    }
}
