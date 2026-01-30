document.addEventListener("DOMContentLoaded", (event) => {
    let BackBtnReg = document.getElementById("BackBtnReg");
    if (BackBtnReg) {
        BackBtnReg.addEventListener('click', () => {
            window.location.href = "https://localhost:7159/User/Intro";
        });
    }
});