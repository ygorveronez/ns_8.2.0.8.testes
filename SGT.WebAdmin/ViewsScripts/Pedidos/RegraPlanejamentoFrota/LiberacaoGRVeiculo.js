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

let _gridLiberacaoGRVeiculoRegraPlanejamentoFrota;
let _liberacaoGRVeiculoRegraPlanejamentoFrota;

let LiberacaoGRVeiculoRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.LiberacaoGRVeiculo = PropertyEntity({ type: types.event, text: "Adicionar Liberação de GR de veículo", idBtnSearch: guid()});
};

//*******EVENTOS*******

function LoadLiberacaoGRVeiculoRegraPlanejamentoFrota() {
    _liberacaoGRVeiculoRegraPlanejamentoFrota = new LiberacaoGRVeiculoRegraPlanejamentoFrota();
    KoBindings(_liberacaoGRVeiculoRegraPlanejamentoFrota, "knockoutLiberacaoGRVeiculoRegraPlanejamentoFrota");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirLiberacaoGRVeiculoClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" }
    ];

    _gridLiberacaoGRVeiculoRegraPlanejamentoFrota = new BasicDataTable(_liberacaoGRVeiculoRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarLicenca(_liberacaoGRVeiculoRegraPlanejamentoFrota.LiberacaoGRVeiculo, null, _gridLiberacaoGRVeiculoRegraPlanejamentoFrota);

    RecarregarGridLiberacaoGRVeiculoRegraPlanejamentoFrota();
}

function RecarregarGridLiberacaoGRVeiculoRegraPlanejamentoFrota() {
    let data = new Array();

    if (_regraPlanejamentoFrota.LiberacoesGRVeiculo.val() != "") {
        $.each(_regraPlanejamentoFrota.LiberacoesGRVeiculo.val(), function (i, arg) {
            let liberacaoGRExistente = new Object();

            liberacaoGRExistente.Codigo = arg.Codigo;
            liberacaoGRExistente.Descricao = arg.Descricao;

            data.push(liberacaoGRExistente);
        });
    }

    _gridLiberacaoGRVeiculoRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirLiberacaoGRVeiculoClickRegraPlanejamentoFrota(data) {
    let liberacoesGR = _gridLiberacaoGRVeiculoRegraPlanejamentoFrota.BuscarRegistros();

    for (let i = 0; i < liberacoesGR.length; i++) {
        if (data.Codigo == liberacoesGR[i].Codigo) {
            liberacoesGR.splice(i, 1);
            break;
        }
    }
    _gridLiberacaoGRVeiculoRegraPlanejamentoFrota.CarregarGrid(liberacoesGR);
}

function LimparCamposLiberacaoGRVeiculoRegraPlanejamentoFrota() {
    LimparCamposLiberacaoGRVeiculo();
    _gridLiberacaoGRVeiculoRegraPlanejamentoFrota.CarregarGrid(new Array());
}

function LimparCamposLiberacaoGRVeiculo() {
    LimparCampos(_liberacaoGRVeiculoRegraPlanejamentoFrota);
}


