/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumMes.js" />

// #region Objetos Globais do Arquivo

var _gridGeracaoFrotaAutomatizada;
var _geracaoFrotaAutomatizada;
var _geracaoSugestaoMensal;
var _pesquisaGeracaoFrotaAutomatizada;
var _gridFiliais;
var _gridTipoOperacao;
var _gridModeloVeicular;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaConfiguracoesGeracaoAutomatizada = function () {
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), required: false });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), required: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), required: false });
    this.Descricao = PropertyEntity({ text: "Descrição", val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGeracaoFrotaAutomatizada.CarregarGrid();
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

var GeracaoFrotaAutomatizada = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição", val: ko.observable("") });
    this.Filiais = PropertyEntity({ type: types.event, text: "Adicionar Filial", idBtnSearch: guid(), idGrid: guid() });
    this.TipoOperacoes = PropertyEntity({ type: types.event, text: "Adicionar Tipo Operação", idBtnSearch: guid(), idGrid: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular", idBtnSearch: guid(), idGrid: guid() });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarGeracaoFrotaAutomatizadaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarGeracaoFrotaAutomatizadaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirGeracaoFrotaAutomatizadaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarGeracaoFrotaAutomatizadaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var GeracaoSugestaoMensal = function () {
    this.CodigoGeracaoFrotaAutomatizada = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ano = PropertyEntity({ text: "*Ano:", val: ko.observable(obterAnoAtual()), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 4, required: true });
    this.Mes = PropertyEntity({ text: "*Mes:", val: ko.observable(EnumMes.obterMesAtual()), options: EnumMes.obterOpcoes(), required: true });

    this.Gerar = PropertyEntity({ eventClick: gerarSugestaoMensalClick, type: types.event, text: "Gerar", idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGeracaoFrotaAutomatizada() {
    _geracaoFrotaAutomatizada = new GeracaoFrotaAutomatizada();
    KoBindings(_geracaoFrotaAutomatizada, "knockoutConfiguracoesGeracaoAutomatizada");

    _pesquisaGeracaoFrotaAutomatizada = new PesquisaConfiguracoesGeracaoAutomatizada();
    KoBindings(_pesquisaGeracaoFrotaAutomatizada, "knockoutPesquisaConfiguracoesGeracaoAutomatizada", false);

    _geracaoSugestaoMensal = new GeracaoSugestaoMensal();
    KoBindings(_geracaoSugestaoMensal, "knockoutGeracaoSugestaoMensal");

    HeaderAuditoria("GeracaoFrotaAutomatizada", _geracaoFrotaAutomatizada);

    new BuscarFilial(_pesquisaGeracaoFrotaAutomatizada.Filial);
    new BuscarTiposOperacao(_pesquisaGeracaoFrotaAutomatizada.TipoOperacao);
    new BuscarModelosVeiculo(_pesquisaGeracaoFrotaAutomatizada.ModeloVeicular);

    loadGridFilial();
    loadGridTipoOperacao();
    loadGridModeloVeicular();
    loadGridGeracaoFrotaAutomatizada();
}

function loadGridFilial() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: (registro) => excluirItemGrid(registro, _gridFiliais, "Filiais") };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Filial", width: "80%" }
    ];

    _gridFiliais = new BasicDataTable(_geracaoFrotaAutomatizada.Filiais.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFilial(_geracaoFrotaAutomatizada.Filiais, null, _gridFiliais);

    _geracaoFrotaAutomatizada.Filiais.basicTable = _gridFiliais;
    _geracaoFrotaAutomatizada.Filiais.basicTable.CarregarGrid([]);
}

function loadGridGeracaoFrotaAutomatizada() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarGeracaoFrotaAutomatizadaClick };
    var opcaoGerarListaMensal = { descricao: "Gerar Lista Mensal", id: guid(), metodo: gerarSugestaoMensalModalClick };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoEditar, opcaoGerarListaMensal], tamanho: 10 };

    _gridGeracaoFrotaAutomatizada = new GridView(_pesquisaGeracaoFrotaAutomatizada.Pesquisar.idGrid, "GeracaoFrotaAutomatizada/Pesquisa", _pesquisaGeracaoFrotaAutomatizada, menuOpcoes);
    _gridGeracaoFrotaAutomatizada.CarregarGrid();
}

function loadGridModeloVeicular() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: (registro) => excluirItemGrid(registro, _gridModeloVeicular, "ModeloVeicular") };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Modelo Veicular", width: "80%" }
    ];

    _gridModeloVeicular = new BasicDataTable(_geracaoFrotaAutomatizada.ModeloVeicular.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_geracaoFrotaAutomatizada.ModeloVeicular, null, null, null, null, null, _gridModeloVeicular);

    _geracaoFrotaAutomatizada.ModeloVeicular.basicTable = _gridModeloVeicular;
    _geracaoFrotaAutomatizada.ModeloVeicular.basicTable.CarregarGrid([]);
}

function loadGridTipoOperacao() {
    const opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: (registro) => excluirItemGrid(registro, _gridTipoOperacao, "TipoOperacoes") };
    const menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Tipo Operação", width: "80%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_geracaoFrotaAutomatizada.TipoOperacoes.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_geracaoFrotaAutomatizada.TipoOperacoes, null, null, null, _gridTipoOperacao);

    _geracaoFrotaAutomatizada.TipoOperacoes.basicTable = _gridTipoOperacao;
    _geracaoFrotaAutomatizada.TipoOperacoes.basicTable.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarGeracaoFrotaAutomatizadaClick() {
    let data = {};

    data.Codigo = _geracaoFrotaAutomatizada.Codigo.val();
    data.Filiais = obterItemsGrid(_gridFiliais);
    data.TipoOperacoes = obterItemsGrid(_gridTipoOperacao);
    data.ModelosVeicular = obterItemsGrid(_gridModeloVeicular);
    data.Descricao = _geracaoFrotaAutomatizada.Descricao.val();

    executarReST("GeracaoFrotaAutomatizada/Adicionar", data, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Configuração Adicionada Com Sucesso");
        limparCamposGeracaoFrotaAutomatizada();
        _gridGeracaoFrotaAutomatizada.CarregarGrid();
    })
}

