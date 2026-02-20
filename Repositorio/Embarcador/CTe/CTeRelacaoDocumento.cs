using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeRelacaoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento>
    {
        public CTeRelacaoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento BuscarPorCTeGerado(int codigoCTe)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento>();
            var result = query.Where(obj => obj.CTeGerado.Codigo == codigoCTe);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento BuscarPorCTeOriginal(int codigoCTe)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento>();
            var result = query.Where(obj => obj.CTeOriginal.Codigo == codigoCTe);
            return result.FirstOrDefault();
        }
    }
}
