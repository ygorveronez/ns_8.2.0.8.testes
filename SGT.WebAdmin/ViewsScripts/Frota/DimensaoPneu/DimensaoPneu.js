/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDDimensaoPneu;
var _dimensaoPneu;
var _pesquisaDimensaoPneu;
var _gridDimensaoPneu;

/*
 * Declaração das Classes
 */

var DimensaoPneu = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Aplicacao = PropertyEntity({ text: "*Aplicacao:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 50, enable: false, required: true });
    this.Aro = PropertyEntity({ text: "*Aro:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 10, required: true });
    this.Largura = PropertyEntity({ text: "*Largura:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, required: true });
    this.Perfil = PropertyEntity({ text: "*Perfil:", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 7, required: true });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Radial = PropertyEntity({ text: "É Radial?", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Aro.val.subscribe(atualizarCampoAplicacao);
    this.Largura.val.subscribe(atualizarCampoAplicacao);
    this.Perfil.val.subscribe(atualizarCampoAplicacao);
    this.Radial.val.subscribe(atualizarCampoAplicacao);
}

var CRUDDimensaoPneu = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaDimensaoPneu = function () {
    this.Aplicacao = PropertyEntity({ text: "Aplicação:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridDimensaoPneu, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridDimensaoPneu() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "DimensaoPneu/ExportarPesquisa", titulo: "Dimensão do Pneu" };

    _gridDimensaoPneu = new GridViewExportacao(_pesquisaDimensaoPneu.Pesquisar.idGrid, "DimensaoPneu/Pesquisa", _pesquisaDimensaoPneu, menuOpcoes, configuracoesExportacao);
    _gridDimensaoPneu.CarregarGrid();
}

function loadDimensaoPneu() {
    _dimensaoPneu = new DimensaoPneu();
    KoBindings(_dimensaoPneu, "knockoutDimensaoPneu");

    HeaderAuditoria("DimensaoPneu", _dimensaoPneu);

    _CRUDDimensaoPneu = new CRUDDimensaoPneu();
    KoBindings(_CRUDDimensaoPneu, "knockoutCRUDDimensaoPneu");

    _pesquisaDimensaoPneu = new PesquisaDimensaoPneu();
    KoBindings(_pesquisaDimensaoPneu, "knockoutPesquisaDimensaoPneu", false, _pesquisaDimensaoPneu.Pesquisar.id);

    loadGridDimensaoPneu();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_dimensaoPneu, "DimensaoPneu/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridDimensaoPneu();
                limparCamposDimensaoPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_dimensaoPneu, "DimensaoPneu/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridDimensaoPneu();
                limparCamposDimensaoPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposDimensaoPneu();
}

function editarClick(registroSelecionado) {
    limparCamposDimensaoPneu();

    _dimensaoPneu.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_dimensaoPneu, "DimensaoPneu/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaDimensaoPneu.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_dimensaoPneu, "DimensaoPneu/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridDimensaoPneu();
                    limparCamposDimensaoPneu();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function atualizarCampoAplicacao() {
    var aro = parseFloat(_dimensaoPneu.Aro.val().replace(/\./g, "").replace(",", "."));

    if (isNaN(aro) || (aro == 0))
        aro = "";
    else
        aro = ((aro % 1) == 0) ? parseInt(aro) : aro;

    var largura = isNaN(parseInt(_dimensaoPneu.Largura.val())) || _dimensaoPneu.Largura.val() == 0 ? "" : _dimensaoPneu.Largura.val();
    var perfil = isNaN(parseInt(_dimensaoPneu.Perfil.val())) || _dimensaoPneu.Perfil.val() == 0 ? "" : _dimensaoPneu.Perfil.val();
    var radial = _dimensaoPneu.Radial.val() ? "R" : "";
    var aplicacao = largura + "/" + perfil + radial + aro;

    if (aplicacao === "/")
        aplicacao = "";

    _dimensaoPneu.Aplicacao.val(aplicacao);
}

function controlarBotoesHabilitados(isEdicao) {
    _CRUDDimensaoPneu.Atualizar.visible(isEdicao);
    _CRUDDimensaoPneu.Excluir.visible(isEdicao);
    _CRUDDimensaoPneu.Cancelar.visible(isEdicao);
    _CRUDDimensaoPneu.Adicionar.visible(!isEdicao);
}

function limparCamposDimensaoPneu() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_dimensaoPneu);
}

function recarregarGridDimensaoPneu() {
    _gridDimensaoPneu.CarregarGrid();
}