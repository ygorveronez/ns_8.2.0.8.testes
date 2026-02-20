/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentacaoEstoquePallet.js" />
/// <reference path="../../Enumeradores/EnumTipoOperacaoMovimentacaoEstoquePallet.js" />

// #region Objetos Globais do Arquivo

var _ajusteSaldo;
var _ajusteSaldoCadastro;
var _ajusteSaldoPesquisa;
var _gridAjusteSaldo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var AjusteSaldo = function () {
    this.DiasRotatividade = PropertyEntity({ text: "", val: ko.observable("0"), def: "0" });
    this.SaldoPorRotatividade = PropertyEntity({ text: "", val: ko.observable("0"), def: "0", visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAjusteSaldoModalClick, type: types.event, text: "Adicionar Ajuste de Saldo", idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });
}

var AjusteSaldoCadastro = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial:", idBtnSearch: guid(), required: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.Observacao = PropertyEntity({ text: "*Observação:", val: ko.observable(""), def: "", maxlength: 400, required: true });
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 7, required: true, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Setor:", idBtnSearch: guid(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS), enable: ko.observable(false) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "*Empresa/Filial:" : "*Transportador:"), idBtnSearch: guid(), required: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        this.TipoOperacaoMovimentacao = PropertyEntity({ val: ko.observable(EnumTipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada), options: EnumTipoOperacaoMovimentacaoEstoquePallet.obterOpcoesTMS(), def: EnumTipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada, text: "*Tipo da Movimentação:" });
    else
        this.TipoOperacaoMovimentacao = PropertyEntity({ val: ko.observable(EnumTipoOperacaoMovimentacaoEstoquePallet.FilialEntrada), options: EnumTipoOperacaoMovimentacaoEstoquePallet.obterOpcoes(), def: EnumTipoOperacaoMovimentacaoEstoquePallet.FilialEntrada, text: "*Tipo da Movimentação:" });
   
    this.Filial.codEntity.subscribe(controlarSetor);
    this.TipoOperacaoMovimentacao.val.subscribe(controlarTipoOperacaoMovimentacao);

    this.Adicionar = PropertyEntity({ eventClick: adicionarAjusteSaldoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var AjusteSaldoPesquisa  = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:" });
    this.DataMovimentoInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataMovimentoFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroDocumento = PropertyEntity({ text: "Número do Documento: " });
    this.RetornarSaldoFilial = PropertyEntity({ val: ko.observable(false), text: "Retornar saldo das filiais", def: false, getType: typesKnockout.bool });
    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentacaoEstoquePallet.Todas), def: EnumTipoMovimentacaoEstoquePallet.Todas, text: "Tipo de Movimento:", options: EnumTipoMovimentacaoEstoquePallet.ObterOpcoesPesquisa() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador:"), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });

    this.DataMovimentoInicial.dateRangeLimit = this.DataMovimentoFinal;
    this.DataMovimentoFinal.dateRangeInit = this.DataMovimentoInicial;

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarAjusteSaldoClick, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({ eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadAjusteSaldo() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        $("#lblTituloPagina").text("Saldo de Pallets");

    _ajusteSaldoPesquisa = new AjusteSaldoPesquisa();
    KoBindings(_ajusteSaldoPesquisa, "knockoutAjusteSaldoPesquisa", false);

    _ajusteSaldo = new AjusteSaldo();
    KoBindings(_ajusteSaldo, "knockoutAjusteSaldo", false);

    _ajusteSaldoCadastro = new AjusteSaldoCadastro();
    KoBindings(_ajusteSaldoCadastro, "knockoutAjusteSaldoCadastro");
    HeaderAuditoria("EstoquePallet", _ajusteSaldoCadastro);

    new BuscarFilial(_ajusteSaldoPesquisa.Filial);
    new BuscarTransportadores(_ajusteSaldoPesquisa.Transportador, null, null, true);

    new BuscarClientes(_ajusteSaldoCadastro.Cliente);
    new BuscarFilial(_ajusteSaldoCadastro.Filial);
    new BuscarSetorFuncionario(_ajusteSaldoCadastro.Setor, null, _ajusteSaldoCadastro.Filial);
    new BuscarTransportadores(_ajusteSaldoCadastro.Transportador, null, null, true);

    loadGridAjusteSaldo();
    buscarSaldoPorRotatividade();
}

function loadGridAjusteSaldo() {
    _gridAjusteSaldo = new GridView(_ajusteSaldo.Adicionar.idGrid, "AjusteSaldo/PesquisaExtrato", _ajusteSaldoPesquisa, null, { column: 1, dir: orderDir.desc }, 10);
    _gridAjusteSaldo.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarAjusteSaldoClick() {
    Salvar(_ajusteSaldoCadastro, "AjusteSaldo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Ajuste adicionado com sucesso!");
                Global.fecharModal('divModalAjusteSaldoCadastro');
                recarregarAjusteSaldo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function adicionarAjusteSaldoModalClick() {
    Global.abrirModal('divModalAjusteSaldoCadastro');
    $("#divModalAjusteSaldoCadastro").one('hidden.bs.modal', function () {
        LimparCampos(_ajusteSaldoCadastro);
    });
}

function pesquisarAjusteSaldoClick() {
    _ajusteSaldoPesquisa.ExibirFiltros.visibleFade(false);

    recarregarAjusteSaldo();
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function buscarSaldoPorRotatividade() {
    _ajusteSaldo.SaldoPorRotatividade.visible(false);

    if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) && (_ajusteSaldoPesquisa.Transportador.codEntity() == 0))
        return;

    executarReST("AjusteSaldo/ObterSaldoPorRotatividade", { Transportador: _ajusteSaldoPesquisa.Transportador.codEntity() }, function (retorno) {
        if (retorno.Success && retorno.Data !== false) {
            _ajusteSaldo.SaldoPorRotatividade.visible(true);
            _ajusteSaldo.SaldoPorRotatividade.val(retorno.Data.Saldo);
            _ajusteSaldo.DiasRotatividade.val(retorno.Data.Dias);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function controlarSetor(codigoFilial) {
    _ajusteSaldoCadastro.Setor.enable(codigoFilial > 0);
    LimparCampo(_ajusteSaldoCadastro.Setor);
}

function controlarTipoOperacaoMovimentacao(tipoOperacaoMovimentacaoSelecionada) {
    var clienteVisivel = false;
    var filialVisivel = false;
    var transportadorVisivel = false;

    switch (tipoOperacaoMovimentacaoSelecionada) {
        case EnumTipoOperacaoMovimentacaoEstoquePallet.ClienteEntrada:
        case EnumTipoOperacaoMovimentacaoEstoquePallet.ClienteSaida:
            clienteVisivel = true;
            break;

        case EnumTipoOperacaoMovimentacaoEstoquePallet.FilialEntrada:
        case EnumTipoOperacaoMovimentacaoEstoquePallet.FilialSaida:
            filialVisivel = true;
            break;

        case EnumTipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada:
        case EnumTipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida:
            transportadorVisivel = true;
            break;

        case EnumTipoOperacaoMovimentacaoEstoquePallet.FilialTransportador:
        case EnumTipoOperacaoMovimentacaoEstoquePallet.TransportadorFilial:
            filialVisivel = true;
            transportadorVisivel = true;
            break;

        case EnumTipoOperacaoMovimentacaoEstoquePallet.FilialCliente:
        case EnumTipoOperacaoMovimentacaoEstoquePallet.ClienteFilial:
            clienteVisivel = true;
            filialVisivel = true;
            break;

        case EnumTipoOperacaoMovimentacaoEstoquePallet.ClienteTransportador:
        case EnumTipoOperacaoMovimentacaoEstoquePallet.TransportadorCliente:
            clienteVisivel = true;
            transportadorVisivel = true;
            break;
    }

    _ajusteSaldoCadastro.Cliente.required(clienteVisivel);
    _ajusteSaldoCadastro.Cliente.visible(clienteVisivel);
    _ajusteSaldoCadastro.Filial.required(filialVisivel);
    _ajusteSaldoCadastro.Filial.visible(filialVisivel);
    _ajusteSaldoCadastro.Setor.visible(filialVisivel);
    _ajusteSaldoCadastro.Transportador.required(transportadorVisivel);
    _ajusteSaldoCadastro.Transportador.visible(transportadorVisivel);

    if (!clienteVisivel)
        LimparCampo(_ajusteSaldoCadastro.Cliente);

    if (!filialVisivel)
        LimparCampo(_ajusteSaldoCadastro.Filial);

    if (!transportadorVisivel)
        LimparCampo(_ajusteSaldoCadastro.Transportador);
}

function recarregarAjusteSaldo() {
    buscarSaldoPorRotatividade();
    recarregarGridAjusteSaldo();
}

function recarregarGridAjusteSaldo() {
    _gridAjusteSaldo.CarregarGrid();
}

// #endregion Funções Privadas
