import { GetWordTranslation } from '../Translations.js';
import { HttpRequest } from '../Modules/AxiosModule.js';
import { Button, Modal, ModalClass, ButtonClass, IconClass } from '../Modules/ModalModule.js';

const OKButton = new Button("ConfirmButton", ButtonClass.Info, "OK");
var Buttons = [OKButton];

export class Table {
    constructor(table, data = null, div = null) {
        if (data != null) {
            this.options = data.Options;
            this.name = data.Name;
            this.ActionsUrls = data.ActionsUrls;
        }

        this.Checked = [];
        this.TrToDelete = [];

        if (table == null && div != null && data.Options != null) {
            let NewMainDiv = document.createElement("div");
            NewMainDiv.className = TabelStyles.DivClass;

            Promise.all([GetWordTranslation(data.Name), GetWordTranslation("delete")]).then(res => {
                let HeaderDiv = document.createElement("div");
                HeaderDiv.className = TabelStyles.HeaderDivClass;
                HeaderDiv.innerHTML = TabelHeader(data.Name, res[0], res[1]);
                NewMainDiv.insertAdjacentElement('afterbegin', HeaderDiv);
            }).then(() => {
                this.DeleteButton = document.getElementById(`DeleteFromTable_${data.Name}`);
                this.DeleteButton.addEventListener('click', () => {
                    this.DeleteItems();
                });
            });

            let scrollDiv = document.createElement("div");
            scrollDiv.className = "scroll-TBody";

            let NewTable = document.createElement("TABLE");
            NewTable.className = TabelStyles.TableClass;

            scrollDiv.appendChild(NewTable);
            NewMainDiv.appendChild(document.createTextNode(''));

            NewMainDiv.appendChild(scrollDiv);

            this.generateTableHead(NewTable, data);
            let newTBody = document.createElement("tbody");
            NewTable.appendChild(newTBody);
            newTBody.className = TabelStyles.TbodyClass;

            //let additionalTds = CreateAdditionalTds(this.options);
            this.fillData(data.items, newTBody, NewTable, CreateAdditionalTds(this.options));

            this.tBody = newTBody;
            div.appendChild(NewMainDiv);
            this.init();

        } else {
            if (table != null && div != null && data.Options != null) {
                this.tBody = table.querySelector('tbody');
            }
        }

        //this.Checked = [];
        //this.TrToDelete = [];
        if (table != null && div != null && data.Options != null) {
            if ($(`#DeleteFromTable_${data.Name}`).length > 0) {
                this.DeleteButton = document.getElementById(`DeleteFromTable_${data.Name}`);
                this.DeleteButton.addEventListener('click', () => {
                    this.DeleteItems();
                });
            }
        }

    }

    generateTableHead(table, data) {
        let thead = table.createTHead();
        thead.className = TabelStyles.TheadClass;
        let row = thead.insertRow();
        row.className = TabelStyles.TheadTrClass;

        let number = 1;
        data.Headers.forEach(item => {
            let th = document.createElement('th');
            if (item.toString().toLowerCase().includes("id")) {
                th.style.display = 'none';
            } else {
                if (item == "") {
                    th.innerHTML = generateTd("SelectAll");
                } else {
                    let text;
                    if (item != "") {
                        GetWordTranslation(item.toString()).then(data => {
                            text = document.createTextNode(data);
                            let span = document.createElement('span');
                            span.appendChild(text);
                            span.className = "Translation";
                            th.appendChild(span);
                        });
                        th.className = TabelStyles.ThClass;
                    } else {
                        text = document.createTextNode("");
                        th.appendChild(text);
                    }
                }
            }

            th.id = `${data.Name}th_${item}`;
            row.appendChild(th);
            number = number + 1;
        });
        //this.options = data.Options;
        //this.name = data.Name;
    }

