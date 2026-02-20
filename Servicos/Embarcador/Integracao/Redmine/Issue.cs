using System;
using System.IO;
using System.Net;

namespace Servicos.Embarcador.Integracao.Redmine
{
    public static class Issue
    {

        public static string BuscarConteudoTarefaPorId(string issue, AdminMultisoftware.Repositorio.UnitOfWork unitOfWork)
        {
            AdminMultisoftware.Repositorio.Configuracoes.RedmineConfig repRedmineConfig = new AdminMultisoftware.Repositorio.Configuracoes.RedmineConfig(unitOfWork);
            AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig redmineConfig = repRedmineConfig.BuscarConfigPadrao();

            if (redmineConfig == null)
                return "";

            string apiKey = redmineConfig.APIkey;
            string url = $"{redmineConfig.URLAPI}/issues/{issue}.json?key={apiKey}&v={Guid.NewGuid()}";

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.Host = "dev666.cloudapp.net";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Dispose();
            response.Close();

            return responseString;
        }
    }
}
