
var _modeloGridAdicionar;

var ModeloGridAdicionar = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true });
    this.ModeloPadrao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.ModeloPadrao.getFieldDescription(), val: ko.observable(false), def: false, required: true });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, eventClick: adicionarModeloGrid });
}

function loadModeloGridAdicionar() {
    _modeloGridAdicionar = new ModeloGridAdicionar();
    KoBindings(_modeloGridAdicionar, "knockoutModeloGridAdicionar");

    $("#modal-modelo-grid-adicionar").on('hidden.bs.modal', function () {
        limparCamposModeloGridAdicionar();
    });
}

function adicionarModeloGrid(e, sender) {
    if (!ValidarCamposObrigatorios(_modeloGridAdicionar)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return false;
    }

    var data = {
        URLGrid: _modeloGrid.UrlGrid.val(),
        IdGrid: _modeloGrid.IdGrid.val(),
        DadosGrid: _modeloGrid.DadosGrid.val(),
        Descricao: _modeloGridAdicionar.Descricao.val(),
        ModeloPadrao: _modeloGridAdicionar.ModeloPadrao.val(),
    };

    executarReST("Preferencias/AdicionarModeloGrid", data, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                BuscarModelosGrid();
                Global.fecharModal("modal-modelo-grid-adicionar");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function limparCamposModeloGridAdicionar() {
    LimparCampos(_modeloGridAdicionar);
}