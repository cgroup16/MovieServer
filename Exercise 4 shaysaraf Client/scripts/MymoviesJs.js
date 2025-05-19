$(document).ready(function () {
    const user = localStorage.getItem("loggedInUser");

    if (!user || user === "Guest") {
        alert("You must login first");
        window.location.href = "login.html";
        return;
    }

    loadRentedMovies();

    $("#backHomeBtn").on("click", function () {
        window.location.href = "index.html";
    });
});

function loadRentedMovies() {
    const user = JSON.parse(localStorage.getItem("loggedInUser"));
    if (!user || user === "Guest") {
        alert("You must login first");
        window.location.href = "login.html";
        return;
    }

    $.ajax({
        url: `https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies/RentedByUser/${user.id}`,
        type: 'GET',
        success: function (movies) {
            if (movies.length === 0) {
                alert("You have no currently rented movies.");
            } else {
                renderMovies(movies);
            }
        },
        error: function () {
            alert("You have no currently rented movies.");
        }
    });
}

function renderMovies(movies) {
    const container = $("#moviesContainer");
    container.empty();

    movies.forEach(movie => {
        const movieCard = $(`
            <div class="movie-card">
                <h3>${movie.primaryTitle} (${movie.year})</h3>
                <img src="${movie.primaryImage}" alt="Movie Image" />
                <p><strong>Description:</strong> ${movie.description || "No description"}</p>
                <p><strong>Genres:</strong> ${movie.genres}</p>
                <p><strong>Release Date:</strong> ${movie.releaseDate?.split('T')[0]}</p>
                <p><strong>Language:</strong> ${movie.language}</p>
                <p><strong>Budget:</strong> $${movie.budget.toLocaleString()}</p>
                <p><strong>Gross Worldwide:</strong> $${movie.grossWorldwide.toLocaleString()}</p>
                <p><strong>Is Adult:</strong> ${movie.isAdult ? "Yes" : "No"}</p>
                <p><strong>Runtime:</strong> ${movie.runtimeMinutes} minutes</p>
                <p><strong>Average Rating:</strong> ${movie.averageRating}</p>
                <p><strong>Number of Votes:</strong> ${movie.numVotes.toLocaleString()}</p>
                <input type="number" class="target-user-id" placeholder="Enter target user ID" />
                <button class="transfer-btn">Transfer Rental</button>
                <button class="delete-btn">Cancel Rental</button>
            </div>
        `);

        // כפתור ביטול השכרה
        movieCard.find(".delete-btn").on("click", function () {
            const user = JSON.parse(localStorage.getItem("loggedInUser"));

            $.ajax({
                url: `https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies/CancelRental?userId=${user.id}&movieId=${movie.id}`,
                type: 'DELETE',
                success: function () {
                    alert("Rental cancelled.");
                    loadRentedMovies();
                },
                error: function () {
                    alert("Failed to cancel rental.");
                }
            });
        });

        // כפתור העברת השכרה
        movieCard.find(".transfer-btn").on("click", function () {
            const user = JSON.parse(localStorage.getItem("loggedInUser"));
            const toUserId = parseInt(movieCard.find(".target-user-id").val());

            if (!toUserId || isNaN(toUserId)) {
                alert("Please enter a valid user ID.");
                return;
            }

            $.ajax({
                url: `https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Movies/TransferRental?fromUserId=${user.id}&toUserId=${toUserId}&movieId=${movie.id}`,
                type: 'PUT',
                success: function () {
                    alert("Rental transferred.");
                    loadRentedMovies();
                },
                error: function (xhr) {
                    alert(xhr.responseText); // כאן הייתה הבעיה - תיקנתי
                }
            });
        });

        container.append(movieCard);
    });
}
