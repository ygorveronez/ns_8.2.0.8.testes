var BuscarDocumentosParaEmissaoNFSManual = function (knout, callbackRetorno, basicGrid, knoutCodigoNFSManual) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Documentos para NFS-e Manual", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Documentos", type: types.local });
        this.Codigo = PropertyEntity({ visible: false, getType: typesKnockout.int });
        this.NumeroInicial = PropertyEntity({ col: 3, text: "Nº Inicial: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' } });
        this.NumeroFinal = PropertyEntity({ col: 3, text: "Nº Final: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' } });
        this.DataInicial = PropertyEntity({ col: 3, text: "Data Inicial: ", getType: typesKnockout.date });
        this.DataFinal = PropertyEntity({ col: 3, text: "Data Final: ", getType: typesKnockout.date });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;

    var url = "NFSManual/PesquisaDocumentoExterno";

    funcaoParamentroDinamico = function () {
        if (knoutCodigoNFSManual != null)
            knoutOpcoes.Codigo.val(knoutCodigoNFSManual.val());
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroInicial.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};