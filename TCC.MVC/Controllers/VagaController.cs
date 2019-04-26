using AutoMapper;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
    public class VagaController : Controller
    {
        private readonly IVagaService _vagaService;
        private readonly ILocacaoService _locacaoService;
        private readonly IGaragemService _garagemService;
        private readonly IFotoService _fotoService;
        private readonly IUsuarioService _usuarioService;
        private readonly INotificacaoEmailSmsService _notificacaoEmailSms;
        private readonly IDadosBancarioService _dadosBancarioService;
        
        public VagaController(IVagaService vagaService, 
                              ILocacaoService locacaoService, 
                              IGaragemService garagemService, 
                              IFotoService fotoService,
                              IUsuarioService usuarioService,
                              INotificacaoEmailSmsService notificacaoEmailSms,
                              IDadosBancarioService dadosBancarioService
                             )
        {
            _vagaService = vagaService;
            _locacaoService = locacaoService;
            _garagemService = garagemService;
            _fotoService = fotoService;
            _usuarioService = usuarioService;
            _notificacaoEmailSms = notificacaoEmailSms;
            _dadosBancarioService = dadosBancarioService;
        }

        [HttpPost]
        public string CadastrarVaga(VagaViewModel vaga, int quantidade, List<Foto> fotos)
        {
            try
            {
                if(ValidaDadosDeEntrada(vaga) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(vaga) });
                }
                else
                {
                    vaga.Ativo = false;
                    vaga.Aceita = null;

                    if(!string.IsNullOrEmpty(vaga.PrecoMensal))
                    {
                        vaga.PrecoMensal = vaga.PrecoMensal.Replace(".", ",");
                    }
                    if(!string.IsNullOrEmpty(vaga.PrecoDiaria))
                    {
                        vaga.PrecoDiaria = vaga.PrecoDiaria.Replace(".", ",");
                    }
                    if (!string.IsNullOrEmpty(vaga.PrecoAvulso))
                    {
                        vaga.PrecoAvulso = vaga.PrecoAvulso.Replace(".", ",");
                    }
                    if (!string.IsNullOrEmpty(vaga.AposPrimeiraHora))
                    {
                        vaga.AposPrimeiraHora = vaga.AposPrimeiraHora.Replace(".", ",");
                    }

                    var vagaService = Mapper.Map<VagaViewModel, Vaga>(vaga);

                    var dadosBancario = _dadosBancarioService.GetByIdUser(vagaService.UsuarioId);

                    if(dadosBancario.Count == 0)
                    {
                        var ga = _garagemService.GetById(vagaService.GaragemId);
                        ga.Vagas = _vagaService.GetByIdGarage(ga.GaragemId);

                        if (ga.Vagas.Count() == 0)
                        {
                            _garagemService.Remove(ga);
                        }
                        return JsonConvert.SerializeObject(new { code = 400, message = "Não foi possível realizar o cadastro da vaga, é necessário que cadastre seus dados bancários." });
                    }

                    if(
                        vagaService.PrecoAvulso == 0 && 
                        vagaService.PrecoMensal == 0 && 
                        vagaService.PrecoDiaria == 0 &&
                        vagaService.Mensal == false &&
                        vagaService.Avulso == false &&
                        vagaService.Diaria == false
                       )
                    {
                        var ga = _garagemService.GetById(vagaService.GaragemId);
                        ga.Vagas = _vagaService.GetByIdGarage(ga.GaragemId);

                        if(ga.Vagas.Count() == 0)
                        {
                            _garagemService.Remove(ga);
                        }
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar ao menos 1 categoria de aluguel e o valor da locação desta categoria." });
                    }

                    if(vagaService.Mensal == true && vagaService.PrecoMensal == 0)
                    {
                        var ga = _garagemService.GetById(vagaService.GaragemId);
                        ga.Vagas = _vagaService.GetByIdGarage(ga.GaragemId);

                        if (ga.Vagas.Count() == 0)
                        {
                            _garagemService.Remove(ga);
                        }
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor da locação mensal." });
                    }

                    if (vagaService.Avulso == true && vagaService.PrecoAvulso == 0)
                    {
                        var ga = _garagemService.GetById(vagaService.GaragemId);
                        ga.Vagas = _vagaService.GetByIdGarage(ga.GaragemId);

                        if (ga.Vagas.Count() == 0)
                        {
                            _garagemService.Remove(ga);
                        }
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor da locação avulsa." });
                    }

                    if (vagaService.Avulso == true && vagaService.AposPrimeiraHora == 0)
                    {
                        var ga = _garagemService.GetById(vagaService.GaragemId);
                        ga.Vagas = _vagaService.GetByIdGarage(ga.GaragemId);

                        if (ga.Vagas.Count() == 0)
                        {
                            _garagemService.Remove(ga);
                        }
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor que deve ser cobrado após a primeira hora de locação." });
                    }

                    if (vagaService.Diaria == true && vagaService.PrecoDiaria == 0)
                    {
                        var ga = _garagemService.GetById(vagaService.GaragemId);
                        ga.Vagas = _vagaService.GetByIdGarage(ga.GaragemId);

                        if (ga.Vagas.Count() == 0)
                        {
                            _garagemService.Remove(ga);
                        }
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor da locação diaria." });
                    }

                    if (vaga.PrecoAvulso == null)
                    {
                        vagaService.PrecoAvulso = null;
                    }
                    if (vaga.PrecoDiaria == null)
                    {
                        vagaService.PrecoDiaria = null;
                    }
                    if (vaga.PrecoMensal == null)
                    {
                        vagaService.PrecoMensal = null;
                    }
                    if (vaga.AposPrimeiraHora == null)
                    {
                        vagaService.AposPrimeiraHora = null;
                    }

                    var usu = _usuarioService.GetById(vagaService.UsuarioId);
                    var gar = _garagemService.GetById(vagaService.GaragemId);

                    if (gar.Condominio && string.IsNullOrEmpty(vaga.NumeroVaga))
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Como esta Vaga pertence a um condomínio, você precisa fornecer o número da Vaga." });
                    }
                    if (gar.Condominio && !string.IsNullOrEmpty(vaga.NumeroVaga))
                    {
                        var vagas = _vagaService.GetByIdGarage(vagaService.GaragemId);

                        foreach (var v in vagas)
                        {
                            if (v.NumeroVaga == vagaService.NumeroVaga)
                            {
                                return JsonConvert.SerializeObject(new { code = 400, message = "Desculpe, mas o número de vaga informado já está cadastrado em outra vaga." });
                            }
                        }
                    }
                    if (vaga.Avulso && vaga.PrecoAvulso == null)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Como você marcou a opção de Locação Avulso, precisa informar o valor para locação avulsa." });
                    }
                    if (vaga.Avulso && vaga.AposPrimeiraHora == null)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Como você marcou a opção de Locação Avulso, precisa informar o valor a ser cobrado após a primeira hora." });
                    }
                    if (vaga.Mensal && vaga.PrecoMensal == null)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Como você marcou a opção de Locação Mensal, precisa informar o valor para locação mensal." });
                    }
                    if (vaga.Diaria && vaga.PrecoDiaria == null)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Como você marcou a opção de Locação Diaria, precisa informar o valor para locação diaria." });
                    }

                    List<int> ids = new List<int>();

                    for (int i = 0; i < quantidade; i++)
                    {
                        int contador = 0;
                        _vagaService.Add(vagaService);
                        ids.Add(vagaService.VagaId);

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
                                        var nomeImagem = vagaService.VagaId.ToString() + usu.Email + contador.ToString() + ".jpg";

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
                                            fot.VagaId = vagaService.VagaId;

                                            _fotoService.Add(fot);
                                            contador++;
                                        }
                                        else
                                        {
                                            foreach (var v in ids)
                                            {
                                                if (_vagaService.GetById(v) != null)
                                                {
                                                    var fo = _fotoService.GetAll().Where(ft => ft.VagaId == v);

                                                    foreach (var ft in fo)
                                                    {
                                                        _fotoService.Remove(ft);
                                                    }

                                                    var vg = _vagaService.GetById(v);
                                                    _vagaService.Remove(vg);
                                                }
                                            }

                                            var g = _garagemService.GetById(vagaService.GaragemId);
                                            g.Vagas = _vagaService.GetByIdGarage(g.GaragemId);

                                            if (g.Vagas.Count() == 0)
                                            {
                                                _garagemService.Remove(g);
                                            }

                                            return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao cadastrar as fotos da Vaga e por isso ela não foi cadastrada. Por favor tente novamente." });
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            foreach(var v in ids)
                            {
                                if(_vagaService.GetById(v) != null)
                                {
                                    var fot = _fotoService.GetAll().Where(f => f.VagaId == v);

                                    foreach(var f in fot)
                                    {
                                        _fotoService.Remove(f);
                                    }

                                    var vg = _vagaService.GetById(v);
                                    _vagaService.Remove(vg);
                                }
                            }

                            var g = _garagemService.GetById(vagaService.GaragemId);
                            g.Vagas = _vagaService.GetByIdGarage(g.GaragemId);

                            if(g.Vagas.Count() == 0)
                            {
                                _garagemService.Remove(g);
                            }
                            return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao cadastrar as fotos da Vaga e por isso a mesma não foi cadastrada. Por favor tente novamente." });
                        }
                    }

                    return JsonConvert.SerializeObject(new { code = 200, message = "Vaga(s) cadastrada(s) com sucesso. Aguarde a nossa aprovação.", id = ids });
                }
                
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro no cadastro. Por favor, tente novamente." });
            }
        }

        public string ConsultarVagas(int idGaragem, int idUsuario)
        {
            try
            {
                var Itens = new List<object>();
                var vaga = _vagaService.GetAllByIdGarageAndIdUser(idGaragem, idUsuario);

                foreach(Vaga v in vaga)
                {
                    var vagaViewModel = Mapper.Map<Vaga, VagaViewModel>(v);                    
                    Itens.Add(vagaViewModel);
                }
                return JsonConvert.SerializeObject(new { code = 200, vagas = Itens });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        public string ConsultarUmaVaga(int idVaga)
        {
            try
            {
                List<string> img = new List<string>();
                int qtdeVagas = 0;

                var fotos = _fotoService.GetPhotosByIdVacancy(idVaga);

                foreach(var f in fotos)
                {
                    img.Add(f.Imagem);
                }
                
                var vaga = _vagaService.GetById(idVaga);
                var garagem = _garagemService.GetById(vaga.GaragemId);
                garagem.Vagas = _vagaService.GetByIdGarage(garagem.GaragemId);

                foreach(var gv in garagem.Vagas)
                {
                    if(gv.Ativo)
                    {
                        qtdeVagas++;
                    }
                }

                var vagaViewModel = Mapper.Map<Vaga, VagaViewModel>(vaga);

                VagaAprovacaoViewModel v = new VagaAprovacaoViewModel();

                v.Vaga = vagaViewModel;
                v.QtdeVagas = qtdeVagas.ToString();
                v.Imagens = img;

                return JsonConvert.SerializeObject(new { code = 200, vaga = v });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarVaga(VagaViewModel vaga)
        {
            try
            {
                if (ValidaDadosDeEntrada(vaga) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(vaga) });
                }
                else
                {
                    if (!string.IsNullOrEmpty(vaga.PrecoMensal))
                    {
                        vaga.PrecoMensal = vaga.PrecoMensal.Replace(".", ",");
                    }
                    if (!string.IsNullOrEmpty(vaga.PrecoDiaria))
                    {
                        vaga.PrecoDiaria = vaga.PrecoDiaria.Replace(".", ",");
                    }
                    if (!string.IsNullOrEmpty(vaga.PrecoAvulso))
                    {
                        vaga.PrecoAvulso = vaga.PrecoAvulso.Replace(".", ",");
                    }
                    if (!string.IsNullOrEmpty(vaga.AposPrimeiraHora))
                    {
                        vaga.AposPrimeiraHora = vaga.AposPrimeiraHora.Replace(".", ",");
                    }

                    var vagaService = Mapper.Map<VagaViewModel, Vaga>(vaga);

                    if (
                        vagaService.PrecoAvulso == 0 &&
                        vagaService.PrecoMensal == 0 &&
                        vagaService.PrecoDiaria == 0 &&
                        vagaService.Mensal == false &&
                        vagaService.Avulso == false &&
                        vagaService.Diaria == false
                       )
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar ao menos 1 categoria de aluguel e o valor da locação desta categoria." });
                    }

                    if (vagaService.Mensal == true && vagaService.PrecoMensal == 0)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor da locação mensal." });
                    }

                    if (vagaService.Avulso == true && vagaService.PrecoAvulso == 0)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor da locação avulsa." });
                    }

                    if (vagaService.Avulso == true && vagaService.AposPrimeiraHora == 0)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor que deve ser cobrado após a primeira hora de locação." });
                    }

                    if (vagaService.Diaria == true && vagaService.PrecoDiaria == 0)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Favor informar o valor da locação diaria." });
                    }

                    if (vaga.PrecoAvulso == null)
                    {
                        vagaService.PrecoAvulso = null;
                    }
                    if (vaga.PrecoDiaria == null)
                    {
                        vagaService.PrecoDiaria = null;
                    }
                    if (vaga.PrecoMensal == null)
                    {
                        vagaService.PrecoMensal = null;
                    }
                    if (vaga.AposPrimeiraHora == null)
                    {
                        vagaService.AposPrimeiraHora = null;
                    }

                    var locacoes = _locacaoService.GetAll().Where(l => l.VagaId == vagaService.VagaId && l.Ativo == true);
                    if (locacoes.Count() > 0)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Uma vaga não pode ter seus dados alterados enquanto uma locação estiver em andamento. Ao finalizar a locação vigente, você poderá alterar os dados da vaga." });
                    }
                    else
                    {
                        vaga.Ativo = false;
                        vaga.Aceita = null;

                        var gar = _garagemService.GetById(vagaService.GaragemId);

                        if (gar.Condominio && string.IsNullOrEmpty(vagaService.NumeroVaga))
                        {
                            return JsonConvert.SerializeObject(new { code = 400, message = "Como esta Vaga pertence a um condomínio, você precisa fornecer o número da Vaga." });
                        }
                        else
                        {
                            _vagaService.Update(vagaService);
                            return JsonConvert.SerializeObject(new { code = 200, message = "Vaga alterada com sucesso. Aguarde a aprovação." });
                        }
                    }
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string DesativarVaga(int idVaga)
        {
            try
            {
                var vaga = _vagaService.GetById(idVaga);

                if(vaga.Aceita == null)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Não foi possível desativar a vaga, somente depois da aprovação você poderá desativá-la." });
                }
                else if(vaga.Aceita == false)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Você não pode desativar esta vaga pois ela não foi aprovada. Portanto, ela não é uma vaga Ativa." });
                }

                if(!(VerificaSeEstaOcupada(vaga)))
                {
                    _vagaService.DisableVacancy(idVaga);

                    var gar = _garagemService.GetById(vaga.GaragemId);

                    gar.Vagas = _vagaService.GetByIdGarage(gar.GaragemId);

                    bool desativaGaragem = true;

                    foreach(var v in gar.Vagas)
                    {
                        if(v.Ativo)
                        {
                            desativaGaragem = false;
                        }
                    }

                    if(desativaGaragem)
                    {
                        _garagemService.DisableGarage(gar.GaragemId);

                        return JsonConvert.SerializeObject(new { code = 200, message = "Vaga desativada com sucesso. Como esta vaga era a última desta garagem, a garagem também foi desativada." });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 200, message = "Vaga desativada com sucesso." });
                    }
                }

                return JsonConvert.SerializeObject(new { code = 400, message = "Não foi possível desativar sua vaga, ela possui locação aprovada." });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar. Por favor tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> AprovarVaga(int idVaga)
        {
            try
            {
                var vaga = _vagaService.GetById(idVaga);

                vaga.Aceita = true;
                vaga.Ativo = true;

                _vagaService.Update(vaga);

                vaga.Garagem = _garagemService.GetById(vaga.GaragemId);
                vaga.Garagem.Ativo = true;
                _garagemService.Update(vaga.Garagem);

                var usuario = vaga.Usuario;

                var notifiEmail = _notificacaoEmailSms.GetAll().Where(n => n.UsuarioId == usuario.UsuarioId && n.TipoNotificacao == 1 && n.MotivoNotificacao == 1 && n.DataCadastro >= DateTime.Now.AddHours(-2) && n.DataCadastro <= DateTime.Now);
                
                if(notifiEmail.Count() == 0)
                {
                    string assunto = "Vaga(s) aprovada(s)";
                    var mensagem = "Olá " + usuario.Nome + ". <br /><br /> A(s) sua(s) vaga(s), localizada(s) na " + vaga.Garagem.Endereco +
                                " Nº " + vaga.Garagem.NumeroRua + " foi aprovada. <br /><br /> A partir de agora ela(s) aparecerá nas buscas para ser alugada(s).<br /><br />" +
                                "Dados da Vaga: <br />" +
                                "Endereço: " + vaga.Garagem.Endereco +
                                ", Nº " + vaga.Garagem.NumeroRua + "<br />" +
                                "Bairro: " + vaga.Garagem.Bairro + "<br />" +
                                "CEP: " + vaga.Garagem.Cep + "<br /><br />" +
                                "Acesse o App PareAqui para mais informações. <br /><br />" +
                                "E-mail automático. Favor não responder.";

                    /*Notification n = new Notification();

                    if (usuario.AceitaReceberEmail)
                    {
                        await n.SendMailAsync(usuario.Email, assunto, mensagem);
                    }
                    */

                    NotificacaoEmailSms nes = new NotificacaoEmailSms();

                    nes.MotivoNotificacao = 1;
                    nes.TipoNotificacao = 1;
                    nes.UsuarioId = usuario.UsuarioId;

                    _notificacaoEmailSms.Add(nes);
                }

                var notifiSms = _notificacaoEmailSms.GetAll().Where(n => n.UsuarioId == usuario.UsuarioId && n.TipoNotificacao == 2 && n.MotivoNotificacao == 1 && n.DataCadastro >= DateTime.Now.AddHours(-2) && n.DataCadastro <= DateTime.Now);

                if (notifiSms.Count() == 0)
                {
                    /*Notification n = new Notification();

                    if(usuario.AceitaReceberSms)
                    {
                        string sms = "Olá " + usuario.Nome + ". A sua vaga foi aprovada. A partir de agora ela aparecerá nas buscas para ser alugada. Para mais informações acesse o App PareAqui.";

                        n.SendSMS(usuario.Celular, sms);
                    }*/

                    NotificacaoEmailSms nes = new NotificacaoEmailSms();

                    nes.MotivoNotificacao = 1;
                    nes.TipoNotificacao = 2;
                    nes.UsuarioId = usuario.UsuarioId;

                    _notificacaoEmailSms.Add(nes);
                }

                var token = _usuarioService.GetById(vaga.UsuarioId).TokenPush;

                if(token != null)
                {
                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                    request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"Sua vaga foi aprovada. Agora você já pode ganhar uma renda extra. Entre no app PareAqui e saiba mais.\",\"title\": \"Sua vaga foi aprovada\" }}", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }               

                return JsonConvert.SerializeObject(new { code = 200, message = "Vaga aprovada com sucesso." });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao aprovar a Vaga. Por favor tente novamente." });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<string> ReprovarVaga(int idVaga, string motivoReprovada)
        {
            try
            {
                var vaga = _vagaService.GetById(idVaga);

                vaga.Aceita = false;
                vaga.Ativo = false;
                vaga.MotivoReprovada = motivoReprovada;

                _vagaService.Update(vaga);

                var usuario = vaga.Usuario;

                var notifiEmail = _notificacaoEmailSms.GetAll().Where(n => n.UsuarioId == usuario.UsuarioId && n.TipoNotificacao == 1 && n.MotivoNotificacao == 2 && n.DataCadastro >= DateTime.Now.AddHours(-2) && n.DataCadastro <= DateTime.Now);

                if (notifiEmail.Count() == 0)
                {
                    string assunto = "Vaga reprovada";
                    var mensagem = "Olá " + usuario.Nome + ". <br /><br /> Infelizmente não podemos aprovar a sua vaga, localizada na " + vaga.Garagem.Endereco +
                                " Nº " + vaga.Garagem.NumeroRua + ". <br /><br /> Segue abaixo o motivo pelo qual não podemos aprovar a mesma.<br /><br />" +
                                "Dados da Vaga: <br />" +
                                "Endereço: " + vaga.Garagem.Endereco +
                                ", Nº " + vaga.Garagem.NumeroRua + "<br />" +
                                "Bairro: " + vaga.Garagem.Bairro + "<br />" +
                                "CEP: " + vaga.Garagem.Cep + "<br /><br />" +
                                "Motivo da Reprovação: " + motivoReprovada + "<br /><br />" +
                                "E-mail automático. Favor não responder.";

                    /*Notification n = new Notification();

                    if (usuario.AceitaReceberEmail)
                    {
                        await n.SendMailAsync(usuario.Email, assunto, mensagem);
                    }*/

                    NotificacaoEmailSms nes = new NotificacaoEmailSms();

                    nes.MotivoNotificacao = 2;
                    nes.TipoNotificacao = 1;
                    nes.UsuarioId = usuario.UsuarioId;

                    _notificacaoEmailSms.Add(nes);
                }

                var notifiSms = _notificacaoEmailSms.GetAll().Where(n => n.UsuarioId == usuario.UsuarioId && n.TipoNotificacao == 2 && n.MotivoNotificacao == 2 && n.DataCadastro >= DateTime.Now.AddHours(-2) && n.DataCadastro <= DateTime.Now);

                if (notifiSms.Count() == 0)
                {
                    /*Notification n = new Notification();

                    if(usuario.AceitaReceberSms)
                    {
                        string sms = "Olá " + usuario.Nome + ". Infelizmente não podemos aprovar a sua vaga. Para mais informações acesse o App PareAqui.";

                        n.SendSMS(usuario.Celular, sms);
                    }*/

                    NotificacaoEmailSms nes = new NotificacaoEmailSms();

                    nes.MotivoNotificacao = 2;
                    nes.TipoNotificacao = 2;
                    nes.UsuarioId = usuario.UsuarioId;

                    _notificacaoEmailSms.Add(nes);
                }

                var token = _usuarioService.GetById(vaga.UsuarioId).TokenPush;

                if(token != null)
                {
                    var client = new RestClient("https://fcm.googleapis.com/fcm/send");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("authorization", "key=AAAApWoJXi4:APA91bHQiIkzL3CnydktDyZaMcs_pDR1Bu6ECbM-apPfRIXKgHZg_WWyYDHAFLsXeb6E1TZmjdqtjrrfZWa9BDd6JQKcF89NtAVZOIj2Kmu6yYSYg3n9mFMbc1np6mmiAXULJkKuwnqj");
                    request.AddParameter("application/json", "{\"to\" : \"" + token + "\",\"collapse_key\" : \"type_a\",\"notification\" : {\"body\" : \"Infelizmente não podemos aprovar a sua vaga. Em instantes você receberá um e-mail com mais detalhes.\",\"title\": \"Vaga reprovada\" }}", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
                
                return JsonConvert.SerializeObject(new { code = 200, message = "Vaga reprovada com sucesso." });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao reprovar a Vaga. Por favor tente novamente." });
            }
        }

        public string VagasNaoAprovadas()
        {
            try
            {
                List<Vaga> listVagas = new List<Vaga>();

                var vagas = _vagaService.GetVacanciesForApproval();

                foreach(var v in vagas)
                {
                    listVagas.Add(v);
                }

                return JsonConvert.SerializeObject(new { code = 200, vag = listVagas });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Houve um erro ao consultar as Vagas. Por favor tente novamente." });
            }
        }

        #region Métodos privados
        private bool VerificaSeEstaOcupada(Vaga vaga)
        {
            bool ocupada = false;
            var loc = _locacaoService.GetByIdVaga(vaga.VagaId);

            foreach (var l in loc)
            {
                if (l.Ativo)
                {
                    ocupada = true;
                }
            }

            return ocupada;
        }

        private bool ConvertStringtoImage(string commands, string nomeImagem)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(commands)))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    bm2.Save("C:/inetpub/wwwroot/Portal/imagens/" + nomeImagem+".jpg");
                    //bm2.Save("C:/Users/kaique.oliveira/Desktop/IMAGENS" + nomeImagem + ".jpg");
                }
            }

            return true;
        }

        private string ValidaDadosDeEntrada(VagaViewModel vaga)
        {
            if(!string.IsNullOrEmpty(vaga.PrecoAvulso))
            {
                decimal n = 0;
                bool teste = decimal.TryParse(vaga.PrecoAvulso, out n);

                if(!teste)
                {
                    return "Valor do Preço para locação Avulsa inválido.";
                }
            }
            if (!string.IsNullOrEmpty(vaga.AposPrimeiraHora))
            {
                decimal n = 0;
                bool teste = decimal.TryParse(vaga.AposPrimeiraHora, out n);

                if (!teste)
                {
                    return "Valor do Preço para após primeira hora inválido.";
                }
            }
            if (!string.IsNullOrEmpty(vaga.PrecoDiaria))
            {
                decimal n = 0;
                bool teste = decimal.TryParse(vaga.PrecoDiaria, out n);

                if (!teste)
                {
                    return "Valor do Preço para locação Diária inválido.";
                }
            }
            if (!string.IsNullOrEmpty(vaga.PrecoMensal))
            {
                decimal n = 0;
                bool teste = decimal.TryParse(vaga.PrecoMensal, out n);

                if (!teste)
                {
                    return "Valor do Preço para locação Mensal inválido.";
                }
            }
            
            return "";
        }
        #endregion
    }
}