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
    public class DadosPagamentoController : Controller
    {
        private readonly IDadosPagamentoService _dadosPagamentoService;
        private readonly IPagamentoService _pagamentoService;


        public DadosPagamentoController(IDadosPagamentoService dadosPagamentoService,
                                        IPagamentoService pagamentoService
                                        )
        {
            _dadosPagamentoService = dadosPagamentoService;
            _pagamentoService = pagamentoService;
        }

        [HttpPost]
        public string CadastrarDadosPagamento(DadosPagamentoViewModel dadosPagamento)
        {
            try
            {
                if (ValidaDadosDeEntrada(dadosPagamento) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(dadosPagamento) });
                }
                else
                {
                    var dadosPagamentoService = Mapper.Map<DadosPagamentoViewModel, DadosPagamento>(dadosPagamento);
                    dadosPagamentoService.Ativo = true;

                    _dadosPagamentoService.Add(dadosPagamentoService);

                    return JsonConvert.SerializeObject(new { code = 200, message = "Cartão cadastrado com Sucesso.", id = dadosPagamentoService.DadosPagamentoId });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao cadastrar o cartão. Por favor, tente novamente." });
            }
        }
        
        public string ConsultarDadosPagamentoPorIdUsuario(int idUsuario)
        {
            try
            {
                var dadosPagamento = _dadosPagamentoService.GetByIdUser(idUsuario);

                return JsonConvert.SerializeObject(new { code = 200, dadosPag = dadosPagamento });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar os cartões. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarDadosPagamento(DadosPagamentoViewModel dadosPagamento)
        {
            try
            {
                if (ValidaDadosDeEntrada(dadosPagamento) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(dadosPagamento) });
                }
                else
                {
                    dadosPagamento.Ativo = true;
                    var dadosPagamentoService = Mapper.Map<DadosPagamentoViewModel, DadosPagamento>(dadosPagamento);

                    _dadosPagamentoService.Update(dadosPagamentoService);
                    return JsonConvert.SerializeObject(new { code = 200, message = "Dados de pagamento alterados com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar os dados do cartão. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string DesativarDadosPagamento(int idDadosPagamento)
        {
            try
            {
                int qtdePagamentos = _pagamentoService.GetAll().Where(p => p.DadosPagamentoId == idDadosPagamento && p.DataEfetivado == null && p.Aprovado == true).Count();

                if(qtdePagamentos > 0)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Existem locações ativas no momento, que utilizarão este cartão como meio de pagamento, portanto você não pode desativa-lo." });
                }
                else
                {
                    _dadosPagamentoService.DisableCard(idDadosPagamento);
                    return JsonConvert.SerializeObject(new { code = 200, message = "Dados de pagamento desativado com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar o cartão. Por favor, tente novamente." });
            }
        }

        #region Métodos Privador
        private string ValidaDadosDeEntrada(DadosPagamentoViewModel dadosPagamento)
        {
            if(string.IsNullOrEmpty(dadosPagamento.Bandeira))
            {
                return "Favor informar a bandeira do cartão.";
            }
            if(string.IsNullOrEmpty(dadosPagamento.NumeroCartao))
            {
                return "Favor informar o número do cartão.";
            }
            if(dadosPagamento.NumeroCartao.Length > 16)
            {
                return "O número do cartão não pode ter mais do que 16 digitos.";
            }
            if(string.IsNullOrEmpty(dadosPagamento.Cvv))
            {
                return "Favor informar o CVV.";
            }
            if(string.IsNullOrEmpty(dadosPagamento.NomeNoCartao))
            {
                return "Favor informar o nome do proprietário do cartão.";
            }
            if(string.IsNullOrEmpty(dadosPagamento.Cpf))
            {
                return "Favor informar o número do CPF do titular do cartão.";
            }
            else
            {
                var cpf = dadosPagamento.Cpf;
                int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                string tempCpf;
                string digito;
                int soma;
                int resto;

                cpf = cpf.Trim();
                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11)
                {
                    return "O CPF não é valido.";
                }

                for(var i=0; i<=9; i++)
                {
                    var referencia = 11111111111;

                    if((i*referencia).ToString() == cpf)
                    {
                        return "O CPF não é valido.";
                    }
                }

                tempCpf = cpf.Substring(0, 9);
                soma = 0;

                for (int i = 0; i < 9; i++)
                {
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
                }
                    
                resto = soma % 11;

                if (resto < 2)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }
                    
                digito = resto.ToString();
                tempCpf = tempCpf + digito;
                soma = 0;

                for (int i = 0; i < 10; i++)
                {
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
                }

                resto = soma % 11;

                if (resto < 2)
                {
                    resto = 0;
                }
                else
                {
                    resto = 11 - resto;
                }
                    
                digito = digito + resto.ToString();

                if(!cpf.EndsWith(digito))
                {
                    return "CPF inválido.";
                }
            }
            if(string.IsNullOrEmpty(dadosPagamento.Nascimento))
            {
                return "Favor informar a data de vencimento do cartão.";
            }

            return "";
        }
        #endregion
    }
}