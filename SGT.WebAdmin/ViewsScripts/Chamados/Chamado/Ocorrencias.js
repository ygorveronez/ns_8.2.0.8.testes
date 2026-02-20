/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Analise.js" />
/// <reference path="Chamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridOcorrenciaNoChamado;
var _pesquisaOcorrenciaNoChamado;
var _chamadoOcorrenciaModalOcorrencia;
var _rejeicaoOcorrenciaNoChamado;
var _delegarMultiplasOcorrenciasNoChamado;

var PesquisaOcorrenciaChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataInicio.getFieldDescription(), getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Carga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroOcorrencia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroOcorrencia.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroOcorrenciaCliente = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroOcorrenciaCliente.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroPedido.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroCTe.getFieldDescription(), val: ko.observable(0), def: 0 });
    this.SituacaoOcorrencia = PropertyEntity({ val: ko.observable(EnumSituacaoOcorrencia.Todas), options: EnumSituacaoOcorrencia.obterOpcoesPesquisa(), def: EnumSituacaoOcorrencia.Todas, text: Localization.Resources.Chamado.ChamadoOcorrencia.Situacao.getFieldDescription(), issue: 411 });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Ocorrencia.getFieldDescription(), idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Chamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.ChamadoOcorrencia.Atendimento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrenciaNoChamado.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.AdicionarOcorrenciaAtendimento = PropertyEntity({ eventClick: adicionarNovaOcorrenciaClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarOcorrencia, visible: ko.observable(true) });
    this.DelegarOcorrenciaAtendimento = PropertyEntity({ eventClick: delegarMultiplasOcorrenciasNoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.DelegarOcorrenciaAtendimento, visible: ko.observable(false) });
    this.AprovarOcorrenciaAtendimento = PropertyEntity({ eventClick: aprovarMultiplasOcorrenciasNoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AprovarOcorrenciaAtendimento, visible: ko.observable(false) });
    this.RejeitarOcorrenciaAtendimento = PropertyEntity({ eventClick: abrirModalRejeitarMultiplasOcorrenciasNoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.RejeitarOcorrenciaAtendimento, visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Gerais.Geral.SelecionarTodos, visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: finalizarChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Finalizar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Cancelar, visible: ko.observable(true) });
    this.Estornar = PropertyEntity({ eventClick: estornarChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Estornar, visible: ko.observable(false), enable: ko.observable(true) });

};

var RejeitarSelecionadosNoChamado = function () {
    this.Justificativa = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Justificativa.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Motivo.getFieldDescription(), maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarMultiplasOcorrenciasNoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Rejeitar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeitarOcorrenciasNoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Cancelar, visible: ko.observable(true) });
};

var DelegarMultiplasOcorrenciasNoChamado = function () {
    this.UsuarioDelegado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Responsavel, type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Observacao, maxlength: 5000, required: ko.observable(false), visible: ko.observable(false) });
    this.Delegar = PropertyEntity({ eventClick: delegarMultiplasOcorrenciasNoChamado, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Delegar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarDelegarOcorrenciasNoChamadoClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.Cancelar, visible: ko.observable(true) });
};

function loadOcorrenciaChamado() {
    _pesquisaOcorrenciaNoChamado = new PesquisaOcorrenciaChamado();
    KoBindings(_pesquisaOcorrenciaNoChamado, "knockoutOcorrencia", false, _pesquisaOcorrenciaNoChamado.Pesquisar.id);
    buscarOcorrenciasChamado();
    _pesquisaOcorrencia = new PesquisaOcorrencia();
    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia");

    _rejeicaoOcorrenciaNoChamado = new RejeitarSelecionadosNoChamado();
    KoBindings(_rejeicaoOcorrenciaNoChamado, "knockoutRejeicaoOcorrenciaNoChamado");

    _delegarMultiplasOcorrenciasNoChamado = new DelegarMultiplasOcorrenciasNoChamado();
    KoBindings(_delegarMultiplasOcorrenciasNoChamado, "knockoutDelegarMultiplasOcorrenciasNoChamado");

    BuscarMotivoRejeicaoOcorrencia(_rejeicaoOcorrenciaNoChamado.Justificativa, null, EnumAprovacaoRejeicao.Rejeicao);
    BuscarFuncionario(_delegarMultiplasOcorrenciasNoChamado.UsuarioDelegado);

    $('#divModalOcorrencia').on('hidden.bs.modal', function () {
        _pesquisaOcorrenciaNoChamado.Chamado.val(_abertura.Codigo.val());
        _pesquisaOcorrenciaNoChamado.Chamado.codEntity(_abertura.Codigo.val());

        _gridOcorrenciaNoChamado.CarregarGrid();
    });

    _chamadoOcorrenciaModalOcorrencia = new bootstrap.Modal(document.getElementById("divModalOcorrencia"), { backdrop: 'static' });
    _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(false);
    _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(false);
    _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(false);
}

