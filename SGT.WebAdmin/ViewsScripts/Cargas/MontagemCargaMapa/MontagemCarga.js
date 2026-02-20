/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SessaoRoteirizador.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/CategoriaPessoa.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Deposito.js" />
/// <reference path="../../Consultas/LinhaSeparacao.js" />
/// <reference path="../../Consultas/RestricaoEntrega.js" />
/// <reference path="../../Consultas/GrupoProduto.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="DetalhesPedidoProdutos.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="Locais.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _preFiltroMontagemCarga;
var _pesquisaMontegemCarga;
var _pesquisaMontegemCargaCarregamentos;
var _centralizarMapaNosPontos = null;
var _objPesquisaMontagem = {};
var _configuracoesMontagemCarga;

var PesquisaMontegemCargaCarregamentos = function () {

    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-md-4") });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedidoEmbarcador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-md-2") });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedido.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Produto.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-md-4"), visible: ko.observable(true) });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-md-2") });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            filtrarCarregamentos();
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            limparFiltrosCarregamentos();
        }, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.LimparFiltros, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PreFiltroMontagemCarga = function () {

    this.CodigoFiltro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NomeFiltro = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), val: ko.observable(""), def: "", visible: true, required: true });
    //this.FiltroPesquisa = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Cancelar = PropertyEntity({ eventClick: limparPreFiltroMontagemCargaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Novo), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirPreFiltroMontagemCargaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false) });
    this.Alterar = PropertyEntity({ eventClick: salvarPreFiltroMontagemCargaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.SalvarFiltros) });

    this.CodigoFiltro.val.subscribe(function (novoValor) {
        var text = Localization.Resources.Cargas.MontagemCargaMapa.SalvarFiltros;
        var visible = false;
        if (novoValor > 0) {
            text = Localization.Resources.Cargas.MontagemCargaMapa.AtualizarFiltros;
            visible = true;
        }
        _pesquisaMontegemCarga.SalvarPreFiltros.text(text);
        _preFiltroMontagemCarga.Alterar.text(text);
        _preFiltroMontagemCarga.Cancelar.visible(visible);
        _preFiltroMontagemCarga.Excluir.visible(visible);
    });
}

