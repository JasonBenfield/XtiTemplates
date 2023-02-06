import { BasicPage } from "@jasonbenfield/sharedwebapp/Components/BasicPage";
import { BasicPageView } from "@jasonbenfield/sharedwebapp/Views/BasicPageView";
import { __APPNAME__AppApi } from "../Lib/Api/__APPNAME__AppApi";
import { Apis } from "./Apis";

export class ScheduledJobsPage extends BasicPage {
    protected readonly defaultApi: __APPNAME__AppApi;

    constructor(view: BasicPageView) {
        super(new Apis(view.modalError).__APPNAME__(), view);
    }
}