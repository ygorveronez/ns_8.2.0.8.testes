//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaDadosEmissaoValePedagio;
var _cargaDadosEmissaoValePedagio;
var _pesquisaCargaDadosEmissaoValePedagio;

var PesquisaCargaDadosEmissaoValePedagio = function () {
    this.Carga = PropertyEntity({});
}

var CargaDadosEmissaoValePedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ idGrid: guid() });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.NumeroComprovante = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoComprovante.getRequiredFieldDescription(), maxlength: 20, required: true, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: '' }, });
    this.CodigoAgendamentoPorto = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CodigoDeAgendamentoNoPorto.getFieldDescription(), maxlength: 16 });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Fornecedor.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Responsavel.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Valor.getRequiredFieldDescription(), maxlength: 15, required: true, getType: typesKnockout.decimal, def: "0,00", val: ko.observable("0,00"), visible: ko.observable(true), configDecimal: { precision: 2, allowZero: true, allowNegative: false } });

    this.TotalValePedagio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), text: ko.observable(""), visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.ValorPorEixoValePedagio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), text: ko.observable(""),visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.EixosPorCargaValePedagio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), text: ko.observable(""), visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCargaDadosEmissaoValePedagioClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarValePedagio, visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadCargaDadosEmissaoValePedagio() {

    _cargaDadosEmissaoValePedagio = new CargaDadosEmissaoValePedagio();
    KoBindings(_cargaDadosEmissaoValePedagio, "tabValePedagio_" + _cargaAtual.DadosEmissaoFrete.id);
    $("#tabValePedagio_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    _pesquisaCargaDadosEmissaoValePedagio = new PesquisaCargaDadosEmissaoValePedagio();

    _cargaDadosEmissaoValePedagio.Carga.val(_cargaAtual.Codigo.val());
    _cargaDadosEmissaoValePedagio.Carga.def = _cargaAtual.Codigo.val();
    _pesquisaCargaDadosEmissaoValePedagio.Carga.val(_cargaAtual.Codigo.val());
    
    _cargaDadosEmissaoValePedagio.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable() || _cargaAtual.EtapaCTeNFs.enable());
    _cargaDadosEmissaoValePedagio.Adicionar.enable(_cargaDadosEmissaoValePedagio.Pedido.enable());

    new BuscarClientes(_cargaDadosEmissaoValePedagio.Fornecedor, null, true, null, null, null, null, null, null, "J");
    new BuscarClientes(_cargaDadosEmissaoValePedagio.Responsavel, null, true, null, null, null, null, null, null, "J");

    ConfigurarGridCargaDadosEmissaoValePedagio();
}

function AdicionarCargaDadosEmissaoValePedagioClick(e, sender) {
    Salvar(e, "CargaValePedagio/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValePedagioAdicionadoComSucesso);
                _gridCargaDadosEmissaoValePedagio.CarregarGrid();
                LimparCamposCargaDadosEmissaoValePedagio();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function ExcluirCargaDadosEmissaoValePedagio(valePedagio) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.RealmenteDesejaExcluirValePedagio.format(valePedagio.NumeroComprovante), function () {

        _cargaDadosEmissaoValePedagio.Codigo.val(valePedagio.Codigo);

        ExcluirPorCodigo(_cargaDadosEmissaoValePedagio, "CargaValePedagio/Excluir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ValePedagioExcluidoComSucesso);
                    _gridCargaDadosEmissaoValePedagio.CarregarGrid();
                    LimparCamposCargaDadosEmissaoValePedagio();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

//*******MÉTODOS*******

function ConfigurarGridCargaDadosEmissaoValePedagio() {
    var menuOpcoes = null;

    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: ExcluirCargaDadosEmissaoValePedagio, tamanho: "20", icone: "" };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), evento: "onclick", metodo: OpcaoAuditoria("CargaValePedagio", "Codigo", _notaFiscal), tamanho: 7, icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    if (_cargaDadosEmissaoValePedagio.Pedido.enable())
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [excluir, auditar] };

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarValePedagio, _PermissoesPersonalizadasCarga) || _FormularioSomenteLeitura)
        menuOpcoes = null;

    if (menuOpcoes == null && PermiteAuditar())
        menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Auditar, opcoes: [auditar], tamanho: 10 };

    _gridCargaDadosEmissaoValePedagio = new GridView(_cargaDadosEmissaoValePedagio.Carga.idGrid, "CargaValePedagio/Pesquisa", _pesquisaCargaDadosEmissaoValePedagio, menuOpcoes, null);
    _gridCargaDadosEmissaoValePedagio.CarregarGrid();

    CarregarValoresValePedagio();
}

function CarregarValoresValePedagio() {
    executarReST("CargaValePedagio/CarregarValoresValePedagio", { Codigo: _cargaDadosEmissaoValePedagio.Carga.val() }, function (arg) {
        if (arg.Data != null) {
            _cargaDadosEmissaoValePedagio.TotalValePedagio.val(arg.Data.TotalValePedagio); 
            _cargaDadosEmissaoValePedagio.EixosPorCargaValePedagio.val(arg.Data.EixosPorCargaValePedagio);
            _cargaDadosEmissaoValePedagio.ValorPorEixoValePedagio.val(arg.Data.ValorPorEixoValePedagio);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}
function LimparCamposCargaDadosEmissaoValePedagio() {
    LimparCampos(_cargaDadosEmissaoValePedagio);
}