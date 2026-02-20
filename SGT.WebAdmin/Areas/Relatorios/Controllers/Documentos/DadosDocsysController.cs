using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Documentos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Documentos/DadosDocsys")]
    public class DadosDocsysController : BaseController
    {
		#region Construtores

		public DadosDocsysController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R236_DadosDocsys;
        
        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Dados do Docsys", "Documentos", "DadosDocsys.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Viagem", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Documentos.DadosDocsys servicoRelatorioDadosDocsys = new Servicos.Embarcador.Relatorios.Documentos.DadosDocsys(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioDadosDocsys.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Documentos.DadosDocsys> listaDadosDocsys, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaDadosDocsys);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroDadosDocsys()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                PedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio")
            };
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Viagem").Nome("VIAGEM").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("PortoOrigem").Nome("POL").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("PortoDestino").Nome("POD").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("TrackingStatus").Nome("Tracking Status").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("TrackingDataFormatado").Nome("Tracking Data").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Booking").Nome("BOOKING").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("NumeroControle").Nome("NRO_CTE").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("TipoCTeFormatado").Nome("TIPO_CTE").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("ValorReceber").Nome("VALOR A RECEBER").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("ValorPrestacao").Nome("VALOR DA PRESTAÇÃO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("PrevisaoSaidaFormatado").Nome("PREVISAO_SAIDA DO PORTO DE ORIGEM").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("EmissaoFormatado").Nome("DATA_EMISSAO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("VoucherNO").Nome("NUM_VOUCHER_DOCSYS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("VoucherDateFormatado").Nome("DAT_VOUCHER_DOCSYS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("DACS_transfFormatado").Nome("DAT_DACS_DOCSYS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("UBLI").Nome("NUM_BLUI_DOCSYS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("DataInclusaoFormatado").Nome("DAT_INCL").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("CorrCode").Nome("COD_COR").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("BLVersion").Nome("NUM_VERSION").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("FlgDocsys").Nome("FLG_DOCSYS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("TipoOperacao").Nome("TIPO_OPER").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("StatusFormatado").Nome("STATUS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("Tomador").Nome("TOMADOR").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("PossuiCartaCorrecaoFormatado").Nome("POSSUI_CCE").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("FoiAnuladoFormatado").Nome("ANULADO").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("PossuiComplementoFormatado").Nome("POSSUI_CTE_COMP").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("FoiSubstituidoFormatado").Nome("SUBSTITUIDO").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("DataEmissaoFaturaFormatado").Nome("DATA DE EMISSÃO DA FATURA").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("FoiFaturadoFormatado").Nome("FATURADO OU COM TÍTULO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("DiaEmissao").Nome("DIA_EMISSAO").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("AnoEmissao").Nome("ANO_EMISSAO").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("CorteEmissao").Nome("DATA CORTE DA EMISSAO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("ViagemBookingCTe").Nome("VVD_BOOKING_CTE").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("StatusDocsys").Nome("Status Docsys").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("CortePrevisaoSaida").Nome("DATA CORTE DA PREVISAO SAÍDA").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("SaidaNavio").Nome("SAÍDA DO NAVIO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("TipoEmissao").Nome("Tipo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("EstadosIguais").Nome("UF Destino = Origem?").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("DuplicadoFormatado").Nome("Duplicado?").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("ConsiderarDesconsiderar").Nome("Considerar?").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("CanceladosAnulados").Nome("Cancelados/Anulados?").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("MesSaidaNavio").Nome("Mês da saída do navio").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("AnoSaidaNavio").Nome("Ano da saída do navio").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Meta").Nome("Meta").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("MesContabil").Nome("Mês Contábil").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);

            return grid;
        }

        #endregion
    }
}
