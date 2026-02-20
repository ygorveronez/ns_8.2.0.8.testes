using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class DocumentosTransporte : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte>
    {
        #region Construtores

        public DocumentosTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte BuscarPorCodigo(int codigo)
        {
            var consultaDocumentosTransporte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte>()
                .Where(o => o.Codigo == codigo);

            return consultaDocumentosTransporte.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaDocumentosTransporte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaDocumentosTransporte.FirstOrDefault();
        }

        #endregion
    }
}
