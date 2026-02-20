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
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="CTe.js" />
/// <reference path="NFS.js" />
/// <reference path="PreCTe.js" />
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
/// <reference path="../../../Enumeradores/EnumSituacaoMDFe.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var CargaMDFes = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable(EnumSituacaoMDFe.Todos), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoMdfe.getFieldDescription(), options: EnumSituacaoMDFe.obterOpcoesPesquisa(), def: EnumSituacaoMDFe.Todos });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaMDFe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.LiberarCargaComMDFesRejeitados = PropertyEntity({
        eventClick: LiberarCargaComMDFesRejeitadosClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.LiberarCargaMesmoComOsMdfesRejeitados), visible: ko.observable(false), enable: ko.observable(false)
    });
    this.RelacaoEntrega = PropertyEntity({
        eventClick: RelacaoEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.RelacaoDeEntrega), visible: ko.observable(!_CONFIGURACAO_TMS.RelatorioEntregaPorPedido), enable: ko.observable(true), icon: "fal fa-eye"
    });
    this.RelacaoEntregaNatura = PropertyEntity({
        eventClick: ImprimirRelacaoEntregaNaturaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.RelacaoEntregaNaturaComprovante), visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-paper-plane"
    });
    this.EmitirNovamenteRejeitados = PropertyEntity({
        eventClick: ReemitirMDFesRejeitadosClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReemitirOsMdfesRejeitados, visible: ko.observable(false), enable: ko.observable(false)
    });
    this.TentarNovamente = PropertyEntity({
        eventClick: tentarEmitirNovamenteClick, type: types.event, text: Localization.Resources.Cargas.Carga.TentarEmitirNovamente, idGrid: guid(), visible: ko.observable(false)
    });
    this.DownloadLoteXMLMDFe = PropertyEntity({
        eventClick: DownloadLoteXMLMDFeClick, type: types.event, text: Localization.Resources.Cargas.Carga.XmlDosMdfes, idGrid: guid(), visible: ko.observable(false)
    });
    this.DownloadLoteDAMDFE = PropertyEntity({
        eventClick: DownloadLoteDAMDFEClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfDosMdfes, idGrid: guid(), visible: ko.observable(false)
    });

    this.DownloadLoteDocumentos = PropertyEntity({
        eventClick: DownloadLoteDocumentosMDFeClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.DownloadDosDocumentos), visible: ko.observable(false), enable: ko.observable(false)
    });

    this.MDFe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), visible: ko.observable(false) });

    this.ImpressoesDaCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ImpressoesDaCarga, val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true) });

    //AVERBAÇÃO
    this.Apolice = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Apolice.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroMDFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroMdfe.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.SituacaoAverbacao = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), options: EnumStatusAverbacaoMDFe.obterOpcoesPesquisa(), def: "" });
    this.PesquisarMDFeAverbacoes = PropertyEntity({
        eventClick: function (e) {
            _gridCargaMDFeAverbacao.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.LiberarCargaComAverbacoesRejeitados = PropertyEntity({
        eventClick: LiberarCargaComAverbacoesMDFeRejeitadosClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.LiberarCargaMesmoComAsAverbacoesRejeitadas), visible: ko.observable(false), enable: ko.observable(false)
    });
    this.EmitirNovamenteAverbacoesMDFe = PropertyEntity({
        eventClick: ReaverbarRejeitadosMDFeClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReaverbarRejeitados, idGrid: guid(), visible: ko.observable(false)
    });

    this.SincronizarTodosDocumentoMDFe = PropertyEntity({
        eventClick: sincronizarTodosDocumentoMDFeClick, type: types.event, text: Localization.Resources.Cargas.Carga.SincronizarTodosDocumento, idGrid: guid(), visible: ko.observable(false)
    });
};

var InformacaoMDFe = function () {
    this.CargaMDFe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Chave = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ChaveMdfe.getFieldDescription(), maxlength: 44 });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), maxlength: 250 });
    this.InformarDados = PropertyEntity({ type: types.event, eventClick: informarDadosMDFeClick, enable: ko.observable(true), text: Localization.Resources.Cargas.Carga.EnviarDadosMdfe, visible: ko.observable(true) });
};

