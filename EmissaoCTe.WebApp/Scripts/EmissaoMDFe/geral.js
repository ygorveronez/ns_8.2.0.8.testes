$(document).ready(function () {
    ObterSeries();
    ObterModalidadesDeTransporte();
    ObterEstados();
    ObterDadosEmpresa();

    FormatarCampoDate("txtDataEmissao");

    $("#txtHoraEmissao").mask("99:99");
    $("#txtPesoBruto").priceFormat({ limit: 15, centsLimit: 4 });
    $("#txtValorTotal").priceFormat({ limit: 15, centsLimit: 2 });
    $("#txtRNTRC").mask("99999999");
    $("#txtCIOT").mask("999999999999");

    $("#txtValorDoPagamento").priceFormat({ limit: 15, centsLimit: 2 });
    $("#txtValorDoAdiantamento").priceFormat({ limit: 15, centsLimit: 2 });
    $("#txtValorDaParcela").priceFormat({ limit: 15, centsLimit: 2 });

    $("body").data("UFCarregamento", $("#selUFCarregamento").val());
    $("body").data("UFDescarregamento", $("#selUFDescarregamento").val());

    $("#selUFCarregamento").change(function () {
        AlterarUFCarregamento();
    });

    $("#selUFDescarregamento").change(function () {
        AlterarUFDescarregamento();
    });

    $("body").data("TipoEmitente", $("#selTipoEmitente").val());

    $("#selTipoEmitente").change(function () {
        AlterarTipoEmitente();
    });
});

function AlterarUFCarregamento() {
    var municipiosCarregamento = $("body").data("municipiosCarregamento") != null ? $("body").data("municipiosCarregamento") : new Array();
    var valido = true;

    for (var i = 0; i < municipiosCarregamento.length; i++) {
        if (!municipiosCarregamento[i].Excluir) {
            valido = false;
            break;
        }
    }

    if (!valido) {
        ExibirMensagemAlerta("Não é possível alterar o estado de carregamento pois já existem municípios de carregamento registrados. Exclua os municípios de carregamento para realizar esta alteração.", "Atenção!", "placeholder-msgEmissaoMDFe");
        $("#selUFCarregamento").val($("body").data("UFCarregamento"));
    } else {
        ObterMunicipios($("#selUFCarregamento").val(), "selMunicipioCarregamento");
        $("body").data("UFCarregamento", $("#selUFCarregamento").val());
        ObterPercursos();
    }
}

function AlterarUFDescarregamento() {
    var municipiosDescarregamento = $("body").data("municipiosDescarregamento") != null ? $("body").data("municipiosDescarregamento") : new Array();
    var valido = true;

    for (var i = 0; i < municipiosDescarregamento.length; i++) {
        if (!municipiosDescarregamento[i].Excluir) {
            valido = false;
            break;
        }
    }

    if (!valido) {
        ExibirMensagemAlerta("Não é possível alterar o estado de descarregamento pois já existem municípios de descarregamento registrados. Exclua os municípios de descarregamento para realizar esta alteração.", "Atenção!", "placeholder-msgEmissaoMDFe");
        $("#selUFDescarregamento").val($("body").data("UFDescarregamento"));
    } else {
        ObterMunicipios($("#selUFDescarregamento").val(), "selMunicipioDescarregamento");
        $("body").data("UFDescarregamento", $("#selUFDescarregamento").val());
        ObterPercursos();
    }
}

function AlterarTipoEmitente() {
    var valido = true;

    if ($("body").data("municipiosDescarregamento") != null && $("body").data("municipiosDescarregamento").length > 0) {

        var municipiosDescarregamento = $("body").data("municipiosDescarregamento");

        for (var i = 0; i < municipiosDescarregamento.length; i++) {
            if (municipiosDescarregamento[i].Documentos != null && municipiosDescarregamento[i].Documentos.length > 0) {
                valido = false;
                break;
            }
        }
    } else {
        if ($("#selTipoEmitente").val() == "2" || $("#selTipoEmitente").val() == "3")
            jConfirm("Deseja gerar os dados do MDF-e à partir de XML de Notas Fiscais Eletrônicas?", "Atenção!", function (r) {
                if (r) {
                    AbrirUploadXMLNFe();
                }
            });
    }

    if (!valido) {
        ExibirMensagemAlerta("Remova os CT-es ou os Municípios de Descarregamento para alterar o tipo de emitente.", "Atenção!", "placeholder-msgEmissaoMDFe");
        $("#selTipoEmitente").val($("body").data("TipoEmitente"));
        return;
    } else {
        RenderizarMunicipiosDescarregamento();
    }
}

