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
var _tipoOperacao;

/*
 * Declaração das Classes
 */

var TipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Tipo = PropertyEntity({ type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarTipoOperacao, idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadTipoOperacao() {

    _tipoOperacao = new TipoOperacao();
    KoBindings(_tipoOperacao, "knockoutTipoOperacao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirTipoOperacaoClick(_tipoOperacao.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
                  { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }];

    _gridTipoOperacao = new BasicDataTable(_tipoOperacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_tipoOperacao.Tipo, null, null, null, _gridTipoOperacao);
    _tipoOperacao.Tipo.basicTable = _gridTipoOperacao;

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

function limparCamposTipoOperacao() {
    LimparCampos(_tipoOperacao);
}

function obterTipoOperacaoSalvar() {
    var tiposOperacoes = new Array();

    $.each(_tipoOperacao.Tipo.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacoes.push({ Tipo: tipoOperacao });
    });

    return JSON.stringify(tiposOperacoes);
}

/*
 * Declaração das Funções
 */

function recarregarGridTipoOperacao() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_filial.TiposOperacoes.val())) {
        $.each(_filial.TiposOperacoes.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.Tipo.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.Tipo.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }

    _gridTipoOperacao.CarregarGrid(data);
}