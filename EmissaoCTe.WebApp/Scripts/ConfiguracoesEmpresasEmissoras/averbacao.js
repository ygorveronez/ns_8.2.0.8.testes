$(document).ready(function () {
    $("#chkAverbacaoConfiguracaoClienteNaoAverbar").change(function () {
        // Quando a configuração por cliente for NAO AVERBA, desabilita todas configs
        if ($("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked')) {
            $("#txtCodigoATMCliente").val("");
            $("#txtCodigoATMCliente").attr("disabled", true);
            $("#txtUsuarioATMCliente").val("");
            $("#txtUsuarioATMCliente").attr("disabled", true);
            $("#txtSenhaATMCliente").val("");
            $("#txtSenhaATMCliente").attr("disabled", true);
            $("#txtTokenSeguroBradescoCliente").val("");
            $("#txtTokenSeguroBradescoCliente").attr("disabled", true);
            $("#selSeguradoraAverbacaoCliente").val("");
        }
    });

    $("#selSeguradoraAverbacaoCliente").change(function () {
        ParametrosAverbacaoCliente();
    });
    ParametrosAverbacaoCliente();
});
function RetornoConsultaCliente(cliente) {
    BuscarCliente(cliente.CPFCNPJ);
}

function BuscarCliente(cpfCnpj) {
    cpfCnpj = cpfCnpj.replace(/[^0-9]/g, '');

    if (cpfCnpj.length == 14 ? ValidarCNPJ(cpfCnpj) : ValidarCPF(cpfCnpj)) {
        executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cpfCnpj }, function (r) {
            if (r.Sucesso) {
                if (r.Objeto != null) {
                    $("#txtCliente").val(r.Objeto.Nome).data("CodigoCliente", r.Objeto.CPF_CNPJ.replace(/[^0-9]/g, ''));
                }
            } else {
                jAlert(r.Erro, "Erro");
                LimparAverbacao();
            }
        });
    } else {
        jAlert("O CPF/CNPJ digitado é inválido.", "Atenção");
        LimparAverbacao();
    }
}

function ValidarAverbacao() {
    return $("#txtCliente").data("CodigoCliente") != null;
}

function SalvarAverbacao() {
    if (!ValidarAverbacao())
        return jAlert("Cliente é obrigatório.", "Atenção");

    var informacaoAverbacao = {
        Id: Globalize.parseInt($("#hddIdAverbacaoEmEdicao").val()),
        CnpjCliente: $("#txtCliente").data("CodigoCliente"),
        Nome: $("#txtCliente").val(),
        Tipo: $("#selTipoAverbacao :selected").val(),
        IntegradoraAverbacao: $("#selSeguradoraAverbacaoCliente :selected").val(),
        CodigoAverbacao: $("#txtCodigoATMCliente").val(),
        UsuarioAverbacao: $("#txtUsuarioATMCliente").val(),
        SenhaAverbacao: $("#txtSenhaATMCliente").val(),
        TokenAverbacao: $("#txtTokenSeguroBradescoCliente").val(),
        RaizCNPJ: $("#chkAverbacaoConfiguracaoClienteRaizCNPJ").prop('checked'),
        NaoAverbar: $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked'),
        TipoCTeAverbacao: "99", //$("#selTipoCTeAverbacaoCliente").val(),
        Excluir: false
    };
    var infomacoesAverbacoes = $("#hddInformacoesAverbacao").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacao").val());
    if (informacaoAverbacao.Id == 0) {
        informacaoAverbacao.Id = -(infomacoesAverbacoes.length + 1);
    }
    for (var i = 0; i < infomacoesAverbacoes.length; i++) {
        if (infomacoesAverbacoes[i].Id == informacaoAverbacao.Id) {
            infomacoesAverbacoes.splice(i, 1);
            break;
        }
    }
    infomacoesAverbacoes.push(informacaoAverbacao);
    $("#hddInformacoesAverbacao").val(JSON.stringify(infomacoesAverbacoes));
    RenderizarAverbacoes();
    LimparAverbacao();
}

