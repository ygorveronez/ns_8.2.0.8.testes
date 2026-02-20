using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Admin.Integracao.Redmine
{
    public static class Users
    {
        public static Dominio.ObjetosDeValor.Integracao.Redmine.User AutenticarUsuario(string login, string senha, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Configuracoes.RedmineConfig repRedmineConfig = new AdminMultisoftware.Repositorio.Configuracoes.RedmineConfig(unitOfWork);
            AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig redmineConfig = repRedmineConfig.BuscarConfigPadrao();

            if (redmineConfig == null)
                return null;

            Dominio.ObjetosDeValor.Integracao.Redmine.User user = null;

            string Uri = $"{ redmineConfig.URLAPI }/users/current.json?v={Guid.NewGuid()}";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(login+':'+senha)));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            client.Timeout = TimeSpan.FromMilliseconds(60000);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Integracao.Redmine.ResponseUser responseUser = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Integracao.Redmine.ResponseUser>(body);
                user = responseUser?.user;
                if ((user?.id ?? 0) > 0)
                    user = ObterDetalhesDoGrupo(user.id, redmineConfig);
                else
                    user = null;

                respostaServer.Dispose();
                client.Dispose();
            }

            return user;
        }

        private static Dominio.ObjetosDeValor.Integracao.Redmine.User ObterDetalhesDoGrupo(int idUsuario, AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig redmineConfig)
        {
            Dominio.ObjetosDeValor.Integracao.Redmine.User user = null;

            string apiKey = redmineConfig.APIkey;
            string Uri = $"{redmineConfig.URLAPI}/users/{idUsuario}.json?key={apiKey}&include=groups&v={Guid.NewGuid()}";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var respostaServer = client.GetAsync(Uri).Result;

            if (respostaServer.IsSuccessStatusCode)
            {
                string body = respostaServer.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Integracao.Redmine.ResponseUser responseUser = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Integracao.Redmine.ResponseUser>(body);
                user = responseUser?.user;
                respostaServer.Dispose();
                client.Dispose();
            }

            return user;
        }
    }
}
