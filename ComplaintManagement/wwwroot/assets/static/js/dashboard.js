var blue = "#348fe2",
    blueLight = "#5da5e8",
    blueDark = "#1993E4",
    aqua = "#49b6d6",
    aquaLight = "#6dc5de",
    aquaDark = "#3a92ab",
    green = "#00ACAC",
    greenLight = "#33bdbd",
    greenDark = "#038E46",
    orange = "#f59c1a",
    orangeLight = "#f7b048",
    orangeDark = "#c47d15",
    dark = "#2d353c",
    grey = "#b6c2c9",
    purple = "#727cb6",
    purpleLight = "#8e96c5",
    purpleDark = "#5b6392",
    red = "#ff5b57";

var getMonthName = function (a) {
    var b = [];
    return b[0] = "January", b[1] = "February", b[2] = "March", b[3] = "April", b[4] = "May", b[5] = "Jun", b[6] = "July", b[7] = "August", b[8] = "September", b[9] = "October", b[10] = "November", b[11] = "December", b[a];
}, getDate = function (a) {
    var b = new Date(a),
        c = b.getDate(),
        d = b.getMonth() + 1,
        e = b.getFullYear();
    return c < 10 && (c = "0" + c), d < 10 && (d = "0" + d), b = e + "-" + d + "-" + c;
}, handleStatisticsLineChart = function (data) {

    var a = "#B6C2C9", //Default
        b = "#FF5B57", //Danger
        c = "#F59C1A", //Warning
        d = "#00ACAC", //Success
        e = "#348FE2"; //Primary
    f = "#2D353C"; //Inverse
    g = "rgba(255,255,255,0.4)";

    var line_chart_data = [];

    var json_items = JSON.parse(data.line_chart_data);

    console.log(json_items);

    for (var i = 0; i < json_items.length; i++) {

        var json_item = json_items[i];

        var obj =
        {
            u: json_item["month_period"],
            v: json_item["hospital"],
            w: json_item["departments"],
            x: json_item["doctors"],
            y: json_item["patient"]
        };

        line_chart_data.push(obj);
    }

    Morris.Line({
        element: "statistics-line-chart",
        data: line_chart_data,
        xkey: "u",
        ykeys: ["v", "w", "x", "y"],
        labels: ["hospital", "departments", "doctors", "patient"],
        lineColors: [a, b, c, d,e],
        pointFillColors: [a, b, c, d,e],
        lineWidth: "2px",
        pointStrokeColors: [f, f, f, f,f],
        resize: !0,
        gridTextFamily: "Open Sans",
        gridTextColor: a,
        gridTextWeight: "normal",
        gridTextSize: "11px",
        gridLineColor: "rgba(0,0,0,0.5)",
        hideHover: "auto"
    });
    },
    handleStatisticsDonutChart = function (data) {

    var a = "#B6C2C9", //Default
        b = "#FF5B57", //Danger
        c = "#F59C1A", //Warning
        d = "#00ACAC", //Success
        e = "#348FE2"; //Primary

    var pie_chart_data = [];

    var json_items = JSON.parse(data.doughnut_data);

    var total_transfers = 0;

    for (var i = 0; i < json_items.length; i++) {

        var json_item = json_items[i];

        total_transfers = total_transfers + Number(json_item["total"]);

        var li = '<li>' +
            '	<i class="fa fa-circle-o fa-fw text-' + json_item["color"] + ' m-r-5"></i> ' + json_item["percentile"] + ' % <span>' + json_item["category"] + ' Count</span>' +
            '</li>';

        $("#percentiles ul").append(li);

        var obj =
        {
            label: json_item["category"],
            value: json_item["total"]
        };

        pie_chart_data.push(obj);
    }

    document.getElementById('total_transfers').innerHTML = total_transfers.toLocaleString();

    Morris.Donut({
        element: "statistics-donut-chart",
        data: pie_chart_data,
        colors: [a, b, c, d,e],
        labelFamily: "Open Sans",
        labelColor: "rgba(255,255,255,0.4)",
        labelTextSize: "12px",
        backgroundColor: "#242a30"
    });
}, handleAdminMessage = function (data) {
    var json_item = data.message_data;

    if (json_item["message"] != '') {
        setTimeout(function () {
            $.gritter.add({
                title: 'Welcome back, ' + json_item["user"] + '!',
                text: json_item["message"],
                image: '../assets/static/img/profile-pics/' + json_item["avatar"],
                sticky: true,
                time: '',
                class_name: 'my-sticky-class'
            });
        }, 1000);
    }
}, handleDashboadData = function () {
    $.get('GetDashboardData', function (data) {

        var jsonapplications = JSON.parse(data.widget_data);

        console.log(jsonapplications);

        document.getElementById('statistic_one').innerHTML = jsonapplications[0]["statistic_one"];
        document.getElementById('statistic_two').innerHTML = jsonapplications[0]["statistic_two"];
        document.getElementById('statistic_three').innerHTML = jsonapplications[0]["statistic_three"];
        document.getElementById('statistic_four').innerHTML = jsonapplications[0]["statistic_four"];

        handleStatisticsDonutChart(data);

        handleStatisticsLineChart(data);

        handleAdminMessage(data);
    });
 },
    DashboardV2 = function () {
        "use strict";
        return {
            init: function () {
                handleDashboadData();
            }
        };
 
    }();