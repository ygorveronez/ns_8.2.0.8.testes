/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoTerceiro.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoTerceiroPropriedadeVeiculo;
var _tiposTerceirosPropriedadeVeiculo;

var TiposTerceiroPropriedadeVeiculo = function () {
    this.GridTipoTerceiroPropriedadeVeiculo = PropertyEntity({ type: types.local });
    this.TerceiroPropriedadeVeiculo = PropertyEntity({ type: types.event, text: "Adicionar Tipos de Terceiro", idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadTiposTerceirosPropriedadeVeiculo() {
    _tiposTerceirosPropriedadeVeiculo = new TiposTerceiroPropriedadeVeiculo();
    KoBindings(_tiposTerceirosPropriedadeVeiculo, "knockoutTiposTerceiroPropriedadeVeiculo");
    
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirTerceiroPropriedadeVeiculoClick(_tiposTerceirosPropriedadeVeiculo.TerceiroPropriedadeVeiculo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "20%" },
        { data: "DescricaoSituacao", title: Localization.Resources.Gerais.Geral.Situacao, width: "20%" }
    ];

    _gridTipoTerceiroPropriedadeVeiculo = new BasicDataTable(_tiposTerceirosPropriedadeVeiculo.GridTipoTerceiroPropriedadeVeiculo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoTerceiro(_tiposTerceirosPropriedadeVeiculo.TerceiroPropriedadeVeiculo, null, _gridTipoTerceiroPropriedadeVeiculo);
    _tiposTerceirosPropriedadeVeiculo.TerceiroPropriedadeVeiculo.basicTable = _gridTipoTerceiroPropriedadeVeiculo;

    RecarregarGridTipoTerceiroPropriedadeVeiculo();
}

function RecarregarGridTipoTerceiroPropriedadeVeiculo() {
    var data = new Array();
    debugger;
    if (!string.IsNullOrWhiteSpace(_tipoOperacao.TiposTerceirosPropriedadeVeiculo.val())) {

        $.each(_tipoOperacao.TiposTerceirosPropriedadeVeiculo.val(), function (i, tiposTerceirosPropriedadeVeiculo) {
            var tipoTerceiroGridPropriedadeVeiculo = new Object();

            tipoTerceiroGridPropriedadeVeiculo.Codigo = tiposTerceirosPropriedadeVeiculo.Codigo;
            tipoTerceiroGridPropriedadeVeiculo.Descricao = tiposTerceirosPropriedadeVeiculo.Descricao;
            tipoTerceiroGridPropriedadeVeiculo.DescricaoSituacao = tiposTerceirosPropriedadeVeiculo.DescricaoSituacao;

            data.push(tipoTerceiroGridPropriedadeVeiculo);
        });
    }
    _tipoOperacao.TiposTerceirosPropriedadeVeiculo.val(RetornarObjetoPesquisa(_tiposTerceirosPropriedadeVeiculo));
    _gridTipoTerceiroPropriedadeVeiculo.CarregarGrid(data);
}


function excluirTerceiroPropriedadeVeiculoClick(knoutTiposTerceirosPropriedadeVeiculo, data) {
    var tiposTerceirosGridPropriedadeVeiculo = knoutTiposTerceirosPropriedadeVeiculo.basicTable.BuscarRegistros();

    for (var i = 0; i < tiposTerceirosGridPropriedadeVeiculo.length; i++) {
        if (data.Codigo == tiposTerceirosGridPropriedadeVeiculo[i].Codigo) {
            tiposTerceirosGridPropriedadeVeiculo.splice(i, 1);
            break;
        }
    }
    knoutTiposTerceirosPropriedadeVeiculo.basicTable.CarregarGrid(tiposTerceirosGridPropriedadeVeiculo);
}

function limparCamposTiposTerceiroPropriedadeVeiculo() {
    LimparCampos(_tiposTerceirosPropriedadeVeiculo);
}