using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IFotoRepository : IRepositoryBase<Foto>
    {
        IQueryable<Foto> GetPhotosByIdVacancy(int idVaga);

        IQueryable<Foto> GetPhotosByIdEvent(int idEvento);
    }
}
