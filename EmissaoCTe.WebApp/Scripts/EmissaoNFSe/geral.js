/// <reference path="emissaoNFSe.js" />
/// <reference path="itens.js" />
/// <reference path="tomador.js" />
/// <reference path="intermediario.js" />
/// <reference path="consultaNFSe.js" />

var janelaDicas = null, emiteNFSeForaEmbarcador = false;

$(document).ready(function () {

    $("#btnImportarCSV").hide();
    $("#btnImportarNFeSalvas").hide();

    FormatarCampos();

    ObterEstados();
    ObterPaises();
    ObterSeries();
    ObterNaturezas();
    ObterConfiguracoesEmissao();

    $("#btnNovaNFSe").click(function () {
        AbrirTelaEmissaoNFSe();

        if (janelaDicas != null)
            janelaDicas.close();
    });

    $("#btnFecharEmissaoNFSe").click(function () {
        FecharTelaEmissaoNFSe();
    });

    $("#btnAbrirDicaEmissaoNFSe").click(function () {
        AbrirDicaParaEmissaoDeNFSe(false);
    });

    $("#btnAbrirDica").click(function () {
        AbrirDicaNovaJanela(false, null);
    });

    // Fix #5309
    $("#divEmissaoNFSe").on("hidden.bs.modal", function () {
        LimparCamposNFSe();
        $('a[href="#tabGeral"]').click();
    });
    // End Fix #5309
});

function ObterConfiguracoesEmissao() {
    executarRest("/NotaFiscalDeServicosEletronica/ObterConfiguracoes?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("body").data("configuracao", r.Objeto);

            if (r.Objeto.PermitirImportarCSV)
                $("#btnImportarCSV").show();
            else
                $("#btnImportarCSV").hide();

            if (r.Objeto.PermitirImportarNFeSalvas)
                $("#btnImportarNFeSalvas").show();
            else
                $("#btnImportarNFeSalvas").hide();

            if (r.Objeto.PermiteImportarXMLNFSe)
                $("#btnImportarXMLNFSe").show();
            else
                $("#btnImportarXMLNFSe").hide();

            if (r.Objeto.EmiteNFSeForaEmbarcador) {
                emiteNFSeForaEmbarcador = true;
                $("#txtNumero").prop("disabled", false);
            }
        } else
            ExibirMensagemErro(r.Erro, "Atenção!");
    });
}

function FormatarCampos() {
    $("#txtDataEmissao").datepicker({});
    $("#txtHoraEmissao").mask("99:99");
    $("#txtDocumentoDataEmissao").datepicker({});
    $("#txtDocumentoDataEmissao").mask("99/99/9999");
    $("#txtDocumentoSerie").mask("9?99", { placeholder: "   " });
    $("#txtDocumentoNumero").mask("9?99999999", { placeholder: "         " });
    $("#txtDocumentoChave").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");
    $("#txtDocumentoPeso").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtDocumentoValor").priceFormat({ prefix: '' });
    $("#txtNumeroSubstituicao").mask("9?9999999999");
    $("#txtValorServicos").priceFormat();
    $("#txtValorDeducoes").priceFormat();
    $("#txtValorPIS").priceFormat();
    $("#txtValorCOFINS").priceFormat();
    $("#txtValorINSS").priceFormat();
    $("#txtValorIR").priceFormat();
    $("#txtValorCSLL").priceFormat();
    $("#txtValorISSRetido").priceFormat();
    $("#txtValorOutrasRetencoes").priceFormat();
    $("#txtValorDescontoIncondicionado").priceFormat();
    $("#txtValorDescontoCondicionado").priceFormat();
    $("#txtAliquotaISS").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtBaseCalculoISS").priceFormat();
    $("#txtValorISS").priceFormat();
    $("#txtValorIBSEstadual").priceFormat();
    $("#txtValorIBSMunicipal").priceFormat();
    $("#txtValorCBS").priceFormat();

    $("#selEstadoPrestacaoServico").change(function () {
        BuscarLocalidades($(this).val(), 'selLocalidadePrestacaoServico');
    });
}

