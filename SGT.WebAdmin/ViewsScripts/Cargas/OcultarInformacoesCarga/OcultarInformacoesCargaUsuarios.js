/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="OcultarInformacoesCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridUsuarios, _usuarios;

var Usuarios = function () {
    this.Usuario = PropertyEntity({ type: types.event, text: "Adicionar Usuário", idBtnSearch: guid(), codEntity: ko.observable(0) });

    this.Grid = PropertyEntity({ type: types.local });
}

function LoadOcultarInformacoesUsuarios() {
    _usuarios = new Usuarios();
    KoBindings(_usuarios, "knockoutOcultarInformacoesCargaUsuarios");

    LoadGridUsuarios();
}

function LoadGridUsuarios() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirUsuarioClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Usuário", width: "70%" },
    ];

    _gridUsuarios = new BasicDataTable(_usuarios.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridUsuarios();
}

function RecarregarGridUsuarios() {
    var data = new Array();
    if (_ocultarInformacoesCarga.Usuarios.val() != "" && _ocultarInformacoesCarga.Usuarios.val().length > 0) {
        $.each(_ocultarInformacoesCarga.Usuarios.val(), function (i, usuario) {
            var usuariosGrid = new Object();

            usuariosGrid.Codigo = usuario.Codigo;
            usuariosGrid.Descricao = usuario.Descricao;

            data.push(usuariosGrid);
        });
    }

    _gridUsuarios.CarregarGrid(data);
}

function ExcluirUsuarioClick(data) {
    var usuariosGrid = _gridUsuarios.BuscarRegistros();

    for (var i = 0; i < usuariosGrid.length; i++) {
        if (data.Codigo == usuariosGrid[i].Codigo) {
            usuariosGrid.splice(i, 1);
            break;
        }
    }

    _gridUsuarios.CarregarGrid(usuariosGrid);
}

function LimparCamposUsuario() {
    LimparCampo(_usuarios.Usuario);
    _gridUsuarios.CarregarGrid(new Array());
}