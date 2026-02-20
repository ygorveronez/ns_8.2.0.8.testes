/// <reference path="Parametros.js" />
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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _importacaoTabelaFrete;
var _crudImportacaoTabelaFrete;
var _tabelaFrete;
var _pesquisaImportacaoArquivoTabelaFrete = null
var _gridImportacaoArquivoTabelaFrete = null
var _gridImportacaoPedidoLinha = null, _gridImportacaoPedidoColuna = null;

var ImportacaoTabelaFrete = function () {
    var layoutPadrao = !_utilizarLayoutImportacaoGPA;

    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid(), issue: 78 });
    this.Vigencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Vigência:", idBtnSearch: guid(), issue: 82, visible: ko.observable(layoutPadrao) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", idBtnSearch: guid(), issue: 121, visible: ko.observable(layoutPadrao) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(layoutPadrao) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(layoutPadrao) });

    this.ColunaOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaCEPOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna CEP Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaEstadoOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Estado Origem:", visible: ko.observable(layoutPadrao) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(layoutPadrao) });
    this.ColunaDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaCEPDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna CEP Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaEstadoDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Estado Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaClienteDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Destinatário:", visible: ko.observable(layoutPadrao) });
    this.ColunaClienteOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Remetente:", visible: ko.observable(layoutPadrao) });
    this.ColunaRotaDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Rota Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaRotaOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Rota Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaRegiaoDestino = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Região Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaRegiaoOrigem = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Região Origem:", visible: ko.observable(layoutPadrao) });
    this.ColunaCodigoIntegracao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Código Integração:", visible: ko.observable(layoutPadrao) });
    this.ColunaTransportador = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Transportador:", visible: ko.observable(layoutPadrao) });
    this.ColunaFronteira = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Fronteira:", visible: ko.observable(layoutPadrao) });
    this.ColunaKMSistema = PropertyEntity({ val: ko.observable(""), def: "0", getType: typesKnockout.int, text: "KM :", visible: ko.observable(layoutPadrao) });
    this.ColunaPrioridadeUso = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Prioridade Uso:", visible: ko.observable(layoutPadrao) });
    this.Moeda = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, issue: 0, visible: ko.observable(layoutPadrao && _CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) });
    this.TipoPagamento = PropertyEntity({ text: "Tipo de Pagamento:", options: EnumTipoPagamentoEmissao.obterOpcoes("", "Não selecionado"), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(layoutPadrao) });
    this.ColunaParametrosBase = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Parâmetros Base:", visible: ko.observable(layoutPadrao) });
    this.LinhaInicioDados = PropertyEntity({ val: ko.observable(1), def: 1, getType: typesKnockout.int, text: "Linha Início Dados:", visible: ko.observable(layoutPadrao) });
    this.ColunaCEPDestinoDiasUteis = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Prazo (Dias Úteis) do CEP Destino:", visible: ko.observable(layoutPadrao) });
    this.ColunaCanalEntrega = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Canal Entrega:", visible: ko.observable(layoutPadrao) });
    this.ColunaTipoOperacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Tipo de Operação:", visible: ko.observable(layoutPadrao) });
    this.ColunaTipoCarga = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Tipo de Carga:", visible: ko.observable(layoutPadrao) });
    this.ColunaLeadTimeDias = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "Coluna Lead Time (Dias):", visible: ko.observable(layoutPadrao) });

    this.FreteValidoParaQualquerOrigem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Válido para qualquer uma das origens", visible: ko.observable(layoutPadrao) });
    this.FreteValidoParaQualquerDestino = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Válido para qualquer um dos destinos", visible: ko.observable(layoutPadrao) });
    this.NaoAtualizarValoresZerados = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não atualizar os valores zerados ou não informados na planilha", visible: ko.observable(layoutPadrao) });
    this.NaoValidarTabelasExistentes = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Não validar tabelas existentes na importação", visible: ko.observable(layoutPadrao) });

    this.Parametros = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Arquivo:", val: ko.observable("") });
};

