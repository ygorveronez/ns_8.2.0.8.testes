function loadCheckList() {
    loadComponenteChecklistPergunta();
}

function CarregarInformacoesCheckList(data) {
    if (data.CheckList == null) {
        $("#liVisualizacaoPesquisa").hide();
        return;
    }

    $("#liVisualizacaoPesquisa").show();

    var grupos = [];
    for (var grupo of data.CheckList.Perguntas) {
        grupos.push({
            Descricao: grupo.Assunto,
            Perguntas: grupo.Perguntas
        });
    }

    _entrega.GrupoPerguntas.val(grupos);
    _entrega.CodigoCheckList.val(data.CheckList.Codigo);
    _entrega.ExibirCheckList.val(true);
}

function SalvarCheckListClick() {
    var perguntas = _entrega.GrupoPerguntas.val();

    if (!validarCheckListComponent(perguntas)) {
        return false;
    }

    var checkList = obterCheckListComponent(perguntas);
    var data = {
        CargaEntrega: _entrega.Codigo.val(),
        CheckList: JSON.stringify(checkList)
    };

    executarReST("ControleEntregaEntrega/SalvarChecklistProduto", data, function (arg) {
        if (!arg.Success) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            return;
        }

        if (!arg.Data) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, arg.Msg, 16000);
            return;
        }


        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
    }, null);
}

var auditoriaCheckList = null;
function exibirAuditoriaCheckListClick() {
    if (auditoriaCheckList == null)
        auditoriaCheckList = OpcaoAuditoria("CargaEntregaCheckList");

    auditoriaCheckList({
        Codigo: _entrega.CodigoCheckList.val()
    });
}

function CarregarDadosCheckList(checkList, codigoProduto) {
}