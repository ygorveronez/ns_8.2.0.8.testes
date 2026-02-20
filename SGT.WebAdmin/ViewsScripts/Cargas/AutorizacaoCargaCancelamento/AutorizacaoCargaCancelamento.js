/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="Frete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoSolicitacaoFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaCancelamentoSolicitacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cargaCancelamento;
var _gridCargaCancelamento;
var _pesquisaCargaCancelamento;
var _rejeicao;
var _situacaoCargaCancelamentoSolicitacaoUltimaPesquisa = EnumSituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarCargasCancelamentoSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var CargaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:" });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.InformacaoTipoFreteEscolhido = PropertyEntity({ visible: ko.observable(false) });
    this.TipoFreteEscolhido = PropertyEntity({ val: ko.observable(0) });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular: " });
    this.Motoristas = PropertyEntity({ text: "Motoristas: " });
    this.Operador = PropertyEntity({ text: "Operador: " });
    this.Peso = PropertyEntity({ text: "Peso: " });
    this.MotivoCancelamento = PropertyEntity({ text: "Motivo Cancelamento: " });
    this.Placas = PropertyEntity({ text: "Placas: " });
    this.Rota = PropertyEntity({ text: "Rota: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.TipoCarga = PropertyEntity({ text: "Tipo da Carga: " });
    this.TipoOperacao = PropertyEntity({ text: "Operação: " });
    this.Transportador = PropertyEntity({ text: _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? "Transportador: " : "Empresa/Filial: " });
    this.ValorFrete = PropertyEntity({ text: "Valor do Frete: " });
    this.ValorTabela = PropertyEntity({ text: "Valor calculado pela Tabela: ", visible: ko.observable(false) });
    this.PercentualEmRelacaoValorFrete = PropertyEntity({ text: "Percentual em Relação a Tabela: ", visible: ko.observable(false) });

    this.VerDetalhesFreteCarga = PropertyEntity({ eventClick: exibirDetalhesFreteCargaClick, type: types.event, text: "Ver Detalhes", visible: ko.observable(false) });
};

var PesquisaCargaCancelamento = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "" });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.SituacaoCargaCancelamentoSolicitacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao), options: EnumSituacaoCargaCancelamentoSolicitacao.obterOpcoesPesquisa(), def: EnumSituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao, text: "Situação da Alteração do Frete: " });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasCargasCancelamentoClick, text: "Aprovar Cancelamentos de Cargas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCargaCancelamento, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasCargasCancelamentoClick, text: "Rejeitar Cancelamentos de Cargas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todos", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Cancelamentos de Cargas", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasCargasCancelamentoClick, text: "Reprocessar Cancelamentos de Cargas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#camposMultimodalPesquisa").hide();

    _cargaCancelamento = new CargaCancelamento();
    KoBindings(_cargaCancelamento, "knockoutCargaCancelamento");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoCargaCancelamento");

    _pesquisaCargaCancelamento = new PesquisaCargaCancelamento();
    KoBindings(_pesquisaCargaCancelamento, "knockoutPesquisaCargaCancelamento");

    loadGridCargaCancelamento();
    loadRegras();
    loadFrete();
    loadDelegar();

    new BuscarFilial(_pesquisaCargaCancelamento.Filial);
    new BuscarFuncionario(_pesquisaCargaCancelamento.Usuario);
    new BuscarPorto(_pesquisaCargaCancelamento.PortoOrigem);
    new BuscarPorto(_pesquisaCargaCancelamento.PortoDestino);
    new BuscarClientes(_pesquisaCargaCancelamento.Tomador);
    new BuscarTiposOperacao(_pesquisaCargaCancelamento.TipoOperacao);

    loadDadosUsuarioLogado(atualizarGridCargaCancelamento);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaCargaCancelamento.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaCargaCancelamento.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridCargaCancelamento() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharCargaCancelamento,
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
        SelecionarTodosKnout: _pesquisaCargaCancelamento.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoCargaCancelamento/ExportarPesquisa",
        titulo: "Autorização de Cancelamentos de Cargas"
    };

    _gridCargaCancelamento = new GridView(_pesquisaCargaCancelamento.Pesquisar.idGrid, "AutorizacaoCargaCancelamento/Pesquisa", _pesquisaCargaCancelamento, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasCargasCancelamentoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todos os cancelamentos de cargas selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCargaCancelamento);

        dados.SelecionarTodos = _pesquisaCargaCancelamento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargaCancelamento.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargaCancelamento.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCargaCancelamento/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridCargaCancelamento();
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

    Global.fecharModal("divModalRejeitarCargaCancelamento");
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal("divModalDelegarSelecionados");
}

function exibirDetalhesFreteCargaClick() {
    verificarFrete(_cargaCancelamento.CodigoCarga.val());
}

function rejeitarMultiplasCargasCancelamentoClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal("divModalRejeitarCargaCancelamento");
}

function rejeitarCargasCancelamentoSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todos os cancelamentos de cargas selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCargaCancelamento);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaCargaCancelamento.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargaCancelamento.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargaCancelamento.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCargaCancelamento/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridCargaCancelamento();
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

function reprocessarMultiplasCargasCancelamentoClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaCargaCancelamento);

    dados.SelecionarTodos = _pesquisaCargaCancelamento.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridCargaCancelamento.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridCargaCancelamento.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoCargaCancelamento/ReprocessarMultiplasCargas", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " cancelamentos de cargas foram reprocessados com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 cancelamento de carga foi reprocessado com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para os cancelamentos de cargas selecionados.");

                atualizarGridCargaCancelamento();
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

