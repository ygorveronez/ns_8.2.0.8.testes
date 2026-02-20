/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="DadosTransporte.js" />
/// <reference path="Motorista.js" />
/// <reference path="Tipo.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaTransportadorSugerido = function () {
    this.Carga = PropertyEntity();
};

//*******EVENTOS*******

function buscarMDFeEmEncerramentoClick(e) {
    buscarDestalhesCarga(e);
}

function etapaTransporteClick(e) {
    _cargaAtual = e;
    var habilitarCampo = e.EtapaDadosTransportador.enable();

    if (_CONFIGURACAO_TMS.FiltrarBuscaVeiculosPorEmpresa)
        e.EtapaDadosTransportadorTranportadoraSugerida.visible(false);

    ocultarTodasAbas(e);
    buscarDestalhesCarga(e);

    e.SolicitarNFEsCarga.enable(habilitarCampo);
    if (permiteInformarTransportadorNaEtapaUM(e) && e.Empresa.codEntity() > 0) {
        e.Empresa.enable(false);
        e.Veiculo.enable(false);
        e.Reboque.enable(false);
    } else {
        e.Empresa.enable(habilitarCampo);
        e.Veiculo.enable(habilitarCampo);
        e.Reboque.enable(habilitarCampo);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
        e.DisponibilizarParaTransportador.visible(habilitarCampo && (e.RejeitadaPeloTransportador.val() || _CONFIGURACAO_TMS.PermitirDisponibilizarCargaParaTransportador));

        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_SalvarDadosTransporte, _PermissoesPersonalizadasCarga)) {
            e.SalvarDadosTransportador.enable(false);
            e.SolicitarNFEsCarga.enable(false);
        }
    }
    else {
        e.DisponibilizarParaTransportador.visible(false);
        e.Empresa.enable(false);
        e.SolicitarNFEsCarga.visible(e.PermitirTransportadorSolicitarNotasFiscais.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe);
    }

    if (e.Empresa.codEntity() > 0 && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAlterarTransportador, _PermissoesPersonalizadasCarga))
        e.Empresa.enable(false);

    if (e.DisponibilizarParaTransportador.visible())
        e.Empresa.enable(true);


    if (e.PossuiPendencia.val()) {
        $("#" + e.EtapaDadosTransportador.idGrid + " .DivPendeciaCarregamento").show();
        if (e.MotivoPendencia.val() != "") {
            var html = "<div class='alert alert-info alert-block'>";
            html += "<h4 class='alert-heading'>" + Localization.Resources.Cargas.Carga.Pendencia + "</h4>";
            html += e.MotivoPendencia.val();
            html += "</div>";

            $("#" + e.EtapaDadosTransportador.idGrid + " .DivMSGPendeciaCarregamento").html(html);
        }
    }
    else
        $("#" + e.EtapaDadosTransportador.idGrid + " .DivPendeciaCarregamento").hide();

    loadIntegracaoCargaTransportador(e);
    loadEtapasPreCheckin(e, true);
    loadDadosIntegracaoCargaPreChekinIntegracao(e, true);
    setarFocoPrimeiraAbaEtapaDadosTransportador(e);
}

