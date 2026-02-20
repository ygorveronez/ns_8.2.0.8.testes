
function buscarCargasAtrasadas(callback) {
    _ControlarManualmenteProgresse = _controleAutomatico;
    executarReST("Indicador/ObterCargasAtrazadas", null, function (arg) {
        _ControlarManualmenteProgresse = false;
        if (arg.Success) {
            ExibirSlide('knockoutIndicadoresQuantidadeCarga', 'knockoutCargasAtrazadas');
            if (arg.Success) {
                renderizarCargaAtrazada(arg.Data);
            }
        }
        callback();
    });
}

function renderizarCargaAtrazada(cargas) {
    _ControlarManualmenteProgresse = false;
    _controleAutomatico = true;
    var header = [
        { data: "NumeroCarga", title: "Carga", width: "10%", className: 'text-align-center' },
        { data: "DiasAtraso", title: "Atraso (dias)", width: "10%", className: 'text-align-center' },
        { data: "Filial", title: "Filial", width: "25%" },
        { data: "Destinatarios", title: "Destinatários", width: "22%" },
        { data: "Destinos", title: "Destinos", width: "13%" },
        { data: "Empresa", title: "Transportador", width: "25%" },
        { data: "Veiculo", title: "Veiculo", width: "10%" },
        { data: "Motorista", title: "Motorista", width: "20%" },
        { data: "SituacaoJanela", title: "Situação", width: "15%", className: 'text-align-center' }
    ];
    $('#tblCargasAtrasadas').html("");
    var grid = new BasicDataTable("tblCargasAtrasadas", header, null, { column: 1, dir: orderDir.desc }, null, 10);
    grid.CarregarGrid(cargas);
};