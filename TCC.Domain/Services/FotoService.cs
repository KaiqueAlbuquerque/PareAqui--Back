using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class FotoService : ServiceBase<Foto>, IFotoService
    {
        private readonly IFotoRepository _fotoRepository;

        public FotoService(IFotoRepository fotoRepository)
            : base(fotoRepository)
        {
            _fotoRepository = fotoRepository;
        }

        public IQueryable<Foto> GetPhotosByIdVacancy(int idVaga)
        {
            return _fotoRepository.GetPhotosByIdVacancy(idVaga);
        }

        public IQueryable<Foto> GetPhotosByIdEvent(int idEvento)
        {
            return _fotoRepository.GetPhotosByIdEvent(idEvento);
        }
    }
}
