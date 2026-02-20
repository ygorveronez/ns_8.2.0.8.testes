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
/// <reference path="FuncionarioMeta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _funcionarioMetaTabela, _gridFuncionarioMetaTabela;

var FuncionarioMetaTabela = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.Mes = PropertyEntity({ text: "*Mês:", getType: typesKnockout.int, maxlength: 2, required: ko.observable(true), enable: ko.observable(true) });
    this.Ano = PropertyEntity({ text: "*Ano:", getType: typesKnockout.int, maxlength: 4, required: ko.observable(true), enable: ko.observable(true) });

    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true), enable: ko.observable(true) });
    this.Percentual = PropertyEntity({ text: "Percentual:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, required: ko.observable(false), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarTabelaFuncionarioMetaClick, type: types.event, text: "Adicionar", enable: ko.observable(true), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarTabelaFuncionarioMetaClick, type: types.event, text: "Atualizar", enable: ko.observable(true), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirTabelaFuncionarioMetaClick, type: types.event, text: "Excluir", enable: ko.observable(true), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposFuncionarioMetaTabela, type: types.event, text: "Cancelar", enable: ko.observable(true), visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadFuncionarioMetaTabela() {
    _funcionarioMetaTabela = new FuncionarioMetaTabela();
    KoBindings(_funcionarioMetaTabela, "knockoutTabelaFuncionarioMeta");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarTabelaFuncionarioMetaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Mes", title: "Mês", width: "20%" },
        { data: "Ano", title: "Ano", width: "20%" },
        { data: "Valor", title: "Valor", width: "20%" },
        { data: "Percentual", title: "Percentual", width: "20%" }
    ];

    _gridFuncionarioMetaTabela = new BasicDataTable(_funcionarioMetaTabela.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridTabelaFuncionarioMeta();
}

//*******MÉTODOS*******

function RecarregarGridTabelaFuncionarioMeta() {
    var data = new Array();

    $.each(_funcionarioMeta.Metas.list, function (i, meta) {
        var metaGrid = new Object();

        metaGrid.Codigo = meta.Codigo.val;
        metaGrid.Mes = meta.Mes.val;
        metaGrid.Ano = meta.Ano.val;
        metaGrid.Valor = meta.Valor.val;
        metaGrid.Percentual = meta.Percentual.val;

        data.push(metaGrid);
    });

    _gridFuncionarioMetaTabela.CarregarGrid(data);
}

function EditarTabelaFuncionarioMetaClick(data) {
    for (var i = 0; i < _funcionarioMeta.Metas.list.length; i++) {
        if (data.Codigo == _funcionarioMeta.Metas.list[i].Codigo.val) {
            var meta = _funcionarioMeta.Metas.list[i];

            _funcionarioMetaTabela.Codigo.val(meta.Codigo.val);
            _funcionarioMetaTabela.Mes.val(meta.Mes.val);
            _funcionarioMetaTabela.Ano.val(meta.Ano.val);
            _funcionarioMetaTabela.Valor.val(meta.Valor.val);
            _funcionarioMetaTabela.Percentual.val(meta.Percentual.val);

            _funcionarioMetaTabela.Adicionar.visible(false);
            _funcionarioMetaTabela.Atualizar.visible(true);
            _funcionarioMetaTabela.Excluir.visible(true);
            _funcionarioMetaTabela.Cancelar.visible(true);
        }
    }
}

function ExcluirTabelaFuncionarioMetaClick() {
    for (var i = 0; i < _funcionarioMeta.Metas.list.length; i++) {
        if (_funcionarioMetaTabela.Codigo.val() == _funcionarioMeta.Metas.list[i].Codigo.val) {
            _funcionarioMeta.Metas.list.splice(i, 1);
            break;
        }
    }

    LimparCamposFuncionarioMetaTabela();
}

function AdicionarTabelaFuncionarioMetaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_funcionarioMetaTabela);

    if ((Globalize.parseFloat(_funcionarioMetaTabela.Mes.val()) <= 0) || (Globalize.parseFloat(_funcionarioMetaTabela.Mes.val()) > 12)) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe o Mês corretamente");
        return;
    } else if (Globalize.parseFloat(_funcionarioMetaTabela.Ano.val()) <= 2000) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe o Ano corretamente");
        return;
    }

    if (valido) {
        _funcionarioMetaTabela.Codigo.val(guid());
        _funcionarioMeta.Metas.list.push(SalvarListEntity(_funcionarioMetaTabela));

        LimparCamposFuncionarioMetaTabela();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarTabelaFuncionarioMetaClick() {
    var valido = ValidarCamposObrigatorios(_funcionarioMetaTabela);

    if ((Globalize.parseFloat(_funcionarioMetaTabela.Mes.val()) <= 0) || (Globalize.parseFloat(_funcionarioMetaTabela.Mes.val()) > 12)) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe o Mês corretamente");
        return;
    } else if (Globalize.parseFloat(_funcionarioMetaTabela.Ano.val()) <= 2000) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe o Ano corretamente");
        return;
    }

    if (valido) {
        for (var i = 0; i < _funcionarioMeta.Metas.list.length; i++) {
            if (_funcionarioMetaTabela.Codigo.val() == _funcionarioMeta.Metas.list[i].Codigo.val) {
                _funcionarioMeta.Metas.list[i] = SalvarListEntity(_funcionarioMetaTabela);
                break;
            }
        }

        LimparCamposFuncionarioMetaTabela();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposFuncionarioMetaTabela() {
    LimparCampos(_funcionarioMetaTabela);
    _funcionarioMetaTabela.Adicionar.visible(true);
    _funcionarioMetaTabela.Atualizar.visible(false);
    _funcionarioMetaTabela.Excluir.visible(false);
    _funcionarioMetaTabela.Cancelar.visible(false);
    RecarregarGridTabelaFuncionarioMeta();
}