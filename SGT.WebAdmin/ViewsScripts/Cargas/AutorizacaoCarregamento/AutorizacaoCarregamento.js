/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCarregamentoSolicitacao.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _carregamento;
var _gridCarregamento;
var _pesquisaCarregamento;
var _rejeicao;
var _situacaoCarregamentoSolicitacaoUltimaPesquisa = EnumSituacaoCarregamentoSolicitacao.AguardandoAprovacao;
var _modalDelegarSelecionados;
var _modalRejeitarCarregamento;
var $modalDetalhesCarregamento;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCarregamentosSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var Carregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.DataCarregamento = PropertyEntity({ text: "Data do Carregamento:" });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular: " });
    this.NumeroCarregamento = PropertyEntity({ text: "Número do Carregamento:" });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: " });

    this.Cubagem = PropertyEntity({ text: "Cubagem", val: ko.observable("0,0000"), def: "0,00", visible: ko.observable(true) });
    this.Pallets = PropertyEntity({ text: "Pallets", val: ko.observable("0,0000"), def: "0,000", visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: "Peso", val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(true) });
    this.CapacidadeCubagem = PropertyEntity({ text: "Capacidade Cubagem: ", val: ko.observable("0,00"), def: "0,00" });
    this.CapacidadePallets = PropertyEntity({ text: "Capacidade de Pallets: ", val: ko.observable("0,000"), def: "0,000" });
    this.CapacidadePeso = PropertyEntity({ text: "Capacidade Peso: ", val: ko.observable("0,0000"), def: "0,0000" });
    this.LotacaoCubagem = PropertyEntity({ text: "Lotação Cubagem: ", val: ko.observable("0,00"), def: "0,00" });
    this.LotacaoPallets = PropertyEntity({ text: "Lotação Pallets: ", val: ko.observable("0,00"), def: "0,00" });
    this.LotacaoPeso = PropertyEntity({ text: "Lotação Peso: ", val: ko.observable("0,0000"), def: "0,0000" });
};

var PesquisaCarregamento = function () {
    this.NumeroCarregamento = PropertyEntity({ text: "Número do Carregamento:", val: ko.observable(""), def: "" });
    this.DataInicio = PropertyEntity({ text: "Data de Carregamento Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data de Carregamento Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo Veicular de Carga:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCarregamentoSolicitacao.AguardandoAprovacao), options: EnumSituacaoCarregamentoSolicitacao.obterOpcoesPesquisa(), def: EnumSituacaoCarregamentoSolicitacao.AguardandoAprovacao, text: "Situação do Carregamento: " });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplosCarregamentosClick, text: "Aprovar Carregamentos", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCarregamento, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplosCarregamentosClick, text: "Rejeitar Carregamentos", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Carregamentos", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _carregamento = new Carregamento();
    KoBindings(_carregamento, "knockoutCarregamento");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoCarregamento");

    _pesquisaCarregamento = new PesquisaCarregamento();
    KoBindings(_pesquisaCarregamento, "knockoutPesquisaCarregamento");

    loadGridCarregamento();
    loadRegras();
    loadDelegar();

    $modalDetalhesCarregamento = $("#divModalCarregamento");

    new BuscarFilial(_pesquisaCarregamento.Filial);
    new BuscarModelosVeicularesCarga(_pesquisaCarregamento.ModeloVeicularCarga);
    new BuscarFuncionario(_pesquisaCarregamento.Usuario);
    new BuscarTiposdeCarga(_pesquisaCarregamento.TipoCarga);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#container-carregamento-peso-medida").hide();

    loadDadosUsuarioLogado(atualizarGridCarregamento);

    _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
    _modalRejeitarCarregamento = new bootstrap.Modal(document.getElementById("divModalRejeitarCarregamento"), { backdrop: true, keyboard: true });

    // Valida se a tela está sendo carregado pelo link de acesso enviado via e-mail
    if (CODIGO_CARREGAMENTO_VIA_TOKEN_ACESSO_AUTORIZACAO_CARREGAMENTO.val() != "")
        carregarCarregamentoUsuarioAcessadoViaLink(CODIGO_CARREGAMENTO_VIA_TOKEN_ACESSO_AUTORIZACAO_CARREGAMENTO.val());
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaCarregamento.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaCarregamento.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridCarregamento() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharCarregamento,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCarregamento.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoCarregamento/ExportarPesquisa",
        titulo: "Autorização de Carregamentos"
    };

    _gridCarregamento = new GridView(_pesquisaCarregamento.Pesquisar.idGrid, "AutorizacaoCarregamento/Pesquisa", _pesquisaCarregamento, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
    _gridCarregamento.SetPermitirEdicaoColunas(true);
    _gridCarregamento.SetSalvarPreferenciasGrid(true);

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplosCarregamentosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os carregamentos selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCarregamento);

        dados.SelecionarTodos = _pesquisaCarregamento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCarregamento.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCarregamento.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCarregamento/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "" || retorno.Data.Msg == null)
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", retorno.Data.Msg);

                    atualizarGridCarregamento();
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

    _modalRejeitarCarregamento.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplosCarregamentosClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarCarregamento.show();
}

function rejeitarCarregamentosSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os carregamentos selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCarregamento);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaCarregamento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCarregamento.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCarregamento.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCarregamento/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridCarregamento();
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

/*
 * Declaração das Funções
 */

function atualizarGridCarregamento() {
    _pesquisaCarregamento.SelecionarTodos.val(false);
    _pesquisaCarregamento.AprovarTodos.visible(false);
    _pesquisaCarregamento.DelegarTodos.visible(false);
    _pesquisaCarregamento.RejeitarTodos.visible(false);

    _gridCarregamento.CarregarGrid();

    _situacaoCarregamentoSolicitacaoUltimaPesquisa = _pesquisaCarregamento.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaCarregamento.AprovarTodos.visible(false);
    _pesquisaCarregamento.DelegarTodos.visible(false);
    _pesquisaCarregamento.RejeitarTodos.visible(false);

    var existemRegistrosSelecionados = _gridCarregamento.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaCarregamento.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoCarregamentoSolicitacaoUltimaPesquisa == EnumSituacaoCarregamentoSolicitacao.AguardandoAprovacao) {
            _pesquisaCarregamento.AprovarTodos.visible(true);
            _pesquisaCarregamento.DelegarTodos.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaCarregamento.RejeitarTodos.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharCarregamento(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaCarregamento);

    _carregamento.Codigo.val(registroSelecionado.Codigo);
    _carregamento.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_carregamento, "AutorizacaoCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoCarregamentoSolicitacao.AguardandoAprovacao);

                $("#" + _carregamento.Peso.id).css("background-color", retorno.Data.CorPeso);
                $("#" + _carregamento.Pallets.id).css("background-color", retorno.Data.CarPallets);
                $("#" + _carregamento.Cubagem.id).css("background-color", retorno.Data.CorCubagem);

                _carregamento.Cubagem.visible(retorno.Data.PossuiCubagem);
                _carregamento.Pallets.visible(retorno.Data.PossuiPallet);

                $modalDetalhesCarregamento.modal("show");
                $modalDetalhesCarregamento.one('hidden.bs.modal', function () {
                    limparCamposCarregamento();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposCarregamento() {
    $("#myTab a:first").tab("show");

    limparRegras();
}

function carregarCarregamentoUsuarioAcessadoViaLink(codigo) {
    _carregamento.Codigo.val(codigo);

    BuscarPorCodigo(_carregamento, "AutorizacaoCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoCarregamentoSolicitacao.AguardandoAprovacao);

                $("#" + _carregamento.Peso.id).css("background-color", retorno.Data.CorPeso);
                $("#" + _carregamento.Pallets.id).css("background-color", retorno.Data.CarPallets);
                $("#" + _carregamento.Cubagem.id).css("background-color", retorno.Data.CorCubagem);

                _carregamento.Cubagem.visible(retorno.Data.PossuiCubagem);
                _carregamento.Pallets.visible(retorno.Data.PossuiPallet);

                $modalDetalhesCarregamento.modal("show");
                $modalDetalhesCarregamento.one('hidden.bs.modal', function () {
                    limparCamposCarregamento();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}