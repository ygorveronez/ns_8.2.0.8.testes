using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class DocumentoFiscal : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal>
    {
        #region Construtores

        public DocumentoFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal BuscarPorCodigo(int codigo)
        {
            var consultaDocumentoFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal>()
                .Where(o => o.Codigo == codigo);

            return consultaDocumentoFiscal.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaDocumentoFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaDocumentoFiscal.FirstOrDefault();
        }

        #endregion
    }
}
