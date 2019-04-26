using System.Collections.Generic;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IAvaliacaoService : IServiceBase<Avaliacao>
    {
        List<Avaliacao> GetEvaluationUser(int idUsuario, bool comoLocador, bool comoLocatario);
    }
}
