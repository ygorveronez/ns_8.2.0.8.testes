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
/// <reference path="../../Enumeradores/EnumSituacaoSelecaoSeparacao.js" />



//*******MAPEAMENTO KNOUCKOUT*******

var _separacaoMecadorias;
var _pesquisaSeparacaoMecadorias;
var _gridSeparacaoMecadorias;
var _gridProdutosSeparacao;
var _descarteLoteConfigDecimal = { precision: 3, allowZero: false, allowNegative: false };
var _situacaoSelecaoSeparacao = [
    { text: "Todos", value: "" },
    { text: "Pendente", value: EnumSituacaoSelecaoSeparacao.Pendente },
    { text: "Enviada", value: EnumSituacaoSelecaoSeparacao.Enviada },
    { text: "Finalizada", value: EnumSituacaoSelecaoSeparacao.Finalizada },
    { text: "Cancelada", value: EnumSituacaoSelecaoSeparacao.Cancelada },
];

var SeparacaoMecadorias = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Carga = PropertyEntity({ text: "Carga: " });
    this.Produtos = PropertyEntity({ text: "Produtos: ", idGrid: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Posicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local Armazenamento:", idBtnSearch: guid(), visible: ko.observable(true), eventChange: ChangePosicao, required: true });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto Embarcador:", idBtnSearch: guid(), visible: ko.observable(true), eventChange: ChangeProdutoEmbarcador, required: true });
    this.Quantidade = PropertyEntity({ type: types.map, configDecimal: _descarteLoteConfigDecimal, getType: typesKnockout.decimal, val: ko.observable("0,000"), def: "", text: "*Quantidade:", enable: ko.observable(true), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.GerarImpressao = PropertyEntity({ eventClick: gerarImpressaoClick, type: types.event, text: "Gerar Impressão", visible: ko.observable(true) });
}

var PesquisaSeparacaoMecadorias = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSelecaoSeparacao.Pendente), def: EnumSituacaoSelecaoSeparacao.Pendente, options: _situacaoSelecaoSeparacao, text: "Situação:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSeparacaoMecadorias.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadSeparacaoMecadorias() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaSeparacaoMecadorias = new PesquisaSeparacaoMecadorias();
    KoBindings(_pesquisaSeparacaoMecadorias, "knockoutPesquisaSeparacaoMecadorias", false, _pesquisaSeparacaoMecadorias.Pesquisar.id);

    // Instancia objeto principal
    _separacaoMecadorias = new SeparacaoMecadorias();
    KoBindings(_separacaoMecadorias, "knockoutSeparacaoMecadorias");

    HeaderAuditoria("Separacao", _separacaoMecadorias);

    GridProdutosSeparacao();

    // Inicia busca
    buscarSeparacaoMecadorias();
}

