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
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="GrupoMotivoChamadoIntegracoes.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _grupoMotivoChamado;
var _crudGrupoMotivoChamado;
var _pesquisaGrupoMotivoChamado;
var _gridOGrupoMotivoChamado;

var GrupoMotivoChamado = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable() });
    this.CodigoIntegracao = PropertyEntity({ text: 'Código Intregação', getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: 'Situação', val: ko.observable(true), required: true, options: _status, def: true });

    this.GeraOcorrenciaNormal = PropertyEntity({ text: 'Gera ocorrência normal', val: ko.observable(false), getType: typesKnockout.bool });
    this.GeraCargaAvulsa = PropertyEntity({ text: 'Gera Carga avulsa', val: ko.observable(false), getType: typesKnockout.bool });
    this.TipoOperacao = PropertyEntity({ text: '*Tipo Operação', val: ko.observable(), codEntity: ko.observable(0), type: types.entity, visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });
    this.NecessarioAprovacaoCriacaoCargaAvulsa = PropertyEntity({ text: 'Necessário aprovação criação Carga avulsa', val: ko.observable(false), getType: typesKnockout.bool });
    this.GeraCargaReversa = PropertyEntity({ text: 'Gera Carga reversa', val: ko.observable(false), getType: typesKnockout.bool });
    this.NaoPermiteLancamentoManual = PropertyEntity({ text: 'Não permite lançamento manual', val: ko.observable(false), getType: typesKnockout.bool });
    this.Sinistro = PropertyEntity({ text: 'Sinistro', val: ko.observable(false), getType: typesKnockout.bool });
    this.RecebeOcorrenciaERP = PropertyEntity({ text: 'Recebe Ocorrência do ERP', val: ko.observable(false), getType: typesKnockout.bool });
    this.GeraCargaAvulsa.val.subscribe(function (novoValor) {
        _grupoMotivoChamado.TipoOperacao.required(novoValor);
        _grupoMotivoChamado.TipoOperacao.visible(novoValor);
    });
};

var CRUDGrupoMotivoChamado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

var PesquisaGrupoMotivoChamado = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: 'Código Intregação', getType: typesKnockout.string, val: ko.observable() });
    this.Situacao = PropertyEntity({ text: 'Situação', val: ko.observable(true), options: _status, def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoMotivoChamado.CarregarGrid(); s
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadGrupoMotivoChamado() {

    _pesquisaGrupoMotivoChamado = new PesquisaGrupoMotivoChamado();
    KoBindings(_pesquisaGrupoMotivoChamado, "knockoutPesquisaGrupoMotivoChamado", false, _pesquisaGrupoMotivoChamado.Pesquisar.id);

    _grupoMotivoChamado = new GrupoMotivoChamado();
    KoBindings(_grupoMotivoChamado, "knockoutGrupoMotivoChamado");

    _crudGrupoMotivoChamado = new CRUDGrupoMotivoChamado();
    KoBindings(_crudGrupoMotivoChamado, "knockoutCRUDGrupoMotivoChamado");

    new BuscarTiposOperacao(_grupoMotivoChamado.TipoOperacao);

    HeaderAuditoria("GrupoMotivoChamado", _grupoMotivoChamado);

    loadGridGrupoMotivoChamado();
    loadGrupoMotivoChamadoIntegracoes();
    loadGrupoMotivoChamadoTiposOperacoes();
}

function adicionarClick() {
    executarReST("GrupoMotivoChamado/Adicionar", PreencherObjetoBack(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, 'Adicionado com sucesso!');
                _gridGrupoMotivoChamado.CarregarGrid();
                limparTudoGrupoMotivoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function atualizarClick() {
    executarReST("GrupoMotivoChamado/Atualizar", PreencherObjetoBack(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, 'Atualizado com sucesso!');
                _gridGrupoMotivoChamado.CarregarGrid();
                limparTudoGrupoMotivoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_grupoMotivoChamado, "GrupoMotivoChamado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, 'Excluído com sucesso!');
                    _gridGrupoMotivoChamado.CarregarGrid();
                    limparTudoGrupoMotivoChamado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparTudoGrupoMotivoChamado();
}

function editarGrupoMotivoChamadoClick(itemGrid) {
    _grupoMotivoChamado.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_grupoMotivoChamado, "GrupoMotivoChamado/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                _pesquisaGrupoMotivoChamado.ExibirFiltros.visibleFade(false);

                _crudGrupoMotivoChamado.Atualizar.visible(true);
                _crudGrupoMotivoChamado.Excluir.visible(true);
                _crudGrupoMotivoChamado.Cancelar.visible(true);
                _crudGrupoMotivoChamado.Adicionar.visible(false);

                _gridGrupoMotivoChamadoIntegracoes.CarregarGrid(arg.Data.TiposIntegracoes);
                _gridGrupoMotivoChamadoTiposOperacoes.CarregarGrid(arg.Data.TiposOperacao);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function loadGridGrupoMotivoChamado() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarGrupoMotivoChamadoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridGrupoMotivoChamado = new GridView(_pesquisaGrupoMotivoChamado.Pesquisar.idGrid, "GrupoMotivoChamado/Pesquisa", _pesquisaGrupoMotivoChamado, menuOpcoes);
    _gridGrupoMotivoChamado.CarregarGrid();
}

function limparTudoGrupoMotivoChamado() {
    _crudGrupoMotivoChamado.Atualizar.visible(false);
    _crudGrupoMotivoChamado.Cancelar.visible(false);
    _crudGrupoMotivoChamado.Excluir.visible(false);
    _crudGrupoMotivoChamado.Adicionar.visible(true);
    LimparCampos(_grupoMotivoChamado);
    limparCamposGrupoMotivoChamadoIntegracoes();
    limparCamposGrupoMotivoChamadoTiposOperacoes();

    Global.ResetarAbas();
}

function PreencherObjetoBack() {
    let tiposIntegracao = JSON.stringify(_gridGrupoMotivoChamadoIntegracoes.BuscarRegistros().map(x => { return x.CodigoSistemaIntegracao }));
    let tiposOperacao = JSON.stringify(_gridGrupoMotivoChamadoTiposOperacoes.BuscarRegistros().map(x => { return x.Codigo }));

    return { ...RetornarObjetoPesquisa(_grupoMotivoChamado), TiposIntegracao: tiposIntegracao, TiposOperacao: tiposOperacao };
}