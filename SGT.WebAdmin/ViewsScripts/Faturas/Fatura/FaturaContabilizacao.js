
var FaturaContabilizacao = function () {
    this.Contabilizacao = PropertyEntity({ text: "Contabilizacao: ", idGrid: guid() });
}

var _HTMLFaturaContabilizacao;

function carregarHTMLFaturaContabilizacao(callback) {
    $.get("Content/Static/Fatura/FaturaContabilizacao.html?dyn=" + guid(), function (data) {
        _HTMLFaturaContabilizacao = data;
        callback();
    });
}

function ObterDetalhesFaturaContabilizacao() {

    var data = { CodigoFatura: _fatura.Codigo.val() };
    executarReST("FaturaFechamento/ObterDetalhesContabilizacao", data, function (e) {
        if (e.Success) {
            if (e.Data !== false) {
                $("#divFaturaContabilizacao").html("");
                for (var i = 0; i < e.Data.length; i++) {
                    var retorno = e.Data[i];
                    var knoutContabilizacao = new FaturaContabilizacao();
                    knoutContabilizacao.Contabilizacao.val(retorno.Descricao);
                    var idDiv = "knoutFaturaContabilizacao" + knoutContabilizacao.Contabilizacao.id;
                    $("#divFaturaContabilizacao").append(_HTMLFaturaContabilizacao.replace(/#knoutFaturaContabilizacao/g, idDiv));
                    KoBindings(knoutContabilizacao, idDiv);
                    
                    var header = [
                        { data: "codigoContaContabil", title: "Código", width: "15%" },
                        { data: "descricaoContaContabil", title: "Conta Contábil", width: "30%" },
                        { data: "codigoCentroResultado", title: "Centro de Custo", width: "15%" },
                        { data: "valor", title: "Valor", width: "15%" },
                        { data: "tipoContabilizacao", title: "Contabilização", width: "15%" }
                    ];

                    var tableContabilizacao = new BasicDataTable(knoutContabilizacao.Contabilizacao.idGrid, header, null, { column: 0, dir: orderDir.asc });
                    tableContabilizacao.CarregarGrid(retorno.ListaContabil);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}