var CRUDImportacaoTabelaFrete = function () {
    this.Importar = PropertyEntity({ eventClick: importarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaImportacaoArquivoTabelaFrete = function () {
    this.Planilha = PropertyEntity({ text: "Planilha:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data inicial: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data final: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ text: "Funcionário:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoImportacaoTabelaFrete.Todas), options: EnumSituacaoImportacaoTabelaFrete.obterOpcoesPesquisa(), def: true, visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: "Mensagem:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ text: "Pesquisar", type: types.event, idGrid: guid(), visible: ko.observable(true), eventClick: pesquisarImportacaoArquivoTabelaFreteClick });
    this.ExibirFiltros = PropertyEntity({ text: "Filtros de Pesquisa", type: types.event, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true), eventClick: function (e) { e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade()); } });
};

//*******EVENTOS*******

function loadImportacaoTabelaFrete() {
    _importacaoTabelaFrete = new ImportacaoTabelaFrete();
    KoBindings(_importacaoTabelaFrete, "knockoutImportacaoTabelaFrete");

    _crudImportacaoTabelaFrete = new CRUDImportacaoTabelaFrete();
    KoBindings(_crudImportacaoTabelaFrete, "knockoutCRUDImportacaoTabelaFrete");

    _pesquisaImportacaoArquivoTabelaFrete = new PesquisaImportacaoArquivoTabelaFrete();
    KoBindings(_pesquisaImportacaoArquivoTabelaFrete, "knockoutPesquisaImportacaoArquivoTabelaFrete", false, _pesquisaImportacaoArquivoTabelaFrete.Pesquisar.id);

    new BuscarLocalidades(_importacaoTabelaFrete.Origem);
    new BuscarLocalidades(_importacaoTabelaFrete.Destino);
    new BuscarTabelasDeFrete(_importacaoTabelaFrete.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaCliente);
    new BuscarVigenciasTabelaFrete(_importacaoTabelaFrete.Vigencia, _importacaoTabelaFrete.TabelaFrete, retornoBuscaVigencias);
    new BuscarTiposOperacao(_importacaoTabelaFrete.TipoOperacao);
    new BuscarTransportadores(_importacaoTabelaFrete.Empresa);
    new BuscarFuncionario(_pesquisaImportacaoArquivoTabelaFrete.Funcionario);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _importacaoTabelaFrete.Empresa.text("Empresa/Filial:");
    }

    if (_utilizarLayoutImportacaoGPA)
        $("#knockoutParametros").hide();
    else
        $("#knockoutParametros").show();

    loadParametros();
}

function loadGridImportacaoArquivoTabelaFrete() {
    var linhas = { descricao: "Visualizar linhas", id: guid(), evento: "onclick", metodo: visualizarLinhasImportacaoClick, tamanho: "10", icone: "" };
    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarImportacaoClick, tamanho: "10", icone: "", visibilidade: reprocessarImportacaoVisible };
    var cancelar = { descricao: "Cancelar", id: guid(), evento: "onclick", metodo: cancelarImportacaoClick, tamanho: "10", icone: "", visibilidade: cancelarImportacaoVisible };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirImportacaoClick, tamanho: "10", icone: "", visibilidade: excluirImportacaoVisible };
    var download = { descricao: "Donwload", id: guid(), evento: "onclick", metodo: downloadArquivoClick, tamanho: "10", icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [linhas, reprocessar, cancelar, excluir, download], tamanho: 10 };

    _gridImportacaoArquivoTabelaFrete = new GridView("grid-importacao-tabela-frete", "ImportacaoArquivoTabelaFrete/Pesquisa", _pesquisaImportacaoArquivoTabelaFrete, menuOpcoes, null, 10);
    _gridImportacaoArquivoTabelaFrete.CarregarGrid();
}

function retornoTabelaFrete(e) {
    limparCamposParametro();
    _importacaoTabelaFrete.Parametros.list = new Array();
    recarregarGridParametros();

    _importacaoTabelaFrete.TabelaFrete.codEntity(e.Codigo);
    _importacaoTabelaFrete.TabelaFrete.val(e.Descricao);

    Global.setarFocoProximoCampo(_importacaoTabelaFrete.Vigencia.id);

    buscarDadosTabelaFrete();
    buscarVigenciaAtual();
}

function buscarDadosTabelaFrete() {
    executarReST("TabelaFrete/BuscarPorCodigo", { Codigo: _importacaoTabelaFrete.TabelaFrete.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _tabelaFrete = arg.Data;

                montarParametros();
            }
        }
    });
}

