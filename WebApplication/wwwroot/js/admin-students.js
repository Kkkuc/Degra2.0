const API_URL = "/api/students";

let allStudents = null;
let metadataPromise = null;
let selectedGroupIds = new Set();

let metadata = {
    studentSuggestions: [],
    groups: []
};

document.addEventListener(
    "DOMContentLoaded",
    initializePage
);

async function initializePage() {
    try {
        metadataPromise = loadMetadata();

        await metadataPromise;
        //await loadStudents();
    } catch (error) {
        console.error(
            "Błąd inicjalizacji panelu studentów:",
            error
        );

        alert("Nie udało się uruchomić panelu studentów.");
    }
}

async function loadMetadata() {
    try {
        const response = await fetch(`${API_URL}/metadata`);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać danych pomocniczych studentów."
            );
        }

        metadata = await response.json();

        metadata.studentSuggestions ??= [];
        metadata.groups ??= [];

        populateMetadata();
    } catch (error) {
        metadataPromise = null;

        console.error(
            "Błąd pobierania metadata studentów:",
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
    const suggestions =
        document.getElementById("student-suggestions");

    if (suggestions) {
        suggestions.innerHTML =
            metadata.studentSuggestions
                .map(item => `
                    <option value="${AdminUi.escapeHtml(item)}">
                    </option>
                `)
                .join("");
    }

    populateGroupFilter();
}

function getGroupLabel(group) {
    const parts = [
        group.name,
        group.fieldOfStudyName,
        group.semesterName,
        group.classType
    ];

    if (group.specializationName) {
        parts.push(group.specializationName);
    }

    return parts.filter(Boolean).join(" — ");
}

function populateGroupFilter() {
    const select =
        document.getElementById("filter-groupId");

    if (!select) {
        return;
    }

    const currentValue = select.value;

    select.innerHTML = `
        <option value="">Wszystkie grupy</option>
    ` + metadata.groups
        .map(group => `
            <option value="${group.id}">
                ${AdminUi.escapeHtml(getGroupLabel(group))}
            </option>
        `)
        .join("");

    select.value = currentValue;
}

function buildQueryString() {
    const params = new URLSearchParams();

    const search = document
        .getElementById("filter-search")
        .value
        .trim();

    const groupId = document
        .getElementById("filter-groupId")
        .value;

    if (search) {
        params.set("search", search);
    }

    if (groupId) {
        params.set("groupId", groupId);
    }

    return params.toString();
}

async function applyFilters() {
    await loadStudents();
}

function clearFilters() {
    document.getElementById("filter-search").value = "";
    document.getElementById("filter-groupId").value = "";

    allStudents = null;
    renderTable();
}

async function loadStudents() {
    const queryString = buildQueryString();

    const response = await fetch(
        queryString
            ? `${API_URL}?${queryString}`
            : API_URL
    );

    if (!response.ok) {
        alert("Nie udało się pobrać studentów.");
        return;
    }

    allStudents = await response.json();
    renderTable();
}

function renderTable() {
    const tbody =
        document.getElementById("students-rows");

    if (allStudents === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Wybierz filtry i kliknij „Filtruj”,
                    aby wyświetlić studentów.
                </td>
            </tr>
        `;
        return;
    }

    if (allStudents.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5"
                    class="p-8 text-center text-gray-500">
                    Brak studentów dla wybranych filtrów.
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = allStudents
        .map(student => `
            <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20">

                <td class="p-4 text-sm font-semibold">
                    ${AdminUi.escapeHtml(student.studentId)}
                </td>

                <td class="p-4 text-sm">
                    ${AdminUi.escapeHtml(student.firstName)}
                </td>

                <td class="p-4 text-sm">
                    ${AdminUi.escapeHtml(student.lastName)}
                </td>

                <td class="p-4 text-sm">
                    <button type="button"
                            onclick="openGroupsPreview(${student.id})"
                            class="font-medium text-indigo-600 hover:underline">
                        ${student.groupsCount}
                        ${student.groupsCount === 1 ? "grupa" : "grup"}
                    </button>
                </td>

                <td class="p-4 text-right text-sm space-x-2">
                    <button onclick="openEditModal(${student.id})"
                            class="font-medium text-blue-600 hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button onclick="deleteStudent(${student.id})"
                            class="font-medium text-red-600 hover:underline">
                        Usuń
                    </button>
                </td>

            </tr>
        `)
        .join("");
}

function renderGroupCheckboxes() {
    const container =
        document.getElementById("form-groups-list");

    const search = document
        .getElementById("group-search-input")
        .value
        .trim()
        .toLowerCase();

    const groups = metadata.groups.filter(group =>
        getGroupLabel(group)
            .toLowerCase()
            .includes(search)
    );

    if (groups.length === 0) {
        container.innerHTML = `
            <p class="py-4 text-center text-sm text-gray-500">
                Nie znaleziono grup.
            </p>
        `;
        return;
    }

    container.innerHTML = groups
        .map(group => {
            const checked =
                selectedGroupIds.has(group.id)
                    ? "checked"
                    : "";

            return `
                <label class="flex cursor-pointer items-start gap-3
                              rounded-lg border p-3 hover:bg-gray-50
                              dark:border-gray-700 dark:hover:bg-gray-800">

                    <input type="checkbox"
                           value="${group.id}"
                           ${checked}
                           onchange="toggleGroup(${group.id}, this.checked)"
                           class="mt-1 rounded border-gray-300" />

                    <span>
                        <span class="block text-sm font-semibold">
                            ${AdminUi.escapeHtml(group.name)}
                        </span>

                        <span class="block text-xs text-gray-500">
                            ${AdminUi.escapeHtml(getGroupLabel(group))}
                        </span>
                    </span>
                </label>
            `;
        })
        .join("");

    updateSelectedGroupsCount();
}

function toggleGroup(groupId, isChecked) {
    if (isChecked) {
        selectedGroupIds.add(groupId);
    } else {
        selectedGroupIds.delete(groupId);
    }

    updateSelectedGroupsCount();
}

function updateSelectedGroupsCount() {
    document.getElementById(
        "selected-groups-count"
    ).innerText =
        `Wybrano: ${selectedGroupIds.size}`;
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
    try {
        await ensureMetadataLoaded();

        document
            .getElementById("student-form")
            .reset();

        document.getElementById("form-id").value = "";

        document.getElementById("modal-title").innerText =
            "Dodaj studenta";

        document.getElementById(
            "group-search-input"
        ).value = "";

        selectedGroupIds = new Set();

        renderGroupCheckboxes();
        showCrudModal();
    } catch (error) {
        console.error(
            "Błąd otwierania formularza studenta:",
            error
        );

        alert(error.message);
    }
}

async function openEditModal(id) {
    await ensureMetadataLoaded();

    const response = await fetch(`${API_URL}/${id}`);

    if (!response.ok) {
        alert("Nie udało się pobrać studenta.");
        return;
    }

    const student = await response.json();

    document.getElementById("modal-title").innerText =
        "Edytuj studenta";

    document.getElementById("form-id").value =
        student.id;

    document.getElementById("form-firstName").value =
        student.firstName ?? "";

    document.getElementById("form-lastName").value =
        student.lastName ?? "";

    document.getElementById("form-studentId").value =
        student.studentId ?? "";

    selectedGroupIds = new Set(
        student.groupIds ?? []
    );

    document.getElementById(
        "group-search-input"
    ).value = "";

    renderGroupCheckboxes();
    showCrudModal();
}

function getFormPayload() {
    return {
        id: Number.parseInt(
            document.getElementById("form-id").value,
            10
        ) || 0,

        firstName: document
            .getElementById("form-firstName")
            .value
            .trim(),

        lastName: document
            .getElementById("form-lastName")
            .value
            .trim(),

        studentId: document
            .getElementById("form-studentId")
            .value
            .trim(),

        groupIds: Array.from(selectedGroupIds)
    };
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

        await loadStudents();

        metadataPromise = loadMetadata();
        await metadataPromise;
    } catch (error) {
        console.error(
            "Błąd zapisu studenta:",
            error
        );

        alert(error.message);
    }
}

async function openGroupsPreview(id) {
    const response = await fetch(`${API_URL}/${id}`);

    if (!response.ok) {
        alert("Nie udało się pobrać grup studenta.");
        return;
    }

    const student = await response.json();

    document.getElementById(
        "groups-preview-student"
    ).innerText =
        `${student.studentId} — ${student.firstName} ${student.lastName}`;

    const container = document.getElementById(
        "groups-preview-content"
    );

    if (!student.groups?.length) {
        container.innerHTML = `
            <p class="py-6 text-center text-gray-500">
                Student nie jest przypisany do żadnej grupy.
            </p>
        `;
    } else {
        container.innerHTML = student.groups
            .map(group => `
                <div class="rounded-lg border p-4 dark:border-gray-700">
                    <div class="font-semibold">
                        ${AdminUi.escapeHtml(group.name)}
                    </div>

                    <div class="mt-1 text-sm text-gray-500">
                        ${AdminUi.escapeHtml(group.fieldOfStudyName)}
                        · ${AdminUi.escapeHtml(group.semesterName)}
                        · ${AdminUi.escapeHtml(group.classType)}
                    </div>

                    ${
                group.specializationName
                    ? `<div class="mt-1 text-sm text-gray-500">
                                   ${AdminUi.escapeHtml(group.specializationName)}
                               </div>`
                    : ""
            }
                </div>
            `)
            .join("");
    }

    const modal = document.getElementById(
        "groups-preview-modal"
    );

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}

function closeGroupsPreviewModal() {
    const modal = document.getElementById(
        "groups-preview-modal"
    );

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}

async function deleteStudent(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć studenta?"
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

    allStudents =
        allStudents?.filter(student =>
            student.id !== id)
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
    }

    return "Operacja nie powiodła się.";
}

function validatePayload(payload) {
    if (!payload.firstName) {
        alert("Imię jest wymagane.");
        return false;
    }

    if (!payload.lastName) {
        alert("Nazwisko jest wymagane.");
        return false;
    }

    if (!payload.studentId) {
        alert("Numer albumu jest wymagany.");
        return false;
    }

    return true;
}