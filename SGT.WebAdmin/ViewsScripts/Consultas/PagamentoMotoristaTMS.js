/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarPagamentoMotoristaTMS = function (knout, callbackRetorno, knoutAcertoViagem, basicGrid, pagamentosParaAcertoViagem) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }
    var vPagamentosParaAcertoViagem = false;
    if (pagamentosParaAcertoViagem == null || pagamentosParaAcertoViagem == undefined)
        vPagamentosParaAcertoViagem = false;
    else
        vPagamentosParaAcertoViagem = pagamentosParaAcertoViagem;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pagamentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pagamentos", type: types.local });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });
        this.PagamentosParaAcertoViagem = PropertyEntity({ col: 12, text: "Para Acerto: ", visible: false, val: ko.observable(vPagamentosParaAcertoViagem), def: ko.observable(vPagamentosParaAcertoViagem) });
        this.TipoPagamentoMotorista = PropertyEntity({ val: ko.observable(EnumTipoPagamentoMotorista.Todos), options: EnumTipoPagamentoMotorista.obterOpcoesPesquisa(), def: EnumTipoPagamentoMotorista.Todos, text: "Tipo de Pagamento: ", visible: false });

        this.Numero = PropertyEntity({ col: 6, text: "Número: " });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutAcertoViagem != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutAcertoViagem.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }
    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PagamentoMotoristaTMS/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PagamentoMotoristaTMS/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
};
var BuscarPagamentoMotoristaTMSPendente = function (knout, knoutMotorista, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }


    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pagamentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pagamentos", type: types.local });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });
        this.TipoPagamentoMotorista = PropertyEntity({ val: ko.observable(EnumTipoPagamentoMotorista.Todos), options: EnumTipoPagamentoMotorista.obterOpcoesPesquisa(), def: EnumTipoPagamentoMotorista.Todos, text: "Tipo de Pagamento: ", visible: false });
        this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista", val: ko.observable(0) });
        this.Pendente = PropertyEntity({ type: types.entity, codEntity: ko.observable(false), text: "Pendente", val: ko.observable(false) });
        this.Situacao = PropertyEntity({ col: 2, val: ko.observable(EnumSituacaoPagamentoMotorista.FinalizadoPagamento), def: (EnumSituacaoPagamentoMotorista.FinalizadoPagamento) });
        this.Numero = PropertyEntity({ col: 6, text: "Número: " });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }
    console.log(this.Situacao);
    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutMotorista != null) {
        funcaoParamentroDinamico = function () {

            knoutOpcoes.Motorista.val(knoutMotorista.val());
            knoutOpcoes.Motorista.codEntity(knoutMotorista.codEntity());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PagamentoMotoristaTMS/PesquisaPendenciaMotorista", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PagamentoMotoristaTMS/PesquisaPendenciaMotorista", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);


    
    divBusca.AddTabPressEvent(function (outraPressionada) {
        
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}
