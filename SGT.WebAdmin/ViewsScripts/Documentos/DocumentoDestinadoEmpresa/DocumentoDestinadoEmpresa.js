/// <reference path="ManifestacaoDestinatario.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="../../Enumeradores/EnumSituacaoManifestacaoDestinatario.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoDestinadoEmpresa.js" />
/// <reference path="../../Enumeradores/EnumTipoOperacaoNotaFiscal.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoDocumento = [
    { text: "NF-e Destinada", value: EnumTipoDocumentoDestinadoEmpresa.NFeDestinada },
    { text: "NF-e para Transporte", value: EnumTipoDocumentoDestinadoEmpresa.NFeTransporte },
    { text: "CC-e", value: EnumTipoDocumentoDestinadoEmpresa.CCe },
    { text: "Cancelamento de NF-e", value: EnumTipoDocumentoDestinadoEmpresa.CancelamentoNFe },
    { text: "Manifestação do Destinatário", value: EnumTipoDocumentoDestinadoEmpresa.MDe },
    { text: "Autorização de CT-e", value: EnumTipoDocumentoDestinadoEmpresa.AutorizacaoCTe },
    { text: "Autorização de Download", value: EnumTipoDocumentoDestinadoEmpresa.AutorizadoDownload },
    { text: "Cancelamento de CT-e", value: EnumTipoDocumentoDestinadoEmpresa.CancelamentoCTe },
    { text: "Cancelamento de MDF-e", value: EnumTipoDocumentoDestinadoEmpresa.CancelamentoMDFe },
    { text: "Autorização de MDF-e", value: EnumTipoDocumentoDestinadoEmpresa.AutorizacaoMDFe },
    { text: "Passagem NF-e RFID", value: EnumTipoDocumentoDestinadoEmpresa.PassagemNFeRFID },
    { text: "Passagem NF-e", value: EnumTipoDocumentoDestinadoEmpresa.PassagemNFe },
    { text: "Autorizacao MDF-e Com CT-e", value: EnumTipoDocumentoDestinadoEmpresa.AutorizacaoMDFeComCTe },
    { text: "Passagem NF-e Propagado Pelo MDF-e Ou CT-e", value: EnumTipoDocumentoDestinadoEmpresa.PassagemNFePropagadoPeloMDFeOuCTe },
    { text: "Passagem NF-e Automatico Pelo MDF-e Ou CT-e", value: EnumTipoDocumentoDestinadoEmpresa.PassagemNFeAutomaticoPeloMDFeOuCTe },
    { text: "Cancelamento MDF-e Autorizado Com CT-e", value: EnumTipoDocumentoDestinadoEmpresa.CancelamentoMDFeAutorizadoComCTe },
    { text: "CT-e Destinado Remetente", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente },
    { text: "CT-e Destinado Destinatario", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario },
    { text: "CT-e Destinado Expedidor", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor },
    { text: "CT-e Destinado Recebedor", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor },
    { text: "CT-e Destinado Tomador", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoTomador },
    { text: "CT-e Destinado Emitente", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente },
    { text: "CT-e Destinado Terceiro", value: EnumTipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro },
    { text: "CT-e OS Destinado Tomador", value: EnumTipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador },    
    { text: "Cancelamento Passagem NF-e", value: EnumTipoDocumentoDestinadoEmpresa.CancelamentoPassagemNFe },
    { text: "Passagem Automatico MDFe", value: EnumTipoDocumentoDestinadoEmpresa.PassagemAutomaticoMDFe },
    { text: "MDF-e Encerrado", value: EnumTipoDocumentoDestinadoEmpresa.EncerramentoMDFe },
    { text: "MDF-e Cancelado", value: EnumTipoDocumentoDestinadoEmpresa.MDFECancelado },
    { text: "MDF-e Destinado", value: EnumTipoDocumentoDestinadoEmpresa.MDFeDestinado },
    { text: "NFS-e Destinada", value: EnumTipoDocumentoDestinadoEmpresa.NFSeDestinada }    
];

var _situacaoMDe = [
    { text: "Ciência da operação", value: EnumSituacaoManifestacaoDestinatario.CienciaOperacao },
    { text: "Confirmada a operação", value: EnumSituacaoManifestacaoDestinatario.ConfirmadaOperacao },
    { text: "Desconhecida", value: EnumSituacaoManifestacaoDestinatario.Desconhecida },
    { text: "Operação não realizada", value: EnumSituacaoManifestacaoDestinatario.OperacaoNaoRealizada },
    { text: "Sem manifestação", value: EnumSituacaoManifestacaoDestinatario.SemManifestacao }
];

