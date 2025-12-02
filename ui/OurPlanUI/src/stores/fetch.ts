import { getCookie } from "./helper";

// const BASE_URL = "http://localhost:5000/api";
const BASE_URL = "https://fitness-there-borders-eastern.trycloudflare.com/api";

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
   // ✅ Adaugă tokenul dacă există
  const token = getCookie('token');
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }


  const options: RequestInit = {
    method,
    headers,
    body: isFormData ? body : JSON.stringify(body),
  };

  const url = `${BASE_URL}/${endpoint}${queryString}`;
  const response = await fetch(url, options);

  if (!response.ok) {
    let errorData;
    try {
      const text = await response.text();
      errorData = text ? JSON.parse(text) : { message: `HTTP ${response.status}: ${response.statusText}` };
    } catch {
      errorData = { message: `HTTP ${response.status}: ${response.statusText}` };
    }
    throw errorData;
  }

  // Check if response has content
  const text = await response.text();
  
  // If response is empty, return null
  if (!text || text.trim() === "") {
    return null;
  }

  // Try to parse as JSON
  try {
    return JSON.parse(text);
  } catch (error) {
    console.error("Failed to parse response as JSON:", text);
    throw { message: "Invalid JSON response from server" };
  }
};

export default fetchApi;