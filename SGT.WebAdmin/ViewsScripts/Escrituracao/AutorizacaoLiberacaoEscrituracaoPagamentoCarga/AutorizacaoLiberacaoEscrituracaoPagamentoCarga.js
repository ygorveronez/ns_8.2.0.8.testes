/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _carga;
var _gridCargas;
var _pesquisaCarga;
var _rejeicao;
var _situacaoLiberacaoEscrituracaoPagamentoCargaUltimaPesquisa = EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao;
var $modalDetalhesCarga;
var _modalCarga;
var _modalRejeitarCarga;
var _modalDelegarSelecionados;
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
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular: " });
    this.Motoristas = PropertyEntity({ text: "Motoristas: " });
    this.Operador = PropertyEntity({ text: "Operador: " });
    this.Peso = PropertyEntity({ text: "Peso: " });
    this.Placas = PropertyEntity({ text: "Placas: " });
    this.Rota = PropertyEntity({ text: "Rota: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: " });
    this.TipoOperacao = PropertyEntity({ text: "Operação: " });
    this.Transportador = PropertyEntity({ text: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? "Transportador: " : "Empresa/Filial: " });
    this.ValorFrete = PropertyEntity({ text: "Valor do Frete: " });
};

var PesquisaCarga = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "" });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.SituacaoLiberacaoEscrituracaoPagamentoCarga = PropertyEntity({ val: ko.observable(EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao), options: EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.obterOpcoesPesquisaAutorizacao(), def: EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao, text: "Situação da Liberação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasCargasClick, text: "Aprovar Cargas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCargas, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasCargasClick, text: "Rejeitar Cargas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Cargas", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasCargasClick, text: "Reprocessar Cargas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    buscarDetalhesOperador(function () {
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

        new BuscarFilial(_pesquisaCarga.Filial);
        new BuscarFuncionario(_pesquisaCarga.Usuario);

        loadDadosUsuarioLogado(atualizarGridCargas);
        _modalCarga = new bootstrap.Modal(document.getElementById("divModalCarga"), { backdrop: true, keyboard: true });
        _modalRejeitarCarga = new bootstrap.Modal(document.getElementById("divModalRejeitarCarga"), { backdrop: true, keyboard: true });
        _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
    });
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
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharCarga, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    var multiplaEscolha = {
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

    var configuracaoExportacao = {
        url: "AutorizacaoLiberacaoEscrituracaoPagamentoCarga/ExportarPesquisa",
        titulo: "Autorização de Liberação de Escrituração e Pagamento de Cargas"
    };

    _gridCargas = new GridView(_pesquisaCarga.Pesquisar.idGrid, "AutorizacaoLiberacaoEscrituracaoPagamentoCarga/Pesquisa", _pesquisaCarga, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasCargasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as cargas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCarga);

        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoLiberacaoEscrituracaoPagamentoCarga/AprovarMultiplosItens", dados, function (retorno) {
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

    _modalRejeitarCarga.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplasCargasClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarCarga.show();
}

function rejeitarCargasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as cargas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCarga);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoLiberacaoEscrituracaoPagamentoCarga/ReprovarMultiplosItens", dados, function (retorno) {
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
    dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoLiberacaoEscrituracaoPagamentoCarga/ReprocessarMultiplasCargas", dados, function (retorno) {
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

    _gridCargas.CarregarGrid();

    _situacaoLiberacaoEscrituracaoPagamentoCargaUltimaPesquisa = _pesquisaCarga.SituacaoLiberacaoEscrituracaoPagamentoCarga.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaCarga.AprovarTodas.visible(false);
    _pesquisaCarga.DelegarTodas.visible(false);
    _pesquisaCarga.RejeitarTodas.visible(false);
    _pesquisaCarga.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridCargas.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaCarga.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoLiberacaoEscrituracaoPagamentoCargaUltimaPesquisa == EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao) {
            _pesquisaCarga.AprovarTodas.visible(true);
            _pesquisaCarga.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaCarga.RejeitarTodas.visible(true);
        }
        else if (_situacaoLiberacaoEscrituracaoPagamentoCargaUltimaPesquisa == EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao)
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
    var pesquisa = RetornarObjetoPesquisa(_pesquisaCarga);

    _carga.Codigo.val(registroSelecionado.Codigo);
    _carga.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_carga, "AutorizacaoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoLiberacaoEscrituracaoPagamentoCarga === EnumSituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao);

                _modalCarga.show();
                $modalDetalhesCarga.one('hidden.bs.modal', function () {
                    limparCamposCarga();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposCarga() {
    $("#myTab a:first").tab("show");

    limparRegras();
}