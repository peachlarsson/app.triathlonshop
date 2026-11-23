import Map, { NavigationControl } from "react-map-gl/maplibre";
import { setWorkerUrl } from "maplibre-gl";
import { useTheme } from "@/context/theme-provider";
import maplibreWorkerUrl from "maplibre-gl/dist/maplibre-gl-worker.mjs?worker&url";
import "maplibre-gl/dist/maplibre-gl.css";

const lightMapStyle = "https://tiles.openfreemap.org/styles/positron";
const darkMapStyle = "https://tiles.openfreemap.org/styles/dark";

setWorkerUrl(maplibreWorkerUrl);

export function FlightMap() {
  const { resolvedTheme } = useTheme();

  return (
    <section
      aria-labelledby="flight-map-title"
      className="overflow-hidden rounded-xl border bg-card"
    >
      <div className="px-4 py-3">
        <h3 id="flight-map-title" className="font-semibold">
          Flight map
        </h3>
        <p className="text-sm text-muted-foreground">Europe overview</p>
      </div>
      <div className="h-[420px] w-full">
        <Map
          aria-label="Map of Europe"
          initialViewState={{
            longitude: 15,
            latitude: 52,
            zoom: 3.2,
          }}
          mapStyle={resolvedTheme === "dark" ? darkMapStyle : lightMapStyle}
          minZoom={2}
          maxZoom={12}
        >
          <NavigationControl position="top-right" />
        </Map>
      </div>
    </section>
  );
}
