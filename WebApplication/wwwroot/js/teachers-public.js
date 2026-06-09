const PUBLIC_TEACHERS_API =
    "/api/public/teachers";

let publicTeachers = [];

document.addEventListener(
    "DOMContentLoaded",
    initializeTeachersPage
);

async function initializeTeachersPage() {
    const searchInput =
        document.getElementById("teacher-search");

    searchInput?.addEventListener(
        "input",
        renderPublicTeachers
    );

    await loadPublicTeachers();
}

async function loadPublicTeachers() {
    try {
        const response = await fetch(
            PUBLIC_TEACHERS_API
        );

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać listy nauczycieli."
            );
        }

        publicTeachers = await response.json();

        document
            .getElementById("teachers-loading")
            ?.classList
            .add("hidden");

        renderPublicTeachers();
    } catch (error) {
        console.error(
            "Błąd pobierania nauczycieli:",
            error
        );

        const loading =
            document.getElementById("teachers-loading");

        if (loading) {
            loading.textContent =
                "Nie udało się pobrać nauczycieli.";
        }
    }
}

function getTeacherDisplayName(teacher) {
    return [
        teacher.academicTitle,
        teacher.firstName,
        teacher.lastName
    ]
        .filter(value =>
            value &&
            value.trim().length > 0)
        .join(" ");
}

function renderPublicTeachers() {
    const grid =
        document.getElementById("teachers-grid");

    const empty =
        document.getElementById("teachers-empty");

    if (!grid || !empty) {
        return;
    }

    const search = document
        .getElementById("teacher-search")
        ?.value
        .trim()
        .toLowerCase() ?? "";

    const filtered = publicTeachers.filter(
        teacher => {
            const searchable = [
                teacher.academicTitle,
                teacher.firstName,
                teacher.lastName,
                teacher.email
            ]
                .filter(Boolean)
                .join(" ")
                .toLowerCase();

            return searchable.includes(search);
        }
    );

    if (filtered.length === 0) {
        grid.classList.add("hidden");
        empty.classList.remove("hidden");
        grid.innerHTML = "";
        return;
    }

    empty.classList.add("hidden");
    grid.classList.remove("hidden");

    grid.innerHTML = filtered
        .map(teacher => `
            <article class="rounded-xl border border-gray-200
                            bg-white p-5 shadow-sm
                            dark:border-gray-700 dark:bg-gray-900">

                <h2 class="text-lg font-semibold
                           text-gray-900 dark:text-gray-100">
                    ${AdminUi.escapeHtml(
            getTeacherDisplayName(teacher)
        )}
                </h2>

                ${
            teacher.email
                ? `
                            <a href="mailto:${encodeURIComponent(
                    teacher.email
                )}"
                               class="mt-2 inline-block text-sm
                                      text-blue-600 hover:underline">
                                ${AdminUi.escapeHtml(
                    teacher.email
                )}
                            </a>
                          `
                : `
                            <p class="mt-2 text-sm text-gray-500">
                                Brak podanego adresu e-mail
                            </p>
                          `
        }
            </article>
        `)
        .join("");
}