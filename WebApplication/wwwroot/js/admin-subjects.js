const API_URL = "/api/subjects";

let allSubjects = null;
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
        //await loadSubjects();
    } catch (error) {
        console.error(
            "Błąd inicjalizacji panelu przedmiotów:",
            error
        );
    }
}

async function loadMetadata() {
    const response = await fetch(`${API_URL}/metadata`);

    if (!response.ok) {
        throw new Error(
            "Nie udało się pobrać danych pomocniczych przedmiotów."
        );
    }

    metadata = await response.json();
    metadata.suggestions ??= [];

    const list =
        document.getElementById("subject-suggestions");

    if (list) {
        list.innerHTML = metadata.suggestions
            .map(value => `
                <option value="${AdminUi.escapeHtml(value)}"></option>
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
    await loadSubjects();
}

function clearFilters() {
    document.getElementById("filter-search").value = "";
    loadSubjects();
}

async function loadSubjects() {
    const search = document
        .getElementById("filter-search")
        .value
        .trim();

    const url = search
        ? `${API_URL}?search=${encodeURIComponent(search)}`
        : API_URL;

    const response = await fetch(url);

    if (!response.ok) {
        alert("Nie udało się pobrać przedmiotów.");
        return;
    }

    allSubjects = await response.json();
    renderTable();
}

function renderTable() {
    const tbody =
        document.getElementById("subjects-rows");

    if (!tbody) {
        return;
    }

    if (!allSubjects?.length) {
        tbody.innerHTML = `
            <tr>
                <td colspan="4"
                    class="p-8 text-center text-gray-500">
                    Brak przedmiotów.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allSubjects
        .map(subject => `
            <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20">

                <td class="p-4 text-sm font-semibold">
                    ${AdminUi.escapeHtml(subject.name)}
                </td>

                <td class="p-4 text-sm text-gray-600">
                    ${AdminUi.escapeHtml(subject.abbreviation ?? "Brak")}
                </td>

                <td class="p-4 text-sm text-gray-600">
                    ${AdminUi.escapeHtml(subject.code ?? "Brak")}
                </td>

                <td class="p-4 text-right text-sm space-x-2">
                    <button onclick="openEditModal(${subject.id})"
                            class="font-medium text-blue-600 hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button onclick="deleteSubject(${subject.id})"
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

async function openCreateModal() {
    await ensureMetadataLoaded();

    document.getElementById("subject-form").reset();
    document.getElementById("form-id").value = "";
    document.getElementById("modal-title").innerText =
        "Dodaj przedmiot";

    showCrudModal();
}

async function openEditModal(id) {
    const response = await fetch(`${API_URL}/${id}`);

    if (!response.ok) {
        alert("Nie udało się pobrać przedmiotu.");
        return;
    }

    const subject = await response.json();

    document.getElementById("modal-title").innerText =
        "Edytuj przedmiot";

    document.getElementById("form-id").value =
        subject.id;

    document.getElementById("form-name").value =
        subject.name ?? "";

    document.getElementById("form-abbreviation").value =
        subject.abbreviation ?? "";

    document.getElementById("form-code").value =
        subject.code ?? "";

    showCrudModal();
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

        abbreviation: document
            .getElementById("form-abbreviation")
            .value
            .trim(),

        code: document
            .getElementById("form-code")
            .value
            .trim()
    };
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const payload = getFormPayload();

    if (!payload.name) {
        alert("Nazwa przedmiotu jest wymagana.");
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
    await loadSubjects();

    metadataPromise = loadMetadata();
    await metadataPromise;
}

async function deleteSubject(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć ten przedmiot?"
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

    allSubjects =
        allSubjects?.filter(subject =>
            subject.id !== id)
        ?? [];

    renderTable();
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
        // Brak JSON.
    }

    return "Operacja nie powiodła się.";
}