var InformarDataPrevisaoEncerramentoMDFe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataPrevisaoEncerramento = PropertyEntity({ getType: typesKnockout.dateTime, text: Localization.Resources.Cargas.Carga.DataPrevisaoDeEncerramento.getRequiredFieldDescription(), required: true });
    this.InformarDataPrevisaoEncerramento = PropertyEntity({ type: types.event, eventClick: InformarDataPrevisaoEncerramentoMDFeClick, enable: ko.observable(true), text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
};

var _HTMLMDFE;
var _gridCargaMDFe;
var _gridCargaMDFeAverbacao;
var _cargaMDFe;
var _idTabEtapaMDFe;
var _informacaoMDFe;
var _informarDataPrevisaoEncerramentoMDFe;

//*******EVENTOS*******

function buscarMDFeClick(e, sender) {

    _cargaAtual = e;
    ocultarTodasAbas(e);

    if (!PermitirAcessarEtapaMDFeCarga())
        return;

    var strKnoutCargaMDFe = "knoutCargaMDFe" + e.EtapaMDFe.idGrid;
    $("#" + e.EtapaMDFe.idGrid).html(_HTMLMDFE.replace("#knoutCargaMDFe", strKnoutCargaMDFe).replace(/#knoutCargaMDFe/g, e.EtapaMDFe.idGrid));

    _cargaMDFe = new CargaMDFes();
    _cargaMDFe.Carga.val(_cargaAtual.Codigo.val());
    KoBindings(_cargaMDFe, strKnoutCargaMDFe);

    LocalizeCurrentPage();

    $("#" + _cargaMDFe.Status.id).removeAttr("disabled");
    $("#" + _cargaMDFe.Pesquisar.id).removeAttr("disabled");

    if (e.PossuiAverbacaoMDFe.val())
        $("#tabAverbacaoMDFes_" + e.EtapaMDFe.idGrid + "_li").show();
    else
        $("#tabAverbacaoMDFes_" + e.EtapaMDFe.idGrid + "_li").hide();

    ExibirMensagemCIOTMdfe(e);

    buscarCargaMDFe(function (mdfeGrid) {

        _cargaMDFe.SincronizarTodosDocumentoMDFe.visible(mdfeGrid.data?.some(item => item.HabilitarSincronizarDocumento === true));

        $("#" + _cargaAtual.EtapaMDFe.idGrid + " .DivMDFeGrid").show();

        if (mdfeGrid != null && mdfeGrid.data.length > 0) {
            _cargaMDFe.MDFe.visible(true);
            _cargaMDFe.Status.visible(true);
            _cargaMDFe.Pesquisar.visible(true);
        } else {
            _cargaMDFe.MDFe.visible(false);
            _cargaMDFe.Status.visible(false);
            _cargaMDFe.Pesquisar.visible(false);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            _cargaMDFe.DownloadLoteDocumentos.visible(true);
            _cargaMDFe.DownloadLoteDocumentos.enable(true);
        }

        if (_cargaAtual.ProblemaMDFe.val()) {
            preencherMotivoPendenciaMDFe(_cargaAtual);
            if (mdfeGrid == null || mdfeGrid.data.length <= 0 && _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.PendeciaDocumentos) {
                _cargaMDFe.TentarNovamente.visible(true);

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_FinalizarCargaMDFeRejeitado, _PermissoesPersonalizadasCarga)) {
                    _cargaMDFe.LiberarCargaComMDFesRejeitados.text(Localization.Resources.Cargas.Carga.LiberarCargaMesmoSemMdfesEmitidos);
                    _cargaMDFe.LiberarCargaComMDFesRejeitados.visible(true);
                }
            }
            else {
                _cargaMDFe.TentarNovamente.visible(false);
                _cargaMDFe.EmitirNovamenteRejeitados.visible(true);
                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_FinalizarCargaMDFeRejeitado, _PermissoesPersonalizadasCarga)) {
                    _cargaMDFe.LiberarCargaComMDFesRejeitados.text(Localization.Resources.Cargas.Carga.LiberarCargaMesmoComOsMdfesRejeitados);
                    _cargaMDFe.LiberarCargaComMDFesRejeitados.visible(true);
                }
            }
        } else if (_cargaAtual.ProblemaAverbacaoMDFe.val()) {
            PreencherMotivoPendenciaAverbacaoMDFe(_cargaAtual);
            _cargaMDFe.EmitirNovamenteAverbacoesMDFe.visible(true);
        } else {
            if (mdfeGrid == null || mdfeGrid.data.length <= 0) {
                $("#" + _cargaAtual.EtapaMDFe.idGrid + " .MensageMDF").html(Localization.Resources.Cargas.Carga.ConformeEstaConfiguradoParaEssaCargaNaoDevemSerEmitidosMdfesAutomaticamente);
                $("#" + _cargaAtual.EtapaMDFe.idGrid + " .DivMensagemMDF").show();
            } else {
                _cargaMDFe.DownloadLoteXMLMDFe.visible(true);
                _cargaMDFe.DownloadLoteDAMDFE.visible(true);
            }
        }
    });
    BuscarCargaMDFeAverbacao();
}

