/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAjusteConfiguracaoDescargaCliente.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/ConfiguracaoDescargaCliente.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridTaxaDescarga;
var _taxaDescarga;
var _pesquisaTaxaDescarga;
var _rejeicao;
var _situacaoTaxaDescargaUltimaPesquisa = EnumSituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao;
var $modalDetalhesTaxaDescarga;

/*
 * Declaração das Classes
 */

var RejeitarSelecionados = function () {
    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Rejeitar = PropertyEntity({ eventClick: rejeitarTaxaDescargaSelecionadasClick, type: types.event, text: "Rejeitar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

var TaxaDescarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({});
    this.Cidade = PropertyEntity({ text: "Cidade: ", visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "Valor: ", visible: ko.observable(true) });
    this.ValorPorPallet = PropertyEntity({ text: "Valor por pallet: ", visible: ko.observable(true) });
    this.ValorPorTonelada = PropertyEntity({ text: "Valor por tonelada: ", visible: ko.observable(true) });
    this.ValorPorUnidade = PropertyEntity({ text: "Valor por unidade: ", visible: ko.observable(true) });
    this.SituacaoDescricao = PropertyEntity({ text: "Situação: ", visible: ko.observable(true) });
    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial: ", visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ text: "Modelo Veicular: ", visible: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ text: "Tipo Carga: ", visible: ko.observable(true) });
    this.Clientes = PropertyEntity({ text: "Clientes: ", visible: ko.observable(true) });
    this.GrupoCliente = PropertyEntity({ text: "Grupo Cliente: ", visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "Tipo Operação: ", visible: ko.observable(true) });
};

var PesquisaTaxaDescarga = function () {
    this.Valor = PropertyEntity({ text: "Valor: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao), options: EnumSituacaoAjusteConfiguracaoDescargaCliente.obterOpcoesPesquisa(), def: EnumSituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao, text: "Situação: " });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasTaxaDescargaClick, text: "Aprovar Taxas de Descarga", visible: ko.observable(false) });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: atualizarGridTaxaDescarga, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: rejeitarMultiplasTaxaDescargaClick, text: "Rejeitar Taxas de Descarga", visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });
    this.DelegarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: exibirDelegarSelecionadosClick, text: "Delegar Taxas de Descarga", visible: ko.observable(false) });
    this.ReprocessarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: reprocessarMultiplasTaxasDescargaClick, text: "Reprocessar Taxas de Descarga", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadAutorizacao() {
    _taxaDescarga = new TaxaDescarga();
    KoBindings(_taxaDescarga, "knockoutTaxaDescarga");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoTaxaDescarga");

    _pesquisaTaxaDescarga = new PesquisaTaxaDescarga();
    KoBindings(_pesquisaTaxaDescarga, "knockoutPesquisaTaxaDescarga");

    loadGridTaxaDescarga();
    loadRegras();
    loadDelegar();

    $modalDetalhesTaxaDescarga = $("#divModalTaxaDescarga");

    new BuscarFilial(_pesquisaTaxaDescarga.Filial);
    new BuscarClientes(_pesquisaTaxaDescarga.Cliente);
    new BuscarFuncionario(_pesquisaTaxaDescarga.Operador);
    new BuscarFuncionario(_pesquisaTaxaDescarga.Usuario);

    loadDadosUsuarioLogado(atualizarGridTaxaDescarga);
}

function loadDadosUsuarioLogado(callback) {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario) {
                _pesquisaTaxaDescarga.Usuario.codEntity(retorno.Data.Codigo);
                _pesquisaTaxaDescarga.Usuario.val(retorno.Data.Nome);
            }
            callback();
        }
    });
}

function loadGridTaxaDescarga() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharTaxaDescarga,
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
        SelecionarTodosKnout: _pesquisaTaxaDescarga.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configuracaoExportacao = {
        url: "AutorizacaoTaxaDescarga/ExportarPesquisa",
        titulo: "Autorização Taxa Descarga"
    };

    _gridTaxaDescarga = new GridView(_pesquisaTaxaDescarga.Pesquisar.idGrid, "AutorizacaoTaxaDescarga/Pesquisa", _pesquisaTaxaDescarga, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function aprovarMultiplasTaxaDescargaClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as Taxas de Descarga selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTaxaDescarga);

        dados.SelecionarTodos = _pesquisaTaxaDescarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTaxaDescarga/AprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridTaxaDescarga();
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

    Global.fecharModal("divModalRejeitarTaxaDescarga");
}

