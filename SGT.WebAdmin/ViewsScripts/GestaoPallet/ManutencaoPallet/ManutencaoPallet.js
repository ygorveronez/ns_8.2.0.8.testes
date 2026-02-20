/// <reference path="../../Enumeradores/EnumSituacaoPallet.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Enumeradores/EnumRegraPallet.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCodigoFiltroPesquisa.js" />

var _pesquisaManutencaoPallet;
var _gridManutencaoPallet;
var _cabecalhoManutencaoPallet;
var _adicionarManutencaoPallet;

function inicializarManutencaoPallet() {
    _pesquisaManutencaoPallet = new PesquisaManutencaoPallet();
    KoBindings(_pesquisaManutencaoPallet, "knockoutPesquisaManutencaoPallet", false, _pesquisaManutencaoPallet.Pesquisar.id);

    _cabecalhoManutencaoPallet = new TotalizadoresManutencaoPallet();
    KoBindings(_cabecalhoManutencaoPallet, "knoutCabecalhoManutencaoPallet");

    _adicionarManutencaoPallet = new AdicionarManutencaoPallet();
    KoBindings(_adicionarManutencaoPallet, "knoutAdicionarManutencaoPallet");

    BuscarFilial(_pesquisaManutencaoPallet.Filial);
    BuscarTransportadores(_pesquisaManutencaoPallet.Transportador);
    BuscarClientes(_pesquisaManutencaoPallet.Cliente);

    BuscarFilial(_adicionarManutencaoPallet.Filial);

    carregarGridManutencaoPallet();
}

var PesquisaManutencaoPallet = function () {
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoMovimentacao = PropertyEntity({ text: "Tipo Movimentação", val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos });
    this.TipoManutencaoPallet = PropertyEntity({ text: "Tipo Manutenção Pallet", val: ko.observable(EnumTipoManutencaoPallet.Todos), options: EnumTipoManutencaoPallet.obterOpcoesPesquisa(), def: EnumTipoManutencaoPallet.Todos });
    this.DataInicialMovimentacao = PropertyEntity({ text: "Dt. Inicial Movimentação", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), visible: ko.observable(true) });
    this.DataFinalMovimentacao = PropertyEntity({ text: "Dt. Final Movimentação", getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.DataInicialMovimentacao.dateRangeLimit = this.DataFinalMovimentacao;
    this.DataFinalMovimentacao.dateRangeInit = this.DataInicialMovimentacao;

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.ManutencaoPallet,
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.ManutencaoPallet, _pesquisaManutencaoPallet) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });
    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            fecharFiltrosManutencaoPallet();
            _gridManutencaoPallet.CarregarGrid();
            ObterTotaisManutencaoPallet();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var TotalizadoresManutencaoPallet = function () {
    this.EmManutencao = PropertyEntity({ text: "Em Manutenção", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Disponivel = PropertyEntity({ text: "Disponível para uso", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Descarte = PropertyEntity({ text: "Descarte", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
    this.Sucata = PropertyEntity({ text: "Sucata", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true) });
};

var AdicionarManutencaoPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ text: "*Quantidade de Palllet", val: ko.observable(""), def: "", visible: ko.observable(true), required: ko.observable(true) });
    this.TipoManutencaoPallet = PropertyEntity({ text: "*Tipo Manutenção Pallet", val: ko.observable(EnumEntradaSaida.Todos), options: EnumTipoManutencaoPallet.obterOpcoesCadastro(), def: EnumEntradaSaida.Todos, required: ko.observable(true), visible: ko.observable(true) });
    this.TipoEntradaSaida = PropertyEntity({ text: "*Tipo Movimentação", val: ko.observable(EnumEntradaSaida.Saida), options: EnumEntradaSaida.obterOpcoes(), def: EnumEntradaSaida.Saida, required: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarManutencaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: fecharModalAdicionarManutencaoPallet, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    this.TipoEntradaSaida.val.subscribe(function (newValue) {
        let isSaida = newValue == EnumEntradaSaida.Saida;

        _adicionarManutencaoPallet.TipoManutencaoPallet.visible(isSaida);
        _adicionarManutencaoPallet.TipoManutencaoPallet.required(isSaida);

        if (!isSaida)
            _adicionarManutencaoPallet.TipoManutencaoPallet.val(_adicionarManutencaoPallet.TipoManutencaoPallet.def);
    });

};

// #endregion Knouts

function carregarGridManutencaoPallet() {
    const configuracaoExportacao = {
        url: "ManutencaoPallet/ExportarPesquisa",
        titulo: "Manutenção de Pallet"
    };

    _gridManutencaoPallet = new GridViewExportacao("grid-gestao-pallet-manutencao-pallet", "ManutencaoPallet/Pesquisa", _pesquisaManutencaoPallet, null, configuracaoExportacao, null, 25);
    _gridManutencaoPallet.SetPermitirEdicaoColunas(true);
    _gridManutencaoPallet.SetSalvarPreferenciasGrid(true);
    _gridManutencaoPallet.CarregarGrid();

    ObterTotaisManutencaoPallet();
}

function ObterTotaisManutencaoPallet() {
    const dados = RetornarObjetoPesquisa(_pesquisaManutencaoPallet);

    executarReST("ManutencaoPallet/ObterTotais", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_cabecalhoManutencaoPallet, arg);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function abrirBuscaFiltrosManual(e) {
    const buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {
        if (retorno.Codigo == 0) return;

        e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
        e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

        PreencherJsonFiltroPesquisa(_pesquisaManutencaoPallet, retorno.Dados);
    }, EnumCodigoFiltroPesquisa.ManutencaoPallet);

    buscaFiltros.AbrirBusca();
}

function exibirFiltrosManutencaoPallet() {
    Global.abrirModal('divModalFiltroPesquisaManutencaoPallet');
}

function fecharFiltrosManutencaoPallet() {
    Global.fecharModal('divModalFiltroPesquisaManutencaoPallet');
}

function exibirModalAdicionarManutencaoPallet() {
    Global.abrirModal('divModalAdicionarManutencaoPallet');
}

function fecharModalAdicionarManutencaoPallet() {
    LimparCampos(_adicionarManutencaoPallet);
    Global.fecharModal('divModalAdicionarManutencaoPallet');
}

function AdicionarManutencaoClick() {
    if (!ValidarCamposObrigatorios(_adicionarManutencaoPallet)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja adicionar essa manutenção?", function () {
        const data = RetornarObjetoPesquisa(_adicionarManutencaoPallet);
        executarReST("ManutencaoPallet/Adicionar", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                fecharModalAdicionarManutencaoPallet();
                carregarGridManutencaoPallet();
                } else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}