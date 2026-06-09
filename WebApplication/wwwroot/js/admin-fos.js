const API_URL = "/api/FieldsOfStudiesApi";

let allFieldsOfStudy = null;
let metadataPromise = null;
let cachedFaculties = [];

const MODE_NAMES = {
    0: "Stacjonarne",
    1: "Niestacjonarne",
    2: "Podyplomowe"
};

document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
    metadataPromise = loadMetadata();
    loadSuggestions();
    renderTable();
}

function getFilterPayload() {
    const modeValue =
        document.getElementById("filter-mode-input")?.value ?? "";

    return {
        name: document
            .getElementById("filter-name-input")
            ?.value
            .trim() || null,

        mode: modeValue === ""
            ? null
            : Number.parseInt(modeValue, 10),

        facultyId: null,
        degree: null
    };
}

async function applyFilters() {
    try {
        const response = await fetch(`${API_URL}/filter`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(getFilterPayload())
        });

        if (!response.ok) {
            throw new Error(
                await AdminUi.getErrorMessage(
                    response,
                    "Nie udało się pobrać kierunków studiów."
                )
            );
        }

        allFieldsOfStudy = await response.json();
        renderTable();
    } catch (error) {
        console.error(
            "Błąd filtrowania kierunków studiów:",
            error
        );

        allFieldsOfStudy = [];
        renderTable();

        alert(error.message);
    }
}

async function loadSuggestions() {
    try {
        const response = await fetch(`${API_URL}/suggestions`);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać sugestii kierunków."
            );
        }

        const names = await response.json();
        const datalist =
            document.getElementById("fos-suggestions");

        if (!datalist) {
            return;
        }

        datalist.innerHTML = names
            .map(name => `
                <option value="${AdminUi.escapeHtml(name)}"></option>
            `)
            .join("");
    } catch (error) {
        console.error(
            "Błąd ładowania sugestii kierunków:",
            error
        );
    }
}

async function loadMetadata() {
    const response = await fetch(`${API_URL}/metadata`);

    if (!response.ok) {
        throw new Error(
            await AdminUi.getErrorMessage(
                response,
                "Nie udało się pobrać listy wydziałów."
            )
        );
    }

    const metadata = await response.json();

    cachedFaculties = metadata.faculties ?? [];

    populateFacultySelect();
    populateModeSelect();
}

async function ensureMetadataLoaded() {
    if (!metadataPromise) {
        metadataPromise = loadMetadata();
    }

    try {
        await metadataPromise;
    } catch (error) {
        metadataPromise = null;
        throw error;
    }
}

function populateFacultySelect() {
    const select =
        document.getElementById("form-facultyId");

    if (!select) {
        return;
    }

    const currentValue = select.value;

    select.innerHTML = `
        <option value="">Wybierz wydział</option>
    ` + cachedFaculties
        .map(faculty => `
            <option value="${faculty.key}">
                ${AdminUi.escapeHtml(faculty.value)}
            </option>
        `)
        .join("");

    if (currentValue) {
        select.value = currentValue;
    }
}

function populateModeSelect() {
    const select =
        document.getElementById("form-mode");

    if (!select) {
        return;
    }

    const currentValue = select.value;

    select.innerHTML = `
        <option value="">Wybierz tryb</option>
    ` + Object.entries(MODE_NAMES)
        .map(([value, text]) => `
            <option value="${value}">
                ${AdminUi.escapeHtml(text)}
            </option>
        `)
        .join("");

    if (currentValue) {
        select.value = currentValue;
    }
}

