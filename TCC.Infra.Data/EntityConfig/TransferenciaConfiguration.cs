using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class TransferenciaConfiguration : EntityTypeConfiguration<Transferencia>
    {
        public TransferenciaConfiguration()
        {
            HasKey(t => t.TransferenciaId);

            Property(t => t.Valor)
                .IsRequired();

            Property(t => t.DataPagamento)
                .IsRequired();

            Property(t => t.DataEfetivado)
                .IsOptional();

            Property(t => t.Aprovado)
                .IsOptional();

            Property(t => t.CodigoTransferencia)
                .IsOptional();

            HasRequired(t => t.Locacao)
                .WithMany()
                .HasForeignKey(t => t.LocacaoId);

            HasRequired(t => t.DadosBancario)
                .WithMany()
                .HasForeignKey(t => t.DadosBancarioId);
        }
    }
}
