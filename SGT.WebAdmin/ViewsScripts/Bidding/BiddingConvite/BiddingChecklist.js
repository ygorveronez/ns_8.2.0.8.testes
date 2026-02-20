/// <reference path="../../Enumeradores/EnumRequisito.js" />
/// <reference path="../../Enumeradores/EnumTipoPreenchimentoChecklist.js" />

var _gridChecklistQuestionarios, _checklist, _questionario, _gridChecklistAnexo, _anexosChecklist, _anexoChecklist, questionarioSelecionado;
var _checklistAddQuestionario, _gridQuestionario;
var questionario = new Array();

var _requisitoOptions = [
    { text: "Desejável", value: 0 },
    { text: "Indispensável", value: 1 }
];

//Funções inicialização
var Checklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.PrazoChecklist = PropertyEntity({ text: ko.observable("*Prazo Preenchimento Checklist:"), required: ko.observable(true), getType: typesKnockout.dateTime, enable: ko.observable(true), val: ko.observable(""), visible: ko.observable(true) });
    this.PreenchimentoChecklist = PropertyEntity({ text: "Preenchimento do Checklist:", required: false, getType: typesKnockout.dynamic, options: EnumTipoPreenchimentoChecklist.ObterOpcoes(), val: ko.observable(EnumTipoPreenchimentoChecklist.PreenchimentoObrigatorio), def: EnumTipoFrete.PreenchimentoObrigatorio, enable: ko.observable(true), visible: ko.observable(true) });

    this.PrazoChecklist.val.subscribe(function () {
        _ofertas.PrazoOferta.val("");

        if (_checklist.PrazoChecklist.val() != "") {
            _ofertas.PrazoOferta.minDate(_checklist.PrazoChecklist.val());
        }
    });

    this.PreenchimentoChecklist.val.subscribe(function () {
        verificarTipoPreenchimentoChecklist(_checklist.PreenchimentoChecklist.val());
    });
}

var Questionario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.Pergunta = PropertyEntity({ text: "*Pergunta:", required: true, getType: typesKnockout.string });
    this.Requisito = PropertyEntity({ text: "*Requisito:", required: true, getType: typesKnockout.dynamic, options: _requisitoOptions, val: ko.observable("0"), def: "0" });
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

function loadChecklist() {
    _checklist = new Checklist();
    KoBindings(_checklist, "knockoutChecklist");

    _questionario = new Questionario();
    KoBindings(_questionario, "knockoutQuestionario");

    _anexosChecklist = new AnexosChecklist();
    KoBindings(_anexosChecklist, "knockoutChecklistAnexos")

    _anexoChecklist = new AnexoChecklist();
    KoBindings(_anexoChecklist, "knockoutChecklistAnexo");

    _checklistAddQuestionario = new AddQuestionario();
    KoBindings(_checklistAddQuestionario, "knockoutAddQuestionario");

    _gridQuestionario = new GridQuestionario();
    KoBindings(_gridQuestionario, "knockoutGridQuestionario");

    loadGridChecklistAnexo();
    loadGridQuestionarios();
}

function loadGridQuestionarios() {
    const linhasPorPagina = 5;
    const opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarQuestionarioClick, icone: "", visiblidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "ChecklistAnexo", visible: false },
        { data: "Descricao", title: "Pergunta", width: "60%", className: "text-align-left" },
        { data: "Requisito", title: "Requisito", width: "40%", className: "text-align-left" }
    ];
    _gridChecklistQuestionarios = new BasicDataTable(_gridQuestionario.Questionarios.id, header, menuOpcoes, { column: 0, dir: orderDir.asc }, null, linhasPorPagina);
    _gridChecklistQuestionarios.CarregarGrid([]);
}

function loadGridChecklistAnexo() {
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
    const requisitoVal = () => {
        for (let i = 0; i < _requisitoOptions.length; i++) {
            if (_requisitoOptions[i].text == registroSelecionado.Requisito) {
                return _requisitoOptions[i].value;
            }
        }
    };

    _questionario.Codigo.val(registroSelecionado.Codigo);
    _questionario.Pergunta.val(registroSelecionado.Descricao);
    _questionario.Requisito.val(requisitoVal());
    _gridChecklistAnexo.CarregarGrid(registroSelecionado.ChecklistAnexo);
}

function recarregarGridChecklist() {
    _gridChecklistQuestionarios.CarregarGrid(questionario);
}

//Funções click
function downloadChecklistAnexoClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("BiddingChecklistAnexo/DownloadAnexo", dados);
}

function removerChecklistAnexoClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerChecklistAnexoLocal(registroSelecionado);
    else {
        executarReST("BiddingChecklistAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
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

    questionario.push({ Codigo: _questionario.Codigo.val(), Descricao: _questionario.Pergunta.val(), Requisito: EnumRequisito.obterDescricao(_questionario.Requisito.val()), ChecklistAnexo: _anexosChecklist.Anexos.val() });
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
    });
}

