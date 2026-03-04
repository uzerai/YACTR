export const parseFormData = (formData: FormData): {
  [k: string]: string | number | File | undefined;
} => {
  return Object.fromEntries(formData.entries().map(([key, value]) => {
    switch (typeof value) {
      case "string":
        return [key, parseStringOrUndefined(value)];
      case "object":
        return [key, parseFileOrUndefined(value)];
      case "number":
        return [key, Number.parseInt(value)];
      default:
        return [key, undefined];
    }
  }));
}

export const parseStringOrUndefined = (string: string): string | undefined => {
  return string === '' ? undefined : string;
}

export const parseFileOrUndefined = (file: File): File | undefined => {
  return file.size === 0 ? undefined : file;
}