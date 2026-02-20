/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoComplementoContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoDisponibilidadeContratoFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoContratoFrete.js" />
/// <reference path="ContratoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _veiculos;
var _gridVeiculos;
var _gridResumoVeiculos;
var _stateVeiculo = null;
var _PossuiCadastroAcordos = false;

var BG_DANGER = "#C1656560";
var BG_SUCCESS = "#DFF0D8";
var BG_LEGENDA_DANGER = "#C16565";
var BG_LEGENDA_SUCCESS = "#356e35";

var _tipoPagamentoContratoFrete = [
    { text: "Diário", value: EnumTipoPagamentoContratoFrete.Diaria },
    { text: "Quinzenal", value: EnumTipoPagamentoContratoFrete.Quinzena },
];

var Veiculos = function () {
    this.ContratoFreteTransportador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoPagamentoContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoPagamentoContratoFrete.Diaria), options: _tipoPagamentoContratoFrete, def: EnumTipoPagamentoContratoFrete.Diaria, text: "Pagamento: ", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", enable: ko.observable(true), visible: ko.observable(true), idBtnSearch: guid(), required: true });
    this.TipoDisponibilidadeContratoFrete = PropertyEntity({ text: _contratoFreteTransportador.TipoDisponibilidadeContratoFrete.text, val: _contratoFreteTransportador.TipoDisponibilidadeContratoFrete.val, def: _contratoFreteTransportador.TipoDisponibilidadeContratoFrete.def, options: ko.observable(EnumTipoDisponibilidadeContratoFrete.obterOpcoes()), visible: ko.observable(true) });

    this.Veiculos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });

    this.Veiculos.val.subscribe(function () {
        _contratoFreteTransportador.Veiculos.val(JSON.stringify(_veiculos.Veiculos.val()));
        RenderizarGridVeiculo();
    });

    this.Resumo = PropertyEntity({
        type: types.local, getType: typesKnockout.dynamic, def: [], val: ko.observable([]), visible: ko.observable(true), idGrid: guid(),
        Legendas: [
            {
                Nome: "Atendido",
                Cor: BG_LEGENDA_SUCCESS
            },
            {
                Nome: "Não Atendido",
                Cor: BG_LEGENDA_DANGER
            }
        ]
    });

    this.AdicionarMultiplos = PropertyEntity({ type: types.event, text: "Adicionar", enable: ko.observable(true), visible: ko.observable(true), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarVeiculoAGrid, text: "Adicionar", enable: ko.observable(true), visible: ko.observable(true), idBtnSearch: guid() });

    this.ImportarVeiculo = PropertyEntity({
        type: types.local,
        text: "Importar Veículos",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "Veiculo/ImportarVeiculos",
        UrlConfiguracao: "Veiculo/ConfiguracaoImportacaoVeiculosContratoFreteTransportador",
        CodigoControleImportacao: EnumCodigoControleImportacao.O056_VeiculosContratoFreteTransportador,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        },
        CallbackImportacao: function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    var veiculos = retorno.Data.Retorno;

                    var dataGrid = _veiculos.Veiculos.val();

                    veiculos.forEach(function (veiculo) {
                        var item = {
                            Codigo: veiculo.Codigo,
                            CodigoModeloVeicular: veiculo.ModeloVeicularCarga.Codigo,
                            ModeloVeicularCarga: veiculo.ModeloVeicularCarga.Descricao,
                            Placa: veiculo.Placa,
                            TipoPagamentoContratoFrete: _veiculos.TipoPagamentoContratoFrete.val(),
                            DescricaoTipoPagamentoContratoFrete: DescricaoEnumTipoPagamentoContratoFrete(_veiculos.TipoPagamentoContratoFrete.val()),
                            CapacidadeKG: veiculo.CapacidadeKG,
                            CapacidadeM3: veiculo.CapacidadeM3
                        };

                        dataGrid.push(item);
                    });

                    _veiculos.Veiculos.val(dataGrid);

                    LimparCamposAdicionarVeiculo();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        }
    });
}

//*******EVENTOS*******

