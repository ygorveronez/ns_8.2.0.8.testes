//#region Variaveis
var _gridFornecedorProduto;
var _fornecedorProduto;
var _modalfornecedorProduto;
//#endregion

//#region Funções Publicos
function FornecedorProduto() {
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.Adicionar = PropertyEntity({ eventClick: abrirModalFornecedorProduto, type: types.event, text: "Adicionar", idGrid: guid() });
}
function ModalFornecedorProduto() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CodigoProduto = PropertyEntity({ text: "Codigo Produto:", val: ko.observable("") });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarItemGrid, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CloseModalFornecedorProduto, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarRegristroGrid, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(false) });
    this.Remover = PropertyEntity({ eventClick: RemoverRegistroGrid, type: types.event, text: "Remover", idGrid: guid(), visible: ko.observable(false) });
}
function LoadFornecedorProduto() {
    _fornecedorProduto = new FornecedorProduto();
    KoBindings(_fornecedorProduto, "knockoutFonecedorProduto");

    _modalfornecedorProduto = new ModalFornecedorProduto();
    KoBindings(_modalfornecedorProduto, "knoutModalFornecedorProduto");

    new BuscarFilial(_modalfornecedorProduto.Filial);
    new BuscarClientes(_modalfornecedorProduto.Fornecedor);

    LoadGridFornecedorProduto();
}

//#endregion

//#region Funções Auxiliares

function abrirModalFornecedorProduto() {
    Global.abrirModal("ModalFornecedorProduto");
}

function LoadGridFornecedorProduto() {
    const headerFornecedorProduto = [
        { data: "Codigo", visible: false },
        { data: "CodigoFilial", visible: false },
        { data: "CodigoFornecedor", visible: false },
        { data: "Fornecedor", title: "Fornecedor" },
        { data: "Filial", title: "Filial" },
        { data: "CodigoInternoProduto", title: "Codigo Interno Fornecedor" },
    ];
    const editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarFornecedorProduto, tamanho: "10", icone: "" }
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFornecedorProduto = new BasicDataTable(_fornecedorProduto.Grid.idGrid, headerFornecedorProduto, menuOpcoes, { column: 0, dir: orderDir.asc });
    CarregarGridForneceroProduto(new Array());
}

function CarregarGridForneceroProduto(lista) {
    _gridFornecedorProduto.CarregarGrid(lista);
}

function AdicionarItemGrid() {

    if (!_modalfornecedorProduto.Filial.codEntity() || !_modalfornecedorProduto.Fornecedor.codEntity() || !_modalfornecedorProduto.CodigoProduto.val())
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor preecha os campos");

    const novoRegistroGrid = new Object();
    novoRegistroGrid.Codigo = _modalfornecedorProduto.Codigo.val();
    novoRegistroGrid.CodigoFilial = _modalfornecedorProduto.Filial.codEntity();
    novoRegistroGrid.CodigoFornecedor = _modalfornecedorProduto.Fornecedor.codEntity();
    novoRegistroGrid.Filial = _modalfornecedorProduto.Filial.val();
    novoRegistroGrid.Fornecedor = _modalfornecedorProduto.Fornecedor.val();
    novoRegistroGrid.CodigoInternoProduto = _modalfornecedorProduto.CodigoProduto.val();

    AdicionarRegistroNoProduto(novoRegistroGrid);
    RecarregarGridFornecedorProduto();
    CloseModalFornecedorProduto();
}

function AdicionarRegistroNoProduto(registro) {
    return _produtoEmbarcador.FornecedorProduto.val().push(registro);
}

function RecarregarGridFornecedorProduto() {
    CarregarGridForneceroProduto(_produtoEmbarcador.FornecedorProduto.val());
}

function CloseModalFornecedorProduto() {
    Global.fecharModal("ModalFornecedorProduto");
    LimparTodosOsCamposFornecedor();
}

function LimparTodosOsCamposFornecedor() {
    LimparCampos(_modalfornecedorProduto);
    _modalfornecedorProduto.Cancelar.visible(false);
    _modalfornecedorProduto.Remover.visible(false);
    _modalfornecedorProduto.Atualizar.visible(false);
    _modalfornecedorProduto.Adicionar.visible(true);
}

function ObterListaFornecedoreProduto() {
    return _produtoEmbarcador.FornecedorProduto.val();
}

function EditarFornecedorProduto(itemGrid) {
    LimparTodosOsCamposFornecedor();

    _modalfornecedorProduto.Codigo.val(itemGrid.Codigo);
    _modalfornecedorProduto.Filial.val(itemGrid.Filial);
    _modalfornecedorProduto.Filial.codEntity(itemGrid.CodigoFilial);
    _modalfornecedorProduto.Fornecedor.val(itemGrid.Fornecedor);
    _modalfornecedorProduto.Fornecedor.codEntity(itemGrid.CodigoFornecedor);
    _modalfornecedorProduto.CodigoProduto.val(itemGrid.CodigoInternoProduto);
    _modalfornecedorProduto.Cancelar.visible(true);
    _modalfornecedorProduto.Remover.visible(true);
    _modalfornecedorProduto.Atualizar.visible(true);
    _modalfornecedorProduto.Adicionar.visible(false);

    abrirModalFornecedorProduto();
}

function RemoverRegistroGrid() {
    const listaRegistroGrid = ObterListaFornecedoreProduto();

    $.each(listaRegistroGrid, (index, item) => {

        if (!(item.Codigo == _modalfornecedorProduto.Codigo.val() && item.CodigoInternoProduto == _modalfornecedorProduto.CodigoProduto.val()))
            return true;

        listaRegistroGrid.splice(index, 1);
        return false;
    });

    _produtoEmbarcador.FornecedorProduto.val(listaRegistroGrid);

    RecarregarGridFornecedorProduto();
    CloseModalFornecedorProduto();
}

function AtualizarRegristroGrid() {
    $.each(ObterListaFornecedoreProduto(), (index, item) => {
        if (!(item.Codigo == _modalfornecedorProduto.Codigo.val()))
            return true;

        item.Codigo = _modalfornecedorProduto.Codigo.val();
        item.CodigoFilial = _modalfornecedorProduto.Filial.codEntity();
        item.CodigoFornecedor = _modalfornecedorProduto.Fornecedor.codEntity();
        item.Filial = _modalfornecedorProduto.Filial.val();
        item.Fornecedor = _modalfornecedorProduto.Fornecedor.val();
        item.CodigoInternoProduto = _modalfornecedorProduto.CodigoProduto.val();
        return false;
    });
    RecarregarGridFornecedorProduto();
    CloseModalFornecedorProduto();
}
function SetarProdutoFornecedor() {
    const listRegistroGrid = _gridFornecedorProduto.BuscarRegistros();;
    const listFormatada = new Array();

    $.each(listRegistroGrid, (_, item) => {
        let data = {
            Codigo: item.Codigo,
            CodigoFilial: item.CodigoFilial,
            CodigoFornecedor: item.CodigoFornecedor,
            Fornecedor: item.Fornecedor,
            Filial: item.Filial,
            CodigoInternoProduto: item.CodigoInternoProduto
        }
        listFormatada.push(data);
    })

    return _produtoEmbarcador.FornecedorProduto.val(JSON.stringify(listFormatada));
}
//#endregion
