/// <reference path="../../../js/plugin/dropzone/dropzone.js" />
/// <reference path="../../../js/plugin/dropzone/dropzone-amd-module.backup.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="DropzoneLiquidacao.js" />


var _liquidacaoPallet;


function LiquidacaoPallet() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Pallets = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.dynamic, type: types.map });

    this.Enviar = PropertyEntity({
        eventClick: function (e) {
            EnviarPalletClic()
        }, type: types.event, text: "Enviar", idGrid: guid(), visible: ko.observable(true)
    });
}

function loadLiquidacaoPallet() {
    _liquidacaoPallet = new LiquidacaoPallet();
    KoBindings(_liquidacaoPallet, "knoutLiquidacaoPallet");
    loadDropZone();
}



/*
 * Declaração das Funções Associadas a Eventos
 */

function AbrirModalLiquidacaoClick(devolucaoPalletsGrid) {
    LimparCamposLiquidacoPallet();
    LimparDropzoneLiquidacao();
    _liquidacaoPallet.Codigo.val(devolucaoPalletsGrid.CodigoPallet);

    Global.abrirModal('divModalLiquidacao');
}


/*
 * Declaração das Funções
 */
function LimparCamposLiquidacoPallet() {
    LimparCampos(_liquidacaoPallet);
}

function EnviarPalletClic() {

    var formData = obterFormDataAnexos();

    if (!formData)
        return;

    enviarArquivo("LiquidacaoValePallet/AnexarArquivos?callback=?", { Codigo: _liquidacaoPallet.Codigo.val()}, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Arquivo(s) Processado com sucesso");
                $('.modal').modal('hide');
            }
            else
                exibirMensagem(tipoMensagem.falha,"Nao Foi possivel enviar pallet(s)", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });

}

function obterFormDataAnexos() {
    const arquivos = obterArquivosProcessados()
    if (arquivos.length > 0) {
        var formData = new FormData();

        arquivos.forEach(function (arquivo) {
            formData.append("Arquivo", arquivo);
            formData.append("Descricao", "");

        });

        return formData;
    }

    return undefined;
}

