(function () {
    var gridName = 'TrainingGrid';
    var pagingUrl = 'Training/GridPager';

    $('.grid-mvc').gridmvc();
    pageGrids[gridName].ajaxify({
        getData: pagingUrl,
        getPagedData: pagingUrl
    });
}())

$('input.typeahead').typeahead({
    source: function (query, process) {
        return $.get('/Training/GetNumberTrainings', { query: query }, function (data) {
            console.log(data);
            return process(data);
        });
    }
});

