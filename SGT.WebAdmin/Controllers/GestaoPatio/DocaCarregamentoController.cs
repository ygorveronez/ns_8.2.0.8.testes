using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/DocaCarregamento", "GestaoPatio/FluxoPatio")]
    public class DocaCarregamentoController : BaseController
    {
        #region Construtores

        public DocaCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Servicos.Embarcador.GestaoPatio.InformarDoca servicoInformarDoca = new Servicos.Embarcador.GestaoPatio.InformarDoca(unitOfWork, Auditado);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaAvancarEtapa informarDocaAvancarEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaAvancarEtapa()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    CodigoLocalCarregamento = Request.GetIntParam("LocalCarregamento"),
                    NumeroDoca = Request.GetStringParam("NumeroDoca"),
                    PossuiLaudo = Request.GetBoolParam("PossuiLaudo"),
                    EtapaAntecipada = Request.GetBoolParam("EtapaAntecipada"),
                };

                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = await repositorioDocaCarregamento.BuscarPorCodigoAsync(informarDocaAvancarEtapa.Codigo, false);

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = docaCarregamento.FluxoGestaoPatio;

                TipoIntegracao tipoIntegracaoTipo = await repositorioFluxoGestaoPatio.BuscarTipoIntegracaoConfiguradoSequenciaGestaoPatioPorFluxoAsync(fluxoGestaoPatio.Codigo, fluxoGestaoPatio.Tipo, cancellationToken);

                if (tipoIntegracaoTipo != TipoIntegracao.NaoInformada && tipoIntegracaoTipo != TipoIntegracao.NaoPossuiIntegracao && tipoIntegracaoTipo != 0)
                {
                    Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(tipoIntegracaoTipo);

                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioIntegracao servicoFluxoPatioIntegracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioIntegracao(unitOfWork);

                    Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repositorioPedido.BuscarPedidoComGrupoPessoasMaiorPrioridadeAsync((from p in fluxoGestaoPatio.Carga.Pedidos select p.Pedido.Codigo).ToList(), cancellationToken);

                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracao = await servicoFluxoPatioIntegracao.AdicionarIntegracaoCargaAsync(fluxoGestaoPatio.Carga, fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual, tipoIntegracao, pedido);
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, integracao, null, $"Registro criado devido a confirmação da doca {informarDocaAvancarEtapa.NumeroDoca}", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert, cancellationToken);

                }

                servicoInformarDoca.Avancar(informarDocaAvancarEtapa);

                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

                if (configuracaoIntegracao?.PossuiIntegracaoGPA ?? false)
                {
                    if (docaCarregamento.CargaBase.IsCarga() && !string.IsNullOrWhiteSpace(docaCarregamento.NumeroDoca))
                    {
                        string mensagemErro = string.Empty;
                        Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.IntegrarEncostaVeiculo(docaCarregamento.Carga, ref mensagemErro, unitOfWork);
                    }
                }

                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocaCarregamento = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = null;

                if (codigoDocaCarregamento > 0)
                    docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(codigoDocaCarregamento);
                else if (codigoFluxoGestaoPatio > 0)
                    docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (docaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (docaCarregamento.Carga != null)
                    return new JsonpResult(ObterDocaCarregamentoPorCarga(unitOfWork, docaCarregamento));

                return new JsonpResult(ObterDocaCarregamentoPorPreCarga(unitOfWork, docaCarregamento));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Numero Doca", "NumeroDoca", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Informação", "DataInformacaoDoca", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);

                string carga = Request.GetStringParam("Carga");
                string preCarga = Request.GetStringParam("PreCarga");
                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocaCarregamento? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocaCarregamento>("Situacao");
                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                int totalRegistros = repositorioDocaCarregamento.ContarConsulta(situacao, dataInicial, dataFinal, carga, preCarga);
                List<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento> listaDocaCarregamento = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;

                if (totalRegistros > 0)
                {
                    listaDocaCarregamento = repositorioDocaCarregamento.Consultar(situacao, dataInicial, dataFinal, carga, preCarga, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    List<int> codigosCargas = (from o in listaDocaCarregamento where o.Carga != null select o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                }
                else
                {
                    listaDocaCarregamento = new List<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                }

                var listaDocaCarregamentoRetornar = (
                    from docaCarregamento in listaDocaCarregamento
                    select ObterDocaCarregamento(docaCarregamento, listaCargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(listaDocaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocaCarregamento = Request.GetIntParam("DocaCarregamento");
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(codigoDocaCarregamento);

                if (docaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (docaCarregamento.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (
                    (docaCarregamento.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca) &&
                    !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteRetornarEtapaSaidaCD)
                )
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(docaCarregamento.FluxoGestaoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o fluxo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracaoDoca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocaCarregamento = Request.GetIntParam("DocaCarregamento");
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(codigoDocaCarregamento);

                if (docaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (docaCarregamento.Carga == null)
                    return new JsonpResult(false, true, "Não é possível reenviar a integração de doca de carregamento para pré carga.");

                string mensagemErro = string.Empty;

                Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.IntegrarEncostaVeiculo(docaCarregamento.Carga, ref mensagemErro, unitOfWork);

                if (string.IsNullOrWhiteSpace(mensagemErro))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, mensagemErro);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração de doca de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocaCarregamento = Request.GetIntParam("DocaCarregamento");
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(codigoDocaCarregamento);

                if (docaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(docaCarregamento.FluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca);
                servicoFluxoGestaoPatio.Auditar(docaCarregamento.FluxoGestaoPatio, "Rejeitou o fluxo.");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocaCarregamento = Request.GetIntParam("DocaCarregamento");
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(codigoDocaCarregamento);

                if (docaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(docaCarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InformarDoca, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDocaCarregamentoFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(unitOfWork);

                string numeroDoca = Request.GetStringParam("NumeroDoca");
                int codigoDocaCarregamento = Request.GetIntParam("Codigo");
                bool etapaAntecipada = Request.GetBoolParam("EtapaAntecipada");

                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(codigoDocaCarregamento);

                if (docaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a Doca de Carregamento.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(docaCarregamento.Carga.Codigo);

                docaCarregamento.NumeroDoca = numeroDoca;

                repositorioDocaCarregamento.Atualizar(docaCarregamento);

                carga.NumeroDoca = numeroDoca;

                repositorioCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Integracao.Eship.IntegracaoEship serEShip = new Servicos.Embarcador.Integracao.Eship.IntegracaoEship(unitOfWork);
                serEShip.VerificarIntegracaoEShip(carga);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDocaCarregamento(Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            int codigoCargaFiltrarJanelaCarregamento = docaCarregamento.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                docaCarregamento.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(docaCarregamento.Carga, configuracaoEmbarcador),
                CodigosAgrupadosCarga = docaCarregamento.Carga == null ? "" : string.Join(", ", docaCarregamento.Carga.CodigosAgrupados),
                docaCarregamento.NumeroDoca,
                DataInformacaoDoca = docaCarregamento.DataInformacaoDoca?.ToString("dd/MM/yyyy") ?? "",
                Situacao = docaCarregamento.DescricaoSituacao,
                Doca = docaCarregamento.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Veiculo = docaCarregamento.Carga?.RetornarPlacas,
                Transportador = docaCarregamento.Carga?.Empresa?.Descricao ?? string.Empty,
                ModeloVeiculo = docaCarregamento.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                TipoOperacao = docaCarregamento.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? string.Empty
            };
        }

        private dynamic ObterDocaCarregamentoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(docaCarregamento.Carga.Codigo);

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = (docaCarregamento.FluxoGestaoPatio.Filial != null) ? repositorioCentroCarregamento.BuscarPorFilial(docaCarregamento.FluxoGestaoPatio.Filial.Codigo) : null;

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(docaCarregamento.FluxoGestaoPatio);

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork).BuscarConfiguracao();

            bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
            bool permitirInformarDocaCarregamento = IsPermitirInformarDocaCarregamento(docaCarregamento.FluxoGestaoPatio);
            bool permitirAnteciparEtapa = IsPermitirIAnteciparDocaCarregamento(docaCarregamento.FluxoGestaoPatio, configuracaoGestaoPatio);
            bool exibirBotaoIntegrarDoca = configuracaoIntegracao?.PossuiIntegracaoGPA ?? false;
            string numeroDocaCarga;

            if (!string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDoca) && !string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDocaEncosta) && docaCarregamento.Carga.NumeroDoca != docaCarregamento.Carga.NumeroDocaEncosta)
                numeroDocaCarga = string.Concat(docaCarregamento.Carga.NumeroDoca, " / ", docaCarregamento.Carga.NumeroDocaEncosta);
            else if (!string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDoca) && !string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDocaEncosta) && docaCarregamento.Carga.NumeroDoca == docaCarregamento.Carga.NumeroDocaEncosta)
                numeroDocaCarga = docaCarregamento.Carga.NumeroDoca;
            else if (string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDoca) && !string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDocaEncosta))
                numeroDocaCarga = docaCarregamento.Carga.NumeroDocaEncosta;
            else
                numeroDocaCarga = docaCarregamento.Carga.NumeroDoca;

            var docaCarregamentoRetornar = new
            {
                docaCarregamento.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(docaCarregamento.Carga, configuracaoEmbarcador),
                PreCarga = docaCarregamento.PreCarga?.NumeroPreCarga ?? "",
                CodigoCarga = docaCarregamento.Carga.Codigo,
                NumeroDocaCarga = numeroDocaCarga,
                NumeroCarga = servicoCarga.ObterNumeroCarga(docaCarregamento.Carga, configuracaoEmbarcador),
                NumeroPreCarga = docaCarregamento.PreCarga?.NumeroPreCarga ?? "",
                Situacao = docaCarregamento.DescricaoSituacao,
                PrevisaoCarregamento = docaCarregamento.FluxoGestaoPatio?.DataDocaInformadaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                LocalCarregamento = new { Codigo = docaCarregamento.LocalCarregamento?.Codigo ?? 0, Descricao = docaCarregamento.LocalCarregamento?.Descricao ?? "" },
                NumeroDoca = (permitirInformarDocaCarregamento || permitirAnteciparEtapa) && string.IsNullOrWhiteSpace(docaCarregamento.NumeroDoca) ? docaCarregamento.Carga.NumeroDoca : docaCarregamento.NumeroDoca,
                DataInformacaoDoca = docaCarregamento.DataInformacaoDoca?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Transportador = docaCarregamento.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = docaCarregamento.Carga.RetornarPlacas,
                Motorista = docaCarregamento.Carga.Motoristas.FirstOrDefault()?.Nome ?? string.Empty,
                MotoristaTelefone = docaCarregamento.Carga.Motoristas.FirstOrDefault()?.Telefone ?? string.Empty,
                PodeVoltarEtapa = permitirInformarDocaCarregamento,
                PodeConfirmarDoca = (permitirInformarDocaCarregamento || permitirAnteciparEtapa),
                docaCarregamento.Carga.DescricaoSituacaoCarga,
                DataCarregamento = docaCarregamento.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy HH:{(exibirDataCarregamentoExato ? "mm" : "00")}") ?? string.Empty,
                Remetente = docaCarregamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = docaCarregamento.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = docaCarregamento.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = docaCarregamento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = docaCarregamento.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                docaCarregamento.PossuiLaudo,
                ExibirBotaoIntegrarDoca = exibirBotaoIntegrarDoca,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                CentroCarregamento = new { Codigo = centroCarregamento?.Codigo ?? 0, Descricao = centroCarregamento?.Descricao ?? "" },
                PermiteInformarDadosLaudo = sequenciaGestaoPatio?.InformarDocaCarregamentoPermiteInformarDadosLaudo ?? false
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterDocaCarregamentoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(docaCarregamento.PreCarga.Codigo);

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = (docaCarregamento.FluxoGestaoPatio.Filial != null) ? repositorioCentroCarregamento.BuscarPorFilial(docaCarregamento.FluxoGestaoPatio.Filial.Codigo) : null;

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(docaCarregamento.FluxoGestaoPatio);

            bool exibirDataCarregamentoExato = sequenciaGestaoPatio?.GuaritaEntradaExibirHorarioExato ?? false;
            bool permitirInformarDocaCarregamento = IsPermitirInformarDocaCarregamento(docaCarregamento.FluxoGestaoPatio);
            bool exibirBotaoIntegrarDoca = configuracaoIntegracao?.PossuiIntegracaoGPA ?? false;

            var docaCarregamentoRetornar = new
            {
                docaCarregamento.Codigo,
                Carga = "",
                PreCarga = docaCarregamento.PreCarga.NumeroPreCarga ?? "",
                CodigoCarga = 0,
                NumeroDocaCarga = docaCarregamento.PreCarga.LocalCarregamento?.DescricaoAcao ?? "",
                NumeroCarga = "",
                NumeroPreCarga = docaCarregamento.PreCarga.NumeroPreCarga ?? "",
                Situacao = docaCarregamento.DescricaoSituacao,
                PrevisaoCarregamento = docaCarregamento.FluxoGestaoPatio?.DataDocaInformadaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                LocalCarregamento = new { Codigo = docaCarregamento.LocalCarregamento?.Codigo ?? 0, Descricao = docaCarregamento.LocalCarregamento?.Descricao ?? "" },
                NumeroDoca = docaCarregamento.NumeroDoca,
                DataInformacaoDoca = docaCarregamento.DataInformacaoDoca?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Transportador = docaCarregamento.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = docaCarregamento.PreCarga.RetornarPlacas,
                Motorista = docaCarregamento.PreCarga.Motoristas.FirstOrDefault()?.Nome ?? string.Empty,
                MotoristaTelefone = docaCarregamento.PreCarga.Motoristas.FirstOrDefault()?.Telefone ?? string.Empty,
                PodeVoltarEtapa = permitirInformarDocaCarregamento,
                PodeConfirmarDoca = permitirInformarDocaCarregamento,
                DescricaoSituacaoCarga = "",
                DataCarregamento = cargaJanelaCarregamento?.InicioCarregamento.ToString($"dd/MM/yyyy HH:{(exibirDataCarregamentoExato ? "mm" : "00")}") ?? string.Empty,
                Remetente = docaCarregamento.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = docaCarregamento.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = docaCarregamento.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = docaCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = docaCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                docaCarregamento.PossuiLaudo,
                ExibirBotaoIntegrarDoca = exibirBotaoIntegrarDoca,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                CentroCarregamento = new { Codigo = centroCarregamento?.Codigo ?? 0, Descricao = centroCarregamento?.Descricao ?? "" },
                PermiteInformarDadosLaudo = sequenciaGestaoPatio?.InformarDocaCarregamentoPermiteInformarDadosLaudo ?? false
            };

            return docaCarregamentoRetornar;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            return propriedadeOrdenar;
        }

        private bool IsPermitirInformarDocaCarregamento(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca);
        }

        private bool IsPermitirIAnteciparDocaCarregamento(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca && (configuracaoGestaoPatio?.InformarDocaPermiteAntecipar ?? false));
        }

        #endregion
    }
}
