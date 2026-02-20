/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="AnexosNFSManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridDocumentosSelecao;
var _gridDocumentosPosterioresSelecao;
var _selecaoDocumentos;
var _gridDocumentos;
var notasJaSelecionadas = {};

var _residuais = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _opcoesComplementoOcorrencia = [
    { text: "Todos", value: 0 },
    { text: "Apenas documentos originais", value: 1 },
    { text: "Apenas complementos de ocorrência", value: 2 }
]

var SelecaoDocumentos = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: ko.observable(false), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: ko.observable(false), visible: ko.observable(true), text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: ko.observable(false), visible: ko.observable(true), text: "Tipo de Operação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Filiais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: true, text: "Tomador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.FechamentoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Fechamento de Frete:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoServico = PropertyEntity({ codEntity: ko.observable(0), val: ko.observable(0), def: "", getType: typesKnockout.string, text: "Código Serviço NFS:", def: "", visible: ko.observable(true), enable: ko.observable(true) });
    this.Residuais = PropertyEntity({ val: ko.observable(false), options: _residuais, def: false, text: "Valores Residuais: ", visible: ko.observable(_possuiResiduais) });

    //Campos Invisiveis para tratativa Minerva (Permitir Somente Notas do Mesmo Pedido e Carga)
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false, val: ko.observable("") });
    this.NumeroCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: false });

    this.Moeda = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0, visible: ko.observable(false), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Carga:", enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroInicial = PropertyEntity({ text: "Nº Inicial:", getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Nº Final:", getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial Emissão:*", getType: typesKnockout.date, val: ko.observable(dataAtual), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final Emissão:", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.NumeroPedidoCliente = PropertyEntity({ text: "Número do Pedido no Cliente:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.string });
    this.ComplementoOcorrencia = PropertyEntity({ val: ko.observable(0), options: _opcoesComplementoOcorrencia, def: 0, text: "Complemento de ocorrência: ", visible: true });

    this.ChaveDocumento = PropertyEntity({ text: "Chave da NF-e:", required: false, idBtnSearch: guid(), eventClick: ChaveDocumentoClick, enable: ko.observable(true), visible: ko.observable(true) });
    this.ListaDocumentos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid() });
    this.ListaChaves = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.Documento = PropertyEntity({ type: types.event, text: "Adicionar Documento", idBtnSearch: guid(), visible: ko.observable(false) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), text: "Selecionar Todos" });
    this.SelecionarTodos.val.subscribe(function (val) {
        if (!val)
            notasJaSelecionadas = {};
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            PreencherListaChaves();
            _gridDocumentosSelecao.CarregarGrid(function (r) {
                // TODO:
                // Tirar o setTimeout e chamar o selecionar todos após renderizar tabela
                // Limpar o selecionar todos quando algum campo do filtro for modificado

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros)
                    return;

                // Só marca todos como selecionado quando Transportador e Tomador forem selecionados
                var busca = RetornarObjetoPesquisa(_selecaoDocumentos);
                if ((busca.Tomador > 0 && busca.Transportador > 0)) {
                    if (_selecaoDocumentos.SelecionarTodos.val() == false && r.data.length > 0) {
                        setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                        _selecaoDocumentos.SelecionarTodos.visible(true);
                    }
                }
                else if (_selecaoDocumentos.SelecionarTodos.val() == true) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                } else {
                    _selecaoDocumentos.SelecionarTodos.val(false);
                    _selecaoDocumentos.SelecionarTodos.visible(false);
                }
            });
        }, type: types.event, text: "Pesquisar", idGrid: "grid-lancamento-nfs-manual", visible: ko.observable(true)
    });


    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar Valores Frete por Nº Pedido",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "NFSManual/ImportarValorFretePorNumeroPedido",
        UrlConfiguracao: "NFSManual/ConfiguracaoImportacaoValorFrete",
        CodigoControleImportacao: this.Codigo,
        CallbackImportacao: function () {
            _gridDocumentosSelecao.CarregarGrid();
        }
    });

    this.ImportarFiltroDocumentos = PropertyEntity({
        type: types.local,
        text: "Importar documentos para filtro",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",

        UrlImportacao: "NFSManual/ImportarFiltroDocumentos",
        UrlConfiguracao: "NFSManual/ObterConfiguracaoImportacaoFiltros",
        CodigoControleImportacao: this.Codigo,
        CallbackImportacao: function (arg) {
            _selecaoDocumentos.ListaDocumentos.list = arg.Data.Retorno;
            RecarregarGridDocumentos();
        }
    });

    this.Criar = PropertyEntity({ eventClick: criarClick, type: types.event, text: "Criar", visible: ko.observable(true) });
    this.ImprimirRelacaoDocumentos = PropertyEntity({ eventClick: ImprimirRelacaoDocumentosClick, type: types.event, text: "Imprimir Relação de Documentos", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadSelecaoDocumentos() {

    _selecaoDocumentos = new SelecaoDocumentos();
    KoBindings(_selecaoDocumentos, "knockoutSelecaoDocumentos");

    // Controle de campos
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _selecaoDocumentos.Transportador.visible(false).required(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros) {
        _selecaoDocumentos.Transportador.visible(false).required(false);
        _selecaoDocumentos.Filial.visible(false).required(false);
        _selecaoDocumentos.Filiais.visible(false).required(false);
        _selecaoDocumentos.TipoOperacao.visible(false).required(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _selecaoDocumentos.Transportador.visible(true).required(true).text("Empresa/Filial:");
        _selecaoDocumentos.Filial.visible(false).required(false);
        _selecaoDocumentos.Filiais.visible(false).required(false);
        _selecaoDocumentos.TipoOperacao.visible(false).required(false);
    }
    else {
        _selecaoDocumentos.Transportador.visible(true).required(true);
    }

    if (!_CONFIGURACAO_TMS.PermiteImportarPlanilhaValoresFreteNFSManual) {
        _selecaoDocumentos.Importar.visible(false);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true)
        _selecaoDocumentos.Moeda.visible(true);

    // Inicia as buscas
    new BuscarFilial(_selecaoDocumentos.Filial);
    new BuscarFilial(_selecaoDocumentos.Filiais);
    new BuscarTiposOperacao(_selecaoDocumentos.TipoOperacao);
    new BuscarTransportadores(_selecaoDocumentos.Transportador);
    new BuscarClientes(_selecaoDocumentos.Tomador);
    new BuscarFechamentoFrete(_selecaoDocumentos.FechamentoFrete);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
        if (_CONFIGURACAO_TMS.HabilitarMultiplaSelecaoEmpresaNFSManual) {
            _selecaoDocumentos.Filiais.visible(true).required(true);
            _selecaoDocumentos.Filial.visible(false).required(false);
        }
        else {
            _selecaoDocumentos.Filiais.visible(false).required(false);
            _selecaoDocumentos.Filial.visible(true).required(true);
        }
    }

    var header = [
        { data: "Codigo", visible: false },
        { data: "Documento", title: "Documento", width: "80%" }
    ];

    _gridDocumentosPosterioresSelecao = new BasicDataTable(_selecaoDocumentos.Grid.id, header, null, { column: 0, dir: orderDir.asc });

    new BuscarDocumentosParaEmissaoNFSManual(_selecaoDocumentos.Documento, AdicionarDocumentosNFSManual, _gridDocumentosPosterioresSelecao, _nfsManual.Codigo);
    _selecaoDocumentos.Documento.basicTable = _gridDocumentosPosterioresSelecao;
    _gridDocumentosPosterioresSelecao.CarregarGrid([]);

    // Inicia grid de dados
    GridSelecaoDocumentos();
    CarregarGridDocumentos();
}

function criarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        var testeLocalidadeAviso = "";
        var dados = RetornarObjetoPesquisa(_selecaoDocumentos);
        dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
        dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
        dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

        executarReST("NFSManual/ValidarLocalidadePrestacaoConfiguracao", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (!string.IsNullOrWhiteSpace(retorno.Data.MensagemAviso)) {
                        testeLocalidadeAviso = retorno.Data.MensagemAviso;
                    }
                }
            }
            exibirConfirmacao("Criar NFS", testeLocalidadeAviso + "Você tem certeza que deseja criar uma NFS para os documentos selecionados?", function () {
                executarReST("NFSManual/Adicionar", dados, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS criada com sucesso");
                            _gridNFS.CarregarGrid();
                            BuscarNFSPorCodigo(retorno.Data.Codigo);
                        }
                        else
                            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
                });
            });

        });
    }
}

