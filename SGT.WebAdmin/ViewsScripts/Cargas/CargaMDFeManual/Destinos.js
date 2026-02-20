/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Configuracao.js" />
/// <reference path="DadosEmissao.js" />
/// <reference path="Geral.js" />
/// <reference path="Lacre.js" />
/// <reference path="LocaisPrestacao.js" />
/// <reference path="Observacao.js" />
/// <reference path="Passagem.js" />
/// <reference path="Percurso.js" />
/// <reference path="Rota.js" />
/// <reference path="Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="../Documentos/CTe.js" />
/// <reference path="../Documentos/MDFe.js" />
/// <reference path="../Documentos/NFS.js" />
/// <reference path="../Documentos/PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridReorder;
var _ordemDestino;

var OrdemDestino = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AlterarPercurso = PropertyEntity({ type: types.event, eventClick: AtualizarPercursoClick, text: "Atualizar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Destinos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0, text: "Percurso da Carga:" });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadDestinos() {
    _ordemDestino = new OrdemDestino();
    KoBindings(_ordemDestino, "tabDestinos");

    var headHtml = '<tr><th width="10%"></th><th width="90%">Destino</th>';
    _gridReorder = new GridReordering("Mova as linhas para reordenar os destinos", "reorderDestino", headHtml, "");
    _gridReorder.CarregarGrid();
    recarregarGridReorder();
}

//function percursoCargaClick(e) {



//    //var dados = { Carga: e.Codigo.val() };
//    //executarReST("Destino/BuscarRotaPorCarga", dados, function (arg) {
//    //    if (arg.Success) {
//    //        e.Destinos.list = new Array();
//    //        $.each(arg.Data, function (i, percurso) {
//    //            var percursoMap = new DestinoMap();
//    //            percursoMap.Codigo.val = percurso.Codigo;
//    //            percursoMap.Posicao.val = percurso.Posicao;
//    //            percursoMap.Destino.val = percurso.Destino.Descricao;
//    //            percursoMap.Destino.codEntity = percurso.Destino.Codigo;
//    //            e.Destinos.list.push(percursoMap);
//    //        });
//    //        reordenarPosicoesDestino(e);
//    //        recarregarGridReorder(e);
//    //        _cargaAtual = e;
//    //        _ordemDestino.Carga.val(e.Codigo.val());
//    //        if (arg.Data.length <= 1) {
//    //            _ordemDestino.AlterarPercurso.visible(false);
//    //        }
//    //    } else {
//    //        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
//    //    }
//    //})

//}

function AtualizarPercursoClick(e, sender) {
    reordenarPosicoesDestino();
    BuscarPercursoMDFe();
}

//*******MÉTODOS*******



function recarregarGridReorder() {

    var html = "";
    _cargaMDFeManual.ListaDestinos.val().sort(function (a, b) { return a.Posicao < b.Posicao });
    $.each(_cargaMDFeManual.ListaDestinos.val(), function (i, destino) {
        html += '<tr data-position="' + destino.Posicao + '" id="sort_' + destino.Codigo + '"><td>' + destino.Posicao + '</td>';
        html += '<td>' + destino.Descricao + '</td>';
        html += '</tr>';
    });
    _gridReorder.RecarregarGrid(html);
}


function reordenarPosicoesDestino() {
    var ListaOrdenada = _gridReorder.ObterOrdencao();
    var lista = new Array();
    $.each(_cargaMDFeManual.ListaDestinos.val(), function (i, percurso) {
        $.each(ListaOrdenada, function (i, ordem) {
            if (ordem.id.split("_")[1] == percurso.Codigo) {
                percurso.Posicao = ordem.posicao;
                lista.push(percurso);
            }
        });
    });
    _cargaMDFeManual.ListaDestinos.val(lista);
}