import { HttpRequest } from '../Modules/AxiosModule.js';


let ShowLeftMenu = sessionStorage.getItem("ShowLeftMenu");
let ShowRightMenu = sessionStorage.getItem("ShowRightMenu");
const NavAreas = $(".Area");
export let Company = document.getElementById("CompanySelect");
var url = "api/Menu/";

export class SideNav {
    constructor(sideNav, trigger, closeBtn, wrapper, wrapperClass) {
        this.openTrigger = function () {
            CheckSession(trigger, sideNav, wrapper, wrapperClass);
            $(trigger).click(function () {
                if (window.screen.width > 767 || document.body.clientWidth > 767) {
                    SetStyles(sideNav, trigger, wrapper, wrapperClass, true)
                }
            });
        };
        this.closeTrigger = function () {
            $(closeBtn).click(() => {
                SetStyles(sideNav, trigger, wrapper, wrapperClass, false)
            });
        }
        //if (sideNav.id == "LeftSidenav") {
        //    this.anchorListener = function () {
        //        let Anchors = $("a");
        //        Anchors.click(() => {
        //            MenuSession("Post", "ShowLeftMenu", 1);
        //            // sessionStorage.setItem("ShowLeftMenu", 1);
        //        });
        //    }
        //    this.CompanySelectListener = function () {
        //        $("#CompanySelect").change(function () {
        //            MenuSession("Post", "MenuCompany", this.value);
        //        });
        //    }
        //}

    }
}

export class LeftNav extends SideNav {
    constructor(sideNav, trigger, closeBtn, wrapper, wrapperClass) {
        super(sideNav, trigger, closeBtn, wrapper, wrapperClass);
        this.anchorListener = function () {
            let Anchors = $("a");
            Anchors.click(function (e) {
                e.preventDefault();
                //Create Promise !!!!!!!!!!!!!
                MenuSession("Post", "ShowLeftMenu", 1);
                let IdMenu = this.getAttribute("data-id");
                if (IdMenu != null) {
                    MenuSession("Post", "IdMenu", IdMenu);
                }
                location.href = this.getAttribute("href");
            });
        }
        this.CompanySelectListener = function () {
            $("#CompanySelect").change(function () {
                //console.log("change");
                MenuSession("Post", "MenuCompany", this.value);
            });
        }
    }
}

function CheckSession(trigger, sideNav, wrapper, wrapperClass) {
    if ((ShowLeftMenu == 1) && (trigger.attr("id") == "sidenavLeft")) {
        sideNav.classList.add("ShowLeftMenu");
        trigger.parent().addClass("active");
        trigger.removeClass("text-white");
        trigger.addClass("text-dark");
        SetWrapperSession(wrapper, wrapperClass);
    }

    if ((ShowRightMenu == 1) && (trigger.attr("id") == "sidenavRight")) {
        trigger.removeClass("text-white");
        sideNav.classList.add("ShowRightMenu");
        SetWrapperSession(wrapper, wrapperClass);
    }
}

function SetWrapperSession(wrapper, wrapperClass) {
    wrapper.addClass(wrapperClass);//
    wrapper.addClass("TransitionUnset");//    
    document.body.style.backgroundColor = "rgba(0,0,0,0.3)";//
}

export function MenuSession(method, MenuName, id) {
    let params = {
        Id: id,
        Text: MenuName
    }
    switch (method) {
        case "Get":
            //let GetUrl = `${url}${MenuName}`;
            //console.log(MenuName);
            return HttpRequest.get(`${url}${MenuName}`).then((response) => {
                return response.data;
            });
            break;
        case "Post":
            HttpRequest.post(url, params);
            sessionStorage.setItem(MenuName, id);
            break;
        case "Put":
            HttpRequest.put(url, params);
            sessionStorage.setItem(MenuName, id);
            break;
        case "Delete":
            HttpRequest.delete(url, id);
            break;
    }
}

function SetStyles(sideNav, trigger, wrapper, wrapperClass, Open = true) {
    if (Open == true) {
        if (trigger.attr("id") == "sidenavLeft") {
            trigger.parent().addClass("active");
            sideNav.style.width = "330px";
            MenuSession("Post", "ShowLeftMenu", 1);
        } else {
            trigger.removeClass("text-white");
            sideNav.style.width = "250px";
            MenuSession("Post", "ShowRightMenu", 1);
        }
        trigger.removeClass("text-white");
        trigger.addClass("text-dark");
        wrapper.addClass(wrapperClass);
        document.body.style.backgroundColor = "rgba(0,0,0,0.3)";
    } else {
        if (trigger.attr("id") == "sidenavLeft") {
            trigger.parent().removeClass("active");
            MenuSession("Put", "ShowLeftMenu", 0);
            sideNav.classList.remove("ShowLeftMenu");
        } else {
            MenuSession("Put", "ShowRightMenu", 0);
            sideNav.classList.remove("ShowRightMenu");
        }
        trigger.removeClass("text-dark");
        trigger.addClass("text-white");
        sideNav.style.width = "0";
        wrapper.removeClass(wrapperClass);
    }
    wrapper.removeClass("TransitionUnset");
    CheckNavs();
}

function CheckNavs() {
    let sideNavs = $(".sidenav");
    let check = true;
    sideNavs.each(function () {
        let width = parseInt(this.style.width, 10);
        if (isNaN(width) == true) {
            width = 0;
        }
        if ((width > 0) || (this.classList.contains("ShowLeftMenu") || this.classList.contains("ShowRightMenu"))) {
            check = false;
        }
    });
    if (check == true) {
        document.body.style.backgroundColor = "white";
    } else {
        document.body.style.backgroundColor = "rgba(0,0,0,0.3)";
    }
}

let AnchorActive = $("#MenuLinks").find('.menu-div.active');
function Rotate(element, parent) {
    let all_Rest = $(parent).children().children().children().find('.rotate-icon').not(element);
    let ParentsArea = $(parent).children().children().children().find('.show');

    // console.log(ParentsArea);
    if (all_Rest.hasClass('RotateArrow')) {
        all_Rest.removeClass('RotateArrow');
    }

    $(element).toggleClass('RotateArrow');

    let AnchorParent = AnchorActive.parent();
    ParentsArea.not(AnchorParent).collapse('hide');
}



$(document).ready(function () {
    var Menus = $(".Menu");
    var topPos;
    var previousTarget = null;

    if (AnchorActive.length > 0) {
        topPos = AnchorActive.offset().top;
        document.getElementById('MenuLinks').scrollTop = (topPos - 260);
    }

    $(".collapse").on('hidden.bs.collapse', function () {
        let syb = $(this).siblings();
        let icon = syb.find("i.far");
        if (!$(this).hasClass("show")) {
            icon.removeClass("far fa-minus-square");
            icon.addClass("far fa-plus-square");
        }
    });

    $(".collapse").on('shown.bs.collapse', function () {
        let syb = $(this).siblings();
        let icon = syb.find("i.far");
        if ($(this).hasClass("show")) {
            icon.removeClass("far fa-plus-square");
            icon.addClass("far fa-minus-square");
        }
        if (!(this === previousTarget)) {
            let Menu = $(this).closest('.Menu');
            let position = $.inArray(Menu[0], Menus);

            $('#MenuLinks').animate({
                scrollTop: (position * 40)
            }, 1);
            previousTarget = this;
            //console.log((position * 40));
        }
    });

    NavAreas.click(function () {
        let arrow = $(this).find(".rotate-icon");
        let AccordionId = "#" + this.parentNode.parentNode.parentNode.id;
        Rotate(arrow, AccordionId);
    });
});



