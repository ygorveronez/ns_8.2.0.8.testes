using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Relatorios
{
    public class OutrosDocumentos
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public OutrosDocumentos(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public byte[] ObterPdf(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            return ReportRequest.WithType(ReportType.OutrosDocumentos)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoCte", cte.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
