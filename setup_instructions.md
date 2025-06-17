# ⚙️ Project Setup: AI-Powered Document Classifier

This document provides a complete step-by-step guide to set up and run the **AI-Powered Document Classifier** using ML.NET, LLMs (OpenAI/LLaMA), Redis, and React.

---

## 🧱 System Requirements

Ensure the following tools and dependencies are installed:

| Tool/Dependency       | Version / Details                     |
|-----------------------|----------------------------------------|
| .NET SDK              | .NET 8.0+                              |
| Node.js & npm         | v18+                                   |
| Redis Server          | v6 or above                            |
| SQL Server / ChromaDB | Any production-ready version           |
| OS                    | Linux (preferred) or Windows           |
| GPU (optional)        | For LLaMA/OpenAI local inference       |

---

## 🛠️ Step-by-Step Setup Instructions

---

### 🔹 Step 1: Clone the Repository

```bash
git clone https://github.com/SidhantRepo/document-classifier-ai.git
cd document-classifier-ai
```

###🔹 Step 2: Setup the .NET Backend (DocumentClassifier.API)
``` bash
cd DocumentClassifier.API
dotnet restore
dotnet build


Run the API: dotnet run 

📍 The backend will run by default at https://localhost:5001

```

### 🔹 Step 3: Place the ML.NET Trained Model
``` bash 
Copy the ML.NET trained model file model.zip into the following path:

DocumentClassifier.API/MLModel/model.zip


Ensure your model loading code points to the correct path: var modelPath = Path.Combine(Environment.CurrentDirectory, "MLModel", "model.zip");

```

### 🔹 Step 4: Setup SQL Server or ChromaDB
``` bash

Create a database called DocumentClassifierDB

Add your SQL Server connection string in appsettings.json: "ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=DocumentClassifierDB;User Id=your-user;Password=your-password;"
}

If using ChromaDB, ensure it’s running and accessible with initialized collection/schema.

```

### 🔹 Step 5: Setup the Frontend (React + Tailwind)

``` bash
cd ../ClientApp
npm install
npm run dev

📍 The frontend will run by default at http://localhost:3000
```


### 🔹 Step 6: Configure OpenAI / LLaMA (Optional)

``` bash
Option A: OpenAI
In appsettings.json, set your OpenAI credentials:

"OpenAI": {
  "ApiKey": "your-api-key",
  "Endpoint": "https://api.openai.com/v1"
}


Option B: LLaMA
If you are using a locally hosted LLaMA model:

"Llama": {
  "ModelPath": "/models/llama-3.bin"
}

Ensure your system has a GPU and enough memory for running local LLM inference.
```

### 🔹 Step 7: Supported Document Types
``` bash

The app currently supports these input formats for classification and Q&A:

.pdf (searchable)

.txt

.docx

Upload documents via:

React UI

API endpoint: POST /api/training/upload

```

### 🔹 Step 8: Upload Documents and Trigger Training
``` bash

Use the API to upload documents:
POST /api/training/upload

The system:

1. Extracts text from the uploaded file

2. Stores it for training and vector embedding

3. Triggers classification model or passes it to LLM


```


### Step 9: Run Deployment Script (Linux)
``` bash 

If you're on a Linux server or VM, use the deploy.sh script to auto-install dependencies and start the app:
cd Scripts
sudo bash deploy.sh

This will:

Install Redis

Install .NET runtime

Install Node.js

Start backend and frontend services

```

## ✅ Post-Setup Checklist

``` bash

 Repo cloned and project folders initialized

 model.zip placed correctly in MLModel folder

 Redis and SQL Server / ChromaDB running

 .NET backend is running without errors

 React frontend is accessible at http://localhost:3000

 Document upload and classification tested

 LLM integration (optional) tested with valid API key

 ```

## 🧪 API & UI Test Flow

``` bash 
Action	                        Endpoint or UI Module
Upload Document	                /api/training/upload or Upload UI
Trigger Classification	        Auto after upload or manual button
Get Training Status	            /api/training/status
Start Training	                /api/training/start
Ask Question (LLM)	            /api/qa/ask
View Chat History	            Redis Viewer or Chat UI

```