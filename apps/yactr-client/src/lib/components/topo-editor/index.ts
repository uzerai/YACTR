import TopoEditorRoot from './topo-editor-root.svelte';
import TopoEditorSvgOverlay from './topo-editor-svg-overlay.svelte';
import TopoEditorTopoImage from './topo-editor-topo-image.svelte';

export type { TopoEditorPoint } from './topo-editor-types';

export const TopoEditor = {
  Root: TopoEditorRoot,
  SvgOverlay: TopoEditorSvgOverlay,
  TopoImage: TopoEditorTopoImage
};

export { TopoEditorRoot, TopoEditorSvgOverlay, TopoEditorTopoImage };
