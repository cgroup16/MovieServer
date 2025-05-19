$(document).ready(function () {
    $("#registerForm").submit(function (e) {
        e.preventDefault();

        const name = $("#nameTB").val();
        const email = $("#emailTB").val();
        const password = $("#passwordTB").val();

        const nameRegex = /^[A-Za-z]{2,}$/;
        const passwordRegex = /^(?=.*[A-Z])(?=.*\d).{8,}$/;

        if (!nameRegex.test(name)) {
            $("#errorMsg").text("Name must be at least 2 letters and letters only.");
            return;
        }

        if (!passwordRegex.test(password)) {
            $("#errorMsg").text("Password must be at least 8 characters, include a number and a capital letter.");
            return;
        }

        $("#errorMsg").text("");

        const newUser = {
            Name: name,
            Email: email,
            Password: password,
            Active: true
        };

        $.ajax({
            url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Users",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(newUser),
            success: function () {
                alert("User registered successfully!");
                $("#registerForm")[0].reset();
                window.location.href = "login.html";
            },
            error: function (xhr) {
                if (xhr.status === 409) {
                    $("#errorMsg").text("This email is already registered.");
                } else {
                    $("#errorMsg").text("Registration failed. Please try again.");
                }
            }
        });
    });
});
