//#region Variaveis
var _criticidadeAtendimento;
var _criticidadeAtendimentoModal;
var _gridCriticidade;
//#endregion

//#region Constructores
var CriticidadeAtendimento = function () {
    this.GridCriticidade = PropertyEntity({ id: guid() });
};

var CriticidadeAtendimentoModal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({
        val: ko.observable(EnumTipoParametroCriticidade.Gerencial),
        options: EnumTipoParametroCriticidade.obterOpcoes(),
        def: EnumTipoParametroCriticidade.Gerencial,
        text: "Tipo",
        required: true,
        visible: ko.observable(true)
    });
    this.Conteudo = PropertyEntity({
        text: "Conteúdo",
        required: true,
        maxlength: 200,
        getType: typesKnockout.string,
        val: ko.observable(""),
        visible: ko.observable(true)
    });
    this.Ativo = PropertyEntity({
        val: ko.observable(true),
        options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"),
        def: true,
        text: "Ativo?",
        required: true,
        visible: ko.observable(true)
    });
    this.TituloModal = ko.observable("Adicionar Parâmetro");
    this.SubtituloModal = ko.observable("Adicione um Parâmetro para Criticidade do Atendimento");
    this.ModoEdicao = ko.observable(false);

    this.Confirmar = PropertyEntity({
        eventClick: confirmarClick,
        type: types.event,
        text: "Confirmar",
        visible: ko.observable(true)
    });
};
//#endregion

//#region Funções de carregamento
function loadCriticidadeAtendimento() {
    _criticidadeAtendimento = new CriticidadeAtendimento();
    _criticidadeAtendimentoModal = new CriticidadeAtendimentoModal();

    KoBindings(_criticidadeAtendimento, "knockoutCriticidadeAtendimento");

    carregarGridCriticidade();

    KoBindings(_criticidadeAtendimentoModal, "knockoutCriticidadeAtendimentoModal");
}

function carregarGridCriticidade() {
    const editar = {
        descricao: "Editar",
        id: guid(),
        metodo: editarParametroClick,
        icone: ""
    };
    const excluir = {
        descricao: "Excluir",
        id: guid(),
        metodo: excluirParametroClick,
        icone: ""
    };

    const menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar, excluir]
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: "Tipo", width: "20%", className: "text-align-left" },
        { data: "Conteudo", title: "Conteúdo", width: "60%", className: "text-align-left" },
        { data: "Ativo", visible: false },
        { data: "AtivoDescricao", title: "Ativo", width: "20%", className: "text-align-center" }
    ];

    _gridCriticidade = new BasicDataTable(_criticidadeAtendimento.GridCriticidade.id, header, menuOpcoes);
    _gridCriticidade.CarregarGrid([]);
}
//#endregion

//#region Eventos
function adicionarParametroClick() {
    _criticidadeAtendimentoModal.ModoEdicao(false);
    _criticidadeAtendimentoModal.TituloModal("Adicionar Parâmetro");
    _criticidadeAtendimentoModal.SubtituloModal("Adicione um Parâmetro para Criticidade do Atendimento");
    limparCamposCriticidade();
    Global.abrirModal("modalAdicionarParametroCriticidade");
}

function confirmarClick(e, sender) {
    if (!ValidarCamposCriticidade()) {
        return;
    }

    const novoParametro = {
        Codigo: _criticidadeAtendimentoModal.Codigo.val(),
        Tipo: _criticidadeAtendimentoModal.Tipo.val(),
        TipoDescricao: EnumTipoParametroCriticidade.obterDescricao(_criticidadeAtendimentoModal.Tipo.val()),
        Conteudo: _criticidadeAtendimentoModal.Conteudo.val(),
        Ativo: _criticidadeAtendimentoModal.Ativo.val(),
        AtivoDescricao: _criticidadeAtendimentoModal.Ativo.val() ? "Sim" : "Não"
    };

    const listaGrid = ObterRegistrosGridCriticidade();

    if (_criticidadeAtendimentoModal.ModoEdicao()) {
        const index = listaGrid.findIndex(item => item.Codigo === novoParametro.Codigo);
        if (index !== -1) {
            listaGrid[index] = novoParametro;
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Parâmetro atualizado com sucesso");
        }
    } else {
        novoParametro.Codigo = -(listaGrid.length + 1);
        listaGrid.push(novoParametro);
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Parâmetro adicionado com sucesso");
    }

    _gridCriticidade.CarregarGrid(listaGrid);
    Global.fecharModal("modalAdicionarParametroCriticidade");
    limparCamposCriticidade();
}

