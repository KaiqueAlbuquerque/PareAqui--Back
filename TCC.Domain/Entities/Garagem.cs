using System.Collections.Generic;

namespace TCC.Domain.Entities
{
    public class Garagem 
    {
        public int GaragemId { get; set; }

        public string Cep { get; set; }

        public string Endereco { get; set; }

        public int NumeroRua { get; set; }

        public string Bairro { get; set; }

        public string Complemento { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }
        
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool Condominio { get; set; }

        public bool? Ativo { get; set; }
                
        public virtual IEnumerable<Vaga> Vagas { get; set; }
    }
}