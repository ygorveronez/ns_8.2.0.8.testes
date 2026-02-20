var TabelaDeFreteEmpresa = null;
var CalculoPorTabelaDeFrete = false;

$(document).ready(function () {
    $("#txtKMInicial, #txtKMFinal").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
    $("#txtValorUnitario, #txtValorFrete, #txtOutrosDescontos").priceFormat({ prefix: '' });
    $("#txtPeso").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtDataInicial, #txtDataFinal").mask("99/99/9999");
    $("#txtDataInicial, #txtDataFinal").datepicker();

    CarregarConsultaDeTiposDeCargas("btnBuscarTipoDaCarga", "btnBuscarTipoDaCarga", "A", RetornoConsultaTipoCarga, true, false);
    CarregarConsultaDeCTes("btnBuscarCTe", "btnBuscarCTe", "", RetornoConsultaCTe, true, false);
    CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);

    RemoveConsulta("#txtCliente, #txtCTe, #txtTipoDaCarga", function ($this) {
        $this.val('');
        $this.data('Codigo', 0);
        $this.data('Acerto', 0);
    });

    $("#txtPeso, #txtValorUnitario").change(function () {
        CalcularValorPorPeso();
    });

    $("#txtKMInicial, #txtKMFinal").change(function () {
        CalcularValorPorTabelaDeFrete();
    });

    $("#btnSalvarDestino").click(function () {
        SalvarDestinos();
    });

    $("#btnExcluirDestino").click(function () {
        ExcluirDestino();
    });

    $("#btnCancelarDestino").click(function () {
        LimparCamposDestino();
    });

    SetarDadosPadraoDestino();
    CarregarTabelasDeFrete();
    LimparCamposDestino();
});

function SetarDadosPadraoDestino() {
    $("#txtDataInicial").val(Globalize.format(new Date(), "dd/MM/yyyy"));
    $("#txtDataFinal").val(Globalize.format(new Date(), "dd/MM/yyyy"));
}

function RetornoConsultaTipoCarga(tipoCarga) {
    $("#txtTipoDaCarga").val(tipoCarga.Descricao);
    $("#txtTipoDaCarga").data('Codigo', tipoCarga.Codigo);
}

function RetornoConsultaCliente(cliente) {
    $("#txtCliente").val(cliente.Nome);
    $("#txtCliente").data('Codigo', cliente.CPFCNPJ.replace(/[^0-9]/g, ''));
}

function RetornoConsultaCTe(cte) {
    $("#txtCTe").val(cte.Numero + " - " + cte.Serie);
    $("#txtCTe").data('Codigo', cte.Codigo);

    if (!CalculoPorTabelaDeFrete)
        $("#txtValorFrete").val(Globalize.format(cte.ValorFrete, "n2"));

    executarRest("/AcertoDeViagem/ObterDetalhesDestinoCTe?callback=?", { CodigoCTe: cte.Codigo }, function (r) {
        if (r.Sucesso) {
            $("#txtCTe").data('Acerto', r.Objeto.AcertoVinculado);
            $("#txtPeso").val(Globalize.format(r.Objeto.PesoTotal, "n4"));

            $("#selUFOrigem").val(r.Objeto.UFInicio);
            BuscarLocalidades(r.Objeto.UFInicio, "selMunicipioOrigem", r.Objeto.CodigoLocalidadeInicio);

            $("#selUFDestino").val(r.Objeto.UFFim);
            BuscarLocalidades(r.Objeto.UFFim, "selMunicipioDestino", r.Objeto.CodigoLocalidadeFim);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "mensagensDestinos-placeholder");
        }
    });
}

function LimparCamposDestino() {
    $("#hddCodigoDestino").val(0);
    $("#txtTipoDaCarga").val("").data('Codigo', 0);
    $("#txtCTe").val("").data('Codigo', 0);
    $("#txtCliente").val("").data('Codigo', 0);
    $("#txtKMInicial").val("0");
    $("#txtKMFinal").val("0");
    $("#txtDataInicial").val("");
    $("#txtDataFinal").val("");
    $("#selUFOrigem").val($("#selUFOrigem option:first").val());
    $("#selMunicipioOrigem").html("");
    $("#selUFDestino").val($("#selUFDestino option:first").val());
    $("#selMunicipioDestino").html("");
    $("#txtPeso").val("0,0000");
    $("#txtValorUnitario").val("0,00"); 
    $("#txtValorFrete").val("0,00");
    $("#txtOutrosDescontos").val("0,00");
    $("#txtObservacaoDestino").val("");

    $("#btnExcluirDestino").hide();
    $("#btnCancelarDestino").hide();

    SetarDadosPadraoDestino();
}

