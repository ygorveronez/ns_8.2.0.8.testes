
function obterTopVeiculosAtrazados(callback) {
    _ControlarManualmenteProgresse = _controleAutomatico;
    executarReST("Indicador/ObterVeiculosAtrazados", null, function (arg) {
        ExibirSlide("VeiculosAtrasados");
        $("#tituloVeiculo").text("Veículos Atrasados");

        $("#divVeiculosAtrazados").show();
        $("#divVeiculosEmCarregamento").hide();
        $("#divVeiculosAgFaturamento").hide();
        _ControlarManualmenteProgresse = false;
        _controleAutomatico = true;
        if (arg.Success) {
            if (arg.Data) {
                var header = [{ data: "Codigo", visible: false },
                { data: "NumeroCarga", title: "Carga", width: "8%", className: 'text-align-center' },
                { data: "Centro", title: "Centro", width: "25%" },
                { data: "Veiculo", title: "Veiculo", width: "15%" },
                { data: "Transportador", title: "Transportador", width: "20%" },
                { data: "Motorista", title: "Motorista", width: "15%" },
                { data: "DescricaoSituacao", title: "Situação", width: "10%", className: 'text-align-center' },
                { data: "DataCarregamento", title: "Data Programada", width: "10%", className: 'text-align-center' },
                { data: "TempoAtraso", title: "Atrasado", width: "5%", className: 'text-align-center' }
                ];
                $('#tblVeiculosAtrazados').html("");
                var grid3 = new BasicDataTable("tblVeiculosAtrazados", header, null, { column: 8, dir: orderDir.desc }, null, 10);
                grid3.CarregarGrid(arg.Data);

                callback();
            } else {
                callback();
            }
        } else {
            callback();
        }
    });
}


function obterTopVeiculosEmCarregamento(callback) {
    _ControlarManualmenteProgresse = _controleAutomatico;
    executarReST("Indicador/ObterVeiculosEmCarregamento", null, function (arg) {
        $("#tituloVeiculo").text("Veículos em Carregamento");
        ExibirSlide("VeiculosAtrasados");

        $("#divVeiculosAtrazados").hide();
        $("#divVeiculosEmCarregamento").show();
        $("#divVeiculosAgFaturamento").hide();
        _ControlarManualmenteProgresse = false;
        _controleAutomatico = true;
        if (arg.Success) {
            if (arg.Data) {
                var header = [{ data: "Codigo", visible: false },
                { data: "NumeroCarga", title: "Carga", width: "8%", className: 'text-align-center' },
                { data: "Centro", title: "Centro", width: "25%" },
                { data: "Veiculo", title: "Veiculo", width: "15%" },
                { data: "Transportador", title: "Transportador", width: "20%" },
                { data: "Motorista", title: "Motorista", width: "15%" },
                //{ data: "DescricaoSituacao", title: "Situação", width: "15%" },
                { data: "DataEntrada", title: "Entrada", width: "10%", className: 'text-align-center' },
                { data: "TempoEmCarregamento", title: "Tempo", width: "5%", className: 'text-align-center' }
                ];
                $('#tblVeiculosAtrazados').html("");
                var grid2 = new BasicDataTable("tblVeiculosEmCarregamento", header, null, { column: 7, dir: orderDir.desc }, null, 10);
                grid2.CarregarGrid(arg.Data);

                callback();
            } else {
                callback();
            }
        } else {
            callback();
        }
    });
}

function obterTopVeiculosAgFaturamento(callback) {
    _ControlarManualmenteProgresse = _controleAutomatico;
    executarReST("Indicador/ObterVeiculosAgFaturamento", null, function (arg) {
        ExibirSlide("VeiculosAtrasados");
        $("#tituloVeiculo").text("Veículos em Faturamento");

        $("#divVeiculosAtrazados").hide();
        $("#divVeiculosEmCarregamento").hide();
        $("#divVeiculosAgFaturamento").show();
        _ControlarManualmenteProgresse = false;
        _controleAutomatico = true;
        if (arg.Success) {
            if (arg.Data) {
                var header = [{ data: "Codigo", visible: false },
                { data: "NumeroCarga", title: "Carga", width: "8%", className: 'text-align-center' },
                { data: "Centro", title: "Centro", width: "26%" },
                { data: "Veiculo", title: "Veiculo", width: "15%" },
                { data: "Transportador", title: "Transportador", width: "20%" },
                { data: "Motorista", title: "Motorista", width: "15%" },
                //{ data: "DescricaoSituacao", title: "Situação", width: "15%" },
                { data: "DataCarregamento", title: "Carregado", width: "10%", className: 'text-align-center' },
                { data: "TempoEmFaturamento", title: "Tempo", width: "5%", className: 'text-align-center' }
                ];
                $('#tblVeiculosAtrazados').html("");
                var grid = new BasicDataTable("tblVeiculosAgFaturamento", header, null, { column: 7, dir: orderDir.desc }, null, 10);
                grid.CarregarGrid(arg.Data);

                callback();
            } else {
                callback();
            }
        } else {
            callback();
        }
    });
}



