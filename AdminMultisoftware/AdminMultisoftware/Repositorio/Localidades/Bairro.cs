using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class Bairro : RepositorioBase<Dominio.Entidades.Localidades.Bairro>
    {
        public Bairro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Localidades.Bairro BuscarCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Bairro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

    }
}