function atualizarGeracaoFrotaAutomatizadaClick() {
    let data = {};

    data.Codigo = _geracaoFrotaAutomatizada.Codigo.val();
    data.Filiais = obterItemsGrid(_gridFiliais);
    data.TipoOperacoes = obterItemsGrid(_gridTipoOperacao);
    data.ModelosVeicular = obterItemsGrid(_gridModeloVeicular);
    data.Descricao = _geracaoFrotaAutomatizada.Descricao.val();

    executarReST("GeracaoFrotaAutomatizada/Atualizar", data, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Configuração Atualizada Com Sucesso");
        limparCamposGeracaoFrotaAutomatizada();
        _gridGeracaoFrotaAutomatizada.CarregarGrid();
    })
}

function cancelarGeracaoFrotaAutomatizadaClick() {
    limparCamposGeracaoFrotaAutomatizada();
}

function editarGeracaoFrotaAutomatizadaClick(editado) {
    limparCamposGeracaoFrotaAutomatizada();

    executarReST("GeracaoFrotaAutomatizada/BuscarPorCodigo", { Codigo: editado.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        if (arg.Data) {
            _pesquisaGeracaoFrotaAutomatizada.ExibirFiltros.visibleFade(false);
            controlarBotoesHabilitadosGeracao(true);

            _geracaoFrotaAutomatizada.Codigo.val(arg.Data.Codigo);
            _geracaoFrotaAutomatizada.Descricao.val(arg.Data.Descricao);
            recarregarGrid(arg.Data.Filiais, _gridFiliais);
            recarregarGrid(arg.Data.ModeloVeicular, _gridModeloVeicular);
            recarregarGrid(arg.Data.TipoOperacoes, _gridTipoOperacao);
        }
    });
}

function excluirGeracaoFrotaAutomatizadaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        executarReST("GeracaoFrotaAutomatizada/ExcluirPorCodigo", { Codigo: _geracaoFrotaAutomatizada.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                limparCamposGeracaoFrotaAutomatizada();

                return exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function gerarSugestaoMensalClick() {
    if (!ValidarCamposObrigatorios(_geracaoSugestaoMensal)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja gerar a lista mensal?", function () {
        executarReST("GeracaoFrotaAutomatizada/GerarListaMensal", RetornarObjetoPesquisa(_geracaoSugestaoMensal), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Lista mensal gerada com sucesso");
                    Global.fecharModal('divModalGeracaoSugestaoMensal');
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function gerarSugestaoMensalModalClick(registroSelecionado) {
    _geracaoSugestaoMensal.CodigoGeracaoFrotaAutomatizada.val(registroSelecionado.Codigo);
    _geracaoSugestaoMensal.Ano.val(obterAnoAtual());
    _geracaoSugestaoMensal.Mes.val(EnumMes.obterMesAtual());

    Global.abrirModal('divModalGeracaoSugestaoMensal');
    $("#divModalGeracaoSugestaoMensal").one('hidden.bs.modal', function () {
        LimparCampos(_geracaoSugestaoMensal);
    });
}

// #endregion Funções Associadas a Eventos

// #region Métodos Privados

function controlarBotoesHabilitadosGeracao(isEdicao) {
    _geracaoFrotaAutomatizada.Atualizar.visible(isEdicao);
    _geracaoFrotaAutomatizada.Excluir.visible(isEdicao);
    _geracaoFrotaAutomatizada.Cancelar.visible(isEdicao);
    _geracaoFrotaAutomatizada.Adicionar.visible(!isEdicao);
}

function excluirItemGrid(registroSelecionado, grid, tipo) {
    var listaItems = obterListaItemsGrid(tipo);

    for (var i = 0; i < listaItems.length; i++) {
        if (registroSelecionado.Codigo == listaItems[i].Codigo) {
            listaItems.splice(i, 1);
            break;
        }
    }

    grid.CarregarGrid(listaItems);
}

function limparCamposGeracaoFrotaAutomatizada() {
    LimparCampos(_geracaoFrotaAutomatizada);
    controlarBotoesHabilitadosGeracao(false);
    recarregarGrid([], _gridFiliais);
    recarregarGrid([], _gridModeloVeicular);
    recarregarGrid([], _gridTipoOperacao);

    $(".nav-tabs a[href='#tabConfiguracao']").tab('show');
}

function obterAnoAtual() {
    return new Date().getFullYear();
}

function obterItemsGrid(grid) {
    return JSON.stringify(grid.BuscarRegistros());
}

function obterListaItemsGrid(tipo) {
    if (tipo == "Filiais")
        return _geracaoFrotaAutomatizada.Filiais.basicTable.BuscarRegistros();

    if (tipo == "TipoOperacoes")
        return _geracaoFrotaAutomatizada.TipoOperacoes.basicTable.BuscarRegistros();

    if (tipo == "ModeloVeicular")
        return _geracaoFrotaAutomatizada.ModeloVeicular.basicTable.BuscarRegistros();
}

function recarregarGrid(items, grid) {
    grid.CarregarGrid(items);
}

// #endregion Métodos Privados
