$(document).ready(function () {
    $("#btnNovoMDFe").click(function () {
        AbrirTelaEmissaoMDFe(true);
    });

    $("#btnCancelarMDFe").click(function () {
        FecharTelaEmissaoMDFe();
    });

    $("#btnSalvarMDFe").click(function () {
        SalvarMDFe(false);
    });

    $("#btnEmitirMDFe").click(function () {
        SalvarMDFe(true);
    });

    $("#divEmissaoMDFe").on("hide.bs.modal", function () {
        ClassesVersao(EmissaoCTe);
    });

    BuscarDadosEmpresa();

    $("#selInformacoesBancarias").change(function () {
        atualizarComponentesBancarios();
    });

    $("#selFormaPagamento").change(function () {
        atualizarComponentesDeParcelamento();
    });

    $("#txtValorDoAdiantamento").change(function () {
        RecalcularValorDasParcelas();
    });

    atualizarComponentesBancarios();
});

function LimparCamposMDFe() {
    $("body").data("MDFe", null);

    LimparCamposDocumentoMunicipioDescarregamento();
    $("body").data("municipioDescarregamentoDocumento", null);
    $("body").data("documentoMunicipioDescarregamentoAlterado", false);
    RenderizarDocumentosMunicipioDescarregamento();

    LimparCamposLacre();
    $("body").data("lacres", null);
    RenderizarLacres();

    LimparCamposMotorista();
    $("body").data("motoristas", null);
    RenderizarMotoristas();

    LimparCamposMunicipioCarregamento();
    $("body").data("municipiosCarregamento", null);
    RenderizarMunicipiosCarregamento();

    LimparCamposMunicipioDescarregamento();
    $("body").data("municipiosDescarregamento", null);
    RenderizarMunicipiosDescarregamento();

    LimparCamposPercurso();
    $("body").data("percursos", null);
    RenderizarPercursos();

    LimparCamposReboque();
    $("body").data("reboques", null);
    RenderizarReboques();

    LimparCamposValePedagio();
    $("body").data("valesPedagio", null);
    RenderizarValesPedagio();

    LimparCamposConsultaCTe();
    LimparCamposVeiculo();
    LimparCamposInformacaoSeguro();
    setSeguros([]);

    $("#txtNumero").val("Automático");
    $("#selSerie").val($("#selSerie option:first").val());
    $("#txtDataEmissao").val("");
    $("#txtHoraEmissao").val("");
    $("#selModal").val($("#selModal option:first").val());
    $("#selUFCarregamento").val("");
    $("#selMunicipioCarregamento").html("");
    $("#selUFDescarregamento").val("");
    $("#selMunicipioDescarregamento").html("");
    $("#selTipoEmitente").val("1");
    $("#txtRNTRC").val("");
    $("#txtCIOT").val("");
    $("#selUnidadeMedida").val($("#selUnidadeMedida option:first").val());
    $("#txtPesoBruto").val("0,0000");
    $("#txtValorTotal").val("0,00");
    $("#txtObservacaoFisco").val("");
    $("#txtObservacaoContribuinte").val("");
    $("#lblLogMDFe").text("");

    $("#selTipoCarga").val($("#selTipoCarga option:first").val());
    $("#txtProdutoPredominanteDescricao").val("");
    $("#txtProdutoPredominanteCEAN").val("");
    $("#txtProdutoPredominanteNCM").val("");

    $("#txtCEPCarregamento").val("");
    $("#txtLatitudeCarregamento").val("");
    $("#txtLongitudeCarregamento").val("");
    $("#txtCEPDescarregamento").val("");
    $("#txtLatitudeDescarregamento").val("");
    $("#txtLongitudeDescarregamento").val("");

    $("#txtDataCancelamentoMDFee").val("");
    $("#txtProtocoloCancelamentoMDFee").val("");
    $("#txtJustificativaCancelamentoMDFee").val("");

    $("#divEmissaoMDFe .modal-body button").removeAttr("disabled");
    $("#divEmissaoMDFe .modal-body input").removeAttr("disabled");
    $("#divEmissaoMDFe .modal-body select").removeAttr("disabled");

    $("#txtDataCancelamentoMDFee").attr("disabled", true)
    $("#txtProtocoloCancelamentoMDFee").attr("disabled", true)
    $("#txtJustificativaCancelamentoMDFee").attr("disabled", true)
    $("#txtNumero").attr("disabled", true);

    $("#txtCNPJRespPagamento").val("");
    $("#selFormaPagamento").val($("#selFormaPagamento option:first").val());

    $("#txtValorDoPagamento").val("0,00");
    $("#txtValorDoAdiantamento").val("0,00");
    $("#selInformacoesBancarias").val($("#selInformacoesBancarias option:first").val());
    $("#txtNumeroDaParcela").val((StateParcela.get().length + 1));
    $("#txtVencimentoDaParcela").val(DataVencimentoProximaParcela());

    $("#txtNumeroBanco").val("");
    $("#txtAgencia").val("");
    $("#txtChavePix").val("");
    $("#txtIPEF").val("");

    desabilitarCamposInfPgto();

    $("#tabsEmissaoMDFe a:first").tab("show");
    $("#tabsDados a:first").tab("show");
    $("#tabsRodoviario a:first").tab("show");
    $("#tabsInfPagamento a:first").tab("show");

    $("#placeholder-msgEmissaoMDFe").html("");
}

