var cteSelecionado = null;

$(document).ready(function () {

    FormatarCampoDate("txtDataEmissaoInicialCTeConsulta");
    FormatarCampoDate("txtDataEmissaoFinalCTeConsulta");

    $("#txtNumeroInicialCTeConsulta").mask("9?99999999999");
    $("#txtNumeroFinalCTeConsulta").mask("9?99999999999");
    $("#txtPlacaVeiculoCTeConsulta").mask("*******");
    $("#txtCPFMotoristaCTeConsulta").mask("99999999999");

    $("#btnSelecionarTodosOsCTes").click(function () {
        SelecionarTodosOsCTes();
    });


    $("#btnGerarMDFeCTesGerados").click(function () {
        GerarMDFeSelecaoCTes();
    });

    $("#btnCancelarGerarMDFe").click(function () {
        FecharTelaGerarMDFe();
    });

    $("#btnConsultarMDFe").click(function () {
        AtualizarMDFeEmitidos();
    });

    $("#btnGerarNovoMDFe").click(function () {
        FecharTelaConsultaMDFe();
        AbrirTelaGerarMDFe(cteSelecionado);
    });

    $("#btnSalvarCancelamentoMDFe").click(function () {
        FinalizarCancelamentoMDFe();
    });

    $("#divCancelamentoMDFe").on('modal.bs.hidden', FecharTelaCancelamentoMDFe);
});


function AbrirTelaGerarMDFe(cte) {
    LimparCamposConsultaCTe();

    var dataInicial = cte.data.DataEmissao;
    dataInicial = dataInicial.substring(0, 10);
    var dataFinal = new Date();

    $("#txtDataEmissaoInicialCTeConsulta").val(Globalize.format(dataInicial, "dd/MM/yyyy"));
    $("#txtPlacaVeiculoCTeConsulta").val(cte.data.Placa);    

    cteSelecionado = null;
    cteSelecionado = cte;

    $("#hddCodigoCTE").val(cteSelecionado.data.Codigo);
    $("#divGerarMDFe.modal-footer").show();
    CarregarTelaGerarMDFe(RetornoConsultaCTeAvancado);
    AdicionarCTe(cteSelecionado);
}

function CarregarTelaGerarMDFe(callback) {
    $("body").data("ctesSelecionadosConsultaAvancada", null);
    $("#containerCTesSelecionados").html("");

    ConsultarCTesMDFe(callback);

    $("#divGerarMDFe").modal({ keyboard: false, backdrop: 'static' });

    $("#btnBuscarCTesConsulta").off();

    $("#btnBuscarCTesConsulta").on("click", function () { ConsultarCTesMDFe(callback) });
}

function AdicionarCTe(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");

    var dados = {
        CodigoCTe: cte.data.Codigo,
        CTesSemMDFe: true
    };

    executarRest("/ManifestoEletronicoDeDocumentosFiscais/ConsultarCTesParaEmissao?callback=?", dados, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.length > 0) {

                var ctesSelecionados = new Array();

                for (var i = 0; i < r.Objeto.length; i++) {
                    if (r.Objeto[i].Codigo == cte.data.Codigo) {
                        ctesSelecionados.push({ Codigo: r.Objeto[i].Codigo });
                        $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);
                        AdicionarTag(cte.data);
                        $("body").data("ufConsultaCTe", r.Objeto[i].UFDescarregamento);
                        $("body").data("ufCarregamentoConsultaCTe", r.Objeto[i].UFCarregamento);
                    }
                }

                ConsultarCTesMDFe(RetornoConsultaCTeAvancado);
                $("#btnSelecionarTodosOsCTes").removeClass("disabled");
                
            }
        } else {
            jAlert(r.Erro, "Atenção!");
        }
    });
}


function RetornoConsultaCTeAvancado(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");

    if (ctesSelecionados.length <= 0) {
        cteSelecionado = null;
        cteSelecionado = cte;

        $("body").data("ufConsultaCTe", cte.data.UFDescarregamento);
        $("body").data("ufCarregamentoConsultaCTe", cte.data.UFCarregamento);
        ConsultarCTesMDFe(RetornoConsultaCTeAvancado);
        $("#btnSelecionarTodosOsCTes").removeClass("disabled");
    }

    for (var i = 0; i < ctesSelecionados.length; i++) {
        if (ctesSelecionados[i].Codigo == cte.data.Codigo)
            return;
    }

    ctesSelecionados.push({ Codigo: cte.data.Codigo });

    $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

    AdicionarTag(cte.data);
}

