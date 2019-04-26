using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class Chat
    {
        public int ChatId { get; set; }

        public int UsuarioLocadorId { get; set; }

        public int UsuarioLocatarioId { get; set; }

        public virtual Usuario UsuarioLocador { get; set; }

        public virtual Usuario UsuarioLocatario { get; set; }
    }
}
