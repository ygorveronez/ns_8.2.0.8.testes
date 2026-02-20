/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/AreaVeiculoPosicao.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/ManobraAcao.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoManobra.js" />
/// <reference path="ManobraDetalhes.js" />
/// <reference path="ManobraSignalR.js" />
/// <reference path="ManobraTracao.js" />
/// <reference path="ManobraTracaoDetalhes.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridManobra;
var _manobra;
var _manobraCadastro;
var _manobraFinalizacao;
var _pesquisaManobra;
var _pesquisaManobraAuxiliar;
/*
 * Declaração das Classes
 */

var PesquisaManobra = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CD:", idBtnSearch: guid(), required: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoManobra.obterListaOpcoesPendentes()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoManobra.obterOpcoes(), text: "Situação:" });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaManobra)) {
                $("#manobra-container").removeClass("hidden");

                _pesquisaManobra.ExibirFiltros.visibleFade(false);

                atualizarFiltrosUltimaPesquisa();
                recarregarDados();
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

var PesquisaManobraAuxiliar = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoManobra.obterOpcoes() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
}

var Manobra = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarManobraModalClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var ManobraCadastro = function () {
    this.LocalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Local de Destino:"), idBtnSearch: guid(), required: ko.observable(false) });
    this.ManobraAcao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Ação:", idBtnSearch: guid(), required: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veiculo:", idBtnSearch: guid(), required: true });

    this.ManobraAcao.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0)
            controlarLocalDestinoManobraObrigatorio(false);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarManobraClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var ManobraFinalizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Local = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Local:"), idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: finalizarManobraClick, type: types.event, text: ko.observable("Finalizar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDroppableManobra() {
    $("#remocao-item-manobra").droppable({
        drop: function (event, ui) {
            var idOrigem = $(ui.draggable[0]).parent().parent()[0].id;

            if (idOrigem == "grid-manobra")
                removerManobra(ui.draggable[0].id);
        },
        hoverClass: "remocao-item-container-hover",
    });
}

function loadGridManobra() {
    var limiteRegistros = 20;
    var totalRegistrosPorPagina = 20;
    var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalhesManobraClick, tamanho: "10", icone: "" };
    var opcaoFinalizar = { descricao: "Finalizar", id: guid(), evento: "onclick", metodo: finalizarManobraModalClick, tamanho: "10", icone: "", visibilidade: isPermitirFinalizar };
    var opcaoRemoverReserva = { descricao: "Remover Reserva", id: guid(), evento: "onclick", metodo: removerReservaClick, tamanho: "10", icone: "", visibilidade: isPermitirRemoverReserva };
    var opcaoRemoverTracaoManobraVinculada = { descricao: "Remover Tração de Manobra", id: guid(), evento: "onclick", metodo: removerManobraTracaoVinculadaClick, tamanho: "10", icone: "", visibilidade: isPermitirRemoverManobraTracaoVinculada };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoDetalhes, opcaoFinalizar, opcaoRemoverReserva, opcaoRemoverTracaoManobraVinculada ], tamanho: 10, };

    _gridManobra = new GridView("grid-manobra", "Manobra/Pesquisa", _pesquisaManobraAuxiliar, menuOpcoes, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowManobra);
}

