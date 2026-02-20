/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroNotaParcial;
var _CRUDcadastroNotaParcial;
var _gridNotaParcial;
var _gridNotasFiscais;
var _notaParcial;

/*
 * Declaração das Classes
 */

var CRUDCadastroNotaParcial = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarNotaParcialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarNotaParcialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirNotaParcialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroNotaParcial = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroNFe = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroNFE.getRequiredFieldDescription(), maxlength: 10, required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
}

var NotaParcial = function () {
    this.ListaNotaParcial = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.ListaNotaFiscal = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(false) });

    this.ListaNotaParcial.val.subscribe(function () {
        recarregarGridNotaParcial();
    });

    this.ListaNotaFiscal.val.subscribe(function () {
        recarregarGridNotaFiscal();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNotaParcialModalClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.AdicionarNFE, visible: ko.observable(true) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "Pedido/ImportarDadosNota",
        UrlConfiguracao: "Pedido/ConfiguracaoImportacaoDadosNota",
        CodigoControleImportacao: EnumCodigoControleImportacao.O025_PedidosDadosNota,
        RetornarDadosPlanilha: true,
        ObterPrimeiraConfiguracao: true,
        CallbackDadosPlanilha: function (dados) {

            if (dados.length > 0)
                _notaParcial.ListaNotaParcial.val([]);

            for (var i = 0; i < dados.length; i++) {

                var nota = {
                    Codigo: guid(),
                    NumeroNFe: dados[i].Colunas[0].Valor
                }
                _notaParcial.ListaNotaParcial.val().push((nota));
            };

            recarregarGridNotaParcial();
        }
    });

}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridNotaParcial() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarNotaParcialClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroNFe", title: Localization.Resources.Pedidos.Pedido.NumeroNFE, width: "80%" }
    ];

    _gridNotaParcial = new BasicDataTable(_notaParcial.ListaNotaParcial.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridNotaParcial.CarregarGrid([]);


    loadGridNotaFiscal();
}

function loadGridNotaFiscal() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 0, dir: orderDir.asc };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "15%", className: "text-align-center" },
        { data: "Chave", title: Localization.Resources.Pedidos.Pedido.Chave, width: "30%" },
        { data: "Peso", title: Localization.Resources.Pedidos.Pedido.Peso, width: "20%", className: "text-align-right" },
        { data: "Valor", title: Localization.Resources.Gerais.Geral.Valor, width: "20%", className: "text-align-right" },
        { data: "DataEmissao", title: "Data Emissão", width: "20%", className: "text-align-center" }
    ];

    _gridNotasFiscais = new BasicDataTable(_notaParcial.ListaNotaFiscal.idGrid, header, null, ordenacao, null, linhasPorPaginas);
    _gridNotasFiscais.CarregarGrid([]);

}

