using AutoMapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.Business;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class LocacaoController : Controller
    {
        private readonly ILocacaoService _locacaoService;
        private readonly IVeiculoService _veiculoService;
        private readonly IVagaService _vagaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IGaragemService _garagemService;
        private readonly IChatService _chatService;
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly IEventoService _eventoService;
        private readonly Chat _chat;
        private readonly IPagamentoService _pagamentoService;
        private readonly IDadosBancarioService _dadosBancarioService;
        private readonly ITransferenciaService _transferenciaService;

        private int ValorTaxaLucro = 0;
        private string MensagemResumoLocador = "";
        private string MensagemResumoLocatario = "";
        private decimal ValorLocacaoLocador = 0.0m;

        public LocacaoController(ILocacaoService locacaoService,
                                 IVeiculoService veiculoService,
                                 IVagaService vagaService,
                                 IUsuarioService usuarioService,
                                 IGaragemService garagemService,
                                 IChatService chatService,
                                 IAvaliacaoService avaliacaoService,
                                 IEventoService eventoService,
                                 Chat chat,
                                 IPagamentoService pagamentoService,
                                 IDadosBancarioService dadosBancarioService,
                                 ITransferenciaService transferenciaService
                                )
        {
            _locacaoService = locacaoService;
            _veiculoService = veiculoService;
            _vagaService = vagaService;
            _usuarioService = usuarioService;
            _garagemService = garagemService;
            _chatService = chatService;
            _avaliacaoService = avaliacaoService;
            _eventoService = eventoService;
            _chat = chat;
            _pagamentoService = pagamentoService;
            _dadosBancarioService = dadosBancarioService;
            _transferenciaService = transferenciaService;
        }

        public string ConsultarGaragemPorLatLong(double latitude,
                                                 double longitude,
                                                 bool coberta,
                                                 bool descoberta,
                                                 bool portaoAutomatico,
                                                 bool portaoManual,
                                                 bool avulso,
                                                 bool mensal,
                                                 bool diaria,
                                                 decimal? valorAte,
                                                 double? distancia,
                                                 DateTime? diaHoraInicio,
                                                 DateTime? diaHoraFim,
                                                 int idUsuarioLocatario
                                                 )
        {
            try
            {
                if (diaHoraFim != null && diaHoraInicio == null)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Favor, informar a Data e hora de início." });
                }

                List<GaragemLocacaoViewModel> garagens = new List<GaragemLocacaoViewModel>();

                var listaGaragens = RetornaLista(latitude,
                                                 longitude,
                                                 coberta,
                                                 descoberta,
                                                 portaoAutomatico,
                                                 portaoManual,
                                                 avulso,
                                                 mensal,
                                                 diaria,
                                                 valorAte,
                                                 distancia,
                                                 diaHoraInicio,
                                                 diaHoraFim,
                                                 idUsuarioLocatario
                                                );

                foreach(var l in listaGaragens)
                {
                    if(l.VagaETaxa.Count() > 0)
                    {
                        List<VagaLocacaoViewModel> vagas = new List<VagaLocacaoViewModel>();
                        foreach (var v in l.VagaETaxa)
                        {
                            if (v.Vaga.UsuarioId != idUsuarioLocatario)
                            {                                
                                vagas.Add(v);
                            }
                        }
                        if(vagas.Count > 0)
                        {
                            GaragemLocacaoViewModel gar = new GaragemLocacaoViewModel();
                            gar.Garagem = l.Garagem;
                            gar.VagaETaxa = vagas;
                            garagens.Add(gar);
                        }
                    }
                }

                return JsonConvert.SerializeObject(new { code = 200, listaComGaragens = garagens });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        public string VerificarDisponibilidade(int idVaga)
        {
            try
            {
                List<Locacao> loc = new List<Locacao>();

                //var vaga = Mapper.Map<VagaViewModel, Vaga>(vagaViewModel);

                var locacoes = _locacaoService.GetByIdVaga(idVaga);

                foreach (var l in locacoes)
                {
                    if ((DateTime.Now.Date <= l.DiaHoraInicio.Date || DateTime.Now.Date <= DateTime.Parse(l.DiaHoraFim.ToString()).Date) && l.Aprovada == true && l.Ativo)
                    {
                        loc.Add(l);
                    }
                }

                return JsonConvert.SerializeObject(new { code = 200, listaLocacoes = loc });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao verificar a disponibilidade. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string CotarLocacao(LocacaoViewModel locacao/*, List<GaragemLocacaoViewModel> garagens*/)
        {
            try
            {
                if(ValidaDadosDeEntrada(locacao) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(locacao) });
                }
                else
                {
                    if ((locacao.ModalidadeLocacao == "1" || locacao.ModalidadeLocacao == "2") && locacao.DiaHoraFim == null)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o dia e horário de término da locação." });
                    }

                    List<Garagem> soAsGar = new List<Garagem>();

                    var locacaoService = Mapper.Map<LocacaoViewModel, Locacao>(locacao);

                    if(locacaoService.DiaHoraInicio <= DateTime.Now)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "O dia ou horário de início informados para esta locação já passou. Por favor, escolha um outro dia ou horário disponíveis." });
                    }

                    locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);
                    locacaoService.Veiculo = _veiculoService.GetById(locacaoService.VeiculoId);

                    var avulsa = locacaoService.ModalidadeLocacao == 1 ? true : false;
                    var diaria = locacaoService.ModalidadeLocacao == 2 ? true : false;
                    var mensal = locacaoService.ModalidadeLocacao == 3 ? true : false;

                    var retorno = RetornaLista(locacaoService.Vaga.Garagem.Latitude,
                                                locacaoService.Vaga.Garagem.Longitude,
                                                locacaoService.Vaga.Coberta,
                                                false,
                                                locacaoService.Vaga.PortaoAutomatico,
                                                false,
                                                avulsa,
                                                mensal,
                                                diaria,
                                                null,
                                                1,
                                                locacaoService.DiaHoraInicio,
                                                locacaoService.DiaHoraFim,
                                                locacaoService.Veiculo.UsuarioId
                                            );

                    foreach (var a in retorno)
                    {
                        soAsGar.Add(a.Garagem);
                    }

                    if(soAsGar.Count == 0)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta vaga foi alugada." });
                    }

                    /*foreach (var g in garagens)
                    {
                        soAsGar.Add(g.Garagem);
                    }*/

                    //locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);
                    //locacaoService.Veiculo = _veiculoService.GetById(locacaoService.VeiculoId);

                    if (locacaoService.ModalidadeLocacao == 3)
                    {
                        locacaoService.ValorLocacaoLocatario = Decimal.Round(CalculaValorLocacaoMensal(locacaoService.Vaga, soAsGar, locacaoService.Veiculo.UsuarioId, locacaoService.DiaHoraInicio, locacaoService.DiaHoraFim, locacaoService.TaxaLucro), 2);
                        locacaoService.ValorLocacaoLocador = ValorLocacaoLocador;
                    }
                    else if (locacaoService.ModalidadeLocacao == 2)
                    {
                        locacaoService.ValorLocacaoLocatario = Decimal.Round(CalculaValorLocacaoDiaria(locacaoService.Vaga, soAsGar, locacaoService.Veiculo.UsuarioId, locacaoService.DiaHoraInicio, locacaoService.DiaHoraFim, locacaoService.TaxaLucro), 2);
                        locacaoService.ValorLocacaoLocador = ValorLocacaoLocador;
                    }
                    else
                    {
                        locacaoService.ValorLocacaoLocatario = Decimal.Round(CalculaValorLocacaoAvulso(locacaoService.Vaga, soAsGar, locacaoService.Veiculo.UsuarioId, locacaoService.DiaHoraInicio, locacaoService.DiaHoraFim, locacaoService.TaxaLucro), 2);
                        locacaoService.ValorLocacaoLocador = ValorLocacaoLocador;
                    }

                    locacaoService.MensagemResumoLocatario = MensagemResumoLocatario;
                    locacaoService.MensagemResumoLocador = MensagemResumoLocador;

                    locacaoService.TaxaLucro = ValorTaxaLucro;

                    var locacaoVM = Mapper.Map<Locacao, LocacaoViewModel>(locacaoService);

                    return JsonConvert.SerializeObject(new { code = 200, locacaoComValor = locacaoVM });
                }
            }
            catch(Exception e)
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao calcular o valor da locação. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> RealizarLocacao(LocacaoViewModel locacao, int idDadosPagamento)
        {
            try
            {
                if(idDadosPagamento == 0)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar um cartão de crédito." });
                }
                if (ValidaDadosDeEntrada(locacao) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(locacao) });
                }
                else
                {
                    if ((locacao.ModalidadeLocacao == "1" || locacao.ModalidadeLocacao == "2") && locacao.DiaHoraFim == null)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o dia e horário de término da locação." });
                    }

                    locacao.Cancelada = false;
                    locacao.Aprovada = null;
                    locacao.Ativo = false;
                    locacao.Desistencia = false;
                    bool naoEstaAlugada = false;
                    bool ehNulo = false;

                    if (string.IsNullOrEmpty(locacao.DiaHoraFim))
                    {
                        ehNulo = true;
                    }

                    var locacaoService = Mapper.Map<LocacaoViewModel, Locacao>(locacao);

                    if (locacaoService.DiaHoraInicio <= DateTime.Now)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "O dia ou horário de início informados para esta locação já passou. Por favor, escolha um outro dia ou horário disponíveis." });
                    }

                    locacaoService.Multa = null;
                    locacao.QuemCancelou = null;

                    locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);
                    locacaoService.Veiculo = _veiculoService.GetById(locacaoService.VeiculoId);

                    naoEstaAlugada = VerificaSeEstaOcupada(locacaoService.Vaga, locacaoService.DiaHoraInicio, locacaoService.DiaHoraFim);

                    if (naoEstaAlugada)
                    {
                        locacaoService.Vaga = null;
                        locacaoService.Veiculo = null;

                        locacaoService.DiaHoraSaida = null;
                        if (ehNulo)
                        {
                            locacaoService.DiaHoraFim = null;
                        }

                        _locacaoService.Add(locacaoService);

                        if (locacaoService.ModalidadeLocacao == 1 || locacaoService.ModalidadeLocacao == 2)
                        {
                            var pagamento = new Pagamento();

                            pagamento.DadosPagamentoId = idDadosPagamento;
                            pagamento.LocacaoId = locacaoService.LocacaoId;
                            pagamento.Valor = Decimal.Round(locacaoService.ValorLocacaoLocatario, 2);
                            pagamento.Aprovado = null;
                            pagamento.DataEfetivado = null;

                            pagamento.DataPagamento = DateTime.Parse(locacaoService.DiaHoraFim.ToString());
                            _pagamentoService.Add(pagamento);

                            var transferencia = new Transferencia();

                            locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);

                            transferencia.DadosBancarioId = _dadosBancarioService.GetByIdUser(locacaoService.Vaga.UsuarioId).FirstOrDefault().DadosBancarioId;
                            transferencia.Valor = Decimal.Round(locacaoService.ValorLocacaoLocador, 2);
                            transferencia.DataPagamento = DateTime.Parse(locacaoService.DiaHoraFim.ToString()).AddDays(7);
                            transferencia.Aprovado = null;
                            transferencia.DataEfetivado = null;
                            transferencia.LocacaoId = locacaoService.LocacaoId;
                            transferencia.CodigoTransferencia = null;

                            _transferenciaService.Add(transferencia);
                        }
                        else
                        {
                            if (locacaoService.DiaHoraFim != null)
                            {
                                var dif = DateTime.Parse(locacaoService.DiaHoraFim.ToString()) - locacaoService.DiaHoraInicio;

                                var a = (dif.TotalDays / 30.436875);

                                var meses = (int)a;

                                for (var i = 1; i <= meses; i++)
                                {
                                    var pagamento = new Pagamento();

                                    pagamento.DadosPagamentoId = idDadosPagamento;
                                    pagamento.LocacaoId = locacaoService.LocacaoId;
                                    pagamento.Valor = Decimal.Round(locacaoService.ValorLocacaoLocatario, 2)/meses;
                                    pagamento.Aprovado = null;

                                    pagamento.DataPagamento = locacaoService.DiaHoraInicio.AddMonths(i);
                                    _pagamentoService.Add(pagamento);

                                    var transferencia = new Transferencia();

                                    locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);

                                    transferencia.DadosBancarioId = _dadosBancarioService.GetByIdUser(locacaoService.Vaga.UsuarioId).FirstOrDefault().DadosBancarioId;
                                    transferencia.Valor = Decimal.Round(locacaoService.ValorLocacaoLocador, 2) / meses;
                                    transferencia.DataPagamento = locacaoService.DiaHoraInicio.AddMonths(i).AddDays(7);
                                    transferencia.Aprovado = null;
                                    transferencia.DataEfetivado = null;
                                    transferencia.LocacaoId = locacaoService.LocacaoId;
                                    transferencia.CodigoTransferencia = null;

                                    _transferenciaService.Add(transferencia);                                    
                                }
                            }
                            else
                            {
                                var pagamento = new Pagamento();

                                pagamento.DadosPagamentoId = idDadosPagamento;
                                pagamento.LocacaoId = locacaoService.LocacaoId;
                                pagamento.Valor = Decimal.Round(locacaoService.ValorLocacaoLocatario, 2);
                                pagamento.Aprovado = null;

                                pagamento.DataPagamento = locacaoService.DiaHoraInicio.AddMonths(1);
                                _pagamentoService.Add(pagamento);

                                var transferencia = new Transferencia();

                                locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);

                                transferencia.DadosBancarioId = _dadosBancarioService.GetByIdUser(locacaoService.Vaga.UsuarioId).FirstOrDefault().DadosBancarioId;
                                transferencia.LocacaoId = locacaoService.LocacaoId;
                                transferencia.Valor = Decimal.Round(locacaoService.ValorLocacaoLocador, 2);
                                transferencia.DataPagamento = locacaoService.DiaHoraInicio.AddMonths(1).AddDays(7);
                                transferencia.Aprovado = null;
                                transferencia.DataEfetivado = null;
                                transferencia.CodigoTransferencia = null;

                                _transferenciaService.Add(transferencia);
                            }
                        }

                        locacaoService.Veiculo = _veiculoService.GetById(locacaoService.VeiculoId);
                        locacaoService.Vaga = _vagaService.GetById(locacaoService.VagaId);

                        string assunto = "Solicitação de Locação";
                        string mensagem = "Olá " + locacaoService.Vaga.Usuario.Nome + "<br /><br /> Informamos que " +
                                        locacaoService.Veiculo.Usuario.Nome + ", gostaria de alugar sua vaga localizada na " +
                                        locacaoService.Vaga.Garagem.Endereco + ", Nº" + locacaoService.Vaga.Garagem.NumeroRua +
                                        ", " + locacaoService.Vaga.Garagem.Bairro + ", Seguem abaixo, os dados da locação: <br /><br />" +
                                        "Dados da Locação: <br /><br />" +
                                        "Local da Locação: " + locacaoService.Vaga.Garagem.Endereco + ", Nº" + locacaoService.Vaga.Garagem.NumeroRua + ", " + locacaoService.Vaga.Garagem.Bairro + "<br />" +
                                        "Solicitante: " + locacaoService.Veiculo.Usuario.Nome + "<br />" +
                                        "Dia da Locação: " + locacaoService.DiaHoraInicio.ToString("dd/MM/yyyy") + " às " +
                                        locacaoService.DiaHoraInicio.ToString("HH:mm") + "<br /><br />" +
                                        "Para aprovar ou reprovar a locação, acesse o App PareAqui. <br /><br />" +
                                        "E-mail automático. Favor não responder.";

                        /*Notification n = new Notification();

                        if (locacaoService.Vaga.Usuario.AceitaReceberEmail)
                        { 
                            await n.SendMailAsync(locacaoService.Vaga.Usuario.Email, assunto, mensagem);
                        }

                        if(locacaoService.Vaga.Usuario.AceitaReceberSms)
                        {
                            string sms = "Olá " + locacaoService.Vaga.Usuario.Nome + ". Há uma nova solicitação de locação de sua(s) vaga(s). Para mais informações acesse o App PareAqui.";

                            n.SendSMS(locacaoService.Vaga.Usuario.Celular, sms);
                        }

                        if (locacaoService.Veiculo.Usuario.AceitaReceberSms)
                        {
                            string sms = "Olá " + locacaoService.Veiculo.Usuario.Nome + ". Sua solicitação de Locação da vaga foi realizada. Aguarde uma posição do proprietário da vaga. Para mais informações acesse o App PareAqui.";

                            n.SendSMS(locacaoService.Veiculo.Usuario.Celular, sms);
                        }*/

                        return JsonConvert.SerializeObject(new { code = 200, message = "Locação solicitada com sucesso. Aguarde aprovação por parte do proprietário da vaga", locacao = locacaoService });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta vaga já está alugada." });
                    }
                }
            }
            catch(Exception e)
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao solicitar a locação. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> DesistirLocacao(int idLocacao)
        {
            try
            {
                var locacao = _locacaoService.GetById(idLocacao);

                if (locacao.Cancelada)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta locação já foi cancelada." });
                }
                else if (locacao.Aprovada == true)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas a locação já foi aprovada." });
                }
                else if (locacao.Aprovada == false)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta locação já foi reprovada e por isso você não pode mais aprovar a mesma." });
                }
                else if (locacao.DiaHoraInicio < DateTime.Now)
                {
                    await ReprovarLocacao(locacao.LocacaoId, "O horário de Início da Locação já passou, então a mesma foi automaticamente reprovada.");
                    return JsonConvert.SerializeObject(new { code = 400, message = "O horário de Início da Locação já passou, então a mesma foi automaticamente reprovada." });
                }
                else
                {
                    locacao.Cancelada = false;
                    locacao.Desistencia = true;
                    locacao.Ativo = false;
                    locacao.Aprovada = null;
                    _locacaoService.Update(locacao);

                    var pagamento = _pagamentoService.GetAll().Where(p => p.LocacaoId == idLocacao);
                    foreach (var p in pagamento)
                    {
                        p.Aprovado = false;
                        _pagamentoService.Update(p);
                    }

                    var transferencia = _transferenciaService.GetAll().Where(t => t.LocacaoId == idLocacao);
                    foreach (var t in transferencia)
                    {
                        t.Aprovado = false;
                        _transferenciaService.Update(t);
                    }

                    string assunto = "Desistência de Locação";
                    string mensagem = "Olá " + locacao.Vaga.Usuario.Nome + "<br /><br /> Informamos que " +
                                    locacao.Veiculo.Usuario.Nome + ", que gostaria de alugar a vaga localizada na " +
                                    locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua +
                                    ", " + locacao.Vaga.Garagem.Bairro + ", desistiu da locação. <br /><br />" +
                                    "E-mail automático. Favor não responder.";

                    /*Notification n = new Notification();

                    if (locacao.Vaga.Usuario.AceitaReceberEmail)
                    { 
                        await n.SendMailAsync(locacao.Vaga.Usuario.Email, assunto, mensagem);
                    }

                    if(locacao.Vaga.Usuario.AceitaReceberSms)
                    {
                        string sms = "Olá " + locacao.Vaga.Usuario.Nome + ". Infelizmente desistiram de uma locação em sua vaga. Para mais informações acesse o App PareAqui.";

                        n.SendSMS(locacao.Vaga.Usuario.Celular, sms);
                    }*/

                    return JsonConvert.SerializeObject(new { code = 200, message = "Você desistiu da locação. O dono da vaga receberá um aviso de que você não quer mais realizar o aluguel da vaga." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desistir da locação. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> AprovarLocacao(int idLocacao)
        {
            try
            {
                bool naoEstaAlugada = false;

                var locacao = _locacaoService.GetById(idLocacao);
                                
                if (locacao.Cancelada)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta locação já foi cancelada." });
                }
                else if (locacao.Aprovada == true)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Você já aprovou esta locação." });
                }
                else if (locacao.Aprovada == false)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta locação já foi reprovada e por isso você não pode mais aprovar a mesma." });
                }
                else if(locacao.Desistencia == true)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas o locatário desistiu da locação." });
                }
                else if (locacao.DiaHoraInicio < DateTime.Now)
                {
                    await ReprovarLocacao(locacao.LocacaoId, "O horário de Início da Locação já passou, então a mesma foi automaticamente reprovada.");
                    return JsonConvert.SerializeObject(new { code = 400, message = "O horário de Início da Locação já passou, então a mesma foi automaticamente reprovada." });
                }
                else
                {
                    naoEstaAlugada = VerificaSeEstaOcupada(locacao.Vaga, locacao.DiaHoraInicio, locacao.DiaHoraFim);

                    if (naoEstaAlugada)
                    {
                        IEnumerable<Locacao> locacoesNoMesmoHorario;
                        List<Locacao> loc = new List<Locacao>();

                        if (locacao.DiaHoraFim != null)
                        {
                            var locacoes = _locacaoService.GetAll().Where(l => l.VagaId == locacao.VagaId && l.Aprovada == null && l.Cancelada == false);

                            foreach (var l in locacoes)
                            {
                                if (
                                    l.DiaHoraInicio < locacao.DiaHoraInicio && l.DiaHoraFim <= locacao.DiaHoraInicio ||
                                    l.DiaHoraInicio >= locacao.DiaHoraFim && l.DiaHoraFim > locacao.DiaHoraFim
                                    )
                                {

                                }
                                else
                                {
                                    loc.Add(l);
                                }
                            }

                            locacoesNoMesmoHorario = loc;
                        }
                        else
                        {
                            locacoesNoMesmoHorario = _locacaoService.GetAll().Where(l => l.VagaId == locacao.VagaId && l.DiaHoraInicio >= locacao.DiaHoraInicio && (l.DiaHoraFim != null && l.DiaHoraFim >= locacao.DiaHoraInicio) || (l.DiaHoraFim == null));
                        }

                        for (var i = 0; i < locacoesNoMesmoHorario.ToList().Count(); i++)
                        {
                            if (locacao.LocacaoId != locacoesNoMesmoHorario.ToList()[i].LocacaoId)
                            {
                                await ReprovarLocacao(locacoesNoMesmoHorario.ToList()[i].LocacaoId, "A Vaga foi alugada para outra pessoa neste período de horário");
                            }
                        }

                        locacao.Cancelada = false;
                        locacao.Desistencia = false;
                        locacao.Ativo = true;
                        locacao.Aprovada = true;
                        _locacaoService.Update(locacao);

                        var pagamento = _pagamentoService.GetAll().Where(p => p.LocacaoId == idLocacao);
                        foreach (var p in pagamento)
                        {
                            p.Aprovado = true;
                            _pagamentoService.Update(p);
                        }

                        var transferencia = _transferenciaService.GetAll().Where(t => t.LocacaoId == idLocacao);
                        foreach (var t in transferencia)
                        {
                            t.Aprovado = true;
                            _transferenciaService.Update(t);
                        }

                        locacao.Veiculo = _veiculoService.GetById(locacao.VeiculoId);
                        locacao.Vaga = _vagaService.GetById(locacao.VagaId);
                        locacao.Veiculo.Usuario = _usuarioService.GetById(locacao.Veiculo.UsuarioId);

                        bool jahExiste = _chatService.CheckIfNotRegistered(locacao.Vaga.UsuarioId, locacao.Veiculo.UsuarioId);

                        if (!jahExiste)
                        {
                            _chat.UsuarioLocadorId = locacao.Vaga.UsuarioId;
                            _chat.UsuarioLocatarioId = locacao.Veiculo.UsuarioId;

                            _chatService.Add(_chat);
                        }

                        string assunto = "Locação Aprovada";
                        string mensagem = "Olá " + locacao.Veiculo.Usuario.Nome + "<br /><br /> Informamos que " +
                                        locacao.Vaga.Usuario.Nome + ", que é o(a) proprietário(a) da vaga localizada na " +
                                        locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua +
                                        ", " + locacao.Vaga.Garagem.Bairro + ", aprovou a sua locação. Seguem abaixo, os dados da mesma: <br /><br />" +
                                        "Dados da Locação: <br /><br />" +
                                        "Local da Locação: " + locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua + ", " + locacao.Vaga.Garagem.Bairro + "<br />" +
                                        "Proprietário da vaga: " + locacao.Vaga.Usuario.Nome + "<br />" +
                                        "Dia da Locação: " + locacao.DiaHoraInicio.ToString("dd/MM/yyyy") + " às " +
                                        locacao.DiaHoraInicio.ToString("HH:mm") + "<br /><br />" +
                                        "E-mail automático. Favor não responder.";

                        /*Notification n = new Notification();

                        if (locacao.Veiculo.Usuario.AceitaReceberEmail)
                        { 
                            await n.SendMailAsync(locacao.Veiculo.Usuario.Email, assunto, mensagem);
                        }

                        if(locacao.Veiculo.Usuario.AceitaReceberSms)
                        {
                            string sms = "Olá " + locacao.Veiculo.Usuario.Nome + ". Sua locação foi aprovada. Para mais informações acesse o App PareAqui.";

                            n.SendSMS(locacao.Veiculo.Usuario.Celular, sms);
                        }*/

                        var locacaoService = Mapper.Map<Locacao, LocacaoViewModel>(locacao);
                        
                        return JsonConvert.SerializeObject(new { code = 200, message = "Locação aprovada, o locatário será notificado sobre a aprovação.", locacao = locacaoService });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta vaga já está alugada." });
                    }
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao aprovar a locação. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> ReprovarLocacao(int idLocacao, string observacao)
        {
            try
            {
                var locacao = _locacaoService.GetById(idLocacao);

                if (locacao.Cancelada)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas esta locação já foi cancelada." });
                }
                else if (locacao.Aprovada == false)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Você já reprovou essa locação." });
                }
                else if (locacao.Desistencia == true)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas o locatário já desistiu da locação." });
                }
                else if (locacao.Aprovada == true)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas você não pode Reprovar uma locação que já foi aprovada. Se você quiser, você pode cancelar a mesma." });
                }
                else
                {
                    locacao.Cancelada = false;
                    locacao.Desistencia = false;
                    locacao.Ativo = false;
                    locacao.Aprovada = false;
                    locacao.Observacao = observacao;
                    _locacaoService.Update(locacao);

                    var pagamento = _pagamentoService.GetAll().Where(p => p.LocacaoId == idLocacao);
                    foreach (var p in pagamento)
                    {
                        p.Aprovado = false;
                        _pagamentoService.Update(p);
                    }

                    var transferencia = _transferenciaService.GetAll().Where(t => t.LocacaoId == idLocacao);
                    foreach (var t in transferencia)
                    {
                        t.Aprovado = false;
                        _transferenciaService.Update(t);
                    }

                    string assunto = "Locação Reprovada";
                    string mensagem = "Olá " + locacao.Veiculo.Usuario.Nome + "<br /><br /> Informamos que " +
                                    locacao.Vaga.Usuario.Nome + ", que é o(a) proprietário(a) da vaga localizada na " +
                                    locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua +
                                    ", " + locacao.Vaga.Garagem.Bairro + ", reprovou a sua locação. Segue abaixo o motivo: <br /><br />" +
                                    "Motivo: " + observacao + "<br /><br />" +
                                    "E-mail automático. Favor não responder.";

                    /*Notification n = new Notification();

                    if (locacao.Veiculo.Usuario.AceitaReceberEmail)
                    { 
                        await n.SendMailAsync(locacao.Veiculo.Usuario.Email, assunto, mensagem);
                    }

                    if(locacao.Veiculo.Usuario.AceitaReceberSms)
                    {
                        string sms = "Olá " + locacao.Veiculo.Usuario.Nome + ". Infelizmente sua locação não foi aprovada. Para mais informações acesse o App PareAqui.";

                        n.SendSMS(locacao.Veiculo.Usuario.Celular, sms);
                    }*/

                    return JsonConvert.SerializeObject(new { code = 200, message = "A Locação foi recusada. O locatário receberá um aviso de que você não aprovou o aluguel da vaga." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao recusar a locação. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> FinalizarLocacao(int idLocacao)
        {
            try
            {
                var locacao = _locacaoService.GetById(idLocacao);
                bool verificaSuspender = false;
                bool adicionaMes = false;

                if (locacao.ModalidadeLocacao == 1 || locacao.ModalidadeLocacao == 2)
                {
                    locacao.DiaHoraSaida = DateTime.Now;
                }
                else
                {
                    var dia = locacao.DiaHoraInicio.Day;

                    if (DateTime.Now.Day > dia)
                    {
                        adicionaMes = true;
                    }

                    var mes = DateTime.Now.Month;
                    var ano = DateTime.Now.Year;
                    var hora = locacao.DiaHoraInicio.ToString("HH:mm:ss");
                    var data = dia.ToString() + "/" + mes.ToString() + "/" + ano + " " + hora;

                    if (locacao.DiaHoraFim != null)
                    {
                        verificaSuspender = true;
                        locacao.DiaHoraSaida = Convert.ToDateTime(data);
                        if (adicionaMes)
                        {
                            locacao.DiaHoraSaida = DateTime.Parse(locacao.DiaHoraSaida.ToString()).AddMonths(1);
                        }
                    }
                    else
                    {
                        locacao.DiaHoraFim = Convert.ToDateTime(data);
                        locacao.DiaHoraSaida = locacao.DiaHoraFim;
                        if (adicionaMes)
                        {
                            locacao.DiaHoraSaida = DateTime.Parse(locacao.DiaHoraSaida.ToString()).AddMonths(1);
                            locacao.DiaHoraFim = locacao.DiaHoraSaida;
                        }
                    }
                }

                if (locacao.DiaHoraSaida < locacao.DiaHoraInicio)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "A locação ainda não iniciou para poder ser finalizada. Se não quiser mais realizar a locação, você deve cancelar a mesma." });
                }
                else
                {
                    string fim = "";
                    string saida = "";
                    string mult = "";
                    string atras = "";

                    fim = " até " + DateTime.Parse(locacao.DiaHoraFim.ToString()).ToString("dd/MM/yyyy") + " às " + DateTime.Parse(locacao.DiaHoraFim.ToString()).ToString("HH:mm") + "<br />";
                    saida = "Saída: " + DateTime.Parse(locacao.DiaHoraSaida.ToString()).ToString("dd/MM/yyyy") + " às " + DateTime.Parse(locacao.DiaHoraSaida.ToString()).ToString("HH:mm");

                    TimeSpan? atraso = locacao.DiaHoraSaida - locacao.DiaHoraFim;

                    TimeSpan atra = TimeSpan.Parse(atraso.ToString());

                    if (atra.TotalMinutes > 5 && locacao.ModalidadeLocacao == 1)//Minutes + (atra.TotalHours * 60) + (atra.TotalDays * 24 * 60) > 5 && locacao.ModalidadeLocacao == 1)
                    {
                        var atrasoEmHoras = Math.Ceiling(atra.TotalHours);
                        locacao.Multa = Decimal.Round(Decimal.Parse(atrasoEmHoras.ToString()) * locacao.ValorLocacaoLocatario, 2);
                        //locacao.Multa = Decimal.Round(CalculaValorMultaAvulso(idLocacao) * Decimal.Parse(atra.TotalMinutes.ToString()), 2);// + (Decimal.Parse(atra.TotalHours.ToString()) * 60) + (Decimal.Parse(atra.TotalDays.ToString()) * 24 * 60),2);
                        atras = "Atraso: " + Math.Round(atra.TotalMinutes) + " minutos <br />";//Minutes + (atra.TotalHours * 60) + (atra.TotalDays * 24 * 60)) + " minutos <br />";
                        mult = "Multa: R$" + locacao.Multa + "<br />";
                        locacao.MensagemResumoMulta = "Atraso: " + Math.Round(atra.TotalMinutes) + " minutos";//(atra.Minutes + (atra.TotalHours * 60) + (atra.TotalDays * 24 * 60)) + " minutos";
                    }

                    locacao.Ativo = false;
                    locacao.Cancelada = false;
                    locacao.Desistencia = false;

                    _locacaoService.Update(locacao);

                    locacao.Pagamento = _pagamentoService.GetAll().Where(p => p.LocacaoId == locacao.LocacaoId);
                    locacao.Transferencia = _transferenciaService.GetAll().Where(t => t.LocacaoId == locacao.LocacaoId);

                    if (locacao.ModalidadeLocacao == 1 || locacao.ModalidadeLocacao == 2)
                    {
                        var pagamento = _pagamentoService.GetById(locacao.Pagamento.Where(p => p.LocacaoId == locacao.LocacaoId).OrderByDescending(p => p.PagamentoId).FirstOrDefault().PagamentoId);
                        if (locacao.Multa != null && locacao.Multa > 0)
                        {
                            pagamento.Valor = Decimal.Round(pagamento.Valor + Decimal.Parse(locacao.Multa.ToString()),2);
                        }

                        _pagamentoService.Update(pagamento);

                        var transferencia = _transferenciaService.GetById(locacao.Transferencia.Where(t => t.LocacaoId == locacao.LocacaoId).OrderByDescending(t => t.TransferenciaId).FirstOrDefault().TransferenciaId);
                        if (locacao.Multa != null && locacao.Multa > 0)
                        {
                            transferencia.Valor = Decimal.Round(transferencia.Valor + Decimal.Parse(locacao.Multa.ToString()), 2);
                        }

                        _transferenciaService.Update(transferencia);
                    }
                    if (verificaSuspender)
                    {
                        var pagamento = _pagamentoService.GetAll().Where(p => p.LocacaoId == locacao.LocacaoId && p.DataPagamento > locacao.DiaHoraSaida);

                        foreach (var pag in pagamento)
                        {
                            pag.Aprovado = false;
                            _pagamentoService.Update(pag);
                        }

                        var transferencia = _transferenciaService.GetAll().Where(t => t.LocacaoId == locacao.LocacaoId && t.DataPagamento > locacao.DiaHoraSaida);

                        foreach (var tra in transferencia)
                        {
                            tra.Aprovado = false;
                            _transferenciaService.Update(tra);
                        }
                    }

                    string assunto = "Detalhes da sua Locação";
                    string mensagem = "Olá " + locacao.Veiculo.Usuario.Nome + "<br /><br />" +
                                    "Segue abaixo os detalhes de sua locação. <br /><br />" +
                                    "Local: " + locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua + ", " +
                                    locacao.Vaga.Garagem.Bairro + "<br />" +
                                    "Locador: " + locacao.Vaga.Usuario.Nome + "<br />" +
                                    "Locatário: " + locacao.Veiculo.Usuario.Nome + "<br />" +
                                    "Início: " + locacao.DiaHoraInicio.ToString("dd/MM/yyyy") + " às " +
                                    locacao.DiaHoraInicio.ToString("HH:mm") + fim + saida + "<br />" +
                                    "Valor: " + Decimal.Round(locacao.ValorLocacaoLocatario, 2) + "<br />" + atras + mult + "<br /><br />" +
                                    "E-mail automático. Favor não responder.";

                    /*Notification n = new Notification();

                    if (locacao.Veiculo.Usuario.AceitaReceberEmail)
                    { 
                        await n.SendMailAsync(locacao.Veiculo.Usuario.Email, assunto, mensagem);
                    }

                    string mensagem2 = "Olá " + locacao.Vaga.Usuario.Nome + "<br /><br />" +
                                    "Segue abaixo os detalhes de sua locação. <br /><br />" +
                                    "Local: " + locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua + ", " +
                                    locacao.Vaga.Garagem.Bairro + "<br />" +
                                    "Locador: " + locacao.Vaga.Usuario.Nome + "<br />" +
                                    "Locatário: " + locacao.Veiculo.Usuario.Nome + "<br />" +
                                    "Início: " + locacao.DiaHoraInicio.ToString("dd/MM/yyyy") + " às " +
                                    locacao.DiaHoraInicio.ToString("HH:mm") + fim + saida + "<br />" +
                                    "Valor: " + Decimal.Round(locacao.ValorLocacaoLocador,2) + "<br />" + atras + mult + "<br /><br />" +
                                    "E-mail automático. Favor não responder.";

                    if (locacao.Vaga.Usuario.AceitaReceberEmail)
                    {
                        await n.SendMailAsync(locacao.Vaga.Usuario.Email, assunto, mensagem2);
                    }*/

                    return JsonConvert.SerializeObject(new { code = 200, message = "Locação finalizada com sucesso.", dadosLocacao = locacao });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao finalizar a locação. Por favor, tente novamente." });
            }
        }

        public string CancelarLocacaoCotacao(int idLocacao)
        {
            try
            {
                decimal multa = 0.0m;
                var locacao = _locacaoService.GetById(idLocacao);

                var agora = DateTime.Now;

                if (agora >= locacao.DiaHoraInicio && locacao.Ativo)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas você não pode cancelar uma locação que esteja em andamento. Se você quiser, pode finalizar a mesma." });
                }

                if (locacao.Aprovada == true)
                {
                    if (locacao.ModalidadeLocacao == 1)
                    {
                        multa = Decimal.Round(CalculaValorMultaCancelamentoAvulso(idLocacao),2);
                    }
                    else if (locacao.ModalidadeLocacao == 2)
                    {
                        multa = Decimal.Round(CalculaValorMultaCancelamentoDiaria(idLocacao),2);
                    }
                    else
                    {
                        multa = Decimal.Round(CalculaValorMultaCancelamentoMensal(idLocacao),2);
                    }
                }

                return JsonConvert.SerializeObject(new { code = 200, valorMulta = multa });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao cancelar a locação. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> CancelarLocacao(int idLocacao, int idUsuario, decimal multa)
        {
            try
            {
                string assunto = "";
                string mensagem = "";
                var locacao = _locacaoService.GetById(idLocacao);

                var agora = DateTime.Now;

                if (agora >= locacao.DiaHoraInicio && locacao.Ativo)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas você não pode cancelar uma locação que esteja em andamento. Se você quiser, pode finalizar a mesma." });
                }

                locacao.Cancelada = true;
                locacao.Desistencia = false;
                locacao.Ativo = false;
                locacao.QuemCancelou = idUsuario;
                locacao.Multa = multa;
                locacao.MensagemResumoMulta = "Cancelamento de Locação.";

                var pagamento = _pagamentoService.GetAll().Where(p => p.LocacaoId == locacao.LocacaoId && p.DataEfetivado == null);

                foreach (var p in pagamento)
                {
                    p.Aprovado = false;
                    _pagamentoService.Update(p);
                }

                var transferencia = _transferenciaService.GetAll().Where(t => t.LocacaoId == locacao.LocacaoId && t.DataEfetivado == null);

                foreach (var t in transferencia)
                {
                    t.Aprovado = false;
                    _transferenciaService.Update(t);
                }

                if (locacao.QuemCancelou == locacao.Veiculo.UsuarioId)
                {
                    if (multa != 0)
                    {
                        var pgto = new Pagamento();
                        pgto.Aprovado = true;
                        pgto.DadosPagamentoId = pagamento.FirstOrDefault().DadosPagamentoId;
                        pgto.DataPagamento = DateTime.Now;
                        pgto.LocacaoId = idLocacao;
                        pgto.Valor = Decimal.Round(multa,2);

                        _pagamentoService.Add(pgto);

                        var trans = new Transferencia();
                        trans.Aprovado = true;
                        trans.DadosBancarioId = transferencia.FirstOrDefault().DadosBancarioId;
                        trans.DataPagamento = DateTime.Now.AddDays(7);
                        trans.LocacaoId = idLocacao;
                        trans.Valor = Decimal.Round(multa, 2);
                        trans.CodigoTransferencia = null;

                        _transferenciaService.Add(trans);
                    }
                }

                _locacaoService.Update(locacao);

                locacao.Vaga = _vagaService.GetById(locacao.VagaId);
                locacao.Veiculo = _veiculoService.GetById(locacao.VeiculoId);
                locacao.Vaga.Usuario = _usuarioService.GetById(locacao.Vaga.UsuarioId);
                locacao.Veiculo.Usuario = _usuarioService.GetById(locacao.Veiculo.UsuarioId);

                if(locacao.QuemCancelou == locacao.Vaga.UsuarioId)
                {
                    assunto = "Locação Cancelada";
                    mensagem = "Olá " + locacao.Veiculo.Usuario.Nome + "<br /><br /> Informamos que " +
                                    locacao.Vaga.Usuario.Nome + ", que é o(a) proprietário(a) da vaga localizada na " +
                                    locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua +
                                    ", " + locacao.Vaga.Garagem.Bairro + ", cancelou a sua locação que seria realizada no dia " + locacao.DiaHoraInicio.ToString("dd/MM/yyyy") + " às " +
                                    locacao.DiaHoraInicio.ToString("HH:mm") + ". < br /><br />" +
                                    "Pedimos desculpas pelo transtorno mas não se preocupe, o Pagamento que havia sido agendado foi cancelado e temos outras milhares de vagas que você poderá alugar, para isso acesso o App PareAqui. <br />Se você quiser, poderá avaliar a sua experiência com o proprietário da vaga. <br /><br />" +
                                    "E-mail automático. Favor não responder.";
                }
                else
                {
                    assunto = "Locação Cancelada";
                    mensagem = "Olá " + locacao.Vaga.Usuario.Nome + "<br /><br /> Informamos que " +
                                    locacao.Veiculo.Usuario.Nome + ", que alugou sua vaga localizada na " +
                                    locacao.Vaga.Garagem.Endereco + ", Nº" + locacao.Vaga.Garagem.NumeroRua +
                                    ", " + locacao.Vaga.Garagem.Bairro + ", no dia " + locacao.DiaHoraInicio.ToString("dd/MM/yyyy") + " às " +
                                    locacao.DiaHoraInicio.ToString("HH:mm") + ", cancelou a locação. <br /><br />" +
                                    "A fim de evitar maiores prejuízos, o proprietário do veículo pagará uma multa e pagaremos o valor da mesma integralmente a você. Você receberá o valor de R$" + Decimal.Round(multa, 2) + " no próximo ciclo de pagamentos. <br />" +
                                    "Se você quiser, poderá avaliar a sua experiência com o locador da vaga. < br />< br /> " +
                                    "E-mail automático. Favor não responder.";
                }
                /*
                Notification n = new Notification();

                if(locacao.QuemCancelou == locacao.Vaga.Usuario.UsuarioId)
                {
                    if (locacao.Veiculo.Usuario.AceitaReceberEmail)
                    {
                        await n.SendMailAsync(locacao.Veiculo.Usuario.Email, assunto, mensagem);
                    }

                    if (locacao.Veiculo.Usuario.AceitaReceberSms)
                    {
                        string sms = "Olá " + locacao.Veiculo.Usuario.Nome + ". Infelizmente sua locação foi cancelada. Para mais informações acesse o App PareAqui.";

                        n.SendSMS(locacao.Veiculo.Usuario.Celular, sms);
                    }
                }
                else
                {
                    if (locacao.Vaga.Usuario.AceitaReceberEmail)
                    {
                        await n.SendMailAsync(locacao.Vaga.Usuario.Email, assunto, mensagem);
                    }

                    if (locacao.Vaga.Usuario.AceitaReceberSms)
                    {
                        string sms = "Olá " + locacao.Vaga.Usuario.Nome + ". Infelizmente a locação de uma de sua(s) vaga(s) foi cancelada. Para mais informações acesse o App PareAqui.";

                        n.SendSMS(locacao.Vaga.Usuario.Celular, sms);
                    }
                }*/                

                return JsonConvert.SerializeObject(new { code = 200, message = "Locação cancelada com sucesso.", dadosLocacao = locacao });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao cancelar a locação. Por favor, tente novamente." });
            }
        }

        public string ConsultarLocacaoPorIdUsuario(int idUsuario,
                                                   bool comoLocador,
                                                   bool comoLocatario,
                                                   bool aprovada,
                                                   bool reprovada,
                                                   bool aguardandoAprovacao,
                                                   bool ativa,
                                                   bool finalizada,
                                                   bool avulso,
                                                   bool mensal,
                                                   bool diaria,
                                                   bool cancelada, 
                                                   bool desistiu
                                                   )
        {
            try
            {
                List<LocacaoViewModel> listLocacaoViewModel = new List<LocacaoViewModel>();

                if (comoLocador == true || comoLocatario == true || comoLocador == false && comoLocatario == false && aprovada == false && reprovada == false && ativa == false && finalizada == false && avulso == false && mensal == false && diaria == false && cancelada == false && aguardandoAprovacao == false && desistiu == false)
                {
                    if (comoLocador == true || comoLocador == false && comoLocatario == false && aprovada == false && reprovada == false && ativa == false && finalizada == false && avulso == false && mensal == false && diaria == false && cancelada == false && aguardandoAprovacao == false && desistiu == false)
                    {
                        var garagens = _garagemService.GetByIdUser(idUsuario);

                        foreach (var g in garagens)
                        {
                            var vagas = _vagaService.GetByIdGarageAndIdUser(g.GaragemId, idUsuario);

                            foreach (var v in vagas)
                            {
                                var locacoes = _locacaoService.GetByIdVaga(v.VagaId);

                                listLocacaoViewModel = FiltroLocacao(locacoes, listLocacaoViewModel, aprovada, reprovada, aguardandoAprovacao, ativa, finalizada, avulso, mensal, diaria, cancelada, desistiu);
                            }
                        }
                    }
                    if (comoLocatario == true || comoLocador == false && comoLocatario == false && aprovada == false && reprovada == false && ativa == false && finalizada == false && avulso == false && mensal == false && diaria == false && cancelada == false && aguardandoAprovacao == false && desistiu == false)
                    {
                        var veiculos = _veiculoService.GetByIdUsuario(idUsuario);

                        foreach (var v in veiculos)
                        {
                            var locacoes = _locacaoService.GetByIdVeiculo(v.VeiculoId);

                            listLocacaoViewModel = FiltroLocacao(locacoes, listLocacaoViewModel, aprovada, reprovada, aguardandoAprovacao, ativa, finalizada, avulso, mensal, diaria, cancelada, desistiu);
                        }
                    }

                    bool jahTemBotao = false;
                    foreach (var l in listLocacaoViewModel)
                    {
                        var avaliacao = _avaliacaoService.GetAll().Where(a => a.LocacaoId.ToString() == l.LocacaoId && a.UsuarioAvaliadorId == idUsuario);
                        jahTemBotao = false;

                        if (avaliacao.Count() == 0)
                        {
                            if (l.Ativo == false && !jahTemBotao && l.Desistencia == false && (l.Aprovada == true || l.Cancelada == true))
                            {
                                l.ColocaBotaoAvaliacao = true;
                                jahTemBotao = true;
                            }
                            else
                            {
                                l.ColocaBotaoAvaliacao = false;
                                jahTemBotao = false;
                            }
                        }
                        else
                        {
                            l.ColocaBotaoAvaliacao = false;
                            jahTemBotao = false;
                        }

                        if(l.Aprovada == null && l.Vaga.UsuarioId == idUsuario.ToString() && !jahTemBotao)
                        {
                            l.ColocaBotaoAprovar = true;
                            l.ColocaBotaoReprovar = true;
                            l.ColocaBotaoDesistir = false;
                            jahTemBotao = true;
                        }
                        else
                        {
                            l.ColocaBotaoAprovar = false;
                            l.ColocaBotaoReprovar = false;
                            jahTemBotao = false;
                            if (l.Cancelada == false && l.Aprovada == null && l.Ativo == false && !jahTemBotao)
                            {
                                l.ColocaBotaoDesistir = true;
                                jahTemBotao = true;
                            }                            
                        }

                        if(l.Ativo && DateTime.Parse(l.DiaHoraInicio) > DateTime.Now && !jahTemBotao) 
                        {
                            l.ColocaBotaoCancelar = true;
                            l.ColocaBotaoFinalizar = false;
                            jahTemBotao = true;
                        }
                        if(l.Ativo && DateTime.Parse(l.DiaHoraInicio) <= DateTime.Now && !jahTemBotao)
                        {
                            l.ColocaBotaoCancelar = false;
                            l.ColocaBotaoFinalizar = true;
                            jahTemBotao = true;
                        }

                        var notasDonoVaga = _avaliacaoService.GetAll().Where(loc => loc.UsuarioAvaliadoId.ToString() == l.Vaga.UsuarioId);
                        var notasDonoVeiculo  = _avaliacaoService.GetAll().Where(loc => loc.UsuarioAvaliadoId.ToString() == l.Veiculo.UsuarioId);
                        double totalNotasDonoVaga = 0.0;
                        double totalNotasDonoVeiculo = 0.0;

                        foreach(var ndva in notasDonoVaga)
                        {
                            totalNotasDonoVaga = totalNotasDonoVaga + ndva.Nota;
                        }

                        foreach (var ndve in notasDonoVeiculo)
                        {
                            totalNotasDonoVeiculo = totalNotasDonoVeiculo + ndve.Nota;
                        }

                        if(notasDonoVaga.Count() > 0)
                        {
                            l.NotaDonoVaga = (totalNotasDonoVaga / notasDonoVaga.Count());
                        }
                        else
                        {
                            l.NotaDonoVaga = 0.0;
                        }
                        if(notasDonoVeiculo.Count() > 0)
                        {
                            l.NotaDonoVeiculo = (totalNotasDonoVeiculo / notasDonoVeiculo.Count());
                        }
                        else
                        {
                            l.NotaDonoVeiculo = 0.0;
                        }
                    }

                    return JsonConvert.SerializeObject(new { code = 200, listaLocacoes = listLocacaoViewModel });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Por favor informe se é como locador ou como locatário." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        public void NotificaPushInicioLocacao()
        {
            var locacoes = _locacaoService.GetAll().Where(l => l.DiaHoraInicio.Date == DateTime.Now.Date && l.DiaHoraInicio.TimeOfDay >= DateTime.Now.TimeOfDay && l.DiaHoraInicio.TimeOfDay < DateTime.Now.AddMinutes(10).TimeOfDay && l.Aprovada == true && l.Ativo);

            foreach(var l in locacoes)
            {
                var token = l.Veiculo.Usuario.TokenPush;

                if (token != null)
                {
                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                    request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"A sua locação inicia em aproximadamente 10 minutos.\",\"title\": \"Início de Locação\" }}", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }

                token = l.Vaga.Usuario.TokenPush;

                if (token != null)
                {
                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                    request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"Uma de suas vagas receberá o início de uma locação em aproximadamente 10 minutos.\",\"title\": \"Início de Locação\" }}", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }
        }

        public void NotificaPushFimLocacao()
        {
            
            var locacoes = _locacaoService.GetAll().Where(l => DateTime.Parse(l.DiaHoraFim.ToString()).Date == DateTime.Now.Date && DateTime.Parse(l.DiaHoraFim.ToString()).TimeOfDay >= DateTime.Now.TimeOfDay && DateTime.Parse(l.DiaHoraFim.ToString()).TimeOfDay < DateTime.Now.AddMinutes(10).TimeOfDay && l.Aprovada == true && l.Ativo);

            foreach (var l in locacoes)
            {
                var token = l.Veiculo.Usuario.TokenPush;

                if (token != null)
                {
                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                    request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"A sua locação termina em aproximadamente 10 minutos. Fique atento.\",\"title\": \"Término de locação\" }}", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }

                token = l.Vaga.Usuario.TokenPush;

                if (token != null)
                {
                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                    request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"Um de seus aluguéis finaliza em aproximadamente 10 minutos.\",\"title\": \"Término de locação\" }}", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }
        }

        #region Métodos privados
        private decimal CalculaValorLocacaoMensal(Vaga vaga, List<Garagem> garagens, int usuarioLocatarioId, DateTime? diaHoraInicio, DateTime? diaHoraFim, int taxaLucro)
        {
            var valorTotal = 0.0m;
            var qtdeVagas = 0;
            //var taxaLucro = 20;
            var auxPrecoMensal = vaga.PrecoMensal;

            foreach (var g in garagens)
            {
                var vagas = g.Vagas;

                foreach (var v in vagas)
                {
                    if (v.Coberta == vaga.Coberta && v.PortaoAutomatico == vaga.PortaoAutomatico && v.Mensal == true)
                    {
                        valorTotal = Decimal.Round(valorTotal + Decimal.Parse(v.PrecoMensal.ToString()),2);
                        qtdeVagas++;
                    }
                }
            }

            var media = Decimal.Round(valorTotal,2) / qtdeVagas;
            var auxMedia = media;

            //taxaLucro = CalculaTaxaDeLucro(vaga.UsuarioId, taxaLucro);

            //taxaLucro = CalculaTaxaDeLucro(usuarioLocatarioId, taxaLucro);

            if (diaHoraInicio != null && diaHoraFim != null)
            {
                var anos = DateTime.Parse(diaHoraFim.ToString()).Year - DateTime.Parse(diaHoraInicio.ToString()).Year;
                var meses = DateTime.Parse(diaHoraFim.ToString()).Month - DateTime.Parse(diaHoraInicio.ToString()).Month;

                var qtdeMeses = meses == 0 && anos == 0 ? 1 : meses;
                var totalMeses = anos * 12 + qtdeMeses;

                media = media * totalMeses;

                vaga.PrecoMensal = vaga.PrecoMensal * totalMeses;
            }

            media = media * (Decimal.Parse(taxaLucro.ToString()) / 100) + media;

            var precoDoUsuario = vaga.PrecoMensal * (Decimal.Parse(taxaLucro.ToString()) / 100) + vaga.PrecoMensal;

            if (vaga.AceitaSugestaoDePreco)
            {
                if (media == 0)
                {
                    media = Decimal.Parse(precoDoUsuario.ToString());
                }

                var precoMensalCalculado = auxMedia * (Decimal.Parse(taxaLucro.ToString()) / 100) + auxMedia;

                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    var anos = DateTime.Parse(diaHoraFim.ToString()).Year - DateTime.Parse(diaHoraInicio.ToString()).Year;
                    var meses = DateTime.Parse(diaHoraFim.ToString()).Month - DateTime.Parse(diaHoraInicio.ToString()).Month;

                    var qtdeMeses = meses == 0 && anos == 0 ? 1 : meses;
                    var totalMeses = anos * 12 + qtdeMeses;

                    var b = totalMeses == 1 ? "Mês" : "Meses";

                    MensagemResumoLocatario = totalMeses + " " + b + " x R$" + Decimal.Round(Decimal.Parse(precoMensalCalculado.ToString()),2);
                    MensagemResumoLocador = totalMeses + " " + b + " x R$" + Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                    ValorLocacaoLocador = totalMeses * Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                }
                else
                {
                    MensagemResumoLocatario = "R$" + Decimal.Round(Decimal.Parse(precoMensalCalculado.ToString()),2) + " por mês";
                    MensagemResumoLocador = "R$" + Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2) + " por mês";
                    ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                }

                return Decimal.Round(Decimal.Parse(media.ToString()),2);
            }
            else
            {
                var precoMensalCalculado = auxPrecoMensal * (Decimal.Parse(taxaLucro.ToString()) / 100) + auxPrecoMensal;

                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    var anos = DateTime.Parse(diaHoraFim.ToString()).Year - DateTime.Parse(diaHoraInicio.ToString()).Year;
                    var meses = DateTime.Parse(diaHoraFim.ToString()).Month - DateTime.Parse(diaHoraInicio.ToString()).Month;

                    var qtdeMeses = meses == 0 && anos == 0 ? 1 : meses;
                    var totalMeses = anos * 12 + qtdeMeses;

                    var b = totalMeses == 1 ? "Mês" : "Meses";

                    MensagemResumoLocatario = totalMeses + " " + b + " x R$" + Decimal.Round(Decimal.Parse(precoMensalCalculado.ToString()),2);
                    MensagemResumoLocador = totalMeses + " " + b + " x R$" + Decimal.Round(Decimal.Parse(auxPrecoMensal.ToString()), 2);
                    ValorLocacaoLocador = totalMeses * Decimal.Round(Decimal.Parse(auxPrecoMensal.ToString()), 2);
                }
                else
                {
                    MensagemResumoLocatario = "R$" + Decimal.Round(Decimal.Parse(precoMensalCalculado.ToString()),2) + " por mês";
                    MensagemResumoLocador = "R$" + Decimal.Round(Decimal.Parse(auxPrecoMensal.ToString()), 2) + " por mês";
                    ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxPrecoMensal.ToString()), 2);
                }

                return Decimal.Round(Decimal.Parse(precoDoUsuario.ToString()),2);
            }
        }

        private decimal CalculaValorLocacaoAvulso(Vaga vaga, List<Garagem> garagens, int usuarioLocatarioId, DateTime? diaHoraInicio, DateTime? diaHoraFim, int taxaLucro)
        {
            var valorTotal = 0.0m;
            var qtdeVagas = 0;
            //var taxaLucro = 20;
            var totalAposPrimeiraHora = 0.0m;
            var auxPrecoAvulso = vaga.PrecoAvulso;
            var auxAposPrimeiraHora = vaga.AposPrimeiraHora;

            if(diaHoraInicio == null && diaHoraFim == null)
            {
                diaHoraInicio = DateTime.Now;
                diaHoraFim = DateTime.Parse(diaHoraInicio.ToString()).AddHours(1);
            }

            foreach (var g in garagens)
            {
                var vagas = g.Vagas;

                foreach (var v in vagas)
                {
                    if (v.Coberta == vaga.Coberta && v.PortaoAutomatico == vaga.PortaoAutomatico && v.Avulso == true)
                    {
                        valorTotal = Decimal.Round(valorTotal + Decimal.Parse(v.PrecoAvulso.ToString()),2);
                        qtdeVagas++;

                        if (diaHoraInicio != null && diaHoraFim != null)
                        {
                            totalAposPrimeiraHora = totalAposPrimeiraHora + Decimal.Parse(v.AposPrimeiraHora.ToString());
                        }
                    }
                }
            }

            var mediaAposPrimeiraHora = totalAposPrimeiraHora / qtdeVagas;

            var media = Decimal.Round(valorTotal,2) / qtdeVagas;

            //taxaLucro = CalculaTaxaDeLucro(vaga.UsuarioId, taxaLucro);

            //taxaLucro = CalculaTaxaDeLucro(usuarioLocatarioId, taxaLucro);

            if (diaHoraInicio != null && diaHoraFim != null)
            {
                var horas = diaHoraFim - diaHoraInicio;

                var qtdeHoras = TimeSpan.Parse(horas.ToString()).TotalHours == 0 ? 1 : TimeSpan.Parse(horas.ToString()).TotalHours;

                qtdeHoras = (int)Math.Round(qtdeHoras) - 1;
                //media = media + (mediaAposPrimeiraHora * Decimal.Parse(qtdeHoras.ToString()));

                if (qtdeHoras >= 1)
                {
                    media = media + (mediaAposPrimeiraHora * Decimal.Parse(qtdeHoras.ToString()));

                    //vaga.PrecoAvulso = vaga.PrecoAvulso + (vaga.AposPrimeiraHora * Decimal.Parse(qtdeHoras.ToString()));
                    vaga.PrecoAvulso = vaga.PrecoAvulso + (vaga.AposPrimeiraHora * Decimal.Parse(qtdeHoras.ToString()));
                }
            }

            media = media * (Decimal.Parse(taxaLucro.ToString()) / 100) + media;

            var precoDoUsuario = vaga.PrecoAvulso * (Decimal.Parse(taxaLucro.ToString()) / 100) + vaga.PrecoAvulso;

            if (vaga.AceitaSugestaoDePreco)
            {
                var totalPorcento = 0;
                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    List<Evento> eve = new List<Evento>();

                    foreach (var g in garagens)
                    {
                        var eventos = _eventoService.GetByLatLong(g.Latitude, g.Longitude, 1);

                        if (eventos.Count() > 0)
                        {
                            foreach (var e in eventos)
                            {
                                if (e.Ativo && (e.DataHoraInicio.AddHours(-1.5)) <= diaHoraInicio && (e.DataHoraFim.AddHours(1.5)) >= diaHoraFim)
                                {
                                    eve.Add(e);
                                }
                            }
                        }
                    }

                    var eventosDistintos = eve.DistinctBy(e => e.EventoId).ToList();

                    bool jahTemEvento1 = false;
                    bool jahTemEvento2 = false;
                    bool jahTemEvento3 = false;
                    bool jahTemEvento4 = false;

                    foreach (var e in eventosDistintos)
                    {
                        if (e.CategoriaEvento == 1 && !jahTemEvento1)
                        {
                            media = media * (20 / 100) + media;
                            precoDoUsuario = precoDoUsuario * (20 / 100) + precoDoUsuario;
                            jahTemEvento1 = true;
                            totalPorcento = totalPorcento + 20;
                        }
                        else if ((e.CategoriaEvento == 2 || e.CategoriaEvento == 4 || e.CategoriaEvento == 7 || e.CategoriaEvento == 8) && !jahTemEvento2)
                        {
                            media = media * (25 / 100) + media;
                            precoDoUsuario = precoDoUsuario * (25 / 100) + precoDoUsuario;
                            jahTemEvento2 = true;
                            totalPorcento = totalPorcento + 25;
                        }
                        else if (e.CategoriaEvento == 3 && !jahTemEvento3)
                        {
                            media = media * (28 / 100) + media;
                            precoDoUsuario = precoDoUsuario * (28 / 100) + precoDoUsuario;
                            jahTemEvento3 = true;
                            totalPorcento = totalPorcento + 28;
                        }
                        else if ((e.CategoriaEvento == 5 || e.CategoriaEvento == 6) && !jahTemEvento4)
                        {
                            media = media * (40 / 100) + media;
                            precoDoUsuario = precoDoUsuario * (40 / 100) + precoDoUsuario;
                            jahTemEvento4 = true;
                            totalPorcento = totalPorcento + 40;
                        }
                    }
                }

                if (media == 0)
                {
                    media = Decimal.Parse(precoDoUsuario.ToString());
                }

                var a = (media == 0) ? auxPrecoAvulso : (Decimal.Round(valorTotal,2) / qtdeVagas);
                var c = (mediaAposPrimeiraHora == 0) ? auxAposPrimeiraHora : totalAposPrimeiraHora / qtdeVagas;
                                
                var aCalulado = a * ((Decimal.Parse(taxaLucro.ToString()) + totalPorcento) / 100) + a;
                var cCalculado = c * ((Decimal.Parse(taxaLucro.ToString()) + totalPorcento) / 100) + c;
                var mediaRecalculada = 0.0m;

                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    var horas = diaHoraFim - diaHoraInicio;
                    var qtdeHoras = TimeSpan.Parse(horas.ToString()).TotalHours == 0 ? 1 : TimeSpan.Parse(horas.ToString()).TotalHours;

                    qtdeHoras = (int)Math.Round(qtdeHoras) - 1;

                    var b = qtdeHoras == 1 ? "Hora" : "Horas";

                    mediaRecalculada = Decimal.Round(Decimal.Parse(aCalulado.ToString()), 2) + (Decimal.Parse(qtdeHoras.ToString()) * Decimal.Round(Decimal.Parse(cCalculado.ToString()), 2));

                    if (qtdeHoras >= 1)
                    {
                        double k = (Double.Parse(totalPorcento.ToString()) / 2) / 100;
                        MensagemResumoLocatario = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(aCalulado.ToString()), 2) + " + " + qtdeHoras + " " + b + " x " + "R$" + Decimal.Round(Decimal.Parse(cCalculado.ToString()), 2);
                        MensagemResumoLocador = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(a.ToString()) * Decimal.Parse(k.ToString()) + Decimal.Parse(a.ToString()), 2) + " + " + qtdeHoras + " " + b + " x " + "R$" + Decimal.Round(Decimal.Parse(c.ToString()) * Decimal.Parse(k.ToString()) + Decimal.Parse(c.ToString()), 2);
                        var q = Decimal.Round(Decimal.Parse(a.ToString()), 2) + (Decimal.Parse(qtdeHoras.ToString()) * Decimal.Round(Decimal.Parse(c.ToString()), 2));
                        ValorLocacaoLocador = q * Decimal.Parse(k.ToString()) + q;
                    }
                    else
                    {
                        double k = (Double.Parse(totalPorcento.ToString()) / 2) / 100;
                        MensagemResumoLocatario = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(aCalulado.ToString()), 2);
                        MensagemResumoLocador = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(a.ToString()) * Decimal.Parse(k.ToString()) + Decimal.Parse(a.ToString()), 2);
                        var q = Decimal.Round(Decimal.Parse(a.ToString()), 2);
                        ValorLocacaoLocador = q * Decimal.Parse(k.ToString()) + q;
                    }
                }
                else
                {
                    double k = (Double.Parse(totalPorcento.ToString()) / 2) / 100;
                    MensagemResumoLocatario = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(aCalulado.ToString()),2) + ". Demais horas " + "R$" + Decimal.Round(Decimal.Parse(cCalculado.ToString()),2) + " cada.";
                    MensagemResumoLocador = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(a.ToString()) * Decimal.Parse(k.ToString()) + Decimal.Parse(a.ToString()), 2) + ". Demais horas " + "R$" + Decimal.Round(Decimal.Parse(c.ToString()) * Decimal.Parse(k.ToString()) + Decimal.Parse(c.ToString()), 2) + " cada.";
                    var q = Decimal.Round(Decimal.Parse(a.ToString()), 2);
                    ValorLocacaoLocador = q * Decimal.Parse(k.ToString()) + q;
                }

                //return Decimal.Round(Decimal.Parse(media.ToString()),2);
                return Decimal.Round(Decimal.Parse(mediaRecalculada.ToString()), 2);
            }
            else
            {
                var precoAvulsoCalculado = auxPrecoAvulso * (Decimal.Parse(taxaLucro.ToString()) / 100) + auxPrecoAvulso;
                var primeiraHoraCalculada = auxAposPrimeiraHora * (Decimal.Parse(taxaLucro.ToString()) / 100) + auxAposPrimeiraHora;
                var precoUsuarioRecalculado = 0.0m;

                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    var horas = diaHoraFim - diaHoraInicio;
                    var qtdeHoras = TimeSpan.Parse(horas.ToString()).TotalHours == 0 ? 1 : TimeSpan.Parse(horas.ToString()).TotalHours;
                                        
                    qtdeHoras = (int)Math.Round(qtdeHoras) - 1;
                    var b = qtdeHoras == 1 ? "Hora" : "Horas";

                    precoUsuarioRecalculado = Decimal.Round(Decimal.Parse(precoAvulsoCalculado.ToString()), 2) + (Decimal.Parse(qtdeHoras.ToString()) * Decimal.Round(Decimal.Parse(primeiraHoraCalculada.ToString()), 2));

                    if (qtdeHoras >= 1)
                    {
                        MensagemResumoLocatario = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(precoAvulsoCalculado.ToString()), 2) + " + " + qtdeHoras + " " + b + " x " + "R$" + Decimal.Round(Decimal.Parse(primeiraHoraCalculada.ToString()), 2);
                        MensagemResumoLocador = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(auxPrecoAvulso.ToString()), 2) + " + " + qtdeHoras + " " + b + " x " + "R$" + Decimal.Round(Decimal.Parse(auxAposPrimeiraHora.ToString()), 2);
                        ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxPrecoAvulso.ToString()), 2) + (Decimal.Parse(qtdeHoras.ToString()) * Decimal.Round(Decimal.Parse(auxAposPrimeiraHora.ToString()), 2));
                    }
                    else
                    {
                        MensagemResumoLocatario = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(precoAvulsoCalculado.ToString()), 2);
                        MensagemResumoLocador = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(auxPrecoAvulso.ToString()), 2);
                        ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxPrecoAvulso.ToString()), 2);
                    }
                }
                else
                {
                    MensagemResumoLocatario = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(precoAvulsoCalculado.ToString()),2) + ". Demais horas " + "R$" + Decimal.Round(Decimal.Parse(primeiraHoraCalculada.ToString()),2) + " cada.";
                    MensagemResumoLocador = "1ª Hora: R$" + Decimal.Round(Decimal.Parse(auxPrecoAvulso.ToString()), 2) + ". Demais horas " + "R$" + Decimal.Round(Decimal.Parse(auxAposPrimeiraHora.ToString()), 2) + " cada.";
                    ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxPrecoAvulso.ToString()), 2);
                }

                //return Decimal.Round(Decimal.Parse(precoDoUsuario.ToString()),2);
                return Decimal.Round(Decimal.Parse(precoUsuarioRecalculado.ToString()), 2);
            }
        }

        private decimal CalculaValorLocacaoDiaria(Vaga vaga, List<Garagem> garagens, int usuarioLocatarioId, DateTime? diaHoraInicio, DateTime? diaHoraFim, int taxaLucro)
        {
            var valorTotal = 0.0m;
            var qtdeVagas = 0;
            //var taxaLucro = 20;
            var auxPrecoDiaria = vaga.PrecoDiaria;

            foreach (var g in garagens)
            {
                var vagas = g.Vagas;

                foreach (var v in vagas)
                {
                    if (v.Coberta == vaga.Coberta && v.PortaoAutomatico == vaga.PortaoAutomatico && v.Diaria)
                    {
                        valorTotal = Decimal.Round(valorTotal + Decimal.Parse(v.PrecoDiaria.ToString()),2);
                        qtdeVagas++;
                    }
                }
            }

            var media = Decimal.Round(valorTotal,2) / qtdeVagas;
            var auxMedia = media;
            //taxaLucro = CalculaTaxaDeLucro(vaga.UsuarioId, taxaLucro);

            //taxaLucro = CalculaTaxaDeLucro(usuarioLocatarioId, taxaLucro);

            if (diaHoraInicio != null && diaHoraFim != null)
            {
                var dias = diaHoraFim - diaHoraInicio;

                var qtdeDias = TimeSpan.Parse(dias.ToString()).TotalDays == 0 ? 1 : TimeSpan.Parse(dias.ToString()).TotalDays;

                qtdeDias = Convert.ToInt32(qtdeDias) == 0 ? 1 : qtdeDias;
                //media = media * Decimal.Parse(qtdeDias.ToString());
                media = media * Convert.ToInt32(qtdeDias);

                //vaga.PrecoDiaria = vaga.PrecoDiaria * Decimal.Parse(qtdeDias.ToString());
                vaga.PrecoDiaria = vaga.PrecoDiaria * Convert.ToInt32(qtdeDias);
            }

            media = media * (Decimal.Parse(taxaLucro.ToString()) / 100) + media;

            var precoDoUsuario = vaga.PrecoDiaria * (Decimal.Parse(taxaLucro.ToString()) / 100) + vaga.PrecoDiaria;

            if (vaga.AceitaSugestaoDePreco)
            {
                if (media == 0)
                {
                    media = Decimal.Parse(precoDoUsuario.ToString());
                }

                var precoDiariaCalculado = auxMedia * (Decimal.Parse(taxaLucro.ToString()) / 100) + auxMedia;

                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    var dias = diaHoraFim - diaHoraInicio;
                    var a = TimeSpan.Parse(dias.ToString()).TotalDays == 0 ? 1 : TimeSpan.Parse(dias.ToString()).TotalDays;
                    var b = Convert.ToInt32(a) == 1 ? "Dia" : "Dias";

                    MensagemResumoLocatario = Convert.ToInt32(a) + " " + b + " x R$" + Decimal.Round(Decimal.Parse(precoDiariaCalculado.ToString()),2);
                    MensagemResumoLocador = Convert.ToInt32(a) + " " + b + " x R$" + Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                    ValorLocacaoLocador = Convert.ToInt32(a) * Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                }
                else
                {
                    MensagemResumoLocatario = "1 Dia: R$" + Decimal.Round(Decimal.Parse(precoDiariaCalculado.ToString()),2);
                    MensagemResumoLocador = "1 Dia: R$" + Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                    ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxMedia.ToString()), 2);
                }

                return Decimal.Round(Decimal.Parse(media.ToString()),2);
            }
            else
            {
                var precoDiariaCalculado = auxPrecoDiaria * (Decimal.Parse(taxaLucro.ToString()) / 100) + auxPrecoDiaria;

                if (diaHoraInicio != null && diaHoraFim != null)
                {
                    var dias = diaHoraFim - diaHoraInicio;
                    var a = TimeSpan.Parse(dias.ToString()).TotalDays == 0 ? 1 : TimeSpan.Parse(dias.ToString()).TotalDays;
                    var b = Convert.ToInt32(a) == 1 ? "Dia" : "Dias";

                    MensagemResumoLocatario = Convert.ToInt32(a) + " " + b + " x R$" + Decimal.Round(Decimal.Parse(precoDiariaCalculado.ToString()),2);
                    MensagemResumoLocador = Convert.ToInt32(a) + " " + b + " x R$" + Decimal.Round(Decimal.Parse(auxPrecoDiaria.ToString()), 2);
                    ValorLocacaoLocador = Convert.ToInt32(a) * Decimal.Round(Decimal.Parse(auxPrecoDiaria.ToString()), 2);
                }
                else
                {
                    MensagemResumoLocatario = "1 Dia: R$" + Decimal.Round(Decimal.Parse(precoDiariaCalculado.ToString()),2);
                    MensagemResumoLocador = "1 Dia: R$" + Decimal.Round(Decimal.Parse(auxPrecoDiaria.ToString()), 2);
                    ValorLocacaoLocador = Decimal.Round(Decimal.Parse(auxPrecoDiaria.ToString()), 2);
                }

                return Decimal.Round(Decimal.Parse(precoDoUsuario.ToString()),2);
            }
        }

        /*private decimal CalculaValorMultaAvulso(int idLocacao)
        {
            return _locacaoService.GetById(idLocacao).ValorLocacaoLocatario * (1 / 100);
        }*/

        private decimal CalculaValorMultaCancelamentoMensal(int idLocacao)
        {
            return _locacaoService.GetById(idLocacao).ValorLocacaoLocatario * 7.0m / 100;
        }

        private decimal CalculaValorMultaCancelamentoAvulso(int idLocacao)
        {
            return _locacaoService.GetById(idLocacao).ValorLocacaoLocatario * 13.0m / 100;
        }

        private decimal CalculaValorMultaCancelamentoDiaria(int idLocacao)
        {
            return _locacaoService.GetById(idLocacao).ValorLocacaoLocatario * 10.0m / 100;
        }

        private int CalculaTaxaDeLucro(int idUsuario, int taxaLucro)
        {
            var qtdeLocacoes = 0;
            var qtdeCancelamento = 0;
            var qtdeLocacoesComoLocador = 0;
            var qtdeLocacoesComoLocatario = 0;
            var qtdeCancelamentoComoLocador = 0;
            var qtdeCancelamentoComoLocatario = 0;
            var somaNotaAvaliacao = 0;

            var garagens = _garagemService.GetByIdUser(idUsuario);

            foreach (var g in garagens)
            {
                var vaga = _vagaService.GetByIdGarageAndIdUser(g.GaragemId, idUsuario);

                foreach (var v in vaga)
                {
                    qtdeLocacoesComoLocador = qtdeLocacoesComoLocador + _locacaoService.GetByIdVaga(v.VagaId).Where(l => l.Aprovada == true).Count();
                    qtdeCancelamentoComoLocador = qtdeCancelamentoComoLocador + _locacaoService.GetByIdVaga(v.VagaId).Where(l => l.Cancelada == true && l.QuemCancelou == idUsuario).Count();
                }
            }

            var veiculos = _veiculoService.GetByIdUsuario(idUsuario);

            foreach (var c in veiculos)
            {
                qtdeLocacoesComoLocatario = qtdeLocacoesComoLocatario + _locacaoService.GetByIdVeiculo(c.VeiculoId).Where(l => l.Aprovada == true).Count();
                qtdeCancelamentoComoLocatario = qtdeCancelamentoComoLocatario + _locacaoService.GetByIdVeiculo(c.VeiculoId).Where(l => l.Cancelada == true && l.QuemCancelou == idUsuario).Count();
            }

            qtdeLocacoes = qtdeLocacoesComoLocador + qtdeLocacoesComoLocatario;
            qtdeCancelamento = qtdeCancelamentoComoLocador + qtdeCancelamentoComoLocatario;

            if (qtdeLocacoes > 10 && qtdeLocacoes < 50)
            {
                taxaLucro = taxaLucro - 1;
            }
            else if (qtdeLocacoes >= 50 && qtdeLocacoes <= 100)
            {
                taxaLucro = taxaLucro - 2;
            }
            else if (qtdeLocacoes > 100 && qtdeLocacoes <= 200)
            {
                taxaLucro = taxaLucro - 3;
            }
            else if (qtdeLocacoes > 200)
            {
                taxaLucro = taxaLucro - 4;
            }

            if (qtdeCancelamento >= 3 && qtdeCancelamento < 10)
            {
                taxaLucro = taxaLucro + 1;
            }
            else if (qtdeCancelamento >= 10 && qtdeCancelamento <= 15)
            {
                taxaLucro = taxaLucro + 2;
            }
            else if (qtdeCancelamento > 15 && qtdeCancelamento <= 20)
            {
                taxaLucro = taxaLucro + 3;
            }
            else if (qtdeCancelamento > 20)
            {
                taxaLucro = taxaLucro + 4;
            }

            var avaliacao = _avaliacaoService.GetEvaluationUser(idUsuario, true, true);

            var qtdeAvaliacao = avaliacao.Count();

            foreach (var a in avaliacao)
            {
                somaNotaAvaliacao = somaNotaAvaliacao + a.Nota;
            }

            if (qtdeAvaliacao > 0)
            {
                var media = somaNotaAvaliacao / qtdeAvaliacao;

                if (media >= 3.0 && media < 4.0)
                {
                    taxaLucro = taxaLucro - 1;
                }
                else if (media >= 4.0)
                {
                    taxaLucro = taxaLucro - 2;
                }
            }

            if (taxaLucro > 20)
            {
                taxaLucro = 20;
            }
            if (taxaLucro < 10)
            {
                taxaLucro = 10;
            }

            ValorTaxaLucro = taxaLucro;

            return taxaLucro;
        }

        private bool VerificaSeEstaOcupada(Vaga vaga, DateTime? diaHoraInicio, DateTime? diaHoraFim)
        {
            bool podeAdicionar = true;
            var loc = _locacaoService.GetByIdVaga(vaga.VagaId);

            foreach (var l in loc)
            {
                if (podeAdicionar && l.Ativo == false)
                {
                    podeAdicionar = true;
                }
                else
                {
                    if (l.DiaHoraFim == null)
                    {
                        if (podeAdicionar && diaHoraFim != null && diaHoraInicio < l.DiaHoraInicio && diaHoraFim <= l.DiaHoraInicio)
                        {
                            podeAdicionar = true;
                        }
                        else
                        {
                            podeAdicionar = false;
                        }
                    }
                    else
                    {
                        if (diaHoraFim == null)
                        {
                            if (podeAdicionar && l.DiaHoraInicio < diaHoraInicio && l.DiaHoraFim <= diaHoraInicio)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else
                        {
                            if
                            (
                                podeAdicionar &&
                                (
                                    l.DiaHoraInicio < diaHoraInicio && l.DiaHoraFim <= diaHoraInicio ||
                                    l.DiaHoraInicio >= diaHoraFim && l.DiaHoraFim > diaHoraFim
                                )
                            )
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                    }
                }
            }

            return podeAdicionar;
        }

        private List<LocacaoViewModel> FiltroLocacao(IQueryable<Locacao> locacoes, List<LocacaoViewModel> listLocacaoViewModel, bool aprovada, bool reprovada, bool aguardandoAprovacao, bool ativa, bool finalizada, bool avulso, bool mensal, bool diaria, bool cancelada, bool desistiu)
        {
            bool podeAdicionar = true;
            foreach (var l in locacoes)
            {
                podeAdicionar = true;
                if (aprovada == false && reprovada == false && ativa == false && finalizada == false && avulso == false && mensal == false && diaria == false && cancelada == false && aguardandoAprovacao == false && desistiu == false)
                {                    
                    var locacaoViewModel = Mapper.Map<Locacao, LocacaoViewModel>(l);
                    listLocacaoViewModel.Add(locacaoViewModel);
                }
                else
                {
                    if (podeAdicionar)
                    {
                        if (aprovada == true && reprovada == false && aguardandoAprovacao == false && cancelada == false && desistiu == false)
                        {
                            if (l.Aprovada == true && l.Cancelada == false && l.Desistencia == false)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else if (aprovada == false && reprovada == true && aguardandoAprovacao == false && cancelada == false && desistiu == false)
                        {
                            if (!l.Aprovada == true && l.Cancelada == false && l.Desistencia == false)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else if(aprovada == false && reprovada == false && aguardandoAprovacao == true && cancelada == false && desistiu == false)
                        {
                            if (l.Aprovada == null && l.Cancelada == false && l.Desistencia == false)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else if(aprovada == false && reprovada == false && aguardandoAprovacao == false && cancelada == true && desistiu == false)
                        {
                            if(l.Cancelada == true)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else if(aprovada == false && reprovada == false && aguardandoAprovacao == false && cancelada == false && desistiu == true)
                        {
                            if(l.Desistencia == true)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else
                        {
                            podeAdicionar = true;
                        }
                    }
                    if (podeAdicionar)
                    {
                        if (ativa == true && finalizada == false)
                        {
                            if (l.Ativo)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else if (ativa == false && finalizada == true)
                        {
                            if (!l.Ativo)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        else
                        {
                            podeAdicionar = true;
                        }
                    }
                    /*if (podeAdicionar)
                    {
                        if (cancelada)
                        {
                            if (l.Cancelada)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                    }*/
                    if (podeAdicionar)
                    {
                        if (avulso == false && mensal == false && diaria == false)
                        {
                            podeAdicionar = true;
                        }
                        else
                        {
                            if (avulso == true && l.ModalidadeLocacao == 1 || mensal == true && l.ModalidadeLocacao == 3 || diaria == true && l.ModalidadeLocacao == 2)
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                    }
                    if (podeAdicionar)
                    {
                        var locacaoViewModel = Mapper.Map<Locacao, LocacaoViewModel>(l);
                        listLocacaoViewModel.Add(locacaoViewModel);
                    }
                }
            }

            return listLocacaoViewModel;
        }

        private List<GaragemLocacaoViewModel> RetornaLista( double latitude,
                                                            double longitude,
                                                            bool coberta,
                                                            bool descoberta,
                                                            bool portaoAutomatico,
                                                            bool portaoManual,
                                                            bool avulso,
                                                            bool mensal,
                                                            bool diaria,
                                                            decimal? valorAte,
                                                            double? distancia,
                                                            DateTime? diaHoraInicio,
                                                            DateTime? diaHoraFim,
                                                            int idUsuarioLocatario
                                                          )
        {
            
            List<Garagem> gar = new List<Garagem>();
            List<GaragemLocacaoViewModel> listaGaragens = new List<GaragemLocacaoViewModel>();
            int taxaLucro = 20;
            double media = 0.0;

            var garagens = _garagemService.GetByLatLong(latitude, longitude, distancia);

            if (garagens.Count > 0)
            {
                bool podeAdicionar = true;

                for (int i = 0; i < garagens.Count; i++)
                {
                    List<Vaga> vag = new List<Vaga>();
                    if (garagens[i].Ativo == true)
                    {
                        garagens[i].Vagas = _vagaService.GetByIdGarage(garagens[i].GaragemId);
                            
                        foreach (var v in garagens[i].Vagas)
                        {
                            if(v.Aceita == true)
                            {
                                podeAdicionar = true;

                                if (
                                    coberta == false &&
                                    descoberta == false &&
                                    portaoAutomatico == false &&
                                    portaoManual == false &&
                                    avulso == false &&
                                    mensal == false &&
                                    diaria == false &&
                                    valorAte == null &&
                                    diaHoraInicio == null &&
                                    diaHoraFim == null
                                    )
                                {

                                    var diaInicio = DateTime.Now;
                                    var diaFim = DateTime.Now.AddHours(1);

                                    podeAdicionar = VerificaSeEstaOcupada(v, diaInicio, diaFim);

                                    if (podeAdicionar)
                                    {
                                        vag.Add(v);
                                    }
                                }
                                else
                                {
                                    if (podeAdicionar)
                                    {
                                        if (coberta == true && descoberta == false)
                                        {
                                            if (v.Coberta)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (coberta == false && descoberta == true)
                                        {
                                            if (!v.Coberta)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else
                                        {
                                            podeAdicionar = true;
                                        }
                                    }
                                    if (podeAdicionar)
                                    {
                                        if (portaoAutomatico == true && portaoManual == false)
                                        {
                                            if (v.PortaoAutomatico)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (portaoAutomatico == false && portaoManual == true)
                                        {
                                            if (!v.PortaoAutomatico)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else
                                        {
                                            podeAdicionar = true;
                                        }
                                    }
                                    if (podeAdicionar)
                                    {
                                        if (avulso == true && mensal == false && diaria == false)
                                        {
                                            if (v.Avulso)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (avulso == false && mensal == true && diaria == false)
                                        {
                                            if (v.Mensal)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (avulso == false && mensal == false && diaria == true)
                                        {
                                            if (v.Diaria)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (avulso == true && mensal == true && diaria == false)
                                        {
                                            if (v.Avulso || v.Mensal)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (avulso == true && mensal == false && diaria == true)
                                        {
                                            if (v.Avulso || v.Diaria)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else if (avulso == false && mensal == true && diaria == true)
                                        {
                                            if (v.Mensal || v.Diaria)
                                            {
                                                podeAdicionar = true;
                                            }
                                            else
                                            {
                                                podeAdicionar = false;
                                            }
                                        }
                                        else
                                        {
                                            podeAdicionar = true;
                                        }
                                    }
                                    if (podeAdicionar)
                                    {
                                        /*if (diaHoraInicio != null && diaHoraFim != null)
                                        {
                                            podeAdicionar = VerificaSeEstaOcupada(v, diaHoraInicio, diaHoraFim);
                                        }
                                        else if (diaHoraInicio != null && diaHoraFim == null)
                                        {
                                            podeAdicionar = VerificaSeEstaOcupada(v, diaHoraInicio, diaHoraInicio);
                                        }
                                        else if (diaHoraInicio == null && diaHoraFim != null)
                                        {
                                            podeAdicionar = VerificaSeEstaOcupada(v, diaHoraFim, diaHoraFim);
                                        }
                                        else
                                        {
                                            podeAdicionar = true;
                                        }*/
                                        if (diaHoraInicio == null && diaHoraFim == null)
                                        {
                                            var diaInicio = DateTime.Now;
                                            var diaFim = DateTime.Now.AddHours(1);

                                            podeAdicionar = VerificaSeEstaOcupada(v, diaInicio, diaFim);
                                        }
                                        else
                                        {
                                            podeAdicionar = VerificaSeEstaOcupada(v, diaHoraInicio, diaHoraFim);
                                        }
                                    }
                                    if (podeAdicionar)
                                    {
                                        vag.Add(v);
                                    }
                                }
                            }
                        }
                        if (vag.Count > 0)
                        {
                            garagens[i].Vagas = vag;
                            gar.Add(garagens[i]);
                        }
                    }                        
                }

                bool verificaTaxa = true;
                bool calculaMedia = true;

                foreach (var g in gar)
                {
                    GaragemLocacaoViewModel gara = new GaragemLocacaoViewModel();
                    List<VagaLocacaoViewModel> listaVagas = new List<VagaLocacaoViewModel>();

                    gara.Garagem = g;

                    verificaTaxa = true;
                    calculaMedia = true;

                    foreach (var v in g.Vagas)
                    {
                        if(verificaTaxa || g.Condominio)
                        {
                            taxaLucro = CalculaTaxaDeLucro(v.UsuarioId, taxaLucro);
                            taxaLucro = CalculaTaxaDeLucro(idUsuarioLocatario, taxaLucro);

                            verificaTaxa = false;
                        }
                            
                        if(calculaMedia || g.Condominio)
                        {
                            var avaliacoes = _avaliacaoService.GetEvaluationUser(v.UsuarioId, true, false);
                            var total = 0.0;

                            foreach(var a in avaliacoes)
                            {
                                total = total + a.Nota;
                            }

                            if(avaliacoes.Count() > 0)
                            {
                                media = total / avaliacoes.Count();
                            }                                

                            calculaMedia = false;
                        }

                        VagaLocacaoViewModel vlvm = new VagaLocacaoViewModel();

                        vlvm.Vaga = new Vaga();

                        vlvm.Vaga.Aceita = v.Aceita;
                        vlvm.Vaga.AceitaSugestaoDePreco = v.AceitaSugestaoDePreco;
                        vlvm.Vaga.AposPrimeiraHora = v.AposPrimeiraHora;
                        vlvm.Vaga.Ativo = v.Ativo;
                        vlvm.Vaga.Avulso = v.Avulso;
                        vlvm.Vaga.Coberta = v.Coberta;
                        vlvm.Vaga.Diaria = v.Diaria;
                        vlvm.Vaga.Garagem = v.Garagem;
                        vlvm.Vaga.GaragemId = v.GaragemId;
                        vlvm.Vaga.Locacoes = v.Locacoes;
                        vlvm.Vaga.Mensal = v.Mensal;
                        vlvm.Vaga.NumeroVaga = v.NumeroVaga;
                        vlvm.Vaga.Observacao = v.Observacao;
                        vlvm.Vaga.PortaoAutomatico = v.PortaoAutomatico;
                        vlvm.Vaga.PrecoAvulso = v.PrecoAvulso;
                        vlvm.Vaga.PrecoDiaria = v.PrecoDiaria;
                        vlvm.Vaga.PrecoMensal = v.PrecoMensal;
                        vlvm.Vaga.Usuario = v.Usuario;
                        vlvm.Vaga.UsuarioId = v.UsuarioId;
                        vlvm.Vaga.VagaId = v.VagaId;

                        vlvm.Media = media;

                        if (v.Avulso && (avulso == true || avulso == false && mensal == false && diaria == false))
                        {
                            vlvm.Vaga.PrecoAvulso = Decimal.Round(CalculaValorLocacaoAvulso(vlvm.Vaga, gar, idUsuarioLocatario, diaHoraInicio, diaHoraFim, taxaLucro),2);
                            vlvm.TaxaLucro = ValorTaxaLucro;
                            taxaLucro = 20;
                        }
                        if (v.Mensal && (mensal == true || avulso == false && mensal == false && diaria == false))
                        {
                            vlvm.Vaga.PrecoMensal = Decimal.Round(CalculaValorLocacaoMensal(vlvm.Vaga, gar, idUsuarioLocatario, diaHoraInicio, diaHoraFim, taxaLucro), 2);
                            vlvm.TaxaLucro = ValorTaxaLucro;
                            taxaLucro = 20;
                        }
                        if (v.Diaria && (diaria == true || avulso == false && mensal == false && diaria == false))
                        {
                            vlvm.Vaga.PrecoDiaria = Decimal.Round(CalculaValorLocacaoDiaria(vlvm.Vaga, gar, idUsuarioLocatario, diaHoraInicio, diaHoraFim, taxaLucro), 2);
                            vlvm.TaxaLucro = ValorTaxaLucro;
                            taxaLucro = 20;
                        }

                        podeAdicionar = true;

                        if (valorAte != null)
                        {
                            if (avulso == true && mensal == false && diaria == false && vlvm.Vaga.Avulso && valorAte >= vlvm.Vaga.PrecoAvulso)
                            {
                                podeAdicionar = true;
                            }
                            else if (avulso == false && mensal == true && diaria == false && vlvm.Vaga.Mensal && valorAte >= vlvm.Vaga.PrecoMensal)
                            {
                                podeAdicionar = true;
                            }
                            else if (avulso == false && mensal == false && diaria == true && vlvm.Vaga.Diaria && valorAte >= vlvm.Vaga.PrecoDiaria)
                            {
                                podeAdicionar = true;
                            }
                            else if (
                                    avulso == false && mensal == false && diaria == false && (valorAte >= vlvm.Vaga.PrecoAvulso || valorAte >= vlvm.Vaga.PrecoMensal || valorAte >= vlvm.Vaga.PrecoDiaria) ||
                                    avulso == true && mensal == true && diaria == true && (valorAte >= vlvm.Vaga.PrecoAvulso || valorAte >= vlvm.Vaga.PrecoMensal || valorAte >= vlvm.Vaga.PrecoDiaria)
                                    )
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                podeAdicionar = false;
                            }
                        }
                        if(podeAdicionar)
                        {
                            listaVagas.Add(vlvm);
                        }
                    }
                    if(listaVagas.Count > 0)
                    {
                        gara.VagaETaxa = listaVagas;
                        listaGaragens.Add(gara);
                    }
                }
            }

            return listaGaragens;
        }

        private string ValidaDadosDeEntrada(LocacaoViewModel locacao)
        {
            if(string.IsNullOrEmpty(locacao.DiaHoraInicio))
            {
                return "Favor informar o dia e horário de início da Locação.";
            }
            if (string.IsNullOrEmpty(locacao.ValorLocacaoLocatario))
            {
                return "Favor informar o valor da Locação para o locatario.";
            }
            else
            {
                decimal n = 0;
                bool teste = decimal.TryParse(locacao.ValorLocacaoLocatario, out n);

                if(!teste)
                {
                    return "Valor de Locação inválido.";
                }
            }
            if (string.IsNullOrEmpty(locacao.ValorLocacaoLocador))
            {
                return "Favor informar o valor da Locação para o locador.";
            }
            else
            {
                decimal n = 0;
                bool teste = decimal.TryParse(locacao.ValorLocacaoLocador, out n);

                if (!teste)
                {
                    return "Valor de Locação inválido.";
                }
            }

            if (!string.IsNullOrEmpty(locacao.Multa))
            {
                decimal n = 0;
                bool teste = decimal.TryParse(locacao.Multa, out n);

                if (!teste)
                {
                    return "Valor de Multa inválido.";
                }
            }
            
            if(string.IsNullOrEmpty(locacao.ModalidadeLocacao) || locacao.ModalidadeLocacao == "0")
            {
                return "Favor informar a modalidade da Locação.";
            }

            return "";
        }
        #endregion
    }
}
 