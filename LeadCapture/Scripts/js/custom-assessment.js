$(document).ready(function () {
    App.init();

    $("div.row").find("input:checkbox").each(function () {
        $(this).bind('click', function () { noaCbxHandler(this); });
    });

    $('#myForm').formValidation();
    showProgress();
});

function onFormSuccess(e) {
    e.preventDefault();

    var $form = $(e.target),
        fv = $form.data('formValidation');

    $(".loading").show();
    $('html, body').animate({ scrollTop: 0 }, 'fast');

    disableAllButtons();
    setTimeout('enableAllButtons()', 1000);

    $.ajax({
        url: $form.attr('action'),
        type: 'POST',
        data: $form.serialize(),
        success: function (data) {
            $(".loading").hide();
            var currentPage = $("#CurrentPage").val();
            if (data["result"] == 'success') {
                if (currentPage == "3") {
                    startCountdown();
                }
                else {
                    redirect(data);
                }
            }
            else {
                if ($("#retry").val() === 'true' && currentPage == "3") {
                    $("#retry").val('false');
                    startCountdown();
                    return;
                }
                $("#retry").val('true');
                showError();
            }
        },
        error: function (data) {
            $(".loading").hide();
            $("#retry").val('true');
            showError();
        }
    });
};

function redirect(data) {
    var targetUrl = data["targetUrl"];
    if (targetUrl != null) {
        window.location = targetUrl;
    }
}

function showProgress() {
    var progress = "0";
    var targetPage = "1";
    if ($("#TargetPage") != null) targetPage = $("#TargetPage").val();
    
    if (targetPage == "2") progress = "50";
    if (targetPage == "3") progress = "100";

    $("#progress").val(progress);
    $("#progressText").text(progress);
    $("#progressBar").width(progress + "%");
}

function showError() {
    alert($("#errorMsg").val());
}

function enableAllButtons() {
    $(".btn").removeAttr("disabled");
    $(":button").removeAttr("disabled");
}

function disableAllButtons() {
    $(".btn").attr("disabled", "disabled");
    $(":button").attr("disabled", "disabled");
}

function startCountdown() {
    var targetUrl = $("#redirect_link").attr('href');
    if (targetUrl == null) targetUrl = 'http://www.ricoh.com';

    $('#thankyou').modal('show');
    var seconds = 15;
    $("#lblCount").html(seconds);
    setInterval(function () {
        seconds--;
        $("#lblCount").html(seconds);
        if (seconds == 0) {
            window.location = targetUrl;
        }
    }, 1000);
}

function altDropDownHandler(source, altChoice, altId) {
    var targetClass = "." + altId;

    if ($(source).val() == altChoice) {
        $(targetClass).show();
    } else {
        $(targetClass + " input").val("");
        $(targetClass).hide();
    }
}

function noaCbxHandler(source) {
    if (source.checked) {
        var container = $(source).parent().parent();
        
        // toggle selection
        if ($(source).attr('class') == 'noa') {
            var targets = container.find('input:checkbox').not('.noa');
            jQuery.each(targets, function () {
                $(this).attr('checked', false);
            });
        }
        else {
            var target = container.find('.noa');
            target.attr('checked', false);
        };
    }
}