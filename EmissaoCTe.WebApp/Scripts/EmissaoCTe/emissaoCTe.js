var path = "";
if (document.location.pathname.split("/").length > 1) {
    var paths = document.location.pathname.split("/");
    for (var i = 0; (paths.length - 1) > i; i++) {
        if (paths[i] != "") {
            path += "/" + paths[i];
        }
    }
}

var janelaDicas = null;
var ConfiguracoesEmpresa = {};
var EmissaoCTe = {};
var codigoOutrasAliquotas = 0;

$(document).ready(function () {
    BuscarConfiguracoesEmpresa();
    BuscarDadosEmpresa();
    CarregarConsultaDeObservacoesPorTipo("txtObservacaoGeral", "btnBuscarObservacaoGeral", 2, RetornoConsultaObservacaoGeral, true, false);

    $("#txtHoraEmissao").mask("99:99");
    FormatarCampoDate("txtDataEmissao");

    FormatarCampoDateTime("txtDataHoraViagem");

    $("#btnNovoCTe").click(function () {
        NovoCTe(true);
        ControlarCamposCTeOS(true);
        $("#tabsEmissaoCTe a[href='#tabDados']").tab("show");
        if (janelaDicas != null)
            janelaDicas.close();
    });

    $("#ddlNaturezaOperacao").change(function () {
        BuscarCFOPs(null);
    });

    $("#btnEmitirCTe").click(function () {
        emitirCTeComValidacao('1', this);
    });

    $("#btnSalvarCTe").click(function () {
        emitirCTeComValidacao('0');
    });

    $("#btnEmitirCTeContingencia").click(function () {
        if ($("#selFormaEmissaoCTe").val() == "5") //FSDA
        {
            jConfirm("Contingência FSDA deve ser impressa 2 vias em formulário de segurança, deseja realmente emitir?", "Atenção", function (ret) {
                if (ret) {
                    $("#hddTipoEmissao").val('1');
                    EmitirCTe_Menu({ Codigo: $("#hddCodigoCTE").val() }, $("#selFormaEmissaoCTe").val());
                    FecharTelaContingenciaCTe();
                }
            });
        }
        else {
            jConfirm("Deseja realmente emitir este CT-e em contingência?", "Atenção", function (ret) {
                if (ret) {
                    $("#hddTipoEmissao").val('1');
                    EmitirCTe_Menu({ Codigo: $("#hddCodigoCTE").val() }, $("#selFormaEmissaoCTe").val());
                    FecharTelaContingenciaCTe();
                }
            });
        }
    });

    $("#btnCancelarCTe").click(function () {
        LimparDadosCTe();
        FecharTelaEmissaoCTe();
    });

    $("#btnCancelarCTeContingencia").click(function () {
        FecharTelaContingenciaCTe();
    });

    $("#btnAbrirDicaEmissaoCTe").click(function () {
        AbrirDicaParaEmissaoDeCTe(false, null);
    });

    $("#btnAbrirDica").click(function () {
        AbrirDicaNovaJanela(false, null);
    });

    $("#btnAbrirArquivosDica").click(function () {
        AlternaTabelaArquivos();
    });

    $("#divEmissaoCTe").on("hide.bs.modal", function () {
        ClassesVersao(EmissaoCTe);
        FecharTabelaArquivos();
    });

    CarregarConsultaIBSCBSCstClassTrib("btnBuscarIBSCBS_CST_Class", "btnBuscarIBSCBS_CST_Class", RetornoConsultaIBSCBSCstClassTrib, true, false);

});

function RetornoConsultaIBSCBSCstClassTrib(outrasAliquotas) {
    if (!outrasAliquotas.CST) {
        LimparCamposIbsCbs(true);
    }

    codigoOutrasAliquotas = outrasAliquotas.Codigo;
    $("#txtIBSCBS_CST").val(outrasAliquotas.CST);
    $("#txtIBSCBS_Class").val(outrasAliquotas.CodigoClassificacaoTributaria);

    if (exigePreenchimentoDoIbsCbs()) {
        BuscarAliquotasPorClass();
    } else {
        LimparCamposIbsCbs(true);
    }

    exibirRowIbsCbs(exigePreenchimentoDoIbsCbs());
}

function emitirCTeComValidacao(tipoEmissao, botaoOpcional) {

    var $botao = botaoOpcional ? $(botaoOpcional) : null;
    if ($botao) {
        $botao.prop("disabled", true);
    }

    var possuiCSTIBSCBS = $('#txtIBSCBS_CST').val() != null && $('#txtIBSCBS_CST').val() !== "";

    var msg = exibirAlertaPreenchimentoIBSCBS(possuiCSTIBSCBS);

    function concluirEmissao() {
        $("#hddTipoEmissao").val(tipoEmissao);
        EmitirCTe('1');
        if (janelaDicas != null) {
            janelaDicas.close();
        }
    }

    if (msg !== "") {
        jConfirm(msg, "Atenção", function (res) {
            if (res) {
                concluirEmissao();
            } else {
                // Se o usuário cancelou, reabilita o botão (se tiver)
                if ($botao) {
                    $botao.prop("disabled", false);
                }
            }
        });
    } else {
        concluirEmissao();
    }
}

function exibirAlertaPreenchimentoIBSCBS(possuiDadosIBSCBS) {
    var dadosEmpresa = JSON.parse($("#hddDadosEmpresa").val());

    if (!dadosEmpresa) return "";

    try {
        var empresa = JSON.parse(dadosEmpresa);
    } catch (e) {
        return "";
    }

    var regime = empresa.RegimeTributarioCTe;

    function obterDescricao(regimeTributarioCTe) {
        switch (regimeTributarioCTe) {
            case 1: return "Simples Nacional";
            case 2: return "Simples Nacional, excesso sublimite de receita bruta";
            case 3: return "Regime Normal";
            case 4: return "Simples Nacional, Microempreendedor Individual MEI";
            default: return "";
        }
    }

    var descricaoRegime = regime ? obterDescricao(regime) : "";

    const msgAlerta =
        "O Regime Tributário da Empresa atual é " + descricaoRegime + ", e foram informados dados de IBS e CBS. " +
        "A necessidade de envio deverá ser confirmada com o contador ou o responsável jurídico da empresa.<br><br>" +
        "Deseja enviar as informações de IBS e CBS para a SEFAZ mesmo assim?";

    if (!possuiDadosIBSCBS) {
        return "";
    }

    const dataEstaVigente = obrigatorioInformarIBSCBS(empresa);

    if (!dataEstaVigente) {
        return msgAlerta;
    }

    if (dataEstaVigente && regime !== 3) {
        return msgAlerta;
    }

    return "";
}

