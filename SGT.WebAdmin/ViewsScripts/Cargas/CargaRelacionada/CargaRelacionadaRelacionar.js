var _relacionarCarga;

var RelacionarCarga = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PossuiRelacionada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.CargaRelacionada = PropertyEntity({ text: "*Carga Relacionada: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.PossuiRelacionada.val.subscribe(function (valor) {
        if (!valor)
            _relacionarCarga.Remover.visible(valor)
        else
            _relacionarCarga.Remover.visible(valor)
    });

    this.Cancelar = PropertyEntity({ eventClick: limparModalCargaRelacionada, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
    this.Remover = PropertyEntity({ eventClick: removerRelacaoCarga, type: types.event, text: "Remover", idGrid: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarRelacaoCarga, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true) });
}

function loadCargaRelacionar() {
    _relacionarCarga = new RelacionarCarga();
    KoBindings(_relacionarCarga, "knockoutRelacionarCarga");

    new BuscarCargasCargaRelacionada(_relacionarCarga.CargaRelacionada, null, null);
}

function adicionarRelacaoCarga() {
    let data = {
        Codigo: _relacionarCarga.Codigo.val(),
        CargaRelacionada: _relacionarCarga.CargaRelacionada.codEntity(),
        PossuiRelacionada: _relacionarCarga.PossuiRelacionada.val()
    }

    if (ValidarCamposObrigatorios(_relacionarCarga)) {
        executarReST("CargaRelacionada/AdicionarCargaRelacionada", data, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);

                _gridCargaRelacionada.CarregarGrid();
                limparModalCargaRelacionada();
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function removerRelacaoCarga() {
    executarReST("CargaRelacionada/RemoverCargaRelacionada", { Codigo: _relacionarCarga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);

            _gridCargaRelacionada.CarregarGrid();
            limparModalCargaRelacionada();
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function buscarCargaRelacionada(codigo) {
    executarReST("CargaRelacionada/BuscarCargaRelacionadaCodigo", { Codigo: codigo }, function (retorno) {
        _relacionarCarga.Codigo.val(retorno.Data.Codigo);
        _relacionarCarga.CargaRelacionada.val(retorno.Data.CargaRelacionada.Descricao);
        _relacionarCarga.CargaRelacionada.codEntity(retorno.Data.CargaRelacionada.Codigo);
        _relacionarCarga.PossuiRelacionada.val(retorno.Data.PossuiRelacionada);
    });
}

function limparModalCargaRelacionada() {
    Global.fecharModal("divModalRelacionarCarga");
    LimparCampos(_relacionarCarga);
}

