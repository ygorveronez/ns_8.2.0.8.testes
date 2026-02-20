/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _descargaArmazemExterno;

var DescargaArmazemExterno = function () {
    this.CodigoJanelaDescarregamento = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.CentroDescarregamento = PropertyEntity({ text: "*Centro de Descarregamento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ text: "*Tipo de Operação:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(true) });

    this.Adicionar = PropertyEntity({
        eventClick: adicionarDescargaArmazemExterno, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true)
    });
};

function loadDescargaArmazemExterno() {
    _descargaArmazemExterno = new DescargaArmazemExterno();
    KoBindings(_descargaArmazemExterno, "knockoutDescargaArmazemExterno");
 
    BuscarCentrosDescarregamento(_descargaArmazemExterno.CentroDescarregamento);
    BuscarTiposOperacao(_descargaArmazemExterno.TipoOperacao);
}

function exibirModalDescargaArmazemExterno(cargaJanelaDescarregamento) {
    _descargaArmazemExterno.CodigoJanelaDescarregamento.val(cargaJanelaDescarregamento.Codigo);

    $("#divModalDescargaArmazemExterno")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_descargaArmazemExterno);
        });
}

function adicionarDescargaArmazemExterno(e, sender) {
    if (!ValidarCamposObrigatorios(_descargaArmazemExterno)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }
    exibirConfirmacao("Confirmação", "Deseja realmente adicionar a Descarga em Armazém Externo?", function () {
        Salvar(_descargaArmazemExterno, "JanelaDescarga/AdicionarDescargaArmazemExterno", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado com sucesso");
                    Global.fecharModal('divModalDescargaArmazemExterno');
                    _tabelaDescarregamento.Load();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}