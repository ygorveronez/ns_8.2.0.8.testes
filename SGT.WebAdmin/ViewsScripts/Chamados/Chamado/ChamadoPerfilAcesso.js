/// <reference path="../../Consultas/PerfilAcesso.js" />
/// <reference path="Chamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _perfilAcessoChamado;
var _gridPerfilAcesso;
var _chamadoOcorrenciaModalPerfilAcesso;

var ChamadoPerfilAcesso = function () {
    this.GridPerfilAcesso = PropertyEntity({ type: types.local });
    this.PerfisAcesso = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PerfilAcesso = PropertyEntity({ type: types.event, text: "Adicionar Perfil de Acesso", idBtnSearch: guid(), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadChamadoPerfilAcesso() {
    _perfilAcessoChamado = new ChamadoPerfilAcesso();
    KoBindings(_perfilAcessoChamado, "knockoutPerfilAcessoChamado");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { excluirPerfilAcessoChamadoClick(data) } }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridPerfilAcesso = new BasicDataTable(_perfilAcessoChamado.GridPerfilAcesso.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarPerfilAcesso(_perfilAcessoChamado.PerfilAcesso, null, null, _gridPerfilAcesso);
    _perfilAcessoChamado.PerfilAcesso.basicTable = _gridPerfilAcesso;

    recarregarGridPerfilAcesso();

    _chamadoOcorrenciaModalPerfilAcesso = new bootstrap.Modal(document.getElementById("divModalPerfilAcessoChamado"), { backdrop: 'static' })
}

function abrirModalPerfilAcessoChamadoClick() {
    _chamadoOcorrenciaModalPerfilAcesso.show();
    //limparCamposChamadoPerfilAcesso();
}

function preencherPerfilAcessoChamado(data) {
    PreencherObjetoKnout(_perfilAcessoChamado, { Data: data });
    recarregarGridPerfilAcesso();

    _gridPerfilAcesso.DesabilitarOpcoes();
    _perfilAcessoChamado.PerfilAcesso.enable(false);
}

function obterPerfisAcesso() {
    return JSON.stringify(_perfilAcessoChamado.PerfilAcesso.basicTable.BuscarRegistros());
}

function recarregarGridPerfilAcesso() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_perfilAcessoChamado.PerfisAcesso.val())) {
        $.each(_perfilAcessoChamado.PerfisAcesso.val(), function (i, perfilAcesso) {
            var obj = new Object();

            obj.Codigo = perfilAcesso.Codigo;
            obj.Descricao = perfilAcesso.Descricao;

            data.push(obj);
        });
    }

    _gridPerfilAcesso.CarregarGrid(data);
}

function excluirPerfilAcessoChamadoClick(data) {
    var perfilGrid = _perfilAcessoChamado.PerfilAcesso.basicTable.BuscarRegistros();

    for (var i = 0; i < perfilGrid.length; i++) {
        if (data.Codigo == perfilGrid[i].Codigo) {
            perfilGrid.splice(i, 1);
            break;
        }
    }

    _perfilAcessoChamado.PerfilAcesso.basicTable.CarregarGrid(perfilGrid);
}

function limparCamposChamadoPerfilAcesso() {
    _perfilAcessoChamado.PerfilAcesso.basicTable.CarregarGrid(new Array());
    _gridPerfilAcesso.HabilitarOpcoes();
    _perfilAcessoChamado.PerfilAcesso.enable(true);
}