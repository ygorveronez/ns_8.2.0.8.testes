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

var _gridLiberacaoGRRegraPlanejamentoFrota;
var _liberacaoGRRegraPlanejamentoFrota;

var LiberacaoGRRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.LiberacaoGR = PropertyEntity({ type: types.event, text: "Adicionar Liberação de GR", idBtnSearch: guid()});


};

//*******EVENTOS*******

function LoadLiberacaoGRRegraPlanejamentoFrota() {
    _liberacaoGRRegraPlanejamentoFrota = new LiberacaoGRRegraPlanejamentoFrota();
    KoBindings(_liberacaoGRRegraPlanejamentoFrota, "knockoutLiberacaoGRRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirLiberacaoGRClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" }
    ];

    _gridLiberacaoGRRegraPlanejamentoFrota = new BasicDataTable(_liberacaoGRRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLicenca(_liberacaoGRRegraPlanejamentoFrota.LiberacaoGR, null, _gridLiberacaoGRRegraPlanejamentoFrota);

    RecarregarGridLiberacaoGRRegraPlanejamentoFrota();
}

function RecarregarGridLiberacaoGRRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.LiberacoesGR.val() != "") {
        $.each(_regraPlanejamentoFrota.LiberacoesGR.val(), function (i, arg) {
            var liberacaoGRExistente = new Object();

            liberacaoGRExistente.Codigo = arg.Codigo;
            liberacaoGRExistente.Descricao = arg.Descricao;

            data.push(liberacaoGRExistente);
        });
    }

    _gridLiberacaoGRRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirLiberacaoGRClickRegraPlanejamentoFrota(data) {
    var liberacoesGR = _gridLiberacaoGRRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < liberacoesGR.length; i++) {
        if (data.Codigo == liberacoesGR[i].Codigo) {
            liberacoesGR.splice(i, 1);
            break;
        }
    }
    _gridLiberacaoGRRegraPlanejamentoFrota.CarregarGrid(liberacoesGR);
}

function LimparCamposLiberacaoGRRegraPlanejamentoFrota() {
    LimparCamposLiberacaoGR();
    _gridLiberacaoGRRegraPlanejamentoFrota.CarregarGrid(new Array());
}

function LimparCamposLiberacaoGR() {
    LimparCampos(_liberacaoGRRegraPlanejamentoFrota);
}


