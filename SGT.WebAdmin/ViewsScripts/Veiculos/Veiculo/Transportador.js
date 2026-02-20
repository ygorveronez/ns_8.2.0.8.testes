/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="Veiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransportadoras;
var _transportador;

var TransportadorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Nome = PropertyEntity({ type: types.map, val: "" });
    this.CNPJ = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadTransportador() {
    new BuscarTransportadores(_veiculo.Transportadoras, retornoTransportador);
    var editar = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirTransportadorClick, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CNPJ", title: Localization.Resources.Veiculos.Veiculo.CNPJ, width: "20%", className: "text-align-left" },
        { data: "Nome", title: Localization.Resources.Veiculos.Veiculo.RazaoSocial, width: "70%", className: "text-align-left" }
    ];
    _gridTransportadoras = new BasicDataTable(_veiculo.Transportadoras.idGrid, header, menuOpcoes);
    recarregarGridTransportadoras();
}

function retornoTransportador(data) {
    _transportador = new TransportadorMap();
    _transportador.Codigo.val = data.Codigo;
    _transportador.CNPJ.val = data.CNPJ;
    _transportador.Nome.val = data.Descricao;

    _veiculo.Transportadoras.codEntity(data.Codigo);
    _veiculo.Transportadoras.val(data.Descricao);
}

function adicionarTransportadorClick() {
    _veiculo.Transportadoras.requiredClass("form-control");
    var tudoCerto = ValidarCampoObrigatorioEntity(_veiculo.Transportadoras);
    tudoCerto = _veiculo.Transportadoras.codEntity() > 0;
    if (tudoCerto) {
        var existe = false;
        $.each(_veiculo.Transportadoras.list, function (i, Transportador) {
            if (Transportador.Codigo.val == _veiculo.Transportadoras.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _veiculo.Transportadoras.list.push(_transportador);
            recarregarGridTransportadoras();
            $("#" + _veiculo.Transportadoras.id).focus();
            _veiculo.Transportadoras.requiredClass("form-control");
        } else {
            exibirMensagem("aviso", Localization.Resources.Veiculos.Veiculo.TransportadorJaInformado, Localization.Resources.Veiculos.Veiculo.TransportadorJaInformadoParaEsteVeiculo.format(_veiculo.Transportadoras.val()));
        }
        LimparCampoEntity(_veiculo.Transportadoras);
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        _veiculo.Transportadoras.requiredClass("form-control is-invalid");
    }
}

function excluirTransportadorClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resouces.Veiculos.Veiculo.RealmenteDesejaExcluirTransportador.format(data.Nome), function () {
        var listaAtualizada = new Array();
        $.each(_veiculo.Transportadoras.list, function (i, Transportador) {
            if (Transportador.Codigo.val != data.Codigo) {
                listaAtualizada.push(Transportador);
            }
        });
        _veiculo.Transportadoras.list = listaAtualizada;
        recarregarGridTransportadoras();
    });
}

//*******MÉTODOS*******

function recarregarGridTransportadoras() {
    var data = new Array();
    $.each(_veiculo.Transportadoras.list, function (i, Tarnsportador) {
        var obj = new Object();
        obj.Codigo = Tarnsportador.Codigo.val;
        obj.CNPJ = Tarnsportador.CNPJ.val;
        obj.Nome = Tarnsportador.Nome.val;
        data.push(obj);
    });
    _gridTransportadoras.CarregarGrid(data);
}