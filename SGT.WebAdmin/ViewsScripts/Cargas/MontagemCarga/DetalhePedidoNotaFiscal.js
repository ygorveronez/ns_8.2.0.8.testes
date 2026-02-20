/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _detalhePedidoNotaFiscal;
var _gridDetalhePedidoNotaFiscal;
var _crudNotaFiscalPedido;
var _notaFiscalPedido;

var DetalhePedidoNotaFisical = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.PesoTotalCubado = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCarga.PesoTotalCubado.getFieldDescription() });
    this.PesoTotalNFe = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Cargas.MontagemCarga.PesoTotalNFe.getFieldDescription() });
}

var NotaFiscalPedido = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.PesoCubado = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoCubado.getRequiredFieldDescription()), maxlength: 18, required: true });
}

var CRUDNotaFiscalPedido = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarNotaFiscalMontagemCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadDetalhePedidoNotaFiscal() {
    _detalhePedidoNotaFiscal = new DetalhePedidoNotaFisical();
    KoBindings(_detalhePedidoNotaFiscal, "knoutDetalhePedidoNotasFiscais");

    _notaFiscalPedido = new NotaFiscalPedido();
    _crudNotaFiscalPedido = new CRUDNotaFiscalPedido();

    KoBindings(_notaFiscalPedido, "knockoutEdicaoNFeMontagemCarga");
    KoBindings(_crudNotaFiscalPedido, "knockoutCRUDEdicaoNFeMontagemCarga");

    loadGridDetalhePedidoNotaFiscal();
}

function loadGridDetalhePedidoNotaFiscal() {
    // Opcoes
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarPedidoNotaFiscalMontagemCargaClick, icone: "", visibilidade: visibilidadeEditarPedidoNotaFiscalMontagemCarga };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 9, opcoes: [editar] };
   
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoPedido", visible: false },
        { data: "Numero", title: Localization.Resources.Cargas.MontagemCarga.Numero, width: "15%" },
        { data: "Chave", title: Localization.Resources.Cargas.MontagemCarga.ChaveNFe, width: "40%" },
        { data: "DataEmissao", title: Localization.Resources.Cargas.MontagemCarga.DataEmissao, width: "15%" },
        { data: "ValorTotal", title: Localization.Resources.Cargas.MontagemCarga.ValorTotal, width: "15%" },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCarga.PesoNFe, width: "15%" },
        { data: "PesoCubado", title: Localization.Resources.Cargas.MontagemCarga.PesoCubado, width: "15%" }
    ];

    _gridDetalhePedidoNotaFiscal = new BasicDataTable(_detalhePedidoNotaFiscal.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc }, { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: false });
    _gridDetalhePedidoNotaFiscal.CarregarGrid([]);
}

function preencherNotasFiscaisPedido(notasFiscais, notasFiscaisEnviar, codigoPedido) {
    _gridDetalhePedidoNotaFiscal.CarregarGrid(notasFiscais);
    if (notasFiscais.length > 0 && pedidoEstaSelecionado(codigoPedido) && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        preencherSelecaoDeNotasFiscaisDoPedido(codigoPedido, notasFiscais, notasFiscaisEnviar);
        $("#liDetalhePedidoNotasFiscais").show();
    }
    else
        $("#liDetalhePedidoNotasFiscais").hide();
}

function preencherSelecaoDeNotasFiscaisDoPedido(codigoPedido, notasFiscais, notasFiscaisEnviar) {
    var notasSelecionadasDoPedido = new Array();
    if (notasFiscaisEnviar.length <= 0) {
        notasSelecionadasDoPedido = NOTAS_FISCAIS_SELECIONADAS().filter(function (nota) {
            return nota.CodigoPedido == codigoPedido;
        });
    }
    else
        notasSelecionadasDoPedido = notasFiscaisEnviar;

    _gridDetalhePedidoNotaFiscal.SetarSelecionados(notasSelecionadasDoPedido);
}

function pedidoEstaSelecionado(codigoPedido) {
    return PEDIDOS_SELECIONADOS().filter(function (registro) {
        return registro.Codigo == codigoPedido;
    }).length > 0;
}

function salvarNotasSelecionadas() {
    removerNotasFiscaisDoPedido(_detalhePedido.Codigo.val());

    var novasNotas = NOTAS_FISCAIS_SELECIONADAS().concat(_gridDetalhePedidoNotaFiscal.ListaSelecionados());
    NOTAS_FISCAIS_SELECIONADAS(novasNotas);
}

function removerNotasFiscaisDoPedido(codigoPedido) {
    var novasNotas = NOTAS_FISCAIS_SELECIONADAS().filter(function (nota) {
        return nota.CodigoPedido != codigoPedido;
    });

    NOTAS_FISCAIS_SELECIONADAS(novasNotas);
}

function obterNotasEnviar() {
    return NOTAS_FISCAIS_SELECIONADAS().map(function (nota) {
        return {
            Codigo: nota.Codigo,
            CodigoPedido: nota.CodigoPedido
        };
    });
}

function editarPedidoNotaFiscalMontagemCargaClick(data) {
    _notaFiscalPedido.Codigo.val(data.Codigo);
    _notaFiscalPedido.PesoCubado.val(data.PesoCubado);

    Global.abrirModal("divEdicaoNotaFiscalMontagemCarga");
    $("#divEdicaoNotaFiscalMontagemCarga").one('hidden.bs.modal', function () {
        LimparCampo(_notaFiscalPedido);
    });   
}

function visibilidadeEditarPedidoNotaFiscalMontagemCarga() {
    return _detalhePedido.PermiteInformarPesoCubadoNaMontagemDaCarga.val();
}

function atualizarNotaFiscalMontagemCargaClick() {
    if (!ValidarCamposObrigatorios(_notaFiscalPedido)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.MontagemCarga.NecessarioInformarPesoCubado);
        return;
    }

    executarReST("MontagemCargaPedido/AtualizarNotaFiscal", { Codigo: _notaFiscalPedido.Codigo.val(), PesoCubado: _notaFiscalPedido.PesoCubado.val() }, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.MontagemCarga.NotaFiscalAtualizadaComSucesso);
            var notasFiscaisDoPedido = _gridDetalhePedidoNotaFiscal.BuscarRegistros();
            var pesoTotalCubado = 0;
            for (var i = 0; i < notasFiscaisDoPedido.length; i++) {
                if (notasFiscaisDoPedido[i].Codigo == _notaFiscalPedido.Codigo.val())
                    notasFiscaisDoPedido[i].PesoCubado = _notaFiscalPedido.PesoCubado.val();
                var valorSomar = notasFiscaisDoPedido[i].PesoCubado.replace(".", "");
                pesoTotalCubado += parseFloat(valorSomar.replace(",", "."));
            }
            _detalhePedidoNotaFiscal.PesoTotalCubado.val(pesoTotalCubado.toLocaleString(undefined, { minimumFractionDigits: 2 }));
            _gridDetalhePedidoNotaFiscal.CarregarGrid(notasFiscaisDoPedido);
            Global.fecharModal("divEdicaoNotaFiscalMontagemCarga");
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}