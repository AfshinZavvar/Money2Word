const MIN_AMOUNT = 0.01;
const MAX_AMOUNT = 999_999_999_999_999.99;

function IsValidCurrency(amount) {
    const regex = /^\d{1,3}(?:,\d{3})*(?:\.\d{1,2})?$/;
    return regex.test(amount);
}

function ValidateInputs() {
    const amount = $("#Amount").val().trim();
    ClearResponses();

    if (!amount || !IsValidCurrency(amount)) {
        $("#Amount").attr("aria-invalid", "true");
        $("#responseError").text("Please enter a valid amount (e.g. 1,234.56)");
        return false;
    }

    const numeric = parseFloat(amount.replace(/,/g, ""));
    if (numeric < MIN_AMOUNT) {
        $("#Amount").attr("aria-invalid", "true");
        $("#responseError").text("Amount must be at least $0.01");
        return false;
    }
    if (numeric > MAX_AMOUNT) {
        $("#Amount").attr("aria-invalid", "true");
        $("#responseError").text("Amount must not exceed $999,999,999,999,999.99");
        return false;
    }

    $("#Amount").removeAttr("aria-invalid");
    return true;
}

const HIGHLIGHT_WORDS = new Set([
    "HUNDRED", "THOUSAND", "MILLION", "BILLION", "TRILLION",
    "DOLLAR", "DOLLARS", "CENT", "CENTS"
]);

function ShowResponse(response) {
    const $amountEl = $("#responseAmount").empty();
    if (response.Words) {
        response.Words.split(" ").forEach(function (word, index) {
            if (index > 0) $amountEl.append(document.createTextNode(" "));
            if (HIGHLIGHT_WORDS.has(word)) {
                $("<span>").addClass("word-highlight").text(word).appendTo($amountEl);
            } else {
                $amountEl.append(document.createTextNode(word));
            }
        });
        $("#resultPanel").show();
    }
    $("#responseError").text(response.ErrorMessage || "");
}

function ClearResponses() {
    $("#responseAmount").empty();
    $("#responseError").text("");
    $("#resultPanel").hide();
    $("#Amount").removeAttr("aria-invalid");
}

function FormatCurrencyInput(value) {
    value = value.replace(/,/g, "");
    if (value === "") return "";

    const parts = value.split(".");
    const whole = parts[0].slice(0, 15).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    const decimal = parts.length > 1 ? "." + parts[1].slice(0, 2) : "";

    return whole + decimal;
}

function Submit() {
    const rawAmount = $("#Amount").val().replace(/,/g, "").trim();
    const $btn = $("#btnSubmit");

    $btn.prop("disabled", true).attr("aria-label", "Converting, please wait").find(".btn-text").text("Converting…");

    $.ajax({
        url: "/api/show",
        type: "POST",
        data: JSON.stringify({ Amount: rawAmount }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        timeout: 10000,
        success: ShowResponse,
        error: function (xhr, status) {
            ClearResponses();
            if (status === "timeout") {
                $("#responseError").text("Request timed out. Please try again.");
                return;
            }
            let msg = "An error occurred. Please try again.";
            try {
                const problem = JSON.parse(xhr.responseText);
                if (problem.errors) {
                    const messages = Object.values(problem.errors).flat();
                    if (messages.length > 0) msg = messages[0];
                } else if (problem.detail) {
                    msg = problem.detail;
                } else if (problem.title) {
                    msg = problem.title;
                }
            } catch (e) { /* non-JSON response — keep generic message */ }
            $("#responseError").text(msg);
        },
        complete: function () {
            $btn.prop("disabled", false).attr("aria-label", "Convert amount to words").find(".btn-text").text("Convert");
        }
    });
}

$(document).ready(function () {
    ClearResponses();
    $("#Amount").trigger("focus");

    $("#Amount").on("input", function () {
        const oldVal = $(this).val();
        const oldCaret = this.selectionStart;
        const formatted = FormatCurrencyInput(oldVal);
        const delta = formatted.length - oldVal.length;
        $(this).val(formatted);
        this.setSelectionRange(Math.max(0, oldCaret + delta), Math.max(0, oldCaret + delta));
    });

    $("#btnSubmit").on("click", function () {
        if (ValidateInputs()) Submit();
    });

    $("#Amount").on("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            if (ValidateInputs()) Submit();
        }
    });
});
