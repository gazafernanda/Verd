/**
 * The single place temperatures are converted and formatted.
 *
 * Celsius is the only unit the app displays. Anything arriving in Fahrenheit is
 * converted here in the data layer rather than at the point of display, so a
 * value can't reach a component still in the wrong unit.
 */

/** Whole degrees — the app never shows fractional temperatures. */
const DECIMALS = 0;

export const TEMPERATURE_UNIT = "°C";

/**
 * Just the unit letter, for layouts that render the degree sign and the letter
 * as separate elements at different sizes. Kept here rather than typed inline in
 * each component — a hardcoded "F" in one card is exactly how this drifted before.
 */
export const TEMPERATURE_UNIT_LETTER = "C";

/** Converts a Fahrenheit reading to Celsius. */
export function fahrenheitToCelsius(fahrenheit: number): number {
  return ((fahrenheit - 32) * 5) / 9;
}

/**
 * Normalises a reading to Celsius. Sources that report Fahrenheit pass
 * `unit: "F"`; everything else is already Celsius and passes through untouched.
 */
export function toCelsius(value: number, unit: "C" | "F" = "C"): number {
  return unit === "F" ? fahrenheitToCelsius(value) : value;
}

/**
 * Rounds to the app's shared precision. Use when a number is needed rather than
 * a display string — chart geometry, comparisons, thresholds.
 */
export function roundTemp(celsius: number): number {
  const factor = 10 ** DECIMALS;
  return Math.round(celsius * factor) / factor;
}

/** The bare number as displayed, with no degree symbol (e.g. "24"). */
export function formatTempValue(celsius: number | null | undefined): string {
  if (celsius === null || celsius === undefined || Number.isNaN(celsius)) return "—";
  return roundTemp(celsius).toFixed(DECIMALS);
}

/**
 * The full display form, unit included (e.g. "24°C"). Every temperature the user
 * sees goes through this, which is what keeps the format identical app-wide.
 */
export function formatTemp(celsius: number | null | undefined): string {
  const value = formatTempValue(celsius);
  return value === "—" ? value : `${value}${TEMPERATURE_UNIT}`;
}

/**
 * Compact form for places where the unit is already established by a nearby
 * label — a hi/lo pair, or an axis that states "°C" once (e.g. "24°").
 */
export function formatTempShort(celsius: number | null | undefined): string {
  const value = formatTempValue(celsius);
  return value === "—" ? value : `${value}°`;
}