function loadNotaParcial() {
    _notaParcial = new NotaParcial();
    KoBindings(_notaParcial, "knockoutNotaParcial");

    _cadastroNotaParcial = new CadastroNotaParcial();
    KoBindings(_cadastroNotaParcial, "knockoutCadastroNotaParcial");

    _CRUDcadastroNotaParcial = new CRUDCadastroNotaParcial();
    KoBindings(_CRUDcadastroNotaParcial, "knockoutCRUDCadastroNotaParcial");

    loadGridNotaParcial();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarNotaParcialClick() {
    if (ValidarCamposObrigatorios(_cadastroNotaParcial)) {
        if (isNotaParcialDuplicada())
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pedidos.Pedido.NFeInformadaJaFoiCadastrada);
        else {
            _notaParcial.ListaNotaParcial.val().push(obterCadastroNotaParcialSalvar());

            recarregarGridNotaParcial();
            fecharModalCadastroNotaParcial();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function adicionarNotaParcialModalClick() {
    _cadastroNotaParcial.Codigo.val(guid());

    controlarBotoesCadastroNotaParcialHabilitados(false);

    exibirModalCadastroNotaParcial();
}



function atualizarNotaParcialClick() {
    if (ValidarCamposObrigatorios(_cadastroNotaParcial)) {
        if (isNotaParcialDuplicada())
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pedidos.Pedido.NFeInformadaJaFoiCadastrada);
        else {
            var listaNotaParcial = obterListaNotaParcial();

            for (var i = 0; i < listaNotaParcial.length; i++) {
                if (_cadastroNotaParcial.Codigo.val() == listaNotaParcial[i].Codigo) {
                    listaNotaParcial.splice(i, 1, obterCadastroNotaParcialSalvar());
                    break;
                }
            }

            _notaParcial.ListaNotaParcial.val(listaNotaParcial);

            recarregarGridNotaParcial();
            fecharModalCadastroNotaParcial();
        }
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function editarNotaParcialClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroNotaParcial, { Data: registroSelecionado });

    controlarBotoesCadastroNotaParcialHabilitados(true);

    exibirModalCadastroNotaParcial();
}

function excluirNotaParcialClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaEcluirANFe, function () {
        removerNotaParcial(_cadastroNotaParcial.Codigo.val());
        fecharModalCadastroNotaParcial();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposNotaParcial() {
    preencherNotaParcial([]);
    preencherNotasFiscais([]);
}

function preencherNotaParcial(dadosNotaParcial) {
    _notaParcial.ListaNotaParcial.val(dadosNotaParcial);
}

function preencherNotasFiscais(dadosNotasFiscais) {
    _notaParcial.ListaNotaFiscal.val(dadosNotasFiscais);
    if (dadosNotasFiscais.length > 0) {
        _notaParcial.Adicionar.visible(false);
        _notaParcial.Importar.visible(false);
        _notaParcial.ListaNotaParcial.visible(false);
        _notaParcial.ListaNotaFiscal.visible(true);
    } else {
        _notaParcial.Adicionar.visible(true);
        _notaParcial.Importar.visible(true);
        _notaParcial.ListaNotaParcial.visible(true);
        _notaParcial.ListaNotaFiscal.visible(false);
    }
}

function preencherNotaParcialSalvar(pedido) {
    pedido["NotasParciais"] = obterListaNotaParcialSalvar();
}

function preencherNotaFiscaisSalvar(pedido) {
    pedido["NotasFiscais"] = obterListaNotaFiscalSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroNotaParcialHabilitados(isEdicao) {
    _CRUDcadastroNotaParcial.Adicionar.visible(!isEdicao);
    _CRUDcadastroNotaParcial.Atualizar.visible(isEdicao);
    _CRUDcadastroNotaParcial.Excluir.visible(isEdicao);
}

function exibirModalCadastroNotaParcial() {
    Global.abrirModal('divModalCadastroNotaParcial');
    $("#divModalCadastroNotaParcial").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroNotaParcial);
    });
}

function fecharModalCadastroNotaParcial() {
    Global.fecharModal('divModalCadastroNotaParcial');
}

function isNotaParcialDuplicada() {
    var listaNotaParcial = obterListaNotaParcial();

    for (var i = 0; i < listaNotaParcial.length; i++) {
        var notaParcial = listaNotaParcial[i];

        if ((_cadastroNotaParcial.Codigo.val() != notaParcial.Codigo) && (_cadastroNotaParcial.NumeroNFe.val() == notaParcial.NumeroNFe))
            return true;
    }

    return false;
}

function obterCadastroNotaParcialSalvar() {
    return {
        Codigo: _cadastroNotaParcial.Codigo.val(),
        NumeroNFe: _cadastroNotaParcial.NumeroNFe.val()
    };
}

function obterListaNotaParcial() {
    return _notaParcial.ListaNotaParcial.val().slice();
}

function obterListaNotaFiscal() {
    return _notaParcial.ListaNotaFiscal.val().slice();
}

function obterListaNotaParcialSalvar() {
    var listaNotaParcial = obterListaNotaParcial();

    return JSON.stringify(listaNotaParcial);
}

function obterListaNotaFiscalSalvar() {
    var listaNotaFiscal = obterListaNotaFiscal();

    return JSON.stringify(listaNotaFiscal);
}

function recarregarGridNotaParcial() {
    var listaNotaParcial = obterListaNotaParcial();

    _gridNotaParcial.CarregarGrid(listaNotaParcial);
}

function recarregarGridNotaFiscal() {
    var listaNotaFiscal = obterListaNotaFiscal();
    _gridNotasFiscais.CarregarGrid(listaNotaFiscal);
}

function removerNotaParcial(codigo) {
    var listaNotaParcial = obterListaNotaParcial();

    for (var i = 0; i < listaNotaParcial.length; i++) {
        if (codigo == listaNotaParcial[i].Codigo) {
            listaNotaParcial.splice(i, 1);
            break;
        }
    }

    _notaParcial.ListaNotaParcial.val(listaNotaParcial);
}