/// <reference path="../../Consultas/SetorFuncionario.js" />

//#region Variaveis
var _gatilhosTempoEscaltionList;
var _gridGatilhosTempoEscaltionList;
//#endregion


//#region Constructores
function GatilhosTempoEscaltionList() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.NivelEscalationList = PropertyEntity({ val: ko.observable(EnumEscalationList.Nenhum), options: EnumEscalationList.obterOpcoesCadastro(), text: Localization.Resources.Chamado.MotivoChamado.NivelEscaltion, def: EnumEscalationList.Nenhum, enable: ko.observable(true) });
    this.Tempo = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.TempoMin, getType: typesKnockout.int });
    this.Setor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Chamado.MotivoChamado.Setor.getFieldDescription(), idBtnSearch: guid() });
    this.PossibilitarInclusaoAnexoAoEscalarAtendimento = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.PossibilitarInclusaoAnexoAoEscalarAtendimento, getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.ConsiderarHorasDiasUteis = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.ConsiderarHorasDiasUteis, getType: typesKnockout.bool, val: ko.observable(false), def: ko.observable(false), visible: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarNivel, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarNivel, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarNivel, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}
//#endregion

//#region Funções de carregamento

function LoadGatilhosTempoEscaltionList() {
    if (_CONFIGURACAO_TMS.HabilitarArvoreDecisaoEscalationList)
        $("#liTabGatilhosTempoList").show();

    _gatilhosTempoEscaltionList = new GatilhosTempoEscaltionList();
    KoBindings(_gatilhosTempoEscaltionList, "knockoutGatilhosTempoList");

    new BuscarSetorFuncionario(_gatilhosTempoEscaltionList.Setor);

    LoadGridGatilhosTempoEscaltionList();
    RecarregarGridGatilhosTempoEscaltionList();
}

function existeUsuarioNoSetor(codigo) {

    return new Promise((resolve, reject) => {

        if (codigo && codigo > 0) {
            executarReST("MotivoChamado/ValidarExisteUsuarioSetor", { Codigo: codigo }, function (retorno) {
                if (!retorno.Success) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.MotivoChamado.Falha, retorno.Msg);
                    return resolve(false);
                }
                return resolve(true);
            });
        } else {
            resolve(false);
        }
    });
}

function LoadGridGatilhosTempoEscaltionList() {

    var excluir = { descricao: Localization.Resources.Chamado.MotivoChamado.Remover, id: guid(), evento: "onclick", metodo: RemoverNivelGrid, tamanho: "15", icone: "" };
    var editar = { descricao: Localization.Resources.Chamado.MotivoChamado.Editar, id: guid(), evento: "onclick", metodo: EditarNivel, tamanho: "15", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoSetor", visible: false },
        { data: "Nivel", title: Localization.Resources.Chamado.MotivoChamado.Nivel, width: "20%" },
        { data: "Tempo", title: Localization.Resources.Chamado.MotivoChamado.TempoMin, width: "10%" },
        { data: "DescricaoSetor", title: Localization.Resources.Chamado.MotivoChamado.Setor, width: "20%" }
    ];

    _gridGatilhosTempoEscaltionList = new BasicDataTable(_gatilhosTempoEscaltionList.Grid.idGrid, header, menuOpcoes);
}

//#endregion


///#region Funções Auxiliares

function RecarregarGridGatilhosTempoEscaltionList() {
    const data = new Array();

    $.each(_gridGatilhosTempoEscaltionList.BuscarRegistros, (_, nivel) => {
        data.push(nivel);
    });
    _gridGatilhosTempoEscaltionList.CarregarGrid(data);
}

async function AdicionarNivel(e) {
    const nivel = e;
    const novoNivel = new Object();
    const listaGrid = ObterRegistrosGrid();

    novoNivel.Codigo = nivel.Codigo.val();
    novoNivel.Nivel = nivel.NivelEscalationList.val();
    novoNivel.Tempo = nivel.Tempo.val();
    novoNivel.DescricaoSetor = nivel.Setor.val();
    novoNivel.CodigoSetor = nivel.Setor.codEntity();
    const usuarioExiste = await existeUsuarioNoSetor(novoNivel.CodigoSetor);
    if (!usuarioExiste) {
        LimparCampo(_gatilhosTempoEscaltionList.Setor);
        return;
    }

    const [existeNivelCadastrado] = listaGrid.filter(itemNivel => itemNivel.Nivel == novoNivel.Nivel);
    if (existeNivelCadastrado)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Chamado.MotivoChamado.Aviso, Localization.Resources.Chamado.MotivoChamado.JaExisteCadastrado);

    listaGrid.push(novoNivel);
    _gridGatilhosTempoEscaltionList.CarregarGrid(listaGrid);
    LimparCamposGatilhos();
}