function buscarDestalhesCarga(e) {
    var data = { Carga: e.Codigo.val() };
    executarReST("CargaTransportador/VerificarAcaoPermitida", data, function (arg) {
        if (arg.Success) {
            validarSolicitacaoNF(arg.Data, e);

            if (e.EtapaDadosTransportadorTranportadoraSugerida.visible() != false) {

                var pesquisaTransportadorSugerido = new PesquisaTransportadorSugerido();
                pesquisaTransportadorSugerido.Carga.val(e.Codigo.val());

                var editar = {
                    descricao: Localization.Resources.Gerais.Geral.Selecionar, id: "clasEditar", evento: "onclick", metodo: function (arg) {
                        selecionarTranportadorSugeridoClick(arg, e);
                    }, tamanho: "20", icone: ""
                };
                var menuOpcoes = new Object();
                menuOpcoes.tipo = TypeOptionMenu.link;
                menuOpcoes.opcoes = new Array();
                menuOpcoes.opcoes.push(editar);

                var gridTranportadores = new GridView(e.Empresa.idGrid, "CargaTransportador/BuscarTransportadoresSugeridosParaCarga", pesquisaTransportadorSugerido, menuOpcoes, { column: 1, dir: orderDir.asc });
                gridTranportadores.CarregarGrid();
            }

            setarFocoPrimeiraAbaEtapaDadosTransportador(e);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function disponibilizarParaTransportadorClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteDisponibilizarCargaParaTransportador, function () {
        if (!ValidarCampoObrigatorioEntity(e.Empresa))
            return;

        var data = {
            Codigo: e.Codigo.val(),
            Empresa: e.Empresa.codEntity()
        };

        executarReST("CargaTransportador/DisponibilizarParaTransportador", data, function (arg) {
            if (arg.Success) {
                if (arg.Data)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaDisponibilizadaParaTransportadorComSucesso);
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        });
    });
}

function selecionarTranportadorSugeridoClick(arg, e) {
    e.Empresa.val(arg.RazaoSocial);
    e.Empresa.codEntity(arg.Codigo);
}

function salvarDadosTransportadorClick(e) {
    const validoEmpresa = ValidarCampoObrigatorioEntity(e.Empresa);
    const validoVeiculo = _CONFIGURACAO_TMS.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador ? true : ValidarCampoObrigatorioEntity(e.Veiculo);
    const validoReboque = e.ExigeConfirmacaoTracao.val() && !_CONFIGURACAO_TMS.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador ? ValidarCampoObrigatorioEntity(e.Reboque) : true;
    const validoMotoristas = _CONFIGURACAO_TMS.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador ? true : e.AdicionarMotoristas.basicTable.BuscarRegistros().length > 0;
    const validoInicioCarregamento = ValidarCampoObrigatorioMap(e.InicioCarregamentoTransportador);
    const validoTerminoCarregamento = ValidarCampoObrigatorioMap(e.TerminoCarregamentoTransportador);
    const validoDataRetiradaCtrn = ValidarCampoObrigatorioMap(e.DataRetiradaCtrnVeiculo) && ValidarCampoObrigatorioMap(e.DataRetiradaCtrnReboque) && ValidarCampoObrigatorioMap(e.DataRetiradaCtrnSegundoReboque);
    const validoNumeroContainer = ValidarCampoObrigatorioMap(e.NumeroContainerVeiculo) && ValidarCampoObrigatorioMap(e.NumeroContainerReboque) && ValidarCampoObrigatorioMap(e.NumeroContainerSegundoReboque);
    const validoBalsa = ValidarCampoObrigatorioEntity(e.Balsa);
    const validoNavio = ValidarCampoObrigatorioEntity(e.Navio);
    const validoTransportadorSubcontratado = ValidarCampoObrigatorioEntity(e.TransportadorSubcontratado);

    if (validoEmpresa && validoVeiculo && validoReboque && validoMotoristas && validoInicioCarregamento && validoTerminoCarregamento && validoDataRetiradaCtrn && validoNumeroContainer && validoBalsa && validoNavio && validoTransportadorSubcontratado) {
        let data = {
            Codigo: e.Codigo.val(),
            Empresa: e.Empresa.codEntity(),
            Motoristas: JSON.stringify(e.AdicionarMotoristas.basicTable.BuscarRegistros()),
            Veiculo: e.Veiculo.codEntity(),
            Reboque: e.Reboque.codEntity(),
            SegundoReboque: e.SegundoReboque.codEntity(),
            DataRetiradaCtrnVeiculo: e.DataRetiradaCtrnVeiculo.val(),
            DataRetiradaCtrnReboque: e.DataRetiradaCtrnReboque.val(),
            DataRetiradaCtrnSegundoReboque: e.DataRetiradaCtrnSegundoReboque.val(),
            GensetVeiculo: e.GensetVeiculo.val(),
            GensetReboque: e.GensetReboque.val(),
            GensetSegundoReboque: e.GensetSegundoReboque.val(),
            MaxGrossVeiculo: Globalize.parseInt(e.MaxGrossVeiculo.val()),
            MaxGrossReboque: Globalize.parseInt(e.MaxGrossReboque.val()),
            MaxGrossSegundoReboque: Globalize.parseInt(e.MaxGrossSegundoReboque.val()),
            NumeroContainerVeiculo: e.NumeroContainerVeiculo.val(),
            NumeroContainerReboque: e.NumeroContainerReboque.val(),
            NumeroContainerSegundoReboque: e.NumeroContainerSegundoReboque.val(),
            TaraContainerVeiculo: Globalize.parseInt(e.TaraContainerVeiculo.val()),
            TaraContainerReboque: Globalize.parseInt(e.TaraContainerReboque.val()),
            TaraContainerSegundoReboque: Globalize.parseInt(e.TaraContainerSegundoReboque.val()),
            ApoliceSeguro: JSON.stringify(recursiveMultiplesEntities(e.ApoliceSeguro)),
            InicioCarregamento: e.InicioCarregamento.val(),
            TerminoCarregamento: e.TerminoCarregamento.val(),
            Navio: e.Navio.codEntity(),
            Balsa: e.Balsa.codEntity(),
            Container: e.Container.codEntity()
        };

        if (e.ExigeTermoAceiteTransportador.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            var dataTermoAceite = {
                Callback: function () { salvarDadosTransportador(e, data); },
                TermoAceite: e.TermoAceite.val(),
                CodigoCarga: e.Codigo.val()
            };

            exibirModalAceiteTermoTransporte(dataTermoAceite);
        }
        else
            salvarDadosTransportador(e, data);
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorInformeOsCamposObrigatorios);
}

//*******MÉTODOS*******

function validarSolicitacaoNF(situacaoLiberacao, e) {
    if (situacaoLiberacao.EncerrandoMDFe) {
        $("#" + e.EtapaDadosTransportador.idGrid + " .DivMDFeGridEmEncerramento").show();

        var header = [
            { data: "CodigoMDFE", visible: false },
            { data: "CodigoEmpresa", visible: false },
            { data: "PossuiInformacoesIMO", visible: false },
            { data: "Numero", title: Localization.Resources.Cargas.Carga.Numero, width: "10%", className: "text-align-center", orderable: false },
            { data: "Serie", title: Localization.Resources.Cargas.Carga.Serie, width: "8%", className: "text-align-center", orderable: false },
            { data: "Emissao", title: Localization.Resources.Cargas.Carga.DataEmissao, width: "10%", className: "text-align-center", orderable: false },
            { data: "UFCarga", title: Localization.Resources.Cargas.Carga.UFCarregamento, width: "20%" },
            { data: "UFDesgarga", title: Localization.Resources.Cargas.Carga.UFDescarregamento, width: "20%" },
            { data: "Status", title: Localization.Resources.Gerais.Geral.Status, width: "10%", className: "text-align-center", orderable: false },
            { data: "RetornoSefaz", title: Localization.Resources.Cargas.Carga.RetornoSefaz, width: "15%", orderable: false }
        ];

        var gridMDFe = new BasicDataTable(e.EtapaDadosTransportador.idGrid + " .MDFeEmEncerramento table", header, null);

        gridMDFe.CarregarGrid(situacaoLiberacao.MDFe);
    }
    else
        $("#" + e.EtapaDadosTransportador.idGrid + " .DivMDFeGridEmEncerramento").hide();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe || (e.PermitirTransportadorSolicitarNotasFiscais.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)) {
        e.SolicitarNFEsCarga.visible(situacaoLiberacao.LiberarSolicitacaoNF);

        if (situacaoLiberacao.Mensagem != "") {
            $("#" + e.SolicitarNFEsCarga.idTab).text(situacaoLiberacao.Mensagem);
            $("#" + e.SolicitarNFEsCarga.idTab).show();
        }
        else
            $("#" + e.SolicitarNFEsCarga.idTab).hide();
    }
}

function EtapaDadosTransportadorDesabilitada(e) {
    $("#" + e.EtapaDadosTransportador.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaDadosTransportador.idTab + " .step").attr("class", "step");
    e.EtapaDadosTransportador.eventClick = function (e) { };
}

function EtapaDadosTransportadorLiberada(e) {
    if (!e.EtapaDadosTransportador)
        return;

    $("#" + e.EtapaDadosTransportador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDadosTransportador.idTab + " .step").attr("class", "step yellow");

    e.EtapaDadosTransportador.eventClick = etapaTransporteClick;
    EtapaFreteEmbarcadorAprovada(e);
}

function EtapaDadosTransportadorAprovada(e) {
    $("#" + e.EtapaDadosTransportador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDadosTransportador.idTab + " .step").attr("class", "step green");

    e.EtapaDadosTransportador.eventClick = etapaTransporteClick;
    EtapaFreteEmbarcadorAprovada(e);
}

function EtapaDadosTransportadorProblema(e) {
    $("#" + e.EtapaDadosTransportador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaDadosTransportador.idTab + " .step").attr("class", "step red");
    e.EtapaDadosTransportador.eventClick = etapaTransporteClick;
    EtapaFreteEmbarcadorAprovada(e);
}

function EtapaDadosTransportadorEdicaoDesabilitada(e) {
    e.EtapaDadosTransportador.enable(false);
    e.EtapaDadosTransportadorTranportadoraSugerida.visible(false);

    e.AdicionarMotoristas.basicTable?.DesabilitarOpcoes();
    e.EtapaInicioTMS.enable(false);

    e.Veiculo.enable(false);
    e.ApoliceSeguro.enable(false);
    e.InicioCarregamento.enable(false);
    e.TerminoCarregamento.enable(false);
    e.Reboque.enable(false);
    e.SegundoReboque.enable(false);
    e.DataRetiradaCtrnVeiculo.enable(false);
    e.DataRetiradaCtrnReboque.enable(false);
    e.DataRetiradaCtrnSegundoReboque.enable(false);
    e.GensetVeiculo.enable(false);
    e.GensetReboque.enable(false);
    e.GensetSegundoReboque.enable(false);
    e.MaxGrossVeiculo.enable(false);
    e.MaxGrossReboque.enable(false);
    e.MaxGrossSegundoReboque.enable(false);
    e.NumeroContainerVeiculo.enable(false);
    e.NumeroContainerReboque.enable(false);
    e.NumeroContainerSegundoReboque.enable(false);
    e.TaraContainerVeiculo.enable(false);
    e.TaraContainerReboque.enable(false);
    e.TaraContainerSegundoReboque.enable(false);
    e.SalvarDadosTransportador.enable(false);
    e.Navio.enable(false);
    e.Balsa.enable(false);

    EtapaFreteEmbarcadorEdicaoDesabilitada(e);
}

function setarFocoPrimeiraAbaEtapaDadosTransportador(e) {
    $("#transportador_" + e.EtapaDadosTransportador.idGrid + "_ul li").removeClass("active");
    $("#" + e.EtapaDadosTransportador.idGrid + " .tab-pane-etapa-dados_transportador").removeClass("active in");
    $("#tabTransportador_" + e.EtapaDadosTransportador.idGrid + "_li").addClass("active");
    $("#tabTransportador_" + e.EtapaDadosTransportador.idGrid).addClass("active in");

    Global.ResetarAba(e.EtapaDadosTransportador.idGrid);
}

function salvarDadosTransportador(e, data) {
    if (_cargaAtual.TipoOperacao.AlertarTransportadorNaoIMOCargasPerigosas && !e.Empresa.PossuiInformacoesIMO) {
        exibirConfirmacao("Confirmação", "Você realmente deseja salvar o transportador sem possuir as informações de IMO cadastrada?", function () {
            executarReST("CargaTransportador/SalvarDadosTransportador", data, function (retorno) {
                if (retorno.Success) {
                    $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();

                    if (retorno.Data != false) {
                        EtapaDadosTransportadorAprovada(e);

                        var infoMotoristas = "";

                        $.each(e.AdicionarMotoristas.basicTable.BuscarRegistros(), function (i, motorista) {
                            if (i > 0) {
                                infoMotoristas += ", ";
                            }
                            infoMotoristas += motorista.Nome;
                        });

                        e.Motorista.motoristas(infoMotoristas);
                        e.Veiculo.placas(retorno.Data.DadosFrete.Placas);
                        e.SituacaoCarga.val(EnumSituacoesCarga.AgTransportador);
                        PreecherInformacaoValorFrete(e, retorno.Data.DadosFrete.ValorFrete);
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                        validarSolicitacaoNF(retorno.Data.DadosFrete, e);
                        setarFocoPrimeiraAbaEtapaDadosTransportador(e);

                        if (isNaN(e.CodigoContainerReboque.val()) && (retorno.Data.DadosContainer.CodigoContainerReboque > 0))
                            e.ContainerReboqueAnexo.enviarArquivosAnexados(retorno.Data.DadosContainer.CodigoContainerReboque);

                        if (isNaN(e.CodigoContainerSegundoReboque.val()) && (retorno.Data.DadosContainer.CodigoContainerSegundoReboque > 0))
                            e.ContainerSegundoReboqueAnexo.enviarArquivosAnexados(retorno.Data.DadosContainer.CodigoContainerSegundoReboque);

                        if (isNaN(e.CodigoContainerVeiculo.val()) && (retorno.Data.DadosContainer.CodigoContainerVeiculo > 0))
                            e.ContainerVeiculoAnexo.enviarArquivosAnexados(retorno.Data.DadosContainer.CodigoContainerVeiculo);

                        if (retorno.Data.DadosFrete.LiberarSolicitacaoNF && (_CONFIGURACAO_TMS.SolicitarNotasFiscaisAoSalvarDadosTransportador || e.TipoOperacao.solicitarNotasFiscaisAoSalvarDadosTransportador))
                            solicitarNotasFiscais(e);
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 60000);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        })
        return;
    } else {
        executarReST("CargaTransportador/SalvarDadosTransportador", data, function (retorno) {
            if (retorno.Success) {
                $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();

                if (retorno.Data != false) {
                    EtapaDadosTransportadorAprovada(e);

                    var infoMotoristas = "";

                    $.each(e.AdicionarMotoristas.basicTable.BuscarRegistros(), function (i, motorista) {
                        if (i > 0) {
                            infoMotoristas += ", ";
                        }
                        infoMotoristas += motorista.Nome;
                    });

                    e.Motorista.motoristas(infoMotoristas);
                    e.Veiculo.placas(retorno.Data.DadosFrete.Placas);
                    e.SituacaoCarga.val(EnumSituacoesCarga.AgTransportador);

                    PreecherInformacaoValorFrete(e, retorno.Data.DadosFrete.ValorFrete);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                    validarSolicitacaoNF(retorno.Data.DadosFrete, e);
                    setarFocoPrimeiraAbaEtapaDadosTransportador(e);

                    if (isNaN(e.CodigoContainerReboque.val()) && (retorno.Data.DadosContainer.CodigoContainerReboque > 0))
                        e.ContainerReboqueAnexo.enviarArquivosAnexados(retorno.Data.DadosContainer.CodigoContainerReboque);

                    if (isNaN(e.CodigoContainerSegundoReboque.val()) && (retorno.Data.DadosContainer.CodigoContainerSegundoReboque > 0))
                        e.ContainerSegundoReboqueAnexo.enviarArquivosAnexados(retorno.Data.DadosContainer.CodigoContainerSegundoReboque);

                    if (isNaN(e.CodigoContainerVeiculo.val()) && (retorno.Data.DadosContainer.CodigoContainerVeiculo > 0))
                        e.ContainerVeiculoAnexo.enviarArquivosAnexados(retorno.Data.DadosContainer.CodigoContainerVeiculo);

                    if (retorno.Data.DadosFrete.LiberarSolicitacaoNF && (_CONFIGURACAO_TMS.SolicitarNotasFiscaisAoSalvarDadosTransportador || e.TipoOperacao.solicitarNotasFiscaisAoSalvarDadosTransportador))
                        solicitarNotasFiscais(e);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 60000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}

function permiteInformarTransportadorNaEtapaUM(e) {
    return e.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete != null && e.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete.val();
}