/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
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
/// <reference path="Transportador.js" />
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
/// <reference path="../../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var SolicitarJustificativa = function () {
    this.Data = PropertyEntity({ val: ko.observable(""), def: 0 });
    this.KnoutTipoCarga = PropertyEntity({ val: ko.observable(""), def: 0 });
    this.Justificativa = PropertyEntity({ type: types.map, required: true, maxlength: 300, text: Localization.Resources.Cargas.Carga.MotivoDaAlteracao.getRequiredFieldDescription() });
    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarJustificativaClick, text: Localization.Resources.Cargas.Carga.ConfirmarAlteracao, visible: ko.observable(true) });
}

//*******EVENTOS*******

function etapaInicioEmbarcadorClick(e) {

    ocultarTodasAbas(e);
    _cargaAtual = e;
    var habilitarCampo = e.EtapaInicioEmbarcador.enable()

    e.SalvarDadosCarga.enable(habilitarCampo);
    e.ModeloVeicularCarga.enable(habilitarCampo);
    e.TipoCarga.enable(habilitarCampo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        e.ModeloVeicularCarga.enable(false);
        e.TipoCarga.enable(false);
        e.SalvarDadosCarga.visible(false);
    }
    else if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_SalvarDadosTransporte, _PermissoesPersonalizadasCarga))
        e.SalvarDadosCarga.enable(false);

    if (Boolean(e.OrdemEmbarque.val()) && e.CargaDePreCarga.val())
        e.ModeloVeicularCarga.enable(false);

    LoadCargaDadosTransporteIntegracao(_cargaAtual, true);

    if (e.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete.val()) {
        e.Veiculo.enable(true);
        e.Reboque.enable(true);
    }
}

function solicitarJustificativaModal(e, data) {
    var justificar = new SolicitarJustificativa();
    justificar.Data.val(data);
    justificar.KnoutTipoCarga.val(e);
    KoBindings(justificar, "knoutModificarTipoCargaModeloVeicularLog");
    Global.abrirModal('divModalModificarTipoCargaModeloVeicularLog');
}

function confirmarJustificativaClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        if (e.Justificativa.val().length >= 20) {
            var data = e.Data.val();
            data.Justificativa = e.Justificativa.val();
            Global.fecharModal('divModalModificarTipoCargaModeloVeicularLog');
            enviarTipoCarga(e.KnoutTipoCarga.val(), data);
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.JustificativaDeveConterNoMinimoVinteCaracteres);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.InformeJustificativa);
    }

}

function salvarDadosCargaClick(e) {
    var data = {
        Codigo: e.Codigo.val(),
        TipoCarga: e.TipoCarga.codEntity(),
        ModeloVeicularCarga: e.ModeloVeicularCarga.codEntity(),
        Justificativa: "",
        CodigoTransportador: e.Empresa.codEntity(),
        CodigoVeiculo: e.Veiculo.codEntity(),
        CodigoReboque: e.Reboque.codEntity(),
        CodigoSegundoReboque: e.SegundoReboque.codEntity(),
        CodigoTerceiroReboque: e.TerceiroReboque.codEntity(),
        Motoristas: JSON.stringify(e.AdicionarMotoristas.basicTable.BuscarRegistros()),
    };

    var tudoCerto = true;
    if (e.TipoCarga.val() == "" || e.TipoCarga.codEntity() == 0) {
        e.TipoCarga.requiredClass("form-control is-invalid");
        e.TipoCarga.codEntity(0);
        e.TipoCarga.val("");
        tudoCerto = false;
    } else {
        e.TipoCarga.requiredClass("form-control");
    }
    if (e.ModeloVeicularCarga.required && (e.ModeloVeicularCarga.val() == "" || e.ModeloVeicularCarga.codEntity() == 0)) {
        e.ModeloVeicularCarga.requiredClass("form-control is-invalid");
        e.ModeloVeicularCarga.codEntity(0);
        e.ModeloVeicularCarga.val("");
        tudoCerto = false;
    } else {
        e.ModeloVeicularCarga.requiredClass("form-control");
    }

    var solicitarJustificativa = false;
    if (!e.PossuiPendencia.val() || e.SituacaoCarga.val() != EnumSituacoesCarga.Nova) {
        if (e.AutoTipoCarga.val() && e.TipoCarga.codEntity() != e.CodTipoCargaOriginal.val) {
            solicitarJustificativa = true;
        }

        if (e.AutoModeloVeicular.val() && e.ModeloVeicularCarga.codEntity() != e.CodModeloVeicularCargaOriginal.val) {
            solicitarJustificativa = true;
        }
    }

    if (tudoCerto || (_cargaAtual.CargaTipoConsolidacao.val() && e.TipoCarga.codEntity() != 0)) {
        if (solicitarJustificativa) {
            solicitarJustificativaModal(e, data);
        } else {
            enviarTipoCarga(e, data);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorInformeOsCamposObrigatorios);
    }
}

function enviarTipoCarga(e, data) {
    executarReST("CargaTipo/SalvarDadosCarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                var retorno = arg.Data.Frete;
                var carga = arg.Data.Carga;
                IniciarBindKnoutCarga(_cargaAtual, carga);
                InformarEstadosDasEtapas(_cargaAtual, carga, _cargaAtual.DivCarga.id);
                if (retorno.situacao == EnumSituacaoRetornoDadosFrete.CalculandoFrete)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Pendencias, Localization.Resources.Cargas.Carga.SalvoComSucessoPoremExistemPendenciasParaCalcularFrete);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function EtapaInicioEmbarcadorProblema(e) {
    $("#" + e.EtapaInicioEmbarcador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioEmbarcador.idTab + " .step").attr("class", "step red");
}

function EtapaInicioEmbarcadorLiberada(e) {
    $("#" + e.EtapaInicioEmbarcador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioEmbarcador.idTab + " .step").attr("class", "step yellow");
}

function EtapaInicioEmbarcadorAprovada(e) {
    $("#" + e.EtapaInicioEmbarcador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioEmbarcador.idTab + " .step").attr("class", "step green");
}

function EtapaInicioEmbarcadorAguardando(e) {
    $("#" + e.EtapaInicioEmbarcador.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaInicioEmbarcador.idTab + " .step").attr("class", "step yellow");
}

function EtapaInicioEmbarcadorEdicaoDesabilitada(e) {
    e.EtapaInicioEmbarcador.enable(false);
}