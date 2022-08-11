import { BasicPageView } from '@jasonbenfield/sharedwebapp/Views/BasicPageView';
import { BasicTextComponentView } from '@jasonbenfield/sharedwebapp/Views/BasicTextComponentView';
import { TextHeading1View } from '@jasonbenfield/sharedwebapp/Views/TextHeadings';

export class MainPageView extends BasicPageView {
    readonly heading: BasicTextComponentView;

    constructor() {
        super();
        this.heading = this.addView(TextHeading1View);
    }
}