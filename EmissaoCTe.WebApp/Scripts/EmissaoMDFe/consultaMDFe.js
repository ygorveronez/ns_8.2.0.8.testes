var _permitirMultiplasParcelas = false;

$(document).ready(function () {
    $("#txtCPFMotoristaInclusao").mask("99999999999");
    CarregarConsultaDeMotoristas("btnBuscarMotoristaInclusao", "btnBuscarMotoristaInclusao", RetornoConsultaMotoristaInclusao, true, false);

    FormatarCampoDateTime("txtDataEncerramento");
    FormatarCampoDateTime("txtDataEventoEncerramento");
    FormatarCampoDateTime("txtDataCancelamento");
    FormatarCampoDateTime("txtDataEventoInclusao");    

    FormatarCampoDate("txtDataEmissaoInicialMDFeFiltro");
    FormatarCampoDate("txtDataEmissaoFinalMDFeFiltro");

    $("#txtPlacaVeiculoFiltro").mask("*******");
    $("#txtPlacaReboqueFiltro").mask("*******");

    FormatarCampoDate("txtVencimentoDaParcela");

    $("#txtNumeroCTe").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });
    $("#txtNumeroInicialMDFeFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });
    $("#txtNumeroFinalMDFeFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });

    // Coloca filtro de data antes de buscar os MDFes
    var today = new Date();
    var yesterday = new Date(today);
    var tomorrow = new Date(today);
    yesterday.setDate(today.getDate() - 1);
    tomorrow.setDate(today.getDate() + 1);
    
    $("#txtDataEmissaoInicialMDFeFiltro").val(Globalize.format(yesterday, "dd/MM/yyyy"));
    $("#txtDataEmissaoFinalMDFeFiltro").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

    $("#btnConsultarMDFe").click(function () {
        ConsultarMDFes();
    });

    $("#btnSalvarCancelamentoMDFe").click(function () {
        FinalizarCancelamentoMDFe();
    });

    $("#btnCancelarCancelamentoMDFe").click(function () {
        FecharTelaCancelamentoMDFe();
    });

    $("#btnSalvarEncerramentoMDFe").click(function () {
        FinalizarEncerramentoMDFe();
    });

    $("#btnCancelarEncerramentoMDFe").click(function () {
        FecharTelaEncerramentoMDFe();
    });

    $("#btnSalvarInclusaoMotorista").click(function () {
        SolicitarInclusaoMotorista();
    });

    $("#btnCancelarInclusaoMotorista").click(function () {
        FecharTelaInclusaoMotorista();
    });

});

function RetornoConsultaMotoristaInclusao(motorista) {
    $("#txtCPFMotoristaInclusao").val(motorista.CPFCNPJ);
    $("#txtNomeMotoristaInclusao").val(motorista.Nome);
}

function VoltarAoTopoDaTela() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
}

