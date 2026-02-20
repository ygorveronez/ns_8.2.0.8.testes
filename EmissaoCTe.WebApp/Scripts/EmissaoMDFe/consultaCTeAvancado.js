/// <reference path="consultaCTe.js" />
/// <reference path="../jquery-3.0.0.js" />

$(document).ready(function () {
    $("#btnCancelarConsultaAvancada").click(function () {
        FecharTelaConsultaCTes();
    });

    $("#btnObterInformacoesCTesSelecionados").click(function () {
        FinalizarSelecaoCTes();
    });

    $("#btnSelecionarTodosOsCTes").click(function () {
        SelecionarTodosOsCTes();
    });
});

function SelecionarTodosOsCTes() {
    var dados = {
        CodigoMunicipio: $("body").data("municipioDescarregamentoDocumento") != null ? $("body").data("municipioDescarregamentoDocumento").CodigoMunicipio : 0,
        UFDescarregamento: $("body").data("ufConsultaCTe") != null ? $("body").data("ufConsultaCTe") : "",
        UFCarregamento: $("body").data("ufCarregamentoConsultaCTe") != null ? $("body").data("ufCarregamentoConsultaCTe") : "",
        DataInicial: $("#txtDataEmissaoInicialCTeConsulta").val(),
        DataFinal: $("#txtDataEmissaoFinalCTeConsulta").val(),
        NumeroInicial: $("#txtNumeroInicialCTeConsulta").val(),
        NumeroFinal: $("#txtNumeroFinalCTeConsulta").val(),
        Placa: $("#txtPlacaVeiculoCTeConsulta").val(),
        NomeMotorista: $("#txtNomeMotoristaCTeConsulta").val(),
        CPFMotorista: $("#txtCPFMotoristaCTeConsulta").val(),
        TipoServico: $("#selTipoServico").val(),
        TipoCTe: $("#selTipoCTe").val()
    };

    executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterTodosOsCTesParaEmissao?callback=?", dados, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.length > 0) {

                $("body").data("ctesSelecionadosConsultaAvancada", null);
                $("#containerCTesSelecionados").html("");

                var ctesSelecionados = new Array();
                
                for (var i = 0; i < r.Objeto.length; i++) {

                    AdicionarTag(r.Objeto[i]);

                    ctesSelecionados.push({ Codigo: r.Objeto[i].Codigo });

                }

                $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

            }
        } else {
            jAlert(r.Erro, "Atenção!");
        }
    });
}

function FinalizarSelecaoCTes() {
    var ctes = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");

    if (ctes.length > 0) {
        executarRest("/ManifestoEletronicoDeDocumentosFiscais/ObterDetalhesCTes?callback=?", { CTes: JSON.stringify(ctes) }, function (r) {
            if (r.Sucesso) {

                $("#selUFDescarregamento").val(r.Objeto.UFDescarregamento).change();

                $("#selUFCarregamento").val(r.Objeto.UFCarregamento).change();

                $("#txtCIOT").val(r.Objeto.CIOT);

                $("body").data("municipiosCarregamento", r.Objeto.MunicipiosCarregamento);
                RenderizarMunicipiosCarregamento();

                $("body").data("municipiosDescarregamento", r.Objeto.MunicipiosDescarregamento);
                RenderizarMunicipiosDescarregamento();

                $("body").data("motoristas", r.Objeto.Motoristas);
                RenderizarMotoristas();

                $("body").data("reboques", r.Objeto.Reboques);
                RenderizarReboques();

                if (r.Objeto.Veiculo != null) {
                    $("#txtPlacaVeiculo").val(r.Objeto.Veiculo.Placa);
                    $("#txtRENAVAMVeiculo").val(r.Objeto.Veiculo.RENAVAM);
                    $("#txtTaraVeiculo").val(r.Objeto.Veiculo.Tara);
                    $("#txtCapacidadeKGVeiculo").val(r.Objeto.Veiculo.CapacidadeKG);
                    $("#txtCapacidadeM3Veiculo").val(r.Objeto.Veiculo.CapacidadeM3);
                    $("#txtRNTRCVeiculo").val(r.Objeto.Veiculo.RNTRC);
                    $("#selRodadoVeiculo").val(r.Objeto.Veiculo.TipoRodado);
                    $("#selCarroceriaVeiculo").val(r.Objeto.Veiculo.TipoCarroceria);
                    $("#selUFVeiculo").val(r.Objeto.Veiculo.UF);
                    $("#txtCPFCNPJProprietarioVeiculo").val(r.Objeto.Veiculo.CPFCNPJ);
                    $("#txtIEProprietarioVeiculo").val(r.Objeto.Veiculo.IE);
                    $("#txtNomeProprietarioVeiculo").val(r.Objeto.Veiculo.Nome);
                    $("#selUFProprietarioVeiculo").val(r.Objeto.Veiculo.UFProprietario);
                    $("#selTipoProprietarioVeiculo").val(r.Objeto.Veiculo.TipoProprietario);

                    if (r.Objeto.Veiculo.CNPJFornecedorValePedagio != "") {

                        var valePedagio = {
                            Codigo: $("body").data("valePedagio") != null ? $("body").data("valePedagio").Codigo : 0,
                            CNPJFornecedor: r.Objeto.Veiculo.CNPJFornecedorValePedagio,
                            CNPJResponsavel: r.Objeto.Veiculo.CNPJResponsavelValePedagio,
                            NumeroComprovante: r.Objeto.Veiculo.NumeroCompraValePedagio,
                            ValorValePedagio: r.Objeto.Veiculo.ValorValePedagio,
                            Excluir: false
                        };

                        var valesPedagio = $("body").data("valesPedagio") == null ? new Array() : $("body").data("valesPedagio");
                        valesPedagio.push(valePedagio);
                        $("body").data("valesPedagio", valesPedagio);
                        RenderizarValesPedagio();
                    }
                }

                // Trata o retorno dos seguros
                var seguros = r.Objeto.Seguros;
                for (var i in seguros) {
                    var seguro = seguros[i];
                    // Converter o responsavel do seguro de:
                    // (Remetente, Expedidor, Recebedor, Destinatario, Emitente_CTE, Tomador_Servico) => (Emitente, Contratante)
                    var responsavel = ConverteResponsavelSeguro(seguro.Responsavel);
                    InsereSeguro({
                        Tipo: responsavel.Id,
                        DescricaoTipo: responsavel.Id + " - " + responsavel.Descricao,
                        Responsavel: seguro.CNPJResponsavel,
                        Seguradora: seguro.Seguradora,
                        NumeroApolice: seguro.NumeroApolice,
                        NumeroAverbacao: seguro.NumeroAverberacao,
                        CNPJSeguradora: seguro.CNPJSeguradora,
                    });
                }

                // Trata o retorno dos tomadores
                var tomadores = r.Objeto.Contratantes;
                for (var i in tomadores) {
                    var tomador = tomadores[i];
                    InsereTomador({
                        Nome: tomador.Nome,
                        CPF_CNPJ: tomador.CPF_CNPJ,
                    });
                }

                // Trata o retorno dos CIOTs
                var ciots = r.Objeto.CIOTs;
                for (var i in ciots) {
                    var ciot = ciots[i];
                    InsereCIOT({
                        CIOT: ciot.CIOT,
                        CPF_CNPJ: ciot.CNPJ,
                    });
                }

                FecharTelaConsultaCTes();

                AtualizarTotais();

            } else {
                ExibirMensagemErro(r.Erro, "Atenção", "placeholder-msgConsultaCTes");
            }
        });
    } else {
        FecharTelaConsultaCTes();
    }
}

