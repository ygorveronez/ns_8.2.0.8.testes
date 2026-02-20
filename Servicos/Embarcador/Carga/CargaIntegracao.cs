using System;
using System.Collections.Generic;
using System.Linq;
using com.alianca.intercab.emp.doc.booking;
using System.Text.RegularExpressions;
using Dominio.ObjetosDeValor.Embarcador.Carga;

namespace Servicos.Embarcador.Carga
{
    public class CargaIntegracao
    {

        public static void AdicionarIntegracoesEtapaNFe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaIntegracao.AdicionarIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trafegus, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, false, unitOfWork);
            Servicos.Embarcador.Carga.CargaIntegracao.AdicionarIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, false, unitOfWork);

            if (!(carga.TipoOperacao?.NaoIntegrarOpentech ?? false) && !(carga.Veiculo?.NaoIntegrarOpentech ?? false))
                Servicos.Embarcador.Carga.CargaIntegracao.AdicionarIntegracao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, false, unitOfWork);
        }

        private static bool ValidarSePodeGerarIntegracao(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoIntegracao != null)
            {
                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee)
                {
                    Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                    if (string.IsNullOrWhiteSpace(integracao?.URLIntegracaoDadosCargaDigibee))
                        return false;
                    else
                        return true;
                }
                else
                    return true;

            }
            else return false;
        }

        private static void AdicionarIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, bool naoAvancarRejeicaoIntegracaoTranspoortador, bool gerarNovaIntegracaoSeJaIntegrado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipo, true);

            if (!ValidarSePodeGerarIntegracao(tipoIntegracao, unitOfWork))
                return;

            bool integrarColeta = tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech && (configuracaoIntegracao?.IntegrarColetaOpentech ?? false);

            Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(carga, tipoIntegracao, unitOfWork, gerarNovaIntegracaoSeJaIntegrado, integrarColeta);

            if (naoAvancarRejeicaoIntegracaoTranspoortador)
            {
                carga.AguardarIntegracaoEtapaTransportador = true;
                repCarga.Atualizar(carga);
            }

        }

        public static Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterCargaEmCargaIntegracao(Dominio.ObjetosDeValor.WebService.Carga.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            cargaIntegracao.NaoAtualizarDadosDoPedido = false;

            cargaIntegracao.CargaSVMProprio = cargaIntegracaoCompleta.CargaSVMProprio.HasValue ? cargaIntegracaoCompleta.CargaSVMProprio.Value : false;
            cargaIntegracao.DataCriacaoCarga = cargaIntegracaoCompleta.DataCriacaoCarga;
            cargaIntegracao.FecharCargaAutomaticamente = false;
            cargaIntegracao.Filial = cargaIntegracaoCompleta.Filial;
            cargaIntegracao.NumeroCarga = cargaIntegracaoCompleta.NumeroCarga;
            cargaIntegracao.NumeroPreCarga = cargaIntegracaoCompleta.NumeroPreCarga;
            //cargaIntegracao.NumeroPedido = pedido.NumeroPedido;            
            //cargaIntegracao.TipoRateioProdutos = pedido.TipoRateioProdutos;
            cargaIntegracao.TipoTomador = pedido.TipoTomador;
            cargaIntegracao.TipoPagamento = pedido.TipoPagamento;
            cargaIntegracao.Remetente = pedido.Remetente;
            cargaIntegracao.Destinatario = pedido.Destinatario;
            cargaIntegracao.Tomador = pedido.Tomador;
            cargaIntegracao.Recebedor = pedido.Recebedor;
            cargaIntegracao.Expedidor = pedido.Expedidor;
            //cargaIntegracao.NotasFiscais = pedido.NotasFiscais;
            //cargaIntegracao.PossuiCTe = pedido.PossuiCTe;
            //cargaIntegracao.PossuiNFS = pedido.PossuiNFS;
            //cargaIntegracao.PossuiNFSManual = pedido.PossuiNFSManual;
            //cargaIntegracao.PalletAgrupamento = pedido.PalletAgrupamento;
            //cargaIntegracao.PedidoCancelado = pedido.PedidoCancelado;
            cargaIntegracao.CNPJsDestinatariosNaoAutorizados = pedido.CNPJsDestinatariosNaoAutorizados;
            cargaIntegracao.CargaRefrigeradaPrecisaEnergia = pedido.CargaRefrigeradaPrecisaEnergia;
            cargaIntegracao.CentroCusto = pedido.CentroCusto;
            cargaIntegracao.CodigoBooking = pedido.CodigoBooking;
            cargaIntegracao.CodigoOrdemServico = pedido.CodigoOrdemServico;
            cargaIntegracao.Container = pedido.Container;
            if (cargaIntegracao.Container != null)
                cargaIntegracao.Container.Atualizar = true;

            cargaIntegracao.ContainerADefinir = pedido.ContainerADefinir;
            cargaIntegracao.ContemCargaPerigosa = pedido.ContemCargaPerigosa;
            cargaIntegracao.ContemCargaRefrigerada = pedido.ContemCargaRefrigerada;
            cargaIntegracao.CubagemTotal = pedido.CubagemTotal;
            cargaIntegracao.DataFinalCarregamento = pedido.DataInicioCarregamento;
            cargaIntegracao.DataInicioCarregamento = pedido.DataInicioCarregamento;
            cargaIntegracao.DataPrevisaoEntrega = pedido.DataPrevisaoEntrega;
            cargaIntegracao.DataPrevisao = pedido.DataPrevisaoSaida;
            cargaIntegracao.DataColeta = pedido.DataColeta;
            cargaIntegracao.DescricaoCarrierNavioViagem = pedido.DescricaoCarrierNavioViagem;
            cargaIntegracao.DescricaoTipoPropostaFeeder = pedido.DescricaoTipoPropostaFeeder;
            cargaIntegracao.Destino = pedido.Destino;
            cargaIntegracao.EmpresaResponsavel = pedido.EmpresaResponsavel != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.EmpresaResponsavel() { CodigoIntegracao = pedido.EmpresaResponsavel.CodigoIntegracao } : null;
            cargaIntegracao.Embarcador = pedido.Embarcador;
            cargaIntegracao.FormaAverbacaoCTE = pedido.FormaAverbacaoCTE;
            cargaIntegracao.NecessitaAverbacao = pedido.NecessitaAverbacao;
            cargaIntegracao.NumeroBL = pedido.NumeroBL;
            cargaIntegracao.NumeroBooking = pedido.NumeroBooking;
            cargaIntegracao.NumeroLacre1 = pedido.NumeroLacre1;
            cargaIntegracao.NumeroLacre2 = pedido.NumeroLacre2;
            cargaIntegracao.NumeroLacre3 = pedido.NumeroLacre3;
            cargaIntegracao.NumeroOrdemServico = pedido.NumeroOrdemServico;
            cargaIntegracao.NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador;
            cargaIntegracao.Observacao = pedido.Observacao;
            cargaIntegracao.ObservacaoLocalEntrega = pedido.ObservacaoLocalEntrega;
            cargaIntegracao.ObservacaoProposta = pedido.ObservacaoProposta;
            cargaIntegracao.Ordem = pedido.Ordem;
            cargaIntegracao.Origem = pedido.Origem;
            cargaIntegracao.PercentualADValorem = pedido.PercentualADValorem;
            cargaIntegracao.PesoBruto = pedido.PesoBruto;
            cargaIntegracao.PortoDestino = pedido.PortoDestino;
            cargaIntegracao.PortoOrigem = pedido.PortoOrigem;
            cargaIntegracao.Produtos = pedido.ProdutosPedido;
            cargaIntegracao.PropostaComercial = pedido.PropostaComercial;
            cargaIntegracao.ProvedorOS = pedido.ProvedorOS;
            cargaIntegracao.QuantidadeConhecimentosTaxaDocumentacao = pedido.QuantidadeConhecimentosTaxaDocumentacao;
            cargaIntegracao.QuantidadeContainerBooking = pedido.QuantidadeContainerBooking;
            cargaIntegracao.QuantidadeTipoContainerReserva = pedido.QuantidadeTipoContainerReserva;
            cargaIntegracao.QuantidadeVolumes = pedido.QuantidadeVolumes;
            cargaIntegracao.RealizarCobrancaTaxaDocumentacao = pedido.RealizarCobrancaTaxaDocumentacao;
            cargaIntegracao.TaraContainer = pedido.TaraContainer;
            cargaIntegracao.Temperatura = pedido.Temperatura;
            cargaIntegracao.TemperaturaObservacao = pedido.TemperaturaObservacao;
            cargaIntegracao.TerminalPortoDestino = pedido.TerminalPortoDestino;
            cargaIntegracao.TerminalPortoOrigem = pedido.TerminalPortoOrigem;
            cargaIntegracao.TipoCargaEmbarcador = pedido.TipoCargaEmbarcador;
            cargaIntegracao.TipoContainerReserva = pedido.TipoContainerReserva;
            cargaIntegracao.TipoDocumentoAverbacao = pedido.TipoDocumentoAverbacao;
            cargaIntegracao.TipoOperacao = pedido.TipoOperacao;
            cargaIntegracao.TipoPropostaFeeder = pedido.TipoPropostaFeeder;
            cargaIntegracao.Transbordo = pedido.Transbordo;
            cargaIntegracao.TransportadoraEmitente = pedido.TransportadoraEmitentePedido;
            cargaIntegracao.UsarOutroEnderecoDestino = pedido.UsarOutroEnderecoDestino;
            cargaIntegracao.UsarOutroEnderecoOrigem = pedido.UsarOutroEnderecoOrigem;
            cargaIntegracao.ValidarNumeroContainer = pedido.ValidarNumeroContainer;
            cargaIntegracao.ValorDescarga = pedido.ValorDescarga;
            cargaIntegracao.ValorFrete = pedido.ValorFrete;
            cargaIntegracao.ValorFreteCalculado = pedido.ValorFreteCalculado;
            cargaIntegracao.ValorPedagio = pedido.ValorPedagio;
            cargaIntegracao.ValorTaxaDocumento = pedido.ValorTaxaDocumento;
            cargaIntegracao.ValorTotalPaletes = pedido.ValorTotalPaletes;
            cargaIntegracao.ViagemJaFoiFinalizada = pedido.ViagemJaFoiFinalizada;
            cargaIntegracao.Viagem = pedido.Viagem;
            cargaIntegracao.ViagemLongoCurso = pedido.ViagemLongoCurso;
            cargaIntegracao.TipoServicoCarga = pedido.TipoServicoCarga;
            cargaIntegracao.ExecaoCab = pedido.ExecaoCap;
            cargaIntegracao.EssePedidopossuiPedidoBonificacao = pedido?.EssePedidopossuiPedidoBonificacao ?? false;
            cargaIntegracao.EssePedidopossuiPedidoVenda = pedido?.EssePedidopossuiPedidoVenda ?? false;
            cargaIntegracao.NumeroPedidoVinculado = pedido?.NumeroPedidoVinculado ?? string.Empty;

            return cargaIntegracao;
        }

        public Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterBookingEmCargaIntegracao(IntercabDocBooking booking, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP)
        {
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao("");

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            cargaIntegracao.NumeroCarga = booking.bookingNumber;
            cargaIntegracao.NumeroBooking = booking.bookingNumber;
            cargaIntegracao.PropostaComercial = PreencherPropostaComercial(booking);
            cargaIntegracao.Remetente = PreencherPessoa(booking.bookingShipper);
            cargaIntegracao.TipoTomador = ObterTipoTomador(booking.bookingResponsibleForPayment, cargaIntegracao);
            cargaIntegracao.TipoTomadorCabotagem = ObterTipoTomadorCabotagem(booking.bookingResponsibleForPayment);
            cargaIntegracao.TipoModalPropostaCabotagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalPropostaCabotagem.PortoPorto;
            cargaIntegracao.Tomador = PreencherPessoa(booking.agreementCustomer);
            cargaIntegracao.ValorFreteCobradoCliente = Convert.ToDecimal(booking.pricingArchitectureEvent.totalRate);
            cargaIntegracao.DescricaoTipoPropostaFeeder = booking.bookingCustomerType.ToUpper().Equals("CABOTAGE") ? "Carga Fechada" : "Feeder";
            cargaIntegracao.TipoPropostaCabotagem = ObterTipoPropostaCabotagem(booking.bookingCustomerType.ToUpper());
            cargaIntegracao.NavioViagem = PreencherListaPedidoViagemNavio(booking.legBookingList);
            cargaIntegracao.ProdutoPredominante = PreencherProdutoPredominante(booking.commodity);
            cargaIntegracao.PortoOrigem = PreencherPorto(booking.legBookingList.FirstOrDefault().portOrigin);
            cargaIntegracao.PortoDestino = PreencherPorto(booking.legBookingList.FirstOrDefault().portDestination);
            cargaIntegracao.TerminalPortoOrigem = PreencherTerminalPorto(booking.legBookingList.FirstOrDefault().portTerminalOrigin);
            cargaIntegracao.TerminalPortoDestino = PreencherTerminalPorto(booking.legBookingList.FirstOrDefault().portTerminalDestination);
            cargaIntegracao.TipoContainerReserva = PreencherTipoContainer(booking);
            //cargaIntegracao.Container = PreencherContainer(booking);
            cargaIntegracao.QuantidadeContainerBooking = (int)booking.equipment.quantity;
            cargaIntegracao.ValorFrete = PreencherListaComponenteFretePedido(booking.pricingArchitectureEvent);
            cargaIntegracao.FecharCargaAutomaticamente = true;
            cargaIntegracao.EmpresaResponsavel = new Dominio.ObjetosDeValor.Embarcador.Carga.EmpresaResponsavel { CodigoIntegracao = "1" };
            cargaIntegracao.Viagem = PreencherViagem(booking.legBookingList);
            if (tipoOperacao != null)
            {
                cargaIntegracao.TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(tipoOperacao);
                cargaIntegracao.TipoOperacaoPedido = serWSTipoOperacao.ConverterObjetoTipoOperacao(tipoOperacao);
            }

            if (!string.IsNullOrWhiteSpace(booking.specialCargo.imo.un))
            {
                cargaIntegracao.ContemCargaPerigosa = true;
                cargaIntegracao.ImprimirObservacaoCTe = true;
                cargaIntegracao.ObservacaoCTe = $" CARGA IMO CLASS: {booking.specialCargo.imo.imoClass} / SEQ.: {booking.specialCargo.imo.imoSubClass} / UN: {booking.specialCargo.imo.un} ";
            }

            if (booking.specialCargo.reefer.temperature != null)
            {
                cargaIntegracao.CargaRefrigeradaPrecisaEnergia = true;
                cargaIntegracao.ImprimirObservacaoCTe = true;
                cargaIntegracao.Temperatura = booking.specialCargo.reefer.temperature.ToString();
            }

            if (configuracaoIntegracaoEMP != null && configuracaoIntegracaoEMP.TipoDeCarga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador tipoCargaEmbarcador = new() { CodigoIntegracao = configuracaoIntegracaoEMP.TipoDeCarga?.CodigoTipoCargaEmbarcador, Descricao = configuracaoIntegracaoEMP.TipoDeCarga?.Descricao };
                cargaIntegracao.TipoCargaEmbarcador = tipoCargaEmbarcador;
            }

            if (booking.legBookingList.Count > 1)
                cargaIntegracao.Transbordo = PreencherTransbordo(booking.legBookingList);

            return cargaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.PropostaComercial PreencherPropostaComercial(IntercabDocBooking booking)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.PropostaComercial()
            {
                CodigoIntegracao = 0,
                Descricao = booking.bookingCustomerType.Equals("Cabotage") || booking.bookingCustomerType.Equals("CABOTAGE") ? "Carga Fechada" : "Feeder"
            };
        }

        private Dominio.Enumeradores.TipoTomador ObterTipoTomador(string tipoTomador, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            switch (tipoTomador)
            {
                case "Remetente":
                    return Dominio.Enumeradores.TipoTomador.Remetente;
                case "Embarcador":
                    return Dominio.Enumeradores.TipoTomador.Remetente;
                case "Terceiro":
                    cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    return Dominio.Enumeradores.TipoTomador.Outros;
                default:
                    return Dominio.Enumeradores.TipoTomador.Destinatario;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem ObterTipoTomadorCabotagem(string tipoTomadorCabotagem)
        {
            switch (tipoTomadorCabotagem)
            {
                case "Destinatario":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem.Destinatario;
                case "Remetente":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem.Remetente;
                case "Embarcador":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem.Remetente;
                case "Terceiro":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem.Outros;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem.Outros;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem ObterTipoPropostaCabotagem(string tipoPropostaCabotagem)
        {
            switch (tipoPropostaCabotagem)
            {
                case "CABOTAGE":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem.Cabotagem;
                case "FEEDER":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem.Feeder;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem.Outros;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa PreencherPessoa(com.alianca.intercab.emp.doc.booking.ShipperEvent customer)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
            {
                TipoPessoa = customer.personType.Equals("CNPJ") ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica,
                NomeFantasia = customer.customerName,
                CPFCNPJ = customer.taxID.ObterSomenteNumeros(),
                Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                {
                    InscricaoEstadual = customer.stateRegistration,
                    CEP = customer.zipCode,
                    EnderecoConcatenado = customer.address,
                    Numero = customer.number,
                    Bairro = customer.district,
                    Cidade = new Dominio.ObjetosDeValor.Localidade()
                    {
                        Descricao = customer.city.name,
                        CodigoIntegracao = string.Join(",", customer.city.alternateCodes.ToList())
                    },
                    Estado = new Dominio.ObjetosDeValor.Localidade()
                    {
                        Descricao = customer.state.name,
                        CodigoIntegracao = string.Join(",", customer.state.alternateCodes.ToList())
                    }
                },
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa PreencherPessoa(com.alianca.intercab.emp.doc.booking.AgreementCustomerEvent customer)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
            {
                TipoPessoa = customer.personType.Equals("CNPJ") ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica,
                NomeFantasia = customer.customerName,
                CPFCNPJ = customer.taxID.ObterSomenteNumeros(),
                Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                {
                    InscricaoEstadual = customer.stateRegistration,
                    CEP = customer.zipCode,
                    EnderecoConcatenado = customer.address,
                    Numero = customer.number,
                    Bairro = customer.district,
                    Cidade = new Dominio.ObjetosDeValor.Localidade()
                    {
                        Descricao = customer.city.name,
                        CodigoIntegracao = string.Join(",", customer.city.alternateCodes.ToList())
                    },
                    Estado = new Dominio.ObjetosDeValor.Localidade()
                    {
                        Descricao = customer.state.name,
                        CodigoIntegracao = string.Join(",", customer.state.alternateCodes.ToList())
                    }
                },
            };
        }

        private Dominio.ObjetosDeValor.WebService.Carga.NavioViagem PreencherListaPedidoViagemNavio(IList<LegBookingEvent> legBookingList)
        {
            Dominio.ObjetosDeValor.WebService.Carga.NavioViagem navioViagem = new Dominio.ObjetosDeValor.WebService.Carga.NavioViagem()
            {
                Codigo = legBookingList.FirstOrDefault().vesselImoNumber + legBookingList.FirstOrDefault().voyageAndDirection,
                Nome = legBookingList.FirstOrDefault().vesselName.name,// + legBookingList.FirstOrDefault().voyageAndDirection
            };

            return navioViagem;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Viagem PreencherViagem(IList<LegBookingEvent> legBookingList)
        {
            string direcao = Regex.Replace(legBookingList.FirstOrDefault().voyageAndDirection, @"[\d]", "");

            Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Carga.Viagem()
            {
                Navio = new Dominio.ObjetosDeValor.Embarcador.Carga.Navio
                {
                    Descricao = legBookingList.FirstOrDefault().vesselName.name,
                    CodigoIMO = legBookingList.FirstOrDefault().vesselImoNumber,
                    CodigoIRIN = legBookingList.FirstOrDefault().callSign,
                    CodigoIntegracao = legBookingList.FirstOrDefault().vesselImoNumber + legBookingList.FirstOrDefault().voyageAndDirection,
                    Atualizar = true,
                    NavioIntegracaoBooking = true,
                },
                Descricao = legBookingList.FirstOrDefault().vesselName.name + legBookingList.FirstOrDefault().voyageAndDirection,
                Direcao = direcao == "N" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Norte
                : direcao == "S" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Sul
                : direcao == "O" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Oeste
                : direcao == "L" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Leste
                : direcao == "E" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Leste
                : direcao == "W" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Oeste : Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Sul,
                NumeroViagem = Utilidades.String.OnlyNumbers(legBookingList.FirstOrDefault().voyageAndDirection).ToInt(0),
                InativarCadastro = false,
                Atualizar = true,
            };

            return viagem;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.Produto PreencherProdutoPredominante(GenericEvent produto)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
            {
                DescricaoProduto = produto.name ?? "não informado",
                CodigoProduto = produto.alternateCodes.FirstOrDefault()?.code ?? "sem código"
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Porto PreencherPorto(GenericEvent port)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.Porto
            {
                Descricao = port.name,
                CodigoIntegracao = port.alternateCodes.FirstOrDefault()?.code
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto PreencherTerminalPorto(GenericEvent port)
        {
            string codigoIntegracao = port.alternateCodes != null && port.alternateCodes.Count > 0 ? port.alternateCodes.Where(t => t.codeType.Equals("Maersk Code") || t.codeType.Equals("maersk code") || t.codeType.Equals("Maersk code")).FirstOrDefault()?.code ?? "" : port.alternateCodes.FirstOrDefault()?.code;
            return new Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto
            {
                Descricao = port.name,
                CodigoIntegracao = string.IsNullOrWhiteSpace(codigoIntegracao) ? port.alternateCodes.FirstOrDefault()?.code : codigoIntegracao
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer PreencherTipoContainer(IntercabDocBooking booking)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer()
            {
                Descricao = booking.equipment.size + booking.equipment.type,
                Tara = Convert.ToDecimal(booking.equipment.equipmentTareWeight)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Container PreencherContainer(IntercabDocBooking booking)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.Container()
            {
                TipoContainer = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer
                {
                    Descricao = booking.equipment.size + booking.equipment.type,
                    Tara = Convert.ToDecimal(booking.equipment.equipmentTareWeight)
                },
                TipoPropriedade = ObterTipoPropriedadeContainer(booking.equipment.ownership)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer ObterTipoPropriedadeContainer(string ownership)
        {
            switch (ownership)
            {
                case "Aliança":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer.Proprio;
                case "Cliente":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer.Soc;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer.Nenhum;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor PreencherListaComponenteFretePedido(PricingArchitectureEvent pricingArchitectureAVRO)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

            IList<PricingArchitectureCostEvent> listaComponentesFrete = pricingArchitectureAVRO.pricingArchitectureCostList;

            foreach (PricingArchitectureCostEvent componenteFrete in listaComponentesFrete)
            {
                componentesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional
                {
                    Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente() { CodigoIntegracao = componenteFrete.costId.ToString() },
                    Descricao = componenteFrete.costName,
                    ValorComponente = componenteFrete.totalCost.HasValue ? Convert.ToDecimal(componenteFrete.totalCost.Value) : 0
                });
            }

            freteValor.FreteProprio = pricingArchitectureAVRO.totalRate.HasValue ? Convert.ToDecimal(pricingArchitectureAVRO.totalRate.Value) : 0;

            return freteValor;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo> PreencherTransbordo(IList<LegBookingEvent> legBookingList)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo> listaTransbordo = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo>();

            foreach (LegBookingEvent legBooking in legBookingList)
            {
                if (legBooking.legNumber == 1)
                    continue;

                int seguencia = 1;
                string direcao = Regex.Replace(legBooking.voyageAndDirection, @"[\d]", "");

                listaTransbordo.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo()
                {
                    Viagem = new Dominio.ObjetosDeValor.Embarcador.Carga.Viagem
                    {
                        NumeroViagem = Utilidades.String.OnlyNumbers(legBooking.voyageAndDirection).ToInt(),
                        Direcao = direcao == "N" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Norte
                        : direcao == "S" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Sul
                        : direcao == "O" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Oeste
                        : direcao == "L" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Leste
                        : direcao == "E" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Leste
                        : direcao == "W" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Oeste : Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Sul,
                        Navio = new Dominio.ObjetosDeValor.Embarcador.Carga.Navio
                        {
                            Descricao = legBooking.vesselName.name,
                        }                        
                    },
                    Porto = new Dominio.ObjetosDeValor.Embarcador.Carga.Porto
                    {
                        Descricao = legBooking.portDestination.name,
                        CodigoIntegracao = legBooking.portDestination.alternateCodes.FirstOrDefault()?.code
                    },
                    Terminal = new Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto()
                    {
                        Descricao = legBooking.portTerminalDestination.name,
                        CodigoIntegracao = legBooking.portTerminalDestination.alternateCodes.FirstOrDefault()?.code,
                        Porto = new Dominio.ObjetosDeValor.Embarcador.Carga.Porto()
                        {
                            Descricao = legBooking.portDestination.name,
                            CodigoIntegracao = legBooking.portDestination.alternateCodes.FirstOrDefault()?.code
                        }
                    },
                    Navio = new Dominio.ObjetosDeValor.Embarcador.Carga.Navio
                    {
                        Descricao = legBooking.vesselName.name,
                    },
                    Sequencia = seguencia
                });

                seguencia++;
            }

            return listaTransbordo;
        }
    }
}
