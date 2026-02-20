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

var _gridUnidadeMedidaFornecedor;
var _unidadeMedidaFornecedor;
var _pesquisaUnidadeMedidaFornecedor;

var _UnidadeMedidaPesquisa = [
    { value: 0, text: "Todos" },
    { value: EnumUnidadeMedida.Quilograma, text: "KG - Quilograma" },
    { value: EnumUnidadeMedida.Tonelada, text: "TON - Tonelada" },
    { value: EnumUnidadeMedida.Unidade, text: "UNID - Unidade" },
    { value: EnumUnidadeMedida.Litros, text: "LITRO - Litros" },
    { value: EnumUnidadeMedida.MetroCubico, text: "M3 - Metro Cúbico" },
    { value: EnumUnidadeMedida.MMBTU, text: "MMBTU" },
    { value: EnumUnidadeMedida.Servico, text: "SERV - Serviço" },
    { value: EnumUnidadeMedida.Caixa, text: "CX - Caixa" },
    { value: EnumUnidadeMedida.Ampola, text: "AMPOLA - Ampola" },
    { value: EnumUnidadeMedida.Balde, text: "BALDE - Balde" },
    { value: EnumUnidadeMedida.Bandeja, text: "BANDEJ - Bandeja" },
    { value: EnumUnidadeMedida.Barra, text: "BARRA - Barra" },
    { value: EnumUnidadeMedida.Bisnaga, text: "BISNAG - Bisnaga" },
    { value: EnumUnidadeMedida.Bloco, text: "BLOCO - Bloco" },
    { value: EnumUnidadeMedida.Bobina, text: "BOBINA - Bobina" },
    { value: EnumUnidadeMedida.Bombona, text: "BOMB - Bombona" },
    { value: EnumUnidadeMedida.Capsula, text: "CAPS - Capsula" },
    { value: EnumUnidadeMedida.Cartela, text: "CART - Cartela" },
    { value: EnumUnidadeMedida.Cento, text: "CENTO - Cento" },
    { value: EnumUnidadeMedida.Conjunto, text: "CJ - Conjunto" },
    { value: EnumUnidadeMedida.Centimetro, text: "CM - Centimetro" },
    { value: EnumUnidadeMedida.CentimetroQuadrado, text: "CM2 - Centimetro Quadrado" },
    { value: EnumUnidadeMedida.CaixaCom2Unidades, text: "CX2 - Caixa Com 2 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom3Unidades, text: "CX3 - Caixa Com 3 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom5Unidades, text: "CX5 - Caixa Com 5 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom10Unidades, text: "CX10 - Caixa Com 10 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom15Unidades, text: "CX15 - Caixa Com 15 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom20Unidades, text: "CX20 - Caixa Com 20 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom25Unidades, text: "CX25 - Caixa Com 25 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom50Unidades, text: "CX50 - Caixa Com 50 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom100Unidades, text: "CX100 - Caixa Com 100 Unidades" },
    { value: EnumUnidadeMedida.Display, text: "DISP - Display" },
    { value: EnumUnidadeMedida.Duzia, text: "DUZIA - Duzia" },
    { value: EnumUnidadeMedida.Embalagem, text: "EMBAL - Embalagem" },
    { value: EnumUnidadeMedida.Fardo, text: "FARDO - Fardo" },
    { value: EnumUnidadeMedida.Folha, text: "FOLHA - Folha" },
    { value: EnumUnidadeMedida.Frasco, text: "FRASCO - Frasco" },
    { value: EnumUnidadeMedida.Galao, text: "GALAO - Galão" },
    { value: EnumUnidadeMedida.Garrafa, text: "GF - Garrafa" },
    { value: EnumUnidadeMedida.Gramas, text: "GRAMAS - Gramas" },
    { value: EnumUnidadeMedida.Jogo, text: "JOGO - Jogo" },
    { value: EnumUnidadeMedida.Kit, text: "KIT - Kit" },
    { value: EnumUnidadeMedida.Lata, text: "LATA - Lata" },
    { value: EnumUnidadeMedida.Metro, text: "M - Metro" },
    { value: EnumUnidadeMedida.MetroQuadrado, text: "M2 - Metro Quadrado" },
    { value: EnumUnidadeMedida.Milheiro, text: "MILHEI - Milheiro" },
    { value: EnumUnidadeMedida.Mililitro, text: "MILI - Mililitro" },
    { value: EnumUnidadeMedida.MegawattHora, text: "MWH - Megawatt Hora" },
    { value: EnumUnidadeMedida.Pacote, text: "PACOTE - Pacote" },
    { value: EnumUnidadeMedida.Palete, text: "PALETE - Palete" },
    { value: EnumUnidadeMedida.Pares, text: "PARES - Pares" },
    { value: EnumUnidadeMedida.Peca, text: "PC - Peça" },
    { value: EnumUnidadeMedida.Pote, text: "POTE - Pote" },
    { value: EnumUnidadeMedida.Quilate, text: "K - Quilate" },
    { value: EnumUnidadeMedida.Resma, text: "RESMA - Resma" },
    { value: EnumUnidadeMedida.Rolo, text: "ROLO - Rolo" },
    { value: EnumUnidadeMedida.Saco, text: "SACO - Saco" },
    { value: EnumUnidadeMedida.Sacola, text: "SACOLA - Sacola" },
    { value: EnumUnidadeMedida.Tambor, text: "TAMBOR - Tambor" },
    { value: EnumUnidadeMedida.Tanque, text: "TANQUE - Tanque" },
    { value: EnumUnidadeMedida.Tubo, text: "TUBO - Tubo" },
    { value: EnumUnidadeMedida.Vasilhame, text: "VASIL - Vasilhame" },
    { value: EnumUnidadeMedida.Vidro, text: "VIDRO - Vidro" },
    { value: EnumUnidadeMedida.UnidadeUN, text: "UN - Unidade" },
    { value: EnumUnidadeMedida.Cone, text: "CN - Cone" },
    { value: EnumUnidadeMedida.Bolsa, text: "BO - Bolsa" },
    { value: EnumUnidadeMedida.Dose, text: "DS - Dose" }
];

