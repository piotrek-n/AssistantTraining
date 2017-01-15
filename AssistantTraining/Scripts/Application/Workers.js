(function () {

    var $row = $('.table').find("tbody>tr").each(function (i, row) {

        var IsSuspend = $(row).find('[data-name="IsSuspend"]');
        if (IsSuspend[0].innerHTML === 'True') {
            $(row).find('[data-name="FullName"]').css('color', 'red');
        }
    });
}())