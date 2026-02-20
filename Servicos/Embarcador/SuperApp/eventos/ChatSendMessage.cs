using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class ChatSendMessage : IntegracaoSuperApp
    {
        public ChatSendMessage(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, out RetornoIntegracaoSuperApp retornoIntegracaoSuperApp)
        {
            retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                Servicos.Log.TratarErro("Inicio Chat Message", "IntegracaoSuperAPPOutrosTipos");

                string jsonRequisicao = integracaoSuperApp.ArquivoRequisicao != null ? obterJsonRequisicao(integracaoSuperApp.ArquivoRequisicao) : integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                EventoChatSendMessage eventoChatSendMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoChatSendMessage>(jsonRequisicao);
                if (eventoChatSendMessage == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repMensagemChat = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);


                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorIDIdentificacaoTrizy(eventoChatSendMessage.Data.Travel._id);

                if (carga == null)
                    throw new ServicoException($"Carga não foi encontrada.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                AtualizarCargaIntegracaoSuperApp(integracaoSuperApp, carga, null);

                if (string.IsNullOrWhiteSpace(eventoChatSendMessage.Data.Message))
                    throw new ServicoException($"Chat sem texto", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem mensagemChat = new Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem();

                mensagemChat.Carga = carga;
                mensagemChat.Mensagem = eventoChatSendMessage.Data.Message;
                mensagemChat.DataCriacao = DateTime.Now;
                mensagemChat.DataConfirmacaoLeitura = DateTime.Now;
                mensagemChat.Remetente = ObterUsuario(eventoChatSendMessage.Data.Driver.Document.Value);

                repMensagemChat.Inserir(mensagemChat);

                // Auditoria
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceSuperApp;
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Mensagem recebida: " + mensagemChat.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"), _unitOfWork);

                Servicos.Log.TratarErro("Fim Chat Message", "IntegracaoSuperAPPOutrosTipos");

                retornoIntegracaoSuperApp.Sucesso = true;

                Servicos.Embarcador.Hubs.Chat hubChat = new Servicos.Embarcador.Hubs.Chat();
                hubChat.NotificarMensagemUsuario(0, mensagemChat.Remetente.Codigo, mensagemChat.Mensagem, mensagemChat.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"), mensagemChat.Remetente.Nome, true);
            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro($"ServicoException (RegistroIgnorado) Processar ChatSendMessage - Integração: {integracaoSuperApp.Codigo} - Erro: {ex.Message}", "IntegracaoSuperAPPOutrosTipos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro($"ServicoException Processar ChatSendMessage - Integração: {integracaoSuperApp.Codigo} - Erro: {ex.Message}", "IntegracaoSuperAPPOutrosTipos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar " + TipoEventoApp.ChatSendMessage.ObterDescricao();
                Servicos.Log.TratarErro($"Exception Processar ChatSendMessage - Integração: {integracaoSuperApp.Codigo} - Erro: {ex.Message}", "IntegracaoSuperAPPOutrosTipos");
            }
        }
    }
}
