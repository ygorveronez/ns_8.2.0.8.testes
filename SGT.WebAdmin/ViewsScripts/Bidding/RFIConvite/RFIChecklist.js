/// <reference path="../../Enumeradores/EnumRequisitoRFI.js" />
/// <reference path="../../Enumeradores/EnumTipoOpcaoCheckListRFI.js" />
/// <reference path="Opcoes.js" />

var _gridChecklistQuestionarios, _checklist, _questionario, _gridChecklistAnexo, _anexosChecklist, _anexoChecklist, questionarioSelecionado, _checkListOpcoesRFI;
var _checklistAddQuestionario, _gridQuestionario;
var questionario = new Array();

var _requisitoOptions = [
    { text: "Desejável", value: 0 },
    { text: "Indispensável", value: 1 }
];

//Funções inicialização
var Checklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.PrazoChecklist = PropertyEntity({ text: "*Prazo Preenchimento Checklist:", required: true, getType: typesKnockout.dateTime, enable: ko.observable(true), val: ko.observable(""), visible: ko.observable(true) });
}

var Questionario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.Pergunta = PropertyEntity({ text: "*Pergunta:", required: true, getType: typesKnockout.string, def: "" });
    this.Requisito = PropertyEntity({ text: "*Requisito:", required: true, options: EnumRequisitoRFI.obterOpcoes(), val: ko.observable(EnumRequisitoRFI.Desejavel), def: EnumRequisitoRFI.Desejavel });
    this.TipoOpcaoCheckListRFI = PropertyEntity({ text: "Tipo de Pergunta: ", val: ko.observable(EnumTipoOpcaoCheckListRFI.SimNao), options: EnumTipoOpcaoCheckListRFI.obterOpcoes(), def: EnumTipoOpcaoCheckListRFI.SimNao, visible: ko.observable(true) });

    this.TipoOpcaoCheckListRFI.val.subscribe(onTipoCheckListChangeRFI);
}

