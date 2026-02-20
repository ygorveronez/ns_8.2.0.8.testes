/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="Carregamento.js" />
/// <reference path="Pedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pesquisaProdutoCarregamento;
var _pesquisaProdutoOutroCarregamento;
var _pesquisaProtudosNaoAtendido;

var _carregamentoProdutos;
var _outroCarregamentoProdutos;
var _produtosNaoAtendidos;

var _gridProdutosCarregamento;
var _gridProdutosOutroCarregamento;
var _gridProdutosNaoAtendidos;

var _detalhePedidoProdutoCarregamento;

/*
 * Declaração das Classes
 */

var FiltroPesquisa = function () {
    this.Carregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cliente = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var CarregamentoProduto = function () {

    this.SelectCarregamentos = PropertyEntity({ val: ko.observable(0), options: ko.observable(new Array()), def: 0, text: Localization.Resources.Cargas.MontagemCargaMapa.Carregamento.getFieldDescription(), required: true, eventChange: changeSelectCarregamentos });
    this.PesquisarCarregamento = PropertyEntity({ eventClick: pesquisarOutroCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar });

    this.ClientesCarregamento = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.Grid = PropertyEntity({ type: types.local });
    this.RemoverProdutosClienteCarregamento = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.RemoverProdutosSelecionados, visible: ko.observable(false) });
    this.EditarProdutoCarregamento = PropertyEntity({ eventClick: editarProdutoCarregamentoClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.EditarProdutosEntreCarregamentos, visible: ko.observable(true) });
    this.ClienteCarregamentoSelect = PropertyEntity({ val: ko.observable(''), options: ko.observable(new Array()), def: '', text: Localization.Resources.Cargas.MontagemCargaMapa.Cliente.getFieldDescription(), required: true, visible: true, eventChange: changeAbaClienteProdutosCarregamentoSelect });

    this.PesoLoja = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Peso.getFieldDescription(), val: ko.observable("") });
    this.PalletsLoja = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Pallets.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.CubagemLoja = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    this.Peso = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Peso.getFieldDescription(), val: ko.observable("") });
    this.Pallets = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Pallets.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Cubagem = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.PesoPallet = PropertyEntity({ id: guid(), text: Localization.Resources.Cargas.MontagemCargaMapa.PesoPallet.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    this.CapacidadePeso = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadePallets = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CapacidadeCubagem = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.OcupacaoPeso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoPeso.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.OcupacaoPallets = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoPallets.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.OcupacaoCubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoCubagem.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.OcupacaoPesoPallet = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.OcupacaoPesoPallet.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    //Canal de entrega dos pedidos
    this.TiposDePedidos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });

    this.ProgressPeso = PropertyEntity({ id: guid() });
    this.ProgressPallets = PropertyEntity({ id: guid() });
    this.ProgressCubagem = PropertyEntity({ id: guid() });
    this.ProgressPesoPallet = PropertyEntity({ id: guid() });

    this.ModeloVeicularCarga = PropertyEntity({ type: types.local });
    this.TipoDeCarga = PropertyEntity({ type: types.local });
}

var FiltroProdutosNaoAtendidos = function () {
    this.SessaoRoteirizador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cliente = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BasicDataTable = PropertyEntity({ val: true, def: true, getType: typesKnockout.bool });
    // ProdutosNaoAtendido
}

var ProdutosNaoAtendido = function () {
    this.SelectClientes = PropertyEntity({ val: ko.observable(0), options: ko.observable(new Array()), def: 0, text: Localization.Resources.Cargas.MontagemCargaMapa.Cliente.getFieldDescription(), required: true, visible: true, eventChange: changeSelectCliente });
    this.PesquisarNaoAtendidos = PropertyEntity({ eventClick: reloadGridProdutosNaoAtendidos, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar });
    this.Grid = PropertyEntity({ type: types.local });
}

function changeSelectCarregamentos() {
    _pesquisaProdutoOutroCarregamento.Carregamento.val(_outroCarregamentoProdutos.SelectCarregamentos.val());
}

function changeAbaClienteProdutosCarregamentoSelect() {
    const valoresCarregamentoCNPJ = _carregamentoProdutos.ClienteCarregamentoSelect.val();

    if (valoresCarregamentoCNPJ) {
        const cod_carregamento = parseInt(valoresCarregamentoCNPJ.split("-")[1]);
        const cpf_cnpj = valoresCarregamentoCNPJ.split("-")[0];
        _pesquisaProdutoCarregamento.Carregamento.val(cod_carregamento);
        _pesquisaProdutoCarregamento.Cliente.val(cpf_cnpj);
        reloadGridMotagemCarregamentoProdutos();
    }

    if (valoresCarregamentoCNPJ == null && _carregamentoProdutos.ClientesCarregamento.val()) {
        BuscarTodosProdutosCarregamento();
    }

}

