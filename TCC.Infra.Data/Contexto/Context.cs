using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Infra.Data.EntityConfig;

namespace TCC.Infra.Data.Contexto
{
    public class Context : DbContext
    {
        public Context()
            : base("PareAqui") 
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Garagem> Garagens { get; set; }
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<DadosPagamento> DadosPagamentos { get; set; }
        public DbSet<NotificacaoEmailSms> NotificacaoEmailSms { get; set; }
        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<DadosBancario> DadosBancarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Para não pluralizar o nome das tabelas
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Para não deletar em cascata quando tiver um para muitos
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //Para não deletar em cascata quando tiver relacionamento muitos para muitos
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //Quando vc tiver uma propriedade, e o nome dela for o nome dela for alguma coisa mais 
            //id no final, ela será a chave primaria desta tabela. EX: ClienteId
            modelBuilder.Properties()
                .Where(p => p.Name == p.ReflectedType.Name + "Id")
                .Configure(p => p.IsKey());

            //Quando vc tiver propriedades string, no banco sera varchar 
            //Se não colocar isso no banco por padrao sera nvarchar que oculpa 2x mais espaço
            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar"));

            //Se quando criarmos nao dissermos o tamanho, por padrão ele vai
            //colocar varchar(100)
            modelBuilder.Properties<string>()
                .Configure(p => p.HasMaxLength(100));

            modelBuilder.Configurations.Add(new UsuarioConfiguration());
            modelBuilder.Configurations.Add(new GaragemConfiguration());
            modelBuilder.Configurations.Add(new VagaConfiguration());
            modelBuilder.Configurations.Add(new VeiculoConfiguration());
            modelBuilder.Configurations.Add(new EventoConfiguration());
            modelBuilder.Configurations.Add(new LocacaoConfiguration());
            modelBuilder.Configurations.Add(new ChatConfiguration());
            modelBuilder.Configurations.Add(new AvaliacaoConfiguration());
            modelBuilder.Configurations.Add(new FotoConfiguration());
            modelBuilder.Configurations.Add(new PagamentoConfiguration());
            modelBuilder.Configurations.Add(new DadosPagamentoConfiguration());
            modelBuilder.Configurations.Add(new NotificacaoEmailSmsConfiguration());
            modelBuilder.Configurations.Add(new TransferenciaConfiguration());
            modelBuilder.Configurations.Add(new DadosBancarioConfiguration());
        }

        //Este método esta sobrescrevendo o metodo SaveChanges. Sempre que este metodo for chamado, ele verifica se a entidade possui a propriedade
        //DataCadastro e ela for diferente de null. Se ele tiver adicionando (Added), o valor dela será DateTime.Now
        //Se tiver modificando, ele nao faz nada, ja que é dataCadastro, uma vez que ela entrou ela fica
        //Com isso, não precisamos passar o valor do DataCadastro ao cadastrar um usuario por exemplo
        public override int SaveChanges()
        {
            foreach(var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if(entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if(entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }
            return base.SaveChanges();
        }
    }
}
