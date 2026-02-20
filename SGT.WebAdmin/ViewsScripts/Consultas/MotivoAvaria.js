/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumFinalidadeMotivoAvaria.js" />

var BuscarMotivoAvaria = function (knout, callbackRetorno, finalidade) {

    var idDiv = guid();
    var GridConsulta;

    /*var _responsavelPesquisa = [
        { text: "Todos", value: EnumResponsavelAvaria.Todos },
        { text: "Transportador", value: EnumResponsavelAvaria.Transportador },
        { text: "Carregamento/Descarregamento", value: EnumResponsavelAvaria.CarregamentoDescarregamento }
    ];*/

    if (!$.isFunction(callbackRetorno))
    {
        finalidade = callbackRetorno;
        callbackRetorno = null;
    }

    if (finalidade == null)
        finalidade = EnumFinalidadeMotivoAvaria.Todas;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Motivo da Avaria", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos da Avaria", type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição:", col: 12 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: "Ativo:", visible: false });
        /**
         * Aguardando a implementacao de campo Option nos filtros de busca
         */
        //this.Responsavel = PropertyEntity({ text: "Responsável: ", getType: typesKnockout.dynamic, val: ko.observable(EnumResponsavelAvaria.Todos), options: _responsavelPesquisa, def: EnumResponsavelAvaria.Todos });
        //this.Responsavel = PropertyEntity({ text: "Responsável: ", getType: typesKnockout.int, val: ko.observable(EnumResponsavelAvaria.Todos), visible: false });
        this.Finalidade = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(finalidade), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoAvaria/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}