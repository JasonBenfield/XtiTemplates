import { __APPNAME__Page } from '../__APPNAME__Page';
import { MainPageView } from './MainPageView';
import { TextComponent } from '@jasonbenfield/sharedwebapp/Components/TextComponent';

class MainPage extends __APPNAME__Page {
    protected readonly view: MainPageView;

    constructor(view: MainPageView) {
        super(view);
        new TextComponent(this.view.heading).setText('Home Page');
    }
}
new MainPage(new MainPageView());