function exibirDelegarSelecionadosClick() {
    Global.abrirModal("divModalDelegarSelecionados");
}

function rejeitarMultiplasTaxaDescargaClick() {
    LimparCampos(_rejeicao);

    $("#divModalRejeitarTaxaDescarga").modal("show");
}

function rejeitarTaxaDescargaSelecionadasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja reprovar todas as Taxas de Descarga selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaTaxaDescarga);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaTaxaDescarga.SelecionarTodos.val();
        dados.ItensSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosSelecionados());
        dados.ItensNaoSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoTaxaDescarga/ReprovarMultiplosItens", dados, function (retorno) {
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

                    atualizarGridTaxaDescarga();
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

function reprocessarMultiplasTaxasDescargaClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaTaxaDescarga);

    dados.SelecionarTodos = _pesquisaTaxaDescarga.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridTaxaDescarga.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoTaxaDescarga/ReprocessarMultiplasTaxasDescarga", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.RegrasReprocessadas > 0) {
                    if (retorno.Data.RegrasReprocessadas > 1)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasReprocessadas + " taxas de descarga foram reprocessadas com sucesso.");
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "1 taxa de descarga foi reprocessada com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Regras de Aprovação", "Nenhuma regra encontrada para as taxas de descarga selecionadas.");

                atualizarGridTaxaDescarga();
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

function atualizarGridTaxaDescarga() {
    _pesquisaTaxaDescarga.SelecionarTodos.val(false);
    _pesquisaTaxaDescarga.AprovarTodas.visible(false);
    _pesquisaTaxaDescarga.DelegarTodas.visible(false);
    _pesquisaTaxaDescarga.RejeitarTodas.visible(false);

    _gridTaxaDescarga.CarregarGrid();

    _situacaoTaxaDescargaUltimaPesquisa = _pesquisaTaxaDescarga.Situacao.val()
}

function exibirMultiplasOpcoes() {
    _pesquisaTaxaDescarga.AprovarTodas.visible(false);
    _pesquisaTaxaDescarga.DelegarTodas.visible(false);
    _pesquisaTaxaDescarga.RejeitarTodas.visible(false);
    _pesquisaTaxaDescarga.ReprocessarTodas.visible(false);

    var existemRegistrosSelecionados = _gridTaxaDescarga.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaTaxaDescarga.SelecionarTodos.val();
    if (existemRegistrosSelecionados || selecionadoTodos) {
        if (_situacaoTaxaDescargaUltimaPesquisa == EnumSituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao) {
            _pesquisaTaxaDescarga.AprovarTodas.visible(true);
            _pesquisaTaxaDescarga.DelegarTodas.visible(!_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar);
            _pesquisaTaxaDescarga.RejeitarTodas.visible(true);
        }
        else if (_situacaoTaxaDescargaUltimaPesquisa = EnumSituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao) {
            _pesquisaTaxaDescarga.ReprocessarTodas.visible(true);
        }
    }
}

function controlarExibicaoAbaDelegar(exibirDelegar) {
    if (exibirDelegar && !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar)
        $("#liDelegar").show();
    else
        $("#liDelegar").hide();
}

function detalharTaxaDescarga(registroSelecionado) {
    limparCamposTaxaDescarga();

    var pesquisa = RetornarObjetoPesquisa(_pesquisaTaxaDescarga);

    _taxaDescarga.Codigo.val(registroSelecionado.Codigo);
    _taxaDescarga.Usuario.val(pesquisa.Usuario);

    BuscarPorCodigo(_taxaDescarga, "AutorizacaoTaxaDescarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                atualizarGridRegras();
                controlarExibicaoAbaDelegar(retorno.Data.Situacao === _situacaoTaxaDescargaUltimaPesquisa.AgAprovacao);

                Global.abrirModal("divModalTaxaDescarga");
                $modalDetalhesTaxaDescarga.one('hidden.bs.modal', function () {
                    limparCamposTaxaDescarga();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }
    }, null);
}

function limparCamposTaxaDescarga() {
    $("#myTab a:first").tab("show");

    limparRegras();
}