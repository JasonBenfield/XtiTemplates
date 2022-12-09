import { AppApiFactory } from "@jasonbenfield/sharedwebapp/Api/AppApiFactory";
import { ModalErrorComponent } from "@jasonbenfield/sharedwebapp/Error/ModalErrorComponent";
import { ModalErrorComponentView } from "@jasonbenfield/sharedwebapp/Error/ModalErrorComponentView";
import { __APPNAME__AppApi } from "../Lib/Api/__APPNAME__AppApi";

export class Apis {
    private readonly modalError: ModalErrorComponent;

    constructor(modalError: ModalErrorComponentView) {
        this.modalError = new ModalErrorComponent(modalError);
    }

    __APPNAME__() {
        let apiFactory = new AppApiFactory(this.modalError)
        return apiFactory.api(__APPNAME__AppApi);
    }
}