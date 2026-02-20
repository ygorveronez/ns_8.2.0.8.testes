using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Financeiro
{
    public class ValeAvulso : ServicoBase
    {
        public ValeAvulso(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public static byte[] GerarPdfValeAvulso(int codigoValeAvulso, Repositorio.UnitOfWork unitOfWork)
        {
            return ReportRequest.WithType(ReportType.ValeAvulso)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CoidgoValeAvulso", codigoValeAvulso.ToString())
                .CallReport()
                .GetContentFile();
        }
    }
}
