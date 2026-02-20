/// <reference path="../../../js/Global/CRUD.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _EdicaoNFe;
var _CRUDEdicaoNFe;

var EdicaoNFe = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Peso = PropertyEntity({ getType: typesKnockout.decimal, text: "*Peso: ", val: ko.observable(0), def: 0, required: true });
    this.ChaveVenda = PropertyEntity({ getType: typesKnockout.string , text: "*Chave de Venda: ", visible: ko.observable(false), val: ko.observable(""), def: "", required: true });
    this.ExigeChaveVenda = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false) });
    this.Volumes = PropertyEntity({ getType: typesKnockout.int, text: "Volumes: ", val: ko.observable(0), def: 0, required: false });
    this.Pallets = PropertyEntity({ getType: typesKnockout.decimal, text: "N° Pallets: ", val: ko.observable(0), def: 0, required: false });
    this.MetrosCubicos = PropertyEntity({ getType: typesKnockout.decimal, text: "Metros Cúbicos: ", val: ko.observable(0), def: 0, required: false });
}

var CRUDEdicaoNFe = function () {
    this.Atualizar = PropertyEntity({ eventClick: AtualizarNFeAgendamentoColeta, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadEdicaoNFe() {
    _EdicaoNFe = new EdicaoNFe();
    KoBindings(_EdicaoNFe, "knockoutDetalhesEdicaoNFe");

    _CRUDEdicaoNFe = new CRUDEdicaoNFe();
    KoBindings(_CRUDEdicaoNFe, "knockoutCRUDEdicaoNFe");
}

//*******MÉTODOS*******

function AtualizarNFeAgendamentoColeta() {
    if (!ValidarCamposObrigatorios(_EdicaoNFe)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("AgendamentoColeta/AtualizarNFe", RetornarObjetoPesquisa(_EdicaoNFe), function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.Success, "Sucesso", arg.msg);

            if (_gridNFeAgendamento)
                _gridNFeAgendamento.CarregarGrid();

            Global.fecharModal("divEdicaoNotaFiscalAgendamentoColeta");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}
