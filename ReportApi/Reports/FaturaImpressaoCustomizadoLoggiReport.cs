using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.FaturaChaveCTe)]
public class FaturaImpressaoChaveCTeReport : ReportBase
{
    public FaturaImpressaoChaveCTeReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = extraData
            .GetValue<string>("parametros").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>>();

        int codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio relatorioTemporario = extraData.GetValue<string>("relatorioTemporario").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        int codigofatura = extraData.GetValue<int>("codigofatura");
        Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigofatura);

        Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoFatura> listaRelatorioPadraoFatura = repositorioFatura.BuscarRelatorioPadrao(fatura);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoFaturaDados> listaRelatorioPadraoFaturaDados = repositorioFatura.BuscarRelatorioPadraoDados(fatura);
        List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoProdutos> listaRelatorioPadraoProdutos = new List<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoProdutos>();

        Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCampos = null;

        if (extraData.GetInfo().TipoServico ==
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
        {
            relatorioTemporario.ExibirSumarios = true;

            identificacaoCampos = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT
            {
                ContadorRegistrosGrupo = "",
                ContadorRegistrosTotal = ""
            };
        }

        if (!string.IsNullOrWhiteSpace(listaRelatorioPadraoFatura[0].Produtos))
        {
            for (int i = 0; i < listaRelatorioPadraoFatura.Count; i++)
                listaRelatorioPadraoFatura[i].PossuiProdutos = true;

            string[] produtos = listaRelatorioPadraoFatura[0].Produtos.Split(',');
            string[] quantidades = listaRelatorioPadraoFatura[0].Quantidades.Split(',');

            for (int i = 0; i < produtos.Length; i++)
            {
                listaRelatorioPadraoProdutos.Add(
                    new Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoProdutos
                    {
                        Produto = produtos[i].Trim(),
                        Quantidade = quantidades[i].ToInt()
                    });
            }
        }

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = _servicoRelatorioReportService.CriarRelatorio(
            relatorioControleGeracao, relatorioTemporario, listaRelatorioPadraoFatura, _unitOfWork, identificacaoCampos,
            new List<KeyValuePair<string, dynamic>>()
            {
                new KeyValuePair<string, dynamic>("FaturaPadrao_Dados", listaRelatorioPadraoFaturaDados),
                new KeyValuePair<string, dynamic>("FaturaPadrao_Produtos", listaRelatorioPadraoProdutos)
            }
        );

        _servicoRelatorioReportService.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao,
            relatorioTemporario, parametros, "", "");
        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Faturas/Fatura",
            _unitOfWork);

        return PrepareReportResult(FileType.PDF);
    }
}