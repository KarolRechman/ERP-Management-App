import { ConfirmInfo } from '../Modules/AxiosModule.js';


export const ModalClass = {
    Error: "modal-notify modal-danger",
    Info: "modal-notify modal-info"
}

export const ButtonClass = {
    Error: "btn btn-outline-danger waves-effect",
    Info: "btn btn-outline-info waves-effect",
    success: "btn btn-success waves-effect"
}

export const IconClass = {
    Error: "fas fa-exclamation-triangle rollIn ml-4 mr-4",
    Info: "fas fa-info rollIn ml-4 mr-4"
}

export class Modal {
    constructor(Modal, ModalClass, icon, Title, BodyTitle, BodyContent, buttons = []) {
        this.show = function () {
            $(Modal).modal('show');
        }
        this.init = function () {
            $("#ModalDialog").addClass(ModalClass);
            $("#ModalIcon").addClass(icon);
            $("#Title").text(Title);
            $("#BodyTitle").html(BodyTitle);
            $("#BodyContent").html(BodyContent);

            let ModalButtons = "";
            for (var i = 0; i < buttons.length; i++) {
                if (buttons[i].styleClass.includes("success")) {
                    ModalButtons += "<button class='" + buttons[i].styleClass + "' id='" + buttons[i].id + "'><span class='Translation'>" + buttons[i].txt + "</span></button>";
                } else {
                    ModalButtons += "<button class='" + buttons[i].styleClass + "' data-dismiss='modal' id='" + buttons[i].id + "'><span class='Translation'>" + buttons[i].txt + "</span></button>";
                }
            }

            $("#ModalButtons").html(ModalButtons);
            $(Modal).modal({
                backdrop: 'static',
                keyboard: false
            });
            this.show();
            this.clear();
        }
        this.clear = function () {
            $(Modal).on('hidden.bs.modal', function () {
                let empty = "";
                $("#ModalDialog").removeClass(ModalClass);
                $("#ModalIcon").removeClass(icon);
                $("#Title").text(empty);
                $("#BodyTitle").html(empty);
                $("#BodyContent").html(empty);
                $("#ModalButtons").html(empty);
            });
            //return true
        }
        this.destroy = function () {
            $(Modal).modal('dispose');
        }
    }
}

export class Button {
    constructor(id, StyleClass, txt) {
        this.id = id;
        this.styleClass = StyleClass;
        this.txt = txt;

        this.listen = function (url, action, confirm) {
            let id = '#' + this.id;
            $(id).click(function () {
                ConfirmInfo(url, action, confirm).then(data => {
                    if (data == true) {
                        $("#Modal").modal('hide');
                    }
                });
            });
        }
    }
}
