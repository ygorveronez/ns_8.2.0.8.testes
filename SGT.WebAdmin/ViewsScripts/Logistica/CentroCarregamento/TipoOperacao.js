/// <reference path="CentroCarregamento.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../enumeradores/enumcentrocarregamentotipooperacaotipo.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridCentroCarregamentoTipoOperacao;
var _centroCarregamentoTipoOperacao;
var _crudCdTipoOperacao;

/*
 * Declaração das Classes
 */

var CentroCarregamentoTipoOperacao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ eventClick: adicionarTipoOperacaoModalClick, type: types.event, text: ko.observable(Localization.Resources.Logistica.CentroCarregamento.AdicionarTipoDeOperacao), visible: true });
}

var CrudCdTipoOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Tipo = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TipoSimulacao.getFieldDescription(), val: ko.observable(EnumCentroCarregamentoTipoOperacaoTipo.CapacMaiorVeiculo), options: EnumCentroCarregamentoTipoOperacaoTipo.obterOpcoes() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarTipoOperacaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirTipoOperacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

function loadCentroCarregamentoTipoOperacao() {
    _centroCarregamentoTipoOperacao = new CentroCarregamentoTipoOperacao();
    KoBindings(_centroCarregamentoTipoOperacao, "knockoutTipoOperacao");

    _crudCdTipoOperacao = new CrudCdTipoOperacao();
    KoBindings(_crudCdTipoOperacao, "knockoutCrudTipoOperacao");

    new BuscarTiposOperacao(_crudCdTipoOperacao.TipoOperacao);

    loadGridCentroCarregamentoTipoOperacao();
}

function loadGridCentroCarregamentoTipoOperacao() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarTipoOperacaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoOperacao", visible: false },
        { data: "TipoOperacao", title: Localization.Resources.Logistica.CentroCarregamento.TipoDeOperacao, width: "60%", className: 'text-align-left' },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: Localization.Resources.Gerais.Geral.Tipo, width: "35%" }
    ];

    _gridCentroCarregamentoTipoOperacao = new BasicDataTable(_centroCarregamentoTipoOperacao.Grid.id, header, menuOpcoes);
    _gridCentroCarregamentoTipoOperacao.CarregarGrid([]);
}

