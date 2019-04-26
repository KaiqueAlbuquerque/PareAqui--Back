using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class TransferenciaService : ServiceBase<Transferencia>, ITransferenciaService
    {
        private readonly ITransferenciaRepository _transferenciaRepository;

        public TransferenciaService(ITransferenciaRepository transferenciaRepository)
            : base(transferenciaRepository)
        {
            _transferenciaRepository = transferenciaRepository;
        }
    }
}
