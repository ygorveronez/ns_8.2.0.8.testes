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
/// <reference path="../../Consultas/GrupoProduto.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _produtoAvaria;
var _pesquisaProdutoAvaria;
var _gridProdutoAvaria;

var _configQuantia = { precision: 0, allowZero: true };
var _configPesos = { precision: 6, allowZero: true };
var _configValor = { precision: 2, allowZero: true };

var ProdutoAvaria = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid(), required: true });
    this.QuantidadeCaixa = PropertyEntity({ text: "*Quantidade por Caixa:", required: true, getType: typesKnockout.int, configDecimal: _configQuantia, val: ko.observable(0), def: (0), maxlength: 5 });
    this.CaixasPallet = PropertyEntity({ text: "*Caixas por Pallet:", required: true, getType: typesKnockout.int, configDecimal: _configQuantia, val: ko.observable(0), def: (0), maxlength: 5 });

    this.PesoUnitario = PropertyEntity({ text: "*Peso Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: _configPesos, val: ko.observable(0.00), def: (0.00) });
    this.PesoCaixa = PropertyEntity({ text: "*Peso Caixa:", required: true, getType: typesKnockout.decimal, configDecimal: _configPesos, val: ko.observable(0.00), def: (0.00) });

    this.ValorEstorno = PropertyEntity({ text: "*Valor Estorno:", required: true, getType: typesKnockout.decimal, configDecimal: _configValor, val: ko.observable(0.000000), def: (0.000000) });
    this.ValorProducao = PropertyEntity({ text: "*Valor Produção:", required: true, getType: typesKnockout.decimal, configDecimal: _configValor, val: ko.observable(0.000000), def: (0.000000) });
    this.PrecoTransferencia = PropertyEntity({ text: "*Preço Transferência:", required: true, getType: typesKnockout.decimal, configDecimal: _configValor, val: ko.observable(0.000000), def: (0.000000) });
    this.CustoPrimario = PropertyEntity({ text: "*Custo Primário:", required: true, getType: typesKnockout.decimal, configDecimal: _configValor, val: ko.observable(0.000000), def: (0.000000) });
    this.CustoSecundario = PropertyEntity({ text: "*Custo Secundário:", required: true, getType: typesKnockout.decimal, configDecimal: _configValor, val: ko.observable(0.000000), def: (0.000000) });

    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });
    
    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "ProdutoAvaria/Importar",
        UrlConfiguracao: "ProdutoAvaria/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O004_ProdutoAvaria,
        CallbackImportacao: function () {
            _gridProdutoAvaria.CarregarGrid();
        }
    });
}

var PesquisaProdutoAvaria = function () {
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto:", idBtnSearch: guid() });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto:", idBtnSearch: guid() });

    this.DataInicio = PropertyEntity({ text: "Data inicio: ", getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data fim: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutoAvaria.CarregarGrid();
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
function loadProdutoAvaria() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaProdutoAvaria = new PesquisaProdutoAvaria();
    KoBindings(_pesquisaProdutoAvaria, "knockoutPesquisaProdutoAvaria", false, _pesquisaProdutoAvaria.Pesquisar.id);

    // Instancia ProdutoAvaria
    _produtoAvaria = new ProdutoAvaria();
    KoBindings(_produtoAvaria, "knockoutProdutoAvaria");

    HeaderAuditoria("ProdutoAvaria", _produtoAvaria);

    // Instancia buscas
    new BuscarProdutos(_pesquisaProdutoAvaria.Produto);
    new BuscarGruposProdutos(_pesquisaProdutoAvaria.GrupoProduto);
    new BuscarProdutos(_produtoAvaria.Produto);

    // Inicia busca
    buscarProdutoAvaria();
}

function adicionarClick(e, sender) {
    Salvar(_produtoAvaria, "ProdutoAvaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridProdutoAvaria.CarregarGrid();
                limparCamposProdutoAvaria();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_produtoAvaria, "ProdutoAvaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutoAvaria.CarregarGrid();
                limparCamposProdutoAvaria();
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
        ExcluirPorCodigo(_produtoAvaria, "ProdutoAvaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridProdutoAvaria.CarregarGrid();
                    limparCamposProdutoAvaria();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposProdutoAvaria();
}

function editarProdutoAvariaClick(itemGrid) {
    // Limpa os campos
    limparCamposProdutoAvaria();

    // Seta o codigo do ProdutoAvaria
    _produtoAvaria.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_produtoAvaria, "ProdutoAvaria/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaProdutoAvaria.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _produtoAvaria.Atualizar.visible(true);
                _produtoAvaria.Excluir.visible(true);
                _produtoAvaria.Cancelar.visible(true);
                _produtoAvaria.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarProdutoAvaria() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarProdutoAvariaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "ProdutoAvaria/ExportarPesquisa",
        titulo: "Produto Avaria"
    };
    
    // Inicia Grid de busca
    _gridProdutoAvaria = new GridViewExportacao(_pesquisaProdutoAvaria.Pesquisar.idGrid, "ProdutoAvaria/Pesquisa", _pesquisaProdutoAvaria, menuOpcoes, configExportacao);
    _gridProdutoAvaria.CarregarGrid();
}

function limparCamposProdutoAvaria() {
    _produtoAvaria.Atualizar.visible(false);
    _produtoAvaria.Cancelar.visible(false);
    _produtoAvaria.Excluir.visible(false);
    _produtoAvaria.Adicionar.visible(true);
    LimparCampos(_produtoAvaria);
}