/// <reference path="LayoutEDI.js" />
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
/// <reference path="../../Consultas/Campos.js" />
/// <reference path="TipoCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridReorder;
var _camposEDI;
var $elementoEditar;
var $headScroll;

var _alinhamentoCampo = [
    { text: "Esquerda", value: 1 },
    { text: "Direita", value: 0 }
];

var _condicaoCampoEDI = [
    { text: "Nenhum", value: 0 },
    { text: "Sum", value: 1 },
    { text: "Max", value: 2 },
    { text: "Min", value: 3 },
    { text: "First", value: 4 },
    { text: "Count", value: 5 },
    { text: "Last", value: 6 }
];

var _objetoCampoEDI = [
    { text: "Nenhum", value: 0 },
    { text: "CTe", value: 1 },
    { text: "NotaFiscal", value: 2 },
    { text: "InformacaoCarga", value: 3 },
    { text: "ComponentePrestacao", value: 4 },
    { text: "Veiculo", value: 5 },
    { text: "Ocorrencia", value: 6 },
    { text: "Global", value: 7 },
    { text: "Dinamico", value: 8 },
    { text: "Fatura", value: 9 },
    { text: "FaturaCTe", value: 10 },
    { text: "NFe", value: 11 },
    { text: "NFe Volume", value: 12 },
    { text: "CTe Anterior", value: 19 },
    { text: "Outro", value: 13 }
];

var _tipoCampoEDI = [
    { text: "Alfanumerico", value: 0 },
    { text: "Númerico", value: 1 },
    { text: "Decimal", value: 2 },
    { text: "Data e Hora", value: 4 }
];


var CamposLayoutEDIMap = function () {
    this.Ordem = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.Descricao = PropertyEntity({ val: "" });
}

