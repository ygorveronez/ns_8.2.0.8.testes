/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />

var _movimentacaoAreaContainerConfirmacao;

var MovimentacaoAreaContainerConfirmacao = function () {
    this.CodigoEntrega = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ColetaDeContainer = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Container.getRequiredFieldDescription(), required: true, idBtnSearch: guid(), visible: this.ColetaDeContainer.val, enable: ko.observable(true) });
    this.ContainerTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*" + Localization.Resources.Cargas.Carga.ContainerTipo, idBtnSearch: guid(), required: true, enable: false });
    this.DataInicioEntregaInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Inicio.getRequiredFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(true) });
    this.Local = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*", idBtnSearch: guid(), enable: false, visible: ko.observable(false) });
    this.DataColetaContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataColetaContainer.getRequiredFieldDescription(), type: types.date, getType: typesKnockout.date, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), required: false, visible: ko.observable(false) });
    this.DataEntregaInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Fim.getRequiredFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(true) });
    this.AreaRedex = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AreasRedex.getRequiredFieldDescription(), val: ko.observable(true), enable: ko.observable(true), required: false, options: ko.observable([]), def: 1, visible: ko.observable(true) });

    this.ConfirmarEntrega = PropertyEntity({ eventClick: confirmarEntregaOuColetaContainerClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Confirmar), visible: ko.observable(true) });
}

function loadMovimentacaoAreaContainerConfirmacao() {
    _movimentacaoAreaContainerConfirmacao = new MovimentacaoAreaContainerConfirmacao();
    KoBindings(_movimentacaoAreaContainerConfirmacao, "knockoutMovimentacaoAreaContainerConfirmacao");

    BuscarContainers(_movimentacaoAreaContainerConfirmacao.Container, null, null, true, cadastrarNovoContainerClick, _movimentacaoAreaContainerConfirmacao.ContainerTipo, _movimentacaoAreaContainerConfirmacao.Local);
}

//#region Eventos

function confirmarEntregaOuColetaContainerClick() {
    if (!ValidarCamposObrigatorios(_movimentacaoAreaContainerConfirmacao)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var data = {
        Codigo: _movimentacaoAreaContainerConfirmacao.CodigoEntrega.val(),
        DataInicioEntregaInformada: _movimentacaoAreaContainerConfirmacao.DataInicioEntregaInformada.val(),
        DataEntregaInformada: _movimentacaoAreaContainerConfirmacao.DataEntregaInformada.val(),
        IniciarViagemAutomaticamente: true,
        ColetaContainer: _movimentacaoAreaContainerConfirmacao.Container.codEntity(),
        DataColetaContainer: _movimentacaoAreaContainerConfirmacao.DataColetaContainer.val(),
        AreaRedex: _movimentacaoAreaContainerConfirmacao.AreaRedex.val()
    };

    executarReST("ControleEntregaEntrega/ConfirmarEntrega", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmação realizada com sucesso.");

                Global.fecharModal("modalMovimentacaoAreaContainerConfirmacao");

                buscarMovimentacaoAreaContainer(1, false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//#endregion

//#region Métodos Privados

function cadastrarNovoContainerClick() {
    var idDiv = guid();
    var knoutCadastro = idDiv + "_knockoutCadastroContainer";
    var modalCadastro = idDiv + "divModalCadastrarContainer";

    var fnPreecherRetornoSelecao = function (knout, e, idDiv, knoutOpcoes) {
        knout.codEntity(e.Codigo);
        knout.val(e.Numero);
        knoutOpcoes.Descricao.val(knoutOpcoes.Descricao.def);        
        Global.fecharModal(idDiv);
        Global.setarFocoProximoCampo(knout.id);
    }

    $.get("Content/Static/Consultas/Cadastros/Container.html?dyn=" + guid(), function (data) {
        var html = data.replace(/#knockoutCadastroContainer/g, knoutCadastro).replace(/#divModalCadastrarContainer/g, modalCadastro);

        $('#js-page-content').append(html);
        var container = new CadastroContainer();

        container.Cancelar.eventClick = function (e) {            
            Global.fecharModal(modalCadastro);
        };

        container.Adicionar.eventClick = function (e) {
            Salvar(e, "Container/Adicionar", function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        Global.fecharModal(modalCadastro);
                        fnPreecherRetornoSelecao(_containerCarga.Container, arg.Data, idDiv, knoutOpcoes);
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        };

        KoBindings(container, knoutCadastro, false);

        new BuscarTiposContainer(container.TipoContainer);

        Global.abrirModal(modalCadastro);

    });
}

function abrirModalMovimentacaoAreaContainerConfirmacao(e) {
    _movimentacaoAreaContainerConfirmacao.CodigoEntrega.val(e.Codigo.val);
    console.log(e);

    if (e.TipoContainerCarga.val > 0) {
        _movimentacaoAreaContainerConfirmacao.ContainerTipo.codEntity(e.TipoContainerCarga.val);
        _movimentacaoAreaContainerConfirmacao.ContainerTipo.val(e.DescricaoTipoContainerCarga.val);
    } else {
        _movimentacaoAreaContainerConfirmacao.ContainerTipo.required = false;
    }

    if (e.LocalRetiradaContainer.val > 0) {
        _movimentacaoAreaContainerConfirmacao.Local.codEntity(e.LocalRetiradaContainer.val);
        _movimentacaoAreaContainerConfirmacao.Local.val(e.LocalRetiradaContainer.val);
    }

    if (e.Armador.val) {
        _movimentacaoAreaContainerConfirmacao.DataColetaContainer.visible(true);
        _movimentacaoAreaContainerConfirmacao.DataColetaContainer.required = true;
    } else {
        _movimentacaoAreaContainerConfirmacao.DataColetaContainer.visible(false);
        _movimentacaoAreaContainerConfirmacao.DataColetaContainer.required = false;
    }

    if (e.ClientePossuiAreaRedex.val) {
        _movimentacaoAreaContainerConfirmacao.AreaRedex.visible(true);
        _movimentacaoAreaContainerConfirmacao.AreaRedex.required = true;
        _movimentacaoAreaContainerConfirmacao.AreaRedex.options(e.AreasRedex.val);
    } else {
        _movimentacaoAreaContainerConfirmacao.AreaRedex.visible(false);
        _movimentacaoAreaContainerConfirmacao.AreaRedex.required = false;
        _movimentacaoAreaContainerConfirmacao.AreaRedex.options([]);
    }

    _movimentacaoAreaContainerConfirmacao.Container.visible(e.ColetaDeContainer.val);
    _movimentacaoAreaContainerConfirmacao.Container.required = e.ColetaDeContainer.val;
    _movimentacaoAreaContainerConfirmacao.Container.enable(e.CodigoContainerRetirar.val == 0);
    _movimentacaoAreaContainerConfirmacao.Container.codEntity(e.CodigoContainerRetirar.val);
    _movimentacaoAreaContainerConfirmacao.Container.val(e.DescricaoContainerRetirar.val);

    $('#modalMovimentacaoAreaContainerConfirmacao').modal("show")
        .one('hidden.bs.modal', function () {
            LimparCampos(_movimentacaoAreaContainerConfirmacao);
        });
}

//#endregion