function adicionarClick(e, sender) {
    Salvar(_separacaoMecadorias, "SeparacaoMecadorias/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutosSeparacao.CarregarGrid();
                LimparCamposProdutoSeparado();
                _separacaoMecadorias.Posicao.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function finalizarClick(e, sender) {
    exibirConfirmacao("Finalizar Separação", "Você tem certeza que deseja Finalizar a Separação?", function () {
        executarReST("SeparacaoMecadorias/Finalizar", { Codigo: _separacaoMecadorias.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridSeparacaoMecadorias.CarregarGrid();
                    limparCamposSeparacaoMecadorias();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Cancelar Separação", "Você tem certeza que deseja Cancelar a Separação?", function () {
        executarReST("SeparacaoMecadorias/Cancelar", { Codigo: _separacaoMecadorias.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridSeparacaoMecadorias.CarregarGrid();
                    limparCamposSeparacaoMecadorias();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function gerarImpressaoClick(e, sender) {
    executarDownload("SeparacaoMecadorias/Imprimir", { Codigo: _separacaoMecadorias.Codigo.val() });
}

function limparClick(e) {
    limparCamposSeparacaoMecadorias();
}

function editarSeparacaoMecadoriasClick(itemGrid) {
    // Limpa os campos
    limparCamposSeparacaoMecadorias();

    // Seta o codigo do objeto
    _separacaoMecadorias.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_separacaoMecadorias, "SeparacaoMecadorias/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaSeparacaoMecadorias.ExibirFiltros.visibleFade(false);
                _separacaoMecadorias.Codigo.visibleFade(true);

                _gridProdutosSeparacao.CarregarGrid();

                if (_separacaoMecadorias.Situacao.val() != EnumSituacaoSelecaoSeparacao.Pendente) {
                    _separacaoMecadorias.Finalizar.visible(false);
                    _separacaoMecadorias.Cancelar.visible(false);

                    _separacaoMecadorias.Adicionar.visible(false);
                }

                // Alternas os campos de CRUD
                _separacaoMecadorias.GerarImpressao.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function ChangeProdutoEmbarcador() {
    if(_separacaoMecadorias.ProdutoEmbarcador.val() != "")
        BuscarProdutoEmbarcadorSeparacao(_separacaoMecadorias.ProdutoEmbarcador.val());
}

function ChangePosicao() {
    if (_separacaoMecadorias.Posicao.val() != "")
        BuscarPosicaoSeparacao(_separacaoMecadorias.Posicao.val());
}



//*******MÉTODOS*******
function buscarSeparacaoMecadorias() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSeparacaoMecadoriasClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridSeparacaoMecadorias = new GridView(_pesquisaSeparacaoMecadorias.Pesquisar.idGrid, "SeparacaoMecadorias/Pesquisa", _pesquisaSeparacaoMecadorias, menuOpcoes, null);
    _gridSeparacaoMecadorias.CarregarGrid();
}

function GridProdutosSeparacao() {
    _gridProdutosSeparacao = new GridView(_separacaoMecadorias.Produtos.idGrid, "SeparacaoMecadorias/ProdutosSeparacao", _separacaoMecadorias);
}

function limparCamposSeparacaoMecadorias() {
    _pesquisaSeparacaoMecadorias.ExibirFiltros.visibleFade(true);
    _separacaoMecadorias.Codigo.visibleFade(false);
    LimparCampos(_separacaoMecadorias);

    _separacaoMecadorias.Finalizar.visible(true);
    _separacaoMecadorias.Cancelar.visible(true);

    _separacaoMecadorias.Adicionar.visible(true);
}

function LimparCamposProdutoSeparado() {
    _separacaoMecadorias.Posicao.val(_separacaoMecadorias.Posicao.def);
    _separacaoMecadorias.Posicao.codEntity(_separacaoMecadorias.Posicao.defCodEntity);
    _separacaoMecadorias.ProdutoEmbarcador.val(_separacaoMecadorias.ProdutoEmbarcador.def);
    _separacaoMecadorias.ProdutoEmbarcador.codEntity(_separacaoMecadorias.ProdutoEmbarcador.defCodEntity);
    _separacaoMecadorias.Quantidade.val(_separacaoMecadorias.Quantidade.def);
}

function BuscarProdutoEmbarcadorSeparacao(produto){
    executarReST("SeparacaoMecadorias/BuscarProdutoEmbarcador", { Separacao: _separacaoMecadorias.Codigo.val(), Produto: produto.trim() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _separacaoMecadorias.ProdutoEmbarcador.val(arg.Data.Descricao);
                _separacaoMecadorias.ProdutoEmbarcador.codEntity(arg.Data.Codigo);
                _separacaoMecadorias.Quantidade.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                _separacaoMecadorias.ProdutoEmbarcador.get$().focus();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function BuscarPosicaoSeparacao(posicao) {
    executarReST("SeparacaoMecadorias/BuscarPosicao", { Separacao: _separacaoMecadorias.Codigo.val(), Posicao: posicao.trim() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _separacaoMecadorias.Posicao.val(arg.Data.Abreviacao);
                _separacaoMecadorias.Posicao.codEntity(arg.Data.Codigo);
                _separacaoMecadorias.ProdutoEmbarcador.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}