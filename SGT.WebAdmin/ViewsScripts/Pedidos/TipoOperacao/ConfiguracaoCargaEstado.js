/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="TipoOperacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _crudConfiguracaoCargaEstadoCadastro;
var _gridConfiguracaoCargaEstado;
var _configuracaoCargaEstado;
var _configuracaoCargaEstadoCadastro;

/*
 * Declaração das Classes
 */

var CRUDValorExigeIscaEstado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarValorExigeIscaEstadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarValorExigeIscaEstadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirValorExigeIscaEstadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
};


var ConfiguracaoCargaEstadoCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Estado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: Localization.Resources.Pedidos.TipoOperacao.Estado, idBtnSearch: guid(), required: true });
    this.ExigeInformarIscaNaCargaComValorMaiorQue = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.ExigeInformarIscaNaCargaComValorMaiorQue, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadConfiguracaoCargaEstado() {

    _configuracaoCargaEstadoCadastro = new ConfiguracaoCargaEstadoCadastro();
    KoBindings(_configuracaoCargaEstadoCadastro, "knockoutConfiguracaoCargaEstadoCadastro");

    _crudConfiguracaoCargaEstadoCadastro = new CRUDValorExigeIscaEstado();
    KoBindings(_crudConfiguracaoCargaEstadoCadastro, "knockoutCRUDValorExigeIscaEstado");

    new BuscarEstados(_configuracaoCargaEstadoCadastro.Estado);

    loadGridConfiguracaoCargaEstado();
}

function callbackModeloVeicular(dados) {
    _configuracaoCargaEstadoCadastro.ModeloVeicular.codEntity(dados.Codigo);
    _configuracaoCargaEstadoCadastro.ModeloVeicular.val(dados.Descricao);
    _configuracaoCargaEstadoCadastro.ModeloVeicular.codIntegracao = dados.CodigoIntegracao;
}

function loadGridConfiguracaoCargaEstado() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarValorExigeIscaEstadoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEstado", visible: false },
        { data: "DescricaoEstado", title: Localization.Resources.Pedidos.TipoOperacao.Estado, width: "70%" },
        { data: "ExigeInformarIscaNaCargaComValorMaiorQue", title: Localization.Resources.Pedidos.TipoOperacao.ExigeInformarIscaNaCargaComValorMaiorQue, width: "30%" }
    ];

    _gridConfiguracaoCargaEstado = new BasicDataTable(_tipoOperacao.ListaConfiguracaoCargaEstado.idGrid, header, menuOpcoes);
    _gridConfiguracaoCargaEstado.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarValorExigeIscaEstadoClick() {
    if (!validarValorExigeIscaEstado())
        return;

    if (!_tipoOperacao.ListaConfiguracaoCargaEstado.val())
        _tipoOperacao.ListaConfiguracaoCargaEstado.val([]);

    _tipoOperacao.ListaConfiguracaoCargaEstado.val().push(obterValorExigeIscaEstadoSalvar());

    recarregarGridValorExigeIscaEstado();
    fecharModalConfiguracaoCargaEstadoCadastro();
}

function pesquisarValorExigeIscaEstadoClick() {
    recarregarGridValorExigeIscaEstado();
}

function adicionarConfiguracaoCargaEstadoModalClick() {
    _configuracaoCargaEstadoCadastro.Codigo.val(guid());

    controlarBotoesConfiguracaoCargaEstadoCadastroHabilitados(false);
    exibirModalConfiguracaoCargaEstadoCadastro();
}

function atualizarValorExigeIscaEstadoClick() {
    if (!validarValorExigeIscaEstado())
        return;

    var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();

    for (var i = 0; i < ListaConfiguracaoCargaEstado.length; i++) {
        if (_configuracaoCargaEstadoCadastro.Codigo.val() == ListaConfiguracaoCargaEstado[i].Codigo) {
            ListaConfiguracaoCargaEstado.splice(i, 1, obterValorExigeIscaEstadoSalvar());
            break;
        }
    }

    _tipoOperacao.ListaConfiguracaoCargaEstado.val(ListaConfiguracaoCargaEstado)

    recarregarGridValorExigeIscaEstado();
    fecharModalConfiguracaoCargaEstadoCadastro();
}

function editarValorExigeIscaEstadoClick(registroSelecionado) {
    var ValorExigeIscaEstado = obterValorExigeIscaEstadoPorCodigo(registroSelecionado.Codigo);

    if (!ValorExigeIscaEstado)
        return;

    _configuracaoCargaEstadoCadastro.Codigo.val(ValorExigeIscaEstado.Codigo);
    _configuracaoCargaEstadoCadastro.Estado.codEntity(ValorExigeIscaEstado.CodigoEstado);
    _configuracaoCargaEstadoCadastro.Estado.val(ValorExigeIscaEstado.DescricaoEstado);
    _configuracaoCargaEstadoCadastro.ExigeInformarIscaNaCargaComValorMaiorQue.val(ValorExigeIscaEstado.ExigeInformarIscaNaCargaComValorMaiorQue);

    controlarBotoesConfiguracaoCargaEstadoCadastroHabilitados(true);
    exibirModalConfiguracaoCargaEstadoCadastro();
}

function excluirValorExigeIscaEstadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.TipoOperacao.RealmenteDesejaExcluirConfiguracaoCargaEstado, function () {
        var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();

        for (var i = 0; i < ListaConfiguracaoCargaEstado.length; i++) {
            if (_configuracaoCargaEstadoCadastro.Codigo.val() == ListaConfiguracaoCargaEstado[i].Codigo)
                ListaConfiguracaoCargaEstado.splice(i, 1);
        }

        _tipoOperacao.ListaConfiguracaoCargaEstado.val(ListaConfiguracaoCargaEstado);

        recarregarGridValorExigeIscaEstado();
        fecharModalConfiguracaoCargaEstadoCadastro();
    });
}

/*
 * Declaração das Funções Públicas
 */

function isTipoCargaPossuiValorExigeIscaEstadoVinculado(codigoTipoCarga) {
    var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();

    for (var i = 0; i < ListaConfiguracaoCargaEstado.length; i++) {
        if (ListaConfiguracaoCargaEstado[i].CodigoTipoCarga == codigoTipoCarga)
            return true;
    }

    return false;
}

function limparCamposValorExigeIscaEstado() {
    _tipoOperacao.ListaConfiguracaoCargaEstado.val([]);
    recarregarGridValorExigeIscaEstado();
}

function preencherConfiguracaoCargaEstado(dadosValorExigeIscaEstado) {
    _tipoOperacao.ListaConfiguracaoCargaEstado.val(dadosValorExigeIscaEstado);
    recarregarGridValorExigeIscaEstado();
}

function preencherConfiguracaoCargaEstadoSalvar(tipoOperacao) {
    tipoOperacao["ConfiguracoesCargaEstado"] = obterListaConfiguracaoCargaEstadoSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesConfiguracaoCargaEstadoCadastroHabilitados(isEdicao) {
    _crudConfiguracaoCargaEstadoCadastro.Atualizar.visible(isEdicao);
    _crudConfiguracaoCargaEstadoCadastro.Excluir.visible(isEdicao);
    _crudConfiguracaoCargaEstadoCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalConfiguracaoCargaEstadoCadastro() {
    Global.abrirModal('divModalConfiguracaoCargaEstadoCadastro');
    $("#divModalConfiguracaoCargaEstadoCadastro").one('hidden.bs.modal', function () {
        limparCamposConfiguracaoCargaEstadoCadastro();
    });
}

function fecharModalConfiguracaoCargaEstadoCadastro() {
    Global.fecharModal('divModalConfiguracaoCargaEstadoCadastro');
}

function limparCamposConfiguracaoCargaEstadoCadastro() {
    LimparCampos(_configuracaoCargaEstadoCadastro);
}


function obterValorExigeIscaEstadoPorCodigo(codigo) {
    var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();

    for (var i = 0; i < ListaConfiguracaoCargaEstado.length; i++) {
        var ValorExigeIscaEstado = ListaConfiguracaoCargaEstado[i];

        if (codigo == ValorExigeIscaEstado.Codigo)
            return ValorExigeIscaEstado;
    }

    return undefined;
}

function obterValorExigeIscaEstadoSalvar() {
    return {
        Codigo: _configuracaoCargaEstadoCadastro.Codigo.val(),
        CodigoEstado: _configuracaoCargaEstadoCadastro.Estado.codEntity(),
        DescricaoEstado: _configuracaoCargaEstadoCadastro.Estado.val(),
        ExigeInformarIscaNaCargaComValorMaiorQue: _configuracaoCargaEstadoCadastro.ExigeInformarIscaNaCargaComValorMaiorQue.val()
    };
}

function obterListaConfiguracaoCargaEstado() {
    if (_tipoOperacao.ListaConfiguracaoCargaEstado.val())
        return _tipoOperacao.ListaConfiguracaoCargaEstado.val().slice();
    else
        return [];
}

function obterListaConfiguracaoCargaEstadoSalvar() {
    var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();
    var ListaConfiguracaoCargaEstadoSalvar = new Array();

    for (var i = 0; i < ListaConfiguracaoCargaEstado.length; i++) {
        var ValorExigeIscaEstado = ListaConfiguracaoCargaEstado[i];

        ListaConfiguracaoCargaEstadoSalvar.push({
            Codigo: ValorExigeIscaEstado.Codigo,
            CodigoEstado: ValorExigeIscaEstado.CodigoEstado,
            DescricaoEstado: ValorExigeIscaEstado.DescricaoEstado,
            ExigeInformarIscaNaCargaComValorMaiorQue: ValorExigeIscaEstado.ExigeInformarIscaNaCargaComValorMaiorQue
        });
    }

    return JSON.stringify(ListaConfiguracaoCargaEstadoSalvar);
}

function recarregarGridValorExigeIscaEstado() {
    var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();
    _gridConfiguracaoCargaEstado.CarregarGrid(ListaConfiguracaoCargaEstado);
}

function validarValorExigeIscaEstado() {

    if (!ValidarCamposObrigatorios(_configuracaoCargaEstadoCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var ListaConfiguracaoCargaEstado = obterListaConfiguracaoCargaEstado();

    for (var i = 0; i < ListaConfiguracaoCargaEstado.length; i++) {
        var ValorExigeIscaEstado = ListaConfiguracaoCargaEstado[i];

        if (ValorExigeIscaEstado.CodigoEstado == _configuracaoCargaEstadoCadastro.Estado.codEntity() && ValorExigeIscaEstado.Codigo != _configuracaoCargaEstadoCadastro.Codigo.val()) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pedidos.TipoOperacao.JaFoiCadastradoUmaConfiguracaoCargaParaOEstadoSelecionado);
            return false;
        }
    }

    return true;
}
