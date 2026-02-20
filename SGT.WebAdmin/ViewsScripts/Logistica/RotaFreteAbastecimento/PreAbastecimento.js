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
/// <reference path="RotaFreteAbastecimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPreAbastecimento;
var _preAbastecimento;

var PreAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local, id: guid() });
    this.PreAbastecimento = PropertyEntity({ eventClick: abrirModalPreAbastecimento, type: types.event, text: Localization.Resources.Logistica.RotaFreteAbastecimento.AdicionarPreAbastecimento});

    this.TipoAbastecimento = PropertyEntity({ val: ko.observable(EnumTipoAbastecimento.Combustivel), options: EnumTipoAbastecimento.obterOpcoes(), def: EnumTipoAbastecimento.Combustivel, text: "*Tipo: ", enable: ko.observable(true) });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Logistica.RotaFreteAbastecimento.Produto, idBtnSearch: guid(), issue: 140, enable: ko.observable(true), eventChange: produtoBlur });
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Logistica.RotaFreteAbastecimento.Posto, idBtnSearch: guid(), issue: 171, enable: ko.observable(true) });

    this.Litros = PropertyEntity({ text: Localization.Resources.Logistica.RotaFreteAbastecimento.Litros, required: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 10, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: Localization.Resources.Logistica.RotaFreteAbastecimento.ValorUnitario, required: true, getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable("0,0000"), maxlength: 8, enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: Localization.Resources.Logistica.RotaFreteAbastecimento.ValorTotal, required: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable("0,00"), maxlength: 10, enable: ko.observable(true), visible: ko.observable(true) });

    this.TanqueCheio = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.RotaFreteAbastecimento.TanqueCheio, visible: ko.observable(true), required: false });
    this.TanqueCheioDescricao = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Logistica.RotaFreteAbastecimento.TanqueCheio, visible: ko.observable(true), required: false });

    this.AdicionarPreAbastecimento = PropertyEntity({ eventClick: adicionarPreAbastecimentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.CancelarPreAbastecimento = PropertyEntity({ eventClick: cancelarPreAbastecimentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    this.TanqueCheio.val.subscribe(function (novoValor) {
        _preAbastecimento.Litros.visible(!novoValor);
        _preAbastecimento.Litros.required(!novoValor);
        _preAbastecimento.ValorTotal.required(!novoValor);
    });
};

//*******EVENTOS*******

function LoadPreAbastecimento() {
    _preAbastecimento = new PreAbastecimento();
    KoBindings(_preAbastecimento, "knockoutPreAbastecimento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirPreAbastecimentoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Posto", visible: false },
        { data: "Produto", visible: false },
        { data: "TipoAbastecimento", visible: false },
        { data: "DescricaoPosto", title: Localization.Resources.Logistica.RotaFreteAbastecimento.Posto, width: "15%" },
        { data: "DescricaoProduto", title: Localization.Resources.Logistica.RotaFreteAbastecimento.Produto, width: "15%" },
        { data: "Litros", title: Localization.Resources.Logistica.RotaFreteAbastecimento.Litros, width: "15%" },
        { data: "ValorUnitario", title: Localization.Resources.Logistica.RotaFreteAbastecimento.ValorUnitario, width: "15%" },
        { data: "ValorTotal", title: Localization.Resources.Logistica.RotaFreteAbastecimento.ValorTotal, width: "15%" },
        { data: "TanqueCheioDescricao", title: Localization.Resources.Logistica.RotaFreteAbastecimento.TanqueCheio, width: "15%" }

    ];

    _gridPreAbastecimento = new BasicDataTable(_preAbastecimento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridPreAbastecimento();

    new BuscarClientes(_preAbastecimento.Posto, RetornoPosto, false, [EnumModalidadePessoa.Fornecedor], null, null, null, null, null, null, null, null, _preAbastecimento.TipoAbastecimento);

    new BuscarProdutoTMS(_preAbastecimento.Produto, RetornoProdutoTMS, _preAbastecimento.TipoAbastecimento);

}

function abrirModalPreAbastecimento(retorno) {
    LimparCamposPreAbastecimento();
    Global.abrirModal("divModalPreAbastecimento");
}

function adicionarPreAbastecimentoClick() {
    var incluir = true;

    if (_preAbastecimento.Produto.val() == "")
        incluir = false;
    if (_preAbastecimento.Posto.val() == "")
        incluir = false;
    if (_preAbastecimento.Litros.val() == "" && _preAbastecimento.TanqueCheio.val() == false)
        incluir = false;
    if (_preAbastecimento.ValorUnitario.val() == "")
        incluir = false;
    if (_preAbastecimento.ValorTotal.val() == "" && _preAbastecimento.TanqueCheio.val() == false)
        incluir = false;
    if (_preAbastecimento.TanqueCheio.val() == "" || _preAbastecimento.TanqueCheio.val() === null) {
        _preAbastecimento.TanqueCheio.val(false);
    }

    if (incluir) {
        var data = new Array();

        var preAbastecimento = new Object();
        preAbastecimento.Codigo = guid();
        preAbastecimento.TipoAbastecimento = _preAbastecimento.TipoAbastecimento.val();
        preAbastecimento.Produto = _preAbastecimento.Produto.codEntity();
        preAbastecimento.Posto = _preAbastecimento.Posto.codEntity();
        preAbastecimento.Litros = _preAbastecimento.Litros.val();
        preAbastecimento.ValorUnitario = _preAbastecimento.ValorUnitario.val();
        preAbastecimento.ValorTotal = _preAbastecimento.ValorTotal.val();
        preAbastecimento.DescricaoPosto = _preAbastecimento.Posto.val();
        preAbastecimento.DescricaoProduto = _preAbastecimento.Produto.val();
        preAbastecimento.TanqueCheio = _preAbastecimento.TanqueCheio.val();
        preAbastecimento.TanqueCheioDescricao = _preAbastecimento.TanqueCheio.val() ? "Sim" : "Não";
        
        data.push(preAbastecimento);        

        if (_rotaFreteAbastecimento.PreAbastecimentos.val() != "") {
            $.each(_rotaFreteAbastecimento.PreAbastecimentos.val(), function (i, arg) {

                if (arg.Codigo != preAbastecimento.Codigo) {
                    var preAbastecimentoExistente = new Object();
                    preAbastecimentoExistente.Codigo = arg.Codigo;
                    preAbastecimentoExistente.TipoAbastecimento = arg.TipoAbastecimento;
                    preAbastecimentoExistente.Posto = arg.Posto;
                    preAbastecimentoExistente.Produto = arg.Produto;
                    preAbastecimentoExistente.Litros = arg.Litros;
                    preAbastecimentoExistente.ValorUnitario = arg.ValorUnitario;
                    preAbastecimentoExistente.ValorTotal = arg.ValorTotal;
                    preAbastecimentoExistente.DescricaoPosto = arg.DescricaoPosto;
                    preAbastecimentoExistente.DescricaoProduto = arg.DescricaoProduto;
                    preAbastecimentoExistente.TanqueCheio = arg.TanqueCheio;
                    preAbastecimentoExistente.TanqueCheioDescricao = arg.TanqueCheioDescricao;

                    data.push(preAbastecimentoExistente);
                }
       
            });
        }
        console.log(preAbastecimento);
        console.log(_rotaFreteAbastecimento.PreAbastecimentos.val());
        _rotaFreteAbastecimento.PreAbastecimentos.val(data);
        _gridPreAbastecimento.CarregarGrid(data);

        LimparCamposPreAbastecimento();
        Global.fecharModal("divModalPreAbastecimento");
    }
    else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function cancelarPreAbastecimentoClick() {
    LimparCamposPreAbastecimento();
    Global.fecharModal("divModalPreAbastecimento");
}

function produtoBlur() {
    if (_PreAbastecimento.Produto.val() == "") {
        _PreAbastecimento.ValorUnitario.val(0.0000);
    }
}

function calculaLitrosAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_preAbastecimento.Litros.val() != null & _preAbastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.Litros.val()), "n4")).toFixed(4);

    if (_preAbastecimento.ValorUnitario.val() != null & _preAbastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_preAbastecimento.ValorTotal.val() != null & _preAbastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (litros > 0) {
        if (valorUnitario > 0) {
            _preAbastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        } else if (valorTotal > 0) {
            _preAbastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorUnitarioAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_preAbastecimento.Litros.val() != null & _preAbastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.Litros.val()), "n4")).toFixed(4);

    if (_preAbastecimento.ValorUnitario.val() != null & _preAbastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_preAbastecimento.ValorTotal.val() != null & _preAbastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (valorUnitario > 0) {
        if (litros > 0) {
            _preAbastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    } else if (valorTotal > 0) {
        if (litros > 0) {
            _preAbastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function calculaValorTotalAbascimento(e) {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_preAbastecimento.Litros.val() != null & _preAbastecimento.Litros.val() != "")
        litros = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.Litros.val()), "n4")).toFixed(4);

    if (_preAbastecimento.ValorUnitario.val() != null & _preAbastecimento.ValorUnitario.val() != "")
        valorUnitario = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.ValorUnitario.val()), "n4")).toFixed(4);

    if (_preAbastecimento.ValorTotal.val() != null & _preAbastecimento.ValorTotal.val() != "")
        valorTotal = parseFloat(formatarStrFloat(Globalize.format(_preAbastecimento.ValorTotal.val(), "n2"))).toFixed(2);

    if (valorTotal > 0) {
        if (litros > 0) {
            _preAbastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    } else if (valorUnitario > 0) {
        if (litros > 0) {
            _preAbastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        }
    }
}

