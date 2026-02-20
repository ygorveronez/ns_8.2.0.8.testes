using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Trizy
{
    public class IntegracaoTrizyOfertas : ServicoBase
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy _configuracaoIntegracaoTrizy;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;

        #endregion

        #region Construtores

        public IntegracaoTrizyOfertas(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public async Task VerificarIntegracoesAguardando(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread identificadorControlePosicaoThread, CancellationToken cancellationToken)
        {
            _unitOfWork.FlushAndClear();

            try
            {
                Servicos.Log.TratarErro("Inicio Buscando Integrações Aguardando", "IntegracaoOfertarCarga");

                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, identificadorControlePosicaoThread);
                Repositorio.Embarcador.Cargas.CargaOfertaIntegracao repCargaOfertaIntegracao = new Repositorio.Embarcador.Cargas.CargaOfertaIntegracao(_unitOfWork);
                List<int> listaIntegracoesAguardando = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCargaOfertaIntegracao.BuscarIntegracoesAguardandoAsync(limiteRegistros, cancellationToken).GetAwaiter().GetResult());

                Servicos.Log.TratarErro("Integrações Aguardando: " + listaIntegracoesAguardando.Count + "", "IntegracaoOfertarCarga");

                List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao> integracoes = repCargaOfertaIntegracao.BuscarPorCodigos(listaIntegracoesAguardando, false);

                if (integracoes != null && integracoes.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao integracao in integracoes)
                    {
                        switch (integracao.TipoIntegracao.Tipo)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyOfertarCarga:
                                if (integracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga.Ofertar)
                                    await IntegrarOferta(integracao, cancellationToken);
                                if (integracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga.Inativar ||
                                    integracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga.Cancelar ||
                                    integracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga.Completar ||
                                    integracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga.Ativar)
                                    await IntegrarAtualizacaoOferta(integracao, cancellationToken);
                                break;
                            default:
                                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                                integracao.ProblemaIntegracao = "Integração não implementada.";
                                await repCargaOfertaIntegracao.AtualizarAsync(integracao);
                                break;
                        }
                    }
                }

                _unitOfWork.Flush();

                Repositorio.Embarcador.Cargas.CargaOferta repCargaOferta = new Repositorio.Embarcador.Cargas.CargaOferta(_unitOfWork);
                await repCargaOferta.AtualizarSituacaoIntegracaoAsync(listaIntegracoesAguardando, cancellationToken);

                Servicos.Log.TratarErro("Fim Integrações Aguardando", "IntegracaoOfertarCarga");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion

        #region Métodos Privados

        private async Task IntegrarOferta(Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao cargaOfertaIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaOfertaIntegracao repositorioCargaOfertaIntegracao = new Repositorio.Embarcador.Cargas.CargaOfertaIntegracao(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaOferta repositorioCargaOferta = new Repositorio.Embarcador.Cargas.CargaOferta(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                (Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy) = await PrepararIntegracao(cargaOfertaIntegracao);
                cargaOfertaIntegracao.CargaOferta.Data = DateTime.Now;

                string endPoint = configuracaoIntegracaoTrizy.URLIntegracaoOfertas;

                HttpClient client = IntegracaoTrizy.CriarHttpClient(configuracaoIntegracao, endPoint);

                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.OfertaCarga ofertaCarga = await ObterDadosOfertaCarga(cargaOfertaIntegracao, cancellationToken);

                jsonRequest = JsonConvert.SerializeObject(ofertaCarga, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync(endPoint, content, cancellationToken);
                jsonResponse = await result.Content.ReadAsStringAsync();

                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.RetornoIntegracao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.RetornoIntegracao>(jsonResponse);

                if (result.IsSuccessStatusCode)
                {
                    cargaOfertaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaOfertaIntegracao.ProblemaIntegracao = "Integrado";
                    cargaOfertaIntegracao.CargaOferta.CodigoIntegracao = retorno.Result.Id;
                }
                else
                    throw new ServicoException(retorno.Message);

            }
            catch (Exception excecao)
            {
                if (excecao is BaseException)
                    cargaOfertaIntegracao.ProblemaIntegracao = excecao.Message;
                else
                    cargaOfertaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com a API da Trizy.";

                cargaOfertaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                cargaOfertaIntegracao.CargaOferta.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                await repositorioCargaOferta.AtualizarAsync(cargaOfertaIntegracao.CargaOferta);
            }
            finally
            {
                await repositorioCargaOfertaIntegracao.AtualizarAsync(cargaOfertaIntegracao);
                await CriarArquivosIntegracao(cargaOfertaIntegracao, jsonRequest, jsonResponse);

            }
        }

        private async Task IntegrarAtualizacaoOferta(Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao cargaOfertaIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaOfertaIntegracao repositorioCargaOfertaIntegracao = new Repositorio.Embarcador.Cargas.CargaOfertaIntegracao(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaOferta repositorioCargaOferta = new Repositorio.Embarcador.Cargas.CargaOferta(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                (Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy) = await PrepararIntegracao(cargaOfertaIntegracao);

                string endPoint = $"{configuracaoIntegracaoTrizy.URLIntegracaoOfertas}/{cargaOfertaIntegracao.CargaOferta.CodigoIntegracao}";

                HttpClient client = IntegracaoTrizy.CriarHttpClient(configuracaoIntegracao, endPoint);

                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.OfertaCarga ofertaCarga = await ObterDadosOfertaCarga(cargaOfertaIntegracao, cancellationToken);

                jsonRequest = JsonConvert.SerializeObject(ofertaCarga, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), endPoint);
                request.Content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.SendAsync(request);
                jsonResponse = await result.Content.ReadAsStringAsync();

                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.RetornoIntegracao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.RetornoIntegracao>(jsonResponse);

                if (result.IsSuccessStatusCode)
                {
                    cargaOfertaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaOfertaIntegracao.ProblemaIntegracao = retorno.Message;

                    if (cargaOfertaIntegracao.Tipo == TipoIntegracaoOfertaCarga.Ativar)
                        cargaOfertaIntegracao.CargaOferta.Situacao = SituacaoCargaOferta.EmOferta;

                    else if (cargaOfertaIntegracao.Tipo == TipoIntegracaoOfertaCarga.Cancelar)
                        cargaOfertaIntegracao.CargaOferta.Situacao = SituacaoCargaOferta.Cancelada;

                    if (retorno.Result != null)
                        cargaOfertaIntegracao.CargaOferta.CodigoIntegracao = retorno.Result.Id;
                }
                else
                    throw new ServicoException(retorno.Message);

            }
            catch (Exception excecao)
            {
                if (excecao is BaseException)
                    cargaOfertaIntegracao.ProblemaIntegracao = excecao.Message;
                else
                    cargaOfertaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com a API da Trizy.";

                cargaOfertaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                cargaOfertaIntegracao.CargaOferta.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                await repositorioCargaOferta.AtualizarAsync(cargaOfertaIntegracao.CargaOferta);
            }
            finally
            {
                await repositorioCargaOfertaIntegracao.AtualizarAsync(cargaOfertaIntegracao);
                await CriarArquivosIntegracao(cargaOfertaIntegracao, jsonRequest, jsonResponse);
            }
        }

        private async Task<(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy)> PrepararIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao cargaOfertaIntegracao)
        {
            cargaOfertaIntegracao.DataIntegracao = DateTime.Now;
            cargaOfertaIntegracao.NumeroTentativas += 1;

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await ObterConfiguracaoIntegracao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = await ObterConfiguracaoIntegracaoTrizy();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return (configuracaoIntegracao, configuracaoIntegracaoTrizy);
        }

        private async Task CriarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao cargaOfertaIntegracao, string jsonRequest, string jsonResponse)
        {
            Repositorio.Embarcador.Cargas.CargaOfertaIntegracaoArquivos repositorioCargaOfertaIntegracaoArquivos = new Repositorio.Embarcador.Cargas.CargaOfertaIntegracaoArquivos(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos();
            arquivoIntegracao.CargaOfertaIntegracao = cargaOfertaIntegracao;
            arquivoIntegracao.Data = cargaOfertaIntegracao.DataIntegracao;
            arquivoIntegracao.Mensagem = cargaOfertaIntegracao.ProblemaIntegracao.Length > 255 ? cargaOfertaIntegracao.ProblemaIntegracao.Substring(0, 255) : cargaOfertaIntegracao.ProblemaIntegracao;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            if (jsonRequest != string.Empty)
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
            if (jsonResponse != string.Empty)
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

            await repositorioCargaOfertaIntegracaoArquivos.InserirAsync(arquivoIntegracao);
        }

        private async Task<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy> ObterConfiguracaoIntegracaoTrizy()
        {
            if (_configuracaoIntegracaoTrizy != null)
                return _configuracaoIntegracaoTrizy;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(_unitOfWork);
            _configuracaoIntegracaoTrizy = await repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            if (_configuracaoIntegracaoTrizy == null || !(_configuracaoIntegracaoTrizy.IntegrarOfertasCargas ?? false))
                throw new ServicoException("Não existe configuração de integração disponível.");

            return _configuracaoIntegracaoTrizy;
        }

        private async Task<Dominio.Entidades.Embarcador.Configuracoes.Integracao> ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return _configuracaoIntegracao;

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new(_unitOfWork);
            _configuracaoIntegracao = await repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            if (_configuracaoIntegracao == null || !_configuracaoIntegracao.PossuiIntegracaoTrizy)
                throw new ServicoException("Não existe configuração de integração disponível.");

            return _configuracaoIntegracao;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.OfertaCarga> ObterDadosOfertaCarga(Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao cargaOfertaIntegracao, CancellationToken cancellationToken)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaOfertaIntegracao.CargaOferta.Carga;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = await ObterPedidosDaCargaAsync(carga.Codigo, cancellationToken);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroPedido = pedidos.FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregas = await ObterEntregasDaCargaAsync(carga.Codigo, cancellationToken);

            (Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint origem, Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint destino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint> entregas, Dominio.Entidades.Cliente remetente) = await ProcessarPontosRotaAsync(primeiroPedido, listaEntregas);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.OfertaCarga
            {
                Status = cargaOfertaIntegracao.Tipo.ObterDescricaoStatusTrizy(),
                ExternalId = carga.Codigo.ToString(),
                Broker = ObterFilial(carga),
                EndAt = ObterDataFinalOferta(cargaOfertaIntegracao.CargaOferta),
                Origin = origem,
                Stopovers = entregas,
                Destination = destino,
                Contacts = ObterContatos(primeiroPedido),
                NegotiationInfo = await ObterInformacoesNegociacaoAsync(carga, cargaOfertaIntegracao.CargaOferta.ParametrosOfertas, remetente, cancellationToken),
                AdditionalInformation = ObterInformacoesAdicionais(carga, pedidos),
            };
        }

        private async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> ObterPedidosDaCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, cancellationToken);
            return await repositorioCargaPedido.BuscarPorCargaOrdenadoPorEntregaAsync(codigoCarga, cancellationToken);
        }

        private async Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>> ObterEntregasDaCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork, cancellationToken);
            return await repositorioCargaEntrega.BuscarPorCargaAsync(codigoCarga, cancellationToken);
        }

        private async Task<(Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint origem, Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint destino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint> entregas, Dominio.Entidades.Cliente remetente)> ProcessarPontosRotaAsync(Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroPedido, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint origem = null;
            Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint destino = null;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint> entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint>();
            Dominio.Entidades.Cliente remetente = null;

            (origem, remetente) = ObterPontoOrigem(primeiroPedido, listaEntregas);

            (destino, entregas) = ObterPontosEntregaEDestino(listaEntregas);

            return (origem, destino, entregas, remetente);
        }

        private (Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint origem, Dominio.Entidades.Cliente remetente) ObterPontoOrigem(Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroPedido, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint origem = null;
            Dominio.Entidades.Cliente remetente = null;

            if (listaEntregas != null && listaEntregas.Exists(cargaEntrega => cargaEntrega.Coleta))
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta = listaEntregas.First(cargaEntrega => cargaEntrega.Coleta);

                if (primeiraColeta.ClienteOutroEndereco != null)
                    origem = ObterEndereco(primeiraColeta.ClienteOutroEndereco.Localidade, primeiraColeta.ClienteOutroEndereco);
                else
                    origem = ObterEndereco(primeiraColeta.Cliente.Localidade, primeiraColeta.Cliente);

                if (listaEntregas.Count > 0)
                    listaEntregas.RemoveAt(0);
            }
            else
            {
                remetente = primeiroPedido.ClienteColeta;
                origem = ObterEndereco(remetente.Localidade, remetente);
            }

            return (origem, remetente);
        }

        private (Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint destino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint> entregas) ObterPontosEntregaEDestino(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint destino = null;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint> entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint>();

            if (listaEntregas == null)
                return (destino, entregas);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasFiltradas = listaEntregas.Where(cargaEntrega => cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Entrega).ToList();

            if (entregasFiltradas.Count > 1)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasIntermediarias = entregasFiltradas.Take(entregasFiltradas.Count - 1).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregasIntermediarias)
                {
                    entregas.Add(entrega.ClienteOutroEndereco != null
                        ? ObterEndereco(entrega.ClienteOutroEndereco.Localidade, entrega.ClienteOutroEndereco)
                        : ObterEndereco(entrega.Cliente.Localidade, entrega.Cliente));
                }

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaEntrega = entregasFiltradas[entregasFiltradas.Count - 1];

                destino = ultimaEntrega.ClienteOutroEndereco != null
                    ? ObterEndereco(ultimaEntrega.ClienteOutroEndereco.Localidade, ultimaEntrega.ClienteOutroEndereco)
                    : ObterEndereco(ultimaEntrega.Cliente.Localidade, ultimaEntrega.Cliente);

            }
            else if (entregasFiltradas.Count == 1)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaEntrega = entregasFiltradas[0];
                destino = ultimaEntrega.ClienteOutroEndereco != null
                    ? ObterEndereco(ultimaEntrega.ClienteOutroEndereco.Localidade, ultimaEntrega.ClienteOutroEndereco)
                    : ObterEndereco(ultimaEntrega.Cliente.Localidade, ultimaEntrega.Cliente);
            }

            return (destino, entregas);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Broker ObterFilial(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Broker
            {
                Document = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Document
                {
                    Type = "CNPJ",
                    Value = carga.Filial.CNPJ_SemFormato
                },
                Name = carga.Filial.Descricao
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint ObterEndereco(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Cliente cliente)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint
            {
                Point = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Point
                {
                    CountryCode = "BRA",
                    Country = localidade.Pais.Descricao,
                    StateCode = localidade.Estado.Sigla,
                    State = localidade.Estado.Nome.Trim(),
                    City = localidade.Descricao,
                    CountyCode = localidade.CodigoIBGE.ToString(),
                    County = localidade.Descricao,
                    Subdistrict = cliente.Bairro,
                    Street = cliente.Endereco,
                    PostalCode = cliente.CEP,
                    HouseNumber = cliente.Numero,
                    Label = localidade.Descricao,
                    Complement = cliente.Complemento ?? string.Empty
                }
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint ObterEndereco(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocationPoint
            {
                Point = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Point
                {
                    CountryCode = "BRA",
                    Country = localidade.Pais.Descricao,
                    StateCode = localidade.Estado.Sigla,
                    State = localidade.Estado.Nome.Trim(),
                    City = localidade.Descricao,
                    CountyCode = localidade.CodigoIBGE.ToString(),
                    County = localidade.Descricao,
                    Subdistrict = clienteOutroEndereco.Bairro,
                    Street = clienteOutroEndereco.Endereco,
                    PostalCode = clienteOutroEndereco.CEP,
                    HouseNumber = clienteOutroEndereco.Numero,
                    Label = localidade.Descricao,
                    Complement = clienteOutroEndereco.Complemento ?? string.Empty
                }
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Contact> ObterContatos(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.ContactItem> listaContatos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.ContactItem>();
            if (!string.IsNullOrEmpty(cargaPedido.ClienteColeta?.Telefone1 ?? string.Empty))
            {
                listaContatos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.ContactItem
                {
                    Type = "WHATSAPP",
                    Value = $"+55{cargaPedido.ClienteColeta.Telefone1}"
                });
            }
            return new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Contact>
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Contact
                {
                    Items = listaContatos
                }
            };
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.NegotiationInfo> ObterInformacoesNegociacaoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.Entidades.Cliente remetente, CancellationToken cancellationToken)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.NegotiationInfo
            {
                Type = "DIRECT_OFFER",
                Price = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Price
                {
                    Value = carga.ValorFrete,
                    Currency = "BRL"
                },
                WithVehicle = true,
                VehicleConfigurations = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.VehicleConfiguration>
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.VehicleConfiguration
                    {
                        _id = carga.ModeloVeicularCarga.CodigosIntegracao.FirstOrDefault() ?? string.Empty,
                    }
                },
                Audience = await ObterPublicoOfertaAsync(parametrosOfertas, remetente, cancellationToken)
            };
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Audience> ObterPublicoOfertaAsync(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.Entidades.Cliente remetente, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario repositorioMotorista = new Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana repositorioDiaSemana = new Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana(_unitOfWork, cancellationToken);

            Dominio.Entidades.Usuario motorista = await repositorioMotorista.BuscarPrimeiroMotoristaPorCodigoParametroOfertaAsync(parametrosOfertas.Codigo, cancellationToken);
            Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana dadosDiaSemana = await repositorioDiaSemana.BuscarPorCodigoParametrosOfertaEPeriodoAsync(parametrosOfertas.Codigo, DateTime.Now, cancellationToken);

            if (parametrosOfertas.GrupoMotoristas != null)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Audience
                {
                    Segmentation = parametrosOfertas.GrupoMotoristas.CodigoIntegracao
                };
            }

            if (motorista != null)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Audience
                {
                    Driver = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Driver
                    {
                        Document = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Document
                        {
                            Type = "CPF",
                            Value = motorista.CPF ?? string.Empty
                        }
                    }
                };
            }

            if (dadosDiaSemana?.ParametrosOfertasDadosOferta?.Raio > 0)
            {
                if (remetente.Longitude == null || remetente.Latitude == null)
                    throw new ServicoException("Coordenadas do remetente não definidas.");

                return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Audience
                {
                    Geofence = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Geofence
                    {
                        Type = "Circle",
                        Coordinates = new double[]
                        {
                            Convert.ToDouble(remetente.Longitude, CultureInfo.InvariantCulture),
                            Convert.ToDouble(remetente.Latitude, CultureInfo.InvariantCulture)
                        },
                        Radius = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.GeofenceRadius
                        {
                            Unit = "m",
                            Value = dadosDiaSemana.ParametrosOfertasDadosOferta.Raio
                        }
                    }
                };
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.Audience();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.AdditionalInformation> ObterInformacoesAdicionais(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.AdditionalInformation> informacoesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.AdditionalInformation>();

            Dictionary<(string ptLabel, string enLabel, string esLabel), Func<Dominio.Entidades.Embarcador.Cargas.Carga, string>> infoMapping = new Dictionary<(string ptLabel, string enLabel, string esLabel), Func<Dominio.Entidades.Embarcador.Cargas.Carga, string>>
            {
                {("Tipo de Carga", "Cargo Type", "Tipo de Carga"),
                    c => c.TipoDeCarga?.Descricao ?? string.Empty},

                {("Peso", "Weight", "Peso"),
                    c => $"{c.DadosSumarizados?.PesoTotal ?? 0} KG"},
            };

            foreach (KeyValuePair<(string ptLabel, string enLabel, string esLabel), Func<Dominio.Entidades.Embarcador.Cargas.Carga, string>> info in infoMapping)
            {
                string valor = info.Value(carga);

                if (!string.IsNullOrEmpty(valor))
                {
                    informacoesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.AdditionalInformation
                    {
                        Label = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocalizedText
                        {
                            Pt = info.Key.ptLabel,
                            En = info.Key.enLabel,
                            Es = info.Key.esLabel
                        },
                        Description = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocalizedText
                        {
                            Pt = valor,
                            En = valor,
                            Es = valor
                        }
                    });
                }
            }

            List<string> produtosPredominantes = pedidos
                .Where(p => p.Pedido != null && !string.IsNullOrEmpty(p.Pedido.ProdutoPredominante))
                .Select(p => p.Pedido.ProdutoPredominante)
                .Distinct()
                .ToList();

            if (produtosPredominantes.Any())
            {
                string produtos = string.Join(", ", produtosPredominantes);
                informacoesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.AdditionalInformation
                {
                    Label = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocalizedText
                    {
                        Pt = "Produto Predominante",
                        En = "Predominant Product",
                        Es = "Produto Predominante"
                    },
                    Description = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyOfertas.LocalizedText
                    {
                        Pt = produtos,
                        En = produtos,
                        Es = produtos
                    }
                });
            }

            return informacoesAdicionais;
        }

        private string ObterDataFinalOferta(Dominio.Entidades.Embarcador.Cargas.CargaOferta cargaOferta)
        {
            if (cargaOferta.DataFimOferta != null)
            {
                return cargaOferta.DataFimOferta.Value.AddHours(3).ToDateTimeStringISO8601();
            }

            return string.Empty;
        }

        #endregion
    }
}