function LoadVeiculos() {
    _veiculos = new Veiculos();
    KoBindings(_veiculos, "knockoutVeiculos");

    LoadGridVeiculos();
    LoadGridResumoVeiculo();

    $("body").on("abaschange", abasVeiculoChange);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        //_veiculos.TipoPagamentoContratoFrete.visible(false);
        _veiculos.Veiculo.visible(false);
        _veiculos.Adicionar.visible(false);

        /* fakeKnoutModeloVeicularCarga
         * Foi colocado esse objeto vazio só para não exibir o filtro de modelo veicular 
         * Foi um pedido do Jeferson (Carrefour)
         */
        var fakeKnoutModeloVeicularCarga = PropertyEntity({ codEntity: ko.observable(0) });

        new BuscarVeiculos(_veiculos.AdicionarMultiplos, AdicionarMultiplos, _contratoFreteTransportador.Transportador, fakeKnoutModeloVeicularCarga, null, null, null, null, null, null, null, null, null, _gridVeiculos, _gridResumoVeiculos);
    } else {
        _veiculos.AdicionarMultiplos.visible(false);

        new BuscarVeiculos(_veiculos.Veiculo, function (data) {
            if (!ValidaVeiculoSelecionado(data))
                return exibirMensagem(tipoMensagem.aviso, "Veículo", "O modelo do veículo não esta cadastrado na aba Valores Veículos.");

            _stateVeiculo = data;
            _veiculos.Veiculo.val(data.Placa);
            _veiculos.Veiculo.codEntity(data.Codigo);
        }, _contratoFreteTransportador.Transportador);
    }

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.ContratoFreteTransportador_DisponibilizarParaTodosVeiculos, _PermissoesPersonalizadas)) {
        _veiculos.TipoDisponibilidadeContratoFrete.options(ko.utils.arrayFilter(_veiculos.TipoDisponibilidadeContratoFrete.options(), function (option) {
            return option.value !== EnumTipoDisponibilidadeContratoFrete.TodosVeiculos;
        }));
    }

}

function abasVeiculoChange(e, opt) {
    // Gerencia aba
    _PossuiCadastroAcordos = opt.PossuiCadastroAcordos;

    // Gerencia grid
    if (_gridVeiculos) {
        var data = _gridVeiculos.BuscarRegistros();
        LoadGridVeiculos();
        _gridVeiculos.CarregarGrid(data);
    } else {
        LoadGridVeiculos();
    }

    // Gerencia Resumo
    if (_PossuiCadastroAcordos) {
        _veiculos.Resumo.visible(true);
    } else {
        _veiculos.Resumo.visible(false);
    }
}

