/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _confirmarChegada;

var ConfirmarChegada = function () {
    this.CodigoJanelaDescarregamento = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.Veiculo = PropertyEntity({ text: "*Veículo:", issue: 143, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Reboque = PropertyEntity({ text: "Reboque:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SegundoReboque = PropertyEntity({ text: "Segundo Reboque:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "*Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.RG = PropertyEntity({ text: "RG:", val: ko.observable(""), def: "" });
    this.Telefone = PropertyEntity({ text: "Telefone:", val: ko.observable(""), def: "" });
    this.SenhaAgendamento = PropertyEntity({ val: ko.observable(""), def: "", visible: false });

    this.ImprimirComprovanteCargaInformada = PropertyEntity({ eventClick: imprimirComprovanteCargaInformadaDocaClick, type: types.event, text: "Imprimir Comprovante de Carga", visible: ko.observable(false) });
    this.Salvar = PropertyEntity({
        eventClick: confirmarChegadaClick, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true)
    });
};

function loadConfirmarChegada() {
    _confirmarChegada = new ConfirmarChegada();
    KoBindings(_confirmarChegada, "knockoutConfirmarChegada");

    BuscarMotoristas(_confirmarChegada.Motorista);
    BuscarVeiculos(_confirmarChegada.Veiculo);
    BuscarVeiculos(_confirmarChegada.Reboque, null, null, null, null, true, null, null, true, null, null, "1");

}

function exibirModalConfirmarChegada(registro) {
    _confirmarChegada.CodigoJanelaDescarregamento.val(registro.Codigo);
    _confirmarChegada.CodigoCarga.val(registro.CodigoCarga);
    _confirmarChegada.SenhaAgendamento.val(registro.SenhaAgendamento);

    executarReST("JanelaDescarga/ObterDadosTransporte", { CodigoCarga: _confirmarChegada.CodigoCarga.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                let dados = retorno.Data;
                _confirmarChegada.Veiculo.codEntity(dados.VeiculoCodigo);
                _confirmarChegada.Veiculo.val(dados.VeiculoDescricao);
                _confirmarChegada.Reboque.codEntity(dados.PrimeiroReboqueCodigo);
                _confirmarChegada.SegundoReboque.codEntity(dados.SegundoReboqueCodigo);
                _confirmarChegada.Reboque.val(dados.PrimeiroReboqueDescricao);
                _confirmarChegada.SegundoReboque.val(dados.SegundoReboqueDescricao);
                _confirmarChegada.Motorista.codEntity(dados.MotoristaCodigo);
                _confirmarChegada.Motorista.val(dados.MotoristaDescricao);
                _confirmarChegada.RG.val(dados.MotoristaRG);
                _confirmarChegada.Telefone.val(dados.MotoristaTelefone);
            }
        }
    });

    $("#divModalConfirmarChegada")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_confirmarChegada);
        });
}

function confirmarChegadaClick() {
    if (!ValidarCamposObrigatorios(_confirmarChegada)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("JanelaDescarga/ConfirmarChegada", RetornarObjetoPesquisa(_confirmarChegada), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Chegada confirmada!");
                Global.fecharModal('divModalConfirmarChegada');
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function imprimirComprovanteCargaInformadaDocaClick(e) {
    if ("imprimirComprovanteCargaInformada" in window)
        imprimirComprovanteCargaInformada(e);
}

function imprimirComprovanteCargaInformada(e) {
    const data = {
        Codigo: e.CodigoCarga.val(),
        Descarga: true,
        SenhaAgendamento: e.SenhaAgendamento.val(),
        CodigoJanelaDescarregamento: e.CodigoJanelaDescarregamento.val()
    }
    executarDownload("JanelaDescarga/ComprovanteCargaInformada", data);
}

function buscarMotoristaPorCpf() {
    if (!validarCpfMotorista()) {
        _confirmarChegada.CodigoMotoristaChegada.val(0);
        $("#" + _confirmarChegada.Motorista.id).focus();
        return;
    }

    executarReST("Motorista/BuscarPorCPF", { Cpf: _confirmarChegada.CpfMotoristaChegada.val(), BuscarPorEmpresaPai: true }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _confirmarChegada.CodigoMotoristaChegada.val(retorno.Data.Codigo);
                _confirmarChegada.Motorista.val(retorno.Data.Nome);
                _confirmarChegada.RG.val(retorno.Data.RG);
                _confirmarChegada.Telefone.val(retorno.Data.Telefone);

                $("#" + _guaritaFluxoPatio.MotoristaChegada.id).focus();
            }
            else {
                _guaritaFluxoPatio.CodigoMotoristaChegada.val(0);
                $("#" + _guaritaFluxoPatio.MotoristaChegada.id).focus();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            $("#" + _guaritaFluxoPatio.CpfMotoristaChegada.id).focus();
        }
    });
}

function validarCpfMotorista() {
    const cpfMotorista = _confirmarChegada.CpfMotoristaChegada.val().replace(/[^0-9]/g, "");

    if (!cpfMotorista)
        return false;

    return ValidarCPF(cpfMotorista);
}
