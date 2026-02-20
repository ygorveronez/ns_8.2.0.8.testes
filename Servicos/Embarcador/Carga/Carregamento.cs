using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
namespace Servicos.Embarcador.Carga;
public class Carregamento
{
    public byte[] RelatorioTroca(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool relatorioTroca, Repositorio.UnitOfWork unitOfWork, ref string mensagem)
    {
        var result = ReportRequest.WithType(ReportType.CarregamentoRelatorioTroca)
            .WithExecutionType(ExecutionType.Sync)
            .AddExtraData("CodigoCarga", carga.Codigo.ToString())
            .AddExtraData("relatorioTroca", relatorioTroca.ToString())
            .CallReport();

        mensagem = result.ErrorMessage;
        return result.GetContentFile();
    }
}