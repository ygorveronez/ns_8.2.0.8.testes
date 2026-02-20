/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />

//#region Propriedades Globais

var _montagemContainerNotaFiscal;

//#endregion

//#region Mapeamento e Load

var MontagemContainerNotaFiscal = function () {
    this.GridNotaFiscal = PropertyEntity({ type: types.local });
    this.NotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar Nota Fiscal/Etiqueta", idBtnSearch: guid(), enable: true });

    this.Visible = PropertyEntity({ visible: _montagemContainer.TipoContainer.codEntity });

    this.Visible.visible.subscribe(function (valor) {
        if (valor <= 0) {
            $("#liTabNotasFiscais").hide();
            $("#montagem-container-informacoes").hide("slow");
        }
        else {
            $("#liTabNotasFiscais").show();
            $("#montagem-container-informacoes").show("slow");
            _montagemContainerNotaFiscal.GridNotaFiscal.CarregarGrid([]);
            obterResumoInformacoesMontagemContainer();
        }
    });
}

function loadMontagemContainerNotaFiscal() {
    _montagemContainerNotaFiscal = new MontagemContainerNotaFiscal();
    KoBindings(_montagemContainerNotaFiscal, "knockoutCadastroMontagemContainerNotasFiscais");

    loadGridMontagemContainerNotaFiscal();
}

function loadGridMontagemContainerNotaFiscal() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [
            { descricao: "Remover", id: guid(), metodo: removerMontagemContainerNotaFiscalClick }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "Chave", title: "Chave", width: "15%" },
        { data: "Emissor", title: "Emissor", width: "10%" },
        { data: "CNPJ", title: "CNPJ", width: "10%" },
        { data: "Quantidade", title: "Quantidade", width: "8%" },
        { data: "Tipo", title: "Tipo", width: "8%" },
        { data: "ProdutoEmbarcador", title: "Produto Embarcador", width: "10%" },
        { data: "ValorNota", title: "Valor da Nota", width: "8%" },
        { data: "ValorProdutos", title: "Valor Produtos", width: "8%" },
        { data: "PesoNota", title: "Peso da Nota", width: "8%" },
        { data: "Medidas", title: "Medidas (CxLxA)", width: "10%" },
        { data: "MetroCubico", title: "M³", width: "8%" },
        { data: "DataRecebimentoWMS", title: "Data Recebimento WMS", width: "10%" }
    ];

    _montagemContainerNotaFiscal.GridNotaFiscal = new BasicDataTable(_montagemContainerNotaFiscal.GridNotaFiscal.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    new BuscarXMLNotaFiscal(_montagemContainerNotaFiscal.NotaFiscal, adicionarRegistroGridMontagemContainerNotaFiscal, null, _montagemContainerNotaFiscal.GridNotaFiscal, true);

    _montagemContainerNotaFiscal.NotaFiscal.basicTable = _montagemContainerNotaFiscal.GridNotaFiscal;
    _montagemContainerNotaFiscal.GridNotaFiscal.CarregarGrid(new Array());
}

//#endregion

//#region Métodos de Click

function removerMontagemContainerNotaFiscalClick(registroSelecionado) {
    var listaRegistros = _montagemContainerNotaFiscal.GridNotaFiscal.BuscarRegistros();

    for (var i = 0; i < listaRegistros.length; i++) {
        if (listaRegistros[i].Codigo == registroSelecionado.Codigo) {
            listaRegistros.splice(i, 1);
            break;
        }
    }

    _montagemContainerNotaFiscal.GridNotaFiscal.CarregarGrid(listaRegistros);

    obterResumoInformacoesMontagemContainer();
}

//#endregion

//#region Métodos Privados

function adicionarRegistroGridMontagemContainerNotaFiscal(registros) {
    var listaRegistros = _montagemContainerNotaFiscal.GridNotaFiscal.BuscarRegistros();
    listaRegistros = listaRegistros.concat(registros);
    
    _montagemContainerNotaFiscal.GridNotaFiscal.CarregarGrid(listaRegistros);

    obterResumoInformacoesMontagemContainer();
}

function obterResumoInformacoesMontagemContainer() {
    var pesoNotas = 0;
    var metroCubicoNotas = 0;
    var quantidadeVolumesNotas = 0;

    var listaRegistros = _montagemContainerNotaFiscal.GridNotaFiscal.BuscarRegistros();

    for (var i = 0; i < listaRegistros.length; i++) {
        var registro = listaRegistros[i];
        
        pesoNotas += !string.IsNullOrWhiteSpace(registro.PesoNota) ? Globalize.parseFloat(registro.PesoNota) : 0;
        metroCubicoNotas += !string.IsNullOrWhiteSpace(registro.MetroCubico) ? Globalize.parseFloat(registro.MetroCubico) : 0;
        quantidadeVolumesNotas += !string.IsNullOrWhiteSpace(registro.Quantidade) ? Globalize.parseInt(registro.Quantidade) : 0;
    }

    _montagemContainerInformacoes.PesoNotas.val(pesoNotas.toFixed(2));
    _montagemContainerInformacoes.MetroCubicoNotas.val(metroCubicoNotas.toFixed(2));
    _montagemContainerInformacoes.QuantidadeVolumesNotas.val(quantidadeVolumesNotas.toFixed(2));

    var pesoContainer = Globalize.parseFloat(_montagemContainerInformacoes.PesoContainer.val());
    var metragemContainer = Globalize.parseFloat(_montagemContainerInformacoes.MetroCubicoContainer.val());
    
    var ocupacaoContainer = ((100 * pesoNotas) / (pesoContainer > 0 ? pesoContainer : 1)).toFixed(2).toString() + "%";
    var ocupacaoContainerMetragem = ((100 * metroCubicoNotas) / (metragemContainer > 0 ? metragemContainer : 1)).toFixed(2).toString() + "%";
    _montagemContainerInformacoes.PorcentagemComposicaoContainerPeso.val(ocupacaoContainer);
    _montagemContainerInformacoes.PorcentagemComposicaoContainerMetragem.val(ocupacaoContainerMetragem);
}

//#endregion