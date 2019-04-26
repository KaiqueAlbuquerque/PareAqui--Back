using AutoMapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.Business;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class EventoController : Controller
    {
        private readonly IEventoService _eventoService;
        private readonly IGaragemService _garagemService;
        private readonly IVagaService _vagaService;
        private readonly ILocacaoService _locacaoService;
        private readonly IFotoService _fotoService;
        private readonly INotificacaoEmailSmsService _notificacaoEmailSms;
        private readonly IUsuarioService _usuarioService;

        public EventoController(IEventoService eventoService, 
                                IGaragemService garagemService, 
                                IVagaService vagaService, 
                                ILocacaoService locacaoService, 
                                IFotoService fotoService,
                                INotificacaoEmailSmsService notificacaoEmailSms,
                                IUsuarioService usuarioService
                                )
        {
            _eventoService = eventoService;
            _garagemService = garagemService;
            _vagaService = vagaService;
            _locacaoService = locacaoService;
            _fotoService = fotoService;
            _notificacaoEmailSms = notificacaoEmailSms;
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public string CadastrarEvento(EventoViewModel evento, List<Foto> fotos)
        {
            try
            {
                if(ValidaDadosDeEntrada(evento) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(evento) });
                }
                else
                {
                    bool podeAdicionar = true;
                    Evento ev = new Evento();
                    evento.Latitude = evento.Latitude.Replace(".", ",");
                    evento.Longitude = evento.Longitude.Replace(".", ",");
                    evento.Ativo = false;
                    evento.Aprovado = null;

                    var eventoService = Mapper.Map<EventoViewModel, Evento>(evento);
                    var retornoConsultaEvento = VerificaSeEventoJaExiste(eventoService);

                    foreach (var v in retornoConsultaEvento)
                    {
                        if (
                            podeAdicionar &&
                            (eventoService.DataHoraInicio <= v.DataHoraInicio && eventoService.DataHoraFim <= v.DataHoraInicio) ||
                            (eventoService.DataHoraInicio >= v.DataHoraFim && eventoService.DataHoraFim >= v.DataHoraFim)
                            )
                        {
                            podeAdicionar = true;
                        }
                        else
                        {
                            ev = v;
                            podeAdicionar = false;
                        }
                    }

                    if (podeAdicionar)
                    {
                        _eventoService.Add(eventoService);
                        int i = 0;

                        try
                        {
                            if (fotos != null)
                            {
                                foreach (var f in fotos)
                                {
                                    if (!string.IsNullOrEmpty(f.Imagem))
                                    {
                                        var fot = new Foto();

                                        //int inicio = f.Imagem.IndexOf(",");
                                        var nomeImagem = eventoService.EventoId.ToString() + eventoService.NomeEvento + i.ToString() + ".jpg";

                                        MD5 md5 = System.Security.Cryptography.MD5.Create();

                                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(nomeImagem);
                                        byte[] hash = md5.ComputeHash(inputBytes);

                                        StringBuilder sb = new StringBuilder();
                                        for (int z = 0; z < hash.Length; z++)
                                        {
                                            sb.Append(hash[z].ToString("X2"));
                                        }

                                        var nomeCriptografado = sb.ToString();

                                        //if (ConvertStringtoImage(f.Imagem.Substring(inicio + 1), nomeCriptografado))
                                        if (ConvertStringtoImage(f.Imagem, nomeCriptografado))
                                        {
                                            fot.Imagem = nomeCriptografado;
                                            fot.Tipo = 1;
                                            fot.EventoId = eventoService.EventoId;

                                            _fotoService.Add(fot);
                                        }
                                        else
                                        {
                                            var evto = _eventoService.GetById(eventoService.EventoId);

                                            if (evto != null)
                                            {
                                                var fo = _fotoService.GetAll().Where(ft => ft.EventoId == evto.EventoId);

                                                foreach (var fto in fo)
                                                {
                                                    _fotoService.Remove(fto);
                                                }

                                                _eventoService.Remove(evto);
                                            }

                                            return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao enviar as fotos do local do evento e por isso o mesmo não foi cadastrado. Por favor tente novamente." });
                                        }

                                        i++;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            var evto = _eventoService.GetById(eventoService.EventoId);

                            if(evto != null)
                            {
                                var fot = _fotoService.GetAll().Where(f => f.EventoId == evto.EventoId);

                                foreach (var f in fot)
                                {
                                    _fotoService.Remove(f);
                                }
                                                                
                                _eventoService.Remove(evto);
                            }

                            return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao enviar as fotos do local do evento e por isso o mesmo não foi cadastrado. Por favor tente novamente." });
                        }
                        return JsonConvert.SerializeObject(new { code = 200, message = "Evento cadastrado com sucesso. Aguarde a aprovação.", id = eventoService.EventoId });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas já existe um evento cadastrado neste local para este horário.", retornoEvento = ev });
                    }
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro no cadastro. Por favor, tente novamente." });
            }
        }

        public string ConsultarEvento(string cep, int numero, DateTime dataInicio)
        {
            try
            {
                var evento = _eventoService.CheckIfEventAlreadyExists(cep, numero, dataInicio).ToList();                
                return JsonConvert.SerializeObject(new { code = 200, eve = evento });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        public string ConsultarUmEvento(int idEvento)
        {
            try
            {
                List<string> img = new List<string>();

                var fotos = _fotoService.GetPhotosByIdEvent(idEvento);

                foreach (var f in fotos)
                {
                    img.Add(f.Imagem);
                }

                var evento = _eventoService.GetById(idEvento);
                var eventoViewModel = Mapper.Map<Evento, EventoViewModel>(evento);

                EventoAprovacaoViewModel e = new EventoAprovacaoViewModel();

                e.Evento = eventoViewModel;
                e.Imagens = img;

                return JsonConvert.SerializeObject(new { code = 200, evento = e });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarEvento(EventoViewModel evento)
        {
            try
            {
                if (ValidaDadosDeEntrada(evento) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(evento) });
                }
                else
                {
                    bool podeAdicionar = true;
                    Evento ev = new Evento();
                    evento.Latitude = evento.Latitude.Replace(".", ",");
                    evento.Longitude = evento.Longitude.Replace(".", ",");
                    evento.Ativo = false;
                    evento.Aprovado = null;

                    var eventoService = Mapper.Map<EventoViewModel, Evento>(evento);
                    var retornoConsultaEvento = VerificaSeEventoJaExiste(eventoService);

                    foreach (var v in retornoConsultaEvento)
                    {
                        if (v.EventoId != eventoService.EventoId)
                        {
                            if (
                                podeAdicionar &&
                                (eventoService.DataHoraInicio <= v.DataHoraInicio && eventoService.DataHoraFim <= v.DataHoraInicio) ||
                                (eventoService.DataHoraInicio >= v.DataHoraFim && eventoService.DataHoraFim >= v.DataHoraFim)
                               )
                            {
                                podeAdicionar = true;
                            }
                            else
                            {
                                ev = v;
                                podeAdicionar = false;
                            }
                        }
                    }

                    if (podeAdicionar)
                    {
                        _eventoService.Update(eventoService);
                        return JsonConvert.SerializeObject(new { code = 200, message = "Evento alterado com sucesso. Aguarde a aprovação." });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas já existe um evento cadastrado neste local para este horário.", retornoEvento = ev });
                    }
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar. Por favor, tente novamente." });
            }
        }
        
        [HttpPost]
        public string DesativarEvento(int idEvento)
        {
            try
            {
                if(_eventoService.DisableEvent(idEvento))
                {
                    return JsonConvert.SerializeObject(new { code = 200, message = "Evento desativado com sucesso." });
                }
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar. Por favor, tente novamente." });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> AprovarEvento(int idEvento)
        {
            try
            {
                bool podeAdicionar = true;
                Evento ev = new Evento();

                List<Usuario> usuarios = new List<Usuario>();

                var evento = _eventoService.GetById(idEvento);

                var retornoConsultaEvento = VerificaSeEventoJaExiste(evento);

                foreach (var v in retornoConsultaEvento)
                {
                    if (
                        podeAdicionar &&
                        (evento.DataHoraInicio <= v.DataHoraInicio && evento.DataHoraFim <= v.DataHoraInicio) ||
                        (evento.DataHoraInicio >= v.DataHoraFim && evento.DataHoraFim >= v.DataHoraFim)
                        )
                    {
                        podeAdicionar = true;
                    }
                    else
                    {
                        ev = v;
                        podeAdicionar = false;
                    }
                }

                if (podeAdicionar)
                {
                    evento.Aprovado = true;
                    evento.Ativo = true;

                    _eventoService.Update(evento);

                    var garagens = _garagemService.GetByLatLong(evento.Latitude, evento.Longitude, 1);

                    foreach (var g in garagens)
                    {
                        var vaga = _vagaService.GetByIdGarage(g.GaragemId);

                        foreach (var v in vaga)
                        {
                            var loc = _locacaoService.GetAll().Where(l => l.VagaId == v.VagaId &&
                                                                    ((l.DiaHoraInicio <= evento.DataHoraInicio && l.DiaHoraFim == null && l.Ativo) ||
                                                                    (l.DiaHoraInicio <= evento.DataHoraInicio && l.DiaHoraFim >= evento.DataHoraFim && l.Ativo)));

                            if(loc.Count() == 0)
                            {
                                usuarios.Add(v.Usuario);
                            }
                        }
                    }

                    var usuariosDistintos = usuarios.DistinctBy(e => e.UsuarioId).ToList();

                    foreach (var u in usuariosDistintos)
                    {
                        var notifiEmail = _notificacaoEmailSms.GetAll().Where(n => n.UsuarioId == u.UsuarioId && n.TipoNotificacao == 1 && n.MotivoNotificacao == 3 && n.DataCadastro >= DateTime.Now.AddHours(-2) && n.DataCadastro <= DateTime.Now);

                        if (notifiEmail.Count() == 0)
                        {

                            var assunto = "Novo Evento em sua região";
                            var mensagem = "Olá " + u.Nome + ". <br /><br /> Gostaríamos de informar que no dia " +
                                        evento.DataHoraInicio.ToString("dd/MM/yyyy") + " das " +
                                        evento.DataHoraInicio.ToString("HH:mm") + " às " +
                                        evento.DataHoraFim.ToString("HH:mm") + " ocorrerá um evento próximo da região na qual você possui vagas. <br /><br />" +
                                        "Constatamos que até o momento " +
                                        "sua vaga possui horários disponíveis no período em que irá ocorrer o evento e achamos que esta será uma ótima " +
                                        "oportunidade para você lucrar. Fique atento. <br /><br />" +
                                        "Dados do Evento: <br />" +
                                        "Nome: " + evento.NomeEvento + "<br />" +
                                        "Endereço: " + evento.Endereco +
                                        ", Nº " + evento.Numero + "<br />" +
                                        "CEP: " + evento.Cep + "<br /><br />" +
                                        "E-mail automático. Favor não responder.";

                            /*Notification n = new Notification();

                            if (u.AceitaReceberEmail)
                            { 
                                await n.SendMailAsync(u.Email, assunto, mensagem);
                            }*/

                            NotificacaoEmailSms nes = new NotificacaoEmailSms();

                            nes.MotivoNotificacao = 3;
                            nes.TipoNotificacao = 1;
                            nes.UsuarioId = u.UsuarioId;

                            _notificacaoEmailSms.Add(nes);
                        }

                        var notifiSms = _notificacaoEmailSms.GetAll().Where(n => n.UsuarioId == u.UsuarioId && n.TipoNotificacao == 2 && n.MotivoNotificacao == 3 && n.DataCadastro >= DateTime.Now.AddHours(-2) && n.DataCadastro <= DateTime.Now);

                        if (notifiSms.Count() == 0)
                        {
                            /*Notification n = new Notification();
                            if(u.AceitaReceberSms)
                            {
                                string sms = "Informamos que no dia " + evento.DataHoraInicio.ToString("dd/MM/yyyy") +
                                            " ocorrerá um evento próximo da região na qual você possui vagas. Para mais informações acesse o App PareAqui.";

                                n.SendSMS(u.Celular, sms);
                            }*/
                            NotificacaoEmailSms nes = new NotificacaoEmailSms();

                            nes.MotivoNotificacao = 3;
                            nes.TipoNotificacao = 2;
                            nes.UsuarioId = u.UsuarioId;

                            _notificacaoEmailSms.Add(nes);
                        }

                        var token = _usuarioService.GetById(u.UsuarioId).TokenPush;

                        if(token != null)
                        {
                            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("content-type", "application/json");
                            request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                            request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"Haverá um evento em sua região. É a oportunidade perfeita para você lucrar. Em instantes você receberá um e-mail com mais detalhes.\",\"title\": \"Oportunidade de lucar\" }}", ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);
                        }                        
                    }

                    var evts = _eventoService.GetEventsForApproval().Where(e => e.Cep == evento.Cep && e.Numero == evento.Numero);

                    foreach (var e in evts)
                    {
                        if (
                            e.DataHoraInicio < evento.DataHoraInicio && e.DataHoraFim <= evento.DataHoraInicio ||
                            e.DataHoraInicio >= evento.DataHoraFim && e.DataHoraFim > evento.DataHoraFim
                            )
                        {

                        }
                        else
                        {
                            var a = ReprovarEvento(e.EventoId, "Há outro Evento neste mesmo horário que já foi aprovado.");
                        }
                    }

                    return JsonConvert.SerializeObject(new { code = 200, message = "Evento aprovado com sucesso." });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas já existe um evento cadastrado neste local para este horário.", retornoEvento = ev });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao aprovar o Evento. Por favor tente novamente." });
            }
        }
        
        [HttpPost]
        public string ReprovarEvento(int idEvento, string motivoReprovado)
        {
            try
            {
                var evento = _eventoService.GetById(idEvento);

                evento.Aprovado = false;
                evento.Ativo = false;
                evento.MotivoReprovado = motivoReprovado;

                _eventoService.Update(evento);
                                
                return JsonConvert.SerializeObject(new { code = 200, message = "Evento reprovado com sucesso." });
            }
            catch(Exception e)
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao reprovar o Evento. Por favor tente novamente." });
            }

        }

        public string EventosNaoAprovados()
        {
            try
            {
                List<Evento> listEventos = new List<Evento>();

                var eventos = _eventoService.GetEventsForApproval();

                foreach (var e in eventos)
                {
                    listEventos.Add(e);
                }

                return JsonConvert.SerializeObject(new { code = 200, eventos = listEventos });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao consultar as Vagas. Por favor tente novamente." });
            }
        }

        #region Métodos private
        private IQueryable<Evento> VerificaSeEventoJaExiste(Evento evento)
        {
            return _eventoService.CheckIfEventAlreadyExists(evento.Cep, evento.Numero, evento.DataHoraInicio);
        }

        private bool ConvertStringtoImage(string commands, string nomeImagem)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(commands)))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    bm2.Save("C:/inetpub/wwwroot/Portal/imagens/" + nomeImagem + ".jpg");
                }
            }

            return true;
        }

        private string ValidaDadosDeEntrada(EventoViewModel evento)
        {
            if(string.IsNullOrEmpty(evento.Cep))
            {
                return "Favor informar o CEP.";
            }
            else
            {
                int n = -1;
                bool teste = int.TryParse(evento.Cep, out n);

                if(!teste)
                {
                    return "O CEP é um campo numérico.";
                }
            }

            if(evento.Cep.Length > 8)
            {
                return "O CEP não pode ter mais do que 8 caracteres.";
            }
            if(string.IsNullOrEmpty(evento.Endereco))
            {
                return "Favor informar o Endereço.";
            }
            if(evento.Endereco.Length > 255)
            {
                return "O Endereço não pode ter mais do que 255 caracteres.";
            }
            if(string.IsNullOrEmpty(evento.Numero))
            {
                return "Favor informar o número do local aonde ocorrerá o evento.";
            }
            else
            {
                int n = -1;
                bool teste = int.TryParse(evento.Numero, out n);

                if (!teste)
                {
                    return "O Número é um campo numérico. Se o local conter alguma letra no número, favor colocar essa informação no complemento.";
                }
            }

            if(evento.Complemento != null && evento.Complemento.Length > 255)
            {
                return "O Complemento não pode ter mais do que 255 caracteres.";
            }
            if(string.IsNullOrEmpty(evento.Bairro))
            {
                return "Favor informar o Bairro.";
            }
            if(string.IsNullOrEmpty(evento.Cidade))
            {
                return "Favor informar a Cidade.";
            }
            if(string.IsNullOrEmpty(evento.Estado))
            {
                return "Favor informar o Estado.";
            }
            if(string.IsNullOrEmpty(evento.DataHoraInicio))
            {
                return "Favor informar o dia e horário de início do Evento.";
            }
            if(string.IsNullOrEmpty(evento.DataHoraFim))
            {
                return "Favor informar o dia e horário de término do Evento.";
            }
            if(string.IsNullOrEmpty(evento.CategoriaEvento))
            {
                return "Favor informar a categoria do Evento.";
            }
            if(string.IsNullOrEmpty(evento.NomeEvento))
            {
                return "Favor informar um Nome para o Evento.";
            }

            return "";
        }
        #endregion
    }
}