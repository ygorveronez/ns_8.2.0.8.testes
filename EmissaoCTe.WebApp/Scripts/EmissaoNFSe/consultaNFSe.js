$(document).ready(function () {
    CarregarConsultadeClientes("btnBuscarTomadorNFSeFiltro", "btnBuscarTomadorNFSeFiltro", RetornoConsultaTomadorFiltro, true, false);

    $("#txtDataEmissaoInicialNFSeFiltro").datepicker({});
    $("#txtDataEmissaoFinalNFSeFiltro").datepicker({});

    $("#txtDataEmissaoInicialNFSeFiltro").mask("99/99/9999");
    $("#txtDataEmissaoFinalNFSeFiltro").mask("99/99/9999");

    $("#txtNumeroInicialNFSeFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });
    $("#txtNumeroFinalNFSeFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });
    $("#txtNumeroRPSFiltro").priceFormat({ limit: 14, centsLimit: 0, centsSeparator: '', thousandsSeparator: '' });
    
    var today = new Date();
    var date = new Date(today);
    date.setDate(today.getDate() - 1);
    $("#txtDataEmissaoInicialNFSeFiltro").val(Globalize.format(date, "dd/MM/yyyy"));
    $("#txtDataEmissaoFinalNFSeFiltro").val(Globalize.format(today, "dd/MM/yyyy"));

    ConsultarNFSes();

    $("#btnConsultarNFSe").click(function () {
        ConsultarNFSes();
    });

    $("#selStatusNFSeFiltro").change(function () {
        var status = $("#selStatusNFSeFiltro").val();

        if (status == "9" || status == "0")
            $("#btnEmitirTodasNFSes").show();
        else
            $("#btnEmitirTodasNFSes").hide();
    });

    $("#btnEmitirTodasNFSes").click(function () {
        EmitirTodasNFSes();
    });

    $("#txtTomadorNFSeFiltro").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                LimparCamposTomadorFiltro();
            } else {
                e.preventDefault();
            }
        }
    });

});

function RetornoConsultaTomadorFiltro(cliente) {
    $("#hddTomadorFiltro").val(cliente.CPFCNPJ);
    $("#txtTomadorNFSeFiltro").val(cliente.CPFCNPJ + " " + cliente.Nome);
}

function LimparCamposTomadorFiltro() {
    $("#hddTomadorFiltro").val("");
    $("#txtTomadorNFSeFiltro").val("");
}

function EmitirTodasNFSes() {
    var _limitededias = 2;
    var dados = DadosFiltroNFSe();

    if (dados.DataInicial == "" || dados.DataFinal == "")
        return ExibirMensagemAlerta("Data de início e fim são obrigatórias.", "Atenção");

    var invertePadrao = function (data) {
        // Converte de data pt pra es
        data = data.split('/');
        data = [data[1], data[0], data[2]].join('/');

        // retorna instancia de data
        return new Date(data);
    }

    var dataInicial = invertePadrao(dados.DataInicial);
    var dataFinal = invertePadrao(dados.DataFinal);
    var diferencaData = dataFinal.getTime() - dataInicial.getTime();

    // Remove ms
    diferencaData = diferencaData / 1000;

    // Remove minutos
    diferencaData = diferencaData / 60;

    // Remove horas
    diferencaData = diferencaData / 60;

    // Remove horas
    diferencaData = diferencaData / 24;

    // Diferenca de data maior que _limitededias dias
    if (diferencaData > _limitededias)
        return ExibirMensagemAlerta("Não é possível emitir NFS-es com mais de " + _limitededias + " dia" + (_limitededias > 1 ? "s" : "") + " de diferença.", "Atenção");

    jConfirm("Deseja realmente emitir todos as NFS-es filtradas?", "Atenção", function (ret) {
        if (ret) {
            executarRest("/NotaFiscalDeServicosEletronica/EmitirTodas?callback=?", dados, function (r) {
                if (r.Sucesso) ExibirMensagemSucesso("NFS-es emitidos com sucesso.", "Sucesso!");
                else ExibirMensagemErro(r.Erro, "Erro!");
                ConsultarNFSes();
            });
        }
    });
}

function DadosFiltroNFSe()
{
    var dados = {
        DataInicial: $("#txtDataEmissaoInicialNFSeFiltro").val(),
        DataFinal: $("#txtDataEmissaoFinalNFSeFiltro").val(),
        NumeroInicial: $("#txtNumeroInicialNFSeFiltro").val(),
        NumeroFinal: $("#txtNumeroFinalNFSeFiltro").val(),
        Serie: $("#selSerieNFSeFiltro").val(),
        Status: $("#selStatusNFSeFiltro").val(),
        NumeroRPS: $("#txtNumeroRPSFiltro").val(),
        NumeroDocumento: $("#txtNumeroDocumentoFiltro").val(),
        Tomador: $("#hddTomadorFiltro").val(),
        inicioRegistros: 0
    };
    return dados;
}

