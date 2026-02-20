/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTravamentoChave.js" />
/// <reference path="../FluxoPatio/FluxoPatio.js" />
/// <reference path="../FluxoPatio/ObservacoesEtapas.js" />
/// <reference path="AssinaturaMotorista.js" />
/// <reference path="TravamentoChaveAnexo.js" />

// #region Objetos Globais do Arquivo

var _configuracaoGestaoPatioTravamentoChave;
var _travamentoChave;
var _pesquisaTravamentoChave;
var _gridTravamentoChave;
var _callbackTravamentoChaveAtualizado = null;

var situacaoTravamentoChave = [
    { text: "Todas", value: "" },
    { text: "Liberada", value: EnumSituacaoTravamentoChave.Liberada },
    { text: "Travada", value: EnumSituacaoTravamentoChave.Travada },
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var TravamentoChave = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: auditarTravamentoChaveClick });
    this.Etapa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), type: types.local });

    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    // Resumo
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.PreCarga = PropertyEntity({ text: "Pré Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string });
    this.Data = PropertyEntity({ text: "Data: ", getType: typesKnockout.string });
    this.Transportador = PropertyEntity({ text: "Transportador: ", getType: typesKnockout.string });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", getType: typesKnockout.string });
    this.Motorista = PropertyEntity({ text: "Motorista: ", getType: typesKnockout.string });
    this.Recebedores = PropertyEntity({ text: "Recebedor: ", getType: typesKnockout.string, visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ text: "Notas fiscais: ", getType: typesKnockout.string, visible: ko.observable(false) });
    this.ObservacaoFluxoPatio = PropertyEntity({ text: "Observação do fluxo de pátio: ", getType: typesKnockout.string, visible: ko.observable(false) });

    this.Recebedores.val.subscribe(function (novoValor) {
        if (novoValor == "")
            _travamentoChave.Recebedores.visible(false);
        else
            _travamentoChave.Recebedores.visible(true);
    });

    this.NotasFiscais.val.subscribe(function (novoValor) {
        if (novoValor == "")
            _travamentoChave.NotasFiscais.visible(false);
        else
            _travamentoChave.NotasFiscais.visible(true);
    });

    this.ObservacaoFluxoPatio.val.subscribe(function (novoValor) {
        if (novoValor == "")
            _travamentoChave.ObservacaoFluxoPatio.visible(false);
        else
            _travamentoChave.ObservacaoFluxoPatio.visible(true);
    });

    // PARA O FLUXO DE PATIO
    this.PrevisaoTravamento = PropertyEntity({ text: "Previsão Travamento: ", getType: typesKnockout.string, visible: ko.observable(false) });
    this.DataTravamento = PropertyEntity({ text: "Data Travamento: ", getType: typesKnockout.string, visible: ko.observable(false) });

    // CRUD
    this.Travado = PropertyEntity({ type: types.map, val: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTravamentoChaveClick, type: types.event, visible: ko.observable(true) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaTravamentoChaveClick, type: types.event, text: "Voltar Etapa", idGrid: guid(), visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_travamentoChave.ObservacaoFluxoPatio.val()); }, type: types.event, text: "Exibir Observação" });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaTravamentoChaveClick, type: types.event, text: "Observações da Etapa", visible: _configuracaoGestaoPatioTravamentoChave.HabilitarObservacaoEtapa });
    this.PaletesPBR = PropertyEntity({ text: "Paletes PBR:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(false), visible: ko.observable(true), required: ko.observable(false) });
    this.PaletesChep = PropertyEntity({ text: "Paletes Chep:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(false), visible: ko.observable(true), required: ko.observable(false) });
    this.AssinaturaMotorista = PropertyEntity({ type: types.event, text: "Assinatura do Motorista", eventClick: exibirModalAssinaturaMotorista, visible: ko.observable(false) });
    this.AbrirAtendimento = PropertyEntity({ type: types.event, text: "Abrir Atendimento", eventClick: abrirAtendimentoTravamentoChaveClick, visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, eventClick: abrirAnexosTravamentoChaveClick, visible: ko.observable(true) });
}

var PesquisaTravamentoChave = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(""), options: situacaoTravamentoChave, def: "" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTravamentoChave.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTravamentoChave(telaTravamentoChave, configuracaoGestaoPatio, callback) {
    $.get("Content/Static/GestaoPatio/FluxoPatioModais.html?dyn=" + guid(), function (modaispatio) {
        $("#ModaisPatio").html(modaispatio);
        $.get("Content/Static/GestaoPatio/TravamentoChave.html?dyn=" + guid(), function (data) {
            $("#divConteudoTravamentoChave").html(data);

            _configuracaoGestaoPatioTravamentoChave = configuracaoGestaoPatio;
            _callbackTravamentoChaveAtualizado = null;
            _pesquisaTravamentoChave = new PesquisaTravamentoChave();

            if (telaTravamentoChave)
                KoBindings(_pesquisaTravamentoChave, "knockoutPesquisaTravamentoChave", false, _pesquisaTravamentoChave.Pesquisar.id);

            _travamentoChave = new TravamentoChave();
            KoBindings(_travamentoChave, "knockoutTravamentoChave");

            loadLiberacaoChaveAssinaturaMotorista();
            loadTravamentoChaveAnexo();

            if (telaTravamentoChave)
                buscarTravamentoChave();

            if (callback instanceof Function)
                callback();
        });
    })
}

function loadTravamentoChavePorTela() {
    buscarConfiguracoesGestaoPatio(function (configuracaoGestaoPatio) {
        loadTravamentoChave(true, configuracaoGestaoPatio);
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function auditarTravamentoChaveClick() {
    var _fn = OpcaoAuditoria("TravamentoChave", "Codigo", _travamentoChave);

    _fn({ Codigo: _travamentoChave.Codigo.val() });
}

function atualizarTravamentoChaveClick(e) {

    if (!ValidarCamposObrigatorios(_travamentoChave))
        exibirMensagem("atencao", "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    Salvar(_travamentoChave, "TravamentoChave/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _pesquisaTravamentoChave.ExibirFiltros.visibleFade(true);
                limparCamposTravamentoChave();

                if (_callbackTravamentoChaveAtualizado != null)
                    _callbackTravamentoChaveAtualizado();
                else
                    _gridTravamentoChave.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function editarTravamentoChaveClick(itemGrid) {
    // Seta o codigo do objeto
    _travamentoChave.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_travamentoChave, "TravamentoChave/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                preecherRetornoTravaChave(arg);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function observacoesEtapaTravamentoChaveClick() {
    buscarObservacoesEtapa(_travamentoChave.Etapa.val());
}

function voltarEtapaTravamentoChaveClick() {
    exibirConfirmacao("Voltar Etapa", "Você tem certeza que deseja retornar à etapa anterior?", function () {
        executarReST("TravamentoChave/VoltarEtapa", { Codigo: _travamentoChave.Codigo.val(), Etapa: _travamentoChave.Etapa.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Etapa Retornada!");
                    _pesquisaTravamentoChave.ExibirFiltros.visibleFade(true);
                    limparCamposTravamentoChave();

                    if (_callbackTravamentoChaveAtualizado != null)
                        _callbackTravamentoChaveAtualizado();
                    else
                        _gridTravamentoChave.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preecherRetornoTravaChave(arg) {
    $("#container-travamento-chave").show();

    // Esconde pesqusia
    _pesquisaTravamentoChave.ExibirFiltros.visibleFade(false);
}

function limparCamposTravamentoChave() {
    $("#container-travamento-chave").hide();
    LimparCampos(_travamentoChave);
}

// #endregion Funções Públicas

// #region Funções Privadas

function buscarConfiguracoesGestaoPatio(callback) {
    executarReST("FluxoPatio/ConfiguracoesGestaoPatio", {}, function (retorno) {
        if (retorno.Success && Boolean(retorno.Data))
            callback(retorno.Data);
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function buscarTravamentoChave() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTravamentoChaveClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridTravamentoChave = new GridView(_pesquisaTravamentoChave.Pesquisar.idGrid, "TravamentoChave/Pesquisa", _pesquisaTravamentoChave, menuOpcoes, null);
    _gridTravamentoChave.CarregarGrid();
}

// #endregion Funções Privadas

function exibirModalAssinaturaMotorista() {
    exibirModalAssinaturaMotoristaEtapaLiberacaoChave();
}

function abrirAtendimentoTravamentoChaveClick() {
    var dadosCarga = { Carga: _travamentoChave.CodigoCarga };

    CriarNovoChamado(dadosCarga, "divModalChamadoOcorrencia");
}

function abrirAnexosTravamentoChaveClick() {
    buscarAnexosTravamentoChave();
    Global.abrirModal('divModalAnexosTravamentoChave');
    $("#divModalAnexosTravamentoChave").one('hidden.bs.modal', function () {
        _listaAnexosTravamentoChave.Anexos.val([]);
    });
}