/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoNaoConformidade.js" />
/// <reference path="../../Enumeradores/EnumTipoRegraNaoConformidade.js" />
/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />

// #region Objetos Globais do Arquivo

var _naoConformidade;
var _gridNaoConformidade;
var _pesquisaNaoConformidade;
var _rejeicao;
var _situacaoNaoConformidadeUltimaPesquisa = EnumSituacaoNaoConformidade.AguardandoTratativa;
var _modalDelegarSelecionados;
var _modalRejeitarNaoConformidade;
var _usuarioPermissaoAprovarNaoConformidade;
var _codigoUsuarioLogado;
var _usuarioLogadoPossuiAprovacao;
var $modalDetalhesNaoConformidade;

// #endregion Objetos Globais do Arquivo

// #region Classes

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarNaoConformidadesSelecionadosClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var NaoConformidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoNaoConformidade.AguardandoTratativa), options: EnumSituacaoNaoConformidade.obterOpcoes(), def: EnumSituacaoNaoConformidade.AguardandoTratativa });

    this.TipoRegraNaoConformidade = PropertyEntity({ val: ko.observable(), def: 0 });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Número da Nota Fiscal:" });
    this.ItemNaoConformidadeDescricao = PropertyEntity({ text: "Item de Não Conformidade:" });
    this.TipoParticipante = PropertyEntity({ text: "Participante:" });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação:" });
    this.PermitirAjustar = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.VincularProdutosFornecedores = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: abrirModalProdutosNota, text: "Vincular produtos fornecedores", visible: ko.observable(false) });

};

var PesquisaNaoConformidade = function () {
    this.NumeroNotaFiscal = PropertyEntity({ text: "Número da Nota Fiscal:", val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoNaoConformidade.AguardandoTratativa), options: EnumSituacaoNaoConformidade.obterOpcoesPesquisa(), def: EnumSituacaoNaoConformidade.AguardandoTratativa, text: "Situação: " });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.DataInicialGeracaoNC = PropertyEntity({ text: "Data Inicial da Geração da NC:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true) });
    this.DataFinalGeracaoNC = PropertyEntity({ text: "Data Final da Geração da NC:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true) });
    this.DataInicialEmissaoNotaFiscal = PropertyEntity({ text: "Data Inicial da Emissão da Nota Fiscal:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true) });
    this.DataFinalEmissaoNotaFiscal = PropertyEntity({ text: "Data Final da Emissão da Nota Fiscal:", getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(true) });
    this.NumeroCarga = PropertyEntity({ codEntity: ko.observable(0), text: "Número da Carga", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ codEntity: ko.observable(0), text: "Número do Pedido Embarcador", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Filial = PropertyEntity({ codEntity: ko.observable(0), text: "Filial", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.ItemNC = PropertyEntity({ codEntity: ko.observable(0), text: "Item de NC", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.NumeroNotas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Número Notas", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor", visible: ko.observable(true), val: ko.observable("") });
    this.NumeroOrdem = PropertyEntity({ codEntity: ko.observable(0), text: "Número da Ordem", visible: ko.observable(true), val: ko.observable("") });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasNaoConformidadesClick, text: "Aprovar Não Conformidades", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridNaoConformidade, text: "Pesquisar", idGrid: "grid-pesquisa-nao-conformidade", visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasNaoConformidadesClick, text: "Rejeitar Não Conformidades", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Não Conformidades", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasNaoConformidadesClick, text: "Reprocessar Não Conformidades", visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAutorizacao() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            loadDadosUsuarioLogado(function () {
                _naoConformidade = new NaoConformidade();
                KoBindings(_naoConformidade, "knockoutNaoConformidade");

                _rejeicao = new RejeitarSelecionados();
                KoBindings(_rejeicao, "knockoutRejeicaoNaoConformidade");

                _pesquisaNaoConformidade = new PesquisaNaoConformidade();
                KoBindings(_pesquisaNaoConformidade, "knockoutPesquisaNaoConformidade", false);

                loadGridNaoConformidade();
                loadRegras();
                loadAjuste();
                loadDelegar();
                loadAnexo();
                loadVincularProdutoFornecedor();

                $modalDetalhesNaoConformidade = $("#divModalNaoConformidade");

                new BuscarFuncionario(_pesquisaNaoConformidade.Usuario);
                new BuscarCargas(_pesquisaNaoConformidade.NumeroCarga);
                new BuscarPedidos(_pesquisaNaoConformidade.NumeroPedidoEmbarcador);
                new BuscarTransportadores(_pesquisaNaoConformidade.Transportador);
                new BuscarFilial(_pesquisaNaoConformidade.Filial);
                new BuscarNaoConformidade(_pesquisaNaoConformidade.ItemNC);
                new BuscarXMLNotaFiscal(_pesquisaNaoConformidade.NumeroNotas);
                new BuscarMotoristas(_pesquisaNaoConformidade.Motorista);
                new BuscarClientes(_pesquisaNaoConformidade.Destino);
                new BuscarClientes(_pesquisaNaoConformidade.Fornecedor);

                atualizarGridNaoConformidade();

                _modalDelegarSelecionados = new bootstrap.Modal(document.getElementById("divModalDelegarSelecionados"), { backdrop: true, keyboard: true });
                _modalRejeitarNaoConformidade = new bootstrap.Modal(document.getElementById("divModalRejeitarNaoConformidade"), { backdrop: true, keyboard: true });
            });
        });
    });
}

function loadDadosUsuarioLogado(callback) {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _usuarioPermissaoAprovarNaoConformidade = retorno.Data.PermitirAprovarNaoConformidade;
            _codigoUsuarioLogado = retorno.Data.Codigo;

            if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario) {
                _pesquisaNaoConformidade.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaNaoConformidade.Usuario.val(retorno.Data.Nome);
            }

            callback();
        }
    });
}

