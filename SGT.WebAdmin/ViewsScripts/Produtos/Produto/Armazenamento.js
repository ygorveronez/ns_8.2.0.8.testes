/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/LocalArmazenamentoProduto.js" />
/// <reference path="Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _armazenamento;
var _tabArmazenamento;
var _gridArmazenamento;
var _gridHistorico;
var _transferenciaArmazenamento;

var TabArmazenamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.NovoLocal = PropertyEntity({ eventClick: NovoLocalArmazenamentoProdutoClick, type: types.event, text: "Novo Local Armazenamento", visible: ko.observable(false) });
};

var Armazenamento = function () {
    this.Historico = PropertyEntity({ type: types.local, visible: ko.observable(true) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Produto = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.QuantidadeAtual = PropertyEntity({ text: "Quantidade Atual: ", getType: typesKnockout.decimal, enable: false });
    this.EstoqueMinimo = PropertyEntity({ text: "Estoque Mínimo: ", getType: typesKnockout.decimal });
    this.EstoqueMaximo = PropertyEntity({ text: "Estoque Máximo: ", getType: typesKnockout.decimal });

    this.QuantidadeEstoqueReservada = PropertyEntity({ text: "Quantidade de Estoque Reservada: ", getType: typesKnockout.decimal });
    this.UltimoCusto = PropertyEntity({ text: "Último Custo: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 4, allowZero: true }, val: ko.observable("") });
    this.CustoMedio = PropertyEntity({ text: "Custo Médio: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 4, allowZero: true }, val: ko.observable("") });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Local de Armazenamento:"), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarArmazenamentoProdutoClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarArmazenamentoProdutoClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });

   
};

var TransferenciaArmazenamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.QuantidadeAtual = PropertyEntity({ text: "*Quantidade Atual: ", getType: typesKnockout.decimal, enable: false });
    this.QuantidadeTransferencia = PropertyEntity({ text: "*Quantidade a ser transferida: ", getType: typesKnockout.decimal, required: true });

    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local de Armazenamento Atual:", idBtnSearch: guid(), enable: ko.observable(false), required: true });
    this.LocalArmazenamentoTransferencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local de Armazenamento Destino:", idBtnSearch: guid(), enable: ko.observable(true), required: true });

    this.Transferir = PropertyEntity({ eventClick: TransferirLocalArmazenamentoProdutoClick, type: types.event, text: "Transferir" });
};

//*******EVENTOS*******

function LoadArmazenamento() {
    _tabArmazenamento = new TabArmazenamento();
    KoBindings(_tabArmazenamento, "knockoutTabArmazenamento");

    _armazenamento = new Armazenamento();
    KoBindings(_armazenamento, "knockoutArmazenamento");

    _transferenciaArmazenamento = new TransferenciaArmazenamento();
    KoBindings(_transferenciaArmazenamento, "knockoutTransferenciaArmazenamento");

    new BuscarEmpresa(_armazenamento.Filial);
    new BuscarLocalArmazenamentoProduto(_armazenamento.LocalArmazenamento);
    new BuscarLocalArmazenamentoProduto(_transferenciaArmazenamento.LocalArmazenamentoTransferencia);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
        _armazenamento.Filial.enable(false);
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        _armazenamento.LocalArmazenamento.visible(true);

    if (_CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento) {
        _armazenamento.LocalArmazenamento.required(true);
        _armazenamento.LocalArmazenamento.visible(true);
        _armazenamento.LocalArmazenamento.text("*Local de Armazenamento:");
        _tabArmazenamento.NovoLocal.visible(true);
    }

    GridArmazenamento();
    GridHistorico();
}

//*******METODOS*******

function AtualizarArmazenamentoProdutoClick(e, sender) {
    Salvar(_armazenamento, "Produto/AtualizarArmazenamentoProduto", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridArmazenamento.CarregarGrid();
                Global.fecharModal('divModalArmazenamento');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function EditarArmazenamentosProdutoClick(data) {
    LimparCampos(_armazenamento);
    _armazenamento.Codigo.val(data.Codigo);
    _armazenamento.Produto.val(_produto.Codigo.val());

    _armazenamento.Filial.codEntity(data.CodigoFilial);
    _armazenamento.Filial.val(data.Filial);
    _armazenamento.LocalArmazenamento.codEntity(data.CodigoLocalArmazenamento);
    _armazenamento.LocalArmazenamento.val(data.LocalArmazenamento);
    
    _armazenamento.QuantidadeAtual.val(data.QuantidadeAtual);
    _armazenamento.EstoqueMinimo.val(data.EstoqueMinimo);
    _armazenamento.EstoqueMaximo.val(data.EstoqueMaximo);
    _armazenamento.QuantidadeEstoqueReservada.val(data.QuantidadeEstoqueReservada);

    _gridHistorico.CarregarGrid();
    _armazenamento.Historico.visible(true);
    _armazenamento.Atualizar.visible(true);
    _armazenamento.Adicionar.visible(false);
    Global.abrirModal('divModalArmazenamento');
}

function EditarArmazenamento() {
    _armazenamento.Produto.val(_produto.Codigo.val());
    _gridArmazenamento.CarregarGrid();
}

function VisibilidadeTranferencia() {
    return _CONFIGURACAO_TMS.UtilizaMultiplosLocaisArmazenamento;
}

function GridArmazenamento() {
    var selecionar = { descricao: "Selecionar", id: guid(), metodo: EditarArmazenamentosProdutoClick };
    var realizarTransferencia = { descricao: "Realizar Transferência", id: guid(), metodo: RealizarTransferenciaArmazenamentoClick, visibilidade: VisibilidadeTranferencia };
    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirArmazenamentoClick, visibilidade: VisibilidadeTranferencia };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [selecionar, realizarTransferencia, excluir] };

    _gridArmazenamento = new GridView(_tabArmazenamento.Grid.id, "Produto/ArmazenamentoProduto", _armazenamento, menuOpcoes);
}

function GridHistorico() {
    _gridHistorico = new GridView(_armazenamento.Historico.id, "Produto/HistoricoProduto", _armazenamento);
}

function excluirArmazenamentoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o local de armazenamento " + data.LocalArmazenamento + "?", function () {
        executarReST("Produto/ExcluirLocalArmazenamentoProduto", { Codigo: data.Codigo }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridArmazenamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function AdicionarArmazenamentoProdutoClick(e, sender) {
    Salvar(_armazenamento, "Produto/AdicionarLocalArmazenamentoProduto", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");
                _gridArmazenamento.CarregarGrid();
                Global.fecharModal('divModalArmazenamento');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function NovoLocalArmazenamentoProdutoClick() {
    LimparCampos(_armazenamento);
    _armazenamento.Produto.val(_produto.Codigo.val());
    _armazenamento.Historico.visible(false);
    _armazenamento.Atualizar.visible(false);
    _armazenamento.Adicionar.visible(true);
    Global.abrirModal('divModalArmazenamento');
}

function RealizarTransferenciaArmazenamentoClick(data) {
    LimparCampos(_transferenciaArmazenamento);
    _transferenciaArmazenamento.Codigo.val(data.Codigo);
    _transferenciaArmazenamento.LocalArmazenamento.codEntity(data.CodigoLocalArmazenamento);
    _transferenciaArmazenamento.LocalArmazenamento.val(data.LocalArmazenamento);
    _transferenciaArmazenamento.QuantidadeAtual.val(data.QuantidadeAtual);
    Global.abrirModal('divModalTransferenciaArmazenamento');
}

function TransferirLocalArmazenamentoProdutoClick() {
    Salvar(_transferenciaArmazenamento, "Produto/TransferirLocalArmazenamentoProduto", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Transferido com sucesso");
                _gridArmazenamento.CarregarGrid();
                Global.fecharModal('divModalTransferenciaArmazenamento');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AtualizarCamposProduto() {
    _produto.CustoMedio.val(_armazenamento.CustoMedio.val());
    _produto.UltimoCusto.val(_armazenamento.UltimoCusto.val());
}