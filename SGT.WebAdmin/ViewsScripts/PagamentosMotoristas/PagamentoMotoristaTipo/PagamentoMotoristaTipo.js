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
/// <reference path="../../Enumeradores/EnumTipoInegracaoPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumFormaPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoEntidade.js" />
/// <reference path="../../consultas/tipodespesafinanceira.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoMotoristaTipo;
var _pagamentoMotoristaTipo;
var _pesquisaPagamentoMotoristaTipo;
var _CRUDPagamentoMotoristaTipo;
var _pesquisaTipoDespesaFinanceira;


var _situacaoPagamentoAdiantamentoMotorista = [
    { text: "Usar Padrão", value: EnumTipoMovimentoEntidade.Nenhum },
    { text: "Entrada", value: EnumTipoMovimentoEntidade.Entrada },
    { text: "Saída", value: EnumTipoMovimentoEntidade.Saida }
];

var PesquisaPagamentoMotoristaTipo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: "*Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPagamentoMotoristaTipo.CarregarGrid();
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

var PagamentoMotoristaTipo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 150 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração (Tipo Efetivação): ", required: false, maxlength: 150 });
    this.CodigoIntegracaoTipo = PropertyEntity({ text: "Código Integração (Tipo Parcela): ", required: false, maxlength: 150 });
    this.CodigoIntegracaoEfetivacaoAdiantamento = PropertyEntity({ text: "Código Integração (Tipo Efetivação Adiantamento): ", required: false, maxlength: 150 });
    this.CodigoIntegracaoTipoParcelaAdiantamento = PropertyEntity({ text: "Código Integração (Tipo Parcela Adiantamento): ", required: false, maxlength: 150 });
    this.CodigoIntegracaoImportacao = PropertyEntity({ text: "Código Integração (Importação): ", required: false, maxlength: 150 });

    this.TipoIntegracaoPagamentoMotorista = PropertyEntity({ val: ko.observable(EnumTipoIntegracaoPagamentoMotorista.SemIntegracao), options: EnumTipoIntegracaoPagamentoMotorista.obterOpcoes(), def: EnumTipoIntegracaoPagamentoMotorista.SemIntegracao, text: "*Tipo de Integração: " });
    this.TipoPagamentoMotorista = PropertyEntity({ val: ko.observable(EnumTipoPagamentoMotorista.Nenhum), options: EnumTipoPagamentoMotorista.obterOpcoes(), def: EnumTipoPagamentoMotorista.Nenhum, text: "*Tipo de Pagamento: " });
    this.FormaPagamentoMotorista = PropertyEntity({ val: ko.observable(EnumFormaPagamentoMotorista.Nenhum), options: EnumFormaPagamentoMotorista.obterOpcoes(), def: EnumFormaPagamentoMotorista.Nenhum, text: "Forma de Pagamento: ", visible: ko.observable(false) });

    // E-mail
    this.AssuntoEmail = PropertyEntity({ val: ko.observable(""), def: "", text: "*Assunto do E-mail:", maxlength: 1000, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.CorpoEmail = PropertyEntity({ val: ko.observable(""), def: "", text: "*Corpo do E-mail:", maxlength: 999999, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    // Tags pro e-mail
    this.CorpoTagNumero = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.CorpoEmail.id, "#Numero#"); }, type: types.event, text: "Número do pagamento", visible: ko.observable(true) });
    this.CorpoTagFornecedor = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.CorpoEmail.id, "#Fornecedor#"); }, type: types.event, text: "Fornecedor", visible: ko.observable(true) });
    this.CorpoTagValor = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.CorpoEmail.id, "#Valor#"); }, type: types.event, text: "Valor", visible: ko.observable(true) });
    this.CorpoTagMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.CorpoEmail.id, "#Motorista#"); }, type: types.event, text: "Motorista", visible: ko.observable(true) });
    this.CorpoTagUsuario = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.CorpoEmail.id, "#Usuario#"); }, type: types.event, text: "Usuário", visible: ko.observable(true) });
    this.CorpoTagQuebraLinha = PropertyEntity({ eventClick: function () { InserirTag(_pagamentoMotoristaTipo.CorpoEmail.id, "#QuebraLinha#"); }, type: types.event, text: "Quebra de Linha", enable: ko.observable(true) });

    this.AssuntoTagNumero = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.AssuntoEmail.id, "#Numero#"); }, type: types.event, text: "Número do pagamento", visible: ko.observable(true) });
    this.AssuntoTagFornecedor = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.AssuntoEmail.id, "#Fornecedor#"); }, type: types.event, text: "Fornecedor", visible: ko.observable(true) });
    this.AssuntoTagValor = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.AssuntoEmail.id, "#Valor#"); }, type: types.event, text: "Valor", visible: ko.observable(true) });
    this.AssuntoTagMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.AssuntoEmail.id, "#Motorista#"); }, type: types.event, text: "Motorista", visible: ko.observable(true) });
    this.AssuntoTagUsuario = PropertyEntity({ eventClick: function (e) { InserirTag(_pagamentoMotoristaTipo.AssuntoEmail.id, "#Usuario#"); }, type: types.event, text: "Usuário", visible: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.NaoPermitirCancelamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não permitir realizar o cancelamento deste tipo de pagamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.GerarMovimentoAutomatico = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento automatizado para este tipo de pagamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoLancamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.PermitirMultiplosPagamentosAbertosParaMesmoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Permitir múltiplos pagamentos abertos para o mesmo motorista ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoReversaoLancamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarMovimentoAutomatico.val.subscribe(function (novoValor) { GerarMovimentoAutomaticoChange(novoValor); });

    this.GerarTituloPagar = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar título a pagar para este tipo de pagamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoTituloPagar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento a Pagar:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Fornecedor:"), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.PessoaSeraInformadaGeracaoPagamento = PropertyEntity({ text: "Fornecedor será informado na geração do pagamento? ", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.GerarTituloPagar.val.subscribe(function (novoValor) { GerarTituloPagarChange(novoValor); });
    this.PessoaSeraInformadaGeracaoPagamento.val.subscribe(function (novoValor) { PessoaSeraInformadaGeracaoPagamentoChange(novoValor); });

    this.GerarTarifaAutomatica = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar tarifa automaticamente para este tipo de pagamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.PercentualTarifa = PropertyEntity({ text: "*Percentual Tarifa:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoTarifa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Uso:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.TipoMovimentoReversaoTarifa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Reversão:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });
    this.GerarTarifaAutomatica.val.subscribe(function (novoValor) { GerarTarifaAutomaticaChange(novoValor); });

    this.NaoAssociarTipoPagamentoNoAcertoDeViagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não associar este tipo de pagamento no acerto de viagem? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.GerarMovimentoEntradaFixaMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar movimento de entrada na ficha do motorista?", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.TipoMovimentoPagamentoMotorista = PropertyEntity({ text: "Tipo de movimento para o Pagamento ao Motorista: ", val: ko.observable(EnumTipoMovimentoEntidade.Nenhum), def: ko.observable(EnumTipoMovimentoEntidade.Nenhum), options: _situacaoPagamentoAdiantamentoMotorista, visible: ko.observable(true) });
    this.GerarTituloAPagarAoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar título a pagar ao motorista para este tipo de pagamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoMovimentoTituloMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), issue: 364, visible: ko.observable(true), required: ko.observable(false) });

    this.DesabilitarAlteracaoDosPlanosDeContas = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Desabilitar alteração dos planos de contas no lançamento? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.HabilitarAprovacaoAutomaticaCasoOperadorSejaIgualDaAlcada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Habilitar aprovação automática caso operador seja igual da alçada? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.PermitirLancarPagamentoContendoAcertoEmAndamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Permitir lançar apenas se houver acerto de viagem em andamento ao motorista? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Utilizar este pagamento para a geração de Pagamento ao Motorista pelo saldo do Acerto de Viagem? ", idFade: guid(), visibleFade: ko.observable(false) });
    this.PermitirLancarComDataRetroativa = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Permitir lançar com data retroativa", idFade: guid(), visibleFade: ko.observable(false) });

    this.RealizarRateio = PropertyEntity({ text: "Fazer Rateio de Despesas?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoDeDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Despesa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.NaoPermitirGerarPagamentoMotoristaTerceiro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não permitir gerar pagamento motorista para motoristas terceiros", idFade: guid(), visibleFade: ko.observable(false) });
    this.RealizarMovimentoFinanceiroPelaDataPagamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Realizar movimento financeiro pela data do pagamento", idFade: guid(), visibleFade: ko.observable(false) });
    this.GerarPendenciaAoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Gerar pendência ao motorista", idFade: guid(), visibleFade: ko.observable(false) });
    this.ReterImpostoPagamentoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Reter Impostos (IRRF, SEST/SENAT, etc.)", idFade: guid(), visibleFade: ko.observable(false) });


    this.RealizarRateio.val.subscribe(function (novoValor) {
        _pagamentoMotoristaTipo.TipoDeDespesa.required(novoValor);
    });

    this.TipoIntegracaoPagamentoMotorista.val.subscribe(function (novoValor) {
        _pagamentoMotoristaTipo.FormaPagamentoMotorista.visible(false);
        if (novoValor === EnumTipoIntegracaoPagamentoMotorista.PamcardCorporativo)
            _pagamentoMotoristaTipo.FormaPagamentoMotorista.visible(true);

        _pagamentoMotoristaTipo.AssuntoEmail.visible(false);
        _pagamentoMotoristaTipo.AssuntoEmail.required(false);
        _pagamentoMotoristaTipo.CorpoEmail.visible(false);
        _pagamentoMotoristaTipo.CorpoEmail.required(false);
        if (novoValor === EnumTipoIntegracaoPagamentoMotorista.Email) {
            _pagamentoMotoristaTipo.AssuntoEmail.visible(true);
            _pagamentoMotoristaTipo.CorpoEmail.visible(true);
            _pagamentoMotoristaTipo.AssuntoEmail.required(true);
            _pagamentoMotoristaTipo.CorpoEmail.required(true);
        }

    });
};

