using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Notificacao;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Notificacao
{
    /// <summary>
    /// Essa classe cuida da regra de negócio de envio das notificações para o novo app MTrack.
    /// </summary>
    public class NotificacaoMTrack
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork unitOfWork;

        #endregion

        #region Construtores

        public NotificacaoMTrack(Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void NotificarMudancaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Usuario> motoristas, MobileHubs tipo, bool notificarSignalR = false, int codigoClienteMultisoftware = 0)
        {
            foreach (var motorista in motoristas)
            {
                NotificarMudancaCargaEntrega(cargaEntrega, motorista, tipo, notificarSignalR, codigoClienteMultisoftware);
            }
        }

        public void NotificarMudancaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Usuario motorista, MobileHubs tipo, bool notificarSignalR = false, int codigoClienteMultisoftware = 0)
        {
            GetDadosNotificacaoCargaEntrega(cargaEntrega, tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data);
            var serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);
            serNotificacaoOneSignal.EnfilerarNotificacao(motorista, tipo, headings, contents, data);

            if (notificarSignalR)
            {
                dynamic conteudo = new
                {
                    Carga = cargaEntrega.Carga.Codigo,
                    Entrega = cargaEntrega.Codigo,
                    Acao = tipo
                };

                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
                        conteudo,
                        codigoClienteMultisoftware,
                        motorista.CodigoMobile,
                        Dominio.MSMQ.MSMQQueue.SGTMobile,
                        Dominio.SignalR.Hubs.Mobile,
                        SignalR.Mobile.GetHub(tipo)
                    );

                MSMQ.MSMQ.SendPrivateMessage(notification);
            }
        }

        public void NotificarMudancaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Usuario> motoristas, MobileHubs tipo, bool notificarSignalR = false, int codigoClienteMultisoftware = 0, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                NotificarMudancaCarga(carga, motorista, tipo, notificarSignalR, codigoClienteMultisoftware, pedido);
            }
        }

        public void NotificarMudancaChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise, List<Dominio.Entidades.Usuario> motoristas, MobileHubs tipo)
        {
            foreach (var motorista in motoristas)
            {
                NotificarMudancaChamado(chamado, chamadoAnalise, motorista, tipo);
            }
        }

        public void NotificarMudancaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista, MobileHubs tipo, bool notificarSignalR = false, int codigoClienteMultisoftware = 0, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            if (tipo == MobileHubs.NovaCargaMotorista)
            {
                bool utilizaAppTrizy = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao().UtilizaAppTrizy;

                if (utilizaAppTrizy)
                    return;
            }
            GetDadosNotificacaoCarga(carga, pedido, tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data);
            NotificacaoOneSignal serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);
            serNotificacaoOneSignal.EnfilerarNotificacao(motorista, tipo, headings, contents, data);

            if (notificarSignalR)
            {
                dynamic conteudo = new
                {
                    CodigoCarga = carga.Codigo,
                    Carga = carga.Codigo,
                    Acao = tipo
                };

                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
                    conteudo,
                    codigoClienteMultisoftware,
                    motorista.CodigoMobile,
                    Dominio.MSMQ.MSMQQueue.SGTMobile,
                    Dominio.SignalR.Hubs.Mobile,
                    SignalR.Mobile.GetHub(tipo)
                );

                MSMQ.MSMQ.SendPrivateMessage(notification);
            }
        }

        public void NotificarMudancaChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise, Dominio.Entidades.Usuario motorista, MobileHubs tipo)
        {
            GetDadosNotificacaoChamado(chamado, chamadoAnalise, tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data);
            var serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);
            serNotificacaoOneSignal.EnfilerarNotificacao(motorista, tipo, headings, contents, data);
        }

        public void NotificarMensagemChat(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Cargas.Carga carga, string mensagem, string usuario)
        {
            var serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);
            OneSignalHeadings headings = new OneSignalHeadings(pt: "Você recebeu uma mensagem");
            OneSignalContents contents = new OneSignalContents(pt: mensagem);
            OneSignalData data = new OneSignalData
            {
                CodigoCarga = carga.Codigo,
                NomeAnalistaChamado = usuario
            };
            serNotificacaoOneSignal.EnfilerarNotificacao(motorista, MobileHubs.MensagemChat, headings, contents, data);
        }

        public void NotificarPushGenerica(List<Dominio.Entidades.Usuario> motoristas, string mensagem)
        {
            foreach (var motorista in motoristas)
            {
                NotificarPushGenerica(motorista, mensagem);
            }
        }

        /// <summary>
        /// Envia uma push notification genérica para o motorista. Essa push não abre nenhuma ação no app e serve apenas
        /// enviar uma mensagem genérica.
        /// </summary>
        public void NotificarPushGenerica(Dominio.Entidades.Usuario motorista, string mensagem)
        {
            var serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);
            OneSignalHeadings headings = new OneSignalHeadings(pt: "Você recebeu uma mensagem");
            OneSignalContents contents = new OneSignalContents(pt: mensagem);
            OneSignalData data = new OneSignalData { };
            serNotificacaoOneSignal.EnfilerarNotificacao(motorista, MobileHubs.PushNotificationGenerica, headings, contents, data);
        }

        public void NotificarMudancaNaoConformidade(List<Dominio.Entidades.Usuario> motoristas, MobileHubs tipo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string numeroNotas = null)
        {
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
                NotificarMudancaNaoConformidade(motorista, tipo, cargaEntrega, numeroNotas);
        }

        public void NotificarMudancaNaoConformidade(Dominio.Entidades.Usuario motorista, MobileHubs tipo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string numeroNotas)
        {
            GetDadosNotificacaoNaoConformidade(cargaEntrega, numeroNotas, tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data);

            NotificacaoOneSignal serNotificacaoOneSignal = new NotificacaoOneSignal(unitOfWork);
            serNotificacaoOneSignal.EnfilerarNotificacao(motorista, tipo, headings, contents, data);
        }

        #endregion

        #region Métodos Privados

        private void GetDadosNotificacaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, MobileHubs tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data)
        {
            headings = new OneSignalHeadings(pt: "");
            contents = new OneSignalContents(pt: "");
            data = new OneSignalData();

            if (cargaEntrega == null || cargaEntrega.Carga == null || cargaEntrega.Cliente == null)
                return;

            string tipoCargaEntrega = cargaEntrega.Coleta ? "coleta" : "entrega";
            int ordem = cargaEntrega.Ordem + 1;
            string nomeCliente = cargaEntrega.Cliente != null ? (cargaEntrega.Cliente.PontoTransbordo ? cargaEntrega.Cliente.NomeFantasia : cargaEntrega.Cliente.Nome) : string.Empty;

            switch (tipo)
            {
                case MobileHubs.EntregaAdicionada:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"Foi adicionada uma {tipoCargaEntrega} na carga {cargaEntrega.Carga.CodigoCargaEmbarcador}"
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;

                case MobileHubs.EntregaExcluida:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"Foi excluída uma {tipoCargaEntrega} na carga {cargaEntrega.Carga.CodigoCargaEmbarcador}"
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;

                case MobileHubs.EntregaAlterada:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A {tipoCargaEntrega} {ordem} na carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi alterada"
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;
                case MobileHubs.EntregaConfirmadaNoEmbarcador:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A {tipoCargaEntrega} {ordem} do cliente {nomeCliente} foi confirmada pelo embarcador"
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;
                case MobileHubs.EntregaRejeitadaNoEmbarcador:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A {tipoCargaEntrega} {ordem} do cliente {nomeCliente} foi rejeitada pelo embarcador"
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;
                case MobileHubs.EntregaPrevisaoAtualizada:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {cargaEntrega.Carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A previsão da {tipoCargaEntrega} {ordem} do cliente {nomeCliente} foi alterada"
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;
            }

        }

        private void GetDadosNotificacaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, MobileHubs tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data)
        {
            switch (tipo)
            {
                case MobileHubs.CargaAtualizada:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A carga {carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    data = new OneSignalData
                    {
                        CodigoCarga = carga.Codigo,
                    };

                    return;
                case MobileHubs.DocumentosDeTransporteEmitidos:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"Os documentos de transporte da Carga {carga.CodigoCargaEmbarcador} foram emitidos"
                    );
                    data = new OneSignalData
                    {
                        CodigoCarga = carga.Codigo,
                    };

                    return;
                case MobileHubs.UnificacaoCarga:
                    headings = new OneSignalHeadings(
                        pt: $"A sua carga {carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A carga {carga.CodigoCargaEmbarcador} foi atualizada"
                    );
                    data = new OneSignalData
                    {
                        CodigoCarga = carga.Codigo,
                    };

                    return;
                case MobileHubs.CargaMotoristaNecessitaConfirmar:
                    headings = new OneSignalHeadings(
                        pt: $"Nova carga"
                    );
                    contents = new OneSignalContents(
                       pt: $"Uma nova carga está disponível para você"
                    );
                    data = new OneSignalData
                    {
                        CodigoCarga = carga.Codigo,
                        DataComparecerEscalaPedido = pedido?.DataComparecerEscala?.ToUnixSeconds(),
                        DataLimiteConfirmacaoMotorista = carga.DataLimiteConfirmacaoMotorista?.ToUnixSeconds()
                    };

                    return;
                case MobileHubs.NovaCargaMotorista:
                    string mensagem = $"Uma nova carga está disponível para você";

                    if (carga.DataCarregamentoCarga.HasValue && carga.DataCarregamentoCarga.Value != DateTime.MinValue)
                        mensagem += $@". Carregamento em {carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm")}";

                    headings = new OneSignalHeadings(
                        pt: $"Nova carga {carga.CodigoCargaEmbarcador}"
                    );
                    contents = new OneSignalContents(
                       pt: mensagem
                    );
                    data = new OneSignalData
                    {
                        CodigoCarga = carga.Codigo,
                    };

                    return;
            }

            headings = new OneSignalHeadings(pt: "");
            contents = new OneSignalContents(pt: "");
            data = new OneSignalData();
        }

        private void GetDadosNotificacaoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise, MobileHubs tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data)
        {
            switch (tipo)
            {
                case MobileHubs.ChamadoEmAnalise:
                    var analista = chamado.Responsavel?.Nome ?? "";
                    headings = new OneSignalHeadings(
                        pt: $"Atualização na ocorrência"
                    );
                    contents = new OneSignalContents(
                        pt: $"A ocorrência do cliente {chamado.CargaEntrega?.Cliente?.Nome} está em atendimento pelo analista {analista}"
                    );
                    data = new OneSignalData
                    {
                        CodigoChamado = chamado.Codigo,
                        NomeAnalistaChamado = chamado.Responsavel?.Nome ?? "",
                        CodigoCargaEntrega = chamado.CargaEntrega.Codigo,
                        CodigoCarga = chamado.CargaEntrega.Carga.Codigo,
                        TratativaDevolucao = chamado.TratativaDevolucao.ObterDescricaoTratativaDevolucao(),
                    };

                    return;
                case MobileHubs.ChamadoAtualizado:
                    headings = new OneSignalHeadings(
                        pt: $"Atualização na ocorrência"
                    );
                    contents = new OneSignalContents(
                       pt: $"A ocorrência do cliente {chamado.CargaEntrega?.Cliente?.Nome} foi atualizada"
                    );
                    data = new OneSignalData
                    {
                        CodigoChamado = chamado.Codigo,
                        Observacao = string.IsNullOrEmpty(chamado.ObservacaoRetornoMotorista) ? (chamadoAnalise?.Observacao ?? "") : chamado.ObservacaoRetornoMotorista,
                        CodigoCargaEntrega = chamado.CargaEntrega?.Codigo,
                        CodigoCarga = chamado.CargaEntrega?.Carga.Codigo,
                        SituacaoChamado = chamado.Situacao,
                        SituacaoCargaEntrega = chamado.CargaEntrega?.Situacao,
                        DiferencaDevolucao = chamado.CargaEntrega?.NotificarDiferencaDevolucao,
                        TratativaDevolucao = chamado.TratativaDevolucao.ObterDescricaoTratativaDevolucao(),
                    };

                    return;
            }

            headings = new OneSignalHeadings(pt: "");
            contents = new OneSignalContents(pt: "");
            data = new OneSignalData();
        }

        private void GetDadosNotificacaoNaoConformidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string numeroNotas, MobileHubs tipo, out OneSignalHeadings headings, out OneSignalContents contents, out OneSignalData data)
        {
            string tipoCargaEntrega = cargaEntrega != null ? cargaEntrega.Coleta ? "coleta" : "entrega" : string.Empty;
            int ordem = cargaEntrega != null ? cargaEntrega.Ordem + 1 : 0;

            switch (tipo)
            {
                case MobileHubs.NaoConformidade:
                    headings = new OneSignalHeadings(
                        pt: $"Não-conformidade identificada"
                    );
                    contents = new OneSignalContents(
                        pt: $"As NF-es {numeroNotas} apresentaram divergência. Aguarde atualizações."
                    );
                    data = new OneSignalData { };

                    return;
                case MobileHubs.NaoConformidadeColetaAutorizada:
                    headings = new OneSignalHeadings(
                       pt: $"A {tipoCargaEntrega} {ordem} foi autorizada"
                    );
                    contents = new OneSignalContents(
                        pt: $"A análise das notas fiscais foram concluídas. Confirme sua coleta."
                    );
                    data = new OneSignalData
                    {
                        CodigoCargaEntrega = cargaEntrega.Codigo,
                    };

                    return;
            }

            headings = new OneSignalHeadings(pt: "");
            contents = new OneSignalContents(pt: "");
            data = new OneSignalData();
        }

        #endregion
    }
}
