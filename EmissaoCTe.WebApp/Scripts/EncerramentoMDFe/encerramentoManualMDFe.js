/**
 * API para encerramento manual
 */
var $EncerramentoChave, $EncerramentoProtocolo, $EncerramentoData, $EncerramentoHora, $EncerramentoEstado, $EncerramentoMunicipio, $EncerramentoSalvar, $Alerta;

$(document).ready(function () {
    $EncerramentoChave = $("#txtEncerramentoChave");
    $EncerramentoProtocolo = $("#txtEncerramentoProtocolo");
    $EncerramentoData = $("#txtEncerramentoData");
    $EncerramentoHora = $("#txtEncerramentoHora");
    $EncerramentoEstado = $("#selEncerramentoEstado");
    $EncerramentoMunicipio = $("#selEncerramentoMunicipio");
    $EncerramentoSalvar = $("#btnSalvarEncerramentoMDFe");
    $Alerta = $("#placeholder-msgEncerramentoMDFe");
})

function HelperEncerramentoManual(MDFE) {
    if (!MDFE) MDFE = {};

    /**
     * Formata os campos
     */
    FormatarCampoDate("txtEncerramentoData");
    $EncerramentoChave.mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $EncerramentoProtocolo.mask("999999999999999");
    $EncerramentoHora.mask("99:99");

    /**
     * Limpa os campos
     */
    LimparCamposEncerramento();

    /**
     * Preenche os valores
     */
    // Chave
    if ("Chave" in MDFE)
        $EncerramentoChave.val(MDFE.Chave).trigger("blur");

    // Protocolo
    if ("Protocolo" in MDFE)
        $EncerramentoProtocolo.val(MDFE.Protocolo);

    // Data
    if ("DataEncerramento" in MDFE)
        $EncerramentoData.val(MDFE.DataEncerramento);
    else
        $EncerramentoData.val(Globalize.format(new Date(), "dd/MM/yyyy"));

    // Hora
    if ("HoraEncerramento" in MDFE)
        $EncerramentoHora.val(MDFE.HoraEncerramento);
    else
        $EncerramentoHora.val(Globalize.format(new Date(), "HH:mm"));

    // Municipio/Estados
    if ("CodigoLocalidadeEncerramento" in MDFE)
        EncerramentoCarregarMunicipioEEstados(MDFE.CodigoLocalidadeEncerramento);
    else
        EncerramentoCarregarEstados(0, EncerramentoCarregarCidades);


    /**
     * Vincula eventos
     */
    $EncerramentoEstado.on("change", EncerramentoCarregarCidades);
    $ModalEncerramento.on('hidden.bs.modal', LimparCamposEncerramento);
    $EncerramentoSalvar.on("click", EncerramentoSalvar);

    /**
     * Abre o modal os valores
     */
    $ModalEncerramento.modal('show');

    /**
     * Metodos Privados
     */
    function EncerramentoCarregarEstados(ufSel, cb) {
        /**
         * Essa funcao busca todos os estados e renderiza no select
         * Apos a renderizacao, e chamado o callback (caso tenha)
         */
        executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
            if (r.Sucesso) {
                RenderizarEncerramentoEstados(r.Objeto, ufSel);
                if ($.isFunction(cb)) cb();
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
    function EncerramentoCarregarCidades(cb) {
        /**
         * Essa funcao busca todos as cidades do estados selecionado e renderiza no select
         * Apos a renderizacao, e chamado o callback (caso tenha)
         */
        var ufSel = $EncerramentoEstado.val();
        executarRest("/Localidade/BuscarPorUF?callback=?", { UF: ufSel }, function (r) {
            if (r.Sucesso) {
                RenderizarEncerramentoCidades(r.Objeto);
                if ($.isFunction(cb)) cb();
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
    function EncerramentoCarregarMunicipioEEstados(codigoMunicipio) {
        /**
         * Essa funcao busca todos os estados da cidade passada por parametros e renderiza no select
         * A diferenca dessa funcao para as outras, e que primeiro busca os estados do codigo da cidade
         * Para entao buscar as cidades
         * Por fim, marcar como selecionado a cidade passadad
         */
        // Busca o estados da cidade
        executarRest("/Localidade/BuscarPorCodigo?callback=?", { Codigo: codigoMunicipio }, function (r) {
            if (r.Sucesso) {
                // Busca os estados e passa o estado da cidade
                EncerramentoCarregarEstados(r.Objeto.UF, function () {
                    // Busca as cidades depois que os estados forem renderizados
                    EncerramentoCarregarCidades(function () {
                        // Apos as cidades serem renderizadas, e selecionado o municipio
                        $EncerramentoMunicipio.val(codigoMunicipio);
                    });
                });
            } else {
                jAlert(r.Erro, "Atenção");
            }
        });
    }
    function RenderizarEncerramentoEstados(ufs, ufSel) {
        /**
         * Essa funcao recebe os estados e renderiza no select
         * Quando ha um option com o valor setado (ufSel) marca como selected
         */
        var $options = [];
        for (var uf in ufs) {
            uf = ufs[uf];
            var selected = uf.Sigla == ufSel ? ' selected="selected" ' : "";
            $options.push('<option value="' + uf.Sigla + '"' + selected + '>' + uf.Nome + '</option>')
        }

        $EncerramentoEstado.html($options.join());
    }
    function RenderizarEncerramentoCidades(cidades, cidadeSel) {
        /**
         * Essa funcao recebe as cidades e renderiza no select
         * Quando ha um option com o valor setado (cidadeSel) marca como selected
         */
        var $options = [];
        for (var cidade in cidades) {
            cidade = cidades[cidade];
            var selected = cidade.Codigo == cidadeSel ? ' selected="selected" ' : "";
            $options.push('<option value="' + cidade.Codigo + '"' + selected + '>' + cidade.Descricao + '</option>')
        }

        $EncerramentoMunicipio.html($options.join());
    }


    /**
     * Limpa campos do encerramento
     */
    function LimparCamposEncerramento() {
        // Remove os valores
        $EncerramentoChave.val("");
        $EncerramentoProtocolo.val("");
        $EncerramentoData.val("");
        $EncerramentoHora.val("");
        $EncerramentoEstado.val($EncerramentoEstado.find("option:first").val());
        $EncerramentoMunicipio.html("");

        // Tira os erros dos campos
        CampoSemErro($EncerramentoChave);
        CampoSemErro($EncerramentoProtocolo);
        CampoSemErro($EncerramentoData);
        CampoSemErro($EncerramentoHora);
        CampoSemErro($EncerramentoEstado);
        CampoSemErro($EncerramentoMunicipio);

        // Remove alertas
        $Alerta.html("");
    }

    /**
     * Salvar encerramento
     */
    function EncerramentoSalvar() {
        if (!EncerramentoValidar()) {
            return ExibirMensagemErro("Todos campos são obrigatórios.", "Atenção", "placeholder-msgEncerramentoMDFe");
        }
        EncerramentoValidarChave(function (err) {
            if (err != "") return ExibirMensagemErro(err, "Atenção", "placeholder-msgEncerramentoMDFe");

            // Cria o objeto de encerramento
            var EnverramentoMDFe = {
                Chave: $EncerramentoChave.val().replace(/[^0-9]/g, ''),
                Protocolo: $EncerramentoProtocolo.val().replace(/[^0-9]/g, ''),
                DataEncerramento: $EncerramentoData.val(),
                HoraEncerramento: $EncerramentoHora.val(),
                CodigoLocalidadeEncerramento: $EncerramentoMunicipio.val()
            };

            // Executa encerramento
            executarRest("/EncerramentoManualMDFe/EncerrarMDFeMultiCTe?callback=?", EnverramentoMDFe, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("MDF-e encerrado com sucesso.", "Sucesso!");
                    $ModalEncerramento.modal('hide');
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        });
    }

    /**
     * Validar chave (se nao existe no sistem e se ela e valida)
     */
    function EncerramentoValidarChave( cb ) {
        executarRest("/EncerramentoManualMDFe/ValidarChave?callback=?", { ChaveMDFe: $EncerramentoChave.val() }, function (r) {
            if (r.Objeto) 
                cb(r.Objeto);
            else 
                jAlert(r.Erro, "Atenção");
        });
    }

    /**
     * Validar campos encerramento
     */
    function EncerramentoValidar() {
        var valido = true;

        if ($EncerramentoChave.val().replace(/[^0-9]/g, '').length != 44) {
            CampoComErro($EncerramentoChave);
            valido = false;
        } else {
            CampoSemErro($EncerramentoChave);
        }

        if ($EncerramentoProtocolo.val().replace(/[^0-9]/g, '').length != 15) {
            CampoComErro($EncerramentoProtocolo);
            valido = false;
        } else {
            CampoSemErro($EncerramentoProtocolo);
        }

        if ($EncerramentoData.val().replace(/[^0-9]/g, '').length != 8) {
            CampoComErro($EncerramentoData);
            valido = false;
        } else {
            CampoSemErro($EncerramentoData);
        }

        if ($EncerramentoHora.val().replace(/[^0-9]/g, '').length != 4) {
            CampoComErro($EncerramentoHora);
            valido = false;
        } else {
            CampoSemErro($EncerramentoHora);
        }

        if (parseInt($EncerramentoEstado.val()) <= 0) {
            CampoComErro($EncerramentoEstado);
            valido = false;
        } else {
            CampoSemErro($EncerramentoEstado);
        }

        if (parseInt($EncerramentoMunicipio.val()) <= 0) {
            CampoComErro($EncerramentoMunicipio);
            valido = false;
        } else {
            CampoSemErro($EncerramentoMunicipio);
        }

        return valido;
    }
}