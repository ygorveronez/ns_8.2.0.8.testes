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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Enumeradores/EnumStatusPedidoVenda.js" />
/// <reference path="OrdemServicoVendaEtapa.js" />
/// <reference path="OrdemServicoVenda.js" />
/// <reference path="OrdemServicoVendaPecas.js" />
/// <reference path="../../Enumeradores/EnumTipoOrdemServicoVenda.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tipoServico = [
    { text: "Próprio", value: 1 },
    { text: "Terceiro", value: 2 }
];

var _ordemServicoVendaMaoObras;
var _gridVendaMaoObra;

var MaoObraMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Servico = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Funcionario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.FuncionarioAuxiliar = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Pessoa = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.TipoServico = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoOrdemServicoVenda = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.CodigoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.DescricaoItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Quantidade = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorUnitario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalItem = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorDesconto = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NomeFuncionario = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NomePessoa = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NomeFuncionarioAuxiliar = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.KMInicial = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.KMFinal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.KMTotal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraInicial = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraFinal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraTotal = PropertyEntity({ type: types.map, val: ko.observable("") });

    this.KMTotalUnidade = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraTotalUnidade = PropertyEntity({ type: types.map, val: ko.observable("") });

    this.ValorKM = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalKM = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorHora = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorTotalHora = PropertyEntity({ type: types.map, val: ko.observable("") });

    this.KMInicial2 = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.KMFinal2 = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.KMTotal2 = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraInicial2 = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraFinal2 = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.HoraTotal2 = PropertyEntity({ type: types.map, val: ko.observable("") });

    this.TipoOrdemServicoVenda = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var OrdemServicoVendaMaoObra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorDesconto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal });

    this.TipoServico = PropertyEntity({ val: ko.observable(1), options: _tipoServico, def: 1, text: "*Tipo Serviço: ", enable: ko.observable(true), eventChange: TipoServicoChange });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Serviço:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Funcionário:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.FuncionarioAuxiliar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Funcionário Auxiliar:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda) });

    this.CodigoItem = PropertyEntity({ text: "*Código Item:", getType: typesKnockout.string, maxlength: 100, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DescricaoItem = PropertyEntity({ text: "*Descrição Item:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Quantidade = PropertyEntity({ def: (_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda ? "1,00" : "0,00"), val: ko.observable(_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda ? "1,00" : "0,00"), text: "*Quantidade:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda ? false : true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorUnitario = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Unitário:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalItem = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "*Valor Total:", getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.KMInicial = PropertyEntity({ text: "KM Inicial 1:", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(true) });
    this.KMFinal = PropertyEntity({ text: "KM Final 1:", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(true) });
    this.KMTotal = PropertyEntity({ text: "KM Total:", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(false) });
    this.KMTotalUnidade = PropertyEntity({ text: "Quantidade:", getType: typesKnockout.int, visible: ko.observable(_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda), enable: ko.observable(false) });

    this.KMInicial2 = PropertyEntity({ text: "KM Inicial 2:", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(true) });
    this.KMFinal2 = PropertyEntity({ text: "KM Final 2:", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(true) });
    this.KMTotal2 = PropertyEntity({ text: "KM Total 2:", getType: typesKnockout.int, visible: ko.observable(false), enable: ko.observable(false) });

    this.HoraInicial = PropertyEntity({ text: "Hora Inicial 1:", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(true) });
    this.HoraFinal = PropertyEntity({ text: "Hora Final 1:", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(true) });
    this.HoraTotal = PropertyEntity({ text: "Hora Total:", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(false) });
    this.HoraTotalUnidade = PropertyEntity({ text: "Quantidade:", getType: typesKnockout.time, visible: ko.observable(_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda), enable: ko.observable(false) });

    this.HoraInicial2 = PropertyEntity({ text: "Hora Inicial 2:", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(true) });
    this.HoraFinal2 = PropertyEntity({ text: "Hora Final 2:", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(true) });
    this.HoraTotal2 = PropertyEntity({ text: "Hora Total 2:", getType: typesKnockout.time, visible: ko.observable(false), enable: ko.observable(false) });

    this.ValorKM = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor KM:", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalKM = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total KM:", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });
    this.ValorHora = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Hora:", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.ValorTotalHora = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Valor Total Hora:", getType: typesKnockout.decimal, maxlength: 15, visible: ko.observable(true), enable: ko.observable(false), configDecimal: { precision: 2, allowZero: true } });

    this.ValorServicos = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Serviços:", maxlength: 15, enable: ko.observable(false) });
    this.ValorTotal = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, text: "Valor Total:", maxlength: 15, enable: ko.observable(false) });

    this.TipoOrdemServicoVenda = PropertyEntity({ text: "Tipo Ordem Serviço: ", val: ko.observable(EnumTipoOrdemServicoVenda.Todos), options: EnumTipoOrdemServicoVenda.obterOpcoesSelecione(), def: EnumTipoOrdemServicoVenda.Todos, enable: ko.observable(false), visible: ko.observable(false) });

    //CRUD
    this.SalvarItem = PropertyEntity({ eventClick: SalvarMaoObraClick, type: types.event, text: "Salvar Item", visible: ko.observable(true), enable: ko.observable(true) });
    this.Gravar = PropertyEntity({ eventClick: GravarOrdemServicoVendaClick, type: types.event, text: "Gravar OS", visible: ko.observable(true), enable: ko.observable(true) });

    this.MaoObraOrdemServicoVenda = PropertyEntity({ type: types.local, id: guid() });

    this.ValorTotalKM.val.subscribe(function () {
        AtualizarValorUnitario();
    });

    this.ValorTotalHora.val.subscribe(function () {
        AtualizarValorUnitario();
    });
};

