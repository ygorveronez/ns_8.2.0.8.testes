$(document).ready(function () {
    HeaderAuditoria("Usuario");

    $("#txtDataHabilitacao").mask("99/99/9999");
    $("#txtDataVencimentoHabilitacao").mask("99/99/9999");
    $("#txtDataNascimento").mask("99/99/9999");
    $("#txtDataAdmissao").mask("99/99/9999");
    $("#txtDataValidadeSeguradora").mask("99/99/9999");    

    $("#txtPercentualComissao").priceFormat({ prefix: '' });
    $("#txtSalario").priceFormat({ prefix: '' });

    $("#txtCPFCNPJ").mask("99999999999?999");
    $("#txtCEP").mask("99999-999");

    $("#txtDataHabilitacao").datepicker({ changeMonth: true, changeYear: true });
    $("#txtDataNascimento").datepicker({ changeMonth: true, changeYear: true });
    $("#txtDataAdmissao").datepicker({ changeMonth: true, changeYear: true });
    $("#txtDataVencimentoHabilitacao").datepicker({ changeMonth: true, changeYear: true });
    $("#txtDataValidadeSeguradora").datepicker({ changeMonth: true, changeYear: true });

    $("#txtNome").change(LimparNome);

    CarregarConsultaDeMotoristas("default-search", "default-search", "", Editar, true, false);

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });
});

