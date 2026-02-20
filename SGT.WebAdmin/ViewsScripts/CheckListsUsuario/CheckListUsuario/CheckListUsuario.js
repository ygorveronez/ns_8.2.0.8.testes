/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumCheckListResposta.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCheckList.js" />
/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckList.js" />
/// <reference path="../../gestaopatio/checklistcomponent/kopergunta.js" />

// #region Objetos Globais do Arquivo

var _checkList;
var _pesquisaCheckList;
var _gridCheckList;
var _PerguntasCheckList;
var _callbackCheckListAtualizado = null;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CheckList = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    //this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: abrirAuditoriaCheckList });
    this.Descricao = PropertyEntity({ text: "", val: ko.observable(""), def: "" });
    this.ObservacoesGerais = PropertyEntity({ text: "Observações: ", val: ko.observable(''), def: '', enable: ko.observable(true) });
    this.GrupoPerguntas = PropertyEntity({ val: ko.observableArray([]), type: types.map, def: [], enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.PreencherChecklist = PropertyEntity({ text: ko.observable("Checklist - " + Global.DataAtual()) });
    this.DataCheckList = PropertyEntity({ text: "Selecione Data: ", getType: typesKnockout.date, def: Global.DataAtual(), val: ko.observable(Global.DataAtual()) });
    this.Visualizar = PropertyEntity({ eventClick: BuscarCheckListData, type: types.event, text: "Visualizar", visible: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });


};

var PesquisaCheckList = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCheckList.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function loadCheckListData() {
    _pesquisaCheckList = new PesquisaCheckList();
    KoBindings(_pesquisaCheckList, "knockoutPesquisaCheckList", false, _pesquisaCheckList.Pesquisar.id);

    _checkList = new CheckList();
    KoBindings(_checkList, "knockoutCheckList");

    loadComponenteChecklistPergunta();
    loadObservacaoPergunta();
    loadValidacao();
    loadGridCheckList();

    BuscarCheckListData();
}

function loadGridCheckList() {
    var opcaoEditar = { descricao: "Visualizar", id: "clasEditar", evento: "onclick", metodo: editarCheckListClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridCheckList = new GridView(_pesquisaCheckList.Pesquisar.idGrid, "CheckListUsuario/Pesquisa", _pesquisaCheckList, menuOpcoes, null);
    _gridCheckList.CarregarGrid();
}


function BuscarCheckListData() {
    var dados = {
        Data: _checkList.DataCheckList.val(),
    };

    limparCamposCheckList();

    executarReST("CheckListUsuario/BuscarCheckList", dados, function (arg) {
        if (arg.Success) {


            if (arg.Data) {
                $(".check-list-form").show();
                PreencherObjetoKnout(_checkList, arg);
                preecherRetornoCheckLitAResponder(arg, arg.Data.Respondido);

                _checkList.PreencherChecklist.text(arg.Data.Descricao + " Data: " + arg.Data.DataResposta);
                _checkList.DataCheckList.val(arg.Data.DataResposta);

            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function loadValidacao() {
    $("#knockoutCheckList")
        // Mudando a resposta
        .on('change', '.state-error select', function () {
            $(this).parent().removeClass("state-error");
        })
        // Ou clicando no botão de obs
        .on('click', 'input[type=button]', function () {
            $(this).parents(".pergunta-container").find('.state-error').removeClass("state-error");
        });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function auditarCheckListClick() {
    var _fn = OpcaoAuditoria("CheckListCarga", "Codigo", _checkList);

    _fn({ Codigo: _checkList.Codigo.val() });
}

function editarCheckListClick(itemGrid) {
    limparCamposCheckList();

    var dados = {
        Codigo: itemGrid.Codigo
    };

    executarReST("CheckListUsuario/BuscarPorCodigo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                $(".check-list-form").show();

                PreencherObjetoKnout(_checkList, arg);
                preecherRetornoCheckLitAResponder(arg, true);

                _checkList.PreencherChecklist.text(arg.Data.Descricao + " Data: " + arg.Data.DataResposta);
                _checkList.DataCheckList.val(arg.Data.DataResposta);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

    });
}


function salvarClick() {
    if (!validarCheckListResposta())
        return;

    executarReST("CheckListUsuario/Salvar", preencherDadosSalvarCheckList(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _pesquisaCheckList.ExibirFiltros.visibleFade(true);
                limparCamposCheckList();

                _gridCheckList.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherDadosSalvarCheckList() {
    var dados = {
        Codigo: _checkList.Codigo.val(),
        Data: _checkList.DataCheckList.val(),
        GrupoPerguntas: JSON.stringify(_checkList.GrupoPerguntas.val()),
        ObservacoesGerais: _checkList.ObservacoesGerais.val()
    };

    return dados;
}


function preecherRetornoCheckLitAResponder(arg, respondido) {
    var data = arg.Data;
    _PerguntasCheckList = data.GrupoPerguntas;

    _pesquisaCheckList.ExibirFiltros.visibleFade(false);

    _checkList.GrupoPerguntas.enable(!respondido);
    _checkList.ObservacoesGerais.enable(!respondido);
    _checkList.Salvar.visible(!respondido);

}

function atualizarObservacao(dados) {
    _PerguntasCheckList[0].Perguntas.map(function (pergunta, iPergunta) {
        if (pergunta.Codigo == dados.Codigo) {
            _PerguntasCheckList[0].Perguntas[iPergunta].Observacao = dados.Observacao;
        }
    });

    _checkList.GrupoPerguntas.val(_PerguntasCheckList);
}

// #endregion Funções Públicas

// #region Funções Privadas

function limparCamposCheckList() {
    LimparCampos(_checkList);
    $(".check-list-form").hide();
}

function validarCheckListResposta() {
    var valido_reprovado = true;
    var valido_semresposta = true;
    var valido_respostaimpeditiva = true;

    _PerguntasCheckList[0].Perguntas.map(function (pergunta, iPergunta) {

        if (pergunta.Tipo == EnumTipoOpcaoCheckList.Aprovacao) {
            if (pergunta.Resposta == EnumCheckListResposta.Reprovada && pergunta.Observacao.length < 20) {
                $("#pergunta-" + pergunta.Codigo + " .select").addClass("state-error");
                valido_reprovado = false;
            }
            else if (pergunta.Resposta == EnumCheckListResposta.Aprovada) {
                var perguntasImpeditivas = _PerguntasCheckList
                    .filter((elemento) => elemento.Perguntas.filter((pergunta) => pergunta.Tipo == EnumTipoOpcaoCheckList.SimNao || pergunta.Tipo == EnumTipoOpcaoCheckList.Opcoes || pergunta.Tipo == EnumTipoOpcaoCheckList.Selecoes).length > 0)
                    .map((elemento) => elemento.Perguntas).flat(1)
                    .filter((pergunta) => pergunta.RespostaImpeditiva != null || pergunta.Alternativas.filter((alternativa) => alternativa.OpcaoImpeditiva == true).length > 0);

                for (var i = 0; i < perguntasImpeditivas.length; i++) {
                    if (perguntasImpeditivas[i].Tipo == EnumTipoOpcaoCheckList.SimNao) {
                        var respostaDaPergunta = perguntasImpeditivas[i].Resposta == "true" ? 1 : 2;

                        if (respostaDaPergunta == perguntasImpeditivas[i].RespostaImpeditiva) {
                            valido_respostaimpeditiva = false;
                            $("#sim_" + perguntasImpeditivas[i].id).parent().addClass("state-error");
                            $("#nao_" + perguntasImpeditivas[i].id).parent().addClass("state-error");
                        }
                    }
                    else {
                        if (perguntasImpeditivas[i].Alternativas.find((p) => p.OpcaoImpeditiva == true && p.Codigo == perguntasImpeditivas[i].Resposta) != undefined) {
                            valido_respostaimpeditiva = false;

                            if (perguntasImpeditivas[i].Tipo == EnumTipoOpcaoCheckList.Selecoes)
                                $("#pergunta-" + perguntasImpeditivas[i].id + " .select").addClass("state-error");
                        }
                    }
                }
            }
            else if (pergunta.Resposta == "") {
                $("#pergunta-" + pergunta.Codigo + " .select").addClass("state-error");
                valido_semresposta = false;
            }
        }

    });

    if (!valido_reprovado)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar uma observação de no mínimo 20 caracteres para itens reprovados.");

    if (!valido_semresposta)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário responder todos os itens.");

    if (!valido_respostaimpeditiva)
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível aprovar a checklist com a resposta atual nos itens destacados.");

    return valido_reprovado && valido_semresposta && valido_respostaimpeditiva;
}

// #endregion Funções Privadas