function adicionarTipoOperacaoClick() {
    if (ValidarCamposObrigatorios(_crudCdTipoOperacao)) {
        if (validarDadosDuplicadosTipoOperacao()) {
            _crudCdTipoOperacao.Codigo.val(guid());

            var registros = _gridCentroCarregamentoTipoOperacao.BuscarRegistros();

            registros.push(obterTipoOperacaoSalvar());

            _gridCentroCarregamentoTipoOperacao.CarregarGrid(registros);

            fecharTipoOperacaoModal();
        }
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarTipoOperacaoModalClick() {
    var isEdicao = false;
    controlarBotoesTipoOperacaoHabilitados(isEdicao);
    exibirTipoOperacaoModal();
}

function atualizarTipoOperacaoClick() {
    if (ValidarCamposObrigatorios(_crudCdTipoOperacao)) {
        if (validarDadosDuplicadosTipoOperacao()) {
            var registros = _gridCentroCarregamentoTipoOperacao.BuscarRegistros();
            for (var i = 0; i < registros.length; i++) {
                if (_crudCdTipoOperacao.TipoOperacao.codEntity() == registros[i].CodigoTipoOperacao) {
                    registros[i] = obterTipoOperacaoSalvar();
                    break;
                }
            }
            _gridCentroCarregamentoTipoOperacao.CarregarGrid(registros);
            fecharTipoOperacaoModal();
        }
    }
    else
        exibirMensagemCamposObrigatorio();
}

function editarTipoOperacaoClick(registroSelecionado) {
    var isEdicao = true;
    preencherDadosTipoOperacao(registroSelecionado);
    controlarBotoesTipoOperacaoHabilitados(isEdicao);
    exibirTipoOperacaoModal();
}

function excluirTipoOperacaoClick() {
    var registros = _gridCentroCarregamentoTipoOperacao.BuscarRegistros();
    for (var i = 0; i < registros.length; i++) {
        if (_crudCdTipoOperacao.Codigo.val() == registros[i].Codigo) {
            registros.splice(i, 1);
            break;
        }
    }
    _gridCentroCarregamentoTipoOperacao.CarregarGrid(registros);
    fecharTipoOperacaoModal();
}

/*
 * Declaração das Funções
 */

function limparCamposCentroCarregamentoTipoOperacao() {
    LimparCampos(_centroCarregamentoTipoOperacao);
    _gridCentroCarregamentoTipoOperacao.CarregarGrid([]);
}

function preencherCentroCarregamentoTipoOperacao(dados) {
    //console.log(dados);
    //PreencherObjetoKnout(_centroCarregamentoTipoOperacao, { Data: dados.Dados });
    _gridCentroCarregamentoTipoOperacao.CarregarGrid(dados);
}

function preencherCentroCarregamentoTipoOperacaoSalvar(centroCarregamento) {
    centroCarregamento["TipoOperacoes"] = obterTipoOperacoes();
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesTipoOperacaoHabilitados(isEdicao) {
    _crudCdTipoOperacao.Adicionar.visible(!isEdicao);
    _crudCdTipoOperacao.Atualizar.visible(isEdicao);
    _crudCdTipoOperacao.Excluir.visible(isEdicao);
}

function exibirTipoOperacaoModal() {
    Global.abrirModal('divModalCrudTipoOperacao');
    $("#divModalCrudTipoOperacao").one('hidden.bs.modal', function () {
        LimparCampos(_crudCdTipoOperacao);
    });
}

function fecharTipoOperacaoModal() {
    Global.fecharModal('divModalCrudTipoOperacao');
}

function obterTipoOperacoes() {
    return JSON.stringify(_gridCentroCarregamentoTipoOperacao.BuscarRegistros());
}

//{ data: "Codigo", visible: false },
//{ data: "CodigoTipoOperacao", visible: false },
//{ data: "TipoOperacao", title: "Tipo de Operação", width: "60%", className: 'text-align-left' },
//{ data: "Tipo", visible: false },
//{ data: "TipoDescricao", title: "Tipo", width: "35%" }

function obterTipoOperacaoSalvar() {
    return {
        Codigo: _crudCdTipoOperacao.Codigo.val(),
        CodigoTipoOperacao: _crudCdTipoOperacao.TipoOperacao.codEntity(),
        TipoOperacao: _crudCdTipoOperacao.TipoOperacao.val(),
        Tipo: _crudCdTipoOperacao.Tipo.val(),
        TipoDescricao: EnumCentroCarregamentoTipoOperacaoTipo.obterDescricao(_crudCdTipoOperacao.Tipo.val())
    };
}

function preencherDadosTipoOperacao(registroSelecionado) {
    _crudCdTipoOperacao.Codigo.val(registroSelecionado.Codigo);
    _crudCdTipoOperacao.TipoOperacao.codEntity(registroSelecionado.CodigoTipoOperacao);
    _crudCdTipoOperacao.TipoOperacao.val(registroSelecionado.TipoOperacao);
    _crudCdTipoOperacao.Tipo.val(registroSelecionado.Tipo);
}

function validarDadosDuplicadosTipoOperacao() {
    var registros = _gridCentroCarregamentoTipoOperacao.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (_crudCdTipoOperacao.Codigo.val() != registros[i].Codigo) {
            if (_crudCdTipoOperacao.TipoOperacao.codEntity() == registros[i].CodigoTipoOperacao) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.CentroCarregamento.CampoDuplicado, Localization.Resources.Logistica.CentroCarregamento.PorFavorInformeOutroTipoDeOperacao);
                return false;
            }
        }
    }

    return true;
}