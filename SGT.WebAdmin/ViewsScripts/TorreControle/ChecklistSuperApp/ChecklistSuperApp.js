// #region Objetos Globais do Arquivo
var _pesquisaChecklistSuperApp;
var _gridChecklistSuperApp;
var _checklistSuperApp;
var _crudChecklistSuperApp;
var opcaoPadraoTipoEvidencia = [{ text: 'Selecione um Tipo de Evento', value: "" }];
// #endregion Objetos Globais do Arquivo

// #region Classes 
var PesquisaChecklistSuperApp = function () {
    this.Codigo = PropertyEntity({ text: "Código:", val: ko.observable(""), getType: typesKnockout.int });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridChecklistSuperApp, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.IdSuperApp = PropertyEntity({ text: "Id Super App:", maxlength: 24, visible: ko.observable(true) });
    this.Titulo = PropertyEntity({ text: "Título:", maxlength: 100, visible: ko.observable(true) });
    this.TipoFluxo = PropertyEntity({ options: EnumTipoFluxoChecklistSuperApp.obterOpcoes(), text: "Tipo de fluxo:", visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var ChecklistSuperApp = function () {

    // Tabela Checklist
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ val: ko.observable(""), text: "Título", getType: typesKnockout.string, required: ko.observable(true), visible: ko.observable(true), maxlength: 100 });
    this.IdSuperApp = PropertyEntity({ val: ko.observable(""), text: "Id Super App", getType: typesKnockout.string, required: ko.observable(false), visible: ko.observable(true), maxlength: 24, enable: ko.observable(false) });
    this.TipoFluxo = PropertyEntity({ options: EnumTipoFluxoChecklistSuperApp.obterOpcoes(), text: "Tipo de Fluxo", required: ko.observable(true), visible: ko.observable(true) });

    // Etapa
    this.GridEtapasChecklist = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), list: new Array() });
    this.CodigoEtapaChecklist = PropertyEntity({ val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.TipoEtapaChecklist = PropertyEntity({ text: "Tipo da Etapa", val: ko.observable(""), def: ko.observable(""), options: EnumTipoEtapaChecklistSuperApp.obterOpcoesCadastroChecklists(), visible: ko.observable(true) });
    this.TipoEvidencia = PropertyEntity({ text: "Tipo da Evidência", val: ko.observable(""), def: ko.observable(""), options: ko.observableArray(opcaoPadraoTipoEvidencia), visible: ko.observable(true) });
    this.TituloEtapaChecklist = PropertyEntity({ text: "Título", val: ko.observable(""), def: ko.observable(""), maxlength: 50, enable: ko.observable(true) });
    this.DescricaoEtapaChecklist = PropertyEntity({ text: "Descrição", val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.ObrigatorioEtapaChecklist = PropertyEntity({ text: "Obrigatório", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.OrdemEtapaChecklist = PropertyEntity({ text: "Ordem", getType: typesKnockout.int, val: ko.observable(), def: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: "" } });
    // Configurações
    this.HelperTextEtapaChecklist = PropertyEntity({ text: "Texto Auxiliar", val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.TipoAlertaEtapaChecklist = PropertyEntity({ text: "Tipo de Aviso", val: ko.observable(""), def: ko.observable(""), options: EnumTipoAlertaEtapaChecklistSuperApp.obterOpcoesCadastroChecklists(), visible: ko.observable(true) });
    this.TituloAlertaEtapaChecklist = PropertyEntity({ text: "Título do Aviso", val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.DescricaoAlertaEtapaChecklist = PropertyEntity({ text: "Descrição do Aviso", val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.PlaceHolderEtapaChecklist = PropertyEntity({ text: "Place Holder (Sugestão de preenchimento)", val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true) });
    this.ValorMinimoEtapaChecklist = PropertyEntity({ text: "Valor mínimo", getType: typesKnockout.string, val: ko.observable(), def: ko.observable("")});
    this.ValorMaximoEtapaChecklist = PropertyEntity({ text: "Valor Máximo", getType: typesKnockout.string, val: ko.observable(), def: ko.observable("")});
    this.QuantidadeMinimaEtapaChecklist = PropertyEntity({ text: "Quantidade mínima", getType: typesKnockout.string, val: ko.observable(), def: ko.observable(null)});
    this.QuantidadeMaximaEtapaChecklist = PropertyEntity({ text: "Quantidade máxima", getType: typesKnockout.string, val: ko.observable(), def: ko.observable(null)});
    this.GaleriaHabilitadaEtapaChecklist = PropertyEntity({ text: "Permitir galeria", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.PermitirPausarEtapaChecklist = PropertyEntity({ text: "Permitir pausar", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.TempoEsperaEtapaChecklist = PropertyEntity({ text: "Tempo de espera em segundos", getType: typesKnockout.int, val: ko.observable(), def: ko.observable(""), configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.TipoProcessamentoImagemEtapaChecklist = PropertyEntity({ text: "Tipo de processamento de imagem", val: ko.observable(""), def: ko.observable(""), options: EnumTipoProcessamentoImagemEtapaChecklistSuperApp.obterOpcoesCadastroChecklists(), visible: ko.observable(true) });
    this.ThresholdEtapaChecklist = PropertyEntity({ text: "Thresold", getType: typesKnockout.decimal, val: ko.observable(0.5), def: ko.observable(0.5), configDecimal: { precision: 2, allowZero: true, thousands: "" } });
    this.ModoValidacaoImagemEtapaChecklist = PropertyEntity({ text: "Modo", val: ko.observable(""), def: ko.observable(""), options: EnumModoValidacaoImagemEtapaChecklistSuperApp.obterOpcoesCadastroChecklists(), visible: ko.observable(true) });
    this.UtilizarMascaraImagemValidatorEtapaChecklist = PropertyEntity({ text: "Utilizar máscara fixa do canhoto na imagem", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false), enable: ko.observable(false) });
    this.LocalizacaoBloquearAvancoEtapa = PropertyEntity({ text: "Bloquear avanço fora do raio?", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.LocalizacaoPodeAvancarForaRaio = PropertyEntity({ text: "Motorista pode avançar fora do raio? Caso o bloqueio esteja habilitado, não permitirá o motorista avançar.", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.LocalizacaoObrigarImagemComprovacao = PropertyEntity({ text: "Obrigar o motorista enviar uma imagem estando fora do raio?", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.UsarDataAtualComoInicial = PropertyEntity({ text: "Usar a data e hora atual como padrão inicial", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.MetadadosImagemMostrarLogo = PropertyEntity({ text: "Exibir o logo da Trizy nas imagens.", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.MetadadosImagemMostrarData = PropertyEntity({ text: "Mostrar data nas imagens.", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.MetadadosImagemMostrarHora = PropertyEntity({ text: "Mostrar hora nas imagens.", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.MetadadosImagemMostrarLocalizacao = PropertyEntity({ text: "Mostrar latitude e longitude nas imagens.", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.MetadadosImagemMostrarNomeMotorista = PropertyEntity({ text: "Mostrar nome do motorista nas imagens.", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.UtilizarNumerosDecimais = PropertyEntity({ text: "Utilizar números decimais?", getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false) });
    this.ValidacaoAdicionalTexto = PropertyEntity({ text: "Validação Adicional", val: ko.observable(""), def: ko.observable(""), options: EnumValidacaoAdicionalEtapaTextoSuperApp.obterOpcoesCadastroChecklist(), visible: ko.observable(true) });

    this.AdicionarCadastroEtapaChecklist = PropertyEntity({ eventClick: adicionarListaCadastroEtapaChecklistClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarCadastroEtapaChecklist = PropertyEntity({ eventClick: atualizarListaCadastroEtapaChecklistClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.CancelarCadastroEtapaChecklist = PropertyEntity({ eventClick: limparCamposCadastroEtapaChecklistClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.TipoEtapaChecklist.val.subscribe(function (newValue) {
        if (newValue) {
            _checklistSuperApp.TipoEvidencia.options(EnumTipoEvidenciaSuperApp.obterOpcoesPorTipoEtapaChecklist(newValue));
            _checklistSuperApp.TipoEvidencia.val(_checklistSuperApp.TipoEvidencia.def());
        }
        else {
            _checklistSuperApp.TipoEvidencia.options(opcaoPadraoTipoEvidencia);
            _checklistSuperApp.TipoEvidencia.val(_checklistSuperApp.TipoEvidencia.def());
        }
    });
    this.ModoValidacaoImagemEtapaChecklist.val.subscribe(function (val) {
        LimparCampo(_checklistSuperApp.UtilizarMascaraImagemValidatorEtapaChecklist);
        _checklistSuperApp.UtilizarMascaraImagemValidatorEtapaChecklist.enable(val == EnumModoValidacaoImagemEtapaChecklistSuperApp.Camera);
        
    })
}

var CRUDChecklistSuperApp = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarChecklistSuperAppClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarChecklistSuperAppClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarChecklistSuperAppClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização
function loadChecklistSuperApp() {
    _pesquisaChecklistSuperApp = new PesquisaChecklistSuperApp();
    KoBindings(_pesquisaChecklistSuperApp, "knockoutPesquisaChecklistSuperApp", false, _pesquisaChecklistSuperApp.Pesquisar.id);

    _checklistSuperApp = new ChecklistSuperApp();
    KoBindings(_checklistSuperApp, "knockoutChecklistSuperApp");

    _crudChecklistSuperApp = new CRUDChecklistSuperApp();
    KoBindings(_crudChecklistSuperApp, "knockoutCRUDChecklistSuperApp");

    loadGridChecklistSuperApp();
    loadGridEtapasChecklist([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirChecklistSuperAppClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o registro " + registroSelecionado.Codigo + "?", function () {
        const dados = {
            Codigo: registroSelecionado.Codigo
        };

        executarReST("ChecklistSuperApp/ExcluirChecklistSuperApp", dados, function (arg) {

            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    limparCamposChecklistSuperApp();
                    recarregarGridChecklistSuperApp();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function adicionarChecklistSuperAppClick(e, sender) {

    if (!ValidarCamposObrigatorios(_checklistSuperApp)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var checklistSuperApp = obterChecklistSuperApp();
    executarReST("ChecklistSuperApp/Adicionar", checklistSuperApp, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                recarregarGridChecklistSuperApp();
                limparCamposChecklistSuperApp();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarChecklistSuperAppClick(e, sender) {
    if (!ValidarCamposObrigatorios(_checklistSuperApp)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var checklistSuperApp = obterChecklistSuperApp();
    executarReST("ChecklistSuperApp/Atualizar", checklistSuperApp, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                recarregarGridChecklistSuperApp();
                limparCamposChecklistSuperApp();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function editarChecklistSuperAppClick(registroSelecionado, duplicar) {
    limparCamposChecklistSuperApp();

    _crudChecklistSuperApp.Atualizar.visible(true);
    _crudChecklistSuperApp.Adicionar.visible(false);
    _crudChecklistSuperApp.Cancelar.visible(true);
    executarReST("ChecklistSuperApp/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo, Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaChecklistSuperApp.ExibirFiltros.visibleFade(false);
                PreencherObjetoKnout(_checklistSuperApp, retorno);
                preencherGridEtapasChecklist(retorno.Data.EtapasChecklist, duplicar);
                if (duplicar) {
                    _crudChecklistSuperApp.Atualizar.visible(false);
                    _crudChecklistSuperApp.Adicionar.visible(true);
                }

            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function cancelarChecklistSuperAppClick() {
    limparCamposChecklistSuperApp();

    _crudChecklistSuperApp.Cancelar.visible(false);
    _crudChecklistSuperApp.Atualizar.visible(false);
    _crudChecklistSuperApp.Adicionar.visible(true);
}
// #endregion Funções Associadas a Eventos

// #region Funções Privadas
function loadGridChecklistSuperApp() {

    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { excluirChecklistSuperAppClick(registroSelecionado); }, tamanho: "15", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { editarChecklistSuperAppClick(registroSelecionado, false); }, tamanho: "10", icone: "" };
    var duplicar = { descricao: "Duplicar", id: "clasEditar", evento: "onclick", metodo: function (registroSelecionado) { editarChecklistSuperAppClick(registroSelecionado, true); }, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [excluir, editar, duplicar] };
    _gridChecklistSuperApp = new GridView(_pesquisaChecklistSuperApp.Grid.idGrid, "ChecklistSuperApp/Pesquisa", _pesquisaChecklistSuperApp, menuOpcoes, null, 10, null, true, false, null, null, null, null);
    _gridChecklistSuperApp.CarregarGrid();
}
function recarregarGridChecklistSuperApp() {
    _gridChecklistSuperApp.CarregarGrid();
}

function obterChecklistSuperApp() {
    var checklistSuperApp = RetornarObjetoPesquisa(_checklistSuperApp);
    obterListaEtapasChecklistSalvar(checklistSuperApp);
    return checklistSuperApp;
}

function limparCamposChecklistSuperApp() {
    LimparCampos(_checklistSuperApp);
    limparCamposCadastroEtapaChecklist();
    _gridEtapasChecklist.CarregarGrid([]);
}
// #endregion Funções Privadas