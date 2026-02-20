/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="Pessoa.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumDiaSemana.js" />

var _frequenciaCarregamento, _gridFrequenciaCarregamento;

var FrequenciaCarregamentoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.map, val: "" });
    this.DiaSemana = PropertyEntity({ type: types.map, val: "" });
    this.TransportadorCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var FrequenciaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Transportador.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
    this.DiaSemana = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Pessoas.Pessoa.DiasDaSemana.getRequiredFieldDescription(), val: ko.observable(new Array()), def: new Array(), options: EnumDiaSemana.obterOpcoes(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarFrequenciaCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarFrequenciaCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirFrequenciaCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposFrequenciaCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

function loadFrequenciaCarregamento() {
    _frequenciaCarregamento = new FrequenciaCarregamento();
    KoBindings(_frequenciaCarregamento, "knockoutFrequenciaCarregamento");

    new BuscarTransportadores(_frequenciaCarregamento.Transportador);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarFrequenciaCarregamento }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Transportador", title: Localization.Resources.Pessoas.Pessoa.Transportador, width: "50%" },
        { data: "DiaSemana", title: Localization.Resources.Pessoas.Pessoa.DiasDaSemana, width: "40%" }
    ];

    _gridFrequenciaCarregamento = new BasicDataTable(_frequenciaCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridFrequenciaCarregamento();
}

function recarregarGridFrequenciaCarregamento() {
    var data = new Array();

    $.each(_pessoa.FrequenciasCarregamento.list, function (i, frequenciaCarregamento) {
        var gridFrequenciaCarregamento = new Object();

        gridFrequenciaCarregamento.Codigo = frequenciaCarregamento.Codigo.val;
        gridFrequenciaCarregamento.Transportador = frequenciaCarregamento.Transportador.val;
        var diasSemana = JSON.parse(frequenciaCarregamento.DiaSemana.val);
        var aux = new Array();
        for (var i = 0; i < diasSemana.length; i++) {
            aux[i] = " " + EnumDiaSemana.obterDescricaoSemConfiguracao(diasSemana[i]);
        }
        gridFrequenciaCarregamento.DiaSemana = aux;

        data.push(gridFrequenciaCarregamento);
    });

    _gridFrequenciaCarregamento.CarregarGrid(data);
}

function adicionarFrequenciaCarregamento(e, sender) {
    if (!ValidarCamposObrigatorios(_frequenciaCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (_frequenciaCarregamento.DiaSemana.val().length === 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorInformarAoMenosUmDiaDaSemana);
        return;
    }

    var existe = false;
    $.each(_pessoa.FrequenciasCarregamento.list, function (i, frequenciasCarregamento) {
        if (frequenciasCarregamento.Transportador.codEntity == _frequenciaCarregamento.Transportador.codEntity()) {
            existe = true;
            return;
        }
    });

    if (existe) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pessoas.Pessoa.TransportadorJaEstaInformado.format(_frequenciaCarregamento.Transportador.val()));
        return;
    }

    _frequenciaCarregamento.Codigo.val(guid());
    _frequenciaCarregamento.DiaSemana.val(JSON.stringify(_frequenciaCarregamento.DiaSemana.val()));
    _pessoa.FrequenciasCarregamento.list.push(SalvarListEntity(_frequenciaCarregamento));

    limparCamposFrequenciaCarregamento();
}

function editarFrequenciaCarregamento(data) {
    for (var i = 0; i < _pessoa.FrequenciasCarregamento.list.length; i++) {
        if (data.Codigo == _pessoa.FrequenciasCarregamento.list[i].Codigo.val) {

            var frequenciaCarregamento = _pessoa.FrequenciasCarregamento.list[i];

            _frequenciaCarregamento.Codigo.val(frequenciaCarregamento.Codigo.val);
            _frequenciaCarregamento.Transportador.val(frequenciaCarregamento.Transportador.val);
            _frequenciaCarregamento.Transportador.codEntity(frequenciaCarregamento.Transportador.codEntity);
            //_frequenciaCarregamento.DiaSemana.val(JSON.parse(frequenciaCarregamento.DiaSemana.val));

            $("#" + _frequenciaCarregamento.DiaSemana.id).selectpicker('val', frequenciaCarregamento.DiaSemana.val);
            
            _frequenciaCarregamento.Adicionar.visible(false);
            _frequenciaCarregamento.Atualizar.visible(true);
            _frequenciaCarregamento.Excluir.visible(true);
            _frequenciaCarregamento.Cancelar.visible(true);

            break;
        }
    }
}

function atualizarFrequenciaCarregamento() {
    if (!ValidarCamposObrigatorios(_frequenciaCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (_frequenciaCarregamento.DiaSemana.val().length === 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorInformarAoMenosUmDiaDaSemana);
        return;
    }

    var existe = false;
    $.each(_pessoa.FrequenciasCarregamento.list, function (i, frequenciasCarregamento) {
        if (frequenciasCarregamento.Transportador.codEntity == _frequenciaCarregamento.Transportador.codEntity() && _frequenciaCarregamento.Codigo.val() != frequenciasCarregamento.Codigo.val) {
            existe = true;
            return;
        }
    });

    if (existe) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Pessoas.Pessoa.TransportadorJaEstaInformado.format(_frequenciaCarregamento.Transportador.val()));
        return;
    }

    for (var i = 0; i < _pessoa.FrequenciasCarregamento.list.length; i++) {
        if (_frequenciaCarregamento.Codigo.val() == _pessoa.FrequenciasCarregamento.list[i].Codigo.val) {
            _frequenciaCarregamento.DiaSemana.val(JSON.stringify(_frequenciaCarregamento.DiaSemana.val()))
            _pessoa.FrequenciasCarregamento.list[i] = SalvarListEntity(_frequenciaCarregamento);
            break;
        }
    }

    limparCamposFrequenciaCarregamento();
}

function excluirFrequenciaCarregamento(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.DesejaRealmenteExcluirFrequenciaDeCarregamento, function () {
        for (var i = 0; i < _pessoa.FrequenciasCarregamento.list.length; i++) {
            if (_frequenciaCarregamento.Codigo.val() == _pessoa.FrequenciasCarregamento.list[i].Codigo.val) {
                _pessoa.FrequenciasCarregamento.list.splice(i, 1);
                break;
            }
        }

        limparCamposFrequenciaCarregamento();
    });
}

function limparCamposFrequenciaCarregamento() {
    LimparCampos(_frequenciaCarregamento);
    _frequenciaCarregamento.Adicionar.visible(true);
    _frequenciaCarregamento.Atualizar.visible(false);
    _frequenciaCarregamento.Excluir.visible(false);
    _frequenciaCarregamento.Cancelar.visible(false);

    recarregarGridFrequenciaCarregamento();
}