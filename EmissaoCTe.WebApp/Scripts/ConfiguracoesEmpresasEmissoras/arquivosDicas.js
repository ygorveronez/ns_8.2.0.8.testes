var templateArquivo = [
        '<div class="col-sm-4 col-md-3 col-lg-2">',
            '<div class="arquivo">',
                '<div class="arquivo-excluir clearfix">',
                    '<button onclick="ExcluirArquivoDica(#Codigo#)" type="button" title="Excluir arquivo #Nome#" class="close" aria-hidden="true">×</button>',
                '</div>',
                '<div class="arquivo-icone">',
                    '<div style="background-image: url(\'Images/arquivo-dicas.png\')"></div>',
                '</div>',
                '<div class="arquivo-nome">',
                    '<a href="#" onclick="BaixarArquivoDica(#Codigo#, event)" title="Download arquivo #Nome#" >#Nome#</a>',
                '</div>',
            '</div>',
        '</div>',
].join("");

var arquivosDicas = [];
/*
Modelo:
{
    Codigo: int,
    Nome: String
}
*/

$(document).ready(function () {
    $("#btnAdicionarArquivosDicas").click(function (e) {
        e.preventDefault();

        AbrirUploadPadrao({
            url: '/ConfiguracaoEmpresa/InserirArquivosDicas?callback=?',
            onFinish: RetornoArquivosDicas,
            title: "Importação de arquivos de dicas",
            filter: [
                { title: "Arquivos de imagem", extensions: "jpg,png" },
                { title: "Arquivos PDF", extensions: "pdf" },
                { title: "Arquivos Office", extensions: "xls,xlsx,doc,docx" },
                { title: "Arquivos padrões", extensions: "txt" },
            ],
        })
    });
});

function ExcluirArquivoDica(codigo) {
    executarRest('/ConfiguracaoEmpresa/ExcluirArquivoDicas?callback=?', { Codigo: codigo }, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Arquivo excluído com sucesso.", "Sucesso!", "messages-placeholderArquivosDicas");

            for (var i in arquivosDicas) {
                if (arquivosDicas[i].Codigo == codigo)
                    arquivosDicas.splice(i, 1);
            }

            RenderizarArquivosDicas();
        } else
            ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholderArquivosDicas");
    });
}

function BaixarArquivoDica(codigo, e) {
    if (e && e.preventDefault) e.preventDefault();
    executarDownload('/ConfiguracaoEmpresa/DownloadArquivoDicas?callback=?', { Codigo: codigo });
}

function RetornoArquivosDicas(arquivos, erros) {
    if (erros.length > 0) {
        for (var e in erros) {
            ExibirMensagemErro(erros[e], "Atenção!", "messages-placeholderArquivosDicas");
        }
    }
    if (arquivos.length > 0) {
        arquivosDicas = arquivos.concat(arquivosDicas);
        RenderizarArquivosDicas();
    }
}

function RenderizarArquivosDicas() {
    var htmlArquivos = "";

    for (var i in arquivosDicas) {
        var arquivo  = arquivosDicas[i];
        var template = templateArquivo;

        for (k in arquivo) 
            template = template.replace(new RegExp("(#" + k + "#)", "gm"), arquivo[k]);

        htmlArquivos += template;
    }
    $("#arqInseridos").html(htmlArquivos);
}