function LimparNome() {
    var nome = $("#txtNome").val();

    nome = nome.replace(/"/gm, '');
    nome = nome.replace(/'/gm, '');

    $("#txtNome").val(nome);
}
function LimparCampos() {
    HeaderAuditoriaCodigo();
    $("#hddCodigo").val('0');
    $("#txtNome").val('');
    $("#txtCPFCNPJ").val('');
    $("#txtRGIE").val('');
    $("#txtDataNascimento").val('');
    $("#txtDataAdmissao").val('');
    $("#txtTelefone").val('').change();
    $("#selUF").val($("#selUF option:first").val());
    $("#txtDataNascimento").val('');
    $("#txtDataAdmissao").val('');
    $("#txtSalario").val('0,00');
    $("#selLocalidade").html("");
    $("#txtEndereco").val("");
    $("#txtComplemento").val("");
    $("#txtEmail").val('');
    $("#txtUsuario").val('');
    $("#txtSenha").val('');
    $("#txtConfirmacaoSenha").val('');
    $("#txtTipoSanguineo").val('');
    $("#txtNumeroHabilitacao").val('');
    $("#txtDataHabilitacao").val('');
    $("#txtDataVencimentoHabilitacao").val('');
    $("#txtCategoriaHabilitacao").val('');
    $("#txtDataValidadeSeguradora").val('');
    $("#txtMOOP").val('');
    $("#txtPercentualComissao").val('0,00');
    $("#txtPIS").val("");
    $("#txtNumeroCartao").val("");
    $("#selStatus").val($("#selStatus option:first").val());
    $("#selUFRG").val($("#selUFRG option:first").val());
    $("#selSexo").val($("#selSexo option:first").val());
    $("#selOrgaoEmissorRG").val($("#selOrgaoEmissorRG option:first").val());
    $("#txtCEP").val("");
    $("#txtBairro").val("");
    $("#divBodyDadosUsuarios").slideUp();
    $("checkbox").each(function () {
        $(this).attr("checked", false);
    });
    LimparPermissoes();
}
function ValidarCampos() {
    var nome = $("#txtNome").val().trim();
    var cpfCnpj = $("#txtCPFCNPJ").val().trim();
    var localidade = $("#selLocalidade").val();
    var valido = true;
    if (nome != "") {
        CampoSemErro("#txtNome");
    } else {
        valido = false;
        CampoComErro("#txtNome");
    }
    if (cpfCnpj.length == 11) {
        if (ValidarCPF(cpfCnpj)) {
            CampoSemErro("#txtCPFCNPJ");
        } else {
            CampoComErro("#txtCPFCNPJ");
            valido = false;
        }
    } else if (cpfCnpj.length == 14) {
        if (ValidarCNPJ(cpfCnpj)) {
            CampoSemErro("#txtCPFCNPJ");
        } else {
            CampoComErro("#txtCPFCNPJ");
            valido = false;
        }
    } else {
        CampoComErro("#txtCPFCNPJ");
        valido = false;
    }
    if (localidade != "0") {
        CampoSemErro("#selLocalidade");
    } else {
        CampoComErro("#selLocalidade");
        valido = false;
    }
    return valido;
}
function Salvar() {
    if (ValidarCampos()) {
        var dados = {
            Codigo: $("#hddCodigo").val(),
            Permissoes: JSON.stringify(ObterPermissoes()),
            Nome: $("#txtNome").val(),
            CPFCNPJ: $("#txtCPFCNPJ").val(),
            RGIE: $("#txtRGIE").val(),
            DataNascimento: $("#txtDataNascimento").val(),
            DataAdmissao: $("#txtDataAdmissao").val(),
            Salario: $("#txtSalario").val(),
            Telefone: $("#txtTelefone").val(),
            Localidade: $("#selLocalidade").val(),
            Endereco: $("#txtEndereco").val(),
            Complemento: $("#txtComplemento").val(),
            Email: $("#txtEmail").val(),
            Usuario: $("#txtUsuario").val(),
            Senha: $("#txtSenha").val(),
            Status: $("#selStatus").val(),
            ConfirmacaoSenha: $("#txtConfirmacaoSenha").val(),
            TipoSanguineo: $("#txtTipoSanguineo").val(),
            NumeroHabilitacao: $("#txtNumeroHabilitacao").val(),
            DataHabilitacao: $("#txtDataHabilitacao").val(),
            DataVencimentoHabilitacao: $("#txtDataVencimentoHabilitacao").val(),
            CategoriaHabilitacao: $("#txtCategoriaHabilitacao").val(),
            DataValidadeSeguradora: $("#txtDataValidadeSeguradora").val(),
            MOOP: $("#txtMOOP").val(),
            PercentualComissao: $("#txtPercentualComissao").val(),
            PIS: $("#txtPIS").val(),
            NumeroCartao: $("#txtNumeroCartao").val(),
            EstadoRG: $("#selUFRG").val(),
            Sexo: $("#selSexo").val(),
            OrgaoEmissorRG: $("#selOrgaoEmissorRG").val(),
            CEP: $("#txtCEP").val(),
            Bairro: $("#txtBairro").val()
        };
        executarRest("/Motorista/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    }
}
function Editar(usuario) {
    LimparCampos();
    executarRest("/Motorista/ObterDetalhes?callback=?", { CodigoUsuario: usuario.Codigo }, function (r) {
        if (r.Sucesso) {
            HeaderAuditoriaCodigo(r.Objeto.Codigo);
            $("#hddCodigo").val(r.Objeto.Codigo);
            $("#txtNome").val(r.Objeto.Nome);
            $("#txtCPFCNPJ").val(r.Objeto.CPFCNPJ);
            $("#txtRGIE").val(r.Objeto.RGIE);
            $("#txtDataNascimento").val(r.Objeto.DataNascimento);
            $("#txtDataAdmissao").val(r.Objeto.DataAdmissao);
            $("#txtSalario").val(r.Objeto.Salario);
            $("#txtTelefone").val(r.Objeto.Telefone).change();
            $("#selUF").val(r.Objeto.SiglaUF);
            $("#selUFRG").val(r.Objeto.EstadoRG);
            BuscarLocalidades(r.Objeto.SiglaUF, "selLocalidade", r.Objeto.Localidade);
            $("#txtEndereco").val(r.Objeto.Endereco);
            $("#txtComplemento").val(r.Objeto.Complemento);
            $("#txtEmail").val(r.Objeto.Email);
            $("#txtUsuario").val(r.Objeto.Usuario);
            $("#txtSenha").val(r.Objeto.Senha);
            if (r.Objeto.Usuario != "")
                $("#divBodyDadosUsuarios").slideDown();
            $("#txtConfirmacaoSenha").val(r.Objeto.Senha);
            $("#txtTipoSanguineo").val(r.Objeto.TipoSanguineo);
            $("#txtNumeroHabilitacao").val(r.Objeto.NumeroHabilitacao);
            $("#txtDataHabilitacao").val(r.Objeto.DataHabilitacao);
            $("#txtDataVencimentoHabilitacao").val(r.Objeto.DataVencimentoHabilitacao);
            $("#txtCategoriaHabilitacao").val(r.Objeto.CategoriaHabilitacao);
            $("#txtDataValidadeSeguradora").val(r.Objeto.DataValidadeSeguradora);
            $("#txtMOOP").val(r.Objeto.MOOP);
            $("#txtPercentualComissao").val(r.Objeto.PercentualComissao);
            $("#txtPIS").val(r.Objeto.PIS);
            $("#selStatus").val(r.Objeto.Status);
            $("#txtNumeroCartao").val(r.Objeto.NumeroCartao);
            $("#txtCEP").val(r.Objeto.CEP);
            $("#txtBairro").val(r.Objeto.Bairro);
            $("#selSexo").val(r.Objeto.Sexo);
            $("#selOrgaoEmissorRG").val(r.Objeto.OrgaoEmissorRG);
            PreencherPermissoes(r.Objeto.Permissoes);
            LimparNome();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}