

//#region Variaveis
var _mesoRegiao;
var _pesquisaMesoRegiao;
var _griPesquisaMesoRegiao;
var _crudMesoRegiao;
//#endregion

//#region Funções Contructoras

var PerquisaMesoRegiao = function () {

    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", val: ko.observable("") });
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _griPesquisaMesoRegiao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var MesoRegiao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });

    this.Descricao = PropertyEntity({ val: ko.observable(""), text: "Descrição" });
    this.CodigoIntegracao = PropertyEntity({ val: ko.observable(""), text: "Codigo Integração" });
    this.Situacao = PropertyEntity({ options: _status, val: ko.observable(true), def: true, text: "Codigo Integração" });
}

var CRUDMesoRegiao = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarMesoRegiao, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarMesoRegiao, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirMesoRegiao, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarMesoRegiao, type: types.event, text: "Limpar/Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "MesoRegiao/CadastrarMesoRegiaoImportacao",
        UrlConfiguracao: "MesoRegiao/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O055_ImportacaoMesoRegiao,
        CallbackImportacao: function () {

        },
        FecharModalSeSucesso: false
    });
}

//#endregion



//#region Função de Carregamento

function LoadMesoRegiao() {
    _mesoRegiao = new MesoRegiao();
    KoBindings(_mesoRegiao, "knockoutMesoRegiao");

    _pesquisaMesoRegiao = new PerquisaMesoRegiao();
    KoBindings(_pesquisaMesoRegiao, "knockoutPesquisaMesoRegiao", false, _pesquisaMesoRegiao.Pesquisar.id)

    _crudMesoRegiao = new CRUDMesoRegiao();
    KoBindings(_crudMesoRegiao, "knockoutCRUDMesoRegiao")

    BuscarMesoRegiao();
}
function BuscarMesoRegiao() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarMesoRegiao, tamanho: "15", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _griPesquisaMesoRegiao = new GridView(_pesquisaMesoRegiao.Pesquisar.idGrid, "MesoRegiao/Pesquisa", _pesquisaMesoRegiao, menuOpcoes, null);
    _griPesquisaMesoRegiao.CarregarGrid();
}

//#endregion

//#region Funções Auxiliares

function AdicionarMesoRegiao() {
    Salvar(_mesoRegiao, "MesoRegiao/Adicionar", (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        BuscarMesoRegiao();
        LimparCadastroMesoRegiao();
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro Adicionar com sucesso");
    });
}

function AtualizarMesoRegiao() {
    Salvar(_mesoRegiao, "MesoRegiao/Atualizar", (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        BuscarMesoRegiao();
        LimparCadastroMesoRegiao();
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro Atualizado com sucesso");

    });
}

function CancelarMesoRegiao() {
    LimparCadastroMesoRegiao();
}

function EditarMesoRegiao(item) {
    executarReST("MesoRegiao/BuscarPorCodigo", { Codigo: item.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        PreencherObjetoKnout(_mesoRegiao, { Data: arg.Data });
        ControlesButtonsCrud(true);
    });
}

function ExcluirMesoRegiao() {
    ExcluirPorCodigo(_mesoRegiao, "MesoRegiao/ExcluirPorCodigo", (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro Excluido com sucesso");
        LimparCadastroMesoRegiao();
        BuscarMesoRegiao();
    })
}

function LimparCadastroMesoRegiao() {
    ControlesButtonsCrud(false);
    LimparCampos(_mesoRegiao);
}
function ControlesButtonsCrud(Visible) {
    _crudMesoRegiao.Atualizar.visible(Visible);
    _crudMesoRegiao.Excluir.visible(Visible);
    _crudMesoRegiao.Cancelar.visible(Visible);
    _crudMesoRegiao.Adicionar.visible(!Visible);
}
//#endregion

