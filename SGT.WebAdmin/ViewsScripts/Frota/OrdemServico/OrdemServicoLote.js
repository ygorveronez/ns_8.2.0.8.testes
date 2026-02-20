/// <reference path="../../Consultas/TipoOrdemServico.js" />
/// <reference path="../../Consultas/ServicoVeiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Equipamento.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="OrdemServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdemServicoLote;
var _ordemServicoLote;
var _ordemServicoLoteItem;
var _CRUDOrdemServicoLote;

var OrdemServicoLote = function () {
    this.Tipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ((_CONFIGURACAO_TMS.TipoOrdemServicoObrigatorio || false) ? "*Tipo:" : "Tipo:"), idBtnSearch: guid(), enable: ko.observable(true), required: (_CONFIGURACAO_TMS.TipoOrdemServicoObrigatorio || false) });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Local de Manutenção:"), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false) });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço", idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.ProdutoOrcado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto Orçado", idBtnSearch: guid(), enable: ko.observable(true), required: false });

    this.DataProgramada = PropertyEntity({ text: "*Data Programada:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "*Observação:", required: true, enable: ko.observable(true) });
    this.CondicaoPagamento = PropertyEntity({ maxlength: 1000, text: "Condição de Pagamento:", required: false, enable: ko.observable(true) });

    this.Itens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.ValorTotalProdutos = PropertyEntity({ text: "Valor Total Produtos: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), configDecimal: { precision: 5, allowZero: false, allowNegative: false } });
    this.ValorTotalServicos = PropertyEntity({ text: "Valor Total Serviços: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), configDecimal: { precision: 5, allowZero: false, allowNegative: false } });
};

var OrdemServicoLoteItem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Horimetro = PropertyEntity({ text: "Horímetro:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.QuilometragemVeiculo = PropertyEntity({ text: "KM Atual:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ValorProdutos = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor em Produtos:", val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true) });
    this.ValorMaoObra = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor em Mão de Obra:", val: ko.observable("0,00"), def: "0,00", enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.AdicionarItem = PropertyEntity({ eventClick: AdicionarItemOrdemServicoLoteClick, type: types.event, text: "Adicionar" });
};

var CRUDOrdemServicoLote = function () {
    this.Gerar = PropertyEntity({ eventClick: GerarMultiplasOSClick, type: types.event, text: "Gerar Múltiplas OS" });
};

//*******EVENTOS*******

function LoadOrdemServicoLote() {
    _ordemServicoLote = new OrdemServicoLote();
    KoBindings(_ordemServicoLote, "knockoutOrdemServicoLote");

    _ordemServicoLoteItem = new OrdemServicoLoteItem();
    KoBindings(_ordemServicoLoteItem, "knockoutItensOrdemServicoLote");

    _CRUDOrdemServicoLote = new CRUDOrdemServicoLote();
    KoBindings(_CRUDOrdemServicoLote, "knockoutCRUDOrdemServicoLote");

    new BuscarClientes(_ordemServicoLote.LocalManutencao, null, false, [EnumModalidadePessoa.Fornecedor]);
    new BuscarVeiculos(_ordemServicoLoteItem.Veiculo, RetornoVeiculoOrdemServicoLote, null, null, _ordemServicoLote.Motorista);
    new BuscarMotoristas(_ordemServicoLoteItem.Motorista);
    new BuscarTipoOrdemServico(_ordemServicoLote.Tipo);
    new BuscarEquipamentos(_ordemServicoLoteItem.Equipamento, RetornoEquipamentoOrdemServicoLote);
    new BuscarServicoVeiculo(_ordemServicoLote.Servico);
    new BuscarProdutoTMS(_ordemServicoLote.ProdutoOrcado);

    if (_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosOrdemServico) {
        _ordemServicoLote.LocalManutencao.required(true);
        _ordemServicoLote.LocalManutencao.text("*Local de Manutenção:");
    }

    LoadGridOrdemServicoLote();
}

function LoadGridOrdemServicoLote() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirItensOrdemServicoLoteClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Veiculo", title: "Veículo", width: "15%" },
        { data: "Equipamento", title: "Equipamento", width: "15%" },
        { data: "QuilometragemVeiculo", title: "KM Atual", width: "10%" },
        { data: "Horimetro", title: "Horímetro", width: "10%" },
        { data: "Motorista", title: "Motorista", width: "10%" },
        { data: "ValorProdutos", title: "Vlr. Produtos", width: "10%" },
        { data: "ValorMaoObra", title: "Vlr. Serviços", width: "10%" }
    ];

    _gridOrdemServicoLote = new BasicDataTable(_ordemServicoLoteItem.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridOrdemServicoLote();
}

function RetornoVeiculoOrdemServicoLote(data) {
    if (data != null & data.Motorista != null & data.Motorista != "" & data.CodigoMotorista > 0) {
        _ordemServicoLoteItem.Motorista.val(data.Motorista);
        _ordemServicoLoteItem.Motorista.codEntity(data.CodigoMotorista);
    }
    if (data != null & data.Placa != null & data.Placa != "" & data.Codigo > 0) {
        if (data.TipoPropriedade == "T")
            _ordemServicoLoteItem.Veiculo.val(data.Placa + " (TERCEIRO)");
        else
            _ordemServicoLoteItem.Veiculo.val(data.Placa + " (PRÓPRIO)");
        _ordemServicoLoteItem.Veiculo.codEntity(data.Codigo);
        if (parseFloat(data.UltimoKMAbastecimento) > 0)
            _ordemServicoLoteItem.QuilometragemVeiculo.val(parseFloat(data.UltimoKMAbastecimento));
        else
            _ordemServicoLoteItem.QuilometragemVeiculo.val(data.KMAtual);
    }
}

function RetornoEquipamentoOrdemServicoLote(data) {
    _ordemServicoLoteItem.Equipamento.val(data.Descricao);
    _ordemServicoLoteItem.Equipamento.codEntity(data.Codigo);
    _ordemServicoLoteItem.Horimetro.val(data.Horimetro);
}

function GerarMultiplasOSClick() {
    var valido = ValidarCamposObrigatorios(_ordemServicoLote);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    exibirConfirmacao("Atenção!", "Deseja realmente gerar as Ordens de Serviços?", function () {
        Salvar(_ordemServicoLote, "OrdemServico/AdicionarPorLote", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordens de Serviços geradas com sucesso!<br>Números: " + r.Data.Numero, 15000);
                    LimparCamposGridOrdemServicoLote();
                    Global.abrirModal("divModalOrdemServicoLote");
                    _gridOrdemServico.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function RecarregarGridOrdemServicoLote() {
    var data = new Array();

    _ordemServicoLote.ValorTotalProdutos.val(Globalize.format(0, "n2"));
    _ordemServicoLote.ValorTotalServicos.val(Globalize.format(0, "n2"));

    $.each(_ordemServicoLote.Itens.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Veiculo = item.Veiculo.val;
        itemGrid.Equipamento = item.Equipamento.val;
        itemGrid.QuilometragemVeiculo = item.QuilometragemVeiculo.val;
        itemGrid.Horimetro = item.Horimetro.val;
        itemGrid.Motorista = item.Motorista.val;
        itemGrid.ValorProdutos = item.ValorProdutos.val;
        itemGrid.ValorMaoObra = item.ValorMaoObra.val;

        data.push(itemGrid);

        var valorTotalProduto = Globalize.parseFloat(_ordemServicoLote.ValorTotalProdutos.val()) || 0;
        var valorProduto = Globalize.parseFloat(item.ValorProdutos.val) || 0;
        _ordemServicoLote.ValorTotalProdutos.val(Globalize.format(valorTotalProduto + valorProduto, "n2"));

        var valorTotalServico = Globalize.parseFloat(_ordemServicoLote.ValorTotalServicos.val()) || 0;
        var valorServico = Globalize.parseFloat(item.ValorMaoObra.val) || 0;
        _ordemServicoLote.ValorTotalServicos.val(Globalize.format(valorTotalServico + valorServico, "n2"));
    });

    _gridOrdemServicoLote.CarregarGrid(data);
}

function AdicionarItemOrdemServicoLoteClick() {
    var valido = _ordemServicoLoteItem.Veiculo.codEntity() > 0 || _ordemServicoLoteItem.Equipamento.codEntity() > 0;
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informar Veículo ou Equipamento!");
        return;
    }

    _ordemServicoLoteItem.Codigo.val(guid());
    _ordemServicoLote.Itens.list.push(SalvarListEntity(_ordemServicoLoteItem));

    RecarregarGridOrdemServicoLote();
    LimparCamposGridOrdemServicoLote();
}

function ExcluirItensOrdemServicoLoteClick(data) {
    for (var i = 0; i < _ordemServicoLote.Itens.list.length; i++) {
        if (data.Codigo == _ordemServicoLote.Itens.list[i].Codigo.val) {
            _ordemServicoLote.Itens.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridOrdemServicoLote();
}

function LimparCamposGridOrdemServicoLote() {
    LimparCampos(_ordemServicoLoteItem);
}

function LimparCamposOrdemServicoLote() {
    LimparCampos(_ordemServicoLote);
    RecarregarGridOrdemServicoLote();
    LimparCamposGridOrdemServicoLote();
}