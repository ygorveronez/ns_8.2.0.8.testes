/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../Consultas/Empresa.js" />
/// <reference path="../ParametrosOfertas.js" />

var _gridEmpresas;
var _empresas;

var Empresas = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.Empresa = PropertyEntity({ type: types.event, text: "Adicionar Empresas", idBtnSearch: guid() });
};

function LoadEmpresas() {
    _empresas = new Empresas();
    KoBindings(_empresas, "knockoutEmpresas");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirEmpresasClick(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
        { data: "CNPJ_Formatado", title: "CNPJ", width: "30%" }
    ];

    _gridEmpresas = new BasicDataTable(_empresas.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _empresas.Empresa.basicTable = _gridEmpresas;

    new BuscarEmpresa(_empresas.Empresa, null, null, null, null, _gridEmpresas);

    RecarregarGridEmpresas();
}

function RecarregarGridEmpresas() {

    let data = new Array();

    if (_parametrosOfertas.Empresas.val() != "" && _parametrosOfertas.Empresas.val().length > 0) {
        $.each(_parametrosOfertas.Empresas.val(), function (i, emp) {
            let empresasGrid = new Object();

            empresasGrid.Codigo = emp.Codigo;
            empresasGrid.Descricao = emp.Descricao;
            empresasGrid.CodigoRelacionamento = emp.CodigoRelacionamento;
            empresasGrid.CNPJ_Formatado = emp.CNPJ_Formatado;

            data.push(empresasGrid);
        });
    }

    _gridEmpresas.CarregarGrid(data);
}

function PreencherEmpresas(listaEmpresasRetornadas) {
    _parametrosOfertas.Empresas.val(listaEmpresasRetornadas);
    RecarregarGridEmpresas();
}

function ExcluirEmpresasClick(data) {
    let empresasGrid = _gridEmpresas.BuscarRegistros();

    for (let i = 0; i < empresasGrid.length; i++) {
        if (data.Codigo == empresasGrid[i].Codigo) {
            empresasGrid.splice(i, 1);
            break;
        }
    }

    _gridEmpresas.CarregarGrid(empresasGrid);
}

function LimparCamposEmpresas() {
    LimparCampos(_empresas);
    _gridEmpresas.CarregarGrid(new Array());
}