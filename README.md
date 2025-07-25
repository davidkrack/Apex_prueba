# Sistema de Gestión de Facturas 📊

**Desarrollador:** David Candia  
**Tecnologías:** .NET 8, React 18, Entity Framework Core, SQLite, Material-UI  
**Patrón de Diseño:** Repository Pattern + Service Layer  
**Estado:** ✅ **COMPLETADO Y FUNCIONAL**

## 🎯 Descripción del Proyecto

Sistema web completo para la gestión de facturas que permite cargar, procesar, validar y administrar facturas desde archivos JSON. Incluye funcionalidades avanzadas de búsqueda, reportes ejecutivos, gestión de notas de crédito, generación de PDFs y dashboard interactivo con autenticación.

### ✨ Características Principales

- ✅ **Autenticación completa** con login personalizado (admin/admin)
- ✅ **Carga de datos inteligente** desde archivos JSON con validación automática
- ✅ **Gestión avanzada de facturas** con búsqueda y filtros múltiples
- ✅ **Sistema de notas de crédito** con validaciones de saldo automáticas
- ✅ **Centro de reportes ejecutivos** con 4 tipos de análisis
- ✅ **Generación de reportes en PDFs** (solo es muestra para mostrar que es posible)
- ✅ **Dashboard interactivo** 
- ✅ **Interfaz 100% responsive** con Material-UI y temas personalizados
- ✅ **API REST completa** documentada con Swagger
- ✅ **Validaciones automáticas** de consistencia y estados de facturas
- ✅ **Doble versión de BD** (SQLite y preparada para SQL Server)

## 🏗️ Arquitectura del Sistema

```
PROYECTO_APEX2/
├── 📁 invoice-management-frontend/     # Frontend React + Material-UI
│   ├── 📁 public/images/              # Logo Apex Group (SVG)
│   ├── 📁 src/
│   │   ├── 📁 components/             # Componentes reutilizables
│   │   │   ├── Login.jsx              # Sistema de autenticación
│   │   │   └── Navigation.jsx         # Barra de navegación
│   │   ├── 📁 pages/                  # Páginas principales
│   │   │   ├── Dashboard.jsx          # Dashboard ejecutivo
│   │   │   ├── Invoices.jsx           # Gestión de facturas
│   │   │   ├── CreditNotes.jsx        # Gestión de NC
│   │   │   ├── Reports.jsx            # Centro de reportes + PDF
│   │   │   └── LoadData.jsx           # Carga de datos JSON
│   │   └── 📁 services/
│   │       └── api.js                 # Configuración Axios + endpoints
├── 📁 InvoiceManagement/              # Backend SQLite (Versión 1)
│   └── 📁 InvoiceManagement.Api/
│       ├── 📁 Controllers/            # Endpoints API REST
│       ├── 📁 Data/                   # DbContext + EF Core
│       ├── 📁 Models/                 # Entidades de BD
│       └── 📁 Services/               # Lógica de negocio
├── 📁 InvoiceManagement SQL SERVER/   # Backend SQL Server (Versión 2)
│   ├── 📁 Scripts/                    # Scripts SQL profesionales
│   │   ├── 01_CreateDatabase.sql      # Creación de BD y tablas
│   │   └── 02_SampleData.sql          # Datos de ejemplo
│   └── 📁 InvoiceManagement.Api/      # API idéntica con BD diferente
├── 📄 bd_exam_invoices.json           # Archivo de datos de prueba
├── 📄 .gitignore                      # Configuración Git
└── 📄 README.md                       # Este archivo
```

### 🎨 Patrón de Diseño Implementado

**Repository Pattern + Service Layer + MVC:**
- **Models:** Entidades de BD (Customer, Invoice, InvoiceDetail, CreditNote, InvoicePayment)
- **Data:** DbContext y configuraciones de Entity Framework Core
- **Services:** Lógica de negocio (InvoiceDataService con validaciones)
- **Controllers:** 4 controladores REST (Data, Invoices, CreditNotes, Reports)
- **Frontend:** Arquitectura por componentes con services centralizados