function AdicionarVeiculoAGrid() {
    if (!ValidaVeiculo())
        return;

    executarReST("Veiculo/BuscarPorCodigo", { codigo: _stateVeiculo.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var veiculo = arg.Data;
                // Pega registros
                var dataGrid = _veiculos.Veiculos.val();

                // Objeto
                var item = {
                    Codigo: veiculo.Codigo,
                    CodigoModeloVeicular: _stateVeiculo.CodigoModeloVeicularCarga,
                    Placa: veiculo.Placa,
                    ModeloVeicularCarga: _stateVeiculo.ModeloVeicularCarga,
                    TipoPagamentoContratoFrete: _veiculos.TipoPagamentoContratoFrete.val(),
                    DescricaoTipoPagamentoContratoFrete: DescricaoEnumTipoPagamentoContratoFrete(_veiculos.TipoPagamentoContratoFrete.val()),
                    CapacidadeKG: veiculo.CapacidadeQuilo,
                    CapacidadeM3: veiculo.CapacidadeM3
                };

                // Adiciona a lista e atualiza a grid
                dataGrid.push(item);

                _veiculos.Veiculos.val(dataGrid);

                LimparCamposAdicionarVeiculo();

                if (_PossuiCadastroAcordos)
                    RenderizaResumoVeiculos();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ValidaVeiculo() {
    var msg = "";

    if (_stateVeiculo == null)
        msg = "Nenhum veículo selecionado";

    var dataGrid = _veiculos.Veiculos.val();
    for (var i = 0; i < dataGrid.length; i++) {
        if (dataGrid[i].Placa == _stateVeiculo.Placa) {
            msg = "Já existe um registro para a placa " + _stateVeiculo.Placa;
            break;
        }
    }

    if (msg != "")
        exibirMensagem(tipoMensagem.atencao, "Veículo Inválido", msg);

    return msg == "";
}

function RemoverVeiculoClick(data) {
    var dataGrid = _veiculos.Veiculos.val();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _veiculos.Veiculos.val(dataGrid);
    if (_PossuiCadastroAcordos)
        RenderizaResumoVeiculos();
}

//*******MÉTODOS*******

function EditarVeiculos(data) {
    _veiculos.ContratoFreteTransportador.val(_contratoFreteTransportador.Codigo.val());

    _veiculos.Veiculos.val(data.Veiculos);
}

function LimparCamposVeiculos() {
    _stateVeiculo = null;
    LimparCampos(_veiculos);
    _veiculos.Veiculos.val([]);
    RenderizarGridVeiculo();
}

function LoadGridVeiculos() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Excluir",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    if (_CAMPOS_BLOQUEADOS) return;
                    RemoverVeiculoClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "TipoPagamentoContratoFrete", visible: false },
        { data: "Placa", title: "Placa", width: "15%", className: "text-align-center" },
        { data: "ModeloVeicularCarga", title: "Modelo", width: "30%", className: "text-align-left" },
        { data: "DescricaoTipoPagamentoContratoFrete", visible: true, title: "Pagamento", width: "30%", className: "text-align-left" },
        { data: "CapacidadeKG", title: "Cap. KG", width: "15%", className: "text-align-right" },
        { data: "CapacidadeM3", title: "Cap. M³", width: "15%", className: "text-align-right" },
    ];

    // Grid
    _gridVeiculos = new BasicDataTable(_veiculos.Veiculos.idGrid, header, menuOpcoes, null, null, 10);
    _gridVeiculos.CarregarGrid([]);
}

function LoadGridResumoVeiculo() {
    //-- Grid
    // Cabecalho
    var header = [
        { data: "ModeloVeicular", title: "Modelo", width: "50%", className: "text-align-left" },
        { data: "Contratado", title: "Contratado", width: "20%", className: "text-align-right" },
        { data: "Disponibilizado", title: "Disponibilizado", width: "20%", className: "text-align-right" },
        { data: "DT_RowColor", visible: false },
        { data: "CodigoModelo", visible: false },
    ];

    // Grid
    _gridResumoVeiculos = new BasicDataTable(_veiculos.Resumo.idGrid, header, null, null, null, 10);
    _gridResumoVeiculos.CarregarGrid([]);
}

function RenderizaResumoVeiculos(resumo) {
    // fn chamada em acordo resumo
    if (!resumo)
        resumo = ExtraiResumoAcordos();

    var dataGrid = [];

    resumo.Resumo.forEach(function (linha) {
        var disponibilizado = GetDisponibilizadoPorModelo(linha.Modelo).length;
        var naoAtendido = (
            (disponibilizado > linha.Quantidade) ||
            ((_contratoFreteTransportador.TipoEmissaoComplemento.val() == EnumTipoEmissaoComplementoContratoFreteTransportador.PorVeiculoDoContrato) && (disponibilizado < linha.Quantidade))
        );

        var data = {
            ModeloVeicular: linha.Descricao,
            Contratado: linha.Quantidade,
            CodigoModelo: linha.Modelo,
            Disponibilizado: disponibilizado,
            DT_RowColor: naoAtendido ? BG_DANGER : BG_SUCCESS,
        };

        dataGrid.push(data);
    });

    _gridResumoVeiculos.CarregarGrid(dataGrid);
}

