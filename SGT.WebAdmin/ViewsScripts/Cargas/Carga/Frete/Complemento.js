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
/// <reference path="../../../Creditos/ControleSaldo/ControleSaldo.js" />
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
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
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
/// <reference path="Componente.js" />
/// <reference path="EtapaFrete.js" />
/// <reference path="Frete.js" />
/// <reference path="SemTabela.js" />
/// <reference path="TabelaCliente.js" />
/// <reference path="TabelaComissao.js" />
/// <reference path="TabelaRota.js" />
/// <reference path="TabelaSubContratacao.js" />
/// <reference path="TabelaTerceiros.js" />
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
/// <reference path="../../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../../Consultas/ComponenteFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteCliente.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoFreteComissao.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoComplementoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var ComplementoFrete = function () {
    this.ValorComplemento = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.Carga.ValorComplementar.getRequiredFieldDescription(), required: true, getType: typesKnockout.decimal });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ComponenteDeFrete.getRequiredFieldDescription(), required: true, idBtnSearch: guid() });
    this.Motivo = PropertyEntity({ getType: typesKnockout.string, text: ko.observable(Localization.Resources.Gerais.Geral.Observacao.getFieldDescription()), maxlength: 250, required: false });
    this.MotivoAdicionalFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Motivo.getRequiredFieldDescription(), required: true, idBtnSearch: guid() });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.ConfirmarComplemento = PropertyEntity({ type: types.event, eventClick: confirmarSolicitacaoComplementoFreteClick, enable: ko.observable(true), text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.CreditosUtilizados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.CodigoCreditorSolicitar = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.DestinoComplemento = PropertyEntity({ type: types.select, val: ko.observable(0), options: EnumDestinoComplemento.ObterOpcoes(), text: Localization.Resources.Cargas.Carga.DestinoComplemento.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ eventClick: AbrirAnexoComplementoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true), icon: "fa fa-file-zip-o" });
}

var DetalhesComplementoFrete = function () {


    this.CargaEstaNaLogistica = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.SituacaoComplementoFrete = PropertyEntity({ val: ko.observable(EnumSituacaoComplementoFrete.Todas), def: EnumSituacaoComplementoFrete.Todas, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ComponenteDeFrete.getFieldDescription(), idBtnSearch: guid() });
    this.Motivo = PropertyEntity({ type: types.map, maxlength: 300, text: Localization.Resources.Cargas.Carga.ObservacaoDaSolicitacao.getFieldDescription() });
    this.MotivoAdicionalFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.MotivoDaSolicitacao.getFieldDescription() });
    this.ValorComplemento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorSolicitado.getFieldDescription(), getType: typesKnockout.decimal });
    this.ValorComplementoOriginal = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorSolicitadoInicialmente.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.DataAlteracao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDaSolicitacao.getFieldDescription(), getType: typesKnockout.dateTime });
    this.SolicitacaoCredito = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.dynamic });
    this.DescricaoSituacao = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Solicitante.getFieldDescription()
    });

    this.DataRetorno = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.DataDoRetorno.getFieldDescription(), getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.DestinoComplemento = PropertyEntity({ type: types.map, maxlength: 300, text: Localization.Resources.Cargas.Carga.DestinoComplemento.getFieldDescription(), visible: ko.observable(false) });

    this.Creditor = PropertyEntity({ type: types.local, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Creditor.getFieldDescription(), visible: ko.observable(false) });
    this.RetornoSolicitacao = PropertyEntity({ type: types.local, type: types.map, maxlength: 300, text: Localization.Resources.Cargas.Carga.Resposta.getFieldDescription(), visible: ko.observable(false) });
    this.Solicitado = PropertyEntity({ type: types.local, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Solicitado.getFieldDescription(), visible: ko.observable(false) });
    this.ValorLiberado = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ValorAprovado.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.ValorSolicitado = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.Carga.ValorSolicitado.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(false) });



    this.ConfirmarUtilizacao = PropertyEntity({ eventClick: confirmarUtilizacaoClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarUso, visible: ko.observable(true) });
    this.ExtornarUtilizacao = PropertyEntity({ eventClick: extornarUtilizacaoClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.Carga.ExtornarComplemento, visible: ko.observable(true) });
    this.Anexos = PropertyEntity({ eventClick: AbrirAnexoComplementoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true), icon: "fa fa-file-zip-o" });
}