function AbrirTelaEmissaoMDFe(novoMDFe) {

    LimparCamposMDFe();
    SetarDataEmissao();
    SetarRNTRC();

    if (novoMDFe != null && novoMDFe) {
        $('#divEmissaoMDFe').on('shown.bs.modal', function (e) {
            jConfirm("Deseja selecionar os CT-es para o MDF-e?", "Atenção", function (r) {
                if (r)
                    AbrirTelaConsultaCTeAvancado();
            });
            $('#divEmissaoMDFe').off('shown.bs.modal');
        });
    }

    $("#divEmissaoMDFe").modal({ keyboard: false, backdrop: 'static' });

}

function FecharTelaEmissaoMDFe() {
    $("#divEmissaoMDFe").modal('hide');
    LimparCamposMDFe();
    VoltarAoTopoDaTela();
}

function SalvarMDFe(emitir) {
    var dados = {
        Codigo: $("body").data("MDFe") != null ? $("body").data("MDFe").Codigo : 0,
        Serie: $("#selSerie").val(),
        DataEmissao: $("#txtDataEmissao").val(),
        HoraEmissao: $("#txtHoraEmissao").val(),
        Modal: $("#selModal").val(),
        TipoEmitente: $("#selTipoEmitente").val(),
        UFCarregamento: $("#selUFCarregamento").val(),
        UFDescarregamento: $("#selUFDescarregamento").val(),
        RNTRC: $("#txtRNTRC").val(),
        CIOT: $("#txtCIOT").val(),
        UnidadeMedida: $("#selUnidadeMedida").val(),
        PesoBruto: $("#txtPesoBruto").val(),
        ValorTotalMercadoria: $("#txtValorTotal").val(),
        ObservacaoFisco: $("#txtObservacaoFisco").val(),
        ObservacaoContribuinte: $("#txtObservacaoContribuinte").val(),
        Lacres: JSON.stringify($("body").data("lacres")),
        Motoristas: JSON.stringify($("body").data("motoristas")),
        MunicipiosCarregamento: JSON.stringify($("body").data("municipiosCarregamento")),
        MunicipiosDescarregamento: JSON.stringify($("body").data("municipiosDescarregamento")),
        Percursos: JSON.stringify($("body").data("percursos")),
        Reboques: JSON.stringify($("body").data("reboques")),
        ValesPedagio: JSON.stringify($("body").data("valesPedagio")),
        Veiculo: JSON.stringify(ObterVeiculo()),
        Seguros: JSON.stringify(getSeguros()),
        Contratantes: StateTomadores.toJson(),
        CIOTs: StateCIOT.toJson(),
        TipoCarga:$("#selTipoCarga").val(),
        ProdutoPredominanteDescricao: $("#txtProdutoPredominanteDescricao").val(),
        ProdutoPredominanteCEAN: $("#txtProdutoPredominanteCEAN").val(),
        ProdutoPredominanteNCM: $("#txtProdutoPredominanteNCM").val(),
        CEPCarregamento: $("#txtCEPCarregamento").val(),
        LatitudeCarregamento: $("#txtLatitudeCarregamento").val(),
        LongitudeCarregamento: $("#txtLongitudeCarregamento").val(),
        CEPDescarregamento: $("#txtCEPDescarregamento").val(),
        LatitudeDescarregamento: $("#txtLatitudeDescarregamento").val(),
        LongitudeDescarregamento: $("#txtLongitudeDescarregamento").val(),

        TipoPagamento: $("#selFormaPagamento").val(),
        ValorAdiantamento: $("#txtValorDoAdiantamento").val(),
        TipoInformacaoBancaria: $("#selInformacoesBancarias").val(),
        Banco: $("#txtNumeroBanco").val(),
        Agencia: $("#txtAgencia").val(),
        ChavePIX: $("#txtChavePix").val(),
        Ipef: $("#txtIPEF").val(),

        Componentes: StateComponente.toJson(),
        Parcelas: StateParcela.toJson(),

        Emitir: emitir
    };
    
    var mensagemValidacao = ValidarMDFe(dados);
    
    if (mensagemValidacao != "") {
        ExibirMensagemAlerta("<br /><br />" + mensagemValidacao, "Atenção!", "placeholder-msgEmissaoMDFe");
        return;
    }
    
    executarRest("/ManifestoEletronicoDeDocumentosFiscais/Salvar?callback=?", dados, function (r) {
        if (r.Sucesso) {
            jAlert("MDF-e " + (emitir ? "emitido" : "salvo") + " com sucesso!", "Sucesso", function (r) {
                FecharTelaEmissaoMDFe();
                ConsultarMDFes();
            });
        } else {
            //if (r.Objeto != null && r.Objeto.Codigo > 0)
            //    $("body").data("MDFe", r.Objeto);
            FecharTelaEmissaoMDFe();
            ConsultarMDFes();
            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder");
        }
    });
}

