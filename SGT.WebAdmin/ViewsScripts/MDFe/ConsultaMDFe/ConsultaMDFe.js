/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Enumeradores/EnumSituacaoMDFe.js" />
/// <reference path="../../Consultas/Localidade.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _statusMDFe = [
    { text: "Todas", value: "" },
    { text: "Autorizado", value: EnumSituacaoMDFe.Autorizado },
    { text: "Rejeitado", value: EnumSituacaoMDFe.Rejeicao },
    { text: "Cancelado", value: EnumSituacaoMDFe.Cancelado },
    { text: "Encerrado", value: EnumSituacaoMDFe.Encerrado },
    { text: "Em Encerramento", value: EnumSituacaoMDFe.EmEncerramento },
    { text: "Em Cancelamento", value: EnumSituacaoMDFe.EmCancelamento },
    { text: "Em Digitação", value: EnumSituacaoMDFe.EmDigitacao },
    { text: "Enviado", value: EnumSituacaoMDFe.Enviado },
    { text: "Pendente", value: EnumSituacaoMDFe.Pendente },
    { text: "Ag. Inclusão de Motorista", value: EnumSituacaoMDFe.EventoInclusaoMotoristaEnviado },
    { text: "Emitido em Contingência", value: EnumSituacaoMDFe.EmitidoContingencia },
];

var _gridConsultaMDFe;
var _pesquisaMDFe;

var ConsultaMDFe = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.Serie = PropertyEntity({ text: "Número de Série:", getType: typesKnockout.int });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Status = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: _statusMDFe, def: "" });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.ChaveCTe = PropertyEntity({ text: "Chave do CT-e:", maxlength: 44 });
    this.Placa = PropertyEntity({ text: "Placa:", maxlength: 7 });
    this.CargaCodigo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });

    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.DownloadLoteXML = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteXMLClick();
        }, type: types.event, text: "Baixar Lote de XML", idFade: guid(), visible: ko.observable(true)
    });

    this.DownloadLoteDAMDFE = PropertyEntity({
        eventClick: function (e) {
            DownloadLoteDAMDFEClick();
        }, type: types.event, text: "Baixar Lote de DAMDFE", idFade: guid(), visible: ko.observable(true)
    });

    this.EmitirLoteMDFeContigenciado = PropertyEntity({
        eventClick: function () {
            EmitirLoteMDFeContigenciadoClick();
        }, type: types.event, text: "Autorizar todos emitidos em Contingência", visible: ko.observable(false)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaMDFe.CarregarGrid();
            this.EmitirLoteMDFeContigenciado.visible(this.Status.val() == EnumSituacaoMDFe.EmitidoContingencia)
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
};

var EncerramentoMDFe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Localidade = PropertyEntity({ val: ko.observable("0"), options: ko.observableArray(), def: "0", text: "*Municipio:", idBtnSearch: guid() });
    this.Estado = PropertyEntity({ type: types.local, enable: ko.observable(false), text: "Estado:" });
    this.DataEncerramento = PropertyEntity({ getType: typesKnockout.date, text: "*Data de Encerramento:", required: true, idBtnSearch: guid() });
    this.HoraEncerramento = PropertyEntity({ getType: typesKnockout.time, text: "*Hora de Encerramento:", required: true, idBtnSearch: guid() });
    this.EncerrarMDFe = PropertyEntity({ eventClick: EncerrarMDFeClick, enable: ko.observable(false), type: types.event, text: "Encerrar MDFe", visible: ko.observable(true) });
}

var EncerramentoManualMDFe = function () {
    this.Chave = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.string, text: "Chave:" });
    this.Protocolo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: "Protocolo:" });
    this.DataEncerramento = PropertyEntity({ getType: typesKnockout.dateTime, text: "Data de Encerramento:", required: true, idBtnSearch: guid() });
    this.LocalidadeManualMdfe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Municipio:", idBtnSearch: guid() });
    this.EmpresaManualMdfe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid() });
    this.EncerrarManualMDFe = PropertyEntity({ eventClick: EncerrarManualMDFeClick, enable: ko.observable(false), type: types.event, text: "Encerrar Manual MDFe", visible: ko.observable(true) });
}


//*******EVENTOS*******

