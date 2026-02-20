using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar;
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

namespace Servicos.Embarcador.Integracao.Olfar
{
    public class IntegracaoOlfar
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOlfar _configuracaoIntegracao;

        #endregion

        public IntegracaoOlfar(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Metodos Publicos

        public void IntegrarCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            ocorrenciaCTeIntegracao.NumeroTentativas++;
            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            try
            {

                Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.NotaDebito notaDebito = ObterNotaDebito(ocorrenciaCTeIntegracao);

                respostaHttp = ExecutarRequisicao(notaDebito, "/v1/trizy/");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK && respostaHttp.conteudoResposta.Contains("Retorno"))
                {
                    DadosRetornoOlfar respostaOlfar = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.DadosRetornoOlfar>>(respostaHttp.conteudoResposta).FirstOrDefault();

                    if (respostaOlfar.Retorno == "ERRO")
                        throw new ServicoException($"Resposta API Olfar: {respostaOlfar.MensagemRetorno}");

                    ocorrenciaCTeIntegracao.ProblemaIntegracao = !string.IsNullOrEmpty(respostaOlfar.MensagemRetorno) ? respostaOlfar.MensagemRetorno : "Sucesso ao comunicar com API Olfar.";
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
                else
                    throw new ServicoException("Problema ao obter a resposta da API Olfar.");
            }
            catch (ServicoException ex)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Problema ao integrar com API Olfar";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(_unitOfWork);
            faturaIntegracao.Tentativas++;
            faturaIntegracao.DataEnvio = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.Fatura fatura = ObterFatura(faturaIntegracao);

                respostaHttp = ExecutarRequisicao(fatura, "/v1/trizy/cte/");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK && respostaHttp.conteudoResposta.Contains("Retorno"))
                {
                    DadosRetornoOlfar respostaOlfar = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.DadosRetornoOlfar>>(respostaHttp.conteudoResposta).FirstOrDefault();

                    if (respostaOlfar.Retorno == "ERRO")
                        throw new ServicoException($"Resposta API Olfar: {respostaOlfar.MensagemRetorno}");
                }
                else
                    throw new ServicoException("Problema ao obter a resposta da API Olfar.");

                faturaIntegracao.MensagemRetorno = "Sucesso ao comunicar com API Olfar.";
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (ServicoException ex)
            {
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                faturaIntegracao.MensagemRetorno = "Problema ao integrar com API Olfar";
            }

            servicoFatura.SalvarArquivosIntegracaoFatura(faturaIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta);

            repFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        #endregion

        #region Metodos Privados

        Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.NotaDebito ObterNotaDebito(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(_unitOfWork);
            Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cteRelacao = repRelacaoDocumento.BuscarPorCTeGerado(ocorrenciaCTeIntegracao.CargaCTe.CTe?.Codigo ?? 0);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.NotaDebito
            {
                Parametro = "FATNotaDebito",
                CodigoEmpresa = 1,
                CodigoFilial = ocorrenciaCTeIntegracao.CargaOcorrencia.Carga?.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                CodigoTransportador = ocorrenciaCTeIntegracao.CargaCTe.Carga?.Empresa?.CodigoIntegracao ?? string.Empty,
                NumeroTitulo = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Numero.ToString() ?? string.Empty,
                DataVencimento = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy"),
                Valor = (ocorrenciaCTeIntegracao.CargaCTe.CTe?.ValorAReceber ?? 0).ToString("F2").Replace('.', ','),
                Observacao = ocorrenciaCTeIntegracao.CargaCTe.CTe?.ObservacoesGerais ?? string.Empty,
                ChaveCTeReferenciado = cteRelacao?.CTeOriginal.ChaveAcesso ?? string.Empty,
                CodigoUsuarioGerador = 429
            };
        }

        Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.Fatura ObterFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.Fatura
            {
                Parametro = "FATGerar",
                CodigoEmpresa = 1,
                CodigoFilial = faturaIntegracao.Fatura.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                CodigoTransportador = faturaIntegracao.Fatura.Transportador?.CodigoIntegracao ?? string.Empty,
                CodigoCondicaoPagamentoFatura = "01X",
                CodigoFormaPagamento = 6,
                Valor = faturaIntegracao.Fatura.Total.ToString(),
                CodigoUsuarioGerador = "429",
                CTes = ObterCTesFatura(faturaIntegracao.Fatura)
            };
        }

        List<Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.CTe> ObterCTesFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.CTe> objetoIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.CTe>();
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = fatura.Documentos.Select(obj => obj.Documento.CTe).ToList();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                if (cte.ModeloDocumentoFiscal.Abreviacao == "ND")
                {
                    Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cteRelacao = repRelacaoDocumento.BuscarPorCTeGerado(cte.Codigo);
                    objetoIntegracao.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.CTe
                    {
                        ChaveCTe = cteRelacao.CTeOriginal.ChaveAcesso
                    });
                }
                else
                    objetoIntegracao.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.CTe
                    {
                        ChaveCTe = cte.ChaveAcesso
                    });
            }
            return objetoIntegracao;
        }
        #endregion

        #region Metodos de Requisicao
        private void ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoOlfar repositorioIntegracaoOlfar = new Repositorio.Embarcador.Configuracoes.IntegracaoOlfar(_unitOfWork);
            _configuracaoIntegracao = repositorioIntegracaoOlfar.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracao == null || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Integração com a Olfar não foi configurada");

            if (string.IsNullOrEmpty(_configuracaoIntegracao.URLIntegracao))
                throw new ServicoException("URL da integração com a Olfar não foi configurada");

        }


        private HttpClient ObterHttpClient()
        {
            ObterConfiguracaoIntegracao();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoOlfar));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

            return client;
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

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicao, string endpointAcao)
        {
            HttpClient client = ObterHttpClient();

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(string.Concat(_configuracaoIntegracao.URLIntegracao.TrimEnd('/'), endpointAcao), content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }
        #endregion
    }
}
