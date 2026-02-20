/// <autosync enabled="true" />
/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
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
var _listaCargaPercursoMap;
var _ordemPercursoCarga;

var CargaPercursoMap = function () {
    this.Posicao = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.TipoRota = PropertyEntity({ val: 0, def: 0 });
    this.DescricaoTipoRota = PropertyEntity({ val: "", def: "", type: types.local });
    this.Destino = PropertyEntity({ val: "", def: "", codEntity: 0, defCodEntity: 0, type: types.entity });
    this.Origem = PropertyEntity({ val: "", def: "", codEntity: 0, defCodEntity: 0, type: types.entity });
    this.DistanciaKM = PropertyEntity({ val: 0, def: 0 });
}



var OrdemPercursoCarga = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AlterarPercurso = PropertyEntity({ type: types.event, eventClick: alterarPercursoClick, text: Localization.Resources.Cargas.Carga.AlterarPercursoDaCarga, visible: ko.observable(true), enable: ko.observable(true) });
    this.CargaPercursos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), defCodEntity: 0, text: Localization.Resources.Cargas.Carga.PercursoDaCarga.getFieldDescription() });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
}



//*******EVENTOS*******


function loadCargaPercurso() {
    $("#tabPercurso_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();

    _ordemPercursoCarga = new OrdemPercursoCarga();
    _ordemPercursoCarga.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());

    _ordemPercursoCarga.AlterarPercurso.enable(_cargaAtual.EtapaFreteEmbarcador.enable());

    KoBindings(_ordemPercursoCarga, "tabPercurso_" + _cargaAtual.DadosEmissaoFrete.id);

    $("#tabPercurso_" + _cargaAtual.DadosEmissaoFrete.id + "_li").unbind();
    $("#tabPercurso_" + _cargaAtual.DadosEmissaoFrete.id + "_li").click(function () {
        percursoCargaClick(_cargaAtual);
    })

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarPercurso, _PermissoesPersonalizadasCarga))
        _ordemPercursoCarga.AlterarPercurso.enable(false);
}

function percursoCargaClick(e) {
    var headHtml = '<tr><th width="6%"></th><th width="80%">' + Localization.Resources.Cargas.Carga.Destino + '<th width="20%">' + Localization.Resources.Gerais.Geral.Tipo ;
    _gridReorder = new GridReordering(Localization.Resources.Cargas.Carga.MovaAsLinhasParaReordenarPercursoDaCarga, "reorderPercursoCarga", headHtml, "");
    _gridReorder.CarregarGrid();


    var dados = { Carga: e.Codigo.val() };
    executarReST("CargaPercurso/BuscarRotaPorCarga", dados, function (arg) {
        if (arg.Success) {
            e.CargaPercursos.list = new Array();
            $.each(arg.Data, function (i, percurso) {
                var percursoMap = new CargaPercursoMap();
                percursoMap.Codigo.val = percurso.Codigo;
                percursoMap.DescricaoTipoRota.val = percurso.DescricaoTipoRota;
                percursoMap.TipoRota.val = percurso.TipoRota;
                percursoMap.Posicao.val = percurso.Posicao;
                percursoMap.Destino.val = percurso.Destino.Descricao;
                percursoMap.Destino.codEntity = percurso.Destino.Codigo;
                percursoMap.Origem.val = percurso.Origem.Descricao;
                percursoMap.Origem.codEntity = percurso.Origem.Codigo;
                percursoMap.DistanciaKM.val = percurso.DistanciaKM;
                e.CargaPercursos.list.push(percursoMap);
            });
            reordenarPosicoesPercursoCarga(e);
            recarregarGridReorder(e);
            _cargaAtual = e;
            _ordemPercursoCarga.Carga.val(e.Codigo.val());
            if (arg.Data.length <= 1) {
                _ordemPercursoCarga.AlterarPercurso.visible(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    })

}

function alterarPercursoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.AoAlterarOrdemDoPercursoValorDoFreteSeuRateioPodemSerModificadosRealmenteDesejaAlterar, function () {
        reordenarPosicoesPercursoCarga(_cargaAtual);
        e.CargaPercursos.list = _cargaAtual.CargaPercursos.list;
        Salvar(e, "CargaPercurso/AlterarPercursoCarga", function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    $("#" + _cargaAtual.DivCarga.id + "_ribbonCargaNova").hide();
                    preecherRetornoFrete(_cargaAtual, arg.Data);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    })
}

//*******MÉTODOS*******



function recarregarGridReorder(e) {

    var html = "";
    e.CargaPercursos.list.sort(function (a, b) { return a.Posicao.val < b.Posicao.val });
    $.each(e.CargaPercursos.list, function (i, percurso) {
        html += '<tr data-position="' + percurso.Posicao.val + '" id="sort_' + percurso.Codigo.val + '"><td>' + percurso.Posicao.val + '</td>';
        html += '<td>' + percurso.Destino.val + '</td>';
        html += '<td>' + percurso.DescricaoTipoRota.val + '</td>';
        html += '</tr>';
    });
    _gridReorder.RecarregarGrid(html);
}


function reordenarPosicoesPercursoCarga(e) {
    var ListaOrdenada = _gridReorder.ObterOrdencao();
    $.each(e.CargaPercursos.list, function (i, percurso) {
        $.each(ListaOrdenada, function (i, ordem) {
            if (ordem.id.split("_")[1] == percurso.Codigo.val) {
                percurso.Posicao.val = ordem.posicao;
            }
        });
    });
}