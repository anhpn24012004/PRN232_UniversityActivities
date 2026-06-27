const activityTypes = {
    1: "Workshop",
    2: "Seminar",
    3: "Competition",
    4: "Volunteer",
    5: "ClubEvent"
};

const activityStatuses = {
    1: "Pending",
    2: "Approved",
    3: "Rejected"
};

function buildActivityQuery() {
    const params = new URLSearchParams();
    const keyword = $("#keyword").val();
    const type = $("#type").val();
    const location = $("#location").val();

    if (keyword) {
        params.append("keyword", keyword);
    }

    if (type) {
        params.append("type", type);
    }

    if (location) {
        params.append("location", location);
    }

    const query = params.toString();
    return query ? "?" + query : "";
}

function loadActivities() {
    clearMessage();
    $("#activitiesBody").html('<tr><td colspan="7" class="text-center">Loading...</td></tr>');

    apiRequest("GET", "/api/Activities" + buildActivityQuery(), null, function (response) {
        const activities = response.data || [];
        renderActivities(activities);
    });
}

function renderActivities(activities) {
    if (!activities.length) {
        $("#activitiesBody").html('<tr><td colspan="7" class="text-center">No activities found.</td></tr>');
        return;
    }

    const rows = activities.map(function (activity) {
        return `
            <tr>
                <td>${escapeHtml(activity.title)}</td>
                <td>${escapeHtml(activityTypes[activity.type] || activity.type)}</td>
                <td>${escapeHtml(activity.location)}</td>
                <td>${formatDateTime(activity.startTime)}</td>
                <td>${formatDateTime(activity.endTime)}</td>
                <td>${activity.maxParticipants}</td>
                <td>
                    <a class="btn btn-sm btn-outline-primary" href="activity-detail.html?id=${activity.id}">View Detail</a>
                </td>
            </tr>
        `;
    });

    $("#activitiesBody").html(rows.join(""));
}

function loadActivityDetail() {
    const id = new URLSearchParams(window.location.search).get("id");
    if (!id) {
        showMessage("Activity id is missing.", "danger");
        return;
    }

    apiRequest("GET", "/api/Activities/" + id, null, function (response) {
        const activity = response.data;
        $("#activityTitle").text(activity.title);
        $("#activityDescription").text(activity.description);
        $("#activityType").text(activityTypes[activity.type] || activity.type);
        $("#activityStatus").text(activityStatuses[activity.status] || activity.status);
        $("#activityLocation").text(activity.location);
        $("#activityStart").text(formatDateTime(activity.startTime));
        $("#activityEnd").text(formatDateTime(activity.endTime));
        $("#activityMax").text(activity.maxParticipants);
        $("#activityOrganizer").text(activity.organizerName || "");
        $("#registerBtn").data("activity-id", activity.id);

        if (!isLoggedIn()) {
            $("#registerHint").text("Please login before registering activity.");
        }
    });
}

function registerActivity(activityId) {
    if (!isLoggedIn()) {
        showMessage("Please login before registering activity.", "warning");
        return;
    }

    apiRequest("POST", "/api/Registrations/activities/" + activityId, null, function (response) {
        showMessage(response.message || "Register activity successfully", "success");
    }, null, true);
}

function bindActivityEvents() {
    $("#searchBtn").on("click", loadActivities);

    $("#resetBtn").on("click", function () {
        $("#keyword").val("");
        $("#type").val("");
        $("#location").val("");
        loadActivities();
    });

    $("#registerBtn").on("click", function () {
        registerActivity($(this).data("activity-id"));
    });
}

$(function () {
    bindActivityEvents();

    if ($("#activitiesBody").length) {
        loadActivities();
    }

    if ($("#activityDetail").length) {
        loadActivityDetail();
    }
});
