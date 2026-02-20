using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos
{
    public class DAMDFE
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public DAMDFE(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public byte[] Gerar(int codigoMDFe, bool contingencia = false)
        {
            ReportResult result = ReportRequest.WithType(ReportType.CargaMDFe)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoMDFe", codigoMDFe.ToString())
                .AddExtraData("contingencia", contingencia.ToString())
                .CallReport();

            byte[] pdf = result.GetContentFile();

            return pdf;
        }

        #endregion Métodos Públicos
    }
}
