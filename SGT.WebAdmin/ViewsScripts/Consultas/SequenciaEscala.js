/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarSequenciaEscalas = function (knout, knoutModeloVeicularCarga, callbackRetorno, basicGrid, multiplaEscolha) {
    var idDiv = guid();
    var GridConsulta;

    if (multiplaEscolha == null)
        multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;



    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Sequência de Escalas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Sequência de Escalas", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: ", maxlength: 250 });
        this.Ativo = PropertyEntity({ col: 12, text: "Ativo: ", val: ko.observable(1), visible: false });
        this.ModeloVeicularCarga = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular de Carga:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;

    if (knoutModeloVeicularCarga != null) {
        knoutOpcoes.ModeloVeicularCarga.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.ModeloVeicularCarga.codEntity(knoutModeloVeicularCarga.codEntity());
            knoutOpcoes.ModeloVeicularCarga.val(knoutModeloVeicularCarga.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarModelosVeicularesCarga(knoutOpcoes.ModeloVeicularCarga);
    }, null);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SequenciaEscalas/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SequenciaEscalas/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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

    return { ModalBusca: divBusca, Grid: GridConsulta };
}