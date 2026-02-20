/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPesagemCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _carga;
var _gridCargas;
var _pesquisaCarga;
var _rejeicao;
var _situacaoPesagemCargaUltimaPesquisa = EnumSituacaoPesagemCarga.AguardandoAprovacao;
var $modalDetalhesCarga;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCargasSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Carga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:" });
    this.Filial = PropertyEntity({ text: "Filial: " });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular: " });
    this.Motoristas = PropertyEntity({ text: "Motoristas: " });
    this.Operador = PropertyEntity({ text: "Operador: " });
    this.Placas = PropertyEntity({ text: "Placas: " });
    this.Rota = PropertyEntity({ text: "Rota: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga: " });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação: " });
    this.Transportador = PropertyEntity({ text: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? "Transportador: " : "Empresa/Filial: " });
    this.PesoCarga = PropertyEntity({ text: "Peso da Carga: " });
    this.PesagemInicial = PropertyEntity({ text: "Pesagem Inicial: " });
    this.PesagemFinal = PropertyEntity({ text: "Pesagem Final: " });
    this.PesoBruto = PropertyEntity({ text: "Peso Bruto: " });
    this.DiferencaPeso = PropertyEntity({ text: "Diferença de Peso (KG): " });
    this.PercentualDiferencaPeso = PropertyEntity({ text: "Diferença de Peso (%): " });

    this.DetalhesCarga = PropertyEntity({ eventClick: detalhesCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes, visible: ko.observable(true) });
};

var PesquisaCarga = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "" });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.SituacaoPesagem = PropertyEntity({ val: ko.observable(EnumSituacaoPesagemCarga.AguardandoAprovacao), options: EnumSituacaoPesagemCarga.obterOpcoesPesquisaAutorizacao(), def: EnumSituacaoPesagemCarga.AguardandoAprovacao, text: "Situação da Pesagem: " });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasCargasClick, text: "Aprovar Cargas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCargas, text: "Pesquisar", idGrid: "grid-pesquisa-autorizacao-carga", visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasCargasClick, text: "Rejeitar Cargas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Cargas", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasCargasClick, text: "Reprocessar Cargas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {

    _carga = new Carga();
    KoBindings(_carga, "knockoutCarga");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoCarga");

    _pesquisaCarga = new PesquisaCarga();
    KoBindings(_pesquisaCarga, "knockoutPesquisaCarga");

    loadGridCargas();
    loadRegras();
    loadDelegar();

    $modalDetalhesCarga = $("#divModalCarga");

    BuscarFilial(_pesquisaCarga.Filial);
    BuscarFuncionario(_pesquisaCarga.Usuario);
    BuscarTiposOperacao(_pesquisaCarga.TipoOperacao);
    BuscarTiposdeCarga(_pesquisaCarga.TipoDeCarga);
    BuscarModelosVeicularesCarga(_pesquisaCarga.ModeloVeicularCarga);

    loadDadosUsuarioLogado(atualizarGridCargas);

    // Valida se a tela está sendo carregado pelo link de acesso enviado via e-mail
    if (CODIGO_CARGA_VIA_TOKEN_ACESSO_AUTORIZACAO_CARGA.val() != "")
        carregarCargaUsuarioAcessadoViaLink(CODIGO_CARGA_VIA_TOKEN_ACESSO_AUTORIZACAO_CARGA.val());
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaCarga.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaCarga.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridCargas() {
    let opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharCarga,
        tamanho: "10",
        icone: ""
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };

    let multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCarga.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    let configuracaoExportacao = {
        url: "AutorizacaoPesagem/ExportarPesquisa",
        titulo: "Autorização Pesagem"
    };

    _gridCargas = new GridView(_pesquisaCarga.Pesquisar.idGrid, "AutorizacaoPesagem/Pesquisa", _pesquisaCarga, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
    _gridCargas.SetPermitirEdicaoColunas(true);
    _gridCargas.SetSalvarPreferenciasGrid(true);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasCargasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as cargas selecionadas?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaCarga);

        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterCodigosMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterCodigosMultiplosNaoSelecionados());

        executarReST("AutorizacaoPesagem/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridCargas();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    Global.fecharModal('divModalRejeitarCarga');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasCargasClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarCarga');
}

function rejeitarCargasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as cargas selecionadas?", function () {
        let dados = RetornarObjetoPesquisa(_pesquisaCarga);
        let rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterCodigosMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterCodigosMultiplosNaoSelecionados());

        executarReST("AutorizacaoPesagem/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridCargas();
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

function reprocessarMultiplasCargasClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaCarga);

    dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterCodigosMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterCodigosMultiplosNaoSelecionados());

    executarReST("AutorizacaoPesagem/ReprocessarMultiplas", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " cargas foram reprocessadas com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 carga foi reprocessada com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para as cargas selecionadas.");

                atualizarGridCargas();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function atualizarGridCargas() {
    _pesquisaCarga.SelecionarTodos.val(false);
    _pesquisaCarga.AprovarTodas.visible(false);
    _pesquisaCarga.DelegarTodas.visible(false);
    _pesquisaCarga.RejeitarTodas.visible(false);
    _pesquisaCarga.ReprocessarTodas.visible(false);

    _gridCargas.CarregarGrid();

    _situacaoPesagemCargaUltimaPesquisa = _pesquisaCarga.SituacaoPesagem.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaCarga.AprovarTodas.visible(false);
    _pesquisaCarga.DelegarTodas.visible(false);
    _pesquisaCarga.RejeitarTodas.visible(false);
    _pesquisaCarga.ReprocessarTodas.visible(false);

    let existemRegistrosSelecionados = _gridCargas.ObterMultiplosSelecionados().length > 0;
    let selecionadoTodos = _pesquisaCarga.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoPesagemCargaUltimaPesquisa == EnumSituacaoPesagemCarga.AguardandoAprovacao) {
            _pesquisaCarga.AprovarTodas.visible(true);
            _pesquisaCarga.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaCarga.RejeitarTodas.visible(true);
        }
        else if (_situacaoPesagemCargaUltimaPesquisa == EnumSituacaoPesagemCarga.SemRegraAprovacao)
            _pesquisaCarga.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharCarga(registroSelecionado) {
    let pesquisa = RetornarObjetoPesquisa(_pesquisaCarga);

    _carga.Codigo.val(registroSelecionado.Codigo);
    _carga.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_carga, "AutorizacaoPesagem/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoPesagemCarga === EnumSituacaoPesagemCarga.AguardandoAprovacao);

                $modalDetalhesCarga.modal("show");
                $modalDetalhesCarga.one('hidden.bs.modal', function () {
                    limparCamposCarga();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposCarga() {
    $("#myTab a:first").tab("show");

    limparRegras();
}

function carregarCargaUsuarioAcessadoViaLink(codigoCarga) {
    _carga.Codigo.val(codigoCarga);

    BuscarPorCodigo(_carga, "AutorizacaoPesagem/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {              

                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoPesagemCarga === EnumSituacaoPesagemCarga.AguardandoAprovacao);

                $modalDetalhesCarga.modal("show");
                $modalDetalhesCarga.one('hidden.bs.modal', function () {
                    limparCamposCarga();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function detalhesCargaClick(cargaSelecionada) {
    _cargaAtual = cargaSelecionada;
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val());
}