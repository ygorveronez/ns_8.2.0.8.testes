/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPedidoCampoObrigatorio;
var _motoristaCampoObrigatorio;

/*
 * Declaração das Classes
 */

var CRUDMotoristaCampoObrigatorio = function () {
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
};

var MotoristaCampoObrigatorio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Campo = PropertyEntity({ type: types.event, text: "Adicionar Campo", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridCampo = PropertyEntity({ type: types.local });
    this.Campos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadMotoristaCampoObrigatorio() {
    _motoristaCampoObrigatorio = new MotoristaCampoObrigatorio();
    KoBindings(_motoristaCampoObrigatorio, "knockoutMotoristaCampoObrigatorio");

    HeaderAuditoria("MotoristaCampoObrigatorio", _motoristaCampoObrigatorio);

    _CRUDMotoristaCampoObrigatorio = new CRUDMotoristaCampoObrigatorio();
    KoBindings(_CRUDMotoristaCampoObrigatorio, "knockoutCRUDMotoristaCampoObrigatorio");

    LoadCampoMotorista();

    EditarClick();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AtualizarClick(e, sender) {
    _motoristaCampoObrigatorio.Campos.val(JSON.stringify(_motoristaCampoObrigatorio.Campo.basicTable.BuscarRegistros()));

    Salvar(_motoristaCampoObrigatorio, "MotoristaCampoObrigatorio/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function EditarClick(registroSelecionado, duplicar) {

    executarReST("MotoristaCampoObrigatorio/BuscarMotoristaCampoObrigatorioPadrao", { }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_motoristaCampoObrigatorio, retorno);

                ControlarBotoesHabilitados(!duplicar);

                _motoristaCampoObrigatorio.Campo.basicTable.CarregarGrid(_motoristaCampoObrigatorio.Campos.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}


/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDMotoristaCampoObrigatorio.Atualizar.visible(true);
}

