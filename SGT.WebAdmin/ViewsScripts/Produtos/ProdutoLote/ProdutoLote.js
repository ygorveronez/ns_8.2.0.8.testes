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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />
/// <reference path="../../Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../Consultas/Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoLote;

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

var ProdutoLote = function () {
    this.IdModal = PropertyEntity({ getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable("") });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.UnidadeMedida = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Quilograma), options: _UnidadeMedida, text: "*Unidade de Medida: ", def: EnumUnidadeMedida.Quilograma, issue: 88 });

    this.Quantidade = PropertyEntity({ text: "*Quantidade Est.:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: true });
    this.ValorVenda = PropertyEntity({ text: "*Valor Venda:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: true });
    this.ValorMinimoVenda = PropertyEntity({ text: "*Valor Mínimo Venda:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: true });

    this.CodigoNCM = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*NCM:"), idBtnSearch: guid(), enable: ko.observable(true), issue: 139, required: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo Produto:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarProdutoLoteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarProdutoLoteClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LancarProdutoLote() {
    _produtoLote = new ProdutoLote();
    _produtoLote.IdModal.val(guid());

    RenderizarModalProdutoLote();
}

function RenderizarModalProdutoLote(callback) {
    $.get("Content/Static/Produtos/ProdutoLote.html?dyn=" + _produtoLote.IdModal.val(), function (dataConteudo) {
        dataConteudo = dataConteudo.replace(/#divModalProdutoLote/g, _produtoLote.IdModal.val());
        $("#js-page-content").append(dataConteudo);

        KoBindings(_produtoLote, "knockoutProdutoLote_" + _produtoLote.IdModal.val());

        new BuscarGruposProdutosTMS(_produtoLote.GrupoProduto, null);
        new BuscarNCMS(_produtoLote.CodigoNCM, retornoSelecaoNCMProdutoLote);

        $("#" + _produtoLote.CodigoNCM.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });

        Global.abrirModal(_produtoLote.IdModal.val());

        $('#' + _produtoLote.IdModal.val()).on('hidden.bs.modal', function () {
            $("#" + _produtoLote.IdModal.val()).remove();
        });

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function retornoSelecaoNCMProdutoLote(e) {
    _produtoLote.CodigoNCM.val(e.Descricao);
    _produtoLote.CodigoNCM.codEntity(e.Descricao);
}

function adicionarProdutoLoteClick() {
    Salvar(_produtoLote, "Produto/AdicionarProdutoLote", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                cancelarProdutoLoteClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarProdutoLoteClick() {
    Global.fecharModal(_produtoLote.IdModal.val());
}