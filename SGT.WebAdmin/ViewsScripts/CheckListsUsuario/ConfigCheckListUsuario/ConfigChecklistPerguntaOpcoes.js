/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckList.js" />
/// <reference path="ChecklistPergunta.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridOpcoes;
var AIOrdem = 0;
var editandoOpcao = false;

//*******EVENTOS*******
function adicionarOpcao() {

    var desc = _checklistPergunta.DescricaoOpcao.val();

    if (desc != "") {
        var opcoes = ObterListaOrdenada();

        var opcao = {
            Codigo: guid(),
            Ordem: opcoes.length,
            Descricao: _checklistPergunta.DescricaoOpcao.val(),
            Valor: _checklistPergunta.ValorOpcao.val(),
            CodigoIntegracao: _checklistPergunta.CodigoIntegracaoOpcao.val(),
            OpcaoImpeditiva: _checklistPergunta.OpcaoImpeditiva.val()
        };
        opcoes.push(opcao);

        SetaLista(opcoes);

        RecarregarGridOpcoes();
        LimparCamposOpcao();
        FocarCampoDescricao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Campo descrição é Obrigátorio");
    }
}

function adicionarOpcaoEnter(ko, e) {
    if (e && e.keyCode === 13) {
        if (editandoOpcao) {
            atualizarOpcao();
        } else {
            adicionarOpcao();
        }
    }

    return true;
}

function atualizarOpcao() {
    var desc = _checklistPergunta.DescricaoOpcao.val();

    if (desc != "") {
        var opcoes = ObterListaOrdenada();

        var opcao = {
            Codigo: _checklistPergunta.CodigoOpcao.val(),
            Ordem: _checklistPergunta.OrdemOpcao.val(),
            Descricao: _checklistPergunta.DescricaoOpcao.val(),
            Valor: _checklistPergunta.ValorOpcao.val(),
            CodigoIntegracao: _checklistPergunta.CodigoIntegracaoOpcao.val(),
            OpcaoImpeditiva: _checklistPergunta.OpcaoImpeditiva.val()
        };

        for (var i in opcoes) {
            if (opcoes[i].Codigo == opcao.Codigo) {
                opcoes[i] = opcao;
                break;
            }
        }

        SetaLista(opcoes);

        RecarregarGridOpcoes();
        LimparCamposOpcao();
        FocarCampoDescricao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Campo descrição é Obrigátorio");
    }
}

function excluirOpcao() {
    var opcoes = ObterListaOrdenada();
    var codigo = _checklistPergunta.CodigoOpcao.val();

    for (var i in opcoes) {
        if (opcoes[i].Codigo == codigo) {
            opcoes.splice(i, 1);
            break;
        }
    }

    SetaLista(opcoes);

    RecarregarGridOpcoes();
    LimparCamposOpcao();
    LinhasReordenadas();
}

function cancelarOpcao() {
    LimparCamposOpcao();
}

function editarOpcaoClick(codigo) {
    var opcoes = ObterListaOrdenada();
    var opcao = null;

    for (var i in opcoes) {
        if (opcoes[i].Codigo == codigo) {
            opcao = opcoes[i];
            break;
        }
    }

    if (opcao != null) {
        editandoOpcao = true;
        _checklistPergunta.CodigoOpcao.val(opcao.Codigo);
        _checklistPergunta.OrdemOpcao.val(opcao.Ordem);
        _checklistPergunta.DescricaoOpcao.val(opcao.Descricao);
        _checklistPergunta.ValorOpcao.val(opcao.Valor);
        _checklistPergunta.CodigoIntegracaoOpcao.val(opcao.CodigoIntegracao);
        _checklistPergunta.OpcaoImpeditiva.val(opcao.OpcaoImpeditiva);

        _checklistPergunta.AdicionarOpcao.visible(false);
        _checklistPergunta.AtualizarOpcao.visible(true);
        _checklistPergunta.ExcluirOpcao.visible(true);
    }
}

//*******MÉTODOS*******
function GridOpcoes() {
    _gridOpcoes = new GridReordering("", _checklistPergunta.Opcoes.idGrid, GeraHeadTable());
    _gridOpcoes.CarregarGrid();

    $("#" + _checklistPergunta.Opcoes.idGrid).on('sortstop', function () {
        LinhasReordenadas();
    });
}


function GeraHeadTable() {
    return '<tr>' +
        '<th width="10%" class="text-align-left hide">Ordem</th>' +
        '<th width="70%" class="text-align-left">Descrição</th>' +
        '<th width="10%" class="text-align-left">Código de integração</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function ObterListaOrdenada() {
    var opcoes = _checklistPergunta.Opcoes.list;
    return opcoes;
}

function SetaLista(data) {
    _checklistPergunta.Opcoes.list = data;
}

function RecarregarGridOpcoes() {
    var html = "";
    var lista = ObterListaOrdenada();

    $.each(lista, function (i, opcao) {
        html += '<tr data-position="' + opcao.Ordem + '" data-codigo="' + opcao.Codigo + '" id="sort_opcao_' + opcao.Ordem + '">';

        html += '<td class="hide">' + opcao.Ordem + '</td>';
        if (isEscala()) {
            html += '<td>' + opcao.Descricao + ' (' + opcao.Valor + ')</td>';
        } else {
            html += '<td>' + opcao.Descricao + '</td>';
        }

        html += '<td>' + opcao.CodigoIntegracao + '</td>';
        html += '<td class="text-align-center"><a href="javascript:;" onclick="editarOpcaoClick(\'' + opcao.Codigo + '\')">Editar</a></td>';

        html += '</tr>';
    });

    _gridOpcoes.RecarregarGrid(html);
}

function LimparCamposOpcao() {
    editandoOpcao = false;
    _checklistPergunta.CodigoOpcao.val(_checklistPergunta.CodigoOpcao.def);
    _checklistPergunta.DescricaoOpcao.val(_checklistPergunta.DescricaoOpcao.def);
    _checklistPergunta.ValorOpcao.val(_checklistPergunta.ValorOpcao.def);
    _checklistPergunta.CodigoIntegracaoOpcao.val(_checklistPergunta.CodigoIntegracaoOpcao.def);
    _checklistPergunta.OpcaoImpeditiva.val(_checklistPergunta.OpcaoImpeditiva.def);

    _checklistPergunta.AdicionarOpcao.visible(true);
    _checklistPergunta.AtualizarOpcao.visible(false);
    _checklistPergunta.ExcluirOpcao.visible(false);
}

function FocarCampoDescricao() {
    $('#' + _checklistPergunta.DescricaoOpcao.id).focus();
}

function LinhasReordenadas() {
    var opcoesOrdenadas = [];
    var lista = ObterListaOrdenada();

    var BuscaPorCodigo = function (codigo) {
        for (var i in lista)
            if (lista[i].Codigo == codigo)
                return lista[i];

        return null;
    }

    $.each(_gridOpcoes.ObterOrdencao(), function (i, obj) {
        var $this = document.getElementById(obj.id);
        var opt = BuscaPorCodigo($this.dataset.codigo);
        opt.Ordem = i;
        opcoesOrdenadas.push(opt);
    });

    SetaLista(opcoesOrdenadas);
    RecarregarGridOpcoes();
}

function LimparOpcoes() {
    SetaLista([]);
    RecarregarGridOpcoes();
    LimparCamposOpcao();
}

function EditarOpcoes() {
    SetaLista(_checklistPergunta.Opcoes.list);
    RecarregarGridOpcoes();
}