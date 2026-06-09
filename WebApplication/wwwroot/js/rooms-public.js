const PUBLIC_ROOMS_API =
    "/api/public/rooms";

let publicRooms = [];

document.addEventListener(
    "DOMContentLoaded",
    initializeRoomsPage
);

async function initializeRoomsPage() {
    document
        .getElementById("room-search")
        ?.addEventListener(
            "input",
            renderPublicRooms
        );

    await loadPublicRooms();
}

async function loadPublicRooms() {
    try {
        const response = await fetch(
            PUBLIC_ROOMS_API
        );

        if (!response.ok) {
            throw new Error(
                "Nie udało się pobrać listy sal."
            );
        }

        publicRooms = await response.json();

        document
            .getElementById("rooms-loading")
            ?.classList
            .add("hidden");

        renderPublicRooms();
    } catch (error) {
        console.error(
            "Błąd pobierania sal:",
            error
        );

        const loading =
            document.getElementById("rooms-loading");

        if (loading) {
            loading.textContent =
                "Nie udało się pobrać listy sal.";
        }
    }
}

function renderPublicRooms() {
    const grid =
        document.getElementById("rooms-grid");

    const empty =
        document.getElementById("rooms-empty");

    if (!grid || !empty) {
        return;
    }

    const search = document
        .getElementById("room-search")
        ?.value
        .trim()
        .toLowerCase() ?? "";

    const filtered = publicRooms.filter(room =>
    {
        const searchable = [
            room.roomNumber,
            room.buildingName
        ]
            .filter(Boolean)
            .join(" ")
            .toLowerCase();

        return searchable.includes(search);
    });

    if (filtered.length === 0) {
        grid.classList.add("hidden");
        empty.classList.remove("hidden");
        grid.innerHTML = "";
        return;
    }

    empty.classList.add("hidden");
    grid.classList.remove("hidden");

    grid.innerHTML = filtered
        .map(room => `
            <article class="rounded-xl border border-gray-200
                            bg-white p-5 shadow-sm
                            dark:border-gray-700 dark:bg-gray-900">

                <h2 class="text-lg font-semibold text-gray-900
                           dark:text-gray-100">
                    Sala ${AdminUi.escapeHtml(room.roomNumber)}
                </h2>

                <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
                    ${AdminUi.escapeHtml(room.buildingName)}
                </p>
            </article>
        `)
        .join("");
}