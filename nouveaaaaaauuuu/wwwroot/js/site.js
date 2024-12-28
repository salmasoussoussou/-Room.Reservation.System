@page
@model nouveaaaaaauuuu.Pages.Account.RegisterModel

    < !DOCTYPE html >
        <html lang="en">
            <head>
                <meta charset="utf-8" />
                <title>Register - Système de Réservation</title>
                <link rel="stylesheet" href="~/css/site.css" />
                <script>
                    function validateForm() {
            var password = document.getElementById("Password").value;
                    var confirmPassword = document.getElementById("ConfirmPassword").value;

                    if (password !== confirmPassword) {
                        alert("Passwords do not match.");
                    return false;
            }
                    return true;
        }
                </script>
            </head>
            <body>
                <div class="container">
                    <h2>Register</h2>
                    <form method="post" onsubmit="return validateForm()">
                        <div class="form-group">
                            <label for="UserName">UserName</label>
                            <input type="text" asp-for="Input.UserName" class="form-control" id="UserName" />
                            <span asp-validation-for="Input.UserName" class="text-danger"></span>
                        </div>
                        <div class="form-group">
                            <label for="Email">Email</label>
                            <input type="email" asp-for="Input.Email" class="form-control" id="Email" />
                            <span asp-validation-for="Input.Email" class="text-danger"></span>
                        </div>
                        <div class="form-group">
                            <label for="Password">Password</label>
                            <input type="password" asp-for="Input.Password" class="form-control" id="Password" />
                            <span asp-validation-for="Input.Password" class="text-danger"></span>
                        </div>
                        <div class="form-group">
                            <label for="ConfirmPassword">Confirm Password</label>
                            <input type="password" asp-for="Input.ConfirmPassword" class="form-control" id="ConfirmPassword" />
                            <span asp-validation-for="Input.ConfirmPassword" class="text-danger"></span>
                        </div>
                        <button type="submit" class="btn btn-primary">Register</button>
                    </form>
                </div>
            </body>
        </html>
