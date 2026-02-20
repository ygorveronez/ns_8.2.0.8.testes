/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDocumento.js" />
/// <reference path="../../Enumeradores/EnumMotivoInconsistenciaGestaoDocumento.js" />
/// <reference path="Desconto.js" />
/// <reference path="DetalhesGestaoDocumento.js" />
/// <reference path="../../Consultas/CTe.js" />

// #region Objetos Globais do Arquivo

var _gridGestaoDocumento;
var _pesquisaGestaoDocumento;
var _rejeicao;
var _situacoesGestaoDocumentoUltimaPesquisa = _CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos ? [EnumSituacaoGestaoDocumento.AguardandoAprovacao] : [EnumSituacaoGestaoDocumento.Inconsistente];

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaGestaoDocumento = function () {
    this.Chave = PropertyEntity({ text: "Chave:", maxlength: 4600 });
    this.NumeroCTe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Número CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroNF = PropertyEntity({ text: "Número NF-e:" });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.MotivoInconsistenciaGestaoDocumento = PropertyEntity({ text: "Motivo da Inconsistência:", val: ko.observable(new Array()), options: EnumMotivoInconsistenciaGestaoDocumento.obterOpcoes(), def: new Array(), getType: typesKnockout.selectMultiple });
    this.SituacaoGestaoDocumento = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Situação:", val: ko.observable(new Array()), options: EnumSituacaoGestaoDocumento.obterOpcoesPesquisa(), def: _situacoesGestaoDocumentoUltimaPesquisa });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, visible: ko.observable(true), codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Nº do Pedido Cliente:", val: ko.observable(""), def: "", visible: _CONFIGURACAO_TMS.ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos });
    this.RegistroComCarga = PropertyEntity({ text: "Registros com cargas", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Usuário:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos) });
    this.ChaveNFe = PropertyEntity({ text: "Chave NF-e:", maxlength: 500 });

    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.Pesquisar = PropertyEntity({ eventClick: atualizarGridGestaoDocumentos, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(permitirEditarInformacoesGestaoDocumentos()) });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ImportarDesconto = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(_CONFIGURACAO_TMS.HabilitarDescontoGestaoDocumento && permitirEditarInformacoesGestaoDocumentos()),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: false,
        UrlImportacao: "GestaoDocumento/ImportacaoDesconto",
        UrlConfiguracao: "GestaoDocumento/ConfiguracaoImportacaoDesconto",
        CodigoControleImportacao: EnumCodigoControleImportacao.O036_GestaoDocumentoDesconto,
        CallbackImportacao: function () {
            _gridGestaoDocumento.CarregarGrid();
        }
    });

    // Aprovação sem alçada // aprovarSelecionadosClick
    this.AprovarSelecionados = PropertyEntity({ eventClick: definirMetodoAprovacaoClick, type: types.event, text: "Aprovar selecionados", idGrid: guid(), visible: ko.observable(false) });
    this.AprovarTodos = PropertyEntity({ eventClick: aprovarTodosClick, type: types.event, text: "Aprovar todos", idGrid: guid(), visible: ko.observable(!_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos && permitirEditarInformacoesGestaoDocumentos()) });

    // Aprovação com alçada
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosDocumentosClick, text: "Aprovar Documentos", visible: ko.observable(false) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Documentos", visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosDocumentosClick, text: "Rejeitar Documentos", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplosDocumentosClick, text: "Reprocessar Documentos", visible: ko.observable(false) });
};

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarDocumentosSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var AprovarComMotivo = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.AjustarValorFrete = PropertyEntity({ text: "Ajustar o valor do frete com o valor do CTE aprovado", getType: typesKnockout.bool, val: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.AprovarComMotivo = PropertyEntity({ eventClick: aprovarSelecionadosClick, type: types.event, text: "Aprovar todos", idGrid: guid(), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaGestaoDocumento.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaGestaoDocumento.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGestaoDocumento() {
    _pesquisaGestaoDocumento = new PesquisaGestaoDocumento();
    KoBindings(_pesquisaGestaoDocumento, "knockoutPesquisaGestaoDocumento", false, _pesquisaGestaoDocumento.Pesquisar.id);

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoGestaoDocumento");

    _aprovarComMotivo = new AprovarComMotivo();
    KoBindings(_aprovarComMotivo, "knockoutMotivoAprovacaoGestaoDocumento");

    HeaderAuditoria("GestaoDocumento");

    BuscarTransportadores(_pesquisaGestaoDocumento.Empresa, null, null, true);
    BuscarFilial(_pesquisaGestaoDocumento.Filial);
    BuscarClientes(_pesquisaGestaoDocumento.Tomador);
    BuscarOcorrencias(_pesquisaGestaoDocumento.Ocorrencia);
    BuscarClientes(_pesquisaGestaoDocumento.Remetente);
    BuscarCargas(_pesquisaGestaoDocumento.Carga);
    BuscarTiposOperacao(_pesquisaGestaoDocumento.TipoOperacao);
    BuscarFuncionario(_pesquisaGestaoDocumento.Usuario);
    BuscarCTes(_pesquisaGestaoDocumento.NumeroCTe);

    $.get("Content/Static/Documentos/ModalDetalheGestaoDocumentoAprovacao.html?dyn=" + guid(), function (conteudo) {
        $('#divPaiModalDetalheGestaoDocumentoAprovacao').html(conteudo);
        loadDetalheGestaoDocumento();
        loadDetalheGestaoDocumentoAprovacao();
        loadGridGestaoDocumento();
        loadDadosUsuarioLogado(atualizarGridGestaoDocumentos);
    });

    verificarTomadores();
    loadDescontoGestaoDocumento();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaGestaoDocumento.Usuario.visible(false);
        _pesquisaGestaoDocumento.Empresa.visible(false);
    }
}

function loadGridGestaoDocumento() {
    let auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("GestaoDocumento"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    let gerarDocumentoEntrada = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: exibirDetalhesGestaoDocumentoClick, tamanho: "20", icone: "" };
    let desconto = { descricao: "Desconto", id: guid(), evento: "onclick", metodo: exibirDescontoGestaoDocumentoClick, tamanho: "20", visibilidade: mostrarOpcaoDescontoGestaoDocumentoGrid };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [gerarDocumentoEntrada, desconto, auditar],
        tamanho: 7
    };

    let multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaGestaoDocumento.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    if (!permitirEditarInformacoesGestaoDocumentos())
        multiplaEscolha = null;

    let configExportacao = {
        url: "GestaoDocumento/ExportarPesquisa",
        titulo: "Gestão Documentos"
    };

    _gridGestaoDocumento = new GridViewExportacao("grid-gestao-documento", "GestaoDocumento/Pesquisa", _pesquisaGestaoDocumento, menuOpcoes, configExportacao, null, 10, multiplaEscolha, 50);
    _gridGestaoDocumento.SetPermitirEdicaoColunas(true);
    _gridGestaoDocumento.SetPermitirReordenarColunas(false);
    _gridGestaoDocumento.SetSalvarPreferenciasGrid(true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarTodosClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja aprovar aprovar todos os CT-es inconsistentes listados abaixo, lembrando que esse processo é irreversível (ao confirmar, o processamento da aprovação irá ocorrer em background no sistema, ou seja, não será possível acompanhar o processamento, mas os inconsistentes serão liberados aos poucos, basta aguardar)?", function () {
        Salvar(_pesquisaGestaoDocumento, "GestaoDocumento/AprovarTodosInconsistentes", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação enviada com sucesso, os CT-es inconsistentes estão sendo aprovados.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function aprovarMultiplosDocumentosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os documentos selecionados?", function () {
        if (_CONFIGURACAO_TMS.AjustarValorFreteAposAprovacaoPreCTe) {
            exibirConfirmacao("Confirmação", "Deseja ajustar o valor do frete conforme valor do(s) CTe(s) aprovado?", function () {
                aprovarMultiplosDocumentos(true);
            }, function () {
                aprovarMultiplosDocumentos(false);
            });
        }
        else
            aprovarMultiplosDocumentos(false);
    });
}

function aprovarMultiplosDocumentos(ajustarValorFrete) {
    let dados = RetornarObjetoPesquisa(_pesquisaGestaoDocumento);

    dados.SelecionarTodos = _pesquisaGestaoDocumento.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridGestaoDocumento.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridGestaoDocumento.ObterMultiplosNaoSelecionados());
    dados.AjustarValorFrete = ajustarValorFrete ?? false;

    executarReST("GestaoDocumento/AprovarMultiplosItens", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                if (retorno.Data.RegrasModificadas > 0) {
                    if (retorno.Data.RegrasModificadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                }
                else if (retorno.Data.Msg == "")
                    exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                atualizarGridGestaoDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function definirMetodoAprovacaoClick() {
    if (_CONFIGURACAO_TMS.NaoExigirMotivoAprovacaoCTeInconsistente)
        aprovarSelecionadosClick();
    else
        exibirAprovarMotivoSelecionados();
}

function aprovarSelecionadosClick() {
    let listaCodigos = new Array();

    for (let i = 0; i < _gridGestaoDocumento.ObterMultiplosSelecionados().length; i++) {
        var registro = _gridGestaoDocumento.ObterMultiplosSelecionados()[i];
        listaCodigos.push(registro.Codigo);
    }

    let motivoAprovacao = RetornarObjetoPesquisa(_aprovarComMotivo);

    let motivoAprovado = motivoAprovacao.Motivo;
    let ajustarValorFrete = motivoAprovacao.AjustarValorFrete;

    executarReST("GestaoDocumento/AprovarSelecionados", { Codigos: JSON.stringify(listaCodigos), MotivoAprovado: motivoAprovado, AjustarValorFrete: ajustarValorFrete }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação enviada com sucesso.");
                fecharAprovarMotivoSelecionados();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    Global.fecharModal('divModalRejeitarGestaoDocumento');
}

function exibirDescontoGestaoDocumentoClick(registroSelecionado) {
    exibirDescontoGestaoDocumento(registroSelecionado);
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function exibirAprovarMotivoSelecionados() {
    _aprovarComMotivo.AjustarValorFrete.visible(_CONFIGURACAO_TMS.AjustarValorFreteAposAprovacaoPreCTe);
    Global.abrirModal('divModalMotivoAprovacaoGestaoDocumento');
}

function fecharAprovarMotivoSelecionados() {
    LimparCampos(_aprovarComMotivo);

    Global.fecharModal('divModalMotivoAprovacaoGestaoDocumento');
}

function exibirDetalhesGestaoDocumentoClick(registroSelecionado) {
    if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos)
        exibirDetalheGestaoDocumentoAprovacao(registroSelecionado);
    else
        exibirDetalheGestaoDocumento(registroSelecionado);
}

function rejeitarDocumentosSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os documentos selecionados?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaGestaoDocumento);
        let rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaGestaoDocumento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridGestaoDocumento.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridGestaoDocumento.ObterMultiplosNaoSelecionados());

        executarReST("GestaoDocumento/ReprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram reprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi reprovada.");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    atualizarGridGestaoDocumentos();
                    cancelarRejeicaoSelecionadosClick();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function rejeitarMultiplosDocumentosClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarGestaoDocumento');
}

function reprocessarMultiplosDocumentosClick() {
    let dados = RetornarObjetoPesquisa(_pesquisaGestaoDocumento);

    dados.SelecionarTodos = _pesquisaGestaoDocumento.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridGestaoDocumento.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridGestaoDocumento.ObterMultiplosNaoSelecionados());

    executarReST("GestaoDocumento/ReprocessarMultiplosDocumentos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " documentos foram reprocessados com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 documento foi reprocessado com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para os documentos selecionados.");

                atualizarGridGestaoDocumentos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function atualizarGridGestaoDocumentos() {
    _pesquisaGestaoDocumento.SelecionarTodos.val(false);
    _pesquisaGestaoDocumento.AprovarSelecionados.visible(false);
    _pesquisaGestaoDocumento.AprovarTodas.visible(false);
    _pesquisaGestaoDocumento.DelegarTodas.visible(false);
    _pesquisaGestaoDocumento.RejeitarTodas.visible(false);
    _pesquisaGestaoDocumento.ReprocessarTodas.visible(false);

    _gridGestaoDocumento.CarregarGrid();

    _situacoesGestaoDocumentoUltimaPesquisa = _pesquisaGestaoDocumento.SituacaoGestaoDocumento.val()
}

function permitirEditarInformacoesGestaoDocumentos() {
    return (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) || (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS);
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirMultiplasOpcoes() {
    let existemRegistrosSelecionados = _gridGestaoDocumento.ObterMultiplosSelecionados().length > 0;
    let selecionadoTodos = _pesquisaGestaoDocumento.SelecionarTodos.val();

    _pesquisaGestaoDocumento.AprovarSelecionados.visible(false);
    _pesquisaGestaoDocumento.AprovarTodas.visible(false);
    _pesquisaGestaoDocumento.DelegarTodas.visible(false);
    _pesquisaGestaoDocumento.RejeitarTodas.visible(false);
    _pesquisaGestaoDocumento.ReprocessarTodas.visible(false);

    if (permitirEditarInformacoesGestaoDocumentos() && (existemRegistrosSelecionados || selecionadoTodos)) {
        if (_CONFIGURACAO_TMS.UsarAlcadaAprovacaoGestaoDocumentos) {
            if (_situacoesGestaoDocumentoUltimaPesquisa.indexOf(EnumSituacaoGestaoDocumento.AguardandoAprovacao) >= 0) {
                _pesquisaGestaoDocumento.AprovarTodas.visible(true);
                _pesquisaGestaoDocumento.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
                _pesquisaGestaoDocumento.RejeitarTodas.visible(true);
            }
            else if ((_situacoesGestaoDocumentoUltimaPesquisa.indexOf(EnumSituacaoGestaoDocumento.Inconsistente) >= 0) || (_situacoesGestaoDocumentoUltimaPesquisa.indexOf(EnumSituacaoGestaoDocumento.SemRegraAprovacao) >= 0))
                _pesquisaGestaoDocumento.ReprocessarTodas.visible(true);
        }
        else
            _pesquisaGestaoDocumento.AprovarSelecionados.visible(true);
    }
}

function verificarTomadores() {
    executarReST("GestaoDocumento/PesquisarTomadores", null, function (retorno) {
        if (retorno.Success)
            _pesquisaGestaoDocumento.Tomador.visible(false);
        else
            _pesquisaGestaoDocumento.Tomador.visible(true);
    }, null, null);
}

// #endregion Funções Privadas