function ExibirMensagemCIOTMdfe(knoutCarga) {
    var rows = [];

    if (knoutCarga.FreteDeTerceiro.val() && knoutCarga.ObrigatoriedadeCIOTEmissaoMDFe.val() && knoutCarga.ProprietarioTAC.val() && !knoutCarga.PossuiCIOT.val())
        rows.push(
            '<div class="alert alert-info mb-2 fade show" role="alert">' +
            '    <div class="d-flex align-items-center">' +
            '        <div class="alert-icon">' +
            '            <i class="fal fa-info-circle"></i>' +
            '        </div>' +
            '        <div class="flex-1">' +
            '            <span class="h5">Aviso sobre CIOT</span>' +
            '            <br>' +
            '            O MDF-e foi emitido, mas sem CIOT. Hoje não há bloqueio, porém a ANTT poderá exigir em breve.' +
            '       </div>' +
            '    </div>' +
            '</div>'
        );

    if (knoutCarga.FreteDeTerceiro.val() && knoutCarga.VeiculoPropriedadeTerceiro.val() && (knoutCarga.CIOTGeradoAutomaticamente.val() || knoutCarga.PossuiCIOT.val()))
        rows.push(
            '<div class="alert alert-info mb-2 fade show" role="alert">' +
            '    <div class="d-flex align-items-center">' +
            '        <div class="alert-icon">' +
            '            <i class="fal fa-info-circle"></i>' +
            '        </div>' +
            '        <div class="flex-1">' +
            '            <span class="h5">CIOT Emitido</span>' +
            '            <br>' +
            '            O transportador já informou o CIOT e as condições de pagamento.' +
            '       </div>' +
            '    </div>' +
            '</div>'
        );

    if (rows.length > 0)
        $("#" + knoutCarga.EtapaMDFe.idGrid + " ." + "DivMensagemMDF").html(rows.join("")).show();
}

function PermitirAcessarEtapaMDFeCarga() {

    var situacaoCargaAtiva = false;
    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgImpressaoDocumentos || 
        _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.EmTransporte || 
        _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Encerrada || 
        _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Cancelada || 
        _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Anulada)
        situacaoCargaAtiva = true

    return (!_CONFIGURACAO_TMS.NaoPermitirAcessarDocumentosAntesCargaEmTransporte && !_cargaAtual.NaoPermitirAcessarDocumentosAntesCargaEmTransporte.val()) || situacaoCargaAtiva || _cargaAtual.ProblemaMDFe.val() === true || _cargaAtual.ProblemaAverbacaoMDFe.val() === true;
}

