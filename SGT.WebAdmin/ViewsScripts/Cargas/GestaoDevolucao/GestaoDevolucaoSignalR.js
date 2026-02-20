/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoAtualizacaoGestaoDevolucao.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Global/SignalR/SignalR.js" />

function loadConexaoSignalRGestaoDevolucao() {
    SignalRGestaoDevolucaoInformarGestaoDevolucaoAtualizada = AtualizarGestaoDevolucaoSignalR;
}

function AtualizarGestaoDevolucaoSignalR(gestaoDevolucao) {
    if (!_gridGestaoDevolucaoDevolucoes) return;

    let deveMovimentarEtapa = EnumTipoAtualizacaoGestaoDevolucao.obterOpcoesMovimentacao().includes(gestaoDevolucao.MovimentarEtapa);

    //Atualização dos dados da Gestão Devolução na grid.
    _gridGestaoDevolucaoDevolucoes.AtualizarDataRow('#' + gestaoDevolucao.Codigo, gestaoDevolucao);

    //Se deve movimentar etapa e a Gestão Devolução atualizada está selecionada na grid.
    if (deveMovimentarEtapa && _informacoesDevolucao && _informacoesDevolucao.CodigoDevolucao.val() == gestaoDevolucao.Codigo)
        movimentarEtapa(gestaoDevolucao.Codigo);

    else if (_informacoesDevolucao && _informacoesDevolucao.CodigoDevolucao.val() == gestaoDevolucao.Codigo && gestaoDevolucao.MovimentarEtapa == EnumTipoAtualizacaoGestaoDevolucao.AtualizarMesmaEtapa)
        atualizarMesmaEtapa(gestaoDevolucao.Codigo);
        
    else if (_informacoesDevolucao && _informacoesDevolucao.CodigoDevolucao.val() == gestaoDevolucao.Codigo && gestaoDevolucao.MovimentarEtapa == EnumTipoAtualizacaoGestaoDevolucao.AtualizarGrid)
        limparEtapasDevolucao();
}