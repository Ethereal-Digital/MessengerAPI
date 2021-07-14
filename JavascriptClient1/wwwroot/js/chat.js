﻿
document.addEventListener("DOMContentLoaded", () => {
   
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(chatHubURL)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveMessage", (message) => {
        console.log("Received message");
       
        if (message.messageTypeId == 1) {
            const li = document.createElement("li");
            console.log(message);
            if (message.senderUID == userID) {
                li.textContent = `me to All: ${message.messageBody}`;
            }
            else {
                li.textContent = `${message.sender}: ${message.messageBody}`;
            }
            document.getElementById("messageList").appendChild(li);
        }
        else if (message.messageTypeId == 2)
        {
            const li = document.createElement("li");
            const element = document.createElement('a');
            const messageList = document.getElementById("messageList");
    
            element.setAttribute('href', ApiBaseURL + "/chat/downloadFile?fileName=" + message.messageBody);
            element.setAttribute('download', message.messageBody);

            element.textContent = message.messageBody;
            li.textContent = `me to All: `;
            li.append(element);
            messageList.appendChild(li);
        }
       
    });
 
    connection.on("ReceivePrivateMessage", (message) => {
        const li = document.createElement("li");
        if (message.receiver == null)
        {
            li.textContent = `${message.messageBody}`;
        }
        else if (message.senderUID == userID)
        {
            li.textContent = `me to ${message.receiver}: ${message.messageBody}`;
        }
        else
        {
            li.textContent = `(Private) ${message.sender}: ${message.messageBody}`;
        }
        document.getElementById("messageList").appendChild(li);
    });
  
    connection.on("GroupReceiveMessage", (groupMessage) => {
        const li = document.createElement("li");
        if (groupMessage.senderUID == userID) 
        {
            li.textContent = `(${groupMessage.roomName}) me: ${groupMessage.messageBody}`;
        }
        else
        {
            li.textContent = `(${groupMessage.roomName}) ${groupMessage.sender}: ${groupMessage.messageBody}`;
        }
        document.getElementById("groupMessageList").appendChild(li);
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
                    "UID": "MUID" + randomNumber,
                    "Sender": user,
                    "SenderUID": id,
                    "Receiver": null,
                    "ReceiverUID": receiverId,
                    "MessageTypeId": 1,
                    "MessageBody": message,
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

            GroupMessageClass = JSON.stringify(
                {
                    "UID": "MUID" + randomNumber,
                    "Sender": user,
                    "SenderUID": id,
                    "MessageTypeId": null,
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

        const id = userID;
        const receiverId = document.getElementById("receiver").value;

        try {

            UserMessageHistory = JSON.stringify(
                {
                    "SenderUID": id,
                    "ReceiverUID": receiverId
                });

            GetMessageHistory().then(response =>
            {
                var dataArray = JSON.parse(response);

                for (let i = 0; i < dataArray.length; i++) {
                    
                    const li = document.createElement("li");

                    if (id == dataArray[i].senderUID) {
                        li.textContent = `me: ${dataArray[i].messageBody}`;
                    }
                    else {
                        if (dataArray[i].receiverUID == "All") {
                            li.textContent = `${dataArray[i].senderUID}: ${dataArray[i].messageBody}`;
                        } else {
                            li.textContent = `${dataArray[i].messageBody}`;
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

    });

    document.getElementById("groupName").addEventListener("change", function (e) {

        var groupMessageList = document.getElementById("groupMessageList");
        groupMessageList.innerHTML = '';

        const id = userID;
        const groupName = document.getElementById("groupName").value;

        try {

            UserGroupMessageHistory = JSON.stringify(
                {
                    "SenderUID": id,
                    "RoomName": groupName
                });

            GetGroupMessageHistory().then(response => {
                var dataArray = JSON.parse(response);

                for (let i = 0; i < dataArray.length; i++) {

                    const li = document.createElement("li");

                    if (id == dataArray[i].senderUID) {
                        li.textContent = `me: ${dataArray[i].messageBody}`;
                    }
                    else {
                         li.textContent = `${dataArray[i].senderUID}: ${dataArray[i].messageBody}`;
                    }
                    document.getElementById("groupMessageList").appendChild(li);
                }

                console.log("retrieved group message history");
            });

        } catch (err) {
            console.log("get group message history error");
            console.error(err);
        }

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


