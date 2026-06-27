function loadProfile() {
    if (!isLoggedIn()) {
        showMessage("Please login to view your profile.", "warning");
        return;
    }

    apiRequest("GET", "/api/Auth/profile", null, function (response) {
        const profile = response.data || {};
        $("#fullName").val(profile.fullName || "");
        $("#email").val(profile.email || "");
        $("#dateOfBirth").val(formatDateForInput(profile.dateOfBirth));
        $("#studentCode").val(profile.studentCode || "");
        $("#department").val(profile.department || "");
    }, null, true);
}

function updateProfile() {
    const request = {
        fullName: $("#fullName").val(),
        dateOfBirth: $("#dateOfBirth").val() + "T00:00:00",
        studentCode: $("#studentCode").val(),
        department: $("#department").val()
    };

    apiRequest("PUT", "/api/Auth/profile", request, function (response) {
        showMessage(response.message || "Update profile successfully", "success");
    }, null, true);
}

$(function () {
    $("#profileForm").on("submit", function (event) {
        event.preventDefault();
        updateProfile();
    });

    if ($("#profileForm").length) {
        loadProfile();
    }
});
