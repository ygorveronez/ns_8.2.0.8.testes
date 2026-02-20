var Upload = null;
var CodigoArquivo = 0;
var $modalarquivo;

$(document).ready(function () {
    FormatarCampoDate("txtData");

    $modalarquivo = $("#divArquivo");

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnAnexo").click(function () {
        EnviarAnexo();
    });

    $("#btnDownloadAnexo").click(function () {
        executarDownload("/ArquivoTransportador/DownloadArquivo", { Codigo: CodigoArquivo }, null, null, null, "cadastroArquivo");
    });

    $("#btnNovoArquivo").click(function () {
        $modalarquivo.modal('show');
    });
    
    $modalarquivo.on("hidden.bs.modal", function () {
        LimparCampos();
    });

    LimparCampos();
});

function LimparCampos() {
    CodigoArquivo = 0;
    $("#txtDescricao").val("");
    $("#txtData").val(moment(new Date).format("DD/MM/YYYY"));
    $("#txtLog").val("");
    $("#selStatus").val($("#selStatus option:first").val());

    $("#btnDownloadAnexo").hide();
    $("#btnAnexo").text("Inserir Anexo");
}

function Salvar(cbsucesso) {
    var dados = {
        Codigo: CodigoArquivo,
        Descricao: $("#txtDescricao").val(),
        Data: $("#txtData").val(),
        Status: $("#selStatus").val()
    };

    if (!Validar())
        return ExibirMensagemAlerta("Campos em vermelho são obrigatórios.", "Cadastrado", "cadastroArquivo");

    executarRest("/ArquivoTransportador/Salvar", dados, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Arquivo foi cadastrado com sucesso.", "Cadastrado", "cadastroArquivo");
            CodigoArquivo = r.Objeto.Codigo;
            AtualizarGridArquivos();

            if (cbsucesso != null) {
                cbsucesso();
            } else {
                BuscarPorCodigo();
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!", "cadastroArquivo");
        }
    });
}

function EnviarAnexo() {
    if (CodigoArquivo == 0)
        return Salvar(EnviarAnexo);

    Upload = AbrirUploadPadrao({
        url: "/ArquivoTransportador/InserirArquivo?Codigo=" + CodigoArquivo,
        multiple: false,
        max_file_size: '10000kb',
        onFinish: function (arquivos, erros) {
            if (erros.legth > 0)
                ExibirMensagemErro(erros[0], "Anexo", "cadastroArquivo");
            else {
                $("#btnDownloadAnexo").show();
                $("#btnAnexo").text("Substituir Anexo");
            }
        }
    });
}

function Validar() {
    var valido = true;

    if ($("#txtDescricao").val() == "") {
        CampoComErro("#txtDescricao");
        valido = false;
    } else
        CampoSemErro("#txtDescricao");

    if ($("#txtData").val() == "") {
        CampoComErro("#txtData");
        valido = false;
    } else
        CampoSemErro("#txtData");

    return valido;
}

function BuscarPorCodigo() {
    var dados = {
        Codigo: CodigoArquivo
    };

    executarRest("/ArquivoTransportador/ObterDetalhes", dados, function (r) {
        if (r.Sucesso) {
            $modalarquivo.modal('show');

            $("#txtDescricao").val(r.Objeto.Descricao);
            $("#txtData").val(r.Objeto.Data);
            $("#txtLog").val(r.Objeto.Log);
            $("#selStatus").val(r.Objeto.Satus);

            if (r.Objeto.PossuiArquivo) {
                $("#btnDownloadAnexo").show();
                $("#btnAnexo").text("Substituir Anexo");
            } else {
                $("#btnDownloadAnexo").hide();
                $("#btnAnexo").text("Inserir Anexo");
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}
