$(document).ready(function () {
    BuscarUFs(["selUF", "selUFRG"]);

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
    for (var x = 0; x < idSelect.length; x++) {
        var selUFs = document.getElementById(idSelect[x]);
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
}