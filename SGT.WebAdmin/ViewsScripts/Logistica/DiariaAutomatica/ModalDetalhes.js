
var _detalhesDiariaAutomatica;
var _dadosCarga;

var DetalhesDiariaAutomatica = function () {
    this.LocalFreeTime = PropertyEntity({ text: "Tipo Free Time: " });
    this.ValorDiaria = PropertyEntity({ text: "Valor (R$): " });
    this.ComposicaoFrete = PropertyEntity({ text: "Composição do valor" });

    this.Chamado = PropertyEntity({ text: "Chamado: " });
    this.DataInicioCobranca = PropertyEntity({ text: "Início da cobrança: " });
    this.DataUltimaAtualizacao = PropertyEntity({ text: "Última atualização: " });
}

var DadosCarga = function () {
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga: " });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, visible: ko.observable(true), enable: ko.observable(true), text: ko.observable("*Modelo Veicular:"), tooltipTitle: ko.observable("Modelo Veicular:")});
    this.TipoOperacao = PropertyEntity({ text: ko.observable("Tipo de Operação:"), visible: ko.observable(true), });
    this.DataCarregamento = PropertyEntity({ type: types.map, required: false, visible: ko.observable(true) });

    this.ValorFrete = PropertyEntity({ text: "Valor Frete: ", required: false, idGrid: guid(), visible: ko.observable(true) });
    this.Peso = PropertyEntity({ text: "Peso:" });
    this.ValorMercadoria = PropertyEntity({ text: "Valor NF: ", visible: ko.observable(true) });
    this.DescricaoSituacaoCarga = PropertyEntity({ text: "Situação: ", type: types.map });

    this.Operador = PropertyEntity({ text: "Operador: ", visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ tooltipTitle: ko.observable("Transportador:") });

    this.Veiculo = PropertyEntity({ tooltipTitle: ko.observable("Placa(s):") });
    this.Motorista = PropertyEntity({ tooltipTitle: ko.observable("Motorista:") });

    // Complementares
    this.CargaDeComplemento = PropertyEntity({ getType: typesKnockout.bool });
}

function openModalDetalhes(codigoDiariaAutomatica, codigoCarga) {
    $.get("Content/Static/Logistica/DiariaAutomatica/Modal.html?dyn=" + guid(), function (data) {
        $("#modais").html(data);

        _detalhesDiariaAutomatica = new DetalhesDiariaAutomatica();
        KoBindings(_detalhesDiariaAutomatica, "knockoutDetalhesDiariaAutomatica");

        _dadosCarga = new DadosCarga();
        KoBindings(_dadosCarga, "knockoutDadosCarga");

        popularDetalhesDiariaAutomatica(codigoDiariaAutomatica);
        popularDadosCarga(codigoCarga);

        Global.abrirModal('divModalDetalhesDiariaAutomatica');
        
    });
}

async function popularDetalhesDiariaAutomatica(codigoDiariaAutomatica) {
    let detalhes = await getDetalhesDiariaAutomatica(codigoDiariaAutomatica);
    PreencherObjetoKnout(_detalhesDiariaAutomatica, detalhes);
}

async function popularDadosCarga(codigoCarga) {
    let dados = await getDadosCarga(codigoCarga);
    PreencherObjetoKnout(_dadosCarga, dados);
}

function getDetalhesDiariaAutomatica(codigoDiariaAutomatica) {
    return new Promise((resolve) => {
        executarReST("DiariaAutomatica/BuscarPorCodigo", { Codigo: codigoDiariaAutomatica }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    resolve(retorno);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }, null);
    });
}

function getDadosCarga(codigoCarga) {
    return new Promise((resolve) => {
        executarReST("Carga/BuscarPorCodigo", { Codigo: codigoCarga }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    resolve(retorno);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        }, null);
    });
}