function SetarUFConsultaAvancadaCTe(uf) {
    $("body").data("ufConsultaCTe", uf);
}

function AdicionarTag(cte) {
    var tag = document.createElement("li");
    tag.className = "tag-item tag-item-delete-experience";
    tag.id = "cteSelecionado_" + cte.Codigo;

    var container = document.createElement("span");
    container.className = "tag-container tag-container-delete-experience";

    var descricao = document.createElement("span");
    descricao.className = "tag-box tag-box-delete-experience";
    if (cte.ValorFrete != null)
        descricao.innerHTML = "<b>" + cte.Numero + "</b> | " + cte.ValorFrete;
    else
        descricao.innerHTML = "<b>" + cte.Numero + "</b> | " + cte.Valor;

    var opcaoExcluir = document.createElement("span");
    opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
    opcaoExcluir.innerHTML = "&nbsp;";
    opcaoExcluir.onclick = function () { RemoverCTeSelecionado(cte) };

    container.appendChild(descricao);
    container.appendChild(opcaoExcluir);

    tag.appendChild(container);

    document.getElementById("containerCTesSelecionados").appendChild(tag);
}

function RemoverCTeSelecionado(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");   

    for (var i = 0; i < ctesSelecionados.length; i++) {
        if (ctesSelecionados[i].Codigo == cte.Codigo)
            ctesSelecionados.splice(i, 1);
    }

    $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

    $("#cteSelecionado_" + cte.Codigo).remove();

    if (ctesSelecionados.length <= 0) {
        $("body").data("ufConsultaCTe", null);
        $("body").data("ufCarregamentoConsultaCTe", null);
        ConsultarCTesMDFe(RetornoConsultaCTeAvancado);
    }
}

function SelecionarTodosOsCTes() {
    var dados = {
        CodigoMunicipio: $("body").data("municipioDescarregamentoDocumento") != null ? $("body").data("municipioDescarregamentoDocumento").CodigoMunicipio : 0,
        UFDescarregamento: $("body").data("ufConsultaCTe") != null ? $("body").data("ufConsultaCTe") : "",
        UFCarregamento: $("body").data("ufCarregamentoConsultaCTe") != null ? $("body").data("ufCarregamentoConsultaCTe") : "",
        DataInicial: $("#txtDataEmissaoInicialCTeConsulta").val(),
        DataFinal: $("#txtDataEmissaoFinalCTeConsulta").val(),
        NumeroInicial: $("#txtNumeroInicialCTeConsulta").val(),
        NumeroFinal: $("#txtNumeroFinalCTeConsulta").val(),
        Placa: $("#txtPlacaVeiculoCTeConsulta").val(),
        NomeMotorista: $("#txtNomeMotoristaCTeConsulta").val(),
        CPFMotorista: $("#txtCPFMotoristaCTeConsulta").val()
    };

    executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterTodosOsCTesParaEmissao?callback=?", dados, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.length > 0) {

                $("body").data("ctesSelecionadosConsultaAvancada", null);
                $("#containerCTesSelecionados").html("");

                var ctesSelecionados = new Array();

                for (var i = 0; i < r.Objeto.length; i++) {
                    AdicionarTag(r.Objeto[i]);
                    ctesSelecionados.push({ Codigo: r.Objeto[i].Codigo });
                }

                $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

            }
        } else {
            jAlert(r.Erro, "Atenção!");
        }
    });
}

function GerarMDFeSelecaoCTes() {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");

    var listaCTes = new Array();

    for (var i = 0; i < ctesSelecionados.length; i++)
        listaCTes.push(ctesSelecionados[i].Codigo);
    
    if (ctesSelecionados.length > 0) {
        executarRest("/ConhecimentoDeTransporteEletronico/GerarMDFeListaCTes?callback=?", { ListaCTes: JSON.stringify(listaCTes) }, function (r) {
            if (r.Sucesso) {
                const sucessoMessage = "O MDF-e foi gerado com sucesso! Para Emitir é necessário o preenchimento das <strong>Informações de Pagamento do Frete</strong> através da opção <strong>Abrir MDF-e</strong>.";
                const finalizarProcesso = () => {
                    FecharTelaGerarMDFe();
                    AbrirTelaConsultaMDFe(cteSelecionado);
                };

                if (!r.Objeto.docImportadoAoIntegrador) {
                    jAlert(sucessoMessage, "Sucesso", function (r) {
                        finalizarProcesso();
                    });
                } else {
                    finalizarProcesso();
                }
            } else {
                ExibirMensagemErro(r.Erro, "Falha na Geração do MDFe", "placeholder-msgConsultaCTes");
            }
        });
    } else {
        FecharTelaGerarMDFe();
    }
}

