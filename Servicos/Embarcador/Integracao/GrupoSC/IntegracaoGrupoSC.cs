using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Infrastructure.Services.HttpClientFactory;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.GrupoSC
{
    public class IntegracaoGrupoSC
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly ITarefaIntegracao _repositorioTarefaIntegracao;
        private readonly IRequestDocumentoRepository _repositorioRequestDocumento;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoSC _configuracaoIntegracao;

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        private readonly Servicos.CTe _servicoCTe;

        #endregion

        #region Construtores

        public IntegracaoGrupoSC(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _servicoCTe = new Servicos.CTe(unitOfWork);
        }

        public IntegracaoGrupoSC(Repositorio.UnitOfWork unitOfWork, IRequestDocumentoRepository repositorioRequestDocumento, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _servicoCTe = new Servicos.CTe(unitOfWork);
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _repositorioRequestDocumento = repositorioRequestDocumento;
        }

        public IntegracaoGrupoSC(Repositorio.UnitOfWork unitOfWork, IRequestSubtarefaRepository repositorioSubtarefa, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _servicoCTe = new Servicos.CTe(unitOfWork);
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _repositorioSubtarefa = repositorioSubtarefa;
        }

        #endregion

        #region Métodos Comunicação

        private HttpClient CriaRequisicao()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGrupoSC));

            requisicao.BaseAddress = new Uri(_configuracaoIntegracao.URLIntegracao);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("APIkey", _configuracaoIntegracao.ApiKey);

            return requisicao;
        }

        #endregion

        #region Métodos Privados

        private void ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC repositorioConfiguracaoIntegracaoGrupoSC = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC(_unitOfWork);
            _configuracaoIntegracao = repositorioConfiguracaoIntegracaoGrupoSC.BuscarPrimeiroRegistro();

            if ((_configuracaoIntegracao == null) || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para o GrupoSC");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLIntegracao) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.ApiKey))
                throw new ServicoException("O URL Autenticação, URL Integração e APIKey devem estar preenchidos na configuração de integração do GrupoSC");
        }

        private async Task ObterConfiguracaoIntegracaoAsync()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC repositorioConfiguracaoIntegracaoGrupoSC = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC(_unitOfWork);
            _configuracaoIntegracao = await repositorioConfiguracaoIntegracaoGrupoSC.BuscarPrimeiroRegistroAsync();

            if ((_configuracaoIntegracao == null) || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para o GrupoSC");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLIntegracao) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.ApiKey))
                throw new ServicoException("O URL Autenticação, URL Integração e APIKey devem estar preenchidos na configuração de integração do GrupoSC");
        }

        private void EnviarArquivosIntegracao<T>(T envio, string url, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao, out string mensagemErroIntegracao, out HttpStatusCode statusCodeRetornoRequisicao)
        {
            jsonDadosRequisicao = string.Empty;
            jsonDadosRetornoRequisicao = string.Empty;
            mensagemErroIntegracao = string.Empty;
            statusCodeRetornoRequisicao = HttpStatusCode.OK;

            try
            {
                jsonDadosRequisicao = JsonConvert.SerializeObject(envio, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonDadosRequisicao, Encoding.UTF8, "application/json");

                HttpClient client = CriaRequisicao();

                HttpResponseMessage retornoRequisicao = client.PostAsync(url, conteudoRequisicao).Result;

                jsonDadosRetornoRequisicao = retornoRequisicao.Content.ReadAsStringAsync().Result;

                statusCodeRetornoRequisicao = retornoRequisicao.StatusCode;

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    Log.TratarErro($"Requisição: {jsonDadosRequisicao}\nResposta: {jsonDadosRetornoRequisicao}", "IntegracaoGrupoSC");
                    return;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoGrupoSC");
                mensagemErroIntegracao = "Ocorreu uma falha ao enviar documento";
            }

            Log.TratarErro($"Requisição: {jsonDadosRequisicao}\nResposta: {jsonDadosRetornoRequisicao}", "IntegracaoGrupoSC");
        }

        private bool IsRetornoSucesso(HttpResponseMessage retornoRequisicao)
        {
            if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                return true;

            return false;
        }

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
                    Mensagem = subtarefa.Mensagem ?? string.Empty
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.EnviaConfirmacaoPedido
            {
                ConfiguracaoPedido = configuracaoPedidos
            };
        }

        private void IntegrarCTesCarga(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao, out string mensagemErroIntegracao, out HttpStatusCode statusCodeRetornoRequisicao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.DocumentoCTe> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.DocumentoCTe>();
            Servicos.CTe svcCTE = new Servicos.CTe(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                string xmlString = svcCTE.ObterStringXMLAutorizacao(integracaoEnvioProgramado.CTe, _unitOfWork);

                string base64Xml = !string.IsNullOrEmpty(xmlString)
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlString))
                    : throw new ServicoException("O XML não pode ser nulo ou vazio.");

                if (integracaoEnvioProgramado.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                    documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.DocumentoCTe()
                    {
                        NumeroCTe = integracaoEnvioProgramado.CTe.Numero.ToString(),
                        Chave = integracaoEnvioProgramado.CTe.Chave,
                        Base64CTe = base64Xml,
                        ProtocoloCargaTMS = integracaoEnvioProgramado.Carga.Codigo.ToString(),
                        ProtocoloCTeTMS = integracaoEnvioProgramado.CTe.Codigo.ToString(),
                        TabelaFrete = integracaoEnvioProgramado.Carga.TabelaFrete?.Descricao,
                        DataVigencia = integracaoEnvioProgramado.Carga.TabelaFrete?.Vigencias.Select(x => x.DataFinal?.ToString("yyyyMMdd")).FirstOrDefault(),
                        NumeroCTeComplementadoTMS = string.Empty,
                        NumeroOC = string.Empty,
                        ProtocoloCTeComplementadoTMS = string.Empty
                    });

                if (integracaoEnvioProgramado.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                {
                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarPorChaveCTeAnterior(integracaoEnvioProgramado.CTe.ChaveCTESubComp);

                    documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.DocumentoCTe()
                    {
                        NumeroCTe = integracaoEnvioProgramado.CTe.Numero.ToString(),
                        Chave = integracaoEnvioProgramado.CTe.Chave,
                        Base64CTe = base64Xml,
                        ProtocoloCargaTMS = integracaoEnvioProgramado.Carga.Codigo.ToString(),
                        ProtocoloCTeTMS = integracaoEnvioProgramado.CTe.Codigo.ToString(),
                        ProtocoloCTeComplementadoTMS = cargaCTeComplementoInfo.CTeComplementado.Codigo.ToString(),
                        NumeroCTeComplementadoTMS = cargaCTeComplementoInfo.CTeComplementado.Numero.ToString(),
                        NumeroOC = cargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia.ToString(),
                        DataVigencia = string.Empty,
                        TabelaFrete = string.Empty
                    });
                }
            }

            ObterConfiguracaoIntegracao();

            string url = _configuracaoIntegracao.URLIntegracao + "/TMS/EnviaCTe";

            EnviarArquivosIntegracao(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioCTe()
            {
                envia_cte = documentos
            }, url, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao, out statusCodeRetornoRequisicao);
        }

        private void IntegrarNFSesCarga(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao, out string mensagemErroIntegracao, out HttpStatusCode statusCodeRetornoRequisicao)
        {
            List<DocumentoNFSe> documentos = new List<DocumentoNFSe>();
            Servicos.CTe servicoCTe = new Servicos.CTe(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                byte[] xmlBytes = servicoCTe.ObterXMLAutorizacao(integracaoEnvioProgramado.CTe, _unitOfWork);
                string base64Xml = Convert.ToBase64String(xmlBytes);

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.DocumentoNFSe()
                {
                    NumeroNFSe = integracaoEnvioProgramado.CTe.Numero.ToString(),
                    Chave = integracaoEnvioProgramado.CTe.Chave,
                    Base64 = base64Xml,
                    ProtocoloCargaTMS = integracaoEnvioProgramado.Carga.Codigo.ToString(),
                    ProtocoloNFSeTMS = integracaoEnvioProgramado.CTe.Codigo.ToString(),
                    TabelaFrete = integracaoEnvioProgramado.Carga.TabelaFrete.Descricao,
                    DataVigencia = integracaoEnvioProgramado.Carga.TabelaFrete.Vigencias.Select(x => x.DataFinal?.ToString("yyyyMMdd")).FirstOrDefault(),
                    NumeroNFSeComplementadoTMS = string.Empty,
                    NumeroOC = string.Empty,
                    ProtocoloNFSeComplementadoTMS = string.Empty,
                });
            }

            ObterConfiguracaoIntegracao();

            string url = _configuracaoIntegracao.URLIntegracao + "/TMS/EnviaNFSe";

            EnviarArquivosIntegracao(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioNFSe()
            {
                envia_nfse = documentos
            }
            , url, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao, out statusCodeRetornoRequisicao);
        }

        private void IntegrarOutrosDocumentosCarga(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao, out string mensagemErroIntegracao, out HttpStatusCode statusCodeRetornoRequisicao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.OutrosDocumentos> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.OutrosDocumentos>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repositorioCargaCTeComplementoInfo.BuscarPorOcorrencias(integracoesEnvioProgramado.Select(obj => obj.CargaOcorrencia.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = cargaCTeComplementoInfos.Find(obj => obj.CTe.Codigo == integracaoEnvioProgramado.CTe.Codigo);

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.OutrosDocumentos()
                {
                    DataEmissao = integracaoEnvioProgramado.CTe.DataEmissao.Value.ToString("yyyyMMdd"),
                    NumeroND = integracaoEnvioProgramado.CTe.Numero.ToString(),
                    SerieND = integracaoEnvioProgramado.CTe.Serie.Numero.ToString(),
                    ProtocoloND = integracaoEnvioProgramado.CTe.Codigo.ToString(),
                    Transportador = integracaoEnvioProgramado.Carga.Empresa.CNPJ,
                    OcorrenciaMulti = integracaoEnvioProgramado.CargaOcorrencia.NumeroOcorrencia.ToString(),
                    Carga = integracaoEnvioProgramado.Carga.CodigoCargaEmbarcador,
                    ProtocoloCargaTMS = integracaoEnvioProgramado.Carga.Protocolo.ToString(),
                    Motivo = integracaoEnvioProgramado.CargaOcorrencia.TipoOcorrencia.CodigoIntegracao,
                    DescricaoMotivo = integracaoEnvioProgramado.CargaOcorrencia.TipoOcorrencia.Descricao,
                    Problema = integracaoEnvioProgramado.CargaOcorrencia.Observacao,
                    CTeOrigem = cargaCTeComplementoInfo.CargaCTeComplementado.CTe.Numero.ToString(),
                    SerieOrigem = cargaCTeComplementoInfo.CargaCTeComplementado.CTe.Serie.Numero.ToString(),
                    NFeOrigem = integracaoEnvioProgramado.CargaOcorrencia.NotasFiscaisCTesOriginarios,
                    ValorDocumento = integracaoEnvioProgramado.CargaOcorrencia.ValorOcorrencia.ToString("F2", CultureInfo.InvariantCulture),
                    QuantidadeParcela = integracaoEnvioProgramado.CargaOcorrencia.QuantidadeParcelas.ToString(),
                    PeriodoPagamento = integracaoEnvioProgramado.CargaOcorrencia.PeriodoPagamento.ObterValorOuPadrao(),
                });
            }

            ObterConfiguracaoIntegracao();

            string url = _configuracaoIntegracao.URLIntegracao + "/TMS/EnviaCFD";

            EnviarArquivosIntegracao(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioOutrosDocumentos()
            {
                OutrosDocumentos = documentos
            }
            , url, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao, out statusCodeRetornoRequisicao);
        }

        private bool IntegrarPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, out string jsonDadosRequisicao, out string jsonDadosRetornoRequisicao, out string mensagemErroIntegracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentofaturamento = new(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoNFSe.NFSeConfirmacao> documentosNFSe = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoNFSe.NFSeConfirmacao>();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoCTe.CTeConfirmacao> documentosCte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoCTe.CTeConfirmacao>();
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repositorioDocumentofaturamento.BuscarDocumentosFaturamentoLiberadosPorPagamento(pagamentoIntegracao.Pagamento.Codigo);

            if (pagamentoIntegracao.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                {
                    if (documentoFaturamento.ModeloDocumentoFiscal.TipoDocumentoEmissao == pagamentoIntegracao.TipoDocumento)
                        documentosNFSe.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoNFSe.NFSeConfirmacao()
                        {
                            Codigo = "2020",
                            DataHora = DateTime.Now.ToString("yyyyMMdd HH:mm:ss"),
                            Evento = "LIBERAR PAGAMENTO",
                            ProtocoloNFSe = documentoFaturamento.CTe.Codigo.ToString(),
                        });
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                {
                    if (documentoFaturamento.ModeloDocumentoFiscal.TipoDocumentoEmissao == pagamentoIntegracao.TipoDocumento)
                        documentosCte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoCTe.CTeConfirmacao()
                        {
                            Codigo = "2020",
                            DataHora = DateTime.Now.ToString("yyyyMMdd HH:mm:ss"),
                            Evento = "LIBERAR PAGAMENTO",
                            ProtocoloCTe = documentoFaturamento.CTe.Codigo.ToString(),
                        });
                }
            }

            jsonDadosRequisicao = string.Empty;
            jsonDadosRetornoRequisicao = string.Empty;
            mensagemErroIntegracao = string.Empty;

            try
            {
                ObterConfiguracaoIntegracao();

                if (pagamentoIntegracao.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    jsonDadosRequisicao = JsonConvert.SerializeObject(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoNFSe.EnvioConfirmacaoNFSe() { EnviaConfirmacaoNFSeLista = documentosNFSe }, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                }
                else
                {
                    jsonDadosRequisicao = JsonConvert.SerializeObject(new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoCTe.EnvioConfirmacaoCTe() { EnviaConfirmacaoCteLista = documentosCte }, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                }

                StringContent conteudoRequisicao = new StringContent(jsonDadosRequisicao, Encoding.UTF8, "application/json");

                HttpClient client = CriaRequisicao();

                HttpResponseMessage retornoRequisicao;

                if (pagamentoIntegracao.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    retornoRequisicao = client.PostAsync(_configuracaoIntegracao.URLIntegracao + "/TMS/EnviaConfirmacoesNFSe", conteudoRequisicao).Result;
                }
                else
                {
                    retornoRequisicao = client.PostAsync(_configuracaoIntegracao.URLIntegracao + "/TMS/EnviaConfirmacoesCTe", conteudoRequisicao).Result;
                }

                retornoRequisicao.EnsureSuccessStatusCode();

                System.Threading.Tasks.Task<byte[]> byteArrayTask = retornoRequisicao.Content.ReadAsByteArrayAsync();
                byteArrayTask.Wait();
                byte[] byteArray = byteArrayTask.Result;

                jsonDadosRetornoRequisicao = Encoding.UTF8.GetString(byteArray);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    Log.TratarErro($"Requisição: {jsonDadosRequisicao}\nResposta: {jsonDadosRetornoRequisicao}", "IntegracaoGrupoSC");
                    return false;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoGrupoSC");
                mensagemErroIntegracao = "Ocorreu uma falha ao enviar documento";

                return false;
            }

            Log.TratarErro($"Requisição: {jsonDadosRequisicao}\nResposta: {jsonDadosRetornoRequisicao}", "IntegracaoGrupoSC");

            return true;
        }

        private List<(string Chave, string Mensagem)> ObtemCTesComErro(string jsonDadosRetornoRequisicao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioCTeRetorno jsonRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioCTeRetorno>(jsonDadosRetornoRequisicao);

            if (jsonRetorno == null)
                return new();

            return jsonRetorno.DocumentosCTeRetorno
                .Where(cte => cte.Status != "S")
                .Select(cte => (cte.Chave, cte.Mensagem))
                .ToList();
        }

        private List<(string ProtocoloPrefeitura, string Mensagem)> ObtemNFSesComErro(string jsonDadosRetornoRequisicao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioNFSeRetorno jsonRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioNFSeRetorno>(jsonDadosRetornoRequisicao);

            if (jsonRetorno == null)
                return new();

            return jsonRetorno.DocumentosNFSeRetorno
                .Where(nfse => nfse.Status != "S")
                .Select(nfse => (nfse.ProtocoloPrefeitura, nfse.Mensagem))
                .ToList();
        }

        private List<(string NumeroND, string SerieND, string Mensagem)> ObtemOutrosDocumentosComErro(string jsonDadosRetornoRequisicao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioOutrosDocumentosRetorno jsonRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.EnvioOutrosDocumentosRetorno>(jsonDadosRetornoRequisicao);

            if (jsonRetorno == null)
                return new();

            return jsonRetorno.Retorno
                .Where(outroDocumento => outroDocumento.Status != "S")
                .Select(outroDocumento => (outroDocumento.NumeroND, outroDocumento.SerieND, outroDocumento.Mensagem))
                .ToList();
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga> PreencherConfirmacaoFecharCarga(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga envioConfirmacaoFecharCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga();
            
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
                AdicionarConfirmacaoFecharCarga(envioConfirmacaoFecharCarga, carregamento.NumeroCarregamento, protocoloCargaFechada, status.ToString(), mensagem);
            }

            return envioConfirmacaoFecharCarga;
        }

        private void AdicionarConfirmacaoFecharCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga envioConfirmacaoFecharCarga, string numeroCarregamento, string protocoloIntegracao, string status, string mensagem)
        {
            if (envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga == null)
            {
                envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga>();
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga confirmacaoFecharCargas = new Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.ConfirmacaoFecharCarga
            {
                NumeroCarregamento = numeroCarregamento,
                ProtocoloIntegracao = protocoloIntegracao,
                Status = status,
                DataRetorno = ObterDataHoraBrasilia(),
                CodigoMensagem = "200",
                Mensagem = mensagem ?? string.Empty
            };

            envioConfirmacaoFecharCarga.ConfirmacaoFecharCarga.Add(confirmacaoFecharCargas);
        }

        private string ObterDataHoraBrasilia()
        {
            TimeZoneInfo brasiliaZone;

            try
            {
                brasiliaZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    brasiliaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
                }
                catch (TimeZoneNotFoundException)
                {
                    brasiliaZone = TimeZoneInfo.CreateCustomTimeZone(
                        "Brasilia Standard Time",
                        new TimeSpan(-3, 0, 0),
                        "Brasilia Standard Time",
                        "Brasilia Standard Time");
                }
            }

            DateTime brasiliaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaZone);
            return brasiliaTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCTes(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            DateTime dataIntegracao = DateTime.Now;
            SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            string problemaIntegracao = string.Empty;
            string jsonDadosRequisicao = string.Empty;
            string jsonDadosRetornoRequisicao = string.Empty;
            string mensagemErroIntegracao = "Integrado com sucesso";
            HttpStatusCode statusCodeRetornoRequisicao = HttpStatusCode.InternalServerError;

            bool sucesso = false;

            try
            {
                IntegrarCTesCarga(integracoesEnvioProgramado, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao, out statusCodeRetornoRequisicao);
            }
            catch (ServicoException excecao)
            {
                problemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoGrupoSC");

                problemaIntegracao = "Ocorreu uma falha ao realizar a integração com a GrupoSC";
            }

            List<(string Chave, string Mensagem)> ctesComErro = new();

            if (statusCodeRetornoRequisicao != HttpStatusCode.InternalServerError)
                ctesComErro = ObtemCTesComErro(jsonDadosRetornoRequisicao);
            else if (string.IsNullOrEmpty(problemaIntegracao))
                problemaIntegracao = "Retorno integração GrupoSC: " + jsonDadosRetornoRequisicao;

            if (ctesComErro?.Count == 0 && string.IsNullOrEmpty(problemaIntegracao))
                sucesso = true;

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                integracaoEnvioProgramado.DataIntegracao = dataIntegracao;
                integracaoEnvioProgramado.NumeroTentativas++;

                (string Chave, string Mensagem)? cteErro = ctesComErro?.Find(x => x.Chave == integracaoEnvioProgramado.CTe.Chave);
                if (sucesso && string.IsNullOrWhiteSpace(problemaIntegracao))
                {
                    integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoEnvioProgramado.ProblemaIntegracao = "Integrado com sucesso";
                }
                else
                {
                    integracaoEnvioProgramado.SituacaoIntegracao = situacaoIntegracao;
                    integracaoEnvioProgramado.ProblemaIntegracao = cteErro?.Mensagem ?? problemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(integracaoEnvioProgramado, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", mensagemErroIntegracao);
                repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
            }
        }

        public void IntegrarNFSes(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            DateTime dataIntegracao = DateTime.Now;
            SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            string problemaIntegracao = string.Empty;
            string jsonDadosRequisicao = string.Empty;
            string jsonDadosRetornoRequisicao = string.Empty;
            string mensagemErroIntegracao = "Integrado com sucesso";
            HttpStatusCode statusCodeRetornoRequisicao = HttpStatusCode.InternalServerError;

            bool sucesso = false;

            try
            {
                IntegrarNFSesCarga(integracoesEnvioProgramado, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao, out statusCodeRetornoRequisicao);
            }
            catch (ServicoException excecao)
            {
                problemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoGrupoSC");

                problemaIntegracao = "Ocorreu uma falha ao realizar a integração com a GrupoSC";
            }

            List<(string ProtocoloPrefeitura, string Mensagem)> nfsesComErro = new();

            if (statusCodeRetornoRequisicao != HttpStatusCode.InternalServerError)
                nfsesComErro = ObtemNFSesComErro(jsonDadosRetornoRequisicao);
            else if (string.IsNullOrEmpty(problemaIntegracao))
                problemaIntegracao = "Retorno integração GrupoSC: " + jsonDadosRetornoRequisicao;

            if (nfsesComErro?.Count == 0 && string.IsNullOrEmpty(problemaIntegracao))
                sucesso = true;

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                integracaoEnvioProgramado.DataIntegracao = dataIntegracao;
                integracaoEnvioProgramado.NumeroTentativas++;

                (string ProtocoloPrefeitura, string Mensagem)? nfseErro = nfsesComErro?.Find(x => x.ProtocoloPrefeitura == integracaoEnvioProgramado.CTe.Chave);
                if (sucesso && string.IsNullOrWhiteSpace(problemaIntegracao))
                {
                    integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoEnvioProgramado.ProblemaIntegracao = "Integrado com sucesso";
                }
                else
                {
                    integracaoEnvioProgramado.SituacaoIntegracao = situacaoIntegracao;
                    integracaoEnvioProgramado.ProblemaIntegracao = nfseErro?.Mensagem ?? problemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(integracaoEnvioProgramado, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", mensagemErroIntegracao);
                repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
            }
        }

        public void IntegrarOutrosDocumentos(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoesEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            DateTime dataIntegracao = DateTime.Now;
            SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            string problemaIntegracao = string.Empty;
            string jsonDadosRequisicao = string.Empty;
            string jsonDadosRetornoRequisicao = string.Empty;
            string mensagemErroIntegracao = "Integrado com sucesso";
            HttpStatusCode statusCodeRetornoRequisicao = HttpStatusCode.InternalServerError;

            bool sucesso = false;

            try
            {
                IntegrarOutrosDocumentosCarga(integracoesEnvioProgramado, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao, out statusCodeRetornoRequisicao);
            }
            catch (ServicoException excecao)
            {
                problemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoGrupoSC");

                problemaIntegracao = "Ocorreu uma falha ao realizar a integração com a GrupoSC";
            }

            List<(string NumeroND, string SerieND, string Mensagem)> outrosDocumentosComErro = new();

            if (statusCodeRetornoRequisicao != HttpStatusCode.InternalServerError)
                outrosDocumentosComErro = ObtemOutrosDocumentosComErro(jsonDadosRetornoRequisicao);
            else if (string.IsNullOrEmpty(problemaIntegracao))
                problemaIntegracao = "Retorno integração GrupoSC: " + jsonDadosRetornoRequisicao;

            if (outrosDocumentosComErro.Count == 0 && string.IsNullOrEmpty(problemaIntegracao))
                sucesso = true;

            foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado in integracoesEnvioProgramado)
            {
                integracaoEnvioProgramado.DataIntegracao = dataIntegracao;
                integracaoEnvioProgramado.NumeroTentativas++;

                (string NumeroND, string SerieND, string Mensagem) outroDocumentoErro = outrosDocumentosComErro.Find(obj => obj.NumeroND.ToInt() == integracaoEnvioProgramado.CTe.Numero && obj.SerieND.ToInt() == integracaoEnvioProgramado.CTe.Serie.Numero);

                if (sucesso && string.IsNullOrWhiteSpace(problemaIntegracao))
                {
                    integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoEnvioProgramado.ProblemaIntegracao = "Integrado com sucesso";
                }
                else
                {
                    integracaoEnvioProgramado.SituacaoIntegracao = situacaoIntegracao;
                    integracaoEnvioProgramado.ProblemaIntegracao = outroDocumentoErro.Mensagem ?? problemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(integracaoEnvioProgramado, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", mensagemErroIntegracao);
                repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
            }
        }

        public async Task IntegrarRetornoConfirmacaoPedidosAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                await ObterConfiguracaoIntegracaoAsync();

                HttpClient requisicao = CriaRequisicao();

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.EnviaConfirmacaoPedido corpoRequisicao = PreencherConfirmacaoPedido(contexto, subtarefas);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = await requisicao.PostAsync(_configuracaoIntegracao.URLIntegracao + "/TMS/EnviaConfirmacoesPedido", conteudoRequisicao);
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado GrupoSC.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com GrupoSC.");

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
                Log.TratarErro(excecao, "IntegracaoGrupoSC");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a GrupoSC.";
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
                await ObterConfiguracaoIntegracaoAsync();

                Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC.ConfirmacaoFecharCarga.EnviaConfirmacaoFecharCarga corpoRequisicao = await PreencherConfirmacaoFecharCarga(contexto, cancellationToken);

                HttpClient requisicao = CriaRequisicao();

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = await requisicao.PostAsync(_configuracaoIntegracao.URLIntegracao + "/TMS/EnviaConfirmacoesFechaCarga", conteudoRequisicao);
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado GrupoSC.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com GrupoSC.");

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
                Log.TratarErro(excecao, "IntegracaoGrupoSC");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a GrupoSC.";
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

        public void IntegrarPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            DateTime dataIntegracao = DateTime.Now;
            SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            string problemaIntegracao = "Ocorreu uma falha ao realizar a integração com a GrupoSC";
            string jsonDadosRequisicao = string.Empty;
            string jsonDadosRetornoRequisicao = string.Empty;
            string mensagemErroIntegracao = "Integrado com sucesso";

            bool sucesso = false;

            try
            {
                sucesso = IntegrarPagamento(pagamentoIntegracao, out jsonDadosRequisicao, out jsonDadosRetornoRequisicao, out mensagemErroIntegracao);
            }
            catch (ServicoException excecao)
            {
                problemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoGrupoSC");
            }

            pagamentoIntegracao.DataIntegracao = dataIntegracao;
            pagamentoIntegracao.NumeroTentativas++;

            if (sucesso)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                pagamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            else
            {
                pagamentoIntegracao.SituacaoIntegracao = situacaoIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = problemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(pagamentoIntegracao, jsonDadosRequisicao, jsonDadosRetornoRequisicao, "json", mensagemErroIntegracao);
            repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);

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

        #endregion Métodos Públicos
    }
}