function ImprimirRelacaoDocumentosClick(e, sender) {
    executarDownload("NFSManual/ImprimirRelacaoDocumentos", { Codigo: _nfsManual.Codigo.val() });
}

//*******MÉTODOS*******

function GridSelecaoDocumentos() {

    if (_gridDocumentosSelecao != null) {
        _gridDocumentosSelecao.Destroy();
        _gridDocumentosSelecao = null;
    }

    //-- Cabecalho
    const removerDaNFS = {
        descricao: "Remover da NFS",
        id: guid(),
        evento: "onclick",
        metodo: RemoverDocumentoDaNFSManual,
        icone: ""
    };

    const menuOpcoes = _selecaoDocumentos.Codigo.val() > 0 && _nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.DadosNota ? {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [removerDaNFS]
    } : null;

    const multiplaescolha = _nfsManual.Situacao.val() != EnumSituacaoLancamentoNFSManual.Todas ? null : {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoDocumentos.SelecionarTodos,
        callbackNaoSelecionado: function (e, item) {
            SelecaoModificado(false, item);
        },
        callbackSelecionado: function (e, item) {
            SelecaoModificado(true, item);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    const editarColuna = PermiteAlterarValoresNFsManual() ? { permite: true, callback: callbackEditarColunaFrete, atualizarRow: true } : null;

    const configExportacao = {
        url: "NFSManual/ExportarPesquisa",
        titulo: "Documentos NFS Manual"
    }

    _gridDocumentosSelecao = new GridView(_selecaoDocumentos.Pesquisar.idGrid, "NFSManual/PesquisaDocumento", _selecaoDocumentos, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, editarColuna, configExportacao);
    _gridDocumentosSelecao.SetPermitirEdicaoColunas(true);
    _gridDocumentosSelecao.SetSalvarPreferenciasGrid(true);
    _gridDocumentosSelecao.CarregarGrid();
}

function PermiteAlterarValoresNFsManual() {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador && _CONFIGURACAO_TMS.PermiteImportarPlanilhaValoresFreteNFSManual;
}


function callbackEditarColunaFrete(dataRow, row, head, callbackTabPress) {
    var dataEnvio = {
        Codigo: dataRow.Codigo,
        Valor: dataRow.ValorFrete,
    };

    executarReST("NFSManual/AlterarValorFreteNFSManualmente", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data);
                _gridDocumentosSelecao.CarregarGrid();
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}


function CompararEAtualizarGridEditableDataRow(dataRow, novaRow) {
    $.each(novaRow, function (i, obj) {
        if (dataRow[i] != null) {
            dataRow[i] = obj;
        }
    });
}

function SelecaoModificado(selecao, item) {
    // Quando o primeiro item é selecionado, seta os filtros de selecao
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    var itensNaoSelecionados = _gridDocumentosSelecao.ObterMultiplosNaoSelecionados();
    // Busca
    var busca = RetornarObjetoPesquisa(_selecaoDocumentos);

    if (itens.length == 1 && !_selecaoDocumentos.SelecionarTodos.val()) {
        // Seta os dados do documentos selecionados nos campos em branco
        var primeiroClick = false;

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) {
            if (busca.Filial == 0 && !_CONFIGURACAO_TMS.HabilitarMultiplaSelecaoEmpresaNFSManual) {
                primeiroClick = true;
                _selecaoDocumentos.Filial.codEntity(itens[0].CodigoFilial).val(itens[0].Filial);

                if (!_CONFIGURACAO_TMS.HabilitarMultiplaSelecaoEmpresaNFSManual && !_CONFIGURACAO_TMS.HabilitarMultiplaSelecaoEmpresaNFSManual)
                    _selecaoDocumentos.Filiais.codEntity(itens[0].CodigoFilial).val(itens[0].Filial);
            }

            if (busca.TipoOperacao == 0) {
                primeiroClick = true;

                if (!_CONFIGURACAO_TMS.HabilitarMultiplaSelecaoEmpresaNFSManual)
                    _selecaoDocumentos.TipoOperacao.codEntity(itens[0].CodigoTipoOperacao).val(itens[0].TipoOperacao);
            }

            if (busca.ComplementoOcorrencia == 0) {
                primeiroClick = true;
                _selecaoDocumentos.ComplementoOcorrencia.val(itens[0].Ocorrencia == "" ? 1 : 2);
            }
        }

        if (busca.Transportador == 0) {
            primeiroClick = true;
            _selecaoDocumentos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Transportador);
        }

        if (busca.Tomador == 0) {
            primeiroClick = true;
            _selecaoDocumentos.Tomador.codEntity(itens[0].CodigoTomador).val(itens[0].Tomador);
        }

        if (busca.FechamentoFrete == 0) {
            primeiroClick = true;
            _selecaoDocumentos.FechamentoFrete.codEntity(itens[0].CodigoFechamentoFrete).val(itens[0].FechamentoFrete);
        }


        if (!_selecaoDocumentos.Residuais.val()) {
            if (_CONFIGURACAO_TMS.PermitirGerarNotaMesmoPedidoCarga) {
                if (busca.Destinatario == 0) {
                    primeiroClick = true;
                    _selecaoDocumentos.Destinatario.codEntity(itens[0].CodigoDestinatario).val(itens[0].Destinatario);
                }
            }

            if (_CONFIGURACAO_TMS.PermitirGerarNotaMesmaCarga || _CONFIGURACAO_TMS.PermitirGerarNotaMesmaCarga) {

                if (busca.Carga == 0) {
                    primeiroClick = true;
                    _selecaoDocumentos.NumeroCarga.val(itens[0].Carga);
                }
            }
        }

        var selecionarTodas = false;

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && !_CONFIGURACAO_TMS.PermitirGerarNotaMesmoPedidoCarga && !_CONFIGURACAO_TMS.PermitirGerarNotaMesmaCarga) {
            if (primeiroClick) {
                if (_selecaoDocumentos.Transportador.codEntity() > 0 && _selecaoDocumentos.Tomador.codEntity() > 0)
                    selecionarTodas = true;
                else
                    selecionarTodas = false;
            }
        }
        else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros && ((!_CONFIGURACAO_TMS.PermitirGerarNotaMesmoPedidoCarga && !_CONFIGURACAO_TMS.PermitirGerarNotaMesmaCarga) || _selecaoDocumentos.Residuais.val())) {
            if (primeiroClick) {
                if (_selecaoDocumentos.Transportador.codEntity() > 0 && _selecaoDocumentos.Tomador.codEntity() > 0 && _selecaoDocumentos.TipoOperacao.codEntity() > 0)
                    selecionarTodas = true;
                else
                    selecionarTodas = false;
            }
        }
        else if (_CONFIGURACAO_TMS.PermitirGerarNotaMesmoPedidoCarga) {
            if (primeiroClick) {
                if (_selecaoDocumentos.Destinatario.codEntity() > 0 && _selecaoDocumentos.NumeroCarga.val() != "")
                    selecionarTodas = true;
                else
                    selecionarTodas = false;
            }
        }
        else if (_CONFIGURACAO_TMS.PermitirGerarNotaMesmaCarga) {
            if (primeiroClick) {
                if (_selecaoDocumentos.NumeroCarga.val() != "")
                    selecionarTodas = true;
                else
                    selecionarTodas = false;
            }
        }

        if (selecao) {
            _gridDocumentosSelecao.CarregarGrid(function () {
                _selecaoDocumentos.SelecionarTodos.visible(selecionarTodas);
            });
        } else {
            _selecaoDocumentos.SelecionarTodos.val(selecionarTodas);
            _selecaoDocumentos.SelecionarTodos.visible(selecionarTodas);
        }
    }
    else if ((itens.length == 0 && !_selecaoDocumentos.SelecionarTodos.val()) || (_selecaoDocumentos.SelecionarTodos.val() && itensNaoSelecionados.length == _gridDocumentosSelecao.NumeroRegistros())) {
        _selecaoDocumentos.SelecionarTodos.visible(false);
        LimparCampos(_selecaoDocumentos);
        _gridDocumentosSelecao.AtualizarRegistrosNaoSelecionados(new Array());
        _gridDocumentosSelecao.AtualizarRegistrosSelecionados(new Array());
        _selecaoDocumentos.SelecionarTodos.val(false);
        _gridDocumentosSelecao.CarregarGrid();
    }
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    // Valida Quantidade
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    // TODO: Se o btn SELECIONAR TODOS estiver clicado, 
    if (itens.length == 0 && !_selecaoDocumentos.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Documentos Selecionados", "Nenhum documento selecionado.");
    }

    return valido;
}

