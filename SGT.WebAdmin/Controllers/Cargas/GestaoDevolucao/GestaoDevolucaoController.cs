using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Xml.Linq;

namespace SGT.WebAdmin.Controllers.Cargas.GestaoDevolucao
{
    [CustomAuthorize("Cargas/GestaoDevolucao")]
    public class GestaoDevolucaoController : BaseController
    {
        #region Construtores

        public GestaoDevolucaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDevolucoes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(await ObterGridPesquisaDevolucoesAsync(unitOfWork, cancellationToken));
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasFiscaisOrigem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisaNotasFiscaisOrigem(unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasFiscaisPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisaNotasFiscaisPallet(unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNFesPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(await ObterGridPesquisaNFesPallet(unitOfWork));
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

        public async Task<IActionResult> BuscarDetalhes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa = new FiltroPesquisaGestaoDevolucao()
                {
                    CodigoNF = codigo
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    LimiteRegistros = 1,
                    PropriedadeOrdenar = "CodigoNF",
                    DirecaoOrdenar = "asc"
                };

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet dados = repositorioGestaoDevolucao.ConsultaDetalhesNota(filtroPesquisa, parametrosConsulta);

                return new JsonpResult(dados);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDevolucoes(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = await ObterGridPesquisaDevolucoesAsync(unitOfWork, cancellationToken);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"GestaoDevolucaoDevolucoes.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarGridPesquisaNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaNotasFiscaisPallet(unitOfWork);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"GestaoDevolucaoNotasFiscais.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoGestaoDevolucao;
                int.TryParse(Request.Params("GestaoDevolucao"), out codigoGestaoDevolucao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "TipoIntegracaoDevolucao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Extrato", "Extrato", 8, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao tipoIntegracaoGestaoDevolucao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao>("Etapa") ?? gestaoDevolucao.EtapaAtual.Etapa;
                List<TipoIntegracaoGestaoDevolucao> listaTipoIntegracao = tipoIntegracaoGestaoDevolucao.ObterTipoPorEtapa();

                int quantidadeIntegracoes = repositorioGestaoDevolucaoIntegracao.ContarConsulta(codigoGestaoDevolucao, situacao, tipo, listaTipoIntegracao);

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao>();

                if (quantidadeIntegracoes > 0)
                {
                    if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repositorioGestaoDevolucaoIntegracao.Consultar(codigoGestaoDevolucao, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, listaTipoIntegracao);
                }
                grid.setarQuantidadeTotal(listaIntegracao.Count);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoIntegracao.Tipo,
                                       obj.SituacaoIntegracao,
                                       Situacao = obj.SituacaoIntegracao.ObterDescricao(),
                                       TipoIntegracao = obj.TipoIntegracao.Tipo.ObterDescricao(),
                                       TipoIntegracaoDevolucao = obj.TipoIntegracaoGestaoDevolucao.ObterDescricao(),
                                       Retorno = obj.ProblemaIntegracao,
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       Extrato = VerificarExibicaoEtratoLaudo(obj.SituacaoIntegracao, gestaoDevolucao),
                                       DT_RowColor = obj.SituacaoIntegracao.ObterCorLinha(),
                                       DT_FontColor = obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao ? CorGrid.Branco : "",
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigoGestaoDevolucao = Request.GetLongParam("GestaoDevolucao");

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao tipoIntegracaoGestaoDevolucao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao>("Etapa") ?? gestaoDevolucao.EtapaAtual.Etapa;
                List<TipoIntegracaoGestaoDevolucao> listaTipoIntegracao = tipoIntegracaoGestaoDevolucao.ObterTipoPorEtapa();

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repositorioGestaoDevolucaoIntegracao.ContarPorGestaoDevolucao(codigoGestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, listaTipoIntegracao);
                int totalIntegrado = repositorioGestaoDevolucaoIntegracao.ContarPorGestaoDevolucao(codigoGestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, listaTipoIntegracao);
                int totalProblemaIntegracao = repositorioGestaoDevolucaoIntegracao.ContarPorGestaoDevolucao(codigoGestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, listaTipoIntegracao);
                int totalAguardandoRetorno = repositorioGestaoDevolucaoIntegracao.ContarPorGestaoDevolucao(codigoGestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno, listaTipoIntegracao);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDevolucao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao = repIntegracao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucaoIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                gestaoDevolucaoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucaoIntegracao, null, "Reenviou Integração.", unitOfWork);

                repIntegracao.Atualizar(gestaoDevolucaoIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarAnaliseCenarioPosEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDevolucao = Request.GetIntParam("CodigoGestaoDevolucao");
                bool procedente = Request.GetBoolParam("Analise");
                string observacao = Request.GetStringParam("Observacao");

                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.CenarioPosEntrega, unitOfWork);

                gestaoDevolucao.Procedente = procedente;
                gestaoDevolucao.ObservacaoProcedencia = observacao;

                if (!gestaoDevolucao.Procedente)
                    servicoGestaoDevolucao.EnviarEmailCenarioPosEntrega(gestaoDevolucao);

                repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), "Salvou análise do cenário Pós Entrega", unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosDevolucaoPorEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic retorno = null;
                int codigoGestaoDevolucao = Request.GetIntParam("CodigoGestaoDevolucao");

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> NotasDeOrigem = gestaoDevolucao.NotasFiscaisDeOrigem?.ToList() ?? new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();
                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> NotasDeDevolucao = gestaoDevolucao.NotasFiscaisDevolucao?.ToList() ?? new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();

                if (gestaoDevolucao == null)
                    throw new ControllerException("Não foi possível encontrar o registro da devolução.");

                EtapaGestaoDevolucao etapaGestaoDevolucao = Request.GetNullableEnumParam<EtapaGestaoDevolucao>("EtapaGestaoDevolucao") ?? gestaoDevolucao.EtapaAtual.Etapa;

                switch (etapaGestaoDevolucao)
                {
                    case EtapaGestaoDevolucao.GestaoDeDevolucao:
                        break;
                    case EtapaGestaoDevolucao.DefinicaoTipoDevolucao:
                        retorno = new
                        {
                            TipoNotasDevolucao = gestaoDevolucao.TipoNotas,
                            TipoTomador = gestaoDevolucao.DadosComplementares?.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? "CIF" : gestaoDevolucao.DadosComplementares?.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? "FOB" : "Outros",
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.AprovacaoTipoDevolucao:
                        retorno = new
                        {
                            Codigo = gestaoDevolucao.Codigo,
                            NumerosNotasFiscais = gestaoDevolucao.NotasFiscaisDeOrigem != null ? string.Join(", ", gestaoDevolucao.NotasFiscaisDeOrigem.Select(x => x.XMLNotaFiscal.Numero)) : string.Empty,
                            DataEmissao = gestaoDevolucao.DataCriacao.ToString(),
                            TipoDevolucao = gestaoDevolucao.Tipo.ObterDescricao(),
                            Observacao = gestaoDevolucao.DadosComplementares?.ObservacaoAprovacao ?? string.Empty,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao

                        };
                        break;
                    case EtapaGestaoDevolucao.OrdemeRemessa:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            Observacao = !string.IsNullOrEmpty(gestaoDevolucao.ObservacaoOrdemRemessa) ? gestaoDevolucao.ObservacaoOrdemRemessa : string.Empty,
                            Ordem = gestaoDevolucao.DadosComplementares?.Ordem ?? string.Empty,
                            Remessa = gestaoDevolucao.DadosComplementares?.Remessa ?? string.Empty,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao

                        };
                        break;
                    case EtapaGestaoDevolucao.GeracaoOcorrenciaDebito:
                        Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repositorioCargaOcorrencia.BuscarOcorrenciaPorCodigoGestaoDevolucao(gestaoDevolucao.Codigo);
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            CodigoOcorrencia = cargaOcorrencia != null ? cargaOcorrencia.Codigo : 0,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.DefinicaoLocalColeta:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            Cliente = new { Codigo = gestaoDevolucao.DadosComplementares?.ClienteDestinoColeta?.CPF_CNPJ, Descricao = gestaoDevolucao.DadosComplementares?.ClienteDestinoColeta?.NomeCNPJ },
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.GeracaoCargaDevolucao:
                        retorno = BuscarDadosGeracaoCargaDevolucao(gestaoDevolucao, unitOfWork);
                        break;
                    case EtapaGestaoDevolucao.AgendamentoParaDescarga:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            DataCarregamento = gestaoDevolucao.DadosComplementares?.DataCarregamento,
                            DataDescarregamento = gestaoDevolucao.DadosComplementares?.DataDescarregamento,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.GestaoCustoContabil:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            ContaContabil = gestaoDevolucao.DadosComplementares?.ContaContabil ?? string.Empty,
                            CentroCusto = gestaoDevolucao.DadosComplementares?.CentroCusto ?? string.Empty,
                            EnviadaAoEmail = gestaoDevolucao.DadosComplementares?.EmailEnviado ?? false,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.Agendamento:
                        retorno = BuscarDadosAgendamento(gestaoDevolucao, unitOfWork);
                        break;
                    case EtapaGestaoDevolucao.AprovacaoDataDescarga:
                        retorno = BuscarDadosAprovacaoDataDescarga(gestaoDevolucao, unitOfWork);
                        break;
                    case EtapaGestaoDevolucao.Monitoramento:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            CodigoCargaDevolucao = gestaoDevolucao.CargaDevolucao?.Codigo ?? 0,
                            TipoCarga = gestaoDevolucao.CargaDevolucao?.TipoDeCarga?.Descricao ?? "-",
                            DataColeta = gestaoDevolucao.CargaDevolucao?.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? "-",
                            Hora = gestaoDevolucao.CargaDevolucao?.DataCarregamentoCarga?.ToString("HH:mm") ?? "-",
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.GeracaoLaudo:
                        retorno = BuscarDadosGeracaoLaudo(gestaoDevolucao, unitOfWork);
                        break;
                    case EtapaGestaoDevolucao.AprovacaoLaudo:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            NumeroLaudo = gestaoDevolucao.Laudo?.Codigo ?? 0,
                            DataCriacao = gestaoDevolucao.Laudo?.DataCriacao.ToString() ?? "-",
                            Responsavel = gestaoDevolucao.Laudo?.Responsavel?.Descricao ?? "-",
                            Transportador = gestaoDevolucao.Laudo?.Transportador?.Descricao ?? "-",
                            Veiculo = gestaoDevolucao.Laudo?.Veiculo?.Descricao ?? "-",
                            Motivo = gestaoDevolucao.Laudo?.Motivo ?? string.Empty,
                            NumeroCompensacao = gestaoDevolucao.Laudo?.NumeroCompensacao ?? "-",
                            DataCompensacao = gestaoDevolucao.Laudo?.DataCompensacao?.ToString() ?? "-",
                            Valor = gestaoDevolucao.Laudo != null && gestaoDevolucao.Laudo?.Valor > 0 ? gestaoDevolucao.Laudo.Valor.ToString() : "-",
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.IntegracaoLaudo:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            Valor = gestaoDevolucao.Laudo?.Valor.ToString("N2") ?? string.Empty,
                            DataCompensacao = gestaoDevolucao.Laudo?.DataCompensacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                            NumeroCompensacao = gestaoDevolucao.Laudo?.NumeroCompensacao,
                            Fornecedor = gestaoDevolucao.Laudo?.Transportador?.NomeCNPJ?.ToString() ?? "-",
                            DataVencimento = gestaoDevolucao.Laudo?.DataVencimento?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.CenarioPosEntrega:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            Analise = gestaoDevolucao.Procedente,
                            Observacao = !string.IsNullOrEmpty(gestaoDevolucao.ObservacaoProcedencia) ? gestaoDevolucao.ObservacaoProcedencia : string.Empty,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.RegistroDocumentosPallet:
                        Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo repRegistroDocumentosPalletAnexo = new Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo> registrosDocumentosPalletAnexo = repRegistroDocumentosPalletAnexo.BuscarPorCodigoGestaoDevolucao(codigoGestaoDevolucao);
                        Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.NotaFiscalPermutaPallet dadosNF = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.NotaFiscalPermutaPallet();

                        if (registrosDocumentosPalletAnexo.Count > 0 && (!string.IsNullOrEmpty(registrosDocumentosPalletAnexo.FirstOrDefault().DadosXMLNota)))
                            dadosNF = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.NotaFiscalPermutaPallet>(registrosDocumentosPalletAnexo.FirstOrDefault().DadosXMLNota);

                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            PossuiDocumentosAnexados = registrosDocumentosPalletAnexo.Count > 0,
                            ClientePagouNFe = registrosDocumentosPalletAnexo?.FirstOrDefault()?.PagamentoRealizado ?? false,
                            DataEmissao = dadosNF.DataEmissao ?? string.Empty,
                            Valor = dadosNF.Valor.ToString("n2"),
                            Numero = dadosNF.Numero.ToString() ?? string.Empty,
                            Chave = dadosNF.Chave ?? string.Empty,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao
                        };
                        break;
                    case EtapaGestaoDevolucao.DocumentacaoEntradaFiscal:
                        retorno = new
                        {
                            CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                            NumeroNotaFiscal = gestaoDevolucao.NotaFiscalOrigem?.Numero ?? 0,
                            DataEmissao = gestaoDevolucao.NotaFiscalOrigem?.DataEmissao.ToString("dd/MM/yyyy") ?? string.Empty,
                            CodigoIntegracaoFilial = gestaoDevolucao.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                            TotalQuantidadePallets = gestaoDevolucao.NotasFiscaisDeOrigem?.Sum(nf => nf.XMLNotaFiscal.QuantidadePallets) ?? 0,
                            AdicionarNFesTransferenciaPallet = gestaoDevolucao.DadosComplementares?.AdicionarNFesTransferenciaPallet,
                            ObservacaoDocumentacaoEntradaFiscal = gestaoDevolucao.DadosComplementares?.ObservacaoDocumentacaoEntradaFiscal,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao,
                            NumeroNotaFiscalDevolucao = gestaoDevolucao.NumeroNotaFiscalDevolucao ?? 0,
                            SerieNotaFiscalDevolucao = gestaoDevolucao.SerieNotaFiscalDevolucao ?? string.Empty,
                            NumeroNotaFiscalPermuta = gestaoDevolucao.NumeroNotaFiscalPermuta ?? 0,
                            SerieNotaFiscalPermuta = gestaoDevolucao.SerieNotaFiscalPermuta ?? string.Empty,
                        };
                        break;
                    case EtapaGestaoDevolucao.AprovacaoCenarioPosEntrega:
                        retorno = new
                        {
                            Codigo = gestaoDevolucao.Codigo,
                            NumerosNotasFiscais = gestaoDevolucao.NotasFiscaisDeOrigem != null ? string.Join(", ", gestaoDevolucao.NotasFiscaisDeOrigem.Select(x => x.XMLNotaFiscal.Numero)) : string.Empty,
                            DataEmissao = gestaoDevolucao.DataCriacao.ToString(),
                            TipoDevolucao = gestaoDevolucao.Tipo.ObterDescricao(),
                            Observacao = gestaoDevolucao.DadosComplementares?.ObservacaoAprovacao ?? string.Empty,
                            SituacaoDevolucao = gestaoDevolucao.SituacaoDevolucao,
                            Recusa = Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao.ObterTipoRecusa(NotasDeOrigem, NotasDeDevolucao)
                        };
                        break;
                    default:
                        break;
                }
                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados da devolução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarTipoGestaoDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AprovacaoTipoDevolucao, unitOfWork);

