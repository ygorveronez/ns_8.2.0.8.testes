/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCaixaFuncionario;
var _gridMovimentacoes;
var _gridMovimentacaoesCaixa;
var _caixaFuncionario;
var _statusCaixa;
var _movimentacoes;
var _pesquisaCaixaFuncionario;
var _resumoCaixa;
var _adicionarValorCaixa;
var _adicionarMovimentoFinanceiro;

var _situacaoCaixa = [
    { text: "Todas", value: 0 },
    { text: "Aberto", value: EnumSituacaoCaixa.Aberto },
    { text: "Fechado", value: EnumSituacaoCaixa.Fechado }
];

var _TipoDocumento = [
    { text: "Manual", value: EnumTipoDocumentoMovimento.Manual },
    { text: "Nota de Entrada", value: EnumTipoDocumentoMovimento.NotaEntrada },
    { text: "Documento Emitido", value: EnumTipoDocumentoMovimento.CTe },
    { text: "Faturamento", value: EnumTipoDocumentoMovimento.Faturamento },
    { text: "Recibo", value: EnumTipoDocumentoMovimento.Recibo },
    { text: "Pagamento", value: EnumTipoDocumentoMovimento.Pagamento },
    { text: "Recebimento", value: EnumTipoDocumentoMovimento.Recebimento },
    { text: "Nota de Saída", value: EnumTipoDocumentoMovimento.NotaSaida },
    { text: "Acerto de Viagem", value: EnumTipoDocumentoMovimento.Acerto },
    { text: "Contrato de Frete", value: EnumTipoDocumentoMovimento.ContratoFrete },
    { text: "Outros", value: EnumTipoDocumentoMovimento.Outros }
];

var PesquisaCaixaFuncionario = function () {
    this.SituacaoCaixa = PropertyEntity({ val: ko.observable(EnumSituacaoCaixa.Fechado), options: _situacaoCaixa, def: EnumSituacaoCaixa.Fechado, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCaixaFuncionario.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CaixaFuncionario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SituacaoCaixa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NovoLancamento = PropertyEntity({ eventClick: NovoLancamentoClick, type: types.event, text: "Novo Lançamento", visible: ko.observable(true) });
    //this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(true) });
    this.ImprimirDetalhado = PropertyEntity({ eventClick: ImprimirDetalhadoClick, type: types.event, text: ko.observable("Detalhado"), visible: ko.observable(false), icon: "fa fa-fw fa-file-pdf-o" });
    this.ImprimirResumido = PropertyEntity({ eventClick: ImprimirResumidoClick, type: types.event, text: ko.observable("Resumido"), visible: ko.observable(false), icon: "fa fa-fw fa-file-pdf-o" });

    this.Limpar = PropertyEntity({ eventClick: LimparClick, type: types.event, text: "Limpar / Atualizar", visible: ko.observable(true) });
}

var StatusCaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Operador = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PlanoConta = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Status = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DataAbertura = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoInicial = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true), visible: true, val: ko.observable("") });

    this.AbrirCaixa = PropertyEntity({ eventClick: AbrirCaixaClick, type: types.event, text: "Abrir Caixa", visible: ko.observable(true), enable: ko.observable(true) });
}

var Movimentacoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Movimentacoes = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.Entradas = PropertyEntity({ text: "Entradas: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.Saidas = PropertyEntity({ text: "Saídas: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.Saldo = PropertyEntity({ text: "Saldo: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.AtualizarMovimento = PropertyEntity({ eventClick: AtualizarMovimentosClick, type: types.event, text: "Atualizar", visible: ko.observable(true), enable: ko.observable(true) });

    this.MovimentacoesCaixa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.SaldoCaixa = PropertyEntity({ text: "Saldo: ", getType: typesKnockout.decimal, val: ko.observable("0,00"), visible: true });
    this.AtualizarMovimentoCaixa = PropertyEntity({ eventClick: AtualizarMovimentoCaixaClick, type: types.event, text: "Atualizar", visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarMovimentoCaixa = PropertyEntity({ eventClick: AdicionarMovimentoCaixaClick, type: types.event, text: "Adicionar Valor no Caixa", visible: ko.observable(true), enable: ko.observable(true) });
}

var ResumoCaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.SaldoInicial = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Entradas = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Saidas = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoFinal = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValoresNoCaixa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.FecharCaixa = PropertyEntity({ eventClick: FecharCaixaClick, type: types.event, text: "Fechar Caixa", visible: ko.observable(true), enable: ko.observable(true) });
}

var AdicionarValorCaixa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoValorCaixa = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Data = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date, val: ko.observable("") });
    this.Valor = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal, val: ko.observable("") });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 500, val: ko.observable("") });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarValorCaixaClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
}

var AdicionarMovimentoFinanceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataMovimento = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date });
    this.DataBase = PropertyEntity({ text: "*Data Base: ", required: true, getType: typesKnockout.date });
    this.ValorMovimento = PropertyEntity({ text: "*Valor: ", required: true, getType: typesKnockout.decimal });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoMovimento.Manual), options: _TipoDocumento, text: "*Tipo do Documento: ", def: EnumTipoDocumentoMovimento.Manual, required: true });
    this.NumeroDocumento = PropertyEntity({ text: "*Nº Documento: ", required: true });

    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Movimento:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), required: false, val: ko.observable(""), enable: ko.observable(true) });
    this.PlanoDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Entrada:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.PlanoCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Plano de Saída:", idBtnSearch: guid(), required: true, val: ko.observable(""), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarMovimentoFinanceiroClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadCaixaFuncionario() {

    _pesquisaCaixaFuncionario = new PesquisaCaixaFuncionario();
    KoBindings(_pesquisaCaixaFuncionario, "knockoutPesquisaCaixaFuncionario", false, _pesquisaCaixaFuncionario.Pesquisar.id);

    _caixaFuncionario = new CaixaFuncionario();
    KoBindings(_caixaFuncionario, "knockoutCadastroCaixaFuncionario");

    _movimentacoes = new Movimentacoes();
    KoBindings(_movimentacoes, "knockoutMovimentacoes");

    _statusCaixa = new StatusCaixa();
    KoBindings(_statusCaixa, "knockoutStatusCaixa");

    _resumoCaixa = new ResumoCaixa();
    KoBindings(_resumoCaixa, "knockoutResumoCaixa");

    _adicionarValorCaixa = new AdicionarValorCaixa();
    KoBindings(_adicionarValorCaixa, "knoutAdicionarValorCaixa");

    _adicionarMovimentoFinanceiro = new AdicionarMovimentoFinanceiro();
    KoBindings(_adicionarMovimentoFinanceiro, "knoutAdicionarMovimentoFinanceiro");

    new BuscarCentroResultado(_adicionarMovimentoFinanceiro.CentroResultado, "Selecione o Centro de Resultado", "Centros de Resultado", null, EnumAnaliticoSintetico.Analitico, _adicionarMovimentoFinanceiro.TipoMovimento);
    new BuscarPlanoConta(_adicionarMovimentoFinanceiro.PlanoDebito, "Selecione a Conta Analítica (Entrada)", "Contas Analíticas (Entrada)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_adicionarMovimentoFinanceiro.PlanoCredito, "Selecione a Conta Analítica (Saída)", "Contas Analíticas (Saída)", null, EnumAnaliticoSintetico.Analitico);
    new BuscarTipoMovimento(_adicionarMovimentoFinanceiro.TipoMovimento, null, null, RetornoTipoMovimento, null, EnumFinalidadeTipoMovimento.MovimentoFinanceiro);

    //HeaderAuditoria("CaixaFuncionario", _caixaFuncionario);

    _gridMovimentacoes = new GridView(_movimentacoes.Movimentacoes.idGrid, "CaixaFuncionario/PesquisaMovimentacoes", _movimentacoes, null, null);

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarMovimentacaoCaixa, icone: "" };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirMovimentacaoCaixa, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editar, excluir] };
    _gridMovimentacaoesCaixa = new GridView(_movimentacoes.MovimentacoesCaixa.idGrid, "CaixaFuncionario/PesquisaMovimentacoesCaixa", _movimentacoes, menuOpcoes, null);

    buscarCaixaFuncionario();
    CarregarCaixaFuncionario();
}

function RetornoTipoMovimento(data) {
    if (data !== null) {
        _adicionarMovimentoFinanceiro.TipoMovimento.codEntity(data.Codigo);
        _adicionarMovimentoFinanceiro.TipoMovimento.val(data.Descricao);

        if (data.CodigoDebito > 0) {
            _adicionarMovimentoFinanceiro.PlanoDebito.codEntity(data.CodigoDebito);
            _adicionarMovimentoFinanceiro.PlanoDebito.val(data.PlanoDebito);
        }
        if (data.CodigoCredito > 0) {
            _adicionarMovimentoFinanceiro.PlanoCredito.codEntity(data.CodigoCredito);
            _adicionarMovimentoFinanceiro.PlanoCredito.val(data.PlanoCredito);
        }
        if (data.CodigoResultado > 0) {
            _adicionarMovimentoFinanceiro.CentroResultado.codEntity(data.CodigoResultado);
            _adicionarMovimentoFinanceiro.CentroResultado.val(data.CentroResultado);
        } else {
            LimparCampoEntity(_adicionarMovimentoFinanceiro.CentroResultado);
        }
    }
}

