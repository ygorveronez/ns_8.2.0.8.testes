using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.MundialRisk
{
    public class IntegracaoMundialRisk
    {
        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            string jsonResponse = string.Empty;
            string jsonPost = string.Empty;

            try
            {
                if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoMundialRisk || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioMundialRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaMundialRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoMundialRisk))
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a MundialRisk.";
                    cargaIntegracao.NumeroTentativas += 1;
                    cargaIntegracao.DataIntegracao = DateTime.Now;
                    repCargaIntegracao.Atualizar(cargaIntegracao);

                    return;
                }

                string urlWebService = configuracaoIntegracao.URLHomologacaoMundialRisk;

                if (cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                    urlWebService = configuracaoIntegracao.URLProducaoMundialRisk;

                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFes = cargaIntegracao.Carga.CargaMDFes?.FirstOrDefault() ?? null;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaIntegracao.Carga.Pedidos.FirstOrDefault();
                Dominio.Entidades.Usuario motorista = null;
                if (cargaIntegracao.Carga.Motoristas != null && cargaIntegracao.Carga.Motoristas.Count > 0)
                    motorista = cargaIntegracao.Carga.Motoristas.FirstOrDefault();

                int.TryParse(cargaIntegracao.Carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco, out int codigoPGR);
                int.TryParse(cargaIntegracao.Carga.Rota?.CodigoIntegracao, out int codigoRota);
                int.TryParse(cargaIntegracao.Carga.TipoOperacao?.ProdutoMundialRisk, out int codigoProduto);
                if (codigoProduto == 0)
                    codigoProduto = 1;

                decimal valorTotalNotas = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(cargaIntegracao.Carga.Codigo);

                DateTime? dataPrevisaoFim = cargaPedido.Pedido.PrevisaoEntrega;

                if (!dataPrevisaoFim.HasValue && cargaPedido.Pedido.DataPrevisaoSaida.HasValue && (cargaIntegracao.Carga.Rota?.TempoDeViagemEmMinutos ?? 0) > 0)
                    dataPrevisaoFim = cargaPedido.Pedido.DataPrevisaoSaida.Value.AddMinutes(cargaIntegracao.Carga.Rota.TempoDeViagemEmMinutos);

                Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.viagem requisicaoMonitoramento = new Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.viagem()
                {
                    viag_codigo_externo = cargaIntegracao.Carga.Codigo,
                    documento_transportador = cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraMundialRisk,
                    cnpj_gr = cargaIntegracao.Carga.TipoOperacao?.CNPJClienteMundialRisk,
                    cnpj_emba = null,
                    viag_numero_manifesto = cargaMDFes != null ? cargaMDFes?.MDFe?.Numero.ToString("D") ?? null : null,
                    viag_ttra_codigo = codigoProduto,
                    viag_observacao = cargaIntegracao.Carga?.Operador?.Nome ?? "",
                    veiculos = cargaIntegracao.Carga.Veiculo != null ? new List<Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.veiculo>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.veiculo()
                    {
                        placa = cargaIntegracao.Carga.Veiculo?.Placa
                    }
                } : null,
                    motoristas = motorista != null ? new List<Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.motorista>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.motorista()
                    {
                        cpf_moto = motorista?.CPF ?? string.Empty
                    }
                } : null,
                    viag_previsao_inicio = cargaPedido.Pedido.DataPrevisaoSaida.HasValue ? cargaPedido.Pedido.DataPrevisaoSaida.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    viag_previsao_fim = dataPrevisaoFim?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    viag_pgpg_codigo = codigoPGR,
                    viag_valor_carga = valorTotalNotas,
                    rota_codigo = codigoRota,
                    origem = null,
                    destino = null
                };

                //if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                //{
                //    requisicaoMonitoramento.viag_previsao_inicio = cargaIntegracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss");
                //    requisicaoMonitoramento.viag_previsao_fim = "";
                //}

                Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.viagens viagens = new Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.viagens();
                viagens.viagem = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.viagem>();
                viagens.viagem.Add(requisicaoMonitoramento);

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMundialRisk));
                client.Timeout = TimeSpan.FromMinutes(5);
                client.BaseAddress = new Uri(urlWebService);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var byteArray = Encoding.ASCII.GetBytes(configuracaoIntegracao.UsuarioMundialRisk + ":" + configuracaoIntegracao.SenhaMundialRisk);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                jsonPost = JsonConvert.SerializeObject(viagens, Formatting.Indented);

                StringContent content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(urlWebService, content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (retorno.success != null && retorno.success.Count > 0)
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Sucesso ";
                        cargaIntegracao.Protocolo = "";

                        foreach (var sucesso in retorno.success)
                        {
                            cargaIntegracao.Protocolo += "\n Nº: " + sucesso.cod_viagem.ToString();
                            cargaIntegracao.ProblemaIntegracao += "\n Cód. Viagem: " + sucesso.cod_viagem.ToString() + " Status: " + sucesso.status.ToString();
                        }
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Erro(s): ";

                        foreach (dynamic erro in retorno.error)
                            cargaIntegracao.ProblemaIntegracao += "\n Campo: " + erro.campo.ToString() + " Menssagem: " + erro.mensagem.ToString();
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (!string.IsNullOrWhiteSpace(jsonResponse) && jsonResponse.Contains("errors"))
                    {
                        dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                        cargaIntegracao.ProblemaIntegracao = "Erro(s): ";

                        foreach (dynamic erro in retorno.errors)
                            cargaIntegracao.ProblemaIntegracao += "\n " + erro.message.ToString();
                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = "Ocorreu um problema ao processar a SM, verifique o arquivo de retorno da integração para maiores detalhes.";
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração.";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonPost, "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = cargaIntegracao.DataIntegracao,
                Mensagem = cargaIntegracao.ProblemaIntegracao
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracao.TipoIntegracao.Codigo);

            if (cargaIntegracao == null || string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo) || string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cargaIntegracao.Protocolo)) || configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoMundialRisk || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioMundialRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaMundialRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoMundialRisk))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a MundialRisk.";
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;
                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoMundialRisk;

            if (cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoMundialRisk;

            urlWebService += "/" + Utilidades.String.OnlyNumbers(cargaIntegracao.Protocolo);

            cargaCancelamentoIntegracao.NumeroTentativas += 1;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCancelamentoIntegracao.CargaCancelamento.Carga.Pedidos.FirstOrDefault();
            Dominio.Entidades.Usuario motorista = cargaCancelamentoIntegracao.CargaCancelamento.Carga.Motoristas.FirstOrDefault();

            int.TryParse(cargaCancelamentoIntegracao.CargaCancelamento.Carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco, out int codigoPGR);
            int.TryParse(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Rota?.CodigoIntegracao, out int codigoRota);
            decimal valorTotalNotas = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.atualizarStatusViagem atualizarStatusViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.atualizarStatusViagem()
            {
                status_viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.MundialRisk.status_viagem() { id_novo_status = 2 }
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMundialRisk));
            client.BaseAddress = new Uri(urlWebService);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var byteArray = Encoding.ASCII.GetBytes(configuracaoIntegracao.UsuarioMundialRisk + ":" + configuracaoIntegracao.SenhaMundialRisk);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            string jsonPost = JsonConvert.SerializeObject(atualizarStatusViagem, Formatting.Indented);
            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PutAsync(urlWebService, content).Result;

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonPost, "json", unitOfWork);
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.Data = cargaCancelamentoIntegracao.DataIntegracao;

            string jsonResponse = result.Content.ReadAsStringAsync().Result;
            Servicos.Log.TratarErro($"Response: {jsonResponse}", "IntegracaoMundialRisk");

            if (result.IsSuccessStatusCode)
            {
                var retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (retorno == null)
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Erro: Problemas na estrutura do arquivo";
                    cargaCancelamentoIntegracao.NumeroTentativas++;
                    arquivoIntegracao.Mensagem = "Erro: Problemas na estrutura do arquivo";
                    Servicos.Log.TratarErro("Erro na integração: " + arquivoIntegracao.Mensagem);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);
                }
                else if (retorno.data != null && retorno.data.sucesso != null)
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = retorno.data.sucesso.ToString();
                    arquivoIntegracao.Mensagem = retorno.data.sucesso.ToString();
                    cargaCancelamentoIntegracao.Protocolo = retorno.data.sucesso.ToString();

                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);
                }
                else if (retorno != null && retorno.sucesso != null)
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Sucesso: ";
                    cargaCancelamentoIntegracao.NumeroTentativas++;
                    arquivoIntegracao.Mensagem = "Sucesso: ";

                    foreach (var sucess in retorno.sucesso)
                    {
                        try
                        {
                            if (sucess.cod != null)
                                cargaCancelamentoIntegracao.Protocolo = sucess.cod.ToString();
                            if (sucess.mensagem != null)
                            {
                                cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Menssagem: " + sucess.mensagem.ToString();
                                arquivoIntegracao.Mensagem += "\n Menssagem: " + sucess.mensagem.ToString();
                            }
                            else
                            {
                                cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Menssagem não catalogada ";
                                arquivoIntegracao.Mensagem += "\nMenssagem não catalcatalogadaogado ";
                            }
                        }
                        catch
                        {
                            cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Menssagem não catalogada ";
                            arquivoIntegracao.Mensagem += "\n Menssagem não catalogada ";
                        }

                    }
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);
                }
                else
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Erro(s): ";
                    cargaCancelamentoIntegracao.NumeroTentativas++;
                    arquivoIntegracao.Mensagem = "Erro(s): ";

                    try
                    {
                        if (retorno != null && retorno.error != null)
                        {
                            foreach (var erro in retorno.error)
                            {
                                try
                                {
                                    if (erro.mensagem != null)
                                    {
                                        cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Menssagem: " + erro.mensagem.ToString();
                                        arquivoIntegracao.Mensagem += "\n Menssagem: " + erro.mensagem.ToString();
                                    }
                                    else
                                    {
                                        cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Menssagem não catalogado ";
                                        arquivoIntegracao.Mensagem += "\n Menssagem não catalogado ";
                                    }
                                }
                                catch
                                {
                                    cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Menssagem não catalogado ";
                                    arquivoIntegracao.Mensagem += "\n Menssagem não catalogado ";
                                }
                            }
                        }
                        else
                        {
                            cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Erro não catalogado.";
                            arquivoIntegracao.Mensagem += "\n Erro não catalogado.";
                        }
                    }
                    catch
                    {
                        cargaCancelamentoIntegracao.ProblemaIntegracao += "\n Erro não catalogado.";
                        arquivoIntegracao.Mensagem += "\n Erro não catalogado.";
                    }

                    Servicos.Log.TratarErro("Erro na integração: " + arquivoIntegracao.Mensagem);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(result.Content.ReadAsStringAsync().Result, "json", unitOfWork);
                }
            }
            else
            {
                var retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Problema de estrutura de arquivo";

                arquivoIntegracao.Mensagem = "Problema de estrutura de arquivo";
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);
            }

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }


        public class LoggingHandler : DelegatingHandler
        {
            public LoggingHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Servicos.Log.TratarErro("Request: " + request.ToString(), "IntegracaoMundialRisk");

                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                Servicos.Log.TratarErro("Response: " + response.ToString(), "IntegracaoMundialRisk");

                return response;
            }
        }
    }
}
