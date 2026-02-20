/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridFiliais;
var _filiaisPermitidas;


var CRUDFiliaisPermitidas = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
};

var FiliaisPermitidas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Filial = PropertyEntity({ type: types.event, text: "Adicionar Filial", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridFilial = PropertyEntity({ type: types.local });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

//*******EVENTOS*******

function loaConfigFiliaisFilaCarregamento() {
    _filiaisPermitidas = new FiliaisPermitidas();
    KoBindings(_filiaisPermitidas, "knockoutConfigFiliais");

    _CRUDFiliaisPermitidas = new CRUDFiliaisPermitidas();
    KoBindings(_CRUDFiliaisPermitidas, "knockoutCRUDFiliaisPermitidas");

    CarregarGridFiliais();
    BuscarRegistrosPadrao();
}

function atualizarClick(e, sender) {
    _filiaisPermitidas.Filiais.val(JSON.stringify(_filiaisPermitidas.Filial.basicTable.BuscarRegistros()));

    Salvar(_filiaisPermitidas, "ConfigFiliaisFilaCarregamento/Atualizar", function (retorno) {
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

function BuscarRegistrosPadrao() {
    executarReST("ConfigFiliaisFilaCarregamento/BuscarFiliaisLiberadasPadrao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_filiaisPermitidas, retorno);

                _filiaisPermitidas.Filial.basicTable.CarregarGrid(_filiaisPermitidas.Filiais.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

//*******MÉTODOS*******


function CarregarGridFiliais() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirFilialClick(_filiaisPermitidas.Filial, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridFiliais = new BasicDataTable(_filiaisPermitidas.GridFilial.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFilial(_filiaisPermitidas.Filial, null, _gridFiliais);

    _filiaisPermitidas.Filial.basicTable = _gridFiliais;
    _filiaisPermitidas.Filial.basicTable.CarregarGrid(new Array());
}

function ExcluirFilialClick(knoutCampo, data) {
    var camposGrid = knoutCampo.basicTable.BuscarRegistros();

    for (var i = 0; i < camposGrid.length; i++) {
        if (data.Codigo == camposGrid[i].Codigo) {
            camposGrid.splice(i, 1);
            break;
        }
    }

    knoutCampo.basicTable.CarregarGrid(camposGrid);
}