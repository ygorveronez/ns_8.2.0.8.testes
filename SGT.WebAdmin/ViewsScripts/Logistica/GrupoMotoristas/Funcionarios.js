/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="./GrupoMotoristas.js" />

var _gridFuncionarios;
var _funcionarios;

var Funcionarios = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.Funcionario = PropertyEntity({ type: types.event, text: "Adicionar Motorista", idBtnSearch: guid() });
};

function LoadFuncionarios() {
    _funcionarios = new Funcionarios();
    KoBindings(_funcionarios, "knockoutMotoristas");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirFuncionariosClick(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Nome", width: "70%" },
        { data: "CPF", title: "CPF", width: "30%" },
    ];

    _gridFuncionarios = new BasicDataTable(_funcionarios.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _funcionarios.Funcionario.basicTable = _gridFuncionarios;

    new BuscarMotoristas(_funcionarios.Funcionario, null, null, _gridFuncionarios);

    RecarregarGridFuncionarios();
}

function RecarregarGridFuncionarios() {

    let data = new Array();

    if (_grupoMotoristas.Funcionarios.val() != "" && _grupoMotoristas.Funcionarios.val().length > 0) {
        $.each(_grupoMotoristas.Funcionarios.val(), function (i, fun) {
            let funcionariosGrid = new Object();

            funcionariosGrid.Codigo = fun.Codigo;
            funcionariosGrid.Descricao = fun.Descricao;
            funcionariosGrid.CodigoRelacionamento = fun.CodigoRelacionamento;
            funcionariosGrid.CPF = fun.CPF;

            data.push(funcionariosGrid);
        });
    }

    _gridFuncionarios.CarregarGrid(data);
}

function PreencherFuncionarios(listaFuncionariosRetornadas) {
    _grupoMotoristas.Funcionarios.val(listaFuncionariosRetornadas);
    RecarregarGridFuncionarios();
}

function ExcluirFuncionariosClick(data) {
    let funcionariosGrid = _gridFuncionarios.BuscarRegistros();

    for (let i = 0; i < funcionariosGrid.length; i++) {
        if (data.Codigo == funcionariosGrid[i].Codigo) {
            funcionariosGrid.splice(i, 1);
            break;
        }
    }

    _gridFuncionarios.CarregarGrid(funcionariosGrid);
}

