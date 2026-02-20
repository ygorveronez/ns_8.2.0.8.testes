/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoMontagemCargaPatio.js" />

// #region Objetos Globais do Arquivo

var _gridMontagemCargaPatio;
var _pesquisaMontagemCargaPatio;
var _montagemCargaPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaMontagemCargaPatio = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoMontagemCargaPatio.AguardandoMontagemCarga), options: EnumSituacaoMontagemCargaPatio.obterOpcoesPesquisa(), def: EnumSituacaoMontagemCargaPatio.AguardandoMontagemCarga });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridMontagemCargaPatio.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var MontagemCargaPatio = function () {
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

    this.QuantidadeCaixas = PropertyEntity({ text: "Quantidade de Caixas:", getType: typesKnockout.int, enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.QuantidadeItens = PropertyEntity({ text: "Quantidade de Itens:", getType: typesKnockout.int, enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.QuantidadePalletsFracionados = PropertyEntity({ text: "Quantidade de Pallets Fracionados:", getType: typesKnockout.int, enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.QuantidadePalletsInteiros = PropertyEntity({ text: "Quantidade de Pallets Inteiros:", getType: typesKnockout.int, enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaMontagemCargaPatioClick, type: types.event, text: "Confirmar", visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_montagemCargaPatio.ObservacaoFluxoPatio.val()); }, type: types.event, text: "Exibir Observação" });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridMontagemCargaPatio() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMontagemCargaPatioClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar], tamanho: "10" };

    var configExportacao = {
        url: "MontagemCargaPatio/ExportarPesquisa",
        titulo: "Montagem de Carga do Pátio"
    };

    _gridMontagemCargaPatio = new GridView(_pesquisaMontagemCargaPatio.Pesquisar.idGrid, "MontagemCargaPatio/Pesquisa", _pesquisaMontagemCargaPatio, menuOpcoes,
        null, null, null, null, null, null, null, null, configExportacao);
    _gridMontagemCargaPatio.CarregarGrid();
}

function loadMontagemCargaPatio() {
    _pesquisaMontagemCargaPatio = new PesquisaMontagemCargaPatio();
    KoBindings(_pesquisaMontagemCargaPatio, "knockoutPesquisaMontagemCargaPatio", false, _pesquisaMontagemCargaPatio.Pesquisar.id);

    _montagemCargaPatio = new MontagemCargaPatio();
    KoBindings(_montagemCargaPatio, "knockoutMontagemCargaPatio");

    loadGridMontagemCargaPatio();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaMontagemCargaPatioClick() {
    var dados = {
        Codigo: _montagemCargaPatio.Codigo.val(),
        QuantidadeCaixas: _montagemCargaPatio.QuantidadeCaixas.val(),
        QuantidadeItens: _montagemCargaPatio.QuantidadeItens.val(),
        QuantidadePalletsFracionados: _montagemCargaPatio.QuantidadePalletsFracionados.val(),
        QuantidadePalletsInteiros: _montagemCargaPatio.QuantidadePalletsInteiros.val()
    };

    executarReST("MontagemCargaPatio/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Montagem de carga do pátio finalizada com sucesso");
                editarMontagemCargaPatio(_montagemCargaPatio.Codigo.val());
                recarregarGridMontagemCargaPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function editarMontagemCargaPatioClick(registroSelecionado) {
    editarMontagemCargaPatio(registroSelecionado.Codigo);
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function editarMontagemCargaPatio(codigo) {
    LimparCampos(_montagemCargaPatio);

    executarReST("MontagemCargaPatio/BuscarPorCodigo", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_montagemCargaPatio, retorno);

                $("#container-montagem-carga-patio").show();

                _montagemCargaPatio.QuantidadeCaixas.visible(retorno.Data.PermiteInformarQuantidadeCaixas);
                _montagemCargaPatio.QuantidadeItens.visible(retorno.Data.PermiteInformarQuantidadeItens);
                _montagemCargaPatio.QuantidadePalletsFracionados.visible(retorno.Data.PermiteInformarQuantidadePallets);
                _montagemCargaPatio.QuantidadePalletsInteiros.visible(retorno.Data.PermiteInformarQuantidadePallets);
                _montagemCargaPatio.AvancarEtapa.visible(_montagemCargaPatio.Situacao.val() == EnumSituacaoMontagemCargaPatio.AguardandoMontagemCarga);

                _pesquisaMontagemCargaPatio.ExibirFiltros.visibleFade(false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function recarregarGridMontagemCargaPatio() {
    _gridMontagemCargaPatio.CarregarGrid();
}

// #endregion Funções Privadas
