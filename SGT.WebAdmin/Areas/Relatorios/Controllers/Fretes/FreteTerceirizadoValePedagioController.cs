using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/FreteTerceirizadoValePedagio")]
    public class FreteTerceirizadoValePedagioController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio>
    {
		#region Construtores

		public FreteTerceirizadoValePedagioController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos Privados

		private Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R285_FreteTerceirizadoValePedagio;

        private readonly decimal TamanhoColunaPequena = 1.75m;
        private readonly decimal TamanhoColunaGrande = 5.50m;
        private readonly decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Fretes Terceirizados com Vale Pedágios", "Fretes", "FreteTerceirizadoValePedagio.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "ContratoFrete", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoValePedagio servicoRelatorioFreteTerceirizadoValePedagio = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoValePedagio(unitOfWork, TipoServicoMultisoftware, Cliente);
                var listaFrete = await servicoRelatorioFreteTerceirizadoValePedagio.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);
                grid.setarQuantidadeTotal(listaFrete.Count);
                grid.AdicionaRows(listaFrete);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio()
            {
                CpfCnpjTerceiros = Request.GetListParam<double>("Terceiro"),
                DataEmissaoContratoInicial = Request.GetDateTimeParam("DataEmissaoContratoInicial"),
                DataEmissaoContratoFinal = Request.GetDateTimeParam("DataEmissaoContratoFinal"),
                DataAprovacaoInicial = Request.GetDateTimeParam("DataAprovacaoInicial"),
                DataAprovacaoFinal = Request.GetDateTimeParam("DataAprovacaoFinal"),
                DataEncerramentoInicial = Request.GetDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoFinal = Request.GetDateTimeParam("DataEncerramentoFinal"),
                DataEncerramentoCIOTInicial = Request.GetDateTimeParam("DataEncerramentoCIOTInicial"),
                DataEncerramentoCIOTFinal = Request.GetDateTimeParam("DataEncerramentoCIOTFinal"),
                DataAberturaCIOTInicial = Request.GetDateTimeParam("DataAberturaCIOTInicial"),
                DataAberturaCIOTFinal = Request.GetDateTimeParam("DataAberturaCIOTFinal"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                Situacao = Request.GetListEnumParam<SituacaoContratoFrete>("Situacao")
            };
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Contrato de Frete", "ContratoFrete", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CT-es", "NumeroCTes", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);

            grid.AdicionarCabecalho("CPF/CNPJ Terceiro", "CPFCNPJTerceiroFormatado", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Terceiro", "Terceiro", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Dt. Nasc. Terceiro", "DataNascimentoTerceiroFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("PIS/PASEP Terceiro", "PISPASEPTerceiro", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Vl. Pago", "ValorPago", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vl. INSS", "ValorINSS", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vl. SEST", "ValorSEST", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vl. SENAT", "ValorSENAT", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Vl. IRRF", "ValorIRRF", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Número Vale Pedágio", "NumeroValePedagio", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Vale Pedágio", "TipoIntegracaoFormatada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Vale Pedágio", "ValorValePedagio", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Transportador Vale Pedágio", "TransportadorTerceiro", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Transportador", "NomeTransportador", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);


            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioFreteTerceirizadoValePedagio> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}