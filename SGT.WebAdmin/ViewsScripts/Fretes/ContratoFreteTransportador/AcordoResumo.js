/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="ContratoFreteTransportador.js" />
/// <reference path="Acordo.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _acordoResumo;

var AcordoResumo = function () {
    this.Modelos = PropertyEntity({ val: ko.observable([]) });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.TotalKm = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.ValorKm = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.ContratoMensal = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.ValorKmExcedente = PropertyEntity({ val: ko.observable(0), def: 0 });
}

function AcordoResumoViewModel(params) {
    this.FormataValor = function (valor) {
        return _FormatHelper(valor);
    };
    this.Modelos = params.Modelos.val;
    this.Total = params.Total.val;

    this.TotalKm = params.TotalKm.val;
    this.ValorKm = params.ValorKm.val;
    this.ContratoMensal = params.ContratoMensal.val;
    this.ValorKmExcedente = params.ValorKmExcedente.val;
}

//*******EVENTOS*******
function LoadAcordoResumo() {
    _acordoResumo = new AcordoResumo();

    RegistrarComponenteResumofranquia();

    KoBindings(_acordoResumo, "knockoutAcordoResumo");
    KoBindings(_acordoResumo, "knockoutFranquiaResumo");
}

//*******MÉTODOS*******

function RegistrarComponenteResumofranquia() {
    if (ko.components.isRegistered('resumofranquia')) return;


    ko.components.register('resumofranquia', {
        viewModel: AcordoResumoViewModel,
        template:
            '<div class="card container-resumo">' + 
                '<div class="card-body">' +  
                    '<h4>Resumo Mensal</h4>' +
                    '<div class="row mt-3">' +
                        '<!-- ko if: Modelos().length > 0 -->' +
                        '<!-- ko foreach: {data: Modelos, as: "modelo"} -->' +
                        '<div class="col-12 col-md-8 d-flex justify-content-between linha-resumo">' +
                            '<label><b><span data-bind="text: modelo.Descricao"></span> (<span data-bind="text: modelo.Quantidade"></span>)</b></label>' +
                            '<div class="col-5 d-flex justify-content-between">' +
                                '<span>R$</span>' +
                                '<span data-bind="text: $parent.FormataValor(modelo.Total)"></span>' +
                            '</div>' +
                        '</div>' +
                        '<!-- /ko -->' +

                        '<div class="col-12 col-md-8 d-flex justify-content-end linha-resumo-totalizador" style="margin-top: -18px; margin-bottom: -14px;">' +
                            '<div class="col-5">' +
                                '<hr style="border-color: #868686; border-width: 1px">' +
                            '</div>' +
                        '</div>' +

                        '<div class="col-12 col-md-8 d-flex justify-content-end">' +
                            '<div class="col-5 d-flex justify-content-between">' +
                                '<span>R$</span>' +
                                '<span data-bind="text: FormataValor(Total())"></span>' +
                            '</div>' +
                        '</div>' +
                        '<!-- /ko -->' +
                        '<!-- ko if: Modelos().length == 0 -->' +
                        '<div class="col-12 col-md-12 d-flex justify-content-center">' +
                            '<span>Nenhum registro cadastrado.</span>' +
                        '</div>' +
                        '<!-- /ko -->' +
                    '</div>' +

                    '<h4 class="mt-2">Resumo Franquia</h4>' +
                    '<div>' +
                        '<div class="col-12 col-md-8 d-flex justify-content-between linha-resumo">' +
                            '<label>Total KM:</label>' +
                            '<span data-bind="text: TotalKm().toFixed(0)">0</span>' +
                        '</div>' +
    
                        '<div class="col-12 col-md-8 d-flex justify-content-between linha-resumo">' +
                                '<label>Valor KM</label>' +
                                '<div class="col-5 d-flex justify-content-between">' +
                                    '<span>R$</span>' +
                                    '<span data-bind="text: FormataValor(ValorKm())"></span>' +
                                '</div>' +
                        '</div>' +
    
                        '<div class="col-12 col-md-8 d-flex justify-content-end linha-resumo-totalizador" style="margin-top: -18px; margin-bottom: -14px;">' +
                            '<div class="col-5">' +
                                '<hr style="border-color: #868686; border-width: 1px">' +
                            '</div>' +
                        '</div>' +
    
                        '<div class="col-12 col-md-8 d-flex justify-content-end">' +
                            '<div class="col-5 d-flex justify-content-between">' +
                                '<span>R$</span>' +
                                '<span data-bind="text: FormataValor(ContratoMensal())"></span>' +
                            '</div>' +
                        '</div>' +
    
                        '<div class="col-12 col-md-8 d-flex justify-content-between mt-3 linha-resumo">' +
                            '<label>Valor KM Excedente:</label>' +
                            '<div class="col-5 d-flex justify-content-between">' +
                                '<span>R$</span>' +
                                '<span data-bind="text: FormataValor(ValorKmExcedente())"></span>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' 
    });
}

function RenderizaResumo() {
    var resumo = ExtraiResumoAcordos();
    
    _acordoResumo.Modelos.val(resumo.Resumo);
    _acordoResumo.Total.val(resumo.Total);
    _franquia.ContratoMensal.val(_FormatHelper(resumo.Total));

    RenderizaResumoVeiculos(resumo);
}

function RenderizaResumoFranquia() {
    _acordoResumo.TotalKm.val(_franquia.TotalKm.val());
    _acordoResumo.ValorKm.val(_ParseFloatHelper(_franquia.ValorKm.val()));
    _acordoResumo.ValorKmExcedente.val(_franquia.ValorKmExcedente.val() != "" ? _ParseFloatHelper(_franquia.ValorKmExcedente.val()) : 0);
    _acordoResumo.ContratoMensal.val(_franquia.ContratoMensal.val() != "" ? _ParseFloatHelper(_franquia.ContratoMensal.val()) : 0);
}


function ExtraiResumoAcordos() {
    var quantidadeAbas = _acordoConfiguracao.QuantidadeAbas.val();
    var objetoTabela = {};
    var tabelaFormatada = [];
    var total = 0;

    var arrayAcordosTodasAbas = _contratoFreteTransportador.Acordos.list.slice();

    var arrayQuantidades = new Array();

    for (var i in arrayAcordosTodasAbas) {
        var acordo = arrayAcordosTodasAbas[i];
        var modeloVeiculo = acordo.ModeloVeicular.codEntity;
        var objPadrao = { Modelo: acordo.ModeloVeicular.codEntity, Descricao: acordo.ModeloVeicular.val, Quantidade: 0, Total: 0 };
        var statAcordo = objetoTabela[modeloVeiculo] || objPadrao;

        if (parseInt(acordo.Quantidade.val) > statAcordo.Quantidade)
            statAcordo.Quantidade = parseInt(acordo.Quantidade.val);

        statAcordo.Total += _ParseFloatHelper(acordo.Total.val);

        objetoTabela[modeloVeiculo] = statAcordo;
    }

    // Sumariza o total
    for (var i in objetoTabela)
        total += objetoTabela[i].Total;

    // Converte o objeto em array
    for (var i in objetoTabela)
        tabelaFormatada.push(objetoTabela[i]);

    return {
        Resumo: tabelaFormatada,
        Total: total
    }
}