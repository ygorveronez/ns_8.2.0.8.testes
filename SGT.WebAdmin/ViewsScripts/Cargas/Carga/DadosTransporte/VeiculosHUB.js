var _cargaVeiculosSugeridos;
var _gridCargaVeiculosSugeridos;

var CargaVeiculosSugeridos = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            GridConsulta.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true, idGrid: guid()
    });
}

function handleSelecaoVeiculoSugerido(rota) {
    clarity("set", "botão_confirmar_sugestão", "value_fix");
    exibirMensagem(tipoMensagem.ok, "Sucesso", "Transportadora vinculado com sucesso!");
    Global.fecharModal('divModalVeiculosSugeridosHUB');
}

function LoadCargaVeiculosSugeridos() {
    _cargaVeiculosSugeridos = new CargaVeiculosSugeridos();
    KoBindings(_cargaVeiculosSugeridos, "divModalVeiculosSugeridosHUB", false);

    var opcoes = {
        tipo: TypeOptionMenu.link,
        descricao: Localization.Resources.Gerais.Geral.Selecionar,
        tamanho: 5,
        opcoes: [
            {
                descricao: Localization.Resources.Gerais.Geral.Selecionar,
                id: guid(),
                evento: "onclick",
                metodo: handleSelecaoVeiculoSugerido,
                tamanho: "10",
                icone: ""
            }
        ]
    };
    _gridCargaVeiculosSugeridos = new GridView(_cargaVeiculosSugeridos.Pesquisar.idGrid, "Veiculo/PesquisaVeiculosSugeridosHUB", _cargaVeiculosSugeridos, opcoes);
    _gridCargaVeiculosSugeridos.CarregarGrid(function () {
        var modalTitle = $("#divModalVeiculosSugeridosHUB .modal-title");
    });
}

function exibirModalVeiculosSugeridosHUB() {
    clarity("set", "botão_gerar_sugestão", "gerar_sugestao_veiculos");
    LoadCargaVeiculosSugeridos();
    Global.abrirModal('divModalVeiculosSugeridosHUB');
}