var _gridHistoricoIntegracaoAgendamento;
var _pesquisaHistoricoIntegracaoAgendamento;

var PesquisaHistoricoIntegracaoAgendamento = function () {
    this.Codigos = PropertyEntity({ val:ko.observable(""), def: "" });
}

function LoadHistoricoIntegracaoAgendamento() {
    _pesquisaHistoricoIntegracaoAgendamento = new PesquisaHistoricoIntegracaoAgendamento();
    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoAgendamento, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoAgendamento = new GridView("tblHistoricoIntegracaoAgendamento", "JanelaDescarga/BuscarHistoricoIntegracaoAgendamento", _pesquisaHistoricoIntegracaoAgendamento, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function ExibirHistoricoIntegracaoAgendamento(codigosAgendamentoColetaPedido) {
    BuscarHistoricoIntegracaoAgendamento(codigosAgendamentoColetaPedido);
    Global.abrirModal("divModalHistoricoIntegracaoAgendamento");
}

function BuscarHistoricoIntegracaoAgendamento(codigosAgendamentoColetaPedido) {
    _pesquisaHistoricoIntegracaoAgendamento.Codigos.val(JSON.stringify(codigosAgendamentoColetaPedido));
    _gridHistoricoIntegracaoAgendamento.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoAgendamento(historicoConsulta) {
    executarDownload("JanelaDescarga/DownloadArquivosHistoricoIntegracaoAgendamento", { Codigo: historicoConsulta.Codigo });
}