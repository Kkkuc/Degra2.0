const API_URL = "/api/semesters";

let allSemesters = null;
let metadataPromise = null;

let metadata = {
    nameSuggestions: [],
    academicYears: []
};

document.addEventListener(
    "DOMContentLoaded",
    initializePage
);

function initializePage() {
    metadataPromise = loadMetadata();
    renderTable();
}

async function loadMetadata() {
    try {
        const response = await fetch(`${API_URL}/metadata`);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać danych pomocniczych semestrów."
            );
        }

        metadata = await response.json();

        metadata.nameSuggestions ??= [];
        metadata.academicYears ??= [];

        populateMetadata();
    } catch (error) {
        metadataPromise = null;

        console.error(
            "Błąd pobierania metadata semestrów:",
            error
        );

        throw error;
    }
}

async function ensureMetadataLoaded() {
    if (!metadataPromise) {
        metadataPromise = loadMetadata();
    }

    await metadataPromise;
}

function populateMetadata() {
    const namesList =
        document.getElementById("semester-names-list");

    if (namesList) {
        namesList.innerHTML = metadata.nameSuggestions
            .map(name => `
                <option value="${AdminUi.escapeHtml(name)}"></option>
            `)
            .join("");
    }

    populateSelect(
        "filter-academicYearId",
        metadata.academicYears,
        "Wszystkie lata"
    );

    populateSelect(
        "form-academicYearId",
        metadata.academicYears,
        "Wybierz rok akademicki"
    );
}

function populateSelect(selectId, items, defaultText) {
    const select = document.getElementById(selectId);

    if (!select) {
        return;
    }

    const currentValue = select.value;

    select.innerHTML = `
        <option value="">
            ${AdminUi.escapeHtml(defaultText)}
        </option>
    ` + items
        .map(item => `
            <option value="${item.id}">
                ${AdminUi.escapeHtml(item.text)}
            </option>
        `)
        .join("");

    if (currentValue) {
        select.value = currentValue;
    }
}

function getFilterPayload() {
    return {
        name: document
            .getElementById("filter-name-input")
            ?.value
            .trim() ?? "",

        academicYearId: AdminUi.getNullableIntValue(
            "filter-academicYearId"
        )
    };
}

function buildQueryString(filters) {
    const params = new URLSearchParams();

    if (filters.name) {
        params.set("name", filters.name);
    }

    if (filters.academicYearId !== null) {
        params.set(
            "academicYearId",
            filters.academicYearId.toString()
        );
    }

    return params.toString();
}

async function applyFilters() {
    await loadSemesters();
}

function clearFilters() {
    document.getElementById("filter-name-input").value = "";
    document.getElementById("filter-academicYearId").value = "";

    allSemesters = null;
    renderTable();
}

