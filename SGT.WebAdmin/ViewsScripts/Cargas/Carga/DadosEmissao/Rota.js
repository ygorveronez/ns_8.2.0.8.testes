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
/// <reference path="Percurso.js" />
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

//*******MAPEAMENTO*******

var _cargaDadosEmissaoRota, _knoutPessoaParaRota;

var CargaDadosEmissaoRota = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.RotasFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid(), idBtnSearch: guid(), text: Localization.Resources.Cargas.Carga.AdicionarRotas });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaDadosEmissaoRota, type: types.event, text: Localization.Resources.Cargas.Carga.AtualizarRota, visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadCargaDadosEmissaoRota() {

    _knoutPessoaParaRota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    _cargaDadosEmissaoRota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable("") });
    _cargaDadosEmissaoRota = new CargaDadosEmissaoRota();

    KoBindings(_cargaDadosEmissaoRota, "tabRotas_" + _cargaAtual.DadosEmissaoFrete.id);
    $("#tabRotas_" + _cargaAtual.DadosEmissaoFrete.id + "_li").show();
    _cargaDadosEmissaoRota.Pedido.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    _cargaDadosEmissaoRota.Atualizar.enable(_cargaAtual.EtapaFreteEmbarcador.enable());
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirRotaFreteClick(_cargaDadosEmissaoRota.RotasFrete, data)
            }
        }]
    };

    if (!_cargaAtual.EtapaFreteEmbarcador.enable())
        menuOpcoes = null;

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarRota, _PermissoesPersonalizadasCarga)) {
        _cargaDadosEmissaoRota.Atualizar.enable(false);
        menuOpcoes = null;
    }
        
    var header = [{ data: "Codigo", visible: false },
                  { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }];

    _gridRotaFrete = new BasicDataTable(_cargaDadosEmissaoRota.RotasFrete.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    new BuscarRotasFrete(_cargaDadosEmissaoRota.RotasFrete, null, _gridRotaFrete, null, _knoutPessoaParaRota, _cargaAtual.Codigo.val());
    _cargaDadosEmissaoRota.RotasFrete.basicTable = _gridRotaFrete;

    recarregarGridRotaFrete();


}

function recarregarGridRotaFrete() {
    _gridRotaFrete.CarregarGrid(_cargaDadosEmissaoRota.RotasFrete.val());
}


function excluirRotaFreteClick(knoutRotaFrete, data) {
    var rotaFreteGrid = knoutRotaFrete.basicTable.BuscarRegistros();

    for (var i = 0; i < rotaFreteGrid.length; i++) {
        if (data.Codigo == rotaFreteGrid[i].Codigo) {
            rotaFreteGrid.splice(i, 1);
            break;
        }
    }
    knoutRotaFrete.basicTable.CarregarGrid(rotaFreteGrid);
}

function limparCamposRotaFrete() {
    LimparCampos(_cargaDadosEmissaoRota);
}

function preencherCargaDadosEmissaoRota(dados) {
    PreencherObjetoKnout(_cargaDadosEmissaoRota, dados);
    recarregarGridRotaFrete();
}

function atualizarCargaDadosEmissaoRota(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAtualizarRotaDaCarga, function () {
        var data = { Carga: e.Carga.val(), Pedido: e.Pedido.val(), RotasFrete: JSON.stringify(e.RotasFrete.basicTable.BuscarRegistros()) };
        executarReST("DadosEmissaoRota/AtualizarRota", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RotaAtualizadaComSucesso);

                    if (arg.Data.RetornoFrete != null)
                        preecherRetornoFrete(_cargaAtual, arg.Data.RetornoFrete);

                    obterDadosEmissaoGeralCarga();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}