/// <reference path="../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarMotivoPedido = function (knout, callbackRetorno, TipoMotivo) {
    var idDiv = guid();
    var GridConsulta;


    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisa de Motivos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });

      
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Buscar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    switch (TipoMotivo) {
        case EnumTipoMotivoPedido.RejeicaoPedido: 
            GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoPedido/PesquisarMotivoRejeicao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
            break;
        case EnumTipoMotivoPedido.LancamentoPedido:
            GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoPedido/PesquisarMotivoLancamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
            break;
        case EnumTipoMotivoPedido.AprovacaoPedido:
            GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoPedido/PesquisarMotivoAprovacao", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
            break;
        default:
            GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoPedido/PesquisarMotivo", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
            break;
    }

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
