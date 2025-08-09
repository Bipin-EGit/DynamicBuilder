# DynamicBuilder
Create the form using dynamic controls

## Quick start
- Open `index.html` in a browser.
- The UI is responsive, supports light/dark theme (use the moon button), and includes a Leaflet map.

## Structure
- `index.html`: markup and CDN links
- `styles.css`: design system tokens, layout, components
- `app.js`: theme toggle, map, and basic interactions

## Customize
- Update colors by changing CSS variables in `:root` and `:root[data-theme="dark"]` within `styles.css`.
- To change initial map location, modify the `placeToCoords` values in `app.js`.
