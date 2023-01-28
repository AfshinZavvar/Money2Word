// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function CurrencyValidation(e) {
    var amount = $("#Amount").val();
    if (!IsValidCurrency(amount)) {
        e.preventDefault();
    }
    else {
        return true;
    }

}

function IsValidCurrency(amount) {
    var regex = /^\d{1,3}(?:[.]\d{0,2})?$/;
    var result = amount.match(regex);
    if (!result) {
        return false;
    }
    else {
        return true;
    }
}


function EnbableButton(enable) {
    $("#btnSubmit").prop('disabled', !enable);
}

function ValidateInputs() {

    var amount = $("#Amount").val();

    ClearResponses();
    if ( !amount) {
        $("#responseError").text("Amount has wrong value");
        return false;
    }
    if (!IsValidCurrency(amount))  {
        $("#responseError").text("Amount has wrong value");
        return false;
    }
    return true;
}

function ShowResponse(response) {

    $("#resopnseAmount").html("<strong>Amount:</strong>" + response.Amount);
    $("#responseError").text(response.errorMessage);
}

function ClearResponses() {

    $("#resopnseAmount").text("");
    $("#responseError").text("");
}


function Submit() {
    var data = {
        "Amount": $("#Amount").val().trim()
    };

    $.ajax({
        url: "/api/show/",
        type: "POST",
        data: JSON.stringify(data),
        dataType: 'JSON',
        contentType: 'application/json; charset=utf-8',
        headers: {
            "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            ShowResponse(data);
        },
        error: function (data) {
            ClearResponses();
            $("#responseError").text("error occured calling API");
        }
    });
}

$(document).ready(function () {

    ClearResponses();

    $("#Amount").bind('paste', function (e) {
        e.preventDefault();
    });

    $("#btnSubmit").on("click keypress", function () {
        if (!ValidateInputs()) { return; }
        Submit();
    });
});