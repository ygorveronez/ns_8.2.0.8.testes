/// <reference path="CargaLacre.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Carga.js" />
/// <reference path="CargaCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="CargaLeilao.js" />
/// <reference path="../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="CargaPercurso.js" />
/// <reference path="../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="CargaFreteComissao.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="CargaComplementoFrete.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />
/// <reference path="Pessoa.js" />
/// <reference path="../../../../js/Global/CRUD.js" />

var _descarga;
var _gridRestricao;

//*******MAPEAMENTO KNOUCKOUT*******

var Descarga = function () {

    /** ---> DADOS DE DESCARGA <--- */
    this.ValorPorPallet = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorPorPallet.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), required: false, getType: typesKnockout.decimal, maxlength: 6, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorPorVolume = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ValorPorVolume.getFieldDescription(), def: "0,000", val: ko.observable("0,000"), required: false, getType: typesKnockout.decimal, maxlength: 6, configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.HoraLimiteDescarga = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.HoraLimiteParaDescarga.getFieldDescription(), getType: typesKnockout.time, required: false });
    this.Distribuidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.Distribuidor.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.VeiculoDistribuidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.Veiculo.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.NaoRecebeCargaCompartilhada = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NaoRecebeCargaCompartilhada, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.FilialResponsavelRedespacho = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.FilialResponsavelPeloRedespacho.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.TipoDeCarga.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.NaoExigePreenchimentoDeChecklistEntrega = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NaoExigePreenchimentoDeChecklistEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    /** ---> DADOS DE AGENDAMENTO <--- */
    this.ExigeAgendamento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExigeAgendamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AgendamentoExigeNotaFiscal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AgendamentoExigeNotaFiscal, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AgendamentoDescargaObrigatorio = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AgendamentoDescargaObrigatorio, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TempoAgendamento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TempoAgendamento, val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(true) });
    this.FormaAgendamento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FormaAgendamento, val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(true) });
    this.LinkParaAgendamento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.LinkParaAgendamento, val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(true) });
    this.PossuiCanhotoDeDuasOuMaisPaginas = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PossuiCanhotoDeDuasOuMaisPaginas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.QuantidadeDePaginasDoCanhoto = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.QuantidadeDePaginasDoCanhoto, val: ko.observable(Localization.Resources.Pessoas.Pessoa.QuantidadeDePaginasDoCanhoto), def: ko.observable(Localization.Resources.Pessoas.Pessoa.QuantidadeDePaginasDoCanhoto), getType: typesKnockout.int, visible: ko.observable(true) });
    this.ExigeSenhaNoAgendamento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExigeSenhaNoAgendamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.AdicionarRestricaoEntrega = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), idBtnSearch: guid() });
    this.Restricoes = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), required: true, idGrid: guid() });
    this.Restricoes.val.subscribe(function () {
        _pessoa.Restricoes.val(JSON.stringify(_descarga.Restricoes.val().map(function (r) { return r.Codigo; })));
        RenderizarGridRestricoes();
    });
}

//*******EVENTOS*******

function loadValorDescarga() {
    _descarga = new Descarga();
    KoBindings(_descarga, "knoutDescarga");

    LoadGridRestricoes();

    new BuscarTransportadores(_descarga.Distribuidor);
    new BuscarVeiculos(_descarga.VeiculoDistribuidor, null, _descarga.Distribuidor);
    new BuscarRestricaoEntrega(_descarga.AdicionarRestricaoEntrega, AdicionarRestricaoAGrid, _gridRestricao);
    new BuscarFilial(_descarga.FilialResponsavelRedespacho);
    new BuscarTiposdeCarga(_descarga.TipoDeCarga);

    $("#liDescarga").show();
}

function LoadGridRestricoes() {
    //-- Grid
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: Localization.Resources.Gerais.Geral.Excluir,
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: function (data) {
                    RemoverRestricaoClick(data);
                }
            }
        ]
    };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "90%", className: "text-align-left" },
    ];

    // Grid
    _gridRestricao = new BasicDataTable(_descarga.Restricoes.idGrid, header, menuOpcoes, null, null, 10);
    _gridRestricao.CarregarGrid([]);
}

//*******MÉTODOS*******
function AdicionarRestricaoAGrid(data) {
    var dataGrid = _descarga.Restricoes.val();

    data.forEach(function (r) {
        var restricao = {
            Codigo: r.Codigo,
            Descricao: r.Descricao,
        };

        dataGrid.push(restricao);
    });

    _descarga.Restricoes.val(dataGrid);
}

function ListarRestricoes(data) {
    if (data.Descarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        _descarga.Restricoes.val(data.Descarga.RestricoesDescarga);
}

function RemoverRestricaoClick(data) {
    var dataGrid = _gridRestricao.BuscarRegistros();

    for (var i = 0; i < dataGrid.length; i++) {
        if (data.Codigo == dataGrid[i].Codigo) {
            dataGrid.splice(i, 1);
            break;
        }
    }

    _descarga.Restricoes.val(dataGrid);
}

function RenderizarGridRestricoes() {
    var data = _descarga.Restricoes.val();

    _gridRestricao.CarregarGrid(data);
}

function LimparCamposDescarga() {
    if (_descarga != null) {
        LimparCampos(_descarga);
        _descarga.Restricoes.val([]);
    }
}