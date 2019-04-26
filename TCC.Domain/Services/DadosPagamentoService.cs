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
    public class DadosPagamentoService : ServiceBase<DadosPagamento>, IDadosPagamentoService
    {
        private readonly IDadosPagamentoRepository _dadosPagamentoRepository;

        public DadosPagamentoService(IDadosPagamentoRepository dadosPagamentoRepository)
            : base(dadosPagamentoRepository)
        {
            _dadosPagamentoRepository = dadosPagamentoRepository;
        }

        public void DisableCard(int idDadosPagamento)
        {
            _dadosPagamentoRepository.DisableCard(idDadosPagamento);
        }

        public List<DadosPagamento> GetByIdUser(int idUsuario)
        {
            return _dadosPagamentoRepository.GetByIdUser(idUsuario);
        }
    }
}
