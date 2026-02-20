/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="../../Consultas/NaturezaOperacao.js" />
/// <reference path="IntegracaoBaixaTituloPagar.js" />
/// <reference path="NegociacaoBaixaTituloPagar.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
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
/// <reference path="CabecalhoBaixaTituloPagar.js" />
/// <reference path="EtapaBaixaTituloPagar.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Titulo.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoBaixa.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumBaixaTituloPagar.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />

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

var _tipoBaixa = [
    { text: "Todos", value: EnumTipoBaixa.Todos },
    { text: "Gerado com negociações", value: EnumTipoBaixa.ComNegociacao },
    { text: "Não gerado com negociação", value: EnumTipoBaixa.SomenteBaixa }
];

var _gridTitulosPagar;
var _baixaTituloPagar;
var _pesquisaTituloPagar;
var _PermissoesPersonalizadas;
var _gridTitulosPendentes;

var PesquisaTituloPagar = function () {
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, maxlength: 16 });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoTituloPesquisa, def: 0, text: "Situação: " });

    this.DataInicialVencimento = PropertyEntity({ text: "Data Vencimento do Título de: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.ValorInicial = PropertyEntity({ text: "Valor do Título de: ", val: ko.observable("0,00"), def: ko.observable("0,00"), getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ text: "Até: ", val: ko.observable("0,00"), def: ko.observable("0,00"), getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão do Título de: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });

    this.NumeroDocumentoOriginario = PropertyEntity({ text: "Número do Documento Originário: " });
    this.TipoBaixa = PropertyEntity({ val: ko.observable(EnumTipoBaixa.Todos), options: _tipoBaixa, def: EnumTipoBaixa.Todos, text: "Tipo da Baixa: " });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", eventChange: TipoPessoaPesquisaChange });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTitulosPagar.CarregarGrid();
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

var BaixaTituloPagar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(1), options: _etapaBaixa, def: 1 });
    this.Situacao = PropertyEntity({ val: ko.observable(1), options: _situacaoTituloPesquisa, def: 1, text: "Situação: " });

    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento:", enable: ko.observable(true) });

    //Filtros Titulos Pendentes
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", eventChange: TipoPessoaTitulosPendentesChange, enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TitulosDeAgrupamento = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: _titulosDeAgrupamento, def: EnumSimNaoPesquisa.Todos, text: "Agrupado:", enable: ko.observable(true) });
    this.NumeroTitulo = PropertyEntity({ text: "Nº Título:", getType: typesKnockout.int, maxlength: 16, enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Todos), options: EnumFormaTitulo.obterOpcoesPesquisa(), text: "Forma do Título: ", def: EnumFormaTitulo.Todos, required: false, enable: ko.observable(true) });
    this.RaizCnpjPessoa = PropertyEntity({ text: "Raiz Cnpj Pessoa:", enable: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Movimento:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataInicialVencimento = PropertyEntity({ text: "Vencimento de:", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataFinalVencimento = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.ValorInicial = PropertyEntity({ text: "Valor de: ", val: ko.observable(""), def: "", getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), def: "", getType: typesKnockout.decimal, enable: ko.observable(true) });

    this.AutorizacaoInicial = PropertyEntity({ text: "Autorização de:", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.AutorizacaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });

    this.DataInicial = PropertyEntity({ text: "Emissão de:", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento de Entrada:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.NaturezaOperacaoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Natureza de Operação do Documento de Entrada:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Portador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Portador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.PesquisarTitulosPendentes = PropertyEntity({ eventClick: PesquisarTitulosPendentesClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    //

    this.TitulosPendentes = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.Documento = PropertyEntity({ type: types.event, text: "Adicionar Títulos", idBtnSearch: guid(), visible: ko.observable(false) });

    this.ParcelaTitulo = PropertyEntity({ text: "Parcela: ", getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.DocumentosTitulo = PropertyEntity({ text: "Documento(s): ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorPendenteTitulo = PropertyEntity({ text: "Valor: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorPendenteTituloMoedaEstrangeira = PropertyEntity({ text: "Valor Moeda Estrangeira: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.DataBaixa = PropertyEntity({ text: "*Data baixa: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.ValorBaixado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "*Valor:", maxlength: 15, enable: ko.observable(true), val: ko.observable("0,00"), def: "0,00" });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    //this.MoedaCotacaoBancoCentral.val.subscribe(function (novoValor) {
    //    CalcularMoedaEstrangeira();
    //});

    //this.DataBaseCRT.val.subscribe(function (novoValor) {
    //    CalcularMoedaEstrangeira();
    //});

    //this.ValorMoedaCotacao.val.subscribe(function (novoValor) {
    //    ConverterValor();
    //});

    //this.ValorOriginalMoedaEstrangeira.val.subscribe(function (novoValor) {
    //    ConverterValor();
    //});

    this.ListaParcelasNegociacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.SalvarObservacao = PropertyEntity({ eventClick: SalvarObservacaoClick, type: types.event, text: "Salvar Observação", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarBaixa = PropertyEntity({ eventClick: CancelarBaixaClick, type: types.event, text: "Cancelar Baixa", visible: ko.observable(false), enable: ko.observable(true) });
    this.BaixarTitulo = PropertyEntity({ eventClick: GerarBaixaTituloPagarClick, type: types.event, text: "Baixar Título", visible: ko.observable(true), enable: ko.observable(true) });

    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: "Importar Planilha",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn btn-default",
        UrlImportacao: "BaixaTituloPagar/Importar",
        UrlConfiguracao: "BaixaTituloPagar/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O011_BaixaTituloAPagar,
        //CallbackSomenteSeSucesso: true,
        //FecharModalSeSucesso: true,
        CallbackImportacao: function (r) {
            SelecionarTitulosImportacao(r);
            //_baixaTituloPagar.ValorPendenteTitulo.val(r.Data.Retorno.ValorPendente);
            //_gridTitulosPendentes.AtualizarRegistrosSelecionados(r.Data.Retorno.Titulos);
            //_gridTitulosPendentes.CarregarGrid();
            //AtualizarValorPendente();
        },
        ParametrosRequisicao: function () {
            return {};
        }
    });
};

//*******EVENTOS*******

function loadBaixaTitulosPagar() {
    $("#knockoutCabecalhoTituloPagar").hide();

    _pesquisaTituloPagar = new PesquisaTituloPagar();
    KoBindings(_pesquisaTituloPagar, "knockoutPesquisaTituloPagar");

    new BuscarClientes(_pesquisaTituloPagar.Pessoa);
    new BuscarFuncionario(_pesquisaTituloPagar.Operador);
    new BuscarGruposPessoas(_pesquisaTituloPagar.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Fornecedores);

    _baixaTituloPagar = new BaixaTituloPagar();
    KoBindings(_baixaTituloPagar, "knockoutCadastroTituloPagar");

    HeaderAuditoria("TituloBaixa", _baixaTituloPagar);

    new BuscarClientes(_baixaTituloPagar.Pessoa);
    new BuscarDocumentoEntrada(_baixaTituloPagar.DocumentoEntrada, RetornoDocumentoEntrada);
    new BuscarGruposPessoas(_baixaTituloPagar.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Fornecedores);
    new BuscarTitulosPendentesParaBaixaTituloPagar(_baixaTituloPagar.Documento, RetornoConsultaTituloPendenteAdicionarBaixa, true);
    new BuscarNaturezasOperacoesNotaFiscal(_baixaTituloPagar.NaturezaOperacaoEntrada);
    new BuscarClientes(_baixaTituloPagar.Portador);
    new BuscarTipoMovimento(_baixaTituloPagar.TipoMovimento);

    $("#" + _baixaTituloPagar.RaizCnpjPessoa.id).mask("00.000.000", { selectOnFocus: true, clearIfNotMatch: true });

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        $("#lcTituloProvenienteNegociacao").hide();
        $("#lcTituloSemNegociacao").hide();
        $("#lcTituloComRemessaPagamentoEletronico").hide();
        _baixaTituloPagar.DataBaixa.val(Global.DataAtual());
    } else {
        $("#lcTituloVencido").hide();
        $("#lcTituloVencendoHoje").hide();
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _baixaTituloPagar.ValorPendenteTituloMoedaEstrangeira.visible(true);
        _baixaTituloPagar.MoedaCotacaoBancoCentral.visible(true);
        _baixaTituloPagar.DataBaseCRT.visible(true);
        _baixaTituloPagar.ValorMoedaCotacao.visible(true);
        _baixaTituloPagar.ValorOriginalMoedaEstrangeira.visible(true);
    }

    buscarTitulosPagar();
    buscarTitulosPendentes();

    loadCabecalhoBaixaTituloPagar();
    loadEtapaBaixaTituloPagar();
    loadNegociacaoBaixa();
    loadIntegracaoBaixa();
    carregarLancamentoCheque("conteudoCheque");
    DesabilitaCamposTitulosPendentes(true);
}

function RetornoConsultaTituloPendenteAdicionarBaixa(titulosSelecionados) {
    var codigosTitulos = new Array();
    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].Codigo);

    executarReST("BaixaTituloPagar/AdicionarTitulo", { Titulos: JSON.stringify(codigosTitulos), BaixaTituloPagar: _baixaTituloPagar.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_baixaTituloPagar, { Data: r.Data.Detalhes });
                PreencherObjetoKnout(_cabecalhoBaixaTituloPagar, { Data: r.Data.Detalhes });
                _gridTitulosPendentes.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function RemoverTituloPagarClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o título " + data.Codigo + " desta baixa?", function () {
        executarReST("BaixaTituloPagar/RemoverTitulo", { Titulo: data.Codigo, BaixaTituloPagar: _baixaTituloPagar.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_baixaTituloPagar, { Data: r.Data.Detalhes });
                    PreencherObjetoKnout(_cabecalhoBaixaTituloPagar, { Data: r.Data.Detalhes });
                    _gridTitulosPendentes.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function SelecionarTitulosImportacao(r) {
    _gridTitulosPendentes.AtualizarRegistrosSelecionados(r.Data.Retorno.Titulos);
    _gridTitulosPendentes.CarregarGrid();
    AtualizarValorPendente();
}

function RetornoDocumentoEntrada(data) {
    _baixaTituloPagar.DocumentoEntrada.val(data.Numero);
    _baixaTituloPagar.DocumentoEntrada.codEntity(data.Codigo);
}

function TipoPessoaPesquisaChange(e, sender) {
    if (_pesquisaTituloPagar.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        _pesquisaTituloPagar.Pessoa.visible(true);
        _pesquisaTituloPagar.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaTituloPagar.GrupoPessoa);
    } else if (_pesquisaTituloPagar.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        _pesquisaTituloPagar.Pessoa.visible(false);
        _pesquisaTituloPagar.GrupoPessoa.visible(true);
        LimparCampoEntity(_pesquisaTituloPagar.Pessoa);
    }
}

function TipoPessoaTitulosPendentesChange(e, sender) {
    if (_baixaTituloPagar.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        _baixaTituloPagar.Pessoa.visible(true);
        _baixaTituloPagar.GrupoPessoa.visible(false);
        LimparCampoEntity(_baixaTituloPagar.GrupoPessoa);
    } else if (_baixaTituloPagar.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        _baixaTituloPagar.Pessoa.visible(false);
        _baixaTituloPagar.GrupoPessoa.visible(true);
        LimparCampoEntity(_baixaTituloPagar.Pessoa);
    }
}

function PesquisarTitulosPendentesClick(e, sender) {
    buscarTitulosPendentes();
}

function GerarBaixaTituloPagarClick(e, sender) {

    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra finalizada.");
        return;
    }
    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Esta baixa já se encontra cancelada");
        return;
    }

    if (_gridTitulosPendentes == undefined) {
        exibirMensagem(tipoMensagem.aviso, "Selecione os Títulos", "Por favor selecione os títulos desejados antes de realizar a baixa.");
        return;
    }

    carregarListaTitulos();

    if (!string.IsNullOrWhiteSpace(_baixaTituloPagar.ListaTitulos.val())) {

        executarReST("BaixaTituloPagar/BuscarDocumentosSelecionados", RetornarObjetoPesquisa(_baixaTituloPagar), function (r) {
            if (r.Success) {
                if (r.Data) {
                    _baixaTituloPagar.ValorPendenteTitulo.val(r.Data.ValorTotalPendente);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar ao menos um título para gerar a baixa.");
        return;
    }

    Salvar(_baixaTituloPagar, "BaixaTituloPagar/BaixarTitulo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                DesabilitaCamposTitulosPendentes(false);
                _gridTitulosPendentes.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    }, sender);
}

//*******MÉTODOS*******

function editarTituloPagar(tituloPagarGrid) {
    limparCamposBaixaTituloPagar(true);
    _baixaTituloPagar.Codigo.val(tituloPagarGrid.Codigo);
    BuscarPorCodigo(_baixaTituloPagar, "BaixaTituloPagar/BuscarPorCodigo", function (arg) {
        _pesquisaTituloPagar.ExibirFiltros.visibleFade(false);

        CarregarDadosCabecalho(arg.Data);
        PosicionarEtapa(arg.Data);
        buscarTitulosPendentes();

        DesabilitaCamposTitulosPendentes(false);

        $("#knockoutCabecalhoTituloPagar").show();
    }, null);
}

function buscarTitulosPagar() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarTituloPagar, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTitulosPagar = new GridView(_pesquisaTituloPagar.Pesquisar.idGrid, "BaixaTituloPagar/Pesquisa", _pesquisaTituloPagar, menuOpcoes, null, null, null);
    _gridTitulosPagar.CarregarGrid();
}

function buscarTitulosPendentes() {

    if (_gridTitulosPendentes != null)
        _gridTitulosPendentes.Destroy();

    _baixaTituloPagar.SelecionarTodos.visible(true);
    _baixaTituloPagar.SelecionarTodos.val(false);

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
        SelecionarTodosKnout: _baixaTituloPagar.SelecionarTodos,
        somenteLeitura: false
    };

    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: RemoverTituloPagarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [remover], descricao: "Opções" };

    if (_baixaTituloPagar.Codigo.val() <= 0 || _baixaTituloPagar.Etapa.val() != EnumEtapasBaixaTituloPagar.Iniciada)
        menuOpcoes = null;

    var configuracoesExportacao = { url: "BaixaTituloPagar/ExportarPesquisaTitulosPendentes", titulo: "Títulos Pendentes" };

    _gridTitulosPendentes = new GridView(_baixaTituloPagar.TitulosPendentes.idGrid, "BaixaTituloPagar/PesquisaTitulosPendentes", _baixaTituloPagar, menuOpcoes, { column: 5, dir: 'asc' }, 50, null, null, null, multiplaescolha, null, null, configuracoesExportacao);
    _gridTitulosPendentes.CarregarGrid();
}

function limparCamposBaixaTituloPagar(editando) {
    LimparCampos(_baixaTituloPagar);

    LimparCamposNegociacao();
    LimparCamposIntegracao();

    _baixaTituloPagar.CancelarBaixa.visible(false);
    _baixaTituloPagar.SalvarObservacao.visible(false);
    _baixaTituloPagar.Documento.visible(false);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _baixaTituloPagar.DataBaixa.val(Global.DataAtual());
    }

    if (!editando)
        buscarTitulosPendentes();

    DesabilitaCamposTitulosPendentes(true);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function carregarListaTitulos() {
    var titulosSelecionados = null;

    if (_baixaTituloPagar.SelecionarTodos.val())
        titulosSelecionados = _gridTitulosPendentes.ObterMultiplosNaoSelecionados();
    else
        titulosSelecionados = _gridTitulosPendentes.ObterMultiplosSelecionados();

    var codigosTitulos = new Array();

    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _baixaTituloPagar.SelecionarTodos.val()))
        _baixaTituloPagar.ListaTitulos.val(JSON.stringify(codigosTitulos));
    else
        _baixaTituloPagar.ListaTitulos.val("");
}

function AtualizarValorPendente() {

    if (_baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada || _baixaTituloPagar.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada)
        return;

    carregarListaTitulos();

    if (!string.IsNullOrWhiteSpace(_baixaTituloPagar.ListaTitulos.val())) {

        executarReST("BaixaTituloPagar/BuscarDocumentosSelecionados", RetornarObjetoPesquisa(_baixaTituloPagar), function (r) {
            if (r.Success) {
                if (r.Data) {
                    var data = r.Data;
                    var valorTotalPendente = data.ValorTotalPendente;
                    var documentosSelecionados = data.DocumentosTitulo;

                    _baixaTituloPagar.ValorPendenteTitulo.val(valorTotalPendente);
                    _baixaTituloPagar.DocumentosTitulo.val(documentosSelecionados.split('.').join(""));
                    _baixaTituloPagar.ValorPendenteTituloMoedaEstrangeira.val(data.ValorOriginalMoedaEstrangeira);

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
                        _baixaTituloPagar.ValorBaixado.val(valorTotalPendente);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else {
        _baixaTituloPagar.ValorPendenteTitulo.val("0,00");
        _baixaTituloPagar.DocumentosTitulo.val("");
        _baixaTituloPagar.ValorBaixado.val("0,00");
        _baixaTituloPagar.ValorPendenteTituloMoedaEstrangeira.val("0,00");
    }
}

function SalvarObservacaoClick(e, sender) {
    if (_baixaTituloPagar == null || _baixaTituloPagar.Codigo == null || _baixaTituloPagar.Codigo.val() == null || _baixaTituloPagar.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa antes de salvar uma observação.");
        return;
    }

    Salvar(_baixaTituloPagar, "BaixaTituloPagar/SalvarObservacao", function (arg) {
        if (arg.Success) {
            CarregarDadosCabecalho(arg.Data);
            PosicionarEtapa(arg.Data);
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function CancelarBaixaClick(e, sender) {
    if (_baixaTituloPagar == null || _baixaTituloPagar.Codigo == null || _baixaTituloPagar.Codigo.val() == null || _baixaTituloPagar.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie uma baixa antes de cancelar a mesma.");
        return;
    }

    exibirConfirmacao("Confirmação", "Deseja realmente cancelar/remover esta baixa?", function () {
        Salvar(_baixaTituloPagar, "BaixaTituloPagar/CancelarBaixa", function (arg) {
            if (arg.Success) {
                LimparCampos(_baixaTituloPagar);
                LimparCampos(_cabecalhoBaixaTituloPagar);
                LimparCampos(_integracaoBaixa);
                LimparCampos(_negociacaoBaixa);
                loadBaixaTitulosPagar();
                var data = {
                    Etapa: EnumEtapasBaixaTituloPagar.Iniciada
                };
                PosicionarEtapa(data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Baixa cancelada com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _baixaTituloPagar.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _baixaTituloPagar.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _baixaTituloPagar.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_baixaTituloPagar.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_baixaTituloPagar.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _baixaTituloPagar.ValorBaixado.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_baixaTituloPagar.ValorMoedaCotacao.val());
        var valorBaixado = Globalize.parseFloat(_baixaTituloPagar.ValorBaixado.val());
        if (valorBaixado > 0 && valorMoedaCotacao > 0) {
            _baixaTituloPagar.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorBaixado / valorMoedaCotacao, "n2"));
        }
    }
}

function DesabilitaCamposTitulosPendentes(v) {
    _baixaTituloPagar.DocumentoEntrada.enable(v);
    _baixaTituloPagar.TipoMovimento.enable(v);
    _baixaTituloPagar.TipoPessoa.enable(v);
    _baixaTituloPagar.GrupoPessoa.enable(v);
    _baixaTituloPagar.Pessoa.enable(v);
    _baixaTituloPagar.TitulosDeAgrupamento.enable(v);
    _baixaTituloPagar.NumeroTitulo.enable(v);
    _baixaTituloPagar.DataInicialVencimento.enable(v);
    _baixaTituloPagar.AutorizacaoInicial.enable(v);
    _baixaTituloPagar.AutorizacaoFinal.enable(v);
    _baixaTituloPagar.DataFinalVencimento.enable(v);
    _baixaTituloPagar.ValorInicial.enable(v);
    _baixaTituloPagar.ValorFinal.enable(v);
    _baixaTituloPagar.DataInicial.enable(v);
    _baixaTituloPagar.DataFinal.enable(v);
    _baixaTituloPagar.PesquisarTitulosPendentes.enable(v);
    _baixaTituloPagar.TitulosPendentes.enable(v);
    _baixaTituloPagar.NumeroDocumento.enable(v);
    _baixaTituloPagar.FormaTitulo.enable(v);
    _baixaTituloPagar.NaturezaOperacaoEntrada.enable(v);
    _baixaTituloPagar.RaizCnpjPessoa.enable(v);
    _baixaTituloPagar.Portador.enable(v);

    _baixaTituloPagar.MoedaCotacaoBancoCentral.enable(v);
    _baixaTituloPagar.DataBaseCRT.enable(v);
    _baixaTituloPagar.ValorMoedaCotacao.enable(v);
    _baixaTituloPagar.ValorOriginalMoedaEstrangeira.enable(v);
    _baixaTituloPagar.DataBaixa.enable(v);
    _baixaTituloPagar.ValorBaixado.enable(v);
}