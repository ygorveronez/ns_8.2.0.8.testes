$(document).ready(function () {
    FormatarCampoDate("txtFiltroDataInicial");
    FormatarCampoDate("txtFiltroDataFinal");

    $("#txtFiltroNumeroInical, #txtFiltroNumeroFinal").mask("9?999999");

    var today = new Date();
    var date = new Date(today);
    date.setDate(today.getDate() - 1);
    $("#txtFiltroDataInicial").val(Globalize.format(date, "dd/MM/yyyy"));
    $("#txtFiltroDataFinal").val(Globalize.format(today, "dd/MM/yyyy"));

    $("#btnConsultarIntegracoes").click(AtualizarGrid);

    AtualizarGrid();
});

function DadosFiltro() {
    return {
        inicioRegistros: 0,
        NumeroInicial: $("#txtFiltroNumeroInical").val(),
        NumeroFinal: $("#txtFiltroNumeroFinal").val(),
        DataInicial: $("#txtFiltroDataInicial").val(),
        DataFinal: $("#txtFiltroDataFinal").val(),
        TipoDocumento: $("#selFiltroTipoDocumento").val(),
        StatusEnvio: $("#selFiltroStatusEnvio").val(),
        StatusConsulta: $("#selFiltroStatusConsulta").val(),
        Identificador: $("#txtFiltroIdentificador").val(),
        NumeroNota: $("#txtFiltroNumeroNota").val()        
    };
}
function AtualizarGrid() {
    var dados = DadosFiltro();

    var opcoes = new Array();
    opcoes.push({ Descricao: "Enviar Manualmente", Evento: EnviarManualmente });
    opcoes.push({ Descricao: "Consultar Manualmente", Evento: ConsultarManualmente });
    opcoes.push({ Descricao: "Log Integrações", Evento: LogIntegracoes });

    CriarGridView("/LSTranslogIntegracao/Consultar?callback=?", dados, "tbl_integracoes_table", "tbl_integracoes", "tbl_paginacao_integracoes", opcoes, [0]);
}