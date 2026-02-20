/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="FechamentoAbastecimento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAbastecimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _listaAbastecimento;
var _gridAbastecimentos;
var _abastecimento;
var _pesquisaAbastecimentos;

var ListaAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Veiculo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Equipamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Posto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAbastecimentos.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.TotalizadorValorTotal = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Totalizador Valor Total:" });
    this.TotalizadorLitros = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: "Totalizador Litros:" });
    this.TotalizadorHorimetro = PropertyEntity({ val: ko.observable("0"), def: "0", text: "Totalizador Horímetro:" });
    this.TotalizadorKM = PropertyEntity({ val: ko.observable("0"), def: "0", text: "Totalizador KM:" });
    this.ListaSumarizadores = ko.observableArray();
};

var Abastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.KM = PropertyEntity({ val: ko.observable(""), text: ko.observable("Quilometragem:"), getType: typesKnockout.int, maxlength: 10 });
    this.Horimetro = PropertyEntity({ val: ko.observable(""), text: ko.observable("Horímetro:"), getType: typesKnockout.int, maxlength: 10 });

    this.Litros = PropertyEntity({ text: "*Litros:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 6 });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable(0.0000), maxlength: 8 });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });

    this.Atualizar = PropertyEntity({ eventClick: atualizarAbastecimentoClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAbastecimentoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.DataInicio = PropertyEntity({ text: "Data início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.VeiculoFechamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var PesquisaAbastecimentos = function () {
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoAbastecimento.Todos), options: EnumSituacaoAbastecimento.obterOpcoesPesquisa(), def: EnumSituacaoAbastecimento.Todos });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid() });
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posto:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _listaAbastecimento.Situacao.val(_pesquisaAbastecimentos.Situacao.val());
            _listaAbastecimento.Veiculo.val(_pesquisaAbastecimentos.Veiculo.codEntity());
            _listaAbastecimento.Equipamento.val(_pesquisaAbastecimentos.Equipamento.codEntity());
            _listaAbastecimento.Posto.val(_pesquisaAbastecimentos.Posto.codEntity());

            _gridAbastecimentos.CarregarGrid(atualizarTotalizadores);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadAbastecimentos() {
    // Abastecimento (editar)
    _abastecimento = new Abastecimento();
    KoBindings(_abastecimento, "knockoutCadastroAbastecimento");

    _pesquisaAbastecimentos = new PesquisaAbastecimentos();
    KoBindings(_pesquisaAbastecimentos, "knockoutPesquisaAbastecimentos");

    new BuscarClientes(_pesquisaAbastecimentos.Posto);
    new BuscarVeiculos(_pesquisaAbastecimentos.Veiculo);
    new BuscarEquipamentos(_pesquisaAbastecimentos.Equipamento);

    // Abastecimentos (grid)
    _listaAbastecimento = new ListaAbastecimento();
    KoBindings(_listaAbastecimento, "knockoutFechamentoAbastecimento");

    buscarAbastecimentos();

}

function buscarAbastecimentos() {
    var editar = { descricao: "Editar", id: guid(), metodo: editarAbastecimento, icone: "", visibilidade: visibleEditar };
    var removerDoFechamento = { descricao: "Remover", id: guid(), metodo: removerDoFechamentoClick, icone: "", visibilidade: visibleRemover };
    var adicionarAoFechamento = { descricao: "Adicionar", id: guid(), metodo: adicionarAoFechamentoClick, icone: "", visibilidade: visibleAdicionar };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [editar, removerDoFechamento, adicionarAoFechamento] };

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
    _gridAbastecimentos = new GridView(_listaAbastecimento.Pesquisar.idGrid, "FechamentoAbastecimento/BuscarAbastecimentos", _listaAbastecimento, menuOpcoes, null, 50, null, null, null, null, null, editarColuna);
}

function cancelarAbastecimentoClick() {
    LimparCampos(_pesquisaAbastecimentos);
    LimparCampos(_abastecimento);
    Global.fecharModal('divModalAbastecimento');
}

