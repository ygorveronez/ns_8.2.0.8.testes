function InserirTag(id, text) {
    if (id != null && id.trim() != "") {
        var txtarea = document.getElementById(id);
        var scrollPos = txtarea.scrollTop;
        var strPos = 0;
        var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
        if (br == "ie") {
            txtarea.focus();
            var range = document.selection.createRange();
            range.moveStart('character', -txtarea.value.length);
            strPos = range.text.length;
        } else if (br == "ff") {
            strPos = txtarea.selectionStart;
        }
        var front = (txtarea.value).substring(0, strPos);
        var back = (txtarea.value).substring(strPos, txtarea.value.length);
        txtarea.value = front + text + back;
        strPos = strPos + text.length;
        if (br == "ie") {
            txtarea.focus();
            var range = document.selection.createRange();
            range.moveStart('character', -txtarea.value.length);
            range.moveStart('character', strPos);
            range.moveEnd('character', 0);
            range.select();
        } else if (br == "ff") {
            txtarea.selectionStart = strPos;
            txtarea.selectionEnd = strPos;
            txtarea.focus();
        }
        txtarea.scrollTop = scrollPos;
    }
}

var Global = {

    abrirModal: function (idModal) {
        let idElemento = (idModal);

        if (string.IsNullOrWhiteSpace(idElemento) || idElemento.length == 0)
            throw "Elemento vazio não pode ser encontrado para interagir com o modal.";

        if (idElemento.substr(0, 1) == "#")
            idElemento = idElemento.substr(1, idElemento.length - 1);

        let element = document.getElementById(idElemento);

        if (!element)
            throw "Elemento " + idModal + " não encontrado.";
        $('#' + idModal).modal({ keyboard: true, backdrop: 'static' });
    },

    fecharModal: function (idModal) {
        let idElemento = (idModal);

        if (string.IsNullOrWhiteSpace(idElemento) || idElemento.length == 0)
            throw "Elemento vazio não pode ser encontrado para interagir com o modal.";

        if (idElemento.substr(0, 1) == "#")
            idElemento = idElemento.substr(1, idElemento.length - 1);

        let element = document.getElementById(idElemento);

        if (!element)
            throw "Elemento " + idModal + " não encontrado.";

        let bsModal = bootstrap.Modal.getOrCreateInstance(element, { backdrop: 'static', keyboard: true });

        if (!bsModal)
            throw "Não foi possível obter/criar o modal para o elemento " + idElemento + ".";

        bsModal.hide();
    },
}

var string = {
    IsNullOrWhiteSpace: function (str) {
        if (typeof str === 'undefined' || str == null)
            return true;

        return str.toString().replace(/\s/g, '').length < 1;
    },
    Left: function (str, n) {
        if (n <= 0)
            return "";
        else if (n > String(str).length)
            return str;
        else
            return String(str).substring(0, n);
    },
    Right: function (str, n) {
        if (n <= 0)
            return "";
        else if (n > String(str).length)
            return str;
        else {
            var iLen = String(str).length;
            return String(str).substring(iLen, iLen - n);
        }
    },
    OnlyNumbers: function (str) {
        if (str == null)
            return str;

        return str.replace(/\D+/g, '');
    }
};