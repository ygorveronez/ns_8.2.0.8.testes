/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/AreaVeiculo.js" />
/// <reference path="RetornoMultiplaSelecao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _areaVeiculo;
var _gridAreaVeiculo;

/*
 * Declaração das Classes
 */

var AreaVeiculo = function () {
    this.Codigos;
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.AreaVeiculo = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarAreaDeVeiculo, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Salvar = PropertyEntity({ eventClick: salvarAreaVeiculoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAreaVeiculo() {
    _areaVeiculo = new AreaVeiculo();
    KoBindings(_areaVeiculo, "knockoutAreaVeiculo");

    loadGridAreaVeiculo();
}

function loadGridAreaVeiculo() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirAreaVeiculoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoExcluir]};
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridAreaVeiculo = new BasicDataTable("grid-area-veiculo", header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    new BuscarAreaVeiculo(_areaVeiculo.AreaVeiculo, null, _areaVeiculo.CentroCarregamento, _gridAreaVeiculo);

    _areaVeiculo.AreaVeiculo.basicTable = _gridAreaVeiculo;

    _gridAreaVeiculo.CarregarGrid(new Array());
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirAreaVeiculoClick(registroSelecionado) {
    var listaAreaVeiculo = _gridAreaVeiculo.BuscarRegistros();

    for (var i = 0; i < listaAreaVeiculo.length; i++) {
        if (registroSelecionado.Codigo == listaAreaVeiculo[i].Codigo) {
            listaAreaVeiculo.splice(i, 1);
            break;
        }
    }

    _gridAreaVeiculo.CarregarGrid(listaAreaVeiculo);
}

function salvarAreaVeiculoClick() {
    var dados = {
        Codigos: JSON.stringify(_areaVeiculo.Codigos),
        AreasVeiculos: JSON.stringify(_gridAreaVeiculo.BuscarCodigosRegistros())
    };

    executarReST("JanelaCarregamento/SalvarAreasVeiculos", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirRetornoMultiplaSelecao(retorno.Data);
                fecharModalAreaVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirAreaVeiculo(codigoCargaJanelaCarregamento) {
    _areaVeiculo.Codigos = [codigoCargaJanelaCarregamento];
    _areaVeiculo.CentroCarregamento.codEntity(_dadosPesquisaCarregamento.CentroCarregamento);
    _areaVeiculo.CentroCarregamento.val(_dadosPesquisaCarregamento.CentroCarregamento);

    executarReST("JanelaCarregamento/ObterAreasVeiculos", { Codigo: codigoCargaJanelaCarregamento }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _areaVeiculo.AreaVeiculo.visible(retorno.Data.PermitirEditar);
                _areaVeiculo.Salvar.visible(retorno.Data.PermitirEditar);

                _gridAreaVeiculo.CarregarGrid(retorno.Data.AreasVeiculos, retorno.Data.PermitirEditar);

                exibirModalAreaVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirAreasVeiculos(codigos) {
    _areaVeiculo.Codigos = codigos;
    _areaVeiculo.CentroCarregamento.codEntity(_dadosPesquisaCarregamento.CentroCarregamento);
    _areaVeiculo.CentroCarregamento.val(_dadosPesquisaCarregamento.CentroCarregamento);
    _areaVeiculo.AreaVeiculo.visible(true);
    _areaVeiculo.Salvar.visible(true);

    exibirModalAreaVeiculo();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalAreaVeiculo() {
    Global.abrirModal('divModalAreaVeiculo');
    $("#divModalAreaVeiculo").one('hidden.bs.modal', function () {
        limparCamposAreaVeiculo();
    });
}

function fecharModalAreaVeiculo() {    
    Global.fecharModal('divModalAreaVeiculo');
}

function limparCamposAreaVeiculo() {
    LimparCampos(_areaVeiculo);
    _gridAreaVeiculo.CarregarGrid(new Array());
}
