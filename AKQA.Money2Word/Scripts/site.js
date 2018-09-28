
function alphabetValidation(e)
{
    var regex = new RegExp("^[a-zA-Z ]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (regex.test(str)) {
        return true;
    }
    else {
        e.preventDefault();
    }
}

function currencyValidation(e) {
    var amount=$("#Amount").val();
    if (!isValidCurrency(amount))
    {
        e.preventDefault();
    }
    else {
        return true;
    }
    
}

function isValidCurrency(amount) {
    var regex = /^\d{1,3}(?:[.]\d{0,2})?$/;
    var result = amount.match(regex);
    if (!result) {
        return false;
    }
    else {
        return true;
    }
}

function isValidName(name) {
    var regex = new RegExp("^([a-zA-Z]+(([a-zA-Z ])?[a-zA-Z]*)){2,15}$");
    return regex.test(name);
}

function enbableButton(enable) {
    $("#btnSubmit").prop('disabled', !enable);
}

function validateInputs() {
    var amount = $("#Amount").val();
    var name = $("#Name").val();
    
    clearResponses() ;
    if (!name || !amount) {
        $("#responseError").text("Name or Amount have wrong value");
        return false;
    }
    if (!isValidCurrency(amount) || !isValidName(name)) {
       $("#responseError").text("Name or Amount have wrong value");
        return false;
    }
    return true;
}

function showResponse(data) {
    $("#resopnseAmount").html("<strong>Amount:</strong>" + data.Amount );
    $("#responseName").html("<strong>Name:</strong>" + data.Name );
    $("#responseError").text(data.ErrorMessage);
}

function clearResponses() {
    $("#resopnseAmount").text("");
    $("#responseName").text("");
    $("#responseError").text("");
}

function submit() {
    var obj = {
        Name:$("#Name").val().trim(),
        Amount:$("#Amount").val().trim()
    };
        $.ajax({
            url: "/api/default/get",
            type: "GET",
            context: this,
            data: obj,
            success: function(data) {
                showResponse(data);
            },
            error: function(data) {
                clearResponses();
                $("#responseError").text("Unknown error occured");
                console.log("error", data);
            }
        });
}

$(document).ready(function () {
    clearResponses();

    $("#Name").keypress(function (e) {
        alphabetValidation(e);
    });

    //$("#Amount, #Name").blur(function () {
    //    validateInputs();
    //});

    $("#Name, #Amount").bind('paste', function (e) {
        e.preventDefault();
    });

    $("#btnSubmit").on("click keypress", function(){
      if (!validateInputs()) {return;}
        submit();
    });

});