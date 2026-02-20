/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDevolucaoPallet.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Baixa.js" />
/// <reference path="DetalhesDevolucao.js" />
/// <reference path="CancelamentoBaixa.js" />
/// <reference path="Contestacao.js" />
/// <reference path="Liquidacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDevolucaoPallets;
var _pesquisaDevolucaoPallets;
var _atualizarDevolucaoPallets;

/*
 * Declaração das Classes
 */

var DevolucaoPallets = function () {
    this.Carga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataBaixaInicial = PropertyEntity({ text: "Baixa Pallet (Início):", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataBaixaFinal = PropertyEntity({ text: "Baixa Pallet (Fim):", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoInicial = PropertyEntity({ text: "Emissão NF-e (Início):", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Emissão NF-e (Fim):", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", getType: typesKnockout.int, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroDevolucao = PropertyEntity({ text: "Nº Devolução:", getType: typesKnockout.int, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoDevolucaoPallet.Todas), options: EnumSituacaoDevolucaoPallet.obterOpcoesPesquisa(), def: EnumSituacaoDevolucaoPallet.Todas, text: "Situação:", visible: ko.observable(true) });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    
    this.DataBaixaFinal.dateRangeInit = this.DataBaixaInicial;
    this.DataBaixaInicial.dateRangeLimit = this.DataBaixaFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDevolucaoPallets.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var AtualizarDevolucaoPallets = function () {
    this.Devolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.QuantidadePallets = PropertyEntity({ getType: typesKnockout.int, maxlength: 5, text: "*Nº Pallets:", required: true });

    this.Confirmar = PropertyEntity({ eventClick: function (e) { AtualizarDevolucaoPalletsClick() }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadDevolucaoPallets() {
    _pesquisaDevolucaoPallets = new DevolucaoPallets();
    KoBindings(_pesquisaDevolucaoPallets, "knockoutPesquisaPallets", _pesquisaDevolucaoPallets.Pesquisar.id);

    _atualizarDevolucaoPallets = new AtualizarDevolucaoPallets();
    KoBindings(_atualizarDevolucaoPallets, "knoutAtualizarDevolucaoPallets");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaDevolucaoPallets.Filial.visible(false);
        _pesquisaDevolucaoPallets.Transportador.visible(false);
        _pesquisaDevolucaoPallets.Motorista.visible(false);
        _pesquisaDevolucaoPallets.Veiculo.visible(false);
    } else {
        LoadBaixaPallets();
    }
    new BuscarFilial(_pesquisaDevolucaoPallets.Filial);
    new BuscarTransportadores(_pesquisaDevolucaoPallets.Transportador, null, null, true, null);
    new BuscarMotoristas(_pesquisaDevolucaoPallets.Motorista, null, _pesquisaDevolucaoPallets.Transportador);
    new BuscarVeiculos(_pesquisaDevolucaoPallets.Veiculo, null, _pesquisaDevolucaoPallets.Transportador);
    new BuscarClientes(_pesquisaDevolucaoPallets.Remetente);
    new BuscarGruposPessoas(_pesquisaDevolucaoPallets.GrupoPessoas);
    new BuscarTiposOperacao(_pesquisaDevolucaoPallets.TipoOperacao);
    new BuscarClientes(_pesquisaDevolucaoPallets.Tomador);


    loadDetalheDevolucao();
    LoadCancelamentoBaixaPallets();
    loadContestacao();
    BuscarDevolucaoPallets();
    loadLiquidacaoPallet();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function detalhesDevolucaoClick(e) {

    BuscarDetalheDevolucao(e.Codigo, function () {
        Global.abrirModal("ModalDivDetalhesDevolucao");
    });
}

function DownloadComprovanteEntregaClick(e) {
    executarDownload("Devolucao/DownloadComprovanteEntrega", { Codigo: e.Codigo });
}

function AbrirTelaAtualizarDevolucaoPalletsClick(devolucaoPalletsGrid) {
    LimparCamposAtualizarDevolucaoPallets();
    _atualizarDevolucaoPallets.Devolucao.val(devolucaoPalletsGrid.Codigo);
    _atualizarDevolucaoPallets.QuantidadePallets.val(devolucaoPalletsGrid.Pallets);
    Global.abrirModal("divModalAtualizarDevolucaoPallets");
}

/*
 * Declaração das Funções
 */

function BuscarDevolucaoPallets() {
    var menuOpcoes = null;
    var detalhes = { descricao: "Ver detalhes", id: guid(), evento: "onclick", metodo: detalhesDevolucaoClick, tamanho: "10", icone: "" };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [detalhes], tamanho: 10, };
    }
    else {
        var baixaPallets = { descricao: "Baixar Pallets", id: guid(), evento: "onclick", metodo: AbrirTelaBaixaPalletsClick, tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoBaixarPallets };
        var cancelarBaixa = { descricao: "Cancelar Baixa", id: guid(), evento: "onclick", metodo: AbrirTelaCancelamentoBaixaPalletsClick, tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoCancelarBaixa };
        var downloadComprovanteEntrega = { descricao: "Comprovante de Entrega", id: guid(), evento: "onclick", metodo: DownloadComprovanteEntregaClick, tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoComprovanteEntrega };
        var detalhes = { descricao: "Ver detalhes", id: guid(), evento: "onclick", metodo: detalhesDevolucaoClick, tamanho: "10", icone: "" };
        var editar = { descricao: "Editar Devolução", id: guid(), evento: "onclick", metodo: AbrirTelaAtualizarDevolucaoPalletsClick, tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoEditarDevolucaoPallets };
        var auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("DevolucaoPallet"), icone: "" };
        var liquidacao = { descricao: "Liquidação", id: guid(), metodo: AbrirModalLiquidacaoClick, icone: "" };
        var contestacao = { descricao: "Contestação", id: guid(), metodo: abrirModalContestacao, icone: "" };

        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [baixaPallets, cancelarBaixa, downloadComprovanteEntrega, detalhes, editar, auditar, liquidacao, contestacao], tamanho: 10, };
    }

    _gridDevolucaoPallets = new GridView(_pesquisaDevolucaoPallets.Pesquisar.idGrid, "Devolucao/Pesquisa", _pesquisaDevolucaoPallets, menuOpcoes, { column: 1, dir: orderDir.asc }, 10);
    _gridDevolucaoPallets.CarregarGrid();
}

function VisibilidadeOpcaoBaixarPallets(e) {
    return (e.Situacao == EnumSituacaoDevolucaoPallet.AgEntrega);
}

function VisibilidadeOpcaoCancelarBaixa(e) {
    return (e.Situacao == EnumSituacaoDevolucaoPallet.Entregue);
}

function VisibilidadeOpcaoComprovanteEntrega(e) {
    return (e.Situacao == EnumSituacaoDevolucaoPallet.Entregue && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);
}

function VisibilidadeOpcaoEditarDevolucaoPallets(e) {
    return _CONFIGURACAO_TMS.UtilizarControlePallets && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS;
}

function AtualizarDevolucaoPalletsClick() {
    exibirConfirmacao("Devolução de Pallets", "Tem certeza que deseja atualizar a devolução de pallets?", function () {
        Salvar(_atualizarDevolucaoPallets, "Devolucao/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.abrirModal('divModalAtualizarDevolucaoPallets');
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualização realizada com sucesso!");
                    _gridDevolucaoPallets.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function LimparCamposAtualizarDevolucaoPallets() {
    LimparCampos(_atualizarDevolucaoPallets);
}