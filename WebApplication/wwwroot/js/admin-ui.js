function findDatalistOption(listId, value) {
    const list = document.getElementById(listId);
    if (!list) {
        return null;
    }

    return Array.from(list.options).find(option => option.value === value) ?? null;
}

function syncDatalistField(listId, hiddenId, value) {
    const hidden = document.getElementById(hiddenId);
    if (!hidden) {
        return;
    }

    const option = findDatalistOption(listId, value);
    hidden.value = option?.dataset.id ?? "";
}

function handleSearch(listId, hiddenId, value) {
    syncDatalistField(listId, hiddenId, value);
}

function wireDatalistInput(inputId, hiddenId, listId) {
    const input = document.getElementById(inputId);
    if (!input) {
        return;
    }

    const updateHiddenField = event => syncDatalistField(listId, hiddenId, event.target.value);

    input.addEventListener("input", updateHiddenField);
    syncDatalistField(listId, hiddenId, input.value);
}

function getNullableIntValue(id) {
    const value = document.getElementById(id)?.value ?? "";
    return value === "" ? null : Number.parseInt(value, 10);
}

function formatTime(value) {
    return value ? value.slice(0, 5) : "";
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

window.AdminUi = {
    escapeHtml,
    formatTime,
    getNullableIntValue,
    syncDatalistField,
    wireDatalistInput
};
window.handleSearch = handleSearch;