function BuscarTodosProdutosCarregamento() {
    let dataProdutos = [];
    for (const element of _carregamentoProdutos.ClientesCarregamento.val()) {
        const data = {
            Carregamento: element.carregamento,
            Cliente: element.value
        };
        executarReST("MontagemCargaPedido/ProdutosCarregamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data.length > 0) {
                    dataProdutos = dataProdutos.concat(arg.Data);
                    _gridProdutosCarregamento.CarregarGrid(dataProdutos);
                    _carregamentoProdutos.RemoverProdutosClienteCarregamento.visible(false);
                }
                progressPercentualCarregamentoProdutos(null, false, data.Carregamento);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}



function changeSelectCliente() {
    _pesquisaProtudosNaoAtendido.Cliente.val(_produtosNaoAtendidos.SelectClientes.val());
}

function pesquisarOutroCarregamentoClick() {
    var carregamento = { Codigo: _pesquisaProdutoOutroCarregamento.Carregamento.val() };
    executarReST("MontagemCarga/BuscarPorCodigo", carregamento, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var carregamento = arg.Data;

                //Ajustando os tipos de pedidos do carregamento
                if (carregamento != null) {
                    let clientes = [];
                    if (carregamento.ClientesCarregamento != null) {
                        for (var i = 0; i < carregamento.ClientesCarregamento.length; i++) {
                            clientes.push({ active: (i === 0 ? 'nav-link active' : 'nav-link'), icon: 'fal fa-lg fa-list', text: carregamento.ClientesCarregamento[i].Nome, value: carregamento.ClientesCarregamento[i].CPF_CNPJ, carregamento: carregamento.Carregamento.Carregamento.Codigo });
                        }
                    }
                    _outroCarregamentoProdutos.ClientesCarregamento.val(clientes);
                    if (clientes.length > 0) {
                        selecionouAbaClienteProdutosOutroCarregamento(clientes[0]);
                    }

                    progressPercentualCarregamentoProdutos(carregamento, true, carregamento.Carregamento.Carregamento.Codigo);
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }

            reloadGridProdutosNaoAtendidos();

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function progressPercentualCarregamentoProdutos(carregamento, outro, codigoCarregamento) {
    var dadosModeloVeicularCarga = null;
    var dadosTipoDeCarga = null;

    if (carregamento != null) {
        dadosModeloVeicularCarga = carregamento.Carregamento.ModeloVeicularCarga;
        dadosTipoDeCarga = carregamento.Transporte.TipoDeCarga;
    }
    else if (outro == false) {
        dadosModeloVeicularCarga = _carregamentoProdutos.ModeloVeicularCarga.val();
        dadosTipoDeCarga = _carregamentoProdutos.TipoDeCarga.val();
    }
    else {
        dadosModeloVeicularCarga = _outroCarregamentoProdutos.ModeloVeicularCarga.val();
        dadosTipoDeCarga = _outroCarregamentoProdutos.TipoDeCarga.val();
    }

    if (dadosModeloVeicularCarga == null || dadosModeloVeicularCarga == "")
        return;

    var toleranciaPesoMenor = Globalize.parseFloat(dadosModeloVeicularCarga.ToleranciaPesoMenor);
    var toleranciaMinimaPaletes = Globalize.parseFloat(dadosModeloVeicularCarga.ToleranciaMinimaPaletes);
    var toleranciaMinimaCubagem = Globalize.parseFloat(dadosModeloVeicularCarga.ToleranciaMinimaCubagem);
    var capacidadePeso = Globalize.parseFloat(dadosModeloVeicularCarga.CapacidadePesoTransporte);
    var capacidadePallets = Globalize.parseFloat(dadosModeloVeicularCarga.NumeroPaletes);
    var capacidadeCubagem = Globalize.parseFloat(dadosModeloVeicularCarga.Cubagem);
    var totalizadoresPedidos = null;

    if (carregamento != null)
        totalizadoresPedidos = totalizadoresPedidosCarregamento(carregamento.Carregamento.Pedidos);
    else if (outro == false)
        totalizadoresPedidos = CarregarPedidoProtudosCarragmento(_gridProdutosCarregamento.BuscarRegistros()); //.GridViewTableData());
    else
        totalizadoresPedidos = CarregarPedidoProtudosCarragmento(_gridProdutosOutroCarregamento.BuscarRegistros()); //.GridViewTableData());

    var peso = totalizadoresPedidos.peso;
    var pesoPallet = totalizadoresPedidos.pesoPallet;
    var cubagem = totalizadoresPedidos.cubagem;
    var pallets = totalizadoresPedidos.pallets;

    if (outro == false) {
        _carregamentoProdutos.CapacidadePeso.val(capacidadePeso);
        _carregamentoProdutos.CapacidadePallets.val(capacidadePallets);
        _carregamentoProdutos.CapacidadeCubagem.val(capacidadeCubagem);
        _carregamentoProdutos.ModeloVeicularCarga.val(dadosModeloVeicularCarga);
        _carregamentoProdutos.TipoDeCarga.val(dadosTipoDeCarga);

        if (dadosModeloVeicularCarga.ModeloControlaCubagem)
            _carregamentoProdutos.Cubagem.visible(true);
        else
            _carregamentoProdutos.Cubagem.visible(false);
        if (dadosModeloVeicularCarga.VeiculoPaletizado)
            _carregamentoProdutos.Pallets.visible(true);
        else
            _carregamentoProdutos.Pallets.visible(false);

        _carregamentoProdutos.PesoPallet.visible(_sessaoRoteirizador.ConsiderarPesoPalletPesoTotalCarga.val());

        _carregamentoProdutos.Peso.val(Globalize.format(peso, "n4"));
        _carregamentoProdutos.PesoPallet.val(Globalize.format(pesoPallet, "n4"));
        _carregamentoProdutos.Cubagem.val(Globalize.format(cubagem, "n2"));
        _carregamentoProdutos.Pallets.val(Globalize.format(pallets, "n2"));

        _carregamento.Peso.val(Globalize.format(peso, "n4"));
        _carregamento.PesoPallet.val(Globalize.format(pesoPallet, "n4"));
        _carregamento.Cubagem.val(Globalize.format(cubagem + Globalize.parseFloat(_carregamento.CubagemPaletes.val()), "n2"));
        _carregamento.Pallets.val(Globalize.format(pallets, "n2"));

        if (capacidadePeso > 0) {
            var cor = corOcupacao(peso, capacidadePeso, toleranciaPesoMenor);
            _carregamentoProdutos.OcupacaoPeso.val((peso * 100) / capacidadePeso);
            $("#" + _carregamentoProdutos.ProgressPeso.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _carregamentoProdutos.Peso.id).css("color", '#666');
        }

        if (capacidadePeso > 0) {
            var cor = corOcupacao(peso + pesoPallet, capacidadePeso, toleranciaPesoMenor);
            _carregamentoProdutos.OcupacaoPesoPallet.val(((peso + pesoPallet) * 100) / capacidadePeso);
            $("#" + _carregamentoProdutos.ProgressPesoPallet.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _carregamentoProdutos.PesoPallet.id).css("color", '#666');
        }

        if (capacidadePallets > 0) {
            var cor = corOcupacao(pallets, capacidadePallets, toleranciaMinimaPaletes);
            _carregamentoProdutos.OcupacaoPallets.val((pallets * 100) / capacidadePallets);
            $("#" + _carregamentoProdutos.ProgressPallets.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _carregamentoProdutos.Pallets.id).css("color", '#666');
        }

        if (capacidadeCubagem > 0) {
            var cor = corOcupacao(cubagem, capacidadeCubagem, toleranciaMinimaCubagem);
            _carregamentoProdutos.OcupacaoCubagem.val((cubagem * 100) / capacidadeCubagem);
            $("#" + _carregamentoProdutos.ProgressCubagem.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _carregamentoProdutos.Cubagem.id).css("color", '#666');
        }

    }
    else {
        _outroCarregamentoProdutos.CapacidadePeso.val(capacidadePeso);
        _outroCarregamentoProdutos.CapacidadePallets.val(capacidadePallets);
        _outroCarregamentoProdutos.CapacidadeCubagem.val(capacidadeCubagem);
        _outroCarregamentoProdutos.ModeloVeicularCarga.val(dadosModeloVeicularCarga);
        _outroCarregamentoProdutos.TipoDeCarga.val(dadosTipoDeCarga);

        if (dadosModeloVeicularCarga.ModeloControlaCubagem)
            _outroCarregamentoProdutos.Cubagem.visible(true);
        else
            _outroCarregamentoProdutos.Cubagem.visible(false);
        if (dadosModeloVeicularCarga.VeiculoPaletizado)
            _outroCarregamentoProdutos.Pallets.visible(true);
        else
            _outroCarregamentoProdutos.Pallets.visible(false);

        _outroCarregamentoProdutos.Peso.val(Globalize.format(peso, "n4"));
        _outroCarregamentoProdutos.Cubagem.val(Globalize.format(cubagem, "n2"));
        _outroCarregamentoProdutos.Pallets.val(Globalize.format(pallets, "n2"));

        if (capacidadePeso > 0) {
            var cor = corOcupacao(peso, capacidadePeso, toleranciaPesoMenor);
            _outroCarregamentoProdutos.OcupacaoPeso.val((peso * 100) / capacidadePeso);
            $("#" + _outroCarregamentoProdutos.ProgressPeso.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _outroCarregamentoProdutos.Peso.id).css("color", '#666');
        }

        if (capacidadePallets > 0) {
            var cor = corOcupacao(pallets, capacidadePallets, toleranciaMinimaPaletes);
            _outroCarregamentoProdutos.OcupacaoPallets.val((pallets * 100) / capacidadePallets);
            $("#" + _outroCarregamentoProdutos.ProgressPallets.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _outroCarregamentoProdutos.Pallets.id).css("color", '#666');
        }

        if (capacidadeCubagem > 0) {
            var cor = corOcupacao(cubagem, capacidadeCubagem, toleranciaMinimaCubagem);
            _outroCarregamentoProdutos.OcupacaoCubagem.val((cubagem * 100) / capacidadeCubagem);
            $("#" + _outroCarregamentoProdutos.ProgressCubagem.id).css("background-color", cor);
            if (cor == _corExcedida)
                $("#" + _outroCarregamentoProdutos.Cubagem.id).css("color", '#666');
        }
    }

    if (codigoCarregamento > 0) {
        //ajustarCapacidadesKnoutCarregamento(codigoCarregamento, dadosModeloVeicularCarga, dadosTipoDeCarga, peso, cubagem, pallets, 0, 0, false, null);
    }
}

var _corAprovado = "#9dde88ad";
var _corReprovado = "#FF6347";
var _corExcedida = "#FFFF00";

function corOcupacao(valor, capacidade, toleranciaMenor) {
    var cor = "";
    if (valor > 0) {
        if (valor >= toleranciaMenor)
            cor = (valor > capacidade) ? _corExcedida : _corAprovado
        else
            cor = _corReprovado;
    }
    return cor;
}

function editarProdutoCarregamentoClick(refresh) {
    //Popular o selectCarregamentos
    if (_Carregamentos != null) {
        let options = [];
        for (let i = 0; i < _Carregamentos.length; i++) {
            if (_Carregamentos[i].Codigo != _pesquisaProdutoCarregamento.Carregamento.val() && _Carregamentos[i].GerandoCargaBackground == false) {
                let filial = '';
                if (_Carregamentos[i].Filial != null)
                    filial = ' - ' + _Carregamentos[i].Filial;
                options.push({
                    text: _Carregamentos[i].NumeroCarregamento + filial,
                    value: _Carregamentos[i].Codigo
                });
            }
        }
        _outroCarregamentoProdutos.SelectCarregamentos.options(options);
    }
    //Agora os clientes da sessão
    if (_sessaoRoteirizador != null) {
        let options = [];
        const destinatarios = _sessaoRoteirizador.Destinatarios.val();
        for (let i = 0; i < destinatarios.length; i++) {
            options.push({
                text: destinatarios[i].Nome,
                value: destinatarios[i].CPF_CNPJ
            });
        }
        _produtosNaoAtendidos.SelectClientes.options(options);
    }

    pesquisarOutroCarregamentoClick();

    if (refresh != true) {
        modalOutroCarregamento();
    }
}

function removerProdutosClienteCarregamentoClick(outro) {
    var codCarregamento = 0;
    var cpf_cnpj = 0;
    var codigos = [];
    var carregamento = null;
    if (outro) {
        codCarregamento = _pesquisaProdutoOutroCarregamento.Carregamento.val();
        cpf_cnpj = _pesquisaProdutoOutroCarregamento.Cliente.val();
        codigos = _gridProdutosOutroCarregamento.ListaSelecionados().map(function (pedidoProdutoCarregamento) {
            return pedidoProdutoCarregamento.Codigo;
        });
    } else {
        codCarregamento = _carregamento.Carregamento.codEntity();
        cpf_cnpj = _pesquisaProdutoCarregamento.Cliente.val();
        codigos = _gridProdutosCarregamento.ListaSelecionados().map(function (pedidoProdutoCarregamento) {
            return pedidoProdutoCarregamento.Codigo;
        });
    }

    if (codigos.length == 0) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.MontagemCargaMapa.NenhumProdutoSelecionadoParaRemover);
        return;
    }

    var c = codigos.length;

    if (codCarregamento > 0 && cpf_cnpj > 0 && c > 0) {

        var dados = {
            Carregamento: codCarregamento,
            Cliente: cpf_cnpj,
            Codigos: JSON.stringify(codigos)
        }

        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaRemoverOsProdutosSelecionadosDoClienteDoCarregamento.format((c > 1 ? c : "")), function () {
            executarReST("MontagemCarga/RemoverProdutosCarregamento", dados, function (arg) {
                if (arg.Success) {

                    if (arg.Data === false) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    }
                    // Precisamos reconsultar o carregamento.
                    if (outro == false) {
                        reloadGridMotagemCarregamentoProdutos();
                        retornoRemoveuProdutoPedidoCarregamento(codCarregamento, arg.Data);
                    } else {
                        reloadGridMotagemOutroCarregamentoProdutos();
                        pesquisarOutroCarregamentoClick();
                    }
                    reloadGridProdutosNaoAtendidos();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            }, null);
        });
    }
}

