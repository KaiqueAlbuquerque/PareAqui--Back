using System.Collections.Generic;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;
using TCC.Domain.Interfaces.Services;

namespace TCC.Domain.Services
{
    public class ChatService : ServiceBase<Chat>, IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
            : base(chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public List<Usuario> GetUsersChat(int idUsuario)
        {
            return _chatRepository.GetUsersChat(idUsuario);
        }

        public bool CheckIfNotRegistered(int usuarioLocadorId, int usuarioLocatarioId)
        {
            return _chatRepository.CheckIfNotRegistered(usuarioLocadorId, usuarioLocatarioId);
        }
    }
}
