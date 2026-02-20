/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoNFSManual.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoNFSManual.js" />
/// <reference path="RegrasAutorizacaoNFSManual.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasValorPrestacaoServico;
var _valorPrestacaoServico;

var ValorPrestacaoServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoNFSManual.IgualA), options: _condicaoAutorizaoNFSManualValor, def: EnumCondicaoAutorizaoNFSManual.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoNFSManual.E), options: _juncaoAutorizaoNFSManual, def: EnumJuncaoAutorizaoNFSManual.E });
    this.ValorPrestacaoServico = PropertyEntity({ text: "Valor da Prestação do Serviço:", type: types.map, getType: typesKnockout.decimal, required: ko.observable(true), def: 0.00 });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Valor da Prestação do Serviço", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_valorPrestacaoServico, _gridRegrasValorPrestacaoServico, "editarRegraValorPrestacaoServicoClick", true);
    });

    // Controle de uso
    this.UsarRegraPorValorPrestacaoServico = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por Valor da Prestação do Serviço:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorValorPrestacaoServico.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorValorPrestacaoServico(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraValorPrestacaoServicoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraValorPrestacaoServicoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraValorPrestacaoServicoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraValorPrestacaoServicoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorValorPrestacaoServico(usarRegra) {
    _valorPrestacaoServico.Visible.visibleFade(usarRegra);
    _valorPrestacaoServico.Regras.required(usarRegra);
}

function loadValorPrestacaoServico() {
    _valorPrestacaoServico = new ValorPrestacaoServico();
    KoBindings(_valorPrestacaoServico, "knockoutRegraValorPrestacaoServico");

    //-- Grid Regras
    _gridRegrasValorPrestacaoServico = new GridReordering(_configRegras.infoTable, _valorPrestacaoServico.Regras.idGrid, GeraHeadTable("Valor da Prestação do Serviço"));
    _gridRegrasValorPrestacaoServico.CarregarGrid();
    $("#" + _valorPrestacaoServico.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasNFSManual(_valorPrestacaoServico);
    });
}

function editarRegraValorPrestacaoServicoClick(codigo) {
    // Buscar todas regras
    var listaRegras = _valorPrestacaoServico.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _valorPrestacaoServico.Codigo.val(regra.Codigo);
        _valorPrestacaoServico.Ordem.val(regra.Ordem);
        _valorPrestacaoServico.Condicao.val(regra.Condicao);
        _valorPrestacaoServico.Juncao.val(regra.Juncao);

        _valorPrestacaoServico.ValorPrestacaoServico.val(Globalize.format(regra.Valor, "n2"));

        _valorPrestacaoServico.Adicionar.visible(false);
        _valorPrestacaoServico.Atualizar.visible(true);
        _valorPrestacaoServico.Excluir.visible(true);
        _valorPrestacaoServico.Cancelar.visible(true);
    }
}

function adicionarRegraValorPrestacaoServicoClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorPrestacaoServico))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorPrestacaoServico();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_valorPrestacaoServico);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _valorPrestacaoServico.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorPrestacaoServico();
}

function atualizarRegraValorPrestacaoServicoClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_valorPrestacaoServico))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraValorPrestacaoServico();

    // Buscar todas regras
    var listaRegras = _valorPrestacaoServico.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, true))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _valorPrestacaoServico.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _valorPrestacaoServico.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposValorPrestacaoServico();
}

function excluirRegraValorPrestacaoServicoClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_valorPrestacaoServico);
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == e.Codigo.val()) {
            index = parseInt(i);
            break;
        }
    }

    // Remove a regra especifica
    listaRegras.splice(index, 1);

    // Itera para corrigir o numero da ordem
    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    // Atuliza o componente de regras
    _valorPrestacaoServico.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposValorPrestacaoServico();
}

function cancelarRegraValorPrestacaoServicoClick(e, sender) {
    LimparCamposValorPrestacaoServico();
}



//*******MÉTODOS*******

function ObjetoRegraValorPrestacaoServico() {
    var codigo = _valorPrestacaoServico.Codigo.val();
    var ordem = _valorPrestacaoServico.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasValorPrestacaoServico.ObterOrdencao().length + 1,
        Juncao: _valorPrestacaoServico.Juncao.val(),
        Condicao: _valorPrestacaoServico.Condicao.val(),
        Valor: Globalize.parseFloat(_valorPrestacaoServico.ValorPrestacaoServico.val())
    };

    return regra;
}

function LimparCamposValorPrestacaoServico() {
    _valorPrestacaoServico.Codigo.val(_valorPrestacaoServico.Codigo.def);
    _valorPrestacaoServico.Ordem.val(_valorPrestacaoServico.Ordem.def);
    _valorPrestacaoServico.Condicao.val(_valorPrestacaoServico.Condicao.def);
    _valorPrestacaoServico.Juncao.val(_valorPrestacaoServico.Juncao.def);

    _valorPrestacaoServico.ValorPrestacaoServico.val(_valorPrestacaoServico.ValorPrestacaoServico.def);

    _valorPrestacaoServico.Adicionar.visible(true);
    _valorPrestacaoServico.Atualizar.visible(false);
    _valorPrestacaoServico.Excluir.visible(false);
    _valorPrestacaoServico.Cancelar.visible(false);
}