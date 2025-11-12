# 🚀 Guía de Integración Frontend → Backend (Crear Aves)

## ✅ Backend Completado

### **Endpoint disponible:**
```
POST http://localhost:5291/api/birds
```

### **Request Body (JSON):**
```json
{
  "commonName": "Águila Real",
  "scientificName": "Aquila chrysaetos",
  "family": "Accipitridae",
  "conservationStatus": "LeastConcern",
  "notes": "Ave rapaz de gran tamaño"
}
```

### **Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

## 📁 Archivos del Frontend que Debes Modificar

### **1. Servicio HTTP (BirdHttpRepository)**
📂 `src/app/core/features/bird/infrastructure/repositories/bird-http.repository.ts`

**Cambio necesario:**
- Actualizar la URL base del backend de `http://localhost:3000` a `http://localhost:5291`
- Asegurar que los nombres de las propiedades coincidan con el backend

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Bird } from '../../domain/entities/bird.interface';
import { BirdRepository } from '../../domain/repositories/bird.repository';

@Injectable()
export class BirdHttpRepository implements BirdRepository {
  private readonly API_URL = 'http://localhost:5291/api/birds'; // ✅ CAMBIA ESTO

  constructor(private http: HttpClient) {}

  addBird(bird: Omit<Bird, 'id'>): Observable<Bird> {
    // El backend espera commonName, scientificName, family, conservationStatus, notes
    const payload = {
      commonName: bird.commonName,
      scientificName: bird.scientificName,
      family: bird.family, // Ya es string enum (ej: "Accipitridae")
      conservationStatus: bird.conservationStatus, // Ya es string enum (ej: "LeastConcern")
      notes: bird.notes
    };
    
    return this.http.post<Bird>(this.API_URL, payload);
  }

  // ... otros métodos
}
```

---

### **2. Mapeo de Enums (IMPORTANTE)**

El backend espera los valores de enum **en inglés** (nombres de miembro del enum), no las descripciones en español.

#### **BirdFamily** (Backend C#)
```csharp
public enum BirdFamily {
    Accipitridae,    // "Rapaces diurnas"
    Anatidae,        // "Patos, gansos y cisnes"
    Columbidae,      // "Palomas y tórtolas"
    Trochilidae,     // "Colibríes"
    Psittacidae,     // "Loros y guacamayas"
    Strigidae,       // "Búhos y lechuzas"
    Tyrannidae,      // "Atrapamoscas y mosqueros"
    Thraupidae,      // "Tangaras"
    Turdidae,        // "Mirlos y zorzales"
    Emberizidae,     // "Pinzones y semilleros"
    Picidae,         // "Pájaros carpinteros"
    Ardeidae,        // "Garzas y garcetas"
    Falconidae       // "Halcones y cernícalos"
}
```

#### **ConservationStatus** (Backend C#)
```csharp
public enum ConservationStatus {
    Extinct,          // "Extinta"
    Endangered,       // "En Peligro"
    Vulnerable,       // "Vulnerable"
    NearThreatened,   // "Casi Amenazada"
    LeastConcern,     // "Preocupacion Menor"
    NotEvaluated      // "No Evaluada"
}
```

#### **Frontend TypeScript** (`bird.interface.ts`)
📂 `src/app/core/features/bird/domain/entities/bird.interface.ts`

**Opción 1: Cambiar el enum del frontend** (Recomendado)
```typescript
export enum BirdFamily {
  Accipitridae = 'Accipitridae',
  Anatidae = 'Anatidae',
  Columbidae = 'Columbidae',
  Trochilidae = 'Trochilidae',
  Psittacidae = 'Psittacidae',
  Strigidae = 'Strigidae',
  Tyrannidae = 'Tyrannidae',
  Thraupidae = 'Thraupidae',
  Turdidae = 'Turdidae',
  Emberizidae = 'Emberizidae',
  Picidae = 'Picidae',
  Ardeidae = 'Ardeidae',
  Falconidae = 'Falconidae'
}

export enum ConservationStatusEnum {
  Extinct = 'Extinct',
  Endangered = 'Endangered',
  Vulnerable = 'Vulnerable',
  NearThreatened = 'NearThreatened',
  LeastConcern = 'LeastConcern',
  NotEvaluated = 'NotEvaluated'
}
```

Luego crea un servicio de mapeo para mostrar las etiquetas en español en la UI:
```typescript
export const BIRD_FAMILY_LABELS: Record<BirdFamily, string> = {
  [BirdFamily.Accipitridae]: 'Rapaces diurnas (halcones, águilas, gavilanes)',
  [BirdFamily.Anatidae]: 'Patos, gansos y cisnes',
  // ... resto
};