function atualizarAbastecimentoClick(e, sender) {
    _abastecimento.DataInicio.val(_fechamentoAbastecimento.DataInicio.val());
    _abastecimento.DataFim.val(_fechamentoAbastecimento.DataFim.val());
    _abastecimento.VeiculoFechamento.val(_fechamentoAbastecimento.Veiculo.val());
    Salvar(_abastecimento, "FechamentoAbastecimento/AtualizarAbastecimento", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                LimparCampos(_abastecimento);
                Global.fecharModal('divModalAbastecimento');
                CompararEAtualizarGridEditableDataRow(_globalDataRow, arg.Data)
                _gridAbastecimentos.AtualizarDataRow(_globalRow, _globalDataRow);

                atualizarTotalizadores();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function calculaLitrosAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_abastecimento.Litros.val() != null & _abastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.Litros.val()), "n2")).toFixed(2);

    if (_abastecimento.ValorUnitario.val() != null & _abastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorTotal.val() != null & _abastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (litros > 0) {
        if (valorUnitario > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        } else if (valorTotal > 0) {
            _abastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorUnitarioAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_abastecimento.Litros.val() != null & _abastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.Litros.val()), "n2")).toFixed(2);

    if (_abastecimento.ValorUnitario.val() != null & _abastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorTotal.val() != null & _abastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (valorUnitario > 0) {
        if (litros > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    } else if (valorTotal > 0) {
        if (litros > 0) {
            _abastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorTotalAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_abastecimento.Litros.val() != null & _abastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.Litros.val()), "n2")).toFixed(2);

    if (_abastecimento.ValorUnitario.val() != null & _abastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_abastecimento.ValorTotal.val() != null & _abastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_abastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (valorTotal > 0) {
        if (litros > 0) {
            _abastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    } else if (valorUnitario > 0) {
        if (litros > 0) {
            _abastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    }
}

function atualizarTotalizadores() {
    executarReST("FechamentoAbastecimento/BuscarTotalizadoresAbastecimento", {
        Codigo: _fechamentoAbastecimento.Codigo.val(),
        Situacao: _pesquisaAbastecimentos.Situacao.val(),
        Veiculo: _pesquisaAbastecimentos.Veiculo.codEntity(),
        Equipamento: _pesquisaAbastecimentos.Equipamento.codEntity(),
        Posto: _pesquisaAbastecimentos.Posto.codEntity()
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;
                _listaAbastecimento.TotalizadorKM.val(data.TotalizadorKM);
                _listaAbastecimento.TotalizadorHorimetro.val(data.TotalizadorHorimetro);
                _listaAbastecimento.TotalizadorLitros.val(data.TotalizadorLitros);
                _listaAbastecimento.TotalizadorValorTotal.val(data.TotalizadorValorTotal);
                _listaAbastecimento.ListaSumarizadores(data.ListaSumarizadores.slice());
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function carregarAbastecimentos(callback) {
    _listaAbastecimento.Codigo.val(_fechamentoAbastecimento.Codigo.val());
    _gridAbastecimentos.CarregarGrid(callback);

    atualizarTotalizadores();
}

function editarAbastecimento(dataRow, row) {
    // So permite edicao quando o status do fechamento e pendente
    if (EnumSituacaoFechamentoAbastecimento.Pendente != _fechamentoAbastecimento.Situacao.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível editar abastecimentos de um fechamento finalizado.");
        return;
    }

    // Exibe modal para editar abastecimento
    LimparCampos(_pesquisaAbastecimentos);
    LimparCampos(_abastecimento);

    _abastecimento.Codigo.val(dataRow.Codigo);
    var veiculoPlacaGrid = dataRow.Placa;
    var equipamentoGrid = dataRow.Equipamento;
    BuscarPorCodigo(_abastecimento, "Abastecimento/BuscarPorCodigo", function (arg) {
        _globalDataRow = dataRow;
        _globalRow = row;        
        Global.abrirModal('divModalAbastecimento');
    }, null);

    if ((veiculoPlacaGrid != null && veiculoPlacaGrid != "") && (equipamentoGrid != null && equipamentoGrid != "")) {
        _abastecimento.KM.required = false;
        _abastecimento.KM.text("Quilometragem:");
        _abastecimento.Horimetro.required = true;
        _abastecimento.Horimetro.text("*Horímetro:");

    } else if (veiculoPlacaGrid != null && veiculoPlacaGrid != "") {
        _abastecimento.KM.required = true;
        _abastecimento.KM.text("*Quilometragem:");
        _abastecimento.Horimetro.required = false;
        _abastecimento.Horimetro.text("Horímetro:");

    } else if (equipamentoGrid != null && equipamentoGrid != "") {
        _abastecimento.KM.required = false;
        _abastecimento.KM.text("Quilometragem:");
        _abastecimento.Horimetro.required = true;
        _abastecimento.Horimetro.text("*Horímetro:");
    }

}

function adicionarAoFechamentoClick(dataRow, row) {
    // So permite adicao quando o status do fechamento e pendente
    if (EnumSituacaoFechamentoAbastecimento.Pendente != _fechamentoAbastecimento.Situacao.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível adicionar abastecimentos em um fechamento finalizado.");
        return;
    }

    // Adiciona abastecimento
    var Dados = {
        Codigo: dataRow.Codigo,
        Fechamento: _fechamentoAbastecimento.Codigo.val()
    };

    executarReST("FechamentoAbastecimento/AdicionarAoFechamento", Dados, function (args) {
        dataRow.DT_Enable = true;
        _gridAbastecimentos.AtualizarDataRow(row, dataRow);

        $.each(_fechamentoAbastecimento.NaoAdicionar.val(), function (i, codigoFechamento) {
            if (codigoFechamento == dataRow.Codigo) {
                _fechamentoAbastecimento.NaoAdicionar.val().splice(i, 1);
                return false;
            }
        });

        atualizarTotalizadores();
    });
}

function removerDoFechamentoClick(dataRow, row) {
    // So permite remocao quando o status do fechamento e pendente
    if (EnumSituacaoFechamentoAbastecimento.Pendente != _fechamentoAbastecimento.Situacao.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Não é possível remover abastecimentos de um fechamento finalizado.");
        return;
    }

    // Remove abastecimento
    var Dados = {
        Codigo: dataRow.Codigo,
        Fechamento: _fechamentoAbastecimento.Codigo.val()
    };

    executarReST("FechamentoAbastecimento/RemoveDoFechamento", Dados, function (args) {
        dataRow.DT_Enable = false;
        _gridAbastecimentos.AtualizarDataRow(row, dataRow);
        _fechamentoAbastecimento.NaoAdicionar.val().push(dataRow.Codigo);

        atualizarTotalizadores();
    });
}

function LimparDetalhesAbastecimentos() {
    //SetarPercentualProcessamento(0);
    $("#liAbastecimentos").hide();
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = {
        Codigo: dataRow.Codigo,
        KM: dataRow.KM,
        Litros: dataRow.Litros,
        Valor: dataRow.Valor,
        Horimetro: dataRow.Horimetro
    };

    executarReST("FechamentoAbastecimento/AlterarColunaAbastecimento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                _gridAbastecimentos.AtualizarDataRow(row, dataRow, callbackTabPress);

                atualizarTotalizadores();
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function formatarStrFloat(valor) {
    valor = valor.replace(".", "");
    return valor.replace(",", ".");
}