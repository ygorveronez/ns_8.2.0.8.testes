using System.Collections.Generic;
using System.Linq;
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

[UseReportType(ReportType.VeiculosQrCode)]
public class VeiculoQrCodeReport : ReportBase
{

    public VeiculoQrCodeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var veiculos = extraData.GetValue<string>("Veiculos").FromJson<List<Dominio.Entidades.Veiculo>>();
        List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.VeiculoQrCode> dataSourceVeiculoQrCode = (
            from veiculo in veiculos
            select new Dominio.Relatorios.Embarcador.DataSource.Veiculos.VeiculoQrCode()
            {
                QRCode = Utilidades.QRcode.Gerar(veiculo.Placa),
                ModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Descricao,
                Placa = veiculo.Placa_Formatada,
                Transportador = veiculo.Empresa?.Descricao ?? ""
            }
        ).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = dataSourceVeiculoQrCode,
            Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Veiculos\VeiculosQrCode.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}