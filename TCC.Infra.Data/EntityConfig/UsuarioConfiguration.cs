using System.Data.Entity.ModelConfiguration;
using TCC.Domain.Entities;

namespace TCC.Infra.Data.EntityConfig
{
    public class UsuarioConfiguration : EntityTypeConfiguration<Usuario>
    {
        public UsuarioConfiguration()
        {
            //Abaixo estamos usando FluenApi para mapear 
            //comportamentos da minha entidade de dominio
            //para ela ser modelada como uma tabela da minha
            //base de dados

            //Embora ele ja saiba, estou dizendo novamente
            //que o usuario possui uma propriedade UsuarioId
            //e que essa propreidade é minha chave primaria
            HasKey(u => u.UsuarioId);

            //Caso eu nao coloque nada a propriedade nome
            //sera criada com varchar(100)
            Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(150);

            Property(u => u.Email)
                .IsRequired();

            Property(u => u.Senha)
                .IsRequired();

            Property(u => u.Ativo)
                .IsRequired();
            
            Property(u => u.AceitaReceberEmail)
                .IsRequired();

            Property(u => u.AceitaReceberSms)
                .IsRequired();

            Property(u => u.Celular)
                .IsOptional()
                .HasMaxLength(11);

            Property(u => u.TokenPush)
                .IsOptional()
                .HasMaxLength(8000);

            Property(u => u.Sexo)
                .IsRequired()
                .HasColumnType("char")
                .HasMaxLength(1);
        }
    }
}