function ValidarCamposDestino() {
    var municipioOrigem = $("#selMunicipioOrigem").val() == null ? 0 : Globalize.parseInt($("#selMunicipioOrigem").val());
    var municipioDestino = $("#selMunicipioDestino").val() == null ? 0 : Globalize.parseInt($("#selMunicipioDestino").val());
    var valido = true;
    if ($("#txtTipoDaCarga").data('Codigo') == 0) {
        CampoComErro("#txtTipoDaCarga");
        valido = false;
    } else {
        CampoSemErro("#txtTipoDaCarga");
    }
    if (isNaN(municipioDestino) || municipioDestino == 0) {
        CampoComErro("#selMunicipioDestino");
        valido = false;
    } else {
        CampoSemErro("#selMunicipioDestino");
    }
    if (isNaN(municipioOrigem) || municipioOrigem == 0) {
        CampoComErro("#selMunicipioOrigem");
        valido = false;
    } else {
        CampoSemErro("#selMunicipioOrigem");
    }
    return valido;
}

function SalvarDestinos() {
    if ($("#txtCTe").data('Acerto') > 0 && $("#txtCTe").data('Acerto') != $("#txtNumero").val() )
        return ExibirMensagemAlerta("O CT-e " + $("#txtCTe").val() + " já foi utilizado no Acerto Nº " + $("#txtCTe").data('Acerto'), "CT-e já utilizado", "mensagensDestinos-placeholder");

    if (ValidarCamposDestino()) {
        var destino = {
            Codigo: Globalize.parseInt($("#hddCodigoDestino").val()),
            CodigoTipoCarga: $("#txtTipoDaCarga").data('Codigo'),
            DescricaoTipoCarga: $("#txtTipoDaCarga").val(),
            CodigoCTe: $("#txtCTe").data('Codigo'),
            DescricaoCTe: $("#txtCTe").val(),
            CodigoCliente: $("#txtCliente").data('Codigo'),
            DescricaoCliente: $("#txtCliente").val(),
            KMInicial: Globalize.parseInt($("#txtKMInicial").val()),
            KMFinal: Globalize.parseInt($("#txtKMFinal").val()),
            DataInicial: $("#txtDataInicial").val(),
            DataFinal: $("#txtDataFinal").val(),
            UFOrigem: $("#selUFOrigem").val(),
            MunicipioOrigem: $("#selMunicipioOrigem").val(),
            DescricaoOrigem: $("#selMunicipioOrigem :selected").text() + " / " + $("#selUFOrigem").val(),
            UFDestino: $("#selUFDestino").val(),
            MunicipioDestino: $("#selMunicipioDestino").val(),
            DescricaoDestino: $("#selMunicipioDestino :selected").text() + " / " + $("#selUFDestino").val(),
            ValorFrete: $("#txtValorFrete").val(),
            ValorUnitario: $("#txtValorUnitario").val(),
            OutrosDescontos: $("#txtOutrosDescontos").val(),
            Observacao: $("#txtObservacaoDestino").val(),
            Peso: $("#txtPeso").val(),
            Excluir: false
        };

        var destinos = $("#hddDestinos").val() == "" ? new Array() : JSON.parse($("#hddDestinos").val());

        destinos.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (destino.Codigo == 0)
            destino.Codigo = (destinos.length > 0 ? (destinos[0].Codigo > 0 ? -1 : (destinos[0].Codigo - 1)) : -1);

        if (destinos.length > 0) {
            for (var i = 0; i < destinos.length; i++) {
                if (destinos[i].Codigo == destino.Codigo) {
                    destinos.splice(i, 1);
                    break;
                }
            }
        }

        destinos.push(destino);

        $("#hddDestinos").val(JSON.stringify(destinos));

        RenderizarDestino();
        LimparCamposDestino();
        AtualizarValores();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "mensagensDestinos-placeholder");
    }
}

function EditarDestino(destino) {
    $("#hddCodigoDestino").val(destino.Codigo);

    $("#txtTipoDaCarga").val(destino.DescricaoTipoCarga).data('Codigo', destino.CodigoTipoCarga);
    $("#txtCTe").val(destino.DescricaoCTe).data('Codigo', destino.CodigoCTe);
    $("#txtCliente").val(destino.DescricaoCliente).data('Codigo', destino.CodigoCliente);
    
    $("#txtKMInicial").val(Globalize.format(destino.KMInicial, "n0"));
    $("#txtKMFinal").val(Globalize.format(destino.KMFinal, "n0"));
    $("#txtDataInicial").val(destino.DataInicial);
    $("#txtDataFinal").val(destino.DataFinal);
    $("#selUFOrigem").val(destino.UFOrigem);
    BuscarLocalidades(destino.UFOrigem, "selMunicipioOrigem", destino.MunicipioOrigem);
    $("#selUFDestino").val(destino.UFDestino);
    BuscarLocalidades(destino.UFDestino, "selMunicipioDestino", destino.MunicipioDestino);
    $("#txtPeso").val(destino.Peso);
    $("#txtValorFrete").val(destino.ValorFrete);
    $("#txtValorUnitario").val(destino.ValorUnitario);
    $("#txtOutrosDescontos").val(destino.OutrosDescontos);
    $("#txtObservacaoDestino").val(destino.Observacao);
    $("#btnExcluirDestino").show();
    $("#btnCancelarDestino").show();
}

