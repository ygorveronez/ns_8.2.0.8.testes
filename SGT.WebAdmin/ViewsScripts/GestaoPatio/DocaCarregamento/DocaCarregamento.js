/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDocaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoAreaVeiculo.js" />

// #region Objetos Globais do Arquivo

var _configuracaoGestaoPatioDocaCarregamento;
var _docaCarregamento;
var _pesquisaDocaCarregamento;
var _gridDocaCarregamento;
var _callbackDocaCarregamentoAtualizado = null;

var situacaoDocaCarregamento = [
    { text: "Todas", value: EnumSituacaoDocaCarregamento.Todos },
    { text: "Ag. Informar Doca", value: EnumSituacaoDocaCarregamento.AgInformarDoca },
    { text: "Doca Informada", value: EnumSituacaoDocaCarregamento.Informada }
];

// #endregion Objetos Globais do Arquivo

// #region Classes

var DocaCarregamento = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ObservacaoFluxoPatio = PropertyEntity({ visible: false });
    this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: auditarDocaCarregamentoClick });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Resumo
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.PreCarga = PropertyEntity({ text: "Pré Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", getType: typesKnockout.string });
    this.Data = PropertyEntity({ text: "Data: ", getType: typesKnockout.string });
    this.Transportador = PropertyEntity({ text: "Transportador: ", getType: typesKnockout.string, visible: ko.observable(_configuracaoGestaoPatioDocaCarregamento.OcultarTransportador) });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", getType: typesKnockout.string });
    this.Motorista = PropertyEntity({ text: "Motorista: ", getType: typesKnockout.string });
    this.MotoristaTelefone = PropertyEntity({ text: "Telefone:", val: ko.observable(""), def: "" });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Centro de Carregamento:"), idBtnSearch: guid(), visible: false });
    this.NumeroDoca = PropertyEntity({ text: "Número da Doca: ", maxlength: 20, visible: ko.observable(!_configuracaoGestaoPatioDocaCarregamento.InformarDocaCarregamentoUtilizarLocalCarregamento), required: !_configuracaoGestaoPatioDocaCarregamento.InformarDocaCarregamentoUtilizarLocalCarregamento });
    this.LocalCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Local de Carregamento:"), idBtnSearch: guid(), required: _configuracaoGestaoPatioDocaCarregamento.InformarDocaCarregamentoUtilizarLocalCarregamento, visible: ko.observable(_configuracaoGestaoPatioDocaCarregamento.InformarDocaCarregamentoUtilizarLocalCarregamento) });
    this.PossuiLaudo = PropertyEntity({ text: "Possui laudo", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    // PARA O FLUXO DE PATIO
    this.DocaDetalhada = PropertyEntity({ type: types.local, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.NumeroCarga = PropertyEntity({ text: "Carga:", val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: "Pré Carga:", val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: "Data:", val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: "Hora:", val: ko.observable(""), def: "" });
    this.DescricaoSituacaoCarga = PropertyEntity({ text: "Situação:", val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: "Fornecedor:", val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga:", val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo da Operação:", val: ko.observable(""), def: "" });
    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: "Destinatário:", val: ko.observable(""), def: "" });
    this.PrevisaoCarregamento = PropertyEntity({ text: "Previsão Carregamento: ", getType: typesKnockout.string, visible: ko.observable(false) });
    this.DataCarregamento = PropertyEntity({ text: "Data Carregamento: ", getType: typesKnockout.string, visible: ko.observable(false) });
    this.NumeroDocaCarga = PropertyEntity({ text: "Número Doca Carga:", val: ko.observable(""), def: "", visible: ko.observable(false) });

    this.ExibirObservacao = PropertyEntity({ text: "Observação", visible: ko.observable(this.ObservacaoFluxoPatio) });
    this.IntegrarDoca = PropertyEntity({ eventClick: IntegrarDocaClick, type: types.event, text: "Enviar integração doca", visible: ko.observable(false) });
    this.ImprimirComprovanteCargaInformada = PropertyEntity({ eventClick: imprimirComprovanteCargaInformadaDocaClick, type: types.event, text: "Imprimir Comprovante de Carga", visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaDocaCarregamentoClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaDocaClick, type: types.event, text: "Rejeitar Etapa", idGrid: guid(), visible: ko.observable(false) });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoDocaClick, type: types.event, text: "Reabrir Fluxo", idGrid: guid(), visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaInformarDocaClick, type: types.event, text: "Observações", visible: _configuracaoGestaoPatioDocaCarregamento.HabilitarObservacaoEtapa });
    this.EtapaAntecipada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    // CRUD
    this.Atualizar = PropertyEntity({ eventClick: atualizarDocaCarregamentoClick, type: types.event, text: "Confirmar Doca", visible: ko.observable(true) });
    this.AtualizarNumeroDoca = PropertyEntity({ eventClick: atualizarDocaCarregamentoFluxoPatioClick, type: types.event, text: "Atualizar Número Doca", visible: ko.observable(false) });
};

