/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ManobraAcao.js" />
/// <reference path="../../Enumeradores/EnumTipoManobraAcao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroCentroCarregamentoManobraAcao;
var _CRUDcadastroCentroCarregamentoManobraAcao;
var _gridCentroCarregamentoManobraAcao;
var _centroCarregamentoManobraAcao;

/*
 * Declaração das Classes
 */

var CRUDCadastroCentroCarregamentoManobraAcao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarCentroCarregamentoManobraAcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCentroCarregamentoManobraAcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCentroCarregamentoManobraAcaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroCentroCarregamentoManobraAcao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ManobraAcao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobra.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TempoToleranciaInicioManobra = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.TempoDeToleranciaParaInicioDaManobraMinutos.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: true, thousands: "" }, required: true, val: ko.observable(0), def: 0 });
}

var CentroCarregamentoManobraAcao = function () {
    this.AcaoManobraPadraoInicioCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraPadraoParaInicioDeCarregamento.getFieldDescription(), idBtnSearch: guid() });
    this.AcaoManobraPadraoInicioReversa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraPadraoParaInicioDeReserva.getFieldDescription(), idBtnSearch: guid() });
    this.AcaoManobraPadraoFimCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraPadraoParaFimDeCarregamento.getFieldDescription(), idBtnSearch: guid() });
    this.AcaoManobraPadraoFimReversa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraPadraoParAFimDeReserva.getFieldDescription(), idBtnSearch: guid() });
    this.UtilizarControleManobra = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.CentroCarregamento.UtilizarControleDeManobras, def: false });
    this.ListaManobraAcao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.ListaManobraAcao.val.subscribe(function () {
        recarregarGridCentroCarregamentoManobraAcao();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCentroCarregamentoManobraAcaoModalClick, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarAcaoDeManobra });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCentroCarregamentoManobraAcao() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 2, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCentroCarregamentoManobraAcaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoManobraAcao", visible: false },
        { data: "DescricaoManobraAcao", title: Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobra, width: "50%", className: "text-align-left", orderable: false },
        { data: "TempoToleranciaInicioManobra", title: Localization.Resources.Logistica.CentroCarregamento.TempoDeTolerancia, width: "30%", className: "text-align-center", orderable: false }
    ];

    _gridCentroCarregamentoManobraAcao = new BasicDataTable(_centroCarregamentoManobraAcao.ListaManobraAcao.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridCentroCarregamentoManobraAcao.CarregarGrid([]);
}

