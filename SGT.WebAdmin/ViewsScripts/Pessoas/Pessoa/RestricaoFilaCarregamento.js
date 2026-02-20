/// <reference path="Pessoa.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridRestricaoFilaCarregamento;
var _listaRestricaoFilaCarregamento;
var _restricaoFilaCarregamento;

/*
 * Declaração das Classes
 */

var ListaRestricaoFilaCarregamento = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRestricaoFilaCarregamentoModalClick, type: types.event, text: ko.observable(Localization.Resources.Pessoas.Pessoa.AdicionarRestricao), visible: true });
}

var RestricaoFilaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoTomador.NaoInformado), options: EnumTipoTomador.obterOpcoesRestricaoFilaCarregamento(), def: EnumTipoTomador.NaoInformado, text: Localization.Resources.Pessoas.Pessoa.Tipo.getRequiredFieldDescription(), required: true });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.TipoDeCarga.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRestricaoFilaCarregamentoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRestricaoFilaCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRestricaoFilaCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRestricaoFilaCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadRestricaoFilaCarregamento() {
    _listaRestricaoFilaCarregamento = new ListaRestricaoFilaCarregamento();
    KoBindings(_listaRestricaoFilaCarregamento, "knockoutRestricaoFilaCarregamento");

    _restricaoFilaCarregamento = new RestricaoFilaCarregamento();
    KoBindings(_restricaoFilaCarregamento, "knockoutCadastroRestricaoFilaCarregamento");

    new BuscarTiposdeCarga(_restricaoFilaCarregamento.TipoCarga);

    loadGridRestricaoFilaCarregamento();
}

function loadGridRestricaoFilaCarregamento() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarRestricaoFilaCarregamentoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoCarga", visible: false },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: Localization.Resources.Pessoas.Pessoa.Tipo, width: "30%", className: 'text-align-center' },
        { data: "TipoCargaDescricao", title: Localization.Resources.Pessoas.Pessoa.TipoDeCarga, width: "50%" },
    ];

    _gridRestricaoFilaCarregamento = new BasicDataTable(_listaRestricaoFilaCarregamento.Grid.id, header, menuOpcoes);

    recarregarGridRestricaoFilaCarregamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarRestricaoFilaCarregamentoClick() {
    if (ValidarCamposObrigatorios(_restricaoFilaCarregamento)) {
        _restricaoFilaCarregamento.Codigo.val(guid());

        var restricoes = _gridRestricaoFilaCarregamento.BuscarRegistros();

        restricoes.push(obterRestricaoFilaCarregamentoSalvar());

        _gridRestricaoFilaCarregamento.CarregarGrid(restricoes);

        fecharRestricaoFilaCarregamentoModal();
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarRestricaoFilaCarregamentoModalClick() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    exibirRestricaoFilaCarregamentoModal();
}

function atualizarRestricaoFilaCarregamentoClick() {
    if (ValidarCamposObrigatorios(_restricaoFilaCarregamento)) {
        var restricoes = _gridRestricaoFilaCarregamento.BuscarRegistros();

        for (var i = 0; i < restricoes.length; i++) {
            if (_restricaoFilaCarregamento.Codigo.val() == restricoes[i].Codigo) {
                restricoes[i] = obterRestricaoFilaCarregamentoSalvar();
                break;
            }
        }

        _gridRestricaoFilaCarregamento.CarregarGrid(restricoes);

        fecharRestricaoFilaCarregamentoModal();
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarRestricaoFilaCarregamentoClick() {
    fecharRestricaoFilaCarregamentoModal();
}

function editarRestricaoFilaCarregamentoClick(registroSelecionado) {
    var isEdicao = true;

    preencherDadosRestricaoFilaCarregamento(registroSelecionado);
    controlarBotoesHabilitados(isEdicao);
    exibirRestricaoFilaCarregamentoModal();
}

function excluirRestricaoFilaCarregamentoClick() {
    var restricoes = _gridRestricaoFilaCarregamento.BuscarRegistros();

    for (var i = 0; i < restricoes.length; i++) {
        if (_restricaoFilaCarregamento.Codigo.val() == restricoes[i].Codigo) {
            restricoes.splice(i, 1);
            break;
        }
    }

    _gridRestricaoFilaCarregamento.CarregarGrid(restricoes);
    fecharRestricaoFilaCarregamentoModal();
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _restricaoFilaCarregamento.Adicionar.visible(!isEdicao);
    _restricaoFilaCarregamento.Atualizar.visible(isEdicao);
    _restricaoFilaCarregamento.Cancelar.visible(isEdicao);
    _restricaoFilaCarregamento.Excluir.visible(isEdicao);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function exibirRestricaoFilaCarregamentoModal() {
    Global.abrirModal('divModalCadastroRestricaoFilaCarregamento');
    $("#divModalCadastroRestricaoFilaCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_restricaoFilaCarregamento);
    });
}

function fecharRestricaoFilaCarregamentoModal() {
    Global.fecharModal('divModalCadastroRestricaoFilaCarregamento');
}

function obterRestricaoFilaCarregamentoSalvar() {
    return {
        Codigo: _restricaoFilaCarregamento.Codigo.val(),
        CodigoTipoCarga: _restricaoFilaCarregamento.TipoCarga.codEntity(),
        Tipo: _restricaoFilaCarregamento.Tipo.val(),
        TipoDescricao: EnumTipoTomador.obterDescricao(_restricaoFilaCarregamento.Tipo.val()),
        TipoCargaDescricao: _restricaoFilaCarregamento.TipoCarga.val()
    };
}

function obterRestricaoFilaCarregamentos() {
    return JSON.stringify(_gridRestricaoFilaCarregamento.BuscarRegistros());
}

function preencherDadosRestricaoFilaCarregamento(registroSelecionado) {
    _restricaoFilaCarregamento.Codigo.val(registroSelecionado.Codigo);
    _restricaoFilaCarregamento.Tipo.val(registroSelecionado.Tipo);
    _restricaoFilaCarregamento.TipoCarga.codEntity(registroSelecionado.CodigoTipoCarga);
    _restricaoFilaCarregamento.TipoCarga.val(registroSelecionado.TipoCargaDescricao);

}

function recarregarGridRestricaoFilaCarregamento() {
    _gridRestricaoFilaCarregamento.CarregarGrid(_pessoa.RestricoesFilaCarregamento.val() || []);
}