function EditarNivel(itemNivel) {
    LimparCamposGatilhos();

    _gatilhosTempoEscaltionList.Codigo.val(itemNivel.Codigo);
    _gatilhosTempoEscaltionList.Tempo.val(itemNivel.Tempo);
    _gatilhosTempoEscaltionList.NivelEscalationList.val(itemNivel.Nivel);
    _gatilhosTempoEscaltionList.Setor.val(itemNivel.DescricaoSetor);
    _gatilhosTempoEscaltionList.Setor.codEntity(itemNivel.CodigoSetor);

    ControleBotoesCrud(false);
}

function RemoverNivelGrid(item) {
    const listaGrid = ObterRegistrosGrid();

    $.each(listaGrid, (i, itemNivel) => {
        if (itemNivel.Nivel == item.Nivel) {
            listaGrid.splice(i, 1);
            return false;
        }
    });

    _gridGatilhosTempoEscaltionList.CarregarGrid(listaGrid);
    LimparCamposGatilhos();
}

async function AtualizarNivel(dadosnivel) {
    const nivel = dadosnivel;
    const listaGrid = ObterRegistrosGrid();

    for (var i = 0; i < listaGrid.length; i++) {
        const itemNivel = listaGrid[i];

        if (itemNivel.Nivel != nivel.NivelEscalationList.val())
            continue;

        itemNivel.Codigo = nivel.Codigo.val();
        itemNivel.Tempo = nivel.Tempo.val();
        itemNivel.Nivel = nivel.NivelEscalationList.val();
        itemNivel.DescricaoSetor = nivel.Setor.val();
        itemNivel.CodigoSetor = nivel.Setor.codEntity();
        const usuarioExiste = await existeUsuarioNoSetor(itemNivel.CodigoSetor);
        if (!usuarioExiste) {
            LimparCampo(_gatilhosTempoEscaltionList.Setor);
            return;
        }
        break;
    }
    _gridGatilhosTempoEscaltionList.CarregarGrid(listaGrid);
    ControleBotoesCrud(true);
    LimparCamposGatilhos();
}

function CancelarNivel() {
    LimparCamposGatilhos();
    ControleBotoesCrud(true);
}

function LimparCamposGatilhos() {
    LimparCampo(_gatilhosTempoEscaltionList.Tempo);
    LimparCampo(_gatilhosTempoEscaltionList.Codigo);
    LimparCampo(_gatilhosTempoEscaltionList.NivelEscalationList);
    LimparCampo(_gatilhosTempoEscaltionList.Setor);
}

function ObterRegistrosGrid() {
    return _gridGatilhosTempoEscaltionList.BuscarRegistros();
}

function ControleBotoesCrud(visible) {
    _gatilhosTempoEscaltionList.Adicionar.visible(visible);
    _gatilhosTempoEscaltionList.Atualizar.visible(!visible);
    _gatilhosTempoEscaltionList.Cancelar.visible(!visible);
}

function SetaGridGatilhos() {
    const listGatilhos = _motivoChamado.GatilhosTempoList.val();
    _gridGatilhosTempoEscaltionList.CarregarGrid(listGatilhos);
    _gatilhosTempoEscaltionList.PossibilitarInclusaoAnexoAoEscalarAtendimento.val(_motivoChamado.PossibilitarInclusaoAnexoAoEscalarAtendimento.val());
    _gatilhosTempoEscaltionList.ConsiderarHorasDiasUteis.val(_motivoChamado.ConsiderarHorasDiasUteis.val());
}
function ObterListaGatilhosGrid() {
    _motivoChamado.GatilhosTempoList.val(JSON.stringify(ObterRegistrosGrid()));
}

//#endregion