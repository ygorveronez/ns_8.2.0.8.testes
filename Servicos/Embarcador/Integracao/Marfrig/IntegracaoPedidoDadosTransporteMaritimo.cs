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
    public class IntegracaoPedidoDadosTransporteMaritimo
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoPedidoDadosTransporteMaritimo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url, string apiKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPedidoDadosTransporteMaritimo));

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

        #region Métodos Privados - Integrar DadosTransporteMaritimoRecebimento

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRecebimento ObterRecebimento(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRecebimento recebimento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRecebimento()
            {
                Cabecalho = ObterRecebimentoCabecalho(dadosTransporteMaritimo),
                Container = ObterRecebimentoContainer(dadosTransporteMaritimo),
                Reserva = ObterRecebimentoReserva(dadosTransporteMaritimo),
                Rota = ObterRecebimentoRota(dadosTransporteMaritimo),
                Transporte = ObterRecebimentoTransporte(dadosTransporteMaritimo),
                PCP = ObterRecebimentoPCP(dadosTransporteMaritimo)
            };

            return recebimento;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoCabecalho ObterRecebimentoCabecalho(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoCabecalho cabecalho = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoCabecalho()
            {
                NumeroEXP = dadosTransporteMaritimo.NumeroEXP ?? "",
                CodigoNcm = dadosTransporteMaritimo.CodigoNCM ?? "",
                ContratoFob = dadosTransporteMaritimo.CodigoContratoFOB ?? "",
                DescricaoCarga = dadosTransporteMaritimo.CodigoCargaEmbarcador ?? "",
                DescricaoIdentificacaoCarga = dadosTransporteMaritimo.CodigoIdentificacaoCarga ?? "",
                EspeciePedido = dadosTransporteMaritimo.DescricaoEspecie ?? "",
                Importador = dadosTransporteMaritimo.Importador?.Descricao ?? "",
                Incoterm = dadosTransporteMaritimo.Incoterm ?? "",
                NumeroPedido = dadosTransporteMaritimo.Pedido.NumeroPedidoEmbarcador,
                TipoProduto = dadosTransporteMaritimo.TipoDeCarga?.Descricao ?? "",
                Filial = dadosTransporteMaritimo.Filial?.Descricao ?? "",
                ProtocoloReferencia = dadosTransporteMaritimo.Codigo.ToString()
            };

            return cabecalho;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoContainer ObterRecebimentoContainer(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoContainer container = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoContainer()
            {
                DataDepositoContainer = dadosTransporteMaritimo.DataDepositoContainer,
                DataRetiraVazio = dadosTransporteMaritimo.DataRetiradaVazio,
                DataRetornoVazio = dadosTransporteMaritimo.DataRetornoVazio,
                NumeroContainer = dadosTransporteMaritimo.Container?.Numero ?? "",
                NumeroLacre = dadosTransporteMaritimo.NumeroLacre ?? "",
                TerminalContainer = dadosTransporteMaritimo.LocalTerminalContainer?.Descricao ?? "",
            };

            return container;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoReserva ObterRecebimentoReserva(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoReserva reserva = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoReserva()
            {
                CodigoArmador = dadosTransporteMaritimo.Armador?.CodigoIntegracao ?? "",
                DataBooking = dadosTransporteMaritimo.DataBooking,
                Despachante = dadosTransporteMaritimo.Despachante?.Descricao?.Left(35) ?? "",
                NomeNavio = dadosTransporteMaritimo.Navio?.Descricao ?? "",
                NumeroBL = dadosTransporteMaritimo.NumeroBL ?? "",
                NumeroBooking = dadosTransporteMaritimo.NumeroBooking ?? "",
                NumeroViagem = dadosTransporteMaritimo.NumeroViagem ?? "",
                PortoCarregamento = dadosTransporteMaritimo.PortoOrigem?.CodigoIntegracao ?? ""
            };

            return reserva;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRota ObterRecebimentoRota(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRota rota;

            rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRota()
            {
                CodigoRota = dadosTransporteMaritimo.CodigoRota ?? "",
                ETATransbordo = dadosTransporteMaritimo.DataETATransbordo,
                ETSTransbordo = dadosTransporteMaritimo.DataETSTransbordo,
                NomeNavioTransbordo = dadosTransporteMaritimo.NavioTransbordo?.Descricao ?? "",
                NumeroViagemTransbordo = dadosTransporteMaritimo.NumeroViagemTransbordo ?? "",
                PortoCarregamentoTransbordo = dadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo ?? "",
                PortoDestinoTransbordo = dadosTransporteMaritimo.PortoDestino?.Descricao ?? ""
            };

            return rota;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoTransporte ObterRecebimentoTransporte(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoTransporte transporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoTransporte()
            {
                DataEstufagem = dadosTransporteMaritimo.DataPrevisaoEstufagem,
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
                nomeTerminalOrigem = dadosTransporteMaritimo.LocalTerminalOrigem?.Descricao ?? "",
                CargaPalletizada = dadosTransporteMaritimo.CargaPaletizada ? "S" : "N",
                CodigoterminalOrigem = dadosTransporteMaritimo.LocalTerminalOrigem?.Codigo.ToString() ?? "",
                DestinoTransbordo = dadosTransporteMaritimo.PortoDestino?.Descricao,
                Genset = dadosTransporteMaritimo.PossuiGenset ? "S" : "N",
                Inland = dadosTransporteMaritimo.TipoInLand.ObterDescricaoIntegracao(),
                MoedaCapatazia = dadosTransporteMaritimo.MoedaCapatazia?.Simbolo.ToString() ?? "",
                NumeroCarga = !string.IsNullOrEmpty(dadosTransporteMaritimo.CodigoCargaEmbarcador) ? dadosTransporteMaritimo.CodigoCargaEmbarcador : dadosTransporteMaritimo.Pedido?.CodigoCargaEmbarcador ?? "",
                SegundaDeadLineCarga = dadosTransporteMaritimo.SegundaDataDeadLineCarga,
                SegundaDeadLineDraft = dadosTransporteMaritimo.SegundaDataDeadLineDraf,
                Temperatura = dadosTransporteMaritimo.Temperatura,
                TipoContainer = dadosTransporteMaritimo.TipoContainer?.Descricao ?? "",
                TipoFrete = dadosTransporteMaritimo.FretePrepaid.ObterDescricao(),
                TipoTransporte = dadosTransporteMaritimo.TipoDeTransporte.ObterDescricao(),
                UsaProbe = dadosTransporteMaritimo.TipoProbe.HasValue ? dadosTransporteMaritimo.TipoProbe.Value.ObterDescricao() : "",
                ValorCapatazia = (int)dadosTransporteMaritimo.ValorCapatazia,
                ValorFrete = (int)dadosTransporteMaritimo.ValorFrete,
                ViaTransporte = dadosTransporteMaritimo.ViaTransporte?.Descricao ?? ""
            };

            return transporte;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoPCP ObterRecebimentoPCP(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoPCP pcp = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoPCP()
            {
                DataEstufagem = dadosTransporteMaritimo.DataPrevisaoEstufagem,
                Halal = dadosTransporteMaritimo.Halal ? "S" : "N",
                Observacao = dadosTransporteMaritimo.Observacao?.Left(60) ?? "",
                Remetente = dadosTransporteMaritimo.Remetente?.Descricao ?? "",
                statusExportacao = dadosTransporteMaritimo.Status == StatusControleMaritimo.Ativo ? "A" : "C"
            };

            return pcp;
        }

        private void AtualizarDadosPedidoExportacao(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoDadosTransporteMaritimo.Pedido;

            if (pedido != null)
            {
                pedido.CodigoPortoOrigem = pedidoDadosTransporteMaritimo.PortoOrigem.CodigoIntegracao;
                pedido.DescricaoPortoOrigem = pedidoDadosTransporteMaritimo.PortoOrigem.Descricao;
                pedido.ClienteDonoContainer = pedidoDadosTransporteMaritimo.Armador;

                repPedido.Atualizar(pedido);
            }
        }

        #endregion Métodos Privados - Integrar Recebimento

        #region Métodos Publicos

        public void AdicionarPedidoDadosTransporteMaritimoIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMarfrig = ObterTipoIntegracaoMarfrig();

            if (tipoIntegracaoMarfrig == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao pedidoDadosTransporteIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao()
            {
                PedidoDadosTransporteMaritimo = pedidoDadosTransporteMaritimo,
                DataCriacao = DateTime.Now,
                DataIntegracao = DateTime.Now,
                NumeroTentativas = 0,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracaoMarfrig
            };

            repositorioIntegracao.Inserir(pedidoDadosTransporteIntegracao);
        }

        public void IntegrarDadosTransporteMaritimoIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao pedidoDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            pedidoDadosTransporteIntegracao.NumeroTentativas += 1;
            pedidoDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarfrig configuracaoIntegracao = ObterConfiguracaoIntegracao();

                if (pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo == null)
                    throw new ServicoException("Não foi possível encontrar os dados de transporte marítimo para integração.");

                string url = configuracaoIntegracao.URLIntegracaoCargaOrdemEmbarqueExportacao + (pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.NumeroEXP?.Replace("/", "") ?? "");
                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao.ApiKey);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.DadosTransporteMaritimoRecebimento recebimento = ObterRecebimento(pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo);
                jsonRequisicao = JsonConvert.SerializeObject(recebimento, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.SendAsync(new HttpRequestMessage(HttpMethod.Put, url) { Content = conteudoRequisicao }).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.RetornoDadosTransporteMaritimoRecebimento retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo.RetornoDadosTransporteMaritimoRecebimento>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    pedidoDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    pedidoDadosTransporteIntegracao.ProblemaIntegracao = "Integração do recebimento realizada com sucesso.";
                }
                else
                {
                    ////Caso deu falha na requisição, precisamos verificar se existe outro booking para o pedido ja ativo. (esse registro pode ser o registro de atualizacao, ou um novo)
                    //Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo DadosTransporteMaritimoPedidoAtivo = repositorioPedidoDadosTransporteMaritimo.BuscarPorPedido(pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.Pedido?.Codigo ?? 0);
                    //if (DadosTransporteMaritimoPedidoAtivo == null)
                    //{
                    //    //se nao existe, este se torna o ativo para uma possivel alteração posterior.
                    //    pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.BookingTemporario = false;
                    //    pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.CodigoOriginal = 0;
                    //}

                    pedidoDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pedidoDadosTransporteIntegracao.ProblemaIntegracao = $"Ocorreu uma falha ao realizar a integração com a Marfrig: {retorno?.ObterMensagemRetorno() ?? ""}.";
                }

                servicoArquivoTransacao.Adicionar(pedidoDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                pedidoDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pedidoDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pedidoDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marfrig";

                servicoArquivoTransacao.Adicionar(pedidoDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioIntegracao.Atualizar(pedidoDadosTransporteIntegracao);
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao> integracoesPendentes = repositorioIntegracao.BuscarIntegracoesPendentes(numeroTentativas: 3, tempoProximaTentativaEmMinutos: 2, limiteRegistros: 5);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao integracao in integracoesPendentes)
                IntegrarDadosTransporteMaritimoIntegracao(integracao);
        }

        public void TratarEntidadeRetornoIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao pedidoDadosTransporteIntegracao, Dominio.ObjetosDeValor.WebService.Carga.RetornoBooking retornoBooking)
        {
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repPedidodadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(_unitOfWork);

            if (retornoBooking.Status)
            {
                //integracao sucesso
                if (pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.CodigoOriginal > 0 && pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.BookingTemporario)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimoOriginal = repPedidodadosTransporteMaritimo.BuscarPorCodigo(pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.CodigoOriginal, false);
                    // o original se torna cancelado e esse passa a ser o ativo para o pedido.
                    pedidoDadosTransporteMaritimoOriginal.Status = StatusControleMaritimo.Cancelado;
                    pedidoDadosTransporteMaritimoOriginal.JustificativaCancelamento = $"Booking cancelado ao ativar novo booking {pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.Codigo } para o pedido. Atravéz de retorno positivo na integração";
                    repPedidodadosTransporteMaritimo.Atualizar(pedidoDadosTransporteMaritimoOriginal);

                    pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.Status = StatusControleMaritimo.Ativo;
                    pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.CodigoOriginal = 0;
                    pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo.BookingTemporario = false;

                    repPedidodadosTransporteMaritimo.Atualizar(pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo);

                    AtualizarDadosPedidoExportacao(pedidoDadosTransporteIntegracao.PedidoDadosTransporteMaritimo);
                }
            }

        }

        #endregion Métodos Publicos
    }
}
