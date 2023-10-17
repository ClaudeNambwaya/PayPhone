
$(document).ready(function () {
    App.init();

});



const app = {
    elements: {
        get businessOnly() { return document.querySelector("#business-only"); },
        get radios() { return Array.apply(null, document.querySelectorAll("input[name=scale]")); },
        get clear() { return document.querySelector("#clear"); },
        get scaleChecked() { return document.querySelector("input[name=scale]:checked"); }
    },
    async loadEvents(day) {
        let from = scheduler.visibleStart();
        let to = scheduler.visibleEnd();
        if (day) {
            from = new DayPilot.Date(day).firstDayOfMonth();
            to = from.addMonths(1);
        }

        const { data } = await DayPilot.Http.get(`/api/appointments?start=${from}&end=${to}`);
        console.log(data);
        const options = {
            events: data
        };

        console.log(options);

        if (day) {
            options.timeline = app.getTimeline(day);
            options.scrollTo = day;
        }

        scheduler.update(options);

        nav.events.list = data;
        nav.update();
    },
    async loadResources() {
        const { data } = await DayPilot.Http.get("/api/doctors");
        scheduler.update({
            resources: data,
            visible: true
        });
    },
    getTimeline(date) {
        date = date || DayPilot.Date.today();
        const start = new DayPilot.Date(date).firstDayOfMonth();
        const days = start.daysInMonth();
        const scale = app.elements.scaleChecked.value;
        const businessOnly = app.elements.businessOnly.checked;

        let morningShiftStarts = 9;
        let morningShiftEnds = 13;
        let afternoonShiftStarts = 14;
        let afternoonShiftEnds = 18;

        if (!businessOnly) {
            morningShiftStarts = 0;
            morningShiftEnds = 12;
            afternoonShiftStarts = 12;
            afternoonShiftEnds = 24;
        }

        const timeline = [];

        let increaseMorning;  // in hours
        let increaseAfternoon;  // in hours
        switch (scale) {
            case "15min":
                increaseMorning = 0.25;
                increaseAfternoon = 0.25;
                break;
            case "hours":
                increaseMorning = 1;
                increaseAfternoon = 1;
                break;
            case "shifts":
                increaseMorning = morningShiftEnds - morningShiftStarts;
                increaseAfternoon = afternoonShiftEnds - afternoonShiftStarts;
                break;
            default:
                throw "Invalid scale value";
        }

        for (let i = 0; i < days; i++) {
            const day = start.addDays(i);

            for (let x = morningShiftStarts; x < morningShiftEnds; x += increaseMorning) {
                timeline.push({ start: day.addHours(x), end: day.addHours(x + increaseMorning) });
            }
            for (let x = afternoonShiftStarts; x < afternoonShiftEnds; x += increaseAfternoon) {
                timeline.push({ start: day.addHours(x), end: day.addHours(x + increaseAfternoon) });
            }
        }

        return timeline;
    },
    getTimeHeaders() {
        const scale = app.elements.scaleChecked.value;
        switch (scale) {
            case "15min":
                return [
                    { groupBy: "Month" },
                    { groupBy: "Day", format: "dddd d" },
                    { groupBy: "Hour", format: "h tt" },
                    { groupBy: "Cell", format: "m" }
                ];
            case "hours":
                return [
                    { groupBy: "Month" },
                    { groupBy: "Day", format: "dddd d" },
                    { groupBy: "Hour", format: "h tt" }
                ];
            case "shifts":
                return [
                    { groupBy: "Month" },
                    { groupBy: "Day", format: "dddd d" },
                    { groupBy: "Cell", format: "tt" }
                ];
        }
    },
    init() {
        app.loadResources();
        app.loadEvents(DayPilot.Date.today());

        app.elements.businessOnly.addEventListener("click", () => {
            scheduler.timeline = app.getTimeline();
            scheduler.update();
        });

        app.elements.radios.forEach(item => {
            item.addEventListener("change", () => {
                scheduler.timeline = app.getTimeline();
                scheduler.timeHeaders = app.getTimeHeaders();
                scheduler.update();
            });
        });

        app.elements.clear.addEventListener("click", async () => {
            const dp = scheduler;
            const params = {
                start: dp.visibleStart(),
                end: dp.visibleEnd()
            };

            const { data } = await DayPilot.Http.post("/api/appointments/clear", params);
            app.loadEvents();
        });
    }

};


const nav = new DayPilot.Navigator("nav", {
    selectMode: "month",
    showMonths: 3,
    skipMonths: 3,
    onTimeRangeSelected: args => {
        if (scheduler.visibleStart().getDatePart() <= args.day && args.day < scheduler.visibleEnd()) {
            scheduler.scrollTo(args.day, "fast");  // just scroll
        } else {
            app.loadEvents(args.day);  // reload and scroll
        }
    }
});
nav.init();


const scheduler = new DayPilot.Scheduler("scheduler", {
    visible: false, // will be displayed after loading the resources
    scale: "Manual",
    timeline: app.getTimeline(),
    timeHeaders: app.getTimeHeaders(),
    useEventBoxes: "Never",
    eventDeleteHandling: "Update",
    eventClickHandling: "Disabled",
    eventMoveHandling: "Disabled",
    eventResizeHandling: "Disabled",
    allowEventOverlap: false,
    onBeforeTimeHeaderRender: (args) => {
        args.header.text = args.header.text.replace(" AM", "a").replace(" PM", "p");  // shorten the hour header
    },
    onBeforeEventRender: (args) => {
        switch (args.data.status) {
            case "free":
                args.data.backColor = "#3d85c6";  // blue
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
                args.data.deleteDisabled = app.elements.scaleChecked.value === "shifts"; // only allow deleting in the more detailed hour scale mode
                break;
            case "waiting":
                args.data.backColor = "#e69138";  // orange
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
                break;
            case "confirmed":
                args.data.backColor = "#6aa84f";  // green
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
                break;
        }
    },
    onEventDeleted: async (args) => {
        const params = {
            id: args.e.id(),
        };
        const { data: result } = await DayPilot.Http.delete(`/api/appointments/${params.id}`);
    },
    onTimeRangeSelected: async (args) => {
        const dp = scheduler;
        const scale = app.elements.scaleChecked.value;

        const params = {
            start: args.start.toString(),
            end: args.end.toString(),
            resource: args.resource,
            scale: scale
        };

        dp.clearSelection();

        const { data } = await DayPilot.Http.post("/api/appointments/create", params);
        app.loadEvents();
        //dp.message(data.message);
    }
});
scheduler.init();

app.init();
