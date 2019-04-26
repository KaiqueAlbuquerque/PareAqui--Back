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
    public class PagamentoController : Controller
    {
        private readonly ILocacaoService _locacaoService;
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(ILocacaoService locacaoService,
                                   IPagamentoService pagamentoService
                                   )
        {
            _locacaoService = locacaoService;
            _pagamentoService = pagamentoService;
        }

        public string ConsultarPagamentos()
        {
            try
            {
                /*List<Pagamento> pgtos = new List<Pagamento>();
                var pagamentos = _pagamentoService.GetAll().Where(p => p.DataPagamento <= DateTime.Now && p.DataEfetivado == null && p.Aprovado == true);

                foreach(var p in pagamentos)
                {
                    p.Locacao = _locacaoService.GetById(p.LocacaoId);

                    if(p.Locacao.ModalidadeLocacao == 1 || p.Locacao.ModalidadeLocacao == 2)
                    {
                        if(p.Locacao.Ativo == false)
                        {
                            pgtos.Add(p);
                        }
                    }
                    else if(p.Locacao.ModalidadeLocacao == 3 && p.Locacao.DiaHoraFim == null)
                    {
                        var pagamento = new Pagamento();

                        pagamento.DadosPagamentoId = p.DadosPagamentoId;
                        pagamento.LocacaoId = p.LocacaoId;
                        pagamento.Valor = p.Valor;
                        pagamento.Aprovado = true;

                        pagamento.DataPagamento = p.DataPagamento.AddMonths(1);
                        _pagamentoService.Add(pagamento);

                        pgtos.Add(p);
                    }
                    else
                    {
                        pgtos.Add(p);
                    }
                }*/
                var pgtos = new Pagamento();
                var pagamentos = _pagamentoService.GetAll().Where(p => p.DataPagamento <= DateTime.Now && p.DataEfetivado == null && p.Aprovado == true).FirstOrDefault();

                if(pagamentos != null)
                {
                    pagamentos.Locacao = _locacaoService.GetById(pagamentos.LocacaoId);

                    if (pagamentos.Locacao.ModalidadeLocacao == 1 || pagamentos.Locacao.ModalidadeLocacao == 2)
                    {
                        if (pagamentos.Locacao.Ativo == false)
                        {
                            pgtos = pagamentos;
                        }
                    }
                    else if (pagamentos.Locacao.ModalidadeLocacao == 3 && pagamentos.Locacao.DiaHoraFim == null)
                    {
                        var pagamento = new Pagamento();

                        pagamento.DadosPagamentoId = pagamentos.DadosPagamentoId;
                        pagamento.LocacaoId = pagamentos.LocacaoId;
                        pagamento.Valor = pagamentos.Valor;
                        pagamento.Aprovado = true;

                        pagamento.DataPagamento = pagamentos.DataPagamento.AddMonths(1);
                        _pagamentoService.Add(pagamento);

                        pgtos = pagamentos;
                    }
                    else
                    {
                        pgtos = pagamentos;
                    }
                }

                var pagamentoViewModel = Mapper.Map<Pagamento, PagamentoViewModel>(pgtos);
                return JsonConvert.SerializeObject(new { code = 200, pgto = pagamentoViewModel });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao solicitar os pagamentos. Por favor, tente novamente." });
            }
        }

        public string AlterarPagamento(int idPagamento, string code, string data)
        {
            try
            {
                var pagamentoService = _pagamentoService.GetById(idPagamento);

                pagamentoService.CodigoPagamento = code;
                pagamentoService.DataEfetivado = data;

                _pagamentoService.Update(pagamentoService);

                return JsonConvert.SerializeObject(new { code = 200 });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar o pagamentos. Por favor, tente novamente." });
            }
        }
    }
}