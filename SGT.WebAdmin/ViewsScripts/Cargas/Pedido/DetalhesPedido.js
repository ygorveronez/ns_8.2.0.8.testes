/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />
/// <reference path="AdicionarPedido.js" />
/// <reference path="AdicionarPedidoMesmaFilial.js" />
/// <reference path="AdicionarPedidoOutraFilial.js" />
/// <reference path="AdicionarPedidoTroca.js" />
/// <reference path="AdicionarNovosPedidosPorNotasAvulsas.js" />
/// <reference path="AlterarDataPrevisaoEntrega.js" />
/// <reference path="Anexo.js" />
/// <reference path="TrocarPedido.js" />
/// <reference path="AlterarQuantidadeVolumes.js" />
/// <reference path="SelecaoMultiplosPedidos.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoContainer;
var _modalAnexosPedidoEmbarcadorDetalhesPedido;
var _modalStagePedidoEmbarcadorDetalhesPedido;
var _modalAgrupamentoStagePedidoEmbarcadorDetalhesPedido
var _modalDetalhesPedido;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoContainer = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ExigirDefinicaoReboquePedido = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.PermitirTrocarMultiplosPedidos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.QuantidadeItensProdutosPedidos = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Anexos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Pedidos = ko.observableArray([]);
    this.ExibirValorUnitarioDoProduto = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.PermitirAdicionarPedidosNaEtapaUm = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.NumeroProtocoloIntegracaoCarga = PropertyEntity({ val: ko.observable("") });

    this.Anexos.val.subscribe(recarregarGridDetalhePedidoAnexo);

    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarAnexoDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AdicionarPedido = PropertyEntity({ eventClick: adicionarPedidoDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, enable: ko.observable(true), visible: ko.observable(false) });
    this.AdicionarPedidoMesmaFilial = PropertyEntity({ eventClick: adicionarPedidoMesmaFilialDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.PedidoDaMesmaFilial, enable: ko.observable(true), visible: ko.observable(false) });
    this.AdicionarPedidoOutraFilial = PropertyEntity({ eventClick: adicionarPedidoOutraFilialDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.PedidoDeOutraFilial, enable: ko.observable(true), visible: ko.observable(false) });
    this.AdicionarPedidoTroca = PropertyEntity({ eventClick: adicionarPedidoTrocaDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.PedidoDeTroca, enable: ko.observable(true), visible: ko.observable(false) });
    this.AdicionarNovosPedidosPorNotasAvulsas = PropertyEntity({ eventClick: adicionarNovosPedidosPorNotasAvulsasDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.NovosPedidosPorNotasAvulsas, visible: ko.observable(false) });

    this.RemoverPedidosSelecao = PropertyEntity({ eventClick: RemoverPedidoDetalheModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.RemoverPedidos, visible: ko.observable(false) });
};

var DetalhePedidoExportacaoPorCarga = function (pedidoExportacao) {
    this.AcondicionamentoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AcondicionamentoDaCarga.getFieldDescription() });
    this.CargaPaletizada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CargaPaletizada.getFieldDescription() });
    this.ClienteAdicional = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ClienteFinal.getFieldDescription() });
    this.ClienteDonoContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Armador.getFieldDescription() });
    this.DataDeadLCargaNavioViagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DeadlineDaCarga.getFieldDescription() });
    this.DataDeadLineNavioViagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Deadline.getFieldDescription() });
    this.DataEstufagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDaEstufagem.getFieldDescription() });
    this.Despachante = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Despachante.getFieldDescription() });
    this.ETA = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETA.getFieldDescription() });
    this.ETS = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETS.getFieldDescription() });
    this.InLand = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InLand.getFieldDescription() });
    this.NavioViagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NavioDaViagem.getFieldDescription() });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoBooking.getFieldDescription() });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroExp.getFieldDescription() });
    this.NumeroPedidoProvisorio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedidoProvisorio.getFieldDescription() });
    this.NumeroReserva = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaReserva.getFieldDescription() });
    this.PortoViagemDestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeDestino.getFieldDescription() });
    this.PortoViagemOrigem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeOrigem.getFieldDescription() });
    this.PossuiGenset = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiGenset.getFieldDescription() });
    this.RefEXPTransferencia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroEXPTransferencia.getFieldDescription() });
    this.TipoContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDoContainer.getFieldDescription() });
    this.TipoProbe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoProbe.getFieldDescription() });
    this.ViaTransporte = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ViaTransporte.getFieldDescription() });

    PreencherObjetoKnout(this, { Data: pedidoExportacao });
};

