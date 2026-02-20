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
/// <reference path="GuaritaCheckList.js"/>
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Enumeradores/EnumTipoCheckListGuarita.js" />
/// <reference path="../../Consultas/CheckListTipo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _novoCheckList;

var _tipoEntradaSaida = [
    { text: "Entrada", value: EnumEntradaSaida.Entrada },
    { text: "Saída", value: EnumEntradaSaida.Saida }
];

//var _tipoCheckListGuarita = [
//    { text: "Manutenção", value: EnumTipoCheckListGuarita.Manutencao },
//    { text: "Rastreamento", value: EnumTipoCheckListGuarita.Rastreamento }
//];

var NovoCheckList = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.OrdemServicoFrota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Ordem de Serviço:", idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.KMAtual = PropertyEntity({ text: "*KM Atual: ", required: true, getType: typesKnockout.int, type: types.int, configInt: { precision: 0, allowZero: true }, enable: ko.observable(true) });
    this.TipoEntradaSaida = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Entrada), options: _tipoEntradaSaida, def: EnumEntradaSaida.Entrada, required: true, text: "*Tipo: ", enable: ko.observable(true) });
    //this.TipoCheckListGuarita = PropertyEntity({ text: "*Tipo Check: ", required: true, val: ko.observable(''), options: _tipoCheckListGuarita, def: EnumTipoCheckListGuarita.Manutencao });
    this.CheckListTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Check List: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNovoCheckClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

function loadNovoCheckList() {

    _novoCheckList = new NovoCheckList();
    KoBindings(_novoCheckList, "knockoutNovoCheckList");

    new BuscarCargas(_novoCheckList.Carga, RetornoCargas);
    new BuscarOrdemServico(_novoCheckList.OrdemServicoFrota, RetornoOrdemServico);
    new BuscarVeiculos(_novoCheckList.Veiculo, RetornarVeiculo);
    new BuscarCheckListTipo(_novoCheckList.CheckListTipo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe) {
        _novoCheckList.Carga.visible(false);
        _novoCheckList.OrdemServicoFrota.visible(false);
    }
}

function RetornarVeiculo(data) {
    _novoCheckList.Veiculo.codEntity(data.Codigo);
    _novoCheckList.Veiculo.val(data.Placa);
}

function RetornoOrdemServico(data) {
    _novoCheckList.OrdemServicoFrota.codEntity(data.Codigo);
    _novoCheckList.OrdemServicoFrota.val(data.Numero);
    if (data.CodigoVeiculo > 0) {
        _novoCheckList.Veiculo.codEntity(data.CodigoVeiculo);
        _novoCheckList.Veiculo.val(data.Veiculo);
        _novoCheckList.Veiculo.enable(false);
    } else {
        _novoCheckList.Veiculo.enable(true);
    }
}

function RetornoCargas(data) {
    _novoCheckList.Carga.codEntity(data.Codigo);
    _novoCheckList.Carga.val(data.CodigoCargaEmbarcador);
    if (data.CodigoVeiculo > 0) {
        _novoCheckList.Veiculo.codEntity(data.CodigoVeiculo);
        _novoCheckList.Veiculo.val(data.Veiculo);
        _novoCheckList.Veiculo.enable(false);
    } else {
        _novoCheckList.Veiculo.enable(true);
    }
}

function adicionarNovoCheckClick(e, sender) {
    if (ValidarCargaOrdemServico()) {
        Salvar(e, "GuaritaCheckList/AdicionarCheckList", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado");

                    Global.fecharModal('divModalNovoCheckList');
                    limparCamposNovoCheckList();
                    _girdCheckList.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        }, sender);
    }
}

function ValidarCargaOrdemServico() {
    var tudoCerto = true;

    /*if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiNFe) {
        _novoCheckList.Carga.requiredClass("form-control");
        _novoCheckList.OrdemServicoFrota.requiredClass("form-control");

        _novoCheckList.Carga.required(true);
        _novoCheckList.OrdemServicoFrota.required(true);

        tudoCerto = ValidarCampoObrigatorioEntity(_novoCheckList.Carga);
        if (!tudoCerto)
            tudoCerto = ValidarCampoObrigatorioEntity(_novoCheckList.OrdemServicoFrota);

        _novoCheckList.Carga.required(false);
        _novoCheckList.OrdemServicoFrota.required(false);

        if (!tudoCerto) {
            exibirMensagem("atencao", "Campos Obrigatórios", "Informe ao menos uma Carga ou uma Ordem de Serviço.");
            _novoCheckList.Carga.requiredClass("form-control is-invalid");
            _novoCheckList.OrdemServicoFrota.requiredClass("form-control is-invalid");
        }
    }*/

    return tudoCerto;
}

function limparCamposNovoCheckList() {
    LimparCampos(_novoCheckList);
}