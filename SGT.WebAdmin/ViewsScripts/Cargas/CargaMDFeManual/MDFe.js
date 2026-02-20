/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
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
/// <reference path="MDFe.js" />
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

var _gridCargaMDFe, _cargaMDFeManualMDFe, _encerramentoMDFe;

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

var CargaMDFeManualMDFe = function () {
    this.CargaMDFeManual = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
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
    _cargaMDFeManualMDFe = new CargaMDFeManualMDFe();
    KoBindings(_cargaMDFeManualMDFe, "knockoutMDFes");

    _encerramentoMDFe = new EncerramentoMDFe();
    KoBindings(_encerramentoMDFe, "divModalEncerramentoMDFe");

    $("#" + _cargaMDFeManualMDFe.Status.id).removeAttr("disabled");
    $("#" + _cargaMDFeManualMDFe.Pesquisar.id).removeAttr("disabled");
}

function buscarMDFeClick(e, sender) {
    _cargaMDFeManualMDFe.CargaMDFeManual.val(_cargaMDFeManual.Codigo.val());
    buscarCargaMDFe(function (mdfeGrid) {
        if (mdfeGrid != null && mdfeGrid.data.length > 0) {
            _cargaMDFeManualMDFe.MDFe.visible(true);
            _cargaMDFeManualMDFe.Status.visible(true);
            _cargaMDFeManualMDFe.Pesquisar.visible(true);
        } else {
            _cargaMDFeManualMDFe.MDFe.visible(false);
            _cargaMDFeManualMDFe.Status.visible(false);
            _cargaMDFeManualMDFe.Pesquisar.visible(false);
        }

        if (_cargaMDFeManual.Situacao.val() == EnumSituacaoMDFeManual.Rejeicao) {
            _cargaMDFeManualMDFe.EmitirNovamenteRejeitados.visible(true);
        }
    });
}


function ReemitirMDFesRejeitadosClick(e) {
    var data = {
        CargaMDFeManual: _cargaMDFeManual.Codigo.val(),
        Percurso: JSON.stringify(_mapaPercursoMDFe.GetEstadosPassagem())
    };
    executarReST("CargaMDFeManualMDFe/ReemitirMDFesRejeitados", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _cargaMDFeManual.Situacao.val(EnumSituacaoMDFeManual.EmEmissao);
                _cargaMDFeManualMDFe.EmitirNovamenteRejeitados.visible(false);
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
        CodigoEmpresa: e.CodigoEmpresa,
        Percurso: JSON.stringify(_mapaPercursoMDFe.GetEstadosPassagem())
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
            CargaMDFeManual: _cargaMDFeManual.Codigo.val(),
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
    _cargaMDFeManual.Situacao.val(EnumSituacaoMDFeManual.EmEmissao);
    _cargaMDFeManualMDFe.EmitirNovamenteRejeitados.visible(false);
    _gridCargaMDFe.CarregarGrid();
    SetarEtapaMDFe();
}

function buscarCargaMDFe(callback) {
    var retornoSefaz = { descricao: "Mensagem Sefaz", id: guid(), metodo: retoronoSefazClick, icone: "" };
    var baixarDAMDFE = { descricao: "Baixar DAMDFE", id: guid(), metodo: baixarDAMDFeClick, visibilidade: visibilidadeDAMDFE };
    var baixarXMLMDFe = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLMDFeClick, visibilidade: visibilidadeDAMDFE };
    var solicitarEncerramento = { descricao: "Encerrar", id: guid(), metodo: AbrirModalEncerrarMDFeClick, visibilidade: visibilidadeEncerramento };
    var emitir = { descricao: "Emitir", id: guid(), metodo: function (datagrid) { emitirMDFeClick(datagrid); }, icone: "", visibilidade: visibilidadeEmitir };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDAMDFE, baixarXMLMDFe, retornoSefaz, emitir, solicitarEncerramento] };

    _gridCargaMDFe = new GridView(_cargaMDFeManualMDFe.Pesquisar.idGrid, "CargaMDFeManualMDFe/ConsultarCargaMDFe", _cargaMDFeManualMDFe, menuOpcoes, null);
    _gridCargaMDFe.CarregarGrid(callback);
}

function visibilidadeDAMDFE(row) {
    if (row.Status == EnumSituacaoMDFe.Autorizado || row.Status == EnumSituacaoMDFe.Encerrado)
        return true;
    else
        return false;
}

function visibilidadeEncerramento(row) {
    if (row.Status == EnumSituacaoMDFe.Autorizado && _cargaMDFeManual.Situacao.val() == EnumSituacaoMDFeManual.Finalizado)
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