function LoadConsultaMDFe() {

    _pesquisaMDFe = new ConsultaMDFe();
    KoBindings(_pesquisaMDFe, "knockoutConsultaMDFe", false, _pesquisaMDFe.Pesquisar.id);

    _encerramentoMDFe = new EncerramentoMDFe();
    KoBindings(_encerramentoMDFe, "knockoutEncerramentoMDFe");

    _encerramentoManualMDFe = new EncerramentoManualMDFe();
    KoBindings(_encerramentoManualMDFe, "knockoutEncerramentoManualMDFe");


    new BuscarCargas(_pesquisaMDFe.CargaCodigo);
    new BuscarEstados(_pesquisaMDFe.EstadoOrigem);
    new BuscarEstados(_pesquisaMDFe.EstadoDestino);
    new BuscarTransportadores(_pesquisaMDFe.Empresa);
    new BuscarClientes(_pesquisaMDFe.Remetente);
    new BuscarLocalidades(_encerramentoManualMDFe.LocalidadeManualMdfe);
    new BuscarTransportadores(_encerramentoManualMDFe.EmpresaManualMdfe);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaMDFe.Remetente.visible(true);
        _pesquisaMDFe.Empresa.visible(true);
    }

    LoadInclusaoMotoristaMDFe();

    BuscarMDFes();
}

function DownloadLoteXMLClick() {
    let data = RetornarObjetoPesquisa(_pesquisaMDFe);
    executarDownload("ConsultaMDFe/DownloadLoteXML", data);
}

function DownloadLoteDAMDFEClick() {
    let data = RetornarObjetoPesquisa(_pesquisaMDFe);
    executarDownload("ConsultaMDFe/DownloadLoteDAMDFE", data);
}

function BaixarXMLClick(e) {
    let data = { MDFe: e.Codigo };
    executarDownload("ConsultaMDFe/DownloadXML", data);
}

function BaixarXMLEncerramentoClick(e) {
    let data = { MDFe: e.Codigo };
    executarDownload("ConsultaMDFe/DownloadXMLEncerramento", data);
}

