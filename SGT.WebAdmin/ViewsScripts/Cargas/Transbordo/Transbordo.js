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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTransbordo.js" />
/// <reference path="EtapaTransbordo.js" />
/// <reference path="Integracao.js" />
/// <reference path="TransbordoSignalR.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransbordo;
var _transbordo;
var _pesquisaTransbordo;
var _gridEntregasTransbordo;
var _gridCTesTransbordados;

var CargaEntregaMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
};

const camposControleEntregaColeta = [
    "DataInicioViagem", "LancamentoColetas", "LancamentoEntregas",
];


var PesquisaTransbordo = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Carga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Transbordo.NumeroTransbordo.getFieldDescription(), def: "", configInt: { precision: 0, allowZero: false, thousands: "" }, getType: typesKnockout.int });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Transbordo.TransportadoraQueFezTransbordo.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Veiculo.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.LocalidadeTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Transbordo.LocalidadeTransbordo.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Cargas.Transbordo.DataInicio.getFieldDescription(), getType: typesKnockout.date });
    this.SituacaoTransbordo = PropertyEntity({ val: ko.observable(EnumSituacaoTransbordo.Todas), options: EnumSituacaoTransbordo.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: EnumSituacaoTransbordo.Todas });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Cargas.Transbordo.DataLimite.getFieldDescription(), dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTransbordo.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Transbordo = function () {
    this.Grid = PropertyEntity({ idGrid: guid() });
    this.Entrega = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.EntregaTransbordo = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Filial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Carga.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true), enable: ko.observable(true), visibleFade: ko.observable(true) });
    this.Cargas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Carga.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.DataTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Cargas.Transbordo.DataTransbordo.getRequiredFieldDescription(), issue: 979, def: "", getType: typesKnockout.dateTime, required: true, enable: ko.observable(true) });
    this.NumeroTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription(), def: "", enable: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Transportador.getRequiredFieldDescription(), issue: 981, idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Veiculo.getRequiredFieldDescription(), issue: 982, idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), text: ko.observable("*Veículo (Carreta)"), idBtnSearch: guid() });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), text: ko.observable("*Veículo (Carreta 2)"), idBtnSearch: guid() });
    this.TerceiroReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), text: ko.observable("*Veículo (Carreta 3)"), idBtnSearch: guid() });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getRequiredFieldDescription(), issue: 983, idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.LocalidadeTransbordo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Transbordo.LocalidadeTransbordo.getRequiredFieldDescription(), issue: 980, idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoDeOperacao.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoCarga.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.MotivoTransbordo = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), required: true, text: Localization.Resources.Gerais.Geral.Motivo.getRequiredFieldDescription(), issue: 984, def: "", enable: ko.observable(true) });
    this.SituacaoTransbordo = PropertyEntity({ val: ko.observable(EnumSituacaoTransbordo.AgInformacoes), options: EnumSituacaoTransbordo.obterOpcoes(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: EnumSituacaoTransbordo.AgInformacoes });

    this.GerarTransbordo = PropertyEntity({ eventClick: gerarTransbordoClick, type: types.event, text: Localization.Resources.Cargas.Transbordo.GerarTransbordo, visible: ko.observable(true) });

    this.Status = PropertyEntity({ val: ko.observable("A"), def: "A" });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(true), def: true, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Cargas.Transbordo.MarcarDesmarcarTodos, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Cargas.Transbordo.LimparGerarNovo, visible: ko.observable(true) });

    this.CamposControleEntregaColeta = PropertyEntity({ val: ko.observable("")});

    this.Habilitar = PropertyEntity({ val: ko.observable(true) });

    this.SelecionarTodosCamposControle = PropertyEntity({ val: ko.observable(false) });

    this.SelecionarTodosCamposControle.val.subscribe(function (val) {
        aplicarSelecionarTodosCamposControle(val);
    });

    this.DataInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioViagem.getFieldDescription(), val: ko.observable(false), def: false });
    this.LancamentoColetas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LancamentoColetas.getFieldDescription(), val: ko.observable(false), def: false });
    this.LancamentoEntregas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LancamentoEntregas, val: ko.observable(false), def: false });
    
};

//*******EVENTOS*******

