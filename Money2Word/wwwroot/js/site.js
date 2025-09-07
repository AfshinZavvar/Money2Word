function IsValidCurrency(amount) {
    // Allow large numbers with optional commas and up to 2 decimal places
    var regex = /^\d{1,3}(?:,\d{3})*(?:\.\d{1,2})?$/;
    return regex.test(amount);
}

function EnableButton(enable) {
    $("#btnSubmit").prop('disabled', !enable);
}

function ValidateInputs() {
    var amount = $("#Amount").val().trim();
    ClearResponses();

    if (!amount || !IsValidCurrency(amount)) {
        $("#responseError").text("Amount has wrong value");
        return false;
    }
    return true;
}

function ShowResponse(response) {
    $("#resopnseAmount").html("<strong>Amount:</strong> " + (response.Amount || ""));
    $("#responseError").text(response.errorMessage || "");
}

function ClearResponses() {
    $("#resopnseAmount").text("");
    $("#responseError").text("");
}

function FormatCurrencyInput(value) {
    // Remove commas for processing
    value = value.replace(/,/g, "");
    if (value === "") return "";

    // Split into whole and decimal
    let parts = value.split(".");
    let whole = parts[0];
    let decimal = parts.length > 1 ? "." + parts[1] : "";

    // Add commas to whole part
    whole = whole.replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    return whole + decimal;
}

function Submit() {
    // Raw value without commas
    var rawAmount = $("#Amount").val().replace(/,/g, "").trim();

    var data = { "Amount": rawAmount };

    $.ajax({
        url: "/api/show/",
        type: "POST",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: ShowResponse,
        error: function () {
            ClearResponses();
            $("#responseError").text("Error occurred calling API");
        }
    });
}

$(document).ready(function () {
    ClearResponses();

    $("#Amount").bind('paste', function (e) {
        e.preventDefault();
    });

    // Auto-format as user types
    $("#Amount").on("input", function () {
        let caretPos = this.selectionStart; // store cursor position
        let formattedValue = FormatCurrencyInput($(this).val());
        $(this).val(formattedValue);
        this.setSelectionRange(caretPos, caretPos); // restore cursor
    });

    $("#btnSubmit").on("click keypress", function () {
        if (ValidateInputs()) {
            Submit();
        }
    });
    $("#Amount").on("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            if (!ValidateInputs()) return;
            Submit();
        }
    });
});
