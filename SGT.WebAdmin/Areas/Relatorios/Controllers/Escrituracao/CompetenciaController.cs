using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Escrituracao
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Escrituracao/Competencia")]
    public class CompetenciaController : BaseController
    {
		#region Construtores

		public CompetenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R160_Competencia;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Competência", "Escrituracao", "Competencia.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Transportador", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Escrituracao.Competencia servicoCompetencia = new Servicos.Embarcador.Relatorios.Escrituracao.Competencia(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoCompetencia.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteCompetencia> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, propriedades, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("DataEmissaoFormatada").Nome("Data Emissão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Filial").Nome("Filial").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("Transportador").Nome("Transportador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("Tomador").Nome("Tomador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("Origem").Nome("Origem").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Destino").Nome("Destino").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Remetente").Nome("Emitente").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Destinatario").Nome("Destinatário").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoCarga").Nome("Tipo Carga").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoOperacao").Nome("Tipo Operação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ModeloDocumento").Nome("Modelo Doc. Fiscal").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ModeloVeicular").Nome("Modelo Veicular").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PlacaFormatada").Nome("Placa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Carga").Nome("Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Agr(true);
            //grid.Prop("DataEmissaoCargaFormatada").Nome("Data Emissão Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Ocorrencia").Nome("Ocorrência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("DataEmissaoOcorrenciaFormatada").Nome("Data Emissão Ocorrência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Pagamento").Nome("Pagamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Agr(true);
            grid.Prop("DataPagamentoFormatada").Nome("Data Pagamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Provisao").Nome("Provisão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Agr(true);
            grid.Prop("CancelamentoProvisao").Nome("Cancelamento Provisão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("NumeroCte").Nome("Número CT-e").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("NumeroNFS").Nome("Número Doc").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.right);
            grid.Prop("Rota").Nome("Rota").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("PesoBruto").Nome("Peso Bruto").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("DataEmissaoNFsManualFormatada").Nome("Data Emissão Doc").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("ValorFrete").Nome("Valor Frete").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorFreteSemICMS").Nome("Valor Frete Sem ICMS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("Aliquota").Nome("Aliquota").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("AliquotaISS").Nome("Aliquota ISS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("CST").Nome("CST").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("ICMS").Nome("ICMS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ICMSRetido").Nome("ICMS Retido").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorISSRetido").Nome("Valor ISS Retido").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorISS").Nome("Valor ISS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorPIS").Nome("Valor PIS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorCOFINS").Nome("Valor COFINS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("DataAprovacaoPagamentoFormatada").Nome("Data Aprovação Pagamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataDigitalizacaoCanhotoFormatada").Nome("Data Digitalização Canhoto").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("NumeroValePedagio").Nome("Número Vale Pedágio").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("NumeroNotaFiscalServico").Nome("Número Nota fiscal de serviço").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("CPFCNPJRemetenteFormatado").Nome("CPF/CNPJ Remetente").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataEmissaoNotaFiscalServicoManualFormatada").Nome("Data Emissão NFs Manual").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("IDAgrupador").Nome("ID Agrupador").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("NumeroOcorrencia").Nome("Número Ocorrência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CnpjCpfTomador = Request.GetDoubleParam("Tomador"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataCargaInicial = Request.GetDateTimeParam("DataCargaInicial"),
                DataCargaFinal = Request.GetDateTimeParam("DataCargaFinal"),
                DataEmissaoCTeInicial = Request.GetDateTimeParam("DataEmissaoCTeInicial"),
                DataEmissaoCTeFinal = Request.GetDateTimeParam("DataEmissaoCTeFinal"),
                DataEmissaoNotaInicial = Request.GetDateTimeParam("DataEmissaoNotaInicial"),
                DataEmissaoNotaFinal = Request.GetDateTimeParam("DataEmissaoNotaFinal"),
                VisualizarTambemDocumentosAguardandoProvisao = Request.GetBoolParam("VisualizarTambemDocumentosAguardandoProvisao"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroValePedagio = Request.GetStringParam("NumeroValePedagio"),
            };
        }

        #endregion
    }
}
