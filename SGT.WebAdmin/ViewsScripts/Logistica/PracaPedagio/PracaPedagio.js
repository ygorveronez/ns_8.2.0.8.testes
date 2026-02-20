/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPracaPedagio;
var _pracaPedagio;
var _pesquisaPracaPedagio;
var _gridPracaPedagio;

/*
 * Declaração das Classes
 */

var CRUDPracaPedagio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var PracaPedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.CodigoDeIntegracao.getRequiredFieldDescription(), issue: 15, required: true, maxlength: 50 });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Descricao.getRequiredFieldDescription(), maxlength: 400, issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Concessionaria = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Concessionaria.getFieldDescription(), maxlength: 400, required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Rodovia = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Rodovia.getRequiredFieldDescription(), maxlength: 150, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.KM = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Km.getRequiredFieldDescription(), maxlength: 15, required: true, getType: typesKnockout.decimal, val: ko.observable("") });
    this.Latitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Observacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Situacao.getRequiredFieldDescription(), issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaPracaPedagio = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Descricao.getFieldDescription(), required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Concessionaria = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Concessionaria.getFieldDescription(), required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.CodigoDeIntegracao.getFieldDescription(), issue: 15, required: false, maxlength: 50 });
    this.Rodovia = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Rodovia.getRequiredFieldDescription(), maxlength: 150, required: false, getType: typesKnockout.string, val: ko.observable("") });

    this.Status = PropertyEntity({ text: Localization.Resources.Logistica.PracaPedagio.Situacao.getRequiredFieldDescription(), val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPracaPedagio, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPracaPedagio() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "PracaPedagio/ExportarPesquisa", titulo: Localization.Resources.Logistica.PracaPedagio.MotivoPunicaoVeiculo };

    _gridPracaPedagio = new GridViewExportacao(_pesquisaPracaPedagio.Pesquisar.idGrid, "PracaPedagio/Pesquisa", _pesquisaPracaPedagio, menuOpcoes, configuracoesExportacao);
    _gridPracaPedagio.CarregarGrid();

    loadGeolocalizacao();
}

function loadPracaPedagio() {
    _pracaPedagio = new PracaPedagio();
    KoBindings(_pracaPedagio, "knockoutPracaPedagio");

    HeaderAuditoria("PracaPedagio", _pracaPedagio);

    _CRUDPracaPedagio = new CRUDPracaPedagio();
    KoBindings(_CRUDPracaPedagio, "knockoutCRUDPracaPedagio");

    _pesquisaPracaPedagio = new PesquisaPracaPedagio();
    KoBindings(_pesquisaPracaPedagio, "knockoutPesquisaPracaPedagio", false, _pesquisaPracaPedagio.Pesquisar.id);

    loadGridPracaPedagio();
    loadPracaPedagioTarifa();
    loadPedagioTarifaIntegracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    if (ValidarCamposObrigatorios(_pracaPedagio)) {
        executarReST("PracaPedagio/Adicionar", obterPracaPedagioSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    recarregarGridPracaPedagio();
                    limparCamposPracaPedagio();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    } else {
        exibirMensagemCamposObrigatorio();
    }
}

function atualizarClick(e, sender) {
    if (ValidarCamposObrigatorios(_pracaPedagio)) {
        executarReST("PracaPedagio/Atualizar", obterPracaPedagioSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                    recarregarGridPracaPedagio();
                    limparCamposPracaPedagio();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    } else {
        exibirMensagemCamposObrigatorio();
    }
}

function cancelarClick() {
    limparCamposPracaPedagio();
}

function editarClick(registroSelecionado) {
    limparCamposPracaPedagio();

    _pracaPedagio.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pracaPedagio, "PracaPedagio/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPracaPedagio.ExibirFiltros.visibleFade(false);
                var isEdicao = true;
                controlarBotoesHabilitados(isEdicao);
                setarCoordenadas();
                preencherPracaPedagioTarifa(retorno.Data.PracaPedagioTarifa);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.PracaPedagio.RealmenteDesejaExcluirEsseCadastro, function () {
        ExcluirPorCodigo(_pracaPedagio, "PracaPedagio/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);

                    recarregarGridPracaPedagio();
                    limparCamposPracaPedagio();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDPracaPedagio.Atualizar.visible(isEdicao);
    _CRUDPracaPedagio.Excluir.visible(isEdicao);
    _CRUDPracaPedagio.Cancelar.visible(isEdicao);
    _CRUDPracaPedagio.Adicionar.visible(!isEdicao);
}

function limparCamposPracaPedagio() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_pracaPedagio);
    limparCamposMapa();
    limparCamposPracaPedagioTarifa();
}

function recarregarGridPracaPedagio() {
    _gridPracaPedagio.CarregarGrid();
}

function obterPracaPedagioSalvar() {
    var pracaPedagio = RetornarObjetoPesquisa(_pracaPedagio);
    pracaPedagio["PracaPedagioTarifa"] = obterPracaPedagioTarifaSalvar();
    return pracaPedagio;
}