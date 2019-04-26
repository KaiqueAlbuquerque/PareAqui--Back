using System.Linq;
using TCC.Domain.Entities;

namespace TCC.Domain.Interfaces.Services
{
    public interface IVagaService : IServiceBase<Vaga>
    {
        IQueryable<Vaga> GetAllByIdGarageAndIdUser(int idGaragem, int idUsuario);

        IQueryable<Vaga> GetByIdGarage(int idGaragem);

        IQueryable<Vaga> GetByIdGarageAndIdUser(int idGaragem, int idUsuario);

        void DisableVacancy(int idVaga);

        void DisableAllVacancy(int idGaragem);

        IQueryable<Vaga> GetVacanciesForApproval();

        int GetQtdVacanciesActive();

        int GetQtdVacanciesActivePerMonth(int mes, int ano);
    }
}
