import Map, { Marker, NavigationControl } from "react-map-gl/maplibre";
import { Package } from "lucide-react";
import { setWorkerUrl } from "maplibre-gl";
import { useEffect } from "react";
import { toast } from "sonner";
import { useTheme } from "@/context/theme-provider";
import { WarehouseErrorPopup } from "../components/warehouse-error-popup";
import maplibreWorkerUrl from "maplibre-gl/dist/maplibre-gl-worker.mjs?worker&url";
import "maplibre-gl/dist/maplibre-gl.css";
import type { WarehouseDto } from "../data/schema";

const lightMapStyle = "https://tiles.openfreemap.org/styles/positron";
const darkMapStyle = "https://tiles.openfreemap.org/styles/dark";

type FlightMapProps = {
  warehouses: WarehouseDto[];
  error?: string;
};

setWorkerUrl(maplibreWorkerUrl);

export function FlightMap({ warehouses, error }: FlightMapProps) {
  const { resolvedTheme } = useTheme();
  useEffect(() => {
    if (error) {
      toast.custom(
        (toastId) => (
          <WarehouseErrorPopup
            message={error}
            onDismiss={() => toast.dismiss(toastId)}
          />
        ),
        { id: "warehouse-location-error" },
      );
    }
  }, [error]);

  const warehouseMarkers = warehouses.flatMap((warehouse) => {
    const { latitude, longitude } = warehouse.location;
    if (
      typeof latitude !== "number" ||
      !Number.isFinite(latitude) ||
      typeof longitude !== "number" ||
      !Number.isFinite(longitude)
    ) {
      return [];
    }

    return [{ warehouse, latitude, longitude }];
  });

  return (
    <section
      aria-labelledby="flight-map-title"
      className="overflow-hidden rounded-xl border bg-card"
    >
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
          {warehouseMarkers.map(({ warehouse, latitude, longitude }) => (
            <Marker
              key={warehouse.warehouseId}
              longitude={longitude}
              latitude={latitude}
              anchor="center"
            >
              <div
                className="text-yellow-500 drop-shadow-md"
                title={warehouse.warehouseName}
              >
                <Package className="size-8 text-yellow-500" aria-hidden="true" />
              </div>
            </Marker>
          ))}
        </Map>
      </div>
    </section>
  );
}