function formatarStrFloat(valor) {
    valor = valor.replace(".", "");
    return valor.replace(",", ".");
}

function RetornoPosto(data) {
    _preAbastecimento.Posto.val(data.Descricao);
    _preAbastecimento.Posto.codEntity(data.Codigo);

    if (data.CodigoCombustivel > 0) {
        LimparCampoEntity(_preAbastecimento.Produto);
        _preAbastecimento.Produto.codEntity(data.CodigoCombustivel);
        _preAbastecimento.Produto.val(data.DescricaoCombustivel);
        _preAbastecimento.ValorUnitario.val(data.ValorCombustivel);

        if (_preAbastecimento.Produto.val() != null && _preAbastecimento.Produto.val() != "" && _preAbastecimento.Posto.val() != null && _preAbastecimento.Posto.val() != "") {
            executarReST("Produto/CustoProduto", { Produto: _preAbastecimento.Produto.codEntity(), Filial: _preAbastecimento.Posto.codEntity() }, function (retorno) {
                if (retorno.Success) {

                    if (parseFloat(_abastecimento.ValorUnitario.val()) == 0)
                        _abastecimento.ValorUnitario.val(retorno.Data.UltimoCusto);
                }
            }, null);
        }
    }
}

function RetornoProdutoTMS(data) {
    var produtoAnterior = _preAbastecimento.Produto.codEntity();
    _preAbastecimento.Produto.val(data.Descricao);
    _preAbastecimento.Produto.codEntity(data.Codigo);
    var alterouProduto = (produtoAnterior != _preAbastecimento.Produto.codEntity())
    if (_preAbastecimento.ValorUnitario.val() == "" || _preAbastecimento.ValorUnitario.val() == null || _preAbastecimento.ValorUnitario.val() == "0,0000" || alterouProduto) {
        if (_preAbastecimento.Produto.val() != null && _preAbastecimento.Produto.val() != "" && _preAbastecimento.Posto.val() != null && _preAbastecimento.Posto.val() != "") {
            executarReST("Produto/CustoProduto", { Produto: _preAbastecimento.Produto.codEntity(), Filial: _preAbastecimento.Posto.codEntity()}, function (retorno) {
                if (retorno.Success)
                    _preAbastecimento.ValorUnitario.val(retorno.Data.UltimoCusto);
            }, null);
        }
        else
            _preAbastecimento.ValorUnitario.val(data.UltimoCustoCombustivel);
    }
}

