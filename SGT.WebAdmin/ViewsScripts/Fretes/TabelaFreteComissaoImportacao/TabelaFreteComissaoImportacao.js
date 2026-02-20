/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/ContratoFreteTransportador.js" />
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
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="TabelaFreteComissaoProdutoAjuste.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _tabelaFreteComissaoImportacao;

var TabelaFreteComissaoImportacao = function () {
    this.ContratoFreteTransportadorOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Contrato de Frete de Origem:", idBtnSearch: guid() });
    this.ContratoFreteTransportadorDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Contrato de Frete de Destino:", idBtnSearch: guid() });

    this.Importar = PropertyEntity({ eventClick: ImportarClick, type: types.event, text: "Importar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadTabelaFreteComissaoImportacao() {
    _tabelaFreteComissaoImportacao = new TabelaFreteComissaoImportacao();
    KoBindings(_tabelaFreteComissaoImportacao, "knockoutTabelaFreteComissaoImportacao");

    new BuscarContratoFreteTransportador(_tabelaFreteComissaoImportacao.ContratoFreteTransportadorOrigem);
    new BuscarContratoFreteTransportador(_tabelaFreteComissaoImportacao.ContratoFreteTransportadorDestino);
}

function ImportarClick(e, sender) {
    Salvar(e, "TabelaFreteComissaoImportacao/Importar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação realizada com sucesso.");
                LimparCamposTabelaFreteComissaoImportacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

//********METODOS*********

function LimparCamposTabelaFreteComissaoImportacao() {
    LimparCampos(_tabelaFreteComissaoImportacao);
}