/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _composicaoHorarioDescarregamentoContainer;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ComposicaoHorarioDescarregamento = function (dados) {
    this.DataCriacao = PropertyEntity({ text: "Data: ", getType: typesKnockout.dateTime, idGrid: guid() });
    this.GridDetalhes;
  
    PreencherObjetoKnout(this, { Data: dados });
};

var ComposicaoHorarioDescarregamentoContainer = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ListaComposicaoHorarioDescarregamento = ko.observableArray([]);
};

// #endregion Classes

// #region Funções de Inicialização

function loadComposicaoHorarioDescarregamento() {
    _composicaoHorarioDescarregamentoContainer = new ComposicaoHorarioDescarregamentoContainer();
    KoBindings(_composicaoHorarioDescarregamentoContainer, "knoutComposicaoHorarioDescarregamento");
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirComposicaoHorarioDescarregamento(janelaDescarregamentoSelecionada) {
    _composicaoHorarioDescarregamentoContainer.Codigo.val(janelaDescarregamentoSelecionada.Codigo);

    preencherComposicaoHorarioDescarregamento();
}

// #endregion Funções Públicas

// #region Funções Privadas

function criarGridComposicaoHorarioDescarregamento(knoutSelecionado, descricoes) {
    var exibirPaginacao = false;
    var ordenacaoPadrao = { column: 0, dir: orderDir.asc };
    var quantidadePorPagina = 9999999;
    var header = [
        { data: "Ordem", title: "Ordem", width: "15%", className: "text-align-center" },
        { data: "Descricao", title: "Passo a Passo", width: "85%", orderable: false }
    ];

    knoutSelecionado.GridDetalhes = new BasicDataTable(knoutSelecionado.DataCriacao.idGrid, header, null, ordenacaoPadrao, null, quantidadePorPagina, null, exibirPaginacao);
    knoutSelecionado.GridDetalhes.CarregarGrid(descricoes);
}

function limparComposicaoHorarioDescarregamento() {
    var listaComposicaoHorarioDescarregamento = _composicaoHorarioDescarregamentoContainer.ListaComposicaoHorarioDescarregamento.slice();

    for (var i = 0; i < listaComposicaoHorarioDescarregamento.length; i++)
        listaComposicaoHorarioDescarregamento[i].GridDetalhes.Destroy();

    _composicaoHorarioDescarregamentoContainer.ListaComposicaoHorarioDescarregamento.removeAll();
}

function preencherComposicaoHorarioDescarregamento() {
    limparComposicaoHorarioDescarregamento();

    executarReST("JanelaDescarga/ObterComposicaoHorarioDescarregamento", { Codigo: _composicaoHorarioDescarregamentoContainer.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                for (var i = 0; i < retorno.Data.length; i++) {
                    var composicao = retorno.Data[i];
                    var knoutComposicaoHorarioDescarregamento = new ComposicaoHorarioDescarregamento(retorno.Data[i]);

                    _composicaoHorarioDescarregamentoContainer.ListaComposicaoHorarioDescarregamento.push(knoutComposicaoHorarioDescarregamento);

                    criarGridComposicaoHorarioDescarregamento(knoutComposicaoHorarioDescarregamento, composicao.Descricoes);
                }

                var carouselComposicaoHorarioDescarregamento = document.querySelector('#carousel-composicao-horario-descarregamento');
                new bootstrap.Carousel(carouselComposicaoHorarioDescarregamento, { interval: false });
                Global.abrirModal('divModalComposicaoHorarioDescarregamento');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Privadas
