
$.draggImagem = function (opt) {
    var self = this;
    var initialY = 0;
    var initialScrollTop = 0;
    var ACTIVE = false;
    var settings = $.extend({}, {
        image: '',
        container: '',
        centralize: false
    }, opt);

    var HandleImageClick = function (e) {
        ACTIVE = true;
        initialY = e.clientY;
        initialScrollTop = $(this).parent().scrollTop();
    }

    var HandleImageRelease = function (e) {
        ACTIVE = false;
    }

    var HandleScroll = function (e) {
        e.preventDefault();
    }

    var HandleImageMove = function (e) {
        if (!ACTIVE) return;
        e.preventDefault();

        var currentY = e.clientY - initialY;
        SetScroll(currentY * 1.2, this);
    }

    var SetScroll = function (positionY, element) {
        var $element = $(element).parent();
        $element.scrollTop(initialScrollTop - positionY);
    }

    $(settings.container).on("mousedown", settings.image, HandleImageClick);
    $(settings.container).on("mouseup", settings.image, HandleImageRelease);
    $(settings.container).on("mousemove", settings.image, HandleImageMove);

    return {
        centralize: function () {
            $(settings.container).find(settings.image).each(function () {
                var $img = $(this);
                var $contain = $img.parent();

                $contain.scrollTop(($img.height() - $contain.height()) / 2);
            })
        },
        top: function () {
            SetScroll(0, self);
        }
    };
}