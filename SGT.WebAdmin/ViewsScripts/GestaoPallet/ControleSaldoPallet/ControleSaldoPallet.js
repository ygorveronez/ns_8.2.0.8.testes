/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacaoPallet.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumCodigoFiltroPesquisa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSituacaoPalletGestaoDevolucao.js" />

var _pesquisaControleSaldo;
var _gridControleSaldo;
var _cabecalhoControleSaldo;
var _detalhesControleSaldo;

function inicializarControleSaldo() {
    _pesquisaControleSaldo = new PesquisaControleSaldo();
    KoBindings(_pesquisaControleSaldo, "knockoutPesquisaControleSaldo", false, _pesquisaControleSaldo.Pesquisar.id);

    _cabecalhoControleSaldo = new TotalizadoresControleSaldo();
    KoBindings(_cabecalhoControleSaldo, "knoutCabecalhoControleSaldo");

    _detalhesControleSaldo = new DetalhesControleSaldo();
    KoBindings(_detalhesControleSaldo, "knoutDetalhesControleSaldo");

    BuscarFilial(_pesquisaControleSaldo.Filial);

    loadAnexos();

    carregarGridControleSaldo();
}

var PesquisaControleSaldo = function () {
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", getType: typesKnockout.int, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial", issue: 69, idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoPallet = PropertyEntity({ text: "Situacao Pallet", val: ko.observable(EnumSituacaoPallet.Todos), options: EnumSituacaoPallet.obterOpcoesPesquisa(), def: EnumSituacaoPallet.Todos });
    this.SituacaoPalletGestaoDevolucao = PropertyEntity({ text: "Situacao Pallet Gestão Devolução", val: ko.observable(EnumSituacaoPalletGestaoDevolucao.Todos), options: EnumSituacaoPalletGestaoDevolucao.obterOpcoesPesquisa(), def: EnumSituacaoPalletGestaoDevolucao.Todos });
    this.DataInicialCriacaoCarga = PropertyEntity({ text: "Dt. Inicial Criação da Carga", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });
    this.DataFinalCriacaoCarga = PropertyEntity({ text: "Dt. Final Criação da Carga", getType: typesKnockout.date, val: ko.observable(""), visible: ko.observable(true) });

    this.DataInicialCriacaoCarga.dateRangeLimit = this.DataFinalCriacaoCarga;
    this.DataFinalCriacaoCarga.dateRangeInit = this.DataInicialCriacaoCarga;

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.ControleSaldo,
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.ControleSaldo, _pesquisaControleSaldo) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });
    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({
        type: types.event, eventClick: function (e) {
            fecharFiltros();
            _gridControleSaldo.CarregarGrid();
            ObterTotaisControleSaldos();
        }, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var TotalizadoresControleSaldo = function () {
    //Aba Situacao
    this.Todos = PropertyEntity({ text: "Todos", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-situacao-todos", eventClick: () => carouselSituacaoClick(this.Todos, EnumSituacaoPallet.Todos) });
    this.Pendente = PropertyEntity({ text: "Pendente", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-situacao-pendente", eventClick: () => carouselSituacaoClick(this.Pendente, EnumSituacaoPallet.Pendente) });
    this.Devolvido = PropertyEntity({ text: "Devolvido", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-situacao-devolvido", eventClick: () => carouselSituacaoClick(this.Devolvido, EnumSituacaoPallet.Concluido) });

    this.TotalSaldo = PropertyEntity({ text: "Total de Pallet Disponivel:", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), requiredClass: ko.observable("") });
    this.PalettsPendente = PropertyEntity({ text: "Total de Pallet Pendente:", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), requiredClass: ko.observable("") });
    this.PalettsReservado = PropertyEntity({ text: "Total de Pallet Reservado:", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), requiredClass: ko.observable("") });

    //Aba Notas
    this.NotasNoPrazo = PropertyEntity({ text: "No Prazo", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor), id: "view-notasPallet-noPrazo", eventClick: () => carouselNotaPalletClick(this.NotasNoPrazo, EnumSituacaoPalletGestaoDevolucao.NoPrazo) });
    this.NotasVencido = PropertyEntity({ text: "Vencido", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor), id: "view-notasPallet-vencido", eventClick: () => carouselNotaPalletClick(this.NotasVencido, EnumSituacaoPalletGestaoDevolucao.Vencido) });
    this.NotasAgendado = PropertyEntity({ text: "Agendado", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-notasPallet-agendado", eventClick: () => carouselNotaPalletClick(this.NotasAgendado, EnumSituacaoPalletGestaoDevolucao.Agendado) });
    this.NotasPermuta = PropertyEntity({ text: "Em Permuta", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-notasPallet-permuta", eventClick: () => carouselNotaPalletClick(this.NotasPermuta, EnumSituacaoPalletGestaoDevolucao.Permuta) });

    //Aba Paletts
    this.PalettsNoPrazo = PropertyEntity({ text: "No Prazo", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor), id: "view-notasPallet-noPrazo", eventClick: () => carouselNotaPalletClick(this.PalettsNoPrazo, EnumSituacaoPalletGestaoDevolucao.NoPrazo) });
    this.PalettsVencido = PropertyEntity({ text: "Vencido", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor), id: "view-notasPallet-vencido", eventClick: () => carouselNotaPalletClick(this.PalettsVencido, EnumSituacaoPalletGestaoDevolucao.Vencido) });
    this.PalettsAgendado = PropertyEntity({ text: "Agendado", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-notasPallet-agendado", eventClick: () => carouselNotaPalletClick(this.PalettsAgendado, EnumSituacaoPalletGestaoDevolucao.Agendado) });
    this.PalettsPermuta = PropertyEntity({ text: "Em Permuta", getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(true), id: "view-notasPallet-permuta", eventClick: () => carouselNotaPalletClick(this.PalettsPermuta, EnumSituacaoPalletGestaoDevolucao.Permuta) });
};

var DetalhesControleSaldo = function () {
    this.SituacaoPallet = PropertyEntity({ text: "Status", val: ko.observable(""), def: "" });
    this.Origem = PropertyEntity({ text: "Origem", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Destino = PropertyEntity({ text: "Destino", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ text: "Carga", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ text: "Quantidade de Paletes", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroNotaFiscalPermuta = PropertyEntity({ text: "Número NF Permuta", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SerieNotaFiscalPermuta = PropertyEntity({ text: "Série NF Permuta", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroNotaFiscalDevolucao = PropertyEntity({ text: "Número NFD", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SerieNotaFiscalDevolucao = PropertyEntity({ text: "Série NFD", val: ko.observable(""), def: "", visible: ko.observable(true) });
};

// #endregion Knouts

function carregarGridControleSaldo() {
    const configuracaoExportacao = {
        url: "ControleSaldoPallet/ExportarPesquisa",
        titulo: "Controle de Saldo"
    };

    const detalhes = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: detalhesControleSaldoClick,
        tamanho: "10",
        icone: "",
        visibilidade: true
    };

    const auditoria = {
        descricao: "Auditar",
        id: guid(),
        evento: "onclick",
        metodo: OpcaoAuditoria("MovimentacaoPallet"),
        tamanho: "10",
        icone: "",
        visibilidade: VisibilidadeOpcaoAuditoria
    }

    const anexos = {
        descricao: "Anexos",
        id: guid(),
        evento: "onclick",
        metodo: gerenciarAnexosClick,
        tamanho: "10",
        icone: "",
        visibilidade: true
    }

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhes, auditoria, anexos] };

    _gridControleSaldo = new GridView("grid-gestao-pallet-controle-saldo", "ControleSaldoPallet/Pesquisa", _pesquisaControleSaldo, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao, null, null, null, callbackColumnDefaultControleSaldoPallet);
    _gridControleSaldo.SetPermitirEdicaoColunas(true);
    _gridControleSaldo.SetSalvarPreferenciasGrid(true);
    _gridControleSaldo.CarregarGrid();

    ObterTotaisControleSaldos();
}

function callbackColumnDefaultControleSaldoPallet(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "LeadTimeDevolucaoFormatado") {
        if (dadosLinha.SituacaoDevolucao == "Pendente") {
            setTimeout(function () {
                $('#' + cabecalho.name + '-' + dadosLinha.DT_RowId)
                    .countdown(moment(valorColuna, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                    .on('update.countdown', function (event) {
                        if (event.offset.totalDays > 0)
                            $(this).text(event.strftime('%-Dd %H:%M:%S'));
                        else
                            $(this).text(event.strftime('%H:%M:%S'));
                    })
                    .on('finish.countdown', function (event) {
                        $(this).text("Tempo limite atingido");
                    });
            }, 1000);
        }

        return '<span id="' + cabecalho.name + '-' + dadosLinha.DT_RowId + '"></span>';
    }
}

function detalhesControleSaldoClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo }

    executarReST("ControleSaldoPallet/BuscarPorCodigo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                PreencherObjetoKnout(_detalhesControleSaldo, retorno);
                _detalhesControleSaldo.SituacaoPallet.val(retorno.Data.Situacao);
                _detalhesControleSaldo.Origem.val(retorno.Data.Carga.DadosSumarizados.Origem);
                _detalhesControleSaldo.Destino.val(retorno.Data.Carga.DadosSumarizados.Destino);
                _detalhesControleSaldo.Carga.val(retorno.Data.Carga.CodigoCargaEmbarcador);

                exibirModalDetalhesControleSaldo();

                $("#modalDetalhePedido").one('hidden.bs.modal', function () {
                    LimparCampos(_detalhesControleSaldo);
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function RecarregarGridControleSaldo() {
}

function ObterTotaisControleSaldos() {
    var dados = RetornarObjetoPesquisa(_pesquisaControleSaldo);

    executarReST("ControleSaldoPallet/ObterTotais", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _cabecalhoControleSaldo.Todos.val(arg.Data.Total);
                _cabecalhoControleSaldo.Pendente.val(arg.Data.TotalPendente);
                _cabecalhoControleSaldo.Devolvido.val(arg.Data.TotalDevolvido);

                _cabecalhoControleSaldo.TotalSaldo.val(arg.Data.TotalSaldo);
                _cabecalhoControleSaldo.TotalSaldo.requiredClass('text-success');

                _cabecalhoControleSaldo.PalettsPendente.val(arg.Data.PalettsPendente);
                _cabecalhoControleSaldo.PalettsPendente.requiredClass('text-danger');

                _cabecalhoControleSaldo.PalettsReservado.val(arg.Data.PalettsReservado)
                //_cabecalhoControleSaldo.PalettsPendente.requiredClass('text-danger');

                _cabecalhoControleSaldo.NotasNoPrazo.val(arg.Data.NotasNoPrazo);
                _cabecalhoControleSaldo.NotasVencido.val(arg.Data.NotasVencido);
                _cabecalhoControleSaldo.NotasAgendado.val(arg.Data.NotasAgendado);
                _cabecalhoControleSaldo.NotasPermuta.val(arg.Data.NotasPermuta);

                _cabecalhoControleSaldo.PalettsNoPrazo.val(arg.Data.PalettsNoPrazo);
                _cabecalhoControleSaldo.PalettsVencido.val(arg.Data.PalettsVencido);
                _cabecalhoControleSaldo.PalettsAgendado.val(arg.Data.PalettsAgendado);
                _cabecalhoControleSaldo.PalettsPermuta.val(arg.Data.PalettsPermuta);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {
        if (retorno.Codigo == 0) return;

        e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
        e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

        PreencherJsonFiltroPesquisa(_pesquisaControleSaldo, retorno.Dados);
    }, EnumCodigoFiltroPesquisa.ControleSaldo);

    buscaFiltros.AbrirBusca();
}

function exibirFiltros() {
    Global.abrirModal('divModalFiltroPesquisaControleSaldo');
}

function fecharFiltros() {
    Global.fecharModal('divModalFiltroPesquisaControleSaldo');
}

function exibirModalDetalhesControleSaldo() {
    Global.abrirModal('modalDetalhesControleSaldo');
}

function fecharModalDetalhesControleSaldo() {
    Global.fecharModal('modalDetalhesControleSaldo');
}

function carouselSituacaoClick(item, situacao) {
    if (!controlarItemCarousel(item.id)) {
        situacao = '';
    };

    _pesquisaControleSaldo.SituacaoPallet.val(situacao);

    _gridControleSaldo.CarregarGrid();
    ObterTotaisControleSaldos();
}

function carouselNotaPalletClick(item, situacao) {
    if (!controlarItemCarousel(item.id)) {
        situacao = '';
    };

    _pesquisaControleSaldo.SituacaoPalletGestaoDevolucao.val(situacao);

    _gridControleSaldo.CarregarGrid();
    ObterTotaisControleSaldos();
}

function controlarItemCarousel(id) {
    let apenasRemover = $(`#${id}`).hasClass('active');

    const grupo = id.split('-')[1];
    $(`[id^='view-${grupo}-']`).each(function () {
        $(this).removeClass('active');
    });

    if (apenasRemover)
        return false;

    $(`[id^='${id}']`).each(function () {
        $(this).addClass('active');
    });

    return true;
}