    generateTable(tabel, data) {
        let oldTBody = tabel.tBodies[0];
        let additionalTds = [];
        if (this.options) {
            additionalTds = CreateAdditionalTds(this.options);
        } else {
            let oldTds = oldTBody.children[0].children;
            oldTds.forEach(td => {
                if (td.id == "" && td.children.length > 0) {
                    additionalTds.push(td);
                }
            });
        }
        $(oldTBody).fadeOut();

        let newTBody = document.createElement('tbody');
        newTBody.className = TabelStyles.TbodyClass;
        $(newTBody).fadeOut();

        const NewTabelContent = new Promise((resolve, reject) => {
            setTimeout(() => resolve(this.fillData(data, newTBody, tabel, additionalTds)), 250);
        });

        NewTabelContent.then(() => {
            oldTBody.parentNode.replaceChild(newTBody, oldTBody);
            $(tabel.parentNode).animate({
                height: ($(newTBody).height() + 175)
            }, 100);
            $(newTBody).fadeIn();
        }).then(() => {
            this.tBody = newTBody;
            this.init();
        });
    }

    init() {
        this.tds = this.tBody.querySelectorAll('td');
        let columnsToDelete = [];
        this.tds.forEach(td => {
            let Name = td.id.substring(td.id.indexOf("_") + 1).replace(/[0-9]/g, '');
            if (td.style.display != "none" && td.children.length == 0) {
                if (this.options.EditableFields) {
                    if (this.options.EditableFields.includes(Name)) {
                        this.SetEditableFields(td);
                    }
                } else {
                    this.SetEditableFields(td);
                }

            } else if (td.children.length > 0) {
                if (td.firstElementChild.className.includes("checkbox")) {
                    const checkBox = td.querySelector("input");
                    checkBox.addEventListener('click', (event) => {
                        let tr = td.closest('tr');
                        let id = parseInt(tr.firstElementChild.textContent);
                        this.AssignId(id, this.Checked, this.TrToDelete, tr);
                    });
                }
            }

            this.isEmptyTd(td, columnsToDelete, Name);

        });

        this.deleteEmptyColumns(columnsToDelete);
    }

    deleteEmptyColumns(columnsToDelete) {
        let trs = this.tBody.closest("table").querySelectorAll('tr');

        columnsToDelete.forEach(column => {

            if (columnsToDelete.length > 0) {
                if ((columnsToDelete.filter(item => item == column).length) == trs.length - 1) {
                    let cell = Array.prototype.find.call(trs[1].children, item => item.id.includes(column) == true);
                    let index = cell.cellIndex;
                    let tabelRows = cell.parentNode.parentNode.parentNode.rows;
                    columnsToDelete = columnsToDelete.filter(item => item != column);
                    tabelRows.forEach(row => {
                        row.deleteCell(index);
                    });
                } else {

                }
            } else {
                return;
            }
        });
    }

    isEmptyTd(td, columnsToDelete, Name) {
        if (td.innerHTML == "") {
            columnsToDelete.push(Name);
        }
    }

    SetEditableFields(td) {
        td.setAttribute('contenteditable', true);
        td.addEventListener('click', (event) => {
            if (!this.inEditing(td)) {
                this.startEditing(td);
            }
        });
    }

    UpadateCell(td) {
        let params = {
            id: parseInt(td.closest('tr').firstElementChild.textContent),
            content: td.innerText.replace("CANCEL SAVE", '').trim()
        }
        HttpRequest.post(this.ActionsUrls.Update, params).then(res => {
            return res.data;
        }).then(result => {
            if (result == "True") {

            } else {
                GetWordTranslation("An error occured").then(data => {
                    const Info = new Modal("#Modal", ModalClass.Error, IconClass.Error, data, result, "", Buttons);
                    Info.init();
                    td.innerHTML = td.getAttribute("data-old-value");
                });
            }
        });
    }

