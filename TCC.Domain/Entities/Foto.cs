using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class Foto
    {
        public int FotoId { get; set; }

        public string Imagem { get; set; }

        public int Tipo { get; set; }

        public int? VagaId { get; set; }

        public int? EventoId { get; set; }
    }
}
