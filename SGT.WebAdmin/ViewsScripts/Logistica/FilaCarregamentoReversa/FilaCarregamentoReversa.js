/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/AreaVeiculoPosicao.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/GrupoModeloVeicular.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumRetornoCargaTipo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculoReversa.js" />
/// <reference path="../../Enumeradores/EnumTipoAreaVeiculo.js" />
/// <reference path="FilaCarregamentoReversaDetalhes.js" />
/// <reference path="FilaCarregamentoReversaSignalR.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filaCarregamentoReversa;
var _filaCarregamentoReversaCadastro;
var _filaCarregamentoReversaInicioDescarregamento;
var _listaTipoRetornoCargaCarregado;
var _gridFilaCarregamentoReversa;
var _pesquisaFilaCarregamentoReversa;
var _pesquisaFilaCarregamentoReversaAuxiliar;

/*
 * Declaração das Classes
 */

var FilaCarregamentoReversa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaCarregamentoReversaModalClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var FilaCarregamentoReversaCadastro = function () {
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: _listaTipoRetornoCargaCarregado, def: "", text: "*Tipo: ", required: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFilaCarregamentoReversaClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var FilaCarregamentoReversaInicioDescarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Local = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Local:"), idBtnSearch: guid(), required: true });
    this.TipoRetornoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

    this.Iniciar = PropertyEntity({ eventClick: iniciarDescarregamentoClick, type: types.event, text: ko.observable("Iniciar"), visible: true });
}

var PesquisaFilaCarregamentoReversa = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CD:", idBtnSearch: guid(), required: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.GrupoModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Modelo Veicular:", idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoFilaCarregamentoVeiculoReversa.obterListaOpcoesPendentes()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoFilaCarregamentoVeiculoReversa.obterOpcoes(), text: "Situação:" });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaFilaCarregamentoReversa)) {
                $("#fila-carregamento-reversa-container").removeClass("d-none");
                
                _pesquisaFilaCarregamentoReversa.ExibirFiltros.visibleFade(false);

                atualizarFiltrosUltimaPesquisa();
                recarregarGridFilaCarregamentoReversa();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaFilaCarregamentoReversaAuxiliar = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ getType: typesKnockout.date });
    this.GrupoModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoFilaCarregamentoVeiculoReversa.obterOpcoes() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoReversa() {
    loadListaTipoRetornoCarga(function () {
        _pesquisaFilaCarregamentoReversa = new PesquisaFilaCarregamentoReversa();
        KoBindings(_pesquisaFilaCarregamentoReversa, "knockoutPesquisaFilaCarregamentoReversa", false, _pesquisaFilaCarregamentoReversa.Pesquisar.id);

        _pesquisaFilaCarregamentoReversaAuxiliar = new PesquisaFilaCarregamentoReversaAuxiliar();

        _filaCarregamentoReversa = new FilaCarregamentoReversa();
        KoBindings(_filaCarregamentoReversa, "knockoutFilaCarregamentoReversa");

        _filaCarregamentoReversaCadastro = new FilaCarregamentoReversaCadastro();
        KoBindings(_filaCarregamentoReversaCadastro, "knockoutCadastroFilaCarregamentoReversa");

        _filaCarregamentoReversaInicioDescarregamento = new FilaCarregamentoReversaInicioDescarregamento();
        KoBindings(_filaCarregamentoReversaInicioDescarregamento, "knockoutInicioDescarregamentoFilaCarregamentoReversa");

        new BuscarAreaVeiculoPosicao(_filaCarregamentoReversaInicioDescarregamento.Local, null, _pesquisaFilaCarregamentoReversaAuxiliar.CentroCarregamento, null, _filaCarregamentoReversaInicioDescarregamento.TipoRetornoCarga, EnumTipoAreaVeiculo.Doca);
        new BuscarCentrosCarregamento(_pesquisaFilaCarregamentoReversa.CentroCarregamento);
        new BuscarGrupoModeloVeicular(_pesquisaFilaCarregamentoReversa.GrupoModeloVeicular);
        new BuscarModelosVeicularesCarga(_pesquisaFilaCarregamentoReversa.ModeloVeicular);
        new BuscarVeiculos(_filaCarregamentoReversaCadastro.Veiculo);

        loadGridFilaCarregamentoReversa();
        loadFilaCarregamentoReversaDetalhes();
        loadFilaCarregamentoReversaSignalR();
    });
}