//const PermitirAcessarEtapaMDFeCarga = () => {
//    let situacoesPermitidas = [EnumSituacoesCarga.AgImpressaoDocumentos, EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.Anulada];

//    return (!_CONFIGURACAO_TMS.NaoPermitirAcessarDocumentosAntesCargaEmTransporte && !_cargaAtual.NaoPermitirAcessarDocumentosAntesCargaEmTransporte.val()) || situacoesPermitidas.includes(_cargaAtual.SituacaoCarga.val()) || _cargaAtual.ProblemaMDFe.val() === true || _cargaAtual.ProblemaAverbacaoMDFe.val() === true;
//}

function tentarEmitirNovamenteClick(e) {
    var data = { Carga: _cargaAtual.Codigo.val() };
    executarReST("CargaMDFe/TentarEmitirNovamente", data, function (arg) {
        if (arg.Success) {
            var retorno = arg.Data;
            if (retorno.MDFeEmitidos) {
                exibirReenvioMDFe();
                _cargaMDFe.MDFe.visible(true);
            } else {
                _cargaAtual.MotivoPendencia.val(retorno.Mensagem)
                preencherMotivoPendenciaMDFe(_cargaAtual);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function RelacaoEntregaClick(e) {
    var data = { Carga: _cargaAtual.Codigo.val() };
    executarDownload("CargaMDFe/ImprimirRelacaoEntrega", data);
}

function RelacaoEmbarqueClick(e) {
    var data = { Carga: _cargaAtual.Codigo.val() };
    executarDownload("CargaMDFe/ImprimirRelacaoEmbarque", data);
}

function ImprimirRelacaoEntregaNaturaClick(e) {
    if (_cargaAtual.Pedidos.val.length > 0) {
        var data = { CargaPesquisada: _cargaAtual.Codigo.val(), CargaPedido: _cargaAtual.Pedidos.val[0].CodigoCargaPedido };
        executarReST("CargaPedidoDocumentoCTe/ImprimirRelacaoEntrega", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    BuscarProcessamentosPendentes();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AguardeQueSeuRelatorioEstaSendoGerado);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.CargaSemPedidoSelecionado);
    }
}

function LiberarCargaComMDFesRejeitadosClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.AtencaoAoLiberarUmaCargaComMdfeRejeitadoOuSemMdfeVeiculoPodeSerMultadoPorAndarSemOsDocumentosFiscaisExigidosDesejaLiberarCargaAssimMesmo, function () {
        var data = { Carga: _cargaAtual.Codigo.val() };
        executarReST("CargaMDFe/LiberarCargaComMDFesRejeitados", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaMDFe.EmitirNovamenteRejeitados.visible(false);
                    _cargaMDFe.LiberarCargaComMDFesRejeitados.visible(false);
                    $("#" + _cargaAtual.EtapaMDFe.idGrid + " .DivMensagemMDF").hide();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaFoiLiberadaSemMdfeComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
                _gridCargaMDFe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ReemitirMDFesRejeitadosClick(e) {
    var data = { Carga: _cargaAtual.Codigo.val() };
    executarReST("CargaMDFe/ReemitirMDFesRejeitados", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _cargaMDFe.EmitirNovamenteRejeitados.visible(false);
                _cargaMDFe.LiberarCargaComMDFesRejeitados.visible(false);
                $("#" + _cargaAtual.EtapaMDFe.idGrid + " .DivMensagemMDF").hide();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.OsMdfesForamReemitidosComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
            _gridCargaMDFe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function RecarregarMDFeRejeicaoSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaMDFe != null) {
        if ($("#" + _cargaAtual.EtapaMDFe.idGrid).is(":visible")) {
            _gridCargaMDFe.CarregarGrid();
            preencherMotivoPendenciaMDFe(_cargaAtual);
            _cargaMDFe.EmitirNovamenteRejeitados.visible(true);
        }
    }
    EtapaMDFeProblema(knoutCarga);
}

function RecarregarMDFeAutorizadoSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaMDFe != null) {
        if ($("#" + _cargaAtual.EtapaMDFe.idGrid).is(":visible")) {
            _gridCargaMDFe.CarregarGrid();
        }
    }
}

function informarPrevisaoEncerramentoClick(e, sender) {
    _informarDataPrevisaoEncerramentoMDFe = new InformarDataPrevisaoEncerramentoMDFe();
    _informarDataPrevisaoEncerramentoMDFe.DataPrevisaoEncerramento.val(e.DataPrevisaoEncerramento);
    _informarDataPrevisaoEncerramentoMDFe.Codigo.val(e.CodigoMDFE);
    _informarDataPrevisaoEncerramentoMDFe.Carga.val(_cargaAtual.Codigo.val());

    KoBindings(_informarDataPrevisaoEncerramentoMDFe, "knoutInformarDataPrevisaoEncerramentoMDFe");

    Global.abrirModal("divModalInformarDataPrevisaoEncerramentoMDFe");
}

function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);

    Global.abrirModal("divModalRetornoSefaz");
}

function abrirModalMDFeManualClick(e) {

    _informacaoMDFe = new InformacaoMDFe();
    _informacaoMDFe.Chave.val(e.Chave);
    _informacaoMDFe.Observacao.val(e.Observacao);
    _informacaoMDFe.Codigo.val(e.CodigoMDFeManual);
    _informacaoMDFe.CargaMDFe.val(e.Codigo);
    if (e.MDFeInformado)
        _informacaoMDFe.InformarDados.enable(false);

    KoBindings(_informacaoMDFe, "knoutInformarDadosMDFe");

    Global.abrirModal("divModalInformarDadosMDFe");
}

function baixarXMLMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaMDFe/DownloadXMLAutorizacao", data);
}

function BaixarXMLEncerramentoMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaMDFe/DownloadXMLEncerramento", data);
}

function baixarDAMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa, Contingencia: false };
    executarDownload("CargaMDFe/DownloadDAMDFE", data);
}

function baixarDAMDFeContingenciaClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.AtencaoImprimirDAMFDEContingencia, function () {
        var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa, Contingencia: true };
        executarDownload("CargaMDFe/DownloadDAMDFE", data);
    });
}

function emitirMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarReST("CargaMDFe/EmitirNovamente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirReenvioMDFe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function sincronizarMDFeClick(e) {
    if (e.Status == EnumSituacaoMDFe.Enviado || e.Status == EnumSituacaoMDFe.EmCancelamento || e.Status == EnumSituacaoMDFe.EmEncerramento || e.Status == EnumSituacaoMDFe.EventoInclusaoMotoristaEnviado) {
        var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaMDFe/SincronizarDocumentoEmProcessamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirReenvioMDFe();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function exibirReenvioMDFe() {
    _gridCargaMDFe.CarregarGrid();
    _cargaAtual.PossuiPendencia.val(false);
    EtapaMDFeAguardando(_cargaAtual);
    _cargaMDFe.TentarNovamente.visible(false);
    $("#" + _cargaAtual.EtapaMDFe.idGrid + " .DivMensagemMDF").hide();
}

function preencherMotivoPendenciaMDFe(knoutCarga) {
    if (knoutCarga.MotivoPendencia.val() != "") {
        var html = "<div class='alert alert-info alert-block'>";
        html += "<h4 class='alert-heading'>" + Localization.Resources.Cargas.Carga.Pendencia + "</h4>";
        html += knoutCarga.MotivoPendencia.val();
        html += "</div>";

        $("#" + knoutCarga.EtapaMDFe.idGrid + " .MensageMDF").html(html);
        $("#" + knoutCarga.EtapaMDFe.idGrid + " .DivMensagemMDF").show();
    }
}

function buscarCargaMDFe(callback) {
    var informarPrevisaoEncerramento = { descricao: Localization.Resources.Cargas.Carga.PrevisaoDeEncerramento, id: guid(), metodo: informarPrevisaoEncerramentoClick, visibilidade: visibilidadePrevisaoEncerramento };
    var retornoSefaz = { descricao: Localization.Resources.Cargas.Carga.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: visibilidadeRetornoSefaz };
    var baixarDAMDFE = { descricao: Localization.Resources.Cargas.Carga.BaixarDamdfe, id: guid(), metodo: baixarDAMDFeClick, visibilidade: visibilidadeDAMDFE };
    var baixarDAMDFEContingencia = { descricao: Localization.Resources.Cargas.Carga.BaixarDamdfeContingencia, id: guid(), metodo: baixarDAMDFeContingenciaClick, visibilidade: visibilidadeDAMDFEContingencia };
    var baixarXMLMDFe = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLMDFeClick, visibilidade: visibilidadeDAMDFE };
    var baixarXMLEncerramentoMDFe = { descricao: Localization.Resources.Cargas.Carga.BaixarXmlEncerramento, id: guid(), metodo: BaixarXMLEncerramentoMDFeClick, visibilidade: VisibilidadeMDFeEncerrado };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("ManifestoEletronicoDeDocumentosFiscais", "CodigoMDFE"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var emitir = { descricao: Localization.Resources.Cargas.Carga.Emitir, id: guid(), metodo: function (datagrid) { emitirMDFeClick(datagrid); }, icone: "", visibilidade: visibilidadeEmitir };
    let sincronizarMDFe = { descricao: Localization.Resources.Cargas.Carga.SincronizarDocumento, id: guid(), metodo: function (datagrid) { sincronizarMDFeClick(datagrid); }, visibilidade: VisibilidadeSincronizarMDFe };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDAMDFE, baixarDAMDFEContingencia, baixarXMLMDFe, baixarXMLEncerramentoMDFe, informarPrevisaoEncerramento, retornoSefaz, emitir, sincronizarMDFe, auditar] };

    _gridCargaMDFe = new GridView(_cargaMDFe.Pesquisar.idGrid, "CargaMDFe/ConsultarCargaMDFe", _cargaMDFe, menuOpcoes, null);
    _gridCargaMDFe.CarregarGrid(callback);
}