function AdicionarValorCaixaClick(e, sender) {
    Salvar(_adicionarValorCaixa, "CaixaFuncionario/AdicionarValorCaixa", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Valor em caixa lançado com sucesso.");

                Global.fecharModal('divAdicionarValorCaixa');
                CarregarCaixaFuncionario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function AdicionarMovimentoFinanceiroClick(e, sender) {
    Salvar(_adicionarMovimentoFinanceiro, "CaixaFuncionario/AdicionarMovimentoFinanceiro", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Movimento financeiro lançado com sucesso.");

                Global.fecharModal('divAdicionarMovimentoFinanceiro');
                CarregarCaixaFuncionario();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function AtualizarMovimentosClick(e, sender) {
    _gridMovimentacoes.CarregarGrid();
}

function AtualizarMovimentoCaixaClick(e, sender) {
    _gridMovimentacaoesCaixa.CarregarGrid();
}

function editarMovimentacaoCaixa(data) {
    if (_caixaFuncionario.SituacaoCaixa.val() === EnumSituacaoCaixa.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível editar um lançamento em um caixa já fechado.");
        return;
    }
    LimparCampos(_adicionarValorCaixa);
    _adicionarValorCaixa.Adicionar.text("Atualizar");
    _adicionarValorCaixa.Codigo.val(_movimentacoes.Codigo.val());
    _adicionarValorCaixa.CodigoValorCaixa.val(data.Codigo);
    _adicionarValorCaixa.Data.val(data.Data);
    _adicionarValorCaixa.Valor.val(data.Valor);
    _adicionarValorCaixa.Descricao.val(data.Descricao);    
    Global.abrirModal('divAdicionarValorCaixa');
}

function excluirMovimentacaoCaixa(e) {
    if (_caixaFuncionario.SituacaoCaixa.val() === EnumSituacaoCaixa.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível excluir um lançamento em um caixa já fechado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja remover o lançamento no caixa selecionado?", function () {
        var data = {
            Codigo: e.CodigoValorCaixa
        };
        executarReST("CaixaFuncionario/RemoverValorCaixa", data, function (arg) {
            if (!arg.Success)
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            else {
                CarregarCaixaFuncionario();
            }
        });
    });
}

function NovoLancamentoClick(e, sender) {
    if (_movimentacoes.Codigo.val() === 0 || _movimentacoes.Codigo.val() === "" || _movimentacoes.Codigo.val() === null || _movimentacoes.Codigo.val() === undefined) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor abra o caixa antes de realizar o lançamento.");
    } else {
        LimparCampos(_adicionarMovimentoFinanceiro);
        _adicionarMovimentoFinanceiro.Codigo.val(_movimentacoes.Codigo.val());        
        Global.abrirModal('divAdicionarMovimentoFinanceiro');
    }
}

function ImprimirDetalhadoClick(e, sender) {
    var data = { Codigo: _caixaFuncionario.Codigo.val(), Detalhado: true };
    executarDownload("CaixaFuncionario/BaixarRelatorio", data);
}

function ImprimirResumidoClick(e, sender) {
    var data = { Codigo: _caixaFuncionario.Codigo.val(), Detalhado: false };
    executarDownload("CaixaFuncionario/BaixarRelatorio", data);
}

function LimparClick(e, sender) {
    CarregarCaixaFuncionario();
}

function AbrirCaixaClick(e, sender) {
    var data = { Observacao: _statusCaixa.Observacao.val() };
    executarReST("CaixaFuncionario/AbrirCaixaFuncionario", data, function (arg) {
        if (arg.Success) {
            _resumoCaixa.FecharCaixa.visible(true);
            _statusCaixa.AbrirCaixa.visible(false);
            _statusCaixa.Observacao.enable(false);
            _movimentacoes.AdicionarMovimentoCaixa.visible(true);
            _caixaFuncionario.NovoLancamento.visible(true);
            
            _caixaFuncionario.ImprimirDetalhado.visible(false);
            _caixaFuncionario.ImprimirResumido.visible(false);

            PreencherObjetoKnout(_resumoCaixa, arg);
            PreencherObjetoKnout(_movimentacoes, arg);
            PreencherObjetoKnout(_statusCaixa, arg);

            _gridMovimentacoes.CarregarGrid();
            _gridMovimentacaoesCaixa.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function FecharCaixaClick(e, sender) {    
    executarReST("CaixaFuncionario/FecharCaixaFuncionario", null, function (arg) {
        if (arg.Success) {
            _caixaFuncionario.Codigo.val(arg.Data.Codigo);
            _statusCaixa.Observacao.val("");
            _resumoCaixa.FecharCaixa.visible(false);
            _statusCaixa.AbrirCaixa.visible(true);
            _statusCaixa.Observacao.enable(true);
            _movimentacoes.AdicionarMovimentoCaixa.visible(false);
            _caixaFuncionario.NovoLancamento.visible(true);            
            _caixaFuncionario.ImprimirDetalhado.visible(true);
            _caixaFuncionario.ImprimirResumido.visible(true);

        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    });
}

function AdicionarMovimentoCaixaClick(e, sender) {
    if (_movimentacoes.Codigo.val() === 0 || _movimentacoes.Codigo.val() === "" || _movimentacoes.Codigo.val() === null || _movimentacoes.Codigo.val() === undefined) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor abra o caixa antes de realizar o lançamento.");
    } else {
        LimparCampos(_adicionarValorCaixa);
        _adicionarValorCaixa.Adicionar.text("Adicionar");
        _adicionarValorCaixa.Codigo.val(_movimentacoes.Codigo.val());        
        Global.abrirModal('divAdicionarValorCaixa');
    }
}

//*******MÉTODOS*******


function editarCaixaFuncionario(caixaFuncionarioGrid) {
    limparCamposCaixaFuncionario();
    _caixaFuncionario.Codigo.val(caixaFuncionarioGrid.Codigo);
    BuscarPorCodigo(_caixaFuncionario, "CaixaFuncionario/BuscarPorCodigo", function (arg) {

        _pesquisaCaixaFuncionario.ExibirFiltros.visibleFade(false);

        var data = arg.Data;
        _resumoCaixa.FecharCaixa.visible(false);
        _statusCaixa.AbrirCaixa.visible(false);
        _statusCaixa.Observacao.enable(false);
        _movimentacoes.AdicionarMovimentoCaixa.visible(false);
        _caixaFuncionario.Limpar.visible(true);
        _caixaFuncionario.NovoLancamento.visible(true);        
        _caixaFuncionario.ImprimirDetalhado.visible(true);
        _caixaFuncionario.ImprimirResumido.visible(true);

        PreencherObjetoKnout(_resumoCaixa, arg);
        PreencherObjetoKnout(_movimentacoes, arg);
        PreencherObjetoKnout(_statusCaixa, arg);

        _gridMovimentacoes.CarregarGrid();
        _gridMovimentacaoesCaixa.CarregarGrid();
    }, null);
}


function buscarCaixaFuncionario() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCaixaFuncionario, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCaixaFuncionario = new GridView(_pesquisaCaixaFuncionario.Pesquisar.idGrid, "CaixaFuncionario/Pesquisa", _pesquisaCaixaFuncionario, menuOpcoes, null);
    _gridCaixaFuncionario.CarregarGrid();
}

function CarregarCaixaFuncionario() {
    executarReST("CaixaFuncionario/BuscarCaixaFuncionario", null, function (arg) {
        if (arg.Success) {
            var data = arg.Data;
            if (data.SituacaoCaixa === EnumSituacaoCaixa.Fechado) {
                _resumoCaixa.FecharCaixa.visible(false);
                _statusCaixa.AbrirCaixa.visible(true);
                _statusCaixa.Observacao.enable(true);
                _movimentacoes.AdicionarMovimentoCaixa.visible(true);
                _caixaFuncionario.NovoLancamento.visible(true);                
                _caixaFuncionario.ImprimirDetalhado.visible(false);
                _caixaFuncionario.ImprimirResumido.visible(false);
            } else {
                _resumoCaixa.FecharCaixa.visible(true);
                _statusCaixa.AbrirCaixa.visible(false);
                _statusCaixa.Observacao.enable(false);
                _movimentacoes.AdicionarMovimentoCaixa.visible(true);
                _caixaFuncionario.NovoLancamento.visible(true);
                _caixaFuncionario.ImprimirDetalhado.visible(false);
                _caixaFuncionario.ImprimirResumido.visible(false);
            }

            PreencherObjetoKnout(_resumoCaixa, arg);
            PreencherObjetoKnout(_movimentacoes, arg);
            PreencherObjetoKnout(_statusCaixa, arg);

            _gridMovimentacoes.CarregarGrid();
            _gridMovimentacaoesCaixa.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposCaixaFuncionario() {

    LimparCampos(_caixaFuncionario);
    LimparCampos(_movimentacoes);
    LimparCampos(_statusCaixa);
    LimparCampos(_resumoCaixa);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}