var CRUDPagamentoMotoristaTipo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadPagamentoMotoristaTipo() {

    _pesquisaPagamentoMotoristaTipo = new PesquisaPagamentoMotoristaTipo();
    KoBindings(_pesquisaPagamentoMotoristaTipo, "knockoutPesquisaPagamentoMotoristaTipo", false, _pesquisaPagamentoMotoristaTipo.Pesquisar.id);

    _pagamentoMotoristaTipo = new PagamentoMotoristaTipo();
    KoBindings(_pagamentoMotoristaTipo, "knockoutCadastroPagamentoMotoristaTipo");

    HeaderAuditoria("PagamentoMotoristaTipo", _pagamentoMotoristaTipo);

    _CRUDPagamentoMotoristaTipo = new CRUDPagamentoMotoristaTipo();
    KoBindings(_CRUDPagamentoMotoristaTipo, "knockoutCRUDPagamentoMotoristaTipo");

    new BuscarTipoMovimento(_pagamentoMotoristaTipo.TipoMovimentoLancamento);
    new BuscarTipoMovimento(_pagamentoMotoristaTipo.TipoMovimentoReversaoLancamento);

    new BuscarTipoMovimento(_pagamentoMotoristaTipo.TipoMovimentoTituloPagar);
    new BuscarClientes(_pagamentoMotoristaTipo.Pessoa);

    new BuscarTipoMovimento(_pagamentoMotoristaTipo.TipoMovimentoTarifa);
    new BuscarTipoMovimento(_pagamentoMotoristaTipo.TipoMovimentoReversaoTarifa);
    new BuscarTipoMovimento(_pagamentoMotoristaTipo.TipoMovimentoTituloMotorista);
    new BuscarTipoDespesaFinanceira(_pagamentoMotoristaTipo.TipoDeDespesa);

    buscarPagamentoMotoristaTipos();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
        $("#liTabMovimentoFinanceiro").hide();
}

