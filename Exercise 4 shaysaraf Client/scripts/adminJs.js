$(document).ready(function () {
    $.get("https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Users", function (users) {
        users.forEach(user => {
            const row = `
        <tr>
          <td>${user.id}</td>
          <td>${user.name}</td>
          <td>${user.email}</td>
          <td>${user.active}</td>
          <td>
            <button onclick="toggleUser(${user.id}, ${!user.active}, '${user.name}', '${user.email}', '${user.password || ''}')">
              ${user.active ? 'Deactivate' : 'Activate'}
            </button>
          </td>
        </tr>`;
            $("#usersTableBody").append(row);
        });
    });
});

function toggleUser(id, newStatus, name, email, password) {
    $.ajax({
        url: "https://proj.ruppin.ac.il/cgroup16/test2/tar1/api/Users/ToggleActive",
        type: "PUT",
        contentType: "application/json",
        data: JSON.stringify({
            Id: id,
            Name: name,
            Email: email,
            Password: password,
            Active: newStatus
        }),
        success: function () {
            alert("Status updated");
            location.reload();
        },
        error: function () {
            alert("Error updating user");
        }
    });
}


