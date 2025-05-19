$(document).ready(function () {

    console.log("✅ LoginJs loaded");

    $("#guestBtn").click(function () {
        localStorage.setItem("loggedInUser", "Guest");
        window.location.href = "index.html";
    });

    $("#loginForm").submit(function (e) {
        e.preventDefault();

        const email = $("#email").val();
        const password = $("#password").val();

        console.log("🟡 JSON being sent:", JSON.stringify({
            Email: email,
            Password: password
        }));
      

        $.ajax({
            url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Users/Login",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                Email: email,
                Password: password,
                Name: "",
                Active: true
            }),
            success: function (user) {
                alert("שלום " + user.name + "!");

                // שמירת פרטי המשתמש בצורה מלאה
                localStorage.setItem("loggedInUser", JSON.stringify({
                    id: user.id,
                    name: user.name,
                    email: user.email,
                    active: user.active // הוספנו את זה!
                }));

                window.location.href = "index.html";
            },

             error: function () {
                 $("#errorMsg").text("Invalid email or password or user is inactive.");
            }
        });

    });
});
