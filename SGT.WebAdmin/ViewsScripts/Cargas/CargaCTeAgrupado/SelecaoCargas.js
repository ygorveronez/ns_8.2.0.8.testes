//*******MAPEAMENTO KNOUCKOUT*******


var _gridSelecaoCargas;
var _selecaoCargas;

var SelecaoCargas = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataAtual), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", def: dataAtual, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Tipo de Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.SemCTeAgrupado = PropertyEntity({ text: "CT-e Agrupado:", options: Global.ObterOpcoesPesquisaBooleano("Sem CT-e gerado", "Com CT-e gerado"), val: ko.observable(true), def: true, issue: 0, enable: ko.observable(true), visible: ko.observable(true) });
    this.ObservacaoCTe = PropertyEntity({ text: "Observação do CT-e:", maxlength: 2000, enable: ko.observable(true), visible: ko.observable(true) });
    this.GerarCTePorCarga = PropertyEntity({ getType: typesKnockout.bool, text: "Gerar um CT-e para cada carga selecionada", val: ko.observable(false), def: false, issue: 0, enable: ko.observable(true), visible: ko.observable(true) });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: "Selecionar Todos" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSelecaoCargas.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: CriarClick, type: types.event, text: "Gerar CT-e", visible: ko.observable(true) });
};

//*******EVENTOS*******
function LoadSelecaoCargasCargaCTeAgrupado() {
    _selecaoCargas = new SelecaoCargas();
    KoBindings(_selecaoCargas, "knockoutSelecaoCargas");

    // Inicia as buscas
    new BuscarTransportadores(_selecaoCargas.Empresa);
    new BuscarGruposPessoas(_selecaoCargas.GrupoPessoas);
    new BuscarTiposdeCarga(_selecaoCargas.TipoCarga);
    new BuscarTiposOperacao(_selecaoCargas.TipoOperacao);
    new BuscarVeiculos(_selecaoCargas.Veiculo);
    new BuscarMotoristas(_selecaoCargas.Motorista);

    // Inicia grid de dados
    GridSelecaoCargas();
}

function CriarClick(e, sender) {
    if (ValidaDocumentosSelecionados()) {
        if (ValidarCamposObrigatorios(e)) {
            exibirConfirmacao("Gerar CT-e", "Deseja realmente gerar um CT-e para as cargas selecionadas?", function () {
                var dados = RetornarObjetoPesquisa(_selecaoCargas);

                dados.SelecionarTodos = _selecaoCargas.SelecionarTodos.val();

                if (dados.SelecionarTodos === false)
                    dados.ListaCargas = JSON.stringify(_gridSelecaoCargas.ObterMultiplosSelecionados().map(o => o.Codigo));
                else
                    dados.ListaCargas = JSON.stringify(_gridSelecaoCargas.ObterMultiplosNaoSelecionados().map(o => o.Codigo));
                
                executarReST("CargaCTeAgrupado/Adicionar", dados, function (arg) {
                    if (arg.Success) {
                        if (arg.Data) {
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e agrupado gerado com sucesso!");

                            //_cargaCTeAgrupado.Situacao.val(EnumSituacaoCargaCTeAgrupado.EmEmissao);

                            _gridCargaCTeAgrupado.CarregarGrid();

                            BuscarCargaCTeAgrupadoPorCodigo(arg.Data.Codigo);
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

function GridSelecaoCargas() {
    
    var menuOpcoes = null;
    
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoCargas.SelecionarTodos,
        callbackNaoSelecionado: function () {
            //SelecaoModificado(false);
        },
        callbackSelecionado: function () {
            //SelecaoModificado(true);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    if (_cargaCTeAgrupado.Codigo.val() > 0)
        multiplaescolha = null;

    var configExportacao = {
        url: "CargaCTeAgrupado/ExportarPesquisaCarga",
        titulo: "Cargas para CT-e Agrupado",
        id: "btnExportarCargas"
    };

    _gridSelecaoCargas = new GridView(_selecaoCargas.Pesquisar.idGrid, "CargaCTeAgrupado/PesquisaCarga", _selecaoCargas, menuOpcoes, null, 10, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridSelecaoCargas.SetPermitirRedimencionarColunas(true);
    _gridSelecaoCargas.CarregarGrid(function () {
        setTimeout(function () {
            if (_selecaoCargas.Codigo.val() > 0)
                $("#btnExportarCargas").show();
            else
                $("#btnExportarCargas").hide();
        }, 200);
    });
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    var itens = _gridSelecaoCargas.ObterMultiplosSelecionados();

    if (itens.length == 0 && !_selecaoCargas.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Cargas Selecionadas", "Nenhuma carga selecionada.");
    }

    return valido;
}

function EditarSelecaoCargas(data) {
    _selecaoCargas.Filtro.visible(false);
    _selecaoCargas.Criar.visible(false);
    _selecaoCargas.ObservacaoCTe.enable(false);
    _selecaoCargas.GerarCTePorCarga.enable(false);

    _selecaoCargas.ObservacaoCTe.val(data.ObservacaoCTe);
    _selecaoCargas.GerarCTePorCarga.val(data.GerarCTePorCarga);
    _selecaoCargas.Codigo.val(data.Codigo);

    GridSelecaoCargas();
}

function LimparCamposSelecaoCargas() {
    _selecaoCargas.SelecionarTodos.val(false);
    _selecaoCargas.SelecionarTodos.visible(true);
    _selecaoCargas.Criar.visible(true);
    _selecaoCargas.Filtro.visible(true);
    _selecaoCargas.ObservacaoCTe.enable(true);
    _selecaoCargas.GerarCTePorCarga.enable(true);

    LimparCampos(_selecaoCargas);
}
