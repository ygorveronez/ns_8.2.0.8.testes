using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.AutorizacaoEmbarqueOpenTech)]
public class AutorizacaoEmbarqueOpenTechReport : ReportBase
{
    public AutorizacaoEmbarqueOpenTechReport(UnitOfWork unitOfWork,
        RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork,
        servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCodigo(extraData.GetValue<int>("codigocargaIntegracao"));
        
        List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque> autorizacaoEmbarque = extraData.GetValue<string>("autorizacaoEmbarque").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.AutorizacaoEmbarque>>();
        List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota> trechos = extraData.GetValue<string>("trechos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.TrechoRota>>();
        List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento> documentos =extraData.GetValue<string>("documentos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.Documento>>();
        List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido> locaisParada =extraData.GetValue<string>("locaisParada").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.LocalParadaSugerido>>();
        List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado> produtos =extraData.GetValue<string>("produtos").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.OpenTech.ProdutoInformado>>();
        
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = autorizacaoEmbarque,
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet() { Key = "AutorizacaoEmbarqueOpenTech_Trechos.rpt", DataSet = trechos },
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet() { Key = "AutorizacaoEmbarqueOpenTech_Documentos.rpt", DataSet = documentos },
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet() { Key = "AutorizacaoEmbarqueOpenTech_LocaisParada.rpt", DataSet = locaisParada },
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet() { Key = "AutorizacaoEmbarqueOpenTech_Produtos.rpt", DataSet = produtos }
            }
        };
        
        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\OutrosDocumentos\AutorizacaoEmbarqueOpenTech.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true); 
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}