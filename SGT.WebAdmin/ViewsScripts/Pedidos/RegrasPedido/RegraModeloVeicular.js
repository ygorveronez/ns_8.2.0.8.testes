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
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasModeloVeicular;
var _modeloVeicular;

var ModeloVeicular = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo Veícular:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Modelo Veícular", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_modeloVeicular, _gridRegrasModeloVeicular, "editarRegraModeloVeicularClick");
    });

    // Controle de uso
    this.RegraPorModeloVeicular = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por modelo veícular:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorModeloVeicular.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorModeloVeicular(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraModeloVeicularClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraModeloVeicularClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraModeloVeicularClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraModeloVeicularClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorModeloVeicular(usarRegra) {
    _modeloVeicular.Visible.visibleFade(usarRegra);
    _modeloVeicular.Regras.required(usarRegra);
}

function loadModeloVeicular() {
    _modeloVeicular = new ModeloVeicular();
    KoBindings(_modeloVeicular, "knockoutRegraModeloVeicular");

    //-- Busca
    new BuscarModelosVeicularesCarga(_modeloVeicular.ModeloVeicular);

    //-- Grid Regras
    _gridRegrasModeloVeicular = new GridReordering(_configRegras.infoTable, _modeloVeicular.Regras.idGrid, GeraHeadTable("Modelo Veícular"));
    _gridRegrasModeloVeicular.CarregarGrid();
    $("#" + _modeloVeicular.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasPagamentoMotorista(_modeloVeicular);
    });
}

function editarRegraModeloVeicularClick(codigo) {
    // Buscar todas regras
    var listaRegras = _modeloVeicular.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _modeloVeicular.Codigo.val(regra.Codigo);
        _modeloVeicular.Ordem.val(regra.Ordem);
        _modeloVeicular.Condicao.val(regra.Condicao);
        _modeloVeicular.Juncao.val(regra.Juncao);

        _modeloVeicular.ModeloVeicular.val(regra.Entidade.Descricao);
        _modeloVeicular.ModeloVeicular.codEntity(regra.Entidade.Codigo);

        _modeloVeicular.Adicionar.visible(false);
        _modeloVeicular.Atualizar.visible(true);
        _modeloVeicular.Excluir.visible(true);
        _modeloVeicular.Cancelar.visible(true);
    }
}

function adicionarRegraModeloVeicularClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_modeloVeicular))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraModeloVeicular();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_modeloVeicular);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _modeloVeicular.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposModeloVeicular();
}

function atualizarRegraModeloVeicularClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_modeloVeicular))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraModeloVeicular();

    // Buscar todas regras
    var listaRegras = _modeloVeicular.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _modeloVeicular.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _modeloVeicular.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposModeloVeicular();
}

function excluirRegraModeloVeicularClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_modeloVeicular);
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
    _modeloVeicular.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposModeloVeicular();
}

function cancelarRegraModeloVeicularClick(e, sender) {
    LimparCamposModeloVeicular();
}

//*******MÉTODOS*******

function ObjetoRegraModeloVeicular() {
    var codigo = _modeloVeicular.Codigo.val();
    var ordem = _modeloVeicular.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasModeloVeicular.ObterOrdencao().length + 1,
        Juncao: _modeloVeicular.Juncao.val(),
        Condicao: _modeloVeicular.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_modeloVeicular.ModeloVeicular.codEntity()),
            Descricao: _modeloVeicular.ModeloVeicular.val()
        }
    };

    return regra;
}

function LimparCamposModeloVeicular() {
    _modeloVeicular.Codigo.val(_modeloVeicular.Codigo.def);
    _modeloVeicular.Ordem.val(_modeloVeicular.Ordem.def);
    _modeloVeicular.Condicao.val(_modeloVeicular.Condicao.def);
    _modeloVeicular.Juncao.val(_modeloVeicular.Juncao.def);

    LimparCampoEntity(_modeloVeicular.ModeloVeicular);

    _modeloVeicular.Adicionar.visible(true);
    _modeloVeicular.Atualizar.visible(false);
    _modeloVeicular.Excluir.visible(false);
    _modeloVeicular.Cancelar.visible(false);
}