$(document).ready(function () {
    $("#btnsubmit").click(function (e) { 
        var valdata = $("#userform").serialize();
        //console.log(valdata)
        //$('#CreateModal').modal('toggle');
        $.ajax({
            url: '/Admin/CreateUser/',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: valdata,
            success: function (response) {
                if (response != null) {
                    alert("User Added Successfully");
                    $('.createUserClose').click();
                }
                else
                    alert("User Creation Failed");
            },
            error: () => alert("Sorry An Error Occurred!!"),

        })
    })

    $('#adminSearchArea').keyup(() => {
        $.ajax({
            url: '/User/SearchResult/',
            dataType: 'json',
            type: 'post',
            success: function (data) {
                //console.log(data);
                adminSearch(data);
            },
            error: () => alert("Error In Loading Data"),
        })
    })
    function adminSearch(data) {
        var i = 0, appendedIDs = [];
        for (; i < data.length; i++) {
            //console.log(data[i].blocked)
            if ((data[i].userName.toLowerCase().startsWith($('#adminSearchArea').val().toLowerCase()))
                && ($('#adminSearchArea').val() != "")) {
                appendedIDs.push(data[i]);
            }
        }
        $('table').hide();
        $('#SearchResults').empty();
        if (appendedIDs.length)
            $('table').show();
        //console.log(appendedIDs);
        i = 0;
        var buttonChangeRole;
        for (; i < appendedIDs.length; i++) {
            if (appendedIDs[i].blocked == false)
                appendedIDs[i].buttonType = '<button data-user-id="' + appendedIDs[i].id + '"class="btn alert-danger js-Block">Block</button>';
            else
                appendedIDs[i].buttonType = '<button data-user-id="' + appendedIDs[i].id + '"class="btn alert-warning js-UnBlock">UnBlock</button>';


            buttonChangeRole = '<button data-user-id="' + appendedIDs[i].id + '" type="button" class="btn btn-secondary ChangeRole" data-toggle="modal" data-target="#ChangeRoleModal">Change Role</button >'
                
                            

            $("#SearchResults").append('<tr><td>' + appendedIDs[i].userName + '</td><td>' +
                appendedIDs[i].fullName + '</td><td>' + appendedIDs[i].buttonType +
                '</td><td>' + appendedIDs[i].roleName + '</td><td>' + buttonChangeRole + '</td></tr>');
                
        }

    }

    $("table").on("click","button.js-Block", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to block this User")) {
            $.ajax({
                url: "/Admin/BlockUser/?BlockedUserID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    if (response == "Blocked") {
                        //console.log(response);
                        //alert("Successful Blocked");
                        button.removeClass("btn alert-danger js-Block")
                            .addClass("btn alert-warning js-UnBlock").text("Unblock");
                    } 
                    else
                        alert("User Not Blocked");
                },
                error: () => alert("Block Error"),
            })
        }
    })

    $("table").on("click", "button.js-UnBlock", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to Unblock this User")) {
            $.ajax({
                url: "/Admin/UnBlockUser/?BlockedUserID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    //console.log(response);
                    if (response == "UnBlocked") {
                        //alert("Successful UnBlocked");
                        button.removeClass("btn alert-warning js-UnBlock")
                            .addClass("btn alert-danger js-Block").text("Block");
                    }
                    else
                        alert("User Not UnBlocked");

                },
                error: () => alert('Unblock Error'), 
            })
        }
    })

    $("#btnsubmit").attr("disabled", true);

    $('.formInput').keyup(function () {
        if (($('.fnameInput').val().length != 0) &&
            ($('.lnameInput').val().length != 0) &&
            ($('.ageInput').val().length != 0) &&
            (validateEmail($('.emailInput').val())) &&
            ($('.countryInput').val().length != 0) &&
            ($('.passwordInput').val().length >5) &&
            ($('.confirmPassInput').val().length > 5) &&
            ($('.passwordInput').val() == $('.confirmPassInput').val()))
            $("#btnsubmit").removeAttr("disabled");
        else
            $("#btnsubmit").attr("disabled", true);
    })

    function validateEmail(email) {
        var regex = /\S+@\S/;
        return regex.test(email);
    }

    $('#AddRole').on("click", function () {
        console.log("qqq")
        $.ajax({
            url: '/Admin/CreateRole/',
            dataType: "html",
            method: "get",
            success: function (response) {
                $('.role-body').html(response);
            },
            error: () => alert("Something Wrong Happened"),
        })
    })


    $("table").on("click", "button.ChangeRole", function () {
        var button = $(this);
        console.log("button change role clicked")
        console.log(button.attr("data-user-id"));
        $.ajax({
            url: '/Admin/AssignRule/?UserID=' + button.attr("data-user-id"),
            dataType: "html",
            method: "get",
            success: function (response) {
                $('.change-role-body').html(response);
            },
            error: () => alert("Something Wrong Happened"),
        })
    })




});
