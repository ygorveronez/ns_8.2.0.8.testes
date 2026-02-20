var path = "";

$(document).ready(function () {
    //$("#txtCTeNumeroInicio, #txtCTeNumeroFim").mask("999999999");
    FormatarCampoDate("txtCTeDataEmissaoInicio");
    FormatarCampoDate("txtCTeDataEmissaoFim");
    $("#txtCTeTomador, #txtCTeRemetente, #txtCTeDestinatario").mask("99999999999?999");
    $modalConsultaCTes = $("#divConsultaCTes");
    $("#txtCte").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoCTe").val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#btnBuscarCte").click(function () {
        if ($("#txtPessoa").val() == "") {
            CampoComErro("#txtPessoa");
            ExibirMensagemAlerta("Necessário adicionar um Tomador!", "Atenção!", "placeholder-mensagem-ctes");
        }
    });

    $("#btnSelecionarCTes").click(function () {
        if (!ConfiguracoesEmpresa.PermiteSelecionarCTeOutroTomador && $("#txtPessoa").val() == "") {
            CampoComErro("#txtPessoa");
            ExibirMensagemAlerta("Necessário adicionar um Tomador!", "Atenção!", "placeholder-mensagem-ctes");
        } else {
            AbrirConsultaCTes();
            CampoSemErro("#txtPessoa");
        }
    });

    $("#btnImportarCTes").click(function () {
        if (!ConfiguracoesEmpresa.PermiteSelecionarCTeOutroTomador && $("#txtPessoa").val() == "") {
            CampoComErro("#txtPessoa");
            ExibirMensagemAlerta("Necessário adicionar um Tomador!", "Atenção!", "placeholder-mensagem-ctes");
        } else {
            CampoSemErro("#txtPessoa");
            InicializarPlUploadCTe();
            AbrirPlUpload();
        }
    });

    $("#btnFinalizarSelecaoCTes").click(function () {
        FinalizarSelecaoCTes();
    });

    $("#btnSelecionarTodosOsCTes").click(function () {
        SelecionarTodosOsCTes();
    });

    $("#btnBuscarCTesConsulta").click(function () {
        ConsultarCTes(RetornoConsultaCTeAvancado);
    });

    $modalConsultaCTes.on('hide.bs.modal', function () {
        $("body").data("ctesSelecionadosConsultaAvancada", []);
    });

    var today = new Date();
    var date = new Date(today);
    date.setDate(today.getDate() - 30);
    $("#txtCTeDataEmissaoInicio").val(Globalize.format(date, "dd/MM/yyyy"));

    LimparCamposCtes();

    if (document.location.pathname.split("/").length > 1) {
        var paths = document.location.pathname.split("/");
        for (var i = 0; (paths.length - 1) > i; i++) {
            if (paths[i] != "") {
                path += "/" + paths[i];
            }
        }
    }
});

var countArquivos = 0;
var MenorIDCte = 0;

