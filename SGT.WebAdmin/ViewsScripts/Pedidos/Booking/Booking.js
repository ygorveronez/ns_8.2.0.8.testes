/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridBooking;
var _booking;
var _planejamentoControle;
var _transporte;
var _CRUDBooking;
var _pesquisaBooking;
var _justificativaCancelamentoBooking;
var _PermissoesPersonalizadas;
var _estaDuplicandoBooking;
var _valorEnableProtocoloCarga;
var _outros;

var CadastroBooking = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("Pedido:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroExp = PropertyEntity({ text: "Número EXP: ", maxlength: 150, visible: ko.observable(true), required: true, enable: ko.observable(true), val: ko.observable(""), def: ko.observable("") });
    this.Importador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Importador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoEspecie = PropertyEntity({ text: "Código Especie: ", maxlength: 50, visible: true, required: false, enable: ko.observable(false) });
    this.DescricaoEspecie = PropertyEntity({ text: "Descrição Especie: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.CodigoNCM = PropertyEntity({ text: "NCM: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.Incoterm = PropertyEntity({ text: "Incoterm: ", maxlength: 200, visible: true, required: false, enable: ko.observable(true) });
    this.DescricaoIdentificacaoCarga = PropertyEntity({ text: "Descrição Identificação Carga: ", maxlength: 200, visible: true, required: false, enable: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo De Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoContratoFOB = PropertyEntity({ text: "Código Contrato FOB: ", maxlength: 150, visible: true, required: false, enable: ko.observable(true) });

    this.PlanejamentoControle = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Transporte = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Outros = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
};

var PlanejamentoControle = function () {
    this.Halal = PropertyEntity({ getType: typesKnockout.bool, text: "Halal", def: false, val: ko.observable(false), enable: ko.observable(true) });

    this.StatusEXP = PropertyEntity({ val: ko.observable(EnumStatusEXP.NaoDefinido), options: EnumStatusEXP.obterOpcoes(), def: EnumStatusEXP.NaoDefinido, text: "Status EXP: ", visible: ko.observable(true), enable: ko.observable(false) });
    this.DataPrevisaoEstufagem = PropertyEntity({ text: "Data Previsão Estufagem: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 800, visible: true, required: false, enable: ko.observable(true) });
}

var Transporte = function () {
    this.NumeroBooking = PropertyEntity({ text: "Número Booking: ", maxlength: 150, visible: true, required: false, enable: ko.observable(true) });
    this.CodigoIdentificacaoCarga = PropertyEntity({ text: "Código Identificação Carga: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.CodigoCarga = PropertyEntity({ text: "Codigo Carga Embarcador: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.ProtocoloCarga = PropertyEntity({ text: "Protocolo Carga: ", maxlength: 100, visible: true, required: false, enable: ko.observable(true) });
    this.MetragemCarga = PropertyEntity({ text: "Metragem Carga: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });

    this.Transbordo = PropertyEntity({ text: "Transbordo: ", maxlength: 200, visible: true, required: false, enable: ko.observable(true) });
    this.MensagemTransbordo = PropertyEntity({ text: "Mensagem Transbordo: ", maxlength: 200, visible: true, required: false, enable: ko.observable(true) });

    this.DataBooking = PropertyEntity({ text: "Data Booking: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataDeadLineCarga = PropertyEntity({ text: "Data Dead Line Carga:", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataDeadLineDraf = PropertyEntity({ text: "Data Dead Line Draf:", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.Despachante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Despachante:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataETAOrigem = PropertyEntity({ text: "Data ETA Origem: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataETAOrigemFinal = PropertyEntity({ text: "Data ETA Origem Final: ", getType: typesKnockout.dateTime, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataETASegundoDestino = PropertyEntity({ text: "Data ETA Segundo Destino: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataETATransbordo = PropertyEntity({ text: "Data ETA Transbordo: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });

    this.DataETS = PropertyEntity({ text: "Data ETS: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataETSTransbordo = PropertyEntity({ text: "Data ETS Transbordo: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });

    this.MoedaCapatazia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Moeda Capatazia: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ValorCapatazia = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Valor Capatazia:", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorFrete = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Valor Frete:", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.FretePrepaid = PropertyEntity({ val: ko.observable(EnumFretePrepaid.Collect), options: EnumFretePrepaid.ObterOpcoes(), def: EnumFretePrepaid.Collect, text: "Frete Prepaid: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Origem:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DescricaoPaisPortoCarregamento = PropertyEntity({ text: "País Origem: ", maxlength: 50, visible: true, required: false, enable: ko.observable(false) });
    this.SiglaPaisPortoCarregamento = PropertyEntity({ text: "Sigla País: ", maxlength: 50, visible: true, required: false, enable: ko.observable(false) });

    this.CodigoPortoCarregamentoTransbordo = PropertyEntity({ text: "Código Porto Carregamento: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.DescricaoPortoCarregamentoTransbordo = PropertyEntity({ text: "Descrição Porto Carregamento: ", maxlength: 150, visible: true, required: false, enable: ko.observable(true) });

    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Destino:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.DescricaoPaisPortoDestino = PropertyEntity({ text: "País Destino: ", maxlength: 50, visible: true, required: false, enable: ko.observable(false) });
    this.SiglaPaisPortoDestinoTransbordo = PropertyEntity({ text: "Sigla País: ", maxlength: 50, visible: true, required: false, enable: ko.observable(false) });

    this.NavioTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Navio Transbordo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Navio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Navio:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModoTransporte = PropertyEntity({ text: "Modo Transporte: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });

    this.TipoTransporte = PropertyEntity({ val: ko.observable(EnumTipoTransporteDadosMaritimos.NaoDefinido), options: EnumTipoTransporteDadosMaritimos.obterOpcoes(), def: EnumTipoTransporteDadosMaritimos.NaoDefinido, text: "Tipo Trasnporte: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.NumeroLacre = PropertyEntity({ text: "Nº Lacre: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.NumeroViagem = PropertyEntity({ text: "Nº Viagem: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.NumeroViagemTransbordo = PropertyEntity({ text: "Nº Viagem Transbordo: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });

    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Temperatura = PropertyEntity({ text: "Temperatura: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.TipoInLand = PropertyEntity({ val: ko.observable(EnumTipoInland.NaoDefinido), options: EnumTipoInland.obterOpcoes(), def: EnumTipoInland.NaoDefinido, text: "Tipo Inland: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.Armador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Armador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Container:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo Container:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ViaTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Via Transporte:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoProbe = PropertyEntity({ val: ko.observable(EnumTipoProbe.NaoDefinido), options: EnumTipoProbe.obterOpcoes(), def: EnumTipoProbe.NaoDefinido, text: "Tipo Probe: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoEnvio = PropertyEntity({ val: ko.observable(EnumTipoEnvioTransporteMaritimo.TON), options: EnumTipoEnvioTransporteMaritimo.obterOpcoes(), def: EnumTipoEnvioTransporteMaritimo.TON, text: "Tipo Envio: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.CargaPaletizada = PropertyEntity({ getType: typesKnockout.bool, text: "Carga Paletizada", def: false, val: ko.observable(false), enable: ko.observable(true) });
    this.PossuiGenset = PropertyEntity({ getType: typesKnockout.bool, text: "Possui Genset", def: false, val: ko.observable(false), enable: ko.observable(true) });
    this.JustificativaCancelamento = PropertyEntity({ text: "Justificativa Cancelamento: ", maxlength: 400, visible: ko.observable(false), required: false, enable: ko.observable(false) });

    this.ETSPermiteEditar = PropertyEntity({ getType: typesKnockout.bool, def: true, val: ko.observable(true) });
    this.RegistroAguardandoRetorno = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });
}

var Outros = function () {
    this.DataDeadLinePedido = PropertyEntity({ text: "Data Dead Line Pedido:", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.SegundaDataDeadLineCarga = PropertyEntity({ text: "Segunda Data Dead Line Carga:", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.SegundaDataDeadLineDraf = PropertyEntity({ text: "Segunda Data Dead Line Draf:", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataETASegundaOrigem = PropertyEntity({ text: "Data ETA Segunda Origem: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataRetiradaContainerDestino = PropertyEntity({ text: "Data Retirada Container Destino: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataETADestinoFinal = PropertyEntity({ text: "Data ETA Destino Final: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoRota = PropertyEntity({ text: "Código Rota: ", maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.DataETADestino = PropertyEntity({ text: "Data ETA Destino: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataDepositoContainer = PropertyEntity({ text: "Data Depósito Container: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataRetiradaContainer = PropertyEntity({ text: "Data Retirada Container: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataRetiradaVazio = PropertyEntity({ text: "Data Retirada Vazio: ", getType: typesKnockout.dateTime, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataCarregamentoPedido = PropertyEntity({ text: "Data Carregamento Pedido: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data Previsão Entrega: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataConhecimento = PropertyEntity({ text: "Data Embarque: ", getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroBL = PropertyEntity({ text: "Nº BL: ", maxlength: 200, visible: true, required: false, enable: ko.observable(true) });
}

var PesquisaBooking = function () {
    this.NumeroEXP = PropertyEntity({ text: "Número EXP: ", getType: typesKnockout.string });
    this.NumeroBooking = PropertyEntity({ text: "Número Booking: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoOcorrencia.Todas), options: EnumStatusControleMaritimo.obterOpcoesPesquisa(), def: EnumStatusControleMaritimo.Todas, text: "Situação: " });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Fim: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBooking.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CRUDBooking = function () {
    //CRUD
    //this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: ko.observable("Atualizar"), enable: ko.observable(true), visible: ko.observable(false) });
    //this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar Atualizações",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "PedidoDadosTransporteMaritimo/Importar",
        UrlConfiguracao: "PedidoDadosTransporteMaritimo/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O052_ImportacaoBooking,
        CallbackImportacao: function () {
            _gridBooking.CarregarGrid();
        }
    });
}

var JustificativaCancelamentoBooking = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Justificativa = PropertyEntity({ text: "*Justificativa: ", maxlength: 400, visible: true, required: true, enable: ko.observable(true) });

    this.Cancelar = PropertyEntity({
        eventClick: function (e) {
            Global.fecharModal("divModalCancelarBooking");
        }, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true)
    });
    this.Confirmar = PropertyEntity({
        eventClick: function (e) {

            cancelarBookingClick(e);
        }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function loadBooking() {
    _booking = new CadastroBooking();
    KoBindings(_booking, "knockoutCadastroBooking");

    _planejamentoControle = new PlanejamentoControle();
    KoBindings(_planejamentoControle, "knockoutPlanejamentoControle");

    _transporte = new Transporte();
    KoBindings(_transporte, "knockoutTransporte");

    _CRUDBooking = new CRUDBooking();
    KoBindings(_CRUDBooking, "knockoutCRUDBooking")

    HeaderAuditoria("Booking", _booking);

    _pesquisaBooking = new PesquisaBooking();
    KoBindings(_pesquisaBooking, "knockoutPesquisaBooking", false, _pesquisaBooking.Pesquisar.id);

    _justificativaCancelamentoBooking = new JustificativaCancelamentoBooking();
    KoBindings(_justificativaCancelamentoBooking, "knockoutJustificativaCancelamentoBooking");

    _outros = new Outros();
    KoBindings(_outros, "knockoutOutros");

    // Inicia as buscas
    new BuscarFilial(_pesquisaBooking.Filial);
    new BuscarLocalidades(_pesquisaBooking.Origem);
    new BuscarLocalidades(_pesquisaBooking.Destino);

    new BuscarFilial(_booking.Filial);

    new BuscarTipoDeCargaDoPedido(_booking.TipoCarga);
    new BuscarPedidos(_booking.Pedido, buscarTipoContainerPorModeloVeicular);

    new BuscarClientes(_booking.Importador);
    new BuscarClientes(_transporte.Despachante);

    new BuscarClientes(_planejamentoControle.Remetente);

    new BuscarViaTransporte(_transporte.ViaTransporte);
    new BuscarNavios(_transporte.Navio);
    new BuscarNavios(_transporte.NavioTransbordo);
    new BuscarClientes(_transporte.Armador);
    new BuscarMoedas(_transporte.MoedaCapatazia, function (r) {
        if (r != null) {
            _transporte.MoedaCapatazia.codEntity(r.Codigo);
            _transporte.MoedaCapatazia.val(r.Sigla);
        }
    });
    new BuscarContainers(_transporte.Container)
    new BuscarTiposContainer(_transporte.TipoContainer);

    new BuscarClientes(_transporte.TerminalOrigem);

    new BuscarClientes(_transporte.PortoOrigem, function (r) {
        if (r != null) {
            _transporte.PortoOrigem.codEntity(r.Codigo);
            _transporte.PortoOrigem.val(r.Descricao);
            _transporte.DescricaoPaisPortoCarregamento.val(r.DescricaoPais);
            _transporte.SiglaPaisPortoCarregamento.val(r.AbreviacaoPais);
        }
    });
    new BuscarClientes(_transporte.PortoDestino, function (r) {
        if (r != null) {
            _transporte.PortoDestino.codEntity(r.Codigo);
            _transporte.PortoDestino.val(r.Descricao);
            _transporte.DescricaoPaisPortoDestino.val(r.DescricaoPais);
            _transporte.SiglaPaisPortoDestinoTransbordo.val(r.AbreviacaoPais);
        }
    });

    LoadGrid();
}


function atualizarClick(e, sender) {
    var valido = ValidarCamposObrigatoriosBooking();
    if (valido) {
        if (_estaDuplicandoBooking) {
            exibirConfirmacao("Duplicar Booking", "Tem certeza que deseja duplicar este registro de booking? O pedido selecionado ficará vinculado a este novo booking", function () {
                preencherListas();
                Salvar(_booking, "PedidoDadosTransporteMaritimo/DuplicarBookingCancelado", function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Booking duplicado com sucesso");
                            _gridBooking.CarregarGrid();
                            limparCampos();
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                }, sender);
            });
        } else {
            preencherListas();
            Salvar(_booking, "PedidoDadosTransporteMaritimo/Atualizar", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                        _gridBooking.CarregarGrid();
                        limparCampos();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, sender);
        }
    }
}

function cancelarBookingClick(e, sender) {
    Salvar(e, "PedidoDadosTransporteMaritimo/CancelarBooking", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal("divModalCancelarBooking");
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Booking cancelado com sucesso");
                _gridBooking.CarregarGrid();
                limparCampos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCampos();
}

// ******** METODOS ********** /

function LoadGrid() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({ descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBooking, tamanho: "15", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Cancelar Booking", id: "clasCancelar", evento: "onclick", metodo: cancelarBooking, tamanho: "15", icone: "", visibilidade: visibilidadeCancelarBooking });
    menuOpcoes.opcoes.push({ descricao: "Duplicar", id: "classDuplicar", evento: "onclick", metodo: duplicarCancelado, tamanho: "15", icone: "", visibilidade: visibilidadeDuplicarcancelado });

    var configExportacao = {
        url: "PedidoDadosTransporteMaritimo/ExportarPesquisa",
        titulo: "Booking"
    };

    _gridBooking = new GridViewExportacao(_pesquisaBooking.Pesquisar.idGrid, "PedidoDadosTransporteMaritimo/Pesquisa", _pesquisaBooking, menuOpcoes, configExportacao);
    _gridBooking.CarregarGrid();
}

function editarBooking(booking) {
    limparCampos();
    _booking.Codigo.val(booking.Codigo);
    _estaDuplicandoBooking = false;
    $("#divBookingAguardandoRetorno").hide();

    BuscarPorCodigo(_booking, "PedidoDadosTransporteMaritimo/BuscarPorCodigo", function (arg) {

        if (arg.Data.PlanejamentoControle != null)
            PreencherObjetoKnout(_planejamentoControle, { Data: arg.Data.PlanejamentoControle });

        if (arg.Data.Transporte != null)
            PreencherObjetoKnout(_transporte, { Data: arg.Data.Transporte });

        if (arg.Data.Outros != null)
            PreencherObjetoKnout(_outros, { Data: arg.Data.Outros });

        $("BookingAguardandoRetorno").hide();
        ControlarCamposEdicao(false);
        _pesquisaBooking.ExibirFiltros.visibleFade(false);
        _CRUDBooking.Atualizar.text("Atualizar");
        _CRUDBooking.Atualizar.visible(true);
        _CRUDBooking.Cancelar.visible(true);
        //_booking.Adicionar.visible(false);

        if (!_transporte.ETSPermiteEditar.val()) {
            _CRUDBooking.Atualizar.visible(false);
        }

        if (_transporte.RegistroAguardandoRetorno.val()) {
            _CRUDBooking.Atualizar.visible(false);
            $("#divBookingAguardandoRetorno").show();
        }

        if (arg.Data.Situacao == EnumStatusControleMaritimo.Cancelado) {
            _transporte.JustificativaCancelamento.visible(true);
            _CRUDBooking.Atualizar.visible(false);
        }
    }, null);
}

function cancelarBooking(booking) {
    limparCampos();
    _justificativaCancelamentoBooking.Codigo.val(booking.Codigo);
    Global.abrirModal('divModalCancelarBooking');
}

function duplicarCancelado(booking) {
    limparCampos();
    _booking.Codigo.val(booking.Codigo);
    _estaDuplicandoBooking = true;
    BuscarPorCodigo(_booking, "PedidoDadosTransporteMaritimo/BuscarPorCodigo", function (arg) {

        ControlarCamposEdicao(false);
        _pesquisaBooking.ExibirFiltros.visibleFade(false);
        _CRUDBooking.Atualizar.text("Duplicar");
        _CRUDBooking.Atualizar.enable(true);
        _CRUDBooking.Atualizar.visible(true);
        _CRUDBooking.Cancelar.visible(true);
        //_booking.Adicionar.visible(false);

    }, null);

}

function limparCampos() {
    _CRUDBooking.Atualizar.text("Atualizar");
    _CRUDBooking.Atualizar.visible(false);
    _CRUDBooking.Cancelar.visible(false);
    //_booking.Adicionar.visible(true);

    ControlarCamposEdicao(true);
    LimparCampos(_booking);
    LimparCampos(_transporte);
    LimparCampos(_outros);
    LimparCampos(_planejamentoControle);
    $("#divBookingAguardandoRetorno").hide();
    Global.ResetarAbas();
}

function ControlarCamposEdicao(valor) {
    _booking.Filial.enable(valor);
    _booking.Pedido.enable(valor);
    _booking.NumeroExp.enable(valor);
    _booking.Importador.enable(valor);
    _booking.DescricaoEspecie.enable(valor);
    _booking.CodigoNCM.enable(valor);
    _booking.Incoterm.enable(valor);
    _booking.DescricaoIdentificacaoCarga.enable(valor);
    _booking.TipoCarga.enable(valor);

    _planejamentoControle.Halal.enable(valor);

    _planejamentoControle.DataPrevisaoEstufagem.enable(valor);
    _planejamentoControle.Remetente.enable(valor);
    _planejamentoControle.Observacao.enable(valor);

    _transporte.CodigoCarga.enable(valor);
    _transporte.MetragemCarga.enable(valor);
    _transporte.CargaPaletizada.enable(valor);
    _transporte.Temperatura.enable(valor);
    _transporte.FretePrepaid.enable(valor);
    _transporte.CodigoIdentificacaoCarga.enable(valor);
    _transporte.TipoTransporte.enable(valor);
    _transporte.ProtocoloCarga.enable(valor);

    if (valor)
        _transporte.JustificativaCancelamento.visible(false);
}

function visibilidadeCancelarBooking(row) {

    if (row.Status != EnumStatusControleMaritimo.Cancelado) {
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Booking_PermitirCancelarBooking, _PermissoesPersonalizadas))
            return true;
    }

    return false;
}

function visibilidadeDuplicarcancelado(row) {
    return (row.Status == EnumStatusControleMaritimo.Cancelado);
}

function preencherListas() {
    _booking.PlanejamentoControle.val(JSON.stringify(RetornarObjetoPesquisa(_planejamentoControle)));
    _booking.Transporte.val(JSON.stringify(RetornarObjetoPesquisa(_transporte)));
    _booking.Outros.val(JSON.stringify(RetornarObjetoPesquisa(_outros)));

}

function ValidarCamposObrigatoriosBooking() {
    if (!ValidarCamposObrigatorios(_transporte)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Necessário informar campos obrigatórios.");
        $("#liTabTransporte a").tab("show");
        return false;
    } else {
        return true;
    }
}

function buscarTipoContainerPorModeloVeicular(retorno) {
    executarReST("PedidoDadosTransporteMaritimo/ObterTipoContainerPorModeloVeicular", { Codigo: retorno.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== null) {
                if (arg.Data.TipoContainer.Codigo !== 0) {
                    _transporte.TipoContainer.val(arg.Data.TipoContainer.Descricao);
                    _transporte.TipoContainer.codEntity(arg.Data.TipoContainer.Codigo);
                    _transporte.TipoContainer.enable(false);
                } else {
                    _transporte.TipoContainer.enable(true);
                }
            }
        }
    });
}