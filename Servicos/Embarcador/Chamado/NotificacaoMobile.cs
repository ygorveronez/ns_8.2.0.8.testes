using System.Collections.Generic;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Servicos.Embarcador.Notificacao;

namespace Servicos.Embarcador.Chamado
{
    public sealed class NotificacaoMobile
    {
        #region Atributos

        private readonly int _codigoCliente;
        private readonly Repositorio.UnitOfWork unitOfWork;

        #endregion

        #region Construtores

        public NotificacaoMobile(Repositorio.UnitOfWork unitOfWork, int codigoCliente)
        {
            this.unitOfWork = unitOfWork;
            _codigoCliente = codigoCliente;
        }

        #endregion

        #region Métodos Públicos

        public void NotificarMotoristasChamadoAtualizado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise, List<Dominio.Entidades.Usuario> motoristas, dynamic conteudo)
        {
            NotificarMotoristas(MobileHubs.ChamadoAtualizado, motoristas, conteudo);
            var serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
            serNotificacaoMTrack.NotificarMudancaChamado(chamado, chamadoAnalise, motoristas, MobileHubs.ChamadoAtualizado);
        }

        public void NotificarMotoristasChamadoEmAnalise(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<Dominio.Entidades.Usuario> motoristas, dynamic conteudo)
        {
            NotificarMotoristas(MobileHubs.ChamadoEmAnalise, motoristas, conteudo);
            var serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
            serNotificacaoMTrack.NotificarMudancaChamado(chamado, null, motoristas, MobileHubs.ChamadoEmAnalise);
        }

        public void NotificarMotoristasNaoConformidadeColetaAutorizada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Usuario> motoristas, dynamic conteudo)
        {
            NotificarMotoristas(MobileHubs.NaoConformidadeColetaAutorizada, motoristas, conteudo);

            NotificacaoMTrack serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
            serNotificacaoMTrack.NotificarMudancaNaoConformidade(motoristas, MobileHubs.NaoConformidadeColetaAutorizada, cargaEntrega);
        }

        public void NotificarMotoristasUnificacaoCarga(List<Dominio.Entidades.Usuario> motoristas, dynamic conteudo)
        {
            NotificarMotoristas(MobileHubs.UnificacaoCarga, motoristas, conteudo);
        }

        public void NotificarMotoristasPushGenerica(List<Dominio.Entidades.Usuario> motoristas, dynamic conteudo)
        {
            NotificarMotoristas(MobileHubs.PushNotificationGenerica, motoristas, conteudo);
        }

        /// <summary>
        /// Manda mensagem para o app notificando todos motoristas que um motorista modificou a carga.
        /// Não envia para o motorista que modificou.
        /// </summary>
        public void NotificarCargaAtualizadaPorOutroMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Usuario motoristaQueAtualizou, TipoEventoAlteracaoCargaPorOutroMotorista tipoEvento)
        {
            if (carga == null)
                return;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Usuario> motoristasParaEnviarNotificacao = new List<Dominio.Entidades.Usuario> { };

            foreach (var motorista in carga.Motoristas)
            {
                if (motorista.Codigo != motoristaQueAtualizou.Codigo)
                {
                    motoristasParaEnviarNotificacao.Add(motorista);
                }
            }

            if (motoristasParaEnviarNotificacao.Count > 0)
            {
                dynamic pt = new { };

                switch (tipoEvento)
                {
                    case TipoEventoAlteracaoCargaPorOutroMotorista.InicioViagem:
                        pt = new
                        {
                            descricaoEvento = tipoEvento.ObterDescricao(),
                            mensagem = $"{motoristaQueAtualizou.Nome} iniciou a viagem da carga {carga.CodigoCargaEmbarcador}",
                        };
                        break;
                    case TipoEventoAlteracaoCargaPorOutroMotorista.FinalizacaoEntregaColeta:
                        pt = new
                        {
                            descricaoEvento = tipoEvento.ObterDescricao(),
                            mensagem = $"{motoristaQueAtualizou.Nome} finalizou a entrega/coleta {cargaEntrega.Ordem} da carga {carga.CodigoCargaEmbarcador}"
                        };
                        break;
                    case TipoEventoAlteracaoCargaPorOutroMotorista.RejeicaoEntregaColeta:
                        pt = new
                        {
                            descricaoEvento = tipoEvento.ObterDescricao(),
                            mensagem = $"{motoristaQueAtualizou.Nome} rejeitou a entrega/coleta {cargaEntrega.Ordem} da carga {carga.CodigoCargaEmbarcador}",
                        };
                        break;
                    case TipoEventoAlteracaoCargaPorOutroMotorista.FimViagem:
                        pt = new
                        {
                            descricaoEvento = tipoEvento.ObterDescricao(),
                            mensagem = $"{motoristaQueAtualizou.Nome} finalizou a viagem da carga ${carga.CodigoCargaEmbarcador}",
                        };
                        break;
                }

                dynamic conteudo = new
                {
                    evento = tipoEvento,
                    pt = pt,
                    es = new { }, // Se precisar espanhol no futuro
                    en = new { }, // Se precisar inglês no futuro
                };

                NotificarMotoristas(MobileHubs.CargaAtualizadaPorOutroMotorista, motoristasParaEnviarNotificacao, conteudo);

                // Notificar novo app MTrack
                var serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
                serNotificacaoMTrack.NotificarMudancaCarga(carga, motoristasParaEnviarNotificacao, MobileHubs.CargaAtualizadaPorOutroMotorista);
            }
        }

        #endregion

        #region Métodos Privados

        private void NotificarMotoristas(MobileHubs tipoHub, List<Dominio.Entidades.Usuario> motoristas, dynamic conteudo)
        {
            string hub = SignalR.Mobile.GetHub(tipoHub);

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(conteudo, _codigoCliente, motorista.CodigoMobile, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.Mobile, hub);
                MSMQ.MSMQ.SendPrivateMessage(notification);
            }
        }

        #endregion
    }
}
