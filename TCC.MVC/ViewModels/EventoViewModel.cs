using System.Collections.Generic;

namespace TCC.MVC.ViewModels
{
    public class EventoViewModel
    {
        public string EventoId { get; set; }
        
        public string Cep { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string DataHoraInicio { get; set; }

        public string DataHoraFim { get; set; }

        public string CategoriaEvento { get; set; }

        public string NomeEvento { get; set; }

        public bool Ativo { get; set; }

        public bool? Aprovado { get; set; }

        public string MotivoReprovado { get; set; }

        public string Observacao { get; set; }

        //public virtual ICollection<VagaViewModel> Vagas { get; set; }
    }
}