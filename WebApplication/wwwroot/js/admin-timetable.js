const API_URL = "/api/Timetables";

const CLASS_TYPES = {
    0: "Wykład",
    1: "Laboratorium",
    2: "Ćwiczenia",
    3: "Projekt"
};

const DAY_NAMES = {
    0: "Niedziela",
    1: "Poniedziałek",
    2: "Wtorek",
    3: "Środa",
    4: "Czwartek",
    5: "Piątek",
    6: "Sobota"
};

const WEEK_CYCLES = {
    0: "Co tydzień",
    1: "Tydzień Parzysty",
    2: "Tydzień Nieparzysty"
};

const FILTER_INPUTS = [
    {inputId: "filter-subject-input", hiddenId: "filter-subject-id", listId: "subjects-list"},
    {inputId: "filter-teacher-input", hiddenId: "filter-teacher-id", listId: "teachers-list"},
    {inputId: "filter-room-input", hiddenId: "filter-room-id", listId: "rooms-list"},
    {inputId: "filter-group-input", hiddenId: "filter-group-id", listId: "groups-list"}
];

const MODAL_INPUTS = [
    {inputId: "form-subject-input", hiddenId: "form-subjectId", listId: "subjects-list-modal"},
    {inputId: "form-teacher-input", hiddenId: "form-teacherId", listId: "teachers-list-modal"},
    {inputId: "form-room-input", hiddenId: "form-roomId", listId: "rooms-list-modal"},
    {inputId: "form-group-input", hiddenId: "form-groupId", listId: "groups-list-modal"}
];

let allLessons = null;

document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
    wireDatalistInputs(FILTER_INPUTS);
    wireDatalistInputs(MODAL_INPUTS);
    renderTable();
}

function wireDatalistInputs(inputs) {
    inputs.forEach(({inputId, hiddenId, listId}) => {
        AdminUi.wireDatalistInput(inputId, hiddenId, listId);
    });
}

function getDisplayValue(map, value) {
    return Object.prototype.hasOwnProperty.call(map, value) ? map[value] : value;
}

function getFilterPayload() {
    return {
        subjectId: AdminUi.getNullableIntValue("filter-subject-id"),
        teacherId: AdminUi.getNullableIntValue("filter-teacher-id"),
        roomId: AdminUi.getNullableIntValue("filter-room-id"),
        groupId: AdminUi.getNullableIntValue("filter-group-id"),
        classType: AdminUi.getNullableIntValue("filter-classType"),
        dayOfWeek: AdminUi.getNullableIntValue("filter-dayOfWeek"),
        weekCycle: AdminUi.getNullableIntValue("filter-weekCycle")
    };
}

function hasActiveFilters() {
    return Object.values(getFilterPayload()).some(value => value !== null);
}

