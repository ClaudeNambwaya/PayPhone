$(document).ready(function () {
    App.init();

});


const app = {
    doctors: [],
    elements: {
        doctor: document.querySelector("#doctor")
    },
    async loadEvents(day) {
        const start = nav.visibleStart();
        const end = nav.visibleEnd();
        const doctor = app.elements.doctor.value;

        const { data } = await DayPilot.Http.get(`/api/appointments?start=${start}&end=${end}&doctor=${doctor}`);

        //console.log(data);

        if (day) {
            calendar.startDate = day;
        }
        calendar.events.list = data;
        calendar.update();

        console.log(calendar.events.list);

        nav.events.list = data;

        console.log(nav.events.list);
        nav.update();
    },
    async init() {
        const { data } = await DayPilot.Http.get("/api/doctors");

        //console.log(data);

        app.doctors = data;

        app.doctors.forEach(item => {
            const option = document.createElement("option");
            option.value = item.id;
            option.innerText = item.name;
            app.elements.doctor.appendChild(option);
        });

        app.elements.doctor.addEventListener("change", () => {
            app.loadEvents();
        });

        app.loadEvents();
    }
};

const nav = new DayPilot.Navigator("nav", {
    selectMode: "week",
    showMonths: 3,
    skipMonths: 3,
    onTimeRangeSelected: (args) => {
        app.loadEvents(args.start.firstDayOfWeek(), args.start.addDays(7));
    }
});
nav.init();

const calendar = new DayPilot.Calendar("calendar", {
    viewType: "Week",
    timeRangeSelectedHandling: "Disabled",
    eventDeleteHandling: "Update",
    onEventMoved: async (args) => {
        const appointment = {
            ...args.e.data,
            start: args.newStart,
            end: args.newEnd
        };
        const { data } = await DayPilot.Http.put(`/api/appointments/${appointment.id}`, appointment);
    },
    onEventResized: async (args) => {
        const appointment = {
            ...args.e.data,
            start: args.newStart,
            end: args.newEnd
        };
        const { data } = await DayPilot.Http.put(`/api/appointments/${appointment.id}`, appointment);
    },
    onEventDeleted: async (args) => {
        const id = args.e.data.id;
        await DayPilot.Http.delete(`/api/appointments/${id}`);
    },
    onBeforeEventRender: (args) => {
        switch (args.data.status) {
            case "free":
                args.data.backColor = "#3d85c6";  // blue
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
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
    onEventClick: async (args) => {

        const form = [
            { name: "Edit Appointment" },
            { name: "Name", id: "text" },
            {
                name: "Status", id: "status", type: "select", options: [
                    { name: "Free", id: "free" },
                    { name: "Waiting", id: "waiting" },
                    { name: "Confirmed", id: "confirmed" },
                ]
            },
            { name: "From", id: "start", dateFormat: "MMMM d, yyyy h:mm tt", disabled: true },
            { name: "To", id: "end", dateFormat: "MMMM d, yyyy h:mm tt", disabled: true },
            { name: "Doctor", id: "resource", disabled: true, options: app.doctors },
        ];

        const data = args.e.data;

        const options = {
            focus: "text"
        };

        const modal = await DayPilot.Modal.form(form, data, options);
        if (modal.canceled) {
            return;
        }

        const params = {
            ...args.e.data,
            name: modal.result.text,
            status: modal.result.status
        };

        console.log("params", params);

        await DayPilot.Http.put(`/api/appointments/${params.id}`, params);
        calendar.events.update(modal.result);
    }
});
calendar.init();

app.init();