function buscarVigenciaAtual() {
    executarReST("TabelaFrete/BuscarVigenciaAtual", { TabelaFrete: _importacaoTabelaFrete.TabelaFrete.codEntity() }, function (arg) {
        if (arg.Success)
            if (arg.Data != null)
                retornoBuscaVigencias(arg.Data);
    });
}

function retornoBuscaVigencias(data) {
    _importacaoTabelaFrete.Vigencia.codEntity(data.Codigo);
    _importacaoTabelaFrete.Vigencia.val("De " + data.DataInicial + " até " + data.DataFinal);
}

function importarClick(e, sender) {

    var dados = {
        TabelaFrete: _importacaoTabelaFrete.TabelaFrete.val() != "" ? _importacaoTabelaFrete.TabelaFrete.codEntity() : 0,
        Vigencia: _importacaoTabelaFrete.Vigencia.val() != "" ? _importacaoTabelaFrete.Vigencia.codEntity() : 0,
        TipoOperacao: _importacaoTabelaFrete.TipoOperacao.val() != "" ? _importacaoTabelaFrete.TipoOperacao.codEntity() : 0,
        Origem: _importacaoTabelaFrete.Origem.val() != "" ? _importacaoTabelaFrete.Origem.codEntity() : 0,
        ColunaCEPOrigem: _importacaoTabelaFrete.ColunaCEPOrigem.val(),
        ColunaOrigem: _importacaoTabelaFrete.ColunaOrigem.val(),
        ColunaEstadoOrigem: _importacaoTabelaFrete.ColunaEstadoOrigem.val(),
        Empresa: _importacaoTabelaFrete.Empresa.val() != "" ? _importacaoTabelaFrete.Empresa.codEntity() : 0,
        ColunaDestino: _importacaoTabelaFrete.ColunaDestino.val(),
        ColunaClienteDestino: _importacaoTabelaFrete.ColunaClienteDestino.val(),
        ColunaClienteOrigem: _importacaoTabelaFrete.ColunaClienteOrigem.val(),
        ColunaRotaDestino: _importacaoTabelaFrete.ColunaRotaDestino.val(),
        ColunaRotaOrigem: _importacaoTabelaFrete.ColunaRotaOrigem.val(),
        ColunaRegiaoDestino: _importacaoTabelaFrete.ColunaRegiaoDestino.val(),
        ColunaRegiaoOrigem: _importacaoTabelaFrete.ColunaRegiaoOrigem.val(),
        ColunaEstadoDestino: _importacaoTabelaFrete.ColunaEstadoDestino.val(),
        ColunaCEPDestino: _importacaoTabelaFrete.ColunaCEPDestino.val(),
        ColunaCEPDestinoDiasUteis: _importacaoTabelaFrete.ColunaCEPDestinoDiasUteis.val(),
        ColunaCodigoIntegracao: _importacaoTabelaFrete.ColunaCodigoIntegracao.val(),
        ColunaTransportador: _importacaoTabelaFrete.ColunaTransportador.val(),
        ColunaParametrosBase: _importacaoTabelaFrete.ColunaParametrosBase.val(),
        ColunaCanalEntrega: _importacaoTabelaFrete.ColunaCanalEntrega.val(),
        FreteValidoParaQualquerOrigem: _importacaoTabelaFrete.FreteValidoParaQualquerOrigem.val(),
        FreteValidoParaQualquerDestino: _importacaoTabelaFrete.FreteValidoParaQualquerDestino.val(),
        NaoAtualizarValoresZerados: _importacaoTabelaFrete.NaoAtualizarValoresZerados.val(),
        NaoValidarTabelasExistentes: _importacaoTabelaFrete.NaoValidarTabelasExistentes.val(),
        Destino: _importacaoTabelaFrete.Destino.val() != "" ? _importacaoTabelaFrete.Destino.codEntity() : 0,
        LinhaInicioDados: _importacaoTabelaFrete.LinhaInicioDados.val(),
        Parametros: JSON.stringify(recursiveListEntity(_importacaoTabelaFrete.Parametros)),
        tokenArquivo: "",
        NomeArquivo: "",
        ColunaFronteira: _importacaoTabelaFrete.ColunaFronteira.val(),
        ColunaKMSistema: _importacaoTabelaFrete.ColunaKMSistema.val(),
        ColunaPrioridadeUso: _importacaoTabelaFrete.ColunaPrioridadeUso.val(),
        Moeda: _importacaoTabelaFrete.Moeda.val(),
        TipoPagamento: _importacaoTabelaFrete.TipoPagamento.val(),
        ColunaTipoOperacao: _importacaoTabelaFrete.ColunaTipoOperacao.val(),
        ColunaTipoCarga: _importacaoTabelaFrete.ColunaTipoCarga.val(),
        ColunaLeadTimeDias: _importacaoTabelaFrete.ColunaLeadTimeDias.val(),
    };

    if (_importacaoTabelaFrete.NaoValidarTabelasExistentes.val()) {
        exibirConfirmacao("Confirmação", "Realmente deseja processar a importação de tabelas de frete da planilha sem validar as tabelas existentes ?", function () {
            importar(dados);
        });
    } else {
        importar(dados);
    }
}

