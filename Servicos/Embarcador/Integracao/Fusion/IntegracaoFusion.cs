using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
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

namespace Servicos.Embarcador.Integracao.Fusion
{
    public class IntegracaoFusion
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
        private readonly IRequestDocumentoRepository _repositorioRequestDocumento;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFusion _configuracaoIntegracao;

        #endregion

        #region Construtores

        public IntegracaoFusion(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IntegracaoFusion(Repositorio.UnitOfWork unitOfWork, IRequestDocumentoRepository repositorioRequestDocumento, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _repositorioRequestDocumento = repositorioRequestDocumento;
        }

        public IntegracaoFusion(Repositorio.UnitOfWork unitOfWork, IRequestSubtarefaRepository repositorioSubtarefa, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _repositorioSubtarefa = repositorioSubtarefa;
        }

        #endregion

        #region Métodos Publicos

        public async Task IntegrarRetornoConfirmacaoPedidosAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                ObterConfiguracaoIntegracao();

                HttpClient requisicao = CriaRequisicao();

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido.EnviaConfirmacaoPedido corpoRequisicao = PreencherConfirmacaoPedido(contexto, subtarefas);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoPedido, conteudoRequisicao).Result;
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(retornoRequisicao.Content.ReadAsStringAsync().Result).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Fusion.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com Fusion.");

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
                Log.TratarErro(excecao, "IntegracaoFusion");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Fusion.";
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

        public async Task IntegrarRetornoGerarCarregamentoAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                ObterConfiguracaoIntegracao();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga corpoRequisicao = await PreencherConfirmacaoFecharCarga(contexto, cancellationToken);

