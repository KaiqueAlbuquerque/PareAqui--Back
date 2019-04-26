using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class DadosBancarioViewModel
    {
        public string DadosBancarioId { get; set; }

        public bool Ativo { get; set; }

        public string Cpf { get; set; }

        public string NomeBanco { get; set; }

        public string Agencia { get; set; }

        public string NumeroConta { get; set; }

        public string NomeDonoConta { get; set; }

        public string UsuarioId { get; set; }

        public virtual UsuarioViewModel Usuario { get; set; }
    }
}