function loadCentroCarregamentoManobraAcao() {
    _centroCarregamentoManobraAcao = new CentroCarregamentoManobraAcao();
    KoBindings(_centroCarregamentoManobraAcao, "knockoutCentroCarregamentoManobraAcao");

    _cadastroCentroCarregamentoManobraAcao = new CadastroCentroCarregamentoManobraAcao();
    KoBindings(_cadastroCentroCarregamentoManobraAcao, "knockoutCadastroCentroCarregamentoManobraAcao");

    _CRUDcadastroCentroCarregamentoManobraAcao = new CRUDCadastroCentroCarregamentoManobraAcao();
    KoBindings(_CRUDcadastroCentroCarregamentoManobraAcao, "knockoutCRUDCadastroCentroCarregamentoManobraAcao");

    new BuscarManobraAcao(_cadastroCentroCarregamentoManobraAcao.ManobraAcao);
    new BuscarManobraAcao(_centroCarregamentoManobraAcao.AcaoManobraPadraoInicioCarregamento, null, null, EnumTipoManobraAcao.InicioCarregamento);
    new BuscarManobraAcao(_centroCarregamentoManobraAcao.AcaoManobraPadraoInicioReversa);
    new BuscarManobraAcao(_centroCarregamentoManobraAcao.AcaoManobraPadraoFimCarregamento);
    new BuscarManobraAcao(_centroCarregamentoManobraAcao.AcaoManobraPadraoFimReversa);

    loadGridCentroCarregamentoManobraAcao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCentroCarregamentoManobraAcaoClick() {
    if (ValidarCamposObrigatorios(_cadastroCentroCarregamentoManobraAcao)) {
        if (!isAcaoManobraCadastrada()) {
            _centroCarregamentoManobraAcao.ListaManobraAcao.val().push(obterCadastroCentroCarregamentoManobraAcaoSalvar());

            recarregarGridCentroCarregamentoManobraAcao();
            fecharModalCadastroCentroCarregamentoManobraAcao();
        }
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraInformadaJaEstaCadastrada);
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function adicionarCentroCarregamentoManobraAcaoModalClick() {
    _cadastroCentroCarregamentoManobraAcao.Codigo.val(guid());

    controlarBotoesCadastroCentroCarregamentoManobraAcaoHabilitados(false);
    controlarCamposCadastroCentroCarregamentoManobraAcaoHabilitados(false);

    exibirModalCadastroCentroCarregamentoManobraAcao();
}

function atualizarCentroCarregamentoManobraAcaoClick() {
    if (ValidarCamposObrigatorios(_cadastroCentroCarregamentoManobraAcao)) {
        if (!isAcaoManobraCadastrada()) {
            var listaManobraAcao = obterListaManobraAcao();

            listaManobraAcao.forEach(function (manobraAcao, i) {
                if (_cadastroCentroCarregamentoManobraAcao.Codigo.val() == manobraAcao.Codigo) {
                    listaManobraAcao.splice(i, 1, obterCadastroCentroCarregamentoManobraAcaoSalvar());
                }
            });

            _centroCarregamentoManobraAcao.ListaManobraAcao.val(listaManobraAcao);

            recarregarGridCentroCarregamentoManobraAcao();
            fecharModalCadastroCentroCarregamentoManobraAcao();
        }
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.AcaoDeManobraInformadaJaEstaCadastrada);
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function editarCentroCarregamentoManobraAcaoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroCentroCarregamentoManobraAcao, { Data: registroSelecionado });

    _cadastroCentroCarregamentoManobraAcao.ManobraAcao.codEntity(registroSelecionado.CodigoManobraAcao);
    _cadastroCentroCarregamentoManobraAcao.ManobraAcao.val(registroSelecionado.DescricaoManobraAcao);
    _cadastroCentroCarregamentoManobraAcao.ManobraAcao.entityDescription(registroSelecionado.DescricaoManobraAcao);

    controlarBotoesCadastroCentroCarregamentoManobraAcaoHabilitados(true);
    controlarCamposCadastroCentroCarregamentoManobraAcaoHabilitados(true);

    exibirModalCadastroCentroCarregamentoManobraAcao();
}

function excluirCentroCarregamentoManobraAcaoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.CentroCarregamento.RealmenteDesejaExcluirAcaoDeManobra, function () {
        removerCentroCarregamentoManobraAcao(_cadastroCentroCarregamentoManobraAcao.Codigo.val());
        fecharModalCadastroCentroCarregamentoManobraAcao();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposCentroCarregamentoManobraAcao() {
    LimparCampos(_centroCarregamentoManobraAcao);

    _centroCarregamentoManobraAcao.ListaManobraAcao.val([]);
}

function preencherCentroCarregamentoManobraAcao(dadosManobraAcao) {
    PreencherObjetoKnout(_centroCarregamentoManobraAcao, { Data: dadosManobraAcao.Dados });

    _centroCarregamentoManobraAcao.ListaManobraAcao.val(dadosManobraAcao.AcoesManobra);
}

function preencherCentroCarregamentoManobraAcaoSalvar(centroCarregamento) {
    centroCarregamento["AcaoManobraPadraoInicioCarregamento"] = _centroCarregamentoManobraAcao.AcaoManobraPadraoInicioCarregamento.codEntity();
    centroCarregamento["AcaoManobraPadraoInicioReversa"] = _centroCarregamentoManobraAcao.AcaoManobraPadraoInicioReversa.codEntity();
    centroCarregamento["AcaoManobraPadraoFimCarregamento"] = _centroCarregamentoManobraAcao.AcaoManobraPadraoFimCarregamento.codEntity();
    centroCarregamento["AcaoManobraPadraoFimReversa"] = _centroCarregamentoManobraAcao.AcaoManobraPadraoFimReversa.codEntity();
    centroCarregamento["UtilizarControleManobra"] = _centroCarregamentoManobraAcao.UtilizarControleManobra.val();
    centroCarregamento["AcoesManobra"] = obterListaManobraAcaoSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroCentroCarregamentoManobraAcaoHabilitados(isEdicao) {
    _CRUDcadastroCentroCarregamentoManobraAcao.Adicionar.visible(!isEdicao);
    _CRUDcadastroCentroCarregamentoManobraAcao.Atualizar.visible(isEdicao);
    _CRUDcadastroCentroCarregamentoManobraAcao.Excluir.visible(isEdicao);
}

function controlarCamposCadastroCentroCarregamentoManobraAcaoHabilitados(isEdicao) {
    _cadastroCentroCarregamentoManobraAcao.ManobraAcao.enable(!isEdicao);
}

function exibirModalCadastroCentroCarregamentoManobraAcao() {
    Global.abrirModal('divModalCadastroCentroCarregamentoManobraAcao');
    $("#divModalCadastroCentroCarregamentoManobraAcao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroCentroCarregamentoManobraAcao);
    });
}

function fecharModalCadastroCentroCarregamentoManobraAcao() {
    Global.fecharModal('divModalCadastroCentroCarregamentoManobraAcao');
}

function obterCadastroCentroCarregamentoManobraAcaoSalvar() {
    return {
        Codigo: _cadastroCentroCarregamentoManobraAcao.Codigo.val(),
        CodigoManobraAcao: _cadastroCentroCarregamentoManobraAcao.ManobraAcao.codEntity(),
        DescricaoManobraAcao: _cadastroCentroCarregamentoManobraAcao.ManobraAcao.val(),
        TempoToleranciaInicioManobra: _cadastroCentroCarregamentoManobraAcao.TempoToleranciaInicioManobra.val()
    };
}

function obterListaManobraAcao() {
    return _centroCarregamentoManobraAcao.ListaManobraAcao.val().slice();
}

function obterListaManobraAcaoSalvar() {
    var listaManobraAcao = obterListaManobraAcao();

    return JSON.stringify(listaManobraAcao);
}

function recarregarGridCentroCarregamentoManobraAcao() {
    var listaManobraAcao = obterListaManobraAcao();

    _gridCentroCarregamentoManobraAcao.CarregarGrid(listaManobraAcao);
}

function removerCentroCarregamentoManobraAcao(codigo) {
    var listaManobraAcao = obterListaManobraAcao();

    listaManobraAcao.forEach(function (manobraAcao, i) {
        if (codigo == manobraAcao.Codigo)
            listaManobraAcao.splice(i, 1);
    });

    _centroCarregamentoManobraAcao.ListaManobraAcao.val(listaManobraAcao);
}

function isAcaoManobraCadastrada() {
    var listaManobraAcao = obterListaManobraAcao();
    var acaoCadastrada = false;

    listaManobraAcao.forEach(function (manobraAcao, i) {
        if ((_cadastroCentroCarregamentoManobraAcao.Codigo.val() != manobraAcao.Codigo) && (_cadastroCentroCarregamentoManobraAcao.ManobraAcao.codEntity() == manobraAcao.CodigoManobraAcao)) {
            acaoCadastrada = true;
            return false;
        }
    });

    return acaoCadastrada;
}