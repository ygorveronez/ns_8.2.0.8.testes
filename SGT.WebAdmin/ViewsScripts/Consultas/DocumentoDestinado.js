var BuscarDocumentosDestinados = function (knout, callbackRetorno, basicGrid, tipoDocumento) {

    var idDiv = guid();
    var gridConsulta;

    if (tipoDocumento != null)
        tipoDocumento = [].concat(tipoDocumento);

    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Documentos Destinados", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Documentos Destinados", type: types.local });

        this.TipoDocumento = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(tipoDocumento)), text: "Tipo do Documento:", visible: false });

        this.NumeroDe = PropertyEntity({ col: 2, getType: typesKnockout.int, visible: true, text: "Nº Inicial:" });
        this.NumeroAte = PropertyEntity({ col: 2, getType: typesKnockout.int, visible: true, text: "Nº Final:" });

        this.DataEmissaoInicial = PropertyEntity({ col: 2, getType: typesKnockout.date, visible: true, text: "Emissão Inicial:", val: ko.observable(Global.DataAtual()) });
        this.DataEmissaoFinal = PropertyEntity({ col: 2, getType: typesKnockout.date, visible: true, text: "Emissão Final:", val: ko.observable(Global.DataAtual()) });

        this.Chave = PropertyEntity({ col: 4, visible: true, text: "Chave:" });

        this.Empresa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: true });

        this.CPFCNPJFornecedor = PropertyEntity({ col: 2, visible: true, text: "CPF/CNPJ Fornecedor:" });
        this.NomeFornecedor = PropertyEntity({ col: 4, visible: true, text: "Nome Fornecedor:" });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () { };
    var url = "DocumentoDestinadoEmpresa/PesquisaParaCarga";

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
    }, null, true);

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
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, { column: 4, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 4, dir: orderDir.desc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroDe.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}