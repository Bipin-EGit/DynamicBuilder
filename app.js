(function init() {
  const root = document.documentElement;
  const savedTheme = localStorage.getItem("theme");
  if (savedTheme === "dark") {
    root.setAttribute("data-theme", "dark");
  }

  const yearSpan = document.getElementById("year");
  if (yearSpan) yearSpan.textContent = String(new Date().getFullYear());

  const themeToggle = document.getElementById("themeToggle");
  if (themeToggle) {
    themeToggle.addEventListener("click", () => {
      const isDark = root.getAttribute("data-theme") === "dark";
      if (isDark) {
        root.removeAttribute("data-theme");
        localStorage.setItem("theme", "light");
      } else {
        root.setAttribute("data-theme", "dark");
        localStorage.setItem("theme", "dark");
      }
    });
  }

  // Initialize Leaflet map if available
  const mapEl = document.getElementById("map");
  if (mapEl && window.L) {
    const placeToCoords = {
      darwin: [-12.4634, 130.8456],
      alice: [-23.6980, 133.8807],
      katherine: [-14.4667, 132.2667],
    };

    const map = L.map("map", { zoomControl: true, attributionControl: true });
    map.setView(placeToCoords.darwin, 11);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      maxZoom: 19,
      attribution: "&copy; OpenStreetMap contributors",
    }).addTo(map);

    let marker = L.marker(placeToCoords.darwin).addTo(map);

    const branchSelect = document.getElementById("branch");
    if (branchSelect) {
      branchSelect.addEventListener("change", (e) => {
        const value = e.target.value;
        const coords = placeToCoords[value] || placeToCoords.darwin;
        map.setView(coords, 12, { animate: true });
        marker.setLatLng(coords);
      });
    }
  }
})();