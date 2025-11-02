const BASE_URL = "https://localhost:7278/api";
/**
 * Funcția pentru a construi query params dintr-un obiect payload.
 *
 * @param payload - obiectul cu parametrii query, ex: {id: '123', name: 'example'}
 * @returns stringul cu query params, ex: '?id=123&name=example'
 */
export const buildQueryParams = (payload?: Record<string, any>) => {
  if (!payload) return '';
  
  const searchParams = new URLSearchParams();
  Object.entries(payload).forEach(([key, value]) => {
    if (value !== undefined && value !== null) {
      searchParams.append(key, String(value));
    }
  });

  const queryString = searchParams.toString();
  return queryString ? `?${queryString}` : '';
};

/**
 * Funcția pentru a trimite cereri API.
 *
 * @param endpoint - segmentul de URL, ex: 'Objectives/getById'
 * @param method - metoda HTTP, ex: 'GET', 'POST', 'PUT'
 * @param body - corpul cererii, opțional, doar pentru metodele POST și PUT
 * @param query - obiect cu parametrii query, ex: {id: '123', name: 'example'}
 * @returns răspunsul cererii ca obiect JSON
 */
const fetchApi = async (
  endpoint: string,
  method: string = "GET",
  body?: any,
  query?: Record<string, any>,
  isFormData: boolean = false
) => {
  const queryString = query
    ? "?" + new URLSearchParams(query as Record<string, string>).toString()
    : "";

  const headers: Record<string, string> = {};
  if (!isFormData) headers["Content-Type"] = "application/json";
  headers["Ngrok-Skip-Browser-Warning"] = "true";

  const options: RequestInit = {
    method,
    headers,
    body: isFormData ? body : JSON.stringify(body),
  };

  const url = `${BASE_URL}/${endpoint}${queryString}`;
  const response = await fetch(url, options);

  if (!response.ok) {
    const errorData = await response.json();
    throw errorData;
  }

  return response.json();
};

export default fetchApi;