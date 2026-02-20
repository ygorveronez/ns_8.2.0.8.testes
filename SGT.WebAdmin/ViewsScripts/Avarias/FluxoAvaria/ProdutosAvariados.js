/// <reference path="EtapasFluxoAvaria.js" />
/// <reference path="Anexos.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAvaria.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFluxoAvaria.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Deposito.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridProdutos;
var _fluxoAvaria;
var _CRUDAvaria;
var _pesquisaNotas;
var _gridNotas;
var _modalInformarLocalArmazenamento;

var Notas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ListaNotas = PropertyEntity({ type: types.map, required: false, text: "Adicionar Notas", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
    this.ListaProdutos = PropertyEntity({ type: types.entity, required: false, text: "Produtos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

var InformarLocalArmazenamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAvaria = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local Armazenamento:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    
    this.Enviar = PropertyEntity({
        eventClick: function (e, sender) {
            if (ValidarCamposObrigatorios(_informarLocalArmazenamento))
                if (enviarLocalArmazenamento(_informarLocalArmazenamento, sender))
                    _modalInformarLocalArmazenamento.hide();
        }, type: types.event, text: "Enviar", idGrid: guid(), visible: ko.observable(true)
    });
};

var InformarProdutoEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoAvaria = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto Embarcador:", idBtnSearch: guid(), required: true, visible: ko.observable(true) });

    this.Enviar = PropertyEntity({
        eventClick: function (e, sender) {
            if (ValidarCamposObrigatorios(_informarProdutoEmbarcador))
                if (enviarProdutoEmbarcador(_informarProdutoEmbarcador, sender))
                    _modalInformarProdutoEmbarcador.hide();
        }, type: types.event, text: "Enviar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadProdutosAvariados() {
    _notas = new Notas();
    KoBindings(_notas, "knockoutPesquisaNotas");
    _notas.Codigo.val(_fluxoAvaria.Codigo.val());

    _modalInformarLocalArmazenamento = new bootstrap.Modal(document.getElementById("divInformarLocalArmazenamento"), { backdrop: true, keyboard: true });
    _modalInformarProdutoEmbarcador = new bootstrap.Modal(document.getElementById("divInformarProdutoEmbarcador"), { backdrop: true, keyboard: true });
}

function notasCallback(notas) {
    var notasGrid = _gridNotas.BuscarRegistros();
    var notasAtualizado = notasGrid !== undefined && notasGrid != null ? notasGrid.concat(notas) : notas;

    _gridNotas.CarregarGrid(notasAtualizado);
    gravarProdutos();
}


//*******MÉTODOS*******
function gridProdutosAvariados() {
    var LocalArmazenamento = { descricao: "Local de Armazenamento", id: guid(), metodo: editarLocalArmazenamento, icone: "", visibilidade: true };
    var ProdutoEmbarcador = { descricao: "Produto Embarcador", id: guid(), metodo: editarProdutoEmbarcador, icone: "", visibilidade: true };
    var Excluir = { descricao: "Excluir", id: guid(), metodo: excluirProduto, icone: "", visibilidade: true };
    var editarColuna = null;

    var menuOpcoes= {
        tipo: TypeOptionMenu.list,
        opcoes: []
    };
    
    if (_fluxoAvaria.Codigo.val() > 0 && _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos) {
        editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
        menuOpcoes.opcoes = [LocalArmazenamento, ProdutoEmbarcador, Excluir];
    }
        
    _gridProdutos = new GridView(_notas.ListaProdutos.idGrid, "FluxoAvaria/ProdutosAvariadosGrid", _fluxoAvaria, menuOpcoes, null, null, null, null, null, null, null, editarColuna);
    _gridProdutos.CarregarGrid();
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, Avaria: _fluxoAvaria.Codigo.val(), Quantidade: dataRow.Quantidade, Unidades: dataRow.Unidades, Valor: dataRow.Valor, GeraEstoque: dataRow.GeraEstoque };

    executarReST("FluxoAvaria/AtualizarCamposProduto", data, function (arg) {
        if (arg.Success) {
            if (arg.Data === false) {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
            AtualizarValorAvaria();
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function gravarProdutos() {
    var dados = {
        Codigo: _fluxoAvaria.Codigo.val(),
        Notas: obterListaNotasSalvar()
    }
    executarReST("/FluxoAvaria/AdicionarProduto", dados, function (r) {
        if (r.Success) {
            _gridProdutos.CarregarGrid();
            AtualizarValorAvaria();
        } else {
            
        }
    });
}

function excluirProduto(registroSelecionado) {
    var dados = {
        Codigo: registroSelecionado.Codigo,
        Avaria: _fluxoAvaria.Codigo.val()
    }
    
    executarReST("/FluxoAvaria/ExcluirProduto", dados, function (r) {
        if (r.Success) {
            _gridProdutos.CarregarGrid();
            obterNotasPorProdutos();
            AtualizarValorAvaria();
        } else {

        }
    });
}

function atualizarNotas(notas, produtos) {
    for (var i = 0; i < notas.length; i++) {
        if (produtos.find(p => p.NFe == notas[i].Numero) === undefined) {
            notas.splice(i, 1);
            break
        }
    }
    _gridNotas.CarregarGrid(notas);
}


function loadGridNotas() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerNota, icone: "" };
    var menuOpcoes = null;
    
    if (_fluxoAvaria.Codigo.val() > 0 && _fluxoAvaria.SituacaoFluxo.val() == EnumSituacaoFluxoAvaria.Produtos)
        menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", visible: false },
        { data: "Numero", title: "Número", visible: true },
    ];

    _gridNotas = new BasicDataTable(_notas.ListaNotas.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    
    new BuscarXMLNotaFiscal(_notas.ListaNotas, notasCallback, _fluxoAvaria.Carga, _gridNotas);
    _notas.ListaNotas.basicTable = _gridNotas;

    _gridNotas.CarregarGrid([]);
}

function obterListaNotas() {
    return _gridNotas.BuscarRegistros();
}

function obterListaNotasSalvar() {
    var listaNotas = obterListaNotas();
    var listaNotasRetornar = new Array();

    listaNotas.forEach(function (nota) {
        listaNotasRetornar.push(Number(nota.Codigo))
    });

    return JSON.stringify(listaNotasRetornar);
}

function removerNota(registroSelecionado) {
    var listaNotas = obterListaNotas();
    var dados = {
        Codigo: _fluxoAvaria.Codigo.val(),
        Nota: registroSelecionado.Codigo
    }

    executarReST("/FluxoAvaria/ExcluirNota", dados, function (r) {
        if (r.Success) {
            for (var i = 0; i < listaNotas.length; i++) {
                if (registroSelecionado.Codigo == listaNotas[i].Codigo) {
                    listaNotas.splice(i, 1);
                    break;
                }
            }
            _gridNotas.CarregarGrid(listaNotas);
            obterNotasPorProdutos();
            _gridProdutos.CarregarGrid();
            AtualizarValorAvaria();
        } else {

        }
    });
}

function obterNotasPorProdutos() {
    var listaNotas = [];
    var dados = {
        Codigo: _fluxoAvaria.Codigo.val()
    }
    
    executarReST("/FluxoAvaria/BuscarNotasProdutos", dados, function (r) {
        if (r.Success) {
            listaProdutos = r.Data;
            listaProdutos.forEach(function (produto) {
                var nota = { Codigo: produto.Codigo.toString(), DT_RowId: produto.Codigo, DT_Enable: true, Descricao: produto.NFe, Numero: produto.NFe };
                if (listaNotas.find(n => n.Numero === nota.Numero) === undefined)
                    listaNotas.push(nota)
            });
            _gridNotas.CarregarGrid(listaNotas);
        }
    });

}

function editarLocalArmazenamento(registroSelecionado) {
    _informarLocalArmazenamento = new InformarLocalArmazenamento()
    KoBindings(_informarLocalArmazenamento, "knockoutInformarLocalArmazenamento", false, _informarLocalArmazenamento.Enviar.id);

    new BuscarDeposito(_informarLocalArmazenamento.LocalArmazenamento);

    _informarLocalArmazenamento.Codigo.val(registroSelecionado.Codigo);
    _informarLocalArmazenamento.CodigoAvaria.val(_fluxoAvaria.Codigo.val());
    _modalInformarLocalArmazenamento.show();
}

function enviarLocalArmazenamento(knockout, sender) {
    Salvar(knockout, "FluxoAvaria/AtualizarLocalArmazenamento", function (retorno) {
        if (retorno.Success) {
            _gridProdutos.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, sender);
    return true;
}

function editarProdutoEmbarcador(registroSelecionado) {
    _informarProdutoEmbarcador = new InformarProdutoEmbarcador()
    KoBindings(_informarProdutoEmbarcador, "knockoutInformarProdutoEmbarcador", false, _informarProdutoEmbarcador.Enviar.id);

    new BuscarProdutos(_informarProdutoEmbarcador.ProdutoEmbarcador);

    _informarProdutoEmbarcador.Codigo.val(registroSelecionado.Codigo);
    _informarProdutoEmbarcador.CodigoAvaria.val(_fluxoAvaria.Codigo.val());
    _modalInformarProdutoEmbarcador.show();
}

function enviarProdutoEmbarcador(knockout, sender) {
    Salvar(knockout, "FluxoAvaria/AtualizarProdutoEmbarcador", function (retorno) {
        if (retorno.Success) {
            _gridProdutos.CarregarGrid();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, sender);
    return true;
}

function ExibirErroDataRow(row, mensagem, tipoMensagem, titulo) {
    _gridProdutos.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}
