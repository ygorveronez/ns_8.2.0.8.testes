//#region Variaveis
var _causa;
var _gridCausas;
//#endregion


//#region Constructores
function Causa() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), def: "", maxlentgh: 100, required: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.GridCausas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, id: guid(), idTab: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarCausa, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Alterar = PropertyEntity({ eventClick: alterarCausa, type: types.event, text: ko.observable("Alterar"), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarEdicao, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

}
//#endregion

//#region Funções de carregamento

function loadCausasParaFinalizacaoArvoreDecisao() {
    if (_CONFIGURACAO_TMS.HabilitarArvoreDecisaoEscalationList)
        $("#liTabCausasParaFinalizacaoArvoreDecisao").show();

    _causa = new Causa();
    KoBindings(_causa, "knockoutCausasParaFinalizacaoArvoreDecisao");
    gridCausas();
}

function gridCausas() {

    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarCausaClick, tamanho: "15", icone: "" };

    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" },
        { data: "Ativo", title: "Ativo", width: "75%", className: "text-align-left" }
    ];

    _gridCausas = new BasicDataTable(_causa.GridCausas.id, header, menuOpcoes);
    _gridCausas.CarregarGrid([]);
}

//#endregion

function adicionarCausa(e) {
    if (!ValidarCamposObrigatorios(_causa))
        return exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    const causa = e;
    const novaCausa = new Object();
    const listaGrid = ObterRegistrosGridCausas();

    novaCausa.Codigo = causa.Codigo.val();
    novaCausa.Descricao = causa.Descricao.val();
    novaCausa.Ativo = causa.Ativo.val() == true ? "Sim" : "Não";

    listaGrid.push(novaCausa);
    _gridCausas.CarregarGrid(listaGrid);
    limparCamposCausa();
}

function alterarCausa(e) {
    if (!ValidarCamposObrigatorios(_causa))
        return exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);

    const causa = e;
    const novaCausa = new Object();
    const listaGrid = ObterRegistrosGridCausas();

    $.each(listaGrid, function (i, item) {
        if (item.Codigo == _causa.Codigo.val()) {
            item.Descricao = _causa.Descricao.val();
            item.Ativo = _causa.Ativo.val() == true ? "Sim" : "Não";
        }
    });

    _gridCausas.CarregarGrid(listaGrid);
    limparCamposCausa();
}

function editarCausaClick(item) {
    _causa.Descricao.val(item.Descricao);
    _causa.Codigo.val(item.Codigo);
    _causa.Ativo.val(item.Ativo == "Sim" ? true : false);
    _causa.Cancelar.visible(true);
    _causa.Alterar.visible(true);
    _causa.Adicionar.visible(false);
}

function ObterRegistrosGridCausas() {
    return _gridCausas.BuscarRegistros();
}

function SetaGridCausas(data) {
    _gridCausas.CarregarGrid(data);
}

function limparCamposCausa() {
    _causa.Descricao.val("");
}

function SetarGridCausas() {
    const listaCausas = _motivoChamado.Causas.val();
    _gridCausas.CarregarGrid(listaCausas);
}

function ObterCausasSalvar() {
    _motivoChamado.Causas.val(JSON.stringify(ObterRegistrosGridCausas()));
}
function cancelarEdicao() {
    _causa.Descricao.val("");
    _causa.Ativo.val(true);
    _causa.Adicionar.visible(true);
    _causa.Cancelar.visible(false);
    _causa.Alterar.visible(false);
}