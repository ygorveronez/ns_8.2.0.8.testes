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

var _tipoLocalPrestacao = [
    { text: "Todas", value: EnumTipoLocalPrestacao.todos },
    { text: "Municipal", value: EnumTipoLocalPrestacao.intraMunicipal },
    { text: "Intermunicipal", value: EnumTipoLocalPrestacao.interMunicipal }
];

var SelecaoDocumentos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: _CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? true : false, text: _CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? "*Transportador:" : "Transportador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "*Tomador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial (Emissão Documento):", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final (Emissão Documento):  ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.TipoOperacaoDocumento = PropertyEntity({ val: ko.observable(""), options: ko.observableArray(_tipoOperacaoDocumentoDinamico), def: "", text: "Regra Documentos: ", enable: ko.observable(true), visible: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoLocalPrestacao = PropertyEntity({ val: ko.observable(EnumTipoLocalPrestacao.todos), options: _tipoLocalPrestacao, def: EnumTipoLocalPrestacao.todos, text: "Tipo da Prestação: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoEtapasDocumentoProvisao = PropertyEntity({ val: ko.observable(false), text: "Tipo Documento: " });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos ? false : true), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosSelecao.CarregarGrid(function () {
                if (_provisao.Situacao.val() == EnumSituacaoProvisao.EmAlteracao) return;

                var busca = RetornarObjetoPesquisa(_selecaoDocumentos);
                if ((busca.Filial > 0 && busca.Tomador > 0 && busca.Transportador > 0) && _selecaoDocumentos.SelecionarTodos.val() == false) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                }
                else if (_selecaoDocumentos.SelecionarTodos.val() == true) {
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

    this.Criar = PropertyEntity({ eventClick: criarClick, type: types.event, text: "Gerar Provisões", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Reabrir = PropertyEntity({ eventClick: ReabrirClick, type: types.event, text: "Reabrir", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadSelecaoDocumentos() {
    _selecaoDocumentos = new SelecaoDocumentos();
    KoBindings(_selecaoDocumentos, "knockoutSelecaoDocumentos");

    // Inicia as buscas
    new BuscarFilial(_selecaoDocumentos.Filial);
    new BuscarTransportadores(_selecaoDocumentos.Transportador);
    new BuscarClientes(_selecaoDocumentos.Tomador);
    new BuscarCargas(_selecaoDocumentos.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_selecaoDocumentos.Ocorrencia);
    new BuscarTiposOperacao(_selecaoDocumentos.TipoOperacao);

    // Inicia grid de dados
    GridSelecaoDocumentos();


    if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
        _selecaoDocumentos.TipoLocalPrestacao.visible(false);
        BuscarTiposOperacaoDocumento();
        _selecaoDocumentos.Transportador.codEntity.subscribe(function (val) {
            if (val > 0 && _provisao.Situacao.val() === EnumSituacaoProvisao.AgIntegracao) {
                _selecaoDocumentos.SelecionarTodos.val(true);
                _selecaoDocumentos.SelecionarTodos.visible(true);
            }
        });
    }
}

function BuscarTiposOperacaoDocumento() {
    executarReST("Provisao/BuscarTiposOperacaoDocumento", {}, function (arg) {
        if (arg.Success) {
            if (typeof arg.Data == "object") {
                var opcoes = arg.Data.map(function (opt) {
                    return {
                        text: opt.Descricao,
                        value: opt.Codigo
                    };
                });

                var todasOpcoes = _tipoOperacaoDocumentoDinamico.concat(opcoes);
                _selecaoDocumentos.TipoOperacaoDocumento.options(todasOpcoes);
                _selecaoDocumentos.TipoOperacaoDocumento.visible(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function ReabrirClick() {
    exibirConfirmacao("Reabrir Provisão", "Você tem certeza que deseja reabrir essa provisão?", function () {

        var dados = {
            Codigo: _provisao.Codigo.val()
        };

        executarReST("Provisao/Reabrir", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Provisão reaberta com sucesso");

                    _gridProvisao.CarregarGrid();

                    LimparCamposProvisao();

                    BuscarProvisaoPorCodigo(dados.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function finalizarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Alterar Provisão", "Você tem certeza que deseja salvar a alteração da provisão?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

                dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
                dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
                dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

                executarReST("Provisao/Atualizar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Provisão alterada com sucesso");

                            _gridProvisao.CarregarGrid();

                            LimparCamposProvisao();

                            BuscarProvisaoPorCodigo(arg.Data.Codigo);
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

function criarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Criar Provisão", "Você tem certeza que deseja criar uma provisão para os documentos selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

                dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
                dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
                dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

                var dadosEtapaProvisao = { TipoProvisao: _etapaProvisao.TipoProvisao.val() };

                var dadosEnvioProvisao = $.extend({}, dadosEtapaProvisao, dados);

                executarReST("Provisao/Adicionar", dadosEnvioProvisao, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Provisão criada com sucesso");
                            //_provisao.Situacao.val(EnumSituacaoProvisao.AgIntegracao);
                            _gridProvisao.CarregarGrid();
                            LimparCamposProvisao();

                            if (arg.Data.Codigo > 0)
                                BuscarProvisaoPorCodigo(arg.Data.Codigo);
                            else {
                                _selecaoDocumentos.SelecionarTodos.val(false)
                                _gridDocumentosSelecao.CarregarGrid();
                            }
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                        _gridDocumentosSelecao.CarregarGrid();
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
        callbackSelecionado: function (_, item) {
            FiltrarCargas(item);
            SelecaoModificado(true);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    }

    if (_provisao.Codigo.val() > 0 && _provisao.Situacao.val() != EnumSituacaoProvisao.EmAlteracao)
        multiplaescolha = null;

    var configExportacao = {
        url: "Provisao/ExportarPesquisaDocumentos",
        titulo: "Documentos da Provisão",
        id: "btnExportarDocumento"
    };

    var ko_selecao = _selecaoDocumentos;
    if (_provisao.Situacao.val() == EnumSituacaoProvisao.EmAlteracao) {
        ko_selecao = {
            Codigo: _selecaoDocumentos.Codigo
        };
    }

    _gridDocumentosSelecao = new GridView(_selecaoDocumentos.Pesquisar.idGrid, "Provisao/PesquisaDocumento", ko_selecao, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
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
    if (!(_provisao.AgruparProvisoesPorNotaFiscalFechamentoMensal.val())) {

        // Na reabertura da provisão permite selecionar qualquer documento
        if (_provisao.Situacao.val() == EnumSituacaoProvisao.EmAlteracao)
            return;

        // Quando o primeiro item é selecionado, seta os filtros de selecao
        var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
        if (itens != null && itens.length == 1) {
            // Busca
            var busca = RetornarObjetoPesquisa(_selecaoDocumentos);

            // Seta os dados do documentos selecionados nos campos em branco

            var primeiroClick = false;

            //if (busca.Filial == 0) {
            //    primeiroClick = true;
            //    _selecaoDocumentos.Filial.codEntity(itens[0].CodigoFilial).val(itens[0].Filial);
            //}
            //if (busca.Transportador == 0) {
            //    primeiroClick = true;
            //    _selecaoDocumentos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Transportador);
            //}

            if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
                if (_selecaoDocumentos.Transportador.codEntity() == 0) {
                    primeiroClick = true;
                    _selecaoDocumentos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Transportador);
                    var transp = { Codigo: itens[0].CodigoTransportador, Descricao: itens[0].Transportador };
                    var transps = new Array();
                    transps.push(transp);
                    _selecaoDocumentos.Transportador.multiplesEntities(transps);

                    if (_selecaoDocumentos.Tomador.codEntity() == 0) {
                        primeiroClick = true;
                        _selecaoDocumentos.Tomador.codEntity(itens[0].CodigoTomador).val(itens[0].Tomador);
                    }
                }

                if (primeiroClick) {
                    if (_selecaoDocumentos.Transportador.codEntity() > 0) {
                        _selecaoDocumentos.SelecionarTodos.val(true);
                        _selecaoDocumentos.SelecionarTodos.visible(true);
                    } else {
                        _selecaoDocumentos.SelecionarTodos.val(false);
                        _selecaoDocumentos.SelecionarTodos.visible(false);
                    }

                    if (selecao)
                        _gridDocumentosSelecao.CarregarGrid();
                }


            } else {
                _selecaoDocumentos.SelecionarTodos.visible(true);
            }
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

function controlarCamposSelecaoDocumentosHabilitados(habilitar) {
    _selecaoDocumentos.Transportador.enable(habilitar);
    _selecaoDocumentos.Filial.enable(habilitar);
    _selecaoDocumentos.Tomador.enable(habilitar);
    _selecaoDocumentos.DataInicio.enable(habilitar);
    _selecaoDocumentos.DataFim.enable(habilitar);
    _selecaoDocumentos.Carga.enable(habilitar);
    _selecaoDocumentos.TipoLocalPrestacao.enable(habilitar);
    _selecaoDocumentos.Ocorrencia.enable(habilitar);
    _selecaoDocumentos.TipoOperacaoDocumento.enable(habilitar);
    _selecaoDocumentos.TipoOperacao.enable(habilitar);
}

function EditarSelecaoDocumentos(data) {
    controlarCamposSelecaoDocumentosHabilitados(false);

    _selecaoDocumentos.Codigo.val(data.Codigo);

    GridSelecaoDocumentos();

    _selecaoDocumentos.Criar.visible(false);

    if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos) {
        if (_provisao.Situacao.val() == EnumSituacaoProvisao.Finalizado)
            _selecaoDocumentos.Reabrir.visible(true);

        if (_provisao.Situacao.val() == EnumSituacaoProvisao.EmAlteracao) {
            _selecaoDocumentos.Finalizar.visible(true);
            _gridDocumentosSelecao.AtualizarRegistrosSelecionados(data.Documentos);
        }
    }

    _selecaoDocumentos.Carga.codEntity(data.Carga.Codigo);
    _selecaoDocumentos.Carga.val(data.Carga.Descricao);
    _selecaoDocumentos.DataInicio.val(data.DataInicial);
    _selecaoDocumentos.DataFim.val(data.DataFinal);
    _selecaoDocumentos.Filial.codEntity(data.Filial.Codigo);
    _selecaoDocumentos.Filial.val(data.Filial.Descricao);
    _selecaoDocumentos.Ocorrencia.codEntity(data.Ocorrencia.Codigo);
    _selecaoDocumentos.Ocorrencia.val(data.Ocorrencia.Descricao);
    _selecaoDocumentos.TipoLocalPrestacao.val(data.TipoLocalPrestacao);
    _selecaoDocumentos.TipoOperacao.codEntity(data.TipoOperacao.Codigo);
    _selecaoDocumentos.TipoOperacao.val(data.TipoOperacao.Descricao);
    _selecaoDocumentos.TipoOperacaoDocumento.val(data.TipoOperacaoDocumento);
    _selecaoDocumentos.Tomador.codEntity(data.Tomador.Codigo);
    _selecaoDocumentos.Tomador.val(data.Tomador.Descricao);
    _selecaoDocumentos.Transportador.multiplesEntities(data.Transportador);
}

function LimparCamposSelecaoDocumentos() {
    $("#btnExportarDocumento").hide();

    controlarCamposSelecaoDocumentosHabilitados(true);

    if (_CONFIGURACAO_TMS.ProvisionarDocumentosEmitidos)
        _selecaoDocumentos.SelecionarTodos.visible(false);
    else
        _selecaoDocumentos.SelecionarTodos.visible(true);

    _selecaoDocumentos.Criar.visible(true);
    _selecaoDocumentos.Reabrir.visible(false);
    _selecaoDocumentos.Finalizar.visible(false);

    LimparCampos(_selecaoDocumentos);
}

function FiltrarCargas(item) {
    if (item == null)
        return;

    if (!item.GerarProvisaoSomenteParaCarga)
        return;

    if (_provisao.AgruparProvisoesPorNotaFiscalFechamentoMensal.val())
        return;

    _selecaoDocumentos.Tomador.val(item.Tomador);
    _selecaoDocumentos.Tomador.codEntity(item.CodigoTomador);
    _selecaoDocumentos.Carga.codEntity(item.CodigoCarga);
    _selecaoDocumentos.Carga.val(item.Carga);
    _gridDocumentosSelecao.CarregarGrid();
    _selecaoDocumentos.SelecionarTodos.eventClick()
}