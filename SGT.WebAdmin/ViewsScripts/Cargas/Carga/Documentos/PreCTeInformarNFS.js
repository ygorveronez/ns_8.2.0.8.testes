/// <reference path="PreCTe.js" />

var _preCTeInformarNFSCarga, _modalPreCTeInformarNFSCarga, _preCTeInformarNFSCarga;

var _opcoesArredondamentoISSNFSManual = [
    { text: "Normal", value: EnumTipoArredondamentoNFSManual.Normal },
    { text: "Para Cima", value: EnumTipoArredondamentoNFSManual.ParaCima },
    { text: "Para Baixo", value: EnumTipoArredondamentoNFSManual.ParaBaixo }
];

var PreCTeInformarNFSCarga = function () {
    this.CodigoCargaCTe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoPreCTe = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Numero = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: false, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: "Número:", enable: ko.observable(true), maxlength: 10 });
    this.Serie = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: true, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: "*Série:", issue: 756, enable: ko.observable(true), maxlength: 10 });
    this.ValorTotalFrete = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor Total do Frete:", enable: ko.observable(false) });
    this.ValorPrestacaoServico = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor Prestação do Serviço:", enable: ko.observable(false), eventChange: AtualizarBCISS });
    this.DataEmissao = PropertyEntity({ type: types.map, required: true, getType: typesKnockout.date, val: ko.observable("0,00"), def: "", text: "*Data Emissão:", enable: ko.observable(true) });
    this.AliquotaISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), maxlength: 5, def: "", text: "*Alíquota ISS:", enable: ko.observable(false), eventChange: AtualizarBCISS });
    this.BaseCalculo = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "*Base de Cálculo:", enable: ko.observable(false), eventChange: CalcularISS });
    this.ValorISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "*Valor ISS:", enable: ko.observable(false) });
    this.TipoArredondamentoISS = PropertyEntity({ text: "Arredondamento do ISS:", options: _opcoesArredondamentoISSNFSManual, val: ko.observable(EnumTipoArredondamentoNFSManual.Normal), def: EnumTipoArredondamentoNFSManual.Normal, issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), maxlength: 6, def: "0,00", text: "Percentual Retenção:", enable: ko.observable(true), eventChange: CalcularRetencao, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor Retenção:", enable: ko.observable(false), eventChange: CalcularRetencao, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorPIS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor PIS:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCOFINS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor COFINS:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorIR = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor IR:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCSLL = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor CSLL:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorReceber = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor a Receber:", enable: ko.observable(false), visible: ko.observable(true) });
    this.NumeroRPS = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: false, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: "*Número RPS:", enable: ko.observable(true) });

    this.IncluirValorBC = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Incluir o valor do ISS na BC", enable: ko.observable(true) });

    this.XMLAutorizacao = PropertyEntity({ type: types.file, val: ko.observable(""), text: "XML Autorização:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.XMLAutorizacao.val().replace('C:\\fakepath\\', '') }) });
    this.DANFSE = PropertyEntity({ type: types.file, val: ko.observable(""), text: "DANFSE:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.DANFSE.val().replace('C:\\fakepath\\', '') }) });

    //this.DownloadXML = PropertyEntity({ eventClick: downloadXMLClick, type: types.event, text: "XML", visible: ko.observable(true) });
    //this.DownloadDANFSE = PropertyEntity({ eventClick: downloadDANFSEClick, type: types.event, text: "DANFSE", visible: ko.observable(true) });

    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Observação:", enable: ko.observable(true) });

    this.Enviar = PropertyEntity({ eventClick: enviarPreCTeInformarNFSCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.EnviarNFS, icon: "fa fa-ban", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: fecharModalPreCTeInformarNFSCarga, type: types.event, text: Localization.Resources.Cargas.Carga.Fechar, icon: "fa fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function loadPreCTeInformarNFSCarga() {
    if (_preCTeInformarNFSCarga)
        return;

    //loadAnexosInformarNFS();

    _preCTeInformarNFSCarga = new PreCTeInformarNFSCarga();
    KoBindings(_preCTeInformarNFSCarga, "knockoutPreCTeInformarNFSCarga");

    _preCTeInformarNFSCarga.XMLAutorizacao.file = document.getElementById(_preCTeInformarNFSCarga.XMLAutorizacao.id);
    _preCTeInformarNFSCarga.DANFSE.file = document.getElementById(_preCTeInformarNFSCarga.DANFSE.id);

    _modalPreCTeInformarNFSCarga = new bootstrap.Modal(document.getElementById("divModalPreCTeInformarNFSCarga"), { backdrop: 'static', keyboard: true });
}

function enviarPreCTeInformarNFSCargaClick() {
    if (!ValidarCamposObrigatorios(_preCTeInformarNFSCarga)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.DesejaRealmenteProsseguir, function () {
        
        var anexos = [];
        if (_preCTeInformarNFSCarga.XMLAutorizacao.file.files.length > 0) {
            anexos.push({
                Tipo: "XML",
                Arquivo: _preCTeInformarNFSCarga.XMLAutorizacao.file.files[0]
            });
        }
        if (_preCTeInformarNFSCarga.DANFSE.file.files.length > 0) {
            anexos.push({
                Tipo: "DANFSE",
                Arquivo: _preCTeInformarNFSCarga.DANFSE.file.files[0]
            });
        }

        var dados = {
            CodigoCargaCTe: _preCTeInformarNFSCarga.CodigoCargaCTe.val(),
            CodigoPreCTe: _preCTeInformarNFSCarga.CodigoPreCTe.val(),
            Numero: _preCTeInformarNFSCarga.Numero.val(),
            Serie: _preCTeInformarNFSCarga.Serie.val(),
            DataEmissao: _preCTeInformarNFSCarga.DataEmissao.val(),
            AliquotaISS: _preCTeInformarNFSCarga.AliquotaISS.val(),
            BaseCalculo: _preCTeInformarNFSCarga.BaseCalculo.val(),
            ValorISS: _preCTeInformarNFSCarga.ValorISS.val(),
            PercentualRetencao: _preCTeInformarNFSCarga.PercentualRetencao.val(),
            ValorRetencao: _preCTeInformarNFSCarga.ValorRetencao.val(),
            ValorPIS: _preCTeInformarNFSCarga.ValorPIS.val(),
            ValorCOFINS: _preCTeInformarNFSCarga.ValorCOFINS.val(),
            ValorIR: _preCTeInformarNFSCarga.ValorIR.val(),
            ValorCSLL: _preCTeInformarNFSCarga.ValorCSLL.val(),
            ValorReceber: _preCTeInformarNFSCarga.ValorReceber.val(),
            NumeroRPS: _preCTeInformarNFSCarga.NumeroRPS.val(),
            Observacao: _preCTeInformarNFSCarga.Observacao.val()
        };

        var formData = new FormData();
        anexos.forEach(function (anexo) {
            formData.append("Tipo", anexo.Tipo);
            formData.append("Arquivo", anexo.Arquivo);
        });

        enviarArquivo("CargaPreCTe/EnviarNFSeParaPreCTe", dados, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.EnviadoComSucesso);

                    fecharModalPreCTeInformarNFSCarga();
                    limparCamposPreCTeInformarNFSCarga();

                    _gridCargaPreCTe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

////*******METODOS*******

function limparCamposPreCTeInformarNFSCarga() {
    LimparCampos(_preCTeInformarNFSCarga);
    _preCTeInformarNFSCarga.XMLAutorizacao.file.files = null;
    _preCTeInformarNFSCarga.DANFSE.file.files = null;
}

function abrirModalPreCTeInformarNFSCarga(preCte) {
    loadPreCTeInformarNFSCarga();

    limparCamposPreCTeInformarNFSCarga();

    _preCTeInformarNFSCarga.CodigoCargaCTe.val(preCte.Codigo);
    _preCTeInformarNFSCarga.CodigoPreCTe.val(preCte.CodigoPreCTE);

    _preCTeInformarNFSCarga.ValorTotalFrete.val(Globalize.format(preCte.ValorFrete, "n2"));
    _preCTeInformarNFSCarga.ValorPrestacaoServico.val(Globalize.format(preCte.ValorFrete, "n2"));
    _preCTeInformarNFSCarga.BaseCalculo.val(Globalize.format(preCte.ValorFrete, "n2"));

    _modalPreCTeInformarNFSCarga.show();
}

function fecharModalPreCTeInformarNFSCarga() {
    _modalPreCTeInformarNFSCarga.hide();
    limparCamposPreCTeInformarNFSCarga();
}

function AtualizarBCISS() {
    _preCTeInformarNFSCarga.BaseCalculo.val(_preCTeInformarNFSCarga.ValorPrestacaoServico.val());
    CalcularISS();
    CalcularValorAReceber();
}

function CalcularISS() {
    if (_preCTeInformarNFSCarga.BaseCalculo.val() != "" && _preCTeInformarNFSCarga.AliquotaISS.val() != "") {

        var aliquota = Globalize.parseFloat(_preCTeInformarNFSCarga.AliquotaISS.val());
        var baseCalculo = Globalize.parseFloat(_preCTeInformarNFSCarga.BaseCalculo.val());
        var incluirBC = _preCTeInformarNFSCarga.IncluirValorBC.val();

        var valorISS = baseCalculo * (aliquota / 100);

        if (incluirBC) {
            baseCalculo += (aliquota > 0 ? ((baseCalculo / ((100 - aliquota) / 100)) - baseCalculo) : 0);
            valorISS = baseCalculo * (aliquota / 100);
        }

        if (_preCTeInformarNFSCarga.TipoArredondamentoISS.val() == EnumTipoArredondamentoNFSManual.ParaCima)
            valorISS = ArredondarParaCima(valorISS, 2);
        else if (_preCTeInformarNFSCarga.TipoArredondamentoISS.val() == EnumTipoArredondamentoNFSManual.ParaBaixo)
            valorISS = ArredondarParaBaixo(valorISS, 2);

        _preCTeInformarNFSCarga.BaseCalculo.val(Globalize.format(baseCalculo, "n2"));
        _preCTeInformarNFSCarga.ValorISS.val(Globalize.format(valorISS, "n2"));

        CalcularRetencao();
    }
}

function CalcularRetencao() {
    if (_preCTeInformarNFSCarga.ValorISS.val() != "" && _preCTeInformarNFSCarga.PercentualRetencao.val() != _preCTeInformarNFSCarga.PercentualRetencao.def) {
        var percentualRetencao = Globalize.parseFloat(_preCTeInformarNFSCarga.PercentualRetencao.val());

        if (percentualRetencao <= 100) {
            var valorISS = Globalize.parseFloat(_preCTeInformarNFSCarga.ValorISS.val());
            var valorRetencao = valorISS * (percentualRetencao / 100);
            _preCTeInformarNFSCarga.ValorRetencao.val(Globalize.format(valorRetencao, "n2"));
        } else {
            _preCTeInformarNFSCarga.PercentualRetencao.val(_preCTeInformarNFSCarga.PercentualRetencao.def);
            _preCTeInformarNFSCarga.ValorRetencao.val(_preCTeInformarNFSCarga.ValorRetencao.def);
        }
    }
    else {
        _preCTeInformarNFSCarga.ValorRetencao.val("0,00");
    }
    CalcularValorAReceber();
}

function CalcularValorAReceber() {
    var valorAReceber = Globalize.parseFloat(_preCTeInformarNFSCarga.BaseCalculo.val());
    var valorPIS = Globalize.parseFloat(_preCTeInformarNFSCarga.ValorPIS.val());
    var valorCOFINS = Globalize.parseFloat(_preCTeInformarNFSCarga.ValorCOFINS.val());
    var valorIR = Globalize.parseFloat(_preCTeInformarNFSCarga.ValorIR.val());
    var valorCSLL = Globalize.parseFloat(_preCTeInformarNFSCarga.ValorCSLL.val());
    var valorRetencao = Globalize.parseFloat(_preCTeInformarNFSCarga.ValorRetencao.val());

    if (isNaN(valorAReceber))
        valorPIS = 0;
    if (isNaN(valorPIS))
        valorPIS = 0;
    if (isNaN(valorCOFINS))
        valorCOFINS = 0;
    if (isNaN(valorIR))
        valorIR = 0;
    if (isNaN(valorCSLL))
        valorCSLL = 0;
    if (isNaN(valorRetencao))
        valorRetencao = 0;

    valorAReceber -= valorPIS + valorCOFINS + valorIR + valorCSLL;

    if (_preCTeInformarNFSCarga.IncluirValorBC.val() || _CONFIGURACAO_TMS.ReduzirRetencaoISSValorAReceberNFSManual)
        valorAReceber -= valorRetencao;

    if (valorAReceber < 0)
        valorAReceber = 0;

    _preCTeInformarNFSCarga.ValorReceber.val(Globalize.format(valorAReceber, "n2"));
}

function ArredondarParaCima(num, precision) {
    precision = Math.pow(10, precision);
    return Math.ceil(num * precision) / precision;
}

function ArredondarParaBaixo(num, precision) {
    precision = Math.pow(10, precision);
    return Math.floor(num * precision) / precision;
}

//function downloadXMLClick(e) {
//    executarDownload("NFSManual/DownloadXML", { Codigo: e.CodigoPreCTe.val() });
//}

//function downloadDANFSEClick(e) {
//    executarDownload("NFSManual/DownloadDANFSE", { Codigo: e.CodigoPreCTe.val() });
//}
