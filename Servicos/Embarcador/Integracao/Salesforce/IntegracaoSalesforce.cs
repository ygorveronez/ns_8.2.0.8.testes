using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Salesforce
{
    public class IntegracaoSalesforce
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Construtores
        public IntegracaoSalesforce(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Métodos Privados
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSalesforce repIntegracaoSalesforce = new Repositorio.Embarcador.Configuracoes.IntegracaoSalesforce(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce configuracaoIntegracao = repIntegracaoSalesforce.BuscarPrimeiroRegistro();
            if (!(configuracaoIntegracao?.PossuiIntegracao ?? false))
                throw new ServicoException("Integração Salesforce não está habilitada.");
            return configuracaoIntegracao;
        }

        private CreateCasoDevolucao ObterObjetoIntegracao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NFD)
        {
            Repositorio.Embarcador.Pessoas.ClienteComplementar repClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(_unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repClienteComplementar.BuscarPorCliente(gestaoDevolucao.NotaFiscalOrigem?.Destinatario?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCarga(gestaoDevolucao.CargaOrigem?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(gestaoDevolucao.NotaFiscalOrigem?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidosXMLNotaFiscal?.FirstOrDefault();

            CreateCasoDevolucao objetoIntegracao = new CreateCasoDevolucao();

            objetoIntegracao.CpfCnpjCliente = gestaoDevolucao.NotaFiscalOrigem?.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty;
            objetoIntegracao.RazaoSocial = gestaoDevolucao.NotaFiscalOrigem?.Destinatario?.Nome ?? string.Empty;
            objetoIntegracao.ChaveNFe = gestaoDevolucao.NotaFiscalOrigem?.Chave ?? string.Empty;
            objetoIntegracao.ValorNFe = gestaoDevolucao.NotaFiscalOrigem?.Valor.ToString().Replace(",", ".") ?? string.Empty;
            objetoIntegracao.EstabelecimentoNF = gestaoDevolucao.NotaFiscalOrigem?.Filial?.SiglaFilial ?? string.Empty;
            objetoIntegracao.EmissaoNF = gestaoDevolucao.NotaFiscalOrigem?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty;
            objetoIntegracao.NotaFiscal = gestaoDevolucao.NotasFiscaisDeOrigem?.FirstOrDefault()?.XMLNotaFiscal.Numero.ToString() ?? "0";

            objetoIntegracao.ChaveNFD = NFD?.Chave ?? string.Empty;
            objetoIntegracao.ValorNFD = NFD?.Valor.ToString().Replace(",", ".") ?? string.Empty;
            objetoIntegracao.NotaFiscalDevolucao = NFD?.Numero.ToString() ?? string.Empty;
            objetoIntegracao.EstabelecimentoNFD = NFD?.Filial?.SiglaFilial ?? string.Empty;
            objetoIntegracao.EmissaoNFD = NFD?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty;
            objetoIntegracao.VolumeNFD = NFD?.Volumes.ToString();

            objetoIntegracao.IncoTerms = (pedidoXMLNotaFiscal?.CargaPedido.Pedido.UsarTipoTomadorPedido ?? false) ? pedidoXMLNotaFiscal.CargaPedido.Pedido.TipoTomador.ObterDescricao() : string.Empty;

            objetoIntegracao.Description = gestaoDevolucao.CargaOrigem?.CodigoCargaEmbarcador ?? string.Empty;
            objetoIntegracao.CnpjTransportadora = gestaoDevolucao.CargaOrigem?.Empresa?.CNPJ ?? string.Empty;

            objetoIntegracao.Regional = clienteComplementar?.EquipeVendasFP ?? string.Empty;
            objetoIntegracao.Br = clienteComplementar?.EscritorioVendas ?? string.Empty;

            objetoIntegracao.TipoRecusa = string.Empty;
            objetoIntegracao.AssuntoOcorrencia = string.Empty;
            objetoIntegracao.Motivo = string.Empty;

            if (chamado != null)
            {
                objetoIntegracao.TipoRecusa = chamado.CargaEntrega?.TipoDevolucao.ObterDescricao() ?? string.Empty;
                objetoIntegracao.AssuntoOcorrencia = chamado.CargaEntrega?.TipoDevolucao.ObterDescricao() ?? string.Empty;
                objetoIntegracao.Motivo = chamado.MotivoDaDevolucao?.Descricao ?? string.Empty;
            }

            objetoIntegracao.TituloOcorrencia = "Solicitação - Reembolso Recusa";
            objetoIntegracao.CaseReason = "Solicitação - Reembolso Recusa";
            objetoIntegracao.Status = "Aguardando Atendimento";
            objetoIntegracao.AreaDeAtendimentoAnterior = "Fila Pendente";
            objetoIntegracao.Type = "Solicitação";

            return objetoIntegracao;
        }

        private string GetToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce configuracaoIntegracao)
        {
            string urlToken = configuracaoIntegracao.URLBase + configuracaoIntegracao.URIToken;
            string token = string.Empty;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSalesforce));

            client.BaseAddress = new Uri(urlToken);
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", configuracaoIntegracao.ClientID),
                new KeyValuePair<string, string>("client_secret", configuracaoIntegracao.ClientSecret)
            });

            HttpResponseMessage result = client.PostAsync(urlToken, requestBody).Result;
            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            if (result.IsSuccessStatusCode)
            {
                ObjetoResponse retorno = JsonConvert.DeserializeObject<ObjetoResponse>(jsonResponse);
                token = retorno?.AccessToken ?? string.Empty;
            }
            else
            {
                ObjetoResponse retorno = JsonConvert.DeserializeObject<ObjetoResponse>(jsonResponse);
                throw new ServicoException("Falha na obtenção do Token:" + (retorno?.ErrorDescription ?? retorno?.Error ?? result.StatusCode.ToString()));
            }
            return token;
        }
        #endregion

        #region Métodos Públicos
        public void IntegrarDevolucaoNotificacaoCRM(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracaoArquivo>(_unitOfWork);

            SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            string problemaIntegracao = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                gestaoDevolucaoIntegracao.NumeroTentativas += 1;
                gestaoDevolucaoIntegracao.DataIntegracao = DateTime.Now;

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce configuracaoIntegracao = ObterConfiguracaoIntegracao();
                string token = GetToken(configuracaoIntegracao);
                if (string.IsNullOrEmpty(token))
                    throw new ServicoException("Falha ao obter o token. Verifique as credenciais nas configurações de integração.");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string endPoint = configuracaoIntegracao.URLBase + configuracaoIntegracao.URICasoDevolucao;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSalesforce));
                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("authorization", "Bearer " + token);

                foreach(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NFD in gestaoDevolucaoIntegracao.GestaoDevolucao.NotasFiscaisDevolucao.Select(nota => nota.XMLNotaFiscal).ToList())
                {
                    CreateCasoDevolucao objetoIntegracao = ObterObjetoIntegracao(gestaoDevolucaoIntegracao.GestaoDevolucao, NFD);

                    jsonRequest = JsonConvert.SerializeObject(objetoIntegracao, Formatting.Indented);
                    StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage result = client.PostAsync(endPoint, content).Result;
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    string json = jsonResponse.Trim('"').Replace("\\\"", "\"");
                    ObjetoResponse retorno = JsonConvert.DeserializeObject<ObjetoResponse>(json);

                    if (result.IsSuccessStatusCode && !json.ToUpper().Contains("ERROR"))
                    {
                        situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        problemaIntegracao = "Integrado com sucesso.";
                    }
                    else
                    {
                        situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        problemaIntegracao = "Erro na integração.";
                    }
                    servicoArquivoTransacao.Adicionar(gestaoDevolucaoIntegracao, jsonRequest, jsonResponse, "json", problemaIntegracao + " NFD: " + NFD.Numero.ToString());
                }
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoSalesforce");

                situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                problemaIntegracao = excecao.Message;
                servicoArquivoTransacao.Adicionar(gestaoDevolucaoIntegracao, jsonRequest, jsonResponse, "json", problemaIntegracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoSalesforce");

                situacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                problemaIntegracao = "Ocorreu uma falha ao comunicar com o Serviço da Salesforce.";
                servicoArquivoTransacao.Adicionar(gestaoDevolucaoIntegracao, jsonRequest, jsonResponse, "json", problemaIntegracao);
            }
            finally
            {
                Servicos.Auditoria.Auditoria.AuditarSemDadosUsuario(gestaoDevolucaoIntegracao, gestaoDevolucaoIntegracao.ProblemaIntegracao, _unitOfWork);
                gestaoDevolucaoIntegracao.SituacaoIntegracao = situacaoIntegracao;
                gestaoDevolucaoIntegracao.ProblemaIntegracao = problemaIntegracao;
                repositorioGestaoDevolucaoIntegracao.Atualizar(gestaoDevolucaoIntegracao);
            }
        }

        #endregion
    }

    public class CreateCasoDevolucao
    {
        public string CpfCnpjCliente { get; set; }
        public string RazaoSocial { get; set; }
        public string ChaveNFe { get; set; }
        public string ValorNFe { get; set; }
        public string ChaveNFD { get; set; }
        public string ValorNFD { get; set; }
        public string Description { get; set; }
        public string TipoRecusa { get; set; }
        public string Motivo { get; set; }
        public string Regional { get; set; }
        public string Br { get; set; }
        public string TituloOcorrencia { get; set; }
        public string AssuntoOcorrencia { get; set; }
        public string Status { get; set; }
        public string AreaDeAtendimentoAnterior { get; set; }
        public string Type { get; set; }
        public string CaseReason { get; set; }
        public string CnpjTransportadora { get; set; }
        public string NotaFiscal { get; set; }
        public string EstabelecimentoNF { get; set; }
        public string EmissaoNF { get; set; }
        public string IncoTerms { get; set; }
        public string NotaFiscalDevolucao { get; set; }
        public string EstabelecimentoNFD { get; set; }
        public string EmissaoNFD { get; set; }
        public string VolumeNFD { get; set; }
    }

    public class ObjetoResponse
    {
        [JsonProperty("Access_Token")]
        public string AccessToken { get; set; }

        [JsonProperty("Error")]
        public string Error { get; set; }

        [JsonProperty("Error_Description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }

        public string RecordTypeId { get; set; }
        public string CNPJ__c { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }
        public string CaseReason__c { get; set; }
        public string TipoDeDevolucao__c { get; set; }
        public string ValorNF__c { get; set; }
        public string SerieNF__c { get; set; }
        public string SerieNFD__c { get; set; }
        public string ValorNFD__c { get; set; }
        public string AreaAtendimento__c { get; set; }
        public string AssuntoOcorrencia__c { get; set; }
        public string Status { get; set; }
        public string OwnerId { get; set; }
        public string Subject { get; set; }
        public string Id { get; set; }
    }
}
