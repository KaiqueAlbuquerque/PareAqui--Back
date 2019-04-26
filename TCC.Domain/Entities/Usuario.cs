using System;
using System.Collections.Generic;

namespace TCC.Domain.Entities
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string Sexo { get; set; }
                
        public bool AceitaReceberEmail { get; set; }

        public bool AceitaReceberSms { get; set; }

        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }

        public string Celular { get; set; }

        public string TokenPush { get; set; }

        public virtual IEnumerable<Garagem> Garagens { get; set; }

        public virtual IEnumerable<Veiculo> Veiculos { get; set; }

        public virtual IEnumerable<Locacao> Locacoes { get; set; }

        public virtual IEnumerable<DadosPagamento> DadosPagamento { get; set; }
    }
}