function GerarMovimentoAutomaticoChange(novoValor) {
    _pagamentoMotoristaTipo.GerarMovimentoAutomatico.visibleFade(novoValor);
    _pagamentoMotoristaTipo.TipoMovimentoLancamento.required(novoValor);
    _pagamentoMotoristaTipo.TipoMovimentoReversaoLancamento.required(novoValor);
}

function GerarTituloPagarChange(novoValor) {
    _pagamentoMotoristaTipo.GerarTituloPagar.visibleFade(novoValor);
    _pagamentoMotoristaTipo.TipoMovimentoTituloPagar.required(novoValor);
    _pagamentoMotoristaTipo.Pessoa.required(novoValor);

    if (!novoValor)
        _pagamentoMotoristaTipo.PessoaSeraInformadaGeracaoPagamento.val(false);
}

function PessoaSeraInformadaGeracaoPagamentoChange(novoValor) {
    _pagamentoMotoristaTipo.Pessoa.required(!novoValor);
    _pagamentoMotoristaTipo.Pessoa.text(novoValor ? "Fornecedor:" : "*Fornecedor:");
}

function GerarTarifaAutomaticaChange(novoValor) {
    _pagamentoMotoristaTipo.GerarTarifaAutomatica.visibleFade(novoValor);
    _pagamentoMotoristaTipo.PercentualTarifa.required(novoValor);
    _pagamentoMotoristaTipo.TipoMovimentoTarifa.required(novoValor);
    _pagamentoMotoristaTipo.TipoMovimentoReversaoTarifa.required(novoValor);
}

