var _operadorLogistica;

function buscarDetalhesOperador(callback) {
    if (_operadorLogistica == undefined || _operadorLogistica.Codigo == undefined || _operadorLogistica.Codigo == 0) {
        executarReST("Usuario/BuscarDetalhesOperador", null, function (retorno) {
            if (retorno.Success) {
                _operadorLogistica = retorno.Data;
                callback();
            }
            else
                exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
        });
    } else {
        callback();
    }
}
