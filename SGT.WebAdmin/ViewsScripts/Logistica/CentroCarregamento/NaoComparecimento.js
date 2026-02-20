/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumGatilhoAutomatizacaoNaoComparecimento.js" />

// #region Objetos Globais do Arquivo

var _cadastroAutomatizacaoNaoComparecimento;
var _CRUDCadastroAutomatizacaoNaoComparecimento;
var _gridAutomatizacaoNaoComparecimento;
var _centroCarregamentoNaoComparecimento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CRUDCadastroAutomatizacaoNaoComparecimento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarAutomatizacaoNaoComparecimentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarAutomatizacaoNaoComparecimentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirAutomatizacaoNaoComparecimentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CadastroAutomatizacaoNaoComparecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Gatilho = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Gatilho.getRequiredFieldDescription(), options: EnumGatilhoAutomatizacaoNaoComparecimento.obterOpcoes(), required: true });
    this.HorasTolerancia = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.HorasDeTolerancia.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "", maxlength: 3 }, required: true });
    this.BloquearCarga = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.BloquearCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailTransportador = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailParaTransportador, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.RetornarCargaParaExcedente = PropertyEntity({ text: "Retornar Carga para excedente", val: ko.observable(false), def: false, getType: typesKnockout.bool });
}

var CentroCarregamentoNaoComparecimento = function () {
    this.PermiteMarcarCargaComoNaoComparecimento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.PermiteMarcarCargasComoNoShowNaoComparecimento, val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ListaAutomatizacaoNaoComparecimento = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAutomatizacaoNaoComparecimentoModalClick, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarAutomatizacao, visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridAutomatizacaoNaoComparecimento() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarAutomatizacaoNaoComparecimentoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Gatilho", title: Localization.Resources.Logistica.CentroCarregamento.Gatilho, width: "30%", className: "text-align-center" },
        { data: "HorasTolerancia", title: Localization.Resources.Logistica.CentroCarregamento.HorasDeTolerancia, width: "30%", className: "text-align-center" },
        { data: "BloquearCarga", title: Localization.Resources.Logistica.CentroCarregamento.BloquearCarga, width: "20%", className: "text-align-center" },
        { data: "EnviarEmailTransportador", title: Localization.Resources.Logistica.CentroCarregamento.EnviarEmail, width: "20%", className: "text-align-center" },
        { data: "RetornarCargaParaExcedente", title: "Retornar Carga para excedente", width: "20%", className: "text-align-center" },
    ];

    _gridAutomatizacaoNaoComparecimento = new BasicDataTable(_centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridAutomatizacaoNaoComparecimento.CarregarGrid([]);
}

