/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="FaturamentoMensalCliente.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridServicoExtra;

var ServicoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoServico = PropertyEntity({ type: types.map, val: "" });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Quantidade = PropertyEntity({ type: types.map, val: "" });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: "" });
    this.ValorTotalServicoExtra = PropertyEntity({ type: types.map, val: "" });
    this.DataLancamentoServico = PropertyEntity({ type: types.map, val: "" });
    this.DataLancamentoServicoAte = PropertyEntity({ type: types.map, val: "" });
    this.TipoObservacaoServicoExtra = PropertyEntity({ type: types.map, val: "" });
    this.ObservacaoServicoExtra = PropertyEntity({ type: types.map, val: "" });
    this.NumeroPedidoCompraExtra = PropertyEntity({ type: types.map, val: "" });
    this.NumeroPedidoItemCompraExtra = PropertyEntity({ type: types.map, val: "" });
    this.Historico = PropertyEntity({ type: types.map, val: "" });
}

//*******EVENTOS*******

function loadServicoExtra() {

    preencherServicoExtra();

    new BuscarServicoTMS(_faturamentoMensalCliente.ServicoExtra, function (data) {
        _faturamentoMensalCliente.ServicoExtra.codEntity(data.Codigo);
        _faturamentoMensalCliente.ServicoExtra.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = parseFloat(data.ValorVenda.toString().replace(".", "").replace(",", ".")).toFixed(2);
            valorUnitario = parseFloat(valorUnitario);
            _faturamentoMensalCliente.ValorUnitario.val(Globalize.format(valorUnitario, "n2"));
            CalcularTotalServico();
        }
    });
}

function CalcularTotalServico() {
    var quantidade = parseFloat(_faturamentoMensalCliente.Quantidade.val().toString().replace(".", "").replace(",", "."));
    quantidade = parseFloat(quantidade);

    var valorUnitario = parseFloat(_faturamentoMensalCliente.ValorUnitario.val().toString().replace(".", "").replace(",", "."));
    valorUnitario = parseFloat(valorUnitario);

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _faturamentoMensalCliente.ValorTotalServicoExtra.val(Globalize.format(valorTotal, "n2"));
    }
}

function AdicionarServicoExtraClick(e, sender) {
    var tudoCerto = true;
    if (_faturamentoMensalCliente.ServicoExtra.codEntity() == "" || _faturamentoMensalCliente.ServicoExtra.codEntity() == "0"
        || _faturamentoMensalCliente.Quantidade.val() == "" || _faturamentoMensalCliente.Quantidade.val() == "0"
        || _faturamentoMensalCliente.ValorTotalServicoExtra.val() == "" || _faturamentoMensalCliente.ValorTotalServicoExtra.val() == "0"
        || _faturamentoMensalCliente.ValorUnitario.val() == "" || _faturamentoMensalCliente.ValorUnitario.val() == "0")
        tudoCerto = false;

    if (tudoCerto) {
        if (_faturamentoMensalCliente.CodigoServicoExtra.val() != "") {
            $.each(_faturamentoMensalCliente.ServicosExtras.list, function (i, servico) {
                if (servico != null && _faturamentoMensalCliente.CodigoServicoExtra.val() == servico.Codigo.val)
                    _faturamentoMensalCliente.ServicosExtras.list.splice(i, 1);
            });
        }
        var map = new ServicoMap();

        if (_faturamentoMensalCliente.CodigoServicoExtra.val() != "")
            map.Codigo.val = _faturamentoMensalCliente.CodigoServicoExtra.val();
        else
            map.Codigo.val = (_faturamentoMensalCliente.ServicosExtras.list.length + 1) * -1;

        map.CodigoServico.val = _faturamentoMensalCliente.ServicoExtra.codEntity();
        map.Descricao.val = _faturamentoMensalCliente.ServicoExtra.val();
        map.Quantidade.val = _faturamentoMensalCliente.Quantidade.val();
        map.ValorUnitario.val = _faturamentoMensalCliente.ValorUnitario.val();
        map.ValorTotalServicoExtra.val = _faturamentoMensalCliente.ValorTotalServicoExtra.val();
        map.DataLancamentoServico.val = _faturamentoMensalCliente.DataLancamentoServico.val();
        map.DataLancamentoServicoAte.val = _faturamentoMensalCliente.DataLancamentoServicoAte.val();
        map.TipoObservacaoServicoExtra.val = _faturamentoMensalCliente.TipoObservacaoServicoExtra.val();
        map.ObservacaoServicoExtra.val = _faturamentoMensalCliente.ObservacaoServicoExtra.val();

        map.NumeroPedidoCompraExtra.val = _faturamentoMensalCliente.NumeroPedidoCompraExtra.val();
        map.NumeroPedidoItemCompraExtra.val = _faturamentoMensalCliente.NumeroPedidoItemCompraExtra.val();
        map.Historico.val = _faturamentoMensalCliente.Historico.val();

        _faturamentoMensalCliente.ServicosExtras.list.push(map);

        recarregarGridServicoExtra();
        limparServicoExtra();
        $("#" + _faturamentoMensalCliente.ServicoExtra.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento do serivço extra!");
    }
}

function ExcluirServicoExtraClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover o serviço extra " + e.Descricao + " deste faturamento?", function () {
        $.each(_faturamentoMensalCliente.ServicosExtras.list, function (i, servico) {
            if (servico != null && servico.Codigo.val != null && e != null && e.Codigo != null && e.Codigo == servico.Codigo.val)
                _faturamentoMensalCliente.ServicosExtras.list.splice(i, 1);
        });
        recarregarGridServicoExtra();
    });
}

