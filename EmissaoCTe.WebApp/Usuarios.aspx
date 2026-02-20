<%@ Page Title="Cadastro de Usuários" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="EmissaoCTe.WebApp.Usuarios" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" id="ScriptLocalidades" type="text/javascript">
        $(document).ready(function () {            
            BuscarUFs("selUF");
            $("#selUF").change(function () {
                BuscarLocalidades($(this).val(), "selLocalidade", null);
            });
        });
        function BuscarLocalidades(uf, idSelect, codigo) {
            executarRest("/Localidade/BuscarPorUF?callback=?", { UF: uf }, function (r) {
                if (r.Sucesso) {
                    RenderizarLocalidades(r.Objeto, idSelect, codigo);
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarLocalidades(localidades, idSelect, codigo) {
            var selLocalidades = document.getElementById(idSelect);
            selLocalidades.options.length = 0;
            for (var i = 0; i < localidades.length; i++) {
                var optn = document.createElement("option");
                optn.text = localidades[i].Descricao;
                optn.value = localidades[i].Codigo;
                if (codigo != null) {
                    if (codigo == localidades[i].Codigo) {
                        optn.setAttribute("selected", "selected");
                    }
                }
                selLocalidades.options.add(optn);
            }
        }
        function BuscarUFs(idSelect) {
            executarRest("/Estado/BuscarTodos?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarUFs(r.Objeto, idSelect);
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarUFs(ufs, idSelect) {
            var selUFs = document.getElementById(idSelect);
            selUFs.options.length = 0;
            var optn = document.createElement("option");
            optn.text = 'Selecione';
            optn.value = '0';
            selUFs.options.add(optn);
            for (var i = 0; i < ufs.length; i++) {
                var optn = document.createElement("option");
                optn.text = ufs[i].Sigla + " - " + ufs[i].Nome;
                optn.value = ufs[i].Sigla;
                selUFs.options.add(optn);
            }
        }
    </script>
    <script defer="defer" id="ScriptPermissoes" type="text/javascript">
        $(document).ready(function () {
            ObterFormularios();
        });
        function ObterFormularios() {
            executarRest("/Pagina/BuscarPaginasDaEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    RenderizarFormularios(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RenderizarFormularios(formularios) {
            $("#divPermissoesPaginas").html("");
            var idDivPai = 'divPermissoesPaginas';
            for (var i = 0; i < formularios.length; i++) {

                var nomeGrupo = formularios[i].Grupo.replace(/\s/g, "");
                var idBody = "divBodyPermissoes_" + nomeGrupo;
                var idCheckbox = "chkCheckUncheck_" + nomeGrupo;

                var html = '';
                html += '<div class="panel panel-default"><div class="panel-heading">';
                html += '    <h4 class="panel-title">';
                html += '        <input type="checkbox" id="' + idCheckbox + '" />';
                html += '        <a class="accordion-toggle" data-toggle="collapse" data-parent="#' + idDivPai + '" href="#' + idBody + '">' + formularios[i].Grupo + '</a>';
                html += '    </h4>';
                html += '</div>';
                html += '<div id="' + idBody + '" class="panel-collapse collapse">';
                html += '    <div class="panel-body">';
                html += '        <div class="table-responsive">'
                html += '            <table class="table table-bordered table-condensed table-hover">';
                html += '                <thead>';
                html += '                    <th>Página</th>';
                html += '                    <th style="width: 60px; text-align: center;">Acesso</th>';
                html += '                    <th style="width: 60px; text-align: center;">Incluir</th>';
                html += '                    <th style="width: 60px; text-align: center;">Alterar</th>';
                html += '                    <th style="width: 60px; text-align: center;">Excluir</th>';
                html += '                </thead>';
                html += '                <tbody>';

                for (var j = 0; j < formularios[i].Paginas.length; j++) {
                    html += '                <tr class="tr_pagina_permissao" id="tr_' + formularios[i].Paginas[j].Codigo + '">';
                    html += '                    <td>' + formularios[i].Paginas[j].Descricao + '</td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" class="chkPermissaoAcesso" onclick="AlterarEstadoAcesso(this);" id="chkAcesso_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" disabled="disabled" id="chkIncluir_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" disabled="disabled" id="chkAlterar_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                    <td style="text-align: center;"><input type="checkbox" disabled="disabled" id="chkExcluir_' + formularios[i].Paginas[j].Codigo + '" /></td>';
                    html += '                </tr>'
                }

                html += '                </tbody>';
                html += '            </table>';
                html += '        </div>';
                html += '    </div>';
                html += '</div></div>';

                $("#divPermissoesPaginas").append(html);

                $("#chkCheckUncheck_" + formularios[i].Grupo.replace(/\s/g, "")).click(function (e) {
                    SelecionarTodosDoGrupo(this);
                    e.stopPropagation();
                });
            }
        }
        function SelecionarTodosDoGrupo(chk) {
            var grupo = chk.id.split("_")[1];
            $('#divBodyPermissoes_' + grupo + ' input[type=checkbox]').each(function () {
                if (this.id.indexOf('chkIncluir_') > -1 || this.id.indexOf('chkAlterar_') > -1 || this.id.indexOf('chkExcluir_') > -1)
                    $(this).prop('disabled', !chk.checked);
                $(this).prop('checked', chk.checked);
            });
        }
        function ObterPermissoes() {
            var permissoes = new Array();
            $("#divPermissoesPaginas .tr_pagina_permissao").each(function () {
                var id = this.id.split('_')[1];
                permissoes.push({
                    Codigo: id,
                    Acesso: $("#chkAcesso_" + id)[0].checked,
                    Incluir: $("#chkIncluir_" + id)[0].checked,
                    Alterar: $("#chkAlterar_" + id)[0].checked,
                    Excluir: $("#chkExcluir_" + id)[0].checked
                });
            });
            return permissoes;
        }
        function PreencherPermissoes(permissoes) {
            for (var i = 0; i < permissoes.length; i++) {
                $("#chkAcesso_" + permissoes[i].Codigo).prop("checked", permissoes[i].Acesso);
                $("#chkIncluir_" + permissoes[i].Codigo).prop({ checked: permissoes[i].Incluir, disabled: !permissoes[i].Acesso });
                $("#chkAlterar_" + permissoes[i].Codigo).prop({ checked: permissoes[i].Alterar, disabled: !permissoes[i].Acesso });
                $("#chkExcluir_" + permissoes[i].Codigo).prop({ checked: permissoes[i].Excluir, disabled: !permissoes[i].Acesso });
            }
        }
        function LimparPermissoes() {
            $(".chkPermissaoAcesso").each(function () {
                $(this).prop("checked", false);
                AlterarEstadoAcesso(this);
            });
        }
        function AlterarEstadoAcesso(chk) {
            var id = chk.id.split('_')[1];
            if (chk.checked) {
                $("#chkIncluir_" + id).prop("disabled", false);
                $("#chkAlterar_" + id).prop("disabled", false);
                $("#chkExcluir_" + id).prop("disabled", false);
            } else {
                $("#chkIncluir_" + id).prop({ disabled: true, checked: false });
                $("#chkAlterar_" + id).prop({ disabled: true, checked: false });
                $("#chkExcluir_" + id).prop({ disabled: true, checked: false });
            }
        }

        function ControlarStatusPermissoes(status) {
            var grupo = chk.id.split("_")[1];
            $('#divBodyPermissoes_' + grupo + ' input[type=checkbox]').each(function () {
                if (this.id.indexOf('chkIncluir_') > -1 || this.id.indexOf('chkAlterar_') > -1 || this.id.indexOf('chkExcluir_') > -1)
                    $(this).prop('disabled', status);
            });
        }
    </script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtCPFCNPJ").mask("99999999999?999");
            $("#txtDataNascimento").datepicker();
            $("#txtDataAdmissao").datepicker();
            $("#txtSalario").priceFormat({ prefix: '' });
            $("#txtDataNascimento").mask("99/99/9999");
            $("#txtDataAdmissao").mask("99/99/9999");
            CarregarConsultaDeUsuarios("default-search", "default-search", Editar, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            LimparCampos();
        });
        function LimparCampos() {
            HeaderAuditoria("");
            HeaderAuditoriaCodigo(0); 
            $("#hddCodigo").val('0');
            $("#txtNome").val('');
            $("#txtCPFCNPJ").val('');
            $("#txtRGIE").val('');
            $("#txtDataNascimento").val('');
            $("#txtDataAdmissao").val('');
            $("#txtTelefone").val('').change();
            $("#selUF").val($("#selUF option:first").val());
            $("#txtSalario").val('0,00');
            $("#selLocalidade").html("");
            $("#txtEndereco").val("");
            $("#txtComplemento").val("");
            $("#txtEmail").val('');
            $("#txtUsuario").val('');
            $("#txtSenha").val('');
            $("#txtConfirmacaoSenha").val('');
            $("#chkAlterarSenhaAcesso").attr("checked", true);
            $("#selStatus").val($("#selStatus option:first").val());
            $("checkbox").each(function () {
                $(this).attr("checked", false);
            });
            $("#txtPerfil").val('');
            $("#hddCodigoPerfil").val('0');
            LimparPermissoes();
            LimparCamposSerie();
            $("#hddSeries").val("");
            RenderizarSeries();
        }
        function ValidarCampos() {
            var nome = $("#txtNome").val().trim();
            var cpfCnpj = $("#txtCPFCNPJ").val().trim();
            var localidade = $("#selLocalidade").val();
            var email = $("#txtEmail").val();
            var usuario = $("#txtUsuario").val();
            var senha = $("#txtSenha").val();
            var confirmacaoSenha = $("#txtConfirmacaoSenha").val();
            var valido = true;
            if (nome != "") {
                CampoSemErro("#txtNome");
            } else {
                valido = false;
                CampoComErro("#txtNome");
            }
            if (cpfCnpj.length == 11) {
                if (ValidarCPF(cpfCnpj) || cpfCnpj == "11111111111") {
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
            if (ValidarEmail(email)) {
                CampoSemErro("#txtEmail");
            } else {
                CampoComErro("#txtEmail");
                valido = false;
            }
            if (usuario.length >= 5) {
                CampoSemErro("#txtUsuario");
            } else {
                CampoComErro("#txtUsuario");
                valido = false;
            }
            if (senha.length >= 5 && (senha == confirmacaoSenha)) {
                CampoSemErro("#txtSenha");
                CampoSemErro("#txtConfirmacaoSenha");
            } else {
                CampoComErro("#txtSenha");
                CampoComErro("#txtConfirmacaoSenha");
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
                    Series: $("#hddSeries").val(),
                    AlterarSenhaAcesso: $("#chkAlterarSenhaAcesso")[0].checked,
                    CodigoPerfil: $("#hddCodigoPerfil").val()
                };
                executarRest("/Usuario/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }
        function Editar(usuario) {
            LimparCampos();
            executarRest("/Usuario/ObterDetalhes?callback=?", { CodigoUsuario: usuario.Codigo }, function (r) {
                if (r.Sucesso) {
                    HeaderAuditoria("Usuario");
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
                    BuscarLocalidades(r.Objeto.SiglaUF, "selLocalidade", r.Objeto.Localidade);
                    $("#txtEndereco").val(r.Objeto.Endereco);
                    $("#txtComplemento").val(r.Objeto.Complemento);
                    $("#txtEmail").val(r.Objeto.Email);
                    $("#txtUsuario").val(r.Objeto.Usuario);
                    $("#txtSenha").val(r.Objeto.Senha);
                    $("#txtConfirmacaoSenha").val(r.Objeto.Senha);
                    $("#selStatus").val(r.Objeto.Status);
                    PreencherPermissoes(r.Objeto.Permissoes);
                    $("#hddSeries").val(JSON.stringify(r.Objeto.Series));
                    $("#chkAlterarSenhaAcesso").attr("checked", r.Objeto.AlterarSenhaAcesso);
                    $("#txtPerfil").val(r.Objeto.DescricaoPerfil);
                    $("#hddCodigoPerfil").val(r.Objeto.CodigoPerfil);
                    if (r.Objeto.CodigoPerfil > 0)
                        $("#divPermissoesPaginas").hide();
                    else
                        $("#divPermissoesPaginas").show();

                    RenderizarSeries();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>


    <script defer="defer" type="text/javascript" id="ScriptSeries">
        $(document).ready(function () {
            CarregarConsultaDeSeries("btnBuscarSerie", "btnBuscarSerie", "A", RetornoConsultaSerie, true, false);
            $("#txtSerie").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoSerie").val("0");
                        $("#hddTipoSerie").val("0");                        
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#btnSalvarSerie").click(function () {
                SalvarSerie();
            });
            $("#btnExcluirSerie").click(function () {
                ExcluirSerie();
            });
            $("#btnCancelarSerie").click(function () {
                LimparCamposSerie();
            });
        });
        function RetornoConsultaSerie(serie) {
            $("#txtSerie").val(serie.Numero);
            $("#hddCodigoSerie").val(serie.Codigo);
            $("#hddTipoSerie").val(serie.DescricaoTipo);            
        }
        function ValidarSerie() {
            var serie = Globalize.parseInt($("#hddCodigoSerie").val());
            var valido = true;
            if (serie > 0) {
                CampoSemErro("#txtSerie");
            } else {
                CampoComErro("#txtSerie");
                valido = false;
            }
            return valido;
        }
        function SalvarSerie() {
            if (ValidarSerie()) {
                var series = $("#hddSeries").val() == "" ? new Array() : JSON.parse($("#hddSeries").val());
                var serie = { Codigo: $("#hddCodigoSerie").val(), Numero: $("#txtSerie").val(), DescricaoTipo: $("#hddTipoSerie").val() };
                for (var i = 0; i < series.length; i++) {
                    if (serie.Codigo == series[i].Codigo) {
                        series.splice(i, 1);
                    }
                }
                series.push(serie);
                $("#hddSeries").val(JSON.stringify(series));
                RenderizarSeries();
                LimparCamposSerie();
            }
        }
        function EditarSerie(serie) {
            $("#txtSerie").val(serie.Numero);
            $("#hddCodigoSerie").val(serie.Codigo);
            $("#btnExcluirSerie").show();
        }
        function ExcluirSerie() {
            jConfirm("Deseja realmente excluir esta série?", "Atenção", function (r) {
                if (r) {
                    var series = $("#hddSeries").val() == "" ? new Array() : JSON.parse($("#hddSeries").val());
                    for (var i = 0; i < series.length; i++) {
                        if ($("#hddCodigoSerie").val() == series[i].Codigo) {
                            series.splice(i, 1);
                        }
                    }
                    $("#hddSeries").val(JSON.stringify(series));
                    RenderizarSeries();
                    LimparCamposSerie();
                }
            });
        }
        function LimparCamposSerie() {
            $("#txtSerie").val('');
            $("#hddCodigoSerie").val('0');
            $("#btnExcluirSerie").hide();
        }
        function RenderizarSeries() {
            var series = $("#hddSeries").val() == "" ? new Array() : JSON.parse($("#hddSeries").val());
            $("#tblSeries tbody").html("");
            if (series.length > 0) {
                for (var i = 0; i < series.length; i++) {
                    $("#tblSeries tbody").append("<tr><td>" + series[i].Numero + "</td><td>" + series[i].DescricaoTipo + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarSerie(" + JSON.stringify(series[i]) + ");'>Editar</button></td></tr>");
                }
            }
            if ($("#tblSeries tbody").html() == "")
                $("#tblSeries tbody").html("<tr><td colspan='3'>Nenhum registro encontrado.</td></tr>");
        }

    </script>

    <script defer="defer" type="text/javascript" id="ScriptPerfil">
        $(document).ready(function () {
            CarregarConsultaDePerfilAtivo("btnBuscarPerfil", "btnBuscarPerfil", RetornoConsultaPerfil, true, false);

            $("#txtPerfil").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoPerfil").val("0");
                        $("#divPermissoesPaginas").show();
                    } else {
                        e.preventDefault();
                    }
                }
            });
        });

        function RetornoConsultaPerfil(perfil) {
            $("#txtPerfil").val(perfil.Descricao);
            $("#hddCodigoPerfil").val(perfil.Codigo);

            executarRest("/PerfilPermissao/ObterDetalhes?callback=?", { Codigo: perfil.Codigo }, function (r) {
                if (r.Sucesso) {
                    PreencherPermissoes(r.Objeto.Permissoes);
                    $("#divPermissoesPaginas").hide();
                    
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="" />
        <input type="hidden" id="hddCodigoSerie" value="0" />
        <input type="hidden" id="hddTipoSerie" value="" />
        <input type="hidden" id="hddSeries" value="" />
        <input type="hidden" id="hddCodigoPerfil" value="0" />        
    </div>
    <div class="page-header">
        <h2>Cadastro de Usuários
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-7 col-md-5 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Nome*:
                </span>
                <input type="text" id="txtNome" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-5 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">CPF/CNPJ*:
                </span>
                <input type="text" id="txtCPFCNPJ" class="form-control maskedInput" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">RG/IE:
                </span>
                <input type="text" id="txtRGIE" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Nasc.:
                </span>
                <input type="text" id="txtDataNascimento" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Admissão:
                </span>
                <input type="text" id="txtDataAdmissao" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Salário:
                </span>
                <input type="text" id="txtSalario" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Telefone:
                </span>
                <input type="text" id="txtTelefone" class="form-control maskedInput phone" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">UF*:
                </span>
                <select id="selUF" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Município*:
                </span>
                <select id="selLocalidade" class="form-control">
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
            <div class="input-group">
                <span class="input-group-addon">Endereço:
                </span>
                <input type="text" id="txtEndereco" class="form-control" maxlength="80" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Complemento:
                </span>
                <input type="text" id="txtComplemento" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-8 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">E-mail*:
                </span>
                <input type="text" id="txtEmail" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Usuário*:
                </span>
                <input type="text" id="txtUsuario" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Senha*:
                </span>
                <input type="password" id="txtSenha" class="form-control" maxlength="15" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Confirmação da Senha">Conf. Senha*</abbr>:
                </span>
                <input type="password" id="txtConfirmacaoSenha" class="form-control" maxlength="15" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkAlterarSenhaAcesso" />
                        Solicitar alteração de senha no próximo acesso
                    </label>
                </div>
            </div>
        </div>
    </div>
    <h3>Séries
    </h3>
    <div class="row" style="margin-top: 10px;">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Série:
                </span>
                <input type="text" id="txtSerie" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarSerie" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
            <div class="input-group">
                <button type="button" id="btnSalvarSerie" class="btn btn-primary">Salvar</button>
                <button type="button" id="btnExcluirSerie" class="btn btn-danger" style="display: none;">Excluir</button>
                <button type="button" id="btnCancelarSerie" class="btn btn-default">Cancelar</button>
            </div>
        </div>
    </div>
    <div class="table-responsive">
        <table id="tblSeries" class="table table-bordered table-condensed table-hover">
            <thead>
                <tr>
                    <th style="width: 30%;">Número</th>
                    <th style="width: 60%;">Tipo</th>
                    <th style="width: 10%">Opções</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td colspan="2">Nenhum registro encontrado.</td>
                </tr>
            </tbody>
        </table>
    </div>
    <h3>Permissões
    </h3>
    <div class="row" style="margin-top: 10px;">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Pefil:
                </span>
                <input type="text" id="txtPerfil" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPerfil" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="panel-group" id="divPermissoesPaginas" style="margin-top: 10px; margin-bottom: 15px;">
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