function EmitirMDFeContigenciadoClick(e) {
    let data = { CodigoMDFE: e.Codigo };
    executarReST("CargaMDFe/EmitirNovamente", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "MDF-e emitido com sucesso!");
                _gridConsultaMDFe.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function EmitirLoteMDFeContigenciadoClick() {
    var data = RetornarObjetoPesquisa(_pesquisaMDFe);
    executarReST("ConsultaMDFe/EmitirLoteMDFesContingenciados", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
                _gridConsultaMDFe.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function BaixarDAMDFEClick(e) {
    let data = { MDFe: e.Codigo };
    executarDownload("ConsultaMDFe/DownloadDAMDFE", data);
}

function AbrirEncerramentoMDFeClick(e) {

    let dados = { Codigo: e.Codigo };

    if (e.Status == EnumSituacaoMDFe.Autorizado) {
        executarReST("ConsultaMDFe/BuscarDadosParaEncerramentoPorCodigo", dados, function (arg) {
            if (arg.Success) {
                var dadosEncerramento = arg.Data;

                _encerramentoMDFe.Codigo.val(dadosEncerramento.Codigo);

                _encerramentoMDFe.Localidade.options(dadosEncerramento.Localidades);

                _encerramentoMDFe.DataEncerramento.val(dadosEncerramento.DataEncerramento);
                _encerramentoMDFe.HoraEncerramento.val(dadosEncerramento.HoraEncerramento);

                _encerramentoMDFe.Estado.val(dadosEncerramento.Estado);
                _encerramentoMDFe.Localidade.val(dadosEncerramento.Localidades[0].Codigo);

                Global.abrirModal('knockoutEncerramentoMDFe');
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "A atual situação do MDF-e não permite o seu encerramento.");
    }
}


function AbrirEncerramentoManualMDFeClick() {

    Global.abrirModal('knockoutEncerramentoManualMDFe');

}

function EncerrarMDFeClick(e, sender) {

    if (ValidarCamposObrigatorios(e)) {
        var dados = {
            Codigo: e.Codigo.val(),
            Localidade: e.Localidade.val(),
            DataEncerramento: e.DataEncerramento.val(),
            HoraEncerramento: e.HoraEncerramento.val()
        }
        executarReST("ConsultaMDFe/EncerrarMDFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "A solicitação de encerramendo do MDF-e foi enviada com sucesso.");
                    _gridConsultaMDFe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                Global.fecharModal('knockoutEncerramentoMDFe');
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "Informe os campos obrigatórios!");
    }
}

function EncerrarManualMDFeClick(e, sender) {

    if (ValidarCamposObrigatorios(e)) {
        var dados = {
            Chave: e.Chave.val(),
            Protocolo: e.Protocolo.val(),
            DataEncerramento: e.DataEncerramento.val(),
            EmpresaManualMdfe: e.EmpresaManualMdfe.codEntity(),
            LocalidadeManualMdfe: e.LocalidadeManualMdfe.codEntity()
        }
        executarReST("ConsultaMDFe/EncerrarManualMDFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "A solicitação de encerramendo manual do MDF-e foi enviada com sucesso.");
                    _gridConsultaMDFe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                Global.fecharModal('knockoutEncerramentoManualMDFe');
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "Informe os campos obrigatórios!");
    }
}

function SincronizarMDFeClick(e) {
    if (!(e.Status == EnumSituacaoMDFe.Enviado || e.Status == EnumSituacaoMDFe.EmCancelamento || e.Status == EnumSituacaoMDFe.EmEncerramento || e.Status == EnumSituacaoMDFe.EventoInclusaoMotoristaEnviado)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Não é possivel sincronizar o documento na situação atual.");
        return;
    }

    var data = { CodigoMDFe: e.Codigo, CodigoEmpresa: e.CodigoEmpresa };
    executarReST("CargaMDFe/SincronizarDocumentoEmProcessamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridConsultaMDFe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

//*******MÉTODOS*******


function BuscarMDFes() {
    let EmitirMDfeContingenciado = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: EmitirMDFeContigenciadoClick, tamanho: "20", icone: "", visibilidade: VisibilidadeMDFeContingencia };
    let downloadXML = { descricao: "Download XML", id: guid(), evento: "onclick", metodo: BaixarXMLClick, tamanho: "20", icone: "" };
    let downloadXMLEncerramento = { descricao: "Download XML Encerramento", id: guid(), evento: "onclick", metodo: BaixarXMLEncerramentoClick, tamanho: "20", icone: "", visibilidade: VisibilidadeMDFeEncerrado };
    let downloadDAMDFE = { descricao: "Download DAMDFE", id: guid(), evento: "onclick", metodo: BaixarDAMDFEClick, tamanho: "20", icone: "" };
    let encerrarMDFe = { descricao: "Encerrar", id: guid(), evento: "onclick", metodo: AbrirEncerramentoMDFeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeEncerramentoMDFe };
    let incluirMotoristaMDFe = { descricao: "Adicionar Motorista", id: guid(), evento: "onclick", metodo: AbrirInclusaoMotoristaMDFeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeEncerramentoMDFe };
    let sincronizarMDFe = { descricao: "Sincronizar Documento", id: guid(), evento: "onclick", metodo: SincronizarMDFeClick, tamanho: "20", icone: "", visibilidade: VisibilidadeSincronizarMDFe };
    let auditar = { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("ManifestoEletronicoDeDocumentosFiscais"), icone: "", visibilidade: VisibilidadeOpcaoAuditoriaMDFe };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [EmitirMDfeContingenciado, downloadXML, downloadXMLEncerramento, downloadDAMDFE, encerrarMDFe, incluirMotoristaMDFe, sincronizarMDFe, auditar],
        tamanho: 7
    };

    _gridConsultaMDFe = new GridView(_pesquisaMDFe.Pesquisar.idGrid, "ConsultaMDFe/Pesquisa", _pesquisaMDFe, menuOpcoes, { column: 0, dir: orderDir.desc }, 10);
    _gridConsultaMDFe.CarregarGrid();
}


function VisibilidadeEncerramentoMDFe(row) {
    //if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS)
    //    return false;

    return (row.Status === EnumSituacaoMDFe.Autorizado);
}

function VisibilidadeOpcaoAuditoriaMDFe() {
    return PermiteAuditar();
}

function VisibilidadeMDFeEncerrado(row) {
    return (row.Status === EnumSituacaoMDFe.Encerrado);
}

function VisibilidadeMDFeContingencia(row) {
    return (row.Status === EnumSituacaoMDFe.EmitidoContingencia
        || row.Status === EnumSituacaoMDFe.Rejeicao
        || row.Status === EnumSituacaoMDFe.Pendente);
}

function VisibilidadeSincronizarMDFe(row) {
    return (row.Status === EnumSituacaoMDFe.Enviado
        || row.Status === EnumSituacaoMDFe.EmCancelamento
        || row.Status === EnumSituacaoMDFe.EmEncerramento
        || row.Status === EnumSituacaoMDFe.EventoInclusaoMotoristaEnviado
    );
}