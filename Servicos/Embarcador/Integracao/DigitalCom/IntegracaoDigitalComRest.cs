using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.DigitalCom
{
    public sealed class IntegracaoDigitalComRest : IIntegracaoDigitalCom
    {
        #region Atributos

        public readonly Repositorio.UnitOfWork _unitOfWork;

        public readonly ConfiguracaoVeiculo _configuracaoVeiculo;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom _configuracaoIntegracaoDigitalCom;

        #endregion

        #region Construtores

        public IntegracaoDigitalComRest(Repositorio.UnitOfWork unitOfWork, ConfiguracaoVeiculo configuracaoVeiculo)
        {
            _unitOfWork = unitOfWork;
            _configuracaoVeiculo = configuracaoVeiculo;

            ObterConfiguracaoIntegracao();
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            ArquivoTransacao<CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                VerificarConfiguracaoIntegracao();

                ObterToken();

                HttpClient client = ObterClient(_configuracaoIntegracaoDigitalCom.EndpointDigitalCom);

                jsonRequest = ObterUrlIntegracao(cargaDadosTransporteIntegracao);

                HttpResponseMessage retornoRequisicao = client.GetAsync(jsonRequest).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if(retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoErro retornoErro = jsonRetorno.FromJson<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoErro>();
                    
                    throw new ServicoException(retornoErro.Mensagem);
                }

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoIntegracao> retornos = jsonRetorno.FromJson<List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoIntegracao>>();

                if (retornos.Count == 0)
                    throw new ServicoException("O WS DigitalCom não respondeu à solicitação");

                cargaDadosTransporteIntegracao.Carga.TAGPedagio = TAGPedagio.Invalida;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                string mensagemRetornoSucesso = string.Empty;

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoIntegracao retornoMeioPagamento in retornos)
                {
                    if (retornoMeioPagamento.Status.Codigo == 214)
                    {
                        cargaDadosTransporteIntegracao.Carga.TAGPedagio = TAGPedagio.Valida;
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        mensagemRetornoSucesso = $"{retornoMeioPagamento.MeioPagamento} - {retornoMeioPagamento.Status.Descricao}";
                    }

                    cargaDadosTransporteIntegracao.ProblemaIntegracao = retornoMeioPagamento.Status.Descricao;
                    MeiosPagamentoDigitalCom? meioPagamentoDigitalCom = retornoMeioPagamento.MeioPagamento.ToNullableEnum<MeiosPagamentoDigitalCom>();

                    string jsonRetornoParcial = retornoMeioPagamento.ToJson();

                    CargaCTeIntegracaoArquivo arquivoRequisicao = servicoArquivoTransacao.Adicionar(jsonRequest, jsonRetornoParcial, "json", cargaDadosTransporteIntegracao);

                    GerarIntegracaoDigitalComArquivosTransacao(cargaDadosTransporteIntegracao, arquivoRequisicao, meioPagamentoDigitalCom, retornoMeioPagamento.Status.Codigo);
                }

                if (cargaDadosTransporteIntegracao.Carga.TAGPedagio == TAGPedagio.Valida)
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemRetornoSucesso;

                repositorioCarga.Atualizar(cargaDadosTransporteIntegracao.Carga);

            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a DigitalCom";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonRetorno, "json");

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public bool PermitirGerarIntegracao()
        {
            return _configuracaoVeiculo?.ValidarTAGDigitalCom ?? false;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient ObterClient(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDigitalComRest));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuracaoIntegracaoDigitalCom.TokenDigitalCom);

            return requisicao;
        }

        private void ObterToken()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao repToken = new Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao tipoIntegracaoAutenticacao = repToken.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom);

            if (!string.IsNullOrWhiteSpace(tipoIntegracaoAutenticacao?.Token) && tipoIntegracaoAutenticacao.DataExpiracao > DateTime.Now)
            {
                _configuracaoIntegracaoDigitalCom.TokenDigitalCom = tipoIntegracaoAutenticacao.Token;
                return;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string urlAutenticacao = !string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.UrlAutenticacaoDigitalCom) ? _configuracaoIntegracaoDigitalCom.UrlAutenticacaoDigitalCom : "https://apigateway.digitalcomm.com.br:8443/auth/oauth/v2/token";
            RestClient client = new RestClient(urlAutenticacao);
            RestRequest request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _configuracaoIntegracaoDigitalCom.UsuarioDigitalCom);
            request.AddParameter("client_secret", _configuracaoIntegracaoDigitalCom.SenhaDigitalCom);
            request.AddParameter("scope", "dclogg-internal");
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoToken retorno = response.Content.FromJson<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte.RetornoToken>();

            if (string.IsNullOrWhiteSpace(retorno.Token))
                throw new ServicoException("Não foi possível obter o Token");

            if (tipoIntegracaoAutenticacao == null)
                tipoIntegracaoAutenticacao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao();

            tipoIntegracaoAutenticacao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom;
            tipoIntegracaoAutenticacao.Token = retorno.Token;
            tipoIntegracaoAutenticacao.DataExpiracao = DateTime.Now.AddSeconds(retorno.ExpiresIn - 60);

            if (tipoIntegracaoAutenticacao.Codigo > 0)
                repToken.Atualizar(tipoIntegracaoAutenticacao);
            else
                repToken.Inserir(tipoIntegracaoAutenticacao);

            _configuracaoIntegracaoDigitalCom.TokenDigitalCom = retorno.Token;
        }

        private string ObterUrlIntegracao(CargaDadosTransporteIntegracao cargaIntegracao)
        {
            string urlBase = $"{_configuracaoIntegracaoDigitalCom.EndpointDigitalCom}veiculo/meioPagamento";

            if (cargaIntegracao.Carga.Veiculo != null)
                return $"{urlBase}/{cargaIntegracao.Carga.Veiculo.Placa}/{_configuracaoIntegracaoDigitalCom.CNPJLogin}";

            throw new ServicoException("Veiculo não encontrado.");
        }

        private void GerarIntegracaoDigitalComArquivosTransacao(CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, CargaCTeIntegracaoArquivo arquivoRequisicao, MeiosPagamentoDigitalCom? meioPagamentoDigitalCom, int id)
        {
            Repositorio.Embarcador.Cargas.IntegracaoDigitalComArquivosTransacao repositorioDigitalComArquivos = new Repositorio.Embarcador.Cargas.IntegracaoDigitalComArquivosTransacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao integracaoDigitalComArquivos = new Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao
            {
                Carga = cargaDadosTransporteIntegracao.Carga,
                CargaCTeIntegracaoArquivo = arquivoRequisicao,
                MeioPagamentoDigitalCom = meioPagamentoDigitalCom,
                IDRetornoDigitalCom = id
            };

            repositorioDigitalComArquivos.Inserir(integracaoDigitalComArquivos);
        }

        private void ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repositorioConfiguracaoDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(_unitOfWork);

            _configuracaoIntegracaoDigitalCom ??= repositorioConfiguracaoDigitalCom.Buscar();
        }

        private void VerificarConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracaoDigitalCom == null)
                throw new ServicoException("Não existe configuração de integração disponível para a DigitalCom");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.TokenDigitalCom))
                throw new ServicoException("O Token deve estar preenchido na configuração de integração da DigitalCom");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.EndpointDigitalCom))
                throw new ServicoException("Não existe URL de integração configurada para DigitalCom");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.CNPJLogin))
                throw new ServicoException("O CNPJ de Login deve ser informado na configuração da integração da DigitalCom");
        }

        #endregion Métodos Privados
    }
}
