$(document).ready(function () {
    $("#btnSalvarFTP").click(function () {
        SalvarFTP();
    });
    $("#btnCancelarFTP").click(function () {
        LimparCamposFTP();
    });
    $("#btnExcluirFTP").click(function () {
        ExcluirFTP();
    });

    CarregarConsultadeClientes('btnBuscarClienteFTP', 'btnBuscarClienteFTP', RetornoConsultaClienteFTP, true, false);
    CarregarConsultaDeLayoutEDI('btnBuscarLayoutEDIFTP', 'btnBuscarLayoutEDIFTP', 2, RetornoConsultaLayoutEDIFTP, true, false);

    RemoveConsulta("#txtClienteFTP, #txtLayoutEDIFTP", function ($this) {
        $this.val('');
        $this.data('codigo', '');
    });

    LimparCamposFTP();
});

var StateFTP = new State({
    name: "FTP",
    id: "Id",
    render: RenderizarConfiguracoesFTP
});
var IdFTPEmEdicao = 0;


function RetornoConsultaClienteFTP(cliente) {
    var $this = $("#txtClienteFTP");
    $this.val(cliente.Nome);
    $this.data('codigo', cliente.CPFCNPJ);
}

function RetornoConsultaLayoutEDIFTP(layout) {
    var $this = $("#txtLayoutEDIFTP");
    $this.val(layout.Descricao);
    $this.data('codigo', layout.Codigo);
}

function LimparCamposFTP() {
    IdFTPEmEdicao = 0;

    $("#txtClienteFTP").val('');
    $("#txtClienteFTP").data('codigo', '');
    $("#txtLayoutEDIFTP").val('');
    $("#txtLayoutEDIFTP").data('codigo', '');

    $("#selTipoFTP").val($("#selTipoFTP option:first").val());
    $("#selFTPRateio").val($("#selFTPRateio option:first").val());
    $("#selFTPTipoArquivo").val($("#selFTPTipoArquivo option:first").val());

    $("#txtFTPHost").val('');
    $("#txtFTPPorta").val('');
    $("#txtFTPUsuario").val('');
    $("#txtFTPSenha").val('');
    $("#txtFTPDiretorio").val('');

    $("#chkFTPPassivo").prop('checked', false);
    $("#chkFTPSeguro").prop('checked', false);
    $("#chkFTPGerarNFSe").prop('checked', false);
    $("#chkFTPEmitirDocumento").prop('checked', false);
    $("#chkFTPUtilizarContratanteComoTomador").prop('checked', false);
    $("#chkFTPSSL").prop('checked', false);

    $("#btnExcluirFTP").hide();

    $("#selTipoFTP").change(function () {
        ParametrosTipo();
    });
    ParametrosTipo();
    // Remove validacoes
    CampoSemErro($("#txtClienteFTP"));
    CampoSemErro($("#txtLayoutEDIFTP"));
    CampoSemErro($("#txtFTPHost"));
    CampoSemErro($("#txtFTPUsuario"));
}