var DetalhePedidoTransporteMaritimoPorCarga = function (pedidoTransporteMaritimo) {
    this.CodigoArmador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CodigoDoArmador.getFieldDescription() });
    this.CodigoNCM = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CodigoNCM.getFieldDescription() });
    this.CodigoRota = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CodigoDaRota.getFieldDescription() });
    this.CodigoIdentificacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DescricaoDaCargaID.getFieldDescription() });
    this.DescricaoIdentificacaoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DescricaoIdentificacaoCarga.getFieldDescription() });
    this.Incoterm = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Incoterm.getFieldDescription() })
    this.MensagemTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MensagemTransbordo.getFieldDescription() });
    this.MetragemCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MetragemDaCarga.getFieldDescription() });
    this.ModoTransporte = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ModoDeTransporte.getFieldDescription() });
    this.NomeNavio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NomeDoNavio.getFieldDescription() });
    this.NomeNavioTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NomeDoNavioDeTransbordo.getFieldDescription() });
    this.NumeroBL = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroBL.getFieldDescription() });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroContainerDoVeiculo.getFieldDescription() });
    this.NumeroLacre = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoLacre.getFieldDescription() });
    this.NumeroViagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaViagem.getFieldDescription() });
    this.NumeroViagemTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaViagemDeTransbordo.getFieldDescription() });
    this.DataBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDoBookingDataDaReserva.getFieldDescription() });
    this.DataETAOrigem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETAOrigem.getFieldDescription() });
    this.DataETASegundaOrigem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETAOrigemDois.getFieldDescription() });
    this.DataETAOrigemFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETAOrigemFinal.getFieldDescription() });
    this.DataETADestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETADestino.getFieldDescription() });
    this.DataETASegundoDestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETADestinoDois.getFieldDescription() });
    this.DataETADestinoFinal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETADestinoFinal.getFieldDescription() });
    this.DataETATransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETADoTransbordo.getFieldDescription() });
    this.DataETS = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETS.getFieldDescription() });
    this.DataETSTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ETSDoTransbordo.getFieldDescription() });
    this.DataDepositoContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DepositoDoContainer.getFieldDescription() });
    this.DataRetiradaContainerDestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RetiradaDoContainerNoDestino.getFieldDescription() });
    this.DataRetiradaVazio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RetiradaDoVazio.getFieldDescription() });
    this.DataRetornoVazio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RetornoDoVazio.getFieldDescription() });
    this.DataDeadLineCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DeadlineDaCarga.getFieldDescription() });
    this.DataDeadLineDraf = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LineDraf.getFieldDescription() });
    this.CodigoPortoCarregamentoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeCarregamentoDeTransbordoCodigo.getFieldDescription() });
    this.DescricaoPortoCarregamentoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeCarregamentoDeTransbordoDescricao.getFieldDescription() });
    this.CodigoPortoDestinoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeDestinoDeTransbordoCodigo.getFieldDescription() });
    this.DescricaoPortoDestinoTransbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeDestinoDeTransbordoDescricao.getFieldDescription() });
    this.TerminalOrigem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TerminalPortuarioDeSaida.getFieldDescription() });
    this.TipoEnvio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDoEnvio.getFieldDescription() });
    this.TipoTransporte = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDoTransporte.getFieldDescription() });
    this.Transbordo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Transbordo.getFieldDescription() });

    if (pedidoTransporteMaritimo)
        PreencherObjetoKnout(this, { Data: pedidoTransporteMaritimo });
};

