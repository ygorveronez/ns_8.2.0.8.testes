/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Enumeradores/EnumStatusColetaContainer.js" />

var KnoutCadastrarContainer = function () {
    this.Numero = PropertyEntity({ text: Localization.Resources.Consultas.Container.Numero.getRequiredFieldDescription(), maxlength: 20, required: ko.observable(true), val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Consultas.Container.CodigoIntegracao.getFieldDescription(), required: ko.observable(false), maxlength: 50, val: ko.observable("") });
    this.Tara = PropertyEntity({ text: Localization.Resources.Consultas.Container.Tara, getType: typesKnockout.decimal, maxlength: 18, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });
    this.TipoContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Container.TipoDeContainer, idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoPropriedade = PropertyEntity({ val: ko.observable(EnumTipoPropriedadeContainer.Soc), options: EnumTipoPropriedadeContainer.obterOpcoes(), def: EnumTipoPropriedadeContainer.Soc, text: Localization.Resources.Consultas.Container.Propriedade, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Container.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Container.Cancelar, visible: ko.observable(true) });
}

var BuscarContainers = function (knout, callbackRetorno, basicGrid, permitirAddContainer, callbackCadastrar, knoutContainerTipo, knoutLocal, statusColetaContainer) {
    var idDiv = guid();
    var gridConsulta;
    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    if (permitirAddContainer == null)
        permitirAddContainer = false;

    if (!statusColetaContainer)
        statusColetaContainer = EnumStatusColetaContainer.Todas;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Container.BuscaDeContainers, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Container.DescricaoContainer, type: types.local });
        this.Status = PropertyEntity({ text: Localization.Resources.Consultas.Container.Status.getFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });
        this.StatusColetaContainer = PropertyEntity({ text: Localization.Resources.Consultas.Container.Status.getFieldDescription(), val: ko.observable(statusColetaContainer), options: EnumStatusColetaContainer.obterOpcoesPesquisa(), def: statusColetaContainer, visible: false });
        this.Local = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Container.Local, idBtnSearch: guid(), visible: false });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.Container.Descricao.getFieldDescription() });
        this.Numero = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Container.Numero.getFieldDescription() });
        this.CodigoIntegracao = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.Container.CodigoIntegracao.getFieldDescription() });
        this.ContainerTipo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Container.TipoDeContainer, idBtnSearch: guid(), visible: knoutContainerTipo == null });

        this.AdicionarContainer = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Container.AdicionarNovoContainer, visible: permitirAddContainer, cssClass: "btn btn-default", icon: " " });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.Container.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = function () {
        if (knoutContainerTipo) {
            knoutOpcoes.ContainerTipo.codEntity(knoutContainerTipo.codEntity());
            knoutOpcoes.ContainerTipo.val(knoutContainerTipo.val());
        }
        if (knoutLocal) {
            knoutOpcoes.Local.codEntity(knoutLocal.codEntity());
            knoutOpcoes.Local.val(knoutLocal.val());
        }
    }

    var fnPreecherRetornoSelecaocontainer = function (knout, e, idDiv, knoutOpcoes) {
        knout.codEntity(e.Codigo);
        knout.entityDescription(e.Numero);
        knout.val(e.Numero);

        knoutOpcoes.Descricao.val(knoutOpcoes.Descricao.def);
        divBusca.CloseModal();
    }

    if (permitirAddContainer) {

        var fnAbrirModalAddContainer = function (e, knoutCadastro, modalCadastro, container) {

            container.Numero.val(e.Numero.val());
            container.CodigoIntegracao.val(e.CodigoIntegracao.val());
            container.Tara.val("0,00");
            container.TipoContainer.codEntity(0);
            container.TipoContainer.val("");
            container.TipoPropriedade.val(EnumTipoPropriedadeContainer.Soc);
                        
            Global.abrirModal(modalCadastro);
        }

        var knoutCadastro = idDiv + "_knockoutCadastroContainer";
        var modalCadastro = idDiv + "divModalCadastrarContainer";
        $.get("Content/Static/Consultas/Cadastros/Container.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroContainer/g, knoutCadastro).replace(/#divModalCadastrarContainer/g, modalCadastro);
            $('body #js-page-content').append(html);
            var container = new KnoutCadastrarContainer();

            container.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            container.Adicionar.eventClick = function (e) {
                Salvar(e, "Container/Adicionar", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.Container.Sucesso, Localization.Resources.Consultas.Container.CadastradoComSucesso);
                            fnPreecherRetornoSelecaocontainer(knout, arg.Data, idDiv, knoutOpcoes);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Consultas.Container.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.Container.Falha, arg.Msg);
                    }
                });
            };

            KoBindings(container, knoutCadastro, false);

            new BuscarTiposContainer(container.TipoContainer);

            knoutOpcoes.AdicionarContainer.eventClick = function (e) {
                fnAbrirModalAddContainer(e, knoutCadastro, modalCadastro, container);
            }
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        new BuscarTiposContainer(knoutOpcoes.ContainerTipo);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
        //fnPreecherRetornoSelecaocontainer(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Container/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Container/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 0) {
                if (knoutOpcoes.Descricao.val() != null && callbackCadastrar != null) {
                    exibirConfirmacao(Localization.Resources.Consultas.Container.Confirmacao, Localization.Resources.Consultas.Container.ContainerPesquisadoPeloNumero.getFieldDescription() + Localization.Resources.Consultas.Container.NaoFoiEncontradoDesejaCadastraLo.format(knoutOpcoes.Descricao.val()), function () {
                        callbackCadastrar();
                    });
                } else {
                    divBusca.OpenModal();
                }
            }
            else if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};