function EtapaOcorrenciaChamadoClick() {
    _pesquisaOcorrenciaNoChamado.Fechar.visible(!_chamado.BloquearFinalizar.val());
    _pesquisaOcorrenciaNoChamado.Chamado.val(_abertura.Codigo.val());
    _pesquisaOcorrenciaNoChamado.Chamado.codEntity(_abertura.Codigo.val());
    _pesquisaOcorrenciaNoChamado.Estornar.visible(_CONFIGURACAO_TMS.PermitirEstornarAprovacaoChamadoLiberado || _motivoChamadoConfiguracao.PermiteEstornarAtendimento);

    _gridOcorrenciaNoChamado.CarregarGrid();
}

function buscarOcorrenciasChamado() {

    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaOcorrenciaNoChamado.SelecionarTodos,
        callbackNaoSelecionado: callbackNaoSelecionadoOcorrenciaNoAtendimento,
        callbackSelecionado: exibirMultiplasOpcoesOcorrenciaNoChamado,
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };


    var editar = { descricao: Localization.Resources.Chamado.ChamadoOcorrencia.Detalhes, id: guid(), evento: "onclick", metodo: editarOcorrenciaNoChamado, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };
    _gridOcorrenciaNoChamado = new GridView(_pesquisaOcorrenciaNoChamado.Pesquisar.idGrid, "Ocorrencia/Pesquisa", _pesquisaOcorrenciaNoChamado, menuOpcoes, null, null, null, null, null, multiplaescolha);
    _gridOcorrenciaNoChamado.SetPermitirEdicaoColunas(true);
    _gridOcorrenciaNoChamado.SetSalvarPreferenciasGrid(true);
}

function exibirMultiplasOpcoesOcorrenciaNoChamado(argumentoNulo, registroSelecionado) {
    let ocorrencias = _gridOcorrenciaNoChamado.ObterMultiplosSelecionados();

    if (!registroSelecionado && _pesquisaOcorrenciaNoChamado.SelecionarTodos.val() == false) {

        for (let ocorrencia of ocorrencias) {
            $("#" + ocorrencia.DT_RowId).click();
        }

        _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(false);
        _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(false);
        _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(false);

        return;
    }

    if (!registroSelecionado && _pesquisaOcorrenciaNoChamado.SelecionarTodos.val() == true) {

        let ocorrenciasQueNaoPodemSerSelecionadas = ocorrencias.filter(ocorrencia =>
            ocorrencia.SituacaoOcorrencia != EnumSituacaoOcorrencia.AgAprovacao &&
            ocorrencia.SituacaoOcorrencia != EnumSituacaoOcorrencia.AgAutorizacaoEmissao &&
            ocorrencia.SituacaoOcorrencia != EnumSituacaoOcorrencia.AutorizacaoPendente
        );

        if (ocorrenciasQueNaoPodemSerSelecionadas.length) {
            if (ocorrenciasQueNaoPodemSerSelecionadas.length === ocorrencias.length) {
                _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(false);
                _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(false);
                _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(false);
            } else {
                _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(true);
                _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(true);
                _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(true);

            }

            for (let i = 0; i < ocorrenciasQueNaoPodemSerSelecionadas.length; i++) {
                let ocorrencia = ocorrenciasQueNaoPodemSerSelecionadas[i];
                $("#" + ocorrencia.DT_RowId).click();
            }

            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.ASituacaoDaOcorrenciaNaoPermiteQueElaSejaSelecionada);
            return;
        } else {
            _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(true);
            _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(true);
            _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(true);

            return;

        }


    }

    if (registroSelecionado) {
        let situacaoPermiteSelecao = (registroSelecionado &&
            (registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgAprovacao ||
                registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgAutorizacaoEmissao ||
                registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AutorizacaoPendente));

        let situacaoPermiteSelecaoDelegar = (registroSelecionado &&
            (registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgAprovacao ||
                registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgAutorizacaoEmissao ||
                registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AutorizacaoPendente));

        if (!situacaoPermiteSelecao) {
            $("#" + registroSelecionado.DT_RowId).click();
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.ASituacaoDaOcorrenciaNaoPermiteQueElaSejaSelecionada);
            return;
        }

        _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(true);
        _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(true);
        _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(true);

        if (!situacaoPermiteSelecaoDelegar)
            _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(false);

        _pesquisaOcorrenciaNoChamado.SelecionarTodos.val(false);
    }
}

