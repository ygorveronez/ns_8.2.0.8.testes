using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Cebrace
{
    public class IntegracaoCebrace
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCebrace _configuracaoIntegracaoCebrace;

        #endregion Atributos Privados

        #region Construtores

        public IntegracaoCebrace(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Canhotos

        public void EnviarCteCanhoto(Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao cteCanhotoIntegracao)
        {
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cteCanhotoIntegracao.NumeroTentativas++;
            cteCanhotoIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Canhoto> informacaoEnvios = ObterListaCanhotoIntegracao(cteCanhotoIntegracao).GetAwaiter().GetResult();

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Canhoto informacaoEnvio in informacaoEnvios)
                {
                    List<string> erros = ValidarCanhotoIntegracao(informacaoEnvio);
                    if (erros.Count > 0)
                        throw new ServicoException(erros.FirstOrDefault());

                    respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/tms/ConfirmarRecebimento");

                    if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                    {
                        JObject json = JObject.Parse(respostaHttp.conteudoResposta);
                        string response = json["rfc:ZTMS_CONFIRMAR_RECEBIMENTO.Response"]?.ToString();

                        if (!string.IsNullOrEmpty(response))
                            throw new ServicoException($"A API Cebrace retornou a seguinte falha: {response}.");
                    }
                    else
                        throw new ServicoException($"Problema ao obter a resposta da API Cebrace, código HTTP retornado: {(int)respostaHttp.httpStatusCode}.");
                }

                cteCanhotoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Cebrace.";
                cteCanhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "LogIntegracaoCebrace");
                cteCanhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cteCanhotoIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(cteCanhotoIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repCTeCanhotoIntegracao.Atualizar(cteCanhotoIntegracao);
        }

        private async Task<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Canhoto>> ObterListaCanhotoIntegracao(Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao cteCanhotoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new(_unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoDadosParaCanhotoIntegracao> dadosParaCanhotoIntegracaos = await repCargaPedido.BuscarDadosParaCanhotoIntegracao(
                cteCanhotoIntegracao.CTe.Codigo,
                cteCanhotoIntegracao.CTe.XMLNotaFiscais.Select(x => x.Codigo).ToList(),
                default
            );

            return dadosParaCanhotoIntegracaos.Select(d => new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Canhoto
            {
                DataFim = d.DataDeEntregaMaisAntiga?.ToString("yyyy-MM-dd") ?? string.Empty,
                HoraFim = d.DataDeEntregaMaisAntiga?.ToString("HH:mm:ss") ?? string.Empty,
                ProtocoloNFe = d.NumeroNotaFiscal.ToString() ?? string.Empty,
                Remessa = d.NumeroPedidoEmbarcador ?? string.Empty,
                ProtocoloRemessa = d.CodigoPedido.ToString() ?? string.Empty,
                Transporte = d.NumeroCargaEmbarcador?? string.Empty,
                ProtocoloTransporte = d.CodigoCarga.ToString() ?? string.Empty,
            }).ToList();
        }

        private List<string> ValidarCanhotoIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Canhoto canhoto)
        {
            List<string> erros = new List<string>();

            if (string.IsNullOrEmpty(canhoto.ProtocoloNFe))
                erros.Add($"Não foi encontrado o número do Canhoto");

            if (string.IsNullOrEmpty(canhoto.ProtocoloRemessa))
                erros.Add($"Não foi encontrado o Protocolo Remessa do Canhoto {canhoto.ProtocoloNFe}");

            if (string.IsNullOrEmpty(canhoto.Remessa))
                erros.Add($"Não foi encontrada a Remessa do Canhoto {canhoto.ProtocoloNFe}");

            if (string.IsNullOrEmpty(canhoto.ProtocoloTransporte))
                erros.Add($"Não foi encontrado o Protocolo Transporte do Canhoto {canhoto.ProtocoloNFe}");

            if (string.IsNullOrEmpty(canhoto.Transporte))
                erros.Add($"Não foi encontrado o Transporte do Canhoto {canhoto.ProtocoloNFe}");

            if (string.IsNullOrEmpty(canhoto.DataFim))
                erros.Add($"Não foi encontrada a Data Final do Canhoto {canhoto.ProtocoloNFe}");

            if (string.IsNullOrEmpty(canhoto.HoraFim))
                erros.Add($"Não foi encontrada a Hora Final do Canhoto {canhoto.ProtocoloNFe}");

            return erros;
        }

        #endregion Canhotos

        #region Cargas

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaIntegracaoPendente.NumeroTentativas++;
            cargaIntegracaoPendente.DataIntegracao = DateTime.Now;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            List<string> erros = new List<string>();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga informacaoEnvio = ObterCargaIntegracao(cargaIntegracaoPendente);

                erros = ValidarCargaIntegracao(informacaoEnvio);
                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/tms/AtualizarFrete");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    JObject json = JObject.Parse(respostaHttp.conteudoResposta);
                    string retorno = json["rfc:ZTMS_FRETE.Response"]?.ToString();

                    if (!string.IsNullOrEmpty(retorno))
                        throw new ServicoException($"A API Cebrace retornou a seguinte falha: {retorno}.");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Cebrace, código HTTP retornado: {(int)respostaHttp.httpStatusCode}.");

                cargaIntegracaoPendente.ProblemaIntegracao = "Sucesso ao comunicar com API Cebrace.";
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "LogIntegracaoCebrace");
                cargaIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendente.ProblemaIntegracao = ex.Message;
            }
            if (respostaHttp.conteudoRequisicao != null)
                servicoArquivoTransacao.Adicionar(cargaIntegracaoPendente, respostaHttp.conteudoRequisicao.ToString(), respostaHttp.conteudoResposta.ToString(), "json");

            repCargaCargaIntegracao.Atualizar(cargaIntegracaoPendente);
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga ObterCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoPendente)
        {

            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(cargaIntegracaoPendente.Carga.Codigo);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga
            {
                ProtocoloIntegracaoCarga = cargaIntegracaoPendente.Carga.Codigo.ToString() ?? string.Empty,
                ProtocoloIntegracaoPedido = cargaIntegracaoPendente.Carga.Pedidos.Any() ? cargaIntegracaoPendente.Carga.Pedidos.Select(x => x.Pedido.Codigo).ToList() : new List<int>(),
                FreteTotal = cargaIntegracaoPendente.Carga.ValorFrete,
                TipoOperacao = cargaIntegracaoPendente.Carga.TipoOperacao.Descricao ?? string.Empty,
                CodigoIntegracao = cargaIntegracaoPendente.Carga.TipoOperacao.CodigoIntegracao ?? string.Empty,
                PedagioValor = cargaValePedagios.Any() ? cargaValePedagios.Sum(x => x.Valor) : 0,
                PedagioID = cargaValePedagios.Any() ? string.Join(", ", cargaValePedagios.Select(x => x.NumeroComprovante)) : string.Empty,
                IVA = string.Empty
            };
        }

        private List<string> ValidarCargaIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga carga)
        {
            List<string> erros = new List<string>();

            if (string.IsNullOrEmpty(carga.ProtocoloIntegracaoCarga))
                erros.Add($"Não foi encontrado o Protocolo da Carga");

            if (carga.ProtocoloIntegracaoPedido.Count() == 0)
                erros.Add($"Não foi encontrado o Protocolo do Pedido");

            if (carga.FreteTotal == 0)
                erros.Add($"Não foi encontrado o Valor Total do Frete");

            if (string.IsNullOrEmpty(carga.TipoOperacao))
                erros.Add($"Não foi encontrado o Tipo Operação da Carga");

            if (string.IsNullOrEmpty(carga.CodigoIntegracao))
                erros.Add($"Não foi encontrado o Código de Integração do Tipo de Operação da Carga");

            return erros;
        }
        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            List<string> erros = new List<string>();
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.CancelamentoCarga informacaoEnvio = ObterCancelamentoCargaIntegracao(integracaoPendente);

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/tms/CancelarCarga");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    JObject json = JObject.Parse(respostaHttp.conteudoResposta);
                    string retorno = json["rfc:ZTMS_CANCELAR_CARGA.Response"]?.ToString();

                    if (!string.IsNullOrEmpty(retorno))
                        throw new ServicoException($"A API Cebrace retornou a seguinte falha: {retorno}.");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Cebrace, código HTTP retornado: {(int)respostaHttp.httpStatusCode}.");

                integracaoPendente.ProblemaIntegracao = "Sucesso ao comunicar com API Cebrace.";
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "LogIntegracaoCebrace");
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message;
            }
            if (respostaHttp.conteudoRequisicao != null)
                servicoArquivoTransacao.Adicionar(integracaoPendente, respostaHttp.conteudoRequisicao.ToString(), respostaHttp.conteudoResposta.ToString(), "json");

            repCargaCancelamentoCargaIntegracao.Atualizar(integracaoPendente);
        }

        private CancelamentoCarga ObterCancelamentoCargaIntegracao(CargaCancelamentoCargaIntegracao integracaoPendente)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.CancelamentoCarga
            {
                ProtocoloIntegracaoCarga = integracaoPendente.CargaCancelamento.Carga.Codigo.ToString(),
                CancelamentoRealizado = true,
                DuplicarCarga = integracaoPendente.CargaCancelamento.DuplicarCarga,
            };
        }


        #endregion Cargas

        #region Dados Carga

        public void IntegrarDadosCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaIntegracao.NumeroTentativas++;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.DadosCarga dadosCarga = ObterDadosCargaIntegracao(cargaIntegracao.Carga);

                HttpClient client = ObterHttpClient();
                string token = ObterToken();

                client.DefaultRequestHeaders.Add("token", token);

                jsonRequisicao = JsonConvert.SerializeObject(dadosCarga, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                StringContent content = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(string.Concat(_configuracaoIntegracaoCebrace.URLIntegracao.TrimEnd('/'), "/v1/tms/AtualizarCarga"), content).Result;

                HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequisicao, result);
                jsonRetorno = httpRequisicaoResposta.conteudoResposta;

                if (httpRequisicaoResposta.httpStatusCode == HttpStatusCode.OK)
                {
                    JObject json = JObject.Parse(httpRequisicaoResposta.conteudoResposta);
                    string response = json["rfc:ZTMS_ATUALIZAR_CARGA.Response"]?.ToString();

                    if (!string.IsNullOrEmpty(response))
                        throw new ServicoException($"A API dados carga Cebrace retornou a seguinte falha: {response}.");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API dados carga Cebrace, código HTTP retornado: {(int)httpRequisicaoResposta.httpStatusCode}.");

                cargaIntegracao.ProblemaIntegracao = "Integrado com Sucesso.";
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "LogIntegracaoCebrace");

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            repCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.DadosCarga ObterDadosCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault();
            string remetente = cargasPedido.FirstOrDefault()?.ObterTomador()?.CodigoIntegracao ?? string.Empty;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.DadosCarga
            {
                ProtocoloIntegracaoCarga = carga.Protocolo,
                NumeroCarga = carga.CodigoCargaEmbarcador,
                DataAgendamento = carga.DataCarregamentoCarga?.ToString("yyyy-MM-dd") ?? string.Empty,
                HoraAgendamento = carga.DataCarregamentoCarga?.ToString("HH:mm:ss") ?? string.Empty,
                PlacaTracao = carga.Veiculo?.Placa ?? string.Empty,
                PlacaCarreta = carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                Pedidos = ObterPedidosCargaIntegracao(cargasPedido),
                ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.ModeloVeicular
                {
                    CodigoIntegracao = carga.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty
                },
                Motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Motorista
                {
                    Nome = motorista?.Nome ?? string.Empty,
                    Documento = motorista?.RG ?? string.Empty,
                },
                Transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Transportador
                {
                    CodigoIntegracao = carga.Empresa?.CodigoIntegracao ?? string.Empty,
                    Remetente = remetente
                },
                Filial = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Filial
                {
                    CodigoIntegracao = carga.Filial?.CodigoFilialEmbarcador ?? string.Empty
                },
                TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.TipoOperacao
                {
                    CodigoIntegracao = carga.TipoOperacao?.CodigoIntegracao ?? string.Empty
                }
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Pedido> ObterPedidosCargaIntegracao(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Pedido>();

            if (cargasPedido.Count == 0)
                return pedidos;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
            {
                pedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.DadosCarga.Pedido
                {
                    Protocolo = cargaPedido.Pedido.Protocolo.ToString(),
                    Destinatario = cargaPedido.Recebedor != null ? cargaPedido.Recebedor.Descricao : cargaPedido.Pedido.Destinatario?.Descricao,
                });
            }

            return pedidos;
        }

        public void IntegrarCargaFrete(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaFreteIntegracaoPendente.NumeroTentativas++;
            cargaFreteIntegracaoPendente.DataIntegracao = DateTime.Now;

            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(_unitOfWork);
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            List<string> erros = new List<string>();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga informacaoEnvio = ObterCargaFreteIntegracao(cargaFreteIntegracaoPendente);

                erros = ValidarCargaIntegracao(informacaoEnvio);
                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/tms/AtualizarFrete");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    JObject json = JObject.Parse(respostaHttp.conteudoResposta);
                    string retorno = json["rfc:ZTMS_FRETE.Response"]?.ToString();

                    if (!string.IsNullOrEmpty(retorno))
                        throw new ServicoException($"A API Cebrace retornou a seguinte falha: {retorno}.");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Cebrace, código HTTP retornado: {(int)respostaHttp.httpStatusCode}.");

                cargaFreteIntegracaoPendente.ProblemaIntegracao = "Sucesso ao comunicar com API Cebrace.";
                cargaFreteIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "LogIntegracaoCebrace");
                new Servicos.Embarcador.Carga.CargaFreteIntegracao().AplicarFalha(cargaFreteIntegracaoPendente, ex.Message);
            }
            if (respostaHttp.conteudoRequisicao != null)
                servicoArquivoTransacao.Adicionar(cargaFreteIntegracaoPendente, respostaHttp.conteudoRequisicao.ToString(), respostaHttp.conteudoResposta.ToString(), "json");

            repCargaFreteIntegracao.Atualizar(cargaFreteIntegracaoPendente);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga ObterCargaFreteIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracaoPendente)
        {

            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(cargaFreteIntegracaoPendente.Carga.Codigo);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.Carga
            {
                ProtocoloIntegracaoCarga = cargaFreteIntegracaoPendente.Carga.Codigo.ToString() ?? string.Empty,
                ProtocoloIntegracaoPedido = cargaFreteIntegracaoPendente.Carga.Pedidos.Any() ? cargaFreteIntegracaoPendente.Carga.Pedidos.Select(x => x.Pedido.Codigo).ToList() : new List<int>(),
                FreteTotal = cargaFreteIntegracaoPendente.Carga.ValorFrete,
                TipoOperacao = cargaFreteIntegracaoPendente.Carga.TipoOperacao.Descricao ?? string.Empty,
                CodigoIntegracao = cargaFreteIntegracaoPendente.Carga.TipoOperacao.CodigoIntegracao ?? string.Empty,
                PedagioValor = cargaValePedagios.Any() ? cargaValePedagios.Sum(x => x.Valor) : 0,
                PedagioID = cargaValePedagios.Any() ? string.Join(", ", cargaValePedagios.Select(x => x.NumeroComprovante)) : string.Empty,
                IVA = string.Empty
            };
        }

        #endregion Dados Carga


        #region Métodos Públicos 

        public bool ExisteIntegracao()
        {
            try
            {
                ObterConfiguracaoIntegracaoCebrace();
                return true;
            }
            catch (ServicoException)
            {
                return false;
            }
        }
        #endregion Métodos Públicos

        #region Métodos Privados de Requisição


        private void ObterConfiguracaoIntegracaoCebrace()
        {
            if (_configuracaoIntegracaoCebrace != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCebrace repositorioConfiguracaoIntegracaoCebrace = new Repositorio.Embarcador.Configuracoes.IntegracaoCebrace(_unitOfWork);
            _configuracaoIntegracaoCebrace = repositorioConfiguracaoIntegracaoCebrace.BuscarPrimeiroRegistro();

            if ((_configuracaoIntegracaoCebrace == null) || !_configuracaoIntegracaoCebrace.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Cebrace");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoCebrace.URLAutenticacao) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoCebrace.URLIntegracao) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoCebrace.ApiKey))
                throw new ServicoException("O URL Autenticação, URL Integração e APIKey devem estar preenchidos na configuração de integração da Cebrace");
        }

        private HttpClient ObterHttpClient()
        {
            ObterConfiguracaoIntegracaoCebrace();
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCebrace));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", _configuracaoIntegracaoCebrace.ApiKey);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            return client;
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicaoCebrace, string endpointAcao)
        {
            HttpClient client = ObterHttpClient();
            string token = ObterToken();

            client.DefaultRequestHeaders.Add("token", token);

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicaoCebrace, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(string.Concat(_configuracaoIntegracaoCebrace.URLIntegracao.TrimEnd('/'), endpointAcao), content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        private string ObterToken()
        {
            HttpClient client = ObterHttpClient();
            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage result = client.PostAsync(_configuracaoIntegracaoCebrace.URLAutenticacao, content).Result;
            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.RetornoToken responseString = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Cebrace.RetornoToken>(jsonResponse);

            return responseString.Token;
        }

        #endregion Métodos Privados de Requisição
    }
}