## 🚀 Versiones de Base de Datos

### **Versión 1: SQLite (Desarrollo)**
- **Ubicación:** `InvoiceManagement/`
- **Base de datos:** SQLite embebida (`invoices.db`)
- **Uso:** Desarrollo, demos y pruebas
- **Puerto:** 5142
- **Ventajas:** Sin instalación, portátil, rápida

### **Versión 2: SQL Server (Producción)**
- **Ubicación:** `InvoiceManagement SQL SERVER/`
- **Base de datos:** SQLite adaptada + Scripts SQL Server
- **Scripts:** Completos para SQL Server empresarial
- **Puerto:** 5143 (configurable)
- **Ventajas:** Scripts profesionales, escalable

> **📋 Nota Técnica:** La Versión 2 incluye scripts SQL completos para SQL Server real, pero usa SQLite adaptada para facilitar la demostración sin requerir instalación de SQL Server Enterprise.

## 📋 Requisitos del Sistema

### Backend (.NET)
- ✅ .NET 8 SDK o superior
- ✅ Visual Studio Code o Visual Studio 2022
- ✅ Entity Framework Core Tools

### Frontend (React)
- ✅ Node.js 16+ 
- ✅ npm o yarn
- ✅ Navegador moderno (Chrome, Firefox, Edge)

### Opcional (SQL Server)
- ✅ SQL Server 2019+ o SQL Server Express
- ✅ SQL Server Management Studio

## ⚡ Instalación Rápida

### 🚀 Método Express (5 minutos)

```bash
# 1. Clonar repositorio
git clone https://github.com/davidkrack/Apex_prueba.git
cd Apex_prueba

# 2. Backend (Versión SQLite)
cd InvoiceManagement/InvoiceManagement.Api
dotnet restore
dotnet ef database update
dotnet run &

# 3. Frontend (Nueva terminal)
cd ../../invoice-management-frontend
npm install
npm start

# 4. Abrir navegador
# Backend: http://localhost:5142/swagger
# Frontend: http://localhost:3000
```

## 🛠️ Instalación Detallada

### 1. Clonar y Preparar
```bash
git clone https://github.com/davidkrack/Apex_prueba.git
cd Apex_prueba
```

### 2. Configurar Backend (Versión SQLite)

```bash
# Navegar al proyecto principal
cd InvoiceManagement/InvoiceManagement.Api

# Restaurar dependencias NuGet
dotnet restore

# Crear base de datos SQLite
dotnet ef migrations add InitialCreate
dotnet ef database update

# Ejecutar API
dotnet run
```

**✅ Resultado esperado:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5142
      Now listening on: https://localhost:7xxx
```

**🔗 URLs del Backend:**
- **API:** `http://localhost:5142`
- **Swagger:** `http://localhost:5142/swagger`

### 3. Configurar Frontend (React + Material-UI)

```bash
# Nueva terminal - ir al frontend
cd invoice-management-frontend

# Instalar dependencias
npm install

# Instalar librerías PDF (si no están)
npm install jspdf

# Ejecutar en modo desarrollo
npm start
```

**✅ Resultado esperado:**
```
webpack compiled with 0 errors
Local:            http://localhost:3000
On Your Network:  http://192.168.1.xxx:3000
```

**🔗 URL del Frontend:** `http://localhost:3000`

### 4. Configurar Versión SQL Server (Opcional)

```bash
# Ir a la versión SQL Server
cd "InvoiceManagement SQL SERVER/InvoiceManagement.Api"

# Instalar dependencias
dotnet restore

# Para SQL Server real, cambiar connection string en appsettings.json:
# "DefaultConnection": "Server=localhost;Database=InvoiceManagementDB;Trusted_Connection=true"

# Crear migraciones
dotnet ef migrations add InitialCreateSqlServer
dotnet ef database update

# Ejecutar en puerto alternativo
dotnet run --urls="http://localhost:5143"
```

