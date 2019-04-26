using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGaragemService _garagemService;
        private readonly IVagaService _vagaService;
        private readonly ILocacaoService _locacaoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IVeiculoService _veiculoService;
        private readonly IEventoService _eventoService;

        public HomeController(IGaragemService garagemService,
                              IVagaService vagaService,
                              ILocacaoService locacaoService,
                              IUsuarioService usuarioService,
                              IVeiculoService veiculoService,
                              IEventoService eventoService
                              )
        {
            _garagemService = garagemService;
            _vagaService = vagaService;
            _locacaoService = locacaoService;
            _usuarioService = usuarioService;
            _veiculoService = veiculoService;
            _eventoService = eventoService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string ConsultarDadosDashBoard()
        {
            try
            {                
                DashBoardViewModel dbvm = new DashBoardViewModel();

                dbvm.QtdeUsuariosAtivos = _usuarioService.GetQtdUsersActive();
                dbvm.QtdeVagasAtivas = _vagaService.GetQtdVacanciesActive();
                dbvm.QtdeVeiculosAtivos = _veiculoService.GetQtdVehicleActive();
                dbvm.QteLocacoes = _locacaoService.GetQtdLocationActive();

                dbvm.QtdeVagasPendentes = _vagaService.GetVacanciesForApproval().Count();
                dbvm.QtdeEventosPendentes = _eventoService.GetEventsForApproval().Count();

                DateTime dataAtual = DateTime.Today;

                dbvm.ArrMeses = new string[12];
                dbvm.ArrValoresUsuarios = new int[12];
                dbvm.ArrValoresVagas = new int[12];
                dbvm.ArrValoresVeiculos = new int[12];
                dbvm.ArrValoresLocacoes = new int[12];

                var m = 0;

                for(var i=11; i>=0; i--)
                {
                    string nomeMes = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(dataAtual.AddMonths(m).Month);
                    dbvm.ArrMeses[i] = nomeMes + " de " + dataAtual.AddMonths(m).Year;

                    dbvm.ArrValoresUsuarios[i] = _usuarioService.GetQtdUsersActivePerMonth(dataAtual.AddMonths(m).Month, dataAtual.AddMonths(m).Year);
                    dbvm.ArrValoresVagas[i] = _vagaService.GetQtdVacanciesActivePerMonth(dataAtual.AddMonths(m).Month, dataAtual.AddMonths(m).Year);
                    dbvm.ArrValoresVeiculos[i] = _veiculoService.GetQtdVehicleActivePerMonth(dataAtual.AddMonths(m).Month, dataAtual.AddMonths(m).Year);
                    dbvm.ArrValoresLocacoes[i] = _locacaoService.GetQtdLocationActivePerMonth(dataAtual.AddMonths(m).Month, dataAtual.AddMonths(m).Year);

                    m--;
                }

                return JsonConvert.SerializeObject(new { code = 200, dashboard = dbvm });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar os dados. Por favor, tente novamente." });
            }
        }
    }
}