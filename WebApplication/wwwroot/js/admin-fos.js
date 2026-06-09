const API_URL = "/api/FieldsOfStudiesApi";
let allFos = [];

document.addEventListener("DOMContentLoaded", () => loadFos());

async function loadFos() {
    try {
        const response = await fetch(API_URL);
        allFos = await response.json();
    } catch (err) {
        console.error(err);
    }
}

function applyFilters() {
    const name = document.getElementById("filter-name-input").value.toLowerCase();
    const mode = document.getElementById("filter-mode-input").value;

    const filtered = allFos.filter(f =>
        (name === "" || f.name.toLowerCase().includes(name)) &&
        (mode === "" || f.mode.toString() === mode)
    );
    renderTable(filtered);
}

const MODE_NAMES = {
    0: "Stacjonarne",
    1: "Niestacjonarne",
    2: "Podyplomowe"
};

function renderTable(data) {
    const tbody = document.getElementById("fos-rows");
    tbody.innerHTML = data.map(f => `
        <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20 transition-colors">
            <td class="p-4 text-sm font-semibold">${f.name}</td>
            <td class="p-4 text-sm text-gray-600">${f.degree}</td>
            <td class="p-4 text-sm text-gray-500">${MODE_NAMES[f.mode] || f.mode}</td>
            <td class="p-4 text-sm text-right space-x-2">
                <button onclick="openEditModal(${f.id})" class="text-blue-600 hover:underline">Edytuj</button>
                <button onclick="deleteFos(${f.id})" class="text-red-600 hover:underline">Usuń</button>
            </td>
        </tr>
    `).join("");

    async function deleteFos(id) {
        if (!confirm("Usunąć ten kierunek?")) return;
        await fetch(`${API_URL}/${id}`, {method: "DELETE"});
        loadFos();
    }

// Pobierz listę wydziałów przy starcie lub przy otwarciu modala
    async function populateFaculties() {
        const response = await fetch('/api/FieldsOfStudiesApi/metadata'); // Zakładając, że masz taką metodę
        const data = await response.json();
        const select = document.getElementById("form-facultyId");

        select.innerHTML = data.faculties.map(f =>
            `<option value="${f.key}">${f.value}</option>`
        ).join("");
    }

// Wywołaj to w openCreateModal
    function openCreateModal() {
        populateFaculties(); // Upewnij się, że opcje są załadowane
        document.getElementById("modal-title").innerText = "Dodaj Kierunek";
        document.getElementById("fos-form").reset();
        document.getElementById("form-id").value = "";
        document.getElementById("crud-modal").classList.remove("hidden");
    }

    async function openEditModal(id) {
        try {
            // Pobieramy dane szczegółowe kierunku
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
            // Odświeżamy listę po udanym zapisie
            await loadFos();
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
}

