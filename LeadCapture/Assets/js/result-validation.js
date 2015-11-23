/* Write here your custom javascript codes */

$(document).ready(function() {
    $('#resultform').formValidation({
        framework: 'bootstrap',

        fields: {
            'fname': {
                validators: {
                    notEmpty: {
                        message: 'Please enter your first name'
                    }
                }
            },

            'lname': {
                validators: {
                    notEmpty: {
                        message: 'Please enter your last name'
                    }
                }
            },

            'email': {
                validators: {
                    notEmpty: {
                        message: 'Please enter your e-mail address'
                    }
                }
            },

            'phone': {
                validators: {
                    notEmpty: {
                        message: 'Please enter your phone number'
                    }
                }
            }
            

        }
    });

});