import TopoEditorRoot from './topo-editor-root.svelte';

interface TopoEditorPoint {
  x: number;
  y: number;
}

export {
  type TopoEditorPoint,
  TopoEditorRoot as TopoEditor,
}