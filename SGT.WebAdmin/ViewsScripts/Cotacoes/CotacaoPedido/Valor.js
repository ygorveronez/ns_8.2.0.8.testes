/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoClienteCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="CotacaoPedido.js" />

var _cotacaoPedidoValor;
var _gridComponentes;

var ComponenteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.Valor = PropertyEntity({ type: types.map, val: "" });
    this.PercentualAcrescimo = PropertyEntity({ type: types.map, val: "" });
    this.PercentualDesconto = PropertyEntity({ type: types.map, val: "" });
    this.ValorTotal = PropertyEntity({ type: types.map, val: "" });
    this.CodigoComponente = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoComponente = PropertyEntity({ type: types.map, val: "" });
};

var CotacaoPedidoValor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BuscarValoresTabelaFrete = PropertyEntity({ type: types.event, eventClick: BuscarValoresTabelaFreteClick, text: ko.observable("Buscar valores da tabela de frete"), visible: ko.observable(true), enable: ko.observable(true) });

    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Componente de Frete:", idBtnSearch: guid(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Valor:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Percentual = PropertyEntity({ val: ko.observable(""), def: "", text: "*% Valor", required: false, getType: typesKnockout.decimal, visible: ko.observable(false), configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.PercentualAcrescimo = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, def: "0,00", val: ko.observable("0,00"), maxlength: 6, text: "% Acréscimo:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualDesconto = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, def: "0,00", val: ko.observable("0,00"), maxlength: 6, text: "% Desconto:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "*Total:", required: false, visible: ko.observable(true), enable: ko.observable(false) });

    this.AdicionarComponente = PropertyEntity({ type: types.event, eventClick: AdicionarComponenteClick, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.Componentes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.ListaComponente = PropertyEntity({ val: ko.observable(""), def: "" });

    this.ValorFrete = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, def: "0,00", val: ko.observable("0,00"), maxlength: 15, text: "Valor do Frete:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.ValorCotacao = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Valor dos Componentes:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.PercentualAcrescimoValorCotacao = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, def: "0,00", val: ko.observable("0,00"), maxlength: 6, text: "% Acréscimo:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.PercentualDescontoValorCotacao = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, def: "0,00", val: ko.observable("0,00"), maxlength: 6, text: "% Desconto:", required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorTotalCotacao = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Total da Cotação:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.AliquotaICMS = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "% ICMS:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.ValorICMS = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "ICMS:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.ValorTotalCotacaoComICMS = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: "Total com ICMS:", required: false, visible: ko.observable(true), enable: ko.observable(false) });
    this.ValorFreteTerceiro = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, def: "0,00", val: ko.observable("0,00"), maxlength: 13, text: "Valor do Frete Terceiro:", required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.IncluirValorICMSBaseCalculo = PropertyEntity({ text: "Incluir valor do ICMS na Base de Cálculo?", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.Valor.val.subscribe(function () {
        CalcularValorComponente();
    });
    this.PercentualAcrescimo.val.subscribe(function () {
        CalcularValorComponente();
    });
    this.PercentualDesconto.val.subscribe(function () {
        CalcularValorComponente();
    });
    this.Percentual.val.subscribe(function (newValue) {
        var valor = parseFloat(newValue);
        if (valor > 100) {
            this.Percentual.val("100"); 
        }
        CalcularValorComponente(); 
    }.bind(this));

    this.PercentualAcrescimoValorCotacao.val.subscribe(function () {
        CalcularValorTotalCotacao();
    });
    this.PercentualDescontoValorCotacao.val.subscribe(function () {
        CalcularValorTotalCotacao();
    });
    this.ValorFrete.val.subscribe(function () {
        CalcularValorTotalCotacao();
    });
    this.IncluirValorICMSBaseCalculo.val.subscribe(function () {
        CalcularICMS();
    });
};

//*******EVENTOS*******

function loadCotacaoPedidoValor() {
    _cotacaoPedidoValor = new CotacaoPedidoValor();
    KoBindings(_cotacaoPedidoValor, "knockoutValores");

    new BuscarComponentesDeFrete(_cotacaoPedidoValor.ComponenteFrete, RetornoConsultaComponentesFrete);
    
    let tabValores = document.querySelector('a[href="#knockoutValores"]');
    tabValores.addEventListener('shown.bs.tab', function (event) {
        BuscarAliquotaICMS();
    });

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirComponente(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoComponente", visible: false },
        { data: "DescricaoComponente", title: "Componente", width: "30%" },
        { data: "Valor", title: "Valor", width: "10%" },
        { data: "Percentual", title: "% Valor", width: "10%" },
        { data: "PercentualAcrescimo", title: "% Acréscimo", width: "10%" },
        { data: "PercentualDesconto", title: "% Desconto", width: "10%" },
        { data: "ValorTotal", title: "Valor Total", width: "10%" }
    ];

    _gridComponentes = new BasicDataTable(_cotacaoPedidoValor.Componentes.idGrid, header, menuOpcoes);
    recarregarGridComponentes();
}

function BuscarValoresTabelaFreteClick(e, sender) {
    var data = {
        DataColeta: _cotacaoPedido.Previsao.val(),
        DataFinalViagem: _cotacaoPedidoAdicional.DataFinalColeta.val(),
        DataInicialViagem: _cotacaoPedidoAdicional.DataInicialColeta.val(),
        DataVigencia: _cotacaoPedido.Data.val(),
        Destino: _cotacaoPedido.Destino.codEntity(),
        CNPJClienteAtivo: _cotacaoPedido.ClienteAtivo.codEntity(),
        CNPJClienteInativo: _cotacaoPedido.ClienteInativo.codEntity(),
        CodigoLocalidadeDestino: _localidadeDestino.Localidade.codEntity(),
        CodigoLocalidadeOrigem: _localidadeOrigem.Localidade.codEntity(),
        CodigoGrupoPessoa: _cotacaoPedido.GrupoPessoas.codEntity(),
        CodigoModeloVeicular: _cotacaoPedido.ModeloVeicularCarga.codEntity(),
        Distancia: _cotacaoPedidoAdicional.KMTotal.val(),
        EscoltaArmada: _cotacaoPedidoAdicional.EscoltaArmada.val(),
        GerenciamentoRisco: _cotacaoPedidoAdicional.GerenciamentoRisco.val(),
        NumeroAjudantes: _cotacaoPedidoAdicional.QtdAjudantes.val(),
        NumeroEntregas: _cotacaoPedidoAdicional.QtdEntregas.val(),
        NumeroPallets: _cotacaoPedidoAdicional.NumeroPaletes.val(),
        NumeroPedidos: 1,
        Origem: _cotacaoPedido.Origem.codEntity(),
        Peso: _cotacaoPedidoAdicional.PesoTotal.val(),
        PesoCubado: _cotacaoPedidoAdicional.TotalPesoCubado.val(),
        QuantidadeNotasFiscais: _cotacaoPedidoAdicional.QuantidadeNotas.val(),
        Rastreado: _cotacaoPedidoAdicional.Rastreado.val(),
        CNPJDestinatario: _cotacaoPedido.Destinatario.codEntity(),
        CodigoTipoDeCarga: _cotacaoPedido.TipoDeCarga.codEntity(),
        CodigoTipoOperacao: _cotacaoPedido.TipoOperacao.codEntity(),
        ValorNotasFiscais: _cotacaoPedidoAdicional.ValorTotalNotasFiscais.val(),
        Volumes: _cotacaoPedidoAdicional.QtVolumes.val(),
        PesoTotal: _cotacaoPedidoAdicional.PesoTotal.val(),
        PagamentoTerceiro: false
    };

    executarReST("CotacaoPedido/BuscarValoresTabelafrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.FreteCalculado === false)
                    exibirMensagem(tipoMensagem.atencao, "Não foi possível calcular os valores", arg.Data.MensagemRetorno);

                _cotacaoPedidoValor.Componentes.list = new Array();

                if (arg.Data.Componentes != null) {
                    $.each(arg.Data.Componentes, function (i, componente) {
                        var obj = new Object();

                        obj.Codigo = componente.Codigo;
                        obj.Valor = componente.Valor;
                        obj.Percentual = componente.Percentual;
                        obj.PercentualAcrescimo = componente.PercentualAcrescimo;
                        obj.PercentualDesconto = componente.PercentualDesconto;
                        obj.ValorTotal = componente.ValorTotal;
                        obj.DescricaoComponente = componente.DescricaoComponente;
                        obj.CodigoComponente = componente.CodigoComponente;

                        _cotacaoPedidoValor.Componentes.list.push(obj);
                    });
                }

                _cotacaoPedidoValor.ValorFrete.val(arg.Data.ValorFrete);

                recarregarGridComponentes();
                limparDadosComponente();
                $("#" + _cotacaoPedidoValor.ComponenteFrete.id).focus();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Data.MensagemRetorno);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });

}

