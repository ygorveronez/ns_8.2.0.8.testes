using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.EtapaQrCode)]
public class EtapasQrCodeReport : ReportBase
{
    public EtapasQrCodeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }
    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoFilial = extraData.GetValue<int>("CodigoFilial");
        var etapa = extraData.GetValue<EtapaFluxoGestaoPatio>("Etapa");
        var tipo = extraData.GetValue<TipoFluxoGestaoPatio>("Tipo");

        return PrepareReportResult(FileType.PDF, ObterPdfQrCodeEtapa(codigoFilial, etapa, tipo));
    }
    
    public byte[] ObterPdfQrCodeEtapa(int codigoFilial, EtapaFluxoGestaoPatio etapa, TipoFluxoGestaoPatio tipo)
    {
        var repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);

        if (filial == null)
            throw new ServicoException("Não foi possível encontrar o registro.");

        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
        Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa gestaoPatioEtapa = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapa(etapa, tipo, codigoFilial);

        if (gestaoPatioEtapa == null)
            throw new ServicoException("A etapa não está ativa.");

        if (!gestaoPatioEtapa.PermiteQRCode)
            throw new ServicoException("A etapa não está configurada para baixar o QR Code.");

        Dominio.Relatorios.Embarcador.DataSource.Filiais.FilialQrCode filialQrCode = ObterFilialQrCodePorEtapa(filial, gestaoPatioEtapa);

        return ObterPdfTodosQrCode(new List<Dominio.Relatorios.Embarcador.DataSource.Filiais.FilialQrCode>() { filialQrCode });
    }
    
    private Dominio.Relatorios.Embarcador.DataSource.Filiais.FilialQrCode ObterFilialQrCodePorEtapa(Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa gestaoPatioEtapa)
    {
        return new Dominio.Relatorios.Embarcador.DataSource.Filiais.FilialQrCode()
        {
            Etapa = gestaoPatioEtapa.Descricao,
            Filial = filial.Descricao,
            QRCode = Utilidades.QRcode.Gerar($"{(int)TipoQRCode.FluxoPatio}|{(int)gestaoPatioEtapa.Etapa}|{filial.Codigo}")
        };
    }

    public byte[] ObterPdfTodosQrCode(List<Dominio.Relatorios.Embarcador.DataSource.Filiais.FilialQrCode> dataSourceFilialQrCode)
    {
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dataSourceFilialQrCode,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Filiais\EtapasQrCode.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o QR Code.");

        return pdf;
    }
}