function LimparCamposNFSe() {
    $("body").data("NFSe", null);
    $("#selSerie").val($("#selSerie option:first").val());

    SetarDataEmissao();

    $("body").data("itens", []);
    RenderizarItens();
    LimparCamposItem();

    if (emiteNFSeForaEmbarcador)
        $("#txtNumero").val("");
    else
        $("#txtNumero").val("Automático");
    $("#txtNumeroRPS").val("Automático");
    $("#txtNumeroRPS").prop("disabled", true);
    $("#txtNumeroSubstituicao").val("");
    $("#txtSerieSubstituicao").val("");
    $("#txtValorServicos").val("0,00");
    $("#txtValorDeducoes").val("0,00");
    $("#txtValorPIS").val("0,00");
    $("#txtValorCOFINS").val("0,00");
    $("#txtValorINSS").val("0,00");
    $("#txtValorIR").val("0,00");
    $("#txtValorCSLL").val("0,00");
    $("#selISSRetido").val($("#selISSRetido option:first").val());
    $("#txtValorISSRetido").val("0,00");
    $("#txtValorIBSEstadual").val("0,00");
    $("#txtValorIBSMunicipal").val("0,00");
    $("#txtValorCBS").val("0,00");
    $("#txtValorOutrasRetencoes").val("0,00");
    $("#txtValorDescontoIncondicionado").val("0,00");
    $("#txtValorDescontoCondicionado").val("0,00");
    $("#txtAliquotaISS").val("0,0000");
    $("#txtBaseCalculoISS").val("0,00");
    $("#txtValorISS").val("0,00");
    $("#txtOutrasInformacoes").val("");
    $("#selNatureza").val("");
    $("#lblLogNFSe").html("");

    LimparCamposTomador();
    LimparCamposIntermediario();
    LimparCamposDocumento();
    StateDocumentos.clear();

    if ($("body").data("configuracao") != null) {

        $("#selEstadoPrestacaoServico").val($("body").data("configuracao").Estado);

        BuscarLocalidades($("body").data("configuracao").Estado, "selLocalidadePrestacaoServico", $("body").data("configuracao").Cidade);

    } else {
        $("#selLocalidadePrestacaoServico").html("");
        $("#selEstadoPrestacaoServico").val("");
    }

}

function SetarDataEmissao() {
    var obsPadrao = $("body").data("configuracao").ObservacaoPadrao;
    $("#txtDataEmissao").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtHoraEmissao").val(Globalize.format(new Date(), "HH:mm"));

    if (obsPadrao != "")
        $("#txtOutrasInformacoes").val(obsPadrao);
}

function AbrirTelaEmissaoNFSe(edicao) {

    if (edicao == null || !edicao) {
        LimparCamposNFSe();
        SetarDataEmissao();
    }

    $("#divEmissaoNFSe").modal({ keyboard: false, backdrop: 'static' });

    AbrirDicaParaEmissaoDeNFSe(true);

}

function FecharTelaEmissaoNFSe() {
    $("#divEmissaoNFSe").modal('hide');
    VoltarAoTopoDaTela();
}

