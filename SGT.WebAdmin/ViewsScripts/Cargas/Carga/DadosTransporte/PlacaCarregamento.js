//*******MÉTODOS*******
function loadPlacaCarregamento(knoutCarga) {
    executarReST("CargaDadosTransporte/ObterVeiculosVinculadosSelecionados", { Codigo: knoutCarga.Codigo.val() }, function (arg) {
        if (arg.Success) {
            var selecionados = new Array();

            if (arg.Data != null) {
                $.each(arg.Data, function (i, obj) {
                    var objCTe = { DT_RowId: obj.Codigo, Codigo: obj.Codigo };
                    selecionados.push(objCTe);
                });
            }
            var editarColuna = {
                permite: true,
                atualizarRow: true,
            };

            var multiplaEscolha = {
                basicGrid: null,
                eventos: {},
                selecionados: selecionados,
                naoSelecionados: new Array(),
                somenteLeitura: false,
            };

            _gridPlacaCarregamento = new GridView(knoutCarga.InformarPlacaCarregamento.idGrid, "CargaDadosTransporte/ObterVeiculosVinculados", knoutCarga, null, null, null, null, null, null, multiplaEscolha, null, editarColuna);

            _gridPlacaCarregamento.CarregarGrid();


        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}



