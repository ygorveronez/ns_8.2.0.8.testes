/// <reference path="Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEmpresas;

//*******EVENTOS*******

function loadEmpresa() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirEmpresaClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CNPJ", title: Localization.Resources.Pessoas.Usuario.CNPJ, width: "20%", className: "text-align-left" },
        { data: "RazaoSocial", title: Localization.Resources.Pessoas.Usuario.RazaoSocial, width: "70%", className: "text-align-left" }
    ];

    _gridEmpresas = new BasicDataTable(_usuario.GridEmpresas.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarTransportadores(_usuario.Empresa, null, null, null, _gridEmpresas);
    _usuario.Empresa.basicTable = _gridEmpresas;

    recarregarGridEmpresas();
}

function recarregarGridEmpresas() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_usuario.Empresas.val())) {

        $.each(_usuario.Empresas.val(), function (i, empresa) {
            var obj = new Object();

            obj.Codigo = empresa.Codigo;
            obj.CNPJ = empresa.CNPJ;
            obj.RazaoSocial = empresa.RazaoSocial;

            data.push(obj);
        });
    }

    _gridEmpresas.CarregarGrid(data);
}

function excluirEmpresaClick(data) {
    var empresaGrid = _usuario.Empresa.basicTable.BuscarRegistros();

    for (var i = 0; i < empresaGrid.length; i++) {
        if (data.Codigo == empresaGrid[i].Codigo) {
            empresaGrid.splice(i, 1);
            break;
        }
    }

    _usuario.Empresa.basicTable.CarregarGrid(empresaGrid);
}

function LimparCamposEmpresa() {
    _usuario.Empresa.basicTable.CarregarGrid(new Array());
}