/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSeparacaoMercadoria.js" />
/// <reference path="../../Consultas/Usuario.js" />

// #region Objetos Globais do Arquivo

var _gridSeparacaoMercadoria,
    _gridSeparadores;

var _pesquisaSeparacaoMercadoria;
var _separacaoMercadoria;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaSeparacaoMercadoria = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria), options: EnumSituacaoSeparacaoMercadoria.obterOpcoesPesquisa(), def: EnumSituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridSeparacaoMercadoria.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var SeparacaoMercadoria = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ObservacaoFluxoPatio = PropertyEntity({ visible: false });

    this.NumeroCarga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: "Pré Carga:", val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: "Data:", val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: "Hora:", val: ko.observable(""), def: "" });

    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: "Fornecedor:", val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação:", val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: "Transportador:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo:", val: ko.observable(""), def: "" });

    this.NumeroCarregadores = PropertyEntity({ text: "Número de Carregadores:", getType: typesKnockout.int, enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.ResponsavelCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsavel pelo Carregamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.AdicionarResponsavelSeparacao = PropertyEntity({ type: types.event, idBtnSearch: guid(), text: "Adicionar Responsável Separação", eventClick: abrirModalResponsavelSeparacao, visible: ko.observable(true) });
    this.Separadores = PropertyEntity({ type: types.local, idGrid: guid(), val: ko.observableArray([]) });

    this.Separadores.val.subscribe(function () {
        recarregarGridSeparadores();
    });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaSeparacaoMercadoriaClick, type: types.event, text: "Confirmar", visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_separacaoMercadoria.ObservacaoFluxoPatio.val()); }, type: types.event, text: "Exibir Observação" });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridSeparacaoMercadoria() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSeparacaoMercadoriaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar], tamanho: "10" };

    var configExportacao = {
        url: "SeparacaoMercadoria/ExportarPesquisa",
        titulo: "Separações de Mercadorias"
    };

    _gridSeparacaoMercadoria = new GridView(_pesquisaSeparacaoMercadoria.Pesquisar.idGrid, "SeparacaoMercadoria/Pesquisa", _pesquisaSeparacaoMercadoria, menuOpcoes,
        null, null, null, null, null, null, null, null, configExportacao);
    _gridSeparacaoMercadoria.CarregarGrid();
}

function loadGridSeparacaoMercadoriaSeparadores() {
    var opcaoRemover = { descricao: "Remover", evento: "onclick", metodo: removerSeparadorClick, tamanho: "10", icone: "", visibilidade: visibilidadeOpcaoRemover };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoRemover], tamanho: "10" };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoResponsavel", visible: false },
        { data: "Descricao", title: "Descrição", width: "60%", className: "text-align-left" },
        { data: "CapacidadeSeparacao", title: "Capacidade de Separação", width: "20%", className: "text-align-right" }
    ];

    _gridSeparadores = new BasicDataTable(_separacaoMercadoria.Separadores.idGrid, header, menuOpcoes, null, null, 5);
    _gridSeparadores.CarregarGrid([]);
}

function loadSeparacaoMercadoria() {
    _pesquisaSeparacaoMercadoria = new PesquisaSeparacaoMercadoria();
    KoBindings(_pesquisaSeparacaoMercadoria, "knockoutPesquisaSeparacaoMercadoria", false, _pesquisaSeparacaoMercadoria.Pesquisar.id);

    _separacaoMercadoria = new SeparacaoMercadoria();
    KoBindings(_separacaoMercadoria, "knockoutSeparacaoMercadoria");

    new BuscarFuncionario(_separacaoMercadoria.ResponsavelCarregamento);

    loadGridSeparacaoMercadoria();
    loadGridSeparacaoMercadoriaSeparadores();
    loadResponsavelSeparacao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaSeparacaoMercadoriaClick() {
    var dados = {
        Codigo: _separacaoMercadoria.Codigo.val(),
        NumeroCarregadores: _separacaoMercadoria.NumeroCarregadores.val(),
        ResponsavelCarregamento: _separacaoMercadoria.ResponsavelCarregamento.codEntity(),
        Separadores: JSON.stringify(_separacaoMercadoria.Separadores.val())
    };
    
    executarReST("SeparacaoMercadoria/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Separação de mercadoria finalizada com sucesso");
                editarSeparacaoMercadoria(_separacaoMercadoria.Codigo.val());
                recarregarGridSeparacaoMercadoria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function editarSeparacaoMercadoriaClick(registroSelecionado) {
    editarSeparacaoMercadoria(registroSelecionado.Codigo);
}

function removerSeparadorClick(registroSelecionado) {
    var registros = _gridSeparadores.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _gridSeparadores.CarregarGrid(registros);
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function editarSeparacaoMercadoria(codigo) {
    LimparCampos(_separacaoMercadoria);
    _separacaoMercadoria.Separadores.val.removeAll();
    
    executarReST("SeparacaoMercadoria/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_separacaoMercadoria, retorno);
                
                _separacaoMercadoria.Separadores.val(retorno.Data.ResponsaveisSeparacao);

                $("#container-separacao-mercadoria").show();

                _separacaoMercadoria.NumeroCarregadores.visible(retorno.Data.PermiteInformarDadosCarregadores);
                _separacaoMercadoria.ResponsavelCarregamento.visible(retorno.Data.PermiteInformarDadosCarregadores);
                _separacaoMercadoria.AvancarEtapa.visible(_separacaoMercadoria.Situacao.val() == EnumSituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria);
                _separacaoMercadoria.AdicionarResponsavelSeparacao.visible(_separacaoMercadoria.Situacao.val() == EnumSituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria);

                _pesquisaSeparacaoMercadoria.ExibirFiltros.visibleFade(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function recarregarGridSeparacaoMercadoria() {
    _gridSeparacaoMercadoria.CarregarGrid();
}

function visibilidadeOpcaoRemover() {
    return _separacaoMercadoria.Situacao.val() != EnumSituacaoSeparacaoMercadoria.SeparacaoMercadoriaFinalizada;
}

// #endregion Funções Privadas
