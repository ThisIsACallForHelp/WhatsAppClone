document.addEventListener("DOMContentLoaded", (event) => {
    let LogInBtn = document.getElementById("LogIn")
    let SignUp = document.getElementById("SignUp")
    
    if (LogInBtn) {
        LogInBtn.addEventListener('click', () => {
            window.location.href = "https://localhost:7159/User/SignInViaQR";
        }); 
    }

    if (SignUp) {
        SignUp.addEventListener('click', () => {
            window.location.href = "https://localhost:7159/User/Register";
        });
    }
});