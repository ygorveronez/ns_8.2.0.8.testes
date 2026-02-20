/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="Destino.js" />
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="TipoCarga.js" />
/// <reference path="TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _configuracaoProgramacaoCarga;
var _crudConfiguracaoProgramacaoCarga;
var _gridConfiguracaoProgramacaoCarga;
var _pesquisaConfiguracaoProgramacaoCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaConfiguracaoProgramacaoCarga = function () {
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _statusPesquisa, def: true });
    this.Descricao = PropertyEntity({ text: "Descrição: ", maxlength: 200 });
    this.Filial = PropertyEntity({ text: "Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoProgramacaoCarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ConfiguracaoProgramacaoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ativo = PropertyEntity({ text: "*Situação:", val: ko.observable(true), options: _status, def: true });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 200, required: true });
    this.Filial = PropertyEntity({ text: "*Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
}

var CrudConfiguracaoProgramacaoCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: true });
}

// #endregion Classes

// #region Funções de Inicialização

function loadConfiguracaoProgramacaoCarga() {
    _configuracaoProgramacaoCarga = new ConfiguracaoProgramacaoCarga();
    KoBindings(_configuracaoProgramacaoCarga, "knockoutCadastroConfiguracaoProgramacaoCarga");

    _crudConfiguracaoProgramacaoCarga = new CrudConfiguracaoProgramacaoCarga();
    KoBindings(_crudConfiguracaoProgramacaoCarga, "knockoutCrudConfiguracaoProgramacaoCarga");

    _pesquisaConfiguracaoProgramacaoCarga = new PesquisaConfiguracaoProgramacaoCarga();
    KoBindings(_pesquisaConfiguracaoProgramacaoCarga, "knockoutPesquisaConfiguracaoProgramacaoCarga", false, _pesquisaConfiguracaoProgramacaoCarga.Pesquisar.id);

    HeaderAuditoria("ConfiguracaoProgramacaoCarga", _configuracaoProgramacaoCarga);

    new BuscarFilial(_configuracaoProgramacaoCarga.Filial);
    new BuscarFilial(_pesquisaConfiguracaoProgramacaoCarga.Filial);
    new BuscarModelosVeicularesCarga(_pesquisaConfiguracaoProgramacaoCarga.ModeloVeicularCarga);
    new BuscarTiposdeCarga(_pesquisaConfiguracaoProgramacaoCarga.TipoCarga);
    new BuscarTiposOperacao(_pesquisaConfiguracaoProgramacaoCarga.TipoOperacao);

    loadDestino();
    loadTipoCarga();
    loadTipoOperacao();
    loadModeloVeicularCarga();
    loadGridConfiguracaoProgramacaoCarga();
}

function loadGridConfiguracaoProgramacaoCarga() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridConfiguracaoProgramacaoCarga = new GridView(_pesquisaConfiguracaoProgramacaoCarga.Pesquisar.idGrid, "ConfiguracaoProgramacaoCarga/Pesquisa", _pesquisaConfiguracaoProgramacaoCarga, menuOpcoes, null);
    _gridConfiguracaoProgramacaoCarga.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick() {
    var configuracaoProgramacaoCarga = obterConfiguracaoProgramacaoCarga();

    if (!configuracaoProgramacaoCarga)
        return;

    executarReST("ConfiguracaoProgramacaoCarga/Adicionar", configuracaoProgramacaoCarga, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso!");
                _gridConfiguracaoProgramacaoCarga.CarregarGrid();
                limparCamposConfiguracaoProgramacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    var configuracaoProgramacaoCarga = obterConfiguracaoProgramacaoCarga();

    if (!configuracaoProgramacaoCarga)
        return;

    executarReST("ConfiguracaoProgramacaoCarga/Atualizar", configuracaoProgramacaoCarga, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso!");
                _gridConfiguracaoProgramacaoCarga.CarregarGrid();
                limparCamposConfiguracaoProgramacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposConfiguracaoProgramacaoCarga();
}

function editarClick(registroSelecionado) {
    limparCamposConfiguracaoProgramacaoCarga();

    executarReST("ConfiguracaoProgramacaoCarga/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            _pesquisaConfiguracaoProgramacaoCarga.ExibirFiltros.visibleFade(false);
            PreencherObjetoKnout(_configuracaoProgramacaoCarga, retorno);
            preencherDestino(retorno.Data);
            preencherTipoCarga(retorno.Data);
            preencherTipoOperacao(retorno.Data);
            preencherModeloVeicularCarga(retorno.Data);
            controlarBotoesHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração de programação de carga?", function () {
        executarReST("ConfiguracaoProgramacaoCarga/ExcluirPorCodigo", { Codigo: _configuracaoProgramacaoCarga.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");
                _gridConfiguracaoProgramacaoCarga.CarregarGrid();
                limparCamposConfiguracaoProgramacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function limparCamposConfiguracaoProgramacaoCarga() {
    LimparCampos(_configuracaoProgramacaoCarga);
    controlarBotoesHabilitados();
    limparCamposDestino();
    limparCamposTipoCarga();
    limparCamposTipoOperacao();
    limparCamposModeloVeicularCarga();

    $(".nav-tabs a[href='#knockoutCadastroConfiguracaoProgramacaoCarga']").tab('show');
}

function controlarBotoesHabilitados() {
    var isEdicao = _configuracaoProgramacaoCarga.Codigo.val() > 0;

    _crudConfiguracaoProgramacaoCarga.Adicionar.visible(!isEdicao);
    _crudConfiguracaoProgramacaoCarga.Atualizar.visible(isEdicao);
    _crudConfiguracaoProgramacaoCarga.Excluir.visible(isEdicao);
}

function obterConfiguracaoProgramacaoCarga() {
    if (!ValidarCamposObrigatorios(_configuracaoProgramacaoCarga)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
        $(".nav-tabs a[href='#knockoutCadastroConfiguracaoProgramacaoCarga']").tab('show');
        return undefined;
    }

    if (!possuiDestinoInformado()) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os destinos");
        $(".nav-tabs a[href='#knockoutCadastroConfiguracaoProgramacaoCargaDestino']").tab('show');
        return undefined;
    }

    var configuracaoProgramacaoCarga = RetornarObjetoPesquisa(_configuracaoProgramacaoCarga);

    preencherDestinoSalvar(configuracaoProgramacaoCarga);
    preencherTipoCargaSalvar(configuracaoProgramacaoCarga);
    preencherTipoOperacaoSalvar(configuracaoProgramacaoCarga);
    preencherModeloVeicularCargaSalvar(configuracaoProgramacaoCarga);

    return configuracaoProgramacaoCarga
}

// #endregion Funções Privadas
