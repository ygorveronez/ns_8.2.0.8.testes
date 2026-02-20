using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class Estado : RepositorioBase<Dominio.Entidades.Localidades.Endereco>
    {
        public Estado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Localidades.Estado BuscarUF(string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Estado>();
            var result = from obj in query where obj.UF.Equals(uf) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Localidades.Estado BuscarCodigoIBGE(string codigoIBGE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Estado>();
            var result = from obj in query where obj.CodigoIBGE.Equals(codigoIBGE) select obj;
            return result.FirstOrDefault();
        }
    }
}