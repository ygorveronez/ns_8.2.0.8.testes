var BuscarOcorrenciaIntegracaoEmbarcador = function (knout, callbackRetorno, multiplaEscolha, apenasConsulta) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Ocorrências Importadas do Embarcador", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Ocorrências Importadas do Embarcador", type: types.local });

        this.NumeroOcorrenciaEmbarcador = PropertyEntity({ text: "Nº da Ocorrência do Embarcador: ", col: 2 });
        this.NumeroOcorrencia = PropertyEntity({ text: "Nº da Ocorrência: ", col: 2 });

        this.DataInicialOcorrencia = PropertyEntity({ text: "Data Inicial: ", col: 2, getType: typesKnockout.date, visible: true });
        this.DataFinalOcorrencia = PropertyEntity({ text: "Data Final: ", col: 2, getType: typesKnockout.date, visible: true });

        this.DataInicialOcorrencia.dateRangeLimit = this.DataFinalOcorrencia;
        this.DataFinalOcorrencia.dateRangeInit = this.DataInicialOcorrencia;

        this.Situacao = PropertyEntity({ text: "Situação: ", col: 4, def: [], val: ko.observable([]), getType: typesKnockout.selectMultiple, options: EnumSituacaoOcorrenciaIntegracaoEmbarcador.ObterOpcoes(), visible: true });
        this.Empresa = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: true });
        this.GrupoPessoa = PropertyEntity({ col: 4, type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo Pessoas:", idBtnSearch: guid(), visible: true });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoa);
    }, null, true);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.NumeroOcorrenciaEmbarcador);
        knoutOpcoes.NumeroOcorrencia.val(knoutOpcoes.NumeroOcorrencia.def);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);

            Global.setarFocoProximoCampo(knout.id);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "OcorrenciaIntegracao/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "OcorrenciaIntegracao/Pesquisa", knoutOpcoes, apenasConsulta ? null : divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroCargaEmbarcador.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}