## 🔧 Configuración Avanzada

### Cambiar Puertos del Backend

**Archivo:** `Properties/launchSettings.json`
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:5200",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Configurar SQL Server Real

**Archivo:** `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InvoiceManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**Ejecutar Scripts:**
1. Abrir SQL Server Management Studio
2. Ejecutar `Scripts/01_CreateDatabase.sql`
3. Ejecutar `Scripts/02_SampleData.sql`

### Configurar CORS Personalizado

**Archivo:** `Program.cs`
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://tudominio.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## 🔐 Sistema de Autenticación

### Credenciales de Acceso
- **Usuario:** `admin`
- **Contraseña:** `admin`

### Características del Login
- ✅ **Logo personalizado** de Apex Group (SVG)
- ✅ **Validación en tiempo real** de credenciales
- ✅ **Sesión persi# Sistema de Gestión de Facturas 📊

**Desarrollador:** David Krack  
**Tecnologías:** .NET 8, React 18, Entity Framework Core, SQLite, Material-UI  
**Patrón de Diseño:** Repository Pattern + Service Layer  

## 🎯 Descripción del Proyecto

Sistema web completo para la gestión de facturas que permite cargar, procesar, validar y administrar facturas desde archivos JSON. Incluye funcionalidades de búsqueda, reportes, gestión de notas de crédito y dashboard ejecutivo.

### Características Principales

- ✅ **Carga de datos** desde archivos JSON con validación automática
- ✅ **Gestión de facturas** con búsqueda y filtros avanzados
- ✅ **Notas de crédito** con validaciones de saldo
- ✅ **Reportes ejecutivos** y estadísticas detalladas
- ✅ **Dashboard interactivo** con métricas en tiempo real
- ✅ **Autenticación** simple (admin/admin)
- ✅ **Interfaz responsive** con Material-UI
- ✅ **API REST** documentada con Swagger

## 🏗️ Arquitectura del Sistema

```
PROYECTO/
├── 📁 invoice-management-frontend/     # Frontend React
├── 📁 InvoiceManagement/              # Backend SQLite (Versión 1)
├── 📁 InvoiceManagement SQL SERVER/   # Backend SQL Server (Versión 2)
└── 📄 README.md
```

### Patrón de Diseño Implementado

**Repository Pattern + Service Layer:**
- **Models:** Entidades de base de datos (Customer, Invoice, etc.)
- **Data:** DbContext y configuraciones de Entity Framework
- **Services:** Lógica de negocio (InvoiceDataService)
- **Controllers:** Endpoints de API REST
- **Frontend:** Componentes React con services para API

## 🚀 Versiones Disponibles

### **Versión 1: SQLite**
- **Ubicación:** `InvoiceManagement/`
- **Base de datos:** SQLite embebida
- **Uso:** Desarrollo y demos
- **Puerto:** 5142

### **Versión 2: SQL Server (Adaptada)**
- **Ubicación:** `InvoiceManagement SQL SERVER/`
- **Base de datos:** SQLite (adaptada para demostración)
- **Scripts SQL:** Incluidos para SQL Server real
- **Puerto:** 5143 (configurable)

> **Nota:** La Versión 2 incluye scripts SQL completos para SQL Server, pero usa SQLite para facilitar la demostración sin requerir instalación de SQL Server.

## 📋 Requisitos Previos

### Backend (.NET)
- .NET 8 SDK o superior
- Visual Studio Code o Visual Studio 2022

### Frontend (React)
- Node.js 16+ 
- npm o yarn

## 🛠️ Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone https://github.com/davidkrack/Apex_prueba.git
cd Apex_prueba
```

### 2. Configurar Backend (Versión 1 - SQLite)

