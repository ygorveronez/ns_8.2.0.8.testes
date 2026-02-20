/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Titulo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAutorizacaoPagamentoEletronico.js" />
/// <reference path="../../Enumeradores/EnumDescricaoUsoEmpresaPagamentoEletronico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRemessaPagamento;
var _remessaPagamento;
var _pesquisaRemessaPagamento;

var _TipoContaPagamentoEletronico = [
    { text: "01 - Crédito em conta corrente", value: EnumTipoContaPagamentoEletronico.ContaCorrenteIndividual }
];

var _FinalidadePagamentoEletronico = [
    { text: "01 - Conta corrente individual", value: EnumFinalidadePagamentoEletronico.CreditoContaCorrente }
];

var _ModalidadePagamentoEletronico = [
    { text: "CC - Crédito em conta corrente", value: EnumModalidadePagamentoEletronico.CC_CreditoContaCorrente },
    { text: "OP - Cheque OP", value: EnumModalidadePagamentoEletronico.OP_ChequeOP },
    { text: "DOC - DOC COMPRE", value: EnumModalidadePagamentoEletronico.DOC_DOCCompre },
    { text: "CCR - Crédito Conta", value: EnumModalidadePagamentoEletronico.CCR_CreditoConta },
    { text: "Boleto", value: EnumModalidadePagamentoEletronico.TDC_TEDCip },
    { text: "TDS - TED STR", value: EnumModalidadePagamentoEletronico.TDS_TEDSTR },
    { text: "TT - Titulos de Terceiro", value: EnumModalidadePagamentoEletronico.TT_TituloTerceiro },
    { text: "TRB - Tributos (Sem Código de Barras)", value: EnumModalidadePagamentoEletronico.TRB_Tributos },
    { text: "CCT - Contas de Consumo/Tributos (Com Código de Barras)", value: EnumModalidadePagamentoEletronico.CCT_ContasConsumoTributo }
];