var PesquisaDocaCarregamento = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.PreCarga = PropertyEntity({ text: "Pré Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoDocaCarregamento.AgInformarDoca), options: situacaoDocaCarregamento, def: EnumSituacaoDocaCarregamento.AgInformarDoca });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocaCarregamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadDocaCarregamento(telaDocaCarregamento, configuracaoGestaoPatio, callback) {
    return $.get("Content/Static/GestaoPatio/DocaCarregamento.html?dyn=" + guid(), function (data) {
        $("#divConteudoDocaCarregamento").html(data);

        _configuracaoGestaoPatioDocaCarregamento = configuracaoGestaoPatio;
        _callbackDocaCarregamentoAtualizado = null;
        _pesquisaDocaCarregamento = new PesquisaDocaCarregamento();

        _docaCarregamento = new DocaCarregamento();
        KoBindings(_docaCarregamento, "knockoutDocaCarregamento");

        if (telaDocaCarregamento) {
            KoBindings(_pesquisaDocaCarregamento, "knockoutPesquisaDocaCarregamento", false, _pesquisaDocaCarregamento.Pesquisar.id);
            buscarDocaCarregamento();
        } else {
            _docaCarregamento.AtualizarNumeroDoca.visible(true);
        }
        new BuscarAreaVeiculoPosicao(_docaCarregamento.LocalCarregamento, null, _docaCarregamento.CentroCarregamento, null, null, EnumTipoAreaVeiculo.Doca);

        _docaCarregamento.DataCarregamento.val.subscribe(definirDataHoraCargaDoca);

        if (callback instanceof Function)
            callback();
    });
}

function loadDocaCarregamentoPorTela() {
    buscarConfiguracoesGestaoPatio(function (configuracaoGestaoPatio) {
        loadDocaCarregamento(true, configuracaoGestaoPatio);
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarDocaCarregamentoClick(e) {
    if (_configuracaoGestaoPatioDocaCarregamento.InformarDocaPermiteAntecipar && _docaCarregamento.EtapaAntecipada.val())
        exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.AnteciparEtapa, Localization.Resources.GestaoPatio.FluxoPatio.AoConfirmarEtapaSeraAntecipadaPermanecendoSequenciaUltimaetapaConfirmada.format(Localization.Resources.GestaoPatio.FluxoPatio.Doca), () => callbackAtualizarDocaCarregamentoFluxoPatio());
    else
        callbackAtualizarDocaCarregamentoFluxoPatio()
}

function callbackAtualizarDocaCarregamentoFluxoPatio() {
    Salvar(_docaCarregamento, "DocaCarregamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _pesquisaDocaCarregamento.ExibirFiltros.visibleFade(true);
                limparCamposDocaCarregamento();
                if (_callbackDocaCarregamentoAtualizado != null)
                    _callbackDocaCarregamentoAtualizado();
                else
                    _gridDocaCarregamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function auditarDocaCarregamentoClick() {
    var _fn = OpcaoAuditoria("DocaCarregamento", "Codigo", _docaCarregamento);

    _fn({ Codigo: _docaCarregamento.Codigo.val() });
}

function editarDocaCarregamentoClick(itemGrid) {
    _docaCarregamento.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_docaCarregamento, "DocaCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                var possuiCarga = _docaCarregamento.Carga.val() != "";

                _docaCarregamento.NumeroDoca.visible(possuiCarga && !_configuracaoGestaoPatioDocaCarregamento.InformarDocaCarregamentoUtilizarLocalCarregamento);
                _docaCarregamento.NumeroDoca.required = _docaCarregamento.NumeroDoca.visible();
                _docaCarregamento.LocalCarregamento.visible(possuiCarga && _configuracaoGestaoPatioDocaCarregamento.InformarDocaCarregamentoUtilizarLocalCarregamento);
                _docaCarregamento.LocalCarregamento.required = _docaCarregamento.LocalCarregamento.visible();
                _docaCarregamento.PossuiLaudo.visible(retorno.Data.PermiteInformarDadosLaudo);

                preecherRetornoDocaCarregamento(retorno);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function imprimirComprovanteCargaInformadaDocaClick(e) {
    if ("imprimirComprovanteCargaInformada" in window)
        imprimirComprovanteCargaInformada(e.CodigoCarga.val());
}

function IntegrarDocaClick(e) {
    exibirConfirmacao("Integrar Doca", "Você tem certeza que deseja enviar integração da Doca?", function () {
        executarReST("DocaCarregamento/ReenviarIntegracaoDoca", { DocaCarregamento: _docaCarregamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Integração enviada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.falha, "Atenção!", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
            }
        });
    });
}

function observacoesEtapaInformarDocaClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.InformarDoca);
}

function reabrirFluxoDocaClick(e) {
    exibirConfirmacao("Reabrir Fluxo", "Você tem certeza que deseja reabrir o fluxo?", function () {
        executarReST("DocaCarregamento/ReabrirFluxo", { DocaCarregamento: _docaCarregamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Etapa Rejeitada!");

                    if (_callbackDocaCarregamentoReabrirFluxo != null)
                        _callbackDocaCarregamentoReabrirFluxo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
            }
        });
    });
}

function rejeitarEtapaDocaClick(e) {
    exibirConfirmacao("Rejeitar Etapa", "Você tem certeza que deseja rejeitar nessa etapa?", function () {
        executarReST("DocaCarregamento/RejeitarEtapa", { DocaCarregamento: _docaCarregamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Etapa Rejeitada!");

                    if (_callbackDocaCarregamentoRejeitarEtapa != null)
                        _callbackDocaCarregamentoRejeitarEtapa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
            }
        });
    });
}

function voltarEtapaDocaCarregamentoClick(e) {
    exibirConfirmacao("Voltar Etapa", "Você tem certeza que deseja retornar à etapa anterior?", function () {
        executarReST("DocaCarregamento/VoltarEtapa", { DocaCarregamento: _docaCarregamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Etapa Retornada!");

                    if (_callbackDocaCarregamentoVoltouEtapa != null)
                        _callbackDocaCarregamentoVoltouEtapa();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
            }
        });
    });
}

