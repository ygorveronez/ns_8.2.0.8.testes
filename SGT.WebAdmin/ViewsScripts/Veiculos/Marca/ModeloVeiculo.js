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
/// <reference path="MarcaVeiculo.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridModeloVeiculo;

var ModeloMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoFIPE = PropertyEntity({ type: types.map, val: "" });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.MediaPadrao = PropertyEntity({ type: types.map, val: "" });
    this.MediaPadraoVazio = PropertyEntity({ type: types.map, val: "" });
    this.AlturaEmMetros = PropertyEntity({ type: types.map, val: "" });
    this.MediaMinima = PropertyEntity({ type: types.map, val: "" });
    this.MediaMaxima = PropertyEntity({ type: types.map, val: "" });
    this.NumeroEixo = PropertyEntity({ type: types.map, val: "" });
    this.SimNao = PropertyEntity({ type: types.map, val: "" });
    this.Status = PropertyEntity({ type: types.map, val: "" });
    this.Produto = PropertyEntity({ type: types.map, val: "" });
    this.CodigoProduto = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadModeloVeiculo() {

    loadGridModeloVeiculo();

    new BuscarTipoCombustiveisTMS(_marcaVeiculo.TipoCombustivel);
}

function adicionarModeloVeiculoClick(e, sender) {
    var tudoCerto = true;
    if (_marcaVeiculo.DescricaoModelo.val() == "")
        tudoCerto = false;
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe) {
        if (tudoCerto && (_marcaVeiculo.NumeroEixosModelo.val() == "" || _marcaVeiculo.NumeroEixosModelo.val() == "0"))
            tudoCerto = false;
        else if (tudoCerto && _marcaVeiculo.TipoVeiculo.val() == 1) {
            if (tudoCerto && (_marcaVeiculo.MediaPadrao.val() == "" || _marcaVeiculo.MediaPadrao.val() == "0"))
                tudoCerto = false;
            else if (tudoCerto && (_marcaVeiculo.TipoCombustivel.codEntity() == "" || _marcaVeiculo.TipoCombustivel.codEntity() == "0"))
                tudoCerto = false;
        }
    }

    if (tudoCerto) {
        if (_marcaVeiculo.CodigoModelo.val() > 0) {
            $.each(_marcaVeiculo.Modelos.list, function (i, modelo) {
                if (modelo != null && _marcaVeiculo.CodigoModelo.val() == modelo.Codigo.val)
                    _marcaVeiculo.Modelos.list.splice(i, 1);
            });
        }
        var map = new ModeloMap();

        if (_marcaVeiculo.CodigoModelo.val() > 0)
            map.Codigo.val = _marcaVeiculo.CodigoModelo.val();
        else
            map.Codigo.val = (_marcaVeiculo.Modelos.list.length + 1) * -1;

        map.CodigoFIPE.val = _marcaVeiculo.CodigoFIPE.val();
        map.Descricao.val = _marcaVeiculo.DescricaoModelo.val();
        map.MediaPadrao.val = _marcaVeiculo.MediaPadrao.val();
        map.MediaPadraoVazio.val = _marcaVeiculo.MediaPadraoVazio.val();
        map.AlturaEmMetros.val = _marcaVeiculo.AlturaEmMetros.val();
        map.MediaMinima.val = _marcaVeiculo.MediaMinima.val();
        map.MediaMaxima.val = _marcaVeiculo.MediaMaxima.val();
        map.NumeroEixo.val = _marcaVeiculo.NumeroEixosModelo.val();
        map.SimNao.val = _marcaVeiculo.MotorArla.val();
        map.Status.val = _marcaVeiculo.AtivoModelo.val();
        map.Produto.val = _marcaVeiculo.TipoCombustivel.val();
        map.CodigoProduto.val = _marcaVeiculo.TipoCombustivel.codEntity();

        _marcaVeiculo.Modelos.list.push(map);

        recarregarGridModeloVeiculo();
        limparModeloVeiculo();
        _marcaVeiculo.AdicionarModelo.text("Adicionar Modelo");
        $("#" + _marcaVeiculo.DescricaoModelo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no lançamento do modelo!");
    }
}

function excluirModeloVeiculo(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover o modelo " + e.Descricao + " desta Marca de Veículo?", function () {
        $.each(_marcaVeiculo.Modelos.list, function (i, modelo) {
            if (modelo != null && modelo.Codigo.val != null && e != null && e.Codigo != null && e.Codigo == modelo.Codigo.val)
                _marcaVeiculo.Modelos.list.splice(i, 1);
        });
        recarregarGridModeloVeiculo();
    });
}

