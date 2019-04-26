using System.Collections.Generic;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class AvaliacaoService : ServiceBase<Avaliacao>, IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;

        public AvaliacaoService(IAvaliacaoRepository avaliacaoRepository)
            : base(avaliacaoRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
        }

        public List<Avaliacao> GetEvaluationUser(int idUsuario, bool comoLocador, bool comoLocatario)
        {
            return _avaliacaoRepository.GetEvaluationUser(idUsuario, comoLocador, comoLocatario);
        }
    }
}
