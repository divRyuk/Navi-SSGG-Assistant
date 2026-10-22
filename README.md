[read_me (1).md](https://github.com/user-attachments/files/33817870/read_me.1.md)
# Navi-SSGG-Assistant
Repositorio dedicado a mi primer projecto de creacion de mi Asistente Virtual para Servicios Generales.
# 🧚‍♂️ NAVI - SSGG RAG Assistant API

![.NET Core](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB-4EA94B?style=for-the-badge&logo=mongodb&logoColor=white)
![Ollama](https://img.shields.io/badge/Ollama-Local_LLM-000000?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=black)
![Status](https://img.shields.io/badge/Status-Sprint%200%20(PoC)-10b981?style=for-the-badge)

**NAVI** es una Web API orientada a microservicios construida en **.NET 8**. Implementa una arquitectura **RAG (Retrieval-Augmented Generation)** diseñada para actuar como soporte de Nivel 0 (Tier-0) en el sector de Servicios Generales (SSGG). Su objetivo es indexar la base de conocimiento del sector (Confluence, PDFs) para interceptar consultas y reducir la escalación de tickets mediante respuestas semánticas precisas.

---

## 🏗 Arquitectura de Alto Nivel

El motor RAG procesa la consulta, extrae el contexto vectorial relevante (vía embeddings) de la Base de Datos Vectorial y orquesta la síntesis del lenguaje mediante un LLM de ejecución local para garantizar la privacidad de los datos.

* **Framework:** .NET 8 (C# 12)
* **Orquestador AI:** Semantic Kernel / LangChain.NET
* **Vector Store:** MongoDB Vector Search
* **Motor LLM:** Ollama (Modelos Open Source locales)
* **Integraciones:** Jira REST API v3, Confluence API

---

## 📂 Estructura del Proyecto (Clean Architecture)

El repositorio sigue los principios de Clean Architecture y separación de responsabilidades:

```text
📦 Navi-SSGG
 ┣ 📂 src
 ┃ ┣ 📂 Navi.Api              # Capa de Presentación (Controladores, Swagger, Middleware)
 ┃ ┣ 📂 Navi.Application      # Casos de Uso, CQRS, Orquestación RAG
 ┃ ┣ 📂 Navi.Domain           # Entidades Core, Interfaces de Repositorios
 ┃ ┗ 📂 Navi.Infrastructure   # Implementación Vector DB (Mongo), LLM Clients (Ollama), Conectores
 ┣ 📂 tests
 ┃ ┗ 📂 Navi.UnitTests        # Pruebas unitarias (xUnit, Moq)
 ┣ 📂 Frontend-UI             # Código fuente del prototipo de interfaz (React/Tailwind)
 ┣ 📂 docs                    # Documentación técnica y reportes (I+D)
 ┣ 📜 Navi.sln                # Solución principal de .NET
 ┗ 📜 README.md
```

---

## 🚀 Requisitos Previos (Prerequisites)

Para compilar y ejecutar el proyecto localmente, necesitas:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community) o Docker Desktop para instanciarlo.
- [Ollama](https://ollama.com/) (Motor local para ejecución de LLMs y Embeddings).
- Git & un IDE (Visual Studio 2022 o VS Code con C# Dev Kit).

---

## ⚙️ Configuración de Servicios Locales

Para que el motor RAG de Navi funcione correctamente, es **obligatorio** tener los servicios de Ollama y MongoDB en ejecución antes de levantar la Web API.

### 1. MongoDB
Levanta una instancia local en el puerto `27017`. Si utilizas Docker:
```bash
docker run --name navi-mongo -d -p 27017:27017 mongo:latest
```

### 2. Ollama
Inicia los modelos locales de generación y embeddings en tu terminal:
```bash
ollama run llama3
ollama pull nomic-embed-text
```

---

## ⚙️ Configuración y Variables de Entorno

Antes de correr el proyecto, debes configurar las credenciales. Crea un archivo `appsettings.Development.json` en `src/Navi.Api/` utilizando la siguiente plantilla ajustada a tu entorno local:

```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017",
    "DatabaseName": "NaviSSGG_Vectors"
  },
  "LlmSettings": {
    "Provider": "Ollama",
    "Endpoint": "http://localhost:11434",
    "EmbeddingModel": "nomic-embed-text",
    "ChatModel": "llama3"
  },
  "Atlassian": {
    "JiraBaseUrl": "https://tu-dominio.atlassian.net",
    "ConfluenceBaseUrl": "https://tu-dominio.atlassian.net/wiki",
    "PersonalAccessToken": "TU_PAT_AQUI"
  }
}
```
> **⚠️ Advertencia de Seguridad (SecOps):** Nunca subas (commit) el archivo `appsettings.Development.json` ni expongas tokens reales en el repositorio. Asegúrate de que este archivo esté ignorado en el `.gitignore`.

---

## 💻 Instalación y Despliegue Local (Getting Started)

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/tu-usuario/navi-ssgg.git
   cd navi-ssgg
   ```

2. **Restaurar paquetes NuGet:**
   ```bash
   dotnet restore Navi.sln
   ```

3. **Compilar la solución:**
   ```bash
   dotnet build Navi.sln --no-restore
   ```

4. **Ejecutar la Web API:**
   ```bash
   cd src/Navi.Api
   dotnet run
   ```

5. **Explorar endpoints:**
   Navega a `https://localhost:7001/swagger` (el puerto puede variar, revisa la salida de la consola) para interactuar con la API a través de la interfaz de Swagger UI.

---

## 📡 Endpoints Principales (API Reference)

* `POST /api/v1/chat/ask` - Recibe el prompt del usuario y ejecuta el flujo RAG.
* `POST /api/v1/knowledge/ingest` - Carga y vectoriza documentos (PDF, CSV).
* `GET /api/v1/tickets/{id}/status` - Consulta el estado de un ticket en Jira.
* `GET /health` - Healthcheck de la API y dependencias externas.

---

## 📚 Documentación del Proyecto (I+D)

Toda la documentación técnica y ejecutiva se encuentra en el directorio [`/docs`](./docs/):
* 📄 [Informe_Ingenieria_Navi.md](./docs/Informe_Ingenieria_Navi.md) - Especificaciones de arquitectura, flujo de datos y requerimientos.

---

## 🤝 Contribución y Flujo de Trabajo

* Este proyecto utiliza **GitFlow**. 
* Toda nueva característica (Feature) debe nacer de la rama `develop`.
* Crea tu rama de trabajo (ej. `feature/JiraIntegration`).
* Realiza commits semánticos (ej. `feat: add ticket query endpoint`, `fix: context overlap bug`).
* Sube un Pull Request apuntando a `develop` para revisión de código.

---
*Desarrollado por Tomas Alvarez - Programa Primer Paso / SSGG (2026)*
