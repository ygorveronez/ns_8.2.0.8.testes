using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Marfrig
{
    public class IntegracaoExportacaoMarfrig
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoExportacaoMarfrig(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url, string apiKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoExportacaoMarfrig));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("apikey", apiKey);

            return requisicao;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMarfrig repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMarfrig(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null)
                throw new ServicoException("Não existe configuração de integração disponível para a Marfrig.");

            return configuracaoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoIntegracao ObterTipoIntegracaoMarfrig()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMarfrig = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig);

            return tipoIntegracaoMarfrig;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Integrar Recebimento

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.Recebimento ObterRecebimento(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.Recebimento recebimento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.Recebimento()
            {
                Cabecalho = ObterRecebimentoCabecalho(cargaPedidoReferencia, dadosTransporteMaritimo),
                Container = ObterRecebimentoContainer(cargaPedidoReferencia, dadosTransporteMaritimo),
                Reserva = ObterRecebimentoReserva(cargaPedidoReferencia, dadosTransporteMaritimo),
                Rota = ObterRecebimentoRota(dadosTransporteMaritimo),
                Transporte = ObterRecebimentoTransporte(cargaPedidoReferencia, dadosTransporteMaritimo)
            };

            return recebimento;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoCabecalho ObterRecebimentoCabecalho(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoCabecalho cabecalho = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoCabecalho()
            {
                NumeroEXP = cargaPedidoReferencia.Pedido.NumeroEXP ?? "",
                ProtocoloReferencia = cargaPedidoReferencia.Carga.Protocolo.ToString()
            };

            return cabecalho;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoContainer ObterRecebimentoContainer(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoContainer container = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoContainer()
            {
                DataDepositoContainer = dadosTransporteMaritimo.DataDepositoContainer,
                DataRetiraVazio = dadosTransporteMaritimo.DataRetiradaVazio,
                DataRetornoVazio = dadosTransporteMaritimo.DataRetornoVazio,
                NumeroContainer = dadosTransporteMaritimo.Container?.Numero ?? "",
                NumeroLacre = dadosTransporteMaritimo.NumeroLacre ?? "",
                TerminalContainer = dadosTransporteMaritimo.LocalTerminalContainer?.Descricao ?? ""
            };

            return container;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoReserva ObterRecebimentoReserva(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoReserva reserva = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoReserva()
            {
                CodigoArmador = dadosTransporteMaritimo.CodigoArmador ?? "",
                DataBooking = dadosTransporteMaritimo.DataBooking,
                Despachante = cargaPedidoReferencia.Pedido.DescricaoDespachante.Left(35) ?? "",
                NomeNavio = dadosTransporteMaritimo.NomeNavio ?? "",
                NumeroBL = dadosTransporteMaritimo.NumeroBL ?? "",
                NumeroBooking = cargaPedidoReferencia.Pedido.NumeroBooking ?? "",
                NumeroViagem = dadosTransporteMaritimo.NumeroViagem ?? "",
                PortoCarregamento = dadosTransporteMaritimo.CodigoPortoCarregamento ?? ""
            };

            return reserva;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoRota ObterRecebimentoRota(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento repositorioPedidoDadosTransporteMaritimoRoteamento = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento dadosTransporteMaritimoRoteamento = repositorioPedidoDadosTransporteMaritimoRoteamento.BuscarUltimoPorDadosTransporteMaritimo(dadosTransporteMaritimo.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoRota rota;

            if (dadosTransporteMaritimoRoteamento != null)
            {
                rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoRota()
                {
                    CodigoRota = dadosTransporteMaritimoRoteamento.CodigoRoteamento ?? "",
                    ETATransbordo = dadosTransporteMaritimoRoteamento.PortoCargaData,
                    ETSTransbordo = dadosTransporteMaritimoRoteamento.PortoDescargaData,
                    NomeNavioTransbordo = dadosTransporteMaritimoRoteamento.NomeNavio ?? "",
                    NumeroViagemTransbordo = dadosTransporteMaritimoRoteamento.NumeroViagem ?? "",
                    PortoCarregamentoTransbordo = dadosTransporteMaritimoRoteamento.PortoCargaLocalizacao ?? "",
                    PortoDestinoTransbordo = dadosTransporteMaritimoRoteamento.PortoDescargaLocalizacao ?? ""
                };
            }
            else
            {
                rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoRota()
                {
                    CodigoRota = dadosTransporteMaritimo.CodigoRota ?? "",
                    ETATransbordo = dadosTransporteMaritimo.DataETATransbordo,
                    ETSTransbordo = dadosTransporteMaritimo.DataETSTransbordo,
                    NomeNavioTransbordo = dadosTransporteMaritimo.NavioTransbordo?.Descricao ?? "",
                    NumeroViagemTransbordo = dadosTransporteMaritimo.NumeroViagemTransbordo ?? "",
                    PortoCarregamentoTransbordo = dadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo ?? "",
                    PortoDestinoTransbordo = dadosTransporteMaritimo.CodigoPortoDestinoTransbordo ?? ""
                };
            }

            return rota;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoTransporte ObterRecebimentoTransporte(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia, Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoTransporte transporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RecebimentoTransporte()
            {
                DataEstufagem = cargaPedidoReferencia.Carga.DataCarregamentoCarga,
                DeadLineCarga = dadosTransporteMaritimo.DataDeadLineCarga,
                DeadLineDraft = dadosTransporteMaritimo.DataDeadLineDraf,
                ETADestino = dadosTransporteMaritimo.DataETADestino,
                ETADestinoFinal = dadosTransporteMaritimo.DataETADestinoFinal,
                ETAOrigem = dadosTransporteMaritimo.DataETAOrigem,
                ETAOrigemFinal = dadosTransporteMaritimo.DataETAOrigemFinal,
                ETASegundaOrigem = dadosTransporteMaritimo.DataETASegundaOrigem,
                ETASegundoDestino = dadosTransporteMaritimo.DataETASegundoDestino,
                ETS = dadosTransporteMaritimo.DataETS,
                RetiraContainerDestino = dadosTransporteMaritimo.DataRetiradaContainerDestino,
                TerminalOrigem = dadosTransporteMaritimo.LocalTerminalOrigem?.Descricao ?? ""
            };

            return transporte;
        }

        #endregion Métodos Privados - Integrar Recebimento

        #region Métodos Publicos

        public void AdicionarCargaIntegracaoExportacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMarfrig = ObterTipoIntegracaoMarfrig();

            if (tipoIntegracaoMarfrig == null)
                return;

            Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorioCargaExportacaoIntegracao = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao cargaExportacaoIntegracao = new Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao()
            {
                Carga = carga,
                DataCriacao = DateTime.Now,
                DataIntegracao = DateTime.Now,
                Numero = repositorioCargaExportacaoIntegracao.BuscarProximoNumero(),
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracaoMarfrig
            };

            repositorioCargaExportacaoIntegracao.Inserir(cargaExportacaoIntegracao);
        }

        public void IntegrarCargaExportacao(Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao cargaExportacaoIntegracao)
        {
            Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorioCargaExportacaoIntegracao = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaExportacaoIntegracao.NumeroTentativas += 1;
            cargaExportacaoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferencia = repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaExportacaoIntegracao.Carga.Codigo);

                if (cargaPedidoReferencia == null)
                    throw new ServicoException("Não foi possível encontrar a carga.");

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = repositorioPedidoDadosTransporteMaritimo.BuscarPorPedido(cargaPedidoReferencia.Pedido.Codigo);

                if (dadosTransporteMaritimo == null)
                    throw new ServicoException("Não foi possível encontrar os dados de transporte marítimo da carga.");

                string url = configuracaoIntegracao.URLIntegracaoCargaOrdemEmbarqueExportacao + (cargaPedidoReferencia.Pedido.NumeroEXP?.Replace("/", "") ?? "");
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.Recebimento recebimento = ObterRecebimento(cargaPedidoReferencia, dadosTransporteMaritimo);
                jsonRequisicao = JsonConvert.SerializeObject(recebimento, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.SendAsync(new HttpRequestMessage(HttpMethod.Put, url) { Content = conteudoRequisicao }).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RetornoIntegracaoRecebimento retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento.RetornoIntegracaoRecebimento>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    cargaExportacaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaExportacaoIntegracao.ProblemaIntegracao = "Integração do recebimento realizada com sucesso.";
                }
                else
                {
                    cargaExportacaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaExportacaoIntegracao.ProblemaIntegracao = $"Ocorreu uma falha ao realizar a integração com a Marfrig: {retorno?.ObterMensagemRetorno() ?? ""}.";
                }

                servicoArquivoTransacao.Adicionar(cargaExportacaoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                cargaExportacaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaExportacaoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaExportacaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaExportacaoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";

                servicoArquivoTransacao.Adicionar(cargaExportacaoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaExportacaoIntegracao.Atualizar(cargaExportacaoIntegracao);
        }

        public void VerificarIntegracoesExportacaoPendentes()
        {
            Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao repositorioCargaExportacaoIntegracao = new Repositorio.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao> integracoesCargaExportacaoPendentes = repositorioCargaExportacaoIntegracao.BuscarIntegracoesPendentes(numeroTentativas: 3, tempoProximaTentativaEmMinutos: 2, limiteRegistros: 5);

            foreach (Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao cargaExportacaoIntegracao in integracoesCargaExportacaoPendentes)
                IntegrarCargaExportacao(cargaExportacaoIntegracao);
        }

        #endregion Métodos Publicos
    }
}