function obrigatorioInformarIBSCBS(empresa) {
    function toDate(valor) {
        if (valor instanceof Date) return valor;

        if (typeof valor !== 'string') return null;

        // tira espaços e aspas simples/duplas das pontas
        let s = valor.trim().replace(/^['"]|['"]$/g, '');

        // Caso "2026-01-01T00:00:00"
        if (s.includes('T')) {
            const [parteData] = s.split('T'); // "2026-01-01"
            s = parteData;
        }

        // Agora tratamos formatos com "-"
        if (s.includes('-')) {
            const [ano, mes, dia] = s.split('-');
            return new Date(+ano, +mes - 1, +dia);
        }

        // Ou com "/" (dd/MM/yyyy)
        if (s.includes('/')) {
            const [dia, mes, ano] = s.split('/');
            return new Date(+ano, +mes - 1, +dia);
        }

        return null;
    }

    const vigencia = toDate(empresa.DataVigenciaReformaTributaria);

    const hoje = new Date();
    const dataAtual = new Date(hoje.getFullYear(), hoje.getMonth(), hoje.getDate());
    const dataVigencia = new Date(vigencia.getFullYear(), vigencia.getMonth(), vigencia.getDate());

    return dataAtual.getTime() >= dataVigencia.getTime();
}

function VersaoCTe() {
    var classBody = "";
    var body = $("body");

    if (body.hasClass("cte-200"))
        classBody = "2.00";
    else if (body.hasClass("cte-400"))
        classBody = "4.00";

    return classBody;
}

function NovoCTe(carregarLocalidadeDeInicioEFimDaPrestacao) {
    $("#tabsEmissaoCTe a[href='#tabsCancelamento']").hide();

    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    var empresa = $("#hddDadosEmpresa").val() == "" ? null : JSON.parse($("#hddDadosEmpresa").val());

    DesbloquearCamposCTe();
    LimparDadosCTe();
    AbrirTelaEmissaoCTe(true);
    SetarDadosPadrao();
    BuscarProximoNumeroFatura();

    $("#divEmissaoCTe").on("shown.bs.modal", function () {
        AbrirDicaParaEmissaoDeCTe(true, configuracaoEmpresa);
        $("#divEmissaoCTe").off("shown.bs.modal");
    });

    if (configuracaoEmpresa.ObservacaoCTeNormal != null && configuracaoEmpresa.ObservacaoCTeNormal != "")
        $("#txtObservacaoGeral").val($("#txtObservacaoGeral").val() + configuracaoEmpresa.ObservacaoCTeNormal);

    if (empresa != null && empresa.SimplesNacional) {
        $("#selICMS").val("11");
        TrocarICMS("11", false);
    }

    if ($("#ddlUFLocalEmissaoCTe").val() != empresa.SiglaUF || $("#ddlUFInicioPrestacao").val() != empresa.SiglaUF || $("#ddlUFTerminoPrestacao").val() != empresa.SiglaUF) {
        $("#ddlUFLocalEmissaoCTe").val(empresa.SiglaUF);

        if (carregarLocalidadeDeInicioEFimDaPrestacao) {
            $("#ddlUFInicioPrestacao").val(empresa.SiglaUF);
            $("#ddlUFTerminoPrestacao").val(empresa.SiglaUF);
        }
    }

    if ($("#ddlMunicipioLocalEmissaoCTe").val() != empresa.Localidade || $("#ddlMunicipioInicioPrestacao").val() != empresa.Localidade || $("#ddlMunicipioTerminoPrestacao").val() != empresa.Localidade) {
        var selectsLocalidades = ["ddlMunicipioLocalEmissaoCTe"];

        if (carregarLocalidadeDeInicioEFimDaPrestacao)
            selectsLocalidades.push("ddlMunicipioInicioPrestacao", "ddlMunicipioTerminoPrestacao");

        BuscarLocalidades(empresa.SiglaUF, selectsLocalidades, empresa.Localidade);
    }

    $("#btnBuscarApoliceSeguro").off();

    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarApoliceSeguro", "btnBuscarApoliceSeguro", "", RetornoConsultaApoliceSeguro, true, false);
}

function AbrirDicaParaEmissaoDeCTe(somenteSeExistir, configuracao) {
    var configuracaoEmpresa = configuracao == null ? ($("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val())) : configuracao;
    var abrirModal = false;

    if (configuracaoEmpresa.DicasEmissaoCTe != null && configuracaoEmpresa.DicasEmissaoCTe != "") {
        var mensagemDicas = configuracaoEmpresa.DicasEmissaoCTe.replace(/(?:\r\n|\r|\n)/g, '<br />');
        $("#divDica").html("");
        $("#divDica").append(mensagemDicas);
        abrirModal = true;
    } else if (somenteSeExistir == null || somenteSeExistir == false)
        jAlert("Nenhuma dica registrada.", "Dicas Para a Emissão do CT-e");

    if ($.isArray(configuracaoEmpresa.ArquivosDicas) && configuracaoEmpresa.ArquivosDicas.length > 0) {
        $("#btnAbrirArquivosDica").show();
        $("#divArquivosDicas").hide();
        RenderizarArquivosDicas(configuracaoEmpresa.ArquivosDicas);
        abrirModal = true;
    } else {
        $("#btnAbrirArquivosDica").hide();
        $("#divArquivosDicas").hide();
    }

    if (abrirModal) {
        $("#divDicasCTe").modal("show");
    }
}

function FecharTelaEmissaoCTe() {
    $("#divEmissaoCTe").modal("hide");
    VoltarAoTopoDaTela();
}

function FecharTelaContingenciaCTe() {
    $("#divContingenciaCTe").modal("hide");
    VoltarAoTopoDaTela();
}

function VoltarAoTopoDaTela() {
    $("html, body").animate({ scrollTop: 0 }, "slow");
}

function AbrirTabelaArquivos() {
    $("#divArquivosDicas").slideDown();
    $("#divArquivosDicas").addClass("arquivos-abertos");
    $("#spnAbrirArquivosDicas").text(" Fechar arquivos");
    $("#spnIconeAbrirArquivosDicas").attr("class", "glyphicon glyphicon-folder-close");
}

function FecharTabelaArquivos() {
    $("#divArquivosDicas").slideUp();
    $("#divArquivosDicas").removeClass("arquivos-abertos");
    $("#spnAbrirArquivosDicas").text(" Abrir arquivos");
    $("#spnIconeAbrirArquivosDicas").attr("class", "glyphicon glyphicon-folder-open");
}

function AlternaTabelaArquivos() {
    if ($("#divArquivosDicas").hasClass("arquivos-abertos")) {
        FecharTabelaArquivos();
    } else {
        AbrirTabelaArquivos();
    }
}

function RenderizarArquivosDicas(arquivos) {
    var $tbody = $("#divArquivosDicas table tbody");
    $tbody.find("tr").remove();

    for (var i in arquivos) {
        var arquivo = arquivos[i];
        var $tr = $([
            '<tr>',
            '<td>' + arquivo.Nome + '</td>',
            //'<td><a href="' + ObterPath() + '/ConfiguracaoEmpresa/DownloadArquivoDicas?Codigo=' + arquivo.Codigo + '" target="_blank" title="Baixar arquivo de dica" class="btn btn-default btn-xs btn-block">Download</button></td>',
            '<td><button type="button" onclick="BaixarArquivoDica(' + arquivo.Codigo + ')" class="btn btn-default btn-xs btn-block">Download</button></td>',
            '</tr>',
        ].join(""));

        $tbody.append($tr);
    }
}

function BaixarArquivoDica(codigo) {
    executarDownload('/ConfiguracaoEmpresa/DownloadArquivoDicas?callback=?', { Codigo: codigo });
}

function EmitirCTeContingencia(cte) {
    $("#divContingenciaCTe").modal("show");
    $("#hddCodigoCTE").val(cte.data.Codigo);
}

function DesbloquearCamposCTe() {
    $("#divEmissaoCTe .modal-body :input").prop("disabled", false);
    $("#divEmissaoCTe .modal-body :button").prop("disabled", false);
    $("#txtNumeroProtocoloCancelamento").prop("disabled", true);
    $("#txtJustificativaCancelamento").prop("disabled", true);
    $("#txtNumeroProtocoloAutorizacao").prop("disabled", true);
    $("#txtNumeroDuplicata").prop("disabled", true);
    $("#txtParcelaDuplicata").prop("disabled", true);
    $("#txtChaveCTe").prop("disabled", true);
    $("#selTipoCTE").prop("disabled", true);
    $("#ddlModelo").prop("disabled", false);
    $("#ddlPaisTomador").prop("disabled", true);
    $("#selAliquotaICMS option").each(function () {
        $(this).prop("disabled", false);
    });

    $("#txtIBSCBS_Class").prop("disabled", true);
    $("#txtIBSCBSBaseCalculo").prop("disabled", true);

    $("#txtIBSEstadualAliquota").prop("disabled", true);
    $("#txtIBSEstadualReducao").prop("disabled", true);
    $("#txtIBSEstadualEfetiva").prop("disabled", true);
    $("#txtIBSEstadualValor").prop("disabled", true);

    $("#txtIBSMunAliquota").prop("disabled", true);
    $("#txtIBSMunReducao").prop("disabled", true);
    $("#txtIBSMunEfetiva").prop("disabled", true);
    $("#txtIBSMunValor").prop("disabled", true);

    $("#txtCBSAliquota").prop("disabled", true);
    $("#txtCBSReducao").prop("disabled", true);
    $("#txtCBSEfetiva").prop("disabled", true);
    $("#txtCBSValor").prop("disabled", true);
}

function ClassesVersao(versoes) {
    // Adiciona classe de versao cte e mdfe
    var classesCTe = ["cte-200", "cte-400"];
    var classesMDFe = ["mdfe-100", "mdfe-300"];
    var $body = $("body");
    var classeCTe = (typeof versoes.VersaoCTe == "string") ? versoes.VersaoCTe.replace('.', '') : "";
    var classeMDFe = (typeof versoes.VersaoMDFe == "string") ? versoes.VersaoMDFe.replace('.', '') : "";

    if (classeCTe != "") {
        for (var i in classesCTe) $body.removeClass(classesCTe[i]);
        $body.addClass("cte-" + classeCTe);
    }

    if (classeMDFe != "") {
        for (var i in classesMDFe) $body.removeClass(classesMDFe[i]);
        $body.addClass("mdfe-" + classeMDFe);
    }

    // Altera classes
    $("*[data-cte200], *[data-cte400],  *[data-mdfe100], *[data-mdfe300]").each(function () {
        var $this = $(this);
        var classecte = $this.data('cte' + classeCTe) || "";
        var classemdfe = $this.data('mdfe' + classeMDFe) || "";

        $this.attr('class', classecte + '' + classemdfe);
    });

    // ClassesVersao({VersaoCTe: "", VersaoMDFe: ""});
}

function BloquearCamposCTe() {
    $("#divEmissaoCTe .modal-body :input, #divEmissaoCTe .modal-body :button").prop("disabled", true);
}

function BuscarDadosEmpresa() {
    executarRest("/Empresa/ObterDetalhesDaEmpresaDoUsuario?callback=?", {}, function (r) {
        if (r.Sucesso) {
            EmissaoCTe = r.Objeto.EmissaoCTe;
            $("#hddDadosEmpresa").val(JSON.stringify(r.Objeto));

            ClassesVersao(EmissaoCTe);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function BuscarConfiguracoesEmpresa() {
    executarRest("/ConfiguracaoEmpresa/ObterDetalhes?callback=?", {}, function (r) {
        if (r.Sucesso) {
            ConfiguracoesEmpresa = r.Objeto;
            $("#ddlModelo").val(r.Objeto.ModeloPadrao);
            $("#hddConfiguracoesEmpresa").val(JSON.stringify(ConfiguracoesEmpresa));
            if (r.Objeto.ModeloPadrao == null || r.Objeto.ModeloPadrao == "" || r.Objeto.ModeloPadrao == "57")
                $("#ddlModelo").prop("disabled", true);
            else
                $("#ddlModelo").prop("disabled", false);

            if (r.Objeto.ExibirCobrancaCancelamento != null && r.Objeto.ExibirCobrancaCancelamento == true) {
                $("#divOpcoesCobrancaCancelamento").show();
                $("#divOpcoesCobrancaCancelamentoMDFe").show();
            }
            else {
                $("#divOpcoesCobrancaCancelamento").hide();
                $("#divOpcoesCobrancaCancelamentoMDFe").hide();
            }

            if (r.Objeto.UtilizaResumoEmissaoCTe != null && r.Objeto.UtilizaResumoEmissaoCTe == true) {
                $("#tabsEmissaoCTe a[href='#tabsResumo']").show();
            }
            else {
                $("#tabsEmissaoCTe a[href='#tabsResumo']").hide();
            }


        } else {
            $("#ddlModelo").prop("disabled", true);
            $("#ddlModelo").val("57")
            ControlarCamposCTeOS(true);
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RetornoConsultaObservacaoGeral(observacao) {
    $("#txtObservacaoGeral").val($("#txtObservacaoGeral").val() + observacao.Descricao);
}

function SetarDadosPadrao() {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    var hoje = new Date();

    var dataEntrega = new Date();
    dataEntrega.setDate(configuracaoEmpresa != null && configuracaoEmpresa.DiasParaEntrega > 0 ? hoje.getDate() + configuracaoEmpresa.DiasParaEntrega : hoje.getDate() + 1);

    $("#txtDataPrevistaEntregaCargaRecebedor").val(Globalize.format(dataEntrega, "dd/MM/yyyy"));
    $("#txtDataEmissao").val(Globalize.format(hoje, "dd/MM/yyyy"));
    $("#txtHoraEmissao").val(Globalize.format(hoje, "HH:mm"));
    $("#txtDataEmissaoNFeRemetente").val(Globalize.format(hoje, "dd/MM/yyyy"));
    $("#txtDataEmissaoNotaFiscalRemetente").val(Globalize.format(hoje, "dd/MM/yyyy"));
    $("#txtDataEmissaoOutrosRemetente").val(Globalize.format(hoje, "dd/MM/yyyy"));
    $("#txtProdutoPredominante").val(configuracaoEmpresa.ProdutoPredominante);
    $("#txtOutrasCaracteristicasCarga").val(configuracaoEmpresa.OutrasCaracteristicas);
    $("#chkIndicadorLotacao").prop("checked", (configuracaoEmpresa != null ? configuracaoEmpresa.IndicadorDeLotacao : false));

    $("#txtDataHoraViagem").val(Globalize.format(hoje, "dd/MM/yyyy") + " " + Globalize.format(hoje, "HH:mm"));

    $("#selFormaImpressao").val(configuracaoEmpresa != null ? configuracaoEmpresa.TipoImpressao : $("#selFormaImpressao option:first").val());

    $("#selResponsavelSeguro").val(configuracaoEmpresa != null ? configuracaoEmpresa.ResponsavelSeguro : 0);

    if (configuracaoEmpresa.ModeloPadrao == null || configuracaoEmpresa.ModeloPadrao == "" || configuracaoEmpresa.ModeloPadrao == "57")
        $("#ddlModelo").prop("disabled", true);
    else
        $("#ddlModelo").prop("disabled", false);

    if (configuracaoEmpresa.CodigoApoliceSeguro > 0) {
        InsereSeguro({
            Seguradora: configuracaoEmpresa.NomeSeguradora,
            NumeroApolice: configuracaoEmpresa.NumeroApoliceSeguro,
            CNPJSeguradora: configuracaoEmpresa.CNPJSeguradora,
            Responsavel: $("#selResponsavelSeguro").val(),
            DescricaoResponsavel: $("#selResponsavelSeguro :selected").text()
        });
    }

    var aliquotaCOFINS = Globalize.parseFloat(configuracaoEmpresa.AliquotaCOFINS || "0");
    if (configuracaoEmpresa.CSTCOFINS != "")
        $("#selCOFINS").val(configuracaoEmpresa.CSTCOFINS);
    if (aliquotaCOFINS > 0)
        $("#selAliquotaCOFINS").val(configuracaoEmpresa.AliquotaCOFINS);

    var aliquotaPIS = Globalize.parseFloat(configuracaoEmpresa.AliquotaPIS || "0");
    if (configuracaoEmpresa.CSTPIS != "")
        $("#selPIS").val(configuracaoEmpresa.CSTPIS);
    if (aliquotaPIS > 0)
        $("#selAliquotaPIS").val(configuracaoEmpresa.AliquotaPIS);

    $("#txtAliquotaIR").val(configuracaoEmpresa.AliquotaIR);
    $("#txtAliquotaINSS").val(configuracaoEmpresa.AliquotaINSS);
    $("#txtAliquotaCSLL").val(configuracaoEmpresa.AliquotaCSLL);
    if (configuracaoEmpresa.DescontarINSSValorReceber == true)
        $("#hddDescontarINSSValorReceber").val("1");
    else
        $("#hddDescontarINSSValorReceber").val("0");
}

function AbrirTelaEmissaoCTe(vincularEventoCalculoFrete) {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());

    BloquearAbasRemetente();
    BloquearAbaTomador();
    BloquearAbasCTeOutros();

    $("#tabsEmissaoCTe a[href='#tabDados']").tab("show");
    $("#tabsDestinatario a[href='#tabGeralDestinatario']").tab("show");
    $("#tabsImpostosCTe a[href='#tabICMS']").tab("show");
    $("#tabsInformacoesCarga a[href='#tabInformacoes']").tab("show");

    $("#divEmissaoCTe").modal({ keyboard: false, backdrop: 'static' });

    vincularEventoCalculoFrete ? VincularEventoCalculoFreteNaAbaServicosEImpostos() : RemoverEventoCalculoFreteNaAbaServicosEImpostos();
    vincularEventoCalculoFrete ? VincularEventoCalculoFreteNaAbaServicosEImpostosResumo() : RemoverEventoCalculoFreteNaAbaServicosEImpostosResumo();

    VincularEventoBuscarDadosImpostosNaAbaServicosEImpostos();

    if (configuracaoEmpresa != null && configuracaoEmpresa.UtilizaResumoEmissaoCTe == true) {
        $("#tabsEmissaoCTe a[href='#tabsResumo']").show();
        $("#tabsEmissaoCTe a[href='#tabsResumo']").tab("show");
    }
    else {
        $("#tabsEmissaoCTe a[href='#tabsResumo']").hide();
        $("#tabsEmissaoCTe a[href='#tabDados']").tab("show");
    }
}

function BuscarCFOPs(codigo) {
    var idNaturezaOperacao = $("#ddlNaturezaOperacao").val();
    executarRest("/CFOP/BuscarPorNaturezaDaOperacaoETipo?callback=?", { IdNaturezaOperacao: idNaturezaOperacao, Tipo: 1 }, function (r) {
        if (r.Sucesso) {
            RenderizarCFOPs(r.Objeto, codigo);
            RenderizarCFOPsResumo(r.Objeto, codigo);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function RenderizarCFOPs(cfops, codigo) {
    var selCFOPs = document.getElementById("selCFOP");
    selCFOPs.options.length = 0;
    var optn = document.createElement("option");
    optn.text = "Selecione";
    optn.value = 0;
    selCFOPs.options.add(optn);
    for (var i = 0; i < cfops.length; i++) {
        optn = document.createElement("option");
        optn.text = cfops[i].CodigoCFOP;
        optn.value = cfops[i].Codigo;
        if (codigo != null) {
            if (codigo == cfops[i].Codigo) {
                optn.setAttribute("selected", "selected");
            }
        }
        selCFOPs.options.add(optn);
    }
}

function ObterTomador() {
    if ($("#hddTomador").val().replace(/[^0-9]/g, '') != "" || $("#ddlPaisTomador").val() != "01058") {
        var tomador = {
            Exportacao: $("#chkTomadorExportacao")[0].checked,
            CPFCNPJ: $("#hddTomador").val(),
            CodigoAtividade: $("#hddAtividadeTomador").val(),
            RGIE: $("#txtRGIETomador").val(),
            RazaoSocial: $("#txtRazaoSocialTomador").val(),
            NomeFantasia: $("#txtNomeFantasiaTomador").val(),
            Telefone1: $("#txtTelefone1Tomador").val(),
            Telefone2: $("#txtTelefone2Tomador").val(),
            Endereco: $("#txtEnderecoTomador").val(),
            Numero: $("#txtNumeroTomador").val(),
            Bairro: $("#txtBairroTomador").val(),
            Complemento: $("#txtComplementoTomador").val(),
            CEP: $("#txtCEPTomador").val(),
            Emails: $("#txtEmailsTomador").val(),
            StatusEmails: $("#chkStatusEmailsTomador")[0].checked,
            EmailsContato: $("#txtEmailsContatoTomador").val(),
            StatusEmailsContato: $("#chkStatusEmailsContatoTomador")[0].checked,
            EmailsContador: $("#txtEmailsContadorTomador").val(),
            StatusEmailsContador: $("#chkStatusEmailsContadorTomador")[0].checked,
            UF: $("#ddlEstadoTomador").val(),
            Localidade: $("#selCidadeTomador").val() == null ? "0" : $("#selCidadeTomador").val(),
            SiglaPais: $("#ddlPaisTomador").val(),
            Cidade: $("#txtCidadeTomadorExportacao").val(),
            SalvarEndereco: $("#chkSalvarEnderecoTomador").prop("checked"),
            EmailsTransportador: $("#txtEmailsTransportadorTomador").val(),
            StatusEmailsTransportador: $("#chkStatusEmailsTransportadorTomador")[0].checked,
        }
        return tomador;
    } else {
        return null;
    }
}

function ObterRemetente() {
    if ($("#hddRemetente").val().replace(/[^0-9]/g, '') != "" || $("#ddlPaisRemetente").val() != "01058") {
        var remetente = {
            CPFCNPJ: $("#hddRemetente").val(),
            CodigoAtividade: $("#hddAtividadeRemetente").val(),
            RGIE: $("#txtRGIERemetente").val(),
            RazaoSocial: $("#txtRazaoSocialRemetente").val(),
            NomeFantasia: $("#txtNomeFantasiaRemetente").val(),
            Telefone1: $("#txtTelefone1Remetente").val(),
            Telefone2: $("#txtTelefone2Remetente").val(),
            Endereco: $("#txtEnderecoRemetente").val(),
            Numero: $("#txtNumeroRemetente").val(),
            Bairro: $("#txtBairroRemetente").val(),
            Complemento: $("#txtComplementoRemetente").val(),
            CEP: $("#txtCEPRemetente").val(),
            Emails: $("#txtEmailsRemetente").val(),
            StatusEmails: $("#chkStatusEmailsRemetente")[0].checked,
            EmailsContato: $("#txtEmailsContatoRemetente").val(),
            StatusEmailsContato: $("#chkStatusEmailsContatoRemetente")[0].checked,
            EmailsContador: $("#txtEmailsContadorRemetente").val(),
            StatusEmailsContador: $("#chkStatusEmailsContadorRemetente")[0].checked,
            UF: $("#ddlEstadoRemetente").val(),
            Localidade: $("#selCidadeRemetente").val() == null ? "0" : $("#selCidadeRemetente").val(),
            Exportacao: $("#chkRemetenteExportacao")[0].checked,
            SiglaPais: $("#ddlPaisRemetente").val(),
            Cidade: $("#txtCidadeRemetenteExportacao").val(),
            SalvarEndereco: $("#chkSalvarEnderecoRemetente").prop("checked"),
            EmailsTransportador: $("#txtEmailsTransportadorRemetente").val(),
            StatusEmailsTransportador: $("#chkStatusEmailsTransportadorRemetente")[0].checked
        }
        return remetente;
    } else {
        return null;
    }
}

function ObterExpedidor() {
    if ($("#hddExpedidor").val().replace(/[^0-9]/g, '') != "" || $("#ddlPaisExpedidor").val() != "01058") {
        var expedidor = {
            CPFCNPJ: $("#hddExpedidor").val(),
            CodigoAtividade: $("#hddAtividadeExpedidor").val(),
            RGIE: $("#txtRGIEExpedidor").val(),
            RazaoSocial: $("#txtRazaoSocialExpedidor").val(),
            NomeFantasia: $("#txtNomeFantasiaExpedidor").val(),
            Telefone1: $("#txtTelefone1Expedidor").val(),
            Telefone2: $("#txtTelefone2Expedidor").val(),
            Endereco: $("#txtEnderecoExpedidor").val(),
            Numero: $("#txtNumeroExpedidor").val(),
            Bairro: $("#txtBairroExpedidor").val(),
            Complemento: $("#txtComplementoExpedidor").val(),
            CEP: $("#txtCEPExpedidor").val(),
            Emails: $("#txtEmailsExpedidor").val(),
            StatusEmails: $("#chkStatusEmailsExpedidor")[0].checked,
            EmailsContato: $("#txtEmailsContatoExpedidor").val(),
            StatusEmailsContato: $("#chkStatusEmailsContatoExpedidor")[0].checked,
            EmailsContador: $("#txtEmailsContadorExpedidor").val(),
            StatusEmailsContador: $("#chkStatusEmailsContadorExpedidor")[0].checked,
            UF: $("#ddlEstadoExpedidor").val(),
            Localidade: $("#selCidadeExpedidor").val() == null ? "0" : $("#selCidadeExpedidor").val(),
            Exportacao: $("#chkExpedidorExportacao")[0].checked,
            SiglaPais: $("#ddlPaisExpedidor").val(),
            Cidade: $("#txtCidadeExpedidorExportacao").val(),
            SalvarEndereco: $("#chkSalvarEnderecoExpedidor").prop("checked"),
            EmailsTransportador: $("#txtEmailsTransportadorExpedidor").val(),
            StatusEmailsTransportador: $("#chkStatusEmailsTransportadorExpedidor")[0].checked,
        }
        return expedidor;
    } else {
        return null;
    }
}

function ObterRecebedor() {
    if ($("#hddRecebedor").val().replace(/[^0-9]/g, '') != "" || $("#ddlPaisRecebedor").val() != "01058") {
        var recebedor = {
            CPFCNPJ: $("#hddRecebedor").val(),
            CodigoAtividade: $("#hddAtividadeRecebedor").val(),
            RGIE: $("#txtRGIERecebedor").val(),
            RazaoSocial: $("#txtRazaoSocialRecebedor").val(),
            NomeFantasia: $("#txtNomeFantasiaRecebedor").val(),
            Telefone1: $("#txtTelefone1Recebedor").val(),
            Telefone2: $("#txtTelefone2Recebedor").val(),
            Endereco: $("#txtEnderecoRecebedor").val(),
            Numero: $("#txtNumeroRecebedor").val(),
            Bairro: $("#txtBairroRecebedor").val(),
            Complemento: $("#txtComplementoRecebedor").val(),
            CEP: $("#txtCEPRecebedor").val(),
            Emails: $("#txtEmailsRecebedor").val(),
            StatusEmails: $("#chkStatusEmailsRecebedor")[0].checked,
            EmailsContato: $("#txtEmailsContatoRecebedor").val(),
            StatusEmailsContato: $("#chkStatusEmailsContatoRecebedor")[0].checked,
            EmailsContador: $("#txtEmailsContadorRecebedor").val(),
            StatusEmailsContador: $("#chkStatusEmailsContadorRecebedor")[0].checked,
            UF: $("#ddlEstadoRecebedor").val(),
            Localidade: $("#selCidadeRecebedor").val() == null ? "0" : $("#selCidadeRecebedor").val(),
            Exportacao: $("#chkRecebedorExportacao")[0].checked,
            SiglaPais: $("#ddlPaisRecebedor").val(),
            Cidade: $("#txtCidadeRecebedorExportacao").val(),
            SalvarEndereco: $("#chkSalvarEnderecoRecebedor").prop("checked"),
            EmailsTransportador: $("#txtEmailsTransportadorRecebedor").val(),
            StatusEmailsTransportador: $("#chkStatusEmailsTransportadorRecebedor")[0].checked
        }
        return recebedor;
    } else {
        return null;
    }
}

function ObterDestinatario() {
    if ($("#hddDestinatario").val().replace(/[^0-9]/g, '') != "" || $("#ddlPaisDestinatario").val() != "01058") {
        var destinatario = {
            CPFCNPJ: $("#hddDestinatario").val(),
            CodigoAtividade: $("#hddAtividadeDestinatario").val(),
            RGIE: $("#txtRGIEDestinatario").val(),
            RazaoSocial: $("#txtRazaoSocialDestinatario").val(),
            NomeFantasia: $("#txtNomeFantasiaDestinatario").val(),
            Telefone1: $("#txtTelefone1Destinatario").val(),
            Telefone2: $("#txtTelefone2Destinatario").val(),
            Endereco: $("#txtEnderecoDestinatario").val(),
            Numero: $("#txtNumeroDestinatario").val(),
            Bairro: $("#txtBairroDestinatario").val(),
            Complemento: $("#txtComplementoDestinatario").val(),
            CEP: $("#txtCEPDestinatario").val(),
            Emails: $("#txtEmailsDestinatario").val(),
            StatusEmails: $("#chkStatusEmailsDestinatario")[0].checked,
            EmailsContato: $("#txtEmailsContatoDestinatario").val(),
            StatusEmailsContato: $("#chkStatusEmailsContatoDestinatario")[0].checked,
            EmailsContador: $("#txtEmailsContadorDestinatario").val(),
            StatusEmailsContador: $("#chkStatusEmailsContadorDestinatario")[0].checked,
            UF: $("#ddlEstadoDestinatario").val(),
            Localidade: $("#selCidadeDestinatario").val() == null ? "0" : $("#selCidadeDestinatario").val(),
            Exportacao: $("#chkDestinatarioExportacao")[0].checked,
            SiglaPais: $("#ddlPaisDestinatario").val(),
            Cidade: $("#txtCidadeDestinatarioExportacao").val(),
            SalvarEndereco: $("#chkSalvarEnderecoDestinatario").prop("checked"),
            EmailsTransportador: $("#txtEmailsTransportadorDestinatario").val(),
            StatusEmailsTransportador: $("#chkStatusEmailsTransportadorDestinatario")[0].checked,
            InscricaoSuframa: $("#txtSuframaDestinatario").val()
        }
        return destinatario;
    } else {
        return null;
    }
}

function ObterLocalEntregaDiferenteDestinatario() {
    if ($("#hddLocalEntregaDiferenteDestinatario").val().replace(/[^0-9]/g, '') != "") {
        var localEntrega = {
            CPFCNPJ: $("#hddLocalEntregaDiferenteDestinatario").val(),
            CodigoAtividade: $("#hddAtividadeLocalEntregaDiferenteDestinatario").val(),
            RGIE: $("#txtRGIE_LocalEntregaDiferenteDestinatario").val(),
            RazaoSocial: $("#txtRazaoSocial_LocalEntregaDiferenteDestinatario").val(),
            NomeFantasia: $("#txtNomeFantasia_LocalEntregaDiferenteDestinatario").val(),
            Telefone1: $("#txtTelefone1_LocalEntregaDiferenteDestinatario").val(),
            Telefone2: $("#txtTelefone2_LocalEntregaDiferenteDestinatario").val(),
            Endereco: $("#txtLogradouro_LocalEntregaDiferenteDestinatario").val(),
            Numero: $("#txtNumero_LocalEntregaDiferenteDestinatario").val(),
            Bairro: $("#txtBairro_LocalEntregaDiferenteDestinatario").val(),
            Complemento: $("#txtComplemento_LocalEntregaDiferenteDestinatario").val(),
            CEP: $("#txtCEP_LocalEntregaDiferenteDestinatario").val(),
            UF: $("#ddlUFLocalEntregaDiferenteDestinatario").val(),
            Localidade: $("#selLocalidade_LocalEntregaDiferenteDestinatario").val() == null ? "0" : $("#selCidadeDestinatario").val()
        };
        return localEntrega;
    } else {
        return null;
    }
}

function EmitirCTe(formaEmissao) {

    var dados = {
        CodigoCTe: $("#hddCodigoCTE").val(),
        DataEmissao: $("#txtDataEmissao").val(),
        HoraEmissao: $("#txtHoraEmissao").val(),
        TipoEmissaoCTe: $("#hddTipoEmissao").val(),
        Serie: $("#ddlSerie").val(),
        FormaPagamento: $("#selPago_APagar").val(),
        TipoServico: $("#selTipoServico").val(),
        TipoCTE: $("#selTipoCTE").val(),
        FormaImpressao: $("#selFormaImpressao").val(),
        NaturezaDaOperacao: $("#ddlNaturezaOperacao").val(),
        CFOP: $("#selCFOP").val(),
        IndicadorIETomador: $("#selIndIEToma").val(),
        IndicadorGlobalizado: $("#selGlobalizado").val(),
        RecebedorRetiraDestino: $("#chkRecebedorRetiraDestino")[0].checked,
        DetalhesRetiradaRecebedor: $("#txtDetalhesRetiradaRecebedor").val(),
        ModalTransporte: $("#ddlModalTransporte").val(),
        TomadorServico: $("#selTomadorServico").val(),
        Tomador: JSON.stringify(ObterTomador()),
        Remetente: JSON.stringify(ObterRemetente()),
        Expedidor: JSON.stringify(ObterExpedidor()),
        Recebedor: JSON.stringify(ObterRecebedor()),
        Destinatario: JSON.stringify(ObterDestinatario()),
        LocalEntregaDiferenteDestinatario: JSON.stringify(ObterLocalEntregaDiferenteDestinatario()),
        ValorFreteContratado: $("#txtValorFreteContratado").val(),
        ValorTotalPrestacaoServico: $("#txtValorTotalPrestacaoServico").val(),
        ValorAReceber: $("#txtValorAReceber").val(),
        IncluirICMSNoFrete: $("#chkIncluirICMSNoFrete")[0].checked,
        PercentualICMSRecolhido: $("#txtPercentualICMSRecolhido").val(),
        ComponentesDaPrestacao: $("#hddComponentesDaPrestacao").val(),
        ICMS: $("#selICMS").val(),
        ReducaoBaseCalculoICMS: $("#txtReducaoBaseCalculoICMS").val(),
        ValorBaseCalculoICMS: $("#txtValorBaseCalculoICMS").val(),
        AliquotaICMS: $("#selAliquotaICMS").val(),
        ValorICMS: $("#txtValorICMS").val(),
        ValorICMSDesoneracao: $("#txtValorICMSDesoneracao").val(),
        CodigoBeneficio: $("#txtCodigoBeneficio").val(),
        ValorCreditoICMS: $("#txtValorCreditoICMS").val(),
        ExibeICMSNaDacte: $("#chkExibirICMSNaDACTE")[0].checked,
        CSTPIS: $("#selPIS").val(),
        ValorBaseCalculoPIS: $("#txtValorBaseCalculoPIS").val(),
        AliquotaPIS: $("#selAliquotaPIS").val(),
        ValorPIS: $("#txtValorPIS").val(),
        CSTCOFINS: $("#selCOFINS").val(),
        ValorBaseCalculoCOFINS: $("#txtValorBaseCalculoCOFINS").val(),
        AliquotaCOFINS: $("#selAliquotaCOFINS").val(),
        ValorCOFINS: $("#txtValorCOFINS").val(),
        ValorTotalCarga: $("#txtValorTotalCarga").val(),
        ValorCargaAverbacao: $("#txtValorCargaAverbacao").val(),
        ProdutoPredominante: $("#txtProdutoPredominante").val(),
        OutrasCaracteristicasCarga: $("#txtOutrasCaracteristicasCarga").val(),
        Conteiner: $("#txtConteiner").val(),
        DataPrevistaEntregaConteiner: $("#txtDataPrevistaEntregaConteiner").val(),
        NumeroLacre: $("#txtNumeroLacre").val(),
        RNTRC: $("#txtRNTRC").val(),
        DataPrevistaEntregaCargaRecebedor: $("#txtDataPrevistaEntregaCargaRecebedor").val(),
        IndicadorLotacao: $("#chkIndicadorLotacao")[0].checked,
        SerieCTRB: $("#txtSerieCTRB").val(),
        NumeroCTRB: $("#txtNumeroCTRB").val(),
        CIOT: $("#txtCIOT").val(),
        InformacoesSeguro: $("#hddInformacoesSeguro").val(),
        ObservacoesFisco: $("#hddObservacoesFisco").val(),
        ObservacoesContribuinte: $("#hddObservacoesContribuinte").val(),
        ObservacoesGerais: $("#txtObservacaoGeral").val(),
        InformacoesQuantidadeCarga: $("#hddInformacoesQuantidadeCarga").val(),
        MunicipioLocalEmissaoCTe: $("#ddlMunicipioLocalEmissaoCTe").val(),
        MunicipioInicioPrestacao: $("#ddlMunicipioInicioPrestacao").val(),
        MunicipioTerminoPrestacao: $("#ddlMunicipioTerminoPrestacao").val(),
        Modelo: $("#ddlModelo").val(),
        Veiculos: $("#hddVeiculos").val(),
        Motoristas: $("#hddMotoristas").val(),
        TipoDocumentoRemetente: $("#selTipoDocumentoRemetente").val(),
        NFERemetente: $("#hddNotasFiscaisEletronicasRemetente").val(),
        NotasFiscaisRemetente: $("#hddNotasFiscaisRemetente").val(),
        OutrosDocumentosRemetente: $("#hddOutrosDocumentosRemetente").val(),
        CPFCNPJTomador: $("#hddTomador").val().replace(/[^0-9]/g, ''),
        ChaveCTeOriginal: $("#hddChaveCTEOriginal").val(),
        DataAnulacao: $("#txtDataEmissaoDeclaracaoCTeAnulado").val(),
        TomadorContribuinte: $("#selTomadorContribuinteICMS").val() == "1" ? true : false,
        ChaveAcessoCTeAnulacao: $("#txtChaveAcessoCTeAnulacao").val(),
        TipoDocumentoSubstituicao: $("#selTipoDocumentoTomadorContribuinte").val(),
        ChaveAcessoCTeEmitidoTomador: $("#txtChaveAcessoCTeEmitidoTomador").val(),
        ChaveAcessoNFeEmitidaTomador: $("#txtChaveAcessoNFeEmitidaTomador").val(),
        CNPJDocumentoEmitidoTomador: $("#txtCNPJNFouCTEmitidoTomador").val().replace(/[^0-9]/g, ''),
        ModeloDocumentoEmitidoTomador: $("#ddlModeloNFouCTEmitidoTomador").val(),
        SerieDocumentoEmitidoTomador: $("#txtSerieNFouCTEmitidoTomador").val(),
        SubserieDocumentoEmitidoTomador: $("#txtSubserieNFouCTEmitidoTomador").val(),
        NumeroDocumentoEmitidoTomador: $("#txtNumeroNFouCTEmitidoTomador").val(),
        ValorDocumentoEmitidoTomador: $("#txtValorNFouCTEmitidoTomador").val(),
        DataEmissaoDocumentoEmitidoTomador: $("#txtDataEmissaoNFouCTEmitidoTomador").val(),
        DocumentosDeTransporteAnterioresEletronicos: $("#hddDocsTranspAntEletronico").val(),
        DocumentosDeTransporteAnterioresPapel: $("#hddDocsTranspAntPapel").val(),
        ProdutosPerigosos: $("#hddProdutosPerigosos").val(),
        DadosCobranca: $("#hddDuplicatas").val(),
        FormaEmissao: formaEmissao,
        ObservacaoDigitacao: $("#txtObservacaoDigitacaoCTe").val(),
        InformacaoAdicionalFisco: $("#txtInformacaoAdicionalFisco").val(),
        CaracteristicaAdicionalTransporte: $("#txtCaracteristicaAdicionalTransporte").val(),
        CaracteristicaAdicionalServico: $("#txtCaracteristicaAdicionalServico").val(),
        CodigoColeta: $("body").data("codigoColeta"),
        Duplicado: $("body").data("cteDuplicado"),
        TipoEnvio: "0",
        ValorBaseCalculoIR: $("#txtValorBaseCalculoIR").val(),
        AliquotaIR: $("#txtAliquotaIR").val(),
        ValorIR: $("#txtValorIR").val(),
        ValorBaseCalculoINSS: $("#txtValorBaseCalculoINSS").val(),
        AliquotaINSS: $("#txtAliquotaINSS").val(),
        ValorINSS: $("#txtValorINSS").val(),
        ValorBaseCalculoCSLL: $("#txtValorBaseCalculoCSLL").val(),
        AliquotaCSLL: $("#txtAliquotaCSLL").val(),
        ValorCSLL: $("#txtValorCSLL").val(),
        Percursos: JSON.stringify($("body").data("percursos")),
        DescricaoComplemento: $("#txtDescricaoComponentePrestacaoServico").val(),
        TipoFretamento: $("#selTipoFretamento").val(),
        DataHoraViagem: $("#txtDataHoraViagem").val(),
        CodigoCTEReferenciado: $("#hddCodigoCTEReferenciado").val(),
        IndNegociavel: $("#selIndNegociavel").val(),
        SubstituicaoTomador: $("#selSubTomador").val(),

        CodigoOutrasAliquotas: codigoOutrasAliquotas,
        CSTIBSCBS: $("#txtIBSCBS_CST").val(),
        ClassificacaoTributariaIBSCBS: $("#txtIBSCBS_Class").val(),
        BaseCalculoIBSCBS: $("#txtIBSCBSBaseCalculo").val(),

        AliquotaIBSEstadual: $("#txtIBSEstadualAliquota").val(),
        PercentualReducaoIBSEstadual: $("#txtIBSEstadualReducao").val(),
        AliquotaIBSEstadualEfetiva: $("#txtIBSEstadualEfetiva").val(),
        ValorIBSEstadual: $("#txtIBSEstadualValor").val(),

        AliquotaIBSMunicipal: $("#txtIBSMunAliquota").val(),
        PercentualReducaoIBSMunicipal: $("#txtIBSMunReducao").val(),
        AliquotaIBSMunicipalEfetiva: $("#txtIBSMunEfetiva").val(),
        ValorIBSMunicipal: $("#txtIBSMunValor").val(),

        AliquotaCBS: $("#txtCBSAliquota").val(),
        PercentualReducaoCBS: $("#txtCBSReducao").val(),
        AliquotaCBSEfetiva: $("#txtCBSEfetiva").val(),
        ValorCBS: $("#txtCBSValor").val()
    };

    // Desabilita botoes de acoes
    $("#btnEmitirCTe, #btnSalvarCTe").prop("disabled", true);

    executarRest("/ConhecimentoDeTransporteEletronico/SalvarDados?callback=?", dados, function (r) {
        if (r.Sucesso) {
            jAlert("CT-e " + ($("#hddTipoEmissao").val() == "1" ? "emitido" : "salvo") + " com sucesso!", "Sucesso", function () {
                FecharTelaEmissaoCTe();
                LimparDadosCTe();
                AtualizarGridCTes();

                // Apenas habilita botoes de acoes depois que fechar o modal
                $("#btnEmitirCTe, #btnSalvarCTe").prop("disabled", false);
            });

            // Realoca o codigo do CTE para garantir 
            // #Fix = Quando duplicado o CTE, salvo e fechado confirmacao no X repetidamente, duplicava varios CTEs
            $("#hddCodigoCTE").val(r.Objeto.CodigoCTE);
        } else {
            if (r.Objeto != null && r.Objeto.Codigo != null)
                $("#hddCodigoCTE").val(r.Objeto.Codigo);
            jAlert(r.Erro, "Atenção");

            // Ou quando acontece algum erro
            $("#btnEmitirCTe, #btnSalvarCTe").prop("disabled", false);
        }
    });
}

function ReiniciaNatureza() {
    var ordem = "1"; //var ordem = $("#ddlNaturezaOperacao option").size() == 2 ? "1" : "0";

    $("#ddlNaturezaOperacao").val($("#ddlNaturezaOperacao option:eq(" + ordem + ")").val());

    if (!$("body").data("cteDuplicado") || ordem == 0)
        $("#ddlNaturezaOperacao").change();
}

function LimparDadosCTe() {
    $("body").data("codigoColeta", null);
    $("#hddCodigoCTE").val('0');
    $("#txtNumero").val("Automático");
    $("#ddlSerie").val($("#ddlSerie option:first").val());
    $("#selPago_APagar").val($("#selPago_APagar option:first").val());
    $("#selTipoServico").val($("#selTipoServico option:first").val());
    $("#selTipoCTE").val($("#selTipoCTE option:first").val());
    $("#selFormaImpressao").val($("#selFormaImpressao option:first").val());
    // Fix #5330 
    ReiniciaNatureza();
    $("body").data("cteDuplicado", false);
    // End Fix #5330
    $("#selCFOP").html("");
    $("#selIndIEToma").val("1");
    $("#selGlobalizado").val("0");
    $("#txtChaveCTe").val('');
    $("#txtNumeroProtocoloCancelamento").val('');
    $("#txtJustificativaCancelamento").val('');
    $("#txtNumeroProtocoloAutorizacao").val('');
    $("#txtDataEmissao").val('');
    $("#txtHoraEmissao").val('');
    $("#chkRecebedorRetiraDestino").prop('checked', false);
    $("#txtDetalhesRetiradaRecebedor").val('');
    $("#ddlModalTransporte").val($("#ddlModalTransporte option:first").val());
    $("#selTomadorServico").val($("#selTomadorServico option:first").val());
    $("#hddNotasFiscaisEletronicasRemetente").val('');
    $("#hddNotasFiscaisRemetente").val('');
    $("#hddOutrosDocumentosRemetente").val('');
    $("#txtValorFreteContratado").val(Globalize.format(0, "n2"));
    $("#txtValorTotalPrestacaoServico").val(Globalize.format(0, "n2"));
    $("#txtValorAReceber").val(Globalize.format(0, "n2"));
    $("#chkIncluirICMSNoFrete").prop('checked', false);
    $("#txtPercentualICMSRecolhido").val(Globalize.format(0, "n2"));
    $("#hddComponentesDaPrestacao").val('');
    $("#selICMSResumo").val($("#selICMSResumo option:first").val());
    $("#selICMS").val($("#selICMS option:first").val());
    TrocarICMS($("#selICMS").val(), false);
    $("#txtReducaoBaseCalculoICMS").val(Globalize.format(0, "n2"));
    $("#txtValorBaseCalculoICMS").val(Globalize.format(0, "n2"));
    $("#selAliquotaICMS").val($("#selAliquotaICMS option:first").val());
    $("#txtValorICMS").val(Globalize.format(0, "n2"));
    $("#txtValorICMSDesoneracao").val(Globalize.format(0, "n2"));
    $("#txtValorCreditoICMS").val(Globalize.format(0, "n2"));
    $("#chkExibirICMSNaDACTE").prop("checked", true);
    $("#selPIS").val($("#selPIS option:first").val());
    $("#txtValorBaseCalculoPIS").val(Globalize.format(0, "n2"));
    $("#selAliquotaPIS").val($("#selAliquotaPIS option:first").val());
    $("#txtValorPIS").val(Globalize.format(0, "n2"));
    $("#selCOFINS").val($("#selCOFINS option:first").val());
    $("#txtValorBaseCalculoCOFINS").val(Globalize.format(0, "n2"));
    $("#selAliquotaCOFINS").val($("#selAliquotaCOFINS option:first").val());
    $("#txtValorCOFINS").val(Globalize.format(0, "n2"));
    $("#txtValorTotalCarga").val(Globalize.format(0, "n2"));
    $("#txtValorCargaAverbacao").val(Globalize.format(0, "n2"));
    $("#txtProdutoPredominante").val('');
    $("#txtOutrasCaracteristicasCarga").val('');
    $("#txtConteiner").val('');
    $("#txtDataPrevistaEntregaConteiner").val('');
    $("#txtNumeroLacre").val('');
    $("#txtDataPrevistaEntregaCargaRecebedor").val('');
    $("#chkIndicadorLotacao").prop('checked', false);
    $("#txtSerieCTRB").val('');
    $("#txtNumeroCTRB").val('');
    $("#txtCIOT").val('');
    $("#hddInformacoesSeguro").val('');
    $("#hddInformacoesQuantidadeCarga").val('');
    $("#hddVeiculos").val('');
    $("#hddMotoristas").val('');
    $("#hddObservacoesContribuinte").val('');
    $("#hddObservacoesFisco").val('');
    $("#txtInformacaoAdicionalFisco").val('');
    $("#txtChaveNFeRemetente").val('');
    $("#txtDataEmissaoNFeRemetente").val('');
    $("#txtValorTotalNFeRemetente").val(Globalize.format(0, "n2"));
    $("#txtPlacaVeiculo").val('');
    $("#txtObservacaoGeral").val('');
    $("#txtObservacaoDigitacaoCTe").val('');
    $("#txtCaracteristicaAdicionalTransporte").val("");
    $("#txtCaracteristicaAdicionalServico").val("");
    $("#selTipoDocumentoRemetente").val($("#selTipoDocumentoRemetente option:first").val());
    $("#txtValorBaseCalculoINSS").val(Globalize.format(0, "n2"));
    $("#txtAliquotaINSS").val(Globalize.format(0, "n2"));
    $("#txtValorINSS").val(Globalize.format(0, "n2"));
    $("#txtValorBaseCalculo").val(Globalize.format(0, "n2"));
    $("#txtAliquotaIR").val(Globalize.format(0, "n2"));
    $("#txtValorIR").val(Globalize.format(0, "n2"));
    $("#txtValorBaseCalculoCSLL").val(Globalize.format(0, "n2"));
    $("#txtAliquotaCSLL").val(Globalize.format(0, "n2"));
    $("#txtValorCSLL").val(Globalize.format(0, "n2"));

    $("#txtIBSCBS_CST").val('');
    $("#txtIBSCBS_Class").val('');
    $("#txtIBSCBS_Class").empty();
    $("#txtIBSCBSBaseCalculo").val(Globalize.format(0, "n2"));

    $("#txtIBSEstadualAliquota").val(Globalize.format(0, "n2"));
    $("#txtIBSEstadualReducao").val(Globalize.format(0, "n2"));
    $("#txtIBSEstadualEfetiva").val(Globalize.format(0, "n2"));
    $("#txtIBSEstadualValor").val(Globalize.format(0, "n2"));

    $("#txtIBSMunAliquota").val(Globalize.format(0, "n2"));
    $("#txtIBSMunReducao").val(Globalize.format(0, "n2"));
    $("#txtIBSMunEfetiva").val(Globalize.format(0, "n2"));
    $("#txtIBSMunValor").val(Globalize.format(0, "n2"));

    $("#txtCBSAliquota").val(Globalize.format(0, "n2"));
    $("#txtCBSReducao").val(Globalize.format(0, "n2"));
    $("#txtCBSEfetiva").val(Globalize.format(0, "n2"));
    $("#txtCBSValor").val(Globalize.format(0, "n2"));

    $("#txtRNTRC").val($("#txtRNTRCEmpresa").val());
    $("#txtDescricaoComponentePrestacaoServico").val('');
    $("#selTipoFretamento").val('');
    $("#txtDataHoraViagem").val('');
    $("#hddCodigoCTEReferenciado").val("0");
    $("#selIndNegociavel").val($("#selIndNegociavel option:first").val());
    $("#selSubTomador").val($("#selSubTomador option:first").val());

    LimparCamposDestinatario();
    $("#hddDestinatario").val('');
    $("#txtCPFCNPJDestinatario").val('');

    LimparCamposLocalEntregaDiferenteDestinatario();
    $("#hddLocalEntregaDiferenteDestinatario").val('');
    $("#txtCPFCNPJ_LocalEntregaDiferenteDestinatario").val('');

    LimparCamposRecebedor();
    $("#hddRecebedor").val('');
    $("#txtCPFCNPJRecebedor").val('');

    LimparCamposExpedidor();
    $("#hddExpedidor").val('');
    $("#txtCPFCNPJExpedidor").val('');

    LimparCamposRemetente();
    $("#hddRemetente").val('');
    $("#txtCPFCNPJRemetente").val('');

    LimparCamposTomador();
    $("#hddTomador").val('');
    $("#txtCPFCNPJTomador").val('');

    SalvarFreteContratado();

    LimparCamposNFeRemetente();
    RenderizarNFesRemetente();

    LimparCamposNotasFiscaisRemetente();
    RenderizarNotasFiscaisRemetente();

    LimparCamposOutrosDocumentosRemetente();
    RenderizarOutrosDocumentosRemetente();

    RenderizarComponentesDaPrestacao();

    LimparCamposInformacaoQuantidadeCarga();
    RenderizarInformacaoQuantidadeCarga();

    RenderizarInformacaoSeguro();

    RenderizarVeiculos();

    RenderizarMotoristas();
    LimparCamposMotorista();

    RenderizarObservacoesFisco();
    LimparCamposObservacaoFisco();

    RenderizarObservacoesContribuinte();
    LimparCamposObservacaoContribuinte();

    $("#divPercentualICMSRecolhido").hide();
    ResetarCalculoPercentualICMSRecolhido();

    $("#txtDataEmissaoDeclaracaoCTeAnulado").val("");
    LimparCamposCTeSubstituicao();

    $("#hddDocsTranspAntPapel").val("");
    LimparCamposDocTranspAntPapel();
    RenderizarDocTranspAntPapel();

    $("#hddDocsTranspAntEletronico").val("");
    LimparCamposDocTranspAntEletronico();
    RenderizarDocTranspAntEletronico();

    $("#hddProdutosPerigosos").val("");
    LimparCamposProdutoPerigoso();
    RenderizarProdutoPerigoso();

    $("#hddDuplicatas").val("");
    LimparCamposFatura();
    RenderizarDuplicata();

    LimparCamposPercurso();
    $("body").data("percursos", null);
    RenderizarPercursos();

    $("#divInformacaoServicosEImpostos").addClass("hidden");
    $("#divInformacaoServicosEImpostosResumo").addClass("hidden");
    $("#btnEmitirCTe, #btnSalvarCTe").prop("disabled", false);
}

function AbrirDicaNovaJanela(somenteSeExistir, configuracao) {
    var configuracaoEmpresa = configuracao == null ? ($("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val())) : configuracao;

    if (configuracaoEmpresa.DicasEmissaoCTe != null && configuracaoEmpresa.DicasEmissaoCTe != "") {
        var posicao = (screen.height - 400) / 2;
        janelaDicas = window.open("", "", "width=900, height=500, resizable=yes, titlebar=yes, scrollbars=yes, top=" + posicao);

        var mensagemDicas = '<b style="font-size: 20px">Dicas para a Emissão do CT-e</b><br><br>' + configuracaoEmpresa.DicasEmissaoCTe.replace(/(?:\r\n|\r|\n)/g, '<br />');
        mensagemDicas = '<div style="font-size: 15px; margin-right: 5px; margin-bottom: 5px; margin-left: 10px;">' + mensagemDicas + '</div>';

        $("#divDicasCTe").modal("hide");
        janelaDicas.document.write(mensagemDicas);
    }
}