using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ValePedagioMDFeCompraXML : RepositorioBase<Dominio.Entidades.ValePedagioMDFeCompraXML>, Dominio.Interfaces.Repositorios.ValePedagioMDFeCompraXML
    {
        public ValePedagioMDFeCompraXML(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ValePedagioMDFeCompraXML BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompraXML>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompraXML> BuscarPorValePedagioMDFeCompra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompraXML>();
            var result = from obj in query where obj.ValePedagioMDFeCompra.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