async function loadSemesters() {
    const tbody =
        document.getElementById("semesters-rows");

    if (tbody) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Ładowanie...
                </td>
            </tr>
        `;
    }

    const queryString =
        buildQueryString(getFilterPayload());

    const url = queryString
        ? `${API_URL}?${queryString}`
        : API_URL;

    try {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać listy semestrów."
            );
        }

        allSemesters = await response.json();
        renderTable();
    } catch (error) {
        console.error(
            "Błąd pobierania semestrów:",
            error
        );

        allSemesters = [];
        renderTable();

        alert(error.message);
    }
}

function renderTable() {
    const tbody =
        document.getElementById("semesters-rows");

    if (!tbody) {
        return;
    }

    if (allSemesters === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Wybierz filtry i kliknij „Filtruj”,
                    aby wyświetlić semestry.
                </td>
            </tr>
        `;

        return;
    }

    if (allSemesters.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Brak semestrów dla wybranych filtrów.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allSemesters
        .map(semester => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20
                       transition-colors">

                <td class="p-4 text-sm font-semibold
                           text-gray-900 dark:text-gray-100">
                    ${AdminUi.escapeHtml(semester.name)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${AdminUi.escapeHtml(
            semester.academicYearName
        )}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${formatDate(semester.startDate)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${formatDate(semester.endDate)}
                </td>

                <td class="p-4 text-sm text-right space-x-2">
                    <button type="button"
                            onclick="openEditModal(${semester.id})"
                            class="font-medium text-blue-600 hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button type="button"
                            onclick="deleteSemester(${semester.id})"
                            class="font-medium text-red-600 hover:underline">
                        Usuń
                    </button>
                </td>
            </tr>
        `)
        .join("");
}

function formatDate(value) {
    if (!value) {
        return "Brak";
    }

    const parts = value.split("-");

    if (parts.length !== 3) {
        return AdminUi.escapeHtml(value);
    }

    return `${parts[2]}.${parts[1]}.${parts[0]}`;
}

function showCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}

function closeCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}

function resetModal() {
    document.getElementById("semester-form").reset();
    document.getElementById("form-id").value = "";

    populateMetadata();
}

async function openCreateModal() {
    try {
        await ensureMetadataLoaded();

        resetModal();

        document.getElementById("modal-title").innerText =
            "Dodaj semestr";

        showCrudModal();
    } catch (error) {
        alert(error.message);
    }
}

async function openEditModal(id) {
    try {
        await ensureMetadataLoaded();

        const response = await fetch(`${API_URL}/${id}`);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać danych semestru."
            );
        }

        const semester = await response.json();

        populateMetadata();

        document.getElementById("modal-title").innerText =
            "Edytuj semestr";

        document.getElementById("form-id").value =
            semester.id;

        document.getElementById("form-name").value =
            semester.name ?? "";

        document.getElementById("form-academicYearId").value =
            semester.academicYearId?.toString() ?? "";

        document.getElementById("form-startDate").value =
            semester.startDate ?? "";

        document.getElementById("form-endDate").value =
            semester.endDate ?? "";

        showCrudModal();
    } catch (error) {
        console.error(
            "Błąd otwierania edycji semestru:",
            error
        );

        alert(error.message);
    }
}

function getFormPayload() {
    return {
        id: Number.parseInt(
            document.getElementById("form-id").value,
            10
        ) || 0,

        name: document
            .getElementById("form-name")
            .value
            .trim(),

        academicYearId: Number.parseInt(
            document.getElementById("form-academicYearId").value,
            10
        ),

        startDate:
        document.getElementById("form-startDate").value,

        endDate:
        document.getElementById("form-endDate").value
    };
}

function validatePayload(payload) {
    if (!payload.name) {
        alert("Nazwa semestru jest wymagana.");
        return false;
    }

    if (Number.isNaN(payload.academicYearId)) {
        alert("Wybierz rok akademicki.");
        return false;
    }

    if (!payload.startDate || !payload.endDate) {
        alert("Podaj datę rozpoczęcia i zakończenia.");
        return false;
    }

    if (payload.endDate <= payload.startDate) {
        alert(
            "Data zakończenia musi być późniejsza " +
            "od daty rozpoczęcia."
        );

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
            const message =
                await readErrorMessage(response);

            throw new Error(message);
        }

        closeCrudModal();
        await loadSemesters();

        metadataPromise = loadMetadata();
        await metadataPromise;
    } catch (error) {
        console.error(
            "Błąd zapisu semestru:",
            error
        );

        alert(error.message);
    }
}

async function deleteSemester(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć ten semestr?"
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
            const message =
                await readErrorMessage(response);

            throw new Error(message);
        }

        allSemesters =
            allSemesters?.filter(
                semester => semester.id !== id
            ) ?? [];

        renderTable();

        metadataPromise = loadMetadata();
        await metadataPromise;
    } catch (error) {
        console.error(
            "Błąd usuwania semestru:",
            error
        );

        alert(error.message);
    }
}

async function readErrorMessage(response) {
    try {
        const body = await response.json();

        if (body.message) {
            return body.message;
        }

        if (body.errors) {
            return Object.values(body.errors)
                .flat()
                .join("\n");
        }
    } catch {
    }

    if (response.status === 409) {
        return "Nie można usunąć semestru, ponieważ jest używany.";
    }

    return "Operacja nie powiodła się.";
}