function apiRequest(method, url, data, onSuccess, onError, requireAuth = false) {
    const headers = {};

    if (requireAuth) {
        const token = localStorage.getItem("token");
        if (token) {
            headers["Authorization"] = "Bearer " + token;
        }
    }

    $.ajax({
        url: API_BASE_URL + url,
        method: method,
        contentType: "application/json",
        headers: headers,
        data: data ? JSON.stringify(data) : null,
        success: function (response) {
            if (response && response.success === false) {
                const message = response.message || "Something went wrong";
                if (onError) {
                    onError(message, null);
                } else {
                    showMessage(message, "danger");
                }
                return;
            }

            if (onSuccess) {
                onSuccess(response);
            }
        },
        error: function (xhr) {
            let message = "Something went wrong";

            if (xhr.status === 0) {
                message = "Cannot connect to API. Please make sure the backend is running and CORS is enabled.";
            } else if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            } else if (xhr.status === 400) {
                message = "Bad request. Please check your input.";
            } else if (xhr.status === 401) {
                message = "Unauthorized. Please login again.";
            } else if (xhr.status === 403) {
                message = "Forbidden. You do not have permission.";
            } else if (xhr.status === 404) {
                message = "The requested resource was not found.";
            } else if (xhr.status >= 500) {
                message = "Server error. Please try again later.";
            }

            if (onError) {
                onError(message, xhr);
            } else {
                showMessage(message, "danger");
            }
        }
    });
}

function showMessage(message, type = "info", target = "#message") {
    const element = $(target);
    if (!element.length) {
        alert(message);
        return;
    }

    element.html(`<div class="alert alert-${type}" role="alert">${escapeHtml(message)}</div>`);
}

function clearMessage(target = "#message") {
    $(target).empty();
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function formatDateTime(value) {
    if (!value) {
        return "";
    }

    return new Date(value).toLocaleString();
}

function formatDateForInput(value) {
    if (!value) {
        return "";
    }

    return new Date(value).toISOString().slice(0, 10);
}
