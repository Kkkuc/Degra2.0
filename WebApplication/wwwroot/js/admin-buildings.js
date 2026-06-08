const API_URL = "/Admin";

let allBuildings = [];

document.addEventListener("DOMContentLoaded", () => {
    loadBuildings();
});

function getFilterPayload() {
    return {
        search: document.getElementById("filter-building-input")?.value ?? "",
        facultyId: AdminUi.getNullableIntValue("filter-faculty-id")
    };
}

function getAddressDisplay(building) {
    return `${building.street} ${building.houseNumber}, ${building.postalCode} ${building.city}`;
}

function renderTable() {
    const tbody = document.getElementById("buildings-rows");
    if (!tbody) {
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
    const params = new URLSearchParams();
    const filters = getFilterPayload();

    if (filters.search) {
        params.set("search", filters.search);
    }

    if (filters.facultyId !== null) {
        params.set("facultyId", filters.facultyId);
    }

    const tbody = document.getElementById("buildings-rows");
    if (tbody) {
        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="p-8 text-center text-gray-500">
                    Ładowanie budynków...
                </td>
            </tr>
        `;
    }

    try {
        const response = await fetch(`${API_URL}/BuildingsData${params.toString() ? `?${params.toString()}` : ""}`);
        if (!response.ok) {
            throw new Error("Nie udało się pobrać budynków.");
        }

        allBuildings = await response.json();
        renderTable();
    } catch (err) {
        console.error(err);
        if (tbody) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="4" class="p-8 text-center text-red-500">
                        Wystąpił błąd podczas ładowania budynków.
                    </td>
                </tr>
            `;
        }
    }
}

function resetModal() {
    document.getElementById("building-form").reset();
    document.getElementById("form-id").value = "";
}

function openCreateModal() {
    document.getElementById("modal-title").innerText = "Dodaj budynek";
    resetModal();
    document.getElementById("crud-modal").classList.remove("hidden");
}

async function openEditModal(id) {
    try {
        const response = await fetch(`${API_URL}/BuildingData?id=${id}`);
        if (!response.ok) {
            return;
        }

        const building = await response.json();

        document.getElementById("modal-title").innerText = "Edytuj budynek";
        document.getElementById("form-id").value = building.id;
        document.getElementById("form-name").value = building.name;
        document.getElementById("form-facultyId").value = building.facultyId;
        document.getElementById("form-street").value = building.addressDto.street;
        document.getElementById("form-houseNumber").value = building.addressDto.houseNumber;
        document.getElementById("form-city").value = building.addressDto.city;
        document.getElementById("form-postalCode").value = building.addressDto.postalCode;
        document.getElementById("crud-modal").classList.remove("hidden");
    } catch (err) {
        console.error(err);
    }
}

function closeCrudModal() {
    document.getElementById("crud-modal").classList.add("hidden");
}

function getFormPayload() {
    return {
        id: Number.parseInt(document.getElementById("form-id").value || "0", 10),
        name: document.getElementById("form-name").value,
        facultyId: Number.parseInt(document.getElementById("form-facultyId").value, 10),
        addressDto: {
            street: document.getElementById("form-street").value,
            houseNumber: document.getElementById("form-houseNumber").value,
            city: document.getElementById("form-city").value,
            postalCode: document.getElementById("form-postalCode").value
        }
    };
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const payload = getFormPayload();
    const isEdit = payload.id > 0;

    try {
        const response = await fetch(isEdit ? `${API_URL}/UpdateBuilding?id=${payload.id}` : `${API_URL}/CreateBuilding`, {
            method: isEdit ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            alert("Wystąpił błąd podczas zapisu budynku.");
            return;
        }

        closeCrudModal();
        await loadBuildings();
    } catch (err) {
        console.error(err);
    }
}

async function deleteBuilding(id) {
    if (!confirm("Usunąć ten budynek?")) {
        return;
    }

    try {
        const response = await fetch(`${API_URL}/DeleteBuilding?id=${id}`, {
            method: "DELETE"
        });

        if (response.ok) {
            await loadBuildings();
        }
    } catch (err) {
        console.error(err);
    }
}