function loadGridNaoConformidade() {
    var opcaoDetalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharNaoConformidade, icone: "" };
    var opcaoDadosCarga = { descricao: "Dados da Carga", id: guid(), evento: "onclick", metodo: exibirDadosCarga, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [opcaoDadosCarga, opcaoDetalhes]
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaNaoConformidade.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "AutorizacaoNaoConformidade/ExportarPesquisa",
        titulo: "Autorização de Não Conformidades"
    };

    _gridNaoConformidade = new GridView(_pesquisaNaoConformidade.Pesquisar.idGrid, "AutorizacaoNaoConformidade/Pesquisa", _pesquisaNaoConformidade, menuOpcoes, null, 25, null, null, null, multiplaEscolha, 10000, null, configuracaoExportacao);
    _gridNaoConformidade.SetPermitirEdicaoColunas(true);
    _gridNaoConformidade.SetSalvarPreferenciasGrid(true);
    _gridNaoConformidade.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function aprovarMultiplasNaoConformidadesClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as não conformidades selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaNaoConformidade);

        dados.SelecionarTodos = _pesquisaNaoConformidade.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridNaoConformidade.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridNaoConformidade.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoNaoConformidade/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridNaoConformidade();
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

    _modalRejeitarNaoConformidade.hide();
}

function exibirDelegarSelecionadosClick() {
    _modalDelegarSelecionados.show();
}

function rejeitarMultiplasNaoConformidadesClick() {
    LimparCampos(_rejeicao);

    _modalRejeitarNaoConformidade.show();
}

function rejeitarNaoConformidadesSelecionadosClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as não conformidades selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaNaoConformidade);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaNaoConformidade.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridNaoConformidade.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridNaoConformidade.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoNaoConformidade/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridNaoConformidade();
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

function reprocessarMultiplasNaoConformidadesClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaNaoConformidade);

    dados.SelecionarTodos = _pesquisaNaoConformidade.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridNaoConformidade.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridNaoConformidade.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoNaoConformidade/ReprocessarMultiplasNaoConformidades", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " não conformidades foram reprocessadas com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 não conformidade foi reprocessada com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para as não conformidades selecionadas.");

                atualizarGridNaoConformidade();
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

