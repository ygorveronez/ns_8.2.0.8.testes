/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Enumeradores/EnumDestinoProdutoAvaria.js" />
/// <reference path="DestinoAvarias.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _destinoAvariasProduto;
var _CRUDDestinoAvariasProduto;
var _gridDestinoAvariasProduto;

var DestinoAvariasProduto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoLote = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Destino = PropertyEntity({ text: "*Destino:", val: ko.observable(EnumDestinoProdutoAvaria.Descartada), options: EnumDestinoProdutoAvaria.obterOpcoes(), def: EnumDestinoProdutoAvaria.Descartada, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.int, text: "*Quantidade:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: "*Data de Vencimento:", val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto: ", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga: ", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente: ", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista: ", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento: ", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });

    this.DestinosAvariasProduto = PropertyEntity({ idGrid: guid() });

    this.Destino.val.subscribe(function (novoValor) {
        ControleCamposDestinoAvariasProduto(novoValor);
    });
};

var CRUDDestinoAvariasProduto = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarDestinoAvariasProdutoClick, type: types.event, text: "Adicionar", enable: ko.observable(true), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarDestinoAvariasProdutoClick, type: types.event, text: "Atualizar", enable: ko.observable(true), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirDestinoAvariasProdutoClick, type: types.event, text: "Excluir", enable: ko.observable(true), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarDestinoAvariasProdutoClick, type: types.event, text: "Cancelar", enable: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadDestinoAvariasProduto() {
    _destinoAvariasProduto = new DestinoAvariasProduto();
    KoBindings(_destinoAvariasProduto, "knockoutDestinoAvariasProduto");

    _CRUDDestinoAvariasProduto = new CRUDDestinoAvariasProduto();
    KoBindings(_CRUDDestinoAvariasProduto, "knockoutCRUDDestinoAvariasProduto");

    new BuscarProdutos(_destinoAvariasProduto.Produto);
    new BuscarCargas(_destinoAvariasProduto.Carga);
    new BuscarClientes(_destinoAvariasProduto.Cliente);
    new BuscarMotoristas(_destinoAvariasProduto.Motorista);
    new BuscarTipoMovimento(_destinoAvariasProduto.TipoMovimento);

    BuscarDestinoAvariasProduto();
}

function AdicionarDestinoAvariasProdutoClick(e, sender) {
    Salvar(_destinoAvariasProduto, "LotesDestinoAvaria/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Produto adicionado com sucesso");
                _gridDestinoAvariasProduto.CarregarGrid();
                LimparCamposDestinoAvariasProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarDestinoAvariasProdutoClick(e, sender) {
    Salvar(_destinoAvariasProduto, "LotesDestinoAvaria/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Produto atualizado com sucesso");
                _gridDestinoAvariasProduto.CarregarGrid();
                LimparCamposDestinoAvariasProduto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirDestinoAvariasProdutoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Produto " + _destinoAvariasProduto.Produto.val() + "?", function () {
        ExcluirPorCodigo(_destinoAvariasProduto, "LotesDestinoAvaria/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Produto excluído com sucesso");
                    _gridDestinoAvariasProduto.CarregarGrid();
                    LimparCamposDestinoAvariasProduto();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Sugestão", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function CancelarDestinoAvariasProdutoClick(e) {
    LimparCamposDestinoAvariasProduto();
}

////*******MÉTODOS*******

function CarregarDadosDestinoAvariasProduto() {
    LimparCamposDestinoAvariasProduto();
    _gridDestinoAvariasProduto.CarregarGrid();

    if (_lote.Situacao.val() === EnumSituacaoLote.FinalizadaComDestino) {
        SetarEnableCamposKnockout(_destinoAvariasProduto, false);
        SetarEnableCamposKnockout(_CRUDDestinoAvariasProduto, false);
    }
}

function BuscarDestinoAvariasProduto() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarDestinoAvariasProduto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDestinoAvariasProduto = new GridView(_destinoAvariasProduto.DestinosAvariasProduto.idGrid, "LotesDestinoAvaria/Pesquisa", _destinoAvariasProduto, menuOpcoes);
}