function ObterDadosEmpresa() {
    executarRest("/Empresa/ObterDetalhesDaEmpresaDoUsuario?callback=?", {}, function (r) {
        if (r.Sucesso) {
            $("body").data("empresa", r.Objeto);

            if (r.Objeto != null && r.Objeto.ExibirCobrancaCancelamento != null && r.Objeto.ExibirCobrancaCancelamento == true)
                $("#divOpcoesCobrancaCancelamento").show();
            else $("#divOpcoesCobrancaCancelamento").hide();

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ObterSeries() {
    executarRest("/Usuario/ObterSeriesDoUsuario?callback=?", { Tipo: 1 }, function (r) {
        if (r.Sucesso) {

            var selSerie = document.getElementById("selSerie");
            var selSerieMDFeFiltro = document.getElementById("selSerieMDFeFiltro");

            selSerie.options.length = 0;
            selSerieMDFeFiltro.options.length = 0;

            var optnTodos = document.createElement("option");
            optnTodos.text = "Todas";
            optnTodos.value = "";

            selSerieMDFeFiltro.options.add(optnTodos);

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Numero;
                optn.value = r.Objeto[i].Codigo;

                selSerie.options.add(optn);
                selSerieMDFeFiltro.add(optn.cloneNode(true));
            }

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ObterModalidadesDeTransporte() {
    executarRest("/ModalTransporte/BuscarTodas?callback=?", {}, function (r) {
        if (r.Sucesso) {
            var selModal = document.getElementById("selModal");
            selModal.options.length = 0;
            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Codigo;
                selModal.options.add(optn);
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ObterEstados() {
    executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
        if (r.Sucesso) {

            var selUFCarregamento = document.getElementById("selUFCarregamento");
            var selUFDescarregamento = document.getElementById("selUFDescarregamento");
            var selUFPercurso = document.getElementById("selUFPercurso");
            var selUFCargaFiltro = document.getElementById("selUFCargaFiltro");
            var selUFDescargaFiltro = document.getElementById("selUFDescargaFiltro");
            var selUFVeiculo = document.getElementById("selUFVeiculo");
            var selUFProprietarioVeiculo = document.getElementById("selUFProprietarioVeiculo");
            var selUFReboque = document.getElementById("selUFReboque");
            var selUFProprietarioReboque = document.getElementById("selUFProprietarioReboque");

            selUFCarregamento.options.length = 0;
            selUFDescarregamento.options.length = 0;
            selUFPercurso.options.length = 0;
            selUFCargaFiltro.options.length = 0;
            selUFDescargaFiltro.options.length = 0;
            selUFVeiculo.options.length = 0;
            selUFProprietarioVeiculo.options.length = 0;
            selUFReboque.options.length = 0;
            selUFProprietarioReboque.options.length = 0;

            var optnTodos = document.createElement("option");
            optnTodos.text = "Todos";
            optnTodos.value = "";

            selUFCargaFiltro.options.add(optnTodos);
            selUFDescargaFiltro.options.add(optnTodos.cloneNode(true));

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Nome;
                optn.value = r.Objeto[i].Sigla;

                selUFCarregamento.options.add(optn);
                selUFDescarregamento.options.add(optn.cloneNode(true));
                selUFPercurso.options.add(optn.cloneNode(true));
                selUFCargaFiltro.options.add(optn.cloneNode(true));
                selUFDescargaFiltro.options.add(optn.cloneNode(true));
                selUFVeiculo.options.add(optn.cloneNode(true));
                selUFProprietarioVeiculo.options.add(optn.cloneNode(true));
                selUFReboque.options.add(optn.cloneNode(true));
                selUFProprietarioReboque.options.add(optn.cloneNode(true));
            }

            $("#selUFCarregamento").val("");
            $("#selUFDescarregamento").val("");
            $("#selUFVeiculo").val("");
            $("#selUFProprietarioVeiculo").val("");
            $("#selUFCargaFiltro").val("");
            $("#selUFDescargaFiltro").val("");
            $("#selUFReboque").val("");
            $("#selUFProprietarioReboque").val("");

        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function ObterMunicipios(estado, select, codigo) {
    executarRest("/Localidade/BuscarPorUF?callback=?", { UF: estado }, function (r) {
        if (r.Sucesso) {
            var selMunicipio = document.getElementById(select);

            selMunicipio.options.length = 0;

            for (var i = 0; i < r.Objeto.length; i++) {
                var optn = document.createElement("option");
                optn.text = r.Objeto[i].Descricao;
                optn.value = r.Objeto[i].Codigo;

                if (codigo != null && codigo == r.Objeto[i].Codigo)
                    optn.selected = "selected";

                selMunicipio.options.add(optn);
            }
        } else {
            ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoMDFe");
        }
    });
}

function SetarDataEmissao() {
    $("#txtDataEmissao").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtHoraEmissao").val(Globalize.format(new Date(), "HH:mm"));
}

function SetarRNTRC() {
    $("#txtRNTRC").val(($("body").data("empresa") != null ? $("body").data("empresa").RNTRC : ""));
}