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