function callbackNaoSelecionadoOcorrenciaNoAtendimento(argumentoNulo, registroSelecionado) {
    var situacaoPermiteSelecao = (registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgAprovacao || registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgAutorizacaoEmissao || registroSelecionado.SituacaoOcorrencia == EnumSituacaoOcorrencia.AutorizacaoPendente);
    let ocorrencias = _gridOcorrenciaNoChamado.ObterMultiplosSelecionados();

    if (situacaoPermiteSelecao && ocorrencias.length == 0) {
        _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(false);
        _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(false);
        _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(false);
        _pesquisaOcorrenciaNoChamado.SelecionarTodos.val(false);
    }
}

function aprovarMultiplasOcorrenciasNoChamadoClick(e, sender) {
    let dados = {
        MultiplasOcorrenciasSelecionadas: JSON.stringify(_gridOcorrenciaNoChamado.ObterMultiplosSelecionados())
    };

    exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.RealmenteDesejaAprovarAsOcorrenciasSelecionadas, function () {
        executarReST("ChamadoOcorrencia/AprovarMultiplasOcorrencias", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (arg.Data.RegrasModificadas > 1)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.AlcadasOcorrenciasForamAprovadas.format(arg.Data.RegrasModificadas));
                    else if (arg.Data.RegrasModificadas == 1)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.AlcadaOcorrenciaFoiAprovada.format(arg.Data.RegrasModificadas));
                    else if (arg.Data.RegrasExigemMotivo == 0)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Chamado.ChamadoOcorrencia.NenhumaAlcadaPendenteParaSeuUsuario);
                    limparGridEsconderBotoesOcorrenciasNoChamado();

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function rejeitarMultiplasOcorrenciasNoChamadoClick(e, sender) {
    let dados = {
        MultiplasOcorrenciasSelecionadas: JSON.stringify(_gridOcorrenciaNoChamado.ObterMultiplosSelecionados()),
        Justificativa: _rejeicaoOcorrenciaNoChamado.Justificativa.codEntity(),
        Motivo: _rejeicaoOcorrenciaNoChamado.Motivo.val()
    };
    exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.RealmenteDesejaRejeitarAsOcorrenciasSelecionadas, function () {
        executarReST("ChamadoOcorrencia/ReprovarMultiplasOcorrencias", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal('divModalRejeitarMultiplasOcorrenciasNoChamado');
                    limparGridEsconderBotoesOcorrenciasNoChamado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}


function abrirModalRejeitarMultiplasOcorrenciasNoChamadoClick() {
    Global.abrirModal('divModalRejeitarMultiplasOcorrenciasNoChamado');
}

function delegarMultiplasOcorrenciasNoChamadoClick() {
    Global.abrirModal('divModalDelegarOcorrenciaNoChamado');
}

function limparGridEsconderBotoesOcorrenciasNoChamado() {
    _pesquisaOcorrenciaNoChamado.DelegarOcorrenciaAtendimento.visible(false);
    _pesquisaOcorrenciaNoChamado.AprovarOcorrenciaAtendimento.visible(false);
    _pesquisaOcorrenciaNoChamado.RejeitarOcorrenciaAtendimento.visible(false);
    _gridOcorrenciaNoChamado.CarregarGrid();
    _gridOcorrenciaNoChamado.AtualizarRegistrosSelecionados([]);
    _gridOcorrenciaNoChamado.AtualizarRegistrosNaoSelecionados([]);
    _pesquisaOcorrenciaNoChamado.SelecionarTodos.val(false);
}

function delegarMultiplasOcorrenciasNoChamado() {
    if (!ValidarCamposObrigatorios(_delegarMultiplasOcorrenciasNoChamado)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.VoceRealmenteDesejaDelegarTodasOcorrencias, function () {

        let dados = {
            UsuarioDelegado: _delegarMultiplasOcorrenciasNoChamado.UsuarioDelegado.codEntity(),
            Observacao: _delegarMultiplasOcorrenciasNoChamado.Observacao.val(),
            OcorrenciasSelecionadas: JSON.stringify(_gridOcorrenciaNoChamado.ObterMultiplosSelecionados()),
            OcorrenciasNaoSelecionadas: JSON.stringify(_gridOcorrenciaNoChamado.ObterMultiplosNaoSelecionados())

        };

        executarReST("AutorizacaoOcorrencia/DelegarMultiplasOcorrencias", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);
                    cancelarDelegarOcorrenciasNoChamadoClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function cancelarDelegarOcorrenciasNoChamadoClick() {
    LimparCampos(_delegarMultiplasOcorrenciasNoChamado);
    Global.fecharModal("divModalDelegarOcorrenciaNoChamado");
}

function cancelarRejeitarOcorrenciasNoChamadoClick() {
    LimparCampos(_rejeicaoOcorrenciaNoChamado);
    Global.fecharModal("divModalRejeitarMultiplasOcorrenciasNoChamado");
}

function adicionarNovaOcorrenciaClick() {
    limparCamposOcorrencia();
    var data = { Codigo: _abertura.Carga.codEntity(), CodigoCargaEmbarcador: _abertura.Carga.val() };
    executarReST("ChamadoOcorrencia/ObterTipoOcorrencia", { MotivoChamado: _abertura.MotivoChamado.codEntity(), Chamado: _abertura.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                limparCamposOcorrencia();

                if (r.Data.Codigo > 0) {
                    _ocorrencia.TipoOcorrencia.codEntity(r.Data.Codigo);
                    _ocorrencia.TipoOcorrencia.val(r.Data.Descricao);
                    _ocorrencia.TipoOcorrencia.origemOcorrencia = r.Data.OrigemOcorrencia;

                    if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                        _ocorrencia.TipoOcorrencia.enable(false);
                    else
                        _ocorrencia.TipoOcorrencia.enable(true);
                }

                if (r.Data.ComponenteCodigo > 0) {
                    _ocorrencia.ComponenteFrete.codEntity(r.Data.ComponenteCodigo);
                    _ocorrencia.ComponenteFrete.val(r.Data.ComponenteDescricao);
                    if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                        _ocorrencia.ComponenteFrete.enable(false);
                    else
                        _ocorrencia.ComponenteFrete.enable(true);
                }

                if (r.Data.CodigoCarga > 0) {

                    _abertura.Carga.codEntity(r.Data.CodigoCarga);
                    _abertura.Carga.val(r.Data.CodigoCargaEmbarcador);

                    data = { Codigo: r.Data.CodigoCarga, CodigoCargaEmbarcador: r.Data.CodigoCargaEmbarcador };
                }

                _ocorrencia.Chamado.val(_resumoChamado.Numero.val());
                _ocorrencia.Chamado.codEntity(_abertura.Codigo.val());
                _ocorrencia.Chamado.visible(true);
                _ocorrencia.Chamado.enable(false);
                _ocorrencia.NaoLimparCarga.val(true);
                _ocorrencia.Carga.enable(false);

                retornoCarga(data, function () {
                    _chamadoOcorrenciaModalOcorrencia.show();
                    visiblidadeValorOcorrencia();

                    if (!string.IsNullOrWhiteSpace(_abertura.Observacao.val())) {
                        _ocorrencia.Observacao.val(_abertura.Observacao.val());
                        _ocorrencia.ObservacaoCTe.val(_abertura.Observacao.val());
                        if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento) {
                            _ocorrencia.Observacao.enable(false);
                            _ocorrencia.ObservacaoCTe.enable(false);
                        }
                        else {
                            _ocorrencia.Observacao.enable(true);
                            _ocorrencia.ObservacaoCTe.enable(true);
                        }

                    }
                    else {
                        _ocorrencia.Observacao.enable(true);
                        _ocorrencia.ObservacaoCTe.enable(true);
                    }

                    var valor = Globalize.parseFloat(_abertura.Valor.val());
                    if (valor > 0 && _ocorrencia.ValorOcorrencia.enable) {
                        _ocorrencia.ValorOcorrencia.val(_abertura.Valor.val());
                        if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                            _ocorrencia.ValorOcorrencia.enable(false);
                        else
                            _ocorrencia.ValorOcorrencia.enable(true);
                    }
                    else
                        _ocorrencia.ValorOcorrencia.enable(true);
                });
                _ocorrencia.OcorrenciaSalvaPelaTelaChamadoOcorrencia.val(true);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function estornarChamadoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Chamado.ChamadoOcorrencia.Confirmacao, Localization.Resources.Chamado.ChamadoOcorrencia.RealmenteDesejaEstornarAtendimento, function () {
        executarReST("ChamadoAnalise/EstornarChamado", { Codigo: _chamado.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Chamado.ChamadoOcorrencia.AtendimentoEstornadoComSucesso);
                    buscarChamadoPorCodigo(_chamado.Codigo.val());
                    recarregarGridChamados();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function editarOcorrenciaNoChamado(data) {
    limparCamposOcorrencia();
    _ocorrencia.Codigo.val(data.Codigo);
    _ocorrencia.NaoLimparCarga.val(true);
    buscarOcorrenciaPorCodigo(function () {
        _chamadoOcorrenciaModalOcorrencia.show()
    });
}

function Etapa3ComoOcorrencia() {
    _etapa.Etapa3.text(Localization.Resources.Chamado.ChamadoOcorrencia.Ocorrencia);
    _etapa.Etapa3.tooltip(Localization.Resources.Chamado.ChamadoOcorrencia.QuandoNecessarioECriadoUmaOcorrencia);
    _etapa.Etapa3.tooltipTitle(Localization.Resources.Chamado.ChamadoOcorrencia.Ocorrencia);

    _analise.Finalizar.text(Localization.Resources.Chamado.ChamadoOcorrencia.LiberarOcorrencia);
    _analise.Fechar.text(Localization.Resources.Chamado.ChamadoOcorrencia.FecharSemOcorrencia);

    $("#knockoutOcorrencia").show();
}