var PesquisaRemessaPagamento = function () {
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Configuração Banco:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataPagamentoInicial = PropertyEntity({ text: "Data Pagamento De: ", required: false, getType: typesKnockout.date, enable: true });
    this.DataPagamentoFinal = PropertyEntity({ text: "Até: ", required: false, getType: typesKnockout.date, enable: true });
    this.PagamentoEletronico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remessa Pagamento:", idBtnSearch: guid() });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid() });
    this.SituacaoAutorizacaoPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumSituacaoAutorizacaoPagamentoEletronico.Todos), options: EnumSituacaoAutorizacaoPagamentoEletronico.obterOpcoesPesquisa(), text: "Situação: ", def: EnumSituacaoAutorizacaoPagamentoEletronico.Todos, required: false, enable: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRemessaPagamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var RemessaPagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "*Número Sequencial: ", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: false });
    this.DataPagamento = PropertyEntity({ text: "*Data do Pagamento: ", required: true, getType: typesKnockout.date, enable: false });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Configuração Banco:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: false });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: false });
    this.DataGeracao = PropertyEntity({ text: "*Data da Geração: ", required: true, getType: typesKnockout.date, enable: false });
    this.ModalidadePagamentoEletronico = PropertyEntity({ val: ko.observable(EnumModalidadePagamentoEletronico.CC_CreditoContaCorrenteo), options: _ModalidadePagamentoEletronico, text: "*Modalidade: ", def: EnumModalidadePagamentoEletronico.CC_CreditoContaCorrente, required: true, enable: false });
    this.TipoContaPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumTipoContaPagamentoEletronico.ContaCorrenteIndividual), options: _TipoContaPagamentoEletronico, text: "*Tipo Conta: ", def: EnumTipoContaPagamentoEletronico.ContaCorrenteIndividual, required: true, enable: false });
    this.FinalidadePagamentoEletronico = PropertyEntity({ val: ko.observable(EnumFinalidadePagamentoEletronico.CreditoContaCorrente), options: _FinalidadePagamentoEletronico, text: "*Finalidade: ", def: EnumFinalidadePagamentoEletronico.CreditoContaCorrente, required: true, enable: false });
    this.DescricaoUsoEmpresaPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumDescricaoUsoEmpresaPagamentoEletronico.Nenhum), options: EnumDescricaoUsoEmpresaPagamentoEletronico.obterOpcoes(), text: "Desc. Uso da Empresa: ", def: EnumDescricaoUsoEmpresaPagamentoEletronico.Nenhum, required: true, enable: false });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Operador:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: false });
    this.QuantidadeTitulos = PropertyEntity({ text: "*Qtd. Títulos: ", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: false });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total: ", required: true, getType: typesKnockout.decimal, enable: false });
    this.SituacaoAutorizacaoPagamentoEletronico = PropertyEntity({ val: ko.observable(EnumSituacaoAutorizacaoPagamentoEletronico.Iniciada), options: EnumSituacaoAutorizacaoPagamentoEletronico.obterOpcoes(), text: "*Situação: ", def: EnumSituacaoAutorizacaoPagamentoEletronico.Iniciada, required: true, enable: false });

    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir / Limpar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.DownloadRelatorio = PropertyEntity({ eventClick: DownloadRelatorioClick, type: types.event, text: "Download Relatório", visible: ko.observable(false) });
    this.DownloadRemessa = PropertyEntity({ eventClick: DownloadRemessaClick, type: types.event, text: "Download Remessa", visible: ko.observable(false) });
    this.SolicitarNovaGeracao = PropertyEntity({ eventClick: SolicitarNovaGeracaoClick, type: types.event, text: "Solicitar nova geração da remessa", visible: ko.observable(false) });
    this.ReprocessarRegra = PropertyEntity({ eventClick: ReprocessarRegraClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadRemessaPagamento() {
    _remessaPagamento = new RemessaPagamento();
    KoBindings(_remessaPagamento, "knockoutCadastroRemessaPagamento");

    HeaderAuditoria("RemessaPagamento", _remessaPagamento);

    _pesquisaRemessaPagamento = new PesquisaRemessaPagamento();
    KoBindings(_pesquisaRemessaPagamento, "knockoutPesquisaRemessaPagamento", false, _pesquisaRemessaPagamento.Pesquisar.id);

    new BuscarBoletoConfiguracao(_pesquisaRemessaPagamento.BoletoConfiguracao, RetornoBoletoConfiguracao, true);
    new BuscarPagamentoEletronico(_pesquisaRemessaPagamento.PagamentoEletronico);
    new BuscarEmpresa(_pesquisaRemessaPagamento.Empresa);
    new BuscarTitulo(_pesquisaRemessaPagamento.Titulo, null, null, RetornoTitulo);
    new BuscarClientes(_pesquisaRemessaPagamento.Pessoa);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaRemessaPagamento.Empresa.visible(false);
        _remessaPagamento.Empresa.visible(false);
        _remessaPagamento.Empresa.required(false);
    }

    buscarRemessaPagamentos();
    HabilitarCampos(_remessaPagamento);
}

function RetornoBoletoConfiguracao(data) {
    _pesquisaRemessaPagamento.BoletoConfiguracao.val(data.Descricao);
    _pesquisaRemessaPagamento.BoletoConfiguracao.codEntity(data.Codigo);
}

function RetornoTitulo(data) {
    _pesquisaRemessaPagamento.Titulo.val(data.Codigo);
    _pesquisaRemessaPagamento.Titulo.codEntity(data.Codigo);
}

function DownloadRelatorioClick(e, sender) {
    if (_remessaPagamento.SituacaoAutorizacaoPagamentoEletronico.val() == null || _remessaPagamento.SituacaoAutorizacaoPagamentoEletronico.val() == EnumSituacaoAutorizacaoPagamentoEletronico.Finalizada || _remessaPagamento.SituacaoAutorizacaoPagamentoEletronico.val() == EnumSituacaoAutorizacaoPagamentoEletronico.Iniciada) {
        var dados = { Codigo: _remessaPagamento.Codigo.val() };
        executarDownload("PagamentoDigital/DownloadRelatorioRemessa", dados);
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Aprovação", "Remessa aguardando a aprovação de sua alçada para realizar o download.");
}

function DownloadRemessaClick(e, sender) {
    if (_remessaPagamento.SituacaoAutorizacaoPagamentoEletronico.val() == null || _remessaPagamento.SituacaoAutorizacaoPagamentoEletronico.val() == EnumSituacaoAutorizacaoPagamentoEletronico.Finalizada || _remessaPagamento.SituacaoAutorizacaoPagamentoEletronico.val() == EnumSituacaoAutorizacaoPagamentoEletronico.Iniciada) {
        var dados = { Codigo: _remessaPagamento.Codigo.val() };
        executarDownload("PagamentoDigital/DownloadArquivoRemessa", dados);
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Aprovação", "Remessa aguardando a aprovação de sua alçada para realizar o download.");

}

function ReprocessarRegraClick(e, sender) {
    executarReST("PagamentoDigital/ReprocessarRegras", { Codigo: _remessaPagamento.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento Eletrônico está aguardando aprovação.");
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function SolicitarNovaGeracaoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja regerar e reenviar o arquivo de remesssa?", function () {
        ExcluirPorCodigo(_remessaPagamento, "RemessaPagamento/ReenviarPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Sucesso");
                _gridRemessaPagamento.CarregarGrid();
                limparCamposRemessaPagamento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir e limpara todos os títulos que possuem este remessa de pagamento?", function () {
        ExcluirPorCodigo(_remessaPagamento, "RemessaPagamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído / Limpado com sucesso");
                _gridRemessaPagamento.CarregarGrid();
                limparCamposRemessaPagamento();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRemessaPagamento();
}

//*******MÉTODOS*******

function buscarRemessaPagamentos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRemessaPagamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRemessaPagamento = new GridView(_pesquisaRemessaPagamento.Pesquisar.idGrid, "PagamentoDigital/Pesquisa", _pesquisaRemessaPagamento, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridRemessaPagamento.CarregarGrid();
}

function editarRemessaPagamento(RemessaPagamentoGrid) {
    limparCamposRemessaPagamento();
    _remessaPagamento.Codigo.val(RemessaPagamentoGrid.Codigo);
    BuscarPorCodigo(_remessaPagamento, "RemessaPagamento/BuscarPorCodigo", function (arg) {
        _pesquisaRemessaPagamento.ExibirFiltros.visibleFade(false);
        _remessaPagamento.Cancelar.visible(true);
        _remessaPagamento.Excluir.visible(true);
        _remessaPagamento.DownloadRelatorio.visible(true);
        _remessaPagamento.DownloadRemessa.visible(true);
        _remessaPagamento.SolicitarNovaGeracao.visible(false);
        _remessaPagamento.ReprocessarRegra.visible(true);
        HabilitarCampos(_remessaPagamento);
    }, null);
}

function limparCamposRemessaPagamento() {
    _remessaPagamento.Cancelar.visible(false);
    _remessaPagamento.Excluir.visible(false);
    _remessaPagamento.DownloadRelatorio.visible(false);
    _remessaPagamento.DownloadRemessa.visible(false);
    _remessaPagamento.SolicitarNovaGeracao.visible(false);
    _remessaPagamento.ReprocessarRegra.visible(false);
    LimparCampos(_remessaPagamento);
    HabilitarCampos(_remessaPagamento);
}

function HabilitarCampos(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });
}
