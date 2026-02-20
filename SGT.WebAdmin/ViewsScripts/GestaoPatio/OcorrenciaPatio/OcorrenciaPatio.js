/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrenciaPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoLancamento.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/OcorrenciaPatioTipo.js" />
/// <reference path="../../Consultas/Veiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridOcorrenciaPatio;
var _ocorrenciaPatio;
var _ocorrenciaPatioCadastro;
var _pesquisaOcorrenciaPatio;
var _pesquisaOcorrenciaPatioAuxiliar;

/*
 * Declaração das Classes
 */

var OcorrenciaPatio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarOcorrenciaPatioModalClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var OcorrenciaPatioCadastro = function () {
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true });
    this.Tipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarOcorrenciaPatioClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var PesquisaOcorrenciaPatio = function () {
    var dataPadrao = Global.DataAtual();

    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CD:", idBtnSearch: guid(), required: true });
    this.DataLimite = PropertyEntity({ text: "Data Limite:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataInicio = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoOcorrenciaPatio.obterListaOpcaoPendente()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoOcorrenciaPatio.obterOpcoes(), text: "Situação:" });

    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaOcorrenciaPatio)) {
                _pesquisaOcorrenciaPatio.ExibirFiltros.visibleFade(false);

                $("#ocorrencia-patio-container").removeClass("d-none");

                atualizarFiltrosUltimaPesquisa();
                _gridOcorrenciaPatio.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção!", "Verifique os campos obrigatórios!");
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaOcorrenciaPatioAuxiliar = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataLimite = PropertyEntity({ getType: typesKnockout.date });
    this.DataInicio = PropertyEntity({ getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoOcorrenciaPatio.obterOpcoes() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDroppableOcorrenciaPatio() {
    $("#remocao-item-ocorrencia-patio").droppable({
        drop: function (event, ui) {
            var idOrigem = $(ui.draggable[0]).parent().parent()[0].id;

            if (idOrigem == "grid-ocorrencia-patio")
                removerOcorrenciaPatio(ui.draggable[0].id);
        },
        hoverClass: "remocao-item-container-hover",
    });
}

function loadOcorrenciaPatio() {
    _pesquisaOcorrenciaPatio = new PesquisaOcorrenciaPatio();
    KoBindings(_pesquisaOcorrenciaPatio, "knockoutPesquisaOcorrenciaPatio", _pesquisaOcorrenciaPatio.Pesquisar.id);

    _pesquisaOcorrenciaPatioAuxiliar = new PesquisaOcorrenciaPatioAuxiliar();

    _ocorrenciaPatio = new OcorrenciaPatio();
    KoBindings(_ocorrenciaPatio, "knockoutOcorrenciaPatio");

    _ocorrenciaPatioCadastro = new OcorrenciaPatioCadastro();
    KoBindings(_ocorrenciaPatioCadastro, "knockoutCadastroOcorrenciaPatio");

    new BuscarCentrosCarregamento(_pesquisaOcorrenciaPatio.CentroCarregamento);
    new BuscarOcorrenciaPatioTipo(_ocorrenciaPatioCadastro.Tipo);
    new BuscarVeiculos(_ocorrenciaPatioCadastro.Veiculo);

    loadGridOcorrenciaPatio();
    loadDroppableOcorrenciaPatio();
}

function loadGridOcorrenciaPatio() {
    var opcaoAprovar = { descricao: "Aprovar", id: guid(), evento: "onclick", metodo: aprovarClick, icone: "", visibilidade: isPermiteAprovar };
    var opcaoReprovar = { descricao: "Reprovar", id: guid(), evento: "onclick", metodo: reprovarClick, icone: "", visibilidade: isPermiteReprovar };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 10, opcoes: [opcaoAprovar, opcaoReprovar] };

    _gridOcorrenciaPatio = new GridView("grid-ocorrencia-patio", "OcorrenciaPatio/Pesquisa", _pesquisaOcorrenciaPatioAuxiliar, menuOpcoes, null, undefined, null, false, false, undefined, undefined, undefined, undefined, undefined, undefined, callbackRowOcorrenciaPatio);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarOcorrenciaPatioClick() {
    if (ValidarCamposObrigatorios(_ocorrenciaPatioCadastro)) {
        var ocorrenciaPatioCadastrar = {
            CentroCarregamento: _pesquisaOcorrenciaPatioAuxiliar.CentroCarregamento.codEntity(),
            Descricao: _ocorrenciaPatioCadastro.Descricao.val(),
            Tipo: _ocorrenciaPatioCadastro.Tipo.codEntity(),
            Veiculo: _ocorrenciaPatioCadastro.Veiculo.codEntity()
        };

        executarReST("OcorrenciaPatio/Adicionar", ocorrenciaPatioCadastrar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionada ocorrência de pátio com sucesso");

                    fecharOcorrenciaPatioModal();
                    _gridOcorrenciaPatio.CarregarGrid();
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

function adicionarOcorrenciaPatioModalClick() {
    if (_pesquisaOcorrenciaPatioAuxiliar.CentroCarregamento.codEntity() > 0) {
        Global.abrirModal('divModalCadastroOcorrenciaPatio');
        $("#divModalCadastroOcorrenciaPatio").one('hidden.bs.modal', function () {
            LimparCampos(_ocorrenciaPatioCadastro);
        });
    }
}

function aprovarClick(registroSelecionado) {
    executarReST("OcorrenciaPatio/Aprovar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrência aprovada com sucesso");
                _gridOcorrenciaPatio.CarregarGrid()
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function reprovarClick(registroSelecionado) {
    executarReST("OcorrenciaPatio/Reprovar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrência reprovada com sucesso");
                _gridOcorrenciaPatio.CarregarGrid()
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function atualizarFiltrosUltimaPesquisa() {
    _pesquisaOcorrenciaPatioAuxiliar.CentroCarregamento.codEntity(_pesquisaOcorrenciaPatio.CentroCarregamento.codEntity());
    _pesquisaOcorrenciaPatioAuxiliar.CentroCarregamento.val(_pesquisaOcorrenciaPatio.CentroCarregamento.val());
    _pesquisaOcorrenciaPatioAuxiliar.DataLimite.val(_pesquisaOcorrenciaPatio.DataLimite.val());
    _pesquisaOcorrenciaPatioAuxiliar.DataInicio.val(_pesquisaOcorrenciaPatio.DataInicio.val());
    _pesquisaOcorrenciaPatioAuxiliar.Situacao.val(_pesquisaOcorrenciaPatio.Situacao.val());
}

function callbackRowOcorrenciaPatio(nRow, aData) {
    if ((aData.Situacao == EnumSituacaoOcorrenciaPatio.Pendente) && (aData.TipoLancamento == EnumTipoLancamento.Manual)){
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
                $("#remocao-item-ocorrencia-patio").show();
            },
            stop: function () {
                $("#remocao-item-ocorrencia-patio").hide();
            }
        });
    }
    else
        $(nRow).css("cursor", "default");
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function fecharOcorrenciaPatioModal() {
    Global.fecharModal("divModalCadastroOcorrenciaPatio");
}

function isPermiteAprovar(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoOcorrenciaPatio.Pendente;
}

function isPermiteReprovar(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoOcorrenciaPatio.Pendente;
}

function removerOcorrenciaPatio(codigoOcorrenciaPatio) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir a ocorrência de pátio?", function () {
        executarReST("OcorrenciaPatio/ExcluirPorCodigo", { Codigo: codigoOcorrenciaPatio }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrência excluída com sucesso");
                    _gridOcorrenciaPatio.CarregarGrid()
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}