using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.InforDoc
{
    public class IntegracaoInforDoc
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoInforDoc(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoInforDoc repIntegracaoInforDoc = new Repositorio.Embarcador.Configuracoes.IntegracaoInforDoc(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc integracao = repIntegracaoInforDoc.Buscar();

            if (string.IsNullOrWhiteSpace(integracao?.URL))
            {
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Não foi configurada a integração com a InforDoc, por favor verifique.";
                return;
            }

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            string mensagem;
            bool sucesso = false;
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotoIntegracao.Canhoto;
                Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.Canhoto objetoCanhoto = ObterCanhoto(canhoto, _unitOfWork);
                if (objetoCanhoto == null)
                    throw new ServicoException("Não foi localizado o arquivo da imagem para envio.");

                HttpClient requisicao = CriarRequisicao(integracao);

                jsonRequisicao = JsonConvert.SerializeObject(objetoCanhoto, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(integracao.URL, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.RetornoCanhoto retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.RetornoCanhoto>(jsonRetorno);

                    if (retorno.Sucesso)
                    {
                        sucesso = true;
                        mensagem = "Integrado com sucesso. URL do Comprovante: " + retorno.URLComprovante;
                    }
                    else
                        mensagem = retorno.Mensagem + (!string.IsNullOrWhiteSpace(retorno.Detalhe) ? " - " + retorno.Detalhe : string.Empty);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(jsonRetorno))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.RetornoCanhoto retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.RetornoCanhoto>(jsonRetorno);
                        mensagem = retorno.Mensagem + (!string.IsNullOrWhiteSpace(retorno.Detalhe) ? " - " + retorno.Detalhe : string.Empty);
                    }
                    else
                        mensagem = $"Falha ao conectar no WS InforDoc: {retornoRequisicao.StatusCode}";
                }
            }
            catch (ServicoException ex)
            {
                mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao integrar com a InforDoc";
            }

            SalvarJsonIntegracao(canhotoIntegracao, jsonRequisicao, jsonRetorno, mensagem);
            canhotoIntegracao.ProblemaIntegracao = mensagem;

            if (sucesso)
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            else
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            repCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.Canhoto ObterCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                return null;

            byte[] bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.InforDoc.Canhoto()
            {
                Chave = canhoto.XMLNotaFiscal?.Chave ?? string.Empty,
                NomeArquivo = canhoto.NomeArquivo,
                Base64 = Convert.ToBase64String(bufferCanhoto)
            };
        }

        private void SalvarJsonIntegracao(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao, string jsonRequisicao, string jsonRetorno, string mensagemRetorno = null)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = canhotoIntegracao.DataIntegracao,
                Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : canhotoIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (canhotoIntegracao.ArquivosTransacao == null)
                canhotoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            canhotoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc integracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoInforDoc));

            requisicao.BaseAddress = new Uri(integracao.URL);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("X-API-Key", integracao.APIKey);

            return requisicao;
        }

        #endregion
    }
}
