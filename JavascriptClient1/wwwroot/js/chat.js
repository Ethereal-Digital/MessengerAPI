
var messageCounter = 0;
var groupMessageCounter = 0;

document.addEventListener("DOMContentLoaded", () => {
   
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(chatHubURL)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("PublicMessage", (message) => {
        console.log("Received message");
        const li = document.createElement("li");
        const element = document.createElement('a');
        const messageList = document.getElementById("messageList");

        if (message.messageTypeId == 1) {
            if (message.senderUID == userID) {
                li.textContent = `me to All: ${message.messageBody}`;
            }
            else {
                li.textContent = `${message.sender}: ${message.messageBody}`;
            }
        }
        else if (message.messageTypeId == 2)
        {
            element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + message.uid);
            element.setAttribute('download', message.messageBody);
            element.textContent = message.messageBody;

            li.textContent = `me to All: `;
            li.append(element);
        }
        messageList.appendChild(li);
    });
 
    connection.on("PrivateMessage", (message) => {
        const li = document.createElement("li");
        const element = document.createElement('a');
        const messageList = document.getElementById("messageList");

        if (message.messageTypeId == 1) {
            if (message.receiverUID == null) {
                li.textContent = `${message.messageBody}`;
            }
            else if (message.senderUID == userID) {
                li.textContent = `me to ${message.receiverUID}: ${message.messageBody}`;
            }
            else {
                li.textContent = `(Private) ${message.sender}: ${message.messageBody}`;
            }
        }
        else if (message.messageTypeId == 2) {
            element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + message.uid);
            element.setAttribute('download', message.messageBody);
            element.textContent = message.messageBody;

            if (message.receiverUID == null) {
                li.textContent = `${message.messageBody}`;
            }
            else if (message.senderUID == userID) {
                li.textContent = `me to ${message.receiverUID}: `;
                li.append(element);
            }
            else {
                li.textContent = `(Private) ${message.sender}: `;
                li.append(element);
            }
        }
        messageList.appendChild(li);
    });
  
    connection.on("RoomMessage", (groupMessage) => {
        const li = document.createElement("li");
        const element = document.createElement('a');
        const messageList = document.getElementById("groupMessageList");

        if (groupMessage.messageTypeId == 1) {
            if (groupMessage.senderUID == userID) {
                li.textContent = `(${groupMessage.roomName}) me: ${groupMessage.messageBody}`;
            }
            else {
                li.textContent = `(${groupMessage.roomName}) ${groupMessage.sender}: ${groupMessage.messageBody}`;
            }
         
        }
        else if (groupMessage.messageTypeId == 2) {
            element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + groupMessage.uid);
            element.setAttribute('download', groupMessage.messageBody);
            element.textContent = groupMessage.messageBody;

            if (groupMessage.senderUID == userID) {
                li.textContent = `(${groupMessage.roomName}) me: `;
                li.append(element);
            }
            else {
                li.textContent = `(${groupMessage.roomName}) ${groupMessage.sender}: `;
                li.append(element);
            }
        }
        messageList.appendChild(li);

    });

    document.getElementById("sendFile").addEventListener("click", async () => {
        console.log("Sending file");
        if (window.FormData !== undefined) {

            var fileData = new FormData();
            const user = userName;
            const id = userID;
            const receiverId = document.getElementById("receiver").value;
            const progress = document.getElementById("progress");
            const today = new Date(Date.now());
            const tmpDate = today.toISOString();
            const randomNumber = Math.floor(Math.random() * 20000);

            var files = document.getElementById("myFile").files;
            var fileVar = files[0];

            console.log("Compiling message");

            MessageClass = JSON.stringify(
                {
                    "UID": "MUID" + randomNumber,
                    "Sender": user,
                    "SenderUID": id,
                    "Receiver": null,
                    "ReceiverUID": receiverId,
                    "MessageTypeId": 2,
                    "MessageBody": fileVar.name,
                    "RoomName": null,
                    "CreatedDate": tmpDate
                });

            fileData.append('File', fileVar);
            fileData.append('MessageInfo', MessageClass);

            console.log("Uploading to server");

            if (receiverId == "All") {
                $.ajax({
                    xhr: function () {
                        var xhr = new window.XMLHttpRequest();
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                var percentComplete = (evt.loaded / evt.total) * 100;
                                console.log("upload: " + percentComplete);
                                progress.setAttribute("style", 'width:' + Math.floor(percentComplete) + '%');
                            }
                        }, false);
                        return xhr;
                    },
                    url: ApiBaseURL + '/chat/SendFileToAll',
                    type: "POST",
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: fileData,
                    success: function (result) {
                        console.log("upload success");
                        progress.setAttribute("style", 'width:' + 0 + '%');
                    },
                    error: function (err) {
                        console.log("upload fail");

                    }
                });
            }
            else {
                $.ajax({
                    xhr: function () {
                        var xhr = new window.XMLHttpRequest();
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                var percentComplete = (evt.loaded / evt.total) * 100;
                                console.log("upload: " + percentComplete);
                                progress.setAttribute("style", 'width:' + Math.floor(percentComplete) + '%');
                            }
                        }, false);
                        return xhr;
                    },
                    url: ApiBaseURL + '/chat/SendFile',
                    type: "POST",
                    contentType: false, // Not to set any content header  
                    processData: false, // Not to process data  
                    data: fileData,
                    success: function (result) {
                        console.log("upload success");
                        progress.setAttribute("style", 'width:' + 0 + '%');
                    },
                    error: function (err) {
                        console.log("upload fail");

                    }
                });
            }

            
        } else {
            console.log("FormData is not supported.");
        }  

    });

    document.getElementById("sendFileToGroup").addEventListener("click", async () => {
        console.log("Sending file to group");
        if (window.FormData !== undefined) {

            var fileData = new FormData();
            const user = userName;
            const id = userID;
            
            const group = document.getElementById("groupName").value;
            const progress = document.getElementById("progressGroup");
            const today = new Date(Date.now());
            const tmpDate = today.toISOString();

            var files = document.getElementById("groupFile").files;
            var fileVar = files[0];

            console.log("Compiling message");

            MessageClass = JSON.stringify(
                {
                    "UID": "",
                    "Sender": user,
                    "SenderUID": id,
                    "Receiver": null,
                    "ReceiverUID": null,
                    "MessageTypeId": 2,
                    "MessageBody": fileVar.name,
                    "RoomName": group,
                    "CreatedDate": tmpDate
                });

            fileData.append('File', fileVar);
            fileData.append('MessageInfo', MessageClass);

            console.log("Uploading to server");

            $.ajax({
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();
                    xhr.upload.addEventListener("progress", function (evt) {
                        if (evt.lengthComputable) {
                            var percentComplete = (evt.loaded / evt.total) * 100;
                            console.log("upload: " + percentComplete);
                            progress.setAttribute("style", 'width:' + Math.floor(percentComplete) + '%');
                        }
                    }, false);
                    return xhr;
                },
                url: ApiBaseURL + '/chat/SendFileToRoom',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: fileData,
                success: function (result) {
                    console.log("upload success");
                    progress.setAttribute("style", 'width:' + 0 + '%');
                },
                error: function (err) {
                    console.log("upload fail");

                }
            });
            
        } else {
            console.log("FormData is not supported.");
        }

    });

    document.getElementById("send").addEventListener("click", async () => {
     
        const user = userName;
        const id = userID;

        const receiverId = document.getElementById("receiver").value;
        const message = document.getElementById("message").value;
   
        const today = new Date(Date.now());
        const tmpDate = today.toISOString();
   
        const randomNumber = Math.floor(Math.random() * 20000);

        try {

            MessageClass = JSON.stringify(
                {
                    "UID": null,
                    "Sender": user,
                    "SenderUID": id,
                    "Receiver": null,
                    "ReceiverUID": receiverId,
                    "MessageTypeId": 1,
                    "MessageBody": message,
                    "RoomName": null,
                    "CreatedDate": tmpDate
                });

            if (receiverId == "All") {
                SendMessageToAll().then(response => { });
            }
            else {
                SendMessage().then(response => { });
            }

        } catch (err) {
            console.log("send message error");
            console.error(err);
        }
       
    });

    document.getElementById("groupSend").addEventListener("click", async () => {
        const user = userName;
        const id = userID;
        const group = document.getElementById("groupName").value;
        const message = document.getElementById("groupMessage").value;

        const today = new Date(Date.now());
        const tmpDate = today.toISOString();

        const randomNumber = Math.floor(Math.random() * 20000);

        try {

            MessageClass = JSON.stringify(
                {
                    "UID": "",
                    "Sender": user,
                    "SenderUID": id,
                    "Receiver": null,
                    "ReceiverUID": null,
                    "MessageTypeId": 1,
                    "MessageBody": message,
                    "RoomName": group,
                    "CreatedDate": tmpDate
                });

            SendMessageToGroup().then(response => { });

        } catch (err) {
            console.log("send group message error");
            console.error(err);
        }

    });

    document.getElementById("joinGroup").addEventListener("click", async () => {
        const user = userName;
        const id = userID;
        const group = document.getElementById("groupName").value;
        
        try {

            JoinGroupJson = JSON.stringify(
                {
                    "UserUID": id,
                    "RoomName": group
                });
    
            JoinGroup().then(response => { });
            console.log("Joined group " + group);

        } catch (err) {
            console.log("Join group error");
            console.error(err);
        }
    });

    document.getElementById("exitGroup").addEventListener("click", async () => {
        const user = userName;
        const id = userID;
        const group = document.getElementById("groupName").value;
        
        try {

            ExitGroupJson = JSON.stringify(
                {
                    "UserUID": id,
                    "RoomName": group
                });

            ExitGroup().then(response => { });
            console.log("Exited group " + group);

        } catch (err) {
            console.log("Exit group error");
            console.error(err);
        }
    });

    document.getElementById("receiver").addEventListener("change", function (e) {

        var messageList = document.getElementById("messageList");
        messageList.innerHTML = '';
        messageCounter = 0;

        getMessageHistoryFunc();

    });

    document.getElementById("LoadMoreMessage").addEventListener("click", function (e) {

        messageCounter = messageCounter + 1;
        getMessageHistoryFunc();

    });

    document.getElementById("groupName").addEventListener("change", function (e) {

        var groupMessageList = document.getElementById("groupMessageList");
        groupMessageList.innerHTML = '';
        groupMessageCounter = 0;

        getGroupMessageHistoryFunc();

    });

    document.getElementById("LoadMoreGroupMessage").addEventListener("click", function (e) {

        groupMessageCounter = groupMessageCounter + 1;
        getGroupMessageHistoryFunc();

    });

  

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");

            UserConnectionInfo = JSON.stringify(
                {
                    "ConnectionId": connection.connectionId,
                    "UserUID": userID,
                    "UserName": userName
                });

            UpdateUserConnection().then(response => { });
            
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };

    connection.onclose(start);

    start();

    
});

