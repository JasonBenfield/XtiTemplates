import { ButtonCommandItem } from "@jasonbenfield/sharedwebapp/Command/ButtonCommandItem";
import { ContextualClass } from "@jasonbenfield/sharedwebapp/ContextualClass";
import { Toolbar } from "@jasonbenfield/sharedwebapp/Html/Toolbar";
import { PaddingCss } from "@jasonbenfield/sharedwebapp/PaddingCss";
import { TextCss } from "@jasonbenfield/sharedwebapp/TextCss";

export class __APPNAME__Theme {
    public static readonly instance = new __APPNAME__Theme();

    readonly listItem = {
        deleteButton() {
            return new ButtonCommandItem()
                .configure(b => {
                    b.icon.solidStyle('times');
                    b.icon.addCssFrom(new TextCss().context(ContextualClass.danger).cssClass());
                    b.useOutlineStyle();
                    b.setText('');
                    b.setTitle('Delete');
                });
        }
    }

    readonly cardHeader = {
        editButton() {
            return new ButtonCommandItem()
                .configure(b => {
                    b.icon.solidStyle('edit');
                    b.setContext(ContextualClass.primary);
                    b.useOutlineStyle();
                    b.setText('Edit');
                    b.setTitle('Edit');
                });
        },
        addButton() {
            return new ButtonCommandItem()
                .configure(b => {
                    b.icon.solidStyle('plus');
                    b.setContext(ContextualClass.primary);
                    b.useOutlineStyle();
                    b.setText('Add');
                    b.setTitle('Add');
                });
        }
    }

    readonly commandToolbar = {
        toolbar() {
            return new Toolbar()
                .configure(t => {
                    t.setBackgroundContext(ContextualClass.secondary);
                    t.setPadding(PaddingCss.xs(3));
                    t.addCssName('bg-opacity-25');
                });
        },
        backButton() {
            return new ButtonCommandItem()
                .configure(b => {
                    b.icon.solidStyle('caret-left');
                    b.setText('Back');
                    b.setTitle('Back');
                    b.useOutlineStyle(ContextualClass.light);
                });
        },
        cancelButton() {
            return new ButtonCommandItem()
                .configure(b => {
                    b.icon.solidStyle('times');
                    b.setText('Cancel');
                    b.setTitle('Cancel');
                    b.setContext(ContextualClass.danger);
                });
        },
        saveButton() {
            return new ButtonCommandItem()
                .configure(b => {
                    b.icon.solidStyle('check');
                    b.setText('Save');
                    b.setTitle('Save');
                    b.setContext(ContextualClass.primary);
                });
        }
    };
}