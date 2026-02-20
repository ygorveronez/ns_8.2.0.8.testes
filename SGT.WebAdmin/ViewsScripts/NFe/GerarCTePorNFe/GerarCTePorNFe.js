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

//*******MAPEAMENTO KNOUCKOUT*******

var _gerarCTePorNFe;
var _pesquisaCTe;
var _gridCTesEmitidos;

var GerarCTePorNFe = function () {
    this.Arquivos = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*NF-es:", val: ko.observable(""), required: true });
    this.ValorFrete = PropertyEntity({ text: "*Valor do Frete: ", val: ko.observable(0), getType: typesKnockout.decimal, required: true });
    this.ValorTotalMercadoria = PropertyEntity({ text: "Valor Total da Mercadoria: ", val: ko.observable(0), getType: typesKnockout.decimal });
    this.DataEmissao = PropertyEntity({ text: "*Data de Emissão: ", val: ko.observable(""), required: true });
    this.Observacao = PropertyEntity({ text: "Observação: ", val: ko.observable("") });

    this.GerarCTe = PropertyEntity({ eventClick: gerarCTePorNFeClick, type: types.event, text: "Gerar CT-e" });
}

var PesquisaCTe = function () {
    this.Atualizar = PropertyEntity({
        eventClick: function (e) {
            _gridCTesEmitidos.CarregarGrid();
        }, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(true)
    });
}


//*******EVENTOS*******

function loadGerarCTePorNFe() {
    _gerarCTePorNFe = new GerarCTePorNFe();
    KoBindings(_gerarCTePorNFe, "knockoutGerarCTePorNFe");

    _pesquisaCTe = new PesquisaCTe();
    KoBindings(_pesquisaCTe, "knockoutCTesGerados");

    CarregarCTesEmitidos();

    $("#" + _gerarCTePorNFe.DataEmissao.id).mask("00/00/0000 00:00", { selectOnFocus: true, clearIfNotMatch: true });
    _gerarCTePorNFe.DataEmissao.val(Global.DataHoraAtual());
}

function CarregarCTesEmitidos() {
    _gridCTesEmitidos = new GridView(_pesquisaCTe.Atualizar.idGrid, "GerarCTePorNFe/Pesquisar", _pesquisaCTe, null, { column: 0, dir: orderDir.asc }, 10, null);
    _gridCTesEmitidos.CarregarGrid();
}

function gerarCTePorNFeClick() {
    if (ValidarCamposObrigatorios(_gerarCTePorNFe)) {
        var documentos = new Array();
        var file = document.getElementById(_gerarCTePorNFe.Arquivos.id);
        var fileCount = file.files.length;

        for (var i = 0; i < fileCount; i++) {
            var formData = new FormData();
            formData.append("upload", file.files[i]);

            enviarArquivo("GerarCTePorNFe/ObterDocumentoParaGeracao?callback=?", {}, formData, function (arg) {
                if (arg.Success) {
                    documentos.push({ NFe2: arg.Data.versao == "2.00" ? arg.Data : null, NFe3: arg.Data.versao == "3.10" ? arg.Data : null });

                    if (documentos.length == fileCount) {
                        executarReST("GerarCTePorNFe/GerarCTePorListaNFe", { ValorFrete: _gerarCTePorNFe.ValorFrete.val(), ValorTotalMercadoria: _gerarCTePorNFe.ValorTotalMercadoria.val(), DataEmissao: _gerarCTePorNFe.DataEmissao.val(), Observacao: _gerarCTePorNFe.Observacao.val(), Documentos: JSON.stringify(documentos) }, function (retGeracao) {
                            if (retGeracao.Success) {
                                exibirMensagem(tipoMensagem.ok, "Sucesso!", "CT-e gerado com sucesso!");
                            } else {
                                exibirMensagem(tipoMensagem.falha, "Falha!", retGeracao.Msg);
                            }

                            LimparCampos(_gerarCTePorNFe);

                            _gerarCTePorNFe.Arquivos.val("");
                            _gerarCTePorNFe.DataEmissao.val(Global.DataHoraAtual());

                            _gridCTesEmitidos.CarregarGrid();
                        });
                    }

                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
                    return;
                }
            });
        }
    }
}