function loadAutomatizacaoNaoComparecimento() {
    _centroCarregamentoNaoComparecimento = new CentroCarregamentoNaoComparecimento();
    KoBindings(_centroCarregamentoNaoComparecimento, "knockoutCentroCarregamentoNaoComparecimento");

    _cadastroAutomatizacaoNaoComparecimento = new CadastroAutomatizacaoNaoComparecimento();
    KoBindings(_cadastroAutomatizacaoNaoComparecimento, "knockoutCadastroAutomatizacaoNaoComparecimento");

    _CRUDCadastroAutomatizacaoNaoComparecimento = new CRUDCadastroAutomatizacaoNaoComparecimento();
    KoBindings(_CRUDCadastroAutomatizacaoNaoComparecimento, "knockoutCRUDCadastroAutomatizacaoNaoComparecimento");

    loadGridAutomatizacaoNaoComparecimento();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarAutomatizacaoNaoComparecimentoClick() {
    var cadastroAutomatizacaoNaoComparecimentoSalvar = obterCadastroAutomatizacaoNaoComparecimentoSalvar()

    if (!cadastroAutomatizacaoNaoComparecimentoSalvar)
        return;

    _centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.val().push(cadastroAutomatizacaoNaoComparecimentoSalvar);

    recarregarGridAutomatizacaoNaoComparecimento();
    fecharModalCadastroAutomatizacaoNaoComparecimento();
}

function adicionarAutomatizacaoNaoComparecimentoModalClick() {
    _cadastroAutomatizacaoNaoComparecimento.Codigo.val(guid());

    controlarBotoesCadastroAutomatizacaoNaoComparecimentoHabilitados(false);
    exibirModalCadastroAutomatizacaoNaoComparecimento();
}

function atualizarAutomatizacaoNaoComparecimentoClick() {
    var cadastroAutomatizacaoNaoComparecimentoSalvar = obterCadastroAutomatizacaoNaoComparecimentoSalvar()

    if (!cadastroAutomatizacaoNaoComparecimentoSalvar)
        return;

    var listaAutomatizacaoNaoComparecimento = obterListaAutomatizacaoNaoComparecimento();

    for (var i = 0; i < listaAutomatizacaoNaoComparecimento.length; i++) {
        if (_cadastroAutomatizacaoNaoComparecimento.Codigo.val() == listaAutomatizacaoNaoComparecimento[i].Codigo) {
            listaAutomatizacaoNaoComparecimento.splice(i, 1, cadastroAutomatizacaoNaoComparecimentoSalvar);
            break;
        }
    }

    _centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.val(listaAutomatizacaoNaoComparecimento);

    recarregarGridAutomatizacaoNaoComparecimento();
    fecharModalCadastroAutomatizacaoNaoComparecimento();
}

function editarAutomatizacaoNaoComparecimentoClick(registroSelecionado) {
    var cadastroAutomatizacaoNaoComparecimento = obterCadastroAutomatizacaoNaoComparecimentoPorCodigo(registroSelecionado.Codigo);

    if (!cadastroAutomatizacaoNaoComparecimento)
        return;

    PreencherObjetoKnout(_cadastroAutomatizacaoNaoComparecimento, { Data: cadastroAutomatizacaoNaoComparecimento });
    controlarBotoesCadastroAutomatizacaoNaoComparecimentoHabilitados(true);
    exibirModalCadastroAutomatizacaoNaoComparecimento();
}

function excluirAutomatizacaoNaoComparecimentoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.CentroCarregamento.RealmenteDesejaExlcuirAutomatizacaoDeNoShow, function () {
        removerAutomatizacaoNaoComparecimento(_cadastroAutomatizacaoNaoComparecimento.Codigo.val());
        fecharModalCadastroAutomatizacaoNaoComparecimento();
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposCentroCarregamentoNaoComparecimento() {
    LimparCampos(_centroCarregamentoNaoComparecimento);

    _centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.val([]);

    recarregarGridAutomatizacaoNaoComparecimento();
}

function preencherCentroCarregamentoNaoComparecimento(dadosNaoComparecimento) {
    PreencherObjetoKnout(_centroCarregamentoNaoComparecimento, { Data: dadosNaoComparecimento.Dados });

    _centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.val(dadosNaoComparecimento.ListaAutomatizacaoNaoComparecimento);

    recarregarGridAutomatizacaoNaoComparecimento();
}

function preencherCentroCarregamentoNaoComparecimentoSalvar(centroCarregamento) {
    var listaAutomatizacaoNaoComparecimento = obterListaAutomatizacaoNaoComparecimento();

    centroCarregamento["PermiteMarcarCargaComoNaoComparecimento"] = _centroCarregamentoNaoComparecimento.PermiteMarcarCargaComoNaoComparecimento.val();
    centroCarregamento["ListaAutomatizacaoNaoComparecimento"] = JSON.stringify(listaAutomatizacaoNaoComparecimento);
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesCadastroAutomatizacaoNaoComparecimentoHabilitados(isEdicao) {
    _CRUDCadastroAutomatizacaoNaoComparecimento.Adicionar.visible(!isEdicao);
    _CRUDCadastroAutomatizacaoNaoComparecimento.Atualizar.visible(isEdicao);
    _CRUDCadastroAutomatizacaoNaoComparecimento.Excluir.visible(isEdicao);
}

function exibirModalCadastroAutomatizacaoNaoComparecimento() {
    Global.abrirModal('divModalCadastroAutomatizacaoNaoComparecimento');
    $("#divModalCadastroAutomatizacaoNaoComparecimento").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroAutomatizacaoNaoComparecimento);
    });
}

function fecharModalCadastroAutomatizacaoNaoComparecimento() {
    Global.fecharModal('divModalCadastroAutomatizacaoNaoComparecimento');
}

