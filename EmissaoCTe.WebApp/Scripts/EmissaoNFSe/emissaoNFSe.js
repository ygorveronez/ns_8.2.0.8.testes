/// <reference path="geral.js" />

$(document).ready(function () {
    $("#btnEmitirNFSe").click(function () {
        SalvarNFSe(true);

        if (janelaDicas != null)
            janelaDicas.close();
    });

    $("#btnSalvarNFSe").click(function () {
        SalvarNFSe(false);

        if (janelaDicas != null)
            janelaDicas.close();
    });

    $("#btnCancelarNFSe").click(function () {
        FecharTelaEmissaoNFSe();

        if (janelaDicas != null)
            janelaDicas.close();
    });
});

function SalvarNFSe(emitir) {
    if (ValidarDadosNFSe()) {
        var dados = {
            Codigo: $("body").data("NFSe") != null ? $("body").data("NFSe").Codigo : 0,
            Numero: $("#txtNumero").val(),
            Serie: $("#selSerie").val(),
            DataEmissao: $("#txtDataEmissao").val(),
            HoraEmissao: $("#txtHoraEmissao").val(),
            Tomador: JSON.stringify(ObterDadosTomador()),
            Intermediario: JSON.stringify(ObterDadosIntermediario()),
            Itens: JSON.stringify($("body").data("itens")),
            NumeroSubstituicao: $("#txtNumeroSubstituicao").val(),
            SerieSubstituicao: $("#txtSerieSubstituicao").val(),
            ValorServicos: $("#txtValorServicos").val(),
            ValorDeducoes: $("#txtValorDeducoes").val(),
            ValorPIS: $("#txtValorPIS").val(),
            ValorCOFINS: $("#txtValorCOFINS").val(),
            ValorINSS: $("#txtValorINSS").val(),
            ValorIR: $("#txtValorIR").val(),
            ValorCSLL: $("#txtValorCSLL").val(),
            ISSRetido: $("#selISSRetido").val(),
            ValorISSRetido: $("#txtValorISSRetido").val(),
            ValorOutrasOperacoes: $("#txtValorOutrasRetencoes").val(),
            ValorDescontoIncondicionado: $("#txtValorDescontoIncondicionado").val(),
            ValorDescontoCondicionado: $("#txtValorDescontoCondicionado").val(),
            AliquotaISS: $("#txtAliquotaISS").val(),
            BaseCalculoISS: $("#txtBaseCalculoISS").val(),
            ValorISS: $("#txtValorISS").val(),
            OutrasInformacoes: $("#txtOutrasInformacoes").val(),
            LocalidadePrestacaoServico: $("#selLocalidadePrestacaoServico").val(),
            EstadoPrestacaoServico: $("#selEstadoPrestacaoServico").val(),
            Natureza: $("#selNatureza").val(),
            Documentos: StateDocumentos.toJson(),
            NumeroRPS: $("#txtNumeroRPS").val(),
            Emitir: emitir,
            ValorIBSEstadual: $("#txtValorIBSEstadual").val(),
            ValorIBSMunicipal: $("#txtValorIBSMunicipal").val(),
            ValorCBS: $("#txtValorCBS").val()
        };

        executarRest("/NotaFiscalDeServicosEletronica/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {

                FecharTelaEmissaoNFSe();

                ExibirMensagemSucesso("Sucesso!", "NFS-e " + (emitir ? "emitida" : "salva") + " com sucesso!");

                ConsultarNFSes();

            } else {

                ExibirMensagemErro(r.Erro, "Falha!", "placeholder-msgEmissaoNFSe");

            }
        });
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoNFSe");
    }
}

function ValidarDadosNFSe() {
    var serie = $("#selSerie").val();
    var dataEmissao = $("#txtDataEmissao").val();
    var horaEmissao = $("#txtHoraEmissao").val();
    var valorServicos = Globalize.parseFloat($("#txtValorServicos").val());
    var localidadePrestacaoServico = $("#selLocalidadePrestacaoServico").val();
    var natureza = $("#selNatureza").val();

    var valido = true;

    if (serie == null || serie == "") {
        CampoComErro("#selSerie");
        valido = false;
    } else {
        CampoSemErro("#selSerie");
    }

    if (dataEmissao == null || dataEmissao == "") {
        CampoComErro("#txtDataEmissao");
        valido = false;
    } else {
        CampoSemErro("#txtDataEmissao");
    }

    if (horaEmissao == null || horaEmissao == "") {
        CampoComErro("#txtHoraEmissao");
        valido = false;
    } else {
        CampoSemErro("#txtHoraEmissao");
    }

    if (localidadePrestacaoServico == null || localidadePrestacaoServico == "") {
        CampoComErro("#selLocalidadePrestacaoServico");
        valido = false;
    } else {
        CampoSemErro("#selLocalidadePrestacaoServico");
    }

    if (natureza == null || natureza == "") {
        CampoComErro("#selNatureza");
        valido = false;
    } else {
        CampoSemErro("#selNatureza");
    }

    if (isNaN(valorServicos) || valorServicos <= 0) {
        CampoComErro("#txtValorServicos");
        valido = false;
    } else {
        CampoSemErro("#txtValorServicos");
    }

    return valido;
}

