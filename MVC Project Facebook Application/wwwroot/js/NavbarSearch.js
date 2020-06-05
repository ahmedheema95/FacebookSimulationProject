/*********************************************************************************************/
/*****************************Navbar Search JS Methods****************************************/
var filteredIDs = [];
$(document).ready(function () {

    $('#searchArea').keyup(() => {
        $.ajax({
            url: '/User/SearchResult/',
            dataType: 'json',
            type: 'post',
            success: function (data) {
                //console.log(data);
                search(data);
            }
        })
    })

});

function search(data) {
    //console.log(data);
    var appenddedIndexes = [], j = 0;
    filteredIDs = [];
    for (var i = 0; i < data.length; i++) {
        //console.log(data[i].userName);
        if ($('#searchArea').val().toLowerCase() != "" && (((data[i].userName.startsWith($('#searchArea').val().toLowerCase())))
            || (data[i].fullName.toLowerCase().startsWith($('#searchArea').val().toLowerCase())))) {
            appenddedIndexes[j] = i;
            filteredIDs[j] = data[i].id;
            j++;
        }
    }
    $('#result').empty();
    //console.log("Length : " + appenddedIndexes.length);
    if (!appenddedIndexes.length)
        $('#result').append('<li class="list-group-item" >' + "No Search Results Found !!" + '</li>');
    for (var i = 0; i < appenddedIndexes.length; i++) {
        //console.log(appenddedIndexes[i])
        $('#result').append('<li class="list-group-item" >' + '<a href =/User/Profile/?ProfileID=' + data[appenddedIndexes[i]].id + '\>' + data[appenddedIndexes[i]].fullName + '</a>' + '</li>');
    }

}

function closeSearchBox() {
    $('#result').empty();
}
function searchClick() {
    if ($('#searchArea').val() == '')
        alert("Please Enter a Text In a Search Box");
    else if (filteredIDs == '')
        alert("No Search Results Found!!");
    else
        window.location.href = "/User/Search/?querySearch=" + $('#searchArea').val();
}