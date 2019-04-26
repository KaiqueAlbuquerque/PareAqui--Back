using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCC.MVC.ViewModels
{
    public class EventoAprovacaoViewModel
    {
        public EventoViewModel Evento { get; set; }

        public List<string> Imagens { get; set; }
    }
}