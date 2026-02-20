/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="PagamentoMotoristaTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoSolicitacaoCredito.js" />
/// <reference path="EtapasPagamentoMotorista.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pagamentoMotoristaAutorizacao;
var _gridAutorizacoes;

var PagamentoMotoristaAutorizacao = function () {

    this.SituacaoSolicitacao = PropertyEntity({ visible: ko.observable(false) });
    this.DescricaoSituacao = PropertyEntity({ visible: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ text: "Data do Solicitação: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Solicitado = PropertyEntity({ text: "Solicitante:", visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: "Motorista:", visible: ko.observable(true) });
    this.TipoPagamentoMotorista = PropertyEntity({ text: "Tipo do Pagamento:", visible: ko.observable(true) });
    this.ValorSolicitado = PropertyEntity({ text: "Valor Solicitado: ", getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", visible: ko.observable(true) });


    this.DataRetorno = PropertyEntity({ text: "Data do Retorno: ", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.Creditor = PropertyEntity({ text: "Supervisor:", visible: ko.observable(true) });
    this.ValorLiberado = PropertyEntity({ text: "Valor Aprovado: ", getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.RetornoSolicitacao = PropertyEntity({ type: types.map, maxlength: 300, text: "Resposta:", visible: ko.observable(true) });

    this.Autorizacao = PropertyEntity({ visible: ko.observable(false) });

    this.ConfirmarPagamentoMotorista = PropertyEntity({ eventClick: confirmarPagamentoMotoristaClick, type: types.event, text: "Confirmar", visible: ko.observable(false) });
    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });

    // Mensagem da etapa
    this.MensagemEtapaSemRegra = PropertyEntity({ type: types.local, visible: ko.observable(false), title: "Autorização Pendente", text: '<b class="margin-bottom-10" style="display:block">Nenhuma regra encontrada.</b><b>Pagamento permanece aguardando autorização.</b>' });
};

//*******EVENTOS*******

function loadPagamentoMotoristaAutorizacao() {
    _pagamentoMotoristaAutorizacao = new PagamentoMotoristaAutorizacao();
    KoBindings(_pagamentoMotoristaAutorizacao, "knockoutPagamentoMotoristaAprovacao");

    _gridAutorizacoes = new GridView(_pagamentoMotoristaAutorizacao.UsuariosAutorizadores.idGrid, "PagamentoMotoristaTMS/ConsultarAutorizacoes", _pagamentoMotorista, null, null, null, null, null, null, null);
}

function confirmarPagamentoMotoristaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja confirmar o pagamento ao motorista ?", function () {
        Salvar(_pagamentoMotorista, "PagamentoMotoristaTMS/ConfirmarPagamento", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridPagamentoMotorista.CarregarGrid();
                    _pagamentoMotorista.Situacao.val(arg.Data.SituacaoPagamentoMotorista);
                    if (arg.Data.SituacaoPagamentoMotorista == EnumSituacaoPagamentoMotorista.FinalizadoPagamento) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento ao motorista finalizada com sucesso!");

                        ImprimirReciboAdiantamentoMotorista();

                        _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(false);
                        _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
                        _CRUDPagamentoMotorista.GerarNovaPagamentoMotorista.visible(false);
                        _CRUDPagamentoMotorista.Adicionar.visible(true);

                        limparCamposPagamentoMotorista();
                        setarEtapaInicioPagamentoMotorista();
                    }

                    if (!string.IsNullOrWhiteSpace(arg.Data.MensagemRetorno))
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.MensagemRetorno);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, sender);
    });
}

function buscarRegrasPagamentoMotoristaClick() {
    BuscarRegrasDaEtapa();
}

//*******MÉTODOS*******

function preecherPagamentoMotoristaAutorizacaoKnout(knout, grid, dadosAutorizacao) {
    if (dadosAutorizacao != null) {
        knout.UsuariosAutorizadores.visible(false);
        knout.DescricaoSituacao.visible(true)
        knout.SituacaoSolicitacao.visible(true);
        knout.Autorizacao.visible(false);
        var data = { Data: dadosAutorizacao };
        PreencherObjetoKnout(knout, data);

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.AgLiberacao || dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.AutorizacaoPendente || !dadosAutorizacao.ComRegraAutorizacao) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Liberado) {
            knout.ConfirmarPagamentoMotorista.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Utilizado) {
            knout.ConfirmarPagamentoMotorista.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Rejeitado) {
            knout.ValorLiberado.visible(false);
        }

        if (dadosAutorizacao.SituacaoSolicitacao == EnumSituacaoSolicitacaoCredito.Todos) {
            knout.DataRetorno.visible(false);
            knout.Creditor.visible(false);
            knout.ValorLiberado.visible(false);
            knout.RetornoSolicitacao.visible(false);
            knout.DescricaoSituacao.visible(false)
        }
    } else {
        knout.UsuariosAutorizadores.visible(false);
        knout.SituacaoSolicitacao.visible(false);
        knout.Autorizacao.visible(true);
    }

    grid.CarregarGrid(function () {
        if (grid.NumeroRegistros() > 0) {
            knout.UsuariosAutorizadores.visible(true);
            knout.Autorizacao.visible(false);
        }
    });
}

function preecherPagamentoMotoristaAutorizacao(dadosAutorizacao) {
    preecherPagamentoMotoristaAutorizacaoKnout(_pagamentoMotoristaAutorizacao, _gridAutorizacoes, dadosAutorizacao);

}

function limparPagamentoMotoristaAutorizacaoKnout(knout) {
    knout.DataRetorno.visible(true);
    knout.Creditor.visible(true);
    knout.RetornoSolicitacao.visible(true);
    knout.ValorLiberado.visible(true);
    knout.ConfirmarPagamentoMotorista.visible(false);
    //knout.ImprimirRecibo.visible(false);
    knout.SituacaoSolicitacao.visible(false);
}

function limparPagamentoMotoristaAutorizacao() {
    limparPagamentoMotoristaAutorizacaoKnout(_pagamentoMotoristaAutorizacao);
    LimparCampos(_pagamentoMotoristaAutorizacao);
}
function BuscarRegrasDaEtapa() {
    var dados = {
        PagamentoMotorista: _pagamentoMotorista.Codigo.val()
    };
    executarReST("PagamentoMotoristaTMS/AtualizarRegrasEtapas", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                Etapa3Desabilitada();
                _pagamentoMotorista.SituacaoPagamentoMotorista.val(arg.Data.SituacaoPagamentoMotorista);
                setarEtapasPagamentoMotorista();
                preecherPagamentoMotoristaAutorizacao(_pagamentoMotorista.SolicitacaoCredito.val());
                _CRUDPagamentoMotorista.ReprocessarRegras.visible(false);
            } else if (arg.Msg != null) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                EtapaSemRegra();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}