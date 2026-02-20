/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _pesquisaGrupoTipoDeOcorrencia,
    _grupoTipoDeOcorrencia,
    _gridPesquisaGrupoTipoDeOcorrencia;

var PesquisaGrupoTipoOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 100 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", maxlength: 100 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarClick, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var GrupoTipoOcorrencia = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 100, required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", maxlength: 100, required: false });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: _status, def: 1 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

function loadGrupoTipoDeOcorrencia() {
    var idCadastro = "knockoutCadastroGrupoTipoDeOcorrencia";
    var idPesquisa = "knockoutPesquisaGrupoTipoDeOcorrencia";

    _pesquisaGrupoTipoDeOcorrencia = new PesquisaGrupoTipoOcorrencia();
    _grupoTipoDeOcorrencia = new GrupoTipoOcorrencia();

    KoBindings(_grupoTipoDeOcorrencia, idCadastro);
    KoBindings(_pesquisaGrupoTipoDeOcorrencia, idPesquisa);

    loadGridPesquisa();
}

function loadGridPesquisa() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPesquisaGrupoTipoDeOcorrencia = new GridView(_pesquisaGrupoTipoDeOcorrencia.Pesquisar.idGrid, "GrupoTipoDeOcorrenciaDeCTe/Pesquisa", _pesquisaGrupoTipoDeOcorrencia, menuOpcoes);
    recarregarGridPesquisa();
}

function pesquisarClick() {
    recarregarGridPesquisa();
}

function editarClick(registroSelecionado) {
    executarReST("GrupoTipoDeOcorrenciaDeCTe/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                limparCampos();
                controlarBotoes(true);
                PreencherObjetoKnout(_grupoTipoDeOcorrencia, r);
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function controlarBotoes(atualizar) {
    _grupoTipoDeOcorrencia.Atualizar.visible(atualizar);
    _grupoTipoDeOcorrencia.Adicionar.visible(!atualizar);
}

function adicionarClick() {
    if (!ValidarCamposObrigatorios(_grupoTipoDeOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("GrupoTipoDeOcorrenciaDeCTe/Adicionar", RetornarObjetoPesquisa(_grupoTipoDeOcorrencia), function (r) {
        if (r.Success) {
            if (r.Data) {
                limparCampos();
                exibirMensagem(tipoMensagem.ok, "Sucesso", r.Msg);
                recarregarGridPesquisa();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function atualizarClick() {
    if (!ValidarCamposObrigatorios(_grupoTipoDeOcorrencia)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("GrupoTipoDeOcorrenciaDeCTe/Atualizar", RetornarObjetoPesquisa(_grupoTipoDeOcorrencia), function (r) {
        if (r.Success) {
            if (r.Data) {
                limparCampos();
                exibirMensagem(tipoMensagem.ok, "Sucesso", r.Msg);
                recarregarGridPesquisa();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function limparClick() {
    limparCampos();
}

function limparCampos() {
    controlarBotoes(false);
    _pesquisaGrupoTipoDeOcorrencia.ExibirFiltros.visibleFade(false);
    LimparCampos(_grupoTipoDeOcorrencia);
}

function recarregarGridPesquisa() {
    _gridPesquisaGrupoTipoDeOcorrencia.CarregarGrid();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}