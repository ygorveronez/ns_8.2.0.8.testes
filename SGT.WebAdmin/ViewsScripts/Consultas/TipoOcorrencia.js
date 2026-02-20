/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumOrigemOcorrencia.js" />
/// <reference path="../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="GrupoPessoa.js" />

var BuscarTipoOcorrencia = function (knout, callbackRetorno, knoutCarga, finalidades, origemOcorrencia, ocultarTiposQueNaoEstaoContratoFrete, basicGrid, knoutTipoOcorrenciaControleEntrega, tipoOcorrenciaControleEntrega, somenteOcorrenciasQueNaoUtilizamControleEntrega, filtroSituacao, naoPermitirQueTransportadorSelecioneTipoOcorrencia, naoUtilizarFlagsControleEntrega, filtrarPorTipoOcorrenciaPermitidaNoPortalDoCliente) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    if (tipoOcorrenciaControleEntrega == null)
        tipoOcorrenciaControleEntrega = false;

    if (somenteOcorrenciasQueNaoUtilizamControleEntrega == null)
        somenteOcorrenciasQueNaoUtilizamControleEntrega = false;

    if (naoPermitirQueTransportadorSelecioneTipoOcorrencia == null)
        naoPermitirQueTransportadorSelecioneTipoOcorrencia = false;

    // EnumFinalidadeTipoOcorrencia
    if (!finalidades)
        finalidades = [""]; // Ambos

    if (!origemOcorrencia)
        origemOcorrencia = "";

    if (typeof ocultarTiposQueNaoEstaoContratoFrete != "boolean")
        ocultarTiposQueNaoEstaoContratoFrete = false;
    var exibirSituacao = false;
    if (filtroSituacao != null)
        exibirSituacao = filtroSituacao;

    if (naoUtilizarFlagsControleEntrega == null)
        naoUtilizarFlagsControleEntrega = false;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: exibirSituacao ? 9 : 12 });
        this.Ativo = PropertyEntity({ options: _statusPesquisa, val: ko.observable(1), def: 1, visible: exibirSituacao, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), col: exibirSituacao ? 3 : 0 });
        this.FiltrarContratoFrete = PropertyEntity({ val: ko.observable(ocultarTiposQueNaoEstaoContratoFrete), def: ocultarTiposQueNaoEstaoContratoFrete, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.SomenteOcorrenciasQueNaoUtilizamControleEntrega = PropertyEntity({ val: ko.observable(somenteOcorrenciasQueNaoUtilizamControleEntrega), def: somenteOcorrenciasQueNaoUtilizamControleEntrega, visible: false, text: Localization.Resources.Consultas.TipoOcorrencia.SomenteOcorrenciasQueNaoUtilizamControleEntrega });
        this.OrigemOcorrencia = PropertyEntity({ val: ko.observable(origemOcorrencia), def: origemOcorrencia, visible: false, text: Localization.Resources.Consultas.TipoOcorrencia.OrigemOcorrencia.getFieldDescription() });
        this.Finalidades = PropertyEntity({ val: ko.observable(JSON.stringify(finalidades)), def: JSON.stringify(finalidades), visible: false, text: Localization.Resources.Consultas.TipoOcorrencia.Finalidades });
        this.GrupoPessoas = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.TipoOcorrencia.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.TipoAplicacaoColetaEntrega = PropertyEntity({ val: ko.observable(EnumTipoAplicacaoColetaEntrega.Todos), options: EnumTipoAplicacaoColetaEntrega.obterOpcoesPesquisa(), def: EnumTipoAplicacaoColetaEntrega.Todos, text: Localization.Resources.Consultas.TipoOcorrencia.AplicacaoDaRegraPara, visible: false });
        this.TipoOcorrenciaControleEntrega = PropertyEntity({ val: ko.observable(tipoOcorrenciaControleEntrega), def: tipoOcorrenciaControleEntrega, visible: false, text: "TipoOcorrenciaControleEntrega: " });
        this.NaoPermitirQueTransportadorSelecioneTipoOcorrencia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(naoPermitirQueTransportadorSelecioneTipoOcorrencia), def: naoPermitirQueTransportadorSelecioneTipoOcorrencia, visible: false, text: "" });
        this.NaoUtilizarFlagsControleEntrega = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(naoUtilizarFlagsControleEntrega), def: naoUtilizarFlagsControleEntrega, visible: false, text: "" });
        this.FiltrarApenasOcorrenciasPermitidasNoPortalDoCliente = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(filtrarPorTipoOcorrenciaPermitidaNoPortalDoCliente), def: filtrarPorTipoOcorrenciaPermitidaNoPortalDoCliente, visible: false, text: "" });

        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoOcorrencia.BuscarTiposDeOcorrencias, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoOcorrencia.TipoDeOcorrencia, type: types.local });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.TipoOcorrencia.Carga.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }


    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = function () {
        if (knoutCarga != null) {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        }
        if (knoutTipoOcorrenciaControleEntrega != null) {
            knoutOpcoes.TipoAplicacaoColetaEntrega.val(knoutTipoOcorrenciaControleEntrega.val());
        }
    }

    //var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    });

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoOcorrencia/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoOcorrencia/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
    })
}