//*******EVENTOS*******

function loadOrdemServicoVendaMaoObra() {
    _ordemServicoVendaMaoObras = new OrdemServicoVendaMaoObra();
    KoBindings(_ordemServicoVendaMaoObras, "knockoutMaoObraOrdemServico");

    new BuscarClientes(_ordemServicoVendaMaoObras.Pessoa, null, true);
    new BuscarFuncionario(_ordemServicoVendaMaoObras.Funcionario);
    new BuscarFuncionario(_ordemServicoVendaMaoObras.FuncionarioAuxiliar);
    new BuscarServicoTMS(_ordemServicoVendaMaoObras.Servico, function (data) {
        _ordemServicoVendaMaoObras.Servico.codEntity(data.Codigo);
        _ordemServicoVendaMaoObras.Servico.val(data.Descricao);
        if (data.ValorVenda != null && data.ValorVenda != "") {
            var valorUnitario = Globalize.parseFloat(data.ValorVenda.toString());
            _ordemServicoVendaMaoObras.ValorUnitario.val(Globalize.format(valorUnitario, "n2"));
        }
        _ordemServicoVendaMaoObras.DescricaoItem.val(data.Descricao);
        _ordemServicoVendaMaoObras.CodigoItem.val(data.Codigo);
        if (_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda)
            $("#" + _ordemServicoVendaMaoObras.KMInicial.id).focus();
        else
            $("#" + _ordemServicoVendaMaoObras.Quantidade.id).focus();
    });

    TipoServicoChange();

    if (_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda) {
        _ordemServicoVendaMaoObras.KMTotalUnidade.visible(false);
        _ordemServicoVendaMaoObras.HoraTotalUnidade.visible(false);
    }

    var habilitaDadosTabela = _configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda;
    if (habilitaDadosTabela)
        $("#liTabelaValor").show();

    var editarItem = { descricao: "Editar", id: guid(), metodo: EditarMaoObraClick, icone: "" };
    var excluirItem = { descricao: "Excluir", id: guid(), metodo: ExcluirMaoObraClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [editarItem, excluirItem] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Servico", visible: false },
        { data: "Funcionario", visible: false },
        { data: "Pessoa", visible: false },
        { data: "TipoServico", visible: false },
        { data: "CodigoOrdemServicoVenda", visible: false },
        { data: "CodigoItem", title: "Código", width: "7%" },
        { data: "DescricaoItem", title: "Descrição", width: "30%" },

        { data: "KMInicial", title: "KM Inicial", width: "7%", visible: habilitaDadosTabela },
        { data: "KMFinal", title: "KM Final", width: "7%", visible: habilitaDadosTabela },
        { data: "KMTotal", title: "KM Total", width: "7%", visible: habilitaDadosTabela },
        { data: "HoraInicial", title: "Hora Inicial", width: "7%", visible: habilitaDadosTabela },
        { data: "HoraFinal", title: "Hora Final", width: "7%", visible: habilitaDadosTabela },
        { data: "HoraTotal", title: "Hora Total", width: "7%", visible: habilitaDadosTabela },

        { data: "Quantidade", title: "Quantidade", width: "7%", visible: !habilitaDadosTabela },
        { data: "ValorUnitario", title: "Valor Unit.", width: "7%" },
        { data: "ValorTotalItem", title: "Total", width: "7%" },
        { data: "ValorDesconto", visible: false },
        { data: "NomeFuncionario", title: "Funcionário", width: "10%", visible: !habilitaDadosTabela },
        { data: "NomePessoa", title: "Fornecedor", width: "10%", visible: !habilitaDadosTabela },


        { data: "ValorKM", visible: false },
        { data: "ValorTotalKM", visible: false },
        { data: "ValorHora", visible: false },
        { data: "ValorTotalHora", visible: false },

        { data: "KMTotalUnidade", visible: false },
        { data: "HoraTotalUnidade", visible: false },

        { data: "TipoOrdemServicoVenda", visible: false },

        { data: "FuncionarioAuxiliar", visible: false },
        { data: "NomeFuncionarioAuxiliar", title: "Funcionário Auxiliar", visible: false },

        { data: "KMInicial2", title: "KM Inicial2", visible: false },
        { data: "KMFinal2", title: "KM Final2", visible: false },
        { data: "KMTotal2", title: "KM Total2", visible: false },

        { data: "HoraInicial2", title: "Hora Inicial2", visible: false },
        { data: "HoraFinal2", title: "Hora Final2", visible: false },
        { data: "HoraTotal2", title: "Hora Total2", visible: false },
    ];

    _gridVendaMaoObra = new BasicDataTable(_ordemServicoVendaMaoObras.MaoObraOrdemServicoVenda.id, header, menuOpcoes);

    recarregarGridListaMaoObra();
}

