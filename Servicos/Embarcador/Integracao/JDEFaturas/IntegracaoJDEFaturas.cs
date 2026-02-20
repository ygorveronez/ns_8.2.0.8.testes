using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Vedacit
{
    public class IntegracaoJDEFaturas
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJDEFaturas _configuracaoIntegracaoJDE;

        #endregion

        #region Constructores

        public IntegracaoJDEFaturas(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos
        //Cliente utiliza essa nomenclatura de fatura, mesmo sendo pagamentos em nosso sistema, e não faturas de fato.
        public void IntegrarPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            pagamentoIntegracao.NumeroTentativas++;
            pagamentoIntegracao.DataIntegracao = DateTime.Now;
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.Fatura fatura = ObterFatura(pagamentoIntegracao.Pagamento);

                respostaHttp = ExecutarRequisicao(fatura, "/api/v1/frete/fatura");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRetornoJDEFaturas retornoJDEFaturas = JsonConvert.DeserializeObject<DadosRetornoJDEFaturas>(respostaHttp.conteudoResposta);

                    if (!retornoJDEFaturas.Status)
                        throw new ServicoException($"Resposta API JDE: {retornoJDEFaturas.Mensagem}");
                }
                else
                    throw new ServicoException("Problema ao obter a resposta da API JDE.");

                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                pagamentoIntegracao.ProblemaIntegracao = $"Integrado com sucesso.";
            }
            catch (ServicoException ex)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = "Problema ao integrar com a API JDE";
            }

            servicoArquivoTransacao.Adicionar(pagamentoIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private void ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoJDEFaturas repositorioIntegracaoYpe = new Repositorio.Embarcador.Configuracoes.IntegracaoJDEFaturas(_unitOfWork);
            _configuracaoIntegracaoJDE = repositorioIntegracaoYpe.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracaoJDE == null || !_configuracaoIntegracaoJDE.PossuiIntegracao)
                throw new ServicoException("Integração com JDE Faturas não configurada");

            if (string.IsNullOrEmpty(_configuracaoIntegracaoJDE.Usuario) || string.IsNullOrEmpty(_configuracaoIntegracaoJDE.Senha))
                throw new ServicoException("Dados de autenticação não configurados");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.Fatura ObterFatura(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.Fatura objetoFatura = new Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.Fatura()
            {
                CNPJEmissor = pagamento?.Empresa?.CNPJ ?? string.Empty,
                CNPJTransportador = pagamento?.Empresa?.CNPJ ?? string.Empty,
                DataFatura = pagamento.DataInicial.HasValue ? pagamento.DataInicial.Value : DateTime.MinValue,
                DataVencimento = ObterDataVencimento(pagamento),
                FaturaCTes = ObterCTesFatura(pagamento),
                ID = pagamento.Codigo,
                NumeroFatura = pagamento.Numero.ToString(),
                Valor = pagamento.ValorPagamento
            };

            return objetoFatura;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.CTe> ObterCTesFatura(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.CTe> listaFaturaCTe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.CTe>();

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento dfa in pagamento.DocumentosFaturamento)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.CTe faturaCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.JDEFaturas.CTe()
                {
                    ChaveCTe = dfa.CTe?.Chave ?? string.Empty,
                    Valor = dfa.CTe?.ValorAReceber ?? 0,
                };

                listaFaturaCTe.Add(faturaCTe);
            }

            return listaFaturaCTe;
        }
        private DateTime ObterDataVencimento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {
            if (pagamento.Empresa == null)
                throw new ServicoException("Transportador não informado no pagamento, não é possível encontrar a condição de pagamento.");

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoComparacao = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento() { TipoPrazoPagamento = TipoPrazoPagamento.DataPagamento };

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamento = Servicos.Embarcador.Escrituracao.CondicaoPagamento.BuscarCondicaoFiltrada(pagamento.Empresa.Codigo, condicaoComparacao, _unitOfWork);

            if (condicaoPagamento == null)
                throw new ServicoException("Não foi possível enncontrar a condição de pagamento.");

            DateTime dataVencimento = Servicos.Embarcador.Escrituracao.CondicaoPagamento.CalculaDataPagamento(condicaoPagamento, pagamento.DataCriacao, _unitOfWork);

            return dataVencimento;
        }

        private HttpClient ObterHttpClient()
        {
            ObterConfiguracaoIntegracao();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoJDEFaturas));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("sistema-origem", "TMS");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.SetBasicAuthentication(_configuracaoIntegracaoJDE.Usuario, _configuracaoIntegracaoJDE.Senha);

            return client;
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicao, string endpointAcao)
        {
            HttpClient client = ObterHttpClient();

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(string.Concat(_configuracaoIntegracaoJDE.URLIntegracao.TrimEnd('/'), endpointAcao), content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private static HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        #endregion

    }
}