var PesquisaMontegemCarga = function () {
    var dataInicial = moment().add(-1, 'days').format("DD/MM/YYYY");
    var dataFinal = moment().add(6, 'days').format("DD/MM/YYYY");

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisaoEntregaDe.getFieldDescription()), getType: typesKnockout.date, val: ko.observable(dataInicial), required: ko.observable(false) });
    this.DataFim = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisaoEntregaAte.getFieldDescription()), dateRangeInit: this.DataInicio, getType: typesKnockout.date, val: ko.observable(dataFinal), required: ko.observable(false) });
    this.DataCriacaoPedidoInicio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.DataCriacaoDe.getFieldDescription()), getType: typesKnockout.date, required: ko.observable(false) });
    this.DataCriacaoPedidoLimite = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.DataCriacaoAte.getFieldDescription()), dateRangeInit: this.DataInicioCriacaoPedido, getType: typesKnockout.date, required: ko.observable(false) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDaCarga.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), cssClass: ko.observable("col col-xs-6 col-sm-6 col-md-2 col-lg-2") });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedidoEmbarcador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcadorDe = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedidoEmbarcadorDe.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcadorAte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedidoEmbarcadorAte.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroPedido.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), cssClass: ko.observable("col col-xs-6 col-sm-6 col-md-2 col-lg-2") });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroBooking, val: ko.observable(""), def: "", maxlength: 150, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroOrdemServico.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.NavioBarraViagemBarraDirecao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getFieldDescription()), issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Filial.getRequiredFieldDescription(), issue: 70, idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.FilialVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PreFiltros = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.PreFiltros.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.RegiaoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Regiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Vendedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Gerente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Gerente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Supervisor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.PedidosOrigemRecebedor = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.RoteirizacaoPedidosRedespacho, def: false, enable: ko.observable(true) });
    this.ComSessaoRoteirizador = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, visible: false });
    this.OpcaoSessaoRoteirizador = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.bool, def: 0, visible: false });
    this.SessaoRoteirizador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.SessaoRoteirizador.getFieldDescription(), issue: 70, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Remetente.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Deposito.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Origem.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Destino.getFieldDescription(), issue: 16, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacao.getFieldDescription(), issue: 121, visible: ko.observable(true), idBtnSearch: guid(), cssClass: "col col-sm-3 col-md-3 col-lg-3" });
    this.EstadosOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.UFOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.EstadosDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.UFDestino.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoPessoaRemetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.GrupoDePessoaRemetente.getFieldDescription(), idBtnSearch: guid() });
    this.GerarCargasDeRedespacho = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.GerarCargasDeRedespacho, def: false });
    this.GerarCargasDeColeta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.GerarCargasDeColeta, def: false, visible: ko.observable(_CONFIGURACAO_TMS.ExibirPedidoDeColeta) });
    this.ExibirMicroRegioesRoteirizacao = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.ExibirRegioesRoteirizacao, def: true, visible: ko.observable(_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente) });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Ordem.getFieldDescription(), maxlength: 50 });
    this.PortoSaida = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PortoDeSaida.getFieldDescription(), maxlength: 150 });
    this.TipoEmbarque = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeEmbarque.getFieldDescription(), maxlength: 150 });
    this.PaisDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.PaisDeDestino.getFieldDescription(), idBtnSearch: guid() });
    this.Reserva = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Reserva.getFieldDescription(), maxlength: 150 });
    this.DeliveryTerm = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.TermoDeEntrega.getFieldDescription(), maxlength: 150 });
    this.IdAutorizacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.IDDeAutorizacao.getFieldDescription(), maxlength: 150 });
    this.NumeroTransporte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroTransporte.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCarregamentoPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });;
    this.DataInclusaoBookingInicial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InclusaoBookingInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoBookingLimite = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InclusaoBookingLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInclusaoPCPInicial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InclusaoPCPInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataInclusaoPCPLimite = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.InclusaoPCPLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.ChaveNotaFiscalEletronica = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ChaveNotaFiscalEletronica.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.MontagemCarga.AtivarMontagemCargaPorNFe), required: ko.observable(false), maxlength: 44 });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.DataCriacaoPedidoInicio.dateRangeLimit = this.DataCriacaoPedidoLimite;
    this.DataCriacaoPedidoLimite.dateRangeInit = this.DataCriacaoPedidoInicio;
    this.DataInclusaoBookingInicial.dateRangeLimit = this.DataInclusaoBookingLimite;
    this.DataInclusaoBookingLimite.dateRangeInit = this.DataInclusaoBookingInicial;
    this.DataInclusaoPCPInicial.dateRangeLimit = this.DataInclusaoPCPLimite;
    this.DataInclusaoPCPLimite.dateRangeInit = this.DataInclusaoPCPInicial;

    this.SalvarPreFiltros = PropertyEntity({
        eventClick: function (e) {
            SalvarPreFiltrosMontagemCarga();
        }, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.SalvarFiltros), idGrid: guid(), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarDadosMontagemCarga(this.OpcaoSessaoRoteirizador.val());
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.CodigosCanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.CanalEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoaDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.GrupoDePessoaDestinatario.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosCategoriaClientes = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.CategoriaClientes.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    //#4429
    this.PesoDe = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoDe.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PesoAte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PesoAte.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PalletDe = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PalletDe.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.PalletAte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PalletAte.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.VolumeDe = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.VolumeDe.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.VolumeAte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.VolumeAte.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    //#34971
    this.ValorDe = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ValorDe.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorAte = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.ValorAte.getFieldDescription(), val: ko.observable(""), def: "0,00", visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

    //this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.NaoRecebeCargaCompartilhada = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.ClientesQueNaoRecebeCargaCompartilhada, def: false });
    this.SomentePedidosComNota = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.SomentePedidosComNotasFiscais, def: false });
    this.PedidosParaReentrega = PropertyEntity({ visible: ko.observable(true), val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.SomentePedidoReentrega, def: false });
    this.PedidosSessao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PedidosSessao.getFieldDescription(), options: EnumPedidoSessao.obterOpcoes(), val: ko.observable(EnumPedidoSessao.Todos), def: EnumPedidoSessao.Todos, visible: ko.observable(true) });

    //ASSAI - Filtro de protudos do carregamento
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Produto.getFieldDescription(), issue: 52, idBtnSearch: guid() });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroCarregamento.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.CodigosTipoDeCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.CodigosLinhaSeparacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.LinhaSeparacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigosRestricoesEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.RestricaoEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigosGrupoProdutos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.GrupoProdutos.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigosProdutos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Produtos, idBtnSearch: guid(), visible: ko.observable(true) });

    //SAINTGOBAIN
    this.UsarTipoTomadorPedido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.MontagemCargaMapa.UsarTipoTomador, def: false, enable: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ val: ko.observable(EnumTipoTomador.Remetente), options: EnumTipoTomador.obterOpcoes(), def: EnumTipoTomador.Remetente, text: Localization.Resources.Cargas.MontagemCargaMapa.TipoTomador.getFieldDescription(), required: false });
    this.CodigosRotaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.RotaDeFrete.getFieldDescription(), idBtnSearch: guid() });
    //this.CodigosUsuario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.UsuarioRemessa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.UsuarioRemessa.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.CodigoProcessamentoEspecial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.ProcessamentoEspecial.getFieldDescription(), idBtnSearch: guid() });
    this.CodigoHorarioEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.TurnoHorarioEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.RestricaoDiasEntrega = PropertyEntity({ val: ko.observable(new Array()), getType: typesKnockout.selectMultiple, options: EnumDiaSemana.obterOpcoes(), def: new Array(), text: Localization.Resources.Cargas.MontagemCargaMapa.RestricaoDiasDaSemana.getFieldDescription() });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Tomador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoZonaTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.ZonaDeTransporte.getFieldDescription(), idBtnSearch: guid() });
    this.CodigoDetalheEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.DetalheDeEntrega.getFieldDescription(), idBtnSearch: guid() });

    this.PedidosOrigemRecebedor.val.subscribe(function (val) {
        _pesquisaMontegemCarga.Expedidor.visible(val);
        _pesquisaMontegemCarga.Expedidor.required(val);
        if (!val) {
            _pesquisaMontegemCarga.Expedidor.codEntity(0);
            _pesquisaMontegemCarga.Expedidor.val('');
        }
    });

    this.PedidosBloqueados = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PedidoBloqueado.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, visible: ko.observable(true) });
    this.PedidosRestricaoData = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PedidoRestricaoData.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, visible: ko.observable(true) });
    this.PedidosRestricaoPercentual = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.PedidoRestricaoPercentual.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2, visible: ko.observable(true) });

    this.DataAgendamentoInicial = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAgendamentoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataAgendamentoFinal = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataAgendamentoFinal.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;
    this.RequisicaoEnviadaMontagemCargaMapa = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });
}

