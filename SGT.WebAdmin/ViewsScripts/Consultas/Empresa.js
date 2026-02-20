/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarEmpresa = function (knout, callbackRetorno, knoutSomenteEmpresaFilho, knoutListarSomenteEmpresasFiliais, somenteEmpresaSemTransportadorContratante, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    somenteEmpresaSemTransportadorContratante = (somenteEmpresaSemTransportadorContratante ? somenteEmpresaSemTransportadorContratante : false);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Empresa.PesquisarEmpresas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Empresa.Empresas, type: types.local });

        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.Empresa.RazaoSocial.getFieldDescription(), col: 12 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false });

        this.SomenteEmpresaFilho = PropertyEntity({ val: ko.observable(knoutSomenteEmpresaFilho), def: knoutSomenteEmpresaFilho, visible: false });
        this.ListarSomenteEmpresasFiliais = PropertyEntity({ val: ko.observable(knoutListarSomenteEmpresasFiliais), def: knoutListarSomenteEmpresasFiliais, visible: false });
        this.SomenteEmpresaSemTransportadorContratante = PropertyEntity({ val: ko.observable(somenteEmpresaSemTransportadorContratante), def: somenteEmpresaSemTransportadorContratante, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.RazaoSocial);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Empresa/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Empresa/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
};