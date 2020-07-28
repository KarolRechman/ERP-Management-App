import { HttpRequest, UserInfo, GetModalTxt, GetLocation } from '../Modules/AxiosModule.js';
import { GetWordTranslation, GetTxtById } from '../Translations.js';
import { Button, Modal, ModalClass, ButtonClass, IconClass } from '../Modules/ModalModule.js';
import { InitPicker, SelectList } from '../Modules/SelectModule.js';
import { Table } from '../Modules/TableModule.js';
import { Company } from '../Modules/MenuModule.js';

const Page = GetLocation.GetPage();
const Area = GetLocation.GetArea();
const OKButton = new Button("ConfirmButton", ButtonClass.Info, "OK");
var Buttons = [OKButton];

switch (Page) {
    case "SparepartTransfer":
        sessionStorage.removeItem(Area + Page + "Params");
       

        let table = new Table(null, null, null);

        let Customer = document.getElementById("Customer-SelectList");
        let Location = document.getElementById("Location-SelectList");
        let Condition = document.getElementById("Condition-SelectList");
        Condition.value = 1;
        let Bin = document.getElementById("Input_Bin");
        let PartNumber = document.getElementById("Input_PartNumber");

        Company.addEventListener("change", (event) => {
            let params = {
                Id: Company.value,
            };
            HttpRequest.post(`${Area}/${Page}?handler=CompanyChange`, params).then(response => {
                let options = {
                    name: "Customer-SelectList",
                    items: response.data.Customers,
                    selected: Customer.value
                };
                let NewCustomer = new SelectList(options);
                NewCustomer.SetItems();

                options = {
                    name: "Location-SelectList",
                    items: response.data.Locations,
                    selected: Location.value
                };
                console.log(Location.value);
                let NewLocation = new SelectList(options);
                NewLocation.SetItems();

                options = {
                    name: "Condition-SelectList",
                    items: response.data.Conditions,
                    selected: Condition.value
                };
                console.log(Condition.value);
                let NewCondition = new SelectList(options);
                NewCondition.SetItems();
            });
        });


        let SearchButton = document.getElementById("SparepartTransferSearch");
        SearchButton.addEventListener("click", (event) => {
            console.log(Bin.value);
            if (Bin.value != "" && PartNumber.value != "") {
                let params = {
                    IdMandant: Company.value,
                    IDCustomer: Customer.value,
                    Location: Bin.value,
                    ProdpartNr: PartNumber.value,
                    IdPartCondition: Condition.value,
                    IdBinLocation: Location.value
                }

                if (!CheckSessionParams(params)) {
                    HttpRequest.post(`${Area}/${Page}?handler=PartSelect`, params).then(data => {
                        if ($('#TabelDiv').length > 0) {
                            table.generateTable(document.querySelector("table"), data.data);
                        } else {
                            let TabelDiv = document.createElement('div');
                            TabelDiv.id = "TabelDiv";
                            TabelDiv.className = "mt-5 pt-5"
                            TabelDiv.style.display = "none";
                            let Section = document.getElementById("SparepartTransferSection");
                            Section.appendChild(TabelDiv);

                            let TableBulidData = {
                                Headers: ["IDPRODPARTSLAGER", "Location", "PRODPARTNR", "Description", "Customer", "Serial Nr", "OPAL Order", "Carton", "Distributor Order", "Distributor Delivery Note",
                                    "Price", "Inbound", "Received Date", "Received Serial", "Vendor", "MHD", "PartCondition", "Qty", "IdInboundPart", "IDLAGERPLATZ", "IDPRODPARTNR", "idlagerort", "Lagerort", ""],
                                Name: "SparepartTransfer",
                                Options: {
                                    HasCheckboxes: true,
                                    IsEditable: true,
                                    EditableFields: ["Price"],
                                    InputFields: ["ShowGPN", "ShowPrice", "ShowPrintLabel"],
                                    Hasbuttons: true,

                                },
                                ActionsUrls: {
                                    Delete: `${Area}/${Page}?handler=`,
                                    Update: `${Area}/${Page}?handler=`
                                },
                                items: data.data
                            }
                            table = new Table(null, TableBulidData, TabelDiv);
                            table.init();
                            $(TabelDiv).fadeIn(function () {
                                $('Main').animate({
                                    scrollTop: $(TabelDiv).offset().top
                                }, 1000);
                            });
                        }
                    });
                }
            }
        });

        break;
    case "":

        break;

    default:
        "No code";
}

function CheckSessionParams(params) {
    let paramsString = JSON.stringify(params)
    let storageKey = Area + Page + "Params";
    var stored = sessionStorage.getItem(storageKey);
    if (stored == paramsString) {
        return true;
    } else {
        sessionStorage.setItem(storageKey, paramsString);
    }
}
