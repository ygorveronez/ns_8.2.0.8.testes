/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDescarteLoteProdutoEmbarcador.js" />
/// <reference path="Etapa.js" />
/// <reference path="DescarteLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridDescartes;
var _descarte;
var _CRUDDescarte;
var _pesquisaDescartes;

var _situacao = [
    { text: "Todas", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao },
    { text: "Finalizado", value: EnumSituacaoDescarteLoteProdutoEmbarcador.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoDescarteLoteProdutoEmbarcador.Rejeitada },
    { text: "Sem Regra", value: EnumSituacaoDescarteLoteProdutoEmbarcador.SemRegra }
];

var Descarte = function () {
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoLancamentoNFSManual.Todas), def: EnumSituacaoLancamentoNFSManual.Todas, text: "Situação: " });
}

var CRUDDescarte = function () {
    this.Adicionar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparLancamentoClick, type: types.event, text: "Limpar", idGrid: guid(), visible: ko.observable(true) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaDescartes = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", options: _situacao, text: "Situação:" });
   
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Embarcador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDescartes.CarregarGrid();
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

function loadDescarteLoteProduto() {
    _descarte = new Descarte();

    _CRUDDescarte = new CRUDDescarte();
    KoBindings(_CRUDDescarte, "knockoutCRUD");

    _pesquisaDescartes = new PesquisaDescartes();
    KoBindings(_pesquisaDescartes, "knockoutPesquisaDescartes", false, _pesquisaDescartes.Pesquisar.id);

    loadEtapaDescarteLote();
    loadDescarteLote();
    loadAnexosDescartes();
    loadAprovacao();

    LimparCamposLancamento();

    // Inicia as buscas
    new BuscarProdutos(_pesquisaDescartes.ProdutoEmbarcador);
    new BuscarProdutoTMS(_pesquisaDescartes.Produto);

    BuscarDescartes();
}

function salvarClick(e, sender) {
    if (!ValidarDadosDescarte())
        return false;

    Salvar(_descarteLote, "DescarteLoteProdutoEmbarcador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Descarte criado com sucesso");

                _descarteLote.Codigo.val(arg.Data.Codigo);

                // Envia arquivos
                var cb_adicionado = function () {
                    _gridDescartes.CarregarGrid();
                    LimparCamposLancamento();
                    BuscarDescartePorCodigo(arg.Data.Codigo);
                };

                if (GetAnexos().length > 0)
                    EnviarArquivosAnexadosAoDescarte(cb_adicionado);
                else
                    cb_adicionado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function reprocessarRegrasClick(e, sender) {
    executarReST("DescarteLoteProdutoEmbarcador/ReprocessarRegras", { Codigo: _descarteLote.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiRegra || arg.Data.Finalizado) {
                    _gridDescartes.CarregarGrid();
                    LimparCamposLancamento();
                    BuscarDescartePorCodigo(arg.Data.Codigo);
                } else 
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparLancamentoClick(e, sender) {
    LimparCamposLancamento();
}


//*******MÉTODOS*******
function BuscarDescartes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDescartes, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridDescartes = new GridView(_pesquisaDescartes.Pesquisar.idGrid, "DescarteLoteProdutoEmbarcador/Pesquisa", _pesquisaDescartes, menuOpcoes);
    _gridDescartes.CarregarGrid();
}

function editarDescartes(itemGrid) {
    // Limpa os campos
    LimparCamposLancamento();

    // Esconde filtros
    _pesquisaDescartes.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarDescartePorCodigo(itemGrid.Codigo);
}

function BuscarDescartePorCodigo(codigo, cb) {
    executarReST("DescarteLoteProdutoEmbarcador/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            // -- Descarte
            EditarDescarte(arg.Data);

            // -- Dados Descate
            EditarDadosDescarte(arg.Data);

            // -- Anexos
            PreencherAnexosDescartes();

            // -- Aprovação
            ListarAprovacoes(arg.Data);

            SetarEtapaDescarteLote();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarDescarte(data) {
    _descarte.Situacao.val(data.Situacao);

    _CRUDDescarte.Adicionar.visible(false);

    if (data.Situacao == EnumSituacaoDescarteLoteProdutoEmbarcador.SemRegra)
        _CRUDDescarte.ReprocessarRegras.visible(true);

    _CRUDDescarte.Limpar.visible(true);
}

function LimparCamposLancamento() {
    LimparCampos(_descarte);

    _CRUDDescarte.Adicionar.visible(true);
    _CRUDDescarte.Limpar.visible(true);
    _CRUDDescarte.ReprocessarRegras.visible(false);

    SetarEtapaInicioLancamento();

    LimparCamposDadosDescarte();
    LimparCamposAnexos();

    $("#knockoutDescarteLote").click();
}

function ValidarDadosDescarte() {
    //var quantidadeDigitada = Globalize.parseFloat(_descarteLote.Quantidade.val() || "0");
    //var quantidadeAtual = Globalize.parseFloat(_descarteLote.QuantidadeAtual.val() || "0");

    //if (quantidadeDigitada > quantidadeAtual) {
    //    exibirMensagem(tipoMensagem.aviso, "Quantidade Informada", "A quantidade informada é superior ao estoque atual");
    //    return false;
    //}

    return true;
}