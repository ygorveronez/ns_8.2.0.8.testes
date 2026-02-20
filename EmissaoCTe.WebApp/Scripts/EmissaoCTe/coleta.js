$(function () {
    $("#btnConsultarColeta").click(function () {
        ConsultarColetas();
    });

    $("#btnFecharColeta").click(function () {
        FecharTelaColetas();
    });
});

function FecharTelaColetas() {
    $("#divColetaCTe").modal("hide");
}

function AbrirTelaColetas() {
    $("#divColetaCTe").modal({ keyboard: false });
}

function ConsultarColetas() {
    $("#spnOrigemColeta").text("");
    $("#spnDestinoColeta").text("");
    $("#spnVeiculosColeta").text("");
    $("#divInfoVeiculosColeta").addClass("hidden");

    var dados = {
        CPFCNPJRemetente: $("#hddRemetente").val(),
        CPFCNPJDestinatario: $("#hddDestinatario").val(),
        CodigoOrigem: $("#ddlMunicipioInicioPrestacao").val(),
        CodigoDestino: $("#ddlMunicipioTerminoPrestacao").val()
    };

    var codigos = new Array();
    var placas = new Array();
    var veiculos = $("#hddVeiculos").val() == "" ? new Array() : JSON.parse($("#hddVeiculos").val());

    for (var i = 0; i < veiculos.length; i++) {
        codigos.push(veiculos[i].Codigo);
        placas.push(veiculos[i].Placa);
    }

    dados.Veiculos = JSON.stringify(codigos);

    if (dados.CPFCNPJRemetente != "" && dados.CPFCNPJRemetente != null && dados.CPFCNPJDestinatario != "" && dados.CPFCNPJDestinatario != null) {
        $("#spnOrigemColeta").html(dados.CPFCNPJRemetente + " - " + $("#txtRazaoSocialRemetente").val() + "&nbsp;&nbsp;<span class='glyphicon glyphicon-arrow-right'></span>&nbsp;&nbsp;");
        $("#spnDestinoColeta").html(dados.CPFCNPJDestinatario + " - " + $("#txtRazaoSocialDestinatario").val() + "&nbsp;&nbsp;<span class='glyphicon glyphicon-arrow-right'></span>&nbsp;&nbsp;");
    }

    $("#spnOrigemColeta").append($("#ddlMunicipioInicioPrestacao option:selected").text() + " / " + $("#ddlUFInicioPrestacao").val());
    $("#spnDestinoColeta").append($("#ddlMunicipioTerminoPrestacao option:selected").text() + " / " + $("#ddlUFTerminoPrestacao").val());

    if (veiculos.length > 0) {
        $("#spnVeiculosColeta").text(placas.join(", "));
        $("#divInfoVeiculosColeta").removeClass("hidden");
    } 

    executarRest("/Coleta/ObterColetasParaCTe?callback=?", dados, function (r) {
        if (r.Sucesso) {

            RenderizarColetas(r.Objeto);

            AbrirTelaColetas();

        } else {
            jAlert(r.Erro, "Erro!");
        }
    });
}

