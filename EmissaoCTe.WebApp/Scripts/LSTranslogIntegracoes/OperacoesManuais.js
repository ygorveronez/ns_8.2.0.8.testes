$(document).ready(function () {
});

// OPCOES DA GRID
function EnviarManualmente(data) {
    jConfirm("Tem certeza que deseja enviar novamente?", "Enviar Manualmente", function (opt) {
        if (!opt) return;
        executarRest("/LSTranslogIntegracao/EnviarManualmente?callback=?", { Codigo: data.data.Codigo }, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Integração enviada com sucesso", "Sucesso!");
                AtualizarGrid();
            }            
            else
                ExibirMensagemErro(r.Erro, "Atenção!");
        });
    });
}

function ConsultarManualmente(data) {
    executarRest("/LSTranslogIntegracao/ConsultarManualmente?callback=?", { Codigo: data.data.Codigo }, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Integração consultada com sucesso", "Sucesso!");
            AtualizarGrid();
        }
        else
            ExibirMensagemErro(r.Erro, "Atenção!");
    });
}