function ValidarMDFe(dados) {

    var municipiosCarregamento = $("body").data("municipiosCarregamento");
    var municipiosDescarregamento = $("body").data("municipiosDescarregamento");
    var motoristas = $("body").data("motoristas");
    var valesPedagio = $("body").data("valesPedagio");
    var veiculo = ObterVeiculo();
    var reboques = $("body").data("reboques");

    var mensagem = "";

    if (dados.Serie == null || dados.Serie == "")
        mensagem += "<b>Série</b> inválida.<br />";

    if (dados.DataEmissao == null || dados.DataEmissao == "")
        mensagem += "<b>Data de emissão</b> inválida.<br/>";

    if (dados.HoraEmissao == null || dados.HoraEmissao == "")
        mensagem += "<b>Hora de emissão</b> inválida.<br/>";

    if (dados.Modal == null || dados.Modal == "")
        mensagem += "<b>Modal</b> inválida.<br/>";

    if (dados.UFCarregamento == null || dados.UFCarregamento == "")
        mensagem += "<b>UF de Carregamento</b> inválida.<br/>";

    if (dados.UFDescarregamento == null || dados.UFDescarregamento == "")
        mensagem += "<b>UF de Descarregamento</b> inválida.<br/>";

    if (dados.RNTRC == null || dados.RNTRC == "")
        mensagem += "<b>RNTRC</b> inválida.<br/>";

    if (dados.UnidadeMedida == null || dados.UnidadeMedida == "")
        mensagem += "<b>Unidade de Medida</b> inválida.<br/>";

    var pesoBruto = Globalize.parseFloat(dados.PesoBruto);
    if (isNaN(pesoBruto) || pesoBruto <= 0)
        mensagem += "<b>Peso Total da Carga</b> inválido.<br/>";

    var valorTotalMercadoria = Globalize.parseFloat(dados.ValorTotalMercadoria);
    if (isNaN(valorTotalMercadoria) || valorTotalMercadoria <= 0)
        mensagem += "<b>Valor Total da Carga</b> inválido.<br/>";

    if (veiculo.Placa == null || veiculo.Placa == "" ||
        veiculo.Tara <= 0 ||
        veiculo.TipoRodado == null || veiculo.TipoRodado == "" ||
        veiculo.TipoCarroceria == null || veiculo.TipoCarroceria == "" ||
        veiculo.UF == null || veiculo.UF == "" ||
        ((veiculo.CPFCNPJ != null && veiculo.CPFCNPJ != "") &&
             (veiculo.Nome == null || veiculo.Nome == "" ||
              veiculo.UFProprietario == null || veiculo.UFProprietario == "" ||
              veiculo.TipoProprietario == null || veiculo.TipoProprietario == "")))
        mensagem += "Informações do <b>Veículo</b> inválidas.<br/>";

    if (reboques != null) {

        var countReboques = 0;

        for (var i = 0; i < reboques.length; i++)
            if (!reboques[i].Excluir)
                countReboques++;

        if (countReboques > 3) {
            mensagem += "Não são permitidos mais de 3 <b>Reboques</b>.<br/>";
        }

    } else if (reboques != null) {
        for (var i = 0; i < reboques.length; i++) {
            if (reboques[i].Placa == null || reboques[i].Placa == "" ||
                reboques[i].Tara <= 0 ||
                reboques[i].TipoCarroceria == null || reboques[i].TipoCarroceria == "" ||
                reboques[i].UF == null || reboques[i].UF == "" ||
                ((reboques[i].CPFCNPJ != null && reboques[i].CPFCNPJ != "") &&
                    (reboques[i].Nome == null || reboques[i].Nome == "" ||
                     reboques[i].UFProprietario == null || reboques[i].UFProprietario == "" ||
                     reboques[i].TipoProprietario == null || reboques[i].TipoProprietario == "")))
                mensagem += "Informações do <b>Reboque</b> " + reboques[i].Placa + " estão inválidas.<br/>";
        }
    }

    if (municipiosCarregamento == null || municipiosCarregamento.length <= 0)
        mensagem += "É necessário adicionar um ou mais <b>Municípios de Carregamento</b>.<br/>";

    if (municipiosDescarregamento == null || municipiosDescarregamento.length <= 0) {
        mensagem += "É necessário adicionar um ou mais <b>Municípios de Descarregamento</b>.<br/>";
    } else {
        //if ($("#selTipoEmitente").val() != "3") { //MDFe de CTe Globalizado deve enviar NFe
            for (var i = 0; i < municipiosDescarregamento.length; i++) {
                if ((municipiosDescarregamento[i].Documentos == null || municipiosDescarregamento[i].Documentos.length <= 0) && (municipiosDescarregamento[i].NFes == null || municipiosDescarregamento[i].NFes.length <= 0) && (municipiosDescarregamento[i].ChaveCTes == null || municipiosDescarregamento[i].ChaveCTes.length <= 0))
                    mensagem += "O <b>Município de Descarregamento</b> " + municipiosDescarregamento[i].DescricaoMunicipio + " não possui documentos.<br/>";
            }
        //}
    }

    if (motoristas == null || motoristas.length <= 0) {
        mensagem += "É necessário adicionar um ou mais <b>Motoristas</b>.<br/>";
    } else if (motoristas.length > 10) {
        mensagem += "Não são permitidos mais de 10 <b>Motoristas</b>.<br/>";
    } else {
        for (var i = 0; i < motoristas.length; i++) {
            if (motoristas[i].Nome == null || motoristas[i].Nome == "" ||
                motoristas[i].CPF == null || motoristas[i].CPF == "")
                mensagem += "Informações do <b>Motorista</b> " + motoristas[i].Nome + " - " + motoristas[i].CPF + " inválidas.<br/>";
        }
    }

    if (valesPedagio != null && valesPedagio.length > 0) {
        for (var i = 0; i < valesPedagio.length; i++) {
            if (valesPedagio[i].CNPJFornecedor == null || valesPedagio[i].CNPJFornecedor == "" ||
                valesPedagio[i].NumeroComprovante == null || valesPedagio[i].NumeroComprovante == "" || valesPedagio[i].NumeroComprovante == "0")
                mensagem += "Informações do <b>Vale Pedágio</b> " + valesPedagio[i].CNPJFornecedor + " - " + valesPedagio[i].NumeroComprovante + " inválidas.<br/>";
        }
    }


    if (validarInformacoesDePagamentoDoFrete()) {
        if (isCampoVazio(dados.ProdutoPredominanteNCM)) {
            mensagem += '<b>Nomenclatura Comum do Mercosul (NCM)</b> inválido. (Acesse a aba Totais e informe um NCM válido).<br/>';
        }

        if (Globalize.parseFloat(dados.ValorAdiantamento) > 0 && Number(dados.TipoPagamento) === 0) {
            mensagem += `A <b>Forma de Pagamento</b> deve ser informada como <b>À Prazo</b> 
                 quando informado <b>Valor do Adiantamento</b>. 
                 (Acesse a aba Rodoviário | Informações de Pagamento de Frete e ajuste a Forma de Pagamento).<br/>`;
        }

        const valorTotalComponentes = CalcularSomaComponentes();
        const valorAdiantamento = parseValorBR(dados.ValorAdiantamento);

        if (valorTotalComponentes > 0) {
            if (valorAdiantamento == valorTotalComponentes)
                mensagem += `O <b>Valor do Adiantamento</b> não pode ser igual ao <b>Valor Total</b> do pagamento. 
                        (Acesse a aba Rodoviário | Informações de Pagamento de Frete e ajuste os Valores informados).<br/>`;

            if ($("#selFormaPagamento").val() === "1") {
                const valorParcelas = arredondarPara2Casas(CalcularSomaParcelas());
                const diferencaEsperada = arredondarPara2Casas(valorTotalComponentes - valorAdiantamento);

                if (Math.abs(diferencaEsperada - valorParcelas) > 0.01) {
                    $("#btnRecalcularParcela").show();

                    mensagem += `O valor da <b>Soma das Parcelas</b> deve ser igual a <b>Soma dos Componentes menos o Valor de Adiantamento </b>. 
                        (Acesse a aba Rodoviário | Informações de Pagamento de Frete | Parcelas e clique em <b>Recalcular</b>).<br/>`;
                }
            }
        }

        switch (parseInt(dados.TipoInformacaoBancaria)) {
            case 1:
                if (isCampoVazio(dados.ChavePIX))
                    mensagem += msgErro("Chave PIX", "e informe a Chave PIX");
                break;

            case 2:
                if (isCampoVazio(dados.Banco))
                    mensagem += msgErro("Nº Banco", "e informe o Número do Banco");
                else if (dados.Banco.length < 3 || dados.Banco.length > 5)
                    mensagem += msgErro("Nº Banco", "Informe um número entre 3 a 5 dígitos para o Número do Banco.");
                
                if (isCampoVazio(dados.Agencia)) 
                    mensagem += msgErro("Agência", "e informe a Agência");
                break;

            case 3:
                if (isCampoVazio(dados.Ipef)) 
                    mensagem += msgErro("IPEF", "e informe Número do CNPJ da Instituição de pagamento Eletrônico do Frete");
                break;
        }
    }

    /**
     * Validacao MDFe 3.00
     */
    if ($("body").hasClass("mdfe-300")) {
        // Validar contratantes
        // Validar ciots
        // Validar seguros
    }

    return mensagem;
}

