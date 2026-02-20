var _dadosEntrega;
var _mapDadosEntrega

/*
 * Declaração das Funções Associadas a Eventos
 */

var DadosEntrega = function (id) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroCarga.getFieldDescription() });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Destinatario.getFieldDescription() });
    this.Localidade = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Localidade.getFieldDescription() });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SituacaoEntrega.getFieldDescription() });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaEntrega.getFieldDescription(), visible: ko.observable(false) });
    this.DataEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataDaEntrega.getFieldDescription(), visible: ko.observable(false) });
    this.DataRejeitado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataDeRejeicao.getFieldDescription(), visible: ko.observable(false) });
    this.Avaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.AvaliacaoEntrega.getFieldDescription(), estrelas: [3, 2, 1], val: ko.observable(0), def: 0 });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription() });
    this.JanelaDescarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.JanelaDeDescarga.getFieldDescription(), visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais.getFieldDescription() });

    this.Transportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Transportador.getFieldDescription(), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Veiculo.getFieldDescription(), visible: ko.observable(false)  });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ModeloDoVeiculo.getFieldDescription(), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Motorista.getFieldDescription() });

    this.LocalEntrega = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.ControleEntrega.LocalDaEntrega.getFieldDescription() });
    this.Imagens = PropertyEntity({ val: ko.observableArray([{ Codigo: 0, Miniatura: "" }]), def: [] });
    this.ImagensCanhoto = PropertyEntity({ val: ko.observableArray([{ Codigo: 0, Miniatura: "" }]), def: [] });
    this.DownloadCanhoto = PropertyEntity({ eventClick: downloadCanhotoClick, type: types.event });
    
    this.Avaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Avaliacao.getFieldDescription(), visible: ko.observable(false) });
    this.DataAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Data.getFieldDescription() });
    this.ObservacaoAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacao.getFieldDescription() });
    this.DadosEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DadosEntrega });
    this.Detalhes = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Detalhes });
    this.Fotos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Fotos });
    this.LocalColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LocalColeta });
    this.Canhotos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Canhotos });

    this.knockoutId = PropertyEntity({ val: "knockout-dados-Entrega-" + id });
}


function carregarHTMLComponenteAcompanhamentoEntrega(callback) {
    $.get('Content/Static/Entrega/AcompanhamentoEntrega/ComponenteAcompanhamentoEntrega.html', function (html) {
        $('#ComponenteAcompanhamentoEntregaContent').html(html);
        callback();

    })
}

function RegistraComponenteAcompanhamentoEntrega() {
    if (ko.components.isRegistered('acompanhamento-Entrega'))
        return;

    ko.components.register('acompanhamento-Entrega', {
        viewModel: EtapaAcompanhamentoEntrega,
        template: {
            element: 'acompanhamento-Entrega-templete'
        }
    });
}


function loadDadosEntrega(e) {
    var knockout = "knockout-dados-Entrega-" + e.EtapaEntrega.idTab

    if (!findListaKnockout(knockout)) {
        _listaKnockout.push(knockout);
        _dadosEntrega = new DadosEntrega(e.EtapaEntrega.idTab);
        KoBindings(_dadosEntrega, knockout);
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */


/*
 * Declaração das Funções
 */
function exibirDadosEtapaEntrega(e) {
    loadDadosEntrega(e);

    fecharDados();



    executarReST("AcompanhamentoPedido/ObterDadosEntrega", { CodigoPedido: e.CodigoPedido.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_dadosEntrega, { Data: arg.Data });

                if (_dadosEntrega.DataEntrega.val() != "")
                    _dadosEntrega.DataEntrega.visible(true);
                else
                    _dadosEntrega.DataEntrega.visible(false);

                if (_dadosEntrega.DataRejeitado.val() != "")
                    _dadosEntrega.DataRejeitado.visible(true);
                else
                    _dadosEntrega.DataRejeitado.visible(false);

                if (_dadosEntrega.JanelaDescarga.val() != "")
                    _dadosEntrega.JanelaDescarga.visible(true);
                else
                    _dadosEntrega.JanelaDescarga.visible(false);

                if (_dadosEntrega.Transportador.val() != "")
                    _dadosEntrega.Transportador.visible(true);
                else
                    _dadosEntrega.Transportador.visible(false);

                if (_dadosEntrega.Veiculo.val() != "")
                    _dadosEntrega.Veiculo.visible(true);
                else
                    _dadosEntrega.Veiculo.visible(false);

                if (_dadosEntrega.ModeloVeiculo.val() != "")
                    _dadosEntrega.ModeloVeiculo.visible(true);
                else
                    _dadosEntrega.ModeloVeiculo.visible(false);

                if (arg.Data.AvaliacaoEfetuada) {
                    _dadosEntrega.Avaliacao.visible(true);
                } else {
                    _dadosEntrega.Avaliacao.visible(false);
                }

                var positionEntrega = {
                    lat: arg.Data.LocalEntrega.Latitude,
                    lng: arg.Data.LocalEntrega.Longitude,
                };

                SetarCoordenadasDadosEntrega(positionEntrega);


            } else {
                //Sem pedido
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

    $("#" + e.EtapaEntrega.idTab).addClass("active show");
}

function SetarCoordenadasDadosEntrega(positionEntrega) {

    setTimeout(function () {

        CarregarMapaDadosEntrega();

        var marker = new ShapeMarker();
        marker.setPosition(positionEntrega.lat, positionEntrega.lng);
        marker.title = '';

        _mapDadosEntrega.draw.addShape(marker);
        _mapDadosEntrega.direction.setZoom(17);
        _mapDadosEntrega.direction.centralizar(positionEntrega.lat, positionEntrega.lng);

    }, 500);
}


function CarregarMapaDadosEntrega() {
    opcoesMapa = new OpcoesMapa(false, false);

    _mapDadosEntrega = new MapaGoogle("map-local-entrega" + _dadosEntrega.knockoutId.val, false, opcoesMapa);

}

function downloadCanhotoClick(e) {
    if (e.Codigo > 0 && e.GuidNomeArquivo != "") {
        var dados = {
            Codigo: e.Codigo
        }
        executarDownload("Canhoto/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Canhotos.Canhoto.CanhotoNaoEnviado, Localization.Resources.Canhotos.Canhoto.NaoFoiEnviadoCanhotoParaEstaNota);
    }
}

