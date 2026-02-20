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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="Veiculo.js" />

var _gridVeiculosVinculados;
var _veiculoVinculado;

//*******MAPEAMENTO KNOUCKOUT*******

var VeiculoVinculadoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Placa = PropertyEntity({ type: types.map, val: "" });
    this.Tara = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CapacidadeM3 = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CapacidadeQuilo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.TipoRodado = PropertyEntity({ type: types.map, val: "" });
    this.Tipo = PropertyEntity({ type: types.map, val: "" });
    this.TipoCarroceria = PropertyEntity({ type: types.map, val: "" });
    this.Estado = PropertyEntity({ type: types.map, val: "" });
    this.TipoProprietario = PropertyEntity({ type: types.map, val: "" });
    this.RNTRC = PropertyEntity({ type: types.map, val: "" });
    this.Renavam = PropertyEntity({ type: types.map, val: "" });
    this.ObservacaoCTe = PropertyEntity({ type: types.map, val: "" });
    this.Proprietario = PropertyEntity({ type: types.entity, val: "", codEntity: 0 });
    this.MarcaVeiculo = PropertyEntity({ type: types.entity, val: "", codEntity: 0 });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, val: "", codEntity: 0 });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, val: "", codEntity: 0 });
};

//*******EVENTOS*******

function loadVeiculoVinculado() {

    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarVeiculoVinculado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Placa", title: Localization.Resources.Veiculos.Veiculo.Placa, width: "20%" },
        { data: "ModeloVeicularCarga", title: Localization.Resources.Veiculos.Veiculo.ModeloCarga, width: "50%", className: "text-align-left" },
        { data: "CapacidadeQuilo", title: Localization.Resources.Veiculos.Veiculo.Capacidade, width: "15%", className: "text-align-rigth" }
    ];
    _gridVeiculosVinculados = new BasicDataTable(_veiculo.VeiculosVinculados.idGrid, header, menuOpcoes);
    recarregarGridVeiculosViculados();

    _veiculoVinculado = new Veiculo();
    _veiculoVinculado.Adicionar.eventClick = adicionarVeiculoVinculadoClick;
    _veiculoVinculado.Atualizar.eventClick = atualizarVeiculoVinculadoClick;
    _veiculoVinculado.Excluir.eventClick = excluirVeiculoVinculadoClick;
    _veiculoVinculado.Cancelar.eventClick = LimparCamposVeiculosVinculados;
    _veiculoVinculado.TagPlaca.eventClick = function (e) { InserirTag(_veiculoVinculado.ObservacaoCTe.id, "#PlacaVeiculo#"); };
    _veiculoVinculado.TagRanavam.eventClick = function (e) { InserirTag(_veiculoVinculado.ObservacaoCTe.id, "#RENAVAMVeiculo#"); };
    _veiculoVinculado.TagNomeProprietario.eventClick = function (e) { InserirTag(_veiculoVinculado.ObservacaoCTe.id, "#NomeProprietarioVeiculo#"); };
    _veiculoVinculado.TagCPFCNPJProprietario.eventClick = function (e) { InserirTag(_veiculoVinculado.ObservacaoCTe.id, "#CPFCNPJProprietarioVeiculo#"); };
    _veiculoVinculado.TagRNTRC.eventClick = function (e) { InserirTag(_veiculoVinculado.ObservacaoCTe.id, "#RNTRCProprietario#"); };
    _veiculoVinculado.Tipo.eventChange = veiculoVinculadoMudouPropriedadeOnChange;
    _veiculoVinculado.TipoVeiculo.required = false;
    _veiculoVinculado.Empresa.required = false;
    _veiculoVinculado.MarcaVeiculo.required = false;
    _veiculoVinculado.KilometragemAtual.required = false;
    _veiculoVinculado.ModeloVeicularCarga.required = false;
    _veiculoVinculado.ModeloVeiculo.required = false;
    _veiculoVinculado.CapacidadeQuilo.required = false;
    KoBindings(_veiculoVinculado, "knoutVeiculosVinculados");

    $("#" + _veiculoVinculado.RNTRC.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _veiculoVinculado.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _veiculoVinculado.Renavam.id).mask("00000000000", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _veiculoVinculado.Renavam.id).on("blur", function () {
        var val = $(this).val();
        if (val.length != 11 || !/^\d+$/.test(val)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Veiculos.Veiculo.RenavamInvalido, Localization.Resources.Veiculos.Veiculo.RenavamDevePossuirOnzeDigitos);
        }
    });

    new BuscarModelosVeicularesCarga(_veiculoVinculado.ModeloVeicularCarga);
    new BuscarClientes(_veiculoVinculado.Proprietario);
}