function ConsultarMDFes() {

    var codigoMDFe = GetUrlParam("x");

    var dados = {
        CodigoMDFe: codigoMDFe,
        DataEmissaoInicial: $("#txtDataEmissaoInicialMDFeFiltro").val(),
        DataEmissaoFinal: $("#txtDataEmissaoFinalMDFeFiltro").val(),
        NumeroInicial: $("#txtNumeroInicialMDFeFiltro").val(),
        NumeroFinal: $("#txtNumeroFinalMDFeFiltro").val(),
        NumeroCTe: $("#txtNumeroCTe").val(),
        Serie: $("#selSerieMDFeFiltro").val(),
        Status: $("#selStatusMDFeFiltro").val(),
        UFCarregamento: $("#selUFCargaFiltro").val(),
        UFDescarregamento: $("#selUFDescargaFiltro").val(),
        Placa: $("#txtPlacaVeiculoFiltro").val(),
        Reboque: $("#txtPlacaReboqueFiltro").val(),
        inicioRegistros: 0,
        fimRegistros: 20
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Editar", Evento: EditarMDFe });
    opcoes.push({ Descricao: "Emitir", Evento: EmitirMDFe });
    opcoes.push({ Descricao: "Duplicar", Evento: DuplicarMDFe });
    opcoes.push({ Descricao: "Cancelar", Evento: CancelarMDFe });
    opcoes.push({ Descricao: "Encerrar", Evento: EncerrarMDFe });
    opcoes.push({ Descricao: "Incluir Motorista", Evento: IncluirMotorista });
    opcoes.push({ Descricao: "DAMDFE", Evento: BaixarDAMDFE });
    opcoes.push({ Descricao: "XML Autorização", Evento: BaixarXML });
    opcoes.push({ Descricao: "XML Cancelamento", Evento: BaixarXMLCancelamento });
    opcoes.push({ Descricao: "XML Encerramento", Evento: BaixarXMLEncerramento });
    opcoes.push({ Descricao: "XML Inclusão Motorista", Evento: BaixarXMLInclusaoMotorista });
    opcoes.push({ Descricao: "EDI Fiscal", Evento: BaixarEDIFiscal });
    opcoes.push({ Descricao: "Averbações do MDF-e", Evento: ConsultarAverbacaoMDFe });
    opcoes.push({ Descricao: "Compras Vale Pedágio", Evento: ConsultarCompraValePedagioMDFe });
    opcoes.push({ Descricao: "Integração SM", Evento: ConsultarSMViagemMDFe });
    opcoes.push({ Descricao: "DAMDFE Contingência", Evento: BaixarDAMDFEContingencia });
    opcoes.push({ Descricao: "Integrações do MDFe", Evento: ConsultarIntegracaoRetorno });
    opcoes.push({ Descricao: "Retornos Sefaz", Evento: RetornosSefaz });
    //if (EmissaoCTe.VersaoMDFe == "3.00")
    //    opcoes.push({ Descricao: "Emitir V. Anterior", Evento: EmitirMDFeVersaoAntiga });

    CriarGridView("/ManifestoEletronicoDeDocumentosFiscais/Consultar?callback=?", dados, "tbl_mdfes_table", "tbl_mdfes", "tbl_paginacao_mdfes", opcoes, [0, 1], null, null, 20);

    jQuery("#btnConsultarMDFe").blur();
}

function GetUrlParam(name) {
    var url = window.location.search.replace("?", "");
    var itens = url.split("&");
    for (n in itens) {
        if (itens[n].match(name)) {
            return itens[n].replace(name + "=", "");
        }
    }
    return null;
}

function CancelarMDFe(mdfe) {
    if (mdfe.data.Status == 3 || mdfe.data.Status == 6) {
        AbrirTelaCancelamentoMDFe();
        $("body").data("mdfeCancelamento", mdfe.data);
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para cancelar o mesmo.", "Atenção!");
    }
}

function EmitirMDFe(mdfe) {
    jConfirm("Deseja realmente emitir o MDF-e " + mdfe.data.Numero + "?", "Atenção!", function (r) {
        if (r) {
            executarRest("/ManifestoEletronicoDeDocumentosFiscais/Emitir?callback=?", { Codigo: mdfe.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("MDF-e emitido com sucesso!", "Sucesso!");
                    ConsultarMDFes();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    });
}

function EmitirMDFeVersaoAntiga(mdfe) {
    jConfirm("Deseja realmente emitir o MDF-e " + mdfe.data.Numero + " na versão antiga do Sefaz?", "Atenção!", function (r) {
        if (r) {
            dados = {
                Codigo: mdfe.data.Codigo,
                Versao: '1.00',
            };
            executarRest("/ManifestoEletronicoDeDocumentosFiscais/Emitir?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("MDF-e emitido na versão 1.00 com sucesso!", "Sucesso!");
                    ConsultarMDFes();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    });
}

function AbrirTelaCancelamentoMDFe() {
    $("#divCancelamentoMDFe").modal({ keyboard: false, backdrop: 'static' });
    $("#txtJustificativaCancelamentoMDFe").val("");
    $("#txtDataCancelamento").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
    $("#selCobrarCancelamento").val("");
    $("body").data("mdfeCancelamento", null);
}

function FecharTelaCancelamentoMDFe() {
    $("#divCancelamentoMDFe").modal('hide');
    $("#txtJustificativaCancelamentoMDFe").val("");
    $("#txtDataCancelamento").val("");
    $("body").data("mdfeCancelamento", null);
    VoltarAoTopoDaTela();
}

function FinalizarCancelamentoMDFe() {
    var justificativa = $("#txtJustificativaCancelamentoMDFe").val();

    if ($("body").data("empresa") != null && $("body").data("empresa").ExibirCobrancaCancelamento == true) {
        if ($("#selCobrarCancelamento").val() == "") {
            CampoComErro("#selCobrarCancelamento");
            return;
        }
        else
            CampoSemErro("#selCobrarCancelamento");
    }
    else
        $("#selCobrarCancelamento").val("Sim");

    if (justificativa.length > 20 && justificativa.length < 255) {
        executarRest("/ManifestoEletronicoDeDocumentosFiscais/Cancelar?callback=?", {
            Justificativa: justificativa,
            CodigoMDFe: $("body").data("mdfeCancelamento") != null ? $("body").data("mdfeCancelamento").Codigo : 0,
            DataCancelamento: $("#txtDataCancelamento").val(),
            CobrarCancelamento: $("#selCobrarCancelamento").val()
        }, function (r) {
            if (r.Sucesso) {
                jAlert("O MDF-e está em processo de cancelamento.", "Atenção", function () {
                    FecharTelaCancelamentoMDFe();
                    ConsultarMDFes();
                });
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgCancelamentoMDFe");
            }
        });
    } else {
        ExibirMensagemAlerta("A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.", "Atenção!", "placeholder-msgCancelamentoMDFe");
    }
}

function EncerrarMDFe(mdfe) {
    if (mdfe.data.Status == 3 || mdfe.data.Status == 4) {
        $("body").data("mdfeEncerramento", mdfe.data);
        AbrirTelaEncerramentoMDFe();
        ObterInformacoesEncerramento();
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para encerrar o mesmo.", "Atenção!");
    }
}

function AbrirTelaEncerramentoMDFe() {
    LimparCamposEncerramentoMDFe();

    $("#divEncerramentoMDFe").modal({ keyboard: false, backdrop: 'static' });
}

function FecharTelaEncerramentoMDFe() {
    LimparCamposEncerramentoMDFe();
    $("body").data("mdfeEncerramento", null);
    $("#divEncerramentoMDFe").modal('hide');
    VoltarAoTopoDaTela();
}

function LimparCamposEncerramentoMDFe() {
    $("#txtDataEncerramento").val("");
    $("#txtDataEventoEncerramento").val("");
    $("#txtEstadoEncerramento").val("");
    $("#selMunicipioEncerramento").html("");
}

function ObterInformacoesEncerramento() {
    executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterDetalhesEncerramento?callback=?", { CodigoMDFe: $("body").data("mdfeEncerramento").Codigo }, function (r) {
        if (r.Sucesso) {
            var data = new Date();
            $("#txtDataEncerramento").val(Globalize.format(data, "dd/MM/yyyy HH:mm"));
            $("#txtDataEventoEncerramento").val(Globalize.format(data, "dd/MM/yyyy HH:mm"));

            $("#txtEstadoEncerramento").val(r.Objeto.DescricaoUF);

            var selMunicipioEncerramento = $("#selMunicipioEncerramento");

            /**
             * Para nao ter dois tipos de tratamento, quando os municipios retornam 
             * como um objeto simples e nao um array de municipios
             * entao criamos um objeto e inserimos no array de municipios
             */
            var arrayMunicipios = [];
            if (!$.isArray(r.Objeto.Municipios)) {
                arrayMunicipios = [{
                    Codigo: r.Objeto.Municipios.Codigo,
                    Descricao: r.Objeto.Municipios.Descricao,
                }];
            } else {
                arrayMunicipios = r.Objeto.Municipios
            }

            /**
             * Itera para criar as opcoes
             * Para melhor eficiencia, criamso as opcoes como um array de string
             * Depois basta concatenar e inserir todas de uma vez so
             */
            var htmlOptions = [];
            for (var opt in arrayMunicipios) {
                opt = arrayMunicipios[opt];

                htmlOptions.push('<option value="' + opt.Codigo + '">' + opt.Descricao + '</option>');
            }
            selMunicipioEncerramento.html(htmlOptions.join(""));

            /**
             * Caso tenha apenas um municipio, colocamos o mesmo como selecionado
             */
            if (arrayMunicipios.length == 1)
                selMunicipioEncerramento.val(arrayMunicipios[0].Codigo);

        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgEncerramentoMDFe");
        }
    });
}

function ValidarCamposEncerramentoMDFe() {
    var data = $("#txtDataEncerramento").val();
    var hora = $("#txtHoraEncerramento").val();
    var municipio = $("#selMunicipioEncerramento").val();
    var valido = true;

    if (data != "") {
        CampoSemErro("#txtDataEncerramento");
    } else {
        CampoComErro("#txtDataEncerramento");
        valido = false;
    }

    if (hora != "") {
        CampoSemErro("#txtHoraEncerramento");
    } else {
        CampoComErro("#txtHoraEncerramento");
        valido = false;
    }

    if (municipio != null && municipio != "") {
        CampoSemErro("#selMunicipioEncerramento");
    } else {
        CampoComErro("#selMunicipioEncerramento");
        valido = false;
    }

    return valido;
}

function FinalizarEncerramentoMDFe() {
    if (ValidarCamposEncerramentoMDFe()) {
        executarRest("/ManifestoEletronicoDeDocumentosFiscais/Encerrar?callback=?", {
            CodigoMDFe: $("body").data("mdfeEncerramento").Codigo,
            CodigoMunicipio: $("#selMunicipioEncerramento").val(),
            DataEncerramento: $("#txtDataEncerramento").val(),
            DataEvento: $("#txtDataEventoEncerramento").val()
        }, function (r) {
            if (r.Sucesso) {
                jAlert("O MDF-e está em processo de encerramento.", "Atenção", function () {
                    FecharTelaEncerramentoMDFe();
                    ConsultarMDFes();
                });
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgEncerramentoMDFe");
            }
        });
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com um asterísco (*) são obrigatórios.", "Atenção!", "placeholder-msgEncerramentoMDFe");
    }
}

function IncluirMotorista(mdfe) {
    if (mdfe.data.Status == 3) {
        $("body").data("mdfeInclusaoMotorista", mdfe.data);
        AbrirTelaInclusaoMotorista();
        //ObterInformacoesEncerramento();
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para incluir motorista.", "Atenção!");
    }
}

function AbrirTelaInclusaoMotorista() {
    LimparCamposInclusaoMotorista();

    $("#divInclusaoMotorista").modal({ keyboard: false, backdrop: 'static' });
}

function FecharTelaInclusaoMotorista() {
    LimparCamposInclusaoMotorista();
    $("body").data("mdfeInclusaoMotorista", null);
    $("#divInclusaoMotorista").modal('hide');
    VoltarAoTopoDaTela();
}

function LimparCamposInclusaoMotorista() {
    $("#txtCPFMotoristaInclusao").val("");
    $("#txtNomeMotoristaInclusao").val("");
    var today = new Date();
    $("#txtDataEventoInclusao").val(Globalize.format(today, "dd/MM/yyyy HH:mm"));
}

function SolicitarInclusaoMotorista() {
    executarRest("/ManifestoEletronicoDeDocumentosFiscais/IncluirMotorista?callback=?", {
        CodigoMDFe: $("body").data("mdfeInclusaoMotorista").Codigo,
        CPFMotorista: $("#txtCPFMotoristaInclusao").val(),
        NomeMotorista: $("#txtNomeMotoristaInclusao").val(),
        DataEvento: $("#txtDataEventoInclusao").val()
    }, function (r) {
        if (r.Sucesso) {
            jAlert("Evento de inclusão de Motorista solicitado.", "Atenção", function () {
                FecharTelaInclusaoMotorista();
                ConsultarMDFes();
            });
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgInclusaoMotorista");
        }
    });
}



function BaixarDAMDFE(mdfe) {
    if (mdfe.data.Status >= 3 && mdfe.data.Status < 9) {
        executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadDAMDFE", { CodigoMDFe: mdfe.data.Codigo, Contingencia: false });
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para baixar o DAMDFE.", "Atenção!");
    }
}

function BaixarDAMDFEContingencia(mdfe) {
    if (mdfe.data.Status == 0 || mdfe.data.Status == 9 || mdfe.data.Status == 10) {
        executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadDAMDFE", { CodigoMDFe: mdfe.data.Codigo, Contingencia: true });
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e não tenha sido autorizado para baixar o DAMDFE em contingência.", "Atenção!");
    }
}

function BaixarXML(mdfe) {
    if (mdfe.data.Status >= 3 && mdfe.data.Status < 9) {
        executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLAutorizacao", { CodigoMDFe: mdfe.data.Codigo });
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja autorizado para baixar o XML de autorização.", "Atenção!");
    }
}

function BaixarXMLCancelamento(mdfe) {
    if (mdfe.data.Status == 7) {
        executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLCancelamento", { CodigoMDFe: mdfe.data.Codigo });
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja cancelado para baixar o XML de cancelamento.", "Atenção!");
    }
}

function BaixarXMLEncerramento(mdfe) {
    if (mdfe.data.Status == 5) {
        executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLEncerramento", { CodigoMDFe: mdfe.data.Codigo });
    } else {
        ExibirMensagemAlerta("É necessário que o MDF-e esteja encerrado para baixar o XML de encerramento.", "Atenção!");
    }
}

function BaixarEDIFiscal(mdfe) {
    executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadEDIFiscal", { CodigoMDFe: mdfe.data.Codigo });
}

function BaixarXMLInclusaoMotorista(mdfe) {
    executarDownload("/ManifestoEletronicoDeDocumentosFiscais/DownloadXMLInclusaoMotorista", { CodigoMDFe: mdfe.data.Codigo });
}

function EditarMDFe(mdfe, showInfPag = false) {
    executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterDetalhes?callback=?", { CodigoMDFe: mdfe.data.Codigo }, function (r) {
        if (r.Sucesso) {
            AbrirTelaEmissaoMDFe();
            
            $("#txtNumero").attr("disabled", true);
            $("#txtNumero").val(r.Objeto.Numero);
            $("#selSerie").val(r.Objeto.Serie);
            $("#txtDataEmissao").val(r.Objeto.DataEmissao);
            $("#txtHoraEmissao").val(r.Objeto.HoraEmissao);
            $("#selModal").val(r.Objeto.Modal);

            $("#selUFCarregamento").val(r.Objeto.UFCarregamento);
            $("body").data("UFCarregamento", $("#selUFCarregamento").val());
            ObterMunicipios($("#selUFCarregamento").val(), "selMunicipioCarregamento");

            $("#selUFDescarregamento").val(r.Objeto.UFDescarregamento);
            $("body").data("UFDescarregamento", $("#selUFDescarregamento").val());
            ObterMunicipios($("#selUFDescarregamento").val(), "selMunicipioDescarregamento");

            $("#txtRNTRC").val(r.Objeto.RNTRC);
            $("#txtCIOT").val(r.Objeto.CIOT);
            $("#selUnidadeMedida").val(r.Objeto.UnidadeMedidaMercadoria);
            $("#txtPesoBruto").val(Globalize.format(r.Objeto.PesoBrutoMercadoria, "n4"));
            $("#txtValorTotal").val(Globalize.format(r.Objeto.ValorTotalMercadoria, "n2"));
            $("#txtObservacaoFisco").val(r.Objeto.ObservacaoFisco);
            $("#txtObservacaoContribuinte").val(r.Objeto.ObservacaoContribuinte);
            $("#selTipoEmitente").val(r.Objeto.TipoEmitente);
            $("#lblLogMDFe").text(r.Objeto.Log);

            $("#txtDataCancelamentoMDFee").val(r.Objeto.DataCancelamento);
            $("#txtProtocoloCancelamentoMDFee").val(r.Objeto.ProtocoloCancelamento);
            $("#txtJustificativaCancelamentoMDFee").val(r.Objeto.Justificativa);

            $("#selTipoCarga").val(r.Objeto.TipoCarga);
            $("#txtProdutoPredominanteDescricao").val(r.Objeto.ProdutoPredominanteDescricao);
            $("#txtProdutoPredominanteCEAN").val(r.Objeto.ProdutoPredominanteCEAN);
            $("#txtProdutoPredominanteNCM").val(r.Objeto.ProdutoPredominanteNCM);

            $("#txtCEPCarregamento").val(r.Objeto.CEPCarregamentoLotacao);
            $("#txtLatitudeCarregamento").val(r.Objeto.LatitudeCarregamentoLotacao,);
            $("#txtLongitudeCarregamento").val(r.Objeto.LongitudeCarregamentoLotacao,);

            $("#txtCEPDescarregamento").val(r.Objeto.CEPDescarregamentoLotacao,);
            $("#txtLatitudeDescarregamento").val(r.Objeto.LatitudeDescarregamentoLotacao,);
            $("#txtLongitudeDescarregamento").val(r.Objeto.LongitudeDescarregamentoLotacao,);

            $("#selFormaPagamento").val(r.Objeto.TipoPagamento);
            $("#txtValorDoAdiantamento").val(Globalize.format(r.Objeto.ValorAdiantamento, "n2"));

            $("#selInformacoesBancarias").val(r.Objeto.TipoInformacaoBancaria);
            $("#txtNumeroBanco").val(r.Objeto.Banco);
            $("#txtAgencia").val(r.Objeto.Agencia);
            $("#txtChavePix").val(r.Objeto.ChavePIX);
            $("#txtIPEF").val(r.Objeto.Ipef);

            StateComponente.set(r.Objeto.ComponentesPagamento)
            StateParcela.set(r.Objeto.ParcelasPagamento)

            _permitirMultiplasParcelas = r.Objeto.TipoEmissorDocumentoMDFe === 1;
            ParcelaUnica()

            $("#txtNumeroDaParcela").val((StateParcela.get().length + 1));
            $("#txtVencimentoDaParcela").val(DataVencimentoProximaParcela());

            if (r.Objeto.Status != 9 && r.Objeto.Status != 0) {
                $("#divEmissaoMDFe .modal-body button").attr("disabled", true);
                $("#divEmissaoMDFe .modal-body .form-control").attr("disabled", true);
            }

            for (var i in r.Objeto.Seguros) {
                var seguro = r.Objeto.Seguros[i];
                seguro.DescricaoTipo = seguro.Tipo + " - " + seguro.DescricaoTipo;
                InsereSeguro(seguro);
            }

            StateTomadores.set(r.Objeto.Contratantes);
            StateCIOT.set(r.Objeto.CIOTs);

            $("body").data("MDFe", r.Objeto);

            CarregarLacres(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);
            CarregarMotoristas(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);
            CarregarMunicipiosCarregamento(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);
            CarregarMunicipiosDescarregamento(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);
            CarregarPercursos(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);
            CarregarReboques(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);
            CarregarValesPedagio(r.Objeto.Status == 9 || r.Objeto.Status == 0 || r.Objeto.Status == 11 ? false : true);
            CarregarVeiculo(r.Objeto.Status != 9 && r.Objeto.Status != 0 ? true : false);

            ConfigurarComponentesDeInfPagamento();

            if (showInfPag) {
                $('a[href="#tabRodoviario"]').tab('show');
                $('a[href="#tabInfPagamento"]').tab('show');
            }

            ClassesVersao({ VersaoMDFe: r.Objeto.Versao });
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function DuplicarMDFe(mdfe) {
    executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterDetalhes?callback=?", { CodigoMDFe: mdfe.data.Codigo }, function (r) {
        if (r.Sucesso) {

            AbrirTelaEmissaoMDFe();
            LimparCamposMDFe();
            SetarDataEmissao();
            SetarRNTRC();

            $("#selSerie").val(r.Objeto.Serie);
            $("#selModal").val(r.Objeto.Modal);

            $("#selUFCarregamento").val(r.Objeto.UFCarregamento);
            $("body").data("UFCarregamento", $("#selUFCarregamento").val());
            ObterMunicipios($("#selUFCarregamento").val(), "selMunicipioCarregamento");

            $("#selUFDescarregamento").val(r.Objeto.UFDescarregamento);
            $("body").data("UFDescarregamento", $("#selUFDescarregamento").val());
            ObterMunicipios($("#selUFDescarregamento").val(), "selMunicipioDescarregamento");

            $("#txtRNTRC").val(r.Objeto.RNTRC);
            $("#txtCIOT").val(r.Objeto.CIOT);
            $("#selUnidadeMedida").val(r.Objeto.UnidadeMedidaMercadoria);
            $("#txtPesoBruto").val(Globalize.format(r.Objeto.PesoBrutoMercadoria, "n4"));
            $("#txtValorTotal").val(Globalize.format(r.Objeto.ValorTotalMercadoria, "n2"));
            $("#txtObservacaoFisco").val(r.Objeto.ObservacaoFisco);
            $("#txtObservacaoContribuinte").val(r.Objeto.ObservacaoContribuinte);
            $("#selTipoEmitente").val(r.Objeto.TipoEmitente);
            $("#lblLogMDFe").text(r.Objeto.Log);

            $("#txtDataCancelamentoMDFee").val(r.Objeto.DataCancelamento);
            $("#txtProtocoloCancelamentoMDFee").val(r.Objeto.ProtocoloCancelamento);
            $("#txtJustificativaCancelamentoMDFee").val(r.Objeto.Justificativa);

            $("#selTipoCarga").val(r.Objeto.TipoCarga);
            $("#txtProdutoPredominanteDescricao").val(r.Objeto.ProdutoPredominanteDescricao);
            $("#txtProdutoPredominanteCEAN").val(r.Objeto.ProdutoPredominanteCEAN);
            $("#txtProdutoPredominanteNCM").val(r.Objeto.ProdutoPredominanteNCM);

            $("#txtCEPCarregamento").val(r.Objeto.CEPCarregamentoLotacao);
            $("#txtLatitudeCarregamento").val(r.Objeto.LatitudeCarregamentoLotacao);
            $("#txtLongitudeCarregamento").val(r.Objeto.LongitudeCarregamentoLotacao);

            $("#txtCEPDescarregamento").val(r.Objeto.CEPDescarregamentoLotacao);
            $("#txtLatitudeDescarregamento").val(r.Objeto.LatitudeDescarregamentoLotacao);
            $("#txtLongitudeDescarregamento").val(r.Objeto.LongitudeDescarregamentoLotacao);


            $("#selFormaPagamento").val(r.Objeto.TipoPagamento);
            $("#txtValorDoAdiantamento").val(Globalize.format(r.Objeto.ValorAdiantamento, "n2"));

            $("#selInformacoesBancarias").val(r.Objeto.TipoInformacaoBancaria);
            $("#txtNumeroBanco").val(r.Objeto.Banco);
            $("#txtAgencia").val(r.Objeto.Agencia);
            $("#txtChavePix").val(r.Objeto.ChavePIX);
            $("#txtIPEF").val(r.Objeto.Ipef);

            StateComponente.set(r.Objeto.ComponentesPagamento);
            StateParcela.set(r.Objeto.ParcelasPagamento);

            for (var i in r.Objeto.Seguros) {
                var seguro = r.Objeto.Seguros[i];
                seguro.DescricaoTipo = seguro.Tipo + " - " + seguro.DescricaoTipo;
                InsereSeguro(seguro);
            }

            StateTomadores.set(r.Objeto.Contratantes);
            StateCIOT.set(r.Objeto.CIOTs);

            $("body").data("MDFe", r.Objeto);

            CarregarLacres(false);
            CarregarMotoristas(false);
            CarregarMunicipiosCarregamento(false);
            CarregarMunicipiosDescarregamento(false);
            CarregarPercursos(false);
            CarregarReboques(false);
            CarregarValesPedagio(false);
            CarregarVeiculo(false);

            ClassesVersao({ VersaoMDFe: r.Objeto.Versao });

            $("body").data("MDFe", null);
            $("#divEmissaoMDFe .modal-body button").attr("disabled", false);
            $("#divEmissaoMDFe .modal-body .form-control").attr("disabled", false);
            $("#txtNumero").attr("disabled", true);

            ConfigurarComponentesDeInfPagamento();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarLacres(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/LacreMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("lacres", r.Objeto);
            RenderizarLacres(status); //mdfe.Status != 9 && mdfe.Status != 0 ? true : false
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarMunicipiosCarregamento(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/MunicipioCarregamentoMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("municipiosCarregamento", r.Objeto);
            RenderizarMunicipiosCarregamento(status);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarMunicipiosDescarregamento(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/MunicipioDescarregamentoMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("municipiosDescarregamento", r.Objeto);
            RenderizarMunicipiosDescarregamento(status);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarMotoristas(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/MotoristaMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("motoristas", r.Objeto);
            RenderizarMotoristas(status);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarPercursos(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/PercursoMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("percursos", r.Objeto);
            RenderizarPercursos(status);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarReboques(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/ReboqueMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("reboques", r.Objeto);
            RenderizarReboques(status);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarValesPedagio(status) {
    var mdfe = $("body").data("MDFe");
    executarRest("/ValePedagioMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: mdfe.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("valesPedagio", r.Objeto);

            $("#txtCNPJFornecedorValePedagio").attr("disabled", status);
            $("#txtCNPJResponsavelValePedagio").attr("disabled", status);
            $("#txtNumeroComprovanteValePedagio").attr("disabled", status);
            $("#txtCodigoAgendamentoPortoValePedagio").attr("disabled", status);
            $("#txtValorValePedagio").attr("disabled", status);
            $("#btnSalvarValePedagio").attr("disabled", status);
            $("#btnExcluirValePedagio").attr("disabled", status);
            $("#btnCancelarValePedagio").attr("disabled", status);

            RenderizarValesPedagio(status);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function CarregarVeiculo() {
    executarRest("/VeiculoMDFe/BuscarPorMDFe?callback=?", { CodigoMDFe: $("body").data("MDFe").Codigo }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto != null) {
                $("#txtPlacaVeiculo").val(r.Objeto.Placa);
                $("#txtRENAVAMVeiculo").val(r.Objeto.RENAVAM);
                $("#txtTaraVeiculo").val(r.Objeto.Tara);
                $("#txtCapacidadeKGVeiculo").val(r.Objeto.CapacidadeKG);
                $("#txtCapacidadeM3Veiculo").val(r.Objeto.CapacidadeM3);
                $("#txtRNTRCVeiculo").val(r.Objeto.RNTRC);
                $("#selRodadoVeiculo").val(r.Objeto.TipoRodado);
                $("#selCarroceriaVeiculo").val(r.Objeto.TipoCarroceria);
                $("#selUFVeiculo").val(r.Objeto.UF);
                $("#txtCPFCNPJProprietarioVeiculo").val(r.Objeto.CPFCNPJ);
                $("#txtIEProprietarioVeiculo").val(r.Objeto.IE);
                $("#txtNomeProprietarioVeiculo").val(r.Objeto.Nome);
                $("#selUFProprietarioVeiculo").val(r.Objeto.UFProprietario);
                $("#selTipoProprietarioVeiculo").val(r.Objeto.TipoProprietario);
            }
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}


function RetornosSefaz(mdfe) {
    var opcoes = new Array();
    opcoes.push({ Descricao: "Visualizar", Evento: VisualizarMensagemRetornoSefaz });

    CriarGridView("/ManifestoEletronicoDeDocumentosFiscais/ConsultarRetornosSefaz?callback=?", { CodigoMDFe: mdfe.data.Codigo }, "tbl_retornossefaz_table", "tbl_retornossefaz", "tbl_paginacao_retornossefaz", opcoes, [0]);
    $("#tituloRetornosSefaz").text("Retornos Sefaz do MDF-e " + mdfe.data.Numero + "-" + mdfe.data.Serie);
    $('#divRetornosSefaz').modal("show");
}

function VisualizarMensagemRetornoSefaz(retornoSefaz) {
    var tagBr = "<br>";
    var html = retornoSefaz.data.RetornoSefaz.replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, '$1' + tagBr + '$2');
    jAlert(html, "Mensagem retorno Sefaz");
}