function ObterSeries() {
    executarRest("/Usuario/ObterSeriesDoUsuario?callback=?", { Tipo: 2 }, function (r) {
        if (r.Sucesso) {

            var selSerie = document.getElementById("selSerie");
            var selSerieNFSeFiltro = document.getElementById("selSerieNFSeFiltro");

            selSerie.options.length = 0;
            selSerieNFSeFiltro.options.length = 0;

            var optnTodos = document.createElement("option");
            optnTodos.text = "Todas";
            optnTodos.value = "";

            selSerieNFSeFiltro.options.add(optnTodos);

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Numero;
                optn.value = r.Objeto[i].Codigo;

                selSerie.options.add(optn);
                selSerieNFSeFiltro.add(optn.cloneNode(true));
            }

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function BuscarLocalidades(uf, idSelect, codigo) {
    executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
        if (r.Sucesso) {
            RenderizarLocalidades(r.Objeto, idSelect, codigo);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarLocalidades(localidades, idSelect, codigo) {
    var locs = new Array();

    if (typeof idSelect == 'string')
        locs.push(idSelect);
    else
        locs = idSelect;

    for (var x = 0; x < locs.length; x++) {

        var selLocalidades = document.getElementById(locs[x]);
        selLocalidades.options.length = 0;

        for (var i = 0; i < localidades.length; i++) {

            var optn = document.createElement("option");
            optn.text = localidades[i].Descricao;
            optn.value = localidades[i].Codigo;

            selLocalidades.options.add(optn);
        }

        if (codigo != null)
            $(selLocalidades).val(codigo).change();
    }
}

function ObterEstados() {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selEstadoTomador = document.getElementById("selEstadoTomador");
            var selEstadoIntermediario = document.getElementById("selEstadoIntermediario");
            var selEstadoItem = document.getElementById("selEstadoItem");
            var selEstadoIncidenciaItem = document.getElementById("selEstadoIncidenciaItem");
            var selEstadoPrestacaoServico = document.getElementById("selEstadoPrestacaoServico");

            selEstadoTomador.options.length = 0;
            selEstadoIntermediario.options.length = 0;
            selEstadoItem.options.length = 0;
            selEstadoIncidenciaItem.options.length = 0;
            selEstadoPrestacaoServico.options.length = 0;

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Sigla + " - " + r.Objeto[i].Nome;
                optn.value = r.Objeto[i].Sigla;

                selEstadoTomador.options.add(optn);
                selEstadoIntermediario.options.add(optn.cloneNode(true));
                selEstadoItem.options.add(optn.cloneNode(true));
                selEstadoIncidenciaItem.options.add(optn.cloneNode(true));
                selEstadoPrestacaoServico.options.add(optn.cloneNode(true));
            }

            $("#selEstadoTomador").val("");
            $("#selEstadoIntermediario").val("");
            $("#selEstadoItem").val("");
            $("#selEstadoIncidenciaItem").val("");
            $("#selEstadoPrestacaoServico").val("");

        } else {

            ExibirMensagemErro(r.Erro, "Atenção");

        }
    });
}

function ObterPaises() {
    executarRest("/Pais/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selPaisTomador = document.getElementById("selPaisTomador");
            var selPaisIntermediario = document.getElementById("selPaisIntermediario");
            var selPaisItem = document.getElementById("selPaisItem");

            selPaisTomador.options.length = 0;
            selPaisIntermediario.options.length = 0;
            selPaisItem.options.length = 0;

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Nome;
                optn.value = r.Objeto[i].Sigla;

                selPaisTomador.options.add(optn);
                selPaisIntermediario.options.add(optn.cloneNode(true));
                selPaisItem.options.add(optn.cloneNode(true));
            }

            $("#selPaisTomador").val("");
            $("#selPaisIntermediario").val("");
            $("#selPaisItem").val("");

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ObterNaturezas() {
    executarRest("/NaturezaNFSe/ObterNaturezasDaEmpresa?callback=?", {}, function (r) {
        if (r.Sucesso) {
            var selNatureza = document.getElementById("selNatureza");

            selNatureza.options.length = 0;

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Codigo;

                selNatureza.options.add(optn);
            }

            $("#selNatureza").val("");
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function AbrirDicaParaEmissaoDeNFSe(somenteSeExistir) {
    var dicasEmissao = $("body").data("configuracao").DicasEmissao;
    var abrirModal = false;

    if (dicasEmissao != null && dicasEmissao != "") {
        var mensagemDicas = dicasEmissao.replace(/(?:\r\n|\r|\n)/g, '<br />');
        $("#divDica").html("");
        $("#divDica").append(mensagemDicas);
        abrirModal = true;
    } else if (somenteSeExistir == null || somenteSeExistir == false)
        jAlert("Nenhuma dica registrada.", "Dicas Para a Emissão");

    //if ($.isArray(configuracaoEmpresa.ArquivosDicas) && configuracaoEmpresa.ArquivosDicas.length > 0) {
    //    $("#btnAbrirArquivosDica").show();
    //    $("#divArquivosDicas").hide();
    //    RenderizarArquivosDicas(configuracaoEmpresa.ArquivosDicas);
    //    abrirModal = true;
    //} else {
    $("#btnAbrirArquivosDica").hide();
    $("#divArquivosDicas").hide();
    //}

    if (abrirModal) {
        $("#divDicasNFSe").modal("show");
    }
}

function AbrirDicaNovaJanela(somenteSeExistir, configuracao) {
    var dicasEmissao = $("body").data("configuracao").DicasEmissao;

    if (dicasEmissao != null && dicasEmissao != "") {
        var posicao = (screen.height - 400) / 2;
        janelaDicas = window.open("", "", "width=900, height=500, resizable=yes, titlebar=yes, scrollbars=yes, top=" + posicao);

        var mensagemDicas = '<b style="font-size: 20px">Dicas para a Emissão do CT-e</b><br><br>' + dicasEmissao.replace(/(?:\r\n|\r|\n)/g, '<br />');
        mensagemDicas = '<div style="font-size: 15px; margin-right: 5px; margin-bottom: 5px; margin-left: 10px;">' + mensagemDicas + '</div>';

        $("#divDicasNFSe").modal("hide");
        janelaDicas.document.write(mensagemDicas);
    }
}