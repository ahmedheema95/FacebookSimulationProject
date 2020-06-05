$(document).ready(function () {
    $(".PostsContainer").on("click","button.btnviewcomments",function () {

        var button = $(this);

        //lets say entering data in textbox named txtname and clicking on button named btnadd

        var data = button.attr("data-user-id");
        $.ajax({
            url: "/User/ViewAllComments",//assume action name is 'addItem' with paramater 'dataparam' of type string in same controller
            dataType: "html",
            data: { postID: data },
            success: function (result) {
                $('.com').html(result);// your partial view rendering with <tr></tr>, becuase we do not know home many nos td tr has at this timme
            },
        })
    });
    /////////////////////////likes

    $(".PostsContainer").on("click", "button.btnviewlikes",function () {

        var button = $(this);

        //lets say entering data in textbox named txtname and clicking on button named btnadd

        var data = button.attr("data-user-id");
        $.ajax({
            url: "/User/Viewalllik",//assume action name is 'addItem' with paramater 'dataparam' of type string in same controller
            dataType: "html",
            data: { postID: data },
            success: function (result) {
                $('.lik').html(result);// your partial view rendering with <tr></tr>, becuase we do not know home many nos td tr has at this timme
            }
        });
    });


    $('.PostsContainer').on("click", "button.js-Like", function () {
        //console.log("mllmmllmmdc");

        var button = $(this);

        $.ajax({
            url: "/user/AddLike/?post_id=" + button.attr("data-user-id"),
            method: "post",
            success: function (response) {
                //alert("I am here");
                if (response == "Liked")
                    button.removeClass("btn btn-light").addClass("btn btn-primary").text("UnLike");
                else if (response == "UnLiked")
                    button.removeClass("btn btn-primary").addClass("btn btn-light").text("Like");
                else
                    alert("Session Ended Please Login Again !!!");
            },
            error: () => alert("Sorry An Error Happens"),
        })

    })

    $(".PostsContainer").on("click","button.jDelete", function () {
        //console.log("mllmmllmmdc");

        var button = $(this);

        $.ajax({
            url: "/User/DeletePost/?post_id=" + button.attr("data-user-id"),
            method: "post",
            dataType:"json",
            success: function (data) {
                if (data == "Deleted") {
                    //alert("Post Successfully Deleted");
                    button.parent("div").parent("div").remove();
                    //button.parent("td").parent("tr").remove();
                }
                else
                    alert("Session Ended Please Login Again !!!");
            },
            error: () => alert("Sorry Something Wrong Happened"),
        })
    })



    $(".btncreatepost").on("click", function () {
        var valdata = $("#posttext").val();
        //alert(valdata);
        $.ajax({
            url: "/User/CreatePost/?newPostText=" + valdata,
            dataType: "html",
            method: "post",
            success: function (response) {
                //alert(response);
                //alert("Successful Added");
                $("#posttext").val('');
                $('.btncreatepost').attr("hidden", true);
                $('.PostsContainer').prepend(response);
                
            },
            error: () => alert("Sorry Something Error Happened!!"),
        })

    })

    /****************************************************************************************************************************/
    $('.PostsContainer').on("click","button.jPostEdit", function () {
        var button = $(this);
        $.ajax({
            url: "/User/EditPost/?post_id=" + button.attr("data-user-id"),
            method: "get",
            success: function (result) {
                $('.editmodalbody').html(result);
            },
            error: () => alert("Sorry An Error Happens !!"),
        })
    })


    $('.PostsContainer').on("click","button.btnaddcomment", function () {
        var button = $(this);
        var data = button.attr("post-id");
        $.ajax({
            url: '/User/AddComment',
            dataType: "html",
            data: { postID: data },
            success: function (result) {
                //alert(result)
                $('.addcommentbody').html(result);
            }
        });
    })

    $('#posttext').keyup(function () {

        if ($('#posttext').val().length > 0)
            $('.btncreatepost').removeAttr("hidden");
        else
            $('.btncreatepost').attr("hidden",true);
    })


    /***********************************************************************************************/

    $("#FriendShipState").on("click","button.js-RemoveFriend", function () {
        var button = $(this);
        console.log("button clicked")
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

    $("#FriendShipState").on("click","button.js-AddFriend", function () {
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
                        console.log(button)
                    }
                    else
                        alert("Request Not Deleted");

                },
                error: () => alert('Sorry An Error Occurred!!'),
            })
        }
    })

    $("#FriendShipState").on("click","button.js-AcceptRequest", function () {
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
                        $('#FriendShipState').find('.js-DeclineRequest').remove();
                        $('#FriendShipState').find('hr').remove();
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

    $("#FriendShipState").on("click","button.js-DeclineRequest", function () {
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
                        $('#FriendShipState').find('.js-AcceptRequest').remove();
                        $('#FriendShipState').find('hr').remove();
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

    $("#FriendShipState").on("click","button.js-DeleteRequest", function () {
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
                        console.log(button)
                    }
                    else
                        alert("Sent FriendRequest Not Deleted");

                },
                error: () => alert('Sorry An Error Occurred!!'),
            })
        }
    })



    $(".showAllFriends").on("click", function () {

        var button = $(this);

        $.ajax({
            url: "/User/ViewAllFriends/?UserID=" + button.attr("data-user-id"),
            dataType: "html",
            type: "post",
            success: function (result) {
                $('.ShowFriends').html(result);
            },
            error: () => alert("Sorry An Error Happens"),
        })
    })



    $(".showAllRequests").on("click", function () {

        var button = $(this);

        $.ajax({
            url: "/User/FriendRequests/?UserID=" + button.attr("data-user-id"),
            dataType: "html",
            type:"post",
            success: function (result) {
                $('.ShowRequests').html(result);
            },
            error: () => alert("Sorry An Error Happens"),
        })
    })







});