function RecarregarGridPreAbastecimento() {
    var data = new Array();

    if (_rotaFreteAbastecimento.PreAbastecimentos.val() != "") {
        $.each(_rotaFreteAbastecimento.PreAbastecimentos.val(), function (i, arg) {
            var preAbastecimento = new Object();

            preAbastecimento.Codigo = arg.Codigo;
            preAbastecimento.TipoAbastecimento = arg.TipoAbastecimento;
            preAbastecimento.Produto = arg.Produto;
            preAbastecimento.Posto = arg.Posto;
            preAbastecimento.Litros = arg.Litros;
            preAbastecimento.ValorUnitario = arg.ValorUnitario;
            preAbastecimento.ValorTotal = arg.ValorTotal;
            preAbastecimento.DescricaoPosto = arg.DescricaoPosto;
            preAbastecimento.DescricaoProduto = arg.DescricaoProduto;
            preAbastecimento.TanqueCheio = arg.TanqueCheio;
            preAbastecimento.TanqueCheioDescricao = arg.TanqueCheioDescricao;

            data.push(preAbastecimento);
        });
    }

    _gridPreAbastecimento.CarregarGrid(data);
}

function ExcluirPreAbastecimentoClick(data) {
    var preAbastecimentos = _gridPreAbastecimento.BuscarRegistros();

    for (var i = 0; i < preAbastecimentos.length; i++) {
        if (data.Codigo == preAbastecimentos[i].Codigo) {
            preAbastecimentos.splice(i, 1);
            break;
        }
    }
    _rotaFreteAbastecimento.PreAbastecimentos.val(preAbastecimentos);
    _gridPreAbastecimento.CarregarGrid(preAbastecimentos);
}

function LimparCamposPreAbastecimentoRotaFreteAbastecimento() {
    LimparCamposPreAbastecimento();
    _gridPreAbastecimento.CarregarGrid(new Array());
}

function LimparCamposPreAbastecimento() {
    LimparCampos(_preAbastecimento);
}