function VoltarAoTopoDaTela() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
}

function ConsultarNFSes() {
    var dados = DadosFiltroNFSe();

    var opcoes = new Array();
    opcoes.push({ Descricao: "Editar", Evento: EditarNFSe });
    opcoes.push({ Descricao: "Duplicar", Evento: DuplicarNFSe });
    opcoes.push({ Descricao: "XML Autorização", Evento: BaixarXMLAutorizacao });
    opcoes.push({ Descricao: "DANFSE", Evento: BaixarDANFSE });
    opcoes.push({ Descricao: "Cancelar", Evento: CancelarNFSe });
    opcoes.push({ Descricao: "Informar Cancelamento/Inutilização Prefeitura", Evento: CancelarNFSePrefeitura });
    opcoes.push({ Descricao: "Excluir", Evento: ExcluirNFSe });
    opcoes.push({ Descricao: "Retorno", Evento: VisualizarRetorno });
    opcoes.push({ Descricao: "Averbações", Evento: ConsultarAverbacaoNFSe });
    opcoes.push({ Descricao: "Integrações de NFSe", Evento: ConsultarIntegracaoRetorno });

    CriarGridView("/NotaFiscalDeServicosEletronica/Consultar?callback=?", dados, "tbl_nfses_table", "tbl_nfses", "tbl_paginacao_nfses", opcoes, [0], null);
}

function VisualizarRetorno(nfse) {
    jAlert(nfse.data.MensagemRetorno, "Mensagem Retorno NFS-e");
}

