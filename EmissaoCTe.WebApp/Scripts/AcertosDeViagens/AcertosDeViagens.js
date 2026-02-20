var StateAnexos;
var IdAcertoViagem;
var UploadAnexos;

$(document).ready(function () {
    $("#txtDataLancamento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtDataLancamento").datepicker();
    $("#txtDataLancamento").mask("99/99/9999");

    $("#txtDataVcto").datepicker();
    $("#txtDataVcto").mask("99/99/9999");

    $("#txtAdiantamento").priceFormat({ prefix: '' });
    $("#txtComissao").priceFormat({ prefix: '' });
    $("#txtTotalReceita").priceFormat({ prefix: '' });
    $("#txtTotalDespesa").priceFormat({ prefix: '' });
    $("#txtVeiculo").mask("*******");
    $("#txtVeiculo").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoVeiculo").val("0");
                $("#hddDescricaoVeiculo").val("");
                $("#hddKmVeiculo").val("0");
                $("#txtVeiculo").data("TipoVeiculo", 0);
                CalculoPorTabelaDeFrete = false;
            } else {
                e.preventDefault();
            }
        }
    });
    $("#txtMotorista").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoMotorista").val("0");
                $("#hddDescricaoMotorista").val("");
            } else {
                e.preventDefault();
            }
        }
    });
    $("#btnSalvar").click(function () {
        SalvarAcertoViagem();
    });

    $("#btnVisualizar").click(function () {
        VisualizarAcertoViagem();
    });

    $("#btnCancelar").click(function () {
        LimparCamposAcertoViagem();
    });
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
    CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
    CarregarConsultaDeAcertosDeViagens("default-search", "default-search", "", EditarAcertoViagem, true, false);
    $('#tabDadosAcertoViagem a').click(function (e) {
        e.preventDefault()
        $(this).tab('show')
    });

    $("#selSituacao").change(function () {
        var valor = $(this).val();

        if (valor == "F") {
            AcertoFinalizado();
            $("#selSituacao").prop('disabled', false);
            $("#btnSalvar").show(true);
        }
        else
            AcertoAberto();
    });

    IdAcertoViagem = 0;
    $("#btnAnexar").addClass("disabled");

    $("#btnAnexar").click(function () {
        Anexar();
    });

    StateAnexos = new State({
        name: "anexos",
        id: "Codigo",
        render: RenderizarAnexos
    });

    CarregarAcerto();
});

