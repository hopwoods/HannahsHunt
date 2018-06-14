// Write your JavaScript code.
$(document).ready(function () {
    //Enable Swipe on carousels
    $('.carousel').bcSwipe({ threshold: 50 });

    //DataTables
    $('#UsersList').DataTable({
        responsive: true,
        "autoWidth": false
    });
});


