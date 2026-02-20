/// <reference path="../app.config.js" />
// este ja serve para dar o load no tree View do Bootstrap conforme a necessidade, por exemplo, ele sempre deve ser carregado após a ligação dos dados via knockout (Rodrigo Romanovski)

function loadTreeView() {
    /*! SmartAdmin - v1.5.2 - 2015-04-23 */
    if ($(".tree > ul") && !mytreebranch) {
        var mytreebranch = $(".tree").find("li:has(ul)").addClass("parent_li").attr("role", "treeitem").find(" > span").attr("title", "Clique para ocultar");
        $(".tree > ul").attr("role", "tree").find("ul").attr("role", "group"), mytreebranch.on("click", function (a) {
            var b = $(this).parent("li.parent_li").find(" > ul > li");
            b.is(":visible") ? (b.hide("fast"), $(this).attr("title", "Clique para expandir")) : (b.show("fast"), $(this).attr("title", "Clique para ocultar")), a.stopPropagation()
        })
        if (_FormularioSomenteLeitura) {
            $(".tree > ul input").prop("disabled", true);
        }
    }
}