function loadGridFilaCarregamentoReversa() {
    var limiteRegistros = 10;
    var totalRegistrosPorPagina = 5;
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: exibirFilaCarregamentoReversaDetalhes, tamanho: "10", icone: "" };
    var opcaoFinalizarDescarregamento = { descricao: "Finalizar Descarregamento", id: guid(), evento: "onclick", metodo: finalizarDescarregamentoClick, tamanho: "10", icone: "", visibilidade: isSituacaoPermiteFinalizarDescarregamento };
    var opcaoIniciarDescarregamento = { descricao: "Iniciar Descarregamento", id: guid(), evento: "onclick", metodo: iniciarDescarregamentoModalClick, tamanho: "10", icone: "", visibilidade: isSituacaoPermiteIniciarDescarregamento };
    var opcaoRemover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFilaCarregamentoReversaClick, tamanho: "10", icone: "", visibilidade: isPermitirRemoverFilaCarregamentoReversa };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoDetalhes, opcaoFinalizarDescarregamento, opcaoIniciarDescarregamento, opcaoRemover], tamanho: 10 };

    _gridFilaCarregamentoReversa = new GridView("grid-fila-carregamento-reversa", "FilaCarregamentoReversa/Pesquisa", _pesquisaFilaCarregamentoReversaAuxiliar, menuOpcoes, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowFilaCarregamentoReversa);
}

