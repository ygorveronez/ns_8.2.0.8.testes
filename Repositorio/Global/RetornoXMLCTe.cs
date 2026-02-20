using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class RetornoXMLCTe : RepositorioBase<Dominio.Entidades.RetornoXMLCTe>, Dominio.Interfaces.Repositorios.RetornoXMLCTe
    {
        public RetornoXMLCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.RetornoXMLCTe> BuscarPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }


        public bool ExisteRegistroCTePorStatus(int codigoCTe, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RetornoXMLCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.Count() > 0;
        }
    }
}