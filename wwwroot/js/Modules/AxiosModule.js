var pageURL = window.location.origin;
if (pageURL.includes("localhost") == true) {
    pageURL = window.location.pathname;
    let split = pageURL.split("/");
    pageURL = "/" + split[1] + "/";
}

export const GetLocation = {
    pathname: window.location.pathname,
    split: "",
    GetPage: function () {
        this.split = this.pathname.split("/");
        return this.split[3];
    },
    GetArea: function () {
        this.split = this.pathname.split("/");
        return this.split[2];
    }
}

export const HttpRequest = axios.create({
    baseURL: pageURL,
    timeout: 5000,
    headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() }
});

export function UserInfo(action) {
    let result = false;

    return HttpRequest.get("Main/Welcome?handler=UserInfo", {
        params: {
            Info: action
        }
    }).then(function (response) {
        return response.data;
    }).catch(function (error) {
        console.log(error);
    });
}

export function ConfirmInfo(url, info, confirm) {

    return HttpRequest.post(url, {
        Info: info,
        confirm: confirm
    }).then(function (response) {
        return response.data;
    }).catch(function (error) {
        console.log(error);
    });
}

export function GetModalTxt(Action, IdLanguage) {
    return HttpRequest.get("api/Language/GetModalTxt", {
        params: {
            Action: Action,
            IdLanguage: IdLanguage
        }
    }).then(function (response) {
        return response.data;;
    }).catch(function (error) {
        console.log(error);
    });

}

