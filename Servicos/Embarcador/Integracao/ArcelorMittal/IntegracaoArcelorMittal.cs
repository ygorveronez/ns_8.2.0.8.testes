using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Infrastructure.Services.HttpClientFactory;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.ArcelorMittal
{
    public class IntegracaoArcelorMittal
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;
        private readonly ITarefaIntegracao _repositorioTarefaIntegracao;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal _configuracaoIntegracao;

        #endregion Atributos

        #region Construtores

        public IntegracaoArcelorMittal(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IntegracaoArcelorMittal(Repositorio.UnitOfWork unitOfWork, IRequestSubtarefaRepository repositorioSubtarefa, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _repositorioSubtarefa = repositorioSubtarefa;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
        }
        #endregion Construtores

        #region Métodos Públicos

        /// <summary>
        /// Será desativado esse método por Pedido
        /// </summary>
        /// <param name="pedidoOcorrenciaColetaEntrega"></param>
        /// <returns></returns>
        public HttpRequisicaoResposta EnviarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioIntegracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelorMittal = repositorioIntegracaoArcelorMittal.Buscar();

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelorMittal?.URLOcorrencia))
                    throw new ServicoException("A integração com a ArcelorMittal não está configurada.");

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracaoArcelorMittal.URLOcorrencia, configuracaoIntegracaoArcelorMittal.Usuario, configuracaoIntegracaoArcelorMittal.Senha);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EnvioOcorrencia evento = ObterDadosEnvioOcorrencia(pedidoOcorrenciaColetaEntrega);
                string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracaoArcelorMittal.URLOcorrencia, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                    httpRequisicaoResposta.sucesso = true;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    httpRequisicaoResposta.mensagem = (string)retorno;
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a ArcelorMittal.";
            }

            return httpRequisicaoResposta;
        }

        public HttpRequisicaoResposta EnviarOcorrencia(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioIntegracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelorMittal = repositorioIntegracaoArcelorMittal.Buscar();

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelorMittal?.URLOcorrencia))
                    throw new ServicoException("A integração com a ArcelorMittal não está configurada.");

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracaoArcelorMittal.URLOcorrencia, configuracaoIntegracaoArcelorMittal.Usuario, configuracaoIntegracaoArcelorMittal.Senha);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EnvioOcorrencia evento = ObterDadosEnvioOcorrencia(cargaEntregaEvento);
                string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracaoArcelorMittal.URLOcorrencia, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                    httpRequisicaoResposta.sucesso = true;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    httpRequisicaoResposta.mensagem = (string)retorno;
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a ArcelorMittal.";
            }

            return httpRequisicaoResposta;
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repConfiguracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelor = repConfiguracaoArcelorMittal.Buscar();

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelor?.URLConfirmarAvancoTransporte))
                    throw new ServicoException("Não há URL configurada para a integração");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = $"{configuracaoIntegracaoArcelor.URLConfirmarAvancoTransporte}";

                HttpClient client = CriarRequisicao(configuracaoIntegracaoArcelor.URLConfirmarAvancoTransporte, configuracaoIntegracaoArcelor.Usuario, configuracaoIntegracaoArcelor.Senha);

                dynamic objEnvio = ConverterObjetoEnvioCarga(cargaCargaIntegracao.Carga);

                jsonRequest = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                if (result.IsSuccessStatusCode)
                {
                    mensagemErro = "Integrado com sucesso";

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);

                    Servicos.Log.TratarErro($"Integrado com sucesso: {cargaCargaIntegracao.Codigo}", "IntegracaoArcelorMittal");
                }
                else
                {
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a ArcelorMittal.";
                    else
                        mensagemErro = "Retorno ArcelorMittal: " + mensagemErro;

                    Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoArcelorMittal");
                    Servicos.Log.TratarErro("IntegracaoArcelorMittalCarga", "IntegracaoArcelorMittal");
                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoArcelorMittal");
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoArcelorMittal");

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoArcelorMittal");

                mensagemErro = excecao.Message;

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + configuracaoIntegracaoArcelor?.URLConfirmarAvancoTransporte);
                Servicos.Log.TratarErro(excecao, "IntegracaoArcelorMittal");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoArcelorMittal");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoArcelorMittal");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da ArcelorMittal.";

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }
        }

        public void IntegrarCargaEvento(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracaoEvento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repConfiguracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelor = repConfiguracaoArcelorMittal.Buscar();

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;

            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelor?.URLConfirmarAvancoTransporte))
                    throw new ServicoException("Não há URL configurada para a integração");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = $"{configuracaoIntegracaoArcelor.URLConfirmarAvancoTransporte}";

                HttpClient client = CriarRequisicao(configuracaoIntegracaoArcelor.URLConfirmarAvancoTransporte, configuracaoIntegracaoArcelor.Usuario, configuracaoIntegracaoArcelor.Senha);

                dynamic objEnvio = ConverterObjetoEnvioCargaEvento(integracao);

                jsonRequest = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                if (result.IsSuccessStatusCode)
                {
                    mensagemErro = "Integrado com sucesso";

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = integracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    if (integracao.ArquivosTransacao == null)
                        integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                    integracao.ArquivosTransacao.Add(arquivoIntegracao);

                    integracao.ProblemaIntegracao = mensagemErro;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    repCargaIntegracao.Atualizar(integracao);

                    Servicos.Log.TratarErro($"Integrado com sucesso: {integracao.Codigo}", "IntegracaoArcelorMittal");
                }
                else
                {
                    if (retorno == null)
                        mensagemErro = result.StatusCode.ToString();
                    else
                        mensagemErro = retorno.message;
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a ArcelorMittal.";
                    else
                        mensagemErro = "Retorno ArcelorMittal: " + mensagemErro;

                    Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoArcelorMittal");
                    Servicos.Log.TratarErro("IntegracaoArcelorMittalCarga", "IntegracaoArcelorMittal");
                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoArcelorMittal");
                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoArcelorMittal");

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = integracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    integracao.ArquivosTransacao.Add(arquivoIntegracao);

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(integracao);
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoArcelorMittal");

                mensagemErro = excecao.Message;

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(integracao);
                return;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("URL: " + configuracaoIntegracaoArcelor?.URLConfirmarAvancoTransporte);
                Servicos.Log.TratarErro(excecao, "IntegracaoArcelorMittal");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoArcelorMittal");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoArcelorMittal");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da ArcelorMittal.";

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = integracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                integracao.ArquivosTransacao.Add(arquivoIntegracao);

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(integracao);
                return;
            }
        }

        public void IntegrarDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            IntegrarDadosCarga<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(cargaDadosTransporteIntegracao, cargaDadosTransporteIntegracao.Carga);
        }

        public void IntegrarDadosCarga(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao gestaoDadosColetaIntegracao)
        {
            IntegrarDadosCarga<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao, Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo>(gestaoDadosColetaIntegracao, gestaoDadosColetaIntegracao.GestaoDadosColeta.CargaEntrega.Carga);
        }

        public void IntegrarTransporteFornecimento(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao gestaoDadosColetaIntegracao)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao repositorioGestaoDadosColetaIntegracao = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo>(_unitOfWork);
            InspectorBehavior inspector = new InspectorBehavior();

            gestaoDadosColetaIntegracao.DataIntegracao = DateTime.Now;
            gestaoDadosColetaIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioIntegracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelorMittal = repositorioIntegracaoArcelorMittal.Buscar();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelorMittal?.URLAtualizarNFeAprovada) || string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelorMittal.Usuario) || string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelorMittal.Senha))
                    throw new ServicoException("Não possui configuração de integração com a Arcelor Mittal");

                ServicoSAP.TransporteFornecimento.MI_LBELGSD00007_TransporteFornecimento_Out_AClient client = ObterClientRequisicao(configuracaoIntegracaoArcelorMittal);

                client.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP.TransporteFornecimento.DT_TRANSPORTEFORNECIMENTO request = PreencherRequesicaoTransporteFornecimento(gestaoDadosColetaIntegracao.GestaoDadosColeta.CargaEntrega.Carga);

                client.MI_LBELGSD00007_TransporteFornecimento_Out_A(request);

                gestaoDadosColetaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                gestaoDadosColetaIntegracao.ProblemaIntegracao = "Integrado com sucesso";

                servicoArquivoTransacao.Adicionar(gestaoDadosColetaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException excecao)
            {
                gestaoDadosColetaIntegracao.ProblemaIntegracao = excecao.Message;
                gestaoDadosColetaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                gestaoDadosColetaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                gestaoDadosColetaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a intergração com a Arcelor Mittal";
                servicoArquivoTransacao.Adicionar(gestaoDadosColetaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioGestaoDadosColetaIntegracao.Atualizar(gestaoDadosColetaIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void IntegrarDadosCarga<TIntegracao, TIntegracaoArquivo>(TIntegracao integracao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
            where TIntegracao : Dominio.Entidades.Embarcador.Integracao.Integracao, Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<TIntegracaoArquivo>
            where TIntegracaoArquivo : Dominio.Entidades.Embarcador.Integracao.IntegracaoArquivo, new()
        {
            Repositorio.RepositorioBase<TIntegracao> repositorioIntegracao = new Repositorio.RepositorioBase<TIntegracao>(_unitOfWork);
            ArquivoTransacao<TIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<TIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonResposta = string.Empty;

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioConfiguracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelor = repositorioConfiguracaoArcelorMittal.Buscar();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoArcelor?.URLDadosTransporteSAP))
                    throw new ServicoException("Não há URL configurada para a integração Dados Transporte SAP");

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracaoArcelor.URLConfirmarAvancoTransporte, configuracaoIntegracaoArcelor.Usuario, configuracaoIntegracaoArcelor.Senha);
                dynamic dadosTransporte = ObterDadosTransporte(carga);
                jsonRequisicao = JsonConvert.SerializeObject(dadosTransporte, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracaoArcelor.URLDadosTransporteSAP, conteudoRequisicao).Result;
                jsonResposta = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResposta);

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    integracao.ProblemaIntegracao = "Integrado com sucesso";
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
                else
                {
                    string mensagemErro = (retorno == null) ? retornoRequisicao.StatusCode.ToString() : retorno.message;

                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = string.IsNullOrWhiteSpace(mensagemErro) ? "Falha na integração com a ArcelorMittal." : $"Retorno ArcelorMittal: {mensagemErro}";
                }

                servicoArquivoTransacao.Adicionar(integracao, jsonRequisicao, jsonResposta, "json");
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoArcelorMittal");
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Serviço da ArcelorMittal.";
                servicoArquivoTransacao.Adicionar(integracao, jsonRequisicao, jsonResposta, "json");
            }

            repositorioIntegracao.Atualizar(integracao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EnvioOcorrencia ObterDadosEnvioOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EnvioOcorrencia()
            {
                CodigoParceiro = "BMJFLE006",
                Parceiro = "Multisoftware",
                Transportes = ObterEventosTransporte(pedidoOcorrenciaColetaEntrega)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EnvioOcorrencia ObterDadosEnvioOcorrencia(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EnvioOcorrencia()
            {
                CodigoParceiro = "BMJFLE006",
                Parceiro = "Multisoftware",
                Transportes = ObterEventosTransporte(cargaEntregaEvento)
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte> ObterEventosTransporte(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte> listaEventos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte>();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = pedidoOcorrenciaColetaEntrega.Carga;
            string tipoTracking = ObterTipoTracking(pedidoOcorrenciaColetaEntrega, carga);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga != null ? repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo) : null;
            Dominio.Entidades.Embarcador.Pedidos.Stage stage = repositorioPedidoStage.BuscarStagePorCargaPedido(cargaPedido?.Codigo ?? 0);

            string etapa = stage?.NumeroStage ?? string.Empty;

            if (string.IsNullOrEmpty(etapa))
                etapa = BuscarEtapaArquivoIntegracao(carga, cargaPedido);

            string codigoCargaEmbarcador = (configuracaoGeralCarga?.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho ?? false) ? cargaPedido?.CargaRedespacho?.Carga?.CodigoCargaEmbarcador ?? string.Empty : carga?.CodigoCargaEmbarcador ?? string.Empty;

            //Enviará apenas o evento da integração
            Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte eventoTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte()
            {
                TipoTransporte = carga.TipoDocumentoTransporte?.Descricao ?? carga.TipoOperacao?.Descricao ?? string.Empty,
                Transporte = codigoCargaEmbarcador,
                Evento = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CodigoIntegracao,
                TrackingTransporte = "Automático",
                TipoTracking = tipoTracking,
                Etapa = etapa,
                Data = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("yyyyMMdd"),
                Hora = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("HHmmss"),
                Fornecimentos = ObterNotasFiscais(pedidoOcorrenciaColetaEntrega, tipoTracking)
            };

            listaEventos.Add(eventoTransporte);

            return listaEventos;
        }

        private string BuscarEtapaArquivoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            try
            {
                Servicos.Log.TratarErro($"Buscando etapa arquivo de origem, carga: {carga.Codigo}, Pedido:{cargaPedido?.Pedido.Codigo ?? 0}", "BuscarEtapaArquivoIntegracao");

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(_unitOfWork);
                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarUltimaPorCarga(carga.Codigo, null);

                if (integracao == null || integracao.ArquivoRequisicao == null)
                    return "";
                else
                {
                    string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integracao.ArquivoRequisicao);
                    Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(request);

                    if (documentoTransporte == null)
                        return "";

                    Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage stage = documentoTransporte.Pedido.Where(x => x.ProtocoloPedido == (cargaPedido?.Pedido.Codigo ?? 0).ToString())?.FirstOrDefault()?.Stage?.FirstOrDefault() ?? null;
                    if (stage == null)
                        return "";
                    else
                        return stage.NumeroStage;
                }
            }
            catch (Exception)
            {

                return "";
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte> ObterEventosTransporte(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repositorioStagePedido = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Redespacho repositorioCargaRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte> listaEventos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte>();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntregaEvento.Carga;

            string trackingTransporte = ObterTrackingTransporte(cargaEntregaEvento, carga);

            string tipoTracking = ObterTipoTracking(cargaEntregaEvento, carga, trackingTransporte);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaEntregaEvento.CargaEntrega != null ? cargaEntregaEvento.CargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido : repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Stage stage = repositorioStagePedido.BuscarStagePorCargaPedido(cargaPedido?.Codigo ?? 0);
            string etapa = stage?.NumeroStage ?? string.Empty;

            if (string.IsNullOrEmpty(etapa))
                etapa = BuscarEtapaArquivoIntegracao(carga, cargaPedido);

            Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = repositorioCargaRedespacho.BuscarPorCarga(carga.Codigo);
            if (redespacho == null)
                redespacho = repositorioCargaRedespacho.BuscarPorCargaGerada(carga.Codigo);

            if (redespacho != null && redespacho.CargaGerada?.Codigo == carga.Codigo)
                etapa = "002";
            else if (redespacho != null && (redespacho.Carga.Codigo == carga.Codigo || string.IsNullOrEmpty(etapa)))
                etapa = "001";

            if (string.IsNullOrEmpty(etapa) && cargaEntregaEvento.CargaEntrega != null && cargaEntregaEvento.CargaEntrega.CargaOrigem != null)
            {
                stage = repStage.BuscarPrimeiraPorCarga(cargaEntregaEvento.CargaEntrega.CargaOrigem.Codigo);
                etapa = stage?.NumeroStage ?? string.Empty;
            }

            string codigoCargaEmbarcador = configuracaoGeralCarga.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho ? cargaPedido?.CargaRedespacho?.Carga?.CodigoCargaEmbarcador ?? string.Empty : carga.CodigoCargaEmbarcador;

            //Enviará apenas o evento da integração
            Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte eventoTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.EventoTransporte()
            {
                TipoTransporte = carga.TipoDocumentoTransporte?.Descricao ?? carga.TipoOperacao?.Descricao ?? string.Empty,
                Transporte = codigoCargaEmbarcador,
                Evento = cargaEntregaEvento.TipoDeOcorrencia.CodigoIntegracao,
                TrackingTransporte = trackingTransporte,
                TipoTracking = tipoTracking,
                Etapa = etapa,
                Data = cargaEntregaEvento.DataOcorrencia.ToString("yyyyMMdd"),
                Hora = cargaEntregaEvento.DataOcorrencia.ToString("HHmmss"),
                Fornecimentos = ObterFornecimentos(cargaEntregaEvento, tipoTracking)
            };
            listaEventos.Add(eventoTransporte);

            return listaEventos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento> ObterFornecimentos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, string tipoTracking)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento> listaFornecimentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaEntregaEvento.CargaEntrega != null ? cargaEntregaEvento.CargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido).ToList() : repCargaPedido.BuscarPedidosPorCarga(cargaEntregaEvento.Carga.Codigo);

            if (cargaEntregaEvento.EventoColetaEntrega == EventoColetaEntrega.FimViagem)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaCargaEntrega = repCargaEntrega.BuscarUltimaCargaEntregaRealizada(cargaEntregaEvento.Carga.Codigo);
                pedidos = ultimaCargaEntrega != null ? ultimaCargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido).ToList() : repCargaPedido.BuscarPedidosPorCarga(cargaEntregaEvento.Carga.Codigo);
            }

            if (pedidos == null || pedidos.Count == 0)
                throw new ServicoException("Não possui pedidos para integrar!");

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = cargaEntregaEvento.CargaEntrega == null ? repCargaEntrega.BuscarPorCarga(cargaEntregaEvento.Carga.Codigo) : null;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = pedido.NotasFiscais.ToList();
                if (notasFiscais.Count <= 0)
                    notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedido.Codigo);

                if (notasFiscais.Count == 0)
                    throw new ServicoException($"Não possui nota fiscal para integrar o pedido {pedido.NumeroPedidoEmbarcador}");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaEvento.CargaEntrega != null ? cargaEntregaEvento.CargaEntrega : cargaEntregas.Where(o => o.Pedidos.Any(p => p.CargaPedido.Pedido.Codigo == pedido.Codigo) && !o.Coleta).FirstOrDefault();
                if (cargaEntrega == null)
                    cargaEntrega = repCargaEntrega.BuscarPorPedido(pedido.Codigo);

                if (cargaEntrega == null)
                    throw new ServicoException($"Não encontrado a entrega para o pedido {pedido.NumeroPedidoEmbarcador}");

                if (!string.IsNullOrWhiteSpace(cargaEntregaEvento.TipoDeOcorrencia.CodigoEventoOcorrenciaPrimeiro))
                {
                    listaFornecimentos.AddRange(ObterNotasFiscais(notasFiscais, pedido, cargaEntregaEvento, cargaEntrega, cargaEntregaEvento.TipoDeOcorrencia.CodigoEventoOcorrenciaPrimeiro, configuracao));

                    if (!string.IsNullOrWhiteSpace(cargaEntregaEvento.TipoDeOcorrencia.CodigoEventoOcorrenciaSegundo))
                        listaFornecimentos.AddRange(ObterNotasFiscais(notasFiscais, pedido, cargaEntregaEvento, cargaEntrega, cargaEntregaEvento.TipoDeOcorrencia.CodigoEventoOcorrenciaSegundo, configuracao));
                }
                else
                    listaFornecimentos.AddRange(ObterNotasFiscais(notasFiscais, pedido, cargaEntregaEvento, cargaEntrega, "", configuracao));
            }

            return listaFornecimentos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento> ObterNotasFiscais(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, string tipoTracking)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento> listaNotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento>();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = pedidoOcorrenciaColetaEntrega.Carga;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoOcorrenciaColetaEntrega.Pedido;

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = pedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
            if (notasFiscais.Count <= 0)
                notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedidoOcorrenciaColetaEntrega.Pedido.Codigo);

            if (notasFiscais.Count == 0)
                throw new ServicoException("Não possui nota fiscal para integrar!");

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento fornecimento = new Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento()
                {
                    NrFornecimento = pedido.NumeroPedidoEmbarcador,
                    NumeroNotaFiscal = xmlNotaFiscal.Numero.ToString(),
                    Serie = xmlNotaFiscal.Serie,
                    TransporteSAP = carga?.CodigoCargaEmbarcador ?? string.Empty,
                    Latitude = pedidoOcorrenciaColetaEntrega.Latitude?.ToString() ?? "0",
                    Longitude = pedidoOcorrenciaColetaEntrega.Longitude?.ToString() ?? "0",
                    Ocorrencia = "",
                    TipoPrevisao = pedidoOcorrenciaColetaEntrega.DataPrevisaoRecalculada.HasValue ? "Dinâmica" : "Estática",
                    DataPrevisaoEntregaInicio = pedido.PrevisaoEntrega?.ToString("yyyyMMdd") ?? null,
                    HoraPrevisaoEntregaInicio = pedido.PrevisaoEntrega?.ToString("HHmmss") ?? null,
                    DataPrevisaoEntregaFim = pedido.DataPrevisaoSaida?.ToString("yyyyMMdd") ?? null,
                    HoraPrevisaoEntregaFim = pedido.DataPrevisaoSaida?.ToString("HHmmss") ?? null,
                    TrackingEntrega = "Automático",
                    TipoTrackingEntrega = tipoTracking,
                    DataAgendaEntrega = pedido.DataAgendamento?.ToString("yyyyMMdd") ?? null,
                    HoraAgendaEntrega = pedido.DataAgendamento?.ToString("HHmmss") ?? null,
                    TipoOcorrencia = "",
                    ImpediuEntrega = "Não"
                };

                listaNotas.Add(fornecimento);
            }

            return listaNotas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento> ObterNotasFiscais(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string ocorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento> listaNotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento>();

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            bool eventoSinistro = cargaEntregaEvento.TipoDeOcorrencia.CodigoIntegracao.Equals("001");

            DateTime? previsaoEntregaFim = null;
            if (cargaEntrega.DataReprogramada.HasValue && configuracao.TempoPadraoDeEntregaParaCalcularPrevisao > 0)
                previsaoEntregaFim = cargaEntrega.DataReprogramada.Value.AddMinutes(configuracao.TempoPadraoDeEntregaParaCalcularPrevisao);
            else if (cargaEntrega.DataPrevista.HasValue && configuracao.TempoPadraoDeEntregaParaCalcularPrevisao > 0)
                previsaoEntregaFim = cargaEntrega.DataPrevista.Value.AddMinutes(configuracao.TempoPadraoDeEntregaParaCalcularPrevisao);

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento fornecimento = new Dominio.ObjetosDeValor.Embarcador.Integracao.ArcelorMittal.Fornecimento()
                {
                    NrFornecimento = pedido.NumeroPedidoEmbarcador,
                    NumeroNotaFiscal = xmlNotaFiscal.Numero.ToString(),
                    Serie = xmlNotaFiscal.Serie,
                    TransporteSAP = cargaEntregaEvento.Carga.CodigoCargaEmbarcador,
                    Latitude = cargaEntregaEvento.Latitude?.ToString(cultura) ?? "0",
                    Longitude = cargaEntregaEvento.Longitude?.ToString(cultura) ?? "0",
                    Ocorrencia = ocorrencia,
                    TipoPrevisao = !string.IsNullOrEmpty(cargaEntregaEvento.TipoDeOcorrencia?.DescricaoTipoPrevisao) ? cargaEntregaEvento.TipoDeOcorrencia?.DescricaoTipoPrevisao : cargaEntregaEvento.DataPrevisaoRecalculada.HasValue ? "Dinâmica" : "Estática",
                    DataPrevisaoEntregaInicio = cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada.Value.ToString("yyyyMMdd") : cargaEntrega.DataPrevista.HasValue ? cargaEntrega.DataPrevista.Value.ToString("yyyyMMdd") : null,
                    HoraPrevisaoEntregaInicio = cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada.Value.ToString("HHmmss") : cargaEntrega.DataPrevista.HasValue ? cargaEntrega.DataPrevista.Value.ToString("HHmmss") : null,
                    DataPrevisaoEntregaFim = previsaoEntregaFim?.ToString("yyyyMMdd") ?? null,
                    HoraPrevisaoEntregaFim = previsaoEntregaFim?.ToString("HHmmss") ?? null,
                    TrackingEntrega = cargaEntregaEvento.Origem == OrigemSituacaoEntrega.App || cargaEntregaEvento.Origem == OrigemSituacaoEntrega.MonitoramentoAutomaticamente ? "Automático" : "Manual",
                    TipoTrackingEntrega = cargaEntregaEvento.Origem == OrigemSituacaoEntrega.App ? "Mobile" : cargaEntregaEvento.Origem == OrigemSituacaoEntrega.MonitoramentoAutomaticamente ? "Rastreador" : "Manual",
                    DataAgendaEntrega = pedido.DataAgendamento?.ToString("yyyyMMdd") ?? null,
                    HoraAgendaEntrega = pedido.DataAgendamento?.ToString("HHmmss") ?? null,
                    TipoOcorrencia = eventoSinistro ? "001" : "",
                    ImpediuEntrega = eventoSinistro ? "Sim" : "Não"
                };

                listaNotas.Add(fornecimento);
            }

            return listaNotas;
        }

        private string ObterTipoTracking(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!pedidoOcorrenciaColetaEntrega.DataPosicao.HasValue || carga == null || carga.Veiculo == null)
                return string.Empty;

            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.Posicao posicao = repPosicao.BuscarPosicaoPorVeiculoData(carga.Veiculo.Codigo, pedidoOcorrenciaColetaEntrega.DataPosicao.Value);
            if (posicao == null || posicao.Rastreador == EnumTecnologiaRastreador.NaoDefinido)
                return string.Empty;

            if (posicao.Rastreador == EnumTecnologiaRastreador.Mobile)
                return "Mobile";

            return "Rastreador";
        }

        private string ObterTrackingTransporte(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

            string retorno = "Rastreador";

            if (cargaEntregaEvento.EventoColetaEntrega == EventoColetaEntrega.FimViagem && repCargaEntrega.ExisteCargaEntregaFinalizadaManualmentePorCarga(cargaEntregaEvento.Carga.Codigo))
                return "Manual";

            if (cargaEntregaEvento.Origem == OrigemSituacaoEntrega.App)
            {
                retorno = "Mobile";
                return retorno;
            }

            if (!cargaEntregaEvento.DataPosicao.HasValue || carga == null || carga.Veiculo == null)
                retorno = "Manual";

            if (carga.TipoOperacao != null && carga.TipoOperacao.ModalCarga == TipoModal.Ferroviario)
                retorno = "Rastreador";
            else
            {
                List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarPorVeiculoDataInicialeFinal(carga.Veiculo?.Codigo ?? 0, cargaEntregaEvento.DataPosicao.Value.AddMinutes(-10), cargaEntregaEvento.DataPosicao.Value.AddMinutes(10));
                if (posicoes != null && posicoes.Count > 0)
                {
                    if (posicoes.Any(obj => obj.Rastreador == EnumTecnologiaRastreador.Mobile))
                        retorno = "Mobile";

                    if (posicoes.Any(obj => obj.Rastreador == EnumTecnologiaRastreador.Manual || obj.Rastreador == EnumTecnologiaRastreador.NaoDefinido))
                        retorno = "Manual";
                }
                else
                    retorno = "Manual";
            }

            return retorno;
        }

        private string ObterTipoTracking(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, Dominio.Entidades.Embarcador.Cargas.Carga carga, string trackingTransporte)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);

            if (trackingTransporte == "Manual")
                return "Manual";

            if (!cargaEntregaEvento.DataPosicao.HasValue || carga.Veiculo == null)
                return string.Empty;

            if (trackingTransporte == "Mobile")
                return "Mobile";

            if (trackingTransporte == "Rastreador")
            {
                if (carga.TipoOperacao != null && carga.TipoOperacao.ModalCarga == TipoModal.Ferroviario)
                    return trackingTransporte;

                Dominio.Entidades.Embarcador.Logistica.Posicao posicao = repPosicao.BuscarPosicaoPorVeiculoData(carga.Veiculo.Codigo, cargaEntregaEvento.DataPosicao.Value);

                if (posicao?.Rastreador == EnumTecnologiaRastreador.Mobile)
                    return "Mobile";
            }

            return "Rastreador";
        }

        private HttpClient CriarRequisicao(string url, string usuario, string senha)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoArcelorMittal));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(usuario, senha);

            return requisicao;
        }

        private dynamic ConverterObjetoEnvioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Pedidos == null || carga.Pedidos.Count == 0)
                throw new ServicoException("Não há pedidos nessa carga");
            List<dynamic> itens = new List<dynamic>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                itens.Add(new
                {
                    ProtocoloPedido = cargaPedido.Pedido?.Codigo ?? 0,
                    Status = "S",
                    Mensagem = "Mensagem do Processamento da Fila"
                });
            }
            dynamic retorno = new
            {
                ProtocoloCarga = carga.Codigo,
                Itens = itens
            };
            return retorno;
        }

        public static string AdicionarHifenSeNecessario(string placa)
        {
            // Verifica se a placa já contém um hífen
            if (!placa.Contains("-"))
            {
                string placaLimpa = placa.Trim().ToUpper();
                if (placaLimpa.Length >= 5)
                    return placaLimpa.Insert(3, "-");
            }
            return placa;
        }

        private dynamic ObterDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {

            string PlacaCavalo = carga?.Veiculo?.Placa ?? "";
            string PlacaCarreta = "";
            string NomeMotorista = carga?.Motoristas?.FirstOrDefault()?.Nome ?? "";
            string CpfMotorista = carga?.Motoristas?.FirstOrDefault()?.CPF ?? "";

            foreach (var veiculo in carga.VeiculosVinculados)
            {
                if (PlacaCavalo == "")
                    PlacaCavalo = veiculo?.Placa ?? "";
                else if (PlacaCarreta == "" && (veiculo?.Placa ?? "") != PlacaCavalo)
                    PlacaCarreta = veiculo?.Placa ?? "";

                if (NomeMotorista == "")
                {
                    NomeMotorista = veiculo?.Motoristas?.FirstOrDefault()?.Nome ?? "";
                    CpfMotorista = veiculo?.Motoristas?.FirstOrDefault()?.CPF ?? "";
                }
            }

            dynamic transporte = new
            {
                Transportes = new[]
                {
                    new
                    {
                        IdTransporte = carga.CodigoCargaEmbarcador,
                        PlacaCavalo = AdicionarHifenSeNecessario(PlacaCavalo),
                        PlacaCarreta = AdicionarHifenSeNecessario(PlacaCarreta),
                        NomeMotorista = NomeMotorista,
                        CpfMotorista = CpfMotorista,
                        IdTransportadora = ""
                    }
                }
            };

            return transporte;
        }

        private dynamic ConverterObjetoEnvioCargaEvento(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento integracao)
        {
            ////if (integracao.Carga.Pedidos == null || integracao.Carga.Pedidos.Count == 0)
            ////    throw new ServicoException("Não há pedidos nessa carga");
            ///
            List<dynamic> itens = new List<dynamic>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in integracao.Carga.Pedidos)
            {
                itens.Add(new
                {
                    ProtocoloPedido = (cargaPedido.Pedido?.Codigo ?? 0).ToString(),
                    Status = (integracao?.EnvioSucesso ?? false) ? "S" : "E",
                    Mensagem = (integracao?.EnvioSucesso ?? false) ? integracao?.Mensagem ?? string.Empty : $"Falha ao avançar a carga: {integracao?.Mensagem ?? $"Erro na etapa {(integracao != null ? integracao.Etapa.ObterDescricao() : "Desconhecida")}"}"
                });
            }
            dynamic retorno = new
            {
                ProtocoloCarga = integracao.Carga.Codigo,
                Evento = integracao.Etapa == EtapaCarga.NotaFiscal ? "D1" : integracao.Etapa == EtapaCarga.SalvarDadosTransporte ? "C1" : "",
                StatusEvento = (integracao?.EnvioSucesso ?? false) ? "S" : "E",
                Mensagem = integracao.Etapa == EtapaCarga.NotaFiscal ? (integracao?.EnvioSucesso ?? false) ? "NFes processadas com sucesso" : $"Falha ao processar Notas Fiscais: {integracao.Mensagem} " : integracao.Etapa == EtapaCarga.SalvarDadosTransporte ? (integracao?.EnvioSucesso ?? false) ? "DT processada com sucesso" : $"Falha ao processar DT: {integracao.Mensagem}" : "",
                Itens = itens
            };
            return retorno;
        }

        private ServicoSAP.TransporteFornecimento.MI_LBELGSD00007_TransporteFornecimento_Out_AClient ObterClientRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelorMittal)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracaoIntegracaoArcelorMittal.URLAtualizarNFeAprovada);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (configuracaoIntegracaoArcelorMittal.URLAtualizarNFeAprovada.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            ServicoSAP.TransporteFornecimento.MI_LBELGSD00007_TransporteFornecimento_Out_AClient client = new ServicoSAP.TransporteFornecimento.MI_LBELGSD00007_TransporteFornecimento_Out_AClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracaoIntegracaoArcelorMittal.Usuario;
            client.ClientCredentials.UserName.Password = configuracaoIntegracaoArcelorMittal.Senha;

            return client;
        }

        private Servicos.ServicoSAP.TransporteFornecimento.DT_TRANSPORTEFORNECIMENTO PreencherRequesicaoTransporteFornecimento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new Servicos.ServicoSAP.TransporteFornecimento.DT_TRANSPORTEFORNECIMENTO
            {
                TransporteFornecimento = ObterDadosTransporteFornecimento(carga)
            };
        }

        private Servicos.ServicoSAP.TransporteFornecimento.DT_TRANSPORTEFORNECIMENTOTransporteFornecimento ObterDadosTransporteFornecimento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new ServicoSAP.TransporteFornecimento.DT_TRANSPORTEFORNECIMENTOTransporteFornecimento
            {
                OPERACAO = "UPD",
                TKNUM = carga.CodigoCargaEmbarcador,
                DATBG = DateTime.Now.ToString("yyyyMMdd")
            };
        }

        private void ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioIntegracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(_unitOfWork);
            _configuracaoIntegracao = repositorioIntegracaoArcelorMittal.BuscarPrimeiroRegistro();

            if ((_configuracaoIntegracao == null) || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Arcelor Mittal");
        }

        #endregion

        #region Métodos de comunicação

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.EnviaConfirmacaoPedido PreencherConfirmacaoPedido(ContextoEtapa contexto, List<RequestSubtarefa> subtarefas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ConfiguracaoPedido> configuracaoPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ConfiguracaoPedido>();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido> objeto = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido>();

            if (contexto.Tarefa.Resultado != null && contexto.Tarefa.Resultado.Contains("pedidos") && contexto.Tarefa.Resultado["pedidos"].IsBsonArray)
            {
                var pedidosArray = contexto.Tarefa.Resultado["pedidos"].AsBsonArray;
                objeto = pedidosArray.Select(p => p.AsBsonDocument.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido>()).ToList();
            }

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            foreach (RequestSubtarefa subtarefa in subtarefas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido item = objeto.Find(o => o.Codigo == subtarefa.Id);

                string numeroPedido = item?.NumeroPedido ?? string.Empty;
                int protocolo = item?.Protocolo ?? 0;

                if (string.IsNullOrWhiteSpace(numeroPedido) && protocolo > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocolo);
                    if (pedido != null && !string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador))
                    {
                        numeroPedido = pedido.NumeroPedidoEmbarcador;
                    }
                }

                configuracaoPedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ConfiguracaoPedido
                {
                    NumeroPedido = numeroPedido,
                    Protocolo = protocolo,
                    Mensagem = subtarefa.Mensagem ?? string.Empty,
                    StatusEvento = subtarefa.Status == Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.StatusTarefa.Concluida ? "S" : "E",
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.EnviaConfirmacaoPedido
            {
                ProtocoloRequisicao = contexto.TarefaId,
                ConfiguracaoPedido = configuracaoPedidos
            };
        }

        public async Task IntegrarRetornoAdicionarPedidoEmLoteAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                ObterConfiguracaoIntegracao();

                HttpClient requisicao = CriarRequisicao(_configuracaoIntegracao.URLRetornoAdicionarPedidoEmLote, _configuracaoIntegracao.Usuario, _configuracaoIntegracao.Senha);

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.EnviaConfirmacaoPedido corpoRequisicao = PreencherConfirmacaoPedido(contexto, subtarefas);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracao.URLRetornoAdicionarPedidoEmLote, conteudoRequisicao).Result;
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                string conteudo = retornoRequisicao.Content.ReadAsStringAsync().Result;
                jsonRetorno = $"Status: {retornoRequisicao.StatusCode}";

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado ArcelorMittal.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com ArcelorMittal.");

                if (!string.IsNullOrWhiteSpace(conteudo))
                {
                    jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(conteudo).ToString(Formatting.Indented);
                    jsonRetorno = LimparRespostaComprovei(jsonRetorno);
                }

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                tarefaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                tarefaIntegracao.ProblemaIntegracao = excecao.Message;
                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoArcelorMittal");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a ArcelorMittal.";
            }
            finally
            {
                var arquivo = new Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao
                {
                    ArquivoRequisicao = !string.IsNullOrEmpty(jsonRequisicao) ? MongoDB.Bson.BsonDocument.Parse(jsonRequisicao) : null,
                    ArquivoResposta = !string.IsNullOrEmpty(jsonRetorno) ? MongoDB.Bson.BsonDocument.Parse(jsonRetorno) : null,
                    Tipo = "json"
                };

                await _repositorioTarefaIntegracao.AdicionarArquivoAsync(tarefaIntegracao.Id, arquivo, cancellationToken);

                var update = Builders<TarefaIntegracao>.Update
                    .Set(x => x.SituacaoIntegracao, tarefaIntegracao.SituacaoIntegracao)
                    .Set(x => x.DataIntegracao, tarefaIntegracao.DataIntegracao)
                    .Set(x => x.ProblemaIntegracao, tarefaIntegracao.ProblemaIntegracao)
                    .Set(x => x.Tentativas, tarefaIntegracao.Tentativas);

                await _repositorioTarefaIntegracao.AtualizarAsync(tarefaIntegracao.Id, update, cancellationToken);
            }
        }

        private string LimparRespostaComprovei(string jsonRetorno)
        {
            if (string.IsNullOrEmpty(jsonRetorno))
                return jsonRetorno;

            try
            {
                var respostaJson = Newtonsoft.Json.Linq.JToken.Parse(jsonRetorno);
                if (respostaJson["request_body"] != null)
                {
                    respostaJson["request_body"].Parent.Remove();
                    return respostaJson.ToString(Formatting.Indented);
                }
            }
            catch
            {
            }

            return jsonRetorno;
        }

        #endregion
    }
}