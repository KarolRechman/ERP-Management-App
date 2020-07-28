import { HttpRequest } from './Modules/AxiosModule.js';
var url = "api/Language/";
sessionStorage.setItem("language", $("#Language").val());


var pathname = window.location.pathname;
var split = pathname.split("/");
var PageName = split[3];
if (PageName == undefined) {
    PageName = "Login";
}
//var LanguageSettings = sessionStorage.getItem("language");
//if (LanguageSettings == null) {
//    sessionStorage.setItem("language", $("#Language").val());
//}
//const Loader = $(".loader");

export function GetWordTranslation(word) {
    let LanguageId = sessionStorage.getItem("language");
    if (LanguageId == null || LanguageId == 1) {
        return new Promise((resolve, reject) => {
            resolve(word);
        });
    } else {
        let params = {
            word: word
        }
        return HttpRequest.get(url + "GetWord/" + LanguageId, { params }).then(function (response) {
            return response.data;
        });
    }
}

export function GetTxtById(IdTxt, IdLanguage) {
    let params = {
        IdTxt: IdTxt
    }
    return HttpRequest.get(url + "GetTxtById/" + IdLanguage, { params }).then(function (response) {
        return response.data;
    });
}

$(document).ready(function () {
    var Language = $("#Language");
    var Words = [];

    Language.change(function () {
        let LanguageId = Language.val();
        let LanguageSettings = sessionStorage.getItem("language");
        let storageKey = PageName + "Words" + LanguageId.toString();
        let wantedKey = PageName + "Words" + LanguageSettings.toString();
        var TranslationElements = $(".Translation");

        var stored = sessionStorage.getItem(storageKey);
        if (stored) {
            Words = JSON.parse(stored);
            SetWords(TranslationElements, Words, LanguageId);
            SetLanguageSession(LanguageId);
        }
        else {
            if (LanguageId != LanguageSettings) {
                Words = [];
                TranslationElements.each(function (index) {
                    Words.push({
                        id: (index + 1).toString(),
                        text: this.textContent.trim()
                    });
                    index += 1;
                });
                sessionStorage.setItem(wantedKey, JSON.stringify(Words));

                HttpRequest.post(url + this.value, Words).then(function (response) {
                    SetWords(TranslationElements, response.data, LanguageId);
                    sessionStorage.setItem(storageKey, JSON.stringify(response.data));
                }).catch(function (error) {
                    //error modal !!!!!!
                    console.log(error);
                });
            }
        }
    });
});

function SetWords(TranslationElements, Words, LanguageId) {
    TranslationElements.hide();
    let indexes = [];
    for (var j = 0; j < Words.length; j++) {

        TranslationElements.each(function (index) {
            let check = indexes.includes(index);
            if (check == false && j == index) {
                this.textContent = Words[j].text;
                indexes.push(index);
                return false;
            }
        });
    }
    TranslationElements.fadeIn('slow');
    sessionStorage.setItem("language", LanguageId);
    Words = [];
}

function SetLanguageSession(LanguageId) {
    HttpRequest.put(url + LanguageId);
}


