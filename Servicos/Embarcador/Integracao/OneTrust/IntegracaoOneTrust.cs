using Dominio.Entidades.Embarcador.Configuracao;
using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.OneTrust
{
    public class IntegracaoOneTrust
    {
        private readonly ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public IntegracaoOneTrust(ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public void VerificarSituacaoMotorista(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            if (!repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Onetrust))
                return;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.BuscarPrimeiroRegistro();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLObterTokenOnetrust) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLOnetrust))
                return;

            try
            {
                string token = ObterToken(configuracaoIntegracao.URLObterTokenOnetrust, configuracaoIntegracao.ClientIdOneTrust, configuracaoIntegracao.ClientSecretOneTrust);

                if (string.IsNullOrWhiteSpace(token))
                    throw new ServicoException("Não foi possivel gerar o token.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.MotoristaInvalidoOneTrust);

                bool? motoristaAtivo = EnviarRequisicao(configuracaoIntegracao.URLOnetrust, token, configuracaoIntegracao.PurposeIdOneTrust, motorista);

                if (!motoristaAtivo.HasValue || !motoristaAtivo.Value)
                    throw new ServicoException("O motorista não está de acordo com os termos da LGPD.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.MotoristaInvalidoOneTrust);
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao.Message, "IntegracaoOneTrust");
                throw;
            }
            catch (Exception e)
            {
                Log.TratarErro(e.Message, "IntegracaoOneTrust");
                throw;
            }
        }

        #endregion

        #region Métodos Privados

        private bool? EnviarRequisicao(string url, string token, string purposeId, Dominio.Entidades.Usuario motorista)
        {
           
            if (url.Last() == '/')
                url = url.Remove(url.Length - 1);

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", $"Bearer {token}");
            request.AddHeader("identifier", motorista.CPF_CNPJ_Formatado.ToString());
            request.AddParameter("purposeGuid", purposeId);

            IRestResponse response = client.Execute(request);

            dynamic dynConteudo = JsonConvert.DeserializeObject<dynamic>(response.Content);

            foreach (dynamic dynResultado in dynConteudo.content)
            {
                dynamic purposes = dynResultado.Purposes;

                foreach (dynamic purpose in purposes)
                    return ((string)purpose.Status).ToString() == "ACTIVE";
            }

            return false;
        }

        private string ObterToken(string urlObterToken, string clientId, string clientSecret)
        {
            
            if (urlObterToken.Last() == '/')
                urlObterToken = urlObterToken.Remove(urlObterToken.Length - 1);

            var client = new RestClient(urlObterToken);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);


            IRestResponse response = client.Execute(request);

            dynamic dynConteudo = JsonConvert.DeserializeObject<dynamic>(response.Content);

            if (dynConteudo.access_token == null)
                throw new ServicoException("Ocorreu um erro ao realizar a autenticação com a OneTrust, favor verificar os dados.");

            return ((string)dynConteudo.access_token).ToString();
        }

        #endregion

    }
}
