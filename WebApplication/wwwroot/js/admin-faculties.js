const API_URL = "/api/faculties";
let allFaculties = [];

// --- Inicjalizacja ---
document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
    loadFaculties();
}

// --- Logika API ---
async function loadFaculties(query = "") {
    const url = query ? `${API_URL}?search=${encodeURIComponent(query)}` : API_URL;
    try {
        const response = await fetch(url);
        allFaculties = await response.json();
        renderTable();
    } catch (err) {
        console.error("Błąd ładowania wydziałów:", err);
    }
}

async function handleFormSubmit(e) {
    e.preventDefault();
    const id = document.getElementById("form-id").value;
    const payload = {
        id: id ? parseInt(id) : 0,
        name: document.getElementById("form-name").value,
        abbreviation: document.getElementById("form-abbreviation").value
    };

    try {
        await fetch(id ? `${API_URL}/${id}` : API_URL, {
            method: id ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        closeCrudModal();
        loadFaculties();
    } catch (err) {
        console.error("Błąd zapisu:", err);
    }
}

async function deleteFaculty(id) {
    if (!confirm("Usunąć ten wydział?")) return;
    try {
        await fetch(`${API_URL}/${id}`, { method: "DELETE" });
        loadFaculties();
    } catch (err) {
        console.error("Błąd usuwania:", err);
    }
}

// --- Filtrowanie ---
async function applyFilters() {
    const query = document.getElementById("filter-faculty-input").value;
    await loadFaculties(query);
}

// --- Renderowanie ---
function renderTable() {
    const tbody = document.getElementById("faculties-rows");
    tbody.innerHTML = allFaculties.map(f => `
        <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20 transition-colors">
            <td class="p-4 text-sm font-semibold">${f.abbreviation}</td>
            <td class="p-4 text-sm text-gray-600 dark:text-gray-400">${f.name}</td>
            <td class="p-4 text-sm text-right space-x-2">
                <button onclick="openEditModal(${f.id})" class="text-blue-600 hover:underline">Edytuj</button>
                <span class="text-gray-300">|</span>
                <button onclick="deleteFaculty(${f.id})" class="text-red-600 hover:underline">Usuń</button>
            </td>
        </tr>
    `).join("");
}

// --- Obsługa Modali ---
function openCreateModal() {
    document.getElementById("modal-title").innerText = "Dodaj Wydział";
    document.getElementById("faculty-form").reset();
    document.getElementById("form-id").value = "";
    document.getElementById("crud-modal").classList.remove("hidden");
}

function openEditModal(id) {
    const faculty = allFaculties.find(f => f.id === id);
    if (!faculty) return;

    document.getElementById("modal-title").innerText = "Edytuj Wydział";
    document.getElementById("form-id").value = faculty.id;
    document.getElementById("form-name").value = faculty.name;
    document.getElementById("form-abbreviation").value = faculty.abbreviation;
    document.getElementById("crud-modal").classList.remove("hidden");
}

function closeCrudModal() {
    document.getElementById("crud-modal").classList.add("hidden");
}