function AdicionarComponenteClick(e, sender) {
    var tudoCerto = true;
    if (_cotacaoPedidoValor.Valor.val() === "" && _cotacaoPedidoValor.Percentual.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoValor.ValorTotal.val() === "")
        tudoCerto = false;
    if (_cotacaoPedidoValor.ComponenteFrete.codEntity() === 0 || _cotacaoPedidoValor.ComponenteFrete.codEntity() === "")
        tudoCerto = false;

    if (tudoCerto) {
        var map = new Object();

        map.Codigo = guid();
        map.Valor = _cotacaoPedidoValor.Valor.val();
        map.Percentual = _cotacaoPedidoValor.Percentual.val();
        map.PercentualAcrescimo = _cotacaoPedidoValor.PercentualAcrescimo.val();
        map.PercentualDesconto = _cotacaoPedidoValor.PercentualDesconto.val();
        map.ValorTotal = _cotacaoPedidoValor.ValorTotal.val();
        map.DescricaoComponente = _cotacaoPedidoValor.ComponenteFrete.val();
        map.CodigoComponente = _cotacaoPedidoValor.ComponenteFrete.codEntity();

        _cotacaoPedidoValor.Componentes.list.push(map);

        recarregarGridComponentes();
        limparDadosComponente();
        $("#" + _cotacaoPedidoValor.ComponenteFrete.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento do componente!");
    }
}

function excluirComponente(e) {
    if (_cotacaoPedido.SituacaoPedido.val() !== EnumSituacaoPedido.Aberto) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "O status da cotação não permite remover o componente!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja o componente selecionado?", function () {
        $.each(_cotacaoPedidoValor.Componentes.list, function (i, componente) {
            if (componente !== null && componente.Codigo !== null && e !== null && e.Codigo !== null && e.Codigo === componente.Codigo) {
                _cotacaoPedidoValor.Componentes.list.splice(i, 1);
            }
        });
        recarregarGridComponentes();
    });
}