function FecharTelaGerarMDFe() {
    $("#divGerarMDFe").modal('hide');
    LimparCamposConsultaCTe();
    VoltarAoTopoDaTela();
}

function ConsultarCTesMDFe(callback) {
    var dados = {
        CodigoMunicipio: $("body").data("municipioDescarregamentoDocumento") != null ? $("body").data("municipioDescarregamentoDocumento").CodigoMunicipio : 0,
        UFDescarregamento: $("body").data("ufConsultaCTe") != null ? $("body").data("ufConsultaCTe") : "",
        UFCarregamento: $("body").data("ufCarregamentoConsultaCTe") != null ? $("body").data("ufCarregamentoConsultaCTe") : "",
        DataInicial: $("#txtDataEmissaoInicialCTeConsulta").val(),
        DataFinal: $("#txtDataEmissaoFinalCTeConsulta").val(),
        NumeroInicial: $("#txtNumeroInicialCTeConsulta").val(),
        NumeroFinal: $("#txtNumeroFinalCTeConsulta").val(),
        Placa: $("#txtPlacaVeiculoCTeConsulta").val(),
        NomeMotorista: $("#txtNomeMotoristaCTeConsulta").val(),
        CPFMotorista: $("#txtCPFMotoristaCTeConsulta").val(),
        CTesSemMDFe: true,
        inicioRegistros: 0
    };

    CriarGridView("/ManifestoEletronicoDeDocumentosFiscais/ConsultarCTesParaEmissao?callback=?", dados, "tbl_ctes_consulta_table", "tbl_ctes_consulta", "tbl_ctes_consulta_paginacao", [{ Descricao: "Selecionar", Evento: callback }], [0, 1, 2, 3, 4, 5], null);
}


function LimparCamposConsultaCTe() {
    $("#btnSelecionarTodosOsCTes").addClass("disabled");
    $("#divGerarMDFe.modal-footer").hide();
    $("body").data("ufConsultaCTe", null);
    $("body").data("ufCarregamentoConsultaCTe", null);
    $("#containerCTesSelecionados").html("");
    $("body").data("ctesSelecionadosConsultaAvancada", null);
    $("#txtDataEmissaoInicialCTeConsulta").val("");
    $("#txtDataEmissaoFinalCTeConsulta").val("");
    $("#txtNumeroInicialCTeConsulta").val("");
    $("#txtNumeroFinalCTeConsulta").val("");
    $("body").data("ctes", null);    
    $("#hddCodigoCTE").val("");
}


//Tela de consulta MDFe
function AbrirTelaConsultaMDFe(cte) {
    cteSelecionado = null;
    cteSelecionado = cte;

    if (cteSelecionado.data.Status == "A" || cteSelecionado.data.Status == null) {
        $("#hddCodigoCTE").val(cteSelecionado.data.Codigo);
        AtualizarMDFeEmitidos();
        $("#divConsultarMDFe").modal({ keyboard: false, backdrop: 'static' });
    }
    else {
        jAlert("Somente é possível gerar MDF-e de CT-e Autorizado", "Atenção!");
    }
}

function FecharTelaConsultaMDFe() {
    $("#divConsultarMDFe").modal("hide");
    VoltarAoTopoDaTela();
}

function AtualizarMDFeEmitidos() {
    var dados = {
        CodigoCTe: $("#hddCodigoCTE").val()
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "DAMDFE", Evento: DownloadPDFMDFe });
    opcoes.push({ Descricao: "XML Autorização", Evento: DownloadXMLMDFe });
    opcoes.push({ Descricao: "Abrir MDF-e", Evento: AbrirMDFe });
    opcoes.push({ Descricao: "Cancelar MDF-e", Evento: CancelarMDFe });
    opcoes.push({ Descricao: "Emitir MDF-e", Evento: EmitirMDFe });
    opcoes.push({ Descricao: "Retorno", Evento: RetornoMDFe });

    CriarGridView("/ConhecimentoDeTransporteEletronico/ConsultarMDFesEmitidos?callback=?", dados, "tbl_paginacao_mdfe_cte_table", "tbl_mdfe_cte", "tbl_paginacao_mdfe_cte", opcoes, [0, 1, 2, 3], null);

    jQuery("#btnConsultarMDFe").blur();
}

