using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TCC.MVC.ViewModels
{
    public class UsuarioViewModel
    {
        //[Key]//aqui estou dizendo que é chave primária
        public string UsuarioId { get; set; }

        //[Required(ErrorMessage = "Preencha o campo nome")]
        //[MaxLength(150, ErrorMessage = "Máximo de {0} caracteres")]
        //[MinLength(2, ErrorMessage = "Mínimo de {0} caracteres")]
        public string Nome { get; set; }

        //[Required(ErrorMessage = "Preencha o campo nome")]
        //[MaxLength(100, ErrorMessage = "Máximo de {0} caracteres")]
        //[EmailAddress(ErrorMessage = "Preencha um E-mail válido")]
        //[DisplayName("E-mail")]//Isso serve para na hora de gerar a minha View, ir com E-mail e não Email
        public string Email { get; set; }

        //[PasswordPropertyText(true)]
        public string Senha { get; set; }

        public string Sexo { get; set; }

        //[ScaffoldColumn(false)]//Serve para não criar este campo quando gerar a view
        public string DataCadastro { get; set; }

        public bool Ativo { get; set; }

        public bool AceitaReceberEmail { get; set; }

        public bool AceitaReceberSms { get; set; }

        public string Celular { get; set; }

        public string TokenPush { get; set; }

        //public virtual IEnumerable<GaragemViewModel> Garagens { get; set; }

        //public virtual IEnumerable<VeiculoViewModel> Veiculos { get; set; }

        //public virtual IEnumerable<LocacaoViewModel> Locacoes { get; set; }
    }
}