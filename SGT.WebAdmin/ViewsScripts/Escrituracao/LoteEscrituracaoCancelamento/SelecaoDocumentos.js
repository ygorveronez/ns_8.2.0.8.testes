//*******MAPEAMENTO KNOUCKOUT*******


var _gridDocumentosSelecao;
var _selecaoDocumentos;

var SelecaoDocumentos = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: true, text: ko.observable("*Tomador:"), idBtnSearch: guid(), enable: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo do Documento:", enable: ko.observable(false), required: true, idBtnSearch: guid(), visible: ko.observable(true), issue: 370 });

    this.DataInicio = PropertyEntity({ text: "Data Inicial (Cancelamento CT-e):", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final (Cancelamento CT-e):  ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), text: "Selecionar Todos" });

    this.Transportador.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            _selecaoDocumentos.Transportador.codEntity(0);
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosSelecao.CarregarGrid(function () {
                // TODO:
                // Triar o setTimeout e chamar o selecionar todos após renderizar tabela
                // Limpar o selecionar todos quando algum campo do filtro for modificado

                // Só marca todos como selecionado quando Filial, Transportador e Tomador forem selecionados
                var busca = RetornarObjetoPesquisa(_selecaoDocumentos);
                if ((busca.Filial > 0 && busca.Tomador > 0 && busca.Transportador > 0) && _selecaoDocumentos.SelecionarTodos.val() == false) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                }
                else if (_selecaoDocumentos.SelecionarTodos.val() == true) {
                    setTimeout(_selecaoDocumentos.SelecionarTodos.eventClick, 100);
                } else {
                    _selecaoDocumentos.SelecionarTodos.val(false);

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                        if (busca.Transportador > 0)
                            _selecaoDocumentos.SelecionarTodos.visible(true);
                        else
                            _selecaoDocumentos.SelecionarTodos.visible(false);
                    } else {
                        if (busca.Tomador > 0)
                            _selecaoDocumentos.SelecionarTodos.visible(true);
                        else
                            _selecaoDocumentos.SelecionarTodos.visible(false);
                    }
                }
            });
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: criarClick, type: types.event, text: "Criar", visible: ko.observable(true) });
};

//*******EVENTOS*******
function LoadSelecaoDocumentosEscrituracaoCancelamento() {
    _selecaoDocumentos = new SelecaoDocumentos();
    KoBindings(_selecaoDocumentos, "knockoutSelecaoDocumentos");

    // Inicia as buscas
    new BuscarFilial(_selecaoDocumentos.Filial);
    new BuscarTransportadores(_selecaoDocumentos.Transportador);
    new BuscarClientes(_selecaoDocumentos.Tomador);
    new BuscarModeloDocumentoFiscal(_selecaoDocumentos.ModeloDocumentoFiscal, null, null, false, true, null, true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _selecaoDocumentos.Filial.visible(false);
        _selecaoDocumentos.Transportador.text("*Empresa/Filial:");
        _selecaoDocumentos.Tomador.text("Tomador:");
        _selecaoDocumentos.Tomador.required = false;
        _selecaoDocumentos.Transportador.required = true;
    }

    // Inicia grid de dados
    GridSelecaoDocumentos();
}

function criarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Criar Lote de Escrituração de Cancelamentos", "Você tem certeza que deseja criar um lote de escrituração para os documentos selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoDocumentos);

                dados.SelecionarTodos = _selecaoDocumentos.SelecionarTodos.val();
                dados.DocumentosSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosSelecionados());
                dados.DocumentosNaoSelecionadas = JSON.stringify(_gridDocumentosSelecao.ObterMultiplosNaoSelecionados());

                executarReST("LoteEscrituracaoCancelamento/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote de escrituração criado com sucesso!");

                            _loteEscrituracaoCancelamento.Situacao.val(EnumSituacaoLoteEscrituracaoCancelamento.AgIntegracao);

                            _gridLoteEscrituracaoCancelamento.CarregarGrid();

                            BuscarLoteEscrituracaoCancelamentoPorCodigo(arg.Data.Codigo);
                        } else {
                            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, informe os campos obrigatórios!");
        }

    }
}


