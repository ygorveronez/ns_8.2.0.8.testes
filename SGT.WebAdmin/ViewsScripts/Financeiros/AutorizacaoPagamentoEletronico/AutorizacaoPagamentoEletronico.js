/// <reference path="AutorizarRegras.js" />
/// <reference path="Titulos.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAutorizacaoPagamentoEletronico.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPagamentosEletronico;
var _pagamentoEletronico;
var _pesquisaPagamentoEletronico;
var _rejeicao;
var _situacaoPagamentoEletronicoUltimaPesquisa = EnumSituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao;
var $modalDetalhesPagamentoEletronico;
var _modalPagamentoEletronico;
var _modalRejeitarPagamentoEletronico;
var _modalDelegarSelecionados;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarPagamentosEletronicoSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var PagamentoEletronico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Data = PropertyEntity({ text: "Data: ", visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.BoletoConfiguracao = PropertyEntity({ text: "Config. Boleto: ", visible: ko.observable(true) });
    this.DataGeracao = PropertyEntity({ text: "Data Geração: ", visible: ko.observable(true) });
    this.Modalidade = PropertyEntity({ text: "Modalidade: ", visible: ko.observable(true) });
    this.TipoConta = PropertyEntity({ text: "Tipo da Conta: ", visible: ko.observable(true) });
    this.Finalidade = PropertyEntity({ text: "Finalidade: ", visible: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", visible: ko.observable(true) });
    this.QtdTitulos = PropertyEntity({ text: "Qtd. Títulos: ", visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "Empresa: ", visible: ko.observable(true) });
};

var PesquisaPagamentoEletronico = function () {
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao), options: EnumSituacaoAutorizacaoPagamentoEletronico.obterOpcoesPesquisa(), def: EnumSituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao, text: "Situação: " });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Boleto Config.:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasPagamentosEletronicoClick, text: "Aprovar Pagamento Eletrônico", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridPagamentosEletronico, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasPagamentosEletronicoClick, text: "Rejeitar Pagamento Eletrônico", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Pagamento Eletrônico", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _pagamentoEletronico = new PagamentoEletronico();
    KoBindings(_pagamentoEletronico, "knockoutPagamentoEletronico");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoPagamentoEletronico");

    _pesquisaPagamentoEletronico = new PesquisaPagamentoEletronico();
    KoBindings(_pesquisaPagamentoEletronico, "knockoutPesquisaPagamentoEletronico");

    loadGridPagamentosEletronico();
    loadRegras();
    loadTitulos();

    $modalDetalhesPagamentoEletronico = $("#divModalPagamentoEletronico");

    new BuscarBoletoConfiguracao(_pesquisaPagamentoEletronico.BoletoConfiguracao);
    new BuscarFuncionario(_pesquisaPagamentoEletronico.Usuario);

    loadDadosUsuarioLogado(atualizarGridPagamentosEletronico);
    _modalPagamentoEletronico = new bootstrap.Modal(document.getElementById("divModalPagamentoEletronico"), { backdrop: true, keyboard: true });
    _modalRejeitarPagamentoEletronico = new bootstrap.Modal(document.getElementById("divModalRejeitarPagamentoEletronico"), { backdrop: true, keyboard: true });
    _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaPagamentoEletronico.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaPagamentoEletronico.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridPagamentosEletronico() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharPagamentoEletronico,
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
        SelecionarTodosKnout: _pesquisaPagamentoEletronico.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoPagamentoEletronico/ExportarPesquisa",
        titulo: "Autorização Ordem de Serviço"
    };

    _gridPagamentosEletronico = new GridView(_pesquisaPagamentoEletronico.Pesquisar.idGrid, "AutorizacaoPagamentoEletronico/Pesquisa", _pesquisaPagamentoEletronico, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasPagamentosEletronicoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os Pagamentos Eltrônicos selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamentoEletronico);

        dados.SelecionarTodos = _pesquisaPagamentoEletronico.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridPagamentosEletronico.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridPagamentosEletronico.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamentoEletronico/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");

                        if (retorno.Data.Codigos != null && retorno.Data.Codigos != undefined) {
                            for (var i = 0; i < retorno.Data.Codigos.length; i++) {
                                var data = {
                                    Codigo: retorno.Data.Codigos[i],
                                };
                                //setTimeout(function () {                                    
                                executarDownload("PagamentoDigital/DownloadArquivoRemessa", data);
                                //}, 1000);
                                //setTimeout(function () {                                   
                                //    executarDownload("PagamentoDigital/DownloadRelatorioRemessa", data);
                                //}, 1000);
                            }
                        }
                    }
                    else if (retorno.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    atualizarGridPagamentosEletronico();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarPagamentoEletronico.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplasPagamentosEletronicoClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarPagamentoEletronico.show();
}

function rejeitarPagamentosEletronicoSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os Pagamentos Eletrônicos selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPagamentoEletronico);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaPagamentoEletronico.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridPagamentosEletronico.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridPagamentosEletronico.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoPagamentoEletronico/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridPagamentosEletronico();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}

/*
 * Declaração das Funções
 */

function atualizarGridPagamentosEletronico() {
    _pesquisaPagamentoEletronico.SelecionarTodos.val(false);
    _pesquisaPagamentoEletronico.AprovarTodas.visible(false);
    _pesquisaPagamentoEletronico.DelegarTodas.visible(false);
    _pesquisaPagamentoEletronico.RejeitarTodas.visible(false);

    _gridPagamentosEletronico.CarregarGrid();

    _situacaoPagamentoEletronicoUltimaPesquisa = _pesquisaPagamentoEletronico.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaPagamentoEletronico.AprovarTodas.visible(false);
    _pesquisaPagamentoEletronico.DelegarTodas.visible(false);
    _pesquisaPagamentoEletronico.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridPagamentosEletronico.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaPagamentoEletronico.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoPagamentoEletronicoUltimaPesquisa == EnumSituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao) {
            _pesquisaPagamentoEletronico.AprovarTodas.visible(true);
            _pesquisaPagamentoEletronico.DelegarTodas.visible(false);
            _pesquisaPagamentoEletronico.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    //if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
    //    $("#liDelegar").show();
    //else
    //    $("#liDelegar").hide();
}

function detalharPagamentoEletronico(registroSelecionado) {
    limparCamposPagamentoEletronico();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaPagamentoEletronico);

    _pagamentoEletronico.Codigo.val(registroSelecionado.Codigo);
    _pagamentoEletronico.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_pagamentoEletronico, "AutorizacaoPagamentoEletronico/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                AtualizarGridTitulos();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao);

                _modalPagamentoEletronico.show();
                $modalDetalhesPagamentoEletronico.one('hidden.bs.modal', function () {
                    limparCamposPagamentoEletronico();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposPagamentoEletronico() {
    $("#myTab a:first").tab("show");

    limparRegras();
    limparTitulos();
}