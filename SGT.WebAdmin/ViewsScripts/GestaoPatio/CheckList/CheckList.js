/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumCheckListResposta.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckList.js" />
/// <reference path="../CheckListComponent/KoPergunta.js" />
/// <reference path="../../../Scripts/signature_pad.js" />
/// <reference path="CheckListAnexo.js" />

// #region Objetos Globais do Arquivo

var _checkList;
var _pesquisaCheckList;
var _gridCheckList;
var _PerguntasCheckList;
var _callbackCheckListAtualizado = null;
var _configuracaoGestaoPatioChecklist;

var canvasAssinaturaMotorista;
var canvasAssinaturaCarregador;
var canvasAssinaturaResponsavelAprovacao;
var signaturePadAssinaturaMotorista;
var signaturePadAssinaturaCarregador;
var signaturePadAssinaturaResponsavelAprovacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CheckList = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: abrirAuditoriaCheckList });
    this.ObservacoesGerais = PropertyEntity({ text: "Observações: ", val: ko.observable(''), def: '', enable: ko.observable(true) });
    this.GrupoPerguntas = PropertyEntity({ val: ko.observableArray([]), type: types.map, def: [], enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.EtapaAntecipada = PropertyEntity({ val: ko.observable(false), def: false });
    this.EdicaoRetroativa = PropertyEntity({ val: ko.observable(false), def: false });
    this.RetornosGR = ko.observableArray([]);
    this.EtapaCheckList = PropertyEntity({ text: "Etapa do Checklist:", val: ko.observable(EnumEtapaChecklist.Checklist), options: ko.observable(EnumEtapaChecklist.obterOpcoes()), def: EnumEtapaChecklist.Checklist });
    this.PreencherChecklist = PropertyEntity({ text: ko.observable("Preencher Checklist:") });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador: "), val: ko.observable("") });
    this.Motorista = PropertyEntity({ text: ko.observable("Motorista: "), val: ko.observable("") });
    this.Veiculo = PropertyEntity({ text: ko.observable("Veículo: "), val: ko.observable("") });
    this.Carga = PropertyEntity({ text: ko.observable("Carga: "), val: ko.observable("") });
    this.Destinatario = PropertyEntity({ text: ko.observable("Destinatário: "), val: ko.observable("") });
  
    //Configurações Sequencia Gestão Pátio
    this.CheckListNaoExigeObservacaoAoReprovar = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    //Assinaturas
    this.AssinaturaMotorista = PropertyEntity({ text: "Assinatura Motorista: ", visible: ko.observable(false) });
    this.AssinaturaCarregador = PropertyEntity({ text: "Assinatura Carregador: ", visible: ko.observable(false) });
    this.AssinaturaResponsavelAprovacao = PropertyEntity({ text: "Assinatura Responsável Aprovação: ", visible: ko.observable(false) });

    this.LimparAssinaturaMotorista = PropertyEntity({ text: "Limpar Assinatura", eventClick: limparAssinaturaMotoristaClick, type: types.event, enable: ko.observable(false) });
    this.LimparAssinaturaCarregador = PropertyEntity({ text: "Limpar Assinatura", eventClick: limparAssinaturaCarregadorClick, type: types.event, enable: ko.observable(false) });
    this.LimparAssinaturaResponsavelAprovacao = PropertyEntity({ text: "Limpar Assinatura", eventClick: limparAssinaturaResponsavelAprovacaoClick, type: types.event, enable: ko.observable(false) });

    //CRUD
    this.Reavaliar = PropertyEntity({ eventClick: reavaliarClick, type: types.event, text: "Reavaliar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true) });
    this.Anexos = PropertyEntity({ eventClick: anexosCheckListClick, type: types.event, text: "Anexos", visible: ko.observable(true) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaCheckListClick, type: types.event, text: "Observações", visible: _configuracaoGestaoPatioChecklist.HabilitarObservacaoEtapa });
    this.Salvar = PropertyEntity({ eventClick: salvarCheckListClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirChecklist, type: types.event, text: "Imprimir", visible: ko.observable(true) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaCheckListClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(false) });
};

var PesquisaCheckList = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Carga = PropertyEntity({ text: "Carga: ", getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(""), options: EnumSituacaoCheckList.obterOpcoesPesquisa(), def: "" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCheckList.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadCheckList(telaChecklist, configuracaoGestaoPatio, callback, etapaCheckList) {
    $.get("Content/Static/GestaoPatio/CheckList.html?dyn=" + guid(), function (data) {
        $.get("Content/Static/GestaoPatio/CheckListModal.html?dyn=" + guid(), function (dataModal) {
            $("#modaisChecklist").html(dataModal);
            $("#divConteudoCheckList").html(data);
            $("#botao-auditoria-checklist").bind("click", abrirAuditoriaCheckList);

            _configuracaoGestaoPatioChecklist = configuracaoGestaoPatio;
            _callbackCheckListAtualizado == null
            _pesquisaCheckList = new PesquisaCheckList();

            if (telaChecklist)
                KoBindings(_pesquisaCheckList, "knockoutPesquisaCheckList", false, _pesquisaCheckList.Pesquisar.id);

            _checkList = new CheckList();
            KoBindings(_checkList, "knockoutCheckList");

            _checkList.EtapaCheckList.val(etapaCheckList);

            loadComponenteChecklistPergunta();
            loadObservacaoPergunta();
            loadValidacao();
            loadResumoCheckList();
            loadCheckListAnexo();
            loadSignatureCheckList();

            if (telaChecklist)
                loadGridCheckList();

            if (callback instanceof Function)
                callback();
        });
    });
}

function loadCheckListPorTela() {
    buscarConfiguracoesGestaoPatio(function (configuracaoGestaoPatio) {
        loadCheckList(true, configuracaoGestaoPatio);
    });
}

function loadGridCheckList() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCheckListClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridCheckList = new GridView(_pesquisaCheckList.Pesquisar.idGrid, "CheckList/Pesquisa", _pesquisaCheckList, menuOpcoes, null);
    _gridCheckList.CarregarGrid();
}

function loadValidacao() {
    $("#knockoutCheckList")
        // Mudando a resposta
        .on('change', '.state-error select', function () {
            $(this).parent().removeClass("state-error");
        })
        // Ou clicando no botão de obs
        .on('click', 'input[type=button]', function () {
            $(this).parents(".pergunta-container").find('.state-error').removeClass("state-error");
        });
}

function loadSignatureCheckList() {
    canvasAssinaturaMotorista = document.getElementById(_checkList.AssinaturaMotorista.id);
    canvasAssinaturaCarregador = document.getElementById(_checkList.AssinaturaCarregador.id);
    canvasAssinaturaResponsavelAprovacao = document.getElementById(_checkList.AssinaturaResponsavelAprovacao.id);

    signaturePadAssinaturaMotorista = new SignaturePad(canvasAssinaturaMotorista, { backgroundColor: 'rgb(255, 255, 255)' });
    signaturePadAssinaturaCarregador = new SignaturePad(canvasAssinaturaCarregador, { backgroundColor: 'rgb(255, 255, 255)' });
    signaturePadAssinaturaResponsavelAprovacao = new SignaturePad(canvasAssinaturaResponsavelAprovacao, { backgroundColor: 'rgb(255, 255, 255)' });

    window.onresize = resizeCanvasCheckList;
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function auditarCheckListClick() {
    var _fn = OpcaoAuditoria("CheckListCarga", "Codigo", _checkList);

    _fn({ Codigo: _checkList.Codigo.val() });
}

function editarCheckListClick(itemGrid) {
    limparCamposCheckList();

    var dados = {
        Codigo: itemGrid.Codigo,
        EtapaCheckList: itemGrid.EtapaCheckList
    };

    executarReST("CheckList/BuscarPorCodigo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                PreencherObjetoKnout(_checkList, arg);
                preecherRetornoCheckLit(arg, false);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

    });
}

function anexosCheckListClick() {
    buscarAnexosCheckList();
    Global.abrirModal('divModalAnexosCheckList');
    $("#divModalAnexosCheckList").one('hidden.bs.modal', function () {
        _listaAnexoCheckList.Anexos.val([]);
    });
}

function finalizarClick() {
    if (!validarCheckList(true))
        return;

    if (!validarCheckListComponent(_checkList.GrupoPerguntas.val()))
        return;

    if (!validarAssinaturaCheckList())
        return;

    if (_configuracaoGestaoPatio.ChecklistPermiteAntecipar && _checkList.EtapaAntecipada.val())
        exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.AnteciparEtapa, Localization.Resources.GestaoPatio.FluxoPatio.AoConfirmarEtapaSeraAntecipadaPermanecendoSequenciaUltimaetapaConfirmada.format(Localization.Resources.GestaoPatio.FluxoPatio.Checklist), () => executarAtualizarChecklist());
    else
        executarAtualizarChecklist();
}

function executarAtualizarChecklist() {
    executarReST("CheckList/Atualizar", preencherDadosRetornoCheckList(), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso.");
                _pesquisaCheckList.ExibirFiltros.visibleFade(true);
                limparCamposCheckList();
                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
                else
                    _gridCheckList.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            if (_callbackCheckListAtualizado != null)
                _callbackCheckListAtualizado();
        }
    });
}

function imprimirChecklist(e, sender) {
    var dados = { Codigo: _checkList.Codigo.val() };

    executarDownload("CheckList/Imprimir", dados);
}

function observacoesEtapaCheckListClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.CheckList);
}

