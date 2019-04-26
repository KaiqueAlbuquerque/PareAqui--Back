using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IGaragemService _garagemService;
        private readonly IVagaService _vagaService;
        private readonly IVeiculoService _veiculoService;
        private readonly ILocacaoService _locacaoService;
        private readonly IChatService _chatService;
        private readonly IAvaliacaoService _avaliacaoService;
        private readonly IDadosPagamentoService _dadosPagamentoService;
        private readonly IDadosBancarioService _dadosBancarioService;

        public UsuarioController(IUsuarioService usuarioService,
                                 IGaragemService garagemService,
                                 IVagaService vagaService,
                                 IVeiculoService veiculoService,
                                 ILocacaoService locacaoService,
                                 IChatService chatService,
                                 IAvaliacaoService avaliacaoService,
                                 IDadosPagamentoService dadosPagamentoService,
                                 IDadosBancarioService dadosBancarioService
                                )
        {
            _usuarioService = usuarioService;
            _garagemService = garagemService;
            _vagaService = vagaService;
            _veiculoService = veiculoService;
            _locacaoService = locacaoService;
            _chatService = chatService;
            _avaliacaoService = avaliacaoService;
            _dadosPagamentoService = dadosPagamentoService;
            _dadosBancarioService = dadosBancarioService;
        }
        
        [HttpPost]
        public string CadastrarUsuario(UsuarioViewModel usuario)
        {
            try
            {
                if(ValidaDadosDeEntrada(usuario) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(usuario) });
                }
                else
                {
                    if (usuario.AceitaReceberSms == true && string.IsNullOrEmpty(usuario.Celular))
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Para receber SMS, é necessário que seja informado o número de celular do usuário." });
                    }
                    if (!(VerificaSeJaNaoEstaCadastrado(usuario.Email)))
                    {
                        usuario.Ativo = true;

                        var usuarioService = Mapper.Map<UsuarioViewModel, Usuario>(usuario);

                        MD5 md5 = System.Security.Cryptography.MD5.Create();

                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(usuarioService.Senha);
                        byte[] hash = md5.ComputeHash(inputBytes);

                        StringBuilder sb = new StringBuilder();
                        for (int z = 0; z < hash.Length; z++)
                        {
                            sb.Append(hash[z].ToString("X2"));
                        }

                        var senhaCriptografada = sb.ToString();

                        usuarioService.Senha = senhaCriptografada;

                        _usuarioService.Add(usuarioService);

                        return JsonConvert.SerializeObject(new { code = 200, message = "Usuário cadastrado com sucesso.", id = usuarioService.UsuarioId });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "O cadastro não pode ser realizado pois o email já está em uso." });
                    }
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro no cadastro. Por favor, tente novamente." });
            }
        }
        
        public string ConsultarPorEmail(string email)
        {
            try
            {
                var usuario = _usuarioService.GetByEmail(email);
                var media = 0.0;

                if(usuario == null)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Usuário inexistente." });
                }
                else
                {
                    if (!usuario.Ativo)
                    {
                        usuario.Ativo = true;
                        _usuarioService.Update(usuario);
                    }
                }

                var avaliacoes = _avaliacaoService.GetEvaluationUser(usuario.UsuarioId, true, true);
                var total = 0.0;
                                
                foreach(var a in avaliacoes)
                {
                    total = total + a.Nota;
                }

                if(avaliacoes.Count() > 0)
                {
                    media = total / avaliacoes.Count();
                }

                UsuarioAvaliadoViewModel u = new UsuarioAvaliadoViewModel();

                var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(usuario);

                u.Usuario = usuarioViewModel;
                u.Nota = media;
                                
                return JsonConvert.SerializeObject(new { code = 200, usuario = u });
            }
            catch
            {                
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        public string ConsultarUmUsuario(int idUsuario)
        {
            try
            {
                var usuario = _usuarioService.GetById(idUsuario);
                var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(usuario);

                return JsonConvert.SerializeObject(new { code = 200, usuario = usuarioViewModel });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarUsuario(UsuarioViewModel usuario)
        {
            try
            {
                if (ValidaDadosDeEntrada(usuario) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(usuario) });
                }
                else
                {
                    usuario.Ativo = true;

                    if(string.IsNullOrEmpty(usuario.TokenPush))
                    {
                        usuario.TokenPush = _usuarioService.GetTokenPush(int.Parse(usuario.UsuarioId));
                    }
                    if (usuario.Senha == null || usuario.Senha == "")
                    {
                        usuario.Senha = _usuarioService.ConsultaUsuarioSemRastrear(int.Parse(usuario.UsuarioId));
                    }
                    else
                    {
                        MD5 md5 = System.Security.Cryptography.MD5.Create();

                        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(usuario.Senha);
                        byte[] hash = md5.ComputeHash(inputBytes);

                        StringBuilder sb = new StringBuilder();
                        for (int z = 0; z < hash.Length; z++)
                        {
                            sb.Append(hash[z].ToString("X2"));
                        }

                        var senhaCriptografada = sb.ToString();

                        usuario.Senha = senhaCriptografada;
                    }

                    var usuarioService = Mapper.Map<UsuarioViewModel, Usuario>(usuario);
                    _usuarioService.Update(usuarioService);

                    return JsonConvert.SerializeObject(new { code = 200, message = "Seus dados foram alterados com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string AlterarTokenPush(int idUsuario, string token)
        {
            try
            {
                var usuarioTokenIgual = _usuarioService.GetAll().Where(u => u.TokenPush == token).FirstOrDefault();

                if(usuarioTokenIgual != null)
                {
                    usuarioTokenIgual.TokenPush = null;

                    _usuarioService.Update(usuarioTokenIgual);
                }

                var usuario = _usuarioService.GetById(idUsuario);

                usuario.TokenPush = token;

                _usuarioService.Update(usuario);
                return JsonConvert.SerializeObject(new { code = 200, message = "Token alterado com sucesso." });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao alterar. Por favor, tente novamente." });
            }
        }

        [HttpPost]
        public string DesativarUsuario(int idUsuario)
        {
            try
            {
                var garagem = _garagemService.GetByIdUser(idUsuario);
                
                bool temGaragemCasa = false;

                foreach (Garagem gar in garagem)
                {
                    temGaragemCasa = true;
                    if (VerificaSeHaVagaOcupada(gar.GaragemId, idUsuario))
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Você não pode desativar sua conta pois existe(m) vaga(s) ocupada(s) em sua(s) garagem(s) no momento." });
                    }                    
                }               

                if(VerificaSeHaVeiculoQuePossuiGaragemOcupada(idUsuario))
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Você não pode desativar sua conta pois você possui veículo(s) alugando vaga(s) no momento." });
                }

                if(temGaragemCasa)
                {
                    DesativaGaragensCasas(idUsuario, garagem);
                }

                _veiculoService.DisableAllVehicle(idUsuario);

                var dadosPagamento = _dadosPagamentoService.GetAll().Where(d => d.UsuarioId == idUsuario);

                foreach(var d in dadosPagamento)
                {
                    d.Ativo = false;
                    _dadosPagamentoService.Update(d);
                }

                var dadosBancario = _dadosBancarioService.GetAll().Where(d => d.UsuarioId == idUsuario);

                foreach (var d in dadosBancario)
                {
                    d.Ativo = false;
                    _dadosBancarioService.Update(d);
                }

                _usuarioService.DisableUser(idUsuario);
                return JsonConvert.SerializeObject(new { code = 200, message = "Sua conta foi desativada. Para ativá-la, basta fazer login novamente!!" });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar. Por favor tente novamente." });
            }
        }
    
        public string ObterUsuariosChat(int idUsuario)
        {
            try
            {
                List<Usuario> users = new List<Usuario>();

                users = _chatService.GetUsersChat(idUsuario);

                return JsonConvert.SerializeObject(new { code = 200, usuarios = users });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar os usuários. Por favor tente novamente." });
            }
        }

        #region Métodos private
        private bool VerificaSeJaNaoEstaCadastrado(string email)
        {
            var usuario = _usuarioService.GetByEmail(email);
            if (usuario != null)
            {
                return true;
            }
            return false;
        }

        private bool VerificaSeHaVeiculoQuePossuiGaragemOcupada(int idUsuario)
        {
            IQueryable<Veiculo> veiculos = _veiculoService.GetByIdUsuario(idUsuario);

            foreach(Veiculo v in veiculos)
            {
                if (VerificaSePossuiGaragemOcupada(v))
                {
                    return true;
                }
            }
            return false;
        }

        private bool VerificaSeHaVagaOcupada(int idGaragem, int idUsuario)
        {
            var vaga = _vagaService.GetByIdGarageAndIdUser(idGaragem, idUsuario);

            foreach (Vaga v in vaga)
            {
                if (VerificaSeEstaOcupada(v))
                {
                    return true;
                }
            }
            return false;
        }

        private void DesativaGaragensCasas(int idUsuario, List<Garagem> garagens)
        {
            foreach (Garagem g in garagens)
            {
                DesativaVagas(g.GaragemId);
            }
            _garagemService.DisableAllGarage(idUsuario);
        }
                
        private void DesativaVagas(int idGaragem)
        {
            _vagaService.DisableAllVacancy(idGaragem);
        }

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

        private string ValidaDadosDeEntrada(UsuarioViewModel usuario)
        {
            if(string.IsNullOrEmpty(usuario.Nome))
            {
                return "Favor informar o seu Nome.";
            }
            if(string.IsNullOrEmpty(usuario.Email))
            {
                return "Favor informar o seu E-mail.";
            }
            else
            {
                Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

                if (!rg.IsMatch(usuario.Email))
                {
                    return "E-mail inválido.";
                }
            }
            if(string.IsNullOrEmpty(usuario.Sexo))
            {
                return "Favor informar o seu Sexo.";
            }
            if(!string.IsNullOrEmpty(usuario.Celular))
            {
                if(usuario.Celular.Length<11)
                {
                    return "O Celular precisa ter 11 dígitos, sendo os 2 primeiros se referem ao código de área.";
                }
                Regex rg = new Regex(@"^[1-9]{2}(9[1-9])[0-9]{3}[0-9]{4}$");

                if (!rg.IsMatch(usuario.Celular))
                {
                    return "Celular inválido.";
                }
            }

            return "";
        }
        #endregion
    }
}