/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Cargas.js" />
/// <reference path="CTes.js" />
/// <reference path="Etapas.js" />
/// <reference path="Impressao.js" />
/// <reference path="SignalR.js" />
/// <reference path="Terminais.js" />
/// <reference path="CargaMDFeAquaviarioManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaMDFe, _cargaMDFeAquaviarioMDFe, _encerramentoMDFe;

var _statusPesquisaMDFe = [
    { text: "Todos", value: EnumSituacaoMDFe.Todos },
    { text: "Autorizado", value: EnumSituacaoMDFe.Autorizado },
    { text: "Pendente", value: EnumSituacaoMDFe.Pendente },
    { text: "Enviado", value: EnumSituacaoMDFe.Enviado },
    { text: "Rejeitado", value: EnumSituacaoMDFe.Rejeicao },
    { text: "Cancelado", value: EnumSituacaoMDFe.Cancelado },
    { text: "Em Digitação", value: EnumSituacaoMDFe.EmDigitacao },
    { text: "Em Cancelamento", value: EnumSituacaoMDFe.EmCancelamento },
    { text: "Em Encerramento", value: EnumSituacaoMDFe.EmEncerramento },
    { text: "Encerrado", value: EnumSituacaoMDFe.Encerrado }
];

var CargaMDFeAquaviarioMDFe = function () {
    this.CargaMDFeAquaviario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable(EnumSituacaoMDFe.Todos), enable: ko.observable(true), visible: ko.observable(true), text: "Situação MDF-e: ", options: _statusPesquisaMDFe, def: EnumSituacaoMDFe.Todos });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaMDFe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.EmitirNovamenteRejeitados = PropertyEntity({
        eventClick: ReemitirMDFesRejeitadosClick, type: types.event, text: "Reemitir os MDF-es Rejeitados", visible: ko.observable(false), enable: ko.observable(false)
    });
    this.MDFe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), visible: ko.observable(false) });
}

