using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.SAD
{
    public sealed class IntegracaoSAD
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao _configuracaoIntegracao = null;

        #endregion

        #region Construtores

        public IntegracaoSAD(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion


        #region Métodos Privados Request

        private WebRequest CriaRequisicao(string url, string metodo, string body, List<(string Chave, string Valor)> headers = null, string contentType = "application/json")
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebRequest requisicao = WebRequest.Create(url);

            byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(body);

            requisicao.Method = metodo;
            requisicao.ContentType = contentType;

            foreach (var (Chave, Valor) in (headers ?? new List<(string Chave, string Valor)>()))
                requisicao.Headers[Chave] = Valor;

            requisicao.ContentLength = byteArrayDadosRequisicao.Length;

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();
            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao;
        }

        private HttpClient ObterClient(string url, List<(string Chave, string Valor)> headers = null)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSAD));

            client.BaseAddress = new Uri(url);
            foreach (var (Chave, Valor) in (headers ?? new List<(string Chave, string Valor)>()))
                client.DefaultRequestHeaders.Add(Chave, Valor);

            return client;
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
                Servicos.Log.TratarErro(webException, "Executar-Requisicao-Integracao-SAD");

                if (webException.Response == null)
                    throw new ServicoException("Falha ao processar o retorno da API");

                return (HttpWebResponse)webException.Response;
            }
        }

        private string ObterResposta(HttpWebResponse response)
        {
            string jsonDadosRetornoRequisicao;
            using (System.IO.Stream streamDadosRetornoRequisicao = response.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                jsonDadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }

            return jsonDadosRetornoRequisicao;
        }

        private (bool Sucesso, HttpStatusCode Status) ObterInformacoesStatusRetorno(HttpWebResponse retornoRequisicao)
        {
            (bool Sucesso, HttpStatusCode Status) retorno = (false, retornoRequisicao.StatusCode);

            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                retorno.Sucesso = true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                retorno.Sucesso = true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.PreconditionFailed)
                retorno.Sucesso = true;

            return retorno;
        }

        private (bool Sucesso, HttpStatusCode Status) ObterInformacoesStatusRetorno(System.Net.Http.HttpResponseMessage retornoRequisicao)
        {
            (bool Sucesso, HttpStatusCode Status) retorno = (false, retornoRequisicao.StatusCode);

            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                retorno.Sucesso = true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                retorno.Sucesso = true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.PreconditionFailed)
                retorno.Sucesso = true;

            return retorno;
        }

        #endregion

        #region Métodos Privados Integração

        private string ObterBodyRequisicaoIntegracaoBuscarSenha(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoBuscarSenha requisicaoIntegracao = ObterObjetoRequisicaoIntegracaoBuscarSenha(agendamentoColetaPedido);
            string body = JsonConvert.SerializeObject(requisicaoIntegracao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return body;
        }

        private string ObterBodyRequisicaoIntegracaoFinalizarAgenda(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoFinalizarAgenda requisicaoIntegracao = ObterObjetoRequisicaoIntegracaoFinalizarAgenda(agendamentoColetaPedido);
            string body = JsonConvert.SerializeObject(requisicaoIntegracao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return body;
        }

        private string ObterBodyRequisicaoIntegracaoCancelarAgenda(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoCancelarAgenda requisicaoIntegracao = ObterObjetoRequisicaoIntegracaoCancelarAgenda(agendamentoColetaPedido);
            string body = JsonConvert.SerializeObject(requisicaoIntegracao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return body;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RespostaIntegracao ObterBodyConvertidoRespostaIntegracao(string body)
        {
            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RespostaIntegracao>(body);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoBuscarSenha ObterObjetoRequisicaoIntegracaoBuscarSenha(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioJanelaDescarga = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioJanelaDescarga.BuscarPorCarga(agendamentoColetaPedido.AgendamentoColeta.Carga?.Codigo ?? 0);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoBuscarSenha
            {
                //TipoCarga = agendamentoColetaPedido.Pedido.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? "",
                TipoCarga = agendamentoColetaPedido.AgendamentoColeta?.Carga?.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? "",
                NumeroAgendamento = agendamentoColetaPedido.AgendamentoColeta?.Carga?.CodigoCargaEmbarcador ?? "",
                NumeroPedido = agendamentoColetaPedido.Pedido?.NumeroPedidoEmbarcador ?? "",
                Quantidade = agendamentoColetaPedido.VolumesEnviar.ToString(),
                DataJanela = cargaJanelaDescarregamento.InicioDescarregamento.ToString("yyyyMMdd"),
                HoraJanela = cargaJanelaDescarregamento.InicioDescarregamento.ToString("HHmmss"),
                FilialCarga = ObterCodigoIntegracaoFilialCarga(agendamentoColetaPedido)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoFinalizarAgenda ObterObjetoRequisicaoIntegracaoFinalizarAgenda(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoFinalizarAgenda
            {
                NumeroAgendamento = agendamentoColetaPedido.AgendamentoColeta.Carga.CodigoCargaEmbarcador ?? "",
                FilialCarga = ObterCodigoIntegracaoFilialCarga(agendamentoColetaPedido)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoCancelarAgenda ObterObjetoRequisicaoIntegracaoCancelarAgenda(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RequisicaoIntegracaoCancelarAgenda
            {
                NumeroAgendamento = agendamentoColetaPedido.AgendamentoColeta.Carga.CodigoCargaEmbarcador ?? "",
                FilialCarga = ObterCodigoIntegracaoFilialCarga(agendamentoColetaPedido)
            };
        }

        private List<(string Chave, string Valor)> ObterHeadersRequisicaoIntegracao(string token)
        {
            string tokenAcesso = token;

            var headers = new List<(string Chave, string Valor)>() {
                ValueTuple.Create("x-api-key", tokenAcesso),
            };

            return headers;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta FinalizarAgenda(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido, Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta retorno)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatario(agendamentoColetaPedido.Pedido.Destinatario.CPF_CNPJ);
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao configuracao = ObterConfiguracao(centroDescarregamento);

            string bodyIntegracao = ObterBodyRequisicaoIntegracaoFinalizarAgenda(agendamentoColetaPedido);
            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicaoIntegracao(configuracao.Token);

            WebRequest requisicao = CriaRequisicao(configuracao.UrlIntegracaoFinalizarAgenda, "POST", bodyIntegracao, headersIntegracao);
            HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

            string bodyResposta = ObterResposta(retornoRequisicao);

            Log.TratarErro($"URL: {configuracao.UrlIntegracaoFinalizarAgenda}\nHeaders:{JsonConvert.SerializeObject(headersIntegracao)}\nRequest:\n{bodyIntegracao}\n\nResponse:\n{bodyResposta}", "SAD");

            string conteudoRequisicao = $"{{\"Headers\":{JsonConvert.SerializeObject(headersIntegracao)},\n\"Request\":\n{bodyIntegracao}}}";
            string conteudoResposta = $"{{\"Headers\":{JsonConvert.SerializeObject(new { traceId = retornoRequisicao.Headers.Get("traceId") })},\n\"Response\":\n{bodyResposta}}}";

            (bool Sucesso, HttpStatusCode Status) informacoesStatusRetorno = ObterInformacoesStatusRetorno(retornoRequisicao);

            if (!informacoesStatusRetorno.Sucesso && informacoesStatusRetorno.Status != HttpStatusCode.PreconditionFailed)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = Utilidades.String.GetHttpStatusDescription((int)retornoRequisicao.StatusCode);
                retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
                retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";

                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                return retorno;
            }

            var respostaIntegracao = ObterBodyConvertidoRespostaIntegracao(bodyResposta);

            if (!string.IsNullOrWhiteSpace(respostaIntegracao.MensagemErro))
            {
                retorno.Sucesso = false;
                retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
                retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";
                retorno.Mensagem = respostaIntegracao.MensagemErro;

                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                return retorno;
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Sucesso";
            retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
            retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";

            agendamentoColetaPedido.DataIntegracao = DateTime.Now;
            agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
            servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
            repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta CancelarAgenda(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatarioEFilial(agendamentoColetaPedido.Pedido.Destinatario.CPF_CNPJ, cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Filial?.Codigo ?? 0);

            if (centroDescarregamento == null)
                centroDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatario(agendamentoColetaPedido.Pedido.Destinatario.CPF_CNPJ);

            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao configuracao = ObterConfiguracao(centroDescarregamento);

            string bodyIntegracao = ObterBodyRequisicaoIntegracaoCancelarAgenda(agendamentoColetaPedido);
            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicaoIntegracao(configuracao.Token);

            WebRequest requisicao = CriaRequisicao(configuracao.UrlIntegracaoCancelarAgenda, "POST", bodyIntegracao, headersIntegracao);
            HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

            string bodyResposta = ObterResposta(retornoRequisicao);

            Log.TratarErro($"URL: {configuracao.UrlIntegracaoCancelarAgenda}\nHeaders:{JsonConvert.SerializeObject(headersIntegracao)}\nRequest:\n{bodyIntegracao}\n\nResponse:\n{bodyResposta}", "SAD");

            string conteudoRequisicao = $"{{\"Headers\":{JsonConvert.SerializeObject(headersIntegracao)},\n\"Request\":\n{bodyIntegracao}}}";
            string conteudoResposta = $"{{\"Headers\":{JsonConvert.SerializeObject(new { traceId = retornoRequisicao.Headers.Get("traceId") })},\n\"Response\":\n{bodyResposta}}}";

            (bool Sucesso, HttpStatusCode Status) informacoesStatusRetorno = ObterInformacoesStatusRetorno(retornoRequisicao);

            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta();

            if (!informacoesStatusRetorno.Sucesso && informacoesStatusRetorno.Status != HttpStatusCode.PreconditionFailed)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = Utilidades.String.GetHttpStatusDescription((int)retornoRequisicao.StatusCode);
                retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
                retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";

                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                AdicionarArquivoTransacaoIntegracaoCancelamento(cargaCancelamentoCargaIntegracao, retorno.Mensagem, conteudoRequisicao, conteudoResposta, _unitOfWork);

                return retorno;
            }

            if (!EhJsonValido(bodyResposta))
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Retorno da integração está em formato inválido.";
                retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
                retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";

                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                AdicionarArquivoTransacaoIntegracaoCancelamento(cargaCancelamentoCargaIntegracao, retorno.Mensagem, conteudoRequisicao, conteudoResposta, _unitOfWork);

                return retorno;
            }

            var respostaIntegracao = ObterBodyConvertidoRespostaIntegracao(bodyResposta);

            if (!string.IsNullOrWhiteSpace(respostaIntegracao.MensagemErro))
            {
                retorno.Sucesso = false;
                retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
                retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";
                retorno.Mensagem = respostaIntegracao.MensagemErro;

                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                AdicionarArquivoTransacaoIntegracaoCancelamento(cargaCancelamentoCargaIntegracao, retorno.Mensagem, conteudoRequisicao, conteudoResposta, _unitOfWork);

                return retorno;
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Sucesso";
            retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
            retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";

            agendamentoColetaPedido.DataIntegracao = DateTime.Now;
            agendamentoColetaPedido.ProblemaIntegracao = retorno.Mensagem;
            servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
            repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

            AdicionarArquivoTransacaoIntegracaoCancelamento(cargaCancelamentoCargaIntegracao, retorno.Mensagem, conteudoRequisicao, conteudoResposta, _unitOfWork);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColetaPedido ObterSenhaAgendamentoAPI(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = ObterCentroDescarregamento(agendamentoColetaPedido);
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao configuracao = ObterConfiguracao(centroDescarregamento);

            string bodyIntegracao = ObterBodyRequisicaoIntegracaoBuscarSenha(agendamentoColetaPedido);
            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicaoIntegracao(configuracao.Token);

            //WebRequest requisicao = CriaRequisicao(configuracao.UrlIntegracaoBuscarSenha, "POST", bodyIntegracao, headersIntegracao);

            HttpClient client = ObterClient(configuracao.UrlIntegracaoBuscarSenha, headersIntegracao);

            var content = new StringContent(bodyIntegracao.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(configuracao.UrlIntegracaoBuscarSenha, content).Result;
            string bodyResposta = "";

            if (result.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                using (var stream = result.Content.ReadAsStreamAsync().Result)
                using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
                using (var reader = new System.IO.StreamReader(decompressionStream))
                {
                    bodyResposta = reader.ReadToEndAsync().Result;
                }
            }
            else
            {
                byte[] bytes = result.Content.ReadAsByteArrayAsync().Result;
                bodyResposta = Encoding.UTF8.GetString(bytes);
            }








            //HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

            //string bodyResposta = ObterResposta(jsonResponse);

            Log.TratarErro($"URL: {configuracao.UrlIntegracaoBuscarSenha}\nHeaders:{JsonConvert.SerializeObject(headersIntegracao)}\nRequest:\n{bodyIntegracao}\n\nResponse:\n{bodyResposta}", "SAD");

            result.Headers.TryGetValues("traceId", out IEnumerable<string> traceId);

            string conteudoRequisicao = $"{{\"Headers\":{JsonConvert.SerializeObject(headersIntegracao)},\n\"Request\":\n{bodyIntegracao}}}";
            string conteudoResposta = $"{{\"Headers\":{JsonConvert.SerializeObject(new { traceId = traceId })},\n\"Response\":\n{bodyResposta}}}";

            (bool Sucesso, HttpStatusCode Status) informacoesStatusRetorno = ObterInformacoesStatusRetorno(result);

            if (!informacoesStatusRetorno.Sucesso && informacoesStatusRetorno.Status != HttpStatusCode.PreconditionFailed)
            {
                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = Utilidades.String.GetHttpStatusDescription((int)result.StatusCode);
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                return RetornarObjetoPedidoIntegracao(false, Utilidades.String.GetHttpStatusDescription((int)result.StatusCode), string.Empty, agendamentoColetaPedido.Pedido.NumeroPedidoEmbarcador, agendamentoColetaPedido.Pedido.Codigo);
            }

            var respostaIntegracao = ObterBodyConvertidoRespostaIntegracao(bodyResposta);

            if (!string.IsNullOrWhiteSpace(respostaIntegracao.MensagemErro))
            {
                agendamentoColetaPedido.DataIntegracao = DateTime.Now;
                agendamentoColetaPedido.ProblemaIntegracao = respostaIntegracao.MensagemErro;
                servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
                repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

                return RetornarObjetoPedidoIntegracao(false, respostaIntegracao.MensagemErro, string.Empty, agendamentoColetaPedido.Pedido.NumeroPedidoEmbarcador, agendamentoColetaPedido.Pedido.Codigo);
            }

            agendamentoColetaPedido.DataIntegracao = DateTime.Now;
            agendamentoColetaPedido.ProblemaIntegracao = "Sucesso";
            servicoArquivoTransacao.Adicionar(agendamentoColetaPedido, conteudoRequisicao, conteudoResposta, "json");
            repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);

            return RetornarObjetoPedidoIntegracao(true, "Sucesso", respostaIntegracao.SenhaAgendamento, agendamentoColetaPedido.Pedido.NumeroPedidoEmbarcador, agendamentoColetaPedido.Pedido.Codigo);
        }

        private void AdicionarArquivoTransacaoIntegracaoCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao, string mensagem, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaCancelamentoCargaIntegracao.DataIntegracao, mensagem, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (cargaCancelamentoCargaIntegracao.ArquivosTransacao == null)
                cargaCancelamentoCargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCancelamentoCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }


        private Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, DateTime data, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColetaPedido RetornarObjetoPedidoIntegracao(bool status, string mensagem, string senha, string numeroPedido, int codigoPedido)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColetaPedido()
            {
                Mensagem = mensagem,
                NumeroPedido = numeroPedido,
                CorLinha = status ? CorGrid.Verde : CorGrid.Vermelho,
                CorFonte = !status ? CorGrid.Branco : "",
                Sucesso = status,
                CodigoPedido = codigoPedido,
                Senha = senha
            };
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao ObterConfiguracao(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento)
        {
            if (_configuracaoIntegracao != null)
                return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.BuscarPrimeiroRegistro();

            if (!(integracao?.PossuiIntegracaoSAD ?? false))
                throw new ServicoException("Não foram configurados os dados de integração com a SAD");

            Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioIntegracaoSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD integracaoSAD = centroDescarregamento != null ? repositorioIntegracaoSAD.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo) : null;

            if (integracaoSAD == null)
                integracaoSAD = repositorioIntegracaoSAD.BuscarRegistroSemCentroDescarregamento();

            if (integracaoSAD == null)
                throw new ServicoException("Não foram encontradas configurações de integração com a SAD");

            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao configuracaoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.ConfiguracaoIntegracao()
            {
                UrlIntegracaoBuscarSenha = integracaoSAD.URLIntegracaoSADBuscarSenha,
                UrlIntegracaoFinalizarAgenda = integracaoSAD.URLIntegracaoSADFinalizarAgenda,
                UrlIntegracaoCancelarAgenda = integracaoSAD.URLIntegracaoSADCancelarAgenda,
                Token = integracaoSAD.Token
            };

            return _configuracaoIntegracao = configuracaoIntegracao;
        }

        private string ObterCodigoIntegracaoFilialCarga(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = ObterCentroDescarregamento(agendamentoColetaPedido);

            if (centroDescarregamento == null)
                return "";

            if (centroDescarregamento.Filial == null)
                return centroDescarregamento.Destinatario.CodigoIntegracao;

            return centroDescarregamento.Filial.OutrosCodigosIntegracao.FirstOrDefault();
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento ObterCentroDescarregamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido)
        {
            if (agendamentoColetaPedido.Pedido.Destinatario == null)
                return null;

            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatarios(new List<double> { agendamentoColetaPedido.Pedido.Destinatario.CPF_CNPJ });

            if (centrosDescarregamento.All(cd => cd.Filial == null))
                return centrosDescarregamento.FirstOrDefault();

            Dominio.Entidades.Embarcador.Filiais.Filial filialCarga = agendamentoColetaPedido.AgendamentoColeta.Carga.Filial;

            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamentoFilial = centrosDescarregamento.Where(obj => obj.Filial != null && obj.Filial.Codigo == filialCarga?.Codigo).FirstOrDefault();

            if (centroDescarregamentoFilial != null)
                return centroDescarregamentoFilial;

            return centrosDescarregamento.Where(cd => cd.Filial == null).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta ObterSenhaAgendamento(List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedidos)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta()
            {
                NumeroAgenda = agendamentoColetaPedidos.FirstOrDefault().AgendamentoColeta.Carga.CodigoCargaEmbarcador,
                NumeroPedidos = agendamentoColetaPedidos.Count,
                Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColetaPedido>()
            };

            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido in agendamentoColetaPedidos)
                retorno.Pedidos.Add(ObterSenhaAgendamentoAPI(agendamentoColetaPedido));

            if (!retorno.Pedidos.All(obj => obj.Sucesso))
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Não foi possível obter a senha de todos os pedidos";
                retorno.CorLinha = retorno.Sucesso ? CorGrid.Verde : CorGrid.Vermelho;
                retorno.CorFonte = !retorno.Sucesso ? CorGrid.Branco : "";

                return retorno;
            }

            return FinalizarAgenda(agendamentoColetaPedidos.FirstOrDefault(), retorno);
        }

        public void CancelarAgendamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPrimeiroPorCargaFetchDestinatarioECarga(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo);
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas += 1;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta retorno = CancelarAgenda(agendamentoColetaPedido, cargaCancelamentoCargaIntegracao);

                if (retorno.Sucesso)
                {
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                }
                else
                {
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração.";
            }

            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        private bool EhJsonValido(string jsonString)
        {
            try
            {
                JToken.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        #endregion
    }
}
