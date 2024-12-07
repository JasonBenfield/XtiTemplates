import { __APPNAME__PageView } from '../__APPNAME__PageView';
import { BasicTextComponentView } from '@jasonbenfield/sharedwebapp/Views/BasicTextComponentView';
import { TextHeading1View } from '@jasonbenfield/sharedwebapp/Views/TextHeadings';

export class MainPageView extends __APPNAME__PageView {
    readonly heading: BasicTextComponentView;

    constructor() {
        super();
        this.heading = this.addView(TextHeading1View);
    }
}