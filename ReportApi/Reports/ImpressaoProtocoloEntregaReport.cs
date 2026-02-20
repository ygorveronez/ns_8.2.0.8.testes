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

[UseReportType(ReportType.ImpressaoProtocoloEntrega)]
public class ImpressaoProtocoloEntregaReport : ReportBase
{
    public ImpressaoProtocoloEntregaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) :
        base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoMalote = extraData.GetValue<int>("CodigoMalote");
        Malote maloteRepositorio = new Malote(_unitOfWork);
        var malote = maloteRepositorio.BuscarPorCodigo(codigoMalote);

        Repositorio.Embarcador.Canhotos.Canhoto repositorioMaloteCanhoto =
            new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> malotesCanhoto =
            repositorioMaloteCanhoto.BuscarMaloteCanhotoPorCodigo(malote.Codigo);

        Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.ProtocoloEntrega protocoloEntrega =
            new Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.ProtocoloEntrega()
            {
                Filial = $"{malote.Filial.Descricao} ({malote.Filial.CNPJ_Formatado})",
                DataHora = malote.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                NomeRecebedor = malote.Operador.Nome,
                NumeroProtocolo = malote.Protocolo.ToString("D8"),
                Transportadora = $"{malote.Empresa.RazaoSocial} - CNPJ: {malote.Empresa.CNPJ_Formatado}",
            };

        List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.ProtocoloEntregaNotas> protocoloEntregaNotas =
            new List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.ProtocoloEntregaNotas>();

        foreach (var maloteCanhoto in malotesCanhoto)
        {
            protocoloEntregaNotas.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.ProtocoloEntregaNotas()
                {
                    NumeroNotaFiscal = maloteCanhoto.XMLNotaFiscal.Numero.ToString(),
                    ChaveNotaFiscal = maloteCanhoto.XMLNotaFiscal.Chave
                });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet protocoloEntregaNotasFiscais =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "ProtocoloEntregaNotas",
                DataSet = protocoloEntregaNotas
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Malote.ProtocoloEntrega>()
                    { protocoloEntrega },
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>
                    { protocoloEntregaNotasFiscais },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>() { }
            };

        var pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Canhotos\ProtocoloEntrega.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        
        return PrepareReportResult(FileType.PDF, pdfContent);
    }
}