var DetalhePedidoPorCarga = function (pedido) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.OcultarPedido = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.ExibirAbas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga), def: _CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga, visible: false });
    this.ExibirAbaTransporteMaritimo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.Pedido = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.DataCarregamentoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeCarregamento.getFieldDescription(), type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.GrupoPessoasDestinatario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.GrupoPessoasDestinatario.getFieldDescription(), val: ko.observable("") });
    this.RestricaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RestricaoDeEntrega.getFieldDescription(), val: ko.observable("") });
    this.PesoTotal = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PesoTotal.getFieldDescription() });
    this.PesoLiquido = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PesoLiquido.getFieldDescription() });
    this.NumeroPaletes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.NumeroDePaletes.getFieldDescription() });
    this.QuantidadeItensProdutos = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.QuantidadeProdutos.getFieldDescription() });
    this.PesoTotalPaletes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.PesoDosPaletes.getFieldDescription() });
    this.ValorFrete = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.ValorFreteParaEntrega.getFieldDescription() });
    this.ValorTotalPedido = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.ExibirValoresPedidosNaCarga) });
    this.DataPrevisaoSaida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PrevisaoDeSaida.getFieldDescription() });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PrevisaoDeEntrega.getFieldDescription(), visible: ko.observable(false) });
    this.DataPrevisaoTerminoCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrevisaoTerminoCarregamento.getFieldDescription() });

    this.LeadTimeFilialEmissora = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LeadTimeFilialEmissoraRetorno.getFieldDescription(), val: ko.observable("") });
    this.LeadTime = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LeadTimeRetorno.getFieldDescription(), val: ko.observable("") });
    this.LeadTimeTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LeadTimeTransportador.getFieldDescription(), val: ko.observable("") });
    this.PrevisaoEntregaFilialEmissora = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrevistaDeEntregaFilialEmissoraRetorno.getFieldDescription(), val: ko.observable("") });

    this.DataAgendamentoEntregaPedido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataAgendamento.getFieldDescription(), visible: ko.observable(true) });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NotasFiscais.getFieldDescription(), visible: ko.observable(true) });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Cubagem.getFieldDescription(), visible: ko.observable(true) });
    this.Temperatura = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Temperatura.getFieldDescription(), val: ko.observable("") });
    this.Vendedor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Vendedor.getFieldDescription(), val: ko.observable("") });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Ordem.getFieldDescription(), val: ko.observable("") });
    this.Ajudante = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Ajudante.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.ExigeAjudantes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ExigeAjudantes.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.TipoTomador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeTomador.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.PortoSaida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeSaida.getFieldDescription(), val: ko.observable("") });
    this.PortoChegada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeChegada.getFieldDescription(), val: ko.observable("") });
    this.Companhia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Companhia.getFieldDescription(), val: ko.observable("") });
    this.NumeroNavio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoNavio.getFieldDescription(), val: ko.observable("") });
    this.Reserva = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Reserva.getFieldDescription(), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.Resumo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Resumo.getFieldDescription(), val: ko.observable("") });
    this.TipoEmbarque = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeEmbarque.getFieldDescription(), val: ko.observable("") });
    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DeliveryTerm.getFieldDescription(), val: ko.observable("") });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.IdDeAutorizacao.getFieldDescription(), val: ko.observable("") });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroExp.getFieldDescription(), val: ko.observable("") });
    this.DataETA = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataETA.getFieldDescription(), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.DataInclusaoBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoDoBooking.getFieldDescription(), val: ko.observable("") });
    this.DataInclusaoPCP = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoDoPCP.getFieldDescription(), val: ko.observable("") });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Container.getFieldDescription(), val: ko.observable("") });
    this.PedidoViagemNavio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NavioViagemDescricao.getFieldDescription(), val: ko.observable("") });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoBooking.getFieldDescription(), val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.PortoOrigem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeOrigem.getFieldDescription(), val: ko.observable("") });
    this.PortoDestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeDestino.getFieldDescription(), val: ko.observable("") });
    this.FuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Vendedor.getFieldDescription(), val: ko.observable("") });
    this.PedidoOriginal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PedidoOriginal.getFieldDescription(), val: ko.observable("") });
    this.PedidoProvisorio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PedidoProvisorio.getFieldDescription(), val: ko.observable("") });
    this.NumeroReboqueDescricao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoReboque.getFieldDescription(), val: ko.observable(""), visible: ko.observable(pedido.NumeroReboque != EnumNumeroReboque.SemReboque) });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.NaoDefinido), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal });
    this.TipoCarregamentoPedidoDescricao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDoCarregamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedido.PedidoDestinadoAFilial) });
    this.PedidoTrocaNota = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.ExibirValoresPedidosNaCarga), text: Localization.Resources.Cargas.Carga.TrocaDeNota.getFieldDescription() });
    this.NumeroPedidoTrocaNota = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.ExibirValoresPedidosNaCarga), text: Localization.Resources.Cargas.Carga.NumeroPedidoDeTrocaDeNota.getFieldDescription() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoDoPedido.getFieldDescription(), val: ko.observable("") });
    this.DataDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeDescarregamento.getFieldDescription(), val: ko.observable("") });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EnderecoDestino.getFieldDescription(), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Remetente.getFieldDescription(), val: ko.observable("") });
    this.RemetenteEndereco = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Endereco.getFieldDescription(), val: ko.observable("") });
    this.RecebedorEndereco = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Endereco.getFieldDescription(), val: ko.observable("") });
    this.PLPCorreios = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PLP.getFieldDescription(), maxlength: 150, visible: ko.observable(true) });
    this.NumeroEtiquetaCorreios = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Objeto.getFieldDescription(), maxlength: 150, visible: ko.observable(true) });
    this.QuantidadeVolumes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Volumes.getFieldDescription(), visible: ko.observable(false) });
    this.VolumesDaNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.VolumesDaNF.getFieldDescription(), visible: ko.observable(false) });
    this.RecebeuDadosPreCalculoFrete = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DadosParaPreCalculo.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.Protocolo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Protocolo.getFieldDescription(), val: ko.observable(""), def: "" });
    this.QuantidadeVolumesNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadeDeVolumesReal.getFieldDescription(), val: ko.observable(""), def: "" });

    this.RestricoesCliente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RestricaoDeEntrega.getFieldDescription(), type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.Agendado = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Agendado.getFieldDescription(), type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.AgendadoComNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AgendadoComNF.getFieldDescription(), type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.DataAgendamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataAgendamento.getFieldDescription(), type: types.local, idGrid: guid(), visible: ko.observable(true) });

    this.AlterarDataPrevisaoSaida = PropertyEntity({ eventClick: alterarDataPrevisaoSaidaDetalhePedido, type: types.event });
    this.AlterarDataPrevisaoEntrega = PropertyEntity({ eventClick: alterarDataPrevisaoEntregaDetalhePedido, type: types.event, visible: ko.observable(true) });
    this.AlterarQuantidadeVolumes = PropertyEntity({ eventClick: alterarQuantidadeVolumesDetalhePedido, type: types.event, visible: ko.observable(false) });
    this.AlterarTipoCarregamentoPedido = PropertyEntity({ eventClick: alterarTipoCarregamentoPedidoDetalhePedidoModalClick, type: types.event });

    this.DesfazerTroca = PropertyEntity({ eventClick: desfazerTrocaPedidoDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.DesfazerTroca, visible: ko.observable(false), enable: ko.observable(true) });
    this.Anexos = PropertyEntity({ eventClick: visualizarAnexosPedidoClick, type: types.event, text: Localization.Resources.Cargas.Carga.Anexos, visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverPedido = PropertyEntity({ eventClick: removerPedidoDetalhePedidoClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverPedido, visible: ko.observable(false), enable: ko.observable(true) });
    this.TrocarPedido = PropertyEntity({ eventClick: trocarPedidoDetalhePedidoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.TrocarPedido, visible: ko.observable(false), enable: ko.observable(true) });
    this.Stage = PropertyEntity({ eventClick: visualizarStagePedidosModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.Stage, visible: ko.observable(false), enable: ko.observable(true) });
    this.AgrupamentoStage = PropertyEntity({ eventClick: visualizarAgrupamentoStagePedidosModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.AgrupamentoStage, visible: ko.observable(false), enable: ko.observable(true) });

    this.ZonaTransporte = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ZonasDeTransporte.getFieldDescription(), visible: ko.observable(false) });
    this.Recebedor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Recebedor.getFieldDescription(), visible: ko.observable(false) });
    this.Expedidor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Expedidor.getFieldDescription(), visible: ko.observable(false) });
    this.ExpedidorEndereco = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Endereco.getFieldDescription(), visible: ko.observable(false) });
    this.SenhaAgendamentoCliente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SenhaAgendamentoCliente.getFieldDescription(), visible: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeCarga.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });

    PreencherObjetoKnout(this, { Data: pedido });
};

