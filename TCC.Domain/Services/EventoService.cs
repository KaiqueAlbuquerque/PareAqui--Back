using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class EventoService : ServiceBase<Evento>, IEventoService
    {
        private readonly IEventoRepository _eventoRepository;

        public EventoService(IEventoRepository eventoRepository)
            : base(eventoRepository)
        {
            _eventoRepository = eventoRepository;
        }

        public IQueryable<Evento> CheckIfEventAlreadyExists(string cep, int numero, DateTime dataInicio)
        {
            return _eventoRepository.CheckIfEventAlreadyExists(cep, numero, dataInicio);
        }

        public bool DisableEvent(int idEvento)
        {
            return _eventoRepository.DisableEvent(idEvento);
        }

        public List<Evento> GetByLatLong(double latitude, double longitude, double? distancia)
        {
            return _eventoRepository.GetByLatLong(latitude, longitude, distancia);
        }

        public IQueryable<Evento> GetEventsForApproval()
        {
            return _eventoRepository.GetEventsForApproval();
        }
    }
}
