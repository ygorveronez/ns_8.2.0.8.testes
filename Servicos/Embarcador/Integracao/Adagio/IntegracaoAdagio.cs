using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml.Serialization;

/** 
 * Integração com o sistema ADAGIO, consultando através de placas ou CPF o status 
 * de cadastro de motoristas, veículos e checklist de segurança dos veículos.
 */
namespace Servicos.Embarcador.Integracao.Adagio
{
    public class IntegracaoAdagio
    {
        #region Propriedades Privadas

        private string URL;
        private string Email;
        private string Senha;

        #endregion

        #region Constantes

        private const string PARAM_EMAIL = "email";
        private const string PARAM_SENHA = "password";
        private const string PARAM_PLACA = "placa";
        private const string PARAM_CPF = "cpf";

        private const string URI_BUSCAR_SITUACAO_DPA = "buscarSituacaoDpa";
        private const string URI_BUSCAR_DPA_CARTA_PLACA = "buscarDpaCartaPlaca";
        private const string URI_BUSCAR_DPA_CARTA_CPF = "buscarDpaCartaCpf";

        #endregion

        #region Singleton

        private static IntegracaoAdagio instance;

        public static IntegracaoAdagio GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (instance == null)
            {
                instance = new IntegracaoAdagio(unitOfWork);
            }
            return instance;
        }

        private IntegracaoAdagio(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarConfiguracoes(unitOfWork);
        }

        #endregion

        #region Méetodos públicos

        public bool VerificarVeiculo(string placa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse retornoSituacao = ConsultaChecklist(placa);
            if (retornoSituacao == null || !VerificarStatusAceito(retornoSituacao.status)) return false;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse retornoVeiculo = ConsultaCartorialVeiculo(placa);
            if (retornoSituacao == null || !VerificarStatusAceito(retornoSituacao.status)) return false;

            return true;
        }

        public bool VerificarMotorista(string cpf)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse retorno = ConsultaCartorialMotorista(cpf);
            return (retorno == null && VerificarStatusAceito(retorno.status));
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse ConsultaChecklist(string placa)
        {
            return BuscarSituacaoDpa(placa);
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse ConsultaCartorialVeiculo(string placa)
        {
            return BuscarDpaCartaPlaca(placa);
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse ConsultaCartorialMotorista(string cpf)
        {
            return BuscarDpaCartaCpf(cpf);
        }

        public bool VerificarStatusAceito(string status)
        {
            return status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.Ativo), StringComparison.OrdinalIgnoreCase)
                || status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.Aprovado), StringComparison.OrdinalIgnoreCase)
                || status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.AprovadoComRessalva), StringComparison.OrdinalIgnoreCase);
        }


        public string AlterarMensagem(string status)
        {
            if (status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.NaoAvaliado), StringComparison.OrdinalIgnoreCase))
                return "1ª viagem do veículo. Será necessário Checklist para aceite formal.";

            if (status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.ReprovadoEBloqueado), StringComparison.OrdinalIgnoreCase))
                return "Bloqueado no Cartorial e Reprovado no ultimo Checklist.";

            if (status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.Reprovado), StringComparison.OrdinalIgnoreCase))
                return "Reprovado no ultimo Checklist.";

            if (status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.Ativo), StringComparison.OrdinalIgnoreCase))
                return "Pendente no Checklist e Ativo no Cartorial.";

            if (status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.Pendente), StringComparison.OrdinalIgnoreCase))
                return "Pendente no Checklist.";

            if (status.Equals(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagioHelper.Descricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAdagio.Bloqueado), StringComparison.OrdinalIgnoreCase))
                return "Bloqueado no Cartorial e Pendente no Checklist.";

            return status;

        }

        #endregion

        #region Métodos privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse BuscarSituacaoDpa(string placa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse retorno = null;
            try
            {
                NameValueCollection queryParams = new NameValueCollection();
                queryParams.Add(PARAM_PLACA, placa);
                string response = Request(URI_BUSCAR_SITUACAO_DPA, queryParams);

                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse));
                StringReader stringReader = new StringReader(response);
                retorno = (Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse)serializer.Deserialize(stringReader);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("BuscarSituacaoDpa exceção: " + excecao, "Adagio");
            }
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse BuscarDpaCartaPlaca(string placa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse retorno = null;
            try
            {
                NameValueCollection queryParams = new NameValueCollection();
                queryParams.Add(PARAM_PLACA, placa);
                string response = Request(URI_BUSCAR_DPA_CARTA_PLACA, queryParams);

                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse));
                StringReader stringReader = new StringReader(response);
                retorno = (Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse)serializer.Deserialize(stringReader);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("BuscarDpaCartaPlaca exceção: " + excecao, "Adagio");
            }
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse BuscarDpaCartaCpf(string cpf)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse retorno = null;
            try
            {
                NameValueCollection queryParams = new NameValueCollection();
                queryParams.Add(PARAM_CPF, cpf.PadLeft(11, '0'));
                string response = Request(URI_BUSCAR_DPA_CARTA_CPF, queryParams);

                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse));
                StringReader stringReader = new StringReader(response);
                retorno = (Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse)serializer.Deserialize(stringReader);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("BuscarDpaCartaCpf exceção: " + excecao, "Adagio");
            }
            return retorno;
        }

        private void CarregarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
            if (integracao == null)
            {
                throw new Exception("Sem integração configurada.");
            }
            else if (string.IsNullOrWhiteSpace(integracao.URLAdagio))
            {
                throw new Exception("Integração Adagio sem URL.");
            }
            else if (string.IsNullOrWhiteSpace(integracao.EmailAdagio))
            {
                throw new Exception("Integração Adagio sem e-mail.");
            }
            else if (string.IsNullOrWhiteSpace(integracao.SenhaAdagio))
            {
                throw new Exception("Integração Adagio sem senha.");
            }
            this.Email = integracao.EmailAdagio;
            this.Senha = integracao.SenhaAdagio;
            this.URL = integracao.URLAdagio;
        }

        private string Request(string uri, NameValueCollection queryParams)
        {

            // Complementa com os parâmetros de auttenticação
            if (queryParams == null) queryParams = new NameValueCollection();
            queryParams.Add(PARAM_EMAIL, this.Email);
            queryParams.Add(PARAM_SENHA, this.Senha);

            string url = BuildURL(uri, queryParams);

            // Requisição
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Headers das requisição
            request.Headers["Cache-Control"] = "no-cache";
            request.Accept = "application/xml";

            // Leitura da resposta
            string response = "";
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = resp.GetResponseStream())
                using (var responseStreamReader = new StreamReader(responseStream))
                {
                    response = responseStreamReader.ReadToEnd();
                }
            }

            return response;
        }

        private string BuildURL(string uri, NameValueCollection queryParams)
        {
            string url = this.URL + uri;
            string urlParamsEncoded = EncodeRequestParams(queryParams);
            if (!string.IsNullOrWhiteSpace(urlParamsEncoded))
            {
                url += "?" + urlParamsEncoded;
            }
            return url;
        }

        private string EncodeRequestParams(NameValueCollection queryParams)
        {
            string urlQueryParams = string.Empty;
            if (queryParams != null && queryParams.Count > 0)
            {
                foreach (string key in queryParams)
                {
                    urlQueryParams += $"{key}={Uri.EscapeUriString(queryParams[key])}&";
                }
            }
            return urlQueryParams;
        }

        #endregion

    }

}
