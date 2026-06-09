const API_URL = "/api/specializations";

let allSpecializations = null;
let metadataPromise = null;

let metadata = {
    nameSuggestions: []
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
                "Nie udało się pobrać danych pomocniczych specjalizacji."
            );
        }

        metadata = await response.json();
        metadata.nameSuggestions ??= [];

        populateNameSuggestions();
    } catch (error) {
        metadataPromise = null;

        console.error(
            "Błąd pobierania metadata specjalizacji:",
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

function populateNameSuggestions() {
    const list = document.getElementById(
        "specialization-names-list"
    );

    if (!list) {
        return;
    }

    list.innerHTML = metadata.nameSuggestions
        .map(name => `
            <option value="${AdminUi.escapeHtml(name)}"></option>
        `)
        .join("");
}

function getFilterValue() {
    return document
        .getElementById("filter-name-input")
        ?.value
        .trim() ?? "";
}

async function applyFilters() {
    await loadSpecializations();
}

function clearFilters() {
    document.getElementById(
        "filter-name-input"
    ).value = "";

    allSpecializations = null;
    renderTable();
}

async function loadSpecializations() {
    const tbody = document.getElementById(
        "specializations-rows"
    );

    if (tbody) {
        tbody.innerHTML = `
            <tr>
                <td colspan="2"
                    class="p-8 text-center text-gray-500">
                    Ładowanie...
                </td>
            </tr>
        `;
    }

    const name = getFilterValue();

    const url = name
        ? `${API_URL}?name=${encodeURIComponent(name)}`
        : API_URL;

    try {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać listy specjalizacji."
            );
        }

        allSpecializations = await response.json();
        renderTable();
    } catch (error) {
        console.error(
            "Błąd pobierania specjalizacji:",
            error
        );

        allSpecializations = [];
        renderTable();

        alert(error.message);
    }
}

function renderTable() {
    const tbody = document.getElementById(
        "specializations-rows"
    );

    if (!tbody) {
        return;
    }

    if (allSpecializations === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="2"
                    class="p-8 text-center text-gray-500">
                    Wpisz nazwę i kliknij „Filtruj”,
                    aby wyświetlić specjalizacje.
                </td>
            </tr>
        `;

        return;
    }

    if (allSpecializations.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="2"
                    class="p-8 text-center text-gray-500">
                    Brak specjalizacji dla wybranego filtra.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allSpecializations
        .map(specialization => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20
                       transition-colors">

                <td class="p-4 text-sm font-semibold
                           text-gray-900 dark:text-gray-100">
                    ${AdminUi.escapeHtml(
            specialization.name
        )}
                </td>

                <td class="p-4 text-sm text-right space-x-2">
                    <button type="button"
                            onclick="openEditModal(${specialization.id})"
                            class="font-medium text-blue-600 hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button type="button"
                            onclick="deleteSpecialization(${specialization.id})"
                            class="font-medium text-red-600 hover:underline">
                        Usuń
                    </button>
                </td>
            </tr>
        `)
        .join("");
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
    document
        .getElementById("specialization-form")
        .reset();

    document.getElementById("form-id").value = "";
}

async function openCreateModal() {
    try {
        await ensureMetadataLoaded();

        resetModal();

        document.getElementById("modal-title").innerText =
            "Dodaj specjalizację";

        showCrudModal();
    } catch (error) {
        alert(error.message);
    }
}

async function openEditModal(id) {
    try {
        const response = await fetch(
            `${API_URL}/${id}`
        );

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać danych specjalizacji."
            );
        }

        const specialization =
            await response.json();

        document.getElementById("modal-title").innerText =
            "Edytuj specjalizację";

        document.getElementById("form-id").value =
            specialization.id;

        document.getElementById("form-name").value =
            specialization.name ?? "";

        showCrudModal();
    } catch (error) {
        console.error(
            "Błąd otwierania edycji specjalizacji:",
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
            .trim()
    };
}

function validatePayload(payload) {
    if (!payload.name) {
        alert("Nazwa specjalizacji jest wymagana.");
        return false;
    }

    if (payload.name.length > 100) {
        alert(
            "Nazwa specjalizacji nie może przekraczać 100 znaków."
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
            throw new Error(
                await readErrorMessage(response)
            );
        }

        closeCrudModal();
        await loadSpecializations();

        metadataPromise = loadMetadata();
        await metadataPromise;
    } catch (error) {
        console.error(
            "Błąd zapisu specjalizacji:",
            error
        );

        alert(error.message);
    }
}

async function deleteSpecialization(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć tę specjalizację?"
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
                await readErrorMessage(response)
            );
        }

        allSpecializations =
            allSpecializations?.filter(
                specialization =>
                    specialization.id !== id
            ) ?? [];

        renderTable();

        metadataPromise = loadMetadata();
        await metadataPromise;
    } catch (error) {
        console.error(
            "Błąd usuwania specjalizacji:",
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
        // Odpowiedź nie była JSON-em.
    }

    if (response.status === 409) {
        return "Operacja jest niemożliwa ze względu na powiązane dane.";
    }

    return "Operacja nie powiodła się.";
}