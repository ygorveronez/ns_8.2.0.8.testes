function BuscarUFs(idSelect) {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {
            RenderizarUFs(r.Objeto, idSelect);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}
function RenderizarUFs(ufs, idSelect) {
    var selUFs = document.getElementById(idSelect);
    selUFs.options.length = 0;
    for (var i = 0; i < ufs.length; i++) {
        var optn = document.createElement("option");
        optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
        optn.value = ufs[i].Sigla;
        selUFs.options.add(optn);
    }
}

function AdicionarSerieUF() {

    var seriesUF = $("body").data("seriesUF") == null ? new Array() : $("body").data("seriesUF");

    var siglaUF = $("#selUF").val();
    var codigoSerie = $("#txtSeriePorUF").data("codigo");
    var serie = $("#txtSeriePorUF").val();

    var serieUF = {
        SiglaUF: siglaUF,
        CodigoSerie: codigoSerie,
        Serie: serie,
        Excluir: false
    }

    seriesUF.push(serieUF);

    $("body").data("seriesUF", seriesUF);

    $("#selUF").val($("#selUF option:first").val());
    $("#txtSeriePorUF").val("");

    RenderizarSeriesUF();
}

function RenderizarSeriesUF() {
    var seriesUF = $("body").data("seriesUF") == null ? new Array() : $("body").data("seriesUF");

    $("#tblSeriesUF tbody").html("");

    for (var i = 0; i < seriesUF.length; i++) {
        if (!seriesUF[i].Excluir)
            $("#tblSeriesUF tbody").append("<tr><td>" + seriesUF[i].SiglaUF + "</td><td>" + seriesUF[i].Serie + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirSerieUF(" + JSON.stringify(seriesUF[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblSeriesUF tbody").html() == "")
        $("#tblSeriesUF tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function ExcluirSerieUF(serie) {
    var seriesUF = $("body").data("seriesUF") == null ? new Array() : $("body").data("seriesUF");

    for (var i = 0; i < seriesUF.length; i++) {
        if (seriesUF[i].CodigoSerie == serie.CodigoSerie) {
            if (serie.CodigoSerie <= 0)
                seriesUF.splice(i, 1);
            else
                seriesUF[i].Excluir = true;
            break;
        }
    }

    $("body").data("seriesUF", seriesUF);

    RenderizarSeriesUF();
}

function ValidarSerieUF() {
    var seriesUF = $("body").data("seriesUF") == null ? new Array() : $("body").data("seriesUF");
    var siglaUF = $("#selUF").val();
    var valido = true;

    if ($("#txtSeriePorUF").val() == "") {
        CampoComErro("#txtSeriePorUF");
        jAlert("Necessário informar uma Série!", "Atenção!");
        valido = false;
    } else {
        CampoSemErro("#txtSeriePorUF");
        for (var i = 0; i < seriesUF.length; i++) {
            if (seriesUF[i].SiglaUF == siglaUF && seriesUF[i].Excluir == false) {
                CampoComErro("#selUF");
                jAlert("Estado já foi adicionado!", "Atenção!");
                valido = false;
                break;
            } else {
                CampoSemErro("#selUF");
            }
        }
    }
    return valido;
}

function RetornoConsultaClienteSerie(cliente) {
    BuscarClienteSerie(cliente.CPFCNPJ);
}

function BuscarClienteSerie(cpfCnpj) {
    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
            if (r.Sucesso) {
                if (r.Objeto != null) {
                    $("#txtClienteSerie").val(r.Objeto.CPF_CNPJ).data("CodigoCliente", r.Objeto.CPF_CNPJ.replace(/[^0-9]/g, ''));
                }
            } else {
                jAlert(r.Erro, "Erro");
            }
        });
    } else {
        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
    }
}


function AdicionarSerieCliente() {

    var seriesCliente = $("body").data("seriesCliente") == null ? new Array() : $("body").data("seriesCliente");

    var cnpjCliente = $("#txtClienteSerie").data("CodigoCliente");
    var codigoSerie = $("#txtSeriePorCliente").data("codigo");
    var serie = $("#txtSeriePorCliente").val();
    var tipoCliente = $("#selTipoClienteSerie :selected").val();
    var descricaoTipoCliente = "";
    if (tipoCliente == 0)
        descricaoTipoCliente = "Remetente";
    else if (tipoCliente == 1)
        descricaoTipoCliente = "Expedidor";
    else if (tipoCliente == 2)
        descricaoTipoCliente = "Recebedor";
    else if (tipoCliente == 3)
        descricaoTipoCliente = "Destinatário";
    else
        descricaoTipoCliente = "Tomador";

    var raizCNPJ = "Não";
    if ($("#chkSerieClienteRaizCNPJ").prop('checked'))
        raizCNPJ = "Sim";


    var serieCliente = {
        CnpjCliente: cnpjCliente,
        CodigoSerie: codigoSerie,
        Serie: serie,
        TipoCliente: tipoCliente,
        DescricaoTipoCliente: descricaoTipoCliente,
        RaizCNPJ: raizCNPJ,
        Excluir: false
    }

    seriesCliente.push(serieCliente);

    $("body").data("seriesCliente", seriesCliente);

    $("#txtClienteSerie").val("").data("CodigoCliente", null);
    $("#txtSeriePorCliente").val("");

    RenderizarSeriesCliente();
}

function RenderizarSeriesCliente() {
    var seriesCliente = $("body").data("seriesCliente") == null ? new Array() : $("body").data("seriesCliente");

    $("#tblSeriesCliente tbody").html("");
   
    for (var i = 0; i < seriesCliente.length; i++) {
        if (!seriesCliente[i].Excluir)
            $("#tblSeriesCliente tbody").append("<tr><td>" + seriesCliente[i].CnpjCliente + "</td><td>" + seriesCliente[i].RaizCNPJ + "</td><td>" + seriesCliente[i].DescricaoTipoCliente + "</td><td>" + seriesCliente[i].Serie + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirSerieCliente(" + JSON.stringify(seriesCliente[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblSeriesCliente tbody").html() == "")
        $("#tblSeriesCliente tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

function ExcluirSerieCliente(serie) {
    var seriesCliente = $("body").data("seriesCliente") == null ? new Array() : $("body").data("seriesCliente");

    for (var i = 0; i < seriesCliente.length; i++) {
        if (seriesCliente[i].CodigoSerie == serie.CodigoSerie) {
            if (serie.CodigoSerie <= 0)
                seriesCliente.splice(i, 1);
            else
                seriesCliente[i].Excluir = true;
            break;
        }
    }

    $("body").data("seriesCliente", seriesCliente);

    RenderizarSeriesCliente();
}

function ValidarSerieCliente() {
    var seriesCliente = $("body").data("seriesCliente") == null ? new Array() : $("body").data("seriesCliente");
    var cnpjCliente = $("#txtClienteSerie").val();
    var valido = true;

    if ($("#txtSeriePorCliente").val() == "") {
        CampoComErro("#txtSeriePorCliente");
        jAlert("Necessário informar uma Série!", "Atenção!");
        valido = false;
    }

    return valido;
}