function TipoServicoChange(e, sender) {
    if (_ordemServicoVendaMaoObras.TipoServico.val() === 1) {
        _ordemServicoVendaMaoObras.Funcionario.visible(true);
        _ordemServicoVendaMaoObras.Pessoa.visible(false);
        LimparCampoEntity(_ordemServicoVendaMaoObras.Pessoa);
    } else if (_ordemServicoVendaMaoObras.TipoServico.val() === 2) {
        _ordemServicoVendaMaoObras.Funcionario.visible(false);
        _ordemServicoVendaMaoObras.Pessoa.visible(true);
        LimparCampoEntity(_ordemServicoVendaMaoObras.Funcionario);
    }
}

function EditarMaoObraClick(data) {
    if (_ordemServicoVenda.Status.val() !== EnumStatusPedidoVenda.Aberta && _ordemServicoVenda.Status.val() !== EnumStatusPedidoVenda.PendenteOperacional) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível editar itens de Pedido em Aberto!");
        return;
    }
    if (_ordemServicoVendaMaoObras.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoVendaMaoObras.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    limparCamposOrdemServicoVendaMaoObra();
    RemoverValorMaoObraTotalizador(data);
    _ordemServicoVenda.ValorDesconto.enable(false);
    _ordemServicoVenda.PercentualDesconto.enable(false);

    _ordemServicoVendaMaoObras.TipoServico.val(data.TipoServico);
    TipoServicoChange();
    _ordemServicoVendaMaoObras.Funcionario.codEntity(data.Funcionario);
    _ordemServicoVendaMaoObras.Funcionario.val(data.NomeFuncionario);

    _ordemServicoVendaMaoObras.FuncionarioAuxiliar.codEntity(data.FuncionarioAuxiliar);
    _ordemServicoVendaMaoObras.FuncionarioAuxiliar.val(data.NomeFuncionarioAuxiliar);

    _ordemServicoVendaMaoObras.Pessoa.codEntity(data.Pessoa);
    _ordemServicoVendaMaoObras.Pessoa.val(data.NomePessoa);

    _ordemServicoVendaMaoObras.Codigo.val(data.Codigo);
    _ordemServicoVendaMaoObras.Servico.codEntity(data.Servico);
    _ordemServicoVendaMaoObras.Servico.val(data.DescricaoItem);
    _ordemServicoVendaMaoObras.CodigoItem.val(data.CodigoItem);
    _ordemServicoVendaMaoObras.DescricaoItem.val(data.DescricaoItem);

    _ordemServicoVendaMaoObras.KMInicial.val(data.KMInicial);
    _ordemServicoVendaMaoObras.KMFinal.val(data.KMFinal);
    _ordemServicoVendaMaoObras.KMTotal.val(data.KMTotal);

    _ordemServicoVendaMaoObras.KMInicial2.val(data.KMInicial2);
    _ordemServicoVendaMaoObras.KMFinal2.val(data.KMFinal2);
    _ordemServicoVendaMaoObras.KMTotal2.val(data.KMTotal2);

    _ordemServicoVendaMaoObras.TipoOrdemServicoVenda.val(data.TipoOrdemServicoVenda);

    _ordemServicoVendaMaoObras.HoraInicial.val(data.HoraInicial);
    _ordemServicoVendaMaoObras.HoraFinal.val(data.HoraFinal);
    _ordemServicoVendaMaoObras.HoraTotal.val(data.HoraTotal);

    _ordemServicoVendaMaoObras.HoraTotal.val(data.HoraTotalUnidade);
    _ordemServicoVendaMaoObras.KMTotalUnidade.val(data.KMTotalUnidade);

    _ordemServicoVendaMaoObras.HoraInicial2.val(data.HoraInicial2);
    _ordemServicoVendaMaoObras.HoraFinal2.val(data.HoraFinal2);
    _ordemServicoVendaMaoObras.HoraTotal2.val(data.HoraTotal2);

    _ordemServicoVendaMaoObras.ValorKM.val(data.ValorKM);
    _ordemServicoVendaMaoObras.ValorTotalKM.val(data.ValorTotalKM);
    _ordemServicoVendaMaoObras.ValorHora.val(data.ValorHora);
    _ordemServicoVendaMaoObras.ValorTotalHora.val(data.ValorTotalHora);

    _ordemServicoVendaMaoObras.Quantidade.val(data.Quantidade);
    _ordemServicoVendaMaoObras.ValorUnitario.val(data.ValorUnitario);
    _ordemServicoVendaMaoObras.ValorTotalItem.val(data.ValorTotalItem);
    _ordemServicoVendaMaoObras.ValorDesconto.val(data.ValorDesconto);

    CalcularTotalKMMaoObra();
    CalcularTotalHoraMaoObra();
}

