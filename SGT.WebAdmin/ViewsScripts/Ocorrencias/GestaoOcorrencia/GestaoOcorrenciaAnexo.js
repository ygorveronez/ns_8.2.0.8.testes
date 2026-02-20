/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _anexosGestaoOcorrencia;
var _gridAnexosGestaoOcorrencia;

var AnexosGestaoOcorrencia = function () {
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //-- Adicionar arquivo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Anexo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Arquivo.val.subscribe(function (novoValor) {
        var nomeArquivo = novoValor.replace('C:\\fakepath\\', '');
        _anexosGestaoOcorrencia.NomeArquivo.val(nomeArquivo);
    });

    this.Anexos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(function () {
        RenderizarGridAnexosGestaoOcorrencia();
    });
    this.QuantidadeImagensEsperada = PropertyEntity({ val: ko.observable(0), def: 0, text: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoChamadoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadGestaoOcorrenciaAnexo() {
    _anexosGestaoOcorrencia = new AnexosGestaoOcorrencia();
    KoBindings(_anexosGestaoOcorrencia, "knockoutCadastroAnexosGestaoOcorrencia");

    //-- Grid AnexosGestaoOcorrencia
    // Opcoes
    var download = { descricao: "Download", id: guid(), metodo: downloadAnexoChamadoClick, icone: "", visibilidade: visibleDownloadChamado };
    var remover = { descricao: "Remover", id: guid(), metodo: removerAnexoChamadoClick, icone: "", visibilidade: visibleRemoverChamado };

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
    _gridAnexosGestaoOcorrencia = new BasicDataTable(_anexosGestaoOcorrencia.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexosGestaoOcorrencia.CarregarGrid([]);

    //-- Controle de AnexosGestaoOcorrencia
    _gestaoOcorrencia.SituacaoChamado.val.subscribe(function () {
        AlternaTelaDeAnexosGestaoOcorrencia(_anexosGestaoOcorrencia);
    });
}

function visibleDownloadChamado(dataRow) {
    return !isNaN(dataRow.Codigo);
}

function visibleRemoverChamado(dataRow) {
    return PodeGerenciarAnexosGestaoOcorrencia();
}

function downloadAnexoChamadoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("ChamadoAnexos/DownloadAnexo", data);
}

function removerAnexoChamadoClick(dataRow) {
    var listaAnexosGestaoOcorrencia = GetAnexosGestaoOcorrencia();
    RemoverAnexoChamado(dataRow, listaAnexosGestaoOcorrencia, _anexosGestaoOcorrencia, "ChamadoAnexos/ExcluirAnexo");
}

function gerenciarAnexosGestaoOcorrenciaClick() {
    LimparCamposAnexosChamado();
    Global.abrirModal('divModalGerenciarAnexosGestaoOcorrencia');
}

function adicionarAnexoChamadoClick() {
    // Permissao
    if (!PodeGerenciarAnexosGestaoOcorrencia())
        return exibirMensagem(tipoMensagem.atencao, "Atenção", "Situação da Ocorrência não permite anexar arquivos.");

    // Busca o input de arquivos
    var file = document.getElementById(_anexosGestaoOcorrencia.Arquivo.id);

    // Valida
    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Atençaõ", "Nenhum arquivo selecionado.");

    // Monta objeto anexo
    var anexo = {
        Codigo: guid(),
        Descricao: _anexosGestaoOcorrencia.Descricao.val(),
        NomeArquivo: _anexosGestaoOcorrencia.NomeArquivo.val(),
        Arquivo: file.files[0]
    };

    // Se ja esta cadastrada a ocorrencia, envia o anexo direto
    if (_gestaoOcorrencia.CodigoChamado.val() > 0) {
        EnviarAnexoChamado(anexo);
    } else {
        // Clona a lista e adiciona o form data
        var listaAnexosGestaoOcorrencia = GetAnexosGestaoOcorrencia();
        listaAnexosGestaoOcorrencia.push(anexo);
        _anexosGestaoOcorrencia.Anexos.val(listaAnexosGestaoOcorrencia.slice());
    }

    // Limpa os campos
    LimparCamposAnexosChamado();
}

//*******MÉTODOS*******

function LimparCamposAnexosChamado() {
    var file = document.getElementById(_anexosGestaoOcorrencia.Arquivo.id);
    LimparCampos(_anexosGestaoOcorrencia);
    _anexosGestaoOcorrencia.Arquivo.val("");
    file.value = null;

}

function GetAnexosGestaoOcorrencia() {
    // Retorna um clone do array para não prender a referencia
    return _anexosGestaoOcorrencia.Anexos.val().slice();
}

function CarregarAnexosGestaoOcorrencia(data) {
    _anexosGestaoOcorrencia.Anexos.val(data.Anexos);
    _anexosGestaoOcorrencia.QuantidadeImagensEsperada.val(data.QuantidadeImagensEsperada);

    var diferencaAnexos = _anexosGestaoOcorrencia.QuantidadeImagensEsperada.val() - data.Anexos.length;
    if (diferencaAnexos > 0) {
        _anexosGestaoOcorrencia.QuantidadeImagensEsperada.text("Ainda faltam ser sincronizado(s) " + diferencaAnexos + " anexo(s), por favor aguarde alguns instantes e acesse novamente o atendimento para ver se os anexos faltantes chegaram.");
        _anexosGestaoOcorrencia.QuantidadeImagensEsperada.visible(true);
    } else {
        _anexosGestaoOcorrencia.QuantidadeImagensEsperada.text("");
        _anexosGestaoOcorrencia.QuantidadeImagensEsperada.visible(false);
    }
}

function RenderizarGridAnexosGestaoOcorrencia() {
    // Busca a lista
    var AnexosGestaoOcorrencia = GetAnexosGestaoOcorrencia();

    // E chama o metodo da grid
    _gridAnexosGestaoOcorrencia.CarregarGrid(AnexosGestaoOcorrencia);
}

function EnviarArquivosAnexadosChamado(cb, codigoGestaoOcorrencia) {
    if (codigoGestaoOcorrencia == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Não foi possível anexar os arquivos pois não foi gerado um chamado.");

        if (cb != undefined)
            cb();
        
        return;
    }

    // Busca a lista
    var AnexosGestaoOcorrencia = GetAnexosGestaoOcorrencia();

    if (AnexosGestaoOcorrencia.length > 0) {
        var dados = {
            Chamado: codigoGestaoOcorrencia
        }
        CriaEEnviaFormDataChamado(AnexosGestaoOcorrencia, dados, cb);
    } else if (cb != null) {
        cb();
    }
}

function RemoverAnexoChamado(dataRow, listaAnexosGestaoOcorrencia, ko, url) {
    // Funcao auxiliar
    var RemoveDaGrid = function () {
        listaAnexosGestaoOcorrencia.forEach(function (anexo, i) {
            if (dataRow.Codigo == anexo.Codigo) {
                listaAnexosGestaoOcorrencia.splice(i, 1);
            }
        });

        ko.Anexos.val(listaAnexosGestaoOcorrencia);
    }

    // Se for arquivo local, apenas remove do array
    if (isNaN(dataRow.Codigo)) {
        RemoveDaGrid();
    } else {
        // Permissao
        if (!PodeGerenciarAnexosGestaoOcorrencia())
            return exibirMensagem(tipoMensagem.atencao, "Atenção", "Situação da Ocorrência não permite excluir arquivos.");

        // Exclui do sistema
        executarReST(url, dataRow, function (arg) {
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

function CriaEEnviaFormDataChamado(AnexosGestaoOcorrencia, dados, cb) {
    // Busca todos file
    var formData = new FormData();
    AnexosGestaoOcorrencia.forEach(function (anexo) {
        if (isNaN(anexo.Codigo)) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        }
    });

    enviarArquivo("ChamadoAnexos/AnexarArquivos?callback=?", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _anexosGestaoOcorrencia.Anexos.val(arg.Data.Anexos);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo anexado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar arquivo.", arg.Msg);
            }
            if (cb)
                cb();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function EnviarAnexoChamado(anexo) {
    var AnexosGestaoOcorrencia = [anexo];
    var dados = {
        Chamado: _gestaoOcorrencia.CodigoChamado.val()
    }

    CriaEEnviaFormDataChamado(AnexosGestaoOcorrencia, dados);
}

function limparOcorrenciaAnexosGestaoOcorrencia() {
    LimparCampos(_anexosGestaoOcorrencia);
    _anexosGestaoOcorrencia.Anexos.val(_anexosGestaoOcorrencia.Anexos.def);
    _anexosGestaoOcorrencia.Adicionar.visible(true);
}

function PodeGerenciarAnexosGestaoOcorrencia() {
    var situacao = _gestaoOcorrencia.SituacaoChamado.val();

    return situacao != EnumSituacaoChamado.Finalizado && situacao != EnumSituacaoChamado.LiberadaOcorrencia && situacao != EnumSituacaoChamado.Cancelada;
}

function AlternaTelaDeAnexosGestaoOcorrencia(ko) {
    if (PodeGerenciarAnexosGestaoOcorrencia()) {
        ko.Anexos.visible(true);
    } else {
        ko.Anexos.visible(false);
    }
}

function isPossuiAnexo() {
    return GetAnexosGestaoOcorrencia().length > 0;
}