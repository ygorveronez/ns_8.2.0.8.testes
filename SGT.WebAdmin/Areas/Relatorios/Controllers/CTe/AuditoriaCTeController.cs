using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/AuditoriaCTe")]
    public class AuditoriaCTeController : BaseController
    {
		#region Construtores

		public AuditoriaCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R274_AuditoriaCTe;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Auditoria de CT-e", "CTe", "AuditoriaCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCTe", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.AuditoriaCTe servicoRelatorioAuditoriaCTe = new Servicos.Embarcador.Relatorios.CTes.AuditoriaCTe(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAuditoriaCTe.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.AuditoriaCTe> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Município da cobrança", "MunicipioCobranca", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data de emissão", "DataEmissaoCTeFormatada", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Mês", "MesEmissaoCTe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Ano", "AnoEmissaoCTe", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("N° Romaneio", "NumeroRomaneio", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Km Rota", "KilometragemRota", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("N° conhec.", "NumeroCTe", TamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Chave conhec.", "ChaveCTe", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Total do conhec.", "TotalReceberCTe", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor Total Mercadoria", "ValorTotalMercadoria", TamanhoColunaPequena, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Notas fiscais", "NotasFiscais", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Frete unitário", "FreteUnitario", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso carregado", "PesoCarregado", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso frete", "PesoFrete", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de frete", "TabelaFreteCarga", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor frete", "ValorFrete", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Pedágio", "ValorPedagio", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor PIS", "ValorPIS", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor COFINS", "ValorCOFINS", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Peso", "Peso", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Mercadoria", "Mercadoria", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Mercadoria grupo", "MercadoriaGrupo", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Espécie Descrição do frete", "EspecieDescricaoFrete", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Placa tração", "PlacaTracao", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Modalidade da placa de tração", "ModalidadePlacaTracao", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Operação", "Operacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Razão social Remetente", "RazaoSocialRemetente", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Município Remetente", "MunicipioRemetente", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF (remetente)", "UFRemetente", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Razão Social (coleta)", "RazaoSocialColeta", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF da coleta", "UFColeta", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Razão Social (redespacho)", "RazaoSocialRedespacho", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Município (redespacho)", "MunicipioRedespacho", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF (redespacho)", "UFRedespacho", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Razão social do destinatário", "RazaoSocialDestinatario", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Município do destinatário", "MunicipioDestinatario", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UF (destinatário)", "UFDestinatario", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo de conhecimento", "TipoConhecimento", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Inserido por", "InseridoPor", TamanhoColunaMedia, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                NumeroDocumentoInicial = Request.GetIntParam("NumeroDocumentoInicial"),
                NumeroDocumentoFinal = Request.GetIntParam("NumeroDocumentoFinal"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                modeloDocumento = Request.GetIntParam("ModeloDocumento"),
                grupoPessoas = Request.GetIntParam("GrupoPessoas"),
                statusCTe = Request.GetListParam<string>("Situacao"),
                codigoFilial = Request.GetIntParam("Filial"),
                codigoTransportador = Request.GetIntParam("Empresa"),
            };
        }

        #endregion
    }
}
