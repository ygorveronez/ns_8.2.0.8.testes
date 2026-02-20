using System.Linq;

namespace Repositorio
{
    public class RelacaoCTesEntreguesColeta : RepositorioBase<Dominio.Entidades.RelacaoCTesEntreguesColeta>
    {

        public RelacaoCTesEntreguesColeta(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RelacaoCTesEntreguesColeta BuscarPorCodigo(int codigo, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntreguesColeta>();

            var result = from obj in query where obj.Codigo == codigo && obj.RelacaoCTesEntregues.Empresa.Codigo == empresa select obj;

            return result.FirstOrDefault();
        }
    }
}
