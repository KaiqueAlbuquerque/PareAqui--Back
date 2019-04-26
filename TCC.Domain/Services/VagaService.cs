using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class VagaService : ServiceBase<Vaga>, IVagaService
    {
        private readonly IVagaRepository _vagaRepository;

        public VagaService(IVagaRepository vagaRepository)
            : base(vagaRepository)
        {
            _vagaRepository = vagaRepository;
        }

        public void DisableAllVacancy(int idGaragem)
        {
            _vagaRepository.DisableAllVacancy(idGaragem);
        }

        public void DisableVacancy(int idVaga)
        {
            _vagaRepository.DisableVacancy(idVaga);
        }

        public IQueryable<Vaga> GetAllByIdGarageAndIdUser(int idGaragem, int idUsuario)
        {
            return _vagaRepository.GetAllByIdGarageAndIdUser(idGaragem, idUsuario);
        }

        public IQueryable<Vaga> GetByIdGarage(int idGaragem)
        {
            return _vagaRepository.GetByIdGarage(idGaragem);
        }

        public IQueryable<Vaga> GetByIdGarageAndIdUser(int idGaragem, int idUsuario)
        {
            return _vagaRepository.GetByIdGarageAndIdUser(idGaragem, idUsuario);
        }

        public int GetQtdVacanciesActive()
        {
            return _vagaRepository.GetQtdVacanciesActive();
        }

        public int GetQtdVacanciesActivePerMonth(int mes, int ano)
        {
            return _vagaRepository.GetQtdVacanciesActivePerMonth(mes, ano);
        }

        public IQueryable<Vaga> GetVacanciesForApproval()
        {
            return _vagaRepository.GetVacanciesForApproval();
        }
    }
}
