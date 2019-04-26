using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Domain.Entities
{
    public class NotificacaoEmailSms
    {
        public int NotificacaoId { get; set; }

        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public DateTime DataCadastro { get; set; }

        public int TipoNotificacao { get; set; }

        public int MotivoNotificacao { get; set; }
    }
}
