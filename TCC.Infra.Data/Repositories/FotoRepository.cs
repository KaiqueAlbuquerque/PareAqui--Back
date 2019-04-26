using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class FotoRepository : RepositoryBase<Foto>, IFotoRepository
    {
        public IQueryable<Foto> GetPhotosByIdVacancy(int idVaga)
        {
            var fotos = Db.Fotos.Where(f => f.VagaId == idVaga);
            return fotos;
        }

        public IQueryable<Foto> GetPhotosByIdEvent(int idEvento)
        {
            var fotos = Db.Fotos.Where(f => f.EventoId == idEvento);
            return fotos;
        }
    }
}
