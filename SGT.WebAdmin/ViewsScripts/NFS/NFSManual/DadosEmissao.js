/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _opcoesArredondamentoISSNFSManual = [
    { text: "Normal", value: EnumTipoArredondamentoNFSManual.Normal },
    { text: "Para Cima", value: EnumTipoArredondamentoNFSManual.ParaCima },
    { text: "Para Baixo", value: EnumTipoArredondamentoNFSManual.ParaBaixo }
];

var _dadosEmissao;
var _gridLancamentoNFSManualDesconto;

var DadosEmissao = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ValorTotalMoeda = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor Total em Moeda:", enable: ko.observable(false), visible: ko.observable(false) });
    this.Moeda = PropertyEntity({ text: "Moeda:", options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real, enable: ko.observable(false), visible: ko.observable(false) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(false), enable: ko.observable(false) });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: ko.observable("*Transportador:"), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), required: ko.observable(false) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "*Filial:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), required: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "*Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), required: ko.observable(false) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: "*Tomador:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(false), required: ko.observable(false) });

    this.ValorTotalFreteBruto = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor Total do Frete Bruto:", enable: false, visible: ko.observable(false) });
    this.Descontos = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Descontos:", enable: false, visible: ko.observable(false), popover: "<strong>Clique aqui para visualizar os detalhes</strong>", detalhesClick: exibirDetalhesDescontosClick });
    this.ValorTotalFrete = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor Total do Frete:", enable: ko.observable(false) });
    this.Numero = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: false, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: "Número:", enable: ko.observable(true), maxlength: 10 });
    this.Serie = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: true, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: "*Série:", issue: 756, enable: ko.observable(true), maxlength: 10 });
    this.ValorPrestacaoServico = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor Prestação do Serviço:", enable: ko.observable(false), eventChange: AtualizarBCISS });
    this.DataEmissao = PropertyEntity({ type: types.map, required: true, getType: typesKnockout.date, val: ko.observable("0,00"), def: "", text: "*Data Emissão:", enable: ko.observable(true) });
    this.AliquotaISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), maxlength: 5, def: "", text: "*Alíquota ISS:", enable: ko.observable(false), eventChange: AtualizarBCISS });
    this.ValorISS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "*Valor ISS:", enable: ko.observable(false) });
    this.BaseCalculo = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "*Base de Cálculo:", enable: ko.observable(false), eventChange: CalcularISS });
    this.PercentualRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), maxlength: 6, def: "0,00", text: "Percentual Retenção:", enable: ko.observable(true), eventChange: CalcularRetencao, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorRetencao = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor Retenção:", enable: ko.observable(false), eventChange: CalcularRetencao, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.IncluirValorBC = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true, text: "Incluir o valor do ISS na BC", enable: ko.observable(true) });
    this.ConsiderarLocalidadeCarga = PropertyEntity({ type: types.map, getType: typesKnockout.bool, def: false, text: "Considerar localidade da carga", enable: ko.observable(true), visible: ko.observable(false) });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Observação:", enable: ko.observable(true), maxlength: 300 });
    this.TipoArredondamentoISS = PropertyEntity({ text: "Arredondamento do ISS:", options: _opcoesArredondamentoISSNFSManual, val: ko.observable(EnumTipoArredondamentoNFSManual.Normal), def: EnumTipoArredondamentoNFSManual.Normal, issue: 0, visible: ko.observable(true), enable: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.ValorPIS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor Ret. PIS:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCOFINS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor Ret. COFINS:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, valueUpdate: "afterkeydown", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorPISIBSCBS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor PIS:", enable: ko.observable(true), visible: ko.observable(true), eventChange: function () { CalcularBaseCalculoIBSCBS(); CalcularIBS(); CalcularCBS() }, valueUpdate: "afterkeydown", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCOFINSIBSCBS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor COFINS:", enable: ko.observable(true), visible: ko.observable(true), eventChange: function () { CalcularBaseCalculoIBSCBS(); CalcularIBS(); CalcularCBS() }, valueUpdate: "afterkeydown", configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorIR = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor IR:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCSLL = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor CSLL:", enable: ko.observable(true), visible: ko.observable(true), eventChange: CalcularValorAReceber, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorReceber = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor a Receber:", enable: ko.observable(false), visible: ko.observable(true) });
    this.BaseCalculoIBSCBS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Base de cálculo IBS e CBS", enable: ko.observable(false), visible: ko.observable(true), eventChange: function () { CalcularIBS(); CalcularCBS() }, valueUpdate: "afterkeydown" });
    this.AliquotaIBSEstadual = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Aliquota IBS Estadual:", enable: ko.observable(false), visible: ko.observable(true), eventChange: CalcularIBS, valueUpdate: "afterkeydown" });
    this.AliquotaIBSMunicipal = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Aliquota IBS Municipal:", enable: ko.observable(false), visible: ko.observable(true), eventChange: CalcularIBS, valueUpdate: "afterkeydown" }); 
    this.ValorIBSEstadual = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor IBS Estadual:", enable: ko.observable(false), visible: ko.observable(true) });
    this.ValorIBSMunicipal = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor IBS Municipal:", enable: ko.observable(false), visible: ko.observable(true) });
    this.AliquotaCBS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Aliquota CBS:", enable: ko.observable(false), visible: ko.observable(true), eventChange: CalcularCBS, valueUpdate: "afterkeydown" });
    this.ValorCBS = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "0,00", text: "Valor CBS:", enable: ko.observable(false), visible: ko.observable(true) });
    this.CSTIBSCBS = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "CST IBS e CBS:", enable: ko.observable(false), visible: ko.observable(true), maxlength: 5 });
    this.ClassificacaoTributariaIBSCBS = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Classificação tributária IBS e CBS:", enable: ko.observable(false), visible: ko.observable(true), maxlength: 8 });
    this.NBS = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "NBS:", enable: ko.observable(false), visible: ko.observable(true), maxlength: 9 });
    this.IndicadorOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Indicador Operação:", enable: ko.observable(false), visible: ko.observable(true), maxlength: 6 });
    this.NumeroRPS = PropertyEntity({ type: types.map, configInt: { precision: 0, allowZero: false, thousands: '' }, required: false, getType: typesKnockout.int, val: ko.observable("0"), def: "", text: "*Número RPS:", enable: ko.observable(false) });
    this.ServicoNFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço NFSe:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true), required: ko.observable(false) });

    this.XMLAutorizacao = PropertyEntity({ type: types.file, val: ko.observable(""), text: "XML Autorização:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.XMLAutorizacao.val().replace('C:\\fakepath\\', '') }) });
    this.DANFSE = PropertyEntity({ type: types.file, val: ko.observable(""), text: "DANFSE:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.DANFSE.val().replace('C:\\fakepath\\', '') }) });
    this.Anexo = PropertyEntity({ type: types.file, val: ko.observable(""), text: "Anexo:", visible: ko.observable(false), enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.Anexo.val().replace('C:\\fakepath\\', '') }) });

    this.ImportarEDI = PropertyEntity({ eventClick: ImportarEDIClick, type: types.event, text: "Importar EDI", visible: ko.observable(false) });
    this.EDI = PropertyEntity({ type: types.file, eventChange: EnviarEDI, codEntity: ko.observable(0), text: "EDI:", val: ko.observable(""), visible: ko.observable(false) });
    this.GerarRPS = PropertyEntity({ eventClick: GerarRPSClick, type: types.event, text: "Gerar RPS", visible: ko.observable(false) });
    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Emitir = PropertyEntity({ eventClick: emitirClick, type: types.event, text: "Finalizar", visible: ko.observable(false) });

    this.DownloadXML = PropertyEntity({ eventClick: downloadXMLClick, type: types.event, text: "XML", visible: ko.observable(true) });
    this.DownloadDANFSE = PropertyEntity({ eventClick: downloadDANFSEClick, type: types.event, text: "DANFSE", visible: ko.observable(true) });
    this.DownloadAnexo = PropertyEntity({ eventClick: downloadAnexoClick, type: types.event, text: "Anexo", visible: ko.observable(true) });

    this.TipoArredondamentoISS.val.subscribe(function () {
        AtualizarBCISS();
    });
};

//*******EVENTOS*******
function loadDadosEmissao() {
    _dadosEmissao = new DadosEmissao();
    KoBindings(_dadosEmissao, "knockoutDadosEmissao");

    $("#" + _dadosEmissao.IncluirValorBC.id).click(AtualizarBCISS);

    _dadosEmissao.XMLAutorizacao.file = document.getElementById(_dadosEmissao.XMLAutorizacao.id);
    _dadosEmissao.DANFSE.file = document.getElementById(_dadosEmissao.DANFSE.id);
    _dadosEmissao.Anexo.file = document.getElementById(_dadosEmissao.Anexo.id);

    new BuscarFilial(_dadosEmissao.Filial);
    new BuscarTiposOperacao(_dadosEmissao.TipoOperacao);
    new BuscarTransportadores(_dadosEmissao.Transportador, callbackTransportadores);
    new BuscarClientes(_dadosEmissao.Tomador);

    new BuscarLocalidades(_dadosEmissao.LocalidadePrestacao);
    new BuscarServicoNFSe(_dadosEmissao.ServicoNFSe, null, obterServicoAliquotaRetencao);
    
    if (_CONFIGURACAO_TMS.EmitirNFSManualParaTransportadorEFiliais) {
        _dadosEmissao.LocalidadePrestacao.visible(true);
        _dadosEmissao.LocalidadePrestacao.required = true;
    }
        

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros)
        _dadosEmissao.Anexo.visible(true);

    if (_CONFIGURACAO_TMS.UtilizarLocalidadeTomadorNFSManual)
        _dadosEmissao.ConsiderarLocalidadeCarga.visible(true);

    new BuscarModeloDocumentoFiscal(_dadosEmissao.ModeloDocumentoFiscal, null, null, false, true, false, false);

    $('#' + _dadosEmissao.Descontos.id + '-detalhes').popover();

    loadGridLancamentoNFSManualDesconto();
}
function callbackTransportadores(retorno) {
    _dadosEmissao.Transportador.codEntity(retorno.Codigo);
    _dadosEmissao.Transportador.val(retorno.Descricao);
    obterServicoAliquotaRetencao();
}
function loadGridLancamentoNFSManualDesconto() {
    _gridLancamentoNFSManualDesconto = new GridView("grid-lancamento-nfs-manual-desconto", "NFSManual/PesquisaDescontos", _dadosEmissao);
}

