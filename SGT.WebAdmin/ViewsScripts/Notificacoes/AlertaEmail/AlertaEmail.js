/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAlerta.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />
/// <reference path="../../Consultas/Irregularidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAlertaEmail;
var _alertaEmail;
var _setorAlertaEmail;
var _portfolioAlertaEmail;
var _irregularidadeAlertaEmail;
var _pesquisaAlertaEmail;
var _gridAlertaEmail;
var _gridAlertaEmailUsuario;
var _numeroRepeticoes = [];


//Preenche numeroRepeticoes
for (i = 1; i < 100; i++) {
    _numeroRepeticoes.push(new Object({ text: i.toString(), value: i }))
};

/*
 * Declaração das Classes
 */

var CRUDAlertaEmail = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false)});
}

var AlertaEmail = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", val: ko.observable(""), required: true, visible: true });
    this.DataHoraInicio = PropertyEntity({ text: "*Data/Hora Início: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: true });
    this.DataHoraFim = PropertyEntity({ text: "Data/Hora Fim: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.NumeroRepeticoes = PropertyEntity({ val: ko.observable(1), options: _numeroRepeticoes, def: 1, text: "*Repetir a cada: ", required: true});
    this.PeriodoNotificacoes = PropertyEntity({ val: ko.observable(EnumIntervaloAlertaEmail.Dia), options: EnumIntervaloAlertaEmail.obterOpcoes(), def: EnumIntervaloAlertaEmail.Dia, text: "*Período:", required: true});

    this.DataHoraInicio.dateRangeLimit = this.DataHoraFim;
    this.DataHoraFim.dateRangeInit = this.DataHoraInicio;

    this.ListaUsuario = PropertyEntity({ type: types.map, required: false, text: "Adicionar Usuário", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) }); 
}

var PesquisaAlertaEmail = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.DataHoraInicio = PropertyEntity({ text: "Data/Hora Início: ", getType: typesKnockout.date, val: ko.observable(""), def: ""});
    this.DataHoraFim = PropertyEntity({ text: "Data/Hora Fim: ", getType: typesKnockout.date, val: ko.observable(""), def: ""});

    this.Setor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Setores", idBtnSearch: guid() });
    this.Portfolio = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Portfólios", idBtnSearch: guid() });
    this.Irregularidade = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Irregularidades", idBtnSearch: guid() });
   

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridAlertaEmail, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAlertaEmail() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "AlertaEmail/ExportarPesquisa", titulo: "Configurações de Alerta" };

    _gridAlertaEmail = new GridViewExportacao(_pesquisaAlertaEmail.Pesquisar.idGrid, "AlertaEmail/Pesquisa", _pesquisaAlertaEmail, menuOpcoes, configuracoesExportacao);
    _gridAlertaEmail.CarregarGrid();
}

function loadGridAlertaEmailUsuario() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAlertaEmailUsuario, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridAlertaEmailUsuario = new BasicDataTable(_alertaEmail.ListaUsuario.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarFuncionario(_alertaEmail.ListaUsuario, null, _gridAlertaEmailUsuario);
    _alertaEmail.ListaUsuario.basicTable = _gridAlertaEmailUsuario;

    _gridAlertaEmailUsuario.CarregarGrid([]);
}

