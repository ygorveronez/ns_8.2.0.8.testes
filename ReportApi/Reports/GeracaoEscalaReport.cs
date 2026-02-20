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

[UseReportType(ReportType.GeracaoEscala)]
public class GeracaoEscalaReport : ReportBase
{
    public GeracaoEscalaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var repGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(_unitOfWork);

        var geracaoEscala = repGeracaoEscala.BuscarPorCodigo(extraData.GetValue<int>("CodigoGeracaoEscala"));
        var tipoArquivoRelatorio = extraData.GetValue<string>("TipoArquivoRelatorio")
            .ToEnum<Dominio.Enumeradores.TipoArquivoRelatorio>();

        Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado repositorioEscalaVeiculoEscalado =
            new Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado> origensDestinos =
            repositorioEscalaVeiculoEscalado.BuscarPorGeracaoEscala(geracaoEscala.Codigo);
        List<Dominio.Relatorios.Embarcador.DataSource.Escala.GeracaoEscala> dataSourceGeracaoEscala = (
            from o in origensDestinos
            select new Dominio.Relatorios.Embarcador.DataSource.Escala.GeracaoEscala()
            {
                Capacidade = o.Quantidade,
                DescricaoOrigemDestino =
                    $"{o.EscalaOrigemDestino.CentroCarregamento.Descricao} até {o.EscalaOrigemDestino.ClienteDestino.Descricao} - {o.EscalaOrigemDestino.ExpedicaoEscala.ProdutoEmbarcador?.Descricao ?? ""}",
                Destino = o.EscalaOrigemDestino.ClienteDestino.Descricao,
                Empresa = o.Empresa.Descricao,
                Hora = o.HoraCarregamento.ToString(@"hh\:mm"),
                ModeloVeicularCarga = o.ModeloVeicularCarga.Descricao,
                Motorista = o.Motoristas?.FirstOrDefault()?.Nome ?? "",
                Origem = o.EscalaOrigemDestino.CentroCarregamento.Descricao
            }
        ).ToList();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dataSourceGeracaoEscala,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            };

        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = ObterRelatorioTemporario(geracaoEscala, extraData.GetInfo().TipoServico);
        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio =
            RelatorioSemPadraoReportService.GerarCrystalReport(
                @"Areas\Relatorios\Reports\Default\Escalas\GeracaoEscala.rpt", tipoArquivoRelatorio, dataSet, true);
        Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT() { ContadorRegistrosGrupo = "", ContadorRegistrosTotal = "" };

        relatorio.PrintOptions.PaperOrientation =
            relatorioTemporario.OrientacaoRelatorio ==
            Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato
                ? CrystalDecisions.Shared.PaperOrientation.Portrait
                : CrystalDecisions.Shared.PaperOrientation.Landscape;
        relatorio.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

        _servicoRelatorioReportService.SetarSubCabecalhoRelatorio(relatorio.Subreports, relatorio, tipoArquivoRelatorio,
            relatorioTemporario.Titulo, relatorioTemporario.FontePadrao, IdentificacaoCamposRPT, _unitOfWork);
        _servicoRelatorioReportService.SetarSubRodapeRelatorio(relatorio.Subreports, relatorio,
            relatorioTemporario.CodigoControleRelatorios, relatorioTemporario.FontePadrao, tipoArquivoRelatorio,
            IdentificacaoCamposRPT);
        _servicoRelatorioReportService.FormatarRelatorio(relatorio, relatorioTemporario, tipoArquivoRelatorio,
            IdentificacaoCamposRPT);

        byte[] arquivo = RelatorioSemPadraoReportService.ObterBufferReport(relatorio, tipoArquivoRelatorio);

        return PrepareReportResult(tipoArquivoRelatorio == TipoArquivoRelatorio.PDF ? FileType.PDF : FileType.EXCEL, arquivo);
    }

    private Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporario(
        Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala,
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
    {
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario =
            new Dominio.Entidades.Embarcador.Relatorios.Relatorio();

        relatorioTemporario.ArquivoRelatorio = "GeracaoEscala.rpt";
        relatorioTemporario.Ativo = true;
        relatorioTemporario.CaminhoRelatorio = @"Areas\Relatorios\Reports\Default\Escalas";
        relatorioTemporario.CodigoControleRelatorios =
            Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R000_Nenhum;
        relatorioTemporario.TimeOutMinutos = 30;
        relatorioTemporario.CortarLinhas = true;
        relatorioTemporario.Descricao = "Relatório Padrão";
        relatorioTemporario.ExibirSumarios = true;
        relatorioTemporario.FontePadrao = "Arial";
        relatorioTemporario.PropriedadeAgrupa = "DescricaoOrigemDestino";
        relatorioTemporario.OrdemAgrupamento = "asc";
        relatorioTemporario.PropriedadeOrdena = "Hora";
        relatorioTemporario.OrdemOrdenacao = "asc";
        relatorioTemporario.FundoListrado = false;
        relatorioTemporario.OrientacaoRelatorio =
            Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato;
        relatorioTemporario.TamanhoPadraoFonte = 10;
        relatorioTemporario.Titulo = $"Geração de Escalas ({geracaoEscala.DataEscala.ToString("dd/MM/yyyy")})";
        relatorioTemporario.Padrao = true;
        relatorioTemporario.TipoServicoMultisoftware = tipoServicoMultisoftware;
        relatorioTemporario.OcultarDetalhe = false;
        relatorioTemporario.NovaPaginaAposAgrupamento = false;
        relatorioTemporario.RelatorioParaTodosUsuarios = true;
        relatorioTemporario.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();

        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "DescricaoOrigemDestino",
            relatorioTemporario.Colunas.Count));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Origem",
            relatorioTemporario.Colunas.Count, "Origem", Alinhamento.left, 10));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Destino",
            relatorioTemporario.Colunas.Count, "Destino", Alinhamento.left, 10));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Hora",
            relatorioTemporario.Colunas.Count, "Hora", Alinhamento.center, 10));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ModeloVeicularCarga",
            relatorioTemporario.Colunas.Count, "Modelo Veicular", Alinhamento.left, 10));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Empresa",
            relatorioTemporario.Colunas.Count, "Empresa", Alinhamento.left, 10));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Motorista",
            relatorioTemporario.Colunas.Count, "Motorista", Alinhamento.left, 10));
        relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Capacidade",
            relatorioTemporario.Colunas.Count, "Capacidade", Alinhamento.right, 10,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum));

        return relatorioTemporario;
    }

    private Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna ObterRelatorioTemporarioColuna(
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, string propriedade, int posicao)
    {
        return ObterRelatorioTemporarioColuna(relatorio, propriedade, posicao, titulo: "",
            alinhamento: Alinhamento.left, tamanhoColuna: 0,
            tipoSumarizacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum);
    }

    private Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna ObterRelatorioTemporarioColuna(
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, string propriedade, int posicao, string titulo,
        Alinhamento alinhamento, decimal tamanhoColuna)
    {
        return ObterRelatorioTemporarioColuna(relatorio, propriedade, posicao, titulo, alinhamento, tamanhoColuna,
            tipoSumarizacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum);
    }

    private Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna ObterRelatorioTemporarioColuna(
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, string propriedade, int posicao, string titulo,
        Alinhamento alinhamento, decimal tamanhoColuna,
        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao tipoSumarizacao)
    {
        Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna =
            new Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna();

        coluna.Alinhamento = alinhamento;
        coluna.Posicao = posicao;
        coluna.Propriedade = propriedade;
        coluna.Relatorio = relatorio;
        coluna.Tamanho = tamanhoColuna;
        coluna.TipoSumarizacao = tipoSumarizacao;
        coluna.Titulo = titulo;
        coluna.Visivel = tamanhoColuna > 0m;

        return coluna;
    }
}