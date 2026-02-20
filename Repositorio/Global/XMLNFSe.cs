using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class XMLNFSe : RepositorioBase<Dominio.Entidades.XMLNFSe>, Dominio.Interfaces.Repositorios.XMLNFSe
    {
        public XMLNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.XMLNFSe BuscarPorNFSe(int codigoNFSe, Dominio.Enumeradores.TipoXMLNFSe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNFSe>();

            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Tipo == tipo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.XMLNFSe> BuscarPorCodigosNFSe(List<int> codigosNFes, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNFSe>();

            var result = from obj in query where codigosNFes.Contains(obj.NFSe.Codigo) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.NFSe.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }
    }
}