function EditarSelecaoDocumentos(data) {
    _selecaoDocumentos.Transportador.enable(false);
    _selecaoDocumentos.Filial.enable(false);
    _selecaoDocumentos.Filiais.enable(false);
    _selecaoDocumentos.TipoOperacao.enable(false);
    _selecaoDocumentos.Tomador.enable(false);
    _selecaoDocumentos.FechamentoFrete.enable(false);
    _selecaoDocumentos.CodigoServico.visible(false);
    _selecaoDocumentos.Carga.visible(false);
    _selecaoDocumentos.DataInicio.visible(false);
    _selecaoDocumentos.DataFim.visible(false);
    _selecaoDocumentos.NumeroInicial.visible(false);
    _selecaoDocumentos.NumeroFinal.visible(false);
    _selecaoDocumentos.ImprimirRelacaoDocumentos.visible(true);
    _selecaoDocumentos.Criar.visible(false);
    _selecaoDocumentos.Moeda.visible(false);
    _selecaoDocumentos.Codigo.val(data.Codigo);

    if (data.Situacao == EnumSituacaoLancamentoNFSManual.DadosNota)
        _selecaoDocumentos.Documento.visible(true);

    GridSelecaoDocumentos();

    _selecaoDocumentos.Transportador.val(data.Transportador.Descricao);
    _selecaoDocumentos.Filial.val(data.Filial.Descricao);
    _selecaoDocumentos.Filiais.val(data.Filial.Descricao);
    _selecaoDocumentos.TipoOperacao.val(data.TipoOperacao.Descricao);
    _selecaoDocumentos.Tomador.val(data.Tomador.Descricao);
    _selecaoDocumentos.FechamentoFrete.val(data.FechamentoFrete.Descricao);
    _selecaoDocumentos.Moeda.val(data.Moeda);
    _selecaoDocumentos.CodigoServico.val(data.CodigoServico);

    $("#liTabDocumentos").hide();
}

