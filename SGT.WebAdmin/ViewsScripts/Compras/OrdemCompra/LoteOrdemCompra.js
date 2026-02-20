/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/PedidoVenda.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/MotivoCompra.js" />
/// <reference path="Mercadorias.js" />
/// <reference path="Qualificao.js" />
/// <reference path="Load.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _loteOrdemCompra;
var _formatoLoteOrdemCompra;
var _crudLoteOrdemCompra;
var usuarioLogadoLoteOC = null;

var _enumRepeticaoLote = [
    { text: "Semanal", value: 1 },
    { text: "Mensal", value: 2 },
    { text: "Anual", value: 3 }
];

var LoteOrdemCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Requisicoes = PropertyEntity({ val: ko.observable(""), def: "" });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, enable: false });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor: ", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.ExigeInformarVeiculoObrigatoriamente = PropertyEntity({ getType: typesKnockout.bool });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo: "), idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Motorista: "), idBtnSearch: guid(), required: false, enable: ko.observable(true) });

    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataPrevistaRetorno = PropertyEntity({ text: "*Data Prev. Retorno: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });

    this.Operador = PropertyEntity({ type: types.map, text: "*Operador: ", idBtnSearch: guid(), enable: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador: ", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", getType: typesKnockout.map, enable: ko.observable(true), maxlength: 2000 });
    this.CondicaoPagamento = PropertyEntity({ text: "Condição de Pagamento: ", getType: typesKnockout.map, enable: ko.observable(true), maxlength: 2000 });

    this.Produtos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Qualificacao = PropertyEntity({ eventClick: QualificacaoLoteOCClick, type: types.event, text: "Qualificação" });

    this.ValorTotal = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", required: ko.observable(false), text: ko.observable("Valor Total das Mercadorias:"), maxlength: 18, visible: ko.observable(true), enable: ko.observable(false) });

    this.Veiculo.codEntity.subscribe(VerificarObrigatoriedadeVeiculoLoteOC);
};

var FormatoLoteOrdemCompra = function () {
    this.Repeticao = PropertyEntity({ text: "*Repetição: ", options: _enumRepeticaoLote, val: ko.observable(2), def: 2, required: true });
    this.NumeroOrdens = PropertyEntity({ text: "*Número de Ordens: ", getType: typesKnockout.int, val: ko.observable(""), def: "", required: true, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.DiaOrdem = PropertyEntity({ text: "*Dia da Ordem: ", getType: typesKnockout.int, val: ko.observable(""), def: "", required: ko.observable(true), visible: ko.observable(true), configInt: { precision: 0, allowZero: false, thousands: "" } });

    this.Repeticao.val.subscribe(function (novoValor) {
        if (novoValor == 1) {
            _formatoLoteOrdemCompra.DiaOrdem.required(false);
            _formatoLoteOrdemCompra.DiaOrdem.visible(false);
        } else {
            _formatoLoteOrdemCompra.DiaOrdem.required(true);
            _formatoLoteOrdemCompra.DiaOrdem.visible(true);
        }
    });
};

var CRUDLoteOrdemCompra = function () {
    this.Fechar = PropertyEntity({ eventClick: FecharLoteOrdemCompra, type: types.event, text: "Fechar", visible: ko.observable(true) });
    this.Gerar = PropertyEntity({ eventClick: GerarLoteOrdemCompra, type: types.event, text: "Gerar", visible: ko.observable(true) });
};

function LoadLoteOrdemCompra() {
    _loteOrdemCompra = new LoteOrdemCompra();
    KoBindings(_loteOrdemCompra, "knockoutLoteOrdemCompra");

    LoadLoteOCMercadorias();

    _formatoLoteOrdemCompra = new FormatoLoteOrdemCompra();
    KoBindings(_formatoLoteOrdemCompra, "knockoutFormatoLoteOrdemCompra");

    _crudLoteOrdemCompra = new CRUDLoteOrdemCompra();
    KoBindings(_crudLoteOrdemCompra, "knockoutCRUDLoteOrdemCompra");

    new BuscarClientes(_loteOrdemCompra.Fornecedor);
    new BuscarFuncionario(_loteOrdemCompra.Operador);
    new BuscarClientes(_loteOrdemCompra.Transportador);
    new BuscarMotivoCompra(_loteOrdemCompra.Motivo, ValidarVeiculoLoteOC);
    new BuscarVeiculos(_loteOrdemCompra.Veiculo, RetornoVeiculoLoteOC);
    new BuscarMotoristas(_loteOrdemCompra.Motorista);
    PreencherUsuarioLogadoLoteOC();
}

function PreencherUsuarioLogadoLoteOC() {
    var _fillName = function () {
        _loteOrdemCompra.Operador.val(usuarioLogadoLoteOC.Nome);
    }

    if (usuarioLogadoLoteOC != null) return _fillName();

    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                usuarioLogadoLoteOC = {
                    Nome: arg.Data.Nome
                };
                _fillName();
            }
        }
    });
}

