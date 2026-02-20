/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../enumeradores/enumtipotipodetalhe.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTiposDetalhe = function (knout, callbackRetorno, menuOpcoes, tipo) {
    
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {

        var texto = Localization.Resources.Consultas.TipoDetalhe.ProcessamentosEspeciais;
        if (tipo == EnumTipoTipoDetalhe.HorarioEntrega)
            texto = Localization.Resources.Consultas.TipoDetalhe.HorariosEntrega;
        if (tipo == EnumTipoTipoDetalhe.PeriodoEntrega)
            texto = Localization.Resources.Pedidos.Pedido.PeriodoEntrega;
        if (tipo == EnumTipoTipoDetalhe.DetalheEntrega)
            texto = Localization.Resources.Pedidos.Pedido.DetalheEntrega;
        if (tipo == EnumTipoTipoDetalhe.ZonaTransporte)
            texto = Localization.Resources.Pedidos.Pedido.ZonaTransporte;

        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoDetalhe.Buscar + " " + texto, type: types.local });
        this.TituloGrid = PropertyEntity({ text: texto, type: types.local });

        this.Codigo = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.TipoDetalhe.Codigo });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.TipoDetalhe.Descricao });
        this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoTipoDetalhe.Todos), def: EnumTipoTipoDetalhe.Todos, visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.TipoDetalhe.Buscar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    if (tipo != EnumTipoTipoDetalhe.Todos) {
        knoutOpcoes.Tipo.val(tipo);
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    var opcoes = divBusca.OpcaoPadrao(callback)
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoDetalhe/Pesquisa", knoutOpcoes, opcoes, null);
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
    })
}