using Dominio.Entidades.Embarcador.Cargas.MontagemCarga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte;
using Newtonsoft.Json;
using Repositorio;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.TelhaNorte
{
    public class IntegracaoTelhaNorte
    {
        private readonly UnitOfWork _unitOfWork;

        public IntegracaoTelhaNorte(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public string SimularIntegracaoCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, int pedidoRemover, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionar)
        {
            if (carga.Carregamento == null)
                return "";

            CarregamentoIntegracao carregamentoIntegracao = new CarregamentoIntegracao()
            {
                Carregamento = carga.Carregamento
            };

            string jsonRequisicao = string.Empty;
            string respostaEmTexto = string.Empty;

            try
            {

                Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte dadosRequisicao = ObterDadosRequisicao(carregamentoIntegracao, true);

                if (pedidoRemover > 0)
                {
                    RequisicaoTelhaNortePedido pedidoRemoverTelha = (from obj in dadosRequisicao.Pedidos where obj.ProtocoloPedido == pedidoRemover select obj).FirstOrDefault();
                    if (pedidoRemoverTelha != null)
                        dadosRequisicao.Pedidos.Remove(pedidoRemoverTelha);
                }

                if (pedidosAdicionar != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionar)
                    {
                        RequisicaoTelhaNortePedido pedidoAdicionarTelha = new RequisicaoTelhaNortePedido();
                        pedidoAdicionarTelha.NumeroRemessa = pedido.NumeroPedidoEmbarcador;
                        pedidoAdicionarTelha.ProtocoloPedido = pedido.Protocolo;
                        dadosRequisicao.Pedidos.Add(pedidoAdicionarTelha);
                    }
                }

                string xmlRequisicao = Utilidades.XML.Serializar(dadosRequisicao, true);

                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                HttpWebResponse resposta = EnviarRequisicao(jsonRequisicao, carregamentoIntegracao, dadosRequisicao);

                System.IO.Stream responseStream = resposta.GetResponseStream();

                using (var reader = new System.IO.StreamReader(responseStream, ASCIIEncoding.ASCII))
                {
                    respostaEmTexto = reader.ReadToEnd();
                    dynamic dynResposta = JsonConvert.DeserializeObject<dynamic>(respostaEmTexto);

                    string respostaString = ((string)dynResposta.mensagem).ToString().ObterSomenteNumerosELetrasComEspaco();
                    
                    if (!string.IsNullOrWhiteSpace(respostaString) && respostaString.Contains("Falha"))
                        return ((string)dynResposta.mensagem).ToString().ObterSomenteNumerosELetras();

                    IntegracaoCarregamento servicoIntegracaoCarregamento = new IntegracaoCarregamento(_unitOfWork);
                    CarregamentoIntegracao retornoCarregamentoIntegracao = servicoIntegracaoCarregamento.AdicionarIntegracaoCarregamento(carga.Carregamento, StatusCarregamentoIntegracao.Atualizar, TipoIntegracao.TelhaNorte);

                    return retornoCarregamentoIntegracao != null ? "" : "A telha norte não permite adição/remoção de pedidos na carga atual.";
                }

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Request: " + jsonRequisicao, "IntegracaoTelhaNorte");
                Servicos.Log.TratarErro("Response: " + respostaEmTexto, "IntegracaoTelhaNorte");
                Servicos.Log.TratarErro(excecao, "IntegracaoTelhaNorte");

                return "Retorno validação telha norte: " + excecao.Message;
                //throw new ServicoException("Ocorreu uma falha ao simular a integração com a telha norte.");
            }
        }

        public void IntegrarCarregamento(CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RetornoIntegracaoTelhaNorte objetoRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RetornoIntegracaoTelhaNorte()
            {
                MensagemErro = ""
            };

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte dadosRequisicao = ObterDadosRequisicao(carregamentoIntegracao);

                xmlRequisicao = Utilidades.XML.Serializar(dadosRequisicao, true);

                string jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                HttpWebResponse resposta = EnviarRequisicao(jsonRequisicao, carregamentoIntegracao, dadosRequisicao);

                System.IO.Stream responseStream = resposta.GetResponseStream();

                using (var reader = new System.IO.StreamReader(responseStream, ASCIIEncoding.ASCII))
                {
                    string respostaEmTexto = reader.ReadToEnd();
                    dynamic dynResposta = JsonConvert.DeserializeObject<dynamic>(respostaEmTexto);
                    string novoNumeroCarga = ((string)dynResposta.numero_carga).ToString();

                    if (!string.IsNullOrWhiteSpace(novoNumeroCarga) && carregamentoIntegracao.Carregamento != null)
                    {
                        Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
                        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                        };

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);

                        if (carga.Redespacho != null)
                        {
                            carga.CargaPossuiOutrosNumerosEmbarcador = true;

                            if (carga.CodigosAgrupados == null)
                                carga.CodigosAgrupados = new List<string>();

                            if (!carga.CodigosAgrupados.Contains(carga.CodigoCargaEmbarcador))
                                carga.CodigosAgrupados.Add(carga.CodigoCargaEmbarcador);
                        }

                        carregamentoIntegracao.Carregamento.NumeroCargaAlteradoViaIntegracao = true;
                        string codigoAntigo = carga.CodigoCargaEmbarcador;
                        carga.CodigoCargaEmbarcador = novoNumeroCarga;

                        repositorioCarregamento.Atualizar(carregamentoIntegracao.Carregamento);
                        repositorioCarga.Atualizar(carga);

                        Auditoria.Auditoria.Auditar(auditado, carga, $"Número da carga alterado pelo SAP de {codigoAntigo} para {novoNumeroCarga}.", _unitOfWork);
                    }
                }

                if (resposta.StatusCode == HttpStatusCode.OK)
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    carregamentoIntegracao.ProblemaIntegracao = string.Empty;
                }
                else
                {
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = resposta.StatusDescription;
                    objetoRetorno.MensagemErro = resposta.StatusDescription;

                    xmlRetorno = Utilidades.XML.Serializar(objetoRetorno, true);
                }
            }
            catch (Exception excecao)
            {
                string retorno = excecao.Message;

                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a TelhaNorte";
                carregamentoIntegracao.NumeroTentativas++;

                if (excecao is WebException && ((WebException)excecao).Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse errResp = ((WebException)excecao).Response;
                    using (System.IO.Stream respStream = errResp.GetResponseStream())
                    {
                        retorno = (new System.IO.StreamReader(respStream, Encoding.UTF8)).ReadToEnd();

                        if (!string.IsNullOrWhiteSpace(retorno))
                            carregamentoIntegracao.ProblemaIntegracao += $" ({retorno})";
                    }
                }

                objetoRetorno.MensagemErro = retorno;
                xmlRetorno = Utilidades.XML.Serializar(objetoRetorno, true);
            }

            servicoArquivoTransacao.Adicionar(carregamentoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RetornoIntegracaoTelhaNorte objetoRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RetornoIntegracaoTelhaNorte()
            {
                MensagemErro = ""
            };

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte dadosRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte()
                {
                    NumeroCarga = cargaCancelamentoIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador
                };

                xmlRequisicao = Utilidades.XML.Serializar(dadosRequisicao, true);
                HttpWebResponse resposta = EnviarRequisicaoCancelamentoCarga(cargaCancelamentoIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador);

                if (resposta.StatusCode == HttpStatusCode.OK)
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = string.Empty;
                }
                else
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = resposta.StatusDescription;
                    objetoRetorno.MensagemErro = resposta.StatusDescription;

                    xmlRetorno = Utilidades.XML.Serializar(objetoRetorno, true);
                }
            }
            catch (Exception excecao)
            {
                string retorno = excecao.Message;

                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a TelhaNorte";
                cargaCancelamentoIntegracao.NumeroTentativas++;

                if (excecao is WebException && ((WebException)excecao).Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse errResp = ((WebException)excecao).Response;
                    using (System.IO.Stream respStream = errResp.GetResponseStream())
                    {
                        retorno = (new System.IO.StreamReader(respStream, Encoding.UTF8)).ReadToEnd();

                        if (!string.IsNullOrWhiteSpace(retorno))
                            cargaCancelamentoIntegracao.ProblemaIntegracao += $" ({retorno})";
                    }
                }
            }

            servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            repositorioCargaCancelamentoIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        public void IntegrarPedidoCancelamentoReserva(PedidoCancelamentoReservaIntegracao pedidoCancelamentoReservaIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.PedidoCancelamentoReservaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            pedidoCancelamentoReservaIntegracao.DataIntegracao = DateTime.Now;

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNortePedido objetoRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNortePedido()
            {
                NumeroRemessa = pedidoCancelamentoReservaIntegracao.Pedido?.NumeroPedidoEmbarcador ?? ""
            };

            string xmlRequisicao = Utilidades.XML.Serializar(objetoRequisicao, true);

            string xmlRetorno = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte dadosIntegracao = ObterDadosIntegracao();

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RetornoIntegracaoTelhaNorte objetoRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RetornoIntegracaoTelhaNorte()
            {
                MensagemErro = ""
            };

            if (dadosIntegracao == null)
            {
                pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "A integração com a TelhaNorte não está configurada";
            }
            else
            {
                try
                {
                    HttpWebResponse resposta = EnviarRequisicaoCancelamentoPedido(pedidoCancelamentoReservaIntegracao.Pedido.NumeroPedidoEmbarcador, dadosIntegracao);

                    if (resposta.StatusCode == HttpStatusCode.OK)
                    {
                        pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = string.Empty;
                    }
                    else
                    {
                        pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = resposta.StatusDescription;
                        objetoRetorno.MensagemErro = resposta.StatusDescription;

                        xmlRetorno = Utilidades.XML.Serializar(objetoRetorno, true);
                    }
                }
                catch (Exception excecao)
                {
                    string retorno = excecao.Message;

                    pedidoCancelamentoReservaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pedidoCancelamentoReservaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a TelhaNorte";
                    pedidoCancelamentoReservaIntegracao.NumeroTentativas++;

                    if (excecao is WebException && ((WebException)excecao).Status == WebExceptionStatus.ProtocolError)
                    {
                        WebResponse errResp = ((WebException)excecao).Response;
                        using (System.IO.Stream respStream = errResp.GetResponseStream())
                        {
                            retorno = (new System.IO.StreamReader(respStream, Encoding.UTF8)).ReadToEnd();

                            if (!string.IsNullOrWhiteSpace(retorno))
                                pedidoCancelamentoReservaIntegracao.ProblemaIntegracao += $" ({retorno})";
                        }
                    }
                }
            }

            servicoArquivoTransacao.Adicionar(pedidoCancelamentoReservaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            repositorioCarregamentoIntegracao.Atualizar(pedidoCancelamentoReservaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private HttpWebResponse EnviarRequisicao(string jsonRequisicao, CarregamentoIntegracao carregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte dadosRequisicao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte dadosIntegracao = ObterDadosIntegracao();

            if (dadosIntegracao == null)
                return null;

            string barraAdicionar = dadosIntegracao.URLCarga[dadosIntegracao.URLCarga.Length - 1] == '/' ? "" : "/";

            string numeroCarga = dadosRequisicao.NumeroCarga;
            string url = (carregamentoIntegracao.Carregamento?.NumeroCargaAlteradoViaIntegracao ?? false) ? $"{dadosIntegracao.URLCarga}{barraAdicionar}{numeroCarga}" : dadosIntegracao.URLCarga;

            if (url.Last() == '/')
                url = url.Remove(url.Length - 1);

            byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(jsonRequisicao);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12;

            HttpWebRequest requisicao = (HttpWebRequest)HttpWebRequest.Create(url);

            requisicao.Method = "POST";
            requisicao.ContentLength = byteArrayDadosRequisicao.Length;
            requisicao.ContentType = "application/json";

            string credenciais = dadosIntegracao.Credencial;
            requisicao.Headers.Add("Authorization", $"Bearer {credenciais}");

            try
            {
                using (System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream())
                {
                    streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
                }

                return (HttpWebResponse)requisicao.GetResponse();
            }
            catch (Exception ex)
            {
                if (ex is WebException webException && webException.Response != null)
                {
                    using (HttpWebResponse respostaErro = (HttpWebResponse)webException.Response)
                    {
                        using (StreamReader leitor = new StreamReader(respostaErro.GetResponseStream()))
                        {
                            string respostaJsonErro = leitor.ReadToEnd();
                            dynamic respostaObjeto = JsonConvert.DeserializeObject(respostaJsonErro);
                            string mensagemErro = respostaObjeto?.mensagem ?? "Erro desconhecido.";
                            throw new Exception(mensagemErro);
                        }
                    }
                }

                throw;
            }
        }

        private HttpWebResponse EnviarRequisicaoCancelamentoCarga(string numeroCarga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte dadosIntegracao = ObterDadosIntegracao();

            if (dadosIntegracao == null)
                return null;

            string url = $"{dadosIntegracao.URLCarga}";

            if (url.Last() == '/')
                url = url.Remove(url.Length - 1);

            url += $"/{numeroCarga}";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12;

            HttpWebRequest requisicao = (HttpWebRequest)HttpWebRequest.Create(url);

            requisicao.Method = "DELETE";
            requisicao.ContentType = "application/json";

            string credenciais = dadosIntegracao.Credencial;
            requisicao.Headers.Add("Authorization", $"Bearer {credenciais}");

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();

            streamDadosRequisicao.Close();

            return (HttpWebResponse)requisicao.GetResponse();
        }

        private HttpWebResponse EnviarRequisicaoCancelamentoPedido(string numeroPedido, Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte dadosIntegracao)
        {
            string url = $"{dadosIntegracao.URLPedido}";

            if (url.Last() == '/')
                url = url.Remove(url.Length - 1);

            url += $"/{numeroPedido}";


            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12;

            HttpWebRequest requisicao = (HttpWebRequest)HttpWebRequest.Create(url);

            requisicao.Method = "DELETE";
            requisicao.ContentType = "application/json";

            string credenciais = dadosIntegracao.Credencial;
            requisicao.Headers.Add("Authorization", $"Bearer {credenciais}");

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();

            streamDadosRequisicao.Close();

            return (HttpWebResponse)requisicao.GetResponse();
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte ObterDadosIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorio = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorio.BuscarPrimeiroRegistro();

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLTelhaNorte))
                return null;

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.DadosIntegracaoTelhaNorte()
            {
                Credencial = ObterToken(configuracaoIntegracao.URLObterTokenTelhaNorte),
                URLCarga = configuracaoIntegracao.URLTelhaNorte,
                URLPedido = configuracaoIntegracao.URLPedidoTelhaNorte
            };

            return dados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte ObterDadosRequisicao(CarregamentoIntegracao carregamentoIntegracao, bool simular = false)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamentoIntegracao.Carregamento.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte requisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNorte()
            {
                CNPJTransportador = carga?.Empresa?.CNPJ ?? "",
                CPFMotorista = carga?.Motoristas?.FirstOrDefault()?.CPF ?? "",
                DataCarregamento = carregamentoIntegracao.Carregamento?.DataCarregamentoCarga,
                NumeroCarga = (carregamentoIntegracao.Carregamento?.NumeroCargaAlteradoViaIntegracao ?? false) ? carga?.CodigoCargaEmbarcador ?? "" : "",
                PlacaVeiculo = carga?.Veiculo?.Placa ?? "",
                ProtocoloCarga = carga?.Protocolo ?? 0,
                TipoOperacao = carga?.TipoOperacao?.CodigoIntegracao ?? "",
                TipoVeiculo = carga?.ModeloVeicularCarga?.CodigoIntegracao ?? "",
                LocalTransporte = carga?.Filial?.CodigoFilialEmbarcador,
                Usuario = carregamentoIntegracao.Carregamento?.SessaoRoteirizador?.Usuario?.Login,
                Simular = simular ? "true" : "false",
                Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.TelhaNorte.RequisicaoTelhaNortePedido> { }
            };

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = carga != null ? repositorioCargaPedido.BuscarPorCarga(carga.Codigo) : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
            {
                requisicao.Pedidos.Add(new RequisicaoTelhaNortePedido()
                {
                    ProtocoloPedido = cargaPedido.Pedido.Protocolo,
                    NumeroRemessa = cargaPedido.Pedido.NumeroPedidoEmbarcador
                });
            }
            
            return requisicao;
        }

        private string ObterToken(string url)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro repositorioParametros = new Repositorio.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro(_unitOfWork);
            System.Collections.Generic.List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro> parametros = repositorioParametros.Buscar();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12;

            if (url.Last() == '/')
                url = url.Remove(url.Length - 1);

            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro parametro in parametros)
                request.AddParameter(parametro.Chave, parametro.Valor, ParameterType.GetOrPost);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return "";

            dynamic dynConteudo = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return ((string)dynConteudo.access_token).ToString();
        }

        #endregion
    }
}