function validarPlacaClick(e) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        var data = { Placa: _veiculoVinculado.Placa.val(), Empresa: 0 };
    else
        var data = { Placa: _veiculoVinculado.Placa.val(), Empresa: _veiculo.Empresa.codEntity() };

    executarReST("Veiculo/BuscarPorPlaca", data, function (e) {
        if (e.Success) {
            if (e.Data != null) {
                if (e.Data.TipoVeiculo == "0")
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.VeiculoTipoTracao, Localization.Resources.Veiculos.Veiculo.PorFavorInformeVeiculoTipoReboque, 10000);
                else {
                    PreencherObjetoKnout(_veiculoVinculado, e);
                    veiculoVinculadoMudouPropriedadeOnChange();

                    if (_veiculoVinculado.ModeloVeicularCarga.codEntity() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
                        _veiculoVinculado.ModeloVeicularCarga.enable(false);
                    else
                        _veiculoVinculado.ModeloVeicularCarga.enable(true);
                }
            }
            else if (_veiculoVinculado.Placa.val() != "")
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.VeiculoNaoExiste, Localization.Resources.Veiculos.Veiculo.VeiculoNaoEstaCadastradoAoFinalizarConfiguracoesOVeiculoSeraIncluidoAutomaticamenteNaBaseDeVeiculos.format(_veiculoVinculado.Placa.val()), 10000);
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Veiculos.Veiculo.VeiculoNaoExiste, Localization.Resources.Veiculos.Veiculo.PorFavorDigiteUmaPlacaAntesDeValidarVeiculo, 10000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
    });
}

