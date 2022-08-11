import { BasicPage } from '@jasonbenfield/sharedwebapp/Components/BasicPage';
import { MainPageView } from './MainPageView';
import { TextComponent } from '@jasonbenfield/sharedwebapp/Components/TextComponent';

class MainPage extends BasicPage {
    protected readonly view: MainPageView;

    constructor() {
        super(new MainPageView());
        new TextComponent(this.view.heading).setText('Home Page');
    }
}
new MainPage();