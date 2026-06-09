const API_URL = "/api/groups";

let allGroups = null;
let metadataPromise = null;

let metadata = {
    nameSuggestions: [],
    semesters: [],
    fieldsOfStudy: [],
    specializations: [],
    classTypes: []
};

document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
    metadataPromise = loadMetadata();
    renderTable();
}

function getNullableIntValue(id) {
    return AdminUi.getNullableIntValue(id);
}

function getFilterPayload() {
    return {
        name: document
            .getElementById("filter-name-input")
            ?.value
            .trim() ?? "",

        semesterId: getNullableIntValue(
            "filter-semesterId"
        ),

        fieldOfStudyId: getNullableIntValue(
            "filter-fieldOfStudyId"
        ),

        specializationId: getNullableIntValue(
            "filter-specializationId"
        ),

        classType: getNullableIntValue(
            "filter-classType"
        )
    };
}

function buildQueryString(filters) {
    const params = new URLSearchParams();

    if (filters.name) {
        params.set("name", filters.name);
    }

    if (filters.semesterId !== null) {
        params.set(
            "semesterId",
            filters.semesterId.toString()
        );
    }

    if (filters.fieldOfStudyId !== null) {
        params.set(
            "fieldOfStudyId",
            filters.fieldOfStudyId.toString()
        );
    }

    if (filters.specializationId !== null) {
        params.set(
            "specializationId",
            filters.specializationId.toString()
        );
    }

    if (filters.classType !== null) {
        params.set(
            "classType",
            filters.classType.toString()
        );
    }

    return params.toString();
}

async function applyFilters() {
    await loadGroups();
}

function clearFilters() {
    document.getElementById("filter-name-input").value = "";
    document.getElementById("filter-semesterId").value = "";
    document.getElementById("filter-fieldOfStudyId").value = "";
    document.getElementById("filter-specializationId").value = "";
    document.getElementById("filter-classType").value = "";

    allGroups = null;
    renderTable();
}

