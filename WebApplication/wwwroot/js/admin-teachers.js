const API_URL =
    "/api/admin/teachers";

let allTeachers = null;
let metadataPromise = null;

let metadata = {
    suggestions: []
};

document.addEventListener(
    "DOMContentLoaded",
    initializePage
);

async function initializePage() {
    try {
        metadataPromise = loadMetadata();

        await metadataPromise;
        await loadTeachers();
    } catch (error) {
        console.error(
            "Błąd inicjalizacji panelu nauczycieli:",
            error
        );
    }
}

async function loadMetadata() {
    const response = await fetch(
        `${API_URL}/metadata`
    );

    if (!response.ok) {
        throw new Error(
            "Nie udało się pobrać danych pomocniczych nauczycieli."
        );
    }

    metadata = await response.json();
    metadata.suggestions ??= [];

    const list = document.getElementById(
        "teacher-suggestions"
    );

    if (list) {
        list.innerHTML = metadata.suggestions
            .map(value => `
                <option value="${AdminUi.escapeHtml(value)}">
                </option>
            `)
            .join("");
    }
}

async function ensureMetadataLoaded() {
    if (!metadataPromise) {
        metadataPromise = loadMetadata();
    }

    await metadataPromise;
}

async function applyFilters() {
    await loadTeachers();
}

function clearFilters() {
    document.getElementById(
        "filter-search"
    ).value = "";

    loadTeachers();
}

async function loadTeachers() {
    const search = document
        .getElementById("filter-search")
        .value
        .trim();

    const url = search
        ? `${API_URL}?search=${encodeURIComponent(search)}`
        : API_URL;

    const response = await fetch(url);

    if (!response.ok) {
        alert("Nie udało się pobrać nauczycieli.");
        return;
    }

    allTeachers = await response.json();
    renderTable();
}

function renderTable() {
    const tbody =
        document.getElementById("teachers-rows");

    if (!tbody) {
        return;
    }

    if (!allTeachers?.length) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Brak nauczycieli.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allTeachers
        .map(teacher => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20">

                <td class="p-4 text-sm">
                    ${AdminUi.escapeHtml(
            teacher.academicTitle || "Brak"
        )}
                </td>

                <td class="p-4 text-sm">
                    ${AdminUi.escapeHtml(
            teacher.firstName
        )}
                </td>

                <td class="p-4 text-sm font-semibold">
                    ${AdminUi.escapeHtml(
            teacher.lastName
        )}
                </td>

                <td class="p-4 text-sm">
                    ${AdminUi.escapeHtml(
            teacher.email || "Brak"
        )}
                </td>

                <td class="p-4 text-right text-sm space-x-2">

                    <button type="button"
                            onclick="openEditModal(${teacher.id})"
                            class="font-medium text-blue-600
                                   hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button type="button"
                            onclick="deleteTeacher(${teacher.id})"
                            class="font-medium text-red-600
                                   hover:underline">
                        Usuń
                    </button>
                </td>
            </tr>
        `)
        .join("");
}

function showCrudModal() {
    const modal =
        document.getElementById("crud-modal");

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}

function closeCrudModal() {
    const modal =
        document.getElementById("crud-modal");

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}

async function openCreateModal() {
    await ensureMetadataLoaded();

    document
        .getElementById("teacher-form")
        .reset();

    document.getElementById("form-id").value = "";

    document.getElementById("modal-title").innerText =
        "Dodaj nauczyciela";

    showCrudModal();
}

async function openEditModal(id) {
    const response = await fetch(
        `${API_URL}/${id}`
    );

    if (!response.ok) {
        alert(
            "Nie udało się pobrać danych nauczyciela."
        );

        return;
    }

    const teacher = await response.json();

    document.getElementById("modal-title").innerText =
        "Edytuj nauczyciela";

    document.getElementById("form-id").value =
        teacher.id;

    document.getElementById(
        "form-academicTitle"
    ).value = teacher.academicTitle ?? "";

    document.getElementById(
        "form-firstName"
    ).value = teacher.firstName ?? "";

    document.getElementById(
        "form-lastName"
    ).value = teacher.lastName ?? "";

    document.getElementById(
        "form-email"
    ).value = teacher.email ?? "";

    showCrudModal();
}

function getFormPayload() {
    return {
        id: Number.parseInt(
            document.getElementById("form-id").value,
            10
        ) || 0,

        academicTitle: document
            .getElementById("form-academicTitle")
            .value
            .trim(),

        firstName: document
            .getElementById("form-firstName")
            .value
            .trim(),

        lastName: document
            .getElementById("form-lastName")
            .value
            .trim(),

        email: document
            .getElementById("form-email")
            .value
            .trim()
    };
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const payload = getFormPayload();

    if (!payload.firstName) {
        alert("Imię jest wymagane.");
        return;
    }

    if (!payload.lastName) {
        alert("Nazwisko jest wymagane.");
        return;
    }

    const isEdit = payload.id > 0;

    const response = await fetch(
        isEdit
            ? `${API_URL}/${payload.id}`
            : API_URL,
        {
            method: isEdit ? "PUT" : "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        }
    );

    if (!response.ok) {
        alert(await readErrorMessage(response));
        return;
    }

    closeCrudModal();
    await loadTeachers();

    metadataPromise = loadMetadata();
    await metadataPromise;
}

async function deleteTeacher(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć nauczyciela?"
    )) {
        return;
    }

    const response = await fetch(
        `${API_URL}/${id}`,
        {
            method: "DELETE"
        }
    );

    if (!response.ok) {
        alert(await readErrorMessage(response));
        return;
    }

    allTeachers =
        allTeachers?.filter(
            teacher => teacher.id !== id
        ) ?? [];

    renderTable();
}

async function readErrorMessage(response) {
    try {
        const body = await response.json();

        if (body.message) {
            return body.message;
        }

        if (body.errors) {
            return Object
                .values(body.errors)
                .flat()
                .join("\n");
        }
    } catch {
        // Odpowiedź nie była JSON-em.
    }

    return "Operacja nie powiodła się.";
}