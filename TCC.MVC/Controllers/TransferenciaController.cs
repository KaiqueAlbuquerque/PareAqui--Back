using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;

namespace TCC.MVC.Controllers
{
    public class TransferenciaController : Controller
    {
        private readonly ILocacaoService _locacaoService;
        private readonly ITransferenciaService _transferenciaService;
        private readonly IDadosBancarioService _dadosBancarioService;

        public TransferenciaController(ILocacaoService locacaoService,
                                       ITransferenciaService transferenciaService,
                                       IDadosBancarioService dadosBancarioService
                                   )
        {
            _locacaoService = locacaoService;
            _transferenciaService = transferenciaService;
            _dadosBancarioService = dadosBancarioService;
        }

        public string ConsultarTransferencias()
        {
            try
            {   
                var transferencias = from t in _transferenciaService.GetAll()
                                     where t.DataPagamento <= DateTime.Now && t.DataEfetivado == null && t.Aprovado == true && 
                                     ((t.Locacao.ModalidadeLocacao == 1 || t.Locacao.ModalidadeLocacao == 2) && t.Locacao.Ativo == false) ||
                                     (t.Locacao.ModalidadeLocacao == 3)
                                     group t by t.Locacao.Vaga.UsuarioId into a
                                     select new
                                     {
                                         Id = a.Key,
                                         Nome = a.FirstOrDefault().Locacao.Vaga.Usuario.Nome,
                                         Sum = a.Sum(t => t.Valor),
                                     };
                                                
                return JsonConvert.SerializeObject(new { code = 200, trans = transferencias });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao solicitar as transferencias. Por favor, tente novamente." });
            }
        }

        public string ConsultarUmaTransferencia(int idUsuario)
        {
            try
            {
                List<int> idsTransferencias = new List<int>();
                string Nome = "";
                DadosBancario dados = new DadosBancario();

                var transferencias = _transferenciaService.GetAll().Where(t => t.DataPagamento <= DateTime.Now && t.DataEfetivado == null && t.Aprovado == true && t.Locacao.Vaga.UsuarioId == idUsuario);

                foreach(var t in transferencias)
                {
                    Nome = t.Locacao.Vaga.Usuario.Nome;
                    dados = _dadosBancarioService.GetByIdUser(t.Locacao.Vaga.UsuarioId).FirstOrDefault();

                    if (t.Locacao.ModalidadeLocacao == 1 || t.Locacao.ModalidadeLocacao == 2)
                    {
                        if (t.Locacao.Ativo == false)
                        {
                            idsTransferencias.Add(t.TransferenciaId);
                        }
                    }
                    else
                    {
                        idsTransferencias.Add(t.TransferenciaId);
                    }
                }
                    
                return JsonConvert.SerializeObject(new { code = 200, dadosBancarios = dados, transferencias = idsTransferencias });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao solicitar as transferencias. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string EfetuarTransferencia(int idTransferencia, string codigoTransferencia)
        {
            try
            {
                var transferenciaService = _transferenciaService.GetById(idTransferencia);

                transferenciaService.DataEfetivado = DateTime.Now;
                transferenciaService.CodigoTransferencia = codigoTransferencia;

                _transferenciaService.Update(transferenciaService);

                return JsonConvert.SerializeObject(new { code = 200 });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao realizar a transferencia. Por favor, tente novamente." });
            }
        }
    }
}