function EditarServicoExtraClick(e) {
    _faturamentoMensalCliente.CodigoServicoExtra.val(e.Codigo);
    _faturamentoMensalCliente.ServicoExtra.codEntity(e.CodigoServico);
    _faturamentoMensalCliente.ServicoExtra.val(e.Descricao);
    _faturamentoMensalCliente.Quantidade.val(e.Quantidade);
    _faturamentoMensalCliente.ValorUnitario.val(e.ValorUnitario);
    _faturamentoMensalCliente.ValorTotalServicoExtra.val(e.ValorTotalServicoExtra);
    _faturamentoMensalCliente.DataLancamentoServico.val(e.DataLancamentoServico);
    _faturamentoMensalCliente.DataLancamentoServicoAte.val(e.DataLancamentoServicoAte);
    _faturamentoMensalCliente.TipoObservacaoServicoExtra.val(e.TipoObservacaoServicoExtra);
    _faturamentoMensalCliente.ObservacaoServicoExtra.val(e.ObservacaoServicoExtra);

    _faturamentoMensalCliente.NumeroPedidoCompraExtra.val(e.NumeroPedidoCompraExtra);
    _faturamentoMensalCliente.NumeroPedidoItemCompraExtra.val(e.NumeroPedidoItemCompraExtra);
    _faturamentoMensalCliente.Historico.val(e.Historico);
}

function preencherServicoExtra() {
    var detalhe = {
        descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) {
            EditarServicoExtraClick(data)
        }, tamanho: "10", icone: ""
    };
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            ExcluirServicoExtraClick(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhe);
    menuOpcoes.opcoes.push(excluir);

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "50%" },
    { data: "Quantidade", title: "Qtd.", width: "15%" },
    { data: "ValorUnitario", title: "Val. Unitário", width: "15%" },
    { data: "ValorTotalServicoExtra", title: "Val. Total", width: "15%" }
    ];

    _gridServicoExtra = new BasicDataTable(_faturamentoMensalCliente.ServicosExtras.idGrid, header, menuOpcoes);
    recarregarGridServicoExtra();
}

function recarregarGridServicoExtra() {
    var data = new Array();
    $.each(_faturamentoMensalCliente.ServicosExtras.list, function (i, Servico) {
        var obj = new Object();

        obj.Codigo = Servico.Codigo.val;
        obj.CodigoServico = Servico.CodigoServico.val;
        obj.Descricao = Servico.Descricao.val;
        obj.Quantidade = Servico.Quantidade.val;
        obj.ValorUnitario = Servico.ValorUnitario.val;
        obj.ValorTotalServicoExtra = Servico.ValorTotalServicoExtra.val;
        if (Servico.DataLancamentoServico != undefined)
            obj.DataLancamentoServico = Servico.DataLancamentoServico.val;
        else
            obj.DataLancamentoServico = "";
        if (Servico.DataLancamentoServicoAte != undefined)
            obj.DataLancamentoServicoAte = Servico.DataLancamentoServicoAte.val;
        else
            obj.DataLancamentoServicoAte = "";
        obj.TipoObservacaoServicoExtra = Servico.TipoObservacaoServicoExtra.val;
        obj.ObservacaoServicoExtra = Servico.ObservacaoServicoExtra.val;

        if (Servico.NumeroPedidoCompraExtra != undefined)
            obj.NumeroPedidoCompraExtra = Servico.NumeroPedidoCompraExtra.val;
        else
            obj.NumeroPedidoCompraExtra = "";
        if (Servico.NumeroPedidoItemCompraExtra != undefined)
            obj.NumeroPedidoItemCompraExtra = Servico.NumeroPedidoItemCompraExtra.val;
        else
            obj.NumeroPedidoItemCompraExtra = "";
        if (Servico.Historico != undefined)
            obj.Historico = Servico.Historico.val;
        else
            obj.Historico = "";

        data.push(obj);
    });
    _gridServicoExtra.CarregarGrid(data);
}

function limparServicoExtra() {
    $("#" + _faturamentoMensalCliente.Quantidade.id).val("");
    $("#" + _faturamentoMensalCliente.ValorUnitario.id).val("");
    $("#" + _faturamentoMensalCliente.TipoObservacaoServicoExtra.id).val($("#" + _faturamentoMensalCliente.TipoObservacaoServicoExtra.id + " option:last").val());
    $("#" + _faturamentoMensalCliente.ValorTotalServicoExtra.id).val("");
    $("#" + _faturamentoMensalCliente.DataLancamentoServico.id).val("");
    $("#" + _faturamentoMensalCliente.DataLancamentoServicoAte.id).val("");
    $("#" + _faturamentoMensalCliente.ObservacaoServicoExtra.id).val("");

    _faturamentoMensalCliente.CodigoServicoExtra.val("");
    _faturamentoMensalCliente.Quantidade.val("");
    _faturamentoMensalCliente.ValorUnitario.val("");
    _faturamentoMensalCliente.ValorTotalServicoExtra.val("");
    _faturamentoMensalCliente.DataLancamentoServico.val("");
    _faturamentoMensalCliente.DataLancamentoServicoAte.val("");
    _faturamentoMensalCliente.ObservacaoServicoExtra.val("");

    _faturamentoMensalCliente.NumeroPedidoCompraExtra.val("");
    _faturamentoMensalCliente.NumeroPedidoItemCompraExtra.val("");
    _faturamentoMensalCliente.Historico.val("");

    LimparCampoEntity(_faturamentoMensalCliente.ServicoExtra);
}