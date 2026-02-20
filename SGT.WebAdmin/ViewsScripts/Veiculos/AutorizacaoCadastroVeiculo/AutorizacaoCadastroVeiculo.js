/// <reference path="AutorizarRegras.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Consultas/Usuario.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridVeiculo;
var _pesquisaVeiculo;
var _rejeicao;
var _veiculo;
var $modalDetalhesVeiculo;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarVeiculoSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var CadastroVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoVeiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Solicitante = PropertyEntity({ text: "Solicitante: ", visible: ko.observable(true) });
    this.DataCadastro = PropertyEntity({ text: "Data do cadastro: ", visible: ko.observable(true) });
    this.Placa = PropertyEntity({ text: "Placa: ", visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo Veicular: ", visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: "Transportador: ", visible: ko.observable(true) });
    this.Renavam = PropertyEntity({ text: "RENAVAM: ", visible: ko.observable(true) });
    this.Tara = PropertyEntity({ text: "Tara: ", visible: ko.observable(true) });
    this.CapacidadeKG = PropertyEntity({ text: "Capacidade em KG: ", visible: ko.observable(true) });
    this.TipoRodado = PropertyEntity({ text: "Tipo do rodado: ", visible: ko.observable(true) });
    this.TipoCarroceria = PropertyEntity({ text: "Tipo da carroceria: ", visible: ko.observable(true) });
    this.ModeloCarroceria = PropertyEntity({ text: "Modelo da carroceria: ", visible: ko.observable(true) });
    this.Reboques = PropertyEntity({ text: "Reboques: ", visible: ko.observable(true) });
}

var PesquisaCadastroVeiculo = function () {
    this.Codigo = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCadastroVeiculo.Pendente), options: EnumSituacaoCadastroVeiculo.obterOpcoes(), def: EnumSituacaoCadastroVeiculo.Pendente, text: "Situação: " });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasVeiculoClick, text: "Aprovar Cadastros", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridCadastroVeiculos, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasVeiculoClick, text: "Rejeitar Cadastros", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Cadastros", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _veiculo = new CadastroVeiculo();
    KoBindings(_veiculo, "knockoutCadastroVeiculo");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoCadastroVeiculo");

    _pesquisaVeiculo = new PesquisaCadastroVeiculo();
    KoBindings(_pesquisaVeiculo, "knockoutPesquisaCadastroVeiculo");

    $("#liAuditar").on("click", carrregarAuditoria);

    loadGridVeiculo();
    loadRegras();
    loadDelegar();

    $modalDetalhesVeiculo = $("#divModalCadastroVeiculo");

    new BuscarFuncionario(_pesquisaVeiculo.Usuario);
    new BuscarTransportadores(_pesquisaVeiculo.Transportador);
    new BuscarModelosVeicularesCarga(_pesquisaVeiculo.ModeloVeicular);

    loadDadosUsuarioLogado(atualizarGridCadastroVeiculos);
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaVeiculo.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaVeiculo.Usuario.val(retorno.Data.Nome);

                callback();
            }
        });
    else
        callback();
}

function loadGridVeiculo() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharCadastroVeiculo,
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
        SelecionarTodosKnout: _pesquisaVeiculo.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoCadastroVeiculo/ExportarPesquisa",
        titulo: "Autorização Cadastro de Veículo"
    };

    _gridVeiculo = new GridView(_pesquisaVeiculo.Pesquisar.idGrid, "AutorizacaoCadastroVeiculo/Pesquisa", _pesquisaVeiculo, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasVeiculoClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas os cadastros de veículos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaVeiculo);

        dados.SelecionarTodos = _pesquisaVeiculo.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridVeiculo.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridVeiculo.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCadastroVeiculo/AprovarMultiplosItens", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.RegrasModificadas > 0) {
                        if (retorno.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + " alçadas foram aprovadas.");
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "1 alçada foi aprovada.");
                    }
                    else if (retorno.Data.Msg == "") {
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");
                    }

                    atualizarGridCadastroVeiculos();
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

    Global.fecharModal('knockoutRejeicaoCadastroVeiculo');
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal("divModalDelegarSelecionados");
}

function rejeitarMultiplasVeiculoClick() {
    LimparCampos(_rejeicao);

    Global.abrirModal("knockoutRejeicaoCadastroVeiculo");
}

function rejeitarVeiculoSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas os cadastros de veículos selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaVeiculo);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaVeiculo.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridVeiculo.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridVeiculo.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoCadastroVeiculo/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridCadastroVeiculos();
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

function atualizarGridCadastroVeiculos() {
    _pesquisaVeiculo.SelecionarTodos.val(false);
    _pesquisaVeiculo.AprovarTodas.visible(false);
    _pesquisaVeiculo.DelegarTodas.visible(false);
    _pesquisaVeiculo.RejeitarTodas.visible(false);

    _gridVeiculo.CarregarGrid();
}

function exibirMultiplasOpcoes() {
    _pesquisaVeiculo.AprovarTodas.visible(false);
    _pesquisaVeiculo.DelegarTodas.visible(false);
    _pesquisaVeiculo.RejeitarTodas.visible(false);

    var existemRegistrosSelecionados = _gridVeiculo.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaVeiculo.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_pesquisaVeiculo.Situacao.val() == EnumSituacaoCadastroVeiculo.Pendente) {
            _pesquisaVeiculo.AprovarTodas.visible(true);
            _pesquisaVeiculo.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaVeiculo.RejeitarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharCadastroVeiculo(registroSelecionado) {
    limparCamposCadastroVeiculo();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaVeiculo);

    _veiculo.Codigo.val(registroSelecionado.Codigo);
    _veiculo.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_veiculo, "AutorizacaoCadastroVeiculo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === EnumSituacaoCadastroVeiculo.Pendente);

                Global.abrirModal("divModalCadastroVeiculo");
                $modalDetalhesVeiculo.one('show.bs.modal', function () {
                    limparCamposCadastroVeiculo();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposCadastroVeiculo() {
    Global.ResetarAbas();

    limparRegras();
}

function carrregarAuditoria() {
    const auditar = OpcaoAuditoria("Veiculo");
    auditar({ Codigo: _veiculo.CodigoVeiculo.val() });
}