document.addEventListener("DOMContentLoaded", (event) => {
    let UserName;
    let Message;
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/GetChats").build();

    connection.on("ReceiveMessage", (user, message) => {
        console.log(user + ": " + message);
    });

    connection.start().then(() => {
        connection.invoke("SendMessage", "Nathaniel", "Hello SignalR!");
    });
    //need to get the user data and the message
});