function validarInformacoesDePagamentoDoFrete() {
    const docQuantidade = retornarQuantidadeDeDocumentos();
    const tipoEmitente = $("#selTipoEmitente").val();
    const veiculo = ObterVeiculo();

    const isVigente = isDataVigente();
    const isAmbienteEmissaoHomologacao = AmbienteEmissao === 'H';
    const isDocumentoValido = docQuantidade === 1;

    if ((isVigente || isAmbienteEmissaoHomologacao) && isDocumentoValido) {
        if (tipoEmitente === "2") { // Não Prestador de Serviço de Transporte
            return veiculo.CPFCNPJ != null && veiculo.CPFCNPJ !== "";
        } 
        return true;
    }
    return false;
}

function isDataVigente() {
    const dataAtual = new Date();
    const dataLimite = new Date(2025, 10, 1);

    return (dataAtual.setHours(0, 0, 0, 0) >= dataLimite.setHours(0, 0, 0, 0));
}

function retornarQuantidadeDeDocumentos() {
    var totalDocs = 0;
    var municipiosDescarregamento = $("body").data("municipiosDescarregamento");

    if (municipiosDescarregamento !== undefined && municipiosDescarregamento !== null) {
        for (var i = 0; i < municipiosDescarregamento.length; i++) {
            var docCount = 0;

            // Verificando e somando os documentos, validando se 'Excluir' é false
            if (municipiosDescarregamento[i].Documentos && municipiosDescarregamento[i].Documentos.length > 0) {
                for (var y = 0; y < municipiosDescarregamento[i].Documentos.length; y++) {
                    if (!municipiosDescarregamento[i].Documentos[y].Excluir) {
                        docCount++;
                    }
                }
            }

            // Verificando e somando as NFes, validando se 'Excluir' é false
            if (municipiosDescarregamento[i].NFes && municipiosDescarregamento[i].NFes.length > 0) {
                for (var y = 0; y < municipiosDescarregamento[i].NFes.length; y++) {
                    if (!municipiosDescarregamento[i].NFes[y].Excluir) {
                        docCount++;
                    }
                }
            }

            // Verificando e somando as ChaveCTes, validando se 'Excluir' é false
            if (municipiosDescarregamento[i].ChaveCTes && municipiosDescarregamento[i].ChaveCTes.length > 0) {
                for (var y = 0; y < municipiosDescarregamento[i].ChaveCTes.length; y++) {
                    if (!municipiosDescarregamento[i].ChaveCTes[y].Excluir) {
                        docCount++;
                    }
                }
            }

            totalDocs += docCount;
        }
    }

    return totalDocs;
}

