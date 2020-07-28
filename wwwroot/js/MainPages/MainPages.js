import { HttpRequest, UserInfo, GetModalTxt, GetLocation } from '../Modules/AxiosModule.js';
import { GetWordTranslation, GetTxtById } from '../Translations.js';
import { Button, Modal, ModalClass, ButtonClass, IconClass } from '../Modules/ModalModule.js';

const Page = GetLocation.GetPage();
const Area = GetLocation.GetArea();

switch (Page) {
    case "Welcome":
        //4 Welcome Page:
        const DontShow = $("#Input_DontShow");
        const SendCoupon = $("#SendCoupons");
        const CheckCoupons = $(".coupons");

        function validateEmail(email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }

        let Information;
        let BodyTitle;
        let BodyInfo;
        let ConfirmButtonTxt;
        let NoButtonTxt;

        function SetInfoTxt(Methods = []) {
            return Promise.all(Methods).then(data => {
                return data;
            });
        }

        $(document).ready(function () {
            //SetInfoTxt(LanguageName, BodyTitle);
            //const Info = new Modal("#Modal", "modal-notify modal-info", "fas fa-info rollIn ml-4 mr-4", "Information", BodyTitle, BodyInfo, Buttons);
            UserInfo("LanguageInfo").then(data => {
                if (data == false) {
                    const InfoModal = new Promise((resolve, reject) => {
                        let IdLanguage = sessionStorage.getItem("language");
                        var Methods = [GetTxtById(27, IdLanguage), GetModalTxt('LanguageInfo', IdLanguage), GetTxtById(32, IdLanguage), GetTxtById(33, IdLanguage)];
                        resolve(SetInfoTxt(Methods).then(data => {
                            Information = data[0];
                            BodyTitle = data[1].bodyTitle;
                            BodyInfo = data[1].bodyInfo;
                            ConfirmButtonTxt = data[2];
                            NoButtonTxt = data[3];
                        }));
                    });
                    InfoModal.then(() => {
                        const ConfirmButton = new Button("ConfirmButton", ButtonClass.success, ConfirmButtonTxt);
                        const LeaveButton = new Button("NoButton", ButtonClass.Error, NoButtonTxt);
                        var Buttons = [ConfirmButton, LeaveButton];
                        const Info = new Modal("#Modal", ModalClass.Info, IconClass.Info, Information, BodyTitle, BodyInfo, Buttons);
                        Info.init();
                        //Info.show();
                        //Info.clear();
                        ConfirmButton.listen(`${Area}/${Page}?handler=UserConfirm`, "LanguageInfo", true);
                    });
                }
            });

            DontShow.click(function () {
                let check = this.checked;
                if (check == true) {
                    let url = `${Area}/${Page}?handler=DontShow`;
                    let params = {
                        check: check
                    }
                    HttpRequest.get(url, { params }).then(function (response) {
                        if (response.data == true) {
                            $(".CouponsContent").hide('slow');
                        } else {
                            DontShow.click();
                        }
                    }).catch(function (error) {
                        console.log(error);
                    });
                }
            });

            SendCoupon.click(function () {
                let email = $("#Input_Email");
                let IdLanguage = sessionStorage.getItem("language");
                const OKButton = new Button("ConfirmButton", ButtonClass.Info, "OK");
                var Buttons = [OKButton];

                if (email.val() == null || email.val() == undefined) {
                    email.val("");
                }
                if (email.val() != "") {

                    if (validateEmail(email.val())) {
                        var Coupons = [];

                        CheckCoupons.each(function () {
                            if (this.checked == true) {
                                Coupons.push({
                                    IdCoupon: this.id.replace("Coupon_", "")
                                });
                            }
                        });
                        if (Coupons.length == 0) {
                            const InfoModal = new Promise((resolve, reject) => {
                                var Methods = [GetTxtById(27, IdLanguage), GetTxtById(37, IdLanguage)];
                                resolve(SetInfoTxt(Methods).then(data => {
                                    Information = data[0];
                                    BodyTitle = data[1];
                                }));
                            });
                            InfoModal.then(() => {
                                const Info = new Modal("#Modal", ModalClass.Info, IconClass.Info, Information, BodyTitle, "", Buttons);
                                Info.init();
                                //Info.show();
                                //Info.clear();
                            });
                        } else {
                            HttpRequest.post(`${Area}/${Page}?handler=SendCoupon`, {
                                email: email.val(),
                                Coupons: Coupons
                            }).then(function (response) {
                                let message = response.data.message;
                                if (response.data.status == false) {
                                    const ErrorModal = new Promise((resolve, reject) => {
                                        var Methods = [GetTxtById(25, IdLanguage)];
                                        resolve(SetInfoTxt(Methods).then(data => {
                                            Information = data[0] + " !";
                                            BodyTitle = message;
                                        }));
                                    });
                                    ErrorModal.then(() => {
                                        const Error = new Modal("#Modal", ModalClass.Error, IconClass.Error, Information, BodyTitle, "", Buttons);
                                        Error.init();
                                        //Error.show();
                                        //Error.clear();
                                    });
                                } else {
                                    const InfoModal = new Promise((resolve, reject) => {
                                        var Methods = [GetTxtById(27, IdLanguage)];
                                        resolve(SetInfoTxt(Methods).then(data => {
                                            Information = data[0];
                                            BodyTitle = message;
                                        }));
                                    });
                                    InfoModal.then(() => {
                                        const Info = new Modal("#Modal", ModalClass.Info, IconClass.Info, Information, BodyTitle, "", Buttons);
                                        Info.init();
                                        //Info.show();
                                        //Info.clear();
                                    });
                                    CheckCoupons.each(function () {
                                        if (this.checked == true) {
                                            this.checked = false;
                                        }
                                    });
                                    email.val('');
                                }
                            }).catch(function (error) {
                                console.log(error);
                            });
                        }
                    } else {
                        const ErrorModal = new Promise((resolve, reject) => {
                            var Methods = [GetTxtById(25, IdLanguage), GetTxtById(15, IdLanguage), GetWordTranslation("is not valid")];
                            resolve(SetInfoTxt(Methods).then(data => {
                                Information = data[0] + " !";
                                BodyTitle = data[1] + " : <span class='text-danger'>" + email.val() + "</span> " + data[2] + " !";
                            }));
                        });
                        ErrorModal.then(() => {
                            const Error = new Modal("#Modal", ModalClass.Error, IconClass.Error, Information, BodyTitle, "", Buttons);
                            Error.init();
                            //Error.show();
                            //Error.clear();
                        });
                    }
                }
            });
        });
        break;
    case "":

        break;

    default:
        "No code";
}

//////////////////////////////////////////////////////////////
//function GetWordTranslation(word) {
//    let LanguageId = sessionStorage.getItem("language");
//    if (LanguageId == null || LanguageId == 1) {
//        return word;
//    } else {
//        let url = "api/Language/" + LanguageId
//        let TranslatedWord;
//        let params = {
//            word: word
//        }
//        return HttpRequest.get(url, { params }).then(function (response) {
//            TranslatedWord = response.data;
//            return TranslatedWord;

//        });
//    }
//}







//GetWordTranslation("is not valid").then(data => {
//    $("#test").text(data);
//});
