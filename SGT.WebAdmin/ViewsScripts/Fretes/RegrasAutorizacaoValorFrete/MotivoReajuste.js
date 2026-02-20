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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoValorFrete.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoTabelaFrete.js" />
/// <reference path="../../Consultas/MotivoReajuste.js" />
/// <reference path="RegrasAutorizacaoValorFrete.js" />


/**
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasMotivoReajuste;
var _motivoReajuste;

var MotivoReajuste = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteEntidade, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.MotivoReajuste = PropertyEntity({ text: "Motivo do Reajuste: ", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Motivo do Reajuste", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_motivoReajuste, _gridRegrasMotivoReajuste, "editarRegraMotivoReajusteClick");
    });

    // Controle de uso
    this.UsarRegraPorMotivoReajuste = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por motivo do reajuste:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorMotivoReajuste.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorMotivoReajuste(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraMotivoReajusteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraMotivoReajusteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraMotivoReajusteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraMotivoReajusteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorMotivoReajuste(usarRegra) {
    _motivoReajuste.Visible.visibleFade(usarRegra);
    _motivoReajuste.Regras.required(usarRegra);
}

function loadMotivoReajuste() {
    _motivoReajuste = new MotivoReajuste();
    KoBindings(_motivoReajuste, "knockoutRegraMotivoReajuste");

    //-- Busca
    new BuscarMotivoReajuste(_motivoReajuste.MotivoReajuste);

    //-- Grid Regras
    _gridRegrasMotivoReajuste = new GridReordering(_configRegras.infoTable, _motivoReajuste.Regras.idGrid, GeraHeadTable("Motivo do Reajuste"));
    _gridRegrasMotivoReajuste.CarregarGrid();
    $("#" + _motivoReajuste.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_motivoReajuste);
    });
}

function editarRegraMotivoReajusteClick(codigo) {
    // Buscar todas regras
    var listaRegras = _motivoReajuste.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _motivoReajuste.Codigo.val(regra.Codigo);
        _motivoReajuste.Ordem.val(regra.Ordem);
        _motivoReajuste.Condicao.val(regra.Condicao);
        _motivoReajuste.Juncao.val(regra.Juncao);

        _motivoReajuste.MotivoReajuste.val(regra.Entidade.Descricao);
        _motivoReajuste.MotivoReajuste.codEntity(regra.Entidade.Codigo);

        _motivoReajuste.Adicionar.visible(false);
        _motivoReajuste.Atualizar.visible(true);
        _motivoReajuste.Excluir.visible(true);
        _motivoReajuste.Cancelar.visible(true);
    }
}

function adicionarRegraMotivoReajusteClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivoReajuste))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivoReajuste();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_motivoReajuste);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _motivoReajuste.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMotivoReajuste();
}

function atualizarRegraMotivoReajusteClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_motivoReajuste))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraMotivoReajuste();

    // Buscar todas regras
    var listaRegras = _motivoReajuste.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _motivoReajuste.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _motivoReajuste.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposMotivoReajuste();
}

function excluirRegraMotivoReajusteClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_motivoReajuste);
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
    _motivoReajuste.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposMotivoReajuste();
}

function cancelarRegraMotivoReajusteClick(e, sender) {
    LimparCamposMotivoReajuste();
}



//*******MÉTODOS*******

function ObjetoRegraMotivoReajuste() {
    var codigo = _motivoReajuste.Codigo.val();
    var ordem = _motivoReajuste.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasMotivoReajuste.ObterOrdencao().length + 1,
        Juncao: _motivoReajuste.Juncao.val(),
        Condicao: _motivoReajuste.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_motivoReajuste.MotivoReajuste.codEntity()),
            Descricao: _motivoReajuste.MotivoReajuste.val()
        }
    };

    return regra;
}

function LimparCamposMotivoReajuste() {
    _motivoReajuste.Codigo.val(_motivoReajuste.Codigo.def);
    _motivoReajuste.Ordem.val(_motivoReajuste.Ordem.def);
    _motivoReajuste.Condicao.val(_motivoReajuste.Condicao.def);
    _motivoReajuste.Juncao.val(_motivoReajuste.Juncao.def);

    LimparCampoEntity(_motivoReajuste.MotivoReajuste);

    _motivoReajuste.Adicionar.visible(true);
    _motivoReajuste.Atualizar.visible(false);
    _motivoReajuste.Excluir.visible(false);
    _motivoReajuste.Cancelar.visible(false);
}