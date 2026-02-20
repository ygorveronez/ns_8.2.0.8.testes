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

namespace Servicos.Embarcador.Integracao.Tecnorisk
{
    public class IntegracaoTecnorisk
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoTecnorisk(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarSolicitacaoMonitoramento(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            cargaIntegracao.NumeroTentativas++;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoContratoTransporteFrete();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestTecnorisk dadosRequisicao = PreencherRequisicao(cargaIntegracao, configuracaoIntegracao);

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.Url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RetornoRequisicao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RetornoRequisicao>(jsonRetorno);
                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    if (retorno.erro == 0)
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.Protocolo = retorno.resultado.monitoramento_id;
                        cargaIntegracao.ProblemaIntegracao = retorno.mensagem;
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = retorno.mensagem;
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Houve um problema ao comunicar com Tecnorisk";
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void CancelarSolicitacaoMonitoramento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            cargaCancelamentoIntegracao.NumeroTentativas++;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoContratoTransporteFrete();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestCancelamento dadosRequisicao = PreencherRequestCancelamento(cargaCancelamentoIntegracao);

                HttpClient requisicao = CriarRequisicaoCancelamento(configuracaoIntegracao, cargaCancelamentoIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(requisicao.BaseAddress, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RetornoRequisicao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RetornoRequisicao>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    if (retorno.erro == 0)
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaCancelamentoIntegracao.ProblemaIntegracao = retorno.mensagem;
                    }
                    else
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaCancelamentoIntegracao.ProblemaIntegracao = retorno.mensagem;
                    }
                }
                else
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Houve um problema ao comunicar com Tecnorisk";
                }

                servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cargaCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;
                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;

                servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repCargaCancelamentoIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoContratoTransporteFrete()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk configuracaoTecnorisk = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao()
            {
                ClientId = configuracaoTecnorisk.UsuarioTecnorisk,
                ClientSecret = configuracaoTecnorisk.SenhaTecnorisk,
                Url = configuracaoTecnorisk.URLIntegracaoTecnorisk,
                PGRId = configuracaoTecnorisk.IDPGR,
                IDPropriedadeMonitoramento = configuracaoTecnorisk.IDPropriedadeMonitoramento,
                CargaMercadoria = configuracaoTecnorisk.CargaMercadoria
            };
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk repIntegracaoTecnorisk = new Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk configuracaoIntegracaoTecnorisk = repIntegracaoTecnorisk.Buscar();

