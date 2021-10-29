// Language JSON File Location
var language = localStorage.getItem('language');
// Default Language
var default_lang = 'en';

// Set Selected Language
function setLanguage(lang) {
    if(lang=='en') {
        document.getElementById("header-lang-img").src="assets/images/flags/us.jpg";
    } else if(lang=='sp') {
        document.getElementById("header-lang-img").src="assets/images/flags/spain.jpg";
    }
    else if(lang=='gr') {
        document.getElementById("header-lang-img").src="assets/images/flags/germany.jpg";
    }
    else if(lang=='it') {
        document.getElementById("header-lang-img").src="assets/images/flags/italy.jpg";
    }
    else if(lang=='ru') {
        document.getElementById("header-lang-img").src="assets/images/flags/russia.jpg";
    }
    localStorage.setItem('language', lang);
    language = localStorage.getItem('language');
    // Run Multi Language Plugin
    getLanguage()
}

// Run Multi Language Plugin
function getLanguage() {
    // Language on user preference
    (language == null) ? setLanguage(default_lang) : false;
    // Load data of selected language
    $.ajax({
        url: 'assets/lang/' + language + '.json',
        dataType: 'json', async: true
    }).done(function (lang) {
        // add selected language class to the body tag
        $('html').attr('lang', language);
        // Loop through message in data
        $.each(lang, function (index, val) {
            (index === 'head') ? $(document).attr("title", val['title']) : false;
            $(index).children().each(function () {
                $(this).text(val[$(this).attr('key')])
            })
            $(index).children().children().each(function () {
                $(this).text(val[$(this).attr('key')])
            })
        })
    })
}

// Auto Loader
$(document).ready(function () {
    if (language != null && language !== default_lang)
        getLanguage(language);
});
