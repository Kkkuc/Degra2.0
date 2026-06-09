const API_URL = "/api/buildings";

let allBuildings = null;
let metadataPromise = null;
let cachedFaculties = [];

document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
    AdminUi.wireDatalistInput("filter-address-input", "filter-address-id", "addresses-list");
    AdminUi.wireDatalistInput("filter-faculty-input", "filter-faculty-id", "faculties-list");

    metadataPromise = loadMetadata();
    renderTable();
}

async function applyFilters() {
    await loadBuildings();
}


function getFilterPayload() {
    return {
        name: document.getElementById("filter-name-input")?.value ?? "",
        addressId: AdminUi.getNullableIntValue("filter-address-id"),
        facultyId: AdminUi.getNullableIntValue("filter-faculty-id")
    };
}

function getAddressDisplay(building) {
    return `${building.street} ${building.houseNumber}, ${building.postalCode} ${building.city}`;
}

function addDatalistOptions(listId, values) {
    const list = document.getElementById(listId);
    if (!list) {
        return;
    }

    list.innerHTML = values.map(value => `<option value="${value}"></option>`).join("");
}

function addOptionDatalist(listId, items) {
    const list = document.getElementById(listId);
    if (!list) {
        return;
    }

    list.innerHTML = items.map(item => `<option value="${item.text}" data-id="${item.id}"></option>`).join("");
}

function addFacultyOptions(listId, faculties) {
    const list = document.getElementById(listId);
    if (!list) {
        return;
    }

    list.innerHTML = faculties.map(faculty => `<option value="${faculty.text}" data-id="${faculty.id}"></option>`).join("");
}

function populateFacultySelect() {
    const select = document.getElementById("form-facultyId");
    if (!select) {
        return;
    }

    const currentValue = select.value;
    select.innerHTML = `<option value="">Wybierz wydział</option>` + cachedFaculties.map(faculty => `
        <option value="${faculty.id}">${faculty.text}</option>
    `).join("");

    if (currentValue) {
        select.value = currentValue;
    }
}