//********METODOS********

function RetornoConsultaComponentesFrete(arg) {
    _cotacaoPedidoValor.ComponenteFrete.val(arg.Descricao);
    _cotacaoPedidoValor.ComponenteFrete.codEntity(arg.Codigo);
   
    if (arg.TipoValor === EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal) {
        _cotacaoPedidoValor.Valor.visible(false);
        _cotacaoPedidoValor.Percentual.visible(true);
        _cotacaoPedidoValor.Percentual.required = true;
        _cotacaoPedidoValor.Valor.required = false;
        _cotacaoPedidoValor.Percentual.val("");
        _cotacaoPedidoValor.Valor.val(0);
    }
    else {
        _cotacaoPedidoValor.Valor.visible(true);
        _cotacaoPedidoValor.Percentual.visible(false);
        _cotacaoPedidoValor.Percentual.required = false;
        _cotacaoPedidoValor.Valor.required = true;
        _cotacaoPedidoValor.Percentual.val(0);
        _cotacaoPedidoValor.Valor.val("");
    }
}

function BuscarAliquotaICMS() {
    if (_cotacaoPedido.ClienteAtivo.codEntity() > 0 || _cotacaoPedido.ClienteInativo.codEntity() > 0) {
        var data = {
            ClienteAtivo: _cotacaoPedido.ClienteAtivo.codEntity(),
            ClienteInativo: _cotacaoPedido.ClienteInativo.codEntity(),
            Destinatario: _cotacaoPedido.Destinatario.codEntity(),
            Tomador: _cotacaoPedido.Tomador.codEntity(),
            Destino: _cotacaoPedido.Destino.codEntity(),
            Origem: _cotacaoPedido.Origem.codEntity(),
            AbaOrigem: JSON.stringify(RetornarObjetoPesquisa(_localidadeOrigem)),
            AbaDestino: JSON.stringify(RetornarObjetoPesquisa(_localidadeDestino)),
            TipoOperacao: _cotacaoPedido.TipoOperacao.codEntity(),
            TipoDeCarga: _cotacaoPedido.TipoDeCarga.codEntity(),
            TipoTomador: _cotacaoPedido.TipoTomador.val(),
        };
        executarReST("CotacaoPedido/BuscarAliquotaICMS", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    _cotacaoPedidoValor.AliquotaICMS.val(arg.Data.AliquotaICMS);
                    CalcularICMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    }
}

function CalcularICMS() {
    var valorTotalCotacao = Globalize.parseFloat(_cotacaoPedidoValor.ValorTotalCotacao.val());
    var aliquotaICMS = Globalize.parseFloat(_cotacaoPedidoValor.AliquotaICMS.val());

    var valorICMS = 0;
    var valorTotalComICMS = valorTotalCotacao;

    if (aliquotaICMS > 0 && valorTotalCotacao > 0) {
        if (_cotacaoPedidoValor.IncluirValorICMSBaseCalculo.val())
            valorICMS = (valorTotalCotacao / ((100 - aliquotaICMS) / 100)) * (aliquotaICMS / 100)
        else
            valorICMS = valorTotalCotacao * (aliquotaICMS / 100);

        if (valorICMS > 0)
            valorTotalComICMS = valorTotalCotacao + valorICMS;
    }
    _cotacaoPedidoValor.ValorICMS.val(Globalize.format(valorICMS, "n2"));
    _cotacaoPedidoValor.ValorTotalCotacaoComICMS.val(Globalize.format(valorTotalComICMS, "n2"));
}

function CalcularValorComponente() {
    var valor = Globalize.parseFloat(_cotacaoPedidoValor.Valor.val() || "0");
    var percentual = Globalize.parseFloat(_cotacaoPedidoValor.Percentual.val() || "0");
    var valorMercadoria = Globalize.parseFloat(_cotacaoPedidoAdicional.ValorTotalNotasFiscais.val() || "0");
    var percentualAcrescimo = Globalize.parseFloat(_cotacaoPedidoValor.PercentualAcrescimo.val() || "0");
    var percentualDesconto = Globalize.parseFloat(_cotacaoPedidoValor.PercentualDesconto.val() || "0");
    var valorTotalComponente = 0;

    if (valorMercadoria <= 0 && percentual > 0) {
        exibirMensagem("atencao", "Atenção", "É necessário informar o valor da mercadoria para calcular o valor percentual do componente de frete!");
        return;
    }

    if (valor > 0) {
        valorTotalComponente = valor;
    } else if (percentual > 0) {
        valorTotalComponente = valorMercadoria * (percentual / 100);
    }

    if (valorTotalComponente > 0) {
        if (percentualAcrescimo > 0) {
            valorTotalComponente += (valorTotalComponente * percentualAcrescimo) / 100;
        }
        if (percentualDesconto > 0) {
            valorTotalComponente -= (valorTotalComponente * percentualDesconto) / 100;
        }
    }

    _cotacaoPedidoValor.ValorTotal.val(Globalize.format(valorTotalComponente, "n2"));
}

function CalcularValorTotalCotacao() {
    var valor = Globalize.parseFloat(_cotacaoPedidoValor.ValorCotacao.val()) + Globalize.parseFloat(_cotacaoPedidoValor.ValorFrete.val());
    var percentualAcrescimo = Globalize.parseFloat(_cotacaoPedidoValor.PercentualAcrescimoValorCotacao.val());
    var percentualDesconto = Globalize.parseFloat(_cotacaoPedidoValor.PercentualDescontoValorCotacao.val());
    var valorTotalCotacao = 0;

    if (valor > 0) {
        if (percentualAcrescimo > 0)
            valor = (((valor * percentualAcrescimo) / 100) + valor);

        if (percentualDesconto > 0)
            valor = (valor - ((valor * percentualDesconto) / 100));

        valorTotalCotacao = valor;
    }

    _cotacaoPedidoValor.ValorTotalCotacao.val(Globalize.format(valorTotalCotacao, "n2"));
    CalcularICMS();
}

function recarregarGridComponentes() {
    var data = new Array();

    var totalCotacao = 0;

    $.each(_cotacaoPedidoValor.Componentes.list, function (i, componente) {
        var obj = new Object();

        obj.Codigo = componente.Codigo;
        obj.Valor = componente.Valor;
        obj.Percentual = componente.Percentual;
        obj.PercentualAcrescimo = componente.PercentualAcrescimo;
        obj.PercentualDesconto = componente.PercentualDesconto;
        obj.ValorTotal = componente.ValorTotal;
        obj.DescricaoComponente = componente.DescricaoComponente;
        obj.CodigoComponente = componente.CodigoComponente;

        var valorComponente = Globalize.parseFloat(componente.ValorTotal);

        if (isNaN(valorComponente))
            valorComponente = 0;

        totalCotacao += valorComponente;

        data.push(obj);
    });

    _cotacaoPedidoValor.ValorCotacao.val(Globalize.format(totalCotacao, "n2"));
    CalcularValorTotalCotacao();
    _gridComponentes.CarregarGrid(data);
}

function limparDadosComponente() {
    LimparCampoEntity(_cotacaoPedidoValor.ComponenteFrete);
    _cotacaoPedidoValor.Valor.val("");
    _cotacaoPedidoValor.Percentual.val("");
    _cotacaoPedidoValor.PercentualAcrescimo.val("0,00");
    _cotacaoPedidoValor.PercentualDesconto.val("0,00");
    _cotacaoPedidoValor.ValorTotal.val("");
}

function limparCotacaoPedidoValor() {
    LimparCampos(_cotacaoPedidoValor);
    recarregarGridComponentes();
}