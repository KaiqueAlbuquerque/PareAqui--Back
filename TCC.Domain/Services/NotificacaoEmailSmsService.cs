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
    public class NotificacaoEmailSmsService : ServiceBase<NotificacaoEmailSms>, INotificacaoEmailSmsService
    {
        private readonly INotificacaoEmailSmsRepository _notificacaoEmailSmsRepository;

        public NotificacaoEmailSmsService(INotificacaoEmailSmsRepository notificacaoEmailSmsRepository)
            : base(notificacaoEmailSmsRepository)
        {
            _notificacaoEmailSmsRepository = notificacaoEmailSmsRepository;
        }
    }
}
