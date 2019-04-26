using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class AvaliacaoViewModel
    {
        public string AvaliacaoId { get; set; }

        public string Nota { get; set; }

        public string Comentario { get; set; }

        public string UsuarioAvaliadorId { get; set; }

        public string UsuarioAvaliadoId { get; set; }

        public string LocacaoId { get; set; }

        public virtual UsuarioViewModel UsuarioAvaliador { get; set; }

        public virtual UsuarioViewModel UsuarioAvaliado { get; set; }

        public virtual LocacaoViewModel Locacao { get; set; }
    }
}