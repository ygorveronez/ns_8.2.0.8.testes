using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.YMS;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.YMS
{
    public class IntegracaoYMS
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYMS _configuracaoIntegracaoYMS;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoYMS(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            string jsonRequest = "";
            string jsonResponse = "";

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracao();
                VerificarConfiguracaoIntegracao();

                Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaDadosTransporte integracaoCarga = ObterIntegracaoCarga(cargaIntegracao.Carga);

                Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno retorno;

                bool atualizacao = !string.IsNullOrEmpty(cargaIntegracao.Protocolo) && DeveUsarHeadersCustomizados();

                if (atualizacao)
                {
                    retorno = AtualizarCarga(cargaIntegracao.Protocolo, integracaoCarga, ref jsonRequest, ref jsonResponse);
                }
                else
                {
                    retorno = EnviarCarga(integracaoCarga, ref jsonRequest, ref jsonResponse);
                }

                bool okNotificacoes = retorno?.Notificacoes == null || retorno.Notificacoes.Count == 0;
                bool temIdentificador = !string.IsNullOrEmpty(retorno?.NumeroAgendamento) || !string.IsNullOrEmpty(retorno?.HashId);

                bool sucesso = atualizacao || (okNotificacoes && temIdentificador);

                if (sucesso && !atualizacao)
                {
                    cargaIntegracao.Protocolo = !string.IsNullOrEmpty(retorno.NumeroAgendamento) ? retorno.NumeroAgendamento : retorno.HashId;
                }

                string mensagem = sucesso ? "Integração gerada com sucesso" : (retorno?.Notificacoes != null && retorno.Notificacoes.Any() ? string.Join(", ", retorno.Notificacoes.Select(n => n.Messagem)) : "erro ao integrar viagem");
                string agendamento = sucesso ? $"Agendamento: {cargaIntegracao.Protocolo}" : string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                AtualizarSituacaoIntegracao(cargaIntegracao, situacao, mensagem, agendamento, jsonRequest, jsonResponse);
            }
            catch (ServicoException excecao)
            {
                AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, excecao.Message, string.Empty, jsonRequest, jsonResponse);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, excecao.Message, string.Empty, jsonRequest, jsonResponse);
            }
        }

        public async Task IntegracaoCancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

                string hashIdCarga = repCargaDadosTransporteIntegracao.BuscarProtocoloPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS);

                ObterConfiguracaoIntegracao();
                VerificarConfiguracaoIntegracao();

                Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.CancelarCargaDadosTransporte integracaoCarga =
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.CancelarCargaDadosTransporte()
                    {
                        NumeroCarga = cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo,
                        Motivo = cargaCancelamentoIntegracao.CargaCancelamento.MotivoCancelamento,
                        DataCancelamento = cargaCancelamentoIntegracao.CargaCancelamento.DataCancelamento,
                        HashCarga = hashIdCarga,
                    };

                jsonRequest = JsonConvert.SerializeObject(integracaoCarga);
                jsonResponse = await CancelarCarga(hashIdCarga,  jsonRequest);

                Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno retorno = null;
                if (jsonResponse != null)
                {
                    retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno>(jsonResponse);
                }                

                var temErros = retorno?.Notificacoes?.Count > 0;
                if (temErros)
                {
                    var mensagemErros = string.Join(", ", retorno!.Notificacoes.Select(n => n.Messagem));
                    throw new ServicoException(mensagemErros);
                }

                string mensagem = "Integração gerada com sucesso";
                string cancelamento = retorno?.NumeroAgendamento ?? retorno?.HashId ?? hashIdCarga;

                await AtualizarSituacaoCancelamentoIntegracao(cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, mensagem, cancelamento, jsonRequest, jsonResponse);
            }
            catch (ServicoException ex)
            {
                await AtualizarSituacaoCancelamentoIntegracao(cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, ex.Message, string.Empty, jsonRequest, jsonResponse);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await AtualizarSituacaoCancelamentoIntegracao(cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, "Erro ao cancelar integração", string.Empty, jsonRequest, jsonResponse);
            }
        }

        public HttpRequisicaoResposta IntegrarEventoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao cargaEntregaEventoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };
            try
            {
                string cargaProtocolo = repCargaDadosTransporteIntegracao.BuscarProtocoloPorCargaETipoIntegracao(cargaEntregaEventoIntegracao.CargaEntregaEvento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS);

                ObterConfiguracaoIntegracao();
                VerificarConfiguracaoIntegracao();

                string baseUrl = _configuracaoIntegracaoYMS.URLIntegracao.TrimEnd('/');
                string hashId = WebUtility.UrlEncode(cargaProtocolo);
                string url = $"{baseUrl}/{hashId}/status";

                HttpClient requisicao = CriarRequisicao(url, _configuracaoIntegracaoYMS.Usuario, _configuracaoIntegracaoYMS.Senha);
                Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaEventos integracaoCarga = ObterIntegracaoEvento(cargaEntregaEventoIntegracao.CargaEntregaEvento);
                string jsonRequisicao = JsonConvert.SerializeObject(integracaoCarga, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = conteudoRequisicao
                };

                request.Headers.Add("lang", "pt-br");
                request.Headers.Add("x-integration-name", "GBM-Trucks");

                HttpResponseMessage retornoRequisicao = requisicao.SendAsync(request).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                    httpRequisicaoResposta.sucesso = true;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.Unauthorized)
                    throw new ServicoException("Integração não autorizada, verifique o usuário e senha!");
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    httpRequisicaoResposta.mensagem = retorno?.ToString();
                }

            }
            catch (ServicoException ex)
            {
                httpRequisicaoResposta.mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                httpRequisicaoResposta.mensagem = "Erro ao integrar evento de entrega";
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Métodos Privados        

        private void ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoYMS repositorioConfiguracaoYMSB = new Repositorio.Embarcador.Configuracoes.IntegracaoYMS(_unitOfWork);

            _configuracaoIntegracaoYMS = repositorioConfiguracaoYMSB.BuscarPrimeiroRegistro();
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno EnviarCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaDadosTransporte integracaoCarga, ref string jsonRequest, ref string jsonResponse)
        {
            jsonRequest = JsonConvert.SerializeObject(integracaoCarga);
            jsonResponse = ExecutarMetodo("POST", _configuracaoIntegracaoYMS.URLIntegracao, jsonRequest).GetAwaiter().GetResult();

            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno>(jsonResponse);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno AtualizarCarga(string protocolo, Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaDadosTransporte integracaoCarga, ref string jsonRequest, ref string jsonResponse)
        {
            string baseUrl = _configuracaoIntegracaoYMS.URLIntegracaoAtualizacao.TrimEnd('/');
            string url = $"{baseUrl}/{Uri.EscapeDataString(protocolo)}";

            jsonRequest = JsonConvert.SerializeObject(integracaoCarga);
            jsonResponse = ExecutarMetodo("PUT", url, jsonRequest).GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(jsonResponse))
                return null;

            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Retorno>(jsonResponse);
        }

        private async Task<string> ExecutarMetodo(string metodo, string url, string json)
        {
            using var client = new System.Net.Http.HttpClient();
            using var request = new System.Net.Http.HttpRequestMessage(new HttpMethod(metodo), new Uri(url));

            string auth;
            switch (_configuracaoIntegracaoYMS?.TipoAutenticacaoYMS)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacaoYMS.Basic:
                    string cred = $"{_configuracaoIntegracaoYMS.Usuario}:{_configuracaoIntegracaoYMS.Senha}";
                    string b64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cred));
                    auth = $"Basic {b64}";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacaoYMS.BearerToken:
                    if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoYMS?.Token))
                        ObterToken();
                    auth = $"Bearer {_configuracaoIntegracaoYMS.Token}";
                    break;

                default:
                    throw new NotSupportedException(
                        $"Tipo de autenticação '{_configuracaoIntegracaoYMS?.TipoAutenticacaoYMS}' não suportado."
                    );
            }

            request.Headers.TryAddWithoutValidation("Authorization", auth);

            if (_configuracaoIntegracaoYMS?.ParametrosAdicionais?.Contains("x-integration-name") == true)
                request.Headers.TryAddWithoutValidation("lang", "en-us");

            request.Headers.TryAddWithoutValidation("Accept", "application/json");

            List<KeyValuePair<string, string>> extras = ObterListaParametrosAdicionais(_configuracaoIntegracaoYMS?.ParametrosAdicionais);

            foreach (KeyValuePair<string, string> key in extras)
            {
                if (key.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                    continue;

                request.Headers.TryAddWithoutValidation(key.Key, key.Value ?? string.Empty);
            }

            request.Content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(body) && body.Contains("\"truck was canceled\""))
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
                throw new ServicoException($"Falha HTTP {(int)response.StatusCode} {response.ReasonPhrase} - Corpo: {body}");

            return body;
        }

        private void VerificarConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracaoYMS == null)
                throw new ServicoException("Não existe configuração de integração disponível para a YMS");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoYMS.Usuario))
                throw new ServicoException("Usuário de integração não configurado.");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoYMS.Senha))
                throw new ServicoException("Senha de integração não configurada.");
        }

        private async Task<string> CancelarCarga(string hashIdCarga, string jsonRequest)
        {            
            string jsonResponse = string.Empty;

            if (DeveUsarHeadersCustomizados())
            {
                string baseUrl = _configuracaoIntegracaoYMS.URLCancelamento.TrimEnd('/');
                string url = $"{baseUrl}/{Uri.EscapeDataString(hashIdCarga)}/cancel";

                jsonResponse = await ExecutarMetodo("PATCH", url, jsonRequest);
            }
            else
            {
                jsonResponse = await ExecutarMetodo("POST", _configuracaoIntegracaoYMS.URLCancelamento, jsonRequest);
            }

            if (string.IsNullOrWhiteSpace(jsonResponse) || jsonResponse == "null")
                return null;

            return jsonResponse;            
        }

        private void ObterToken()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao repToken = new Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao tipoIntegracaoAutenticacao = repToken.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS);

            RestClient client = new RestClient(_configuracaoIntegracaoYMS.URLAutenticacao);
            RestRequest request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _configuracaoIntegracaoYMS.Usuario);
            request.AddParameter("client_secret", _configuracaoIntegracaoYMS.Senha);
            request.AddParameter("scope", "dclogg-internal");
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.RetornoToken retorno = response.Content.FromJson<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.RetornoToken>();

            if (string.IsNullOrWhiteSpace(retorno.Token))
                throw new ServicoException("Não foi possível obter o Token");

            if (tipoIntegracaoAutenticacao == null)
                tipoIntegracaoAutenticacao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao();

            tipoIntegracaoAutenticacao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YMS;
            tipoIntegracaoAutenticacao.Token = retorno.Token;

            _configuracaoIntegracaoYMS.Token = retorno.Token;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaDadosTransporte ObterIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaDadosTransporte integracaoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaDadosTransporte();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> listaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repCargaPedido.BuscarPedidosPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = repositorioOrdemEmbarque.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = listaPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaMotorista primeiroMotorista = listaMotoristas.FirstOrDefault();

            integracaoCarga.NumeroCarga = carga.Codigo;
            integracaoCarga.DataCriacao = carga.DataCriacaoCarga;
            integracaoCarga.Filial = new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Cliente
            {
                Cnpj = carga.Filial?.CNPJ_SemFormato ?? string.Empty,
                Name = carga.Filial?.Descricao ?? string.Empty
            };
            integracaoCarga.Remetente = new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Cliente
            {
                Cnpj = carga.Empresa?.CNPJ_SemFormato ?? string.Empty,
                Name = carga.Empresa?.RazaoSocial ?? string.Empty
            };
            integracaoCarga.Destinatario = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Cliente>
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Cliente
                {
                    Cnpj = primeiroPedido?.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Name = primeiroPedido?.Destinatario?.Nome ?? string.Empty
                }
            };
            integracaoCarga.OrdensEmbarque = ordensEmbarque.Select(x => x.Numero).ToList();
            integracaoCarga.TipoCarga = carga.TipoDeCarga?.Descricao ?? string.Empty;
            integracaoCarga.TipoOperacao = carga.TipoOperacao?.Descricao ?? string.Empty;
            integracaoCarga.TipoCargaTaura = carga.CategoriaCargaEmbarcador ?? string.Empty;
            integracaoCarga.DataCarregamento = carga.DataCarregamentoCarga;

            if (carga.TipoDeCarga?.ControlaTemperatura ?? false)
            {
                integracaoCarga.RangeTemperaturaTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Temperatura
                {
                    Descricao = carga.TipoDeCarga?.FaixaDeTemperatura?.Descricao ?? string.Empty,
                    De = Convert.ToInt32(carga.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0),
                    Ate = Convert.ToInt32(carga.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0)
                };
            }
            else
            {
                integracaoCarga.RangeTemperaturaTransporte = new Temperatura();
            }

            integracaoCarga.Transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Cliente
            {
                Cnpj = carga.Filial?.CNPJ_SemFormato ?? string.Empty,
                Name = carga.Filial?.Descricao ?? string.Empty
            };
            integracaoCarga.Plate = carga.Veiculo?.Placa ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(carga.PlacasVeiculos))
            {
                string[] placas = carga.PlacasVeiculos.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                integracaoCarga.AdditionalPlates = placas.Skip(1).Select(p => new AdditionalPlate { Plate = p }).ToList();
            }


            integracaoCarga.ModeloVeicular = carga.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty;
            integracaoCarga.CpfMotorista = primeiroMotorista?.Motorista?.CPF ?? string.Empty;
            integracaoCarga.NomeMotorista = primeiroMotorista?.Motorista?.Nome ?? string.Empty;

            integracaoCarga.PesoTotalKg = carga.DadosSumarizados?.PesoTotal != null ? Convert.ToDouble(carga.DadosSumarizados.PesoTotal) : 0;
            integracaoCarga.PesoLiquidoKg = carga.DadosSumarizados?.PesoLiquidoTotal != null ? Convert.ToDouble(carga.DadosSumarizados.PesoLiquidoTotal) : 0;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.PedidosPorRemetente> pedidosPorRemetente = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.PedidosPorRemetente>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
            {
                pedidosPorRemetente.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.PedidosPorRemetente
                {
                    Remetente = pedido.Remetente?.Nome ?? string.Empty,
                    CnpjRemetente = pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Pedidos = new List<int> { pedido.Numero }
                });
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Produtos> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Produtos>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in listaProdutos)
            {
                produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.Produtos
                {
                    CodigoProduto = produto.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                    Descricao = produto.Produto?.Descricao ?? string.Empty,
                    Quantidade = produto.Quantidade,
                    Unidade = produto.Produto?.Unidade?.Sigla ?? string.Empty,
                });
            }
            integracaoCarga.Operation = "charge";
            integracaoCarga.NumeroBooking = primeiroPedido.NumeroBooking ?? string.Empty;
            integracaoCarga.PedidosPorRemetente = pedidosPorRemetente ?? new List<PedidosPorRemetente>();
            integracaoCarga.Produtos = produtos ?? new List<Produtos>();
            integracaoCarga.Produto = listaProdutos.Select(x => x.Produto?.Descricao).FirstOrDefault() ?? string.Empty;
            integracaoCarga.ProdutoCodigo = listaProdutos.Select(x => x.Produto?.CodigoProdutoEmbarcador).FirstOrDefault() ?? string.Empty;
            integracaoCarga.NumeroContainer = carga.Containeres ?? string.Empty;

            return integracaoCarga;
        }

        private void AtualizarSituacaoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem, string protocolo, string jSonRequest, string jSonResponse)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaIntegracao.SituacaoIntegracao = situacao;
            cargaIntegracao.ProblemaIntegracao = mensagem;

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jSonRequest, jSonResponse, "json");
            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private async Task AtualizarSituacaoCancelamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem, string protocolo, string jSonRequest, string jSonResponse)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCancelamentoIntegracao.SituacaoIntegracao = situacao;
            cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;
            cargaCancelamentoIntegracao.Protocolo = protocolo;

            servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jSonRequest, jSonResponse, "json");
            await repCargaCargaCancelamentoIntegracao.AtualizarAsync(cargaCancelamentoIntegracao);
        }

        private bool DeveUsarHeadersCustomizados()
        {
            return (_configuracaoIntegracaoYMS?.TipoAutenticacaoYMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacaoYMS.Basic)
                   || !string.IsNullOrWhiteSpace(_configuracaoIntegracaoYMS?.ParametrosAdicionais);
        }

        private static List<KeyValuePair<string, string>> ObterListaParametrosAdicionais(string parametrosAdicionais)
        {
            List<KeyValuePair<string, string>> lista = new List<KeyValuePair<string, string>>();

            if (parametrosAdicionais == null)
            {
                return lista;
            }

            string[] parametros = parametrosAdicionais.Trim().Split(';');
            int total = parametros.Length;
            for (int i = 0; i < total; i++)
            {
                string param = parametros[i].Trim();
                int pos = param.IndexOf('=');
                if (pos > 0)
                {
                    lista.Add(new KeyValuePair<string, string>(param.Substring(0, pos), param.Substring(pos + 1)));
                }
            }
            return lista;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaEventos ObterIntegracaoEvento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.YMS.IntegrarCargaEventos
            {
                CargaEventos = cargaEntregaEvento.EventoColetaEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.ChegadaNoAlvo ? "CHECK-IN" : cargaEntregaEvento.EventoColetaEntrega.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Confirma ? "CHECK-OUT" : string.Empty,
                DataEvento = cargaEntregaEvento.DataOcorrencia,
                Local = string.Empty
            };
        }        

        private HttpClient CriarRequisicao(string url, string usuario, string senha)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = new HttpClient
            {
                BaseAddress = new Uri(url)
            };

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var credenciais = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{usuario}:{senha}"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credenciais);

            return requisicao;
        }

        #endregion
    }
}
