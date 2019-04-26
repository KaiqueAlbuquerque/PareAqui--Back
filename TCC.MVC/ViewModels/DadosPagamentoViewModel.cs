using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class DadosPagamentoViewModel
    {
        public string DadosPagamentoId { get; set; }

        public bool Ativo { get; set; }

        public string Bandeira { get; set; }

        public string NumeroCartao { get; set; }

        public string Cvv { get; set; }

        public string NomeNoCartao { get; set; }

        public string Cpf { get; set; }

        public string Nascimento { get; set; }

        public string UsuarioId { get; set; }

        public virtual UsuarioViewModel Usuario { get; set; }
    }
}