async function loadGroups() {
    const tbody = document.getElementById("groups-rows");

    if (tbody) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6"
                    class="p-8 text-center text-gray-500">
                    Ładowanie...
                </td>
            </tr>
        `;
    }

    const queryString = buildQueryString(
        getFilterPayload()
    );

    const url = queryString
        ? `${API_URL}?${queryString}`
        : API_URL;

    try {
        const response = await fetch(url);

        if (!response.ok) {
            const message = await response.text();

            console.error(
                "Błąd pobierania grup:",
                response.status,
                message
            );

            throw new Error(
                "Nie udało się pobrać listy grup."
            );
        }

        allGroups = await response.json();
        renderTable();
    } catch (error) {
        console.error("Błąd pobierania grup:", error);

        allGroups = [];
        renderTable();

        alert(error.message);
    }
}

async function loadMetadata() {
    try {
        const response = await fetch(`${API_URL}/metadata`);

        if (!response.ok) {
            const body = await response.text();

            console.error(
                "Błąd pobierania danych pomocniczych:",
                response.status,
                body
            );

            throw new Error(
                "Nie udało się pobrać danych formularza grup."
            );
        }

        metadata = await response.json();

        metadata.nameSuggestions ??= [];
        metadata.semesters ??= [];
        metadata.fieldsOfStudy ??= [];
        metadata.specializations ??= [];
        metadata.classTypes ??= [];

        populateMetadata();
    } catch (error) {
        metadataPromise = null;

        console.error(
            "Błąd pobierania danych pomocniczych grup:",
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
    populateNameSuggestions();

    populateSelect(
        "filter-semesterId",
        metadata.semesters,
        "Wszystkie"
    );

    populateSelect(
        "filter-fieldOfStudyId",
        metadata.fieldsOfStudy,
        "Wszystkie"
    );

    populateSelect(
        "filter-specializationId",
        metadata.specializations,
        "Wszystkie"
    );

    populateSelect(
        "filter-classType",
        metadata.classTypes,
        "Wszystkie"
    );

    populateSelect(
        "form-semesterId",
        metadata.semesters,
        "Wybierz semestr"
    );

    populateSelect(
        "form-fieldOfStudyId",
        metadata.fieldsOfStudy,
        "Wybierz kierunek"
    );

    populateSelect(
        "form-specializationId",
        metadata.specializations,
        "Brak specjalizacji"
    );

    populateSelect(
        "form-classType",
        metadata.classTypes,
        "Wybierz typ zajęć"
    );
}

function populateNameSuggestions() {
    const list =
        document.getElementById("group-names-list");

    if (!list) {
        return;
    }

    list.innerHTML = metadata.nameSuggestions
        .map(name => `
            <option value="${AdminUi.escapeHtml(name)}"></option>
        `)
        .join("");
}

function populateSelect(
    selectId,
    items,
    defaultText
) {
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

function renderTable() {
    const tbody = document.getElementById("groups-rows");

    if (!tbody) {
        return;
    }

    if (allGroups === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6"
                    class="p-8 text-center text-gray-500">
                    Wybierz filtry i kliknij „Filtruj”,
                    aby wyświetlić grupy.
                </td>
            </tr>
        `;

        return;
    }

    if (allGroups.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6"
                    class="p-8 text-center text-gray-500">
                    Brak grup dla wybranych filtrów.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allGroups
        .map(group => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20
                       transition-colors">

                <td class="p-4 text-sm font-semibold
                           text-gray-900 dark:text-gray-100">
                    ${AdminUi.escapeHtml(group.name)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${AdminUi.escapeHtml(group.semesterName)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${AdminUi.escapeHtml(group.fieldOfStudyName)}
                </td>

                <td class="p-4 text-sm
                           text-gray-600 dark:text-gray-400">
                    ${AdminUi.escapeHtml(
            group.specializationName ?? "Brak"
        )}
                </td>

                <td class="p-4 text-sm text-gray-500">
                    <span class="rounded bg-gray-100 px-2 py-1
                                 text-xs font-medium
                                 dark:bg-gray-800">
                        ${AdminUi.escapeHtml(
            group.classTypeDisplay
        )}
                    </span>
                </td>

                <td class="p-4 text-sm text-right space-x-2">
                    <button type="button"
                            onclick="openEditModal(${group.id})"
                            class="font-medium text-blue-600 hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button type="button"
                            onclick="deleteGroup(${group.id})"
                            class="font-medium text-red-600 hover:underline">
                        Usuń
                    </button>
                </td>
            </tr>
        `)
        .join("");
}

function resetModal() {
    document.getElementById("group-form").reset();
    document.getElementById("form-id").value = "";

    populateMetadata();
}

function showModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}

function closeCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}

async function openCreateModal() {
    try {
        await ensureMetadataLoaded();

        resetModal();

        document.getElementById("modal-title").innerText =
            "Dodaj grupę";

        showModal();
    } catch (error) {
        alert(error.message);
    }
}

async function openEditModal(id) {
    try {
        await ensureMetadataLoaded();

        const response = await fetch(`${API_URL}/${id}`);

        if (!response.ok) {
            const body = await response.text();

            console.error(
                "Błąd pobierania grupy:",
                response.status,
                body
            );

            throw new Error(
                "Nie udało się pobrać danych grupy."
            );
        }

        const group = await response.json();

        populateMetadata();

        document.getElementById("modal-title").innerText =
            "Edytuj grupę";

        document.getElementById("form-id").value =
            group.id;

        document.getElementById("form-name").value =
            group.name ?? "";

        document.getElementById("form-semesterId").value =
            group.semesterId?.toString() ?? "";

        document.getElementById("form-fieldOfStudyId").value =
            group.fieldOfStudyId?.toString() ?? "";

        document.getElementById("form-specializationId").value =
            group.specializationId?.toString() ?? "";

        document.getElementById("form-classType").value =
            group.classType?.toString() ?? "";

        showModal();
    } catch (error) {
        console.error(
            "Błąd otwierania edycji grupy:",
            error
        );

        alert(error.message);
    }
}

function getFormPayload() {
    const idValue =
        document.getElementById("form-id").value;

    return {
        id: Number.parseInt(idValue, 10) || 0,

        name: document
            .getElementById("form-name")
            .value
            .trim(),

        semesterId: Number.parseInt(
            document.getElementById("form-semesterId").value,
            10
        ),

        fieldOfStudyId: Number.parseInt(
            document.getElementById("form-fieldOfStudyId").value,
            10
        ),

        specializationId: AdminUi.getNullableIntValue(
            "form-specializationId"
        ),

        classType: Number.parseInt(
            document.getElementById("form-classType").value,
            10
        )
    };
}

function validatePayload(payload) {
    if (!payload.name) {
        alert("Nazwa grupy jest wymagana.");
        return false;
    }

    if (Number.isNaN(payload.semesterId)) {
        alert("Wybierz semestr.");
        return false;
    }

    if (Number.isNaN(payload.fieldOfStudyId)) {
        alert("Wybierz kierunek.");
        return false;
    }

    if (Number.isNaN(payload.classType)) {
        alert("Wybierz typ zajęć.");
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
            const responseBody = await response.text();

            console.error(
                "Błąd zapisu grupy:",
                response.status,
                responseBody
            );

            throw new Error(
                "Nie udało się zapisać grupy."
            );
        }

        closeCrudModal();
        await loadGroups();
        await loadMetadata();
    } catch (error) {
        console.error("Błąd zapisu grupy:", error);
        alert(error.message);
    }
}

async function deleteGroup(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć tę grupę?"
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
            const body = await response.text();

            console.error(
                "Błąd usuwania grupy:",
                response.status,
                body
            );

            if (response.status === 409) {
                throw new Error(
                    "Nie można usunąć grupy, ponieważ jest " +
                    "używana w planie zajęć lub ma przypisanych studentów."
                );
            }

            throw new Error(
                "Nie udało się usunąć grupy."
            );
        }

        allGroups =
            allGroups?.filter(group => group.id !== id)
            ?? [];

        renderTable();
        await loadMetadata();
    } catch (error) {
        console.error("Błąd usuwania grupy:", error);
        alert(error.message);
    }
}