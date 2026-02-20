/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Licenca.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLicencaRegraPlanejamentoFrota;
var _licencaRegraPlanejamentoFrota;

var LicencaRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.Licenca = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Licença", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadLicencaRegraPlanejamentoFrota() {
    _licencaRegraPlanejamentoFrota = new LicencaRegraPlanejamentoFrota();
    KoBindings(_licencaRegraPlanejamentoFrota, "knockoutLicencaRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirLicencaClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridLicencaRegraPlanejamentoFrota = new BasicDataTable(_licencaRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLicenca(_licencaRegraPlanejamentoFrota.Licenca, null, _gridLicencaRegraPlanejamentoFrota);

    RecarregarGridLicencaRegraPlanejamentoFrota();
}

function RecarregarGridLicencaRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.Licencas.val() != "") {
        $.each(_regraPlanejamentoFrota.Licencas.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridLicencaRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirLicencaClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridLicencaRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridLicencaRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposLicencaRegraPlanejamentoFrota() {
    LimparCampos(_licencaRegraPlanejamentoFrota);
    _gridLicencaRegraPlanejamentoFrota.CarregarGrid(new Array());
}