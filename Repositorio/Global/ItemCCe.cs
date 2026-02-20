using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ItemCCe : RepositorioBase<Dominio.Entidades.ItemCCe>, Dominio.Interfaces.Repositorios.ItemCCe
    {
        public ItemCCe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ItemCCe BuscarPorCodigo(int codigo, int codigoCCe){
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemCCe>();
            var result = from obj in query where obj.CCe.Codigo == codigoCCe && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ItemCCe> BuscarPorCCe(int codigoCCe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemCCe>();
            var result = from obj in query where obj.CCe.Codigo == codigoCCe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ItemCCe> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemCCe>();
            var result = from obj in query where obj.CCe.CTe.Codigo == codigoCTe && obj.CCe.Status == Dominio.Enumeradores.StatusCCe.Autorizado select obj;
            return result.ToList();
        }
    }
}
