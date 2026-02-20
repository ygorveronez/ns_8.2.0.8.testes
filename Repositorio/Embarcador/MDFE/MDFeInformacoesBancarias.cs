using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.MDFE
{

    public class MDFeInformacoesBancarias : RepositorioBase<Dominio.Entidades.MDFeInformacoesBancarias>
    {
        #region Construtores

        public MDFeInformacoesBancarias(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MDFeInformacoesBancarias(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Metodos Públicos
        public Dominio.Entidades.MDFeInformacoesBancarias BuscarPorMDFe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeInformacoesBancarias>();

            query = query.Where(o => o.MDFe.Codigo == codigo);

            return query.FirstOrDefault();
        }
        #endregion
    }
}
