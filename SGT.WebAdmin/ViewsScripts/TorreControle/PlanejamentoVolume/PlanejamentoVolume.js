    // #region Objetos Globais do Arquivo
var _pesquisaPlanejamentoVolume;
var _gridPlanejamentoVolume;
var _planejamentoVolume;
var _crudPlanejamento;
// #endregion Objetos Globais do Arquivo

// #region Classes 
var PesquisaPlanejamentoVolume = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPlanejamentoVolume, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataProgramacaoCargaInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data de programação da carga inicial:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataProgramacaoCargaFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data de programação da carga final:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Remetentes = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetentes:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatarios = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatários:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var PlanejamentoVolume = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de carga:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de operação:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });
    this.DataProgramacaoCargaInicial = PropertyEntity({ getType: typesKnockout.date, text: "Data de programação da carga inicial:", val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(true) });
    this.DataProgramacaoCargaFinal = PropertyEntity({ getType: typesKnockout.date, text: "Data de programação da carga final:", val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.DisponibilidadePlacas = PropertyEntity({ getType: typesKnockout.int, text: "Disponibilidade de Placas", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ getType: typesKnockout.text, text: "Número do Contrato:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TotalToneladasMes = PropertyEntity({ getType: typesKnockout.decimal, text: "Total de tonelada (mês):", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.PesoBruto = PropertyEntity({ getType: typesKnockout.decimal, text: "Peso bruto:", val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.TotalTransferenciaEntrePlantas = PropertyEntity({ getType: typesKnockout.int, text: "Total de transferência entre plantas:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Remetentes = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.Destinatarios = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.Origens = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.Destinos = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });

}

var CRUDPlanejamentoVolume = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarPlanejamentoVolumeClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPlanejamentoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPlanejamentoVolumeClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "PlanejamentoVolume/Importar",
        UrlConfiguracao: "PlanejamentoVolume/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O077_PlanejamentoVolume,
        CallbackImportacao: function () {
            recarregarGridPlanejamentoVolume();
        },
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        }
    });
}

// #endregion Classes

