$(document).ready(function () {

    // Navigation menu

    $('ul#navigation').superfish({
        delay: 1000,
        animation: { opacity: 'show', height: 'show' },
        speed: 'fast',
        autoArrows: true,
        dropShadows: false
    });

    $('ul#navigation li').hover(function () {
        $(this).addClass('sfHover2');
    },
	function () {
	    $(this).removeClass('sfHover2');
	});

    // Live Search
    if ($.liveSearch != undefined) {
        jQuery('#search-bar input[name="q"]').liveSearch({ url: 'live_search.php?q=' });
    }

    //Hover states on the static widgets

    $('.ui-state-default').hover(
		function () { $(this).addClass('ui-state-hover'); },
		function () { $(this).removeClass('ui-state-hover'); }
	);

    //Sortable portlets

    if ($.sortable != undefined) {
        $('.sortable .column').sortable({
            cursor: "move",
            connectWith: '.sortable .column',
            dropOnEmpty: false
        });

        $(".column").disableSelection();

        //Sidebar only sortable boxes
        $(".side_sort").sortable({
            axis: 'y',
            cursor: "move",
            connectWith: '.side_sort'
        });
    }

    //Close/Open portlets
    $(".portlet-header").hover(function () {
        $(this).addClass("ui-portlet-hover");
    },
	function () {
	    $(this).removeClass("ui-portlet-hover");
	});

    $(".portlet-header .ui-icon").click(function () {
        $(this).toggleClass("ui-icon-circle-arrow-n");
        $(this).parents(".portlet:first").find(".portlet-content").toggle();
    });

    // Sidebar close/open (with cookies)

    function close_sidebar() {

        $("#sidebar").addClass('closed-sidebar');
        $("#page_wrapper #page-content #page-content-wrapper").addClass("no-bg-image wrapper-full");
        $("#open_sidebar").show();
        $("#close_sidebar, .hide_sidebar").hide();
    }

    function open_sidebar() {
        $("#sidebar").removeClass('closed-sidebar');
        $("#page_wrapper #page-content #page-content-wrapper").removeClass("no-bg-image wrapper-full");
        $("#open_sidebar").hide();
        $("#close_sidebar, .hide_sidebar").show();
    }

    $('#close_sidebar').click(function () {
        close_sidebar();
        if ($.browser.safari) {
            location.reload();
        }
        $.cookie('sidebar', 'closed');
        $(this).addClass("active");
    });

    $('#open_sidebar').click(function () {
        open_sidebar();
        if ($.browser.safari) {
            location.reload();
        }
        if ($.cookie) $.cookie('sidebar', 'open');
    });

    var sidebar = $.cookie ? $.cookie('sidebar') : '';

    if (sidebar == 'closed') {
        close_sidebar();
    };

    if (sidebar == 'open') {
        open_sidebar();
    };

    /* Tooltip */
    if ($.tooltip) {
        $(function () {
            $('.tooltip').tooltip({
                track: true,
                delay: 0,
                showURL: false,
                showBody: " - ",
                fade: 250
            });
        });
    }

    $('#dialog_link').click(function () {
        $('#dialog').dialog('open');
        return false;
    });

    // Modal Confirmation Link

    $('#modal_confirmation_link').click(function () {
        $('#modal_confirmation').dialog('open');
        return false;
    });

    // Same height

    var sidebarHeight = $("#sidebar").height();
    $("#page-content-wrapper").css({ "minHeight": sidebarHeight });

    // Simple drop down menu

    var myIndex, myMenu, position, space = 20;

    $("div.sub").each(function () {
        $(this).css('left', $(this).parent().offset().left);
        $(this).slideUp('fast');
    });

    $(".drop-down li").hover(function () {
        $("ul", this).slideDown('fast');

        //get the index, set the selector, add class
        myIndex = $(".main1").index(this);
        myMenu = $(".drop-down a.btn:eq(" + myIndex + ")");
    }, function () {
        $("ul", this).slideUp('fast');
    });

});