function importar(dados) {
    var file = document.getElementById(_importacaoTabelaFrete.Arquivo.id);
    var formData = new FormData();

    formData.append("upload", file.files[0]);

    var urlImportacao = "ImportacaoArquivoTabelaFrete/ImportarArquivo?callback=?";
    enviarArquivo(urlImportacao, {}, formData, function (retorno) {
        file.value = null;
        if (retorno.Success) {
            dados.tokenArquivo = retorno.Data.Token;
            dados.NomeArquivo = retorno.Data.NomeOriginal;

            executarReST("ImportacaoArquivoTabelaFrete/ImportarDadosArquivo", dados, function (ret) {
                if (ret.Success) {
                    if (ret.Data !== false) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", ret.Data.MensagemAviso);
                        loadGridImportacaoArquivoTabelaFrete();
                    } else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", ret.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", ret.Msg);
            });
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function reprocessarImportacaoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoTabelaFrete.Erro || row.Situacao == EnumSituacaoImportacaoTabelaFrete.Sucesso || row.Situacao == EnumSituacaoImportacaoTabelaFrete.Cancelado);
}

function cancelarImportacaoVisible(row) {
    return (row.Situacao == EnumSituacaoImportacaoTabelaFrete.Pendente || row.Situacao == EnumSituacaoImportacaoTabelaFrete.Processando);
}

function excluirImportacaoVisible(row) {
    return (row.Situacao != EnumSituacaoImportacaoTabelaFrete.Processando);
}

function pesquisarImportacaoArquivoTabelaFreteClick(e, sender) {
    $("#container-importacao-arquivo-tabela-frete-conteudo").removeClass("d-none");
    loadGridImportacaoArquivoTabelaFrete();
}

function cancelarClick(e) {
    limparCamposImportacao();
}


function visualizarLinhasImportacaoClick(row) {
    Global.abrirModal('divModalImportacaoTabelaFreteLinha');
    _gridImportacaoPedidoLinha = new GridView("grid-importacao-tabelafrete-linha", "ImportacaoArquivoTabelaFrete/Linhas?codigo=" + row.Codigo, null, null, null, 10, null, true, null, null, 10000, true, null, null, true, null, false, true);
    _gridImportacaoPedidoLinha.CarregarGrid();
}


function reprocessarImportacaoClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a importação de tabelas de frete da planilha ?", function () {
        executarReST("ImportacaoArquivoTabelaFrete/Reprocessar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoArquivoTabelaFrete();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}


function downloadArquivoClick(row) {
    console.log(row.Codigo);
    executarDownload("ImportacaoArquivoTabelaFrete/DownloadArquivo", { Codigo: row.Codigo });
}

function cancelarImportacaoClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar a importação de tabelas de frete da planilha ?", function () {
        executarReST("ImportacaoArquivoTabelaFrete/Cancelar", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoArquivoTabelaFrete();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function excluirImportacaoClick(row) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a importação de tabelas de frete da planilha ?", function () {
        executarReST("ImportacaoArquivoTabelaFrete/Excluir", row, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
                loadGridImportacaoArquivoTabelaFrete();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}


//*******MÉTODOS*******

function limparCamposImportacao() {
    //LimparCampos(_importacaoTabelaFrete);
}