//*******EVENTOS*******

function DataInicioModificada(val) {
    var dataInicio = moment(val, "DD/MM/YYYY");
    var dataFim = moment(_pesquisaMontegemCarga.DataFim.val(), "DD/MM/YYYY");
    var diferenca = dataFim.diff(dataInicio, 'days', true);

    if (diferenca > 7) {
        dataFim = dataInicio.add(7, 'days').format("DD/MM/YYYY");
        _pesquisaMontegemCarga.DataFim.val(dataFim);
        _pesquisaMontegemCarga.DataFim.get$().change()
    }
}

function DataFimModificada(val) {
    var dataInicio = moment(_pesquisaMontegemCarga.DataInicio.val(), "DD/MM/YYYY");
    var dataFim = moment(val, "DD/MM/YYYY");
    var diferenca = dataFim.diff(dataInicio, 'days', true);

    if (diferenca > 7) {
        dataInicio = dataFim.add(-7, 'days').format("DD/MM/YYYY");
        _pesquisaMontegemCarga.DataInicio.val(dataInicio);
        _pesquisaMontegemCarga.DataInicio.get$().change()
    }
}

function permiteAdicionarNotaManualmente() {
    return _CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente;
}

function loadMontagemCargaMapa() {
    buscarConfiguracoesMontagemCarga(function () {
        buscarDetalhesOperador(function () {
            carregarConteudosHTML(function () {
                $("#wid-id-4").css('height', (window.innerHeight - 110) + 'px');

                $("#divMapa").css('height', (window.innerHeight - 215) + 'px');

                $("#divBodyknoutAreaPedido").css('height', (window.innerHeight - 380) + 'px');

                var heigth = $(window).height();
                $("#divBodyCarregamentos").css('max-height', (heigth - 355) + 'px');
                $("#divBodyCarregamentos").css('height', (heigth - 355) + 'px');

                var width = window.innerWidth;
                $("#fldModalBloco").css('max-height', (heigth - 201) + 'px');
                $("#divModalBlocoContent").css('width', (width - 200) + 'px');

                _preFiltroMontagemCarga = new PreFiltroMontagemCarga();
                KoBindings(_preFiltroMontagemCarga, "knockoutSalvarPreFiltroMontagemCarga");

                _pesquisaMontegemCarga = new PesquisaMontegemCarga();
                KoBindings(_pesquisaMontegemCarga, "knoutPesquisaMontagemCarga");

                _pesquisaMontegemCargaCarregamentos = new PesquisaMontegemCargaCarregamentos();
                KoBindings(_pesquisaMontegemCargaCarregamentos, "knoutPesquisaMontagemCargaCarregamentos");

                loadSessaoRoteirizador();

                _objPesquisaMontagem = RetornarObjetoPesquisa(_pesquisaMontegemCarga);

                new BuscarClientes(_pesquisaMontegemCarga.Remetente);//
                new BuscarLocalidades(_pesquisaMontegemCarga.Origem);//
                new BuscarClientes(_pesquisaMontegemCarga.Destinatario);//
                new BuscarLocalidades(_pesquisaMontegemCarga.Destino);//
                new BuscarFilial(_pesquisaMontegemCarga.Filial);//
                new BuscarFilial(_pesquisaMontegemCarga.FilialVenda);
                new BuscarSessaoRoteirizador(_pesquisaMontegemCarga.SessaoRoteirizador, confirmAbrirSessaoRoteirizador);//
                new BuscarTransportadores(_pesquisaMontegemCarga.Empresa);//
                new BuscarTiposOperacao(_pesquisaMontegemCarga.TipoOperacao);//
                new BuscarGruposPessoas(_pesquisaMontegemCarga.GrupoPessoaRemetente);//
                //new BuscarEstados(_pesquisaMontegemCarga.EstadoDestino);
                //new BuscarEstados(_pesquisaMontegemCarga.EstadoOrigem);
                new BuscarEstados(_pesquisaMontegemCarga.EstadosDestino);//
                new BuscarEstados(_pesquisaMontegemCarga.EstadosOrigem);//
                new BuscarClientes(_pesquisaMontegemCarga.Expedidor);//
                new BuscarClientes(_pesquisaMontegemCarga.Recebedor);//
                new BuscarPedidoViagemNavio(_pesquisaMontegemCarga.PedidoViagemNavio);//
                new BuscarPaises(_pesquisaMontegemCarga.PaisDestino);//
                new BuscarDeposito(_pesquisaMontegemCarga.Deposito);//
                new BuscarRegioes(_pesquisaMontegemCarga.RegiaoDestino);
                new BuscarFuncionario(_pesquisaMontegemCarga.Vendedor);
                new BuscarFuncionario(_pesquisaMontegemCarga.Supervisor);
                new BuscarFuncionario(_pesquisaMontegemCarga.Gerente);

                //#4947
                new BuscarCanaisEntrega(_pesquisaMontegemCarga.CodigosCanalEntrega);//CanalEntrega);
                new BuscarGruposPessoas(_pesquisaMontegemCarga.GrupoPessoaDestinatario);
                //new BuscarCategoriaPessoa(_pesquisaMontegemCarga.CategoriaPessoa);

                //#4429
                //new BuscarTiposdeCarga(_pesquisaMontegemCarga.TipoDeCarga);
                new BuscarTiposdeCarga(_pesquisaMontegemCarga.CodigosTipoDeCarga);

                //#11779
                new BuscarLinhasSeparacao(_pesquisaMontegemCarga.CodigosLinhaSeparacao, null, null, _pesquisaMontegemCarga.Filial);
                new BuscarRestricaoEntrega(_pesquisaMontegemCarga.CodigosRestricoesEntrega);
                new BuscarGruposProdutos(_pesquisaMontegemCarga.CodigosGrupoProdutos);
                new BuscarCategoriaPessoa(_pesquisaMontegemCarga.CodigosCategoriaClientes);

                //#33212
                new BuscarProdutos(_pesquisaMontegemCarga.CodigosProdutos);

                new BuscarClientes(_pesquisaMontegemCargaCarregamentos.Destinatario);
                new BuscarProdutos(_pesquisaMontegemCargaCarregamentos.Produto);
                new BuscarEmpresa(_pesquisaMontegemCargaCarregamentos.Transportador);
                new BuscarTiposOperacao(_pesquisaMontegemCargaCarregamentos.TipoOperacao);

                new BuscarClientes(_pesquisaMontegemCarga.Tomador);
                new BuscarRotasFrete(_pesquisaMontegemCarga.CodigosRotaFrete);


                new BuscarTiposDetalhe(_pesquisaMontegemCarga.CodigoProcessamentoEspecial, null, null, EnumTipoTipoDetalhe.ProcessamentoEspecial);
                new BuscarTiposDetalhe(_pesquisaMontegemCarga.CodigoHorarioEntrega, null, null, EnumTipoTipoDetalhe.HorarioEntrega);
                new BuscarTiposDetalhe(_pesquisaMontegemCarga.CodigoZonaTransporte, null, null, EnumTipoTipoDetalhe.ZonaTransporte);
                new BuscarTiposDetalhe(_pesquisaMontegemCarga.CodigoDetalheEntrega, null, null, EnumTipoTipoDetalhe.DetalheEntrega);

                new BuscarFiltroPesquisa(_pesquisaMontegemCarga.PreFiltros, abrirPreFiltrosMontagemCarga, 1, null);

                //Inicializando Google Maps
                loadDirecoesGoogleMaps();
                loadCarregamento();
                loadPedidoProduto();
                loadPedidoProdutosCarregamentos();
                loadDetalhesPedidoProdutos();
                loadDetalhesPedido();
                loadDetalhesCarregamento();
                loadRoteirizadorCarregamento();
                loadSimulacao();
                loadImportacao();
                loadCapacidadeJanelaCarregamento();
                loadPeriodoCarregamento();

                if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente) {
                    var $tabMapa = $("#liMapa");

                    $tabMapa.show();
                    $tabMapa.on('shown.bs.tab', 'a', function () {
                        if (_centralizarMapaNosPontos != null) {
                            _map.fitBounds(_centralizarMapaNosPontos);
                            _map.panToBounds(_centralizarMapaNosPontos);
                            _centralizarMapaNosPontos = null;
                        }
                    });
                    $(window).one('hashchange', function (e) {
                        $tabMapa.off('shown.bs.tab', 'a');
                    });
                }

                if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao !== EnumTipoMontagemCarga.Todos) {
                    _carregamento.TipoMontagemCarga.val(_CONFIGURACAO_TMS.TipoMontagemCargaPadrao);
                    _carregamento.TipoMontagemCarga.def = _CONFIGURACAO_TMS.TipoMontagemCargaPadrao;
                    _carregamento.TipoMontagemCarga.visible(false);
                    buscarInformacoesTipoMontagem();
                }

                _pesquisaMontegemCarga.ChaveNotaFiscalEletronica.visible(_CONFIGURACAO_TMS.MontagemCarga.AtivarMontagemCargaPorNFe);

                _pesquisaMontegemCarga.ChaveNotaFiscalEletronica.val.subscribe(function (chaveNFe) {
                    if (_pesquisaMontegemCarga.RequisicaoEnviadaMontagemCargaMapa.val() || !chaveNFe || chaveNFe.trim() === "" || chaveNFe.length < 44) return;

                    if (!ValidarChaveAcesso(chaveNFe))
                        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.MontagemCargaMapa.ChaveNotaFiscalEletronicaInvalida);

                    BuscarPedidoPorChaveNotaFiscalEletronica(chaveNFe);
                });

                if (_CONFIGURACAO_TMS.TipoFiltroDataMontagemCarga == EnumTipoFiltroDataMontagemCarga.CarregamentoPedido) {
                    _pesquisaMontegemCarga.DataInicio.text(Localization.Resources.Cargas.MontagemCargaMapa.DataColetaDe.getFieldDescription());
                    _pesquisaMontegemCarga.DataFim.text(Localization.Resources.Cargas.MontagemCargaMapa.DataColetaAte.getFieldDescription());
                } else if (_CONFIGURACAO_TMS.TipoFiltroDataMontagemCarga == EnumTipoFiltroDataMontagemCarga.PrevisaoSaida) {
                    _pesquisaMontegemCarga.DataInicio.text(Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisaoSaidaDe.getFieldDescription());
                    _pesquisaMontegemCarga.DataFim.text(Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisaoSaidaAte.getFieldDescription());
                }

                VisibilidadeCampos();

                DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS = this['DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS_WINDOW'];
                this['DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS_WINDOW'] = {};

                var telaGestaoPedidos = !!(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS != null && DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Filial != null && DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Filial.Codigo);
                modalPesquisar(telaGestaoPedidos);
            });
        });
    });
}

