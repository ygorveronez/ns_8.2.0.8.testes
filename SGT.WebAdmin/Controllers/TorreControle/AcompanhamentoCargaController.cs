using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/AcompanhamentoCarga")]
    public class AcompanhamentoCargaController : BaseController
    {
        #region Construtores

        public AcompanhamentoCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Metodos Publicos

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargasAcompanhamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                SalvarFiltroPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.AcompanhamentoCarga, Request.GetStringParam("FiltroPesquisa"), unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repconfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfigTMS = repconfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga filtrosPesquisa = FiltrosPesquisa(unitOfWork, ConfigTMS);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);
                ConfiguracaoWidgetUsuario configUser = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);

                Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario repositorioConfiguracaoWidget = new Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario configWidget = repositorioConfiguracaoWidget.BuscarConfiguracaoPadrao();

                IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga> listaCargas = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga>();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.CodigosTransportador = new List<int>();
                    filtrosPesquisa.CodigosTransportador.Add(this.Empresa.Codigo);
                }

                int totalRegistros = repositorioCarga.ContarCargasAcompanhamentoCarga(filtrosPesquisa);

                if (totalRegistros == 0)
                    return new JsonpResult(new List<dynamic>(), totalRegistros);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                    InicioRegistros = Request.GetIntParam("inicio"),
                    LimiteRegistros = Request.GetIntParam("limite"),
                    PropriedadeOrdenar = "DataCriacaoCarga"
                };

                filtrosPesquisa.PropriedadeOrdenacaoCargasAcompanhamentoCarga = configWidget?.OpcaoOrdenacaoCardsAcompanhamentoCarga ?? OpcoesOrdenacaoCardsAcompanhamentoCarga.DataCriacaoCarga;

                if (totalRegistros > 0)
                    listaCargas = repositorioCarga.ConsultarCargasAcompanhamentoCarga(filtrosPesquisa, parametrosConsulta);

                //listaCargas = listaCargas.Where(x => x.CodigoCarga == 813).ToList();

                foreach (var carga in listaCargas)
                {
                    var pedidoEmCargas = false;
                    if (carga.Entregas != null && carga.Entregas.Count > 0)
                    {
                        foreach (var entrega in carga.Entregas)
                        {
                            entrega.ImagemAtrasado = entrega.ObterAtrasado(entrega, ConfigTMS);
                            entrega.DestacarFiltro = entrega.IdentificarFiltrosConsultados(entrega, filtrosPesquisa);
                            pedidoEmCargas = entrega.Cliente.Any(obj => obj.PedidoEmMaisCargas > 0);
                        }
                    }

                    carga.Cor = carga.BuscarCor(ConfigTMS);
                    carga.ExibirNivelBateria = carga.NivelBateria > 0 ? true : configUser?.ExibirNivelBateria ?? false;
                    carga.PedidoEmOutrasCargas = pedidoEmCargas;
                    carga.onLine = carga.OnlineStatus(ConfigTMS);
                    carga.PossuiMonitoramentoAtivoProVeiculoEmOutraCarga = (!carga.PossuiMonitoramento || !carga.onLine) && carga.CodigoCargaEspelhadaComMonitoramentoAtivo > 0;
                }

                return new JsonpResult(new
                {
                    Cargas = listaCargas
                }, totalRegistros);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VerificarCargaFiltrosNovoCard()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repconfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfigTMS = repconfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga filtrosPesquisa = FiltrosPesquisa(unitOfWork, ConfigTMS);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarCargasAcompanhamentoCarga(filtrosPesquisa);

                return new JsonpResult(new
                {
                    Carga = true
                }, totalRegistros);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterResumoCargasAcompanhamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repconfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfigTMS = repconfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga filtrosPesquisa = FiltrosPesquisa(unitOfWork, ConfigTMS);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.CodigosTransportador = new List<int>();
                    filtrosPesquisa.CodigosTransportador.Add(this.Empresa.Codigo);
                }

                dynamic obj = repositorioCarga.ContarResumoCargasAcompanhamentoCarga(filtrosPesquisa);

                return new JsonpResult(obj);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterMensagensNaoLidasCards()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Mensagens> mensagens = repositorioCarga.ConsultarMensagensNaoLidasCards(this.Usuario?.Codigo ?? 0);

                return new JsonpResult(new
                {
                    Mensagens = mensagens
                }, mensagens.Count);

            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> FixarCargaPin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var codcarga = Request.GetIntParam("Codigo");
                bool fixar = Request.GetBoolParam("Fixar");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codcarga);
                carga.CargaFixadaControleCargas = fixar;
                repositorioCarga.Atualizar(carga);

                return new JsonpResult(true);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Legendas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<dynamic> listGrupos = new List<dynamic>();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                {
                    Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> gruposTipoOperacao = repGrupoTipoOperacao.BuscarAtivos();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupo in gruposTipoOperacao)
                    {
                        listGrupos.Add(new
                        {
                            grupo.Descricao,
                            grupo.Cor
                        });
                    }
                }
                else
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                    Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem> gruposStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarAtivos();
                    foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem grupo in gruposStatusViagem)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> status = repMonitoramentoStatusViagem.BuscarPorGrupo(grupo.Codigo);
                        listGrupos.Add(new
                        {
                            grupo.Descricao,
                            grupo.Cor
                        });
                    }
                }

                // Categorias das pessoas
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repoCategoria = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> listCategorias = repoCategoria.BuscarTodos();

                return new JsonpResult(new
                {
                    Grupos = listGrupos,
                    Categorias = listCategorias
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarConfiguracaoWidgetAcompanhamentoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario repositorioConfiguracaoWidget = new Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario configWidget = repositorioConfiguracaoWidget.BuscarConfiguracaoPadrao();

                if (configWidget == null)
                    configWidget = new Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario();

                if (this.Usuario != null)
                {
                    Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario configWidgetUser = repositorioConfiguracaoWidget.BuscarConfiguracaoPorUsuario(this.Usuario.Codigo);

                    if (configWidgetUser == null)
                        configWidgetUser = new Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario();

                    configWidget.Usuario = this.Usuario;
                    configWidget.ExibirAlertas = Request.GetBoolParam("ExibirAlertas");
                    configWidget.ExibirDataAgendamentoEntrega = Request.GetBoolParam("ExibirDataAgendamentoEntrega");
                    configWidget.ExibirDataInicioViagem = Request.GetBoolParam("ExibirDataInicioViagem");
                    configWidget.ExibirMotorista = Request.GetBoolParam("ExibirMotorista");
                    configWidget.ExibirPedidosEmMaisCargas = Request.GetBoolParam("ExibirPedidosEmMaisCargas");
                    configWidget.ExibirPrevisaoReprogramada = Request.GetBoolParam("ExibirPrevisaoReprogramada");
                    configWidget.ExibirProximaEntregaPrevisao = Request.GetBoolParam("ExibirProximaEntregaPrevisao");
                    configWidget.ExibirProximoDestino = Request.GetBoolParam("ExibirProximoDestino");
                    configWidget.ExibirTendenciaAtrasoEntrega = Request.GetBoolParam("ExibirTendenciaAtrasoEntrega");
                    configWidget.ExibirTendenciaAtrasoColeta = Request.GetBoolParam("ExibirTendenciaAtrasoColeta");
                    configWidget.ExibirVeiculoTracao = Request.GetBoolParam("ExibirVeiculoTracao");
                    configWidget.ExibirReboques = Request.GetBoolParam("ExibirReboques");
                    configWidget.ExibirNumeroFrotaReboques = Request.GetBoolParam("ExibirNumeroFrotaReboques");
                    configWidget.ExibirDescricaoTipoOperacao = Request.GetBoolParam("ExibirDescricaoTipoOperacao");
                    configWidget.ExibirCidadeProximoDestino = Request.GetBoolParam("ExibirCidadeProximoDestino");
                    configWidget.ExibirValorTotalNFe = Request.GetBoolParam("ExibirValorTotalNFe");
                    configWidget.ExibirPesoTotalNFe = Request.GetBoolParam("ExibirPesoTotalNFe");
                    configWidget.ExibirPesoBrutoNFe = Request.GetBoolParam("ExibirPesoBrutoNFe");
                    configWidget.ExibirAnotacoes = Request.GetBoolParam("ExibirAnotacoes");
                    configWidget.ExibirFilial = Request.GetBoolParam("ExibirFilial");
                    configWidget.DesativarAtualizacaoNovasCargas = Request.GetBoolParam("DesativarAtualizacaoNovasCargas");
                    configWidget.ExibirLeadTimeTransportador = Request.GetBoolParam("ExibirLeadTimeTransportador");
                    configWidget.ExibirDataColeta = Request.GetBoolParam("ExibirDataColeta");
                    configWidget.ExibirDataAgendamentoPedido = Request.GetBoolParam("ExibirDataAgendamentoPedido");
                    configWidget.ExibirCanalVenda = Request.GetBoolParam("ExibirCanalVenda");
                    configWidget.ExibirModalTransporte = Request.GetBoolParam("ExibirModalTransporte");
                    configWidget.ExibirMesorregiao = Request.GetBoolParam("ExibirMesorregiao");
                    configWidget.ExibirRegiao = Request.GetBoolParam("ExibirRegiao");
                    configWidget.BotaoPrimarioDetalheAcompanhamentoCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga>("BotaoPrimario");
                    configWidget.BotaoSecundarioDetalheAcompanhamentoCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.BotoesDetalheAcompanhamentoCarga>("BotaoSecundario");
                    configWidget.OpcaoOrdenacaoCardsAcompanhamentoCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OpcoesOrdenacaoCardsAcompanhamentoCarga>("OpcaoOrdenacaoCardsAcompanhamentoCarga");

                    if (configWidgetUser.Codigo > 0)
                        repositorioConfiguracaoWidget.Atualizar(configWidget);
                    else
                        repositorioConfiguracaoWidget.Inserir(configWidget);

                }

                return new JsonpResult(true);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoWidget()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario repositorioConfiguracaoWidget = new Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario configWidget = repositorioConfiguracaoWidget.BuscarConfiguracaoPadrao();

                if (configWidget == null)
                    configWidget = new Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario();

                if (this.Usuario != null)
                {
                    Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario configWidgetUsuario = repositorioConfiguracaoWidget.BuscarConfiguracaoPorUsuario(this.Usuario.Codigo);

                    if (configWidgetUsuario != null)
                        configWidget = configWidgetUsuario;
                }

                // Valida
                if (configWidget == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    configWidget.ExibirAlertas,
                    configWidget.ExibirDataInicioViagem,
                    configWidget.ExibirMotorista,
                    configWidget.ExibirPedidosEmMaisCargas,
                    configWidget.ExibirPrevisaoReprogramada,
                    configWidget.ExibirProximaEntregaPrevisao,
                    configWidget.ExibirProximoDestino,
                    configWidget.ExibirTendenciaAtrasoEntrega,
                    configWidget.ExibirTendenciaAtrasoColeta,
                    configWidget.ExibirVeiculoTracao,
                    configWidget.ExibirReboques,
                    configWidget.ExibirDescricaoTipoOperacao,
                    configWidget.ExibirNumeroFrotaReboques,
                    configWidget.ExibirCidadeProximoDestino,
                    configWidget.ExibirValorTotalNFe,
                    configWidget.ExibirPesoTotalNFe,
                    configWidget.ExibirPesoBrutoNFe,
                    configWidget.DesativarAtualizacaoNovasCargas,
                    configWidget.ExibirAnotacoes,
                    configWidget.ExibirFilial,
                    configWidget.ExibirLeadTimeTransportador,
                    configWidget.ExibirDataAgendamentoPedido,
                    configWidget.ExibirDataColeta,
                    configWidget.ExibirModalTransporte,
                    configWidget.ExibirCanalVenda,
                    configWidget.ExibirMesorregiao,
                    configWidget.ExibirRegiao,
                    configWidget.BotaoPrimarioDetalheAcompanhamentoCarga,
                    configWidget.BotaoSecundarioDetalheAcompanhamentoCarga,
                    configWidget.OpcaoOrdenacaoCardsAcompanhamentoCarga,
                };

                return new JsonpResult(retorno);

            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao obter configurações do card");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirAnalistaResponsavelMonitoramentoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoAnalista = Request.GetIntParam("CodigoAnalista");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperador = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Usuario analista = repUsuario.BuscarPorCodigo(codigoAnalista);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador = repOperador.BuscarPorUsuario(codigoAnalista);

                if (operador == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                if (!operador.PermitirAssumirCargasControleEntrega && !operador.SupervisorLogistica)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoPossuiPermissaoProsseguir);

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.NenhumaCargaFoiSelecionada);

                var configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                carga.AnalistaResponsavelMonitoramento = analista;
                repCarga.Atualizar(carga);

                if (configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga != "" && analista != null)
                {
                    var motoristas = (from obj in carga.ListaMotorista where obj.CodigoMobile > 0 select obj).ToList();

                    foreach (var motorista in motoristas)
                    {
                        var mensagem = configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga;
                        mensagem = mensagem.Replace("#Motorista#", motorista.Nome);
                        mensagem = mensagem.Replace("#Analista#", analista.Nome);

                        Servicos.Embarcador.Chat.ChatMensagem.EnviarMensagemChat(
                            carga,
                            this.Usuario,
                            DateTime.Now,
                            new List<Usuario> { motorista },
                            mensagem,
                            ClienteAcesso.Cliente.Codigo,
                            unitOfWork,
                            null
                        );
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoDefinirUmAnalista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarAnotacoesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                string anotacoesCarga = Request.GetStringParam("AnotacoesCarga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a carga.");

                unitOfWork.Start();

                carga.AnotacoesCard = anotacoesCarga;
                repCarga.Atualizar(carga, Auditado, null, "Informou anotações na Carga!");

                unitOfWork.CommitChanges();

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
                return new JsonpResult(false, "Ocorreu uma falha ao salvar anotações da carga!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAnotacoesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = Request.GetIntParam("carga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                string anotacoesCarga = string.Empty;
                if (carga != null)
                    anotacoesCarga = carga.AnotacoesCard ?? string.Empty;

                return new JsonpResult(anotacoesCarga);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesCargaEspelhada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga == null) return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.CargaEspelhadaNaoEncontrada);

                DateTime? ETAUltimaEntrega = carga.Entregas.Where(x => !x.Coleta).Max(x => x.DataReprogramada.HasValue ? x.DataReprogramada : x.DataFimPrevista);

                return new JsonpResult(
                    new
                    {
                        CargaEmbarcador = carga.CodigoCargaEmbarcador,
                        PlacaVeiculo = carga.Veiculo.Placa,
                        ETAUltimaEntrega = ETAUltimaEntrega.HasValue ? ETAUltimaEntrega.Value.ToString("dd/MM/yyyy HH:mm:ss") : "Sem previsão definida",
                        UltimaEntregaEspelhadaAtrasada = ETAUltimaEntrega.HasValue ? ETAUltimaEntrega.Value < DateTime.Now : false,
                        DataCarregamentoEspelhada = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : "Sem data de Carregamento",
                    });
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarDetalhesDaCargaEspelhada);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region ALTERAÇÃO DE VEÍCULOS

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarAlteracaoVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                var alteracaoVeiculoPermitida = await ValidarAlteracaoVeiculoAcompanhamentoCarga(codigoCarga, cancellationToken);

                dynamic retorno = new
                {
                    Msg = "É permitida a escolha de novo veículo apenas quando não houver CTe, MDFe ou Integração com GR na carga",
                    AlteracaoVeiculoPermitida = alteracaoVeiculoPermitida
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarVeiculosAlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Veiculo> veiculos = [];

                string codigoCargaParam = Request.GetStringParam("CodigoCarga");

                if (!string.IsNullOrEmpty(codigoCargaParam) &&
                    int.TryParse(codigoCargaParam, out int codigoCarga) &&
                    codigoCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork, cancellationToken);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                    if (carga != null)
                    {
                        var codigoEmpresa = carga.Empresa?.Codigo ?? 0;

                        if (codigoEmpresa > 0)
                        {
                            string placaVeiculo = Request.GetStringParam("PlacaVeiculo");
                            int.TryParse(Request.GetStringParam("ModeloVeiculo"), out int modeloVeiculo);

                            Repositorio.Veiculo repositorioVeiculo = new(unitOfWork, cancellationToken);
                            veiculos = await repositorioVeiculo.BuscarVeiculosPorEmpresaModeloVeicularPlaca(codigoEmpresa, modeloVeiculo, placaVeiculo);
                        }
                    }
                }

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 70, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Placa", "PlacaVeiculo", 30, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(veiculos.Count);

                grid.AdicionaRows((from veiculo in veiculos
                                   select new
                                   {
                                       veiculo?.Codigo,
                                       ModeloVeiculo = veiculo?.ModeloVeicularCarga?.Descricao,
                                       PlacaVeiculo = veiculo?.Placa
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os veículos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotoristasAlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Usuario> motoristas = [];
                string codigoCargaParam = Request.GetStringParam("CodigoCarga");

                if (!string.IsNullOrEmpty(codigoCargaParam) &&
                    int.TryParse(codigoCargaParam, out int codigoCarga) && codigoCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork, cancellationToken);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                    if (carga != null)
                    {
                        var codigoEmpresa = carga.Empresa?.Codigo ?? 0;

                        if (codigoEmpresa > 0)
                        {
                            Repositorio.Usuario repositorioUsuario = new(unitOfWork, cancellationToken);

                            int codigoMotorista = Request.GetIntParam("CodigoMotorista");
                            string cpfMotorista = Request.GetStringParam("CPFMotorista");

                            cpfMotorista = !string.IsNullOrEmpty(cpfMotorista) ? cpfMotorista.ObterSomenteNumeros() : cpfMotorista;

                            if (codigoMotorista > 0 && !string.IsNullOrEmpty(cpfMotorista))
                                motoristas.AddRange(await repositorioUsuario.BuscarMotoristasPorCPFeCodigoAsync(cancellationToken, cpfMotorista, codigoMotorista, codigoEmpresa));

                            else if (codigoMotorista > 0 && string.IsNullOrEmpty(cpfMotorista))
                            {
                                motoristas.AddRange(await repositorioUsuario.BuscarMotoristasPorCodigoAsync(cancellationToken, codigoMotorista, codigoEmpresa));
                            }

                            else if (codigoMotorista <= 0 && !string.IsNullOrEmpty(cpfMotorista))
                                motoristas.AddRange(await repositorioUsuario.BuscarMotoristasPorCPFAsync(cancellationToken, cpfMotorista, codigoEmpresa));
                        }
                    }
                }

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 70, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CPF", "CPF", 30, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(motoristas.Count);
                grid.AdicionaRows((from motorista in motoristas
                                   select new
                                   {
                                       motorista.Codigo,
                                       Motorista = motorista?.Nome,
                                       CPF = motorista?.CPF_CNPJ_Formatado
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Usuario repositorioUsuario = new(unitOfWork, cancellationToken);
                Repositorio.Veiculo repositorioVeiculo = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new(unitOfWork, cancellationToken);

                bool temIntegracao = false;
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                bool alterarMotoristaCarga = Request.GetBoolParam("AlterarMotoristaCarga");
                List<int> veiculosAlterados = Request.GetListParam<int>("CodigosVeiculos");
                List<int> motoristasAlterados = Request.GetListParam<int>("CodigosMotoristas");

                if (veiculosAlterados.Count == 0)
                    throw new ControllerException("Selecione pelo menos um veículo para alteração.");

                List<Dominio.Entidades.Usuario> motoristasSelecionados = [];

                if (alterarMotoristaCarga)
                {
                    if (motoristasAlterados.Count == 0)
                        throw new ControllerException("Selecione pelo menos um motorista para alteração.");
                    else
                    {
                        motoristasSelecionados = await repositorioUsuario.BuscarPorCodigosAsync(motoristasAlterados);

                        if (motoristasSelecionados.Count == 0 || motoristasAlterados.Count != motoristasSelecionados.Count)
                            throw new ControllerException("Motoristas não encontrados para alteração.");
                    }
                }

                var codigoVeiculoAlterado = veiculosAlterados[0];

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Carga não encontrada. Recarregue a página e tente novamente.");

                if (!string.IsNullOrEmpty(carga.ProtocoloIntegracaoGR))
                    throw new ControllerException("Carga já tem integração com Gerenciadora de Risco, alteração de veículo não é permitida.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = await repositorioCargaMDFe.BuscarPorCargaAsync(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repositorioCargaCTe.BuscarPorCargaAsync(carga.Codigo);

                if (cargaCTes.Any(cargaCTe => cargaCTe.CTe != null) || cargaMDFes.Count > 0)
                    throw new ControllerException("Carga já tem documentos emitidos, alteração de motorista não é permitida.");

                Dominio.Entidades.Veiculo veiculoSelecionado = await repositorioVeiculo.BuscarPorCodigoAsync(codigoVeiculoAlterado);

                if (veiculoSelecionado == null)
                    throw new ControllerException("Veículo não encontrado para alteração.");

                carga.Veiculo = veiculoSelecionado;
                carga.ModeloVeicularCarga = veiculoSelecionado.ModeloVeicularCarga;

                List<TipoIntegracao> tipoIntegracoes = [TipoIntegracao.Trizy];

                if (await repositorioCargaCargaIntegracao.ExistePorCargaETipoAsync(carga.Codigo, tipoIntegracoes))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.Trizy);
                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoTrizy = await repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracaoAsync(carga.Codigo, tipoIntegracao.Codigo);

                    if (integracaoTrizy != null)
                    {
                        if (motoristasSelecionados.Count > 0)
                        {
                            integracaoTrizy.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                            integracaoTrizy.ProblemaIntegracao = $"Integração Reenviada pela Troca de Veículo{(alterarMotoristaCarga ? " e motorista " : " ")}manual Acompanhamento de Carga.";
                            await repositorioCargaCargaIntegracao.AtualizarAsync(integracaoTrizy);
                        }
                        else
                        {
                            integracaoTrizy.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                            integracaoTrizy.ProblemaIntegracao = $"Integração Reenviada pela Troca de Veículo manual Acompanhamento de Carga.";
                            await Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AtualizarVeiculoAsync(carga, integracaoTrizy, unitOfWork);
                        }

                        temIntegracao = true;
                    }
                }

                await repositorioCarga.AtualizarAsync(carga);

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigoCarga(carga.Codigo);

                Servicos.Embarcador.Monitoramento.Monitoramento.AtualizarMonitoramento(monitoramento, carga, DateTime.Now, Auditado, "Acompanhamento carga", ConfiguracaoEmbarcador, unitOfWork);

                if (motoristasSelecionados.Count > 0)
                {
                    Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);

                    if (!servicoCargaMotorista.AtualizarMotoristas(carga, motoristasSelecionados, false))
                        throw new ControllerException("Não foi possível atualizar os motoristas da carga.");
                }

                await unitOfWork.CommitChangesAsync();

                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new(unitOfWork);
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(carga);

                dynamic retorno = new
                {
                    Msg = $"Veículos(s){(alterarMotoristaCarga ? " e motorista(s) " : " ")}da carga atualizado(s) com sucesso.",
                    TemIntegracao = temIntegracao
                };
                return new JsonpResult(retorno);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os veículos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracoesAlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Integração", "Integracao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data do Envio", "DataDoEnvio", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 30, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoes = await repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracoesAsync(codigoCarga, new() { TipoIntegracao.Trizy });

                grid.setarQuantidadeTotal(integracoes.Count);
                grid.AdicionaRows((from integracao in integracoes
                                   select new
                                   {
                                       integracao.Codigo,
                                       Integracao = integracao.TipoIntegracao.Descricao,
                                       Tentativas = integracao.NumeroTentativas,
                                       DataDoEnvio = integracao.DataIntegracao.ToDateTimeString(),
                                       Situacao = integracao.DescricaoSituacaoIntegracao,
                                       Retorno = integracao.Protocolo,
                                       Mensagem = integracao.ProblemaIntegracao,
                                       DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha()
                                   }).ToList());
                return new JsonpResult(grid);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaintegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigo, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = cargaCargaintegracao.ArquivosTransacao.OrderByDescending(i => i.Codigo).ToList();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoIntegracao", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 60, Models.Grid.Align.left, false);
                grid.setarQuantidadeTotal(integracoesArquivos.Count);

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   CodigoIntegracao = cargaCargaintegracao.Codigo,
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
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoAlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoIntegracao = Request.GetIntParam("CodigoIntegracao");
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaintegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigoIntegracao, false);

                if (cargaCargaintegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo cargaCargaintegracaoArquivo = cargaCargaintegracao.ArquivosTransacao.FirstOrDefault(o => o.Codigo == codigo);

                if ((cargaCargaintegracaoArquivo == null) || ((cargaCargaintegracaoArquivo.ArquivoRequisicao == null) && (cargaCargaintegracaoArquivo.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { cargaCargaintegracaoArquivo.ArquivoRequisicao, cargaCargaintegracaoArquivo.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos {cargaCargaintegracao.Carga.CodigoCargaEmbarcador}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarIntegracaoAlterarVeiculoAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaintegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigo, false);

                if (cargaCargaintegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                await unitOfWork.StartAsync();

                cargaCargaintegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCargaintegracao, null, "Reenviou Integração (Alteração Veículo Acompanhamento Carga)", unitOfWork);

                await repositorioCargaCargaIntegracao.AtualizarAsync(cargaCargaintegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotoristasAlterarMotoristaAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Usuario repositorioUsuario = new(unitOfWork, cancellationToken);

                string nomeMotorista = Request.GetStringParam("NomeMotorista");
                string cpfMotorista = Request.GetStringParam("CPFMotorista");
                int codigoTransportador = Request.GetIntParam("CodigoTransportador");

                List<Dominio.Entidades.Usuario> motoristas = new();

                if (!string.IsNullOrEmpty(nomeMotorista) && !string.IsNullOrEmpty(cpfMotorista))
                    motoristas.AddRange(await repositorioUsuario.BuscarMotoristasPorCPFeParteDoNomeAsync(cancellationToken, cpfMotorista, nomeMotorista, codigoTransportador));

                else if (!string.IsNullOrEmpty(nomeMotorista) && string.IsNullOrEmpty(cpfMotorista))
                    motoristas.AddRange(await repositorioUsuario.BuscarMotoristasPorParteDoNomeAsync(cancellationToken, nomeMotorista, codigoTransportador));

                else if (string.IsNullOrEmpty(nomeMotorista) && !string.IsNullOrEmpty(cpfMotorista))
                    motoristas.AddRange(await repositorioUsuario.BuscarMotoristasPorCPFAsync(cancellationToken, cpfMotorista, codigoTransportador));

                if (motoristas.Count == 0)
                    throw new ControllerException("Nenhum motorista encontrado.");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 70, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CPF", "CPF", 30, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(motoristas.Count);
                grid.AdicionaRows((from motorista in motoristas
                                   select new
                                   {
                                       motorista.Codigo,
                                       Motorista = motorista.Nome,
                                       CPF = motorista.CPF_CNPJ_Formatado
                                   }).ToList());
                return new JsonpResult(grid);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarMotoristaAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Usuario repositorioUsuario = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new(unitOfWork, cancellationToken);

                bool temIntegracao = false;
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                List<int> motoristasAlterados = Request.GetListParam<int>("CodigosMotoristas");

                if (motoristasAlterados.Count == 0)
                    throw new ControllerException("Selecione pelo menos um motorista para alteração.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);
                if (carga == null)
                    throw new ControllerException("Carga não encontrada. Recarregue a página e tente novamente.");

                if (!string.IsNullOrEmpty(carga.ProtocoloIntegracaoGR))
                    throw new ControllerException("Carga já tem integração com Gerenciadora de Risco, alteração de motorista não é permitida.");

                List<Dominio.Entidades.Usuario> motoristasSelecionados = await repositorioUsuario.BuscarPorCodigosAsync(motoristasAlterados);
                if (motoristasSelecionados.Count == 0 || motoristasAlterados.Count != motoristasSelecionados.Count)
                    throw new ControllerException("Motoristas não encontrados para alteração.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = await repositorioCargaMDFe.BuscarPorCargaAsync(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repositorioCargaCTe.BuscarPorCargaAsync(carga.Codigo);
                if (cargaCTes.Any(cargaCTe => cargaCTe.CTe != null) || cargaMDFes.Count > 0)
                    throw new ControllerException("Carga já tem documentos emitidos, alteração de motorista não é permitida.");

                List<TipoIntegracao> tipoIntegracoes = new() { TipoIntegracao.Trizy };
                if (await repositorioCargaCargaIntegracao.ExistePorCargaETipoAsync(carga.Codigo, tipoIntegracoes))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.Trizy);
                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoTrizy = await repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracaoAsync(carga.Codigo, tipoIntegracao.Codigo);
                    if (integracaoTrizy != null)
                    {
                        integracaoTrizy.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        integracaoTrizy.ProblemaIntegracao = "Integração Reenviada pela Troca de Motorista manual Acompanhamento de Carga.";
                        await repositorioCargaCargaIntegracao.AtualizarAsync(integracaoTrizy);
                        temIntegracao = true;
                    }
                }

                Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
                if (!servicoCargaMotorista.AtualizarMotoristas(carga, motoristasSelecionados, false, auditado: Auditado))
                    throw new ControllerException("Não foi possível atualizar os motoristas da carga.");

                await unitOfWork.CommitChangesAsync();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Alteração de motorista via Acompanhamento de Carga feita com sucesso", unitOfWork);


                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new(unitOfWork);
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(carga);

                dynamic retorno = new
                {
                    Msg = "Motorista(s) da carga atualizado(s) com sucesso.",
                    temIntegracao = temIntegracao
                };
                return new JsonpResult(retorno);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracoesAlterarMotoristaAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Integração", "Integracao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data do Envio", "DataDoEnvio", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 30, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoes = await repositorioCargaCargaIntegracao.BuscarPorCargaETipoIntegracoesAsync(codigoCarga, new() { TipoIntegracao.Trizy });

                grid.setarQuantidadeTotal(integracoes.Count);
                grid.AdicionaRows((from integracao in integracoes
                                   select new
                                   {
                                       integracao.Codigo,
                                       Integracao = integracao.TipoIntegracao.Descricao,
                                       Tentativas = integracao.NumeroTentativas,
                                       DataDoEnvio = integracao.DataIntegracao.ToDateTimeString(),
                                       Situacao = integracao.DescricaoSituacaoIntegracao,
                                       Retorno = integracao.Protocolo,
                                       Mensagem = integracao.ProblemaIntegracao,
                                       DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha()
                                   }).ToList());
                return new JsonpResult(grid);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os motoristas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaintegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigo, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = cargaCargaintegracao.ArquivosTransacao.OrderByDescending(i => i.Codigo).ToList();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoIntegracao", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 60, Models.Grid.Align.left, false);
                grid.setarQuantidadeTotal(integracoesArquivos.Count);

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   CodigoIntegracao = cargaCargaintegracao.Codigo,
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
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoAlterarMotoristaAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoIntegracao = Request.GetIntParam("CodigoIntegracao");
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaintegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigoIntegracao, false);

                if (cargaCargaintegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo cargaCargaintegracaoArquivo = cargaCargaintegracao.ArquivosTransacao.FirstOrDefault(o => o.Codigo == codigo);

                if ((cargaCargaintegracaoArquivo == null) || ((cargaCargaintegracaoArquivo.ArquivoRequisicao == null) && (cargaCargaintegracaoArquivo.ArquivoResposta == null)))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { cargaCargaintegracaoArquivo.ArquivoRequisicao, cargaCargaintegracaoArquivo.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos {cargaCargaintegracao.Carga.CodigoCargaEmbarcador}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarIntegracaoAlterarMotoristaAcompanhamentoCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaintegracao = await repositorioCargaCargaIntegracao.BuscarPorCodigoAsync(codigo, false);

                if (cargaCargaintegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                await unitOfWork.StartAsync();

                cargaCargaintegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCargaintegracao, null, "Reenviou Integração (Alteração Motorista Acompanhamento Carga)", unitOfWork);

                await repositorioCargaCargaIntegracao.AtualizarAsync(cargaCargaintegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion Metodos Publicos

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga FiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfigTMS)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaAcompanhamentoCarga()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroPedidoEmbarcador = Request.GetStringParam("Pedido"),
                CodigosVendedor = Request.GetListParam<int>("Vendedor"),
                DataInicioViagemInicial = Request.GetNullableDateTimeParam("DataInicioViagemInicial"),
                DataInicioViagemFinal = Request.GetNullableDateTimeParam("DataInicioViagemFinal"),
                NumeroNotasFiscais = Request.GetListParam<int>("NumerosNotasFiscais"),
                StatusViagem = Request.GetListEnumParam<StatusViagemControleEntrega>("StatusViagemControleEntrega"),
                DataPrevisaoInicioViagemInicial = Request.GetNullableDateTimeParam("DataPrevisaoInicioViagemInicial"),
                DataPrevisaoInicioViagemFinal = Request.GetNullableDateTimeParam("DataPrevisaoInicioViagemFinal"),
                DataPrevisaoEntregaInicial = Request.GetNullableDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetNullableDateTimeParam("DataPrevisaoEntregaFinal"),
                DataCriacaoCargaInicial = Request.GetNullableDateTimeParam("DataCriacaoCargaInicial"),
                DataCriacaoCargaFinal = Request.GetNullableDateTimeParam("DataCriacaoCargaFinal"),
                DataEntregaInicial = Request.GetNullableDateTimeParam("DataEntregaInicial"),
                DataEntregaFinal = Request.GetNullableDateTimeParam("DataEntregaFinal"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculos"),
                CodigosMotorista = Request.GetListParam<int>("Motorista"),
                LocaisColeta = Request.GetListParam<double>("LocalColeta"),
                LocaisEntrega = Request.GetListParam<double>("LocalEntrega"),
                localidadeColeta = Request.GetListParam<int>("CidadeColeta"),
                localidadeEntrega = Request.GetListParam<int>("CidadeEntrega"),
                ExibirSomenteCargasComVeiculo = Request.GetBoolParam("ExibirSomenteCargasComChamadoAberto"),
                ExibirSomenteCargasComChamadoAberto = Request.GetBoolParam("ExibirSomenteCargasComChamadoAberto"),
                ExibirSomenteCargasComReentrega = Request.GetBoolParam("ExibirSomenteCargasComReentrega"),
                ExibirSomenteCargasUsuarioMonitora = Request.GetBoolParam("ExibirSomenteCargasUsuarioMonitora"),
                ExibirSomenteCargasMotoristaMobile = Request.GetBoolParam("ExibirSomenteCargasMotoristaMobile"),
                ExibirSomenteCargasEmAtraso = Request.GetBoolParam("ExibirSomenteCargasEmAtraso"),
                ExibirSomenteCargasCriticas = Request.GetBoolParam("ExibirSomenteCargasCriticas"),
                ExibirSomenteCargasAlertaMonitoramentoAberto = Request.GetBoolParam("ExibirSomenteCargasAlertaMonitoramentoAberto"),
                ExibirSomenteCargasAlertaMonitoramentoEmTratativa = Request.GetBoolParam("ExibirSomenteCargasAlertaMonitoramentoEmTratativa"),
                ExibirSomenteCargasSemAlertas = Request.GetBoolParam("ExibirSomenteCargasSemAlertas"),
                ExibirSomenteCargasComPesquisaDeDesembarquePendente = Request.GetBoolParam("ExibirSomenteCargasComPesquisaDeDesembarquePendente"),
                PreTrip = Request.GetNullableBoolParam("PreTrip"),
                CodigosTipoDocumentoTransporte = Request.GetListParam<int>("TipoDocumentoTransporte"),
                resumoFinalizadas = Request.GetBoolParam("resumoFinalizadas"),
                resumoNaoIniciada = Request.GetBoolParam("resumoNaoIniciada"),
                resumoEmViagem = Request.GetBoolParam("resumoEmViagem"),
                resumoTodas = Request.GetBoolParam("resumoTodas"),
                CodigosExpedidor = Request.GetListParam<double>("Expedidor"),
                CodigosRecebedor = Request.GetListParam<double>("Recebedor"),
                CodigoNovaCarga = Request.GetIntParam("CodigoNovaCarga"),
                CodigoUsuario = this.Usuario?.Codigo ?? 0,
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                FiltroAtendimento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.TorreControle.tipoFiltroAtendimento>("FiltroAtendimento"),
                FiltroTendenciaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.TorreControle.tipoFiltroTendenciaEntrega>("FiltroTendenciaEntrega"),
                FiltroTendenciaColeta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.TorreControle.tipoFiltroTendenciaEntrega>("FiltroTendenciaColeta"),
                FiltroTendencia = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.TorreControle.tipoFiltroTendenciaEntrega>("FiltroTendencia"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFinal"),
                NumeroOrdem = Request.GetStringParam("NumeroOrdem"),
                PossuiRecebedor = Request.GetNullableBoolParam("PossuiRecebedor"),
                PossuiExpedidor = Request.GetNullableBoolParam("PossuiExpedidor"),
                SituacoesCarga = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                CodigosStatusDaViagem = Request.GetListParam<int>("StatusDaViagem"),
                DataAgendamentoPedidoInicial = Request.GetDateTimeParam("DataAgendamentoPedidoInicial"),
                DataAgendamentoPedidoFinal = Request.GetDateTimeParam("DataAgendamentoPedidoFinal"),
                DataColetaPedidoInicial = Request.GetDateTimeParam("DataColetaPedidoInicial"),
                DataColetaPedidoFinal = Request.GetDateTimeParam("DataColetaPedidoFinal"),
                CodigosClienteComplementar = Request.GetListParam<double>("ClientesComplementar"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                CanalVenda = Request.GetIntParam("CanalVenda"),
                VeiculoNoRaio = Request.GetBoolParam("VeiculoNoRaio"),
                ModalTransporte = Request.GetEnumParam("TipoCobrancaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum),
                MonitoramentoStatus = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
                TipoAlerta = Request.GetListParam<int>("TipoAlerta"),
                EquipeVendas = Request.GetStringParam("EquipeVendas"),
                TipoMercadoria = Request.GetStringParam("TipoMercadoria"),
                EscritorioVenda = Request.GetStringParam("EscritorioVenda"),
                RotaFrete = Request.GetStringParam("RotaFrete"),
                Mesoregiao = Request.GetListParam<int>("Mesoregiao"),
                Regiao = Request.GetListParam<int>("Regiao"),
                GrupoDePessoas = Request.GetListParam<int>("GrupoDePessoas"),
                Matriz = Request.GetStringParam("Matriz"),
                Parqueada = Request.GetNullableBoolParam("Parqueada"),
                CanalEntrega = Request.GetIntParam("CanalEntrega"),
                ExibirSomenteCargasFarolEspelhamentoOnline = Request.GetNullableBoolParam("ExibirSomenteCargasFarolEspelhamentoOnline"),
                ExibirSomenteCargasFarolEspelhamentoOffline = Request.GetNullableBoolParam("ExibirSomenteCargasFarolEspelhamentoOffline"),
                DataInicioAbate = Request.GetDateTimeParam("DataInicioAbate"),
                DataFimAbate = Request.GetDateTimeParam("DataFimAbate")
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoOperacoes = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);
            List<double> codigosDestinatarioPedido = ObterListaCnpjCpfClientePermitidosOperadorLogistica(unitOfWork);
            List<double> codigosRemetentePedido = ObterListaCnpjCpfClientePermitidosOperadorLogistica(unitOfWork);
            List<int> codigosTransportador = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);
            List<int> codigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            List<int> codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

            bool filialOperadorComRegra = false;
            if (codigosFilial.Count == 0)
            {
                codigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
                filialOperadorComRegra = codigosFilial.Any(o => o == -1);
            }

            if (codigosFilial.Count > 0)
                filtrosPesquisa.CodigosFilial = repFilial.BuscarCodigoFilialEmbarcadorPorCodigos(codigosFilial);

            if (filialOperadorComRegra && filtrosPesquisa.CodigosFilial?.Count > 0)
                filtrosPesquisa.CodigosFilial.Add("-1");

            filtrosPesquisa.CpfCnpjDestinatariosPedido = codigosDestinatarioPedido.Count == 0 ? Request.GetListParam<double>("DestinatarioPedido") : codigosDestinatarioPedido;
            filtrosPesquisa.CpfCnpjRemetentesPedido = codigosRemetentePedido.Count == 0 ? Request.GetListParam<double>("RemetentePedido") : codigosRemetentePedido;
            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacoes.Count == 0 ? Request.GetListParam<int>("TipoOperacao") : codigosTipoOperacoes;
            filtrosPesquisa.CodigosTransportador = codigosTransportador.Count == 0 ? Request.GetListParam<int>("Transportador") : codigosTransportador;
            filtrosPesquisa.CodigosFilialVenda = codigosFilialVenda.Count == 0 ? Request.GetListParam<int>("FilialVenda") : codigosFilialVenda;
            filtrosPesquisa.CodigosTipoCarga = codigosTipoCarga.Count == 0 ? Request.GetListParam<int>("TipoCarga") : codigosTipoCarga;
            filtrosPesquisa.CpfCnpjRecebedoresOuSemRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.ExibirEntregaAntesEtapaTransporte = ConfigTMS?.ExibirEntregaAntesEtapaTransporte ?? false;

            return filtrosPesquisa;
        }

        private async Task<bool> ValidarAlteracaoVeiculoAcompanhamentoCarga(int codigoCarga, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            var alteracaoVeiculoPermitida = true;

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Carga não encontrada. Recarregue a página e tente novamente.");

                if (!string.IsNullOrEmpty(carga.ProtocoloIntegracaoGR))
                    alteracaoVeiculoPermitida = false;

                if (alteracaoVeiculoPermitida)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = await repositorioCargaMDFe.BuscarPorCargaAsync(carga.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repositorioCargaCTe.BuscarPorCargaAsync(carga.Codigo);

                    if (cargaCTes.Any(cargaCTe => cargaCTe.CTe != null) || cargaMDFes.Count > 0)
                        alteracaoVeiculoPermitida = false;
                }
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

            return alteracaoVeiculoPermitida;
        }

        #endregion Metodos Privados
    }
}
