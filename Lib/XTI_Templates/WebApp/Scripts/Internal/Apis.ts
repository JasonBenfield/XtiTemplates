import { AppApiFactory } from "@jasonbenfield/sharedwebapp/Api/AppApiFactory";
import { ModalErrorView } from "@jasonbenfield/sharedwebapp/Views/ModalError";
import { __APPNAME__AppApi } from "../Lib/Api/__APPNAME__AppApi";

export class Apis {
    private readonly apiFactory: AppApiFactory;

    constructor(modalError: ModalErrorView) {
        this.apiFactory = new AppApiFactory(modalError)
    }

    __APPNAME__() {
        return this.apiFactory.api(__APPNAME__AppApi);
    }
}