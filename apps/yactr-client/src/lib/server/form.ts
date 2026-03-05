export function getRequiredField(form: FormData, name: string): string {
	const value = form.get(name);

	if (typeof value !== 'string' || value.trim() === '') {
		throw new Error(`Missing or empty form field: ${name}`);
	}

	return value;
}

export function parseRequiredJsonField<T>(form: FormData, name: string): T {
	const raw = getRequiredField(form, name);

	return JSON.parse(raw) as T;
}

