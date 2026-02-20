/// <reference path="../../Consultas/Conhecimento.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Fatura.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="IntegracaoBaixaTituloReceber.js" />
/// <reference path="NegociacaoBaixaTituloReceber.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="CabecalhoBaixaTituloReceber.js" />
/// <reference path="EtapaBaixaTituloReceber.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _situacaoTituloPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Iniciada", value: 1 },
    { text: "Negociação", value: 2 },
    { text: "Finalizada", value: 3 },
    { text: "Cancelada", value: 4 }
];

var _etapaBaixa = [
    { text: "Iniciada", value: 1 },
    { text: "Negociação", value: 2 },
    { text: "Finalizada", value: 3 },
    { text: "Cancelada", value: 4 }
];

var _titulosDeAgrupamento = [
    { text: "Todos", value: EnumSimNaoPesquisa.Todos },
    { text: "Gerado de negociação", value: EnumSimNaoPesquisa.Sim },
    { text: "Não gerado de negociação", value: EnumSimNaoPesquisa.Nao }
];

var _gridTitulosReceber;
var _baixaTituloReceber;
var _pesquisaTituloReceber;
var _PermissoesPersonalizadas;
var _gridTitulosPendentes;

var PesquisaTituloReceber = function () {
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura: ", getType: typesKnockout.int, maxlength: 16, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", eventChange: TipoPessoaPesquisaChange });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoTituloPesquisa, def: 0, text: "Situação: " });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataBaseInicial = PropertyEntity({ text: "Data Base Inicial: ", getType: typesKnockout.date });
    this.DataBaseFinal = PropertyEntity({ text: "Data Base Final: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTitulosReceber.CarregarGrid();
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

var BaixaTituloReceber = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(1), options: _etapaBaixa, def: 1 });
    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(1), options: _situacaoTituloPesquisa, def: 1, text: "Situação: " });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", eventChange: TipoPessoaTitulosPendentesChange, enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TitulosDeAgrupamento = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: _titulosDeAgrupamento, def: EnumSimNaoPesquisa.Todos, text: "Agrupado: ", enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Todos), options: EnumFormaTitulo.obterOpcoesPesquisa(), text: "Forma do Título: ", def: EnumFormaTitulo.Todos, enable: ko.observable(true) });

    this.DataProgramacaoPagamentoInicial = PropertyEntity({ text: "Progr. Inicial: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataProgramacaoPagamentoFinal = PropertyEntity({ text: "Progr. Final: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", getType: typesKnockout.string, visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.PesquisarTitulosPendentes = PropertyEntity({ eventClick: PesquisarTitulosPendentesClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });

    this.TitulosPendentes = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura: ", getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.ParcelaTitulo = PropertyEntity({ text: "Parcela: ", getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.ValorPendenteTitulo = PropertyEntity({ text: "Valor: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.DataBaixa = PropertyEntity({ text: "*Data baixa: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.DataBase = PropertyEntity({ text: "*Data base: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.ValorBaixado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "*Valor:", maxlength: 15, val: ko.observable("0,00"), def: ko.observable("0,00"), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    /*this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeira();
    });

    this.DataBaseCRT.val.subscribe(function (novoValor) {
        CalcularMoedaEstrangeira();
    });

    this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
        ConverterValor();
    });

    this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
        ConverterValor();
    });*/

    this.ListaParcelasNegociacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CTesRemovidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.SalvarObservacao = PropertyEntity({ eventClick: SalvarObservacaoClick, type: types.event, text: "Salvar Observação", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarBaixa = PropertyEntity({ eventClick: CancelarBaixaClick, type: types.event, text: "Cancelar Baixa", visible: ko.observable(false), enable: ko.observable(true) });
    this.BaixarTitulo = PropertyEntity({ eventClick: BaixarTituloClick, type: types.event, text: "Baixar Título", visible: ko.observable(true), enable: ko.observable(true) });

    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: "Importar Planilha",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "BaixaTituloReceber/Importar",
        UrlConfiguracao: "BaixaTituloReceber/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O011_BaixaTituloAPagar,
        CallbackSomenteSeSucesso: false,
        FecharModalSeSucesso: false,
        CallbackImportacao: function (r) {
            _baixaTituloReceber.ValorPendenteTitulo.val(r.Data.Retorno.ValorPendente);
            _gridTitulosPendentes.AtualizarRegistrosSelecionados(r.Data.Retorno.Titulos);
            _gridTitulosPendentes.CarregarGrid();
        },
        ParametrosRequisicao: function () {
            return {};
        }
    });
};

//*******EVENTOS*******

function loadBaixaTitulosReceber() {
    $("#knockoutCabecalhoTituloReceber").hide();

    _pesquisaTituloReceber = new PesquisaTituloReceber();
    KoBindings(_pesquisaTituloReceber, "knockoutPesquisaTituloReceber");

    new BuscarClientes(_pesquisaTituloReceber.Pessoa);
    new BuscarGruposPessoas(_pesquisaTituloReceber.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarFuncionario(_pesquisaTituloReceber.Operador);
    new BuscarConhecimentoNotaReferencia(_pesquisaTituloReceber.Conhecimento, RetornoBuscarPesquisaConhecimento);

    _baixaTituloReceber = new BaixaTituloReceber();
    KoBindings(_baixaTituloReceber, "knockoutCadastroTituloReceber");

    HeaderAuditoria("TituloBaixa", _baixaTituloReceber);

    new BuscarGruposPessoas(_baixaTituloReceber.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_baixaTituloReceber.Pessoa);
    new BuscarFatura(_baixaTituloReceber.Fatura, RetornoBuscarFatura);
    new BuscarConhecimentoNotaReferencia(_baixaTituloReceber.Conhecimento, RetornoBuscarConhecimento);
    new BuscarCargas(_baixaTituloReceber.Carga);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaTituloReceber.NumeroFatura.visible(false);
        _pesquisaTituloReceber.Conhecimento.visible(true);
        _baixaTituloReceber.Fatura.visible(false);
        _baixaTituloReceber.Conhecimento.visible(true);
        _baixaTituloReceber.NumeroOcorrencia.visible(false);
        _baixaTituloReceber.Carga.visible(false);
        _baixaTituloReceber.NumeroPedido.visible(false);
    } else {
        $("#lcTituloVencido").hide();
        $("#lcTituloVencendoHoje").hide();
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _baixaTituloReceber.MoedaCotacaoBancoCentral.visible(true);
        _baixaTituloReceber.DataBaseCRT.visible(true);
        _baixaTituloReceber.ValorMoedaCotacao.visible(true);
        _baixaTituloReceber.ValorOriginalMoedaEstrangeira.visible(true);
    }

    buscarTitulosReceber();
    buscarTitulosPendentes();

    loadCabecalhoBaixaTituloReceber();
    loadEtapaBaixaTituloReceber();
    loadNegociacaoBaixa();
    loadIntegracaoBaixa();
    carregarLancamentoCheque("conteudoCheque");
}

function RetornoBuscarFatura(data) {
    _baixaTituloReceber.Fatura.codEntity(data.Codigo);
    _baixaTituloReceber.Fatura.val(data.Numero);
}

function RetornoBuscarPesquisaConhecimento(data) {
    _pesquisaTituloReceber.Conhecimento.codEntity(data.Codigo);
    _pesquisaTituloReceber.Conhecimento.val(data.Numero + "-" + data.Serie);
}

function RetornoBuscarConhecimento(data) {
    _baixaTituloReceber.Conhecimento.codEntity(data.Codigo);
    _baixaTituloReceber.Conhecimento.val(data.Numero + "-" + data.Serie);
}

function TipoPessoaPesquisaChange(e, sender) {
    if (_pesquisaTituloReceber.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        _pesquisaTituloReceber.Pessoa.visible(true);
        _pesquisaTituloReceber.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaTituloReceber.GrupoPessoa);
    } else if (_pesquisaTituloReceber.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _pesquisaTituloReceber.Pessoa.visible(false);
        _pesquisaTituloReceber.GrupoPessoa.visible(true);
        LimparCampoEntity(_pesquisaTituloReceber.Pessoa);
    }
}

function TipoPessoaTitulosPendentesChange(e, sender) {
    if (_baixaTituloReceber.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        _baixaTituloReceber.Pessoa.visible(true);
        _baixaTituloReceber.GrupoPessoa.visible(false);
        LimparCampoEntity(_baixaTituloReceber.GrupoPessoa);
    } else if (_baixaTituloReceber.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _baixaTituloReceber.Pessoa.visible(false);
        _baixaTituloReceber.GrupoPessoa.visible(true);
        LimparCampoEntity(_baixaTituloReceber.Pessoa);
    }
}

function PesquisarTitulosPendentesClick(e, sender) {
    buscarTitulosPendentes();
}

//*******MÉTODOS*******

function editarTituloReceber(tituloReceberGrid) {
    limparCamposBaixaTituloReceber();
    _baixaTituloReceber.Codigo.val(tituloReceberGrid.Codigo);
    BuscarPorCodigo(_baixaTituloReceber, "BaixaTituloReceber/BuscarPorCodigo", function (arg) {
        _pesquisaTituloReceber.ExibirFiltros.visibleFade(false);

        CarregarDadosCabecalho(arg.Data);
        PosicionarEtapa(arg.Data);
        buscarTitulosPendentes();

        $("#knockoutCabecalhoTituloReceber").show();
    }, null);
}

function buscarTitulosReceber() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTituloReceber, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTitulosReceber = new GridView(_pesquisaTituloReceber.Pesquisar.idGrid, "BaixaTituloReceber/Pesquisa", _pesquisaTituloReceber, menuOpcoes, null, null, null);
    _gridTitulosReceber.CarregarGrid();
}

function buscarTitulosPendentes() {
    var somenteLeitura = false;

    _baixaTituloReceber.SelecionarTodos.visible(true);
    _baixaTituloReceber.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarValorPendente();
        },
        callbackNaoSelecionado: function () {
            AtualizarValorPendente();
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _baixaTituloReceber.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridTitulosPendentes = new GridView(_baixaTituloReceber.TitulosPendentes.idGrid, "BaixaTituloReceber/PesquisaTitulosPendentes", _baixaTituloReceber, null, { column: 11, dir: 'asc' }, 20, null, null, null, multiplaescolha);
    _gridTitulosPendentes.CarregarGrid();
}

function limparCamposBaixaTituloReceber() {
    LimparCampos(_baixaTituloReceber);
    _baixaTituloReceber.ValorBaixado.val("0,00");
    _baixaTituloReceber.CancelarBaixa.visible(false);
    _baixaTituloReceber.SalvarObservacao.visible(false);
    buscarTitulosPendentes();
    DesabilitaCamposTitulosPendentes(true);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function carregarListaTitulos() {
    var titulosSelecionados = _gridTitulosPendentes.ObterMultiplosSelecionados();

    if (titulosSelecionados.length > 0) {
        var dataGrid = new Array();

        $.each(titulosSelecionados, function (i, titulo) {

            var obj = new Object();
            obj.Codigo = titulo.Codigo;
            obj.CNPJPessoa = titulo.CNPJPessoa;
            obj.CodigoTitulo = titulo.CodigoTitulo;
            obj.NumeroParcela = titulo.NumeroParcela;
            obj.DataVencimento = titulo.DataVencimento;
            obj.Pessoa = titulo.Pessoa;
            obj.Valor = titulo.Valor;

            dataGrid.push(obj);
        });

        _baixaTituloReceber.ListaTitulos.val(JSON.stringify(dataGrid));
    }
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _baixaTituloReceber.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _baixaTituloReceber.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _baixaTituloReceber.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_baixaTituloReceber.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_baixaTituloReceber.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _baixaTituloReceber.ValorBaixado.val(Globalize.format(valorMoedaCotacao * valorOriginal, "n2"));
        }
    }
}

function ConverterValorEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_baixaTituloReceber.ValorMoedaCotacao.val());
        var valorBaixado = Globalize.parseFloat(_baixaTituloReceber.ValorBaixado.val());
        if (valorBaixado > 0 && valorMoedaCotacao > 0) {
            _baixaTituloReceber.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorBaixado / valorMoedaCotacao, "n2"));
        }
    }
}

function DesabilitaCamposTitulosPendentes(v) {
    _baixaTituloReceber.TipoPessoa.enable(v);
    _baixaTituloReceber.GrupoPessoa.enable(v);
    _baixaTituloReceber.Pessoa.enable(v);
    _baixaTituloReceber.TitulosDeAgrupamento.enable(v);
    _baixaTituloReceber.Fatura.enable(v);
    _baixaTituloReceber.Conhecimento.enable(v);
    _baixaTituloReceber.Carga.enable(v);
    _baixaTituloReceber.DataInicial.enable(v);
    _baixaTituloReceber.DataFinal.enable(v);
    _baixaTituloReceber.NumeroPedido.enable(v);
    _baixaTituloReceber.NumeroOcorrencia.enable(v);
    _baixaTituloReceber.FormaTitulo.enable(v);
    _baixaTituloReceber.DataProgramacaoPagamentoInicial.enable(v);
    _baixaTituloReceber.DataProgramacaoPagamentoFinal.enable(v);
    _baixaTituloReceber.NumeroDocumento.enable(v);

    _baixaTituloReceber.SelecionarTodos.enable(v);
    _baixaTituloReceber.PesquisarTitulosPendentes.enable(v);
    _baixaTituloReceber.TitulosPendentes.enable(v);
    _baixaTituloReceber.MoedaCotacaoBancoCentral.enable(v);
    _baixaTituloReceber.DataBaseCRT.enable(v);
    _baixaTituloReceber.ValorMoedaCotacao.enable(v);
    _baixaTituloReceber.ValorOriginalMoedaEstrangeira.enable(v);
}

function AtualizarValorPendente() {
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        return;
    }
    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        return;
    }

    var titulosSelecionados = _gridTitulosPendentes.ObterMultiplosSelecionados();
    var valorTotalPendente = 0.0;

    if (titulosSelecionados.length > 0) {
        $.each(titulosSelecionados, function (i, titulo) {
            valorTotalPendente = valorTotalPendente + parseFloat(titulo.Valor.toString().replace(".", "").replace(",", "."));
            _baixaTituloReceber.ValorPendenteTitulo.val(Globalize.format(valorTotalPendente, "n2"));
        });

    } else
        _baixaTituloReceber.ValorPendenteTitulo.val("0,00");
}