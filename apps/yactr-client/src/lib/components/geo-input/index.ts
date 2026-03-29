import type { Coordinate } from "ol/coordinate";
import { transform } from "ol/proj";
import Point from "./point.svelte";
import LineString from "./line-string.svelte";
import Polygon from "./polygon.svelte";
import MultiPolygon from "./multi-polygon.svelte";

/**
 * Converts a coordinate from EPSG:3857 (Web Mercator, x/y in meters) to
 * EPSG:4326 / SRID 4326 (WGS 84 longitude, latitude in degrees).
 *
 * Does not mutate the input array.
 */
export function fromEPSG3857ToSRID4326(coordinate: Coordinate): Coordinate {
	return transform(
		coordinate,
		"EPSG:3857",
		"EPSG:4326",
	);
}

/**
 * Converts a coordinate from EPSG:4326 / SRID 4326 (WGS 84 longitude, latitude
 * in degrees) to EPSG:3857 (Web Mercator, x/y in meters).
 *
 * Does not mutate the input array.
 */
export function fromSRID4326ToEPSG3857(coordinate: Coordinate): Coordinate {
	return transform(
		coordinate,
		"EPSG:4326",
		"EPSG:3857",
	);
}

export {
	Point,
	LineString,
	Polygon,
	MultiPolygon,
};