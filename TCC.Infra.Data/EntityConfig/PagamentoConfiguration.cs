using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class PagamentoConfiguration : EntityTypeConfiguration<Pagamento>
    {
        public PagamentoConfiguration()
        {
            HasKey(p => p.PagamentoId);

            Property(p => p.Valor)
                .IsRequired();
                        
            Property(p => p.Aprovado)
                .IsOptional();

            Property(p => p.DataPagamento)
                .IsRequired();

            Property(p => p.CodigoPagamento)
                .IsOptional();

            Property(p => p.DataEfetivado)
                .IsOptional();

            HasRequired(p => p.Locacao)
                .WithMany()
                .HasForeignKey(p => p.LocacaoId);

            HasRequired(p => p.DadosPagamento)
                .WithMany()
                .HasForeignKey(p => p.DadosPagamentoId);
        }
    }
}