function loadTransbordo() {

    _transbordo = new Transbordo();
    KoBindings(_transbordo, "knockoutCadastroTransbordo");

    _pesquisaTransbordo = new PesquisaTransbordo();
    KoBindings(_pesquisaTransbordo, "knockoutPesquisaTransbordo", false, _pesquisaTransbordo.Pesquisar.id);

    HeaderAuditoria("Transbordo", _transbordo);

    let situacoesPermitidasParaTransbordo = [EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.AgImpressaoDocumentos, EnumSituacoesCarga.AgIntegracao];

    new BuscarCargas(_transbordo.Carga, retornoCarga, null, situacoesPermitidasParaTransbordo, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarCargas(_transbordo.Cargas, null, null, situacoesPermitidasParaTransbordo, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarMotoristas(_transbordo.Motorista, null, null, null, true);
    new BuscarLocalidades(_transbordo.LocalidadeTransbordo);
    new BuscarTransportadores(_transbordo.Empresa);
    new BuscarTiposOperacao(_transbordo.TipoOperacao);
    new BuscarTiposdeCarga(_transbordo.TipoCarga);

    new BuscarLocalidades(_pesquisaTransbordo.LocalidadeTransbordo);
    new BuscarTransportadores(_pesquisaTransbordo.Empresa);
    new BuscarCargas(_pesquisaTransbordo.Carga);

    new BuscarVeiculos(_transbordo.Veiculo, null, _transbordo.Empresa);
    new BuscarVeiculos(_pesquisaTransbordo.Veiculo);

    new BuscarVeiculos(_transbordo.Reboque, ValidaReboqueSelecionados(_transbordo), null, null, null, true, null, null, null, null, _transbordo.Carga.Codigo, "1");
    new BuscarVeiculos(_transbordo.SegundoReboque, ValidaSegundoReboqueSelecionados(_transbordo), null, null, null, true, null, null, null, null, _transbordo.Carga.Codigo, "1");
    new BuscarVeiculos(_transbordo.TerceiroReboque, ValidaTerceiroReboqueSelecionados(_transbordo), null, null, null, true, null, null, null, null, _transbordo.Carga.Codigo, "1");

    _transbordo.Cargas.multiplesEntities.subscribe(atualizarInformacoesMultiplasCargas);

    if (_CONFIGURACAO_TMS.PermiteSelecionarPlacaPorTipoVeiculoTransbordo) {
        _transbordo.NumeroReboques.val.subscribe(atualizarInformacoesReboques);
    }

    controlarCamposTransbordo();
    LoadEtapaTransbordo();
    buscarTransbordos();
    PreencherGridEntregasParaTransbordo();
    LoadIntegracoes();
    LoadConexaoSignalRTransbordo();
    aplicarSelecionarTodosCamposControle(true)
}

function controlarCamposTransbordo() {
    if (_CONFIGURACAO_TMS.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo) {
        _transbordo.Carga.visible(false);
        _transbordo.Carga.required(false);
        _transbordo.Cargas.visible(true);
        _transbordo.Cargas.required(true);
        _transbordo.TipoOperacao.visible(true);
        _transbordo.TipoOperacao.required(true);
        _transbordo.TipoCarga.visible(true);
        _transbordo.TipoCarga.required(true);
    }
}

function ValidaReboqueSelecionados(ko) {
    return function (data) {
        if (ko.SegundoReboque.codEntity() == data.Codigo || ko.TerceiroReboque.codEntity() == data.Codigo) {
            exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");
            data = {
                Codigo: 0,
                Placa: ''
            };
        }

        ko.Reboque.val(data.Placa);
        ko.Reboque.entityDescription(data.Placa);
        ko.Reboque.codEntity(data.Codigo);
    }
}

function ValidaSegundoReboqueSelecionados(ko) {
    return function (data) {
        if (ko.Reboque.codEntity() == data.Codigo) {
            exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 2)", "Não é possível selecionar duas carretas iguais.");
            data = {
                Codigo: 0,
                Placa: ''
            };
        }

        ko.SegundoReboque.val(data.Placa);
        ko.SegundoReboque.entityDescription(data.Placa);
        ko.SegundoReboque.codEntity(data.Codigo);
    }
}

function ValidaTerceiroReboqueSelecionados(ko) {
    return function (data) {
        if (ko.Reboque.codEntity() == data.Codigo || ko.SegundoReboque.codEntity() == data.Codigo) {
            exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 2)", "Não é possível selecionar duas carretas iguais.");
            data = {
                Codigo: 0,
                Placa: ''
            };
        }

        ko.TerceiroReboque.val(data.Placa);
        ko.TerceiroReboque.entityDescription(data.Placa);
        ko.TerceiroReboque.codEntity(data.Codigo);
    }
}