function getMessageHistoryFunc() {

    const id = userID;
    const receiverId = document.getElementById("receiver").value;

    try {

        UserMessageHistory = JSON.stringify(
            {
                "ItemSize": 5,
                "Counter": messageCounter,
                "SenderUID": id,
                "ReceiverUID": receiverId,
                "RoomName": null
            });

        GetMessageHistory().then(response => {
            var dataArray = JSON.parse(response);

            for (let i = 0; i < dataArray.length; i++) {
                const li = document.createElement("li");
                const element = document.createElement('a');
       
                if (id == dataArray[i].senderUID) {
                    if (dataArray[i].messageTypeId == 1) {
                        li.textContent = `me: ${dataArray[i].messageBody}`;
                    }
                    else if (dataArray[i].messageTypeId == 2) {
                        element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + dataArray[i].uid);
                        element.setAttribute('download', dataArray[i].messageBody);
                        element.textContent = dataArray[i].messageBody;

                        li.textContent = `me: `;
                        li.append(element);
                    }
                }
                else {
                    if (dataArray[i].receiverUID == "All") {

                        if (dataArray[i].messageTypeId == 1) {
                            li.textContent = `${dataArray[i].senderUID}: ${dataArray[i].messageBody}`;
                        }
                        else if (dataArray[i].messageTypeId == 2) {
                            element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + dataArray[i].uid);
                            element.setAttribute('download', dataArray[i].messageBody);
                            element.textContent = dataArray[i].messageBody;

                            li.textContent = `${dataArray[i].senderUID}: `;
                            li.append(element);
                        }
                       
                    } else {
                        if (dataArray[i].messageTypeId == 1) {
                            li.textContent = `${dataArray[i].messageBody}`;
                        }
                        else if (dataArray[i].messageTypeId == 2) {
                            element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + dataArray[i].uid);
                            element.setAttribute('download', dataArray[i].messageBody);
                            element.textContent = dataArray[i].messageBody;

                            li.append(element);
                        }
                    }

                }
                document.getElementById("messageList").appendChild(li);
            }
            console.log("retrieved message history");


        });

    } catch (err) {
        console.log("get message history error");
        console.error(err);
    }
}

