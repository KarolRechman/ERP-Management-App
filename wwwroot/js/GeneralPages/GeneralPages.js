import { HttpRequest, UserInfo, GetModalTxt, GetLocation } from '../Modules/AxiosModule.js';
import { GetWordTranslation, GetTxtById } from '../Translations.js';
import { Button, Modal, ModalClass, ButtonClass, IconClass } from '../Modules/ModalModule.js';
import { InitPicker, SelectList } from '../Modules/SelectModule.js';
import { Table } from '../Modules/TableModule.js';


const Page = GetLocation.GetPage();
const Area = GetLocation.GetArea();
const CompanySelect = $("#CompanySelect");
const OKButton = new Button("ConfirmButton", ButtonClass.Info, "OK");
var Buttons = [OKButton];

switch (Page) {
    case "Holidays":
        let TableBulidData = {}

        TableBulidData = {
            Headers: ["IdHoliday", "Date", "Customer", ""],
            Name: "Holidays",
            Options: {
                HasCheckboxes: true,
                IsEditable: true,
                Hasbuttons: false
            },
            ActionsUrls: {
                Delete: `${Area}/${Page}?handler=DeleteHolidays`,
                Update: `${Area}/${Page}?handler=UpdateHoliday`
            }
        }
        const table = new Table(document.querySelector("table"), TableBulidData, document.getElementById("TabelDiv"));
        table.init();

        let year = new Date().getFullYear();
        var options = {
            name: "Holidays-DatePicker",
            minYear: year - 1,
            maxYear: year + 2,
            orientation: 'top',
        }
        const myPicker = InitPicker(options);
        let yearSelect = $("#Filterbyyear-SelectList");
        yearSelect[0].name = "Year";
        let Customer = $("#Customer-SelectList");
        Customer[0].name = "Id" + Customer[0].name;
        CompanySelect[0].name = "IdMandant";
        let elementsChange = [yearSelect, Customer, CompanySelect];

        yearSelect.selectpicker('val', year.toString());

        $(document).ready(function () {
            elementsChange.forEach((element) => {
                $(element).change(function () {
                    if (element[0].name == "IdMandant") {
                        Customer[0].value = 0;
                        yearSelect[0].value = year;
                    }
                    let params = SetParams(elementsChange);
                    let RefreshHolidays = new Promise((res, rej) => {
                        res(SetTable(params));
                    });
                    RefreshHolidays.then((data) => {
                        let options = {
                            name: "Customer-SelectList",
                            items: data.Customers,
                            selected: Customer[0].value
                        };
                        const select = new SelectList(options);
                        select.SetItems();
                        let NewYears = [];
                        for (var i = (year - 1); i < (year + 3); i++) {
                            NewYears.push({
                                id: i,
                                text: i
                            });
                        }
                        options = {
                            name: "Filterbyyear-SelectList",
                            items: NewYears,
                            selected: yearSelect[0].value
                        };
                        const YearSelect = new SelectList(options);
                        YearSelect.SetItems();

                        table.generateTable(document.querySelector("table"), data.Table.items);
                    });
                });
            });

            $("#SubmitHoliday").click(function () {
                let date = document.getElementById("Holidays-DatePicker");
                if (date.value == "") {
                    return;
                } else {

                    let params = SetParams(elementsChange);
                    let splitDate = date.value.split(" - ");
                    if (splitDate[0] == splitDate[1]) {
                        params["Date"] = moment().format(splitDate[0]);
                    } else {
                        if (moment(splitDate[1].trim(), 'DD/MM/YYYY', true).isValid()) {
                            params["Date"] = date.value;
                        } if (splitDate[1] == "...") {
                            params["Date"] = moment().format(splitDate[0]);
                        }
                    }
                    HttpRequest.post(`${Area}/${Page}?handler=AddHoliday`, params).then((response) => {
                        if (response.data == "True") {
                            params = SetParams(elementsChange);
                            SetTable(params).then(data => {
                                table.generateTable(document.querySelector("table"), data.Table.items);
                                myPicker.reset();
                            });
                        } else {
                            GetWordTranslation("An error occured").then(data => {
                                const Info = new Modal("#Modal", ModalClass.Error, IconClass.Error, data, `${response.data.trim().slice(0, -1)}.`, "", Buttons);
                                Info.init();
                                //Info.show();
                                //Info.clear();
                                myPicker.reset();
                            });
                        }
                    });
                }
            });
        });

        break;
    case "":

        break;

    default:
        "No code";
}

function SetParams(elements) {
    let params = {};
    elements.forEach((element) => {
        params[element[0].name] = element[0].value;
    });
    return params;
}

function SetTable(params) {
    return HttpRequest.post(`${Area}/${Page}?handler=RefreshHolidays`, params).then((response) => {
        return response.data;
    });
}

//7 - 33, 5 = 4,7857
//9 - 44 = 4,8888
//77,5/16 = 4,8437
//});



/*

         if (pair.Key.Contains("List"))
                {

                }
                else
                {
                    DbType dbt = (DbType)Enum.Parse(typeof(DbType), pair.Value.GetType().Name);
                    parameters.Add("@" + pair.Key, pair.Value, dbt);
                }

 */
