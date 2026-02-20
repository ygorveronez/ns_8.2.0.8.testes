/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLoteEscrituracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridSelecaoMovimentos;
var _selecaoMovimentos;

var SelecaoMovimentos = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: true, text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: "Tomador:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "Tipo:", options: EnumTipoMovimentoExportacao.ObterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true), enable: ko.observable(true) });

    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento:", getType: typesKnockout.string, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final:  ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSelecaoMovimentos.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: CriarClick, type: types.event, text: "Criar", visible: ko.observable(true) });
};

//*******EVENTOS*******
function LoadSelecaoMovimentos() {
    _selecaoMovimentos = new SelecaoMovimentos();
    KoBindings(_selecaoMovimentos, "knockoutSelecaoMovimentos");

    // Inicia as buscas
    new BuscarTransportadores(_selecaoMovimentos.Empresa);
    new BuscarClientes(_selecaoMovimentos.Tomador);

    // Inicia grid de dados
    GridSelecaoMovimentos();
}

//*******MÉTODOS*******

function CriarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Criar Lote de Contabilização", "Você tem certeza que deseja criar um lote de contabilização para os movimentos selecionados?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoMovimentos);

                dados.SelecionarTodos = _selecaoMovimentos.SelecionarTodos.val();

                if (dados.SelecionarTodos == false)
                    dados.MovimentosSelecionados = JSON.stringify(ObterCodigosRegistrosMultiplaSelecao(_gridSelecaoMovimentos.ObterMultiplosSelecionados()));
                else 
                    dados.MovimentosSelecionados = JSON.stringify(ObterCodigosRegistrosMultiplaSelecao(_gridSelecaoMovimentos.ObterMultiplosNaoSelecionados()));

                executarReST("LoteContabilizacao/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote de contabilização criado com sucesso!");

                            _loteContabilizacao.Situacao.val(EnumSituacaoLoteContabilizacao.AgIntegracao);

                            //_gridSelecaoMovimentos.CarregarGrid();

                            BuscarLoteContabilizacaoPorCodigo(arg.Data.Codigo);
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

function ObterCodigosRegistrosMultiplaSelecao(objetos) {
    var codigos = [];

    for (var i = 0; i < objetos.length; i++)
        codigos.push(objetos[i].Codigo);

    return codigos;
}

function GridSelecaoMovimentos() {
    //-- Cabecalho
    //var detalhes = {
    //    descricao: "Detalhes",
    //    id: guid(),
    //    evento: "onclick",
    //    metodo: function () { },
    //    tamanho: "10",
    //    icone: ""
    //};

    //var menuOpcoes = {
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
        SelecionarTodosKnout: _selecaoMovimentos.SelecionarTodos,
        callbackNaoSelecionado: function () {
            //SelecaoModificado(false);
        },
        callbackSelecionado: function () {
            //SelecaoModificado(true);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    if (_loteContabilizacao.Situacao.val() != EnumSituacaoLoteContabilizacao.EmCriacao)
        multiplaescolha = null;

    var configExportacao = {
        url: "LoteContabilizacao/ExportarPesquisaMovimento",
        titulo: "Movimentos do Lote de Contabilização"
    };

    _gridSelecaoMovimentos = new GridView(_selecaoMovimentos.Pesquisar.idGrid, "LoteContabilizacao/PesquisaDocumento", _selecaoMovimentos, null, null, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridSelecaoMovimentos.SetPermitirRedimencionarColunas(true);
    _gridSelecaoMovimentos.CarregarGrid();
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    var itens = _gridSelecaoMovimentos.ObterMultiplosSelecionados();

    if (itens.length == 0 && !_selecaoMovimentos.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Movimentos Selecionados", "Nenhum movimento selecionado.");
    }

    return valido;
}

function EditarSelecaoMovimentos(data) {
    _selecaoMovimentos.SelecionarTodos.visible(false);
    _selecaoMovimentos.Empresa.enable(false);
    _selecaoMovimentos.Tomador.enable(false);
    _selecaoMovimentos.DataInicio.enable(false);
    _selecaoMovimentos.DataFim.enable(false);
    _selecaoMovimentos.Tipo.enable(false);
    _selecaoMovimentos.Criar.visible(false);
    _selecaoMovimentos.NumeroDocumento.enable(false);

    _selecaoMovimentos.Codigo.val(data.Codigo);
    _selecaoMovimentos.DataInicio.val(data.DataInicial);
    _selecaoMovimentos.DataFim.val(data.DataFinal);

    _selecaoMovimentos.Empresa.val(data.Empresa.Descricao);
    _selecaoMovimentos.Empresa.codEntity(data.Empresa.Codigo);

    _selecaoMovimentos.Tomador.val(data.Tomador.Descricao);
    _selecaoMovimentos.Tomador.codEntity(data.Tomador.Codigo);

    _selecaoMovimentos.NumeroDocumento.val(data.NumeroDocumento);

    _selecaoMovimentos.Tipo.val(data.Tipo);

    GridSelecaoMovimentos();
}

function LimparCamposSelecaoMovimentos() {
    _selecaoMovimentos.Empresa.enable(true);
    _selecaoMovimentos.Tipo.enable(true);
    _selecaoMovimentos.Tomador.enable(true);
    _selecaoMovimentos.DataInicio.enable(true);
    _selecaoMovimentos.DataFim.enable(true);
    _selecaoMovimentos.Criar.visible(true);
    _selecaoMovimentos.SelecionarTodos.visible(true);
    _selecaoMovimentos.SelecionarTodos.val(false);
    _selecaoMovimentos.NumeroDocumento.enable(true);

    LimparCampos(_selecaoMovimentos);
}