function ExcluirDestino() {
    jConfirm("Deseja realmente excluir este destino da viagem?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddCodigoDestino").val());
            var destinos = $("#hddDestinos").val() == "" ? new Array() : JSON.parse($("#hddDestinos").val());
            for (var i = 0; i < destinos.length; i++) {
                if (destinos[i].Codigo == codigo) {
                    if (codigo > 0) {
                        destinos[i].Excluir = true;
                    } else {
                        destinos.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddDestinos").val(JSON.stringify(destinos));
            RenderizarDestino();
            LimparCamposDestino();
            AtualizarValores();
        }
    });
}

function RenderizarDestino() {
    var itens = $("#hddDestinos").val() == "" ? new Array() : JSON.parse($("#hddDestinos").val());
    var $tabela = $("#tblDestinos");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var $row = $("<tr>" +
                "<td>" + info.DescricaoCTe + "</td>" +
                "<td>" + info.DescricaoTipoCarga + "</td>" +
                "<td>" + info.DataInicial + "</td>" +
                "<td>" + info.DataFinal + "</td>" +
                "<td>" + info.DescricaoOrigem + "</td>" +
                "<td>" + info.DescricaoDestino + "</td>" +
                "<td>" + info.Peso + "</td>" +
                "<td>" + info.ValorFrete + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                EditarDestino(info);
            });

            $tabela.find("tbody").append($row);
        }
    });
    
    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function BuscarDestinos(acertoDeViagem) {
    executarRest("/DestinoDoAcertodeViagem/BuscarPorAcertoDeViagem?callback=?", { CodigoAcertoViagem: acertoDeViagem.Codigo }, function (r) {
        if (r.Sucesso) {
            $("#hddDestinos").val(JSON.stringify(r.Objeto));
            RenderizarDestino();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção", "mensagensDestinos-placeholder");
        }
    });
}

function CalcularValorPorPeso() {
    var cteSelecionado = $("#txtCTe").data('Codigo') > 0;

    if (!cteSelecionado && !CalculoPorTabelaDeFrete) {
        var peso = Globalize.parseFloat($("#txtPeso").val());
        var valorUnitario = Globalize.parseFloat($("#txtValorUnitario").val());

        var valorTotal = peso * valorUnitario;

        $("#txtValorFrete").val(Globalize.format(valorTotal, "n2"));
    }
}

function CarregarTabelasDeFrete() {
    executarRest("/FretePorKMTipoDeVeiculo/ObterTabelasDeFreteAcerto?callback=?", {}, function (r) {
        if (r.Sucesso && r.Objeto.length > 0) {
            TabelaDeFreteEmpresa = r.Objeto;
        } else {
            ExibirMensagemErro(r.Erro, "Atenção", "mensagensDestinos-placeholder");
        }
    });
}

function CalcularValorPorTabelaDeFrete() {
    if(TabelaDeFreteEmpresa != null) {
        var diferencaKm = Globalize.parseFloat($("#txtKMFinal").val()) - Globalize.parseFloat($("#txtKMInicial").val());
        var tipoVeiculo = $("#txtVeiculo").data("TipoVeiculo");
        var valorTotal = 0;

        if (diferencaKm < 0) return;

        for (var i in TabelaDeFreteEmpresa) {
            if (TabelaDeFreteEmpresa[i].TipoVeiculo == tipoVeiculo) {
                CalculoPorTabelaDeFrete = true;
                var tabelaCompativel = TabelaDeFreteEmpresa[i];
                valorTotal = tabelaCompativel.Valor;
                if (diferencaKm > tabelaCompativel.KMFranquia) {
                    valorTotal += (diferencaKm - tabelaCompativel.KMFranquia) * tabelaCompativel.ExcedentePorKM;
                }
                $("#txtValorFrete").val(Globalize.format(valorTotal, "n2"));
                
                break;
            }
        }
    }
}
function BuscaFreteVeiculo() {
    CalculoPorTabelaDeFrete = false;
    if (TabelaDeFreteEmpresa != null) {
        var tipoVeiculo = $("#txtVeiculo").data("TipoVeiculo");

        for (var i in TabelaDeFreteEmpresa) {
            if (TabelaDeFreteEmpresa[i].TipoVeiculo == tipoVeiculo) {
                CalculoPorTabelaDeFrete = true;
                break;
            }
        }
    }
}