function atualizarGridNaoConformidade() {
    _pesquisaNaoConformidade.SelecionarTodos.val(false);

    _gridNaoConformidade.AtualizarRegistrosSelecionados([]);
    _gridNaoConformidade.CarregarGrid();

    _situacaoNaoConformidadeUltimaPesquisa = _pesquisaNaoConformidade.Situacao.val();

    exibirMultiplasOpcoes();
}

function exibirMultiplasOpcoes() {
    _pesquisaNaoConformidade.AprovarTodas.visible(false);
    _pesquisaNaoConformidade.DelegarTodas.visible(false);
    _pesquisaNaoConformidade.RejeitarTodas.visible(false);
    _pesquisaNaoConformidade.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridNaoConformidade.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaNaoConformidade.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoNaoConformidadeUltimaPesquisa == EnumSituacaoNaoConformidade.AguardandoTratativa) {
            _pesquisaNaoConformidade.AprovarTodas.visible(permiteAprovar());
            _pesquisaNaoConformidade.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaNaoConformidade.RejeitarTodas.visible(_usuarioPermissaoAprovarNaoConformidade);
        }
        else if (_situacaoNaoConformidadeUltimaPesquisa == EnumSituacaoNaoConformidade.SemRegraAprovacao)
            _pesquisaNaoConformidade.ReprocessarTodas.visible(true);
    }
}

function controlarExibicaoAbas(dadosNaoConformidade) {
    if ((dadosNaoConformidade.Situacao === EnumSituacaoNaoConformidade.AguardandoTratativa) && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();

    if (dadosNaoConformidade.PermitirAjustar && (dadosNaoConformidade.TipoRegra === EnumTipoRegraNaoConformidade.PesoLiquidoTotal) && _usuarioLogadoPossuiAprovacao)
        $("#liAjustarPeso").show();
    else
        $("#liAjustarPeso").hide();

    if (dadosNaoConformidade.PermitirAjustar && (dadosNaoConformidade.TipoRegra === EnumTipoRegraNaoConformidade.NumeroPedido) && _usuarioLogadoPossuiAprovacao)
        $("#liAjustarNumeroPedido").show();
    else
        $("#liAjustarNumeroPedido").hide();

    if (dadosNaoConformidade.PermitirAjustar && (dadosNaoConformidade.TipoRegra === EnumTipoRegraNaoConformidade.ProdutoDePara) && _usuarioLogadoPossuiAprovacao)
        $("#liAjustarDeParaProdutos").show();
    else
        $("#liAjustarDeParaProdutos").hide();
}

function detalharNaoConformidade(registroSelecionado) {
    var pesquisa = RetornarObjetoPesquisa(_pesquisaNaoConformidade);

    _naoConformidade.Codigo.val(registroSelecionado.Codigo);
    _naoConformidade.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_naoConformidade, "AutorizacaoNaoConformidade/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _naoConformidade.PermitirAjustar.val(retorno.Data.PermitirAjustar)
                atualizarGridRegras(function () { controlarExibicaoAbas(retorno.Data); });

                Global.abrirModal('divModalNaoConformidade')
                $modalDetalhesNaoConformidade.one('hidden.bs.modal', function () {
                    limparCamposNaoConformidade();
                });
                _naoConformidade.VincularProdutosFornecedores.visible(retorno.Data.TipoRegraNaoConformidade == EnumTipoRegraNaoConformidade.ProdutoDePara)
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function exibirDadosCarga(e) {
    var data = { Carga: e.CodigoCarga };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close mt-3 me-2" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCarga");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function permiteAprovar() {
    var registros = _gridNaoConformidade.ObterMultiplosSelecionados();
    return registros.every(function (registro) {
        return registro.PermitirAjustar;
    }) && (registros.length > 0 || _pesquisaNaoConformidade.SelecionarTodos.val()) && _usuarioPermissaoAprovarNaoConformidade;
}

function limparCamposNaoConformidade() {
    $("#myTab a:first").tab("show");

    limparRegras();
}

function abrirModalVincularProdutos() {
    
}

// #endregion Funções