function DownloadPDFMDFe(mdfe) {
    executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadDAMDFE", { codigoMDFe: mdfe.data.Codigo });
}

function DownloadXMLMDFe(mdfe) {
    executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLAutorizacao", { codigoMDFe: mdfe.data.Codigo });
}

function AbrirMDFe(json) {
    mdfe = json.data;
    if (mdfe.Status == 0) { // Em Digitação
        var url = "EmissaoMDFe.aspx?mdfe=" + mdfe.Numero + "&serie=" + mdfe.Serie;
        window.open(url, '_blank');
    } else {
        mdfe = json.data;
        window.location.href = "EmissaoMDFe.aspx?mdfe=" + mdfe.Numero + "&serie=" + mdfe.Serie;
    }
}

function RetornoMDFe(mdfe) {
    jAlert(mdfe.data.MensagemRetornoSefaz, "Mensagem Retorno MDF-e");
}

function CancelarMDFe(mdfe) {
    if (mdfe.data.Status == 3 || mdfe.data.Status == 6) {
        AbrirTelaCancelamentoMDFe();
        $("body").data("mdfeCancelamento", mdfe.data);
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para cancelar o mesmo.", "Atenção!", "messages-placeholderMDFesEmitidos");
    }
}

function EmitirMDFe(mdfe) {
    if (mdfe.data.Status == 0) {
        ExibirMensagemAlerta("Por motivo de alterações referentes a <strong>Nota Técnica MDF-e 2025.001</strong>, é necessário revisar as Informações de Pagamento do Frete utilizando a opção <strong>Abrir MDF-e</strong>.", "Atenção!", "messages-placeholderMDFesEmitidos");
    } else {
        var envio = {
            Codigo: mdfe.data.Codigo,
            FormaEmissao: 1
        };

        executarRest("/ManifestoEletronicoDeDocumentosFiscais/Emitir?callback=?", envio, function (r) {
            AtualizarMDFeEmitidos();
            if (!r.Sucesso) {
                jAlert(r.Erro, "Atenção!");
            }
        });
    }
}

function AbrirTelaCancelamentoMDFe() {
    $("#divCancelamentoMDFe").modal({ keyboard: false, backdrop: 'static' });
    $("#txtJustificativaCancelamentoMDFe").val("");
    $("#txtDataCancelamento").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
    $("body").data("mdfeCancelamento", null);
}

function FinalizarCancelamentoMDFe() {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.ExibirCobrancaCancelamento != null && configuracaoEmpresa.ExibirCobrancaCancelamento == true) {
        if ($("#selCobrarCancelamentoMDFe").val() == "") {
            CampoComErro("#selCobrarCancelamentoMDFe");
            return;
        }
    }
    else
        $("#selCobrarCancelamentoMDFe").val("Sim");

    var justificativa = $("#txtJustificativaCancelamentoMDFe").val();
    if (justificativa.length > 20 && justificativa.length < 255) {
        executarRest("/ManifestoEletronicoDeDocumentosFiscais/Cancelar?callback=?", {
            Justificativa: justificativa,
            CodigoMDFe: $("body").data("mdfeCancelamento") != null ? $("body").data("mdfeCancelamento").Codigo : 0,
            DataCancelamento: $("#txtDataCancelamento").val(),
            CobrarCancelamento: $("#selCobrarCancelamentoMDFe").val()
        }, function (r) {
            if (r.Sucesso) {
                jAlert("O MDF-e está em processo de cancelamento.", "Atenção", function () {
                    $("#divCancelamentoMDFe").modal('hide');
                    AtualizarMDFeEmitidos();
                });
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgCancelamentoMDFe");
            }
        });
    } else {
        ExibirMensagemAlerta("A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.", "Atenção!", "placeholder-msgCancelamentoMDFe");
    }
}

function FecharTelaCancelamentoMDFe() {
    $("#txtJustificativaCancelamentoMDFe").val("");
    $("#txtDataCancelamento").val("");
    $("body").data("mdfeCancelamento", null);
}