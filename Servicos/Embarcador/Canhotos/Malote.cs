using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Canhotos;

public class Malote
{
    public static byte[] GerarImpressaoProtocolo(Dominio.Entidades.Embarcador.Canhotos.Malote malote, Repositorio.UnitOfWork unitOfWork)
    {
        return ReportRequest.WithType(ReportType.ImpressaoProtocolo)
            .WithExecutionType(ExecutionType.Sync)
            .AddExtraData("CodigoMalote", malote.Codigo.ToString())
            .CallReport()
            .GetContentFile();
    }
    
    public static byte[] GerarImpressaoProtocoloEntrega(Dominio.Entidades.Embarcador.Canhotos.Malote malote, Repositorio.UnitOfWork unitOfWork)
    {
        return ReportRequest.WithType(ReportType.ImpressaoProtocoloEntrega)
            .WithExecutionType(ExecutionType.Sync)
            .AddExtraData("CodigoMalote", malote.Codigo.ToString())
            .CallReport()
            .GetContentFile();
    }

}