function atualizarDocaCarregamentoFluxoPatioClick(e) {
    Salvar(_docaCarregamento, "DocaCarregamento/AtualizarDocaCarregamentoFluxoPatio", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                Global.fecharModal('divModalDetalhesDocaCarregamento');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposDocaCarregamento() {
    $("#container-doca-carregamento").hide();
    LimparCampos(_docaCarregamento);
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

function buscarDocaCarregamento() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDocaCarregamentoClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridDocaCarregamento = new GridView(_pesquisaDocaCarregamento.Pesquisar.idGrid, "DocaCarregamento/Pesquisa", _pesquisaDocaCarregamento, menuOpcoes, null);
    _gridDocaCarregamento.CarregarGrid();
}

function definirDataHoraCargaDoca() {
    var dataHora = _docaCarregamento.DataCarregamento.val();
    var data = "";
    var hora = "";

    if (dataHora != null && dataHora != "") {
        var splittedTime = dataHora.split(" ");

        data = splittedTime[0] || "";
        hora = splittedTime[1] || "";
    }

    _docaCarregamento.CargaData.val(data);
    _docaCarregamento.CargaHora.val(hora);
}

function preecherRetornoDocaCarregamento(arg) {
    $("#container-doca-carregamento").show();

    // Esconde pesqusia
    _pesquisaDocaCarregamento.ExibirFiltros.visibleFade(false);
}

// #endregion Funções Privadas