function ExcluirMaoObraClick(data) {
    if (_ordemServicoVenda.Status.val() !== EnumStatusPedidoVenda.Aberta) {
        exibirMensagem("atencao", "Pedido Concluído", "Só é possível excluir itens de Pedido em Aberto!");
        return;
    }
    if (_ordemServicoVendaMaoObras.Codigo.val() > 0) {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique os itens pois existe um em edição.");
        return;
    }
    if (_ordemServicoVendaMaoObras.Servico.val() != "") {
        exibirMensagem("atencao", "Item em Edição", "Por favor, verifique pois existe um serviço sem salvar.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + data.DescricaoItem + "?", function () {
        $.each(_ordemServicoVenda.ListaMaoObras.list, function (i, listaItens) {
            if (data.Codigo == listaItens.Codigo.val) {
                RemoverValorMaoObraTotalizador(data);
                _ordemServicoVenda.ListaMaoObras.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridListaMaoObra();
    });
}

function SalvarMaoObraClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_ordemServicoVendaMaoObras);
    _ordemServicoVendaMaoObras.Quantidade.requiredClass("form-control");
    _ordemServicoVendaMaoObras.ValorUnitario.requiredClass("form-control");
    _ordemServicoVendaMaoObras.ValorTotalItem.requiredClass("form-control");

    if (Globalize.parseFloat(_ordemServicoVendaMaoObras.Quantidade.val()) <= 0) {
        valido = false;
        _ordemServicoVendaMaoObras.Quantidade.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorUnitario.val()) <= 0) {
        valido = false;
        _ordemServicoVendaMaoObras.ValorUnitario.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalItem.val()) <= 0) {
        valido = false;
        _ordemServicoVendaMaoObras.ValorTotalItem.requiredClass("form-control is-invalid");
    }

    if (valido) {
        if (_ordemServicoVendaMaoObras.Codigo.val() > "0" && _ordemServicoVendaMaoObras.Codigo.val() != undefined) {
            for (var i = 0; i < _ordemServicoVenda.ListaMaoObras.list.length; i++) {
                if (_ordemServicoVendaMaoObras.Codigo.val() == _ordemServicoVenda.ListaMaoObras.list[i].Codigo.val) {
                    _ordemServicoVenda.ListaMaoObras.list.splice(i, 1);
                    break;
                }
            }
        }
        _ordemServicoVendaMaoObras.Codigo.val(guid());

        var listaMaoObrasGrid = new MaoObraMap();

        listaMaoObrasGrid.Codigo.val = _ordemServicoVendaMaoObras.Codigo.val();
        listaMaoObrasGrid.Servico.val = _ordemServicoVendaMaoObras.Servico.codEntity();
        listaMaoObrasGrid.Funcionario.val = _ordemServicoVendaMaoObras.Funcionario.codEntity();
        listaMaoObrasGrid.FuncionarioAuxiliar.val = _ordemServicoVendaMaoObras.FuncionarioAuxiliar.codEntity();
        listaMaoObrasGrid.Pessoa.val = _ordemServicoVendaMaoObras.Pessoa.codEntity();
        listaMaoObrasGrid.TipoServico.val = _ordemServicoVendaMaoObras.TipoServico.val();
        listaMaoObrasGrid.CodigoOrdemServicoVenda.val = _ordemServicoVenda.Codigo.val();
        listaMaoObrasGrid.CodigoItem.val = _ordemServicoVendaMaoObras.CodigoItem.val();
        listaMaoObrasGrid.DescricaoItem.val = _ordemServicoVendaMaoObras.DescricaoItem.val();
        listaMaoObrasGrid.Quantidade.val = _ordemServicoVendaMaoObras.Quantidade.val();
        listaMaoObrasGrid.ValorUnitario.val = _ordemServicoVendaMaoObras.ValorUnitario.val();
        listaMaoObrasGrid.ValorTotalItem.val = _ordemServicoVendaMaoObras.ValorTotalItem.val();
        listaMaoObrasGrid.ValorDesconto.val = _ordemServicoVendaMaoObras.ValorDesconto.val();
        listaMaoObrasGrid.NomeFuncionario.val = _ordemServicoVendaMaoObras.Funcionario.val();
        listaMaoObrasGrid.NomeFuncionarioAuxiliar.val = _ordemServicoVendaMaoObras.FuncionarioAuxiliar.val();
        listaMaoObrasGrid.NomePessoa.val = _ordemServicoVendaMaoObras.Pessoa.val();

        listaMaoObrasGrid.KMInicial.val = _ordemServicoVendaMaoObras.KMInicial.val();
        listaMaoObrasGrid.KMFinal.val = _ordemServicoVendaMaoObras.KMFinal.val();
        listaMaoObrasGrid.KMTotal.val = _ordemServicoVendaMaoObras.KMTotal.val();
        listaMaoObrasGrid.KMInicial2.val = _ordemServicoVendaMaoObras.KMInicial2.val();
        listaMaoObrasGrid.KMFinal2.val = _ordemServicoVendaMaoObras.KMFinal2.val();
        listaMaoObrasGrid.KMTotal2.val = _ordemServicoVendaMaoObras.KMTotal2.val();

        listaMaoObrasGrid.TipoOrdemServicoVenda.val = _ordemServicoVendaMaoObras.TipoOrdemServicoVenda.val();

        listaMaoObrasGrid.HoraInicial.val = _ordemServicoVendaMaoObras.HoraInicial.val();
        listaMaoObrasGrid.HoraFinal.val = _ordemServicoVendaMaoObras.HoraFinal.val();
        listaMaoObrasGrid.HoraTotal.val = _ordemServicoVendaMaoObras.HoraTotal.val();
        listaMaoObrasGrid.HoraInicial2.val = _ordemServicoVendaMaoObras.HoraInicial2.val();
        listaMaoObrasGrid.HoraFinal2.val = _ordemServicoVendaMaoObras.HoraFinal2.val();
        listaMaoObrasGrid.HoraTotal2.val = _ordemServicoVendaMaoObras.HoraTotal2.val();

        listaMaoObrasGrid.KMTotalUnidade.val = _ordemServicoVendaMaoObras.KMTotalUnidade.val();
        listaMaoObrasGrid.HoraTotalUnidade.val = _ordemServicoVendaMaoObras.HoraTotalUnidade.val();

        listaMaoObrasGrid.ValorKM.val = _ordemServicoVendaMaoObras.ValorKM.val();
        listaMaoObrasGrid.ValorTotalKM.val = _ordemServicoVendaMaoObras.ValorTotalKM.val();
        listaMaoObrasGrid.ValorHora.val = _ordemServicoVendaMaoObras.ValorHora.val();
        listaMaoObrasGrid.ValorTotalHora.val = _ordemServicoVendaMaoObras.ValorTotalHora.val();

        _ordemServicoVenda.ListaMaoObras.list.push(listaMaoObrasGrid);

        recarregarGridListaMaoObra();
        AtualizarTotalizadoresMaoObra();
        limparCamposOrdemServicoVendaMaoObra();
        $("#" + _ordemServicoVendaMaoObras.Servico.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

//*******MÉTODOS*******

function recarregarGridListaMaoObra() {

    var data = new Array();

    $.each(_ordemServicoVenda.ListaMaoObras.list, function (i, listaItens) {
        var listaMaoObrasGrid = new Object();

        listaMaoObrasGrid.Codigo = listaItens.Codigo.val;
        listaMaoObrasGrid.Servico = listaItens.Servico.val;
        listaMaoObrasGrid.Funcionario = listaItens.Funcionario.val;
        listaMaoObrasGrid.Pessoa = listaItens.Pessoa.val;
        listaMaoObrasGrid.TipoServico = listaItens.TipoServico.val;
        listaMaoObrasGrid.CodigoOrdemServicoVenda = _ordemServicoVenda.Codigo.val();
        listaMaoObrasGrid.CodigoItem = listaItens.CodigoItem.val;
        listaMaoObrasGrid.DescricaoItem = listaItens.DescricaoItem.val;
        listaMaoObrasGrid.Quantidade = listaItens.Quantidade.val;
        listaMaoObrasGrid.ValorUnitario = listaItens.ValorUnitario.val;
        listaMaoObrasGrid.ValorTotalItem = listaItens.ValorTotalItem.val;
        listaMaoObrasGrid.ValorDesconto = listaItens.ValorDesconto.val;
        listaMaoObrasGrid.NomeFuncionario = listaItens.NomeFuncionario.val;
        listaMaoObrasGrid.NomePessoa = listaItens.NomePessoa.val;

        listaMaoObrasGrid.KMInicial = listaItens.KMInicial.val;
        listaMaoObrasGrid.KMFinal = listaItens.KMFinal.val;
        listaMaoObrasGrid.KMTotal = listaItens.KMTotal.val;
        listaMaoObrasGrid.HoraInicial = listaItens.HoraInicial.val;
        listaMaoObrasGrid.HoraFinal = listaItens.HoraFinal.val;
        listaMaoObrasGrid.HoraTotal = listaItens.HoraTotal.val;
        listaMaoObrasGrid.ValorKM = listaItens.ValorKM.val;
        listaMaoObrasGrid.ValorTotalKM = listaItens.ValorTotalKM.val;
        listaMaoObrasGrid.ValorHora = listaItens.ValorHora.val;
        listaMaoObrasGrid.ValorTotalHora = listaItens.ValorTotalHora.val;

        listaMaoObrasGrid.KMInicial2 = listaItens.KMInicial2.val;
        listaMaoObrasGrid.KMFinal2 = listaItens.KMFinal2.val;
        listaMaoObrasGrid.KMTotal2 = listaItens.KMTotal2.val;
        listaMaoObrasGrid.HoraInicial2 = listaItens.HoraInicial2.val;
        listaMaoObrasGrid.HoraFinal2 = listaItens.HoraFinal2.val;
        listaMaoObrasGrid.HoraTotal2 = listaItens.HoraTotal2.val;

        listaMaoObrasGrid.KMTotalUnidade = listaItens.KMTotalUnidade.val;
        listaMaoObrasGrid.HoraTotalUnidade = listaItens.HoraTotalUnidade.val;

        listaMaoObrasGrid.FuncionarioAuxiliar = listaItens.FuncionarioAuxiliar.val;
        listaMaoObrasGrid.NomeFuncionarioAuxiliar = listaItens.NomeFuncionarioAuxiliar.val;
        listaMaoObrasGrid.TipoOrdemServicoVenda = listaItens.TipoOrdemServicoVenda.val;

        data.push(listaMaoObrasGrid);
    });

    _gridVendaMaoObra.CarregarGrid(data);
}

function CalcularTotalMaoObra() {
    var valorKM = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorKM.val());
    var valorHora = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorHora.val());
    var valorTotalItem = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalItem.val());

    if (isNaN(valorKM))
        valorKM = 0;
    if (isNaN(valorHora))
        valorHora = 0;
    if (isNaN(valorTotalItem))
        valorTotalItem = 0;

    if ((valorKM == 0 && valorHora == 0)) {
        var quantidade = Globalize.parseFloat(_ordemServicoVendaMaoObras.Quantidade.val());
        var valorUnitario = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorUnitario.val());

        if (quantidade > 0 && valorUnitario > 0) {
            var valorTotal = quantidade * valorUnitario;
            _ordemServicoVendaMaoObras.ValorTotalItem.val(Globalize.format(valorTotal, "n2"));
        }
    }
    else {
        var valorTotalKM = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalKM.val());
        var valorTotalHora = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalHora.val());

        if (isNaN(valorTotalKM))
            valorTotalKM = 0;
        if (isNaN(valorTotalHora))
            valorTotalHora = 0;

        _ordemServicoVendaMaoObras.ValorTotalItem.val(Globalize.format(valorTotalKM + valorTotalHora, "n2"));
    }
}