var EncerramentoMDFe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Localidade = PropertyEntity({ val: ko.observable("0"), options: ko.observableArray(), def: "0", text: "*Municipio:", idBtnSearch: guid() });
    this.Estado = PropertyEntity({ type: types.local, enable: ko.observable(false), text: "Estado:" });
    this.DataEncerramento = PropertyEntity({ getType: typesKnockout.date, text: "*Data de Encerramento:", required: true, idBtnSearch: guid() });
    this.HoraEncerramento = PropertyEntity({ getType: typesKnockout.time, text: "*Hora de Encerramento:", required: true, idBtnSearch: guid() });
    this.EncerrarMDFe = PropertyEntity({ eventClick: EncerrarMDFeClick, enable: ko.observable(false), type: types.event, text: "Encerrar MDFe", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadMDFe() {
    _cargaMDFeAquaviarioMDFe = new CargaMDFeAquaviarioMDFe();
    KoBindings(_cargaMDFeAquaviarioMDFe, "knockoutMDFes");

    _encerramentoMDFe = new EncerramentoMDFe();
    KoBindings(_encerramentoMDFe, "divModalEncerramentoMDFe");

    $("#" + _cargaMDFeAquaviarioMDFe.Status.id).removeAttr("disabled");
    $("#" + _cargaMDFeAquaviarioMDFe.Pesquisar.id).removeAttr("disabled");
}

function buscarMDFeClick(e, sender) {
    _cargaMDFeAquaviarioMDFe.CargaMDFeAquaviario.val(_cargaMDFeAquaviario.Codigo.val());
    buscarCargaMDFe(function (mdfeGrid) {
        if (mdfeGrid != null && mdfeGrid.data.length > 0) {
            _cargaMDFeAquaviarioMDFe.MDFe.visible(true);
            _cargaMDFeAquaviarioMDFe.Status.visible(true);
            _cargaMDFeAquaviarioMDFe.Pesquisar.visible(true);
        } else {
            _cargaMDFeAquaviarioMDFe.MDFe.visible(false);
            _cargaMDFeAquaviarioMDFe.Status.visible(false);
            _cargaMDFeAquaviarioMDFe.Pesquisar.visible(false);
        }

        if (_cargaMDFeAquaviario.Situacao.val() == EnumSituacaoMDFeManual.Rejeicao) {
            _cargaMDFeAquaviarioMDFe.EmitirNovamenteRejeitados.visible(true);
        }
    });
}


function ReemitirMDFesRejeitadosClick(e) {
    var data = {
        CargaMDFeManual: _cargaMDFeAquaviario.Codigo.val()
    };
    executarReST("CargaMDFeManualMDFe/ReemitirMDFesRejeitados", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _cargaMDFeAquaviario.Situacao.val(EnumSituacaoMDFeManual.EmEmissao);
                _cargaMDFeAquaviarioMDFe.EmitirNovamenteRejeitados.visible(false);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Os MDF-es foram reemitidos com sucesso.");
                SetarEtapaMDFe();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
            _gridCargaMDFe.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal("divModalRetornoSefaz");    
}

function baixarXMLMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaMDFe/DownloadXMLAutorizacao", data);
}

function baixarDAMDFeClick(e) {
    var data = { CodigoMDFe: e.CodigoMDFE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaMDFe/DownloadDAMDFE", data);
}

function emitirMDFeClick(e) {
    var data = {
        CodigoMDFe: e.CodigoMDFE,
        CodigoEmpresa: e.CodigoEmpresa
    };
    executarReST("CargaMDFeManualMDFe/EmitirNovamente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirReenvioMDFe();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}


function AbrirModalEncerrarMDFeClick(e) {

    var dados = { Codigo: e.CodigoMDFE };

    if (e.Status == EnumSituacaoMDFe.Autorizado) {
        executarReST("CargaMDFeManualMDFe/BuscarDadosParaEncerramentoPorCodigo", dados, function (arg) {
            if (arg.Success) {
                var dadosEncerramento = arg.Data;

                _encerramentoMDFe.Codigo.val(dadosEncerramento.Codigo);

                _encerramentoMDFe.Localidade.options(dadosEncerramento.Localidades);

                _encerramentoMDFe.DataEncerramento.val(dadosEncerramento.DataEncerramento);
                _encerramentoMDFe.HoraEncerramento.val(dadosEncerramento.HoraEncerramento);

                _encerramentoMDFe.Estado.val(dadosEncerramento.Estado);
                _encerramentoMDFe.Localidade.val(dadosEncerramento.Localidades[0].Codigo);
                                
                Global.abrirModal("divModalEncerramentoMDFe");
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "A atual situação do MDF-e não permite o seu encerramento.");
    }
}

function EncerrarMDFeClick(e, sender) {

    if (ValidarCamposObrigatorios(e)) {
        var dados = {
            Codigo: e.Codigo.val(),
            CargaMDFeManual: _cargaMDFeAquaviario.Codigo.val(),
            Localidade: e.Localidade.val(),
            DataEncerramento: e.DataEncerramento.val(),
            HoraEncerramento: e.HoraEncerramento.val()
        }
        executarReST("CargaMDFeManualMDFe/EncerrarMDFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Solicitação Realizada", "A solicitação de encerramendo do MDF-e foi enviada com sucesso.");
                    _gridCargaMDFe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
                Global.fecharModal('divModalEncerramentoMDFe');
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe os campos obrigatórios");
    }
}

function exibirReenvioMDFe() {
    _cargaMDFeAquaviario.Situacao.val(EnumSituacaoMDFeManual.EmEmissao);
    _cargaMDFeAquaviarioMDFe.EmitirNovamenteRejeitados.visible(false);
    _gridCargaMDFe.CarregarGrid();
    SetarEtapaMDFe();
}

function buscarCargaMDFe(callback) {
    var retornoSefaz = { descricao: "Mensagem Sefaz", id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: visibilidadeRetornoSefaz };
    var baixarDAMDFE = { descricao: "Baixar DAMDFE", id: guid(), metodo: baixarDAMDFeClick, visibilidade: visibilidadeDAMDFE };
    var baixarXMLMDFe = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLMDFeClick, visibilidade: visibilidadeDAMDFE };
    var solicitarEncerramento = { descricao: "Encerrar", id: guid(), metodo: AbrirModalEncerrarMDFeClick, visibilidade: visibilidadeEncerramento };
    var emitir = { descricao: "Emitir", id: guid(), metodo: function (datagrid) { emitirMDFeClick(datagrid); }, icone: "", visibilidade: visibilidadeEmitir };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDAMDFE, baixarXMLMDFe, retornoSefaz, emitir, solicitarEncerramento] };

    _gridCargaMDFe = new GridView(_cargaMDFeAquaviarioMDFe.Pesquisar.idGrid, "CargaMDFeManualMDFe/ConsultarCargaMDFe", _cargaMDFeAquaviarioMDFe, menuOpcoes, null);
    _gridCargaMDFe.CarregarGrid(callback);
}

function visibilidadeDAMDFE(row) {
    if (row.Status == EnumSituacaoMDFe.Autorizado || row.Status == EnumSituacaoMDFe.Encerrado)
        return true;
    else
        return false;
}

function visibilidadeEncerramento(row) {
    if (row.Status == EnumSituacaoMDFe.Autorizado && _cargaMDFeAquaviario.Situacao.val() == EnumSituacaoMDFeManual.Finalizado)
        return true;
    else
        return false;
}

function visibilidadeRetornoSefaz(row) {
    if (row.Importado != 1)
        return true;
    else
        return false;
}

function visibilidadeEmitir(row) {
    if (row.Status == EnumSituacaoMDFe.Rejeicao || row.Status == EnumSituacaoMDFe.EmDigitacao)
        return true;
    else
        return false;
}
