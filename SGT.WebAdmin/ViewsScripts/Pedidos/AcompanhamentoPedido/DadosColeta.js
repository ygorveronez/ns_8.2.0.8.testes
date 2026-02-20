var _dadosColeta;
var _mapDadosColeta

/*
 * Declaração das Funções Associadas a Eventos
 */

var DadosColeta = function (id) {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true) });
    this.Programacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Programacao.getFieldDescription() });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Transportador.getFieldDescription() });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Veiculo.getFieldDescription() });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ModeloDoVeiculo.getFieldDescription() });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription() });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SituacaoColeta.getFieldDescription() });
    this.DadosColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DadosColeta });
    this.TextoInformacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.VeiculoAguardandoCarregamento.getFieldDescription()) });;
    this.Detalhes = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Detalhes });
    this.Fotos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Fotos });
    this.Informacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Informacao });

    this.LocalColeta = PropertyEntity({ type: types.local, text: Localization.Resources.Cargas.ControleEntrega.LocalColeta });
    this.Imagens = PropertyEntity({ val: ko.observableArray([{ Codigo: 0, Miniatura: "" }]), def: [] });


    this.Lacres = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Lacres.getRequiredFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });

    this.Salvar = PropertyEntity({ eventClick: salvarColetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar.getFieldDescription(), visible: ko.observable(true) });

    this.knockoutId = PropertyEntity({ val: "knockout-dados-Coleta-" + id });
}


function carregarHTMLComponenteAcompanhamentoColeta(callback) {
    $.get('Content/Static/Coleta/AcompanhamentoColeta/ComponenteAcompanhamentoColeta.html', function (html) {
        $('#ComponenteAcompanhamentoColetaContent').html(html);
        callback();

    })
}

function RegistraComponenteAcompanhamentoColeta() {
    if (ko.components.isRegistered('acompanhamento-Coleta'))
        return;

    ko.components.register('acompanhamento-Coleta', {
        viewModel: EtapaAcompanhamentoColeta,
        template: {
            element: 'acompanhamento-Coleta-templete'
        }
    });
}

function loadDadosColeta(e) {

    var knockout = "knockout-dados-Coleta-" + e.EtapaColeta.idTab

    if (!findListaKnockout(knockout)) {
        _listaKnockout.push(knockout);
        _dadosColeta = new DadosColeta(e.EtapaColeta.idTab);
        KoBindings(_dadosColeta, knockout);
    }

}

/*
 * Declaração das Funções Associadas a Eventos
 */


/*
 * Declaração das Funções
 */
function exibirDadosEtapaColeta(e) {
    loadDadosColeta(e);
    fecharDados();

    // Se a etapa de Transporte estiver já ativa, muda o texto informativo
    if (e.EtapaTransporte.stepClass == 'green' || e.EtapaTransporte.stepClass == 'yellow') {
        _dadosColeta.TextoInformacao.text(Localization.Resources.Cargas.ControleEntrega.VeiculoCarregado);
    }

    executarReST("AcompanhamentoPedido/ObterDadosColeta", { CodigoPedido: e.CodigoPedido.val() }, function (arg) {

        if (arg.Success) {
            if (arg.Data !== false) {
                _dadosColeta.CodigoPedido.visible(true);
                PreencherObjetoKnout(_dadosColeta, { Data: arg.Data });
                var positionColeta = {
                    lat: arg.Data.LocalColeta.Latitude,
                    lng: arg.Data.LocalColeta.Longitude,
                };
                SetarCoordenadasDadosColeta(positionColeta);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                _dadosColeta.CodigoPedido.visible(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

    $("#" + e.EtapaColeta.idTab).addClass("active show");
}

function SetarCoordenadasDadosColeta(positionColeta) {

    setTimeout(function () {

        CarregarMapaDadosColeta();

        var marker = new ShapeMarker();
        marker.setPosition(positionColeta.lat, positionColeta.lng);
        marker.title = '';

        _mapDadosColeta.draw.addShape(marker);
        _mapDadosColeta.direction.setZoom(17);
        _mapDadosColeta.direction.centralizar(positionColeta.lat, positionColeta.lng);

    }, 500);
}


function CarregarMapaDadosColeta() {
    opcoesMapa = new OpcoesMapa(false, false);

    _mapDadosColeta = new MapaGoogle("map-local-coleta" + _dadosColeta.knockoutId.val, false, opcoesMapa);

}


function salvarColetaClick(e, sender) {
    ValidarCamposObrigatorios(_dadosColeta);

    var dados = { CodigoPedido: _dadosColeta.CodigoPedido.val(), Lacres: _dadosColeta.Lacres.val(), Observacao: _dadosColeta.Observacao.val() }

    executarReST("AcompanhamentoPedido/SalvarDadosColeta", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);


            } else {
                //Sem pedido
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}