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

var _fechamentoPallets;
var _CRUDFechamentoPallets;
var _pesquisaFechamentoPallets;
var _situacaoFechamento = EnumSituacaoFechamentoPallets.obterOpcoes([ "", EnumSituacaoFechamentoPallets.Aberto, EnumSituacaoFechamentoPallets.Finalizado ]);
var _gridFechamentoPallets;

var FechamentoPallets = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var CRUDFechamentoPallets = function () {
    this.GerarFechamento = PropertyEntity({ eventClick: gerarFechamentoClick, type: types.event, text: "Gerar Fechamento", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
}

var PesquisaFechamentoPallets = function () {
    this.Situacao = PropertyEntity({ text: "Situação:", options: _situacaoFechamento });
    this.DataInicio = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), getType: typesKnockout.int });

    this.DataFim.dateRangeInit = this.DataInicio;
    this.DataInicio.dateRangeLimit = this.DataFim;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFechamentoPallets.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadFechamentoPallets() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaFechamentoPallets = new PesquisaFechamentoPallets();
    KoBindings(_pesquisaFechamentoPallets, "knockoutPesquisaFechamentoPallets", false, _pesquisaFechamentoPallets.Pesquisar.id);

    // Instancia objeto principal
    _fechamentoPallets = new FechamentoPallets();

    _CRUDFechamentoPallets = new CRUDFechamentoPallets();
    KoBindings(_CRUDFechamentoPallets, "knockoutCRUD");

    HeaderAuditoria("FechamentoPallets", _fechamentoPallets);

    // Carrega Partes
    LoadEtapasFechamento();
    LoadDadosFechamento();
    LoadAvaria();
    LoadCompraPallets();
    LoadDevolucao();
    LoadDevolucaoValePallet();
    LoadEstoquePallets();
    LoadReforma();
    LoadTransferencia();
    LoadValePallet();
    LoadFinalizarFechamento();

    // Inicia busca
    BuscarFechamentoPallets();
}

function visibilidadeComposicaoFechamentoIncluir(data) {
    return !data.AdicionarAoFechamento;
}
function visibilidadeComposicaoFechamentoRemover(data) {
    return data.AdicionarAoFechamento;
}

function gerarFechamentoClick(e, sender) {
    Salvar(_dadosFechamento, "FechamentoPallets/GerarFechamento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Gerado com sucesso.");
                _gridFechamentoPallets.CarregarGrid();
                BuscarFechamentoPorCodigo(arg.Data.Codigo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_fechamentoPallets, "FechamentoPallets/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridFechamentoPallets.CarregarGrid();
                    LimparCamposFechamentoPallets();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function limparClick(e) {
    LimparCamposFechamentoPallets();
}

function editarFechamentoPalletsClick(itemGrid) {
    // Limpa os campos
    LimparCamposFechamentoPallets();

    BuscarFechamentoPorCodigo(itemGrid.Codigo);
}

function BuscarFechamentoPorCodigo(codigo) {
    // Seta o codigo do objeto
    _fechamentoPallets.Codigo.val(codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_fechamentoPallets, "FechamentoPallets/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaFechamentoPallets.ExibirFiltros.visibleFade(false);
                _CRUDFechamentoPallets.GerarFechamento.visible(false);

                SetarEtapasFechamento();
                EditarDadosFechamento(arg.Data);
                VisualizarAvaria(arg.Data);
                VisualizarCompraPallets(arg.Data);
                VisualizarDevolucao(arg.Data);
                VisualizarDevolucaoValePallet(arg.Data);
                VisualizarEstoquePallets(arg.Data);
                VisualizarReforma(arg.Data);
                VisualizarTransferencia(arg.Data);
                VisualizarValePallet(arg.Data);

                // Alternas os campos de CRUD
                if (arg.Data.Situacao == EnumSituacaoFechamentoPallets.Aberto) {
                    _CRUDFechamentoPallets.GerarFechamento.visible(false);
                    _CRUDFechamentoPallets.Finalizar.visible(true);
                }
                else if (arg.Data.Situacao == EnumSituacaoFechamentoPallets.Finalizado) {
                    _CRUDFechamentoPallets.Excluir.visible(true);
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarFechamentoPallets() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFechamentoPalletsClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridFechamentoPallets = new GridView(_pesquisaFechamentoPallets.Pesquisar.idGrid, "FechamentoPallets/Pesquisa", _pesquisaFechamentoPallets, menuOpcoes, null);
    _gridFechamentoPallets.CarregarGrid();
}

function LimparCamposFechamentoPallets() {
    _CRUDFechamentoPallets.Excluir.visible(false);
    _CRUDFechamentoPallets.Finalizar.visible(false);
    _CRUDFechamentoPallets.GerarFechamento.visible(true);

    SetarEtapaInicioFechamento();
    LimparDadosFechamento();

    LimparCampos(_fechamentoPallets);
}