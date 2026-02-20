
var _modalPedidoImportacaoAtraso;
var _gridModalPedidoImportacaoAtraso;
var _parametrosMotivosImporacaoAtrasado = [];

var ModalPedidoImportacaoAtraso = function () {
    this.Grid = new PropertyEntity({ type: types.local, list: [] });
    this.Callback = new PropertyEntity({ type: types.local, handle: function () { } });

    this.Carga = new PropertyEntity({ text: Localization.Resources.Gerais.Geral.Carga });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Pedidos.Pedido.Motivo.getRequiredFieldDescription(), idBtnSearch: guid() });

    this.Salvar = PropertyEntity({ eventClick: salvarVinculoImportacaoAtraso, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(false) });
    this.Confirmar = PropertyEntity({ eventClick: confirmarVinculoImportacaoAtraso, type: types.event, text: Localization.Resources.Pedidos.Pedido.ConfirmarVinculos });
}

function callbackImportacaoAtrasadaPreProcessamento(tabela) {
    return new Promise(function (resolve) {
        var dados = {
            Dados: JSON.stringify(tabela.dados)
        };

        executarReST("Pedido/VerificarPedidosPreImportacao", dados, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                return resolve(false);
            }

            if (!arg.Data) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                return resolve(false);
            }

            var cargasAtrasadas = arg.Data.CargasAtrasadas;
            if (!cargasAtrasadas.length) {
                return resolve(true);
            }

            _modalPedidoImportacaoAtraso.Callback.handle = resolve;
            carregarEAbrirModalPedidoImportacaoAtraso(cargasAtrasadas);
        });
    });
}


function loadModalPedidoImportacaoAtraso() {
    _modalPedidoImportacaoAtraso = new ModalPedidoImportacaoAtraso();
    KoBindings(_modalPedidoImportacaoAtraso, "divModalPedidoImportacaoAtraso");

    $('#divModalPedidoImportacaoAtraso').on('hidden.bs.modal', function () {
        modalPedidoImportacaoAtrasoFechado();
    });

    loadGridModalPedidoImportacaoAtraso();

    new BuscarMotivoImportacaoPedidoAtrasada(_modalPedidoImportacaoAtraso.Motivo);
}


function loadGridModalPedidoImportacaoAtraso() {
    var editarVinculo = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarVinculoImportacaoAtraso, icone: "" };
    var menuOpcoesPedido = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [editarVinculo] };

    var headers = [
        { data: "Carga", title: Localization.Resources.Gerais.Geral.Carga, width: "40%" },
        { data: "MotivoAtraso", title: Localization.Resources.Pedidos.Pedido.MotivoAtraso, width: "40%" },
        { data: "CodigoMotivoAtraso", visible: false },
    ];

    _gridModalPedidoImportacaoAtraso = new BasicDataTable(_modalPedidoImportacaoAtraso.Grid.id, headers, menuOpcoesPedido);
    _gridModalPedidoImportacaoAtraso.CarregarGrid([]);
}

function carregarEAbrirModalPedidoImportacaoAtraso(cargasAtrasadas) {
    _parametrosMotivosImporacaoAtrasado = [];
    var dados = [];

    for (var i = 0; i < cargasAtrasadas.length; i++) {
        dados.push({
            Carga: cargasAtrasadas[i],
            CodigoMotivo: 0,
            DescricaoMotivo: ""
        });
    }

    _modalPedidoImportacaoAtraso.Grid.list = dados;
    recarregarGridImportacaoAtraso();
    Global.abrirModal('divModalPedidoImportacaoAtraso');
}

function modalPedidoImportacaoAtrasoFechado() {
    if ($.isFunction(_modalPedidoImportacaoAtraso.Callback.handle))
        _modalPedidoImportacaoAtraso.Callback.handle(false);
    _modalPedidoImportacaoAtraso.Callback.handle = null;
}

function editarVinculoImportacaoAtraso(data) {
    _modalPedidoImportacaoAtraso.Salvar.visible(true);
    _modalPedidoImportacaoAtraso.Carga.val(data.Carga);
    _modalPedidoImportacaoAtraso.Motivo.val(data.MotivoAtraso);
    _modalPedidoImportacaoAtraso.Motivo.entityDescription(data.MotivoAtraso);
    _modalPedidoImportacaoAtraso.Motivo.codEntity(data.CodigoMotivoAtraso);
}

function salvarVinculoImportacaoAtraso() {
    _modalPedidoImportacaoAtraso.Salvar.visible(false);
    var dados = _modalPedidoImportacaoAtraso.Grid.list.slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].Carga == _modalPedidoImportacaoAtraso.Carga.val()) {
            dados[i].DescricaoMotivo = _modalPedidoImportacaoAtraso.Motivo.val();
            dados[i].CodigoMotivo = _modalPedidoImportacaoAtraso.Motivo.codEntity();
        }
    }

    LimparCampoEntity(_modalPedidoImportacaoAtraso.Motivo);
    _modalPedidoImportacaoAtraso.Grid.list = dados;
    recarregarGridImportacaoAtraso();
}

function confirmarVinculoImportacaoAtraso() {
    if (!validarTodasCargasAtrasadasPossuemMotivo())
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.Pedido.ObrigatorioInformarMotivoDasCargasAtrasadas);

    if ($.isFunction(_modalPedidoImportacaoAtraso.Callback.handle))
        _modalPedidoImportacaoAtraso.Callback.handle(true);
    _modalPedidoImportacaoAtraso.Callback.handle = null;

    Global.fecharModal('divModalPedidoImportacaoAtraso');
    _parametrosMotivosImporacaoAtrasado = obterImportacaoAtrasoParametros();
}

function validarTodasCargasAtrasadasPossuemMotivo() {
    var dados = _modalPedidoImportacaoAtraso.Grid.list.slice();

    for (var i = 0; i < dados.length; i++) {
        if (dados[i].CodigoMotivo == 0) return false;
    }

    return true;
}

function obterImportacaoAtrasoParametros() {
    var dados = _modalPedidoImportacaoAtraso.Grid.list.slice();

    for (var i = 0; i < dados.length; i++) {
        dados[i] = {
            Carga: dados[i].Carga,
            Motivo: dados[i].CodigoMotivo,
        };
    }

    return dados;
}

function recarregarGridImportacaoAtraso() {
    var dados = _modalPedidoImportacaoAtraso.Grid.list.slice();

    for (var i = 0; i < dados.length; i++) {
        dados[i] = {
            Carga: dados[i].Carga,
            MotivoAtraso: dados[i].DescricaoMotivo,
            CodigoMotivoAtraso: dados[i].CodigoMotivo,
        };
    }

    _gridModalPedidoImportacaoAtraso.CarregarGrid(dados);
}