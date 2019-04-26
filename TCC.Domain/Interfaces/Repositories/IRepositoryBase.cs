using System.Collections.Generic;

namespace TCC.Domain.Interfaces.Repositories
{
    //Aqui é TEntity pois não sabemos qual Entity que ela vai tratar. Então TEntity será genérico
    //where TEntity : class -> Estamos dizendo que TEntity é uma classe
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        //Aqui vão os métodos padrões. Os cruds
        void Add(TEntity obj);

        TEntity GetById(int id);

        IEnumerable<TEntity> GetAll();

        void Update(TEntity obj);

        void Remove(TEntity obj);

        void Dispose();
    }
}
