using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/PreCarga")]
    public class PreCargaController : BaseController
    {
		#region Construtores

		public PreCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R223_PreCarga;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pré Planejamentos", "Cargas", "PreCarga.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroPreCarga", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork), relatorio);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.PreCargas.PreCarga repositorio = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork,cancellationToken);
                int totalRegistros = await repositorio.ContarConsultaRelatorioPreCargaAsync(filtrosPesquisa, agrupamentos, cancellationToken);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga> listaCargas = (totalRegistros > 0)
                    ? await repositorio.ConsultarRelatorioPreCargaAsync(filtrosPesquisa, agrupamentos, parametrosConsulta) :
                    new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga>();

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
            finally
            {
                await unitOfWork.DisposeAsync();
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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Repositorio.Embarcador.PreCargas.PreCarga repositorio = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga> dataSource = await repositorio.ConsultarRelatorioPreCargaAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                List<Parametro> parametros = await ObterParametrosAsync(filtrosPesquisa, unitOfWork, parametrosConsulta, cancellationToken);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Cargas/PreCarga", parametros, relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork);
            }
            catch (Exception excecao)
            {
                await servicoRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, excecao, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();

            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataCriacaoPreCargaInicial = Request.GetDateTimeParam("DataCriacaoPreCargaInicial"),
                DataCriacaoPreCargaFinal = Request.GetDateTimeParam("DataCriacaoPreCargaFinal"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoConfiguracaoProgramacaoCarga = Request.GetIntParam("ConfiguracaoProgramacaoCarga"),
                CodigoOperador = Request.GetIntParam("Operador"),
                PreCarga = Request.GetStringParam("PreCarga"),
                Pedido = Request.GetStringParam("Pedido"),
                Carga = Request.GetStringParam("Carga"),
                Situacao = Request.GetEnumParam<FiltroPreCarga>("Situacao"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                SomenteProgramacaoCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga
            };
        }

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();
            string transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial" : "Transportador";

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Pré Planejamento", "NumeroPreCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Doca Carregamento", "DocaCarregamento", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Previsão Início Viagem", "DataPrevisaoInicioViagemFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Previsão Chegada Doca", "PrevisaoChegadaDocaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Criação", "DataCriacaoPreCargaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("CNPJ " + transportador, "CNPJTransportadorFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(transportador, "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Veículo", "Veiculos", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motorista", "Motoristas", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeiculo", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Faixa Temperatura", "FaixaTemperatura", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº Pedido", "NumeroPedidos", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetente", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CNPJDestinatario", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Peso", "Peso", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Operador", "Operador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Previsão Chegada Destinatário", "PrevisaoChegadaDestinatarioFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Previsão Saída Destinatário", "PrevisaoSaidaDestinatarioFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                grid.AdicionarCabecalho("Rota Programada", "RotaProgramada", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametrosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa, Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? await repFilial.BuscarPorCodigoAsync(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CpfCnpjRemetente > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CpfCnpjRemetente) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0 ? await repCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Usuario operador = filtrosPesquisa.CodigoOperador > 0 ? await repUsuario.BuscarPorCodigoAsync(filtrosPesquisa.CodigoOperador, false) : null;
            Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga configuracaoProgramacaoCarga = filtrosPesquisa.CodigoConfiguracaoProgramacaoCarga > 0 ?
                await repositorioConfiguracaoProgramacaoCarga.BuscarPorCodigoAsync(filtrosPesquisa.CodigoConfiguracaoProgramacaoCarga, auditavel: false) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DataCriacaoPreCarga", filtrosPesquisa.DataCriacaoPreCargaInicial, filtrosPesquisa.DataCriacaoPreCargaFinal));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao.ObterDescricao()));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Parametro("Destinatario", destinatario?.Descricao));
            parametros.Add(new Parametro("Operador", operador?.Descricao));
            parametros.Add(new Parametro("PreCarga", filtrosPesquisa.PreCarga));
            parametros.Add(new Parametro("Pedido", filtrosPesquisa.Pedido));
            parametros.Add(new Parametro("Carga", filtrosPesquisa.Carga));
            parametros.Add(new Parametro("RotaProgramada", configuracaoProgramacaoCarga?.Descricao));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataPrevisaoInicioViagemFormatada")
                return "DataPrevisaoInicioViagem";
            if (propriedadeOrdenar == "PrevisaoChegadaDocaFormatada")
                return "PrevisaoChegadaDoca";
            if (propriedadeOrdenar == "DataCriacaoPreCargaFormatada")
                return "DataCriacaoPreCarga";
            if (propriedadeOrdenar == "PrevisaoChegadaDestinatarioFormatada")
                return "PrevisaoChegadaDestinatario";
            if (propriedadeOrdenar == "PrevisaoSaidaDestinatarioFormatada")
                return "PrevisaoSaidaDestinatario";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
