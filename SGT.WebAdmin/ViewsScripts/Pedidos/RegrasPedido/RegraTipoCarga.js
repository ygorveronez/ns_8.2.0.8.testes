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
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasTipoCarga;
var _tipoCarga;

var TipoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Tipo de Carga", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_tipoCarga, _gridRegrasTipoCarga, "editarRegraTipoCargaClick");
    });

    // Controle de uso
    this.RegraPorTipoCarga = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por tipo de carga:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorTipoCarga.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorTipoCarga(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraTipoCargaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraTipoCargaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraTipoCargaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraTipoCargaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorTipoCarga(usarRegra) {
    _tipoCarga.Visible.visibleFade(usarRegra);
    _tipoCarga.Regras.required(usarRegra);
}

function loadTipoCarga() {
    _tipoCarga = new TipoCarga();
    KoBindings(_tipoCarga, "knockoutRegraTipoCarga");

    //-- Busca
    new BuscarTiposdeCarga(_tipoCarga.TipoCarga);

    //-- Grid Regras
    _gridRegrasTipoCarga = new GridReordering(_configRegras.infoTable, _tipoCarga.Regras.idGrid, GeraHeadTable("Tipo de Carga"));
    _gridRegrasTipoCarga.CarregarGrid();
    $("#" + _tipoCarga.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasPagamentoMotorista(_tipoCarga);
    });
}

function editarRegraTipoCargaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _tipoCarga.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _tipoCarga.Codigo.val(regra.Codigo);
        _tipoCarga.Ordem.val(regra.Ordem);
        _tipoCarga.Condicao.val(regra.Condicao);
        _tipoCarga.Juncao.val(regra.Juncao);

        _tipoCarga.TipoCarga.val(regra.Entidade.Descricao);
        _tipoCarga.TipoCarga.codEntity(regra.Entidade.Codigo);

        _tipoCarga.Adicionar.visible(false);
        _tipoCarga.Atualizar.visible(true);
        _tipoCarga.Excluir.visible(true);
        _tipoCarga.Cancelar.visible(true);
    }
}

function adicionarRegraTipoCargaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoCarga))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTipoCarga();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_tipoCarga);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _tipoCarga.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoCarga();
}

function atualizarRegraTipoCargaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_tipoCarga))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraTipoCarga();

    // Buscar todas regras
    var listaRegras = _tipoCarga.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _tipoCarga.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _tipoCarga.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposTipoCarga();
}

function excluirRegraTipoCargaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_tipoCarga);
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
    _tipoCarga.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposTipoCarga();
}

function cancelarRegraTipoCargaClick(e, sender) {
    LimparCamposTipoCarga();
}

//*******MÉTODOS*******

function ObjetoRegraTipoCarga() {
    var codigo = _tipoCarga.Codigo.val();
    var ordem = _tipoCarga.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasTipoCarga.ObterOrdencao().length + 1,
        Juncao: _tipoCarga.Juncao.val(),
        Condicao: _tipoCarga.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_tipoCarga.TipoCarga.codEntity()),
            Descricao: _tipoCarga.TipoCarga.val()
        }
    };

    return regra;
}

function LimparCamposTipoCarga() {
    _tipoCarga.Codigo.val(_tipoCarga.Codigo.def);
    _tipoCarga.Ordem.val(_tipoCarga.Ordem.def);
    _tipoCarga.Condicao.val(_tipoCarga.Condicao.def);
    _tipoCarga.Juncao.val(_tipoCarga.Juncao.def);

    LimparCampoEntity(_tipoCarga.TipoCarga);

    _tipoCarga.Adicionar.visible(true);
    _tipoCarga.Atualizar.visible(false);
    _tipoCarga.Excluir.visible(false);
    _tipoCarga.Cancelar.visible(false);
}