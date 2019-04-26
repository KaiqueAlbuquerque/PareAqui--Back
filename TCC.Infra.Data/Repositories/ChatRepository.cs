using System.Collections.Generic;
using System.Linq;
using TCC.Domain.Entities;
using TCC.Domain.Interfaces.Repositories;

namespace TCC.Infra.Data.Repositories
{
    public class ChatRepository : RepositoryBase<Chat>, IChatRepository
    {
        public List<Usuario> GetUsersChat(int idUsuario)
        {
            List<Usuario> usuarios = new List<Usuario>();
            
            var usuariosLocador = Db.Chats.Where(c => c.UsuarioLocadorId == idUsuario);

            foreach (var u in usuariosLocador)
            {
                var usuarioLocatario = Db.Usuarios.Where(x => x.UsuarioId == u.UsuarioLocatarioId).FirstOrDefault();
                usuarios.Add(usuarioLocatario);
            }
            
            var usuariosLocatario = Db.Chats.Where(c => c.UsuarioLocatarioId == idUsuario);

            foreach (var u in usuariosLocatario)
            {
                var usuarioLocador = Db.Usuarios.Where(x => x.UsuarioId == u.UsuarioLocadorId).FirstOrDefault();
                usuarios.Add(usuarioLocador);
            }

            usuarios = usuarios.OrderBy(e => e.Nome).Distinct().ToList();

            return usuarios;
        }

        public bool CheckIfNotRegistered(int usuarioLocadorId, int usuarioLocatarioId)
        {
            var jahExiste = Db.Chats.Where(c => c.UsuarioLocadorId == usuarioLocadorId && c.UsuarioLocatarioId == usuarioLocatarioId ||
                            c.UsuarioLocadorId == usuarioLocatarioId && c.UsuarioLocatarioId == usuarioLocadorId).FirstOrDefault();

            if(jahExiste != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