// #region Funções de Inicialização
function loadPlanejamentoVolume() {
    _pesquisaPlanejamentoVolume = new PesquisaPlanejamentoVolume();
    KoBindings(_pesquisaPlanejamentoVolume, "knockoutPesquisaPlanejamentoVolume", false, _pesquisaPlanejamentoVolume.Pesquisar.id);

    _planejamentoVolume = new PlanejamentoVolume();
    KoBindings(_planejamentoVolume, "knockoutPlanejamentoVolume");

    _crudPlanejamento = new CRUDPlanejamentoVolume();
    KoBindings(_crudPlanejamento, "knockoutCRUDPlanejamento");

    BuscarTiposdeCarga(_pesquisaPlanejamentoVolume.TipoDeCarga);
    BuscarTiposOperacao(_pesquisaPlanejamentoVolume.TipoOperacao);
    BuscarClientes(_pesquisaPlanejamentoVolume.Remetentes);
    BuscarClientes(_pesquisaPlanejamentoVolume.Destinatarios);
    BuscarTransportadores(_pesquisaPlanejamentoVolume.Transportador);

    BuscarLocalidades(_planejamentoVolume.Destinos);
    BuscarLocalidades(_planejamentoVolume.Origens);
    BuscarTiposdeCarga(_planejamentoVolume.TipoDeCarga);
    BuscarTiposOperacao(_planejamentoVolume.TipoOperacao);
    BuscarTransportadores(_planejamentoVolume.Transportador);
    BuscarModelosVeicularesCarga(_planejamentoVolume.ModeloVeicular);

    loadGridPlanejamentoVolume();
    loadDestinos();
    loadOrigens();
    loadRemetentes();
    loadDestinatarios();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirPlanejamentoVolumeClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o registro " + registroSelecionado.Codigo + "?", function () {
        const dados = {
            Codigo: registroSelecionado.Codigo
        };

        executarReST("PlanejamentoVolume/ExcluirPlanejamento", dados, function (arg) {

            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    limparCamposPlanejamentoVolume();
                    recarregarGridPlanejamentoVolume();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}


function adicionarPlanejamentoVolumeClick(e, sender) {

    if (!ValidarCamposObrigatorios(_planejamentoVolume)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var planejamentoVolume = obterPlanejamentoVolume();
    executarReST("PlanejamentoVolume/Adicionar", planejamentoVolume, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                recarregarGridPlanejamentoVolume();
                limparCamposPlanejamentoVolume();
                recarregarGridRemetentes();

                _crudPlanejamento.Importar.visible(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPlanejamentoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_planejamentoVolume)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    preencherListas();
    Salvar(_planejamentoVolume, "PlanejamentoVolume/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposPlanejamentoVolume();
                recarregarGridPlanejamentoVolume();
                _crudPlanejamento.Adicionar.visible(true);
                _crudPlanejamento.Atualizar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function editarPlanejamentoClick(registroSelecionado, duplicar) {
    limparCamposPlanejamentoVolume();

    _crudPlanejamento.Atualizar.visible(true);
    _crudPlanejamento.Adicionar.visible(false);
    _crudPlanejamento.Cancelar.visible(true);
    _crudPlanejamento.Importar.visible(false);
    executarReST("PlanejamentoVolume/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo, Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPlanejamentoVolume.ExibirFiltros.visibleFade(false);
                PreencherObjetoKnout(_planejamentoVolume, retorno);
                recarregarGridRemetentes();
                recarregarGridDestinatarios();
                recarregarGridOrigens();
                recarregarGridDestinos();

                if (duplicar) {
                    _crudPlanejamento.Atualizar.visible(false);
                    _crudPlanejamento.Adicionar.visible(true);
                }

            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function cancelarPlanejamentoVolumeClick() {
    limparCamposPlanejamentoVolume();

    _crudPlanejamento.Cancelar.visible(false);
    _crudPlanejamento.Atualizar.visible(false);
    _crudPlanejamento.Adicionar.visible(true);
    _crudPlanejamento.Importar.visible(true);
}
// #endregion Funções Associadas a Eventos

// #region Funções Privadas
function loadGridPlanejamentoVolume() {

    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { excluirPlanejamentoVolumeClick(registroSelecionado); }, tamanho: "15", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { editarPlanejamentoClick(registroSelecionado, false); }, tamanho: "10", icone: "" };
    var duplicar = { descricao: "Duplicar", id: "clasEditar", evento: "onclick", metodo: function (registroSelecionado) { editarPlanejamentoClick(registroSelecionado, true); }, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [excluir, editar, duplicar] };

    var configuracaoExportacao = {
        url: "PlanejamentoVolume/ExportarPesquisa",
        titulo: "Planejamento Volume"
    };

    _gridPlanejamentoVolume = new GridView(_pesquisaPlanejamentoVolume.Grid.idGrid, "PlanejamentoVolume/Pesquisa", _pesquisaPlanejamentoVolume, menuOpcoes, null, 10, null, true, false, null, null, null, configuracaoExportacao);
    _gridPlanejamentoVolume.CarregarGrid();
}
function recarregarGridPlanejamentoVolume() {
    _gridPlanejamentoVolume.CarregarGrid();
}

function obterPlanejamentoVolume() {
    preencherListas();
    var planejamentoVolume = RetornarObjetoPesquisa(_planejamentoVolume);

    return planejamentoVolume;
}

function preencherListas() {
    _planejamentoVolume.Remetentes.val(JSON.stringify(_gridRemetentes.BuscarRegistros()));
    _planejamentoVolume.Destinatarios.val(JSON.stringify(_gridDestinatarios.BuscarRegistros()));
    _planejamentoVolume.Origens.val(JSON.stringify(_gridOrigens.BuscarRegistros()));
    _planejamentoVolume.Destinos.val(JSON.stringify(_gridDestinos.BuscarRegistros()));
}

function limparCamposPlanejamentoVolume() {

    LimparCampos(_planejamentoVolume);
    limparCamposDestino();
    limparCamposOrigem();
    limparCamposRemetente();
    limparCamposDestinatario();
}
// #endregion Funções Privadas