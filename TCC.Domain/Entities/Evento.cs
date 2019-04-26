using System;
using System.Collections.Generic;

namespace TCC.Domain.Entities
{
    public class Evento
    {
        public int EventoId { get; set; }

        public string Cep { get; set; }

        public string Endereco { get; set; }

        public int Numero { get; set; }

        public string Complemento { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime DataHoraInicio { get; set; }

        public DateTime DataHoraFim { get; set; }

        public int CategoriaEvento { get; set; }

        public string NomeEvento { get; set; }

        public bool Ativo { get; set; }

        public bool? Aprovado { get; set; }

        public string MotivoReprovado { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string Observacao { get; set; }

        //public virtual ICollection<Vaga> Vagas { get; set; }
    }
}
