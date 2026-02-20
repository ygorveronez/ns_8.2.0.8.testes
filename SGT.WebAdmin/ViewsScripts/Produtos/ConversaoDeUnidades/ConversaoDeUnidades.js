//#region Variaveis Globais
var _gridConversaoDeUnidades;
var _cadastroUnidade;
//#endregion

//#region Funções Construtoras
function CadastroUnidade() {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.UnidadePara = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Unidade (Para)", idBtnSearch: guid() });
    this.SiglaPara = PropertyEntity({ val: ko.observable(""), enable: false, text: "Sigla (Para)" });

    this.SiglaDe = PropertyEntity({ val: ko.observable(""), text: "Sigla (de)" });
    this.DescricaoDe = PropertyEntity({ val: ko.observable(""), text: "Descrição (de)" });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarConversoes, type: types.event, text: "Adicionar" });
    this.Salvar = PropertyEntity({ eventClick: SalvarConversoes, type: types.event, text: "Salvar Conversões" });
}

function LoadConversaoDeUnidades() {
    _cadastroUnidade = new CadastroUnidade();
    KoBindings(_cadastroUnidade, "knoutConversaoDeUnidades");

    new BuscarUnidadesMedida(_cadastroUnidade.UnidadePara, retornoBuscaUnidadeMedida);

    LoagGridConversaoDeUnidades();
    ObterConversoesExistentes();
}

function LoagGridConversaoDeUnidades() {
    const headerGrid = [
        { data: "Codigo", visible: false },
        { data: "CodigoUnidadePara", visible: false },
        { data: "SiglaDe", title: "Sigla (de)" },
        { data: "DescricaoDe", title: "Descrição (de)" },
        { data: "SiglaPara", title: "Sigla (para)" },
        { data: "DescricaoPara", title: "Descrição (para)" },
    ]

    let excluirConversao = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: RemoverConversaoGrid, tamanho: "15", icone: ""
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [excluirConversao]
    };

    _gridConversaoDeUnidades = new BasicDataTable(_cadastroUnidade.Grid.idGrid, headerGrid, menuOpcoes);
    _gridConversaoDeUnidades.CarregarGrid([]);
}
//#endregion

//#region Funções auxiliares

function RemoverConversaoGrid(e) {
    const registros = ObterRegistrosGrid();

    for (var i = 0; i < registros.length; i++) {
        if (!(registros[i].CodigoUnidadePara == e.CodigoUnidadePara && registros[i].SiglaDe == e.SiglaDe && registros[i].DescricaoDe == e.DescricaoDe))
            continue;

        registros.splice(i, 1);
        break;
    }
    _gridConversaoDeUnidades.CarregarGrid(registros);
}

function retornoBuscaUnidadeMedida(selecionado) {
    _cadastroUnidade.UnidadePara.val(selecionado.Descricao);
    _cadastroUnidade.UnidadePara.codEntity(selecionado.Codigo);
    _cadastroUnidade.SiglaPara.val(selecionado.Sigla);
}

function AdicionarConversoes() {
    const listaConversoesGrid = ObterRegistrosGrid();

    if (ExisteConversaoIgualCadastrada())
        return exibirMensagem(tipoMensagem.atencao, "Aviso", "Tipo de conversão já cadastrada");

    const novaConversao = {
        Codigo: 0,
        CodigoUnidadePara: _cadastroUnidade.UnidadePara.codEntity(),
        SiglaDe: _cadastroUnidade.SiglaDe.val(),
        DescricaoDe: _cadastroUnidade.DescricaoDe.val(),
        SiglaPara: _cadastroUnidade.SiglaPara.val(),
        DescricaoPara: _cadastroUnidade.UnidadePara.val()
    }
    listaConversoesGrid.push(novaConversao);

    _gridConversaoDeUnidades.CarregarGrid(listaConversoesGrid);
    LimparCamposCadastro();
}

function ObterRegistrosGrid() {
    return _gridConversaoDeUnidades.BuscarRegistros();
}

function LimparCamposCadastro() {
    LimparCampo(_cadastroUnidade.SiglaPara);
    LimparCampo(_cadastroUnidade.UnidadePara);
    LimparCampo(_cadastroUnidade.SiglaDe);
    LimparCampo(_cadastroUnidade.DescricaoDe);
}

function ExisteConversaoIgualCadastrada() {
    const listaConversoesGrid = ObterRegistrosGrid();

    const [exiteConversao] = listaConversoesGrid.filter(coversao => coversao.CodigoUnidadeDe == _cadastroUnidade.UnidadePara.codEntity()
        && coversao.SiglaDe == _cadastroUnidade.SiglaDe.val()
        && coversao.DescricaoDe == _cadastroUnidade.DescricaoDe.val());

    if (exiteConversao)
        return true;

    return false;
}

function ObterDadosGridParaSalvar() {
    const listaItemSalvar = new Array();

    $.each(ObterRegistrosGrid(), (_, item) => {
        let novoItem = {
            Codigo: item.Codigo,
            CodigoUnidadePara: item.CodigoUnidadePara,
            SiglaDe: item.SiglaDe,
            DescricaoDe: item.DescricaoDe
        }
        listaItemSalvar.push(novoItem);
    });

    return JSON.stringify(listaItemSalvar);
}

function SalvarConversoes() {
    const Conversoes = ObterDadosGridParaSalvar();
    executarReST("ConversaoDeUnidades/Adicionar", { Conversoes }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Conversões Cadastradas com sucesso");
        ObterConversoesExistentes();
    });
}

function ObterConversoesExistentes() {
    executarReST("ConversaoDeUnidades/ObterConversoes", null, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        _gridConversaoDeUnidades.CarregarGrid(arg.Data);
    });
}
//#endregion