var CheckListOpcoesRFI = function () {

    this.CodigoOpcao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.OrdemOpcao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoOpcao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, maxlength: 1000, eventKeydown: AdicionarOpcaoEnterRFI });
    this.ValorOpcao = PropertyEntity({ text: "Valor:", getType: typesKnockout.int, visible: ko.observable(false), eventKeydown: AdicionarOpcaoEnterRFI });
    this.Opcoes = PropertyEntity({ idGrid: guid(), val: ko.observable(""), list: [], visible: ko.observable(false) });

    this.AdicionarOpcao = PropertyEntity({ eventClick: AdicionarOpcaoRFI, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.CancelarOpcao = PropertyEntity({ eventClick: CancelarOpcaoRFI, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.AtualizarOpcao = PropertyEntity({ eventClick: AtualizarOpcaoRFI, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.ExcluirOpcao = PropertyEntity({ eventClick: ExcluirOpcaoRFI, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var AnexosChecklist = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexo();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoModalChecklistClick, type: types.event, text: "Adicionar Anexos", visible: ko.observable(true) });
}

var AnexoChecklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoChecklist.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoChecklistClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var AddQuestionario = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarQuestionarioClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarQuestionarioClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirQuestionarioClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var GridQuestionario = function () {
    this.Questionarios = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ eventClick: adicionarQuestionarioModalClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

function LoadChecklistRFI() {
    _checklist = new Checklist();
    KoBindings(_checklist, "knockoutChecklist");

    _questionario = new Questionario();
    KoBindings(_questionario, "knockoutQuestionario");

    _anexosChecklist = new AnexosChecklist();
    KoBindings(_anexosChecklist, "knockoutChecklistAnexos")

    _anexoChecklist = new AnexoChecklist();
    KoBindings(_anexoChecklist, "knockoutChecklistAnexo");

    _checkListOpcoesRFI = new CheckListOpcoesRFI();
    KoBindings(_checkListOpcoesRFI, "knockoutChecklistOpcoes");

    _checklistAddQuestionario = new AddQuestionario();
    KoBindings(_checklistAddQuestionario, "knockoutAddQuestionario");

    _gridQuestionario = new GridQuestionario();
    KoBindings(_gridQuestionario, "knockoutGridQuestionario");

    LoadGridChecklistAnexo();
    LoadGridQuestionarios();
    GridOpcoesRFI();
}

function LoadGridQuestionarios() {
    const linhasPorPagina = 5;
    const opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarQuestionarioClick, icone: "", visiblidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "ChecklistAnexo", visible: false },
        { data: "Descricao", title: "Pergunta", width: "40%", className: "text-align-left" },
        { data: "Requisito", title: "Requisito", width: "30%", className: "text-align-left" },
        { data: "TipoOpcaoCheckListRFI", title: "Tipo de Pergunta", width: "40%", className: "text-align-left" },
        { data: "GridMultiplaEscolha", visible: false },
    ];
    _gridChecklistQuestionarios = new BasicDataTable(_gridQuestionario.Questionarios.id, header, menuOpcoes, null, null, linhasPorPagina);
    _gridChecklistQuestionarios.CarregarGrid([]);
}

function LoadGridChecklistAnexo() {
    const linhasPorPaginas = 2;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadChecklistAnexoClick, icone: "", visibilidade: true };
    const opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerChecklistAnexoClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridChecklistAnexo = new BasicDataTable(_anexosChecklist.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridChecklistAnexo.CarregarGrid([]);
}

//Funções click
function downloadChecklistAnexoClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("RFIChecklistAnexo/DownloadAnexo", dados);
}

function removerChecklistAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerChecklistAnexoLocal(registroSelecionado);
    else {
        executarReST("RFIChecklistAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerChecklistAnexoLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function adicionarAnexoChecklistClick() {
    const arquivo = document.getElementById(_anexoChecklist.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    const anexo = {
        Codigo: guid(),
        Descricao: _anexoChecklist.Descricao.val(),
        NomeArquivo: _anexoChecklist.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    if (questionarioSelecionado != undefined && !isNaN(questionarioSelecionado.Codigo))
        enviarChecklistAnexos(questionarioSelecionado.Codigo, [anexo]);

    const listaAnexos = obterChecklistAnexos();

    listaAnexos.push(anexo);

    _anexosChecklist.Anexos.val(listaAnexos.slice());


    _anexoChecklist.Arquivo.val("");

    preencherGridChecklistAnexo();
    Global.fecharModal('divModalChecklistAnexo');
}

function adicionarAnexoModalChecklistClick() {
    Global.abrirModal('divModalChecklistAnexo');
    $("#divModalChecklistAnexo").one('hidden.bs.modal', function () {
        LimparCampos(_anexoChecklist);
    });
}

function adicionarQuestionarioClick() {
    if (!ValidarCamposObrigatorios(_questionario)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    questionario.unshift({
        Codigo: _questionario.Codigo.val(),
        Descricao: _questionario.Pergunta.val(),
        Requisito: EnumRequisitoRFI.obterDescricao(_questionario.Requisito.val()),
        ChecklistAnexo: _anexosChecklist.Anexos.val(),
        TipoOpcaoCheckListRFI: EnumTipoOpcaoCheckListRFI.obterDescricao(_questionario.TipoOpcaoCheckListRFI.val()),
        GridMultiplaEscolha: ObterListaOrdenada()
    });

    _anexosChecklist.Anexos.val([]);
    recarregarGridChecklist();
    Global.fecharModal('divModalAddQuestionario');
}

function adicionarQuestionarioModalClick() {
    _questionario.Codigo.val(guid());
    Global.abrirModal('divModalAddQuestionario');
    $("#divModalAddQuestionario").one('hidden.bs.modal', function () {
        LimparCampos(_questionario);
        LimparGridChecklistAnexos();
        LimparOpcoes();
    });
}

function editarQuestionarioClick(registroSelecionado) {
    questionarioSelecionado = registroSelecionado;

    _checklistAddQuestionario.Atualizar.visible(true);
    _checklistAddQuestionario.Excluir.visible(true);
    _checklistAddQuestionario.Adicionar.visible(false);
    preencherEditarQuestionario(registroSelecionado);

    validaTipoOpcaoQuestao();

    Global.abrirModal('divModalAddQuestionario');
    $("#divModalAddQuestionario").one('hidden.bs.modal', function () {
        registroSelecionado = questionarioSelecionado;
        questionarioSelecionado = undefined;
        _checklistAddQuestionario.Atualizar.visible(false);
        _checklistAddQuestionario.Excluir.visible(false);
        _checklistAddQuestionario.Adicionar.visible(true);
        LimparCampos(_questionario);
        LimparGridChecklistAnexos();
        recarregarGridChecklist();
    });
}

function atualizarQuestionarioClick() {
    questionarioSelecionado.Pergunta = _questionario.Pergunta.val();
    questionarioSelecionado.Requisito = EnumRequisitoRFI.obterDescricao(_questionario.Requisito.val());
    questionarioSelecionado.ChecklistAnexo = _anexosChecklist.Anexos.val();
    questionarioSelecionado.TipoOpcaoCheckListRFI = EnumTipoOpcaoCheckListRFI.obterDescricao(_questionario.TipoOpcaoCheckListRFI.val());
    questionarioSelecionado.GridMultiplaEscolha = ObterListaOrdenada();
    Global.fecharModal('divModalAddQuestionario');
}

function excluirQuestionarioClick() {
    let index = questionario.indexOf(questionarioSelecionado);
    questionario.splice(index, 1);
    if (!isNaN(questionarioSelecionado.Codigo))
        executarReST("RFIConvite/ExcluirQuestionario", { Codigo: questionarioSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Questionario removido com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        }, null);

    Global.fecharModal("divModalAddQuestionario");
}

function onTipoCheckListChangeRFI() {
    validaTipoOpcaoQuestao();
}

//Funçoes publicas
function enviarChecklistAnexos(codigo, anexos) {
    const formData = obterFormDataChecklistAnexos(anexos);

    if (!formData)
        return;

    enviarArquivo("RFIChecklistAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                return exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");

            return exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function preencherChecklist(dadosRFI) {
    dadosRFI["PrazoChecklist"] = JSON.stringify(RetornarObjetoPesquisa(_checklist));
    dadosRFI["Checklist"] = ObterListaChecklist();
}

function ObterListaChecklist() {
    const listaQuestionarios = ObterListaQuestionarios();
    const listaQuestionariosRFI = new Array();

    for (let i = 0; i < listaQuestionarios.length; i++) {
        const questao = listaQuestionarios[i];
        listaQuestionariosRFI.push({
            Codigo: questao.Codigo,
            Pergunta: questao.Descricao,
            Requisito: EnumRequisitoRFI.obterValor(questao.Requisito),
            Tipo: EnumTipoOpcaoCheckListRFI.obterValor(questao.TipoOpcaoCheckListRFI),
            Alternativas: questao.GridMultiplaEscolha
        });
    }

    return JSON.stringify(listaQuestionariosRFI);
}

function ObterListaQuestionarios() {
    return _gridChecklistQuestionarios.BuscarRegistros();
}

function enviarArquivosAnexadosQuestionario(codigosQuestionario) {
    for (let i = 0; i < codigosQuestionario.length; i++) {
        if (questionario[i].Codigo == codigosQuestionario[i].codigo) {
            if (questionario[i].ChecklistAnexo.length > 0) {
                enviarChecklistAnexos(codigosQuestionario[i].novoCodigo, questionario[i].ChecklistAnexo);
            }
        }
    }
}

//Funções privadas
function limparCamposChecklist() {
    LimparCampos(_checklist);
    questionario.splice(0, questionario.length);
    _gridChecklistQuestionarios.CarregarGrid(questionario);
}

function obterChecklistAnexos() {
    return _gridChecklistAnexo.BuscarRegistros();
}

function preencherGridChecklistAnexo() {
    const checklistAnexos = obterChecklistAnexos();
    _gridChecklistAnexo.CarregarGrid(checklistAnexos);
}

function removerChecklistAnexoLocal(registroSelecionado) {
    const listaAnexos = obterChecklistAnexos();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexosChecklist.Anexos.val(listaAnexos);
    preencherGridChecklistAnexo();
}

function obterFormDataChecklistAnexos(anexos) {
    if (anexos.length > 0) {
        let formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function LimparGridChecklistAnexos() {
    _gridChecklistAnexo.CarregarGrid([]);
}

function obterRequisitoVal(value) {
    for (let i = 0; i < _requisitoOptions.length; i++) {
        if (_requisitoOptions[i].text == value) {
            return _requisitoOptions[i].value;
        }
    }
}

function preencherEditarQuestionario(registroSelecionado) {
    _questionario.Codigo.val(registroSelecionado.Codigo);
    _questionario.Pergunta.val(registroSelecionado.Descricao);
    _questionario.Requisito.val(EnumRequisitoRFI.obterValor(registroSelecionado.Requisito));
    _questionario.TipoOpcaoCheckListRFI.val(EnumTipoOpcaoCheckListRFI.obterValor(registroSelecionado.TipoOpcaoCheckListRFI));
    SetaLista(registroSelecionado.GridMultiplaEscolha);
    RecarregarGridOpcoes();
    _gridChecklistAnexo.CarregarGrid(registroSelecionado.ChecklistAnexo);
}

function recarregarGridChecklist() {
    _gridChecklistQuestionarios.CarregarGrid(questionario);
}

function validaTipoOpcaoQuestao() {

    if (_questionario.TipoOpcaoCheckListRFI.val() === EnumTipoOpcaoCheckListRFI.Opcoes) {
        _checkListOpcoesRFI.Opcoes.visible(true);
        _anexosChecklist.Anexos.visible(false);
    }
    else {
        _checkListOpcoesRFI.Opcoes.visible(false);
        _anexosChecklist.Anexos.visible(true);
    }
}