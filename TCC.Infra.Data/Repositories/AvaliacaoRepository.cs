using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class AvaliacaoRepository : RepositoryBase<Avaliacao>, IAvaliacaoRepository
    {
        public List<Avaliacao> GetEvaluationUser(int idUsuario, bool comoLocador, bool comoLocatario)
        {
            List<Avaliacao> avaliacoes = new List<Avaliacao>();

            if(comoLocador)
            {
                IQueryable<Avaliacao> avaliacao = from a in Db.Avaliacoes where a.Locacao.Vaga.UsuarioId == idUsuario && a.UsuarioAvaliadoId == idUsuario select a;

                foreach(var a in avaliacao)
                {
                    avaliacoes.Add(a);
                }
            }
            if(comoLocatario)
            {
                IQueryable<Avaliacao> avaliacao = from a in Db.Avaliacoes where a.Locacao.Veiculo.UsuarioId == idUsuario && a.UsuarioAvaliadoId == idUsuario select a;

                foreach (var a in avaliacao)
                {
                    avaliacoes.Add(a);
                }
            }

            return avaliacoes;
        }
    }
}
