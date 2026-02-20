/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _anexosAutorizacao;
var _gridAnexosAutorizacao;
var _ArquivosParaImportar

var AnexosAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AnexosAutorizacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, idGrid: guid() });

    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarAnexoAutorizacaoClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AdicionarAnexo, visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, eventChange: enviarArquivoAnexo, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Arquivo, val: ko.observable(""), visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadAnexosAutorizacao() {
    _anexosAutorizacao = new AnexosAutorizacao();
    KoBindings(_anexosAutorizacao, "knockoutAnexosAutorizacao");

    //-- Grid Anexos
    // Opcao de downlaod
    var download = {
        descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Download,
        id: "clasEditar",
        evento: "onclick",
        metodo: downloadAnexoClick,
        tamanho: 15,
        icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    // Gera grid
    var linhasPorPaginas = 7;
    _gridAnexosAutorizacao = new GridView(_anexosAutorizacao.AnexosAutorizacao.idGrid, "OcorrenciaAnexos/Pesquisa", _anexosAutorizacao, menuOpcoes, null);
}

function downloadAnexoClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("OcorrenciaAnexos/DownloadAnexo", data);
}




//*******MÉTODOS*******

function limparAnexos() {
    LimparCampos(_anexosAutorizacao);
    _gridAnexosAutorizacao.CarregarGrid();
}

function CarregarAnexosAutorizacao(ocorrencia, recursivo) {
    // Anexos da Ocorrência/Motorista
    _anexosAutorizacao.Codigo.val(ocorrencia);
    _gridAnexosAutorizacao.CarregarGrid(function () {
        //Alterado para sempre exibir a aba anexos
        //if (_gridAnexosAutorizacao.NumeroRegistros() == 0)
        //    $("#liAnexosAutorizacao").hide();
        //else
        $("#liAnexosAutorizacao").show();
    });

    //_anexosAutorizacao.Codigo.val(ocorrencia);
    //if (typeof recursivo == "undefined")
    //    recursivo = 0;

    //$("#liAnexosAutorizacao").show();

    //_gridAnexosAutorizacao.CarregarGrid(function () {
    //    if (_gridAnexosAutorizacao.NumeroRegistros() == 0) {
    //        recursivo++;
    //        if (recursivo < 3) {
    //            $("#liAnexos").hide();
    //            CarregarAnexosAutorizacao(ocorrencia, recursivo);
    //        }
    //    }
    //});
}

function adicionarAnexoAutorizacaoClick() {
    $("#" + _anexosAutorizacao.Arquivo.id).trigger("click");
}

function enviarArquivoAnexo(e, sender) {
    if (_anexosAutorizacao.Arquivo.val() != "") {
        var file = document.getElementById(_anexosAutorizacao.Arquivo.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        var dados = {
            Codigo: _anexosAutorizacao.Codigo.val()
        };

        enviarArquivo("AutorizacaoOcorrencia/AnexarArquivosAutorizacao", dados, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridAnexosAutorizacao.CarregarGrid();
                } else {
                    _gridAnexosAutorizacao.CarregarGrid();
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                _gridAnexosAutorizacao.CarregarGrid();
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}