function atualizarGridCargaCancelamento() {
    _pesquisaCargaCancelamento.SelecionarTodos.val(false);
    _pesquisaCargaCancelamento.AprovarTodas.visible(false);
    _pesquisaCargaCancelamento.DelegarTodas.visible(false);
    _pesquisaCargaCancelamento.RejeitarTodas.visible(false);

    _gridCargaCancelamento.CarregarGrid();

    _situacaoCargaCancelamentoSolicitacaoUltimaPesquisa = _pesquisaCargaCancelamento.SituacaoCargaCancelamentoSolicitacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaCargaCancelamento.AprovarTodas.visible(false);
    _pesquisaCargaCancelamento.DelegarTodas.visible(false);
    _pesquisaCargaCancelamento.RejeitarTodas.visible(false);
    _pesquisaCargaCancelamento.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridCargaCancelamento.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaCargaCancelamento.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoCargaCancelamentoSolicitacaoUltimaPesquisa == EnumSituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao) {
            _pesquisaCargaCancelamento.AprovarTodas.visible(true);
            _pesquisaCargaCancelamento.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaCargaCancelamento.RejeitarTodas.visible(true);
        }
        else if (_situacaoCargaCancelamentoSolicitacaoUltimaPesquisa == EnumSituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao)
            _pesquisaCargaCancelamento.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharCargaCancelamento(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaCargaCancelamento);

    _cargaCancelamento.Codigo.val(registroSelecionado.Codigo);
    _cargaCancelamento.Usuario.val(pesquisa.Usuario);
    _cargaCancelamento.PercentualEmRelacaoValorFrete.visible(false);
    _cargaCancelamento.ValorTabela.visible(false);

    BuscarPorCodigo(_cargaCancelamento, "AutorizacaoCargaCancelamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _cargaCancelamento.InformacaoTipoFreteEscolhido.visible(retorno.Data.InformacaoTipoFreteEscolhido != "");
                _cargaCancelamento.VerDetalhesFreteCarga.visible(retorno.Data.ExibirDetalhesFreteCarga);

                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoCargaCancelamentoSolicitacao === EnumSituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao);
                ExibirPercentualFrete(retorno.Data);

                Global.abrirModal("divModalCargaCancelamento");

                $("#divModalCargaCancelamento").one('hidden.bs.modal', function () {
                    limparCamposCarga();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    }, null);
}

function ExibirPercentualFrete(dado) {
    if (dado.TipoFreteEscolhido != EnumTipoFreteEscolhido.todos && dado.TipoFreteEscolhido != EnumTipoFreteEscolhido.Tabela && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        VerificarFrete(function (retornoFrete) {
            if (retornoFrete.situacao == EnumSituacaoRetornoDadosFrete.FreteValido) {
                var valorFrete = 0;
                var valorTabela = 0;
                var tipoFreteEscolhido = _cargaCancelamento.TipoFreteEscolhido.val();

                if (tipoFreteEscolhido != EnumTipoFreteEscolhido.todos && tipoFreteEscolhido != EnumTipoFreteEscolhido.Tabela) {
                    valorTabela = retornoFrete.valorFreteTabelaFrete;

                    if (_CONFIGURACAO_TMS.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga) {
                        valorTabela -= (valorTabela * (retornoFrete.aliquotaICMS / 100)) + (valorTabela * (retornoFrete.aliquotaISS / 100));
                        valorTabela -= BuscarValorTotalComponentes(retornoFrete);
                    }

                    if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Embarcador)
                        valorFrete = retornoFrete.valorFreteEmbarcador;
                    else if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Operador)
                        valorFrete = retornoFrete.valorFreteOperador;
                    else if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Leilao)
                        valorFrete = retornoFrete.valorFreteLeilao;
                    else if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Cliente)
                        valorFrete = retornoFrete.valorFreteContratoFrete;
                }

                if (valorTabela > 0) {
                    var percentual = 0;

                    if (_CONFIGURACAO_TMS.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga)
                        percentual = ((valorFrete - valorTabela) * 100) / valorTabela;
                    else {
                        var maiorValor = 0;
                        var menorValor = 0;
                        var fator = 1;

                        if (valorFrete >= valorTabela) {
                            fator = -1;
                            maiorValor = valorFrete;
                            menorValor = valorTabela;
                        }
                        else {
                            maiorValor = valorTabela;
                            menorValor = valorFrete;
                        }

                        percentual = (((menorValor * 100) / maiorValor) - 100) * fator;
                    }

                    _cargaCancelamento.PercentualEmRelacaoValorFrete.val(Globalize.format(percentual, "n2") + "%").visible(true);
                    _cargaCancelamento.ValorTabela.val(Globalize.format(valorTabela, "n2")).visible(true);
                }
            }
        });
    }
}

function VerificarFrete(callback) {
    executarReST("CargaFrete/VerificarFrete", { Codigo: _cargaCancelamento.CodigoCarga.val() }, function (arg) {
        if (arg.Success) {
            if (callback != null)
                callback(arg.Data);

        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
    });
}

function limparCamposCarga() {
    $("#myTab a:first").tab("show");

    limparRegras();
}
