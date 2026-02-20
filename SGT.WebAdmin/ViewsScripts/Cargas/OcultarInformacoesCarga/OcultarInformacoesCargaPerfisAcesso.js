/// <reference path="../../Consultas/PerfilAcesso.js" />
/// <reference path="OcultarInformacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPerfisAcesso, _perfisAcesso;

var PerfisAcesso = function () {
    this.PerfilAcesso = PropertyEntity({ type: types.event, text: "Adicionar Perfil Acesso", idBtnSearch: guid() });

    this.Grid = PropertyEntity({ type: types.local });
}

function LoadOcultarInformacoesPerfisAcesso() {
    _perfisAcesso = new PerfisAcesso();
    KoBindings(_perfisAcesso, "knockoutOcultarInformacoesCargaPerfilAcesso");

    LoadGridPerfisAcesso();
}

function LoadGridPerfisAcesso() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirPerfilAcessoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Usuário", width: "70%" },
    ];

    _gridPerfisAcesso = new BasicDataTable(_perfisAcesso.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridPerfisAcesso();
}

function RecarregarGridPerfisAcesso() {
    var data = new Array();
    if (_ocultarInformacoesCarga.PerfisAcesso.val() != "" && _ocultarInformacoesCarga.PerfisAcesso.val().length > 0) {
        $.each(_ocultarInformacoesCarga.PerfisAcesso.val(), function (i, perfilAcesso) {
            var perfisAcessoGrid = new Object();

            perfisAcessoGrid.Codigo = perfilAcesso.Codigo;
            perfisAcessoGrid.Descricao = perfilAcesso.Descricao;

            data.push(perfisAcessoGrid);
        });
    }

    _gridPerfisAcesso.CarregarGrid(data);
}

function ExcluirPerfilAcessoClick(data) {
    var perfisAcessoGrid = _gridPerfisAcesso.BuscarRegistros();

    for (var i = 0; i < perfisAcessoGrid.length; i++) {
        if (data.Codigo == perfisAcessoGrid[i].Codigo) {
            perfisAcessoGrid.splice(i, 1);
            break;
        }
    }

    _gridPerfisAcesso.CarregarGrid(perfisAcessoGrid);
}

function LimparCamposPerfilAcesso() {
    LimparCampo(_perfisAcesso.PerfilAcesso);
    _gridPerfisAcesso.CarregarGrid(new Array());
}