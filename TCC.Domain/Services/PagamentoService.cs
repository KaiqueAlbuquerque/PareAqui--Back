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
    public class PagamentoService : ServiceBase<Pagamento>, IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;

        public PagamentoService(IPagamentoRepository pagamentoRepository)
            : base(pagamentoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
        }
    }
}