function loadManobra() {
    _pesquisaManobra = new PesquisaManobra();
    KoBindings(_pesquisaManobra, "knockoutPesquisaManobra", false, _pesquisaManobra.Pesquisar.id);

    _pesquisaManobraAuxiliar = new PesquisaManobraAuxiliar();

    _manobra = new Manobra();
    KoBindings(_manobra, "knockoutManobra");

    _manobraCadastro = new ManobraCadastro();
    KoBindings(_manobraCadastro, "knockoutCadastroManobra");

    _manobraFinalizacao = new ManobraFinalizacao();
    KoBindings(_manobraFinalizacao, "knockoutFinalizarManobra");

    new BuscarAreaVeiculoPosicao(_manobraCadastro.LocalDestino, null, _pesquisaManobraAuxiliar.CentroCarregamento);
    new BuscarAreaVeiculoPosicao(_manobraFinalizacao.Local, null, _pesquisaManobraAuxiliar.CentroCarregamento);
    new BuscarCentrosCarregamento(_pesquisaManobra.CentroCarregamento, undefined, undefined, undefined, undefined, true);
    new BuscarManobraAcao(_manobraCadastro.ManobraAcao, retornoBuscaManobraAcao, _pesquisaManobraAuxiliar.CentroCarregamento);
    new BuscarVeiculos(_manobraCadastro.Veiculo);
    new BuscarTransportadores(_pesquisaManobra.Transportador);

    loadManobraTracao();
    loadManobraDetalhes();
    loadManobraTracaoDetalhes();
    loadGridManobra();
    loadDroppableManobra();
    loadManobraSignalR();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarManobraClick() {
    if (ValidarCamposObrigatorios(_manobraCadastro)) {
        var manobraCadastrar = {
            CentroCarregamento: _pesquisaManobraAuxiliar.CentroCarregamento.codEntity(),
            LocalDestino: _manobraCadastro.LocalDestino.codEntity(),
            ManobraAcao: _manobraCadastro.ManobraAcao.codEntity(),
            Veiculo: _manobraCadastro.Veiculo.codEntity()
        };

        executarReST("Manobra/Adicionar", manobraCadastrar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionada manobra com sucesso");

                    fecharManobraModal();
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

function adicionarManobraModalClick() {
    if (_pesquisaManobraAuxiliar.CentroCarregamento.codEntity() > 0) {
        Global.abrirModal('divModalCadastroManobra');
        $("#divModalCadastroManobra").one('hidden.bs.modal', function () {
            LimparCampos(_manobraCadastro);
        });
    }
}

function detalhesManobraClick(registroSelecionado) {
    exibirManobraDetalhes(registroSelecionado);
}

function finalizarManobraClick() {
    if (ValidarCamposObrigatorios(_manobraFinalizacao)) {
        executarReST("Manobra/Finalizar", RetornarObjetoPesquisa(_manobraFinalizacao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizada manobra com sucesso");
                    fecharManobraFinalizacaoModal();
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

function finalizarManobraModalClick(registroSelecionado) {
    if (_pesquisaManobraAuxiliar.CentroCarregamento.codEntity() > 0) {
        _manobraFinalizacao.Codigo.val(registroSelecionado.Codigo);

        Global.abrirModal('divModalFinalizarManobra');
        $("#divModalFinalizarManobra").one('hidden.bs.modal', function () {
            LimparCampos(_manobraFinalizacao);
        });
    }
}

function removerManobraTracaoVinculadaClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente remover a tração de manobra vinculada?", function () {
        executarReST("Manobra/RemoverManobraTracaoVinculada", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Removida a tração de manobra com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function removerReservaClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja realmente remover a reserva?", function () {
        executarReST("Manobra/RemoverReserva", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Removida a reserva com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

/*
 * Declaração das Funções
 */

function atualizarFiltrosUltimaPesquisa() {
    _pesquisaManobraAuxiliar.CentroCarregamento.codEntity(_pesquisaManobra.CentroCarregamento.codEntity());
    _pesquisaManobraAuxiliar.CentroCarregamento.val(_pesquisaManobra.CentroCarregamento.val());
    _pesquisaManobraAuxiliar.Transportador.codEntity(_pesquisaManobra.Transportador.codEntity());
    _pesquisaManobraAuxiliar.Transportador.val(_pesquisaManobra.Transportador.val());
    _pesquisaManobraAuxiliar.DataInicial.val(_pesquisaManobra.DataInicial.val());
    _pesquisaManobraAuxiliar.DataLimite.val(_pesquisaManobra.DataLimite.val());
    _pesquisaManobraAuxiliar.Situacao.val(_pesquisaManobra.Situacao.val());
}

function callbackRowManobra(nRow, aData) {
    if (!aData.Tempo) {
        var indiceColunaTempo = 5;
        var span = $(nRow).find('td').eq(indiceColunaTempo).find('span')[0];

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

    if (aData.Situacao == EnumSituacaoManobra.AguardandoManobra)
        $(nRow).addClass("manobra-tracao-droppable");

    if ((aData.Situacao == EnumSituacaoManobra.AguardandoManobra) || (aData.Situacao == EnumSituacaoManobra.Reservada)) {
        $(nRow).draggable({
            cursor: "move",
            helper: function (event) {
                var html = '';

                $(event.currentTarget).children().each(function (i, coluna) {
                    var $coluna = $(coluna);

                    html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
                });

                var corLinha = $(event.currentTarget).css("background-color");
                var corLinhaSelecionada = "#ecf3f8";
                var coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
                var isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

                return '<tr style="z-index: 5000; width: ' + $(event.currentTarget).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
            },
            revert: 'invalid',
            start: function () {
                $("#remocao-item-manobra").show();
            },
            stop: function () {
                $("#remocao-item-manobra").hide();
            }
        });
    }
    else
        $(nRow).css("cursor", "default");
}

function controlarLocalDestinoManobraObrigatorio(localDestinoObrigatorio) {
    _manobraCadastro.LocalDestino.required(localDestinoObrigatorio);
    _manobraCadastro.LocalDestino.text((localDestinoObrigatorio ? "*" : "") + "Local de Destino");
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function fecharManobraModal() {
    Global.fecharModal("divModalCadastroManobra");
}

function fecharManobraFinalizacaoModal() {
    Global.fecharModal("divModalFinalizarManobra");
}

function isPermitirFinalizar(registroSelecionado) {
    return (registroSelecionado.Situacao == EnumSituacaoManobra.EmManobra);
}

function isPermitirRemoverManobraTracaoVinculada(registroSelecionado) {
    return (registroSelecionado.Situacao == EnumSituacaoManobra.EmManobra);
}

function isPermitirRemoverReserva(registroSelecionado) {
    return (registroSelecionado.Situacao == EnumSituacaoManobra.Reservada);
}

function recarregarDados() {
    _gridManobra.CarregarGrid();
    carregarListaManobraTracao();
}

function removerManobra(codigoManobra) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir a manobra?", function () {
        executarReST("Manobra/Remover", { Codigo: codigoManobra }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Removida manobra com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function retornoBuscaManobraAcao(manobraAcao) {
    _manobraCadastro.ManobraAcao.codEntity(manobraAcao.Codigo);
    _manobraCadastro.ManobraAcao.entityDescription(manobraAcao.Descricao);
    _manobraCadastro.ManobraAcao.val(manobraAcao.Descricao);

    controlarLocalDestinoManobraObrigatorio(manobraAcao.LocalDestinoObrigatorio);
}