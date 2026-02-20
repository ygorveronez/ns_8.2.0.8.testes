var _gridProdutos;
var _cargaAcaoParcial;
var _listaProdutosAcaoParcial;

var editable = {
    editable: true,
    type: EnumTipoColunaEditavelGrid.int,
    numberMask: ConfigInt()
};

var editarColuna = {
    permite: true,
    atualizarRow: true,
    callback: linhaAlterada,
};

var CargaAcaoParcial = function () {
    this.Titulo = PropertyEntity({ text: ko.observable("") });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.CodigoPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Quantidade = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.int, val: ko.observable(0), required: true, enable: ko.observable(true)});
    this.Produtos = PropertyEntity({ text: ko.observable(""), type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Entrega = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });

    this.Salvar = PropertyEntity({ eventClick: salvarAcaoParcialClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: fecharAcaoParcialClick, type: types.event, text: "Fechar", visible: ko.observable(false) });
}

function loadCargaAcaoParcial() {
    _cargaAcaoParcial = new CargaAcaoParcial();
    KoBindings(_cargaAcaoParcial, "knockoutInformarAcaoParcialCarga");

    loadGridProdutosAcaoParcial();
}

function exibirModalCargaAcaoParcial(dados, entrega) {
    if (entrega) {
        _cargaAcaoParcial.Quantidade.text("Quantidade não entregues: ");
        _cargaAcaoParcial.Produtos.text("Quantidade não entregues por Produto: ");
    } else {
        _cargaAcaoParcial.Quantidade.text("Quantidade devolvidas: ");
        _cargaAcaoParcial.Produtos.text("Quantidade devolvidas por Produtos: ");
    }
    _cargaAcaoParcial.Titulo.text("Informar Ação Parcial");
    _cargaAcaoParcial.Codigo.val(dados.Codigo);
    _cargaAcaoParcial.CodigoCarga.val(dados.CodigoCarga);
    _cargaAcaoParcial.CodigoPedido.val(dados.CodigoPedido);
    _cargaAcaoParcial.Entrega.val(entrega);

    _cargaAcaoParcial.Quantidade.enable(true);
    _cargaAcaoParcial.Salvar.visible(true);
    _cargaAcaoParcial.Fechar.visible(false);

    Global.abrirModal('divModalCargaAcaoParcial');

    $("#divModalCargaAcaoParcial").on('hidden.bs.modal', function () {
        LimparCampos(_cargaAcaoParcial);
    });
    RecarregarGridProdutosAcaoParcial();
}

function salvarAcaoParcialClick() {
    var data = RetornarObjetoPesquisa(_cargaAcaoParcial);
    data["Produtos"] = JSON.stringify(_listaProdutosAcaoParcial);

    executarReST("JanelaDescarga/InformarAcaoParcial", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "O status foi atualizado!");
                Global.fecharModal('divModalCargaAcaoParcial');
                _tabelaDescarregamento.Load();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function buscarPedidoProdutosDescargaParcial(dados, entrega) {
    executarReST("JanelaDescarga/BuscarPedidoProdutosDescargaParcial", { CodigoCarga: dados.CodigoCarga, CodigoPedido: dados.CodigoPedido }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaProdutosAcaoParcial = retorno.Data;
                exibirModalCargaAcaoParcial(dados, entrega);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function loadGridProdutosAcaoParcial() {
    _listaProdutosAcaoParcial = new Array();

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "Removido", visible: false },
        { data: "Descricao", title: "Produto", width: "30%" },
        { data: "CodigoEmbarcador", title: "Código Produto", width: "15%" },
        { data: "Setor", title: "Setor (Grupo)", width: "10%" },
        { data: "Quantidade", title: "Quantidade", width: "15%", editableCell: editable },
        { data: "QuantidadeOriginal", title: "Quantidade Original", width: "15%" }
    ];
    _gridProdutos = new BasicDataTable(_cargaAcaoParcial.Produtos.idGrid, header, null, null, null, 10, null, null, editarColuna);
    RecarregarGridProdutosAcaoParcial();
}
function RecarregarGridProdutosAcaoParcial() {
    var data = new Array();
    $.each(_listaProdutosAcaoParcial, function (i, produto) {
        data.push(produto);
    });
    _gridProdutos.CarregarGrid(data);
}

function linhaAlterada(registro) {

    if (registro.Quantidade == registro.QuantidadeOriginal)
        registro.DT_RowColor = "";
    else
        registro.DT_RowColor = "#fffce8";

    RecarregarGridProdutosAcaoParcial();
}
function linhaAlteradaVermelho(registro) {
    registro.DT_RowColor = "#ebc3c3";
    for (var i = 0; i < _listaProdutosAcaoParcial.length; i++) {
        if (_listaProdutosAcaoParcial[i].Codigo == registro.Codigo) {
            _listaProdutosAcaoParcial[i].Removido = true;
            break;
        }
    }
    RecarregarGridProdutosAcaoParcial();
}

function linhaAlteradaBranco(registro) {
    registro.DT_RowColor = "";
    for (var i = 0; i < _listaProdutosAcaoParcial.length; i++) {
        if (_listaProdutosAcaoParcial[i].Codigo == registro.Codigo) {
            _listaProdutosAcaoParcial[i].Removido = false;
            break;
        }
    }
    RecarregarGridProdutosAcaoParcial();
}


function ExcluirProdutoAgendamentoColetaClick(registroSelecionado) {
    registroSelecionado.QuantidadeAgendada = 0;
    linhaAlteradaVermelho(registroSelecionado)
}

function AdicionarProdutoAgendamentoColetaClick(registroSelecionado) {
    registroSelecionado.Quantidade = registroSelecionado.QuantidadeOriginal;
    linhaAlteradaBranco(registroSelecionado)
}
function isRemovido(registro) {
    return registro.Removido == true;
}

function isNotRemovido(registro) {
    return (!(registro.Removido == true));
}

function visualizarPedidoProdutosDescargaParcial(dados) {
    executarReST("JanelaDescarga/VisualizarPedidoProdutosDescargaParcial", { CodigoCarga: dados.CodigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaProdutosAcaoParcial = retorno.Data.Produtos;
                exibirModalCargaAcaoParcial(dados, retorno.Data.EntregueParcial);
                _cargaAcaoParcial.Quantidade.enable(false);
                _cargaAcaoParcial.Quantidade.val(retorno.Data.QuantidadeNaoEntregue);
                _cargaAcaoParcial.Salvar.visible(false);
                _cargaAcaoParcial.Fechar.visible(true);
                _cargaAcaoParcial.Titulo.text("Carga " + (retorno.Data.EntregueParcial ? "Entregue" : "Devolvida") + " Parcialmente");
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function fecharAcaoParcialClick() {
    Global.fecharModal('divModalCargaAcaoParcial');
}