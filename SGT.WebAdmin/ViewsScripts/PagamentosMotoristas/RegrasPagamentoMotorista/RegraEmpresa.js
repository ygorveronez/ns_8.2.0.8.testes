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
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPagamentoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasEmpresa;
var _empresa;

var Empresa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.Empresa = PropertyEntity({ text: "Empresa da carga:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Empresa da carga", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_empresa, _gridRegrasEmpresa, "editarRegraEmpresaClick");
    });

    // Controle de uso
    this.RegraPorEmpresa = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por empresa da carga:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorEmpresa.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorEmpresa(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraEmpresaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraEmpresaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraEmpresaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraEmpresaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorEmpresa(usarRegra) {
    _empresa.Visible.visibleFade(usarRegra);
    _empresa.Regras.required(usarRegra);
}

function loadEmpresa() {
    _empresa = new Empresa();
    KoBindings(_empresa, "knockoutRegraPagamentoEmpresa");

    //-- Busca
    new BuscarEmpresa(_empresa.Empresa);

    //-- Grid Regras
    _gridRegrasEmpresa = new GridReordering(_configRegras.infoTable, _empresa.Regras.idGrid, GeraHeadTable("Empresa"));
    _gridRegrasEmpresa.CarregarGrid();
    $("#" + _empresa.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasPagamentoMotorista(_empresa);
    });
}

function editarRegraEmpresaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _empresa.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _empresa.Codigo.val(regra.Codigo);
        _empresa.Ordem.val(regra.Ordem);
        _empresa.Condicao.val(regra.Condicao);
        _empresa.Juncao.val(regra.Juncao);

        _empresa.Empresa.val(regra.Entidade.Descricao);
        _empresa.Empresa.codEntity(regra.Entidade.Codigo);

        _empresa.Adicionar.visible(false);
        _empresa.Atualizar.visible(true);
        _empresa.Excluir.visible(true);
        _empresa.Cancelar.visible(true);
    }
}

function adicionarRegraEmpresaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_empresa))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraEmpresa();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_empresa);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _empresa.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposEmpresa();
}

function atualizarRegraEmpresaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_empresa))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraEmpresa();

    // Buscar todas regras
    var listaRegras = _empresa.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _empresa.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _empresa.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposEmpresa();
}

function excluirRegraEmpresaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_empresa);
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
    _empresa.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposEmpresa();
}

function cancelarRegraEmpresaClick(e, sender) {
    LimparCamposEmpresa();
}



//*******MÉTODOS*******

function ObjetoRegraEmpresa() {
    var codigo = _empresa.Codigo.val();
    var ordem = _empresa.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasEmpresa.ObterOrdencao().length + 1,
        Juncao: _empresa.Juncao.val(),
        Condicao: _empresa.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_empresa.Empresa.codEntity()),
            Descricao: _empresa.Empresa.val()
        }
    };

    return regra;
}

function LimparCamposEmpresa() {
    _empresa.Codigo.val(_empresa.Codigo.def);
    _empresa.Ordem.val(_empresa.Ordem.def);
    _empresa.Condicao.val(_empresa.Condicao.def);
    _empresa.Juncao.val(_empresa.Juncao.def);

    LimparCampoEntity(_empresa.Empresa);

    _empresa.Adicionar.visible(true);
    _empresa.Atualizar.visible(false);
    _empresa.Excluir.visible(false);
    _empresa.Cancelar.visible(false);
}