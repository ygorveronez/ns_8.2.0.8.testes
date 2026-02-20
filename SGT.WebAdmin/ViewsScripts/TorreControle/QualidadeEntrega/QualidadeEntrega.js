/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumTipoEventoApp.js" />
/// <reference path="../../Enumeradores/EnumSituacaoProcessamentoIntegracaoSuperApp.js" />

// #region Objetos Globais do Arquivo
var _pesquisaQualidadeEntrega;
var _qualidadeEntrega;
var _configuracaoQualidadeEntrega;
var _gridQualidadeEntrega;
var _resumoQualidadeEntrega;
var _alterarDataEntrega;
var _visualizarImagemCanhoto;
var multiplaescolha = {};
var _knoutArquivo;
var _gridHistoricoCanhotos;
var _knoutDetalhesCanhoto;

// #endregion Objetos Globais do Arquivo

// #region Classes
var PesquisaQualidadeEntrega = function () {
    this.Pesquisar = PropertyEntity({ eventClick: pesquisarQualidadeEntregasClick, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.NumeroCarga = PropertyEntity({ text: "Carga:", visible: ko.observable(true) });
    this.NumeroNF = PropertyEntity({ text: "Número NF:", visible: ko.observable(true) });
    this.DataInicioCriacaoCarga = PropertyEntity({ text: "Data de início de criação da carga: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: "" });
    this.DataFimCriacaoCarga = PropertyEntity({ text: "Data de fim de criação da carga: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: "" });
    this.DataInicioEmissaoNF = PropertyEntity({ text: "Data de início da emissão da NF: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.DataFimEmissaoNF = PropertyEntity({ text: "Data de fim da emissão da NF: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "" });
    this.DisponivelParaConsulta = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null), def: null });

    this.DataInicioCriacaoCarga.dateRangeLimit = this.DataFimCriacaoCarga;
    this.DataFimCriacaoCarga.dateRangeInit = this.DataInicioCriacaoCarga;
    this.DataInicioEmissaoNF.dateRangeLimit = this.DataFimEmissaoNF;
    this.DataFimEmissaoNF.dateRangeInit = this.DataInicioEmissaoNF;
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ text: "Situação Digitalização Canhoto:", val: ko.observable(EnumSituacaoDigitalizacaoCanhoto.Todas), options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoesPesquisa(), def: EnumSituacaoDigitalizacaoCanhoto.Todas });
    this.SituacaoPgtoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoPgtoCanhoto.Todas), def: EnumSituacaoDigitalizacaoCanhoto.Todas });
    this.TipoCanhoto = PropertyEntity({ text: "Tipo Canhoto:", val: ko.observable(EnumTipoCanhoto.Todos), options: EnumTipoCanhoto.obterOpcoesPesquisaComPlaceHolder(), def: EnumTipoCanhoto.Todos, visible: ko.observable(true) });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.QualidadeEntrega,
    });

    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.QualidadeEntrega, _pesquisaQualidadeEntrega) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });

    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            LimparCampos(_pesquisaQualidadeEntrega);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var QualidadeEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar 100 primeiros registros", visible: ko.observable(true) });
    this.LiberarMultiplasNotas = PropertyEntity({ eventClick: liberarMultiplasNotasParaConsulta, type: types.event, text: "Liberar Notas", visible: ko.observable(false) });
}

var ConfiguracaoQualidadeEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.VerificarDataConfirmacaoIntervaloRaio = PropertyEntity({ text: "Data de confirmação dentro do intervalo do raio", getType: typesKnockout.bool, visible: ko.observable(true), tooltip: "Avaliação se a data de confirmação de entrega está dentro do intervalo entre a data de entrada e saída no raio de entrega do cliente." });
    this.ConsiderarDataHoraConfirmacaoIntervaloRaio = PropertyEntity({ text: "Considerar horas e minutos", getType: typesKnockout.bool, visible: ko.observable(true) });
    this.SalvarPreferencias = PropertyEntity({ eventClick: salvarConfiguracoesQualidadeEntrega, type: types.event, text: "Salvar Preferencias", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarConfiguracaoQualidadeEntrega, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
    this.Auditar = PropertyEntity({ eventClick: auditarQualidadeEntregaClick, type: types.event, text: "Auditoria", visible: ko.observable(true) });
}

var ResumoQualidadeEntrega = function () {
    this.QtdDisponivelParaConsulta = PropertyEntity({ type: types.string, val: ko.observable(0) });
    this.QtdNaoDisponivelParaConsulta = PropertyEntity({ type: types.string, val: ko.observable(0) });
    this.QtdTotalParaConsulta = PropertyEntity({ type: types.string, val: ko.observable(0) });

    this.DisponivelParaConsulta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            ObterResumoQualidadeEntregas(1, false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.NaoDisponivelParaConsulta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            ObterResumoQualidadeEntregas(0, false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.TotalParaConsulta = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            ObterResumoQualidadeEntregas(null, false);
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

}

var AlterarDataEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicioEntregaInformada = PropertyEntity({ text: "Data início entrega", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(true) });
    this.DataFimEntregaInformada = PropertyEntity({ text: "Data Fim Entrega", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(true) });
    this.DataEntradaRaio = PropertyEntity({ text: "Data entrada no Raio", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(true) });
    this.DataSaidaRaio = PropertyEntity({ text: "Data saída do Raio", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(true) });
    this.AlterarDataEntrega = PropertyEntity({ eventClick: alterarDataEntregaQualidadeClick, type: types.event, text: "Alterar data de Entrega", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlterarDataEntrega, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
    this.LiberarPreenchimentoEntradaSaidaRaio = PropertyEntity({ val: ko.observable(false) });
}

var VisualizarImagemCanhoto = function () {
    this.CodigoCanhoto = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Imagem = PropertyEntity({ val: ko.observable(), getType: typesKnockout.string });
    this.ArquivoPDF = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
}

var KnoutArquivo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

// #endregion Classes

// #region Funções de Inicialização
function loadQualidadeEntrega() {
    _pesquisaQualidadeEntrega = new PesquisaQualidadeEntrega();
    KoBindings(_pesquisaQualidadeEntrega, "knockoutPesquisaQualidadeEntrega");

    _qualidadeEntrega = new QualidadeEntrega();
    KoBindings(_qualidadeEntrega, "knockoutQualidadeEntrega");

    _configuracaoQualidadeEntrega = new ConfiguracaoQualidadeEntrega();
    KoBindings(_configuracaoQualidadeEntrega, "knockoutConfiguracaoQualidadeEntrega");

    _resumoQualidadeEntrega = new ResumoQualidadeEntrega();
    KoBindings(_resumoQualidadeEntrega, "knockoutResumoQualidadeEntrega");

    _alterarDataEntrega = new AlterarDataEntrega();
    KoBindings(_alterarDataEntrega, "knockoutAlterarDataEntrega");

    _visualizarImagemCanhoto = new VisualizarImagemCanhoto();
    KoBindings(_visualizarImagemCanhoto, "knockoutVisualizarImagemCanhoto");

    BuscarFilial(_pesquisaQualidadeEntrega.Filial);

    _knoutArquivo = new KnoutArquivo();
    KoBindings(_knoutArquivo, "knockoutArquivo");  

    // Inicializar o módulo DetalhesCanhoto com log para diagnóstico
    console.log("Status do DetalhesCanhotoModulo:", window.DetalhesCanhotoModulo);
    if (window.DetalhesCanhotoModulo && typeof window.DetalhesCanhotoModulo.inicializar === 'function') {
        console.log("Inicializando DetalhesCanhotoModulo");
        window.DetalhesCanhotoModulo.inicializar(_knoutArquivo);
    } else {
        console.error("Módulo DetalhesCanhotoModulo não encontrado");
    }

    ObterResumoQualidadeEntregas(null, true);
    loadGridQualidadeEntrega();
}

function loadGridQualidadeEntrega() {
    var alterarDataEntrega = { descricao: "Alterar data de Entrega", id: guid(), evento: "onclick", metodo: abrirModalAlterarDataEntregaClick };
    var liberarNota = { descricao: "Liberar Nota", id: guid(), evento: "onclick", metodo: liberarNotaParaConsulta, visibilidade: visibilidadeLiberarNota };
    var visualizarCanhoto = { descricao: "Visualizar canhoto", id: guid(), evento: "onclick", metodo: abrirModalVisualizarImagemCanhoto };

    var auditoriaCanhoto = { descricao: "Auditoria do Canhoto", id: guid(), metodo: abrirModalVisualizarAuditoriaCanhoto };
    var historicoCanhoto = {
        descricao: "Histórico do Canhoto",
        id: guid(),
        evento: "onclick",
        metodo: function (e) {
            if (window.DetalhesCanhotoModulo && typeof window.DetalhesCanhotoModulo.detalhesCanhotoClick === 'function') {
                window.DetalhesCanhotoModulo.detalhesCanhotoClick(e);
            } else if (typeof detalhesCanhotoClick === 'function') {
                detalhesCanhotoClick(e);
            }
        }
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 5, opcoes: [alterarDataEntrega, liberarNota, visualizarCanhoto, auditoriaCanhoto, historicoCanhoto] };

    var configuracoesExportacao = { url: "QualidadeEntrega/ExportarPesquisa", titulo: "Qualidade das entregas" };

    multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _qualidadeEntrega.SelecionarTodos,
        somenteLeitura: false,
        callbackNaoSelecionado: callbackNaoSelecionado,
        callbackSelecionado: callbackSelecionado,
        callbackSelecionarTodos: callbackSelecionarTodos,
    };

    _gridQualidadeEntrega = new GridView("grid-qualidade-entrega", "QualidadeEntrega/Pesquisa", _pesquisaQualidadeEntrega, menuOpcoes, null, 10, null, true, false, multiplaescolha, 50, undefined, configuracoesExportacao);

    _gridQualidadeEntrega.SetPermitirEdicaoColunas(true);
    _gridQualidadeEntrega.SetSalvarPreferenciasGrid(true);
    _gridQualidadeEntrega.SetScrollHorizontal(true);
    _gridQualidadeEntrega.SetHabilitarScrollHorizontal(true, 150);
    _gridQualidadeEntrega.CarregarGrid();

}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function auditarCanhotoClick(e, sender) {
    var data = { Codigo: _knoutArquivo.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("Canhoto", null, e);

    closureAuditoria(data);
}

function callbackSelecionado(argumentoNulo, registroSelecionado) {
    let selecionarTodos = _qualidadeEntrega.SelecionarTodos.val();
    let dadosGrid = _gridQualidadeEntrega.GridViewTableData();
    let limiteRegistrosPermitidoSelecionar = 100;

    if (registroSelecionado) {
        _qualidadeEntrega.LiberarMultiplasNotas.visible(true);
        multiplaescolha.selecionados.push(registroSelecionado);
    }

    if (selecionarTodos) {
        for (i = 0; i < dadosGrid.length; i++) {
            let dado = dadosGrid[i];
            $('#' + dado.DT_RowId).removeClass('selected');
        }

        let primeirosXRegistros = dadosGrid.slice(0, limiteRegistrosPermitidoSelecionar);
        multiplaescolha.selecionados = primeirosXRegistros;

        for (i = 0; i < primeirosXRegistros.length; i++) {
            let registro = primeirosXRegistros[i];
            $('#' + registro.DT_RowId).addClass('selected');
        }
    }

    if (!selecionarTodos && registroSelecionado == undefined) {
        multiplaescolha.callbackNaoSelecionado();
    }

    if (multiplaescolha.selecionados.length > 0)
        _qualidadeEntrega.LiberarMultiplasNotas.visible(true);
}

function callbackNaoSelecionado(argumentoNulo, registroClicado) {

    if (registroClicado)
        multiplaescolha.selecionados = multiplaescolha.selecionados.filter(item => item.DT_RowId !== registroClicado.DT_RowId);
    else
        multiplaescolha.selecionados = [];

    if (multiplaescolha.selecionados.length === 0) {
        _qualidadeEntrega.LiberarMultiplasNotas.visible(false);
    }
}


function callbackSelecionarTodos() {
    multiplaescolha.selecionados = [];

    if (multiplaescolha.selecionados.length === 0) {
        _qualidadeEntrega.LiberarMultiplasNotas.visible(false);
    }
}

function abrirModalFiltrosPesquisaQualidadeEntrega() {
    Global.abrirModal('divModalFiltrosPesquisaQualidadeEntrega');
}

function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {
        if (retorno.Codigo !== 0) {
            e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
            e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

            PreencherJsonFiltroPesquisa(_pesquisaQualidadeEntrega, retorno.Dados);
        }
    }, EnumCodigoFiltroPesquisa.QualidadeEntrega);

    buscaFiltros.AbrirBusca();
}

function pesquisarQualidadeEntregasClick() {
    loadGridQualidadeEntrega();
    ObterResumoQualidadeEntregas(null, true);
    Global.fecharModal('divModalFiltrosPesquisaQualidadeEntrega');
}

function abrirModalConfiguracaoQualidadeEntrega() {
    executarReST("QualidadeEntrega/BuscarConfiguracoesQualidadeEntrega", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno) {
                PreencherObjetoKnout(_configuracaoQualidadeEntrega, retorno);
                Global.abrirModal('divModalConfigurarQualidadeEntrega');
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarConfiguracaoQualidadeEntrega() {
    Global.fecharModal('divModalConfigurarQualidadeEntrega');
    LimparCampos(_configuracaoQualidadeEntrega);
}

function cancelarAlterarDataEntrega() {
    Global.fecharModal('divModalAlterarDataEntrega');
    LimparCampos(_alterarDataEntrega);
}

function abrirModalVisualizarImagemCanhoto(e) {
    executarReST("QualidadeEntrega/ObterImagemCanhoto", { CodigoCanhoto: e.CodigoCanhoto }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                _visualizarImagemCanhoto.CodigoCanhoto.val(retorno.Data.CodigoCanhoto);
                _visualizarImagemCanhoto.Imagem.val(retorno.Data.Miniatura);
                _visualizarImagemCanhoto.ArquivoPDF.val(retorno.Data.ArquivoPDF);

                if (_visualizarImagemCanhoto.Imagem.val() == null && _visualizarImagemCanhoto.ArquivoPDF.val() == false) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Não foi possível encontrar a imagem do canhoto.");
                    return;
                }

                Global.abrirModal('divModalVisualizarCanhoto');
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function abrirModalVisualizarAuditoriaCanhoto(e) {
    var data = { Codigo: e.CodigoCanhoto };
    var closureAuditoria = OpcaoAuditoria("Canhoto", null, e);

    closureAuditoria(data);
}

function abrirModalAlterarDataEntregaClick(e) {
    _alterarDataEntrega.Codigo.val(e.CodigoCargaEntrega);
    _alterarDataEntrega.DataEntradaRaio.val(e.DataEntradaRaioFormatada);
    _alterarDataEntrega.DataSaidaRaio.val(e.DataSaidaRaioFormatada);
    _alterarDataEntrega.LiberarPreenchimentoEntradaSaidaRaio.val(_alterarDataEntrega.DataSaidaRaio.val().length == 0 || _alterarDataEntrega.DataEntradaRaio.val().length == 0);
    Global.abrirModal('divModalAlterarDataEntrega');
}

function salvarConfiguracoesQualidadeEntrega() {
    let configuracaoSalvar = RetornarObjetoPesquisa(_configuracaoQualidadeEntrega);
    executarReST("QualidadeEntrega/SalvarConfiguracoesQualidadeEntrega", configuracaoSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                cancelarConfiguracaoQualidadeEntrega();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ObterResumoQualidadeEntregas(disponivelParaConsulta, consultarContagemLiberacaoCanhoto) {

    _pesquisaQualidadeEntrega.DisponivelParaConsulta.val(disponivelParaConsulta);

    var data = RetornarObjetoPesquisa(_pesquisaQualidadeEntrega);
    if (consultarContagemLiberacaoCanhoto) {
        executarReST("QualidadeEntrega/ObterResumoQualidadeEntregas", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data != undefined) {
                        _resumoQualidadeEntrega.QtdDisponivelParaConsulta.val(arg.Data.QtdDisponivelParaConsulta);
                        _resumoQualidadeEntrega.QtdNaoDisponivelParaConsulta.val(arg.Data.QtdNaoDisponivelParaConsulta);
                        _resumoQualidadeEntrega.QtdTotalParaConsulta.val(arg.Data.QtdTotalParaConsulta);
                    }

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
    if (_gridQualidadeEntrega)
        _gridQualidadeEntrega.CarregarGrid();
}

function alterarDataEntregaQualidadeClick(e) {
    if (string.IsNullOrWhiteSpace(e.DataInicioEntregaInformada.val()) && string.IsNullOrWhiteSpace(e.DataFimEntregaInformada.val())) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Preencha as datas.");
        return;
    }

    executarReST("ControleEntregaEntrega/AlterarDatasEntrega", {
        Codigo: e.Codigo.val(),
        DataInicioEntregaInformada: e.DataInicioEntregaInformada.val(),
        DataEntregaInformada: e.DataFimEntregaInformada.val(),
        DataEntradaRaio: e.DataEntradaRaio.val(),
        DataSaidaRaio: e.DataSaidaRaio.val(),
    },
        function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Data Alterada com sucesso");
                    cancelarAlterarDataEntrega();
                    ObterResumoQualidadeEntregas(null, true);
                    _gridQualidadeEntrega.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
}

function liberarNotaParaConsulta(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja liberar a nota para consulta?", function () {
        executarReST("QualidadeEntrega/LiberarNotaParaConsulta", {
            CodigoCanhoto: registroSelecionado.CodigoCanhoto
        }, function (retorno) {
            if (retorno.Success) {
                if (retorno) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                    _gridQualidadeEntrega.CarregarGrid();
                    ObterResumoQualidadeEntregas(null, true);
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function visibilidadeLiberarNota(e) {
    if ((!e.DisponivelParaConsulta) && (_CONFIGURACAO_TMS.UsuarioAdministrador || (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.QualidadeEntrega_PermiteLiberarNotasBloqueadas, _PermissoesPersonalizadasQualidadeEntrega))))
        return true;

    return false;
}

function liberarMultiplasNotasParaConsulta() {
    if (multiplaescolha.selecionados.length > 100) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O número máximo para envio em massa é de 100 registros");
        return;
    }

    let registrosSelecionados = JSON.stringify(multiplaescolha.selecionados);
    exibirConfirmacao("Confirmação", "Realmente deseja liberar as notas selecionadas para consulta?", function () {
        executarReST("QualidadeEntrega/LiberarMultiplasNotasParaConsulta", { ItensSelecionados: registrosSelecionados }, function (retorno) {
            if (retorno.Success) {
                if (retorno) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                    _gridQualidadeEntrega.CarregarGrid();
                    _gridQualidadeEntrega.AtualizarRegistrosSelecionados([]);
                    ObterResumoQualidadeEntregas(null, true);
                    _qualidadeEntrega.SelecionarTodos.val(false);
                    _qualidadeEntrega.LiberarMultiplasNotas.visible(false);
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function auditarQualidadeEntregaClick(e, sender) {
    const data = { Codigo: e.Codigo.val() };
    const closureAuditoria = OpcaoAuditoria("ConfiguracaoQualidadeEntrega", null, e);

    closureAuditoria(data);
}

// #endregion Funções Associadas a Eventos