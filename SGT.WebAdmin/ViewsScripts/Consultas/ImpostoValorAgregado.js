/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/ModeloDocumentoFiscal.js" />

var BuscarImpostoValorAgregado = function (knout, callbackRetorno, somentePermitirInformarManualmente) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Imposto sobre Valor Agregado (IVA)", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Impostos sobre Valor Agregado", type: types.local });

        this.CodigoIVA = PropertyEntity({ text: "Código IVA:", maxlength: 4, col: 6});
        this.ModeloDocumentoFiscal = PropertyEntity({ text: "Modelo de Documento Fiscal:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), col: 6 });
        this.PermitirInformarManualmente = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarModeloDocumentoFiscal(knoutOpcoes.ModeloDocumentoFiscal);
    });

    if (somentePermitirInformarManualmente)
        knoutOpcoes.PermitirInformarManualmente.val(EnumSimNaoPesquisa.Sim);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ImpostoValorAgregado/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.CodigoIVA.val(knout.val());
        
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}
