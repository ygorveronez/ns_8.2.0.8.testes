using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Repositorio.Embarcador.Canhotos;
using Servicos.Embarcador.Relatorios;


namespace ReportApi.Reports;

[UseReportType(ReportType.ImpressaoProtocolo)]

public class ImpressaoProtocoloReport : ReportBase
{
    public ImpressaoProtocoloReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoMalote = extraData.GetValue<int>("CodigoMalote");
        Malote maloteRepositorio = new Malote(_unitOfWork);
        var malote = maloteRepositorio.BuscarPorCodigo(codigoMalote);

        List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.Protocolo> dsProtocolo = new List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.Protocolo>()
        {
            new Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.Protocolo()
            {
                NumeroProtocolo = malote.Protocolo.ToString("D8"),
                Filial = malote.Filial.CNPJ + " - " + malote.Filial.Descricao,
                DataEnvio = malote.DataEnvio,
                QuantidadeDocumentos = malote.QuantidadeCanhotos,
                Transportadora = malote.Empresa.RazaoSocial
            }
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dsProtocolo,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            {
            }
        };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Canhotos\Protocolo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}