function CarregarAcerto() {

    var codigoAcerto = GetUrlParam("x");

    LimparCamposAcertoViagem();

    if (codigoAcerto != null) {
        executarRest("/AcertoDeViagem/ObterDetalhes?callback=?", { CodigoX: codigoAcerto }, function (r) {
            if (r.Sucesso) {
                IdAcertoViagem = r.Objeto.Codigo;

                $("#hddCodigoAcertoViagem").val(r.Objeto.Codigo);
                $("#txtNumero").val(r.Objeto.Numero);
                $("#txtDataLancamento").val(r.Objeto.DataLancamento);
                $("#txtDataVcto").val(r.Objeto.DataVcto);                
                $("#txtVeiculo").val(r.Objeto.DescricaoVeiculo);
                $("#hddDescricaoVeiculo").val(r.Objeto.DescricaoVeiculo);
                $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                $("#txtVeiculo").data("TipoVeiculo", r.Objeto.TipoVeiculo);
                $("#txtMotorista").val(r.Objeto.DescricaoMotorista);
                $("#hddDescricaoMotorista").val(r.Objeto.DescricaoMotorista);
                $("#hddCodigoMotorista").val(r.Objeto.CodigoMotorista);
                $("#txtAdiantamento").val(Globalize.format(r.Objeto.Adiantamento, "n2"));
                $("#txtComissao").val(Globalize.format(r.Objeto.PercentualComissao, "n2"));
                $("#txtTotalReceita").val(Globalize.format(r.Objeto.TotalReceitas, "n2"));
                $("#txtTotalDespesa").val(Globalize.format(r.Objeto.TotalDespesas, "n2"));
                $("#selSituacao").val(r.Objeto.Situacao);
                $("#selStatus").val(r.Objeto.Status);
                $("#txtObservacao").val(r.Objeto.Observacao);
                $("#selTipoComissao").val(r.Objeto.TipoComissao);
                BuscarDestinos(r.Objeto);
                BuscarDespesas(r.Objeto);
                BuscarAbastecimentos(r.Objeto);
                BuscarVales(r.Objeto);
                BuscarKMVeiculo(r.Objeto.DescricaoVeiculo);
                $("#btnVisualizar").show();

                StateAnexos.set(r.Objeto.Anexos);
                $("#btnAnexar").removeClass("disabled");

                if (r.Objeto.Situacao == "F")
                    AcertoFinalizado();
                else
                    AcertoAberto();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
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

function RetornoConsultaVeiculos(veiculo) {
    $("#txtVeiculo").data("TipoVeiculo", veiculo.TipoVeiculo);
    $("#hddCodigoVeiculo").val(veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);
    $("#hddDescricaoVeiculo").val(veiculo.Placa);
    if ($("#hddKmVeiculo").val() == "0") {
        BuscarKMVeiculo(veiculo.Placa);
    }
    BuscaFreteVeiculo();
}
function RetornoConsultaMotorista(motorista) {
    $("#hddCodigoMotorista").val(motorista.Codigo);
    $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
    $("#hddDescricaoMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
    $("#txtComissao").val(Globalize.format(motorista.PercentualComissao, "n2"));
}
function LimparCamposAcertoViagem() {
    AcertoAberto();
    $("#hddCodigoAcertoViagem").val("0");
    $("#txtNumero").val("Automático");
    $("#txtDataLancamento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtDataVcto").val("");    
    $("#txtVeiculo").val("");
    $("#txtVeiculo").data("TipoVeiculo", 0);
    CalculoPorTabelaDeFrete = false;
    $("#hddCodigoVeiculo").val("0");
    $("#hddDescricaoVeiculo").val("");
    $("#hddKmVeiculo").val("0");
    $("#txtMotorista").val("");
    $("#hddCodigoMotorista").val("0");
    $("#hddDescricaoMotorista").val("");
    $("#txtAdiantamento").val("0,00");
    $("#txtComissao").val("0,00");
    $("#txtTotalReceita").val("0,00");
    $("#txtTotalDespesa").val("0,00");
    $("#selSituacao").val($("#selSituacao option:first").val());
    $("#selStatus").val($("#selStatus option:first").val());
    $("#txtObservacao").val("");
    $("#selTipoComissao").val($("#selTipoComissao option:first").val());

    LimparCamposDestino();
    LimparCamposDespesa();
    LimparCamposAbastecimento();
    LimparCamposVale();

    $("#hddDestinos").val("");
    RenderizarDestino();

    $("#hddDespesas").val("");
    RenderizarDespesa();

    $("#hddAbastecimentos").val("");
    RenderizarAbastecimento();

    $("body").data("vales", null);
    RenderizarVales();

    $("#btnAnexar").addClass("disabled");

    $("#btnVisualizar").hide();

    StateAnexos.clear();
    IdOcorrenciaCTe = 0;
}
function ValidarCamposAcertoViagem() {
    var veiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
    var descricaoVeiculo = $("#txtVeiculo").val().trim();
    var codigoMotorista = Globalize.parseInt($("#hddCodigoMotorista").val());
    var valido = true;
    if (veiculo == 0 && descricaoVeiculo == "") {
        CampoComErro("#txtVeiculo");
        valido = false;
    } else {
        CampoSemErro("#txtVeiculo");
    }
    if (isNaN(codigoMotorista) || codigoMotorista == 0) {
        CampoComErro("#txtMotorista");
        valido = false;
    } else {
        CampoSemErro("#txtMotorista");
    }
    return valido;
}
function SalvarAcertoViagem() {
    if (ValidarCamposAcertoViagem()) {
        var dados = {
            Codigo: $("#hddCodigoAcertoViagem").val(),
            DataLancamento: $("#txtDataLancamento").val(),
            DataVcto: $("#txtDataVcto").val(),            
            CodigoVeiculo: $("#hddCodigoVeiculo").val(),
            DescricaoVeiculo: $("#hddCodigoVeiculo").val() != "0" ? $("#hddDescricaoVeiculo").val() : $("#txtVeiculo").val(),
            CodigoMotorista: $("#hddCodigoMotorista").val(),
            Adiantamento: $("#txtAdiantamento").val(),
            Comissao: $("#txtComissao").val(),
            TotalReceitas: $("#txtTotalReceita").val(),
            TotalDespesas: $("#txtTotalDespesa").val(),
            Situacao: $("#selSituacao").val(),
            Status: $("#selStatus").val(),
            Observacao: $("#txtObservacao").val(),
            Destinos: $("#hddDestinos").val(),
            Despesas: $("#hddDespesas").val(),
            Abastecimentos: $("#hddAbastecimentos").val(),
            Vales: JSON.stringify($("body").data("vales")),
            TipoDespesa: $("#selTipoComissao").val()
        };

        var _confirmSave = function () {
            executarRest("/AcertoDeViagem/Salvar?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                    LimparCamposAcertoViagem();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        if (dados.Situacao == "F")
            jConfirm("Tem certeza que deseja finalizar o Acerto?<br>O Acerto finalizado não poderá ser modificado depois de salvo.", "Acerto Finalizado", function (conf) {
                if (conf)
                    _confirmSave();
            });
        else
            _confirmSave();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
    }
}

function VisualizarAcertoViagem() {
    executarDownload("/AcertoDeViagem/VisualizarAcerto?callback=?", { Codigo: $("#hddCodigoAcertoViagem").val() });
}

function EditarAcertoViagem(acerto) {
    executarRest("/AcertoDeViagem/ObterDetalhes?callback=?", { Codigo: acerto.Codigo }, function (r) {
        if (r.Sucesso) {
            IdAcertoViagem = r.Objeto.Codigo;

            $("#hddCodigoAcertoViagem").val(r.Objeto.Codigo);
            $("#txtNumero").val(r.Objeto.Numero);
            $("#txtDataLancamento").val(r.Objeto.DataLancamento);
            $("#txtDataVcto").val(r.Objeto.DataVcto);            
            $("#txtVeiculo").val(r.Objeto.DescricaoVeiculo);
            $("#hddDescricaoVeiculo").val(r.Objeto.DescricaoVeiculo);
            $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
            $("#txtVeiculo").data("TipoVeiculo", r.Objeto.TipoVeiculo);
            $("#txtMotorista").val(r.Objeto.DescricaoMotorista);
            $("#hddDescricaoMotorista").val(r.Objeto.DescricaoMotorista);
            $("#hddCodigoMotorista").val(r.Objeto.CodigoMotorista);
            $("#txtAdiantamento").val(Globalize.format(r.Objeto.Adiantamento, "n2"));
            $("#txtComissao").val(Globalize.format(r.Objeto.PercentualComissao, "n2"));
            $("#txtTotalReceita").val(Globalize.format(r.Objeto.TotalReceitas, "n2"));
            $("#txtTotalDespesa").val(Globalize.format(r.Objeto.TotalDespesas, "n2"));
            $("#selSituacao").val(r.Objeto.Situacao);
            $("#selStatus").val(r.Objeto.Status);
            $("#txtObservacao").val(r.Objeto.Observacao);
            $("#selTipoComissao").val(r.Objeto.TipoComissao);
            BuscarDestinos(r.Objeto);
            BuscarDespesas(r.Objeto);
            BuscarAbastecimentos(r.Objeto);
            BuscarVales(r.Objeto);
            BuscarKMVeiculo(r.Objeto.DescricaoVeiculo);
            $("#btnVisualizar").show();

            StateAnexos.set(r.Objeto.Anexos);
            $("#btnAnexar").removeClass("disabled");

            if (r.Objeto.Situacao == "F")
                AcertoFinalizado();
            else
                AcertoAberto();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}
function BuscarKMVeiculo(placa) {
    if ($("#hddKmVeiculo").val() == "0") {
        executarRest("/AcertoDeViagem/BuscarKilometragem?callback=?", { Placa: placa }, function (r) {
            if (r.Sucesso) {
                $("#hddKmVeiculo").val(Globalize.format(r.Objeto.kmMaxima, "n0"));
                $("#txtKMInicialAbastecimento").val(Globalize.format(r.Objeto.kmMaxima, "n0"));
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
}

function AtualizarValores() {
    var debitos = 0;
    var creditos = 0;
    var descontos = 0;
    var adiantamento = 0;
    var destinos = $("#hddDestinos").val() == "" ? new Array() : JSON.parse($("#hddDestinos").val());
    for (var i = 0; i < destinos.length; i++) {
        if (!destinos[i].Excluir) {
            creditos += Globalize.parseFloat(destinos[i].ValorFrete);
            descontos += Globalize.parseFloat(destinos[i].OutrosDescontos);
        }
    }
    var despesas = $("#hddDespesas").val() == "" ? new Array() : JSON.parse($("#hddDespesas").val());
    for (var i = 0; i < despesas.length; i++) {
        if (!despesas[i].Excluir) {
            var total = (Globalize.parseFloat(despesas[i].Quantidade) * Globalize.parseFloat(despesas[i].ValorUnitario));

            if (!despesas[i].Paga) //Soma no total dos debitos quando despesa não é paga pelo motorista
                debitos += total;
            //else
            //debitos -= total;
        }
    }
    var abastecimentos = $("#hddAbastecimentos").val() == "" ? new Array() : JSON.parse($("#hddAbastecimentos").val());
    for (var i = 0; i < abastecimentos.length; i++) {
        if (!abastecimentos[i].Excluir)
            debitos += (Globalize.parseFloat(abastecimentos[i].Litros) * Globalize.parseFloat(abastecimentos[i].ValorUnitario));
    }
    var vales = $("body").data("vales") == null ? new Array() : $("body").data("vales");
    for (var i = 0; i < vales.length; i++) {
        if (!vales[i].Excluir) {
            var valor = Globalize.parseFloat(vales[i].Valor);
            if (vales[i].Tipo == 1) // 1 = Vale 2 = Devolucao
                adiantamento += valor;
            else
                adiantamento -= valor;
        }
    }
    $("#txtTotalReceita").val(Globalize.format(creditos - descontos, "n2"));
    $("#txtTotalDespesa").val(Globalize.format(debitos, "n2"));
    $("#txtAdiantamento").val(Globalize.format(adiantamento, "n2"));
}

function AcertoFinalizado() {
    var match_filter = "input, select, textarea, .btn";

    // Campos do Acerto
    $(".acerto-viagem-container").find(match_filter).prop('disabled', true);
    $("#btnSalvar").hide();

    // Destinos
    $("#divDestinos").find(match_filter).prop('disabled', true);
    $("#btnSalvarDestino").hide();

    // Despesas
    $("#divDespesas").find(match_filter).prop('disabled', true);
    $("#btnSalvarDespesa").hide();

    // Abastecimentos
    $("#divAbastecimentos").find(match_filter).prop('disabled', true);
    $("#btnSalvarAbastecimento").hide();

    // Abastecimentos
    $("#divVales").find(match_filter).prop('disabled', true);
    $("#btnSalvarVale, #btnCancelarVale").hide();
}
function AcertoAberto() {
    var match_filter = "input, select, textarea, .btn";

    // Campos do Acerto
    $(".acerto-viagem-container").find(match_filter).prop('disabled', false);
    $(".acerto-viagem-container").find("#txtNumero, #txtTotalReceita, #txtTotalDespesa").prop('disabled', true);
    $("#btnSalvar").show();

    // Destinos
    $("#divDestinos").find(match_filter).prop('disabled', false);
    $("#btnSalvarDestino").show();

    // Despesas
    $("#divDespesas").find(match_filter).prop('disabled', false);
    $("#btnSalvarDespesa").show();

    // Abastecimentos
    $("#divAbastecimentos").find(match_filter).prop('disabled', false);
    $("#divAbastecimentos").find("#txtValorTotalAbastecimento, #txtMediaAbastecimento").prop('disabled', true);
    $("#btnSalvarAbastecimento").show();

    // Abastecimentos
    $("#divVales").find(match_filter).prop('disabled', false);
    $("#divVales").find("#txtNumeroVale").prop('disabled', true);
    $("#btnSalvarVale, #btnCancelarVale").show();
}
function Anexar() {
    if (IdAcertoViagem == 0)
        return jAlert("É preciso salvar o acerto antes de enviar anexos.", "Anexos indisponíveis")
    UploadAnexos = AbrirUploadPadrao({
        title: "Anexar arquivos",
        url: "/AcertoDeViagem/Anexar?callback=?&Codigo=" + IdAcertoViagem,
        onFinish: function (arquivos, erros) {
            if (arquivos.length > 0) {
                for (var i in arquivos)
                    StateAnexos.insert(arquivos[i]);

            }

            if (erros.length > 0) {
                var uuErros = [];
                for (var i in erros)
                    if ($.inArray(erros[i], uuErros) < 0)
                        uuErros.push(erros[i]);
                ExibirMensagemErro(uuErros.join("<br>"), "Erro no envio de anexo:<br>");
            }
        }
    });
}

function ExcluirAnexo(anexo) {
    jConfirm("Tem certeza que deseja excluir esse anexo?", "Excluir Anexo", function (res) {
        if (res) {
            executarRest("/AcertoDeViagem/ExcluirAnexo?callback=?", anexo, function (r) {
                if (r.Sucesso) {
                    StateAnexos.remove({ Codigo: anexo.Codigo });
                    ExibirMensagemSucesso("Excluído com sucesso.", "Sucesso!");
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
    });
}

function DownloadAnexo(anexo) {
    executarDownload("/AcertoDeViagem/DownloadAnexo", anexo);
}

function RenderizarAnexos() {
    var itens = StateAnexos.get();
    var $tabela = $("#tblAnexos");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td><a href='#' class='download'>" + info.Nome + "</button></td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Excluir</button></td>" +
                "</tr>");

            $row.on("click", "button", function () {
                ExcluirAnexo(info);
            });

            $row.on("click", ".download", function (e) {
                if (e && e.preventDefault) e.preventDefault();
                DownloadAnexo(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td class='text-center' colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}