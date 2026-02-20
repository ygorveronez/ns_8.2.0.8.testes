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
/// <reference path="RFICheckList.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _gridOpcoes;
var AIOrdem = 0;
var editandoOpcao = false;

//*******EVENTOS*******
function AdicionarOpcaoRFI() {

    if (!validarCampo(_checkListOpcoesRFI.DescricaoOpcao.val())) {
        RecarregarGridOpcoes();
        return;
    }

    let opcoes = ObterListaOrdenada();

    let opcao = {
        Codigo: guid(),
        Ordem: opcoes.length,
        Descricao: _checkListOpcoesRFI.DescricaoOpcao.val(),
    };
    opcoes.push(opcao);

    SetaLista(opcoes);

    RecarregarGridOpcoes();
    LimparCamposOpcao();
    FocarCampoDescricao();
}

function AdicionarOpcaoEnterRFI(ko, e) {
    if (e && e.keyCode === 13) {
        if (editandoOpcao) {
            AtualizarOpcaoRFI();
        } else {
            AtualizarOpcaoRFI();
        }
    }

    return true;
}

function AtualizarOpcaoRFI() {

    if (!validarCampo(_checkListOpcoesRFI.DescricaoOpcao.val())) {
        RecarregarGridOpcoes();
        return;
    }
    let opcoes = ObterListaOrdenada();

    let opcao = {
        Codigo: _checkListOpcoesRFI.CodigoOpcao.val(),
        Ordem: _checkListOpcoesRFI.OrdemOpcao.val(),
        Descricao: _checkListOpcoesRFI.DescricaoOpcao.val(),
    };

    for (let i in opcoes) {
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

function ExcluirOpcaoRFI() {
    let opcoes = ObterListaOrdenada();
    let codigo = _checkListOpcoesRFI.CodigoOpcao.val();

    for (let i in opcoes) {
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

function CancelarOpcaoRFI() {
    LimparCamposOpcao();
}

function EditarOpcaoClickRFI(codigo) {

    let opcoes = ObterListaOrdenada();
    let opcao = null;

    for (let i in opcoes) {
        if (opcoes[i].Codigo == codigo) {
            opcao = opcoes[i];
            break;
        }
    }

    if (opcao != null) {
        editandoOpcao = true;
        _checkListOpcoesRFI.CodigoOpcao.val(opcao.Codigo);
        _checkListOpcoesRFI.OrdemOpcao.val(opcao.Ordem);
        _checkListOpcoesRFI.DescricaoOpcao.val(opcao.Descricao);

        _checkListOpcoesRFI.AdicionarOpcao.visible(false);
        _checkListOpcoesRFI.AtualizarOpcao.visible(true);
        _checkListOpcoesRFI.ExcluirOpcao.visible(true);
    }
}

//*******MÉTODOS*******
function GridOpcoesRFI() {
    _gridOpcoes = new GridReordering("", _checkListOpcoesRFI.Opcoes.idGrid, GeraHeadTable());
    _gridOpcoes.CarregarGrid();

    $("#" + _checkListOpcoesRFI.Opcoes.idGrid).on('sortstop', function () {
        LinhasReordenadas();
    });
}

function GeraHeadTable() {
    return '<tr>' +
        '<th width="10%" class="text-align-left hide">Ordem</th>' +
        '<th width="70%" class="text-align-left">Descrição</th>' +
        '<th width="15%" class="text-align-center">Editar</th>' +
        '</tr>';
}

function ObterListaOrdenada() {
    let regras = _checkListOpcoesRFI.Opcoes.list.slice();

    regras.sort(function (a, b) { return a.Ordem - b.Ordem });
    return regras;
}

function SetaLista(data) {
    _checkListOpcoesRFI.Opcoes.list = data.slice();
    _checkListOpcoesRFI.Opcoes.val(JSON.stringify(data));
}

function RecarregarGridOpcoes() {
    let html = "";
    let lista = ObterListaOrdenada();

    $.each(lista, function (i, opcao) {
        html += '<tr data-position="' + opcao.Ordem + '" data-codigo="' + opcao.Codigo + '" id="sort_opcao_' + opcao.Ordem + '">';

        html += '<td class="hide">' + opcao.Ordem + '</td>';

        html += '<td>' + opcao.Descricao + '</td>';

        html += '<td class="text-align-center"><a href="javascript:;" onclick="EditarOpcaoClickRFI(\'' + opcao.Codigo + '\')">Editar</a></td>';

        html += '</tr>';
    });

    _gridOpcoes.RecarregarGrid(html);
}

function LimparCamposOpcao() {
    editandoOpcao = false;
    _checkListOpcoesRFI.CodigoOpcao.val(_checkListOpcoesRFI.CodigoOpcao.def);
    _checkListOpcoesRFI.DescricaoOpcao.val(_checkListOpcoesRFI.DescricaoOpcao.def);

    _checkListOpcoesRFI.AdicionarOpcao.visible(true);
    _checkListOpcoesRFI.AtualizarOpcao.visible(false);
    _checkListOpcoesRFI.ExcluirOpcao.visible(false);
}

function FocarCampoDescricao() {
    $('#' + _checkListOpcoesRFI.DescricaoOpcao.id).focus();
}

function LinhasReordenadas() {
    let opcoesOrdenadas = [];
    let lista = ObterListaOrdenada();

    let BuscaPorCodigo = function (codigo) {
        for (let i in lista)
            if (lista[i].Codigo == codigo)
                return lista[i];

        return null;
    }

    $.each(_gridOpcoes.ObterOrdencao(), function (i, obj) {
        let $this = document.getElementById(obj.id);
        let opt = BuscaPorCodigo($this.dataset.codigo);
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
    SetaLista(_checkListOpcoesRFI.Opcoes.val());
    RecarregarGridOpcoes();
}

function validarCampo(valor) {
    if (valor === null || valor === "" || typeof valor === undefined) {
        return false;
    } else {
        return true;
    }
}