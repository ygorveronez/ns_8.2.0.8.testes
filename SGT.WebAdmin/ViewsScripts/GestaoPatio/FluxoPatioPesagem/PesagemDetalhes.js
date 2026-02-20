/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />

// #region Objetos Globais do Arquivo

var _pesagemDetalhes;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesagemDetalhes = function () {
    this.Destino = PropertyEntity({ text: "Destino: " });
    this.Lacre = PropertyEntity({ text: "Lacre: ", visible: ko.observable(false) });
    this.LoteInterno = PropertyEntity({ text: "Lote Interno: ", visible: ko.observable(false) });
    this.Origem = PropertyEntity({ text: "Origem: " });
    this.PesagemFinal = PropertyEntity({ text: "Pesagem Final: " });
    this.PesagemInicial = PropertyEntity({ text: "Pesagem Inicial: " });
    this.PesoLiquido = PropertyEntity({ text: "Peso Líquido: " });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: " });
};

// #endregion Classes

// #region Funções de Inicialização

function loadPesagemDetalhes() {
    _pesagemDetalhes = new PesagemDetalhes();
    KoBindings(_pesagemDetalhes, "knockoutPesagemDetalhes");
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirPesagemDetalhes(codigoFluxoGestaoPatio) {
    executarReST("Guarita/BuscarInformacoesPesagem", { FluxoGestaoPatio: codigoFluxoGestaoPatio }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_pesagemDetalhes, retorno);

                _pesagemDetalhes.Lacre.visible(retorno.Data.ExibirLacre);
                _pesagemDetalhes.LoteInterno.visible(retorno.Data.ExibirLoteInterno);

                Global.abrirModal('divModalPesagemDetalhes');
                $("#divModalPesagemDetalhes").one('hidden.bs.modal', function () {
                    LimparCampos(_pesagemDetalhes);
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

// #endregion Funções Públicas
