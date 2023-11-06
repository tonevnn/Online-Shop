function Login() {
    var email = $("#email").val();
    var pass = $("#pass").val();
    if (email == "" || pass == "") {
        $("#error-email").attr("hidden", email != "");
        $("#error-pass").attr("hidden", pass != "");
    }
    else {
        const account = {
            "email": email,
            "password": pass
        };
        let body = JSON.stringify(account);
        $.ajax({
            type: "POST",
            url: 'http://localhost:5024/api/login',
            data: body,
            contentType: "application/json",
            dataType: 'text',
            success: function (result) {
                localStorage['accessToken'] = result;
                window.location.href = 'product.html';
            },
            error: function (xhr, status, error) {
                switch (xhr.status) {
                    case 400:
                        alert("Email or password doesn't correct!");
                        window.location.href = 'signin.html';
                        break;
                    default:
                        alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                        window.location.href = 'index.html';
                        break;
                }
            }
        });
    }

}

function Logout(){
    localStorage.removeItem("accessToken");
    window.location.href = 'index.html';
}