var DetalhePedidoPorPedido = function (pedido, index) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Index = PropertyEntity({ val: ko.observable(index), def: index, getType: typesKnockout.int });
    this.OcultarPedido = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: false });
    this.PesoTotal = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.NumeroPaletes = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });

    this.DataCriacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataCriacaoDoPedido.getFieldDescription(), val: ko.observable("") });
    this.DataCarregamentoPedido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Data.getFieldDescription(), val: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroPedido.getFieldDescription(), val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoPedidoNoEmbarcador.getFieldDescription(), val: ko.observable("") });
    this.Origem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Origem.getFieldDescription(), val: ko.observable("") });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Destino.getFieldDescription(), val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Remetente.getFieldDescription(), val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Destinatario.getFieldDescription(), val: ko.observable("") });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Peso.getFieldDescription(), val: ko.observable("") });
    this.PesoCargaPedido = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PesoPedidoNaCarga.getFieldDescription(), val: ko.observable("") });
    this.Empresa = PropertyEntity({ text: Localization.Resources.Cargas.Carga.EmpresaFilial.getFieldDescription(), val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeOperacao.getFieldDescription(), val: ko.observable("") });
    this.TotalPallets = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Pallets.getFieldDescription(), val: ko.observable("") });
    this.PalletsCarregados = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PalletsCarregados.getFieldDescription(), val: ko.observable("") });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroContainer.getFieldDescription(), val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Cubagem.getFieldDescription(), val: ko.observable("") });
    this.Restricao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.RestricaoDeEntrega.getFieldDescription(), val: ko.observable("") });
    this.ObservacaoRestricao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoDeRestricao.getFieldDescription(), val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoDoPedido.getFieldDescription(), val: ko.observable("") });
    this.ObservacaoInterna = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ObservacaoInterna.getFieldDescription(), val: ko.observable("") });
    this.DataInicialColeta = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDaColeta.getFieldDescription(), val: ko.observable("") });
    this.DataPrevisaoSaida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrevisaoDeSaida.getFieldDescription(), val: ko.observable("") });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrevistaDeEntregaRetorno.getFieldDescription(), val: ko.observable("") });

    this.LeadTimeFilialEmissora = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LeadTimeFilialEmissoraRetorno.getFieldDescription(), val: ko.observable("") });
    this.LeadTime = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LeadTimeRetorno.getFieldDescription(), val: ko.observable("") });
    this.LeadTimeTransportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LeadTimeTransportador.getFieldDescription(), val: ko.observable("") });
    this.PrevisaoEntregaFilialEmissora = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataPrevistaDeEntregaFilialEmissoraRetorno.getFieldDescription(), val: ko.observable("") });

    this.Temperatura = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Temperatura.getFieldDescription(), val: ko.observable("") });
    this.Vendedor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Vendedor.getFieldDescription(), val: ko.observable("") });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Ordem.getFieldDescription(), val: ko.observable("") });
    this.PortoSaida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeSaida.getFieldDescription(), val: ko.observable("") });
    this.PortoChegada = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeChegada.getFieldDescription(), val: ko.observable("") });
    this.Companhia = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Companhia.getFieldDescription(), val: ko.observable("") });
    this.NumeroNavio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoNavio.getFieldDescription(), val: ko.observable("") });
    this.Reserva = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Reserva.getFieldDescription(), val: ko.observable("") });
    this.Resumo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Resumo.getFieldDescription(), val: ko.observable("") });
    this.TipoEmbarque = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeEmbarque.getFieldDescription(), val: ko.observable("") });
    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DeliveryTerm.getFieldDescription(), val: ko.observable("") });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.IdDeAutorizacao.getFieldDescription(), val: ko.observable("") });
    this.DataETA = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataETA.getFieldDescription(), val: ko.observable("") });
    this.DataInclusaoBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoDoBooking.getFieldDescription(), val: ko.observable("") });
    this.DataInclusaoPCP = PropertyEntity({ text: Localization.Resources.Cargas.Carga.InclusaoDoPCP.getFieldDescription(), val: ko.observable("") });
    this.NumeroContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Container.getFieldDescription(), val: ko.observable("") });
    this.PedidoViagemNavio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NavioViagemDescricao.getFieldDescription(), val: ko.observable("") });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoBooking.getFieldDescription(), val: ko.observable("") });
    this.PortoOrigem = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeOrigem.getFieldDescription(), val: ko.observable("") });
    this.PortoDestino = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PortoDeDestino.getFieldDescription(), val: ko.observable("") });
    this.NumeroReboqueDescricao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoReboque.getFieldDescription(), val: ko.observable(""), visible: ko.observable(pedido.NumeroReboque != EnumNumeroReboque.SemReboque) });
    this.TipoCarregamentoPedidoDescricao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDoCarregamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedido.PedidoDestinadoAFilial) });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OperadorDoPedido.getFieldDescription(), val: ko.observable(""), visible: ko.observable(pedido.Usuario) });
    this.FuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Vendedor.getFieldDescription(), val: ko.observable("") });
    this.QuantidadeVolumes = PropertyEntity({ text: Localization.Resources.Cargas.Carga.QuantidadeDeVolumes.getFieldDescription(), val: ko.observable("") });
    this.FormaPagamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.FormaDePagamento.getFieldDescription(), val: ko.observable("") });
    this.DiasDePrazoFatura = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DiasDePrazoDoFaturamento.getFieldDescription(), val: ko.observable("") });
    this.Tomador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Tomador.getFieldDescription(), val: ko.observable("") });
    this.Recebedor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Recebedor.getFieldDescription(), val: ko.observable("") });
    this.Expedidor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Expedidor.getFieldDescription(), val: ko.observable("") });
    this.DataAgendamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDoAgendamento.getFieldDescription(), val: ko.observable("") });
    this.SenhaAgendamentoCliente = PropertyEntity({ text: Localization.Resources.Cargas.Carga.SenhaDoAgendamento.getFieldDescription(), val: ko.observable("") });
    this.PossuiAjudanteCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiAjudanteCarga.getFieldDescription(), val: ko.observable("") });
    this.PossuiCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiCarga.getFieldDescription(), val: ko.observable("") });
    this.PossuiDescarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiDescarga.getFieldDescription(), val: ko.observable("") });
    this.PossuiAjudanteDescarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PossuiAjudanteDescarga.getFieldDescription(), val: ko.observable("") });
    this.PalletsCarregadosNestaCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.PalletsCarregadosNestaCarga.getFieldDescription(), val: ko.observable("") });
    this.RemoverPedido = PropertyEntity({ eventClick: removerPedidoDetalhePedidoClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverPedido, visible: ko.observable(false), enable: ko.observable(true) });
    PreencherObjetoKnout(this, { Data: pedido });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedido() {
    loadHtmlDetalhePedido(function () {
        loadHtmlDetalhePedidoModais(function () {
            _detalhePedidoContainer = new DetalhePedidoContainer();
            KoBindings(_detalhePedidoContainer, "divModalDetalhesPedido");

            loadDetalhePedidoAnexo();
            loadDetalhePedidoAnexoEmbarcador();
            loadDetalhePedidoAdicionarPedido();
            loadDetalhePedidoAdicionarPedidoMesmaFilial();
            loadDetalhePedidoAdicionarPedidoOutraFilial();
            loadDetalhePedidoAdicionarPedidoTroca();
            loadDetalhePedidoAdicionarNovosPedidosPorNotasAvulsas();
            loadDetalhePedidoTrocarPedido();
            loadDetalhePedidoAlterarPrevisaoEntrega();
            loadDetalhePedidoAlterarPrevisaoSaida();
            loadDetalhePedidoAlterarQuantidadeVolumes();
            loadDetalhePedidoAlterarTipoCarregamentoPedido();
            loadDetalhePedidoStagePedidoEmbarcador();
            loadDetalhePedidoAgrupamentoStagePedidoEmbarcador();
            loadSelecaoMultiplosPedidos();

            _modalAnexosPedidoEmbarcadorDetalhesPedido = new bootstrap.Modal(document.getElementById("divModalAnexosPedidoEmbarcador"), { backdrop: 'static', keyboard: true });
            _modalStagePedidoEmbarcadorDetalhesPedido = new bootstrap.Modal(document.getElementById("divModalStagePedidoEmbarcador"), { backdrop: 'static', keyboard: true });
            _modalAgrupamentoStagePedidoEmbarcadorDetalhesPedido = new bootstrap.Modal(document.getElementById("divModalAgrupamentoStagePedidoEmbarcador"), { backdrop: 'static', keyboard: true });
            _modalDetalhesPedido = new bootstrap.Modal(document.getElementById("divModalDetalhesPedido"), { backdrop: 'static', keyboard: true });
            _modalSelecaoMultiplosPedidos = new bootstrap.Modal(document.getElementById("divModalSelecaoMultiplosPedidos"), { backdrop: 'static', keyboard: true });
        });

        LocalizeCurrentPage();
    });
}

