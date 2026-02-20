using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.P44;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.P44
{
    public class IntegracaoP44
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoP44(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void Integrar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracaoFluxoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioIntegracaoFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            integracaoFluxoPatio.NumeroTentativas += 1;
            integracaoFluxoPatio.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoP44 repositorioIntegracaoP44 = new Repositorio.Embarcador.Configuracoes.IntegracaoP44(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 dadosIntegracao = repositorioIntegracaoP44.BuscarPrimeiroRegistro();

                if (!(dadosIntegracao?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não existe configuração de integração disponível para a P44.");

                string url = ObterUrlIntegracaoPatio(dadosIntegracao);
                string token = ObterToken(dadosIntegracao, integracaoFluxoPatio.TipoIntegracao.Tipo);
                HttpClient requisicao = CriarRequisicao(url, token);
                Dominio.ObjetosDeValor.Embarcador.Integracao.P44.ObjetoEnvio objetoEnvio = ConverterObjetoIntegracao(integracaoFluxoPatio.Carga);

                jsonRequisicao = JsonConvert.SerializeObject(objetoEnvio, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                var conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                var retornoRequisicao = string.IsNullOrWhiteSpace(objetoEnvio.id) ? requisicao.PostAsync(url, conteudoRequisicao).Result : requisicao.PutAsync(url, conteudoRequisicao).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    integracaoFluxoPatio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoFluxoPatio.ProblemaIntegracao = "Integrado com sucesso";

                    if (string.IsNullOrWhiteSpace(objetoEnvio.id))
                    {
                        dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                        if ((retorno.shipment != null) && (retorno.shipment.id != null))
                            integracaoFluxoPatio.PreProtocolo = (string)retorno.shipment.id;
                    }
                }
                else
                {
                    integracaoFluxoPatio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoFluxoPatio.ProblemaIntegracao = "Falha na integração com a P44.";
                }

                servicoArquivoTransacao.Adicionar(integracaoFluxoPatio, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                integracaoFluxoPatio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoFluxoPatio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoFluxoPatio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoFluxoPatio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a P44";

                servicoArquivoTransacao.Adicionar(integracaoFluxoPatio, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioIntegracaoFluxoPatio.Atualizar(integracaoFluxoPatio);
        }

        public void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            integracaoPendente.NumeroTentativas += 1;
            integracaoPendente.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoP44 repositorioIntegracaoP44 = new Repositorio.Embarcador.Configuracoes.IntegracaoP44(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 dadosIntegracao = repositorioIntegracaoP44.BuscarPrimeiroRegistro();

                if (!(dadosIntegracao?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não existe configuração de integração disponível para a P44.");

                string url = ObterUrlIntegracaoPatio(dadosIntegracao);
                string token = ObterToken(dadosIntegracao, integracaoPendente.TipoIntegracao.Tipo);
                HttpClient requisicao = CriarRequisicao(url, token);
                Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioIntegracaoFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = integracaoPendente.CargaCancelamento.Carga;
                string preProtocolo = repositorioIntegracaoFluxoPatio.BuscarPreProtocoloPorCarga(carga.Codigo);
                StringBuilder stringBuilder = new StringBuilder();

                if (string.IsNullOrEmpty(preProtocolo))
                    stringBuilder
                        .Append(url)
                        .Append("?carrierIdentifier.type=P44_EU&carrierIdentifier.value=" + carga.Empresa?.CodigoIntegracao ?? string.Empty)
                        .Append("shipmentIdentifiers.type=BILL_OF_LADING&shipmentIdentifiers.value=" + carga.CodigoCargaEmbarcador);
                else
                    stringBuilder
                        .Append(url)
                        .Append("/" + preProtocolo);

                var retornoRequisicao = requisicao.DeleteAsync(stringBuilder.ToString()).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.ProblemaIntegracao = "Integrado com sucesso";
                }
                else
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.ProblemaIntegracao = "Falha na integração com a P44.";
                }

                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a P44";

                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaCancelamentoCargaIntegracao.Atualizar(integracaoPendente);
        }

        public bool ValidarEtapaIntegracao(EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (!(fluxoGestaoPatio.Filial?.GerarIntegracaoP44 ?? false))
                return false;

            Repositorio.Embarcador.Configuracoes.IntegracaoP44 repositorioConfiguracaoP44 = new Repositorio.Embarcador.Configuracoes.IntegracaoP44(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 configuracaoP44 = repositorioConfiguracaoP44.Buscar();

            if (!(configuracaoP44?.PossuiIntegracao ?? false))
                return false;

            GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            return sequenciaGestaoPatio.PermitirGerarIntegracaoP44(etapa);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url, string token)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoP44));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 configuracao, TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao repToken = new Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao token = repToken.BuscarPorTipo(tipoIntegracao);

            if (!string.IsNullOrWhiteSpace(token?.Token) && token.DataExpiracao > DateTime.Now)
                return token.Token;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(configuracao.URLAutenticacao.Contains("https:") ? configuracao.URLAutenticacao : string.Concat(configuracao.URLAplicacao, configuracao.URLAutenticacao));
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", configuracao.ClientId);
            request.AddParameter("client_secret", configuracao.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            if (string.IsNullOrWhiteSpace(retorno.access_token))
                throw new ServicoException("Não foi possível obter o Token");

            if (token == null)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao tipoIntegracaoAutenticacao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao()
                {
                    Tipo = tipoIntegracao,
                    DataExpiracao = DateTime.Now.AddSeconds(42500),
                    Token = retorno.access_token
                };
                repToken.Inserir(tipoIntegracaoAutenticacao);
            }
            else
            {
                token.Token = retorno.access_token;
                token.DataExpiracao = DateTime.Now.AddSeconds(42500);
                repToken.Atualizar(token);
            }

            return retorno.access_token;
        }

        private ObjetoEnvio ConverterObjetoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioIntegracaoFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repNotaFiscal.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCarga(carga.Codigo).FirstOrDefault();


            if (notasFiscais == null || notasFiscais.Count == 0)
                notasFiscais = repNotaFiscal.BuscarPorCargasEntrega(carga.Entregas?.Select(entrega => entrega.Codigo).ToList());

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = carga.Pedidos?.FirstOrDefault()?.Pedido;

            string nomeCLiente = pedido?.Destinatario?.Nome?.Trim() ?? string.Empty;
            string numeroPedido = string.IsNullOrWhiteSpace(pedido?.CodigoPedidoCliente) ? pedido?.NumeroPedidoEmbarcador : pedido?.CodigoPedidoCliente;
            string dataPrevisao = DateTime.Now.AddDays(carga.DadosSumarizados?.DiasItinerario ?? 1).ToString("yyyy-MM-dd");
            decimal latitudeFilial = carga.Filial?.Localidade?.Latitude ?? 0;
            decimal longitudeFilial = carga.Filial?.Localidade?.Longitude ?? 0;
            decimal latitudeDestinatario = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(pedido?.Destinatario?.Latitude ?? string.Empty).ToDecimal();
            decimal longitudeDestinatario = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(pedido?.Destinatario?.Longitude ?? string.Empty).ToDecimal();
            string preProtocolo = repositorioIntegracaoFluxoPatio.BuscarPreProtocoloPorCarga(carga.Codigo);

            ObjetoEnvio obj = new ObjetoEnvio();

            obj.id = preProtocolo;
            obj.carrierIdentifier = new TypeValue()
            {
                type = "P44_EU",
                value = carga.Empresa?.CodigoIntegracao ?? string.Empty,
            };
            obj.shipmentIdentifiers = new List<TypeValue>()
            {
                new TypeValue()
                {
                    type = "BILL_OF_LADING",
                    value = carga.CodigoCargaEmbarcador
                }
            };
            obj.attributes = new List<NameValuePredefined>();
            if (notasFiscais != null && notasFiscais.Count > 0)
            {
                obj.attributes.Add(new NameValuePredefined()
                {
                    name = "NOTA_FISCAL",
                    value = string.Join(", ", notasFiscais.Select(x => x.Numero).ToList()),
                    predefined = !string.IsNullOrWhiteSpace(preProtocolo)
                });
            }
            obj.attributes.Add(new NameValuePredefined()
            {
                name = "CLIENT_ID",
                value = nomeCLiente ?? string.Empty,
                predefined = true
            });
            obj.attributes.Add(new NameValuePredefined()
            {
                name = "CLIENT_PO",
                value = numeroPedido ?? string.Empty,
                predefined = true
            });

            obj.shipmentStops = new List<ShipmentStop>()
            {
                new ShipmentStop()
                {
                    stopNumber = 1,
                    appointmentWindow = new AppointmentWindow()
                    {
                        startDateTime = fluxoPatio?.DataEntregaGuarita == null ? string.Empty : fluxoPatio.DataEntregaGuarita?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        endDateTime =  fluxoPatio?.DataEntregaGuarita == null ? string.Empty : string.Concat(fluxoPatio.DataEntregaGuarita?.ToString("yyyy-MM-dd"), "T22:00:00")
                    },
                    location = new Location()
                    {

                        contact = new Contact()
                        {
                            companyName = cargaPedido?.Pedido?.Remetente?.Nome ?? string.Empty
                        }

                    },
                    geoCoordinates = new GeoCoordinates()
                    {
                        latitude = latitudeFilial,
                        longitude = longitudeFilial
                    }
                },
                new ShipmentStop()
                {
                    stopNumber = 2,
                    appointmentWindow = new AppointmentWindow()
                    {
                        startDateTime = string.IsNullOrWhiteSpace(dataPrevisao) ? string.Empty : string.Concat(dataPrevisao, "T07:00:00"),
                        endDateTime = string.IsNullOrWhiteSpace(dataPrevisao) ? string.Empty : string.Concat(dataPrevisao, "T22:00:00"),
                    },
                    location = new Location()
                    {
                        contact = new Contact()
                        {
                            companyName = cargaPedido?.Pedido?.Destinatario?.Nome ?? string.Empty
                        }
                    },
                    geoCoordinates = new GeoCoordinates()
                    {
                        latitude = latitudeDestinatario,
                        longitude = longitudeDestinatario
                    }
                },
            };
            obj.equipmentIdentifiers = new List<TypeValue>()
            {
                new TypeValue()
                {
                    type = "MOBILE_PHONE_NUMBER",
                    value = ObterCelularFormatado(carga.Motoristas?.FirstOrDefault()?.Celular)
                },
                new TypeValue()
                {
                    type = "VEHICLE_ID",
                    value = carga.Veiculo?.Placa
                },
            };

            return obj;
        }

        private string ObterCelularFormatado(string telefone)
        {
            telefone = telefone.ObterSomenteNumeros();

            if (string.IsNullOrWhiteSpace(telefone))
                return string.Empty;

            return string.Concat("+55", telefone);
        }

        private string ObterUrlIntegracaoPatio(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 dadosIntegracao)
        {
            return dadosIntegracao.URLIntegracaoPatio.Contains("https:") ? dadosIntegracao.URLIntegracaoPatio : string.Concat(dadosIntegracao.URLAplicacao, dadosIntegracao.URLIntegracaoPatio);
        }

        #endregion Métodos Privados
    }
}