//*******EVENTOS*******

function confirmarUtilizacaoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaConfirmarUtilizacaoDoComplementoDeFrete.format(e.ComponenteFrete.val()), function () {
        var data = { Codigo: e.Codigo.val() };
        executarReST("CargaComplementoFrete/ConfirmarUtilizacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    _cargaAtual.AgConfirmacaoUtilizacaoCredito.val(false);
                    preecherRetornoFrete(_cargaAtual, arg.Data, false);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ConfirmadoComSucesso);
                    limparCamposComplementoFrete(e);
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    })
}

function extornarUtilizacaoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExtornarUtilizacaoDesteComponenteDoComplementoDeFrete.format(e.ComponenteFrete.val()), function () {
        var data = { Codigo: e.Codigo.val() };
        executarReST("CargaComplementoFrete/ExtornarUtilizacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    _cargaAtual.AgConfirmacaoUtilizacaoCredito.val(false);
                    preecherRetornoFrete(_cargaAtual, arg.Data, false);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.EstornadoComSucesso);
                    limparCamposComplementoFrete(e);
                }
                else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    })
}

function limparCamposComplementoFrete(e) {
    LimparCampos(e);
    Global.fecharModal("divModalInformarComplementoFrete");
}

function confirmarSolicitacaoComplementoFreteClick(e, sender) {
    if (ValidarCamposObrigatorios(e)) {
        ValidarUtilizacaoSaldo(e.ValorComplemento.val(), function (creditosUtilizados, codigoCreditorSolicitar) {
            if (creditosUtilizados != null)
                e.CreditosUtilizados.val(JSON.stringify(creditosUtilizados));

            if (codigoCreditorSolicitar != null)
                e.CodigoCreditorSolicitar.val(codigoCreditorSolicitar);

            e.Carga.val(_cargaAtual.Codigo.val());

            Salvar(e, "CargaComplementoFrete/Adicionar", function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        enviarArquivosAnexadosComplemento(arg.Data.complementosDoFrete[arg.Data.complementosDoFrete.length - 1].Codigo);
                        preecherRetornoFrete(_cargaAtual, arg.Data, false);
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ConfirmadoComSucesso);
                        limparCamposComplementoFrete(e);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        }, true);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function adicionarComplementoFreteClick(e, sender) {

    var complementoFrete = new ComplementoFrete();
    complementoFrete.DestinoComplemento.visible(e.TipoOperacao.PermiteEscolherDestinacaoDoComplementoDeFrete);
    KoBindings(complementoFrete, "knoutInformarDadosComplementoFrete");
    new BuscarComponentesDeFrete(complementoFrete.ComponenteFrete);
    new BuscarMotivoAdicionalFrete(complementoFrete.MotivoAdicionalFrete);

    loadAnexoCargaComplementoFreteAnexo();
    Global.abrirModal("divModalInformarComplementoFrete");
}

function verDetalhesComplementoFreteClick(data) {

    executarReST("CargaComplementoFrete/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            var detalhesComplementoFrete = new DetalhesComplementoFrete();
            PreencherObjetoKnout(detalhesComplementoFrete, arg);

            if (detalhesComplementoFrete.SolicitacaoCredito.val() != null) {
                detalhesComplementoFrete.DataRetorno.val(detalhesComplementoFrete.SolicitacaoCredito.val().DataRetorno);
                if (detalhesComplementoFrete.DataRetorno.val() != "")
                    detalhesComplementoFrete.DataRetorno.visible(true);

                detalhesComplementoFrete.Creditor.val(detalhesComplementoFrete.SolicitacaoCredito.val().Creditor);
                if (detalhesComplementoFrete.Creditor.val() != "")
                    detalhesComplementoFrete.Creditor.visible(true);

                detalhesComplementoFrete.RetornoSolicitacao.val(detalhesComplementoFrete.SolicitacaoCredito.val().RetornoSolicitacao);
                if (detalhesComplementoFrete.RetornoSolicitacao.val() != "")
                    detalhesComplementoFrete.RetornoSolicitacao.visible(true);

                detalhesComplementoFrete.Solicitado.val(detalhesComplementoFrete.SolicitacaoCredito.val().Solicitado);
                if (detalhesComplementoFrete.Solicitado.val() != "")
                    detalhesComplementoFrete.Solicitado.visible(true);

                detalhesComplementoFrete.ValorSolicitado.val(detalhesComplementoFrete.SolicitacaoCredito.val().ValorSolicitado);
                detalhesComplementoFrete.ValorSolicitado.visible(true);

                detalhesComplementoFrete.ValorLiberado.val(detalhesComplementoFrete.SolicitacaoCredito.val().ValorLiberado);
                if (detalhesComplementoFrete.SituacaoComplementoFrete.val() != EnumSituacaoComplementoFrete.AgAprovacao)
                    detalhesComplementoFrete.ValorLiberado.visible(true);

                if (detalhesComplementoFrete.ValorComplemento.val() != detalhesComplementoFrete.ValorComplementoOriginal.val())
                    detalhesComplementoFrete.ValorComplementoOriginal.visible(true);
            }

            if (arg.Data.PermiteEscolherDestinacaoDoComplementoDeFrete)
                detalhesComplementoFrete.DestinoComplemento.visible(true);
            else
                detalhesComplementoFrete.DestinoComplemento.visible(false);

            if (detalhesComplementoFrete.SituacaoComplementoFrete.val() == EnumSituacaoComplementoFrete.AgConfirmacaoUso)
                detalhesComplementoFrete.ConfirmarUtilizacao.enable(true);

            if ((detalhesComplementoFrete.CargaEstaNaLogistica.val() && (detalhesComplementoFrete.SituacaoComplementoFrete.val() != EnumSituacaoComplementoFrete.Extornada && detalhesComplementoFrete.SituacaoComplementoFrete.val() != EnumSituacaoComplementoFrete.Rejeitada)) || detalhesComplementoFrete.SituacaoComplementoFrete.val() == EnumSituacaoComplementoFrete.AgAprovacao || detalhesComplementoFrete.SituacaoComplementoFrete.val() == EnumSituacaoComplementoFrete.AgConfirmacaoUso)
                detalhesComplementoFrete.ExtornarUtilizacao.enable(true);

            if (!_cargaAtual.CargaDesabilitada.enable()) {
                detalhesComplementoFrete.ExtornarUtilizacao.visible(false);
                detalhesComplementoFrete.ConfirmarUtilizacao.visible(false);
            }


            if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AdicionarComponentes, _PermissoesPersonalizadasCarga)) {
                detalhesComplementoFrete.ExtornarUtilizacao.enable(false);
                detalhesComplementoFrete.ConfirmarUtilizacao.enable(false);
            }

            loadAnexoCargaComplementoFreteAnexo();
            _anexoComplemento.Anexos.val(arg.Data.Anexos);
            _anexoComplemento.Anexos.visible(false);
            KoBindings(detalhesComplementoFrete, "knoutDetalhesComplementoFrete");
            Global.abrirModal("divModalDetalhesComplementoFrete");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

//*******MÉTODOS*******

function preencherComplementosFrete(e, complementosDoFrete) {

    if (complementosDoFrete != null) {
        e.AdicionarComplementoFrete.visibleFade(true);
        var verDetalhes = { descricao: Localization.Resources.Gerais.Geral.VerDetalhes, tamanho: 15, id: guid(), metodo: verDetalhesComplementoFreteClick, icone: "" };
        var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [verDetalhes] };
        var header = [{ data: "Codigo", visible: false },
        { data: "Data", title: Localization.Resources.Cargas.Carga.Data, width: "15%", className: "text-align-center", orderable: false },
        { data: "Operador", title: Localization.Resources.Cargas.Carga.Operador, width: "20%" },
        { data: "ComponenteFrete", title: Localization.Resources.Cargas.Carga.Componente, width: "30%" },
        { data: "ValorComplemento", title: Localization.Resources.Cargas.Carga.Valor, width: "15%", className: "text-align-right" },
        { data: "DescricaoSituacao", title: Localization.Resources.Cargas.Carga.Situacao, width: "20%", className: "text-align-center", orderable: false }
        ];
        var gridCTe = new BasicDataTable(e.AdicionarComplementoFrete.idGrid, header, menuOpcoes);
        gridCTe.CarregarGrid(complementosDoFrete);
    } else {
        e.AdicionarComplementoFrete.visibleFade(false);
    }
}