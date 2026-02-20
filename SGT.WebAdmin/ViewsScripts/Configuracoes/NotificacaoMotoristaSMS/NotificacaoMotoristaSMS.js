/// <reference path="../../Enumeradores/EnumTipoNotificacaoMotoristaSMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotificacaoMotoristaSMS, _notificacaoMotoristaSMS, _pesquisaNotificacaoMotoristaSMS, _crudNotificacaoMotoristaSMS;

var PesquisaNotificacaoMotoristaSMS = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inatvo"), def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNotificacaoMotoristaSMS.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}
var NotificacaoMotoristaSMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesBooleano("Ativo", "Inatvo"), def: true, text: "*Situação: " });
    this.TipoEnvio = PropertyEntity({ text: "*Tipo de Envio:", options: EnumTipoNotificacaoMotoristaSMS.obterOpcoes(), val: ko.observable(EnumTipoNotificacaoMotoristaSMS.FilaCarregamentoEnvioManual), def: EnumTipoNotificacaoMotoristaSMS.FilaCarregamentoEnvioManual, issue: 0, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "*Mensagem: ", required: true });
    this.NotificacaoSuperApp = PropertyEntity({ text: "Notificação SuperApp Trizy", getType: typesKnockout.bool, val: ko.observable(false) });

    this.TagDataMensagem = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#DataMensagem"); }, type: types.event, text: "Data da Mensagem", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCpfMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#CpfMotorista"); }, type: types.event, text: "CPF do Motorista", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNomeMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#NomeMotorista"); }, type: types.event, text: "Nome do Motorista", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagNumeroCarga = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagPlaca = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#PlacaVeiculo"); }, type: types.event, text: "Placa do veículo", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagTransportadora = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#NomeTransportadora"); }, type: types.event, text: "Transportadora", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagCnpjTransportadora = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#CnpjTransportadora"); }, type: types.event, text: "Cnpj Transportadora", enable: ko.observable(true), visible: ko.observable(true) });
    this.TagDoca = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#Doca"); }, type: types.event, text: "Doca", enable: ko.observable(true), visible: ko.observable(true) });

    this.NotificacaoSuperApp.val.subscribe((novoValor) => controlarVisibilidadeTags(!novoValor));
}

var CRUDNotificacaoMotoristaSMS = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENT
function loadNotificacaoMotoristaSMS() {

    _notificacaoMotoristaSMS = new NotificacaoMotoristaSMS();

    KoBindings(_notificacaoMotoristaSMS, "knockoutCadastroConfiguracoesDeNotificacaoMotoristaSMS");

    _crudNotificacaoMotoristaSMS = new CRUDNotificacaoMotoristaSMS();
    KoBindings(_crudNotificacaoMotoristaSMS, "knockoutCRUDConfiguracoesDeNotificacaoMotoristaSMS");

    _pesquisaNotificacaoMotoristaSMS = new PesquisaNotificacaoMotoristaSMS();
    KoBindings(_pesquisaNotificacaoMotoristaSMS, "knockoutPesquisaConfiguracoesDeNotificacaoMotoristaSMS", false, _pesquisaNotificacaoMotoristaSMS.Pesquisar.id);

    HeaderAuditoria("NotificacaoMotoristaSMS", _notificacaoMotoristaSMS, "Codigo");

    buscarNotificacaoMotoristaSMS();
}

function adicionarClick(e, sender) {
    Salvar(_notificacaoMotoristaSMS, "NotificacaoMotoristaSMS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridNotificacaoMotoristaSMS.CarregarGrid();
                limparCamposNotificacaoMotoristaSMS();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_notificacaoMotoristaSMS, "NotificacaoMotoristaSMS/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            _gridNotificacaoMotoristaSMS.CarregarGrid();
            limparCamposNotificacaoMotoristaSMS();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a notificação? " + _notificacaoMotoristaSMS.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_notificacaoMotoristaSMS, "NotificacaoMotoristaSMS/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridNotificacaoMotoristaSMS.CarregarGrid();
                    limparCamposNotificacaoMotoristaSMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposNotificacaoMotoristaSMS();
}

//*******MÉTODOS*******

function buscarNotificacaoMotoristaSMS() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarNotificacaoMotoristaSMS, tamanho: "9", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridNotificacaoMotoristaSMS = new GridView(_pesquisaNotificacaoMotoristaSMS.Pesquisar.idGrid, "NotificacaoMotoristaSMS/Pesquisa", _pesquisaNotificacaoMotoristaSMS, menuOpcoes, null);
    _gridNotificacaoMotoristaSMS.CarregarGrid();
}

function editarNotificacaoMotoristaSMS(arquivoGrid) {
    limparCamposNotificacaoMotoristaSMS();
    _notificacaoMotoristaSMS.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_notificacaoMotoristaSMS, "NotificacaoMotoristaSMS/BuscarPorCodigo", function (arg) {
        _pesquisaNotificacaoMotoristaSMS.ExibirFiltros.visibleFade(false);
        _crudNotificacaoMotoristaSMS.Atualizar.visible(true);
        _crudNotificacaoMotoristaSMS.Cancelar.visible(true);
        _crudNotificacaoMotoristaSMS.Excluir.visible(true);
        _crudNotificacaoMotoristaSMS.Adicionar.visible(false);

    }, null);
}

function limparCamposNotificacaoMotoristaSMS() {
    _crudNotificacaoMotoristaSMS.Atualizar.visible(false);
    _crudNotificacaoMotoristaSMS.Cancelar.visible(false);
    _crudNotificacaoMotoristaSMS.Excluir.visible(false);
    _crudNotificacaoMotoristaSMS.Adicionar.visible(true);

    LimparCampos(_notificacaoMotoristaSMS);
}

function controlarVisibilidadeTags(visivel) {
    _notificacaoMotoristaSMS.TagDataMensagem.visible(visivel);
    _notificacaoMotoristaSMS.TagCpfMotorista.visible(visivel);
    _notificacaoMotoristaSMS.TagNomeMotorista.visible(visivel);
    _notificacaoMotoristaSMS.TagNumeroCarga.visible(visivel);
    _notificacaoMotoristaSMS.TagPlaca.visible(visivel);
    _notificacaoMotoristaSMS.TagTransportadora.visible(visivel);
    _notificacaoMotoristaSMS.TagCnpjTransportadora.visible(visivel);
    _notificacaoMotoristaSMS.TagDoca.visible(visivel);
}
