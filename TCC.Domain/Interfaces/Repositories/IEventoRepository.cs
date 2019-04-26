using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IEventoRepository : IRepositoryBase<Evento>
    {
        IQueryable<Evento> CheckIfEventAlreadyExists(string cep, int numero, DateTime dataInicio);

        bool DisableEvent(int idEvento);

        List<Evento> GetByLatLong(double latitude, double longitude, double? distancia);

        IQueryable<Evento> GetEventsForApproval();
    }
}
