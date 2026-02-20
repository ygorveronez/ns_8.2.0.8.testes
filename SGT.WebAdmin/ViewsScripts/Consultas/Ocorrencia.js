/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumSituacaoOcorrencia.js" />

var BuscarOcorrenciaSemAcertoDeViagem = function (knout, callbackRetorno, basicGrid, knoutCodigoAcertoViagem, knoutBuscarTodasOcorrencias) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Ocorrências para o Acerto de Viagem", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Ocorrência", type: types.local });

        this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", col: 4, getType: typesKnockout.int });
        this.DataInicio = PropertyEntity({ text: "Data Inicio: ", col: 4, getType: typesKnockout.date, visible: false });
        this.DataFim = PropertyEntity({ text: "Data Fim: ", col: 4, getType: typesKnockout.date, visible: false });
        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });
        this.BuscarTodasOcorrencias = PropertyEntity({ text: "Todas Ocorrencias: ", col: 12, getType: typesKnockout.bool, visible: false, val: ko.observable(false) });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoAcertoViagem != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutCodigoAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutCodigoAcertoViagem.val());
        }
    }
    if (knoutBuscarTodasOcorrencias != null) {
        knoutOpcoes.BuscarTodasOcorrencias.val(knoutBuscarTodasOcorrencias);
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, null);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AcertoOcorrencia/PesquisarOcorrenciaSemAcerto", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AcertoOcorrencia/PesquisarOcorrenciaSemAcerto", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroOcorrencia.val(knout.val());
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

var BuscarOcorrenciaComissaoFuncionario = function (knout, callbackRetorno, basicGrid, knoutCodigoComissaoFuncionario) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Ocorrências para a Comissão", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Ocorrência", type: types.local });

        this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", col: 4, getType: typesKnockout.int });
        this.DataInicio = PropertyEntity({ text: "Data Inicio: ", col: 4, getType: typesKnockout.date, visible: false });
        this.DataFim = PropertyEntity({ text: "Data Fim: ", col: 4, getType: typesKnockout.date, visible: false });
        this.ComissaoFuncionario = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Comissão Funcionario:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoComissaoFuncionario != null) {
        knoutOpcoes.ComissaoFuncionario.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.ComissaoFuncionario.codEntity(knoutCodigoComissaoFuncionario.val());
            knoutOpcoes.ComissaoFuncionario.val(knoutCodigoComissaoFuncionario.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, null);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AcertoOcorrencia/PesquisarOcorrenciaComissaoFuncionario", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "AcertoOcorrencia/PesquisarOcorrenciaComissaoFuncionario", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroOcorrencia.val(knout.val());
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

var BuscarOcorrencias = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Ocorrências", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Ocorrência", type: types.local });

        this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", col: 6, getType: typesKnockout.int });
        this.Carga = PropertyEntity({ text: "Carga: ", col: 6, getType: typesKnockout.string });
        this.DataInicio = PropertyEntity({ text: "Data Inicio: ", col: 6, getType: typesKnockout.date });
        this.DataFim = PropertyEntity({ text: "Data Fim: ", col: 6, getType: typesKnockout.date });
        this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Todos), def: EnumTipoDocumentoCreditoDebito.Todos, visible: false });
        this.SituacaoOcorrencia = PropertyEntity({ text: "Situação: ", col: 6, options: EnumSituacaoOcorrencia.obterOpcoesPesquisaRelatorio(), val: ko.observable(EnumSituacaoOcorrencia.Todas), visible: true });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Ocorrencia/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroOcorrencia.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}
