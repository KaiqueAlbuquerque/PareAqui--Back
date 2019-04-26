using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class VeiculoController : Controller
    {
        private readonly IVeiculoService _veiculoService;
        private readonly ILocacaoService _locacaoService;

        public VeiculoController(IVeiculoService veiculoService, ILocacaoService locacaoService)
        {
            _veiculoService = veiculoService;
            _locacaoService = locacaoService;
        }

        [HttpPost]
        public string CadastrarVeiculo(VeiculoViewModel veiculo)
        {
            try
            {
                if(ValidaDadosDeEntrada(veiculo) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(veiculo) });
                }
                else
                {
                    veiculo.Ativo = true;

                    var veiculoService = Mapper.Map<VeiculoViewModel, Veiculo>(veiculo);
                    _veiculoService.Add(veiculoService);

                    return JsonConvert.SerializeObject(new { code = 200, message = "Veículo cadastrado com sucesso.", id = veiculoService.VeiculoId });
                }                
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro no cadastro. Por favor, tente novamente." });
            }
        }

        public string ConsultarVeiculos(int idUsuario)
        {
            try
            {
                var veiculos = _veiculoService.GetByIdUsuario(idUsuario);
                var Itens = new List<object>();

                foreach (Veiculo v in veiculos)
                {
                    var veiculoViewModel = Mapper.Map<Veiculo, VeiculoViewModel>(v);
                    Itens.Add(veiculoViewModel);
                }
                return JsonConvert.SerializeObject(new { code = 200, veiculo = Itens });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarVeiculo(VeiculoViewModel veiculo)
        {
            try
            {
                if (ValidaDadosDeEntrada(veiculo) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(veiculo) });
                }
                else
                {
                    veiculo.Ativo = true;
                    var veiculoService = Mapper.Map<VeiculoViewModel, Veiculo>(veiculo);
                    _veiculoService.Update(veiculoService);
                    return JsonConvert.SerializeObject(new { code = 200, message = "Veículo alterado com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar. Por favor, tente novamente." });
            }
        }
        
        [HttpPost]
        public string DesativarVeiculo(int idVeiculo)
        {
            try
            {
                var veiculo = _veiculoService.GetById(idVeiculo);

                if(!(VerificaSePossuiGaragemOcupada(veiculo)))
                {
                    _veiculoService.DisableVehicle(idVeiculo);
                    return JsonConvert.SerializeObject(new { code = 200, message = "Veiculo desativado com sucesso." });
                }

                return JsonConvert.SerializeObject(new { code = 400, message = "Você não pode desativar este veiculo pois ele está alugando uma vaga no momento." });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar. Por favor tente novamente." });
            }
        }

        #region Métodos privados
        private bool VerificaSePossuiGaragemOcupada(Veiculo veiculo)
        {
            bool ocupada = false;
            var loc = _locacaoService.GetByIdVeiculo(veiculo.VeiculoId);

            foreach (var l in loc)
            {
                if (l.Ativo)
                {
                    ocupada = true;
                }
            }

            return ocupada;
        }

        private string ValidaDadosDeEntrada(VeiculoViewModel veiculo)
        {
            if(string.IsNullOrEmpty(veiculo.Placa))
            {
                return "Por favor, informe a placa do veículo.";
            }
            if(veiculo.Placa.Length > 7)
            {
                return "A placa só pode conter 7 caracteres.";
            }

            Regex regex = new Regex(@"^[a-zA-Z]{3}\d{4}$");

            if (!regex.IsMatch(veiculo.Placa))
            {
                return "Placa inválida.";
            }
            
            if (string.IsNullOrEmpty(veiculo.Cor))
            {
                return "Por favor, informe a cor do veículo.";
            }

            return "";
        }
        #endregion
    }
}