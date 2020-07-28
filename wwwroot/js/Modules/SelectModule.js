export function InitPicker(options = {}) {
    if (options != null) {
        const myPicker = new Lightpick({
            field: document.getElementById(options.name),
            singleDate: false,
            // number of months to display
            numberOfMonths: 2,
            // number of columns to display
            numberOfColumns: 2,
            // shows footer
            footer: true,
            orientation: options.orientation,
            dropdowns: {
                years: {
                    min: options.minYear,
                    max: options.maxYear,
                },
                months: true,
            },
            // localization
            locale: {
                buttons: {
                    prev: '<i class="fas fa-arrow-alt-circle-left"></i>',
                    next: '<i class="fas fa-arrow-alt-circle-right"></i>',
                    close: '×',
                    reset: 'Reset',
                    apply: 'Apply',
                }
            }
        });

        return myPicker;
    }
}

export function GetDates() {

}


export class SelectList {
    constructor(options = {}) {
        this.name = options.name
        this.items = options.items
        this.selected = options.selected

        this.SetItems = function () {
            let select = document.getElementById(options.name);
            select.innerHTML = "";
            this.items.forEach(function (item) {
                let opt = document.createElement('option');
                opt.appendChild(document.createTextNode(item.text));
                opt.value = item.id; 
                select.appendChild(opt); 
            });          
            $(select).selectpicker('refresh');
            console.log(this.selected);
            let checkId = this.items.find(item => item.id == this.selected);
            if (checkId != undefined) {
                $(select).selectpicker('val', this.selected);
            } else {
                //$(select).selectpicker('val', 0);
                $(select).selectpicker('val', this.items[0].id);
            }            
        }
    }
}



export function OnChange(element, action) {
    element.change(action);
}