import type { TopoEditorPoint } from './topo-editor-types';

export interface TopoOverlaySvgOptions {
	points: readonly TopoEditorPoint[];
	naturalWidth: number;
	naturalHeight: number;
	strokeColor: string;
	strokeWidth: number;
}

/**
 * Signature identifying the rendered output; used to skip redundant File
 * creation when nothing visible changed.
 */
export const buildTopoOverlaySignature = (options: TopoOverlaySvgOptions): string => {
	const pointsSignature = options.points.map((point) => `${point.x},${point.y}`).join(' ');
	return `${options.naturalWidth}x${options.naturalHeight}|${options.strokeWidth}|${pointsSignature}`;
};

export const buildTopoOverlayFile = (options: TopoOverlaySvgOptions): File => {
	const pointsSignature = options.points.map((point) => `${point.x},${point.y}`).join(' ');
	const svgString = `<?xml version="1.0" encoding="utf-8"?>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${options.naturalWidth} ${options.naturalHeight}">
	<polyline points="${pointsSignature}" fill="none" stroke="${options.strokeColor}" stroke-width="${options.strokeWidth}" />
</svg>`;

	return new File([svgString], 'route_topo.svg', { type: 'image/svg+xml' });
};
