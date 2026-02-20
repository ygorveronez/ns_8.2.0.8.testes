using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Notificacao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Embarcador.Notificacao
{
    /// <summary>
    /// Essa classe cuida apenas do envio das notificações para o OneSignal. Para ver a regra de negócio de envio, se refira para a classe NotificacaoMTrack.
    /// </summary>
    public class NotificacaoOneSignal
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork unitOfWork;
        private string appId;
        private string apiKey;

        #endregion

        #region Construtores

        public NotificacaoOneSignal(Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EnfilerarNotificacao(List<Dominio.Entidades.Usuario> motoristas, MobileHubs tipo, OneSignalHeadings headings, OneSignalContents contents, OneSignalData data)
        {
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                EnfilerarNotificacao(motorista, tipo, headings, contents, data);
            }
        }

        public void EnfilerarNotificacao(Dominio.Entidades.Usuario motorista, MobileHubs tipo, OneSignalHeadings headings, OneSignalContents contents, OneSignalData data)
        {
            string conexaoAdmin = string.Empty;
#if DEBUG
            conexaoAdmin = System.Configuration.ConfigurationManager.ConnectionStrings["AdminMultisoftware"]?.ToString() ?? string.Empty;
#else
            conexaoAdmin = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
#endif

            if (string.IsNullOrEmpty(conexaoAdmin))
                return;

            var conteudo = new
            {
                stringConexao = unitOfWork.StringConexao,
                stringAdminConexao = conexaoAdmin,
                codigoMotorista = motorista.Codigo,
                headings = headings,
                contents = contents,
                data = data,
                tipo = tipo,
            };

            string hub = SignalR.Mobile.GetHub(tipo);
            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(conteudo, 0, motorista.CodigoMobile, Dominio.MSMQ.MSMQQueue.SGTMobile, Dominio.SignalR.Hubs.NotificacaoOneSignal, hub);
            MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        /// <summary>
        /// Esse método deve ser chamado exclusivamente pela fila de mensagens.
        /// </summary>
        public void EnviarNotificacaoSync(Dominio.Entidades.Usuario motorista, MobileHubs tipo, OneSignalHeadings headings, OneSignalContents contents, OneSignalData data, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork)
        {
            if (adminUnitOfWork != null)
            {
                AdminMultisoftware.Repositorio.Mobile.ConfiguracaoMobile repConfiguracaoMobile = new AdminMultisoftware.Repositorio.Mobile.ConfiguracaoMobile(adminUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Mobile.ConfiguracaoMobile configMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();
                this.appId = configMobile.OneSignalAppId;
                this.apiKey = configMobile.OneSignalApiKey;
            }

            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repositorioUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(adminUnitOfWork);
            AdminMultisoftware.Repositorio.Mobile.NotificacaoOneSignal repNotificacaoOneSignal = new AdminMultisoftware.Repositorio.Mobile.NotificacaoOneSignal(adminUnitOfWork);

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repositorioUsuarioMobile.BuscarPorCFP(motorista.CPF);
            if (usuarioMobile == null)
                throw new ServicoException($"Usuário não encontrado para o motorista {motorista.Nome}");

            if (string.IsNullOrEmpty(usuarioMobile.OneSignalPlayerId))
                throw new ServicoException($"Motorista {motorista.Nome} não tem um OneSignalPlayerId registrado");

            data.Tipo = tipo;
            DateTime now = DateTime.Now;
            data.DataCriacao = now.ToUnixSeconds();

            string idOneSignal = CriarNotificacaoOneSignal(
                new List<string> { usuarioMobile.OneSignalPlayerId },
                headings,
                contents,
                data
            );

            if (idOneSignal != null)
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                repNotificacaoOneSignal.Inserir(new AdminMultisoftware.Dominio.Entidades.Mobile.NotificacaoOneSignal
                {
                    Motorista = usuarioMobile,
                    Tipo = tipo,
                    Headings = JsonConvert.SerializeObject(headings, jsonSettings),
                    Contents = JsonConvert.SerializeObject(contents, jsonSettings),
                    Data = JsonConvert.SerializeObject(data, jsonSettings),
                    DataCriacao = now,
                    IdOneSignal = idOneSignal
                });
            }

        }

        #endregion

        #region Métodos privados

        private string CriarNotificacaoOneSignal(List<string> playerIds, OneSignalHeadings headings, OneSignalContents contents, OneSignalData data)
        {
            string url = "https://onesignal.com/api/v1/notifications";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(NotificacaoOneSignal));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {this.apiKey}");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string body = JsonConvert.SerializeObject(new
            {
                app_id = this.appId,
                include_player_ids = playerIds,
                headings = headings,
                contents = contents,
                data = data
            },
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(body);
            ByteArrayContent byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage respostaServer = client.PostAsync(url, byteContent).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string response = respostaServer.Content.ReadAsStringAsync().Result;
                //Servicos.Log.TratarErro(response, "AndroidID");
                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(response);
                return retorno.id;
            }

            return null;
        }

        #endregion
    }
}
