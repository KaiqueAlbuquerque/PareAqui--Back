using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class LocacaoService : ServiceBase<Locacao>, ILocacaoService
    {
        private readonly ILocacaoRepository _locacaoRepository;

        public LocacaoService(ILocacaoRepository locacaoRepository) 
            : base(locacaoRepository)
        {
            _locacaoRepository = locacaoRepository;
        }

        public IQueryable<Locacao> GetByIdVaga(int idVaga)
        {
            return _locacaoRepository.GetByIdVaga(idVaga);
        }

        public IQueryable<Locacao> GetByIdVeiculo(int idVeiculo)
        {
            return _locacaoRepository.GetByIdVeiculo(idVeiculo);
        }

        public int GetQtdLocationActive()
        {
            return _locacaoRepository.GetQtdLocationActive();
        }

        public int GetQtdLocationActivePerMonth(int mes, int ano)
        {
            return _locacaoRepository.GetQtdLocationActivePerMonth(mes, ano);
        }
    }
}