function salvarCheckListClick() {
    if (!validarCheckList(false))
        return;

    if (!validarAssinaturaCheckList())
        return;

    executarReST("CheckList/Salvar", preencherDadosRetornoCheckList(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _pesquisaCheckList.ExibirFiltros.visibleFade(true);
                limparCamposCheckList();

                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
                else
                    _gridCheckList.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);

                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

            if (_callbackCheckListAtualizado != null)
                _callbackCheckListAtualizado();
        }
    });
}

function voltarEtapaCheckListClick() {
    executarReST("CheckList/VoltarEtapa", { Codigo: _checkList.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa retornada com sucesso");
                _pesquisaCheckList.ExibirFiltros.visibleFade(true);
                limparCamposCheckList();

                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
                else
                    _gridCheckList.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);

                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

            if (_callbackCheckListAtualizado != null)
                _callbackCheckListAtualizado();
        }
    });
}

function reavaliarClick() {
    executarReST("CheckList/ReavaliarChecklist", { Codigo: _checkList.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Checklist aguardando reavaliação.");

                _pesquisaCheckList.ExibirFiltros.visibleFade(true);
                limparCamposCheckList();

                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
                else
                    _gridCheckList.CarregarGrid();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);

                if (_callbackCheckListAtualizado != null)
                    _callbackCheckListAtualizado();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

            if (_callbackCheckListAtualizado != null)
                _callbackCheckListAtualizado();
        }
    });
}

