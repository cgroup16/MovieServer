$(document).ready(function () {

    const user = localStorage.getItem("loggedInUser");

    if (!user || user === "Guest") {
        window.location.href = "login.html"; // לא מחובר או אורח? עף החוצה  
        alert("you must to login first");
    }
    $("#backHomeBtn").click(function () {
        window.location.href = "index.html";
    });

    $("#AddmovieForm").submit(function (e) {
        e.preventDefault();
        console.log("📤 Sending movie..."); // לוודא שהגענו לכאן

        const genresInput = $("#genresTB").val();
        const genresRegex = /^([A-Za-z-]+)(,\s*[A-Za-z-]+)*$/;

        if (!genresRegex.test(genresInput)) {
            alert("Genres must be separated by commas (e.g. Action, Sci-Fi)");
            return;
        }



        const movieData = {
            url: $("#urlTB").val(),
            primaryTitle: $("#titleTB").val(),
            description: $("#descriptionTB").val(),
            primaryImage: $("#imageTB").val(),
            year: Number($("#yearTB").val()),
            releaseDate: new Date($("#releaseDateTB").val()).toISOString(),
            language: $("#languageTB").val(),
            budget: Number($("#budgetTB").val()),
            grossWorldwide: Number($("#grossTB").val()) || 0,
            genres: genresInput,
            isAdult: $("#isAdultCB").is(":checked"),
            runtimeMinutes: Number($("#runtimeTB").val()),
            averageRating: Number($("#ratingTB").val()) || 0,
            numVotes: Number($("#votesTB").val()) || 0
        };
        console.log("📦 movieData to send:", movieData);


        $.ajax({
            url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(movieData),
            success: function () {
                alert("Movie added successfully!");
                $("#AddmovieForm")[0].reset(); // מנקה את הטופס
            },
            error: function (xhr) {
                if (xhr.status === 400) {
                    alert("This movie already exists."); // סרט עם אותו Title
                } else {
                    alert("Error adding movie.");
                    console.error(xhr);
                }
            }

        });
    });
});
