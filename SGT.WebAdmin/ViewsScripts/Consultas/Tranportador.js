/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

let BuscarTransportadores = function (knout, callbackRetorno, menuOpcoes, somenteProducao, basicGrid, knoutFilial, knoutLocalidadeFilialTransportador, statusTodos, knoutRaizCnpj, isFiltrarPorConfiguracaoOperadorLogistica, consultarSomenteAssociadoAoUsuario, consultarSomenteEmpresaNaoTransportadoraPadraoContratacao, codigoPedidoBase, somenteTransportadoresPermitidosCadastroAgendamentoColeta, somenteTransportadoresManuais, limiteRegistros, knockoutEmpresaMatriz, telaMontagemCargaMapa, codigosTransportadores) {

    let idDiv = guid();
    let GridConsulta;
    let buscaLocalidades;
    let buscaTipoOperacao;

    if (somenteProducao == null)
        somenteProducao = false;

    let multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    let tituloModal = Localization.Resources.Consultas.Transportador.PesquisarTransportador;
    let tituloGrid = Localization.Resources.Consultas.Transportador.Transportadores;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        tituloModal = Localization.Resources.Consultas.Transportador.PesquisarEmpresaFilial;
        tituloGrid = Localization.Resources.Consultas.Transportador.EmpresasFiliais;
    }

    if (somenteTransportadoresManuais) {
        tituloGrid = Localization.Resources.Consultas.Transportador.Transportadores + " " + Localization.Resources.Consultas.Transportador.SomenteTransportadoresConfigurados
    }

    let _consultarSomenteAssociadoAoUsuario = false;
    if (consultarSomenteAssociadoAoUsuario != null)
        _consultarSomenteAssociadoAoUsuario = consultarSomenteAssociadoAoUsuario;

    consultarSomenteEmpresaNaoTransportadoraPadraoContratacao = (consultarSomenteEmpresaNaoTransportadoraPadraoContratacao ? consultarSomenteEmpresaNaoTransportadoraPadraoContratacao : false);
    codigoPedidoBase = (codigoPedidoBase ? codigoPedidoBase : 0);

    const vemTelaMontagemCargaMapa = (telaMontagemCargaMapa || false);

    let OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloModal, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGrid, type: types.local });

        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Transportador.RazaoSocial.getFieldDescription() });
        this.CNPJ = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Transportador.CNPJ.getFieldDescription(), getType: typesKnockout.cnpj });
        this.NomeFantasia = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Transportador.NomeFantasia.getFieldDescription() });
        this.CodigoIntegracao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Transportador.CodigoIntegracao.getFieldDescription(), getType: typesKnockout.string });
        this.Localidade = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Localidade.DescricaoLocalidade.getFieldDescription(), issue: 16, idBtnSearch: guid(), visible: ko.observable(true) });
        this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Todos), def: EnumEstado.Todos, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription() });
        this.TipoOperacao = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Transportador.TipoOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
        this.GrupoTransportador = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Transportador.GrupoTransportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

        this.SomenteProducao = PropertyEntity({ getType: typesKnockout.bool, visible: false, val: ko.observable(somenteProducao) });
        this.StatusTodos = PropertyEntity({ getType: typesKnockout.bool, visible: false, val: ko.observable(statusTodos) });
        this.Filial = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.EmpresaMatriz = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.LocalidadeFilialTransportador = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.RaizCnpj = PropertyEntity({ visible: false });
        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogistica != false), visible: false });
        this.ConsultarSomenteAssociadoAoUsuario = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_consultarSomenteAssociadoAoUsuario != false), visible: false });
        this.SomenteEmpresaNaoTransportadoraPadraoContratacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(consultarSomenteEmpresaNaoTransportadoraPadraoContratacao), visible: false });
        this.CodigoPedidoBase = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(codigoPedidoBase), visible: false });
        this.SomenteTransportadoresPermitidosCadastroAgendamentoColeta = PropertyEntity({ getType: typesKnockout.bool, visible: false, val: ko.observable(somenteTransportadoresPermitidosCadastroAgendamentoColeta) });
        this.SomenteTransportadoresManuais = PropertyEntity({ getType: typesKnockout.bool, visible: false, val: ko.observable(somenteTransportadoresManuais) });
        this.TelaMontagemCargaMapa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(vemTelaMontagemCargaMapa), visible: false })
        this.CodigosTransportadores = PropertyEntity({ visible: false, type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };


    let knoutOpcoes = new OpcoesKnout();
    let funcaoParametroDinamico = null;

    if (knoutFilial || knoutLocalidadeFilialTransportador || knoutRaizCnpj || codigoPedidoBase || knockoutEmpresaMatriz || codigosTransportadores) {
        funcaoParametroDinamico = function () {
            if (knoutFilial) {
                knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
                knoutOpcoes.Filial.val(knoutFilial.val());
            }

            if (knoutLocalidadeFilialTransportador) {
                knoutOpcoes.LocalidadeFilialTransportador.codEntity(knoutLocalidadeFilialTransportador.codEntity());
                knoutOpcoes.LocalidadeFilialTransportador.val(knoutLocalidadeFilialTransportador.val());
            }

            if (knoutRaizCnpj)
                knoutOpcoes.RaizCnpj.val(knoutRaizCnpj.val());

            if (codigoPedidoBase)
                knoutOpcoes.CodigoPedidoBase.val(codigoPedidoBase.val());

            if (codigosTransportadores)
                knoutOpcoes.CodigosTransportadores.val(JSON.stringify(codigosTransportadores.val()));

            if (knockoutEmpresaMatriz) {
                knoutOpcoes.EmpresaMatriz.codEntity(knockoutEmpresaMatriz.codEntity());
                knoutOpcoes.EmpresaMatriz.val(knockoutEmpresaMatriz.val());
            }

            if (knoutFilial && vemTelaMontagemCargaMapa) {
                knoutOpcoes.TelaMontagemCargaMapa.val(vemTelaMontagemCargaMapa)
            }
        };
    }

    function PreencherCamposBusca() {
        buscaLocalidades = new BuscarLocalidades(knoutOpcoes.Localidade);
        buscaTipoOperacao = new BuscarTiposOperacao(knoutOpcoes.TipoOperacao);
        buscaGrupoTransportador = new BuscarGruposTransportadores(knoutOpcoes.GrupoTransportador);
    }
    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, PreencherCamposBusca, null, null, null, null, limiteRegistros);

    let callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    let opcoes = divBusca.OpcaoPadrao(callback);
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    let url = "Transportador/Pesquisa";

    if (multiplaEscolha) {
        let objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid, limiteRegistros);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, opcoes, null, null, null, null, null, null, limiteRegistros);
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

    this.Destroy = function () {
        if (buscaLocalidades)
            buscaLocalidades.Destroy();
        if (buscaTipoOperacao)
            buscaTipoOperacao.Destroy();

        divBusca.Destroy();
    };
};