function limparAssinaturaMotoristaClick() {
    signaturePadAssinaturaMotorista.clear();
}

function limparAssinaturaCarregadorClick() {
    signaturePadAssinaturaCarregador.clear();
}

function limparAssinaturaResponsavelAprovacaoClick() {
    signaturePadAssinaturaResponsavelAprovacao.clear();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherDadosRetornoCheckList(){
    var dados = {
        Codigo: _checkList.Codigo.val(),
        EdicaoRetroativa: _checkList.EdicaoRetroativa.val(),
        GrupoPerguntas: JSON.stringify(_checkList.GrupoPerguntas.val()),
        ObservacoesGerais: _checkList.ObservacoesGerais.val(),
        AssinaturaMotorista: !signaturePadAssinaturaMotorista.isEmpty() ? canvasAssinaturaMotorista.toDataURL() : "",
        AssinaturaCarregador: !signaturePadAssinaturaCarregador.isEmpty() ? canvasAssinaturaCarregador.toDataURL() : "",
        AssinaturaResponsavelAprovacao: !signaturePadAssinaturaResponsavelAprovacao.isEmpty() ? canvasAssinaturaResponsavelAprovacao.toDataURL() : ""
    };
    return dados;
}

function atualizarObservacao(dados) {
    _PerguntasCheckList.map(function (grupo, iGrupo) {
        grupo.Perguntas.map(function (pergunta, iPergunta) {
            if (pergunta.Codigo == dados.Codigo) {
                _PerguntasCheckList[iGrupo].Perguntas[iPergunta].Observacao = dados.Observacao;
            }
        });
    });

    _checkList.GrupoPerguntas.val(_PerguntasCheckList);
}

function preecherRetornoCheckLit(arg, primeiraEtapa) {
    $(".check-list-form").show();

    var data = arg.Data;

    _checkList.RetornosGR(data.RetornosGR);

    _PerguntasCheckList = data.GrupoPerguntas;

    _pesquisaCheckList.ExibirFiltros.visibleFade(false);

    var habilitarEdicao = EnumSituacaoCheckList.isPermiteEdicao(_checkList.Situacao.val()) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS);

    let habilitarEdicaoRetroativa = habilitarEdicao || data.EdicaoRetroativa;

    _listaAnexoCheckList.Adicionar.visible(habilitarEdicao);

    _checkList.GrupoPerguntas.enable(habilitarEdicaoRetroativa);
    _checkList.ObservacoesGerais.enable(habilitarEdicaoRetroativa);

    _observacaoPergunta.Observacao.enable(habilitarEdicaoRetroativa);
    _observacaoPergunta.Adicionar.visible(habilitarEdicaoRetroativa);

    if (data.CheckListPermiteImpressaoApenasComCheckListFinalizada)
        _checkList.Imprimir.visible(!habilitarEdicao);
    else
        _checkList.Imprimir.visible(true);

    if (data.Reavaliada)
        $("#span-auditoria-checklist").show();
    else
        $("#span-auditoria-checklist").hide();

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteReavaliarChecklist, _PermissoesPersonalizadasFluxoPatio) && data.Situacao == EnumSituacaoCheckList.Rejeitado)
        _checkList.Reavaliar.visible(true);
    else
        _checkList.Reavaliar.visible(false);

    _checkList.Atualizar.visible(habilitarEdicao);
    _checkList.Salvar.visible(((_checkList.Situacao.val() == EnumSituacaoCheckList.Aberto) && (_configuracaoGestaoPatioChecklist.CheckListPermiteSalvarSemPreencher || data.CheckListPermiteSalvarSemPreencher)) || data.EdicaoRetroativa);

    if (_configuracaoGestaoPatioChecklist.CheckListPermiteVoltar)
        _checkList.VoltarEtapa.visible(!primeiraEtapa && habilitarEdicao);

    if (data.PossuiAssinatura) {
        $("#liAssinaturaCheckList").show();
        _checkList.AssinaturaMotorista.visible(data.CheckListAssinaturaMotorista);
        _checkList.AssinaturaCarregador.visible(data.CheckListAssinaturaCarregador);
        _checkList.AssinaturaResponsavelAprovacao.visible(data.CheckListAssinaturaResponsavelAprovacao);
        _checkList.LimparAssinaturaMotorista.enable(habilitarEdicaoRetroativa);
        _checkList.LimparAssinaturaCarregador.enable(habilitarEdicaoRetroativa);
        _checkList.LimparAssinaturaResponsavelAprovacao.enable(habilitarEdicaoRetroativa);
        habilitarDesabilitarEventHandlersSignaturePadCheckList(habilitarEdicaoRetroativa);
    }

    setTimeout(function (data) {
        if (data.PossuiAssinatura) {
            resizeCanvasCheckList();

            setTimeout(function (data) {
                if (!string.IsNullOrWhiteSpace(data.AssinaturaMotorista))
                    signaturePadAssinaturaMotorista.fromDataURL(data.AssinaturaMotorista);
                if (!string.IsNullOrWhiteSpace(data.AssinaturaCarregador))
                    signaturePadAssinaturaCarregador.fromDataURL(data.AssinaturaCarregador);
                if (!string.IsNullOrWhiteSpace(data.AssinaturaResponsavelAprovacao))
                    signaturePadAssinaturaResponsavelAprovacao.fromDataURL(data.AssinaturaResponsavelAprovacao);
            }, 500, data);
        }
    }, 1000, data);
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

