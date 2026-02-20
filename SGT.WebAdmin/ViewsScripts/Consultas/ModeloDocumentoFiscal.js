/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarModeloDocumentoFiscal = function (knout, callbackRetorno, basicGrid, somenteEditavel, incluirNFSe, todosOsModelos, incluirNFSManual) {

    if (somenteEditavel == null)
        somenteEditavel = false;

    if (todosOsModelos === true)
        somenteEditavel = null;

    if (incluirNFSe == null)
        incluirNFSe = false;

    if (incluirNFSManual == null)
        incluirNFSManual = false;

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ModeloDocumentoFiscal.BuscarModelosDeDocumentos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ModeloDocumentoFiscal.ModelosDeDocumentos, type: types.local });
        this.Numero = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.ModeloDocumentoFiscal.Numero.getFieldDescription() });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.ModeloDocumentoFiscal.Descricao.getFieldDescription() });
        this.SomenteEditavel = PropertyEntity({ val: ko.observable(somenteEditavel), visible: false });
        this.IncluirNFSe = PropertyEntity({ val: ko.observable(incluirNFSe), visible: false });
        this.IncluirNFSManual = PropertyEntity({ val: ko.observable(incluirNFSManual), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloDocumentoFiscal/Pesquisa", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModeloDocumentoFiscal/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Numero.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};