function adicionarVeiculoVinculadoClick(veiculoVinculadoMap, gridModelosVeiculares) {
    var tudoCerto = ValidarCamposObrigatorios(_veiculoVinculado);
    if (tudoCerto) {
        var existe = false;
        $.each(_veiculo.VeiculosVinculados.list, function (i, veiculoVinculado) {
            if (veiculoVinculado.Placa.val == _veiculoVinculado.Placa.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            var veiculoVinculado = new VeiculoVinculadoMap();
            veiculoVinculado.Placa.val = _veiculoVinculado.Placa.val();
            veiculoVinculado.Tara.val = _veiculoVinculado.Tara.val();
            veiculoVinculado.CapacidadeM3.val = _veiculoVinculado.CapacidadeM3.val();
            veiculoVinculado.CapacidadeQuilo.val = _veiculoVinculado.CapacidadeQuilo.val();
            veiculoVinculado.TipoRodado.val = _veiculoVinculado.TipoRodado.val();
            veiculoVinculado.Tipo.val = _veiculoVinculado.Tipo.val();
            veiculoVinculado.TipoCarroceria.val = _veiculoVinculado.TipoCarroceria.val();
            veiculoVinculado.Estado.val = _veiculoVinculado.Estado.val();
            veiculoVinculado.TipoProprietario.val = _veiculoVinculado.TipoProprietario.val();
            veiculoVinculado.RNTRC.val = _veiculoVinculado.RNTRC.val();
            veiculoVinculado.Renavam.val = _veiculoVinculado.Renavam.val();
            veiculoVinculado.ObservacaoCTe.val = _veiculoVinculado.ObservacaoCTe.val();
            veiculoVinculado.Proprietario.val = _veiculoVinculado.Proprietario.val();
            veiculoVinculado.Proprietario.codEntity = _veiculoVinculado.Proprietario.codEntity();
            veiculoVinculado.MarcaVeiculo.val = _veiculoVinculado.MarcaVeiculo.val();
            veiculoVinculado.MarcaVeiculo.codEntity = _veiculoVinculado.MarcaVeiculo.codEntity();

            veiculoVinculado.ModeloVeicularCarga.val = _veiculoVinculado.ModeloVeicularCarga.val();
            veiculoVinculado.ModeloVeicularCarga.codEntity = _veiculoVinculado.ModeloVeicularCarga.codEntity();

            veiculoVinculado.ModeloVeiculo.val = _veiculoVinculado.ModeloVeiculo.val();
            veiculoVinculado.ModeloVeiculo.codEntity = _veiculoVinculado.ModeloVeiculo.codEntity();

            _veiculo.VeiculosVinculados.list.push(veiculoVinculado);
            recarregarGridVeiculosViculados();
            $("#" + _veiculoVinculado.Placa.id).focus();
        } else {
            exibirMensagem("aviso", Localization.Resources.Veiculos.Veiculo.VeiculoJaInformado, Localization.Resources.Veiculos.Veiculo.VeiculoInformadoJaFoiVinculadoEsteVeiculo.format(_veiculoVinculado.Placa.val(), _veiculo.Placa.val()));
        }
        LimparCamposVeiculosVinculados();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}


function atualizarVeiculoVinculadoClick() {
    var tudoCerto = ValidarCamposObrigatorios(_veiculoVinculado);
    if (tudoCerto) {
        $.each(_veiculo.VeiculosVinculados.list, function (i, veiculoVinculado) {
            if (veiculoVinculado.Placa.val == _veiculoVinculado.Placa.val()) {
                veiculoVinculado.Tara.val = _veiculoVinculado.Tara.val();
                veiculoVinculado.Tara.val = _veiculoVinculado.Tara.val();
                veiculoVinculado.CapacidadeM3.val = _veiculoVinculado.CapacidadeM3.val();
                veiculoVinculado.CapacidadeQuilo.val = _veiculoVinculado.CapacidadeQuilo.val();
                veiculoVinculado.TipoRodado.val = _veiculoVinculado.TipoRodado.val();
                veiculoVinculado.Tipo.val = _veiculoVinculado.Tipo.val();
                veiculoVinculado.TipoCarroceria.val = _veiculoVinculado.TipoCarroceria.val();
                veiculoVinculado.Estado.val = _veiculoVinculado.Estado.val();
                veiculoVinculado.TipoProprietario.val = _veiculoVinculado.TipoProprietario.val();
                veiculoVinculado.RNTRC.val = _veiculoVinculado.RNTRC.val();
                veiculoVinculado.Renavam.val = _veiculoVinculado.Renavam.val();
                veiculoVinculado.ObservacaoCTe.val = _veiculoVinculado.ObservacaoCTe.val();
                veiculoVinculado.Proprietario.val = _veiculoVinculado.Proprietario.val();
                veiculoVinculado.Proprietario.codEntity = _veiculoVinculado.Proprietario.codEntity();
                veiculoVinculado.MarcaVeiculo.val = _veiculoVinculado.MarcaVeiculo.val();
                veiculoVinculado.MarcaVeiculo.codEntity = _veiculoVinculado.MarcaVeiculo.codEntity();
                veiculoVinculado.ModeloVeicularCarga.val = _veiculoVinculado.ModeloVeicularCarga.val();
                veiculoVinculado.ModeloVeicularCarga.codEntity = _veiculoVinculado.ModeloVeicularCarga.codEntity();
                veiculoVinculado.ModeloVeiculo.val = _veiculoVinculado.ModeloVeiculo.val();
                veiculoVinculado.ModeloVeiculo.codEntity = _veiculoVinculado.ModeloVeiculo.codEntity();
                return false;
            }
        });
        recarregarGridVeiculosViculados();
        LimparCamposVeiculosVinculados();
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function excluirVeiculoVinculadoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Veiculos.Veiculo.RealmenteDesejaExcluirOVeiculoVinculado.format(_veiculoVinculado.Placa.val()), function () {
        var data = { Reboque: _veiculoVinculado.Placa.val(), Veiculo: _veiculo.Codigo.val() };
        executarReST("Veiculo/ValidarPneuVeiculo", data, function (e) {
            if (e.Success) {
                var listaAtualizada = new Array();
                $.each(_veiculo.VeiculosVinculados.list, function (i, veiculoVinculado) {
                    if (veiculoVinculado.Placa.val !== _veiculoVinculado.Placa.val()) {
                        listaAtualizada.push(veiculoVinculado);
                    }
                });
                _veiculo.VeiculosVinculados.list = listaAtualizada;
                recarregarGridVeiculosViculados();
                LimparCamposVeiculosVinculados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, e.Msg);
        });
    });
}


//*******MÉTODOS*******

function veiculoVinculadoMudouPropriedadeOnChange() {
    if (_veiculoVinculado.Tipo.val() == "T") {
        _veiculoVinculado.TipoProprietario.required = true;
        _veiculoVinculado.RNTRC.required = true;
        _veiculoVinculado.Proprietario.required = true;
        _veiculoVinculado.Tipo.visibleFade(true);

        _veiculoVinculado.Proprietario.visible(true);
        _veiculoVinculado.TipoProprietario.visible(true);
        _veiculoVinculado.RNTRC.visible(true);
    } else {
        _veiculoVinculado.TipoProprietario.required = false;
        _veiculoVinculado.RNTRC.required = false;
        _veiculoVinculado.Proprietario.required = false;
        _veiculoVinculado.Tipo.visibleFade(false);
        _veiculoVinculado.Proprietario.visible(false);
        _veiculoVinculado.TipoProprietario.visible(false);
        _veiculoVinculado.RNTRC.visible(false);
    }
}

function recarregarGridVeiculosViculados() {
    var data = new Array();
    $.each(_veiculo.VeiculosVinculados.list, function (i, veiculo) {
        var veiculoVinculado = new Object();
        veiculoVinculado.Placa = veiculo.Placa.val;
        veiculoVinculado.ModeloVeicularCarga = veiculo.ModeloVeicularCarga.val;
        veiculoVinculado.CapacidadeQuilo = veiculo.CapacidadeQuilo.val;
        data.push(veiculoVinculado);
    });
    _gridVeiculosVinculados.CarregarGrid(data);
}

function editarVeiculoVinculado(data) {
    LimparCampos(_veiculoVinculado);
    $.each(_veiculo.VeiculosVinculados.list, function (i, veiculoVinculado) {
        if (veiculoVinculado.Placa.val == data.Placa) {
            _veiculoVinculado.Placa.val(veiculoVinculado.Placa.val);
            _veiculoVinculado.Tara.val(veiculoVinculado.Tara.val);
            _veiculoVinculado.CapacidadeM3.val(veiculoVinculado.CapacidadeM3.val);
            _veiculoVinculado.CapacidadeQuilo.val(veiculoVinculado.CapacidadeQuilo.val);
            _veiculoVinculado.TipoRodado.val(veiculoVinculado.TipoRodado.val);
            _veiculoVinculado.Tipo.val(veiculoVinculado.Tipo.val);
            _veiculoVinculado.TipoCarroceria.val(veiculoVinculado.TipoCarroceria.val);
            _veiculoVinculado.Estado.val(veiculoVinculado.Estado.val);
            _veiculoVinculado.TipoProprietario.val(veiculoVinculado.TipoProprietario.val);
            _veiculoVinculado.RNTRC.val(veiculoVinculado.RNTRC.val);
            _veiculoVinculado.Renavam.val(veiculoVinculado.Renavam.val);
            _veiculoVinculado.ObservacaoCTe.val(veiculoVinculado.ObservacaoCTe.val);
            _veiculoVinculado.Proprietario.val(veiculoVinculado.Proprietario.val);
            _veiculoVinculado.Proprietario.codEntity(veiculoVinculado.Proprietario.codEntity);
            _veiculoVinculado.MarcaVeiculo.val(veiculoVinculado.MarcaVeiculo.val);
            _veiculoVinculado.MarcaVeiculo.codEntity(veiculoVinculado.MarcaVeiculo.codEntity);
            _veiculoVinculado.ModeloVeicularCarga.val(veiculoVinculado.ModeloVeicularCarga.val);
            _veiculoVinculado.ModeloVeicularCarga.codEntity(veiculoVinculado.ModeloVeicularCarga.codEntity);
            _veiculoVinculado.ModeloVeiculo.val(veiculoVinculado.ModeloVeiculo.val);
            _veiculoVinculado.ModeloVeiculo.codEntity(veiculoVinculado.ModeloVeiculo.codEntity);

            if (_veiculoVinculado.ModeloVeicularCarga.codEntity() > 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
                _veiculoVinculado.ModeloVeicularCarga.enable(false);
            else
                _veiculoVinculado.ModeloVeicularCarga.enable(true);

            return false;
        }
    });

    _veiculoVinculado.Adicionar.visible(false);
    _veiculoVinculado.Atualizar.visible(true);
    _veiculoVinculado.Excluir.visible(true);
    _veiculoVinculado.Cancelar.visible(true);
    veiculoVinculadoMudouPropriedadeOnChange();

}

function LimparCamposVeiculosVinculados() {
    LimparCampos(_veiculoVinculado);
    _veiculoVinculado.Adicionar.visible(true);
    _veiculoVinculado.Atualizar.visible(false);
    _veiculoVinculado.Excluir.visible(false);
    _veiculoVinculado.Cancelar.visible(false);
    veiculoVinculadoMudouPropriedadeOnChange();
}