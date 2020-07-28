//history.replaceState(window.location.href, null, window.location.pathname);
import { SideNav, MenuSession, LeftNav } from './Modules/MenuModule.js';
import { HttpRequest } from './Modules/AxiosModule.js';
import './Translations.js';

const Main = $('main');
const Theme = sessionStorage.getItem("data-theme");
const ModeCheckbox = $("#MaterialToggleBlue");
const wrapper = $(".wrapper");
const ScrollingNav = $(".scrolling-navbar");

let Table = $('table');

const RightSideNav = new SideNav(document.getElementById("RightSidenav"), $("#sidenavRight"), $("#closebtnLeft"), wrapper, "SideNavRight");
RightSideNav.openTrigger();
RightSideNav.closeTrigger();

const LeftSideNav = new LeftNav(document.getElementById("LeftSidenav"), $("#sidenavLeft"), $("#closebtnRight"), wrapper, "SideNavLeft");
LeftSideNav.openTrigger();
LeftSideNav.closeTrigger();
LeftSideNav.anchorListener();
LeftSideNav.CompanySelectListener();


if (Theme == 1) {
    Main.attr('data-theme', 'dark');
    ModeCheckbox.prop("checked", true);
} else {
    Main.attr('data-theme', 'light');
    ModeCheckbox.prop("checked", false);
}

$(window).bind("pageshow", function () {
    $('input.form-control').each(function () {
        this.value = "";
    });
    $('input').blur()
});

$(document).ready(function () {
    ModeCheckbox.click(function () {
        if (this.checked) {
            sessionStorage.setItem("data-theme", 1);
            Main.attr('data-theme', 'dark');
            MenuSession("Post", 'data-theme', 1);
            Table.addClass("table-dark");
            Table.parent().addClass("special-color-dark");
            Table.parent().parent().addClass("special-color-dark");
        } else {
            Main.attr('data-theme', 'light');
            sessionStorage.setItem("data-theme", 0);
            MenuSession("Put", 'data-theme', 0);
            Table.removeClass("table-dark");
            Table.parent().removeClass("special-color-dark");
            Table.parent().parent().removeClass("special-color-dark");
        }
    });
    $('#CompanySelect').selectpicker();
    let IdCompany = sessionStorage.getItem("MenuCompany");
    if (IdCompany != $("#CompanySelect").selectpicker('val')) {
        MenuSession("Get", "MenuCompany").then(data => {
            $("#CompanySelect").selectpicker('val', data);
            console.log(data);
        });
    }
});

Main.scroll(function () {
    if (this.scrollTop > 8) {
        ScrollingNav.addClass("top-nav-collapse");
    } else {
        ScrollingNav.removeClass("top-nav-collapse");
    }
});