function CalcularTotalKMMaoObra() {
    if (!_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda)
        return;

    var valorKM = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorKM.val());
    var kmInicial = Globalize.parseFloat(_ordemServicoVendaMaoObras.KMInicial.val());
    var kmFinal = Globalize.parseFloat(_ordemServicoVendaMaoObras.KMFinal.val());
    var kmInicial2 = Globalize.parseFloat(_ordemServicoVendaMaoObras.KMInicial2.val());
    var kmFinal2 = Globalize.parseFloat(_ordemServicoVendaMaoObras.KMFinal2.val());

    if (isNaN(valorKM))
        valorKM = 0;
    if (isNaN(kmInicial))
        kmInicial = 0;
    if (isNaN(kmFinal))
        kmFinal = 0;
    if (isNaN(kmInicial2))
        kmInicial2 = 0;
    if (isNaN(kmFinal2))
        kmFinal2 = 0;

    _ordemServicoVendaMaoObras.KMTotal.val("");
    _ordemServicoVendaMaoObras.ValorTotalKM.val(Globalize.format(0, "n2"));

    var kmTotal = (kmFinal - kmInicial) + (kmFinal2 - kmInicial2);
    _ordemServicoVendaMaoObras.KMTotal.val(kmTotal > 0 ? kmTotal : 0);

    if (kmTotal > 0 && valorKM > 0) {
        var valorTotal = kmTotal * valorKM;
        _ordemServicoVendaMaoObras.ValorTotalKM.val(Globalize.format(valorTotal, "n2"));
        _ordemServicoVendaMaoObras.ValorUnitario.val(Globalize.format(valorKM, "n2"));

        _ordemServicoVendaMaoObras.KMTotalUnidade.visible(true);
        _ordemServicoVendaMaoObras.KMTotalUnidade.val(_ordemServicoVendaMaoObras.KMTotal.val());
        _ordemServicoVendaMaoObras.HoraTotalUnidade.visible(false);

        _ordemServicoVendaMaoObras.HoraTotalUnidade.enable(false);
        _ordemServicoVendaMaoObras.KMTotalUnidade.enable(false);
        _ordemServicoVendaMaoObras.ValorUnitario.enable(false);
    }
}