function PreencherGridEntregasParaTransbordo() {
    _transbordo.SelecionarTodos.visible(true);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _transbordo.SelecionarTodos,
        somenteLeitura: false,
    };

    _gridEntregasTransbordo = new GridView(_transbordo.Entrega.idGrid, "Transbordo/ConsultarEntregasParaTransbordo", _transbordo, null, null, null, null, null, null, multiplaescolha);
    _gridCTesTransbordados = new GridView(_transbordo.EntregaTransbordo.idGrid, "Transbordo/ConsultarEntregasDoTransbordo", _transbordo, null);
}

function retornoCarga(carga) {
    _transbordo.Carga.val(carga.CodigoCargaEmbarcador);
    _transbordo.Carga.codEntity(carga.Codigo);
    _transbordo.Filial.val(carga.Filial);
    _transbordo.NumeroReboques.val(carga.NumeroReboques);

    _gridEntregasTransbordo.CarregarGrid(function () {
        _transbordo.SelecionarTodos.visible(true);
        _transbordo.Carga.visibleFade(false);
    });
}

function atualizarInformacoesMultiplasCargas() {
    var cargas = recursiveMultiplesEntities(_transbordo.Cargas);
    if (cargas.length == 0) {
        _transbordo.Carga.visibleFade(true);
        return;
    }

    _gridEntregasTransbordo.CarregarGrid(function () {
        _transbordo.SelecionarTodos.visible(true);
        _transbordo.Carga.visibleFade(false);
    });
}

function atualizarInformacoesReboques() {
    var numeroReboques = _transbordo.NumeroReboques.val();

    if (numeroReboques > 0) {
        _transbordo.Reboque.visible(true);
        _transbordo.Reboque.required(true);
    }

    if (numeroReboques > 1) {
        _transbordo.SegundoReboque.visible(true);
        _transbordo.SegundoReboque.required(true);

    }

    if (numeroReboques > 2) {
        _transbordo.TerceiroReboque.visible(true);
        _transbordo.TerceiroReboque.required(true);
    }
}

function buscarMenuCTes() {
    var baixarDACTE = { descricao: Localization.Resources.Cargas.Transbordo.BaixarDACTE, id: guid(), metodo: baixarDacteClick, icone: "" };
    var baixarXML = { descricao: Localization.Resources.Cargas.Transbordo.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "" };
    var visualizar = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesCTeClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarXML, visualizar] };
    return menuOpcoes;
}

function detalhesCTeClick(e, sender) {
    var codigo = parseInt(e.CodigoCTE);
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, e.Codigo, instancia);
        }
    }, permissoesSomenteLeituraCTe);
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function montarPayloadCamposControle() {
    const payload = {};

    camposControleEntregaColeta.forEach(campo => {
        if (_transbordo[campo] && ko.isObservable(_transbordo[campo].val)) {
            payload[campo] = _transbordo[campo].val();
        }
    });

    return payload;
}

function aplicarSelecionarTodosCamposControle(marcar) {
    camposControleEntregaColeta.forEach(campo => {
        if (_transbordo[campo] && ko.isObservable(_transbordo[campo].val)) {
            _transbordo[campo].val(marcar);
        }
    });
}


function gerarTransbordoClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        var entregasSelecionadas;

        if (_transbordo.SelecionarTodos.val())
            entregasSelecionadas = _gridEntregasTransbordo.ObterMultiplosNaoSelecionados();
        else
            entregasSelecionadas = _gridEntregasTransbordo.ObterMultiplosSelecionados();

        if (entregasSelecionadas.length > 0 || _transbordo.SelecionarTodos.val()) {
            _transbordo.Entrega.list = new Array();
            $.each(entregasSelecionadas, function (i, entrega) {
                var map = new CargaEntregaMap();
                map.Codigo.val = entrega.Codigo;
                _transbordo.Entrega.list.push(map);
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.Atencao.ObrigatorioInformarPeloMenosEntregaTransbordo);
            valido = false;
        }

        var transbordo = RetornarObjetoPesquisa(_transbordo)

        var payload = montarPayloadCamposControle();

        const algumCheckboxMarcado = Object.values(payload).some(v => v === true);

        if (!algumCheckboxMarcado) {
            exibirMensagem(
                tipoMensagem.atencao,
                Localization.Resources.Gerais.Geral.Atencao,
                "Pelo menos uma opção de migração deve ser selecionada nos campos de controle de entrega."
            );
            return;
        }

        var json = JSON.stringify(payload);
        transbordo["CamposControleEntregaColeta"] = json

        executarReST("Transbordo/GerarTransbordo", transbordo, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_CONFIGURACAO_TMS.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.TransbordoGeradoSucesso);
                        limparCamposTransbordo();
                        _gridTransbordo.CarregarGrid();
                    }
                    else {
                        PreencherTransbordo(arg.Data)
                        $("#tblEntregasTransbordo").show();
                        $("#tblEntregas").hide();
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.TransbordoGeradoSucesso);
                        _gridTransbordo.CarregarGrid();
                        _gridCTesTransbordados.CarregarGrid();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
}

