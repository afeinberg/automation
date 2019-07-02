// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    var options = {
        xAxis: {
            type: 'datetime'
        },
        title: 'Room data',
        series: [{}, {}]
    };
    Highcharts.ajax({
        url: '/sensors/last',
        enablePolling: true,
        success: function (data) {
            options.series[0].data = $.map(data,
                function (dataItem, index) {
                    return {
                        x: dataItem.epoch,
                        y: dataItem.telemetry.temperature
                    }
                });
            options.series[0].name = 'Temperature (C)';
            options.series[1].data = $.map(data,
                function (dataItem, index) {
                    return {
                        x: dataItem.epoch,
                        y: dataItem.telemetry.humidity
                    }
                });
            options.series[1].name = 'Humidity (%)';
            new Highcharts.Chart('hccontainer', options);
        }
    });
});