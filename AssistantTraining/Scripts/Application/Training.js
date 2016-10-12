(function () {
    LoadGrid();
}())
function LoadGrid() {
    var gridName = 'TrainingGrid';
    var pagingUrl = 'Training/GridPager';

    $('.grid-mvc').gridmvc();
    pageGrids[gridName].ajaxify({
        getData: pagingUrl,
        getPagedData: pagingUrl
    });

    var gridName2 = 'TrainingWorkersGrid';
    var pagingUrl2 = 'Training/GridWorkerPager';

    //$('.grid-mvc').gridmvc();
    pageGrids[gridName2].ajaxify({
        getData: pagingUrl2,
        getPagedData: pagingUrl2
    });

    $('#sandbox-container input').datepicker({
        todayBtn: "linked",
        language: "pl",
        autoclose: true
    });

    $('#SaveTrainingWorkersGrid').click(
        function () {
            var $row = $('#grid-mvc').find("table>tbody>tr");    // Finds the closest row <tr>
                $tds = $row.find("td");                               // Finds all children <td> elements

            $.each($tds, function () {                                  // Visits every single <td> element
                console.log($(this).text());                           // Prints out the text within the <td>
            });
        }
    );
}
$('#srch-term-instruction').typeahead(
    {
        source: function (query, process) {
            return $.get('/Training/GetInstructionsByQuery', { query: query }, function (data) {
                console.log(data);
                return process(data);
            });
        }
    });

$('#srch-term-training').typeahead(
    {
        source: function (query, process) {
            return $.get('/Training/GetTrainingNamesByQuery', { query: query }, function (data) {
                console.log(data);
                return process(data);
            });
        }
    });

$(document).ready(function () {
    $("#idInstructionForm").submit(function (e) {
        var val = $('#srch-term-instruction').val();
        $.ajax({
            url: "Training/GetGrid",
            type: "POST",
            data: { term: val }
        })
        .done(function (partialViewResult) {
            $("#refGrid").html(partialViewResult);
        });
        e.preventDefault(); //STOP default action
    });

    $('span > a').click(function (event) {
        event.preventDefault();
        var value = $(this).attr("href");
        var id = $(this).attr("id");
        if (id === 'untrained') {
            $.ajax({
                url: "Training/GetWorkerGrid",
                type: "POST",
                data: { term: value, type: 'untrained' }
            })
            .done(function (partialViewResult) {
                $("#refWorkerGrid").html(partialViewResult);

                LoadGrid();
            });
            //return false; //for good measure
        }
        else if (id === 'trained') {
            $.ajax({
                url: "Training/GetWorkerGrid",
                type: "POST",
                data: { term: value, type: 'trained' }
            })
            .done(function (partialViewResult) {
                $("#refWorkerGrid").html(partialViewResult);
                LoadGrid();
            });
            //return false; //for good measure
        }
    });
});

//function tableToJson(table) {
//    var data = [];

//    // first row needs to be headers
//    var headers = [];
//    for (var i = 0; i < table.rows[0].cells.length; i++) {
//        headers[i] = table.rows[0].cells[i].innerHTML.toLowerCase().replace(/ /gi, '');
//    }

//    // go through cells
//    for (var i = 1; i < table.rows.length; i++) {
//        var tableRow = table.rows[i];
//        var rowData = {};

//        for (var j = 0; j < tableRow.cells.length; j++) {
//            rowData[headers[j]] = tableRow.cells[j].innerHTML;
//        }

//        data.push(rowData);
//    }

//    return data;
//}