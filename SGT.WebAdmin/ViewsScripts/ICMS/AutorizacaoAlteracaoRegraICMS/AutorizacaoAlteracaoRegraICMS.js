/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoRegraICMS.js" />

// #region Objetos Globais do Arquivo

var _regraICMS;
var _gridRegrasICMS;
var _pesquisaRegraICMS;
var _rejeicao;
var _situacaoAlteracaoRegraICMSUltimaPesquisa = EnumSituacaoAlteracaoRegraICMS.AguardandoAprovacao;
var $modalDetalhesRegraICMS;

// #endregion Objetos Globais do Arquivo

// #region Classes

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarRegrasICMSSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var RegraICMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.CNPJ = PropertyEntity({ text: "CNPJ: " });
    this.RazaoSocial = PropertyEntity({ text: "Razão Social: " });
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: " });
    this.UFEmitente = PropertyEntity({ text: "UF Emitente: " });
    this.Origem = PropertyEntity({ text: "Origem: " });
    this.Destino = PropertyEntity({ text: "Destino: " });
    this.Tomador = PropertyEntity({ text: "Tomador: " });
    this.VigenciaInicio = PropertyEntity({ text: "Vigência Início: " });
    this.VigenciaFim = PropertyEntity({ text: "Vigência Fim: " });
    this.DescricaoRegra = PropertyEntity({ text: "Regra: " });

    this.CST = PropertyEntity({ text: "CST: " });
    this.CFOP = PropertyEntity({ text: "CFOP: " });
    this.Aliquota = PropertyEntity({ text: "Aliquota: " });
    this.ZerarValorICMS = PropertyEntity({ text: "Zerar base de calculo do ICMS: " });
    this.NaoImprimirImpostosDACTE = PropertyEntity({ text: "Não imprimir os impostos no DACTE: " });
    this.NaoEnviarImpostoICMSNaEmissaoCte = PropertyEntity({ text: "Não Enviar imposto ICMS na emissão de CTe: " });
    this.NaoIncluirICMSValorFrete = PropertyEntity({ text: "Não incluir ICMS no valor do frete: " });
};

var PesquisaRegraICMS = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, maxlength: 200 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoRegraICMS.AguardandoAprovacao), options: EnumSituacaoAlteracaoRegraICMS.obterOpcoesPesquisaAutorizacao(), def: EnumSituacaoAlteracaoRegraICMS.AguardandoAprovacao, text: "Situação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasRegrasICMSClick, text: "Aprovar Configurações", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridRegrasICMS, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasRegrasICMSClick, text: "Rejeitar Configurações", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Configurações", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasRegrasICMSClick, text: "Reprocessar Configurações", visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAutorizacao() {
    buscarDetalhesOperador(function () {
        _regraICMS = new RegraICMS();
        KoBindings(_regraICMS, "knockoutRegraICMS");

        _rejeicao = new RejeitarSelecionados();
        KoBindings(_rejeicao, "knockoutRejeicaoRegraICMS");

        _pesquisaRegraICMS = new PesquisaRegraICMS();
        KoBindings(_pesquisaRegraICMS, "knockoutPesquisaRegraICMS");

        loadGridRegrasICMS();
        loadRegras();
        loadDelegar();

        $modalDetalhesRegraICMS = $("#divModalRegraICMS");

        new BuscarFuncionario(_pesquisaRegraICMS.Usuario);

        loadDadosUsuarioLogado(atualizarGridRegrasICMS);
    });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaRegraICMS.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaRegraICMS.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridRegrasICMS() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharRegraICMS, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDetalhes] };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaRegraICMS.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoAlteracaoRegraICMS/ExportarPesquisa",
        titulo: "Autorização de Configuração de Regra para ICMS"
    };

    _gridRegrasICMS = new GridView(_pesquisaRegraICMS.Pesquisar.idGrid, "AutorizacaoAlteracaoRegraICMS/Pesquisa", _pesquisaRegraICMS, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplasRegrasICMSClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as configurações de regra para ICMS selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaRegraICMS);

        dados.SelecionarTodos = _pesquisaRegraICMS.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridRegrasICMS.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridRegrasICMS.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAlteracaoRegraICMS/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridRegrasICMS();
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

    Global.fecharModal('divModalRejeitarRegraICMS');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal('divModalDelegarSelecionados');
}

function rejeitarMultiplasRegrasICMSClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarRegraICMS');
}

function rejeitarRegrasICMSSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as configurações de regra para ICMS selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaRegraICMS);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaRegraICMS.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridRegrasICMS.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridRegrasICMS.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoAlteracaoRegraICMS/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridRegrasICMS();
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

function reprocessarMultiplasRegrasICMSClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaRegraICMS);

    dados.SelecionarTodos = _pesquisaRegraICMS.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridRegrasICMS.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridRegrasICMS.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoAlteracaoRegraICMS/ReprocessarMultiplasRegrasICMS", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " configurações de regra para ICMS foram reprocessadas com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 configuração de regra para ICMS foi reprocessada com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para as configurações de regra para ICMS selecionadas.");

                atualizarGridRegrasICMS();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções

function atualizarGridRegrasICMS() {
    _pesquisaRegraICMS.SelecionarTodos.val(false);
    _pesquisaRegraICMS.AprovarTodas.visible(false);
    _pesquisaRegraICMS.DelegarTodas.visible(false);
    _pesquisaRegraICMS.RejeitarTodas.visible(false);

    _gridRegrasICMS.CarregarGrid();

    _situacaoAlteracaoRegraICMSUltimaPesquisa = _pesquisaRegraICMS.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaRegraICMS.AprovarTodas.visible(false);
    _pesquisaRegraICMS.DelegarTodas.visible(false);
    _pesquisaRegraICMS.RejeitarTodas.visible(false);
    _pesquisaRegraICMS.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridRegrasICMS.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaRegraICMS.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoAlteracaoRegraICMSUltimaPesquisa == EnumSituacaoAlteracaoRegraICMS.AguardandoAprovacao) {
            _pesquisaRegraICMS.AprovarTodas.visible(true);
            _pesquisaRegraICMS.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaRegraICMS.RejeitarTodas.visible(true);
        }
        else if (_situacaoAlteracaoRegraICMSUltimaPesquisa == EnumSituacaoAlteracaoRegraICMS.SemRegraAprovacao)
            _pesquisaRegraICMS.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharRegraICMS(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaRegraICMS);

    _regraICMS.Codigo.val(registroSelecionado.Codigo);
    _regraICMS.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_regraICMS, "AutorizacaoAlteracaoRegraICMS/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAlteracaoRegraICMS.AguardandoAprovacao);

                $modalDetalhesRegraICMS.modal("show");
                $modalDetalhesRegraICMS.one('hidden.bs.modal', function () {
                    limparCamposRegraICMS();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function limparCamposRegraICMS() {
    $("#myTab a:first").tab("show");

    limparRegras();
}

// #endregion Funções
