/**
 * Objeto global para acesso
 */
var $DataInicial, $DataFinal, $Chave, $Protocolo, $NovoEncerramento, $ModalEncerramento, $Consular;

$(document).ready(function () {
    /**
     * Cache dos campos
     */
    $DataInicial = $("#txtDataEncerramentoInicial");
    $DataFinal = $("#txtDataEncerramentoFinal");
    $Chave = $("#txtChaveMDFe"); 
    $Protocolo = $("#txtProtocolo");
    $NovoEncerramento = $("#btnNovoEncerramentoMDFe");
    $ModalEncerramento = $("#divEncerramentoMDFe");
    $Consular = $("#btnConsultarEncerramentos")

    /**
     * Mascaras dos campos
     */
    FormatarCampoDate("txtDataEncerramentoInicial");
    FormatarCampoDate("txtDataEncerramentoFinal");
    $Chave.mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");

    /**
     * Eventos
     */
    $NovoEncerramento.on("click", NovoEncerramento);
    $Consular.click(AtualizarGridEncerramentos);

    AtualizarGridEncerramentos();
});


/**
 * Chama o Helper de ciracao de encerramento
 */
function NovoEncerramento(){ 
    HelperEncerramentoManual();
    /*EXEMPLO DE USO
    HelperEncerramentoManual({
        Chave: "4217 0213 4960 2300 0180 5800 1000 0005 9510 0000 5955",
        Protocolo: "942170000002915",
        CodigoLocalidadeEncerramento: "71986"
    });*/
}

/**
 * Atualzia o grid de encerramentos
 */
function AtualizarGridEncerramentos() {
    dados = {
        inicioRegistros: 0,
        DataInicial: $DataInicial.val(),
        DataFinal: $DataFinal.val(),
        ChaveMDFe: $Chave.val().replace(/[^0-9]/g, ''),
    };

    CriarGridView("/EncerramentoManualMDFe/ConsultarMultiCTe?callback=?", dados, "tbl_encerramentos_table", "tbl_encerramentos", "tbl_paginacao_encerramentos", 0, [0], null, [0]);
}