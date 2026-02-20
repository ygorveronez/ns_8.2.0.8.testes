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

var SelecaoDocumentos = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PagamentoLiberado = PropertyEntity({ val: ko.observable(false), options: _pagamentoLiberado, def: false, text: "Pagamento Liberado: ", visible: ko.observable(_CONFIGURACAO_TMS.GerarPagamentoBloqueado && !_CONFIGURACAO_TMS.GerarSomenteDocumentosDesbloqueados) });
    this.Transportador = PropertyEntity({ type: (_CONFIGURACAO_TMS.PermitirMultiplaSelecaoLancamentoLotePagamento ? types.multiplesEntities : types.entity), codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: (_CONFIGURACAO_TMS.PermitirMultiplaSelecaoLancamentoLotePagamento ? types.multiplesEntities : types.entity), codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: (_CONFIGURACAO_TMS.PermitirMultiplaSelecaoLancamentoLotePagamento ? types.multiplesEntities : types.entity), codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final:  ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataInicialEmissao = PropertyEntity({ text: "Data Inicial Emissão:", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.GerarPagamentoBloqueado) });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Final Emissão:  ", val: ko.observable(""), def: "", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.GerarPagamentoBloqueado) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Documento Fiscal:", idBtnSearch: guid(), visible: ko.observable(!_CONFIGURACAO_TMS.GerarLotePagamentoSomenteParaCTe) });
    this.TipoOperacao = PropertyEntity({ type: (_CONFIGURACAO_TMS.PermitirMultiplaSelecaoLancamentoLotePagamento ? types.multiplesEntities : types.entity), codEntity: ko.observable(0), text: ko.observable("Tipo Operação:"), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoDocumentoGerado = PropertyEntity({ val: ko.observable(EnumTipoDocumentoGerado.Todos), options: EnumTipoDocumentoGerado.obterOpcoesPesquisa(), text: "Documentos gerados em:", def: EnumTipoDocumentoGerado.Todos });
    this.SituacaoDocumento = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoPagamento.Todos), options: EnumSituacaoDocumentoPagamento.obterOpcoesPesquisa(), text: "Situação do Documento:", def: EnumSituacaoDocumentoPagamento.Todos, visible: ko.observable(_CONFIGURACAO_TMS.PermitirBloquearDocumentoManualmente) });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosSelecao.CarregarGrid(function () {
                if (_pagamento.Situacao.val() == EnumSituacaoPagamento.EmAlteracao) return;

                var busca = RetornarObjetoPesquisa(_selecaoDocumentos);
                if ((busca.Filial > 0 && busca.Tomador > 0 && busca.Transportador > 0) && _selecaoDocumentos.SelecionarTodos.val() == false) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                }
                else if (_selecaoDocumentos.SelecionarTodos.val() == true) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                } else {
                    _selecaoDocumentos.SelecionarTodos.visible(true);
                }

                atualizarValorTotalDocumentos();
            });
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ValorTotal = PropertyEntity({ text: "Valor Total: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorSelecionados = PropertyEntity({ text: "Valor Total Selecionados: ", getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ListaDocumentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Criar = PropertyEntity({ eventClick: criarClick, type: types.event, text: "Gerar Pagamentos", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });
    this.Reabrir = PropertyEntity({ eventClick: ReabrirClick, type: types.event, text: "Reabrir", visible: ko.observable(false) });
    this.LiberarBloqueados = PropertyEntity({ eventClick: LiberarBloqueadosClick, type: types.event, text: "Liberar Bloqueados", visible: ko.observable(_CONFIGURACAO_TMS.PermitirBloquearDocumentoManualmente) });
};

//*******EVENTOS*******
function loadSelecaoDocumentos() {
    _selecaoDocumentos = new SelecaoDocumentos();
    KoBindings(_selecaoDocumentos, "knockoutSelecaoDocumentos");

    // Inicia as buscas
    new BuscarFilial(_selecaoDocumentos.Filial);
    new BuscarTransportadores(_selecaoDocumentos.Transportador);
    new BuscarCargas(_selecaoDocumentos.Carga, null, null, null, null, null, null, null, null, true, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_selecaoDocumentos.Ocorrencia);
    new BuscarClientes(_selecaoDocumentos.Tomador);
    new BuscarModeloDocumentoFiscal(_selecaoDocumentos.ModeloDocumentoFiscal);
    new BuscarTiposOperacao(_selecaoDocumentos.TipoOperacao);

    // Inicia grid de dados
    GridSelecaoDocumentos();
}

function ReabrirClick() {
    exibirConfirmacao("Reabrir Pagamento", "Você tem certeza que deseja reabrir essa pagamento?", function () {

        var dados = {
            Codigo: _pagamento.Codigo.val()
        };

        executarReST("Pagamento/Reabrir", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento reaberta com sucesso");

                    _gridPagamento.CarregarGrid();

                    BuscarPagamentoPorCodigo(dados.Codigo);
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
            exibirConfirmacao("Alterar Pagamento", "Você tem certeza que deseja salvar a alteração da pagamento?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

                dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
                dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
                dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

                executarReST("Pagamento/Atualizar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento alterada com sucesso");

                            _gridPagamento.CarregarGrid();

                            LimparCamposPagamento();

                            BuscarPagamentoPorCodigo(arg.Data.Codigo);
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
            exibirConfirmacao("Criar Pagamento", "Você tem certeza que deseja criar uma pagamento para os documentos selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

                dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
                dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
                dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

                executarReST("Pagamento/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento criada com sucesso");

                            _gridPagamento.CarregarGrid();

                            if (arg.Data.Codigo > 0)
                                BuscarPagamentoPorCodigo(arg.Data.Codigo);
                            else
                                _gridDocumentosSelecao.CarregarGrid();
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

function LiberarBloqueadosClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        exibirConfirmacao("Desbloquear Documentos", "Você tem certeza que deseja desbloquear os documentos selecionados?", function () {
            var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

            dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
            dados.DocumentosSelecionados = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
            dados.DocumentosNaoSelecionados = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

            executarReST("Pagamento/DesbloquearDocumentoLote", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Documentos desbloqueados com sucesso!");

                        _gridDocumentosSelecao.CarregarGrid();
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
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
    var bloquear = { descricao: "Bloquear / Liberar", id: guid(), metodo: bloquearClick, icone: "", visibilidade: VisibilidadeBloquear };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDACTE, baixarDANFSE, baixarXML, baixarXMLNFSe, baixarPDF, visualizar, bloquear] };

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoDocumentos.SelecionarTodos,
        callbackNaoSelecionado: function () {
            SelecaoModificado(false);
            atualizarValorSelecionados();
        },
        callbackSelecionado: function () {
            SelecaoModificado(true);
            atualizarValorSelecionados();
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    }

    if (_pagamento.Codigo.val() > 0 && _pagamento.Situacao.val() != EnumSituacaoPagamento.Todas)
        multiplaescolha = null;

    var configExportacao = {
        url: "Pagamento/ExportarPesquisaDocumentos",
        titulo: "Documentos da Pagamento",
        id: "btnExportarDocumento"
    };

    var ko_selecao = _selecaoDocumentos;
    if (_pagamento.Situacao.val() != EnumSituacaoPagamento.Todas) {
        ko_selecao = {
            Codigo: _selecaoDocumentos.Codigo
        };
    }

    _gridDocumentosSelecao = new GridView(_selecaoDocumentos.Pesquisar.idGrid, "Pagamento/PesquisaDocumento", ko_selecao, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridDocumentosSelecao.SetPermitirRedimencionarColunas(true);
    _gridDocumentosSelecao.CarregarGrid(function () {
        setTimeout(function () {
            if (_selecaoDocumentos.Codigo.val() > 0)
                $("#btnExportarDocumento").show();
            else
                $("#btnExportarDocumento").hide();

            atualizarValorTotalDocumentos();
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

function VisibilidadeBloquear(data) {
    if (_CONFIGURACAO_TMS.PermitirBloquearDocumentoManualmente) {
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
    // Na reabertura da pagamento permite selecionar qualquer documento
    if (_pagamento.Situacao.val() == EnumSituacaoPagamento.EmAlteracao)
        return;

    // Quando o primeiro item é selecionado, seta os filtros de selecao
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    if (itens != null && itens.length == 1) {
        // Busca
        var busca = RetornarObjetoPesquisa(_selecaoDocumentos);
        _selecaoDocumentos.SelecionarTodos.visible(true);

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

function bloquearClick(e) {
    if (e.SituacaoBloqueio == 'Bloqueado') {
        _bloqueioDocumento.Bloquear.visible(false);
        _bloqueioDocumento.Desbloquear.visible(true);
    } else {
        _bloqueioDocumento.Bloquear.visible(true);
        _bloqueioDocumento.Desbloquear.visible(false);
    }

    _bloqueioDocumento.Codigo.val(e.Codigo);
    _bloqueioDocumento.MotivoBloqueio.val(e.MotivoBloqueio);
    Global.abrirModal('divModalBloqueioDocumento');
}

function EditarSelecaoDocumentos(data) {
    _selecaoDocumentos.Transportador.enable(false);
    _selecaoDocumentos.Filial.enable(false);
    _selecaoDocumentos.Tomador.enable(false);
    _selecaoDocumentos.Carga.enable(false);
    _selecaoDocumentos.Ocorrencia.enable(false);
    _selecaoDocumentos.DataInicio.enable(false);
    _selecaoDocumentos.DataFim.enable(false);
    _selecaoDocumentos.DataInicialEmissao.enable(false);
    _selecaoDocumentos.DataFinalEmissao.enable(false);

    _selecaoDocumentos.Codigo.val(data.Codigo);
    GridSelecaoDocumentos();

    _selecaoDocumentos.Criar.visible(false);
    _selecaoDocumentos.DataInicio.val(data.DataInicial);
    _selecaoDocumentos.DataFim.val(data.DataFinal);
    _selecaoDocumentos.DataInicialEmissao.val(data.DataInicialEmissao);
    _selecaoDocumentos.DataFinalEmissao.val(data.DataFinalEmissao);

    _selecaoDocumentos.Transportador.val(data.Transportador.Descricao);
    _selecaoDocumentos.Filial.val(data.Filial.Descricao);
    _selecaoDocumentos.Tomador.val(data.Tomador.Descricao);
    _selecaoDocumentos.Ocorrencia.val(data.Ocorrencia.Descricao);
    _selecaoDocumentos.Carga.val(data.Carga.Descricao);

    _selecaoDocumentos.Ocorrencia.codEntity(data.Ocorrencia.Codigo);
    _selecaoDocumentos.Carga.codEntity(data.Carga.Codigo);
    _selecaoDocumentos.Transportador.codEntity(data.Transportador.Codigo);
    _selecaoDocumentos.Filial.codEntity(data.Filial.Codigo);
    _selecaoDocumentos.Tomador.codEntity(data.Tomador.Codigo);
}

function LimparCamposSelecaoDocumentos() {
    // Mostra
    $("#btnExportarDocumento").hide();
    _selecaoDocumentos.Transportador.enable(true);
    _selecaoDocumentos.Filial.enable(true);
    _selecaoDocumentos.Tomador.enable(true);
    _selecaoDocumentos.Carga.enable(true);
    _selecaoDocumentos.Ocorrencia.enable(true);
    _selecaoDocumentos.DataInicio.enable(true);
    _selecaoDocumentos.DataFim.enable(true);
    _selecaoDocumentos.SelecionarTodos.val(false);

    _selecaoDocumentos.SelecionarTodos.visible(true);
    _selecaoDocumentos.Criar.visible(true);
    _selecaoDocumentos.Reabrir.visible(false);
    _selecaoDocumentos.Finalizar.visible(false);
    LimparCampos(_selecaoDocumentos);
}

function atualizarValorSelecionados() {
    carregarListaDocumentos();

    if (!string.IsNullOrWhiteSpace(_selecaoDocumentos.ListaDocumentos.val())) {
        executarReST("Pagamento/BuscarDocumentosSelecionados", RetornarObjetoPesquisa(_selecaoDocumentos), function (r) {
            if (r.Success) {
                if (r.Data) {
                    var data = r.Data;
                    var valorTotalPendente = data.ValorTotalPendente;

                    _selecaoDocumentos.ValorSelecionados.val(valorTotalPendente);

                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else {
        _selecaoDocumentos.ValorSelecionados.val("0,00");
    }
}

function atualizarValorTotalDocumentos() {
    executarReST("Pagamento/BuscarSomaTotalDocumentos", RetornarObjetoPesquisa(_selecaoDocumentos), function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                var valorTotalDocumentos = data.ValorTotalDocumentos;

                _selecaoDocumentos.ValorTotal.val(valorTotalDocumentos);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function carregarListaDocumentos() {
    var titulosSelecionados = null;

    if (_selecaoDocumentos.SelecionarTodos.val())
        titulosSelecionados = _gridDocumentosSelecao.ObterMultiplosNaoSelecionados();
    else
        titulosSelecionados = _gridDocumentosSelecao.ObterMultiplosSelecionados();

    var codigosTitulos = new Array();

    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _selecaoDocumentos.SelecionarTodos.val()))
        _selecaoDocumentos.ListaDocumentos.val(JSON.stringify(codigosTitulos));
    else
        _selecaoDocumentos.ListaDocumentos.val("");
}