function loadListaTipoRetornoCarga(callback) {
    executarReST("TipoRetornoCarga/PesquisaRetornoCargaTipo", { Tipo: EnumRetornoCargaTipo.Carregado }, function (retorno) {
        if (retorno.Success) {
            _listaTipoRetornoCargaCarregado = retorno.Data;

            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarFilaCarregamentoReversaClick() {
    if (ValidarCamposObrigatorios(_filaCarregamentoReversaCadastro)) {
        var filaCarregamentoReversaCadastrar = {
            Tipo: _filaCarregamentoReversaCadastro.Tipo.val(),
            Veiculo: _filaCarregamentoReversaCadastro.Veiculo.codEntity(),
            CentroCarregamento: _pesquisaFilaCarregamentoReversaAuxiliar.CentroCarregamento.codEntity()
        };

        executarReST("FilaCarregamentoReversa/AdicionarFila", filaCarregamentoReversaCadastrar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado registro na fila com sucesso");

                    fecharModalCadastroFilaCarregamentoReversa();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarFilaCarregamentoReversaModalClick() {
    if (_pesquisaFilaCarregamentoReversaAuxiliar.CentroCarregamento.codEntity() > 0) {
        Global.abrirModal('divModalCadastroFilaCarregamentoReversa');
        $("#divModalCadastroFilaCarregamentoReversa").one('hidden.bs.modal', function () {
            LimparCampos(_filaCarregamentoReversaCadastro);
        });
    }
}

function finalizarDescarregamentoClick(registroSelecionado) {
    executarReST("FilaCarregamentoReversa/FinalizarDescarregamento", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso");
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function iniciarDescarregamentoClick() {
    if (ValidarCamposObrigatorios(_filaCarregamentoReversaInicioDescarregamento)) {
        executarReST("FilaCarregamentoReversa/IniciarDescarregamento", RetornarObjetoPesquisa(_filaCarregamentoReversaInicioDescarregamento), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Descarregamento iniciado com sucesso");

                    fecharModalInicioDescarregamentoFilaCarregamentoReversa();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function iniciarDescarregamentoModalClick(registroSelecionado) {
    _filaCarregamentoReversaInicioDescarregamento.Codigo.val(registroSelecionado.Codigo);
    _filaCarregamentoReversaInicioDescarregamento.TipoRetornoCarga.codEntity(registroSelecionado.CodigoTipoRetornoCarga);
    _filaCarregamentoReversaInicioDescarregamento.TipoRetornoCarga.val(registroSelecionado.TipoRetornoCarga);

    Global.abrirModal('divModalInicioDescarregamentoFilaCarregamentoReversa');
    $("#divModalInicioDescarregamentoFilaCarregamentoReversa").one('hidden.bs.modal', function () {
        LimparCampos(_filaCarregamentoReversaInicioDescarregamento);
    });
}

function removerFilaCarregamentoReversaClick(registroSelecionado) {
    cancelarDescarregamento(registroSelecionado.Codigo);
}

/*
 * Declaração das Funções
 */

function atualizarFiltrosUltimaPesquisa() {
    _pesquisaFilaCarregamentoReversaAuxiliar.CentroCarregamento.codEntity(_pesquisaFilaCarregamentoReversa.CentroCarregamento.codEntity());
    _pesquisaFilaCarregamentoReversaAuxiliar.CentroCarregamento.val(_pesquisaFilaCarregamentoReversa.CentroCarregamento.val());
    _pesquisaFilaCarregamentoReversaAuxiliar.DataInicial.val(_pesquisaFilaCarregamentoReversa.DataInicial.val());
    _pesquisaFilaCarregamentoReversaAuxiliar.DataLimite.val(_pesquisaFilaCarregamentoReversa.DataLimite.val());
    _pesquisaFilaCarregamentoReversaAuxiliar.GrupoModeloVeicular.codEntity(_pesquisaFilaCarregamentoReversa.GrupoModeloVeicular.codEntity());
    _pesquisaFilaCarregamentoReversaAuxiliar.GrupoModeloVeicular.val(_pesquisaFilaCarregamentoReversa.GrupoModeloVeicular.val());
    _pesquisaFilaCarregamentoReversaAuxiliar.ModeloVeicular.codEntity(_pesquisaFilaCarregamentoReversa.ModeloVeicular.codEntity());
    _pesquisaFilaCarregamentoReversaAuxiliar.ModeloVeicular.val(_pesquisaFilaCarregamentoReversa.ModeloVeicular.val());
    _pesquisaFilaCarregamentoReversaAuxiliar.Situacao.val(_pesquisaFilaCarregamentoReversa.Situacao.val());
}

function callbackRowFilaCarregamentoReversa(nRow, aData) {
    if (!aData.Tempo) {
        var indiceColunaTempoFila = _pesquisaFilaCarregamentoReversaAuxiliar.ModeloVeicular.codEntity() > 0 ? 5 : 6;
        var span = $(nRow).find('td').eq(indiceColunaTempoFila).find('span')[0];

        if (span) {
            $(span)
                .countdown(moment(aData.DataCriacao, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                .on('update.countdown', function (event) {
                    if (event.offset.totalDays > 0)
                        $(this).text(event.strftime('%-Dd %H:%M:%S'));
                    else
                        $(this).text(event.strftime('%H:%M:%S'));
                })
        }
    }
}

function cancelarDescarregamento(codigoFilaCarregamentoVeiculoReversa) {
    executarReST("FilaCarregamentoReversa/CancelarDescarregamento", { Codigo: codigoFilaCarregamentoVeiculoReversa }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function fecharModalCadastroFilaCarregamentoReversa() {
    Global.fecharModal("divModalCadastroFilaCarregamentoReversa");
}

function fecharModalInicioDescarregamentoFilaCarregamentoReversa() {
    Global.fecharModal("divModalInicioDescarregamentoFilaCarregamentoReversa");
}

function isPermitirRemoverFilaCarregamentoReversa(registroSelecionado) {
    return registroSelecionado.PermitirRemocao;
}

function isSituacaoPermiteFinalizarDescarregamento(registroSelecionado) {
    return (registroSelecionado.Situacao == EnumSituacaoFilaCarregamentoVeiculoReversa.EmDescarregamento);
}

function isSituacaoPermiteIniciarDescarregamento(registroSelecionado) {
    return (registroSelecionado.Situacao == EnumSituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento);
}

function recarregarGridFilaCarregamentoReversa() {
    _gridFilaCarregamentoReversa.CarregarGrid();
}
