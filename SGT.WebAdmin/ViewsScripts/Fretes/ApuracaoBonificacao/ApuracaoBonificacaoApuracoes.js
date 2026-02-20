/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoPontuacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _apuracaoBonificacaoApuracoes;
var _gridApuracaoFechamento;

/*
 * Declaração das Classes
 */

var ApuracaoBonificacaoApuracoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TotalAcrescimo = PropertyEntity({ text: "Total Acréscimo: ", val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.TotalDesconto = PropertyEntity({ text: "Total Desconto: ", val: ko.observable(0), def: 0, getType: typesKnockout.decimal });
    this.ApuracaoBonificacao = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridApuracaoFechamento() {
    const configuracaoExportacao = { url: "ApuracaoBonificacao/ExportarPesquisaFechamentoApuracao", titulo: "Fechamento - Apurações de Bonificação" };

    _gridApuracaoFechamento = new GridViewExportacao(_apuracaoBonificacaoApuracoes.ApuracaoBonificacao.idGrid, "ApuracaoBonificacao/PesquisaFechamentoApuracao", _apuracaoBonificacaoApuracoes, null, configuracaoExportacao);
    _gridApuracaoFechamento.CarregarGrid();
}


function loadApuracaoBonificacaoApuracoes() {
    _apuracaoBonificacaoApuracoes = new ApuracaoBonificacaoApuracoes();
    KoBindings(_apuracaoBonificacaoApuracoes, "knockoutApuracaoBonificacaoApuracoes");

    loadGridApuracaoFechamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */



/*
 * Declaração das Funções Públicas
 */

function gerarOcorrenciasPGT() {
    exibirConfirmacao("Confirmação", "Você realmente deseja gerar as ocorrências?", function () {
        executarReST("ApuracaoBonificacao/GerarOcorrenciasPGT", { Codigo: _apuracaoBonificacao.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrências geradas com sucesso");
                    buscarApuracaoBonificacaoPorCodigo(_apuracaoBonificacao.Codigo.val());
                    recarregarGridApuracaoBonificacao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Privadas
 */

function recarregarGridApuracaoFechamento(e) {
    _apuracaoBonificacaoApuracoes.Codigo.val(e.Codigo);
    _apuracaoBonificacaoApuracoes.TotalAcrescimo.val(e.TotalAcrescimo);
    _apuracaoBonificacaoApuracoes.TotalDesconto.val(e.TotalDesconto);
    _gridApuracaoFechamento.CarregarGrid();
}