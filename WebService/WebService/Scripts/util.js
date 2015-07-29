$(function () {
    var chat = $.connection.chatHub;
    chat.client.addMessage = function (message) {
        $('#chatroom').append("<p><b>" + "</b>: " + htmlEncode(message) + "</p>");
    };

    $.connection.hub.start().done(function () {

        $('#sendmessage').click(function () {
            chat.server.send($('#message').val());
            $('#message').val('');
        });
    });
});

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}