function CalcularTotalKMSecundario() {
    var kmInicial2 = Globalize.parseFloat(_ordemServicoVendaMaoObras.KMInicial2.val());
    var kmFinal2 = Globalize.parseFloat(_ordemServicoVendaMaoObras.KMFinal2.val());

    _ordemServicoVendaMaoObras.KMTotal2.val("");

    if (kmInicial2 > 0 && kmFinal2 > 0) {
        var kmTotal2 = kmFinal2 - kmInicial2;
        _ordemServicoVendaMaoObras.KMTotal2.val(kmTotal2 > 0 ? kmTotal2 : 0);
    }
}

function CalcularTotalHoraMaoObra() {
    if (!_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda)
        return;

    var valorHora = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorHora.val());
    var horaInicial = _ordemServicoVendaMaoObras.HoraInicial.val();
    var horaFinal = _ordemServicoVendaMaoObras.HoraFinal.val();
    var horaInicial2 = _ordemServicoVendaMaoObras.HoraInicial2.val();
    var horaFinal2 = _ordemServicoVendaMaoObras.HoraFinal2.val();

    if (isNaN(valorHora))
        valorHora = 0;

    _ordemServicoVendaMaoObras.HoraTotal.val("");
    _ordemServicoVendaMaoObras.ValorTotalHora.val(Globalize.format(0, "n2"));

    if (horaInicial != "" && horaFinal == "")
        return;
    if (horaInicial2 != "" && horaFinal2 == "")
        return;
    if (horaInicial == "" && horaFinal != "")
        return;
    if (horaInicial2 == "" && horaFinal2 != "")
        return;

    var horaTotal = 0;
    if (horaInicial != "" && horaFinal != "" && horaFinal != "00:00") {
        var horaInicio = moment(horaInicial, "HH:mm");
        var horaFim = moment(horaFinal, "HH:mm");
        var horaTotal = "";

        if (horaFim.diff(horaInicio) > 0)
            horaTotal = horaFim - horaInicio;
    }
    var horaTotal2 = 0
    if (horaInicial2 != "" && horaFinal2 != "" && horaFinal2 != "00:00") {
        var horaInicio2 = moment(horaInicial2, "HH:mm");
        var horaFim2 = moment(horaFinal2, "HH:mm");
        var horaTotal2 = "";

        if (horaFim2.diff(horaInicio2) > 0)
            horaTotal2 = horaFim2 - horaInicio2;
    }

    horaTotal = horaTotal + horaTotal2;

    _ordemServicoVendaMaoObras.HoraTotal.val(horaTotal != "" ? moment.utc(horaTotal).format("HH:mm") : "");

    if (horaTotal != "" && valorHora > 0) {
        var valorTotal = (horaTotal / 60000) * (valorHora / 60);
        _ordemServicoVendaMaoObras.ValorTotalHora.val(Globalize.format(valorTotal, "n2"));
        _ordemServicoVendaMaoObras.ValorUnitario.val(Globalize.format(valorHora, "n2"));

        _ordemServicoVendaMaoObras.HoraTotalUnidade.visible(true);
        _ordemServicoVendaMaoObras.HoraTotalUnidade.val(_ordemServicoVendaMaoObras.HoraTotal.val());
        _ordemServicoVendaMaoObras.KMTotalUnidade.visible(false);

        _ordemServicoVendaMaoObras.HoraTotalUnidade.enable(false);
        _ordemServicoVendaMaoObras.KMTotalUnidade.enable(false);
        _ordemServicoVendaMaoObras.ValorUnitario.enable(false);
    }
}