function VisibilidadeCampos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaMontegemCarga.Empresa.text(Localization.Resources.Cargas.MontagemCargaMapa.EmpresaBarraFilial.getFieldDescription());
        _pesquisaMontegemCarga.CodigoCargaEmbarcador.cssClass("col col-xs-6 col-sm-6 col-md-3 col-lg-3");
        _pesquisaMontegemCarga.NumeroPedido.cssClass("col col-xs-6 col-sm-6 col-md-3 col-lg-3");
        _pesquisaMontegemCarga.CodigoPedidoEmbarcadorDe.visible(false);
        _pesquisaMontegemCarga.CodigoPedidoEmbarcadorAte.visible(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaMontegemCarga.NumeroPedido.visible(false);
        _pesquisaMontegemCarga.CodigoCargaEmbarcador.visible(false);
        _pesquisaMontegemCargaCarregamentos.NumeroPedido.visible(false);
        _pesquisaMontegemCargaCarregamentos.Destinatario.cssClass("col col-xs-12 col-md-5");
        _pesquisaMontegemCargaCarregamentos.Produto.cssClass("col col-xs-12 col-md-5");

        if (_sessaoRoteirizador != null) {

            var produto = _sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val();
            _pesquisaMontegemCargaCarregamentos.Produto.visible(produto);

            if (produto) {
                $("#liDetalhePedidoProdutosCarregamentos").css('display', 'block');
                _pesquisaMontegemCargaCarregamentos.Destinatario.cssClass("col col-xs-12 col-md-4");
                _pesquisaMontegemCargaCarregamentos.Produto.cssClass("col col-xs-12 col-md-4");
                _pesquisaMontegemCargaCarregamentos.CodigoPedidoEmbarcador.cssClass("col col-xs-12 col-md-2");
                _pesquisaMontegemCargaCarregamentos.NumeroCarregamento.cssClass("col col-xs-12 col-md-2");
            } else {
                $("#liDetalhePedidoProdutosCarregamentos").css('display', 'none');
                _pesquisaMontegemCargaCarregamentos.Destinatario.cssClass("col col-xs-12 col-md-6");
                _pesquisaMontegemCargaCarregamentos.CodigoPedidoEmbarcador.cssClass("col col-xs-12 col-md-3");
                _pesquisaMontegemCargaCarregamentos.NumeroCarregamento.cssClass("col col-xs-12 col-md-3");
            }

        }

    }

    _pesquisaMontegemCarga.GerarCargasDeRedespacho.visible(!_CONFIGURACAO_TMS.NaoGerarCarregamentoRedespacho);
}

