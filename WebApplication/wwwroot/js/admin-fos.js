const API_URL = "/api/FieldsOfStudiesApi";
let allFos = [];

const MODE_NAMES = {
    0: "Stacjonarne",
    1: "Niestacjonarne",
    2: "Podyplomowe"
};

document.addEventListener("DOMContentLoaded", () => {
    loadFos();
    loadSuggestions(); // Pobieramy sugestie niezależnie od tabeli
});
document.getElementById("filter-name-input").addEventListener("input", applyFilters);

async function loadSuggestions() {
    try {
        const response = await fetch(`${API_URL}/suggestions`);
        if (!response.ok) return;

        const names = await response.json();
        const datalist = document.getElementById("fos-suggestions");
        datalist.innerHTML = names.map(name => `<option value="${name}">`).join("");
    } catch (err) {
        console.error("Błąd ładowania sugestii:", err);
    }
}

async function loadFos() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error("Błąd ładowania danych");
        allFos = await response.json();
        renderTable(allFos);
    } catch (err) {
        console.error(err);
    }
}

function populateModeSelect() {
    const select = document.getElementById("form-mode");
    select.innerHTML = Object.entries(MODE_NAMES).map(([key, value]) =>
        `<option value="${key}">${value}</option>`
    ).join("");
}

// --- Renderowanie i Filtrowanie ---
function renderTable(data) {
    const tbody = document.getElementById("fos-rows");
    if (!tbody) return;

    tbody.innerHTML = data.map(f => `
        <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20 transition-colors">
            <td class="p-4 text-sm font-semibold">${f.name}</td>
            <td class="p-4 text-sm text-gray-600">${f.facultyAbbreviation}</td> 
            <td class="p-4 text-sm text-gray-600">${f.degree}</td>
            <td class="p-4 text-sm text-gray-500">${MODE_NAMES[f.mode] || f.mode}</td>
            <td class="p-4 text-sm text-right space-x-2">
                <button onclick="openEditModal(${f.id})" class="text-blue-600 hover:underline">Edytuj</button>
                <button onclick="deleteFos(${f.id})" class="text-red-600 hover:underline">Usuń</button>
            </td>
        </tr>
    `).join("");
}

function applyFilters() {
    const name = document.getElementById("filter-name-input").value.toLowerCase();
    const mode = document.getElementById("filter-mode-input").value;

    // Filtrujemy dane, które już są w pamięci (z loadFos)
    const filtered = allFos.filter(f =>
        (name === "" || f.name.toLowerCase().includes(name)) &&
        (mode === "" || f.mode.toString() === mode)
    );
    renderTable(filtered);
}

// --- Obsługa Modali ---
async function populateFaculties() {
    try {
        const response = await fetch(`${API_URL}/metadata`);
        const data = await response.json();
        const select = document.getElementById("form-facultyId");
        if (select) {
            select.innerHTML = data.faculties.map(f =>
                `<option value="${f.key}">${f.value}</option>`
            ).join("");
        }
    } catch (err) {
        console.error("Błąd ładowania wydziałów:", err);
    }
}

function openCreateModal() {
    populateFaculties();
    populateModeSelect();
    document.getElementById("modal-title").innerText = "Dodaj Kierunek";
    document.getElementById("fos-form").reset();
    document.getElementById("form-id").value = "";
    document.getElementById("crud-modal").classList.remove("hidden");
}

async function openEditModal(id) {
    try {
        await populateFaculties();
        const response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) return;

        const fos = await response.json();
        document.getElementById("modal-title").innerText = "Edytuj Kierunek";
        document.getElementById("form-id").value = fos.id;
        document.getElementById("form-name").value = fos.name;
        document.getElementById("form-degree").value = fos.degree;
        document.getElementById("form-facultyId").value = fos.facultyId;
        document.getElementById("form-mode").value = fos.mode;

        document.getElementById("crud-modal").classList.remove("hidden");
    } catch (err) {
        console.error("Błąd przy pobieraniu danych do edycji:", err);
    }
}

function closeCrudModal() {
    document.getElementById("crud-modal").classList.add("hidden");
}

// --- Operacje CRUD (API) ---
async function handleFormSubmit(event) {
    event.preventDefault();

    const id = document.getElementById("form-id").value;
    const payload = {
        id: parseInt(id) || 0,
        name: document.getElementById("form-name").value,
        degree: document.getElementById("form-degree").value,
        facultyId: parseInt(document.getElementById("form-facultyId").value),
        mode: parseInt(document.getElementById("form-mode").value)
    };

    const isEdit = payload.id > 0;

    try {
        const response = await fetch(isEdit ? `${API_URL}/${payload.id}` : API_URL, {
            method: isEdit ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            alert("Wystąpił błąd podczas zapisu.");
            return;
        }

        closeCrudModal();
        await loadFos(); // Odśwież listę po zapisie
    } catch (err) {
        console.error("Błąd zapisu:", err);
    }
}

async function deleteFos(id) {
    if (!confirm("Czy na pewno chcesz usunąć ten kierunek?")) return;

    try {
        const response = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
        if (response.ok) {
            await loadFos();
        } else {
            alert("Nie udało się usunąć kierunku.");
        }
    } catch (err) {
        console.error("Błąd usuwania:", err);
    }
}