function ParametrosTipo() {
    if ($("#selTipoFTP").val() == "1") { //IMPORTACAO XML NFE
        $("#chkFTPGerarNFSe").prop('checked', false);
        $("#chkFTPGerarNFSe").attr("disabled", true);
        $("#selFTPRateio").val("3");
        $("#selFTPRateio").attr("disabled", true);
        $("#txtLayoutEDIFTP").attr("disabled", true);
        $("#btnBuscarLayoutEDIFTP").attr("disabled", true);  
        $("#chkFTPUtilizarContratanteComoTomador").attr("disabled", true);       
    } else if ($("#selTipoFTP").val() == "0") { //IMPORTACAO NOTFIS
        $("#chkFTPGerarNFSe").attr("disabled", false);
        $("#selFTPRateio").val("0");
        $("#selFTPRateio").attr("disabled", false);
        $("#txtLayoutEDIFTP").attr("disabled", false);
        $("#btnBuscarLayoutEDIFTP").attr("disabled", false);
        $("#chkFTPUtilizarContratanteComoTomador").attr("disabled", false);       
        $("#btnBuscarLayoutEDIFTP").unbind();
        CarregarConsultaDeLayoutEDI('btnBuscarLayoutEDIFTP', 'btnBuscarLayoutEDIFTP', 2, RetornoConsultaLayoutEDIFTP, true, false);
    }
    else if ($("#selTipoFTP").val() == "3") { //ENVIO OCORRENCIA CTE
        $("#chkFTPGerarNFSe").attr("disabled", true);
        $("#selFTPRateio").val("0");
        $("#selFTPRateio").attr("disabled", true);
        $("#txtLayoutEDIFTP").attr("disabled", false);
        $("#btnBuscarLayoutEDIFTP").attr("disabled", false);
        $("#chkFTPEmitirDocumento").attr("disabled", true);       
        $("#chkFTPUtilizarContratanteComoTomador").attr("disabled", true);               
        $("#btnBuscarLayoutEDIFTP").unbind();
        CarregarConsultaDeLayoutEDI('btnBuscarLayoutEDIFTP', 'btnBuscarLayoutEDIFTP', 3, RetornoConsultaLayoutEDIFTP, true, false);
    }
    else if ($("#selTipoFTP").val() == "4") { //ENVIO OCORRENCIA NFSE
        $("#chkFTPGerarNFSe").attr("disabled", true);
        $("#selFTPRateio").val("0");
        $("#selFTPRateio").attr("disabled", true);
        $("#txtLayoutEDIFTP").attr("disabled", false);
        $("#btnBuscarLayoutEDIFTP").attr("disabled", false);
        $("#chkFTPEmitirDocumento").attr("disabled", true);     
        $("#chkFTPUtilizarContratanteComoTomador").attr("disabled", true);       
        $("#btnBuscarLayoutEDIFTP").unbind();
        CarregarConsultaDeLayoutEDI('btnBuscarLayoutEDIFTP', 'btnBuscarLayoutEDIFTP', 18, RetornoConsultaLayoutEDIFTP, true, false);
    }
    else if ($("#selTipoFTP").val() == "5") { //ENVIO XML CTE
        $("#chkFTPGerarNFSe").prop('checked', true);
        $("#chkFTPGerarNFSe").attr("disabled", true);
        $("#selFTPRateio").val("3");
        $("#selFTPRateio").attr("disabled", true);
        $("#txtLayoutEDIFTP").attr("disabled", true);
        $("#btnBuscarLayoutEDIFTP").attr("disabled", true);  
        $("#chkFTPEmitirDocumento").attr("disabled", true);    
        $("#chkFTPUtilizarContratanteComoTomador").attr("disabled", true);       
    }
}

function EditarFTP(info) {
    IdFTPEmEdicao = info.Id;

    $("#txtClienteFTP").val(info.DescricaoCliente);
    $("#txtClienteFTP").data('codigo', info.Cliente);
    $("#txtLayoutEDIFTP").val(info.DescricaoLayoutEDI);
    $("#txtLayoutEDIFTP").data('codigo', info.LayoutEDI);

    $("#selTipoFTP").val(info.Tipo);
    $("#selFTPRateio").val(info.Rateio);
    $("#selFTPTipoArquivo").val(info.TipoArquivo);    

    $("#txtFTPHost").val(info.Host);
    $("#txtFTPPorta").val(info.Porta);
    $("#txtFTPUsuario").val(info.Usuario);
    $("#txtFTPSenha").val(info.Senha);
    $("#txtFTPDiretorio").val(info.Diretorio);

    $("#chkFTPPassivo").prop('checked', info.Passivo);
    $("#chkFTPSeguro").prop('checked', info.Seguro);
    $("#chkFTPGerarNFSe").prop('checked', info.GerarNFSe);
    $("#chkFTPEmitirDocumento").prop('checked', info.EmitirDocumento);
    $("#chkFTPUtilizarContratanteComoTomador").prop('checked', info.UtilizarContratanteComoTomador);    
    $("#chkFTPSSL").prop('checked', info.SSL);

    $("#btnExcluirFTP").show();
}

function ExcluirFTP() {
    StateFTP.remove({ Id: IdFTPEmEdicao });
    LimparCamposFTP();
}

