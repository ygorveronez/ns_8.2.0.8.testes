var _gridSelecaoMDFes, _selecaoMDFes, _cargaGeracaoEmbarcador, _modalGeracaoCargaEmbarcador;

var SelecaoMDFes = function () {
    var dataFinal = moment().format("DD/MM/YYYY");
    var dataInicial = moment().subtract(3, 'days').format("DD/MM/YYYY");

    this.NumeroMDFe = PropertyEntity({ text: "Nº do MDF-e:", getType: typesKnockout.int, visible: ko.observable(true) });
    this.Serie = PropertyEntity({ text: "Série do MDF-e:", getType: typesKnockout.int, visible: ko.observable(true) });
    this.PlacaVeiculo = PropertyEntity({ text: "Veículo:", getType: typesKnockout.string, visible: ko.observable(true) });
    this.NomeMotorista = PropertyEntity({ text: "Motorista:", getType: typesKnockout.string, visible: ko.observable(true) });

    this.UFOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Estado de Origem:", idBtnSearch: guid() });
    this.UFDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Estado de Destino:", idBtnSearch: guid() });
    this.LocalidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Localidade de Origem:", idBtnSearch: guid() });
    this.LocalidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Localidade de Destino:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Empresa/Filial:", idBtnSearch: guid() });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataInicial), def: dataInicial, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataFinal), def: dataFinal, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSelecaoMDFes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: CriarClick, type: types.event, text: "Criar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: LimparCamposSelecaoMDFes, type: types.event, text: "Limpar Seleção", visible: ko.observable(true) });
};

var CargaGeracaoEmbarcador = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Tipo de Operação:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Tipo de Carga:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "Centro de Resultados:", idBtnSearch: guid() });
    this.DataPrevisaoSaida = PropertyEntity({ text: "Data previsão de saída:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data previsão de entrega/retorno:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.DataPrevisaoSaida.dateRangeLimit = this.DataPrevisaoEntrega;
    this.DataPrevisaoEntrega.dateRangeInit = this.DataPrevisaoSaida;

    this.GerarCarga = PropertyEntity({ eventClick: GerarCargaClick, type: types.event, text: "Gerar Carga", visible: ko.observable(true), icon: "fal fa-chevron-down" });
    this.Cancelar = PropertyEntity({ eventClick: FecharTelaGeracaoCargaEmbarcador, type: types.event, text: "Cancelar/Voltar", visible: ko.observable(true), icon: "fal fa-window-close" });
};

//*******EVENTOS*******

function LoadSelecaoMDFes() {
    _selecaoMDFes = new SelecaoMDFes();
    KoBindings(_selecaoMDFes, "knockoutSelecaoMDFes");

    _cargaGeracaoEmbarcador = new CargaGeracaoEmbarcador();
    KoBindings(_cargaGeracaoEmbarcador, "knockoutGeracaoCargaEmbarcador");

    BuscarEstados(_selecaoMDFes.UFOrigem);
    BuscarEstados(_selecaoMDFes.UFDestino);
    BuscarLocalidadesBrasil(_selecaoMDFes.LocalidadeOrigem);
    BuscarLocalidadesBrasil(_selecaoMDFes.LocalidadeDestino);
    BuscarTransportadores(_selecaoMDFes.Empresa);

    BuscarVeiculos(_cargaGeracaoEmbarcador.Veiculo);
    BuscarTiposdeCarga(_cargaGeracaoEmbarcador.TipoCarga);
    BuscarTiposOperacao(_cargaGeracaoEmbarcador.TipoOperacao);
    BuscarCentroResultado(_cargaGeracaoEmbarcador.CentroResultado);

    GridSelecaoMDFes();

    //Abre o modal
    _modalGeracaoCargaEmbarcador = new bootstrap.Modal(document.getElementById("knockoutGeracaoCargaEmbarcador"), { backdrop: true, keyboard: true });

}

function CriarClick(e, sender) {
    if (ValidaDocumentosSelecionados())
        AbrirTelaGeracaoCargaEmbarcador();
}

function GerarCargaClick() {
    exibirConfirmacao("Criar Carga", "Você tem certeza que deseja criar uma carga para os MDF-es selecionados?", function () {
        var dados = RetornarObjetoPesquisa(_cargaGeracaoEmbarcador);

        dados.MDFes = JSON.stringify(ObterCodigosRegistrosMultiplaSelecao(_gridSelecaoMDFes.ObterMultiplosSelecionados()));

        executarReST("GeracaoCargaEmbarcador/GerarCarga", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga gerada com sucesso!");

                    _gridPesquisaGeracaoCargaEmbarcador.CarregarGrid();

                    _gridSelecaoMDFes.AtualizarRegistrosSelecionados([]);
                    _gridSelecaoMDFes.CarregarGrid();

                    FecharTelaGeracaoCargaEmbarcador();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function AbrirTelaGeracaoCargaEmbarcador() {
    _modalGeracaoCargaEmbarcador.show();
    LimparCamposGeracaoCargaEmbarcador();
}

function FecharTelaGeracaoCargaEmbarcador() {
    _modalGeracaoCargaEmbarcador.hide();
    LimparCamposGeracaoCargaEmbarcador();
}

function LimparCamposGeracaoCargaEmbarcador() {
    LimparCampos(_cargaGeracaoEmbarcador);
}

//*******MÉTODOS*******

function ObterCodigosRegistrosMultiplaSelecao(objetos) {
    var codigos = [];

    for (var i = 0; i < objetos.length; i++)
        codigos.push(objetos[i].Codigo);

    return codigos;
}

function GridSelecaoMDFes() {

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: function () { },
        callbackSelecionado: function () { },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    _gridSelecaoMDFes = new GridView(_selecaoMDFes.Pesquisar.idGrid, "GeracaoCargaEmbarcador/PesquisaMDFesDisponiveis", _selecaoMDFes, null, { column: 3, dir: orderDir.desc }, 15, null, null, null, multiplaescolha, null, null, null);
    _gridSelecaoMDFes.SetPermitirRedimencionarColunas(true);
    _gridSelecaoMDFes.CarregarGrid();
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    var itens = _gridSelecaoMDFes.ObterMultiplosSelecionados();

    if (itens.length == 0) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "MDF-es Selecionados", "Nenhum MDF-e selecionado.");
    }

    return valido;
}

function LimparCamposSelecaoMDFes() {
    LimparCampos(_selecaoMDFes);
}
