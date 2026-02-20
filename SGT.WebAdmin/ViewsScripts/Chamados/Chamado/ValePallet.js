/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoValePallet.js" />
/// <reference path="Chamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridValePalletNoChamado;
var _pesquisaValePalletNoChamado;
var _chamadoOcorrenciaModalValePallet;

var PesquisaValePalletChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Chamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Chamado:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridValePalletNoChamado.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.AdicionarValePallet = PropertyEntity({ eventClick: adicionarValePalletChamadoClick, type: types.event, text: "Adicionar Vale Pallet", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: finalizarChamadoClick, type: types.event, text: "Finalizar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarChamadoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function EtapaValePalletClick() {
    _pesquisaValePalletNoChamado.Chamado.val(_abertura.Codigo.val());
    _pesquisaValePalletNoChamado.Chamado.codEntity(_abertura.Codigo.val());

    if (_chamado.Situacao.val() != EnumSituacaoChamado.LiberadaValePallet) {
        _pesquisaValePalletNoChamado.AdicionarValePallet.visible(false);
        _pesquisaValePalletNoChamado.Cancelar.visible(false);
        _pesquisaValePalletNoChamado.Fechar.visible(false);
    } else {
        _pesquisaValePalletNoChamado.AdicionarValePallet.visible(true);
        _pesquisaValePalletNoChamado.Cancelar.visible(true);
        _pesquisaValePalletNoChamado.Fechar.visible(true);
    }

    _gridValePalletNoChamado.CarregarGrid();
}

function adicionarValePalletChamadoClick() {
    LimparCamposValePallet();

    var dados = {
        Chamado: { Codigo: _chamado.Codigo.val(), Descricao: _chamado.Codigo.val() },
        Representante: { Codigo: _analise.Representante.codEntity(), Descricao: _analise.Representante.val() },
        Quantidade: Globalize.parseInt(_abertura.NumeroPallet.val()),
        Responsavel: { Codigo: _chamado.Responsavel.codEntity(), Descricao: _chamado.Responsavel.val() },
    };

    DadosLancamento({ Lancamento: dados, Situacao: EnumSituacaoValePallet.Todas });

    _chamadoOcorrenciaModalValePallet.show();
}

//*******MÉTODOS*******

function LoadValePalletChamado() {
    _pesquisaValePalletNoChamado = new PesquisaValePalletChamado();
    KoBindings(_pesquisaValePalletNoChamado, "knockoutValePallet", false, _pesquisaValePalletNoChamado.Pesquisar.id);

    BuscarValePalletsChamado();

    _pesquisaValePallet = new PesquisaValePallet();
    LoadValePalletExterno("#divModalValePallet .modal-body", _abertura?.Carga);

    $('#divModalValePallet').on('hidden.bs.modal', function () {
        _gridValePalletNoChamado.CarregarGrid();
    });

    $('body').on('ValePalletAdicionado', function () {
        _chamadoOcorrenciaModalValePallet.hide();
    });

    _chamadoOcorrenciaModalValePallet = new bootstrap.Modal(document.getElementById("divModalValePallet"), { backdrop: 'static' });
}

function BuscarValePalletsChamado() {
    var editar = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: editarValePalletNoChamado,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridValePalletNoChamado = new GridView(_pesquisaValePalletNoChamado.Pesquisar.idGrid, "ValePallet/Pesquisa", _pesquisaValePalletNoChamado, menuOpcoes, null, null);
}

function editarValePalletNoChamado(data) {
    LimparCamposValePallet();

    BuscarValePalletPorCodigo(data.Codigo, function () {
        _chamadoOcorrenciaModalValePallet.show();
    });
}

function Etapa3ComoValePallet() {
    _etapa.Etapa3.text("Vale Pallet");
    _etapa.Etapa3.tooltip("Gera Vale Pallet no fim do chamado.");
    _etapa.Etapa3.tooltipTitle("Vale Pallet");

    _analise.Finalizar.text("Gerar Vale Pallet");
    _analise.Fechar.text("Fechar (sem vale pallet)");

    $("#knockoutValePallet").show();
}