function loadHtmlDetalhePedido(callback) {

    var nomeArquivoHtmlDetalhePedido = habilitarModalDetalhesPedidosPorPedidos() ? "DetalhePedidoPorPedidos.html" : "DetalhePedidoPorCarga.html";

    $.get("Content/Static/Carga/" + nomeArquivoHtmlDetalhePedido + "?dyn=" + guid(), function (data) {
        var $containerModalDetalhesPedido = $("#containerModalDetalhesPedido");

        if ($containerModalDetalhesPedido.length == 0) {
            $("#js-page-content").append("<div id='containerModalDetalhesPedido'></div>");
            $containerModalDetalhesPedido = $("#containerModalDetalhesPedido");
        }

        $containerModalDetalhesPedido.html(data);
        callback();
    });
}

function loadHtmlDetalhePedidoModais(callback) {
    $.get("Content/Static/Carga/DetalhePedidoModais.html?dyn=" + guid(), function (data) {
        $("#container-detalhe-pedido-modais").html(data);
        callback();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarAnexoDetalhePedidoModalClick() {
    adicionarDetalhePedidoAnexo();
}

function adicionarPedidoDetalhePedidoModalClick() {
    adicionarPedidoDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), _detalhePedidoContainer.ExigirDefinicaoReboquePedido.val());
}

function adicionarPedidoMesmaFilialDetalhePedidoModalClick() {
    adicionarPedidoMesmaFilialDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), _detalhePedidoContainer.ExigirDefinicaoReboquePedido.val());
}

function adicionarPedidoOutraFilialDetalhePedidoModalClick() {
    adicionarPedidoOutraFilialDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), _detalhePedidoContainer.ExigirDefinicaoReboquePedido.val());
}

function adicionarPedidoTrocaDetalhePedidoModalClick() {
    adicionarPedidoTrocaDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), _detalhePedidoContainer.ExigirDefinicaoReboquePedido.val());
}

function adicionarNovosPedidosPorNotasAvulsasDetalhePedidoModalClick() {
    adicionarNovosPedidosPorNotasAvulsasDetalhePedido(_detalhePedidoContainer.CodigoCarga.val());
}

function alterarTipoCarregamentoPedidoDetalhePedidoModalClick(registroSelecionado) {
    alterarTipoCarregamentoPedidoDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), registroSelecionado);
}

function desfazerTrocaPedidoDetalhePedidoModalClick(registroSelecionado) {
    desfazerTrocaPedidoDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), registroSelecionado.Codigo.val());
}

function visualizarAnexosPedidoClick(registroSelecionado) {
    var codigo = registroSelecionado.Codigo.val();

    carregarAnexosPedidoEmbarcador(codigo);

    _modalAnexosPedidoEmbarcadorDetalhesPedido.show();
}

function visualizarStagePedidosModalClick(registroSelecionado) {
    var codigo = registroSelecionado.Codigo.val();
    var carga = _detalhePedidoContainer.CodigoCarga.val();

    carregarStagesPedido(codigo, carga);

    _modalStagePedidoEmbarcadorDetalhesPedido.show();
}

function visualizarAgrupamentoStagePedidosModalClick(registroSelecionado) {
    var carga = _detalhePedidoContainer.CodigoCarga.val();

    carregarAgrupamentoStagesPedido(carga);

    _modalAgrupamentoStagePedidoEmbarcadorDetalhesPedido.show();
}


function removerPedidoDetalhePedidoClick(registroSelecionado) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverPedido, function () {
        var data = {
            CodigoPedido: registroSelecionado.Codigo.val(),
            CodigoCarga: _detalhePedidoContainer.CodigoCarga.val(),
            PermitirRemoverTodos: false
        };

        removerPedidoDetalhePedido(data);
    });
}