                HttpClient requisicao = CriaRequisicao();

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoCarga, conteudoRequisicao).Result;
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(retornoRequisicao.Content.ReadAsStringAsync().Result).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Fusion.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com Fusion.");

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
                Log.TratarErro(excecao, "IntegracaoFusion");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Fusion.";
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

        public async Task IntegrarRetornoGerarCarregamentoRoteirizacaoEmLoteAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                ObterConfiguracaoIntegracao();

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga corpoRequisicao = PreencherConfirmacaoFecharCargaCarregamentoRoteirizacaoEmLote(contexto, subtarefas);

                HttpClient requisicao = CriaRequisicao();

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = await requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoCarga, conteudoRequisicao);
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Fusion.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com Fusion.");

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
                Log.TratarErro(excecao, "IntegracaoFusion");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Fusion.";
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

        #endregion

        #region Métodos Comunicação

        private HttpClient CriaRequisicao()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoFusion));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("Token", _configuracaoIntegracao.Token);

            return requisicao;
        }

        #endregion

        #region Métodos Privados

        private void ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFusion repositorioConfiguracaoIntegracaoFusion = new Repositorio.Embarcador.Configuracoes.IntegracaoFusion(_unitOfWork);
            _configuracaoIntegracao = repositorioConfiguracaoIntegracaoFusion.BuscarPrimeiroRegistro();

            if ((_configuracaoIntegracao == null) || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para o Fusion");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLIntegracaoCarga) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.Token))
                throw new ServicoException("O URL Autenticação, URL Integração e APIKey devem estar preenchidos na configuração de integração do Fusion");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido.EnviaConfirmacaoPedido PreencherConfirmacaoPedido(ContextoEtapa contexto, List<RequestSubtarefa> subtarefas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido.ConfiguracaoPedido> configuracaoPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido.ConfiguracaoPedido>();
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

                configuracaoPedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido.ConfiguracaoPedido
                {
                    NumeroPedido = numeroPedido,
                    Protocolo = protocolo,
                    Mensagem = subtarefa.Mensagem ?? string.Empty,
                    Filial = item?.ProtocoloIntegracaoFilial ?? string.Empty
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoPedido.EnviaConfirmacaoPedido
            {
                ConfiguracaoPedido = configuracaoPedidos
            };
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga> PreencherConfirmacaoFecharCarga(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga envioConfirmacaoFecharCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga();
            
            RequestDocumento requestDoc = contexto.RequestDoc;
            if (requestDoc == null && !string.IsNullOrEmpty(contexto.Tarefa.RequestId))
            {
                requestDoc = await _repositorioRequestDocumento.ObterPorIdAsync(contexto.Tarefa.RequestId, cancellationToken);
            }
            
            Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento = requestDoc?.Dados?.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>();

            if (carregamento == null)
                return envioConfirmacaoFecharCarga;

            string protocoloCargaFechada = string.Empty;
            string mensagem = string.Empty;
            bool status = true;

            if (contexto.Tarefa.Resultado != null)
            {
                if (contexto.Tarefa.Resultado.Contains("protocolo_carga_fechada"))
                {
                    protocoloCargaFechada = contexto.Tarefa.Resultado["protocolo_carga_fechada"].ToString();
                }

                if (string.IsNullOrEmpty(protocoloCargaFechada) && contexto.Tarefa.Resultado.Contains("cargas") && contexto.Tarefa.Resultado["cargas"].IsBsonArray)
                {
                    var cargasArray = contexto.Tarefa.Resultado["cargas"].AsBsonArray;
                    if (cargasArray.Count > 0)
                    {
                        var primeiraCarga = cargasArray[0].AsBsonDocument.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoObjetoTarefa>();
                        
                        protocoloCargaFechada = primeiraCarga?.Protocolo ?? string.Empty;
                        status = primeiraCarga?.Status ?? true;
                        mensagem = primeiraCarga?.Mensagem ?? string.Empty;
                    }
                }
            }

            if (!string.IsNullOrEmpty(protocoloCargaFechada))
            {
                AdicionarConfirmacaoFecharCarga(envioConfirmacaoFecharCarga, carregamento, protocoloCargaFechada, status.ToString(), mensagem);
            }

            return envioConfirmacaoFecharCarga;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga PreencherConfirmacaoFecharCargaCarregamentoRoteirizacaoEmLote(ContextoEtapa contexto, List<RequestSubtarefa> subtarefas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga envioConfirmacaoFecharCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga
            {
                ConfirmacaoFecharCarga = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga>()
            };

            Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoObjetoTarefa> cargasResultado = new Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoObjetoTarefa>();

            if (contexto.Tarefa.Resultado != null && contexto.Tarefa.Resultado.Contains("cargas") && contexto.Tarefa.Resultado["cargas"].IsBsonArray)
            {
                var cargasArray = contexto.Tarefa.Resultado["cargas"].AsBsonArray;
                foreach (var cargaItem in cargasArray)
                {
                    if (cargaItem.IsBsonDocument)
                    {
                        var cargaDoc = cargaItem.AsBsonDocument;
                        var retornoObjeto = cargaDoc.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoObjetoTarefa>();
                        if (!string.IsNullOrEmpty(retornoObjeto?.Codigo))
                        {
                            cargasResultado[retornoObjeto.Codigo] = retornoObjeto;
                        }
                    }
                }
            }

            foreach (RequestSubtarefa subtarefa in subtarefas)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao carregamentoRoteirizacao = subtarefa.Dados?.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao>();
                Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoObjetoTarefa objeto = cargasResultado.ContainsKey(subtarefa.Id) ? cargasResultado[subtarefa.Id] : null;

                if (carregamentoRoteirizacao != null)
                {
                    envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga
                    {
                        NumeroCarregamento = carregamentoRoteirizacao.NumeroCarregamento,
                        ProtocoloIntegracao = objeto?.Protocolo ?? string.Empty,
                        Status = objeto?.Status.ToString() ?? string.Empty,
                        DataRetorno = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        CodigoMensagem = string.IsNullOrWhiteSpace(objeto?.Mensagem) ? CodigoMensagemRetorno.Sucesso.ToString() : CodigoMensagemRetorno.FalhaGenerica.ToString(),
                        Mensagem = objeto?.Mensagem ?? string.Empty,
                        Filial = carregamentoRoteirizacao.Filial?.CodigoIntegracao ?? string.Empty
                    });
                }
            }

            return envioConfirmacaoFecharCarga;
        }

        private void AdicionarConfirmacaoFecharCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga envioConfirmacaoFecharCarga, Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento, string protocoloIntegracao, string status, string mensagem)
        {
            if (envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga == null)
            {
                envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga>();
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga confirmacaoFecharCargas = new Dominio.ObjetosDeValor.Embarcador.Integracao.Fusion.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga
            {
                NumeroCarregamento = carregamento.NumeroCarregamento,
                ProtocoloIntegracao = protocoloIntegracao,
                Status = status,
                DataRetorno = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                CodigoMensagem = string.IsNullOrWhiteSpace(mensagem) ? CodigoMensagemRetorno.Sucesso.ToString() : CodigoMensagemRetorno.FalhaGenerica.ToString(),
                Mensagem = mensagem ?? string.Empty,
                Filial = carregamento.Filial.CodigoIntegracao
            };

            envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga.Add(confirmacaoFecharCargas);
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
