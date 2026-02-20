
// #region Objetos Globais do Arquivo

var _coletaContainer;
var _detalhes
var _cnpjCliente;
var _cpfCnpjArmadorContaner;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ColetaContainer = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Container.getRequiredFieldDescription(), required: true, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.ContainerTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*" + Localization.Resources.Cargas.Carga.ContainerTipo, idBtnSearch: guid(), required: false, enable: false });
    this.Local = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*", idBtnSearch: guid(), enable: false, visible: ko.observable(false) });
    this.DataColetaContainer = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataColetaContainer.getRequiredFieldDescription(), type: types.date, getType: typesKnockout.date, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), required: false, visible: ko.observable(false) });
    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarEntregaColetaContainerClick, text: Localization.Resources.Cargas.ControleEntrega.ConfirmacaoEntrega, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadColetaContainer() {
    _coletaContainer = new ColetaContainer();
    KoBindings(_coletaContainer, "knockouSelecionarColetasContainer");

    new BuscarContainers(_coletaContainer.Container, retornoSelecaoContainer, null, true, cadastrarNovoContainerClick, _coletaContainer.ContainerTipo, _coletaContainer.Local);
}

function retornoSelecaoContainer(data) {
    console.log(data);
    _coletaContainer.Container.codEntity(data.Codigo);
    _coletaContainer.Container.entityDescription(data.Numero);
    _coletaContainer.Container.val(data.Numero);

    _cpfCnpjArmadorContaner = data.Armador;
}

function abrirModalColetaDeContainer(entrega, detalhes) {
    _coletaContainer.Codigo.val(entrega.Codigo.val());
    _cnpjCliente = entrega.CpfCnpjCliente.val();

    if (entrega.TipoContainerCarga.val() > 0) {
        _coletaContainer.ContainerTipo.codEntity(entrega.TipoContainerCarga.val());
        _coletaContainer.ContainerTipo.val(entrega.DescricaoTipoContainerCarga.val());
    }

    if (entrega.LocalRetiradaContainer.val() > 0) {
        _coletaContainer.Local.codEntity(entrega.LocalRetiradaContainer.val());
        _coletaContainer.Local.val(entrega.LocalRetiradaContainer.val());
    }

    if (entrega.CodigoContainer.val() > 0) {
        _coletaContainer.Container.codEntity(entrega.CodigoContainer.val());
        _coletaContainer.Container.val(entrega.DescricaoContainer.val());
        _coletaContainer.Container.enable(false);
    }
    else
        _coletaContainer.Container.enable(true);

    _coletaContainer.DataColetaContainer.visible(entrega.Armador.val());

    _detalhes = detalhes;

    Global.abrirModal("divModalSelecionarColetaContainer");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function confirmarEntregaColetaContainerClick() {
    if (ValidarCamposObrigatorios(_coletaContainer)) {

        Global.fecharModal("divModalSelecionarColetaContainer");

        var msg = Localization.Resources.Cargas.ControleEntrega.ClienteArmadorDiferenteDoArmadorContainer;

        if (_cpfCnpjArmadorContaner > 0) {
            if (_cpfCnpjArmadorContaner != _cnpjCliente) {
                exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, msg, function () {
                    confirmarEntrega(_detalhes, undefined, _coletaContainer.Container.codEntity(), _coletaContainer.DataColetaContainer.val());
                });
            } else {
                confirmarEntrega(_detalhes, undefined, _coletaContainer.Container.codEntity(), _coletaContainer.DataColetaContainer.val());
            }
        } else {
            confirmarEntrega(_detalhes, undefined, _coletaContainer.Container.codEntity(), _coletaContainer.DataColetaContainer.val());
        }
    }
}

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

// #endregion Funções Associadas a Eventos
