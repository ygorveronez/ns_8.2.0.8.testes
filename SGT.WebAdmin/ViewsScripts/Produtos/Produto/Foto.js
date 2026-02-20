/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Produto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _produtoFoto;

var ProdutoFoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DescricaoFoto = PropertyEntity({ text: "*Descrição da foto:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(false) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), required: ko.observable(false), visible: ko.observable(true) });

    this.RemoverFoto = PropertyEntity({ eventClick: RemoverFotoClick, type: types.event, text: "Remover Foto", visible: ko.observable(true), enable: ko.observable(true) });

    this.FotoProduto = PropertyEntity({ val: ko.observable(""), def: "" });

    this.Arquivo.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado) {
            carregarFotoProduto();
            _produtoFoto.DescricaoFoto.enable(true);
        } else {
            _produtoFoto.DescricaoFoto.enable(false);
        }
    });
}

//*******EVENTOS*******

function loadProdutoFoto() {
    _produtoFoto = new ProdutoFoto();
    KoBindings(_produtoFoto, "knockoutFotoProduto");
}

function RemoverFotoClick(e, sender) {
    if (_produtoFoto.Arquivo.val() != "" || _produtoFoto.FotoProduto.val() != "") {
        exibirConfirmacao("Confirmação", "Realmente deseja excluir a Foto do Produto?", function () {
            var codigoFoto = _produtoFoto.Codigo.val();
            limparCamposProdutoFoto();
            _produtoFoto.Codigo.val(codigoFoto);
        });
    }
}

////*******MÉTODOS*******

function limparCamposProdutoFoto() {
    LimparCampos(_produtoFoto);
    _produtoFoto.Arquivo.val("");
    _produtoFoto.FotoProduto.val("");
    _produtoFoto.DescricaoFoto.enable(false);
    _produtoFoto.Arquivo.visible(true);
}

function validaCamposObrigatoriosFoto() {
    if (_produtoFoto.DescricaoFoto.val() == "" && _produtoFoto.FotoProduto.val() != "")
        return false;

    return true;
}

//O campo 'FotoProduto' possuirá valor sempre que tiver uma foto sendo exibida. Seja ela carregada do banco ou carregada de um arquivo selecionado...
function temFoto() {
    if (_produtoFoto.FotoProduto.val() != "")
        return true;

    return false;
}

//O campo 'Arquivo' só é preenchido quando é selecionado uma nova foto. Portanto, só será necessário salvar a foto quando este campo possuir valor...
function adicionarFoto() {
    if (_produtoFoto.Arquivo.val() != "")
        return true;

    return false;
}

function carregarFotoProduto() {
    var formData = obterFormDataFotoProduto();

    if (formData) {
        enviarArquivo("Produto/CarregarFotoProduto?callback=?", { }, formData, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    _produtoFoto.FotoProduto.val(retorno.Data.FotoProduto);
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function obterFormDataFotoProduto() {
    var arquivo = document.getElementById(_produtoFoto.Arquivo.id);
    
    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("Arquivo", arquivo.files[0]);

        return formData;
    }

    return undefined;
}