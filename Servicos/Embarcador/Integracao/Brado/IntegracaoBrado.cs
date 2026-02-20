using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Brado
{
    public class IntegracaoBrado
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoBrado(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        public void IntegrarCargasDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoBrado repIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao = repIntegracaoBrado.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestTransportador dadosRequisicao = PreencherRequisicaoTransportador(cargaDadosTransporteIntegracao);

                ObterToken(configuracaoIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicaoTransportador(configuracaoIntegracao, ObterToken(configuracaoIntegracao));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URLEnvioDadosTransporte, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");
                else
                    throw new ServicoException($"Problema ao integrar com Brado: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar Integrar.";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarCargaCte(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);

            cargaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;
            cargaCTeIntegracaoPendente.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoBrado repIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao = repIntegracaoBrado.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosIntegracaoCarga dadosRequisicao = PreencherRequisicaoIntegracaoCarga(cargaCTeIntegracaoPendente);
                ObterToken(configuracaoIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicaoTransportador(configuracaoIntegracao, ObterToken(configuracaoIntegracao));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URLEnvioDocumentosEmitidos, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCTeIntegracaoPendente.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com Brado: {retornoRequisicao.StatusCode}");
            }

            catch (ServicoException excecao)
            {
                cargaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brado";
            }

            servicoArquivoTransacao.Adicionar(cargaCTeIntegracaoPendente, jsonRequisicao, jsonRetorno, "json");
            repCargaCte.Atualizar(cargaCTeIntegracaoPendente);
        }

        public void IntegrarCargaCteOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            ocorrenciaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracaoPendente.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoBrado repIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao = repIntegracaoBrado.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametroDocumentoCte dadosRequisicao = PreencherRequisicaoDocumentosOcorrencia(ocorrenciaCTeIntegracaoPendente);
                ObterToken(configuracaoIntegracao);


                HttpClient requisicaoTransportador = CriarRequisicaoTransportador(configuracaoIntegracao, ObterToken(configuracaoIntegracao));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URLEnvioDocumentosEmitidos, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (RetornoSucesso(retornoRequisicao))
                {
                    ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else
                    throw new ServicoException($"Problema ao integrar com Brado: {retornoRequisicao.StatusCode}");
            }

            catch (ServicoException excecao)
            {
                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brado";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracaoPendente, jsonRequisicao, jsonRetorno, "json");
            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracaoPendente);
        }

        public void IntegrarCancelamentoCargaCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCTeCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> arquivoIntegracao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCTeCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeCancelamentoIntegracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoBrado repIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao = repIntegracaoBrado.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento dadosRequisicao = PreencherRequisicaoCancelamentoCargaCTe(ref cargaCTeCancelamentoIntegracao);
                ObterToken(configuracaoIntegracao);
                PreencherRequisicaoCancelamentoCargaCTe(ref cargaCTeCancelamentoIntegracao);

                HttpClient requisicaoTransportador = CriarRequisicaoCancelamento(configuracaoIntegracao, ObterToken(configuracaoIntegracao));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URLCancelamentoBrado, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    cargaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCTeCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else if (jsonRetorno.Contains("Conhecimento não encontrado ao solicitar cancelamento."))
                    throw new ServicoException("Problema ao integrar com Brado: conhecimento não encontrado ao solicitar cancelamento.");

                else
                    throw new ServicoException($"Problema ao integrar com Brado: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                cargaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brado";
            }

            arquivoIntegracao.Adicionar(cargaCTeCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaCTeIntegracaoArquivo.Atualizar(cargaCTeCancelamentoIntegracao);

        }

        public void IntegrarCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao integracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);

            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoBrado repIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao = repIntegracaoBrado.Buscar();
                ValidarConfiguracoesIntegracao(configuracaoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento dadosRequisicao = PreencherRequisicaoCancelamento(integracao);
                ObterToken(configuracaoIntegracao);
                PreencherRequisicaoCancelamento(integracao);

                HttpClient requisicaoTransportador = CriarRequisicaoTransportador(configuracaoIntegracao, ObterToken(configuracaoIntegracao));
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicaoTransportador.PostAsync(configuracaoIntegracao.URLCancelamentoBrado, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (RetornoSucesso(retornoRequisicao))
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracao.ProblemaIntegracao = "Integrado com sucesso";
                }

                else if (retornoRequisicao.StatusCode == HttpStatusCode.NotFound)
                    throw new ServicoException("Requisição não encontrada");

                else if (jsonRetorno.Contains("Conhecimento não encontrado ao solicitar cancelamento."))
                    throw new ServicoException("Problema ao integrar com Brado: conhecimento não encontrado ao solicitar cancelamento.");

                else
                    throw new ServicoException($"Problema ao integrar com Brado: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brado";
            }

            servicoArquivoTransacao.Adicionar(integracao, jsonRequisicao, jsonRetorno, "json");
            repOcorrenciaCTeCancelamentoIntegracao.Atualizar(integracao);
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao)
        {

            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestLoginIntegracao dadosRequisicao = PreencherRequisicaoLogin(configuracaoIntegracao);

            HttpClient requisicao = CriarRequisicaoLogin(configuracaoIntegracao);
            string jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLAutenticacao, conteudoRequisicao).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ResponseLogin resposta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ResponseLogin>(jsonRetorno);
            if (resposta.Token?.Token != null)
                return resposta?.Token?.Token;
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ResponseLoginProd respostaProd = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ResponseLoginProd>(jsonRetorno);
                return respostaProd?.Token;
            }

        }

        private void ValidarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao)
        {
            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Brado");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAutenticacao) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAutenticacao))
                throw new ServicoException("A URL não está configurada para a integração com Brado");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Usuario) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha))
                throw new ServicoException("O usuário e senha devem estar preenchidos na configuração de Integração com Brado");
        }

        private HttpClient CriarRequisicaoLogin(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrado));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLAutenticacao);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return requisicao;
        }

        private HttpClient CriarRequisicaoTransportador(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrado));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLEnvioDadosTransporte);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            return requisicao;
        }

        private HttpClient CriarRequisicaoCancelamento(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrado));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLCancelamentoBrado);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            return requisicao;
        }

        private bool RetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            return (retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestLoginIntegracao PreencherRequisicaoLogin(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestLoginIntegracao request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestLoginIntegracao();
            request.Modulo = "LOGON";
            request.Operacao = "LOGON";
            request.Parametros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosLoginIntegracao()
            {
                Usuario = configuracaoIntegracao.Usuario,
                Senha = configuracaoIntegracao.Senha,
                CodigoGestao = configuracaoIntegracao.CodigoGestao.ToInt()

            };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestTransportador PreencherRequisicaoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestTransportador request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestTransportador();
            request.Modulo = "M1867";
            request.Operacao = "aceiteTransportador";
            request.Parametros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosTransportador()
            {
                CodigoPedido = cargaDadosTransporteIntegracao.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.NumeroPedidoEmbarcador ?? "",
                CNPJTransportador = cargaDadosTransporteIntegracao.Carga.Empresa.CNPJ,
                Quantidade = 1,
                PlacaCavalo = cargaDadosTransporteIntegracao?.Carga?.Veiculo?.Placa ?? "",
                PlacaCarreta1 = cargaDadosTransporteIntegracao?.Carga?.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa ?? "",
                PlacaCarreta2 = cargaDadosTransporteIntegracao?.Carga?.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa ?? "",
                NomeMotorista = cargaDadosTransporteIntegracao.Carga?.Motoristas?.FirstOrDefault()?.Nome ?? "",
                MotoristaCPF = cargaDadosTransporteIntegracao.Carga?.Motoristas?.FirstOrDefault()?.CPF ?? "",
                MotoristaCNH = cargaDadosTransporteIntegracao.Carga?.Motoristas?.FirstOrDefault()?.NumeroHabilitacao ?? "",
            };


            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosIntegracaoCarga PreencherRequisicaoIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

            string xml = servicoCTe.ObterStringXMLAutorizacao(cargaCTeIntegracaoPendente.CargaCTe.CTe, _unitOfWork);

            string numeroPedido = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarPrimeiroNumeroPedidoPorCargaCTe(cargaCTeIntegracaoPendente.CargaCTe.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosIntegracaoCarga request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosIntegracaoCarga()
            {
                CodigoPedido = numeroPedido,
                ArquivoXML = Utilidades.XML.Serializar(xml).Replace("<string>", "").Replace("</string>", "")
            };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametroDocumentoCte PreencherRequisicaoDocumentosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

            string xml = servicoCTe.ObterStringXMLAutorizacao(ocorrenciaCTeIntegracaoPendente.CargaCTe.CTe, _unitOfWork);

            string numeroPedido = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarPrimeiroNumeroPedidoPorCargaCTe(ocorrenciaCTeIntegracaoPendente.CargaCTe.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametroDocumentoCte request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametroDocumentoCte()
            {
                CodigoPedido = numeroPedido,
                ArquivoXML = Utilidades.XML.Serializar(xml).Replace("<string>", "").Replace("</string>", "")
            };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento PreencherRequisicaoCancelamentoCargaCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCTeCancelamentoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento()
            {
                Modulo = "M1867",
                Operacao = "cancelarDocumento",
                Parametros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosCargaCancelamento()
                {
                    TipoDocumento = "CTE",
                    TipoCancelamento = "C",
                    MotivoCancelamento = cargaCTeCancelamentoIntegracao.CargaCancelamento?.MotivoCancelamento,

                }
            };

            request.Parametros.Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.DocumentosCargaCancelamento>();


            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.DocumentosCargaCancelamento objetoDocumento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.DocumentosCargaCancelamento()
            {
                ChaveDocumento = cargaCTeCancelamentoIntegracao.CargaCTe?.CTe?.Chave.ToString() ?? "",
                EmissorCNJPJ = cargaCTeCancelamentoIntegracao.CargaCTe?.CTe?.Empresa?.CNPJ_SemFormato ?? "",
                NumeroDocumento = cargaCTeCancelamentoIntegracao.CargaCTe?.CTe?.Numero.ToString() ?? "",
                Serie = cargaCTeCancelamentoIntegracao.CargaCTe?.CTe?.Serie?.Numero.ToString() ?? ""
            };

            request.Parametros.Documentos.Add(objetoDocumento);


            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento PreencherRequisicaoCancelamento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao integracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.RequestCargaCancelamento()
            {
                Modulo = "M1867",
                Operacao = "cancelarDocumento",
                Parametros = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.ParametrosCargaCancelamento()
                {
                    TipoDocumento = "CTE",
                    TipoCancelamento = "C",
                    MotivoCancelamento = integracao.OcorrenciaCancelamento?.MotivoCancelamento
                }
            };


            request.Parametros.Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.DocumentosCargaCancelamento>();


            Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.DocumentosCargaCancelamento objetoDocumento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Brado.DocumentosCargaCancelamento()
            {

                ChaveDocumento = integracao.OcorrenciaCTeIntegracao?.CargaCTe?.CTe?.Chave ?? "",
                EmissorCNJPJ = integracao.OcorrenciaCTeIntegracao?.CargaCTe?.CTe?.Empresa?.CNPJ_SemFormato ?? "",
                NumeroDocumento = integracao.OcorrenciaCTeIntegracao?.CargaCTe?.CTe?.Numero.ToString() ?? "",
                Serie = integracao.OcorrenciaCTeIntegracao?.CargaCTe?.CTe?.Serie?.Numero.ToString() ?? "",

            };

            request.Parametros.Documentos.Add(objetoDocumento);


            return request;
        }
    }
}
