<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationvue">https://aka.ms/jspsintegrationvue</a> for more details.
        </div>
        <input id="inputDate" v-model="requestDate" type="date" class="fc fc-getData-button fc-button fc-button-primary">
        <FullCalendar ref="fullCalendar" :options="calendarOptions" />
    </div>
</template>

<script lang="js">
    import Vue from 'vue';
    import '@fullcalendar/core/vdom'; // solves problem with Vite
    import FullCalendar from '@fullcalendar/vue';
    import resourceTimeGridPlugin from '@fullcalendar/resource-timegrid';


    export default Vue.extend({
        components: {
            FullCalendar // make the <FullCalendar> tag available
        },
        data() {
            var self = this;
            return {
                current: (new Date()).setHours(0, 0, 0, 0),
                requestDate: (new Date()).toISOString().substr(0, 10),
                loading: false,
                post: null,
                calendarOptions: {
                    locale: 'de', // the initial locale. of not specified, uses the first one
                    plugins: [resourceTimeGridPlugin],
                    initialView: 'resourceTimeGridDay',
                    datesAboveResources: true,
                    slotMinTime: "07:00",
                    slotMaxTime: "23:00",
                    customButtons: {
                        getData: {
                            type: "Date",
                            text: 'Daten abrufen',
                            click: function () {
                                self.fetchData();
                            }
                        }
                    },
                    headerToolbar: {
                        left: 'prev,next,getData',
                        center: 'title',
                        right: 'resourceTimeGridDay,resourceTimeGridAllDays'
                    },
                    views: {
                        resourceTimeGridAllDays: {
                            type: 'resourceTimeGrid',
                            duration: { days: 5 },
                            buttonText: '5 Tage',
                        }
                    },                    
                    events: [],
                    resources: [
                        { id: 'ostbloc', title: 'Ostbloc' },
                        { id: 'kegel-boulder', title: 'Der Kegel (bouldern)' },
                        { id: 'boulderKlub', title: 'Boulderklub' },
                        { id: 'boulderGarten', title: 'Bouldergarten' }
                    ]
                }
            };
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            fetchData() {
                this.loading = true;
                //ToDo: Adjust 
                //this.calendarOptions.views.resourceTimeGridAllDays.duration.days = 3; //this.requestDate - this.current;
                let calendarApi = this.$refs.fullCalendar.getApi();
                calendarApi.gotoDate(this.requestDate);
                fetch('boulderingtimes/?requestDate=' + this.requestDate)
                    .then(r => r.json())
                    .then(boulderingTimesRespones => {
                        boulderingTimesRespones.forEach(j => {
                            j.timeSlots.forEach(t => {
                                let freePlaces = 0;
                                if (t.currentCourseFreePlacesCount !== undefined && t.currentCourseFreePlacesCount !== null) {
                                    freePlaces = t.currentCourseFreePlacesCount;
                                }
                                else if (t.currentCourseParticipantCount !== undefined && t.currentCourseParticipantCount !== null) {
                                    freePlaces = t.maxCourseParticipantCount - t.currentCourseParticipantCount;
                                }

                                let backgroundcolor = "rgb(215, 255, 156)";
                                if (t.state !== "BOOKABLE") {
                                    backgroundcolor = "lightcoral";
                                }

                                let textcolor = "black";
                                if (freePlaces > 8) {
                                    textcolor = "green"
                                }
                                else if (freePlaces > 4) {
                                    textcolor = "orange";
                                } else {
                                    textcolor = "red";
                                }


                                if (freePlaces > 0) {
                                    this.calendarOptions.events.push({
                                        "resourceId": j.boulderingPlace,
                                        "title": freePlaces,
                                        "start": t.dateList[0].start,
                                        "end": t.dateList[0].end,
                                        "backgroundColor": backgroundcolor,
                                        "textColor": textcolor
                                    })
                                }
                            });
                        });
                        this.loading = false;
                        return;
                    });
            }
        },
    });
</script>

<style>
    .fc {
        font-size: 12px
    }
</style>