function CalcularTotalHoraSecundaria() {
    var horaInicial2 = _ordemServicoVendaMaoObras.HoraInicial2.val();
    var horaFinal2 = _ordemServicoVendaMaoObras.HoraFinal2.val();

    _ordemServicoVendaMaoObras.HoraTotal2.val("");

    if (horaInicial2 == "" || horaFinal2 == "")
        return;

    var horaInicio2 = moment(horaInicial2, "HH:mm");
    var horaFim2 = moment(horaFinal2, "HH:mm");
    var horaTotal2 = "";

    if (horaFim2.diff(horaInicio2) > 0)
        horaTotal2 = horaFim2 - horaInicio2;

    _ordemServicoVendaMaoObras.HoraTotal2.val(horaTotal2 != "" ? moment.utc(horaTotal2).format("HH:mm") : "");
}

function AtualizarValorUnitario() {
    var valorkm = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalKM.val());
    var valorHora = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalHora.val());

    _ordemServicoVendaMaoObras.ValorUnitario.val(Globalize.format(valorkm + valorHora, "n2"));
    CalcularTotalMaoObra();
}

function AtualizarTotalizadoresMaoObra() {
    var valorTotalItem = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotalItem.val());
    var valorTotal = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotal.val());
    var valorTotalServicos = 0;

    var valorTotalServicosTotal = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorServicos.val());
    valorTotalServicos = valorTotalItem + valorTotalServicosTotal;
    _ordemServicoVendaMaoObras.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
    _ordemServicoVenda.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));

    valorTotal = valorTotal + valorTotalItem;
    _ordemServicoVendaMaoObras.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVendaPecas.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    _ordemServicoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));
}

