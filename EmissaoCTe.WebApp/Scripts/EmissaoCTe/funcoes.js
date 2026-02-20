function BuscaMenorProximoCodigo(lista, key) {
    if (!key) key = "Id";
    var proximoCodigo = 0;

    for (var i in lista) {
        var item = lista[i];

        if (item[key] < 0) {
            var codigoAtual = Math.abs(item[key]);
            if (codigoAtual > proximoCodigo)
                proximoCodigo = codigoAtual;
        }
    }

    return -(proximoCodigo + 1);
}