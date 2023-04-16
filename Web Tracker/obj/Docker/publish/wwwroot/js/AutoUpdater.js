function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "0";
}



var connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl("/chatHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .build();


connection.start();
connection.on("GetWebsiteId", () => {
    var siteId = getCookie("loggedInUserWebsiteId");
    //alert(typeof siteId);
    console.log('-------------------- called ---------------------')
    connection.invoke("SaveWebsiteId", siteId);
    console.log('-------------------- called after---------------------')
});

connection.on("TotalVisitors", (totalvisitors) => {
    document.getElementById("totalvisitors").innerHTML = totalvisitors;
});
connection.on("NewVisitors", (newvisitors) => {
    document.getElementById("newvisitors").innerHTML = newvisitors;
});
connection.on("ReturningVisitors", (returningvisitors) => {
    document.getElementById("returningvisitors").innerHTML = returningvisitors;
});
connection.on("ActionCount", (actioncount) => {
    document.getElementById("actionscount").innerHTML = actioncount;
});
connection.on("SummaryTable", (summarylist) => {
    document.getElementById("SumaryTable").innerHTML = "";
    for (var i = 0; i < summarylist.length; i++) {
        $("#SumaryTable").append(
            '<tr><td>' + (i + 1) + '</td><td>' + summarylist[i]["flowSummed"] + '</td><td>' + summarylist[i]["count"] +'</td></tr>'
        );
    }
});
connection.on("ActionSummaryTable", (summarylist) => {
    document.getElementById("ActionSumaryTable").innerHTML = "";
    for (var i = 0; i < summarylist.length; i++) {
        $("#ActionSumaryTable").append(
            '<tr><td>' + (i + 1) + '</td><td>' + summarylist[i]["actionSummed"] + '</td><td>' + summarylist[i]["count"] + '</td></tr>'
        );
    }
});