var _situacaoCancelamentoDocumentoDestinado = [
    { text: "Todos", value: "" },
    { text: "Cancelado", value: true },
    { text: "Autorizado", value: false }
];

var _situacaoPossuiDocumentoEntrada = [
    { text: "Todos", value: "" },
    { text: "Gerado", value: true },
    { text: "Não gerado", value: false }
];

var _gridDocumentoDestinadoEmpresa;
var _pesquisaDocumentoDestinadoEmpresa;
var _gridManifestacaoDestinatario;
var _pesquisaManifestacaoDestinatario;
var _AuditoriaOrdemServico;
var _gridAuditoriaOrdemServico;
var _PermissoesPersonalizadas;
var _gerandoDocumentoEntrada = false;
var _utilizaIntegracaoDocumentosDestinado = false;
var _buscaVinculoDocumentoEntrada;
var _configuracaoGeral;
var _modalEmissaoManifestacaoDestinatario;
var _modalEmissaoDesacordo;
var _modalVisualizarManifestacaoDestinatario;
var _modalVisualizarAuditoriaOrdemServico;
var _modalConsultaStatusSefazPorChave;
var _consultaStatusSefazPorChave;
var _gridHistoricoIntegracaoSAP;
var _pesquisaHistoricoIntegracaoSAP;

var ConsultaStatusSefazPorChave = function () {

    this.Chave = PropertyEntity({ text: "Chave:", maxlength: 44 });

    this.Consultar = PropertyEntity({
        eventClick: function (e) {
            EfetuarConsultaStatusSefazPorChave();
        }, type: types.event, text: "Consultar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Cancelar = PropertyEntity({
        eventClick: function (e) {
            _modalConsultaStatusSefazPorChave.hide();
        }, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true)
    });
};

var PesquisaManifestacaoDestinatario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var AuditoriaOrdemServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.FornecedorDocumentoEntrada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.FornecedorOrdemCompra = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
};



var PesquisaDocumentoDestinadoEmpresa = function () {

    this.Chave = PropertyEntity({ text: "Chave:", maxlength: 44 });
    this.CPFCNPJFornecedor = PropertyEntity({ text: "CPF/CNPJ Fornecedor:", maxlength: 14 });
    this.NomeFornecedor = PropertyEntity({ text: "Nome do Fornecedor:", maxlength: 100 });
    this.NumeroDe = PropertyEntity({ text: "Número de:", getType: typesKnockout.int });
    this.NumeroAte = PropertyEntity({ text: "Até:", getType: typesKnockout.int });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int });
    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data Autorização Inicial:", getType: typesKnockout.date });
    this.DataAutorizacaoFinal = PropertyEntity({ text: "Data Autorização Final:", getType: typesKnockout.date });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoDocumento = PropertyEntity({ text: "Tipo de Documento:", issue: 1115, val: ko.observable([EnumTipoDocumentoDestinadoEmpresa.NFeDestinada]), options: _tipoDocumento, def: [EnumTipoDocumentoDestinadoEmpresa.NFeDestinada], getType: typesKnockout.selectMultiple });
    this.SituacaoManifestacaoDestinatario = PropertyEntity({ text: "Situação da MD-e:", issue: 1116, val: ko.observable([]), options: _situacaoMDe, def: [], getType: typesKnockout.selectMultiple });
    this.Cancelado = PropertyEntity({ text: "Situação do Documento:", val: ko.observable(""), options: _situacaoCancelamentoDocumentoDestinado, def: "" });
    this.PossuiDocumentoEntrada = PropertyEntity({ text: "Documento de Entrada:", val: ko.observable(""), options: _situacaoPossuiDocumentoEntrada, def: "" });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação:", val: ko.observable(""), options: EnumTipoOperacaoNotaFiscal.obterOpcoesPesquisa(), def: "" });

    this.DataAutorizacaoFinal.dateRangeInit = this.DataAutorizacaoInicial;
    this.DataAutorizacaoInicial.dateRangeLimit = this.DataAutorizacaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaDocumentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.SituacaoIntegracao = PropertyEntity({ text: "Situação Integração:", val: ko.observable(""), options: EnumSituacaoIntegracao.obterOpcoesPesquisa(), def: "" });

    this.ReintegrarDocumentos = PropertyEntity({
        eventClick: function (e) {
            ReintegrarDocumentos();
        }, type: types.event, text: "Reenviar integrações pendentes", idGrid: guid(), icon: ko.observable("fal fa-share"), visible: ko.observable(true)
    });

    this.ConsultarStatusSefazPorChave = PropertyEntity({
        eventClick: function (e) {
            _modalConsultaStatusSefazPorChave.show();
        }, type: types.event, text: "Consultar Documento Individual", idGrid: guid(), icon: ko.observable("fal fa-search"), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoDestinadoEmpresa.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-minus"), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ConsultarDocumentosDestinados = PropertyEntity({
        eventClick: function (e) {
            ConsultarDocumentosDestinados();
        }, type: types.event, text: "Consultar Documentos Destinados", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });

    this.EmitirManifestacaoDestinatario = PropertyEntity({
        eventClick: function (e) {
            AbrirTelaEmissaoManifestacaoDestinatario();
        }, type: types.event, text: "Emitir Manifestação de NF-e do Destinatário", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.EmitirDesacordoCTe = PropertyEntity({
        eventClick: function (e) {
            AbrirTelaEmissaoDesacordo();
        }, type: types.event, text: "Emitir Desacordo dos CT-es", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo, _PermissoesPersonalizadas) : true)
    });

    this.GerarDocumentoEntrada = PropertyEntity({
        eventClick: function (e) {
            GerarDocumentoEntradaLote();
        }, type: types.event, text: "Gerar Documento de Entrada", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ConsultarCTeDestinados = PropertyEntity({
        eventClick: function (e) {
            GerarCTesDocumentosDestinados();
        }, type: types.event, text: "Buscar CT-es Destinados", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });

    this.ConsultarMDFeDestinados = PropertyEntity({
        eventClick: function (e) {
            GerarMDFesDocumentosDestinados();
        }, type: types.event, text: "Buscar MDFe-es Destinados", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)

    });
    this.ConsultarXMLCTeDestinados = PropertyEntity({
        eventClick: function (e) {
            GerarCTesXMLDocumentosDestinados();
        }, type: types.event, text: "Processar CT-es do XML Importado", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(false)
    });

    this.DownloadLoteXML = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteXMLNFeClick();
        }, type: types.event, text: "Baixar Lote de XML NF-e", idFade: guid(), icon: "fal fa-download"
    });

    this.DownloadLoteXMLCTe = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteXMLCTeClick();
        }, type: types.event, text: "Baixar Lote de XML CT-e", idFade: guid(), icon: "fal fa-download"
    });

    //Propriedades Virtuais
    this.VinculoDocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Vínculo:", idBtnSearch: guid() });
};

