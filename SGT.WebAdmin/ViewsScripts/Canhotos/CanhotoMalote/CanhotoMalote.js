/// <reference path="../../Consultas/CTe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _knoutPesquisar;
var _gridCanhotos;

var _situacao = [
    { text: "Todos", value: EnumSituacaoCanhoto.Todas },
    { text: "Pendente", value: EnumSituacaoCanhoto.Pendente },
    { text: "Justificado", value: EnumSituacaoCanhoto.Justificado },
    { text: "Recebido Fisicamente", value: EnumSituacaoCanhoto.RecebidoFisicamente },
    { text: "Extraviado", value: EnumSituacaoCanhoto.Extraviado }
];

var _enumTipoCanhoto = [
    { text: "Todos", value: EnumTipoCanhoto.Todos },
    { text: "NF-e", value: EnumTipoCanhoto.NFe },
    { text: "Avulso", value: EnumTipoCanhoto.Avulso },
    { text: "CT-e para Subcontratação", value: EnumTipoCanhoto.CTeSubcontratacao },
    { text: "CT-e", value: EnumTipoCanhoto.CTe }
];


var PesquisaCanhoto = function () {
    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Numero = PropertyEntity({ text: "Número do Canhoto:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.DataInicio = PropertyEntity({ text: "Data Emissão Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Emissão Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto.Todos), options: _enumTipoCanhoto, def: EnumTipoCanhoto.Todos, text: "Tipo: ", visible: ko.observable(true) });
    this.SituacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoCanhoto.Pendente), options: _situacao, def: EnumSituacaoCanhoto.Pendente, text: "Situação: " });
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoDigitalizacaoCanhoto.Todas), options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoesPesquisa(), def: EnumSituacaoDigitalizacaoCanhoto.Todas, text: "Situação Digitalização: " });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Viagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigosCargaEmbarcador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Viagens:", val: ko.observable(""), def: "", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Emitente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Filial:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Transportador:", idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarClick, text: "Salvar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: cancelarClick, text: "Cancelar" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarCanhotos(true);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******
function loadCanhotoMalote() {
    _knoutPesquisar = new PesquisaCanhoto();
    KoBindings(_knoutPesquisar, "knockoutPesquisaCanhotos", false, _knoutPesquisar.Pesquisar.id);

    new BuscarMotorista(_knoutPesquisar.Motorista);
    new BuscarClientes(_knoutPesquisar.Emitente);
    new BuscarTransportadores(_knoutPesquisar.Transportador);
    new BuscarFilial(_knoutPesquisar.Filial, RetornoBuscaFilial);
    new BuscarCargas(_knoutPesquisar.CodigosCargaEmbarcador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _knoutPesquisar.Transportador.visible(false);
        _knoutPesquisar.Transportador.required = false;
    }

    loadMalotes();

    $.get("Content/Static/Canhotos/DetalheCanhoto.html?dyn=" + guid(), function (data) {
        $("#divDetalhesCanhoto").html(data.replace(/#KnoutDetalhesCanhoto/g, guid()));
        $("#KnoutDetalhesCanhoto > .nav").hide();
        loadDetalhesCanhoto();
    });

    BuscarCanhotos(false);
}

function RetornoBuscaFilial(filial) {
    PreencheCampoFilial(filial);
    BuscarCanhotos(false);
}

function PreencheCampoFilial(filial) {
    _knoutPesquisar.Filial.val(filial.Descricao);
    _knoutPesquisar.Filial.entityDescription(filial.Descricao);
    _knoutPesquisar.Filial.codEntity(filial.Codigo);
}

function BuscarCanhotos(validarObrigatorios) {


    _knoutPesquisar.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _knoutPesquisar.SelecionarTodos,
        callbackNaoSelecionado: function () { },
        callbackSelecionado: CanhotoSelecionado,
        callbackSelecionarTodos: function () { },
        somenteLeitura: false
    };

    var detalhes = { descricao: "Detalhes", id: guid(), tamanho: 9, metodo: detalhesCanhotoClick };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [detalhes]
    };

    _gridCanhotos = new GridView(_knoutPesquisar.Pesquisar.idGrid, "CanhotoMalote/Pesquisa", _knoutPesquisar, menuOpcoes, null, 10, null, null, null, multiplaescolha);

    if ((_knoutPesquisar.Transportador.codEntity() > 0 || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) && _knoutPesquisar.Filial.codEntity() > 0) {
        _knoutPesquisar.Salvar.visible(true);
        _gridCanhotos.CarregarGrid();
    } else {
        _knoutPesquisar.Salvar.visible(false);
        if (validarObrigatorios && !ValidarCamposObrigatorios(_knoutPesquisar)) {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informar Campos Obrigatórios");
        }
    }
}

function CanhotoSelecionado(e, data) {
    if (_knoutPesquisar.Filial.codEntity() == 0 && data.CodigoFilial != 0) {
        PreencheCampoFilial({ Codigo: data.CodigoFilial, Descricao: data.Filial });
        _gridCanhotos.CarregarGrid();
    }
}

function salvarClick() {
    if (_knoutPesquisar.Filial.codEntity() == 0)
        return exibirMensagem(tipoMensagem.aviso, "Filtros obrigatórios", "Informe uma Filial.");

    if (_knoutPesquisar.Transportador.codEntity() == 0 && _knoutPesquisar.Transportador.required == true)
        return exibirMensagem(tipoMensagem.aviso, "Filtros obrigatórios", "Informe um Transportador.");

    if (QuantidadeCanhotosSelecionados() == 0)
        return exibirMensagem(tipoMensagem.aviso, "Canhotos Selecionados", "Nenhum canhoto foi selecionado.");

    SalvarMalote();
}

function detalhesCanhotoClick(e) {
    BuscarDetalhesCanhoto(e.Codigo, function () {
        Global.abrirModal("ModalDivDetalhesCanhoto");
    });
}

function cancelarClick() {
    LimparCamposMalote();
}

function LimparCamposMalote() {
    LimparCampos(_knoutPesquisar);
    _knoutPesquisar.SelecionarTodos.val(false);
    BuscarCanhotos(false);
}