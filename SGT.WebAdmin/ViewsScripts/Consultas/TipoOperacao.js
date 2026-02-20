/// <reference path="../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="Cliente.js" />

var BuscarTiposOperacao = function (knout, callbackRetorno, knoutGrupoPessoas, knoutPessoa, basicGrid, codigoCarga, knoutContratoFrete, isFiltrarPorConfiguracaoOperadorLogistica, isFiltrarSomenteTipoOperacaoPermiteGerarRedespacho, fnCallbackTomador, _isFiltrarTipoOperacaoPorTransportador, filtroSituacao, knoutTipoCargaEmissao, filtrarTipoOperacaoOcultas, isFiltrarTipoDevolucao) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    if (codigoCarga == null)
        codigoCarga = 0;

    var exibirFiltroSituacao = false;
    if (filtroSituacao != null)
        exibirFiltroSituacao = filtroSituacao;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoOperacao.PesquisarTiposOperacao, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoOperacao.TiposOperacao, type: types.local });
        this.Descricao = PropertyEntity({ col: exibirFiltroSituacao ? 9 : 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Ativo = PropertyEntity({ col: 3, options: _statusPesquisa, val: ko.observable(1), def: 1, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: false });
        this.GrupoPessoas = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: true });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid() });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.ContratoFrete = PropertyEntity({ type: types.map, val: ko.observable(0), visible: false });
        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogistica != false), visible: false });
        this.SomenteTipoOperacaoPermiteGerarRedespacho = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarSomenteTipoOperacaoPermiteGerarRedespacho == true), visible: false });
        this.TipoOperacaoPorTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_isFiltrarTipoOperacaoPorTransportador), visible: false });
        this.TipoCargaEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.PermitirInformarAjudantesNaCarga = PropertyEntity({ visible: false,  val: ko.observable(false) });
        this.ObrigatorioJustificarCustoExtra = PropertyEntity({ visible: false,  val: ko.observable(false) });
        this.LiberarCargaSemPlanejamento = PropertyEntity({ visible: false, val: ko.observable(false) });
        this.FiltrarPorTipoDevolucao = PropertyEntity({getType: typesKnockout.bool, val: ko.observable(isFiltrarTipoDevolucao), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
        this.FiltrarTipoOperacaoOcultas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(filtrarTipoOperacaoOcultas), visible: false });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
   
    if (knoutGrupoPessoas != null ||
        knoutPessoa != null ||
        knoutContratoFrete != null ||
        knoutTipoCargaEmissao != null ||
        fnCallbackTomador instanceof Function) {
        funcaoParamentroDinamico = function () {

            if (knoutTipoCargaEmissao != null) {
                knoutOpcoes.TipoCargaEmissao.codEntity(knoutTipoCargaEmissao.codEntity());
                knoutOpcoes.TipoCargaEmissao.val(knoutTipoCargaEmissao.val());
            }

            if (knoutGrupoPessoas != null) {
                knoutOpcoes.GrupoPessoas.visible = false;
                knoutOpcoes.Pessoa.visible = false;
                knoutOpcoes.GrupoPessoas.codEntity(knoutGrupoPessoas.codEntity());
                knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.val());
            }

            if (knoutPessoa != null) {
                knoutOpcoes.GrupoPessoas.visible = false;
                knoutOpcoes.Pessoa.visible = false;
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }

            if (knoutContratoFrete != null) {
                knoutOpcoes.ContratoFrete.val(knoutContratoFrete.codEntity());
            }

            if (fnCallbackTomador instanceof Function) {
                knoutOpcoes.GrupoPessoas.visible = false;
                knoutOpcoes.Pessoa.visible = false;
                knoutOpcoes.Pessoa.codEntity(fnCallbackTomador().codEntity());
                knoutOpcoes.Pessoa.val(fnCallbackTomador().val());
            }
        };
    }

    if (codigoCarga > 0) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.Carga.codEntity(codigoCarga);
        knoutOpcoes.Carga.val(codigoCarga);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
        new BuscarClientes(knoutOpcoes.Pessoa);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoOperacao/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoOperacao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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