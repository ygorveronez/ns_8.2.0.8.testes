var _painelTracao;
var _gridTracao;

var PainelTracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ text: "Adicionar", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Tracao = PropertyEntity({  idGrid: guid() });
}

function CarregarGridTracao() {
    var excluir = { descricao: "Excluir", evento: "onclick", metodo: excluirTracaoClick, tamanho: "20" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var configuracaoExportacao = {
        url: "ManutencaoCentroResultado/ExportarTracao",
        titulo: "Trações"
    };

    _gridTracao = new GridView(_painelTracao.Tracao.idGrid, "ManutencaoCentroResultado/BuscarTracao", _manutencaoCentroResultado, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridTracao.CarregarGrid();
}

function retornoAdicionarTracao(item) {
    executarReST("ManutencaoCentroResultado/AdicionarTracao", { Codigo: item.Codigo, CodigoCentroResultado: _manutencaoCentroResultado.CentroResultado.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridTracao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function excluirTracaoClick(e) {
    _painelTracao.Codigo.val(e.Codigo);

    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_painelTracao, "ManutencaoCentroResultado/ExcluirTracaoPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTracao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}