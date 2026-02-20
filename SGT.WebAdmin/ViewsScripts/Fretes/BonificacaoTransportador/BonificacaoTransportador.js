/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoAjusteValor.js" />
/// <reference path="Filial.js" />
/// <reference path="TipoDeCarga.js" />
/// <reference path="TipoOcorrencia.js" />
/// <reference path="FiliaisTransportador.js" />

// #region Objetos Globais do Arquivo

var _gridBonificacaoTransportador;
var _bonificacaoTransportador;
var _pesquisaBonificacaoTransportador;
var _crudBonificacaoTransportador;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaBonificacaoTransportador = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBonificacaoTransportador.CarregarGrid();
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

var BonificacaoTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Transportador:", issue: 63, idBtnSearch: guid() });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), visible: ko.observable(true), text: "*Componente de Frete:", issue: 85, idBtnSearch: guid() });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false), required: ko.observable(true), text: "*Tipo de Ocorrência Complementar Gerada:", idBtnSearch: guid() });
    this.IncluirBaseCalculoICMS = PropertyEntity({ text: "Incluir na base de cálculo do ICMS", getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.Percentual = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 5, text: "*Percentual:", required: true, visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoAjusteValor.Acrescimo), options: EnumTipoAjusteValor.obterOpcoes(), def: EnumTipoAjusteValor.Acrescimo, text: "*Tipo:" });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação:", issue: 557 });
    this.NaoIncluirComponentesFreteCalculoBonificacao = PropertyEntity({ text: "Não incluir componentes de frete no cálculo da bonificação", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.FiliaisInfo = PropertyEntity({ type: types.map, required: false, text: "Adicionar Filiais", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.TiposDeCargaInfo = PropertyEntity({ type: types.map, required: false, text: "Adicionar Tipos de Carga", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.TiposDeCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.TiposDeOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.FiliaisTransportador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
}

var CRUDBonificacaoTransportador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadBonificacaoTransportador() {
    _bonificacaoTransportador = new BonificacaoTransportador();
    KoBindings(_bonificacaoTransportador, "knockoutCadastroBonificacaoTransportador");

    _crudBonificacaoTransportador = new CRUDBonificacaoTransportador();
    KoBindings(_crudBonificacaoTransportador, "knockoutCRUDBonificacaoTransportador");

    _pesquisaBonificacaoTransportador = new PesquisaBonificacaoTransportador();
    KoBindings(_pesquisaBonificacaoTransportador, "knockoutPesquisaBonificacaoTransportador", false, _pesquisaBonificacaoTransportador.Pesquisar.id);

    HeaderAuditoria("BonificacaoTransportador", _bonificacaoTransportador);

    new BuscarTransportadores(_bonificacaoTransportador.Empresa);
    new BuscarComponentesDeFrete(_bonificacaoTransportador.ComponenteFrete);
    new BuscarTipoOcorrencia(_bonificacaoTransportador.TipoOcorrencia);

    new BuscarTransportadores(_pesquisaBonificacaoTransportador.Empresa);
    new BuscarTiposdeCarga(_pesquisaBonificacaoTransportador.TipoDeCarga);
    new BuscarFilial(_pesquisaBonificacaoTransportador.Filial);

    loadGridBonificacaoTransportador();

    loadFiliais()
    RecarregarFiliais();
    loadTipoOcorrencia();
    loadFilialTransportador();

    loadTiposDeCarga();
    RecarregarTiposDeCarga();

    utilizarBonificacaoParaTransportadoresViaOcorrencia();
}

function loadGridBonificacaoTransportador() {
    var configExportacao = {
        url: "BonificacaoTransportador/ExportarPesquisa",
        titulo: "Bonificações aos Transportadores"
    };

    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBonificacaoTransportador, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridBonificacaoTransportador = new GridViewExportacao(_pesquisaBonificacaoTransportador.Pesquisar.idGrid, "BonificacaoTransportador/Pesquisa", _pesquisaBonificacaoTransportador, menuOpcoes, configExportacao, null);
    _gridBonificacaoTransportador.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    preencherListaFilial();
    preencherListaTipoDeCarga();
    preencherListasTiposOcorrencia();
    preencherListasFilialTransportador();

    Salvar(_bonificacaoTransportador, "BonificacaoTransportador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridBonificacaoTransportador.CarregarGrid();
                limparCamposBonificacaoTransportador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListaFilial();
    preencherListaTipoDeCarga();
    preencherListasTiposOcorrencia();
    preencherListasFilialTransportador();

    Salvar(_bonificacaoTransportador, "BonificacaoTransportador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridBonificacaoTransportador.CarregarGrid();
                limparCamposBonificacaoTransportador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposBonificacaoTransportador();
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a bonificação da transportadora ?", function () {
        ExcluirPorCodigo(_bonificacaoTransportador, "BonificacaoTransportador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridBonificacaoTransportador.CarregarGrid();
                limparCamposBonificacaoTransportador();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function editarBonificacaoTransportador(bonificacaoTransportadorGrid) {
    limparCamposBonificacaoTransportador();
    _bonificacaoTransportador.Codigo.val(bonificacaoTransportadorGrid.Codigo);

    BuscarPorCodigo(_bonificacaoTransportador, "BonificacaoTransportador/BuscarPorCodigo", function (arg) {
        _pesquisaBonificacaoTransportador.ExibirFiltros.visibleFade(false);
        _crudBonificacaoTransportador.Atualizar.visible(true);
        _crudBonificacaoTransportador.Cancelar.visible(true);
        _crudBonificacaoTransportador.Excluir.visible(true);
        _crudBonificacaoTransportador.Adicionar.visible(false);

        RecarregarFiliais();
        RecarregarTiposDeCarga();
        recarregarGridTipoOcorrencia();
        recarregarGridFilialTransportador();
    }, null);
}

function limparCamposBonificacaoTransportador() {
    _crudBonificacaoTransportador.Atualizar.visible(false);
    _crudBonificacaoTransportador.Cancelar.visible(false);
    _crudBonificacaoTransportador.Excluir.visible(false);
    _crudBonificacaoTransportador.Adicionar.visible(true);

    LimparCampos(_bonificacaoTransportador);
    limparCamposTipoOcorrencia();
    limparCamposFilialTransportador();

    RecarregarFiliais();
    RecarregarTiposDeCarga();
    recarregarGridTipoOcorrencia();
    recarregarGridFilialTransportador();
}

function utilizarBonificacaoParaTransportadoresViaOcorrencia() {
    if (_CONFIGURACAO_TMS.UtilizarBonificacaoParaTransportadoresViaOcorrencia) {
        $("#liFiliaisTransportador").show();
        $("#liOcorrencias").show();

        _bonificacaoTransportador.ComponenteFrete.visible(false);
        _bonificacaoTransportador.ComponenteFrete.required(false);

        _bonificacaoTransportador.TipoOcorrencia.visible(true);
        _bonificacaoTransportador.TipoOcorrencia.required(true);
    } else {
        $("#liFiliaisTransportador").hide();
        $("#liOcorrencias").hide();
    }
}

function preencherListasTiposOcorrencia() {
    _bonificacaoTransportador.TiposDeOcorrencia.val(JSON.stringify(_gridTipoOcorrencia.BuscarRegistros()));
}

function preencherListasFilialTransportador() {
    _bonificacaoTransportador.FiliaisTransportador.val(JSON.stringify(_gridFilialTransportador.BuscarRegistros()));
}

// #endregion Funções Privadas
