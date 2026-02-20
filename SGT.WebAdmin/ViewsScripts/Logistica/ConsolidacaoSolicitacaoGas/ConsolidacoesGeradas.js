var _consolidacoesGeradas;
var _gridConsolidacoesGeradas;

var ConsolidacoesGeradas = function () {
    this.ConsolidacoesGeradas = PropertyEntity({ visible: ko.observable(true), idGrid: guid() });
}

function loadConsolidacoesGeradas() {
    _consolidacoesGeradas = new ConsolidacoesGeradas();
    KoBindings(_consolidacoesGeradas, "knockoutConsolidacoesGeradas");

    loadGridConsolidacoesGeradas();
}

function loadGridConsolidacoesGeradas() {
    var opcaoCancelar = {
        descricao: "Cancelar",
        id: guid(),
        evento: "onclick",
        metodo: cancelarConsolidacaoClick,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoCancelar]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Carga", title: "Carga", width: "12%" },
        { data: "Origem", title: "Origem", width: "20%" },
        { data: "Quantidade", title: "Quantidade", width: "10%" },
        { data: "ModeloVeicular", title: "Modelo Veicular", width: "15%" },
        { data: "SituacaoCarga", title: "Situação Carga", width: "12%" }
    ];

    _gridConsolidacoesGeradas = new BasicDataTable(_consolidacoesGeradas.ConsolidacoesGeradas.idGrid, header, menuOpcoes, null, null, 5);
    _gridConsolidacoesGeradas.CarregarGrid([]);
}

function limparCamposConsolidacoesGeradas() {
    _gridConsolidacoesGeradas.CarregarGrid([]);
}

function cancelarConsolidacaoClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Deseja cancelar a consolidação?", function () {
        executarReST("ConsolidacaoSolicitacaoGas/CancelarConsolidacao", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                var registros = _gridConsolidacoesGeradas.BuscarRegistros();

                for (var i = 0; i < registros.length; i++) {
                    if (registros[i].Codigo == registroSelecionado.Codigo) {
                        registros[i].SituacaoCarga = "Cancelada";
                        break;
                    }
                }

                _gridConsolidacoesGeradas.CarregarGrid(registros);
                _gridConsolidacaoSolicitacaoGas.CarregarGrid();
                _gridQuantidades.CarregarGrid();
                Global.fecharModal('divModalConsolidarSolicitacao');

                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            }
        })
    });
}