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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="Motoristas.js" />
/// <reference path="MovimentacaoDePlacas.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Equipamento.js" />

var _tipoVeiculo = [
    { text: "Tração", value: "0" },
    { text: "Reboque", value: "1" }
];

//*******MAPEAMENTO KNOUCKOUT*******

var _gridReboque;
var _pesquisaReboque;
var _veiculoSelecionado;
var _pesquisaFuncionarioResponsavel;

var PesquisaVeiculo = function () {
    this.PlacaReboque = PropertyEntity({ text: "Placa: " });
    this.TipoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Reboque:", idBtnSearch: guid() });
    this.NumeroFrota = PropertyEntity({ text: "Nº Frota: ", val: ko.observable(""), maxlength: 30 });
    this.SomentePedentes = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: "Retornar somente reboques disponíveis?", def: ko.observable(true) });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable("1"), options: _tipoVeiculo, def: "1", text: "*Tipo de Veículo: ", required: false, visible: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridReboque.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadVeiculos(callback) {
    _pesquisaReboque = new PesquisaVeiculo();
    KoBindings(_pesquisaReboque, "knockoutPesquisaReboque", false, _pesquisaReboque.Pesquisar.id);
    $("#" + _pesquisaReboque.PlacaReboque.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _pesquisaReboque.SomentePedentes.id).click(function () {
        if (_gridReboque != null)
            _gridReboque.CarregarGrid();
    });

    new BuscarModelosVeicularesCarga(_pesquisaReboque.TipoReboque, retornoMovelosVeiculares, null, null, null, _pesquisaReboque.TipoVeiculo);

    _opcoes = new Opcoes();
    KoBindings(_opcoes, "divOpcoes");

    new BuscarCentroResultado(_opcoes.AlterarCentroResultado, null, null, AlterarCentroResultado);
    /*new BuscarCentroResultado(_opcoes.AlterarGestor, null, null, AlterarGestor);*/
    new BuscarFuncionario(_opcoes.AlterarGestor, AlterarGestor);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descricao", width: "80%" }
    ];
    _gridEquipamentos = new BasicDataTable(_opcoes.Grid.id, header, null, { column: 0, dir: orderDir.asc });

    new BuscarEquipamentos(_opcoes.Equipamento, InformarEquipamentos, null, null, _gridEquipamentos);

    _opcoes.Equipamento.basicTable = _gridEquipamentos;
    _gridEquipamentos.CarregarGrid([]);

    buscarReboques(callback);
}

//*******MÉTODOS*******

function AlterarEquipamentoVeiculoClick(tipo) {
    _veiculoSelecionado = tipo;
    $("#" + _opcoes.Equipamento.idBtnSearch).click();
}

function InformarEquipamentos(data) {
    if (_veiculoSelecionado != null && _veiculoSelecionado != undefined && _veiculoSelecionado > 0) {
        var codigosEquipamentos = new Array();

        for (var i = 0; i < data.length; i++)
            codigosEquipamentos.push(data[i].Codigo);

        executarReST("MovimentacaoDePlacas/InformarEquipamentos", { Veiculo: _veiculoSelecionado, Equipamentos: JSON.stringify(codigosEquipamentos) }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    buscarVeiculosAlterar();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Nenhum veículo selecionado.");
}

function AlterarCentroResultado(data) {
    if (_codigoVeiculoSelecionado != null && _codigoVeiculoSelecionado != undefined && _codigoVeiculoSelecionado > 0) {
        var dataEnvio = {
            Veiculo: _codigoVeiculoSelecionado, CentroResultado: data.Codigo
        }
        executarReST("MovimentacaoDePlacas/AlterarCentroResultado", dataEnvio, function (e) {
            if (e.Success) {
                if (e.Data != false) {
                    buscarVeiculosAlterar();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Nenhum conjunto selecionado.");
}

function AlterarGestor(data) {

    if (_codigoVeiculoSelecionado != null && _codigoVeiculoSelecionado != undefined && _codigoVeiculoSelecionado > 0) {
        var dataEnvio = {
            Veiculo: _codigoVeiculoSelecionado,
            GestorSelecionado: data.Codigo,
            NomeGestor: data.Descricao,
            Cpf: data.CPF
        }
        executarReST("MovimentacaoDePlacas/AlterarGestor", dataEnvio, function (e) {
            if (e.Success) {
                if (e.Data != false) {

                    buscarVeiculosAlterar();

                    //exibirMensagem(tipoMensagem.atencao, "Atenção", "Rever!!!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Nenhum conjunto selecionado.");



/*
    if (_codigoGestorSelecionado != null && _codigoGestorSelecionado != undefined && codigoGestorSelecionado > 0) {
        var dataEnvio = {
            Veiculo: _codigoGestorSelecionado, CentroResultado: data.Codigo
        }
        executarReST("MovimentacaoDePlacas/AlterarGestor", dataEnvio, function (e) {
            if (e.Success) {
                if (e.Data != false) {
                    buscarVeiculosAlterar(); // Rever
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Nenhum gestor selecionado.");
*/
}

function retornoMovelosVeiculares(data) {
    if (data != null) {
        _pesquisaReboque.TipoReboque.codEntity(data.Codigo);
        _pesquisaReboque.TipoReboque.val(data.Descricao);
        buscarReboques();
    }
}

function consultarReboques(e) {
    if (_gridReboque != null)
        _gridReboque.CarregarGrid();
}

function verificaPlacaReboque(e) {
    if ($("#" + _pesquisaReboque.PlacaReboque.id).val().replace(/\_/g, "").length < 7 && $("#" + _pesquisaReboque.PlacaReboque.id).val().replace(/\_/g, "").length > 0) {
        _pesquisaReboque.PlacaReboque.val("");
        $("#" + _pesquisaReboque.PlacaReboque.id).val("");
    }
}

function buscarReboques(callback) {
    _gridReboque = new GridView(_pesquisaReboque.Pesquisar.idGrid, "Veiculo/PesquisaReboqueMovimentacaoDePlacas", _pesquisaReboque, null, null, null, null, false, true);
    if (callback != null) {
        _gridReboque.CarregarGrid(function () {
            loadMotoristas(callback);
        });
    } else {
        _gridReboque.CarregarGrid();
    }
}

