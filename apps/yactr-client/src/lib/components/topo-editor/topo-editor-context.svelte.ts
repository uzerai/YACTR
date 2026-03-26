import { createContext } from 'svelte';
import type { TopoEditorPoint } from './topo-editor-types';

class TopoEditorViewport {
	canvasWidth = $state<number>();
	canvasHeight = $state<number>();
	naturalWidth = $state<number>();
	naturalHeight = $state<number>();

	setCanvasSize(width?: number, height?: number) {
		this.canvasWidth = width;
		this.canvasHeight = height;
	}

	setNaturalSize(width?: number, height?: number) {
		this.naturalWidth = width;
		this.naturalHeight = height;
	}

	get scaleX() {
		return (this.canvasWidth ?? 1) / (this.naturalWidth ?? 1);
	}

	get scaleY() {
		return (this.canvasHeight ?? 1) / (this.naturalHeight ?? 1);
	}

	applyScaling(point: TopoEditorPoint): TopoEditorPoint {
		return {
			x: point.x * this.scaleX,
			y: point.y * this.scaleY
		};
	}

	removeScaling(point: TopoEditorPoint): TopoEditorPoint {
		return {
			x: point.x / this.scaleX,
			y: point.y / this.scaleY
		};
	}
}

const [getTopoEditorViewport, setTopoEditorViewport] = createContext<TopoEditorViewport>();

export { TopoEditorViewport, getTopoEditorViewport, setTopoEditorViewport };
