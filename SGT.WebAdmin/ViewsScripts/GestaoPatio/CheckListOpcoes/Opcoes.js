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
/// <reference path="CheckListOpcoes.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _gridOpcoes;
var AIOrdem = 0;
var editandoOpcao = false;




//*******EVENTOS*******
function adicionarOpcao() {
    var opcoes = ObterListaOrdenada();

    var opcao = {
        Codigo: guid(),
        Ordem: opcoes.length,
        Descricao: _checkListOpcoes.DescricaoOpcao.val(),
        Valor: _checkListOpcoes.ValorOpcao.val(),
        CodigoIntegracao: _checkListOpcoes.CodigoIntegracaoOpcao.val(),
        OpcaoImpeditiva: _checkListOpcoes.OpcaoImpeditiva.val()
    };
    opcoes.push(opcao);

    SetaLista(opcoes);

    RecarregarGridOpcoes();
    LimparCamposOpcao();
    FocarCampoDescricao();
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
    var opcoes = ObterListaOrdenada();

    var opcao = {
        Codigo: _checkListOpcoes.CodigoOpcao.val(),
        Ordem: _checkListOpcoes.OrdemOpcao.val(),
        Descricao: _checkListOpcoes.DescricaoOpcao.val(),
        Valor: _checkListOpcoes.ValorOpcao.val(),
        CodigoIntegracao: _checkListOpcoes.CodigoIntegracaoOpcao.val(),
        OpcaoImpeditiva: _checkListOpcoes.OpcaoImpeditiva.val()
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
}

function excluirOpcao() {
    var opcoes = ObterListaOrdenada();
    var codigo = _checkListOpcoes.CodigoOpcao.val();

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
        _checkListOpcoes.CodigoOpcao.val(opcao.Codigo);
        _checkListOpcoes.OrdemOpcao.val(opcao.Ordem);
        _checkListOpcoes.DescricaoOpcao.val(opcao.Descricao);
        _checkListOpcoes.ValorOpcao.val(opcao.Valor);
        _checkListOpcoes.CodigoIntegracaoOpcao.val(opcao.CodigoIntegracao);
        _checkListOpcoes.OpcaoImpeditiva.val(opcao.OpcaoImpeditiva);

        _checkListOpcoes.AdicionarOpcao.visible(false);
        _checkListOpcoes.AtualizarOpcao.visible(true);
        _checkListOpcoes.ExcluirOpcao.visible(true);
    }
}





//*******MÉTODOS*******
function GridOpcoes() {
    _gridOpcoes = new GridReordering("", _checkListOpcoes.Opcoes.idGrid, GeraHeadTable());
    _gridOpcoes.CarregarGrid();

    $("#" + _checkListOpcoes.Opcoes.idGrid).on('sortstop', function () {
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
    var regras = _checkListOpcoes.Opcoes.list.slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function SetaLista(data) {
    _checkListOpcoes.Opcoes.list = data.slice();
    _checkListOpcoes.Opcoes.val(JSON.stringify(data));
}

function RecarregarGridOpcoes() {
    var html = "";
    var lista = ObterListaOrdenada();

    $.each(lista, function (i, opcao) {
        html += '<tr data-position="' + opcao.Ordem + '" data-codigo="' + opcao.Codigo + '" id="sort_opcao_' + opcao.Ordem + '">';

        html += '<td class="hide">' + opcao.Ordem + '</td>';
        if (isEscalaOption()) {
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
    _checkListOpcoes.CodigoOpcao.val(_checkListOpcoes.CodigoOpcao.def);
    _checkListOpcoes.DescricaoOpcao.val(_checkListOpcoes.DescricaoOpcao.def);
    _checkListOpcoes.ValorOpcao.val(_checkListOpcoes.ValorOpcao.def);
    _checkListOpcoes.CodigoIntegracaoOpcao.val(_checkListOpcoes.CodigoIntegracaoOpcao.def);
    _checkListOpcoes.OpcaoImpeditiva.val(_checkListOpcoes.OpcaoImpeditiva.def);

    _checkListOpcoes.AdicionarOpcao.visible(true);
    _checkListOpcoes.AtualizarOpcao.visible(false);
    _checkListOpcoes.ExcluirOpcao.visible(false);
}

function FocarCampoDescricao() {
    $('#' + _checkListOpcoes.DescricaoOpcao.id).focus();
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
    SetaLista(_checkListOpcoes.Opcoes.val());
    RecarregarGridOpcoes();
}