function ValidarVeiculoLoteOC(e) {
    _loteOrdemCompra.Motivo.val(e.Descricao);
    _loteOrdemCompra.Motivo.codEntity(e.Codigo);
    _loteOrdemCompra.ExigeInformarVeiculoObrigatoriamente.val(e.ExigeInformarVeiculoObrigatoriamente);

    if (e.ExigeInformarVeiculoObrigatoriamente) {
        _loteOrdemCompra.Veiculo.text("*Veículo: ");
        _loteOrdemCompra.Veiculo.required = true;
    }
    else {
        _loteOrdemCompra.Veiculo.text("Veículo: ");
        _loteOrdemCompra.Veiculo.required = false;
    }
}

function RetornoVeiculoLoteOC(e) {
    _loteOrdemCompra.Veiculo.val(e.Descricao);
    _loteOrdemCompra.Veiculo.codEntity(e.Codigo);
    _loteOrdemCompra.Motorista.val(e.Motorista);
    _loteOrdemCompra.Motorista.codEntity(e.CodigoMotorista);
}

function VerificarObrigatoriedadeVeiculoLoteOC() {
    if (_loteOrdemCompra.ExigeInformarVeiculoObrigatoriamente.val()) {
        _loteOrdemCompra.Veiculo.text("*Veículo: ");
        _loteOrdemCompra.Veiculo.required = true;
    }
    else {
        _loteOrdemCompra.Veiculo.text("Veículo: ");
        _loteOrdemCompra.Veiculo.required = false;
    }
}

function GerarLoteOrdemCompra() {
    var valido = true;

    if (!ValidarCamposObrigatorios(_loteOrdemCompra) || !ValidarCamposObrigatorios(_formatoLoteOrdemCompra)) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    } else if (_formatoLoteOrdemCompra.NumeroOrdens.val() == 1) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Informação inválida", "Número de ordens inválido");
    } else if (_formatoLoteOrdemCompra.Repeticao.val() != 1 && _formatoLoteOrdemCompra.DiaOrdem.val() > 31) {
            valido = false;
            exibirMensagem(tipoMensagem.atencao, "Informação inválida", "Dia da ordem inválido");
    }

    if (valido) {
        var dados = RetornarObjetoPesquisa(_loteOrdemCompra);

        dados.Repeticao = _formatoLoteOrdemCompra.Repeticao.val();
        dados.NumeroOrdens = _formatoLoteOrdemCompra.NumeroOrdens.val();
        dados.DiaOrdem = _formatoLoteOrdemCompra.DiaOrdem.val();

        executarReST("OrdemCompra/LoteOrdemCompra", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Gerado lote de ordens de compras com sucesso");
                    FecharLoteOrdemCompra();
                    _gridOrdem.CarregarGrid();

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function FecharLoteOrdemCompra() {
    _modalLoteOrdemCompra.hide();
    LimparCamposLoteOrdemCompra();
}

function LimparCamposLoteOrdemCompra() {
    LimparCampos(_loteOrdemCompra);
    LimparCampos(_formatoLoteOrdemCompra);
    LimparCamposProdutoLoteOC();
    RecarregarGridProdutosLoteOC();
    _loteOrdemCompra.Situacao.val(EnumSituacaoOrdemCompra.Aberta);
}

function QualificacaoLoteOCClick() {
    _qualificacao.Codigo.val(_loteOrdemCompra.Fornecedor.codEntity());

    _gridQualificacao.CarregarGrid(function () {
        Global.abrirModal("divQualificacaoFornecedor");
    });
};