function LimparCamposSelecaoDocumentos() {
    _selecaoDocumentos.Transportador.enable(true);
    _selecaoDocumentos.Filial.enable(true);
    _selecaoDocumentos.Filiais.enable(true);
    _selecaoDocumentos.TipoOperacao.enable(true);
    _selecaoDocumentos.Tomador.enable(true);
    _selecaoDocumentos.FechamentoFrete.enable(true);
    _selecaoDocumentos.CodigoServico.enable(true);
    _selecaoDocumentos.DataInicio.visible(true);
    _selecaoDocumentos.DataFim.visible(true);
    _selecaoDocumentos.NumeroInicial.visible(true);
    _selecaoDocumentos.NumeroFinal.visible(true);
    _selecaoDocumentos.Carga.visible(true);
    _selecaoDocumentos.SelecionarTodos.val(false);
    _selecaoDocumentos.SelecionarTodos.visible(false);
    _selecaoDocumentos.Documento.visible(false);
    _selecaoDocumentos.ImprimirRelacaoDocumentos.visible(false);
    _selecaoDocumentos.Moeda.visible(true);
    _selecaoDocumentos.Criar.visible(true);
    notasJaSelecionadas = {};
    LimparCampos(_selecaoDocumentos);

    $("#liTabDocumentos").show();
    Global.ResetarAbas();
}

