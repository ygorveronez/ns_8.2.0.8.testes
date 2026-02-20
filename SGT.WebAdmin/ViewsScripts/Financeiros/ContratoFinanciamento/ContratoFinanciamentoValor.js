/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="ContratoFinanciamento.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoValor, _gridContratoFinanciamentoValor;

var _tipoJustificativaContratoFinanciamentoValor = [
    { value: EnumTipoJustificativa.Desconto, text: "Desconto" },
    { value: EnumTipoJustificativa.Acrescimo, text: "Acréscimo" }
];

var ContratoFinanciamentoValor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoTipoMovimento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoTipo = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Grid = PropertyEntity({ type: types.local });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500, enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true), enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(_tipoJustificativaContratoFinanciamentoValor.Desconto), options: _tipoJustificativaContratoFinanciamentoValor, def: _tipoJustificativaContratoFinanciamentoValor.Desconto, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarValorContratoFinanciamentoClick, type: types.event, text: "Adicionar", enable: ko.observable(true), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarValorContratoFinanciamentoClick, type: types.event, text: "Atualizar", enable: ko.observable(true), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirValorContratoFinanciamentoClick, type: types.event, text: "Excluir", enable: ko.observable(true), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposContratoFinanciamentoValor, type: types.event, text: "Cancelar", enable: ko.observable(true), visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadContratoFinanciamentoValor() {
    _contratoFinanciamentoValor = new ContratoFinanciamentoValor();
    KoBindings(_contratoFinanciamentoValor, "knockoutValorContratoFinanciamento");

    new BuscarTipoMovimento(_contratoFinanciamentoValor.TipoMovimento);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarValorContratoFinanciamentoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoMovimento", visible: false },
        { data: "Tipo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%" },
        { data: "TipoMovimento", title: "Tipo Movimento", width: "20%" },
        { data: "DescricaoTipo", title: "Tipo", width: "10%" },
        { data: "Valor", title: "Valor", width: "10%" }
    ];

    _gridContratoFinanciamentoValor = new BasicDataTable(_contratoFinanciamentoValor.Grid.id, header, menuOpcoes, { column: 3, dir: orderDir.asc });

    RecarregarGridValorContratoFinanciamento();
}

//*******MÉTODOS*******

function RecarregarGridValorContratoFinanciamento() {
    var data = new Array();

    $.each(_contratoFinanciamento.Valores.list, function (i, valor) {
        var valorGrid = new Object();

        valorGrid.Codigo = valor.Codigo.val;
        valorGrid.CodigoTipoMovimento = valor.CodigoTipoMovimento.val;
        valorGrid.DescricaoTipo = valor.DescricaoTipo.val;
        valorGrid.Descricao = valor.Descricao.val;
        valorGrid.TipoMovimento = valor.TipoMovimento.val;
        valorGrid.Tipo = valor.Tipo.val;
        valorGrid.Valor = valor.Valor.val;

        data.push(valorGrid);
    });

    _gridContratoFinanciamentoValor.CarregarGrid(data);
}

function EditarValorContratoFinanciamentoClick(data) {
    for (var i = 0; i < _contratoFinanciamento.Valores.list.length; i++) {
        if (data.Codigo == _contratoFinanciamento.Valores.list[i].Codigo.val) {
            var valor = _contratoFinanciamento.Valores.list[i];

            _contratoFinanciamentoValor.Codigo.val(valor.Codigo.val);
            _contratoFinanciamentoValor.TipoMovimento.codEntity(valor.CodigoTipoMovimento.val);
            _contratoFinanciamentoValor.Descricao.val(valor.Descricao.val);
            _contratoFinanciamentoValor.TipoMovimento.val(valor.TipoMovimento.val);
            _contratoFinanciamentoValor.Tipo.val(valor.Tipo.val);
            _contratoFinanciamentoValor.Valor.val(valor.Valor.val);

            _contratoFinanciamentoValor.Adicionar.visible(false);
            _contratoFinanciamentoValor.Atualizar.visible(true);
            _contratoFinanciamentoValor.Excluir.visible(true);
            _contratoFinanciamentoValor.Cancelar.visible(true);
        }
    }
}

function ExcluirValorContratoFinanciamentoClick() {
    for (var i = 0; i < _contratoFinanciamento.Valores.list.length; i++) {
        if (_contratoFinanciamentoValor.Codigo.val() == _contratoFinanciamento.Valores.list[i].Codigo.val) {
            _contratoFinanciamento.Valores.list.splice(i, 1);
            break;
        }
    }

    LimparCamposContratoFinanciamentoValor();
}

function AdicionarValorContratoFinanciamentoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_contratoFinanciamentoValor);

    if (valido) {
        _contratoFinanciamentoValor.Codigo.val(guid());
        _contratoFinanciamentoValor.CodigoTipoMovimento.val(_contratoFinanciamentoValor.TipoMovimento.codEntity());
        _contratoFinanciamentoValor.DescricaoTipo.val($('#' + _contratoFinanciamentoValor.Tipo.id + ' option:selected').text());
        _contratoFinanciamento.Valores.list.push(SalvarListEntity(_contratoFinanciamentoValor));

        LimparCamposContratoFinanciamentoValor();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarValorContratoFinanciamentoClick() {
    var valido = ValidarCamposObrigatorios(_contratoFinanciamentoValor);

    if (valido) {
        for (var i = 0; i < _contratoFinanciamento.Valores.list.length; i++) {
            if (_contratoFinanciamentoValor.Codigo.val() == _contratoFinanciamento.Valores.list[i].Codigo.val) {
                _contratoFinanciamentoValor.CodigoTipoMovimento.val(_contratoFinanciamentoValor.TipoMovimento.codEntity());
                _contratoFinanciamentoValor.DescricaoTipo.val($('#' + _contratoFinanciamentoValor.Tipo.id + ' option:selected').text());
                _contratoFinanciamento.Valores.list[i] = SalvarListEntity(_contratoFinanciamentoValor);
                break;
            }
        }

        LimparCamposContratoFinanciamentoValor();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposContratoFinanciamentoValor() {
    LimparCampos(_contratoFinanciamentoValor);
    _contratoFinanciamentoValor.Adicionar.visible(true);
    _contratoFinanciamentoValor.Atualizar.visible(false);
    _contratoFinanciamentoValor.Excluir.visible(false);
    _contratoFinanciamentoValor.Cancelar.visible(false);
    RecarregarGridValorContratoFinanciamento();
}