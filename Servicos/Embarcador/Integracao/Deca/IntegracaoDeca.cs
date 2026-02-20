using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Deca
{
    public class IntegracaoDeca
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoDeca(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void Integrar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracaoFluxoPatio)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDeca repositorioIntegracaoDexcoDecoMadeira = new Repositorio.Embarcador.Configuracoes.IntegracaoDeca(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca dadosIntegracao = repositorioIntegracaoDexcoDecoMadeira.BuscarPrimeiroRegistro();

            if (dadosIntegracao == null || !dadosIntegracao.PossuiIntegracaoDeca)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioIntegracaoFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);


            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            integracaoFluxoPatio.NumeroTentativas += 1;
            integracaoFluxoPatio.DataIntegracao = DateTime.Now;

            try
            {
                xmlRequisicao = Utilidades.XML.Serializar(ObterVeiculoManager(integracaoFluxoPatio));
                string endPoint = dadosIntegracao.URLAutenticacaoDeca + "MultiSoftware/GerenciarVeiculos";

                HttpClient cliente = ObterClienteRequisicao(dadosIntegracao, endPoint);
                StringContent dadosEnviar = new StringContent(xmlRequisicao, Encoding.UTF8, "application/xml");
                HttpResponseMessage retornoRequisicao = cliente.PostAsync(endPoint, dadosEnviar).Result;

                xmlRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Erro ao tentar fazer a integração");

                integracaoFluxoPatio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoFluxoPatio.ProblemaIntegracao = string.Empty;
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
                integracaoFluxoPatio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Deco";
            }

            servicoArquivoTransacao.Adicionar(integracaoFluxoPatio, xmlRequisicao, xmlRetorno, "xml");
            repositorioIntegracaoFluxoPatio.Atualizar(integracaoFluxoPatio);
        }

        public void IntegrarInicioViagem(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracaoFluxoPatio)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDeca repositorioIntegracaoDexcoDecoMadeira = new Repositorio.Embarcador.Configuracoes.IntegracaoDeca(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca dadosIntegracao = repositorioIntegracaoDexcoDecoMadeira.BuscarPrimeiroRegistro();

            if (dadosIntegracao == null || !dadosIntegracao.PossuiIntegracaoDeca)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioIntegracaoFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);


            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            integracaoFluxoPatio.NumeroTentativas += 1;
            integracaoFluxoPatio.DataIntegracao = DateTime.Now;

            try
            {
                jsonRequisicao = JsonConvert.SerializeObject(ObterIntegracaoInicioViagem(integracaoFluxoPatio), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                string endPoint = dadosIntegracao.URLInicioViagemDeca;

                HttpClient cliente = ObterClienteRequisicao(dadosIntegracao, endPoint);
                StringContent dadosEnviar = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = cliente.PostAsync(endPoint, dadosEnviar).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                dynamic response = JsonConvert.DeserializeObject(jsonRetorno);

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Ocorreu uma falha ao gerar a integração.");

                if (response["Result"].Codigo != "200")
                    throw new ServicoException(response["Result"].Mensagem);

                integracaoFluxoPatio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoFluxoPatio.ProblemaIntegracao = "Integrado com sucesso.";
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
                integracaoFluxoPatio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Deco";
            }

            servicoArquivoTransacao.Adicionar(integracaoFluxoPatio, jsonRequisicao, jsonRetorno, "json");
            repositorioIntegracaoFluxoPatio.Atualizar(integracaoFluxoPatio);
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, int codigoClienteURLAcesso)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
            };


            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedidoOcorrenciaColetaEntrega.Pedido.Codigo);

            if (notasFiscais == null || notasFiscais.Count == 0)
            {
                httpRequisicaoResposta.mensagem = "Nenhuma NFe localizada.";
            }
            else
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoDeca repositorioConfiguracaoDeca = new Repositorio.Embarcador.Configuracoes.IntegracaoDeca(_unitOfWork);
                var dadosIntegracao = repositorioConfiguracaoDeca.BuscarPrimeiroRegistro();

                if (dadosIntegracao == null || string.IsNullOrWhiteSpace(dadosIntegracao.URLAutenticacaoDeca))
                {
                    httpRequisicaoResposta.mensagem = "Não existe configuração de integração disponível para a Deca Madeira.";
                }
                else
                {
                    string jsonRequisicao = string.Empty;
                    string jsonRetorno = string.Empty;

                    try
                    {
                        dynamic objetoRetorno = new
                        {
                            Transporte = pedidoOcorrenciaColetaEntrega.Carga.CodigoCargaEmbarcador,
                            NDocMulti = pedidoOcorrenciaColetaEntrega.Carga.Protocolo.ToString(),
                            Empresa = "DEXCO" + (pedidoOcorrenciaColetaEntrega.Carga.Filial?.Descricao ?? string.Empty),
                            Centro = pedidoOcorrenciaColetaEntrega.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                            N_Nfe = "NFE " + notasFiscais.FirstOrDefault().Numero.ToString(),
                            Serie = "Serie " + notasFiscais.FirstOrDefault().SerieOuSerieDaChave,
                            Cliente = "SAP",
                            CodOcorrencia = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CodigoIntegracao,
                            DenomOcorrencia = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.Descricao,
                            Grupos = "GRP",
                            OcorrenciaFinal = "",
                            NAgenteFrete = pedidoOcorrenciaColetaEntrega.Carga.Empresa.CNPJ_SemFormato,
                            DtOcorrencia = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("dd/MM/yyyy"),
                            HrOcorrencia = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("HH:mm")
                        };

                        string endPoint = dadosIntegracao.URLAutenticacaoDeca + "multiembarcador/OcorrenciaEntrega";
                        jsonRequisicao = JsonConvert.SerializeObject(objetoRetorno, Formatting.Indented);

                        HttpClient cliente = ObterClienteRequisicao(dadosIntegracao, endPoint);
                        var content = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                        httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                        
                        HttpResponseMessage retornoRequisicao = cliente.PostAsync(endPoint, content).Result;

                        jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                        if (!retornoRequisicao.IsSuccessStatusCode)
                            throw new ServicoException("API retornou falha");

                        httpRequisicaoResposta.conteudoResposta = jsonRetorno;
                        httpRequisicaoResposta.httpStatusCode = retornoRequisicao.StatusCode;

                        if (retornoRequisicao.IsSuccessStatusCode)
                        {
                            string retorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
                                if (retornoJSON == null)
                                {
                                    httpRequisicaoResposta.mensagem = "Integração Deca não retornou JSON.";
                                }
                                else
                                {
                                    DateTime dataIntegracao = DateTime.Now;
                                    httpRequisicaoResposta.mensagem = (string)retornoJSON.Result.Mensagem ?? string.Empty;
                                    httpRequisicaoResposta.sucesso = (string)retornoJSON.Result.Codigo == "200";
                                }
                            }
                            else
                            {
                                httpRequisicaoResposta.mensagem = "Integração Deca não teve retorno.";
                            }
                        }
                        else
                        {
                            httpRequisicaoResposta.mensagem = "Retorno integração Deca: " + retornoRequisicao.StatusCode.ToString();
                        }

                        if (!httpRequisicaoResposta.sucesso && string.IsNullOrWhiteSpace(httpRequisicaoResposta.mensagem))
                            httpRequisicaoResposta.mensagem = "Integração Deca não retornou sucesso.";
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro("Request: " + jsonRequisicao, "IntegracaoDexcoMadeira");
                        Log.TratarErro("Response: " + jsonRetorno, "IntegracaoDexcoMadeira");
                        Log.TratarErro(excecao, "IntegracaoDexcoMadeira");
                        httpRequisicaoResposta.mensagem = "API Deca não retornou sucesso, verifique o historico de integração;";
                        httpRequisicaoResposta.sucesso = false;
                        httpRequisicaoResposta.conteudoResposta = jsonRetorno;
                    }
                }
            }

            return httpRequisicaoResposta;
        }

        public void ConsultarPesagensBalanca(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoDeca repositorioIntegracaoDeca = new Repositorio.Embarcador.Configuracoes.IntegracaoDeca(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca configuracaoIntegracaoDeca = repositorioIntegracaoDeca.Buscar();
            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = integracaoPesagem.Pesagem;

            string jsonRetorno = "";

            integracaoPesagem.DataIntegracao = DateTime.Now;
            integracaoPesagem.NumeroTentativas++;

            try
            {
                if (!(configuracaoIntegracaoDeca?.PossuiIntegracaoBalanca ?? false))
                    throw new ServicoException("Não possui configuração de balança para a Deca.");

                Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca = integracaoPesagem.Balanca;

                if (string.IsNullOrWhiteSpace(filialBalanca?.HostConsultaBalanca) || filialBalanca.PortaBalanca == 0)
                    throw new ServicoException("Balança não foi configurada na etapa da Filial, necessário gerar novo fluxo de pátio para vincular a balança.");

                decimal peso = ObterPesoBalancaComRegras(filialBalanca, configuracaoIntegracaoDeca, integracaoPesagem, pesagem, out jsonRetorno);
                if (peso <= 0)
                    throw new ServicoException("Favor realizar nova consulta, balança não retornou valores válidos para pesagem.");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = integracaoPesagem.Pesagem.Guarita;

                if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemInicial)
                {
                    pesagem.StatusBalanca = StatusBalanca.TicketCriado;
                    pesagem.PesoInicial = peso;
                    guarita.PesagemInicial = peso;
                    guarita.UsuarioPesagemInicial = null;
                    guarita.BalancaPesagemInicial = filialBalanca;
                    guarita.OrigemPesagemInicial = OrigemPesagemGuarita.Integracao;
                }
                else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemFinal)
                {
                    pesagem.PesoFinal = peso;
                    guarita.PesagemFinal = peso;
                    guarita.UsuarioPesagemFinal = null;
                    guarita.BalancaPesagemFinal = filialBalanca;
                    guarita.OrigemPesagemFinal = OrigemPesagemGuarita.Integracao;
                }

                if (pesagem.PesoFinal > 0)
                    pesagem.StatusBalanca = StatusBalanca.Encerrado;

                repositorioPesagem.Atualizar(pesagem);
                repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita);

                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoPesagem.ProblemaIntegracao = "Integração realizada com sucesso";
            }
            catch (ServicoException ex)
            {
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da Deca";

                if (!string.IsNullOrWhiteSpace(jsonRetorno) && (jsonRetorno.Contains("500") || jsonRetorno.Contains("502") || jsonRetorno.Contains("503")))
                    integracaoPesagem.ProblemaIntegracao = "Houve erro na conexão com a balança, refaça a consulta";
            }

            servicoArquivoTransacao.Adicionar(integracaoPesagem, "", jsonRetorno, "json");

            repositorioPesagemIntegracao.Atualizar(integracaoPesagem);

            if (integracaoPesagem.SituacaoIntegracao == SituacaoIntegracao.Integrado)
            {
                Servicos.Embarcador.Hubs.FluxoPatio hubFluxoPatio = new Servicos.Embarcador.Hubs.FluxoPatio();
                if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemInicial)
                    hubFluxoPatio.InformarPesagemInicialAtualizada(pesagem);
                else
                    hubFluxoPatio.InformarPesagemFinalAtualizada(pesagem);
            }
        }

        public bool ValidarEtapaIntegracao(EtapaFluxoGestaoPatio etapa)
        {
            switch (etapa)
            {
                case EtapaFluxoGestaoPatio.Guarita:
                case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                case EtapaFluxoGestaoPatio.InicioViagem:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Métodos Privados

        private HttpClient CriarRequisicaoBalanca(string url, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDeca));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("X-API-Key", token);

            return requisicao;
        }

        private HttpClient ObterClienteRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca integracaDeca, string endPoint)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDeca));

            client.BaseAddress = new Uri(endPoint);
            client.SetBasicAuthentication(integracaDeca.UsuarioDeca, integracaDeca.SenhaDeca);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            return client;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.VeiculoManager ObterVeiculoManager(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracaoFluxoPatio)
        {
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioGestaoPatio.BuscarConfiguracao();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.VeiculoManager veiculoManager = new Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.VeiculoManager()
            {
                Veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.Veiculo()
                {
                    Action = configuracaoGestaoPatio?.ChegadaVeiculoAction ?? string.Empty,
                    CodigoTransportador = integracaoFluxoPatio?.Carga?.CodigoCargaEmbarcador ?? string.Empty
                },
                PlacaVeiculo = integracaoFluxoPatio?.Carga?.Veiculo.Placa ?? string.Empty,
                PlacaCarreta = (integracaoFluxoPatio?.Carga?.VeiculosVinculados?.Count ?? 0) > 0 ? string.Join(";", integracaoFluxoPatio?.Carga?.VeiculosVinculados?.Select(o => o.Placa).ToList()) : string.Empty,
                Condutor = integracaoFluxoPatio?.Carga?.NomeMotoristas ?? string.Empty,
            };

            if (integracaoFluxoPatio.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.Guarita)
            {
                veiculoManager.Veiculo.Action = configuracaoGestaoPatio.GuaritaEntradaAction;
            }
            else if (integracaoFluxoPatio.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.ChegadaVeiculo)
            {
                veiculoManager.Veiculo.Action = configuracaoGestaoPatio.ChegadaVeiculoAction;
            };

            return veiculoManager;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.IntegracaoInicioViagem ObterIntegracaoInicioViagem(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracaoFluxoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorUnicaCarga(integracaoFluxoPatio.Carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.IntegracaoInicioViagem integracaoInicioViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.IntegracaoInicioViagem()
            {
                Documento = integracaoFluxoPatio?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                DataInicioViagem = fluxoGestaoPatio.DataInicioViagem.HasValue ? fluxoGestaoPatio.DataInicioViagem.Value.ToString("dd.MM.yyyy") : string.Empty,
                Hora = fluxoGestaoPatio.DataInicioViagem.HasValue ? fluxoGestaoPatio.DataInicioViagem.Value.ToString("HH:mm:ss") : string.Empty,
            };

            return integracaoInicioViagem;
        }

        private decimal ObterPesoBalancaComRegras(Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca configuracaoIntegracaoDeca, Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem, Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, out string jsonRetorno)
        {
            int quantidadeRegistrosConsulta = filialBalanca.QuantidadeConsultasPesoBalanca > 0 ? filialBalanca.QuantidadeConsultasPesoBalanca : 1;

            string urlConsulta = $"{configuracaoIntegracaoDeca.URLBalanca}?host={filialBalanca.HostConsultaBalanca}&port={filialBalanca.PortaBalanca}&pulsesize={quantidadeRegistrosConsulta}";
            HttpClient requisicao = CriarRequisicaoBalanca(urlConsulta, configuracaoIntegracaoDeca.TokenBalanca);

            //Exemplo de retorno com sucesso da Deca
            //string urlConsulta = "https://dexcoweighing.prd.cloud.dex.co/api/balanca?host=10.154.30.16&port=30001&pulsesize=3";
            //HttpClient requisicao = CriarRequisicaoBalanca(urlConsulta, "gsBGLoKGtI30czvGc21t49gCGdgZtBGF5ahIZ8ZA");

            jsonRetorno = null;
            List<decimal> listaPesos = new List<decimal>();

            int consultasRealizadas = 1;
            while (consultasRealizadas <= quantidadeRegistrosConsulta)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.RetornoPesagem> retorno = RealizarConsultaIntegracaoPesagem(integracaoPesagem, requisicao, urlConsulta, out jsonRetorno);
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.RetornoPesagem retornoPesagem in retorno)
                    listaPesos.Add(ObterPesoConvertido(retornoPesagem.Peso, filialBalanca));

                if (quantidadeRegistrosConsulta == 1)
                    return listaPesos.FirstOrDefault();

                decimal menorPeso = listaPesos.Min();
                decimal maiorPeso = listaPesos.Max();

                if ((menorPeso <= 0 && maiorPeso <= 0) || menorPeso < 0)
                    throw new ServicoException("Favor realizar nova consulta, balança não retornou valores válidos para pesagem");

                listaPesos = listaPesos.Where(i => i != 0).ToList();//Retira os pesos zerados para consultar novamente
                menorPeso = listaPesos.Min();

                if (consultasRealizadas == quantidadeRegistrosConsulta && listaPesos.Count < quantidadeRegistrosConsulta)
                    throw new ServicoException($"Houve inconsitência na pesagem, realize nova pesagem. Em {quantidadeRegistrosConsulta} consultas retornou apenas {listaPesos.Count} valores válidos");
                else if (listaPesos.Count >= quantidadeRegistrosConsulta)
                {
                    decimal percentualDiferenca = menorPeso > 0 ? ((maiorPeso - menorPeso) / menorPeso) * 100 : (decimal)99.99;
                    if (percentualDiferenca > filialBalanca.PercentualToleranciaPesoBalanca)
                        throw new ServicoException("Houve inconsitência na pesagem, realize nova pesagem.");

                    consultasRealizadas = quantidadeRegistrosConsulta;
                }

                consultasRealizadas++;
            }

            decimal peso = listaPesos.LastOrDefault();

            //ValidarToleranciaPesagemEntradaSaida(filialBalanca, integracaoPesagem, pesagem, peso);

            return peso;
        }

        private decimal ObterPesoConvertido(string retornoPeso, Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca)
        {
            //if (retornoPeso.FirstOrDefault().Equals('i')|| retornoPeso.FirstOrDefault().Equals('q'))//Para tratar casos que não recebe o i e q em uma balança específica
            //    retornoPeso = retornoPeso.Remove(0, 1);

            if (retornoPeso.Length != filialBalanca.TamanhoRetornoBalanca && filialBalanca.TamanhoRetornoBalanca > 0)
                return 0; //throw new ServicoException($"Tamanho do retorno do peso {retornoPeso.Length} diferente do configurado na balança da filial {filialBalanca.TamanhoRetornoBalanca}!");

            int posicaoInicioPesoBalanca = filialBalanca.PosicaoInicioPesoBalanca - 1;
            int posicaoFimPesoBalanca = filialBalanca.PosicaoFimPesoBalanca - 1;

            if (posicaoInicioPesoBalanca > 0 && posicaoFimPesoBalanca > 0 && posicaoFimPesoBalanca <= retornoPeso.Length)
            {
                retornoPeso = retornoPeso.Substring(posicaoInicioPesoBalanca, posicaoFimPesoBalanca - posicaoInicioPesoBalanca);
                retornoPeso = retornoPeso.ObterSomenteNumeros();

                if (filialBalanca.CasasDecimaisPesoBalanca > 0)
                {
                    string valor = retornoPeso.Substring(0, retornoPeso.Length - filialBalanca.CasasDecimaisPesoBalanca);
                    string valorDecimal = retornoPeso.Substring(retornoPeso.Length - filialBalanca.CasasDecimaisPesoBalanca, filialBalanca.CasasDecimaisPesoBalanca);
                    return $"{valor}.{valorDecimal}".ToDecimal();
                }

                return retornoPeso.ToDecimal();
            }

            return retornoPeso.ObterSomenteNumeros().ToDecimal();
        }

        private void ValidarToleranciaPesagemEntradaSaida(Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca, Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem, Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, decimal peso)
        {
            if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemInicial && filialBalanca.PercentualToleranciaPesagemEntrada > 0)
            {
                decimal pesoCarga = integracaoPesagem.Pesagem.Guarita.CargaBase.DadosSumarizados?.PesoTotal ?? 0;
                decimal capacidade = integracaoPesagem.Pesagem.Guarita.CargaBase.ModeloVeicularCarga?.CapacidadePesoTransporte ?? 0;
                if (pesoCarga > 0 && capacidade > 0 && peso > 0)
                {
                    decimal calculoPercentual = ((peso + pesoCarga) / capacidade) * 100;
                    decimal diferencaDoPercentual = calculoPercentual - 100;
                    if (diferencaDoPercentual > 0 && diferencaDoPercentual > filialBalanca.PercentualToleranciaPesagemEntrada)
                        throw new ServicoException($"(({peso.ToString("n2")} + {pesoCarga.ToString("n2")}) / {capacidade.ToString("n2")}) * 100 = {calculoPercentual.ToString("n2")}% = {diferencaDoPercentual.ToString("n2")}% a mais da capacidade ou {peso + pesoCarga - capacidade} KG a mais\nTolerância da diferença por filial\nNecessário refazer o pesagem");
                }
            }
            else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.PesagemFinal && filialBalanca.PercentualToleranciaPesagemSaida > 0)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

                decimal pesoEntrada = pesagem.PesoInicial;
                decimal pesoNF = repositorioPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(integracaoPesagem.Pesagem.Guarita.Carga.Codigo);
                if (pesoNF > 0 && pesoEntrada > 0 && peso > 0)
                {
                    decimal calculoPercentual = (((peso - pesoEntrada) / pesoNF) - 1) * 100;
                    if (calculoPercentual > 0 && calculoPercentual > filialBalanca.PercentualToleranciaPesagemSaida)
                        throw new ServicoException($"((({peso.ToString("n2")} - {pesoEntrada.ToString("n2")}) / {pesoNF.ToString("n2")}) - 1) * 100 = {calculoPercentual.ToString("n2")}% a mais da capacidade ou {peso - pesoEntrada - pesoNF} KG a mais\nTolerância da diferença por filial\nNecessário refazer o pesagem");
                }
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.RetornoPesagem> RealizarConsultaIntegracaoPesagem(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem, HttpClient requisicao, string urlConsulta, out string jsonRetorno)
        {
            HttpResponseMessage retornoRequisicao = requisicao.GetAsync(urlConsulta).Result;
            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                throw new ServicoException($"Falha ao conectar no WS Deca: {retornoRequisicao.StatusCode}");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.RetornoPesagem> retorno = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Deca.RetornoPesagem>>(jsonRetorno);

            if ((retorno?.Count ?? 0) == 0)
                throw new ServicoException("Problemas ao conectar, lista de pesos retornada vazia!");

            if (retorno.Any(o => string.IsNullOrWhiteSpace(o.Peso)))
                throw new ServicoException("Problemas ao conectar, tag 'weight' não retornada!");

            Log.TratarErro($"Retorno balança integração {integracaoPesagem.Codigo}: { jsonRetorno}", "IntegracaoDeca");

            return retorno;
        }

        #endregion
    }
}
