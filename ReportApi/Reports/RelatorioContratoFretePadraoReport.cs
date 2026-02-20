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

namespace ReportApi.Reports;

[UseReportType(ReportType.RelatorioContratoFretePadrao)]
public class RelatorioContratoFretePadraoReport : ReportBase
{
    public RelatorioContratoFretePadraoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);

        var contrato = repContratoFreteTransportador.BuscarPorCodigo(extraData.GetValue<int>("CodigoContrato"));
        var excell = extraData.GetValue<bool>("Excel");

        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

        string descricaoPeriodo = "";
        decimal kmConsumido = 0m;

        if (contrato.PeriodoAcordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Semanal)
            descricaoPeriodo = "#index#ª Semana";
        else if (contrato.PeriodoAcordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Decendial)
            descricaoPeriodo = "#index#ª Dezena";
        else if (contrato.PeriodoAcordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Quinzenal)
            descricaoPeriodo = "#index#ª Quinzena";
        else if (contrato.PeriodoAcordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Mensal)
            descricaoPeriodo = "Mês";

        if (configuracaoEmbarcador.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato)
            kmConsumido = Servicos.Embarcador.Carga.ContratoFrete.ObterKmUtilizadoContratoFreteNoPeriodoVigencia(contrato, _unitOfWork);
        else
            kmConsumido = Servicos.Embarcador.Carga.ContratoFrete.ObterKmUtilizadoContratoFreteNoPeriodoAtual(contrato, _unitOfWork);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo1", descricaoPeriodo.Replace("#index#", "1"), true),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo2", descricaoPeriodo.Replace("#index#", "2"), true),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo3", descricaoPeriodo.Replace("#index#", "3"), true),
            };

        Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportador DScontrato = new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportador
        {
            Numero = contrato.Numero,
            Descricao = contrato.Descricao,
            DataInicial = contrato.DataInicial,
            DataFinal = contrato.DataFinal,
            Transportador = contrato.Transportador?.Descricao ?? string.Empty,
            Situacao = contrato.DescricaoSituacao,
            Observacao = contrato.Observacao,

            FranquiaTotalPorCavalo = contrato.FranquiaTotalPorCavalo,
            FranquiaTotalKM = contrato.FranquiaTotalKM,
            FranquiaContratoMensal = contrato.TipoFranquia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.NaoPossui ? contrato.FranquiaContratoMensal : contrato.ValorMensal,
            FranquiaValorKM = contrato.FranquiaValorKM,
            FranquiaValorKmExcedente = contrato.FranquiaValorKmExcedente,
            KMConsumido = kmConsumido
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet veiculos = null;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet filiais = null;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet acordo = null;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet acordo1 = null;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet acordo2 = null;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet tipoCargas = null;
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet tipoOperacoes = null;

        if (contrato.Veiculos != null)
            veiculos = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorVeiculos_.rpt",
                DataSet = (from c in contrato.Veiculos
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorVeiculo
                           {
                               Modelo = c.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                               Placa = c.Veiculo.Placa,
                               CapacidadeKg = c.Veiculo.CapacidadeKG,
                               CapacidadeM3 = c.Veiculo.CapacidadeM3
                           }).ToList()
            };

        if (contrato.Filiais != null)
            filiais = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorFiliais_.rpt",
                DataSet = (from f in contrato.Filiais
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorListaBag
                           {
                               Descricao = f.Filial.Descricao
                           }).ToList()
            };

        if (contrato.Acordos != null)
        {
            acordo = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorAcordos_.rpt",
                DataSet = (from a in contrato.Acordos
                           where a.Periodo == 0
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorAcordo
                           {
                               Modelo = a.ModeloVeicular.Descricao ?? string.Empty,
                               ValorAcordado = a.ValorAcordado,
                               Quantidade = a.Quantidade,
                               Rotulo = a.Rotulo,
                               Total = a.Total,
                               Franquia = a.FranquiaPorKm ? "Sim" : "Não"
                           }).ToList()
            };


            acordo1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorAcordos_1.rpt",
                DataSet = (from a in contrato.Acordos
                           where a.Periodo == 1
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorAcordo
                           {
                               Modelo = a.ModeloVeicular.Descricao ?? string.Empty,
                               ValorAcordado = a.ValorAcordado,
                               Quantidade = a.Quantidade,
                               Rotulo = a.Rotulo,
                               Total = a.Total,
                               Franquia = a.FranquiaPorKm ? "Sim" : "Não"
                           }).ToList()
            };
            acordo2 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorAcordos_2.rpt",
                DataSet = (from a in contrato.Acordos
                           where a.Periodo == 2
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorAcordo
                           {
                               Modelo = a.ModeloVeicular.Descricao ?? string.Empty,
                               ValorAcordado = a.ValorAcordado,
                               Quantidade = a.Quantidade,
                               Rotulo = a.Rotulo,
                               Total = a.Total,
                               Franquia = a.FranquiaPorKm ? "Sim" : "Não"
                           }).ToList()
            };
        }

        if (contrato.TipoCargas != null)
            tipoCargas = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorTipoCargas_.rpt",
                DataSet = (from t in contrato.TipoCargas
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorListaBag
                           {
                               Descricao = t.TipoDeCarga.Descricao
                           }).ToList()
            };

        if (contrato.TipoOperacoes != null)
            tipoOperacoes = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
            {
                Key = "_ContratoFreteTransportadorTipoOperacoes_.rpt",
                DataSet = (from t in contrato.TipoOperacoes
                           select new Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportadorListaBag
                           {
                               Descricao = t.TipoOperacao.Descricao
                           }).ToList()
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            Parameters = parametros,
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ContratoFreteTransportador>() { DScontrato },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    veiculos, filiais, acordo, acordo1, acordo2/*, tipoCargas, tipoOperacoes*/
                }
        };

        Dominio.Enumeradores.TipoArquivoRelatorio tipo = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
        if (excell)
            tipo = Dominio.Enumeradores.TipoArquivoRelatorio.XLS;

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Fretes\ContratoFreteTransportador.rpt", tipo, dataSet, true);

        if (contrato.TipoFranquia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.NaoPossui)
        {
            report.ReportDefinition.Sections["DetailSection3"].SectionFormat.EnableSuppress = true;
            report.ReportDefinition.Sections["DetailSection9"].SectionFormat.EnableSuppress = false;

            CrystalDecisions.CrystalReports.Engine.Subreports sub = report.Subreports;
            if (acordo.DataSet.Count > 0)
                OcultarCamposSubReportAcordo(sub, "_ContratoFreteTransportadorAcordos_.rpt", report.PrintOptions.PageContentWidth);
            if (acordo1.DataSet.Count > 0)
                OcultarCamposSubReportAcordo(sub, "_ContratoFreteTransportadorAcordos_1.rpt", report.PrintOptions.PageContentWidth);
            if (acordo2.DataSet.Count > 0)
                OcultarCamposSubReportAcordo(sub, "_ContratoFreteTransportadorAcordos_2.rpt", report.PrintOptions.PageContentWidth);
        }


        var pdfcontent = RelatorioSemPadraoReportService.ObterBufferReport(report, tipo);

        return PrepareReportResult(excell ? FileType.EXCEL : FileType.PDF, pdfcontent);
    }

    private static void OcultarCamposSubReportAcordo(CrystalDecisions.CrystalReports.Engine.Subreports sub, string subreport, int tamanhoReport)
    {
        CrystalDecisions.CrystalReports.Engine.ReportObject franquia = sub[subreport].ReportDefinition.ReportObjects["Franquia1"];
        franquia.ObjectFormat.EnableSuppress = true;
        CrystalDecisions.CrystalReports.Engine.ReportObject textoFranquia = sub[subreport].ReportDefinition.ReportObjects["Text6"];
        textoFranquia.ObjectFormat.EnableSuppress = true;

        CrystalDecisions.CrystalReports.Engine.ReportObject total = sub[subreport].ReportDefinition.ReportObjects["Total1"];
        total.ObjectFormat.EnableSuppress = true;
        CrystalDecisions.CrystalReports.Engine.ReportObject textoTotal = sub[subreport].ReportDefinition.ReportObjects["Text5"];
        textoTotal.ObjectFormat.EnableSuppress = true;

        CrystalDecisions.CrystalReports.Engine.ReportObject rotulo = sub[subreport].ReportDefinition.ReportObjects["Rotulo1"];
        rotulo.ObjectFormat.EnableSuppress = true;
        CrystalDecisions.CrystalReports.Engine.ReportObject textoRotulo = sub[subreport].ReportDefinition.ReportObjects["Text4"];
        textoRotulo.ObjectFormat.EnableSuppress = true;

        CrystalDecisions.CrystalReports.Engine.ReportObject acordo = sub[subreport].ReportDefinition.ReportObjects["ValorAcordado1"];
        acordo.ObjectFormat.EnableSuppress = true;
        CrystalDecisions.CrystalReports.Engine.ReportObject textoAcordo = sub[subreport].ReportDefinition.ReportObjects["Text2"];
        textoAcordo.ObjectFormat.EnableSuppress = true;


        CrystalDecisions.CrystalReports.Engine.ReportObject quantidade = sub[subreport].ReportDefinition.ReportObjects["Quantidade1"];
        quantidade.Left = 10250;
        CrystalDecisions.CrystalReports.Engine.ReportObject textoQuantidade = sub[subreport].ReportDefinition.ReportObjects["Text3"];
        textoQuantidade.Left = 10250;
    }
}