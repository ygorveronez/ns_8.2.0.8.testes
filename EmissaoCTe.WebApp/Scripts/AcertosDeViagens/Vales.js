$(document).ready(function () {
    $("#txtValorVale").priceFormat({ prefix: '' });
    $("#txtDataVale").mask("99/99/9999");
    $("#txtDataVale").datepicker();

    $("#btnSalvarVale").click(function () {
        SalvarVale();
    });

    $("#btnExcluirVale").click(function () {
        ExcluirVale();
    });

    $("#btnCancelarVale").click(function () {
        LimparCamposVale();
    });

    LimparCamposVale();
});

function SetarDadosPadroesVale() {
    $("#txtDataVale").val(Globalize.format(new Date(), "dd/MM/yyyy"));
}

function LimparCamposVale() {
    $("body").data("vale", null);
    $("#txtNumeroVale").val("0");
    $("#txtDataVale").val("");
    $("#txtValorVale").val("0,00");
    $("#txtDescricaoVale").val("");
    $("#txtObservacaoVale").val("");
    $("#selTipoVale").val($("#selTipoVale option:first").val());
    $("#btnExcluirVale").hide();

    SetarDadosPadroesVale();
}

function ValidarCamposVale() {
    var data = $("#txtDataVale").val();
    var valor = Globalize.parseFloat($("#txtValorVale").val());
    var descricao = $("#txtDescricaoVale").val();
    var valido = true;

    if (data == null || data == "") {
        CampoComErro("#txtDataVale");
        valido = false;
    } else {
        CampoSemErro("#txtDataVale");
    }

    if (isNaN(valor) || valor <= 0) {
        CampoComErro("#txtValorVale");
        valido = false;
    } else {
        CampoSemErro("#txtValorVale");
    }

    if (descricao == null || descricao == "") {
        CampoComErro("#txtDescricaoVale");
        valido = false;
    } else {
        CampoSemErro("#txtDescricaoVale");
    }

    return valido;
}

function SalvarVale() {
    if (ValidarCamposVale()) {
        var codigo = $("body").data("vale") != null ? $("body").data("vale").Codigo : 0;
        var vale = {
            Codigo: codigo,
            Numero: codigo == 0 ? NumeroSequencialVale() : $("#txtNumeroVale").val(),
            Descricao: $("#txtDescricaoVale").val(),
            Tipo: $("#selTipoVale").val(),
            DescricaoTipo: $("#selTipoVale option:selected").text(),
            Data: $("#txtDataVale").val(),
            Observacao: $("#txtObservacaoVale").val(),
            Valor: $("#txtValorVale").val(),
            Excluir: false
        };

        var vales = $("body").data("vales") == null ? new Array() : $("body").data("vales");
        var inserindo = codigo == 0;
        vales.sort(function (a, b) { return a.Numero < b.Numero ? -1 : 1; });

        if (vale.Codigo == 0)
            vale.Codigo = (vales.length > 0 ? (vales[0].Codigo > 0 ? -1 : (vales[0].Codigo - 1)) : -1);

        if (!inserindo && vales.length > 0) {
            for (var i = 0; i < vales.length; i++) {
                if (vales[i].Codigo == vale.Codigo) {
                    vales[i] = vale;
                    break;
                }
            }
        } else {
            vales.push(vale);
        }

        $("body").data("vales", vales);

        RenderizarVales();
        LimparCamposVale();
        AtualizarValores();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "mensagensVales-placeholder");
    }
}

function NumeroSequencialVale() {
    var itens = $("body").data("vales") == null ? [] : $("body").data("vales");
    // Remove excluidos
    var itensReais = itens.filter(function (vale) { return !vale.Excluir; });

    return itensReais.length + 1;
}

function EditarVale(vale) {
    LimparCamposVale();

    $("body").data("vale", vale);
    $("#txtNumeroVale").val(vale.Numero);
    $("#txtDataVale").val(vale.Data);
    $("#txtValorVale").val(vale.Valor);
    $("#selTipoVale").val(vale.Tipo);
    $("#txtDescricaoVale").val(vale.Descricao);
    $("#txtObservacaoVale").val(vale.Observacao);
    $("#btnExcluirVale").show();
}

function ExcluirVale() {
    jConfirm("Deseja realmente excluir este vale?", "Atenção", function (r) {
        if (r) {
            var codigo = $("body").data("vale").Codigo;

            var vales = $("body").data("vales") == null ? new Array() : $("body").data("vales");

            for (var i = 0; i < vales.length; i++) {
                if (vales[i].Codigo == codigo) {
                    if (codigo > 0) {
                        vales[i].Excluir = true;
                    } else {
                        vales.splice(i, 1);
                    }
                    break;
                }
            }

            // Recalcula o numero sequencia
            var ultimoNumero = 0;
            vales = vales.map(function (vale) {
                if (!vale.Excluir) 
                    vale.Numero = ++ultimoNumero;

                return vale;
            });
            $("body").data("vales", vales);

            RenderizarVales();
            LimparCamposVale();
            AtualizarValores();
        }
    });
}

function RenderizarVales() {
    var itens = $("body").data("vales") == null ? new Array() : $("body").data("vales");
    var $tabela = $("#tblVales");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.Numero + "</td>" +
                "<td>" + info.Data + "</td>" +
                "<td>" + info.Descricao + "</td>" +
                "<td>" + info.DescricaoTipo + "</td>" +
                "<td>" + info.Valor + "</td>" +
                "<td>" +
                    "<button type='button' class='btn btn-default btn-xs recibo'>Recibo</button> " +
                    "<button type='button' class='btn btn-default btn-xs editar'>Editar</button> " +
                "</td>" +
            "</tr>");

            $row.on("click", ".editar", function () {
                EditarVale(info);
            });
            $row.on("click", ".recibo", function () {
                ReciboVale(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function BuscarVales(acertoDeViagem) {
    executarRest("/ValeDoAcertoDeViagem/BuscarPorAcertoDeViagem?callback=?", { CodigoAcertoViagem: acertoDeViagem.Codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("vales", r.Objeto);
            RenderizarVales();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "mensagensVales-placeholder");
        }
    });
}

function ReciboVale(vale) {
    executarDownload("/ValeDoAcertoDeViagem/DownlaodRebibo?callback=?", { Vale: vale.Codigo, AcertoViagem: $("#hddCodigoAcertoViagem").val() });
}