
(function () {

    console.log("main.js started");

    var $toggleSidebarBtn = $("#toggleSidebar");


    var $sidebar = $("#sidebar, #wrapper");

    $toggleSidebarBtn.on('click', () => {
        console.log('toggle clicked');
        $sidebar.toggleClass('hide-sidebar');
        if ($sidebar.hasClass('hide-sidebar')) {
            $sidebar.addClass('slow-show');
            $toggleSidebarBtn.text('Show');
        }
        else
            $toggleSidebarBtn.text('Hide');
    });

})();