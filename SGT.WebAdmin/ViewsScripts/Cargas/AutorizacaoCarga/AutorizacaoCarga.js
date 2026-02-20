/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="Frete.js" />
/// <reference path="SolicitacaoFrete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoSolicitacaoFrete.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Porto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoFreteCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _carga;
var _gridCargas;
var _pesquisaCarga;
var _rejeicao;
var _situacaoAlteracaoFreteCargaUltimaPesquisa = EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao;
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
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.InformacaoTipoFreteEscolhido = PropertyEntity({ visible: ko.observable(false) });
    this.TipoFreteEscolhido = PropertyEntity({ val: ko.observable(0) });
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
    this.ValorTabela = PropertyEntity({ text: "Valor calculado pela Tabela: ", visible: ko.observable(false) });
    this.PercentualEmRelacaoValorFrete = PropertyEntity({ text: "Percentual em Relação a Tabela: ", visible: ko.observable(false) });

    this.VerDetalhesFreteCarga = PropertyEntity({ eventClick: exibirDetalhesFreteCargaClick, type: types.event, text: "Ver Detalhes", visible: ko.observable(false) });

    this.DetalhesCarga = PropertyEntity({ eventClick: detalhesCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes, visible: ko.observable(true) });
};

var PesquisaCarga = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", val: ko.observable(""), def: "" });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", getType: typesKnockout.date });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.MotivoSolicitacaoFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motivo da Solicitação de Frete:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.SituacaoAlteracaoFrete = PropertyEntity({ val: ko.observable(EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao), options: EnumSituacaoAlteracaoFreteCarga.obterOpcoesPesquisaAutorizacao(), def: EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao, text: "Situação da Alteração do Frete: " });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS) });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: TestarJustificativaCustoExtraMultiplasCargas, text: "Aprovar Cargas", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCargas, text: "Pesquisar", idGrid: "grid-pesquisa-autorizacao-carga", visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasCargasClick, text: "Rejeitar Cargas", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Cargas", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#camposMultimodalPesquisa").hide();
    
    _carga = new Carga();
    KoBindings(_carga, "knockoutCarga");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoCarga");

    _pesquisaCarga = new PesquisaCarga();
    KoBindings(_pesquisaCarga, "knockoutPesquisaCarga");

    loadGridCargas();
    loadSolicitacaoFrete();
    loadRegras();
    loadFrete();
    loadDelegar();
    loadDetalhePedido();

    $modalDetalhesCarga = $("#divModalCarga");

    new BuscarFilial(_pesquisaCarga.Filial);
    new BuscarMotivoSolicitacaoFrete(_pesquisaCarga.MotivoSolicitacaoFrete);
    new BuscarFuncionario(_pesquisaCarga.Usuario);
    new BuscarPorto(_pesquisaCarga.PortoOrigem);
    new BuscarPorto(_pesquisaCarga.PortoDestino);
    new BuscarClientes(_pesquisaCarga.Tomador);
    new BuscarTiposOperacao(_pesquisaCarga.TipoOperacao);

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
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharCarga,
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
        SelecionarTodosKnout: _pesquisaCarga.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoCarga/ExportarPesquisa",
        titulo: "Autorização Cargas"
    };

    _gridCargas = new GridView(_pesquisaCarga.Pesquisar.idGrid, "AutorizacaoCarga/Pesquisa", _pesquisaCarga, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
    _gridCargas.SetPermitirEdicaoColunas(true);
    _gridCargas.SetSalvarPreferenciasGrid(true);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasCargas() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as cargas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCarga);

        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCarga/AprovarMultiplosItens", dados, function (retorno) {
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

function exibirDetalhesFreteCargaClick() {
    verificarFrete(_carga.Codigo.val());
}

function rejeitarMultiplasCargasClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal('divModalRejeitarCarga');
}

function rejeitarCargasSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as cargas selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaCarga);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaCarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridCargas.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridCargas.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCarga/ReprovarMultiplosItens", dados, function (retorno) {
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

/*
 * Declaração das Funções
 */

function atualizarGridCargas() {
    _pesquisaCarga.SelecionarTodos.val(false);
    _pesquisaCarga.AprovarTodas.visible(false);
    _pesquisaCarga.DelegarTodas.visible(false);
    _pesquisaCarga.RejeitarTodas.visible(false);

    _gridCargas.CarregarGrid();

    _situacaoAlteracaoFreteCargaUltimaPesquisa = _pesquisaCarga.SituacaoAlteracaoFrete.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaCarga.AprovarTodas.visible(false);
    _pesquisaCarga.DelegarTodas.visible(false);
    _pesquisaCarga.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridCargas.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaCarga.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoAlteracaoFreteCargaUltimaPesquisa == EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao) {
            _pesquisaCarga.AprovarTodas.visible(true);
            _pesquisaCarga.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaCarga.RejeitarTodas.visible(true);
        }
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
    _carga.PercentualEmRelacaoValorFrete.visible(false);
    _carga.ValorTabela.visible(false);

    BuscarPorCodigo(_carga, "AutorizacaoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _carga.InformacaoTipoFreteEscolhido.visible(retorno.Data.InformacaoTipoFreteEscolhido != "");
                _carga.VerDetalhesFreteCarga.visible(retorno.Data.ExibirDetalhesFreteCarga);

                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoAlteracaoFreteCarga === EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao);
                preencherSolicitacaoFrete(retorno.Data.SolicitacaoFrete);
                ExibirPercentualFrete(retorno.Data);

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

function ExibirPercentualFrete(dado) {
    if (dado.TipoFreteEscolhido != EnumTipoFreteEscolhido.todos && dado.TipoFreteEscolhido != EnumTipoFreteEscolhido.Tabela && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        VerificarFrete(function (retornoFrete) {
            if (retornoFrete.situacao == EnumSituacaoRetornoDadosFrete.FreteValido) {
                var valorFrete = 0;
                var valorTabela = 0;
                var tipoFreteEscolhido = _carga.TipoFreteEscolhido.val();

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

                    _carga.PercentualEmRelacaoValorFrete.val(Globalize.format(percentual, "n2") + "%").visible(true);
                    _carga.ValorTabela.val(Globalize.format(valorTabela, "n2")).visible(true);
                }
            }
        });
    }
}

function VerificarFrete(callback) {
    var data = { Codigo: _carga.Codigo.val() };
    executarReST("CargaFrete/VerificarFrete", data, function (arg) {
        if (arg.Success) {
            if (callback != null)
                callback(arg.Data);

        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function limparCamposCarga() {
    $("#myTab a:first").tab("show");

    limparRegras();
    limparSolicitacaoFrete();
}

function carregarCargaUsuarioAcessadoViaLink(codigoCarga) {
    _carga.Codigo.val(codigoCarga);

    BuscarPorCodigo(_carga, "AutorizacaoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _carga.InformacaoTipoFreteEscolhido.visible(retorno.Data.InformacaoTipoFreteEscolhido != "");
                _carga.VerDetalhesFreteCarga.visible(retorno.Data.ExibirDetalhesFreteCarga);

                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.SituacaoAlteracaoFreteCarga === EnumSituacaoAlteracaoFreteCarga.AguardandoAprovacao);
                preencherSolicitacaoFrete(retorno.Data.SolicitacaoFrete);
                ExibirPercentualFrete(retorno.Data);

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