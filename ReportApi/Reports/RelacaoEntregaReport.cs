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

[UseReportType(ReportType.RelacaoEntrega)]
public class RelacaoEntregaReport : ReportBase
{
    public RelacaoEntregaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        string nomeEmpresa = extraData.GetValue<string>("NomeEmpresa");
        var dadosRelacaoEntrega = extraData.GetValue<string>("DadosRelacaoEntrega")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega>>();
        var dadosRelacaoEntregaDocumento = extraData.GetValue<string>("DadosRelacaoEntregaDocumento")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento>>();
        var dadosRelacaoEntregaMotorista = extraData.GetValue<string>("DadosRelacaoEntregaMotorista")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista>>();
        var dadosRelacaoEntregaReboque = extraData.GetValue<string>("DadosRelacaoEntregaReboque")
            .FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("RelatorioControleGeracao");
        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = GerarRelatorioRelacaoEntrega(nomeEmpresa,
            dadosRelacaoEntrega, dadosRelacaoEntregaDocumento, dadosRelacaoEntregaMotorista,
            dadosRelacaoEntregaReboque);

        _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Cargas/Carga", _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument GerarRelatorioRelacaoEntrega(string nomeEmpresa,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega> dadosRelacaoEntrega,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento> dadosRelacaoEntregaDocumento,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista> dadosRelacaoEntregaMotorista,
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque> dadosRelacaoEntregaReboque)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro empresa =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        empresa.NomeParametro = "Empresa";
        empresa.ValorParametro = nomeEmpresa;
        parametros.Add(empresa);

        if (dadosRelacaoEntregaDocumento.Count == 0)
        {
            dadosRelacaoEntregaDocumento.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento()
                {
                    CodigoCarga = 1
                });
        }

        if (dadosRelacaoEntregaMotorista.Count == 0)
        {
            dadosRelacaoEntregaMotorista.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista()
                {
                    CodigoCarga = 1
                });
        }

        if (dadosRelacaoEntregaReboque.Count == 0)
        {
            dadosRelacaoEntregaReboque.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque()
                {
                    CodigoCarga = 1
                });
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RelacaoEntregaDocumento.rpt",
                DataSet = dadosRelacaoEntregaDocumento
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RelacaoEntregaMotorista.rpt",
                DataSet = dadosRelacaoEntregaMotorista
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "RelacaoEntregaReboque.rpt",
                DataSet = dadosRelacaoEntregaReboque
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dadosRelacaoEntrega,
                Parameters = parametros,
                SubReports = subReports
            };

        return RelatorioSemPadraoReportService.GerarCrystalReport(
            @"Areas\Relatorios\Reports\Default\Pedidos\RelacaoEntrega.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
    }
}