function retornoRemoveuProdutoPedidoCarregamento(codCarregamento, codigosPedido) {
    //retornoCarregamento({ Codigo: codCarregamento });
    //Se removeu algum pedido do carregamento.
    if (codigosPedido.length > 0) {

        removerCarregamentoPedidos(codCarregamento, codigosPedido)

        EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;

        for (var i in codigosPedido) {
            var codigoPedido = codigosPedido[i];
            var pedido = getPedidosPorCodigo(codigoPedido);
            SelecionarPedido(pedido, true);
            //Remove o pedido do carregamento..
            _carregamento.Pedidos.val(_carregamento.Pedidos.val().filter(function (item) {
                return item.Codigo !== codigoPedido
            }));
            //Limpar o marker
            var marker = getMarkerFromPedido(codigoPedido);
            if (marker != null) {
                marker.codigo_carregamento = 0;
                var objIconSet = null;
                if (marker.distribuidor) {
                    objIconSet = GetIconMarker(_EnumMarker.Distribuidor);
                } else {
                    objIconSet = GetIconMarker(_EnumMarker.Pin);
                }
                marker.marker.setIcon(objIconSet);
            }
        }
        //Atualiza grid pedidos selecionados carregamento.
        RenderizarGridMotagemPedidos();
        EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;

        drawPolylineCarregamentos();
    }
}

function modalOutroCarregamento() {
    var effect = 'slide';
    var options = { direction: "left" };
    var duration = 500;
    //$('#modal-outro-carregamento').toggle(effect, options, duration);
    ////$('#modal-outro-carregamento').slideToggle("slow");
    $("#modal-outro-carregamento").toggle(500, "swing");
}