function downloadXMLClick(e) {
    executarDownload("NFSManual/DownloadXML", { Codigo: e.Codigo.val() });
}

function downloadDANFSEClick(e) {
    executarDownload("NFSManual/DownloadDANFSE", { Codigo: e.Codigo.val() });
}

function downloadAnexoClick(e) {
    executarDownload("NFSManual/DownloadAnexo", { Codigo: e.Codigo.val() });
}

function GerarRPSClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_dadosEmissao);
    if (valido) {
        exibirConfirmacao("Gerar RPS", "Você tem certeza que deseja gerar o arquivo de RPS?", function () {
            var dados = {
                Codigo: _dadosEmissao.Codigo.val()
            };
            executarDownload("NFSManual/GerarArquivoRPS", dados, null, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                } else
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            });
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function emitirClick(e, sender) {
    exibirConfirmacao("Emitir NFS", "Você tem certeza que deseja enviar para aprovação a NFS?", function () {
        executarReST("NFSManual/Emitir", { Codigo: _dadosEmissao.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (arg.Data.PossuiRegra)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS está aguardando aprovação.");
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a NFS.");
                    BuscarNFSPorCodigo(arg.Data.Codigo, null, true);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function salvarClick(e, sender) {
    Salvar(_dadosEmissao, "NFSManual/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "NFS criada com sucesso");

                // Envia arquivos
                var anexos = [];
                if (_dadosEmissao.XMLAutorizacao.file.files.length > 0) {
                    anexos.push({
                        Tipo: "XML",
                        Arquivo: _dadosEmissao.XMLAutorizacao.file.files[0]
                    });
                }
                if (_dadosEmissao.DANFSE.file.files.length > 0) {
                    anexos.push({
                        Tipo: "DANFSE",
                        Arquivo: _dadosEmissao.DANFSE.file.files[0]
                    });
                }
                if (_dadosEmissao.Anexo.file.files.length > 0) {
                    anexos.push({
                        Tipo: "Anexo",
                        Arquivo: _dadosEmissao.Anexo.file.files[0]
                    });
                }
                if (anexos.length > 0)
                    AnexarArquivos(anexos);

                _dadosEmissao.Emitir.visible(true);
                if (_nfsManual.ContemEDI.val() === true) {
                    _dadosEmissao.GerarRPS.visible(true);
                    _dadosEmissao.ImportarEDI.visible(true);
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function exibirDetalhesDescontosClick() {
    Global.abrirModal('divModalDadosEmissaoDescontos');
}

//*******MÉTODOS*******
function AtualizarBCISS() {
    _dadosEmissao.BaseCalculo.val(_dadosEmissao.ValorPrestacaoServico.val());
    CalcularISS();
    CalcularValorAReceber();
}

function CalcularISS() {
    if (_dadosEmissao.BaseCalculo.val() != "" && _dadosEmissao.AliquotaISS.val() != "") {

        var aliquota = Globalize.parseFloat(_dadosEmissao.AliquotaISS.val());
        var baseCalculo = Globalize.parseFloat(_dadosEmissao.BaseCalculo.val());
        var incluirBC = _dadosEmissao.IncluirValorBC.val();

        var valorISS = baseCalculo * (aliquota / 100);

        if (incluirBC) {
            baseCalculo += (aliquota > 0 ? ((baseCalculo / ((100 - aliquota) / 100)) - baseCalculo) : 0);
            valorISS = baseCalculo * (aliquota / 100);
        }

        if (_dadosEmissao.TipoArredondamentoISS.val() == EnumTipoArredondamentoNFSManual.ParaCima)
            valorISS = ArredondarParaCima(valorISS, 2);
        else if (_dadosEmissao.TipoArredondamentoISS.val() == EnumTipoArredondamentoNFSManual.ParaBaixo)
            valorISS = ArredondarParaBaixo(valorISS, 2);

        _dadosEmissao.BaseCalculo.val(Globalize.format(baseCalculo, "n2"));
        _dadosEmissao.ValorISS.val(Globalize.format(valorISS, "n2"));

        CalcularRetencao();
    }
}
function CalcularBaseCalculoIBSCBS() {
    if (_dadosEmissao.ValorPrestacaoServico.val() != "" && _dadosEmissao.ValorPISIBSCBS.val() != "" && _dadosEmissao.ValorCOFINSIBSCBS.val() != "") {

        var valorPrestacao = Globalize.parseFloat(_dadosEmissao.ValorPrestacaoServico.val());
        var valorPIS = Globalize.parseFloat(_dadosEmissao.ValorPISIBSCBS.val());
        var valorCOFINS = Globalize.parseFloat(_dadosEmissao.ValorCOFINSIBSCBS.val());

        _dadosEmissao.BaseCalculoIBSCBS.val(Globalize.format(valorPrestacao - valorPIS - valorCOFINS, "n2"));
    }
}

function CalcularIBS() {
    if (_dadosEmissao.BaseCalculoIBSCBS.val() != "" && _dadosEmissao.AliquotaIBSEstadual.val() != "") {

        var aliquota = Globalize.parseFloat(_dadosEmissao.AliquotaIBSEstadual.val());
        var baseCalculo = Globalize.parseFloat(_dadosEmissao.BaseCalculoIBSCBS.val());

        _dadosEmissao.ValorIBSEstadual.val(Globalize.format(baseCalculo * (aliquota / 100), "n2"));
    }

    if (_dadosEmissao.BaseCalculoIBSCBS.val() != "" && _dadosEmissao.AliquotaIBSMunicipal.val() != "") {

        var aliquota = Globalize.parseFloat(_dadosEmissao.AliquotaIBSMunicipal.val());
        var baseCalculo = Globalize.parseFloat(_dadosEmissao.BaseCalculoIBSCBS.val());

        _dadosEmissao.ValorIBSMunicipal.val(Globalize.format(baseCalculo * (aliquota / 100), "n2"));
    }
}

function CalcularCBS() {
    if (_dadosEmissao.BaseCalculoIBSCBS.val() != "" && _dadosEmissao.AliquotaCBS.val() != "") {

        var aliquota = Globalize.parseFloat(_dadosEmissao.AliquotaCBS.val());
        var baseCalculo = Globalize.parseFloat(_dadosEmissao.BaseCalculoIBSCBS.val());

        _dadosEmissao.ValorCBS.val(Globalize.format(baseCalculo * (aliquota / 100), "n2"));
    }
}

function CalcularValorAReceber() {
    //var valorAReceber = Globalize.parseFloat(_dadosEmissao.ValorPrestacaoServico.val());
    //if (_CONFIGURACAO_TMS.ReduzirRetencaoISSValorAReceberNFSManual)
    var valorAReceber = Globalize.parseFloat(_dadosEmissao.BaseCalculo.val());
    var valorPIS = Globalize.parseFloat(_dadosEmissao.ValorPIS.val());
    var valorCOFINS = Globalize.parseFloat(_dadosEmissao.ValorCOFINS.val());
    var valorIR = Globalize.parseFloat(_dadosEmissao.ValorIR.val());
    var valorCSLL = Globalize.parseFloat(_dadosEmissao.ValorCSLL.val());
    var valorRetencao = Globalize.parseFloat(_dadosEmissao.ValorRetencao.val());

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

    if (_dadosEmissao.IncluirValorBC.val() || _CONFIGURACAO_TMS.ReduzirRetencaoISSValorAReceberNFSManual)
        valorAReceber -= valorRetencao;

    if (valorAReceber < 0)
        valorAReceber = 0;

    _dadosEmissao.ValorReceber.val(Globalize.format(valorAReceber, "n2"));
}

function ArredondarParaCima(num, precision) {
    precision = Math.pow(10, precision);
    return Math.ceil(num * precision) / precision;
}

function ArredondarParaBaixo(num, precision) {
    precision = Math.pow(10, precision);
    return Math.floor(num * precision) / precision;
}

function CalcularRetencao() {
    if (_dadosEmissao.ValorISS.val() != "" && _dadosEmissao.PercentualRetencao.val() != _dadosEmissao.PercentualRetencao.def) {
        var percentualRetencao = Globalize.parseFloat(_dadosEmissao.PercentualRetencao.val());

        if (percentualRetencao <= 100) {
            var valorISS = Globalize.parseFloat(_dadosEmissao.ValorISS.val());
            var valorRetencao = valorISS * (percentualRetencao / 100);
            _dadosEmissao.ValorRetencao.val(Globalize.format(valorRetencao, "n2"));
        } else {
            _dadosEmissao.PercentualRetencao.val(_dadosEmissao.PercentualRetencao.def);
            _dadosEmissao.ValorRetencao.val(_dadosEmissao.ValorRetencao.def);
        }
    }
    else {
        _dadosEmissao.ValorRetencao.val("0,00");
        //if (_dadosEmissao.ValorISS.val() != "" && _dadosEmissao.ValorRetencao.val() != _dadosEmissao.ValorRetencao.def) {

        //var valorISS = Globalize.parseFloat(_dadosEmissao.ValorISS.val());
        //var valorRetencao = Globalize.parseFloat(_dadosEmissao.ValorRetencao.val());

        //if (valorRetencao <= valorISS) {
        //    var percentualRetencao = (valorRetencao * 100) / valorISS;
        //    _dadosEmissao.PercentualRetencao.val(Globalize.format(percentualRetencao, "n2"));
        //} else {
        //    _dadosEmissao.ValorRetencao.val(_dadosEmissao.ValorRetencao.def);
        //    _dadosEmissao.PercentualRetencao.val(_dadosEmissao.PercentualRetencao.def);
        //}
    }
    CalcularValorAReceber();
}

function EditarDadosNFS(data) {
    _dadosEmissao.Codigo.val(data.Codigo);

    if (data.DadosNFS != null) {
        PreencherObjetoKnout(_dadosEmissao, { Data: data.DadosNFS });

        if (data.DadosNFS.Numero > 0) {
            _dadosEmissao.Emitir.visible(true);

            if (_nfsManual.ContemEDI.val() === true) {
                _dadosEmissao.GerarRPS.visible(true);
                _dadosEmissao.ImportarEDI.visible(true);
            }
        }
        else {
            _dadosEmissao.Emitir.visible(false);
            _dadosEmissao.GerarRPS.visible(false);
            _dadosEmissao.ImportarEDI.visible(false);
        }

        if (data.Filial.Codigo == 0) {
            _dadosEmissao.Filial.visible(true);
            _dadosEmissao.Filial.enable(true);
            _dadosEmissao.Filial.required(true);
        } else {
            _dadosEmissao.Filial.visible(false);
            _dadosEmissao.Filial.enable(false);
            _dadosEmissao.Filial.required(false);
        }

        if (data.Transportador.Codigo == 0) {
            _dadosEmissao.Transportador.visible(true);
            _dadosEmissao.Transportador.enable(true);
            _dadosEmissao.Transportador.required(true);
        } else {
            _dadosEmissao.Transportador.visible(false);
            _dadosEmissao.Transportador.enable(false);
            _dadosEmissao.Transportador.required(false);
        }

        if (data.Tomador.Codigo == 0) {
            _dadosEmissao.Tomador.visible(true);
            _dadosEmissao.Tomador.enable(true);
            _dadosEmissao.Tomador.required(true);
        } else {
            _dadosEmissao.Tomador.visible(false);
            _dadosEmissao.Tomador.enable(false);
            _dadosEmissao.Tomador.required(false);
        }

        if (data.TipoOperacao.Codigo == 0) {
            _dadosEmissao.TipoOperacao.visible(true);
            _dadosEmissao.TipoOperacao.enable(true);
            _dadosEmissao.TipoOperacao.required(true);
        } else {
            _dadosEmissao.TipoOperacao.visible(false);
            _dadosEmissao.TipoOperacao.enable(false);
            _dadosEmissao.TipoOperacao.required(false);
        }

    }
    else {
        _dadosEmissao.Emitir.visible(false);
        _dadosEmissao.GerarRPS.visible(false);
        _dadosEmissao.ImportarEDI.visible(false);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true && _dadosEmissao.Moeda.val() != null && _dadosEmissao.Moeda.val() !== EnumMoedaCotacaoBancoCentral.Real) {
        _dadosEmissao.ModeloDocumentoFiscal.enable(true);
        _dadosEmissao.ModeloDocumentoFiscal.visible(true);
        _dadosEmissao.Moeda.visible(true);
        _dadosEmissao.ValorTotalMoeda.visible(true);
    }

    if (_CONFIGURACAO_TMS.AlterarModeloDocumentoNFSManual) {
        _dadosEmissao.ModeloDocumentoFiscal.enable(true);
        _dadosEmissao.ModeloDocumentoFiscal.visible(true);
    }

    controlarExibicaoCamposDadosDescontos(Globalize.parseFloat(data.DadosNFS.Descontos) > 0);
    ControleCamposDadosEmissao(_nfsManual.Situacao.val() == EnumSituacaoLancamentoNFSManual.DadosNota);
    recarregarGridLancamentoNFSManualDesconto();
    ControleCamposDadosNFSPorTipoServico();

}

function controlarExibicaoCamposDadosDescontos(exibir) {
    _dadosEmissao.ValorTotalFreteBruto.visible(exibir);
    _dadosEmissao.Descontos.visible(exibir);
}

function ControleCamposDadosEmissao(status) {
    _dadosEmissao.Numero.enable(status);
    _dadosEmissao.Serie.enable(status);
    _dadosEmissao.TipoArredondamentoISS.enable(status);
    _dadosEmissao.DataEmissao.enable(status);
    _dadosEmissao.AliquotaISS.enable(status);
    _dadosEmissao.IncluirValorBC.enable(status);
    _dadosEmissao.Observacao.enable(status);
    _dadosEmissao.PercentualRetencao.enable(status);
    _dadosEmissao.XMLAutorizacao.enable(status);
    _dadosEmissao.DANFSE.enable(status);
    _dadosEmissao.Anexo.enable(status);
    _dadosEmissao.ValorPIS.enable(status);
    _dadosEmissao.ValorCOFINS.enable(status);
    _dadosEmissao.ValorPISIBSCBS.enable(status);
    _dadosEmissao.ValorCOFINSIBSCBS.enable(status);
    _dadosEmissao.ValorIR.enable(status);
    _dadosEmissao.ValorCSLL.enable(status);
    _dadosEmissao.ModeloDocumentoFiscal.enable(status);
    _dadosEmissao.CSTIBSCBS.enable(status);
    _dadosEmissao.ClassificacaoTributariaIBSCBS.enable(status);
    _dadosEmissao.BaseCalculoIBSCBS.enable(status);
    _dadosEmissao.NBS.enable(status);
    _dadosEmissao.IndicadorOperacao.enable(status);
    _dadosEmissao.Transportador.enable(status);
    _dadosEmissao.Filial.enable(status);
    _dadosEmissao.TipoOperacao.enable(status);
    _dadosEmissao.Tomador.enable(status);
    _dadosEmissao.ServicoNFSe.enable(status);
    _dadosEmissao.Salvar.visible(status);
    _dadosEmissao.AliquotaCBS.enable(status);
    _dadosEmissao.AliquotaIBSEstadual.enable(status);
    _dadosEmissao.AliquotaIBSMunicipal.enable(status);

    if (_dadosEmissao.Emitir.visible())
        _dadosEmissao.Emitir.visible(status);

    if (_nfsManual.ContemEDI.val() === true) {
        if (_dadosEmissao.GerarRPS.visible())
            _dadosEmissao.GerarRPS.visible(status);

        if (_dadosEmissao.ImportarEDI.visible())
            _dadosEmissao.ImportarEDI.visible(status);
    }
}

function LimparCamposDadosNFS() {
    LimparCampos(_dadosEmissao);
    controlarExibicaoCamposDadosDescontos(false);
    ControleCamposDadosEmissao(true);

    _dadosEmissao.XMLAutorizacao.val(_dadosEmissao.XMLAutorizacao.def);
    _dadosEmissao.DANFSE.val(_dadosEmissao.DANFSE.def);
    _dadosEmissao.Anexo.val(_dadosEmissao.Anexo.def);

    _dadosEmissao.XMLAutorizacao.file.value = null;
    _dadosEmissao.DANFSE.file.value = null;
    _dadosEmissao.Anexo.file.value = null;

    recarregarGridLancamentoNFSManualDesconto();
}

function AnexarArquivos(anexos) {
    // Dados da req
    var dados = {
        Codigo: _dadosEmissao.Codigo.val()
    };

    // Arquivos
    var formData = new FormData();
    anexos.forEach(function (anexo) {
        formData.append("Tipo", anexo.Tipo);
        formData.append("Arquivo", anexo.Arquivo);
    });

    enviarArquivo("NFSManual/Anexar", dados, formData, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Arquivo(s) anexado(s) com sucesso");
                _dadosEmissao.XMLAutorizacao.file.files = null;
                _dadosEmissao.DANFSE.file.files = null;
                _dadosEmissao.Anexo.file.files = null;
            } else {
                exibirMensagem(tipoMensagem.falha, "Não foi possível anexar o(s) arquivo(s).", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function recarregarGridLancamentoNFSManualDesconto() {
    _gridLancamentoNFSManualDesconto.CarregarGrid();
}

function ControleCamposDadosNFSPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _dadosEmissao.Transportador.visible(false).required(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros) {
        _dadosEmissao.Transportador.visible(false).required(false);
        _dadosEmissao.Filial.visible(false).required(false);
        _dadosEmissao.TipoOperacao.visible(false).required(false);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _dadosEmissao.Transportador.visible(true).required(true).text("Empresa/Filial:");
        _dadosEmissao.Filial.visible(false).required(false);
        _dadosEmissao.TipoOperacao.visible(false).required(false);
    }
}