function SelecionarTodosOsCTes() {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") || new Array();
    var codigosCtes = ctesSelecionados.map(function (cte) { return cte.Codigo });
    var dados = {
        NumeroInicial: $("#txtCTeNumeroInicio").val(),
        NumeroFinal: $("#txtCTeNumeroFim").val(),
        DataEmissaoInicial: $("#txtCTeDataEmissaoInicio").val(),
        DataEmissaoFinal: $("#txtCTeDataEmissaoFim").val(),
        NumeroDocumento: $("#txtCTeNumeroNF").val(),
        Tomador: $("#txtCTeTomador").val(),
        Remetente: $("#txtCTeRemetente").val(),
        Destinatario: $("#txtCTeDestinatario").val(),
        Pessoa: $("body").data("pessoa"),
    };

    executarRest("/Duplicatas/SelecionarTodosCTes?callback=?", dados, function (r) {
        if (r.Sucesso) {
            $("body").data("ctesSelecionadosConsultaAvancada", []);
            for (var i = 0; i < r.Objeto.length; i++) {
                if ($.inArray(r.Objeto[i].Codigo, codigosCtes) < 0)
                    RetornoConsultaCTeAvancado({ data: r.Objeto[i] });
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function FinalizarSelecaoCTes() {
    var ctes = $("body").data("ctes") || new Array();
    var codigosCtes = ctes.map(function (cte) { return cte.CodigoCte });
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") || new Array();

    ctesSelecionados.forEach(function (cte) {
        if ($.inArray(cte.Codigo, codigosCtes) >= 0) return;
        var _cte = {
            Codigo: --MenorIDCte,
            CodigoCte: cte.Codigo,
            Numero: cte.Numero,
            DescricaoCte: cte.Numero + " - " + cte.Serie,
            Modelo: cte.Modelo,
            TomadorCte: cte.Tomador,
            ValorCte: Globalize.parseFloat(cte.Valor),
            Excluir: false
        };
        ctes.push(_cte);
    });

    OrdenarCTes(ctes);
    $("body").data("ctes", ctes);

    RenderizarCtes();
    AtualizaValorDuplicata();
    $modalConsultaCTes.modal('hide');
}

function ConsultarCTes(callback) {
    var dados = {
        NumeroInicial: $("#txtCTeNumeroInicio").val(),
        NumeroFinal: $("#txtCTeNumeroFim").val(),
        DataEmissaoInicial: $("#txtCTeDataEmissaoInicio").val(),
        DataEmissaoFinal: $("#txtCTeDataEmissaoFim").val(),
        NumeroDocumento: $("#txtCTeNumeroNF").val(),
        Tomador: $("#txtCTeTomador").val(),
        Remetente: $("#txtCTeRemetente").val(),
        Destinatario: $("#txtCTeDestinatario").val(),
        Pessoa: $("body").data("pessoa"),
        inicioRegistros: 0
    };

    CriarGridView("/Duplicatas/ConsultarCtesSemDuplicata?callback=?", dados, "tbl_ctes_consulta_table", "tbl_ctes_consulta", "tbl_ctes_consulta_paginacao", [{ Descricao: "Selecionar", Evento: callback }], [0, 1, 2], null);
}

function RetornoConsultaCTeAvancado(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") || new Array();

    for (var i = 0; i < ctesSelecionados.length; i++) {
        if (ctesSelecionados[i].Codigo == cte.data.Codigo)
            return;
    }

    ctesSelecionados.push(cte.data);
    $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

    AdicionarTag(cte.data);
}

function AdicionarTag(cte) {
    var tag = $("<li></li>", { class: "tag-item tag-item-delete-experience", id: "cteSelecionado_" + cte.Codigo });
    var container = $("<span></span>", { class: "tag-container tag-container-delete-experience" });
    var descricao = $("<span></span>", { class: "tag-box tag-box-delete-experience" }).html("<b>" + cte.Numero + "</b>");

    var opcaoExcluir = $("<span></span>", { class: "tag-delete tag-box tag-box-delete-experience" }).html("&nbsp;");
    opcaoExcluir.click(function () { RemoverCTeSelecionado(cte) });

    container.append(descricao);
    container.append(opcaoExcluir);

    tag.append(container);

    $("#containerCTesSelecionados").append(tag);
}

function RemoverCTeSelecionado(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") || new Array();

    for (var i = 0; i < ctesSelecionados.length; i++) {
        if (ctesSelecionados[i].Codigo == cte.Codigo)
            ctesSelecionados.splice(i, 1);
    }

    $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

    $("#cteSelecionado_" + cte.Codigo).remove();
}

function OrdenarCTes(ctes) {
    ctes.sort(function (a, b) {
        return a.Numero < b.Numero ? 1 : a.Numero > b.Numero ? -1 : 0;
    });
}

function AbrirConsultaCTes() {
    $("#containerCTesSelecionados").html("");
    ConsultarCTes(RetornoConsultaCTeAvancado);

    $modalConsultaCTes.modal('show');
}

function RetornoConsultaCTe(cte) {
    if (cte.Codigo > 0) {
        var cte = {
            Codigo: Globalize.parseInt(cte.Codigo),
            DescricaoCte: cte.Numero + " - " + cte.Serie,
            TomadorCte: Globalize.format(cte.Tomador, "n2"),
            ValorCte: Globalize.parseFloat(cte.Valor),
        };
        AdicionarCte(cte);
    }
}

function LimparCamposCtes() {
    MenorIDCte = 0;
    $("body").data("cte", null);
    $("#txtCte").val("");
    $("#hddCodigoCte").val("0");
    $("#hddDescricaoCte").val("");
    $("#hddTomadorCte").val("");
    $("#hddValorCte").val("0");

    $("body").data("ctes", null);
}

function AdicionarCte(cte) {
    if (ValidarCamposCte()) {
        var ctes = $("body").data("ctes") || new Array();

        cte.Excluir = false;
        ctes.push(cte);

        OrdenarCTes(ctes);
        $("body").data("ctes", ctes);

        RenderizarCtes();
        AtualizaValorDuplicata();

        $("#selTipo").attr('disabled', true);
    }
}

function ValidarCamposCte() {
    var ctes = $("body").data("ctes") == null ? new Array() : $("body").data("ctes");
    var codigoCte = Globalize.parseFloat($("#hddCodigoCte").val());
    var valido = true;

    if (isNaN(codigoCte) || codigoCte <= 0) {
        CampoComErro("#txtCte");
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-mensagem-ctes");
        valido = false;
    } else {
        for (var i = 0; i < ctes.length; i++) {
            if (ctes[i].CodigoCte == codigoCte) {
                CampoComErro("#txtCte");
                ExibirMensagemAlerta("CTe já foi adicionado!", "Atenção!", "placeholder-mensagem-ctes");
                valido = false;
                break;
            } else {
                CampoSemErro("#txtCte");
            }
        }
    }

    if ($("#txtPessoa").val() == "") {
        valido = false;
        CampoComErro("#txtPessoa");
        ExibirMensagemAlerta("Necessário adicionar um Tomador!", "Atenção!", "placeholder-mensagem-ctes");
        $("#txtCte").val("");
        $("#hddCodigoCte").val("0");
        $("#hddDescricaoCte").val("");
        $("#hddTomadorCte").val("");
        $("#hddValorCte").val("0");
    }

    return valido;
}


function RenderizarCtes() {
    var ctes = $("body").data("ctes") == null ? new Array() : $("body").data("ctes");

    $("#tblCtes tbody").html("");

    for (var i = 0; i < ctes.length; i++) {
        if (!ctes[i].Excluir)
            $("#tblCtes tbody").append("<tr><td>" + ctes[i].DescricaoCte + "</td><td>" + ctes[i].Modelo + "</td><td>" + ctes[i].TomadorCte + "</td><td>" + Globalize.format(ctes[i].ValorCte, "n2") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='ExcluirCte(" + JSON.stringify(ctes[i]) + ")'>Excluir</button></td></tr>");
    }

    if ($("#tblCtes tbody").html() == "")
        $("#tblCtes tbody").html("<tr><td colspan='4'>Nenhum registro encontrado.</td></tr>");
}

function ExcluirCte(cte) {
    if (!VerificaParcelaPaga()) {
        var ctes = $("body").data("ctes") == null ? new Array() : $("body").data("ctes");

        for (var i = 0; i < ctes.length; i++) {
            if (ctes[i].Codigo == cte.Codigo) {
                if (cte.Codigo <= 0)
                    ctes.splice(i, 1);
                else
                    ctes[i].Excluir = true;
                break;
            }
        }

        OrdenarCTes(ctes);
        $("body").data("ctes", ctes);

        RenderizarCtes();
        AtualizaValorDuplicata();
    }
}

function AtualizaValorDuplicata() {
    var ctes = $("body").data("ctes") == null ? new Array() : $("body").data("ctes");
    var valorCtes = 0;

    for (var i = 0; i < ctes.length; i++) {
        if (!ctes[i].Excluir)
            valorCtes = valorCtes + ctes[i].ValorCte;
    }
    $("#txtValor").val(Globalize.format(valorCtes, "n2"));
}

function InicializarPlUploadCTe() {
    errosEnvioXMLCTe = "";
    ctesImportados = new Array();
    $("#divUploadArquivos").pluploadQueue({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        url: path + '/Duplicatas/ImportarXMLCTeDuplicata?callback=?',
        max_file_size: '500kb',
        unique_names: true,
        filters: [{ title: 'Arquivos XML', extensions: 'xml' }],
        silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
        flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
        init: {
            FileUploaded: function (up, file, info) {
                var retorno = JSON.parse(info.response.replace("?(", "").replace(");", ""));
                if (retorno.Sucesso) {
                    ctesImportados.push(retorno.Objeto);
                } else {
                    errosEnvioXMLCTe += retorno.Erro + "<br />";
                }
            },
            StateChanged: function (up) {
                if (up.state != plupload.STARTED) {
                    if (errosEnvioXMLCTe.trim() != "") {
                        jAlert("Ocorreram as seguintes falhas na importação dos arquivos: <br /><br />" + errosEnvioXMLCTe + "<br />", "Atenção");
                        AdicionarCTesImportados(ctesImportados);
                    } else {
                        AdicionarCTesImportados(ctesImportados);

                        jAlert("CT-es importados com sucesso!", "Sucesso", function () { $("#modalUploadArquivos").modal("hide"); });
                    }
                }
            }
        }
    });
}

function AbrirPlUpload() {
    $("#modalUploadArquivos").modal("show");
}

function AdicionarCTesImportados(ctesImportados) {
    var ctes = $("body").data("ctes") || new Array();
    var codigosCtes = ctes.map(function (cte) { return cte.CodigoCte });

    for (var i = 0; i < ctesImportados.length; i++) {
        if ($.inArray(ctesImportados[i].CodigoCte, codigosCtes) < 0)
            AdicionarCteImportado(ctesImportados[i]);
    }
}

function AdicionarCteImportado(cte) {
    var ctes = $("body").data("ctes") || new Array();

    cte.Excluir = false;
    ctes.push(cte);

    OrdenarCTes(ctes);
    $("body").data("ctes", ctes);

    RenderizarCtes();
    AtualizaValorDuplicata();

    $("#selTipo").attr('disabled', true);
}
