/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ciot, _gridCIOT;

var CIOT = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CancelamentoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.BuscarCIOT = PropertyEntity({ eventClick: ConsultarCIOTCarga, type: types.event, text: "Buscar/Atualizar CIOT", visible: ko.observable(true), idGrid: guid() });
};

//*******EVENTOS*******

function LoadCIOT() {
    _ciot = new CIOT();
    KoBindings(_ciot, "knockoutCIOT");
}

function ConsultarCIOTCarga(e) {
    _ciot.Carga.val(_cancelamento.Carga.codEntity());
    _ciot.CancelamentoCarga.val(_cancelamento.Codigo.val());

    var reenviarCIOT = { descricao: "Reenviar", id: guid(), metodo: ReenviarCIOTClick, icone: ""};
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [reenviarCIOT], tamanho: 7 };

    _gridCIOT = new GridView(_ciot.BuscarCIOT.idGrid, "CancelamentoCargaCIOT/Pesquisa", _ciot, menuOpcoes, { column: 2, dir: orderDir.desc }, null);
    _gridCIOT.CarregarGrid();
}

function ReenviarCIOTClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja mesmo reenviar o CIOT?", function () {
        executarReST("CancelamentoCargaCIOT/ReenviarCIOT", { Cancelamento: _cancelamento.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "CIOT reenviado para cancelamento");
                    _gridCIOT.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}