/*      NENHUM = 0,
        CRIAR_NOVA = 1,
        ABRIR_SESSAO = 2,
        ADD_PEDIDOS_SESSAO = 3
*/
function BuscarDadosMontagemCarga(opcao, callback) {
    if (opcao > 0) {
        _pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val(opcao);
        _objPesquisaMontagem = RetornarObjetoPesquisa(_pesquisaMontegemCarga);
        _gerarCargasDeColeta = _pesquisaMontegemCarga.GerarCargasDeColeta.val();

        var valido = true;
        if (!ValidarCamposObrigatorios(_pesquisaMontegemCarga))
            valido = false;

        if (valido) {
            BuscarPedidos(function () {
                ObterPontosPedidos();
                PesquisarPedidos();
                PesquisarCarregamentos();
                setConfiguracaoColeta();
                //Vamos pesquisar em locais, os tipos MicroRegiãoRoterizacao
                if (_pesquisaMontegemCarga.ExibirMicroRegioesRoteirizacao.val()) {
                    //loadMicroRegiaoRoteirizacao();
                    //loadPontosDeApoio();
                    //loadBalancas();
                    loadLogisticaLocais();
                    loadPracasPedagio();
                } else {
                    clearMicroRegioes();
                    clearMarkerPontosDeApoio();
                    clearMarkerBalancas();
                    clearMarkerPracasPedagio();
                }
                BuscarPedidosInconsistentes();
                //Prolema quando não encontra nenhum pedido.. congela a tela.. não achei aonde gera esse elemento "modal-backdrop"
                $('.modal-backdrop').css('display', 'none');
                //validaLinhaSeparacaoNaoAjustadoAgrupamentos();
                Global.fecharModal('divModalFiltroPesquisa');
                zoomIn();

                DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS = {
                    Pedidos: [],
                    CodigosAgrupadores: [],
                    Filial: null,
                    DataInicio: null,
                    DataFim: null,
                    SessaoRoteirizador: 0
                };
                if (callback) callback();
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            if (callback) callback();
        }
    }
}

function abrirPreFiltrosMontagemCarga(data) {
    if (data) {
        _preFiltroMontagemCarga.CodigoFiltro.val(data.Codigo);
        _preFiltroMontagemCarga.NomeFiltro.val(data.NomeFiltro);
        PreencherJsonFiltroPesquisa(_pesquisaMontegemCarga, data.Dados);
        //
        var dataIni = _pesquisaMontegemCarga.DataInicio.val();
        var dataFim = _pesquisaMontegemCarga.DataFim.val();
        if (dataIni != '' && dataFim != '') {
            // Dt inicial (D-15) e Dt Final (D+2)
            dataIni = moment().add(-15, 'days').format("DD/MM/YYYY");
            dataFim = moment().add(2, 'days').format("DD/MM/YYYY");
            _pesquisaMontegemCarga.DataInicio.val(dataIni);
            _pesquisaMontegemCarga.DataFim.val(dataFim);
        }
    }
}

function SalvarPreFiltrosMontagemCarga() {
    Global.abrirModal('modalSalvarPreFiltroMontagemCarga');
}

function limparPreFiltroMontagemCargaClick() {
    _preFiltroMontagemCarga.CodigoFiltro.val(0);
    _preFiltroMontagemCarga.NomeFiltro.val('');
}

function excluirPreFiltroMontagemCargaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaExcluirFiltroDePesquisa, function () {
        var data = { CodigoFiltro: _preFiltroMontagemCarga.CodigoFiltro.val() };
        executarReST("SessaoRoteirizador/ExcluirPreFiltrosMontagemCarregamento", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.FiltroDePesquisaExcluidoComSucesso);
                limparTudo();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    });
}

