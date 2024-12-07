import { BasicPage } from "@jasonbenfield/sharedwebapp/Components/BasicPage";
import { __APPNAME__AppClient } from "../Lib/Http/__APPNAME__AppClient";
import { AppClients } from "./AppClients";
import { __APPNAME__PageView } from "./__APPNAME__PageView";

export class __APPNAME__Page extends BasicPage {
    protected readonly __APPNAME__Client: __APPNAME__AppClient;

    constructor(view: __APPNAME__PageView) {
        const appClients = new AppClients(view.modalError);
        const __APPNAME__Client = appClients.__APPNAME__();
        super(__APPNAME__Client, view);
        this.__APPNAME__Client = __APPNAME__Client;
    }
}