async function loadMetadata() {
    try {
        const response = await fetch(`${API_URL}/metadata`);

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać danych pomocniczych budynków."
            );
        }

        const metadata = await response.json();

        cachedFaculties = metadata.faculties ?? [];

        addDatalistOptions(
            "buildings-list",
            metadata.nameSuggestions ?? []
        );

        addOptionDatalist(
            "addresses-list",
            metadata.addressSuggestions ?? []
        );

        addFacultyOptions(
            "faculties-list",
            cachedFaculties
        );

        populateFacultySelect();
    } catch (error) {
        console.error(
            "Błąd pobierania metadata budynków:",
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

function renderTable() {
    const tbody = document.getElementById("buildings-rows");
    if (!tbody) {
        return;
    }

    if (allBuildings === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="p-8 text-center text-gray-500">
                    Wybierz filtry i kliknij "Filtruj", aby wyświetlić budynki.
                </td>
            </tr>
        `;
        return;
    }

    if (allBuildings.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="p-8 text-center text-gray-500">
                    Brak budynków dla wybranych filtrów.
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = allBuildings.map(building => `
        <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20 transition-colors">
            <td class="p-4 text-sm font-semibold text-gray-900 dark:text-gray-100">${building.name}</td>
            <td class="p-4 text-sm text-gray-600 dark:text-gray-400">${getAddressDisplay(building)}</td>
            <td class="p-4 text-sm text-gray-500">
                <span class="px-2 py-0.5 bg-gray-100 dark:bg-gray-800 rounded text-xs font-medium">
                    ${building.facultyAbbreviation}
                </span>
            </td>
            <td class="p-4 text-sm text-right space-x-2">
                <button onclick="openEditModal(${building.id})" class="text-blue-600 hover:underline font-medium">Edytuj</button>
                <span class="text-gray-300">|</span>
                <button onclick="deleteBuilding(${building.id})" class="text-red-600 hover:underline font-medium">Usuń</button>
            </td>
        </tr>
    `).join("");
}

async function loadBuildings() {
    const filters = getFilterPayload();
    const params = new URLSearchParams();

    if (filters.name) params.set("name", filters.name);
    if (filters.addressId !== null) params.set("addressId", filters.addressId);
    if (filters.facultyId !== null) params.set("facultyId", filters.facultyId);

    const tbody = document.getElementById("buildings-rows");
    tbody.innerHTML = `<tr><td colspan="4" class="p-8 text-center">Ładowanie...</td></tr>`;

    try {
        const response = await fetch(`${API_URL}?${params.toString()}`);
        if (!response.ok) throw new Error("Błąd ładowania");
        allBuildings = await response.json();
        renderTable();
    } catch (err) {
        console.error(err);
    }
}

function resetModal() {
    document.getElementById("building-form").reset();
    document.getElementById("form-id").value = "";
    populateFacultySelect();
}

async function openCreateModal() {
    await ensureMetadataLoaded();

    document.getElementById("modal-title").innerText =
        "Dodaj budynek";

    resetModal();

    showCrudModal();
}


// --- Obsługa Modali ---
async function openEditModal(id) {
    await ensureMetadataLoaded();

    try {
        const response = await fetch(`${API_URL}/${id}`);

        if (!response.ok) {
            alert("Nie udało się pobrać danych budynku.");
            return;
        }

        const building = await response.json();

        document.getElementById("modal-title").innerText = "Edytuj budynek";
        document.getElementById("form-id").value = building.id;
        document.getElementById("form-name").value = building.name ?? "";
        document.getElementById("form-facultyId").value =
            building.facultyId?.toString() ?? "";

        document.getElementById("form-street").value =
            building.addressDto?.street ?? "";

        document.getElementById("form-houseNumber").value =
            building.addressDto?.houseNumber ?? "";

        document.getElementById("form-postalCode").value =
            building.addressDto?.postalCode ?? "";

        document.getElementById("form-city").value =
            building.addressDto?.city ?? "";

        showCrudModal();
    } catch (error) {
        console.error("Błąd pobierania budynku:", error);
        alert("Wystąpił błąd podczas pobierania danych budynku.");
    }
}

function closeCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const idValue =
        document.getElementById("form-id").value;

    const payload = {
        id: Number.parseInt(idValue, 10) || 0,

        name: document
            .getElementById("form-name")
            .value
            .trim(),

        facultyId: Number.parseInt(
            document.getElementById("form-facultyId").value,
            10
        ),

        addressDto: {
            street: document
                .getElementById("form-street")
                .value
                .trim(),

            houseNumber: document
                .getElementById("form-houseNumber")
                .value
                .trim(),

            postalCode: document
                .getElementById("form-postalCode")
                .value
                .trim(),

            city: document
                .getElementById("form-city")
                .value
                .trim()
        }
    };

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
            const errorBody = await response.text();

            console.error(
                "Błąd zapisu budynku:",
                response.status,
                errorBody
            );

            alert("Nie udało się zapisać budynku.");
            return;
        }

        closeCrudModal();
        await loadBuildings();
        await loadMetadata();
    } catch (error) {
        console.error("Błąd zapisu budynku:", error);
        alert("Wystąpił błąd podczas zapisywania budynku.");
    }
}

async function deleteBuilding(id) {
    if (!confirm("Czy na pewno chcesz usunąć ten budynek?")) {
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
            const responseBody = await response.text();

            console.error(
                "Błąd usuwania budynku:",
                response.status,
                responseBody
            );

            alert(
                "Nie udało się usunąć budynku. " +
                "Możliwe, że są do niego przypisane sale."
            );

            return;
        }

        allBuildings =
            allBuildings?.filter(
                building => building.id !== id
            ) ?? [];

        renderTable();
        await loadMetadata();
    } catch (error) {
        console.error(
            "Błąd usuwania budynku:",
            error
        );

        alert(
            "Wystąpił błąd podczas usuwania budynku."
        );
    }
}

function showCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}