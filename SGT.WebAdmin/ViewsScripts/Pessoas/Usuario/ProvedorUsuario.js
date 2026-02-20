/// <reference path="Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProvedoresUsuarios;

//*******EVENTOS*******

function loadProvedorUsuario() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirProvedorUsuarioClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridProvedoresUsuarios = new BasicDataTable(_usuario.GridProvedoresUsuarios.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    BuscarClientes(_usuario.ProvedorUsuario, null, false, null, null, _gridProvedoresUsuarios);

    _usuario.ProvedoresUsuarios.basicTable = _gridProvedoresUsuarios;
    recarregarGridProvedoresUsuarios();
}

function recarregarGridProvedoresUsuarios() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_usuario.ProvedoresUsuarios.val())) {

        $.each(_usuario.ProvedoresUsuarios.val(), function (i, provedor) {
            var obj = new Object();

            obj.Codigo = provedor.Codigo;
            obj.Descricao = provedor.Descricao;

            data.push(obj);
        });
    }

    _gridProvedoresUsuarios.CarregarGrid(data);
}

function excluirProvedorUsuarioClick(data) {
    var provedorUsuarioGrid = _usuario.ProvedoresUsuarios.basicTable.BuscarRegistros();

    for (var i = 0; i < provedorUsuarioGrid.length; i++) {
        if (data.Codigo == provedorUsuarioGrid[i].Codigo) {
            provedorUsuarioGrid.splice(i, 1);
            break;
        }
    }

    _usuario.ProvedoresUsuarios.basicTable.CarregarGrid(provedorUsuarioGrid);
}

function LimparCamposProvedorUsuario() {
    _usuario.ProvedoresUsuarios.basicTable.CarregarGrid(new Array());
}