const API_URL = '/api/Timetables';
let allLessons = null;

const classTypes = {0: "Wykład", 1: "Laboratorium", 2: "Ćwiczenia", 3: "Projekt"};
const daysOfWeek = {
    0: "Niedziela",
    1: "Poniedziałek",
    2: "Wtorek",
    3: "Środa",
    4: "Czwartek",
    5: "Piątek",
    6: "Sobota"
};
const weekCycles = {0: "Co tydzień", 1: "Tydzień Parzysty", 2: "Tydzień Nieparzysty"};

document.addEventListener("DOMContentLoaded", () => {
    const adminData = document.getElementById('admin-data');
    if (adminData && adminData.dataset.isInvalid === "true") {
        switchTab('tab-users');
    }
    renderTable();
});

document.getElementById('filter-subject-input').addEventListener('input', function(e) {
    const input = e.target;
    const list = document.getElementById('subjects-list');
    const hiddenId = document.getElementById('filter-subject-id');


    const option = Array.from(list.options).find(opt => opt.value === input.value);

    if (option) {
        hiddenId.value = option.getAttribute('data-id');
    } else {
        hiddenId.value = ""; 
    }
});

function switchTab(tabId) {
    document.querySelectorAll('.tab-content').forEach(el => el.classList.add('hidden'));
    document.getElementById(tabId).classList.remove('hidden');

    const tabs = ['tab-timetable', 'tab-users', 'tab-reports'];
    tabs.forEach(id => {
        const btn = document.getElementById(`btn-${id}`);
        if (id === tabId) {
            btn.className = "px-6 py-3 font-semibold text-sm border-b-2 border-blue-600 text-blue-600 transition-colors focus:outline-none";
        } else {
            btn.className = "px-6 py-3 font-semibold text-sm border-b-2 border-transparent text-gray-500 hover:text-gray-700 transition-colors focus:outline-none";
        }
    });
}

async function fetchLessons() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error("Błąd API.");
        allLessons = await response.json();
        renderTable();
    } catch (err) {
        console.error(err);
    }
}

function getFilterValue(id) {
    const val = document.getElementById(id).value;
    return val === "" ? null : parseInt(val);
}

async function applyFilters() {
    const filter = {
        subjectId: document.getElementById('filter-subject-id').value || null,
        teacherId: document.getElementById('filter-teacher-id').value || null,
        roomId: document.getElementById('filter-room-id').value || null,
        groupId: document.getElementById('filter-group-id').value || null,
        classType: getFilterValue('filter-classType'), 
        dayOfWeek: getFilterValue('filter-dayOfWeek'),
        weekCycle: getFilterValue('filter-weekCycle')
    };

    try {
        const response = await fetch(`${API_URL}/filter`, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(filter)
        });

        if (!response.ok) throw new Error("Błąd filtrowania.");
        allLessons = await response.json();
        renderTable();
    } catch (err) {
        console.error(err);
    }
}

function renderTable() {
    const tbody = document.getElementById('timetable-rows');
    if (allLessons === null) {
        tbody.innerHTML = `<tr><td colspan="9" class="p-8 text-center text-gray-500">
            Wybierz filtry i kliknij "Filtruj", aby wyświetlić dostępne zajęcia.
        </td></tr>`;
        return;
    }
    
    if (allLessons.length === 0) {
        tbody.innerHTML = `<tr><td colspan="9" class="p-8 text-center text-gray-500">
            Brak zajęć dla wybranych filtrów.
        </td></tr>`;
        return;
    }
    tbody.innerHTML = allLessons.map(l => `
        <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20 transition-colors">
            <td class="p-4 text-sm font-semibold text-gray-900 dark:text-gray-100">${l.subjectName}</td>
            <td class="p-4 text-sm text-gray-600 dark:text-gray-400">${l.teacherName}</td>
            <td class="p-4 text-sm text-gray-500">${l.roomNumber}</td>
            <td class="p-4 text-sm text-gray-500">${l.groupName}</td>
            <td class="p-4 text-sm text-gray-500">
                <span class="px-2 py-0.5 bg-gray-100 dark:bg-gray-800 rounded text-xs font-medium">
                    ${classTypes[l.classType] !== undefined ? classTypes[l.classType] : l.classType}
                </span>
            </td>
            <td class="p-4 text-sm text-gray-500">${daysOfWeek[l.dayOfWeek] || l.dayOfWeek}</td>
            <td class="p-4 text-sm text-gray-500">${weekCycles[l.weekCycle] || l.weekCycle}</td>
            <td class="p-4 text-sm text-gray-500">${l.startTime.substring(0, 5)} - ${l.endTime.substring(0, 5)}</td>
            <td class="p-4 text-sm text-right space-x-2">
                <button onclick="openEditModal(${l.id})" class="text-blue-600 hover:underline font-medium">Edytuj</button>
                <span class="text-gray-300">|</span>
                <button onclick="deleteLesson(${l.id})" class="text-red-600 hover:underline font-medium">Usuń</button>
            </td>
        </tr>
    `).join('');
}

