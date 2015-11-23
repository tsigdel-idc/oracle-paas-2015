$(document).ready(function () {
    App.init();

    $('#signout-link').on('click', function (event) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/account/logoff',
            type: "POST",
            data: { __RequestVerificationToken: token },
            success: function() {
                window.location = '/account/logoutconfirmation';
            }
        });
        event.preventDefault();
        return false;
    });

    var frm = $('.sky-form');
    if (frm.length) {

        // change the text on each required attribute
        $.extend($.validator.messages, {
            required: $("#Validation").attr('requiredMsg')
        });

        // strong password rule
        $.validator.addMethod("strongpwd", function (value) {
            return value.match(/^[A-Za-z0-9\d=!\-@._*]*$/)
                && value.match(/[a-z]/)
                && value.match(/[A-Z]/)
                && value.match(/\d/);
        });

        frm.validate();
    }
});

function submit(event, source) {
    var frm = $(source).closest('.sky-form');

    if (frm.valid()) {
        $.ajax({
            type: frm.attr('method'),
            url: frm.attr('action'),
            data: frm.serialize(),
            success: function (data) {
                if (data["result"] == 'success') {
                    showResult(data);
                }
                else {
                    var msg = data["message"];
                    showError(msg);
                }
            },
            error: function (data) {
                showError();
            }
        });

        event.preventDefault();
    }
};

function showResult(data) {
    var msg = data["message"];
    alert(msg);
}

function showError(msg) {
    if (msg == null) msg = $('#error-msg').val();
    alert(msg);
}