function RemoverValorMaoObraTotalizador(itemPedido) {
    for (var i = 0; i < _ordemServicoVenda.ListaMaoObras.list.length; i++) {
        if (itemPedido.Codigo == _ordemServicoVenda.ListaMaoObras.list[i].Codigo.val) {
            var valorTotalItem = Globalize.parseFloat(itemPedido.ValorTotalItem.toString());
            var valorTotal = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorTotal.val());
            var valorTotalServicos = 0;

            var valorTotalServicosTotal = Globalize.parseFloat(_ordemServicoVendaMaoObras.ValorServicos.val());
            valorTotalServicos = valorTotalServicosTotal - valorTotalItem;
            _ordemServicoVendaMaoObras.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));
            _ordemServicoVenda.ValorServicos.val(Globalize.format(valorTotalServicos, "n2"));

            valorTotal = valorTotal - valorTotalItem;
            _ordemServicoVendaMaoObras.ValorTotal.val(Globalize.format(valorTotal, "n2"));
            _ordemServicoVendaPecas.ValorTotal.val(Globalize.format(valorTotal, "n2"));
            _ordemServicoVenda.ValorTotal.val(Globalize.format(valorTotal, "n2"));

            break;
        }
    }
}

function limparCamposOrdemServicoVendaMaoObra() {
    var valorServicosPagina = _ordemServicoVendaMaoObras.ValorServicos.val();
    var valorTotalPagina = _ordemServicoVendaMaoObras.ValorTotal.val();

    LimparCampos(_ordemServicoVendaMaoObras);

    _ordemServicoVendaMaoObras.ValorServicos.val(valorServicosPagina);
    _ordemServicoVendaMaoObras.ValorTotal.val(valorTotalPagina);
    _ordemServicoVendaMaoObras.ValorUnitario.enable(true);

    TipoServicoChange();

    _ordemServicoVenda.ValorDesconto.enable(true);
    _ordemServicoVenda.PercentualDesconto.enable(true);

    if (_configuracoesEmpresa.HabilitarTabelaValorOrdemServicoVenda) {
        _ordemServicoVendaMaoObras.KMTotalUnidade.visible(false);
        _ordemServicoVendaMaoObras.HoraTotalUnidade.visible(false);
        _ordemServicoVendaMaoObras.HoraTotalUnidade.enable(false);
        _ordemServicoVendaMaoObras.KMTotalUnidade.enable(false);
    }
}