    DeleteItems() {
        if (this.Checked.length > 0) {
            HttpRequest.post(this.ActionsUrls.Delete, this.Checked).then(res => {
                return res.data;
            }).then(result => {
                if (result == "True") {
                    let defaultTr = this.TrToDelete[0];
                    //let tbody = defaultTr.closest("tbody");

                    $(defaultTr.closest(".scroll-TBody")).animate({
                        height: ($(defaultTr.closest("table")).height() - (20 * this.TrToDelete.length))
                    }, 300);
                    this.TrToDelete.forEach(tr => {
                        $(tr).fadeOut();
                        setTimeout(() => {
                            tr.remove();
                        }, 300);
                    });

                    this.isEmpty(this.tBody);

                } else {
                    GetWordTranslation("An error occured").then(data => {
                        const Info = new Modal("#Modal", ModalClass.Error, IconClass.Error, data, result, "", Buttons);
                        Info.init();
                    });
                }
                this.Checked = [];
                this.TrToDelete = [];
            });
        }
    }

    isEmpty(tbody) {
        if (tbody.children.length == this.TrToDelete.length) {
            setTimeout(() => {
                NoData(tbody);
            }, 300);
        }
    }

    AssignId(id, Checked, TrToDelete, tr) {
        var removeIndex = Checked.map(function (item) { return item.id; }).indexOf(id);
        if (removeIndex != (-1)) {
            Checked.splice(removeIndex, 1);
            TrToDelete.splice(removeIndex, 1);
        } else {
            Checked.push({ id: id });
            TrToDelete.push(tr);
        }
    }

    startEditing(td) {
        const activeTd = this.findEditing();
        if (activeTd) {
            this.cancelEditing(activeTd);
        }

        td.classList.add("in-editing");
        td.setAttribute("data-old-value", td.innerHTML);
        this.CreateButtonToolbar(td);
    }

    cancelEditing(td) {
        td.innerHTML = td.getAttribute("data-old-value");
        td.classList.remove("in-editing");
    }

    finishEditing(td) {
        if (td.getAttribute("data-old-value").trim() != td.innerText.replace("CANCEL SAVE", '').trim()) {
            this.UpadateCell(td);
        }
        td.classList.remove("in-editing");
        this.removeToolbar(td);
    }

    inEditing(td) {
        return td.classList.contains("in-editing");
    }

    findEditing() {
        return Array.prototype.find.call(this.tds, td => td.classList.contains("in-editing"));
    }

    CreateButtonToolbar(td) {
        const toolbar = document.createElement("div");
        toolbar.className = "button-toolbar";
        toolbar.setAttribute('contenteditable', false);
        toolbar.innerHTML = `<div class="button-wrapper">
                                <button class="btn btn-danger btn-sm EditTableCancel">Cancel</button>
                                <button class="btn btn-success btn-sm EditTableSave">Save</button>
                            <div>`
        td.appendChild(toolbar);

        const SaveButton = toolbar.querySelector(".EditTableSave");
        const CancelButton = toolbar.querySelector(".EditTableCancel");

        SaveButton.addEventListener("click", (event) => {
            event.stopPropagation();
            this.finishEditing(td);
        });

        CancelButton.addEventListener("click", (event) => {
            event.stopPropagation();
            this.cancelEditing(td);
        });
    }

    removeToolbar(td) {
        const toolbar = td.querySelector(".button-toolbar");
        toolbar.remove();
    }

    fillData(data, newTBody, tabel, additionalTds = null) {
        if (data != null) {
            let idName = null;

            if (data.Name) {
                idName = `${data.Name}_`;
            } else {
                if (this.name) {
                    idName = `${this.name}td`;
                } else {
                    idName = tabel.querySelector("td").id.substring(0, tabel.querySelector("td").id.indexOf("_"));
                }
                let options = this.options
                let number = 1;
                data.forEach(function (item) {
                    let row = newTBody.insertRow();

                    for (let element of Object.entries(item)) {
                        let cell = row.insertCell();
                        let text = document.createTextNode(element[1]);
                        let NewIdName;
                        if (element[0].toLowerCase().includes("id")) {
                            cell.style.display = 'none';
                        }
                        if (options.InputFields.includes(element[0])) {
                            cell.style.display = 'none';
                        }
                        NewIdName = `${idName}_${element[0]}${number}`;
                        cell.appendChild(text);
                        cell.id = NewIdName;
                    }
                    //if there will be other fields, change this to fuction !!!!!!!!!!!!
                    if (additionalTds != null) {
                        additionalTds.forEach(td => {
                            let additionalCell = row.insertCell();
                            let TextNode = td.cloneNode(true);
                            SetId(td, TextNode, idName, number);
                            additionalCell.parentNode.replaceChild(TextNode, additionalCell);
                        })
                    }
                    number = number + 1;
                });
            }
        } else {
            NoData(newTBody);
        }
    }
}