var PesquisaHistoricoIntegracaoSAP = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};
//*******EVENTOS*******

function loadDocumentoDestinadoEmpresa() {
    _pesquisaDocumentoDestinadoEmpresa = new PesquisaDocumentoDestinadoEmpresa();
    KoBindings(_pesquisaDocumentoDestinadoEmpresa, "knockoutPesquisaDocumentoDestinadoEmpresa", false, _pesquisaDocumentoDestinadoEmpresa.Pesquisar.id);

    _consultaStatusSefazPorChave = new ConsultaStatusSefazPorChave();
    KoBindings(_consultaStatusSefazPorChave, "knockoutConsultaStatusSefazPorChave", false);

    ObterConfiguracaoGeral();

    HeaderAuditoria("DocumentoDestinadoEmpresa");

    new BuscarTransportadores(_pesquisaDocumentoDestinadoEmpresa.Empresa, null, null, true);
    _buscaVinculoDocumentoEntrada = new BuscarDocumentoEntrada(_pesquisaDocumentoDestinadoEmpresa.VinculoDocumentoEntrada, RetornoVinculoDocumentoEntrada);

    BuscarConfiguracoesEmpresa();
    loadManifestacaoDestinatario();

    configurarLayoutPorTipoSistemaDocumentoDestinado();
    _modalEmissaoManifestacaoDestinatario = new bootstrap.Modal(document.getElementById("divModalEmissaoManifestacaoDestinatario"), { backdrop: true, keyboard: true });
    _modalEmissaoDesacordo = new bootstrap.Modal(document.getElementById("divModalEmissaoDesacordo"), { backdrop: true, keyboard: true });
    _modalVisualizarManifestacaoDestinatario = new bootstrap.Modal(document.getElementById("divModalVisualizarManifestacaoDestinatario"), { backdrop: true, keyboard: true });
    _modalVisualizarAuditoriaOrdemServico = new bootstrap.Modal(document.getElementById("divModalVisualizarAuditoriaOrdemServico"), { backdrop: true, keyboard: true });
    _modalConsultaStatusSefazPorChave = new bootstrap.Modal(document.getElementById("divModalConsultarStatusSefazPorChave"), { backdrop: true, keyboard: true });
    _pesquisaHistoricoIntegracaoSAP = new PesquisaHistoricoIntegracaoSAP();
    loadGridHistoricoIntegracaoSAP();
}