function loadAlertaEmail() {
    _alertaEmail = new AlertaEmail();
    KoBindings(_alertaEmail, "knockoutAlertaEmailDetalhes");

    _setorAlertaEmail = new Setor();
    KoBindings(_setorAlertaEmail, "knockoutAlertaEmailSetor");

    _portfolioAlertaEmail = new Portfolio();
    KoBindings(_portfolioAlertaEmail, "knockoutAlertaEmailPortfolio");

    _irregularidadeAlertaEmail = new Irregularidade();
    KoBindings(_irregularidadeAlertaEmail, "knockoutAlertaEmailIrregularidade");

    HeaderAuditoria("AlertaEmail", _alertaEmail);

    _CRUDAlertaEmail = new CRUDAlertaEmail();
    KoBindings(_CRUDAlertaEmail, "knockoutCRUDAlertaEmail");

    _pesquisaAlertaEmail = new PesquisaAlertaEmail();
    KoBindings(_pesquisaAlertaEmail, "knockoutPesquisaAlertaEmail", false, _pesquisaAlertaEmail.Pesquisar.id);

    new BuscarSetorFuncionario(_pesquisaAlertaEmail.Setor);
    new BuscarPortfolioModuloControle(_pesquisaAlertaEmail.Portfolio);
    new BuscarIrregularidades(_pesquisaAlertaEmail.Irregularidade);

    loadGridAlertaEmail();
    loadGridAlertaEmailUsuario();
    loadGridAlertaEmailSetor();
    loadGridAlertaEmailPortfolio();
    loadGridAlertaEmailIrregularidade();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (!validarCamposObrigatoriosAlertaEmail())
        return;

    executarReST("AlertaEmail/Adicionar", obterAlertaEmailSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridAlertaEmail();
                limparCamposAlertaEmail();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    if (!validarCamposObrigatoriosAlertaEmail())
        return;

    executarReST("AlertaEmail/Atualizar", obterAlertaEmailSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridAlertaEmail();
                limparCamposAlertaEmail();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposAlertaEmail();
}

function editarClick(registroSelecionado) {
    limparCamposAlertaEmail();

    _alertaEmail.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_alertaEmail, "AlertaEmail/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAlertaEmail.ExibirFiltros.visibleFade(false);
                _gridAlertaEmailUsuario.CarregarGrid(retorno.Data.ListaUsuario);
                _gridAlertaEmailSetor.CarregarGrid(retorno.Data.ListaSetor);
                _gridAlertaEmailPortfolio.CarregarGrid(retorno.Data.ListaPortfolio);
                _gridAlertaEmailIrregularidade.CarregarGrid(retorno.Data.ListaIrregularidade);
                controlarBotoesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_alertaEmail, "AlertaEmail/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridAlertaEmail();
                    limparCamposAlertaEmail();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }); 
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function tipoChange(tipo) {
    var exibirAlertarAposVencimento = true;
    var exibirAlertarTransportador = false;
    var exibirDiasAlertarAntesVencimento = true;
    var exibirDiasRepetirAlerta = true;

    _alertaEmail.CodigosRejeicoes.visible(false);
    _alertaEmail.CodigosRejeicoes.required(false);
    _alertaEmail.CodigosRejeicoes.val("");

    switch (tipo) {
        case EnumTipoAlertaEmail.Antt:
        case EnumTipoAlertaEmail.ApoliceSeguro:
        case EnumTipoAlertaEmail.CertificadoDigital:
        case EnumTipoAlertaEmail.Cnh:
        case EnumTipoAlertaEmail.PendenciaNfsManual:
            exibirAlertarTransportador = true;
            break;

        case EnumTipoAlertaEmail.PedidoSemTabelaFrete:
        case EnumTipoAlertaEmail.RotaNaoCadastrada:
            exibirAlertarAposVencimento = false;
            exibirDiasAlertarAntesVencimento = false;
            exibirDiasRepetirAlerta = false;
            break;

        case EnumTipoAlertaEmail.MDFEPendenteDeEncerramento:
            _alertaEmail.CodigosRejeicoes.visible(true);
            _alertaEmail.CodigosRejeicoes.required(true);
            exibirAlertarTransportador = true;
            break;
    }

    _alertaEmail.AlertarAposVencimento.visible(exibirAlertarAposVencimento);
    _alertaEmail.AlertarTransportador.visible(exibirAlertarTransportador);
    _alertaEmail.DiasAlertarAntesVencimento.visible(exibirDiasAlertarAntesVencimento);
    _alertaEmail.DiasRepetirAlerta.visible(exibirDiasRepetirAlerta);

    if (!exibirAlertarAposVencimento)
        _alertaEmail.AlertarAposVencimento.val(_alertaEmail.AlertarAposVencimento.def);

    if (!exibirAlertarTransportador)
        _alertaEmail.AlertarTransportador.val(_alertaEmail.AlertarTransportador.def);

    if (!exibirDiasAlertarAntesVencimento)
        _alertaEmail.DiasAlertarAntesVencimento.val(_alertaEmail.DiasAlertarAntesVencimento.def);

    if (!exibirDiasRepetirAlerta)
        _alertaEmail.DiasRepetirAlerta.val(_alertaEmail.DiasRepetirAlerta.def);
}  //Verificar se está sendo utilizada!!!!1

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _alertaEmail.Codigo.val() > 0;

    _CRUDAlertaEmail.Atualizar.visible(isEdicao);
    _CRUDAlertaEmail.Excluir.visible(isEdicao);
    _CRUDAlertaEmail.Adicionar.visible(!isEdicao);
}

function limparCamposAlertaEmail() {
    LimparCampos(_alertaEmail);
    LimparCampos(_setorAlertaEmail);
    LimparCampos(_portfolioAlertaEmail);
    LimparCampos(_irregularidadeAlertaEmail);

    _gridAlertaEmailUsuario.CarregarGrid([]);
    _gridAlertaEmailSetor.CarregarGrid([]);
    _gridAlertaEmailPortfolio.CarregarGrid([]);
    _gridAlertaEmailIrregularidade.CarregarGrid([]);

    controlarBotoesHabilitados();
}

function obterAlertaEmailSalvar() {
    var alertaEmail = RetornarObjetoPesquisa(_alertaEmail);

    alertaEmail["Usuarios"] = obterListaUsuarioSalvar();
    alertaEmail["Setores"] = obterListaSetorSalvar();
    alertaEmail["Portfolios"] = obterListaPortfolioSalvar();
    alertaEmail["Irregularidades"] = obterListaIrregularidadeSalvar();

    return alertaEmail;
}

function obterListaUsuario() {
    return _gridAlertaEmailUsuario.BuscarRegistros();
}

function obterListaUsuarioSalvar() {
    var listaUsuario = obterListaUsuario();
    var listaUsuarioRetornar = new Array();

    listaUsuario.forEach(function (usuario) {
        listaUsuarioRetornar.push(Number(usuario.Codigo));
    });

    return JSON.stringify(listaUsuarioRetornar);
}

function removerAlertaEmailUsuario(registroSelecionado) {
    var listaUsuario = obterListaUsuario();

    for (var i = 0; i < listaUsuario.length; i++) {
        if (registroSelecionado.Codigo == listaUsuario[i].Codigo) {
            listaUsuario.splice(i, 1);
            break;
        }
    }

    _gridAlertaEmailUsuario.CarregarGrid(listaUsuario);
}

function recarregarGridAlertaEmail() {
    _gridAlertaEmail.CarregarGrid();
}

function validarCamposObrigatoriosAlertaEmail() {
    if (!ValidarCamposObrigatorios(_alertaEmail)) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
        return false;
    }
    return true;
}