function editarQuestionarioClick(registroSelecionado) {
    questionarioSelecionado = registroSelecionado;

    _checklistAddQuestionario.Atualizar.visible(true);
    _checklistAddQuestionario.Excluir.visible(true);
    _checklistAddQuestionario.Adicionar.visible(false);
    preencherEditarQuestionario(registroSelecionado);
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
    questionarioSelecionado.Requisito = EnumRequisito.obterDescricao(_questionario.Requisito.val());
    questionarioSelecionado.ChecklistAnexo = _anexosChecklist.Anexos.val();
    Global.fecharModal('divModalAddQuestionario');
}

function excluirQuestionarioClick() {
    let index = questionario.indexOf(questionarioSelecionado);
    questionario.splice(index, 1);
    if (!isNaN(questionarioSelecionado.Codigo))
        executarReST("BiddingConvite/ExcluirQuestionario", { Codigo: questionarioSelecionado.Codigo }, function (retorno) {
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

//Funçoes publicas
function enviarChecklistAnexos(codigo, anexos) {
    const formData = obterFormDataChecklistAnexos(anexos);

    if (!formData)
        return;

    enviarArquivo("BiddingChecklistAnexo/AnexarArquivos", { Codigo: codigo }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                return exibirMensagem(tipoMensagem.ok, "Sucesso", (retorno.Data.Anexos.length > 1) ? "Arquivos anexados com sucesso" : "Arquivo anexado com sucesso");

            return exibirMensagem(tipoMensagem.falha, (anexos.length > 1) ? "Não foi possível anexar os arquivos." : "Não foi possível anexar o arquivo.", retorno.Msg);
        }
        exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function salvarChecklist(codigo) {
    const prazoChecklist = _checklist.PrazoChecklist.val();
    const preenchimentoChecklist = _checklist.PreenchimentoChecklist.val();

    executarReST("BiddingChecklist/Adicionar", { CodigoConvite: codigo, PrazoChecklist: prazoChecklist, PreenchimentoChecklist: preenchimentoChecklist }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Checklist adicionada com sucesso");
                enviarQuestionario(retorno.Data.Codigo);
                limparCamposChecklist();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function enviarQuestionario(codigo) {
    function salvarItem(item) {
        executarReST("BiddingChecklistQuestionario/Adicionar", { CodigoChecklist: codigo, Pergunta: item.Descricao, Requisito: obterRequisitoVal(item.Requisito) }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Questão adicionada com sucesso");
                    enviarChecklistAnexos(retorno.Data.Codigo, item.ChecklistAnexo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        }, null);
    }

    _gridChecklistQuestionarios.BuscarRegistros().forEach(salvarItem);
}

function preencherChecklist(dadosBidding) {
    dadosBidding["PrazoChecklist"] = JSON.stringify(RetornarObjetoPesquisa(_checklist));
    dadosBidding["preenchimentoChecklist"] = JSON.stringify(RetornarObjetoPesquisa(_checklist));
    dadosBidding["Checklist"] = ObterListaChecklist();
}

function ObterListaChecklist() {
    const listaQuestionarios = ObterListaQuestionarios();
    const listaQuestionariosSalvar = new Array();

    for (let i = 0; i < listaQuestionarios.length; i++) {
        const questao = listaQuestionarios[i];
        listaQuestionariosSalvar.push({
            Codigo: questao.Codigo,
            Pergunta: questao.Descricao,
            Requisito: questao.Requisito
        });
    }

    return JSON.stringify(listaQuestionariosSalvar);
}

function verificarTipoPreenchimentoChecklist(tipoPreenchimentoChecklist) {
    if (tipoPreenchimentoChecklist == 0) {
        if (!_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
            _checklist.PrazoChecklist.required(true);
        }
        _checklist.PrazoChecklist.enable(true);
        _biddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.val(true);
    }
    if (tipoPreenchimentoChecklist == 1) {
        if (!_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
            _checklist.PrazoChecklist.required(true);
        }

        _checklist.PrazoChecklist.enable(true);
        _biddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.val(false);
    }
    if (tipoPreenchimentoChecklist == 2) {
        if (!_CONFIGURACAO_TMS.PermiteRemoverObrigatoriedadeDatas) {
            _checklist.PrazoChecklist.required(false);
        }

        _checklist.PrazoChecklist.enable(false);
        _checklist.PrazoChecklist.val("");
        _biddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.val(false);
    }
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

function verificarSeExistePerguntaQuestionario() {
    const perguntas = ObterListaQuestionarios();
    const preenchimentoOpcional = _checklist.PreenchimentoChecklist.val() == EnumTipoPreenchimentoChecklist.PreenchimentoOpcional;

    if (perguntas.length == 0 && (_biddingConvite.ExigirPreenchimentoChecklistConvitePeloTransportador.val() || preenchimentoOpcional))
        return true;
    return false;
}