using Repositorio;
using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.Integracao.Dansales
{
    /// <summary>
    /// Se existe integração com a Dansales, toda mensagem que é enviada para o chat entre o ME e o Aplicativo 
    /// deve também ser enviada para a Dansales. Essa classe cuida só disso.
    /// </summary>
    public class ChatIntegracaoDansales
    {
        private readonly UnitOfWork unitOfWork;

        public ChatIntegracaoDansales(Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void CriarEntidadeTemporariaChatMensagemDansales(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem)
        {
            var repIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal repChatDansalesCargaNotaFiscal = new Repositorio.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal(unitOfWork);

            var integracaoDansales = repIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dansales);

            if (integracaoDansales == null)
            {
                return;
            }

            Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal chatDansalesCarganota = new Dominio.Entidades.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal();
            chatDansalesCarganota.Carga = carga;
            chatDansalesCarganota.DataCriacao = DateTime.Now;
            chatDansalesCarganota.chatMobileMensagem = chatMobileMensagem;
            chatDansalesCarganota.XMLNotaFiscal = xmlNotaFiscal;

            repChatDansalesCargaNotaFiscal.Inserir(chatDansalesCarganota);
        }

        public void EnviarMensagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario remetente, Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem)
        {
            var repIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            var integracaoDansales = repIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dansales);

            if (integracaoDansales == null || carga == null)
            {
                return;
            }

            EnviarMensagemVendedor(remetente, chatMobileMensagem, carga);
        }


        public bool ReenviarMensagemIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario remetente, Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem)
        {
            var repIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            var integracaoDansales = repIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dansales);

            if (integracaoDansales == null)
            {
                return false;
            }

            // faz a integração para mandar a mensagem para a dansales

            Repositorio.Embarcador.Configuracoes.IntegracaoDansales repositorioConfiguracaoIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unitOfWork);
            Repositorio.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal repChatDansalesCargaNotaFiscal = new Repositorio.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales configuracaoIntegracaoDansales = repositorioConfiguracaoIntegracaoDansales.Buscar();

            if ((configuracaoIntegracaoDansales == null) || string.IsNullOrWhiteSpace(configuracaoIntegracaoDansales.URLIntegracaoChat))
            {
                throw new ServicoException("Não existe configuração de integração de chat disponível para a DANSALES.");
            }

            var chatsAIntegrar = repChatDansalesCargaNotaFiscal.BuscarPorMensagemChat(chatMobileMensagem.Codigo);

            if (chatsAIntegrar.Count == 0)
            {
                return false;
            }

            string user = configuracaoIntegracaoDansales.Usuario;
            string senha = configuracaoIntegracaoDansales.Senha;
            string urlToken = configuracaoIntegracaoDansales.URLToken;
            string endPoint = configuracaoIntegracaoDansales.URLIntegracaoChat;
            //string endPoint = "https://dansaleswebdev.danone.com.br/homologApp/api/OrderTracking/message";

            string userToken = configuracaoIntegracaoDansales.UsuarioToken;
            string senhaToken = configuracaoIntegracaoDansales.SenhaToken;

            string tokenID = Servicos.Embarcador.Integracao.Dansales.IntegracaoDansales.ObterToken(urlToken, user, senha, userToken, senhaToken);

            if (string.IsNullOrWhiteSpace(tokenID)) throw new ServicoException("DANSALES não retornou Token.");

            try
            {
                bool tudoCerto = true;

                foreach (var chat in chatsAIntegrar)
                {
                    HttpClient client = Servicos.Embarcador.Integracao.Dansales.IntegracaoDansales.ObterClienteRequisicao(endPoint, tokenID, userToken, senhaToken);

                    var dadosRequisicao = new
                    {
                        NumNF = chat.XMLNotaFiscal.Numero,
                        Serie = chat.XMLNotaFiscal.Serie,
                        CnpjEmitente = chat.XMLNotaFiscal.Emitente.CPF_CNPJ,
                        TipoUsuario = remetente.Tipo == "M" ? 1 : 2, // 1 se for motorista, 2 se for operador
                        NomeUsuario = remetente.Nome,
                        DataMensagem = DateTime.Now,
                        Mensagem = chatMobileMensagem.Mensagem,
                    };

                    string jsonRequestBody = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);

                    // Request
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endPoint, content).Result;

                    // Response
                    var response = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Deu boa
                        chatMobileMensagem.MensagemLida = true;
                        chatMobileMensagem.MensagemFalhaIntegracao = false;
                        repChatDansalesCargaNotaFiscal.Deletar(chat);
                    }
                    else
                    {
                        Log.TratarErro($"Erro ao enviar mensagem no chat da Dansales: {response}");
                        chatMobileMensagem.MensagemFalhaIntegracao = true;
                        tudoCerto = false;
                    }
                }

                return tudoCerto;

            }
            catch (Exception e)
            {
                return false;
            }

        }

        private bool EnviarMensagemVendedor(Dominio.Entidades.Usuario remetente, Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            // faz a integração para mandar a mensagem para a dansales
            Repositorio.Embarcador.Configuracoes.IntegracaoDansales repositorioConfiguracaoIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unitOfWork);
            Repositorio.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal repChatDansalesCargaNotaFiscal = new Repositorio.Embarcador.Integracao.Dansales.ChatDansalesCargaNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales configuracaoIntegracaoDansales = repositorioConfiguracaoIntegracaoDansales.Buscar();

            if ((configuracaoIntegracaoDansales == null) || string.IsNullOrWhiteSpace(configuracaoIntegracaoDansales.URLIntegracaoChat))
            {
                throw new ServicoException("Não existe configuração de integração de chat disponível para a DANSALES.");
            }

            var chatsAIntegrar = repChatDansalesCargaNotaFiscal.BuscarPorCarga(carga.Codigo);

            if (chatsAIntegrar.Count == 0)
            {
                return true;
            }

            string user = configuracaoIntegracaoDansales.Usuario;
            string senha = configuracaoIntegracaoDansales.Senha;
            string urlToken = configuracaoIntegracaoDansales.URLToken;
            string endPoint = configuracaoIntegracaoDansales.URLIntegracaoChat;
            //string endPoint = "https://dansaleswebdev.danone.com.br/homologApp/api/OrderTracking/message";

            string userToken = configuracaoIntegracaoDansales.UsuarioToken;
            string senhaToken = configuracaoIntegracaoDansales.SenhaToken;

            string tokenID = Servicos.Embarcador.Integracao.Dansales.IntegracaoDansales.ObterToken(urlToken, user, senha, userToken, senhaToken);

            if (string.IsNullOrWhiteSpace(tokenID)) throw new ServicoException("DANSALES não retornou Token.");

            try
            {
                bool tudoCerto = true;

                foreach (var chat in chatsAIntegrar)
                {
                    HttpClient client = Servicos.Embarcador.Integracao.Dansales.IntegracaoDansales.ObterClienteRequisicao(endPoint, tokenID, userToken, senhaToken);

                    var dadosRequisicao = new
                    {
                        NumNF = chat.XMLNotaFiscal.Numero,
                        Serie = chat.XMLNotaFiscal.Serie,
                        CnpjEmitente = chat.XMLNotaFiscal.Emitente.CPF_CNPJ,
                        TipoUsuario = remetente.Tipo == "M" ? 1 : 2, // 1 se for motorista, 2 se for operador
                        NomeUsuario = remetente.Nome,
                        DataMensagem = DateTime.Now,
                        Mensagem = chatMobileMensagem.Mensagem,
                    };

                    string jsonRequestBody = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);

                    // Request
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endPoint, content).Result;

                    // Response
                    var response = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        // Deu boa
                        chatMobileMensagem.MensagemLida = true;
                        chatMobileMensagem.MensagemFalhaIntegracao = false;
                        repChatDansalesCargaNotaFiscal.Deletar(chat);
                    }
                    else
                    {
                        Log.TratarErro($"Erro ao enviar mensagem no chat da Dansales: {response}");
                        chatMobileMensagem.MensagemFalhaIntegracao = true;
                        tudoCerto = false;
                    }
                }

                return tudoCerto;

            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
