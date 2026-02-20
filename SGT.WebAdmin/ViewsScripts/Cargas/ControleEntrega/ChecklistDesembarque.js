function loadCheckListDesembarque() {
    loadComponenteChecklistPergunta();
}

function CarregarInformacoesCheckListDesembarque(data) {
    if (data.CheckListDesembarque == null) {
        $("#liVisualizacaoPesquisaDesembarque").hide();
        return;
    }

    $("#liVisualizacaoPesquisaDesembarque").show();

    var grupos = [];
    for (var grupo of data.CheckListDesembarque.Perguntas) {
        for (var pergunta of grupo.Perguntas) {
            if (pergunta.Tipo === EnumTipoOpcaoCheckList.SimNao)
                pergunta.Obrigatorio = true;
        }

        grupos.push({
            Descricao: grupo.Assunto,
            Perguntas: grupo.Perguntas
        });
    }

    _entrega.GrupoPerguntasDesembarque.val(grupos);
    _entrega.CodigoCheckListDesembarque.val(data.CheckListDesembarque.Codigo);
    _entrega.ExibirCheckListDesembarque.val(true);
}

function SalvarCheckListDesembarqueClick() {
    var perguntas = _entrega.GrupoPerguntasDesembarque.val();

    if (!validarCheckListComponent(perguntas)) {
        return false;
    }

    var checkList = obterCheckListComponent(perguntas);
    var data = {
        CargaEntrega: _entrega.Codigo.val(),
        CheckList: JSON.stringify(checkList)
    };

    executarReST("ControleEntregaEntrega/SalvarChecklistDesembarque", data, function (arg) {
        if (!arg.Success) {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            return;
        }

        if (!arg.Data) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            return;
        }


        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
    }, null);
}

var auditoriaCheckList = null;
function exibirAuditoriaCheckListDesembarqueClick() {
    if (auditoriaCheckList == null)
        auditoriaCheckList = OpcaoAuditoria("CargaEntregaCheckList");

    auditoriaCheckList({
        Codigo: _entrega.CodigoCheckList.val()
    });
}

function CarregarDadosCheckListDesembarque(checkList, codigoProduto) {
}