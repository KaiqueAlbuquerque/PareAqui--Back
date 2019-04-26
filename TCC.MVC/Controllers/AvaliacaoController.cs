using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly ILocacaoService _locacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService,
                                   ILocacaoService locacaoService
                                   )
        {
            _avaliacaoService = avaliacaoService;
            _locacaoService = locacaoService;
        }

        [HttpPost]
        public string CriarAvaliacao(AvaliacaoViewModel avaliacaoViewModel)
        {
            try
            {
                if (ValidaDadosDeEntrada(avaliacaoViewModel) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(avaliacaoViewModel) });
                }
                else
                {
                    var avaliacaoService = Mapper.Map<AvaliacaoViewModel, Avaliacao>(avaliacaoViewModel);

                    var locacao = _locacaoService.GetById(avaliacaoService.LocacaoId);

                    if ((locacao.Aprovada == true || locacao.Cancelada == true) && locacao.Ativo == false)
                    {
                        _avaliacaoService.Add(avaliacaoService);

                        return JsonConvert.SerializeObject(new { code = 200, message = "Avaliação criada com Sucesso.", id = avaliacaoService.AvaliacaoId });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Uma locação só pode ser avaliada quando a mesma tiver sido finalizada." });
                    }
                }                
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao realizar a avaliação. Por favor, tente novamente." });
            }
        }

        public string ConsultarAvaliacoesUsuario(int idUsuario, bool comoLocador, bool comoLocatario)
        {
            try
            {
                List<Avaliacao> avaliacoes = new List<Avaliacao>();

                if(comoLocador && comoLocatario || !comoLocador && !comoLocatario)
                {
                    avaliacoes = _avaliacaoService.GetEvaluationUser(idUsuario, true, true);
                }
                else
                {
                    avaliacoes = _avaliacaoService.GetEvaluationUser(idUsuario, comoLocador, comoLocatario);
                }

                return JsonConvert.SerializeObject(new { code = 200, aval = avaliacoes });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar as avaliações. Por favor, tente novamente." });
            }
        }

        #region Métodos privados
        private string ValidaDadosDeEntrada(AvaliacaoViewModel avaliacaoViewModel)
        {
            if(string.IsNullOrEmpty(avaliacaoViewModel.Nota))
            {
                return "Favor informar a nota que você dá para o usuário.";
            }
            else
            {
                int n = -1;
                bool teste = int.TryParse(avaliacaoViewModel.Nota, out n);

                if(!teste)
                {
                    return "O Campo nota deve ser um número de 0 a 5 cujo o mesmo será o valor da nota que você dá para o usuário.";
                }
            }

            if(avaliacaoViewModel.Comentario.Length > 250)
            {
                return "O limite máximo de caracteres em seu comentário é de 250 caracteres.";
            }

            return "";
        }
        #endregion
    }
}