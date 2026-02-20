/// <reference path="../../../js/Global/Globais.js" />

//#region Variaveis Globais
var _gridLinkarDocumento;
var _linkarDocumento;
//#endregion

//#region Constructores
function LinkarDocumento() {
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.Codigo = PropertyEntity({ val: ko.observable(0) });

    this.Linkar = PropertyEntity({ eventClick: executarLinkDocumento, type: types.event, text: "Linkar", idGrid: guid(), visible: ko.observable(true) });
}
//#endregion

//#region Carregamento
function loadLinkarDocumento() {
    _linkarDocumento = new LinkarDocumento();
    KoBindings(_linkarDocumento, "divModalLinkarDocumento");
    loadGridLinkarDocumento();
}

function loadGridLinkarDocumento() {
    //var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: abrirDetalhesIrregularidade, tamanho: "15", icone: "" };
    //var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    let multiplaEscolha = {
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        permitirSelecionarSomenteUmRegistro: true,
        somenteLeitura: false,
        callbackSelecionado: function (e, registroSelecionado) { },
        callbackNaoSelecionado: function () { }
    };

    _gridLinkarDocumento = new GridView(_linkarDocumento.Grid.idGrid, "ControleDocumento/BuscarPreCtesLikarDocumento", _linkarDocumento, null/*menuOpcoes*/, null, null, null, null, null, multiplaEscolha);

}
//#endregion
 
//#region Funções Auxiliares
function CarregarRegistroLinkarDocumento(codigo) {
    _linkarDocumento.Codigo.val(codigo);
    _gridLinkarDocumento.CarregarGrid();
}

function executarLinkDocumento() {
    let registros = _gridLinkarDocumento.ObterMultiplosSelecionados();
    if (registros.length == 0) {
        exibirMensagem(tipoMensagem.falha, "Falha", "Selecione um regidstro o link");
        return;
    }

    executarReST("ControleDocumento/LinkarDocumento", { Codigo: _linkarDocumento.Codigo.val(), CargaCTe: registros[0].Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Link do documento realizado com sucesso.");
                fecharAprovarMotivoSelecionados();
                _gridControleDocumento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function CancelarLinkarDocumento() {
    Global.fecharModal("divModalLinkarDocumento");

}

function fecharModalObservacao() {
    $("#observacaoIrregularidade").html("");
    Global.fecharModal("divModalObservacaoIrregularidade");
}
//#endregion