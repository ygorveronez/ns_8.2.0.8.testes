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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="NFe.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="../../Cargas/Carga/Carga.js" />
/// <reference path="../ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridCreditoLiberacao;
var _pesquisaCreditoLiberacao;
var _creditoLiberacao;
var _CRUDcreditoLiberacao;
var _cteComplementar;
var _gridCTe;
var _ocorrencia;
var _gridCTeComplementar;

var _situacaoSolicitacaoCredito = [
    { text: "Todos", value: EnumSituacaoSolicitacaoCredito.Todos },
    { text: "Ag. Liberação", value: EnumSituacaoSolicitacaoCredito.AgLiberacao },
    { text: "Estornado", value: EnumSituacaoSolicitacaoCredito.Estornado },
    { text: "Liberado", value: EnumSituacaoSolicitacaoCredito.Liberado },
    { text: "Negado", value: EnumSituacaoSolicitacaoCredito.Negado },
    { text: "Rejeitado", value: EnumSituacaoSolicitacaoCredito.Rejeitado },
    { text: "Utilizado", value: EnumSituacaoSolicitacaoCredito.Utilizado },
];

var _responsavelOcorrencia = [
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
];

var PesquisaCreditoLiberacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCargaOcorrencia = PropertyEntity({ text: "Número da Ocorrência:", val: ko.observable(""), def: 0, getType: typesKnockout.int, visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante:", idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid() });

    this.SituacaoSolicitacaoCredito = PropertyEntity({ val: ko.observable(EnumSituacaoSolicitacaoCredito.AgLiberacao), options: _situacaoSolicitacaoCredito, def: EnumSituacaoSolicitacaoCredito.AgLiberacao, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCreditoLiberacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var CreditoLiberacao = function () {
    this.SituacaoSolicitacaoCredito = PropertyEntity({ val: ko.observable(EnumSituacaoSolicitacaoCredito.AgLiberacao), def: EnumSituacaoSolicitacaoCredito.AgLiberacao, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.dynamic });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Componente de Frete:", idBtnSearch: guid() });
    this.DataSolicitacao = PropertyEntity({ text: "Data da Solicitação: ", getType: typesKnockout.dateTime });
    this.DataRetorno = PropertyEntity({ text: "Data do Retorno: ", getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante:" });
    this.Creditor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Creditor:", visible: ko.observable(false) });
    this.MotivoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: "Motivo da solicitação:" });
    this.RetornoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: "Resposta:", enable: ko.observable(false) });
    this.Solicitado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitado:" });
    this.ValorSolicitado = PropertyEntity({ text: "Valor solicitado: ", getType: typesKnockout.decimal });
    this.ValorLiberado = PropertyEntity({ text: "Valor aprovado: ", getType: typesKnockout.decimal, enable: ko.observable(false), visible: ko.observable(true) });
    this.DescricaoSituacao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NumeroCTesCarga = PropertyEntity({ val: ko.observable(""), def: "" });
    this.NumeroOcorrencia = PropertyEntity({ text: "Número ocorrência: ", visible: ko.observable(false) });
    this.CreditosUtilizados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.CodigoCreditorSolicitar = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });

    this.Responsavel = PropertyEntity({ val: ko.observable(EnumResponsavelOcorrencia.Destinatario), options: _responsavelOcorrencia, def: EnumResponsavelOcorrencia.Destinatario, text: "Responsável: ", enable: ko.observable(false), visible: ko.observable(false) });
    this.CFOP = PropertyEntity({ type: types.map, required: false, maxlength: 6, text: "CFOP:", enable: ko.observable(false), visible: ko.observable(false) });
    this.ContaContabil = PropertyEntity({ type: types.map, required: false, maxlength: 6, text: "Conta Contábil:", enable: ko.observable(false), visible: ko.observable(false) });

    this.Remetente = PropertyEntity({ text: "Remetente: ", visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ text: "Destinatario: ", visible: ko.observable(false) });

    this.CodigoOcorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SolicitacaoTerceiro = PropertyEntity({ val: ko.observable(0), def: false, getType: typesKnockout.bool });
    this.SolicitacaoTransportador = PropertyEntity({ val: ko.observable(0), def: false, getType: typesKnockout.bool });
    this.ParametroPeriodoCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ParametroPeriodoHoras = PropertyEntity({ text: "Quantidade de Horas: ", getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.ParametroDataInicio = PropertyEntity({ text: "*Data Início: ", getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.ParametroDataFim = PropertyEntity({ text: "*Data Fim: ", getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.ParametroBooleanoCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ParametroApenasReboque = PropertyEntity({ text: "Apenas Reboque", getType: typesKnockout.bool, visible: ko.observable(false) });

    this.ParametroDomingo = PropertyEntity({ text: "Domingo:", getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ParametroData1Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ParametroData2Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ParametroTextoCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ParametroTextoDescricao = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string });
    this.ParametroData1Descricao = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string });
    this.ParametroData2Descricao = PropertyEntity({ val: ko.observable(""), def: 0, getType: typesKnockout.string });
    this.ParametroData1 = PropertyEntity({ text: ko.observable("Data 1:"), getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.ParametroData2 = PropertyEntity({ text: ko.observable("Data 2:"), getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.ParametroTexto = PropertyEntity({ text: ko.observable("Texto:"), getType: typesKnockout.string, visible: ko.observable(false) });

    this.ArquivoOcorrencia = PropertyEntity({ val: ko.observable(0), def: "", getType: typesKnockout.string });

    this.DownloadArquivo = PropertyEntity({ eventClick: gerenciarAnexosClick, type: types.event, text: "Download Anexo", visible: ko.observable(true), enable: ko.observable(true) });

}

var Ocorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}


var CRUDCreditoLiberacao = function () {
    this.RejeitarSolicitacao = PropertyEntity({ type: types.event, eventClick: rejeitarSolicitacaoClick, text: "Rejeitar Solicitação", visible: ko.observable(true), enable: ko.observable(true) });
    this.ResponderSolicitacao = PropertyEntity({ type: types.event, eventClick: responderSolicitacaoClick, text: "Aprovar Solicitação", visible: ko.observable(true), enable: ko.observable(true) });
}

var CTeComplementar = function () {
    this.CTesComplementares = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.BuscarNovamenteCTesComplementares = PropertyEntity({ eventClick: atualizarCTesComplementaresClick, type: types.event, text: "Buscar / Atualizar CT-e(s)", visible: ko.observable(true) });
}


//*******EVENTOS*******

function rejeitarSolicitacaoClick(e) {

    exibirConfirmacao("Confirmação", "Realmente deseja rejeitar essa solicitação de crédito?", function () {
        if (_creditoLiberacao.RetornoSolicitacao.val() != "") {
            _creditoLiberacao.SituacaoSolicitacaoCredito.val(EnumSituacaoSolicitacaoCredito.Rejeitado);
            AtualizarSolicitacao();
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por favor, informe o motivo da rejeição do crédito solicitado");
            resetarTabs();
        }
    });
}

function responderSolicitacaoClick(e) {
    var valido = true;
    if (_creditoLiberacao.SolicitacaoTerceiro.val() == true) {
        //if (_creditoLiberacao.CFOP.val() == null || _creditoLiberacao.CFOP.val() == "") {
        //    exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatorio informar uma CFOP");
        //    valido = false;
        //}
        //if (_creditoLiberacao.ContaContabil.val() == null || _creditoLiberacao.ContaContabil.val() == "") {
        //    exibirMensagem(tipoMensagem.atencao, "Atenção", "É obrigatorio informar uma Conta Contábil");
        //    valido = false;
        //}
    }

    if (valido) {
        exibirConfirmacao("Confirmação", "Realmente deseja responder essa solicitação de crédito?", function () {
            if (_creditoLiberacao.ValorLiberado.val() != "") {
                if (Globalize.parseFloat(_creditoLiberacao.ValorLiberado.val()) <= Globalize.parseFloat(_creditoLiberacao.ValorSolicitado.val())) {

                    ValidarUtilizacaoSaldo(_creditoLiberacao.ValorLiberado.val(), function (creditosUtilizados, codigoCreditorSolicitar) {
                        if (creditosUtilizados != null)
                            _creditoLiberacao.CreditosUtilizados.val(JSON.stringify(creditosUtilizados));

                        if (codigoCreditorSolicitar != null)
                            _creditoLiberacao.CodigoCreditorSolicitar.val(codigoCreditorSolicitar);


                        _creditoLiberacao.SituacaoSolicitacaoCredito.val(EnumSituacaoSolicitacaoCredito.Liberado);
                        AtualizarSolicitacao();

                    });


                } else {
                    exibirMensagem(tipoMensagem.atencao, "Valor Superior", "O valor liberado não pode ser superior ao solicitado");
                    _creditoLiberacao.ValorLiberado.val(_creditoLiberacao.ValorSolicitado.val());
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por favor, informe o valor liberado");
                resetarTabs();
            }
        });
    }
}

function loadCreditoLiberacao() {
    buscarDetalhesOperador(function () {
        _creditoLiberacao = new CreditoLiberacao();
        _pesquisaCreditoLiberacao = new PesquisaCreditoLiberacao();
        KoBindings(_pesquisaCreditoLiberacao, "knockoutPesquisaCreditoLiberacao", false, _pesquisaCreditoLiberacao.Pesquisar.id);

        if (_notificacaoGlobal.CodigoObjeto.val() > 0) {
            _pesquisaCreditoLiberacao.Codigo.val(_notificacaoGlobal.CodigoObjeto.val());
            _notificacaoGlobal.CodigoObjeto.val(0);
        }

        _creditoLiberacao = new CreditoLiberacao();
        KoBindings(_creditoLiberacao, "knockoutCreditoLiberado");

        _cteComplementar = new CTeComplementar();
        KoBindings(_cteComplementar, "knockoutCTeComplementar");

        new BuscarOperador(_pesquisaCreditoLiberacao.Recebedor);
        _CRUDcreditoLiberacao = new CRUDCreditoLiberacao()
        KoBindings(_CRUDcreditoLiberacao, "knockoutCRUDCreditoLiberado");
        carregarConteudosHTML(function () {
            buscarCreditoLiberacaos();
            loadControleSaldo();
        });

        new BuscarUsuarioTerceiro(_pesquisaCreditoLiberacao.Solicitante);

        new BuscarTipoOcorrencia(_pesquisaCreditoLiberacao.Ocorrencia);
        carregarGridCTeComplementares();
    });

    loadAnexos();
}

function editarCreditoLiberacao(e) {
    $("#wid-id-4").show();
    limparCamposCreditoLiberacao(_creditoLiberacao);
    _creditoLiberacao.Codigo.val(e.Codigo);
    BuscarPorCodigo(_creditoLiberacao, "CreditoLiberacao/BuscarPorCodigo", function (arg) {
        // Aneoxs da ocorrencia
        CarregarAnexos(_creditoLiberacao.CodigoOcorrencia.val());

        _pesquisaCreditoLiberacao.ExibirFiltros.visibleFade(false);
        if (_creditoLiberacao.NumeroOcorrencia.val() > 0)
            _creditoLiberacao.NumeroOcorrencia.visible(true);
        else
            _creditoLiberacao.NumeroOcorrencia.visible(false);

        if (_creditoLiberacao.SituacaoSolicitacaoCredito.val() == EnumSituacaoSolicitacaoCredito.AgLiberacao) {
            _creditoLiberacao.RetornoSolicitacao.enable(true);
            _creditoLiberacao.ValorLiberado.enable(true);
            _CRUDcreditoLiberacao.RejeitarSolicitacao.enable(true);
            _CRUDcreditoLiberacao.ResponderSolicitacao.enable(true);
            _creditoLiberacao.ValorLiberado.val(_creditoLiberacao.ValorSolicitado.val());
        } else {
            _creditoLiberacao.DataRetorno.visible(true);
            _creditoLiberacao.Creditor.visible(true);
        }

        if (_creditoLiberacao.SolicitacaoTerceiro.val() == true || _creditoLiberacao.SolicitacaoTransportador.val() == true) {
            $("#liCTeComplementar").show();
            $("#liCarga").hide();

            _creditoLiberacao.ValorLiberado.visible(false);
            _creditoLiberacao.Remetente.visible(true);
            _creditoLiberacao.Destinatario.visible(true);

            if (_creditoLiberacao.SolicitacaoTerceiro.val() == true) {
                _creditoLiberacao.Responsavel.visible(true);
                _creditoLiberacao.ContaContabil.visible(false);
                if (_creditoLiberacao.CFOP.val() != "")
                    _creditoLiberacao.CFOP.visible(false);
                else
                    _creditoLiberacao.CFOP.visible(true);

                if (_creditoLiberacao.SituacaoSolicitacaoCredito.val() == EnumSituacaoSolicitacaoCredito.AgLiberacao) {
                    _creditoLiberacao.Responsavel.enable(true);
                    _creditoLiberacao.CFOP.enable(true);
                    _creditoLiberacao.ContaContabil.enable(true);
                }
                else {
                    _creditoLiberacao.Responsavel.enable(false);
                    _creditoLiberacao.CFOP.enable(false);
                    _creditoLiberacao.ContaContabil.enable(false);
                }

                if (_creditoLiberacao.ParametroPeriodoCodigo.val() > 0) {
                    _creditoLiberacao.ParametroDataInicio.visible(true);
                    _creditoLiberacao.ParametroDataFim.visible(true);
                    _creditoLiberacao.ParametroPeriodoHoras.visible(true);
                }
                else {
                    _creditoLiberacao.ParametroDataInicio.visible(false);
                    _creditoLiberacao.ParametroDataFim.visible(false);
                    _creditoLiberacao.ParametroPeriodoHoras.visible(false);
                }

                if (_creditoLiberacao.ParametroBooleanoCodigo.val() > 0)
                    _creditoLiberacao.ParametroApenasReboque.visible(true);
                else
                    _creditoLiberacao.ParametroApenasReboque.visible(false);

                if (_creditoLiberacao.ParametroData1Codigo.val() > 0) {
                    _creditoLiberacao.ParametroData1.text(_creditoLiberacao.ParametroData1Descricao.val() + ": ");
                    _creditoLiberacao.ParametroData1.visible(true);
                }
                else
                    _creditoLiberacao.ParametroData1.visible(false);

                if (_creditoLiberacao.ParametroData2Codigo.val() > 0) {
                    _creditoLiberacao.ParametroData2.text(_creditoLiberacao.ParametroData2Descricao.val() + ": ");
                    _creditoLiberacao.ParametroData2.visible(true);
                }
                else
                    _creditoLiberacao.ParametroData2.visible(false);

                if (_creditoLiberacao.ParametroTextoCodigo.val() > 0) {
                    _creditoLiberacao.ParametroTexto.text(_creditoLiberacao.ParametroTextoDescricao.val()+": ");
                    _creditoLiberacao.ParametroTexto.visible(true);
                }
                else
                    _creditoLiberacao.ParametroTexto.visible(false);

                if (_creditoLiberacao.ParametroTextoCodigo.val() > 0) {
                    _creditoLiberacao.ParametroTexto.text(_creditoLiberacao.ParametroTextoDescricao.val() + ": ");
                    
                }
                else
                    _creditoLiberacao.ParametroTexto.visible(false);

                if (!string.IsNullOrWhiteSpace(_creditoLiberacao.ParametroDomingo.val()))
                    _creditoLiberacao.ParametroDomingo.visible(true);
                else
                    _creditoLiberacao.ParametroDomingo.visible(false);
            }

            PreencherCTesComplementares();
        }
        else {
            $("#liCTeComplementar").hide();
            $("#liCarga").show();

            _creditoLiberacao.ValorLiberado.visible(true);
            _creditoLiberacao.Remetente.visible(false);
            _creditoLiberacao.Destinatario.visible(false);
            //_creditoLiberacao.DownloadArquivo.visible(false);
        }

        $("#fdsCarga").html("");
        var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", _creditoLiberacao.Carga.val());
        $("#" + knoutCarga.DivCarga.id).attr("class", "p-2");
        _cargaAtual = knoutCarga;
        desabilitarTodasOpcoes(knoutCarga);
    }, null);
    //buscarNotasFiscais();
}

function atualizarCTesComplementaresClick(e) {
    PreencherCTesComplementares();
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}


//*******MÉTODOS*******


function AtualizarSolicitacao() {
    Salvar(_creditoLiberacao, "CreditoLiberacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridCreditoLiberacao.CarregarGrid();
                limparCamposCreditoLiberacao();
                $("#wid-id-4").hide();
                _pesquisaCreditoLiberacao.ExibirFiltros.visibleFade(true);
                //AtualizarDadosControleSaldo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}


function buscarCreditoLiberacaos() {
    var editar = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: editarCreditoLiberacao, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCreditoLiberacao = new GridView(_pesquisaCreditoLiberacao.Pesquisar.idGrid, "CreditoLiberacao/Pesquisa", _pesquisaCreditoLiberacao, menuOpcoes, { column: 0, dir: orderDir.asc }, 10, function () {
        _pesquisaCreditoLiberacao.Codigo.val(0);
    });
    _gridCreditoLiberacao.CarregarGrid();
}


function limparCamposCreditoLiberacao(e) {
    LimparCampos(_creditoLiberacao);
    _creditoLiberacao.RetornoSolicitacao.enable(false);
    _creditoLiberacao.ValorLiberado.enable(false);
    _CRUDcreditoLiberacao.RejeitarSolicitacao.enable(false);
    _CRUDcreditoLiberacao.ResponderSolicitacao.enable(false);
    _creditoLiberacao.DataRetorno.visible(false);
    _creditoLiberacao.Creditor.visible(false);
    limparAnexos();
    resetarTabs();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}


function carregarGridCTeComplementares() {
    _ocorrencia = new Ocorrencia();

    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: "Baixar PDF", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: "Mensagem Sefaz", id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var emitir = {
        descricao: "Emitir", id: guid(), metodo: function (datagrid) {
            reemitirCTeClick(datagrid, _cargaAtual);
        }, icone: "", visibilidade: VisibilidadeRejeicao
    };

    //if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
    //    var visualizar = { descricao: "Detalhes", id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    //    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, retornoSefaz, visualizar, emitir] };
    //}
    //else
    //    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, retornoSefaz, emitir] };
    if (_creditoLiberacao.SolicitacaoTerceiro.val() == true || _creditoLiberacao.SolicitacaoTransportador.val() == true)
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, retornoSefaz, emitir] };
    else {
        var visualizar = { descricao: "Detalhes", id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, retornoSefaz, visualizar, emitir] };
    }

    _gridCTeComplementar = new GridView(_cteComplementar.CTesComplementares.idGrid, "Ocorrencia/ConsultarCTesOcorrencia", _ocorrencia, menuOpcoes, null);
}

function PreencherCTesComplementares() {
    _cteComplementar.CTesComplementares.visible(true);
    _ocorrencia.Codigo.val(_creditoLiberacao.CodigoOcorrencia.val());
    _gridCTeComplementar.CarregarGrid();
}

function reemitirCTeClick(e) {
    if (e.Status == "Rejeição") {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var data = { ocorrencia: _creditoLiberacao.CodigoOcorrencia.val() };
                    PreencherCTesComplementares();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite emissão", "A atual situação do ct-e (" + e.Status + ")  não permite que ele seja emitido novamente");
    }
}

function LimparCamposCTeComplementar() {
    _cteComplementar.CTesComplementares.visible(false);
    LimparCampos(_cteComplementar);
}


function VisibilidadeMensagemSefaz(data) {
    if (data.RetornoSefaz != "" && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeRejeicao(data) {
    if (data.SituacaoCTe == EnumStatusCTe.REJEICAO) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownload(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.NumeroModeloDocumentoFiscal == "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeDownloadOutrosDoc(data) {
    if (data.NumeroModeloDocumentoFiscal != "57") {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoEditar(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.EMDIGITACAO || data.SituacaoCTe == EnumStatusCTe.REJEICAO) && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        return true;
    } else {
        return false;
    }
}


function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);    
    Global.abrirModal('divModalRetornoSefaz');
}
