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

    $('[data-gridname="TrainingWorkersGrid"]').click(function () {

    });

    $('#myform').validate({ // initialize the plugin
        rules: {
            field1: {
                required: true
            }
        },
        submitHandler: function (form) { // for demo
            //alert('valid form submitted'); // for demo
            return false; // for demo
        }
    });

    $('#SaveTrainingWorkersGrid').click(
        function () {
            if ($('[data-gridname="TrainingWorkersGrid"]').find("table>tbody>tr").find('[type=checkbox]').size() == 0)
                return;
            if ($('#myform').valid()) {
                
                var trainingWorkersGrid = {};
                var workers = [];
                trainingWorkersGrid.Workers = workers;
                trainingWorkersGrid.WrainingDate = "";
                trainingWorkersGrid.TrainingNumber = "";

                var $row = $('[data-gridname="TrainingWorkersGrid"]').find("table>tbody>tr").each(function (i, row) {

                    var checked = $(row).find('[type=checkbox]').prop('checked');
                    var workerID = $(row).find('[data-name="WorkerID"]').html();
                    var trainingNameId = $(row).find('[data-name="TrainingNameId"]').html();

                    var worker = {
                        "WorkerID": workerID,
                        "TrainingNameId": trainingNameId,
                        "Checked": checked
                    };
                    trainingWorkersGrid.Workers.push(worker);
                });
                var trainingDate = $('#TrainingDate').val();
                var trainingNumber = $('#TrainingNumber').val();

                trainingWorkersGrid.TrainingDate = trainingDate;
                trainingWorkersGrid.TrainingNumber = trainingNumber;

                console.log(JSON.stringify(trainingWorkersGrid));

                $.ajax({
                    url: "Training/UpdateTrainings",
                    type: "POST",
                    data: JSON.stringify(trainingWorkersGrid),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    error: function(jqXHR, textStatus, errorThrown) {
                        $("#refWorkerGrid").html(jqXHR.responseText);
                        LoadGrid();
                    },
                    success: function (response) {
                        $("#refWorkerGrid").html(partialViewResult);
                        LoadGrid();
                    }
                });

            } else {
               // alert('form is not valid');
            }



        }
    );

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

    //$('#TrainingNumber').val('nr');
}
//
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
        var val = $('#srchterminstruction').val();
        $.ajax({
            url: "Training/GetGridByInstruction",
            type: "POST",
            data: { term: val }
        })
        .done(function (partialViewResult) {
            $("#refGrid").html(partialViewResult);
            LoadGrid();
        });
        e.preventDefault(); //STOP default action
    });
    $("#idTrainingForm").submit(function (e) {
        var val = $('#srchtermtraining').val();
        $.ajax({
            url: "Training/GetGridByTraining",
            type: "POST",
            data: { term: val }
        })
        .done(function (partialViewResult) {
            $("#refGrid").html(partialViewResult);
            LoadGrid();
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