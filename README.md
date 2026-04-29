# KnockProject: Knockin' on Heaven's Door

> "Leave your badge at the door. Every farewell creates an echo from 1973."

KnockProject is an AI-integrated, philosophical web application that serves as a "Digital Memory Wall" for modern farewells. By combining advanced Retrieval-Augmented Generation (RAG) architecture with generative visual models, the project transforms a user's personal farewell into a historical echo rooted in the melancholic aesthetics of 1973.

## 📜 Artistic Vision & Concept

Inspired by the poignant badge-removal scene in Sam Peckinpah’s 1973 film *Pat Garrett & Billy the Kid* (accompanied by Bob Dylan’s "Knockin' on Heaven's Door"), this project explores the universal theme of "removing the badge." Whether quitting a job, leaving a city, or ending a relationship, modern farewells lack a physical ritual. 

KnockProject provides that ritual. Users submit their modern farewells, and the system cross-references them with a curated vector database of 1973 artifacts—Vietnam War letters, anti-war slogans, and melancholic poetry. The result is a personalized, poetic epigraph and a visually generated rusted sheriff's badge, serving as a permanent digital monument to closure.

*For a deep dive into the philosophical framework, please read our [Artist Manifesto](docs/Artist_Manifesto.md).*

## 🏗️ Technical Architecture

The application is built upon a robust, layered architecture integrating modern web technologies with AI models:

1. **Backend API (.NET 8):** A clean, modular RESTful API architecture divided into Core, Infrastructure, and API layers.
2. **Data Layer (PostgreSQL + pgvector):** The heart of the RAG system. A PostgreSQL database utilizing the `pgvector` extension and Entity Framework Core to store and perform Cosine Similarity searches on 1973 historical text embeddings.
3. **AI Services (Infrastructure Layer):**
   - **Embedding Service:** Converts historical texts and user inputs into vector arrays (using OpenAI `text-embedding-3-small`).
   - **LLM Service:** A language model (OpenAI/Anthropic) strictly prompted to act as a 1973 poet, synthesizing the user's input with the retrieved historical vector to generate the epigraph.
   - **Image Generation Service:** A visual model (DALL-E/Stable Diffusion) that generates a 1973 analog film, rusty sheriff badge image based on strict aesthetic prompt templates.
4. **Frontend (Vanilla HTML/CSS/JS):** A minimalist, dark-themed, 1973 analog-style user interface designed to maximize emotional resonance without modern UI distractions.

## 🚀 Setup & Installation

### Prerequisites
- .NET 8 SDK
- Docker (for PostgreSQL & pgvector)
- API Keys for OpenAI/Anthropic (stored in `appsettings.Development.json` or User Secrets)

### Running Locally
1. **Database:** Start the pgvector database via Docker:
   ```bash
   docker-compose up -d
   ```
2. **Backend:** Navigate to the API folder and run the server:
   ```bash
   cd KnockProject.API
   dotnet run
   ```
3. **Frontend:** Open `KnockProject.UI/index.html` in any modern web browser.

## 🤝 The Team

This project was a collaborative effort requiring the synergy of three distinct roles:

- **Ömer Faruk Önder** - *System Architect & Data Layer* (RAG architecture, PostgreSQL/pgvector integration, .NET 8 API)
- **Şükran Hilal Hocaoğlu** - *AI Orchestrator & Prompt Engineer* (1973 dataset curation, LLM Persona engineering, DALL-E aesthetic templates)
- **Levent Can Ceylan** - *Experience Designer & Philosophical Lead* (Artist Manifesto, UI/UX conceptualization, 1973 frontend aesthetics)

## 📚 Academic Honesty & References

- **Dataset:** Historical texts compiled from public domain Vietnam War archives, historical protest literature, and referenced Bob Dylan lyrics.
- **AI Models:** Vector embeddings and text generation powered by OpenAI API. Image generation powered by DALL-E/Stable Diffusion API.
- **Inspiration:** *Pat Garrett & Billy the Kid* (1973), directed by Sam Peckinpah.