                await unitOfWork.StartAsync();

                if (gestaoDevolucao.DadosComplementares != null)
                {
                    gestaoDevolucao.DadosComplementares.UsuarioAprovacao = Usuario;
                    if (!string.IsNullOrEmpty(observacao))
                        gestaoDevolucao.DadosComplementares.ObservacaoAprovacao = observacao;
                    await repositorioGestaoDevolucaoDadosComplementares.AtualizarAsync(gestaoDevolucao.DadosComplementares);
                }

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                gestaoDevolucao.Aprovada = true;

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true, excecao.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ReprovarTipoGestaoDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");
                TipoGestaoDevolucao tipoGestaoDevolucao = Request.GetEnumParam<TipoGestaoDevolucao>("TipoGestaoDevolucao");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AprovacaoTipoDevolucao, unitOfWork);

                await unitOfWork.StartAsync();

                if (!string.IsNullOrEmpty(observacao) && gestaoDevolucao.DadosComplementares != null)
                    gestaoDevolucao.DadosComplementares.ObservacaoAprovacao = observacao;

                gestaoDevolucao.EtapaAtual.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.Finalizada;
                servicoGestaoDevolucao.AlterarTipoGestaoDevolucao(gestaoDevolucao, tipoGestaoDevolucao);
                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar o tipo da devolução.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarPosEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");
                SimNao PosEntrega = Request.GetEnumParam<SimNao>("PosEntrega");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AprovacaoCenarioPosEntrega, unitOfWork);

                await unitOfWork.StartAsync();

                await servicoGestaoDevolucao.SalvarPosEntrega(observacao, gestaoDevolucao, PosEntrega);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, "Pós entrega salva com sucesso");
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a pós entrega");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarPosEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");
                TipoGestaoDevolucao tipoGestaoDevolucao = Request.GetEnumParam<TipoGestaoDevolucao>("TipoGestaoDevolucao");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AprovacaoCenarioPosEntrega, unitOfWork);

                await unitOfWork.StartAsync();

                await servicoGestaoDevolucao.AprovarPosEntrega(tipoGestaoDevolucao, gestaoDevolucao, Usuario);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 60, Models.Grid.Align.left, false);


                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao = repGestaoDevolucaoIntegracao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo> integracoesArquivos = gestaoDevolucaoIntegracao.ArquivosTransacao.OrderByDescending(i => i.Codigo).ToList();

                grid.setarQuantidadeTotal(integracoesArquivos.Count);

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao = repositorioIntegracao.BuscarPorCodigoArquivo(codigo);

                if (gestaoDevolucaoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo gestaoDevolucaoIntegracaoArquivo = gestaoDevolucaoIntegracao.ArquivosTransacao.FirstOrDefault(o => o.Codigo == codigo);

                if ((gestaoDevolucaoIntegracaoArquivo == null) || ((gestaoDevolucaoIntegracaoArquivo.ArquivoRequisicao == null) && (gestaoDevolucaoIntegracaoArquivo.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { gestaoDevolucaoIntegracaoArquivo.ArquivoRequisicao, gestaoDevolucaoIntegracaoArquivo.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos {gestaoDevolucaoIntegracao.GestaoDevolucao.Tipo.ObterDescricao()} NFD {gestaoDevolucaoIntegracao.GestaoDevolucao.NotaFiscalDevolucao?.Numero}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarProdutosNotasFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisaProdutos(unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> DefinirTipoGestaoDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, "Somente o transportador pode definir o tipo da devolução");

                int codigoGestaoDevolucao = Request.GetIntParam("Codigo");
                TipoGestaoDevolucao tipoGestaoDevolucao = Request.GetEnumParam<TipoGestaoDevolucao>("TipoGestaoDevolucao");

                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa repositorioGestaoDevolucaoEtapa = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.DefinicaoTipoDevolucao, unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa gestaoDevolucaoEtapaDefinicao = repositorioGestaoDevolucaoEtapa.BuscarEtapaPorEtapaECodigoGestao(codigoGestaoDevolucao, EtapaGestaoDevolucao.DefinicaoTipoDevolucao);

                unitOfWork.Start();
                servicoGestaoDevolucao.DefinirTipoGestaoDevolucao(gestaoDevolucao, tipoGestaoDevolucao);

                gestaoDevolucaoEtapaDefinicao.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.Finalizada;
                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                if (tipoGestaoDevolucao == TipoGestaoDevolucao.PermutaPallet && gestaoDevolucao.TipoNotas == TipoNotasGestaoDevolucao.Pallet)
                {
                    AvancarEtapaAprovacaoPermutaPallet(gestaoDevolucao, unitOfWork);
                    repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao definir o tipo da devolução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarDadosLaudo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servico = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                int codigoGestaoDevolucao = Request.GetIntParam("CodigoGestaoDevolucao");
                int CodigoLaudo = Request.GetIntParam("Codigo");
                DateTime DataCriacao = Request.GetDateTimeParam("DataCriacao");
                int codigoResponsavel = Request.GetIntParam("Responsavel");
                int codigoTransportador = Request.GetIntParam("Transportador");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                List<dynamic> produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Produtos"));

                if (produtos.Count == 0)
                {
                    throw new ControllerException("Informe ao menos um produto para gerar o Laudo.");
                }

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo = repositorioGestaoDevolucaoLaudo.BuscarPorCodigo(CodigoLaudo);
                if (gestaoDevolucaoLaudo == null || gestaoDevolucao == null) return new JsonpResult(false, "Não foi possível encontrar o registro");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.GeracaoLaudo, unitOfWork);

                unitOfWork.Start();
                gestaoDevolucaoLaudo.Initialize();

                gestaoDevolucaoLaudo.DataCriacao = DataCriacao;
                gestaoDevolucaoLaudo.Responsavel = new Repositorio.Usuario(unitOfWork).BuscarPorCodigo(codigoResponsavel);
                gestaoDevolucaoLaudo.Transportador = new Repositorio.Empresa(unitOfWork).BuscarPorCodigo(codigoTransportador);
                gestaoDevolucaoLaudo.Veiculo = new Repositorio.Veiculo(unitOfWork).BuscarPorCodigo(codigoVeiculo);

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto> produtosOld = gestaoDevolucaoLaudo.Produtos.ToList();
                gestaoDevolucaoLaudo.Produtos = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>();
                foreach (dynamic produto in produtos)
                {
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto produtoLaudo = produtosOld.Find(p => p.Produto != null && p.Produto.Codigo == (int)produto.Produto);
                    if (produtoLaudo == null)
                    {
                        produtoLaudo = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto();
                        produtoLaudo.Laudo = gestaoDevolucaoLaudo;
                        produtoLaudo.Produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador() { Codigo = produto.Produto };
                        produtoLaudo.QuantidadeOrigem = ((string)produto.QuantidadeOrigem).ToDecimal();
                    }

                    produtoLaudo.QuantidadeDevolvida = ((string)produto.QuantidadeDevolvida).ToDecimal();
                    produtoLaudo.QuantidadeAvariada = ((string)produto.QuantidadeAvariada).ToDecimal();
                    produtoLaudo.ValorAvariado = ((string)produto.ValorAvariado).ToDecimal();
                    produtoLaudo.QuantidadeSobras = ((string)produto.QuantidadeSobras).ToDecimal();
                    produtoLaudo.ValorSobras = ((string)produto.ValorSobras).ToDecimal();
                    produtoLaudo.QuantidadeSemCondicao = ((string)produto.QuantidadeSemCondicao).ToDecimal();
                    produtoLaudo.ValorSemCondicao = ((string)produto.ValorSemCondicao).ToDecimal();
                    produtoLaudo.QuantidadeFalta = ((string)produto.QuantidadeFalta).ToDecimal();
                    produtoLaudo.ValorFalta = ((string)produto.ValorFalta).ToDecimal();
                    produtoLaudo.ValorTotal = ((string)produto.ValorTotal).ToDecimal();
                    produtoLaudo.QuantidadeDescarte = ((string)produto.QuantidadeDescarte).ToDecimal();
                    produtoLaudo.QuantidadeManutencao = ((string)produto.QuantidadeManutencao).ToDecimal();

                    gestaoDevolucaoLaudo.Produtos.Add(produtoLaudo);
                }

                repositorioGestaoDevolucaoLaudo.Atualizar(gestaoDevolucaoLaudo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucaoLaudo, gestaoDevolucaoLaudo.GetChanges(), "Salvou dados do Laudo", unitOfWork);
                unitOfWork.CommitChanges();

                gestaoDevolucaoLaudo.CaminhoArquivoLaudo = string.Empty;
                servico.ObterPDFLaudo(gestaoDevolucaoLaudo);

                servico.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                JsonpResult retorno = null;

                if (excecao is ControllerException)
                {
                    retorno = new JsonpResult(false, true, excecao.Message);
                }
                else
                {
                    retorno = new JsonpResult(false, "Ocorreu uma falha ao gerar o Laudo da Devolução.");
                    Servicos.Log.TratarErro(excecao);
                }

                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarOrdemERemessa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDevolucao = Request.GetIntParam("CodigoGestaoDevolucao");
                string ordem = Request.GetStringParam("Ordem");
                string remessa = Request.GetStringParam("Remessa");

                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    throw new Exception("Não foi possível encontrar o registro");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.OrdemeRemessa, unitOfWork);

                unitOfWork.Start();

                gestaoDevolucao.ObservacaoOrdemRemessa = $"Ordem:{ordem} | Remessa:{remessa}";
                repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);

                if (gestaoDevolucao.DadosComplementares != null)
                {
                    gestaoDevolucao.DadosComplementares.Initialize();
                    gestaoDevolucao.DadosComplementares.Ordem = ordem;
                    gestaoDevolucao.DadosComplementares.Remessa = remessa;
                    repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucao.DadosComplementares);
                }

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                servicoGestaoDevolucao.GerarIntegracoes(gestaoDevolucao, TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega, new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Salesforce });

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao, gestaoDevolucao.DadosComplementares.GetChanges(), "Salvou Ordem e Remessa", unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar ordem e remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarAprovacaoLaudo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente, cancellationToken);
                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");
                string motivoAvaliacao = Request.GetStringParam("Motivo");
                bool laudoAprovado = Request.GetBoolParam("LaudoAprovado");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AprovacaoLaudo, unitOfWork);

                unitOfWork.Start();
                gestaoDevolucao.Laudo.Initialize();

                gestaoDevolucao.Laudo.DataAnalise = DateTime.Now;
                gestaoDevolucao.Laudo.Motivo = motivoAvaliacao;
                gestaoDevolucao.Laudo.Situacao = laudoAprovado ? SituacaoAprovacaoLaudo.Aprovado : SituacaoAprovacaoLaudo.Reprovado;

                repositorioGestaoDevolucaoLaudo.Atualizar(gestaoDevolucao.Laudo);

                if (laudoAprovado)
                {
                    servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                    servicoGestaoDevolucao.GerarIntegracoes(gestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao.SAPLaudo, new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { TipoIntegracao.YPE });
                }
                else
                    servicoGestaoDevolucao.VoltarEtapaGestaoDevolucao(gestaoDevolucao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao.Laudo, gestaoDevolucao.Laudo.GetChanges(), "Salvou análise do Laudo", unitOfWork);

                unitOfWork.CommitChanges();

                gestaoDevolucao.Laudo.CaminhoArquivoLaudo = string.Empty;
                servicoGestaoDevolucao.ObterPDFLaudo(gestaoDevolucao.Laudo);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados de avaliação do laudo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarGestaoCustoContabil()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao) ?? throw new ControllerException("Não foi possível encontar a gestão de devolução.");
                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.GestaoCustoContabil, unitOfWork);

                if (gestaoDevolucao.DadosComplementares != null)
                {
                    gestaoDevolucao.DadosComplementares.GestaoDevolucao = gestaoDevolucao;
                    gestaoDevolucao.DadosComplementares.CentroCusto = Request.GetStringParam("CentroCusto");
                    gestaoDevolucao.DadosComplementares.ContaContabil = Request.GetStringParam("ContaContabil");
                    gestaoDevolucao.DadosComplementares.EmailEnviado = servicoGestaoDevolucao.EnviarEmailGestaoCustoContabil(gestaoDevolucao);
                    repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucao.DadosComplementares);
                }

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao) ?? throw new ControllerException("Não foi possível encontar a gestão de devolução.");
                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.Agendamento, unitOfWork);

                DateTime dataCarregamento = Request.GetDateTimeParam("DataCarregamento");
                DateTime dataDescarregamento = Request.GetDateTimeParam("DataDescarregamento");
                string observacao = Request.GetStringParam("Observacao");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoPeriodoDescarregamento = Request.GetIntParam("PeriodoDescarregamento");
                double codigoDestinatario = Request.GetDoubleParam("Destinatario");
                double codigoRemetente = Request.GetDoubleParam("Remetente");
                int codigoTipoCarga = Request.GetIntParam("TipoDeCarga");

                unitOfWork.Start();

                Dominio.Entidades.Cliente remetente = repositorioCliente.BuscarPorCPFCNPJ(codigoRemetente);
                Dominio.Entidades.Cliente destinatario = repositorioCliente.BuscarPorCPFCNPJ(codigoDestinatario);
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoCarga);
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCodigo(codigoPeriodoDescarregamento);

                if (gestaoDevolucao.DadosComplementares != null)
                {
                    gestaoDevolucao.DadosComplementares.GestaoDevolucao = gestaoDevolucao;
                    gestaoDevolucao.DadosComplementares.DataDaSolicitacao = DateTime.Now;
                    gestaoDevolucao.DadosComplementares.DataCarregamento = dataCarregamento;
                    gestaoDevolucao.DadosComplementares.DataDescarregamento = dataDescarregamento;
                    gestaoDevolucao.DadosComplementares.Transportador = Usuario.Empresa;
                    gestaoDevolucao.DadosComplementares.Motorista = motorista;
                    gestaoDevolucao.DadosComplementares.Veiculo = veiculo;
                    gestaoDevolucao.DadosComplementares.ObservacaoAgendamento = observacao;
                    gestaoDevolucao.DadosComplementares.ObservacaoAnaliseAgendamento = string.Empty;
                    gestaoDevolucao.DadosComplementares.DestinatarioAgendamento = destinatario;
                    gestaoDevolucao.DadosComplementares.RemetenteAgendamento = remetente;
                    gestaoDevolucao.DadosComplementares.PeriodoDescarregamento = periodoDescarregamento;
                    gestaoDevolucao.DadosComplementares.TipoDeCarga = tipoDeCarga;
                    repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucao.DadosComplementares);
                }

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAtendimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao) ?? throw new ControllerException("Não foi possível encontar a Gestão de Devolução.");

                IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamados = await repositorioChamado.BuscarChamadosPorGestaodevolucaoAsync(new List<long>() { gestaoDevolucao.Codigo });

                return new JsonpResult(chamados.ToList());
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarLocalColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, "Somente o transportador pode definir o local de coleta");

                int codigoGestaoDevolucao = Request.GetIntParam("CodigoGestaoDevolucao");
                double cpfCnpjCliente = Request.GetDoubleParam("Cliente");

                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    throw new Exception("Não foi possível encontrar o registro");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.DefinicaoLocalColeta, unitOfWork);

                Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                if (cliente == null)
                    throw new Exception("Não foi possível encontrar o cliente");

                unitOfWork.Start();

                if (gestaoDevolucao.DadosComplementares != null)
                {
                    gestaoDevolucao.DadosComplementares.ClienteDestinoColeta = cliente;
                    gestaoDevolucao.DadosComplementares.DestinatarioAgendamento = gestaoDevolucao.NotaFiscalOrigem?.Emitente;
                    gestaoDevolucao.DadosComplementares.RemetenteAgendamento = cliente;
                    gestaoDevolucao.DadosComplementares.TipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarTipoOperacaoPadraoDevolucaoTipoColeta();
                    repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucao.DadosComplementares);
                }

                repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);

                servicoGestaoDevolucao.AdicionarCargaDevolucao(gestaoDevolucao);
                unitOfWork.CommitChanges();

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), "Salvou Local de Coleta", unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao salvar o local de coleta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarAprovacaoAgendamentoDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AprovacaoDataDescarga, unitOfWork);

                bool aprovado = Request.GetBoolParam("Aprovado");
                string observacoes = Request.GetStringParam("Observacoes");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                unitOfWork.Start();


                gestaoDevolucao.DadosComplementares.DataAnaliseAgendamento = DateTime.Now;
                gestaoDevolucao.DadosComplementares.ObservacaoAnaliseAgendamento = observacoes;
                if (!aprovado)
                    gestaoDevolucao.DadosComplementares.ObservacaoAgendamento = string.Empty;
                else
                    gestaoDevolucao.DadosComplementares.TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucao.DadosComplementares);

                if (aprovado)
                {
                    servicoGestaoDevolucao.AdicionarCargaDevolucao(gestaoDevolucao);
                    servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                }
                else
                    servicoGestaoDevolucao.VoltarEtapaGestaoDevolucao(gestaoDevolucao);

                repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);


                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar aprovação da data de descarga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPdfVisualizarLaudo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(unitOfWork);

                int codigoLaudo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo = repositorioGestaoDevolucaoLaudo.BuscarPorCodigo(codigoLaudo) ?? throw new ControllerException($"Falha ao encontrar Laudo {codigoLaudo}");

                string base64PdfLaudo = Convert.ToBase64String(servicoGestaoDevolucao.ObterPDFLaudo(gestaoDevolucaoLaudo));
                if (string.IsNullOrWhiteSpace(base64PdfLaudo)) throw new ControllerException("Falha na obtenção do PDF do Laudo. Entre em contato com o administrador do sistema.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucaoLaudo, gestaoDevolucaoLaudo.GetChanges(), "Visualizou PDF", unitOfWork);

                return new JsonpResult(new
                {
                    Codigo = codigoLaudo,
                    PdfLaudoBase64 = $"data:application/pdf;base64,{base64PdfLaudo}"
                });
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o laudo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLaudo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(unitOfWork);

                int codigoLaudo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo = repositorioGestaoDevolucaoLaudo.BuscarPorCodigo(codigoLaudo) ?? throw new ControllerException($"Falha ao encontrar Laudo {codigoLaudo}");

                byte[] arquivo = servicoGestaoDevolucao.ObterPDFLaudo(gestaoDevolucaoLaudo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucaoLaudo, gestaoDevolucaoLaudo.GetChanges(), "Efetuou Download do PDF", unitOfWork);

                return Arquivo(arquivo, "application/pdf", $"Laudo {gestaoDevolucaoLaudo.Codigo}.pdf");
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarGestaoDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GeracaoDevolucaoPallet> listaNotasSelecionadasParaDevolucao = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GeracaoDevolucaoPallet>>(Request.Params("NotasSelecionadas"));
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao tipoFluxoGestaoDevolucao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao>("TipoFluxoDevolucao", TipoFluxoGestaoDevolucao.Normal);

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao svcGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais = await repositorioXMLNotaFiscal.BuscarTodasPorCodigosAsync(listaNotasSelecionadasParaDevolucao.Select(nota => nota.CodigoNF).ToList()); ;

                if (xmlNotasFiscais.Count == 0)
                    throw new ControllerException("Não foi possível encontrar as notas fiscais");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(listaNotasSelecionadasParaDevolucao.FirstOrDefault().CodigoCarga);
                Dominio.Enumeradores.TipoTomador? tipoTomadorNota = listaNotasSelecionadasParaDevolucao.FirstOrDefault().TipoTomadorDescricao == "CIF" ? Dominio.Enumeradores.TipoTomador.Remetente : listaNotasSelecionadasParaDevolucao.FirstOrDefault().TipoTomadorDescricao == "FOB" ? Dominio.Enumeradores.TipoTomador.Destinatario : null;

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await svcGestaoDevolucao.GerarDevolucaoPallet(xmlNotasFiscais, carga, TipoServicoMultisoftware, tipoTomadorNota, tipoFluxoGestaoDevolucao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, $"Devolução {gestaoDevolucao?.Codigo} gerada com sucesso.");
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(true, false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a devolução.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarAgendamentoParaDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);
                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarPorCarga(gestaoDevolucao.CargaDevolucao.Codigo);
                if (cargaEntregas.Count == 0)
                    return new JsonpResult("Carga de Devolução sem Controle de Entregas gerado. Aguarde até que o fluxo da carga tenha finalizado.");

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.AgendamentoParaDescarga, unitOfWork);

                DateTime? dataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento");
                DateTime? dataDescarregamento = Request.GetNullableDateTimeParam("DataDescarregamento");

                unitOfWork.Start();

                gestaoDevolucao.DadosComplementares.Initialize();
                gestaoDevolucao.DadosComplementares.DataCarregamento = dataCarregamento;
                gestaoDevolucao.DadosComplementares.DataDescarregamento = dataDescarregamento;
                repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucao.DadosComplementares);

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                {
                    if (cargaEntrega.Coleta) continue;
                    cargaEntrega.DataPrevista = dataDescarregamento;
                    repositorioCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, "Data de Previsão de Entrega alterada via Etapa de 'Agendamento para Descarga': " + cargaEntrega.DataPrevista.Value.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);
                }

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao, gestaoDevolucao.DadosComplementares.GetChanges(), "Salvar Agendamento para Descarga", unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar aprovação da data de descarga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarArquivosNotaFiscalPermutaPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo repGestaoDevolucaoPermutaPalletAnexo = new Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo(unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Servicos.NFe serNFe = new Servicos.NFe(unitOfWork, Auditado);

                long codigo = Request.GetLongParam("CodigoGestaoDevolucao");
                string descricao = Request.GetStringParam("Observacao");
                TipoRegistroPermutaPallet tipoRegistroPermutaPallet = Request.GetEnumParam<TipoRegistroPermutaPallet>("TipoRegistroPermutaPallet");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repGestaoDevolucao.BuscarPorCodigo(codigo);

                if (gestaoDevolucao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a devolução.");

                string caminho = ObterCaminhoArquivos(unitOfWork, typeof(Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo).Name);

                Servicos.DTO.CustomFile arquivo = arquivos.FirstOrDefault();
                string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNotaFiscal = null;

                string conteudoXML = string.Empty;
                if (extensaoArquivo == ".xml")
                {
                    string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}");
                    using (var stream = new FileStream(caminhoCompleto, FileMode.Open, FileAccess.Read))
                    {
                        XDocument xmlDoc = XDocument.Load(stream);
                        stream.Position = 0;
                        dadosNotaFiscal = serNFe.ObterDocumentoPorXML(stream, unitOfWork, false, false);

                        var jsonObject = new
                        {
                            dadosNotaFiscal.ValorTotal,
                            dadosNotaFiscal.Chave,
                            dadosNotaFiscal.DataEmissao,
                            dadosNotaFiscal.Numero
                        };

                        conteudoXML = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                    }
                }

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo gestaoDevolucaoPermutaPalletAnexo = new Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo()
                {
                    GestaoDevolucao = gestaoDevolucao,
                    Observacao = descricao ?? string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName))),
                    DataEnvioArquivo = DateTime.Now,
                    DadosXMLNota = conteudoXML,
                    TipoRegistroPermutaPallet = tipoRegistroPermutaPallet
                };

                repGestaoDevolucaoPermutaPalletAnexo.Inserir(gestaoDevolucaoPermutaPalletAnexo);

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao, gestaoDevolucaoPermutaPalletAnexo.GetChanges(), "Anexou arquivo de NFe na Permuta de pallet", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar importar os XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterGridRegistroDocumentosPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Observação", "Observacao", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Nome do arquivo", "NomeArquivo", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Extensão Arquivo", "TipoArquivo", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo do Registro", "TipoRegistroPermutaPallet", 7, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo repGestaoDevolucaoPermutaPalletAnexo = new Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo> registrosDocumentosPalletAnexo = repGestaoDevolucaoPermutaPalletAnexo.BuscarPorCodigoGestaoDevolucao(codigoGestaoDevolucao);

                var listaRetornar = (
                    from registro in registrosDocumentosPalletAnexo
                    select new
                    {
                        registro.Codigo,
                        registro.Observacao,
                        NomeArquivo = Path.GetFileNameWithoutExtension(registro.NomeArquivo),
                        TipoArquivo = registro.ExtensaoArquivo?.ToUpper() ?? string.Empty,
                        TipoRegistroPermutaPallet = registro.TipoRegistroPermutaPallet.ObterDescricao()
                    }).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(listaRetornar.Count());

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar importar os XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadRegistroDocumentoPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo repGestaoDevolucaoPermutaPalletAnexo = new Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo anexo = repGestaoDevolucaoPermutaPalletAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, false, "Anexo não encontrado.");

                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string caminho = this.ObterCaminhoArquivos(unitOfWork, typeof(Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo).Name);
                string arquivoFisico = anexo.GuidArquivo + extensao;

                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    return new JsonpResult(false, false, "Anexo não encontrado.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoFisico), $"application/{anexo.ExtensaoArquivo}", anexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfirmarRegistrosDocumentosPermutaPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");
                bool clienteRealizouPagamento = Request.GetBoolParam("ClientePagouNFe");


                Repositorio.Embarcador.Devolucao.GestaoDevolucao repGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo repGestaoDevolucaoPermutaPalletAnexo = new Repositorio.Embarcador.Devolucao.RegistroDocumentosPalletAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo anexo = repGestaoDevolucaoPermutaPalletAnexo.BuscarPorCodigoGestaoDevolucao(codigoGestaoDevolucao).FirstOrDefault();
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);

                if (clienteRealizouPagamento)
                {
                    anexo.PagamentoRealizado = true;
                    servicoMovimentacaoPallet.InformarDevolucaoPallet(gestaoDevolucao);
                    servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                }
                else
                {
                    //TODO: Quando desenvolver o agendamento de pallet mudar o tipo da devolução para agendamento de pallet aqui
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao confirmar o pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SolicitarCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");
                var observacaoCancelamento = Request.GetStringParam("ObservacaoCancelamento");

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorCodigoAsync(codigoGestaoDevolucao, true);

                if (gestaoDevolucao == null)
                    throw new ControllerException("Devolução não encontrada.");

                List<TipoGestaoDevolucao> tiposPermitidosAntesAprovacao = new() { TipoGestaoDevolucao.Permuta, TipoGestaoDevolucao.Descarte };
                if (tiposPermitidosAntesAprovacao.Contains(gestaoDevolucao.Tipo) && gestaoDevolucao.Aprovada)
                    throw new ControllerException("Devolução já aprovada. Não é possível solicitar cancelamento.");

                List<TipoGestaoDevolucao> tiposPermitidosAntesEmissaoDocumentos = new() { TipoGestaoDevolucao.Agendamento, TipoGestaoDevolucao.Coleta };
                if (tiposPermitidosAntesEmissaoDocumentos.Contains(gestaoDevolucao.Tipo) && gestaoDevolucao.CargaDevolucao != null)
                {
                    Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repositorioCargaCTe.BuscarPorCargaAsync(gestaoDevolucao.CargaDevolucao.Codigo);
                    Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = await repositorioCargaMDFe.BuscarPorCargaAsync(gestaoDevolucao.CargaDevolucao.Codigo);

                    if (cargaCTes.Count > 0 || cargaMDFes.Count > 0)
                        throw new ControllerException("Carga de Devolução já possui documentos emitidos. Não é possível solicitar cancelamento.");
                }

                await unitOfWork.StartAsync();

                if (gestaoDevolucao.DadosComplementares == null)
                    gestaoDevolucao.DadosComplementares = CriarGestaoDevolucaoDadosComplementares(gestaoDevolucao, unitOfWork);

                gestaoDevolucao.DadosComplementares.ObservacaoCancelamento = observacaoCancelamento;
                gestaoDevolucao.SituacaoDevolucao = SituacaoGestaoDevolucao.AnaliseCancelamento;

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, "Erro ao solicitar cancelamento!");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosAnaliseCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamados = await repositorioChamado.BuscarChamadosPorGestaodevolucaoAsync(new List<long>() { gestaoDevolucao.Codigo });

                if (gestaoDevolucao == null)
                    return new JsonpResult(new { Success = false, Message = "Devolução não encontrada." });

                bool existeChamadoAbertoOuEmTratativa = chamados.Any(chamado =>
                    chamado.Situacao == SituacaoChamado.Aberto ||
                    chamado.Situacao == SituacaoChamado.EmTratativa
                );

                string descricaoAtendimento = existeChamadoAbertoOuEmTratativa ? "Aberto" : "-";

                var retorno = new
                {
                    Success = true,
                    NFOrigem = gestaoDevolucao.NotaFiscalOrigem?.Numero.ToString() ?? "-",
                    NFDevolucao = gestaoDevolucao.NotaFiscalDevolucao?.Numero.ToString() ?? "-",
                    CargaDevolucao = gestaoDevolucao.CargaDevolucao?.CodigoCargaEmbarcador.ToString() ?? "-",
                    CargaOrigem = gestaoDevolucao.CargaOrigem?.CodigoCargaEmbarcador.ToString() ?? "-",
                    Origem = gestaoDevolucao.CargaOrigem?.Filial?.Descricao ?? "-",
                    Filial = gestaoDevolucao.Filial?.Descricao ?? "-",
                    Transportador = gestaoDevolucao.Transportador?.Descricao ?? "-",
                    Tomadores = gestaoDevolucao.CargaOrigem?.Pedidos != null && gestaoDevolucao.CargaOrigem.Pedidos.Any(pedido => pedido.Tomador != null)
                    ? string.Join(", ", gestaoDevolucao.CargaOrigem.Pedidos
                        .Where(pedido => pedido.Tomador != null)
                        .Select(pedido => pedido.Tomador?.NomeCNPJ ?? string.Empty))
                    : "-",
                    EtapaAtual = gestaoDevolucao.EtapaAtual?.Etapa.ObterDescricao().ToString() ?? "-",
                    Aprovado = !string.IsNullOrEmpty(gestaoDevolucao.AprovadaDescricao) ? gestaoDevolucao.AprovadaDescricao : "-",
                    Laudo = !string.IsNullOrEmpty(gestaoDevolucao.LaudoDescricao) ? gestaoDevolucao.LaudoDescricao : "-",
                    TipoDevolucao = gestaoDevolucao.Tipo.ObterDescricao().ToString(),
                    PendenciaFinanceira = gestaoDevolucao.ComPendenciaFinanceira ? "Sim" : "Não",
                    Atendimentos = descricaoAtendimento ?? "-",
                    ObservacaoCancelamento = !string.IsNullOrEmpty(gestaoDevolucao.DadosComplementares?.ObservacaoCancelamento)
                    ? gestaoDevolucao.DadosComplementares.ObservacaoCancelamento
                    : "-"
                };


                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(new { Success = false, Message = "Erro ao processar solicitação: " + ex.Message });
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarCancelamento()
        {
            using var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                var codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                var repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                var gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorCodigoAsync(codigoGestaoDevolucao, true);

                if (gestaoDevolucao == null)
                    return new JsonpResult(new { Success = false, Message = "Devolução não encontrada." });

                gestaoDevolucao.SituacaoDevolucao = SituacaoGestaoDevolucao.Cancelada;

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

                if (gestaoDevolucao.CargaDevolucao != null && gestaoDevolucao.CargaDevolucao.Codigo > 0)
                {
                    var repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    var carga = await repositorioCarga.BuscarPorCodigoAsync(gestaoDevolucao.CargaDevolucao.Codigo);

                    var cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                    {
                        Carga = carga,
                        MotivoCancelamento = Localization.Resources.Cargas.Carga.CancelarCarga,
                        TipoServicoMultisoftware = TipoServicoMultisoftware,
                        Usuario = this.Usuario
                    };

                    var cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, ConfiguracaoEmbarcador, unitOfWork);
                    Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);

                }

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(new { Success = false, Message = "Erro ao processar solicitação: " + ex.Message });
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReprovarCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigoGestaoDevolucao = Request.GetLongParam("CodigoGestaoDevolucao");

                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorCodigoAsync(codigoGestaoDevolucao, true);

                if (gestaoDevolucao == null)
                    return new JsonpResult(new { Success = false, Message = "Devolução não encontrada." });

                gestaoDevolucao.SituacaoDevolucao = SituacaoGestaoDevolucao.Ativa;

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(new { Success = false, Message = "Erro ao processar solicitação: " + ex.Message });
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfirmarDocumentacaoEntradaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");
                string observacao = Request.GetStringParam("ObservacaoDocumentacaoEntradaFiscal");
                bool adicionarNFesTransferenciaPallets = Request.GetBoolParam("AdicionarNFesTransferenciaPallet");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                gestaoDevolucao.Initialize();

                AlterarDadosNotaFiscalDevolucao(gestaoDevolucao);
                AlterarDadosNotaFiscalPermuta(gestaoDevolucao);

                ValidarAcaoPorEtapa(gestaoDevolucao, EtapaGestaoDevolucao.DocumentacaoEntradaFiscal, unitOfWork);

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares = CriarGestaoDevolucaoDadosComplementares(gestaoDevolucao, unitOfWork);

                gestaoDevolucaoDadosComplementares.Initialize();

                gestaoDevolucaoDadosComplementares.AdicionarNFesTransferenciaPallet = adicionarNFesTransferenciaPallets;
                gestaoDevolucao.DadosComplementares = gestaoDevolucaoDadosComplementares;

                if (!string.IsNullOrEmpty(observacao) && gestaoDevolucao.DadosComplementares != null)
                    gestaoDevolucao.DadosComplementares.ObservacaoDocumentacaoEntradaFiscal = observacao;

                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                await servicoGestaoDevolucao.AtualizarComDadosComplementaresAsync(gestaoDevolucao);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true, excecao.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarDadosNotaFiscalDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorCodigoAsync(codigoGestaoDevolucao, false);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                gestaoDevolucao.Initialize();

                return await AtualizarAsync(unitOfWork, AlterarDadosNotaFiscalDevolucao(gestaoDevolucao));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarDadosNotaFiscalPermuta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

                long codigoGestaoDevolucao = Request.GetLongParam("Codigo");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorCodigoAsync(codigoGestaoDevolucao, false);

                if (gestaoDevolucao == null)
                    return new JsonpResult("Não foi possível encontrar o registro de gestão de devolução");

                gestaoDevolucao.Initialize();

                return await AtualizarAsync(unitOfWork, AlterarDadosNotaFiscalPermuta(gestaoDevolucao));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion

        #region Métodos Privados     

        private Models.Grid.Grid ObterGridPesquisaNotasFiscaisOrigem(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data de emissão NF", "DataEmissaoNF", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Nº Nota Fiscal", "NFDeOrigem", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Chave", "Chave", 7, Models.Grid.Align.center, false);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "GestaoDevolucao/Pesquisa", "grid-gestao-devolucao-nfes");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            long codigoGestaoDevolucao = Request.GetLongParam("Codigo");

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);

            int totalRegistros = gestaoDevolucao.NotasFiscaisDeOrigem.Count();

            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<object> { });
                return grid;
            }


            var listaRetornar = (
                from nota in gestaoDevolucao.NotasFiscaisDeOrigem
                select new
                {
                    nota.Codigo,
                    NFDeOrigem = nota.XMLNotaFiscal.Numero,
                    DataEmissaoNF = nota.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                    Chave = nota.XMLNotaFiscal.Chave,
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaNotasFiscaisPallet(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoNF", false);
            grid.AdicionarCabecalho("CodigoCarga", false);
            grid.AdicionarCabecalho("TipoNF", false);
            grid.AdicionarCabecalho("Chave", "Chave", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Série", "Serie", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data de emissão NF", "StringDataEmissao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Transportador", "NomeCNPJTransportador", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Filial", "NomeCNPJFilial", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo NF", "DescricaoTipoNF", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Gerou Devolução?", "GerouDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Pendência", "DescricaoResponsavelPallet", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Destinatário", "NomeCNPJCliente", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Prazo geração Devolução", "PrazoGeracaoDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número NF", "NumeroNF", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número Atendimentos", "NumerosAtendimentos", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Motivo devoluções", "MotivoDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo da Devolução", "DevolucaoTotal", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número NF devolução", "NumeroNFDevolucao", 7, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Código da devolução", "CodigoGestaoDevolucao", 7, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipo de Tomador", "TipoTomadorDescricao", 7, Models.Grid.Align.center, false, false);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "GestaoDevolucao/ObterGridPesquisaNotasFiscaisPallet", "grid-gestao-devolucao-pallet");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork).BuscarPrimeiroRegistro();

            int totalRegistros = repositorioGestaoDevolucao.ContarConsultaNotaPalletGestaoDevolucao(filtroPesquisa);

            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<object> { });
                return grid;
            }

            IList<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet> gestaoDevolucoes = repositorioGestaoDevolucao.ConsultarNotaPalletGestaoDevolucao(filtroPesquisa, parametrosConsulta);

            var listaRetornar = (
                from nota in gestaoDevolucoes
                select new
                {
                    Codigo = nota.CodigoNF + Guid.NewGuid().ToString(),
                    nota.Chave,
                    nota.Serie,
                    nota.CodigoNF,
                    nota.CodigoCargaEmbarcador,
                    nota.CodigoCarga,
                    nota.StringDataEmissao,
                    nota.NomeCNPJFilial,
                    nota.NomeCNPJTransportador,
                    nota.DescricaoTipoNF,
                    GerouDevolucao = nota.DevolucaoGerada,
                    nota.DescricaoResponsavelPallet,
                    nota.NomeCNPJCliente,
                    nota.PrazoGeracaoDevolucao,
                    nota.NumeroNF,
                    nota.NumerosAtendimentos,
                    nota.MotivoDevolucao,
                    nota.DevolucaoTotal,
                    NumeroNFDevolucao = nota.NumeroNFDevolucao.ToString() ?? "",
                    CodigoGestaoDevolucao = nota.CodigoGestaoDevolucao.ToString() ?? "",
                    nota.TipoTomadorDescricao,
                    nota.TipoNF,
                }

            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private async Task<Models.Grid.Grid> ObterGridPesquisaNFesPallet(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data de emissão NF", "DataEmissao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número NF", "Numero", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Série NF", "Serie", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Chave", "Chave", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Quantidade", "QuantidadePallets", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Cód. Integração filial", "CodigoIntegracaoFilial", 7, Models.Grid.Align.center, false);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "GestaoDevolucao/ObterGridPesquisaNFesPallet", "grid-gestao-nfes");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            long codigoGestaoDevolucao = Request.GetLongParam("Codigo");

            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorCodigoAsync(codigoGestaoDevolucao, false);

            if (gestaoDevolucao == null)
                throw new ControllerException("Registro não encontrado");

            int totalRegistros = gestaoDevolucao.NotasFiscaisDeOrigem?.Count ?? 0;

            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<object> { });
                return grid;
            }

            var listaRetornar = (
                from nota in gestaoDevolucao.NotasFiscaisDeOrigem
                where nota.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet
                select new
                {
                    nota.XMLNotaFiscal.Chave,
                    nota.XMLNotaFiscal.Serie,
                    nota.XMLNotaFiscal.Codigo,
                    DataEmissao = nota.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy"),
                    nota.XMLNotaFiscal.Numero,
                    QuantidadePallets = (int)nota.XMLNotaFiscal.QuantidadePallets,
                    CodigoIntegracaoFilial = nota.XMLNotaFiscal.Filial?.CodigoFilialEmbarcador,
                    TotalQuantidadePallets = gestaoDevolucao.NotasFiscaisDeOrigem.Sum(nf => nf.XMLNotaFiscal.QuantidadePallets),
                }

            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VincularNotasPallets()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chaveNF = Request.GetStringParam("ChaveNFe");
                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("ObservacaoDocumentacaoEntradaFiscal");
                bool adicionarNFesTransferenciaPallets = Request.GetBoolParam("AdicionarNFesTransferenciaPallet");

                if (string.IsNullOrEmpty(chaveNF))
                    return new JsonpResult(false, "Necessário incluir a chave da nota.");

                var repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                var repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                var repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota = await repositorioNotaFiscal.BuscarPorChaveAsync(chaveNF, TipoNotaFiscalIntegrada.RemessaPallet);

                if (nota == null)
                    return new JsonpResult(false, "Nenhuma nota fiscal de pallet encontrada com a chave informada.");

                var gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigo);

                if (gestaoDevolucao == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                if (gestaoDevolucao.NotasFiscaisDeOrigem == null)
                    gestaoDevolucao.NotasFiscaisDeOrigem = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();

                if (gestaoDevolucao.NotasFiscaisDeOrigem.Any(n => n.Codigo == nota.Codigo))
                    return new JsonpResult(false, "Nota fiscal já vinculada.");


                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem repositorioNotaFiscalOrigem = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem(unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem notaFiscalOrigem = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem()
                {
                    XMLNotaFiscal = nota,
                    GestaoDevolucao = gestaoDevolucao
                };

                await repositorioNotaFiscalOrigem.InserirAsync(notaFiscalOrigem);


                gestaoDevolucao.DadosComplementares.AdicionarNFesTransferenciaPallet = adicionarNFesTransferenciaPallets;

                if (!string.IsNullOrEmpty(observacao) && gestaoDevolucao.DadosComplementares != null)
                    gestaoDevolucao.DadosComplementares.ObservacaoDocumentacaoEntradaFiscal = observacao;

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);
                await repositorioGestaoDevolucaoDadosComplementares.AtualizarAsync(gestaoDevolucao.DadosComplementares);

                await unitOfWork.CommitChangesAsync();

                gestaoDevolucao.NotasFiscaisDeOrigem.Add(notaFiscalOrigem);

                var totalPallets = notaFiscalOrigem.GestaoDevolucao.NotasFiscaisDeOrigem.Sum(nf => nf.XMLNotaFiscal.QuantidadePallets);

                return new JsonpResult(new { TotalQuantidadePallets = totalPallets }, true, "Nota vinculada com sucesso.");
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmErroAoExecutarAcao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task<Models.Grid.Grid> ObterGridPesquisaDevolucoesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

            grid.AdicionarCabecalho("Código Devolução", "Codigo", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("TipoDevolucao", false);
            grid.AdicionarCabecalho("SituacaoDevolucao", false);
            grid.AdicionarCabecalho("Etapas", false);
            grid.AdicionarCabecalho("DataEmissaoNFDevolucao", false);
            grid.AdicionarCabecalho("NF de Devolução", "NFDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Carga Devolução", "CargaDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("NF de Origem", "NFOrigem", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Carga Origem", "CargaOrigem", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tomadores", "Tomadores", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Filial", "Filial", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Etapa Atual", "EtapaAtualDescricao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Origem", "OrigemRecebimento", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Aprovado", "Aprovado", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Laudo?", "Laudo", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo de Devolução", "TipoDevolucaoDescricao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Fluxo de Devolução", "TipoFluxoGestaoDevolucaoDescricao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Pendência Financeira?", "ComPendenciaFinanceira", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Origem Geração", "OrigemGeracao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo Notas de Devolução", "TipoNotasDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Atendimentos", "Atendimentos", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Prazo para escolha do Tipo Devolução", "PrazoEscolhaTipoDevolucao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Finalização ERP", "ControleFinalizacaoDevolucaoDescricao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Status Atendimento", "StatusAtendimento", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Motivo do Atendimento", "MotivoAtendimento", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data do Atendimento", "DataAtendimento", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Ordem", "Ordem", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Remessa", "Remessa", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Volume", "Volume", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data do Agendamento", "DataAgendamento", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número do Laudo", "NLaudo", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data NFD", "DataNFD", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data Canhoto", "DataCanhoto", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("CPF/CNPJ Cliente", "ClienteCPFCNPJ", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Cliente", "ClienteNome", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Escritorio de Vendas", "EscritorioVendas", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Equipe de Vendas", "EquipeVendas", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Doc. Contábil", "DocContabil", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo Recusa", "TipoRecusa", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Aprovação", "Aprovacao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Custo", "Custo", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação Devolução", "SituacaoDevolucaoDescricao", 7, Models.Grid.Align.center, false);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "GestaoDevolucao/PesquisaDevolucoes", "grid-gestao-devolucao-devolucoes");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

            int totalRegistros = repositorioGestaoDevolucao.ContarConsulta(filtroPesquisa);

            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<object> { });
                return grid;
            }

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> gestaoDevolucoes = repositorioGestaoDevolucao.Consultar(filtroPesquisa, parametrosConsulta);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao> listaRetornar = await new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente, cancellationToken).MontarItensGrid(gestaoDevolucoes);

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaProdutos(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código Produto", "CodigoProduto", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Produto", "ProdutoDescricao", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("NF Devolução", "NFDevolucao", 7, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", 7, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Filial", "Filial", 7, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 7, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Emissão", "Emissao", 7, Models.Grid.Align.center, false, false);

            long codigoGestaoDevolucao = Request.GetLongParam("Codigo");

            Repositorio.Embarcador.Devolucao.GestaoDevolucaoProduto repositorioGestaoDevolucaoProduto = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoProduto(unitOfWork);
            List<int> codigosNotasFiscais = new List<int>();

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> listaProdutosNotaFiscal = repositorioGestaoDevolucaoProduto.BuscarPorGestaoDevolucao(codigoGestaoDevolucao);
            int totalRegistros = listaProdutosNotaFiscal.Count;

            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<object> { });
                return grid;
            }

            var listaRetornar = (
               from produtoNota in listaProdutosNotaFiscal
               select new
               {
                   Codigo = produtoNota.Codigo,
                   CodigoProduto = produtoNota.Produto?.CodigoProdutoEmbarcador ?? "NÃO CADASTRADO",
                   ProdutoDescricao = produtoNota.Produto?.Descricao ?? produtoNota.ProdutoDescricao,
                   Quantidade = produtoNota?.Quantidade ?? 0,
                   Valor = produtoNota.ValorTotal.ToString("n2"),
                   NFDevolucao = produtoNota.NotaFiscalDevolucao?.Numero.ToString() ?? "",
                   Tomador = produtoNota.NotaFiscalDevolucao?.Tomador?.NomeCNPJ ?? produtoNota.NotaFiscalDevolucao?.Destinatario?.NomeCNPJ ?? string.Empty,
                   Filial = produtoNota.NotaFiscalDevolucao?.Filial?.Descricao ?? string.Empty,
                   Transportador = produtoNota.NotaFiscalDevolucao?.Empresa?.NomeCNPJ ?? string.Empty,
                   Emissao = produtoNota.NotaFiscalDevolucao?.DataEmissao.ToString("dd/MM/yyyy")
               }
               ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private dynamic BuscarDadosGeracaoLaudo(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo;
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto> gestaoDevolucaoLaudoProdutos;

            if (gestaoDevolucao.Laudo == null)
                servicoGestaoDevolucao.GerarLaudo(gestaoDevolucao);

            gestaoDevolucaoLaudo = gestaoDevolucao.Laudo;
            gestaoDevolucaoLaudoProdutos = gestaoDevolucaoLaudo.Produtos.ToList();

            dynamic retorno = new
            {
                gestaoDevolucaoLaudo.Codigo,
                DevolucaoExclusivaPallet = gestaoDevolucao.TipoNotas == TipoNotasGestaoDevolucao.Pallet ? true : false,
                DataCriacao = gestaoDevolucaoLaudo.DataCriacao.ToString(),
                NumeroCompensacao = gestaoDevolucaoLaudo.NumeroCompensacao,
                DataCompensacao = gestaoDevolucaoLaudo.DataCompensacao.ToString(),
                Valor = gestaoDevolucaoLaudo.Valor.ToString(),
                Responsavel = new
                {
                    Codigo = gestaoDevolucaoLaudo.Responsavel?.Codigo.ToString() ?? string.Empty,
                    Descricao = gestaoDevolucaoLaudo.Responsavel?.Descricao ?? string.Empty
                },
                Transportador = new
                {
                    Codigo = gestaoDevolucaoLaudo.Transportador?.Codigo.ToString() ?? string.Empty,
                    Descricao = gestaoDevolucaoLaudo.Transportador?.Descricao ?? string.Empty
                },
                Veiculo = new
                {
                    Codigo = gestaoDevolucaoLaudo.Veiculo?.Codigo.ToString() ?? string.Empty,
                    Descricao = gestaoDevolucaoLaudo.Veiculo?.Descricao ?? string.Empty
                },
                DadosDevolucao = gestaoDevolucaoLaudoProdutos.Select(p => new
                {
                    p.Codigo,
                    Produto = new
                    {
                        Codigo = p.Produto?.Codigo ?? 0,
                        Descricao = p.Produto?.Descricao ?? string.Empty,
                        CodigoProdutoEmbarcador = p.Produto?.CodigoProdutoEmbarcador ?? p.ProdutoDescricao,
                    },
                    p.QuantidadeOrigem,
                    p.QuantidadeDevolvida,
                    p.QuantidadeAvariada,
                    p.ValorAvariado,
                    p.QuantidadeSobras,
                    p.ValorSobras,
                    p.QuantidadeSemCondicao,
                    p.ValorSemCondicao,
                    p.QuantidadeFalta,
                    p.ValorFalta,
                    p.QuantidadeDescarte,
                    p.QuantidadeManutencao
                }).ToList()
            };

            return retorno;
        }

        private dynamic BuscarDadosAprovacaoDataDescarga(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaMovimentacaoPallet consultaMovimentacaoPallet = new ConsultaMovimentacaoPallet
            {
                DataDescarregamento = gestaoDevolucao.DadosComplementares.DataDescarregamento,
                PeriodoDescarregamentoHoraInicio = gestaoDevolucao.DadosComplementares.PeriodoDescarregamento?.HoraInicio.ToString(@"hh\:mm"),
                PeriodoDescarregamentoHoraTermino = gestaoDevolucao.DadosComplementares.PeriodoDescarregamento?.HoraTermino.ToString(@"hh\:mm")
            };

            dynamic retorno = new
            {
                gestaoDevolucao.Codigo,
                DataDaSolicitacao = gestaoDevolucao.DadosComplementares?.DataDaSolicitacao?.ToString() ?? "-",
                DataCarregamento = gestaoDevolucao.DadosComplementares?.DataCarregamento?.ToString() ?? "-",
                DataDescarregamento = consultaMovimentacaoPallet.DataAgendamentoDevolucao,
                ObservacaoAgendamento = gestaoDevolucao.DadosComplementares?.ObservacaoAgendamento ?? string.Empty,
                ObservacaoAnaliseAgendamento = gestaoDevolucao.DadosComplementares?.ObservacaoAnaliseAgendamento ?? "-",
                DataAnaliseAgendamento = gestaoDevolucao.DadosComplementares?.DataAnaliseAgendamento.ToString() ?? string.Empty,
                Transportador = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.Transportador?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.Transportador?.Descricao ?? "-"
                },
                Veiculo = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.Veiculo?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.Veiculo?.Descricao ?? "-"
                },
                Motorista = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.Motorista?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.Motorista?.Descricao ?? "-"
                },
                RemetenteAgendamento = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.RemetenteAgendamento?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.RemetenteAgendamento?.Descricao ?? "-"
                },
                Destinatario = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.DestinatarioAgendamento?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.DestinatarioAgendamento?.Descricao ?? "-"
                },
                PeriodoDescarregamento = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.PeriodoDescarregamento?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.PeriodoDescarregamento?.Descricao ?? "-"
                },
                TipoOperacao = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.TipoOperacao?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.TipoOperacao?.Descricao ?? string.Empty
                },
                TipoDeCarga = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.TipoDeCarga?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.TipoDeCarga?.Descricao ?? string.Empty
                },
            };

            return retorno;
        }

        private dynamic BuscarDadosAgendamento(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal primeiraNotaOrigem = gestaoDevolucao.NotaFiscalOrigem;

            string dataDescarregamento = string.Empty;
            if (gestaoDevolucao.DadosComplementares != null && gestaoDevolucao.DadosComplementares.PeriodoDescarregamento != null)
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaMovimentacaoPallet consultaMovimentacaoPallet = new ConsultaMovimentacaoPallet
                {
                    DataDescarregamento = gestaoDevolucao.DadosComplementares.DataDescarregamento,
                    PeriodoDescarregamentoHoraInicio = gestaoDevolucao.DadosComplementares.PeriodoDescarregamento.HoraInicio.ToString(@"hh\:mm"),
                    PeriodoDescarregamentoHoraTermino = gestaoDevolucao.DadosComplementares.PeriodoDescarregamento.HoraTermino.ToString(@"hh\:mm")
                };
                dataDescarregamento = consultaMovimentacaoPallet.DataAgendamentoDevolucao;
            }

            dynamic retorno = new
            {
                CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                DescricaoDataDescarregamento = dataDescarregamento,
                ObservacaoAgendamento = gestaoDevolucao.DadosComplementares?.ObservacaoAgendamento ?? string.Empty,
                DataCarregamento = gestaoDevolucao.DadosComplementares?.DataCarregamento.ToString() ?? string.Empty,
                DataDescarregamento = gestaoDevolucao.DadosComplementares?.DataDescarregamento.ToString() ?? string.Empty,
                ObservacaoAnaliseAgendamento = gestaoDevolucao.DadosComplementares?.ObservacaoAnaliseAgendamento ?? "-",
                DataAnaliseAgendamento = gestaoDevolucao.DadosComplementares?.DataAnaliseAgendamento.ToString() ?? string.Empty,
                gestaoDevolucao.SituacaoDevolucao,
                Motorista = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.Motorista?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.Motorista?.Nome ?? string.Empty
                },
                Veiculo = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.Veiculo?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.Veiculo?.Placa ?? string.Empty
                },
                PeriodoDescarregamento = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.PeriodoDescarregamento?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.PeriodoDescarregamento?.Descricao ?? string.Empty
                },
                Remetente = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.RemetenteAgendamento?.Codigo ?? primeiraNotaOrigem?.Destinatario.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.RemetenteAgendamento?.Descricao ?? primeiraNotaOrigem?.Destinatario.Descricao ?? string.Empty
                },
                Destinatario = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.DestinatarioAgendamento?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.DestinatarioAgendamento?.Descricao ?? string.Empty
                },
                TipoDeCarga = new
                {
                    Codigo = gestaoDevolucao.DadosComplementares?.TipoDeCarga?.Codigo ?? 0,
                    Descricao = gestaoDevolucao.DadosComplementares?.TipoDeCarga?.Descricao ?? string.Empty
                },
            };

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao();

            filtroPesquisa.Carga = Request.GetStringParam("Carga");
            filtroPesquisa.CargaDevolucao = Request.GetStringParam("CargaDevolucao");
            filtroPesquisa.NFOrigem = Request.GetIntParam("NFOrigem");
            filtroPesquisa.NFDevolucao = Request.GetIntParam("NFDevolucao");
            filtroPesquisa.Transportadores = Request.GetListParam<int>("Transportador");
            filtroPesquisa.Filiais = Request.GetListParam<int>("Filial");
            filtroPesquisa.OrigemRecebimento = Request.GetNullableEnumParam<OrigemGestaoDevolucao>("OrigemGestaoDevolucao");
            filtroPesquisa.DevolucaoGerada = Request.GetNullableBoolParam("DevolucaoGerada");
            filtroPesquisa.DataEmissaoNFInicial = Request.GetDateTimeParam("DataEmissaoNFInicial");
            filtroPesquisa.DataEmissaoNFFinal = Request.GetDateTimeParam("DataEmissaoNFFinal");
            filtroPesquisa.TipoServicoMultisoftware = TipoServicoMultisoftware;
            filtroPesquisa.TipoNotasGestaoDevolucao = Request.GetListEnumParam<TipoNotasGestaoDevolucao>("TipoNotasGestaoDevolucao");
            filtroPesquisa.TipoNotasFiscais = Request.GetListEnumParam<TipoNotaFiscalIntegrada>("TipoNotasFiscais");
            filtroPesquisa.TipoFluxoGestaoDevolucao = Request.GetNullableEnumParam<TipoFluxoGestaoDevolucao>("TipoFluxoGestaoDevolucao");
            filtroPesquisa.SituacaoDevolucao = Request.GetListEnumParam<SituacaoGestaoDevolucao>("SituacaoDevolucao");
            filtroPesquisa.EscritorioVendas = Request.GetStringParam("EscritorioVendas");
            filtroPesquisa.EquipeVendas = Request.GetStringParam("EquipeVendas");
            filtroPesquisa.TipoGestaoDevolucao = Request.GetListEnumParam<TipoGestaoDevolucao>("TipoGestaoDevolucao");
            filtroPesquisa.Cliente = Request.GetDoubleParam("Cliente");
            filtroPesquisa.Etapas = Request.GetListEnumParam<EtapaGestaoDevolucao>("EtapaAtual");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtroPesquisa.Transportadores.Add(this.Empresa.Codigo);
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                filtroPesquisa.Cliente = this.Usuario.Cliente.CPF_CNPJ;
            return filtroPesquisa;
        }

        private Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares CriarGestaoDevolucaoDadosComplementares(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares = gestaoDevolucao.DadosComplementares ?? repositorioGestaoDevolucaoDadosComplementares.BuscarPorGestaoDevolucao(gestaoDevolucao.Codigo) ?? new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares();

            gestaoDevolucaoDadosComplementares.GestaoDevolucao = gestaoDevolucao;
            gestaoDevolucao.DadosComplementares = gestaoDevolucaoDadosComplementares;

            if (gestaoDevolucaoDadosComplementares.Codigo == 0)
                repositorioGestaoDevolucaoDadosComplementares.Inserir(gestaoDevolucaoDadosComplementares);

            return gestaoDevolucaoDadosComplementares;
        }

        private bool VerificarExibicaoEtratoLaudo(SituacaoIntegracao SituacaoIntegracao, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            if (SituacaoIntegracao == SituacaoIntegracao.Integrado && (gestaoDevolucao.Laudo != null && gestaoDevolucao.Laudo.Valor > 0 && gestaoDevolucao.Laudo.DataVencimento != null && gestaoDevolucao.Laudo.Transportador != null))
                return true;

            return false;
        }

        private void ValidarAcaoPorEtapa(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, EtapaGestaoDevolucao etapa, Repositorio.UnitOfWork unitOfWork)
        {
            if (gestaoDevolucao.EtapaAtual.Etapa != etapa && gestaoDevolucao.SituacaoDevolucao == SituacaoGestaoDevolucao.AnaliseCancelamento)
            {
                new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente).AtualizarGestaoDevolucaoNaGrid(gestaoDevolucao);
                throw new ControllerException($"Ação não permitida para Etapa atual da Gestão Devolução: {gestaoDevolucao.Codigo}", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
            }
        }

        private dynamic BuscarDadosGeracaoCargaDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDevolucao = gestaoDevolucao.CargaDevolucao;
            int codigoCargaDevolucao = cargaDevolucao?.Codigo ?? 0;
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatariosDaCarga(codigoCargaDevolucao);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCargaECentroDescarregamento(codigoCargaDevolucao, centrosDescarregamento?.FirstOrDefault()?.Codigo ?? 0);

            if (cargaDevolucao != null && cargaDevolucao.SituacaoCarga != SituacaoCarga.Nova && cargaDevolucao.SituacaoCarga != SituacaoCarga.Cancelada)
            {
                servicoCarga.AvancarEtapaGestaoDevolucao(gestaoDevolucao.CargaDevolucao, Auditado, Cliente);
            }

            dynamic retorno = new
            {
                CodigoGestaoDevolucao = gestaoDevolucao.Codigo,
                CodigoCargaDevolucao = codigoCargaDevolucao,
                CodigoJanelaDescarregamento = codigoCargaDevolucao,
                SituacaoCargaJanelaDescarregamento = cargaJanelaDescarregamento?.Situacao.ObterDescricao() ?? "-",
                CorSituacaoCargaJanelaDescarregamento = cargaJanelaDescarregamento?.Situacao.ObterCorLinha() ?? "-",
            };

            return retorno;
        }

        private void AvancarEtapaAprovacaoPermutaPallet(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);

            Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(unitOfWork);
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares = CriarGestaoDevolucaoDadosComplementares(gestaoDevolucao, unitOfWork);

            gestaoDevolucaoDadosComplementares.ObservacaoAprovacao = "Aprovada automáticamente via fluxo de Permuta de Pallet";

            servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao);
            gestaoDevolucao.Aprovada = true;

            Servicos.Auditoria.Auditoria.Auditar(Auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), $"Etapa de aprovação avançada automáticamente via fluxo de Permuta de Pallet", unitOfWork);

            repositorioGestaoDevolucaoDadosComplementares.Atualizar(gestaoDevolucaoDadosComplementares);
        }

        protected string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork, string entidade)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", entidade });
        }

        private async Task<IActionResult> AtualizarAsync(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, Auditado, Cliente);

            await unitOfWork.StartAsync();

            await servicoGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

            await unitOfWork.CommitChangesAsync();

            return new JsonpResult(true);
        }

        private Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao AlterarDadosNotaFiscalDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            gestaoDevolucao.SerieNotaFiscalDevolucao = Request.GetNullableStringParam(nameof(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao.SerieNotaFiscalDevolucao));
            gestaoDevolucao.NumeroNotaFiscalDevolucao = Request.GetNullableIntParam(nameof(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao.NumeroNotaFiscalDevolucao));

            return gestaoDevolucao;
        }

        private Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao AlterarDadosNotaFiscalPermuta(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            gestaoDevolucao.SerieNotaFiscalPermuta = Request.GetNullableStringParam(nameof(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao.SerieNotaFiscalPermuta));
            gestaoDevolucao.NumeroNotaFiscalPermuta = Request.GetNullableIntParam(nameof(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao.NumeroNotaFiscalPermuta));

            return gestaoDevolucao;
        }
        #endregion
    }
}