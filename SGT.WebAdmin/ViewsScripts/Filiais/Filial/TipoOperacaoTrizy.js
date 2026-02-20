/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="Filial.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTipoOperacao;
var _tipoOperacaoTrizy;

/*
 * Declaração das Classes
 */

var TipoOperacaoTrizy = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TiposOperacao = PropertyEntity({ type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarTipoOperacao, idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadTipoOperacaoTrizy() {

    _tipoOperacaoTrizy = new TipoOperacaoTrizy();
    KoBindings(_tipoOperacaoTrizy, "knockoutTipoOperacaoTrizy");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirTipoOperacaoClick(_tipoOperacaoTrizy.TiposOperacao, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
                  { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacaoTrizy.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacaoTrizy.TiposOperacao, null, null, null, _gridTipoOperacao);
    _tipoOperacaoTrizy.TiposOperacao.basicTable = _gridTipoOperacao;

    recarregarGridTipoOperacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirTipoOperacaoClick(knoutTipoOperacao, data) {
    var tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo == tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposTipoOperacaoTrizy() {
    LimparCampos(_tipoOperacaoTrizy);
}

function obterTipoOperacaoTrizySalvar() {
    var tiposOperacoes = new Array();

    $.each(_tipoOperacaoTrizy.TiposOperacao.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacoes.push({ TiposOperacao: tipoOperacao });
    });

    return JSON.stringify(tiposOperacoes);
}

/*
 * Declaração das Funções
 */

function recarregarGridTipoOperacaoTrizy() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_filial.TipoOperacao.val())) {
        $.each(_filial.TipoOperacao.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.Tipo.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.Tipo.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }

    _gridTipoOperacao.CarregarGrid(data);
}