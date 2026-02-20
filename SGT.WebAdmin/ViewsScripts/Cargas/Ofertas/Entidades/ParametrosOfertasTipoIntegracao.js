/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../Consultas/Empresa.js" />
/// <reference path="../ParametrosOfertas.js" />

var _parametrosOfertasTipoIntegracao;
var _gridParametrosOfertasTipoIntegracao;

var ParametrosOfertasTipoIntegracao = function () {
    this.TipoIntegracao = PropertyEntity({ text: "Sistema de integração", val: ko.observable(""), def: "", options: ko.observable([]), required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarParametrosOfertasTipoIntegracaoClick, type: types.event, text: "Adicionar Tipo de Integração", visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local, id: guid() });
}

function LoadParemterosOfertasTipoIntegracao() {
    _parametrosOfertasTipoIntegracao = new ParametrosOfertasTipoIntegracao();
    KoBindings(_parametrosOfertasTipoIntegracao, "knockoutTipoIntegracao");

    buscarIntegracoes();

    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { ExcluirParametrosOfertasTipoIntegracaoClick(data); }, tamanho: "10", icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [excluir], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
    ];

    _gridParametrosOfertasTipoIntegracao = new BasicDataTable(_parametrosOfertasTipoIntegracao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridParametrosOfertasTipoIntegracao.CarregarGrid([]);
}

function ExcluirParametrosOfertasTipoIntegracaoClick(data) {
    let listaPOTI = _parametrosOfertas.TiposIntegracao.val().filter(
        poti => data.Codigo != poti
    );

    _parametrosOfertas.TiposIntegracao.val(listaPOTI);

    RecarregarGridParametrosOfertasTipoIntegracao(listaPOTI);
}

function AdicionarParametrosOfertasTipoIntegracaoClick() {
    const poTiposIntegracao = _parametrosOfertas.TiposIntegracao.val();
    const incoming = _parametrosOfertasTipoIntegracao.TipoIntegracao.val();

    if (incoming == 1) {
        LimparCamposParametrosOfertasTipoIntegracao();
        return;
    }

    if (poTiposIntegracao.some(poti => poti == incoming)) {
        LimparCamposParametrosOfertasTipoIntegracao();

        exibirMensagem(tipoMensagem.aviso, "Aviso", "Tipo de Integração já adicionado.")
        return;
    }

    poTiposIntegracao.push(incoming);

    RecarregarGridParametrosOfertasTipoIntegracao(poTiposIntegracao);
    LimparCamposParametrosOfertasTipoIntegracao();
}

function RecarregarGridParametrosOfertasTipoIntegracao(poTiposIntegracao) {
    const objListPOTI = [];

    poTiposIntegracao.forEach(poti => {
        objListPOTI.push({
            Codigo: poti,
            Descricao: EnumTipoIntegracao.obterOpcoes().find(ti => ti.value == poti).text
        })
    });

    _gridParametrosOfertasTipoIntegracao.CarregarGrid(objListPOTI);
}

function PreencherTiposIntegracao(listaTiposIntegracaoRetornadas) {
    _parametrosOfertas.TiposIntegracao.val(listaTiposIntegracaoRetornadas);
    RecarregarGridParametrosOfertasTipoIntegracao(listaTiposIntegracaoRetornadas);
}

function LimparCamposParametrosOfertasTipoIntegracao(comGrid) {
    LimparCampos(_parametrosOfertasTipoIntegracao);

    if (comGrid) {
        _gridParametrosOfertasTipoIntegracao.CarregarGrid(new Array());
    }
}