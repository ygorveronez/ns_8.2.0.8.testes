/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTiposCriticidadeAtendimento = function (knout, tipoCriticidade, knouMotivoAtendimento, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        var titulo = tipoCriticidade === "Gerencial" ? "Buscar Gerencial" : "Buscar Causa do Problema";
        var tituloGrid = tipoCriticidade === "Gerencial" ? "Gerenciais" : "Causas do Problema";

        this.Titulo = PropertyEntity({ text: titulo, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGrid, type: types.local });
        this.FiltrarPorTipo = PropertyEntity({ visible: false, val: ko.observable(true) });
        this.TipoCriticidade = PropertyEntity({ visible: false, val: ko.observable(tipoCriticidade) });

        this.Descricao = PropertyEntity({ text: "Descrição", col: 9 });

        this.CodigoMotivoChamado = PropertyEntity({
            type: types.entity,
            codEntity: ko.observable(0),
            idBtnSearch: guid(),
            visible: false
        });

        this.Pesquisar = PropertyEntity({
            eventClick: function () { GridConsulta.CarregarGrid(); },
            type: types.event,
            text: "Pesquisar",
            visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametrosDinamicos = function () {
        var cod = 0, val = "";
        if (knouMotivoAtendimento && typeof knouMotivoAtendimento.codEntity === "function") {
            cod = Number(knouMotivoAtendimento.codEntity() || 0);
            val = (knouMotivoAtendimento.val && knouMotivoAtendimento.val()) || "";
        }
        if (!cod && typeof _abertura !== "undefined" && _abertura && _abertura.MotivoChamado) {
            cod = Number(_abertura.MotivoChamado.codEntity && _abertura.MotivoChamado.codEntity() || 0);
            val = (_abertura.MotivoChamado.val && _abertura.MotivoChamado.val()) || "";
        }
        knoutOpcoes.CodigoMotivoChamado.codEntity(cod || 0);
        knoutOpcoes.CodigoMotivoChamado.val(val || "");
        knoutOpcoes.TipoCriticidade.val(tipoCriticidade);
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Conteudo || e.Descricao || "");
        divBusca.CloseModal();
    };
    if (callbackRetorno) {
        var cb = callbackRetorno;
        callback = function (e) { divBusca.CloseModal(); cb(e); };
    }

    GridConsulta = new GridView(
        idDiv + "_tabelaEntidades",
        "MotivoChamado/CriticidadeAtendimentoPesquisa",
        knoutOpcoes,
        divBusca.OpcaoPadrao(callback)
    );

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data && lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};