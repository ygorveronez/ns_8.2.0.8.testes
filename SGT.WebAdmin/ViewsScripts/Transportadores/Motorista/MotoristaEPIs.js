/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/EPI.js" />
/// <reference path="Motorista.js" />

var _gridMotoristaEPI;
//*******MAPEAMENTO KNOUCKOUT*******

var MotoristaEPIMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoEPI = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoEPI = PropertyEntity({ type: types.map, val: "" });
    this.DataRepasse = PropertyEntity({ type: types.map, val: "" });
    this.SerieEPI = PropertyEntity({ type: types.map, val: "" });
    this.Quantidade = PropertyEntity({ type: types.map, val: "" });
};

//*******EVENTOS*******

function loadMotoristaEPIs() {
    new BuscarEPI(_motorista.EPI);

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarMotoristaEPI, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEPI", visible: false },
        { data: "DescricaoEPI", title: Localization.Resources.Transportadores.Motorista.EPI, width: "45%", className: "text-align-left" },
        { data: "DataRepasse", title: Localization.Resources.Transportadores.Motorista.DataRepasse, width: "15%", className: "text-align-center" },
        { data: "SerieEPI", title: Localization.Resources.Transportadores.Motorista.SerieEPI, width: "15%", className: "text-align-left" },
        { data: "Quantidade", title: Localization.Resources.Transportadores.Motorista.QuantidadeEPI, width: "15%", className: "text-align-right" }
    ];

    _gridMotoristaEPI = new BasicDataTable(_motorista.GridEPIs.idGrid, header, menuOpcoes);
    recarregarGridEPIs();
}

function adicionarMotoristaEPIClick() {
    var tudoCerto = validarCamposObrigatoriosMotoristaEPI();

    if (tudoCerto) {
        var motoristaEPI = new MotoristaEPIMap();
        motoristaEPI.Codigo.val = guid();
        motoristaEPI.CodigoEPI.val = _motorista.EPI.codEntity();
        motoristaEPI.DescricaoEPI.val = _motorista.EPI.val();
        motoristaEPI.DataRepasse.val = _motorista.DataRepasse.val();
        motoristaEPI.SerieEPI.val = _motorista.SerieEPI.val();
        motoristaEPI.Quantidade.val = _motorista.Quantidade.val();

        _motorista.GridEPIs.list.push(motoristaEPI);
        recarregarGridEPIs();
        $("#" + _motorista.Descricao.id).focus();
        LimparCamposMotoristaEPI();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarMotoristaEPIClick() {
    var tudoCerto = validarCamposObrigatoriosMotoristaEPI();

    if (tudoCerto) {
        $.each(_motorista.GridEPIs.list, function (i, motoristaEPI) {
            if (motoristaEPI.Codigo.val == _motorista.CodigoEPI.val()) {

                motoristaEPI.CodigoEPI.val = _motorista.EPI.codEntity();
                motoristaEPI.DescricaoEPI.val = _motorista.EPI.val();
                motoristaEPI.DataRepasse.val = _motorista.DataRepasse.val();
                motoristaEPI.SerieEPI.val = _motorista.SerieEPI.val();
                motoristaEPI.Quantidade.val = _motorista.Quantidade.val();

                return false;
            }
        });
        recarregarGridEPIs();
        LimparCamposMotoristaEPI();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirMotoristaEPIClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaExcluirEPISelecionado, function () {
        var listaAtualizada = new Array();
        $.each(_motorista.GridEPIs.list, function (i, motoristaEPI) {
            if (motoristaEPI.Codigo.val != _motorista.CodigoEPI.val()) {
                listaAtualizada.push(motoristaEPI);
            }
        });
        _motorista.GridEPIs.list = listaAtualizada;
        recarregarGridEPIs();
        LimparCamposMotoristaEPI();
    });
}

//*******MÉTODOS*******

function recarregarGridEPIs() {
    var data = new Array();
    $.each(_motorista.GridEPIs.list, function (i, epi) {
        var motoristaEPI = new Object();

        motoristaEPI.Codigo = epi.Codigo.val;
        motoristaEPI.CodigoEPI = epi.CodigoEPI.val;
        motoristaEPI.DescricaoEPI = epi.DescricaoEPI.val;
        motoristaEPI.DataRepasse = epi.DataRepasse.val;
        motoristaEPI.SerieEPI = epi.SerieEPI.val;
        motoristaEPI.Quantidade = epi.Quantidade.val;

        data.push(motoristaEPI);
    });
    _gridMotoristaEPI.CarregarGrid(data);
}

function editarMotoristaEPI(data) {
    LimparCamposMotoristaEPI();
    $.each(_motorista.GridEPIs.list, function (i, motoristaEPI) {
        if (motoristaEPI.Codigo.val == data.Codigo) {
            _motorista.CodigoEPI.val(motoristaEPI.Codigo.val);
            _motorista.EPI.codEntity(motoristaEPI.CodigoEPI.val);
            _motorista.EPI.val(motoristaEPI.DescricaoEPI.val);
            _motorista.DataRepasse.val(motoristaEPI.DataRepasse.val);
            _motorista.SerieEPI.val(motoristaEPI.SerieEPI.val);
            _motorista.Quantidade.val(motoristaEPI.Quantidade.val);
            
            return false;
        }
    });

    _motorista.AdicionarEPI.visible(false);
    _motorista.AtualizarEPI.visible(true);
    _motorista.ExcluirEPI.visible(true);
    _motorista.CancelarEPI.visible(true);
}

function LimparCamposMotoristaEPI() {
    _motorista.CodigoEPI.val(0);
    LimparCampoEntity(_motorista.EPI);
    _motorista.EPI.requiredClass("form-control");
    _motorista.DataRepasse.val("");
    _motorista.SerieEPI.val("");
    LimparCampo(_motorista.Quantidade);

    _motorista.AdicionarEPI.visible(true);
    _motorista.AtualizarEPI.visible(false);
    _motorista.ExcluirEPI.visible(false);
    _motorista.CancelarEPI.visible(false);
}

function validarCamposObrigatoriosMotoristaEPI() {
    var valido = true;
    if (_motorista.EPI.codEntity() == 0 || _motorista.EPI.val() == "") {
        _motorista.EPI.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _motorista.EPI.requiredClass("form-control");
    }

    if (_motorista.Quantidade.val() == 0) {
        _motorista.Quantidade.requiredClass("form-control is-invalid");
        valido = false;
    } else {
        _motorista.Quantidade.requiredClass("form-control");
    }

    return valido;
}