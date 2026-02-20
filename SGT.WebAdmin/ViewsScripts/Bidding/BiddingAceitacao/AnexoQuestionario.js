var _anexoChecklist, _listaAnexoChecklist, _gridAnexoChecklist;
var listaAnexos = [];

var AnexoChecklist = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoChecklist.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoChecklistClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

var ListaAnexoChecklist = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), id: guid(), visible: ko.observable(true) });
}

function loadAnexoQuestionario() {
    _anexoChecklist = new AnexoChecklist();
    KoBindings(_anexoChecklist, "knockoutAnexoChecklist");

    _listaAnexoChecklist = new ListaAnexoChecklist();
    KoBindings(_listaAnexoChecklist, "knockoutListaAnexosChecklist");

    loadGridAnexoChecklist();
}

function loadGridAnexoChecklist() {
    var linhasPorPaginas = 10;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoChecklistClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAnexoChecklistClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoPergunta", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoChecklist = new BasicDataTable(_listaAnexoChecklist.Anexos.id, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoChecklist.CarregarGrid([]);
}

function adicionarAnexoChecklistClick() {
    var arquivo = document.getElementById(_anexoChecklist.Arquivo.id);

    if (arquivo.files.length == 0) {
        exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
        return;
    }

    if (!ValidarCamposObrigatorios(_anexoChecklist)) {
        exibirMensagem(tipoMensagem.atencao, "Campo vazio", "Você precisa informar uma descrição.")
        return;
    }

    var anexo = {
        Codigo: guid(),
        CodigoPergunta: _codigoPergunta,
        Descricao: _anexoChecklist.Descricao.val(),
        NomeArquivo: _anexoChecklist.NomeArquivo.val(),
        Arquivo: arquivo.files[0]
    };

    listaAnexos.push(anexo);

    _listaAnexoChecklist.Anexos.val(listaAnexos.slice());

    _anexoChecklist.Arquivo.val("");

    Global.fecharModal("divModalAnexo");
}

function removerAnexoChecklistClick(registroSelecionado) {
    if (isNaN(registroSelecionado.Codigo))
        removerAnexoChecklistLocal(registroSelecionado);
    else {
        executarReST("BiddingAceitamentoAnexo/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    removerAnexoChecklistLocal(registroSelecionado);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function downloadAnexoChecklistClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    if (isNaN(registroSelecionado.Codigo)) {
        exibirMensagem("atencao", "Não é possível fazer o download do anexo pois o mesmo ainda não foi enviado.");
        return;
    }
    executarDownload("BiddingAceitamentoAnexo/DownloadAnexo", dados);
}

function obterAnexosCodigoPergunta(codigoPergunta) {
    let arrayAnexos = [];
    for (let i = 0; i < _listaAnexoChecklist.Anexos.val().length; i++) {
        let item = _listaAnexoChecklist.Anexos.val()[i];
        if (item.CodigoPergunta == _codigoPergunta)
            arrayAnexos.push(item);
    }

    return arrayAnexos;
}

function removerAnexoChecklistLocal(registroSelecionado) {
    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });
    _listaAnexoChecklist.Anexos.val(listaAnexos.slice());
    recarregarGrid();
}

function recarregarGrid() {
    var anexos = obterAnexosCodigoPergunta(_codigoPergunta);
    _gridAnexoChecklist.CarregarGrid(anexos);
}
