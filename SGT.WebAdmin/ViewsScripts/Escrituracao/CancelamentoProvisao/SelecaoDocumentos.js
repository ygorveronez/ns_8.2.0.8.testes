/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridDocumentosSelecao;
var _selecaoDocumentos;
var _tipoOperacaoDocumentoDinamico = [
    { text: "Todos", value: "" }
];

var _situacaoCancelarProvisaoContraPartida = [
    { text: "Não", value: false },
    { text: "Sim", value: true }
];

var SelecaoDocumentos = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: _CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? true : false, text: _CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? "*Transportador:" : "Transportador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, cssClass: (_CONFIGURACAO_TMS.DisponbilizarProvisaoContraPartidaParaCancelamento ? "col col-xs-12 col-sm-12 col-md-4 col-lg-4" : "col col-xs-12 col-sm-12 col-md-6 col-lg-6"), required: false, text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataInicio = PropertyEntity({ text: "Data Inicial (Emissão Documento):", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final (Emissão Documento):  ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.TipoOperacaoDocumento = PropertyEntity({ val: ko.observable(""), options: ko.observableArray(_tipoOperacaoDocumentoDinamico), def: "", text: "Regra Documentos: ", visible: ko.observable(false) });
    this.CancelamentoProvisaoContraPartida = PropertyEntity({ val: ko.observable(false), options: _situacaoCancelarProvisaoContraPartida, def: false, text: "Cancelar provisões de contra partida: ", visible: _CONFIGURACAO_TMS.DisponbilizarProvisaoContraPartidaParaCancelamento, enable: ko.observable(true) });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? false : true), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosSelecao.CarregarGrid(function () {
                if (_cancelamentoProvisao.Situacao.val() == EnumSituacaoCancelamentoProvisao.EmCancelamento) return;

                var busca = RetornarObjetoPesquisa(_selecaoDocumentos);
                if (_selecaoDocumentos.SelecionarTodos.val() == true) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                } else {
                    if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
                        if (busca.Transportador > 0) {
                            _selecaoDocumentos.SelecionarTodos.val(true);
                            _selecaoDocumentos.SelecionarTodos.visible(true);
                        } else {
                            _selecaoDocumentos.SelecionarTodos.val(false);
                            _selecaoDocumentos.SelecionarTodos.visible(false);
                        }
                    } else {
                        _selecaoDocumentos.SelecionarTodos.visible(true);
                    }
                }
            });
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: criarClick, type: types.event, text: "Gerar Cancelamento das Provisões", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadSelecaoDocumentos() {
    _selecaoDocumentos = new SelecaoDocumentos();
    KoBindings(_selecaoDocumentos, "knockoutSelecaoDocumentos");

    // Inicia as buscas
    new BuscarFilial(_selecaoDocumentos.Filial);
    new BuscarCargas(_selecaoDocumentos.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_selecaoDocumentos.Ocorrencia);
    new BuscarTransportadores(_selecaoDocumentos.Transportador);
    new BuscarClientes(_selecaoDocumentos.Tomador);

    // Inicia grid de dados
    GridSelecaoDocumentos();


    if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
        _selecaoDocumentos.Transportador.codEntity.subscribe(function (val) {
            if (val > 0 && _cancelamentoProvisao.Situacao.val() === EnumSituacaoCancelamentoProvisao.AgIntegracao) {
                _selecaoDocumentos.SelecionarTodos.val(true);
                _selecaoDocumentos.SelecionarTodos.visible(true);
            }
        });
    }
}

function criarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Criar Provisão", "Você tem certeza que deseja gerar o cancelamento de provisão para os documentos selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

                dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
                dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
                dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

                executarReST("CancelamentoProvisao/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento criado com sucesso");
                            _gridCancelamentoProvisao.CarregarGrid();
                            LimparCamposCancelamentoProvisao();
                            BuscarCancelamentoProvisaoPorCodigo(arg.Data.Codigo);
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, informe os campos obrigatórios");
        }

    }
}


//*******MÉTODOS*******
function GridSelecaoDocumentos() {
    //-- Cabecalho
    var baixarXMLNFSe = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: "Baixar DANFSE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: "Baixar PDF", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var visualizar = { descricao: "Detalhes", id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, visualizar] };

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoDocumentos.SelecionarTodos,
        callbackNaoSelecionado: function () {
            SelecaoModificado(false);
        },
        callbackSelecionado: function () {
            SelecaoModificado(true);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    }

    //if (_cancelamentoProvisao.Codigo.val() > 0 && _cancelamentoProvisao.Situacao.val() != EnumSituacaoCancelamentoProvisao.EmAlteracao)
    //    multiplaescolha = null;

    var configExportacao = {
        url: "CancelamentoProvisao/ExportarPesquisaDocumentos",
        titulo: "Documentos da Provisão",
        id: "btnExportarDocumento"
    };

    var ko_selecao = _selecaoDocumentos;
    //if (_cancelamentoProvisao.Situacao.val() == EnumSituacaoCancelamentoProvisao.EmAlteracao) {
    //    ko_selecao = {
    //        Codigo: _selecaoDocumentos.Codigo
    //    };
    //}

    _gridDocumentosSelecao = new GridView(_selecaoDocumentos.Pesquisar.idGrid, "CancelamentoProvisao/PesquisaDocumento", ko_selecao, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridDocumentosSelecao.SetPermitirRedimencionarColunas(true);
    _gridDocumentosSelecao.CarregarGrid(function () {
        setTimeout(function () {
            if (_selecaoDocumentos.Codigo.val() > 0)
                $("#btnExportarDocumento").show();
            else
                $("#btnExportarDocumento").hide();
        }, 200);
    });
}

function VisibilidadeOpcaoDownloadDANFSE(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFS)) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownload(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeDownloadOutrosDoc(data) {
    if (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.Outros && data.SituacaoCTe != "") {
        return true;
    } else {
        return false;
    }
}


function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}


