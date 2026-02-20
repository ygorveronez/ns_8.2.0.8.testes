/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="PlanoEmissaoFaturamento.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridPlanoEmissaoFaturamentoValor;

var PlanoEmissaoFaturamentoValor = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.QuantidadeInicial = PropertyEntity({ type: types.map, val: "" });
    this.QuantidadeFinal = PropertyEntity({ type: types.map, val: "" });
    this.Valor = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoValor = PropertyEntity({ type: types.map, val: "" });
    this.TipoObservacaoValor = PropertyEntity({ type: types.map, val: "" });
    this.ObservacaoValor = PropertyEntity({ type: types.map, val: "" });
}

//*******EVENTOS*******

function loadPlanoEmissaoFaturamentoValor() {

    preencherPlanoEmissaoFaturamentoValor();
}

function AdicionarPlanoEmissaoFaturamentoValorClick(e, sender) {
    var tudoCerto = true;
    if (_planoEmissaoFaturamento.QuantidadeFinal.val() == "" || _planoEmissaoFaturamento.QuantidadeFinal.val() == "0"
        || _planoEmissaoFaturamento.Valor.val() == "" || _planoEmissaoFaturamento.Valor.val() == "0")
        tudoCerto = false;

    if (tudoCerto) {
        if (_planoEmissaoFaturamento.CodigoValor.val() > 0) {
            $.each(_planoEmissaoFaturamento.ValoresPlano.list, function (i, valor) {
                if (valor != null && _planoEmissaoFaturamento.CodigoValor.val() == valor.Codigo.val)
                    _planoEmissaoFaturamento.ValoresPlano.list.splice(i, 1);
            });
        }
        var map = new PlanoEmissaoFaturamentoValor();

        if (_planoEmissaoFaturamento.CodigoValor.val() > 0)
            map.Codigo.val = _planoEmissaoFaturamento.CodigoValor.val();
        else
            map.Codigo.val = (_planoEmissaoFaturamento.ValoresPlano.list.length + 1) * -1;

        map.QuantidadeInicial.val = _planoEmissaoFaturamento.QuantidadeInicial.val();
        map.QuantidadeFinal.val = _planoEmissaoFaturamento.QuantidadeFinal.val();
        map.Valor.val = _planoEmissaoFaturamento.Valor.val();
        map.DescricaoValor.val = _planoEmissaoFaturamento.DescricaoValor.val();
        map.TipoObservacaoValor.val = _planoEmissaoFaturamento.TipoObservacaoValor.val();
        map.ObservacaoValor.val = _planoEmissaoFaturamento.ObservacaoValor.val();

        _planoEmissaoFaturamento.ValoresPlano.list.push(map);

        recarregarGridPlanoEmissaoFaturamentoValor();
        limparPlanoEmissaoFaturamentoValor();
        $("#" + _planoEmissaoFaturamento.QuantidadeInicial.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento do valor!");
    }
}

function ExcluirPlanoEmissaoFaturamentoValorClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover o plano selecionado?", function () {
        $.each(_planoEmissaoFaturamento.ValoresPlano.list, function (i, valor) {
            if (valor != null && valor.Codigo.val != null && e != null && e.Codigo != null && e.Codigo == valor.Codigo.val)
                _planoEmissaoFaturamento.ValoresPlano.list.splice(i, 1);
        });
        recarregarGridPlanoEmissaoFaturamentoValor();
    });
}

function EditarPlanoEmissaoFaturamentoValorClick(e) {
    _planoEmissaoFaturamento.CodigoValor.val(e.Codigo);
    _planoEmissaoFaturamento.QuantidadeInicial.val(e.QuantidadeInicial);
    _planoEmissaoFaturamento.QuantidadeFinal.val(e.QuantidadeFinal);
    _planoEmissaoFaturamento.Valor.val(e.Valor);
    _planoEmissaoFaturamento.DescricaoValor.val(e.DescricaoValor);
    _planoEmissaoFaturamento.TipoObservacaoValor.val(e.TipoObservacaoValor);
    _planoEmissaoFaturamento.ObservacaoValor.val(e.ObservacaoValor);
}

function preencherPlanoEmissaoFaturamentoValor() {
    var detalhe = {
        descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) {
            EditarPlanoEmissaoFaturamentoValorClick(data)
        }, tamanho: "10", icone: ""
    };
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            ExcluirPlanoEmissaoFaturamentoValorClick(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhe);
    menuOpcoes.opcoes.push(excluir);

    var header = [{ data: "Codigo", visible: false },
    { data: "DescricaoValor", title: "Descrição", width: "30%" },
    { data: "QuantidadeInicial", title: "Qtd. Inicial", width: "15%" },
    { data: "QuantidadeFinal", title: "Qtd. Final", width: "15%" },
    { data: "Valor", title: "Valor", width: "15%" }
    ];

    _gridPlanoEmissaoFaturamentoValor = new BasicDataTable(_planoEmissaoFaturamento.ValoresPlano.idGrid, header, menuOpcoes);
    recarregarGridPlanoEmissaoFaturamentoValor();
}

function recarregarGridPlanoEmissaoFaturamentoValor() {
    var data = new Array();
    $.each(_planoEmissaoFaturamento.ValoresPlano.list, function (i, Valor) {
        var obj = new Object();

        obj.Codigo = Valor.Codigo.val;
        obj.QuantidadeInicial = Valor.QuantidadeInicial.val;
        obj.QuantidadeFinal = Valor.QuantidadeFinal.val;
        obj.Valor = Valor.Valor.val;
        obj.DescricaoValor = Valor.DescricaoValor.val;
        obj.TipoObservacaoValor = Valor.TipoObservacaoValor.val;
        obj.ObservacaoValor = Valor.ObservacaoValor.val;

        data.push(obj);
    });
    _gridPlanoEmissaoFaturamentoValor.CarregarGrid(data);
}

function limparPlanoEmissaoFaturamentoValor() {
    $("#" + _planoEmissaoFaturamento.QuantidadeInicial.id).val("");
    $("#" + _planoEmissaoFaturamento.QuantidadeFinal.id).val("");
    $("#" + _planoEmissaoFaturamento.Valor.id).val("");
    $("#" + _planoEmissaoFaturamento.DescricaoValor.id).val("");
    $("#" + _planoEmissaoFaturamento.TipoObservacaoValor.id).val($("#" + _planoEmissaoFaturamento.TipoObservacaoValor.id + " option:last").val());
    $("#" + _planoEmissaoFaturamento.ObservacaoValor.id).val("");

    _planoEmissaoFaturamento.QuantidadeInicial.val("");
    _planoEmissaoFaturamento.QuantidadeFinal.val("");
    _planoEmissaoFaturamento.Valor.val("");
    _planoEmissaoFaturamento.DescricaoValor.val("");
    _planoEmissaoFaturamento.ObservacaoValor.val("");
    _planoEmissaoFaturamento.CodigoValor.val(0);    
}