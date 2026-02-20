/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTransportadoresElectrolux = function (knout, callbackRetorno, menuOpcoes, somenteProducao, basicGrid, knoutFilial, knoutLocalidadeFilialTransportador, statusTodos, knoutRaizCnpj, isFiltrarPorConfiguracaoOperadorLogistica, consultarSomenteAssociadoAoUsuario, consultarSomenteEmpresaNaoTransportadoraPadraoContratacao, codigoPedidoBase, somenteTransportadoresPermitidosCadastroAgendamentoColeta, somenteTransportadoresManuais, limiteRegistros, knockoutEmpresaMatriz, telaMontagemCargaMapa) {

    var idDiv = guid();
    var GridConsulta;
    let buscaLocalidades;

    if (somenteProducao == null)
        somenteProducao = false;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var tituloModal = Localization.Resources.Consultas.Transportador.PesquisarTransportador;
    var tituloGrid = Localization.Resources.Consultas.Transportador.Transportadores;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        tituloModal = Localization.Resources.Consultas.Transportador.PesquisarEmpresaFilial;
        tituloGrid = Localization.Resources.Consultas.Transportador.EmpresasFiliais;
    }

    if (somenteTransportadoresManuais) {
        tituloGrid = Localization.Resources.Consultas.Transportador.Transportadores + " " + Localization.Resources.Consultas.Transportador.SomenteTransportadoresConfigurados
    }

    var _consultarSomenteAssociadoAoUsuario = false;
    if (consultarSomenteAssociadoAoUsuario != null)
        _consultarSomenteAssociadoAoUsuario = consultarSomenteAssociadoAoUsuario;

    consultarSomenteEmpresaNaoTransportadoraPadraoContratacao = (consultarSomenteEmpresaNaoTransportadoraPadraoContratacao ? consultarSomenteEmpresaNaoTransportadoraPadraoContratacao : false);
    codigoPedidoBase = (codigoPedidoBase ? codigoPedidoBase : 0);

    const vemTelaMontagemCargaMapa = (telaMontagemCargaMapa || false);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: tituloModal, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGrid, type: types.local });

        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Consultas.Transportador.RazaoSocial.getFieldDescription() });
        this.CNPJ = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Transportador.CNPJ.getFieldDescription(), getType: typesKnockout.cnpj });
        this.NomeFantasia = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Transportador.NomeFantasia.getFieldDescription() });
        this.CodigoIntegracao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Transportador.CodigoIntegracao.getFieldDescription(), getType: typesKnockout.string });
        this.Localidade = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Localidade.DescricaoLocalidade.getFieldDescription(), issue: 16, idBtnSearch: guid(), visible: ko.observable(true) });
        this.Estado = PropertyEntity({ col: 3, val: ko.observable(EnumEstado.Todos), def: EnumEstado.Todos, options: EnumEstado.obterOpcoesPesquisaSemExterior(), text: Localization.Resources.Consultas.Localidade.UF.getFieldDescription() });

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

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };


    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutFilial || knoutLocalidadeFilialTransportador || knoutRaizCnpj || codigoPedidoBase || knockoutEmpresaMatriz) {
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
    }
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, false, PreencherCamposBusca, null, null, null, null, limiteRegistros);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    var opcoes = divBusca.OpcaoPadrao(callback);
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    var url = "Transportador/PesquisaElectrolux";

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, opcoes, null, null, null, null, null, null, limiteRegistros);

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

        divBusca.Destroy();
    };
};