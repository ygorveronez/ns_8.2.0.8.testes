/// <reference path="../../Consultas/TipoTerminalImportacao.js" />
/// <reference path="../../Consultas/PedidoViagemNavio.js" />
/// <reference path="../../Consultas/Conhecimento.js" />
/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusCTeSemRejeicao = [
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Anulado Gerencialmente", value: EnumStatusCTe.ANULADO },
    { text: "Denegado", value: EnumStatusCTe.DENEGADO },
    { text: "Inutilizado", value: EnumStatusCTe.INUTILIZADO }
];

var _statusCTeComRejeicao = [
    { text: "Autorizado", value: EnumStatusCTe.AUTORIZADO },
    { text: "Cancelado", value: EnumStatusCTe.CANCELADO },
    { text: "Anulado Gerencialmente", value: EnumStatusCTe.ANULADO },
    { text: "Denegado", value: EnumStatusCTe.DENEGADO },
    { text: "Inutilizado", value: EnumStatusCTe.INUTILIZADO },
    { text: "Rejeitado", value: EnumStatusCTe.REJEICAO }
];

var _statusCTe = _statusCTeSemRejeicao;

var _situacaoVinculoCargaCTe = [
    { text: "Todas", value: "" },
    { text: "Com Carga", value: true },
    { text: "Sem Carga", value: false }
];

var _tipoDocumentoRegularManual = [
    { text: "Todos", value: "" },
    { text: "Regular", value: true },
    { text: "Manual ", value: false }
];

var _gridConsultaCTe;
var _pesquisaCTe;
var _envioEmailCTe;
var _modalEnvioEmail;

var EnvioEmailCTe = function () {
    this.Emails = PropertyEntity({ text: "*E-mails:", required: true, val: ko.observable("") });
    this.CTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnviarEmLote = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Enviar = PropertyEntity({
        eventClick: function (e) {
            EnviarPorEmailClick();
        }, type: types.event, text: "Enviar", idGrid: guid(), icon: "fal fa-send"
    });
};

var ConsultaCTe = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.NumeroSerie = PropertyEntity({ text: "Número de Série:", getType: typesKnockout.int });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", issue: 16, idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", issue: 58, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Status = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Situação do CT-e:", options: _statusCTe });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Fatura = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.VinculoCarga = PropertyEntity({ text: "Vínculo à carga:", val: ko.observable(""), options: _situacaoVinculoCargaCTe, def: "" });
    this.Placa = PropertyEntity({ text: "Placa:", maxlength: 7 });
    this.TipoCTe = PropertyEntity({ val: ko.observable(EnumTipoCTe.Todos), def: EnumTipoCTe.Todos, text: "Tipo CT-e:", options: EnumTipoCTe.ObterOpcoesPesquisa() });
    this.CTe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    //Emissão Multimodal
    this.NumeroBooking = PropertyEntity({ text: "Número Booking:" });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });
    this.NumeroControleCliente = PropertyEntity({ text: "Número Controle Cliente:" });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:" });
    this.TipoProposta = PropertyEntity({ text: "Tipo Proposta:", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumTipoPropostaMultimodal.obterOpcoes(), def: [], visible: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(""), def: "", text: "Tipo Documento:", options: _tipoDocumentoRegularManual });
    this.TipoServico = PropertyEntity({ text: "Tipo Serviço CT-e:", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumTipoServicoCTe.obterOpcoes(), def: [], visible: ko.observable(true) });
    this.TipoServicoCarga = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Todos), def: EnumTipoServicoMultimodal.Todos, text: "Tipo Serviço Multimodal:", options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), getType: typesKnockout.selectMultiple });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Origem:", idBtnSearch: guid() });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terminal Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
    this.ViagemTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem Transbordo:", idBtnSearch: guid() });
    this.PortoTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Transbordo:", idBtnSearch: guid() });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.VeioPorImportacao = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, text: "Veio por importação:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa() });
    this.CTeSubstituido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Somente CT-e Substituído" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaCTe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
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
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Emails = PropertyEntity({ text: "E-mails:", required: false, val: ko.observable("") });
    this.EnviarEmail = PropertyEntity({
        eventClick: function (e) {
            EnviarEmailClick();
        }, type: types.event, text: "Enviar E-mail em Lote", idFade: guid(), visible: ko.observable(true)
    });

    this.DownloadLoteXML = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteXMLClick();
        }, type: types.event, text: "Baixar Lote de XML", idFade: guid(), visible: ko.observable(true)
    });

    this.DownloadLoteDACTE = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteDACTEClick();
        }, type: types.event, text: "Baixar Lote de DACTE", idFade: guid(), visible: ko.observable(true)
    });

    this.DownloadCONEMB = PropertyEntity({
        eventClick: function (e) {
            AbrirTelaDownloadCONEMB();
        }, type: types.event, text: "Baixar CONEMB", idFade: guid(), visible: ko.observable(false)
    });

    this.DownloadOCOREN = PropertyEntity({
        eventClick: function (e) {
            AbrirTelaDownloadOCOREN();
        }, type: types.event, text: "Baixar OCOREN", idFade: guid(), visible: ko.observable(false)
    });

    this.EmitirCTe = PropertyEntity({
        eventClick: function (e) {
            EmitirCTeClick();
        }, type: types.event, text: "Baixar OCOREN", idFade: guid(), visible: ko.observable(false)
    });
};

