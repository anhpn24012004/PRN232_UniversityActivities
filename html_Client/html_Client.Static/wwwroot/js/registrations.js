const registrationStatuses = {
    1: "Registered",
    2: "Attended",
    3: "Cancelled",
    4: "Absent"
};

function loadMyRegistrations() {
    if (!isLoggedIn()) {
        showMessage("Please login to view your registrations.", "warning");
        $("#registrationsBody").html('<tr><td colspan="6" class="text-center">Login required.</td></tr>');
        return;
    }

    $("#registrationsBody").html('<tr><td colspan="6" class="text-center">Loading...</td></tr>');

    apiRequest("GET", "/api/Registrations/my", null, function (response) {
        renderRegistrations(response.data || []);
    }, null, true);
}

function renderRegistrations(registrations) {
    if (!registrations.length) {
        $("#registrationsBody").html('<tr><td colspan="6" class="text-center">No registrations found.</td></tr>');
        return;
    }

    const rows = registrations.map(function (registration) {
        const canCancel = registration.status === 1;
        return `
            <tr>
                <td>${registration.id}</td>
                <td>${registration.activityId}</td>
                <td>${escapeHtml(registration.activityTitle)}</td>
                <td>${formatDateTime(registration.registeredAt)}</td>
                <td><span class="status-pill">${escapeHtml(registrationStatuses[registration.status] || registration.status)}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-danger cancel-registration" data-id="${registration.id}" ${canCancel ? "" : "disabled"}>
                        Cancel
                    </button>
                </td>
            </tr>
        `;
    });

    $("#registrationsBody").html(rows.join(""));
}

function cancelRegistration(registrationId) {
    apiRequest("PUT", "/api/Registrations/" + registrationId + "/cancel", null, function (response) {
        showMessage(response.message || "Cancel registration successfully", "success");
        loadMyRegistrations();
    }, null, true);
}

$(function () {
    $(document).on("click", ".cancel-registration", function () {
        const registrationId = $(this).data("id");
        cancelRegistration(registrationId);
    });

    if ($("#registrationsBody").length) {
        loadMyRegistrations();
    }
});
