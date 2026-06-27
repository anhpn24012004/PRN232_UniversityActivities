function isLoggedIn() {
    return !!localStorage.getItem("token");
}

function getCurrentUser() {
    const rawUser = localStorage.getItem("user");
    if (!rawUser) {
        return null;
    }

    try {
        return JSON.parse(rawUser);
    } catch {
        return null;
    }
}

function saveAuth(data) {
    if (!data || !data.token) {
        return;
    }

    const user = {
        email: data.email || "",
        fullName: data.fullName || "",
        roles: data.roles || []
    };

    localStorage.setItem("token", data.token);
    localStorage.setItem("user", JSON.stringify(user));
}

function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    window.location.href = "login.html";
}

function updateNavbar() {
    const loggedIn = isLoggedIn();
    const user = getCurrentUser();

    $(".auth-required").toggleClass("d-none", !loggedIn);
    $(".guest-only").toggleClass("d-none", loggedIn);

    if (loggedIn && user) {
        const roles = Array.isArray(user.roles) ? user.roles.join(", ") : "";
        $("#authStatus").text(`${user.email || user.fullName} ${roles ? "(" + roles + ")" : ""}`);
    } else {
        $("#authStatus").text("Guest");
    }
}

function bindAuthForms() {
    $("#loginForm").on("submit", function (event) {
        event.preventDefault();
        clearMessage();

        const request = {
            email: $("#email").val(),
            password: $("#password").val()
        };

        apiRequest("POST", "/api/Auth/login", request, function (response) {
            saveAuth(response.data);
            showMessage(response.message || "Login successfully", "success");
            setTimeout(function () {
                window.location.href = "activities.html";
            }, 600);
        });
    });

    $("#registerForm").on("submit", function (event) {
        event.preventDefault();
        clearMessage();

        const request = {
            fullName: $("#fullName").val(),
            email: $("#email").val(),
            password: $("#password").val(),
            dateOfBirth: $("#dateOfBirth").val() + "T00:00:00",
            studentCode: $("#studentCode").val(),
            department: $("#department").val()
        };

        apiRequest("POST", "/api/Auth/register", request, function (response) {
            showMessage(response.message || "Register successfully", "success");
            setTimeout(function () {
                window.location.href = "login.html";
            }, 800);
        });
    });

    $(document).on("click", "#logoutBtn", function (event) {
        event.preventDefault();
        logout();
    });
}

$(function () {
    updateNavbar();
    bindAuthForms();
});
