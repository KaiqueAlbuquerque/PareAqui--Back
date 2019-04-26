using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TCC.Domain.Interfaces.Repositories;
using TCC.Infra.Data.Contexto;

namespace TCC.Infra.Data.Repositories
{
    //Este é um repositório genérico
    //TEntity para ser genérico. Como foi feito nas interfaces de 3 - Dominio
    //IDisposable pq quero poder destruir esse cara
    //TBM implementa a interface IRepositoryBase<TEntity> no 3 - Dominio
    //onde TEntity é uma classe. Clicar com o botão direito em IRepositoryBase<TEntity> e escolher na lampada a opção implementar interface
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        protected Context Db = new Context();//Aqui é protected pois se em outros repositórios tiverem métodos especificos daquela classe,
        //eu posso usar esse DB, pois ele é visivel a todas as classes que herdam de RepositoryBase. Exemplo, se em usuario tiver uma consulta
        //expecifica, para ir no banco eu poderia usar este Db, pois UsuarioRepository herda de RespositoryBase

        public void Add(TEntity obj)
        {
            Db.Set<TEntity>().Add(obj);
            Db.SaveChanges();//PROCURAR UNIT OF WORK         
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Db.Set<TEntity>().ToList();//PROCURAR ASNOTRACKING
        }

        public TEntity GetById(int id)
        {
            return Db.Set<TEntity>().Find(id);
        }

        public void Remove(TEntity obj)
        {
            Db.Set<TEntity>().Remove(obj);
            Db.SaveChanges();
        }

        public void Update(TEntity obj)
        {
            Db.Set<TEntity>().Attach(obj);
            Db.Entry(obj).State = EntityState.Modified;
            Db.SaveChanges();
        }
    }
}
