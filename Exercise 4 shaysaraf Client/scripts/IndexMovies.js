$(document).ready(function () {
    $("#logoutBtn").click(function () {
        localStorage.removeItem("loggedInUser");
        alert("You have been logged out.");
        window.location.href = "login.html";
    });
    $("#goToLoginBtn").click(function () {
        window.location.href = "login.html";
    });

    $("#loadMoviesBtn").click(loadMovies);

    $("#filterByTitleBtn").click(function () {
        const title = $("#titleFilter").val();
        if (!title) return alert("Please enter a title");

        $.ajax({
            url: `https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies/ByTitle?title=${title}`,
            method: "GET",
            success: function (movies) {
                if (movies.length === 0) return alert("No movies found for this title");
                displayMovies(movies);
            },
            error: () => alert("Error fetching movies by title")
        });
    });

    $("#filterByDateBtn").click(function () {
        const start = $("#startDate").val();
        const end = $("#endDate").val();
        if (!start || !end) return alert("Please select both start and end dates");

        $.ajax({
            url: `https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies/from/${start}/until/${end}`,
            method: "GET",
            success: function (movies) {
                if (movies.length === 0) return alert("No movies found in this date range");
                displayMovies(movies);
            },
            error: () => alert("Error filtering by date")
        });
    });
});

function loadMovies() {
    $.ajax({
        url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies",
        method: "GET",
        success: displayMovies,
        error: function () {
            alert("Failed to load movies.");
        }
    });
}

$("#editProfileBtn").click(function () {
    let loggedUser = localStorage.getItem("loggedInUser");

    if (!loggedUser || loggedUser === "Guest") {
        alert("Guests cannot edit profile. Please login first.");
        window.location.href = "login.html";
        return;
    }

    const user = JSON.parse(loggedUser);

    $("body").append(`
        <div id="popupBackground" style="position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(0,0,0,0.5); z-index:999;">
            <div id="editProfilePopup" style="position:fixed; top:20%; left:35%; background:white; padding:20px; border:1px solid black; z-index:1000;">
                <h3>Edit Profile</h3>
                <input type="text" id="editName" placeholder="New Username" value="${user.name}" /><br><br>
                <input type="password" id="editPassword" placeholder="New Password" /><br><br>
                <input type="email" id="editEmail" placeholder="New Email" value="${user.email}" /><br><br>
                <button id="saveProfileBtn">Save</button>
                <button id="cancelEditBtn">Cancel</button>
            </div>
        </div>
    `);

    $("#popupBackground").click(function (e) {
        if (e.target.id === "popupBackground") {
            $("#popupBackground").remove();
        }
    });

    $("#cancelEditBtn").click(function () {
        $("#popupBackground").remove();
    });

    $("#saveProfileBtn").click(function () {
        const updatedName = $("#editName").val();
        const updatedPassword = $("#editPassword").val();
        const updatedEmail = $("#editEmail").val();
        const loggedInUser = JSON.parse(localStorage.getItem("loggedInUser"));

        if (!updatedName || !updatedPassword || !updatedEmail) {
            alert("Please fill in all fields.");
            return;
        }

        const nameRegex = /^[A-Za-z]{2,}$/;
        const passwordRegex = /^(?=.*[A-Z])(?=.*\d).{8,}$/;

        if (!nameRegex.test(updatedName)) {
            alert("Name must be at least 2 letters and contain letters only.");
            return;
        }

        if (!passwordRegex.test(updatedPassword)) {
            alert("Password must be at least 8 characters, include a number and a capital letter.");
            return;
        }

        $.ajax({
            url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Users",
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                Id: loggedInUser.id,
                Name: updatedName,
                Password: updatedPassword,
                Email: updatedEmail,
                Active: loggedInUser.active
            }),
            success: function () {
                alert("Profile updated successfully!");

                localStorage.setItem("loggedInUser", JSON.stringify({
                    id: loggedInUser.id,
                    name: updatedName,
                    email: updatedEmail,
                    active: loggedInUser.active
                }));

                $("#popupBackground").remove();
            },
            error: function () {
                alert("Failed to update profile.");
            }
        });
    });
});