function AbrirTelaConsultaCTeAvancado() {
    $("#divConsultaCTes .modal-footer").show();
    AbrirTelaConsultaCTes(RetornoConsultaCTeAvancado, false);
}

function RetornoConsultaCTeAvancado(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");

    if (ctesSelecionados.length <= 0) {
        $("body").data("ufConsultaCTe", cte.data.UFDescarregamento);
        $("body").data("ufCarregamentoConsultaCTe", cte.data.UFCarregamento);
        ConsultarCTes(RetornoConsultaCTeAvancado);
        $("#btnSelecionarTodosOsCTes").removeClass("disabled");
    }

    for (var i = 0; i < ctesSelecionados.length; i++) {
        if (ctesSelecionados[i].Codigo == cte.data.Codigo)
            return;
    }

    ctesSelecionados.push({ Codigo: cte.data.Codigo });

    $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

    AdicionarTag(cte.data);
}

function SetarUFConsultaAvancadaCTe(uf) {
    $("body").data("ufConsultaCTe", uf);
}

function AdicionarTag(cte) {
    var tag = document.createElement("li");
    tag.className = "tag-item tag-item-delete-experience";
    tag.id = "cteSelecionado_" + cte.Codigo;

    var container = document.createElement("span");
    container.className = "tag-container tag-container-delete-experience";

    var descricao = document.createElement("span");
    descricao.className = "tag-box tag-box-delete-experience";
    descricao.innerHTML = "<b>" + cte.Numero + "</b> | " + cte.ValorFrete;

    var opcaoExcluir = document.createElement("span");
    opcaoExcluir.className = "tag-delete tag-box tag-box-delete-experience";
    opcaoExcluir.innerHTML = "&nbsp;";
    opcaoExcluir.onclick = function () { RemoverCTeSelecionado(cte) };

    container.appendChild(descricao);
    container.appendChild(opcaoExcluir);

    tag.appendChild(container);

    document.getElementById("containerCTesSelecionados").appendChild(tag);
}

function RemoverCTeSelecionado(cte) {
    var ctesSelecionados = $("body").data("ctesSelecionadosConsultaAvancada") == null ? new Array() : $("body").data("ctesSelecionadosConsultaAvancada");

    for (var i = 0; i < ctesSelecionados.length; i++) {
        if (ctesSelecionados[i].Codigo == cte.Codigo)
            ctesSelecionados.splice(i, 1);
    }

    $("body").data("ctesSelecionadosConsultaAvancada", ctesSelecionados);

    $("#cteSelecionado_" + cte.Codigo).remove();

    if (ctesSelecionados.length <= 0) {
        $("body").data("ufConsultaCTe", null);
        $("body").data("ufCarregamentoConsultaCTe", null);
        ConsultarCTes(RetornoConsultaCTeAvancado);
    }
}