function visibilidadePrevisaoEncerramento(row) {
    return _CONFIGURACAO_TMS.EncerrarMDFeAutomaticamente && row.Importado != 1;
}

function visibilidadeRetornoSefaz(row) {
    if (row.Importado != 1)
        return true;
    else
        return false;
}

function visibilidadeDAMDFE(row) {
    if (row.Status == EnumSituacaoMDFe.Autorizado || row.Status == EnumSituacaoMDFe.Encerrado)
        return true;
    else
        return false;
}

function visibilidadeDAMDFEContingencia(row) {
    if ((row.Status == EnumSituacaoMDFe.Rejeicao && row.CodigoDoErro == 8888 && _CONFIGURACAO_TMS.PermitirImpressaoDAMDFEContingencia) || row.Status == EnumSituacaoMDFe.EmitidoContingencia)
        return true;
    else
        return false;
}

function VisibilidadeMDFeEncerrado(row) {
    return (row.Status === EnumSituacaoMDFe.Encerrado);
}

function visibilidadeEmitir(row) {
    if ((_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Anulada) || (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.Cancelada))
        return false;

    return (row.Status == EnumSituacaoMDFe.Rejeicao || row.Status == EnumSituacaoMDFe.Pendente || row.Status == EnumSituacaoMDFe.EmitidoContingencia);
}

function VisibilidadeSincronizarMDFe(data) {
    if (data.HabilitarSincronizarDocumento == true) {
        return true;
    } else {
        return false;
    }
}

function informarDadosMDFeClick(e) {
    if (_informacaoMDFe.Chave.val() != "" || _informacaoMDFe.Observacao.val() != "") {
        Salvar(_informacaoMDFe, "CargaMDFe/InformarDadosMDFeManual", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal("divModalInformarDadosMDFe");
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MdfeInformadoComSucesso);
                    $("#" + _idTabEtapaMDFe).trigger("click");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.ObrigatorioInformarChaveOuUmaObservacaoSobreMdfe);
    }

}

