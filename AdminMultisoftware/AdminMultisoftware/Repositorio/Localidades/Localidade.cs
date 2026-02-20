using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class Localidade : RepositorioBase<Dominio.Entidades.Localidades.Bairro>
    {
        public Localidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Localidades.Localidade BuscarCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Localidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidades.Localidade BuscarCodigoIBGE(string codigoIBGE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Localidade>();
            var result = from obj in query where obj.CodigoIBGE.Equals(codigoIBGE) select obj;
            return result.FirstOrDefault();
        }

    }
}
