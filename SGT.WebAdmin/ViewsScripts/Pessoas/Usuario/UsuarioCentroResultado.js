/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentrosResultado;

//*******EVENTOS*******

function loadCentroResultado() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirCentroResultadoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%" },
        { data: "Plano", title: Localization.Resources.Pessoas.Usuario.NumeroDoCentro, width: "20%" }
    ];

    _gridCentrosResultado = new BasicDataTable(_usuario.GridCentrosResultado.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCentroResultado(_usuario.CentroResultado, null, null, null, null, null, _gridCentrosResultado);
    _usuario.CentroResultado.basicTable = _gridCentrosResultado;

    recarregarGridCentrosResultado();

    if (_CONFIGURACAO_TMS.PermiteInformarCentroResultadoAprovacaoOcorrencia)
        $("#liTabCentroResultado").show();
}

function recarregarGridCentrosResultado() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_usuario.CentrosResultado.val())) {

        $.each(_usuario.CentrosResultado.val(), function (i, centroResultado) {
            var obj = new Object();

            obj.Codigo = centroResultado.Codigo;
            obj.Descricao = centroResultado.Descricao;
            obj.Plano = centroResultado.Plano;

            data.push(obj);
        });
    }
    _gridCentrosResultado.CarregarGrid(data);
}

function excluirCentroResultadoClick(data) {
    var centroResultadoGrid = _usuario.CentroResultado.basicTable.BuscarRegistros();

    for (var i = 0; i < centroResultadoGrid.length; i++) {
        if (data.Codigo == centroResultadoGrid[i].Codigo) {
            centroResultadoGrid.splice(i, 1);
            break;
        }
    }

    _usuario.CentroResultado.basicTable.CarregarGrid(centroResultadoGrid);
}

function LimparCamposCentroResultado() {
    _usuario.CentroResultado.basicTable.CarregarGrid(new Array());
}