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
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="MotivoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _motivoChamadoData, _gridMotivoChamadoData;

var MotivoChamadoData = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.Descricao = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Descricao.getRequiredFieldDescription(), required: true, maxlength: 100 });
    this.Obrigatorio = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Nao), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Nao, text: Localization.Resources.Chamado.MotivoChamado.ObrigatorioInterrogacao });
    this.Status = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Status.getFieldDescription(), val: ko.observable(true), options: _status, def: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarMotivoChamadoDataClick, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMotivoChamadoDataClick, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposMotivoChamadoData, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadMotivoChamadoData() {
    _motivoChamadoData = new MotivoChamadoData();
    KoBindings(_motivoChamadoData, "knockoutMotivoChamadoData");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Chamado.MotivoChamado.Editar, id: guid(), metodo: editarMotivoChamadoDataClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Chamado.MotivoChamado.Descricao, width: "60%" },
        { data: "Obrigatorio", visible: false },
        { data: "DescricaoObrigatorio", title: Localization.Resources.Chamado.MotivoChamado.Obrigatorio, width: "20%" },
        { data: "Status", visible: false },
        { data: "DescricaoStatus", title: Localization.Resources.Chamado.MotivoChamado.Status, width: "20%" }
    ];

    _gridMotivoChamadoData = new BasicDataTable(_motivoChamadoData.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridMotivoChamadoData();

    if (_CONFIGURACAO_TMS.UtilizaListaDinamicaDatasChamado)
        $("#liTabMotivoChamadoData").show();
}

//*******MÉTODOS*******

function RecarregarGridMotivoChamadoData() {
    var data = new Array();

    $.each(_motivoChamado.Datas.list, function (i, motivoData) {
        var dataGrid = new Object();

        dataGrid.Codigo = motivoData.Codigo.val;
        dataGrid.Descricao = motivoData.Descricao.val;
        dataGrid.Obrigatorio = motivoData.Obrigatorio.val;
        dataGrid.DescricaoObrigatorio = dataGrid.Obrigatorio ? "Sim" : "Não";
        dataGrid.Status = motivoData.Status.val;
        dataGrid.DescricaoStatus = dataGrid.Status ? "Ativo" : "Inativo";

        data.push(dataGrid);
    });

    _gridMotivoChamadoData.CarregarGrid(data);
}

function editarMotivoChamadoDataClick(data) {
    for (var i = 0; i < _motivoChamado.Datas.list.length; i++) {
        if (data.Codigo == _motivoChamado.Datas.list[i].Codigo.val) {
            var data = _motivoChamado.Datas.list[i];

            _motivoChamadoData.Codigo.val(data.Codigo.val);
            _motivoChamadoData.Descricao.val(data.Descricao.val);
            _motivoChamadoData.Obrigatorio.val(data.Obrigatorio.val);
            _motivoChamadoData.Status.val(data.Status.val);

            _motivoChamadoData.Adicionar.visible(false);
            _motivoChamadoData.Atualizar.visible(true);
            _motivoChamadoData.Cancelar.visible(true);
        }
    }
}

function adicionarMotivoChamadoDataClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_motivoChamadoData);

    if (valido) {
        _motivoChamadoData.Codigo.val(guid());
        _motivoChamado.Datas.list.push(SalvarListEntity(_motivoChamadoData));

        limparCamposMotivoChamadoData();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarMotivoChamadoDataClick() {
    var valido = ValidarCamposObrigatorios(_motivoChamadoData);

    if (valido) {
        for (var i = 0; i < _motivoChamado.Datas.list.length; i++) {
            if (_motivoChamadoData.Codigo.val() == _motivoChamado.Datas.list[i].Codigo.val) {
                _motivoChamado.Datas.list[i] = SalvarListEntity(_motivoChamadoData);
                break;
            }
        }

        limparCamposMotivoChamadoData();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function limparCamposMotivoChamadoData() {
    LimparCampos(_motivoChamadoData);
    _motivoChamadoData.Adicionar.visible(true);
    _motivoChamadoData.Atualizar.visible(false);
    _motivoChamadoData.Cancelar.visible(false);

    RecarregarGridMotivoChamadoData();
}