/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />

var _anexoHistorico;

var AnexoHistorico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição:", type: types.map, getType: typesKnockout.string, maxlength: 150, required: true });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "Arquivo:", val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoHistorico.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoHistoricoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

function loadEtapaAcompanhamentoHistoricoAnexo() {
    _anexoHistorico = new AnexoHistorico();
    KoBindings(_anexoHistorico, "knockoutAcompanhamentoHistoricoAnexo");
}

function adicionarAnexoHistoricoClick() {
    var file = document.getElementById(_anexoHistorico.Arquivo.id);

    if (file.files.length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Anexos", "Nenhum arquivo selecionado.");
    
    var data = {
        Codigo: guid(),
        Descricao: _anexoHistorico.Descricao.val(),
        NomeArquivo: _anexoHistorico.NomeArquivo.val(),
        Arquivo: file.files[0]
    };
    
    _acompanhamentoAdicionarHistorico.Anexos.val.push(data);
    
    Global.fecharModal('divModalAcompanhamentoHistoricoAnexo');
}

function adicionarAnexoHistoricoModalClick() {
    $("#divModalAcompanhamentoHistoricoAnexo")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_anexoHistorico);
        });
}