function limparCamposCheckList() {
    LimparCampos(_checkList);
    _checkList.RetornosGR.removeAll();

    _checkList.AssinaturaMotorista.visible(false);
    _checkList.AssinaturaCarregador.visible(false);
    _checkList.AssinaturaResponsavelAprovacao.visible(false);
    limparSignaturePadCheckList();
    habilitarDesabilitarEventHandlersSignaturePadCheckList(true);

    $("#liAssinaturaCheckList").hide();
    $(".check-list-form").hide();
}

function validarCheckList(isFinalizar) {
    var valido_reprovado = true;
    var valido_semresposta = true;
    var valido_respostaimpeditiva = true;

    _PerguntasCheckList.map(function (grupo, iGrupo) {
        grupo.Perguntas.map(function (pergunta, iPergunta) {
            if (pergunta.Tipo == EnumTipoOpcaoCheckList.Aprovacao) {
                if (pergunta.Resposta == EnumCheckListResposta.Reprovada && pergunta.Observacao.length < 20) {
                    $("#pergunta-" + pergunta.Codigo + " .select").addClass("state-error");
                    valido_reprovado = false;
                }
                else if (pergunta.Resposta == EnumCheckListResposta.Aprovada) {
                    var perguntasImpeditivas = _PerguntasCheckList
                        .filter((elemento) => elemento.Perguntas.filter((pergunta) => pergunta.Tipo == EnumTipoOpcaoCheckList.SimNao || pergunta.Tipo == EnumTipoOpcaoCheckList.Opcoes || pergunta.Tipo == EnumTipoOpcaoCheckList.Selecoes).length > 0)
                        .map((elemento) => elemento.Perguntas).flat(1)
                        .filter((pergunta) => pergunta.RespostaImpeditiva != null || pergunta.Alternativas.filter((alternativa) => alternativa.OpcaoImpeditiva == true).length > 0);

                    for (var i = 0; i < perguntasImpeditivas.length; i++) {
                        if (perguntasImpeditivas[i].Tipo == EnumTipoOpcaoCheckList.SimNao) {
                            var respostaDaPergunta = perguntasImpeditivas[i].Resposta == "true" ? 1 : 2;

                            if (respostaDaPergunta == perguntasImpeditivas[i].RespostaImpeditiva) {
                                valido_respostaimpeditiva = false;
                                $("#sim_" + perguntasImpeditivas[i].id).parent().addClass("state-error");
                                $("#nao_" + perguntasImpeditivas[i].id).parent().addClass("state-error");
                            }
                        }
                        else {
                            if (perguntasImpeditivas[i].Alternativas.find((p) => p.OpcaoImpeditiva == true && p.Codigo == perguntasImpeditivas[i].Resposta) != undefined) {
                                valido_respostaimpeditiva = false;

                                if (perguntasImpeditivas[i].Tipo == EnumTipoOpcaoCheckList.Selecoes)
                                    $("#pergunta-" + perguntasImpeditivas[i].id + " .select").addClass("state-error");
                            }
                        }
                    }
                }
                else if (isFinalizar && (pergunta.Resposta == "")) {
                    $("#pergunta-" + pergunta.Codigo + " .select").addClass("state-error");
                    valido_semresposta = false;
                }
            }
        });
    });

    valido_reprovado = valido_reprovado || _checkList.CheckListNaoExigeObservacaoAoReprovar.val();

    if (!valido_reprovado)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar uma observação de no mínimo 20 caracteres para itens reprovados.");

    if (!valido_semresposta)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário responder todos os itens.");

    if (!valido_respostaimpeditiva)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível aprovar a checklist com a resposta atual nos itens destacados.");

    return valido_reprovado && valido_semresposta && valido_respostaimpeditiva;
}