function NoData(tbody) {
    let tabel = tbody.closest("table");
    GetWordTranslation("There's no data").then(data => {
        let columns = tabel.tHead.children[0].children.length;
        let newtr = document.createElement("tr");
        let newtd = document.createElement("td");
        newtd.setAttribute("colspan", columns.toString())
        let newContent = document.createTextNode(`${data}`);
        let span = document.createElement("span");
        span.className = "Translation";
        span.appendChild(newContent);
        newtd.appendChild(span);
        newtd.setAttribute('contenteditable', false);
        newtr.appendChild(newtd);
        tbody.appendChild(newtr);
    });
}

function SetId(td, TextNode, idName, number) {
    if (td.firstElementChild.className.includes("checkbox")) {
        TextNode.querySelector(".custom-control-input").id = `${idName}_CheckBox${number}`;
        TextNode.querySelector(".custom-control-label").setAttribute("for", `${idName}_CheckBox${number}`);
    }
}

function generateTd(type) {
    switch (type) {
        case "checkbox":
            return CheckBox;
            break;
        case "SelectAll":
            GetWordTranslation("Select All").then(data => {
                SelectAllButton(data);
            });
            return SelectAll;
            break;
    }
}

const TabelStyles = {
    DivClass: "card pt-5 px-1",
    HeaderDivClass: "blue-gradient p-2 mb-3 d-flex justify-content-between tableHeader align-items-center z-depth-2",
    TableClass: "table table-striped table-responsive-sm btn-table pt-2 table-hover",
    TheadClass: "text-center",
    TheadTrClass: "font-weight-bold",
    ThClass: "font-weight-bold",
    TbodyClass: "text-center",
    TbodyTrClass: "",
    TbodyTdClass: ""
}

const CheckBox = `<div class="custom-control custom-checkbox">
                      <input type="checkbox" class="custom-control-input" id="" >
                      <label class="custom-control-label" for=""></label>
                  </div>`;

function SelectAllButton(Translation) {
    return `<button type="button" class="btn btn-outline-primary btn-sm m-0 waves-effect"><span class="Translation">${Translation}</span></button>`;
}

function TabelHeader(name, Translation, Action) {
    return `<div>
                <button type="button" class="btn btn-outline-white btn-sm px-2">
                    <i class="fas fa-th-large mt-0"></i>
                </button>
            </div>
            <span class="white-text Translation font-weight-bold TableHeaderFont">${Translation}</span>
            <div>
                <button type="button" class="btn btn-danger btn-sm font-weight-bold px-2" id="DeleteFromTable_${name}">
                    <span class="Translation">${Action}</span>&nbsp;
                    <i class="fa x2 far fa-trash-alt mt-0"></i>
                </button>
            </div>`;
}

function CreateAdditionalTds(options = null, oldTBody = null) {
    let additionalTds = [];
    if (options != null) {
        if (options.HasCheckboxes == true) {
            let newTd = document.createElement("td");
            newTd.innerHTML = generateTd("checkbox");
            additionalTds.push(newTd);
        }

        //if (options.HasCheckboxes == true) {
        //    let newTd = document.createElement("td");
        //    newTd.innerHTML = generateTd("checkbox");
        //    additionalTds.push(newTd);
        //}

        //if (options.Hasbuttons == true) {
        //    let newTd = document.createElement("td");
        //    newTd.innerHTML = generateTd("Buttons");
        //    additionalTds.push(newTd);
        //}

    } else {
        let oldTds = oldTBody.children[0].children;
        oldTds.forEach(td => {
            if (td.id == "" && td.children.length > 0) {
                additionalTds.push(td);
            }
        });
    }

    return additionalTds;
}