function renderTable() {
    const tbody = document.getElementById("fos-rows");

    if (!tbody) {
        return;
    }

    if (allFieldsOfStudy === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Wybierz filtry i kliknij „Filtruj”,
                    aby wyświetlić kierunki studiów.
                </td>
            </tr>
        `;

        return;
    }

    if (allFieldsOfStudy.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Brak kierunków dla wybranych filtrów.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allFieldsOfStudy
        .map(field => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20
                       transition-colors">

                <td class="p-4 text-sm font-semibold
                           text-gray-900 dark:text-gray-100">
                    ${AdminUi.escapeHtml(field.name)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${AdminUi.escapeHtml(
            field.facultyAbbreviation
        )}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${AdminUi.escapeHtml(field.degree)}
                </td>

                <td class="p-4 text-sm text-gray-500">
                    ${AdminUi.escapeHtml(
            MODE_NAMES[field.mode] ??
            `Nieznany tryb (${field.mode})`
        )}
                </td>

                <td class="p-4 text-sm text-right space-x-2">
                    <button
                        onclick="openEditModal(${field.id})"
                        class="text-blue-600 hover:underline
                               font-medium">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button
                        onclick="deleteFieldOfStudy(${field.id})"
                        class="text-red-600 hover:underline
                               font-medium">
                        Usuń
                    </button>
                </td>
            </tr>
        `)
        .join("");
}

function getFormPayload() {
    const idValue =
        document.getElementById("form-id").value;

    return {
        id: idValue === ""
            ? 0
            : Number.parseInt(idValue, 10),

        name: document
            .getElementById("form-name")
            .value
            .trim(),

        degree: document
            .getElementById("form-degree")
            .value
            .trim(),

        facultyId: Number.parseInt(
            document.getElementById("form-facultyId").value,
            10
        ),

        mode: Number.parseInt(
            document.getElementById("form-mode").value,
            10
        )
    };
}

function validatePayload(payload) {
    if (!payload.name) {
        alert("Nazwa kierunku jest wymagana.");
        return false;
    }

    if (!payload.degree) {
        alert("Stopień studiów jest wymagany.");
        return false;
    }

    if (Number.isNaN(payload.facultyId)) {
        alert("Wybierz wydział.");
        return false;
    }

    if (Number.isNaN(payload.mode)) {
        alert("Wybierz tryb studiów.");
        return false;
    }

    return true;
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const payload = getFormPayload();

    if (!validatePayload(payload)) {
        return;
    }

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
            throw new Error(
                await AdminUi.getErrorMessage(
                    response,
                    "Nie udało się zapisać kierunku studiów."
                )
            );
        }

        closeCrudModal();
        await applyFilters();
        await loadSuggestions();
    } catch (error) {
        console.error(
            "Błąd zapisu kierunku studiów:",
            error
        );

        alert(error.message);
    }
}

async function openCreateModal() {
    try {
        await ensureMetadataLoaded();

        document.getElementById("modal-title").innerText =
            "Dodaj kierunek";

        document.getElementById("fos-form").reset();
        document.getElementById("form-id").value = "";

        populateFacultySelect();
        populateModeSelect();

        showCrudModal();
    } catch (error) {
        console.error(
            "Błąd przygotowania formularza kierunku:",
            error
        );

        alert(error.message);
    }
}

async function openEditModal(id) {
    try {
        await ensureMetadataLoaded();

        const response = await fetch(`${API_URL}/${id}`);

        if (!response.ok) {
            throw new Error(
                await AdminUi.getErrorMessage(
                    response,
                    "Nie udało się pobrać danych kierunku."
                )
            );
        }

        const field = await response.json();

        populateFacultySelect();
        populateModeSelect();

        document.getElementById("modal-title").innerText =
            "Edytuj kierunek";

        document.getElementById("form-id").value =
            field.id;

        document.getElementById("form-name").value =
            field.name ?? "";

        document.getElementById("form-degree").value =
            field.degree ?? "";

        document.getElementById("form-facultyId").value =
            field.facultyId?.toString() ?? "";

        document.getElementById("form-mode").value =
            field.mode?.toString() ?? "";

        showCrudModal();
    } catch (error) {
        console.error(
            "Błąd pobierania kierunku do edycji:",
            error
        );

        alert(error.message);
    }
}

function closeCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}
async function deleteFieldOfStudy(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć ten kierunek studiów?"
    )) {
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
            throw new Error(
                await AdminUi.getErrorMessage(
                    response,
                    "Nie udało się usunąć kierunku studiów."
                )
            );
        }

        allFieldsOfStudy =
            allFieldsOfStudy?.filter(field => field.id !== id)
            ?? [];

        renderTable();
        await loadSuggestions();
    } catch (error) {
        console.error(
            "Błąd usuwania kierunku studiów:",
            error
        );

        alert(error.message);
    }
}

function showCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}