var _simularFrete, _gridCompomentesFrete;

var SimularFrete = function () {
    this.Compomentes = PropertyEntity({ type: types.basicTable, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TabelaFrete = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Tabela de Frete" });
    this.ValorFrete = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Valor do Frete" });
    this.ValorTotalComponentes = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Valor dos Componentes" });
    this.TotalFrete = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Total do Frete" });
    this.AliquotaICMS = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "% ICMS" });
    this.ValorICMS = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "ICMS" });
    this.TotalComICMS = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Total com ICMS" });
    this.BaseCalculoICMS = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Base de cálculo" });
    this.IncluirValorICMSBaseCalculo = PropertyEntity({ text: "Incluir valor do ICMS na Base de Cálculo?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.IncluirValorICMSBaseCalculo.val.subscribe(function () {
        CalcularICMS();
    });


    this.ValorFreteTreceiro = PropertyEntity({ type: types.map, visible: ko.observable(true), text: "Valor do Frete" });

    this.MensagemErroFreteNormal = PropertyEntity({ type: types.map, visible: ko.observable(true)});
    this.MensagemErroFreteTreceiro = PropertyEntity({ type: types.map, visible: ko.observable(true)});

};

function loadSimularFrete(){

    _simularFrete = new SimularFrete();
    KoBindings(_simularFrete, "knockoutSimularFrete");

    ConfigurarGridSimularFreteCompomentes();
}

function ConfigurarGridSimularFreteCompomentes() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Componente", title: "Componente", width: "40%" },
        { data: "Valor", title: "Valor", width: "20%" },
        { data: "Percentual", title: "Valor %", width: "20%" },        
        { data: "ValorTotal", title: "Valor Total", width: "20%" },
    ];
    _gridCompomentesFrete = new BasicDataTable(_simularFrete.Compomentes.idGrid, header, null, { column: 1, dir: orderDir.asc });
    _gridCompomentesFrete.CarregarGrid([]);
}

function ShowErrroFrete(erro) {
    $("#DadosSimulacaoFrete").hide();
    $("#MensgemErroFrete").show();
    _simularFrete.MensagemErroFreteNormal.val(erro);
}

function ShowErrroFreteTeceiro(erro) {
    $("#DadosSimulacaoFreteTreceiro").hide();
    $("#MensgemErroFreteTreceiro").show();
    _simularFrete.MensagemErroFreteTreceiro.val(erro);
}

function simularFreteClick(pedidoSelecionado) {
    _gridCompomentesFrete.CarregarGrid([]);
    LimparCampos(_simularFrete);
    var dadosPedidoAtualizar = {
        Codigo: pedidoSelecionado.Codigo,
        PagamentoTerceiro: false
    };
    executarReST("PlanejamentoPedidoTMS/SimularFrete", dadosPedidoAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                PrencherFreteNoraml(retorno.Data);
            else
                ShowErrroFrete(retorno.Msg);
        }
        else
            ShowErrroFrete(retorno.Msg);       
    }, BuscarFreteTerceiro(pedidoSelecionado)).then(el => {
        ShowSimulacao();
    });    
}

function PrencherFreteNoraml(data) {
    $("#MensgemErroFrete").hide();
    $("#DadosSimulacaoFrete").show();
    if (data.compomentes) {
        _gridCompomentesFrete.CarregarGrid(data.compomentes);
    } 
    _simularFrete.TabelaFrete.val(data.TabelaFrete);
    _simularFrete.ValorFrete.val(data.ValorFrete);
    _simularFrete.ValorTotalComponentes.val(data.ValorComponentes);
    _simularFrete.AliquotaICMS.val(data.AliquotaICMS);
    _simularFrete.BaseCalculoICMS.val(data.BaseCalculoICMS);

    _simularFrete.IncluirValorICMSBaseCalculo.val(!data.NaoIncluirICMSFrete);

    var valorFrete = Globalize.parseFloat(data.ValorFrete || "0");
    var valorCompomentes = Globalize.parseFloat(data.ValorComponentes || "0");

    let totalFrete = valorFrete + valorCompomentes;

    _simularFrete.TotalFrete.val(Globalize.format(totalFrete, "n2"));
    CalcularICMS(data);
    
}

function CalcularICMS() {
    
    var aliquotaICMS = Globalize.parseFloat(_simularFrete.AliquotaICMS.val() || "0");
    var totalFrete = Globalize.parseFloat(_simularFrete.TotalFrete.val() || "0");
    var BaseCalculoICMS = Globalize.parseFloat(_simularFrete.BaseCalculoICMS.val() || "0");

    var valorICMS = 0;
    var valorTotalComICMS = totalFrete;

    if (aliquotaICMS > 0 && BaseCalculoICMS > 0) {
        if (_simularFrete.IncluirValorICMSBaseCalculo.val())
            valorICMS = (BaseCalculoICMS / ((100 - aliquotaICMS) / 100)) * (aliquotaICMS / 100)
        else
            valorICMS = BaseCalculoICMS * (aliquotaICMS / 100);

        if (valorICMS > 0)
            valorTotalComICMS = totalFrete + valorICMS;
    }

    _simularFrete.ValorICMS.val(Globalize.format(valorICMS, "n2"));
    _simularFrete.TotalComICMS.val(Globalize.format(valorTotalComICMS, "n2"));
}


function BuscarFreteTerceiro(pedidoSelecionado) {
    var dadosPedidoAtualizar = {
        Codigo: pedidoSelecionado.Codigo,
        PagamentoTerceiro: true
    };

    executarReST("PlanejamentoPedidoTMS/SimularFrete", dadosPedidoAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                PreencherFreteTeceiro(retorno.Data);
            else
                ShowErrroFreteTeceiro(retorno.Msg);                
        }
        else
            ShowErrroFreteTeceiro(retorno.Msg);
    });
}

function PreencherFreteTeceiro(data) {
    $("#MensgemErroFreteTreceiro").hide();
    $("#DadosSimulacaoFreteTreceiro").show();
    _simularFrete.ValorFreteTreceiro.val(data.ValorFrete);   
}

function ShowSimulacao() {
    Global.abrirModal('divSimularFrete'); 
}


