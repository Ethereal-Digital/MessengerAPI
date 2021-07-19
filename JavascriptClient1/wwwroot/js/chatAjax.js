
//let ApiBaseURL = "https://localhost:44364";
//let chatHubURL = "https://localhost:44364/ChatHub";
let ApiBaseURL = "https://cece116c57cc.ngrok.io";
let chatHubURL = "https://cece116c57cc.ngrok.io/ChatHub";
let MessageClass;
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

function GetMessageHistory() {
    return new Promise((resolve, reject) => {

        $.ajax({
            url: ApiBaseURL + '/chat/GetMessageHistory',
            startTime: performance.now(),
            method: 'POST',
            data: UserMessageHistory,
            contentType: "application/json",
            success: function (data) {
                var time = performance.now() - this.startTime;
                var result = 'AJAX: ' + time + ' milliseconds.';
                console.log(result);
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
            startTime: performance.now(),
            method: 'POST',
            data: UserGroupMessageHistory,
            contentType: "application/json",
            success: function (data) {
                var time = performance.now() - this.startTime;
                var result = 'AJAX: ' + time + ' milliseconds.';
                console.log(result);
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