function msgErro(campo, detalhe) {
    return `<b>${campo}</b> inválido. (Acesse a aba Rodoviário | Informações de Pagamento do Frete ${detalhe}).<br/>`;
}

function isCampoVazio(valor) {
    return !valor || valor.trim() === "";
}

function BuscarDadosEmpresa() {
    executarRest("/Empresa/ObterDetalhesDaEmpresaDoUsuario?callback=?", {}, function (r) {
        if (r.Sucesso) {
            EmissaoCTe = r.Objeto.EmissaoCTe;
            AmbienteEmissao = r.Objeto.Emissao;
            _permitirMultiplasParcelas = r.Objeto.TipoEmissorDocumentoMDFe;

            ClassesVersao(EmissaoCTe);
            ConsultarMDFes();
            AbrirMDFePorParametros();
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
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

function AbrirMDFePorParametros() {
    var mdfe = parseInt(GetUrlParam("mdfe"));
    var serie = parseInt(GetUrlParam("serie"));

    if (mdfe > 0 && serie > 0) {
        var dados = {
            Numero: mdfe,
            Serie: serie
        };
        executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterCodigoPorNumeroESerie?callback=?", dados, function (r) {
            if (r.Sucesso && r.Objeto.Codigo > 0) {
                EditarMDFe({
                    data: {
                        Codigo: r.Objeto.Codigo
                    }
                }, r.Objeto.Status == 0);
            }
        });
    }
}

function SetarLabelVersao() {
    var $body = $("body");
    var cls = '';

    if ($body.hasClass("mdfe-100"))
        cls = '1.00';
    else if ($body.hasClass("mdfe-300"))
        cls = '3.00';

    $("#spnLabelVersao").text(' (' + cls + ')');
}

function ClassesVersao(versoes) {
    // Adiciona classe de versao cte e mdfe
    var classesCTe = ["cte-200", "cte-400"];
    var classesMDFe = ["mdfe-100", "mdfe-300"];
    var $body = $("body");

    if ("VersaoCTe" in versoes) {
        for (var i in classesCTe) $body.removeClass(classesCTe[i]);
        $body.addClass("cte-" + (versoes.VersaoCTe.replace('.', '')));
    }

    if ("VersaoMDFe" in versoes) {
        for (var i in classesMDFe) $body.removeClass(classesMDFe[i]);
        $body.addClass("mdfe-" + (versoes.VersaoMDFe.replace('.', '')));
    }

    SetarLabelVersao();
    // ClassesVersao({VersaoCTe: "", VersaoMDFe: ""});
}

function BuscaMenorProximoCodigo(lista, key) {
    if (!key) key = "Id";
    var proximoCodigo = 0;

    for (var i in lista) {
        var item = lista[i];

        if (item[key] < 0) {
            var codigoAtual = Math.abs(item[key]);
            if (codigoAtual > proximoCodigo)
                proximoCodigo = codigoAtual;
        }
    }

    return -(proximoCodigo + 1);
}

function ConfigurarComponentesDeInfPagamento() {
    desabilitarCamposInfPgto();

    $(".row-banco, .row-pix, .row-ipef").hide();
    $("#li-tab-bancarias").hide();

    CalcularValorProximaParcela()
    atualizarComponentesBancarios(false);
    atualizarComponentesDeParcelamento();
}

function atualizarComponentesBancarios(tabShow = true) {
    var tipo = $("#selInformacoesBancarias").val();

    $(".row-banco, .row-pix, .row-ipef").hide();

    switch (tipo) {
        case "1": // Chave PIX
            $("#txtNumeroBanco, #txtAgencia, #txtIPEF").val("");
            $("#txtChavePix").attr("required", true);
            $(".row-pix").show();
            break;
        case "2": // Banco e Agencia
            $("#txtChavePix, #txtIPEF").val("");
            $("#txtNumeroBanco, #txtAgencia").attr("required", true);
            $(".row-banco").show();
            break;
        case "3": // IPEF
            $("#txtNumeroBanco, #txtAgencia, #txtChavePix").val("");
            $("#txtIPEF").attr("required", true);
            $(".row-ipef").show();
            break;
    }
}

function atualizarComponentesDeParcelamento() {
    if (telaSomenteLeitura()) 
        return;

    if ($("#selFormaPagamento").val() === "0") {
        StateParcela.clear();
        RenderizarParcelasGrid();
        $("#txtNumeroDaParcela").val(1);
        $("#txtVencimentoDaParcela").val(DataVencimentoProximaParcela());
    } else
        CalcularValorProximaParcela();
}

function telaSomenteLeitura() {
    return $("#txtDataEmissao").prop("disabled");
}

function desabilitarCamposInfPgto() {
    $("#txtNumeroDaParcela, #txtValorDaParcela").attr("disabled", true);
}