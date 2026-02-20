using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
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

[UseReportType(ReportType.ConfiguracaoSubcontratacaoTabelaFrete)]
public class ConfiguracaoSubcontratacaoTabelaFreteReport : ReportBase
{
    public ConfiguracaoSubcontratacaoTabelaFreteReport(UnitOfWork unitOfWork,
        RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork,
        servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<int> codigosGruposPessoas = extraData.GetValue<string>("GrupoPessoas").FromJson<List<int>>();
        List<int> codigosTabelasFrete = extraData.GetValue<string>("TabelaFrete").FromJson<List<int>>();

        bool tipoArquivoPDF = extraData.GetValue<bool>("TipoArquivoPDF");

        Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete =
            new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas =
            new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

        IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFrete> configuracoes =
            repTabelaFrete.BuscarRelatorioConfiguracoes(codigosTabelasFrete, codigosGruposPessoas);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteTerceiro>
            configuracoesTerceiros =
                repTabelaFrete.BuscarRelatorioConfiguracoesTerceiros(codigosTabelasFrete, codigosGruposPessoas);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro>
            configuracoesTerceirosTabelaFreteCliente =
                repTabelaFrete.BuscarRelatorioConfiguracoesTerceirosTabelaFreteCliente(codigosTabelasFrete,
                    codigosGruposPessoas);

        if (configuracoes == null || configuracoes.Count == 0)
        {
            configuracoes =
                new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFrete>();
            configuracoes.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFrete()
                {
                    Codigo = 0,
                    Descricao = "",
                    PercentualCobrancaPadrao = 0
                });
        }

        if (configuracoesTerceiros == null || configuracoesTerceiros.Count == 0)
        {
            configuracoesTerceiros =
                new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.
                    ConfiguracaoSubcontratacaoTabelaFreteTerceiro>();
            configuracoesTerceiros.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoSubcontratacaoTabelaFreteTerceiro()
                {
                    Codigo = 0,
                    CodigoTabelaFrete = 0,
                    CPFCNPJTerceiro = 0,
                    NomeTerceiro = "",
                    PercentualCobranca = 0,
                    PercentualDesconto = 0,
                    TipoTerceiro = ""
                });
        }

        if (configuracoesTerceirosTabelaFreteCliente == null || configuracoesTerceirosTabelaFreteCliente.Count == 0)
        {
            configuracoesTerceirosTabelaFreteCliente =
                new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.
                    ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro>();
            configuracoesTerceirosTabelaFreteCliente.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Fretes.
                    ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro()
                    {
                        Codigo = 0,
                        CodigoTabelaFrete = 0,
                        CPFCNPJTerceiro = 0,
                        Destino = "",
                        NomeTerceiro = "",
                        Origem = "",
                        PercentualCobrancaPadrao = 0,
                        PercentualDesconto = 0,
                        TipoTerceiro = "",
                        ValorFixoSubcontratacaoParcial = 0
                    });
        }

        List<string> gruposPessoas = codigosGruposPessoas.Count > 0
            ? repGrupoPessoas.BuscarDescricaoPorCodigo(codigosGruposPessoas)
            : new List<string>();
        List<string> tabelasFrete = codigosTabelasFrete.Count > 0
            ? repTabelaFrete.BuscarDescricaoPorCodigo(codigosTabelasFrete)
            : new List<string>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = configuracoes,
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ConfiguracaoSubcontratacaoTabelaFreteTerceiro",
                        DataSet = configuracoesTerceiros
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "ConfiguracaoSubcontratacaoTabelaFreteClienteTerceiro",
                        DataSet = configuracoesTerceirosTabelaFreteCliente
                    }
                },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TabelaFrete",
                        tabelasFrete.Any() ? string.Join(", ", tabelasFrete) : "Todos", true),
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas",
                        gruposPessoas.Any() ? string.Join(", ", gruposPessoas) : "Todos", true)
                }
            };

        byte[] relatorio = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Fretes\ConfiguracaoSubcontratacaoTabelaFrete.rpt",
            tipoArquivoPDF ? TipoArquivoRelatorio.PDF : TipoArquivoRelatorio.XLS, dataSet, true);

        return PrepareReportResult(tipoArquivoPDF ? FileType.PDF : FileType.EXCEL, relatorio);
    }
}