using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ValePedagioCTe : RepositorioBase<Dominio.Entidades.ValePedagioCTe>, Dominio.Interfaces.Repositorios.ValePedagioCTe
    {
        public ValePedagioCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ValePedagioCTe BuscarPorCodigo(int codigo, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioCTe>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ValePedagioCTe> BuscarPorMDFe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }
    }
}
