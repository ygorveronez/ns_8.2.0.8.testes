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
/// <reference path="GrupoMotivoChamado.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoMotivoChamadoTiposOperacoes, _gridGrupoMotivoChamadoTiposOperacoes, _crudGrupoMotivoChamadoTiposOperacoes;

var GrupoMotivoChamadoTiposOperacoes = function () {
    this.Grid = PropertyEntity({ text: 'Tipos de Operações', type: types.local, id: guid() });
    this.Adicionar = PropertyEntity({ type: types.event, text: 'Adicionar Tipo de Operação', visible: ko.observable(true), idBtnSearch: guid() });
};
//*******EVENTOS*******

function loadGrupoMotivoChamadoTiposOperacoes() {

    _grupoMotivoChamadoTiposOperacoes = new GrupoMotivoChamadoTiposOperacoes();
    KoBindings(_grupoMotivoChamadoTiposOperacoes, "knockoutGrupoMotivoChamadoTiposOperacoes");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [
            { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: removerGrupoMotivoChamadoTiposOperacoesClick }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%" },
    ];

    _gridGrupoMotivoChamadoTiposOperacoes = new BasicDataTable(_grupoMotivoChamadoTiposOperacoes.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridGrupoMotivoChamadoTiposOperacoes.CarregarGrid([]);

    new BuscarTiposOperacao(_grupoMotivoChamadoTiposOperacoes.Adicionar, null, null, null, _gridGrupoMotivoChamadoTiposOperacoes);
}

//*******MÉTODOS*******

function removerGrupoMotivoChamadoTiposOperacoesClick(registro) {
    let list = _gridGrupoMotivoChamadoTiposOperacoes.BuscarRegistros();

    list = list.filter(x => x.Codigo != registro.Codigo);
    _gridGrupoMotivoChamadoTiposOperacoes.CarregarGrid(list);
}

function limparCamposGrupoMotivoChamadoTiposOperacoes() {
    LimparCampos(_grupoMotivoChamadoTiposOperacoes);
    _gridGrupoMotivoChamadoTiposOperacoes.CarregarGrid([]);
}