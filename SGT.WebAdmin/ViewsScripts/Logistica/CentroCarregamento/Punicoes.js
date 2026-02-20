/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoFrota.js" />
/// <reference path="CentroCarregamento.js" />

var _punicoesCarregamento
var _gridPunicoesCarregamento
var selectedTipoFrota = "";

var punicoesCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });
    this.TipoFrota = PropertyEntity({ val: ko.observable(EnumTipoFrota.NaoDefinido), options: EnumTipoFrota.obterOpcoes(), text: Localization.Resources.Logistica.CentroCarregamento.TipoFrota.getRequiredFieldDescription(), enable: ko.observable(true) });
    this.HorasPunicao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.HorasPunicao.getRequiredFieldDescription(), getType: typesKnockout.int, required: true, val: ko.observable(0) });
    this.TipoFrotaDescricao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Adicionar = PropertyEntity({ eventClick: adicionarPunicoesCarregamento, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.PunicaoAdicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposPunicoesCarregamento, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.PunicaoCancelar, visible: ko.observable(false) });
};

function loadPunicoesCarregamento() {

    _punicoesCarregamento = new punicoesCarregamento();
    KoBindings(_punicoesCarregamento, "knockoutPunicoesCarregamento");

    let opcoesGrid = [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirPunicoesCarregamento }]
    let menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 7, opcoes: opcoesGrid };

    onTipoFrotaChange();

    let header = [
        { data: "Codigo", visible: false },
        { data: "TipoFrota", visible: false },
        { data: "TipoFrotaDescricao", title: Localization.Resources.Logistica.CentroCarregamento.TipoFrota, width: "45%" },
        { data: "HorasPunicao", title: Localization.Resources.Logistica.CentroCarregamento.HorasPunicao, width: "40%" },
    ];

    _gridPunicoesCarregamento = new BasicDataTable(_punicoesCarregamento.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc });

    recarregarGridPunicoesCarregamento();
}

function recarregarGridPunicoesCarregamento() {

    let data = _centroCarregamento.PunicoesCarregamento.list.map(function (item) {
        return {
            Codigo: item.Codigo.val,
            TipoFrota: item.TipoFrota.val,
            TipoFrotaDescricao: item.TipoFrotaDescricao.val,
            HorasPunicao: item.HorasPunicao.val
        };
    });

    _gridPunicoesCarregamento.CarregarGrid(data);
}

function adicionarPunicoesCarregamento(e, sender) {

    if (!ValidarCamposObrigatorios(_punicoesCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _punicoesCarregamento.Codigo.val(guid());
    _punicoesCarregamento.TipoFrotaDescricao.val(selectedTipoFrota);

    let horasToNumber = parseInt(_punicoesCarregamento.HorasPunicao.val(), 10).toString();
    _punicoesCarregamento.HorasPunicao.val(horasToNumber);    

    _centroCarregamento.PunicoesCarregamento.list.push(SalvarListEntity(_punicoesCarregamento));

    limparCamposPunicoesCarregamento();
}

function excluirPunicoesCarregamento(data) {

    _centroCarregamento.PunicoesCarregamento.list = _centroCarregamento.PunicoesCarregamento.list.filter(function (item) {
        return item.Codigo.val != data.Codigo;
    });
    recarregarGridPunicoesCarregamento();
}

function limparCamposPunicoesCarregamento() {

    LimparCampos(_punicoesCarregamento);

    // Redefine a seleção de Tipo de Frota
    _punicoesCarregamento.TipoFrota.val(EnumTipoFrota.NaoDefinido);
    onTipoFrotaChange();

    _punicoesCarregamento.Adicionar.visible(true);
    _punicoesCarregamento.Cancelar.visible(false);  

    recarregarGridPunicoesCarregamento();
}

function limparGridPunicoesCarregamento() {
    limparCamposPunicoesCarregamento();
    _centroCarregamento.PunicoesCarregamento.list = [];
}

let onTipoFrotaChange = function () {
    selectedTipoFrota = $("#" + _punicoesCarregamento.TipoFrota.id + " option:selected").text();
};