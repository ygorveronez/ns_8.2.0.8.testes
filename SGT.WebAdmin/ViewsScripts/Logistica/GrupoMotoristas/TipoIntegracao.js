/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="./GrupoMotoristas.js" />

var _grupoMotoristasTipoIntegracao;
var _gridGrupoMotoristasTipoIntegracao;

var GrupoMotoristasTipoIntegracao = function () {
    this.TipoIntegracao = PropertyEntity({ type: types.map, val: ko.observable(EnumTipoIntegracao.NaoInformada), options: ko.observable([]), text: "Sistema de Integração", def: EnumTipoIntegracao.NaoInformada, required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarGrupoMotoristasTipoIntegracaoClick, type: types.event, text: "Adicionar Tipo de Integração", visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local, id: guid() });
}

function LoadGrupoMotoristasTipoIntegracao() {
    _grupoMotoristasTipoIntegracao = new GrupoMotoristasTipoIntegracao();
    KoBindings(_grupoMotoristasTipoIntegracao, "knockoutTipoIntegracao");

    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { ExcluirGrupoMotoristasTipoIntegracaoClick(data); }, tamanho: "10", icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [excluir], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
    ];

    _gridGrupoMotoristasTipoIntegracao = new BasicDataTable(_grupoMotoristasTipoIntegracao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridGrupoMotoristasTipoIntegracao.CarregarGrid([]);

    buscarIntegracoes();
}

function ExcluirGrupoMotoristasTipoIntegracaoClick(data) {
    let listaPOTI = _grupoMotoristas.TiposIntegracao.val().filter(
        poti => data.Codigo != poti
    );

    _grupoMotoristas.TiposIntegracao.val(listaPOTI);

    RecarregarGridGrupoMotoristasTipoIntegracao(listaPOTI);
}

function AdicionarGrupoMotoristasTipoIntegracaoClick() {
    const poTiposIntegracao = _grupoMotoristas.TiposIntegracao.val();
    const incoming = _grupoMotoristasTipoIntegracao.TipoIntegracao.val();

    if (incoming == 1) {
        LimparCamposGrupoMotoristasTipoIntegracao();
        return;
    }

    if (poTiposIntegracao.some(poti => poti == incoming)) {
        LimparCamposGrupoMotoristasTipoIntegracao();

        exibirMensagem(tipoMensagem.aviso, "Aviso", "Tipo de Integração já adicionado.")
        return;
    }

    poTiposIntegracao.push(incoming);

    RecarregarGridGrupoMotoristasTipoIntegracao(poTiposIntegracao);
    LimparCamposGrupoMotoristasTipoIntegracao();
}

function RecarregarGridGrupoMotoristasTipoIntegracao(poTiposIntegracao) {
    const objListPOTI = [];

    poTiposIntegracao.forEach(poti => {
        objListPOTI.push({
            Codigo: poti,
            Descricao: _grupoMotoristasTipoIntegracao.TipoIntegracao.options().find(ti => ti.value == poti).text
        })
    });

    _gridGrupoMotoristasTipoIntegracao.CarregarGrid(objListPOTI);
}

function PreencherTiposIntegracao(listaTiposIntegracaoRetornadas) {
    _grupoMotoristas.TiposIntegracao.val(listaTiposIntegracaoRetornadas);
    RecarregarGridGrupoMotoristasTipoIntegracao(listaTiposIntegracaoRetornadas);
}

