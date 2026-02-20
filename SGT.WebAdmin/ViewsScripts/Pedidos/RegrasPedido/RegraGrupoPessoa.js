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
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoOcorrencia.js" />
/// <reference path="RegrasPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegrasGrupoPessoa;
var _grupoPessoa;

var GrupoPessoa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ",issue: 1734, val: ko.observable(EnumCondicaoAutorizaoOcorrencia.IgualA), options: _condicaoAutorizaoValor, def: EnumCondicaoAutorizaoOcorrencia.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoOcorrencia.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizaoOcorrencia.E });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Regras = PropertyEntity({ text: "Grupo de Pessoas", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_grupoPessoa, _gridRegrasGrupoPessoa, "editarRegraGrupoPessoaClick");
    });

    // Controle de uso
    this.RegraPorGrupoPessoa = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por grupo de pessoa:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegraPorGrupoPessoa.val.subscribe(function (novoValor) {
        SincronzarRegras();
        RegraPorGrupoPessoa(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraGrupoPessoaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraGrupoPessoaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraGrupoPessoaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraGrupoPessoaClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function RegraPorGrupoPessoa(usarRegra) {
    _grupoPessoa.Visible.visibleFade(usarRegra);
    _grupoPessoa.Regras.required(usarRegra);
}

function loadGrupoPessoa() {
    _grupoPessoa = new GrupoPessoa();
    KoBindings(_grupoPessoa, "knockoutRegraGrupoPessoa");

    //-- Busca
    new BuscarGruposPessoas(_grupoPessoa.GrupoPessoa);

    //-- Grid Regras
    _gridRegrasGrupoPessoa = new GridReordering(_configRegras.infoTable, _grupoPessoa.Regras.idGrid, GeraHeadTable("Grupo de Pessoa"));
    _gridRegrasGrupoPessoa.CarregarGrid();
    $("#" + _grupoPessoa.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasPagamentoMotorista(_grupoPessoa);
    });
}

function editarRegraGrupoPessoaClick(codigo) {
    // Buscar todas regras
    var listaRegras = _grupoPessoa.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _grupoPessoa.Codigo.val(regra.Codigo);
        _grupoPessoa.Ordem.val(regra.Ordem);
        _grupoPessoa.Condicao.val(regra.Condicao);
        _grupoPessoa.Juncao.val(regra.Juncao);

        _grupoPessoa.GrupoPessoa.val(regra.Entidade.Descricao);
        _grupoPessoa.GrupoPessoa.codEntity(regra.Entidade.Codigo);

        _grupoPessoa.Adicionar.visible(false);
        _grupoPessoa.Atualizar.visible(true);
        _grupoPessoa.Excluir.visible(true);
        _grupoPessoa.Cancelar.visible(true);
    }
}

function adicionarRegraGrupoPessoaClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_grupoPessoa))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraGrupoPessoa();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_grupoPessoa);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _grupoPessoa.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposGrupoPessoa();
}

function atualizarRegraGrupoPessoaClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_grupoPessoa))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraGrupoPessoa();

    // Buscar todas regras
    var listaRegras = _grupoPessoa.Regras.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _grupoPessoa.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _grupoPessoa.Regras.val(listaRegras);

    // Limpa campos
    LimparCamposGrupoPessoa();
}

function excluirRegraGrupoPessoaClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_grupoPessoa);
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
    _grupoPessoa.Regras.val(listaRegras);

    // Limpa o crud
    LimparCamposGrupoPessoa();
}

function cancelarRegraGrupoPessoaClick(e, sender) {
    LimparCamposGrupoPessoa();
}

//*******MÉTODOS*******

function ObjetoRegraGrupoPessoa() {
    var codigo = _grupoPessoa.Codigo.val();
    var ordem = _grupoPessoa.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasGrupoPessoa.ObterOrdencao().length + 1,
        Juncao: _grupoPessoa.Juncao.val(),
        Condicao: _grupoPessoa.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_grupoPessoa.GrupoPessoa.codEntity()),
            Descricao: _grupoPessoa.GrupoPessoa.val()
        }
    };

    return regra;
}

function LimparCamposGrupoPessoa() {
    _grupoPessoa.Codigo.val(_grupoPessoa.Codigo.def);
    _grupoPessoa.Ordem.val(_grupoPessoa.Ordem.def);
    _grupoPessoa.Condicao.val(_grupoPessoa.Condicao.def);
    _grupoPessoa.Juncao.val(_grupoPessoa.Juncao.def);

    LimparCampoEntity(_grupoPessoa.GrupoPessoa);

    _grupoPessoa.Adicionar.visible(true);
    _grupoPessoa.Atualizar.visible(false);
    _grupoPessoa.Excluir.visible(false);
    _grupoPessoa.Cancelar.visible(false);
}