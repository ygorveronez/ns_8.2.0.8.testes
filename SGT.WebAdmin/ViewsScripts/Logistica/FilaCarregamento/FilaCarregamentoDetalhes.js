/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filaCarregamentoDetalhes;
var _gridFilaCarregamentoDetalhes;

/*
 * Declaração das Classes
 */

var FilaCarregamentoDetalhe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LocalAtual = PropertyEntity({ text: "Local Atual: ", visible: _CONFIGURACAO_TMS.UtilizarFilaCarregamentoReversa });
    this.Historicos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()) });

    this.Historicos.val.subscribe(function (historicos) {
        _gridFilaCarregamentoDetalhes.CarregarGrid(historicos);
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoDetalhes() {
    _filaCarregamentoDetalhes = new FilaCarregamentoDetalhe();
    KoBindings(_filaCarregamentoDetalhes, "knockoutFilaCarregamentoDetalhes");

    loadGridFilaCarregamentoDetalhes();
}

function loadGridFilaCarregamentoDetalhes() {
    const header = [
        { data: "DataOrdenar", visible: false },
        { data: "Data", title: "Data", width: "10%", className: 'text-align-center', orderable: false },
        { data: "Descricao", title: "Descrição", width: "30%", orderable: false },
        { data: "Usuario", title: "Usuário", width: "30%", orderable: false, visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga },
        { data: "OrigemAlteracao", title: "Origem da Alteração", width: "20%", orderable: false, className: 'text-align-center', visible: _CONFIGURACAO_TMS.UtilizarProgramacaoCarga },
        { data: "Posicao", title: "Posição", width: "10%", orderable: false, className: 'text-align-center' }
    ];
    const menuOpcoes = null;
    const ordenacao = { column: 0, dir: orderDir.asc };

    _gridFilaCarregamentoDetalhes = new BasicDataTable("grid-detalhes-fila-carregamento", header, menuOpcoes, ordenacao);
}

/*
 * Declaração das Funções
 */

function ExibirDetalhes(filaSelecionada) {
    executarReST("FilaCarregamento/ObterDetalhes", { Codigo: filaSelecionada.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_filaCarregamentoDetalhes, retorno);
                ExibirModalDetalhes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ExibirModalDetalhes() {
    Global.abrirModal('divModalDetalhesFilaCarregamento');
    $("#divModalDetalhesFilaCarregamento").one('hidden.bs.modal', function () {
        limparCamposFilaCarregamentoDetalhes();
    });
}

function limparCamposFilaCarregamentoDetalhes() {
    LimparCampos(_filaCarregamentoDetalhes);

    _filaCarregamentoDetalhes.Historicos.val(new Array());
}