function displayMovies(movies) {
    const moviesContainer = document.getElementById("moviesContainer");
    moviesContainer.innerHTML = '';

    movies.forEach(movie => {
        const movieCard = document.createElement("div");
        movieCard.classList.add("movie-card");

        movieCard.innerHTML = `
            <img src="${movie.primaryImage}" alt="Movie Image">
            <h3>${movie.primaryTitle} (${new Date(movie.releaseDate).getFullYear()})</h3>
            <p><strong>Description:</strong> ${movie.description || "No description"}</p>
            <p><strong>Genres:</strong> ${movie.genres || "N/A"}</p>
            <p><strong>Release Date:</strong> ${new Date(movie.releaseDate).toLocaleDateString()}</p>
            <p><strong>Language:</strong> ${movie.language}</p>
            <p><strong>Budget:</strong> $${Number(movie.budget).toLocaleString()}</p>
            <p><strong>Gross Worldwide:</strong> $${Number(movie.grossWorldwide).toLocaleString()}</p>
            <p><strong>Is Adult:</strong> ${movie.isAdult ? "Yes" : "No"}</p>
            <p><strong>Runtime:</strong> ${movie.runtimeMinutes} minutes</p>
            <p><strong>Average Rating:</strong> ${movie.averageRating}</p>
            <p><strong>Number of Votes:</strong> ${movie.numVotes.toLocaleString()}</p>
            <p><strong>Price per day:</strong> ${movie.priceToRent} ₪</p>
        `;

        const addToCartBtn = document.createElement("button");
        addToCartBtn.textContent = "Rent me";
        addToCartBtn.classList.add("add-btn");
        addToCartBtn.addEventListener("click", () => addToCart(movie));

        movieCard.appendChild(addToCartBtn);
        moviesContainer.appendChild(movieCard);
    });
}

// פונקציה  להוספת סרט לעגלה
function addToCart(movie) {
    const user = JSON.parse(localStorage.getItem("loggedInUser"));

    if (!user || user === "Guest") {
        alert("You must login first");
        window.location.href = "login.html";
        return;
    }

    $("body").append(`
        <div id="rentPopup" style="position:fixed; top:20%; left:35%; background:white; padding:20px; border:1px solid black; z-index:1000;">
            <h3>Rent Movie: ${movie.primaryTitle}</h3>
            <label>Start Date: <input type="date" id="rentStart"></label><br><br>
            <label>End Date: <input type="date" id="rentEnd"></label><br><br>
            <p>Price per day: <strong>${movie.priceToRent}</strong> ₪</p>
            <p>Total Price: <strong id="totalPrice">0</strong> ₪</p>
            <button id="confirmRent">Confirm</button>
            <button onclick="$('#rentPopup').remove()">Cancel</button>
        </div>
    `);

    $("#rentStart, #rentEnd").on("change", function () {
        const start = new Date($("#rentStart").val());
        const end = new Date($("#rentEnd").val());
        const diffDays = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
        const total = (diffDays > 0) ? diffDays * movie.priceToRent : 0;
        $("#totalPrice").text(total.toFixed(2));
    });

    $("#confirmRent").on("click", function () {
        const rentStartStr = $("#rentStart").val(); // פורמט: YYYY-MM-DD
        const rentEndStr = $("#rentEnd").val();

        if (!rentStartStr || !rentEndStr) {
            alert("Please select both start and end dates.");
            return;
        }

        // המרה לתאריכים אובייקטיביים
        const rentStart = new Date(rentStartStr + "T00:00:00");
        const rentEnd = new Date(rentEndStr + "T00:00:00");

        const today = new Date();
        today.setHours(0, 0, 0, 0); // אפס את השעה

        if (rentStart < today) {
            alert("Start date must be today or in the future.");
            return;
        }

        if (rentEnd <= rentStart) {
            alert("End date must be after start date.");
            return;
        }

        const user = JSON.parse(localStorage.getItem("loggedInUser"));

        $.ajax({
            url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies/RentMovie",
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                userId: user.id,
                movieId: movie.id,
                rentStart: rentStartStr,
                rentEnd: rentEndStr
            }),
            success: function () {
                alert("Movie rented successfully!");
                $("#rentPopup").remove();
            },
            error: function (xhr) {
                if (xhr.status === 400) {
                    alert("You already rented this movie.");
                } else {
                    alert("Error during rental.");
                }
            }
        });
    });

}