export const CONSERVATION_STATUS_LABELS: Record<ConservationStatusEnum, string> = {
  [ConservationStatusEnum.Extinct]: 'Extinta',
  [ConservationStatusEnum.Endangered]: 'En Peligro',
  // ... resto
};
```

---

### **3. Verificar CORS**

El backend ya tiene CORS configurado para `http://localhost:4200`:

```csharp
// Program.cs (ya está configurado)
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: "AllowFrontend",
    policy =>
    {
      policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
```

Si tu frontend corre en otro puerto, actualiza esta línea en `Program.cs`.

---

## 🧪 Prueba Rápida con Postman/Thunder Client

### **Request:**
```http
POST http://localhost:5291/api/birds
Content-Type: application/json

{
  "commonName": "Colibrí Esmeralda",
  "scientificName": "Chlorostilbon mellisugus",
  "family": "Trochilidae",
  "conservationStatus": "LeastConcern",
  "notes": "Pequeño colibrí de plumaje verde brillante"
}
```

### **Expected Response:**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

---

## 🔧 Cambios Mínimos en el Frontend (Resumen)

### 1. **Actualiza la URL del servicio:**
```typescript
// bird-http.repository.ts
private readonly API_URL = 'http://localhost:5291/api/birds';
```

### 2. **Asegura que los enums coincidan:**
- El backend espera nombres de enum en inglés (ej: `"Accipitridae"`, `"LeastConcern"`).
- Si tu frontend usa valores en español, necesitas mapear antes de enviar.

### 3. **Verifica el formato del payload:**
```typescript
const payload = {
  commonName: string,
  scientificName: string,
  family: string,  // Valor del enum en inglés
  conservationStatus: string,  // Valor del enum en inglés
  notes: string
};
```

### 4. **Maneja la respuesta:**
El backend devuelve `{ id: "guid" }`, así que actualiza el use-case/service para procesar esto correctamente.

---

## ✅ Checklist de Integración

- [ ] Backend corriendo en `http://localhost:5291`
- [ ] Frontend corriendo en `http://localhost:4200`
- [ ] URL del servicio actualizada a `http://localhost:5291/api/birds`
- [ ] Enums del frontend alineados con el backend (inglés)
- [ ] Probado con Postman/Thunder Client primero
- [ ] Formulario del frontend enviando datos correctos
- [ ] Manejo de errores implementado (400, 401, 500)

---

## 🚨 Troubleshooting

### **Error: CORS**
Si ves `Access-Control-Allow-Origin` en la consola del navegador:
- Verifica que el frontend esté en `http://localhost:4200`
- O actualiza `Program.cs` con el puerto correcto

### **Error: 400 Bad Request**
- Verifica que los nombres de propiedades coincidan exactamente (camelCase)
- Verifica que los valores de enum sean strings válidos del backend

### **Error: Network Failed**
- Verifica que el backend esté corriendo (`dotnet run`)
- Verifica la URL (`http://localhost:5291/api/birds`)

---

## 📝 Notas Adicionales

- **Autenticación:** El endpoint actualmente NO require autenticación (comentado con `// [Authorize]`). Si descomentas esa línea, necesitarás enviar el token JWT en el header `Authorization: Bearer <token>`.
- **Created_By:** El backend automáticamente extrae el usuario del token JWT (`ClaimTypes.NameIdentifier`), o usa `"system"` si no hay autenticación.
- **Base de datos:** Se usa SQLite (`backbird.db`) en la raíz del proyecto backend.

---

## 🎉 ¡Listo!

Con estos cambios, tu formulario del frontend debería poder crear aves en el backend correctamente. 

**Test end-to-end:**
1. Inicia el backend: `dotnet run` (en `backend/bird-app-NET/`)
2. Inicia el frontend: `ng serve` (en la raíz del proyecto Angular)
3. Abre `http://localhost:4200`
4. Llena el formulario de crear ave
5. Envía y verifica que aparece el nuevo ID en la respuesta
6. ¡Ave creada! 🐦
