using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;

namespace Servicos.Embarcador.HubsMobile
{
    public sealed class NotificacaoMobile : HubBaseMobile<NotificacaoMobile>
    {
        #region Métodos Privados

        private void NotificarFilaCarregamentoMobile(string chaveUsuario, string notificacaoEnviar)
        {
            FilaCarregamentoMobile servicoFilaCarregamentoMobile = new FilaCarregamentoMobile();

            servicoFilaCarregamentoMobile.Notificar(chaveUsuario, notificacaoEnviar);
        }

        private void SalvarNotificacao(Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados notificacao, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                bool notificacaoEnviada = IsConexaoAtiva(notificacao.Usuario.CPF);

                Repositorio.Embarcador.Notificacoes.NotificacaoMobile repositorioNotificacao = new Repositorio.Embarcador.Notificacoes.NotificacaoMobile(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile notificacaoMobile = new Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile()
                {
                    Assunto = notificacao.Assunto,
                    CentroCarregamento = notificacao.CentroCarregamento,
                    Data = DateTime.Now,
                    Mensagem = notificacao.Mensagem,
                    TipoLancamento = (notificacao.Tipo == TipoNotificacaoMobile.Mensagem) ? TipoLancamentoNotificacaoMobile.Manual : TipoLancamentoNotificacaoMobile.Automatico
                };

                repositorioNotificacao.Inserir(notificacaoMobile);

                Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorioNotificacaoUsuario = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario notificacaoMobileUsuario = new Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario()
                {
                    Usuario = notificacao.Usuario,
                    Enviada = notificacaoEnviada,
                    Notificacao = notificacaoMobile
                };

                if (notificacaoEnviada && (notificacao.Tipo != TipoNotificacaoMobile.Mensagem))
                    notificacaoMobileUsuario.DataLeitura = notificacaoMobile.Data;

                repositorioNotificacaoUsuario.Inserir(notificacaoMobileUsuario);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        #endregion

        #region Métodos Públicos

        public void Notificar(Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados dadosNotificacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (dadosNotificacao.Usuario != null)
            {
                string notificacaoEnviar = JsonConvert.SerializeObject(new { dadosNotificacao.Mensagem, dadosNotificacao.Tipo });
                string notificacaoEnviarFilaCarregamentoMobile = JsonConvert.SerializeObject(new { dadosNotificacao.Mensagem, TipoAlteracao = dadosNotificacao.Tipo });

                SendToClient(dadosNotificacao.Usuario.CPF, "notificar", notificacaoEnviar);
                NotificarFilaCarregamentoMobile(dadosNotificacao.Usuario.CPF, notificacaoEnviarFilaCarregamentoMobile);
                SalvarNotificacao(dadosNotificacao, unitOfWork);
            }
        }

        public bool NotificarMensagemAdicionada(string chaveUsuario)
        {
            string notificacaoEnviar = JsonConvert.SerializeObject(new { Mensagem = "Você recebeu uma nova mensagem", Tipo = TipoNotificacaoMobile.Mensagem });
            string notificacaoEnviarFilaCarregamentoMobile = JsonConvert.SerializeObject(new { Mensagem = "Você recebeu uma nova mensagem", TipoAlteracao = TipoNotificacaoMobile.Mensagem });

            SendToClient(chaveUsuario, "notificar", notificacaoEnviar);
            NotificarFilaCarregamentoMobile(chaveUsuario, notificacaoEnviarFilaCarregamentoMobile);

            return IsConexaoAtiva(chaveUsuario);
        }

        #endregion
    }
}
