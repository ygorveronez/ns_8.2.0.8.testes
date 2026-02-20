/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="Veiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEquipamentos;
var _equipamento;

var EquipamentoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
    this.Numero = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadEquipamento() {
    new BuscarEquipamentos(_veiculo.Equipamentos, retornoEquipamento);
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirEquipamentoClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", className: "text-align-left" },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "20%", className: "text-align-left"}
    ];
    _gridEquipamentos = new BasicDataTable(_veiculo.Equipamentos.idGrid, header, menuOpcoes);
    recarregarGridEquipamentos();
}

function retornoEquipamento(data) {
    _equipamento = new EquipamentoMap();
    _equipamento.Codigo.val = data.Codigo;
    _equipamento.Descricao.val = data.Descricao;
    _equipamento.Numero.val = data.Numero;

    _veiculo.Equipamentos.codEntity(data.Codigo);
    _veiculo.Equipamentos.val(data.Descricao);
}

function adicionarEquipamentoClick() {
    _veiculo.Equipamentos.requiredClass("form-control");
    var tudoCerto = ValidarCampoObrigatorioEntity(_veiculo.Equipamentos);
    tudoCerto = _veiculo.Equipamentos.codEntity() > 0;
    if (tudoCerto) {
        var existe = false;
        $.each(_veiculo.Equipamentos.list, function (i, Equipamento) {
            if (Equipamento.Codigo.val == _veiculo.Equipamentos.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _veiculo.Equipamentos.list.push(_equipamento);
            recarregarGridEquipamentos();
            $("#" + _veiculo.Equipamentos.id).focus();
            _veiculo.Equipamentos.requiredClass("form-control");
        } else {
            exibirMensagem("aviso", Localization.Resources.Veiculos.Veiculo.EquipamentoJaInformado, Localization.Resources.Veiculos.Veiculo.EquipamentoJaInformadoParaEsteVeiculo.format(_veiculo.Equipamentos.val()));
        }
        LimparCampoEntity(_veiculo.Equipamentos);
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        _veiculo.Equipamentos.requiredClass("form-control is-invalid");
    }
}

function excluirEquipamentoClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirEquipamento.format(data.Descricao), function () {
        var listaAtualizada = new Array();
        $.each(_veiculo.Equipamentos.list, function (i, Equipamento) {
            if (Equipamento.Codigo.val != data.Codigo) {
                listaAtualizada.push(Equipamento);
            }
        });
        _veiculo.Equipamentos.list = listaAtualizada;
        recarregarGridEquipamentos();
    });
}

//*******MÉTODOS*******

function recarregarGridEquipamentos() {
    var data = new Array();
    $.each(_veiculo.Equipamentos.list, function (i, Equipamento) {
        var obj = new Object();
        obj.Codigo = Equipamento.Codigo.val;
        obj.Descricao = Equipamento.Descricao.val;
        obj.Numero = Equipamento.Numero.val;
        data.push(obj);
    });
    _gridEquipamentos.CarregarGrid(data);
}