async function applyFilters() {
    try {
        const response = await fetch(`${API_URL}/filter`, {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify(getFilterPayload())
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
    const tbody = document.getElementById("timetable-rows");
    if (!tbody) {
        return;
    }

    if (allLessons === null) {
        tbody.innerHTML = `
            <tr>
                <td colspan="9" class="p-8 text-center text-gray-500">
                    Wybierz filtry i kliknij "Filtruj", aby wyświetlić dostępne zajęcia.
                </td>
            </tr>
        `;
        return;
    }

    if (allLessons.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="9" class="p-8 text-center text-gray-500">
                    Brak zajęć dla wybranych filtrów.
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = allLessons.map(lesson => `
        <tr class="hover:bg-gray-50/50 dark:hover:bg-gray-800/20 transition-colors">
            <td class="p-4 text-sm font-semibold text-gray-900 dark:text-gray-100">${lesson.subjectName}</td>
            <td class="p-4 text-sm text-gray-600 dark:text-gray-400">${lesson.teacherName}</td>
            <td class="p-4 text-sm text-gray-500">${lesson.roomNumber}</td>
            <td class="p-4 text-sm text-gray-500">${lesson.groupName}</td>
            <td class="p-4 text-sm text-gray-500">
                <span class="px-2 py-0.5 bg-gray-100 dark:bg-gray-800 rounded text-xs font-medium">
                    ${getDisplayValue(CLASS_TYPES, lesson.classType)}
                </span>
            </td>
            <td class="p-4 text-sm text-gray-500">${getDisplayValue(DAY_NAMES, lesson.dayOfWeek)}</td>
            <td class="p-4 text-sm text-gray-500">${getDisplayValue(WEEK_CYCLES, lesson.weekCycle)}</td>
            <td class="p-4 text-sm text-gray-500">${AdminUi.formatTime(lesson.startTime)} - ${AdminUi.formatTime(lesson.endTime)}</td>
            <td class="p-4 text-sm text-right space-x-2">
                <button onclick="openEditModal(${lesson.id})" class="text-blue-600 hover:underline font-medium">Edytuj</button>
                <span class="text-gray-300">|</span>
                <button onclick="deleteLesson(${lesson.id})" class="text-red-600 hover:underline font-medium">Usuń</button>
            </td>
        </tr>
    `).join("");
}

function getFormPayload() {
    return {
        subjectId: Number.parseInt(document.getElementById("form-subjectId").value, 10),
        teacherId: Number.parseInt(document.getElementById("form-teacherId").value, 10),
        roomId: Number.parseInt(document.getElementById("form-roomId").value, 10),
        groupId: Number.parseInt(document.getElementById("form-groupId").value, 10),
        classType: Number.parseInt(document.getElementById("form-classType").value, 10),
        dayOfWeek: Number.parseInt(document.getElementById("form-dayOfWeek").value, 10),
        weekCycle: Number.parseInt(document.getElementById("form-weekCycle").value, 10),
        startTime: document.getElementById("form-startTime").value,
        endTime: document.getElementById("form-endTime").value
    };
}

async function handleFormSubmit(event) {
    event.preventDefault();

    const id = document.getElementById("form-id").value;
    const isEdit = id !== "";
    const payload = getFormPayload();

    if (isEdit) {
        payload.id = Number.parseInt(id, 10);
    }

    try {
        const response = await fetch(isEdit ? `${API_URL}/${id}` : API_URL, {
            method: isEdit ? "PUT" : "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            alert("Wystąpił błąd podczas zapisu danych.");
            return;
        }

        closeCrudModal();

        if (hasActiveFilters()) {
            await applyFilters();
        }
    } catch (err) {
        console.error(err);
    }
}

function openCreateModal() {
    document.getElementById("modal-title").innerText = "Dodaj nowe zajęcia";
    document.getElementById("timetable-form").reset();
    document.getElementById("form-id").value = "";

    showCrudModal();
}

async function openEditModal(id) {
    try {
        const response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) {
            return;
        }

        const lesson = await response.json();

        document.getElementById("modal-title").innerText = "Edytuj zajęcia";
        document.getElementById("form-id").value = lesson.id;
        document.getElementById("form-subject-input").value = lesson.subjectName;
        document.getElementById("form-subjectId").value = lesson.subjectId;
        document.getElementById("form-teacher-input").value = lesson.teacherName;
        document.getElementById("form-teacherId").value = lesson.teacherId;
        document.getElementById("form-room-input").value = lesson.roomNumber;
        document.getElementById("form-roomId").value = lesson.roomId;
        document.getElementById("form-group-input").value = lesson.groupName;
        document.getElementById("form-groupId").value = lesson.groupId;
        document.getElementById("form-classType").value = lesson.classType;
        document.getElementById("form-dayOfWeek").value = lesson.dayOfWeek;
        document.getElementById("form-weekCycle").value = lesson.weekCycle;
        document.getElementById("form-startTime").value = AdminUi.formatTime(lesson.startTime);
        document.getElementById("form-endTime").value = AdminUi.formatTime(lesson.endTime);
        showCrudModal();
    } catch (err) {
        console.error(err);
    }
}

function closeCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.add("hidden");
    modal.classList.remove("flex");
}

async function deleteLesson(id) {
    if (!confirm("Usunąć ten wpis z planu zajęć?")) {
        return;
    }

    try {
        const response = await fetch(`${API_URL}/${id}`, {method: "DELETE"});
        if (response.ok) {
            allLessons = allLessons?.filter(lesson => lesson.id !== id) ?? null;
            renderTable();
        }
    } catch (err) {
        console.error(err);
    }
}

function showCrudModal() {
    const modal = document.getElementById("crud-modal");

    modal.classList.remove("hidden");
    modal.classList.add("flex");
}