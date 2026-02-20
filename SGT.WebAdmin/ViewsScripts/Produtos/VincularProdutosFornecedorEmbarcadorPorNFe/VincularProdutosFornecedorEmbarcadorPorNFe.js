/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/knockout/knockout-3.3.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _importacaoXMLNFe;
var _produtosNFe;
var _gridProdutos;

var ImportacaoXMLNFe = function () {
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "XML NF-e:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _importacaoXMLNFe.NomeArquivo.val(nomeArquivo);
        
        if (nomeArquivo != "")
            carregarXML();
    });

    this.Limpar = PropertyEntity({ type: types.event, eventClick: limparClick, text: "Limpar", visible: ko.observable(true) });
};

var ProdutosNFe = function () {
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid() });
    this.Produtos = PropertyEntity({ val: ko.observable(new Array()), def: new Array() });
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
};

function loadVincularProdutosFornecedorEmbarcadorPorNFe() {
    _importacaoXMLNFe = new ImportacaoXMLNFe();
    KoBindings(_importacaoXMLNFe, "knockoutImportacaoXMLNFe");

    _produtosNFe = new ProdutosNFe();
    KoBindings(_produtosNFe, "knockoutProdutosNFe");
    
    loadGridProdutos();
    loadVincularProdutoFornecedor();
}

function carregarXML() {
    var arquivo = document.getElementById(_importacaoXMLNFe.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        _importacaoXMLNFe.Arquivo.val("");
        _importacaoXMLNFe.NomeArquivo.val("");
        return;
    }

    if (!validFileType(arquivo.files[0])) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O tipo do arquivo não é permitido.");
        _importacaoXMLNFe.Arquivo.val("");
        _importacaoXMLNFe.NomeArquivo.val("");
        return;
    }

    var formData = new FormData();

    formData.append("Arquivo", arquivo.files[0]);
    formData.append("Descricao", _importacaoXMLNFe.NomeArquivo.val());

    enviarArquivo("VincularProdutosFornecedorEmbarcadorPorNFe/CarregarArquivoXML", {}, formData, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_produtosNFe, r);
                _gridProdutos.CarregarGrid(r.Data.Produtos);
                _produtosNFe.Produtos.val(r.Data.Produtos);
                _produtosNFe.Grid.visible(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function validFileType(file) {
    var acceptTypes = ["text/xml"];

    return acceptTypes.includes(file.type)
}

function loadGridProdutos() {
    var opcaoVincular = { descricao: "Vincular", id: guid(), metodo: vincularProdutoClick, icone: "", visibilidade: visibilidadeVincularProduto };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarProdutoClick, icone: "", visibilidade: visibilidadeEditarProduto };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [opcaoVincular, opcaoEditar] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Incluso", visible: false },
        { data: "ProdutoEmbarcadorCodigo", visible: false },
        { data: "Filiais", visible: false },
        { data: "CodigoProdutoFornecedor", title: "Cód. Fornecedor", width: "10%", className: "text-align-left" },
        { data: "DescricaoProdutoFornecedor", title: "Descrição", width: "20%", className: "text-align-left" },
        { data: "Status", title: "Status", width: "8%", className: "text-align-left" },
        { data: "CodigoProdutoEmbarcador", title: "Cód. Embarcador", width: "12%", className: "text-align-left" },
        { data: "DescricaoProdutoEmbarcador", title: "Descrição Embarcador", width: "20%", className: "text-align-left" },
        { data: "DescricoesFiliais", title: "Filial", width: "20%", className: "text-align-left" }
    ];

    _gridProdutos = new BasicDataTable(_produtosNFe.Grid.idGrid, header, menuOpcoes, null, null, 10);
    _gridProdutos.CarregarGrid([]);
}

function visibilidadeVincularProduto(data) {
    return !data.Incluso;
}

function visibilidadeEditarProduto(data) {
    return data.Incluso;
}

function vincularProdutoClick(data) {
    abrirModalVincularProduto(data);
}

function editarProdutoClick(data) {
    abrirModalVincularProduto(data);
}

function limparClick() {
    LimparCampos(_importacaoXMLNFe);
    _importacaoXMLNFe.Arquivo.val("");
    LimparCampos(_produtosNFe);
    _gridProdutos.CarregarGrid([]);
    _produtosNFe.Produtos.val([]);
    _produtosNFe.Grid.visible(false);
}