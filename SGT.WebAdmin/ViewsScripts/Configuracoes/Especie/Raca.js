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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Especie.js" />

var _gridRaca;

var RacaMap = function () {
    this.PosicaoGrid = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Ativo = PropertyEntity({ type: types.map, val: "" });
};

function loadRacas() {
    LoadGridRacas();
}

function LoadGridRacas() {
    var detalhe = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { EditarRacaClick(data) }, tamanho: "10", icone: "" };
    var excluir = { descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) { ExcluirRacaClick(data) }, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhe);
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "PosicaoGrid", visible: false},
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "60%" },
        { data: "Ativo", title: "Status", width: "20%" },
    ];

    _gridRaca = new BasicDataTable(_formulario.Racas.idGrid, header, menuOpcoes);
    RecarregarGridRacas();
}

function RecarregarGridRacas() {
    var data = new Array();
    $.each(_formulario.Racas.list, function (i, Raca) {
        var obj = new Object();
      
        obj.PosicaoGrid = Raca.PosicaoGrid.val;
        obj.Codigo = Raca.Codigo.val;
        obj.Descricao = Raca.Descricao.val;
        obj.Ativo = Raca.Ativo.val ? "Ativo" : "Inativo";

        data.push(obj);
    });

    _gridRaca.CarregarGrid(data);
}

function LimparGridRacas() {
    _gridRaca.CarregarGrid(new Array());
}

function AdicionarRacaClick(e, sender) {

    var possuiDescricaoRaca = !(_formulario.DescricaoRaca.val() == null || _formulario.DescricaoRaca.val().trim() == '');
    if (!possuiDescricaoRaca)
        return;

    var map = new RacaMap();
   
    if (_formulario.CodigoRaca.val() > 0)
        map.Codigo.val = _formulario.CodigoRaca.val();

    map.Descricao.val = _formulario.DescricaoRaca.val().trim();
    map.Ativo.val = _formulario.AtivoRaca.val();

    if (_formulario.PosicaoGrid.val() == '') {
        map.PosicaoGrid.val = _formulario.Racas.list.length + 1;
        _formulario.Racas.list.push(map);
    }
    else
    {
        map.PosicaoGrid.val = _formulario.PosicaoGrid.val()

        // Atualizo item atual da grid
        $.each(_formulario.Racas.list, function (i, raca) {
            if (raca != null && _formulario.PosicaoGrid.val() == raca.PosicaoGrid.val) {
                _formulario.Racas.list.splice(i, 1, map);
            }
        });
    }

    RecarregarGridRacas();
    LimparRaca();
    _formulario.AdicionarRaca.text("Adicionar Raça");
    $("#" + _formulario.DescricaoRaca.id).focus();
}

function EditarRacaClick(e) {

    _formulario.PosicaoGrid.val(e.PosicaoGrid);
    _formulario.CodigoRaca.val(e.Codigo);
    _formulario.DescricaoRaca.val(e.Descricao);
    _formulario.AtivoRaca.val(e.Ativo);

    _formulario.AdicionarRaca.text("Atualizar Raça");

}

function ExcluirRacaClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a raça " + e.Descricao + " desta espécie?", function () {
        $.each(_formulario.Racas.list, function (i, raca) {
            if (raca != null && raca.PosicaoGrid.val != null && e != null && e.PosicaoGrid != null && e.PosicaoGrid == raca.PosicaoGrid.val)
                _formulario.Racas.list.splice(i, 1);
        });

        RecarregarGridRacas();
    });
}

function LimparRaca() {
    _formulario.PosicaoGrid.val('');
    _formulario.CodigoRaca.val('');
    _formulario.DescricaoRaca.val('');
    _formulario.AtivoRaca.val(true);
}