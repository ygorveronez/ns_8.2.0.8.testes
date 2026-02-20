using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System.Threading;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/AlteracaoFrete")]
    public class AlteracaoFreteController : BaseController
    {
		#region Construtores

		public AlteracaoFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R210_AlteracaoFrete;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Alterações de Frete", "Cargas", "AlteracaoFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                int totalRegistros = repositorio.ContarConsultaRelatorioAlteracaoFrete(filtrosPesquisa, agrupamentos);
                var listaCargas = (totalRegistros > 0) ? await repositorio.ConsultarRelatorioAlteracaoFreteAsync(filtrosPesquisa, agrupamentos, parametrosConsulta, cancellationToken) : new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AlteracaoFrete>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCargas);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(agrupamentos, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioAsync(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario,
            string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var dataSource = await repositorio.ConsultarRelatorioAlteracaoFreteAsync(filtrosPesquisa, agrupamentos, parametrosConsulta, cancellationToken);

                List<Parametro> parametros = await ObterParametrosAsync(filtrosPesquisa, unitOfWork, parametrosConsulta, cancellationToken);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Cargas/AlteracaoFrete", parametros, relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork );

                await unitOfWork.DisposeAsync();
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeiculo"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                SituacaoAlteracaoFrete = Request.GetNullableEnumParam<SituacaoAlteracaoFreteCarga>("SituacaoAlteracaoFrete"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacoes"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador")
            };

            filtrosPesquisa.CodigosFilial = Request.GetNullableListParam<int>("Filial") ?? await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken);
            filtrosPesquisa.CodigosTipoOperacao = Request.GetNullableListParam<int>("TipoOperacao") ?? await ObterListaCodigoTipoOperacaoPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken);

            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            if (codigoTipoCarga > 0)
                filtrosPesquisa.CodigoTipoCarga = codigoTipoCarga;
            else
                filtrosPesquisa.CodigosTipoCarga = await ObterListaCodigoTipoCargaPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;
            else
                filtrosPesquisa.CodigosTransportador = Request.GetListParam<int>("Transportador");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador = await repConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();
            filtrosPesquisa.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga = _configuracaoEmbarcador?.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga ?? false;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            string transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial" : "Transportador";

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Outros Número da Carga", "CodigosAgrupados", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data da Carga", "DataCarregamentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilialFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            }

            grid.AdicionarCabecalho("CNPJ " + transportador, "CNPJTransportadorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(transportador, "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeiculo", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Veiculos", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Operador da Carga", "OperadorCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rota", "Rota", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Negociação", "Negociacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Tabela", "ValorTabela", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("% Diferença", "PercentualDiferenca", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Alteração Frete", "SituacaoAlteracaoFreteFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor Acréscimo", "ValorAcrescimo", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Usuário Aprovação", "UsuarioAprovacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Solicitação Frete", "MotivoSolicitacaoFrete", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Status Pagamento", "StatusTituloCTe", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Diferença valor pago x tabela", "DiferencaValorPagoTabela", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametrosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioAlteracaoFrete filtrosPesquisa,
            Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork, cancellationToken);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? await repVeiculo.BuscarPorCodigoAsync(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = filtrosPesquisa.CodigoModeloVeicularCarga > 0 ? await repModeloVeiculo.BuscarPorCodigoAsync(filtrosPesquisa.CodigoModeloVeicularCarga, cancellationToken) : null;
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = filtrosPesquisa.CodigoTipoCarga > 0 ? await repTipoCarga.BuscarPorCodigoAsync(filtrosPesquisa.CodigoTipoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? await repEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Usuario operador = filtrosPesquisa.CodigoOperador > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoOperador) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("SituacaoAlteracaoFrete", filtrosPesquisa.SituacaoAlteracaoFrete?.ObterDescricao()));
            parametros.Add(new Parametro("SituacaoCarga", string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("Transportador", empresa != null ? empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial : null));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("TipoCarga", tipoCarga?.Descricao));
            parametros.Add(new Parametro("ModeloVeiculo", modeloVeiculo?.Descricao));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("Operador", operador?.Nome));
            parametros.Add(new Parametro("CodigoCargaEmbarcador", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "SituacaoCargaFormatada")
                return "SituacaoCarga";
            else if (propriedadeOrdenar == "DataCarregamentoFormatada")
                return "DataCarregamento";
            else if (propriedadeOrdenar == "CNPJFilialFormatado")
                return "CNPJFilial";
            else if (propriedadeOrdenar == "CNPJTransportadorFormatado")
                return "CNPJTransportador";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