function ocultarModalOutroCarregamento() {
    if (modalOutroCarregamentoVisible()) {
        modalOutroCarregamento();
    }
}

function modalOutroCarregamentoVisible() {
    return $('#modal-outro-carregamento').is(":visible");
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCarregamentoProdutos() {

    //Filtro de pesquisa para produtos do carregamento.
    _pesquisaProdutoCarregamento = new FiltroPesquisa();
    _pesquisaProdutoOutroCarregamento = new FiltroPesquisa();
    _pesquisaProtudosNaoAtendido = new FiltroProdutosNaoAtendidos();

    _carregamentoProdutos = new CarregamentoProduto(false);
    KoBindings(_carregamentoProdutos, "knoutCarregamentoProdutos");
    loadGridProdutosCarregamento(false);

    _outroCarregamentoProdutos = new CarregamentoProduto(true);
    KoBindings(_outroCarregamentoProdutos, "knoutOutroCarregamentoProdutos");
    loadGridProdutosCarregamento(true);

    _produtosNaoAtendidos = new ProdutosNaoAtendido();
    KoBindings(_produtosNaoAtendidos, "knoutProdutosNaoAtendido");
    loadGridProdutosNaoAtendido();

    loadDetalhePedidoProdutoCarregamento();

    loadDroppable();
}

function limparCarregamentoProdutos(outro) {
    if (outro) {
        _pesquisaProdutoOutroCarregamento.Carregamento.val(0);
        LimparCampos(_outroCarregamentoProdutos);
        reloadGridMotagemOutroCarregamentoProdutos();
    } else {
        _pesquisaProdutoCarregamento.Carregamento.val(0);
        LimparCampos(_carregamentoProdutos);
        reloadGridMotagemCarregamentoProdutos();
    }
}

function callbackCellEditableQtde(dataRow) {
    //if (dataRow.PalletFechado.toUpperCase() === 'NÃO')
    //if (dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Nao)
    //if (dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim)
    //    return true;
    //if (dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim)
    //    return true;
    if (_sessaoRoteirizador.TipoEdicaoPalletProdutoMontagemCarregamento.val() == EnumTipoEdicaoPalletProdutoMontagemCarregamento.EdicaoPermitida)
        return true;
    else if (_sessaoRoteirizador.TipoEdicaoPalletProdutoMontagemCarregamento.val() == EnumTipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado && dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim)
        return true;
    // Pallet fechado = False || EnumTipoEdicaoPalletProdutoMontagemCarregamento.EdicaoNaoPermitida
    return false;
}

function callbackCellEditablePallet(dataRow) {
    //if (dataRow.PalletFechado.toUpperCase() === 'SIM')
    // #51793 solicitação do Sidney, quando Pallet Fechado = Não não pode deixar alterar na coluna quantidade.
    // Porem em contato com o Joel, pois o comentário dele foi diferente do que o Sidnei printou.... somente pode mexer quando pallet Fechado = Não.
    // Agora vamos obter da configuração do centro de carregamento...
    if (_sessaoRoteirizador.TipoEdicaoPalletProdutoMontagemCarregamento.val() == EnumTipoEdicaoPalletProdutoMontagemCarregamento.EdicaoPermitida)
        return true;
    else if (_sessaoRoteirizador.TipoEdicaoPalletProdutoMontagemCarregamento.val() == EnumTipoEdicaoPalletProdutoMontagemCarregamento.ControlePalletAbertoFechado && dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim)
        return true;
    // Pallet fechado = False || EnumTipoEdicaoPalletProdutoMontagemCarregamento.EdicaoNaoPermitida
    return false;
}

function loadGridProdutosCarregamento(outro) {
    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesPedidoProdutoClick, icone: "" };
    const opcaoDetalhesPedido = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.DetalhesPedido, id: guid(), metodo: detalhesPedidoClick, icone: "" };
    const opcaoExcluirProduto = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.RemoverProduto, id: guid(), metodo: removerProdutoClick, icone: "" };
    const opcaoExcluirProdutoOutro = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.RemoverProduto, id: guid(), metodo: removerProdutoOutroClick, icone: "" };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoDetalhes, opcaoDetalhesPedido, opcaoExcluirProduto] };
    const menuOpcoesOutro = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [opcaoDetalhes, opcaoDetalhesPedido, opcaoExcluirProdutoOutro] };

    const editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: (outro == false ? AtualizarPedidoProdutoCarregamento : AtualizarPedidoProdutoCarregamentoOutro)
    };

    const editableQtde = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        maxlength: 0,
        numberMask: ConfigDecimal(),
        conditions: callbackCellEditableQtde
    };

    const editablePallet = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        maxlength: 0,
        numberMask: ConfigInt({ thousands: "" }),
        conditions: callbackCellEditablePallet
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "CodigoPedido", visible: false },
        { data: "CodigoPedidoProduto", visible: false },
        { data: "Cliente", visible: false },
        { data: "Produto", title: Localization.Resources.Cargas.MontagemCargaMapa.Produto, width: "20%", widthDefault: "20%", filter: true, permiteEsconderColuna: false },
        { data: "Categoria", title: Localization.Resources.Cargas.MontagemCargaMapa.Categoria, width: "15%", widthDefault: "15%", filter: true, permiteEsconderColuna: true },
        { data: "LinhaSeparacao", title: Localization.Resources.Cargas.MontagemCargaMapa.LinhaSeparacao, width: "15%", widthDefault: "15%", filter: true, permiteEsconderColuna: true },
        { data: "Pedido", title: Localization.Resources.Cargas.MontagemCargaMapa.Pedido, width: "10%", widthDefault: "10%", filter: true, permiteEsconderColuna: true },
        { data: "Qtde", visible: false },
        { data: "QtdeOriginal", title: Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeAnterior, width: "10%", widthDefault: "10%", permiteEsconderColuna: true },
        { data: "QtdeCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Quantidade, width: "10%", widthDefault: "10%", editableCell: editableQtde, permiteEsconderColuna: true },
        { data: "Peso", visible: false },
        { data: "PesoCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "10%", widthDefault: "10%", permiteEsconderColuna: true },
        { data: "Pallet", visible: false },
        { data: "PalletCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Pallet, width: "10%", widthDefault: "10%", editableCell: editablePallet, permiteEsconderColuna: true },
        { data: "Metro", visible: false },
        { data: "MetroCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Metro, width: "10%", widthDefault: "10%", permiteEsconderColuna: true },
        { data: "PalletFechado", title: Localization.Resources.Cargas.MontagemCargaMapa.PalletFechado, width: "5%", widthDefault: "5%", permiteEsconderColuna: true },
        { data: "QuantidadeCaixaPorPallet", visible: false }
    ];

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };

    if (outro == false) {
        _gridProdutosCarregamento = new BasicDataTable(_carregamentoProdutos.Grid.id, header, menuOpcoes, null, configRowsSelect, null, null, null, editarColuna, null, null, null, null, callbackRowProdutoCarregamento, null, true);
        _gridProdutosCarregamento.SetPermitirEdicaoColunas(true)
        reloadGridMotagemCarregamentoProdutos();
    } else {
        _gridProdutosOutroCarregamento = new BasicDataTable(_outroCarregamentoProdutos.Grid.id, header, menuOpcoesOutro, null, configRowsSelect, null, null, null, editarColuna, null, null, null, null, callbackRowProdutoCarregamentoOutro, null, true);
        reloadGridMotagemOutroCarregamentoProdutos();
    }
}

function loadGridProdutosNaoAtendido() {
    if (_pesquisaProtudosNaoAtendido.BasicDataTable.val == false) {

        var quantidadePorPagina = 1000;
        _gridProdutosNaoAtendidos = new GridView(_produtosNaoAtendidos.Grid.id, "MontagemCargaPedido/ProdutosNaoAtendido", _pesquisaProtudosNaoAtendido, null, null, 5, null, null, null, null, quantidadePorPagina, null, null, null, null, callbackRowProdutosNaoAtendidos);

    } else {

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoPedido", visible: false },
            { data: "CodigoPedidoProduto", visible: false },
            { data: "Cliente", visible: false },
            { data: "Produto", title: Localization.Resources.Cargas.MontagemCargaMapa.Produto, width: "20%", filter: true },
            { data: "Categoria", title: Localization.Resources.Cargas.MontagemCargaMapa.Categoria, width: "15%", filter: true },
            { data: "LinhaSeparacao", title: Localization.Resources.Cargas.MontagemCargaMapa.LinhaSeparacao, width: "15%", filter: true },
            { data: "Pedido", title: Localization.Resources.Cargas.MontagemCargaMapa.Pedido, width: "10%", filter: true },
            { data: "Qtde", visible: false },
            { data: "QtdeCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Quantidade, width: "10%" },
            { data: "Peso", visible: false },
            { data: "PesoCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Peso, width: "10%" },
            { data: "Pallet", visible: false },
            { data: "PalletCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Pallet, width: "10%" },
            { data: "Metro", visible: false },
            { data: "MetroCarregar", title: Localization.Resources.Cargas.MontagemCargaMapa.Metro, width: "10%" }
        ];

        var configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };
        _gridProdutosNaoAtendidos = new BasicDataTable(_produtosNaoAtendidos.Grid.id, header, null, null, configRowsSelect, 5, null, null, null, null, null, null, null, callbackRowProdutosNaoAtendidos);

    }
    reloadGridProdutosNaoAtendidos();
}

function callbackRowProdutoCarregamento(nRow, aData) {
    $(_carregamentoProdutos.Grid.id).addClass("tableCursorMove");
    $(nRow).draggable(obterObjetoDragglable("remocao-item-produto-carregamento"));
}

function callbackRowProdutoCarregamentoOutro(nRow, aData) {
    $(_outroCarregamentoProdutos.Grid.id).addClass("tableCursorMove");
    $(nRow).draggable(obterObjetoDragglable("remocao-item-produto-outro-carregamento"));
}

function callbackRowProdutosNaoAtendidos(nRow, aData) {
    $(_produtosNaoAtendidos.Grid.id).addClass("tableCursorMove");
    $(nRow).draggable(obterObjetoDragglable(null));
}

function obterObjetoDragglable(idContainerRemocaoItem) {
    return {
        cursor: "move",
        zIndex: 99999,
        appendTo: '#widget-grid',
        helper: function (event) {
            _row = event;
            event.preventDefault();

            var html = '';

            $(event.currentTarget).children().each(function (i, coluna) {
                var $coluna = $(coluna);
                html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
            });

            var corLinha = $(event.currentTarget).css("background-color");
            var classLinha = $(event.currentTarget).attr("class");
            var corLinhaSelecionada = "#ecf3f8";
            var coresLinhaPadrao = ["#ffffff", "#F9F9F9"];

            var isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

            var tr = '<table style="font-size: 10px; border: solid 2px #57889c; cursor: move; z-index: 99999; position: absolute; overflow-anchor: visible; width: ' + $(event.currentTarget).width() +
                'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';" class="table table-bordered table-hover table-condensed table-striped no-footer dataTable JColResizer"><tbody><tr class="' + classLinha + '">' + html + '</tr></tbody></table>';

            return tr;
        },
        revert: 'invalid',
        start: function () {
            if (idContainerRemocaoItem != null) {
                $("#" + idContainerRemocaoItem).show();
            }
        },
        stop: function () {
            if (idContainerRemocaoItem != null)
                $("#" + idContainerRemocaoItem).hide();
        }
    };
}

/* */
function loadDroppable() {

    $("#remocao-item-produto-carregamento").droppable({
        drop: function (event, ui) {
            var idGridOrigem = $(ui.draggable[0]).parent().parent()[0].id;
        },
        hoverClass: "remocao-item-container-hover",
    });

    $("#remocao-item-produto-outro-carregamento").droppable({
        drop: function (event, ui) {
            var idGridOrigem = $(ui.draggable[0]).parent().parent()[0].id;
            //if (idGridOrigem == "grid-fila-motorista")
            //    removerFilaCarregamentoMotorista(ui.draggable[0].id);
        },
        hoverClass: "remocao-item-container-hover",
    });

    loadDroppableAlteracaoProdutoCarregamento();
    loadDroppableRemocaoProdutoCarregamento();
}

function loadDroppableAlteracaoProdutoCarregamento() {
    $("#container-grid-produtos-carregamento, #container-grid-produtos-outro-carregamento, #container-grid-produtos-nao-atendido").droppable({
        drop: itemSoltado,
        hoverClass: "ui-state-active backgroundDropHover",
    });
}

function obterCodigosPedidoProdutoCarregamentoSelecionados(outro) {
    var codigos = [];
    if (outro) {
        codigos = _gridProdutosOutroCarregamento.ListaSelecionados().map(function (pedidoProdutoCarregamento) {
            return pedidoProdutoCarregamento.Codigo;
        });
    } else {
        codigos = _gridProdutosCarregamento.ListaSelecionados().map(function (pedidoProdutoCarregamento) {
            return pedidoProdutoCarregamento.Codigo;
        });
    }
    return JSON.stringify(codigos);
}

function loadDroppableRemocaoProdutoCarregamento() {
    $("#remocao-item-produto-carregamento, #remocao-item-produto-outro-carregamento").droppable({
        drop: function (event, ui) {
            var idGridOrigem = $(ui.draggable[0]).parent().parent()[0].id;
            var codCarregamento = _pesquisaProdutoCarregamento.Carregamento.val();
            var codPedidoProdutoCarregamento = ui.draggable[0].id;
            var outro = false;
            if (idGridOrigem == _outroCarregamentoProdutos.Grid.id) {
                codCarregamento = _pesquisaProdutoOutroCarregamento.Carregamento.val();
                outro = true;
            }

            var dados = {
                Carregamento: codCarregamento,
                Codigo: 0,
                CodigoPedidoProdutoCarregamento: codPedidoProdutoCarregamento,
                CodigosPedidoProdutoCarregamento: obterCodigosPedidoProdutoCarregamentoSelecionados(outro),
                Peso: 0,
                Qtde: 0,
                Pallet: 0,
                Cubico: 0,
                Operacao: 0, // Atualiza ou remove o produto do carregamento
                Alterando: 0
            };
            AlterarPedidoProdutoCarregamento(dados, outro, false, false);
        },
        hoverClass: "remocao-item-container-hover",
    });
}

var _container_grid_carregamento = "container-grid-produtos-carregamento";
var _container_grid_outro_carregamento = "container-grid-produtos-outro-carregamento";
var _container_grid_nao_atendidos = 'container-grid-produtos-nao-atendido';


function itemSoltado(event, ui) {
    var idContainerDestino = event.target.id;

    var idContainerOrigem = $(ui.draggable[0]).parent().parent().parent().parent().parent().parent().parent()[0].id; //$(ui.draggable[0]).parent().parent().parent().parent()[0].id; //$(ui.draggable[0]).parent().parent()[0].id;

    if (idContainerOrigem !== idContainerDestino) {

        var codCarregamento = _pesquisaProdutoCarregamento.Carregamento.val();
        var codPedidoProdutoCarregamento = ui.draggable[0].id;
        var codPedidoProduto = 0;
        var outro = false;
        var reconsultarCarregamento = false;
        var peso, qtde, pallet, cubico = 0;

        if (idContainerDestino == _container_grid_outro_carregamento) {
            codCarregamento = _pesquisaProdutoOutroCarregamento.Carregamento.val();
            outro = true;
        }

        //Contem todos os códigos selecionados no basicTable dos carregementosPedidoProdutos.
        var codigos = [];
        var codigosPedidosProdutos = []
        var operacao = 1; // Troca de produtos entre carregamentos.

        //Remove o pedido/produto do carregamento.
        if (idContainerDestino == _container_grid_nao_atendidos) {

            operacao = 0; // Remove o produto do carregamento controlado pelo "Peso"
            if (idContainerOrigem == _container_grid_outro_carregamento) {
                codCarregamento = _pesquisaProdutoOutroCarregamento.Carregamento.val();
                outro = true;
                codigos = obterCodigosPedidoProdutoCarregamentoSelecionados(true);
            } else {
                codigos = obterCodigosPedidoProdutoCarregamentoSelecionados(false);
            }

        } else if (idContainerOrigem == _container_grid_nao_atendidos) {

            codPedidoProduto = codPedidoProdutoCarregamento;
            codPedidoProdutoCarregamento = 0;
            operacao = 2;

            if (_pesquisaProtudosNaoAtendido.BasicDataTable.val == false) {

                var itensNaoAtendido = _gridProdutosNaoAtendidos.GridViewTableData();
                var index = -1;
                if (!NavegadorIEInferiorVersao12()) {
                    index = itensNaoAtendido.findIndex(function (item) { return item.Codigo == codPedidoProduto });
                } else {
                    for (var i = 0; i < itensNaoAtendido.length; i++) {
                        if (codigo == itensNaoAtendido[i].Codigo)
                            index = i;
                    }
                }
                if (index >= 0) {
                    peso = itensNaoAtendido[index].PesoCarregar;
                    qtde = itensNaoAtendido[index].QtdeCarregar;
                    pallet = itensNaoAtendido[index].PalletCarregar;
                    cubico = itensNaoAtendido[index].MetroCarregar;
                }

            } else {
                // Pega todos os Pedidos Produtos não atendidos selecionados.. e passa os códigos
                codigosPedidosProdutos = JSON.stringify(_gridProdutosNaoAtendidos.ListaSelecionados().map(function (pedidoProduto) {
                    return pedidoProduto.Codigo;
                }));
            }

        } else {
            codigos = obterCodigosPedidoProdutoCarregamentoSelecionados(idContainerOrigem == _container_grid_outro_carregamento);
        }

        var dados = {
            Carregamento: codCarregamento,
            Codigo: codPedidoProduto,
            CodigoPedidoProdutoCarregamento: codPedidoProdutoCarregamento,
            CodigosPedidoProdutoCarregamento: codigos,
            CodigosPedidosProdutos: codigosPedidosProdutos,
            Peso: peso,
            Qtde: qtde,
            Pallet: pallet,
            Cubico: cubico,
            Operacao: operacao,
            BasicDataTable: _pesquisaProtudosNaoAtendido.BasicDataTable.val,
            Alterando: 0
        };

        // Reconsultar ambos os pedidos produtos dos carregamentos;
        // Quando for troca entre as jaenlas do carregamentos vamos reconsultar.. quando origem or destino é o grid nao atendidos.. não precisamos reconsultar
        var ambos = true;
        if (idContainerOrigem == _container_grid_nao_atendidos || idContainerDestino == _container_grid_nao_atendidos) {
            ambos = false;
        }

        if ((idContainerDestino == _container_grid_carregamento || idContainerDestino == _container_grid_outro_carregamento) && idContainerOrigem != _container_grid_nao_atendidos) {
            reconsultarCarregamento = true;
        }

        AlterarPedidoProdutoCarregamento(dados, outro, ambos, reconsultarCarregamento);
    }
}

var _row;

function double(value) {
    return parseFloat(value.replace(".", "").replace(",", "."));
}

function toString(value) {
    if (isNaN(parseFloat(value)))
        return "";
    return value.toString().replace(",", "").replace(".", ",");
}

function AtualizarPedidoProdutoCarregamentoOutro(dataRow, row, head) {
    AtualizarPedidoProduto(dataRow, row, head, null, true);
}

function AtualizarPedidoProdutoCarregamento(dataRow, row, head) {
    AtualizarPedidoProduto(dataRow, row, head, null, false);
}

function AtualizarPedidoProduto(dataRow, row, head, callbackTabPress, outro) {

    let alterouPeso = false;
    let alterouQtde = false;
    let alterouPallet = false;
    let alterando = 1;

    //Alterando o peso
    if (head.data == 'QtdeCarregar') {
        alterouQtde = true;
    } else if (head.data == 'PesoCarregar') {
        alterouPeso = true;
        alterando = 2;
    } else if (head.data == 'PalletCarregar') {
        alterouPallet = true;
        alterando = 3;
    }

    let codCarregamento = 0;
    if (_carregamento != null) {
        codCarregamento = _carregamento.Carregamento.codEntity();
    }
    if (outro)
        codCarregamento = _pesquisaProdutoOutroCarregamento.Carregamento.val();

    if (codCarregamento > 0) {

        //const palletFechado = (dataRow.PalletFechado == Localization.Resources.Gerais.Geral.Sim ? true : false);
        const palletFechado = !callbackCellEditablePallet(dataRow); 
        let peso = double(dataRow.PesoCarregar);
        let qtde = double(dataRow.QtdeCarregar);
        let pallet = double(dataRow.PalletCarregar);
        let metro = double(dataRow.PalletCarregar);

        let pesoTotalPedidoProduto = dataRow.Peso;

        if (palletFechado && alterouPeso && (peso != pesoTotalPedidoProduto))
            return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAlterarPesoDeProdutosDePalletFechado, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);

        if (palletFechado && alterouQtde)
            return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.NaoPermitidoAlterarQuantidadeDeProdutosDePalletFechado, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);

        // ASSAI - NÃO EDITA PESO, somante quantidade e pallet quando pallet fechado...
        if (peso < 0)
            return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.PesoCarregarNaoPodeSerInferiorZero, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);

        else if (alterouPeso) { // recalcula a qtde

            if (dataRow.Qtde > 0) {
                const pesoUnitario = (dataRow.Peso / dataRow.Qtde);
                qtde = (peso / pesoUnitario);
            }

            // Pallet Fechado
            //  Ex: 1 pallet com 5 caixas... retira 1, continua com 1 pallet, altera a qtde e o peso.
            if (dataRow.Peso > 0 && !palletFechado) {
                //Quando não é pallet fechado, recalcula a qtde de pallet
                const palletPesoUnitario = (double(dataRow.Pallet) / dataRow.Peso);
                pallet = (peso * palletPesoUnitario);
            }

        } else if (alterouQtde) { // recalcula o peso e pallet quando não for pallet fechado.

            if (dataRow.Qtde > 0) {

                const pesoUnitario = (dataRow.Peso / dataRow.Qtde);
                peso = qtde * pesoUnitario;

                // Pallet Fechado
                //  Ex: 1 pallet com 5 caixas... retira 1, continua com 1 pallet, altera a qtde e o peso.
                if (!palletFechado) {
                    //Quando não é pallet fechado, recalcula a qtde de pallet
                    const palletUnitario = (double(dataRow.Pallet) / dataRow.Qtde);
                    pallet = qtde * palletUnitario;

                    const metroUnitario = (dataRow.Metro / dataRow.Qtde);
                    metro = qtde * metroUnitario;
                }

            }

        }
        else if (alterouPallet) {

            //Quando pallet fechado.. o valor quebrado tem que ser inteiro...
            const totalPallet = double(dataRow.Pallet);
            if (totalPallet <= 0) {
                return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeTotalDePalletFechadoDoProdutoInvalida, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);
            }
            let int = parseInt(pallet);
            if (int <= 0) {
                return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeDePalletFechadoDeveSerUmNumeroInteiroMaiorQueZero, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);
            } else if (int > totalPallet) {
                return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.QuantidadeDePalletFechadoNaoPodeSerSuperior.format(totalPallet), tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);
            }

            let pesoTotal = dataRow.Peso;
            let qtdeTotal = dataRow.Qtde;
            let metroTotal = dataRow.Metro;

            peso = (int / totalPallet) * pesoTotal;

            // Comentado o código abaixo, pois apresenta divergência..
            //  Ex: Pedido homolog 337792, Qtde produto 175, pallet 2,917 o que totaliza 60 por pallet sendo que no cadasto do produto embarcador está 72 caixa por pallet.

            //Se no produto embarcador, possuir a quantidade de caixa por pallet, vamos calcular com base nela.
            if (dataRow.QuantidadeCaixaPorPallet > 0) {
                qtde = Math.round(dataRow.QuantidadeCaixaPorPallet * int);
            } else {
                qtde = Math.round((int / totalPallet) * qtdeTotal);
            }

            pallet = int;
            metro = (int / totalPallet) * metroTotal;

        }

        const dados = {
            Carregamento: codCarregamento,
            Codigo: parseInt(dataRow.CodigoPedidoProduto),
            CodigoPedidoProdutoCarregamento: 0,
            CodigosPedidoProdutoCarregamento: [],
            Peso: toString(peso),
            Qtde: toString(qtde),
            Pallet: toString(pallet),
            Cubico: toString(metro), //toString(dataRow.Metro),
            Operacao: 0, // Atualiza/remove produto do carregamento
            Alterando: alterando
        };

        AlterarPedidoProdutoCarregamento(dados, outro, false, false);

    } else {
        return ExibirErroRow(row, Localization.Resources.Cargas.MontagemCargaMapa.PrimeiroSalveCarregamentoParaDepoisConfigurarOsProdutosDoCarregamento, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, outro);
    }
}
function retornoAlterarPedidoProduto(dados) {
    if (dados == false)
        return;

    //Selecionar
    for (let i = 0; i < dados.adicionados.length; i++)
        SelecionarPedido({ Codigo: dados.adicionados[i] }, false);

    //Remover seleção
    for (let i = 0; i < dados.removidos.length; i++)
        SelecionarPedido({ Codigo: dados.removidos[i] }, true);
}