function salvarPreFiltroMontagemCargaClick() {

    if (!ValidarCamposObrigatorios(_preFiltroMontagemCarga)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    var clone = _pesquisaMontegemCarga;
    clone.SessaoRoteirizador.val(0);
    clone.OpcaoSessaoRoteirizador.val(1)// NOVA SESSÃO.

    var dados = {
        CodigoFiltro: _preFiltroMontagemCarga.CodigoFiltro.val(),
        NomeFiltro: _preFiltroMontagemCarga.NomeFiltro.val(),
        FiltroPesquisa: RetornarJsonFiltroPesquisa(clone),
        TipoFiltro: 1 // MontagemCarregamento
    }

    executarReST("SessaoRoteirizador/SalvarPreFiltrosMontagemCarregamento", dados, function (arg) {
        if (arg.Success) {
            Global.fecharModal("modalSalvarPreFiltroMontagemCarga");
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.FiltrosSalvoComSucesso);
            limparPreFiltroMontagemCarga();
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
    });

}

function limparFiltrosCarregamentos() {
    _pesquisaMontegemCargaCarregamentos.Destinatario.codEntity(0);
    _pesquisaMontegemCargaCarregamentos.Destinatario.val("");
    _pesquisaMontegemCargaCarregamentos.Produto.codEntity(0);
    _pesquisaMontegemCargaCarregamentos.Produto.val("");
    _pesquisaMontegemCargaCarregamentos.NumeroPedido.val("");
    _pesquisaMontegemCargaCarregamentos.CodigoPedidoEmbarcador.val("");
    _pesquisaMontegemCargaCarregamentos.NumeroCarregamento.val("");
    _pesquisaMontegemCarga.Destinatario.codEntity(0);
    _pesquisaMontegemCarga.Destinatario.val("");
    _pesquisaMontegemCarga.Produto.codEntity(0);
    _pesquisaMontegemCarga.Produto.val("");
    _pesquisaMontegemCarga.NumeroPedido.val("");
    _pesquisaMontegemCarga.NumeroCarregamento.val("");
    _pesquisaMontegemCargaCarregamentos.Transportador.codEntity(0);
    _pesquisaMontegemCargaCarregamentos.Transportador.val("");
    _pesquisaMontegemCargaCarregamentos.TipoOperacao.codEntity(0);
    _pesquisaMontegemCargaCarregamentos.TipoOperacao.val("");
}

function filtrarCarregamentos() {
    _pesquisaMontegemCarga.Destinatario.codEntity(_pesquisaMontegemCargaCarregamentos.Destinatario.codEntity());
    _pesquisaMontegemCarga.Destinatario.val(_pesquisaMontegemCargaCarregamentos.Destinatario.val());
    _pesquisaMontegemCarga.Produto.codEntity(_pesquisaMontegemCargaCarregamentos.Produto.codEntity());
    _pesquisaMontegemCarga.Produto.val(_pesquisaMontegemCargaCarregamentos.Produto.val());
    _pesquisaMontegemCarga.NumeroPedido.val(_pesquisaMontegemCargaCarregamentos.NumeroPedido.val());
    _pesquisaMontegemCarga.CodigoPedidoEmbarcador.val(_pesquisaMontegemCargaCarregamentos.CodigoPedidoEmbarcador.val());
    _pesquisaMontegemCarga.NumeroCarregamento.val(_pesquisaMontegemCargaCarregamentos.NumeroCarregamento.val());
    _pesquisaMontegemCarga.TipoOperacao.codEntity(_pesquisaMontegemCargaCarregamentos.TipoOperacao.codEntity());
    _pesquisaMontegemCarga.TipoOperacao.val(_pesquisaMontegemCargaCarregamentos.TipoOperacao.val());
    _pesquisaMontegemCarga.Empresa.codEntity(_pesquisaMontegemCargaCarregamentos.Transportador.codEntity());
    _pesquisaMontegemCarga.Empresa.val(_pesquisaMontegemCargaCarregamentos.Transportador.val());

    BuscarDadosMontagemCarga(2);
    Global.fecharModal('divModalFiltroPesquisaCarregamento');
}

function modalPesquisar(telaGestaoPedidos) {
    var codigo = 0;

    if (_pesquisaMontegemCarga) {
        codigo = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
    }

    if (codigo > 0) {
        $("#divRowSessaoAndamento").css('display', 'block');
    } else {
        $("#divRowSessaoAndamento").css('display', 'none');
    }

    if (telaGestaoPedidos) {
        _pesquisaMontegemCarga.Filial.val(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Filial.Descricao);
        _pesquisaMontegemCarga.Filial.codEntity(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.Filial.Codigo);
        _pesquisaMontegemCarga.DataInicio.val(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.DataInicio);
        _pesquisaMontegemCarga.DataFim.val(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.DataFim);

        if (DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.SessaoRoteirizador > 0) {
            limparTudo();
            consultarSessaoRoteirizador(DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS.SessaoRoteirizador, false); // ABRIR SESSÃO
        } else {
            BuscarDadosMontagemCarga(1); // NOVA SESSÃO
        }

    } else
        Global.abrirModal('divModalOpcoesSessao');
}

function novaSessaoRoteirizador() {
    limparTudo();
    Global.fecharModal('divModalOpcoesSessao');
    _pesquisaMontegemCarga.Filial.required(true);
    _pesquisaMontegemCarga.DataInicio.required(false);
    _pesquisaMontegemCarga.DataFim.required(false);
    _pesquisaMontegemCarga.Pesquisar.text(Localization.Resources.Cargas.MontagemCargaMapa.GerarNovaSessao);
    _pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val(1); //NOVA SESSAO
    enableCamposFiltroNovaSessao(true);
    Global.abrirModal('divModalFiltroPesquisa');
}

function abrirSessaoRoteirizador() {
    if (_pesquisaMontegemCarga) {
        $("#" + _pesquisaMontegemCarga.SessaoRoteirizador.idBtnSearch).trigger("click");
        _pesquisaMontegemCarga.ChaveNotaFiscalEletronica.required(false);
        _pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val(2); //ABRIR SESSAO        
        enableCamposFiltroNovaSessao(false);
    }
}

function enableCamposFiltroNovaSessao(enable) {
    if (_pesquisaMontegemCarga) {
        _pesquisaMontegemCarga.Filial.enable(enable);
        _pesquisaMontegemCarga.PedidosOrigemRecebedor.enable(enable);
        _pesquisaMontegemCarga.Expedidor.enable(enable);
    }
}

function confirmAbrirSessaoRoteirizador(row) {
    limparTudo();
    var codigo = parseInt(row.Codigo);
    consultarSessaoRoteirizador(codigo, false);
}

function addPedidosSessaoRoteirizador() {
    Global.fecharModal('divModalOpcoesSessao');
    _pesquisaMontegemCarga.Pesquisar.text(Localization.Resources.Gerais.Geral.Pesquisar);
    _pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val(3); //ADD PEDIDOS A SESSAO
    enableCamposFiltroNovaSessao(false);
    Global.abrirModal('divModalFiltroPesquisa');
}

function cancelarSessaoRoteirizador() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaCancelarSessaoDeRoteirizacao, function () {
        var data = { Codigo: _pesquisaMontegemCarga.SessaoRoteirizador.codEntity() };
        executarReST("SessaoRoteirizador/Cancelar", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.SessaoDeRoteirizacaoCanceladaComSucesso);
                limparTudo();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    });
}

