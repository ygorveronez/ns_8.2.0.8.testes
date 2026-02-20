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
/// <reference path="../../Consultas/ServicoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoVeiculo.js" />
/// <reference path="GrupoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridServicoVeiculo;
var _servicoVeiculo;

var ServicoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.ServicoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoServicoVeiculo.Ambos), def: EnumTipoServicoVeiculo.Ambos, options: EnumTipoServicoVeiculo.obterOpcoes(), text: "*Tipo:", enable: ko.observable(true) });
    this.ValidadeDias = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 6, text: "Validade em Dias:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ToleranciaDias = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: "Tolerância em Dias:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ValidadeKM = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 7, text: "Validade em KM:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ToleranciaKM = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 6, text: "Tolerância em KM:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ValidadeHorimetro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 15, text: "Validade em Horímetro:", required: false, visible: ko.observable(false), configInt: { precision: 0, allowZero: true } });
    this.ToleranciaHorimetro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 15, text: "Tolerância em Horímetro:", required: false, visible: ko.observable(false), configInt: { precision: 0, allowZero: true } });

    this.Tipo.val.subscribe(function (novoValor) {
        TipoServicoVeiculoGrupoServicoChange(novoValor);
    });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: AdicionarServicoVeiculoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarServicoVeiculoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirServicoVeiculoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarServicoVeiculoClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadServicoVeiculo() {
    _servicoVeiculo = new ServicoVeiculo();
    KoBindings(_servicoVeiculo, "knockoutGrupoServicoServicoVeiculo");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarServicoVeiculoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "ServicoVeiculo", title: "Serviço", width: "80%" }
    ];

    _gridServicoVeiculo = new BasicDataTable(_servicoVeiculo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarServicoVeiculo(_servicoVeiculo.ServicoVeiculo);

    RecarregarGridServicoVeiculo();
}

function RecarregarGridServicoVeiculo() {
    var data = new Array();

    $.each(_grupoServico.ServicosVeiculo.list, function (i, servicoVeiculo) {
        var servicoVeiculoGrid = new Object();

        servicoVeiculoGrid.Codigo = servicoVeiculo.Codigo.val;
        servicoVeiculoGrid.ServicoVeiculo = servicoVeiculo.ServicoVeiculo.val;

        data.push(servicoVeiculoGrid);
    });

    _gridServicoVeiculo.CarregarGrid(data);
}

