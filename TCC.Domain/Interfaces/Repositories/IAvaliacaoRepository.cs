using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IAvaliacaoRepository : IRepositoryBase<Avaliacao>
    {
        List<Avaliacao> GetEvaluationUser(int idUsuario, bool comoLocador, bool comoLocatario);
    }
}