function trocarPedidoDetalhePedidoModalClick(registroSelecionado) {
    trocarPedidoDetalhePedido(_detalhePedidoContainer.CodigoCarga.val(), registroSelecionado.Codigo.val(), _detalhePedidoContainer.PermitirTrocarMultiplosPedidos.val());
}

function RemoverPedidoDetalheModalClick() {
    exibirModalSelecaoMultiplosPedidos();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesPedidos(codigoCarga, callback) {

    if (habilitarRemocaoDeMultiplosPedidosDaCarga())
        _detalhePedidoContainer.RemoverPedidosSelecao.visible(true);

    if (habilitarModalDetalhesPedidosPorPedidos())
        exibirDetalhesPedidosPorPedidos(codigoCarga, callback);
    else
        exibirDetalhesPedidosPorCarga(codigoCarga, callback);
}

function exibirDetalhesPedidosPorCarga(codigoCarga, callback) {

    var isMultiEmbarcador = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador;
    var isMultiCte = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;

    executarReST("Carga/BuscarDetalhesDaCarga", { codigo: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var quantidadeItensProdutosPedidos = 0;
                var carga = retorno.Data.Carga;

                PreencherObjetoKnout(_detalhePedidoContainer, { Data: carga });

                _detalhePedidoContainer.Pedidos.removeAll();

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
                    _detalhePedidoContainer.AdicionarPedido.visible(carga.PermitirAdicionarPedido);
                    _detalhePedidoContainer.AdicionarPedidoMesmaFilial.visible(carga.PermitirAdicionarPedidoMesmaFilial);
                    _detalhePedidoContainer.AdicionarPedidoOutraFilial.visible(carga.PermitirAdicionarPedidoOutraFilial);
                    _detalhePedidoContainer.AdicionarPedidoTroca.visible(carga.PermitirAdicionarPedidoTroca && carga.PermitirTrocarMultiplosPedidos);
                    _detalhePedidoContainer.AdicionarNovosPedidosPorNotasAvulsas.visible(carga.PermitirAdicionarNovosPedidosPorNotasAvulsas);
                }
                else {
                    _detalhePedidoContainer.AdicionarPedido.visible(false);
                    _detalhePedidoContainer.AdicionarPedidoMesmaFilial.visible(false);
                    _detalhePedidoContainer.AdicionarPedidoOutraFilial.visible(false);
                    _detalhePedidoContainer.AdicionarPedidoTroca.visible(false);
                    _detalhePedidoContainer.AdicionarNovosPedidosPorNotasAvulsas.visible(false);
                }

                for (var i = 0; i < retorno.Data.Pedidos.length; i++) {
                    var pedido = retorno.Data.Pedidos[i].DetalhesPedido;
                    var pedidoExportacao = retorno.Data.Pedidos[i].DetalhesPedidoExportacao;
                    var pedidoTransporteMaritimo = retorno.Data.Pedidos[i].DetalhesPedidoTransporteMaritimo;
                    var knoutDetalhePedidoPorCarga = new DetalhePedidoPorCarga(pedido);
                    var knoutDetalhePedidoExportacaoPorCarga = new DetalhePedidoExportacaoPorCarga(pedidoExportacao);
                    var knoutDetalhePedidoTransporteMaritimoPorCarga = new DetalhePedidoTransporteMaritimoPorCarga(pedidoTransporteMaritimo);
                    var knoutDetalhePedido = $.extend({}, knoutDetalhePedidoPorCarga, knoutDetalhePedidoExportacaoPorCarga, knoutDetalhePedidoTransporteMaritimoPorCarga);

                    quantidadeItensProdutosPedidos += parseFloat(pedido.QuantidadeItensProdutos);

                    knoutDetalhePedido.ExibirAbaTransporteMaritimo.val(Boolean(pedidoTransporteMaritimo))
                    knoutDetalhePedido.Pedido.val(Localization.Resources.Cargas.Carga.PedidoNumero + pedido.NumeroPedidoEmbarcador);
                    knoutDetalhePedido.DataCarregamentoCarga.val(carga.DataCarregamentoCarga);
                    knoutDetalhePedido.DataPrevisaoTerminoCarregamento.val(pedido.DataPrevisaoTerminoCarregamento);

                    knoutDetalhePedido.RestricoesCliente.val(pedido.RestricoesCliente);
                    knoutDetalhePedido.Agendado.val(pedido.Agendado);
                    knoutDetalhePedido.AgendadoComNF.val(pedido.AgendadoComNF);
                    knoutDetalhePedido.DataAgendamento.val(pedido.DataAgendamento);

                    knoutDetalhePedido.Destinatario.val(pedido.Destinatario.Descricao);
                    knoutDetalhePedido.DataPrevisaoSaida.val(pedido.PrevisaoSaida);
                    knoutDetalhePedido.ValorFrete.val(Globalize.format(pedido.ValorFrete, "n2"));
                    knoutDetalhePedido.PesoTotalPaletes.val(Globalize.format(pedido.PesoTotalPaletes, "n3"));
                    knoutDetalhePedido.PesoTotal.val(Globalize.format(pedido.PesoTotal, "n3"));
                    knoutDetalhePedido.PesoLiquido.val(Globalize.format(pedido.PesoLiquido, "n3"));
                    knoutDetalhePedido.NumeroPaletes.val(Globalize.format(pedido.NumeroPaletes, "n2"));
                    knoutDetalhePedido.QuantidadeItensProdutos.val(Globalize.format(pedido.QuantidadeItensProdutos, "n2"));
                    knoutDetalhePedido.ValorTotalPedido.val(pedido.ValorPedido);
                    knoutDetalhePedido.Remetente.val(pedido.Remetente.Descricao);
                    knoutDetalhePedido.RestricaoEntrega.val(pedido.RestricaoEntrega);

                    if (isMultiEmbarcador || isMultiCte) {
                        knoutDetalhePedido.Ajudante.visible(true);
                        knoutDetalhePedido.ExigeAjudantes.visible(true);
                        knoutDetalhePedido.TipoTomador.visible(true);
                    }

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
                        knoutDetalhePedido.DesfazerTroca.visible(pedido.PermitirDesfazerTrocaPedido);
                        knoutDetalhePedido.RemoverPedido.visible(pedido.PermitirRemoverPedido);
                        knoutDetalhePedido.TrocarPedido.visible(pedido.PermitirTrocarPedidoPedido);
                    }
                    else {
                        knoutDetalhePedido.DesfazerTroca.visible(false);
                        knoutDetalhePedido.RemoverPedido.visible(false);
                        knoutDetalhePedido.TrocarPedido.visible(false);
                    }

                    if (pedido.PossuiStage) {
                        knoutDetalhePedido.Stage.visible(true);
                    } else {
                        knoutDetalhePedido.Stage.visible(false);
                    }

                    if (pedido.TipoCargaTrecho == EnumCargaTrechoSumarizada.SubCarga) {
                        knoutDetalhePedido.AgrupamentoStage.visible(true);
                        knoutDetalhePedido.Stage.visible(false);
                    } else {
                        knoutDetalhePedido.AgrupamentoStage.visible(false);
                    }

                    if (pedido.PrevisaoEntrega != "" || _CONFIGURACAO_TMS.PermitirAlterarDataPrevisaoEntregaPedidoNoCarga) {
                        knoutDetalhePedido.DataPrevisaoEntrega.val(pedido.PrevisaoEntrega);
                        knoutDetalhePedido.DataPrevisaoEntrega.visible(true);
                    }

                    knoutDetalhePedido.AlterarDataPrevisaoEntrega.visible(_CONFIGURACAO_TMS.PermitirAlterarDataPrevisaoEntregaPedidoNoCarga);

                    if (carga.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega) {
                        knoutDetalhePedido.AlterarDataPrevisaoEntrega.visible(true);
                    }

                    if (pedido.QuantidadeVolumes != "") {
                        knoutDetalhePedido.QuantidadeVolumes.visible(true);
                        knoutDetalhePedido.QuantidadeVolumes.val(pedido.QuantidadeVolumes);
                    }

                    knoutDetalhePedido.AlterarQuantidadeVolumes.visible(carga.PermitirAlterarVolumesNaCarga);

                    _detalhePedidoContainer.Pedidos.push(knoutDetalhePedido);

                    if (pedido.Produtos.length > 0) {
                        var header = [
                            { data: "Produto", title: Localization.Resources.Cargas.Carga.Produto, width: "40%" },
                            { data: "ValorUnitario", title: Localization.Resources.Cargas.Carga.ValorUnitario, width: "10%", className: "text-align-right", visible: obterVisibilidadeValorUnitario() },
                            { data: "NumeroLotePedidoProdutoLote", title: "Lote", width: "8%", className: "text-align-right" },
                            { data: "LinhaSeparacao", title: "Linha Separação", width: "8%", className: "text-align-right" },
                            { data: "Quantidade", title: Localization.Resources.Cargas.Carga.Quantidade, width: "12%", className: "text-align-right" },
                            { data: "PesoUnitario", title: Localization.Resources.Cargas.Carga.PesoUnitario, width: "10%", className: "text-align-right" },
                            { data: "Valor", title: Localization.Resources.Cargas.Carga.ValorUnitario, width: "10%", className: "text-align-right", visible: _CONFIGURACAO_TMS.ExibirValoresPedidosNaCarga },
                            { data: "PesoTotalEmbalagem", title: Localization.Resources.Cargas.Carga.PesoEmbalagem, width: "15%", className: "text-align-right", visible: !_CONFIGURACAO_TMS.ExibirValoresPedidosNaCarga },
                            { data: "PesoTotal", title: Localization.Resources.Cargas.Carga.PesoTotal, width: "12%", className: "text-align-right" },
                            { data: "ValorTotal", title: Localization.Resources.Cargas.Carga.ValorTotal, width: "12%", className: "text-align-right" }
                        ];

                        var gridProdutos = new BasicDataTable(knoutDetalhePedido.Destinatario.idGrid, header, null);

                        gridProdutos.CarregarGrid(pedido.Produtos);

                        $("#container-" + knoutDetalhePedido.Destinatario.idGrid).show();

                        knoutDetalhePedido.QuantidadeItensProdutos.visible(true);
                    }
                    else {
                        $("#container-" + knoutDetalhePedido.Destinatario.idGrid).hide();
                        knoutDetalhePedido.QuantidadeItensProdutos.visible(false);
                    }

                    if (Boolean(pedidoTransporteMaritimo) && (pedidoTransporteMaritimo.Roteamentos.length > 0)) {
                        var header = [
                            { data: "CodigoRoteamento", title: Localization.Resources.Cargas.Carga.CodigoDaRota, width: "10%", className: "text-align-center" },
                            { data: "CodigoSCAC", title: Localization.Resources.Cargas.Carga.CodigoSCAC, width: "10%", className: "text-align-center" },
                            { data: "FlagNavio", title: Localization.Resources.Cargas.Carga.FlagDoNavio, width: "10%", className: "text-align-center" },
                            { data: "NomeNavio", title: Localization.Resources.Cargas.Carga.NomeDoNavio, width: "10%", className: "text-align-center" },
                            { data: "NumeroViagem", title: Localization.Resources.Cargas.Carga.NumeroDaViagem, width: "10%", className: "text-align-center" },
                            { data: "TipoRemessa", title: Localization.Resources.Cargas.Carga.TipoDaRemessa, width: "10%", className: "text-align-center" },
                            { data: "PortoCargaLocalizacao", title: Localization.Resources.Cargas.Carga.PortoDeCarga, width: "10%", className: "text-align-center" },
                            { data: "PortoCargaData", title: Localization.Resources.Cargas.Carga.ETA, width: "10%", className: "text-align-center" },
                            { data: "PortoDescargaLocalizacao", title: Localization.Resources.Cargas.Carga.PortoDeDescarga, width: "10%", className: "text-align-center" },
                            { data: "PortoDescargaData", title: Localization.Resources.Cargas.Carga.ETS, width: "10%", className: "text-align-center" }
                        ];

                        var gridRoteamentos = new BasicDataTable("grid-pedido-transporte-maritimo-roteamento-" + pedido.Codigo, header, null);

                        gridRoteamentos.CarregarGrid(pedidoTransporteMaritimo.Roteamentos);

                        $("#container-grid-pedido-transporte-maritimo-roteamento-" + pedido.Codigo).show();
                    }
                    else
                        $("#container-grid-pedido-transporte-maritimo-roteamento-" + pedido.Codigo).hide();

                    if (callback instanceof Function)
                        callback(knoutDetalhePedido, pedido);
                }
                _detalhePedidoContainer.QuantidadeItensProdutosPedidos.visible(quantidadeItensProdutosPedidos > 0);
                _detalhePedidoContainer.QuantidadeItensProdutosPedidos.val(Globalize.format(quantidadeItensProdutosPedidos, "n2"));

                _detalhePedidoAdicionarPedido.BotaoAdicionarPorNumeroCarregamentoPedido.visible(retorno.Data.Carga.PossuiNumeroCarregamento);

                _modalDetalhesPedido.show();
                $('#divModalDetalhesPedido').on('shown.bs.modal', function () {
                    $('.detalhe-pedido-popover').popover();
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirDetalhesPedidosPorPedidos(codigoCarga, callback) {
    executarReST("Carga/BuscarDetalhesCargaPedidos", { Carga: codigoCarga }, function (retorno) {


        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalhePedidoContainer, { Data: retorno.Data.Carga });

                _detalhePedidoContainer.Pedidos.removeAll();
                var carga = retorno.Data.Carga;
                _detalhePedidoContainer.AdicionarPedido.visible(carga.PermitirAdicionarRemoverPedidosEtapa1);

                var anexos = new Array();

                for (var i = 0; i < retorno.Data.Pedidos.length; i++) {
                    var pedido = retorno.Data.Pedidos[i];

                    if (pedido.Anexos != null && pedido.Anexos != undefined)
                        anexos = anexos.concat(pedido.Anexos.slice());

                    var knoutDetalhePedido = new DetalhePedidoPorPedido(pedido, i);
                    knoutDetalhePedido.RemoverPedido.visible(carga.PermitirAdicionarRemoverPedidosEtapa1);

                    _detalhePedidoContainer.Pedidos.push(knoutDetalhePedido);

                    if (callback instanceof Function)
                        callback(knoutDetalhePedido, pedido);
                }

                carregarPedidosCargaMultiplaSelecao(_detalhePedidoContainer.CodigoCarga.val());

                _detalhePedidoContainer.Anexos.val(anexos.slice());
                recarregarGridDetalhePedidoAnexo();

                let triggerEl = document.querySelector('#tabDetalhesCargaPedidosLista a:first-of-type');
                let firstTab = new bootstrap.Tab(triggerEl);
                firstTab.show();

                _modalDetalhesPedido.show();
                $('#divModalDetalhesPedido').on('shown.bs.modal', function () {
                    //$('.detalhe-pedido-popover').popover();
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function obterVisibilidadeValorUnitario() {
    return _detalhePedidoContainer.ExibirValorUnitarioDoProduto.val();
}

function removerPedidoDetalhePedido(data) {
    executarReST("Carga/RemoverPedidoCarga", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                if (retorno.Data.NaoPermitirRemoverUltimoPedidoCarga) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.NaoPossivelRemoverUltimoPedidoSeNecessarioDeveSeCancelarCargaPeloFluxoDeCancelamento, 16000);
                }
                else if (retorno.Data.ConfirmarRemocaoPedidoViculadoCarga) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.EsseUltimoPedidoDaCargaSuaRemocaoIraCancelarCargaDesejaProsseguir, function () {
                        removerPedidoDetalhePedido({
                            CodigoPedido: data.CodigoPedido,
                            CodigoCarga: data.CodigoCarga,
                            PermitirRemoverTodos: true,
                            PermitirSeparacaoMercadoriaInformada: data.PermitirSeparacaoMercadoriaInformada,
                            PermitirAlteracoesPedidos: data.PermitirAlteracoesPedidos
                        });
                    })
                }
                else if (retorno.Data.ConfirmarSeparacaoMercadoriaInformada) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
                        removerPedidoDetalhePedido({
                            CodigoPedido: data.CodigoPedido,
                            CodigoCarga: data.CodigoCarga,
                            PermitirRemoverTodos: data.PermitirRemoverTodos,
                            PermitirSeparacaoMercadoriaInformada: true,
                            PermitirAlteracoesPedidos: data.PermitirAlteracoesPedidos
                        });
                    })
                }
                else if (retorno.Data.ConfirmarAlteracoesPedidos) {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, retorno.Data.MensagemErro + " " + Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
                        removerPedidoDetalhePedido({
                            CodigoPedido: data.CodigoPedido,
                            CodigoCarga: data.CodigoCarga,
                            PermitirRemoverTodos: data.PermitirRemoverTodos,
                            PermitirSeparacaoMercadoriaInformada: data.PermitirSeparacaoMercadoriaInformada,
                            PermitirAlteracoesPedidos: true
                        });
                    })
                }
                else {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.PedidoRemovidoComSucesso);
                    exibirDetalhesPedidos(_detalhePedidoContainer.CodigoCarga.val());
                    IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function habilitarModalDetalhesPedidosPorPedidos() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.PadraoVisualizacaoOperadorLogistico);
}

function habilitarRemocaoDeMultiplosPedidosDaCarga() {
    return _CONFIGURACAO_TMS.Carga.PermitirRemoverMultiplosPedidosCarga;
}

// #endregion Funções Privadas

