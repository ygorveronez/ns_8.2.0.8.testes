/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />


var BuscarMotivosDevolucaoEntrega = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, KnouMarca, basicGrid) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa de Motivo de Devolução", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Motivos de Devolução", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });
        this.Marca = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (KnouMarca != null) {
        knoutOpcoes.Marca.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Marca.codEntity(KnouMarca.codEntity());
            knoutOpcoes.Marca.val(KnouMarca.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoDevolucaoEntrega/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoDevolucaoEntrega/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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
    })

    this.openModal = () => {
        divBusca.OpenModal();
    }
}