function ObterDadosTomador() {
    if ($("#txtCPFCNPJTomador").val() == "")
        return null;

    var tomador = {
        Exportacao: $("#chkTomadorExportacao").prop("checked"),
        CPFCNPJ: $("#txtCPFCNPJTomador").val(),
        NumeroDocumentoExportacao: $("#txtNumeroDocumentoExteriorTomador").val(),
        RGIE: $("#txtRGIETomador").val(),
        IM: $("#txtIMTomador").val(),
        RazaoSocial: $("#txtRazaoSocialTomador").val(),
        NomeFantasia: $("#txtNomeFantasiaTomador").val(),
        Telefone1: $("#txtTelefone1Tomador").val(),
        Telefone2: $("#txtTelefone2Tomador").val(),
        CodigoAtividade: $("body").data("codigoAtividadeTomador"),
        Endereco: $("#txtEnderecoTomador").val(),
        Numero: $("#txtNumeroTomador").val(),
        Bairro: $("#txtBairroTomador").val(),
        Complemento: $("#txtComplementoTomador").val(),
        CEP: $("#txtCEPTomador").val(),
        SiglaPais: $("#selPaisTomador").val(),
        UF: $("#selEstadoTomador").val(),
        Localidade: $("#selCidadeTomador").val(),
        Cidade: $("#txtCidadeTomadorExportacao").val(),
        Emails: $("#txtEmailsTomador").val(),
        StatusEmails: $("#chkStatusEmailsTomador").prop("checked"),
        EmailsContato: $("#txtEmailsContatoTomador").val(),
        StatusEmailsContato: $("#chkStatusEmailsContatoTomador").prop("checked"),
        EmailsContador: $("#txtEmailsContadorTomador").val(),
        StatusEmailsContador: $("#chkStatusEmailsContadorTomador").prop("checked"),
        SalvarEndereco: $("#chkSalvarEnderecoTomador").prop("checked")
    };

    return tomador;
}

function ObterDadosIntermediario() {
    if ($("#txtCPFCNPJIntermediario").val() == "")
        return null;

    var Intermediario = {
        Exportacao: $("#chkIntermediarioExportacao").prop("checked"),
        CPFCNPJ: $("#txtCPFCNPJIntermediario").val(),
        NumeroDocumentoExportacao: $("#txtNumeroDocumentoExteriorIntermediario").val(),
        RGIE: $("#txtRGIEIntermediario").val(),
        IM: $("#txtIMIntermediario").val(),
        RazaoSocial: $("#txtRazaoSocialIntermediario").val(),
        NomeFantasia: $("#txtNomeFantasiaIntermediario").val(),
        Telefone1: $("#txtTelefone1Intermediario").val(),
        Telefone2: $("#txtTelefone2Intermediario").val(),
        CodigoAtividade: $("body").data("codigoAtividadeIntermediario"),
        Endereco: $("#txtEnderecoIntermediario").val(),
        Numero: $("#txtNumeroIntermediario").val(),
        Bairro: $("#txtBairroIntermediario").val(),
        Complemento: $("#txtComplementoIntermediario").val(),
        CEP: $("#txtCEPIntermediario").val(),
        SiglaPais: $("#selPaisIntermediario").val(),
        UF: $("#selEstadoIntermediario").val(),
        Localidade: $("#selCidadeIntermediario").val(),
        Cidade: $("#txtCidadeIntermediarioExportacao").val(),
        Emails: $("#txtEmailsIntermediario").val(),
        StatusEmails: $("#chkStatusEmailsIntermediario").prop("checked"),
        EmailsContato: $("#txtEmailsContatoIntermediario").val(),
        StatusEmailsContato: $("#chkStatusEmailsContatoIntermediario").prop("checked"),
        EmailsContador: $("#txtEmailsContadorIntermediario").val(),
        StatusEmailsContador: $("#chkStatusEmailsContadorIntermediario").prop("checked"),
        SalvarEndereco: $("#chkSalvarEnderecoIntermediario").prop("checked")
    };

    return Intermediario;
}