```bash
# Navegar al proyecto backend
cd InvoiceManagement/InvoiceManagement.Api

# Restaurar dependencias
dotnet restore

# Crear migraciones (si no existen)
dotnet ef migrations add InitialCreate

# Aplicar migraciones
dotnet ef database update

# Ejecutar el proyecto
dotnet run
```

**URL Backend:** `https://localhost:7XXX` o `http://localhost:5142`  
**Swagger:** `http://localhost:5142/swagger`

### 3. Configurar Frontend (React)

```bash
# En una nueva terminal, navegar al frontend
cd invoice-management-frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm start
```

**URL Frontend:** `http://localhost:3000`

### 4. Configurar Versión SQL Server (Opcional)

```bash
# Navegar al proyecto SQL Server
cd "InvoiceManagement SQL SERVER/InvoiceManagement.Api"

# Restaurar dependencias
dotnet restore

# Crear migraciones
dotnet ef migrations add InitialCreateSqlServer

# Aplicar migraciones
dotnet ef database update

# Ejecutar el proyecto (puerto diferente)
dotnet run --urls="http://localhost:5143"
```

## 📊 Scripts de Base de Datos

### Para SQL Server Real

Los scripts se encuentran en `InvoiceManagement SQL SERVER/Scripts/`:

1. **`01_CreateDatabase.sql`** - Creación de base de datos y tablas
2. **`02_SampleData.sql`** - Datos de ejemplo

```sql
-- Ejecutar en SQL Server Management Studio
-- 1. Ejecutar 01_CreateDatabase.sql
-- 2. Ejecutar 02_SampleData.sql
```

## 🔧 Configuración de Puertos

### Backend
- **Versión SQLite:** Puerto 5142
- **Versión SQL Server:** Puerto 5143 (configurable)

### Frontend
- **React:** Puerto 3000

### Cambiar Puerto del Backend
En `Properties/launchSettings.json`:
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5142",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## 🔐 Credenciales de Acceso

**Usuario:** `admin`  
**Contraseña:** `admin`

## 📖 Uso del Sistema

### 1. Cargar Datos Iniciales

1. Accede a **"Cargar Datos"** en la navegación
2. Usa el archivo predeterminado `bd_exam_invoices.json`
3. Haz clic en **"Cargar Archivo"**

### 2. Explorar Funcionalidades

- **Dashboard:** Métricas generales y KPIs
- **Facturas:** Búsqueda y filtrado de facturas
- **Notas de Crédito:** Gestión de NC con validaciones
- **Reportes:** Estadísticas y reportes ejecutivos

## 🧪 Estructura de Datos

### Factura (JSON)
```json
{
  "invoice_number": 1,
  "invoice_date": "2025-01-14",
  "invoice_status": "issued",
  "total_amount": 265019,
  "payment_status": "Pending",
  "customer": {
    "customer_run": "13075795-2",
    "customer_name": "Juanita Hugh",
    "customer_email": "jhugh0@xinhuanet.com"
  },
  "invoice_detail": [...],
  "invoice_credit_note": [...]
}
```

### Validaciones Automáticas

- ✅ **Consistencia:** Suma de productos = Total declarado
- ✅ **Estados:** Cálculo automático según notas de crédito
- ✅ **Pagos:** Determinación de estado según fechas
- ✅ **Duplicados:** Prevención por número de factura único

## 📡 API Endpoints

### Datos
- `POST /api/Data/load-from-file` - Cargar desde archivo
- `POST /api/Data/load-from-json` - Cargar desde JSON directo

### Facturas
- `GET /api/Invoices` - Listar con filtros
- `GET /api/Invoices/{id}` - Obtener por ID
- `GET /api/Invoices/statistics` - Estadísticas

### Notas de Crédito
- `GET /api/CreditNotes` - Listar todas
- `POST /api/CreditNotes` - Crear nueva
- `DELETE /api/CreditNotes/{id}` - Eliminar

