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
//// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="Veiculo.js" />
/// <reference path="Meta.js" />
/// <reference path="Vigencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoBonificacao;
var _grupoBonificacao;
var _pesquisaGrupoBonificacao;

var PesquisaGrupoBonificacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração: " });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoBonificacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var GrupoBonificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de integração:", required: true, maxlength: 50 });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 50 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });

    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Metas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Vigencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDGrupoBonificacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadGrupoBonificacao() {
    _grupoBonificacao = new GrupoBonificacao();
    KoBindings(_grupoBonificacao, "knockoutCadastroGrupoBonificacao");

    HeaderAuditoria("GrupoBonificacao", _grupoBonificacao);

    _crudGrupoBonificacao = new CRUDGrupoBonificacao();
    KoBindings(_crudGrupoBonificacao, "knockoutCRUDGrupoBonificacao");

    _pesquisaGrupoBonificacao = new PesquisaGrupoBonificacao();
    KoBindings(_pesquisaGrupoBonificacao, "knockoutPesquisaGrupoBonificacao", false, _pesquisaGrupoBonificacao.Pesquisar.id);

    buscarGrupoBonificacao();

    LoadVeiculo();
    LoadGrupoBonificacaoMeta();
    LoadGrupoBonificacaoVigencia();
}

function adicionarClick() {

    if (ValidarTodosCamposGrupoBonificacao()) {
        executarReST("GrupoBonificacao/Adicionar", ObterGrupoBonificacaoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridGrupoBonificacao.CarregarGrid();
                    limparCamposGrupoBonificacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        Global.ResetarAbas();
    }
}

function atualizarClick() {

    if (ValidarTodosCamposGrupoBonificacao()) {
        executarReST("GrupoBonificacao/Atualizar", ObterGrupoBonificacaoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridGrupoBonificacao.CarregarGrid();
                    limparCamposGrupoBonificacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        Global.ResetarAbas();
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Grupo de Bonificação " + _grupoBonificacao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_grupoBonificacao, "GrupoBonificacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridGrupoBonificacao.CarregarGrid();
                    limparCamposGrupoBonificacao();
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
    limparCamposGrupoBonificacao();
}

//*******MÉTODOS*******

function buscarGrupoBonificacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarGrupoBonificacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGrupoBonificacao = new GridView(_pesquisaGrupoBonificacao.Pesquisar.idGrid, "GrupoBonificacao/Pesquisa", _pesquisaGrupoBonificacao, menuOpcoes, null);
    _gridGrupoBonificacao.CarregarGrid();
}

function editarGrupoBonificacao(grupoBonificacaoGrid) {
    limparCamposGrupoBonificacao();
    _grupoBonificacao.Codigo.val(grupoBonificacaoGrid.Codigo);
    executarReST("GrupoBonificacao/BuscarPorCodigo", { Codigo: _grupoBonificacao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_grupoBonificacao, retorno);

                _pesquisaGrupoBonificacao.ExibirFiltros.visibleFade(false);
                _crudGrupoBonificacao.Atualizar.visible(true);
                _crudGrupoBonificacao.Cancelar.visible(true);
                _crudGrupoBonificacao.Excluir.visible(true);
                _crudGrupoBonificacao.Adicionar.visible(false);

                preencherGrupoBonificacaoMeta(retorno.Data.Metas);
                preencherGrupoBonificacaoVeiculo(retorno.Data.Veiculos);
                preencherGrupoBonificacaoVigencia(retorno.Data.Vigencia);

                recarregarGridVeiculo();
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function limparCamposGrupoBonificacao() {
    Global.ResetarAbas();
    _crudGrupoBonificacao.Atualizar.visible(false);
    _crudGrupoBonificacao.Cancelar.visible(false);
    _crudGrupoBonificacao.Excluir.visible(false);
    _crudGrupoBonificacao.Adicionar.visible(true);
    LimparCamposVeiculo()
    limparCamposGrupoBonificacaoMeta()
    limparCamposGrupoBonificacaoVigencia()
    LimparCampos(_grupoBonificacao);
}

function ObterGrupoBonificacaoSalvar() {
    _grupoBonificacao.Veiculos.val(obterVeiculos());

    let grupoBonificacao = RetornarObjetoPesquisa(_grupoBonificacao);

    preencherGrupoBonificacaoMetaSalvar(grupoBonificacao);
    preencherGrupoBonificacaoVigenciaSalvar(grupoBonificacao);

    return grupoBonificacao;
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function ValidarTodosCamposGrupoBonificacao() {

    if (!ValidarCamposObrigatorios(_grupoBonificacao))
        return false;
    return true
}