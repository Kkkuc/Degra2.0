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
    setupModalInputs();
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

        if (!response.ok) {
            throw new Error("Błąd filtrowania.");
        }
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
            const hasFilters = document.getElementById('filter-subject-id').value ||
                document.getElementById('filter-teacher-id').value ||
                document.getElementById('filter-classType').value;

            if (hasFilters) {
                await applyFilters(); 
            } 
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
    const fieldsToClear = [
        { input: 'form-subject-input', hidden: 'form-subjectId' },
        { input: 'form-teacher-input', hidden: 'form-teacherId' },
        { input: 'form-room-input', hidden: 'form-roomId' },
        { input: 'form-group-input', hidden: 'form-groupId' }
    ];

    fieldsToClear.forEach(field => {
        document.getElementById(field.input).value = "";
        document.getElementById(field.hidden).value = "";
    });
    document.getElementById('crud-modal').classList.remove('hidden');
}

async function openEditModal(id) {
    try {
        const response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) return;
        const l = await response.json();
        document.getElementById('modal-title').innerText = "Edytuj zajęcia";
        document.getElementById('form-id').value = l.id;
        document.getElementById('form-subject-input').value = l.subjectName;
        document.getElementById('form-subjectId').value = l.subjectId;
        document.getElementById('form-teacher-input').value = l.teacherName;
        document.getElementById('form-teacherId').value = l.teacherId;
        document.getElementById('form-room-input').value = l.roomNumber;
        document.getElementById('form-roomId').value = l.roomId;
        document.getElementById('form-group-input').value = l.groupName;
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

function setupModalInputs() {
    const inputs = [
        {input: 'form-subject-input', hidden: 'form-subjectId', list: 'subjects-list-modal'},
        {input: 'form-teacher-input', hidden: 'form-teacherId', list: 'teachers-list-modal'},
        {input: 'form-room-input', hidden: 'form-roomId', list: 'rooms-list-modal'},
        {input: 'form-group-input', hidden: 'form-groupId', list: 'groups-list-modal'}
    ];

    inputs.forEach(item => {
        document.getElementById(item.input).addEventListener('input', function() {
            const list = document.getElementById(item.list);
            const hidden = document.getElementById(item.hidden);
            const option = Array.from(list.options).find(opt => opt.value === this.value);
            hidden.value = option ? option.getAttribute('data-id') : "";
        });
    });
}