/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumCondicaoAutorizaoValorFrete.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizaoTabelaFrete.js" />
/// <reference path="RegrasAutorizacaoTabelaFrete.js" />

/*
 * Descricao:
 * Dentro das regras, o funcionamento é igual ou parecido, mas é isolado do crud principal
 * Todas regras alteradas aqui não serao salvas ou não estaão efetivas até que seja incovada
 * A função SincronzarRegras()
 */

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filial;
var _gridRegrasFilial;

/*
 * Declaração das Classes
 */

var Filial = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", issue: 1734, val: ko.observable(EnumCondicaoAutorizaoValorFrete.IgualA), options: _condicaoAutorizaoTabelaFreteEntidade, def: EnumCondicaoAutorizaoValorFrete.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", issue: 1735, val: ko.observable(EnumJuncaoAutorizaoTabelaFrete.E), options: _juncaoAutorizaoTabelaFrete, def: EnumJuncaoAutorizaoTabelaFrete.E });
    this.Filial = PropertyEntity({ text: "Filial: ", type: types.multiplesEntities, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Regras = PropertyEntity({ text: "Filial", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Regras.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_filial, _gridRegrasFilial, "editarRegraFilialClick");
    });

    this.UsarRegraPorFilial = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por filial:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorFilial.val.subscribe(function (novoValor) {
        SincronzarRegras();
        usarRegraPorFilialChange(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraFilialClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraFilialClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraFilialClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraFilialClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilial() {
    _filial = new Filial();
    KoBindings(_filial, "knockoutRegraFilial");

    //-- Busca
    new BuscarFilial(_filial.Filial);

    //-- Grid Regras
    _gridRegrasFilial = new GridReordering(_configRegras.infoTable, _filial.Regras.idGrid, GeraHeadTable("Filial"));
    _gridRegrasFilial.CarregarGrid();
    $("#" + _filial.Regras.idGrid).on('sortstop', function () {
        LinhasReordenadasTabelaFrete(_filial);
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarRegraFilialClick() {
    if (!ValidarCamposObrigatorios(_filial))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var listaRegrasAdicionar = obterListaRegraFilial();
    var listaRegras = ObterRegrasOrdenadas(_filial);

    if (listaRegrasAdicionar.length == 1) {
        if (!ValidarRegraDuplicada(listaRegras, listaRegrasAdicionar[0], false))
            return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

        listaRegras.push(listaRegrasAdicionar[0]);
    }
    else {
        listaRegrasAdicionar.forEach(function (regraAdicionar) {
            if (ValidarRegraDuplicada(listaRegras, regraAdicionar, false))
                listaRegras.push(regraAdicionar);
        });
    }

    _filial.Regras.val(listaRegras);

    LimparCamposFilial();
}

function atualizarRegraFilialClick() {
    if (!ValidarCamposObrigatorios(_filial))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    var listaRegrasAtualizar = obterListaRegraFilial();
    var listaRegras = ObterRegrasOrdenadas(_filial);

    if (listaRegrasAtualizar.length == 1) {
        var regra = listaRegrasAtualizar[0];

        if (!ValidarRegraDuplicada(listaRegras, regra, false))
            return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

        for (var i in listaRegras) {
            if (listaRegras[i].Codigo == regra.Codigo) {
                listaRegras[i] = regra;
                break;
            }
        }
    }
    else {
        listaRegrasAtualizar.forEach(function (regraAtualizar) {
            if (ValidarRegraDuplicada(listaRegras, regraAtualizar, false))
                listaRegras.push(regraAtualizar);
        })
    }

    _filial.Regras.val(listaRegras);

    LimparCamposFilial();
}

function cancelarRegraFilialClick() {
    LimparCamposFilial();
}

function editarRegraFilialClick(codigo) {
    var listaRegras = _filial.Regras.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _filial.Codigo.val(regra.Codigo);
        _filial.Ordem.val(regra.Ordem);
        _filial.Condicao.val(regra.Condicao);
        _filial.Juncao.val(regra.Juncao);

        _filial.Filial.multiplesEntities([{ Codigo: regra.Entidade.Codigo, Descricao: regra.Entidade.Descricao }]);

        _filial.Adicionar.visible(false);
        _filial.Atualizar.visible(true);
        _filial.Excluir.visible(true);
        _filial.Cancelar.visible(true);
    }
}

function excluirRegraFilialClick(e) {
    excluirRegraFilial(e.Codigo.val());

    LimparCamposFilial();
}

function usarRegraPorFilialChange(usarRegra) {
    _filial.Visible.visibleFade(usarRegra);
    _filial.Regras.required(usarRegra);
}

/*
 * Declaração das Funções Privadas
 */

function excluirRegraFilial(codigo) {
    var listaRegras = ObterRegrasOrdenadas(_filial);
    var index = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            index = parseInt(i);
            break;
        }
    }

    listaRegras.splice(index, 1);

    for (i = 1; i <= listaRegras.length; i++)
        listaRegras[i - 1].Ordem = i;

    _filial.Regras.val(listaRegras);
}

function LimparCamposFilial() {
    _filial.Codigo.val(_filial.Codigo.def);
    _filial.Ordem.val(_filial.Ordem.def);
    _filial.Condicao.val(_filial.Condicao.def);
    _filial.Juncao.val(_filial.Juncao.def);

    LimparCampo(_filial.Filial);

    _filial.Adicionar.visible(true);
    _filial.Atualizar.visible(false);
    _filial.Excluir.visible(false);
    _filial.Cancelar.visible(false);
}

function obterListaRegraFilial() {
    var listaRegra = new Array();
    var entidades = _filial.Filial.multiplesEntities();
    var codigo = _filial.Codigo.val();
    var ordem = _filial.Ordem.val();

    if (codigo && (entidades.length > 1)) {
        excluirRegraFilial(codigo);

        codigo = 0;
        ordem = 0;
    }

    ordem = ordem != 0 ? ordem : _gridRegrasFilial.ObterOrdencao().length + 1;

    entidades.forEach(function (entidade) {
        var regra = {
            Codigo: codigo ? codigo : guid(),
            Ordem: ordem,
            Juncao: _filial.Juncao.val(),
            Condicao: _filial.Condicao.val(),
            Entidade: {
                Codigo: parseInt(entidade.Codigo),
                Descricao: entidade.Descricao
            }
        };

        listaRegra.push(regra);

        ordem++;
    });

    return listaRegra;
}
