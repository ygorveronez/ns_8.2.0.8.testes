using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace ReportApi.Reports;

[UseReportType(ReportType.FaturaImpressaoMultimodal)]
public class FaturaImpressaoMultimodalReport : ReportBase
{
    public FaturaImpressaoMultimodalReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }
    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = extraData.GetValue<string>("parametros").FromJson<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        var relatorioTemporario = extraData.GetValue<string>("relatorioTemporario").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura tipoImpressaoFatura = extraData.GetValue<string>("tipoImpressaoFatura").FromJson<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura>();

        int codigofatura = extraData.GetValue<int>("codigofatura");
        Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigofatura);

        Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoFatura> listaRelatorioPadraoFatura =
            repositorioFatura.BuscarRelatorioPadrao(fatura);
        IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.RelatorioPadraoFaturaDados>
            listaRelatorioPadraoFaturaDados = repositorioFatura.BuscarRelatorioPadraoDados(fatura, tipoImpressaoFatura);
        Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCampos = null;

        relatorioTemporario.ExibirSumarios = true;

        identificacaoCampos = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT
        {
            ContadorRegistrosGrupo = "",
            ContadorRegistrosTotal = ""
        };

        if (listaRelatorioPadraoFaturaDados?.FirstOrDefault()?.TipoProposta == 9 ||
            listaRelatorioPadraoFaturaDados?.FirstOrDefault()?.TipoProposta == 10)
        {
            relatorioControleGeracao.Titulo = "Nota Débito";
            relatorioTemporario.Titulo = "Nota Débito";
            relatorioControleGeracao.Relatorio.Titulo = "Nota Débito";
        }
        else if (listaRelatorioPadraoFaturaDados?.FirstOrDefault()?.TipoProposta == 8)
        {
            relatorioControleGeracao.Titulo = "Comercial Invoice";
            relatorioTemporario.Titulo = "Comercial Invoice";
            relatorioControleGeracao.Relatorio.Titulo = "Comercial Invoice";
        }
        else
        {
            relatorioControleGeracao.Titulo = "Fatura Duplicata";
            relatorioTemporario.Titulo = "Fatura Duplicata";
            relatorioControleGeracao.Relatorio.Titulo = "Fatura Duplicata";

            if (fatura.Carga?.TipoOperacao?.ConfiguracaoImpressao?.AlterarLayoutDaFaturaIncluirTipoServico ?? false)
            {
                string descricao = fatura.Carga.TipoOperacao.TipoPropostaMultimodal.ObterDescricao();

                if (!string.IsNullOrEmpty(descricao) && descricao != "Nenhum")
                {
                    if (!relatorioControleGeracao.Titulo.Contains(descricao))
                        relatorioControleGeracao.Titulo += " - " + descricao;

                    if (!relatorioTemporario.Titulo.Contains(descricao))
                        relatorioTemporario.Titulo += " - " + descricao;

                    if (!relatorioControleGeracao.Relatorio.Titulo.Contains(descricao))
                        relatorioControleGeracao.Relatorio.Titulo += " - " + descricao;
                }
            }

        }

        CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = _servicoRelatorioReportService.CriarRelatorio(
            relatorioControleGeracao, relatorioTemporario, listaRelatorioPadraoFatura, _unitOfWork, identificacaoCampos,
            new List<KeyValuePair<string, dynamic>>()
            {
                new KeyValuePair<string, dynamic>("FaturaPadrao_Dados", listaRelatorioPadraoFaturaDados)
            }
        );
        _servicoRelatorioReportService.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao, relatorioTemporario, parametros, "", "");
        _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Faturas/Fatura", _unitOfWork);
        return PrepareReportResult(FileType.PDF);
    }
}