function obterCadastroAutomatizacaoNaoComparecimentoPorCodigo(codigo) {
    var listaAutomatizacaoNaoComparecimento = obterListaAutomatizacaoNaoComparecimento();

    for (var i = 0; i < listaAutomatizacaoNaoComparecimento.length; i++) {
        if (listaAutomatizacaoNaoComparecimento[i].Codigo == codigo)
            return listaAutomatizacaoNaoComparecimento[i];
    }

    return undefined;
}

function obterCadastroAutomatizacaoNaoComparecimentoSalvar() {
    if (!ValidarCamposObrigatorios(_cadastroAutomatizacaoNaoComparecimento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return undefined;
    }

    var acoesInformadas = _cadastroAutomatizacaoNaoComparecimento.BloquearCarga.val() || _cadastroAutomatizacaoNaoComparecimento.EnviarEmailTransportador.val();

    if (!acoesInformadas) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.CentroCarregamento.InformeUmaOuMaisAcoesSeremExecutadas);
        return undefined;
    }

    var listaAutomatizacaoNaoComparecimento = obterListaAutomatizacaoNaoComparecimento();

    for (var i = 0; i < listaAutomatizacaoNaoComparecimento.length; i++) {
        if (
            (listaAutomatizacaoNaoComparecimento[i].Codigo != _cadastroAutomatizacaoNaoComparecimento.Codigo.val()) &&
            (listaAutomatizacaoNaoComparecimento[i].Gatilho == _cadastroAutomatizacaoNaoComparecimento.Gatilho.val())
        ) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.GatilhoInformadoJaEstaCadastrado);
            return undefined;
        }
    }

    return {
        Codigo: _cadastroAutomatizacaoNaoComparecimento.Codigo.val(),
        Gatilho: _cadastroAutomatizacaoNaoComparecimento.Gatilho.val(),
        HorasTolerancia: _cadastroAutomatizacaoNaoComparecimento.HorasTolerancia.val(),
        BloquearCarga: _cadastroAutomatizacaoNaoComparecimento.BloquearCarga.val(),
        EnviarEmailTransportador: _cadastroAutomatizacaoNaoComparecimento.EnviarEmailTransportador.val(),
        RetornarCargaParaExcedente: _cadastroAutomatizacaoNaoComparecimento.RetornarCargaParaExcedente.val()
    };
}

function obterListaAutomatizacaoNaoComparecimento() {
    return _centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.val().slice();
}

function recarregarGridAutomatizacaoNaoComparecimento() {
    var listaAutomatizacaoNaoComparecimento = obterListaAutomatizacaoNaoComparecimento();
    var listaAutomatizacaoNaoComparecimentoGrid = [];

    for (var i = 0; i < listaAutomatizacaoNaoComparecimento.length; i++) {
        var automatizacaoNaoComparecimento = listaAutomatizacaoNaoComparecimento[i];

        listaAutomatizacaoNaoComparecimentoGrid.push({
            Codigo: automatizacaoNaoComparecimento.Codigo,
            Gatilho: EnumGatilhoAutomatizacaoNaoComparecimento.obterDescricao(automatizacaoNaoComparecimento.Gatilho),
            HorasTolerancia: automatizacaoNaoComparecimento.HorasTolerancia,
            BloquearCarga: (automatizacaoNaoComparecimento.BloquearCarga ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao),
            EnviarEmailTransportador: (automatizacaoNaoComparecimento.EnviarEmailTransportador ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao),
            RetornarCargaParaExcedente: (automatizacaoNaoComparecimento.RetornarCargaParaExcedente ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao)
        });
    }

    _gridAutomatizacaoNaoComparecimento.CarregarGrid(listaAutomatizacaoNaoComparecimentoGrid);
}

function removerAutomatizacaoNaoComparecimento(codigo) {
    var listaAutomatizacaoNaoComparecimento = obterListaAutomatizacaoNaoComparecimento();

    for (var i = 0; i < listaAutomatizacaoNaoComparecimento.length; i++) {
        if (codigo == listaAutomatizacaoNaoComparecimento[i].Codigo) {
            listaAutomatizacaoNaoComparecimento.splice(i, 1);
            break;
        }
    }

    _centroCarregamentoNaoComparecimento.ListaAutomatizacaoNaoComparecimento.val(listaAutomatizacaoNaoComparecimento);
    recarregarGridAutomatizacaoNaoComparecimento();
}

// #endregion Funções Privadas
