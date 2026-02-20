using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize(new string[] { "ValidarCamposVisiveisEtapaAnalise" }, "Chamados/ChamadoOcorrencia", "Cargas/ControleEntrega")]
    public class ChamadoAnaliseController : BaseController
    {
        #region Construtores

        public ChamadoAnaliseController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarOcorrenciasChamado()
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
                dynamic lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
                dynamic lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        public async Task<IActionResult> CancelarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");

                bool permissaoCancelar = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Cancelar);
                if (!Usuario.UsuarioAdministrador && !permissaoCancelar)
                    return new JsonpResult(false, true, "Seu usuário não possui permissão para cancelar esse chamado.");

                Servicos.Embarcador.Chamado.Chamado srvChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoMotivoRecusaCancelamento = Request.GetIntParam("MotivoRecusaCancelamento");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Situacao == SituacaoChamado.Cancelada)
                    return new JsonpResult(false, true, "Esse chamado já está cancelado.");

                Dominio.Entidades.Setor setor = chamado.Carga?.Filial?.SetorAtendimento;

                if (!chamado.Analistas.Contains(Usuario) && (setor == null || Usuario.Setor == null || setor.Codigo != Usuario.Setor.Codigo || !(setor?.PermitirCancelarAtendimento ?? false)) && !permissaoCancelar)
                    return new JsonpResult(false, true, "Usuário sem permissão para esse chamado.");

                unitOfWork.Start();

                chamado.MotivoRecusaCancelamento = codigoMotivoRecusaCancelamento > 0 ? repMotivoRecusaCancelamento.BuscarPorCodigo(codigoMotivoRecusaCancelamento) : null;

                srvChamado.CancelarChamado(chamado, unitOfWork, Auditado, Usuario, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                srvChamado.EnviarEmailChamadoCanceladoParaTransportador(chamado, unitOfWork);

                if (chamado.NotificacaoMotoristaMobile)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();

                    string observacao = string.IsNullOrEmpty(chamado.ObservacaoRetornoMotorista) ? (chamadoAnalise?.Observacao ?? "") : chamado.ObservacaoRetornoMotorista;
                    dynamic conteudo = ObterObjetoNotificacao(chamado, observacao, chamado.Situacao, chamado.CargaEntrega.NotificarDiferencaDevolucao);
                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork, TipoServicoMultisoftware, Auditado);
                // Instancia repositorios
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork, cancellationToken);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = await repositorioChamado.BuscarPorCodigoAsync(codigo);

                // Valida
                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!await servicoChamado.DefinirAnalistasChamado(chamado, this.Usuario))
                    chamado.Situacao = SituacaoChamado.SemRegra;
                else
                    chamado.Situacao = SituacaoChamado.Aberto;

                //chamado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.LiberadaOcorrencia;
                await repositorioChamado.AtualizarAsync(chamado);

                servicoChamado.EnviarEmailChamadoAberto(chamado, unitOfWork);

                // Retorna informacoes
                return new JsonpResult(chamado.Situacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Delegar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaOutroUsuario))
                    return new JsonpResult(false, true, "Você não possui permissões para delegar para outro usuário.");

                int codigoChamado = Request.GetIntParam("Chamado");
                int codigoUsuario = Request.GetIntParam("Usuario");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!PossuiAnaliseParaEfetuarOperacao(chamado, unitOfWork))
                    return new JsonpResult(false, true, "Favor informar uma análise antes de efetuar a operação.");

                Dominio.Entidades.Usuario usuarioDelegado = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                chamado.Responsavel = usuarioDelegado;
                chamado.SetorResponsavel = null;

                if (!chamado.DataPrimeiraVezAssumido.HasValue)
                    chamado.DataPrimeiraVezAssumido = DateTime.Now;

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "É obrigatório informar o usuário.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuarioDelegado.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, $"Delegou o chamado para o usuário {chamado.Responsavel.Nome}", unitOfWork);
                repositorioChamado.Atualizar(chamado);

                servicoChamado.EnviarEmailChamadoDelegado(chamado, unitOfWork);
                servicoChamado.EnviarEmailAtendimentoDelegado(chamado, ClienteAcesso.URLAcesso, unitOfWork, this.Usuario);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarParaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoChamado = Request.GetIntParam("Chamado");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado);

                if (chamado == null) throw new ControllerException("Não foi possível encontrar o Chamado");
                if (chamado?.AguardandoTratativaDoCliente ?? false) throw new ControllerException("O Chamado já está pendente da tratativa do Cliente");

                unitOfWork.Start();

                chamado.AguardandoTratativaDoCliente = true;
                repositorioChamado.Atualizar(chamado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar para o cliente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DelegarPorSetor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermitirDelegarParaUmSetor))
                    return new JsonpResult(false, true, "Você não possui permissões para delegar para um setor.");

                unitOfWork.Start();

                int codigoChamado = Request.GetIntParam("Chamado");
                int codigoSetorFuncionario = Request.GetIntParam("Setor");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Setor repositorioSetor = new Repositorio.Setor(unitOfWork);
                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado) ?? throw new ControllerException("Não foi possível encontrar o registro.");
                Dominio.Entidades.Setor setorFuncionario = repositorioSetor.BuscarPorCodigo(codigoSetorFuncionario) ?? throw new ControllerException("Não foi possível encontrar o setor informado.");

                servicoChamado.DefinirAnalistasChamadoPorSetor(chamado, setorFuncionario, this.Usuario, TipoServicoMultisoftware, unitOfWork, Auditado);

                unitOfWork.CommitChanges();

                servicoChamado.EnviarEmailChamadoDelegadoParaSetor(chamado, setorFuncionario, unitOfWork);
                servicoChamado.EnviarEmailAtendimentoDelegado(chamado, ClienteAcesso.URLAcesso, unitOfWork, this.Usuario, true);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar por setor o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AbrirOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto servContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoRepresentante = Request.GetIntParam("Representante");
                double pessoaTituloPagar = Request.GetDoubleParam("PessoaTituloPagar");
                ChamadoResponsavelOcorrencia responsavelOcorrencia = Request.GetEnumParam<ChamadoResponsavelOcorrencia>("ResponsavelOcorrencia");
                bool podeResponder = Request.GetBoolParam("PodeResponder");
                bool naoAssumirDataEntregaNota = Request.GetBoolParam("NaoAssumirDataEntregaNota");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnali = repChamadoAnalise.BuscarPorChamado(codigo);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "Atendimento não possui responsável.");

                if (chamado.Responsavel.Codigo != Usuario.Codigo && !chamado.Analistas.Contains(Usuario))
                    return new JsonpResult(false, true, "Usuário sem permissão para esse atendimento.");

                if (chamado.Situacao == SituacaoChamado.AgIntegracao || chamado.Situacao == SituacaoChamado.FalhaIntegracao)
                    return new JsonpResult(false, true, "Esse atendimento já está no fluxo de integração!");

                if (chamadoAnali.Where(c => c.LiberadoValorCargaDescarga == true).Any() && ValidarValoresCargaDescarga(chamado.Codigo, chamado.Carga.Codigo, chamado.MotivoChamado.Codigo, chamado.Cliente.CPF_CNPJ))
                    return new JsonpResult(false, true, "Chamado com valor maior que Pedido !");

                List<SituacaoCarga> situacaoPermite = new List<SituacaoCarga>() { SituacaoCarga.AgImpressaoDocumentos, SituacaoCarga.EmTransporte, SituacaoCarga.Encerrada, SituacaoCarga.PendeciaDocumentos, SituacaoCarga.AgIntegracao };

                if (ConfiguracaoEmbarcador.FiltrarCargasSemDocumentosParaChamados)
                    situacaoPermite.AddRange(new List<SituacaoCarga>() { SituacaoCarga.AgNFe, SituacaoCarga.AgTransportador, SituacaoCarga.CalculoFrete, SituacaoCarga.Nova });

                if (chamado.Carga != null && !situacaoPermite.Contains(chamado.Carga.SituacaoCarga))
                    return new JsonpResult(false, true, "Situação da carga (" + chamado.Carga.DescricaoSituacaoCarga + ") não permite seguir.");

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = chamado.MotivoChamado;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;

                if (motivoChamado.ExigeValorNaLiberacao && chamado.Valor == 0)
                    return new JsonpResult(false, true, "Atendimento exige informar valor na etapa 1 para liberar a ocorrência.");

                if (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega && !chamado.DataReentrega.HasValue)
                    return new JsonpResult(false, true, "Atendimento exige informar a Data Reentrega na etapa 1 para liberar a ocorrência.");

                if ((motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem) && !(cargaEntrega?.DataSaidaRaio.HasValue ?? true))
                    return new JsonpResult(false, true, "Atendimento exige que seja informada a Data de Saída na Carga Entrega para liberar a ocorrência.");

                if (ConfiguracaoEmbarcador.ExigirClienteResponsavelPeloAtendimento && chamado.ClienteResponsavel == null)
                    return new JsonpResult(false, true, "Atendimento exige informar o cliente responsável na etapa 1 para liberar a ocorrência.");

                if (!PossuiAnaliseParaEfetuarOperacao(chamado, unitOfWork))
                    return new JsonpResult(false, true, "Favor informar uma análise antes de efetuar a operação.");

                // Validação relacionada a Diárias automáticas
                ValidarChamadoParaDiariasAutomaticas(chamado, unitOfWork);

                chamado.ResponsavelOcorrencia = responsavelOcorrencia;
                chamado.Representante = codigoRepresentante > 0 ? repRepresentante.BuscarPorCodigo(codigoRepresentante) : null;
                chamado.DataFinalizacao = DateTime.Now;
                chamado.ControleDuplicidade = chamado.Codigo;

                unitOfWork.Start();

                SalvarCriticidadeChamado(chamado, unitOfWork);
                if (chamado.GerarCargaDevolucao)
                {
                    if (motivoChamado.GerarCargaDevolucaoSeAprovado)
                        Servicos.Embarcador.Chamado.Chamado.GerarCargaDevolucao(ref chamado, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Cliente, unitOfWork);

                    servicoChamado.GerarIntegracoes(chamado, unitOfWork, Auditado, TipoServicoMultisoftware);

                    if (chamado.Situacao != SituacaoChamado.AgIntegracao)
                    {
                        chamado.Situacao = SituacaoChamado.Finalizado;

                        repChamado.Atualizar(chamado);

                        new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(chamado, TipoNotificacaoApp.TratativaDoAtendimento);
                    }
                }
                else if (motivoChamado.GerarValePalletSeAprovado)
                {
                    chamado.Situacao = SituacaoChamado.LiberadaValePallet;
                    repChamado.Atualizar(chamado, Auditado);
                }
                else if (ShouldGerarOcorrencia(chamado))
                {
                    string mensagemRetorno = GerarOcorrencia(chamado, motivoChamado, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                        throw new ControllerException(mensagemRetorno);

                    //Se a diferença entre data de chegada e data reentrega for maior que 5 hrs, gera ocorrência de retenção também
                    if (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega && cargaEntrega != null && cargaEntrega.DataEntradaRaio.HasValue && chamado.DataReentrega.HasValue &&
                        (cargaEntrega.DataEntradaRaio.Value.AddHours(5) < chamado.DataReentrega))
                    {
                        Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoRetencao = repMotivoChamado.BuscarPorTipoMotivoAtendimento(TipoMotivoAtendimento.Retencao);
                        if (motivoRetencao != null && motivoRetencao.GerarOcorrenciaAutomaticamente && motivoRetencao.TipoOcorrencia != null && motivoRetencao.TipoOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorCarga)
                        {
                            if (!chamado.DataRetencaoInicio.HasValue)
                                chamado.DataRetencaoInicio = cargaEntrega?.DataEntradaRaio;
                            if (!chamado.DataRetencaoFim.HasValue)
                                chamado.DataRetencaoFim = chamado.DataReentrega;

                            mensagemRetorno = GerarOcorrencia(chamado, motivoRetencao, unitOfWork);
                            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                                throw new ControllerException(mensagemRetorno);
                        }
                    }

                    servicoChamado.GerarIntegracoes(chamado, unitOfWork, Auditado, TipoServicoMultisoftware);

                    if (chamado.Situacao != SituacaoChamado.AgIntegracao)
                    {
                        chamado.Situacao = SituacaoChamado.Finalizado;

                        repChamado.Atualizar(chamado);

                        new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(chamado, TipoNotificacaoApp.TratativaDoAtendimento);
                    }
                }
                else
                {
                    chamado.Situacao = SituacaoChamado.LiberadaOcorrencia;
                    repChamado.Atualizar(chamado, Auditado);
                }

                if (cargaEntrega != null)
                {
                    if (cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue && (!chamado.DataRetencaoInicio.HasValue || !chamado.DataRetencaoFim.HasValue))
                    {
                        chamado.DataRetencaoInicio = cargaEntrega.DataEntradaRaio;
                        chamado.DataRetencaoFim = cargaEntrega.DataSaidaRaio;
                        repChamado.Atualizar(chamado);
                    }

                    chamado.NotificacaoMotoristaMobile = true;
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    cargaEntrega.ChamadoEmAberto = false;

                    if ((chamado.MotivoChamado?.TipoMotivoAtendimento != TipoMotivoAtendimento.Atendimento || cargaEntrega.Situacao == SituacaoEntrega.AgAtendimento) && cargaEntrega.Situacao != SituacaoEntrega.Entregue)
                        cargaEntrega.Situacao = SituacaoEntrega.NaoEntregue;

                    repCargaEntrega.Atualizar(cargaEntrega);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NaoPossuiEntregasPendentes(cargaEntrega.Carga, unitOfWork))
                    {
                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(cargaEntrega.Carga.Codigo, cargaEntrega.DataRejeitado ?? DateTime.Now, base.Auditado, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, unitOfWork))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega.Carga, $"Fim de viagem informado automaticamente ao abrir atendimento", unitOfWork);
                    }
                }

                if (ConfiguracaoEmbarcador.ResponderAnaliseAoLiberarOcorrenciaChamado && podeResponder)
                    Servicos.Embarcador.Chamado.Chamado.ResponderChamado(chamado, TipoServicoMultisoftware, unitOfWork);

                if (motivoChamado.TipoOcorrencia?.OcorrenciaParaQuebraRegraPallet ?? false)
                    new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado).InformarQuebraRegra(chamado, motivoChamado.TipoQuebraRegraPallet);

                servicoChamado.GerarPagamentoMotorista(chamado, pessoaTituloPagar, ConfiguracaoEmbarcador, Usuario, Auditado, TipoServicoMultisoftware, unitOfWork);
                servicoChamado.InserirDespesaAcertoViagem(chamado, Auditado, TipoServicoMultisoftware, unitOfWork);
                servicoChamado.InserirDescontoAcertoViagem(chamado, Auditado, TipoServicoMultisoftware, unitOfWork);

                string mensagemErro = string.Empty;
                if (!servContratoFreteAcrescimoDesconto.GerarContratoFreteAcrescimoDescontoApartirChamado(chamado, ref mensagemErro, Usuario, TipoServicoMultisoftware, Auditado, false, unitOfWork))
                    throw new ServicoException(mensagemErro);

                if (!string.IsNullOrWhiteSpace(chamado.CargaEntrega?.IdTrizy))
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarChamadoOcorrencia(chamado, EventoIntegracaoOcorrenciaTrizy.LiberarOcorrencia, this.Usuario, unitOfWork);

                unitOfWork.CommitChanges();

                if (chamado.NotificacaoMotoristaMobile)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();

                    dynamic conteudo = ObterObjetoNotificacao(chamado, chamadoAnalise?.Observacao, chamado.Situacao, cargaEntrega.NotificarDiferencaDevolucao);
                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                if ((chamado.Situacao == SituacaoChamado.LiberadaOcorrencia || chamado.Situacao == SituacaoChamado.Finalizado) && cargaEntrega != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);
                    if (cargaEvento != null)
                    {
                        servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }
                }

                if (chamado.Situacao == SituacaoChamado.Finalizado)
                    new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoFinalizado(chamado, unitOfWork);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o atendimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoRepresentante = Request.GetIntParam("Representante");
                double pessoaTituloPagar = Request.GetDoubleParam("PessoaTituloPagar");
                ChamadoResponsavelOcorrencia responsavelOcorrencia = Request.GetEnumParam<ChamadoResponsavelOcorrencia>("ResponsavelOcorrencia");
                string itensDevolverParam = Request.Params("ItensDevolver");
                dynamic itensDevolver = null;
                if (!string.IsNullOrEmpty(itensDevolverParam))
                    itensDevolver = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(itensDevolverParam);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "Atendimento não possui responsável.");

                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia) && chamado.Situacao == SituacaoChamado.LiberadaOcorrencia)
                    return new JsonpResult(false, true, "Usuário sem Permissão para finalizar o atendimento nesta Situação.");

                if (chamado.Responsavel.Codigo != Usuario.Codigo && !Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermitirFinalizarMesmoNaoSendoResponsavel))
                    return new JsonpResult(false, true, "Usuário sem Permissão para finalizar o Atendimento do qual não é o Responsável.");

                if (!configuracaoChamado.PermiteFinalizarAtendimentoComOcorrenciaRejeitada)
                    if (!ValidaFinalizacaoChamado(chamado, out string erro, unitOfWork))
                        return new JsonpResult(false, true, erro);

                if (chamado.Situacao == SituacaoChamado.Finalizado)
                    return new JsonpResult(false, true, "Esse atendimento já está finalizado.");

                if (chamado.Situacao == SituacaoChamado.RecusadoPeloCliente)
                    return new JsonpResult(false, true, "Esse atendimento já está finalizado com recusa.");

                if (chamado.Situacao == SituacaoChamado.AgIntegracao || chamado.Situacao == SituacaoChamado.FalhaIntegracao)
                    return new JsonpResult(false, true, "Esse atendimento já está no fluxo de integração!");

                if (!PossuiAnaliseParaEfetuarOperacao(chamado, unitOfWork) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermiteFinalizarEmLiberadoParaOcorrencia) && chamado.Situacao == SituacaoChamado.LiberadaOcorrencia && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, true, "Favor informar uma análise antes de efetuar a operação.");

                servicoChamado.GerarIntegracoes(chamado, unitOfWork, Auditado, TipoServicoMultisoftware);

                if (chamado.Situacao != SituacaoChamado.AgIntegracao)
                {
                    chamado.Situacao = SituacaoChamado.Finalizado;
                    chamado.DataFinalizacao = DateTime.Now;
                }

                chamado.ResponsavelOcorrencia = responsavelOcorrencia;
                chamado.Representante = codigoRepresentante > 0 ? repRepresentante.BuscarPorCodigo(codigoRepresentante) : null;
                chamado.ControleDuplicidade = chamado.Codigo;
                chamado.AguardandoIntegracao = true;

                unitOfWork.Start();

                SalvarCriticidadeChamado(chamado, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
                if (cargaEntrega != null)
                {
                    chamado.NotificacaoMotoristaMobile = true;

                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = cargaEntrega.MotivoRejeicao;
                    if (tipoDeOcorrencia == null)
                        tipoDeOcorrencia = chamado.MotivoChamado.TipoOcorrencia;

                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    cargaEntrega.ChamadoEmAberto = false;
                    cargaEntrega.MotivoRejeicao = null;

                    if (!cargaEntrega.DataConfirmacao.HasValue)
                        cargaEntrega.Situacao = SituacaoEntrega.NaoEntregue;
                    else
                    {
                        cargaEntrega.Situacao = SituacaoEntrega.Entregue;
                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                        repositorioXmlNotaFiscal.AtualizarSituacaoNotasFiscaisPorEntrega(cargaEntrega.Codigo, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Entregue);
                    }

                    if (itensDevolver != null && itensDevolver.NotasFiscais != null && itensDevolver.NotasFiscais.Count > 0)
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargasEntregasNotaFiscal = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                        foreach (var item in itensDevolver.NotasFiscais)
                        {
                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = cargasEntregasNotaFiscal.Where(x => x.Codigo == (int)item.Codigo).FirstOrDefault();
                            if (cargaEntregaNotaFiscal != null)
                            {
                                if ((bool)item.DevolucaoParcial)
                                    cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.DevolvidaParcial;
                                else
                                    cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.Devolvida;

                                repositorioCargaEntregaNotaFiscal.Atualizar(cargaEntregaNotaFiscal);
                            }

                        }
                    }

                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                    OrigemSituacaoEntrega origem = !string.IsNullOrEmpty(chamado.IdOcorrenciaTrizy) ? OrigemSituacaoEntrega.App : OrigemSituacaoEntrega.UsuarioMultiEmbarcador;

                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaRejeicao(cargaEntrega, cargaEntrega.DataRejeitado ?? DateTime.Now, tipoDeOcorrencia, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, "", 0m, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Cliente, origem, unitOfWork, Auditado, null, chamado);

                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NaoPossuiEntregasPendentes(cargaEntrega.Carga, unitOfWork) || (tipoDeOcorrencia?.OcorrenciaFinalizaViagem ?? false))
                    {
                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(cargaEntrega.Carga.Codigo, cargaEntrega.DataRejeitado ?? DateTime.Now, Auditado, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, unitOfWork))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega.Carga, $"Fim de viagem informado automaticamente ao finalizar atendimento", unitOfWork);
                    }
                }

                repChamado.Atualizar(chamado, Auditado);

                if (chamado.MotivoChamado.TipoOcorrencia?.OcorrenciaParaQuebraRegraPallet ?? false)
                    new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado).InformarQuebraRegra(chamado, chamado.MotivoChamado.TipoQuebraRegraPallet);

                servicoChamado.GerarPagamentoMotorista(chamado, pessoaTituloPagar, ConfiguracaoEmbarcador, Usuario, Auditado, TipoServicoMultisoftware, unitOfWork);
                servicoChamado.InserirDespesaAcertoViagem(chamado, Auditado, TipoServicoMultisoftware, unitOfWork);
                servicoChamado.InserirDescontoAcertoViagem(chamado, Auditado, TipoServicoMultisoftware, unitOfWork);

                if (!string.IsNullOrWhiteSpace(chamado.CargaEntrega?.IdTrizy))
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarChamadoOcorrencia(chamado, EventoIntegracaoOcorrenciaTrizy.Fechar, this.Usuario, unitOfWork);

                if (cargaEntrega != null)
                {
                    if (chamado.NotificacaoMotoristaMobile)
                    {
                        List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                        Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();

                        dynamic conteudo = ObterObjetoNotificacao(chamado, chamadoAnalise?.Observacao, SituacaoChamado.Finalizado, cargaEntrega.NotificarDiferencaDevolucao);
                        Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                        notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                    }

                    Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);
                    Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaMonitor = repAlertaMonitor.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);

                    if (cargaEvento != null)
                    {
                        servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }

                    if (alertaMonitor != null)
                    {
                        Servicos.Embarcador.Logistica.AlertaMonitor.EfetuarTratativaAlertaMonitor(alertaMonitor, "Finalizado após finalização do atendimento", unitOfWork);
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(alertaMonitor, null);
                    }

                    if (!string.IsNullOrEmpty(cargaEntrega.Carga.IDIdentificacaoTrizzy) && !string.IsNullOrEmpty(cargaEntrega.IdTrizy) && chamado.MotivoChamado?.BloquearParadaAppTrizy == true)
                    {
                        Task t = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                Repositorio.UnitOfWork _unitOfWork = new Repositorio.UnitOfWork(unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
                                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AlternarBloqueioParada(false, cargaEntrega.Carga.IDIdentificacaoTrizzy, cargaEntrega.IdTrizy, _unitOfWork);
                                _unitOfWork.Dispose();
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                            }
                        });
                    }
                }

                servicoChamado.EnviarEmailChamadoFinalizado(chamado, unitOfWork);

                servicoChamado.EnviarEmailParaTransportadorAoFinalizarChamado(chamado, unitOfWork);

                new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(chamado, TipoNotificacaoApp.TratativaDoAtendimento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o atendimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Chamados.ChamadoAnaliseAnexo, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Chamados.ChamadoAnaliseAnexo, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>(unitOfWork);


                int codigo = Request.GetIntParam("Codigo");

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnaliseAnexo> listaAnexo = repositorioAnexo.BuscarPorEntidade(codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise = repChamadoAnalise.BuscarPorCodigo(codigo);

                if (AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe == TipoServicoMultisoftware && (analise?.NaoRegistrarObservacaoTransportadora ?? false))
                    return new JsonpResult(true, "Você não tem prermissão para acessar esse registro");

                if (analise == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    analise.Codigo,
                    analise.Observacao,
                    analise.NaoRegistrarObservacaoTransportadora,
                    DataAnalise = analise.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                    DataRetorno = analise.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    JustificativaOcorrencia = new { Codigo = analise.JustificativaOcorrencia?.Codigo ?? 0, Descricao = analise.JustificativaOcorrencia?.Descricao ?? string.Empty },
                    SituacaoChamadoAberto = (analise?.Chamado?.Situacao ?? SituacaoChamado.EmTratativa) == SituacaoChamado.Aberto ? true : false,
                    Anexos = (
                        from anexo in listaAnexo
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        })
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Preenche entidade com dados
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise();
                PreencheEntidade(ref analise, unitOfWork);

                bool.TryParse(Request.Params("Responder"), out bool responder);
                Enum.TryParse(Request.Params("ResponsavelOcorrencia"), out ChamadoResponsavelOcorrencia responsavelOcorrencia);
                int.TryParse(Request.Params("Representante"), out int representante);
                bool naoAssumirDataEntregaNota = Request.GetBoolParam("NaoAssumirDataEntregaNota");

                // Valida entidade
                if (!ValidaEntidade(analise, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repChamadoAnalise.Inserir(analise, Auditado);

                if (responder)
                {
                    //if(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    //    analise.Chamado.AosCuidadosDo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ChamadoAosCuidadosDo.Embarcador;
                    //else
                    //{
                    //    if (!analise.Chamado.MotivoChamado.ChamadoDeveSerAbertoPeloEmbarcador)
                    //    {
                    //        analise.Chamado.AosCuidadosDo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ChamadoAosCuidadosDo.Transporador;

                    //        // Emite notificação de retorno
                    //        string nota = "O Chamado " + analise.Chamado.Descricao + " está pendente de sua resposta. Prazo: " + analise.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm");
                    //        serNotificacao.GerarNotificacao(analise.Chamado.Autor, analise.Chamado.Codigo, string.Empty, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.agConfirmacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.yellow, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unitOfWork);
                    //    }
                    //    else
                    //    {
                    //        analise.Chamado.AosCuidadosDo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ChamadoAosCuidadosDo.Embarcador;
                    //    }
                    //}
                }

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = analise.Chamado;
                chamado.Notificado = false;
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    chamado.DataRetorno = analise.DataRetorno;
                    chamado.ResponsavelOcorrencia = responsavelOcorrencia;
                    chamado.Representante = repRepresentante.BuscarPorCodigo(representante);
                }
                else
                    chamado.DataRetorno = null;

                SituacaoChamado situacaoAnterior = chamado.Situacao;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                    (situacaoAnterior == SituacaoChamado.Aberto || situacaoAnterior == SituacaoChamado.EmTratativa))
                    chamado.Situacao = Request.GetEnumParam<SituacaoChamado>("SituacaoTratativa");

                chamado.NaoAssumirDataEntregaNota = naoAssumirDataEntregaNota;
                chamado.NovaMovimentacao = true;

                repChamado.Atualizar(chamado);

                if (chamado.Situacao != situacaoAnterior)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, "Alterado situação do atendimento para " + chamado.Situacao.ObterDescricao(), unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
                servicoChamado.EnviarEmailTransportadorAlteracaoChamado(chamado, analise, unitOfWork);

                return new JsonpResult(new { chamado.Situacao, SituacaoDescricao = chamado.Situacao.ObterDescricao(), Codigo = analise.Codigo });
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorAnaliseDevolucaoCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> cargaEntregaNFeDevolucao = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();

                if (chamado.CargaEntrega != null && chamado.XMLNotasFiscais?.Count > 0)
                {
                    cargaEntregaNFeDevolucao = repCargaEntregaNFeDevolucao.BuscarPorChamado(chamado.Codigo);

                    if (cargaEntregaNFeDevolucao.Count == 0)
                        cargaEntregaNFeDevolucao = repCargaEntregaNFeDevolucao.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo);

                    if (cargaEntregaNFeDevolucao.Count > 0)
                    {
                        List<int> numeroNotasVinculadas = chamado.XMLNotasFiscais.Select(nota => nota.Numero).ToList();
                        cargaEntregaNFeDevolucao = cargaEntregaNFeDevolucao.Where(nota => numeroNotasVinculadas.Contains(nota.XMLNotaFiscal?.Numero ?? 0)).ToList();
                    }
                }

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotas = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(chamado.CargaEntrega?.Codigo ?? 0);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalise = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                DateTime? dataEntregaMesmaCarga = chamadoAnalise.Where(x => x.DataReentregaMesmaCarga.HasValue).Select(x => x.DataReentregaMesmaCarga).FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> informacoesFechamento = repositorioChamadoInformacaoFechamento.BuscarPorChamado(chamado.Codigo);

                //Validar regras 

                var retorno = new
                {
                    chamado.Codigo,
                    NotaFiscal = cargaEntregaNotas != null && cargaEntregaNotas.Count > 0 ? string.Join("", (from notas in cargaEntregaNotas select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero)) : string.Empty,
                    Motivo = chamado.MotivoChamado.Descricao,
                    chamado.Observacao,
                    ClienteEntrega = chamado.CargaEntrega != null ? chamado.CargaEntrega.Cliente.Descricao : string.Empty,
                    TipoDevolucao = chamado.CargaEntrega?.TipoDevolucao ?? TipoColetaEntregaDevolucao.Total,
                    chamado.TratativaDevolucao,
                    MotivoDaDevolucao = new { Codigo = chamado.MotivoDaDevolucao?.Codigo ?? 0, Descricao = chamado.MotivoDaDevolucao?.Descricao ?? "" },
                    ClienteNovaEntrega = new { Codigo = chamado.ClienteNovaEntrega?.Codigo ?? 0, Descricao = chamado.ClienteNovaEntrega?.Nome ?? "" },
                    CodigoSIF = new { Codigo = chamado.ServicoInspecaoFederal?.Codigo ?? 0, Descricao = chamado.ServicoInspecaoFederal?.CodigoSIF ?? "" },
                    chamado.FreteRetornoDevolucao,
                    chamado.ObservacaoRetornoMotorista,
                    NaoAssumirDataEntregaNota = chamado.NaoAssumirDataEntregaNota,
                    MotivoTipoOcorrencia = chamado.CargaEntrega?.MotivoRejeicao?.MotivoChamado?.Descricao ?? string.Empty,
                    DataReentregaMesmaCarga = dataEntregaMesmaCarga?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                    NFeDevolucaoAnalise = chamado.CargaEntrega != null ? (from obj in cargaEntregaNFeDevolucao
                                                                          select new
                                                                          {
                                                                              obj.Codigo,
                                                                              NotaOrigem = obj.XMLNotaFiscal != null ? obj.XMLNotaFiscal.Numero.ToString() : "",
                                                                              CodigoNotaOrigem = obj.XMLNotaFiscal != null ? obj.XMLNotaFiscal.Codigo.ToString() : "",
                                                                              PossuiImagem = !string.IsNullOrEmpty(obj.GuidArquivo),
                                                                              Chave = !string.IsNullOrWhiteSpace(obj.ChaveNFe) ? obj.ChaveNFe : obj.ObservacaoMotorista ?? string.Empty,
                                                                              Numero = obj.Numero > 0 ? obj.Numero.ToString() : string.Empty,
                                                                              Serie = obj.Serie.ToString(),
                                                                              DataEmissao = obj.DataEmissao?.ToDateString() ?? string.Empty,
                                                                              ValorTotalProdutos = obj.ValorTotalProdutos.ToString("n2"),
                                                                              ValorTotal = obj.ValorTotal.ToString("n2"),
                                                                              PesoDevolvido = obj.PesoDevolvido.ToString("n2"),
                                                                              ObservacaoMotorista = obj.ObservacaoMotorista ?? string.Empty
                                                                          }).ToList() : null,
                    InformacoesFechamento = (from obj in informacoesFechamento
                                             select new
                                             {
                                                 obj.Codigo,
                                                 CodigoNotaFiscal = obj.XMLNotaFiscal.Codigo,
                                                 NotaFiscal = obj.XMLNotaFiscal.Numero,
                                                 CodigoMotivoProcesso = obj.MotivoProcesso.Codigo,
                                                 MotivoProcesso = obj.MotivoProcesso.Descricao,
                                                 obj.QuantidadeDivergencia
                                             }).ToList(),
                    SituacaoChamadoAberto = chamado.Situacao == SituacaoChamado.Aberto,
                    AprovarValorChamado = true,
                    HabilitarSenhaDevolucao = chamado.MotivoChamado.HabilitarSenhaDevolucao,
                    SenhaDevolucao = chamado.SenhaDevolucao ?? string.Empty,
                };

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
        public async Task<IActionResult> DownloadImagemNFDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao cargaEntregaNFeDevolucao = repCargaEntregaNFeDevolucao.BuscarPorCodigo(codigo);

                // Valida
                if (cargaEntregaNFeDevolucao == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string extensao = ".jpg";
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "NotasDevolucao" });
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaEntregaNFeDevolucao.GuidArquivo + extensao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/jpg", cargaEntregaNFeDevolucao.Codigo.ToString() + extensao);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTratativasDevolucao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> tipos = null;
                bool? integracaoTransportador = Request.GetNullableBoolParam("IntegracaoTransportador");

                if (!string.IsNullOrWhiteSpace(Request.Params("Tipos")))
                    tipos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>>(Request.Params("Tipos"));

                Repositorio.Embarcador.Chamados.TratativasAnaliseDevolucao repTratativasAnaliseDevolucao = new Repositorio.Embarcador.Chamados.TratativasAnaliseDevolucao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Chamados.TratativasAnaliseDevolucao> trataticasDevolucao = repTratativasAnaliseDevolucao.BuscarPorTipos(tipos);

                return new JsonpResult((from obj in trataticasDevolucao
                                        orderby obj.TratativaDevolucao
                                        select new
                                        {
                                            Codigo = obj.TratativaDevolucao,
                                            Descricao = obj.TratativaDevolucao.ObterDescricaoTratativaDevolucao()
                                        }).ToList()); ;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os tipos de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarAnaliseDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ServicoInspecaoFederal repositorioSIF = new Repositorio.Embarcador.Cargas.ServicoInspecaoFederal(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                double cpfCnpjClienteNovaEntrega = Request.GetDoubleParam("ClienteNovaEntrega");
                bool informarDadosChamadoFinalizadoComCusto = Request.GetBoolParam("InformarDadosChamadoFinalizadoComCusto");
                bool naoAssumirDataEntregaNota = Request.GetBoolParam("NaoAssumirDataEntregaNota");
                DateTime? novaDataPrevisaoEntrega = Request.GetNullableDateTimeParam("DataPrevisaoEntregaPedidos");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (chamado.Situacao == SituacaoChamado.AgIntegracao || chamado.Situacao == SituacaoChamado.FalhaIntegracao)
                    throw new ControllerException("Esse atendimento já está no fluxo de integração!");

                if (!PossuiAnaliseParaEfetuarOperacao(chamado, unitOfWork))
                    throw new ControllerException("Favor informar uma análise antes de efetuar a operação.");

                if (chamado.MotivoChamado.PermitirAtualizarInformacoesPedido && (!novaDataPrevisaoEntrega.HasValue || novaDataPrevisaoEntrega < DateTime.Now))
                    throw new ControllerException("Data de previsão deve ser fornecida e ser maior que a data/hora atual.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                bool devolucaoParcial = Request.GetEnumParam("TipoDevolucao", TipoColetaEntregaDevolucao.Total) == TipoColetaEntregaDevolucao.Parcial;

                chamado.TratativaDevolucao = Request.GetEnumParam("TratativaDevolucao", SituacaoEntrega.Rejeitado);

                int codigoMotivoDaDevolucao = Request.GetIntParam("MotivoDaDevolucao");
                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDaDevolucao = codigoMotivoDaDevolucao > 0 ? repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(codigoMotivoDaDevolucao) : null;
                chamado.MotivoDaDevolucao = motivoDaDevolucao;

                int codigoSIF = Request.GetIntParam("CodigoSIF");

                chamado.ServicoInspecaoFederal = codigoSIF > 0 ? repositorioSIF.BuscarPorCodigo(codigoSIF, false) : null;

                bool selecaoRetornoFreteDevolucao = Request.GetBoolParam("FreteRetornoDevolucao");
                chamado.FreteRetornoDevolucao = selecaoRetornoFreteDevolucao;

                string senhaDevolucao = Request.GetStringParam("SenhaDevolucao");
                chamado.SenhaDevolucao = senhaDevolucao;

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
                if (devolucaoParcial && chamado.TratativaDevolucao != SituacaoEntrega.Reentergue)
                    chamado.TratativaDevolucao = configuracaoChamado.FinalizarEntregaQuandoDevolucaoParcial ? SituacaoEntrega.Entregue : SituacaoEntrega.NaoEntregue;

                chamado.ObservacaoRetornoMotorista = Request.GetNullableStringParam("ObservacaoRetornoMotorista");

                if (chamado.TratativaDevolucao == SituacaoEntrega.EntregarEmOutroCliente)
                {
                    chamado.ClienteNovaEntrega = repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteNovaEntrega);
                    if (chamado.ClienteNovaEntrega == null)
                        throw new ControllerException("Quando tratativa for Entregar em outro cliente deve-se informar o cliente.");
                }
                else
                    chamado.ClienteNovaEntrega = null;

                chamado.InformarDadosChamadoFinalizadoComCusto = informarDadosChamadoFinalizadoComCusto;
                chamado.NaoAssumirDataEntregaNota = naoAssumirDataEntregaNota;
                if (chamado.MotivoChamado.PermitirAtualizarInformacoesPedido)
                    chamado.DataPrevisaoEntregaPedidos = novaDataPrevisaoEntrega;

                SalvarCriticidadeChamado(chamado, unitOfWork);

                if (informarDadosChamadoFinalizadoComCusto)
                    SalvarInformacaoFechamento(chamado, unitOfWork);

                bool notificarDiferencaDevolucao = false;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;

                dynamic itensDevolver = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensDevolver"));

                List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notasDevolver = servicoControleEntrega.ConverterDevolucaoNotasFiscais(itensDevolver).GetAwaiter().GetResult();

                servicoControleEntrega.SalvarDevolucaoCargaEntrega(cargaEntrega, notasDevolver, servicoControleEntrega.ConverterDevolucaoProdutos(itensDevolver), chamado.MotivoChamado, chamado, ConfiguracaoEmbarcador, Auditado, chamado.TratativaDevolucao, devolucaoParcial, true, configuracaoChamado, TipoServicoMultisoftware);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises?.FirstOrDefault();

                if (cargaEntrega != null)
                {
                    if (cargaEntrega.DevolucaoParcial != devolucaoParcial)
                        notificarDiferencaDevolucao = true;

                    cargaEntrega.DevolucaoParcial = devolucaoParcial;

                    TipoMotivoAtendimento tipoMotivoAtendimento = chamado.MotivoChamado.TipoMotivoAtendimento;

                    if (tipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao || (devolucaoParcial && chamado.TratativaDevolucao == SituacaoEntrega.Reentergue) ||
                        (tipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga && (chamado.TratativaDevolucao == SituacaoEntrega.NaoEntregue || chamado.TratativaDevolucao == SituacaoEntrega.Rejeitado)))//Mesma condição do SalvarDevolucaoCargaEntrega, já que o salvar está lá
                    {
                        SalvarNFeDevolucao(cargaEntrega, unitOfWork, chamado);
                        GerarControleNotaDevolucao(chamado, unitOfWork);
                    }

                    chamado.NotificacaoMotoristaMobile = true;

                    cargaEntrega.NotificarDiferencaDevolucao = notificarDiferencaDevolucao;

                    if (chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga)
                    {
                        chamadoAnalise.DataReentregaMesmaCarga = Request.GetNullableDateTimeParam("DataReentregaMesmaCarga");
                        cargaEntrega.DataReentregaEmMesmaCarga = chamadoAnalise.DataReentregaMesmaCarga;
                    }

                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                }

                servicoChamado.ProcessarFinalizacaoAnaliseDevolucao(chamado, unitOfWork, Auditado, TipoServicoMultisoftware, Cliente);

                if (!string.IsNullOrWhiteSpace(chamado.CargaEntrega?.IdTrizy))
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarChamadoOcorrencia(chamado, EventoIntegracaoOcorrenciaTrizy.SalvarTratativa, this.Usuario, unitOfWork);

                if (ShouldGerarOcorrencia(chamado) && selecaoRetornoFreteDevolucao)
                    GerarOcorrencia(chamado, chamado.MotivoChamado, unitOfWork, notasDevolver);

                repChamado.Atualizar(chamado);

                unitOfWork.CommitChanges();

                if (chamado.NotificacaoMotoristaMobile)
                {
                    dynamic conteudo = ObterObjetoNotificacao(chamado, chamado.ObservacaoRetornoMotorista, chamado.Situacao, notificarDiferencaDevolucao);

                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);
                servicoChamado.NotificarChamadoPedidoPortalFornecedor(chamado, devolucaoParcial, unitOfWork, Cliente);

                if ((chamado.Situacao == SituacaoChamado.LiberadaOcorrencia || chamado.Situacao == SituacaoChamado.Finalizado) && cargaEntrega != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);
                    if (cargaEvento != null)
                    {
                        servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && (chamado?.AguardandoTratativaDoCliente ?? false))
                    chamado.AguardandoTratativaDoCliente = false;

                if (chamado.Situacao == SituacaoChamado.Finalizado)
                    servicoChamado.EnviarEmailChamadoFinalizado(chamado, unitOfWork);

                if (
                    cargaEntrega != null &&
                    !string.IsNullOrEmpty(cargaEntrega.IdTrizy) &&
                    (
                        chamado.TratativaDevolucao == SituacaoEntrega.Reentergue ||
                        (
                            chamado.TratativaDevolucao == SituacaoEntrega.Rejeitado &&
                            !cargaEntrega.DevolucaoParcial
                        )
                    )
                )
                {
                    Task t = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Repositorio.UnitOfWork unitOfWorkTask = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
                            Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AtualzarEntrega(cargaEntrega, _conexao.StringConexao, configuracaoTMS, true, unitOfWorkTask);
                            unitOfWorkTask.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }
                    });
                }

                return new JsonpResult(chamado.Codigo);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar análise.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRelatorioAnalises()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unidadeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigoChamado);
                bool liberouOcorrencia = Request.GetBoolParam("LiberouOcorrencia");

                Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado);

                string mensagemErro = string.Empty;

                byte[] pdf = ReportRequest.WithType(ReportType.RelatorioChamados)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoChamado", chamado.Codigo.ToString())
                    .CallReport().GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, mensagemErro);

                string fileName = "Análises do Atendimento - " + chamado.Numero + ".pdf";
                if (ConfiguracaoEmbarcador.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado && liberouOcorrencia)
                    SalvarRelatorioAnaliseNosAnexos(chamado, pdf, fileName, unidadeTrabalho);

                return Arquivo(pdf, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download das análises do atendimento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EstornarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (((configuracaoChamado?.BloquearEstornoAtendimentosFinalizadosPortalTransportador ?? false) || chamado.MotivoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, true, "O estorno está bloqueado por configuração do administrador.");

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "Chamado não possui responsável.");

                //if (chamado.Responsavel.Codigo != this.Usuario.Codigo && (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermitirEstornarOcorrencia) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ReAbrir)))
                //    return new JsonpResult(false, true, "Usuário sem permissão para esse chamado.");

                if (!ConfiguracaoEmbarcador.PermitirEstornarAprovacaoChamadoLiberado && !chamado.MotivoChamado.PermiteEstornarAtendimento)
                    return new JsonpResult(false, true, "A funcionalidade de estornar não está liberada para esse atendimento.");

                if (!ValidaEstornoChamado(chamado, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                if (chamado.Situacao == SituacaoChamado.Aberto || chamado.Situacao == SituacaoChamado.EmTratativa)
                    return new JsonpResult(false, true, "Esse chamado já está estornado.");

                if (chamado.Situacao == SituacaoChamado.AgIntegracao || chamado.Situacao == SituacaoChamado.FalhaIntegracao)
                    return new JsonpResult(false, true, "Esse atendimento já está no fluxo de integração!");

                if (repPagamentoMotorista.ContemPagamentoFinalizadoPorChamado(chamado.Codigo))
                    return new JsonpResult(false, true, "Esse chamado possui Pagamento de Motorista finalizado, favor reverta o mesmo antes de estornar o chamado.");

                if (repPagamentoMotorista.ContemPagamentoAutorizacaoPendente(chamado.Codigo))
                    return new JsonpResult(false, true, "Esse chamado possui um pagamento com autorização pendente, favor reverta o mesmo antes de estornar o chamado.");

                unitOfWork.Start();

                chamado.Situacao = SituacaoChamado.Aberto;
                chamado.DataFinalizacao = null;
                chamado.Estornado = true;
                chamado.DataEstorno = DateTime.Now;
                chamado.UsuarioEstorno = Usuario;

                RetornarNotaParaUltimaSituacaoAntesDaAberturaDoChamado(chamado, unitOfWork);

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();

                if (chamado.NotificacaoMotoristaMobile)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();

                    dynamic conteudo = ObterObjetoNotificacao(chamado, chamadoAnalise?.Observacao, SituacaoChamado.Aberto, chamado.CargaEntrega.NotificarDiferencaDevolucao);
                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao estornar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecusarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoMotivoRecusaCancelamento = Request.GetIntParam("MotivoRecusaCancelamento");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "Chamado não possui responsável.");

                if (chamado.Responsavel.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, true, "Usuário sem permissão para esse chamado.");

                if (!ValidaFinalizacaoChamado(chamado, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                if (chamado.Situacao == SituacaoChamado.Finalizado)
                    return new JsonpResult(false, true, "Esse chamado já está finalizado.");

                if (chamado.Situacao == SituacaoChamado.RecusadoPeloCliente)
                    return new JsonpResult(false, true, "Esse chamado já está recusado.");

                if (chamado.Situacao == SituacaoChamado.AgIntegracao || chamado.Situacao == SituacaoChamado.FalhaIntegracao)
                    return new JsonpResult(false, true, "Esse atendimento já está no fluxo de integração!");

                chamado.MotivoRecusaCancelamento = codigoMotivoRecusaCancelamento > 0 ? repMotivoRecusaCancelamento.BuscarPorCodigo(codigoMotivoRecusaCancelamento) : null;
                chamado.Situacao = SituacaoChamado.RecusadoPeloCliente;
                chamado.DataFinalizacao = DateTime.Now;

                unitOfWork.Start();

                if (chamado.CargaEntrega != null)
                {
                    chamado.NotificacaoMotoristaMobile = true;
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    chamado.CargaEntrega.ChamadoEmAberto = false;
                    chamado.CargaEntrega.MotivoRejeicao = null;

                    if (!chamado.CargaEntrega.DataConfirmacao.HasValue)
                        chamado.CargaEntrega.Situacao = SituacaoEntrega.NaoEntregue;
                    else
                    {
                        chamado.CargaEntrega.Situacao = SituacaoEntrega.Entregue;
                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                        repositorioXmlNotaFiscal.AtualizarSituacaoNotasFiscaisPorEntrega(chamado.CargaEntrega.Codigo, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Entregue);
                    }

                    repCargaEntrega.Atualizar(chamado.CargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(chamado.CargaEntrega, repCargaEntrega, unitOfWork);

                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NaoPossuiEntregasPendentes(chamado.CargaEntrega.Carga, unitOfWork))
                    {
                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(chamado.CargaEntrega.Carga.Codigo, chamado.CargaEntrega.DataRejeitado ?? DateTime.Now, Auditado, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, unitOfWork))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado.CargaEntrega.Carga, $"Fim de viagem informado automaticamente ao recusar atendimento", unitOfWork);
                    }
                }

                repChamado.Atualizar(chamado, Auditado);
                unitOfWork.CommitChanges();

                if (chamado.NotificacaoMotoristaMobile)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();

                    dynamic conteudo = ObterObjetoNotificacao(chamado, chamadoAnalise?.Observacao, SituacaoChamado.Cancelada, chamado.CargaEntrega.NotificarDiferencaDevolucao);
                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao recusar o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Responsavel == null)
                    return new JsonpResult(false, true, "Chamado não possui responsável.");

                if (chamado.Responsavel.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, true, "Usuário sem permissão para esse chamado.");

                if (!chamado.MotivoChamado.PermiteRetornarParaAjuste)
                    return new JsonpResult(false, true, "A funcionalidade de retornar para ajuste não está liberada para esse atendimento.");

                if (chamado.Situacao != SituacaoChamado.Aberto && chamado.Situacao != SituacaoChamado.EmTratativa)
                    return new JsonpResult(false, true, "Esse chamado não está mais em aberto.");

                chamado.Responsavel = null;

                unitOfWork.Start();

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();

                if (chamado.NotificacaoMotoristaMobile)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();

                    dynamic conteudo = ObterObjetoNotificacao(chamado, chamadoAnalise?.Observacao, SituacaoChamado.Aberto, chamado.CargaEntrega.NotificarDiferencaDevolucao);
                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoAtualizado(chamado, chamadoAnalise, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInformacoesFechamentoEtapaAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                int codigoChamado = Request.GetIntParam("Codigo");

                bool informarDadosChamadoFinalizadoComCusto = Request.GetBoolParam("InformarDadosChamadoFinalizadoComCusto");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado, true);

                if (chamado == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (!PossuiAnaliseParaEfetuarOperacao(chamado, unitOfWork))
                    throw new ControllerException("Favor informar uma análise antes de efetuar a operação.");

                chamado.InformarDadosChamadoFinalizadoComCusto = informarDadosChamadoFinalizadoComCusto;

                SalvarInformacaoFechamento(chamado, unitOfWork);

                repositorioChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Informações de Fechamento Alterados com Sucesso");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar informações de fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCamposVisiveisEtapaAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoIntegracao repositorioChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                int codigoChamado = Request.GetIntParam("Codigo");

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> listaChamadoIntegracao = repositorioChamadoIntegracao.BuscarPorChamado(codigoChamado);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado, true);

                int quantiaIntegrada = 0;
                int quantiaNaoIntegrada = 0;

                foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao in listaChamadoIntegracao)
                {
                    if (chamadoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado || chamadoIntegracao.SituacaoIntegracao == SituacaoIntegracao.AgRetorno)
                        quantiaIntegrada++;
                    else
                        quantiaNaoIntegrada++;
                }

                dynamic dyn = new
                {
                    SalvarInformacoesFechamento = chamado?.Situacao == SituacaoChamado.Finalizado && !repositorioChamadoInformacaoFechamento.PossuiPorChamado(codigoChamado),
                    PermiteReenviarIntegracoesInformacoesFechamento = true
                };

                return new JsonpResult(dyn);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao validar campos visíveis da análise.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarProdutosNotasFiscaisAnaliseDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic produtosDevolucao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));
                int codigoChamado = Request.GetIntParam("Chamado");
                int codigoNotaFiscal = Request.GetIntParam("NotaFiscal");
                bool devolucaoParcial = Request.GetBoolParam("DevolucaoParcial");
                int codigoMotivoDevolucao = Request.GetIntParam("MotivoDaDevolucao");

                List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos = ConverterDevolucaoNotaFiscalProdutos(produtosDevolucao);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado repCargaEntregaNotaFiscalChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado, true);
                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega = repMotivoDevolucaoEntrega.BuscarPorCodigo(codigoMotivoDevolucao);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repCargaEntregaNotaFiscal.BuscarPorCodigo(codigoNotaFiscal);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado cargaEntregaNotaFiscalChamado = repCargaEntregaNotaFiscalChamado.BuscarPorChamadoECargaEntregaNotaFiscal(codigoChamado, codigoNotaFiscal);

                if (chamado == null || configuracaoChamado == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao salvar os produtos.");

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                bool devolucaoPorPeso = chamado.CargaEntrega.Carga.TipoOperacao?.DevolucaoProdutosPorPeso ?? false;

                unitOfWork.Start();

                servicoControleEntrega.SalvarProdutosNotasFiscaisAnaliseDevolucao(produtos, devolucaoParcial, devolucaoPorPeso, configuracaoChamado, Auditado, codigoNotaFiscal, chamado);

                if (cargaEntregaNotaFiscalChamado != null)
                {
                    cargaEntregaNotaFiscalChamado.MotivoDaDevolucao = motivoDevolucaoEntrega;
                    repCargaEntregaNotaFiscalChamado.Atualizar(cargaEntregaNotaFiscalChamado);
                }
                else if (cargaEntregaNotaFiscal != null)
                {
                    if (motivoDevolucaoEntrega != null)
                        cargaEntregaNotaFiscal.MotivoDaDevolucao = motivoDevolucaoEntrega;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && devolucaoParcial)
                    {
                        cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.DevolvidaParcial;
                        cargaEntregaNotaFiscal.CargaEntrega.DevolucaoParcial = true;
                    }
                    repCargaEntregaNotaFiscal.Atualizar(cargaEntregaNotaFiscal);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os produtos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterObjetoNotificacao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, string observacao, SituacaoChamado situacao, bool notificarDiferencaDevolucao)
        {
            return new
            {
                chamado.Codigo,
                Observacao = observacao ?? "",
                CargaEntrega = chamado.CargaEntrega.Codigo,
                ClienteMultisoftware = Cliente.Codigo,
                SituacaoChamado = situacao,
                SituacaoCargaEntrega = chamado.CargaEntrega.Situacao,
                DiferencaDevolucao = notificarDiferencaDevolucao,
                NumeroAtendimento = chamado.Numero
            };
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia repositorioJustificativaOcorrencia = new Repositorio.Embarcador.Ocorrencias.JustificativaOcorrencia(unitOfWork);

            int codigoChamado = Request.GetIntParam("Codigo");
            int codigoJustificativaOcorrencia = Request.GetIntParam("JustificativaOcorrencia");

            DateTime.TryParse(Request.Params("DataRetorno"), out DateTime dataRetorno);

            analise.Chamado = repChamado.BuscarPorCodigo(codigoChamado);
            analise.Autor = this.Usuario;
            analise.Observacao = Request.GetStringParam("Observacao");
            analise.NaoRegistrarObservacaoTransportadora = Request.GetBoolParam("NaoRegistrarObservacaoTransportadora");
            analise.DataCriacao = DateTime.Now;
            analise.DataRetorno = dataRetorno;
            analise.JustificativaOcorrencia = codigoJustificativaOcorrencia > 0 ? repositorioJustificativaOcorrencia.BuscarPorCodigo(codigoJustificativaOcorrencia, false) : null;
            analise.DataReentregaMesmaCarga = Request.GetNullableDateTimeParam("DataReentregaMesmaCarga");
            analise.Chamado.NaoAssumirDataEntregaNota = Request.GetBoolParam("NaoAssumirDataEntregaNota");
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ChamadoAnalise.Autor, "Autor", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ChamadoAnalise.Data, "DataCriacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ChamadoAnalise.Retorno, "DataRetorno", 10, Models.Grid.Align.center, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Codigo"), out int chamado);

            // Consulta
            List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> listaGrid = repChamadoAnalise.Consultar(chamado, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repChamadoAnalise.ContarConsulta(chamado);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Autor = obj.Autor.Nome,
                            DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                            DataRetorno = obj.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
                        };

            return lista.ToList();
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(analise.Observacao))
            {
                msgErro = "Nenhuma observação escrita.";
                return false;
            }

            if (!analise.DataRetorno.HasValue || analise.DataRetorno.Value == DateTime.MinValue)
            {
                msgErro = "Data Retorno é obrigatório.";
                return false;
            }

            if (analise.Chamado.Situacao != SituacaoChamado.Aberto && analise.Chamado.Situacao != SituacaoChamado.EmTratativa)
            {
                msgErro = "A situação não permite essa operação.";
                return false;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !analise.Chamado.Analistas.Contains(this.Usuario))
            {
                if (analise.Chamado.UsuarioLiberacao != null && analise.Chamado.UsuarioLiberacao?.Codigo != this.Usuario.Codigo)
                {
                    msgErro = "Usuário sem permissão para adicionar informação.";
                    return false;
                }
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Carga") propOrdenar = "Carga.CodigoCargaEmbarcador";
            else if (propOrdenar == "Cliente") propOrdenar = "Cliente.Nome";
            else if (propOrdenar == "Tomador") propOrdenar = "Tomador.Nome";
        }

        private bool ValidaFinalizacaoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            erro = "";
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repChamadoOcorrencia.BuscarOcorrenciasPorChamado(chamado.Codigo);
            if (!ocorrencias.All(obj => obj.SituacaoOcorrencia == SituacaoOcorrencia.Finalizada || obj.SituacaoOcorrencia == SituacaoOcorrencia.Cancelada || (obj.SituacaoOcorrencia == SituacaoOcorrencia.Rejeitada && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)))
            {
                erro = "Não é possível finalizar o atendimento, pois ainda existem ocorrências em andamento.";
                return false;
            }

            return true;
        }

        private bool ValidaEstornoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, out string erro, Repositorio.UnitOfWork unitOfWork)
        {
            erro = "";
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repChamadoOcorrencia.BuscarOcorrenciasPorChamado(chamado.Codigo);
            if (ocorrencias.Any(obj => obj.SituacaoOcorrencia != SituacaoOcorrencia.Rejeitada && obj.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada))
            {
                erro = "Não é possível estornar o atendimento, pois existem ocorrências em andamento.";
                return false;
            }

            return true;
        }

        private string GerarOcorrencia(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notasDevolver = null)
        {
            string mensagemRetorno = string.Empty;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia servicoOcorrenciaCalculoFrete = new Servicos.Embarcador.Carga.Ocorrencia();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = chamado.Carga;

            if (carga == null)
                throw new ControllerException("Não foi selecionada a carga para geração da ocorrência.");

            bool efetuarCalculoValorOcorrenciaComBaseNasNotas = (motivoChamado.TipoOcorrencia.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao ?? false);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega)
                cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, true, false, true, false, true, 0, chamado.Destinatario.CPF_CNPJ, true, false, 0);
            else if (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEsRetencao = repCargaCTe.BuscarPorCarga(carga.Codigo, true, false, true, false, true, 0, chamado.Destinatario.CPF_CNPJ, true, false, 0);
                if (cargaCTEsRetencao != null && cargaCTEsRetencao.Count > 0)
                    cargaCTEs.Add(cargaCTEsRetencao.FirstOrDefault());
            }
            else if (chamado.Valor > 0 || (motivoChamado?.CalcularOcorrenciaPorTabelaFrete ?? false) || efetuarCalculoValorOcorrenciaComBaseNasNotas)
            {
                bool retornarPreCtes = false;
                if (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe)
                    retornarPreCtes = true;
                cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, false, true, 0, chamado.Destinatario?.CPF_CNPJ ?? 0, retornarPreCtes);
            }

            if (cargaCTEs == null || cargaCTEs.Count == 0)
                throw new ControllerException("Não encontrado CTes autorizados para geração da ocorrência.");

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(motivoChamado.TipoOcorrencia.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia = null;

            bool dividirOcorrencia = false;
            TipoMotivoAtendimento tipoMotivoAtendimento = motivoChamado.TipoMotivoAtendimento;
            bool gerarOcorrenciaCalculandoPorTabelaFrete = tipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega || tipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || tipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem || motivoChamado.CalcularOcorrenciaPorTabelaFrete;

            if (gerarOcorrenciaCalculandoPorTabelaFrete || efetuarCalculoValorOcorrenciaComBaseNasNotas)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroBooleano = tipoOcorrencia.ParametrosOcorrencia.FirstOrDefault(o => o.TipoParametro == TipoParametroOcorrencia.Booleano);
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroInteiro = tipoOcorrencia.ParametrosOcorrencia.FirstOrDefault(o => o.TipoParametro == TipoParametroOcorrencia.Inteiro);
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroPeriodo = tipoOcorrencia.ParametrosOcorrencia.FirstOrDefault(o => o.TipoParametro == TipoParametroOcorrencia.Periodo);

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
                {
                    ApenasReboque = chamado.RetencaoBau,
                    CodigoCarga = chamado.Carga.Codigo,
                    CodigoParametroBooleano = parametroBooleano?.Codigo ?? 0,
                    CodigoParametroInteiro = parametroInteiro?.Codigo ?? 0,
                    CodigoParametroPeriodo = parametroPeriodo?.Codigo ?? 0,
                    CodigoParametroData = 1,
                    CodigoTipoOcorrencia = tipoOcorrencia.Codigo,
                    DataFim = chamado.DataRetencaoFim ?? DateTime.Now,
                    DataInicio = chamado.DataRetencaoInicio ?? DateTime.Now,
                    ParametroData = chamado.DataReentrega ?? DateTime.Now,
                    Minutos = 0,
                    HorasSemFranquia = tipoOcorrencia?.HorasSemFranquia ?? 0,
                    KmInformado = 0,
                    PermiteInformarValor = false,
                    ValorOcorrencia = 0,
                    ListaCargaCTe = cargaCTEs,
                    DevolucaoParcial = chamado.CargaEntrega?.DevolucaoParcial ?? false,
                    CargaEntrega = chamado.CargaEntrega,
                    CargaEntregaNotaFiscals = notasDevolver,
                };

                calculoFreteOcorrencia = servicoOcorrenciaCalculoFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);

                cargaCTEs = parametrosCalcularValorOcorrencia.ListaCargaCTe;

                if (calculoFreteOcorrencia.ValorOcorrenciaDestino > 0)
                    dividirOcorrencia = true;
            }

            Dominio.Entidades.Usuario usuario = null;

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
            cargaOcorrencia.DataOcorrencia = DateTime.Now;
            cargaOcorrencia.DataAlteracao = DateTime.Now;
            cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            cargaOcorrencia.Observacao = chamado.Observacao;
            cargaOcorrencia.Observacao ??= string.Empty;
            cargaOcorrencia.ObservacaoCTe = string.Empty;
            cargaOcorrencia.ObservacaoCTes = string.Empty;
            cargaOcorrencia.Carga = carga;
            cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;
            cargaOcorrencia.OrigemOcorrencia = cargaOcorrencia.TipoOcorrencia.OrigemOcorrencia;
            cargaOcorrencia.ComponenteFrete = cargaOcorrencia.TipoOcorrencia?.ComponenteFrete;
            cargaOcorrencia.ModeloDocumentoFiscal = tipoOcorrencia?.ModeloDocumentoFiscal;

            Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCTe = cargaCTEs.FirstOrDefault();

            if (primeiroCTe?.CTe != null)
                cargaOcorrencia.IncluirICMSFrete = primeiroCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
            else
                cargaOcorrencia.IncluirICMSFrete = false;


            if (motivoChamado.CalcularOcorrenciaPorTabelaFrete || gerarOcorrenciaCalculandoPorTabelaFrete || efetuarCalculoValorOcorrenciaComBaseNasNotas)
                cargaOcorrencia.ValorOcorrencia = calculoFreteOcorrencia?.ValorOcorrencia ?? 0;
            else if (motivoChamado.TipoMotivoAtendimento != TipoMotivoAtendimento.Reentrega && motivoChamado.TipoMotivoAtendimento != TipoMotivoAtendimento.Retencao)
            {
                cargaOcorrencia.ValorOcorrencia = chamado.Valor;
                cargaOcorrencia.ValorOcorrenciaOriginal = chamado.Valor;
            }
            else if (motivoChamado.GerarCTeComValorIgualCTeAnterior && chamado.TratativaDevolucao == SituacaoEntrega.Reentergue)
            {
                cargaOcorrencia.ValorOcorrencia = cargaCTEs.Sum(o => o.CTe.ValorAReceber);
                cargaOcorrencia.ValorOcorrenciaOriginal = cargaOcorrencia.ValorOcorrencia;
            }

            repCargaOcorrencia.Inserir(cargaOcorrencia);

            if (cargaOcorrencia.TipoOcorrencia.BloqueiaOcorrenciaDuplicada)
            {
                if (cargaOcorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorPeriodo(cargaOcorrencia, out string erro, unitOfWork, this.Usuario))
                        throw new ControllerException(erro);
                }
                else
                {
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, cargaOcorrencia, out string erro, unitOfWork, TipoServicoMultisoftware, chamado.Codigo))
                        throw new ControllerException(erro);
                }
            }

            Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
            {
                CargaOcorrencia = cargaOcorrencia,
                Chamado = chamado
            };

            repChamadoOcorrencia.Inserir(chamadoOcorrencia);

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, cargaCTEs, unitOfWork);

            if (calculoFreteOcorrencia != null)
                GerarParametrosOcorrencia(cargaOcorrencia, chamado, tipoOcorrencia, calculoFreteOcorrencia, false, unitOfWork);

            if (!srvOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, usuario, configuracaoEmbarcador, Cliente, "", false, false, Auditado))
                throw new ControllerException(mensagemRetorno);

            if (dividirOcorrencia && calculoFreteOcorrencia.ValorOcorrenciaDestino > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaDestino = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();

                string observacaoOcorrenciaDestino = calculoFreteOcorrencia.ObservacaoOcorrenciaDestino;
                string observacaoCTeDestino = calculoFreteOcorrencia.ObservacaoCTeDestino;

                CopiarOcorrencia(cargaOcorrencia, ref ocorrenciaDestino);

                if (!string.IsNullOrWhiteSpace(observacaoOcorrenciaDestino))
                    ocorrenciaDestino.Observacao = string.Concat(ocorrenciaDestino.Observacao, " / ", observacaoOcorrenciaDestino);

                ocorrenciaDestino.ObservacaoCTe = observacaoCTeDestino;
                ocorrenciaDestino.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                ocorrenciaDestino.ComponenteFrete = ocorrenciaDestino.TipoOcorrencia?.ComponenteFrete;
                ocorrenciaDestino.CargaOcorrenciaVinculada = cargaOcorrencia.Codigo;
                ocorrenciaDestino.Responsavel = Dominio.Enumeradores.TipoTomador.Destinatario;
                ocorrenciaDestino.ValorOcorrencia = calculoFreteOcorrencia.ValorOcorrenciaDestino;
                ocorrenciaDestino.ValorOcorrenciaOriginal = calculoFreteOcorrencia.ValorOcorrenciaDestino;

                if (ocorrenciaDestino.ValorOcorrencia > 0)
                {
                    repCargaOcorrencia.Inserir(ocorrenciaDestino);

                    Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrenciaDestino = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                    {
                        CargaOcorrencia = ocorrenciaDestino,
                        Chamado = chamado
                    };

                    repChamadoOcorrencia.Inserir(chamadoOcorrenciaDestino);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaDestino, "Adicionou ocorrência pelo atendimento " + chamado.Descricao, unitOfWork);

                    GerarParametrosOcorrencia(ocorrenciaDestino, chamado, tipoOcorrencia, calculoFreteOcorrencia, true, unitOfWork);

                    mensagemRetorno = string.Empty;

                    if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrenciaDestino, cargaCTEs, null, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, this.Usuario, this.ConfiguracaoEmbarcador, this.Cliente, Request.Params("CargaCTesImportados"), false, false, Auditado))
                        unitOfWork.Rollback();
                    else
                    {
                        cargaOcorrencia.CargaOcorrenciaVinculada = ocorrenciaDestino.Codigo;

                        if (cargaOcorrencia.ValorOcorrencia == 0)
                            cargaOcorrencia.Inativa = true;
                    }
                }
            }

            repCargaOcorrencia.Atualizar(cargaOcorrencia);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, "Adicionou ocorrência pelo atendimento " + chamado.Descricao, unitOfWork);

            unitOfWork.CommitChanges();

            return mensagemRetorno;
        }

        private void GerarParametrosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia, bool ocorrenciaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);


            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroBooleano = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                              where obj.TipoParametro == TipoParametroOcorrencia.Booleano
                                                                                              select obj).FirstOrDefault();

            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroInteiro = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                             where obj.TipoParametro == TipoParametroOcorrencia.Inteiro
                                                                                             select obj).FirstOrDefault();

            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroPeriodo = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                             where obj.TipoParametro == TipoParametroOcorrencia.Periodo
                                                                                             select obj).FirstOrDefault();

            Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroTexto = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                           where obj.TipoParametro == TipoParametroOcorrencia.Texto
                                                                                           select obj).FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> parametrosData = (from obj in tipoOcorrencia.ParametrosOcorrencia
                                                                                                 where obj.TipoParametro == TipoParametroOcorrencia.Data
                                                                                                 select obj).ToList();

            int codigoParametroPeriodo = parametroPeriodo?.Codigo ?? 0; // int.TryParse(Request.Params("CodigoParametroPeriodo"), out int codigoParametroPeriodo);
            int codigoParametroInteiro = parametroInteiro?.Codigo ?? 0; // int.TryParse(Request.Params("CodigoParametroInteiro"), out int codigoParametroInteiro);
            int codigoParametroBooleano = parametroBooleano?.Codigo ?? 0; // int.TryParse(Request.Params("CodigoParametroBooleano"), out int codigoParametroBooleano);
            int codigoParametroData1 = parametrosData != null && parametrosData.Count == 1 ? parametrosData[0].Codigo : 0; //int.TryParse(Request.Params("CodigoParametroData1"), out int codigoParametroData1);
            int codigoParametroData2 = parametrosData != null && parametrosData.Count == 2 ? parametrosData[1].Codigo : 0; //int.TryParse(Request.Params("CodigoParametroData2"), out int codigoParametroData2);
            int codigoParametroTexto = parametroTexto?.Codigo ?? 0; //int.TryParse(Request.Params("CodigoParametroTexto"), out int codigoParametroTexto);

            string textoParametroTexto = chamado.Descricao; ////string parametroTexto = Request.Params("ParametroTexto") ?? string.Empty;

            int textoParametroInteiro = 0;//int.TryParse(Request.Params("ParametroInteiro"), out int textoParametroInteiro);

            bool apenasReboque = chamado.RetencaoBau; //bool.TryParse(Request.Params("ApenasReboque"), out bool apenasReboque);


            decimal horasOcorrencia = !ocorrenciaDestino ? calculoFreteOcorrencia.HorasOcorrencia : calculoFreteOcorrencia.HorasOcorrenciaDestino;  //string hora = Request.Params("HorasOcorrencia") ?? string.Empty; //decimal.TryParse(!string.IsNullOrEmpty(hora) ? hora.Replace(".", ",") : "0", out decimal horasOcorrencia);

            //decimal horasOcorrenciaDestino = calculoFreteOcorrencia.HorasOcorrenciaDestino;  //hora = Request.Params("HorasOcorrenciaDestino"); //decimal.TryParse(!string.IsNullOrEmpty(hora) ? hora.Replace(".", ",") : "0", out decimal horasOcorrenciaDestino);

            DateTime dataInicio = chamado.DataRetencaoInicio.HasValue ? chamado.DataRetencaoInicio.Value : DateTime.MinValue;// DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
            DateTime dataFim = chamado.DataRetencaoFim.HasValue ? chamado.DataRetencaoFim.Value : DateTime.MinValue;//  DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);
            DateTime parametroData1 = chamado.DataCriacao; //DateTime.TryParseExact(Request.Params("ParametroData1"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parametroData1);
            DateTime parametroData2 = chamado.DataReentrega.HasValue ? chamado.DataReentrega.Value : DateTime.MinValue; // DateTime.TryParseExact(Request.Params("ParametroData2"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parametroData2);


            if (codigoParametroPeriodo > 0 && dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametros = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroPeriodo),
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    TotalHoras = horasOcorrencia
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametros);
            }

            if (codigoParametroBooleano > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosBooleano = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroBooleano),
                    Booleano = apenasReboque
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosBooleano);
            }

            if (codigoParametroInteiro > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosInteiro = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroInteiro),
                    Texto = textoParametroInteiro.ToString()
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosInteiro);
            }

            if (codigoParametroData1 > 0 && (parametroData1 > DateTime.MinValue))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosData1 = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroData1),
                    Data = parametroData1
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosData1);
            }

            if (codigoParametroData2 > 0 && parametroData2 > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosData2 = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroData2),
                    Data = parametroData2
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosData2);
            }

            if (codigoParametroTexto > 0 && !string.IsNullOrWhiteSpace(textoParametroTexto))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosTexto = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroTexto),
                    Texto = textoParametroTexto
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosTexto);
            }
        }

        private void CopiarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaOrigem, ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaDestino)
        {
            ocorrenciaDestino.Carga = ocorrenciaOrigem.Carga;
            ocorrenciaDestino.CFOP = ocorrenciaOrigem.CFOP;
            ocorrenciaDestino.ComponenteFrete = ocorrenciaOrigem.ComponenteFrete;
            ocorrenciaDestino.ContaContabil = ocorrenciaOrigem.ContaContabil;
            ocorrenciaDestino.DataAlteracao = ocorrenciaOrigem.DataAlteracao;
            ocorrenciaDestino.DataFinalizacaoEmissaoOcorrencia = ocorrenciaOrigem.DataFinalizacaoEmissaoOcorrencia;
            ocorrenciaDestino.DataOcorrencia = ocorrenciaOrigem.DataOcorrencia;
            ocorrenciaDestino.GuidNomeArquivo = ocorrenciaOrigem.GuidNomeArquivo;
            ocorrenciaDestino.IncluirICMSFrete = ocorrenciaOrigem.IncluirICMSFrete;
            ocorrenciaDestino.ModeloDocumentoFiscal = ocorrenciaOrigem.ModeloDocumentoFiscal;
            ocorrenciaDestino.MotivoRejeicaoCancelamento = ocorrenciaOrigem.MotivoRejeicaoCancelamento;
            ocorrenciaDestino.NomeArquivo = ocorrenciaOrigem.NomeArquivo;
            ocorrenciaDestino.Observacao = ocorrenciaOrigem.Observacao;
            ocorrenciaDestino.ObservacaoCancelamento = ocorrenciaOrigem.ObservacaoCancelamento;
            ocorrenciaDestino.ObservacaoCTe = ocorrenciaOrigem.ObservacaoCTe;
            ocorrenciaDestino.PercentualAcresciomoValor = ocorrenciaOrigem.PercentualAcresciomoValor;
            ocorrenciaDestino.Responsavel = ocorrenciaOrigem.Responsavel;
            ocorrenciaDestino.SituacaoOcorrencia = ocorrenciaOrigem.SituacaoOcorrencia;
            ocorrenciaDestino.SituacaoOcorrenciaNoCancelamento = ocorrenciaOrigem.SituacaoOcorrenciaNoCancelamento;
            ocorrenciaDestino.SolicitacaoCredito = ocorrenciaOrigem.SolicitacaoCredito;
            ocorrenciaDestino.TipoOcorrencia = ocorrenciaOrigem.TipoOcorrencia;
            ocorrenciaDestino.OrigemOcorrencia = ocorrenciaOrigem.OrigemOcorrencia;
            ocorrenciaDestino.Usuario = ocorrenciaOrigem.Usuario;
            ocorrenciaDestino.ValorOcorrencia = ocorrenciaOrigem.ValorOcorrencia;
            ocorrenciaDestino.ValorOcorrenciaLiquida = ocorrenciaOrigem.ValorOcorrenciaLiquida;
            ocorrenciaDestino.ValorOcorrenciaOriginal = ocorrenciaOrigem.ValorOcorrenciaOriginal;

            ocorrenciaDestino.ErroIntegracaoComGPA = ocorrenciaOrigem.ErroIntegracaoComGPA;
            ocorrenciaDestino.IntegradoComGPA = ocorrenciaOrigem.IntegradoComGPA;
            ocorrenciaDestino.AgImportacaoCTe = ocorrenciaOrigem.AgImportacaoCTe;
        }

        private void SalvarNFeDevolucao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            dynamic dynNFes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NFeDevolucaoAnalise"));

            if (cargaEntrega.NFesDevolucao?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dado in dynNFes)
                    if (dado.Codigo != null)
                        codigos.Add((int)dado.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> listaDeletar = (from obj in cargaEntrega.NFesDevolucao where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < listaDeletar.Count; i++)
                {
                    repControleNFeDevolucao.DeletarPorCargaEntregaNFeDevolucao(listaDeletar[i].Codigo);
                    repCargaEntregaNFeDevolucao.Deletar(listaDeletar[i]);
                }
            }
            else
                cargaEntrega.NFesDevolucao = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();

            foreach (dynamic dados in dynNFes)
            {
                int codigo = ((string)dados.Codigo).ToInt();
                int codigoNFeOrigem = ((string)dados.CodigoNotaOrigem).ToInt();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao nfe = codigo > 0 ? repCargaEntregaNFeDevolucao.BuscarPorCodigo((int)dados.Codigo) : null;

                nfe ??= new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao();

                nfe.ChaveNFe = Utilidades.String.OnlyNumbers((string)dados.Chave);

                if (!string.IsNullOrWhiteSpace(nfe.ChaveNFe) && !Utilidades.Validate.ValidarChaveNFe(nfe.ChaveNFe))
                    throw new ControllerException($"A chave {nfe.ChaveNFe} da NF-e de devolução é inválida.");

                nfe.Numero = ((string)dados.Numero).ToInt();
                nfe.Serie = ((string)dados.Serie).ToInt();
                nfe.DataEmissao = ((string)dados.DataEmissao).ToNullableDateTime();
                nfe.ValorTotalProdutos = ((string)dados.ValorTotalProdutos).ToDecimal();
                nfe.ValorTotal = ((string)dados.ValorTotal).ToDecimal();
                nfe.PesoDevolvido = ((string)dados.PesoDevolvido).ToDecimal();
                if (codigoNFeOrigem > 0)
                    nfe.XMLNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigoNFeOrigem);

                if (!string.IsNullOrWhiteSpace(nfe.ChaveNFe) && (nfe.Numero == 0 || nfe.Serie == 0))
                {
                    nfe.Numero = Utilidades.Chave.ObterNumero(nfe.ChaveNFe);
                    nfe.Serie = Utilidades.Chave.ObterSerie(nfe.ChaveNFe).ToInt();
                }

                nfe.CargaEntrega = cargaEntrega;
                nfe.Chamado = chamado;

                if (nfe.Codigo > 0)
                    repCargaEntregaNFeDevolucao.Atualizar(nfe);
                else
                    repCargaEntregaNFeDevolucao.Inserir(nfe);
            }
        }

        private void GerarControleNotaDevolucao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            if (!ConfiguracaoEmbarcador.HabilitarControleFluxoNFeDevolucaoChamado)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> nfes = repCargaEntregaNFeDevolucao.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> produtos = repCargaEntregaProduto.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo);

            if (nfes.Count == 0 && chamado.CargaEntrega.DevolucaoParcial)
            {
                bool validado = false;
                int countEncontrados = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto in produtos)
                {
                    if (cargaEntregaProduto.QuantidadeDevolucao <= cargaEntregaProduto.Quantidade && cargaEntregaProduto.XMLNotaFiscal != null)
                    {
                        bool encontrouProduto = false;
                        //bool notaFiscalComProdutosDevolvidos = repCargaEntregaProduto.NotaFiscalComProdutosDevolvidos(cargaEntregaProduto.XMLNotaFiscal.Codigo);
                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosXML = repXMLNotaFiscalProduto.BuscarPorNotaFiscal(cargaEntregaProduto.XMLNotaFiscal.Codigo, false);
                        for (int i = 0; i < produtosXML.Count; i++)
                        {
                            if (cargaEntregaProduto.Produto.Codigo == produtosXML[i].Produto.Codigo)
                            {
                                //decimal quantidade = (from obj in produtosXML where obj.Produto.Codigo == cargaEntregaProduto.Produto.Codigo select obj.Quantidade).Sum();
                                encontrouProduto = true;
                                //if (Math.Round(cargaEntregaProduto.QuantidadeDevolucao, 4) != Math.Round(quantidade, 4) && notaFiscalComProdutosDevolvidos)
                                //    throw new ControllerException("Quantidade devolvida difere da quantidade da nota fiscal.");
                                break;
                            }
                        }

                        if (!encontrouProduto)
                            throw new ControllerException($"Produto {cargaEntregaProduto.Produto.Descricao} não consta na nota fiscal.");

                        countEncontrados++;
                    }
                }

                if (countEncontrados > 0)
                    validado = true;

                if (!validado)
                    throw new ControllerException("Quando devolução parcial, deve-se informar pelo menos uma NF-e de devolução.");
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao nfe in nfes)
            {
                if (string.IsNullOrWhiteSpace(nfe.ChaveNFe))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao controleNotaDevolucao = repControleNotaDevolucao.BuscarPorChave(nfe.ChaveNFe);
                if (controleNotaDevolucao == null)
                    controleNotaDevolucao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao();
                else
                    controleNotaDevolucao.Initialize();

                controleNotaDevolucao.ChaveNFe = nfe.ChaveNFe;
                controleNotaDevolucao.CargaEntregaNFeDevolucao = nfe;
                controleNotaDevolucao.Chamado = chamado;
                controleNotaDevolucao.Status = StatusControleNotaDevolucao.AgNotaFiscal;

                if (Utilidades.Validate.ValidarChaveNFe(nfe.ChaveNFe))
                {
                    controleNotaDevolucao.Status = StatusControleNotaDevolucao.ComChaveNotaFiscal;

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(nfe.ChaveNFe);
                    if (xmlNotaFiscal != null)
                    {
                        controleNotaDevolucao.XMLNotaFiscal = xmlNotaFiscal;
                        controleNotaDevolucao.Status = StatusControleNotaDevolucao.ComNotaFiscal;
                    }
                }

                if (controleNotaDevolucao.Codigo > 0)
                    repControleNotaDevolucao.Atualizar(controleNotaDevolucao, Auditado);
                else
                    repControleNotaDevolucao.Inserir(controleNotaDevolucao, Auditado);
            }
        }

        private void SalvarRelatorioAnaliseNosAnexos(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, byte[] pdf, string fileName, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);

            string token = Guid.NewGuid().ToString().Replace("-", "");
            string extensao = ".pdf";
            string caminhoChamado = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), token + extensao);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoChamado, pdf);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo()
                {
                    Chamado = chamado,
                    Descricao = "Análise Atendimento " + chamado.Numero,
                    GuidArquivo = token,
                    NomeArquivo = fileName
                };

                repChamadoAnexo.Inserir(chamadoAnexo);
            }
        }

        private bool PossuiAnaliseParaEfetuarOperacao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
            return servicoChamado.PossuiAnaliseParaEfetuarOperacao(chamado, Usuario.Codigo, unitOfWork);
        }

        private bool ShouldGerarOcorrencia(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = chamado.MotivoChamado;
            TipoMotivoAtendimento tipoAtendimento = motivoChamado.TipoMotivoAtendimento;

            if (!motivoChamado.GerarOcorrenciaAutomaticamente)
                return false;

            if (motivoChamado.TipoOcorrencia == null)
                return false;

            if (motivoChamado.TipoOcorrencia.OrigemOcorrencia != OrigemOcorrencia.PorCarga)
                return false;

            if (chamado.Valor > 0)
                return true;

            if (motivoChamado.TipoOcorrencia.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao ?? false)
                return true;

            bool tipoAtendimentoPermiteGerarOcorrencia = tipoAtendimento == TipoMotivoAtendimento.Reentrega || tipoAtendimento == TipoMotivoAtendimento.Retencao || tipoAtendimento == TipoMotivoAtendimento.RetencaoOrigem;
            bool gerarOcorrenciaReentrega = motivoChamado.GerarCTeComValorIgualCTeAnterior && chamado.TratativaDevolucao == SituacaoEntrega.Reentergue;

            return gerarOcorrenciaReentrega || tipoAtendimentoPermiteGerarOcorrencia || motivoChamado.CalcularOcorrenciaPorTabelaFrete;
        }

        private void ValidarChamadoParaDiariasAutomaticas(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica = repDiariaAutomatica.BuscarPorChamado(chamado.Codigo);

            if (diariaAutomatica == null)
            {
                return;
            }

            if (diariaAutomatica.Status != StatusDiariaAutomatica.Finalizada)
            {
                throw new ControllerException("A Diária Automática não está finalizada");
            }

            if (chamado.MotivoChamado?.BloquearAprovacaoValoresSuperioresADiariaAutomatica == true && chamado.Valor > diariaAutomatica.ValorDiaria)
            {
                throw new ControllerException("Não é possível aprovar um chamado com valor superior à Diária Automática da carga");
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> ConverterDevolucaoNotaFiscalProdutos(dynamic produtos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> cargaEntregaNotaFiscalProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>();

            foreach (dynamic produto in produtos)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto cargaEntregaNotaFiscalProduto = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                {
                    Protocolo = ((string)produto.Codigo).ToInt(),
                    QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal(),
                    Lote = ((string)produto.Lote),
                    DataCritica = ((string)produto.DataCritica).ToNullableDateTime(),
                    ValorDevolucao = ((string)produto.ValorDevolucao).ToDecimal(),
                    NFDevolucao = ((string)produto.NFDevolucao).ToInt(),
                };

                cargaEntregaNotaFiscalProduto.CodigoMotivoDaDevolucao = produto.CodigoMotivoDaDevolucao != null ? ((string)produto.CodigoMotivoDaDevolucao).ToInt() : 0;

                cargaEntregaNotaFiscalProdutos.Add(cargaEntregaNotaFiscalProduto);
            }

            return cargaEntregaNotaFiscalProdutos;
        }

        private void SalvarInformacaoFechamento(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            dynamic dynInfomacoesFechamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("InformacoesFechamento"));

            List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> informacoesFechamento = repositorioChamadoInformacaoFechamento.BuscarPorChamado(chamado.Codigo);

            if (informacoesFechamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dado in dynInfomacoesFechamento)
                    if (dado.Codigo != null)
                        codigos.Add((int)dado.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento> listaDeletar = (from obj in informacoesFechamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < listaDeletar.Count; i++)
                    repositorioChamadoInformacaoFechamento.Deletar(listaDeletar[i]);
            }

            bool possuiInformacoesFechamento = false;
            foreach (dynamic dados in dynInfomacoesFechamento)
            {
                int codigo = ((string)dados.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento informacaoFechamento = codigo > 0 ? informacoesFechamento.Where(o => o.Codigo == codigo).FirstOrDefault() : null;
                if (informacaoFechamento == null)
                {
                    informacaoFechamento = new Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento();
                    informacaoFechamento.Chamado = chamado;
                    informacaoFechamento.QuantidadeDivergencia = ((string)dados.QuantidadeDivergencia).ToInt();
                    informacaoFechamento.XMLNotaFiscal = repositorioXMLNotaFiscal.BuscarPorCodigo(((string)dados.CodigoNotaFiscal).ToInt());
                    informacaoFechamento.MotivoProcesso = repositorioTipoOcorrencia.BuscarPorCodigo(((string)dados.CodigoMotivoProcesso).ToInt());

                    repositorioChamadoInformacaoFechamento.Inserir(informacaoFechamento);
                }

                possuiInformacoesFechamento = true;
            }

            if (!possuiInformacoesFechamento && chamado.InformarDadosChamadoFinalizadoComCusto)
                throw new ControllerException("Necessário informar as informações de fechamento.");
        }

        private bool ValidarValoresCargaDescarga(int codigo, int codigoCarga, int codigoMotivoChamado, double cliente)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                decimal valor = 0;
                decimal valorCargaParam = 0;
                decimal valorDescargaParam = 0;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = codigoMotivoChamado > 0 ? repMotivoChamado.BuscarPorCodigo(codigoMotivoChamado) : null;

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadosAnalise = repChamadoAnalise.BuscarPorChamado(codigo);
                //se ja contem aprovação não precisa buscar nova aprovação.
                if (chamadosAnalise.Where(c => c.LiberadoValorCargaDescarga == true).Any())
                    return false;

                if (carga == null)
                    return false;

                if (!(motivoChamado?.ValidaValorCarga ?? false) && !(motivoChamado?.ValidaValorDescarga ?? false))
                    return false;

                decimal valorCargaJaUtilizado = 0, valorDescargaJaUtilizado = 0;
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosCarga = repChamado.ChamadosMesmaCargaMotivoCliente(codigoCarga, codigoMotivoChamado, cliente, codigo);
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamadosCarga)
                {
                    if (chamado.MotivoChamado?.ValidaValorCarga ?? false)
                        valorCargaJaUtilizado += chamado.Valor;

                    if (chamado.MotivoChamado?.ValidaValorDescarga ?? false)
                        valorDescargaJaUtilizado += chamado.Valor;
                }

                if (motivoChamado?.ValidaValorCarga ?? false)
                {
                    if ((valorCargaJaUtilizado + valor) > valorCargaParam)
                        return false;
                }
                else if (motivoChamado?.ValidaValorDescarga ?? false)
                {
                    if ((valorDescargaJaUtilizado + valor) > valorDescargaParam)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private (decimal valorCarga, decimal valorDescarga) CalcularValoresCargaDescarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            decimal valorCarga = 0;
            decimal valorDescarga = 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                if (cargaPedido.Pedido.PossuiCarga || (cargaPedido.Pedido.ValorCarga ?? 0) > 0)
                    valorCarga += cargaPedido.Pedido.ValorCarga ?? 0;

                if (cargaPedido.Pedido.PossuiDescarga || (cargaPedido.Pedido.ValorDescarga ?? 0) > 0)
                    valorDescarga += cargaPedido.Pedido.ValorDescarga ?? 0;
            }

            return (valorCarga, valorDescarga);
        }

        private void RetornarNotaParaUltimaSituacaoAntesDaAberturaDoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.HistoricoNotaFiscalChamado repositorioHistoricoNotaFiscalChamado = new Repositorio.Embarcador.Chamados.HistoricoNotaFiscalChamado(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado repositorioCargaEntregaNotaFiscalChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado> historicosNotaFiscalChamado = repositorioHistoricoNotaFiscalChamado.BuscarPorChamado(chamado.Codigo);

            foreach (Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado historicoNotaFiscalChamado in historicosNotaFiscalChamado)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repositorioXMLNotaFiscal.BuscarPorCodigo(historicoNotaFiscalChamado.XMLNotaFiscal.Codigo);

                xMLNotaFiscal.SituacaoEntregaNotaFiscal = historicoNotaFiscalChamado.SituacaoNotaFiscal;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, "Alterou a situação da nota " + xMLNotaFiscal.Numero + "para " + xMLNotaFiscal.SituacaoEntregaNotaFiscal.ObterDescricao() + " ao estornar o chamado", unitOfWork);

                repositorioXMLNotaFiscal.Atualizar(xMLNotaFiscal);
            }

        }

        private void SalvarCriticidadeChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(chamado.MotivoChamado?.HabilitarClassificacaoCriticos ?? false))
                return;

            Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repositorioTipoCriticidade = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork);

            string dadosCriticidadeJson = Request.GetStringParam("CriticidadeAtendimento");
            if (string.IsNullOrWhiteSpace(dadosCriticidadeJson))
                return;

            dynamic dadosCriticidadeAtendimento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("CriticidadeAtendimento"));

            chamado.Critico = dadosCriticidadeAtendimento?.critico;
            int codigoGerencial = dadosCriticidadeAtendimento?.codigoGerencial != null ? (int)dadosCriticidadeAtendimento.codigoGerencial : 0;
            chamado.Gerencial = codigoGerencial > 0 ? repositorioTipoCriticidade.BuscarPorCodigoAsync(codigoGerencial)?.Result : null;
            int codigoCausaProblema = dadosCriticidadeAtendimento?.codigoCausaProblema != null ? (int)dadosCriticidadeAtendimento.codigoCausaProblema : 0;
            chamado.CausaProblema = codigoCausaProblema > 0 ? repositorioTipoCriticidade.BuscarPorCodigoAsync(codigoCausaProblema)?.Result : null;
            chamado.FUP = dadosCriticidadeAtendimento?.fup ?? string.Empty;
        }
        #endregion
    }
}