function RemoverDocumentoDaNFSManual(documento) {
    exibirConfirmacao("Remover o Documento", "Deseja realmente remover o documento " + documento.Numero + " desta NFS-e Manual?", function () {
        executarReST("NFSManual/RemoverDocumento", { Codigo: documento.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Documento removido com sucesso!");

                    BuscarNFSPorCodigo(_nfsManual.Codigo.val(), null, false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function AdicionarDocumentosNFSManual(documentos) {
    var codigosDocumentos = new Array();

    for (var i = 0; i < documentos.length; i++)
        codigosDocumentos.push(documentos[i].Codigo);

    executarReST("NFSManual/AdicionarDocumentos", { NFSManual: _nfsManual.Codigo.val(), Documentos: JSON.stringify(codigosDocumentos) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                BuscarNFSPorCodigo(_nfsManual.Codigo.val(), null, false);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function CarregarGridDocumentos() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: ExcluirDocumentoGridClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número NF", width: "15%", className: "text-align-left" },
        { data: "Chave", title: "Chave", width: "70%", className: "text-align-left" }
    ];

    _selecaoDocumentos.ListaChaves.val("");
    _selecaoDocumentos.ListaDocumentos.list = [];

    _gridDocumentos = new BasicDataTable(_selecaoDocumentos.ListaDocumentos.idGrid, header, menuOpcoes);
    _gridDocumentos.CarregarGrid([]);
}

function ExcluirDocumentoGridClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir o documento " + data.Chave + "?", function () {

        for (var i = 0; i < _selecaoDocumentos.ListaDocumentos.list.length; i++) {
            var documento = _selecaoDocumentos.ListaDocumentos.list[i];

            if (documento.Codigo == data.Codigo) {
                _selecaoDocumentos.ListaDocumentos.list.splice(i, 1);
                break;
            }
        }

        RecarregarGridDocumentos();
    });
}

function RecarregarGridDocumentos() {
    var data = new Array();

    $.each(_selecaoDocumentos.ListaDocumentos.list, function (i, doc) {
        data.push({
            Codigo: doc.Codigo,
            Numero: doc.Numero,
            Chave: doc.Chave
        });
    });

    _gridDocumentos.CarregarGrid(data);
}

function ChaveDocumentoClick() {
    setTimeout(function () {
        if (_selecaoDocumentos.ChaveDocumento.val() != "") {
            executarReST("NFSManual/ConsultarDocumento", { ChaveDocumento: _selecaoDocumentos.ChaveDocumento.val() }, function (arg) {
                _selecaoDocumentos.ChaveDocumento.val("");
                if (arg.Success) {
                    if (arg.Data) {
                        if (arg.Data.Codigo > 0)
                            AdicionarDocumentoNaGrid(arg.Data);
                        $("#" + _selecaoDocumentos.ChaveDocumento.id).focus();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }
    }, 30);
}

function AdicionarDocumentoNaGrid(dados) {

    if (!_selecaoDocumentos.ListaDocumentos.list.some(function (o) { return o.Codigo == dados.Codigo && o.Chave == dados.Chave; }))
        _selecaoDocumentos.ListaDocumentos.list.push(dados);

    RecarregarGridDocumentos();
}

function PreencherListaChaves() {
    _selecaoDocumentos.ListaChaves.val(JSON.stringify(_selecaoDocumentos.ListaDocumentos.list));
}