function AlterarPedidoProdutoCarregamento(dados, outro, ambos, reconsultarCarregamento) {
    executarReST("MontagemCarga/AlterarPedidoProduto", dados, function (arg) {

        if (arg.Success) {
            if (arg.Data === false) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                atualizouQuantidadesPedido(arg.Data);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        var reconsultarNaoAtendidos = true;

        if (!outro && arg.Success) {
            //Remover os pedidos selecionados.. e adicionar
            retornoAlterarPedidoProduto(arg.Data);
        }

        if (reconsultarCarregamento) {
            if (outro == false || ambos) {
                reloadGridMotagemCarregamentoProdutos();
                retornoCarregamento({ Codigo: _carregamento.Carregamento.codEntity() });
            }
            if (ambos) {
                reconsultarNaoAtendidos = false;
                //pesquisarOutroCarregamentoClick();
                if (outro == false) {
                    reloadGridMotagemOutroCarregamentoProdutos();
                }
            }
        } else {

            if (outro == false || ambos) {
                reloadGridMotagemCarregamentoProdutos();
                retornoRemoveuProdutoPedidoCarregamento(dados.Carregamento, arg.Data);
            }

            if (outro || ambos)
                reloadGridMotagemOutroCarregamentoProdutos();
        }
        //Atualiza não atendidos.
        if (reconsultarNaoAtendidos === true) {
            reloadGridProdutosNaoAtendidos();
        }
    });
}

function ExibirErroRow(row, mensagem, tipoMensagem, titulo, outro) {
    if (outro == false)
        reloadGridMotagemCarregamentoProdutos();
    else
        reloadGridMotagemOutroCarregamentoProdutos();

    exibirMensagem(tipoMensagem, titulo, mensagem);

    return null;
}

function reloadGridMotagemCarregamentoProdutos() {
    const data = {
        Carregamento: _pesquisaProdutoCarregamento.Carregamento.val(),
        Cliente: _pesquisaProdutoCarregamento.Cliente.val()
    };
    executarReST("MontagemCargaPedido/ProdutosCarregamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.length > 0) {
                _gridProdutosCarregamento.CarregarGrid(arg.Data);
                _carregamentoProdutos.RemoverProdutosClienteCarregamento.visible(true);
            } else {
                _gridProdutosCarregamento.CarregarGrid([]);
                _carregamentoProdutos.RemoverProdutosClienteCarregamento.visible(false);
            }
            progressPercentualCarregamentoProdutos(null, false, data.Carregamento);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function reloadGridMotagemOutroCarregamentoProdutos() {
    var data = {
        Carregamento: _pesquisaProdutoOutroCarregamento.Carregamento.val(),
        Cliente: _pesquisaProdutoOutroCarregamento.Cliente.val()
    };
    executarReST("MontagemCargaPedido/ProdutosCarregamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.length > 0) {
                _gridProdutosOutroCarregamento.CarregarGrid(arg.Data);
                _outroCarregamentoProdutos.RemoverProdutosClienteCarregamento.visible(true);
            } else {
                _gridProdutosOutroCarregamento.CarregarGrid([]);
                _outroCarregamentoProdutos.RemoverProdutosClienteCarregamento.visible(false);
            }
            progressPercentualCarregamentoProdutos(null, true, data.Carregamento);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function reloadGridProdutosNaoAtendidos() {

    _pesquisaProtudosNaoAtendido.Cliente.val(_produtosNaoAtendidos.SelectClientes.val());

    if (_pesquisaProtudosNaoAtendido.BasicDataTable.val == false) {

        _gridProdutosNaoAtendidos.CarregarGrid();

    } else {

        var data = {
            SessaoRoteirizador: _pesquisaProtudosNaoAtendido.SessaoRoteirizador.val(),
            Cliente: _pesquisaProtudosNaoAtendido.Cliente.val(),
            BasicDataTable: _pesquisaProtudosNaoAtendido.BasicDataTable.val
        };

        executarReST("MontagemCargaPedido/ProdutosNaoAtendido", data, function (arg) {
            if (arg.Success) {
                if (arg.Data.length > 0) {
                    _gridProdutosNaoAtendidos.CarregarGrid(arg.Data);
                } else {
                    _gridProdutosNaoAtendidos.CarregarGrid([]);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
    // Atualizar Resumo Loja, Peso, pallet e Cubagem...
    carregamentoProdutosResumoLoja({ tipo: 'TOTAL', value: 0 });
}

function localizaMapaClick() {

}

function detalhesPedidoProdutoClick(produtoCarregamento) {
    ObterDetalhesPedidoProdutoCarregamento(produtoCarregamento.Codigo);
}

function detalhesPedidoClick(produtoCarregamento) {
    if (_filtroDetalhePedido)
        _filtroDetalhePedido.Pedidos.visible(false);
    ObterDetalhesPedido(produtoCarregamento.CodigoPedido);
}

function removerProdutoClick(produtoCarregamento) {
    removerProdutoGridClick(produtoCarregamento, false);
}

function removerProdutoOutroClick(produtoCarregamento) {
    removerProdutoGridClick(produtoCarregamento, true);
}

function removerProdutoGridClick(produtoCarregamento, outro) {
    var codCarregamento = _pesquisaProdutoCarregamento.Carregamento.val();
    var codPedidoProdutoCarregamento = produtoCarregamento.Codigo;

    if (outro) {
        codCarregamento = _pesquisaProdutoOutroCarregamento.Carregamento.val();
    }

    var dados = {
        Carregamento: codCarregamento,
        Codigo: 0,
        CodigoPedidoProdutoCarregamento: codPedidoProdutoCarregamento,
        CodigosPedidoProdutoCarregamento: [],
        Peso: 0,
        Qtde: 0,
        Pallet: 0,
        Cubico: 0,
        Operacao: 0, // Atualiza ou remove o produto do carregamento
        Alterando: 0
    };
    AlterarPedidoProdutoCarregamento(dados, outro, false, false);
}

function selecionouAbaClienteProdutosCarregamento(item) {
    BuscarTodosProdutosCarregamento();
}



function selecionouAbaClienteProdutosOutroCarregamento(item) {
    const cpf_cnpj = item.value;
    _pesquisaProdutoOutroCarregamento.Cliente.val(cpf_cnpj);
    _pesquisaProtudosNaoAtendido.Cliente.val(cpf_cnpj);
    reloadGridMotagemOutroCarregamentoProdutos()
}

function carregamentoProdutosResumoLoja(tipo) {
    return;
    var canal_entrega = parseInt(tipo.value);
    //Progress total da carga
    var data = { Carregamento: _pesquisaProdutoCarregamento.Carregamento.val(), CanalEntrega: canal_entrega, Cliente: _pesquisaProdutoCarregamento.Cliente.val() };
    executarReST("MontagemCarga/OcupacaoCarregamentoLoja", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                var info = arg.Data;
                //Total do peso da loja no peso total do carregamento
                _carregamentoProdutos.PesoLoja.val('(' + Globalize.format(info.TotalPesoCanalEntrega, "n2") + ' / ' + Globalize.format(info.TotalPeso, "n2") + ')');
                _carregamentoProdutos.CubagemLoja.val('(' + Globalize.format(info.TotalCubagemCanalEntrega, "n2") + ' / ' + Globalize.format(info.TotalCubagem, "n2") + ')');
                _carregamentoProdutos.PalletsLoja.val('(' + Globalize.format(info.TotalPalletsCanalEntrega, "n2") + ' / ' + Globalize.format(info.TotalPallets, "n2") + ')');
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

var DetalhePedidoProdutoCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoProduto = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CodigoDoProduto.getFieldDescription(), val: ko.observable("") });
    this.Produto = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Produto.getFieldDescription(), val: ko.observable("") });
    this.CanalEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CanalDeEntrega.getFieldDescription(), val: ko.observable("") });
    this.GrupoProduto = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.GrupoDoProduto.getFieldDescription(), val: ko.observable("") });
    this.LinhaSeparacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.LinhaSeparacao.getFieldDescription(), val: ko.observable("") });
    this.PalletFechado = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PalletFechado.getFieldDescription(), val: ko.observable("") });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Peso.getFieldDescription(), val: ko.observable("") });
    this.Qtde = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Quantidade.getFieldDescription(), val: ko.observable("") });
    this.Pallet = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Pallet.getFieldDescription(), val: ko.observable("") });
    this.CxsPallet = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CaixasPorPallet.getFieldDescription(), val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CubagemMetrosCubicos.getFieldDescription(), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable("") });
    this.PadraoPalletizacao = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PadraoDePalletizacao.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Embalagem = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Embalagem.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Carregamentos = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Carregamentos.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
}


function loadDetalhePedidoProdutoCarregamento(callback) {
    _detalhePedidoProdutoCarregamento = new DetalhePedidoProdutoCarregamento();
    KoBindings(_detalhePedidoProdutoCarregamento, "knoutDetalhePedidoProdutoCarregamento");
}

function ObterDetalhesPedidoProdutoCarregamento(codigo) {
    var data = { Codigo: codigo };
    carregarPedidoProduto(codigo, function () {
        executarReST("MontagemCargaPedido/BuscarPorCodigoPedidoProdutoCarregamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    Global.abrirModal("modalDetalhePedidoProdutoCarregamento");
                    PreencherObjetoKnout(_detalhePedidoProdutoCarregamento, arg);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}