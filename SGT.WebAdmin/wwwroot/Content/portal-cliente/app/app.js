var _FormularioSomenteLeitura = false;
var _FiltroPedidoGlobal = {
    texto: '',
    situacao: null
};

$(document).ready(function () {
    $(document).on('click', 'nav a[href="#"], .bootstrapWizard > li > a', function (e) {
        e.preventDefault();
    });

    $(window).on('hashchange', function () {
        checkURL();
    });

    $(".pesquisa-global").on('submit', function (e) {
        e.preventDefault();
        _FiltroPedidoGlobal.texto = $(".search-input").val();

        if (window.location.hash.replace('#', '') != 'Pedidos/Pedido')
            window.location.hash = 'Pedidos/Pedido';
    });

    $(".logout-btn").on('click', function (e) {
        e.preventDefault();
        window.location.href = 'Login';
    });

    var $nav = $('.main-nav');
    var $container = $('#content');

    //setarConfiguracaoPadrao(_BAGConfiguracaoPadrao);
    _CONFIGURACAO_TMS.PermiteAuditar = false;

    var checkURL = function () {
        var url = location.href.split('#').splice(1).join('#') || '/Home';

        if (!url)
            return;

        $nav.find(".active").removeClass("active");

        $nav.find('li:has(a[href="#' + url + '"])').addClass("active");
        var title = ($nav.find('nav a[href="#' + url + '"]').first().text());

        document.title = (title || document.title);

        loadURL(url + location.search, $container);
    }

    var loadURL = function (url, container) {
        $.ajax({
            type: "GET",
            url: url,
            dataType: 'html',
            cache: true, // (warning: setting it to false will cause a timestamp and will call the request twice)
            beforeSend: function () {
                container.removeData().html("");
                container.html('<h1 class="ajax-loading-animation"><i class="fa fa-cog fa-spin"></i> Carregando...</h1>');
            },
            success: function (data, status, xhr) { //Modificar o retorno do sucess para validar a questão do somente leitura. Rodrigo Romanovski
                try {
                    var obj = JSON.parse(data);
                    if (!obj.Authorized)
                        window.location.href = "Login?ReturnUrl=" + obj.RedirectURL;
                    return;

                    if (somenteLeitura != null) {
                        if (somenteLeitura == "true")
                            _FormularioSomenteLeitura = true;
                        else
                            _FormularioSomenteLeitura = false;
                    } else
                        _FormularioSomenteLeitura = false;

                } catch (e) {
                    var somenteLeitura = xhr.getResponseHeader("SomenteLeitura");
                    if (somenteLeitura != null) {
                        if (somenteLeitura == "true")
                            _FormularioSomenteLeitura = true;
                        else
                            _FormularioSomenteLeitura = false;
                    } else
                        _FormularioSomenteLeitura = false;
                }

                container.css({
                    opacity: '0.0'
                }).html(data).delay(50).animate({
                    opacity: '1.0'
                }, 300);
            },
            error: function (xhr, status, thrownError, error) {
                container.html('<h4 class="ajax-loading-error"><i class="fa fa-warning txt-color-orangeDark"></i> Error requesting <span class="txt-color-red">' + url + '</span>: ' + xhr.status + ' <span style="text-transform: capitalize;">' + thrownError + '</span></h4>');
            },
            async: true
        });

    }

    checkURL();
});

function pageSetUp() {

}