function editarParametroClick(itemGrid) {
    _criticidadeAtendimentoModal.Codigo.val(itemGrid.Codigo);
    _criticidadeAtendimentoModal.Tipo.val(itemGrid.Tipo);
    _criticidadeAtendimentoModal.Conteudo.val(itemGrid.Conteudo);
    _criticidadeAtendimentoModal.Ativo.val(itemGrid.Ativo);

    _criticidadeAtendimentoModal.ModoEdicao(true);
    _criticidadeAtendimentoModal.TituloModal("Editar Parâmetro");
    _criticidadeAtendimentoModal.SubtituloModal("Edite o Parâmetro de Criticidade do Atendimento");

    Global.abrirModal("modalAdicionarParametroCriticidade");
}

function excluirParametroClick(itemGrid) {
    exibirConfirmacao(
        "Confirmação",
        "Realmente deseja excluir este parâmetro?",
        function () {
            const listaGrid = ObterRegistrosGridCriticidade();
            const index = listaGrid.findIndex(item => item.Codigo === itemGrid.Codigo);

            if (index !== -1) {
                listaGrid.splice(index, 1);
                _gridCriticidade.CarregarGrid(listaGrid);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Parâmetro excluído com sucesso.");
            }
        }
    );
}
//#endregion

//#region Métodos auxiliares
function limparCamposCriticidade() {
    if (!_criticidadeAtendimentoModal) return;

    _criticidadeAtendimentoModal.Codigo.val(0);
    _criticidadeAtendimentoModal.Tipo.val(EnumTipoParametroCriticidade.Gerencial);
    _criticidadeAtendimentoModal.Conteudo.val("");
    _criticidadeAtendimentoModal.Ativo.val(true);
    _criticidadeAtendimentoModal.ModoEdicao(false);
}

function limparGridCriticidade() {
    if (_gridCriticidade) {
        _gridCriticidade.CarregarGrid([]);
    }
}

function ValidarCamposCriticidade() {
    if (!_criticidadeAtendimentoModal.Conteudo.val() || _criticidadeAtendimentoModal.Conteudo.val().trim() === "") {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O campo Conteúdo é obrigatório.");
        return false;
    }
    return true;
}

function ObterRegistrosGridCriticidade() {
    return _gridCriticidade.BuscarRegistros();
}

function SetarGridCriticidade(data) {
    if (!_gridCriticidade) {
        console.error("Grid de criticidade não foi inicializada ainda");
        return;
    }

    if (!data || data.length === 0) {
        _gridCriticidade.CarregarGrid([]);
        return;
    }

    const dadosMapeados = data.map(item => ({
        Codigo: item.Codigo,
        Tipo: item.Tipo,
        TipoDescricao: EnumTipoParametroCriticidade.obterDescricao(item.Tipo),
        Conteudo: item.Conteudo,
        Ativo: item.Ativo,
        AtivoDescricao: item.Ativo ? "Sim" : "Não"
    }));

    _gridCriticidade.CarregarGrid(dadosMapeados);
}

function ObterTiposCriticidadeSalvar() {
    const lista = ObterRegistrosGridCriticidade();

    const listaMapeada = lista.map(item => ({
        Codigo: item.Codigo > 0 ? item.Codigo : 0,
        Tipo: item.Tipo,
        Conteudo: item.Conteudo,
        Ativo: item.Ativo
    }));

    _motivoChamado.TiposCriticidade.val(JSON.stringify(listaMapeada));
}

function RecarregarGridCriticidadeAposAtualizar() {
    const codigoMotivoChamado = _motivoChamado.Codigo.val();

    if (codigoMotivoChamado > 0) {
        executarReST("MotivoChamado/BuscarPorCodigo", { Codigo: codigoMotivoChamado }, function (retorno) {
            if (retorno.Success && retorno.Data && retorno.Data.TiposCriticidade) {
                SetarGridCriticidade(retorno.Data.TiposCriticidade);
            }
        });
    }
}
//#endregion