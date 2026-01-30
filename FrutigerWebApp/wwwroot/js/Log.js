document.addEventListener("DOMContentLoaded", (event) => {
    let BackBtnLog = document.getElementById("BackBtnLog");
    if (BackBtnLog) {
        BackBtnLog.addEventListener('click', () => {
            window.location.href = "https://localhost:7159/User/Intro";
        });
    }
});