using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
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

[UseReportType(ReportType.ValoresCotacao)]
public class ValoresCotacaoReport : ReportBase
{
    public ValoresCotacaoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(_unitOfWork);

        int codigoCarga = extraData.GetValue<int>("CodigoCarga");
        bool confirmado = extraData.GetValue<bool>("Confirmado");

        List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportador = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

        if (confirmado)
        {
            listaCargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCargaESituacaoDiferente(codigoCarga, SituacaoCargaJanelaCarregamentoTransportador.Disponivel);
        }
        else
        {
            listaCargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCargaComInteresse(codigoCarga);
        }

        System.Reflection.PropertyInfo[] properties =
            typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacao).GetProperties();

        List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacao> dadosSubReport =
            new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacao>();
        Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacaoPrincipal dadoPrincipal =
            new Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacaoPrincipal();

        dadoPrincipal.NumeroCarga = listaCargaJanelaCarregamentoTransportador[0].CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador;

        List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>
            listaTodosComponentesFrete =
                repCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargasJanelaCarregamentoTransportador(
                    listaCargaJanelaCarregamentoTransportador.Select(o => o.Codigo).ToList());

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaTransportador in
                 listaCargaJanelaCarregamentoTransportador)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete>
                listaComponentesFrete = listaTodosComponentesFrete.Where(o =>
                    o.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaTransportador.Codigo).ToList();

            Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacao dataSource =
                new Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacao()
                {
                    Transportador = cargaJanelaTransportador?.Transportador?.Descricao ?? "",
                    ModalDeTransporte =
                        cargaJanelaTransportador?.CargaJanelaCarregamento?.Carga?.TipoOperacao.TipoCobrancaMultimodal == TipoCobrancaMultimodal.Nenhum
                            ? Localization.Resources.Cargas.Carga.Rodoviario
                            : cargaJanelaTransportador?.CargaJanelaCarregamento?.Carga?.TipoOperacao
                                ?.TipoCobrancaMultimodal == TipoCobrancaMultimodal.CTeMultimodal
                                ? Localization.Resources.Cargas.Carga.Multimodal : "",
                    LeadTime = cargaJanelaTransportador?.HorarioCarregamento?.ToDateTimeString(false) ?? "",
                    TipoVeiculo = cargaJanelaTransportador?.CargaJanelaCarregamento?.Carga?.ModeloVeicularCarga?.Descricao ?? "",
                    ValorFrete = cargaJanelaTransportador?.ObterValorFreteTransportador() ?? 0
                };

            decimal somaDeValores = dataSource.ValorFrete;

            foreach (System.Reflection.PropertyInfo property in properties)
            {
                int? numeroComponente = null;
                if (property.Name.Contains("ValorComponenteFrete"))
                    numeroComponente = property.Name.ObterSomenteNumeros().ToInt();

                if (numeroComponente >= listaComponentesFrete.Count)
                    break;

                if (numeroComponente.HasValue)
                {
                    if (listaComponentesFrete[numeroComponente.Value].TipoValor ==
                        TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
                    {
                        property.SetValue(dataSource,listaComponentesFrete[numeroComponente.Value].Percentual.ToString("F2") + "%");
                    }
                    else
                    {
                        property.SetValue(dataSource,"R$" + (string.Format("{0:0,0.00}", listaComponentesFrete[numeroComponente.Value].ValorComponente).ToString()));
                        somaDeValores += listaComponentesFrete[numeroComponente.Value].ValorComponente;
                    }
                }
            }

            foreach (System.Reflection.PropertyInfo property in properties)
            {
                int? numeroComponente = null;
                if (property.Name.Contains("ValorComponenteFrete") && property.Name.Contains("Descricao"))
                    numeroComponente = property.Name.ObterSomenteNumeros().ToInt();

                if (numeroComponente >= listaComponentesFrete.Count)
                    break;

                if (numeroComponente.HasValue)
                    property.SetValue(dataSource,
                        (listaComponentesFrete[numeroComponente.Value].ComponenteFrete?.Descricao ??
                         listaComponentesFrete[numeroComponente.Value].DescricaoComponente).ToString());
            }

            dataSource.SomaDeValores = somaDeValores;

            dadosSubReport.Add(dataSource);
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "ValorCotacao",
                DataSet = dadosSubReport
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ValoresCotacaoPrincipal>()
                    { dadoPrincipal },
                Parameters = null,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> { ds1 }
            };

        byte[] arquivo = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Logistica\ValoresCotacao.rpt", TipoArquivoRelatorio.PDF, dataSet,
            possuiLogo: true);

        return PrepareReportResult(FileType.PDF, arquivo);
    }
}