using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.RelacaoDocumentosNFSManual)]
public class RelacaoDocumentosNFSManualReport:ReportBase
{
    public RelacaoDocumentosNFSManualReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
        
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
        Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual =
            repLancamentoNFSManual.BuscarPorCodigo(extraData.GetValue<int>("codigolancamentoNFSManual"));
        
            
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
        List<Dominio.Relatorios.Embarcador.DataSource.NFSManual.Documento> documentos = repCargaDocumentoParaEmissaoNFSManual.BuscarParaImpressaoRelacao(lancamentoNFSManual.Codigo);
        Dominio.Relatorios.Embarcador.DataSource.NFSManual.NFSManual nfsManual = new Dominio.Relatorios.Embarcador.DataSource.NFSManual.NFSManual()
        {
            CNPJEmpresa = lancamentoNFSManual.Transportador.CNPJ,
            Empresa = lancamentoNFSManual.Transportador.RazaoSocial,
            CPFCNPJTomador = lancamentoNFSManual.Tomador.CPF_CNPJ,
            TipoPessoaTomador = lancamentoNFSManual.Tomador.Tipo,
            Tomador = lancamentoNFSManual.Tomador.Nome
        };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.NFSManual.NFSManual>() { nfsManual },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "RelacaoDocumentosNFSManual_Documentos.rpt",
                    DataSet = documentos
                }
            }
        };

        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\NFSManual\RelacaoDocumentosNFSManual.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}