function ExcluirServicoVeiculoClick() {
    for (var i = 0; i < _grupoServico.ServicosVeiculo.list.length; i++) {
        if (_servicoVeiculo.Codigo.val() == _grupoServico.ServicosVeiculo.list[i].Codigo.val) {
            _grupoServico.ServicosVeiculo.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridServicoVeiculo();
    CancelarServicoVeiculoClick();
}

function EditarServicoVeiculoClick(data) {
    for (var i = 0; i < _grupoServico.ServicosVeiculo.list.length; i++) {
        if (data.Codigo == _grupoServico.ServicosVeiculo.list[i].Codigo.val) {
            var servico = _grupoServico.ServicosVeiculo.list[i];

            _servicoVeiculo.Codigo.val(servico.Codigo.val);
            _servicoVeiculo.ServicoVeiculo.val(servico.ServicoVeiculo.val);
            _servicoVeiculo.ServicoVeiculo.codEntity(servico.ServicoVeiculo.codEntity);
            _servicoVeiculo.Tipo.val(servico.Tipo.val);
            _servicoVeiculo.ValidadeDias.val(servico.ValidadeDias.val);
            _servicoVeiculo.ToleranciaDias.val(servico.ToleranciaDias.val);
            _servicoVeiculo.ValidadeKM.val(servico.ValidadeKM.val);
            _servicoVeiculo.ToleranciaKM.val(servico.ToleranciaKM.val);
            _servicoVeiculo.ValidadeHorimetro.val(servico.ValidadeHorimetro.val);
            _servicoVeiculo.ToleranciaHorimetro.val(servico.ToleranciaHorimetro.val);

            _servicoVeiculo.ServicoVeiculo.enable(false);
            _servicoVeiculo.Adicionar.visible(false);
            _servicoVeiculo.Atualizar.visible(true);
            _servicoVeiculo.Excluir.visible(true);
            _servicoVeiculo.Cancelar.visible(true);
        }
    }
}

function AdicionarServicoVeiculoClick() {
    var valido = ValidarCamposObrigatorios(_servicoVeiculo);

    if (valido) {
        for (var i = 0; i < _grupoServico.ServicosVeiculo.list.length; i++) {
            if (_servicoVeiculo.ServicoVeiculo.codEntity() == _grupoServico.ServicosVeiculo.list[i].ServicoVeiculo.codEntity) {
                exibirMensagem(tipoMensagem.atencao, "Atenção", "Serviço já está adicionado!");
                return;
            }
        }

        _servicoVeiculo.Codigo.val(guid());
        _grupoServico.ServicosVeiculo.list.push(SalvarListEntity(_servicoVeiculo));

        RecarregarGridServicoVeiculo();
        CancelarServicoVeiculoClick();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarServicoVeiculoClick() {
    var valido = ValidarCamposObrigatorios(_servicoVeiculo);

    if (valido) {
        for (var i = 0; i < _grupoServico.ServicosVeiculo.list.length; i++) {
            if (_servicoVeiculo.Codigo.val() == _grupoServico.ServicosVeiculo.list[i].Codigo.val) {
                _grupoServico.ServicosVeiculo.list[i] = SalvarListEntity(_servicoVeiculo);
                break;
            }
        }

        RecarregarGridServicoVeiculo();
        CancelarServicoVeiculoClick();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function CancelarServicoVeiculoClick() {
    LimparCampos(_servicoVeiculo);
    _servicoVeiculo.ServicoVeiculo.enable(true);
    _servicoVeiculo.Adicionar.visible(true);
    _servicoVeiculo.Atualizar.visible(false);
    _servicoVeiculo.Excluir.visible(false);
    _servicoVeiculo.Cancelar.visible(false);
}

function TipoServicoVeiculoGrupoServicoChange(novoValor) {
    var possuiKM = false;
    var possuiDias = false;
    var possuiHorimetro = false;

    _servicoVeiculo.ValidadeHorimetro.required = false;
    _servicoVeiculo.ToleranciaHorimetro.required = false;

    _servicoVeiculo.ToleranciaDias.required = false;
    _servicoVeiculo.ValidadeDias.required = false;

    _servicoVeiculo.ValidadeKM.required = false;
    _servicoVeiculo.ToleranciaKM.required = false;

    switch (novoValor) {
        case EnumTipoServicoVeiculo.Ambos:
            possuiKM = true;
            possuiDias = true;
            possuiHorimetro = false;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            _servicoVeiculo.ValidadeKM.required = true;
            _servicoVeiculo.ToleranciaKM.required = true;
            break;
        case EnumTipoServicoVeiculo.PorDia:
            possuiKM = false;
            possuiDias = true;
            possuiHorimetro = false;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            break;
        case EnumTipoServicoVeiculo.PorKM:
            possuiDias = false;
            possuiKM = true;
            possuiHorimetro = false;
            _servicoVeiculo.ValidadeKM.required = true;
            _servicoVeiculo.ToleranciaKM.required = true;
            break;
        case EnumTipoServicoVeiculo.PorHorimetro:
            possuiDias = false;
            possuiKM = false;
            possuiHorimetro = true;
            _servicoVeiculo.ValidadeHorimetro.required = true;
            _servicoVeiculo.ToleranciaHorimetro.required = true;
            break;
        case EnumTipoServicoVeiculo.Todos:
            possuiDias = true;
            possuiKM = true;
            possuiHorimetro = true;
            _servicoVeiculo.ValidadeHorimetro.required = true;
            _servicoVeiculo.ToleranciaHorimetro.required = true;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            _servicoVeiculo.ValidadeKM.required = true;
            _servicoVeiculo.ToleranciaKM.required = true;
            break;
        case EnumTipoServicoVeiculo.PorHorimetroDia:
            possuiDias = true;
            possuiKM = false;
            possuiHorimetro = true;
            _servicoVeiculo.ValidadeHorimetro.required = true;
            _servicoVeiculo.ToleranciaHorimetro.required = true;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            break;
        default:
            break;
    }

    if (possuiDias) {
        _servicoVeiculo.ValidadeDias.visible(true);
        _servicoVeiculo.ToleranciaDias.visible(true);
    } else {
        _servicoVeiculo.ValidadeDias.val(0);
        _servicoVeiculo.ToleranciaDias.val(0);
        _servicoVeiculo.ValidadeDias.visible(false);
        _servicoVeiculo.ToleranciaDias.visible(false);
    }

    if (possuiKM) {
        _servicoVeiculo.ValidadeKM.visible(true);
        _servicoVeiculo.ToleranciaKM.visible(true);
    } else {
        _servicoVeiculo.ValidadeKM.val(0);
        _servicoVeiculo.ToleranciaKM.val(0);
        _servicoVeiculo.ValidadeKM.visible(false);
        _servicoVeiculo.ToleranciaKM.visible(false);
    }

    if (possuiHorimetro) {
        _servicoVeiculo.ValidadeHorimetro.visible(true);
        _servicoVeiculo.ToleranciaHorimetro.visible(true);
    } else {
        _servicoVeiculo.ValidadeHorimetro.val(0);
        _servicoVeiculo.ToleranciaHorimetro.val(0);
        _servicoVeiculo.ValidadeHorimetro.visible(false);
        _servicoVeiculo.ToleranciaHorimetro.visible(false);
    }
}

function LimparCamposServicoVeiculo() {
    LimparCampos(_servicoVeiculo);
    _grupoServico.ServicosVeiculo.list = new Array();
    RecarregarGridServicoVeiculo();
}