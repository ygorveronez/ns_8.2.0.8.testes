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
var _descarteLote;
var _descarteLoteConfigDecimal = { precision: 3, allowZero: false, allowNegative: false };

var DescarteLote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.LoteProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Lote do Produto do Embarcador:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Produto:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });

    this.Quantidade = PropertyEntity({ type: types.map, configDecimal: _descarteLoteConfigDecimal, getType: typesKnockout.decimal, val: ko.observable("0,000"), def: "", text: "*Quantidade:", enable: ko.observable(true), required: true });
    this.Numero = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Nº Lote:", enable: ko.observable(false) });
    this.CodigoBarras = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Codigo de Barras:", enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Data Vencimento:", enable: ko.observable(false) });
    this.QuantidadeLote = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Quantidade Lote:", enable: ko.observable(false) });
    this.QuantidadeAtual = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Quantidade Atual:", enable: ko.observable(false) });
    this.DepositoPosicao = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Local Armazenamento Atual:", enable: ko.observable(false) });

    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "*Motivo:", enable: ko.observable(true), required: true });
}


//*******EVENTOS*******
function loadDescarteLote() {
    _descarteLote = new DescarteLote();
    KoBindings(_descarteLote, "knockoutDescarteLote");

    HeaderAuditoria("DescarteLoteProdutoEmbarcador", _descarteLote);

    new BuscarProdutoEmbarcadorLote(_descarteLote.LoteProdutoEmbarcador, RetornarProdutoSelecionado);
    new BuscarProdutoTMS(_descarteLote.Produto);

    if (_CONFIGURACAO_TMS.EmitirNFeRemessaNaCarga) {
        _descarteLote.Produto.required(true);
        _descarteLote.Produto.text("*Produto:");
    }
}


//*******MÉTODOS*******
function EditarDadosDescarte(data) {
    _descarteLote.Codigo.val(data.Codigo);
    if (data.DadosDescarte != null) {
        PreencherObjetoKnout(_descarteLote, { Data: data.DadosDescarte });
    } else {
        _descarteLote.Emitir.visible(false);
    }

    ControleCamposDescarteLote(false);
}

function RetornarProdutoSelecionado(data){
    _descarteLote.LoteProdutoEmbarcador.codEntity(data.Codigo);
    _descarteLote.LoteProdutoEmbarcador.val(data.Descricao);

    _descarteLote.Numero.val(data.Numero);
    _descarteLote.DataVencimento.val(data.DataVencimento);
    _descarteLote.QuantidadeLote.val(data.QuantidadeLote);
    _descarteLote.QuantidadeAtual.val(data.QuantidadeAtual);
    _descarteLote.CodigoBarras.val(data.CodigoBarras);
    _descarteLote.DepositoPosicao.val(data.DepositoPosicao);
}

function ControleCamposDescarteLote(status) {
    _descarteLote.LoteProdutoEmbarcador.enable(status);
    _descarteLote.Produto.enable(status);
    _descarteLote.Quantidade.enable(status);
    _descarteLote.Motivo.enable(status);
}

function LimparCamposDadosDescarte() {
    LimparCampos(_descarteLote);
    ControleCamposDescarteLote(true);
}

function AnexarArquivos(anexos) {
    // Dados da req
    var dados = {
        Codigo: _descarteLote.Codigo.val()
    };

    // Arquivos
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        formData.append("Tipo", anexo.Tipo);
        formData.append("Arquivo", anexo.Arquivo);
    });

    enviarArquivo("NFSManual/Anexar", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo(s) anexado(s) com sucesso");
                _descarteLote.XMLAutorizacao.file.files = null;
                _descarteLote.DANFSE.file.files = null;
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar o(s) arquivo(s).", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}