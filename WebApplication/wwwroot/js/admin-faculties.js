const API_URL = "/api/faculties";

let allFaculties = null;

document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
    AdminUi.wireDatalistInput(
        "filter-faculty-input",
        "filter-faculty-id",
        "faculties-list"
    );

    renderTable();
    //loadFaculties();
}

function getFilterValue() {
    return document
        .getElementById("filter-faculty-input")
        ?.value
        .trim() ?? "";
}

async function applyFilters() {
    await loadFaculties(getFilterValue());
}

async function loadFaculties(search = "") {
    const queryString = search
        ? `?search=${encodeURIComponent(search)}`
        : "";

    try {
        const response = await fetch(`${API_URL}${queryString}`);

        if (!response.ok) {
            throw new Error(
                `Nie udało się pobrać wydziałów. Kod: ${response.status}`
            );
        }

        allFaculties = await response.json();
        renderTable();
    } catch (error) {
        console.error("Błąd pobierania wydziałów:", error);

        allFaculties = [];
        renderTable();

        alert("Nie udało się pobrać listy wydziałów.");
    }
}

function renderTable() {
    const tbody = document.getElementById("faculties-rows");

    if (!tbody) {
        return;
    }

    if (allFaculties === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="3"
                    class="p-8 text-center text-gray-500">
                    Wpisz nazwę wydziału i kliknij „Filtruj”,
                    aby wyświetlić wyniki.
                </td>
            </tr>
        `;
        return;
    }

    if (allFaculties.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="3"
                    class="p-8 text-center text-gray-500">
                    Nie znaleziono wydziałów.
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = allFaculties
        .map(faculty => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20
                       transition-colors">

                <td class="p-4 text-sm font-semibold
                           text-gray-900 dark:text-gray-100">
                    ${escapeHtml(faculty.abbreviation)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${escapeHtml(faculty.name)}
                </td>

                <td class="p-4 text-sm text-right space-x-2">
                    <button
                        onclick="openEditModal(${faculty.id})"
                        class="text-blue-600 hover:underline font-medium">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button
                        onclick="deleteFaculty(${faculty.id})"
                        class="text-red-600 hover:underline font-medium">
                        Usuń
                    </button>
                </td>
            </tr>
        `)
        .join("");
}

function getFormPayload() {
    const idValue = document.getElementById("form-id").value;

    return {
        id: idValue
            ? Number.parseInt(idValue, 10)
            : 0,

        abbreviation: document
            .getElementById("form-abbreviation")
            .value
            .trim(),

        name: document
            .getElementById("form-name")
            .value
            .trim()
    };
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const payload = getFormPayload();
    const isEdit = payload.id > 0;

    try {
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
            const errorText = await response.text();

            console.error(
                "Błąd zapisu wydziału:",
                response.status,
                errorText
            );

            alert("Nie udało się zapisać wydziału.");
            return;
        }

        closeCrudModal();
        await loadFaculties(getFilterValue());
    } catch (error) {
        console.error("Błąd zapisu wydziału:", error);
        alert("Wystąpił błąd podczas zapisywania wydziału.");
    }
}

function openCreateModal() {
    document.getElementById("modal-title").innerText =
        "Dodaj wydział";

    document.getElementById("faculty-form").reset();
    document.getElementById("form-id").value = "";

    document
        .getElementById("crud-modal")
        .classList
        .remove("hidden");
}

async function openEditModal(id) {
    try {
        const response = await fetch(`${API_URL}/${id}`);

        if (!response.ok) {
            alert("Nie udało się pobrać danych wydziału.");
            return;
        }

        const faculty = await response.json();

        document.getElementById("modal-title").innerText =
            "Edytuj wydział";

        document.getElementById("form-id").value =
            faculty.id;

        document.getElementById("form-abbreviation").value =
            faculty.abbreviation ?? "";

        document.getElementById("form-name").value =
            faculty.name ?? "";

        document
            .getElementById("crud-modal")
            .classList
            .remove("hidden");
    } catch (error) {
        console.error(
            "Błąd pobierania danych wydziału:",
            error
        );

        alert("Wystąpił błąd podczas pobierania wydziału.");
    }
}

function closeCrudModal() {
    document
        .getElementById("crud-modal")
        .classList
        .add("hidden");
}

async function deleteFaculty(id) {
    if (!confirm("Czy na pewno chcesz usunąć ten wydział?")) {
        return;
    }

    try {
        const response = await fetch(
            `${API_URL}/${id}`,
            {
                method: "DELETE"
            }
        );

        if (!response.ok) {
            const errorText = await response.text();

            console.error(
                "Błąd usuwania wydziału:",
                response.status,
                errorText
            );

            alert("Nie udało się usunąć wydziału.");
            return;
        }

        allFaculties =
            allFaculties?.filter(faculty => faculty.id !== id)
            ?? [];

        renderTable();
    } catch (error) {
        console.error("Błąd usuwania wydziału:", error);
        alert("Wystąpił błąd podczas usuwania wydziału.");
    }
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}