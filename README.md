# ⚡ AI-Powered Energy Consumption Prediction System

> **Courses:** Advanced Web Programming | Database Management Systems | Service Oriented Architecture (SOA) | Machine Learning

---

## 🇬🇧 English Documentation

### 📖 About the Project
This project is an integrated software engineering solution designed to predict the next 24-hour electricity demand using historical consumption data and meteorological data. It is built upon a **Microservices (SOA)** architecture, supported by **Machine Learning (AI)**, and presented via a modern web interface.

### 🏗️ Monorepo Architecture
The project follows a Monorepo structure, integrating four main disciplines:

| Folder | Description | Technology |
| :--- | :--- | :--- |
| 📂 **Machine-Learning** | Data scraping, analysis, and the AI prediction API. | Python, Flask, Playwright |
| 📂 **SOA** | Backend API and service orchestration. | .NET Core / Node.js |
| 📂 **WEB** | User interface and dashboard. | ASP.NET MVC / Bootstrap |
| 📂 **DBMS** | Database schemas and SQL files. | MySQL |

### 🚀 Setup & Installation

#### 1. Machine Learning Service (Python)
Required to generate predictions.
```bash
cd Machine-Learning
pip install -r requirements.txt
python api/ml_api.py
```
---
# ⚡ Akıllı Şehirler İçin Enerji Tüketim Tahmin ve Yönetim Sistemi

> **Alanlar:** İleri Web Programlama | Veri Tabanı Yönetim Sistemleri | Servis Odaklı Mimari (SOA) | Makine Öğrenmesi

Bu proje, Türkiye geneli elektrik tüketim verilerini ve hava durumu verilerini analiz ederek, Yapay Zeka (Makine Öğrenmesi) desteğiyle gelecek 24 saatlik enerji talebini tahmin eden ve bunu modern bir web arayüzü ile sunan bütünleşik bir yazılım mühendisliği projesidir.

---

## 🏗️ Proje Mimarisi (Monorepo)

Proje, farklı disiplinlerin bir araya geldiği **4 ana modülden** oluşmaktadır:

| Klasör | Açıklama | Teknoloji |
| :--- | :--- | :--- |
| 📂 **Machine-Learning** | Veri toplama, analiz ve tahmin modeli servisi. | Python, Flask, Playwright, Scikit-Learn |
| 📂 **SOA** | Servislerin yönetimi ve Backend API. | .NET Core / Node.js |
| 📂 **WEB** | Son kullanıcı arayüzü ve Dashboard. | ASP.NET MVC / Bootstrap |
| 📂 **DBMS** | Veri tabanı tasarımları ve SQL dosyaları. | MySQL |

---

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları takip edin.

### 1. Makine Öğrenmesi (ML) Servisi
Modelin çalışması ve tahmin üretmesi için bu servisin ayakta olması gerekir.

```bash
# 1. Klasöre girin
cd Machine-Learning

# 2. Gerekli kütüphaneleri yükleyin
pip install -r requirements.txt

# 3. API Servisini başlatın
python api/ml_api.py