function PreencherTransbordo(transbordo) {
    _transbordo.Carga.val(transbordo.Carga.CodigoCargaEmbarcador);
    _transbordo.Carga.codEntity(transbordo.Carga.Codigo);
    _transbordo.Filial.val(transbordo.Carga.Filial);
    _transbordo.Codigo.val(transbordo.Codigo);
    _transbordo.NumeroTransbordo.val(transbordo.NumeroTransbordo);
    _transbordo.DataTransbordo.val(transbordo.DataTransbordo);
    _transbordo.Veiculo.val(transbordo.Veiculo.Descricao);
    _transbordo.Veiculo.codEntity(transbordo.Veiculo.Codigo);
    _transbordo.LocalidadeTransbordo.codEntity(transbordo.LocalidadeTransbordo.Codigo);
    _transbordo.LocalidadeTransbordo.val(transbordo.LocalidadeTransbordo.Descricao);
    _transbordo.SituacaoTransbordo.val(transbordo.SituacaoTransbordo);
    _transbordo.MotivoTransbordo.val(transbordo.MotivoTransbordo);
    _transbordo.NumeroReboques.val(transbordo.Carga.NumeroReboques);

    _transbordo.Reboque.val(transbordo.Reboque.Descricao);
    _transbordo.Reboque.codEntity(transbordo.Reboque.Codigo);

    _transbordo.SegundoReboque.val(transbordo.SegundoReboque.Descricao);
    _transbordo.SegundoReboque.codEntity(transbordo.SegundoReboque.Codigo);

    _transbordo.TerceiroReboque.val(transbordo.TerceiroReboque.Descricao);
    _transbordo.TerceiroReboque.codEntity(transbordo.TerceiroReboque.Codigo);

    _transbordo.NumeroReboques.val(transbordo.Carga.NumeroReboques);

    SetarEnableCamposKnockout(_transbordo, false);
    PreencherGridCTesDoTransbordo();
    _transbordo.GerarTransbordo.visible(false);
    SetarEtapaTransbordo();

    _transbordo.Carga.visible(_CONFIGURACAO_TMS.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);
    _transbordo.Cargas.visible(!_CONFIGURACAO_TMS.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);
}

function PreencherGridCTesDoTransbordo() {
    _transbordo.SelecionarTodos.visible(false);
    _gridCTesTransbordados.CarregarGrid(function () {
        _transbordo.Carga.visibleFade(false);
    });
}

function cancelarClick(e) {
    limparCamposTransbordo();
}

//*******MÉTODOS*******

function buscarTransbordos() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: "clasEditar", evento: "onclick", metodo: editarTransbordo, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTransbordo = new GridView(_pesquisaTransbordo.Pesquisar.idGrid, "Transbordo/Pesquisa", _pesquisaTransbordo, menuOpcoes, null);
    _gridTransbordo.CarregarGrid();
}

function editarTransbordo(transbordoGrid) {
    buscarTransbordo(transbordoGrid.Codigo);
}

function buscarTransbordo(codigo) {
    limparCamposTransbordo();
    _transbordo.Codigo.val(codigo);
    BuscarPorCodigo(_transbordo, "Transbordo/BuscarPorCodigo", function (arg) {
        $("#tblEntregasTransbordo").show();
        $("#tblEntregas").hide();
        PreencherTransbordo(arg.Data);
        SetarEtapaTransbordo();
        _pesquisaTransbordo.ExibirFiltros.visibleFade(false);
        _transbordo.GerarTransbordo.visible(false);
        DefinirTab();
    });
}

function limparCamposTransbordo() {
    _transbordo.GerarTransbordo.visible(true);
    LimparCampos(_transbordo);

    SetarEnableCamposKnockout(_transbordo, true);
    _transbordo.NumeroTransbordo.enable(false);

    $("#tblEntregasTransbordo").hide();
    $("#tblEntregas").show();

    _transbordo.Carga.visible(!_CONFIGURACAO_TMS.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);
    _transbordo.Cargas.visible(_CONFIGURACAO_TMS.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);
    _transbordo.Carga.visibleFade(true);
    SetarEtapaTransbordo();
}
