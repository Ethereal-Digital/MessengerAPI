
//let ApiBaseURL = "https://localhost:44364";
//let chatHubURL = "https://localhost:44364/ChatHub";
let ApiBaseURL = "https://953cad0ac598.ngrok.io";
let chatHubURL = "https://953cad0ac598.ngrok.io/ChatHub";
let MessageClass;
let GroupMessageClass;
let UserConnectionInfo;
let JoinGroupJson;
let ExitGroupJson;
let UserMessageHistory;
let UserGroupMessageHistory;

function UpdateUserConnection() {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: ApiBaseURL + '/chat/UpdateUsersHubConnection',
            method: 'POST',
            data: UserConnectionInfo,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {

                reject(data);
            }
        });
    })
};

function SendMessage() {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: ApiBaseURL + '/chat/SendMessage',
            method: 'POST',
            data: MessageClass,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};

function SendMessageToAll() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/SendMessageToAll',
            method: 'POST',
            data: MessageClass,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};

function SendMessageToGroup() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/SendMessageToRoom',
            method: 'POST',
            data: GroupMessageClass,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};

function GetMessageHistory() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/GetMessageHistory',
            method: 'POST',
            data: UserMessageHistory,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};

function GetGroupMessageHistory() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/GetGroupMessageHistory',
            method: 'POST',
            data: UserGroupMessageHistory,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};

function JoinGroup() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/JoinRoom',
            method: 'POST',
            data: JoinGroupJson,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};

function ExitGroup() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/ExitRoom',
            method: 'POST',
            data: ExitGroupJson,
            contentType: "application/json",
            success: function (data) {

                var jsonData = JSON.stringify(data);
                resolve(jsonData);
            },
            error: function (data) {
                reject(data);
            }
        });
    })
};