function InformarDataPrevisaoEncerramentoMDFeClick(e) {
    Salvar(_informarDataPrevisaoEncerramentoMDFe, "CargaMDFe/InformarDataPrevisaoEncerramento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                Global.fecharModal("divModalInformarDataPrevisaoEncerramentoMDFe");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MdfeDataDePrevisaoInformadoComSucesso);
                _gridCargaMDFe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function DownloadLoteXMLMDFeClick(e) {
    executarDownload("CargaMDFe/DownloadLoteXML", { Carga: e.Carga.val(), CodigoEmpresa: e.CodigoEmpresa });
}

function DownloadLoteDocumentosMDFeClick(e) {
    executarDownload("CargaIntegracaoCarga/DownloadLoteDocumentos", { Carga: e.Carga.val() });
}

function DownloadLoteDAMDFEClick(e) {
    executarDownload("CargaMDFe/DownloadLoteDAMDFE", { Carga: e.Carga.val(), CodigoEmpresa: e.CodigoEmpresa });
}

//*******MÉTODOS*******

function EtapaMDFeDesabilitada(e) {
    $("#" + e.EtapaMDFe.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaMDFe.idTab + " .step").attr("class", "step");

    $("#" + e.EtapaMDFeFilialEmissora.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaMDFeFilialEmissora.idTab + " .step").attr("class", "step");

    e.EtapaMDFe.eventClick = function (e) { };
}

function EtapaMDFeAguardando(e) {
    $("#" + e.EtapaMDFe.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFe.idTab + " .step").attr("class", "step yellow");

    $("#" + e.EtapaMDFeFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeFilialEmissora.idTab + " .step").attr("class", "step yellow");

    e.EtapaMDFe.eventClick = buscarMDFeClick;
    aprovarEtapasAnterioresMDFe(e);
}

function EtapaMDFeLiberada(e) {
    $("#" + e.EtapaMDFe.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFe.idTab + " .step").attr("class", "step yellow");

    $("#" + e.EtapaMDFeFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeFilialEmissora.idTab + " .step").attr("class", "step yellow");

    e.EtapaMDFe.eventClick = buscarMDFeClick;
    aprovarEtapasAnterioresMDFe(e);
}

function EtapaMDFeAprovada(e) {
    $("#" + e.EtapaMDFe.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFe.idTab + " .step").attr("class", "step green");

    $("#" + e.EtapaMDFeFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeFilialEmissora.idTab + " .step").attr("class", "step green");

    e.EtapaMDFe.eventClick = buscarMDFeClick;
    aprovarEtapasAnterioresMDFe(e);
}

function EtapaMDFeProblema(e) {
    $("#" + e.EtapaMDFe.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFe.idTab + " .step").attr("class", "step red");

    $("#" + e.EtapaMDFeFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaMDFeFilialEmissora.idTab + " .step").attr("class", "step red");

    e.EtapaMDFe.eventClick = buscarMDFeClick;
    aprovarEtapasAnterioresMDFe(e);
}

function EtapaMDFeEdicaoDesabilitada(e) {
    e.EtapaMDFe.enable(false);
    EtapaCTeNFsEdicaoDesabilitada(e);
}

function aprovarEtapasAnterioresMDFe(e) {
    if (e.PossuiCTeSubcontratacaoFilialEmissora.val()) {
        if (e.EmiteMDFeFilialEmissora.val())
            EtapaCTeFilialEmissoraAprovada(e);
        else
            EtapaCTeNFsAprovada(e);
    }
    else
        EtapaCTeNFsAprovada(e);
}

function sincronizarTodosDocumentoMDFeClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteSincronizarTodosDocumentoDestaCarga, function () {
        sincronizarTodosDocumentoMDFe({ Carga: e.Carga.val() });
    });
}


function sincronizarTodosDocumentoMDFe(data) {
    executarReST("CargaMDFe/SincronizarLoteDocumentoEmProcessamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirReenvioMDFe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}