### Reportes
- `GET /api/Reports/executive-dashboard` - Dashboard ejecutivo
- `GET /api/Reports/overdue-invoices` - Facturas vencidas
- `GET /api/Reports/payment-status-summary` - Resumen de pagos
- `GET /api/Reports/inconsistent-invoices` - Facturas inconsistentes

## 🧩 Componentes Frontend

### Páginas Principales
- **Dashboard.jsx** - Métricas y KPIs
- **Invoices.jsx** - Gestión de facturas
- **CreditNotes.jsx** - Gestión de notas de crédito
- **Reports.jsx** - Centro de reportes
- **LoadData.jsx** - Carga de datos

### Componentes
- **Navigation.jsx** - Barra de navegación
- **Login.jsx** - Autenticación

### Servicios
- **api.js** - Configuración de Axios y endpoints

## 🔍 Testing

### Probar Backend
```bash
# Ejecutar desde el directorio del API
dotnet test
```

### Probar Frontend
```bash
# Ejecutar desde el directorio del frontend
npm test
```

### Probar API con Swagger
1. Ejecuta el backend
2. Ve a `http://localhost:5142/swagger`
3. Prueba los endpoints directamente

## 🚨 Solución de Problemas

### Error: "Cannot connect to database"
```bash
# Recrear base de datos
dotnet ef database drop
dotnet ef database update
```

### Error: "Port already in use"
```bash
# Cambiar puerto en launchSettings.json
# O usar comando específico
dotnet run --urls="http://localhost:5200"
```

### Error: "CORS policy"
- Verificar que el frontend esté en puerto 3000
- Verificar configuración CORS en Program.cs

### Frontend no conecta con Backend
1. Verificar que el backend esté corriendo en puerto 5142
2. Verificar URL en `src/services/api.js`
3. Verificar configuración CORS

## 📦 Dependencias Principales

### Backend (.NET)
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```

### Frontend (React)
```json
{
  "dependencies": {
    "react": "^18.2.0",
    "axios": "^1.6.0",
    "react-router-dom": "^6.8.0",
    "@mui/material": "^5.15.0",
    "@mui/icons-material": "^5.15.0",
    "@emotion/react": "^11.11.0",
    "@emotion/styled": "^11.11.0"
  }
}
```

## 🎨 Características Técnicas

### Backend Features
- **Entity Framework Core** con Code First
- **Repository Pattern** para abstracción de datos
- **Service Layer** para lógica de negocio
- **Swagger** para documentación automática
- **CORS** configurado para React
- **Logging** integrado
- **Validaciones** automáticas de datos

### Frontend Features
- **Material-UI** para componentes
- **Responsive Design** para móviles
- **State Management** con React Hooks
- **Routing** con React Router
- **Axios** para llamadas HTTP
- **Error Handling** centralizado
- **Loading States** en todas las operaciones

## 🏆 Logros del Proyecto

✅ **Cumplimiento 100%** de requerimientos del PDF  
✅ **Dos versiones** de base de datos implementadas  
✅ **Frontend completo** con React y Material-UI  
✅ **API REST** completa y documentada  
✅ **Validaciones automáticas** de consistencia  
✅ **Reportes ejecutivos** implementados  
✅ **Sistema de autenticación** funcional  
✅ **Código limpio** y bien estructurado  

## 📞 Contacto

**Desarrollador:** David candia
**Email:** davidn.candia96@gmail.com  
**GitHub:** https://github.com/davidkrack  
**Proyecto:** https://github.com/davidkrack/Apex_prueba  

---

## 📝 Notas Adicionales

- El sistema está diseñado para ser escalable y mantenible
- Se implementó el patrón Repository + Service Layer para separación de responsabilidades
- La validación de consistencia se realiza automáticamente al cargar datos
- Los reportes se generan en tiempo real desde la base de datos
- El frontend es completamente responsive y funciona en dispositivos móviles

**¡Gracias por revisar este proyecto!** 🚀
