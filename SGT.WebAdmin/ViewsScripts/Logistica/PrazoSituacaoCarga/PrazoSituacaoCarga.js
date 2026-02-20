/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridPrazoSituacaoCarga;
var _prazoSituacaoCarga;
var _pesquisaPrazoSituacaoCarga;

var PesquisaPrazoSituacaoCarga = function () {

    this.SituacaoCarga = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoCargaJanelaCarregamento.obterOpcoesPesquisa(), def: "", text: "Situação da Carga:", issue: 341 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPrazoSituacaoCarga.CarregarGrid();
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
}

var PrazoSituacaoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tempo = PropertyEntity({ text: "*Tempo:", required: true, maxlength: 5 });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.AgAprovacaoComercial), options: EnumSituacaoCargaJanelaCarregamento.obterOpcoes(), def: EnumSituacaoCargaJanelaCarregamento.AgAprovacaoComercial, text: "*Situação da Carga:", issue: 341 });
    this.NotificarTransportadorPorEmailAoEsgotarPrazo = PropertyEntity({ text: "Notificar transportador por e-mail ao esgotar o prazo", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false), def: false });

    this.SituacaoCarga.val.subscribe(situacaoCargaChange);

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadPrazoSituacaoCarga() {

    _prazoSituacaoCarga = new PrazoSituacaoCarga();
    KoBindings(_prazoSituacaoCarga, "knockoutCadastroPrazoSituacaoCarga");

    HeaderAuditoria("PrazoSituacaoCarga", _prazoSituacaoCarga);

    _pesquisaPrazoSituacaoCarga = new PesquisaPrazoSituacaoCarga();
    KoBindings(_pesquisaPrazoSituacaoCarga, "knockoutPesquisaPrazoSituacaoCarga", _pesquisaPrazoSituacaoCarga.Pesquisar.id);

    buscarPrazosSituacaoCarga();

    $("#" + _prazoSituacaoCarga.Tempo.id).mask("00:00", { selectOnFocus: true, clearIfNotMatch: true });
}

function adicionarClick(e, sender) {
    Salvar(e, "PrazoSituacaoCarga/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridPrazoSituacaoCarga.CarregarGrid();
                limparCamposPrazoSituacaoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "PrazoSituacaoCarga/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridPrazoSituacaoCarga.CarregarGrid();
                limparCamposPrazoSituacaoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o prazo para a situação da carga?", function () {
        ExcluirPorCodigo(_prazoSituacaoCarga, "PrazoSituacaoCarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridPrazoSituacaoCarga.CarregarGrid();
                    limparCamposPrazoSituacaoCarga();
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
    limparCamposPrazoSituacaoCarga();
}

function situacaoCargaChange() {
    if (_prazoSituacaoCarga.SituacaoCarga.val() == EnumSituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador)
        _prazoSituacaoCarga.NotificarTransportadorPorEmailAoEsgotarPrazo.visible(true);
    else {
        _prazoSituacaoCarga.NotificarTransportadorPorEmailAoEsgotarPrazo.val(false);
        _prazoSituacaoCarga.NotificarTransportadorPorEmailAoEsgotarPrazo.visible(false);
    }
}

//*******MÉTODOS*******

function editarPrazoSituacaoCarga(tipoCargaGrid) {
    limparCamposPrazoSituacaoCarga();
    _prazoSituacaoCarga.Codigo.val(tipoCargaGrid.Codigo);
    BuscarPorCodigo(_prazoSituacaoCarga, "PrazoSituacaoCarga/BuscarPorCodigo", function (arg) {
        _pesquisaPrazoSituacaoCarga.ExibirFiltros.visibleFade(false);
        _prazoSituacaoCarga.Atualizar.visible(true);
        _prazoSituacaoCarga.Cancelar.visible(true);
        _prazoSituacaoCarga.Excluir.visible(true);
        _prazoSituacaoCarga.Adicionar.visible(false);
    }, null);
}

function buscarPrazosSituacaoCarga() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPrazoSituacaoCarga, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPrazoSituacaoCarga = new GridView(_pesquisaPrazoSituacaoCarga.Pesquisar.idGrid, "PrazoSituacaoCarga/Pesquisa", _pesquisaPrazoSituacaoCarga, menuOpcoes, null);
    _gridPrazoSituacaoCarga.CarregarGrid();
}

function limparCamposPrazoSituacaoCarga() {
    _prazoSituacaoCarga.Atualizar.visible(false);
    _prazoSituacaoCarga.Cancelar.visible(false);
    _prazoSituacaoCarga.Excluir.visible(false);
    _prazoSituacaoCarga.Adicionar.visible(true);
    LimparCampos(_prazoSituacaoCarga);
}