function EditarDestinoAvariasProduto(itemGrid) {
    LimparCamposDestinoAvariasProduto();
    _destinoAvariasProduto.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_destinoAvariasProduto, "LotesDestinoAvaria/BuscarPorCodigo", function (arg) {
        _CRUDDestinoAvariasProduto.Atualizar.visible(true);
        _CRUDDestinoAvariasProduto.Excluir.visible(true);
        _CRUDDestinoAvariasProduto.Adicionar.visible(false);
        _destinoAvariasProduto.Produto.enable(false);
    }, null);
}

function ControleCamposDestinoAvariasProduto(novoValor) {
    _destinoAvariasProduto.Carga.required(false);
    _destinoAvariasProduto.Carga.visible(false);
    _destinoAvariasProduto.Cliente.required(false);
    _destinoAvariasProduto.Cliente.visible(false);
    _destinoAvariasProduto.Motorista.required(false);
    _destinoAvariasProduto.Motorista.visible(false);
    _destinoAvariasProduto.TipoMovimento.required(false);
    _destinoAvariasProduto.TipoMovimento.visible(false);
    _destinoAvariasProduto.Valor.required(false);
    _destinoAvariasProduto.Valor.visible(false);
    _destinoAvariasProduto.DataVencimento.required(false);
    _destinoAvariasProduto.DataVencimento.visible(false);

    if (novoValor === EnumDestinoProdutoAvaria.Descartada) {
        LimparCampoEntity(_destinoAvariasProduto.Carga);
        LimparCampoEntity(_destinoAvariasProduto.Cliente);
        LimparCampoEntity(_destinoAvariasProduto.Motorista);
        LimparCampoEntity(_destinoAvariasProduto.TipoMovimento);
    }
    else if (novoValor === EnumDestinoProdutoAvaria.Vendida) {
        _destinoAvariasProduto.Cliente.required(true);
        _destinoAvariasProduto.Cliente.visible(true);
        _destinoAvariasProduto.TipoMovimento.required(true);
        _destinoAvariasProduto.TipoMovimento.visible(true);
        LimparCampoEntity(_destinoAvariasProduto.Carga);
        LimparCampoEntity(_destinoAvariasProduto.Motorista);

        _destinoAvariasProduto.Valor.required(true);
        _destinoAvariasProduto.Valor.visible(true);
        _destinoAvariasProduto.DataVencimento.required(true);
        _destinoAvariasProduto.DataVencimento.visible(true);
        LimparCampo(_destinoAvariasProduto.Valor);
        LimparCampo(_destinoAvariasProduto.DataVencimento);
    }
    else if (novoValor === EnumDestinoProdutoAvaria.DescontadaMotorista) {
        _destinoAvariasProduto.Motorista.required(true);
        _destinoAvariasProduto.Motorista.visible(true);
        _destinoAvariasProduto.TipoMovimento.required(true);
        _destinoAvariasProduto.TipoMovimento.visible(true);
        LimparCampoEntity(_destinoAvariasProduto.Carga);
        LimparCampoEntity(_destinoAvariasProduto.Cliente);

        _destinoAvariasProduto.Valor.required(true);
        _destinoAvariasProduto.Valor.visible(true);
        _destinoAvariasProduto.DataVencimento.required(true);
        _destinoAvariasProduto.DataVencimento.visible(true);
        LimparCampo(_destinoAvariasProduto.Valor);
        LimparCampo(_destinoAvariasProduto.DataVencimento);
    }
    else if (novoValor === EnumDestinoProdutoAvaria.DevolvidaCliente) {
        _destinoAvariasProduto.Carga.required(true);
        _destinoAvariasProduto.Carga.visible(true);
        LimparCampoEntity(_destinoAvariasProduto.Cliente);
        LimparCampoEntity(_destinoAvariasProduto.Motorista);
        LimparCampoEntity(_destinoAvariasProduto.TipoMovimento);
    }
}

function LimparCamposDestinoAvariasProduto() {
    LimparCampos(_destinoAvariasProduto);
    _destinoAvariasProduto.CodigoLote.val(_lote.Codigo.val());

    _CRUDDestinoAvariasProduto.Adicionar.visible(true);
    _CRUDDestinoAvariasProduto.Atualizar.visible(false);
    _CRUDDestinoAvariasProduto.Excluir.visible(false);

    if (_lote.Situacao.val() !== EnumSituacaoLote.FinalizadaComDestino) {
        _destinoAvariasProduto.Produto.enable(true);
        SetarEnableCamposKnockout(_destinoAvariasProduto, true);
        SetarEnableCamposKnockout(_CRUDDestinoAvariasProduto, true);
    }
}