function downloadArquivosHistoricoIntegracaoSAPClick(row) {
    executarDownload("DocumentoDestinadoEmpresa/DownloadArquivosHistoricoIntegracaoSAP", { Codigo: row.Codigo });
}

function loadGridHistoricoIntegracaoSAP() {
    var opcaoDownload = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: downloadArquivosHistoricoIntegracaoSAPClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    _gridHistoricoIntegracaoSAP = new GridView("gridHistoricoIntegracaoSAP", "DocumentoDestinadoEmpresa/ObterHistoricoIntegracaoSAP", _pesquisaHistoricoIntegracaoSAP, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function exibirHistoricoIntegracaoSAPClick(row) {
    _pesquisaHistoricoIntegracaoSAP.Codigo.val(row.Codigo);
    _pesquisaHistoricoIntegracaoSAP.TipoDocumento.val(row.CodigoTipoDocumento);
    _gridHistoricoIntegracaoSAP.CarregarGrid();
    Global.abrirModal("divHistoricoIntegracaoSAP");
}

function configurarLayoutPorTipoSistemaDocumentoDestinado() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaDocumentoDestinadoEmpresa.ConsultarMDFeDestinados.visible(true);
        _pesquisaDocumentoDestinadoEmpresa.ConsultarCTeDestinados.visible(true);
        _pesquisaDocumentoDestinadoEmpresa.GerarDocumentoEntrada.visible(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaDocumentoDestinadoEmpresa.Empresa.visible(false);
        _pesquisaDocumentoDestinadoEmpresa.ConsultarDocumentosDestinados.visible(true);
    }
}

function executarConsultaPorChave(chave) {
    _modalConsultaStatusSefazPorChave.hide();
    var data = {
        Chave: chave,
        CodigoEmpresa: _pesquisaDocumentoDestinadoEmpresa.Empresa.codEntity()
    };
    executarReST("DocumentoDestinadoEmpresa/ConsultarStatusSefazPorChave", data, function (arg) {
        if (arg.Success) {
            _gridDocumentoDestinadoEmpresa.CarregarGrid();
            exibirSucesso("Consulta efetuada!", arg.Msg, "Ok");
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
        }
    });
}

function executarConsultaPorChaveTabela(row) {
    executarConsultaPorChave(row.Chave);
}

function EfetuarConsultaStatusSefazPorChave() {
    if (_consultaStatusSefazPorChave.Chave.val().length == 44) {
        executarConsultaPorChave(_consultaStatusSefazPorChave.Chave.val());
    } else {
        exibirMensagem(tipoMensagem.atencao, "Chave", "A chave do documento deve ser informada com 44 caracteres numéricos.");
    }
}

function BuscarConfiguracoesEmpresa() {
    executarReST("Usuario/DadosUsuarioLogado", null, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _utilizaIntegracaoDocumentosDestinado = arg.Data.Empresa.UtilizaIntegracaoDocumentosDestinado;
            }
        }
        buscarDocumentosDestinadosEmpresa();
    });
}