function RenderizarAverbacoes() {
    var infomacoesAverbacoes = $("#hddInformacoesAverbacao").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacao").val());

    $("#tblAverbacoes tbody").html("");
    for (var i = 0; i < infomacoesAverbacoes.length; i++) {
        var dadosDaAverbacao = infomacoesAverbacoes[i];

        if (!dadosDaAverbacao.Excluir) {
            var Descricao = $("#selTipoAverbacao option[value=" + dadosDaAverbacao.Tipo + "]").text();
            var Integradora = '';
            if (dadosDaAverbacao.NaoAverbar)
                Integradora = "Não averbar";
            else if (dadosDaAverbacao.IntegradoraAverbacao == null || dadosDaAverbacao.IntegradoraAverbacao == '')
                Integradora = "Não Definida";
            else
                Integradora = $("#selSeguradoraAverbacaoCliente option[value=" + dadosDaAverbacao.IntegradoraAverbacao + "]").text();
            var raizCNPJ = "Não";
            if (dadosDaAverbacao.RaizCNPJ != null && dadosDaAverbacao.RaizCNPJ)
                raizCNPJ = "Sim";
            else
                raizCNPJ = "Não";

            var $row = $("<tr><td>" + dadosDaAverbacao.Nome + "</td><td>" + raizCNPJ + "</td><td>" + Descricao + "</td><td>" + Integradora + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarAverbacao(" + JSON.stringify(infomacoesAverbacoes[i]) + ")'>Editar</button></td></tr>");

            $("#tblAverbacoes tbody").append($row);
        }
    }
    if ($("#tblAverbacoes tbody").html() == "")
        $("#tblAverbacoes tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
}

//function ExcluirAverbacao(averbacao) {
//    var id = averbacao.Id;
//    var infomacoesAverbacoes = $("#hddInformacoesAverbacao").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacao").val());
//    for (var i = 0; i < infomacoesAverbacoes.length; i++) {
//        if (infomacoesAverbacoes[i].Id == id) {
//            if (id <= 0)
//                infomacoesAverbacoes.splice(i, 1);
//            else
//                infomacoesAverbacoes[i].Excluir = true;
//            break;
//        }
//    }
//    $("#hddInformacoesAverbacao").val(JSON.stringify(infomacoesAverbacoes));

//    RenderizarAverbacoes();
//    LimparAverbacao();
//}

function ExcluirAverbacao() {
    var id = $("#hddIdAverbacaoEmEdicao").val();
    var infomacoesAverbacoes = $("#hddInformacoesAverbacao").val() == "" ? new Array() : JSON.parse($("#hddInformacoesAverbacao").val());
    for (var i = 0; i < infomacoesAverbacoes.length; i++) {
        if (infomacoesAverbacoes[i].Id == id) {
            if (id <= 0)
                infomacoesAverbacoes.splice(i, 1);
            else
                infomacoesAverbacoes[i].Excluir = true;
            break;
        }
    }
    $("#hddInformacoesAverbacao").val(JSON.stringify(infomacoesAverbacoes));

    RenderizarAverbacoes();
    LimparAverbacao();
}

function EditarAverbacao(averbacao) {
    LimparAverbacao();

    $("#hddIdAverbacaoEmEdicao").val(averbacao.Id);
    $("#selTipoAverbacao").val(averbacao.Tipo);
    $("#txtCliente").val("").data("CodigoCliente", averbacao.CnpjCliente);
    $("#txtCliente").val(averbacao.Nome)
    $("#selSeguradoraAverbacaoCliente").val(averbacao.IntegradoraAverbacao);
    $("#txtCodigoATMCliente").val(averbacao.CodigoAverbacao);
    $("#txtUsuarioATMCliente").val(averbacao.UsuarioAverbacao);
    $("#txtSenhaATMCliente").val(averbacao.SenhaAverbacao);
    $("#txtTokenSeguroBradescoCliente").val(averbacao.TokenAverbacao);
    $("#chkAverbacaoConfiguracaoClienteRaizCNPJ").prop('checked', averbacao.RaizCNPJ);
    $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked', averbacao.NaoAverbar);
    //$("#selTipoCTeAverbacaoCliente").val(averbacao.TipoCTeAverbacao);
    ParametrosAverbacaoCliente();

    $("#btnExcluirAverbacao").show();
}

function LimparAverbacao() {
    $("#hddIdAverbacaoEmEdicao").val('0');
    $("#selTipoAverbacao").val($("#selTipoAverbacao option:first").val());
    $("#txtCliente").val("").data("CodigoCliente", null);
    $("#selSeguradoraAverbacaoCliente").val($("#selSeguradoraAverbacaoCliente option:first").val());
    $("#txtCodigoATMCliente").val('');
    $("#txtUsuarioATMCliente").val('');
    $("#txtSenhaATMCliente").val('');
    $("#txtTokenSeguroBradescoCliente").val('');
    $("#chkAverbacaoConfiguracaoClienteRaizCNPJ").prop('checked', false);
    $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked', false);
    //$("#selTipoCTeAverbacaoCliente").val($("#selTipoCTeAverbacaoCliente option:first").val());
    ParametrosAverbacaoCliente();
    $("#btnExcluirAverbacao").hide();    
}

function ParametrosAverbacao() {
    if ($("#selSeguradoraAverbacao").val() == "A") {
        $("#txtCodigoATM").attr("disabled", false);
        $("#txtUsuarioATM").attr("disabled", false);
        $("#txtSenhaATM").attr("disabled", false);
        $("#txtTokenSeguroBradesco").val("");
        $("#txtTokenSeguroBradesco").attr("disabled", true);
        //$("#txtWsdlQuorum").val("");
        $("#txtWsdlQuorum").attr("disabled", false);
        $("#chkAverbarMDFe").attr("disabled", false);        
    } else if ($("#selSeguradoraAverbacao").val() == "B") {
        $("#txtCodigoATM").val("");
        $("#txtCodigoATM").attr("disabled", true);
        $("#txtUsuarioATM").val("");
        $("#txtUsuarioATM").attr("disabled", true);
        $("#txtSenhaATM").val("");
        $("#txtSenhaATM").attr("disabled", true);
        $("#txtTokenSeguroBradesco").attr("disabled", false);
        $("#txtWsdlQuorum").attr("disabled", false);
        $("#chkAverbarMDFe").prop('checked', true);
        $("#chkAverbarMDFe").attr("disabled", false);
    } else if ($("#selSeguradoraAverbacao").val() == "P") {
        $("#txtCodigoATM").val("");
        $("#txtCodigoATM").attr("disabled", true);
        $("#txtUsuarioATM").attr("disabled", false);
        $("#txtSenhaATM").attr("disabled", false);
        $("#txtTokenSeguroBradesco").val("");
        $("#txtTokenSeguroBradesco").attr("disabled", true);
        //$("#txtWsdlQuorum").val("");
        $("#txtWsdlQuorum").attr("disabled", false);
        $("#chkAverbarMDFe").attr("disabled", false);
    } else if ($("#selSeguradoraAverbacao").val() == "E") {
        $("#txtCodigoATM").val("");
        $("#txtCodigoATM").attr("disabled", true);
        $("#txtUsuarioATM").attr("disabled", true);
        $("#txtSenhaATM").attr("disabled", true);
        $("#txtTokenSeguroBradesco").val("");
        $("#txtTokenSeguroBradesco").attr("disabled", true);
        //$("#txtWsdlQuorum").val("");
        $("#txtWsdlQuorum").attr("disabled", true);
        $("#chkAverbarMDFe").attr("disabled", true);
    } else {
        $("#txtCodigoATM").val("");
        $("#txtCodigoATM").attr("disabled", true);
        $("#txtUsuarioATM").val("");
        $("#txtUsuarioATM").attr("disabled", true);
        $("#txtSenhaATM").val("");
        $("#txtSenhaATM").attr("disabled", true);
        $("#txtTokenSeguroBradesco").val("");
        $("#txtTokenSeguroBradesco").attr("disabled", true);
        //$("#txtWsdlQuorum").val("");
        $("#txtWsdlQuorum").attr("disabled", false);
        $("#chkAverbarMDFe").prop('checked', false);
        $("#chkAverbarMDFe").attr("disabled", false);
    }
}

function ParametrosAverbacaoCliente() {
    if ($("#selSeguradoraAverbacaoCliente").val() == "A") {
        $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked', false);
        $("#txtCodigoATMCliente").attr("disabled", false);
        $("#txtUsuarioATMCliente").attr("disabled", false);
        $("#txtSenhaATMCliente").attr("disabled", false);
        $("#txtTokenSeguroBradescoCliente").val("");
        $("#txtTokenSeguroBradescoCliente").attr("disabled", true);
    } else if ($("#selSeguradoraAverbacaoCliente").val() == "B") {
        $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked', false);
        $("#txtCodigoATMCliente").val("");
        $("#txtCodigoATMCliente").attr("disabled", true);
        $("#txtUsuarioATMCliente").val("");
        $("#txtUsuarioATMCliente").attr("disabled", true);
        $("#txtSenhaATMCliente").val("");
        $("#txtSenhaATMCliente").attr("disabled", true);
        $("#txtTokenSeguroBradescoCliente").attr("disabled", false);
    } else if ($("#selSeguradoraAverbacaoCliente").val() == "P") {
        $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked', false);
        $("#txtCodigoATMCliente").val("");
        $("#txtCodigoATMCliente").attr("disabled", true);
        $("#txtUsuarioATMCliente").attr("disabled", false);
        $("#txtSenhaATMCliente").attr("disabled", false);
        $("#txtTokenSeguroBradescoCliente").val("");
        $("#txtTokenSeguroBradescoCliente").attr("disabled", true);
    } else if ($("#selSeguradoraAverbacaoCliente").val() == "E") {
        $("#chkAverbacaoConfiguracaoClienteNaoAverbar").prop('checked', false);
        $("#txtCodigoATMCliente").val("");
        $("#txtCodigoATMCliente").attr("disabled", true);
        $("#txtUsuarioATMCliente").attr("disabled", true);
        $("#txtSenhaATMCliente").attr("disabled", true);
        $("#txtTokenSeguroBradescoCliente").val("");
        $("#txtTokenSeguroBradescoCliente").attr("disabled", true);
    } else {
        $("#txtCodigoATMCliente").val("");
        $("#txtCodigoATMCliente").attr("disabled", true);
        $("#txtUsuarioATMCliente").val("");
        $("#txtUsuarioATMCliente").attr("disabled", true);
        $("#txtSenhaATMCliente").val("");
        $("#txtSenhaATMCliente").attr("disabled", true);
        $("#txtTokenSeguroBradescoCliente").val("");
        $("#txtTokenSeguroBradescoCliente").attr("disabled", true);
    }
}