function finalizarSessaoRoteirizador() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaFinalizarSessaoDeRoteirizacao, function () {
        var data = { Codigo: _pesquisaMontegemCarga.SessaoRoteirizador.codEntity() };
        executarReST("SessaoRoteirizador/Finalizar", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCargaMapa.SessaoDeRoteirizacaoFinalizadaComSucesso);
                limparTudo();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        });
    });
}

function limparTudo() {
    Global.fecharModal('divModalOpcoesSessao');
    _pesquisaMontegemCarga.SessaoRoteirizador.val(0);
    _pesquisaMontegemCarga.SessaoRoteirizador.codEntity(0);
    _pesquisaProtudosNaoAtendido.SessaoRoteirizador.val(0);
    _sessaoRoteirizador.Codigo.val(0);
    limparDadosCarregamento();
    clearPolylines();
    disposeDirection();
    PEDIDOS([]);
    PEDIDOS_INCONSISTENTES.removeAll();
    RenderizarGridPedidosInconsistentes();
    RemoverPontos();
    clearMicroRegioes();
    clearMarkerPontosDeApoio();
    _Carregamentos = [];
    limparAreaCarregamentos();
    if (_AreaCarregamento) {
        _AreaCarregamento.GerarCargaEmLote.visible(false);
        _AreaCarregamento.CancelarTodosCarregamentos.visible(false);
        _AreaCarregamento.Total.val(0);
        _AreaCarregamento.TotalCarregamentos.val(0);
        _AreaCarregamento.ExibirTodosCarregamentoMapa.val(true);
    }
    limparPreFiltroMontagemCarga();
    limparSimuladorFreteGrid();
    carregarTabelaPedidosSessao();
    reloadGridResumoCarregamentos();
}

