using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/MotivoChamado")]
    public class MotivoChamadoController : BaseController
    {
        #region Construtores

        public MotivoChamadoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCausasMotivoChamado()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa = ObterFiltrosPesquisaCausas();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Chamados.MotivoChamadoCausas repositorioTipoOcorrenciaCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> MotivoChamadoCausas = repositorioTipoOcorrenciaCausas.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioTipoOcorrenciaCausas.ContarConsulta(filtrosPesquisa));

                var lista = (from p in MotivoChamadoCausas
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                             }).ToList();

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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoDoMotivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repMotivoChamado.BuscarPorCodigo(codigo);

                if (motivoChamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    motivoChamado.Codigo,
                    motivoChamado.ChamadoDeveSerAbertoPeloEmbarcador,
                    GerarCargaDevolucao = motivoChamado.GerarCargaDevolucaoSeAprovado,
                    GerarValePallet = motivoChamado.GerarValePalletSeAprovado,
                    motivoChamado.ExigeValor,
                    motivoChamado.ExigeValorNaLiberacao,
                    motivoChamado.ObrigarMotoristaInformarMultiMobile,
                    motivoChamado.DisponibilizaParaReeentrega,
                    motivoChamado.ValidarDuplicidade,
                    motivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga,
                    motivoChamado.ValidarDuplicidadePorDestinatario,
                    Devolucao = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao,
                    Reentrega = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega,
                    Retencao = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao,
                    RetencaoOrigem = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem,
                    MotivoDevolucao = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao,
                    MotivoReentrega = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega,
                    MotivoRetencao = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao,
                    MotivoRetencaoOrigem = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem,
                    MotivoReentregaMesmaCarga = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga,
                    motivoChamado.ReferentePagamentoDescarga,
                    motivoChamado.PermiteInformarDesconto,
                    motivoChamado.PermiteEstornarAtendimento,
                    PermiteSelecionarMotorista = motivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista || motivoChamado.PermiteAdicionarValorComoDespesaMotorista || motivoChamado.PermiteAdicionarValorComoDescontoMotorista || motivoChamado.PermiteInformarMotoristaNoAtendimento,
                    motivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista,
                    motivoChamado.PermiteAdicionarValorComoDespesaMotorista,
                    motivoChamado.PermiteRetornarParaAjuste,
                    motivoChamado.ObrigarAnexo,
                    motivoChamado.PermiteAdicionarValorComoDescontoMotorista,
                    motivoChamado.PermiteAtendimentoSemCarga,
                    motivoChamado.PermiteAlterarDatasCargaEntrega,
                    motivoChamado.BuscaContaBancariaDestinatario,
                    motivoChamado.ObrigarInformarResponsavelAtendimento,
                    motivoChamado.NaoPermitirLancarAtendimentoSemAcertoAberto,
                    motivoChamado.PermitirLancarAtendimentoEmCargasComDocumentoEmitido,
                    PessoaSeraInformadaGeracaoPagamento = motivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista ? motivoChamado.PagamentoMotoristaTipo.PessoaSeraInformadaGeracaoPagamento : false,
                    motivoChamado.PermiteInserirJustificativaOcorrencia,
                    motivoChamado.InformarQuantidade,
                    motivoChamado.ObrigarInformacaoLote,
                    motivoChamado.ObrigarDataCritica,
                    motivoChamado.ObrigarRealMotivo,
                    motivoChamado.ExigeInformarModeloVeicularAberturaChamado,
                    motivoChamado.TratativaDeveSerConfirmadaPeloCliente,
                    motivoChamado.NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao,
                    motivoChamado.IntegrarComDansales,
                    motivoChamado.InformarCodigoSIF,
                    motivoChamado.PermitirFreteRetornoDevolucao,
                    motivoChamado.ValidarEscaladaAtendimentoUsuarioResponsavel,
                    motivoChamado.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise,
                    PermitirInformarCausadorOcorrencia = motivoChamado?.PermitirInformarCausadorOcorrencia ?? false,
                    motivoChamado.ValidaValorCarga,
                    motivoChamado.ValidaValorDescarga,
                    GerarGestaoDeDevolucao = motivoChamado.ListarNotasParaGeracaoGestaoDeDevolucao,
                    PossibilitarInclusaoAnexoAoEscalarAtendimento = motivoChamado?.PossibilitarInclusaoAnexoAoEscalarAtendimento ?? false,
                    ApresentarValorPesoDaCarga = motivoChamado.TipoOcorrencia?.ApresentarValorPesoDaCarga ?? false,
                    motivoChamado.PermiteInformarNFD,
                    motivoChamado.ObrigarPreenchimentoNFD,
                    motivoChamado.EnviarEmailParaTransportadorAoCancelarChamado,
                    motivoChamado.EnviarEmailParaTransportadorAoAlterarChamado,
                    motivoChamado.EnviarEmailParaTransportadorAoFinalizarChamado,
                    motivoChamado.NumeroCriticidadeAtendimento,
                    motivoChamado.BloquearParadaAppTrizy,
                    motivoChamado.HabilitarSenhaDevolucao,
                    motivoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador,
                    motivoChamado.HabilitarClassificacaoCriticos,
                    motivoChamado.HabilitarEstadia,
                    motivoChamado.PermitirAtualizarInformacoesPedido,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoArvore repMotivoChamadoData = new Repositorio.Embarcador.Chamados.MotivoChamadoArvore(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repMotivoChamadoGatilhos = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga repMotivoChamadoGatilhosNaCarga = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoCausas repMotivoChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repositorioMotivoChamadoTipoCriticidade = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repMotivoChamado.BuscarPorCodigo(codigo);
                if (motivoChamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> motivoChamadoArvore = repMotivoChamadoData.BuscarPorCodigoMotivoChamado(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> motivoChamadoGatilhos = repMotivoChamadoGatilhos.BuscarPorMotivoChamado(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga> motivoChamadoGatilhosNaCarga = repMotivoChamadoGatilhosNaCarga.BuscarPorMotivoChamado(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> motivoChamadoCausas = repMotivoChamadoCausas.BuscarPorMotivoChamado(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento> tipoCriticidadeAtendimentos = repositorioMotivoChamadoTipoCriticidade.BuscarPorCodigoMotivoChamado(codigo).Result;

                var retorno = new
                {
                    motivoChamado.Codigo,
                    motivoChamado.Descricao,
                    motivoChamado.CodigoIntegracao,
                    motivoChamado.Status,
                    motivoChamado.ChamadoDeveSerAbertoPeloEmbarcador,
                    motivoChamado.GerarCargaDevolucaoSeAprovado,
                    motivoChamado.GerarValePalletSeAprovado,
                    motivoChamado.Assunto,
                    motivoChamado.ConteudoEmail,
                    motivoChamado.Observacao,
                    motivoChamado.ExigeValor,
                    motivoChamado.ExigeValorNaLiberacao,
                    motivoChamado.ObrigarMotoristaInformarMultiMobile,
                    motivoChamado.DisponibilizaParaReeentrega,
                    motivoChamado.TipoMotivoAtendimento,
                    motivoChamado.ExigeFotoAbertura,
                    motivoChamado.ExigeQRCodeAbertura,
                    motivoChamado.GerarOcorrenciaAutomaticamente,
                    motivoChamado.GerarCTeComValorIgualCTeAnterior,
                    motivoChamado.CalcularOcorrenciaPorTabelaFrete,
                    motivoChamado.ValidarDuplicidade,
                    motivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga,
                    motivoChamado.ValidarDuplicidadePorDestinatario,
                    TipoOcorrencia = new { Codigo = motivoChamado.TipoOcorrencia?.Codigo ?? 0, Descricao = motivoChamado.TipoOcorrencia?.Descricao ?? string.Empty, },
                    TipoOcorrenciaQuebraRegra = motivoChamado.TipoOcorrencia?.OcorrenciaParaQuebraRegraPallet ?? false,
                    motivoChamado.ReferentePagamentoDescarga,
                    motivoChamado.PermiteInformarDesconto,
                    motivoChamado.PermiteEstornarAtendimento,
                    motivoChamado.ExigeAnaliseParaOperacao,
                    motivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista,
                    motivoChamado.PermiteAdicionarValorComoDespesaMotorista,
                    motivoChamado.PermiteAdicionarValorComoDescontoMotorista,
                    motivoChamado.HabilitarPerfilAcessoEnvioEmail,
                    motivoChamado.GerarAcrescimoDescontoContratoFrete,
                    motivoChamado.InformarCodigoSIF,
                    motivoChamado.ValidarEscaladaAtendimentoUsuarioResponsavel,
                    PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise = motivoChamado?.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise ?? false,
                    PermitirInformarCausadorOcorrencia = motivoChamado?.PermitirInformarCausadorOcorrencia ?? false,
                    motivoChamado.PermitirFreteRetornoDevolucao,
                    PagamentoMotoristaTipo = new { Codigo = motivoChamado.PagamentoMotoristaTipo?.Codigo ?? 0, Descricao = motivoChamado.PagamentoMotoristaTipo?.Descricao ?? string.Empty, },
                    Justificativa = new { Codigo = motivoChamado.Justificativa?.Codigo ?? 0, Descricao = motivoChamado.Justificativa?.Descricao ?? string.Empty, },
                    FornecedorDespesa = new { Codigo = motivoChamado.FornecedorDespesa?.Codigo ?? 0, Descricao = motivoChamado.FornecedorDespesa?.Descricao ?? string.Empty, },
                    JustificativaAcrescimoDescontoContratoFrete = new { Codigo = motivoChamado.JustificativaAcrescimoDescontoContratoFrete?.Codigo ?? 0, Descricao = motivoChamado.JustificativaAcrescimoDescontoContratoFrete?.Descricao ?? string.Empty, },
                    TipoTransportadorAcrescimoDescontoContratoFrete = motivoChamado.TipoTransportadorAcrescimoDesconto,
                    motivoChamado.PermiteRetornarParaAjuste,
                    Datas = (
                            from obj in motivoChamado.Datas
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.Obrigatorio,
                                obj.Status
                            }
                        ).ToList(),
                    motivoChamado.ObrigatorioTerDiariaAutomatica,
                    motivoChamado.BloquearAprovacaoValoresSuperioresADiariaAutomatica,
                    motivoChamado.DiasLimiteAberturaAposDiariaAutomatica,
                    motivoChamado.LocalFreeTime,
                    motivoChamado.ObrigarAnexo,
                    motivoChamado.PermiteAtendimentoSemCarga,
                    motivoChamado.PermiteAlterarDatasCargaEntrega,
                    motivoChamado.BuscaContaBancariaDestinatario,
                    motivoChamado.ObrigarInformarResponsavelAtendimento,
                    motivoChamado.NaoPermitirLancarAtendimentoSemAcertoAberto,
                    motivoChamado.PermitirLancarAtendimentoEmCargasComDocumentoEmitido,
                    motivoChamado.PermiteInserirJustificativaOcorrencia,
                    motivoChamado.InformarQuantidade,
                    motivoChamado.PossibilitarInclusaoAnexoAoEscalarAtendimento,
                    motivoChamado.ConsiderarHorasDiasUteis,
                    motivoChamado.EnviarEmailParaTransportadorAoCancelarChamado,
                    motivoChamado.EnviarEmailParaTransportadorAoAlterarChamado,
                    motivoChamado.EnviarEmailParaTransportadorAoFinalizarChamado,
                    motivoChamado.NumeroCriticidadeAtendimento,
                    Arvore = (
                        from obj in motivoChamadoArvore
                        select new
                        {
                            key = obj.Key,
                            obj.Pergunta,
                            obj.Pai,
                            obj.Resposta,
                            obj.StatusFinalizacaoAtendimento
                        }
                    ),
                    GatilhosTempoList = (from obj in motivoChamadoGatilhos
                                         select new
                                         {
                                             obj.Codigo,
                                             obj.Nivel,
                                             obj.Tempo,
                                             CodigoSetor = obj?.Setor?.Codigo ?? 0,
                                             DescricaoSetor = obj?.Setor?.Descricao ?? string.Empty
                                         }).ToList(),
                    GatilhosNaCarga = (from obj in motivoChamadoGatilhosNaCarga
                                       select new
                                       {
                                           obj.Codigo,
                                           Descricao = obj.Gatilho.ObterDescricao(),
                                           obj.Gatilho
                                       }).ToList(),
                    motivoChamado.ObrigarInformacaoLote,
                    motivoChamado.ObrigarDataCritica,
                    motivoChamado.ObrigarRealMotivo,
                    motivoChamado.ExigeInformarModeloVeicularAberturaChamado,
                    motivoChamado.TratativaDeveSerConfirmadaPeloCliente,
                    motivoChamado.NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao,
                    motivoChamado.IntegrarComDansales,
                    motivoChamado.PermiteTrocarTransportadora,
                    motivoChamado.AtendimentoPorLote,
                    motivoChamado.PermiteInformarNFD,
                    motivoChamado.ObrigarPreenchimentoNFD,
                    motivoChamado.PermiteInformarQuantidadeParaCalculo,
                    motivoChamado.PermiteInformarQuantidadeVolumesParaCalculo,
                    motivoChamado.PermiteInformarPesoParaCalculo,
                    motivoChamado.PermiteInformarDataRetornoAposEstadia,
                    motivoChamado.PermiteInformarOrdemInterna,
                    motivoChamado.PermiteInformarMotoristaNoAtendimento,
                    motivoChamado.ValidaValorCarga,
                    motivoChamado.ValidaValorDescarga,
                    GerarGestaoDeDevolucao = motivoChamado.ListarNotasParaGeracaoGestaoDeDevolucao,
                    motivoChamado.TipoQuebraRegraPallet,
                    Genero = new { Codigo = motivoChamado.Genero?.Codigo ?? 0, Descricao = motivoChamado.Genero?.Descricao ?? string.Empty },
                    AreaEnvolvida = new { Codigo = motivoChamado.AreaEnvolvida?.Codigo ?? 0, Descricao = motivoChamado.AreaEnvolvida?.Descricao ?? string.Empty },
                    GrupoMotivoChamado = new { Codigo = motivoChamado.GrupoMotivoChamado?.Codigo ?? 0, Descricao = motivoChamado.GrupoMotivoChamado?.Descricao ?? string.Empty },
                    TiposIntegracao = (
                        from obj in motivoChamado.TipoIntegracao
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }).ToList(),
                    Causas = (from obj in motivoChamadoCausas
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  Ativo = (obj?.Ativo ?? false) ? "Sim" : "Não"
                              }).ToList(),
                    motivoChamado.BloquearParadaAppTrizy,
                    motivoChamado.HabilitarSenhaDevolucao,
                    motivoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador,
                    motivoChamado.HabilitarEstadia,
                    motivoChamado.PermitirAtualizarInformacoesPedido,
                    motivoChamado.HabilitarClassificacaoCriticos,
                    TiposCriticidade = (from obj in tipoCriticidadeAtendimentos
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Tipo,
                                            TipoDescricao = obj.DescricaoTipo,
                                            obj.Conteudo,
                                            obj.Ativo,
                                            AtivoDescricao = obj.DescricaoAtivo
                                        }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracao = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = new Dominio.Entidades.Embarcador.Chamados.MotivoChamado();

                PreencheEntidade(motivoChamado, unitOfWork);

                if (!ValidaEntidade(motivoChamado, out string erro))
                    return new JsonpResult(false, true, erro);

                repMotivoChamado.Inserir(motivoChamado, Auditado);

                SalvarDatas(motivoChamado, unitOfWork);
                SalvarTipoIntegracao(motivoChamado, unitOfWork);
                SalvarCausas(motivoChamado, unitOfWork);
                SalvarGatilhosNaCarga(motivoChamado, unitOfWork);
                await SalvarTiposCriticidade(motivoChamado, unitOfWork, cancellationToken);

                if (configuracao.HabilitarArvoreDecisaoEscalationList)
                {
                    this.SalvarArvore(motivoChamado, unitOfWork);
                    this.SalvarGatilhosTempoList(motivoChamado, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracao = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repMotivoChamado.BuscarPorCodigo(codigo, true);

                if (motivoChamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencheEntidade(motivoChamado, unitOfWork);

                if (!ValidaEntidade(motivoChamado, out string erro))
                    return new JsonpResult(false, true, erro);

                repMotivoChamado.Atualizar(motivoChamado, Auditado);

                SalvarDatas(motivoChamado, unitOfWork);
                SalvarTipoIntegracao(motivoChamado, unitOfWork);
                SalvarCausas(motivoChamado, unitOfWork);
                SalvarGatilhosNaCarga(motivoChamado, unitOfWork);
                await SalvarTiposCriticidade(motivoChamado, unitOfWork, cancellationToken);
                if (configuracao.HabilitarArvoreDecisaoEscalationList)
                {
                    this.SalvarArvore(motivoChamado, unitOfWork);
                    this.SalvarGatilhosTempoList(motivoChamado, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, Localization.Resources.Chamado.MotivoChamado.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculo);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repMotivoChamado.BuscarPorCodigo(codigo);

                if (motivoChamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repMotivoChamado.Deletar(motivoChamado, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Chamado.MotivoChamado.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Chamado.MotivoChamado.OcorreUmaFalhaAoRemoverDados);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTiposMotivoAtendimento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento> tipos = null;

                if (!string.IsNullOrWhiteSpace(Request.Params("Tipos")))
                    tipos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento>>(Request.Params("Tipos"));

                Repositorio.Embarcador.Chamados.TiposMotivoAtendimento repTiposMotivoAtendimento = new Repositorio.Embarcador.Chamados.TiposMotivoAtendimento(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Chamados.TiposMotivoAtendimento> tiposMotivoAtendimento = repTiposMotivoAtendimento.BuscarPorTipos(tipos);

                return new JsonpResult((from obj in tiposMotivoAtendimento
                                        orderby obj.TipoMotivoAtendimento
                                        select new
                                        {
                                            Codigo = obj.TipoMotivoAtendimento,
                                            Descricao = obj.TipoMotivoAtendimento.ObterDescricao()
                                        }).ToList()); ;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Chamado.MotivoChamado.OcorreuUmaFalhaAoObterTipos);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarArvoreAtendimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("CodigoMotivo");
                int codigoAnalisis = Request.GetIntParam("CodigoAnalisis");
                int codigoCausa = Request.GetIntParam("CodigoCausa");

                this.SalvarArvoreRespondida(codigo, codigoAnalisis, codigoCausa, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException exception)
            {
                return new JsonpResult(false, exception.Message);
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro(exception);
                return new JsonpResult(false, exception.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarArvorePorCodigoMotivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.MotivoChamadoArvore repMotivoChamadoData = new Repositorio.Embarcador.Chamados.MotivoChamadoArvore(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoA = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalisisArvore repoChamadoAnalisiArvore = new Repositorio.Embarcador.Chamados.ChamadoAnalisisArvore(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoCausas repoMotivoChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);

                int codigo = Request.GetIntParam("CodigoMotivo");
                int codigoAnalisis = Request.GetIntParam("CodigoAnalisis");
                int codigoCausa = Request.GetIntParam("CodigoCausa");

                if (codigo == 0)
                    throw new ServicoException(string.Format(Localization.Resources.Chamado.MotivoChamado.MotivoComCodigoNaoExiste, codigo));

                if (codigoAnalisis == 0)
                    throw new ServicoException(string.Format(Localization.Resources.Chamado.MotivoChamado.MotivoComCodigoNaoExiste, codigo));

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> arvorePadrao = repMotivoChamadoData.BuscarPorCodigoMotivoChamado(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore> listaArvoreAnalisis = repoChamadoAnalisiArvore.BuscarPorMotivoChamado(codigo, codigoAnalisis);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamadoAnalise = repChamado.BuscarPorCodigo(codigoAnalisis);
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas> chamadoCausas = repoMotivoChamadoCausas.BuscarPorMotivosChamadoAtivos(codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore chamadoAnalisisSelecionado = repoChamadoAnalisiArvore.BuscarChamadoSelecionado(codigoAnalisis);

                if (listaArvoreAnalisis != null && listaArvoreAnalisis.Count == 0 && arvorePadrao != null)
                {
                    listaArvoreAnalisis = (from obj in arvorePadrao
                                           select new Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore
                                           {
                                               Key = obj.Key,
                                               MotivoChamado = obj.MotivoChamado,
                                               Pai = obj.Pai,
                                               Pergunta = obj.Pergunta,
                                               Resposta = obj.Resposta,
                                               Situacao = obj.Situacao,
                                               StatusFinalizacaoAtendimento = obj.StatusFinalizacaoAtendimento,
                                               Chamado = chamadoAnalise
                                           }).ToList();
                    foreach (var itemArvore in listaArvoreAnalisis)
                        repoChamadoAnalisiArvore.Inserir(itemArvore);
                }

                if (listaArvoreAnalisis == null)
                    throw new ServicoException(Localization.Resources.Chamado.MotivoChamado.NaoExisteArvoreCadastradaNoMotivoAtendimento);

                dynamic retorno = new
                {
                    ListaArvore = (from obj in listaArvoreAnalisis
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Pai,
                                       obj.StatusFinalizacaoAtendimento,
                                       obj.Key,
                                       obj.Pergunta,
                                       obj.Resposta,
                                       obj.Situacao
                                   }),
                    Causas = (from obj in chamadoCausas
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao
                              }),
                    CausaSelecionada = new { Codigo = chamadoAnalisisSelecionado?.MotivoChamadoCausa?.Codigo ?? 0, Descricao = chamadoAnalisisSelecionado?.MotivoChamadoCausa?.Descricao ?? string.Empty },
                };

                return new JsonpResult(retorno);
            }
            catch (ServicoException exception)
            {
                return new JsonpResult(false, exception.Message);
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro(exception);
                return new JsonpResult(false, exception.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarExisteGeneroCadastrado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);

                return new JsonpResult(repositorioGenero.ExisteGeneroCadastrado());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarExisteAreaEnvolvidaCadastrada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvidaMotivoChamado = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

                return new JsonpResult(repositorioAreaEnvolvidaMotivoChamado.ExisteAreaEnvolvidaCadastrada());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarConfiguracaoChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    FazerGestaoCriticidade = configuracaoChamado?.FazerGestaoCriticidade ?? false,
                    PermitirAtualizarChamadoStatus = configuracaoChamado?.PermitirAtualizarChamadoStatus ?? false,
                    OcultarTomadorNoAtendimento = configuracaoChamado?.OcultarTomadorNoAtendimento ?? false,
                };


                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao rep = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracoes = repTipoIntegracao.BuscarAtivos();

                var retornoIntegracoes = (
                        from obj in tipoIntegracoes
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }
                    ).ToList();
                return new JsonpResult(new
                {
                    Integracoes = retornoIntegracoes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoBuscarIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarExisteUsuarioSetor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                bool resultado = repUsuario.ExisteUsuarioParaOhSetor(codigo);

                if (resultado)
                {
                    return new JsonpResult
                    {
                        Success = true,
                        Authorized = true,

                    };
                }
                else
                {
                    return new JsonpResult
                    {
                        Success = false,
                        Msg = "Não existe nenhum usuário cadastrado com o setor selecionado.",
                        Authorized = true
                    };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos de Criticidade de Atendimento

        [AllowAuthenticate]
        public async Task<IActionResult> CriticidadeAtendimentoPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisaCriticidade();
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int totalRegistros = 0;
                var lista = ExecutaPesquisaCriticidade(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, cancellationToken);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CriticidadeAtendimentoBuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repTipoCriticidade = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork, cancellationToken);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento tipoCriticidade = await repTipoCriticidade.BuscarPorCodigoAsync(codigo);

                if (tipoCriticidade == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new { tipoCriticidade.Codigo, tipoCriticidade.Tipo, tipoCriticidade.Conteudo, tipoCriticidade.Ativo };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTiposCriticidadePorMotivo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMotivo = Request.GetIntParam("CodigoMotivo");

                Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repositorioTipoCritAtendimento = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento> tipoCriticidadeAtendimentos = await repositorioTipoCritAtendimento.BuscarPorMotivoChamadoAsync(codigoMotivo)
                    ?? new List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>();

                var gerenciais = tipoCriticidadeAtendimentos
                    .Where(t => t.Tipo == TipoParametroCriticidade.Gerencial && t.Ativo)
                    .Select(t => new { t.Codigo, Descricao = t.Conteudo, t.Ativo, AtivoDesc = t.DescricaoAtivo }).ToList();

                var causasProblema = tipoCriticidadeAtendimentos
                    .Where(t => t.Tipo == TipoParametroCriticidade.CausaProblema && t.Ativo)
                    .Select(t => new { t.Codigo, Descricao = t.Conteudo, t.Ativo, AtivoDesc = t.DescricaoAtivo }).ToList();

                return new JsonpResult(new
                {
                    Gerenciais = gerenciais,
                    CausasProblema = causasProblema
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Falha ao buscar tipos de criticidade.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            if (Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo) == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 25, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("GerarCargaDevolucaoSeAprovado", false);
            grid.AdicionarCabecalho("GerarValePalletSeAprovado", false);
            grid.AdicionarCabecalho("ExigeValor", false);
            grid.AdicionarCabecalho("ExigeValorNaLiberacao", false);
            grid.AdicionarCabecalho("DescricaoComTipo", false);
            grid.AdicionarCabecalho("PermiteInserirJustificativaOcorrencia", false);
            grid.AdicionarCabecalho("ValidaValorCarga", false);
            grid.AdicionarCabecalho("ValidaValorDescarga", false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

            SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

            string descricao = Request.GetStringParam("Descricao");

            int codigoCarga = Request.GetIntParam("Carga");

            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> listaGrid = repMotivoChamado.Consultar(descricao, status, codigoCarga, TipoServicoMultisoftware, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoChamado.ContarConsulta(descricao, status, codigoCarga, TipoServicoMultisoftware);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.GerarCargaDevolucaoSeAprovado,
                            obj.GerarValePalletSeAprovado,
                            obj.DescricaoStatus,
                            obj.ExigeValor,
                            obj.ExigeValorNaLiberacao,
                            obj.DescricaoComTipo,
                            obj.PermiteInserirJustificativaOcorrencia,
                            obj.ValidaValorCarga,
                            obj.ValidaValorDescarga
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvida = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.GrupoMotivoChamado repGrupoMotivoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoCausas repositorioMotivoChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);

            int codigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia");
            int codigoPagamentoMotoristaTipo = Request.GetIntParam("PagamentoMotoristaTipo");
            int codigoJustificativa = Request.GetIntParam("Justificativa");
            int codigoGeneroMotivoChamado = Request.GetIntParam("Genero");
            int codigoAreaEnvolvidaMotivoChamado = Request.GetIntParam("AreaEnvolvida");
            int codigoGrupoMotivoChamado = Request.GetIntParam("GrupoMotivoChamado");
            int codigoJustificativaAcrescimoDescontoContratoFrete = Request.GetIntParam("JustificativaAcrescimoDescontoContratoFrete");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo? tipoTransportadorAcrescimoDesconto = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>("TipoTransportadorAcrescimoDescontoContratoFrete");

            double cnpjCpfFornecedor = Request.GetDoubleParam("FornecedorDespesa");

            bool.TryParse(Request.Params("Status"), out bool status);
            bool.TryParse(Request.Params("ExigeValor"), out bool exigeValor);
            bool.TryParse(Request.Params("ExigeValorNaLiberacao"), out bool exigeValorNaLiberacao);
            bool.TryParse(Request.Params("DisponibilizaParaReeentrega"), out bool disponibilizaParaReeentrega);
            bool.TryParse(Request.Params("ChamadoDeveSerAbertoPeloEmbarcador"), out bool chamadoDeveSerAbertoPeloEmbarcador);
            bool.TryParse(Request.Params("GerarCargaDevolucaoSeAprovado"), out bool gerarCargaDevolucaoSeAprovado);
            bool.TryParse(Request.Params("GerarValePalletSeAprovado"), out bool gerarValePalletSeAprovado);
            bool.TryParse(Request.Params("ExigeFotoAbertura"), out bool exigeFotoAbertura);
            bool.TryParse(Request.Params("ExigeQRCodeAbertura"), out bool exigeQRCodeAbertura);
            bool.TryParse(Request.Params("GerarOcorrenciaAutomaticamente"), out bool gerarOcorrenciaAutomaticamente);
            bool.TryParse(Request.Params("GerarCTeComValorIgualCTeAnterior"), out bool gerarCTeComValorIgualCTeAnterior);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento tipoMotivoAtendimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Atendimento;
            if (Enum.TryParse(Request.Params("TipoMotivoAtendimento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento tipoMotivoAtendimentoAux))
                tipoMotivoAtendimento = tipoMotivoAtendimentoAux;

            motivoChamado.Descricao = Request.GetStringParam("Descricao");
            motivoChamado.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            motivoChamado.Observacao = Request.GetStringParam("Observacao");
            motivoChamado.ConteudoEmail = Request.GetStringParam("ConteudoEmail");
            motivoChamado.Assunto = Request.GetStringParam("Assunto");

            motivoChamado.ChamadoDeveSerAbertoPeloEmbarcador = chamadoDeveSerAbertoPeloEmbarcador;
            motivoChamado.GerarCargaDevolucaoSeAprovado = gerarCargaDevolucaoSeAprovado;
            motivoChamado.GerarValePalletSeAprovado = gerarValePalletSeAprovado;
            motivoChamado.Status = status;
            motivoChamado.ExigeValor = exigeValor;
            motivoChamado.ExigeValorNaLiberacao = exigeValorNaLiberacao;
            motivoChamado.DisponibilizaParaReeentrega = disponibilizaParaReeentrega;
            motivoChamado.TipoMotivoAtendimento = tipoMotivoAtendimento;
            motivoChamado.ExigeFotoAbertura = exigeFotoAbertura;
            motivoChamado.ExigeQRCodeAbertura = exigeQRCodeAbertura;
            motivoChamado.GerarOcorrenciaAutomaticamente = gerarOcorrenciaAutomaticamente;
            motivoChamado.GerarCTeComValorIgualCTeAnterior = gerarOcorrenciaAutomaticamente && gerarCTeComValorIgualCTeAnterior;
            motivoChamado.CalcularOcorrenciaPorTabelaFrete = gerarOcorrenciaAutomaticamente && Request.GetBoolParam("CalcularOcorrenciaPorTabelaFrete");
            motivoChamado.ValidarDuplicidade = Request.GetBoolParam("ValidarDuplicidade");
            motivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga = Request.GetBoolParam("PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga");
            motivoChamado.ValidarDuplicidadePorDestinatario = Request.GetBoolParam("ValidarDuplicidadePorDestinatario");
            motivoChamado.ReferentePagamentoDescarga = Request.GetBoolParam("ReferentePagamentoDescarga");
            motivoChamado.PermiteInformarDesconto = Request.GetBoolParam("PermiteInformarDesconto");
            motivoChamado.PermiteEstornarAtendimento = Request.GetBoolParam("PermiteEstornarAtendimento");
            motivoChamado.ExigeAnaliseParaOperacao = Request.GetBoolParam("ExigeAnaliseParaOperacao");
            motivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista = Request.GetBoolParam("PermiteAdicionarValorComoAdiantamentoMotorista");
            motivoChamado.PermiteAdicionarValorComoDespesaMotorista = Request.GetBoolParam("PermiteAdicionarValorComoDespesaMotorista");
            motivoChamado.PermiteAdicionarValorComoDescontoMotorista = Request.GetBoolParam("PermiteAdicionarValorComoDescontoMotorista");
            motivoChamado.HabilitarPerfilAcessoEnvioEmail = Request.GetBoolParam("HabilitarPerfilAcessoEnvioEmail");
            motivoChamado.PermiteRetornarParaAjuste = Request.GetBoolParam("PermiteRetornarParaAjuste");
            motivoChamado.ObrigarAnexo = Request.GetBoolParam("ObrigarAnexo");
            motivoChamado.PermiteAtendimentoSemCarga = Request.GetBoolParam("PermiteAtendimentoSemCarga");
            motivoChamado.PermiteAlterarDatasCargaEntrega = Request.GetBoolParam("PermiteAlterarDatasCargaEntrega");
            motivoChamado.BuscaContaBancariaDestinatario = Request.GetBoolParam("BuscaContaBancariaDestinatario");
            motivoChamado.ObrigarInformarResponsavelAtendimento = Request.GetBoolParam("ObrigarInformarResponsavelAtendimento");
            motivoChamado.NaoPermitirLancarAtendimentoSemAcertoAberto = Request.GetBoolParam("NaoPermitirLancarAtendimentoSemAcertoAberto");
            motivoChamado.PermitirLancarAtendimentoEmCargasComDocumentoEmitido = Request.GetBoolParam("PermitirLancarAtendimentoEmCargasComDocumentoEmitido");
            motivoChamado.PermiteInserirJustificativaOcorrencia = Request.GetBoolParam("PermiteInserirJustificativaOcorrencia");
            motivoChamado.InformarQuantidade = Request.GetBoolParam("InformarQuantidade");
            motivoChamado.ObrigarInformacaoLote = Request.GetBoolParam("ObrigarInformacaoLote");
            motivoChamado.ObrigarDataCritica = Request.GetBoolParam("ObrigarDataCritica");
            motivoChamado.ObrigarRealMotivo = Request.GetBoolParam("ObrigarRealMotivo");
            motivoChamado.ExigeInformarModeloVeicularAberturaChamado = Request.GetBoolParam("ExigeInformarModeloVeicularAberturaChamado");
            motivoChamado.TratativaDeveSerConfirmadaPeloCliente = Request.GetBoolParam("TratativaDeveSerConfirmadaPeloCliente");
            motivoChamado.ObrigarMotoristaInformarMultiMobile = Request.GetBoolParam("ObrigarMotoristaInformarMultiMobile");
            motivoChamado.IntegrarComDansales = Request.GetBoolParam("IntegrarComDansales");
            motivoChamado.PermiteTrocarTransportadora = Request.GetBoolParam("PermiteTrocarTransportadora");
            motivoChamado.AtendimentoPorLote = Request.GetBoolParam("AtendimentoPorLote");
            motivoChamado.PermiteInformarNFD = Request.GetBoolParam("PermiteInformarNFD");
            motivoChamado.ObrigarPreenchimentoNFD = motivoChamado.PermiteInformarNFD && Request.GetBoolParam("ObrigarPreenchimentoNFD");
            motivoChamado.PermiteInformarQuantidadeParaCalculo = Request.GetBoolParam("PermiteInformarQuantidadeParaCalculo");
            motivoChamado.PermiteInformarQuantidadeVolumesParaCalculo = Request.GetBoolParam("PermiteInformarQuantidadeVolumesParaCalculo");
            motivoChamado.PermiteInformarPesoParaCalculo = Request.GetBoolParam("PermiteInformarPesoParaCalculo");
            motivoChamado.PermiteInformarDataRetornoAposEstadia = Request.GetBoolParam("PermiteInformarDataRetornoAposEstadia");
            motivoChamado.PermiteInformarOrdemInterna = Request.GetBoolParam("PermiteInformarOrdemInterna");
            motivoChamado.PermiteInformarMotoristaNoAtendimento = Request.GetBoolParam("PermiteInformarMotoristaNoAtendimento");
            motivoChamado.GerarAcrescimoDescontoContratoFrete = Request.GetBoolParam("GerarAcrescimoDescontoContratoFrete");
            motivoChamado.InformarCodigoSIF = Request.GetBoolParam("InformarCodigoSIF");
            motivoChamado.PermitirFreteRetornoDevolucao = Request.GetBoolParam("PermitirFreteRetornoDevolucao");
            motivoChamado.PossibilitarInclusaoAnexoAoEscalarAtendimento = Request.GetBoolParam("PossibilitarInclusaoAnexoAoEscalarAtendimento");
            motivoChamado.ConsiderarHorasDiasUteis = Request.GetBoolParam("ConsiderarHorasDiasUteis");
            motivoChamado.ValidarEscaladaAtendimentoUsuarioResponsavel = Request.GetBoolParam("ValidarEscaladaAtendimentoUsuarioResponsavel");
            motivoChamado.PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise = Request.GetBoolParam("PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise");
            motivoChamado.PermitirInformarCausadorOcorrencia = Request.GetBoolParam("PermitirInformarCausadorOcorrencia");
            motivoChamado.ValidaValorCarga = Request.GetBoolParam("ValidaValorCarga");
            motivoChamado.ValidaValorDescarga = Request.GetBoolParam("ValidaValorDescarga");
            motivoChamado.ListarNotasParaGeracaoGestaoDeDevolucao = Request.GetBoolParam("GerarGestaoDeDevolucao");
            motivoChamado.TipoQuebraRegraPallet = Request.GetEnumParam<TipoQuebraRegra>("TipoQuebraRegraPallet");
            motivoChamado.EnviarEmailParaTransportadorAoCancelarChamado = Request.GetBoolParam("EnviarEmailParaTransportadorAoCancelarChamado");
            motivoChamado.EnviarEmailParaTransportadorAoAlterarChamado = Request.GetBoolParam("EnviarEmailParaTransportadorAoAlterarChamado");
            motivoChamado.EnviarEmailParaTransportadorAoFinalizarChamado = Request.GetBoolParam("EnviarEmailParaTransportadorAoFinalizarChamado");
            motivoChamado.NumeroCriticidadeAtendimento = Request.GetIntParam("NumeroCriticidadeAtendimento");
            motivoChamado.BloquearParadaAppTrizy = Request.GetBoolParam("BloquearParadaAppTrizy");
            motivoChamado.HabilitarSenhaDevolucao = Request.GetBoolParam("HabilitarSenhaDevolucao");
            motivoChamado.HabilitarEstadia = Request.GetBoolParam("HabilitarEstadia");
            motivoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador = Request.GetBoolParam("BloquearEstornoAtendimentosFinalizadosPortalTransportador");
            motivoChamado.HabilitarClassificacaoCriticos = Request.GetBoolParam("HabilitarClassificacaoCriticos");

            motivoChamado.TipoOcorrencia = codigoTipoOcorrencia > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia) : null;
            motivoChamado.PagamentoMotoristaTipo = codigoPagamentoMotoristaTipo > 0 ? repPagamentoMotoristaTipo.BuscarPorCodigo(codigoPagamentoMotoristaTipo) : null;
            motivoChamado.Justificativa = codigoJustificativa > 0 ? repJustificativa.BuscarPorCodigo(codigoJustificativa) : null;
            motivoChamado.FornecedorDespesa = cnpjCpfFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCpfFornecedor) : null;
            motivoChamado.Genero = repositorioGenero.BuscarPorCodigo(codigoGeneroMotivoChamado, false);
            motivoChamado.AreaEnvolvida = repositorioAreaEnvolvida.BuscarPorCodigo(codigoAreaEnvolvidaMotivoChamado, false);
            motivoChamado.GrupoMotivoChamado = repGrupoMotivoChamado.BuscarPorCodigo(codigoGrupoMotivoChamado, false);
            motivoChamado.JustificativaAcrescimoDescontoContratoFrete = codigoJustificativaAcrescimoDescontoContratoFrete > 0 ? repJustificativa.BuscarPorCodigo(codigoJustificativaAcrescimoDescontoContratoFrete) : null;
            motivoChamado.TipoTransportadorAcrescimoDesconto = tipoTransportadorAcrescimoDesconto;

            // Atributos referentes a Diária Automática
            motivoChamado.ObrigatorioTerDiariaAutomatica = Request.GetBoolParam("ObrigatorioTerDiariaAutomatica");
            motivoChamado.BloquearAprovacaoValoresSuperioresADiariaAutomatica = Request.GetBoolParam("BloquearAprovacaoValoresSuperioresADiariaAutomatica");
            motivoChamado.DiasLimiteAberturaAposDiariaAutomatica = Request.GetIntParam("DiasLimiteAberturaAposDiariaAutomatica");
            motivoChamado.LocalFreeTime = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime>("LocalFreeTime");
            motivoChamado.PermitirAtualizarInformacoesPedido = Request.GetBoolParam("PermitirAtualizarInformacoesPedido");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(motivoChamado.Descricao))
            {
                msgErro = Localization.Resources.Chamado.MotivoChamado.DescricaoEObrigatorio;
                return false;
            }

            if (motivoChamado.GerarOcorrenciaAutomaticamente)
            {
                if (motivoChamado.TipoOcorrencia == null)
                {
                    msgErro = Localization.Resources.Chamado.MotivoChamado.TipoOcorrenciaEObrigatorioQuandoConfiguradoParaGerarCargaAutomaticamente;
                    return false;
                }
                else if (motivoChamado.TipoOcorrencia.OrigemOcorrencia != OrigemOcorrencia.PorCarga)
                {
                    msgErro = Localization.Resources.Chamado.MotivoChamado.GeracaoDeOcorrenciaAutomaticaApenasParaOcorrenciaPorCarga;
                    return false;
                }

            }

            return true;
        }

        private Models.Grid.Grid GridPesquisaCriticidade()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Conteúdo", "Conteudo", 60, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Ativo", "Ativo", 20, Models.Grid.Align.center, true);
            return grid;
        }

        private dynamic ExecutaPesquisaCriticidade(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repositorioTipoCritAtendimento = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork, cancellationToken);

            int codigoMotivoChamado = Request.GetIntParam("CodigoMotivoChamado");
            bool filtrarPorTipo = Request.GetBoolParam("FiltrarPorTipo");

            string tipoStr = Request.GetStringParam("TipoCriticidade");
            if (string.IsNullOrWhiteSpace(tipoStr))
                tipoStr = Request.GetStringParam("Tipo");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCriticidade? tipoFiltro = null;
            if (filtrarPorTipo && !string.IsNullOrWhiteSpace(tipoStr))
            {
                string norm = tipoStr.Trim();
                if (Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCriticidade>(norm, true, out var parsed))
                    tipoFiltro = parsed;
            }

            var lista = tipoFiltro.HasValue ? repositorioTipoCritAtendimento.BuscarPorMotivoChamadoETipoAsync(codigoMotivoChamado, tipoFiltro.Value).Result : repositorioTipoCritAtendimento.BuscarPorMotivoChamadoAsync(codigoMotivoChamado).Result;

            lista ??= new List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>();
            totalRegistros = lista.Count;

            var proj = lista.Select(x => new
            {
                x.Codigo,
                Tipo = x.DescricaoTipo,
                Conteudo = x.Conteudo,
                Ativo = x.DescricaoAtivo
            }).ToList();

            return proj;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoStatus") propOrdenar = "Status";
        }

        private void SalvarDatas(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoData repMotivoChamadoData = new Repositorio.Embarcador.Chamados.MotivoChamadoData(unitOfWork);

            dynamic dynDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Datas"));

            if (motivoChamado.Datas != null && motivoChamado.Datas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var data in dynDatas)
                    if (data.Codigo != null)
                        codigos.Add((int)data.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData> motivoChamadoDataDeletar = (from obj in motivoChamado.Datas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < motivoChamadoDataDeletar.Count; i++)
                    repMotivoChamadoData.Deletar(motivoChamadoDataDeletar[i]);
            }
            else
                motivoChamado.Datas = new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData>();

            foreach (var data in dynDatas)
            {
                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData motivoChamadoData = data.Codigo != null ? repMotivoChamadoData.BuscarPorCodigo((int)data.Codigo, false) : null;
                if (motivoChamadoData == null)
                    motivoChamadoData = new Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData();

                Dominio.Enumeradores.OpcaoSimNaoPesquisa opcaoSimNaoPesquisa = ((string)data.Obrigatorio).ToEnum<Dominio.Enumeradores.OpcaoSimNaoPesquisa>();

                motivoChamadoData.Descricao = (string)data.Descricao;
                motivoChamadoData.Obrigatorio = opcaoSimNaoPesquisa == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim ? true : false;
                motivoChamadoData.Status = ((string)data.Status).ToBool();

                motivoChamadoData.MotivoChamado = motivoChamado;

                if (motivoChamadoData.Codigo > 0)
                    repMotivoChamadoData.Atualizar(motivoChamadoData);
                else
                    repMotivoChamadoData.Inserir(motivoChamadoData);
            }
        }

        private void SalvarArvore(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoArvore repMotivoChamadoData = new Repositorio.Embarcador.Chamados.MotivoChamadoArvore(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> motivoChamadoArvore = repMotivoChamadoData.BuscarPorCodigoMotivoChamado(motivoChamado.Codigo);

            dynamic dynArvore = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Arvore"));

            if (motivoChamadoArvore != null && motivoChamadoArvore.Count > 0)
            {
                List<string> codigos = new List<string>();

                foreach (var data in dynArvore)
                    if (data.key != null)
                        codigos.Add((string)data.key);

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> motivoChamadoDataDeletar = (from obj in motivoChamadoArvore where !codigos.Contains(obj.Key) select obj).ToList();

                for (var i = 0; i < motivoChamadoDataDeletar.Count; i++)
                    repMotivoChamadoData.Deletar(motivoChamadoDataDeletar[i]);
            }
            else
                motivoChamadoArvore = new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore>();


            foreach (var pergunta in dynArvore)
            {
                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore arvore = motivoChamadoArvore.Where(motivo => motivo.Key == (string)pergunta.key).FirstOrDefault();

                if (arvore == null)
                    arvore = new Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore();

                arvore.Key = (string)pergunta.key;
                arvore.Pai = (string)pergunta.pai;
                arvore.Pergunta = (string)pergunta.pergunta;
                arvore.Resposta = (TipoResposta)pergunta.resposta;
                arvore.MotivoChamado = motivoChamado;
                arvore.StatusFinalizacaoAtendimento = (StatusFinalizacaoAtendimento)pergunta.statusFinalizador;

                if (arvore.Codigo == 0)
                    repMotivoChamadoData.Inserir(arvore);
                else
                    repMotivoChamadoData.Atualizar(arvore);
            }

        }

        private void SalvarArvoreRespondida(int codigoMotivo, int codigoAnalisis, int codigoCausa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoArvore repMotivoChamadoData = new Repositorio.Embarcador.Chamados.MotivoChamadoArvore(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoCausas repChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoAnalisisArvore repoChamadoAnalisiArvore = new Repositorio.Embarcador.Chamados.ChamadoAnalisisArvore(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> arvorePadrao = repMotivoChamadoData.BuscarPorCodigoMotivoChamado(codigoMotivo);
            List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore> listaArvoreAnalisis = repoChamadoAnalisiArvore.BuscarPorMotivoChamado(codigoMotivo, codigoAnalisis);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoAnalisis);
            Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas chamadoCausa = repChamadoCausas.BuscarPorCodigo(codigoCausa);

            if (chamado == null)
                throw new ControllerException(Localization.Resources.Chamado.MotivoChamado.ChamadoNaoEncontrado);

            if (listaArvoreAnalisis != null && listaArvoreAnalisis.Count == 0 && arvorePadrao != null)
            {
                listaArvoreAnalisis = (from obj in arvorePadrao
                                       select new Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore
                                       {
                                           Key = obj.Key,
                                           MotivoChamado = obj.MotivoChamado,
                                           Pai = obj.Pai,
                                           Pergunta = obj.Pergunta,
                                           Resposta = obj.Resposta,
                                           Situacao = obj.Situacao,
                                           StatusFinalizacaoAtendimento = obj.StatusFinalizacaoAtendimento,
                                           Chamado = chamado,
                                           MotivoChamadoCausa = chamadoCausa,
                                       }).ToList();
                foreach (var itemArvore in listaArvoreAnalisis)
                    repoChamadoAnalisiArvore.Inserir(itemArvore);
            }

            dynamic dynArvore = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Arvore"));

            if (listaArvoreAnalisis != null && listaArvoreAnalisis.Count > 0)
            {
                List<int> listaCodigo = new List<int>();

                foreach (dynamic itemArvore in dynArvore)
                    if ((int)itemArvore.Codigo > 0)
                        listaCodigo.Add((int)itemArvore.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore> listaRemover = listaArvoreAnalisis.Where(a => !listaCodigo.Contains(a.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore itemArvore in listaRemover)
                {
                    itemArvore.Situacao = SituacaoPerguntaArvore.Inativo;
                    repoChamadoAnalisiArvore.Atualizar(itemArvore);
                }


            }

            foreach (var pergunta in dynArvore)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalisisArvore arvore = listaArvoreAnalisis.Where(motivo => motivo.Key == (string)pergunta.Key).FirstOrDefault();

                if (arvore == null)
                    continue;

                arvore.Situacao = pergunta.Situacao;
                arvore.MotivoChamadoCausa = chamadoCausa;
                repoChamadoAnalisiArvore.Atualizar(arvore);
            }

        }

        private void SalvarGatilhosTempoList(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repMotivoChamadoGatilhos = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork);
            Repositorio.Setor repSertorFuncionario = new Repositorio.Setor(unitOfWork);

            dynamic dynGatilhos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GatilhosTempoList"));

            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> listaGatilhos = repMotivoChamadoGatilhos.BuscarPorMotivoChamado(motivoChamado.Codigo);

            if (listaGatilhos == null)
                listaGatilhos = new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>();

            if (listaGatilhos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var gatilho in dynGatilhos)
                {
                    int codigo;
                    int.TryParse((string)gatilho.Codigo, out codigo);
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> motivoChamadoDeletar = (from obj in listaGatilhos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < motivoChamadoDeletar.Count; i++)
                    repMotivoChamadoGatilhos.Deletar(motivoChamadoDeletar[i]);
            }

            foreach (var gatilho in dynGatilhos)
            {
                int codigo;
                int.TryParse((string)gatilho.Codigo, out codigo);

                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList existeGatilho = repMotivoChamadoGatilhos.BuscarPorCodigo(codigo, false);

                if (existeGatilho == null)
                    existeGatilho = new Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList();

                existeGatilho.Nivel = (EscalationList)gatilho.Nivel;
                existeGatilho.Tempo = (int)gatilho.Tempo;
                existeGatilho.MotivoChamado = motivoChamado;
                existeGatilho.Setor = (int)gatilho.CodigoSetor > 0 ? repSertorFuncionario.BuscarPorCodigo((int)gatilho.CodigoSetor) : null;

                if (existeGatilho.Codigo == 0)
                    repMotivoChamadoGatilhos.Inserir(existeGatilho);
                else
                    repMotivoChamadoGatilhos.Atualizar(existeGatilho);
            }


        }

        private void SalvarTipoIntegracao(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoCarga = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unidadeDeTrabalho);

            dynamic tiposIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposIntegracao"));

            if (motivoChamado.TipoIntegracao == null)
            {
                motivoChamado.TipoIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoIntegracao in tiposIntegracao)
                    codigos.Add((int)tipoIntegracao.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposDeletar = motivoChamado.TipoIntegracao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoDeCargaDeletar in tiposDeletar)
                {
                    motivoChamado.TipoIntegracao.Remove(tipoDeCargaDeletar);
                    codigos.Remove(tipoDeCargaDeletar.Codigo);
                }

                List<int> codigosExistentes = motivoChamado.TipoIntegracao.Select(tipo => tipo.Codigo).ToList();
                List<int> codigosNovos = codigos.Except(codigosExistentes).ToList();

                if (codigosNovos.Count > 0)
                {
                    bool tiposJaCadastrados = repMotivoChamado.VerificarIntegracoesDuplcadas(codigosNovos, motivoChamado.Codigo);

                    if (tiposJaCadastrados)
                        throw new ControllerException("Não é possível cadastrar o mesmo tipo de integração para mais de um chamado");

                    bool tiposJaCadastradosEmOutroMotivoDeAtendimento = repMotivoChamado.VerificarIntegracoesDuplcadasMotivosDiferente(codigosNovos, motivoChamado.Codigo);

                    if (tiposJaCadastradosEmOutroMotivoDeAtendimento)
                        throw new ControllerException("Não é possível cadastrar o mesmo tipo de integração já cadastrado em um outro motivo de atendimento");
                }
            }
            ;

            foreach (var tipoIntegracao in tiposIntegracao)
            {
                int codigo = 0;
                codigo = tipoIntegracao.Codigo;

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoCargaAdicionar = repTipoCarga.BuscarPorCodigo(codigo);

                if (!motivoChamado.TipoIntegracao.Any(o => o.Codigo == (int)tipoIntegracao.Codigo))
                    motivoChamado.TipoIntegracao.Add(tipoCargaAdicionar);
            }

        }

        private void SalvarCausas(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoCausas repMotivoChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);


            dynamic dynCausas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Causas"));

            foreach (var causa in dynCausas)
            {
                int codigo = 0;
                codigo = causa.Codigo;

                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas causasAdicionar = repMotivoChamadoCausas.BuscarPorCodigo(codigo, false);

                if (causasAdicionar == null)
                    causasAdicionar = new Dominio.Entidades.Embarcador.Chamados.MotivoChamadoCausas();

                causasAdicionar.Descricao = causa.Descricao;
                causasAdicionar.MotivoChamado = motivoChamado;
                causasAdicionar.Ativo = causa.Ativo == "Sim" ? true : false;

                if (causasAdicionar.Codigo == 0)
                    repMotivoChamadoCausas.Inserir(causasAdicionar);
                else
                    repMotivoChamadoCausas.Atualizar(causasAdicionar);

            }


        }

        private void SalvarGatilhosNaCarga(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga repMotivoChamadoGatilhosNaCarga = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga(unitOfWork);

            dynamic dynGatilhos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("GatilhosNaCarga"));

            repMotivoChamadoGatilhosNaCarga.DeletarPorMotivoChamado(motivoChamado.Codigo);

            foreach (var gatilho in dynGatilhos)
            {
                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga gatilhoNaCarga = new Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga()
                {
                    Gatilho = (TipoMotivoChamadoGatilhoNaCarga)gatilho.Gatilho,
                    MotivoChamado = motivoChamado
                };

                repMotivoChamadoGatilhosNaCarga.Inserir(gatilhoNaCarga);
            }
        }

        private async Task SalvarTiposCriticidade(Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {

            string tiposCriticidadeJson = Request.GetStringParam("TiposCriticidade");

            if (string.IsNullOrWhiteSpace(tiposCriticidadeJson))
                return;

            Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repositorioTipoCriticidade = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork, cancellationToken);
            dynamic dynTiposCriticidade = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("TiposCriticidade"));

            List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento> tiposExistentes = await repositorioTipoCriticidade.BuscarPorMotivoChamadoAsync(motivoChamado.Codigo);

            List<int> codigosRecebidos = new List<int>();

            foreach (var tipoCrit in dynTiposCriticidade)
            {
                int codigo = (int)(tipoCrit.Codigo ?? 0);

                if (codigo > 0)
                    codigosRecebidos.Add(codigo);
            }

            List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento> tiposParaDeletar =
                (from obj in tiposExistentes
                 where !codigosRecebidos.Contains(obj.Codigo)
                 select obj).ToList();

            foreach (var tipoDeletar in tiposParaDeletar)
            {
                await repositorioTipoCriticidade.DeletarAsync(tipoDeletar);
            }

            foreach (var tipoCrit in dynTiposCriticidade)
            {
                int codigo = (int)(tipoCrit.Codigo ?? 0);

                Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento tipoCriticidade = null;

                if (codigo > 0)
                {
                    tipoCriticidade = tiposExistentes.FirstOrDefault(t => t.Codigo == codigo);
                }

                tipoCriticidade ??= new Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento();

                tipoCriticidade.MotivoChamado = motivoChamado;
                tipoCriticidade.Tipo = (TipoParametroCriticidade)tipoCrit.Tipo;
                tipoCriticidade.Conteudo = (string)tipoCrit.Conteudo;
                tipoCriticidade.Ativo = (bool)tipoCrit.Ativo;

                if (tipoCriticidade.Codigo > 0)
                    await repositorioTipoCriticidade.AtualizarAsync(tipoCriticidade);
                else
                    await repositorioTipoCriticidade.InserirAsync(tipoCriticidade);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas ObterFiltrosPesquisaCausas()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoMotivoChamado = Request.GetIntParam("CodigoMotivoChamado"),
                BuscarTodasCausasDesconsiderandoMotivoChamado = Request.GetBoolParam("BuscarTodasCausasDesconsiderandoMotivoChamado"),
            };
        }

        #endregion
    }
}