function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}


function detalhesCTeClick(e, sender) {
    var codigo = parseInt(e.CodigoCTE);
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, e.Codigo, instancia);
        }
    }, permissoesSomenteLeituraCTe);
}

function SelecaoModificado(selecao) {
    // Na reabertura da provisão permite selecionar qualquer documento
    //if (_cancelamentoProvisao.Situacao.val() == EnumSituacaoCancelamentoProvisao.EmAlteracao)
    //    return;

    // Quando o primeiro item é selecionado, seta os filtros de selecao
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    if (itens != null && itens.length == 1) {
        // Busca
        var busca = RetornarObjetoPesquisa(_selecaoDocumentos);

        // Seta os dados do documentos selecionados nos campos em branco

        //var primeiroClick = false;

        //if (busca.Filial == 0) {
        //    primeiroClick = true;
        //    _selecaoDocumentos.Filial.codEntity(itens[0].CodigoFilial).val(itens[0].Filial);
        //}
        //if (busca.Transportador == 0) {
        //    primeiroClick = true;
        //    _selecaoDocumentos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Transportador);
        //}

        if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
            if (busca.Transportador == 0) {
                //primeiroClick = true;
                _selecaoDocumentos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Transportador);
            }

            //if (primeiroClick) {

            //}

            if (_selecaoDocumentos.Transportador.codEntity() > 0) {
                _selecaoDocumentos.SelecionarTodos.val(true);
                _selecaoDocumentos.SelecionarTodos.visible(true);
            } else {
                _selecaoDocumentos.SelecionarTodos.val(false);
                _selecaoDocumentos.SelecionarTodos.visible(false);
            }

            if (selecao)
                _gridDocumentosSelecao.CarregarGrid();
        } else {
            _selecaoDocumentos.SelecionarTodos.visible(true);
        }
    }
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    // Valida Quantidade
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    // TODO: Se o btn SELECIONAR TODOS estiver clicado, 
    if (itens && itens.length == 0 && !_selecaoDocumentos.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Documentos Selecionados", "Nenhum documento selecionado.");
    }

    return valido;
}

function EditarSelecaoDocumentos(data) {
    _selecaoDocumentos.Transportador.enable(false);
    _selecaoDocumentos.Filial.enable(false);
    _selecaoDocumentos.Tomador.enable(false);
    _selecaoDocumentos.Carga.enable(false);
    _selecaoDocumentos.Ocorrencia.enable(false);
    _selecaoDocumentos.DataInicio.enable(false);
    _selecaoDocumentos.DataFim.enable(false);
    _selecaoDocumentos.CancelamentoProvisaoContraPartida.enable(false);

    _selecaoDocumentos.Codigo.val(data.Codigo);
    GridSelecaoDocumentos();

    _selecaoDocumentos.Criar.visible(false);

    _selecaoDocumentos.CancelamentoProvisaoContraPartida.val(data.CancelamentoProvisaoContraPartida);
    _selecaoDocumentos.DataInicio.val(data.DataInicial);
    _selecaoDocumentos.DataFim.val(data.DataFinal);
    _selecaoDocumentos.Transportador.val(data.Transportador.Descricao);
    _selecaoDocumentos.Filial.val(data.Filial.Descricao);
    _selecaoDocumentos.Carga.val(data.Carga.Descricao);
    _selecaoDocumentos.Ocorrencia.val(data.Ocorrencia.Descricao);
    _selecaoDocumentos.Tomador.val(data.Tomador.Descricao);
    _selecaoDocumentos.Transportador.codEntity(data.Transportador.Codigo);
    _selecaoDocumentos.Filial.codEntity(data.Filial.Codigo);
    _selecaoDocumentos.Carga.codEntity(data.Carga.Codigo);
    _selecaoDocumentos.Ocorrencia.codEntity(data.Ocorrencia.Codigo);
    _selecaoDocumentos.Tomador.codEntity(data.Tomador.Codigo);
}

function LimparCamposSelecaoDocumentos() {
    // Mostra
    $("#btnExportarDocumento").hide();
    _selecaoDocumentos.CancelamentoProvisaoContraPartida.enable(true);
    _selecaoDocumentos.Transportador.enable(true);
    _selecaoDocumentos.Filial.enable(true);
    _selecaoDocumentos.Carga.enable(true);
    _selecaoDocumentos.Ocorrencia.enable(true);
    _selecaoDocumentos.Tomador.enable(true);
    _selecaoDocumentos.DataInicio.enable(true);
    _selecaoDocumentos.DataFim.enable(true);
    _selecaoDocumentos.SelecionarTodos.val(false);
    if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
        _selecaoDocumentos.SelecionarTodos.visible(false);
    } else {
        _selecaoDocumentos.SelecionarTodos.visible(true);
    }
    _selecaoDocumentos.Criar.visible(true);
    LimparCampos(_selecaoDocumentos);

}