function adicionarClick(e, sender) {
    Salvar(_pagamentoMotoristaTipo, "PagamentoMotoristaTipo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPagamentoMotoristaTipo.CarregarGrid();
                limparCamposPagamentoMotoristaTipo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_pagamentoMotoristaTipo, "PagamentoMotoristaTipo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPagamentoMotoristaTipo.CarregarGrid();
                limparCamposPagamentoMotoristaTipo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de pagamento " + _pagamentoMotoristaTipo.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_pagamentoMotoristaTipo, "PagamentoMotoristaTipo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPagamentoMotoristaTipo.CarregarGrid();
                    limparCamposPagamentoMotoristaTipo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPagamentoMotoristaTipo();
}

//*******MÉTODOS*******

function buscarPagamentoMotoristaTipos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPagamentoMotoristaTipo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPagamentoMotoristaTipo = new GridView(_pesquisaPagamentoMotoristaTipo.Pesquisar.idGrid, "PagamentoMotoristaTipo/Pesquisa", _pesquisaPagamentoMotoristaTipo, menuOpcoes, null);
    _gridPagamentoMotoristaTipo.CarregarGrid();
}

function editarPagamentoMotoristaTipo(pagamentoMotoristaTipoGrid) {
    limparCamposPagamentoMotoristaTipo();
    _pagamentoMotoristaTipo.Codigo.val(pagamentoMotoristaTipoGrid.Codigo);
    BuscarPorCodigo(_pagamentoMotoristaTipo, "PagamentoMotoristaTipo/BuscarPorCodigo", function (arg) {
        _pesquisaPagamentoMotoristaTipo.ExibirFiltros.visibleFade(false);
        _CRUDPagamentoMotoristaTipo.Atualizar.visible(true);
        _CRUDPagamentoMotoristaTipo.Cancelar.visible(true);
        _CRUDPagamentoMotoristaTipo.Excluir.visible(true);
        _CRUDPagamentoMotoristaTipo.Adicionar.visible(false);
    }, null);
}

function limparCamposPagamentoMotoristaTipo() {
    _CRUDPagamentoMotoristaTipo.Atualizar.visible(false);
    _CRUDPagamentoMotoristaTipo.Cancelar.visible(false);
    _CRUDPagamentoMotoristaTipo.Excluir.visible(false);
    _CRUDPagamentoMotoristaTipo.Adicionar.visible(true);
    LimparCampos(_pagamentoMotoristaTipo);
    _pagamentoMotoristaTipo.TipoMovimentoPagamentoMotorista.val(EnumTipoMovimentoEntidade.Nenhum);
    $(".nav-tabs a:first").trigger("click");
}