function validarVeiculosAdicionados() {
    var resumo = ExtraiResumoAcordos();

    for (var i = 0; i < resumo.Resumo.length; i++) {
        var linha = resumo.Resumo[i];
        var disponibilizado = GetDisponibilizadoPorModelo(linha.Modelo).length;

        if (disponibilizado > linha.Quantidade) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é permitido adicionar mais veiculos do tipo " + linha.Descricao + " do que o previsto no acordo.");
            return false;
        }

        if ((_contratoFreteTransportador.TipoEmissaoComplemento.val() == EnumTipoEmissaoComplementoContratoFreteTransportador.PorVeiculoDoContrato) && (disponibilizado < linha.Quantidade)) {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é permitido adicionar menos veiculos do tipo " + linha.Descricao + " do que o previsto no acordo.");
            return false;
        }
    }

    return true;
}

function GetDisponibilizadoPorModelo(modelo) {
    var veiculos = _gridVeiculos.BuscarRegistros();

    return veiculos.filter(function (veic) {
        return veic.CodigoModeloVeicular == modelo;
    });
}

function RenderizarGridVeiculo() {
    var Veiculos = _veiculos.Veiculos.val();

    _gridVeiculos.CarregarGrid(Veiculos);
}

function DescricaoEnumTipoPagamentoContratoFrete(tipoPagamentoContratoFrete) {
    if (tipoPagamentoContratoFrete == EnumTipoPagamentoContratoFrete.Diaria)
        return "Diário";
    if (tipoPagamentoContratoFrete == EnumTipoPagamentoContratoFrete.Quinzena)
        return "Quinzenal";

    return "";
}

function LimparCamposAdicionarVeiculo() {
    _veiculos.TipoPagamentoContratoFrete.val(_veiculos.TipoPagamentoContratoFrete.def);
    _veiculos.Veiculo.val("");
    _veiculos.Veiculo.codEntity(0);
    _stateVeiculo = null;
}

function ValidaVeiculoSelecionado(data) {
    var modelos = _valoresVeiculos.ValoresVeiculos.val();
    var valido = false;

    // Quando não ha modelo veicular cadastrado, permite selecionar qualquer veículo
    if (modelos.length == 0)
        return true;

    // Valida se o modelo veicular do veiculo selecionado esta cadastado na outra aba
    for (var i = 0; i < modelos.length; i++) {
        if (modelos[i].Codigo == data.CodigoModeloVeicularCarga) {
            valido = true;
            break;
        }
    }

    return valido;
}

function AdicionarMultiplos(data) {
    if (!$.isArray(data)) data = [data];

    var codigos = [];
    var placasInvalidas = [];
    var placasJaInseridas = [];

    var placas = _veiculos.Veiculos.val().map(function (i) { return i.Placa; });
    var modelos = _valoresVeiculos.ValoresVeiculos.val().map(function (i) { return i.Codigo; });

    data.forEach(function (i) {
        if ($.inArray(i.Placa, placas) >= 0)
            placasJaInseridas.push(i.Placa);
        else
            codigos.push(i.Codigo);
    });

    if (codigos.length > 0) {
        executarReST("Veiculo/BuscarVarios", { Codigos: JSON.stringify(codigos) }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var dataGrid = _veiculos.Veiculos.val();

                    for (var i in arg.Data) {
                        var veiculo = arg.Data[i];

                        veiculo.TipoPagamentoContratoFrete = _veiculos.TipoPagamentoContratoFrete.val(),
                            veiculo.DescricaoTipoPagamentoContratoFrete = DescricaoEnumTipoPagamentoContratoFrete(_veiculos.TipoPagamentoContratoFrete.val()),

                            dataGrid.push(veiculo);
                    }

                    _veiculos.Veiculos.val(dataGrid);

                    if (_PossuiCadastroAcordos)
                        RenderizaResumoVeiculos();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }

    if (placasInvalidas.length > 0) {
        exibirMensagem(tipoMensagem.atencao, "Placas Inválidas", "As placas " + placasInvalidas.join(", ") + " não possuem configuração para seus respectivos modelos veiculares.");
    }

    if (placasJaInseridas.length > 0) {
        exibirMensagem(tipoMensagem.atencao, "Placas Inválidas", "As placas " + placasJaInseridas.join(", ") + " já estão cadastradas.");
    }
}