using AdminMultisoftware.Dominio.Enumeradores;
using Servicos.Embarcador.Notificacao;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Chat
{
    public class ChatMensagem
    {
        public static Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem EnviarMensagemChat(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario remetente, DateTime dataMensagem, List<Dominio.Entidades.Usuario> destinatarios, string mensagem, int codigoClienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.Carga.Chat.PromotorChat> ListaPromotor = null)
        {
            Repositorio.Embarcador.Cargas.ChatMensagemDestinatario repChatMensagemDestinatario = new Repositorio.Embarcador.Cargas.ChatMensagemDestinatario(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repxmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            var serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem = new Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem();
            chatMobileMensagem.Carga = carga;
            chatMobileMensagem.DataCriacao = dataMensagem;
            chatMobileMensagem.Mensagem = mensagem.Length < 500 ? mensagem : mensagem.Left(500);
            chatMobileMensagem.DataConfirmacaoLeitura = DateTime.Now;
            chatMobileMensagem.Remetente = remetente;
            chatMobileMensagem.Pedido = pedido;

            repChatMobileMensagem.Inserir(chatMobileMensagem);

            foreach (Dominio.Entidades.Usuario destinatario in destinatarios)
            {
                Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario chatMensagemDestinatario = new Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario();
                chatMensagemDestinatario.Destinatario = destinatario;
                chatMensagemDestinatario.ChatMobileMensagem = chatMobileMensagem;
                chatMensagemDestinatario.DataRecebimento = DateTime.Now;
                repChatMensagemDestinatario.Inserir(chatMensagemDestinatario);
                if (destinatario.CodigoMobile > 0)
                {
                    EnviarNotificacaoMobile(chatMobileMensagem, codigoClienteMultisoftware, destinatario.CodigoMobile);
                    // Notificação pelo OneSignal para o novo app
                    serNotificacaoMTrack.NotificarMensagemChat(destinatario, carga, chatMobileMensagem.Mensagem, chatMobileMensagem.Remetente?.Nome ?? "");
                }
                else
                {
                    //todo: notificar via portal. transportador embarcador etc.
                    Servicos.Embarcador.Hubs.Chat hubChat = new Servicos.Embarcador.Hubs.Chat();

                    hubChat.NotificarMensagemUsuario(destinatario.Codigo, remetente.Codigo, chatMobileMensagem.Mensagem, chatMobileMensagem.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"), remetente.Nome);
                }
            }

            if (configuracaoTMS?.UtilizaAppTrizy ?? false)
                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.EnviarMensagem(carga, mensagem, dataMensagem, unitOfWork);


            // Se tiver integração com a Dansales, enviar para eles a mensagem também
            var chatDansales = new Servicos.Embarcador.Integracao.Dansales.ChatIntegracaoDansales(unitOfWork);
            if (ListaPromotor != null && ListaPromotor.Count > 0)
            {
                foreach (var promotor in ListaPromotor)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repxmlNotaFiscal.BuscarPorCodigo(promotor.NotaFiscal);
                    if (xmlNotaFiscal != null)
                        chatDansales.CriarEntidadeTemporariaChatMensagemDansales(carga, xmlNotaFiscal, chatMobileMensagem);
                }
            }

            chatDansales.EnviarMensagem(carga, remetente, chatMobileMensagem);

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(ObterMensagemMontada(chatMobileMensagem, codigoClienteMultisoftware), codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.ControleColetaEntrega, Servicos.SignalR.Hubs.ControleColetaEntrega.GetHub(Servicos.SignalR.Hubs.ControleColetaEntregaHubs.MensagemChatEnviada));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);

            Dominio.MSMQ.Notification notificationChamado = new Dominio.MSMQ.Notification(ObterMensagemMontada(chatMobileMensagem, codigoClienteMultisoftware), codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.ControleColetaEntrega, Servicos.SignalR.Hubs.ChamadoChat.GetHub(Servicos.SignalR.Hubs.ControleColetaEntregaHubs.MensagemChatEnviada));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notificationChamado);

            Dominio.MSMQ.Notification notificationChatPedido = new Dominio.MSMQ.Notification(ObterMensagemMontada(chatMobileMensagem, codigoClienteMultisoftware), codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.Pedidos, Servicos.SignalR.Hubs.Pedidos.GetHub(Servicos.SignalR.Hubs.PedidoHubs.MensagemChatEnviada));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notificationChatPedido);

            Dominio.MSMQ.Notification notificationChatPedidoPortal = new Dominio.MSMQ.Notification(ObterMensagemMontada(chatMobileMensagem, codigoClienteMultisoftware), codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.Fornecedor, Dominio.SignalR.Hubs.Pedidos, Servicos.SignalR.Hubs.Pedidos.GetHub(Servicos.SignalR.Hubs.PedidoHubs.MensagemChatEnviada));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notificationChatPedidoPortal);


            return chatMobileMensagem;
        }

        public static void EnviarNotificacaoMobile(Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem, int codigoClienteMultisoftware, int codigoMobile)
        {
            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(ObterMensagemMontada(chatMobileMensagem, codigoClienteMultisoftware), codigoClienteMultisoftware, codigoMobile, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.Mobile, Servicos.SignalR.Mobile.GetHub(MobileHubs.MensagemChat));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        public static dynamic ObterMensagemMontada(Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem, int codigoClienteMultisoftware)
        {
            dynamic mensagem = new
            {
                codigo = chatMobileMensagem.Codigo,
                carga = chatMobileMensagem.Carga != null ? chatMobileMensagem.Carga.Codigo : 0,
                pedido = chatMobileMensagem.Pedido != null ? chatMobileMensagem.Pedido.Codigo : 0,
                clienteMultisoftware = codigoClienteMultisoftware,
                remetente = chatMobileMensagem.Remetente.Nome,
                codigoRemetente = chatMobileMensagem.Remetente.Codigo,
                codigoMobileRemetente = chatMobileMensagem.Remetente.CodigoMobile,
                mensagem = chatMobileMensagem.Mensagem,
                dataMensagem = chatMobileMensagem.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"),
                visualizada = chatMobileMensagem.MensagemLida,
                falhaIntegracao = chatMobileMensagem.MensagemFalhaIntegracao
            };
            return mensagem;
        }

        public static void NotificarMensagemRecebida(Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem, int codigoClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new TorreControle.AlertaAcompanhamentoCargaMensagens();

            dynamic mensagem = new
            {
                codigo = chatMobileMensagem.Codigo,
                carga = chatMobileMensagem.Carga != null ? chatMobileMensagem.Carga.Codigo : 0,
                pedido = chatMobileMensagem.Pedido != null ? chatMobileMensagem.Pedido.Codigo : 0
            };

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(mensagem, codigoClienteMultisoftware, chatMobileMensagem.Remetente.Codigo, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.ControleColetaEntrega, Servicos.SignalR.Hubs.ControleColetaEntrega.GetHub(Servicos.SignalR.Hubs.ControleColetaEntregaHubs.MensagemRecebida));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);

            Dominio.MSMQ.Notification notificationChamado = new Dominio.MSMQ.Notification(mensagem, codigoClienteMultisoftware, chatMobileMensagem.Remetente.Codigo, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.ControleColetaEntrega, Servicos.SignalR.Hubs.ChamadoChat.GetHub(Servicos.SignalR.Hubs.ControleColetaEntregaHubs.MensagemRecebida)); ;
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notificationChamado);

            Dominio.MSMQ.Notification notificationChatPedido = new Dominio.MSMQ.Notification(mensagem, codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.Pedidos, Servicos.SignalR.Hubs.Pedidos.GetHub(Servicos.SignalR.Hubs.PedidoHubs.MensagemRecebida));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notificationChatPedido);

            Dominio.MSMQ.Notification notificationChatPedidoPortal = new Dominio.MSMQ.Notification(mensagem, codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.Fornecedor, Dominio.SignalR.Hubs.Pedidos, Servicos.SignalR.Hubs.Pedidos.GetHub(Servicos.SignalR.Hubs.PedidoHubs.MensagemRecebida));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notificationChatPedidoPortal);

            servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

            //if (chatMobileMensagem.Remetente.CodigoMobile > 0)
            //todo: implementar aqui confirmação leitura mobile criando um hub no serviço mobile que notifica o motorista igual foi feito no portal

        }
    }
}