var CamposLayoutEDI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.IdentificadorRegistro = PropertyEntity({ text: "Identificador do Registro: ", required: false });
    this.IdentificadorRegistroPai = PropertyEntity({ text: "Identificador do Registro Pai: " });
    this.Tipo = PropertyEntity({ val: ko.observable(0), options: _tipoCampoEDI, def: 0, text: "*Tipo do Campo: " });
    this.Objeto = PropertyEntity({ val: ko.observable(0), options: _objetoCampoEDI, def: 0, text: "Tipo do Objeto: " });
    this.PropriedadeObjeto = PropertyEntity({ text: "Propriedade Objeto: ", required: false });
    this.PropriedadeObjetoPai = PropertyEntity({ text: "Propriedade Objeto Pai: ", required: false });
    this.Condicao = PropertyEntity({ val: ko.observable(0), options: _condicaoCampoEDI, def: 0, text: "Condição: " });
    this.Mascara = PropertyEntity({ text: "Máscara: " });
    this.Expressao = PropertyEntity({ text: "Expressão: ", maxlength: 5000 });

    this.Alinhamento = PropertyEntity({ val: ko.observable(1), options: _alinhamentoCampo, def: 1, text: "Alinhamento: " });
    this.QuantidadeCaracteres = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "*Quantidade de Caracteres: ", configInt: { precision: 0, allowZero: true }, visible: ko.observable(true) });
    this.QuantidadeInteiros = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "*Quantidade de Inteiros: ", configInt: { precision: 0, allowZero: true }, visible: ko.observable(true) });
    this.QuantidadeDecimais = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "*Quantidade de Decimais: ", configInt: { precision: 0, allowZero: true }, visible: ko.observable(true) });
    this.Indice = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "*Índice: ", configInt: { precision: 0, allowZero: true }, visible: ko.observable(false) });
    this.ValorFixo = PropertyEntity({ text: "Valor Fixo: " });
    this.Repetir = PropertyEntity({ getType: typesKnockout.bool, text: "Repetir?", val: ko.observable(false), def: false });
    this.Status = PropertyEntity({ val: ko.observable("A"), def: "A" });
    this.NaoEscreverRegistro = PropertyEntity({ getType: typesKnockout.bool, text: "Não escrever o registro no arquivo?", val: ko.observable(false), def: false });
    this.RemoverCaracteresEspeciais = PropertyEntity({ getType: typesKnockout.bool, text: "Remover caracteres especiais?", val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCampoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCampoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCampoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCampoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTipoCamposLayoutEDI() {
    _camposEDI = new CamposLayoutEDI();
    KoBindings(_camposEDI, "knockoutCadastroCamposEDI");

    let headHtml = '<tr><th width="5%"></th><th width="5%">Registro</th>';
    headHtml += '<th width="10%">Descrição</th>';
    headHtml += '<th width="10%">Tipo</th>';
    headHtml += '<th width="10%">Objeto</th>';
    headHtml += '<th width="20%">Prop</th>';
    headHtml += '<th width="20%">Prop Pai</th>';
    headHtml += '<th width="4%">Char</th>';
    headHtml += '<th width="4%">Dec</th>';
    headHtml += '<th width="4%">Int</th>';
    
    headHtml += '<th class="text-align-center" width="10%">Editar</th></tr>';

    _gridReorder = new GridReordering("Mova as linhas conforme a prefêrencia de prioridades", "reorderSelector", headHtml, "");
    _gridReorder.CarregarGrid();

    $elementoEditar = $("#knockoutCadastroCamposEDI ");
    $headScroll = $('html, body');
}

function adicionarCampoClick(e, sender) {
    const tudoCerto = ValidarCamposObrigatorios(_camposEDI);
    if (tudoCerto) {
        let objeto = RetornarObjetoPesquisa(_camposEDI);
        objeto.Ordem = _layoutEDI.Campos.val().length + 1;
        objeto.Codigo = guid();
        _layoutEDI.Campos.val().push(objeto);
        recarregarGridReorder();
        reordenarPosicoesCampos();
        recarregarGridReorder();
        limparCamposCamposEDI();
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Campos salvos com sucesso");
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function atualizarCampoClick(e, sender) {
    const tudoCerto = ValidarCamposObrigatorios(_camposEDI);
    if (tudoCerto) {
        let objeto = RetornarObjetoPesquisa(_camposEDI);
        $.each(_layoutEDI.Campos.val(), function (i, campo) {
            if (campo.Codigo == _camposEDI.Codigo.val()) {
                _layoutEDI.Campos.val()[i] = objeto;
                return false;
            }
        });
        recarregarGridReorder();
        reordenarPosicoesCampos();
        recarregarGridReorder();
        limparCamposCamposEDI();
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Campos salvos com sucesso");
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios");
    }
}

function excluirCampoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover este campo " + _camposEDI.Descricao.val() + " do layout EDI " + _layoutEDI.Descricao.val() + "?", function () {
        reordenarPosicoesCampos();
        let novaLista = new Array();
        let novaOrdem = 1;
        $.each(ObterCamposOrdenados(), function (i, campo) {
            if (_camposEDI.Codigo.val() != campo.Codigo) {
                campo.Ordem = novaOrdem;
                novaOrdem++;
                novaLista.push(campo);
            }
        });
        _layoutEDI.Campos.val(novaLista);
        recarregarGridReorder();
        limparCamposCamposEDI();
    });
}

function cancelarCampoClick(e, sender) {
    limparCamposCamposEDI();
}

function limparCamposCamposEDI() {
    _camposEDI.Atualizar.visible(false);
    _camposEDI.Cancelar.visible(false);
    _camposEDI.Excluir.visible(false);
    _camposEDI.Adicionar.visible(true);
    LimparCampos(_camposEDI);
}

function editarCampoEDIClick(codigo) {
    ScrollEditarCampo();
    buscarCampo(codigo);
    _camposEDI.Atualizar.visible(true);
    _camposEDI.Cancelar.visible(true);
    _camposEDI.Excluir.visible(true);
    _camposEDI.Adicionar.visible(false);

}

//*******MÉTODOS*******

function ObterCampos() {
    const campos = _layoutEDI.Campos.val().slice();

    return campos;
}

function ObterCamposOrdenados() {
    let campos = _layoutEDI.Campos.val().slice();

    campos.sort(function (a, b) { return a.Ordem - b.Ordem });
    return campos;
}

function AlternaModoPorInidice(valor) {
    _camposEDI.QuantidadeCaracteres.visible(!valor);
    _camposEDI.Indice.visible(valor);
}

function recarregarGridReorder() {
    let html = "";
    _layoutEDI.Campos.val().sort(function (a, b) { return a.Ordem < b.Ordem });
    $.each(_layoutEDI.Campos.val(), function (i, campo) {
        html += '<tr data-position="' + campo.Ordem + '" id="sort_' + campo.Codigo + '"><td>' + campo.Ordem + '</td>';
        html += '<td>' + campo.IdentificadorRegistro + '</td>';
        html += '<td>' + campo.Descricao + '</td>';
        html += '<td>' + retornarDescricaoArray(campo.Tipo, _tipoCampoEDI) + '</td>';
        html += '<td>' + retornarDescricaoArray(campo.Objeto, _objetoCampoEDI) + '</td>';
        html += '<td>' + campo.PropriedadeObjeto + '</td>';
        html += '<td>' + campo.PropriedadeObjetoPai + '</td>';
        html += '<td>' + campo.QuantidadeCaracteres + '</td>';
        html += '<td>' + campo.QuantidadeDecimais + '</td>';
        html += '<td>' + campo.QuantidadeInteiros + '</td>';

        html += '<td class="text-align-center"><a href="javascript:;" onclick="editarCampoEDIClick(\'' + campo.Codigo + '\');">Editar</a></td></tr>';
    });
    _gridReorder.RecarregarGrid(html);
}

function reordenarPosicoesCampos() {
    let novaLista = new Array();
    let camposOriginal = ObterCampos();    
    _layoutEDI.Campos.val(novaLista);

    let ListaOrdenada = _gridReorder.ObterOrdencao();
    $.each(camposOriginal, function (i, campo) {
        $.each(ListaOrdenada, function (i, ordem) {
            if (ordem.id.split("_")[1] == campo.Codigo) {
                campo.Ordem = ordem.posicao;                
                novaLista.push(campo);
            }
        });
    });

    _layoutEDI.Campos.val(novaLista);
}

function buscarCampo(codigo) {
    let campoRetorno = null;
    $.each(_layoutEDI.Campos.val(), function (i, campo) {
        if (campo.Codigo == codigo) {
            campoRetorno = campo;
            return false;
        }
    });
    const data = { Data: campoRetorno };
    PreencherObjetoKnout(_camposEDI, data);
}

function retornarDescricaoArray(indice, tipoArray) {
    let retorno = "";
    $.each(tipoArray, function (i, tipo) {
        if (tipo.value == indice) {
            retorno = tipo.text;
            return false;
        }
    });
    return retorno;
}

function ScrollEditarCampo() {
    $headScroll.animate({
        scrollTop: $(document).height()
    }, 1000);
}