async function handleFormSubmit(e) {
    e.preventDefault();
    const id = document.getElementById('form-id').value;
    const payload = {
        subjectId: parseInt(document.getElementById('form-subjectId').value),
        teacherId: parseInt(document.getElementById('form-teacherId').value),
        roomId: parseInt(document.getElementById('form-roomId').value),
        groupId: parseInt(document.getElementById('form-groupId').value),
        classType: parseInt(document.getElementById('form-classType').value),
        dayOfWeek: parseInt(document.getElementById('form-dayOfWeek').value),
        weekCycle: parseInt(document.getElementById('form-weekCycle').value),
        startTime: document.getElementById('form-startTime').value,
        endTime: document.getElementById('form-endTime').value
    };
    const isEdit = id !== "";
    if (isEdit) payload.id = parseInt(id);
    try {
        const response = await fetch(isEdit ? `${API_URL}/${id}` : API_URL, {
            method: isEdit ? 'PUT' : 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(payload)
        });
        if (response.ok) {
            closeCrudModal();
            fetchLessons();
        } else {
            alert("Wystąpił błąd podczas zapisu danych.");
        }
    } catch (err) {
        console.error(err);
    }
}

function openCreateModal() {
    document.getElementById('modal-title').innerText = "Dodaj nowe zajęcia";
    document.getElementById('timetable-form').reset();
    document.getElementById('form-id').value = "";
    document.getElementById('crud-modal').classList.remove('hidden');
}

async function openEditModal(id) {
    try {
        const response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) return;
        const l = await response.json();
        document.getElementById('modal-title').innerText = "Edytuj zajęcia";
        document.getElementById('form-id').value = l.id;
        document.getElementById('form-subjectId').value = l.subjectId;
        document.getElementById('form-teacherId').value = l.teacherId;
        document.getElementById('form-roomId').value = l.roomId;
        document.getElementById('form-groupId').value = l.groupId;
        document.getElementById('form-classType').value = l.classType;
        document.getElementById('form-dayOfWeek').value = l.dayOfWeek;
        document.getElementById('form-weekCycle').value = l.weekCycle;
        document.getElementById('form-startTime').value = l.startTime.substring(0, 5);
        document.getElementById('form-endTime').value = l.endTime.substring(0, 5);
        document.getElementById('crud-modal').classList.remove('hidden');
    } catch (err) {
        console.error(err);
    }
}

function closeCrudModal() {
    document.getElementById('crud-modal').classList.add('hidden');
}

async function deleteLesson(id) {
    if (!confirm("Usunąć ten wpis z planu zajęć?")) return;
    try {
        const response = await fetch(`${API_URL}/${id}`, {method: 'DELETE'});
        if (response.ok) {
            allLessons = allLessons.filter(l => l.id !== id);
            renderTable();
        }
    } catch (err) {
        console.error(err);
    }
}

function handleSearch(listId, hiddenId, val) {
    const list = document.getElementById(listId);
    const hidden = document.getElementById(hiddenId);
    
    const option = Array.from(list.options).find(opt => opt.value === val);

    hidden.value = option ? option.getAttribute('data-id') : "";
}