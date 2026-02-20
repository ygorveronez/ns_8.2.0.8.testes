using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Gadle
{
    public class IntegracaoGadle
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private string _jsonrequest;
        private string _jsonresponse;

        #endregion

        #region Construtores

        public IntegracaoGadle(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Metodos privados

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGadle repositorioConfiguracaoIntegracaoGadle = new Repositorio.Embarcador.Configuracoes.IntegracaoGadle(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle configuracaoIntegracao = repositorioConfiguracaoIntegracaoGadle.Buscar();

            if (!(configuracaoIntegracao?.PossuiIntegracao ?? false) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoGadle))
                throw new ServicoException("Não existe configuração de integração disponível para a Gadle.");

            return configuracaoIntegracao;
        }

        private bool IntegraCarga(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle configuracaoIntegracao, ref string jsonRequest, ref string jsonResponse)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = $"{configuracaoIntegracao.URLIntegracaoGadle}";
            mensagem = "";
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servConsultaCarregamento = new Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(cargaDadosTransporteIntegracao.Carga.Codigo);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Cliente clienteRemetente = pedidosCarga.FirstOrDefault()?.Remetente;

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento carregamentoCarga = servConsultaCarregamento.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle.integracaoGadle integracaoGadle = new Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle.integracaoGadle()
            {
                vehicleType = !string.IsNullOrWhiteSpace(carga.ModeloVeicularCarga?.TipoVeiculoGadle) ? carga.ModeloVeicularCarga?.TipoVeiculoGadle : "TRUCK",
                scheduledFor = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd HH:mm:ss") : carregamentoCarga?.DataCarregamentoProgramada.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                notes = carga.Rota?.Observacao ?? "",
                externalId = carga.CodigoCargaEmbarcador,
                totalWeight = carga.DadosSumarizados?.PesoTotal ?? 0,
                driverAssistantsCount = pedidosCarga.Any(obj => obj.Ajudante) ? 1 : 0,
                warehouse = BuscarDadosCarregamento(carga, carregamentoCarga, clienteRemetente, _unitOfWork),
                merchandises = BuscarDadosMercadoriasCarga(carga, _unitOfWork)
            };

            try
            {
                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGadle));
                client.BaseAddress = new Uri(endPoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.TokenIntegracaoGadle);

                jsonRequest = JsonConvert.SerializeObject(integracaoGadle, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle.RetornoGadle retornoIntegracao = null;
                if (result.IsSuccessStatusCode)
                {
                    retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle.RetornoGadle>(result.Content.ReadAsStringAsync().Result);
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);

                    if (retornoIntegracao.orderId > 0)
                    {
                        //valorPedagio = retornoIntegracao.resultado.pedagioTotal;
                        mensagem = retornoIntegracao.orderId.ToString();
                        return true;
                    }
                    else
                    {
                        Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "IntegracaoGadle");
                        mensagem = result.Content.ReadAsStringAsync().Result;

                        return false;
                    }
                }
                else
                {
                    Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "IntegracaoGadle");
                    mensagem = result.Content.ReadAsStringAsync().Result;

                    return false;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Não foi possível comunicar com a Gadle";
                return false;
            }
        }

        private List<Merchandise> BuscarDadosMercadoriasCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, UnitOfWork unitOfWork)
        {
            List<Merchandise> listaRetorno = new List<Merchandise>();
            Repositorio.Embarcador.Cargas.CargaPedido repcargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repcargaPedido.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Cliente cliente = cargaPedido.ClienteEntrega;
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                if (cliente != null && pedido != null)
                {
                    string number = cliente.Numero.ToString().ObterSomenteNumeros();
                    string complement = !String.IsNullOrWhiteSpace(cliente.Complemento) ? cliente.Complemento.Trim() : "";
                    string postalCode = cliente.CEP.ToString().ObterSomenteNumeros();
                    string latitude = cliente.Latitude;
                    string longitude = cliente.Longitude;
                    string thoroughfare = cliente.Endereco;
                    string neighborhood = cliente.Bairro;
                    string city = cliente.Localidade.Descricao;
                    string state = cliente.Localidade.Estado.Descricao;

                    if (pedido.UsarOutroEnderecoDestino && pedido.EnderecoDestino != null)
                    {
                        number = pedido.EnderecoDestino.Numero;
                        complement = pedido.EnderecoDestino.Complemento;
                        postalCode = pedido.EnderecoDestino.CEP;
                        latitude = pedido.EnderecoDestino.ClienteOutroEndereco?.Latitude ?? "";
                        longitude = pedido.EnderecoDestino.ClienteOutroEndereco?.Longitude ?? "";
                        thoroughfare = pedido.EnderecoDestino.Endereco;
                        neighborhood = pedido.EnderecoDestino.Bairro;
                        city = pedido.EnderecoDestino.Localidade.Descricao;
                        state = pedido.EnderecoDestino.Localidade.Estado.Descricao;
                    }

                    Merchandise objetoEntrega = new Merchandise()
                    {
                        timeWindow = null, //new TimeWindow { start = "12:00", end = "16:00" },
                                           //notes = carga.CodigoCargaEmbarcador,
                        trackingCode = pedido.NumeroPedidoEmbarcador,
                        packagesAmount = pedido.Produtos.Count,
                        address = new Address
                        {
                            number = number,
                            complement = complement,
                            postalCode = postalCode,
                            latitude = latitude,
                            longitude = longitude,
                            thoroughfare = thoroughfare,
                            neighborhood = neighborhood,
                            city = city,
                            state = state
                        },
                        totalWeight = (double)pedido.PesoTotal,
                        recipient = new Recipient { name = cliente.Nome, nationalDocumentNumber = cliente.CPF_CNPJ.ToString(), phoneNumber = cliente.Telefone1.ToString().ObterSomenteNumeros() }
                    };

                    listaRetorno.Add(objetoEntrega);
                }
            }

            return listaRetorno;
        }

        private Warehouse BuscarDadosCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento carregamentoCarga, Dominio.Entidades.Cliente remetente, UnitOfWork unitOfWork)
        {
            Warehouse dadosCarregamentoRetorno = new Warehouse();

            if (carregamentoCarga != null && remetente != null)
            {
                dadosCarregamentoRetorno.address = new Address
                {
                    complement = remetente.Complemento,
                    number = remetente.Numero,
                    postalCode = remetente.CEP.ToString().ObterSomenteNumeros(),
                    latitude = remetente.Latitude,
                    longitude = remetente.Longitude,
                    thoroughfare = remetente.Endereco,
                    neighborhood = remetente.Bairro,
                    city = remetente.Localidade.Descricao,
                    state = remetente.Localidade.Estado.Descricao
                };

                dadosCarregamentoRetorno.timeWindow = null;
                //new TimeWindow { start = "12:00", end = "16:00" };

                dadosCarregamentoRetorno.shipper = new Shipper
                {
                    name = remetente.Nome,
                    nationalDocumentNumber = remetente.CPF_CNPJ.ToString()
                };
            }

            return dadosCarregamentoRetorno;
        }

        #endregion

        #region Metodos Publicos

        public void IntegraDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle configuracaoGadle = ObterConfiguracaoIntegracao();

                if (IntegraCarga(out string mensagem, cargaDadosTransporteIntegracao, configuracaoGadle, ref jsonRequisicao, ref jsonRetorno))
                {
                    cargaDadosTransporteIntegracao.Protocolo = mensagem;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso.";
                }
                else
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da Marfrig.";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork);
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.Mensagem = cargaDadosTransporteIntegracao.ProblemaIntegracao;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

        }

        #endregion

    }
}