function ConsultarDocumentosDestinados() {
    exibirConfirmacao("Atenção!", "Esta consulta pode demorar alguns minutos, de acordo com a quantidade de notas fiscais disponíveis na SEFAZ. Deseja prosseguir?", function () {

        executarReST("DocumentoDestinadoEmpresa/ConsultarDocumentosDestinados", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos consultados com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}


function ReintegrarDocumentos() {

    exibirConfirmacao("Atenção!", "Esta reintegração pode demorar alguns minutos, de acordo com a quantidade de documentos pendentes. Deseja prosseguir?", function () {
        var data = { Empresa: _pesquisaDocumentoDestinadoEmpresa.Empresa.codEntity() };
        executarReST("DocumentoDestinadoEmpresa/ReintegrarDocumentosDestinados", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O processo de reintegração dos documentos foi iniciado com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });

}

function GerarCTesXMLDocumentosDestinados() {
    exibirConfirmacao("Atenção!", "Esta consulta pode demorar alguns minutos, de acordo com a quantidade de ctes importados. Deseja prosseguir?", function () {

        var data = { Empresa: _pesquisaDocumentoDestinadoEmpresa.Empresa.codEntity() };
        executarReST("DocumentoDestinadoEmpresa/ConsultarXMLCTesDestinados", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos consultados com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function GerarMDFesDocumentosDestinados() {
    exibirConfirmacao("Atenção!", "Esta consulta pode demorar alguns minutos, de acordo com a quantidade de mdfes disponíveis na SEFAZ. Deseja prosseguir?", function () {

        var data = { Empresa: _pesquisaDocumentoDestinadoEmpresa.Empresa.codEntity() };
        executarReST("DocumentoDestinadoEmpresa/ConsultarMDFesDestinados", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos consultados com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function GerarCTesDocumentosDestinados() {
    exibirConfirmacao("Atenção!", "Esta consulta pode demorar alguns minutos, de acordo com a quantidade de ctes disponíveis na SEFAZ. Deseja prosseguir?", function () {

        var data = { Empresa: _pesquisaDocumentoDestinadoEmpresa.Empresa.codEntity() };
        executarReST("DocumentoDestinadoEmpresa/ConsultarCTesDestinados", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos consultados com sucesso!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function GerarDocumentoEntradaLote() {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoDocumentoEntrada, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você não possui permissão para gerar documentos de entrada!");
        return;
    }

    CarregarDocumentos();

    if (!string.IsNullOrWhiteSpace(_pesquisaDocumentoDestinadoEmpresa.ListaDocumentos.val())) {
        exibirConfirmacao("Atenção!", "A geração dos documentos de entrada pode demorar alguns minutos, de acordo com a quantidade de notas fiscais disponíveis. Deseja prosseguir?", function () {
            if (!_gerandoDocumentoEntrada) {

                _gerandoDocumentoEntrada = true;

                var dados = RetornarObjetoPesquisa(_pesquisaDocumentoDestinadoEmpresa);

                executarReST("DocumentoDestinadoEmpresa/GerarDocumentoEntradaLote", dados, function (arg) {
                    _gerandoDocumentoEntrada = false;

                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos de entrada gerados com sucesso!");
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg, 16000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                }, function (x) {
                    _gerandoDocumentoEntrada = false;
                    exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
                });
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar ao menos uma nota para gerar os documentos de entrada.");
        return;
    }
}

function GerarDocumentoEntrada(documento) {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoDocumentoEntrada, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você não possui permissão para gerar documentos de entrada!");
        return;
    }

    exibirConfirmacao("Atenção!", "Deseja realmente gerar um documento de entrada a partir da nota " + documento.Numero + "?", function () {

        if (!_gerandoDocumentoEntrada) {
            _gerandoDocumentoEntrada = true;

            executarReST("DocumentoDestinadoEmpresa/GerarDocumentoEntrada", { Codigo: documento.Codigo }, function (arg) {
                _gerandoDocumentoEntrada = false;

                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento de entrada gerado com sucesso!");
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg, 16000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }

            }, function (x) {
                _gerandoDocumentoEntrada = false;
                exibirMensagem(tipoMensagem.falha, "Falha", "Não foi possível realizar uma requisição para o servidor. Erro: " + x.status + " - " + x.statusText);
            });
        }
    });
}

function VincularDocumentoEntrada(documento) {
    exibirConfirmacao("Atenção!", "Deseja realmente vincular um documento de entrada com a nota " + documento.Numero + "?", function () {
        _pesquisaDocumentoDestinadoEmpresa.VinculoDocumentoEntrada.val(documento.Codigo);
        _buscaVinculoDocumentoEntrada.AbrirBusca();
    });
}

function RetornoVinculoDocumentoEntrada(data) {
    executarReST("DocumentoDestinadoEmpresa/VincularDocumentoEntrada", { Codigo: _pesquisaDocumentoDestinadoEmpresa.VinculoDocumentoEntrada.val(), CodigoDocumentoEntrada: data.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Documento de entrada vinculado com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function DownloadXMLNFe(documento) {
    executarDownload("DocumentoDestinadoEmpresa/DownloadXMLNFe", { Codigo: documento.Codigo });
}

function DownloadXMLCTe(documento) {
    executarDownload("DocumentoDestinadoEmpresa/DownloadXMLCTe", { Codigo: documento.Codigo });
}

function reintegrarDocumento(documento) {
    exibirConfirmacao("Atenção!", "Deseja realmente reintegrar este documento?", function () {
        var data = { codigo: documento.Codigo };
        executarReST("DocumentoDestinadoEmpresa/ReintegrarDocumentoDestinado", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "O processo de reintegração do documento foi iniciado com sucesso, verifique em instantes o resultado no histórico de integração!");
                    _gridDocumentoDestinadoEmpresa.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function DownloadXMLMDFe(documento) {
    executarDownload("DocumentoDestinadoEmpresa/DownloadXMLMDFe", { Codigo: documento.Codigo });
}

function EmitirManifestacao_Menu(documento) {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_BloquearGeracaoManifestacao, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você não possui permissão para gerar manifestações!");
        return;
    }

    _manifestacaoDestinatario.Codigo.val(documento.Codigo);
    _modalEmissaoManifestacaoDestinatario.show();
}

function EmitirDesacordo_Menu(documento) {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você não possui permissão para Emitir Desacordo");
    }
    else if (documento.DocumentoCTe && documento.DocumentoCTeNaoComplementarPossuiOcorrencia) {
        exibirConfirmacao('Atenção', 'O Documento já possui Ocorrências e/ou documentos complementares vinculados, realmente deseja prosseguir?', () => AbrirModalEmissaoDesacordo(documento));
    }
    else {
        AbrirModalEmissaoDesacordo(documento);
    }
}

function auditoriaOrdemServicoClick(documento) {
    BuscarAuditoriaOrdemServico(documento);
    _modalVisualizarAuditoriaOrdemServico.show();
}

function BuscarAuditoriaOrdemServico(documento) {
    _AuditoriaOrdemServico = new AuditoriaOrdemServico();
    KoBindings(_AuditoriaOrdemServico, "divModalVisualizarAuditoriaOrdemServico")

    _AuditoriaOrdemServico.Codigo.val(documento.CodigoDocumentoEntrada);
    _AuditoriaOrdemServico.FornecedorDocumentoEntrada.val(documento.FornecedorDocumentoEntrada);
    _AuditoriaOrdemServico.FornecedorOrdemCompra.val(documento.FornecedorOrdemCompra);

    _gridAuditoriaOrdemServico = new GridView("tblAuditoriaOrdemServico", "DocumentoDestinadoEmpresa/PesquisaAuditoriaOrdemServico", _AuditoriaOrdemServico, null, { column: 1, dir: orderDir.desc });
    _gridAuditoriaOrdemServico.CarregarGrid();
}

function VisualizarManifestacoesClick(documento) {
    BuscarManifestacoesDestinatario(documento);
    _modalVisualizarManifestacaoDestinatario.show();
}

function BuscarManifestacoesDestinatario(documento) {
    _pesquisaManifestacaoDestinatario = new PesquisaManifestacaoDestinatario();
    _pesquisaManifestacaoDestinatario.Codigo.val(documento.Codigo);

    _gridManifestacaoDestinatario = new GridView("tblManifestacaoDestinatario", "DocumentoDestinadoEmpresa/PesquisaManifestacaoDestinatario", _pesquisaManifestacaoDestinatario, null, { column: 1, dir: orderDir.desc });
    _gridManifestacaoDestinatario.CarregarGrid();
}

function DownloadDANFENFe(documento) {
    executarDownload("DocumentoDestinadoEmpresa/DownloadDANFENFe", { Codigo: documento.Codigo, Chave: documento.Chave });
}

function DownloadLoteXMLNFeClick() {
    exibirConfirmacao("Atenção!", "O download de lote dos documentos baixará apenas os documentos com XMLs armazenados no servidor do sistema. Deseja prosseguir?", function () {
        var data = RetornarObjetoPesquisa(_pesquisaDocumentoDestinadoEmpresa);
        executarDownload("DocumentoDestinadoEmpresa/DownloadLoteXMLNFe", data);
    });
}

function DownloadLoteXMLCTeClick() {
    exibirConfirmacao("Atenção!", "O download de lote dos documentos baixará apenas os documentos com XMLs armazenados no servidor do sistema. Deseja prosseguir?", function () {
        var data = RetornarObjetoPesquisa(_pesquisaDocumentoDestinadoEmpresa);
        executarDownload("DocumentoDestinadoEmpresa/DownloadLoteXMLCTe", data);
    });
}

function SolicitarArquivoTxtFTP(documento) {
    exibirConfirmacao("Atenção!", "Deseja realmente solicitar o TXT via FTP do documento " + documento.Numero + "?", function () {
        executarReST("DocumentoDestinadoEmpresa/SolicitarTXTFTP", { Codigo: documento.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "TXT solicitado com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function EnviarXMLParaFTP(documento) {
    executarDownload("DocumentoDestinadoEmpresa/EnviarXMLParaFTP", { Codigo: documento.Codigo });
}

function DownloadTXTDocumento(documento) {
    executarDownload("DocumentoDestinadoEmpresa/DownloadTXTDocumento", { Codigo: documento.Codigo });
}

function downloadXMLCCeClick(registro) {
    executarDownload('DocumentoNF/DownloadXmlCCe', { Codigo: registro.Codigo });
}

function downloadDACCeClick(registro) {
    executarDownload('DocumentoNF/DownloadDACCe', { Codigo: registro.Codigo });
}


function VisibilidadeParaNFe(dataRow) {
    return !dataRow.DocumentoCTe && !dataRow.DocumentoMDFe;
}

function VisibilidadeParaGerarDocumentoEntrada(dataRow) {
    return !dataRow.DocumentoCTe && !dataRow.DocumentoMDFe && (!_configuracaoGeral.BloquearLancamentoDocumentosTipoEntrada || dataRow.TipoOperacao != "Entrada");
}

function VisivilidadeAuditoriaOrdemServico(dataRow) {
    return dataRow.CodigoDocumentoEntrada > 0;
}

function VisibilidadeParaVincularDocumentoEntrada(dataRow) {
    return !dataRow.DocumentoCTe && !dataRow.DocumentoMDFe;
}

function VisibilidadeParaCTe(dataRow) {
    return dataRow.DocumentoCTe &&
        (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo, _PermissoesPersonalizadas) ||
        _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador);
}

function VisibilidadeParaMDFe(dataRow) {
    return dataRow.DocumentoMDFe;
}

function VisibilidadeParaTXT(dataRow) {
    return _utilizaIntegracaoDocumentosDestinado;
}

function visibilidadeIntegracaoSAP(dataRow) {
    return (VisibilidadeParaNFe(dataRow) || VisibilidadeParaCTe(dataRow));
}

function visibilidadeDownloadXMLCCe(dataRow) {
    return dataRow.CodigoTipoDocumento == EnumTipoDocumentoDestinadoEmpresa.CCe;
}

function visibilidadeDownloadDACCe(dataRow) {
    return visibilidadeDownloadXMLCCe(dataRow);
}


//*******MÉTODOS*******

function buscarDocumentosDestinadosEmpresa() {
    var downloadXMLNFe = { descricao: "Download XML NF-e", id: guid(), evento: "onclick", metodo: DownloadXMLNFe, tamanho: "20", icone: "", visibilidade: VisibilidadeParaNFe };
    var downloadDANFENFe = { descricao: "Download DANFE NF-e", id: guid(), evento: "onclick", metodo: DownloadDANFENFe, tamanho: "20", icone: "", visibilidade: VisibilidadeParaNFe };
    var emitirManifestacao = { descricao: "Emitir Manifestação", id: guid(), evento: "onclick", metodo: EmitirManifestacao_Menu, tamanho: "20", icone: "", visibilidade: VisibilidadeParaNFe };
    var visualizarManifestacoes = { descricao: "Visualizar Manifestações", id: guid(), evento: "onclick", metodo: VisualizarManifestacoesClick, tamanho: "20", icone: "" };
    var gerarDocumentoEntrada = { descricao: "Gerar Documento de Entrada", id: guid(), evento: "onclick", metodo: GerarDocumentoEntrada, tamanho: "20", icone: "", visibilidade: VisibilidadeParaGerarDocumentoEntrada };
    var vincularDocumentoEntrada = { descricao: "Vincular Documento de Entrada", id: guid(), evento: "onclick", metodo: VincularDocumentoEntrada, tamanho: "20", icone: "", visibilidade: VisibilidadeParaVincularDocumentoEntrada };
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("DocumentoDestinadoEmpresa", null, _manifestacaoDestinatario), tamanho: "20", icone: "" };
    var auditoriaOrdemServico = { descricao: "Validação Doc. X O.C.", id: guid(), evento: "onclick", metodo: auditoriaOrdemServicoClick, tamanho: "20", icone: "", visibilidade: VisivilidadeAuditoriaOrdemServico };

    var downloadXMLCTe = { descricao: "Download XML CT-e", id: guid(), evento: "onclick", metodo: DownloadXMLCTe, tamanho: "20", icone: "", visibilidade: VisibilidadeParaCTe };
    var emitirDesacordo = { descricao: "Emitir Desacordo", id: guid(), evento: "onclick", metodo: EmitirDesacordo_Menu, tamanho: "20", icone: "", visibilidade: VisibilidadeParaCTe };

    var solicitarArquivoTxtFTP = { descricao: "Solicitar Arquivo TXT ao FTP", id: guid(), evento: "onclick", metodo: SolicitarArquivoTxtFTP, tamanho: "20", icone: "", visibilidade: VisibilidadeParaTXT };
    var downloadArquivoTXT = { descricao: "Download Arquivo TXT", id: guid(), evento: "onclick", metodo: DownloadTXTDocumento, tamanho: "20", icone: "", visibilidade: VisibilidadeParaTXT };
    var enviarXMLParaFTP = { descricao: "Enviar XML para o FTP", id: guid(), evento: "onclick", metodo: EnviarXMLParaFTP, tamanho: "20", icone: "", visibilidade: VisibilidadeParaTXT };

    var downloadXMLMDFe = { descricao: "Download XML MDF-e", id: guid(), evento: "onclick", metodo: DownloadXMLMDFe, tamanho: "20", icone: "", visibilidade: VisibilidadeParaMDFe };

    var reenviarParaSAP = { descricao: "Reenviar para SAP", id: guid(), evento: "onclick", metodo: reintegrarDocumento, tamanho: "20", icone: "", visibilidade: visibilidadeIntegracaoSAP };
    var historicoIntegracaoSAP = { descricao: "Histórico integração SAP", id: guid(), evento: "onclick", metodo: exibirHistoricoIntegracaoSAPClick, tamanho: "20", icone: "", visibilidade: visibilidadeIntegracaoSAP };
    var reenviarParaSAP = { descricao: "Reenviar para SAP", id: guid(), evento: "onclick", metodo: reintegrarDocumento, tamanho: "20", icone: "", visibilidade: visibilidadeIntegracaoSAP };
    var ConsultarStatusSefaz = { descricao: "Consultar situação documento", id: guid(), evento: "onclick", metodo: executarConsultaPorChaveTabela, tamanho: "20", icone: "", visibilidade: visibilidadeIntegracaoSAP };

    let downloadXMLCCe = { descricao: "Download XML CC-e", id: guid(), evento: "onclick", metodo: downloadXMLCCeClick, tamanho: "20", icone: "", visibilidade: visibilidadeDownloadXMLCCe };
    let downloadDACCe = { descricao: "Download DACCe", id: guid(), evento: "onclick", metodo: downloadDACCeClick, tamanho: "20", icone: "", visibilidade: visibilidadeDownloadDACCe };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadXMLNFe, downloadXMLCTe, downloadDANFENFe, emitirManifestacao, emitirDesacordo, gerarDocumentoEntrada, visualizarManifestacoes, auditar, solicitarArquivoTxtFTP, downloadArquivoTXT, downloadXMLMDFe, vincularDocumentoEntrada, reenviarParaSAP, historicoIntegracaoSAP, ConsultarStatusSefaz, auditoriaOrdemServico, downloadXMLCCe, downloadDACCe],
        tamanho: 7
    };

    var configExportacao = {
        url: "DocumentoDestinadoEmpresa/ExportarPesquisa",
        titulo: "Documentos Destinados"
    };

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            SelecionarDocumentos();
        },
        callbackNaoSelecionado: function () {
            SelecionarDocumentos();
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaDocumentoDestinadoEmpresa.SelecionarTodos,
        somenteLeitura: false
    };

    _gridDocumentoDestinadoEmpresa = new GridViewExportacao("grid-documento-destino-empresa", "DocumentoDestinadoEmpresa/Pesquisa", _pesquisaDocumentoDestinadoEmpresa, menuOpcoes, configExportacao, { column: 5, dir: orderDir.desc }, 10, multiplaescolha);
    _gridDocumentoDestinadoEmpresa.SetPermitirEdicaoColunas(true);
    _gridDocumentoDestinadoEmpresa.SetSalvarPreferenciasGrid(true);
    _gridDocumentoDestinadoEmpresa.CarregarGrid();
}

function ObterConfiguracaoGeral() {
    executarReST("DocumentoDestinadoEmpresa/ObterConfiguracaoGeral", {}, function (r) {
        if (r.Success && r.Data) {
            _configuracaoGeral = r.Data;
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
    });
}


function SelecionarDocumentos() {
    CarregarDocumentos();
}


function CarregarDocumentos() {
    var titulosSelecionados = null;

    if (_pesquisaDocumentoDestinadoEmpresa.SelecionarTodos.val())
        titulosSelecionados = _gridDocumentoDestinadoEmpresa.ObterMultiplosNaoSelecionados();
    else
        titulosSelecionados = _gridDocumentoDestinadoEmpresa.ObterMultiplosSelecionados();

    var codigosTitulos = new Array();

    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _pesquisaDocumentoDestinadoEmpresa.SelecionarTodos.val()))
        _pesquisaDocumentoDestinadoEmpresa.ListaDocumentos.val(JSON.stringify(codigosTitulos));
    else
        _pesquisaDocumentoDestinadoEmpresa.ListaDocumentos.val("");
}

function AbrirModalEmissaoDesacordo(documento) {
    _eventoDesacordo.Codigo.val(documento.Codigo);
    _modalEmissaoDesacordo.show();
}