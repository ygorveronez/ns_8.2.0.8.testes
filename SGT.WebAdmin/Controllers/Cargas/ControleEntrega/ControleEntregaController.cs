using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Servicos.Embarcador.Notificacao;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "ObterControleEntregaPorcarga", "ObterHistoricoMensagemChatMobile", "ObterControleEntrega", "ObterGuiasTransporteAnimal", "ObterPedidos", "ObterDadosMonitoramento", "ObterComprovanteEntrega" }, "Cargas/ControleEntrega", "Logistica/Monitoramento")]
    public class ControleEntregaController : BaseControleEntregaController
    {
        #region Construtores

        public ControleEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private static readonly string _caminhoImagem = "../../../../img/controle-entrega/";
        private static readonly string imagemForaRaio = _caminhoImagem + "fora-raio.png";
        private static readonly string imagemForaSequencia = _caminhoImagem + "fora-sequencia.png";
        private static readonly string imagemNotaCobertura = "../../../../Content/TorreControle/Icones/gerais/nota-cobertura.svg";
        private static readonly string imagemParcial = _caminhoImagem + "parcial.png";
        private static readonly string imagemSemCoordenada = _caminhoImagem + "sem-coordenada.png";
        private static readonly string imagemPedidoReentrega = "../../../../Content/TorreControle/Icones/alertas/pedido-reentrega.svg";
        private static readonly string imagemAtrasado = "../../../../Content/TorreControle/Icones/alertas/atrasado.svg";
        private static readonly string imagemTendenciaAdiantamento = "../../../../Content/TorreControle/Icones/alertas/tendencia-adiantado.svg";
        private static readonly string imagemTendenciaAtraso = "../../../../Content/TorreControle/Icones/alertas/tendencia-atraso.svg";
        private static readonly string imagemEntregaFiltro = _caminhoImagem + "dentro_filtro.png";
        private static readonly string imagemPedidoEmMaisCargas = "../../../../Content/TorreControle/Icones/alertas/pedidosOutrasCargas.svg";
        private static readonly string imagemEntregaFinalizadaViaFinalizacaoMonitoramento = _caminhoImagem + $"entrega-finalizada-por-monitoramento.png";
        private static readonly string imagemWifiOnline = "../../../../Content/TorreControle/Icones/gerais/wifi-verde.svg";
        private static readonly string imagemWifiOffline = "../../../../Content/TorreControle/Icones/gerais/wifi-vermelho.svg";

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterControleEntregaPorcarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

                ConfiguracaoWidgetUsuario configuracao = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga?.Entregas == null || carga.Entregas.Count == 0)
                    return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.NaoExisteEntregasParaEstaCarga);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> alertas = servicoControleEntrega.ObterAlertasDasCargas(new List<Dominio.Entidades.Embarcador.Cargas.Carga> { carga }, unitOfWork, null);
                List<Dominio.Entidades.Cliente> pessoaFiliais = ObterPessoaFilial(new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga }, unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(carga?.Veiculo?.Codigo ?? 0);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarListaPorCarga(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacaoRetornoControleEntrega> tipoOperacoes = await repCarga.BuscarTipoOperacaoControleEntregaAsync(new List<int> { carga.Codigo });


                Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem mensagem = repChatMobileMensagem.BuscarNaoLidaPorCarga(carga.Codigo, this.Usuario.Codigo);
                bool mensagemNaoLida = mensagem != null;

                List<CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(codigoCarga);
                if (cargaRotaFrete != null)
                    cargasRotaFrete.Add(cargaRotaFrete);

                Repositorio.Embarcador.Configuracoes.IntegracaoDansales repIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales integracaoDansales = repIntegracaoDansales.Buscar();

                Repositorio.Embarcador.Configuracoes.Integracao integracaoRepositorio = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                bool utilizaIntegracaoTrizy = configuracaoEmbarcador.UtilizaAppTrizy && (integracaoRepositorio.Buscar()?.PossuiIntegracaoTrizy ?? false);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                var retorno = new { Entregas = ObterDetalhesControleEntrega(carga, carga.Entregas.ToList(), alertas, configuracaoEmbarcador.RaioPadrao, pessoaFiliais, posicaoAtual, carga.Motoristas.ToList(), mensagemNaoLida, chamados, cargaEntregaPedidos, null, cargasRotaFrete, null, null, configuracaoEmbarcador, configuracao, integracaoDansales, unitOfWork, configuracaoControleEntrega, null, null, tipoOperacoes) };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoMensagemChatMobile()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

                List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> chatMobileMensagens = repChatMobileMensagem.BuscarPorCarga(codigoCarga);

                dynamic mensagens = new List<dynamic>();
                foreach (Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem in chatMobileMensagens)
                {
                    mensagens.Add(Servicos.Embarcador.Chat.ChatMensagem.ObterMensagemMontada(chatMobileMensagem, ClienteAcesso.Cliente.Codigo));
                    if (!chatMobileMensagem.MensagemLida && chatMobileMensagem.Remetente.Codigo != this.Usuario.Codigo)
                        Servicos.Embarcador.Chat.ChatMensagem.NotificarMensagemRecebida(chatMobileMensagem, ClienteAcesso.Cliente.Codigo, unitOfWork);
                }

                repChatMobileMensagem.MarcarTodasComoLidasPorRemetente(codigoCarga, this.Usuario.Codigo, DateTime.Now);
                servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

                return new JsonpResult(mensagens);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarHistoricoDoChat);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarMensagemChat()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMensagemPromotor repChatMensagemPromotor = new Repositorio.Embarcador.Cargas.ChatMensagemPromotor(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repxmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(codigoCarga);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.Chat.PromotorChat> PromotoresNoChat = null;
                List<Dominio.Entidades.Usuario> usuariosDestinatario = new List<Usuario>();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (!string.IsNullOrEmpty(Request.GetStringParam("ListaPromotorEnviar")))
                {

                    PromotoresNoChat = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Carga.Chat.PromotorChat>>(Request.GetStringParam("ListaPromotorEnviar"));
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Chat.PromotorChat promotor in PromotoresNoChat)
                    {
                        Usuario funcionario = repUsuario.BuscarPorCodigo(promotor.CodigoVendedor);
                        if (!repChatMensagemPromotor.ExistePorCargaEPromotor(codigoCarga, promotor.CodigoVendedor))
                        {
                            //registrar o promotor para o chat(carga)
                            Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor chatMensagemPromotor = new Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor();
                            chatMensagemPromotor.Carga = new Dominio.Entidades.Embarcador.Cargas.Carga { Codigo = codigoCarga };
                            chatMensagemPromotor.XMLNotaFiscal = repxmlNotaFiscal.BuscarPorCodigo(promotor.NotaFiscal);
                            chatMensagemPromotor.FuncionarioVendedor = funcionario;

                            repChatMensagemPromotor.Inserir(chatMensagemPromotor);
                        }

                        usuariosDestinatario.Add(funcionario);
                    }
                }


                if (cargaMotoristas.Count > 0 && (configuracaoTMS.UtilizaAppTrizy || cargaMotoristas.Any(obj => obj.Motorista.CodigoMobile > 0)))
                {
                    unitOfWork.Start();

                    usuariosDestinatario.AddRange((from obj in cargaMotoristas where obj.Motorista.CodigoMobile > 0 select obj.Motorista).ToList());

                    Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem = Servicos.Embarcador.Chat.ChatMensagem.EnviarMensagemChat(repositorioCarga.BuscarPorCodigo(codigoCarga), this.Usuario, DateTime.Now, usuariosDestinatario, Request.GetStringParam("Mensagem"), ClienteAcesso.Cliente.Codigo, unitOfWork, null, PromotoresNoChat);
                    unitOfWork.CommitChanges();

                    Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();
                    servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

                    return new JsonpResult(Servicos.Embarcador.Chat.ChatMensagem.ObterMensagemMontada(chatMobileMensagem, ClienteAcesso.Cliente.Codigo));
                }
                else
                {
                    return new JsonpResult(true, false, Localization.Resources.Cargas.ControleEntrega.EssaCargaNaoPossuiUmMotoristaLiberdoParaUsarMultiMobile);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoEnviarMensagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> reenviarMensagemIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoMensagem = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem = new Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem();
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

                chatMobileMensagem = repChatMobileMensagem.BuscarPorCodigo(codigoMensagem);
                if (chatMobileMensagem != null)
                {
                    Servicos.Embarcador.Integracao.Dansales.ChatIntegracaoDansales chatDansales = new Servicos.Embarcador.Integracao.Dansales.ChatIntegracaoDansales(unitOfWork);
                    if (chatDansales.ReenviarMensagemIntegracao(chatMobileMensagem.Carga, chatMobileMensagem.Remetente, chatMobileMensagem))
                    {
                        chatMobileMensagem.MensagemLida = true;
                        chatMobileMensagem.MensagemFalhaIntegracao = false;

                        repChatMobileMensagem.Atualizar(chatMobileMensagem);
                        servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

                        unitOfWork.CommitChanges();

                        return new JsonpResult(new
                        {
                            Codigo = chatMobileMensagem.Codigo
                        });

                    }
                    else
                    {
                        return new JsonpResult(true, true, "Falha no envio da mensagem por integração");
                    }
                }
                else
                {
                    return new JsonpResult(true, false, "Falha no envio da mensagem por integração");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoEnviarMensagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> MarcarComoNaoLido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagens = repChatMobileMensagem.BuscarUltimoPorCarga(codigoCarga, this.Usuario.Codigo);

                if (chatMobileMensagens != null)
                {
                    chatMobileMensagens.MensagemLida = false;
                    repChatMobileMensagem.Atualizar(chatMobileMensagens);

                    servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();
                }
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoMarcarMensagemComoNaoLida);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> MarcarTodasComoNaoLido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> chatMobileMensagens = repChatMobileMensagem.BuscarNaoLidaOuLidaPorCargaERemetente(codigoCarga, this.Usuario.Codigo, true);

                foreach (Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem in chatMobileMensagens)
                {
                    chatMobileMensagem.MensagemLida = false;
                    chatMobileMensagem.DataConfirmacaoLeitura = null;
                    repChatMobileMensagem.Atualizar(chatMobileMensagem);
                }

                servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

                return new JsonpResult(chatMobileMensagens?.Count ?? 0);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoMarcarMensagemComoNaoLida);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> MarcarTodasComoLido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> chatMobileMensagens = repChatMobileMensagem.BuscarNaoLidaOuLidaPorCargaERemetente(codigoCarga, this.Usuario.Codigo, false);

                foreach (Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem in chatMobileMensagens)
                {
                    chatMobileMensagem.MensagemLida = true;
                    chatMobileMensagem.DataConfirmacaoLeitura = DateTime.Now;
                    repChatMobileMensagem.Atualizar(chatMobileMensagem);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao amrcar mensagem como lida.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarMensagensNaoLida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> chatMobileMensagens = repChatMobileMensagem.BuscarNaoLidaOuLidaPorCargaERemetente(codigoCarga, this.Usuario.Codigo, false);

                return new JsonpResult(chatMobileMensagens?.Count ?? 0);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoMarcarMensagemComoNaoLida);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverPromotorChat()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoPromotor = Request.GetIntParam("Promotor");

                Repositorio.Embarcador.Cargas.ChatMensagemPromotor repChatmensagemPromotor = new Repositorio.Embarcador.Cargas.ChatMensagemPromotor(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor chatPromotor = repChatmensagemPromotor.BuscarPorCargaEPromotor(codigoCarga, codigoPromotor);

                if (chatPromotor != null)
                {
                    repChatmensagemPromotor.Deletar(chatPromotor);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoRemoverPromotor);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ObterControleEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao, TipoSessaoBancoDados.Nova);

            try
            {

                SalvarFiltroPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa.ControleEntrega, Request.GetStringParam("FiltroPesquisa"), unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.Fornecedor && this.Usuario != null && this.Usuario.Cliente != null && !this.Usuario.Cliente.CompartilharAcessoEntreGrupoPessoas)
                    filtrosPesquisa.CpfCnpjDestinatarios.Add(Usuario.CPF.ToDouble());

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

                if (configuracaoControleEntrega != null && configuracaoControleEntrega.PermiteExibirCargaCancelada)
                    filtrosPesquisa.PermiteExibirCargaCancelada = true;

                if (configuracaoControleEntrega != null && configuracaoControleEntrega.PermitirBuscarCargasAgrupadasAoPesquisarNumero)
                    filtrosPesquisa.PermitirBuscarCargasAgrupadasAoPesquisarNumero = true;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.CodigosTransportador = new List<int>();
                    if (this.Usuario.Empresa != null)
                        filtrosPesquisa.CodigosTransportador.Add(this.Usuario.Empresa.Codigo);
                    filtrosPesquisa.CodigosTransportador.AddRange(this.Usuario.Empresa != null ? this.Usuario.Empresa.Filiais.Select(o => o.Codigo).ToList() : new List<int>());
                }

                // Obter versão do app na loja
                AdminMultisoftware.Repositorio.Mobile.ConfiguracaoMobile repConfiguracaoMobile = new AdminMultisoftware.Repositorio.Mobile.ConfiguracaoMobile(adminUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Mobile.ConfiguracaoMobile configMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();
                string versaoAppLoja = configMobile.VersaoAppLoja;

                int totalRegistros = repositorioCarga.ContarConsultarCargaEntrega(filtrosPesquisa, this.Usuario, versaoAppLoja: versaoAppLoja);

                if (totalRegistros == 0)
                    return new JsonpResult(new List<dynamic>(), totalRegistros);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                    InicioRegistros = Request.GetIntParam("inicio"),
                    LimiteRegistros = Request.GetIntParam("limite"),
                    PropriedadeOrdenar = "DataCriacaoCarga"
                };

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repositorioChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargaEntrega = repositorioCarga.ConsultarCargasEntrega(filtrosPesquisa, parametrosConsulta, this.Usuario, orderBy: "AnalistaResponsavelMonitoramento", versaoAppLoja: versaoAppLoja);
                List<int> codigosCarga = (from carga in listaCargaEntrega select carga.Codigo).ToList();
                ConfiguracaoWidgetUsuario configuracao = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarPorCargas(codigosCarga);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> alertas = servicoControleEntrega.ObterAlertasDasCargas(listaCargaEntrega, unitOfWork, null);
                List<int> codigosVeiculosCargas = (from carga in listaCargaEntrega where carga.Veiculo != null select carga.Veiculo.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorCargas(codigosCarga);
                List<Dominio.Entidades.Cliente> pessoaFiliais = ObterPessoaFilial(listaCargaEntrega, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargasMotoristas = repositorioCargaMotorista.BuscarPorCargas(codigosCarga);
                List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicaoAtuais = repositorioPosicaoAtual.BuscarPorVeiculos(codigosVeiculosCargas);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem> mensagens = repositorioChatMobileMensagem.BuscarNaoLidaPorCargas(codigosCarga, this.Usuario.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacaoRetornoControleEntrega> tipoOperacoes = await repositorioCarga.BuscarTipoOperacaoControleEntregaAsync(codigosCarga);

                List<CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargas(codigosCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete = repCargaRotaFrete.BuscarPorCargas(codigosCarga);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentos = null;
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisaMonitoramento = null;
                if (ConfiguracaoEmbarcador.PossuiMonitoramento && filtrosPesquisa.RetornarInformacoesMonitoramento)
                {
                    filtrosPesquisaMonitoramento = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento { CodigosCarga = codigosCarga };
                    if (codigosVeiculosCargas.Count > 0)
                        filtrosPesquisaMonitoramento.CodigosVeiculos = codigosVeiculosCargas;

                    Logistica.GridMonitoramento gridMonitoramento = new Logistica.GridMonitoramento();
                    monitoramentos = gridMonitoramento.ObterRegistrosPesquisa(null, this.Usuario.Codigo, filtrosPesquisaMonitoramento, ConfiguracaoEmbarcador, configuracaoMonitoramento, unitOfWork, this.TipoServicoMultisoftware);
                }

                Repositorio.Embarcador.Configuracoes.IntegracaoDansales repIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales integracaoDansales = repIntegracaoDansales.Buscar();
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao integracaoRepositorio = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                bool utilizaIntegracaoTrizy = ConfiguracaoEmbarcador.UtilizaAppTrizy && (integracaoRepositorio.Buscar()?.PossuiIntegracaoTrizy ?? false);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> lstCargaEntrega = repCargaEntrega.BuscarPorCargas(codigosCarga);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> lstCargaEntregaCheckListPergunta = repCargaEntregaCheckListPergunta.BuscarPorCargasEntregas(lstCargaEntrega.Select(x => x.Codigo).ToList());

                List<dynamic> listaCargaEntregaRetornar = (
                    from carga in listaCargaEntrega
                    select ObterDetalhesControleEntrega(
                        carga, cargasEntrega, alertas, ConfiguracaoEmbarcador.RaioPadrao, pessoaFiliais,
                        (from posicaoAtual in posicaoAtuais where posicaoAtual != null && posicaoAtual.Veiculo != null && carga.Veiculo != null && posicaoAtual.Veiculo.Codigo == carga.Veiculo.Codigo select posicaoAtual).FirstOrDefault(),
                        (from cargaMotorista in cargasMotoristas where cargaMotorista.Carga.Codigo == carga.Codigo select cargaMotorista.Motorista).ToList(),
                        (from mensagem in mensagens where mensagem.CodigoCarga == carga.Codigo select mensagem.CodigoCarga).ToList().Count > 0, chamados,
                        cargaEntregaPedidos,
                        monitoramentos,
                        cargasRotaFrete,
                        filtrosPesquisaMonitoramento,
                        filtrosPesquisa,
                        ConfiguracaoEmbarcador, configuracao,
                        integracaoDansales,
                        unitOfWork, configuracaoControleEntrega, lstCargaEntrega, lstCargaEntregaCheckListPergunta, tipoOperacoes, utilizaIntegracaoTrizy
                    )
                ).ToList();

                return new JsonpResult(listaCargaEntregaRetornar, totalRegistros);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarAsEntregas);
            }
            finally
            {
                unitOfWork.Dispose();
                adminUnitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> HabilitarImportacaoCargaFluvial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                bool habilitar = VerificarConfiguracaoHabilitarImportacaoCargaFluvial(unitOfWork);
                return new JsonpResult(new
                {
                    Habilitar = habilitar
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarConfiguracaoDaImportacaoDeCargaFluvial);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoImportacaoCargaFluvial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!VerificarConfiguracaoHabilitarImportacaoCargaFluvial(unitOfWork)) return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.RecursoDesabilitado);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Cargas.ControleEntrega.ChaveDoCteFluvial, Propriedade = "ChaveCTe", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Cargas.ControleEntrega.DataHoraDeInicioDaViagem, Propriedade = "DataHoraInicioViagem", Tamanho = 20 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Cargas.ControleEntrega.NomeDoEmpurrador, Propriedade = "NomeEmpurrador", Tamanho = 100 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Cargas.ControleEntrega.DataHoraDePrevisaoDeEntrega, Propriedade = "DataHoraPrevisaoEntrega", Tamanho = 20 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Cargas.ControleEntrega.DataHoraDeFimDaViagem, Propriedade = "DataHoraFimViagem", Tamanho = 20 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Cargas.ControleEntrega.DataHoraDeInicioDaViagem, Propriedade = "DataHoraInicioViagem", Tamanho = 20 },
            };
                return new JsonpResult(configuracoes.ToList());
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportacaoCargaFluvial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            if (!VerificarConfiguracaoHabilitarImportacaoCargaFluvial(unitOfWork)) return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.RecursoDesabilitado);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                retornoImportacao.Total = linhas.Count;
                retornoImportacao.Importados = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    if (!unitOfWork.IsActiveTransaction()) unitOfWork.Start();

                    // Extração dos dados da linha

                    string chaveCTe = string.Empty;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveCTe = (from obj in linhas[i].Colunas where obj.NomeCampo == "ChaveCTe" select obj).FirstOrDefault();
                    if (colChaveCTe != null) chaveCTe = Utilidades.String.OnlyNumbers((string)colChaveCTe.Valor);

                    string nomeEmpurrador = string.Empty;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNomeEmpurrador = (from obj in linhas[i].Colunas where obj.NomeCampo == "NomeEmpurrador" select obj).FirstOrDefault();
                    if (colNomeEmpurrador != null) nomeEmpurrador = (string)colNomeEmpurrador.Valor;

                    DateTime dataInicioViagem = DateTime.MinValue;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataHoraInicioViagem = (from obj in linhas[i].Colunas where obj.NomeCampo == "DataHoraInicioViagem" select obj).FirstOrDefault();
                    if (colDataHoraInicioViagem != null)
                    {
                        string dataInicioViagemString = (string)colDataHoraInicioViagem.Valor;
                        if (!string.IsNullOrWhiteSpace(dataInicioViagemString)) DateTime.TryParse(dataInicioViagemString, out dataInicioViagem);
                    }

                    DateTime dataPrevisaoEntrega = DateTime.MinValue;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataHoraPrevisaoEntrega = (from obj in linhas[i].Colunas where obj.NomeCampo == "DataHoraPrevisaoEntrega" select obj).FirstOrDefault();
                    if (colDataHoraPrevisaoEntrega != null)
                    {
                        string dataPrevisaoEntregaString = (string)colDataHoraPrevisaoEntrega.Valor;
                        if (!string.IsNullOrWhiteSpace(dataPrevisaoEntregaString)) DateTime.TryParse(dataPrevisaoEntregaString, out dataPrevisaoEntrega);
                    }

                    DateTime dataFimViagem = DateTime.MinValue;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataHoraFimViagem = (from obj in linhas[i].Colunas where obj.NomeCampo == "DataHoraFimViagem" select obj).FirstOrDefault();
                    if (colDataHoraFimViagem != null)
                    {
                        string dataFimViagemString = (string)colDataHoraFimViagem.Valor;
                        if (!string.IsNullOrWhiteSpace(dataFimViagemString)) DateTime.TryParse(dataFimViagemString, out dataFimViagem);
                    }

                    bool sucesso = false;
                    string mensagem = string.Empty;

                    if (string.IsNullOrEmpty(chaveCTe))
                    {
                        sucesso = false;
                        mensagem = Localization.Resources.Cargas.ControleEntrega.RegistroIgnoradoNaImportacaoChaveDoCteObrigatoria;
                    }
                    else
                    {
                        // Consulta a carga a partir do CTe
                        Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repConhecimentoDeTransporteEletronico.BuscarPorChave(chaveCTe);
                        if (cte == null)
                        {
                            sucesso = false;
                            mensagem = string.Format(Localization.Resources.Cargas.ControleEntrega.ChaveDoCteNaoEncontrada, chaveCTe);
                        }
                        else
                        {
                            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarTodosPorCTe(cte.Codigo);
                            int totalCargaCTes = cargaCTes?.Count ?? 0;
                            if (totalCargaCTes == 0)
                            {
                                sucesso = false;
                                mensagem = string.Format(Localization.Resources.Cargas.ControleEntrega.ChaveDoCteEncontradoMasCargaNaoRelacionada, chaveCTe, cte.Codigo);
                            }
                            else
                            {

                                int totalCargasEmpurrador = 0;
                                int totalCargasPrevisoes = 0;
                                int totalCargasIniciadas = 0;
                                int totalCargasFinalizadas = 0;

                                // Percorre as cargas relacionadas ao CTe encontrado. Todas devem devem ser alteradas.
                                for (int j = 0; j < totalCargaCTes; j++)
                                {

                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[j];

                                    if (cargaCTe.Carga.DataInicioViagem == null)
                                    {
                                        totalCargasEmpurrador++;
                                        cargaCTe.Carga.NomeEmpurrador = !string.IsNullOrWhiteSpace(nomeEmpurrador) ? nomeEmpurrador : null;
                                        repCarga.Atualizar(cargaCTe.Carga);
                                    }

                                    // Data e hora do início de viagem
                                    if (dataInicioViagem != DateTime.MinValue && cargaCTe.Carga.DataInicioViagem == null)
                                    {
                                        totalCargasIniciadas++;

                                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(cargaCTe.Carga.Codigo, dataInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado, unitOfWork))
                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.Carga, Localization.Resources.Cargas.ControleEntrega.InicioDeViagemInformadoAoImportarCargasFluviais, unitOfWork);
                                    }

                                    // Data e hora da preivsão da entrega
                                    if (dataPrevisaoEntrega != DateTime.MinValue && cargaCTe.Carga.DataFimViagem == null)
                                    {
                                        // Localiza e altera todas as entregas da carga
                                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(cargaCTe.Carga.Codigo);
                                        int totalCargaEntregas = cargaEntregas?.Count ?? 0;
                                        if (totalCargaEntregas > 0)
                                        {
                                            totalCargasPrevisoes++;
                                            for (int k = 0; k < totalCargaEntregas; k++)
                                            {
                                                if (cargaEntregas[k].DataConfirmacao != null)
                                                {
                                                    cargaEntregas[k].DataPrevista = dataPrevisaoEntrega;
                                                    repCargaEntrega.Atualizar(cargaEntregas[k]);
                                                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntregas[k], repCargaEntrega, unitOfWork, configControleEntrega);
                                                }
                                            }
                                        }
                                    }

                                    // Data e hora do fim da viagem
                                    if (dataFimViagem != DateTime.MinValue && cargaCTe.Carga.DataFimViagem == null)
                                    {
                                        totalCargasFinalizadas++;

                                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(cargaCTe.Carga.Codigo, dataFimViagem, Auditado, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, unitOfWork))
                                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.Carga, Localization.Resources.Cargas.ControleEntrega.FimDeViagemInformadoAoImportarCargasFluviais, unitOfWork);
                                    }
                                }

                                // Considera sucesso se conseguiu definir alguma das colunas
                                sucesso = (totalCargasEmpurrador > 0 || totalCargasPrevisoes > 0 || totalCargasIniciadas > 0 || totalCargasFinalizadas > 0);
                                if (!sucesso)
                                {
                                    mensagem = Localization.Resources.Cargas.ControleEntrega.CargasJaHaviamSidoIniciadasFinalizadasAnteriormente;
                                }
                            }
                        }
                    }

                    retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, mensagemFalha = mensagem, processou = sucesso, contar = sucesso });
                    if (sucesso) retornoImportacao.Importados++;

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(retornoImportacao);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterGuiasTransporteAnimal()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unitOfWork);

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                    {
                        header = new List<Models.Grid.Head>()
                    };

                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CodigoDeBarras, "CodigoBarras", 30, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Numero, "NumeroNotaFiscal", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Serie, "Serie", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.UF, "Estado", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Quantidade, "Quantidade", 15, Models.Grid.Align.right, false);

                    int codigoCargaEntrega = Request.GetIntParam("Codigo");
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                        InicioRegistros = grid.inicio,
                        LimiteRegistros = grid.limite,
                        PropriedadeOrdenar = "Codigo"
                    };

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal> listaGuias = new List<CargaEntregaGuiaTransporteAnimal>();
                    int totalRegistros = repCargaEntregaGuiaTransporteAnimal.ContarConsultarCargaEntrega(codigoCargaEntrega);
                    if (totalRegistros > 0)
                        listaGuias = repCargaEntregaGuiaTransporteAnimal.ConsultarCargasEntrega(codigoCargaEntrega, parametrosConsulta);


                    grid.AdicionaRows((from guia in listaGuias
                                       select new
                                       {
                                           guia.Codigo,
                                           guia.CodigoBarras,
                                           guia.NumeroNotaFiscal,
                                           Estado = guia.Estado?.Descricao ?? string.Empty,
                                           guia.Serie,
                                           guia.Quantidade
                                       }).ToList());
                    grid.setarQuantidadeTotal(totalRegistros);

                    return new JsonpResult(grid);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> ObterPedidos()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    int codigoCargaEntrega = Request.GetIntParam("Codigo");
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntregaPedido> pedidos = repCargaEntrega.ConsultarPedidosPorCargaEntrega(codigoCargaEntrega);

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho("CodigoCargaEntrega", false);
                    grid.AdicionarCabecalho("CodigoCargaEntregaPedido", false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Pedido, "Pedido", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente, "CodigoPedidoCliente", 20, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NumeroOrdem, "NumeroOrdem", 20, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NotasFiscais, "NotasFiscais", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.QuantidadeVolumes, "QuantidadeVolumes", 30, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.ValorNotasFiscais, "ValorNF", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CanalEntrega, "CanalEntrega", 20, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Vendedor, "Vendedor", 25, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.PrevisaoEntrega, "PrevisaoEntrega", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Reentrega, "Reentrega", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Carga, "CodigosCargasPedido", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataAbate, "DataAbate", 10, Models.Grid.Align.left, false);

                    Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "ControleEntrega/ObterPedidos", "grid-controle-entrega-pedidos");
                    grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                    grid.AdicionaRows(pedidos);
                    grid.setarQuantidadeTotal(pedidos.Count);

                    return new JsonpResult(grid);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConsultarOsPedidosDaEntrega);
            }
        }

        public async Task<IActionResult> ObterNotasChatAdicionarPromotor()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    int codigoCarga = Request.GetIntParam("Codigo");

                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaNotaVendedorPedido> pedidos = repCargaEntrega.ConsultarVendedoresNotasPorCarga(codigoCarga);

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho("CodigoVendedor", false);
                    grid.AdicionarCabecalho("CodigoNota", false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Pedido, "Pedido", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NotasFiscais, "NotaFiscal", 40, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.ValorNotasFiscais, "ValorNF", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Promotor, "Vendedor", 20, Models.Grid.Align.left, false);
                    grid.AdicionaRows(pedidos);
                    grid.setarQuantidadeTotal(pedidos.Count);

                    return new JsonpResult(grid);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConsultarOsPedidosDaEntrega);
            }
        }

        public async Task<IActionResult> ConsultaPromotorAdicionadoAoChat()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    int codigoCarga = Request.GetIntParam("Codigo");

                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.ChatMensagemPromotor repChatMensagemPromotor = new Repositorio.Embarcador.Cargas.ChatMensagemPromotor(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor> chatMensagensPromotor = repChatMensagemPromotor.BuscarPorCarga(codigoCarga);

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho("CodigoVendedor", false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Promotor, "Vendedor", 20, Models.Grid.Align.left, false);
                    grid.AdicionaRows((from promotor in chatMensagensPromotor
                                       select new
                                       {
                                           CodigoVendedor = promotor.FuncionarioVendedor.Codigo,
                                           Vendedor = promotor.FuncionarioVendedor.Nome,
                                       }).ToList());
                    grid.setarQuantidadeTotal(chatMensagensPromotor.Count);

                    return new JsonpResult(grid);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConsultarOsPedidosDaEntrega);
            }
        }

        public async Task<IActionResult> ObterListaPromotorAdicionadoAoChat()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.ChatMensagemPromotor repChatMensagemPromotor = new Repositorio.Embarcador.Cargas.ChatMensagemPromotor(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor> chatMensagensPromotor = repChatMensagemPromotor.BuscarPorCarga(codigoCarga);

                dynamic promotorChat = new List<dynamic>();
                foreach (Dominio.Entidades.Embarcador.Cargas.ChatMensagemPromotor chatMensagem in chatMensagensPromotor)
                {
                    promotorChat.Add(new
                    {
                        Vendedor = chatMensagem.FuncionarioVendedor.Nome,
                        CodigoVendedor = chatMensagem.FuncionarioVendedor.Codigo,
                        CodigoNota = chatMensagem.XMLNotaFiscal.Codigo
                    });
                }

                return new JsonpResult(promotorChat);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarHistoricoDoChat);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterChavesNFe()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    int codigoCargaEntrega = Request.GetIntParam("Codigo");
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.ChaveNotaFiscal, "ChaveNfe", 20, Models.Grid.Align.left, false);

                    List<CargaEntregaChaveNfe> listaChaveNfe = repCargaEntregaChaveNfe.BuscarPorCargaEntrega(codigoCargaEntrega);

                    grid.AdicionaRows((from o in listaChaveNfe
                                       select new
                                       {
                                           Codigo = o.Codigo,
                                           ChaveNfe = o.ChaveNfe
                                       }).ToList());

                    grid.setarQuantidadeTotal(listaChaveNfe.Count);

                    return new JsonpResult(grid);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoConsultarOsPedidosDaEntrega);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarChaveNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chaveNfe = Request.GetStringParam("ChaveNFe");

                if (string.IsNullOrEmpty(chaveNfe))
                    throw new ControllerException("Chave NFe não informada");

                int codigoCargaEntrega = Request.GetIntParam("CodigoEntrega");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    throw new ControllerException("Coleta não encontrada");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repositorioXMLnotaFiscal.BuscarPorChave(chaveNfe);

                if (xMLNotaFiscal == null)
                    throw new ControllerException("Nota fiscal não encontrada na base de dados");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);

                //if (!cargaPedidos.Any(obj => obj.Pedido.Destinatario.CPF_CNPJ == xMLNotaFiscal.Destinatario.CPF_CNPJ && obj.Pedido.Remetente.CPF_CNPJ == xMLNotaFiscal.Emitente.CPF_CNPJ))
                //    throw new ControllerException("Nota fiscal não pertence a nenhum Destinatario ou Remetente dos pedidos desta etapa");

                if ((cargaEntrega?.Carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValidarRelevanciaNotasPrechekin ?? false) && cargaPedidos.Any(cargaPedido => cargaPedido.StageRelevanteCusto == null))
                    return new JsonpResult(false, "Não é permitido checkin das notas para essa etapa pois a mesma é irrelevante");

                Servicos.Embarcador.Pedido.NotaFiscal servicoNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                bool msgAlertaObservacao = false;
                bool notaFiscalEmOutraCarga = false;
                string retorno;
                retorno = servicoNotaFiscal.ValidarRegrasNota(xMLNotaFiscal, cargaPedidos.FirstOrDefault(), TipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
                if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                    retorno = "";

                if (!string.IsNullOrWhiteSpace(retorno))
                    throw new ControllerException(retorno);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    servicoNotaFiscal.ValidarTransportadorDivergente(xMLNotaFiscal, cargaPedido, TipoServicoMultisoftware);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                bool permitirEnviarNotasFiscais = (
                    (cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.AgNFe) ||
                    (!configuracao.NaoAceitarNotasNaEtapa1 && (cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.Nova)) ||
                    (
                        cargaEntrega.Carga.CargaEmitidaParcialmente && (
                            (cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.EmTransporte) ||
                            (cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao) ||
                            (cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos)
                        )
                    )
                );

                if (!permitirEnviarNotasFiscais)
                    throw new ControllerException($"Não é possível enviar as notas fiscais para a carga em sua atual situação ({cargaEntrega.Carga.DescricaoSituacaoCarga}).");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repositorioCargaEntregaChavesNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> chaves = repositorioCargaEntregaChavesNfe.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                if (!chaves.Any(o => o.ChaveNfe == chaveNfe))
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe cargaEntregaChaveNfe = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe()
                    {
                        CargaEntrega = cargaEntrega,
                        ChaveNfe = chaveNfe,
                        DataCriacao = DateTime.Now
                    };

                    repositorioCargaEntregaChavesNfe.Inserir(cargaEntregaChaveNfe);
                }

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao adicionar a chave de NF-e");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverChaveNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChavesNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe chaveCargaEntrega = repCargaEntregaChavesNfe.BuscarPorCodigo(codigo);

                if (chaveCargaEntrega == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro");

                repCargaEntregaChavesNfe.Deletar(chaveCargaEntrega);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao remover a chave de NF-e");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> FinalizarEnvioChavesNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChavesNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                int codigoCargaEntrega = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, "Coleta não encontrada");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> chaves = repCargaEntregaChavesNfe.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                servicoControleEntrega.FinalizarEnvioChavesNFeControleEntrega(cargaEntrega, TipoServicoMultisoftware, configuracao, Auditado, true, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao adicionar nova ChaveNFe");
            }
        }

        public async Task<IActionResult> ObterDadosMonitoramento()
        {
            try
            {
                return new JsonpResult(ObterGridMonitoramento());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> ObterComprovanteEntrega()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unitOfWork);
                    Repositorio.Embarcador.Cargas.LoteComprovanteEntrega repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);

                    Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                    {
                        header = new List<Models.Grid.Head>()
                    };

                    grid.AdicionarCabecalho("Codigo", false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Lote, "Lote", 30, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataCriacao, "DataCriacao", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Integradora, "Integradora", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoDescricao", 15, Models.Grid.Align.left, false);

                    int codigoCargaEntrega = Request.GetIntParam("Codigo");

                    List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega> listaLoteComprovanteEntrega = repLoteComprovanteEntrega.BuscarPorCargaEntrega(codigoCargaEntrega);

                    grid.AdicionaRows((from lote in listaLoteComprovanteEntrega
                                       select new
                                       {
                                           Codigo = lote.Codigo,
                                           Lote = lote.Codigo,
                                           DataCriacao = lote.DataCriacao != DateTime.MinValue ? lote.DataCriacao.ToString("dd/MM/yyyy HH:ss") : "",
                                           Integradora = "?????",
                                           SituacaoDescricao = "??????"
                                       }).ToList());

                    grid.setarQuantidadeTotal(listaLoteComprovanteEntrega.Count());

                    return new JsonpResult(grid);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> ObterBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoNotas = Request.GetStringParam("NotasFiscais");

                string[] listaCodigos = codigoNotas.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                List<int> listaCodigoInteiros = new List<int>();

                foreach (string codigo in listaCodigos)
                    listaCodigoInteiros.Add(int.Parse(codigo));

                Repositorio.Embarcador.NotaFiscal.NotaFiscalBoleto repositorioNotaFiscalBoleto = new Repositorio.Embarcador.NotaFiscal.NotaFiscalBoleto(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Parcela, "Parcela", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataVencimento, "DataVencimento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Valor, "Valor", 15, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalBoleto> notasFiscaisBoleto = repositorioNotaFiscalBoleto.BuscarPorNotaFiscal(listaCodigoInteiros);

                var retorno = (from obj in notasFiscaisBoleto
                               select new
                               {
                                   obj.Codigo,
                                   DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Numero,
                                   obj.Parcela,
                                   obj.Valor,
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(notasFiscaisBoleto.Count());

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
        public async Task<IActionResult> ObterGridNFTransferenciaDevolucaoPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaEntrega = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet repositorioGestaoDevolucaoNFeTransferenciaPallet = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Chave", "ChaveNF", 30, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet> notasTransferenciaPallet = repositorioGestaoDevolucaoNFeTransferenciaPallet.BuscarPorCargaEntrega(codigoCargaEntrega);

                var retorno = (from obj in notasTransferenciaPallet
                               select new
                               {
                                   obj.Codigo,
                                   obj.ChaveNF
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(notasTransferenciaPallet.Count);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario repConfiguracaoWidgetUsuario = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoWidgetUsuario(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                ConfiguracaoWidgetUsuario configuracao = repConfiguracaoWidgetUsuario.BuscarPorUsuario(this.Usuario.Codigo);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> alertas = servicoControleEntrega.ObterAlertasDasCargas(new List<Dominio.Entidades.Embarcador.Cargas.Carga> { carga }, unitOfWork, null);
                List<Dominio.Entidades.Cliente> pessoaFiliais = ObterPessoaFilial(new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga }, unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repPosicaoAtual.BuscarPorVeiculo(carga?.Veiculo?.Codigo ?? 0);
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem mensagem = repChatMobileMensagem.BuscarNaoLidaPorCarga(carga.Codigo, this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarListaPorCarga(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacaoRetornoControleEntrega> tipoOperacoes = await repCarga.BuscarTipoOperacaoControleEntregaAsync(new List<int> { carga.Codigo });


                bool mensagemNaoLida = mensagem != null;

                List<CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(codigoCarga);
                if (cargaRotaFrete != null)
                    cargasRotaFrete.Add(cargaRotaFrete);

                Repositorio.Embarcador.Configuracoes.Integracao integracaoRepositorio = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                bool utilizaIntegracaoTrizy = configuracaoEmbarcador.UtilizaAppTrizy && (integracaoRepositorio.Buscar()?.PossuiIntegracaoTrizy ?? false);

                Repositorio.Embarcador.Configuracoes.IntegracaoDansales repIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales integracaoDansales = repIntegracaoDansales.Buscar();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

                dynamic retorno = ObterDetalhesControleEntrega(carga, carga.Entregas.ToList(), alertas, configuracaoEmbarcador.RaioPadrao, pessoaFiliais, posicaoAtual, carga.Motoristas.ToList(), mensagemNaoLida, chamados, cargaEntregaPedidos, monitoramentos: null, cargasRotaFrete, filtrosPesquisaMonitoramento: null, filtrosPesquisa: null, configuracaoEmbarcador, configuracao, integracaoDansales, unitOfWork, configuracaoControleEntrega, null, null, tipoOperacoes);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarEvento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);


                TipoMonitoramentoEvento tipoEvento = Request.GetEnumParam<TipoMonitoramentoEvento>("TipoEvento");
                TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEventoHelper.ObterTipoAlerta(tipoEvento);
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga?.DataInicioViagem == null)
                    return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelAdicionarEventoViagemNaoIniciada);


                if (carga.DataFimViagem != null)
                    return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelAdicionarEventoViagemFinalizada);

                DateTime dataEvento = Request.GetDateTimeParam("DataEvento");
                if (dataEvento <= DateTime.MinValue)
                    dataEvento = DateTime.Now;

                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarAtivo(tipoAlerta);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarAlerta(tipoAlerta, monitoramentoEvento, null, null, dataEvento, "", carga, unitOfWork);

                return new JsonpResult("Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoCarga = Request.GetIntParam("Carga");
                double cpfcnpjRemetente = Request.GetDoubleParam("Remetente");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cpfcnpjRemetente);

                if (carga?.DataInicioViagem == null)
                    return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelAdicionarColetaViagemNaoIniciada);

                if (carga.DataFimViagem != null || !serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.NaoPossivelAdicionarColetaViagemFinalizada);

                if (!(carga.TipoOperacao?.PermiteAdicionarColeta ?? false))
                    return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.NaoPermitidoAdicionarColetasNestaCarga);

                if (remetente == null)
                    return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.LocalDeColetaInformadoInvalido);

                unitOfWork.Start();

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AdicionarNovaColeta(carga, remetente, Auditado, TipoServicoMultisoftware, ConfiguracaoEmbarcador, unitOfWork, configuracaoPedido);

                List<int> motoristasNotificar = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork).GerarNotificacaoMotoristaMobile(carga);

                unitOfWork.CommitChanges();

                if (motoristasNotificar.Count > 0)
                {
                    Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(new { carga.Codigo, CodigoCliente = Cliente.Codigo }, Cliente.Codigo, motoristasNotificar, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.Mobile, Servicos.SignalR.Mobile.GetHub(MobileHubs.CargaAtualizada));
                    Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);

                    // Enviar notificacao para o novo app MTrack
                    NotificacaoMTrack serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
                    serNotificacaoMTrack.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), MobileHubs.CargaAtualizada);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime dataAgendamento = Request.GetDateTimeParam("DataAgendamento");
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPedidosPorCarga(codigoCarga);
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    pedido.Initialize();
                    pedido.DataAgendamento = dataAgendamento;
                    repPedido.Atualizar(pedido, Auditado);
                }

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorPedidos(pedidos.Select(obj => obj.Codigo).ToList());

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarDataAgendamentoPorPedido(cargasEntrega, pedidos, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult("Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAtualizarDataDePagamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataReagendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                DateTime? dataReagendamento = Request.GetNullableDateTimeParam("DataReagendamento");
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (dataReagendamento.HasValue && dataReagendamento.Value > DateTime.MinValue && carga != null)
                {
                    if (dataReagendamento < carga.DataCriacaoCarga)
                        return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.DataRegagendamentoInferiorDataCarga);
                    if (repCargaPedido.ContemDataPrevisaoInferior(codigoCarga, dataReagendamento.Value))
                        return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.DataRegagendamentoInferiorPrevisaoSaida);
                }

                unitOfWork.Start();
                carga.DataReagendamento = dataReagendamento;
                repositorioCarga.Atualizar(carga);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAtualizarDataReagendamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoPedido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.CargaNaoLocalizada);

                if (pedido == null)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.PedidoNaoLocalizado);

                if (pedido.Destinatario == null)
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.PedidoNaoPossuiNenhumDestinatario);

                if (!carga.DataInicioViagem.HasValue && !carga.TipoOperacao.PermitirAdicionarPedidoReentregaAposInicioViagem)
                    throw new ControllerException("A Viagem já foi iniciada e o Tipo de Operação não permite inclusão de Reentregas!");

                pedido.AdicionadaManualmente = true;

                repositorioPedido.Atualizar(pedido);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = Servicos.Embarcador.Carga.CargaPedido.CriarCargaPedido(carga, pedido, null, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador, false, configuracaoGeralCarga);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AdicionarEntrega(cargaPedido, cargaPedido.ClienteEntrega, unitOfWork, ConfiguracaoEmbarcador);

                Servicos.Embarcador.Carga.CargaDadosSumarizados cargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                cargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                // Auditoria
                string notasFiscais = string.Join(", ", (from o in xMLNotaFiscals select o.XMLNotaFiscal.Numero.ToString()));
                string mensagemAuditoria = "";
                if (!string.IsNullOrEmpty(notasFiscais))
                {
                    mensagemAuditoria = string.Format(Localization.Resources.Cargas.ControleEntrega.ReentregaDoPedidoNotasFiscaisFoiAdicionadaCarga, pedido.NumeroPedidoEmbarcador, notasFiscais);
                }
                else
                {
                    mensagemAuditoria = string.Format(Localization.Resources.Cargas.ControleEntrega.ReentregaDoPedidoSemNotasFiscaisFoiAdicionadaCarga, pedido.NumeroPedidoEmbarcador);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, mensagemAuditoria, unitOfWork);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaAdicionada(cargaEntrega, Cliente, unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
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

                List<int> codigosCarga = Request.GetListParam<int>("CodigosCargas");
                int codigoAnalista = Request.GetIntParam("CodigoAnalista");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(codigosCarga);
                Dominio.Entidades.Usuario analista = repUsuario.BuscarPorCodigo(codigoAnalista);

                if (cargas.Count == 0)
                {
                    throw new ControllerException(Localization.Resources.Cargas.ControleEntrega.NenhumaCargaFoiSelecionada);
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    carga.AnalistaResponsavelMonitoramento = analista;
                    repCarga.Atualizar(carga);

                    // Se tem uma mensagem para ser mandada, envia para o motorista
                    if (configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga != "" && analista != null)
                    {
                        List<Usuario> motoristas = (from obj in carga.ListaMotorista where obj.CodigoMobile > 0 select obj).ToList();

                        foreach (Usuario motorista in motoristas)
                        {
                            string mensagem = configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga;
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> RelatorioDemonstrativoEstadias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] arquivo = servicoImpressao.ObterPdfDemonstrativoEstadia(carga);

                return Arquivo(arquivo, "application/pdf", $"Demonstrativo Estadias {carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirMinuta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                byte[] arquivo = servicoImpressao.ObterMinutaTransporte(carga);

                return Arquivo(arquivo, "application/pdf", $"Minuta de Transporte {carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReverterEntregaZerarData()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
                int codigoMotivoRetificacao = Request.GetIntParam("CodigoMotivoRetificacao");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacao = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta motivoRetificacao = repMotivoRetificacao.BuscarPorCodigo(codigoMotivoRetificacao);

                if (!(motivoRetificacao?.ReabrirEntregaZerarDataFim ?? false))
                    return new JsonpResult(false);

                cargaEntrega.Initialize();

                cargaEntrega.DataFim = null;
                cargaEntrega.DataInicio = null;
                cargaEntrega.DataConfirmacao = null;
                cargaEntrega.Situacao = SituacaoEntrega.NaoEntregue;

                repCargaEntrega.Atualizar(cargaEntrega, Auditado);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, $"Retificado a entrega. Devido a isso foi revertida a mesma.", unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmErroAoExecutarAcao);
            }
            finally
            {

                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarNfTransferenciaDevolucaoPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chaveNF = Request.GetStringParam("ChaveNfTransferencia");
                int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
                bool nfeTransferencia = Request.GetBoolParam("NFeTransferencia");

                if (string.IsNullOrEmpty(chaveNF))
                    return new JsonpResult(false, "Necessário incluir a chave da nota.");

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet repositorioGestaoDevolucaoNFeTransferenciaPallet = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, "Não foi possível encontrar a entrega.");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet gestaoDevolucaoNFeTransferenciaPallet = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet()
                {
                    ChaveNF = chaveNF,
                    CargaEntrega = cargaEntrega,
                    NFeTransferencia = nfeTransferencia
                };

                repositorioGestaoDevolucaoNFeTransferenciaPallet.Inserir(gestaoDevolucaoNFeTransferenciaPallet);

                return new JsonpResult(true, "Nota inserida com sucesso.");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmErroAoExecutarAcao);
            }
            finally
            {

                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridMonitoramento()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Carga", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Data, "Data", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Origem, "Origem", 16, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Veiculo, "Placa", 8, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.EmpresaFilial, "Transportador", 18, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Transportador, "Transportador", 18, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Carga, "CargaEmbarcador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Motorista, "Motorista", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Posicao, "Posicao", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Velocidade, "Velocidade", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Ignicao, "Ignicao", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Temperatura, "Temperatura", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.FaixaTemperatura, "ControleDeTemperatura", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.PorcentagemViagem, "PercentualViagem", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "Status", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", false);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);
                grid.AdicionarCabecalho("Veiculo", false);
                grid.AdicionarCabecalho("IDEquipamento", false);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                    InicioRegistros = grid.inicio,
                    LimiteRegistros = grid.limite,
                    PropriedadeOrdenar = "DataCriacaoCarga"
                };

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarConsultarCargaEntrega(filtrosPesquisa, this.Usuario);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargaEntrega = (totalRegistros > 0) ? repositorioCarga.ConsultarCargasEntrega(filtrosPesquisa, parametrosConsulta, this.Usuario) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                List<dynamic> listaCargaEntregaRetornar = (from carga in listaCargaEntrega select ObterDetalhesGridMonitoramento(carga, unitOfWork)).ToList();

                grid.AdicionaRows(listaCargaEntregaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterImagemInicioViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {

            if (carga.DataInicioViagem > DateTime.MinValue)
            {
                if (!carga.InicioDeViagemNoRaio)
                    return "Content/TorreControle/Icones/alertas/play-blue.svg";

                return "Content/TorreControle/Icones/alertas/play-blue.svg";
            }

            return "Content/TorreControle/Icones/alertas/play-off.svg";
        }

        private string ObterImagemFimViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.DataFimViagem > DateTime.MinValue)
                return "Content/TorreControle/Icones/alertas/fim-viagem-black.svg";

            return "Content/TorreControle/Icones/alertas/fim-viagem-pendente.svg";
        }

        private string ObterImagemPreTripIniciado(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {

            if (carga.DataPreViagemInicio > DateTime.MinValue)
            {
                return "Content/TorreControle/Icones/alertas/pre-trip-iniciado.svg";
            }

            return "";
        }


        private string ObterImagemPreTripFinalizado(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.DataPreViagemFim > DateTime.MinValue)
                return "Content/TorreControle/Icones/alertas/pre-trip-finalizado.svg";

            return "";
        }

        private string ObterImagemPreTripNaoIniciado(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if ((carga.Filial != null &&
                (carga.Filial.TipoOperacoesTrizy.Contains(carga.TipoOperacao)) &&
                (carga.Filial.HabilitarPreViagemTrizy)) &&
                (carga.DataPreViagemFim == DateTime.MinValue || carga.DataPreViagemFim == null) &&
                (carga.DataPreViagemInicio == DateTime.MinValue || carga.DataPreViagemInicio == null))
                return "Content/TorreControle/Icones/alertas/pre-trip-nao-iniciado.svg";

            return "";
        }

        private string ObterImagemEntregaColeta(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega situacaoEntrega, bool coleta, bool fronteira, bool parqueamento, bool postoFiscal, bool reentrega, bool noRaio, bool entrouESaiuDoRaioSemEntregar, DateTime? dataLimitePermanenciaRaio, bool menosDaMetadeDoTempoParaResolverChamado, bool motoristaChegou, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, bool monitormanetoFinalizado)
        {
            if (fronteira)
            {
                if (noRaio)
                    return $"Content/TorreControle/Icones/alertas/fronteira-no-raio.svg";

                switch (situacaoEntrega)
                {

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue:
                        return $"Content/TorreControle/Icones/alertas/fronteira-realizada.svg";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado:
                        return $"Content/TorreControle/Icones/alertas/fronteira-nao-realizada.svg";
                    default:
                        return "Content/TorreControle/Icones/alertas/fronteira-pendente.svg";
                }
            }

            if (postoFiscal)
            {
                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    return $"Content/TorreControle/Icones/alertas/posto-fiscal-realizado.svg";

                if (noRaio && (dataLimitePermanenciaRaio.HasValue && DateTime.Now > dataLimitePermanenciaRaio.Value))
                    return $"Content/TorreControle/Icones/alertas/posto-fiscal-veiculo-no-raio-atrasado.svg";

                if (noRaio)
                    return $"Content/TorreControle/Icones/alertas/posto-fiscal-no-raio.svg";

                if (entrouESaiuDoRaioSemEntregar && configuracaoEmbarcador.HabilitarEstadoPassouRaioSemConfirmar)
                    return $"Content/TorreControle/Icones/alertas/posto-fiscal-entrou-e-saiu-sem-entregar.svg";

                return $"Content/TorreControle/Icones/alertas/posto-fiscal-pendente.svg";

            }

            if (parqueamento)
            {
                if (noRaio)
                    return $"Content/TorreControle/Icones/alertas/parqueamento-no-raio.svg";

                switch (situacaoEntrega)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue:
                        return $"Content/TorreControle/Icones/alertas/parqueamento-realizado.svg";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado:
                        return $"Content/TorreControle/Icones/alertas/parqueamento-nao-realizado.svg";
                    default:
                        return "Content/TorreControle/Icones/alertas/parqueamento-pendente.svg";
                }
            }

            if (coleta)
            {
                if (chamado?.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.EmTratativa || chamado?.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Aberto)
                    return _caminhoImagem + $"coleta-atendimento.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    return _caminhoImagem + $"coleta-realizada.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmFinalizacao)
                {
                    if (entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento == SituacaoProcessamentoIntegracao.AguardandoProcessamento)
                        return _caminhoImagem + $"coleta_finalizacao_assincrona.png";
                    else if (entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento == SituacaoProcessamentoIntegracao.ErroProcessamento)
                        return _caminhoImagem + $"coleta_finalizacao_assincrona_erro.png";
                }

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                    return _caminhoImagem + $"coleta-nao-realizada.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida)
                    return _caminhoImagem + $"coleta-revertida.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue)
                    return _caminhoImagem + $"coleta-reentregue.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento)
                    return _caminhoImagem + $"coleta-atendimento.png";

                if (noRaio)
                    return _caminhoImagem + $"coleta-veiculo-no-raio.png";

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && entrega.DataInicio.HasValue && !entrega.DataFim.HasValue)
                    return _caminhoImagem + $"coleta-veiculo-no-raio.png";

                if (reentrega)
                    return _caminhoImagem + $"coleta-reentregue.png";

                if (entrouESaiuDoRaioSemEntregar && configuracaoEmbarcador.HabilitarEstadoPassouRaioSemConfirmar)
                    return _caminhoImagem + $"coleta-entrou-e-saiu-sem-entregar.png";

                if (motoristaChegou)
                    return _caminhoImagem + "coleta-motorista-chegou.png";

                return _caminhoImagem + "coleta-pendente.png";
            }
            else
            {
                if (chamado?.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.EmTratativa || chamado?.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Aberto)
                    return _caminhoImagem + $"entrega-atendimento.png";

                if (entrega.DataReentregaEmMesmaCarga.HasValue && SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(situacaoEntrega))
                    return _caminhoImagem + $"entrega-mesma-carga.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    return _caminhoImagem + $"entrega-realizada.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmFinalizacao)
                {
                    if (entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento == SituacaoProcessamentoIntegracao.AguardandoProcessamento)
                        return _caminhoImagem + $"entrega_finalizacao_assincrona.png";
                    else if (entrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento == SituacaoProcessamentoIntegracao.ErroProcessamento)
                        return _caminhoImagem + $"entrega_finalizacao_assincrona_erro.png";
                }

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado || menosDaMetadeDoTempoParaResolverChamado)
                    return _caminhoImagem + $"entrega-devolvida.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida)
                    return _caminhoImagem + $"entrega-revertida.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue)
                    return _caminhoImagem + $"entrega-reentregue.png";

                if (situacaoEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento)
                    return _caminhoImagem + $"entrega-atendimento.png";

                if (entrega.ProntoParaDescarregar)
                    return _caminhoImagem + $"em-descarregamento.png";

                if (noRaio && (dataLimitePermanenciaRaio.HasValue && DateTime.Now > dataLimitePermanenciaRaio.Value))
                    return _caminhoImagem + $"entrega-veiculo-no-raio-atrasado.png";

                if (noRaio && !monitormanetoFinalizado)
                    return _caminhoImagem + $"entrega-veiculo-no-raio.png";

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && entrega.DataInicio.HasValue && !entrega.DataFim.HasValue)
                    return _caminhoImagem + $"entrega-veiculo-no-raio.png";

                if (reentrega)
                    return _caminhoImagem + $"entrega-reentregue.png";

                if (entrouESaiuDoRaioSemEntregar && configuracaoEmbarcador.HabilitarEstadoPassouRaioSemConfirmar)
                    return _caminhoImagem + $"entrega-entrou-e-saiu-sem-entregar.png";

                if (motoristaChegou)
                    return _caminhoImagem + "entrega-motorista-chegou.png";

                return _caminhoImagem + "entrega-pendente.png";
            }

            return "coleta-pendente.png";
        }

        private string ObterTooltip(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Usuario> motoristas, string placas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasDaCarga)
        {
            string motoristaTelefone = string.Empty;

            foreach (Usuario motorista in motoristas)
                motoristaTelefone += motorista.Nome + (!string.IsNullOrEmpty(motorista.Celular) ? "/ " + motorista.Celular : string.Empty);

            int total = entregasDaCarga.Count();

            if (total == 0)
                return string.Empty;

            int jaRealizadas = entregasDaCarga.Where(obj => obj.Situacao != SituacaoEntrega.EmCliente && obj.Situacao != SituacaoEntrega.AgAtendimento && obj.Situacao != SituacaoEntrega.NaoEntregue).Count();

            int percentual = jaRealizadas * 100 / total;

            decimal? pesoCarga = carga?.DadosSumarizados?.PesoTotal;
            decimal? pesoReentrega = entregasDaCarga.Where(obj => obj.Reentrega).Sum(obj => obj.PesoPedidosReentrega);
            decimal? pesoTotal = pesoCarga + pesoReentrega;

            string placaOuEmpurrador = (!string.IsNullOrWhiteSpace(carga.NomeEmpurrador)) ? $"{Localization.Resources.Cargas.ControleEntrega.Empurrador}: {carga.NomeEmpurrador}<BR>" : $"{Localization.Resources.Cargas.ControleEntrega.Placa}: {placas}<BR>";

            return
                $"{Localization.Resources.Cargas.ControleEntrega.Carga}: {carga.CodigoCargaEmbarcador}<BR>" +
                placaOuEmpurrador +
                $"{Localization.Resources.Cargas.ControleEntrega.Transportador}: {carga?.Empresa?.Descricao ?? string.Empty}<BR>" +
                $"{Localization.Resources.Cargas.ControleEntrega.Motorista}: {motoristaTelefone}<BR>" +
                $"{Localization.Resources.Cargas.ControleEntrega.Peso}: {pesoCarga?.ToString("n3") ?? string.Empty}<BR>" +
                (pesoReentrega > 0 ? $"{Localization.Resources.Cargas.ControleEntrega.PesoReentrega}: {pesoReentrega?.ToString("n3") ?? string.Empty}<BR>" : string.Empty) +
                (pesoReentrega > 0 ? $"{Localization.Resources.Cargas.ControleEntrega.PesoTotal}: {pesoTotal?.ToString("n3") ?? string.Empty}<BR>" : string.Empty) +
                $"{Localization.Resources.Cargas.ControleEntrega.PorcentagemConclusao}: {percentual}% ({jaRealizadas}/{total})" +
                (carga.Rota != null ? $"<BR>{Localization.Resources.Cargas.ControleEntrega.Rota}: {carga.Rota.Descricao}" : "");
        }

        private bool ObterSituacaoEntregaFinalizada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            return entrega.Situacao == SituacaoEntrega.Entregue || entrega.Situacao == SituacaoEntrega.Reentergue || entrega.Situacao == SituacaoEntrega.Rejeitado;
        }

        private bool ValidarEntregaNoRaio(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, int raioPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            if (entrega.Carga.DataInicioViagem == null)
                return true;

            bool situacaoFinalizada = ObterSituacaoEntregaFinalizada(entrega);

            if (!situacaoFinalizada)
                return true;

            return Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ValidarEntregaRaio(entrega.Cliente, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = entrega.LatitudeFinalizada != null ? Convert.ToDouble(entrega.LatitudeFinalizada) : 0, Longitude = entrega.LongitudeFinalizada != null ? Convert.ToDouble(entrega.LongitudeFinalizada) : 0 }, raioPadrao);
        }

        private string ValidarEntregaFinalizadaViaMonitoramento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            if (!entrega?.EntregaFinalizadaViaFinalizacaoMonitoramento ?? true)
                return "";

            return imagemEntregaFinalizadaViaFinalizacaoMonitoramento;
        }

        private string ValidarViagemFinalizadaViaMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga?.ViagemFinalizadaViaFinalizacaoMonitoramento ?? true)
                return "";

            return imagemEntregaFinalizadaViaFinalizacaoMonitoramento;
        }
        private string ValidarViagemIniciadaViaMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga?.ViagemIniciadaViaFinalizacaoMonitoramento ?? true)
                return "";

            return imagemEntregaFinalizadaViaFinalizacaoMonitoramento;
        }

        private bool IsEntregaNoFiltroPesquisa(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtroPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            if (filtroPesquisa == null)
                return false;
            else
            {

                if (filtroPesquisa.NumeroPedido > 0)
                {
                    //entrega.nu
                }

                if (filtroPesquisa.NumeroNotasFiscais != null && filtroPesquisa.NumeroNotasFiscais.Count > 0)
                {

                }

                if (filtroPesquisa.CodigoResponsavelEntrega > 0)
                {

                }

                if (filtroPesquisa.CpfCnpjDestinatarios != null && filtroPesquisa.CpfCnpjDestinatarios.Count > 0)
                {

                }

                if (filtroPesquisa.CpfCnpjEmitentes != null && filtroPesquisa.CpfCnpjEmitentes.Count > 0)
                {

                }

                if (filtroPesquisa.CpfCnpjExpedidores != null && filtroPesquisa.CpfCnpjExpedidores.Count > 0)
                {

                }

                if (filtroPesquisa.CodigosVendedor != null && filtroPesquisa.CodigosVendedor.Count > 0)
                {

                }

                if (filtroPesquisa.CodigosSupervisor != null && filtroPesquisa.CodigosSupervisor.Count > 0)
                {

                }

                if (filtroPesquisa.CodigosGerente != null && filtroPesquisa.CodigosGerente.Count > 0)
                {

                }

                if (filtroPesquisa.Recebedor != null && filtroPesquisa.Recebedor.Count > 0)
                {

                }

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroPedidoCliente))
                {

                }

                if (filtroPesquisa.CanaisEntrega != null && filtroPesquisa.CanaisEntrega.Count > 0)
                {

                }

                return false;

            }
        }

        private string ObterDataRealizada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega)
        {
            string retorno = string.Empty;
            if (entrega?.DataInicio != null)
                retorno += $" {entrega.DataInicio?.ToString("dd/MM/yyyy HH:mm")}";

            if (entrega?.DataFim != null)
                retorno += $" / {entrega.DataFim?.ToString("dd/MM/yyyy HH:mm")}";

            return retorno;

        }

        private static int ObterParametroRaioInicioViagem(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            int raio = 0;
            Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork);

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioDeViagem);

            raio = parametro?.Raio ?? 0;

            if (raio == 0)
                raio = configuracaoEmbarcador.RaioPadrao;

            return raio;
        }

        private bool ValidarFimDeViagemNoRaio(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasDaCarga, int raioPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.DataFimViagem == null)
                return true;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaEntrega = entregasDaCarga.OrderByDescending(obj => obj.DataConfirmacao).Take(1).FirstOrDefault();
            if (ultimaEntrega == null)
                return true;

            return Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ValidarEntregaRaio(ultimaEntrega.Cliente, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = ultimaEntrega.LatitudeFinalizada != null ? Convert.ToDouble(ultimaEntrega.LatitudeFinalizada) : 0, Longitude = ultimaEntrega.LongitudeFinalizada != null ? Convert.ToDouble(ultimaEntrega.LongitudeFinalizada) : 0 }, raioPadrao);
        }

        private string ObterImagemMensagem(bool mensagemNaoLida)
        {
            if (mensagemNaoLida)
                return $"../../../../Content/TorreControle/Icones/gerais/mensagem-nao-lida.svg";
            else
                return $"../../../../Content/TorreControle/Icones/gerais/mensagem.svg";

        }

        private string ObterImagemImprimirMinuta()
        {
            return $"Content/TorreControle/Icones/alertas/imprimir-minuta.svg";
        }

        private dynamic ObterDetalhesControleEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> alertas, int raioPadrao, List<Dominio.Entidades.Cliente> pessoasFiliais, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, List<Dominio.Entidades.Usuario> motoristas, bool mensagemNaoLida, List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados, List<CargaEntregaPedido> cargaEntregaPedidos, IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentos, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisaMonitoramento, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, ConfiguracaoWidgetUsuario configuracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales integracaoDansales, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> lstCargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> lstCargaEntregaCheckListPergunta, List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacaoRetornoControleEntrega> tipoOperacao, bool utilizaIntegracaoTrizy = false)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasDaCarga;
            if (lstCargaEntrega == null)
                entregasDaCarga = carga.Entregas.Where(o => o.Carga.Codigo == carga.Codigo).ToList();
            else
                entregasDaCarga = lstCargaEntrega.Where(o => o.Carga.Codigo == carga.Codigo).ToList();

            bool possuiPesquisaDesembarque = repCargaEntregaCheckListPergunta.PossuiChecklistPorCargasEntrega(entregasDaCarga.Select(o => o.Codigo).ToList(), TipoCheckList.Desembarque, lstCargaEntregaCheckListPergunta);
            string placas = carga?.DadosSumarizados?.Veiculos ?? ""; //carga != null ? ObterPlacas(carga.Veiculo, carga.VeiculosVinculados) : string.Empty;

            Usuario motoristaPrincipal = motoristas?.FirstOrDefault();
            int idEquipamento = motoristaPrincipal?.CodigoMobile ?? 0;
            Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacaoRetornoControleEntrega tipoOperacaoRetorno = tipoOperacao?.Where(t => t.CodigoCarga == carga.Codigo).FirstOrDefault();

            Repositorio.Veiculo veiculo = new Repositorio.Veiculo(unitOfWork);

            var retorno = new
            {
                CargaCancelada = carga != null && (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada),
                Carga = carga?.Codigo ?? 0,
                NumeroCarregamento = servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador),
                DataInicioViagem = carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemPrevista = carga.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemReprogramada = carga.DataInicioViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataPrevisaoInicioViagem = carga.DadosSumarizados?.DataPrevisaoInicioViagem?.ToString("dd/MM/yyyy HH:mm"),
                ImagemInicioViagem = ObterImagemInicioViagem(carga),
                ImagemInicioDeViagemForaRaio = carga.DataInicioViagem.HasValue ? !carga.InicioDeViagemNoRaio ? imagemForaRaio : string.Empty : string.Empty,   //!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ValidarInicioViagemRaio(carga.Codigo, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = carga.LatitudeInicioViagem != null ? Convert.ToDouble(carga.LatitudeInicioViagem) : 0, Longitude = carga.LongitudeInicioViagem != null ? Convert.ToDouble(carga.LongitudeInicioViagem) : 0 }, configuracaoEmbarcador, unitOfWork) ? imagemForaRaio : string.Empty,
                carga.DiferencaInicioViagem,
                DataFimViagem = carga.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemPrevista = carga.DataFimViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemReprogramada = carga.DataFimViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                ImagemFimDeViagem = ObterImagemFimViagem(carga),
                ImagemFimDeViagemForaRaio = !ValidarFimDeViagemNoRaio(carga, entregasDaCarga, raioPadrao, unitOfWork) ? imagemForaRaio : string.Empty,
                ImagemViagemFinalizadaViaFinalizacaoMonitoramento = ValidarViagemFinalizadaViaMonitoramento(carga),
                ImagemViagemIniciadaViaFinalizacaoMonitoramento = ValidarViagemIniciadaViaMonitoramento(carga),
                carga.DiferencaFimViagem,
                //Placas = Placas != string.Empty ? Placas : ("Carga: " + carga?.CodigoCargaEmbarcador ?? string.Empty),
                Placas = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? (carga?.CodigoCargaEmbarcador ?? (carga?.Veiculo?.Placa ?? "")) : carga?.Veiculo?.Placa != null ? carga?.Veiculo?.Placa : ("Carga: " + carga?.CodigoCargaEmbarcador ?? string.Empty),
                ImagemMensagem = ObterImagemMensagem(mensagemNaoLida),
                Datacarga = carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Remetente = carga?.DadosSumarizados?.CodigoIntegracaoRemetentes ?? string.Empty,
                Transportador = carga != null ? carga.Empresa?.NomeFantasia : string.Empty,
                TipoOperacao = tipoOperacaoRetorno?.Descricao ?? string.Empty,
                Destinatario = carga?.DadosSumarizados?.CodigoIntegracaoDestinatarios, //carga != null ? ConcatenarDestinatarios(cargasPedidoDaCarga.ToList()) : string.Empty,
                CodigoVeiculo = carga.Veiculo?.Codigo ?? 0,
                IDEquipamento = idEquipamento,
                Quantidade = carga?.DadosSumarizados?.PesoTotal ?? 0,
                PermiteAdicionarEntrega = entregasDaCarga.Any(o => o.PossuiNotaCobertura) && !carga.DataFimViagem.HasValue,
                PermiteAdicionarPromotor = integracaoDansales != null && !string.IsNullOrWhiteSpace(integracaoDansales.URLIntegracao),
                PermiteReordenarEntrega = configuracaoControleEntrega?.PermitirReordenarEntregasAoAddPedido ?? false,
                Tooltip = ObterTooltip(carga, motoristas, placas, entregasDaCarga),
                PermiteAdicionarColeta = tipoOperacaoRetorno?.PermiteAdicionarColeta ?? false,
                PermiteDownloadBoletimViagem = tipoOperacaoRetorno?.EnviarBoletimDeViagemAoFinalizarViagem ?? false,
                PermiteAdicionarReentrega = !carga.DataInicioViagem.HasValue || (!carga.DataFimViagem.HasValue && (tipoOperacaoRetorno?.PermiteAdicionarPedidoReentregaAposInicioViagem ?? false)),
                NomeMotorista = motoristaPrincipal?.Nome ?? string.Empty,
                NumeroMotorista = ObterNumeroCelularCompleto(motoristaPrincipal),

                InformacoesComplementares = ObterInformacoesComplementares(carga, configuracao, motoristaPrincipal, posicaoAtual, entregasDaCarga, cargaEntregaPedidos, cargasRotaFrete, unitOfWork),

                Entregas = (from entrega in entregasDaCarga
                            select ObterDetalhesEntregaControleEntrega(entrega, carga, alertas, raioPadrao, posicaoAtual, chamados, filtrosPesquisa, configuracaoEmbarcador, possuiPesquisaDesembarque, configuracaoControleEntrega, unitOfWork, monitoramento)
                            ).OrderBy(o => o.Ordem).ToList(),
                AlertasSemEntregaVinculada = alertas != null ? (from alerta in alertas where alerta.Carga == carga.Codigo && alerta.CodigoEntrega == 0 && alerta.ExibirNoControleEntrega select alerta) : null,
                TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega?.TornarFinalizacaoDeEntregasAssincrona ?? false,
                GridMonitoramento = ObterGridMonitoramentoCarga(filtrosPesquisaMonitoramento, monitoramentos, this.Usuario.Codigo, carga.Codigo, unitOfWork),
                AnalistaResponsavelMonitoramento = carga.AnalistaResponsavelMonitoramento?.Nome ?? null,
                DataReagendamento = carga.DataReagendamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                ImagemImprimirMinuta = possuiPesquisaDesembarque ? ObterImagemImprimirMinuta() : string.Empty,
                ImagemPreTripIniciado = ObterImagemPreTripIniciado(carga),
                ImagemPreTripFinalizado = ObterImagemPreTripFinalizado(carga),
                ImagemPreTripNaoIniciado = ObterImagemPreTripNaoIniciado(carga),
                DataPreViagemInicio = carga.DataPreViagemInicio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataPreViagemFim = carga.DataPreViagemFim?.ToString("dd/MM/yyyy HH:mm") ?? "",
                CargaCritica = monitoramento?.Critico ?? false,
                CodigoMonitoramento = monitoramento?.Codigo ?? 0,
                OnlineOffline = RetornarStatusMonitoramento(monitoramento, configuracaoEmbarcador),
                UtilizaAppTrizy = utilizaIntegracaoTrizy ? "true" : "false",
                TipoOperacaoPermiteChat = tipoOperacaoRetorno?.PermiteChat ?? false
            };

            return retorno;
        }

        private int RetornarStatusMonitoramento(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
         => monitoramento?.UltimaPosicao?.DataVeiculo is null ? 1 : monitoramento?.UltimaPosicao?.DataVeiculo != null && (DateTime.Now - monitoramento.UltimaPosicao.DataVeiculo).TotalMinutes <= configuracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal ? 3 : 4;

        private dynamic ObterDetalhesEntregaControleEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> alertas, int raioPadrao, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, bool possuiPesquisaDesembarque, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            Servicos.Embarcador.Chamado.ConfiguracaoTempoChamado servicoConfiguracaoTempoChamado = new Servicos.Embarcador.Chamado.ConfiguracaoTempoChamado(unitOfWork);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = ObterChamado(entrega, chamados);
            Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado configuracaoTempoChamado = chamado != null ? servicoConfiguracaoTempoChamado.ObterConfiguracaoPorCriterios(entrega.Cliente, carga.TipoOperacao, carga.Filial) : null;
            bool menosDaMetadeDoTempoParaResolverChamado = (configuracaoTempoChamado?.TempoAtendimento ?? 0) > 0 ? chamado.DataCriacao.AddMinutes(configuracaoTempoChamado.TempoAtendimento / 2) <= DateTime.Now : false;
            bool monitoramentoFinalizado = monitoramento == null || monitoramento.Status == MonitoramentoStatus.Finalizado;
            //VerificarPedidoReentregaNaCargaEntrega(entrega, unitOfWork); //Não pode ser feito neste lugar, pois executa para cada entrega de cada carga que exibe na tela de controle de entrega. Se tiver algum retorno/tarefa referente a isso trazer para reunião diaria

            var retorno = new
            {
                Pessoa = entrega.Cliente?.Descricao ?? string.Empty,
                entrega.Codigo,
                DataPrevista = entrega.DataPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataReprogramada = entrega.DataReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataAgendamento = entrega.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataRealizada = ObterDataRealizada(entrega),
                DataEntrega = entrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegada = entrega.DataInicio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataConfirmacao = entrega.DataConfirmacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                NumeroCTe = TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && entrega.NotasFiscais != null && entrega.NotasFiscais.Count > 0 ? RetornarNumerosCTes(entrega.NotasFiscais.Where(p => p.PedidoXMLNotaFiscal != null)?.Select(p => p.PedidoXMLNotaFiscal.Codigo).ToList() ?? null, unitOfWork) : "",
                CodigoCliente = entrega.Cliente?.Codigo,
                Cliente = entrega.Cliente?.Nome,
                Descricao = entrega.Cliente?.CPF_CNPJ,
                EntergaNaJanela = entrega.StatusPrazoEntrega,
                ChamadoEmAberto = entrega.ChamadoEmAberto || chamado?.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.EmTratativa || chamado?.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Aberto,
                ResponsavelChamado = chamado?.Responsavel?.Nome ?? string.Empty,
                MenosDaMetadeDoTempoParaResolverChamado = menosDaMetadeDoTempoParaResolverChamado,
                MotoristaChegou = entrega.MotoristaACaminho,
                PrevisaoEntergaNaJanela = string.Empty,//Verificar
                entrega.Situacao,
                entrega.DiferencaEntrega,
                entrega.Ordem,
                Imagem = ObterImagemEntregaColeta(entrega, entrega.Situacao,
                                                                 entrega.Coleta,
                                                                 entrega.Fronteira,
                                                                 entrega.Parqueamento,
                                                                 entrega.PostoFiscal,
                                                                 entrega.Reentrega,
                                                                 carga.DataInicioViagem.HasValue && Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(entrega.Cliente, posicaoAtual?.Latitude ?? 0, posicaoAtual?.Longitude ?? 0, raioPadrao),
                                                                 carga.DataInicioViagem.HasValue && Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEntrouESaiuDoRaioSemEntregar(entrega, configuracaoEmbarcador),
                                                                 entrega.DataLimitePermanenciaRaio,
                                                                 menosDaMetadeDoTempoParaResolverChamado,
                                                                 entrega.MotoristaACaminho,
                                                                 configuracaoEmbarcador,
                                                                 chamado,
                                                                 monitoramentoFinalizado
                                                                 ),
                ImagemForaSequencia = ObterSituacaoEntregaFinalizada(entrega) && !entrega.EntregaEmOrdem ? imagemForaSequencia : string.Empty,
                ImagemForaRaio = ValidarEntregaNoRaio(entrega, raioPadrao, unitOfWork) == false ? imagemForaRaio : string.Empty,
                ImagemParcial = entrega.DevolucaoParcial ? imagemParcial : string.Empty,
                ImagemSemCoordenada = string.IsNullOrWhiteSpace(entrega.Cliente?.Latitude ?? "") && string.IsNullOrWhiteSpace(entrega.Localidade?.Latitude.ToString() ?? "") ? imagemSemCoordenada : string.Empty,
                IndicadorComplementar = ObterIndicadorComplementar(entrega, chamado, monitoramentoFinalizado, configuracaoControleEntrega),
                ChamadoEntregaMesmaCarga = entrega.DataReentregaEmMesmaCarga.HasValue && SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(entrega.Situacao),
                entrega.Coleta,
                Alertas = alertas != null ? (from alerta in alertas where alerta.Carga == carga.Codigo && alerta.CodigoEntrega == entrega.Codigo && alerta.ExibirNoControleEntrega select alerta) : null,
                DestacarFiltrosConsultados = IdentificarFiltrosConsultados(entrega, filtrosPesquisa),
                PossuiPesquisaDesembarque = possuiPesquisaDesembarque,
                ExigirInformarNumeroPacotesNaColetaTrizy = entrega.Coleta && (carga.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirInformarNumeroPacotesNaColetaTrizy ?? false),
                QuantidadePacotesColetados = entrega.QuantidadePacotesColetados,
                ExibirDataPrevisaoEntregaTransportador = entrega.DataPrevisaoEntregaTransportador.HasValue,
                DataPrevisaoEntregaTransportador = entrega.DataPrevisaoEntregaTransportador?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                SituacaoProcessamento = entrega.CargaEntregaFinalizacaoAssincrona?.SituacaoProcessamento ?? null,
                Tendencia = entrega.Tendencia.ObterDescricao(),
                DescricaoEntregaNoPrazo = entrega.DescricaoEntregaNoPrazo,

            };

            return retorno;
        }

        private string ObterIndicadorComplementar(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, bool monitoramentoFinalizado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            if (chamado != null && !string.IsNullOrEmpty(chamado.Responsavel?.Nome))
            {
                return string.Empty;
            }

            if (!entrega.Coleta && entrega.DataEntradaRaio.HasValue && entrega.DataEntradaRaio.Value != DateTime.MinValue && entrega.DataEntradaRaio.Value < DateTime.Now && SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(entrega.Situacao) && ConfiguracaoEmbarcador.HabilitarIconeEntregaAtrasada && !monitoramentoFinalizado)
            {
                return imagemAtrasado;
            }

            string indicadorMonitoramento = ValidarEntregaFinalizadaViaMonitoramento(entrega);
            if (!string.IsNullOrEmpty(indicadorMonitoramento))
            {
                return indicadorMonitoramento;
            }

            if (entrega.Reentrega || entrega.PossuiPedidosReentrega)
            {
                return imagemPedidoReentrega;
            }

            //Não pode ser feito neste lugar e desta forma, cargas com muitos pedidos ferra tudo.Se tiver rejeições novas trazer para conversa com equipe
            //if (ObterImagemMaisDeUmaCarga(entrega, unitOfWork) ? imagemPedidoEmMaisCargas)
            //{
            //    return imagemPedidoEmMaisCargas;
            //}

            if (!ObterSituacaoEntregaFinalizada(entrega) && entrega.PossuiNotaCobertura && configuracaoControleEntrega.PossuiNotaCobertura)
            {
                return imagemNotaCobertura;
            }

            return string.Empty;
        }

        private string ObterNumeroCelularCompleto(Usuario motorista)
        {
            if (motorista == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(motorista.Celular))
                return string.Empty;

            string celular = Utilidades.String.OnlyNumbers(motorista.Celular ?? string.Empty);

            bool numeroBrasileiro = motorista.Localidade == null || motorista.Localidade.Pais?.Abreviacao == "BR";

            return $"{(numeroBrasileiro ? "+55" : "")}{celular}";
        }

        private Dominio.Entidades.Embarcador.Chamados.Chamado ObterChamado(CargaEntrega entrega, List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados)
        {
            return (from obj in chamados where obj.CargaEntrega != null && obj.CargaEntrega.Codigo == entrega.Codigo select obj).FirstOrDefault();
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculo != null)
            {
                List<string> placas = new List<string>() { veiculo.Placa };
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

                return string.Join(", ", placas);
            }
            else
                return "";

        }

        private dynamic ObterInformacoesComplementares(Dominio.Entidades.Embarcador.Cargas.Carga carga, ConfiguracaoWidgetUsuario configuracao, Usuario motorista, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, List<CargaEntrega> entregasDaCarga, List<CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            List<(string Descricao, string Valor, bool Icone, string Tooltip)> informacoes = new List<(string Descricao, string Valor, bool Icone, string Tooltip)>();

            if (configuracao?.ExibirNomeMotorista ?? false)
            {
                string nomeMotorista = motorista?.Nome ?? "";
                string primeiroNome = nomeMotorista.Split(' ').FirstOrDefault();
                string nomeMotorita = primeiroNome ?? "-";
                informacoes.Add(ValueTuple.Create("nome-motorista", nomeMotorita, false, nomeMotorista));
            }

            if (configuracao?.ExibirVersaoAplicativo ?? false)
            {
                string versao = motorista?.VersaoAPP ?? "-";
                informacoes.Add(ValueTuple.Create("versao-aplicativo", versao, false, ""));
            }

            if (configuracao?.ExibirNivelBateria ?? false)
            {
                decimal? nivelBateria = posicaoAtual?.NivelBateria;
                string faIcone = "";

                if (nivelBateria.HasValue)
                {
                    if (nivelBateria < 1 && nivelBateria > 0)//por algum motivo esta vindo 0.98, 0.35, 0.40...
                        nivelBateria = nivelBateria * 100;

                    if (nivelBateria < 5) faIcone = "fa-battery-empty";
                    else if (nivelBateria <= 25) faIcone = "fa-battery-quarter";
                    else if (nivelBateria <= 50) faIcone = "fa-battery-half";
                    else if (nivelBateria <= 75) faIcone = "fa-battery-three-quarters";
                    else faIcone = "fa-battery-full";
                }

                string nivel = nivelBateria.HasValue ? faIcone : "fa-question";
                informacoes.Add(ValueTuple.Create("nivel-bateria", nivel, true, ""));
            }

            if (configuracao?.ExibirSinal ?? false)
            {
                decimal? sinal = posicaoAtual?.NivelSinalGPS;
                string sinalGPS = sinal?.ToString("n2") ?? "-";
                informacoes.Add(ValueTuple.Create("sinal", sinalGPS, false, ""));
            }


            if (configuracao?.ExibirNumeroCarga ?? false)
            {
                informacoes.Add(ValueTuple.Create("numero-carga", carga.CodigoCargaEmbarcador, false, ""));
            }

            if (configuracao?.ExibirValorTotalProdutos ?? false)
            {
                decimal? valorTotal = carga.DadosSumarizados.ValorTotalProdutos;
                string valor = valorTotal?.ToString("n2") ?? "-";
                informacoes.Add(ValueTuple.Create("valor-total-produtos", valor, false, ""));
            }

            if (configuracao?.ExibirNumeroPedido ?? false)
            {
                string pedido = carga.DadosSumarizados?.NumeroPedidoEmbarcador ?? string.Empty;
                informacoes.Add(ValueTuple.Create("numero-pedido", pedido, false, ""));
            }

            if (configuracao?.ExibirNumeroPedidoCliente ?? false)
            {
                string pedidoCliente = carga.DadosSumarizados?.CodigoPedidoCliente ?? string.Empty;
                informacoes.Add(ValueTuple.Create("numero-pedido-cliente", pedidoCliente, false, ""));
            }

            if (configuracao?.ExibirNumeroOrdemPedido ?? false)
            {
                string pedidoCliente = carga.DadosSumarizados?.NumeroOrdem ?? string.Empty;
                informacoes.Add(ValueTuple.Create("numero-ordem-pedido", pedidoCliente, false, ""));
            }

            if (configuracao?.ExibirPrevisaoProximaParada ?? false)
            {
                CargaEntrega entrega = (from obj in entregasDaCarga where obj.Situacao == SituacaoEntrega.NaoEntregue select obj).FirstOrDefault();
                string previsao = entrega?.DataPrevista?.ToString("dd/MM HH:mm") ?? "-";
                informacoes.Add(ValueTuple.Create("previsao-proxima-parada", previsao, false, ""));
            }

            if (configuracao?.ExibirDistanciaRota ?? false)
            {
                decimal? quilometros = carga.DadosSumarizados?.Distancia;

                string distancia = quilometros.HasValue ? $"{quilometros?.ToString("n2")}km" : "-";
                informacoes.Add(ValueTuple.Create("distancia-rota", distancia, false, ""));
            }

            if (configuracao?.ExibirTempoRota ?? false)
            {
                int? horas;

                int tempoRotaFrete = (
                   from obj in cargasRotaFrete
                   where obj.Carga.Codigo == carga.Codigo
                   select obj.TempoDeViagemEmMinutos
               ).FirstOrDefault();

                if (tempoRotaFrete <= 0)
                    horas = carga.Rota?.TempoDeViagemEmMinutos != null ? (int?)Math.Ceiling((decimal)carga.Rota.TempoDeViagemEmMinutos / 60) : null;
                else
                    horas = (int?)Math.Ceiling((decimal)tempoRotaFrete / 60);

                string tempo = horas.HasValue ? $"{horas}h" : "-";
                informacoes.Add(ValueTuple.Create("tempo-rota", tempo, false, ""));
            }

            if (configuracao?.ExibirEntregaColetasRealizadas ?? false)
            {
                int quantidadeEntregues = (from obj in entregasDaCarga where obj.Situacao == SituacaoEntrega.Entregue select obj).Count();
                int quantidadeTotal = entregasDaCarga.Count();
                string realizadoETotal = $"{quantidadeEntregues}/{quantidadeTotal}";
                informacoes.Add(ValueTuple.Create("entrega-coletas-realizadas", realizadoETotal, false, ""));
            }

            if (configuracao?.ExibirPesoRestanteEntrega ?? false)
            {
                // Soma do peso de todas CargaEntregas que não têm situação Entregue
                decimal pesoRestante = (
                    from obj in cargaEntregaPedidos
                    where obj.CargaEntrega.Carga.Codigo == carga.Codigo && obj.CargaEntrega.Situacao != SituacaoEntrega.Entregue && !obj.CargaEntrega.Coleta
                    select obj.CargaPedido.Peso
                ).Sum();

                informacoes.Add(ValueTuple.Create("peso-restante-entregue", pesoRestante.ToString(), false, ""));
            }

            if (configuracao?.ExibirPrimeiroSegundoTrecho ?? false)
            {
                string valor = "";

                bool temRecebedor = carga.DadosSumarizados.Recebedores != null && carga.DadosSumarizados.Recebedores != "";
                bool temExpedidor = carga.DadosSumarizados.Expedidores != null && carga.DadosSumarizados.Expedidores != "";

                if (temRecebedor && !temExpedidor)
                {
                    valor = Localization.Resources.Cargas.ControleEntrega.PrimeiroTrecho;
                }
                else if (!temRecebedor && temExpedidor)
                {
                    valor = Localization.Resources.Cargas.ControleEntrega.SegundoTrecho;
                }
                else if (temRecebedor && temExpedidor)
                {
                    valor = Localization.Resources.Cargas.ControleEntrega.Intermediaria;
                }
                else if (!temRecebedor && !temExpedidor)
                {
                    valor = Localization.Resources.Cargas.ControleEntrega.CargaDireta;
                }

                informacoes.Add(ValueTuple.Create("primeiro-segundo-trecho", valor, false, ""));
            }

            if (configuracao?.ExibirFilial ?? false)
            {
                string valor = carga?.Filial?.Descricao ?? Localization.Resources.Cargas.ControleEntrega.SemFilial;
                informacoes.Add(ValueTuple.Create("filial", valor, false, ""));
            }

            if (configuracao?.ExibirAnalistaResponsavelMonitoramento ?? false)
            {
                //string valor = carga?.AnalistaResponsavelMonitoramento?.Nome ?? "Sem analista";
                string nomeAnalistaMonitoramento = carga?.AnalistaResponsavelMonitoramento?.Nome != "" && carga?.AnalistaResponsavelMonitoramento?.Nome != null ?
                                                        carga?.AnalistaResponsavelMonitoramento?.Nome.Split(' ').FirstOrDefault() : Localization.Resources.Cargas.ControleEntrega.SemAnalista;
                informacoes.Add(ValueTuple.Create("analista-responsavel-monitoramento", nomeAnalistaMonitoramento, false, ""));
            }

            if (configuracao?.ExibirTelefoneCelular ?? false)
            {
                string valor = motorista?.Celular_Formatado ?? "";
                informacoes.Add(ValueTuple.Create("telefone-celular", valor, false, ""));
            }

            if (configuracao?.ExibirPrevisaoRecalculada ?? false)
            {
                entregasDaCarga.Sort((a, b) => a.Ordem > b.Ordem ? 1 : -1);

                List<SituacaoEntrega> situacoesNaoEntregues =
                    new List<SituacaoEntrega>() {
                        SituacaoEntrega.NaoEntregue,
                        SituacaoEntrega.EmCliente,
                        SituacaoEntrega.AgAtendimento,
                        SituacaoEntrega.EntregarEmOutroCliente,
                    };

                CargaEntrega primeiraEntregaNaoRealizada = entregasDaCarga.Find(o => situacoesNaoEntregues.Contains(o.Situacao)) ?? entregasDaCarga.Last();
                DateTime? dataEntregaReprogramada = primeiraEntregaNaoRealizada.DataReprogramada ?? primeiraEntregaNaoRealizada.DataPrevista;

                informacoes.Add(ValueTuple.Create("previsao-recalculada", dataEntregaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "", false, ""));
            }

            if (configuracao?.ExibirExpedidor ?? false)
            {
                string valor = carga?.DadosSumarizados?.Expedidores ?? string.Empty;
                informacoes.Add(ValueTuple.Create("expedidor", valor, false, ""));
            }

            if (configuracao?.ExibirTransportador ?? false)
            {
                string valor = carga?.Empresa?.NomeFantasia ?? string.Empty;
                informacoes.Add(ValueTuple.Create("transportador", valor, false, ""));
            }

            if (configuracao?.ExibirTipoOperacao ?? false)
            {
                string valor = carga?.TipoOperacao?.Descricao ?? string.Empty;
                informacoes.Add(ValueTuple.Create("tipo-operacao", valor, false, ""));
            }

            if (configuracao?.ExibirPesoBruto ?? false)
            {
                string valor = carga?.Pedidos.Sum(p => p.Peso).ToString("n2");
                informacoes.Add(ValueTuple.Create("peso-bruto", valor, false, ""));
            }

            if (configuracao?.ExibirPesoLiquido ?? false)
            {
                string valor = carga?.Pedidos.Sum(p => p.PesoLiquido).ToString("n2");
                informacoes.Add(ValueTuple.Create("peso-liquido", valor, false, ""));
            }

            if (configuracao?.ExibirTendenciaEntrega ?? false)
            {
                var tendenciaNaoEntregue = carga.Entregas
                   .Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                               && !x.Coleta
                               && x.Tendencia != TendenciaEntrega.Nenhum)
                   .OrderBy(x => x.Ordem)
                   .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.Tendencia))
                   .FirstOrDefault();

                var tendenciaEntregue = carga.Entregas
                    .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                && x.Tendencia != TendenciaEntrega.Nenhum
                                && !x.Coleta)
                    .OrderByDescending(x => x.Ordem)
                    .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.Tendencia))
                    .FirstOrDefault();

                string valor = tendenciaNaoEntregue ?? tendenciaEntregue;

                informacoes.Add(ValueTuple.Create("tendencia-entrega", valor, false, ""));
            }

            if (configuracao?.ExibirTendenciaColeta ?? false)
            {

                var tendenciaNaoEntregue = carga.Entregas
                   .Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                               && x.Coleta
                               && x.Tendencia != TendenciaEntrega.Nenhum)
                   .OrderBy(x => x.Ordem)
                   .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.Tendencia))
                   .FirstOrDefault();

                var tendenciaEntregue = carga.Entregas
                    .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                && x.Tendencia != TendenciaEntrega.Nenhum
                                && x.Coleta)
                    .OrderByDescending(x => x.Ordem)
                    .Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(x.Tendencia))
                    .FirstOrDefault();

                string valor = tendenciaNaoEntregue ?? tendenciaEntregue;

                informacoes.Add(ValueTuple.Create("tendencia-coleta", valor, false, ""));
            }

            if (configuracao?.ExibirCanalEntrega ?? false)
            {
                string valor = string.Join(",", carga?.Pedidos?.Select(pedido => pedido?.Pedido?.CanalEntrega?.Descricao ?? string.Empty));
                informacoes.Add(ValueTuple.Create("canal-entrega", valor, false, ""));
            }

            if (configuracao?.ExibirCanalVenda ?? false)
            {
                string valor = string.Join(",", carga?.Pedidos?.Select(pedido => pedido?.Pedido?.CanalVenda?.Descricao ?? string.Empty));
                informacoes.Add(ValueTuple.Create("canal-venda", valor, false, ""));
            }

            if (configuracao?.ExibirModalTransporte ?? false)
            {
                string valor = carga?.Pedidos?.Select(pedido => TipoCobrancaMultimodalHelper.ObterDescricao(pedido.TipoCobrancaMultimodal)).FirstOrDefault() ?? string.Empty;
                informacoes.Add(ValueTuple.Create("modal-transporte", valor, false, ""));
            }

            if (configuracao?.ExibirMesorregiao ?? false)
            {
                string valor = string.Join(",", carga?.Pedidos?.Select(pedido => pedido.Pedido.Destinatario.MesoRegiao?.Descricao ?? string.Empty).Distinct());
                informacoes.Add(ValueTuple.Create("modal-mesorregiao", valor, false, ""));
            }

            if (configuracao?.ExibirRegiao ?? false)
            {
                string valor = string.Join(",", carga?.Pedidos?.Select(pedido => pedido.Pedido.Destinatario.Regiao?.Descricao ?? string.Empty).Distinct());
                informacoes.Add(ValueTuple.Create("modal-regiao", valor, false, ""));
            }


            if (configuracao?.ExibirProximoCliente ?? false)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega proximaEntrega = entregasDaCarga != null ? entregasDaCarga.FirstOrDefault(x => x.EstaPendente) : null;

                string valor = !string.IsNullOrEmpty(proximaEntrega?.Cliente?.NomeFantasia) ? proximaEntrega.Cliente.NomeFantasia : proximaEntrega?.Cliente?.NomeCNPJ;
                informacoes.Add(ValueTuple.Create("Cliente-Proxima-Entrega", valor, false, ""));
            }



            return (from info in informacoes
                    select new
                    {
                        info.Descricao,
                        info.Valor,
                        info.Icone,
                        info.Tooltip
                    }).ToList();
        }

        private dynamic ObterDetalhesGridMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = carga?.Veiculo?.PosicaoAtual?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = carga?.Monitoramento?.FirstOrDefault();

            DateTime data = carga.DataCarregamentoCarga ?? carga.DataCriacaoCarga;

            var cargaMonitorada = new
            {
                Carga = carga.Codigo,
                CargaEmbarcador = carga.CodigoCargaEmbarcador,
                Veiculo = new { Codigo = carga.Veiculo?.Codigo ?? 0, Descricao = carga.Veiculo?.Descricao ?? string.Empty },
                IDEquipamento = posicaoAtual?.IDEquipamento,
                Posicao = posicaoAtual?.DescricaoPosicao ?? string.Empty,
                Placa = ObterPlacas(carga.Veiculo, carga.VeiculosVinculados),
                Velocidade = posicaoAtual?.Velocidade ?? 0,
                Ignicao = posicaoAtual?.IgnicaoDescricao ?? string.Empty,
                Temperatura = posicaoAtual?.Temperatura ?? 0,
                ControleDeTemperatura = "SEM CONTROLE", //verificar
                Latitude = posicaoAtual?.Latitude ?? 0,
                Longitude = posicaoAtual?.Longitude ?? 0,
                PercentualViagem = monitoramento?.PercentualViagem ?? 0,
                Transportador = carga.Empresa?.Descricao ?? string.Empty,
                Origem = carga.Filial?.Descricao ?? string.Empty,
                Data = data.ToString("dd/MM/yyyy"),
                Motorista = carga.NomeMotoristas,
                Status = posicaoAtual != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicaoHelper.ObterDescricao(posicaoAtual.Status) : string.Empty
            };

            return cargaMonitorada;
        }

        private string ConcatenarCodigosIntegradoes(IEnumerable<string> codigos)
        {
            return String.Join(" - ", (from o in codigos where !string.IsNullOrEmpty(o) select o));
        }

        private string ConcatenarDestinatarios(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            return ConcatenarCodigosIntegradoes(from o in cargaPedidos select o?.Pedido?.Destinatario?.CodigoIntegracao);
        }

        private bool ObterPrevisaoNaJanelaDescarga(Dominio.Entidades.Cliente cliente, DateTime? dataPrevisao)
        {
            if ((cliente == null) || (dataPrevisao == null))
                return true;

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> listaDescarga = (from obj in cliente?.ClienteDescargas
                                                                                        where string.IsNullOrEmpty(obj?.HoraInicioDescarga) == false &&
                                                                                              string.IsNullOrEmpty(obj?.HoraLimiteDescarga) == false
                                                                                        select obj).ToList();

            if (listaDescarga.Count == 0)
                return true;

            DateTime? horaPrevisao = Convert.ToDateTime(dataPrevisao?.ToString("HH:mm:ss"));

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> descargasNoHorario = (from obj in cliente.ClienteDescargas
                                                                                             where Convert.ToDateTime(obj.HoraInicioDescarga) < horaPrevisao && Convert.ToDateTime(obj.HoraLimiteDescarga) > horaPrevisao
                                                                                             select obj).ToList();
            return descargasNoHorario.Count > 0;

        }

        private List<Dominio.Entidades.Cliente> ObterPessoaFilial(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<double> codigoFiliais = (from carga in cargas where carga.Filial != null select carga.Filial.CNPJ.ToDouble()).Distinct().ToList();

            List<Cliente> pessoaFiliais = repCliente.BuscarPorCPFCNPJs(codigoFiliais);
            return pessoaFiliais;
        }

        private string RetornarNumerosCTes(List<int> codigosPedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<int> numerosCTes = new List<int>();

            if (codigosPedidoXMLNotaFiscal == null || codigosPedidoXMLNotaFiscal.Count == 0)
                return "";

            if (codigosPedidoXMLNotaFiscal.Count < 2000)
                numerosCTes = repCargaPedidoXMLNotaFiscalCTe.NumerosCTesPorNotasFiscais(codigosPedidoXMLNotaFiscal);
            else
            {
                try
                {
                    decimal decimalBlocos = Math.Ceiling(((decimal)codigosPedidoXMLNotaFiscal.Count) / 1000);
                    int blocos = (int)Math.Truncate(decimalBlocos);

                    for (int i = 0; i < blocos; i++)
                    {
                        Servicos.Log.TratarErro($"Controle Entrega - blocos {codigosPedidoXMLNotaFiscal.Count} indice {i}");
                        numerosCTes.AddRange(repCargaPedidoXMLNotaFiscalCTe.NumerosCTesPorNotasFiscais((codigosPedidoXMLNotaFiscal.Skip(i * 1000).Take(1000).ToList())));
                    }

                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }
            }

            if (numerosCTes != null && numerosCTes.Count > 0)
                return string.Join(", ", numerosCTes);
            else
                return "";
        }

        private bool IdentificarFiltrosConsultados(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa)
        {
            if (entrega != null && !entrega.Coleta && filtrosPesquisa != null)
            {
                if (filtrosPesquisa.NumeroPedido > 0)
                {
                    bool pedidoNestaEntrega = entrega.Pedidos.Where(o => o.CargaPedido.Pedido.Numero == filtrosPesquisa.NumeroPedido).FirstOrDefault() != null;
                    if (pedidoNestaEntrega) return true;
                }

                if (filtrosPesquisa.NumeroPedidosEmbarcador.Count > 0)
                {
                    bool pedidoNestaEntrega = false;

                    foreach (string numeroPedidoEmbarcadorDigitado in filtrosPesquisa.NumeroPedidosEmbarcador)
                    {
                        pedidoNestaEntrega = entrega.Pedidos.Any(o => o.CargaPedido.Pedido.NumeroPedidoEmbarcador.ToUpper().Contains(numeroPedidoEmbarcadorDigitado.ToUpper()));
                        if (pedidoNestaEntrega) return true;
                    }
                }

                if (filtrosPesquisa.NumeroNotasFiscais?.Count > 0)
                {
                    bool notaFiscalNestaEntrega = entrega.NotasFiscais.Where(o => filtrosPesquisa.NumeroNotasFiscais.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)).FirstOrDefault() != null;
                    if (notaFiscalNestaEntrega) return true;
                }

                if (filtrosPesquisa.NumeroNotaFiscal > 0)
                {
                    bool notaFiscalNestaEntrega = entrega.NotasFiscais.Any(o => filtrosPesquisa.NumeroNotaFiscal.Equals(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero));
                    if (notaFiscalNestaEntrega) return true;
                }

                if (filtrosPesquisa.CodigosVendedor?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CodigosVendedor.Contains(p.CargaPedido.Pedido.FuncionarioVendedor?.Codigo ?? 0)))
                    return true;

                if (filtrosPesquisa.CodigosSupervisor?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CodigosSupervisor.Contains(p.CargaPedido.Pedido.FuncionarioSupervisor?.Codigo ?? 0)))
                    return true;

                if (filtrosPesquisa.CodigosGerente?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CodigosGerente.Contains(p.CargaPedido.Pedido.FuncionarioGerente?.Codigo ?? 0)))
                    return true;

                if (filtrosPesquisa.CpfCnpjExpedidores?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CpfCnpjExpedidores.Contains(p.CargaPedido.Pedido.Expedidor?.CPF_CNPJ ?? 0)))
                    return true;

                if (filtrosPesquisa.CpfCnpjEmitentes?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CpfCnpjEmitentes.Contains(p.CargaPedido.Pedido.Remetente?.Codigo ?? 0)))
                    return true;

                if (filtrosPesquisa.CpfCnpjDestinatarios?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CpfCnpjDestinatarios.Contains(p.CargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0)))
                    return true;

                if (filtrosPesquisa.CodigoResponsavelEntrega > 0)
                {
                    if (entrega.Carga.ResponsavelEntrega != null && entrega.Carga.ResponsavelEntrega.Codigo == filtrosPesquisa.CodigoResponsavelEntrega)
                        return true;
                }

                if (filtrosPesquisa.Recebedor?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.Recebedor.Contains(p.CargaPedido.Pedido.Recebedor?.CPF_CNPJ ?? 0)))
                    return true;

                if (filtrosPesquisa.CanaisEntrega?.Count > 0 && entrega.Pedidos.Any(p => filtrosPesquisa.CanaisEntrega.Contains(p.CargaPedido.Pedido.CanalEntrega?.Codigo ?? 0)))
                    return true;

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoSap) && entrega.Pedidos.Any(p => p.CargaPedido.Pedido.Destinatario.CodigoSap == filtrosPesquisa.CodigoSap))
                    return true;
            }

            return false;
        }

        private bool VerificarConfiguracaoHabilitarImportacaoCargaFluvial(Repositorio.UnitOfWork unitOfWork)
        {
            bool habilitar = false;
            try
            {
                habilitar = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().CargaControleEntrega_Habilitar_ImportacaoCargaFluvial.Value;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter configuração de importação de carga fluvial: {ex.ToString()}", "CatchNoAction");
            }
            return habilitar;
        }

        private Models.Grid.Grid ObterGridMonitoramentoCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisaMonitoramento, IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentos, int codigoUsuario, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            int total = monitoramentos?.Count() ?? 0;
            if (total > 0)
            {
                for (int i = 0; i < total; i++)
                {
                    if (monitoramentos[i].Carga == codigoCarga)
                    {
                        Logistica.GridMonitoramento gridMonitoramento = new Logistica.GridMonitoramento();
                        Models.Grid.Grid grid = gridMonitoramento.ObterGrid(null, codigoUsuario, filtrosPesquisaMonitoramento, ConfiguracaoEmbarcador, unitOfWork, this.TipoServicoMultisoftware);
                        grid.DesabilitarTodasOrdenacoes();
                        gridMonitoramento.AdicionarRegistrosAoGrid(new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> { monitoramentos[i] }, grid, ConfiguracaoEmbarcador, filtrosPesquisaMonitoramento);
                        return grid;
                    }
                }
            }
            return null;
        }

        private void VerificarPedidoReentregaNaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            bool aindaPossuiReentrega = repCargaEntrega.VerificarCargaPedidosComReentregaSolicitada(entrega.Carga.Codigo, entrega.Cliente.CPF_CNPJ);

            if (entrega.PossuiPedidosReentrega != aindaPossuiReentrega)
            {
                entrega.PossuiPedidosReentrega = aindaPossuiReentrega;
                repCargaEntrega.Atualizar(entrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entrega, repCargaEntrega, unitOfWork);
            }
        }

        private bool ObterImagemMaisDeUmaCarga(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (entrega == null)
                return false;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repositorioCargaPedido.BuscarPorPedidos(repositorioCargaEntregaPedido.BuscarCodigosPedidosPorCargaEntrega(entrega.Codigo));

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPed = repositorioCargaPedido.BuscarPorPedido(cargaPedido?.Pedido?.Codigo ?? 0);

                if (listaCargaPed?.Count > 1)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