function validarAssinaturaCheckList() {
    if (_checkList.AssinaturaMotorista.visible() && signaturePadAssinaturaMotorista.isEmpty()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário a assinatura do motorista.");
        return false;
    }

    if (_checkList.AssinaturaCarregador.visible() && signaturePadAssinaturaCarregador.isEmpty()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário a assinatura do carregador.");
        return false;
    }

    if (_checkList.AssinaturaResponsavelAprovacao.visible() && signaturePadAssinaturaResponsavelAprovacao.isEmpty()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário a assinatura do responsável pela aprovação.");
        return false;
    }

    return true;
}

function abrirAuditoriaCheckList() {
    executarReST("CheckList/BuscarAuditoria", { Codigo: _checkList.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                carregarModalAuditoriaCheckList(retorno.Data);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

function resizeCanvasCheckList() {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    var ratio = Math.max(window.devicePixelRatio || 1, 1);

    resizeCanvasCheckListElement(ratio, canvasAssinaturaMotorista);
    resizeCanvasCheckListElement(ratio, canvasAssinaturaCarregador);
    resizeCanvasCheckListElement(ratio, canvasAssinaturaResponsavelAprovacao);

    limparSignaturePadCheckList();
}

function resizeCanvasCheckListElement(ratio, canvas) {
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext("2d").scale(ratio, ratio);
}

function habilitarDesabilitarEventHandlersSignaturePadCheckList(habilitar) {
    if (habilitar) {
        signaturePadAssinaturaMotorista.on();
        signaturePadAssinaturaCarregador.on();
        signaturePadAssinaturaResponsavelAprovacao.on();
    } else {
        signaturePadAssinaturaMotorista.off();
        signaturePadAssinaturaCarregador.off();
        signaturePadAssinaturaResponsavelAprovacao.off();
    }
}

function limparSignaturePadCheckList() {
    signaturePadAssinaturaMotorista.clear();
    signaturePadAssinaturaCarregador.clear();
    signaturePadAssinaturaResponsavelAprovacao.clear();
}

// #endregion Funções Privadas
