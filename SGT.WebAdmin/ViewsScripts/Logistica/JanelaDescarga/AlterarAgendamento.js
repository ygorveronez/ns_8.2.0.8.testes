/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../../js/Global/Grid.js" />

var _alterarAgendamento;
var _gridPedidosAgendamento;
var _pedidosAgendamentoExistentes = [];

var AlterarAgendamento = function () {
    this.CodigoAgendamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Destinatario = PropertyEntity({ val: ko.observable(0), codEntity: ko.observable(0), type: types.entity });

    this.Pedidos = PropertyEntity({ type: types.map, text: "Adicionar Pedido", getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.Salvar = PropertyEntity({ eventClick: salvarAlteracaoAgendamentoClick, text: "Salvar", type: types.event });
}

function loadAlterarAgendamento() {
    _alterarAgendamento = new AlterarAgendamento();
    KoBindings(_alterarAgendamento, "knockoutAlterarAgendamento");

    loadGridPedidosAgendamento();
}

function abrirModalAlterarAgendamento(cargaSelecionada) {
    _alterarAgendamento.CodigoAgendamento.val(cargaSelecionada.CodigoAgendamentoColeta);
    _alterarAgendamento.Destinatario.val(cargaSelecionada.Destinatario);
    _alterarAgendamento.Destinatario.codEntity(cargaSelecionada.CPFCNPJDestinatario);

    executarReST("AgendamentoColeta/BuscarPedidosAgendamento", { Codigo: _alterarAgendamento.CodigoAgendamento.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pedidosAgendamentoExistentes = retorno.Data.ListaPedidos;
                _gridPedidosAgendamento.CarregarGrid(_pedidosAgendamentoExistentes);

                $("#divModalAlterarAgendamento")
                    .modal('show')
                    .on('hidden.bs.modal', function () {
                        LimparCampos(_alterarAgendamento);
                        _alterarAgendamento.Pedidos.val([]);
                        _gridPedidosAgendamento.CarregarGrid([]);
                        _pedidosAgendamentoExistentes = [];
                    });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function salvarAlteracaoAgendamentoClick() {
    var dados = {
        CodigoAgendamento: _alterarAgendamento.CodigoAgendamento.val(),
        Pedidos: JSON.stringify(obterPedidosAgendamentoColeta())
    };

    executarReST("AgendamentoColeta/AlterarAgendamento", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterações realizadas com sucesso.");
                $("#divModalAlterarAgendamento")
                    .modal('hide');
                _tabelaDescarregamento._gridTabelaDescarregamento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function loadGridPedidosAgendamento() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirPedidoAgendamentoClick }] };

    var editableVolumes = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        maxlength: 0,
        numberMask: { precision: 0, thousands: "", allowZero: false }
    };

    var editableSKU = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        maxlength: 0,
        numberMask: { precision: 0, thousands: "", allowZero: false }
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DT_Enable", visible: false },
        { data: "DT_RowId", visible: false },
        { data: "NumeroPedidoEmbarcador", title: "Pedido", width: "25%" },
        { data: "Saldo", title: "Saldo Disponível", width: "25%" },
        { data: "VolumesEnviar", title: "Caixas", width: "25%", editableCell: editableVolumes },
        { data: "SKU", title: "Itens", width: "25%", editableCell: editableSKU }
    ];

    var editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: callbackEditarColuna
    };

    _gridPedidosAgendamento = new BasicDataTable(_alterarAgendamento.Pedidos.idGrid, header, menuOpcoes, null, null, 5, null, null, editarColuna);

    new BuscarPedidosPendetesAgendamento(_alterarAgendamento.Pedidos, null, _gridPedidosAgendamento, _alterarAgendamento.Destinatario);
    _alterarAgendamento.Pedidos.basicTable = _gridPedidosAgendamento;

    _gridPedidosAgendamento.CarregarGrid([]);
}

function excluirPedidoAgendamentoClick(registroSelecionado) {
    if (!_ConfiguracoesJanelaDescarga.PermiteExcluirAgendamentoDaCargaJanelaDescarga) {
        if (_pedidosAgendamentoExistentes.filter((pedido) => {
            return pedido.Codigo == registroSelecionado.Codigo;
        }).length > 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Não é possível excluir pedidos que já foram salvos.");
            return;
        }
    }
    
    var registros = _gridPedidosAgendamento.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);

            _gridPedidosAgendamento.CarregarGrid(registros);
            break;
        }
    }
}

function callbackEditarColuna(dataRow, row, head) {
    var volumesEnviar = parseInt(dataRow.VolumesEnviar);
    var saldoDisponivel = parseInt(dataRow.Saldo);

    if (volumesEnviar > saldoDisponivel) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "A quantidade de volumes para enviar não pode ser superior ao saldo disponível.");
        dataRow.VolumesEnviar = saldoDisponivel.toString();

        var registros = _gridPedidosAgendamento.BuscarRegistros();
        _gridPedidosAgendamento.CarregarGrid(registros);
    }
}

function obterPedidosAgendamentoColeta() {
    var registros = _gridPedidosAgendamento.BuscarRegistros();

    return registros.map(function (registro) {
        return {
            Codigo: parseInt(registro.Codigo),
            VolumesEnviar: parseInt(registro.VolumesEnviar),
            SKU: parseInt(registro.SKU)
        }
    });
}