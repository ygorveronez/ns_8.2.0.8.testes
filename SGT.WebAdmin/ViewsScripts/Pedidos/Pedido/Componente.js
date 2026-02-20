var _componenteFretePedido, _gridComponenteFretePedido;

var ComponenteFretePedido = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.TipoValor = PropertyEntity({});
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Pedidos.Pedido.ComponenteFrete.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false, precision: 2 }, maxlength: 10, text: Localization.Resources.Pedidos.Pedido.Valor.getRequiredFieldDescription(), required: true, visible: ko.observable(true) });
    this.Percentual = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false, precision: 3 }, maxlength: 10, text: Localization.Resources.Pedidos.Pedido.Percentual.getRequiredFieldDescription(), required: false, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarComponenteFretePedidoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });
};

function LoadComponenteFretePedido() {
    _componenteFretePedido = new ComponenteFretePedido();
    KoBindings(_componenteFretePedido, "knockoutComponenteFretePedido");

    BuscarComponentesDeFrete(_componenteFretePedido.ComponenteFrete, RetornoSelecaoComponenteFrete);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: ExcluirComponenteFretePedidoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pedidos.Pedido.ComponenteFrete, width: "60%" },
        { data: "Percentual", title: Localization.Resources.Pedidos.Pedido.Percentual, width: "15%" },
        { data: "Valor", title: Localization.Resources.Pedidos.Pedido.Valor, width: "15%" }
    ];

    _gridComponenteFretePedido = new BasicDataTable(_componenteFretePedido.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridComponenteFretePedido();
}

function RetornoSelecaoComponenteFrete(componenteSelecionado) {
    _componenteFretePedido.ComponenteFrete.val(componenteSelecionado.Descricao);
    _componenteFretePedido.ComponenteFrete.codEntity(componenteSelecionado.Codigo);

    if (componenteSelecionado.TipoValor == EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {
        _componenteFretePedido.Percentual.visible(true);
        _componenteFretePedido.Percentual.required = true;

        _componenteFretePedido.Valor.visible(false);
        _componenteFretePedido.Valor.required = false;
        _componenteFretePedido.Valor.val("");
    } else {
        _componenteFretePedido.Percentual.visible(false);
        _componenteFretePedido.Percentual.required = false;
        _componenteFretePedido.Percentual.val("");

        _componenteFretePedido.Valor.visible(true);
        _componenteFretePedido.Valor.required = true;
    }
}

function RecarregarGridComponenteFretePedido() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pedido.ComponentesFrete.val())) {
        $.each(_pedido.ComponentesFrete.val(), function (i, componenteFretePedido) {
            var componenteFreteGrid = new Object();

            componenteFreteGrid.Codigo = componenteFretePedido.Codigo;
            componenteFreteGrid.Descricao = componenteFretePedido.ComponenteFrete.Descricao;
            componenteFreteGrid.Valor = componenteFretePedido.Valor;
            componenteFreteGrid.Percentual = componenteFretePedido.Percentual;

            data.push(componenteFreteGrid);
        });
    }
    _gridComponenteFretePedido.CarregarGrid(data);
}

function ExcluirComponenteFretePedidoClick(data) {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermitePreencherValoresFrete, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.Pedido.VoceNaoPossuiPermissaoParaManipularComponentesFrete);
        return;
    }

    var componentesFretePedido = _pedido.ComponentesFrete.val();

    for (var i = 0; i < componentesFretePedido.length; i++) {
        if (data.Codigo == componentesFretePedido[i].Codigo) {
            componentesFretePedido.splice(i, 1);
            break;
        }
    }

    _pedido.ComponentesFrete.val(componentesFretePedido);

    RecarregarGridComponenteFretePedido();
}

function AdicionarComponenteFretePedidoClick(e, sender) {
    var sucesso = ValidarCamposObrigatorios(_componenteFretePedido);

    if (!sucesso) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.VerifiqueCampos);
        return;
    }

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermitePreencherValoresFrete, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.Pedido.VoceNaoPossuiPermissaoParaManipularComponentesFrete);
        return;
    }

    var componentesFretePedido = _pedido.ComponentesFrete.val();

    componentesFretePedido.push({ Codigo: guid(), ComponenteFrete: { Descricao: _componenteFretePedido.ComponenteFrete.val(), Codigo: _componenteFretePedido.ComponenteFrete.codEntity() }, Valor: _componenteFretePedido.Valor.val(), Percentual: _componenteFretePedido.Percentual.val() });

    _pedido.ComponentesFrete.val(componentesFretePedido);

    LimparCamposComponenteFretePedido();

    RecarregarGridComponenteFretePedido();
}

function LimparCamposComponenteFretePedido() {
    LimparCampos(_componenteFretePedido);
}

function LimparGridComponenteFretePedido() {
    _pedido.ComponentesFrete.val([]);
}