var _UnidadeMedida = [
    { value: EnumUnidadeMedida.Quilograma, text: "KG - Quilograma" },
    { value: EnumUnidadeMedida.Tonelada, text: "TON - Tonelada" },
    { value: EnumUnidadeMedida.Unidade, text: "UNID - Unidade" },
    { value: EnumUnidadeMedida.Litros, text: "LITRO - Litros" },
    { value: EnumUnidadeMedida.MetroCubico, text: "M3 - Metro Cúbico" },
    { value: EnumUnidadeMedida.MMBTU, text: "MMBTU" },
    { value: EnumUnidadeMedida.Servico, text: "SERV - Serviço" },
    { value: EnumUnidadeMedida.Caixa, text: "CX - Caixa" },
    { value: EnumUnidadeMedida.Ampola, text: "AMPOLA - Ampola" },
    { value: EnumUnidadeMedida.Balde, text: "BALDE - Balde" },
    { value: EnumUnidadeMedida.Bandeja, text: "BANDEJ - Bandeja" },
    { value: EnumUnidadeMedida.Barra, text: "BARRA - Barra" },
    { value: EnumUnidadeMedida.Bisnaga, text: "BISNAG - Bisnaga" },
    { value: EnumUnidadeMedida.Bloco, text: "BLOCO - Bloco" },
    { value: EnumUnidadeMedida.Bobina, text: "BOBINA - Bobina" },
    { value: EnumUnidadeMedida.Bombona, text: "BOMB - Bombona" },
    { value: EnumUnidadeMedida.Capsula, text: "CAPS - Capsula" },
    { value: EnumUnidadeMedida.Cartela, text: "CART - Cartela" },
    { value: EnumUnidadeMedida.Cento, text: "CENTO - Cento" },
    { value: EnumUnidadeMedida.Conjunto, text: "CJ - Conjunto" },
    { value: EnumUnidadeMedida.Centimetro, text: "CM - Centimetro" },
    { value: EnumUnidadeMedida.CentimetroQuadrado, text: "CM2 - Centimetro Quadrado" },
    { value: EnumUnidadeMedida.CaixaCom2Unidades, text: "CX2 - Caixa Com 2 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom3Unidades, text: "CX3 - Caixa Com 3 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom5Unidades, text: "CX5 - Caixa Com 5 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom10Unidades, text: "CX10 - Caixa Com 10 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom15Unidades, text: "CX15 - Caixa Com 15 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom20Unidades, text: "CX20 - Caixa Com 20 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom25Unidades, text: "CX25 - Caixa Com 25 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom50Unidades, text: "CX50 - Caixa Com 50 Unidades" },
    { value: EnumUnidadeMedida.CaixaCom100Unidades, text: "CX100 - Caixa Com 100 Unidades" },
    { value: EnumUnidadeMedida.Display, text: "DISP - Display" },
    { value: EnumUnidadeMedida.Duzia, text: "DUZIA - Duzia" },
    { value: EnumUnidadeMedida.Embalagem, text: "EMBAL - Embalagem" },
    { value: EnumUnidadeMedida.Fardo, text: "FARDO - Fardo" },
    { value: EnumUnidadeMedida.Folha, text: "FOLHA - Folha" },
    { value: EnumUnidadeMedida.Frasco, text: "FRASCO - Frasco" },
    { value: EnumUnidadeMedida.Galao, text: "GALAO - Galão" },
    { value: EnumUnidadeMedida.Garrafa, text: "GF - Garrafa" },
    { value: EnumUnidadeMedida.Gramas, text: "GRAMAS - Gramas" },
    { value: EnumUnidadeMedida.Jogo, text: "JOGO - Jogo" },
    { value: EnumUnidadeMedida.Kit, text: "KIT - Kit" },
    { value: EnumUnidadeMedida.Lata, text: "LATA - Lata" },
    { value: EnumUnidadeMedida.Metro, text: "M - Metro" },
    { value: EnumUnidadeMedida.MetroQuadrado, text: "M2 - Metro Quadrado" },
    { value: EnumUnidadeMedida.Milheiro, text: "MILHEI - Milheiro" },
    { value: EnumUnidadeMedida.Mililitro, text: "MILI - Mililitro" },
    { value: EnumUnidadeMedida.MegawattHora, text: "MWH - Megawatt Hora" },
    { value: EnumUnidadeMedida.Pacote, text: "PACOTE - Pacote" },
    { value: EnumUnidadeMedida.Palete, text: "PALETE - Palete" },
    { value: EnumUnidadeMedida.Pares, text: "PARES - Pares" },
    { value: EnumUnidadeMedida.Peca, text: "PC - Peça" },
    { value: EnumUnidadeMedida.Pote, text: "POTE - Pote" },
    { value: EnumUnidadeMedida.Quilate, text: "K - Quilate" },
    { value: EnumUnidadeMedida.Resma, text: "RESMA - Resma" },
    { value: EnumUnidadeMedida.Rolo, text: "ROLO - Rolo" },
    { value: EnumUnidadeMedida.Saco, text: "SACO - Saco" },
    { value: EnumUnidadeMedida.Sacola, text: "SACOLA - Sacola" },
    { value: EnumUnidadeMedida.Tambor, text: "TAMBOR - Tambor" },
    { value: EnumUnidadeMedida.Tanque, text: "TANQUE - Tanque" },
    { value: EnumUnidadeMedida.Tubo, text: "TUBO - Tubo" },
    { value: EnumUnidadeMedida.Vasilhame, text: "VASIL - Vasilhame" },
    { value: EnumUnidadeMedida.Vidro, text: "VIDRO - Vidro" },
    { value: EnumUnidadeMedida.UnidadeUN, text: "UN - Unidade" },
    { value: EnumUnidadeMedida.Cone, text: "CN - Cone" },
    { value: EnumUnidadeMedida.Bolsa, text: "BO - Bolsa" },
    { value: EnumUnidadeMedida.Dose, text: "DS - Dose" }
];

var PesquisaUnidadeMedidaFornecedor = function () {
    this.DescricaoFornecedor = PropertyEntity({ text: "Descrição do Fornecedor: " });
    this.UnidadeDeMedida = PropertyEntity({ val: ko.observable(0), options: _UnidadeMedidaPesquisa, text: "Unidade de Medida: ", def: 0, issue: 88 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridUnidadeMedidaFornecedor.CarregarGrid();
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

var UnidadeMedidaFornecedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoFornecedor = PropertyEntity({ text: "*Descrição do Fornecedor:", required: true });
    this.UnidadeDeMedida = PropertyEntity({ val: ko.observable(1), options: _UnidadeMedida, text: "*Unidade de Medida: ", def: 1, issue: 88, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadUnidadeMedidaFornecedor() {

    _pesquisaUnidadeMedidaFornecedor = new PesquisaUnidadeMedidaFornecedor();
    KoBindings(_pesquisaUnidadeMedidaFornecedor, "knockoutPesquisaUnidadeMedidaFornecedor", false, _pesquisaUnidadeMedidaFornecedor.Pesquisar.id);

    _unidadeMedidaFornecedor = new UnidadeMedidaFornecedor();
    KoBindings(_unidadeMedidaFornecedor, "knockoutCadastroUnidadeMedidaFornecedor");

    HeaderAuditoria("UnidadeMedidaFornecedor", _unidadeMedidaFornecedor);

    buscarMarcasUnidadeMedidaFornecedor();
}

function adicionarClick(e, sender) {
    Salvar(_unidadeMedidaFornecedor, "UnidadeMedidaFornecedor/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridUnidadeMedidaFornecedor.CarregarGrid();
                limparCamposUnidadeMedidaFornecedor();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    Salvar(_unidadeMedidaFornecedor, "UnidadeMedidaFornecedor/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridUnidadeMedidaFornecedor.CarregarGrid();
                limparCamposUnidadeMedidaFornecedor();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a unidade selecionada?", function () {
        ExcluirPorCodigo(_unidadeMedidaFornecedor, "UnidadeMedidaFornecedor/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridUnidadeMedidaFornecedor.CarregarGrid();
                    limparCamposUnidadeMedidaFornecedor();
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
    limparCamposUnidadeMedidaFornecedor();
}

//*******MÉTODOS*******

function editarUnidadeMedidaFornecedor(unidadeMedidaFornecedorGrid) {
    limparCamposUnidadeMedidaFornecedor();
    _unidadeMedidaFornecedor.Codigo.val(unidadeMedidaFornecedorGrid.Codigo);
    BuscarPorCodigo(_unidadeMedidaFornecedor, "UnidadeMedidaFornecedor/BuscarPorCodigo", function (arg) {
        _pesquisaUnidadeMedidaFornecedor.ExibirFiltros.visibleFade(false);
        _unidadeMedidaFornecedor.Atualizar.visible(true);
        _unidadeMedidaFornecedor.Cancelar.visible(true);
        _unidadeMedidaFornecedor.Excluir.visible(true);
        _unidadeMedidaFornecedor.Adicionar.visible(false);
    }, null);
}


function buscarMarcasUnidadeMedidaFornecedor() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarUnidadeMedidaFornecedor, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridUnidadeMedidaFornecedor = new GridView(_pesquisaUnidadeMedidaFornecedor.Pesquisar.idGrid, "UnidadeMedidaFornecedor/Pesquisa", _pesquisaUnidadeMedidaFornecedor, menuOpcoes, null);
    _gridUnidadeMedidaFornecedor.CarregarGrid();
}


function limparCamposUnidadeMedidaFornecedor() {
    _unidadeMedidaFornecedor.Atualizar.visible(false);
    _unidadeMedidaFornecedor.Cancelar.visible(false);
    _unidadeMedidaFornecedor.Excluir.visible(false);
    _unidadeMedidaFornecedor.Adicionar.visible(true);
    LimparCampos(_unidadeMedidaFornecedor);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}