function EditarModeloClick(e) {
    console.log(e);
    _marcaVeiculo.DescricaoModelo.val(e.Descricao);
    _marcaVeiculo.NumeroEixosModelo.val(e.NumeroEixo);
    _marcaVeiculo.AtivoModelo.val(e.Status);
    _marcaVeiculo.TipoCombustivel.codEntity(e.CodigoProduto);
    _marcaVeiculo.TipoCombustivel.val(e.Produto);
    _marcaVeiculo.MotorArla.val(e.SimNao);
    _marcaVeiculo.MediaPadrao.val(e.MediaPadrao);
    _marcaVeiculo.MediaPadraoVazio.val(e.MediaPadraoVazio);
    _marcaVeiculo.AlturaEmMetros.val(e.AlturaEmMetros);
    _marcaVeiculo.MediaMinima.val(e.MediaMinima);
    _marcaVeiculo.MediaMaxima.val(e.MediaMaxima);
    _marcaVeiculo.CodigoFIPE.val(e.CodigoFIPE);
    _marcaVeiculo.CodigoModelo.val(e.Codigo);

    _marcaVeiculo.AdicionarModelo.text("Atualizar Modelo");
}

function loadGridModeloVeiculo() {
    var detalhe = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { EditarModeloClick(data) }, tamanho: "10", icone: "" };
    var excluir = { descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) { excluirModeloVeiculo(data) }, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhe);
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "60%" },
        { data: "NumeroEixo", title: "Qtd. Eixos", width: "20%" },
        { data: "CodigoFIPE", visible: false },
        { data: "MediaPadrao", visible: false },
        { data: "MediaPadraoVazio", visible: false },
        { data: "AlturaEmMetros", visible: false },
        { data: "MediaMinima", visible: false },
        { data: "MediaMaxima", visible: false },
        { data: "SimNao", visible: false },
        { data: "Status", visible: false },
        { data: "Produto", visible: false },
        { data: "CodigoProduto", visible: false }
    ];

    _gridModeloVeiculo = new BasicDataTable(_marcaVeiculo.Modelos.idGrid, header, menuOpcoes);
    recarregarGridModeloVeiculo();
}

function recarregarGridModeloVeiculo() {
    var data = new Array();
    $.each(_marcaVeiculo.Modelos.list, function (i, Modelo) {
        var obj = new Object();

        obj.Codigo = Modelo.Codigo.val;
        obj.CodigoFIPE = Modelo.CodigoFIPE.val;
        obj.Descricao = Modelo.Descricao.val;
        obj.MediaPadrao = Modelo.MediaPadrao.val;
        obj.MediaPadraoVazio = Modelo.MediaPadraoVazio.val;
        obj.AlturaEmMetros = Modelo.AlturaEmMetros.val;
        obj.MediaMinima = Modelo.MediaMinima.val;
        obj.MediaMaxima = Modelo.MediaMaxima.val;
        obj.NumeroEixo = Modelo.NumeroEixo.val;
        obj.SimNao = Modelo.SimNao.val;
        obj.Status = Modelo.Status.val;
        obj.Produto = Modelo.Produto.val;
        obj.CodigoProduto = Modelo.CodigoProduto.val;

        data.push(obj);
    });

    _gridModeloVeiculo.CarregarGrid(data);
}

function limparModeloVeiculo() {
    $("#" + _marcaVeiculo.DescricaoModelo.id).val("");
    $("#" + _marcaVeiculo.NumeroEixosModelo.id).val("");
    $("#" + _marcaVeiculo.AtivoModelo.id).val($("#" + _marcaVeiculo.AtivoModelo.id + " option:first").val());
    $("#" + _marcaVeiculo.MotorArla.id).val($("#" + _marcaVeiculo.MotorArla.id + " option:first").val());
    $("#" + _marcaVeiculo.MediaPadrao.id).val("");
    $("#" + _marcaVeiculo.MediaPadraoVazio.id).val("");
    $("#" + _marcaVeiculo.AlturaEmMetros.id).val("");
    $("#" + _marcaVeiculo.MediaMinima.id).val("");
    $("#" + _marcaVeiculo.MediaMaxima.id).val("");
    $("#" + _marcaVeiculo.CodigoFIPE.id).val("");

    _marcaVeiculo.DescricaoModelo.val("");
    _marcaVeiculo.NumeroEixosModelo.val("");
    _marcaVeiculo.MediaPadrao.val("");
    _marcaVeiculo.MediaPadraoVazio.val("");
    _marcaVeiculo.AlturaEmMetros.val("");
    _marcaVeiculo.MediaMinima.val("");
    _marcaVeiculo.MediaMaxima.val("");
    _marcaVeiculo.CodigoFIPE.val("");

    LimparCampoEntity(_marcaVeiculo.TipoCombustivel);
}