//*******EVENTOS*******

function LoadConsultaCTe() {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
    {
        _statusCTe = _statusCTeComRejeicao;
    }
    console.log('status', _statusCTe);

    _pesquisaCTe = new ConsultaCTe();
    KoBindings(_pesquisaCTe, "knockoutConsultaCTe", false, _pesquisaCTe.Pesquisar.id);

    _envioEmailCTe = new EnvioEmailCTe();
    KoBindings(_envioEmailCTe, "divModalEnvioEmail");

    new BuscarClientes(_pesquisaCTe.Remetente);
    new BuscarClientes(_pesquisaCTe.Destinatario);
    new BuscarClientes(_pesquisaCTe.Tomador);
    new BuscarLocalidadesBrasil(_pesquisaCTe.Origem);
    new BuscarLocalidadesBrasil(_pesquisaCTe.Destino);
    new BuscarGruposPessoas(_pesquisaCTe.GrupoPessoas);
    new BuscarCargas(_pesquisaCTe.Carga);
    new BuscarEmpresa(_pesquisaCTe.Empresa);
    new BuscarFatura(_pesquisaCTe.Fatura);
    new BuscarModeloDocumentoFiscal(_pesquisaCTe.ModeloDocumento, null, null, null, null, true);
    new BuscarTiposOperacao(_pesquisaCTe.TipoOperacao);
    new BuscarTransportadores(_pesquisaCTe.Transportador);
    new BuscarFilial(_pesquisaCTe.Filial);
    new BuscarTipoTerminalImportacao(_pesquisaCTe.TerminalOrigem);
    new BuscarTipoTerminalImportacao(_pesquisaCTe.TerminalDestino);
    new BuscarPedidoViagemNavio(_pesquisaCTe.Viagem);
    new BuscarConhecimentoNotaReferencia(_pesquisaCTe.CTe);
    new BuscarPorto(_pesquisaCTe.PortoOrigem);
    new BuscarPorto(_pesquisaCTe.PortoDestino);
    new BuscarPorto(_pesquisaCTe.PortoTransbordo);
    new BuscarPedidoViagemNavio(_pesquisaCTe.ViagemTransbordo);

    BuscarCTes();
    SetarVisibilidadeCampos();
    LoadDownloadCONEMB();
    LoadDownloadOCOREN();
    _modalEnvioEmail = new bootstrap.Modal(document.getElementById("divModalEnvioEmail"), { backdrop: true, keyboard: true });
}

function SetarVisibilidadeCampos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaCTe.GrupoPessoas.visible(true);
        _pesquisaCTe.Carga.visible(true);
        _pesquisaCTe.Empresa.visible(true);
        _pesquisaCTe.DownloadOCOREN.visible(true);
        _pesquisaCTe.DownloadCONEMB.visible(true);
        _pesquisaCTe.Fatura.visible(true);
        _pesquisaCTe.ModeloDocumento.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaCTe.Filial.visible(true);
        _pesquisaCTe.Transportador.visible(true);
        _pesquisaCTe.Fatura.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaCTe.Carga.visible(true);
    }

    if (_CONFIGURACAO_TMS.PermitirBaixarArquivosConembOcorenManualmente) {
        _pesquisaCTe.DownloadOCOREN.visible(true);
        _pesquisaCTe.DownloadCONEMB.visible(true);
    }

    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#liFiltrosEmissaoMultimodal").hide();
}

function EnviarEmailClick() {
    LimparCampos(_envioEmailCTe);
    _envioEmailCTe.EnviarEmLote.val(true);
    _envioEmailCTe.CTe.val(0);
    _modalEnvioEmail.show();
}

function DownloadLoteXMLClick() {
    var data = RetornarObjetoPesquisa(_pesquisaCTe);
    executarDownload("ConsultaCTe/DownloadLoteXML", data);
}

function DownloadLoteDACTEClick() {
    var data = RetornarObjetoPesquisa(_pesquisaCTe);
    executarDownload("ConsultaCTe/DownloadLoteDACTE", data);
}

function BaixarXMLCancelamentoClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("ConsultaCTe/DownloadXMLCancelamento", data);
}

function BaixarXMLInutilizacaoClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("ConsultaCTe/DownloadXMLInutilizacao", data);
}

function BaixarXMLClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("ConsultaCTe/DownloadXML", data);
}

function BaixarDACTEClick(e) {
    var data = { CTe: e.Codigo };
    executarDownload("ConsultaCTe/DownloadDACTE", data);
}

function AbrirTelaEnvioEmailClick(e) {
    LimparCampos(_envioEmailCTe);
    _envioEmailCTe.EnviarEmLote.val(false);
    _envioEmailCTe.CTe.val(e.Codigo);
    _modalEnvioEmail.show();
}

function EmitirCTeClick(e) {
    if (e.SituacaoCTe == EnumStatusCTe.REJEICAO || e.SituacaoCTe == EnumStatusCTe.FSDA) {
        var data = { CodigoCTe: e.Codigo };
        executarReST("CargaCTe/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaCTe.CarregarGrid();
                    exibirReenvioCTe();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.SituacaoNaoPermiteEmissao, Localization.Resources.Cargas.Carga.AtualSituacaoDoCteNaoPermiteQueEleSejaEmitidoNovamente.format(e.Status));
    }
}

function EnviarPorEmailClick() {
    if (_envioEmailCTe.EnviarEmLote.val()) {
        _pesquisaCTe.Emails.val(_envioEmailCTe.Emails.val());
        var data = RetornarObjetoPesquisa(_pesquisaCTe);
        executarReST("ConsultaCTe/EnviarPorEmailLote", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "E-mail enviado com sucesso.");
                    _modalEnvioEmail.hide();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
    else {
        Salvar(_envioEmailCTe, "ConsultaCTe/EnviarPorEmail", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "E-mail enviado com sucesso.");
                    _modalEnvioEmail.hide();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

//*******MÉTODOS*******

function BuscarCTes() {

    var downloadXMLCancelamento = { descricao: "Download XML Cancelamento", id: guid(), evento: "onclick", metodo: BaixarXMLCancelamentoClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDownloadXMLCancelamento };
    var downloadXMLInutilizacao = { descricao: "Download XML Inutilização", id: guid(), evento: "onclick", metodo: BaixarXMLInutilizacaoClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDownloadXMLInutilizacao };
    var downloadXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: BaixarXMLClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDownloadXMLAutorizacao };
    var downloadDACTE = { descricao: "Download PDF", id: guid(), evento: "onclick", metodo: BaixarDACTEClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoDownloadXMLAutorizacao };
    var enviarEmail = { descricao: "Enviar por e-mail ", id: guid(), evento: "onclick", metodo: AbrirTelaEnvioEmailClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoEnvioEmail };
    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ConhecimentoDeTransporteEletronico", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var emitirAutorizar = { descricao: "Emitir", id: guid(), evento: "onclick", metodo: EmitirCTeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoEmissao };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadXML, downloadXMLCancelamento, downloadXMLInutilizacao, downloadDACTE, enviarEmail, auditar, emitirAutorizar],
        tamanho: 7
    };

    _gridConsultaCTe = new GridView(_pesquisaCTe.Pesquisar.idGrid, "ConsultaCTe/Pesquisa", _pesquisaCTe, menuOpcoes, { column: 3, dir: orderDir.desc }, 10);
    _gridConsultaCTe.CarregarGrid();
}

function VisibilidadeOpcaoDownloadXMLCancelamento(data) {
    if (data.SituacaoCTe == EnumStatusCTe.CANCELADO && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadXMLInutilizacao(data) {
    if (data.SituacaoCTe == EnumStatusCTe.INUTILIZADO && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadXMLAutorizacao(data) {
    if ((data.SituacaoCTe === EnumStatusCTe.AUTORIZADO || data.SituacaoCTe === EnumStatusCTe.CANCELADO || data.SituacaoCTe === EnumStatusCTe.ANULADO) && (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFS || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.Outros)) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoEnvioEmail(data) {
    if ((data.SituacaoCTe === EnumStatusCTe.AUTORIZADO || data.SituacaoCTe === EnumStatusCTe.CANCELADO || data.SituacaoCTe === EnumStatusCTe.ANULADO) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe) {
        return true;
    } else {
        return false;
    }
} 

function VisibilidadeOpcaoEmissao(data) {
    if (data.SituacaoCTe === EnumStatusCTe.REJEICAO) {
        return true;
    } else {
        return false;
    }
} 