            if ((configuracaoIntegracaoTecnorisk == null) || !configuracaoIntegracaoTecnorisk.PossuiIntegracaoTecnorisk)
                throw new ServicoException("Não existe configuração de integração disponível para a Tecnorisk");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoTecnorisk.UsuarioTecnorisk) || string.IsNullOrWhiteSpace(configuracaoIntegracaoTecnorisk.SenhaTecnorisk))
                throw new ServicoException("O usuário e a senha devem estar preenchidos na configuração de integração da Tecnorisk");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoTecnorisk.URLIntegracaoTecnorisk))
                throw new ServicoException("A URL não está configurada para a integração com a LBC");

            return configuracaoIntegracaoTecnorisk;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestTecnorisk PreencherRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestTecnorisk request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestTecnorisk();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao.Carga;

            Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = cargaIntegracao.Carga.CargaMDFes?.FirstOrDefault() ?? new Dominio.Entidades.Embarcador.Cargas.CargaMDFe();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido = cargaIntegracao.Carga.Pedidos.FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.InformacoesViagem informacoesViagem = ObterInformacoesViagem(cargaMDFe?.MDFe?.DataEmissao, pedido?.Origem, pedido?.Destino);

            DateTime horaEmissaoMaisUm = DateTime.MinValue;

            if (cargaMDFe.MDFe != null)
                horaEmissaoMaisUm = cargaMDFe.MDFe.DataEmissao.HasValue ? (cargaMDFe.MDFe.DataEmissao.Value != DateTime.MinValue ? cargaMDFe.MDFe.DataEmissao.Value.AddHours(1).AddMinutes(1) : DateTime.MinValue) : DateTime.MinValue;

            request.DataHoraAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            request.PGRId = configuracaoIntegracao.PGRId;
            request.UnidadeTransporteId = (carga.Veiculo?.NumeroFrota).ToInt();
            request.Veiculos = ObterVeiculos(carga);
            request.CPF = carga?.CPFPrimeiroMotorista ?? string.Empty;
            request.Telefone = carga.ListaMotorista?.FirstOrDefault()?.Telefone ?? string.Empty;
            request.MonitoramentoPrioridade = configuracaoIntegracao.IDPropriedadeMonitoramento;
            request.CargaMercadoria = configuracaoIntegracao.CargaMercadoria;
            request.Cargas = ObterCargas(carga);
            request.DataSaidaVeiculo = horaEmissaoMaisUm != DateTime.MinValue ? horaEmissaoMaisUm.ToString("yyyy-MM-dd") : string.Empty;
            request.HoraSaidaVeiculo = horaEmissaoMaisUm != DateTime.MinValue ? horaEmissaoMaisUm.ToString("HH:mm") : string.Empty;
            request.EstadoOrigemViagem = pedido?.Origem?.Estado?.Nome.Trim() ?? string.Empty;
            request.CidadeOrigemViagem = pedido?.Origem?.Descricao.Trim() ?? string.Empty;
            request.EstadoDestinoViagem = pedido.Destino?.Estado?.Nome.Trim() ?? string.Empty;
            request.CidadeDestinoViagem = pedido.Destino?.Descricao.Trim() ?? string.Empty;
            request.DataChegadaDestino = informacoesViagem.DataPrevisaoChegada;
            request.HoraChegadaDestino = informacoesViagem.HoraPrevisaoChegada;
            request.IDRotaViagem = informacoesViagem.CodigoRotaIntegracao;

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestCancelamento PreencherRequestCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestCancelamento requestCancalamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.RequestCancelamento();
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, TipoIntegracao.Tecnorisk);

            requestCancalamento.ProtocoloCancelamento = cargaCargaIntegracao?.Protocolo ?? string.Empty;
            requestCancalamento.MotivoCancelamento = "Solicitação de cancelamento de monitoramento";

            return requestCancalamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Veiculos> ObterVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Veiculos> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Veiculos>();

            if (carga.VeiculosVinculados.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
                {
                    veiculos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Veiculos
                    {
                        Placa = veiculo?.Placa ?? string.Empty,
                        VeiculoTipo = veiculo?.TipoVeiculo == "0" ? "V" : "R1" ?? string.Empty
                    });
                }
            }
            else if (carga.Veiculo != null)
            {
                veiculos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Veiculos
                {
                    Placa = carga.Veiculo.Placa ?? string.Empty,
                    VeiculoTipo = carga.Veiculo.TipoVeiculo == "0" ? "V" : "R1" ?? string.Empty
                });
            }

            return veiculos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Cargas> ObterCargas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Cargas> cargas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Cargas>();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notaPedido = carga.Pedidos.SelectMany(pedido => pedido.NotasFiscais).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas = notaPedido.Select(np => np.XMLNotaFiscal).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in notas)
            {
                cargas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.Cargas
                {
                    Valor = nota.Valor,
                });
            }

            return cargas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.InformacoesViagem ObterInformacoesViagem(DateTime? dataEmissaoMDFE, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorOrigemDestino(origem.Codigo, destino.Codigo);

            DateTime dataEmissao = new DateTime();
            DateTime dataPrevisaoChegada = new DateTime();

            if (dataEmissaoMDFE.HasValue && dataEmissaoMDFE.Value != DateTime.MinValue)
                dataEmissao = dataEmissaoMDFE.Value;

            if (rota != null)
                dataPrevisaoChegada = dataEmissao.AddMinutes(rota.TempoViagemEmMinutos);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.InformacoesViagem
            {
                DataPrevisaoChegada = dataPrevisaoChegada.ToString("yyyy-MM-dd"),
                HoraPrevisaoChegada = dataPrevisaoChegada.ToString("HH:mm"),
                CodigoRotaIntegracao = rota?.DescricaoRotaSemParar ?? ""
            };
        }

        private HttpClient CriarRequisicao(Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTecnorisk));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.Url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracao.ClientId, configuracaoIntegracao.ClientSecret);

            return requisicao;
        }

        private HttpClient CriarRequisicaoCancelamento(Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk.ConfiguracaoIntegracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoTecnorisk));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.Url + "/cancelar");
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracao.ClientId, configuracaoIntegracao.ClientSecret);

            return requisicao;
        }

        #endregion
    }
}
