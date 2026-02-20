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
/// <reference path="../../Enumeradores/EnumCondicaoAutorizao.js" />
/// <reference path="../../Enumeradores/EnumJuncaoAutorizao.js" />
/// <reference path="RegrasPagamentoAgregado.js" />
/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridRegrasCliente;
var _cliente;

var Cliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Ordem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false) });

    this.Condicao = PropertyEntity({ text: "Condição: ", val: ko.observable(EnumCondicaoAutorizao.IgualA), options: _condicaoAutorizaoEntidade, def: EnumCondicaoAutorizao.IgualA });
    this.Juncao = PropertyEntity({ text: "Junção: ", val: ko.observable(EnumJuncaoAutorizao.E), options: _juncaoAutorizao, def: EnumJuncaoAutorizao.E });
    this.Cliente = PropertyEntity({ text: "Cliente:", type: types.entity, required: ko.observable(true), codEntity: ko.observable(0), idBtnSearch: guid() });

    // Controle de regra
    this.Alcadas = PropertyEntity({ text: "Cliente", type: types.map, getType: typesKnockout.dynamic, def: new Array(), required: ko.observable(false), val: ko.observable(new Array()), idGrid: guid() });
    this.Alcadas.val.subscribe(function () {
        SincronzarRegras();
        RenderizarGridRegras(_cliente, _gridRegrasCliente, "editarRegraClienteClick");
    });

    // Controle de uso
    this.UsarRegraPorCliente = PropertyEntity({ getType: typesKnockout.bool, text: "Ativar Regra de autorização por agregado:", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.UsarRegraPorCliente.val.subscribe(function (novoValor) {
        SincronzarRegras();
        UsarRegraPorCliente(novoValor);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRegraClienteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRegraClienteClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRegraClienteClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRegraClienteClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}


//*******EVENTOS*******
function UsarRegraPorCliente(usarRegra) {
    _cliente.Visible.visibleFade(usarRegra);
    _cliente.Alcadas.required(usarRegra);
}

function loadCliente() {
    _cliente = new Cliente();
    KoBindings(_cliente, "knockoutRegraCliente");

    //-- Busca
    new BuscarClientes(_cliente.Cliente);

    //-- Grid Regras
    _gridRegrasCliente = new GridReordering(_configRegras.infoTable, _cliente.Alcadas.idGrid, GeraHeadTable("Cliente"));
    _gridRegrasCliente.CarregarGrid();
    $("#" + _cliente.Alcadas.idGrid).on('sortstop', function () {
        LinhasReordenadasDescarteLoteProduto(_cliente);
    });
}

function editarRegraClienteClick(codigo) {
    // Buscar todas regras
    var listaRegras = _cliente.Alcadas.val();
    var regra = null;

    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == codigo) {
            regra = listaRegras[i];
            break;
        }
    }

    if (regra != null) {
        _cliente.Codigo.val(regra.Codigo);
        _cliente.Ordem.val(regra.Ordem);
        _cliente.Condicao.val(regra.Condicao);
        _cliente.Juncao.val(regra.Juncao);

        _cliente.Cliente.val(regra.Entidade.Descricao);
        _cliente.Cliente.codEntity(regra.Entidade.Codigo);

        _cliente.Adicionar.visible(false);
        _cliente.Atualizar.visible(true);
        _cliente.Excluir.visible(true);
        _cliente.Cancelar.visible(true);
    }
}

function adicionarRegraClienteClick() {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_cliente))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraCliente();

    // Buscar todas regras
    var listaRegras = ObterRegrasOrdenadas(_cliente);

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    listaRegras.push(regra);
    _cliente.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposCliente();
}

function atualizarRegraClienteClick(e, sender) {
    // Validacao de campos
    if (!ValidarCamposObrigatorios(_cliente))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    // Codigo da regra
    var regra = ObjetoRegraCliente();

    // Buscar todas regras
    var listaRegras = _cliente.Alcadas.val();

    // Validacao de repetidos
    if (!ValidarRegraDuplicada(listaRegras, regra, false))
        return exibirMensagem(tipoMensagem.atencao, "Regra Duplicada", "Já existe uma regra idêntica a essa.");

    // Itera pra remover a regra
    for (var i in listaRegras) {
        if (listaRegras[i].Codigo == _cliente.Codigo.val()) {
            listaRegras[i] = regra;
            break;
        }
    }

    // Atualiza os valores
    _cliente.Alcadas.val(listaRegras);

    // Limpa campos
    LimparCamposCliente();
}

function excluirRegraClienteClick(e, sender) {
    var listaRegras = ObterRegrasOrdenadas(_cliente);
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
    _cliente.Alcadas.val(listaRegras);

    // Limpa o crud
    LimparCamposCliente();
}

function cancelarRegraClienteClick(e, sender) {
    LimparCamposCliente();
}



//*******MÉTODOS*******

function ObjetoRegraCliente() {
    var codigo = _cliente.Codigo.val();
    var ordem = _cliente.Ordem.val();
    var regra = {
        Codigo: codigo != 0 ? codigo : guid(),
        Ordem: ordem != 0 ? ordem : _gridRegrasCliente.ObterOrdencao().length + 1,
        Juncao: _cliente.Juncao.val(),
        Condicao: _cliente.Condicao.val(),
        Entidade: {
            Codigo: parseInt(_cliente.Cliente.codEntity()),
            Descricao: _cliente.Cliente.val()
        }
    };

    return regra;
}

function LimparCamposCliente() {
    _cliente.Codigo.val(_cliente.Codigo.def);
    _cliente.Ordem.val(_cliente.Ordem.def);
    _cliente.Condicao.val(_cliente.Condicao.def);
    _cliente.Juncao.val(_cliente.Juncao.def);

    LimparCampoEntity(_cliente.Cliente);

    _cliente.Adicionar.visible(true);
    _cliente.Atualizar.visible(false);
    _cliente.Excluir.visible(false);
    _cliente.Cancelar.visible(false);
}