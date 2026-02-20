/// <reference path="SolicitacaoAvaria.js" />
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
/// <reference path="../../Enumeradores/EnumSituacaoFechamentoColetaProdutor.js" />


//*******MAPEAMENTO KNOUCKOUT*******


//*******EVENTOS*******

function LoadCargasFechamento(callback) {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            callback();
        });
    });
}

function buscarCargaFechamentoProdutor() {
    $("#fdsCarga").html("");
    if (_selecaoPedidos.Carga.val() > 0) {
        var data = { Carga: _selecaoPedidos.Carga.val() };
        executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                    $("#" + knoutCarga.DivCarga.id).attr("class", "p-2");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else {
        $("#fdsCarga").html("Aguardando gerar a carga do fechamento");
    }
}


function gerarCargaClick(e, sender) {
    var dados = RetornarObjetoPesquisa(_selecaoPedidos);
    executarReST("FechamentoColetaProdutor/CriarCargas", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento criado com sucesso");
                _fechamentoColetaProdutor.Situacao.val(EnumSituacaoFechamentoColetaProdutor.Finalizado);
                _gridFechamentoColetaProdutor.CarregarGrid();
                BuscarFechamentoColetaProdutorPorCodigo(arg.Data.Codigo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}
