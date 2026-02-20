using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/CancelamentoProvisao")]
    public class CancelamentoProvisaoController : BaseController
    {
		#region Construtores

		public CancelamentoProvisaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);

                Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao svcEstornoAprovacao = new Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao(unitOfWork);

                bool.TryParse(Request.Params("CancelamentoProvisaoContraPartida"), out bool cancelamentoProvisaoContraPartida);

                if (!cancelamentoProvisaoContraPartida)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarSeExistePagamentoEmFechamento();

                    if (pagamento != null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é possível cancelar a provisão enquanto o pagamento " + pagamento.Numero + " estiver em fechamento.");
                    }
                }

                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);

                double.TryParse(Request.Params("Tomador"), out double tomador);

                int.TryParse(Request.Params("Filial"), out int filial);
                int.TryParse(Request.Params("Transportador"), out int empresa);



                int.TryParse(Request.Params("Carga"), out int carga);
                int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);

                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao();

                cancelamentoProvisao.CancelamentoProvisaoContraPartida = cancelamentoProvisaoContraPartida;

                if (dataInicio != DateTime.MinValue)
                    cancelamentoProvisao.DataInicial = dataInicio;

                if (dataFim != DateTime.MinValue)
                    cancelamentoProvisao.DataFinal = dataFim;

                cancelamentoProvisao.DataCriacao = DateTime.Now;
                cancelamentoProvisao.Numero = repProvisao.ObterProximoNumero();

                if (tomador > 0)
                    cancelamentoProvisao.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);

                if (filial > 0)
                    cancelamentoProvisao.Filial = repFilial.BuscarPorCodigo(filial);

                if (empresa > 0)
                    cancelamentoProvisao.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                if (carga > 0)
                    cancelamentoProvisao.Carga = repCarga.BuscarPorCodigo(carga);

                if (ocorrencia > 0)
                    cancelamentoProvisao.CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(ocorrencia);

                //Caso for unilever, a aprovação sera necessaria
                bool tipoUnilever = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever);
                cancelamentoProvisao.Situacao = tipoUnilever ? SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao : SituacaoCancelamentoProvisao.EmCancelamento;

                repProvisao.Inserir(cancelamentoProvisao, Auditado);
                Servicos.Embarcador.Escrituracao.DocumentoProvisao.SetarDocumentosSelecionados(cancelamentoProvisao, carga, ocorrencia, dataInicio, dataFim, tomador, filial, empresa, listaNaoSelecionadas, listaSelecionadas, todosSelecionados, unitOfWork);

                if (tipoUnilever)
                    svcEstornoAprovacao.CriarAprovacao(cancelamentoProvisao, TipoGeracaoRegraProvisao.Indefinida, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    cancelamentoProvisao.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = repProvisao.BuscarPorCodigo(codigo);

                // Valida
                if (cancelamentoProvisao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    cancelamentoProvisao.Codigo,
                    cancelamentoProvisao.Situacao,
                    DataInicial = cancelamentoProvisao.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = cancelamentoProvisao.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    cancelamentoProvisao.Numero,
                    cancelamentoProvisao.GerandoMovimentoFinanceiro,
                    cancelamentoProvisao.MotivoRejeicaoCancelamentoFechamentoProvisao,
                    Filial = cancelamentoProvisao.Filial != null ? new { cancelamentoProvisao.Filial.Codigo, cancelamentoProvisao.Filial.Descricao } : new { Codigo = 0, Descricao = "" },
                    Transportador = new { Codigo = cancelamentoProvisao.Empresa?.Codigo ?? 0, Descricao = cancelamentoProvisao.Empresa?.Descricao ?? "" },
                    Tomador = new { Codigo = cancelamentoProvisao.Tomador?.Codigo ?? 0, Descricao = cancelamentoProvisao.Tomador?.Nome ?? "" },
                    cancelamentoProvisao.DescricaoSituacao,
                    cancelamentoProvisao.CancelamentoProvisaoContraPartida,
                    Carga = new { Codigo = cancelamentoProvisao.Carga?.Codigo ?? 0, Descricao = cancelamentoProvisao.Carga?.Descricao ?? "" },
                    Ocorrencia = new { Codigo = cancelamentoProvisao.CargaOcorrencia?.Codigo ?? 0, Descricao = cancelamentoProvisao.CargaOcorrencia?.Descricao ?? "" },
                    UtilizaAutorizacao = repSolicitacao.ExisteRegraAprovacao(),
                    Resumo = new
                    {
                        cancelamentoProvisao.Codigo,
                        Transportador = cancelamentoProvisao.Empresa?.Descricao ?? string.Empty,
                        Filial = cancelamentoProvisao.Filial?.Descricao ?? string.Empty,
                        Numero = cancelamentoProvisao.Numero.ToString(),
                        cancelamentoProvisao.CancelamentoProvisaoContraPartida,
                        DataInicial = cancelamentoProvisao.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFinal = cancelamentoProvisao.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Situacao = cancelamentoProvisao.DescricaoSituacao,
                        Carga = cancelamentoProvisao.Carga?.Descricao ?? "",
                        Ocorrencia = cancelamentoProvisao.CargaOcorrencia?.Descricao ?? "",
                        QuantidadeDocumentos = cancelamentoProvisao.QuantidadeDocsProvisao.ToString(),
                        ValorCancelamentoProvisao = cancelamentoProvisao.ValorCancelamentoProvisao.ToString("n2"),
                    }
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();
                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisa(configuracaoFatura);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                ObterPropriedadeOrdenar(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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
        public async Task<IActionResult> PesquisaDocumento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDocumento(exportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisa(configuracaoFatura);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                ObterPropriedadeOrdenar(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDocumentos()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaDocumento(exportarPesquisa: true);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);

                int codAjuste = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> listaAutorizacao = repAprovacao.ConsultarAutorizacoes(codAjuste, grid.ObterParametrosConsulta());
                int totalRegistros = repAprovacao.ContarAutorizacoes(codAjuste);

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeAprovacao = obj.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome ?? "",
                                 Regra = obj.RegraAutorizacao?.Descricao ?? string.Empty,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 DT_RowColor = CorAprovacao(obj.Situacao)
                             }).ToList();


                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos(CancellationToken cancelationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork, cancelationToken);
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork, cancelationToken);

                List<int> codigosDeCancelamentosPendentes = repositorioCancelamentoProvisao.BuscarCodigosFalhaIntegracao();

                if (codigosDeCancelamentosPendentes == null || codigosDeCancelamentosPendentes.Count == 0)
                    return new JsonpResult(false, true, "Não há integrações com falha.");

                await unitOfWork.StartAsync(cancelationToken);

                var cancelamentoProvisaoAtualizados = await repoIntegracao.AtualizarSituacaoAsync(
                    codigosDeCancelamentosPendentes, 
                    SituacaoIntegracao.AgIntegracao, 
                    string.Empty
                );

                var integracoesAtualizados = await repositorioCancelamentoProvisao.AtualizarSituacaoAsync(
                    codigosDeCancelamentosPendentes, 
                    SituacaoCancelamentoProvisao.AgIntegracao
                );

                await unitOfWork.CommitChangesAsync(cancelationToken);

                return new JsonpResult(
                    true,
                    $"Cancelmento Provisões atualizadas: {cancelamentoProvisaoAtualizados}. Integrações atualizadas: {integracoesAtualizados}."
                );
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unitOfWork.RollbackAsync(cancelationToken);

                return new JsonpResult(false, $"Erro ao reprocessar as integrações do cancelamento de provisão: {excecao.Message}");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReenviarAprovacaoCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao repAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente.AprovacaoAlcadaEstornoProvisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao existeCancelamentoProvisao = repositorioCancelamentoProvisao.BuscarPorCodigo(codigo, false);
                if (existeCancelamentoProvisao == null)
                    return new JsonpResult(false, "Cancelamento Não encontrado para enviar");

                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> listaAutorizacao = repAprovacao.BuscarPorEstornoProvisaoLista(codigo, this.Usuario.Codigo);

                if (listaAutorizacao.Count == 0 && listaAutorizacao.Any(x => x.Situacao != SituacaoAlcadaRegra.Rejeitada))
                    return new JsonpResult(false, "Não existe Solicitação rejeitada para reenviar");

                foreach (var autorizacao in listaAutorizacao)
                {
                    autorizacao.Situacao = SituacaoAlcadaRegra.Pendente;
                    repAprovacao.Atualizar(autorizacao);
                }

                existeCancelamentoProvisao.Situacao = SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao;
                repositorioCancelamentoProvisao.Atualizar(existeCancelamentoProvisao);

                return new JsonpResult(true, "Solicitação enviada para processamento");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Problema ao tentar reenviar a aprovação do cancelamento.");
            }
        }
        public async Task<IActionResult> CancelarProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao repositorioEstornoProvisaoSolicitacao = new Repositorio.Embarcador.Escrituracao.EstornoProvisaoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao existeCancelamentoProvisao = repositorioCancelamentoProvisao.BuscarPorCodigo(codigo, false);

                if (existeCancelamentoProvisao == null)
                    return new JsonpResult(false, "Cancelamento Não encontrado para enviar");

                var existeSolicitacao = repositorioEstornoProvisaoSolicitacao.BuscarPorCancelamentoProvisao(existeCancelamentoProvisao.Codigo);

                existeSolicitacao.Situacao = SituacaoEstornoProvisaoSolicitacao.Reprovada;
                existeCancelamentoProvisao.Situacao = SituacaoCancelamentoProvisao.NaoProcessado;
                repositorioCancelamentoProvisao.Atualizar(existeCancelamentoProvisao);
                repositorioEstornoProvisaoSolicitacao.Atualizar(existeSolicitacao);
                return new JsonpResult(true, "Solicitação enviada para processamento");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Problema ao tentar reenviar a aprovação do cancelamento.");
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);

            var filtroPesquisa = ObterFiltroPesquisaCancelamento();

            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> listaGrid = repProvisao.Consultar(filtroPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProvisao.ContarConsulta(filtroPesquisa);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            DataInicio = obj.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                            DataFim = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                            Tomador = obj.Tomador?.Descricao ?? "",
                            QuantidadeDocumentos = obj.QuantidadeDocsProvisao,
                            ValorProvisao = obj.ValorCancelamentoProvisao.ToString("n2"),
                            Transportador = obj.Empresa?.Descricao ?? "",
                            Filial = obj.Filial?.Descricao ?? "",
                            obj.DescricaoSituacao,
                            CancelamentoProvisaoContraPartida = obj.CancelamentoProvisaoContraPartida ? "Sim" : "Não"
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            List<dynamic> listaRetornar = new List<dynamic>();
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa = ObterFiltrosPesquisa();
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            totalRegistros = repositorioDocumentoProvisao.ContarConsulta(filtrosPesquisa);
            listaGrid = totalRegistros > 0 ? repositorioDocumentoProvisao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();

            if (listaGrid.Count > 0)
            {
                string nomeArquivoEDI = ObterNomeArquivoEDI(unitOfWork, filtrosPesquisa.CodigoCancelamentoProvisao, exportarPesquisa);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in listaGrid)
                {
                    listaRetornar.Add(new
                    {
                        documento.ModeloDocumentoFiscal?.TipoDocumentoEmissao,
                        SituacaoCTe = documento.CTe?.Status ?? "",
                        CodigoEmpresa = documento.Empresa?.Codigo ?? 0,
                        CodigoCTE = documento.CTe?.Codigo ?? 0,
                        documento.Codigo,
                        CodigoFilial = documento.Filial?.Codigo ?? 0,
                        CodigoTomador = documento.Tomador.Codigo,
                        CodigoTransportador = documento.Empresa?.Codigo ?? 0,
                        Transportador = documento.Empresa?.Descricao ?? "",
                        Documento = documento.NumeroDocumento.ToString() + " - " + documento.SerieDocumento.ToString(),
                        Tipo = documento.ModeloDocumentoFiscal?.Abreviacao,
                        FechamentoFrete = documento.FechamentoFrete?.Numero.ToString() ?? "",
                        Carga = documento.Carga?.CodigoCargaEmbarcador ?? "",
                        Ocorrencia = documento.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? "",
                        DataEmissao = documento.DataEmissao.ToString("dd/MM/yyyy"),
                        Tomador = documento.Tomador.Descricao,
                        Filial = documento.Filial?.Descricao ?? "",
                        TipoDocumentoCreditoDebito = documento.CTe?.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebitoDescricao ?? "",
                        ValorFrete = documento.ValorProvisao.ToString("n2"),
                        NomeArquivoEDI = nomeArquivoEDI,
                        DataFolha = documento.DataFolha.HasValue ? documento.DataFolha.Value.ToString("dd/MM/yyyy") : (documento.Stage != null ? (documento.Stage.DataFolha.HasValue ? documento.Stage.DataFolha.Value.ToString("dd/MM/yyyy") : "") : ""),
                        DiasEmAberto = documento.Stage != null ? (documento.Stage.DataFolha.HasValue ? (DateTime.Now - documento.Stage.DataFolha).Value.Days : 0) : 0
                    });
                }
            }

            return listaRetornar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao()
            {
                CodigoCancelamentoProvisao = Request.GetIntParam("Codigo"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoOcorrencia = Request.GetIntParam("Ocorrencia"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                SituacaoProvisaoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado,
                TipoLocalPrestacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.todos,
                CancelamentoProvisaoContraPartida = Request.GetBoolParam("CancelamentoProvisaoContraPartida")
            };

            if (filtrosPesquisa.CodigoTransportador > 0)
                filtrosPesquisa.ListaCodigoTransportador = new List<int>() { filtrosPesquisa.CodigoTransportador };

            if (filtrosPesquisa.CodigoCancelamentoProvisao > 0)
                filtrosPesquisa.SituacaoProvisaoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Todos;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Inicio", "DataInicio", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Fim", "DataFim", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Qnt. Docs", "QuantidadeDocumentos", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor Provisão", "ValorProvisao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);


            if (configuracaoFatura?.DisponbilizarProvisaoContraPartidaParaCancelamento ?? false)
                grid.AdicionarCabecalho("Cnacelamento de Contra Partida", "CancelamentoProvisaoContraPartida", 20, Models.Grid.Align.left, true);


            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaDocumento(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoTomador", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);
                if (repFechamentoFrete.VerificarExistemFechamento())
                    grid.AdicionarCabecalho("Fechamento de Contrato", "FechamentoFrete", 8, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);

                if (configuracaoTMS.ProvisionarDocumentosEmitidos)
                    grid.AdicionarCabecalho("Tipo Doc.", "TipoDocumentoCreditoDebito", 10, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data Folha", "DataFolha", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Dias em Aberto", "DiasEmAberto", 10, Models.Grid.Align.right, false);

                if (exportarPesquisa)
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> listaGrid = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
                int totalRegistros = 0;
                var lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterNomeArquivoEDI(Repositorio.UnitOfWork unitOfWork, int codigoCancelamentoProvisao, bool exportarPesquisa)
        {
            if (exportarPesquisa & (codigoCancelamentoProvisao > 0))
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repositorio = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao integracao = repositorio.BuscarPorCancelamentoProvisao(codigoCancelamentoProvisao).FirstOrDefault();

                if (integracao != null)
                    return Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, true, unitOfWork);
            }

            return "";
        }

        private void ObterPropriedadeOrdenar(ref string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "DataInicio")
                propriedadeOrdenar = "DataInicial";
            else if (propriedadeOrdenar == "DataFim")
                propriedadeOrdenar = "DataFinal";
            else if (propriedadeOrdenar == "QuantidadeDocumentos")
                propriedadeOrdenar = "QuantidadeDocsProvisao";
            else if (propriedadeOrdenar == "DescricaoSituacao")
                propriedadeOrdenar = "Situacao";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";
            else if (propriedadeOrdenar == "ValorProvisao")
                propriedadeOrdenar = "ValorCancelamentoProvisao";

        }

        private string ObterPropriedadeOrdenarDocumentos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                propriedadeOrdenar = "NumeroDocumento";
            else if (propriedadeOrdenar == "FechamentoFrete")
                return "FechamentoFrete.Numero";
            else if (propriedadeOrdenar == "Carga")
                propriedadeOrdenar = "Carga.CodigoCargaEmbarcador";
            else if (propriedadeOrdenar == "Ocorrencia")
                propriedadeOrdenar = "CargaOcorrencia.NumeroOcorrencia";
            else if (propriedadeOrdenar == "Tipo")
                propriedadeOrdenar = "ModeloDocumentoFiscal.Abreviacao";
            else if (propriedadeOrdenar == "Destinatario")
                propriedadeOrdenar = "Destinatario.Nome";
            else if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "ValorFrete")
                propriedadeOrdenar = "ValorProvisao";
            else if (propriedadeOrdenar == "TipoDocumentoCreditoDebito")
                propriedadeOrdenar = "CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";

            return propriedadeOrdenar;
        }

        private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaCancelamentoProvisao ObterFiltroPesquisaCancelamento()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaCancelamentoProvisao()
            {
                Carga = Request.GetIntParam("Carga"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataFinal = Request.GetNullableDateTimeParam("DataFim"),
                Filial = Request.GetIntParam("Filial"),
                LocalPrestacao = Request.GetIntParam("LocalidadePrestacao"),
                Numero = Request.GetIntParam("Numero"),
                NumeroDoc = Request.GetIntParam("NumeroDOC"),
                Ocorrencia = Request.GetIntParam("Ocorrencia"),
                NumeroFolha = Request.GetStringParam("NumeroFolha"),
                Situacao = Request.GetEnumParam<SituacaoCancelamentoProvisao>("Situacao"),
                Tomador = Request.GetDoubleParam("Tomador"),
                CancelamentoProvisaoContraPartida = Request.GetNullableBoolParam("CancelamentoProvisaoContraPartida"),
                Transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : Request.GetIntParam("Transportador")
            };
        }
        #endregion
    }
}