function SalvarFTP() {
    var erros = ValidarFTP();

    if (erros.length == 0) {
        var FTP = {
            Id: IdFTPEmEdicao,
            Cliente: $("#txtClienteFTP").data('codigo'),
            DescricaoCliente: $("#txtClienteFTP").val(),
            LayoutEDI: $("#txtLayoutEDIFTP").data('codigo'),
            DescricaoLayoutEDI: $("#txtLayoutEDIFTP").val(),
            Tipo: $("#selTipoFTP").val(),
            DescricaoTipo: $("#selTipoFTP option:selected").text(),
            Rateio: $("#selFTPRateio").val(),
            DescricaoRateio: $("#selFTPRateio option:selected").text(),
            TipoArquivo: $("#selFTPTipoArquivo").val(),
            DescricaoTipoArquivo: $("#selFTPTipoArquivo option:selected").text(),
            Host: $("#txtFTPHost").val(),
            Porta: $("#txtFTPPorta").val(),
            Usuario: $("#txtFTPUsuario").val(),
            Senha: $("#txtFTPSenha").val(),
            Diretorio: $("#txtFTPDiretorio").val(),
            Passivo: $("#chkFTPPassivo").prop('checked'),
            Seguro: $("#chkFTPSeguro").prop('checked'),
            GerarNFSe: $("#chkFTPGerarNFSe").prop('checked'),
            EmitirDocumento: $("#chkFTPEmitirDocumento").prop('checked'),
            UtilizarContratanteComoTomador: $("#chkFTPUtilizarContratanteComoTomador").prop('checked'),            
            SSL: $("#chkFTPSSL").prop('checked')
        };

        InsereFTP(FTP);
        LimparCamposFTP();
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-ftp").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-ftp");
    }
}

function ValidarFTP() {
    var valido = [];

    if ($("#txtClienteFTP").data('codigo') == "") {
        valido.push("Cliente é obrigatório.");
        CampoComErro($("#txtClienteFTP"));
    } else {
        CampoSemErro($("#txtClienteFTP"));
    }

    if ($("#selTipoFTP").val() == "0" || $("#selTipoFTP").val() == "3" || $("#selTipoFTP").val() == "4") {
        if ($("#txtLayoutEDIFTP").data('codigo') == "") {
            valido.push("Layout EDI é obrigatório.");
            CampoComErro($("#txtLayoutEDIFTP"));
        } else {
            CampoSemErro($("#txtLayoutEDIFTP"));
        }
    }

    if ($("#txtFTPHost").val() == "") {
        valido.push("Host é obrigatório.");
        CampoComErro($("#txtFTPHost"));
    } else {
        CampoSemErro($("#txtFTPHost"));
    }

    if ($("#txtFTPUsuario").val() == "") {
        valido.push("Usuário é obrigatório.");
        CampoComErro($("#txtFTPUsuario"));
    } else {
        CampoSemErro($("#txtFTPUsuario"));
    }

    return valido;
}

function InsereFTP(obj) {
    var obj = $.extend({
        Id: 0,
        Cliente: 0,
        DescricaoCliente: '',
        LayoutEDI: 0,
        DescricaoLayoutEDI: '',
        Tipo: 0,
        DescricaoTipo: '',
        Rateio: 0,
        DescricaoRateio: '',
        TipoArquivo: 0,
        DescricaoTipoArquivo: '',
        Host: '',
        Porta: '',
        Usuario: '',
        Senha: '',
        Diretorio: '',
        Passivo: false,
        SSL: false,
        Seguro: false,
        GerarNFSe: false,
        EmitirDocumento: false,
        UtilizarContratanteComoTomador: false,
        Excluir: false
    }, obj);

    // Converte dados
    if ("string" == typeof obj.Cliente)
        obj.Cliente = obj.Cliente.replace(/[^0-9]/g, '');
    obj.Cliente = parseInt(obj.Cliente);
    obj.LayoutEDI = parseInt(obj.LayoutEDI);
    obj.Tipo = parseInt(obj.Tipo);

    if (obj.Id != 0)
        StateFTP.update(obj);
    else
        StateFTP.insert(obj);
}

function RenderizarConfiguracoesFTP() {
    var itens = StateFTP.get();
    var $tabela = $("#tblConfiguracoesFTP");
    var $tbody = $tabela.find("tbody");
    var $rows = [];

    $tbody.html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {

            var $row = $("<tr>" +
                "<td>" + info.DescricaoCliente + "</td>" +
                "<td>" + info.DescricaoLayoutEDI + "</td>" +
                "<td>" + info.DescricaoTipo + "</td>" +
                "<td>" + info.Host + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                EditarFTP(info);
            });

            $rows.push($row);
        }
    });

    $tbody.append.apply($tbody, $rows);

    if ($tbody.find("tr").length == 0)
        $tbody.html("<tr><td colspan='" + $tabela.find("thead th").length + "' class='text-center'>Nenhum registro encontrado.</td></tr>");
}