function limparPreFiltroMontagemCarga() {
    if (_preFiltroMontagemCarga) {
        _preFiltroMontagemCarga.CodigoFiltro.val(0);
        _preFiltroMontagemCarga.NomeFiltro.val('');
    }
}

function mapaFocus() {
    setTimeout(function () {
        zoomIn();
    }, 500);
}

function BuscarPedidoPorChaveNotaFiscalEletronica(chaveNFe) {
    executarReST("MontagemCarga/ObterPedidoAtravesChaveNFe", { ChaveNotaFiscalEletronica: chaveNFe }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                let result = arg.Data;
                if (result !== null) {
                    _pesquisaMontegemCarga.Pedido.codEntity(result.CodigoPedido);
                    _pesquisaMontegemCarga.Pedido.val(result.CodigoPedido);
                    _pesquisaMontegemCarga.Filial.codEntity(result.CodigoFilial);
                    _pesquisaMontegemCarga.Filial.val(result.DescricaoFilial);
                    BuscarDadosMontagemCarga(_pesquisaMontegemCarga.OpcaoSessaoRoteirizador.val());
                }
            }
            else { exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg); }
        }
        else { exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg); }
    });
}

function buscarConfiguracoesMontagemCarga(callback) {
    executarReST("MontagemCarga/ObterConfiguracaoMontagemCarga", {}, function (response) {
        if (response.Success) {
            if (response.Data) {
                _configuracoesMontagemCarga = response.Data;
                if (callback instanceof Function)
                    callback();
            }
            else { exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg); }
        }
        else { exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg); }
    })
}