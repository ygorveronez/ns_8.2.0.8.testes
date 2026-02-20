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
/// <reference path="Bem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _bemComponente, _gridBemComponente;

var BemComponente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.NumeroSerie = PropertyEntity({ text: "*Número de Série: ", required: ko.observable(true), maxlength: 500 });
    this.DataFimGarantia = PropertyEntity({ text: "*Data Fim Garantia: ", getType: typesKnockout.date, required: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarBemComponenteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarBemComponenteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirBemComponenteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposBemComponenteAlteracao, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadBemComponente() {
    _bemComponente = new BemComponente();
    KoBindings(_bemComponente, "knockoutComponenteBem");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarBemComponenteClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "50%" },
        { data: "NumeroSerie", title: "Número Série", width: "20%" },
        { data: "DataFimGarantia", title: "Data Fim Garantia", width: "10%" }
    ];

    _gridBemComponente = new BasicDataTable(_bemComponente.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridBemComponente();
}

//*******MÉTODOS*******

function RecarregarGridBemComponente() {
    var data = new Array();

    $.each(_bem.Componentes.list, function (i, componente) {
        var componenteGrid = new Object();

        componenteGrid.Codigo = componente.Codigo.val;
        componenteGrid.Descricao = componente.Descricao.val;
        componenteGrid.NumeroSerie = componente.NumeroSerie.val;
        componenteGrid.DataFimGarantia = componente.DataFimGarantia.val;

        data.push(componenteGrid);
    });

    _gridBemComponente.CarregarGrid(data);
}

function EditarBemComponenteClick(data) {
    for (var i = 0; i < _bem.Componentes.list.length; i++) {
        if (data.Codigo == _bem.Componentes.list[i].Codigo.val) {
            var componente = _bem.Componentes.list[i];

            _bemComponente.Codigo.val(componente.Codigo.val);
            _bemComponente.Descricao.val(componente.Descricao.val);
            _bemComponente.NumeroSerie.val(componente.NumeroSerie.val);
            _bemComponente.DataFimGarantia.val(componente.DataFimGarantia.val);

            _bemComponente.Adicionar.visible(false);
            _bemComponente.Atualizar.visible(true);
            _bemComponente.Excluir.visible(true);
            _bemComponente.Cancelar.visible(true);
        }
    }
}

function ExcluirBemComponenteClick() {
    for (var i = 0; i < _bem.Componentes.list.length; i++) {
        if (_bemComponente.Codigo.val() == _bem.Componentes.list[i].Codigo.val) {
            _bem.Componentes.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridBemComponente();
    LimparCamposBemComponenteAlteracao();
}

function AdicionarBemComponenteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_bemComponente);

    if (valido) {
        _bemComponente.Codigo.val(guid());
        _bem.Componentes.list.push(SalvarListEntity(_bemComponente));

        RecarregarGridBemComponente();
        LimparCamposBemComponenteAlteracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarBemComponenteClick() {
    var valido = ValidarCamposObrigatorios(_bemComponente);

    if (valido) {
        for (var i = 0; i < _bem.Componentes.list.length; i++) {
            if (_bemComponente.Codigo.val() == _bem.Componentes.list[i].Codigo.val) {
                _bem.Componentes.list[i] = SalvarListEntity(_bemComponente);
                break;
            }
        }

        RecarregarGridBemComponente();
        LimparCamposBemComponenteAlteracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposBemComponenteAlteracao() {
    _bemComponente.Descricao.val("");
    _bemComponente.NumeroSerie.val("");
    _bemComponente.DataFimGarantia.val("");

    _bemComponente.Adicionar.visible(true);
    _bemComponente.Atualizar.visible(false);
    _bemComponente.Excluir.visible(false);
    _bemComponente.Cancelar.visible(false);
}

function LimparCamposBemComponente() {
    LimparCampos(_bemComponente);
    _bem.Componentes.list = new Array();
    RecarregarGridBemComponente();
}