function getGroupMessageHistoryFunc() {

    const id = userID;
    const groupName = document.getElementById("groupName").value;

    try {

        UserGroupMessageHistory = JSON.stringify(
            {
                "ItemSize": 5,
                "Counter": groupMessageCounter,
                "SenderUID": id,
                "ReceiverUID": null,
                "RoomName": groupName
            });

        GetGroupMessageHistory().then(response => {
            var dataArray = JSON.parse(response);

            for (let i = 0; i < dataArray.length; i++) {

                const li = document.createElement("li");
                const element = document.createElement('a');
                const messageList = document.getElementById("groupMessageList");

                console.log(dataArray[i]);

                if (dataArray[i].messageTypeId == 1) {
                    if (id == dataArray[i].senderUID) {
                        li.textContent = `me: ${dataArray[i].messageBody}`;
                    }
                    else {
                        li.textContent = `${dataArray[i].senderUID}: ${dataArray[i].messageBody}`;
                    }
                }
                else if (dataArray[i].messageTypeId == 2) {
                    element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?attachment_id=" + dataArray[i].uid);
                    element.setAttribute('download', dataArray[i].messageBody);
                    element.textContent = dataArray[i].messageBody;

                    if (dataArray[i].senderUID == userID) {
                        li.textContent = `(${dataArray[i].roomName}) me: `;
                        li.append(element);
                    }
                    else {
                        li.textContent = `(${dataArray[i].roomName}) ${dataArray[i].senderUID}: `;
                        li.append(element);
                    }
                }

                messageList.appendChild(li);
            }

            console.log("retrieved group message history");
        });

    } catch (err) {
        console.log("get group message history error");
        console.error(err);
    }
}
