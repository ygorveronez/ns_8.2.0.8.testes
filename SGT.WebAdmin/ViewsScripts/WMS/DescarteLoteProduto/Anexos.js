/// <reference path="../../Enumeradores/EnumSituacaoDescarteLoteProdutoEmbarcador.js" />


//*******MAPEAMENTO*******

var _anexosDescarte;
var _gridAnexosDescarte;

var AnexosDescarte = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescarteLote = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosDescarte.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosDescartes();
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoDescarteClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadAnexosDescartes() {
    _anexosDescarte = new AnexosDescarte();
    KoBindings(_anexosDescarte, "knockoutAnexos");

    //-- Grid Anexos
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoDescarteClick, icone: "" };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoDescarteClick, icone: "", visibilidade: function () { return PodeGerenciarAnexos(); } };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 9, opcoes: [download, remover] };

    // Cabecalho
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    // Grid
    var linhasPorPaginas = 7;
    _gridAnexosDescarte = new BasicDataTable(_anexosDescarte.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosDescarte.CarregarGrid([]);
}

function downloadAnexoDescarteClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("DescarteLoteProdutoEmbarcadorAnexo/DownloadAnexo", data);
}

function removerAnexoDescarteClick(dataRow) {
    var listaAnexos = GetAnexos();

    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexos.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexos.splice(i, 1);
            }
        });

        _anexosDescarte.Anexos.val(listaAnexos);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexos())
            return exibirMensagem(tipoMensagem.atencao, "Anexos", "Situação do Descarte não permite excluir arquivos.");

        // Exclui do sistema
        executarReST("DescarteLoteProdutoEmbarcadorAnexo/ExcluirAnexo", dataRow, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    RemoveDaGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function adicionarAnexoDescarteClick() {
    // Permissao
    if (!PodeGerenciarAnexos())
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Situação do Descarte não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexosDescarte.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosDescarte.Descricao.val(),
        NomeArquivo: _anexosDescarte.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_descarteLote.Codigo.val() > 0) {
        EnviarAnexoDescarte(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexos = GetAnexos();
        listaAnexos.push(anexo);
        _anexosDescarte.Anexos.val(listaAnexos.slice());
    }

    // Limpa os campos mas mantem o codigo veiculo
    LimparCampos(_anexosDescarte);
    _anexosDescarte.Arquivo.val("");
    file.value = null;
}

//*******MÉTODOS*******
function GetAnexos() {
    return _anexosDescarte.Anexos.val().slice();
}

function RenderizarGridAnexosDescartes() {
    // Busca a lista
    var anexos = GetAnexos();

    // E chama o metodo da grid
    _gridAnexosDescarte.CarregarGrid(anexos);
}


function EnviarArquivosAnexadosAoDescarte(cb) {
    // Busca a lista
    var anexos = GetAnexos();

    if (anexos.length > 0) {
        var dados = {
            CodigoDescarteLote: _descarteLote.Codigo.val()
        }
        CriaEEnviaFormData(anexos, dados, cb);
    } 
}

function EnviarAnexoDescarte(anexo) {
    var anexos = [anexo];
    var dados = {
        CodigoDescarteLote: _descarteLote.Codigo.val(),
    }

    CriaEEnviaFormData(anexos, dados);
}

function CriaEEnviaFormData(anexos, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("DescarteLoteProdutoEmbarcadorAnexo/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosDescarte.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");

                if (cb != null)
                    cb();
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AgruparAnexosDescartes() {
    // Pega anexos
    var lista = GetAnexos();

    // Agrupa
    var agrupamento = {};
    lista.forEach(function (anexo) {
        var objGrupo = agrupamento[anexo.DescarteLote] || {
            DescarteLote: anexo.DescarteLote,
            Anexos: []
        };

        objGrupo.Anexos.push(anexo);
        agrupamento[anexo.CodigoVeiculo] = objGrupo;
    });

    return agrupamento;
}

function PreencherAnexosDescartes() {
    executarReST("DescarteLoteProdutoEmbarcadorAnexo/BuscarAnexosDescartePorCodigo", { DescarteLote: _descarteLote.Codigo.val() }, function (arg) {
        if (arg.Success) {
            _anexosDescarte.Anexos.val(arg.Data.Anexos);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    });
}

function PodeGerenciarAnexos() {
    var situacao = _descarte.Situacao.val();

    return EnumSituacaoDescarteLoteProdutoEmbarcador.AgAprovacao == situacao || EnumSituacaoDescarteLoteProdutoEmbarcador.SemRegra == situacao || situacao == 0;
}

function LimparCamposAnexos() {
    LimparCampos(_anexosDescarte);
    _anexosDescarte.Anexos.val([]);
}