function DuplicarNFSe(nfse) {
    jConfirm("Deseja realmente duplicar o NFS-e nº " + nfse.data.Numero + "?", "Atenção", function (ret) {
        if (ret) {
            executarRest("/NotaFiscalDeServicosEletronica/ObterDetalhes?callback=?", { CodigoNFSe: nfse.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    PreencherNFSe(r.Objeto, true)
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
    });
}

function EditarNFSe(nfse) {
    executarRest("/NotaFiscalDeServicosEletronica/ObterDetalhes?callback=?", { CodigoNFSe: nfse.data.Codigo }, function (r) {
        if (r.Sucesso) {
            PreencherNFSe(r.Objeto, false);
            AbrirDicaParaEmissaoDeNFSe(true);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function PreencherNFSe(nfse, fromDuplicada) {
    AbrirTelaEmissaoNFSe(true);

    if (fromDuplicada == null || fromDuplicada == false) {
        $("body").data("NFSe", nfse);

        $("body").data("itens", nfse.Itens);
        RenderizarItens();
        LimparCamposItem();

        $("#txtNumero").val(nfse.Numero);
        if (nfse.NumeroRPS != "0") {
            $("#txtNumeroRPS").val(nfse.NumeroRPS);
            $("#txtNumeroRPS").prop("disabled", false);
        }
        else {            
            $("#txtNumeroRPS").val("Automático");
            $("#txtNumeroRPS").prop("disabled", true);
        }
        $("#txtDataEmissao").val(nfse.DataEmissao);
        $("#txtHoraEmissao").val(nfse.HoraEmissao);
        $("#selSerie").val(nfse.Serie);
        $("#txtNumeroSubstituicao").val(nfse.NumeroSubstituicao);
        $("#txtSerieSubstituicao").val(nfse.SerieSubstituicao);
    }
    else {
        var item = {
            Codigo: 0,
            CodigoServico: nfse.Itens[0].CodigoServico,
            DescricaoServico: nfse.Itens[0].DescricaoServico,
            Estado: nfse.Itens[0].Estado,
            Localidade: nfse.Itens[0].Localidade,
            EstadoIncidencia: nfse.Itens[0].EstadoIncidencia,
            LocalidadeIncidencia: nfse.Itens[0].LocalidadeIncidencia,
            ServicoPrestadoPais: nfse.Itens[0].ServicoPrestadoPais,
            Pais: nfse.Itens[0].Pais,
            ValorServico: nfse.Itens[0].ValorServico,
            Quantidade: nfse.Itens[0].Quantidade,
            ValorTotal: nfse.Itens[0].ValorTotal,
            ValorDescontoIncondicionado: nfse.Itens[0].ValorDescontoIncondicionado,
            ValorDescontoCondicionado: nfse.Itens[0].ValorDescontoCondicionado,
            ValorDeducoes: nfse.Itens[0].ValorDeducoes,
            BaseCalculoISS: nfse.Itens[0].BaseCalculoISS,
            AliquotaISS: nfse.Itens[0].AliquotaISS,
            ValorISS: nfse.Itens[0].ValorISS,
            ExigibilidadeISS: nfse.Itens[0].ExigibilidadeISS,
            Discriminacao: nfse.Itens[0].Discriminacao,
            Excluir: false
        };

        // Fix #5309
        /*var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");
        
        itens.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (item.Codigo == 0)
            item.Codigo = (itens.length > 0 ? (itens[0].Codigo > 0 ? -1 : (itens[0].Codigo - 1)) : -1);

        for (var i = 0; i < itens.length; i++) {
            if (itens[i].Codigo == item.Codigo) {
                itens.splice(i, 1);
                break;
            }
        }

        itens.push(item);*/
        var itens = nfse.Itens;

        for (var i in itens)
            itens[i].Codigo = -(i + 1);
        // End Fix #5309

        $("body").data("itens", itens);

        RenderizarItens();
        LimparCamposItem();

        $("#txtNumero").val("Automático")
        $("#selSerie").val(nfse.Serie);
        SetarDataEmissao();
    }

    $("#txtValorServicos").val(Globalize.format(nfse.ValorServicos, "n2"));
    $("#txtValorDeducoes").val(Globalize.format(nfse.ValorDeducoes, "n2"));
    $("#txtValorPIS").val(Globalize.format(nfse.ValorPIS, "n2"));
    $("#txtValorCOFINS").val(Globalize.format(nfse.ValorCOFINS, "n2"));
    $("#txtValorINSS").val(Globalize.format(nfse.ValorINSS, "n2"));
    $("#txtValorIR").val(Globalize.format(nfse.ValorIR, "n2"));
    $("#txtValorCSLL").val(Globalize.format(nfse.ValorCSLL, "n2"));
    $("#selISSRetido").val(nfse.ISSRetido.toString());
    $("#txtValorISSRetido").val(Globalize.format(nfse.ValorISSRetido, "n2"));
    $("#txtValorOutrasRetencoes").val(Globalize.format(nfse.ValorOutrasRetencoes, "n2"));
    $("#txtValorDescontoIncondicionado").val(Globalize.format(nfse.ValorDescontoIncondicionado, "n2"));
    $("#txtValorDescontoCondicionado").val(Globalize.format(nfse.ValorDescontoCondicionado, "n2"));
    $("#txtAliquotaISS").val(Globalize.format(nfse.AliquotaISS, "n2"));
    $("#txtBaseCalculoISS").val(Globalize.format(nfse.BaseCalculoISS, "n2"));
    $("#txtValorISS").val(Globalize.format(nfse.ValorISS, "n2"));
    $("#txtOutrasInformacoes").val(nfse.OutrasInformacoes);
    $("#lblLogNFSe").html(nfse.Log);
    $("#txtValorIBSEstadual").val(Globalize.format(nfse.ValorIBSEstadual, "n2"));
    $("#txtValorIBSMunicipal").val(Globalize.format(nfse.ValorIBSMunicipal, "n2"));
    $("#txtValorCBS").val(Globalize.format(nfse.ValorCBS, "n2"));

    StateDocumentos.set(nfse.Documentos);

    BuscarLocalidades(nfse.EstadoPrestacaoServico, "selLocalidadePrestacaoServico", nfse.LocalidadePrestacaoServico);
    $("#selEstadoPrestacaoServico").val(nfse.EstadoPrestacaoServico);

    $("#selNatureza").val(nfse.Natureza);

    if (nfse.Tomador != null) {
        if (nfse.Tomador.Exportacao)
            PreencherCamposTomadorExportacao(nfse.Tomador);
        else
            PreencherCamposTomador(nfse.Tomador);
    }
    else
        LimparCamposTomador();

    if (nfse.Intermediario != null) {
        if (nfse.Intermediario.Exportacao)
            PreencherCamposIntermediarioExportacao(nfse.Intermediario);
        else
            PreencherCamposIntermediario(nfse.Intermediario);
    } else {
        LimparCamposIntermediario();
    }
}

function BaixarDANFSE(nfse) {
    executarDownload("/NotaFiscalDeServicosEletronica/DownloadDANFSE", { CodigoNFSe: nfse.data.Codigo });
}

function BaixarXMLAutorizacao(nfse) {
    executarDownload("/NotaFiscalDeServicosEletronica/DownloadXMLAutorizacao", { CodigoNFSe: nfse.data.Codigo });
}

function ExcluirNFSe(nfse) {
    jConfirm("Deseja realmente excluir a NFS-e " + nfse.data.Numero + "? <b>Este processo é irreversível!</b>", "Atenção!", function (resposta) {
        if (resposta) {
            executarRest("/NotaFiscalDeServicosEletronica/Excluir?callback=?", { CodigoNFSe: nfse.data.Codigo }, function (r) {
                if (r.Sucesso) {

                    ExibirMensagemSucesso("NFS-e excluída com sucesso!", "Sucesso!");

                    ConsultarNFSes();

                } else {
                    ExibirMensagemErro(r.Erro, "Falha!");
                }
            });
        }
    });

}