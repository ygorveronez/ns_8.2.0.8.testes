using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.Klios
{
    public sealed class IntegracaoKlios
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public IntegracaoKlios(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Publicos

        public void EnviarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> cargaJanelaCarregamentoIntegracoes = repJanelaCarregamentoIntegracao.BuscarIntegracoesPendentesDeEnvio();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao in cargaJanelaCarregamentoIntegracoes)
                IntegrarJanelaDeCarregamento(janelaCarregamentoIntegracao);
        }

        public void EnviarIntegracoesAguardandoRetorno()
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao> cargaJanelaCarregamentoIntegracoes = repJanelaCarregamentoIntegracao.BuscarIntegracoesAguardandoRetorno();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao in cargaJanelaCarregamentoIntegracoes)
            {
                if (janelaCarregamentoIntegracao.DataCriacao.Value.AddMinutes(5) > DateTime.Now)
                    return;

                ConsultarRetornoAnalise(janelaCarregamentoIntegracao);
            }
        }

        public void EnviarDocumentosReprovadosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao cargaJanelaCarregamentoIntegracao)
        {
            IntegrarArquivosConjunto(cargaJanelaCarregamentoIntegracao, ObterConfiguracaoIntegracao());

            AtualizarSituacaoJanelaCarregamanto(cargaJanelaCarregamentoIntegracao, SituacaoIntegracao.AgRetorno, RecomendacaoGR.AguardandoValidacaoGR, "Documentos Reenviados");
        }

        #endregion

        #region Métodos Privados

        private void IntegrarJanelaDeCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao serJanelaCarregamentoIntegracao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);
            string mensagemRetorno = string.Empty;
            string requestParamsEncoded = string.Empty;
            string response = string.Empty;
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

                HttpWebRequest request = CriarRequisicao($"{configuracaoIntegracao.UrlApi}/pesquisa", "POST", "application/x-www-form-urlencoded");
                request.Headers.Add("Authorization", $"Bearer {configuracaoIntegracao.Token}");

                requestParamsEncoded = PreencherParametrosPesquisa(janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga);

                byte[] sendData = Encoding.UTF8.GetBytes(requestParamsEncoded);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Flush();
                requestStream.Dispose();

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(request);
                response = ObterResposta(retornoRequisicao);
                dynamic result = JsonConvert.DeserializeObject<dynamic>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    mensagemRetorno = result.details;
                    if (mensagemRetorno == "Sem dados para esta pesquisa")
                        IntegrarAnaliseConjunto(janelaCarregamentoIntegracao, configuracaoIntegracao);
                    else
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
                }

                if (result.consulta != null)
                {
                    janelaCarregamentoIntegracao.Protocolo = result.consulta.id_KliosAnalise;

                    if (result.consulta.status_analise == "Expirado")
                    {
                        mensagemRetorno = "Cadastro Expirado";
                        IntegrarAnaliseConjunto(janelaCarregamentoIntegracao, configuracaoIntegracao);
                    }
                    else if (result.consulta.status_analise == "Concluído")
                    {
                        mensagemRetorno = result.consulta.resultado;
                        if (result.consulta.resultado == "Recomendado")
                            AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.Recomendado, mensagemRetorno);
                        else if (result.consulta.resultado == "Não recomendado")
                            AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.NaoRecomendado, mensagemRetorno);
                    }
                    else if (result.consulta.status_analise == "Em Análise")
                    {
                        mensagemRetorno = "Documentos em Análise";
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.AgRetorno, RecomendacaoGR.AguardandoValidacaoGR, mensagemRetorno);
                    }
                    else if (result.consulta.status_analise == "Cadastrado")
                    {
                        mensagemRetorno = "Dados Enviados - Pendente de Anexo";
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.PendenciaDocumentos, mensagemRetorno);
                    }
                    else if (result.consulta.status_analise == "Documento Reprovado")
                    {
                        mensagemRetorno = "Documento Reprovado";
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.PendenciaDocumentos, mensagemRetorno);
                    }
                    else
                    {
                        mensagemRetorno = result.consulta.status_analise;
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoKlios");
                mensagemRetorno = "Ocorreu uma falha ao integrar";
                AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
            }

            serJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, requestParamsEncoded, response, mensagemRetorno);
        }

        private void IntegrarAnaliseConjunto(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao serJanelaCarregamentoIntegracao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);
            string mensagemRetorno = string.Empty;
            string requestParamsEncoded = string.Empty;
            string response = string.Empty;
            try
            {
                HttpWebRequest request = CriarRequisicao($"{configuracaoIntegracao.UrlApi}/create", "POST", "application/x-www-form-urlencoded");
                request.Headers.Add("Authorization", $"Bearer {configuracaoIntegracao.Token}");

                requestParamsEncoded = PreencherParametrosCriacao(janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga);

                byte[] sendData = Encoding.UTF8.GetBytes(requestParamsEncoded);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Flush();
                requestStream.Dispose();

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(request);
                response = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    throw new ServicoException("Ocorreu uma falha ao tentar Criar Cadastro de Conjunto");
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ResponseCreate result = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ResponseCreate>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    janelaCarregamentoIntegracao.Protocolo = result.id_KliosAnalise;
                    janelaCarregamentoIntegracao.NovaAnalise = true;

                    mensagemRetorno = "Enviado cadastro do Conjunto";
                    AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.AgRetorno, RecomendacaoGR.AguardandoValidacaoGR, mensagemRetorno);

                    IntegrarArquivosConjunto(janelaCarregamentoIntegracao, configuracaoIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                mensagemRetorno = excecao.Message;
                AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoKlios");
                mensagemRetorno = "Ocorreu uma falha ao Criar Cadastro de Conjunto";
                AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
            }

            serJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, requestParamsEncoded, response, mensagemRetorno);
        }

        private void IntegrarArquivosConjunto(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao serJanelaCarregamentoIntegracao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc> requestSendDocs = ObterRequestSendDoc(janelaCarregamentoIntegracao.CargaJanelaCarregamento.Carga, janelaCarregamentoIntegracao.Protocolo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc requestSendDoc in requestSendDocs)
            {
                string mensagemRetorno = string.Empty;
                string jsonRequest = string.Empty;
                string response = string.Empty;
                try
                {
                    HttpWebRequest request = CriarRequisicao($"{configuracaoIntegracao.UrlApi}/sendDoc", "POST", "application/json");
                    request.Headers.Add("Authorization", $"Bearer {configuracaoIntegracao.Token}");

                    jsonRequest = JsonConvert.SerializeObject(requestSendDoc, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                    byte[] sendData = Encoding.UTF8.GetBytes(jsonRequest);
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(sendData, 0, sendData.Length);
                    requestStream.Flush();
                    requestStream.Dispose();

                    HttpWebResponse retornoRequisicao = ExecutarRequisicao(request);
                    response = ObterResposta(retornoRequisicao);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ResponseCreate result = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ResponseCreate>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    mensagemRetorno = "Falha ao integrar documento";
                    if (IsRetornoSucesso(retornoRequisicao))
                        mensagemRetorno = result.Sucesso;

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoKlios");
                    mensagemRetorno = "Ocorreu uma falha ao integrar documentos";
                }

                serJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, jsonRequest, response, mensagemRetorno);
            }
        }

        private void ConsultarRetornoAnalise(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao serJanelaCarregamentoIntegracao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);
            string mensagemRetorno = string.Empty;
            string requestParamsEncoded = string.Empty;
            string response = string.Empty;
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                requestParamsEncoded = ObterRequestConsulta(janelaCarregamentoIntegracao);

                HttpWebRequest request = CriarRequisicao(BuildURL($"{configuracaoIntegracao.UrlApi}/result", requestParamsEncoded), "GET", "application/json");
                request.Headers.Add("Authorization", $"Bearer {configuracaoIntegracao.Token}");

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(request);
                response = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                    throw new ServicoException($"Consulta não Encontrada");
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ResponseConsulta result = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ResponseConsulta>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    if (result.StatusAnalise == "Concluído")
                    {
                        mensagemRetorno = result.resultado;
                        if (result.resultado == "Recomendado")
                            AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.Recomendado, mensagemRetorno);
                        else if (result.resultado == "Não recomendado")
                            AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.NaoRecomendado, mensagemRetorno);
                    }
                    else if (result.StatusAnalise == "Em Análise")
                    {
                        mensagemRetorno = "Documentos em Análise";
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.AgRetorno, RecomendacaoGR.AguardandoValidacaoGR, mensagemRetorno);
                    }
                    else if (result.StatusAnalise == "Cadastrado")
                    {
                        mensagemRetorno = "Dados Enviados - Pendente de Anexo";
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.PendenciaDocumentos, mensagemRetorno);
                    }
                    else if (result.StatusAnalise == "Documento Reprovado")
                    {
                        mensagemRetorno = "Documento Reprovado";
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.Integrado, RecomendacaoGR.PendenciaDocumentos, mensagemRetorno);
                    }
                    else
                    {
                        mensagemRetorno = result.StatusAnalise;
                        AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
                    }
                }
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoKlios");
                mensagemRetorno = ex.Message;
                AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoKlios");
                mensagemRetorno = "Ocorreu uma falha ao consultar retorno da Analise";
                AtualizarSituacaoJanelaCarregamanto(janelaCarregamentoIntegracao, SituacaoIntegracao.ProblemaIntegracao, RecomendacaoGR.Falha, mensagemRetorno);
            }

            serJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, requestParamsEncoded, response, mensagemRetorno);
        }

        private void ValidarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios configuracaoIntegracao)
        {
            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Klios");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAutenticacao) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLConsultaAnaliseConjunto))
                throw new ServicoException("A URL não está configurada para a integração com Klios");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Usuario) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha))
                throw new ServicoException("O usuário e a senha devem estar preenchidos na configuração de integração Klios");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ConfiguracaoIntegracao ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKlios repIntegracaoKlios = new Repositorio.Embarcador.Configuracoes.IntegracaoKlios(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios configuracaoIntegracao = repIntegracaoKlios.Buscar();
            ValidarConfiguracoesIntegracao(configuracaoIntegracao);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.ConfiguracaoIntegracao()
            {
                UrlApi = configuracaoIntegracao.URLConsultaAnaliseConjunto,
                Token = ObterToken(configuracaoIntegracao.URLAutenticacao, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha)
            };
        }

        private HttpWebRequest CriarRequisicao(string url, string metodo, string contentType)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = metodo;
            request.Accept = "application/json";
            request.ContentType = contentType;

            return request;
        }

        private string BuildURL(string url, string queryParams)
        {
            if (!string.IsNullOrWhiteSpace(queryParams))
            {
                url += "?" + queryParams;
            }
            return url;
        }

        public string ObterToken(string urlToken, string usuario, string senha)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.Token token = new Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.Token();

            HttpWebRequest request = CriarRequisicao(urlToken, "POST", "application/x-www-form-urlencoded");

            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("email", usuario);
            requestParams.Add("password", senha);
            string requestParamsEncoded = EncodeRequestParams(requestParams);

            byte[] sendData = Encoding.UTF8.GetBytes(requestParamsEncoded);
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(sendData, 0, sendData.Length);
            requestStream.Flush();
            requestStream.Dispose();

            HttpWebResponse retornoRequisicao = ExecutarRequisicao(request);
            string response = ObterResposta(retornoRequisicao);

            if (IsRetornoSucesso(retornoRequisicao))
                token = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.Token>(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return token.token;
        }

        private string EncodeRequestParams(NameValueCollection queryParams)
        {
            string url = string.Empty;
            if (queryParams != null && queryParams.Count > 0)
            {
                foreach (string key in queryParams)
                {
                    url += $"{key}={Uri.EscapeUriString(queryParams[key])}&";
                }
            }
            return url;
        }

        private HttpWebResponse ExecutarRequisicao(WebRequest request)
        {
            try
            {
                WebResponse retornoRequisicao = request.GetResponse();
                return (HttpWebResponse)retornoRequisicao;
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                    throw new ServicoException("Falha ao processar o retorno da API");

                return (HttpWebResponse)webException.Response;
            }
        }

        private string ObterResposta(HttpWebResponse response)
        {
            string dadosRetornoRequisicao;
            using (System.IO.Stream streamDadosRetornoRequisicao = response.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                dadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }

            return dadosRetornoRequisicao;
        }

        private string PreencherParametrosPesquisa(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("id_solicitante", "8078");
            requestParams.Add("pesquisa_cpf", carga?.Motoristas?.FirstOrDefault()?.CPF ?? "");
            requestParams.Add("placa_veiculo", carga?.Veiculo?.Placa ?? "");
            requestParams.Add("placa_veiculo1", carga.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa ?? "");
            requestParams.Add("placa_veiculo2", carga.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa ?? "");
            requestParams.Add("placa_veiculo3", carga.VeiculosVinculados?.ElementAtOrDefault(2)?.Placa ?? "");

            return EncodeRequestParams(requestParams);
        }

        private string PreencherParametrosCriacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("id_solicitante", "8078");
            requestParams.Add("grupos", "1737");
            requestParams.Add("nome_condutor", carga.Motoristas?.FirstOrDefault()?.Nome ?? "");
            requestParams.Add("rg_condutor", carga.Motoristas?.FirstOrDefault()?.RG ?? "");
            requestParams.Add("cpf_condutor", carga.Motoristas?.FirstOrDefault()?.CPF ?? "");
            requestParams.Add("placa_veiculo", carga.Veiculo?.Placa ?? "");
            requestParams.Add("placa_veiculo1", carga.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa ?? "");
            requestParams.Add("placa_veiculo2", carga.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa ?? "");
            requestParams.Add("telefone", carga.Motoristas?.FirstOrDefault()?.Telefone ?? "");
            requestParams.Add("celular", carga.Motoristas?.FirstOrDefault()?.Celular ?? "");
            requestParams.Add("transportadora_cnpj_cpf", carga.Empresa?.CNPJ ?? "");
            requestParams.Add("funcao", "6");
            requestParams.Add("carga_nome", "");
            requestParams.Add("transportadora_cnpj_cpf_implemento_1", "");
            requestParams.Add("transportadora_cnpj_cpf_implemento_2", "");
            requestParams.Add("transportadora_cnpj_cpf_implemento_3", "");
            requestParams.Add("carga_lote", "");

            return EncodeRequestParams(requestParams);
        }

        private string ObterRequestConsulta(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao)
        {
            NameValueCollection requestParams = new NameValueCollection();
            requestParams.Add("id_solicitante", "8078");
            requestParams.Add("id_KliosAnalise", janelaCarregamentoIntegracao.Protocolo);

            return EncodeRequestParams(requestParams);
        }

        private bool IsRetornoSucesso(HttpWebResponse retornoRequisicao)
        {
            HttpStatusCode statusRequisicao = retornoRequisicao.StatusCode;

            return (statusRequisicao == HttpStatusCode.OK) || (statusRequisicao == HttpStatusCode.Created);
        }

        private void AtualizarSituacaoJanelaCarregamanto(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao, SituacaoIntegracao situacaoIntegracao, RecomendacaoGR? recomendacaoGR, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            janelaCarregamentoIntegracao.SituacaoIntegracao = situacaoIntegracao;
            janelaCarregamentoIntegracao.Mensagem = mensagem;
            janelaCarregamentoIntegracao.DataCriacao = DateTime.Now;

            if (recomendacaoGR.HasValue)
            {
                janelaCarregamentoIntegracao.CargaJanelaCarregamento.RecomendacaoGR = recomendacaoGR;
                repJanelaCarregamento.Atualizar(janelaCarregamentoIntegracao.CargaJanelaCarregamento);
            }

            repJanelaCarregamentoIntegracao.Atualizar(janelaCarregamentoIntegracao);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc> ObterRequestSendDoc(Dominio.Entidades.Embarcador.Cargas.Carga carga, string idAnalise)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc> documentosAnexo = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc>();

            Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> anexosMotorista = repFuncionarioAnexo.BuscarPorCodigoUsuarioETipo(carga.Motoristas.FirstOrDefault().Codigo, TipoAnexoMotorista.Cnh);

            foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo anexo in anexosMotorista)
                documentosAnexo.Add(PreencherRequestSendDoc(ObterAnexoBase64("Funcionario", anexo.NomeArquivo, anexo.GuidArquivo, _unitOfWork), "116", idAnalise));

            List<string> listaArquivos = ObterFotosMotorista(carga.Motoristas.FirstOrDefault().Codigo, _unitOfWork);

            foreach (string anexo in listaArquivos)
                documentosAnexo.Add(PreencherRequestSendDoc(ConverterParaBase64(anexo), "255", idAnalise));

            documentosAnexo.AddRange(ObterAnexosVeiculo(carga.Veiculo, "120", idAnalise));
            documentosAnexo.AddRange(ObterAnexosVeiculo(carga.VeiculosVinculados?.ElementAtOrDefault(0), "121", idAnalise));
            documentosAnexo.AddRange(ObterAnexosVeiculo(carga.VeiculosVinculados?.ElementAtOrDefault(1), "122", idAnalise));
            documentosAnexo.AddRange(ObterAnexosVeiculo(carga.VeiculosVinculados?.ElementAtOrDefault(2), "183", idAnalise));

            return documentosAnexo;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc> ObterAnexosVeiculo(Dominio.Entidades.Veiculo veiculo, string tipoDoc, string idAnalise)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc> documentosAnexo = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc>();

            if (veiculo == null)
                return documentosAnexo;

            Repositorio.Embarcador.Veiculos.VeiculoAnexo repVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo> anexosVeiculo = repVeiculoAnexo.BuscarPorCodigoVeiculoETipo(veiculo.Codigo, TipoAnexoVeiculo.Crlv);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo anexo in anexosVeiculo)
                documentosAnexo.Add(PreencherRequestSendDoc(ObterAnexoBase64("Veiculo", anexo.NomeArquivo, anexo.GuidArquivo, _unitOfWork), tipoDoc, idAnalise));

            return documentosAnexo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc PreencherRequestSendDoc(string anexoBase64, string tipoDoc, string idAnalise)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc requestSendDoc = new Dominio.ObjetosDeValor.Embarcador.Integracao.Klios.RequestSendDoc();
            requestSendDoc.id_KliosAnalise = idAnalise;
            requestSendDoc.tipo_doc = tipoDoc;
            requestSendDoc.file_base64 = anexoBase64;

            return requestSendDoc;
        }

        private string ObterAnexoBase64(string diretorio, string nomeAnexo, string guidAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", diretorio });
            string extencao = System.IO.Path.GetExtension(nomeAnexo).ToLower();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidAnexo + extencao);

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            return ConverterParaBase64(nomeArquivo);
        }

        private List<string> ObterFotosMotorista(int codigoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoGaleria = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Galeria", "Motorista" });
            string caminhoFoto = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" });

            List<string> listaArquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoGaleria, $"{codigoMotorista}_*.*").ToList();
            string fotoMotorista = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoFoto, $"{codigoMotorista}.*").FirstOrDefault();

            if (!string.IsNullOrEmpty(fotoMotorista))
                listaArquivos.Add(fotoMotorista);

            return listaArquivos;
        }

        private string ConverterParaBase64(string nomeArquivo)
        {
            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        #endregion
    }
}
