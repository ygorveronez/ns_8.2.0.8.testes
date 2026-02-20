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
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="RegraPlanejamentoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoPessoaRegraPlanejamentoFrota;
var _grupoPessoaRegraPlanejamentoFrota;

var GrupoPessoaRegraPlanejamentoFrota = function () {
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.event, text: "Adicionar Grupo de Pessoa", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadGrupoPessoaRegraPlanejamentoFrota() {
    _grupoPessoaRegraPlanejamentoFrota = new GrupoPessoaRegraPlanejamentoFrota();
    KoBindings(_grupoPessoaRegraPlanejamentoFrota, "knockoutGruposPessoasRegraPlanejamentoFrota");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirGrupoPessoaClickRegraPlanejamentoFrota(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridGrupoPessoaRegraPlanejamentoFrota = new BasicDataTable(_grupoPessoaRegraPlanejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarGruposPessoas(_grupoPessoaRegraPlanejamentoFrota.GrupoPessoa, null, null, _gridGrupoPessoaRegraPlanejamentoFrota);

    RecarregarGridGrupoPessoaRegraPlanejamentoFrota();
}

function RecarregarGridGrupoPessoaRegraPlanejamentoFrota() {
    var data = new Array();

    if (_regraPlanejamentoFrota.GruposPessoas.val() != "")
    {
        $.each(_regraPlanejamentoFrota.GruposPessoas.val(), function (i, tipoCarga) {
            var tiposCargasGrid = new Object();

            tiposCargasGrid.Codigo = tipoCarga.Codigo;
            tiposCargasGrid.Descricao = tipoCarga.Descricao;

            data.push(tiposCargasGrid);
        });
    }

    _gridGrupoPessoaRegraPlanejamentoFrota.CarregarGrid(data);
}

function ExcluirGrupoPessoaClickRegraPlanejamentoFrota(data) {
    var tiposCargasGrid = _gridGrupoPessoaRegraPlanejamentoFrota.BuscarRegistros();

    for (var i = 0; i < tiposCargasGrid.length; i++) {
        if (data.Codigo == tiposCargasGrid[i].Codigo) {
            tiposCargasGrid.splice(i, 1);
            break;
        }
    }

    _gridGrupoPessoaRegraPlanejamentoFrota.CarregarGrid(tiposCargasGrid);
}

function LimparCamposGrupoPessoaRegraPlanejamentoFrota() {
    LimparCampos(_grupoPessoaRegraPlanejamentoFrota);
    _gridGrupoPessoaRegraPlanejamentoFrota.CarregarGrid(new Array());
}