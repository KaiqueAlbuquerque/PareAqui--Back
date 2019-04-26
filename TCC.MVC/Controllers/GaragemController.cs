using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Services;
using TCC.MVC.ViewModels;

namespace TCC.MVC.Controllers
{
    public class GaragemController : Controller
    {
        private readonly IGaragemService _garagemService;
        private readonly IVagaService _vagaService;
        private readonly ILocacaoService _locacaoService;

        public GaragemController(IGaragemService garagemService, 
                                 IVagaService vagaService, 
                                 ILocacaoService locacaoService
                                )
        {
            _garagemService = garagemService;
            _vagaService = vagaService;
            _locacaoService = locacaoService;
        }

        [HttpPost]
        public string CadastrarGaragem(GaragemViewModel garagem)
        {
            try
            {
                if(ValidaDadosDeEntrada(garagem) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(garagem) });
                }
                else
                {
                    garagem.Latitude = garagem.Latitude.Replace(".", ",");
                    garagem.Longitude = garagem.Longitude.Replace(".", ",");
                    garagem.Ativo = null;

                    var garagemService = Mapper.Map<GaragemViewModel, Garagem>(garagem);
                    if (!(ConsultarSeGaragemJaExiste(garagemService.Cep, garagemService.NumeroRua)))
                    {
                        _garagemService.Add(garagemService);
                        return JsonConvert.SerializeObject(new { code = 200, message = "Garagem cadastrada com sucesso. Aguarde a nossa aprovação.", id = garagemService.GaragemId });
                    }
                    else
                    {
                        if (VerificaSeEhApartamento(garagemService.Cep, garagemService.NumeroRua))
                        {
                            var idGar = _garagemService.IdGarageIfAlreadyExists(garagemService.Cep, garagemService.NumeroRua);
                            return JsonConvert.SerializeObject(new { code = 400, message = "Esta garagem já está cadastrada, porém identificamos que ela pertence a um Condomínio, então você não precisa cadastra-la novamente. Basta apenas adicionar sua vaga e aguardar a nossa aprovação.", id = idGar });
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new { code = 400, message = "Esta garagem já está cadastrada e você não pode cadastrar a mesma garagem. Talvez ela não esteja aparecendo pois ainda não foi aprovada. Aguarde que em breve teremos novidades para você." });
                        }
                    }
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro no cadastro. Por favor, tente novamente." });
            }
        }

        public string ConsultarGaragens(int idUsuario)
        {
            try
            {
                var garagem = _garagemService.GetByIdUser(idUsuario);
                var Itens = new List<object>();

                foreach (Garagem gar in garagem)
                {
                    var vags = _vagaService.GetAll().Where(v => v.GaragemId == gar.GaragemId && v.UsuarioId == idUsuario && v.Desativa != true).ToList();
                                        
                    if(gar.Ativo != false && vags.Count > 0)
                    {
                        var garagemViewModel = Mapper.Map<Garagem, GaragemViewModel>(gar);
                        garagemViewModel.Latitude = garagemViewModel.Latitude.Replace(",", ".");
                        garagemViewModel.Longitude = garagemViewModel.Longitude.Replace(",", ".");
                        Itens.Add(garagemViewModel);
                    }
                }

                return JsonConvert.SerializeObject(new { code = 200, garagens = Itens });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }       

        public string ConsultarUmaGaragem(int idGaragem)
        {
            try
            {                
                var garagem = _garagemService.GetById(idGaragem);
                var garagemViewModel = Mapper.Map<Garagem, GaragemViewModel>(garagem);

                return JsonConvert.SerializeObject(new { code = 200, garagem = garagemViewModel });                
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao consultar. Por favor, tente novamente." });
            }
        }
               
        [HttpPost]
        public string AlterarGaragem(GaragemViewModel garagem)
        {
            try
            {
                if (ValidaDadosDeEntrada(garagem) != "")
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = ValidaDadosDeEntrada(garagem) });
                }
                else
                {
                    garagem.Latitude = garagem.Latitude.Replace(".", ",");
                    garagem.Longitude = garagem.Longitude.Replace(".", ",");
                
                    var garagemService = Mapper.Map<GaragemViewModel, Garagem>(garagem);

                    if (garagemService.Condominio)
                    {
                        return JsonConvert.SerializeObject(new { code = 400, message = "Esta garagem pertence a um condomínio e portanto não pode ser alterada." });
                    }
                    else
                    {
                        if (!(ConsultarSeGaragemJaExiste(garagemService.Cep, garagemService.NumeroRua)))
                        {
                            _garagemService.Update(garagemService);
                            return JsonConvert.SerializeObject(new { code = 200, message = "Garagem alterada com sucesso." });
                        }
                        else if (_garagemService.IdGarageIfAlreadyExists(garagemService.Cep, garagemService.NumeroRua) == garagemService.GaragemId)
                        {
                            _garagemService.Update(garagemService);
                            return JsonConvert.SerializeObject(new { code = 200, message = "Garagem alterada com sucesso." });
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new { code = 400, message = "Esta garagem já existe. Você não pode cadastrar a mesma garagem." });
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
        public string DesativarGaragem(int idGaragem)
        {
            try
            {
                var garagem = _garagemService.GetById(idGaragem);

                if(garagem.Condominio)
                {
                    return JsonConvert.SerializeObject(new { code = 400, message = "Esta garagem pertence a um condomínio, e portanto não pode ser desativada. Se você quiser, você pode desativar a sua vaga." });
                }
                else
                {
                    var vaga = _vagaService.GetByIdGarage(idGaragem);

                    foreach (Vaga v in vaga)
                    {
                        if (VerificaSeEstaOcupada(v))
                        {
                            return JsonConvert.SerializeObject(new { code = 400, message = "Não foi possível desativar a garagem, ela possui vaga(s) ocupada(s)." });
                        }
                    }
                    _vagaService.DisableAllVacancy(idGaragem);

                    _garagemService.DisableGarage(idGaragem);

                    return JsonConvert.SerializeObject(new { code = 200, message = "Garagem desativada com sucesso." });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { code = 400, message = "Erro ao desativar. Por favor tente novamente." });
            }
        }

        #region Métodos private
        private bool ConsultarSeGaragemJaExiste(string cep, int numeroRua)
        {
            if (_garagemService.CheckIfGarageAlreadyExists(cep, numeroRua))
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        private bool VerificaSeEhApartamento(string cep, int numeroRua)
        {
            var idGar = _garagemService.IdGarageIfAlreadyExists(cep, numeroRua);

            var gar = _garagemService.GetById(idGar);

            if(gar.Condominio)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        private string ValidaDadosDeEntrada(GaragemViewModel garagem)
        {
            if (string.IsNullOrEmpty(garagem.Cep))
            {
                return "Favor informar o CEP.";
            }
            else
            {
                int n = -1;
                bool teste = int.TryParse(garagem.Cep, out n);

                if (!teste)
                {
                    return "O CEP é um campo numérico.";
                }
            }

            if (garagem.Cep.Length > 8)
            {
                return "O CEP não pode ter mais do que 8 caracteres.";
            }
            if (string.IsNullOrEmpty(garagem.Endereco))
            {
                return "Favor informar o Endereço.";
            }
            if (garagem.Endereco.Length > 255)
            {
                return "O Endereço não pode ter mais do que 255 caracteres.";
            }
            if (string.IsNullOrEmpty(garagem.NumeroRua))
            {
                return "Favor informar o número do local da Garagem.";
            }
            else
            {
                int n = -1;
                bool teste = int.TryParse(garagem.NumeroRua, out n);

                if (!teste)
                {
                    return "O Número é um campo numérico. Se o local conter alguma letra no número, favor colocar essa informação no complemento.";
                }
            }

            if (garagem.Complemento != null && garagem.Complemento.Length > 255)
            {
                return "O Complemento não pode ter mais do que 255 caracteres.";
            }
            if (string.IsNullOrEmpty(garagem.Bairro))
            {
                return "Favor informar o Bairro.";
            }
            if (string.IsNullOrEmpty(garagem.Cidade))
            {
                return "Favor informar a Cidade.";
            }
            if (string.IsNullOrEmpty(garagem.Estado))
            {
                return "Favor informar o Estado.";
            }            

            return "";
        }
        #endregion
    }
}