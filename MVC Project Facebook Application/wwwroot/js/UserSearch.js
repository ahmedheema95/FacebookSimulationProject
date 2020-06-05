/*****************************************************************************************/
/******************************Search Page JS Methods*************************************/
$(document).ready(function () {
    $("table").on("click", "button.js-RemoveFriend", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to Remove this Friend")) {
            $.ajax({
                url: "/User/RemoveFriend/?FriendID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    if (response == "FriendRemoved") {
                        //console.log(response);
                        //alert("Friend Successfully Removed");
                        button.removeClass("btn alert-danger js-RemoveFriend")
                            .addClass("btn alert-primary js-AddFriend").text("Add Friend");
                    }
                    else
                        alert("Friend Not Removed");
                },
                error: () => alert("Sorry An Error Occurred!!"),
            })
        }
    })

    $("table").on("click", "button.js-AddFriend", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to Add this Friend")) {
            $.ajax({
                url: "/User/AddFriend/?FriendID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    //console.log(response);
                    if (response == "FriendAdded") {
                        //alert("Friend Request Sent");
                        button.removeClass("btn alert-primary js-AddFriend")
                            .addClass("btn alert-secondary js-DeleteRequest").text("Delete Request");
                    }
                    else
                        alert("Request Not Deleted");

                },
                error: () => alert('Sorry An Error Occurred!!'),
            })
        }
    })

    $("table").on("click", "button.js-AcceptRequest", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to Accept this Friend Request")) {
            $.ajax({
                url: "/User/AcceptFriendRequest/?FriendID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    //console.log(response);
                    if (response == "RequestAccepted") {
                        //alert("You Are Now Friends");
                        button.parent('td').find('.js-DeclineRequest').remove();
                        button.parent('td').find('hr').remove();
                        button.removeClass("btn alert-success js-AcceptRequest")
                            .addClass("btn alert-danger js-RemoveFriend").text("UnFriend");
                    }
                    else
                        alert("FriendRequest Not Accepted");

                },
                error: () => alert('Sorry An Error Occurred!!'),
            })
        }
    })

    $("table").on("click", "button.js-DeclineRequest", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to Decline this Friend Request")) {
            $.ajax({
                url: "/User/DeclineFriendRequest/?FriendID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    //console.log(response);
                    if (response == "RequestDeclined") {
                        //alert("Friend Request Declined");
                        button.parent('td').find('.js-AcceptRequest').remove();
                        button.parent('td').find('hr').remove();
                        button.removeClass("btn alert-warning js-DeclineRequest")
                            .addClass("btn alert-primary js-AddFriend").text("Add Friend");
                    }
                    else
                        alert("Request Not Declined");

                },
                error: () => alert('Sorry An Error Occurred!!'),
            })
        }
    })

    $("table").on("click", "button.js-DeleteRequest", function () {
        var button = $(this);
        //console.log("button clicked")
        if (confirm("Are you Sure You want to Delete this Friend Request")) {
            $.ajax({
                url: "/User/DeleteFriendRequest/?FriendID=" + button.attr("data-user-id"),
                method: "post",
                success: function (response) {
                    //console.log(response);
                    if (response == "RequestDeleted") {
                        //alert("Sent FriendRequest Deleted");
                        button.removeClass("btn alert-secondary js-DeleteRequest")
                            .addClass("btn alert-primary js-AddFriend").text("Add Friend");
                    }
                    else
                        alert("Sent FriendRequest Not Deleted");

                },
                error: () => alert('Sorry An Error Occurred!!'),
            })
        }
    })

});