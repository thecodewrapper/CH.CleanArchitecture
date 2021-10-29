//Helper HTML functions
function GetViewBtnHtml(link, alt, id) {
    return '<a href="' + link.replace("tmpid", id) + '" class="btn btn-sm btn-info width-30 height-30" data-toggle="tooltip" data-placement="top"  title="' + alt + '"><i class="fas fa-info"></i></a> ';
}

function GetLinkHtml(link, alt, id, text) {
    return '<a href="' + link.replace("tmpid", id) + '" class="btn btn-sm" data-toggle="tooltip" data-placement="top"  title="' + alt + '">' + text + '</a> ';
}

function GetEditBtnHtml(link, alt, id) {
    return '<a href="' + link.replace("tmpid", id) + '" class="btn btn-sm btn-warning width-30 height-30" data-toggle="tooltip" data-placement="top" title="' + alt + '"><i class="fas fa-edit"></i></a> ';
}

function GetDeleteBtnHtml(link, alt) {
    return '<a id="deleteBtn" title="' + alt + '" class="btn btn-sm btn-danger width-30 height-30"  href="' + link + '"><i class="fas fa-trash fa-lg"></i></a>';
}

function GetDeactivateBtnHtml(link, alt) {
    return '<a id="deactivateBtn" title="' + alt + '" class="btn btn-sm btn-danger" href="' + link + '"><i class="fas fa-power-off"></i> </a>';
}

function GetActivateBtnHtml(link, alt) {
    return '<a id="activateBtn" title="' + alt + '" class="btn btn-sm btn-success" href="' + link + '"><i class="fas fa-power-off fa-lg"></i> </a>';
}

function GetReportStatusBadge(status, localizedStatus) {
    var statusClass = null;
    switch (status) {
        case 0:
            statusClass = 'badge-soft-info';
            break;
        case 1:
            statusClass = 'badge-soft-primary';
            break;
        case 2:
            statusClass = 'badge-soft-danger';
            break;
        case 3:
            statusClass = 'badge-soft-success';
            break;
        default:
            break;
    }
    return '<div onclick="return false;" class="badge ' + statusClass + ' font-size-11 m-1" >' + localizedStatus + '</div>';
}

function decodeHTMLString(text) {
    return $("<textarea/>")
        .html(text)
        .text();
}

function GetActionUrlWithId(action, id) {
    var link = '@Url.Action(' + action + ', new { id = "xx" })';
    return link.replace("xx", id);
}

function scrollTo(x, y) {
    window.scroll(x, y);
}

/* Blazor Functions */

function ResizeElement(element) {
    if (!element) {
        return;
    }
    element.style.height = '5px';
    element.style.height = (element.scrollHeight + 2) + "px";
}

$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('#back-to-top').fadeIn();
        } else {
            $('#back-to-top').fadeOut();
        }
    });
    // scroll body to 0px on click
    $('#back-to-top').click(function () {
        $('body,html').animate({
            scrollTop: 0
        }, 400);
        return false;
    });
});

function TriggerElementCollapse(elementId) {
    if ($(elementId) == null) {
        return;
    }

    $(elementId).collapse('toggle');

    $(elementId).find('.resizable-textarea').each(function () {
        ResizeElement(this);
    });
}

function InitializePhotoPopovers() {
    $('.popover-wrapper a').popover(
        {
            boundary: 'window',
            container: 'body',
            html: true,
            trigger: 'hover click',
            placement: 'top',
            content: function () { return '<img src="' + $(this).data('img') + '"  class="img-fluid"/>'; }
        });
}

function focus(element) {
    if (!element) {
        return;
    }
    element.focus();
}


function blur(element) {

    if (!element) {
        return;
    }

    element.blur();
}
function scrollToElement(id) {
    var element = document.getElementById(id);

    if (!element) {
        return false;
    }

    var elmTop = element.getBoundingClientRect().top + window.scrollY;

    if (elmTop) {
        $('body,html').animate({
            scrollTop: elmTop
        }, 400);
      
        return true;
    }

    return false;
}

function downloadFromUrl(url, fileName) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}