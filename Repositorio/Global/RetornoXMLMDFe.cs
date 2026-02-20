using System.Linq;

namespace Repositorio
{
    public class RetornoXMLMDFe : RepositorioBase<Dominio.Entidades.RetornoXMLMDFe>, Dominio.Interfaces.Repositorios.RetornoXMLMDFe
    {
        public RetornoXMLMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.RetornoXMLMDFe BuscarPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }
    }
}