//*******MÉTODOS*******
function GridSelecaoDocumentos() {
    //-- Cabecalho
    var detalhes = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: function () { },
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = null;
    //{
    //    tipo: TypeOptionMenu.list,
    //    descricao: "Opções",
    //    tamanho: 7,
    //    opcoes: [detalhes]
    //};

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
    };

    if (_loteEscrituracaoCancelamento.Situacao.val() != "")
        multiplaescolha = null;

    var configExportacao = {
        url: "LoteEscrituracaoCancelamento/ExportarPesquisaDocumento",
        titulo: "Documentos do Lote de Escrituração de Cancelamentos",
        id: "btnExportarDocumento"
    };

    _gridDocumentosSelecao = new GridView(_selecaoDocumentos.Pesquisar.idGrid, "LoteEscrituracaoCancelamento/PesquisaDocumento", _selecaoDocumentos, menuOpcoes, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
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

function SelecaoModificado(selecao) {
    // Quando o primeiro item é selecionado, seta os filtros de selecao
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    if (itens.length == 1) {
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

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            if (busca.Transportador == 0) {
                primeiroClick = true;
                _selecaoDocumentos.Transportador.codEntity(itens[0].CodigoTransportador).val(itens[0].Transportador);
                _selecaoDocumentos.ModeloDocumentoFiscal.codEntity(itens[0].CodigoModeloDocumento).val(itens[0].ModeloDocumento);
            }
        } else {
            if (busca.Tomador == 0) {
                primeiroClick = true;
                _selecaoDocumentos.Tomador.codEntity(itens[0].CodigoTomador).val(itens[0].Tomador);
                _selecaoDocumentos.ModeloDocumentoFiscal.codEntity(itens[0].CodigoModeloDocumento).val(itens[0].ModeloDocumento);
            }
        }

        if (primeiroClick) {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                if (_selecaoDocumentos.Transportador.codEntity() > 0) {
                    _selecaoDocumentos.SelecionarTodos.val(true);
                    _selecaoDocumentos.SelecionarTodos.visible(true);
                } else {
                    _selecaoDocumentos.SelecionarTodos.val(false);
                    _selecaoDocumentos.SelecionarTodos.visible(false);
                }
            } else {
                if (_selecaoDocumentos.Tomador.codEntity() > 0) {
                    _selecaoDocumentos.SelecionarTodos.val(true);
                    _selecaoDocumentos.SelecionarTodos.visible(true);
                } else {
                    _selecaoDocumentos.SelecionarTodos.val(false);
                    _selecaoDocumentos.SelecionarTodos.visible(false);
                }
            }
        }


        if (selecao)
            _gridDocumentosSelecao.CarregarGrid();
    }
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    // Valida Quantidade
    var itens = _gridDocumentosSelecao.ObterMultiplosSelecionados();
    // TODO: Se o btn SELECIONAR TODOS estiver clicado, 
    if (itens.length == 0 && !_selecaoDocumentos.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Documentos Selecionados", "Nenhum documento selecionado.");
    }

    return valido;
}

function EditarSelecaoDocumentos(data) {
    _selecaoDocumentos.Transportador.enable(false);
    _selecaoDocumentos.ModeloDocumentoFiscal.enable(false);
    _selecaoDocumentos.Filial.enable(false);
    _selecaoDocumentos.Tomador.enable(false);
    _selecaoDocumentos.DataInicio.enable(false);
    _selecaoDocumentos.DataFim.enable(false);
    _selecaoDocumentos.Criar.visible(false);
    _selecaoDocumentos.Codigo.val(data.Codigo);
    _selecaoDocumentos.DataInicio.val(data.DataInicio);
    _selecaoDocumentos.DataFim.val(data.DataFinal);
    _selecaoDocumentos.Transportador.val(data.Transportador.Descricao);
    _selecaoDocumentos.Filial.val(data.Filial.Descricao);
    _selecaoDocumentos.Tomador.val(data.Tomador.Descricao);
    _selecaoDocumentos.Transportador.codEntity(data.Transportador.Codigo);
    _selecaoDocumentos.Filial.codEntity(data.Filial.Codigo);
    _selecaoDocumentos.Tomador.codEntity(data.Tomador.Codigo);
    _selecaoDocumentos.ModeloDocumentoFiscal.codEntity(data.ModeloDocumentoFiscal.Codigo);
    _selecaoDocumentos.ModeloDocumentoFiscal.val(data.ModeloDocumentoFiscal.Descricao);

    GridSelecaoDocumentos();
}

function LimparCamposSelecaoDocumentos() {
    // Mostra
    _selecaoDocumentos.Transportador.enable(true);
    _selecaoDocumentos.ModeloDocumentoFiscal.enable(true);
    _selecaoDocumentos.Filial.enable(true);
    _selecaoDocumentos.Tomador.enable(true);
    _selecaoDocumentos.DataInicio.enable(true);
    _selecaoDocumentos.DataFim.enable(true);
    _selecaoDocumentos.SelecionarTodos.val(false);
    _selecaoDocumentos.SelecionarTodos.visible(false);
    _selecaoDocumentos.Criar.visible(true);
    LimparCampos(_selecaoDocumentos);
}
