//*******MAPEAMENTO KNOUCKOUT*******

var _gridDocumentosTituloManual;
var _geracaoTituloManual;

var _tipoPessoa = [
    { text: "Pessoa (CNPJ/CPF)", value: 1 },
    { text: "Grupo de Pessoas", value: 2 }
];

var GeracaoTituloManual = function () {
    var dataAtual = Global.DataAtual();

    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, text: "Tipo de Pessoa:" });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.NumeroInicial = PropertyEntity({ text: "Nº Doc. Inicial:", maxlength: 15, getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Nº Doc. Final:", maxlength: 15, getType: typesKnockout.int });
    this.Serie = PropertyEntity({ text: "Série:", maxlength: 3, getType: typesKnockout.int });
    this.NumeroCarga = PropertyEntity({ text: "Nº Carga:", maxlength: 15 });
    this.NumeroPedido = PropertyEntity({ text: "Nº Pedido:" });
    this.NumeroOcorrencia = PropertyEntity({ text: "Nº Ocorrência:" });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos" });
    this.ListaDocumentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.GerarTitulos = PropertyEntity({ eventClick: GerarTitulosClick, type: types.event, text: "Gerar Títulos", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-chevron-down" });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosTituloManual.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), icon: "fal fa-search"
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), icon: "fal fa-plus"
    });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _geracaoTituloManual.Pessoa.visible(true);
            _geracaoTituloManual.GrupoPessoas.visible(false);
            _geracaoTituloManual.GrupoPessoas.codEntity(0);
            _geracaoTituloManual.GrupoPessoas.val('');
        } else {
            _geracaoTituloManual.GrupoPessoas.visible(true);
            _geracaoTituloManual.Pessoa.visible(false);
            _geracaoTituloManual.Pessoa.codEntity(0);
            _geracaoTituloManual.Pessoa.val('');
        }
    });
}

//*******EVENTOS*******

function LoadGeracaoTituloManual() {

    _geracaoTituloManual = new GeracaoTituloManual();
    KoBindings(_geracaoTituloManual, "knockoutGeracaoTituloManual");

    HeaderAuditoria("Titulo", _geracaoTituloManual);

    new BuscarClientes(_geracaoTituloManual.Pessoa);
    new BuscarClientes(_geracaoTituloManual.Remetente);
    new BuscarClientes(_geracaoTituloManual.Destinatario);
    new BuscarGruposPessoas(_geracaoTituloManual.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarEmpresa(_geracaoTituloManual.Empresa);
    new BuscarLocalidadesBrasil(_geracaoTituloManual.Origem);
    new BuscarLocalidadesBrasil(_geracaoTituloManual.Destino);
    new BuscarVeiculos(_geracaoTituloManual.Veiculo);
    new BuscarMotoristas(_geracaoTituloManual.Motorista);

    BuscarDocumentosGeracaoTituloManual();
}

function GerarTitulosClick() {
    var documentosSelecionados = null;

    if (_geracaoTituloManual.SelecionarTodos.val()) {
        documentosSelecionados = _gridDocumentosTituloManual.ObterMultiplosNaoSelecionados();
    } else {
        documentosSelecionados = _gridDocumentosTituloManual.ObterMultiplosSelecionados();
    }

    var codigosDocumentos = new Array();

    for (var i = 0; i < documentosSelecionados.length; i++)
        codigosDocumentos.push(documentosSelecionados[i].DT_RowId);

    _geracaoTituloManual.ListaDocumentos.val(JSON.stringify(codigosDocumentos));

    executarReST("GeracaoTituloManual/GerarTitulos", RetornarObjetoPesquisa(_geracaoTituloManual), function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Títulos gerados com sucesso!");

                _geracaoTituloManual.ListaDocumentos.val("");
                _geracaoTituloManual.SelecionarTodos.val(false);

                _gridDocumentosTituloManual.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******MÉTODOS*******

function BuscarDocumentosGeracaoTituloManual() {

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () { },
        callbackNaoSelecionado: function () { },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _geracaoTituloManual.SelecionarTodos,
        somenteLeitura: false,
    };

    _gridDocumentosTituloManual = new GridView(_geracaoTituloManual.Pesquisar.idGrid, "GeracaoTituloManual/PesquisaDocumentosPendentes", _geracaoTituloManual, null, { column: 4, dir: orderDir.desc }, 20, null, null, null, multiplaescolha);

    _gridDocumentosTituloManual.CarregarGrid();

}

function LimparCamposGeracaoTituloManual() {
    LimparCampos(_geracaoTituloManual);
}