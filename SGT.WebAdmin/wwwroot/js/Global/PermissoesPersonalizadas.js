function VerificaSePossuiPermissaoEspecial(enumPermissao, permissoesPersonalizadas) {
    var possui = false;
    if (permissoesPersonalizadas != null && permissoesPersonalizadas != undefined) {
        for (var i = 0; i < permissoesPersonalizadas.length; i++) {
            if (permissoesPersonalizadas[i] == enumPermissao) {
                possui = true;
                break;
            }
        }
    }
    return possui;
}
