import { AppClientFactory } from "@jasonbenfield/sharedwebapp/Http/AppClientFactory";
import { ModalErrorView } from "@jasonbenfield/sharedwebapp/Views/ModalError";
import { __APPNAME__AppClient } from "../Lib/Http/__APPNAME__AppClient";

export class AppClients {
    private readonly clientFactory: AppClientFactory;

    constructor(modalError: ModalErrorView) {
        this.clientFactory = new AppClientFactory(modalError)
    }

    __APPNAME__() {
        return this.clientFactory.create(__APPNAME__AppClient);
    }
}