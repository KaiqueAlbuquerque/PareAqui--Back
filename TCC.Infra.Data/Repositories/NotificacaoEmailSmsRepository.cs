using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class NotificacaoEmailSmsRepository : RepositoryBase<NotificacaoEmailSms>, INotificacaoEmailSmsRepository
    {
    }
}
