/// <reference path="Abertura.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aberturaChapa;
var _gridAberturaChapa;

var AberturaChapa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Nome = PropertyEntity({ text: "*Nome:", maxlength: 60, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.CPF = PropertyEntity({ text: "*CPF:", getType: typesKnockout.cpf, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: "Telefone: ", getType: typesKnockout.phone, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarChapaClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local, id: guid() });
};

//*******EVENTOS*******

function loadAberturaChapa() {
    _aberturaChapa = new AberturaChapa();
    KoBindings(_aberturaChapa, "tabChapa");

    var excluir = { descricao: "Excluir", id: guid(), metodo: ExcluirChapaClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Nome", width: "50%" },
        { data: "CPF", title: "CPF", width: "15%" },
        { data: "Telefone", title: "Telefone", width: "15%" }
    ];

    _gridAberturaChapa = new BasicDataTable(_aberturaChapa.Grid.id, header, menuOpcoes);

    recarregarGridChapas();
}

function ExcluirChapaClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o chapa?", function () {
        $.each(_abertura.Chapas.list, function (i, lista) {
            if (data.Codigo == lista.Codigo.val) {
                _abertura.Chapas.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridChapas();
    });
}

function AdicionarChapaClick() {
    var valido = ValidarCamposObrigatorios(_aberturaChapa);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }
    else if (!ValidarCPF(_aberturaChapa.CPF.val())) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "CPF informado é inválido!");
        return;
    }

    _aberturaChapa.Codigo.val(guid());
    _abertura.Chapas.list.push(SalvarListEntity(_aberturaChapa));

    limparCamposAberturaChapa();
    $("#" + _aberturaChapa.Nome.id).focus();

}

//*******MÉTODOS*******

function recarregarGridChapas() {

    var data = new Array();

    $.each(_abertura.Chapas.list, function (i, lista) {
        var listaGrid = new Object();

        listaGrid.Codigo = lista.Codigo.val;
        listaGrid.Nome = lista.Nome.val;
        listaGrid.CPF = lista.CPF.val;
        listaGrid.Telefone = lista.Telefone.val;

        data.push(listaGrid);
    });

    _gridAberturaChapa.CarregarGrid(data);
}

function ControleCamposAberturaChapa(status) {
    SetarEnableCamposKnockout(_aberturaChapa, status);
    if (!status)
        _gridAberturaChapa.DesabilitarOpcoes();
    else
        _gridAberturaChapa.HabilitarOpcoes();
}

function limparCamposAberturaChapa() {
    LimparCampos(_aberturaChapa);
    recarregarGridChapas();
}