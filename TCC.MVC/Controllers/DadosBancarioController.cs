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
    public class DadosBancarioController : Controller
    {
        private readonly IDadosBancarioService _dadosBancarioService;
        private readonly ITransferenciaService _transferenciaService;


        public DadosBancarioController(IDadosBancarioService dadosBancarioService,
                                        ITransferenciaService transferenciaService
                                        )
        {
            _dadosBancarioService = dadosBancarioService;
            _transferenciaService = transferenciaService;
        }

        [HttpPost]
        public string CadastrarDadosBancario(DadosBancarioViewModel dadosBancario)
        {
            try
            {
                if (ValidaDadosDeEntrada(dadosBancario) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(dadosBancario) });
                }
                else
                {
                    var dadosBancarioService = Mapper.Map<DadosBancarioViewModel, DadosBancario>(dadosBancario);
                    dadosBancarioService.Ativo = true;

                    _dadosBancarioService.Add(dadosBancarioService);

                    return JsonConvert.SerializeObject(new { code = 200, message = "Conta cadastrada com sucesso.", id = dadosBancarioService.DadosBancarioId });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao cadastrar a conta. Por favor, tente novamente." });
            }
        }

        public string ConsultarDadosBancarioPorIdUsuario(int idUsuario)
        {
            try
            {
                var dadosBancario = _dadosBancarioService.GetByIdUser(idUsuario);

                return JsonConvert.SerializeObject(new { code = 200, dadosPag = dadosBancario });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar os dados Bancários. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarDadosBancario(DadosBancarioViewModel dadosBancario)
        {
            try
            {
                if (ValidaDadosDeEntrada(dadosBancario) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(dadosBancario) });
                }
                else
                {
                    dadosBancario.Ativo = true;
                    var dadosBancarioService = Mapper.Map<DadosBancarioViewModel, DadosBancario>(dadosBancario);

                    _dadosBancarioService.Update(dadosBancarioService);
                    return JsonConvert.SerializeObject(new { code = 200, message = "Conta alterada com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar os dados bancários. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string DesativarDadosBancario(int idDadosBancario)
        {
            try
            {
                int qtdeTransferencias = _transferenciaService.GetAll().Where(p => p.DadosBancarioId == idDadosBancario && p.DataPagamento == null).Count();

                if (qtdeTransferencias > 0)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Existe(m) pagamento(s) a serem feitos para esta conta, portanto você não pode desativa-la." });
                }
                else
                {
                    _dadosBancarioService.DisableAccount(idDadosBancario);
                    return JsonConvert.SerializeObject(new { code = 200, message = "Dados bancário desativado com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar os dados bancário. Por favor, tente novamente." });
            }
        }

        #region Métodos Privados
        private string ValidaDadosDeEntrada(DadosBancarioViewModel dadosBancario)
        {
            if (string.IsNullOrEmpty(dadosBancario.NomeBanco))
            {
                return "Favor informar o nome do banco.";
            }
            if (string.IsNullOrEmpty(dadosBancario.Agencia))
            {
                return "Favor informar o número da agência.";
            }            
            if (string.IsNullOrEmpty(dadosBancario.NumeroConta))
            {
                return "Favor informar o número da conta.";
            }
            if (string.IsNullOrEmpty(dadosBancario.NomeDonoConta))
            {
                return "Favor informar o Nome do Titular da conta.";
            }
            if (string.IsNullOrEmpty(dadosBancario.Cpf))
            {
                return "Favor informar o número do CPF do titular da conta.";
            }
            else
            {
                var cpf = dadosBancario.Cpf;
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

                for (var i = 0; i <= 9; i++)
                {
                    var referencia = 11111111111;

                    if ((i * referencia).ToString() == cpf)
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

                if (!cpf.EndsWith(digito))
                {
                    return "CPF inválido.";
                }
            }

            return "";
        }
        #endregion
    }
}