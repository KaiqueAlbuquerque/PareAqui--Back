using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Repositories
{
    public interface IDadosPagamentoRepository : IRepositoryBase<DadosPagamento>
    {
        List<DadosPagamento> GetByIdUser(int idUsuario);

        void DisableCard(int idDadosPagamento);
    }
}
