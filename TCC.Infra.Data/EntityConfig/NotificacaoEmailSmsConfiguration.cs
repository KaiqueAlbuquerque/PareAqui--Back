using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class NotificacaoEmailSmsConfiguration : EntityTypeConfiguration<NotificacaoEmailSms>
    {
        public NotificacaoEmailSmsConfiguration()
        {
            HasKey(n => n.NotificacaoId);

            Property(n => n.TipoNotificacao)
                .IsRequired();

            Property(n => n.MotivoNotificacao)
                .IsRequired();

            HasRequired(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.UsuarioId);
        }
    }
}