function RenderizarColetas(coletas) {
    var listaInfoQuantidade = $("#hddInformacoesQuantidadeCarga").val() == "" ? new Array() : JSON.parse($("#hddInformacoesQuantidadeCarga").val());

    var peso = 0;
    var valorCarga = Globalize.parseFloat($("#txtValorTotalCarga").val());

    for (var i = 0; i < listaInfoQuantidade.length; i++)
        if (listaInfoQuantidade[i].UnidadeMedida == "1")
            peso += listaInfoQuantidade[i].Quantidade;

    $("#tbl_coletas tbody").html("");

    if (coletas != null && coletas.length > 0) {
        for (var i = 0; i < coletas.length; i++) {
            var pesoColeta = Globalize.parseFloat(coletas[i].Peso);
            var valorCargaColeta = Globalize.parseFloat(coletas[i].ValorNFs);

            $("#tbl_coletas tbody").append("<tr class='" + ((pesoColeta == peso) && (valorCarga == valorCargaColeta) ? "success" : "danger") + "'><td>" + coletas[i].Numero +
                                           "</td><td>" + (coletas[i].Remetente != null ? coletas[i].Remetente : coletas[i].Origem) +
                                           "</td><td>" + (coletas[i].Destinatario != null ? coletas[i].Destinatario : coletas[i].Destino) +
                                           "</td><td>" + (valorCarga != valorCargaColeta ? "<span class='glyphicon glyphicon-exclamation-sign'></span>&nbsp;" : "") + coletas[i].ValorNFs +
                                           "</td><td>" + (peso != pesoColeta ? "<span class='glyphicon glyphicon-exclamation-sign'></span>&nbsp;" : "") + coletas[i].Peso + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='SelecionarColeta(" + coletas[i].Codigo + ")'>Selecionar</button></td></tr>");
        }
    }

    if ($("#tbl_coletas tbody").html() == "")
        $("#tbl_coletas tbody").html('<tr><td colspan="6">Nenhum registro encontrado!</td></tr>');
}

function SelecionarColeta(codigo) {
    executarRest("/Coleta/ObterDetalhes?callback=?", { CodigoColeta: codigo }, function (r) {
        if (r.Sucesso) {
            $("body").data("codigoColeta", r.Objeto.Codigo);

            FecharTelaColetas();

            $("#selPago_APagar").val(r.Objeto.TipoPagamento);

            if (r.Objeto.CPFCNPJRemetente != null && r.Objeto.CPFCNPJRemetente.length > 0 && r.Objeto.CPFCNPJRemetente != $("#hddRemetente").val()) {
                $("#txtCPFCNPJRemetente").val(r.Objeto.CPFCNPJRemetente);
                BuscarRemetente(true);
            }

            if (r.Objeto.CPFCNPJDestinatario != null && r.Objeto.CPFCNPJDestinatario.length > 0 && r.Objeto.CPFCNPJDestinatario != $("#hddDestinatario").val()) {
                $("#txtCPFCNPJDestinatario").val(r.Objeto.CPFCNPJDestinatario);
                BuscarDestinatario(true);
            }

            if (r.Objeto.TipoPagamento == 2 && r.Objeto.CPFCNPJTomador != null && r.Objeto.CPFCNPJTomador.length > 0 && r.Objeto.CPFCNPJTomador != $("#hddTomador").val()) {
                $("#txtCPFCNPJTomador").val(r.Objeto.CPFCNPJTomador);
                BuscarTomador(true);
            }

            if (r.Objeto.ObservacaoCTe != null && r.Objeto.ObservacaoCTe.length > 0 && $("#txtObservacaoGeral").val().indexOf(r.Objeto.ObservacaoCTe) == -1) {
                if ($("#txtObservacaoGeral").val().length <= 0)
                    $("#txtObservacaoGeral").val(r.Objeto.ObservacaoCTe);
                else
                    $("#txtObservacaoGeral").append(" - " + r.Objeto.ObservacaoCTe);
            }

            if (r.Objeto.DescricaoTipoCarga != null && r.Objeto.DescricaoTipoCarga.length > 0)
                $("#txtProdutoPredominante").val(r.Objeto.DescricaoTipoCarga);

            if (r.Objeto.DataEntrega != null && r.Objeto.DataEntrega.length > 0)
                $("#txtDataPrevistaEntregaCargaRecebedor").val(r.Objeto.DataEntrega);

            CadastrarVeiculosColeta(r.Objeto.Veiculos);
            CadastrarMotoristasColeta(r.Objeto.Motoristas);

        } else {
            jAlert(r.Erro, "Erro!");
        }
    });
}

function CadastrarVeiculosColeta(veiculosCadastrar) {
    if (veiculosCadastrar != null && veiculosCadastrar.length > 0) {

        var veiculos = $("#hddVeiculos").val() == "" ? new Array() : JSON.parse($("#hddVeiculos").val());

        for (var i = 0; i < veiculos.length; i++) {
            if (veiculos[i].Id <= 0)
                veiculos.splice(i, 1);
            else
                veiculos[i].Excluir = true;
        }

        for (var i = 0; i < veiculosCadastrar.length; i++) {
            veiculosCadastrar[i].Id = -(i + 1);
            veiculosCadastrar[i].Excluir = false;

            veiculos.push(veiculosCadastrar[i]);
        }

        $("#hddVeiculos").val(JSON.stringify(veiculos));
        RenderizarVeiculos();
        LimparCamposVeiculo();
    }
}

function CadastrarMotoristasColeta(motoristasCadastrar) {
    if (motoristasCadastrar != null && motoristasCadastrar.length > 0) {

        var motoristas = $("#hddMotoristas").val() == "" ? new Array() : JSON.parse($("#hddMotoristas").val());

        for (var i = 0; i < motoristas.length; i++) {
            if (motoristas[i].Codigo > 0)
                motoristas[i].Excluir = true;
            else
                motoristas.splice(i, 1);
        }

        for (var i = 0; i < motoristasCadastrar.length; i++) {
            motoristasCadastrar[i].Codigo = -(i + 1);
            motoristasCadastrar[i].Excluir = false;

            motoristas.push(motoristasCadastrar[i]);
        }

        $("#hddMotoristas").val(JSON.stringify(motoristas));
        RenderizarMotoristas();
        LimparCamposMotorista();
    }
}

