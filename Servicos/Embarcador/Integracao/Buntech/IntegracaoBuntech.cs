using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Buntech
{
    public class IntegracaoBuntech
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoBuntech(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta> IntegrarEventoEntregaAsync(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBuntech repositorioIntegracaoBuntech = new(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech configuracaoIntegracaoBuntech = await repositorioIntegracaoBuntech.BuscarPrimeiroRegistroAsync();
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta respostaHttp = new();

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoBuntech?.URLAutenticacao))
                    throw new ServicoException("A integração de Ocorrência Buntech não está configurada.");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repXmlNotaFiscal.BuscarPorCargaEntrega(cargaEntregaEvento.CargaEntrega.Codigo);

                if (!notasFiscais.Any())
                    throw new ServicoException($"Entrega {cargaEntregaEvento.CargaEntrega.Codigo} não possui nota fiscal para integrar.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Request evento = ObterDadosEnvio(xmlNotaFiscal, cargaEntregaEvento);

                    respostaHttp = await ExecutarRequisicaoAsync(evento, configuracaoIntegracaoBuntech.URLAutenticacao);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.ObjetoRetorno objetoRetorno = new();
                    if (!string.IsNullOrEmpty(respostaHttp.conteudoResposta))
                        objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.ObjetoRetorno>(respostaHttp.conteudoResposta);

                    if (respostaHttp.sucesso)
                    {
                        respostaHttp.mensagem = "Integrado com sucesso.";
                    }
                    else if (respostaHttp.httpStatusCode == HttpStatusCode.Unauthorized)
                        throw new ServicoException("Integração não autorizada, verifique os dados de configuração!");
                    else if (string.IsNullOrWhiteSpace(respostaHttp.conteudoResposta))
                        throw new ServicoException("Problema ao integrar: " + respostaHttp.httpStatusCode);
                    else
                        throw new ServicoException("Problema ao integrar com a Buntech: " + (objetoRetorno.Mensagem?.ToLower() ?? string.Empty));
                }
            }
            catch (ServicoException excecao)
            {
                respostaHttp.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                respostaHttp.mensagem = "Ocorreu uma falha ao realizar a integração com a Buntech.";
                respostaHttp.conteudoResposta = "Ocorreu uma falha ao realizar a integração com a Buntech.";
            }

            return respostaHttp;
        }

        public async Task IntegrarProvisaoAsync(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoBuntech repositorioIntegracaoBuntech = new(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech configuracaoIntegracaoBuntech = repositorioIntegracaoBuntech.BuscarPrimeiroRegistro();
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta respostaHttp = new();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new(_unitOfWork);

            provisaoIntegracao.NumeroTentativas++;
            provisaoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repositorioDocumentoProvisao.BuscarPorProvisao(provisaoIntegracao.Provisao.Codigo);

                if (documentosProvisao.IsNullOrEmpty())
                    throw new ServicoException("Nenhum documento encontrado na provisão");

                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = documentosProvisao.FirstOrDefault();

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Provisao> provisoesBuntech = new();
                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisao)
                    provisoesBuntech.Add(ObterDadosProvisao(documento));

                respostaHttp = await ExecutarRequisicaoAsync(provisoesBuntech, configuracaoIntegracaoBuntech.URLProvisao);

                if (respostaHttp.sucesso)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.ResultadoIntegracaoProvisao> resposta = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.ResultadoIntegracaoProvisao>>(respostaHttp.conteudoResposta);
                    if (resposta.IsNullOrEmpty())
                        throw new ServicoException($"Nenhuma resposta obtida da API.");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Buntech.");

                provisaoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Buntech.";
                provisaoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                provisaoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                provisaoIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(provisaoIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioProvisaoIntegracao.Atualizar(provisaoIntegracao);
        }

        public async Task IntegrarCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new(_unitOfWork);

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoBuntech repositorioIntegracaoBuntech = new(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech configuracaoIntegracaoBuntech = await repositorioIntegracaoBuntech.BuscarPrimeiroRegistroAsync();

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoBuntech?.URLAutenticacao))
                    throw new ServicoException("A integração de Ocorrência Buntech não está configurada.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Request evento = ObterDadosEnvio(canhotoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta resposta = await ExecutarRequisicaoAsync(evento, configuracaoIntegracaoBuntech.URLAutenticacao);

                jsonRequisicao = resposta.conteudoRequisicao;
                jsonRetorno = resposta.conteudoResposta;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.ObjetoRetorno objetoRetorno = new();

                if (!string.IsNullOrEmpty(jsonRetorno))
                    objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.ObjetoRetorno>(jsonRetorno);

                if (resposta.sucesso)
                {
                    canhotoIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                    canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else if (resposta.httpStatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique os dados de configuração!");
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    throw new ServicoException($"Problema ao integrar: {resposta.httpStatusCode}");
                else
                    throw new ServicoException($"Problema ao integrar com a Buntech: {objetoRetorno.Mensagem?.ToLower() ?? string.Empty}");
            }
            catch (BaseException excecao)
            {
                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoBuntech");

                canhotoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Problema ao tentar integrar com Buntech.";
            }

            servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta> ExecutarRequisicaoAsync(object dadosRequisicao, string url)
        {
            HttpClient client = CriarRequisicao(url);

            JsonSerializerSettings jsonSerializerSettings = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented, jsonSerializerSettings);

            StringContent content = new(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(url, content).Result;
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = await ObterHttRequisicaoRespostaAsync(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta> ObterHttRequisicaoRespostaAsync(string jsonRequest, HttpResponseMessage result)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = await result.Content.ReadAsStringAsync(),
                extensaoResposta = "json",
                sucesso = result.IsSuccessStatusCode,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Provisao ObterDadosProvisao(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNotaFiscalECarga(documentoProvisao.XMLNotaFiscal.Codigo, documentoProvisao.Carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Provisao provisaoBuntech = new()
            {
                CodigoIntegracaoFilial = documentoProvisao.Filial.CodigoFilialEmbarcador,
                ChaveNotaFiscal = documentoProvisao.XMLNotaFiscal.Chave.ToString(),
                CodigoIntegracaoTipoOperacao = documentoProvisao.TipoOperacao.CodigoIntegracao,
                ValorFreteNotaFiscal = documentoProvisao.ValorProvisao,
                NumeroPedido = pedidoXMLNotaFiscal.CargaPedido.Pedido.Protocolo
            };

            return provisaoBuntech;
        }

        private HttpClient CriarRequisicao(string url)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBuntech));
            requisicao.BaseAddress = new Uri(url);

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Request ObterDadosEnvio(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Request request = new()
            {
                Filial = cargaEntregaEvento.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                Nota = xmlNotaFiscal.Numero.ToString() ?? string.Empty,
                Serie = xmlNotaFiscal.Serie?.ToString() ?? string.Empty,
                DataEntrega = cargaEntregaEvento.CargaEntrega?.DataConfirmacao?.ToString("dd/MM/yyyy") ??
                              cargaEntregaEvento.CargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy") ??
                              cargaEntregaEvento.CargaEntrega?.DataInicio?.ToString("dd/MM/yyyy") ??
                              DateTime.UtcNow.ToString("dd/MM/yyyy")
            };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Request ObterDadosEnvio(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Buntech.Request request = new()
            {
                Filial = canhotoIntegracao.Canhoto?.Carga?.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                Nota = canhotoIntegracao.Canhoto?.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                Serie = canhotoIntegracao.Canhoto?.XMLNotaFiscal?.Serie?.ToString() ?? string.Empty,
                DataEntrega = canhotoIntegracao.Canhoto?.DataEntregaNotaCliente?.ToString("dd/MM/yyyy") ?? DateTime.UtcNow.ToString("dd/MM/yyyy")
            };

            return request;
        }

        #endregion
    }
}
