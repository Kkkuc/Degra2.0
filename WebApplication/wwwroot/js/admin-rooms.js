const API_URL = "/api/admin/rooms";

let allRooms = null;
let metadataPromise = null;

let metadata = {
    roomSuggestions: [],
    buildings: []
};

document.addEventListener(
    "DOMContentLoaded",
    initializePage
);

async function initializePage() {
    try {
        AdminUi.wireDatalistInput(
            "filter-search",
            "filter-room-id",
            "room-suggestions"
        );

        metadataPromise = loadMetadata();

        await metadataPromise;
        await loadRooms();
    } catch (error) {
        console.error(
            "Błąd inicjalizacji panelu sal:",
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
            "Nie udało się pobrać danych pomocniczych sal."
        );
    }

    metadata = await response.json();

    metadata.roomSuggestions ??= [];
    metadata.buildings ??= [];

    populateMetadata();
}

async function ensureMetadataLoaded() {
    if (!metadataPromise) {
        metadataPromise = loadMetadata();
    }

    await metadataPromise;
}

function populateMetadata() {
    const suggestions =
        document.getElementById("room-suggestions");

    if (suggestions) {
        suggestions.innerHTML =
            metadata.roomSuggestions
                .map(item => `
                    <option
                        value="${AdminUi.escapeHtml(item.text)}"
                        data-id="${item.id}">
                    </option>
                `)
                .join("");
    }

    populateBuildingSelect(
        "filter-buildingId",
        "Wszystkie budynki"
    );

    populateBuildingSelect(
        "form-buildingId",
        "Wybierz budynek"
    );
}

function populateBuildingSelect(
    selectId,
    defaultText
) {
    const select =
        document.getElementById(selectId);

    if (!select) {
        return;
    }

    const currentValue = select.value;

    select.innerHTML = `
        <option value="">
            ${AdminUi.escapeHtml(defaultText)}
        </option>
    ` + metadata.buildings
        .map(building => `
            <option value="${building.id}">
                ${AdminUi.escapeHtml(building.text)}
            </option>
        `)
        .join("");

    if (currentValue) {
        select.value = currentValue;
    }
}

async function applyFilters() {
    await loadRooms();
}

function clearFilters() {
    document.getElementById("filter-search").value = "";
    document.getElementById("filter-room-id").value = "";
    document.getElementById("filter-buildingId").value = "";

    loadRooms();
}

async function loadRooms() {
    const search = document
        .getElementById("filter-search")
        .value
        .trim();

    const roomId = document
        .getElementById("filter-room-id")
        .value;

    const buildingId = document
        .getElementById("filter-buildingId")
        .value;

    const params = new URLSearchParams();

    if (roomId) {
        params.set("roomId", roomId);
    } else if (search) {
        params.set("search", search);
    }

    if (buildingId) {
        params.set("buildingId", buildingId);
    }

    const queryString = params.toString();

    const response = await fetch(
        queryString
            ? `${API_URL}?${queryString}`
            : API_URL
    );

    if (!response.ok) {
        alert("Nie udało się pobrać sal.");
        return;
    }

    allRooms = await response.json();
    renderTable();
}

function renderTable() {
    const tbody =
        document.getElementById("rooms-rows");

    if (!tbody) {
        return;
    }

    if (!allRooms?.length) {
        tbody.innerHTML = `
            <tr>
                <td colspan="3"
                    class="p-8 text-center text-gray-500">
                    Brak sal.
                </td>
            </tr>
        `;

        return;
    }

    tbody.innerHTML = allRooms
        .map(room => `
            <tr class="hover:bg-gray-50/50
                       dark:hover:bg-gray-800/20">

                <td class="p-4 text-sm font-semibold">
                    ${AdminUi.escapeHtml(room.roomNumber)}
                </td>

                <td class="p-4 text-sm">
                    ${AdminUi.escapeHtml(room.buildingName)}
                </td>

                <td class="p-4 text-right text-sm space-x-2">

                    <button type="button"
                            onclick="openEditModal(${room.id})"
                            class="font-medium text-blue-600
                                   hover:underline">
                        Edytuj
                    </button>

                    <span class="text-gray-300">|</span>

                    <button type="button"
                            onclick="deleteRoom(${room.id})"
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
        .getElementById("room-form")
        .reset();

    document.getElementById("form-id").value = "";

    document.getElementById("modal-title").innerText =
        "Dodaj salę";

    populateBuildingSelect(
        "form-buildingId",
        "Wybierz budynek"
    );

    showCrudModal();
}

async function openEditModal(id) {
    await ensureMetadataLoaded();

    const response = await fetch(
        `${API_URL}/${id}`
    );

    if (!response.ok) {
        alert("Nie udało się pobrać sali.");
        return;
    }

    const room = await response.json();

    populateBuildingSelect(
        "form-buildingId",
        "Wybierz budynek"
    );

    document.getElementById("modal-title").innerText =
        "Edytuj salę";

    document.getElementById("form-id").value =
        room.id;

    document.getElementById("form-roomNumber").value =
        room.roomNumber ?? "";

    document.getElementById("form-buildingId").value =
        room.buildingId?.toString() ?? "";

    showCrudModal();
}

function getFormPayload() {
    return {
        id: Number.parseInt(
            document.getElementById("form-id").value,
            10
        ) || 0,

        roomNumber: document
            .getElementById("form-roomNumber")
            .value
            .trim(),

        buildingId: Number.parseInt(
            document.getElementById("form-buildingId").value,
            10
        )
    };
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const payload = getFormPayload();

    if (!payload.roomNumber) {
        alert("Numer sali jest wymagany.");
        return;
    }

    if (Number.isNaN(payload.buildingId)) {
        alert("Wybierz budynek.");
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
    await loadRooms();

    metadataPromise = loadMetadata();
    await metadataPromise;
}

async function deleteRoom(id) {
    if (!confirm(
        "Czy na pewno chcesz usunąć tę salę?"
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

    allRooms =
        allRooms?.filter(room => room.id !== id)
        ?? [];

    renderTable();

    metadataPromise = loadMetadata();
    await metadataPromise;
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